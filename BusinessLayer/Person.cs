using System;
using System.Collections.Generic;

#region struct PersonStruct

public struct PersonStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   /* 05 */ internal uint     _personCD      ;
   /* 06 */ internal string   _ime           ;
   /* 07 */ internal string   _prezime       ;
   /* 08 */ internal string   _ulica         ;   
   /* 09 */ internal string   _grad          ;     
   /* 10 */ internal string   _postaBr       ;  
   /* 11 */ internal string   _tel           ;     
   /* 12 */ internal string   _gsm           ;      
   /* 13 */ internal string   _email         ;
   /* 14 */ internal DateTime _datePri       ;
   /* 15 */ internal DateTime _dateOdj       ;
   /* 16 */ internal string   _strSpr        ;    
   /* 17 */ internal string   _strSprCd      ;    
   /* 18 */ internal string   _jmbg          ;    
   /* 19 */ internal string   _oib           ;    
   /* 20 */ internal string   _regob         ;    
   /* 21 */ internal string   _osBrOsig      ;    
   /* 22 */ internal uint  	_banka_cd      ;
   /* 23 */ internal string  	_banka_tk      ;
   /* 24 */ internal string   _pnbM          ;
   /* 25 */ internal string   _pnbV          ;
   /* 26 */ internal uint  	_mtros_cd      ;
   /* 27 */ internal string   _mtros_tk      ;
   /* 28 */ internal string   _radMj         ;
   /* 29 */ internal string   _napomena      ;
   /* 30 */ internal bool     _isIzuzet      ;
   /* 31 */ internal bool     _isPlaca       ;
   /* 32 */ internal bool     _isUgDj        ;
   /* 33 */ internal bool     _isAutH        ;
   /* 34 */ internal bool     _isPoduz       ;
   /* 35 */ internal bool     _isNadzO       ;
   /* 36 */ internal string   _zupan         ;
   /* 37 */ internal string   _zupCd         ;
   /* 38 */ internal DateTime _birthDate     ;
   /* 39 */ internal ushort   _vrstaRadVrem  ;
   /* 40 */ internal ushort   _vrstaRadOdns  ;
   /* 41 */ internal ushort   _vrstaIsplate  ;
   /* 42 */ internal ushort   _spol          ;
   /* 43 */ internal string   _ts            ;
   /* 44 */ internal string   _tsName        ;

   /* 45 */ internal uint  	_prevStazDD    ;
   /* 46 */ internal uint  	_prevStazMM    ;
   /* 47 */ internal uint  	_prevStazYY    ;

   /* 48 */ internal decimal 	_korKoef01     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 49 */ internal decimal 	_korKoef02     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 50 */ internal decimal 	_korKoef03     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 51 */ internal decimal 	_korKoef04     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 52 */ internal decimal 	_korKoef05     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 53 */ internal decimal 	_korKoef06     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 54 */ internal decimal 	_korKoef07     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 55 */ internal decimal 	_korKoef08     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 56 */ internal decimal 	_korKoef09     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 57 */ internal decimal 	_korKoef10     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 58 */ internal decimal 	_korKoef11     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /* 59 */ internal decimal 	_korKoef12     ; // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 

   /* 60 */ internal string  _birthMjestoDrz ;
   /* 61 */ internal string  _naravVrRad     ;
   /* 62 */ internal string  _drzavljanstvo  ;
   /* 63 */ internal string  _dozvola        ;
   /* 64 */ internal string  _prijava        ;
   /* 65 */ internal string  _strucno        ;
   /* 66 */ internal string  _zanimanje      ;
   /* 67 */ internal string  _trajanjeUgOdr  ;
   /* 68 */ internal string  _cl61ugSuglas   ;
   /* 69 */ internal string  _cl62ugSuglas   ;
   /* 70 */ internal string  _probniRad      ;
   /* 71 */ internal string  _pripravStaz    ;
   /* 72 */ internal string  _rezIspita      ;
   /* 73 */ internal string  _radIno         ;
   /* 74 */ internal string  _gdjeRadIno     ;
   /* 75 */ internal string  _ustupRadnika   ;
   /* 76 */ internal string  _ustupMjesto    ;
   /* 77 */ internal string  _drzavaPovezDrs ;
   /* 78 */ internal string  _posaoBenefStaz ;
   /* 79 */ internal string  _nacinBenef     ;
   /* 80 */ internal string  _sposobnost     ;
   /* 81 */ internal string  _mjestoRada     ;
   /* 82 */ internal string  _mirovanjeRO    ;
   /* 83 */ internal string  _razlogOdj      ;
   /* 84 */ internal string  _banka2TK       ;
   /* 85 */ internal uint    _banka2         ;
   /* 86 */ internal decimal _tfs            ;
   /* 87 */ internal decimal _skrRV          ;
   /* 88 */ internal bool    _isSO           ;
   /* 89 */ internal bool    _isOtherVr      ;

   /* 90 */ internal decimal _x_brutoOsn     ; // for IMPORT then for first dokument 
   /* 91 */ internal decimal _x_godStaza     ; // for IMPORT then for first dokument 
   /* 92 */ internal decimal _x_koef         ; // for IMPORT then for first dokument 
   /* 93 */ internal decimal _x_prijevoz     ; // for IMPORT then for first dokument 
   /* 94 */ internal decimal _x_dnFondSati   ; // for IMPORT then for first dokument 
   /* 95 */ internal string  _x_opcCD        ; // for IMPORT then for first dokument 
   /* 96 */ internal string  _x_opcRadCD     ; // for IMPORT then for first dokument 
   /* 97 */ internal bool    _x_isMioII      ; // for IMPORT then for first dokument 
   /* 98 */ internal bool    _x_isTrgov      ; // for IMPORT then for first dokument 

}

#endregion struct PersonStruct

public class Person : VvSifrarRecord
{

   #region Fildz

   public const string recordName       = "person";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PersonStruct currentData;
   private PersonStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.PersonDao.TheSchemaTable;
   protected static PersonDao.PersonCI CI                = ZXC.PersonDao.CI;

   public enum VrstaIsplateEnum
   {
      TEKUCI, BANKA, GOTOVINA
   }

   public enum IBAN_kind
   {
      TEKUCI_REDOVNI, TEKUCI_ZASTICENI, ZIRO_RACUN, BANKA_IBAN
   }

   public enum VrstaRadnogVremenaEnum
   {
      PUNO, NEPUNO, SKRACENO
   }

   public enum VrstaRadnogOdnosaEnum
   {
      NEODREDJENO, ODREDJENO, PRIPR_VJEZB
   }

   public enum RAD1G_StarosnaGrupaEnum
   {
      NEODREDJENO  , /* 00 */
      Starost_Do_18, /* 01 */
      Starost_19_24, /* 02 */
      Starost_25_29, /* 03 */
      Starost_30_34, /* 04 */
      Starost_35_39, /* 05 */
      Starost_40_44, /* 06 */
      Starost_45_49, /* 07 */
      Starost_50_54, /* 08 */
      Starost_55_59, /* 09 */
      Starost_60_64, /* 10 */
      Starost_65_xx  /* 11 */
   }

   public static string BankaForeignKey
   {
      get { return "banka_cd"; }
   }

   public static string MtrosForeignKey
   {
      get { return "mtros_cd"; }
   }

   public static string Banka2ForeignKey
   {
      get { return "banka2"; }
   }

   #endregion Fildz

   #region Sorters

   public static VvSQL.RecordSorter sorterPrezime = new VvSQL.RecordSorter(Person.recordName, Person.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prezime]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ime]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.personCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Prezime", VvSQL.SorterType.Person, false);

   public static VvSQL.RecordSorter sorterCD = new VvSQL.RecordSorter(Person.recordName, Person.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.personCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Sifra", VvSQL.SorterType.Code, false);

   private VvSQL.RecordSorter[] _sorters =
      new  VvSQL.RecordSorter[]
      { 
         sorterPrezime,
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
         case VvSQL.SorterType.Person: return new object[] { this.Prezime , this.Ime, this.PersonCD, RecVer };
         case VvSQL.SorterType.Code  : return new object[] { this.PersonCD,                          RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion Sorters

   #region Constructors

   public Person() : this(0)
   {
   }

   public Person(uint ID) : base()
   {
      this.currentData = new PersonStruct();

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

      this.currentData._personCD    = 0;
      this.currentData._ime         = "";
      this.currentData._prezime     = "";
      this.currentData._ulica       = "";
      this.currentData._grad        = "";
      this.currentData._postaBr     = "";
      this.currentData._tel         = "";
      this.currentData._gsm         = "";
      this.currentData._email       = "";
      this.currentData._datePri     = DateTime.MinValue;
      this.currentData._dateOdj     = DateTime.MinValue;
      this.currentData._strSpr      = "";
      this.currentData._strSprCd    = "";
      this.currentData._jmbg        = "";
      this.currentData._oib         = "";
      this.currentData._regob       = "";
      this.currentData._osBrOsig    = "";
      this.currentData._banka_cd    = 0;
      this.currentData._banka_tk    = "";
      this.currentData._pnbM        = "";
      this.currentData._pnbV        = "";
      this.currentData._mtros_cd    = 0;
      this.currentData._mtros_tk    = "";
      this.currentData._radMj       = "";
      this.currentData._napomena    = "";
      this.currentData._isIzuzet    = false;
      this.currentData._isPlaca     = false;
      this.currentData._isUgDj      = false;
      this.currentData._isAutH      = false;
      this.currentData._isPoduz     = false;
      this.currentData._isNadzO     = false;
      //this.currentData._zupan        = "";      
      this.currentData._zupCd       = "";      
      this.currentData._birthDate   = DateTime.MinValue;     
      this.currentData._vrstaRadVrem= 0;
      this.currentData._vrstaRadOdns= 0;
      this.currentData._vrstaIsplate= 0;
      this.currentData._spol        = 0; 
      this.currentData._ts          = ""; 
      this.currentData._tsName      = "";
      this.currentData._prevStazDD  = 0;
      this.currentData._prevStazMM  = 0;
      this.currentData._prevStazYY  = 0;

      this.currentData._korKoef01 = 0M;
      this.currentData._korKoef02 = 0M;
      this.currentData._korKoef03 = 0M;
      this.currentData._korKoef04 = 0M;
      this.currentData._korKoef05 = 0M;
      this.currentData._korKoef06 = 0M;
      this.currentData._korKoef07 = 0M;
      this.currentData._korKoef08 = 0M;
      this.currentData._korKoef09 = 0M;
      this.currentData._korKoef10 = 0M;
      this.currentData._korKoef11 = 0M;
      this.currentData._korKoef12 = 0M; 

      this.currentData._birthMjestoDrz  = ""; 
      this.currentData._naravVrRad      = ""; 
      this.currentData._drzavljanstvo   = ""; 
      this.currentData._dozvola         = ""; 
      this.currentData._prijava         = ""; 
      this.currentData._strucno         = ""; 
      this.currentData._zanimanje       = ""; 
      this.currentData._trajanjeUgOdr   = ""; 
      this.currentData._cl61ugSuglas    = ""; 
      this.currentData._cl62ugSuglas    = ""; 
      this.currentData._probniRad       = ""; 
      this.currentData._pripravStaz     = ""; 
      this.currentData._rezIspita       = ""; 
      this.currentData._radIno          = ""; 
      this.currentData._gdjeRadIno      = ""; 
      this.currentData._ustupRadnika    = ""; 
      this.currentData._ustupMjesto     = ""; 
      this.currentData._drzavaPovezDrs  = ""; 
      this.currentData._posaoBenefStaz  = ""; 
      this.currentData._nacinBenef      = ""; 
      this.currentData._sposobnost      = ""; 
      this.currentData._mjestoRada      = ""; 
      this.currentData._mirovanjeRO     = ""; 
      this.currentData._razlogOdj       = ""; 
      this.currentData._banka2TK        = ""; 
      this.currentData._banka2          = 0; 
      this.currentData._tfs             = 0M; 
      this.currentData._skrRV           = 0M; 
      this.currentData._isSO            = false; 
      this.currentData._isOtherVr       = false; 
      this.currentData._x_brutoOsn      = 0M;
      this.currentData._x_godStaza      = 0M;
      this.currentData._x_koef          = 0M;
      this.currentData._x_prijevoz      = 0M;
      this.currentData._x_dnFondSati    = 0M;
      this.currentData._x_opcCD         = "";
      this.currentData._x_opcRadCD      = "";
      this.currentData._x_isMioII       = false;
      this.currentData._x_isTrgov       = false;
   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return Prezime + ", " + Ime + "(" + PersonCD + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Person person_rec = (Person)vvDataRecord;

      if(restrictor == ZXC.AutoCompleteRestrictor.PER_RR_Only && person_rec.IsPlaca == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.PER_PP_Only && person_rec.IsPoduz == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.PER_NO_Only && person_rec.IsNadzO == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.PER_UD_Only && person_rec.IsUgDj  == false) return "";

      // 19.02.2020: zaboravili prije? 
      if(restrictor == ZXC.AutoCompleteRestrictor.PER_AHiAU_Only && person_rec.IsAutH  == false) return "";

      switch(sifrarType)
      {
         // 13.02.2020: HZTK... 
       //case VvSQL.SorterType.Person: return person_rec.Prezime;
         case VvSQL.SorterType.Person: return person_rec.PrezimeIme;
         case VvSQL.SorterType.Code:   return person_rec.PersonCD.ToString("000000");

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Person.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   #region propertiz

   internal PersonStruct CurrentData // cijela PersonStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   internal PersonStruct BackupData // zasada samo za ovaj Person record za potrebe RENAME USER-a
   {
      get { return this.backupData; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PersonDao; }
   }

   public override string VirtualRecordName
   {
      get { return Person.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Person.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "od"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set { this.RecID = value; }
   }

   /// <summary>
   /// Overrajdamo defaultno VvSifrarDataRecord ponasanje gdje je ovo true (za npr. sklad i osred)
   /// </summary>
   public override bool IsStringAutoSifra
   {
      get { return false; }
   }

   public override string SifraColName
   {
      get { return "personCD"; }
   }

   public override string SifraColValue
   {
      get { return this.PersonCD.ToString() + "_" + this.Prezime; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Person.sorterPrezime; }
   }

   /// <summary>
   /// A je podatak 'personCD' kao link (foreign key) za druge tablice,
   /// izmijenjen u operaciji 'Edit'? 
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         return this.currentData._personCD != this.backupData._personCD;
      }
   }

   private List<Ptrans> _ptranses;
   /// <summary>
   /// Gets or sets a list of Ptrans (ala customers orders) for this Person.
   /// </summary>
   public List<Ptrans> Ptranses
   {
      get { return _ptranses; }
      set {        _ptranses = value; }
   }

   private List<Ptrane> _ptranes;
   /// <summary>
   /// Gets or sets a list of Ptrane (ala customers orders) for this Person.
   /// </summary>
   public List<Ptrane> Ptranes
   {
      get { return _ptranes; }
      set { _ptranes = value; }
   }

   private List<Ptrano> _ptranos;
   /// <summary>
   /// Gets or sets a list of Ptrano (ala customers orders) for this Person.
   /// </summary>
   public List<Ptrano> Ptranos
   {
      get { return _ptranos; }
      set { _ptranos = value; }
   }

   public override void InvokeTransClear()
   {
      if(this.Ptranses != null) this.Ptranses.Clear();
      if(this.Ptranes  != null) this.Ptranes.Clear();
      if(this.Ptranos  != null) this.Ptranos.Clear();
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

   public uint BackupedPersonCD
   {
      get { return this.backupData._personCD; }
   }

   /*05*/
   public uint PersonCD
   {
      get { return this.currentData._personCD; }
      set {        this.currentData._personCD = value; }
   }

   /*06*/
   public string Ime
   {
      get { return this.currentData._ime; }
      set {        this.currentData._ime = value; }
   }

   /*07*/
   public string Prezime
   {
      get { return this.currentData._prezime; }
      set {        this.currentData._prezime = value; }
   }

   public static string GetPrezimeIme(string prezime, string ime)
   {
      return (prezime + ", " + ime);
   }

   public string PrezimeIme
   {
      get { return GetPrezimeIme(this.currentData._prezime, this.currentData._ime); }
   }

   public string ImePrezime
   {
      get { return (this.currentData._ime + " " + this.currentData._prezime); }

   }

   public string ZipGrad
   {
      get { return (this.currentData._postaBr + " " + this.currentData._grad); }

   }

   /*08*/
   public string Ulica
   {
      get { return this.currentData._ulica; }
      set {        this.currentData._ulica = value; }
   }

   /*09*/
   public string Grad
   {
      get { return this.currentData._grad; }
      set {        this.currentData._grad = value; }
   }

   /*10*/
   public string PostaBr
   {
      get { return this.currentData._postaBr; }
      set {        this.currentData._postaBr = value; }
   }

   /*11*/ 
   public string Tel
   {
      get { return this.currentData._tel; }
      set {        this.currentData._tel = value; }
   }

   /*12*/
   public string Gsm
   {
      get { return this.currentData._gsm; }
      set {        this.currentData._gsm = value; }
   }

   /*13*/ 
   public string Email
   {
      get { return this.currentData._email; }
      set {        this.currentData._email = value; }
   }

   /*14*/
   public DateTime DatePri
   {
      get { return this.currentData._datePri; }
      set {        this.currentData._datePri = value; }
   }

   /*15*/
   public DateTime DateOdj
   {
      get { return this.currentData._dateOdj; }
      set {        this.currentData._dateOdj = value; }
   }

   /*16*/
   public string StrSpr
   {
      get { return this.currentData._strSpr; }
      set {        this.currentData._strSpr = value; }
   }

   /*17*/
   public string StrSprCd
   {
      get { return this.currentData._strSprCd; }
      set {        this.currentData._strSprCd = value; }
   }

   /*18*/
   public string Jmbg
   {
      get { return this.currentData._jmbg; }
      set {        this.currentData._jmbg = value; }
   }

   /*19*/
   public string Oib
   {
      get { return this.currentData._oib; }
      set {        this.currentData._oib = value; }
   }

   /*20*/
   public string Regob
   {
      get { return this.currentData._regob; }
      set {        this.currentData._regob = value; }
   }

   /*21*/
   public string OsBrOsig
   {
      get { return this.currentData._osBrOsig; }
      set {        this.currentData._osBrOsig = value; }
   }

   /*22*/
   public uint BankaCd
   {
      get { return this.currentData._banka_cd; }
      set {        this.currentData._banka_cd = value; }
   }

   /*23*/
   public string BankaTk
   {
      get { return this.currentData._banka_tk; }
      set {        this.currentData._banka_tk = value; }
   }

   /*24*/
   public string PnbM
   {
      get { return this.currentData._pnbM; }
      set {        this.currentData._pnbM = value; }
   }

   /*25*/
   public string PnbV
   {
      get { return this.currentData._pnbV; }
      set {        this.currentData._pnbV = value; }
   }

   /*26*/
   public uint MtrosCd
   {
      get { return this.currentData._mtros_cd; }
      set {        this.currentData._mtros_cd = value; }
   }

   /*27*/   
   public string MtrosTk
   {
      get { return this.currentData._mtros_tk; }
      set {        this.currentData._mtros_tk = value; }
   }

   /*28*/
   public string RadMj
   {
      get { return this.currentData._radMj; }
      set {        this.currentData._radMj = value; }
   }

   /*29*/
   public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }

   /*30*/
   public bool IsIzuzet
   {
      get { return this.currentData._isIzuzet; }
      set {        this.currentData._isIzuzet = value; }
   }

   /*31*/
   public bool IsPlaca
   {
      get { return this.currentData._isPlaca; }
      set {        this.currentData._isPlaca = value; }
   }

   /*32*/
   public bool IsUgDj
   {
      get { return this.currentData._isUgDj; }
      set {        this.currentData._isUgDj = value; }
   }

   /*33*/
   public bool IsAutH
   {
      get { return this.currentData._isAutH; }
      set {        this.currentData._isAutH = value; }
   }

   /*34*/
   public bool IsPoduz
   {
      get { return this.currentData._isPoduz; }
      set {        this.currentData._isPoduz = value; }
   }

   /*35*/
   public bool IsNadzO
   {
      get { return this.currentData._isNadzO; }
      set {        this.currentData._isNadzO = value; }
   }

   /*36*/
   public string Zupan
   {
      get
      {
         return ZXC.luiListaZupanija.GetNameForThisCd(ZupCd);
      }
      //set {        this.currentData._zupan = value; }
   }

   /*37*/
   public string ZupCd
   {
      get { return this.currentData._zupCd; }
      set {        this.currentData._zupCd = value; }
   }

   /*38*/
   public DateTime BirthDate
   {
      get { return this.currentData._birthDate; }
      set {        this.currentData._birthDate = value; }
   }

   public int StarostUgodinama_Na_31_03_ProjectYear
   {
      get 
      {
         if(this.BirthDate.IsEmpty()) return 0;

         DateTime date3103YY = new DateTime(ZXC.ValOrZero_Int(ZXC.projectYear), 03, 31);

         //TimeSpan personAge_On_31_03_ProjectYear = date3103YY - this.BirthDate;

         return ZXC.YearDifference(this.BirthDate, date3103YY); 
      }
   }

   public Person.RAD1G_StarosnaGrupaEnum RAD1G_StarosnaGrupa
   {
      get 
      {
         if(                                               StarostUgodinama_Na_31_03_ProjectYear <= 18) return Person.RAD1G_StarosnaGrupaEnum.Starost_Do_18;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 19 && StarostUgodinama_Na_31_03_ProjectYear <= 24) return Person.RAD1G_StarosnaGrupaEnum.Starost_19_24;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 25 && StarostUgodinama_Na_31_03_ProjectYear <= 29) return Person.RAD1G_StarosnaGrupaEnum.Starost_25_29;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 30 && StarostUgodinama_Na_31_03_ProjectYear <= 34) return Person.RAD1G_StarosnaGrupaEnum.Starost_30_34;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 35 && StarostUgodinama_Na_31_03_ProjectYear <= 39) return Person.RAD1G_StarosnaGrupaEnum.Starost_35_39;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 40 && StarostUgodinama_Na_31_03_ProjectYear <= 44) return Person.RAD1G_StarosnaGrupaEnum.Starost_40_44;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 45 && StarostUgodinama_Na_31_03_ProjectYear <= 49) return Person.RAD1G_StarosnaGrupaEnum.Starost_45_49;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 50 && StarostUgodinama_Na_31_03_ProjectYear <= 54) return Person.RAD1G_StarosnaGrupaEnum.Starost_50_54;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 55 && StarostUgodinama_Na_31_03_ProjectYear <= 59) return Person.RAD1G_StarosnaGrupaEnum.Starost_55_59;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 60 && StarostUgodinama_Na_31_03_ProjectYear <= 64) return Person.RAD1G_StarosnaGrupaEnum.Starost_60_64;
         if(StarostUgodinama_Na_31_03_ProjectYear >= 65                                               ) return Person.RAD1G_StarosnaGrupaEnum.Starost_65_xx;

         return Person.RAD1G_StarosnaGrupaEnum.NEODREDJENO;
      }
   }

   /*39*/
   public Person.VrstaRadnogVremenaEnum VrstaRadVrem
   {
      get { return (Person.VrstaRadnogVremenaEnum)this.currentData._vrstaRadVrem; }
      set {                                       this.currentData._vrstaRadVrem = (ushort)value; }
   }

   /*40*/
   public Person.VrstaRadnogOdnosaEnum VrstaRadOdns
   {
      get { return (Person.VrstaRadnogOdnosaEnum)this.currentData._vrstaRadOdns; }
      set {                                      this.currentData._vrstaRadOdns = (ushort)value; }
   }

   /*41*/
   public Person.VrstaIsplateEnum VrstaIsplate
   {
      get { return (Person.VrstaIsplateEnum)this.currentData._vrstaIsplate; }
      set {                                 this.currentData._vrstaIsplate = (ushort)value; }
   }

   /*42*/
   internal ZXC.Spol Spol
   {
      get { return (ZXC.Spol)this.currentData._spol; }
      set {                  this.currentData._spol =(ushort) value; }
   }

   /*43*/
   internal string TS
   {
      get { return this.currentData._ts; }
      set {        this.currentData._ts = value; }
   }

   /*44*/
   internal string TsName
   {
      get { return this.currentData._tsName; }
      set { this.currentData._tsName = value; }
   }

   /*45*/
   public uint PrevStazDD
   {
      get { return this.currentData._prevStazDD; }
      set {        this.currentData._prevStazDD = value; }
   }

   /*46*/
   public uint PrevStazMM
   {
      get { return this.currentData._prevStazMM; }
      set {        this.currentData._prevStazMM = value; }
   }

   /*47*/
   public uint PrevStazYY
   {
      get { return this.currentData._prevStazYY; }
      set {        this.currentData._prevStazYY = value; }
   }

   /*48*/ public decimal KorKoef01 { get { return this.currentData._korKoef01; } set { this.currentData._korKoef01 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*49*/ public decimal KorKoef02 { get { return this.currentData._korKoef02; } set { this.currentData._korKoef02 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*50*/ public decimal KorKoef03 { get { return this.currentData._korKoef03; } set { this.currentData._korKoef03 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*51*/ public decimal KorKoef04 { get { return this.currentData._korKoef04; } set { this.currentData._korKoef04 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*52*/ public decimal KorKoef05 { get { return this.currentData._korKoef05; } set { this.currentData._korKoef05 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*53*/ public decimal KorKoef06 { get { return this.currentData._korKoef06; } set { this.currentData._korKoef06 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*54*/ public decimal KorKoef07 { get { return this.currentData._korKoef07; } set { this.currentData._korKoef07 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*55*/ public decimal KorKoef08 { get { return this.currentData._korKoef08; } set { this.currentData._korKoef08 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*56*/ public decimal KorKoef09 { get { return this.currentData._korKoef09; } set { this.currentData._korKoef09 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*57*/ public decimal KorKoef10 { get { return this.currentData._korKoef10; } set { this.currentData._korKoef10 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*58*/ public decimal KorKoef11 { get { return this.currentData._korKoef11; } set { this.currentData._korKoef11 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 
   /*59*/ public decimal KorKoef12 { get { return this.currentData._korKoef12; } set { this.currentData._korKoef12 = value; } } // korekcija koeficijenta osobnog odbitka. ako je 0, znaci da je i kod obracuna bio dobar. 

   /* 60 */ public string  BirthMjestoDrz  { get { return this.currentData._birthMjestoDrz; } set { this.currentData._birthMjestoDrz = value; } } 
   /* 61 */ public string  NaravVrRad      { get { return this.currentData._naravVrRad    ; } set { this.currentData._naravVrRad     = value; } } 
   /* 62 */ public string  Drzavljanstvo   { get { return this.currentData._drzavljanstvo ; } set { this.currentData._drzavljanstvo  = value; } } 
   /* 63 */ public string  Dozvola         { get { return this.currentData._dozvola       ; } set { this.currentData._dozvola        = value; } } 
   /* 64 */ public string  Prijava         { get { return this.currentData._prijava       ; } set { this.currentData._prijava        = value; } } 
   /* 65 */ public string  Strucno         { get { return this.currentData._strucno       ; } set { this.currentData._strucno        = value; } } 
   /* 66 */ public string  Zanimanje       { get { return this.currentData._zanimanje     ; } set { this.currentData._zanimanje      = value; } } 
   /* 67 */ public string  TrajanjeUgOdr   { get { return this.currentData._trajanjeUgOdr ; } set { this.currentData._trajanjeUgOdr  = value; } } 
   /* 68 */ public string  Cl61ugSuglas    { get { return this.currentData._cl61ugSuglas  ; } set { this.currentData._cl61ugSuglas   = value; } } 
   /* 69 */ public string  Cl62ugSuglas    { get { return this.currentData._cl62ugSuglas  ; } set { this.currentData._cl62ugSuglas   = value; } } 
   /* 70 */ public string  ProbniRad       { get { return this.currentData._probniRad     ; } set { this.currentData._probniRad      = value; } } 
   /* 71 */ public string  PripravStaz     { get { return this.currentData._pripravStaz   ; } set { this.currentData._pripravStaz    = value; } } 
   /* 72 */ public string  RezIspita       { get { return this.currentData._rezIspita     ; } set { this.currentData._rezIspita      = value; } } 
   /* 73 */ public string  RadIno          { get { return this.currentData._radIno        ; } set { this.currentData._radIno         = value; } } 
   /* 74 */ public string  GdjeRadIno      { get { return this.currentData._gdjeRadIno    ; } set { this.currentData._gdjeRadIno     = value; } } 
   /* 75 */ public string  UstupRadnika    { get { return this.currentData._ustupRadnika  ; } set { this.currentData._ustupRadnika   = value; } } 
   /* 76 */ public string  UstupMjesto     { get { return this.currentData._ustupMjesto   ; } set { this.currentData._ustupMjesto    = value; } } 
   /* 77 */ public string  DrzavaPovezDrs  { get { return this.currentData._drzavaPovezDrs; } set { this.currentData._drzavaPovezDrs = value; } } 
   /* 78 */ public string  PosaoBenefStaz  { get { return this.currentData._posaoBenefStaz; } set { this.currentData._posaoBenefStaz = value; } } 
   /* 79 */ public string  NacinBenef      { get { return this.currentData._nacinBenef    ; } set { this.currentData._nacinBenef     = value; } } 
   /* 80 */ public string  Sposobnost      { get { return this.currentData._sposobnost    ; } set { this.currentData._sposobnost     = value; } } 
   /* 81 */ public string  MjestoRada      { get { return this.currentData._mjestoRada    ; } set { this.currentData._mjestoRada     = value; } } 
   /* 82 */ public string  MirovanjeRO     { get { return this.currentData._mirovanjeRO   ; } set { this.currentData._mirovanjeRO    = value; } } 
   /* 83 */ public string  RazlogOdj       { get { return this.currentData._razlogOdj     ; } set { this.currentData._razlogOdj      = value; } } 
   /* 84 */ public string  Banka2TK        { get { return this.currentData._banka2TK      ; } set { this.currentData._banka2TK       = value; } } 
   /* 85 */ public uint    Banka2          { get { return this.currentData._banka2        ; } set { this.currentData._banka2         = value; } } 
   /* 86 */ public decimal Tfs             { get { return this.currentData._tfs           ; } set { this.currentData._tfs            = value; } } 
   /* 87 */ public decimal SkrRV           { get { return this.currentData._skrRV         ; } set { this.currentData._skrRV          = value; } } 
   /* 88 */ public bool    IsSO            { get { return this.currentData._isSO          ; } set { this.currentData._isSO           = value; } } 
   /* 89 */ public bool    IsOtherVr       { get { return this.currentData._isOtherVr     ; } set { this.currentData._isOtherVr      = value; } } 

   /* 90 */ public decimal X_brutoOsn      { get { return this.currentData._x_brutoOsn    ; } set { this.currentData._x_brutoOsn     = value; } } 
   /* 91 */ public decimal X_godStaza      { get { return this.currentData._x_godStaza    ; } set { this.currentData._x_godStaza     = value; } } 
   /* 92 */ public decimal X_koef          { get { return this.currentData._x_koef        ; } set { this.currentData._x_koef         = value; } } 
   /* 93 */ public decimal X_prijevoz      { get { return this.currentData._x_prijevoz    ; } set { this.currentData._x_prijevoz     = value; } } 
   /* 94 */ public decimal X_dnFondSati    { get { return this.currentData._x_dnFondSati  ; } set { this.currentData._x_dnFondSati   = value; } } 
   /* 95 */ public string  X_opcCD         { get { return this.currentData._x_opcCD       ; } set { this.currentData._x_opcCD        = value; } } 
   /* 96 */ public string  X_opcRadCD      { get { return this.currentData._x_opcRadCD    ; } set { this.currentData._x_opcRadCD     = value; } } 
   /* 97 */ public bool    X_isMioII       { get { return this.currentData._x_isMioII     ; } set { this.currentData._x_isMioII      = value; } } 
   /* 98 */ public bool    X_isTrgov       { get { return this.currentData._x_isTrgov     ; } set { this.currentData._x_isTrgov      = value; } } 
 
   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PersonStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PersonStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PersonStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PersonStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PersonStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Person newObject = new Person();

      Generic_CloneData<PersonStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Person MakeDeepCopy()
   {
      return (Person)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Person();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Person)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Person)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Person>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Person>(fileName);
   }


   #endregion VvDataRecordFactory


   #region Calc Staz

   public uint CalcGodineStaza(string placaZaMMYYYY, bool ignorePrevStaz)
   {
      uint godineStaza = 0;

      if(this.DatePri.IsEmpty())
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Upozorenje!\n\nDjelatnik {0}\n\n nema datum prijave!", this);

         return 0;
      }

      DateTime stazDateOD = this.DatePri;
      DateTime stazDateDO = Placa.GetDateTimeFromMMYYYY(placaZaMMYYYY, true);

      if(ignorePrevStaz == false && (this.PrevStazDD + this.PrevStazMM + this.PrevStazYY).NotZero())
      {
         int prevDaysCumulative = (int)((this.PrevStazYY * 365) + (this.PrevStazMM * 30) + this.PrevStazDD);

         TimeSpan prevFirmaStaz = new TimeSpan(prevDaysCumulative, 0 /*hours*/, 0 /*minutes*/, 0 /*seconds*/);

         stazDateOD -= prevFirmaStaz;
      }

      int mm, yy;

      if(stazDateOD.Month == 12)
      {
         mm = 1;
         yy = stazDateOD.Year + 1;
      }
      else
      {
         mm = stazDateOD.Month + 1;
         yy = stazDateOD.Year;
      }

      stazDateOD = new DateTime(yy, mm, 1);

      // 12.05.2015: 
      if(stazDateOD.Date > stazDateDO.Date) godineStaza = 0;
      else                                  godineStaza = Convert.ToUInt32(Math.Floor(((stazDateDO.Date - stazDateOD.Date).TotalDays / 365)));

      return godineStaza;
   }

   public void IssueGodineStazaWarning(decimal ptransT_godStaza, uint calculatedGodineStaza)
   {
      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning,
         "Upozorenje!\n\nDjelatnik {0}\n\n na dokumentu ima navedeno {1} godina staža\n\na prema datumu prijave {2}\n\ni dotadašnjem stažu {3} god. {4} mjes. {5} dana\n\ntreba imati {6} godina staža!",
         this, ptransT_godStaza.ToString0Vv(), this.DatePri.ToString(ZXC.VvDateFormat), this.PrevStazYY, this.PrevStazMM, this.PrevStazDD, calculatedGodineStaza);
   }

   #endregion Calc Staz

   public Kupdob BankaKupdob 
   { 
      get 
      {
         Kupdob kupdob_rec = ZXC.TheVvForm.TheVvUC.Get_Kupdob_FromVvUcSifrar(this.BankaCd);

         if(kupdob_rec == null) kupdob_rec = new Kupdob();

         return kupdob_rec;
      } 
   }

   public bool Is_BankaKupdob_Ziro1_Normal { get { return BankaKupdob.Ziro1_asIBAN.SubstringSafe(11, 2) == VvPlacaReport.Normal_IBAN_root; } }
   public bool Is_BankaKupdob_Ziro1_Zastic { get { return BankaKupdob.Ziro1_asIBAN.SubstringSafe(11, 2) == VvPlacaReport.Zastic_IBAN_root; } }
   public bool Is_BankaKupdob_Ziro1_ZiroRn { get { return BankaKupdob.Ziro1_asIBAN.SubstringSafe(11, 2) == VvPlacaReport.ZiroRn_IBAN_root; } }

   public static bool CheckIBANroot(IBAN_kind theIBAN_kind, string theIBAN)
   {
      switch (theIBAN_kind)
      {
         case IBAN_kind.TEKUCI_REDOVNI  : return theIBAN.SubstringSafe(11, 2) == VvPlacaReport.Normal_IBAN_root;
         case IBAN_kind.TEKUCI_ZASTICENI: return theIBAN.SubstringSafe(11, 2) == VvPlacaReport.Zastic_IBAN_root;
         case IBAN_kind.ZIRO_RACUN      : return theIBAN.SubstringSafe(11, 2) == VvPlacaReport.ZiroRn_IBAN_root;
      }

      return true;
   }

   #region Person 2 Kupdob & ViceVersa

   public static void CopyData_FromPerson_ToKupdob(Kupdob kupdob_rec, Person person_rec, bool isTekuci)
   {
      kupdob_rec.Naziv   = isTekuci ? person_rec.Ime : person_rec.Prezime; // TODO 
      kupdob_rec.Ime     = person_rec.Ime    ;
      kupdob_rec.Prezime = person_rec.Prezime;
   }

   public static void CopyData_FromKupdob_ToPerson(Person person_rec, Kupdob kupdob_rec, bool isTekuci)
   {
      person_rec.Ime     = kupdob_rec.Ime    ;
      person_rec.Prezime = kupdob_rec.Prezime;
   }

   #endregion Person 2 Kupdob & ViceVersa

}
