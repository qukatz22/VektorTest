using System;

#region struct PtraneStruct

public struct PtraneStruct
{
   internal uint     _recID;

   /* 01 */  internal uint      _t_parentID     ;
   /* 02 */  internal uint      _t_dokNum       ;
   /* 03 */  internal ushort    _t_serial       ;
   /* 04 */  internal DateTime  _t_dokDate      ;
   /* 05 */  internal string    _t_tt           ;
   /* 06 */  internal uint      _t_ttNum        ;
   /* 07 */  internal uint      _t_personCD     ;
   /* 08 */  internal string    _t_ime          ;
   /* 09 */  internal string    _t_prezime      ;

   /* 10 */  internal string    _t_vrstaR_cd    ;   
   /* 11 */  internal string    _t_vrstaR_name  ;   
   /* 12 */  internal decimal   _t_cijPerc      ;   
   /* 13 */  internal decimal   _t_sati         ;   
   /* 14 */  internal string    _t_rsOO         ;   
   /* 15 */  internal uint      _t_rsOD         ;
   /* 16 */  internal uint      _t_rsDO         ;   
                                                
   /* 17 */  internal string    _t_stjecatCD    ;
   /* 18 */  internal string    _t_primDohCD    ;
   /* 19 */  internal string    _t_pocKrajCD    ;
   /* 20 */  internal uint      _t_ip1gr        ;

   /* 21 */  internal  int      _t_rbrIsprJop   ;
}

public struct PtraneResultStruct
{
  /* 01 */ internal decimal _t_evrCijena;	   
  /* 02 */ internal decimal _t_evrBruto;
  /* 03 */ internal decimal _t_thisEvrCijena;	   
  /* 04 */ internal decimal _t_thisEvrBruto;
}

#endregion struct PtraneStruct

public class Ptrane : VvTransRecord
{

   #region Fildz

   public const string recordName = "ptrane";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PtraneStruct currentData;
   private PtraneStruct backupData;

   private PtraneResultStruct _ptraneResult;

   // spec za ptrane: 
   protected static System.Data.DataTable TheSchemaTable = ZXC.PtraneDao.TheSchemaTable;
   protected static PtraneDao.PtraneCI    CI             = ZXC.PtraneDao.CI;

   public enum SpecEnum
   {
      NOVOZAPOSL, UMIROV, NIJE
   }
   public enum InvalidEnum
   {
      HRVI, INVALID, NIJE
   }
   public enum RSmSpecifikumEnum
   {
      POROD51, POROD52, POROD60, NIJE
   }

   public const string RSOO_NE_ZBRAJAJ_U_FOND = "99";
   public const string RSOO_PRIBRAJAJ_NA_FOND = "98";
   public const string VrstaR_cd_thisIs_ZPI   = "99";

   public const string RAD1_ONPN_rootCD       = "002"; // svi koji pocinju sa 002

   public static bool IsBolDo42(string rsOO, decimal cijPerc, string vrstaR_name)
   {
      bool isBkratko = (rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
                        cijPerc < 100M &&                              // Cijena manja od 100%
                        vrstaR_name.ToUpper().Contains("BOLOVANJE"));

      return isBkratko;
   }

   public static bool IsGodisnjiOdmor(string rsOO, string vrstaR_name)
   {
      bool isGodOdm = (rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
                       vrstaR_name.ToUpper().Contains("GODIŠNJI"));

      return isGodOdm;
   }

   public static bool IsPrekovremeno(string rsOO)
   {
      bool isPrekovremeno = (rsOO == Ptrane.RSOO_PRIBRAJAJ_NA_FOND); // 'RSOO_PRIBRAJAJ_NA_FOND' je uvijek i samo prekovremeno 

      return isPrekovremeno;
   }

   public static bool IsBlagdan(decimal cijPerc, string vrstaR_name)
   {
      bool isBlagdan =  (cijPerc == 100M && // ako je cijPerc > 100%, znaci da je to placeni rad na blagdan koji ovdje NE smije doci (npr RAD1) 
                        vrstaR_name.ToUpper().Contains("BLAGDAN"));

      return isBlagdan;
   }

   #endregion Fildz

   #region Constructors

   public Ptrane() : this(0)
   {
   }

   public Ptrane(uint ID) : base()
   {
      this.currentData = new PtraneStruct();

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

      /* 01 */   this.currentData._t_parentID  = 0;
      /* 02 */   this.currentData._t_dokNum    = 0;
      /* 03 */   this.currentData._t_serial    = 0;
      /* 04 */   this.currentData._t_dokDate   = DateTime.MinValue;
      /* 05 */   this.currentData._t_tt        = "";
      /* 06 */   this.currentData._t_ttNum     = 0;
      /* 07 */   this.currentData._t_personCD  = 0;
      /* 08 */   this.currentData._t_ime       = "";
      /* 09 */   this.currentData._t_prezime   = "";

      /* 10 */  this.currentData._t_vrstaR_cd  = "";   
      /* 11 */  this.currentData._t_vrstaR_name= "";   
      /* 12 */  this.currentData._t_cijPerc    = decimal.Zero;   
      /* 13 */  this.currentData._t_sati     = decimal.Zero;   
      /* 14 */  this.currentData._t_rsOO       = "";   
      /* 15 */  this.currentData._t_rsOD       = 0;
      /* 16 */  this.currentData._t_rsDO       = 0;

      /* 17 */  this.currentData._t_stjecatCD  = "";   
      /* 18 */  this.currentData._t_primDohCD  = "";   
      /* 19 */  this.currentData._t_pocKrajCD  = "";   
      /* 20 */  this.currentData._t_ip1gr      = 0;   
      /* 21 */  this.currentData._t_rbrIsprJop = 0;   

   }

   #endregion Constructors

   #region Sorters

   // sluzi samo za GTEREC kod PersonDao.GetPersonDokumDataList(); 
   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Ptrane.recordName, Ptrane.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokNum]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

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
      //get { return Ptrane.sorter_Person_DokDate_DokNum; }
      get 
      { 
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region propertiz

   internal PtraneStruct CurrentData // cijela PtraneStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PtraneDao; }
   }

   public override string VirtualRecordName
   {
      get { return Ptrane.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Ptrane.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Placa.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "pe"; }
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

   public static string PersonForeignKey
   {
      get { return "t_personCD"; }
   }

   public static string PlacaForeignKey
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

   //public DateTime AddTS
   //{
   //   get { return this.currentData._addTS; }
   //   set { this.currentData._addTS = value; }
   //}

   //public DateTime ModTS
   //{
   //   get { return this.currentData._modTS; }
   //   set { this.currentData._modTS = value; }
   //}

   //public string AddUID
   //{
   //   get { return this.currentData._addUID; }
   //   set { this.currentData._addUID = value; }
   //}

   //public string ModUID
   //{
   //   get { return this.currentData._modUID; }
   //   set { this.currentData._modUID = value; }
   //}

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
      set {        this.currentData._t_tt = value; }
   }
   /* 06 */
   public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public uint T_personCD
   {
      get { return this.currentData._t_personCD; }
      set {        this.currentData._t_personCD = value; }
   }
   /* 08 */ public string T_ime
   {
      get { return this.currentData._t_ime; }
      set {        this.currentData._t_ime = value; }
   }
   /* 09 */ public string T_prezime
   {
      get { return this.currentData._t_prezime; }
      set {        this.currentData._t_prezime = value; }
   }

   public string T_PrezimeIme
   {
      get { return Person.GetPrezimeIme(this.currentData._t_prezime, this.currentData._t_ime); }
   }

   /* 10 */public string T_vrstaR_cd
   {
      get { return this.currentData._t_vrstaR_cd; }
      set {        this.currentData._t_vrstaR_cd = value; }
   }
   /* 11 */public string T_vrstaR_name
   {
      get { return this.currentData._t_vrstaR_name; }
      set {        this.currentData._t_vrstaR_name = value; }
   }
   /* 12 */public decimal T_cijPerc
   {
      get { return this.currentData._t_cijPerc; }
      set {        this.currentData._t_cijPerc = value; }
   }
   /* 13 */public decimal T_sati
   {
      get { return this.currentData._t_sati; }
      set {        this.currentData._t_sati = value; }
   }
   /* 14 */public string T_rsOO
   {
      get { return this.currentData._t_rsOO; }
      set {        this.currentData._t_rsOO = value; }
   }
   /* 15 */public uint T_rsOD
   {
      get { return this.currentData._t_rsOD; }
      set {        this.currentData._t_rsOD = value; }
   }
   /* 16 */public uint T_rsDO
   {
      get { return this.currentData._t_rsDO; }
      set {        this.currentData._t_rsDO = value; }
   }
   
   /* 17 */public string T_stjecatCD { get { return this.currentData._t_stjecatCD; } set {this.currentData._t_stjecatCD = value; } }
   /* 18 */public string T_primDohCD { get { return this.currentData._t_primDohCD; } set {this.currentData._t_primDohCD = value; } }
   /* 19 */public string T_pocKrajCD { get { return this.currentData._t_pocKrajCD; } set {this.currentData._t_pocKrajCD = value; } }

   /* 20 */
   public uint T_ip1gr
   {
      get { return this.currentData._t_ip1gr; }
      set {        this.currentData._t_ip1gr = value; }
   }

   /* 21 */
   public int T_rbrIsprJop
   {
      get { return this.currentData._t_rbrIsprJop; }
      set {        this.currentData._t_rbrIsprJop = value; }
   }

   /* */

   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   public PtraneResultStruct PtraneResults
   {
      get { return this._ptraneResult;         }
      set {        this._ptraneResult = value; }
   }

   /* 01 */ public decimal R_EvrCijena
   { 
     get { return this._ptraneResult._t_evrCijena;         }
     set {        this._ptraneResult._t_evrCijena = value; }
   }	   
   /* 02 */ public decimal R_EvrBruto
   { 
      get { return this._ptraneResult._t_evrBruto;         }
      set {        this._ptraneResult._t_evrBruto = value; }
   }
   /* 03 */
   public decimal R_ThisEvrCijena
   {
      get { return this._ptraneResult._t_thisEvrCijena; }
      set {        this._ptraneResult._t_thisEvrCijena = value; }
   }
   /* 04 */
   public decimal R_ThisEvrBruto
   {
      get { return this._ptraneResult._t_thisEvrBruto; }
      set {        this._ptraneResult._t_thisEvrBruto = value; }
   }

   //public decimal R_RSm_Bruto  { get; set; }
   //public decimal R_RSm_MioOsn { get; set; }
   //public decimal R_RSm_Mio1Uk { get; set; }
   //public decimal R_RSm_Mio2Uk { get; set; }
   //public decimal R_RSm_Netto  { get; set; }

   //public bool IsThis_rsOO_HZZO_Pays
   //{
   //   get
   //   {
   //      if(
   //         T_rsOO == "50" ||
   //         T_rsOO == "58"
   //         )
   //      {
   //         return true;
   //      }
   //      else
   //      {
   //         return false;
   //      }
   //   }
   //}

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return "EviRad: " + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" + 
           "\n   Ser: " + T_serial + 
           "\n PerCd: " + T_personCD + 
           "\nPerson: " + T_PrezimeIme + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PtraneStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PtraneStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PtraneStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PtraneStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PtraneStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Ptrane newObject = new Ptrane();

      Generic_CloneData<PtraneStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Ptrane MakeDeepCopy()
   {
      return (Ptrane)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Ptrane();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Ptrane)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Ptrane)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory

}

