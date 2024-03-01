using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;
using static ArtiklDao;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct ArtiklStruct

public struct ArtiklStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

/* /* 05 */ internal string   _artiklCD      ;
/* /* 06 */ internal string   _artiklName    ;
/* /* 07 */ internal string   _barCode1      ;
/* /* 08 */ internal string   _skladCD       ;
/* /* 09 */ internal string   _grupa1CD      ;   
/* /* 10 */ internal string   _jedMj         ;     
/* /* 11 */ internal string   _konto         ;  
   /* 12 */ internal string   _artiklCD2     ; // (npr dobavljaceva sifra, ATK sifra...)
   /* 13 */ internal string   _artiklName2   ; // (npr strani naziv ili naziv dobavljaca, genericko ime...)
/* /* 14 */ internal string   _ts            ; // (roba, mater, vlproizv, usluga, ambalaza, uzorak, prolazna stavka, takse, opis(kod printanja printaj longOpis a ne naziv artikla)
   /* 15 */ internal string   _barCode2      ;
   /* 16 */ internal string   _serNo         ;
   /* 17 */ internal string   _grupa2CD      ;   
   /* 18 */ internal string   _grupa3CD      ;   
   /* 19 */ internal string   _placement     ;   // smjestaj, polica, ...
   /* 20 */ internal string   _linkArtCD     ;   // vezani artikl, ambalaza, sadrzaj, dobavljacev komplet naziva-sifre-jedMj-...(centrala/jedinica pattern)
   /* 21 */ internal DateTime _dateProizv    ;   // godina, datum proizvodnje
/* /* 22 */ internal string   _pdvKat        ;   // PDV kategorija (tarifni broj) lookup...
/* /* 23 */ internal string   _longOpis      ;   // vezan na _ts
   /* 24 */ internal string   _prefValName   ;   // preferirana oznaka valute (npr za linkArtCD)
   /* 25 */ internal string   _orgPak        ;   // opis orginalnog pakiranja (VFarm pattern)
/* /* 26 */ internal bool     _isRashod      ;   // NEAKTIVNA kartica...
   /* 27 */ internal bool     _isAkcija      ;   // artikl na akciji...
   /* 28 */ internal bool     _isMaster      ;   // za Centrala/Jedinica pattern
   /* 29 */ internal decimal  _masaNetto     ; 
   /* 30 */ internal decimal  _masaBruto     ; 
   /* 31 */ internal decimal  _promjer       ; 
   /* 32 */ internal decimal  _povrsina      ; 
   /* 33 */ internal decimal  _zapremina     ; 
   /* 34 */ internal decimal  _duljina       ; 
   /* 35 */ internal decimal  _sirina        ; 
   /* 36 */ internal decimal  _visina        ; 
   /* 37 */ internal decimal  _starost       ; 
   /* 38 */ internal string   _boja          ; 
   /* 39 */ internal ushort   _spol          ;  // M, Z, unisex

   /* 40 */ internal ushort   _garancija     ;  
   /* 41 */ internal uint     _dobavCD       ;    
   /* 42 */ internal uint     _proizCD       ;    
   /* 43 */ internal bool     _isAllowMinus  ; 
   /* 44 */ internal bool     _isSerNo       ; 
   /* 45 */ internal string   _masaNettoJM   ; 
   /* 46 */ internal string   _masaBrutoJM   ; 
   /* 47 */ internal string   _promjerJM     ; 
   /* 48 */ internal string   _povrsinaJM    ; 
   /* 59 */ internal string   _zapreminaJM   ; 
   /* 50 */ internal string   _duljinaJM     ; 
   /* 51 */ internal string   _sirinaJM      ; 
   /* 52 */ internal string   _visinaJM      ; 
   /* 53 */ internal string   _velicina      ; 
   /* 54 */ internal string   _madeIn        ; 
   /* 55 */ internal string   _url           ; 
   /* 56 */ internal string   _atestBr       ; 
   /* 57 */ internal DateTime _atestDate     ; 
   /* 58 */ internal ushort   _vpc1Policy    ;
   /* 59 */ internal bool     _isPrnOpis     ;   // Printaj Opis umjesto Naziv-a 
   /* 60 */ internal string   _carTarifa     ; 
   /* 61 */ internal string   _partNo        ; 
   /* 62 */ internal string   _napomena      ; 
   /* 63 */ internal decimal  _importCij     ; 

   /* 64 */ internal decimal  _snaga         ; 
   /* 65 */ internal string   _snagaJM       ; 

   /* 66 */ internal ushort   _emisCO2       ; 
   /* 67 */ internal ushort   _euroNorma     ; 

}

#endregion struct ArtiklStruct

public class Artikl : VvSifrarRecord
{

   #region Fildz

   public const string recordName       = "artikl";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private ArtiklStruct currentData;
   private ArtiklStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.ArtiklDao.TheSchemaTable;
   protected static ArtiklDao.ArtiklCI CI                = ZXC.ArtiklDao.CI;

   public const string OtpadArtiklCD = "OTPAD"    ;
   public const string PljvnArtiklCD = "PILJEVINA";

   // 13.04.2018: preseljeno u ZXC 
 //public const string MotVoziloGrCD = "MOT";
 //public const string UmjetninaGrCD = "UMJ";
   public const string KomisRobaGrCD = "KMR";

   public const string NabRobaGrCD   = "NBKG" ;
   public const string ProdRobaGrCD  = "PRKOM";
   public const string ProAndNabGrCD = "NBiPR";

   public const string FinKorekArtTS = "FKZ"; // financijska korekcija financijskog stanja skladista (zaliha) 

   #endregion Fildz

   #region Sorters

   public static VvSQL.RecordSorter sorterName = new VvSQL.RecordSorter(Artikl.recordName, Artikl.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklCD  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer    ], true)
      }, "Naziv", VvSQL.SorterType.Name, false);

   public static VvSQL.RecordSorter sorterCD = new VvSQL.RecordSorter(Artikl.recordName, Artikl.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer  ], true)
      }, "Sifra", VvSQL.SorterType.Code, false);

   public static VvSQL.RecordSorter sorterBCode = new VvSQL.RecordSorter(Artikl.recordName, Artikl.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.barCode1]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer  ], true)
      }, "BarKod", VvSQL.SorterType.BarCode, false);

   public static VvSQL.RecordSorter sorterName2 = new VvSQL.RecordSorter(Artikl.recordName, Artikl.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklName2]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklName ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer     ], true)
      }, "Naziv2", VvSQL.SorterType.Name2, false);

   public static VvSQL.RecordSorter sorterCD2 = new VvSQL.RecordSorter(Artikl.recordName, Artikl.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklCD2 ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.artiklName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer    ], true)
      }, "Sifra2", VvSQL.SorterType.Code2, false);

   private VvSQL.RecordSorter[] _sorters =
      new  VvSQL.RecordSorter[]
      { 
         sorterName,
         sorterCD,
         sorterBCode,
         sorterName2,
         sorterCD2
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Name   : return new object[] { this.ArtiklName , this.ArtiklCD  , RecVer };
         case VvSQL.SorterType.Code   : return new object[] { this.ArtiklCD   ,                  RecVer };
         case VvSQL.SorterType.BarCode: return new object[] { this.BarCode1   , this.ArtiklCD  , RecVer };

         case VvSQL.SorterType.Name2  : return new object[] { this.ArtiklName2, this.ArtiklName, RecVer };
         case VvSQL.SorterType.Code2  : return new object[] { this.ArtiklCD2  , this.ArtiklName, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion Sorters

   #region Constructors

   public Artikl() : this(0)
   {
   }

   public Artikl(uint ID) : base()
   {
      this.currentData = new ArtiklStruct();

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

      this.currentData._artiklCD      = "";
      this.currentData._artiklName    = "";
      this.currentData._barCode1      = "";
      this.currentData._skladCD       = "";
      this.currentData._grupa1CD      = "";
      this.currentData._jedMj         = "";
      this.currentData._konto         = "";
      this.currentData._artiklCD2     = "";
      this.currentData._artiklName2   = "";
      this.currentData._ts            = "";
      this.currentData._barCode2      = "";
      this.currentData._serNo         = "";
      this.currentData._grupa2CD      = "";
      this.currentData._grupa3CD      = "";
      this.currentData._placement     = "";
      this.currentData._linkArtCD     = "";
      this.currentData._dateProizv    = DateTime.MinValue;
      this.currentData._pdvKat        = "";
      this.currentData._longOpis      = "";
      this.currentData._prefValName   = "";
      this.currentData._orgPak        = "";
      this.currentData._isRashod      = false;
      this.currentData._isAkcija      = false;
      this.currentData._isMaster      = false;
      this.currentData._masaNetto     = 0.00M;
      this.currentData._masaBruto     = 0.00M;
      this.currentData._promjer       = 0.00M;
      this.currentData._povrsina      = 0.00M;
      this.currentData._zapremina     = 0.00M;
      this.currentData._duljina       = 0.00M;
      this.currentData._sirina        = 0.00M;
      this.currentData._visina        = 0.00M;
      this.currentData._starost       = 0.00M;
      this.currentData._boja          = "";
      this.currentData._spol          = 0;

      this.currentData._masaNettoJM   = "";
      this.currentData._masaBrutoJM   = "";
      this.currentData._promjerJM     = "";
      this.currentData._povrsinaJM    = "";
      this.currentData._zapreminaJM   = "";
      this.currentData._duljinaJM     = "";
      this.currentData._sirinaJM      = "";
      this.currentData._visinaJM      = "";
      this.currentData._velicina      = "";
      this.currentData._garancija     = 0;
      this.currentData._madeIn        = "";
      this.currentData._atestBr       = "";
      this.currentData._atestDate     = DateTime.MinValue;
      this.currentData._url           = "";
      this.currentData._dobavCD       = 0;
      this.currentData._proizCD       = 0;
      this.currentData._isAllowMinus  = false;
      this.currentData._isSerNo       = false;
      this.currentData._vpc1Policy    = 0;
      this.currentData._isPrnOpis     = false;
      this.currentData._carTarifa     = "";
      this.currentData._partNo        = "";
      this.currentData._napomena      = "";
      this.currentData._importCij     = 0.00M;
      this.currentData._snaga         = 0.00M;
      this.currentData._snagaJM       = "";
      this.currentData._emisCO2       = 0;
      this.currentData._euroNorma     = 0;

      this.TheAsEx = new ArtStat();

   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return ArtiklCD + " (" + ArtiklName + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Artikl artikl_rec = (Artikl)vvDataRecord;

      // 02.05.2022: 
      if(restrictor == ZXC.AutoCompleteRestrictor.ART_NonRashod_Only && artikl_rec.IsRashod == true) return "";

      switch(sifrarType)
      {
         case VvSQL.SorterType.Name   : return artikl_rec.ArtiklName     ;
         case VvSQL.SorterType.Code   : return artikl_rec.ArtiklCD       ;
         case VvSQL.SorterType.BarCode: return /*artikl_rec.BarCode1*/ ""; // !!! da ne puni bezveze 

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Artikl.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   #region propertiz

   #region General propertiez

   internal ArtiklStruct CurrentData // cijela ArtiklStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   internal ArtiklStruct BackupData // zasada samo za ovaj Artikl record za potrebe RENAME USER-a
   {
      get { return this.backupData; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.ArtiklDao; }
   }

   public override string VirtualRecordName
   {
      get { return Artikl.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Artikl.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "sk"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   /// <summary>
   /// Overrajdamo defaultno VvSifrarDataRecord ponasanje gdje je ovo true (za npr. artikl i osred)
   /// </summary>
   public override bool IsStringAutoSifra
   {
      get { return true; }
   }

   public override string SifraColName
   {
      get { return "artiklCD"; }
   }

   public override string SifraColValue
   {
      get { return this.ArtiklCD; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Artikl.sorterName; }
   }

   /// <summary>
   /// A je podatak 'artiklCD' kao link (foreign key) za druge tablice,
   /// izmijenjen u operaciji 'Edit'? 
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         // 28.03.2016: 
       //return  this.currentData._artiklCD != this.backupData._artiklCD                                                                ;

         // 25.01.2017:
         if(ZXC.IsRNMnotRNP) // Metaflex artikl.masaNetto ide u rtrans.t_ppmvOsn
         {
            return (this.currentData._artiklCD  != this.backupData._artiklCD || this.currentData._artiklName != this.backupData._artiklName || 
                    this.currentData._masaNetto != this.backupData._masaNetto);
         }
         else
         {
            return (this.currentData._artiklCD != this.backupData._artiklCD || this.currentData._artiklName != this.backupData._artiklName);
         }
      }
   }

   private List<Rtrans> _rtranses;
   /// <summary>
   /// Gets or sets a list of Rtrans (ala customers orders) for this Artikl.
   /// </summary>
   public List<Rtrans> Rtranses
   {
      get { return _rtranses; }
      set {        _rtranses = value; }
   }

   public override void InvokeTransClear()
   {
      if(this.Rtranses != null) this.Rtranses.Clear();
   }

   // 19.7.2011
 //public ArtStat TheAsEx { get; set; }
   private ArtStat theAsEx;
   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public ArtStat TheAsEx
   {
      get 
      {
         if(theAsEx == null) theAsEx = new ArtStat();
         return              theAsEx; 
      }
      set { theAsEx = value; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override VvDataRecord VirtualExtenderRecord
   {
      get
      {
         return this.TheAsEx;
      }
      set
      {
         this.TheAsEx = (ArtStat)value;
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

   public string BackupedArtiklCD
   {
      get { return this.backupData._artiklCD; }
   }

   //==============================================================

   /*05*/ public string ArtiklCD
   {
      get { return this.currentData._artiklCD; }
      set {        this.currentData._artiklCD = value; }
   }

   /*06*/ public string ArtiklName
   {
      get { return this.currentData._artiklName; }
      set {        this.currentData._artiklName = value; }
   }

   /*07*/ public string BarCode1
   {
      get { return this.currentData._barCode1; }
      set {        this.currentData._barCode1 = value; }
   }

   /*08*/ public string SkladCD
   {
      get { return this.currentData._skladCD; }
      set {        this.currentData._skladCD = value; }
   }

   /*09*/ public string Grupa1CD
   {
      get { return this.currentData._grupa1CD; }
      set {        this.currentData._grupa1CD = value; }
   }

   /*10*/ public string JedMj
   {
      get { return this.currentData._jedMj; }
      set {        this.currentData._jedMj = value; }
   }

   /*11*/ public string Konto
   {
      get { return this.currentData._konto; }
      set {        this.currentData._konto = value; }
   }

   /*12*/ public string ArtiklCD2
   {
      get { return this.currentData._artiklCD2; }
      set {        this.currentData._artiklCD2 = value; }
   }

   /*13*/ public string ArtiklName2
   {
      get { return this.currentData._artiklName2; }
      set {        this.currentData._artiklName2 = value; }
   }
   /* 14 */ public string TS  
   {
      get { return this.currentData._ts; }
      set {        this.currentData._ts = value; }
   }
   /* 15 */ public string BarCode2  
   {
      get { return this.currentData._barCode2; }
      set {        this.currentData._barCode2 = value; }
   }
   /* 16 */ public string SerNo     
   {
      get { return this.currentData._serNo; }
      set {        this.currentData._serNo = value; }
   }
   /* 17 */ public string Grupa2CD  
   {
      get { return this.currentData._grupa2CD; }
      set {        this.currentData._grupa2CD = value; }
   }
   /* 18 */ public string Grupa3CD  
   {
      get { return this.currentData._grupa3CD; }
      set {        this.currentData._grupa3CD = value; }
   }
   /* 19 */ public string Placement 
   {
      get { return this.currentData._placement; }
      set {        this.currentData._placement = value; }
   }
   /* 20 */ public string LinkArtCD 
   {
      get { return this.currentData._linkArtCD; }
      set {        this.currentData._linkArtCD = value; }
   }
   /* 21 */ public DateTime DateProizv
   {
      get { return this.currentData._dateProizv; }
      set {        this.currentData._dateProizv = value; }
   }
   /* 22 */ public string PdvKat    
   {
      get { return this.currentData._pdvKat; }
      set {        this.currentData._pdvKat = value; }
   }
   /* 23 */ public string LongOpis  
   {
      get { return this.currentData._longOpis; }
      set {        this.currentData._longOpis = value; }
   }
   /* 24 */ public string PrefValName   
   {
      get { return this.currentData._prefValName; }
      set {        this.currentData._prefValName = value; }
   }
   /* 25 */ public string OrgPak    
   {
      get { return this.currentData._orgPak; }
      set {        this.currentData._orgPak = value; }
   }
   /* 26 */ public bool IsRashod  
   {
      get { return this.currentData._isRashod; }
      set {        this.currentData._isRashod = value; }
   }
   /* 27 */ public bool IsAkcija  
   {
      get { return this.currentData._isAkcija; }
      set {        this.currentData._isAkcija = value; }
   }
   /* 28 */ public bool IsMaster
   {
      get { return this.currentData._isMaster; }
      set {        this.currentData._isMaster = value; }
   }
   /* 29 */ public decimal MasaNetto 
   {
      get { return this.currentData._masaNetto; }
      set {        this.currentData._masaNetto = value; }
   }
   /* 30 */ public decimal MasaBruto 
   {
      get { return this.currentData._masaBruto; }
      set {        this.currentData._masaBruto = value; }
   }
   /* 31 */ public decimal Promjer   
   {
      get { return this.currentData._promjer; }
      set {        this.currentData._promjer = value; }
   }
   /* 32 */ public decimal Povrsina  
   {
      get { return this.currentData._povrsina; }
      set {        this.currentData._povrsina = value; }
   }
   /* 33 */ public decimal Zapremina 
   {
      get { return this.currentData._zapremina; }
      set {        this.currentData._zapremina = value; }
   }
   /* 34 */ public decimal Duljina   
   {
      get { return this.currentData._duljina; }
      set {        this.currentData._duljina = value; }
   }
   /* 35 */ public decimal Sirina    
   {
      get { return this.currentData._sirina; }
      set {        this.currentData._sirina = value; }
   }
   /* 36 */ public decimal Visina    
   {
      get { return this.currentData._visina; }
      set {        this.currentData._visina = value; }
   }
   /* 37 */ public decimal Starost   
   {
      get { return this.currentData._starost; }
      set {        this.currentData._starost = value; }
   }
   /* 38 */ public string Boja      
   {
      get { return this.currentData._boja; }
      set {        this.currentData._boja = value; }
   }
   /* 39 */ public ushort Spol
   {
      get { return this.currentData._spol; }
      set {        this.currentData._spol = value; }
   }

   /* 40 */ public ushort Garancija
   {
      get { return this.currentData._garancija; }
      set {        this.currentData._garancija = value; }
   }

   /* 41 */ public uint DobavCD
   {
      get { return this.currentData._dobavCD; }
      set {        this.currentData._dobavCD = value; }
   }

   /* 42 */ public uint ProizCD
   {
      get { return this.currentData._proizCD; }
      set {        this.currentData._proizCD = value; }
   }

   /* 43 */ public bool IsAllowMinus
   {
      get { return this.currentData._isAllowMinus; }
      set {        this.currentData._isAllowMinus = value; }
   }

   /* 44 */ public bool IsSerNo
   {
      get { return this.currentData._isSerNo; }
      set {        this.currentData._isSerNo = value; }
   }

   /* 45 */ public string MasaNettoJM
   {
      get { return this.currentData._masaNettoJM; }
      set {        this.currentData._masaNettoJM = value; }
   }

   /* 46 */ public string MasaBrutoJM
   {
      get { return this.currentData._masaBrutoJM; }
      set {        this.currentData._masaBrutoJM = value; }
   }

   /* 47 */ public string PromjerJM
   {
      get { return this.currentData._promjerJM; }
      set {        this.currentData._promjerJM = value; }
   }

   /* 48 */ public string PovrsinaJM
   {
      get { return this.currentData._povrsinaJM; }
      set {        this.currentData._povrsinaJM = value; }
   }

   /* 49 */ public string ZapreminaJM
   {
      get { return this.currentData._zapreminaJM; }
      set {        this.currentData._zapreminaJM = value; }
   }

   /* 50 */ public string DuljinaJM
   {
      get { return this.currentData._duljinaJM; }
      set {        this.currentData._duljinaJM = value; }
   }

   /* 51 */ public string SirinaJM
   {
      get { return this.currentData._sirinaJM; }
      set {        this.currentData._sirinaJM = value; }
   }

   /* 52 */ public string VisinaJM
   {
      get { return this.currentData._visinaJM; }
      set {        this.currentData._visinaJM = value; }
   }

   /* 53 */ public string Velicina
   {
      get { return this.currentData._velicina; }
      set {        this.currentData._velicina = value; }
   }

   /* 54 */ public string MadeIn
   {
      get { return this.currentData._madeIn; }
      set {        this.currentData._madeIn = value; }
   }

   /* 55 */ public string Url
   {
      get { return this.currentData._url; }
      set {        this.currentData._url = value; }
   }

   /* 56 */ public string AtestBr
   {
      get { return this.currentData._atestBr; }
      set {        this.currentData._atestBr = value; }
   }

   /* 57 */ public DateTime AtestDate
   {
      get { return this.currentData._atestDate; }
      set {        this.currentData._atestDate = value; }
   }

   /* 58 */ public ZXC.ArtiklVpc1Policy Vpc1Policy
   {
      get { return (ZXC.ArtiklVpc1Policy)this.currentData._vpc1Policy; }
      set {                              this.currentData._vpc1Policy = (ushort)value; }
   }

   /* 59 */ public bool IsPrnOpis
   {
      get { return this.currentData._isPrnOpis; }
      set {        this.currentData._isPrnOpis = value; }
   }

   /* 60 */ public string CarTarifa
   {
      get { return this.currentData._carTarifa; }
      set {        this.currentData._carTarifa = value; }
   }

   /* 61 */ public string PartNo
   {
      get { return this.currentData._partNo; }
      set {        this.currentData._partNo = value; }
   }

   /* 62 */ public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 63 */ public decimal ImportCij
   {
      get { return this.currentData._importCij; }
      set {        this.currentData._importCij = value; }
   }

   /* 64 */ public decimal Snaga
   {
      get { return this.currentData._snaga; }
      set {        this.currentData._snaga = value; }
   }

   /* 65 */ public string SnagaJM
   {
      get { return this.currentData._snagaJM; }
      set {        this.currentData._snagaJM = value; }
   }

   /* 66 */ public ushort EmisCO2
   {
      get { return this.currentData._emisCO2; }
      set {        this.currentData._emisCO2 = value; }
   }

   /* 67 */ public ZXC.EuroNormaEnum EuroNorma
   {
      get { return (ZXC.EuroNormaEnum)this.currentData._euroNorma; }
      set {                           this.currentData._euroNorma = (ushort)value; }
   }
   /*    */ public ushort EuroNorma_u
   {
      get { return                 this.currentData._euroNorma;         }
      set {                        this.currentData._euroNorma = value; }
   }

   public string TsName     { get { return ZXC.luiListaArtiklTS     .GetNameForThisCd(this.TS      ); } }
   public string SkladName  { get { return ZXC.luiListaSkladista    .GetNameForThisCd(this.SkladCD ); } }
   public string PdvKatName { get { return ZXC.luiListaPdvKat       .GetNameForThisCd(this.PdvKat  ); } }
   public string Grupa1Name { get { return ZXC.luiListaGrupa1Artikla.GetNameForThisCd(this.Grupa1CD); } }
   public string Grupa2Name { get { return ZXC.luiListaGrupa2Artikla.GetNameForThisCd(this.Grupa2CD); } }
   public string Grupa3Name { get { return ZXC.luiListaGrupa3Artikla.GetNameForThisCd(this.Grupa3CD); } }

   public string Artikl_34_CD { get { return this.ArtiklCD.Length > 3 ? this.ArtiklCD.Substring(2, 2) : ""; } } // za PPUK - 'sortiment' 

   // 21.09.2016: 
 //public string R_orgPakJM { get; set; }
   public string R_orgPakJM 
   {
      get
      {
         // tko prvi, taj kwachy 

         if(this.MasaNetto.NotZero()) { return this.MasaNettoJM; }
         if(this.MasaBruto.NotZero()) { return this.MasaBrutoJM; }
         if(this.Povrsina .NotZero()) { return this.PovrsinaJM ; }
         if(this.Zapremina.NotZero()) { return this.ZapreminaJM; }
         if(this.Duljina  .NotZero()) { return this.DuljinaJM  ; }
         if(this.Sirina   .NotZero()) { return this.SirinaJM   ; }
         if(this.Visina   .NotZero()) { return this.VisinaJM   ; }

         //...else - default: 

         // 21.09.2016: 
       //return this.JedMj;
         return "";
      }
   }

   public decimal /*Calc*/R_orgPak
   {
      get
      {
         // 12.10.2016: usluga i sl.: vrati 0 
         if(this.IsMinusOK) return 0M;

         // tko prvi, taj kwachy 

         if(this.MasaNetto.NotZero()) { /*this.R_orgPakJM = this.MasaNettoJM;*/ return /* this.T_kol * */ this.MasaNetto; }
         if(this.MasaBruto.NotZero()) { /*this.R_orgPakJM = this.MasaBrutoJM;*/ return /* this.T_kol * */ this.MasaBruto; }
         if(this.Povrsina .NotZero()) { /*this.R_orgPakJM = this.PovrsinaJM ;*/ return /* this.T_kol * */ this.Povrsina ; }
         if(this.Zapremina.NotZero()) { /*this.R_orgPakJM = this.ZapreminaJM;*/ return /* this.T_kol * */ this.Zapremina; }
         if(this.Duljina  .NotZero()) { /*this.R_orgPakJM = this.DuljinaJM  ;*/ return /* this.T_kol * */ this.Duljina  ; }
         if(this.Sirina   .NotZero()) { /*this.R_orgPakJM = this.SirinaJM   ;*/ return /* this.T_kol * */ this.Sirina   ; }
         if(this.Visina   .NotZero()) { /*this.R_orgPakJM = this.VisinaJM   ;*/ return /* this.T_kol * */ this.Visina   ; }

         //...else - default: 
         
         /*this.R_orgPakJM = this.JedMj;*/

         return 1.00M;
      }

   }

   public decimal /*Calc*/R_BackupDataOrgPak
   {
      get
      {
         // tko prvi, taj kwachy 

         if(this.BackupData._masaNetto.NotZero()) { return /* this.T_kol * */ this.BackupData._masaNetto; }
         if(this.BackupData._masaBruto.NotZero()) { return /* this.T_kol * */ this.BackupData._masaBruto; }
         if(this.BackupData._povrsina .NotZero()) { return /* this.T_kol * */ this.BackupData._povrsina ; }
         if(this.BackupData._zapremina.NotZero()) { return /* this.T_kol * */ this.BackupData._zapremina; }
         if(this.BackupData._duljina  .NotZero()) { return /* this.T_kol * */ this.BackupData._duljina  ; }
         if(this.BackupData._sirina   .NotZero()) { return /* this.T_kol * */ this.BackupData._sirina   ; }
         if(this.BackupData._visina   .NotZero()) { return /* this.T_kol * */ this.BackupData._visina   ; }

         //...else - default: 

         //this.R_orgPakJM = this.JedMj;

         return 1.00M;
      }

   }


   #endregion General propertiez

   #region Artstat propertiz
   
   public uint AS_RecID
   {
      get { return this.TheAsEx.currentData._recID; }
      set {        this.TheAsEx.currentData._recID = value; }
   }

public uint     AS_RtransRecID        { get { return TheAsEx.RtransRecID             ; } set { this.TheAsEx.RtransRecID         = value; } }
public string   AS_ArtiklCD           { get { return TheAsEx.ArtiklCD                ; } set { this.TheAsEx.ArtiklCD            = value; } }
public string   AS_SkladCD            { get { return TheAsEx.SkladCD                 ; } set { this.TheAsEx.SkladCD             = value; } }
public DateTime AS_SkladDate          { get { return TheAsEx.SkladDate               ; } set { this.TheAsEx.SkladDate           = value; } }
public string   AS_TT                 { get { return TheAsEx.TT                      ; } set { this.TheAsEx.TT                  = value; } }
public Int16    AS_TtSort             { get { return TheAsEx.TtSort                  ; } set { this.TheAsEx.TtSort              = value; } }
public uint     AS_TtNum              { get { return TheAsEx.TtNum                   ; } set { this.TheAsEx.TtNum               = value; } }
public ushort   AS_Serial             { get { return TheAsEx.Serial                  ; } set { this.TheAsEx.Serial              = value; } }
public uint     AS_TransRbr           { get { return TheAsEx.TransRbr                ; } set { this.TheAsEx.TransRbr            = value; } }

public string   AS_ArtiklTS           { get { return this.TheAsEx.ArtiklTS           ; } set { this.TheAsEx.ArtiklTS            = value; } }
public string   AS_ArtiklJM           { get { return this.TheAsEx.ArtiklJM           ; } set { this.TheAsEx.ArtiklJM            = value; } }
public decimal  AS_UkPstFinKNJ        { get { return this.TheAsEx.UkPstFinKNJ        ; }                                                   }
public decimal  AS_UkUlazFinKNJ       { get { return this.TheAsEx.UkUlazFinKNJ       ; }                                                   }
public decimal  AS_UkUlazFinKNJAll    { get { return this.TheAsEx.UkUlazFinKNJAll    ; }                                                   }
public decimal  AS_UkIzlazFinKNJ      { get { return this.TheAsEx.UkIzlazFinKNJ      ; }                                                   }
public decimal  AS_UkUlazFirmaFinKNJ  { get { return this.TheAsEx.UkUlazFirmaFinKNJ  ; }                                                   }
public decimal  AS_UkIzlazFirmaFinKNJ { get { return this.TheAsEx.UkIzlazFirmaFinKNJ ; }                                                   }
public decimal  AS_StanjeFinKNJ       { get { return this.TheAsEx.StanjeFinKNJ       ; }                                                   }
public decimal  AS_KnjigCij           { get { return this.TheAsEx.KnjigCij           ; }                                                   }
public decimal  AS_LastKnjigCij       { get { return this.TheAsEx.LastKnjigCij       ; }                                                   }
public decimal  AS_UkPstFinNBC        { get { return this.TheAsEx.UkPstFinNBC        ; } set { this.TheAsEx.UkPstFinNBC         = value; } }
public decimal  AS_UkUlazFinNBC       { get { return this.TheAsEx.UkUlazFinNBC       ; } set { this.TheAsEx.UkUlazFinNBC        = value; } }
public decimal  AS_UkUlazFinNBCAll    { get { return this.TheAsEx.UkUlazFinNBCAll    ; }                                                   }
public decimal  AS_UkIzlazFinNBC      { get { return this.TheAsEx.UkIzlazFinNBC      ; } set { this.TheAsEx.UkIzlazFinNBC       = value; } }
public decimal  AS_UkUlazFirmaFinNBC  { get { return this.TheAsEx.UkUlazFirmaFinNBC  ; } set { this.TheAsEx.UkUlazFirmaFinNBC   = value; } }
public decimal  AS_UkIzlazFirmaFinNBC { get { return this.TheAsEx.UkIzlazFirmaFinNBC ; } set { this.TheAsEx.UkIzlazFirmaFinNBC  = value; } }
public decimal  AS_StanjeFinNBC       { get { return this.TheAsEx.StanjeFinNBC       ; }                                                   }
public decimal  AS_PrNabCij           { get { return this.TheAsEx.PrNabCij           ; }                                                   }
public decimal  AS_LastPrNabCij       { get { return this.TheAsEx.LastPrNabCij       ; } set { this.TheAsEx.LastPrNabCij        = value; } }
public decimal  AS_UkPstFinMPC        { get { return this.TheAsEx.UkPstFinMPC        ; } set { this.TheAsEx.UkPstFinMPC         = value; } }
public decimal  AS_UkUlazFinMPC       { get { return this.TheAsEx.UkUlazFinMPC       ; } set { this.TheAsEx.UkUlazFinMPC        = value; } }
public decimal  AS_UkUlazFinMPCAll    { get { return this.TheAsEx.UkUlazFinMPCAll    ; }                                                   }
public decimal  AS_UkIzlazFinMPC      { get { return this.TheAsEx.UkIzlazFinMPC      ; } set { this.TheAsEx.UkIzlazFinMPC       = value; } }
public decimal  AS_UkUlazFirmaFinMPC  { get { return this.TheAsEx.UkUlazFirmaFinMPC  ; } set { this.TheAsEx.UkUlazFirmaFinMPC   = value; } }
public decimal  AS_UkIzlazFirmaFinMPC { get { return this.TheAsEx.UkIzlazFirmaFinMPC ; } set { this.TheAsEx.UkIzlazFirmaFinMPC  = value; } }
public decimal  AS_StanjeFinMPC       { get { return this.TheAsEx.StanjeFinMPC       ; }                                                   }
public decimal  AS_MalopCij           { get { return this.TheAsEx.MalopCij           ; }                                                   }
public decimal  AS_PrevMalopCij       { get { return this.TheAsEx.PrevMalopCij       ; }                                                   }
public decimal  AS_LastUlazMPC        { get { return this.TheAsEx.LastUlazMPC        ; } set { this.TheAsEx.LastUlazMPC         = value; } }
public decimal  AS_UkIzlFinProdKCR    { get { return this.TheAsEx.UkIzlFinProdKCR    ; } set { this.TheAsEx.UkIzlFinProdKCR     = value; } }
public decimal  AS_StanjeKol          { get { return this.TheAsEx.StanjeKol          ; }                                                   }
public decimal  AS_UkUlazKolAll       { get { return this.TheAsEx.UkUlazKolAll       ; }                                                   }
public decimal  AS_UkPstKol           { get { return this.TheAsEx.UkPstKol           ; } set { this.TheAsEx.UkPstKol            = value; } }
public string   AS_OrgPakJM           { get { return this.TheAsEx.OrgPakJM           ; } set { this.TheAsEx.OrgPakJM            = value; } }
public decimal  AS_OrgPak             { get { return this.TheAsEx.OrgPak             ; } set { this.TheAsEx.OrgPak              = value; } }
public decimal  AS_UkUlazKol          { get { return this.TheAsEx.UkUlazKol          ; } set { this.TheAsEx.UkUlazKol           = value; } }
public decimal  AS_UkIzlazKol         { get { return this.TheAsEx.UkIzlazKol         ; } set { this.TheAsEx.UkIzlazKol          = value; } }
public decimal  AS_UkUlazFirmaKol     { get { return this.TheAsEx.UkUlazFirmaKol     ; } set { this.TheAsEx.UkUlazFirmaKol      = value; } }
public decimal  AS_UkIzlazFirmaKol    { get { return this.TheAsEx.UkIzlazFirmaKol    ; } set { this.TheAsEx.UkIzlazFirmaKol     = value; } }
public decimal  AS_StanjeKol2         { get { return this.TheAsEx.StanjeKol2         ; }                                                   }
public decimal  AS_UkUlazKol2All      { get { return this.TheAsEx.UkUlazKol2All      ; }                                                   }
public decimal  AS_UkPstKol2          { get { return this.TheAsEx.UkPstKol2          ; } set { this.TheAsEx.UkPstKol2           = value; } }
public decimal  AS_UkUlazKol2         { get { return this.TheAsEx.UkUlazKol2         ; } set { this.TheAsEx.UkUlazKol2          = value; } }
public decimal  AS_UkIzlazKol2        { get { return this.TheAsEx.UkIzlazKol2        ; } set { this.TheAsEx.UkIzlazKol2         = value; } }
public decimal  AS_UkUlazFirmaKol2    { get { return this.TheAsEx.UkUlazFirmaKol2    ; } set { this.TheAsEx.UkUlazFirmaKol2     = value; } }
public decimal  AS_UkIzlazFirmaKol2   { get { return this.TheAsEx.UkIzlazFirmaKol2   ; } set { this.TheAsEx.UkIzlazFirmaKol2    = value; } }
   

public decimal  AS_StanjKolFisycal    { get { return this.TheAsEx.StanjKolFisycal    ; }                                                   }
public decimal  AS_UkUlazKolAllFisycal{ get { return this.TheAsEx.UkUlazKolAllFisycal; }                                                   }
public decimal  AS_UkUlazKolFisycal   { get { return this.TheAsEx.UkUlazKolFisycal   ; } set { this.TheAsEx.UkUlazKolFisycal    = value; } }
public decimal  AS_UkIzlazKolFisycal  { get { return this.TheAsEx.UkIzlazKolFisycal  ; } set { this.TheAsEx.UkIzlazKolFisycal   = value; } }
public decimal  AS_StanjeKolFree      { get { return this.TheAsEx.StanjeKolFree      ; }                                                   }
public decimal  AS_UkRezervKolNaruc   { get { return this.TheAsEx.UkRezervKolNaruc   ; } set { this.TheAsEx.UkRezervKolNaruc    = value; } }
public decimal  AS_UkRezervKolIsporu  { get { return this.TheAsEx.UkRezervKolIsporu  ; } set { this.TheAsEx.UkRezervKolIsporu   = value; } }
public decimal  AS_UkStanjeKolRezerv  { get { return this.TheAsEx.UkStanjeKolRezerv  ; } set { this.TheAsEx.UkStanjeKolRezerv   = value; } }
public decimal  AS_InvKol             { get { return this.TheAsEx.InvKol             ; } set { this.TheAsEx.InvKol              = value; } }
public decimal  AS_InvKol2            { get { return this.TheAsEx.InvKol2            ; } set { this.TheAsEx.InvKol2             = value; } }
public decimal  AS_InvFinNBC          { get { return this.TheAsEx.InvFinNBC          ; } set { this.TheAsEx.InvFinNBC           = value; } }
public decimal  AS_InvFinMPC          { get { return this.TheAsEx.InvFinMPC          ; } set { this.TheAsEx.InvFinMPC           = value; } }
public decimal  AS_InvFinKNJ          { get { return this.TheAsEx.InvFinKNJ          ; }                                                   }
public decimal  AS_UlazCijMin         { get { return this.TheAsEx.UlazCijMin         ; } set { this.TheAsEx.UlazCijMin          = value; } }
public decimal  AS_UlazCijMax         { get { return this.TheAsEx.UlazCijMax         ; } set { this.TheAsEx.UlazCijMax          = value; } }
public decimal  AS_UlazCijLast        { get { return this.TheAsEx.UlazCijLast        ; } set { this.TheAsEx.UlazCijLast         = value; } }
public decimal  AS_IzlazCijMin        { get { return this.TheAsEx.IzlazCijMin        ; } set { this.TheAsEx.IzlazCijMin         = value; } }
public decimal  AS_IzlazCijMax        { get { return this.TheAsEx.IzlazCijMax        ; } set { this.TheAsEx.IzlazCijMax         = value; } }
public decimal  AS_IzlazCijLast       { get { return this.TheAsEx.IzlazCijLast       ; } set { this.TheAsEx.IzlazCijLast        = value; } }
public decimal  AS_PreDefVpc1         { get { return this.TheAsEx.PreDefVpc1         ; } set { this.TheAsEx.PreDefVpc1          = value; } }
public decimal  AS_PreDefVpc2         { get { return this.TheAsEx.PreDefVpc2         ; } set { this.TheAsEx.PreDefVpc2          = value; } }
public decimal  AS_PreDefMpc1         { get { return this.TheAsEx.PreDefMpc1         ; } set { this.TheAsEx.PreDefMpc1          = value; } }
public decimal  AS_PreDefDevc         { get { return this.TheAsEx.PreDefDevc         ; } set { this.TheAsEx.PreDefDevc          = value; } }
public decimal  AS_PreDefRbt1         { get { return this.TheAsEx.PreDefRbt1         ; } set { this.TheAsEx.PreDefRbt1          = value; } }
public decimal  AS_PreDefRbt2         { get { return this.TheAsEx.PreDefRbt2         ; } set { this.TheAsEx.PreDefRbt2          = value; } }
public decimal  AS_PreDefMinKol       { get { return this.TheAsEx.PreDefMinKol       ; } set { this.TheAsEx.PreDefMinKol        = value; } }
public decimal  AS_PreDefMarza        { get { return this.TheAsEx.PreDefMarza        ; } set { this.TheAsEx.PreDefMarza         = value; } }
public string   AS_FrsMinTt           { get { return this.TheAsEx.FrsMinTt           ; } set { this.TheAsEx.FrsMinTt            = value; } }
public uint     AS_FrsMinTtNum        { get { return this.TheAsEx.FrsMinTtNum        ; } set { this.TheAsEx.FrsMinTtNum         = value; } }
public DateTime AS_DateZadUlaz        { get { return this.TheAsEx.DateZadUlaz        ; } set { this.TheAsEx.DateZadUlaz         = value; } }
public DateTime AS_DateZadIzlaz       { get { return this.TheAsEx.DateZadIzlaz       ; } set { this.TheAsEx.DateZadIzlaz        = value; } }
public DateTime AS_DateZadPst         { get { return this.TheAsEx.DateZadPst         ; } set { this.TheAsEx.DateZadPst          = value; } }
public DateTime AS_DateZadInv         { get { return this.TheAsEx.DateZadInv         ; } set { this.TheAsEx.DateZadInv          = value; } }
public decimal  AS_RtrPstKol          { get { return this.TheAsEx.RtrPstKol          ; } set { this.TheAsEx.RtrPstKol           = value; } } 
public decimal  AS_RtrUlazKol         { get { return this.TheAsEx.RtrUlazKol         ; } set { this.TheAsEx.RtrUlazKol          = value; } } 
public decimal  AS_RtrUlazKolFisycal  { get { return this.TheAsEx.RtrUlazKolFisycal  ; } set { this.TheAsEx.RtrUlazKolFisycal   = value; } } 
public decimal  AS_RtrKolNaruceno     { get { return this.TheAsEx.RtrKolNaruceno     ; } set { this.TheAsEx.RtrKolNaruceno      = value; } } 
public decimal  AS_RtrKolIsporuceno   { get { return this.TheAsEx.RtrKolIsporuceno   ; } set { this.TheAsEx.RtrKolIsporuceno    = value; } } 
public decimal  AS_RtrIzlazKolFisycal { get { return this.TheAsEx.RtrIzlazKolFisycal ; } set { this.TheAsEx.RtrIzlazKolFisycal  = value; } } 
public decimal  AS_RtrUlazAllKol      { get { return this.TheAsEx.RtrUlazAllKol      ; }                                                   } 
public decimal  AS_RtrIzlazKol        { get { return this.TheAsEx.RtrIzlazKol        ; } set { this.TheAsEx.RtrIzlazKol         = value; } } 
public decimal  AS_RtrPstKol2         { get { return this.TheAsEx.RtrPstKol2         ; } set { this.TheAsEx.RtrPstKol2          = value; } } 
public decimal  AS_RtrUlazKol2        { get { return this.TheAsEx.RtrUlazKol2        ; } set { this.TheAsEx.RtrUlazKol2         = value; } } 
public decimal  AS_RtrUlazAllKol2     { get { return this.TheAsEx.RtrUlazAllKol2     ; }                                                   } 
public decimal  AS_RtrIzlazKol2       { get { return this.TheAsEx.RtrIzlazKol2       ; } set { this.TheAsEx.RtrIzlazKol2        = value; } } 
public decimal  AS_RtrPstVrjNBC       { get { return this.TheAsEx.RtrPstVrjNBC       ; } set { this.TheAsEx.RtrPstVrjNBC        = value; } } 
public decimal  AS_RtrUlazVrjNBC      { get { return this.TheAsEx.RtrUlazVrjNBC      ; } set { this.TheAsEx.RtrUlazVrjNBC       = value; } } 
public decimal  AS_RtrUlazAllVrjNBC   { get { return this.TheAsEx.RtrUlazAllVrjNBC   ; }                                                   } 
public decimal  AS_RtrIzlazVrjNBC     { get { return this.TheAsEx.RtrIzlazVrjNBC     ; } set { this.TheAsEx.RtrIzlazVrjNBC      = value; } } 
public decimal  AS_RtrPstCijNBC       { get { return this.TheAsEx.RtrPstCijNBC       ; } set { this.TheAsEx.RtrPstCijNBC        = value; } } 
public decimal  AS_RtrUlazCijNBC      { get { return this.TheAsEx.RtrUlazCijNBC      ; } set { this.TheAsEx.RtrUlazCijNBC       = value; } } 
public decimal  AS_RtrIzlazCijNBC     { get { return this.TheAsEx.RtrIzlazCijNBC     ; } set { this.TheAsEx.RtrIzlazCijNBC      = value; } } 
public decimal  AS_RtrCijenaNBC       { get { return this.TheAsEx.RtrCijenaNBC       ; } set { this.TheAsEx.RtrCijenaNBC        = value; } } 
public decimal  AS_RtrPstVrjMPC       { get { return this.TheAsEx.RtrPstVrjMPC       ; } set { this.TheAsEx.RtrPstVrjMPC        = value; } } 
public decimal  AS_RtrUlazVrjMPC      { get { return this.TheAsEx.RtrUlazVrjMPC      ; } set { this.TheAsEx.RtrUlazVrjMPC       = value; } } 
public decimal  AS_RtrUlazAllVrjMPC   { get { return this.TheAsEx.RtrUlazAllVrjMPC   ; }                                                   } 
public decimal  AS_RtrIzlazVrjMPC     { get { return this.TheAsEx.RtrIzlazVrjMPC     ; } set { this.TheAsEx.RtrIzlazVrjMPC      = value; } } 
public decimal  AS_RtrPstCijMPC       { get { return this.TheAsEx.RtrPstCijMPC       ; } set { this.TheAsEx.RtrPstCijMPC        = value; } } 
public decimal  AS_RtrUlazCijMPC      { get { return this.TheAsEx.RtrUlazCijMPC      ; } set { this.TheAsEx.RtrUlazCijMPC       = value; } } 
public decimal  AS_RtrIzlazCijMPC     { get { return this.TheAsEx.RtrIzlazCijMPC     ; } set { this.TheAsEx.RtrIzlazCijMPC      = value; } } 
public decimal  AS_RtrCijenaMPC       { get { return this.TheAsEx.RtrCijenaMPC       ; } set { this.TheAsEx.RtrCijenaMPC        = value; } } 
public decimal  AS_RtrPstVrjKNJ       { get { return this.TheAsEx.RtrPstVrjKNJ       ; }                                                   } 
public decimal  AS_RtrUlazVrjKNJ      { get { return this.TheAsEx.RtrUlazVrjKNJ      ; }                                                   } 
public decimal  AS_RtrUlazAllVrjKNJ   { get { return this.TheAsEx.RtrUlazAllVrjKNJ   ; }                                                   } 
public decimal  AS_RtrIzlazVrjKNJ     { get { return this.TheAsEx.RtrIzlazVrjKNJ     ; }                                                   } 
public decimal  AS_RtrPstCijKNJ       { get { return this.TheAsEx.RtrPstCijKNJ       ; }                                                   } 
public decimal  AS_RtrUlazCijKNJ      { get { return this.TheAsEx.RtrUlazCijKNJ      ; }                                                   } 
public decimal  AS_RtrIzlazCijKNJ     { get { return this.TheAsEx.RtrIzlazCijKNJ     ; }                                                   } 
public decimal  AS_RtrCijenaKNJ       { get { return this.TheAsEx.RtrCijenaKNJ       ; }                                                   } 

public decimal  AS_PstCijProsKNJ      { get { return this.TheAsEx.PstCijProsKNJ      ; }                                                   } 
public decimal  AS_UlazCijProsKNJ     { get { return this.TheAsEx.UlazCijProsKNJ     ; }                                                   } 
public decimal  AS_IzlCijProsKNJ      { get { return this.TheAsEx.IzlCijProsKNJ      ; }                                                   } 
public decimal  AS_PstCijProsMPC      { get { return this.TheAsEx.PstCijProsMPC      ; }                                                   } 
public decimal  AS_UlazCijProsMPC     { get { return this.TheAsEx.UlazCijProsMPC     ; }                                                   } 
public decimal  AS_IzlCijProsMPC      { get { return this.TheAsEx.IzlCijProsMPC      ; }                                                   } 
public decimal  AS_PstCijProsNBC      { get { return this.TheAsEx.PstCijProsNBC      ; }                                                   } 
public decimal  AS_UlazCijProsNBC     { get { return this.TheAsEx.UlazCijProsNBC     ; }                                                   } 
public decimal  AS_IzlCijProsNBC      { get { return this.TheAsEx.IzlCijProsNBC      ; }                                                   } 
public decimal  AS_IzlProdCijPros     { get { return this.TheAsEx.IzlProdCijPros     ; }                                                   } 
public decimal  AS_IzlazRUVIznos      { get { return this.TheAsEx.IzlazRUVIznos      ; }                                                   } 
public decimal  AS_IzlazRUVKoef       { get { return this.TheAsEx.IzlazRUVKoef       ; }                                                   } 
public decimal  AS_IzlazRUVStopa      { get { return this.TheAsEx.IzlazRUVStopa      ; }                                                   }
public decimal  AS_RucVpc1Iznos       { get { return this.TheAsEx.RucVpc1Iznos       ; }                                                   } 
public decimal  AS_RucVpc1Koef        { get { return this.TheAsEx.RucVpc1Koef        ; }                                                   } 
public decimal  AS_RucVpc1Stopa       { get { return this.TheAsEx.RucVpc1Stopa       ; }                                                   } 
public decimal  AS_PrNabCijPlusMarza  { get { return this.TheAsEx.PrNabCijPlusMarza  ; }                                                   } 
public decimal  AS_PrevKolStanje      { get { return this.TheAsEx.PrevKolStanje      ; }                                                   } 
public decimal  AS_PrevKolStanje2     { get { return this.TheAsEx.PrevKolStanje2     ; }                                                   } 
public decimal  AS_DiffMalopCij       { get { return this.TheAsEx.DiffMalopCij       ; }                                                   } 
public decimal  AS_NivelacUlazVrj     { get { return this.TheAsEx.NivelacUlazVrj     ; }                                                   } 
public decimal  AS_NivelacIzlazVrj    { get { return this.TheAsEx.NivelacIzlazVrj    ; }                                                   } 
public bool     AS_IsMinusOK          { get { return this.TheAsEx.IsMinusOK          ; }                                                   } 
public bool     AS_IsMinusNotOK       { get { return this.TheAsEx.IsMinusNotOK       ; }                                                   } 
public bool     AS_IsRuc4Usluga       { get { return this.TheAsEx.IsRuc4Usluga       ; }                                                   } 
public bool     AS_IsKonto4Usluga     { get { return this.TheAsEx.IsKonto4Usluga     ; }                                                   } 
public bool     AS_IsKonto4UslugaDP   { get { return this.TheAsEx.IsKonto4UslugaDP   ; }                                                   } 
public bool     AS_IsMaterOrPotros    { get { return this.TheAsEx.IsMaterOrPotros    ; }                                                   } 
public bool     AS_IsMaterijal        { get { return this.TheAsEx.IsMaterijal        ; }                                                   } 
public bool     AS_IsSitniInv         { get { return this.TheAsEx.IsSitniInv         ; }                                                   } 
public bool     AS_IsAllSkladCD       { get { return this.TheAsEx.IsAllSkladCD       ; }                                                   } 
public DateTime AS_DateZadPromj       { get { return this.TheAsEx.DateZadPromj       ; }                                                   }
public decimal  AS_InvDiff            { get { return this.TheAsEx.InvDiff            ; }                                                   }
public decimal  AS_InvDiff2           { get { return this.TheAsEx.InvDiff2           ; }                                                   }

public decimal AS_InvKolDiff          { get { return this.TheAsEx.InvKolDiff         ; } set { this.TheAsEx.InvKolDiff    = value;       } } // ovi su izuzetak jer imaju potrebu za SET-om kod CheckInvProblems
public decimal AS_InvKol2Diff         { get { return this.TheAsEx.InvKol2Diff        ; } set { this.TheAsEx.InvKol2Diff   = value;       } } // ovi su izuzetak jer imaju potrebu za SET-om kod CheckInvProblems
public decimal AS_InvFinDiffNBC       { get { return this.TheAsEx.InvFinDiffNBC      ; } set { this.TheAsEx.InvFinDiffNBC = value;       } } // ovi su izuzetak jer imaju potrebu za SET-om kod CheckInvProblems
public decimal AS_InvFinDiffMPC       { get { return this.TheAsEx.InvFinDiffMPC      ; } set { this.TheAsEx.InvFinDiffMPC = value;       } } // ovi su izuzetak jer imaju potrebu za SET-om kod CheckInvProblems
public decimal AS_InvFinDiffKNJ       { get { return this.TheAsEx.InvFinDiffKNJ      ; }                                                   }

public decimal AS_InvKol_Visak_AFT    { get { return this.TheAsEx.InvKol_Visak_AFT   ; }                                                   }
public decimal AS_InvKol_Manjk_AFT    { get { return this.TheAsEx.InvKol_Manjk_AFT   ; }                                                   }
public decimal AS_InvFinNBC_Visak_AFT { get { return this.TheAsEx.InvFinNBC_Visak_AFT; }                                                   }
public decimal AS_InvFinNBC_Manjk_AFT { get { return this.TheAsEx.InvFinNBC_Manjk_AFT; }                                                   }
public decimal AS_InvFinMPC_Visak_AFT { get { return this.TheAsEx.InvFinMPC_Visak_AFT; }                                                   }
public decimal AS_InvFinMPC_Manjk_AFT { get { return this.TheAsEx.InvFinMPC_Manjk_AFT; }                                                   }
public decimal AS_InvFinKNJ_Visak_AFT { get { return this.TheAsEx.InvFinKNJ_Visak_AFT; }                                                   }
public decimal AS_InvFinKNJ_Manjk_AFT { get { return this.TheAsEx.InvFinKNJ_Manjk_AFT; }                                                   }
public bool    AS_IsShadowInventura   { get { return this.TheAsEx.IsShadowInventura;   } set { this.TheAsEx.IsShadowInventura   = value; } }
public decimal AS_InvKol_Visak_BEF    { get { return this.TheAsEx.InvKol_Visak_BEF   ; }                                                   }
public decimal AS_InvKol_Manjk_BEF    { get { return this.TheAsEx.InvKol_Manjk_BEF   ; }                                                   }
public decimal AS_InvFinNBC_Visak_BEF { get { return this.TheAsEx.InvFinNBC_Visak_BEF; }                                                   }
public decimal AS_InvFinNBC_Manjk_BEF { get { return this.TheAsEx.InvFinNBC_Manjk_BEF; }                                                   }
public decimal AS_InvFinMPC_Visak_BEF { get { return this.TheAsEx.InvFinMPC_Visak_BEF; }                                                   }
public decimal AS_InvFinMPC_Manjk_BEF { get { return this.TheAsEx.InvFinMPC_Manjk_BEF; }                                                   }
public decimal AS_InvFinKNJ_Visak_BEF { get { return this.TheAsEx.InvFinKNJ_Visak_BEF; }                                                   }
public decimal AS_InvFinKNJ_Manjk_BEF { get { return this.TheAsEx.InvFinKNJ_Manjk_BEF; }                                                   }
public decimal AS_StanjeKol_INV       { get { return this.TheAsEx.StanjeKol_INV      ; }                                                   }
public decimal AS_StanjeFinNBC_INV    { get { return this.TheAsEx.StanjeFinNBC_INV   ; }                                                   }
public decimal AS_StanjeFinMPC_INV    { get { return this.TheAsEx.StanjeFinMPC_INV   ; }                                                   }
public decimal AS_StanjeFinKNJ_INV    { get { return this.TheAsEx.StanjeFinKNJ_INV   ; }                                                   }

public decimal  AS_KnjigCijOP        { get { return this.TheAsEx.KnjigCijOP          ; }                                                   } 
public decimal  AS_LastKnjigCijOP    { get { return this.TheAsEx.LastKnjigCijOP      ; }                                                   } 
public decimal  AS_PrNabCijOP        { get { return this.TheAsEx.PrNabCijOP          ; }                                                   } 
public decimal  AS_LastPrNabCijOP    { get { return this.TheAsEx.LastPrNabCijOP      ; }                                                   } 
public decimal  AS_MalopCijOP        { get { return this.TheAsEx.MalopCijOP          ; }                                                   } 
public decimal  AS_LastUlazMPCOP     { get { return this.TheAsEx.LastUlazMPCOP       ; }                                                   } 
public decimal  AS_PrevMalopCijOP    { get { return this.TheAsEx.PrevMalopCijOP      ; }                                                   } 
public decimal  AS_UlazCijMinOP      { get { return this.TheAsEx.UlazCijMinOP        ; }                                                   } 
public decimal  AS_UlazCijMaxOP      { get { return this.TheAsEx.UlazCijMaxOP        ; }                                                   } 
public decimal  AS_UlazCijLastOP     { get { return this.TheAsEx.UlazCijLastOP       ; }                                                   } 
public decimal  AS_IzlazCijMinOP     { get { return this.TheAsEx.IzlazCijMinOP       ; }                                                   } 
public decimal  AS_IzlazCijMaxOP     { get { return this.TheAsEx.IzlazCijMaxOP       ; }                                                   } 
public decimal  AS_IzlazCijLastOP    { get { return this.TheAsEx.IzlazCijLastOP      ; }                                                   }
public decimal  AS_RtrUlazCijKNJOP   { get { return this.TheAsEx.RtrUlazCijKNJOP     ; }                                                   } 
public decimal  AS_RtrIzlazCijKNJOP  { get { return this.TheAsEx.RtrIzlazCijKNJOP    ; }                                                   } 
public decimal  AS_RtrCijenaKNJOP    { get { return this.TheAsEx.RtrCijenaKNJOP      ; }                                                   } 
public decimal  AS_PstCijProsKNJOP   { get { return this.TheAsEx.PstCijProsKNJOP     ; }                                                   } 
public decimal  AS_UlazCijProsKNJOP  { get { return this.TheAsEx.UlazCijProsKNJOP    ; }                                                   } 
public decimal  AS_IzlCijProsKNJOP   { get { return this.TheAsEx.IzlCijProsKNJOP     ; }                                                   } 
public decimal  AS_IzlProdCijProsOP  { get { return this.TheAsEx.IzlProdCijProsOP    ; }                                                   } 
public decimal  AS_RucVpc1IznosOP    { get { return this.TheAsEx.RucVpc1IznosOP      ; }                                                   } 
public decimal  AS_StanjeKolOP       { get { return this.TheAsEx.StanjeKolOP         ; }                                                   } 

public decimal  AS_UkUlazKolAllOP    { get { return this.TheAsEx.UkUlazKolAllOP      ; }                                                   } 
public decimal  AS_UkPstKolOP        { get { return this.TheAsEx.UkPstKolOP          ; }                                                   } 
public decimal  AS_UkUlazKolOP       { get { return this.TheAsEx.UkUlazKolOP         ; }                                                   } 
public decimal  AS_UkIzlazKolOP      { get { return this.TheAsEx.UkIzlazKolOP        ; }                                                   } 
                                                                                     
public string   AS_ArtGrCd1          { get { return this.TheAsEx.ArtGrCd1            ; } set { this.TheAsEx.ArtGrCd1            = value; } }
public string   AS_ArtGrCd2          { get { return this.TheAsEx.ArtGrCd2            ; } set { this.TheAsEx.ArtGrCd2            = value; } }
public string   AS_ArtGrCd3          { get { return this.TheAsEx.ArtGrCd3            ; } set { this.TheAsEx.ArtGrCd3            = value; } }
                                                                                     
public decimal  AS_RtrPdvSt          { get { return this.TheAsEx.RtrPdvSt            ; } set { this.TheAsEx.RtrPdvSt            = value; } }
public bool     AS_RtrIsIrmUslug     { get { return this.TheAsEx.RtrIsIrmUslug       ; } set { this.TheAsEx.RtrIsIrmUslug       = value; } }
public uint     AS_RtrParentID       { get { return this.TheAsEx.RtrParentID         ; } set { this.TheAsEx.RtrParentID         = value; } }

// 15.12.2017:
public decimal AS_Rtr_Ratio_Kol_Kol2         { get { return ZXC.DivSafe(TheAsEx.RtrPstKol + TheAsEx.RtrUlazKol + TheAsEx.RtrIzlazKol, TheAsEx.RtrPstKol2 + TheAsEx.RtrUlazKol2 + TheAsEx.RtrIzlazKol2); } }
public decimal AS_Rtr_UlazAll_Ratio_Kol_Kol2 { get { return ZXC.DivSafe(TheAsEx.RtrPstKol + TheAsEx.RtrUlazKol                      , TheAsEx.RtrPstKol2 + TheAsEx.RtrUlazKol2                       ); } }
public decimal AS_Rtr_Izlaz_Ratio_Kol_Kol2   { get { return ZXC.DivSafe(TheAsEx.RtrIzlazKol                                         , TheAsEx.RtrIzlazKol2                                           ); } }
public decimal AS_UlazAll_Ratio_Kol_Kol2     { get { return ZXC.DivSafe(TheAsEx.UkUlazKolAll                                        , TheAsEx.UkUlazKol2All                                          ); } }
public decimal AS_Izlaz_Ratio_Kol_Kol2       { get { return ZXC.DivSafe(TheAsEx.UkIzlazKol                                          , TheAsEx.UkIzlazKol2                                            ); } }

public decimal AS_WeeklyIzlazKol             { get { return this.TheAsEx.WeeklyIzlazKol ; }                                                   } 
public decimal AS_WeeklyDeviation            { get { return this.TheAsEx.WeeklyDeviation; }                                                   } 

public decimal  AS_UkUlazKolBOP              { get { return this.TheAsEx.UkUlazKolBOP         ; }                                             } 
public decimal  AS_UkIzlazKolBOP             { get { return this.TheAsEx.UkIzlazKolBOP        ; }                                             } 
public decimal  AS_PrNabCijCOP               { get { return this.TheAsEx.PrNabCijCOP          ; }                                             } 

public decimal  AS_HalmedORG                 { get { return this.TheAsEx.HalmedORG            ; } set { this.TheAsEx.HalmedORG = value;     } }
public decimal  AS_HalmedCIJ                 { get { return this.TheAsEx.HalmedCIJ            ; }                                             } 
public decimal  AS_HalmedCOP                 { get { return this.TheAsEx.HalmedCOP            ; }                                             } 
public decimal  AS_HalmedBOP                 { get { return this.TheAsEx.HalmedBOP            ; }                                             }

   // 09.01.2023: 
//public bool     AS_HasUselessPST { get { return this.TheAsEx.DateZadPst.NotEmpty() && this.TheAsEx.UkPstKol.IsZero() && this.TheAsEx.UkUlazKol.IsZero() && this.TheAsEx.UkIzlazKol.IsZero()                                                                          ; } }
  public bool     AS_HasUselessPST { get { return this.TheAsEx.DateZadPst.NotEmpty() && this.TheAsEx.UkPstKol.IsZero() && this.TheAsEx.UkUlazKol.IsZero() && this.TheAsEx.UkIzlazKol.IsZero() && (this.TheAsEx.TT == Faktur.TT_PST || this.TheAsEx.TT == Faktur.TT_INV); } }
  public decimal AS_PrNBCBefThisUlaz { get { return this.TheAsEx.PrNBCBefThisUlaz; } set { this.TheAsEx.PrNBCBefThisUlaz = value; } }

   #endregion Artstat propertiz

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<ArtiklStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<ArtiklStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<ArtiklStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<ArtiklStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<ArtiklStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Artikl newObject = new Artikl();

      Generic_CloneData<ArtiklStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // TODO: IsExtendable4Read je virtualan i defaultno je false. Vidi hoce li ti ovdje trebati as true te u kojim slucajevima (kod rtransa je ako je TtInfo.IsIRArucableTT) 
      if(this.IsExtendable4Read) newObject.TheAsEx = this.TheAsEx.MakeDeepCopy();

      return newObject;

   }

   public Artikl MakeDeepCopy()
   {
      return (Artikl)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Artikl();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Artikl)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Artikl)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Artikl>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Artikl>(fileName);
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

   #region Some Util - Results propertiz

   // 22.10.2012: 
   //public static bool IsMinusOk(string theTS)
   //{
   //   return (theTS == "USL" || theTS == "PRS" || theTS == "TAK" );
   //}

   // even newer: 12.11.2012: 
   //public static bool IsMinusOk(string theTS)
   //{
   //   if(theTS == "USL" || theTS == "PRS" || theTS == "TAK") return true;

   //   VvLookUpItem lui = ZXC.luiListaArtiklTS.SingleOrDefault(li => li.Cd == theTS);

   //   if(lui == null) return false;

   //   return lui.Flag;
   //}
   public static bool IsMinusOk(string theTS)
   {
      return ZXC.NoNeedForZaliha_ArtiklTSs.Contains(theTS);
   }

   public static bool IsUslugaDP(string theTS)
   {
      // 10.05.2017: Tembo UDG additions 
    //return (theTS == "UDP"                  );
      return (theTS == "UDP" || theTS == "UDG");
   }
   public static bool IsMaterOrPotros(string theTS)
   {
      return (theTS == "MAT" || theTS == "POT" || theTS == "KMT");
   }
   public static bool IsMaterijal(string theTS)
   {
      return (theTS == "MAT" /*|| theTS == "POT" || theTS == "KMT"*/);
   }
   public static bool IsSitni(string theTS)
   {
      return (theTS == "SIT");
   }
   public static bool IsUslNoRuc(string theTS)
   {
      return (theTS == "UNR");
   }
   public bool IsMinusOK      { get { return IsMinusOk(this.TS); } } // Usluga, Prolazna stavka, Taksa 
   public bool IsMinusNotOK   { get { return !IsMinusOK        ; } } // Roba, Materijal, Vlastiti Proizvod, Uzorak, Ambalaza, ... 
 //public bool IsRuc4Usluga   { get { return IsMinusOK         ; } } // Dakle izjednacavas a ubuduce mozda treba dodatne razluciti 
   
   public bool IsMinusOK_or_UDP_Artikl { get { return /*IsRuc4Usluga*/this.IsMinusOK || IsUslugaDP(this.TS); } } // za Faktur2Nalog 

   public bool IsFKZ { get { return this.TS == FinKorekArtTS; } } // financijska korekcija financijskog stanja skladista (zaliha) 

   // TODO kako jos kada dobijes nekoga sa autima? 
   public bool IsPPMV { get { return this.Grupa1CD == ZXC.MotVoziloGrCD; } } // je li ovo motocikl, auto, ... motorno vozilo na koje ide Posebni Porez na Motorna Vozila 

   public bool IsUMJETNINA { get { return this.Grupa1CD == ZXC.UmjetninaGrCD; } } // je li ovo umjetnina,                     ... na koje PDV ide samo na RUC 
   public bool IsRabMotVoz { get { return this.Grupa1CD == ZXC.RabMotVozGrCD; } } // je li ovo               rabljeno vozilo, ... na koje PDV ide samo na RUC 
   public bool IsPDVonRUC  { get { return IsUMJETNINA || IsRabMotVoz        ; } } // je li ovo umjetnina ili rabljeno vozilo, ... na koje PDV ide samo na RUC 


   public bool IsKOMISROBA { get { return this.Grupa1CD == Artikl.KomisRobaGrCD; } } // je li ovo artikl kojega prodajemo iz komisije 

   public bool IsPNP   { get { return (this.TS == "RPP" || this.TS == "ČPP"); } } // pića, porez na potrošnju 
   public bool IsGlass { get { return (this.TS == "ČPP" || this.TS == "ČNP"); } } // čaše - proizvodnja putem vezanog artikla ili normativa - sastavnice 

   public static string CreateUmjetninaNaziv(string _rawNaziv, string _artiklCD, string _proizNaziv)
   {
      return _rawNaziv + " - " + _proizNaziv + " <" + _artiklCD + ">";
   }

   public decimal TH_MPCfromArtiklCD { get { return GetTH_MPCfromArtiklCD(this.ArtiklCD); } }

   public static decimal GetTH_MPCfromArtiklCD(string artiklCD)
   {
      if(artiklCD.Length < 4) return 0M;

      decimal theMPC = 0M;

      if(artiklCD.StartsWith("VR")) theMPC = ZXC.ValOrZero_Decimal(ZXC.GetStringsLastNchars(artiklCD, 4), 2) / 100M;
      else                          theMPC = ZXC.ValOrZero_Decimal(ZXC.GetStringsLastNchars(artiklCD, 4), 2);
      return theMPC;
   }

   // Konto as result po prioritetima: 
   // 1. s_konto                       
   // 2. grupa_konto                   
   // 3. uobSklad_konto                
   //public string R_Konto
   //{ 
   //   get 
   //   {
   //      if(this.Konto.NotEmpty()) return this.Konto;
   //
   //      //string grupa_konto = ZXC.luiListaGrupa1Artikla.GetNameForThisCd  ForThisCd(this.TS      )
   //      //if(
   //      return ""; 
   //   } 
   //}

   public bool IsSvdArtGR_Ljek_ { get { return IsSvdArtGR_Ljek(this.Grupa1CD); } }
   public bool IsSvdArtGR_Potr_ { get { return IsSvdArtGR_Potr(this.Grupa1CD); } }

   public static bool IsSvdArtGR_Ljek(string artGR1)
   {
      return
         artGR1 == "90" ||
         artGR1 == "A0" ||
         artGR1 == "N0" ||
         artGR1 == "10";
   }

   public static bool IsSvdArtGR_Potr(string artGR1)
   {
      return IsSvdArtGR_Ljek(artGR1) == false;
   }

   public static bool IsSvdSklCD_50(string skladCD)
   {
      return
         skladCD == "50";
   }

   public bool HasInvVisOrManj
   {
      get
      {
         return (
                 AS_InvFinKNJ_Manjk_AFT +
                 AS_InvFinKNJ_Manjk_BEF +
                 AS_InvFinKNJ_Visak_AFT +
                 AS_InvFinKNJ_Visak_BEF).NotZero();
      }
   }

   public static string Get_New_ArtiklCD_From_PCK_base_RAM_HDD(string PCK_baseCD, decimal ram, decimal hdd)
   {
      if(ZXC.IsPCTOGO == false) return PCK_baseCD;

    //return PCK_baseCD + "." + ram.ToString0Vv()         + "." + hdd.ToString0Vv()        ;
      return PCK_baseCD + "." + ram.ToString0Vv_NoGroup() + "." + hdd.ToString0Vv_NoGroup();
   }

   public string New_ArtiklCD_From_PCK_base_RAM_HDD
   {
      get
      {
         return Get_New_ArtiklCD_From_PCK_base_RAM_HDD(this.PCK_BazaCD, this.PCK_RAM, this.PCK_HDD);
      }
   }

   public static string Get_New_ArtiklName_From_OldPCK_name_RAM_HDD(string oldPCK_name, decimal ram, decimal hdd)
   {
      if(ZXC.IsPCTOGO == false) return oldPCK_name;

      return ZXC.ModifyPCK_ArtiklName(oldPCK_name, ram, hdd);
   }

   public string New_ArtiklName_From_OldPCK_name_RAM_HDD
   {
      get
      {
         return Get_New_ArtiklName_From_OldPCK_name_RAM_HDD(this.ArtiklName, this.PCK_RAM, this.PCK_HDD);
      }
   }

   public static string Get_PCK_BazaCD(string artiklCD)
   {
      if(ZXC.IsPCTOGO == false) return artiklCD;

      string[] tokens = artiklCD./*Replace(" ", "").*/Split(new char[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries);

      if(tokens.Length.IsZero()) return artiklCD;

      return tokens[0].TrimEnd(new char[] { ' '});
   }

   public static (decimal PCK_RAM, decimal PCK_HDD) Get_PTG_RAM_HDD_From_ArtiklCD(string artiklCD)
   {
      if(ZXC.IsPCTOGO == false) return (0, 0);

      //  nadi cio artiklCD_rec i vrati njegove zapremina, diuljina

      string[] tokens = artiklCD.Replace(" ", "").Split(new char[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries);

      decimal thePCK_RAM = 0M;
      decimal thePCK_HDD = 0M;

      if(tokens.Length == 0) return (0, 0);
      if(tokens.Length >  1) thePCK_RAM = ZXC.ValOrZero_Decimal(tokens[1], 0);
      if(tokens.Length >  2) thePCK_HDD = ZXC.ValOrZero_Decimal(tokens[2], 0);

      return (thePCK_RAM, thePCK_HDD);
   }

   //public string ArtiklCD_PCK_base
   //{
   //   get
   //   {
   //      return Get_ArtiklCD_PCK_base(this.ArtiklCD);
   //   }
   //}

   public static bool Has_equal_PCK_base(string artiklCD1, string artiklCD2)
   {
      return Get_PCK_BazaCD(artiklCD1) == Get_PCK_BazaCD(artiklCD2);
   }

   public static string Get_PTG_CalculatedArtiklCD_From_SenderArtiklCD_NewRAM_NewHDD(string senderArtiklCD, decimal newPCK_RAM, decimal newPCK_HDD)
   {
      if(ZXC.IsPCTOGO == false) return senderArtiklCD;

      Artikl origArtikl_rec = ZXC.TheVvForm.TheVvUC.Get_Artikl_FromVvUcSifrar(senderArtiklCD);

      if(origArtikl_rec == null) return "";

      Artikl newArtikl_rec = VvUserControl.ArtiklSifrar
         ./*Single*/FirstOrDefault(a => a.PCK_BazaCD.ToUpper() == origArtikl_rec.PCK_BazaCD.ToUpper() && 
                                        a.PCK_RAM              == newPCK_RAM                          && 
                                        a.PCK_HDD              == newPCK_HDD                           );

      if(newArtikl_rec == null)
      {
         if(false/*pitaj ga os / nes*/) return ""; // TODO 

         newArtikl_rec = ZXC.TheVvForm.ADD_NEW_PTG_CalculatedArtikl_From_SenderArtiklCD_NewRAM_NewHDD(origArtikl_rec, newPCK_RAM, newPCK_HDD);
      }

      if(newArtikl_rec == null) return "";
      else                      return newArtikl_rec.ArtiklCD;
   }


   public string  PCK_BazaCD { get { if(ZXC.IsPCTOGO == false) return ""; return this.CarTarifa; } set { if(ZXC.IsPCTOGO) this.CarTarifa = value; } }
   public decimal PCK_RAM    { get { if(ZXC.IsPCTOGO == false) return 0M; return this.Zapremina; } set { if(ZXC.IsPCTOGO) this.Zapremina = value; } }
   public decimal PCK_HDD    { get { if(ZXC.IsPCTOGO == false) return 0M; return this.Duljina  ; } set { if(ZXC.IsPCTOGO) this.Duljina   = value; } }

   #endregion Some Util - Results propertiz

   #region Ppmv - Posebni Porez Na Motorna Vozila 

   public static decimal Get_Ppmv_Iznos_1i2(decimal osnovica, ushort theCO2, decimal theCM3, ZXC.EuroNormaEnum theEN, bool isAutomobil)
   {
      if(ZXC.projectYearFirstDay.Year < 2017) return Get_Ppmv_Iznos_1i2_Bef2017(osnovica, theCO2, theCM3, theEN, isAutomobil);
      else                                    return Get_Ppmv_Iznos_1i2_2017   (osnovica, theCO2, theCM3, theEN, isAutomobil);
   }

   public static decimal Get_Ppmv_Iznos_1i2_2017(decimal osnovica, ushort theCO2, decimal theCM3, ZXC.EuroNormaEnum theEN, bool isAutomobil)
   {
      //bool isAutomobil = false; // TODO kada dodje neko sa autima... 

    //decimal stopa1, stopa2;
      decimal ppmvIznos = 0;

      if(isAutomobil)
      {
         //bool isDizel = true; // TODO 
         //
         //stopa1 = GetPpmvStopaFor_PorOsn   (osnovica);
         //
         //if(isDizel) stopa2 = GetPpmvStopaFor_CO2_Dizel(theCO2, theEN);
         //else        stopa2 = GetPpmvStopaFor_CO2_Benz (theCO2);
         throw new Exception("Get_Ppmv_Iznos_1i2 za AUTO is undone.");
      }
      else // Moto 
      {
         //stopa1 = GetPpmvStopaFor_CM3      (theCM3);
         //stopa2 = GetPpmvStopaFor_EuroNorma(theEN) ;

         ppmvIznos = GetMotoPpmv2017(theCM3, theEN);
      }

    //return Get_Ppmv_Iznos_1i2(osnovica, stopa1, stopa2);
      return ppmvIznos;
   }

   // 11.01.2023: ovo postaje _OLD 
   internal static decimal GetMotoPpmv2017_OLD(decimal theCM3, ZXC.EuroNormaEnum theEN)
   {
      decimal ppmvIznos = 0;

      // Motocikli i ATV vozila
      // Na novi motocikl i ATV vozilo posebni porez se plaća prema izrazu O x KO, u kojem je:
      // O – obujam motora u kubičnim centimetrima (cm³)
      // KO – koeficijent obujma motora prema Tablici 4 koji se uvećava ovisno o razini emisije ispušnih plinova na način da se za razinu emisije ispušnih plinova: EURO III uvećava za 5, EURO II uvećava za 10 i EURO I uvećava za 15.
      // Tablica 4 – obujam motora

      // Obujam motora u kub cm     KO 
      //   51  do   125              4 
      //  126  do   300              6 
      //  301  do   700              7 
      //  701  do  1000              8 
      // 1001  do                   10 

      // Ako na primjer obujam novog motocikla u kubičnim centimetrima iznosi 750 cm3 te 
      // razina emisije ispušnih plinova EURO III, posebni porez ćete za novi motocikl platiti u 
      // iznosu od 9.750,00 kuna (750 x (8+5) = 9.750,00).

      decimal KO = 0     ;
      decimal O  = theCM3;


           if(  51M <= theCM3 && theCM3 <=  125M) KO =  4.0M;
      else if( 126M <= theCM3 && theCM3 <=  300M) KO =  6.0M;
      else if( 301M <= theCM3 && theCM3 <=  700M) KO =  7.0M;
      else if( 701M <= theCM3 && theCM3 <= 1000M) KO =  8.0M;
      else if(                   theCM3 >= 1001M) KO = 10.0M;
                                                  
      else                                        KO =  0.0M;

      KO += GetPpmvStopaFor_EuroNorma_OLD(theEN);

      ppmvIznos = O * KO;

      return ppmvIznos;
   }

   internal static decimal GetMotoPpmv2017(decimal theCM3, ZXC.EuroNormaEnum theEN)
   {
      decimal ppmvIznos = 0;

      // Motocikli i ATV vozila
      // Na novi motocikl i ATV vozilo posebni porez se plaća prema izrazu O x KO, u kojem je:
      // O – obujam motora u kubičnim centimetrima (cm³)
      // KO – koeficijent obujma motora prema Tablici 4 koji se uvećava ovisno o razini emisije ispušnih plinova na način da se za razinu emisije ispušnih plinova: EURO III uvećava za 5, EURO II uvećava za 10 i EURO I uvećava za 15.
      // Tablica 4 – obujam motora

      // Obujam motora u kub cm     KO 
      //   51  do   125              4 
      //  126  do   300              6 
      //  301  do   700              7 
      //  701  do  1000              8 
      // 1001  do                   10 

      // Ako na primjer obujam novog motocikla u kubičnim centimetrima iznosi 750 cm3 te 
      // razina emisije ispušnih plinova EURO III, posebni porez ćete za novi motocikl platiti u 
      // iznosu od 9.750,00 kuna (750 x (8+5) = 9.750,00).

      decimal KO = 0     ;
      decimal O  = theCM3;

      // Novost u 2023 je da su ovi stari KO-ovi preracunati u eure 
           if(  51M <= theCM3 && theCM3 <=  125M) KO = /* 4.0M*/ 0.53M;
      else if( 126M <= theCM3 && theCM3 <=  300M) KO = /* 6.0M*/ 0.80M;
      else if( 301M <= theCM3 && theCM3 <=  700M) KO = /* 7.0M*/ 0.93M;
      else if( 701M <= theCM3 && theCM3 <= 1000M) KO = /* 8.0M*/ 1.06M;
      else if(                   theCM3 >= 1001M) KO = /*10.0M*/ 1.33M;
                                                  
      else                                        KO =  0.0M;

      KO += GetPpmvStopaFor_EuroNorma(theEN);

      ppmvIznos = O * KO;

      return ppmvIznos;
   }

   public static decimal Get_Ppmv_Iznos_1i2_Bef2017(decimal osnovica, ushort theCO2, decimal theCM3, ZXC.EuroNormaEnum theEN, bool isAutomobil)
   {
      //bool isAutomobil = false; // TODO kada dodje neko sa autima... 

      decimal stopa1, stopa2;

      if(isAutomobil)
      {
         bool isDizel = true; // TODO 

         stopa1 = GetPpmvStopaFor_PorOsn   (osnovica);

         if(isDizel) stopa2 = GetPpmvStopaFor_CO2_Dizel(theCO2, theEN);
         else        stopa2 = GetPpmvStopaFor_CO2_Benz (theCO2);
      }
      else
      {
         stopa1 = GetPpmvStopaFor_CM3      (theCM3);
         stopa2 = GetPpmvStopaFor_EuroNorma(theEN) ;
      }

      return Get_Ppmv_Iznos_1i2(osnovica, stopa1, stopa2);
   }

   // Moto 
   // 11.01.2023: ovo postaje _OLD 
   public static decimal GetPpmvStopaFor_EuroNorma_OLD(ZXC.EuroNormaEnum theEN)
   {
      decimal stopa2;

      // 10.01.2017: da se ne zbunis...               
      // do 2017 je stopa2 bila zaista stopa          
      // a od 2017 je stopa2 uvecanje KO za Moto ppmv 
      // tako da ti ista metoda koristi i za 2017     
      switch(theEN)
      {
         case ZXC.EuroNormaEnum.EuroIV : stopa2 =  0.00M; break;
         case ZXC.EuroNormaEnum.EuroIII: stopa2 =  5.00M; break;
         case ZXC.EuroNormaEnum.EuroII : stopa2 = 10.00M; break;
         case ZXC.EuroNormaEnum.EuroI  : stopa2 = 15.00M; break;

         default: stopa2 = 0; break;
      }

      return stopa2;
   }

   public static decimal GetPpmvStopaFor_EuroNorma(ZXC.EuroNormaEnum theEN)
   {
      decimal stopa2;

      // 10.01.2017: da se ne zbunis...               
      // do 2017 je stopa2 bila zaista stopa          
      // a od 2017 je stopa2 uvecanje KO za Moto ppmv 
      // tako da ti ista metoda koristi i za 2017     
      switch(theEN)
      {
         case ZXC.EuroNormaEnum.EuroIV : stopa2 = /* 0.00M*/  0.00M; break;
         case ZXC.EuroNormaEnum.EuroIII: stopa2 = /* 5.00M*/  0.66M; break;
         case ZXC.EuroNormaEnum.EuroII : stopa2 = /*10.00M*/  1.33M; break;
         case ZXC.EuroNormaEnum.EuroI  : stopa2 = /*15.00M*/  1.99M; break;

         default: stopa2 = 0; break;
      }

      return stopa2;
   }

   // Moto 
   public static decimal GetPpmvStopaFor_CM3(decimal theCM3)
   {
      decimal stopa1;

           if(  51M <= theCM3 && theCM3 <=  125M) stopa1 = 2.5M;
      else if( 126M <= theCM3 && theCM3 <=  250M) stopa1 = 3.0M;
      else if( 251M <= theCM3 && theCM3 <=  400M) stopa1 = 3.5M;
      else if( 401M <= theCM3 && theCM3 <=  600M) stopa1 = 4.0M;
      else if( 601M <= theCM3 && theCM3 <=  800M) stopa1 = 4.5M;
      else if( 801M <= theCM3 && theCM3 <= 1000M) stopa1 = 5.0M;
      else if(                   theCM3 >= 1001M) stopa1 = 5.5M;

      else                                        stopa1 = 0.0M;

      return stopa1;
   }

   // Auto 
   public static decimal GetPpmvStopaFor_PorOsn(decimal thePorOsn)
   {
      decimal stopa1;

           if(     0.00M <= thePorOsn && thePorOsn <= 100000.00M) stopa1 =  1M;
      else if(100000.01M <= thePorOsn && thePorOsn <= 150000.00M) stopa1 =  2M;
      else if(150000.01M <= thePorOsn && thePorOsn <= 200000.00M) stopa1 =  4M;
      else if(200000.01M <= thePorOsn && thePorOsn <= 250000.00M) stopa1 =  6M;
      else if(250000.01M <= thePorOsn && thePorOsn <= 300000.00M) stopa1 =  7M;
      else if(300000.01M <= thePorOsn && thePorOsn <= 350000.00M) stopa1 =  8M;
      else if(350000.01M <= thePorOsn && thePorOsn <= 400000.00M) stopa1 =  9M;
      else if(400000.01M <= thePorOsn && thePorOsn <= 450000.00M) stopa1 = 11M;
      else if(450000.01M <= thePorOsn && thePorOsn <= 500000.00M) stopa1 = 12M;
      else if(                           thePorOsn >= 500000.01M) stopa1 = 14M;

      else                                                        stopa1 =  0M;

      return stopa1;
   }

   public static decimal GetListPriceKorigStopaFor_MonthAge(int monthAge)
   {
      decimal stopa1;

           if(monthAge <=   0) stopa1 =100M;
      else if(monthAge <=   1) stopa1 = 97M;
      else if(monthAge <=   2) stopa1 = 94M;
      else if(monthAge <=   3) stopa1 = 91M;
      else if(monthAge <=   4) stopa1 = 89M;
      else if(monthAge <=   5) stopa1 = 87M;
      else if(monthAge <=   6) stopa1 = 85M;
      else if(monthAge <=   7) stopa1 = 84M;
      else if(monthAge <=   8) stopa1 = 83M;
      else if(monthAge <=   9) stopa1 = 82M;
      else if(monthAge <=  10) stopa1 = 81M;
      else if(monthAge <=  12) stopa1 = 80M;
      else if(monthAge <=  15) stopa1 = 77M;
      else if(monthAge <=  18) stopa1 = 74M;
      else if(monthAge <=  21) stopa1 = 71M;
      else if(monthAge <=  24) stopa1 = 69M;
      else if(monthAge <=  30) stopa1 = 64.5M;
      else if(monthAge <=  36) stopa1 = 60M;
      else if(monthAge <=  42) stopa1 = 56M;
      else if(monthAge <=  48) stopa1 = 52M;
      else if(monthAge <=  60) stopa1 = 45M;
      else if(monthAge <=  72) stopa1 = 39M;
      else if(monthAge <=  84) stopa1 = 34M;
      else if(monthAge <=  96) stopa1 = 30M;
      else if(monthAge <= 108) stopa1 = 27M;
      else if(monthAge <= 120) stopa1 = 26M;

      else                    stopa1 =  0M;

      return stopa1 / 100M;
   }

   // Auto 
   public static decimal GetPpmvStopaFor_CO2_Dizel(decimal theCO2, ZXC.EuroNormaEnum euroNorma)
   {
      if(euroNorma == ZXC.EuroNormaEnum.EuroVI) return GetPpmvStopaFor_CO2_E6(theCO2);
      else                                      return GetPpmvStopaFor_CO2_Dizel_EX(theCO2);
   }

   // Auto 
   public static decimal GetPpmvStopaFor_CO2_Dizel_EX(decimal theCO2)
   {
      decimal stopa1;

           if( 86 <= theCO2 && theCO2 <= 100) stopa1 =  1.5M;
      else if(101 <= theCO2 && theCO2 <= 110) stopa1 =  2.5M;
      else if(111 <= theCO2 && theCO2 <= 120) stopa1 =  3.5M;
      else if(121 <= theCO2 && theCO2 <= 130) stopa1 =  7.0M;
      else if(131 <= theCO2 && theCO2 <= 140) stopa1 = 11.5M;
      else if(141 <= theCO2 && theCO2 <= 160) stopa1 = 16.0M;
      else if(161 <= theCO2 && theCO2 <= 180) stopa1 = 18.0M;
      else if(181 <= theCO2 && theCO2 <= 200) stopa1 = 20.0M;
      else if(201 <= theCO2 && theCO2 <= 225) stopa1 = 23.0M;
      else if(226 <= theCO2 && theCO2 <= 250) stopa1 = 27.0M;
      else if(251 <= theCO2 && theCO2 <= 300) stopa1 = 29.0M;
      else if(                 theCO2 >= 301) stopa1 = 31.0M;

      else                                    stopa1 =  0.0M;

      return stopa1;
   }

   // Auto 
   public static decimal GetPpmvStopaFor_CO2_E6(decimal theCO2) // i benz i diesel 
   {
      decimal stopa1;

           if( 91 <= theCO2 && theCO2 <= 100) stopa1 =  1.0M;
      else if(101 <= theCO2 && theCO2 <= 110) stopa1 =  2.0M;
      else if(111 <= theCO2 && theCO2 <= 120) stopa1 =  3.0M;
      else if(121 <= theCO2 && theCO2 <= 130) stopa1 =  6.0M;
      else if(131 <= theCO2 && theCO2 <= 140) stopa1 = 10.0M;
      else if(141 <= theCO2 && theCO2 <= 160) stopa1 = 14.0M;
      else if(161 <= theCO2 && theCO2 <= 180) stopa1 = 16.0M;
      else if(181 <= theCO2 && theCO2 <= 200) stopa1 = 18.0M;
      else if(201 <= theCO2 && theCO2 <= 225) stopa1 = 21.0M;
      else if(226 <= theCO2 && theCO2 <= 250) stopa1 = 23.0M;
      else if(251 <= theCO2 && theCO2 <= 300) stopa1 = 27.0M;
      else if(                 theCO2 >= 301) stopa1 = 29.0M;

      else                                    stopa1 =  0.0M;

      return stopa1;
   }

   // Auto 
   public static decimal GetPpmvStopaFor_CO2_Benz(decimal theCO2)
   {
      decimal stopa1;

           if( 91 <= theCO2 && theCO2 <= 100) stopa1 =  1.0M;
      else if(101 <= theCO2 && theCO2 <= 110) stopa1 =  2.0M;
      else if(111 <= theCO2 && theCO2 <= 120) stopa1 =  3.0M;
      else if(121 <= theCO2 && theCO2 <= 130) stopa1 =  6.0M;
      else if(131 <= theCO2 && theCO2 <= 140) stopa1 = 10.0M;
      else if(141 <= theCO2 && theCO2 <= 160) stopa1 = 14.0M;
      else if(161 <= theCO2 && theCO2 <= 180) stopa1 = 16.0M;
      else if(181 <= theCO2 && theCO2 <= 200) stopa1 = 18.0M;
      else if(201 <= theCO2 && theCO2 <= 225) stopa1 = 21.0M;
      else if(226 <= theCO2 && theCO2 <= 250) stopa1 = 23.0M;
      else if(251 <= theCO2 && theCO2 <= 300) stopa1 = 27.0M;
      else if(                 theCO2 >= 301) stopa1 = 29.0M;

      else                                    stopa1 =  0.0M;

      return stopa1;
   }

   private static decimal Get_Ppmv_Iznos_1i2(decimal osnovica, decimal stopa1, decimal stopa2)
   {
      decimal ppmvIzn1;
      decimal ppmvIzn2;

      ppmvIzn1 = ZXC.VvGet_25_of_100(osnovica, stopa1);
      ppmvIzn2 = ZXC.VvGet_25_of_100(osnovica, stopa2);

      return (ppmvIzn1 + ppmvIzn2).Ron2();
   }

   #endregion Ppmv - Posebni Porez Na Motorna Vozila

}

public class BMW
{
   #region Parameters

   public decimal  RealMPC_EUR             { get; set; } // Cijena u oglasu   
   public decimal  HR_historyOsnovnaCij_Kn { get; set; } // Cijena u Hrvatskoj na dan prve registracije - osnovna bez opreme   
   public decimal  HR_historyOprermaCij_Kn { get; set; } // Cijena u Hrvatskoj na dan prve registracije - cijena dodat. opreme 
   public int      mm1Registracije         { get; set; }
   public int      yy1Registracije         { get; set; }
   public int      co2Emisija              { get; set; }
   public bool     isBenzinac              { get; set; }
   public DateTime buyOnDate               { get; set; }
   public DateTime date1Registracije       { get; set; }

   /// <summary>
   /// Cijena u Hrvatskoj na dan prve registracije TOTAL: VPC + PDV = MPC ('prije PPMV-a') 
   /// </summary>
   public decimal HR_history_TOTAL_Cij_Kn { get { return HR_historyOsnovnaCij_Kn + HR_historyOprermaCij_Kn; } } 

   public decimal todayTecaj { get; set; }

   #endregion Parameters

   #region Constructor

   const decimal MAX_priznatiAutoPDV = 50000M;

   public BMW(
                   decimal  _RealMPC_EUR            ,
                   decimal  _HR_historyOsnovnaCij_Kn,
                   decimal  _HR_historyOprermaCij_Kn,
                   int      _mm1Registracije        ,
                   int      _yy1Registracije        ,
                   int      _co2Emisija             ,
                   bool     _isBenzinac             ,
                   DateTime _buyOnDate              
      ) //: this()
   {
      this.RealMPC_EUR             = _RealMPC_EUR            ;
      this.HR_historyOsnovnaCij_Kn = _HR_historyOsnovnaCij_Kn;
      this.HR_historyOprermaCij_Kn = _HR_historyOprermaCij_Kn;
      this.mm1Registracije         = _mm1Registracije        ;
      this.yy1Registracije         = _yy1Registracije        ;
      this.co2Emisija              = _co2Emisija             ;
      this.isBenzinac              = _isBenzinac             ;
      this.buyOnDate               = _buyOnDate              ;

      this.todayTecaj              = ZXC.DevTecDao.GetHnbTecaj(ZXC.ValutaNameEnum.EUR, DateTime.Today);

    //this.date1Registracije       = new DateTime(yy1Registracije, mm1Registracije, 01); // !!! 
    //
    //SetVN();                       // !!! 
    //                               
    //if(isBenzinac) SetON_benzin(); // !!! 
    //else           SetON_diesel(); // !!! 
    //
    //SetMonthAge();                 // !!! 
    //SetAgeSt();                    // !!! 

      CalcResults();
   }

   public void CalcResults()
   {
      if(yy1Registracije.IsZero() || mm1Registracije.IsZero()) this.date1Registracije = DateTime.Now;
      else                                                     this.date1Registracije = new DateTime(yy1Registracije, mm1Registracije, 01);

      SetVN();                      
                                    
      if(isBenzinac) SetON_benzin();
      else           SetON_diesel();

      SetMonthAge();                
      SetAgeSt();                   
   }

   #endregion Constructor

   #region Results

   public decimal VN { get; set; }
 //public decimal PC { get; set; }
   public decimal ON { get; set; }
 //public decimal EN { get; set; }

   public int     VN_kat { get; set; }
   public decimal VN_OD  { get; set; }
   public decimal VN_DO  { get; set; }
   public decimal PC_st  { get; set; }
                         
   public int     ON_kat { get; set; }
   public decimal ON_OD  { get; set; }
   public decimal ON_DO  { get; set; }
   public decimal EN_1k  { get; set; }

   public decimal ageSt    { get; set; }
   public int     monthAge { get; set; }

   public void SetVN()
   {
     if(     0.00M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 100000.00M) { VN_kat =  1; VN_OD =      0.00M; VN_DO = 100000.00M; VN =     0M; PC_st =  0M; }
else if(100000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 150000.00M) { VN_kat =  2; VN_OD = 100000.00M; VN_DO = 150000.00M; VN =     0M; PC_st =  0M; }
else if(150000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 200000.00M) { VN_kat =  3; VN_OD = 150000.00M; VN_DO = 200000.00M; VN =  2000M; PC_st =  5M; }
else if(200000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 250000.00M) { VN_kat =  4; VN_OD = 200000.00M; VN_DO = 250000.00M; VN =  4500M; PC_st =  7M; }
else if(250000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 300000.00M) { VN_kat =  5; VN_OD = 250000.00M; VN_DO = 300000.00M; VN =  8000M; PC_st =  9M; }
else if(300000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 350000.00M) { VN_kat =  6; VN_OD = 300000.00M; VN_DO = 350000.00M; VN = 12500M; PC_st = 11M; }
else if(350000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 400000.00M) { VN_kat =  7; VN_OD = 350000.00M; VN_DO = 400000.00M; VN = 18000M; PC_st = 13M; }
else if(400000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 450000.00M) { VN_kat =  8; VN_OD = 400000.00M; VN_DO = 450000.00M; VN = 24500M; PC_st = 15M; }
else if(450000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 500000.00M) { VN_kat =  9; VN_OD = 450000.00M; VN_DO = 500000.00M; VN = 32000M; PC_st = 17M; }
else if(500000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 550000.00M) { VN_kat = 10; VN_OD = 500000.00M; VN_DO = 550000.00M; VN = 40500M; PC_st = 19M; }
else if(550000.01M <= HR_history_TOTAL_Cij_Kn && HR_history_TOTAL_Cij_Kn <= 600000.00M) { VN_kat = 11; VN_OD = 550000.00M; VN_DO = 600000.00M; VN = 50000M; PC_st = 20M; }
else if(              HR_history_TOTAL_Cij_Kn                            >= 600000.01M) { VN_kat = 12; VN_OD = 600000.00M; VN_DO =         0M; VN = 60000M; PC_st = 21M; }
else                                                                                    { VN_kat =  0; VN_OD =         0M; VN_DO =         0M; VN =     0M; PC_st =  0M; }
   }

   public decimal PC      { get { return ZXC.VvGet_25_of_100(HR_history_TOTAL_Cij_Kn - VN_OD, PC_st); } }
   public decimal VNPC    { get { return VN + PC; } }
   public decimal VNPCEur { get { return ZXC.DivSafe(VNPC, todayTecaj); } }
   public decimal ONEN    { get { return ON + EN; } }
   public decimal ONENEur { get { return ZXC.DivSafe(ONEN, todayTecaj); } }

   public void SetON_diesel()
   {
     if( 70.00M <= co2Emisija && co2Emisija <=  85.00M) { ON_kat =  1; ON_OD =  70.00M; ON_DO =  85.00M; ON =    185M; EN_1k =   55M; }
else if( 85.01M <= co2Emisija && co2Emisija <= 120.00M) { ON_kat =  2; ON_OD =  85.00M; ON_DO = 120.00M; ON =   1010M; EN_1k =  175M; }
else if(120.01M <= co2Emisija && co2Emisija <= 140.00M) { ON_kat =  3; ON_OD = 120.00M; ON_DO = 140.00M; ON =   7135M; EN_1k = 1150M; }
else if(140.01M <= co2Emisija && co2Emisija <= 170.00M) { ON_kat =  4; ON_OD = 140.00M; ON_DO = 170.00M; ON =  30135M; EN_1k = 1250M; }
else if(170.01M <= co2Emisija && co2Emisija <= 200.00M) { ON_kat =  5; ON_OD = 170.00M; ON_DO = 200.00M; ON =  67635M; EN_1k = 1350M; }
else if(           co2Emisija               >= 200.01M) { ON_kat =  6; ON_OD = 200.00M; ON_DO =      0M; ON = 108135M; EN_1k = 1450M; }
else                                                    { ON_kat =  0; ON_OD =      0M; ON_DO =      0M; ON =      0M; EN_1k =    0M; }
   }

   public void SetON_benzin()
   {
     if( 75.00M <= co2Emisija && co2Emisija <=  90.00M) { ON_kat =  1; ON_OD =  75.00M; ON_DO =  90.00M; ON =     95M; EN_1k =   35M; }
else if( 90.01M <= co2Emisija && co2Emisija <= 120.00M) { ON_kat =  2; ON_OD =  90.00M; ON_DO = 120.00M; ON =    620M; EN_1k =  135M; }
else if(120.01M <= co2Emisija && co2Emisija <= 140.00M) { ON_kat =  3; ON_OD = 120.00M; ON_DO = 140.00M; ON =   4670M; EN_1k =  450M; }
else if(140.01M <= co2Emisija && co2Emisija <= 170.00M) { ON_kat =  4; ON_OD = 140.00M; ON_DO = 170.00M; ON =  13670M; EN_1k =  700M; }
else if(170.01M <= co2Emisija && co2Emisija <= 200.00M) { ON_kat =  5; ON_OD = 170.00M; ON_DO = 200.00M; ON =  34670M; EN_1k = 1200M; }
else if(           co2Emisija               >= 200.01M) { ON_kat =  6; ON_OD = 200.00M; ON_DO =      0M; ON =  70670M; EN_1k = 1300M; }
else                                                    { ON_kat =  0; ON_OD =      0M; ON_DO =      0M; ON =      0M; EN_1k =    0M; }
   }

   public decimal EN { get { return (co2Emisija - ON_OD) * EN_1k; } }

   public void SetAgeSt()
   {
           if(monthAge <=   0) ageSt = 100M   ;
      else if(monthAge <=   1) ageSt =  96M   ;
      else if(monthAge <=   2) ageSt =  93M   ;
      else if(monthAge <=   3) ageSt =  90M   ;
      else if(monthAge <=   4) ageSt =  88M   ;
      else if(monthAge <=   5) ageSt =  86M   ;
      else if(monthAge <=   6) ageSt =  84M   ;
      else if(monthAge <=   7) ageSt =  82M   ;
      else if(monthAge <=   8) ageSt =  81M   ;
      else if(monthAge <=   9) ageSt =  80M   ;
      else if(monthAge <=  10) ageSt =  79M   ;
      else if(monthAge <=  11) ageSt =  78M   ;
      else if(monthAge <=  12) ageSt =  77M   ;
      else if(monthAge <=  13) ageSt =  76.04M;
      else if(monthAge <=  14) ageSt =  75.08M;
      else if(monthAge <=  15) ageSt =  74.12M;
      else if(monthAge <=  16) ageSt =  73.16M;
      else if(monthAge <=  17) ageSt =  72.20M;
      else if(monthAge <=  18) ageSt =  71.24M;
      else if(monthAge <=  19) ageSt =  70.28M;
      else if(monthAge <=  20) ageSt =  69.32M;
      else if(monthAge <=  21) ageSt =  68.36M;
      else if(monthAge <=  22) ageSt =  67.40M;
      else if(monthAge <=  23) ageSt =  66.44M;
      else if(monthAge <=  24) ageSt =  65.00M;
      else if(monthAge <=  26) ageSt =  63.58M;
      else if(monthAge <=  28) ageSt =  62.16M;
      else if(monthAge <=  30) ageSt =  60.74M;
      else if(monthAge <=  32) ageSt =  59.32M;
      else if(monthAge <=  34) ageSt =  57.90M;
      else if(monthAge <=  36) ageSt =  56.48M;
      else if(monthAge <=  38) ageSt =  55.06M;
      else if(monthAge <=  40) ageSt =  53.64M;
      else if(monthAge <=  42) ageSt =  52.22M;
      else if(monthAge <=  44) ageSt =  50.80M;
      else if(monthAge <=  46) ageSt =  49.38M;
      else if(monthAge <=  48) ageSt =  47.96M;
      else if(monthAge <=  51) ageSt =  46.36M;
      else if(monthAge <=  54) ageSt =  44.76M;
      else if(monthAge <=  57) ageSt =  43.16M;
      else if(monthAge <=  60) ageSt =  41.56M;
      else                     ageSt =      0M;

      // tu ti se nija dalo prepisivati preko 60 mjeseca 

   }

   public void SetMonthAge()
   {
      monthAge = ZXC.MonthDifference(buyOnDate, date1Registracije);

      if(IsNHR) monthAge = 0; // !!! 

   }

   public decimal PPMV_historyKn    { get { return (VN + PC) + (ON + EN)                     ; } }
   public decimal PPMV_historyEUR   { get { return ZXC.DivSafe(PPMV_historyKn, todayTecaj)   ; } }
                                                                                             
   public decimal PPMV_Kn           { get { return PPMV_historyKn * (ageSt / 100M)           ; } }
   public decimal PPMV_EUR          { get { return ZXC.DivSafe(PPMV_Kn, todayTecaj)          ; } }
                                                                                             
   public decimal toPayAutohouseEUR { get { return ZXC.VvGet_100_from_125(RealMPC_EUR, 19)   ; } }
   public decimal toPayAutohouseKn  { get { return toPayAutohouseEUR * todayTecaj            ; } }
   
   public decimal toPayHR_PDVkn     { get { return ZXC.VvGet_25_of_100 (toPayAutohouseKn, 25); } }
 //public decimal good_PDVkn        { get { return          toPayHR_PDVkn / 2                ; } }
   public decimal good_PDVkn        { get { return Math.Min(toPayHR_PDVkn / 2, MAX_priznatiAutoPDV); } }
   
   public decimal toPayTotalKn      { get { return toPayAutohouseKn + toPayHR_PDVkn + PPMV_Kn; } }
   public decimal toPayTotalEUR     { get { return ZXC.DivSafe(toPayTotalKn, todayTecaj  )   ; } }
   
   public decimal finalCostKn       { get { return toPayTotalKn - good_PDVkn                 ; } }
   public decimal finalCostEUR      { get { return ZXC.DivSafe(finalCostKn, todayTecaj   )   ; } }

   // NHR additions 

   public bool    IsNHR                { get { return RealMPC_EUR.IsZero(); } }
   
   public decimal NHR_toPayHR_PDVkn    { get { return ZXC.VvGet_25_from_125(HR_history_TOTAL_Cij_Kn, 25)          ; } }
   public decimal NHR_toPayTotalKn     { get { return HR_history_TOTAL_Cij_Kn + /*NHR_toPayHR_PDVkn +*/ PPMV_historyKn; } }
 //public decimal NHR_good_PDVkn       { get { return          NHR_toPayHR_PDVkn / 2                              ; } }
   public decimal NHR_good_PDVkn       { get { return Math.Min(NHR_toPayHR_PDVkn / 2, MAX_priznatiAutoPDV)        ; } }
   public decimal NHR_finalCostKn      { get { return NHR_toPayTotalKn - NHR_good_PDVkn                           ; } }
   public decimal NHR_toPayTotalEUR    { get { return ZXC.DivSafe(NHR_toPayTotalKn, todayTecaj                )   ; } }
   public decimal NHR_finalCostEUR     { get { return ZXC.DivSafe(NHR_finalCostKn , todayTecaj                )   ; } }


   #endregion Results

   #region Helpers

   // (VN + PC) + (ON + EN)

   // Zakon propisuje da se posebni porez na ovu vrstu vozila plaća ovisno o vrsti goriva koje vozilo koristi za pogon prema izrazu (VN + PC) + (ON + EN), u kojem je:
   // VN – vrijednosna naknada u kunama                           
   // PC - naknada koja se utvrđuje na način da se od prodajne cijene motornog vozila oduzme najniži iznos za skupinu kojoj motorno vozilo pripada prema Tablici 1 i tako dobiveni iznos pomnoži s postotkom utvrđenim za skupinu kojoj motorno vozilo pripada prema Tablici 1
   // ON – osnovna naknada u kunama prema Tablici 2 ili Tablici 3 
   // EN – naknada koja se utvrđuje na način da se od iznosa prosječne emisije ugljičnog dioksida (CO2) motornog vozila oduzme najniži iznos za skupinu kojoj motorno vozilo pripada prema Tablici 2 ili Tablici 3 i tako dobiveni iznos pomnoži s pripadajućim iznosom u kunama za jedan g/km CO2

   #endregion Helpers
}

public struct ArtiklInfo
{
   private Rtrans rtrans_rec;
   public ArtiklInfo(Rtrans _rtrans_rec) : this()
   {
      this.rtrans_rec = _rtrans_rec;

      ArtiklSifra = rtrans_rec.T_artiklCD       ;
      ArtiklNaziv = rtrans_rec.T_artiklName     ;
                  
      SkladSifra  = rtrans_rec.TheAsEx.SkladCD  ;
      StanjeKol   = rtrans_rec.TheAsEx.StanjeKol;
   }

   public string   ArtiklSifra  { get; set; }
   public string   ArtiklNaziv  { get; set; }
   public string   SkladSifra   { get; set; }
   public decimal  StanjeKol    { get; set; }
}

// 09.11.2014: koji je ovo kur'tz? Jel' to nesto za Tembo 2 web? 
public class ArtiklInfoExporter
{
   #region Fieldz

   private static XmlSerializer xmlSer = new XmlSerializer(typeof(ArtiklInfoExporter));
   private static string rootFileName = @"VvArtStatExporter";

   private string fileName;
   private string filePath;

   private string FilePathAndName
   {
      get
      {
         return filePath + @"\" + fileName;
      }
   }

   #endregion Fieldz

   #region Constructor

   public ArtiklInfoExporter() // bez ovoga nece serializirati?! 
   {
   }

   public ArtiklInfoExporter(List<Rtrans> rtransList, string _filePath, DateTime timeStamp)
   {
      this.filePath     = _filePath  ;
      this.fileName     = CreateFilename(rootFileName, timeStamp);
      this.DatumStatusa = timeStamp;

      this.ArtiklInfoList = new List<ArtiklInfo>(rtransList.Count);

      foreach(Rtrans rtrans in rtransList)
      {
         ArtiklInfoList.Add(new ArtiklInfo(rtrans));
      }

   }

   #endregion Constructor

   #region Methods

   public bool Save_ArtStatXmlExporter_ToXmlFile()
   {
      bool success = false;

      try
      {
         using(System.IO.StreamWriter sw = new System.IO.StreamWriter(this.FilePathAndName))
         {
            xmlSer.Serialize(sw, this);
            sw.Close();
            success = true;
         }
      }
      catch(Exception ex)
      {
         System.Windows.Forms.MessageBox.Show("Save_ArtStatXmlExporter_ToXmlFile PROBLEM:\n\n" + ex.Message);
      }

      return success;
   }

   private string CreateFilename(string rootFileName, DateTime timeStamp)
   {
      return rootFileName + "_" + timeStamp.ToString(ZXC.VvTimeStampFormat4FileName) + ".xml";
   }

   #endregion Methods

   #region Serializible Fields And Propertiz

   public DateTime DatumStatusa           { get; set; }
   public List<ArtiklInfo> ArtiklInfoList { get; set; }

   #endregion Serializible Fields And Propertiz

}

public struct RNMstatus
{
   public string  TT               { get; set; }
   public uint    TtNum            { get; set; }
   public string  ProjektCD        { get; set; }

   public decimal koefDovrsPG      { get; set; }
   public decimal koefDovrsOG      { get; set; }
   public decimal koefDovrsUK      { get; set; }
   public decimal koefDovrsUKreal  { get; set; }

   public decimal ukKolKgUlazPG    { get; set; }
   public decimal ukKolKgUlazOG    { get; set; }
   public decimal ukKolIzlazOG     { get; set; }
   public decimal ukKolUlazOG      { get; set; }
   public decimal ukKolKgIzlazOG   { get; set; }
   public decimal ukKolKgUlazUK    { get; set; }

   public decimal ukFinIzlazPG     { get; set; }
   public decimal ukFinNedovrIzlPG { get; set; }
   public decimal ukFinUlazPG      { get; set; }
   public decimal ukFinIzlazOG     { get; set; }
   public decimal ukFinUlazOG      { get; set; }
   public decimal ukFinIzlazUK     { get; set; }
   public decimal finDiff          { get; set; }
   public decimal ukFinViaOTP      { get; set; }

   public decimal ncPerUlKol       { get; set; }
   public decimal ncPerUlKolKg     { get; set; } // VOLIA! prema PPR fin 
   public decimal ncPerUlKolKgOTP  { get; set; } // VOLIA! prema OTP fin 

   public RNMstatus(Faktur rnmFaktur_rec, List<Rtrans> rnuRtransList, List<Rtrans> ppr_pip_RtransListOG) : this()
   {
      // !!! ako je rnmFaktur_rec.Transes prazan, znaci da smo dosli iz Reporta,
      // a rnuRtransList je u biti to kaj trebamo pa mu podmecemo.
      if(rnmFaktur_rec.Transes == null || rnmFaktur_rec.Transes.Count.IsZero())
      {
         rnmFaktur_rec.Transes = rnuRtransList;
      }

      TT              = rnmFaktur_rec.TT       ;
      TtNum           = rnmFaktur_rec.TtNum    ;
      ProjektCD       = rnmFaktur_rec.ProjektCD;

      ukFinIzlazPG    = rnmFaktur_rec.Decimal01; // ApgUk 
      ukKolKgUlazPG   = rnmFaktur_rec.Decimal02; // Bpg   
      ukFinViaOTP     = rnmFaktur_rec.R2_uplata; // Aotp  
                      
      koefDovrsPG     = RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list_PG(rnuRtransList, ukKolKgUlazPG                                    ); // za Spg      
      koefDovrsOG     = RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list   (true , rnmFaktur_rec, ppr_pip_RtransListOG, ukKolKgUlazPG, false); // za Sog      
      koefDovrsUK     = RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list   (false, rnmFaktur_rec, ppr_pip_RtransListOG, ukKolKgUlazPG, false); // za Suk      
      koefDovrsUKreal = RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list   (false, rnmFaktur_rec, ppr_pip_RtransListOG, ukKolKgUlazPG, true ); // za Suk Real 

      ukFinNedovrIzlPG = ukFinIzlazPG * (1 - koefDovrsPG); // Apg = ApgUk * Spg 

      ukFinUlazPG      = ukKolKgUlazPG * ZXC.DivSafe(ukFinIzlazPG * koefDovrsPG, ukKolKgUlazPG); // fuse 

      ukFinIzlazOG     = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PPR).Sum(rtr => rtr.R_KC   .Ron2()); // Aog           
      ukKolKgUlazOG    = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.R_kolOP.Ron2()); // Bog           
      ukFinUlazOG      = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.R_KC   .Ron2()); // Cog           
      ukKolIzlazOG     = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PPR).Sum(rtr => rtr.T_kol  .Ron2()); // fuse          
      ukKolUlazOG      = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.T_kol  .Ron2()); // za ncPerUlKol 
      ukKolKgIzlazOG   = ppr_pip_RtransListOG.Where(rtr => rtr.T_TT == Faktur.TT_PPR).Sum(rtr => rtr.R_kolOP.Ron2()); // fuse          
                       
      ukKolKgUlazUK    = ukKolKgUlazPG + ukKolKgUlazOG                  ; // Bpg + Bog 
      ukFinIzlazUK     = (ukFinNedovrIzlPG + ukFinIzlazOG) * koefDovrsOG; // Auk       
                       
      finDiff          = (ukFinUlazOG - ukFinIzlazUK).Ron2(); // (Cog-Auk) 
                       
      ncPerUlKol       = ZXC.DivSafe(ukFinIzlazUK, ukKolUlazOG        ).Ron(/*4*/2); // (Auk / ukKolUlazOG) 
                       
      ncPerUlKolKg     = ZXC.DivSafe(ukFinIzlazUK, ukKolKgUlazOG/*UK*/).Ron(/*4*/2); // (Auk / Bog)         VOILA! 

      ncPerUlKolKgOTP  = ZXC.DivSafe(ukFinViaOTP , ukKolKgUlazOG/*UK*/).Ron(/*4*/2); // (Aotp / Bog)        VOILA! 
   }

   public override string ToString()
   {
      return TT + "-" + TtNum + " ncPerUlKolKgOTP: " + ncPerUlKolKgOTP.ToStringVv();
   }
}

