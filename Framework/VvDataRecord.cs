using System;
using System.ComponentModel; // za IEditable 
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using XSqlDbType     = MySql.Data.MySqlClient.MySqlDbType;
using System.ComponentModel;
using System.Reflection;
#endif

[Serializable]
public abstract class VvDataRecord : ICloneable, IEditableObject
{

   #region Fieldz & abstract properties

   protected bool editInProgress = false;

   public bool IsFillingFromJoinReader { get; set; }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualRecID
   {
      get;
      set;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string VirtualRecordName
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public virtual string VirtualRecordName2 { get { return VirtualRecordName.FirstLetterToUpperCase(); } }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string VirtualIDstring
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public virtual string VirtualLegacyRecordPreffix
   {
      get { return "NotOverrided!"; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract DateTime VirtualAddTS { get; }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract DateTime VirtualModTS { get; }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string   VirtualAddUID{ get; }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string   VirtualModUID{ get; }
   // SkyNews 
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint     VirtualLanSrvID{ get; set;}
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint     VirtualLanRecID{ get; set;}

   public abstract bool IsSifrar
   {
      get;
   }

   /// <summary>
   /// Da li je auto sifra (npr Kplan nije, docim Person, Osred, ... jesu)
   /// </summary>
   public abstract bool IsAutoSifra
   {
      get;
   }

   /// <summary>
   /// Da li je string ili uint sifra
   /// </summary>
   public abstract bool IsStringAutoSifra
   {
      get;
   }

   public abstract bool IsDocument
   {
      get;
   }

   public abstract bool IsDocumentLike
   {
      get;
   }

   public abstract bool IsPolyDocument
   {
      get;
   }

   public abstract bool IsTrans
   {
      get;
   }

   public abstract bool IsArhivable
   {
      get;
   }

   public bool VvArhivedVersionsExists 
   { 
      get 
      { 
         return VvArhivedVersionsCount > 0; 
      } 
   }
   
   public uint VvArhivedVersionsCount  
   { 
      get; 
      set; 
   }
   
   public /*abstract*/ bool HasSenseCheckingForArhivaVersionExistance
   {
      get { return (IsArhivable && VirtualAddTS != VirtualModTS); }
   }

   public abstract IVvDao VvDao
   {
      get;
   }

   public abstract VvSQL.RecordSorter DefaultSorter
   {
      get;
   }

   public abstract VvSQL.RecordSorter[] Sorters
   {
      get;
   }

   public abstract /*virtual*/ bool IsSomeOfPossibleForeignKeyFieldsChanged { get  /*return (false)*/; }

   public virtual bool IsTwinTT { get { return (false); } }

   public virtual bool IsSplitTT { get { return (false); } }

   public virtual bool IsProzivodnjaUlazTT { get { return (false); } }

   public virtual bool IsExtendable { get { return (false); } }

   public virtual bool IsExtendable4Read { get { return (false); } }

   public virtual bool IsExtender { get { return (false); } }

   public virtual bool IsCacheable { get { return (false); } } // Rtrans overrides as true 

   public virtual bool IsCacheForStatus { get { return (false); } } // ArtStat overrides as true 

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public virtual VvDataRecord VirtualExtenderRecord { get { return (null); } set {} }

   public virtual uint UintSifraRootNum    { get { return (0); } } // Kupdob overrides 
   public virtual uint UintSifraBaseFactor { get { return (0); } } // Kupdob overrides 

   public virtual bool VirtualIsLocked { get { return false; } } // Placa overrides 

   public virtual bool IsPrjkt_NonPUG_DataRecord { get { return (false); } } // User, Prjkt, DevTec, Htrans, Prvlg, SkyRule overrides AS true 

   #endregion Fieldz & abstract properties

   #region ABSTRACT methods (no bodies)

   // Implements ICloneable 
   public abstract object Clone();

   // Implements IEditableObject 
   public abstract void BeginEdit();
   public abstract void CancelEdit(); 
   public abstract void EndEdit(); 

   public abstract bool EditedHasChanges();
   public abstract void RestoreBackupData();

   public abstract void Memset0(uint recID);

   public abstract object[] SorterCurrVal(VvSQL.SorterType sortType);

   //public virtual string ToSifrarString(VvSQL.SorterType_Dokument sifrarType) { return "ToSifrarString as VIRTUAL"; }

   public abstract VvDataRecord VvDataRecordFactory();

   // 04.12.2014: 
   public abstract void TakeCurrentDataFrom(VvDataRecord vvDataRecord);

   public abstract void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord);

   #endregion ABSTRACT methods (no bodies)

   #region Generic methods

   public void Generic_RestoreBackupData<T>(ref T currentData, ref T backupData)
   {
      currentData = backupData;
   }

   public void Generic_BeginEdit<T>(ref T currentData, ref T backupData)
   {
      //Console.WriteLine("Start BeginEdit");
      if(this.editInProgress == false)
      {
         backupData     = currentData;
         editInProgress = true;
      }
   }

   public void Generic_CancelEdit<T>(ref T currentData, ref T backupData)
   {
      //Console.WriteLine("Start CancelEdit");
      if(editInProgress == true)
      {
         currentData    = backupData;
         editInProgress = false;
      }
   }

   public void Generic_EndEdit<T>(ref T currentData, ref T backupData)
   {
      if(editInProgress == true)
      {
         backupData     = default(T);
         editInProgress = false;
      }
   }

   public bool Generic_EditedHasChanges<T>(T currentData, T backupData)
   {
      return !(currentData.Equals(backupData));
   }

   public void Generic_CloneData<T>(T currentData, T backupData, ref T newCurrentData, ref T newBackupData)
   {
      newCurrentData = currentData;
      newBackupData  = backupData;
   }

   public void Generic_CloneResultData<T>(T resultData, ref T newResultData)
   {
      newResultData = resultData;
   }

   public VvDataRecord CreateNewRecordAndCloneItComplete()
   {
      VvDataRecord vvDataRecord = (VvDataRecord)this.Clone();

      if(this.IsExtendable || IsExtendable4Read)
      {
         // TODO: provjeri jel ovo radi, nisi li pobrkao davaoca i primaoca? 
         ((IVvExtendableDataRecord)vvDataRecord).TakeExtenderDataFrom(this.VirtualExtenderRecord);
      }

      if(this.IsDocument)
      {
         ((VvDocumentRecord)vvDataRecord).TakeTransesFrom(this as VvDocumentRecord);
      }

      if(this.IsPolyDocument)
      {
         ((VvPolyDocumRecord)vvDataRecord).TakeTransesFrom2(this as VvPolyDocumRecord);
         ((VvPolyDocumRecord)vvDataRecord).TakeTransesFrom3(this as VvPolyDocumRecord);
      }

      return vvDataRecord;
   }

   public void TakeInBackupData_CurrentDataFrom_Complete(VvDataRecord incoming_vvDataRecord)
   {
      this.TakeInBackupData_CurrentDataFrom(incoming_vvDataRecord);

      if(this.IsExtendable || IsExtendable4Read)
      {
         // TODO: provjeri jel ovo radi, nisi li pobrkao davaoca i primaoca? 
         ((IVvExtendableDataRecord)this).TakeExtender_Backup_DataFrom(incoming_vvDataRecord.VirtualExtenderRecord);
      }

      // 23.10.2019: NE! BackupData ti treba samo za zaglavlje i sifrare. 
      // transovi nikada ne idu kao RWT nego uvijek kao ADD               

      //if(this.IsDocument)
      //{
      //   ((VvDocumentRecord)this).TakeTranses_Backup_DataFrom(incoming_vvDataRecord as VvDocumentRecord);
      //}
      //
      //if(this.IsPolyDocument)
      //{
      //   ((VvPolyDocumRecord)this).TakeTranses2_Backup_DataFrom(incoming_vvDataRecord as VvPolyDocumRecord);
      //   ((VvPolyDocumRecord)this).TakeTranses3_Backup_DataFrom(incoming_vvDataRecord as VvPolyDocumRecord);
      //}
   }

   #endregion Generic methods

   #region GetHashCode(), Equals() - uses column $fieldID to look up objects in liveObjectMap
   
   public override int GetHashCode()
   {
      return VirtualRecID.GetHashCode();
   }

   public override bool Equals(object obj)
   {
      VvDataRecord o2 = obj as VvDataRecord;

      if(o2 == null) return false;

      if(this.IsTrans && VirtualRecID == 0 && o2.VirtualRecID == 0) // ovo treba kod GetDgvFields.document_rec.InvokeTransRemove() // 09.08.2009 
      {
         return ((VvTransRecord)this).VirtualT_Serial == ((VvTransRecord)o2).VirtualT_Serial;
      }

      return(VirtualRecID      == o2.VirtualRecID && 
             VirtualRecordName == o2.VirtualRecordName);
   }

   #endregion

   #region All About ARHIVA

   #region Fieldz, Propertiez, ...

   public const string ArhRecNameExstension = "_ar";

   private VvArhivaStruct arhivaData;
   public  VvArhivaStruct TheArhivaData
   {
      get { return arhivaData        ; }
      set {        arhivaData = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string VirtualRecordNameArhiva
   {
      get;
   }

   #endregion Fieldz, Propertiez, ...

   #region VvArhivaStruct Propertiz

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public uint OrigRecID
   {
      get { return arhivaData._origRecID; }
      set {        arhivaData._origRecID = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public uint RecVer
   {
      get { return arhivaData._recVer; }
      set {        arhivaData._recVer = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public string ArAction
   {
      get { return arhivaData._arAction; }
      set {        arhivaData._arAction = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public DateTime ArTS
   {
      get { return arhivaData._arTS; }
      set {        arhivaData._arTS = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public string ArUID
   {
      get { return arhivaData._arUID; }
      set {        arhivaData._arUID = value; }
   }

   public static string[] metadataColumnNames = { "addUID", "modUID", "addTS", "modTS", "lanSrvID", "lanRecID" };
   public static string[] arhivaColumnNames   = { "origRecID", "recVer", "arAction", "arTS", "arUID" };

   #endregion VvArhivaStruct Propertiz


   #region Methodz

   private VvArhivaStruct CreateAndInitArhivaStruct(XSqlConnection conn, string _arAction)
   {
      arhivaData = new VvArhivaStruct();

      arhivaData._origRecID = VirtualRecID;
      arhivaData._recVer    = VvDaoBase.GetNextArhivaRecordVersion(/*ZXC.TheVvForm.TheDbConnection*/conn, this.VirtualRecordNameArhiva, this.VirtualRecID);
      arhivaData._arAction  = _arAction;
      arhivaData._arTS      = DateTime./*Now*/MinValue; // "arTS   = CURRENT_TIMESTAMP " vodi brigu o ovome 
      arhivaData._arUID     = ZXC.vvDB_User;

      return arhivaData;
   }

   public VvDataRecord CreateArhivedDataRecord(XSqlConnection conn, string _arAction)
   {
      if(this.IsPolyDocument)
      {
         VvPolyDocumRecord arhivedPolyDocumRecord = (VvPolyDocumRecord)this.Clone();

         arhivedPolyDocumRecord.CreateAndInitArhivaStruct(conn, _arAction);

         arhivedPolyDocumRecord.TakeTransesFrom ((VvPolyDocumRecord)this);
         arhivedPolyDocumRecord.TakeTransesFrom2((VvPolyDocumRecord)this);
         arhivedPolyDocumRecord.TakeTransesFrom3((VvPolyDocumRecord)this);

         if(this.IsExtendable)
         {
            ((IVvExtendableDataRecord)arhivedPolyDocumRecord).TakeExtenderDataFrom(this.VirtualExtenderRecord);
         }

         return arhivedPolyDocumRecord;
      }

      if(this.IsDocument)
      {
         VvDocumentRecord arhivedDocumentRecord = (VvDocumentRecord)this.Clone();

         arhivedDocumentRecord.CreateAndInitArhivaStruct(conn, _arAction);

         arhivedDocumentRecord.TakeTransesFrom((VvDocumentRecord)this);

         if(this.IsExtendable)
         {
            ((IVvExtendableDataRecord)arhivedDocumentRecord).TakeExtenderDataFrom(this.VirtualExtenderRecord);
         }

         return arhivedDocumentRecord;
      }

      VvDataRecord arhivedDataRecord = (VvDataRecord)this.Clone();

      arhivedDataRecord.CreateAndInitArhivaStruct(conn, _arAction);

      if(this.IsExtendable)
      {
         ((IVvExtendableDataRecord)arhivedDataRecord).TakeExtenderDataFrom(this.VirtualExtenderRecord);
      }

      return arhivedDataRecord;

   }

   public static string ExtractNormalTableNameFromArhivaTableName(string arhivaTableName)
   {
      if(arhivaTableName.EndsWith(VvDataRecord.ArhRecNameExstension))
      {
         // old, bugovito!___________________
         //int extensionLen = VvDataRecord.ArhRecNameExstension.Length;
         //char[] charsToTrim = new char[extensionLen];
         //for(int i = 0; i < extensionLen; ++i)
         //   charsToTrim[i] = VvDataRecord.ArhRecNameExstension[i];

         //return arhivaTableName.TrimEnd(charsToTrim);
         // old, bugovito!___________________

         int extensionLen = VvDataRecord.ArhRecNameExstension.Length;

         return arhivaTableName.Substring(0, arhivaTableName.Length - extensionLen);
      }
      else
      {
         return arhivaTableName;
      }
   }

   public static string AddArhRecNameExstensionToNormalTableName(string normalTableName)
   {
      return normalTableName + VvDataRecord.ArhRecNameExstension;
   }

   #endregion Methodz

   #endregion All About ARHIVA

   #region All About SKY

   public virtual SkyRule SkyRule
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false) return null;

         return ZXC.CURR_SkyRules.SingleOrDefault(sr => sr.Record == this.VirtualRecordName);
      }
   }

   public ZXC.SkyOperation SkyOperation
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false || this.SkyRule == null) return ZXC.SkyOperation.NONE;

         return this.SkyRule.TheSkyOperation; // PAZI!: NEMOJ koristiti property 'SkyRule.SkyOperation' nego 'SkyRule.TheSkyOperation'. SkyRule.SkyOperation poziva samog sebe 
      }
   }

   public bool SkyRuleDeniesWrite_ThisIsReceiveOnly
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false || this.SkyRule == null) return false;

         // NotaBene: this.SkyRule is virtual 
         return this.SkyRule.TheSkyOperation == ZXC.SkyOperation.RECEIVE;
      }
   }

   public ZXC.LanSrvKind SkyBirthLocationKind
   {
      get
      {
         //if(this.VirtualLanSrvID == ZXC.vvDB_ServerID_SkyCloud) return ZXC.LanSrvKind.SKY ; // TEORETSKI NEMOGUCE!!! 
         //if(this.VirtualLanSrvID == ZXC.vvDB_ServerID_CENTRALA) return ZXC.LanSrvKind.CENT;
         //if(this.VirtualLanSrvID.IsPoslovnicaServerID()       ) return ZXC.LanSrvKind.SHOP;
         //
         //return ZXC.LanSrvKind.NONE;

         return ZXC.GetLanSrvKind(this.VirtualLanSrvID);
      }
   }

   public bool ShouldConsider_SKYlocker // ConcurentWriteChange (istovremeno 'žuto' izazvano sa Dodaj ili Ispravi) 
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false || this.SkyRule == null) return false;

         if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT && this.SkyRule.CentOPS == ZXC.SkyOperation.SendAndReceive) return true;

         if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.SHOP && this.SkyRule.ShopOPS == ZXC.SkyOperation.SendAndReceive)
         {
            if(this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyOTHERskl1 ||
               this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyOTHERskl2 ||
               this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.NONE           )
            {
               return false;
            }

            if(this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyLOCALskl1 ||
               this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyLOCALskl2 ||
               this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.EVERYTHING     )
            {
               return true;
            }
         }

         return false;
      }
   }

   // Ako i ja mogu raditi WRITE operacije na ovakvom recordu koji se zatekao u SKYlockeru 
   // tada mi to treba ZABRANITI, jer ne smijem mijenjati takav podatak sve dok ne dobijem 
   // tj. RECEIVE-am kompletnu najnoviju verziju citave tablice tog recorda. 
   public bool ShouldDeny_RWTorDEL_Operation_ConsideringSKYlocker(VvSQL.VvSKYlockerInfo SKYlockerInfo) // ADD op je dozvoljena, jer se prethodno osigurava unikatnost recorda 
   {
      if(ZXC.IsSkyEnvironment == false || this.SkyRule == null) return false;

      // ako je tt prazan, ovo je kupdob, artikl, ... i cim je nesto u skyLockeru: deny this operation! 
      if(this.SkyRule.DocumTT.IsEmpty()) return true;

      // ako se RECEIVE-a EVERYTHING, a JE u skyLockeru
      if(this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.EVERYTHING) return true;

      // ako sam ja CENTRALA, tada je moj RECEIVE kind implicitno RECEIVE-a EVERYTHING 
      if(ZXC.ThisLanServerKind == ZXC.LanSrvKind.CENT) return true;

      if(this is Faktur)
      {
         // Ako je u lockeru, a i ja ga mogu mijenjati... ODBIJ! 
         if(this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyLOCALskl1 && ((this as Faktur).Skl1kind == ZXC.SkySklKind.ShopMPSK || 
                                                                             (this as Faktur).Skl1kind == ZXC.SkySklKind.ShopVPSK )) return true;

         if(this.SkyRule.ShopRCVkind == ZXC.SkyReceiveKind.OnlyLOCALskl2 && ((this as Faktur).Skl2kind == ZXC.SkySklKind.ShopMPSK ||
                                                                             (this as Faktur).Skl2kind == ZXC.SkySklKind.ShopVPSK)) return true;
      }

      return false;
   }

   #endregion All About SKY

   public static bool GetDifferentPropertiz(List<ZXC.VvUtilDataPackage> differentPropertizList, VvDataRecord olderVvDataRecord, VvDataRecord newerVvDataRecord)
   {
      Type thisType  = olderVvDataRecord.GetType();
      Type otherType = newerVvDataRecord.GetType();

      bool isTrans = olderVvDataRecord is VvTransRecord;
      bool diffOccuredForThisTrans = false;

      object thisValue, otherValue;

    //List<ZXC.VvUtilDataPackage> differentPropertizList = new List<ZXC.VvUtilDataPackage>();

      #region Trans additions

      if(olderVvDataRecord is VvDocumentRecord)
      {
         VvDocumentRecord olderVvDocumentRecord = olderVvDataRecord as VvDocumentRecord;
         VvDocumentRecord newerVvDocumentRecord = newerVvDataRecord as VvDocumentRecord;
         VvTransRecord    newerTrans_rec;

         foreach(VvTransRecord olderTrans_rec in olderVvDocumentRecord.VirtualTranses)
         {
            newerTrans_rec = newerVvDocumentRecord.VirtualTranses.SingleOrDefault(othertTrn => othertTrn.VirtualT_Serial == olderTrans_rec.VirtualT_Serial);

            if(newerTrans_rec == null)
            {
               differentPropertizList.Add(new ZXC.VvUtilDataPackage 
               { 
                  TheStr1 = "thisTrnRecID"/*pi.Name*/, 
                  TheStr2 = "Redak " + olderTrans_rec.VirtualT_Serial.ToString() + ".", 
                  TheStr3 = "NULL", /*otherValue.ToString()*/ 
                  TheBool = true,   // isTrans
                  TheInt  = olderTrans_rec.VirtualT_Serial
               });

               continue;
            }
            else
            {
               GetDifferentPropertiz(differentPropertizList, olderTrans_rec, newerTrans_rec);
            }
         }
      } // if(this is VvDocumentRecord) 

      // TODO: PolyDocument trn2 & trn3 additions! 

      #endregion Trans additions

      PropertyInfo[] sourceProperties = thisType.GetProperties();
      foreach(PropertyInfo pi in sourceProperties)
      {
         if(isTrans && diffOccuredForThisTrans) break; // javljati cemo samo PRVU razliku za trans-ove 

         thisValue  = thisType .GetProperty(pi.Name).GetValue(olderVvDataRecord, null);
         otherValue = otherType.GetProperty(pi.Name).GetValue(newerVvDataRecord, null);

         if(thisValue == null && otherValue == null) 
         {
            // do nothin' 
         }
         else if((thisValue == null && otherValue != null) ||
                 (thisValue != null && otherValue == null))
         {
            if(ShouldSkipThsiPI(pi, thisValue)) continue;

            differentPropertizList.Add(new ZXC.VvUtilDataPackage { TheStr1 = pi.Name, TheStr2 = thisValue == null ? "null" : thisValue.ToString(), TheStr3 = otherValue == null ? "null" : otherValue.ToString(), TheBool = isTrans, TheInt = (isTrans ? (olderVvDataRecord as VvTransRecord).VirtualT_Serial : 0) });

            if(isTrans) diffOccuredForThisTrans = true;
         }
         else if(thisValue .ToString() != 
                 otherValue.ToString())
         {
            if(ShouldSkipThsiPI(pi, thisValue)) continue;

            differentPropertizList.Add(new ZXC.VvUtilDataPackage { TheStr1 = pi.Name, TheStr2 = thisValue.ToString(), TheStr3 = otherValue.ToString(), TheBool = isTrans, TheInt = (isTrans ? (olderVvDataRecord as VvTransRecord).VirtualT_Serial : 0) });

            if(isTrans) diffOccuredForThisTrans = true;
         }
      }

      return differentPropertizList.Count.NotZero(); // has differences 
   }

   private static bool ShouldSkipThsiPI(PropertyInfo pi, object olderValue)
   {
      // 09.01.2023: 
      if(olderValue == null) return true;
      if(
         pi.Name.Contains("RecID" ) ||
         pi.Name.Contains("ModTS" ) ||
         pi.Name.Contains("ModUID") ||
         pi.Name.Contains("RecVer") ||
         //pi.Name.Contains("Ar"    ) ||

         pi.Name.Contains("TheLogo") ||
         pi.Name.Contains("CertFile") ||
         pi.Name.Contains("ESgnCertifikat") ||

         pi.Name.ToLower().Contains("astext") ||

       //pi.Name.StartsWith("S_"    ) ||
         pi.Name.StartsWith("R_"    ) ||
         pi.Name.StartsWith("Skn_"  ) ||
         pi.Name.StartsWith("TrnSum") ||
         pi.Name.StartsWith("Suggested") ||
         pi.Name.StartsWith("New_Artik") ||

         pi.Name.StartsWith("S_" ) ||
         pi.Name.Contains  ("_uk") ||

         pi.Name == "T_recID" ||
         pi.Name == "T_parentID" ||
         pi.Name == "HasSenseCheckingForArhivaVersionExistance" ||

         olderValue.ToString().Contains("RecID") 
        )
      return true ;

      return false;
   }

   #region VvXmlDR Serialize / Deserialize

   private VvXmlDRmetaData vvXmlDRmetaData;
   public  VvXmlDRmetaData VvXmlDRmetaData
   {
      get => vvXmlDRmetaData        ;
      set => vvXmlDRmetaData = value;
   }

   public string Suggested_vvXmlDR_fileName_root { get { return "vv" + VirtualRecordName2; } }
   public string Suggested_vvXmlDR_fileName
   {
      get
      {
       //return ZXC.SystemValidFileName(Suggested_vvXmlDR_fileName_root + "_" + VirtualIDstring + "_" + ZXC.CURR_prjkt_rec.Ticker + "_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName));
         return ZXC.SystemValidFileName(Suggested_vvXmlDR_fileName_root + "_" + VirtualIDstring + "_" + ZXC.PUG_ID                + "_" + DateTime.Now.ToString(ZXC.VvTimeStampFormat4FileName));
      }
   }

   public string Suggested_vvXmlDR_fileName_wExt { get { return Suggested_vvXmlDR_fileName + ".xml"; } }

   public static string Auto_vvXmlDR_preffix            { get { return "$" + "_"; } }
   public static string Auto_vvXmlDR_preffixInvalidated { get { return "#" + "_"; } }

   public string Suggested_vvXmlDR_fileName_wExt_andAutoFlagPreffix { get { return Auto_vvXmlDR_preffix + VirtualRecID + "_" + Suggested_vvXmlDR_fileName_wExt; } }

   // SERIALIZE: ##########################################################################################

   private static System.Xml.Serialization.XmlSerializer serializer;
   private static System.Xml.Serialization.XmlSerializer GetSerializer<T>()
   {
      //if((serializer == null)) ... olvejs! jer se inace mjesaju Artikl, Kupdob, ...
      {
         serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
      }

      return serializer;
   }

   public string Serialize<T>(System.Text.Encoding encoding)
   {
      System.IO.StreamReader streamReader = null;
      System.IO.MemoryStream memoryStream = null;
      try
      {
         memoryStream = new System.IO.MemoryStream();
         System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
         xmlWriterSettings.Encoding = encoding;
         System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(memoryStream, xmlWriterSettings);

         GetSerializer<T>().Serialize(xmlWriter, this);

         memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
         streamReader = new System.IO.StreamReader(memoryStream, encoding);
         return streamReader.ReadToEnd();
      }
      finally
      {
         if((streamReader != null)) { streamReader.Dispose(); }
         if((memoryStream != null)) { memoryStream.Dispose(); }
      }
   }

   public string Serialize<T>()
   {
      return Serialize<T>(System.Text.Encoding.UTF8);
   }

   public virtual string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      throw new Exception("SaveSerialized_VvDataRecord_ToXmlFile NOT OVERRIDEN");
   }

   public string SaveSerialized_VvDataRecord_ToXmlFile_AUTOMATICALLY(VvSQL.DB_RW_ActionType dbAction)
   {
      string directoryName = ZXC.VvSerializedDR_DirectoryName;

      string fileName      = this.Suggested_vvXmlDR_fileName_wExt_andAutoFlagPreffix.Replace(                  "_vv", 
                                                                                              "_" + dbAction + "_vv");
      // 22.10.2019:
      if(this is VvDocumentRecord)
      {
         VvDocumentRecord vvDocumentRecord = (VvDocumentRecord)this.CreateNewRecordAndCloneItComplete();

         vvDocumentRecord.DiscardPreviouslyDeletedTranses();
         if(vvDocumentRecord.IsPolyDocument)
         {
            (vvDocumentRecord as VvPolyDocumRecord).DiscardPreviouslyDeletedTranses2();
            (vvDocumentRecord as VvPolyDocumRecord).DiscardPreviouslyDeletedTranses3();

         } // if(deserialized_vvDataRecord.IsPolyDocument) 

         return vvDocumentRecord.SaveSerialized_VvDataRecord_ToXmlFile(Path.Combine(directoryName, fileName), true);
      }

      return this.SaveSerialized_VvDataRecord_ToXmlFile(Path.Combine(directoryName, fileName), true);
   }

   /*public*/protected string SaveSerialized_VvDataRecord_ToXmlFile_JOB<T>(string fileName/*, System.Text.Encoding encoding*/, bool _isAutoCreat)
   {
      string xmlString = "";

      System.Text.Encoding encoding = ZXC.VvUTF8Encoding_noBOM;

      //System.IO.StreamWriter streamWriter = null;

      this.vvXmlDRmetaData = new VvXmlDRmetaData(this, _isAutoCreat);

      try
      {
         xmlString = Serialize<T>(encoding);
         using(StreamWriter streamWriter = new StreamWriter(fileName, false, encoding))
         {
          //streamWriter.WriteLine(                     xmlString          );
            streamWriter.WriteLine(ZXC.RemoveEmptyNodes(xmlString).OuterXml);
            streamWriter.Flush();
            streamWriter.Close();
         }
      }
      //finally
      //{
      //   if((streamWriter != null))
      //   {
      //      streamWriter.Dispose();
      //   }
      //}
      catch (Exception ex)
      {
         ZXC.aim_log("SaveSerialized_VvDataRecord_ToXmlFile_JOB EXCEPTION:\n\n{0}\n\nInner:\n\n{1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "no inner ex");
      }

      //if(true) ZXC.Vv_GZip_ThisFile(fileName);

      return xmlString;
   }

   // DESERIALIZE: ##########################################################################################

   public bool IsJustDeserializedFromXML { get; set; }

   public virtual VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      throw new Exception("Deserialize_VvDataRecord_FromXmlFile NOT OVERRIDEN");
   }

   /*public*/ protected static T DeserializeFromXmlFile<T>(string fileName) //where T : VvDataRecord
   {
      T vvdataRecord_OR_fakturType = default(T);

      XmlSerializer ser = new XmlSerializer(typeof(T));
      FileStream    fs  = new FileStream(fileName, FileMode.Open);
      try
      {
         vvdataRecord_OR_fakturType = (T) ser.Deserialize(fs);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Neuspjeh pri DESERIALIZIRANJU\n\r{0}\n\nUKLONITI ili ZAMIJENITI xml datoteku!!!", fileName);
         ZXC.aim_emsg_VvException(ex);
      }
      fs.Close();

      // jer moze biti i 'FakturType' koji nije VvDataRecord 
      if(vvdataRecord_OR_fakturType is VvDataRecord) (vvdataRecord_OR_fakturType as VvDataRecord).IsJustDeserializedFromXML = true;

      return vvdataRecord_OR_fakturType;
   }

   public void TurnNullValuesToEmptyString()
   {
      var stringProperties = this.GetType().GetProperties().Where(x => x.PropertyType == typeof(string));

      foreach(var property in stringProperties)
      {
         if(property.GetValue(this, null) == null && property.GetSetMethod() != null)
         {
            property.SetValue(this, string.Empty, null);
         }
      }

      if(this.IsDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvDocumentRecord)this).VirtualTranses)
         {
            trans_rec.TurnNullValuesToEmptyString();
         }
      }

      if(this.IsPolyDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)this).VirtualTranses2)
         {
            trans_rec.TurnNullValuesToEmptyString();
         }
         foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)this).VirtualTranses3)
         {
            trans_rec.TurnNullValuesToEmptyString();
         }
      }

   }

   #endregion Serialize / Deserialize

   #region Convert All Money Propertiez of VvDataRecord

   public virtual bool Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(XSqlConnection conn/*, T vvDataRecord*/) //where T : VvDataRecord
   {
    //throw new Exception("Virtual Convert_Kuna_To_Euro_ForAllMoneyPropertiez NOT OVERRIDEN!\n\r\n\r For Type " + typeof(T).ToString());
      throw new Exception("Virtual Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB NOT OVERRIDEN!");
    //return false; // EditedHasChanges 
   }
   internal static bool Convert_Kuna_To_Euro_ForAllMoneyPropertiez/*<T>*/(XSqlConnection conn, /*T*/VvDataRecord vvDataRecord) //where T : VvDataRecord//, new()
   {
      return vvDataRecord.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB/*<T>*/(conn/*, vvDataRecord*/);
   }

   public virtual bool Convert_Euro_To_Kuna_ForAllMoneyPropertiez_JOB(XSqlConnection conn/*, T vvDataRecord*/) //where T : VvDataRecord
   {
    //throw new Exception("Virtual Convert_Kuna_To_Euro_ForAllMoneyPropertiez NOT OVERRIDEN!\n\r\n\r For Type " + typeof(T).ToString());
      throw new Exception("Virtual Convert_Euro_To_Kuna_ForAllMoneyPropertiez_JOB NOT OVERRIDEN!");
    //return false; // EditedHasChanges 
   }
   internal static bool Convert_Euro_To_Kuna_ForAllMoneyPropertiez/*<T>*/(XSqlConnection conn, /*T*/VvDataRecord vvDataRecord) //where T : VvDataRecord//, new()
   {
      return vvDataRecord.Convert_Euro_To_Kuna_ForAllMoneyPropertiez_JOB/*<T>*/(conn/*, vvDataRecord*/);
   }

   public virtual bool Convert_HRDkontra_ForAllMoneyPropertiez(XSqlConnection conn)
   {
      bool isHrkEra  = ZXC.projectYearAsInt <= 2022;
      bool isEuroEra = ZXC.projectYearAsInt >= 2023;

      if(isHrkEra) return Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB(conn);
      else         return Convert_Euro_To_Kuna_ForAllMoneyPropertiez_JOB(conn);

   }

   #endregion Convert All Money Propertiez of VvDataRecord

}

public abstract class VvSifrarRecord     : VvDataRecord
{
   public abstract void InvokeTransClear();

   public abstract string SifraColName
   {
      get;
   }

   public abstract string SifraColValue
   {
      get;
   }

   public abstract System.Data.DataRow SifraColDrSchema
   {
      get;
   }
   public override bool IsSifrar
   {
      get { return true; }
   }

   public override bool IsAutoSifra
   {
      get { return true; }
   }

   /// <summary>
   /// Za npr Sklad i Osred ovo je true, a za Person je false (overrajdano u Person.cs - u)
   /// </summary>
   public override bool IsStringAutoSifra
   {
      get { return true; }
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

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string VirtualIDstring
   {
      get { return this.SifraColValue; }
   }

}

public abstract class VvDocumLikeRecord  : VvDataRecord
{
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualDokNum
   {
      get;
      /* 02.10.2012: */ set;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract DateTime VirtualDokDate
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract string VirtualTT
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualTTnum
   {
      get;
      /* 02.10.2012: */ set;
   }

   // 11.04.2019: 
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualTTnum_Bkp
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string VirtualRecordName2 { get { return base.VirtualRecordName2 + "_" + VirtualTT; } }

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
      get { return true; }
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

   public override SkyRule SkyRule
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false) return null;

         SkyRule skyRule = ZXC.CURR_SkyRules
            .SingleOrDefault
            (sr =>
               sr.Record == this.VirtualRecordName
               &&
               (sr.DocumTT.NotEmpty() ? sr.DocumTT == this.VirtualTT : true)
               &&
               (sr.BirthLoc.NotEmpty() && this is Faktur ? sr.BirthLoc == this.SkyBirthLocationKind : true)
               &&
               (sr.Skl1kind.NotEmpty() && this is Faktur ? sr.Skl1kind == (this as Faktur).Skl1kind : true)
               &&
               (sr.Skl2kind.NotEmpty() && this is Faktur ? sr.Skl2kind == (this as Faktur).Skl2kind : true)
            );

         return skyRule;
      }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string VirtualIDstring
   {
      get { return /*this.VirtualTT + "_" +*/ this.VirtualTTnum.ToString(); }
   }

}

public abstract class VvDocumentRecord   : VvDocumLikeRecord
{
   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public abstract List<VvTransRecord> VirtualTranses
   {
      get;
   }

   public abstract string TransRecordName
   {
      get;
   }

   public abstract string TransRecordNameArhiva
   {
      get;
   }

   public abstract void InvokeTransClear();
   public abstract void InvokeTransRemove(VvTransRecord trans_rec);

   public abstract List<VvTransRecord> CloneTranses();

   public abstract void TakeTransesFrom(VvDocumentRecord vvDocumentRecord);
   public /*abstract*/ void TakeTranses_Backup_DataFrom(VvDocumentRecord vvDocumentRecord)
   {
      for(int i = 0; i < vvDocumentRecord.VirtualTranses.Count; ++i)
      {
         this.VirtualTranses[i].TakeInBackupData_CurrentDataFrom(vvDocumentRecord.VirtualTranses[i]);
      }
   }

   // 09.04.2019: 
   public abstract void TakeTheseTranses(List<VvTransRecord> transList);

   public bool EditedTransesHaveChanges()
   {
      foreach(VvTransRecord trans_rec in VirtualTranses)
      {
         if(trans_rec.SaveTransesWriteMode != ZXC.WriteMode.None) return true;
      }

      return false;
   }

   /// <summary>
   /// Zgodno, ha? Tricky ti je removati iteme iz neke liste u jednom loopingu. Moras 'vako!
   /// </summary>
   /// <param name="_vvPolyDocumRecord"></param>
   public void DiscardPreviouslyAddedTranses()
   {
      List<VvTransRecord> transesToRemoveList = new List<VvTransRecord>(this.VirtualTranses.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses)
      {
         if(trans_rec.VirtualRecID == 0) transesToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transesToRemoveList)
      {
         this.InvokeTransRemove(trans_rec);
      }
   }

   // 22.10.2019: 
   public void DiscardPreviouslyDeletedTranses()
   {
      List<VvTransRecord> transesToRemoveList = new List<VvTransRecord>(this.VirtualTranses.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses)
      {
       //if(trans_rec.VirtualRecID == 0) transesToRemoveList.Add(trans_rec);
         if(trans_rec.SaveTransesWriteMode == ZXC.WriteMode.Delete) transesToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transesToRemoveList)
      {
         this.InvokeTransRemove(trans_rec);
      }
   }

   public override bool IsDocument
   {
      get { return true; }
   }

   //public override bool IsDocumentLike
   //{
   //   get { return true; }
   //}

   public abstract VvTransRecord VvTransRecordFactory();

   public virtual bool IsOneTransChangeShouldRecalcOtherAllTranses { get { return false; } }

   public /*bool*/ int TransDuplicatesCount()
   {
      var serialGR = VirtualTranses.GroupBy(trn => trn.VirtualT_Serial);

      return serialGR.Count(serGR => serGR.Count() > 1);
   }

}

public abstract class VvPolyDocumRecord  : VvDocumentRecord
{
   public override bool IsPolyDocument
   {
      get { return true; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract List<VvTransRecord> VirtualTranses2
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract List<VvTransRecord> VirtualTranses3
   {
      get;
   }

   public abstract string TransRecordName2
   {
      get;
   }

   public abstract string TransRecordNameArhiva2
   {
      get;
   }

   public abstract string TransRecordName3
   {
      get;
   }

   public abstract string TransRecordNameArhiva3
   {
      get;
   }

   public abstract void InvokeTransClear2();
   public abstract void InvokeTransClear3();

   public abstract void InvokeTransRemove2(VvTransRecord trans_rec);
   public abstract void InvokeTransRemove3(VvTransRecord trans_rec);

   public abstract List<VvTransRecord> CloneTranses2();
   public abstract List<VvTransRecord> CloneTranses3();

   public abstract void TakeTransesFrom2(VvPolyDocumRecord vvDocumentRecord);
   public abstract void TakeTransesFrom3(VvPolyDocumRecord vvDocumentRecord);

   public abstract void TakeTheseTranses2(List<VvTransRecord> transList);
   public abstract void TakeTheseTranses3(List<VvTransRecord> transList);

   public /*abstract*/ void TakeTranses2_Backup_DataFrom(VvPolyDocumRecord vvPolyDocumentRecord)
   {
      for(int i = 0; i < vvPolyDocumentRecord.VirtualTranses2.Count; ++i)
      {
         this.VirtualTranses2[i].TakeInBackupData_CurrentDataFrom(vvPolyDocumentRecord.VirtualTranses2[i]);
      }
   }

   public /*abstract*/ void TakeTranses3_Backup_DataFrom(VvPolyDocumRecord vvPolyDocumentRecord)
   {
      for(int i = 0; i < vvPolyDocumentRecord.VirtualTranses3.Count; ++i)
      {
         this.VirtualTranses3[i].TakeInBackupData_CurrentDataFrom(vvPolyDocumentRecord.VirtualTranses3[i]);
      }
   }

   public bool EditedTransesHaveChanges2()
   {
      foreach(VvTransRecord trans_rec in VirtualTranses2)
      {
         if(trans_rec.SaveTransesWriteMode != ZXC.WriteMode.None) return true;
      }

      return false;
   }

   public bool EditedTransesHaveChanges3()
   {
      foreach(VvTransRecord trans_rec in VirtualTranses3)
      {
         if(trans_rec.SaveTransesWriteMode != ZXC.WriteMode.None) return true;
      }

      return false;
   }

   /// <summary>
   /// Zgodno, ha? Tricky ti je removati iteme iz neke liste u jednom loopingu. Moras 'vako!
   /// </summary>
   /// <param name="_vvPolyDocumRecord"></param>
   public void DiscardPreviouslyAddedTranses2()
   {
      List<VvTransRecord> transes2ToRemoveList = new List<VvTransRecord>(this.VirtualTranses2.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses2)
      {
         if(trans_rec.VirtualRecID == 0) transes2ToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transes2ToRemoveList)
      {
         this.InvokeTransRemove2(trans_rec);
      }
   }

   public void DiscardPreviouslyDeletedTranses2()
   {
      List<VvTransRecord> transes2ToRemoveList = new List<VvTransRecord>(this.VirtualTranses2.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses2)
      {
         //if(trans_rec.VirtualRecID == 0) transesToRemoveList.Add(trans_rec);
         if(trans_rec.SaveTransesWriteMode == ZXC.WriteMode.Delete) transes2ToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transes2ToRemoveList)
      {
         this.InvokeTransRemove2(trans_rec);
      }
   }

   public void DiscardPreviouslyAddedTranses3()
   {
      List<VvTransRecord> transes3ToRemoveList = new List<VvTransRecord>(this.VirtualTranses3.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses3)
      {
         if(trans_rec.VirtualRecID == 0) transes3ToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transes3ToRemoveList)
      {
         this.InvokeTransRemove3(trans_rec);
      }
   }

   public void DiscardPreviouslyDeletedTranses3()
   {
      List<VvTransRecord> transes3ToRemoveList = new List<VvTransRecord>(this.VirtualTranses3.Count);

      foreach(VvTransRecord trans_rec in this.VirtualTranses3)
      {
         //if(trans_rec.VirtualRecID == 0) transesToRemoveList.Add(trans_rec);
         if(trans_rec.SaveTransesWriteMode == ZXC.WriteMode.Delete) transes3ToRemoveList.Add(trans_rec);
      }

      foreach(VvTransRecord trans_rec in transes3ToRemoveList)
      {
         this.InvokeTransRemove3(trans_rec);
      }
   }

}

public abstract class VvTransRecord      : VvDataRecord
{
   public virtual void CalcTransResults(VvDocumentRecord vvDocumentRecord)
   {
   }

   public abstract string DocumentRecordName
   {
      get;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualParentRecID
   {
      get;
      set;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract ushort VirtualT_Serial
   {
      get;
      set;
   }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualT_dokNum
   {
      get;
      set;
   }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public abstract uint VirtualT_ttNum
   {
      get;
      set;
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override DateTime VirtualAddTS { get { return DateTime.MinValue; } }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override DateTime VirtualModTS { get { return DateTime.MinValue;  } }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string   VirtualAddUID{ get { return null; } }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string   VirtualModUID{ get { return null; } }

   // SkyNews 
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override uint     VirtualLanSrvID{ get { return 0; } set {;} }
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override uint     VirtualLanRecID{ get { return 0; } set {;} }

   // NOTA BENE !!! ovo ti se ne Clone-ira samo od sebe! 
   private ZXC.WriteMode saveTransesWriteMode = ZXC.WriteMode.None;
   public  ZXC.WriteMode SaveTransesWriteMode
   {
      get { return saveTransesWriteMode; }
      set {        saveTransesWriteMode = value; }
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
      get { return true; }
   }

   public override bool IsArhivable
   {
      get { return false; }
   }

   public virtual bool IsGlassOnIRM { get { return false; } }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override string VirtualIDstring
   {
      get { return ""; }
   }

   public uint RecID_InXML { get; set; }

}

public struct VvArhivaStruct
{
   public uint     _origRecID;
   public uint     _recVer;
   public string   _arAction;
   public DateTime _arTS;
   public string   _arUID;
}

public struct VvXmlDRmetaData
{
   public bool     isAutoCreat;
   public string   vvdbUser   ;
   public DateTime xmlTS      ;
   public string   projektYear;
   public uint     projektCD  ;
   public string   projektTK  ;
   public string   projektName;
   public string   vvDomena   ;
   public string   server     ;
   public uint     serverID   ;
   public uint     recID      ;
   public uint     lanSrvID   ;
   public uint     lanRecID   ;
   public string   addUID     ;
   public string   modUID     ;
   public DateTime addTS      ;
   public DateTime modTS      ;
   public string   programInfo;
   public string   recordName ;
   public string   recordName2;
   public string   IDstring   ;

   public VvXmlDRmetaData(VvDataRecord vvDataRecord, bool _isAutoCreat)
   {
      this.isAutoCreat = _isAutoCreat                       ;
      this.vvdbUser    = ZXC.vvDB_User                      ;
      this.xmlTS       = DateTime.Now                       ;
      this.projektYear = ZXC.projectYear                    ;
      this.projektCD   = ZXC.CURR_prjkt_rec.KupdobCD        ;
      this.projektTK   = ZXC.CURR_prjkt_rec.Ticker          ;
      this.projektName = ZXC.CURR_prjkt_rec.Naziv           ;
      this.vvDomena    = ZXC.vvDB_VvDomena                  ;
      this.server      = ZXC.vvDB_Server                    ;
      this.serverID    = ZXC.vvDB_ServerID                  ;
      this.recID       = vvDataRecord.VirtualRecID          ;
      this.lanSrvID    = vvDataRecord.VirtualLanSrvID       ;
      this.lanRecID    = vvDataRecord.VirtualLanRecID       ;
      this.addUID      = vvDataRecord.VirtualAddUID         ;
      this.modUID      = vvDataRecord.VirtualModUID         ;
      this.addTS       = vvDataRecord.VirtualAddTS          ;
      this.modTS       = vvDataRecord.VirtualModTS          ;
      this.programInfo = ZXC.TheVvForm.Text                 ;
      this.recordName  = vvDataRecord.VirtualRecordName     ;
      this.recordName2 = vvDataRecord.VirtualRecordName2    ;
      this.IDstring    = vvDataRecord.VirtualIDstring       ;

      // 21.10.2019: 
      if(vvDataRecord.IsDocument)
      {
         foreach(VvTransRecord trans_rec in ((VvDocumentRecord)vvDataRecord).VirtualTranses)
         {
            trans_rec.RecID_InXML = trans_rec.VirtualRecID;
         }

         if(vvDataRecord.IsPolyDocument)
         {
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses2)
            {
               trans_rec.RecID_InXML = trans_rec.VirtualRecID;
            }
            foreach(VvTransRecord trans_rec in ((VvPolyDocumRecord)vvDataRecord).VirtualTranses3)
            {
               trans_rec.RecID_InXML = trans_rec.VirtualRecID;
            }
         } // if(vvDataRecord.IsPolyDocument) 

      } // if(vvDataRecord.IsDocument) 

   }
}

#region OBSOLETE / Sepsolete: public class VvList<T> : List<T> where T : VvDataRecord

   //public class VvList<T> : List<T> where T : VvDataRecord
   //{
   //   public VvList() : base()
   //   {
   //   }

   //   public VvList(int initialCapacity) : base(initialCapacity)
   //   {
   //   }

   //   public T FindByRecID(uint prjktKupdobCD)
   //   {
   //      //return this.Where(list => list.VirtualRecID == prjktKupdobCD)/*.Select(list => list)*/.SingleOrDefault();
   //      return this.SingleOrDefault(vvDR => vvDR.VirtualRecID == prjktKupdobCD);
   //   }

   //   public T FindByRecID_OLD(uint prjktKupdobCD)
   //   {
   //      return Find(
   //         delegate(T arhivedDataRecord) // ovo je kak ti anonimous method 
   //         {
   //            return arhivedDataRecord.VirtualRecID == prjktKupdobCD;
   //         }
   //      );
   //   }

   //   //public bool RecIDExists(uint prjktKupdobCD)
   //   //{
   //   //   return FindByRecID(prjktKupdobCD) != null;
   //   //}

   //   public VvList<TOutput> Convert_ListToVvList<TOutput>(Converter<T, TOutput> converter, VvList<TOutput> outputList) where TOutput : VvTransRecord
   //   {
   //      List<TOutput> inputList = ConvertAll<TOutput>(converter);

   //      if(outputList == null) outputList = new VvList<TOutput>();
   //      else                   outputList.Clear();

   //      outputList.AddRange(inputList);

   //      return outputList;
   //   }
   //}

   #endregion

public interface IVvExtendableDataRecord
{
   string ExtenderTableName         { get; }
   string ExtenderTableNameArhiva   { get; }

   void TakeExtenderDataFrom(VvDataRecord vvExtenderDataRecord);

   void TakeExtender_Backup_DataFrom(VvDataRecord vvExtenderDataRecord);
}

public interface IVvExtenderDataRecord
{
   string ParentTableName { get;      }
   uint   ParentRecID     { get; set; }
   string JoinedColName   { get; }
}

[Serializable]
public abstract class VvDataRecordType
{
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public VvDataRecord TheVvDataRecord;

   private VvXmlDRmetaData vvXmlDRmetaData;
   public  VvXmlDRmetaData VvXmlDRmetaData
   {
      get => vvXmlDRmetaData        ;
      set => vvXmlDRmetaData = value;
   }

   #region Serialize / Deserialize

   // SERIALIZE: ##########################################################################################

   private static System.Xml.Serialization.XmlSerializer serializer;
   private static System.Xml.Serialization.XmlSerializer GetSerializer<T>()
   {
      //if((serializer == null)) ... olvejs! jer se inace mjesaju Artikl, Kupdob, ...
      {
         serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
      }

      return serializer;
   }

   public string Serialize<T>(System.Text.Encoding encoding)
   {
      System.IO.StreamReader streamReader = null;
      System.IO.MemoryStream memoryStream = null;
      try
      {
         memoryStream = new System.IO.MemoryStream();
         System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
         xmlWriterSettings.Encoding = encoding;
         System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(memoryStream, xmlWriterSettings);

         GetSerializer<T>().Serialize(xmlWriter, this);

         memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
         streamReader = new System.IO.StreamReader(memoryStream, encoding);
         return streamReader.ReadToEnd();
      }
      finally
      {
         if((streamReader != null)) { streamReader.Dispose(); }
         if((memoryStream != null)) { memoryStream.Dispose(); }
      }
   }

   public string Serialize<T>()
   {
      return Serialize<T>(System.Text.Encoding.UTF8);
   }

   public virtual string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      throw new Exception("SaveSerialized_VvDataRecord_ToXmlFile NOT OVERRIDEN");
   }

   /*public*/protected string SaveSerialized_VvDataRecord_ToXmlFile_JOB<T>(string fileName/*, System.Text.Encoding encoding*/, bool _isAutoCreat, VvDataRecord _theVvDataRecord)
   {
      string xmlString = "";

      System.Text.Encoding encoding = ZXC.VvUTF8Encoding_noBOM;

      System.IO.StreamWriter streamWriter = null;

      this.vvXmlDRmetaData = new VvXmlDRmetaData(_theVvDataRecord, _isAutoCreat);

      try
      {
         xmlString = Serialize<T>(encoding);
         streamWriter = new System.IO.StreamWriter(fileName, false, encoding);
       //streamWriter.WriteLine(                     xmlString          );
         streamWriter.WriteLine(ZXC.RemoveEmptyNodes(xmlString).OuterXml);
         streamWriter.Close();
      }
      finally
      {
         if((streamWriter != null))
         {
            streamWriter.Dispose();
         }
      }

      return xmlString;
   }

   // DESERIALIZE: ##########################################################################################

   public virtual VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      throw new Exception("Deserialize_VvDataRecord_FromXmlFile NOT OVERRIDEN");
   }

   /*public*/ protected static T DeserializeFromXmlFile<T>(string fileName)
   {
      T vvdataRecord = default(T);

      XmlSerializer ser = new XmlSerializer(typeof(T));
      FileStream fs = new FileStream(fileName, FileMode.Open);
      try
      {
         vvdataRecord = (T) ser.Deserialize(fs);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg_VvException(ex);
      }
      fs.Close();

      return vvdataRecord;
   }

   #endregion Serialize / Deserialize

}