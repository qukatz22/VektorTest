using System;
using System.Collections.Generic;
using System.Linq;

#region struct ArtStatStruct

public struct ArtStatStruct
{
   internal uint     _recID;

   /* 01 */ internal DateTime _addTS            ;
   /* 02 */ internal string   _addUID           ;
   /* 03 */ internal uint     _rtransRecID      ;
   /* 04 */ internal string   _t_artiklCD       ;
   /* 05 */ internal string   _t_skladCD        ;
   /* 06 */ internal DateTime _t_skladDate      ;
   /* 07 */ internal string   _t_tt             ;
   /* 08 */ internal Int16    _t_ttSort         ;
   /* 09 */ internal uint     _t_ttNum          ;
   /* 10 */ internal ushort   _t_serial         ;
   /* 11 */ internal uint     _transRbr         ;
   /* 12 */ internal decimal  _pstFinNBC        ;
   /* 13 */ internal decimal  _ulazFinNBC       ;
   /* 14 */ internal decimal  _izlazFinNBC      ;
   /* 15 */ internal decimal  _ulazFirmaFinNBC  ;
   /* 16 */ internal decimal  _izlazFirmaFinNBC ;
   /* 17 */ internal decimal  _lastPrNabCij     ;
   /* 18 */ internal decimal  _pstFinMPC        ;
   /* 19 */ internal decimal  _ulazFinMPC       ;
   /* 20 */ internal decimal  _izlazFinMPC      ;
   /* 21 */ internal decimal  _ulazFirmaFinMPC  ;
   /* 22 */ internal decimal  _izlazFirmaFinMPC ;
   /* 23 */ internal decimal  _lastMalopCij     ; 
   /* 24 */ internal decimal  _pstKol           ;
   /* 25 */ internal decimal  _invKol           ;
   /* 26 */ internal decimal  _invFin           ;
   /* 27 */ internal decimal  _ulazKol          ;
   /* 28 */ internal decimal  _ulazFinKCR       ;
   /* 29 */ internal decimal  _ulazCijMin       ;
   /* 30 */ internal decimal  _ulazCijMax       ;
   /* 31 */ internal decimal  _ulazCijLast      ;
   /* 32 */ internal decimal  _izlazKol         ;
   /* 33 */ internal decimal  _stanjeKolRezerv  ;
   /* 34 */ internal decimal  _izlFinProdKCR    ;
   /* 35 */ internal decimal  _izlazCijMin      ;
   /* 36 */ internal decimal  _izlazCijMax      ;
   /* 37 */ internal decimal  _izlazCijLast     ;
   /* 38 */ internal decimal  _preDefVpc1       ;
   /* 39 */ internal decimal  _preDefVpc2       ;
   /* 40 */ internal decimal  _preDefMpc1       ;
   /* 41 */ internal decimal  _preDefDevc       ;
   /* 42 */ internal decimal  _preDefRbt1       ;
   /* 43 */ internal decimal  _preDefRbt2       ;
   /* 44 */ internal decimal  _preDefMinKol     ;
   /* 45 */ internal decimal  _preDefMarza      ;
   /* 46 */ internal decimal  _ulazKolFisycal   ; 
   /* 47 */ internal decimal  _izlazKolFisycal  ; 
   /* 48 */ internal DateTime _dateZadUlaz      ;
   /* 49 */ internal DateTime _dateZadIzlaz     ;
   /* 50 */ internal DateTime _dateZadPst       ;
   /* 51 */ internal DateTime _dateZadInv       ;
   /* 52 */ internal string   _artiklTS         ;
   /* 53 */ internal string   _artiklJM         ;
   /* 54 */ internal string   _frsMinTt         ;
   /* 55 */ internal uint     _frsMinTtNum      ;
   /* 56 */ internal decimal  _ulazFirmaKol     ;
   /* 57 */ internal decimal  _izlazFirmaKol    ;
   /* 58 */ internal decimal  _rezervKolNaruc   ; 
   /* 59 */ internal decimal  _rezervKolIsporu  ;
   
   /* 60 */ internal decimal  _rtrPstKol        ; 
   /* 61 */ internal decimal  _rtrUlazKol       ; 
   /* 62 */ internal decimal  _rtrIzlazKol      ; 
   /* 63 */ internal decimal  _rtrUlazKolFisycal; 
   /* 64 */ internal decimal  _rtrIzlzKolFisycal; 
   /* 65 */ internal decimal  _rtrKolNaruceno   ; 
   /* 66 */ internal decimal  _rtrKolIsporuceno ; 
   /* 67 */ internal decimal  _rtrPstVrjNBC     ; 
   /* 68 */ internal decimal  _rtrUlazVrjNBC    ; 
   /* 69 */ internal decimal  _rtrIzlazVrjNBC   ; 
   /* 70 */ internal decimal  _rtrPstCijNBC     ; 
   /* 71 */ internal decimal  _rtrUlazCijNBC    ; 
   /* 72 */ internal decimal  _rtrIzlazCijNBC   ; 
   /* 73 */ internal decimal  _rtrCijenaNBC     ; 
   /* 74 */ internal decimal  _rtrPstVrjMPC     ; 
   /* 75 */ internal decimal  _rtrUlazVrjMPC    ; 
   /* 76 */ internal decimal  _rtrIzlazVrjMPC   ; 
   /* 77 */ internal decimal  _rtrPstCijMPC     ; 
   /* 78 */ internal decimal  _rtrUlazCijMPC    ; 
   /* 79 */ internal decimal  _rtrIzlazCijMPC   ; 
   /* 80 */ internal decimal  _rtrCijenaMPC     ; 

   /* 81 */ internal decimal  _prevMalopCij     ; 

   /* 82 */ internal decimal  _orgPak           ; 
   /* 83 */ internal string   _orgPakJM         ; 

   /* 84 */ internal decimal  _ulazKol2         ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 85 */ internal decimal  _izlazKol2        ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 86 */ internal decimal  _pstKol2          ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 87 */ internal decimal  _invKol2          ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 98 */ internal decimal  _ulazFirmaKol2    ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 99 */ internal decimal  _izlazFirmaKol2   ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 90 */ internal decimal  _rtrPstKol2       ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 91 */ internal decimal  _rtrUlazKol2      ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...
   /* 92 */ internal decimal  _rtrIzlazKol2     ; // paralelni svijet kolicina. NIJE jednoznacno ovisan o normal kol. Npr Alumil komada letvi i duljina letve. Tekstilhaus broj vreča...

   /* 93 */ internal string   _artGrCd1         ; 
   /* 94 */ internal string   _artGrCd2         ; 
   /* 95 */ internal string   _artGrCd3         ; 

   /* 96 */ internal decimal  _rtrPdvSt         ; // news 4 faster JOINs (no need for rtrans) 
   /* 97 */ internal bool     _rtrIsIrmUslug    ; // news 4 faster JOINs (no need for rtrans) 
   /* 98 */ internal uint     _rtrParentID      ; // news 4 faster JOINs (no need for rtrans) 

   /* 99 */ internal decimal  _invKolDiff       ; // news 4 new inventuraDiff system 
   /*100 */ internal decimal  _invKol2Diff      ; // news 4 new inventuraDiff system 
   /*101 */ internal decimal  _invFinDiff       ; // NBC! news 4 new inventuraDiff system 
   /*102 */ internal decimal  _invFinDiffMPC    ; // news 4 new inventuraDiff system 
   /*103 */ internal decimal  _invFinMPC        ; // od ranije postoji _invFin koji ce odsada biti _invFinNBC 

   // 2023: 
   /*104 */ internal decimal  _prNBCBefThisUlaz ; // za NUP shadow rtrans 

}

#endregion struct ArtStatStruct

public class ArtStat : VvDataRecord, IComparable<ArtStat>, IVvExtenderDataRecord
{

   #region Fildz

   public const string recordName       = "artstat";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   public /*private*/ ArtStatStruct currentData;
   private ArtStatStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.ArtStatDao.TheSchemaTable;
   protected static ArtStatDao.ArtStatCI CI              = ZXC.ArtStatDao.CI;

   #endregion Fildz

   #region Sorters

   public static VvSQL.RecordSorter sorterArtStat = new VvSQL.RecordSorter(ArtStat.recordName, ArtStat.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_artiklCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladCD ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial  ]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "Qwerty", VvSQL.SorterType.ArtStat, false);

   private VvSQL.RecordSorter[] _sorters =
      new  VvSQL.RecordSorter[]
      { 
         sorterArtStat
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         //case VvSQL.SorterType.ArtStat: return new object[] { this.ArtStatName, this.ArtStatCD, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion Sorters

   #region Constructors

   public ArtStat() : this(0)
   {
   }

   public ArtStat(uint ID) : base()
   {
      this.currentData = new ArtStatStruct();

      Memset0(ID);
   }

   public ArtStat(Rtrans rtrans_rec) : this(0)
   {
      this.RtransRecID = rtrans_rec.T_recID    ;
      this.ArtiklCD    = rtrans_rec.T_artiklCD ;
      this.SkladCD     = rtrans_rec.T_skladCD  ;
      this.SkladDate   = rtrans_rec.T_skladDate;
      this.TT          = rtrans_rec.T_TT       ;
      this.TtSort      = rtrans_rec.T_ttSort   ;
      this.TtNum       = rtrans_rec.T_ttNum    ;
      this.Serial      = rtrans_rec.T_serial   ;
   }

   public override void Memset0(uint ID)
   {
               this.currentData._recID = ID;

               this.currentData._addTS             = DateTime.MinValue;
               this.currentData._addUID            = "";

      /* 03 */ this.currentData._rtransRecID       = 0;
      /* 04 */ this.currentData._t_artiklCD        = "";
      /* 05 */ this.currentData._t_skladCD         = "";
      /* 06 */ this.currentData._t_skladDate         = DateTime.MinValue;
      /* 07 */ this.currentData._t_tt              = "";
      /* 08 */ this.currentData._t_ttSort          = 0;
      /* 09 */ this.currentData._t_ttNum           = 0;
      /* 10 */ this.currentData._t_serial          = 0;
      /* 11 */ this.currentData._transRbr          = 0;

      /* 12 */ this.currentData._pstFinNBC         = 0.00M;
      /* 13 */ this.currentData._ulazFinNBC        = 0.00M;
      /* 14 */ this.currentData._izlazFinNBC       = 0.00M;
      /* 15 */ this.currentData._ulazFirmaFinNBC   = 0.00M;
      /* 16 */ this.currentData._izlazFirmaFinNBC  = 0.00M;
      /* 17 */ this.currentData._lastPrNabCij      = 0.00M;
      /* 18 */ this.currentData._pstFinMPC         = 0.00M;
      /* 19 */ this.currentData._ulazFinMPC        = 0.00M;
      /* 20 */ this.currentData._izlazFinMPC       = 0.00M;
      /* 21 */ this.currentData._ulazFirmaFinMPC   = 0.00M;
      /* 22 */ this.currentData._izlazFirmaFinMPC  = 0.00M;
      /* 23 */ this.currentData._lastMalopCij      = 0.00M;
      /* 24 */ this.currentData._pstKol            = 0.00M;
      /* 25 */ this.currentData._invKol            = 0.00M;
      /* 26 */ this.currentData._invFin            = 0.00M;
      /* 27 */ this.currentData._ulazKol           = 0.00M;
      /* 28 */ this.currentData._ulazFinKCR        = 0.00M;
      /* 29 */ this.currentData._ulazCijMin        = 0.00M;
      /* 30 */ this.currentData._ulazCijMax        = 0.00M;
      /* 31 */ this.currentData._ulazCijLast       = 0.00M;
      /* 32 */ this.currentData._izlazKol          = 0.00M;
      /* 33 */ this.currentData._stanjeKolRezerv   = 0.00M;
      /* 34 */ this.currentData._izlFinProdKCR     = 0.00M;
      /* 35 */ this.currentData._izlazCijMin       = 0.00M;
      /* 36 */ this.currentData._izlazCijMax       = 0.00M;
      /* 37 */ this.currentData._izlazCijLast      = 0.00M;
      /* 38 */ this.currentData._preDefVpc1        = 0.00M;
      /* 39 */ this.currentData._preDefVpc2        = 0.00M;
      /* 48 */ this.currentData._preDefMpc1        = 0.00M;
      /* 49 */ this.currentData._preDefDevc        = 0.00M;
      /* 40 */ this.currentData._preDefRbt1        = 0.00M;
      /* 41 */ this.currentData._preDefRbt2        = 0.00M;
      /* 42 */ this.currentData._preDefMinKol      = 0.00M;
      /* 43 */ this.currentData._preDefMarza       = 0.00M;
      /* 46 */ this.currentData._ulazKolFisycal    = 0.00M;
      /* 47 */ this.currentData._izlazKolFisycal   = 0.00M;
      /* 48 */ this.currentData._dateZadUlaz       = DateTime.MinValue;
      /* 49 */ this.currentData._dateZadIzlaz      = DateTime.MinValue;
      /* 50 */ this.currentData._dateZadPst        = DateTime.MinValue;
      /* 51 */ this.currentData._dateZadInv        = DateTime.MinValue;
      /* 52 */ this.currentData._artiklTS          = "";
      /* 53 */ this.currentData._artiklJM          = "";
      /* 54 */ this.currentData._frsMinTt          = "";
      /* 55 */ this.currentData._frsMinTtNum       = 0;
      /* 56 */ this.currentData._ulazFirmaKol      = 0.00M;
      /* 57 */ this.currentData._izlazFirmaKol     = 0.00M;
      /* 58 */ this.currentData._rezervKolNaruc    = 0.00M;
      /* 59 */ this.currentData._rezervKolIsporu   = 0.00M;

               this.currentData._rtrPstKol         = 0.00M;
               this.currentData._rtrUlazKol        = 0.00M;
               this.currentData._rtrIzlazKol       = 0.00M;
               this.currentData._rtrPstVrjNBC      = 0.00M;
               this.currentData._rtrUlazVrjNBC     = 0.00M;
               this.currentData._rtrIzlazVrjNBC    = 0.00M;
               this.currentData._rtrPstCijNBC      = 0.00M;
               this.currentData._rtrUlazCijNBC     = 0.00M;
               this.currentData._rtrIzlazCijNBC    = 0.00M;
               this.currentData._rtrCijenaNBC      = 0.00M;
               this.currentData._rtrPstVrjMPC      = 0.00M;
               this.currentData._rtrUlazVrjMPC     = 0.00M;
               this.currentData._rtrIzlazVrjMPC    = 0.00M;
               this.currentData._rtrPstCijMPC      = 0.00M;
               this.currentData._rtrUlazCijMPC     = 0.00M;
               this.currentData._rtrIzlazCijMPC    = 0.00M;
               this.currentData._rtrCijenaMPC      = 0.00M;
               this.currentData._prevMalopCij      = 0.00M;
               this.currentData._orgPak            = 0.00M;
               this.currentData._orgPakJM          = "";
               this.currentData._rtrUlazKolFisycal = this.currentData._rtrIzlzKolFisycal = this.currentData._rtrKolNaruceno = this.currentData._rtrKolIsporuceno = 0.00M;

               this.currentData._ulazKol2          = 0.00M;
               this.currentData._izlazKol2         = 0.00M;
               this.currentData._pstKol2           = 0.00M;
               this.currentData._invKol2           = 0.00M;
               this.currentData._ulazFirmaKol2     = 0.00M;
               this.currentData._izlazFirmaKol2    = 0.00M;
               this.currentData._rtrPstKol2        = 0.00M;
               this.currentData._rtrUlazKol2       = 0.00M;
               this.currentData._rtrIzlazKol2      = 0.00M;

               this.currentData._artGrCd1 = "";
               this.currentData._artGrCd2 = "";
               this.currentData._artGrCd3 = "";

               this.currentData._rtrPdvSt          = 0.00M;
               this.currentData._rtrIsIrmUslug     = false;
               this.currentData._rtrParentID       = 0;
               this.currentData._invKolDiff        = 0.00M;
               this.currentData._invKol2Diff       = 0.00M;
               this.currentData._invFinDiff     = 0.00M;
               this.currentData._invFinDiffMPC     = 0.00M;
               this.currentData._invFinMPC         = 0.00M;
               this.currentData._prNBCBefThisUlaz  = 0.00M;

   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return ArtiklCD + ": " + SkladDate.ToShortDateString() + " skl " + SkladCD + " " + TT + " br " + TtNum + " ser " + Serial;
   }

   #endregion ToString

   #region Propertiz --- !!! PAZI: Kada ovdje nesto mijenjas moras i u class 'Rtrans' i 'Artikl' !!!

   #region Common

   internal ArtStatStruct CurrentData // cijela ArtStatStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   internal ArtStatStruct BackupData // zasada samo za ovaj ArtStat record za potrebe RENAME USER-a
   {
      get { return this.backupData; }
      set { this.backupData = value; }
   }

   //public TtInfo TtInfo { get { try {                      return ZXC.RiskTT[this.TT]               ; } catch(Exception) { return new TtInfo(); } } }
   public TtInfo TtInfo { get { try { return this.TT.NotEmpty() ? ZXC.RiskTT[this.TT] : new TtInfo(); } catch(Exception) { return new TtInfo(); } } }

   public static string ArtiklForeignKey
   {
      get { return "t_artiklCD"; }
   }

   public bool IsNullFromReader
   {
      get
      {
         // 27.10.2014: 
       //return (ArtiklCD == null   && SkladCD == null   && TT == null   && RtransRecID == 0);     // && ili || pitanje je sad?! 
         return (ArtiklCD.IsEmpty() && SkladCD.IsEmpty() && TT.IsEmpty() && RtransRecID.IsZero()); // && ili || pitanje je sad?! 
      }
   }

   #region VvDataRecord Propertiz Overriders

   public override IVvDao VvDao
   {
      get { return ZXC.ArtStatDao; }
   }

   public override string VirtualRecordName
   {
      get { return ArtStat.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return ArtStat.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "xy"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return DateTime.MinValue; } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return null; } }
   public override uint     VirtualLanSrvID{ get { return 0; } set {;} }
   public override uint     VirtualLanRecID{ get { return 0; } set {;} }



   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return ArtStat.sorterArtStat; }
   }

   public override bool IsArhivable  { get { return false; } }

   public override bool IsSifrar
   {
      get { return false; }
   }

   public override bool IsAutoSifra
   {
      get { return false; }
   }

   /// <summary>
   /// Za npr Sklad i Osred ovo je true, a za Person je false (overrajdano u Person.cs - u)
   /// </summary>
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

   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get { return false; }
   }

   public override bool IsCacheForStatus { get { return (true); } }

   public override string VirtualIDstring { get { return ""; } }

   #endregion VvDataRecord Propertiz Overriders

   #endregion Common

   #region DataLayer Propertiz

   #region Common propertiz

   public uint RecID
   {
      get { return this.currentData._recID; }
      set { this.currentData._recID = value; }
   }

   /* 01 */ public uint     RtransRecID 
   {
      get { return this.currentData._rtransRecID; }
      set {        this.currentData._rtransRecID = value; }
   }
   /* 02 */ public string   ArtiklCD    
   {
      get { return this.currentData._t_artiklCD; }
      set {        this.currentData._t_artiklCD = value; }
   }
   /* 03 */ public string   SkladCD     
   {
      get { return this.currentData._t_skladCD; }
      set {        this.currentData._t_skladCD = value; }
   }
   /* 04 */ public DateTime SkladDate   
   {
      get { return this.currentData._t_skladDate; }
      set {        this.currentData._t_skladDate = value; }
   }
   /* 05 */ public string   TT          
   {
      get { return this.currentData._t_tt; }
      set {        this.currentData._t_tt = value; }
   }
   /* 06 */ public Int16    TtSort      
   {
      get { return this.currentData._t_ttSort; }
      set {        this.currentData._t_ttSort = value; }
   }
   /* 07 */ public uint     TtNum       
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 08 */ public ushort   Serial      
   {
      get { return this.currentData._t_serial; }
      set {        this.currentData._t_serial = value; }
   }
   /* 09a */ public uint     TransRbr    
   {
      get { return this.currentData._transRbr; }
      set {        this.currentData._transRbr = value; }
   }

   /* 09b */ public uint RtransTwinRecID
   {
      get { return this.currentData._transRbr; }
      set {        this.currentData._transRbr = value; }
   }

   public string ArtiklTS
   {
      get { return this.currentData._artiklTS; }
      set { this.currentData._artiklTS = value; }
   }
   public string ArtiklJM
   {
      get { return this.currentData._artiklJM; }
      set { this.currentData._artiklJM = value; }
   }
   public DateTime AddTS
   {
      get { return this.currentData._addTS; }
      set { this.currentData._addTS = value; }
   }

   public string AddUID
   {
      get { return this.currentData._addUID; }
      set { this.currentData._addUID = value; }
   }

   public string  OrgPakJM { get { return this.currentData._orgPakJM; } set { this.currentData._orgPakJM = value; } }
   public decimal OrgPak   { get { return this.currentData._orgPak  ; } set { this.currentData._orgPak   = value; } }

   public string  ArtGrCd1 { get { return this.currentData._artGrCd1; } set { this.currentData._artGrCd1 = value; } }
   public string  ArtGrCd2 { get { return this.currentData._artGrCd2; } set { this.currentData._artGrCd2 = value; } }
   public string  ArtGrCd3 { get { return this.currentData._artGrCd3; } set { this.currentData._artGrCd3 = value; } }

   public decimal RtrPdvSt          { get { return this.currentData._rtrPdvSt     ; } set { this.currentData._rtrPdvSt      = value; } }
   public bool    RtrIsIrmUslug     { get { return this.currentData._rtrIsIrmUslug; } set { this.currentData._rtrIsIrmUslug = value; } }
   public uint    RtrParentID       { get { return this.currentData._rtrParentID  ; } set { this.currentData._rtrParentID   = value; } }

   #endregion Common propertiz

public decimal UkPstFinKNJ        { get { return (TtInfo.IsMalopTT ? UkPstFinMPC        : UkPstFinNBC       );                           } }
public decimal UkUlazFinKNJ       { get { return (TtInfo.IsMalopTT ? UkUlazFinMPC       : UkUlazFinNBC      );                           } }
public decimal UkUlazFinKNJAll    { get { return (TtInfo.IsMalopTT ? UkUlazFinMPCAll    : UkUlazFinNBCAll   );                           } }
public decimal UkIzlazFinKNJ      { get { return (TtInfo.IsMalopTT ? UkIzlazFinMPC      : UkIzlazFinNBC     );                           } }
public decimal UkUlazFirmaFinKNJ  { get { return (TtInfo.IsMalopTT ? UkUlazFirmaFinMPC  : UkUlazFirmaFinNBC );                           } }
public decimal UkIzlazFirmaFinKNJ { get { return (TtInfo.IsMalopTT ? UkIzlazFirmaFinMPC : UkIzlazFirmaFinNBC);                           } }
public decimal StanjeFinKNJ       { get { return (TtInfo.IsMalopTT ? StanjeFinMPC       : StanjeFinNBC      );                           } }
public decimal KnjigCij           { get { return (TtInfo.IsMalopTT ? MalopCij           : PrNabCij          );                           } }
public decimal LastKnjigCij       { get { return (TtInfo.IsMalopTT ? LastUlazMPC        : LastPrNabCij      );                           } }
                                                                                                                                         
public decimal UkPstFinNBC        { get { return this.currentData._pstFinNBC       ; } set { this.currentData._pstFinNBC        = value; } } 
public decimal UkUlazFinNBC       { get { return this.currentData._ulazFinNBC      ; } set { this.currentData._ulazFinNBC       = value; } } 
public decimal UkUlazFinNBCAll    { get { return UkPstFinNBC + UkUlazFinNBC        ;                                                     } }
public decimal UkIzlazFinNBC      { get { return this.currentData._izlazFinNBC     ; } set { this.currentData._izlazFinNBC      = value; } } 
public decimal UkUlazFirmaFinNBC  { get { return this.currentData._ulazFirmaFinNBC ; } set { this.currentData._ulazFirmaFinNBC  = value; } } 
public decimal UkIzlazFirmaFinNBC { get { return this.currentData._izlazFirmaFinNBC; } set { this.currentData._izlazFirmaFinNBC = value; } } 
public decimal StanjeFinNBC       { get { return (IsMinusOK ? 0 : UkPstFinNBC + UkUlazFinNBC - UkIzlazFinNBC);                           } } // <-------------- 
public decimal PrNabCij 
{ 
   get 
   {
      // 28.03.2017: !!! HUGE NEWS !!! BIG NEWS !!! 
      if(StanjeKol.IsNegative()) return LastPrNabCij;

      decimal prNabCij = ZXC.DivSafe(StanjeFinNBC, StanjeKol);
      return (IsMinusOK ? 0 : prNabCij.NotZero() ? prNabCij : LastPrNabCij); 
   } 
}                                                                                                              
public decimal LastPrNabCij       { get { return this.currentData._lastPrNabCij; } set { this.currentData._lastPrNabCij = value;         } } // ovo bi moglo biti result property da nije situacije kada ti PocetnoStanje (kol 0, ali cijena postoji) rjesava problem povrata kao prva transakcija u godini... 
                                                                                                                                         
public decimal UkPstFinMPC        { get { return this.currentData._pstFinMPC       ; } set { this.currentData._pstFinMPC        = value; } }
public decimal UkUlazFinMPC       { get { return this.currentData._ulazFinMPC      ; } set { this.currentData._ulazFinMPC       = value; } } 
public decimal UkUlazFinMPCAll    { get { return UkPstFinMPC + UkUlazFinMPC        ;                                                     } }
public decimal UkIzlazFinMPC      { get { return this.currentData._izlazFinMPC     ; } set { this.currentData._izlazFinMPC      = value; } } 
public decimal UkUlazFirmaFinMPC  { get { return this.currentData._ulazFirmaFinMPC ; } set { this.currentData._ulazFirmaFinMPC  = value; } } 
public decimal UkIzlazFirmaFinMPC { get { return this.currentData._izlazFirmaFinMPC; } set { this.currentData._izlazFirmaFinMPC = value; } } 
public decimal StanjeFinMPC       { get { return (IsMinusOK ? 0 : UkPstFinMPC + UkUlazFinMPC - UkIzlazFinMPC);                           } }
public decimal MalopCij     
{ 
   get 
   {
      decimal malopCij = ZXC.DivSafe(StanjeFinMPC, StanjeKol);
      // 18.12.2012: 
    //return (IsMinusOK ? 0 : malopCij.NotZero() ? malopCij : LastUlazMPC);
      return (IsMinusOK ? 0 :                                 LastUlazMPC); 
   } 
}
public decimal LastUlazMPC        { get { return this.currentData._lastMalopCij;     } set { this.currentData._lastMalopCij     = value; } } // ovo bi moglo biti result property da nije situacije kada ti PocetnoStanje (kol 0, ali cijena postoji) rjesava problem povrata kao prva transakcija u godini... 
public decimal PrevMalopCij       { get { return this.currentData._prevMalopCij;     } set { this.currentData._prevMalopCij     = value; } } // za this rtrans ali prije njegovog djelovanja kolika je bila zatecena MalopCij (do_cij_mal) 

public decimal UkIzlFinProdKCR    { get { return this.currentData._izlFinProdKCR;    } set { this.currentData._izlFinProdKCR    = value; } } // IZLAZ - suma financijska (po T_Cij_KCR)

public decimal StanjeKol          { get { return (IsMinusOK ? 0 : UkPstKol + UkUlazKol - UkIzlazKol);                                    } }
public decimal UkUlazKolAll       { get { return                  UkPstKol + UkUlazKol;                                                  } }
public decimal UkPstKol           { get { return this.currentData._pstKol;           } set { this.currentData._pstKol           = value; } } // Pocetno stanje - kolicinsko
public decimal UkUlazKol          { get { return this.currentData._ulazKol;          } set { this.currentData._ulazKol          = value; } } // ULAZ (bez Pst) - suma kolicinska
public decimal UkIzlazKol         { get { return this.currentData._izlazKol;         } set { this.currentData._izlazKol         = value; } } // IZLAZ - suma kolicinska - classic
public decimal UkUlazFirmaKol     { get { return this.currentData._ulazFirmaKol;     } set { this.currentData._ulazFirmaKol     = value; } } // S obzirom na cijelu Frimu - UkUlazKOL
public decimal UkIzlazFirmaKol    { get { return this.currentData._izlazFirmaKol;    } set { this.currentData._izlazFirmaKol    = value; } } // S obzirom na cijelu Frimu - UkIzlazKOL

public decimal StanjeKol2         { get { return (IsMinusOK ? 0 : UkPstKol2 + UkUlazKol2 - UkIzlazKol2);                                 } }
public decimal UkUlazKol2All      { get { return                  UkPstKol2 + UkUlazKol2;                                                } }
public decimal UkPstKol2          { get { return this.currentData._pstKol2;          } set { this.currentData._pstKol2          = value; } } // Pocetno stanje - kolicinsko
public decimal UkUlazKol2         { get { return this.currentData._ulazKol2;         } set { this.currentData._ulazKol2         = value; } } // ULAZ (bez Pst) - suma kolicinska
public decimal UkIzlazKol2        { get { return this.currentData._izlazKol2;        } set { this.currentData._izlazKol2        = value; } } // IZLAZ - suma kolicinska - classic
public decimal UkUlazFirmaKol2    { get { return this.currentData._ulazFirmaKol2;    } set { this.currentData._ulazFirmaKol2    = value; } } // S obzirom na cijelu Frimu - UkUlazKOL
public decimal UkIzlazFirmaKol2   { get { return this.currentData._izlazFirmaKol2;   } set { this.currentData._izlazFirmaKol2   = value; } } // S obzirom na cijelu Frimu - UkIzlazKOL

public decimal StanjKolFisycal    { get { return (IsMinusOK ? 0 : UkPstKol + UkUlazKolFisycal - UkIzlazKolFisycal);                      } }
public decimal UkUlazKolAllFisycal{ get { return                  UkPstKol + UkUlazKolFisycal;                                           } }
public decimal UkUlazKolFisycal   { get { return this.currentData._ulazKolFisycal;   } set { this.currentData._ulazKolFisycal   = value; } } // ULAZ - suma kolicinska - ALI SAMO PO SKLADISNIM dokumentima - FIZICKI
public decimal UkIzlazKolFisycal  { get { return this.currentData._izlazKolFisycal;  } set { this.currentData._izlazKolFisycal  = value; } } // IZLAZ - suma kolicinska - ALI SAMO PO SKLADISNIM dokumentima - FIZICKI

public decimal StanjeKolFree      { get { return (IsMinusOK ? 0 : /*StanjeKol*/StanjKolFisycal - UkStanjeKolRezerv);                     } }
public decimal UkRezervKolNaruc   { get { return this.currentData._rezervKolNaruc;   } set { this.currentData._rezervKolNaruc   = value; } } // For this artikl UKUPNA Rezervacija - NARUCENO
public decimal UkRezervKolIsporu  { get { return this.currentData._rezervKolIsporu;  } set { this.currentData._rezervKolIsporu  = value; } } // For this artikl UKUPNA Rezervacija - ISPORUCENO
public decimal UkStanjeKolRezerv  { get { return this.currentData._stanjeKolRezerv;  } set { this.currentData._stanjeKolRezerv  = value; } } // IZLAZ - suma REZERVACIJA kolicinska

public decimal InvKol             { get { return this.currentData._invKol;           } set { this.currentData._invKol           = value; } } // Inventurno stanje - kolicinsko
public decimal InvKol2            { get { return this.currentData._invKol2;          } set { this.currentData._invKol2          = value; } } // Inventurno stanje - kolicinsko
public decimal InvFinNBC          { get { return this.currentData._invFin;           } set { this.currentData._invFin           = value; } } // Inventurno stanje - financijsko
public decimal InvFinMPC          { get { return this.currentData._invFinMPC;        } set { this.currentData._invFinMPC        = value; } } // Inventurno stanje - financijsko
public decimal InvFinKNJ          { get { return (TtInfo.IsMalopTT ? InvFinMPC  : InvFinNBC); } }

public decimal InvKolDiff         { get { return this.currentData._invKolDiff   ;    } set { this.currentData._invKolDiff       = value; } } // news for 2015/2016 
public decimal InvKol2Diff        { get { return this.currentData._invKol2Diff  ;    } set { this.currentData._invKol2Diff      = value; } } // news for 2015/2016 
public decimal InvFinDiffNBC      { get { return this.currentData._invFinDiff   ;    } set { this.currentData._invFinDiff       = value; } } // news for 2015/2016 
public decimal InvFinDiffMPC      { get { return this.currentData._invFinDiffMPC;    } set { this.currentData._invFinDiffMPC    = value; } } // news for 2015/2016 
public decimal InvFinDiffKNJ      { get { return (TtInfo.IsMalopTT ? InvFinDiffMPC  : InvFinDiffNBC); } }


public decimal UlazCijMin         { get { return this.currentData._ulazCijMin;       } set { this.currentData._ulazCijMin       = value; } } // ULAZ - minimalna T_Cij_KCR
public decimal UlazCijMax         { get { return this.currentData._ulazCijMax;       } set { this.currentData._ulazCijMax       = value; } } // ULAZ - maximalna T_Cij_KCR
public decimal UlazCijLast        { get { return this.currentData._ulazCijLast;      } set { this.currentData._ulazCijLast      = value; } } // ULAZ - zadnja T_Cij_KCR
public decimal IzlazCijMin        { get { return this.currentData._izlazCijMin;      } set { this.currentData._izlazCijMin      = value; } } // IZLAZ - minimalna T_Cij_KCR
public decimal IzlazCijMax        { get { return this.currentData._izlazCijMax;      } set { this.currentData._izlazCijMax      = value; } } // IZLAZ - maximalna T_Cij_KCR
public decimal IzlazCijLast       { get { return this.currentData._izlazCijLast;     } set { this.currentData._izlazCijLast     = value; } } // IZLAZ - zadnja T_Cij_KCR

public decimal PreDefVpc1         { get { return this.currentData._preDefVpc1;       } set { this.currentData._preDefVpc1       = value; } } // PreDef - VPC1 - dolazi sa dokumenta 'PreDef'
public decimal PreDefVpc2         { get { return this.currentData._preDefVpc2;       } set { this.currentData._preDefVpc2       = value; } } // PreDef - VPC2 - dolazi sa dokumenta 'PreDef'
public decimal PreDefMpc1         { get { return this.currentData._preDefMpc1;       } set { this.currentData._preDefMpc1       = value; } } // PreDef - MPC1 - dolazi sa dokumenta 'PreDef'
public decimal PreDefDevc         { get { return this.currentData._preDefDevc;       } set { this.currentData._preDefDevc       = value; } } // PreDef - DEVC1 - dolazi sa dokumenta 'PreDef'
public decimal PreDefRbt1         { get { return this.currentData._preDefRbt1;       } set { this.currentData._preDefRbt1       = value; } } // PreDef - RBT1 - dolazi sa dokumenta 'PreDef'
public decimal PreDefRbt2         { get { return this.currentData._preDefRbt2;       } set { this.currentData._preDefRbt2       = value; } } // PreDef - RBT2 - dolazi sa dokumenta 'PreDef'
public decimal PreDefMinKol       { get { return this.currentData._preDefMinKol;     } set { this.currentData._preDefMinKol     = value; } } // PreDef - MinKol - dolazi sa dokumenta 'PreDef'
public decimal PreDefMarza        { get { return this.currentData._preDefMarza;      } set { this.currentData._preDefMarza      = value; } } // PreDef - Marza - dolazi sa dokumenta 'PreDef'

public string   FrsMinTt          { get { return this.currentData._frsMinTt;         } set { this.currentData._frsMinTt         = value; } }
public uint     FrsMinTtNum       { get { return this.currentData._frsMinTtNum;      } set { this.currentData._frsMinTtNum      = value; } }
public DateTime DateZadUlaz       { get { return this.currentData._dateZadUlaz;      } set { this.currentData._dateZadUlaz      = value; } }
public DateTime DateZadIzlaz      { get { return this.currentData._dateZadIzlaz;     } set { this.currentData._dateZadIzlaz     = value; } }
public DateTime DateZadPst        { get { return this.currentData._dateZadPst;       } set { this.currentData._dateZadPst       = value; } }
public DateTime DateZadInv        { get { return this.currentData._dateZadInv;       } set { this.currentData._dateZadInv       = value; } }

public decimal PrNBCBefThisUlaz  { get { return this.currentData._prNBCBefThisUlaz; } set { this.currentData._prNBCBefThisUlaz = value; } } // za NUP shadow rtrans 

   #endregion DataLayer Propertiz

   #region Robna Kartica propertiz

   public decimal RtrPstKol        { get { return this.currentData._rtrPstKol;      } set { this.currentData._rtrPstKol = value;      } } 
   public decimal RtrUlazKol       { get { return this.currentData._rtrUlazKol;     } set { this.currentData._rtrUlazKol = value;     } } 
   public decimal RtrUlazKolFisycal{ get { return this.currentData._rtrUlazKolFisycal;}set{ this.currentData._rtrUlazKolFisycal = value;}}
   public decimal RtrKolNaruceno   { get { return this.currentData._rtrKolNaruceno; } set { this.currentData._rtrKolNaruceno = value; } } 
   public decimal RtrKolIsporuceno { get { return this.currentData._rtrKolIsporuceno;}set { this.currentData._rtrKolIsporuceno = value;}} 
   public decimal RtrIzlazKolFisycal{get { return this.currentData._rtrIzlzKolFisycal;}set{ this.currentData._rtrIzlzKolFisycal = value;}}
   public decimal RtrUlazAllKol    { get { return RtrPstKol + RtrUlazKol;           }                                                   } 
   public decimal RtrIzlazKol      { get { return this.currentData._rtrIzlazKol;    } set { this.currentData._rtrIzlazKol = value;    } } 

   public decimal RtrPstKol2       { get { return this.currentData._rtrPstKol2;     } set { this.currentData._rtrPstKol2 = value;     } } 
   public decimal RtrUlazKol2      { get { return this.currentData._rtrUlazKol2;    } set { this.currentData._rtrUlazKol2 = value;    } } 
   public decimal RtrUlazAllKol2   { get { return RtrPstKol2 + RtrUlazKol2;         }                                                   } 
   public decimal RtrIzlazKol2     { get { return this.currentData._rtrIzlazKol2;   } set { this.currentData._rtrIzlazKol2 = value;   } } 

   public decimal RtrPstVrjNBC     { get { return this.currentData._rtrPstVrjNBC;   } set { this.currentData._rtrPstVrjNBC   = value; } } 
   public decimal RtrUlazVrjNBC    { get { return this.currentData._rtrUlazVrjNBC;  } set { this.currentData._rtrUlazVrjNBC  = value; } } 
   public decimal RtrUlazAllVrjNBC { get { return RtrPstVrjNBC + RtrUlazVrjNBC;                                                       } }
   public decimal RtrIzlazVrjNBC   { get { return this.currentData._rtrIzlazVrjNBC; } set { this.currentData._rtrIzlazVrjNBC = value; } } 
   public decimal RtrPstCijNBC     { get { return this.currentData._rtrPstCijNBC;   } set { this.currentData._rtrPstCijNBC   = value; } } 
   public decimal RtrUlazCijNBC    { get { return this.currentData._rtrUlazCijNBC;  } set { this.currentData._rtrUlazCijNBC  = value; } } 
   public decimal RtrIzlazCijNBC   { get { return this.currentData._rtrIzlazCijNBC; } set { this.currentData._rtrIzlazCijNBC = value; } }
   public decimal RtrCijenaNBC     { get { return this.currentData._rtrCijenaNBC  ; } set { this.currentData._rtrCijenaNBC   = value; } }

   public decimal RtrPstVrjMPC     { get { return this.currentData._rtrPstVrjMPC;   } set { this.currentData._rtrPstVrjMPC   = value; } } 
   public decimal RtrUlazVrjMPC    { get { return this.currentData._rtrUlazVrjMPC;  } set { this.currentData._rtrUlazVrjMPC  = value; } } 
   public decimal RtrUlazAllVrjMPC { get { return RtrPstVrjMPC + RtrUlazVrjMPC;                                                       } }
   public decimal RtrIzlazVrjMPC   { get { return this.currentData._rtrIzlazVrjMPC; } set { this.currentData._rtrIzlazVrjMPC = value; } } 
   public decimal RtrPstCijMPC     { get { return this.currentData._rtrPstCijMPC;   } set { this.currentData._rtrPstCijMPC   = value; } } 
   public decimal RtrUlazCijMPC    { get { return this.currentData._rtrUlazCijMPC;  } set { this.currentData._rtrUlazCijMPC  = value; } } 
   public decimal RtrIzlazCijMPC   { get { return this.currentData._rtrIzlazCijMPC; } set { this.currentData._rtrIzlazCijMPC = value; } }
   public decimal RtrCijenaMPC     { get { return this.currentData._rtrCijenaMPC;   } set { this.currentData._rtrCijenaMPC   = value; } }

   public decimal RtrPstVrjKNJ     { get { return (TtInfo.IsMalopTT ? RtrPstVrjMPC     : RtrPstVrjNBC)    ; } } 
   public decimal RtrUlazVrjKNJ    { get { return (TtInfo.IsMalopTT ? RtrUlazVrjMPC    : RtrUlazVrjNBC)   ; } }
   public decimal RtrUlazAllVrjKNJ { get { return (TtInfo.IsMalopTT ? RtrUlazAllVrjMPC : RtrUlazAllVrjNBC); } }
   public decimal RtrIzlazVrjKNJ   { get { return (TtInfo.IsMalopTT ? RtrIzlazVrjMPC   : RtrIzlazVrjNBC)  ; } } 
   public decimal RtrPstCijKNJ     { get { return (TtInfo.IsMalopTT ? RtrPstCijMPC     : RtrPstCijNBC)    ; } } 
   public decimal RtrUlazCijKNJ    { get { return (TtInfo.IsMalopTT ? RtrUlazCijMPC    : RtrUlazCijNBC)   ; } } 
   public decimal RtrIzlazCijKNJ   { get { return (TtInfo.IsMalopTT ? RtrIzlazCijMPC   : RtrIzlazCijNBC)  ; } }
   public decimal RtrCijenaKNJ     { get { return (TtInfo.IsMalopTT ? RtrCijenaMPC     : RtrCijenaNBC)    ; } }

   // 15.12.2017:
   public decimal Rtr_Ratio_Kol_Kol2         { get { return ZXC.DivSafe(RtrPstKol + RtrUlazKol + RtrIzlazKol, RtrPstKol2 + RtrUlazKol2 + RtrIzlazKol2); } }
   public decimal Rtr_UlazAll_Ratio_Kol_Kol2 { get { return ZXC.DivSafe(RtrPstKol + RtrUlazKol              , RtrPstKol2 + RtrUlazKol2               ); } }
   public decimal Rtr_Izlaz_Ratio_Kol_Kol2   { get { return ZXC.DivSafe(RtrIzlazKol                         , RtrIzlazKol2                           ); } }
   public decimal UlazAll_Ratio_Kol_Kol2     { get { return ZXC.DivSafe(UkUlazKolAll                        , UkUlazKol2All                          ); } }
   public decimal Izlaz_Ratio_Kol_Kol2       { get { return ZXC.DivSafe(UkIzlazKol                          , UkIzlazKol2                            ); } }

   #endregion Robna Kartica propertiez

   #region Izvedene (Expression) Propertiz

   public decimal PstCijProsKNJ     { get { return (TtInfo.IsMalopTT ? PstCijProsMPC  : PstCijProsNBC) ;                       } }
   public decimal UlazCijProsKNJ    { get { return (TtInfo.IsMalopTT ? UlazCijProsMPC : UlazCijProsNBC);                       } }
   public decimal IzlCijProsKNJ     { get { return (TtInfo.IsMalopTT ? IzlCijProsMPC  : IzlCijProsNBC) ;                       } }

   public decimal PstCijProsMPC     { get { return ZXC.DivSafe(UkPstFinMPC    , UkPstKol  );                                   } }
   public decimal UlazCijProsMPC    { get { return ZXC.DivSafe(UkUlazFinMPC   , UkUlazKol );                                   } }
   public decimal IzlCijProsMPC     { get { return ZXC.DivSafe(UkIzlazFinMPC  , UkIzlazKol);                                   } }
   public decimal PstCijProsNBC     { get { return ZXC.DivSafe(UkPstFinNBC    , UkPstKol  );                                   } }
   public decimal UlazCijProsNBC    { get { return ZXC.DivSafe(UkUlazFinNBC   , UkUlazKol );                                   } }
   public decimal IzlCijProsNBC     { get { return ZXC.DivSafe(UkIzlazFinNBC  , UkIzlazKol);                                   } }
   public decimal IzlProdCijPros    { get { return ZXC.DivSafe(UkIzlFinProdKCR, UkIzlazKol);                                   } }
   public decimal IzlazRUVIznos     { get { return UkIzlFinProdKCR - UkIzlazFinNBC;                                            } }
   public decimal IzlazRUVKoef      { get { return (IsMinusOK ? 1.00M : ZXC.DivSafe(IzlazRUVIznos, UkIzlazFinNBC));            } }
   public decimal IzlazRUVStopa     { get { try { return IzlazRUVKoef * 100.00M; } catch (OverflowException) { return 0.00M; } } }
   public decimal RucVpc1Iznos      { get { return (PreDefVpc1.NotZero() ? PreDefVpc1 - PrNabCij : 0.00M);                     } }
   public decimal RucVpc1Koef       { get { return (IsMinusOK ? 1 : ZXC.DivSafe(RucVpc1Iznos, PrNabCij));                      } }
   public decimal RucVpc1Stopa      { get { return RucVpc1Koef * 100.00M;                                                      } }
   public decimal PrNabCijPlusMarza { get { return (PrNabCij * (1.00M + PreDefMarza/100.00M)).Ron2();                          } }

   public decimal PrevKolStanje     { get { return (StanjeKol    - (RtrPstKol + RtrUlazKol - RtrIzlazKol )); /* jer je ili Ulaz ili Izlaz*/ } }
   public decimal PrevKolStanje2    { get { return (StanjeKol2   - (RtrPstKol2+ RtrUlazKol2- RtrIzlazKol2)); /* jer je ili Ulaz ili Izlaz*/ } }
 //public decimal DiffMalopCij      { get { return (RtrCijenaMPC - PrevMalopCij);  /* PrevMalopCij - zatecena do_cij_mal */    } }
   public decimal DiffMalopCij      { get { return (RtrCijenaMPC.Ron2() - PrevMalopCij.Ron2());                                } }
   public decimal NivelacUlazVrj    { get { return (PrevKolStanje * DiffMalopCij);                                             } }

   // 29.03.2016: !?!?!? obrni ova dva dole reda (remarckiraj prvi a odr drugi)
 //public decimal NivelacIzlazVrj { get { return                              (RtrIzlazKol * DiffMalopCij); } }
   public decimal NivelacIzlazVrj { get { return /*isusluga*/IsMinusOK ? 0M : (RtrIzlazKol * DiffMalopCij); } }
   
   public bool    IsMinusOK         { get { return Artikl.IsMinusOk(ArtiklTS); } } // Usluga, Prolazna stavka, Taksa 
   public bool    IsMinusNotOK      { get { return !IsMinusOK;                                                     } } // Roba, Materijal, Vlastiti Proizvod, Uzorak, Ambalaza, ... 
   // 14.6.2011. IsRuc4Usluga nije radio dobro na Fak2Nal kada je IFA u pitanju onda je po Rtrans+Artstat JOIN-u TheAsEx null (tj, nula jer je struktura) a TheAsEx.ArtiklTS = null
   // pa ovaj IsRuc4Usluga zakljuci da je false, a u biti svi artikli na IFA-i su usluge.
 //public bool    IsRuc4Usluga      { get { return IsMinusOK                    ; } } // Dakle izjednacavas a ubuduce mozda treba dodatne razluciti 
   public bool    IsRuc4Usluga      { get { return IsMinusOK || ArtiklTS == null; } } // Dakle izjednacavas a ubuduce mozda treba dodatne razluciti 
   public bool    IsKonto4Usluga    { get { return IsRuc4Usluga || Artikl.IsUslugaDP (ArtiklTS); } } // za Faktur2Nalog 
   public bool    IsKonto4UslugaDP  { get { return                 Artikl.IsUslugaDP (ArtiklTS); } } // za Faktur2Nalog 
   public bool    IsMaterOrPotros   { get { return             Artikl.IsMaterOrPotros(ArtiklTS); } } // za Faktur2Nalog 
   public bool    IsMaterijal       { get { return                 Artikl.IsMaterijal(ArtiklTS); } } // za Faktur2Nalog 
   public bool    IsSitniInv        { get { return                 Artikl.IsSitni    (ArtiklTS); } } // za Faktur2Nalog 
   public bool    IsAllSkladCD      { get { return SkladCD.IsEmpty(); } }
  public DateTime DateZadPromj      { get { return new DateTime[] { DateZadPst, DateZadUlaz, DateZadIzlaz, DateZadInv }.Max(); } }
   public bool    IsUslNoRuc        { get { return                 Artikl.IsUslNoRuc (ArtiklTS); } } // za R_Ira_PV_NoRuc 


   public bool IsWorthForPS         { get { return StanjeKol.NotZero() || KnjigCij.NotZero(); } }
   
 //public bool IsInKlipping         { get { return StanjeKol.IsZero() && StanjeFinKNJ.NotZero()            ; } }
   public bool IsInKlipping         { get { return StanjeKol.IsZero() && StanjeFinKNJ.AlmostZero() == false; } }

   public decimal InvDiff           { get { return (InvKol - StanjeKol ); } }
   public decimal InvDiff2          { get { return (InvKol2- StanjeKol2); } }

   // 01.01.2022:
   public bool Is_NOT_NultiZPC { get { return Faktur.Get_IsNultiZPC(TT, TtNum) == false; } }
   
   #endregion Izvedene (Expression) Propertiz

   #region Izvedene OrgPak Propertiz

   private decimal GetOP_KOL(decimal normalKOL) { return normalKOL * this.OrgPak;             }
   private decimal GetOP_CIJ(decimal normalCIJ) { return ZXC.DivSafe(normalCIJ, this.OrgPak); }

   public decimal KnjigCijOP       { get { return GetOP_CIJ(KnjigCij      ); } }
   public decimal LastKnjigCijOP   { get { return GetOP_CIJ(LastKnjigCij  ); } }
   public decimal PrNabCijOP       { get { return GetOP_CIJ(PrNabCij      ); } }
   public decimal LastPrNabCijOP   { get { return GetOP_CIJ(LastPrNabCij  ); } }
   public decimal MalopCijOP       { get { return GetOP_CIJ(MalopCij      ); } }
   public decimal LastUlazMPCOP    { get { return GetOP_CIJ(LastUlazMPC   ); } }
   public decimal PrevMalopCijOP   { get { return GetOP_CIJ(PrevMalopCij  ); } }
   public decimal UlazCijMinOP     { get { return GetOP_CIJ(UlazCijMin    ); } }
   public decimal UlazCijMaxOP     { get { return GetOP_CIJ(UlazCijMax    ); } }
   public decimal UlazCijLastOP    { get { return GetOP_CIJ(UlazCijLast   ); } }
   public decimal IzlazCijMinOP    { get { return GetOP_CIJ(IzlazCijMin   ); } }
   public decimal IzlazCijMaxOP    { get { return GetOP_CIJ(IzlazCijMax   ); } }
   public decimal IzlazCijLastOP   { get { return GetOP_CIJ(IzlazCijLast  ); } }
   public decimal RtrUlazCijKNJOP  { get { return GetOP_CIJ(RtrUlazCijKNJ ); } }
   public decimal RtrIzlazCijKNJOP { get { return GetOP_CIJ(RtrIzlazCijKNJ); } }
   public decimal RtrCijenaKNJOP   { get { return GetOP_CIJ(RtrCijenaKNJ  ); } }
   public decimal PstCijProsKNJOP  { get { return GetOP_CIJ(RtrIzlazCijKNJ); } }
   public decimal UlazCijProsKNJOP { get { return GetOP_CIJ(UlazCijProsKNJ); } }
   public decimal IzlCijProsKNJOP  { get { return GetOP_CIJ(IzlCijProsKNJ ); } }
   public decimal IzlProdCijProsOP { get { return GetOP_CIJ(IzlProdCijPros); } }
   public decimal RucVpc1IznosOP   { get { return GetOP_CIJ(RucVpc1Iznos  ); } }
   public decimal StanjeKolOP      { get { return GetOP_KOL(StanjeKol     ); } }

 //public decimal RtrUlazCijNBCOP  { get { return GetOP_CIJ(RtrUlazCijNBC ); } } upali ako zatreba 
 //public decimal RtrIzlazCijNBCOP { get { return GetOP_CIJ(RtrIzlazCijNBC); } }
 //public decimal RtrCijenaNBCOP   { get { return GetOP_CIJ(RtrCijenaNBC  ); } }
 //public decimal RtrUlazCijMPCOP  { get { return GetOP_CIJ(RtrUlazCijMPC ); } }
 //public decimal RtrIzlazCijMPCOP { get { return GetOP_CIJ(RtrIzlazCijMPC); } }
 //public decimal RtrCijenaMPCOP   { get { return GetOP_CIJ(RtrCijenaMPC  ); } }

   public decimal UkUlazKolAllOP   { get { return GetOP_KOL(UkUlazKolAll  ); } }
   public decimal UkPstKolOP       { get { return GetOP_KOL(UkPstKol      ); } }
   public decimal UkUlazKolOP      { get { return GetOP_KOL(UkUlazKol     ); } }
   public decimal UkIzlazKolOP     { get { return GetOP_KOL(UkIzlazKol    ); } }

   // 11.03.2019: SVD ORG ... koji nije isti kao kod Temba 
   private decimal GetBOP_KOL(decimal normalKOL) { return ZXC.DivSafe(normalKOL, this.OrgPak); }
   private decimal GetCOP_CIJ(decimal normalCIJ) { return normalCIJ * this.OrgPak;             }

   public decimal UkUlazKolBOP      { get { return GetBOP_KOL(UkUlazKol     ); } }
   public decimal UkIzlazKolBOP     { get { return GetBOP_KOL(UkIzlazKol    ); } }
   public decimal PrNabCijCOP       { get { return GetCOP_CIJ(PrNabCij      ); } }

   // HALMED u 2020: 
   public decimal HalmedORG         { get; set; }
   public decimal HalmedCIJ         { get { return ZXC.DivSafe(UkIzlazFinNBC, UkIzlazKol); } }
   public decimal HalmedCOP         { get { return             HalmedCIJ *    HalmedORG  ; } }
   public decimal HalmedBOP         { get { return ZXC.DivSafe(UkIzlazKol   , HalmedORG ); } }


   #endregion Izvedene OrgPak Propertiz

   #region Izvedene INVENTURA Propertiz

   public decimal InvKol_Visak_AFT    { get { return (InvKol           - StanjeKol          ).IsPositive() ?  (InvKol           - StanjeKol          ) : 0M; } }
   public decimal InvKol_Manjk_AFT    { get { return (InvKol           - StanjeKol          ).IsNegative() ? -(InvKol           - StanjeKol          ) : 0M; } }
 //public decimal InvFinNBC_Visak_AFT { get { return (InvFinNBC.Ron2() - StanjeFinNBC.Ron2()).IsPositive() ?  (InvFinNBC.Ron2() - StanjeFinNBC.Ron2()) : 0M; } }
 //public decimal InvFinNBC_Manjk_AFT { get { return (InvFinNBC.Ron2() - StanjeFinNBC.Ron2()).IsNegative() ? -(InvFinNBC.Ron2() - StanjeFinNBC.Ron2()) : 0M; } }
 //public decimal InvFinMPC_Visak_AFT { get { return (InvFinMPC.Ron2() - StanjeFinMPC.Ron2()).IsPositive() ?  (InvFinMPC.Ron2() - StanjeFinMPC.Ron2()) : 0M; } }
 //public decimal InvFinMPC_Manjk_AFT { get { return (InvFinMPC.Ron2() - StanjeFinMPC.Ron2()).IsNegative() ? -(InvFinMPC.Ron2() - StanjeFinMPC.Ron2()) : 0M; } }
   public decimal InvFinNBC_Visak_AFT { get { return((InvFinNBC        - StanjeFinNBC       ).IsPositive() ?  (InvFinNBC        - StanjeFinNBC       ) : 0M).ZeroIfAlmostZero();} }
   public decimal InvFinNBC_Manjk_AFT { get { return((InvFinNBC        - StanjeFinNBC       ).IsNegative() ? -(InvFinNBC        - StanjeFinNBC       ) : 0M).ZeroIfAlmostZero();} }
   public decimal InvFinMPC_Visak_AFT { get { return((InvFinMPC        - StanjeFinMPC       ).IsPositive() ?  (InvFinMPC        - StanjeFinMPC       ) : 0M).ZeroIfAlmostZero();} }
   public decimal InvFinMPC_Manjk_AFT { get { return((InvFinMPC        - StanjeFinMPC       ).IsNegative() ? -(InvFinMPC        - StanjeFinMPC       ) : 0M).ZeroIfAlmostZero();} }

   public decimal InvFinKNJ_Visak_AFT { get { return (TtInfo.IsMalopTT ? InvFinMPC_Visak_AFT : InvFinNBC_Visak_AFT); } }
   public decimal InvFinKNJ_Manjk_AFT { get { return (TtInfo.IsMalopTT ? InvFinMPC_Manjk_AFT : InvFinNBC_Manjk_AFT); } }

   public bool IsShadowInventura { get; set; } // artikl se nije pojavio na INM/INV dokumentu, a trebao je sa stanjem 0 

   public decimal InvKol_Visak_BEF    { get {                                            return (InvKolDiff)   .IsPositive() ?  (InvKolDiff)    : 0M; } }
   public decimal InvKol_Manjk_BEF    { get { if(IsShadowInventura) return StanjeKol;    return (InvKolDiff)   .IsNegative() ? -(InvKolDiff)    : 0M; } }
   public decimal InvFinNBC_Visak_BEF { get {                                            return (InvFinDiffNBC).IsPositive() ?  (InvFinDiffNBC) : 0M; } }
   public decimal InvFinNBC_Manjk_BEF { get { if(IsShadowInventura) return StanjeFinNBC; return (InvFinDiffNBC).IsNegative() ? -(InvFinDiffNBC) : 0M; } }
   public decimal InvFinMPC_Visak_BEF { get {                                            return (InvFinDiffMPC).IsPositive() ?  (InvFinDiffMPC) : 0M; } }
   public decimal InvFinMPC_Manjk_BEF { get { if(IsShadowInventura) return StanjeFinMPC; return (InvFinDiffMPC).IsNegative() ? -(InvFinDiffMPC) : 0M; } }

   public decimal InvFinKNJ_Visak_BEF { get { return (TtInfo.IsMalopTT ? InvFinMPC_Visak_BEF : InvFinNBC_Visak_BEF); } }
   public decimal InvFinKNJ_Manjk_BEF { get { return (TtInfo.IsMalopTT ? InvFinMPC_Manjk_BEF : InvFinNBC_Manjk_BEF); } }

   public decimal StanjeKol_INV       { get { return StanjeKol    - InvKolDiff   ; } }
   public decimal StanjeFinNBC_INV    { get { return StanjeFinNBC - InvFinDiffNBC; } }
   public decimal StanjeFinMPC_INV    { get { return StanjeFinMPC - InvFinDiffMPC; } }
   public decimal StanjeFinKNJ_INV    { get { return (TtInfo.IsMalopTT ? StanjeFinMPC_INV : StanjeFinNBC_INV); } }

   #endregion Izvedene INVENTURA Propertiz

   #region Weekly Average Needs

   public decimal WeeklyIzlazKol
   {
      get
      {
       //int daysToDate = SkladDate.DayOfYear;
         int daysToDate = DateTime.Today.DayOfYear;

         decimal weeks = daysToDate / 7M;

         return ZXC.DivSafe(UkIzlazKol, weeks);
      }
   }
   public decimal WeeklyDeviation { get { return StanjeKol - WeeklyIzlazKol; } }

   #endregion Weekly Average Needs

   public bool HasUselessPST  { get { return DateZadPst.NotEmpty() && UkPstKol.IsZero() && UkUlazKol.IsZero() && UkIzlazKol.IsZero(); } }

   #endregion propertiz --- !!! PAZI: Kada ovdje nesto mijenjas moras i u class 'Rtrans' i 'Artikl' !!!

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<ArtStatStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<ArtStatStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<ArtStatStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<ArtStatStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<ArtStatStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      ArtStat newObject = new ArtStat();

      Generic_CloneData<ArtStatStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public ArtStat MakeDeepCopy()
   {
      return (ArtStat)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new ArtStat();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((ArtStat)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((ArtStat)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory


   #region Metodz - Voila !!!

   /* $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ */
   /* $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ */
   /* $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ $$$$ */

   public bool SumFromRtrans(Rtrans rtr, string _artiklCD, string _skladCD, bool resloveMinusMode)
   {
      return SumFromRtrans(rtr, _artiklCD, _skladCD, resloveMinusMode, false);
   }

   private Artikl GetArtiklRec(string _artiklCD)
   {
      Artikl artikl_rec;

      if(_artiklCD.IsEmpty()) return new Artikl();

      try
      {
         artikl_rec = VvUserControl.ArtiklSifrar.Single(art => art.ArtiklCD == _artiklCD);
      }
      catch(Exception ex)
      {
         if(ZXC.OffixImport_InProgress == false)
         {
            if(ZXC.RenewCache_InProgress == false)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "TheVvUC.ArtiklSifrar (count {0}) ne sadrži artikl\n\n[{1}]\n\n{2}", VvUserControl.ArtiklSifrar.Count, _artiklCD, ex.Message);
            }
            else // RenewCache IS InProgress 
            {
               if(ZXC.ErrorsList == null) ZXC.ErrorsList = new List<string>();

               ZXC.ErrorsList.Add("ArtiklCD not found [" + _artiklCD + "]");
            }
         }
         artikl_rec = new Artikl();

         // Nema smisla pokusavati 'SetSifrarAndAutocomplete()' kada je pri dolasku ovamo vec otvoren reader od 'GetAndSetArtiklStatus()' 
         //try
         //{
         //   ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
         //   artikl_rec = ZXC.TheVvForm.TheVvUC.ArtiklSifrar.Single(art => art.ArtiklCD == _artiklCD);
         //}
         //catch(Exception)
         //{
         //   artikl_rec = new Artikl();
         //}
      }

      return artikl_rec;
   }

   public static ArtStat NullifyRtransValues(ArtStat a)
   {
         a.RtrPstCijNBC = a.RtrPstVrjNBC = a.RtrUlazCijNBC = a.RtrUlazVrjNBC     = a.RtrIzlazCijNBC     = a.RtrIzlazVrjNBC = a.RtrCijenaNBC     =
         a.RtrPstCijMPC = a.RtrPstVrjMPC = a.RtrUlazCijMPC = a.RtrUlazVrjMPC     = a.RtrIzlazCijMPC     = a.RtrIzlazVrjMPC = a.RtrCijenaMPC     =
         a.RtrIzlazKol  = a.RtrUlazKol   = a.RtrPstKol     = a.RtrUlazKolFisycal = a.RtrIzlazKolFisycal = a.RtrKolNaruceno = a.RtrKolIsporuceno =
         a.RtrIzlazKol2 = a.RtrUlazKol2  = a.RtrPstKol2    = 0.00M;

         return a;
   }

   /*private*/ public bool SumFromRtrans(Rtrans rtr, string _artiklCD, string _skladCD, bool isReslovingMinusInProgress, bool recalcAfterMinusForReportList)
   {
      #region SET IDENTITY of Rtrans sender

      /* 01 */ RtransRecID     = rtr.T_recID          ;
      /* 02 */ ArtiklCD        = /*rtrans.T*/_artiklCD;
      /* 03 */ SkladCD         = /*rtrans.T*/_skladCD ;
      /* 04 */ SkladDate       = rtr.T_skladDate      ;
      /* 05 */ TT              = rtr.T_TT             ;
      /* 06 */ TtSort          = rtr.T_ttSort         ;
      /* 07 */ TtNum           = rtr.T_ttNum          ;
      /* 08 */ Serial          = rtr.T_serial         ;
      // 04.07.2025: 
      // 09 */ TransRbr++                             ; // NOVO u 2025 ... za PTG XY2 rtrans.T_cij 
      /* 09 */ RtransTwinRecID = rtr.T_twinID         ; // NOVO u 2025 ... za PTG XY2 rtrans.T_cij 
      // 11.05.2015:                                   
               RtrParentID       = rtr.T_parentID     ;
               RtrPdvSt          = rtr.T_pdvSt        ;
               RtrIsIrmUslug     = rtr.T_isIrmUsluga  ;

    //RtrPstCijNBC = RtrPstVrjNBC = RtrUlazCijNBC = RtrUlazVrjNBC     = RtrIzlazCijNBC     = RtrIzlazVrjNBC = RtrCijenaNBC     =
    //RtrPstCijMPC = RtrPstVrjMPC = RtrUlazCijMPC = RtrUlazVrjMPC     = RtrIzlazCijMPC     = RtrIzlazVrjMPC = RtrCijenaMPC     =
    //RtrIzlazKol  = RtrUlazKol   = RtrPstKol     = RtrUlazKolFisycal = RtrIzlazKolFisycal = RtrKolNaruceno = RtrKolIsporuceno =
    //RtrIzlazKol2 = RtrUlazKol2  = RtrPstKol2                                                                                 = 0.00M;
               NullifyRtransValues(this);

      bool isInMinus = false;

      bool isUlazFromPrevYears  = ZXC.IsManyYearDB && TtInfo.IsFinKol_U && rtr.T_skladDate < ZXC.NowYearFirstDay;
      bool isIzlazFromPrevYears = ZXC.IsManyYearDB && TtInfo.IsFinKol_I && rtr.T_skladDate < ZXC.NowYearFirstDay;

      if(IsMinusNotOK && (isReslovingMinusInProgress || rtr.MinusStatus == ZXC.MinusTrouble.IN_TROUBLE) && FrsMinTt.IsEmpty() && FrsMinTtNum.IsZero())
      {
         FrsMinTt    = TT;
         FrsMinTtNum = TtNum;
      }

      #endregion SET IDENTITY of Rtrans sender

      #region Set Artikl's artikl_rec data

      Artikl artikl_rec = GetArtiklRec(_artiklCD);

      ArtiklTS = artikl_rec.TS;
      ArtiklJM = artikl_rec.JedMj;

      OrgPak   = artikl_rec.R_orgPak  ;
      OrgPakJM = artikl_rec.R_orgPakJM;

      // 17.05.2017: ugasio ovo sranje jer radi probleme onima koji zaista koriste OrgPak 

      // 16.10.2015: 4 debug purposes TH-u stavljamo                                               START 

      //             TtAndTtNumAndSerial rtransa koji je inicirao Delete_Then_Renew_Cache_FromThisRtrans 
      //             tj. podmecemo u artstat-ovu OrgPakJM verijablu                                      
      // 17.05.2017: ugasio                                                              OrgPakJM = RtransDao.FromThisRtrans_TtAndTtNum
    //if(ZXC.IsTEXTHOany || ZXC.CURR_prjkt_rec.KupdobCD == 1 || ZXC.ThisIsSkyLabProject) OrgPakJM = RtransDao.FromThisRtrans_TtAndTtNum;

      // 16.10.2015: END                                                                                 

      ArtGrCd1 = artikl_rec.Grupa1CD  ;
      ArtGrCd2 = artikl_rec.Grupa2CD  ;
      ArtGrCd3 = artikl_rec.Grupa3CD  ;

      #endregion Set Artikl's artikl_rec data

      #region POCETO STANJE

      // 24.01.2025: 
    //if(TtInfo.IsFinKol_PS                                               )
      if(TtInfo.IsFinKol_PS || isUlazFromPrevYears || isIzlazFromPrevYears)
      {
         if(isIzlazFromPrevYears) // !!! ************** !!! VOILA !!! 
         {
            rtr.T_kol *= -1;
            rtr.T_cij = LastPrNabCij;

            rtr.CalcTransResults(null);
         }

         // 09.12.2015: 
         bool isFKZ = this.ArtiklTS == Artikl.FinKorekArtTS; // financijska korekcija financijskog stanja skladista (zaliha) 
         
         // 15.07.2022: dodan if() tako da za CalcRtrans kolicina 1 izracuna r_KC, r_KCR, ... 
         // a da ne utjece na RtrPstKol                                                       
         if(!isFKZ)
         RtrPstKol  = rtr.T_kol ;
         UkPstKol  += RtrPstKol ;
         RtrPstKol2 = rtr.T_kol2;
         UkPstKol2 += RtrPstKol2;
         
         RtrPstCijNBC = rtr.T_cij    ; 
         RtrPstCijMPC = rtr.R_CIJ_MSK; 
         
         RtrCijenaNBC = RtrPstCijNBC;
         RtrCijenaMPC = RtrPstCijMPC;

         if(!isFKZ) // !!! normal     case 
         {
            RtrPstVrjNBC = rtr.R_KC;   
         }
         else // !!! korekturni case 
         {
            RtrPstVrjNBC = rtr.T_cij   ; //  ovako je kad korigiramo - direktan utjecaj na FinSt po NBC preko iznosa upisanog u polje t_cij (a moze biti poz. i negativan pa smanjujemo)
         }

         RtrPstVrjMPC = rtr.R_MSK;
         
         UkPstFinNBC += RtrPstVrjNBC;
         UkPstFinMPC += RtrPstVrjMPC;
         
         LastPrNabCij = /*PrNabCij*/RtrPstCijNBC;
         LastUlazMPC  = /*MalopCij*/RtrPstCijMPC;
         
         DateZadPst = rtr.T_skladDate;
         
         RtrUlazKolFisycal = rtr.T_kol;

         #region #if(DEBUG)
#if(DEBUG)
         //Console.WriteLine("{0,6:N} {1,6:N} {2,10:N} {3,6:N} {4,6:N} {5,10:N} {6,6:N} {7,6:N} {8,10:N}",
         //   RtrUlazKol, RtrCijena, RtrUlazVrjKNJ, RtrIzlazKol, RtrCijena, RtrIzlazVrjKNJ, StanjeKol, PrNabCij, StanjeFinKNJ);
#endif
         #endregion #if(DEBUG)
      }

      #endregion POCETO STANJE

      #region FIN KOL ULAZ

    //else if(TtInfo.IsKorekTemTT && TtInfo.IsFinKol_U)
      else if(TtInfo.IsKorekTemTT && TtInfo.IsFinKol_U && ZXC.IsVELEFORM == false)
      {
         RtrUlazKol     = rtr.T_kol    ;
         UkUlazKol     += RtrUlazKol   ;
       //RtrUlazVrjNBC  = rtr.R_KCR    ; ... ovako je kod normalnog ulaza 
         RtrUlazVrjNBC  = rtr.T_cij    ; //  ovako je kad korigiramo - direktan utjecaj na FinSt po NBC preko iznosa upisanog u polje t_cij (a moze biti poz. i negativan pa smanjujemo)
         UkUlazFinNBC  += RtrUlazVrjNBC;

      } // else if(TtInfo.IsKorekTemTT && TtInfo.IsFinKol_U)

      else if(TtInfo.IsFinKol_U)
      {
         PrNBCBefThisUlaz  = this.PrNabCij;

         #region IsStornoTT additions

         if(TtInfo.IsStornoTT && (StanjeKol - rtr.T_kol).IsNegative()) // Znaci, ovaj ce storno ulaza proizvesti minus! 
         {
            ZXC.TheVvForm.ReportArtiklMinusManager(this, false);

            // 13.04.2016: remarkirali jer TIME MACHINE - ABRAKAKOBREDABRA  manager
            // se ne snalazi kada sa ulaznim TT-om odes u minus,
            // nego moras paziti na cijenu koju ces zadati na tom stornu ulaza 
          //isInMinus = true;
         }

         #endregion IsStornoTT additions

         #region InternUlaz_From One or Many Izlaz

         // 24.10.2014: radeci na TT_TRI zakljucio da cijela ova regija ne radi nista korisno!?!? 
         // ...pa naknadno zakljucio da ipak DA, jer je if od 10cm nize else-a. Mozes mozda ostaviti prazan if
         if(
           (TtInfo.IsInternUlaz_FromOneIzlaz || TtInfo.IsInternUlaz_FromManyIzlaz) // TT_MS_UL, TT_PR_UL 
            && 
            TtInfo.IsManualPUcijTT == false)
         {
          //decimal linkedIzlazDokPrNabCij = 0M;

            // 24.10.2014: ova dva pedera ne rade nista korisno! 
            // sluze da u prvom prolsaku pri promjeni cijene poprave vezani artstat nekog npr. MSU-a, ALI NE POPRAVE I SAM rtrans 
            // buduci da vako i onako CheckPrNabCij mora odraditi svoje (popraqviti ce i artstat i rtrans) ovo bi samo nepotrebno usporavalo prvi prolazak 
          //if(    TtInfo.IsInternUlaz_FromManyIzlaz) linkedIzlazDokPrNabCij = ArtiklDao.GetPrNabCijFrom_Many_LinkedIzlazRtranses(rtr);
          //else /*TtInfo.IsInternUlaz_FromOneIzlaz*/ linkedIzlazDokPrNabCij = ArtiklDao.GetPrNabCijFrom_One_LinkedIzlazRtrans   (rtr);
          //
          //if(rtr.R_CIJ_KCR != linkedIzlazDokPrNabCij) ZXC.TheVvForm.ReportInternCijenaDiscrepancyManager(this, linkedIzlazDokPrNabCij, rtr.R_CIJ_KCR);
         
            // 11.11.2014: tek sad?! 
            RtrUlazCijNBC = rtr.R_CIJ_KCR /*= linkedIzlazDokPrNabCij*/;

            // 04.07.2025: 
            if(TtInfo.IsPTGTwinRtrans_UgAnDodTT) // UG2 
            {                                    // AU2 
                                                 // DI2 
                                                 // PV2 
                                                 // ZI2 
                                                 // ZU2 

             //decimal theCij = PrNabCij    ; // ILI ILI 
               decimal theCij = rtr.R_theVPC; // ILI ILI 

               if(theCij.NotZero())
               {
                  RtrUlazCijNBC = theCij;
               }

            } // if(TtInfo.IsPTGTwinRtrans_UgAnDodTT) // UG2 ... 

         } // MSU, PUX, XY2, ...

         #endregion InternUlaz_From One or Many Izlaz

         // ugaseno 14.10.2024. ... nez cem sluzi, smeta, valjda dio onog starog dizajna MOD-a 
         //else if(TtInfo.IsMODulazTT)
         //{
         //   RtrUlazCijNBC = PrNBCBefThisUlaz; // jer TT_MOU nema t_cij nit' na gridu nit' u dataLayer-u 
         //}
         else
         {
            RtrUlazCijNBC = rtr.R_CIJ_KCR; // ex 'nab'
         }

         #region 2015-16 INVENTURNA PRIMKA

         // 25.01.2023: zakljucili da na primjeru SvDUH-a ovo samo radi probleme i ugasili
         // ubuduce, ako treba prNabCij osvjeziti na PRI viska, napravi se to sa PrNabCij sub modul akcijom
         // a mozda u dogledno vrijeme uvedemo dedicirane TT-ove koji bi kao i MSI, ... bili podlozni ChkPrNabCij proceduri 

         bool isInventurnaPrimka = false;// (this.TT == Faktur.TT_PRI && this.SkladDate == this.DateZadInv);
       //bool isInventurnaPrimka = (this.TT == Faktur.TT_PRI && this.SkladDate == ZXC.Date30062022);

         // 23.01.2023: 
       //if(isInventurnaPrimka)
         if(isInventurnaPrimka && PrNabCij.NotZero())
         {
            RtrUlazCijNBC = PrNabCij;
#if DEBUG
            if(!ZXC.AlmostEqual(rtr.R_CIJ_KCR.Ron2(), PrNabCij.Ron2(), 0.02M)) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "isInventurnaPrimka R_CIJ_KCR {0} != PrNabCij {1}!\n\r\n\r{2}", rtr.R_CIJ_KCR.Ron2(), PrNabCij.Ron2(), this);
#endif
         }

#if DEBUG
         else if(isInventurnaPrimka)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Exclamation, "isInventurnaPrimka PrNabCij je NULA pa uzimam R_CIJ_KCR {1}!\n\r\n\r{0}", this, RtrUlazCijNBC);
         }
#endif

         #endregion 2015-16 INVENTURNA PRIMKA

         #region    STORNO ULAZA - POVRAT kao zadnja stavka kartice (KolSaldo ide na nulu - FinSaldo ne ode inace na nulu)

         bool isKolSt_ZERO_AfterPovrat = rtr.T_kol.IsNegative() && (StanjeKol + rtr.T_kol).IsZero();
#if DEBUG
         // DEBUG ONLY !!! +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
         
         bool povratCijNotEqualPrNabCij = rtr.R_CIJ_KCR.Ron2() != PrNabCij.Ron2();
         
         if(isKolSt_ZERO_AfterPovrat && povratCijNotEqualPrNabCij)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Exclamation, "KolSt je NULA nakon Ulaza sa negativnom količinom!\n\r\n\r{0}", this);
         }
         // DEBUG ONLY !!! +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
#endif
         // 23.01.2026: zbog problema kod DUCATI vis / manj procedure otkrivamo da je ovo do danas bilo ugaseno
         // pa ga ponovo palimo. zasto i kada je ugaseno nije jasno                                            
         if(  isKolSt_ZERO_AfterPovrat        )
       //if(/*isKolSt_ZERO_AfterPovrat*/ false)
         {
            RtrUlazCijNBC = this.PrNabCij;
         }

         #endregion STORNO ULAZA - POVRAT kao zadnja stavka kartice (FinSaldo ne ode inace na nulu)

         RtrUlazKol = TtInfo.IsStornoTT ? rtr.ForceNegative_T_kol : rtr.T_kol ; // IsStornoTT: Odobrenje od dobavljaca, povrat dobavljacu (cijena sa upisuje na odobrenju, 
                                                                                // a u slucaju povrata mora se zadati cijena sa UR-e . PAZI!!! ovaj kontra ulaz moze proizvesti minus na skladistu! Solve it in the future.
         RtrUlazKol2= TtInfo.IsStornoTT ? rtr.ForceNegative_T_kol2: rtr.T_kol2; 

         UkUlazKol  += RtrUlazKol ;
         UkUlazKol2 += RtrUlazKol2;

       //RtrUlazCijNBC = rtrans.R_CIJ_KCR ili linkedIzlazDokPrNabCij ... nastaje par redova gore;
         RtrUlazCijMPC = rtr.R_CIJ_MSK; // ex 'mal'

         RtrCijenaNBC = RtrUlazCijNBC;
         RtrCijenaMPC = RtrUlazCijMPC;

         // 04.07.2025: 
       //RtrUlazVrjNBC = rtr.R_KCR;
         RtrUlazVrjNBC = RtrUlazKol * RtrUlazCijNBC;
         RtrUlazVrjMPC = rtr.R_MSK;

         // 14.01.2019: isplivalo da kako gore u '#region STORNO ULAZA - POVRAT kao zadnja stavka kartice (FinSaldo ne ode inace na nulu)' 
         // popravljas RtrUlazCijNBC (a njome i RtrCijenaNBC) ALI JU DALJE NE KORISTIS!                                                    
         // RtrUlazVrjNBC i dalje punis preko rtr.R_CIJ_KCR ... pa treba i RtrUlazVrjNBC korigirati                                        

       //if(isInventurnaPrimka                            ) RtrUlazVrjNBC = rtr.T_kol * this.RtrUlazCijNBC; 
       //if(isInventurnaPrimka || isKolSt_ZERO_AfterPovrat) RtrUlazVrjNBC = rtr.T_kol * this.RtrUlazCijNBC; 
         if(isInventurnaPrimka && RtrUlazCijNBC.NotZero() ) RtrUlazVrjNBC = rtr.T_kol * this.RtrUlazCijNBC;

         UkUlazFinNBC += RtrUlazVrjNBC;
         UkUlazFinMPC += RtrUlazVrjMPC;

         bool firstUlazEver;

         LastPrNabCij = PrNabCij;

         // 21.01.2021: 100% rabat pattern kod SvDUH-a (npr. Ulaz - donacije) 
         if(ZXC.IsSvDUH && rtr.T_skladDate >= ZXC.Date01012021 && rtr.T_rbt1St.Ron2() == 100.00M)
         {
            LastPrNabCij = 0.00M;
         }

         if(LastUlazMPC.NotZero()) { firstUlazEver = false; PrevMalopCij = LastUlazMPC  ; }
         else                      { firstUlazEver = true ; PrevMalopCij = RtrUlazCijMPC; } // npr. prvi ulaz 
         LastUlazMPC = RtrUlazCijMPC;

         #region Explicitna Nivelacija

         // 01.01.2022:
       //if(TtInfo.IsNivelacijaZPC)
         if(TtInfo.IsNivelacijaZPC && Is_NOT_NultiZPC)
         {
            // ponisti KOL operacije
            UkUlazKol  -= RtrUlazKol;
            RtrUlazKol  = 0.00M;
            UkUlazKol2 -= RtrUlazKol2;
            RtrUlazKol2 = 0.00M;

            UkUlazFinNBC -= RtrUlazVrjNBC;
            LastPrNabCij = PrNabCij;

            // 06.09.2019: opaska glede NULTI_ZPC_iz2018_u2019_NEWS regije:                                                                 
            // regija je nastala 02.01.2019 pri observaciji da 'cij sa nultog ZPC-a ponekad smeta' pa smo ju pokusali ponistiti             
            // 10.01.2019 primijetismo da ipak treba kada nema PSM cijene                                                                   
            // 06.09.2019 primijetismo da pak ima i treci slucaj kojega je i tesko opisati / rjesiti pa smo odlucili                        
            // ugasiti novost zapocetu 02.01.2019 i nadati se da nas u 2020 nece smetati                                                    
            // !!! NOTA BENE !!!: VvSkyLab u trenutku pisanja ovog komentara niti NEMA ovu novost jer je zadnji put deployan u 05.2018! :-( 
            // pa je rezultat rebuildanja cache-a na poslovnici 'vako a na centrali 'nako                                                   

            // !!! 
            // !!! 
            // !!! 
            // Ovaj blok opaske dodan 31.12.2019. Nakon Sto je u novoj 2020 Chk0 LagerListe opet javio nebuloze;
            // Danas smo napokon nulirali [t_kol, t_cij, t_doCijMal, t_wanted = t_noCijMal]
            // na svim nultim ZPC-ovima, pa mozda to rjesi peripetiju kolizije PSM vs NultiZPC 
            // !!! 
            // !!! 
            // !!! 

            #region NULTI_ZPC_iz2018_u2019_NEWS

            //// 02.01.2019: TH: ukoliko NIJE na snazi nulti ZPC cijena ZPCa se posvadi s cijenom PSM-a pa ju treba ignorirati 
            //if(Faktur.Get_IsNultiZPC(TT, TtNum) && this.SkladDate == ZXC.projectYearFirstDay)
            //{
            //   // 10.01.2019: ali ovo pak smeta kada nema PSM-a za artikl, a trebamo nultiZPC cijenu 
            // //if(rtr.R_CIJ_MSK.NotZero()                           ) // ovime saznajemo da li ovaj nulti ZPC pokusava promijeniti cijenu 
            //   if(rtr.R_CIJ_MSK.NotZero() && PstCijProsMPC.NotZero()) // ovime saznajemo da li ovaj nulti ZPC pokusava promijeniti cijenu 
            //   {                                                      // R_CIJ_MSK = T_noCijMal - T_doCijMal                              
            //
            //      UkUlazFinMPC  -=  RtrUlazVrjMPC; // izbij iz fin ulaz kumulativa 
            //      RtrUlazVrjMPC  =             0M; // nuliraj iznos ovog fin ulaza 
            //      rtr.R_CIJ_MSK  =             0M; // nuliraj razliku cijena       
            //      rtr.T_noCijMal = rtr.T_doCijMal; // vrati doCijMal               
            //   }
            //}

            #endregion NULTI_ZPC_iz2018_u2019_NEWS

            //PrevMalopCij = LastUlazMPC;
            RtrCijenaMPC = 
            LastUlazMPC  = rtr.T_noCijMal;

            // 16.07.2015: 
            if(firstUlazEver) PrevMalopCij = rtr.T_doCijMal;
         }

         #endregion Explicitna Nivelacija

         if(TtInfo.IsAllSkladFinKol_U)
         {
            UkUlazFirmaKol    += RtrUlazKol ;
            UkUlazFirmaKol2   += RtrUlazKol2;
            UkUlazFirmaFinNBC += RtrUlazVrjNBC;
            UkUlazFirmaFinMPC += RtrUlazVrjMPC;
         }

         SetMinMaxLastCij_Ulaz(rtr.R_CIJ_KCR, rtr.T_skladDate);

         DateZadUlaz = rtr.T_skladDate;

         if(ZXC.CURR_prjkt_rec.UrSkipKolStSkl == false)
         {
            RtrUlazKolFisycal = rtr.T_kol;

            UkUlazKolFisycal += RtrUlazKol;
         }

         #region SHADOW Trans - Implicitna Nivelacija Malop Ulaza

         // 26.02.2013: ova potreba za VvLookUpListom RiskRulesDsc (ZXC.RRD) DIZE TheDbConnection EXCEPTION
         // kada ide RenewCache iz nekog razloga. Do dalnjega stopamo provjeru 'Dsc_IsSupressSHADOWing' 
       //if(/*ZXC.RRD.Dsc_IsSupressSHADOWing == false &&*/ TtInfo.HasShadowTT) // TODO: VMI/VMU zasada nije ShadowTT sto pase kod 'PreTank', al kod klasicne medjuskladisnice bi trebalo ipak nivelirati. Rjesenje: treba novi TT
         // 01.01.2022: 
       //if(  ZXC.RRD.Dsc_IsSupressSHADOWing == false &&   TtInfo.HasShadowTT                   ) // ipak vratio za probu ... i skuzio gdje je prije bio bug.
         if(  ZXC.RRD.Dsc_IsSupressSHADOWing == false &&   TtInfo.HasShadowTT && Is_NOT_NultiZPC) // ipak vratio za probu ... i skuzio gdje je prije bio bug.
         {
            // ... just to remember ...
            // PrevKolStanje   = (StanjeKol     - (RtrPstKol + RtrUlazKol - RtrIzlazKol)); 
            // DiffMalopCij    = (RtrCijenaMPC  - PrevMalopCij                          ); 
            // NivelacUlazVrj  = (PrevKolStanje * DiffMalopCij                          ); 
            // NivelacIzlacVrj = (RtrIzlazKol   * DiffMalopCij                          ); 

            UkUlazFinMPC += NivelacUlazVrj;
         }

         #endregion SHADOW Trans - Implicitna Nivelacija

         #region SHADOW Trans - Implicitna Nivelacija Velep Ulaznog Povrata

         // 01.02.2023: big news ... a 06.06.2023 dorađeno                              
       //if(rtr.Is_URA_wMinusKol                        )                               
       //if(rtr.Is_URA_wMinusKol && StanjeKol.IsZero()  ) // NUP_DILEMA! 1. od 2 dileme 
         if(rtr.Is_URA_wMinusKol                        ) 
         {
            bool isThisDirektStornoRtrans = Get_isThisDirektStornoRtrans(ArtiklCD, SkladCD,  rtr);

            // Observacija od 24.05.2024: ova ovdje NUP logika se razlikuje od NUP logike u 'GetArtstat_SUM_list_Command' 
            // pa je moguca pojava crvene brojke razlike u Bilanci skladista kada 'isThisDirektStornoRtrans' postoji      

            if(isThisDirektStornoRtrans == false) // NUP-aj samo ako ovo nije storno neposredno prethodnog redka robne kartice (npr. SVD odmah stornira krivu URA-u) 
                                                  // ali ako i je storno ali naknadni (nepriljubljeni redci) onda ipak NUP-aj                                        
            { 
               decimal DiffPovratCij     = RtrUlazCijNBC - PrNBCBefThisUlaz;
             //decimal NivelacUlazPovVrj = RtrUlazKol    * DiffPovratCij;
               decimal NivelacUlazPovVrj = PrevKolStanje * DiffPovratCij;
               UkUlazFinNBC             += NivelacUlazPovVrj;
            }
         }

         #endregion SHADOW Trans - Implicitna Nivelacija Velep Ulaznog Povrata

         #region MOU ... vrednuj FIN po PrNBCBefThisUlaz

         // ugaseno 14.10.2024. ... nez cem sluzi, smeta, valjda dio onog starog dizajna MOD-a 
       //if(TtInfo.IsMODulazTT)
       //{
       //   UkUlazFinNBC      -=             RtrUlazVrjNBC   ;
       //   UkUlazFinNBC      += rtr.T_kol * PrNBCBefThisUlaz;
       //   UkUlazFirmaFinNBC -=             RtrUlazVrjNBC   ;
       //   UkUlazFirmaFinNBC += rtr.T_kol * PrNBCBefThisUlaz;
       //   RtrUlazVrjNBC      = rtr.T_kol * RtrUlazCijNBC   ;
       //
       //   LastPrNabCij       = /*PrNabCij*/ZXC.DivSafe(StanjeFinNBC, StanjeKol);
       //}

         #endregion MOU ... vrednuj FIN po PrNBCBefThisUlaz

         #region #if(DEBUG)
#if(DEBUG)
         //Console.WriteLine("{0,6:N} {1,6:N} {2,10:N} {3,6:N} {4,6:N} {5,10:N} {6,6:N} {7,6:N} {8,10:N}",
         //   RtrUlazKol, RtrCijena, RtrUlazVrjKNJ, RtrIzlazKol, RtrCijena, RtrIzlazVrjKNJ, StanjeKol, PrNabCij, StanjeFinKNJ);

         //bool nijeSveNaNuli = StanjeKol.IsZero() && (!StanjeFinNBC.AlmostZero() /*|| (isMalopSkl && !artikl.AS_StanjeFinMPC.AlmostZero())*/);
         //if(nijeSveNaNuli)
         //{
         // //if(isMalopSkl) ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{4}]\n\nArtikl [{0}] na sklad [{3}]\n\nima količinsko stanje nula a financijsko različito od nule!\n\nFinSt NBC: {1}\n\nFinSt MPC: {2}", artikl.ArtiklCD, artikl.AS_StanjeFinNBC.ToStringVv(), artikl.AS_StanjeFinMPC.ToStringVv(), artikl.AS_SkladCD, artikl.TheAsEx.ToString());
         // /*else*/         ZXC.aim_emsg(MessageBoxIcon.Error, "On artstat [{4}]\n\nArtikl [{0}] na sklad [{3}]\n\nima količinsko stanje nula a financijsko različito od nule!\n\nFinSt NBC: {1}", ArtiklCD, StanjeFinNBC.ToStringVv(), StanjeFinMPC.ToStringVv(), SkladCD, this.ToString());
         //}

#endif
         #endregion #if(DEBUG)

      } // else if(TtInfo.IsFinKol_U) 

      #endregion FIN KOL ULAZ

      #region FIN KOL IZLAZ

      else if(TtInfo.IsFinKol_I)
      {
         #region Going in minus

         if(TtInfo.IsStornoTT == false && ((StanjeKol - rtr.T_kol).IsNegative() || StanjKolFisycal.IsNegative())) // Znaci, ovaj ce izlaz proizvesti minus! (a ne provjeravaj ako je storno, jer to je u biti ulaz na skladiste a ne izlaz)
         {
            if(isReslovingMinusInProgress == false) ZXC.TheVvForm.ReportArtiklMinusManager(this, false);
            if(recalcAfterMinusForReportList                   ||
            //rtrans.MinusStatus == ZXC.MinusTrouble.IN_TROUBLE  ||
               rtr.MinusStatus == ZXC.MinusTrouble.REPAIRED)
            {
               isInMinus = true; // if recalcAfterMinusForReportList is in progress or irreparable, fall down for recalculation 
            }
            else
            {
               /*isInMinus = true;*/ // For classic, break after found minus 
               return true;
            } 
         } // Odosmo u MINUS 

         //else                      { IsInMinus = false;             }

         #endregion Going in minus

         #region MinusStatus is REPAIRED or IN_TROUBLE

         bool isKolSt_ZERO_AfterPovrat = rtr.T_kol.IsNegative() && (StanjeKol - rtr.T_kol).IsZero();

         if(rtr.MinusStatus == ZXC.MinusTrouble.REPAIRED)
         {
            RtrIzlazCijNBC = rtr.TmpDecimal;
            RtrIzlazCijMPC = rtr.TmpDecimal2;
         }
         else if(recalcAfterMinusForReportList)
         {
            if(rtr.MinusStatus == ZXC.MinusTrouble.IN_TROUBLE)
            {
               RtrIzlazCijNBC = LastPrNabCij;
               RtrIzlazCijMPC = LastUlazMPC ;

               // 14.01.2019: vidi primjer ABRA - ABRAKAKOBREDABRA.Ripley.VIPER1.2019 ... 
               // kada je kao storno IZD izlazak iz minusa i zadnji redak kartice,        
               // a da bi dosli na nulu                                             START 
               if(isKolSt_ZERO_AfterPovrat) // storno izlaza kao zadnji redak kartice koji nas iz minusa dize na nulu, 'zamjena artikala' 
               {
                  RtrIzlazCijNBC = ZXC.DivSafe(StanjeFinNBC, StanjeKol);
               }
               // a da bi dosli na nulu                                              END  
            }
            else
            {
               RtrIzlazCijNBC = LastPrNabCij ;
               RtrIzlazCijMPC = rtr.R_CIJ_MSK;
            }
         }

         #endregion MinusStatus is REPAIRED or IN_TROUBLE

         else // Normal case ############################################################################## 
         {
            RtrIzlazCijMPC = rtr.R_CIJ_MSK; // R_CIJ_MSK JE T_cij <ALFA> 
          //RtrIzlazCijMPC = LastMalopCij;  //                    <BETA> 
            RtrIzlazCijNBC = PrNabCij     ; // ovo mora biti prvi redak u ovom if-u da pokupi zatecene 'FinSt' i 'KolSt' tj. prije ove promjene  

            if(RtrIzlazCijNBC.IsZero()) // Za povrat kupca, povrat po reversu i storno izlaznog racuna ako je stanje nula (tada je i PrNabCij nula       
            {                                                     // Tada se koristi sacuvana zadnje koristena PrNabCij jer je to legalna operacija (povrat kada je lager na nuli) 
               RtrIzlazCijNBC = LastPrNabCij;                        // 18.11.2011: besmisleno je navoditi sto sve moze biti 'IsPrNcPrm_noSt' pa ide uvijek kad treba...
               // 1.2.2011: tu gore ti je duplirana logika jer si danas odlucio uvijek podmetati helpPrNabCij ukoliko je klasika na nuli (vidi property 'PrNabCij')
            }
            if(RtrIzlazCijMPC.IsZero()) 
            {                           
               // 24.93.2014: 
             //RtrIzlazCijMPC = LastUlazMPC;
               // ... Ak je NULA, onda je NULA na MP dokumentu. Ne smije se izmisljati wantedMPC! 
            }
         }

         RtrIzlazKol  = TtInfo.IsStornoTT ? rtr.ForceNegative_T_kol  : rtr.T_kol ; // IsStornoTT: Odobrenje kupcu, povrat od kupca (cijena ide PrNabCij) 
         RtrIzlazKol2 = TtInfo.IsStornoTT ? rtr.ForceNegative_T_kol2 : rtr.T_kol2; // IsStornoTT: Odobrenje kupcu, povrat od kupca (cijena ide PrNabCij) 

         UkIzlazKol  += RtrIzlazKol ;
         UkIzlazKol2 += RtrIzlazKol2;

         RtrCijenaNBC = RtrIzlazCijNBC;
         RtrCijenaMPC = RtrIzlazCijMPC;

         if(TtInfo.IsInternIzlaz && rtr.R_CIJ_KCR != PrNabCij) ZXC.TheVvForm.ReportInternCijenaDiscrepancyManager(this, PrNabCij, rtr.R_CIJ_KCR);

         RtrIzlazVrjNBC = RtrIzlazKol * RtrIzlazCijNBC;
         RtrIzlazVrjMPC = RtrIzlazKol * RtrIzlazCijMPC;

         UkIzlazFinNBC    += RtrIzlazVrjNBC;
         UkIzlazFinMPC    += RtrIzlazVrjMPC;

         UkIzlFinProdKCR  += rtr.R_KCR;

         if(TtInfo.IsAllSkladFinKol_I)
         {
            UkIzlazFirmaKol    += RtrIzlazKol ;
            UkIzlazFirmaKol2   += RtrIzlazKol2;
            UkIzlazFirmaFinNBC += RtrIzlazVrjNBC;
            UkIzlazFirmaFinMPC += RtrIzlazVrjMPC;
         }

         // 06.10.2011: remarckirao. mozda da mozda ne?!
         LastPrNabCij = RtrIzlazCijNBC;
         PrevMalopCij = LastUlazMPC;
       //LastUlazMPC  = RtrIzlazCijMPC;

         SetMinMaxLastCij_Izlaz(rtr.R_CIJ_KCR, rtr.T_skladDate);

         DateZadIzlaz = rtr.T_skladDate;

         if(ZXC.CURR_prjkt_rec.IrSkipKolStSkl == false)
         {
            RtrIzlazKolFisycal = rtr.T_kol;

            UkIzlazKolFisycal += RtrIzlazKol;
         }

         #region SHADOW Trans - Implicitna Nivelacija

         if(ZXC.RRD.Dsc_IsSupressSHADOWing == false && TtInfo.HasShadowTT) // TODO: VMI/VMU zasada nije ShadowTT sto pase kod 'PreTank', al kod klasicne medjuskladisnice bi trebalo ipak nivelirati. Rjesenje: treba novi TT
         {
            // ... just to remember ...
            // PrevKolStanje   = (StanjeKol     - (RtrPstKol + RtrUlazKol - RtrIzlazKol)); 
            // DiffMalopCij    = (RtrCijenaMPC  - PrevMalopCij                          ); 
            // NivelacUlazVrj  = (PrevKolStanje * DiffMalopCij                          ); 
            // NivelacIzlacVrj = (RtrIzlazKol   * DiffMalopCij                          ); 

            bool shouldSKIP = false;
            // 08.01.2015: dodano: 1. shouldSKIP varijabla, 2. if(shouldSKIP) NEMOJ 'UkIzlazFinMPC -= NivelacIzlazVrj;'
            // a isti takav if postoji i u Reports_RIZ: CreateRtransShadow kod robne kartice!!! 
            if((PrevKolStanje.Ron2().IsZero() && MalopCij.Ron2().IsZero()) ||
                DiffMalopCij.Ron2().IsZero()) shouldSKIP = true; // ili je prevKol nula ili je novaCijena = staraCijena 

          //                        UkIzlazFinMPC -= NivelacIzlazVrj;
            if(shouldSKIP == false) UkIzlazFinMPC -= NivelacIzlazVrj;
         }

         #endregion SHADOW Trans - Implicitna Nivelacija

         #region #if(DEBUG)
#if(DEBUG)
         //         Console.WriteLine("{0,6:N} {1,6:N} {2,10:N} {3,6:N} {4,6:N} {5,10:N} {6,6:N} {7,6:N} {8,10:N}",
//            RtrUlazKol, RtrCijena, RtrUlazVrjKNJ, RtrIzlazKol, RtrCijena, RtrIzlazVrjKNJ, StanjeKol, PrNabCij, StanjeFinKNJ);
#endif
         #endregion #if(DEBUG)
      }

      #endregion FIN KOL IZLAZ

      #region SKL KOL (Fisycal) ULAZ & IZLAZ

      else if(TtInfo.IsKolOnly_U) // TT_SKU, TT_RVU (SklOnlyPrimka, Reversi - Povrat)
      {
         RtrUlazKolFisycal = rtr.T_kol;

         UkUlazKolFisycal += RtrUlazKolFisycal;
      }

      else if(TtInfo.IsKolOnly_I) // TT_SKI, TT_RVI (SklOnlyIzdatnica, Reversi - Izlaz)
      {
         //int t;
         //if(_artiklCD == "EM869AA-Pliva") t = 9;
         //// 4.2.2011: start ____________________________________________________________________________________________________________________________________________ 
         //if(TtInfo.IsStornoTT == false && ((StanjKolFisycal - rtrans.T_kol).IsNegative() || StanjeKol.IsNegative())) // Znaci, ovaj ce izlaz proizvesti minus! (a ne provjeravaj ako je storno, jer to je u biti ulaz na skladiste a ne izlaz)
         //{
         //   if(isReslovingMinusInProgress == false) ZXC.TheVvForm.ReportArtiklMinusManager(this, false);

         //   if(recalcAfterMinusForReportList || isIrreparableMinus) isInMinus = true; // if recalcAfterMinusForReportList is in progress or irreparable, fall down for recalculation 
         //   else                                                         return true; // For classic, break after found minus 
         //}
         //// 4.2.2011: end ______________________________________________________________________________________________________________________________________________ 

         RtrIzlazKolFisycal = rtr.T_kol;

         UkIzlazKolFisycal += RtrIzlazKolFisycal;

      }

      #endregion SKL KOL ULAZ & IZLAZ

      #region REZERVACIJE

      else if(TtInfo.IsRezervKol)
      {
         RtrKolNaruceno   = rtr.T_kol ;
         RtrKolIsporuceno = rtr.T_kol2;

         UkRezervKolNaruc  += RtrKolNaruceno  ;
         UkRezervKolIsporu += RtrKolIsporuceno;

         UkStanjeKolRezerv += rtr.T_kol - rtr.T_kol2;
      }

      #endregion REZERVACIJE

      #region INVENTURA

      else if(TtInfo.IsInventura)
      {
         //public decimal InvKol_Visak_AFT { get { return (InvKol - StanjeKol   ).IsPositive() ?  (InvKol - StanjeKol   ) : 0M; } }
         //public decimal InvKol_Manjk_AFT { get { return (InvKol - StanjeKol   ).IsNegative() ? -(InvKol - StanjeKol   ) : 0M; } }
         //public decimal InvFin_Visak_AFT { get { return (InvFin - StanjeFinKNJ).IsPositive() ?  (InvFin - StanjeFinKNJ) : 0M; } }
         //public decimal InvFin_Manjk_AFT { get { return (InvFin - StanjeFinKNJ).IsNegative() ? -(InvFin - StanjeFinKNJ) : 0M; } }

         //public decimal InvKol_Visak_BEF { get { return (InvKolDiff).IsPositive() ?  (InvKolDiff) : 0M; } }
         //public decimal InvKol_Manjk_BEF { get { return (InvKolDiff).IsNegative() ? -(InvKolDiff) : 0M; } }
         //public decimal InvFin_Visak_BEF { get { return (InvFinDiff).IsPositive() ?  (InvFinDiff) : 0M; } }
         //public decimal InvFin_Manjk_BEF { get { return (InvFinDiff).IsNegative() ? -(InvFinDiff) : 0M; } }

         //public decimal StanjeKol_INV { get { return StanjeKol    - InvKolDiff; } }
         //public decimal StanjeFin_INV { get { return StanjeFinKNJ - InvFinDiff; } }

         // 19.06.2023: dodajemo mogucnost kumulativne inventure ali NA ISTI DATUM 
         if(DateZadInv == rtr.T_skladDate) // ovo je, dakle, druga ili treca ili ... inventura na isti datum 
         {
            InvKol  += rtr.T_kol ; // NotBene: JE kumulativ 
            InvKol2 += rtr.T_kol2; // NotBene: JE kumulativ 

            // 27.12.2024: ovo je bio BUG!!!
          //InvFinNBC += InvKol    * this.PrNabCij; // NotBene: JE kumulativ 
          //InvFinMPC += InvKol    * this.MalopCij; // NotBene: JE kumulativ 
            InvFinNBC += rtr.T_kol * this.PrNabCij; // NotBene: JE kumulativ 
            InvFinMPC += rtr.T_kol * this.MalopCij; // NotBene: JE kumulativ 
         }
         else // classic, NIJE kumulativ (ovako je bilo do 19.06.2023)
         { 
            InvKol  /*+*/= rtr.T_kol ; // NotBene: nije kumulativ 
            InvKol2 /*+*/= rtr.T_kol2; // NotBene: nije kumulativ 

            InvFinNBC /*+*/= InvKol * this.PrNabCij; // NotBene: nije kumulativ 
            InvFinMPC /*+*/= InvKol * this.MalopCij; // NotBene: nije kumulativ 
         }

         // 10.01.2017: 
       //if(ZXC.AlmostEqual(StanjeFinNBC, InvFinNBC, 0.0555M)) InvFinNBC = StanjeFinNBC;
       //if(ZXC.AlmostEqual(StanjeFinMPC, InvFinMPC, 0.0555M)) InvFinMPC = StanjeFinMPC;
         if(StanjeKol == InvKol && ZXC.AlmostEqual(StanjeFinNBC, InvFinNBC, 0.7555M)) InvFinNBC = StanjeFinNBC;
         if(StanjeKol == InvKol && ZXC.AlmostEqual(StanjeFinMPC, InvFinMPC, 0.7555M)) InvFinMPC = StanjeFinMPC;

         DateZadInv = rtr.T_skladDate;

         // 18.12.2015: novi sistem inventurnih podataka, tako da se uvijek mogu rekonstruirati inventurne razlike i nakon izrade 'VisManj' operacije/dokumenata 
         InvKolDiff     = InvKol           - StanjeKol          ;
         InvKol2Diff    = InvKol2          - StanjeKol2         ;
         InvFinDiffNBC  = InvFinNBC.Ron2() - StanjeFinNBC.Ron2();
         InvFinDiffMPC  = InvFinMPC.Ron2() - StanjeFinMPC.Ron2();

         // 02.01.2017: 
         // 10.01.2017: ipak NE 
       //if(InvFinDiffNBC.NotZero() && Math.Abs(InvFinDiffNBC) < 0.0555M) InvFinDiffNBC = 0.00M;
       //if(InvFinDiffMPC.NotZero() && Math.Abs(InvFinDiffMPC) < 0.0555M) InvFinDiffMPC = 0.00M;

         // !!! PAZI !!! 
         // ako ovdje nesto korigiras, moras i u
         // Reports_RIZ: CheckInvProblems() 
      }

      #endregion INVENTURA

      #region PreDefined Values (VPC, MPC, RBT, ...)

      else if(TT == Faktur.TT_CJ_VP1) PreDefVpc1   = rtr.T_cij;
      else if(TT == Faktur.TT_CJ_VP2) PreDefVpc2   = rtr.T_cij;
      else if(TT == Faktur.TT_CJ_MP)  PreDefMpc1   = rtr.T_cij;
      else if(TT == Faktur.TT_CJ_DE)  PreDefDevc   = rtr.T_cij;
      else if(TT == Faktur.TT_CJ_MK)  PreDefMinKol = rtr.T_kol; 
      else if(TT == Faktur.TT_CJ_RB1){PreDefRbt1   = rtr.T_rbt1St; PreDefRbt2 = rtr.T_rbt2St; }
      else if(TT == Faktur.TT_CJ_RB2) PreDefRbt2   = rtr.T_rbt2St; 
      else if(TT == Faktur.TT_CJ_MRZ) PreDefMarza  = rtr.T_wanted; 

      #endregion PreDefined Values (VPC, MPC, RBT, ...)

      return isInMinus;

   } // public bool SumFromRtrans(Rtrans rtr, string _artiklCD, string _skladCD, bool isReslovingMinusInProgress, bool recalcAfterMinusForReportList) 

   /*private*/
   public static bool Get_isThisDirektStornoRtrans(string artiklCD, string skladCD, Rtrans forThisRtrans_rec)
   {
      Rtrans prevRtrans_rec = FakturDao.SetMePreviousRtransForArtiklRobnaKarticaRtrans(ZXC.TheThirdDbConn_SameDB, artiklCD, skladCD, forThisRtrans_rec);

      if(prevRtrans_rec == null) return false;

      if(forThisRtrans_rec.T_TT != prevRtrans_rec.T_TT) return false;

      prevRtrans_rec.CalcTransResults(null);

      if(ZXC.AlmostEqual(-1.00M * forThisRtrans_rec.R_KCRP, prevRtrans_rec.R_KCRP, 0.009M)) return true; // DA. ovo je direktni storno. kcrp-ovi se poništavaju 

      return false; 
   }

   #region Small UTIL Methodz

   private void SetMinMaxLastCij_Ulaz(decimal cijena, DateTime date)
   {
      if(cijena.IsZero()) return;

      if(cijena < UlazCijMin || UlazCijMin.IsZero()) UlazCijMin  = cijena;
      if(cijena > UlazCijMax || UlazCijMax.IsZero()) UlazCijMax  = cijena;
                                                     UlazCijLast = cijena;
   }

   private void SetMinMaxLastCij_Izlaz(decimal cijena, DateTime date)
   {
      if(cijena.IsZero()) return;

      if(cijena < IzlazCijMin || IzlazCijMin.IsZero()) IzlazCijMin  = cijena;
      if(cijena > IzlazCijMax || IzlazCijMax.IsZero()) IzlazCijMax  = cijena;
                                                       IzlazCijLast = cijena;
   }

   #endregion Small UTIL Methodz

   #endregion Metodz

   #region IComparable<ArtStat> Members

   // (t_artiklCD, t_skladDate, t_skladCD,   t_ttSort, t_ttNum, t_serial) NE!
   // (t_artiklCD, t_skladCD,   t_skladDate, t_ttSort, t_ttNum, t_serial) 

   // NEMOJ ovdje koristiti overridani Equals(), jer onda sjebes nesto za InvokeTransRemove!
   private bool ThisIsEqualTo(ArtStat other)
   {
      if(other == null) return false;

      return 
         (
            ArtiklCD  == other.ArtiklCD &&
            SkladDate == other.SkladDate  &&
            SkladCD   == other.SkladCD  &&
            TtSort    == other.TtSort   &&
            TtNum     == other.TtNum    &&
            Serial    == other.Serial
         );
   }

   public int CompareTo(ArtStat other)
   {
           if(ThisIsEqualTo  (other)) return  0;
      else if(ThisIsGreatThen(other)) return  1;
      else if(ThisIsLessThen (other)) return -1;

      else throw new Exception("ArtStat.CompareTo BUMMER!");
   }

   //private bool ORIGThisIsGreatThen(ArtStat other)
   //{
   //   if
   //   (
   //   (ArtiklCD.CompareTo(other.ArtiklCD)  > 0) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate  > other.SkladDate) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD)  > 0) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort  > other.TtSort) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort == other.TtSort  && TtNum  > other.TtNum) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort == other.TtSort  && TtNum == other.TtNum  && Serial > other.Serial)
   //   )    return true ;
   //   else return false;
   //}

   private bool ThisIsGreatThen(ArtStat other)
   {
      if
      (
      (ArtiklCD.CompareTo(other.ArtiklCD)  > 0) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD)  > 0)||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0 && SkladDate > other.SkladDate) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0 && SkladDate == other.SkladDate && TtSort > other.TtSort) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0 && SkladDate == other.SkladDate && TtSort == other.TtSort && TtNum > other.TtNum) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0 && SkladDate == other.SkladDate && TtSort == other.TtSort && TtNum == other.TtNum && Serial > other.Serial)
      )    return true ;
      else return false;
   }

   //private bool ORIGThisIsLessThen(ArtStat other)
   //{
   //   if
   //   (
   //   (ArtiklCD.CompareTo(other.ArtiklCD)  < 0) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate  < other.SkladDate) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD)  < 0) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort  < other.TtSort) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort == other.TtSort  && TtNum  < other.TtNum) ||
   //   (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladDate == other.SkladDate  && SkladCD.CompareTo(other.SkladCD) == 0  && TtSort == other.TtSort  && TtNum == other.TtNum  && Serial < other.Serial)
   //   )    return true ;
   //   else return false;
   //}

   private bool ThisIsLessThen(ArtStat other)
   {
      if
      (
      (ArtiklCD.CompareTo(other.ArtiklCD)  < 0) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD)  < 0) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0  && SkladDate  < other.SkladDate) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0  && SkladDate == other.SkladDate  && TtSort  < other.TtSort) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0  && SkladDate == other.SkladDate  && TtSort == other.TtSort  && TtNum  < other.TtNum) ||
      (ArtiklCD.CompareTo(other.ArtiklCD) == 0  && SkladCD.CompareTo(other.SkladCD) == 0  && SkladDate == other.SkladDate  && TtSort == other.TtSort  && TtNum == other.TtNum  && Serial < other.Serial)
      )    return true ;
      else return false;
   }

   #endregion

   #region IVvExtenderDataRecord Members

   public string ParentTableName
   {
      get { return Rtrans.recordName; }
   }

   public string JoinedColName
   {
      get { return "rtransRecID"; }
   }

   public uint ParentRecID
   {
      get { return RtransRecID; }
      set {        RtransRecID = value; }
   }

   #endregion

}
