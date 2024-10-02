using System;
using System.Collections.Generic;

#region struct OsredStruct

public struct OsredStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   internal string   _naziv;
   internal string   _osredCD;
   internal string   _konto;
   internal string   _konto_iv;
   internal string   _grupa;
   internal string   _strana_k;
   internal string   _ser_br;
   internal uint  	_mtros_cd;
   internal string  	_mtros_tk;
   internal uint  	_kupdob_cd;
   internal string  	_kupdob_tk;
   internal string   _dokum_cd; 
   internal string  	_invest;
   internal string   _invbr_od;
   internal string   _invbr_do;
   internal string   _koef_am;
   internal decimal  _amort_st;
   internal decimal  _vijek;
   internal bool     _isRashod;
}

#endregion struct OsredStruct

#region struct OsredStatus

public /*struct*/class OsredStatus
{
   internal string OsredCD { get; set; }

   internal DateTime           DateOfStatus { get; set; }
   internal ZXC.AmortRazdoblje AmRazdoblje  { get; set; }

   internal decimal KolSt     { get; set; } // (NB +), (RS -)   
   internal decimal UkNabDug  { get; set; } // SUM(Dug) za 'NB' 
   internal decimal UkNabPot  { get; set; } // SUM(Pot) za 'NB' 
   internal decimal UkRasDug  { get; set; } // SUM(Dug) za 'RS' 
   internal decimal UkRasPot  { get; set; } // SUM(Pot) za 'RS' 
   internal decimal OldAmDug  { get; set; } // SUM(Dug) za 'AM' ... Za stavke datuma <  StartAmRazdobljeDate 
   internal decimal OldAmPot  { get; set; } // SUM(Pot) za 'AM' ... Za stavke datuma <  StartAmRazdobljeDate 
   internal decimal NewAmDug  { get; set; } // SUM(Dug) za 'AM' ... Za stavke datuma >= StartAmRazdobljeDate 
   internal decimal NewAmPot  { get; set; } // SUM(Pot) za 'AM' ... Za stavke datuma >= StartAmRazdobljeDate 

   internal bool    IsRashodovan { get; set; }

   internal decimal InvSt     { get; set; } // (IN (DateOfStatus))   

   internal decimal JustCalculatedNewAmort       { get; set; } // different    then    normalAmort if AmKoef != 1 
   internal decimal JustCalculatedNewNormalAmort { get; set; } // theoretical, same as NewAmort    if AmKoef == 1 

   // Read (get) only: _________________________________________________________________________________________________ 

   internal decimal UkNabDugS { get { return UkNabDug - UkNabPot; } }

   internal decimal OldAmPotS { get { return OldAmPot - OldAmDug; } }
   internal decimal NewAmPotS { get { return NewAmPot - NewAmDug; } }

   internal decimal UkAmDug   { get { return OldAmDug + NewAmDug; } }
   internal decimal UkAmPot   { get { return OldAmPot + NewAmPot; } }

   internal decimal UkAmPotS  { get { return OldAmPotS + NewAmPotS; } }

   internal decimal NewKnjVr  { get { return UkNabDugS - UkAmPotS + UkRasDug - UkRasPot; } }
   internal decimal OldKnjVr  { get { return NewKnjVr  + NewAmPotS; } }

   internal decimal UkDugS    { get { return UkNabDugS + UkRasDug; } }
   internal decimal UkPotS    { get { return UkAmPotS  + UkRasPot; } }

   internal decimal UkSALDO   { get { return UkDugS    - UkPotS; } }
   
   internal DateTime DateAmRazdobljeStart { get { return GetDateAmRazdobljeStart(this.AmRazdoblje, this.DateAmRazdobljeEnd); } }
   internal DateTime DateAmRazdobljeEnd   { get { return DateOfStatus; } }

   internal DateTime DateNabava { get; set; }
   internal DateTime DateRashod { get; set; }

   public static DateTime GetDateAmRazdobljeStart(ZXC.AmortRazdoblje _amRazdoblje, DateTime _dateAmRazdobljeEnd)
   {
      /*if(_amRazdoblje == ZXC.AmortRazdoblje.NOW)
      {
         return _dateAmRazdobljeEnd;
      }
      else*/ if(_amRazdoblje == ZXC.AmortRazdoblje.MJESEC)
      {
         return new DateTime(_dateAmRazdobljeEnd.Year, _dateAmRazdobljeEnd.Month, 01);
      }
      else if(_amRazdoblje == ZXC.AmortRazdoblje.KVARTAL)
      {
         int twoMonthsBefore = _dateAmRazdobljeEnd.Month - 2;

         if(twoMonthsBefore < 1) 
         { 
            twoMonthsBefore = 1; 
            ZXC.aim_emsg("Traziti obracun za kvartal koji zavrsava " + _dateAmRazdobljeEnd.ToShortDateString() + " NEMA SMISLA!"); 
         }

         return new DateTime(_dateAmRazdobljeEnd.Year, twoMonthsBefore, 01);
      }
      else if(_amRazdoblje == ZXC.AmortRazdoblje.POLUGOD)
      {
         int fiveMonthsBefore = _dateAmRazdobljeEnd.Month - 5;

         if(fiveMonthsBefore < 1) 
         { 
            fiveMonthsBefore = 1; 
            ZXC.aim_emsg("Traziti obracun za polugodište koje zavrsava " + _dateAmRazdobljeEnd.ToShortDateString() + " NEMA SMISLA!"); 
         }

         return new DateTime(_dateAmRazdobljeEnd.Year, fiveMonthsBefore, 01);
      }
      else
      {
         return new DateTime(_dateAmRazdobljeEnd.Year, 01, 01);
      }
   }
}

#endregion struct OsredStatus

public class Osred : VvSifrarRecord
{      

   #region Fildz

   public const string recordName = "osred";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private OsredStruct currentData;
   private OsredStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.OsredDao.TheSchemaTable;
   protected static OsredDao.OsredCI CI                  = ZXC.OsredDao.CI;

   #endregion Fildz

   #region Sorters

   public static VvSQL.RecordSorter sorterNaziv = new VvSQL.RecordSorter(Osred.recordName, Osred.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.naziv]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.osredCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Naziv", VvSQL.SorterType.Name, false);

   public static VvSQL.RecordSorter sorterCD = new VvSQL.RecordSorter(Osred.recordName, Osred.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.osredCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Sifra", VvSQL.SorterType.Code, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      { 
         sorterNaziv,
         sorterCD 
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Name: return new object[] { this.Naziv,   this.OsredCD, RecVer };
         case VvSQL.SorterType.Code: return new object[] { this.OsredCD,               RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion Sorters

   #region Constructors

   public Osred() : this(0)
   {
   }

   public Osred(uint ID) : base()
   {
      this.currentData        = new OsredStruct();

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

      this.currentData._naziv       = "";
      this.currentData._osredCD     = "";
      this.currentData._konto       = "";
      this.currentData._konto_iv    = "";
      this.currentData._grupa       = "";
      this.currentData._strana_k    = "";
      this.currentData._ser_br      = "";
      this.currentData._mtros_cd    = 0 ;
      this.currentData._mtros_tk  = "";
      this.currentData._kupdob_cd   = 0 ;
      this.currentData._kupdob_tk = "";
      this.currentData._dokum_cd    = "";
      this.currentData._invest      = "";
      this.currentData._invbr_od    = "";
      this.currentData._invbr_do    = "";
      this.currentData._koef_am     = "";
      this.currentData._amort_st    = 0m;
      this.currentData._vijek       = 0m;
      this.currentData._isRashod    = false ;

   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return Naziv + "(" + OsredCD + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Osred osred_rec = (Osred)vvDataRecord;

      switch(sifrarType)
      {
         case VvSQL.SorterType.Name:   return osred_rec.Naziv;
         case VvSQL.SorterType.Code:   return osred_rec.OsredCD;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Osred.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   // 15.02.2016: 

   #region TheOsEx - 'OsredStatus' property for 'Osred' class

   // 15.02.2016: 
  private OsredStatus theOsEx;
   public OsredStatus TheOsEx
   {
      get 
      {
         if(theOsEx == null) theOsEx = new OsredStatus();
         return              theOsEx; 
      }
      set { theOsEx = value; }
   }

   public DateTime           O_DateOfStatus                 { get { return this.TheOsEx.DateOfStatus                ; } }
   public ZXC.AmortRazdoblje O_AmRazdoblje                  { get { return this.TheOsEx.AmRazdoblje                 ; } }
   public decimal            O_KolSt                        { get { return this.TheOsEx.KolSt                       ; } }
   public decimal            O_UkNabDug                     { get { return this.TheOsEx.UkNabDug                    ; } }
   public decimal            O_UkNabPot                     { get { return this.TheOsEx.UkNabPot                    ; } }
   public decimal            O_UkRasDug                     { get { return this.TheOsEx.UkRasDug                    ; } }
   public decimal            O_UkRasPot                     { get { return this.TheOsEx.UkRasPot                    ; } }
   public decimal            O_OldAmDug                     { get { return this.TheOsEx.OldAmDug                    ; } }
   public decimal            O_OldAmPot                     { get { return this.TheOsEx.OldAmPot                    ; } }
   public decimal            O_NewAmDug                     { get { return this.TheOsEx.NewAmDug                    ; } }
   public decimal            O_NewAmPot                     { get { return this.TheOsEx.NewAmPot                    ; } }
   public bool               O_IsRashodovan                 { get { return this.TheOsEx.IsRashodovan                ; } }
   public decimal            O_InvSt                        { get { return this.TheOsEx.InvSt                       ; } }
   public decimal            O_JustCalculatedNewAmort       { get { return this.TheOsEx.JustCalculatedNewAmort      ; } }
   public decimal            O_JustCalculatedNewNormalAmort { get { return this.TheOsEx.JustCalculatedNewNormalAmort; } }
   public decimal            O_UkNabDugS                    { get { return this.TheOsEx.UkNabDugS                   ; } }
   public decimal            O_OldAmPotS                    { get { return this.TheOsEx.OldAmPotS                   ; } }
   public decimal            O_NewAmPotS                    { get { return this.TheOsEx.NewAmPotS                   ; } }
   public decimal            O_UkAmDug                      { get { return this.TheOsEx.UkAmDug                     ; } }
   public decimal            O_UkAmPot                      { get { return this.TheOsEx.UkAmPot                     ; } }
   public decimal            O_UkAmPotS                     { get { return this.TheOsEx.UkAmPotS                    ; } }
   public decimal            O_NewKnjVr                     { get { return this.TheOsEx.NewKnjVr                    ; } }
   public decimal            O_OldKnjVr                     { get { return this.TheOsEx.OldKnjVr                    ; } }
   public decimal            O_UkDugS                       { get { return this.TheOsEx.UkDugS                      ; } }
   public decimal            O_UkPotS                       { get { return this.TheOsEx.UkPotS                      ; } }
   public decimal            O_UkSALDO                      { get { return this.TheOsEx.UkSALDO                     ; } }
   public DateTime           O_DateAmRazdobljeStart         { get { return this.TheOsEx.DateAmRazdobljeStart        ; } }
   public DateTime           O_DateAmRazdobljeEnd           { get { return this.TheOsEx.DateAmRazdobljeEnd          ; } }
   public DateTime           O_DateNabava                   { get { return this.TheOsEx.DateNabava                  ; } }
   public DateTime           O_DateRashod                   { get { return this.TheOsEx.DateRashod                  ; } }

   #endregion TheOsEx - 'OsredStatus' property for 'Osred' class

   #region propertiz

   internal OsredStruct CurrentData // cijela OsredStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   internal OsredStruct BackupData // zasada samo za ovaj Osred record za potrebe RENAME USER-a
   {
      get { return this.backupData; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.OsredDao; }
   }

   public override string VirtualRecordName
   {
      get { return Osred.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Osred.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "os"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set { this.RecID = value; }
   }

   public override string SifraColName
   {
      get { return "osredCD"; }
   }

   public override string SifraColValue
   {
      get { return this.OsredCD; }
   }

   public override System.Data.DataRow SifraColDrSchema
   {
      get { return ZXC.OsredSchemaRows[ZXC.OsrCI.osredCD]; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Osred.sorterNaziv; }
   }

   /// <summary>
   /// A je podatak 'osredCD' kao link (foreign key) za druge tablice,
   /// izmijenjen u operaciji 'Edit'? 
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         return this.currentData._osredCD != this.backupData._osredCD;
      }
   }

   private List<Atrans> _atranses;
   /// <summary>
   /// Gets or sets a list of Atrans (ala customers orders) for this Osred.
   /// </summary>
   public List<Atrans> Atranses
   {
      get { return _atranses;         }
      set {        _atranses = value; }
   }

   public override void InvokeTransClear()
   {
      if(this.Atranses != null) this.Atranses.Clear();
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

   public string Naziv
   {
      get { return this.currentData._naziv; }
      set { this.currentData._naziv = value; }
   }

   public string OsredCD
   {
      get { return this.currentData._osredCD; }
      set { this.currentData._osredCD = value; }
   }

   public string BackupedOsredCD
   {
      get { return this.backupData._osredCD; }
   }

   public string Konto
   {
      get { return this.currentData._konto; }
      set { this.currentData._konto = value; }
   }

   public string KontoIv
   {
      get { return this.currentData._konto_iv; }
      set { this.currentData._konto_iv = value; }
   }

   /// <summary>
   /// AOP oznaka
   /// </summary>
   public string Grupa
   {
      get { return this.currentData._grupa; }
      set { this.currentData._grupa = value; }
   }

   public string StranaK
   {
      get { return this.currentData._strana_k; }
      set { this.currentData._strana_k = value; }
   }

   public string SerBr
   {
      get { return this.currentData._ser_br; }
      set { this.currentData._ser_br = value; }
   }

   public uint MtrosCd
   {
      get { return this.currentData._mtros_cd; }
      set { this.currentData._mtros_cd = value; }
   }

   public string MtrosTk
   {
      get { return this.currentData._mtros_tk; }
      set { this.currentData._mtros_tk = value; }
   }

   public uint KupdobCd
   {
      get { return this.currentData._kupdob_cd; }
      set { this.currentData._kupdob_cd = value; }
   }

   public string KupdobTk
   {
      get { return this.currentData._kupdob_tk; }
      set { this.currentData._kupdob_tk = value; }
   }

   public string DokumCd
   {
      get { return this.currentData._dokum_cd; }
      set { this.currentData._dokum_cd = value; }
   }

   public string Invest
   {
      get { return this.currentData._invest; }
      set {        this.currentData._invest = value; }
   }
   
   public string InvbrOd
   {
      get { return this.currentData._invbr_od; }
      set { this.currentData._invbr_od = value; }
   }
  
   public string InvbrDo
   {
      get { return this.currentData._invbr_do; }
      set { this.currentData._invbr_do = value; }
   }
  
   public string KoefAm
   {
      get { return this.currentData._koef_am; }
      set { this.currentData._koef_am = value; }
   }
  
   public decimal AmortSt
   {
      get { return this.currentData._amort_st; }
      set { this.currentData._amort_st = value; }
   }

   public decimal Vijek
   {
      get { return this.currentData._vijek; }
      set { this.currentData._vijek = value; }
   }

   public bool IsRashod
   {
      get { return this.currentData._isRashod; }
      set { this.currentData._isRashod = value; }
   }

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<OsredStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<OsredStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<OsredStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<OsredStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<OsredStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Osred newObject = new Osred();

      Generic_CloneData<OsredStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Osred MakeDeepCopy()
   {
      return (Osred)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Osred();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Osred)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Osred)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Osred>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Osred>(fileName);
   }


   #endregion VvDataRecordFactory

}
