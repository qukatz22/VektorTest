using System;
using System.Collections.Generic;

#region struct UserStruct

public struct UserStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   internal string   _userName;
   internal string   _password;
   internal string   _ime;
   internal string   _prezime;
   internal string   _email;
   internal string   _opis;
   internal bool     _isSuper;
   internal string   _oib;
}

#endregion struct UserStruct

public class User : VvSifrarRecord
{

   #region Fildz

   public const string recordName = "user";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private UserStruct currentData;
   private UserStruct backupData;

   //protected static System.Data.DataTable TheSchemaTable = ZXC.UserDao.TheSchemaTable;
   protected static System.Data.DataTable TheSchemaTable /*= ZXC.GetUserDaoTheSchemaTable()*/;
   protected static UserDao.UserCI        CI             /*= ZXC.UserDao.CI*/;

   #endregion Fildz

   #region Constructors

   // static constructor!!! vidi effective c# knjigu, strana 84 
   // Dok nije bilo ovoga, nego si static 'TheSchemaTable' inicijalizirao gore, dizao bi ti se Exception kod promijeni password 
   // na LoginForm-i i to: SAMO!!! kod 'Release - start without debugging' varijante!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
   // Bilo Debug u configuration manageru, bilo obicni F5 (start WITH debugging) ne bi dizao Exception. ZAPAMTI OVO!            
   static User()
   {
      TheSchemaTable = ZXC.UserDao.TheSchemaTable;
      CI             = ZXC.UserDao.CI;

      sorterUserName = new VvSQL.RecordSorter(recordName, recordNameArhiva, new VvSQL.IndexSegment[]
         {
            new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
            new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
         }, "UserName", VvSQL.SorterType.Code, false);

      sorterPrezime = new VvSQL.RecordSorter(User.recordName, User.recordNameArhiva, new VvSQL.IndexSegment[]
         {
            new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prezime]),
            new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
            new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
         }, "Prezime", VvSQL.SorterType.Person, false);
   }

   public User() : this(0)
   {
   }

   public User(uint ID) : base()
   {
      this.currentData = new UserStruct();
      
      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;

      this.currentData._addTS  = DateTime.MinValue;
      this.currentData._modTS  = DateTime.MinValue;
      this.currentData._addUID = "";
      this.currentData._modUID = "";
      this.currentData._lanSrvID = 0;
      this.currentData._lanRecID = 0;

      this.currentData._userName = "";
      this.currentData._password = "";
      this.currentData._ime      = "";
      this.currentData._prezime  = "";
      this.currentData._email    = "";
      this.currentData._opis     = "";
      this.currentData._isSuper  = false;
      this.currentData._oib      = "";
   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return UserName + " (" + Ime + " " + Prezime + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      User user_rec = (User)vvDataRecord;

      switch(sifrarType)
      {
         case VvSQL.SorterType.Code:   return user_rec.UserName;
         case VvSQL.SorterType.Person: return user_rec.Prezime;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in User.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   #region propertiz

   internal UserStruct CurrentData // cijela UserStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   internal UserStruct BackupData // zasada samo za ovaj User record za potrebe RENAME USER-a
   {
      get { return this.backupData; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.UserDao; }
   }

   public override string VirtualRecordName
   {
      get { return User.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return User.recordNameArhiva; }
   }

   public override bool IsAutoSifra
   {
      get { return false; }
   }

   // Dummy for User 
   public override string SifraColName
   {
      get { return ""; }
   }

   public override string SifraColValue
   {
      get { return this.UserName.ToString(); }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set { this.RecID = value; }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return User.sorterUserName; }
   }

   /// <summary>
   /// A je podatak 'userName' kao link (foreign key) za druge tablice,
   /// izmijenjen u operaciji 'Edit'? 
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         return this.currentData._userName != this.backupData._userName;
      }
   }

   private List<Prvlg> _privileges;
   /// <summary>
   /// Gets or sets a list of privileges (line items) for this user.
   /// </summary>
   public List<Prvlg> Privileges
   {
      get { return _privileges; }
      set {        _privileges = value; }
   }

   public override void InvokeTransClear()
   {
      if(this.Privileges != null) this.Privileges.Clear();
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

   public string UserName
   {
      get { return this.currentData._userName; }
      set { this.currentData._userName = value; }
   }

   public string UserName4Sql
   {
      get { return ZXC.GetUserNameWithWwwPreffix(this.currentData._userName); }
   }

   public string BackupedUserName
   {
      get { return this.backupData._userName; }
   }

   public string BackupedUserName4Sql
   {
      get { return ZXC.GetUserNameWithWwwPreffix(this.backupData._userName); }
   }

   /// <summary>
   /// Cryptiran, encoded, sifrirani password
   /// </summary>
   public string PasswdEncodedAsInFile
   {
      get { return this.currentData._password; }
      set { this.currentData._password = value; }
   }

   /// <summary>
   /// Decryptiran, decoded, desifrirani password
   /// </summary>
   public string PasswdDecrypted
   {
      get 
      {
         return (currentData._password.NotEmpty()) ? VvAES.DecryptData(currentData._password, ZXC.vv_User_AES_key) : ""; 
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }

   public string Ime
   {
      get { return this.currentData._ime; }
      set { this.currentData._ime = value; }
   }

   public string Prezime
   {
      get { return this.currentData._prezime; }
      set { this.currentData._prezime = value; }
   }

   public string Email
   {
      get { return this.currentData._email; }
      set { this.currentData._email = value; }
   }

   public string Opis
   {
      get { return this.currentData._opis; }
      set { this.currentData._opis = value; }
   }

   public bool IsSuper
   {
      get { return this.currentData._isSuper; }
      set { this.currentData._isSuper = value; }
   }

   public string Oib
   {
      get { return this.currentData._oib; }
      set { this.currentData._oib = value; }
   }

   public string PrezimeIme
   {
      get { return Person.GetPrezimeIme(this.currentData._prezime, this.currentData._ime); }
   }

   public string ImePrezime
   {
      get { return (this.currentData._ime + " " + this.currentData._prezime); }

   }

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<UserStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<UserStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<UserStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<UserStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<UserStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      User newObject = new User();

      Generic_CloneData<UserStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public User MakeDeepCopy()
   {
      return (User)Clone();
   }

   #endregion

   #region SorterCurrVal

   // TODO: a zakaj je ovo u komentaru? 19.08.2009 

   public static VvSQL.RecordSorter sorterUserName /*= new VvSQL.RecordSorter(recordName, recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "UserName", VvSQL.SorterType.Code, false)*/;

   public static VvSQL.RecordSorter sorterPrezime /*= new VvSQL.RecordSorter(User.recordName, User.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prezime]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.userName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Prezime", VvSQL.SorterType.Person, false)*/;

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      { 
         sorterUserName, 
         sorterPrezime
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Code:   return new object[] { this.UserName,                RecVer };
         case VvSQL.SorterType.Person: return new object[] { this.Prezime,  this.UserName, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion SorterCurrVal

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new User();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((User)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((User)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<User>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<User>(fileName);
   }


   #endregion VvDataRecordFactory

}
