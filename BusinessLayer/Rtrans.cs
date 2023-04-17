using System;
using System.Linq;
using System.Collections.Generic;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.Reflection;

#region struct RtransStruct

public struct RtransStruct
{
             [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
             /*internal*/ public uint      _recID       ;
   /* 01 */  /*internal*/ public uint      _t_parentID  ;
   /* 02 */  /*internal*/ public uint      _t_dokNum    ;
   /* 03 */  /*internal*/ public ushort    _t_serial    ;
   /* 04 */  /*internal*/ public DateTime  _t_skladDate ;
   /* 05 */  /*internal*/ public string    _t_tt        ;
   /* 06 */  /*internal*/ public uint      _t_ttNum     ;
   /* 07 */  /*internal*/ public Int16     _t_ttSort    ;
   /* 08 */  /*internal*/ public string    _t_artiklCD  ;
   /* 09 */  /*internal*/ public string    _t_skladCD   ;
   /* 10 */  /*internal*/ public string    _t_artiklName;
   /* 11 */  /*internal*/ public uint      _t_kupdob_cd ;
   /* 12 */  /*internal*/ public string    _t_jedMj     ;
   /* 13 */  /*internal*/ public string    _t_konto     ;
   /* 14 */  /*internal*/ public decimal   _t_kol       ;   
   /* 15 */  /*internal*/ public decimal   _t_cij       ;   
   /* 16 */  /*internal*/ public decimal   _t_pdvSt     ;   
   /* 17 */  /*internal*/ public decimal   _t_rbt1St    ;   
   /* 18 */  /*internal*/ public decimal   _t_rbt2St    ;   
   /* 19 */  /*internal*/ public decimal   _t_wanted    ;   
   /* 20 */  /*internal*/ public decimal   _t_doCijMal  ;   
   /* 21 */  /*internal*/ public decimal   _t_noCijMal  ;   
   /* 22 */  /*internal*/ public uint      _t_twinID    ;
   /* 23 */  /*internal*/ public ushort    _t_pdvKolTip ;
   /* 24 */  /*internal*/ public decimal   _t_ztr       ;   
   /* 25 */  /*internal*/ public decimal   _t_kol2      ;   
   /* 26 */  /*internal*/ public ushort    _t_mCalcKind ;
   /* 27 */  /*internal*/ public uint      _t_mtros_cd  ;
   /* 28 */  /*internal*/ public bool      _t_isIrmUslug;
   /* 29 */  /*internal*/ public decimal   _t_ppmvOsn   ;   
   /* 30 */  /*internal*/ public decimal   _t_ppmvSt1i2 ;   
   /* 31 */  /*internal*/ public decimal   _t_pnpSt     ;   
   /* 32 */  /*internal*/ public string    _t_serlot    ; // serNo, lotNo, atest, ... 

             //internal RtransResultStruct _ptrResult;
}

public struct RtransResultStruct
{

   internal decimal _r_KC      ;
   internal decimal _r_rbt1    ; 
   internal decimal _r_rbt2    ; 
   internal decimal _r_KCR     ; 
   internal decimal _r_mrz     ; 
   internal decimal _r_KCRM    ;
   internal decimal _r_pdv     ;
   internal decimal _r_pnp     ;
   internal decimal _r_pnpOsn  ;
   internal decimal _r_KCRP    ;
   internal decimal _r_cij_KCR ;
   internal decimal _r_cij_KCRM;
   internal decimal _r_cij_KCRP;
   internal decimal _r_cij_MSK ;
   internal decimal _r_mrzSt   ;

   internal decimal _r_rbtPdv;

   internal decimal _r_mskPdv;
   internal decimal _r_mskPnp;
   internal decimal _r_mskPdv10;
   internal decimal _r_mskPdv23;
   internal decimal _r_mskPdv05;
   internal decimal _r_mskPdv25;
   internal decimal _r_MSK;
   internal decimal _r_MSK_00;
   internal decimal _r_MSK_10;
   internal decimal _r_MSK_23;
   internal decimal _r_MSK_05;
   internal decimal _r_MSK_25;
   internal decimal _r_KCR_usl;
   internal decimal _r_KCRP_usl;
   // TODO: zapisnik o prom cijena results 
   // TODO: zavisni troskovi... 
   internal decimal _r_pdv_jed;

   internal decimal _rkiz_KC      ;
   internal decimal _rkiz_rbt1    ; 
   internal decimal _rkiz_KCR     ; 
}

#endregion struct RtransStruct

public class Rtrans : VvTransRecord, IComparable<Rtrans>, IVvExtendableDataRecord
{

   #region Fildz

   public const string recordName       = "rtrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   /*private*/ public RtransStruct currentData;
   /*private*/ public RtransStruct backupData;

   /*private*/ public RtransResultStruct _rtrResults;

   // spec za rtrans: 
   protected static System.Data.DataTable TheSchemaTable = ZXC.RtransDao.TheSchemaTable;
   protected static RtransDao.RtransCI    CI             = ZXC.RtransDao.CI;

   public static string artiklOrderBy_ASC  = "t_skladDate ASC, t_ttSort ASC, t_ttNum ASC, t_serial ASC ";
   public static string artiklOrderBy_DESC = artiklOrderBy_ASC.Replace("ASC", "DESC");

   #endregion Fildz

   #region Constructors

   public Rtrans() : this(0)
   {
   }

   public Rtrans(uint ID) : base()
   {
      this.currentData = new RtransStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;

      // well, svi reference types (string, date, ...)

      /* 01 */   this.currentData._t_parentID  = 0;
      /* 02 */   this.currentData._t_dokNum    = 0;
      /* 03 */   this.currentData._t_serial    = 0;
      /* 04 */   this.currentData._t_skladDate = DateTime.MinValue;
      /* 05 */   this.currentData._t_tt        = "";
      /* 06 */   this.currentData._t_ttNum     = 0;
      /* 07 */   this.currentData._t_ttSort    = 0;
      /* 08 */   this.currentData._t_artiklCD  = "";
      /* 09 */   this.currentData._t_skladCD   = "";
      /* 10 */   this.currentData._t_artiklName= "";
      /* 11 */   this.currentData._t_kupdob_cd = 0;
      /* 12 */   this.currentData._t_jedMj     = "";
      /* 13 */   this.currentData._t_konto     = "";
      /* 14 */   this.currentData._t_kol       = decimal.Zero;   
      /* 15 */   this.currentData._t_cij       = decimal.Zero;   
      /* 16 */   this.currentData._t_pdvSt     = decimal.Zero;   
      /* 17 */   this.currentData._t_rbt1St    = decimal.Zero;   
      /* 18 */   this.currentData._t_rbt2St    = decimal.Zero;   
      /* 19 */   this.currentData._t_wanted     = decimal.Zero;   
      /* 20 */   this.currentData._t_doCijMal  = decimal.Zero;   
      /* 21 */   this.currentData._t_noCijMal  = decimal.Zero;   
      /* 22 */   this.currentData._t_twinID    = 0;
      /* 23 */   this.currentData._t_pdvKolTip = 0;
      /* 24 */   this.currentData._t_ztr       = decimal.Zero;   
      /* 25 */   this.currentData._t_kol2      = decimal.Zero;
      /* 26 */   this.currentData._t_mCalcKind = 0;
      /* 27 */   this.currentData._t_mtros_cd  = 0;
      /* 28 */   this.currentData._t_isIrmUslug= false;
      /* 29 */   this.currentData._t_ppmvOsn   = decimal.Zero;   
      /* 30 */   this.currentData._t_ppmvSt1i2 = decimal.Zero;   
      /* 31 */   this.currentData._t_pnpSt     = decimal.Zero;   
      /* 32 */   this.currentData._t_serlot    = "";

      this.R_grName     = "";
      this.R_kupdobName = "";

                 this.TheAsEx = new ArtStat();
   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterArtiklCD = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_artiklCD  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_artiklName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladCD   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum     ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial    ]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "ArtiklCD", VvSQL.SorterType.Code, false);

   public static VvSQL.RecordSorter sorterArtiklName = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_artiklName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladCD   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum     ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial    ]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "ArtiklName", VvSQL.SorterType.Name, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial   ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer ] , true)
      }, "TtNum", VvSQL.SorterType.TtNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial   ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer ] , true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterKpdbName = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_kupdobCD ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort   ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial   ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer     ] , true)
      }, "Partner", VvSQL.SorterType.KpdbName, false);

   public static VvSQL.RecordSorter sorterSerlot = new VvSQL.RecordSorter(Rtrans.recordName, Rtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serlot    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_artiklName]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_skladDate ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttSort    ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_ttNum     ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_serial    ]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer      ] , true)
      }, "SerLot", VvSQL.SorterType.Serlot, false);


   // 16.03.2016: 
 //private VvSQL.RecordSorter[] _sorters = null;
   private VvSQL.RecordSorter[] _sorters =
      new  VvSQL.RecordSorter[]
      {   
         sorterSerlot    ,
         sorterArtiklCD  ,
         sorterArtiklName,
         sorterTtNum     ,
         sorterDokDate   ,
         sorterKpdbName
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   // 16.03.2016: 
 //public override object[] SorterCurrVal(VvSQL.SorterType sortType)
 //{
 //   throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet.");
 //}
   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Code    : return new object[] { T_artiklCD  , T_skladDate, T_ttSort, T_ttNum, T_serial };
         case VvSQL.SorterType.Name    : return new object[] { T_artiklName, T_skladDate, T_ttSort, T_ttNum, T_serial };
         case VvSQL.SorterType.TtNum   : return new object[] {                            T_ttSort, T_ttNum, T_serial };
         case VvSQL.SorterType.DokDate : return new object[] {               T_skladDate, T_ttSort, T_ttNum, T_serial };
         case VvSQL.SorterType.KpdbName: return new object[] { T_kupdobCD  , T_skladDate, T_ttSort, T_ttNum, T_serial };
         case VvSQL.SorterType.Serlot  : return new object[] { T_serlot    , T_skladDate, T_ttSort, T_ttNum, T_serial };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      //get { return Atrans.sorter_Person_DokDate_DokNum; }
      get 
      {
         return Rtrans.sorterArtiklCD;
         //throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region Propertiz 

   #region General Propertiz

   internal RtransStruct CurrentData // cijela RtransStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   //internal RtransStruct BackupData // SetTwinTransRec2 
   //{
   //   get { return this.backupData; }
   //   set {        this.backupData = value; }
   //}

   public override IVvDao VvDao
   {
      get { return ZXC.RtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Rtrans.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Rtrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Faktur.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "tr"; }
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

   public static string ArtiklForeignKey
   {
      get { return "t_artiklCD"; }
   }

   public static string FakturForeignKey
   {
      get { return "t_parentID"; }
   }

 //public TtInfo TtInfo { get { try { return                        ZXC.RiskTT[this.T_TT]               ; } catch(Exception) { return new TtInfo(); } } }
   public TtInfo TtInfo { get { try { return this.T_TT.NotEmpty() ? ZXC.RiskTT[this.T_TT] : new TtInfo(); } catch(Exception) { return new TtInfo(); } } }


   public override bool IsTwinTT
   {
      get
      {
         return this.TtInfo.HasTwinTT;
      }
   }

   public override bool IsSplitTT
   {
      get
      {
         return this.TtInfo.HasSplitTT;
      }
   }

   public override bool IsProzivodnjaUlazTT
   {
      get
      {
         // 29.03.2013: 
       //return this.TtInfo.Is_PUL_TT;
         return this.T_TT == Faktur.TT_PUL ||
                this.T_TT == Faktur.TT_PUX;
      }
   }

   public override bool IsCacheable 
   { 
      get 
      {
         return this.TtInfo.IsArtiklStatusInfluencer;
      } 
   }

   public override bool IsExtendable4Read
   {
      get
      {
         return this.TtInfo.IsIRArucableTT || // za RUC          vrijednosti 
                this.TtInfo.HasShadowTT      ; // za Nivelacijske vrijednosti 
         //return false;
      }
   }

   public ArtStat TheAsEx { get; set; }

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

   public override bool IsGlassOnIRM { get { return (this.T_TT == Faktur.TT_IRM && this.T_pdvColTip == ZXC.PdvKolTipEnum.GlassOnIRM); } }

   //public bool IsAlwaysRefreshData
   //{
   //   get
   //   {
   //      return this.IsTwinTT || this.IsIRMwGlassAction;
   //   }
   //}

   #endregion General Propertiz

   #region DataLayer Propertiez

   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public uint T_recID
   {
      get { return this.currentData._recID; }
      set {        this.currentData._recID = value; }
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

   #region Some Backup Values (For DeleteFromCache)

   public string     T_BCKPskladCD   { get { return this.backupData._t_skladCD  ; } set { this.backupData._t_skladCD   = value; } }
   public string     T_BCKPartiklCD  { get { return this.backupData._t_artiklCD ; } set { this.backupData._t_artiklCD  = value; } }
   public DateTime   T_BCKPskladDate { get { return this.backupData._t_skladDate; } set { this.backupData._t_skladDate = value; } }
   public Int16      T_BCKPttSort    { get { return this.backupData._t_ttSort   ; } set { this.backupData._t_ttSort    = value; } }
   public uint       T_BCKPttNum     { get { return this.backupData._t_ttNum    ; } set { this.backupData._t_ttNum     = value; } }
   public ushort     T_BCKPserial    { get { return this.backupData._t_serial   ; } set { this.backupData._t_serial    = value; } }

   public decimal    T_BCKPkol       { get { return this.backupData._t_kol ; } set { this.backupData._t_kol = value; } } // set komponenta tek od 2023 za CheckForMinus23 !? 
   public decimal    T_BCKPkol2      { get { return this.backupData._t_kol2; } }

   #endregion Some Backup Values (For DeleteFromCache)

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
   /* 04 */ public DateTime T_skladDate
   {
      get { return this.currentData._t_skladDate; }
      set {        this.currentData._t_skladDate = value; }
   } 
   /* 05 */ public string T_TT
   {
      get { return this.currentData._t_tt; }
      set {        this.currentData._t_tt = value; }
   }
   /* 06 */ public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public short T_ttSort
   {
      get { return this.currentData._t_ttSort; }
      set {        this.currentData._t_ttSort = value; }
   }
   /* 08 */ public string T_artiklCD
   {
      get { return this.currentData._t_artiklCD; }
      set {        this.currentData._t_artiklCD = value; }
   }
   /* 09 */ public string T_skladCD
   {
      get { return this.currentData._t_skladCD; }
      set {        this.currentData._t_skladCD = value; }
   }
   /* 10 */ public string T_artiklName
   {
      get { return this.currentData._t_artiklName; }
      set {        this.currentData._t_artiklName = value; }
   }
   /* 11 */ public uint T_kupdobCD
   {
      get { return this.currentData._t_kupdob_cd; }
      set {        this.currentData._t_kupdob_cd = value; }
   }
   /* 12 */ public string T_jedMj
   {
      get { return this.currentData._t_jedMj; }
      set {        this.currentData._t_jedMj = value; }
   }
   /* 13 */ public string T_konto
   {
      get { return this.currentData._t_konto; }
      set {        this.currentData._t_konto = value; }
   }
   /* 14 */ public decimal T_kol
   {
      get { return this.currentData._t_kol; }
      set {        this.currentData._t_kol = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 15 */ public decimal T_cij
   {
      get { return this.currentData._t_cij; }
      set {        this.currentData._t_cij = value; }
   }
   /* 16 */ public decimal T_pdvSt
   {
      get { return this.currentData._t_pdvSt; }
      set {        this.currentData._t_pdvSt = value; }
   }
   /* 17 */ public decimal T_rbt1St
   {
      get { return this.currentData._t_rbt1St; }
      set {        this.currentData._t_rbt1St = value; }
   }
   /* 18 */ public decimal T_rbt2St
   {
      get { return this.currentData._t_rbt2St; }
      set {        this.currentData._t_rbt2St = value; }
   }
   /// <summary>
   /// Ovo je: ili T_wantedMrzSt, ili T_wantedVPC, ili T_wantedMPC
   /// </summary>
   /* 19 */
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal T_wanted
   {
      get { return this.currentData._t_wanted; }
      set {        this.currentData._t_wanted = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 20 */ public decimal T_doCijMal
   {
      get { return this.currentData._t_doCijMal; }
      set {        this.currentData._t_doCijMal = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 21 */ public decimal T_noCijMal
   {
      get { return this.currentData._t_noCijMal; }
      set {        this.currentData._t_noCijMal = value; }
   }
   /* 22 */ public uint T_twinID
   {
      get { return this.currentData._t_twinID; }
      set {        this.currentData._t_twinID = value; }
   }

   /* 23 */ public ZXC.PdvKolTipEnum T_pdvColTip
   {
      get { return (ZXC.PdvKolTipEnum)this.currentData._t_pdvKolTip; }
      set {                           this.currentData._t_pdvKolTip =(ushort) value; }
   }
   ///*    */ public ushort T_pdvColTip_u
   //{
   //   get { return                    this.currentData._t_pdvKolTip; }
   //   set {                           this.currentData._t_pdvKolTip =         value; }
   //}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 24 */ public decimal T_ztr
   {
      get { return this.currentData._t_ztr; }
      set {        this.currentData._t_ztr = value; }
   }
   /* 25 */ public decimal T_kol2
   {
      get { return this.currentData._t_kol2; }
      set {        this.currentData._t_kol2 = value; }
   }
   /* 26 */ public ZXC.MalopCalcKind T_mCalcKind
   {
      get { return (ZXC.MalopCalcKind)this.currentData._t_mCalcKind; }
      set {                           this.currentData._t_mCalcKind = (ushort)value; }
   }

   /* 27 */ public uint T_mtrosCD
   {
      get { return this.currentData._t_mtros_cd; }
      set {        this.currentData._t_mtros_cd = value; }
   }

   /* 28 */ public bool T_isIrmUsluga
   {
      get { return this.currentData._t_isIrmUslug; }
      set {        this.currentData._t_isIrmUslug = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 29 */
   public decimal T_ppmvOsn
   {
      get { return this.currentData._t_ppmvOsn; }
      set {        this.currentData._t_ppmvOsn = value; }
   }
   /* 30 */ public decimal T_ppmvSt1i2
   {
      get { return this.currentData._t_ppmvSt1i2; }
      set {        this.currentData._t_ppmvSt1i2 = value; }
   }

   /* 31 */ public decimal T_pnpSt
   {
      get { return this.currentData._t_pnpSt; }
      set {        this.currentData._t_pnpSt = value; }
   }

   /* 32 */ public string T_serlot
   {
      get { return this.currentData._t_serlot; }
      set {        this.currentData._t_serlot = value; }
   }

   /* */

   #endregion DataLayer Propertiez

   #region SetTwinTransRec2

   public uint T_bak_recID
   {
      get { return this.backupData._recID; }
      set {        this.backupData._recID = value; }
   }
   public string T_bak_TT
   {
      get { return this.backupData._t_tt; }
      set {        this.backupData._t_tt = value; }
   }
   public short T_bak_TTsort
   {
      get { return this.backupData._t_ttSort; }
      set {        this.backupData._t_ttSort = value; }
   }
   public uint T_bak_twinID
   {
      get { return this.backupData._t_twinID; }
      set {        this.backupData._t_twinID = value; }
   }
   public string T_bak_skladCD
   {
      get { return this.backupData._t_skladCD; }
      set {        this.backupData._t_skladCD = value; }
   }
   public DateTime T_bak_skladDate
   {
      get { return this.backupData._t_skladDate; }
      set { this.backupData._t_skladDate = value; }
   }


   #endregion SetTwinTransRec2

   #region Result Propertiez

   public RtransResultStruct RtrResults
   {
      get { return this._rtrResults;         }
      set {        this._rtrResults = value; }
   }

   /// <summary>
   /// return (TtInfo.IsStornoTT ? ForceNegative_T_kol /* always negativno */ : T_kol);
   /// </summary>
   public decimal R_kol
   {
      get
      {
         return (TtInfo.IsStornoTT ? ForceNegative_T_kol /* always negativno */ : T_kol);
      }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KC 
   { 
     get { return this._rtrResults._r_KC;         }
     set {        this._rtrResults._r_KC = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCR
   {
      get { return this._rtrResults._r_KCR; }
      set {        this._rtrResults._r_KCR = value; }
   }
   //[VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCRwoPDV { get { return this.R_PdvOsn; } } // backward compatibility (za kristale) 
   public decimal R_PdvOsn
   {
      //get { return this._rtrResults._r_KCR   - this._rtrResults._r_pdv  ; }
      //get { return this._rtrResults._r_KCR/* - this._rtrResults._r_pdv*/; }

      get { return this._rtrResults._r_KCR - (T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN ? this.T_ppmvOsn : 0.00M); }
   }
   public decimal R_KCRdivKOL
   {
      get { return ZXC.DivSafe(this._rtrResults._r_KCR, this.T_kol); }
   }
   /// <summary>
   /// Voila! 
   /// return this.T_kol * this.TheAsEx.PrNabCij;
   /// </summary>
   //[VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_Kol_Puta_PrNabCij
   {
      get 
      {
         if(TheAsEx == null) return 0M;

         decimal kol = (TtInfo.IsStornoTT ? ForceNegative_T_kol /* always negativno */ : T_kol);

         // 06.03.2017: !!! BIG NEWS !!! ... a kako nije trebalo 'niprije'? 
       //return  kol * this.TheAsEx.PrNabCij;
       //return (kol * this.TheAsEx.PrNabCij).Ron2(); 

         // 10.01.2019: start 
         // 21.05.2020: ipak ugasio jer ovo i ne moze biti tocno u izvj. trenutku
         // npr. dok si u ispravi 'zutome' i Artstat jos nije formiran (recimo, SintSameArtiklRows())
         //
           decimal r_Kol_Puta_PrNabCij = (kol * this.TheAsEx.PrNabCij).Ron2();
         //if(TtInfo.IsFinKol_I && ZXC.AlmostEqual(r_Kol_Puta_PrNabCij, this.TheAsEx.RtrIzlazVrjNBC, 0.05M) == false)
         //{
         //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "R_Kol_Puta_PrNabCij != RtrIzlazVrjNBC\n\r\n\r{0}\t{1}\n\r{2}", 
         //      r_Kol_Puta_PrNabCij, this.TheAsEx.RtrIzlazVrjNBC, this);
         //}
         // 10.01.2019:  end  

         return r_Kol_Puta_PrNabCij; 
      }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCRP
   {
      get { return this._rtrResults._r_KCRP; }
      set {        this._rtrResults._r_KCRP = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCRM
   {
      get { return this._rtrResults._r_KCRM; }
      set {        this._rtrResults._r_KCRM = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_rbt1
   {
      get { return this._rtrResults._r_rbt1; }
      set {        this._rtrResults._r_rbt1 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_rbt2
   {
      get { return this._rtrResults._r_rbt2; }
      set {        this._rtrResults._r_rbt2 = value; }
   }
   // NE [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_rbtAll { get { return R_rbt1 + R_rbt2; } }

   //public decimal R_ztr
   //{
   //   get { return this._rtrResults._r_ztr; }
   //   set {        this._rtrResults._r_ztr = value; }
   //}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mrz
   {
      get { return this._rtrResults._r_mrz; }
      set {        this._rtrResults._r_mrz = value; }
   }

   public decimal R_pdvKoef { get { return T_pdvSt / 100.00M; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_pdv
   {
      get { return this._rtrResults._r_pdv; }
      set {        this._rtrResults._r_pdv = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_pdv_jed
   {
      get { return this._rtrResults._r_pdv_jed; }
      set { this._rtrResults._r_pdv_jed = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_CIJ_KCR
   {
      get { return this._rtrResults._r_cij_KCR; }
      set {        this._rtrResults._r_cij_KCR = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_CIJ_KCRM
   {
      get { return this._rtrResults._r_cij_KCRM; }
      set {        this._rtrResults._r_cij_KCRM = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_CIJ_KCRP
   {
      get { return this._rtrResults._r_cij_KCRP; }
      set {        this._rtrResults._r_cij_KCRP = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_CIJ_MSK
   {
      get { return this._rtrResults._r_cij_MSK; }
      set {        this._rtrResults._r_cij_MSK = value; }
   }
   public decimal R_mrzSt
   {
      get { return this._rtrResults._r_mrzSt; }
      set {        this._rtrResults._r_mrzSt = value; }
   }


   public bool R_isPdv_25m   { get { return T_pdvSt == 25M &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || 
                                                              T_pdvColTip == ZXC.PdvKolTipEnum.NIJE  ||
                                                              T_pdvColTip == ZXC.PdvKolTipEnum.AVANS_STORNO ||
                                                              T_pdvColTip == ZXC.PdvKolTipEnum.GlassOnIRM || 
                                                              T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN); } }

   public bool R_isPdv_25n   { get { return T_pdvSt == 25M && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }
   public bool R_isPdv_05m   { get { return T_pdvSt == 05M &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || T_pdvColTip == ZXC.PdvKolTipEnum.AVANS_STORNO || T_pdvColTip == ZXC.PdvKolTipEnum.NIJE); } }
   public bool R_isPdv_05n   { get { return T_pdvSt == 05M && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }
   public bool R_isPdv_23m   { get { return T_pdvSt == 23M &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || T_pdvColTip == ZXC.PdvKolTipEnum.NIJE); } }
   public bool R_isPdv_23n   { get { return T_pdvSt == 23M && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }
   public bool R_isPdv_22m   { get { return T_pdvSt == 22M &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || T_pdvColTip == ZXC.PdvKolTipEnum.NIJE); } }
   public bool R_isPdv_22n   { get { return T_pdvSt == 22M && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }

   public bool IsOld10Pdv    { get { return T_skladDate < ZXC.Date01012014; } }
 //public bool R_isPdv_10m   { get { return  T_pdvSt == 10M                    &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || T_pdvColTip == ZXC.PdvKolTipEnum.NIJE || T_pdvColTip == ZXC.PdvKolTipEnum.GlassOnIRM); } }
 //public bool R_isPdv_10n   { get { return  T_pdvSt == 10M                    && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }
   public bool R_isPdv_10m   { get { return (T_pdvSt == 10M || T_pdvSt == 13M) &&(T_pdvColTip == ZXC.PdvKolTipEnum.MOZE  || T_pdvColTip == ZXC.PdvKolTipEnum.NIJE || T_pdvColTip == ZXC.PdvKolTipEnum.AVANS_STORNO || T_pdvColTip == ZXC.PdvKolTipEnum.GlassOnIRM); } }
   public bool R_isPdv_10n   { get { return (T_pdvSt == 10M || T_pdvSt == 13M) && T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE; } }

   public bool R_isPdv_PR    { get { return T_pdvSt ==  0M && T_pdvColTip == ZXC.PdvKolTipEnum.PROLAZ; } }

   public bool R_isPdv_0     { get { return T_pdvSt ==  0M && 
      T_pdvColTip != ZXC.PdvKolTipEnum.PROLAZ &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL07  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL08  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL09  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL10  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL11  &&      
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL12  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL13  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL14  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL15  &&
      T_pdvColTip != ZXC.PdvKolTipEnum.KOL16  
      ; } }

   public bool R_isPdv_10    { get { return R_isPdv_10m || R_isPdv_10n; } }
   public bool R_isPdv_22    { get { return R_isPdv_22m || R_isPdv_22n; } }
   public bool R_isPdv_23    { get { return R_isPdv_23m || R_isPdv_23n; } }
   public bool R_isPdv_05    { get { return R_isPdv_05m || R_isPdv_05n; } }
   public bool R_isPdv_25    { get { return R_isPdv_25m || R_isPdv_25n; } }

   public bool R_isPdv_kol07 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL07 ; } }
   public bool R_isPdv_kol08 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL08 ; } }
   public bool R_isPdv_kol09 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL09 ; } }
   public bool R_isPdv_kol10 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL10 ; } }
   public bool R_isPdv_kol11 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL11 ; } }

   public bool R_isPdv_kol12 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL12 ; } }
   public bool R_isPdv_kol13 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL13 ; } }
   public bool R_isPdv_kol14 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL14 ; } }
   public bool R_isPdv_kol15 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL15 ; } }
   public bool R_isPdv_kol16 { get { return                                                             T_pdvColTip == ZXC.PdvKolTipEnum.KOL16 ; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_rbtPdv
   {
      get { return this._rtrResults._r_rbtPdv;  }
      set {        this._rtrResults._r_rbtPdv = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPdv
   {
      get { return this._rtrResults._r_mskPdv;  }
      set {        this._rtrResults._r_mskPdv = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPdv10
   {
      get { return this._rtrResults._r_mskPdv10;  }
      set {        this._rtrResults._r_mskPdv10 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPdv23
   {
      get { return this._rtrResults._r_mskPdv23;  }
      set {        this._rtrResults._r_mskPdv23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPdv25
   {
      get { return this._rtrResults._r_mskPdv25;  }
      set {        this._rtrResults._r_mskPdv25 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPdv05
   {
      get { return this._rtrResults._r_mskPdv05;  }
      set {        this._rtrResults._r_mskPdv05 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK
   {
      get { return this._rtrResults._r_MSK; }
      set {        this._rtrResults._r_MSK = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK_00
   {
      get { return this._rtrResults._r_MSK_00; }
      set {        this._rtrResults._r_MSK_00 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK_10
   {
      get { return this._rtrResults._r_MSK_10; }
      set {        this._rtrResults._r_MSK_10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK_23
   {
      get { return this._rtrResults._r_MSK_23; }
      set {        this._rtrResults._r_MSK_23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK_25
   {
      get { return this._rtrResults._r_MSK_25; }
      set {        this._rtrResults._r_MSK_25 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_MSK_05
   {
      get { return this._rtrResults._r_MSK_05; }
      set {        this._rtrResults._r_MSK_05 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCR_usl
   {
      get { return this._rtrResults._r_KCR_usl; }
      set {        this._rtrResults._r_KCR_usl = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_KCRP_usl
   {
      get { return this._rtrResults._r_KCRP_usl; }
      set {        this._rtrResults._r_KCRP_usl = value; }
   }

   // prodajna vrijednost
   // 16.09.2015: uvodimo tip artikla - usluge za kojega zelimo da nam je ruc uvijek 0, tj da ne ulazu u R_Ira_RUV
 //public decimal R_Ira_PV { get { return (TtInfo.IsMalopTT ? R_KCR /*- R_pdv*/ : R_KCR); } }
   // 25.01.2016: vracamo R_Ira_PV na staro, a uvodimo novi porperty R_Ira_PVunr jer na klasicnim ruc izvj. treba ovaj stari, a na onim 'sipekovim' cemo koristiti unr varijantu
 //public decimal R_Ira_PV
 //{
 //   get
 //   {
 //      if(TheAsEx == null) return R_KCR;
 //
 //      return TheAsEx.IsUslNoRuc ? 0M : R_KCR;
 //   }
 //}
   public decimal R_Ira_PV { get { return (TtInfo.IsMalopTT ? R_KCR /*- R_pdv*/ : R_KCR); } }
   public decimal R_Ira_PVunr
   {
      get
      {
         if(TheAsEx == null) return R_KCR;
   
         return TheAsEx.IsUslNoRuc ? 0M : R_KCR;
      }
   }

   // 02.03.2017: za potrebe Fak2Nal neki put nam rtrans i ne zna da je usluga, a je. 
   // pa smo si stvorili nove propertije da nam bude 'lakse' 
   // ______ START _______________
   public decimal R_byTheAsEx_uslOnly_KC
   {
      get
      {
         if(TheAsEx == null) return 0M;

         return TheAsEx.ArtiklTS == "USL" ? R_KC : 0M;
      }
   }
   public decimal R_byTheAsEx_bezUsl_KC
   {
      get
      {
         if(TheAsEx == null) return 0M;

         return TheAsEx.ArtiklTS != "USL" ? R_KC : 0M;
      }
   }
   // _______ END _______________

   // nabavna vrijednost
   public decimal R_Ira_NV { get { return (this.R_Kol_Puta_PrNabCij); } }

   public decimal R_Ira_NC 
   { 
      get 
      { 
         if(TheAsEx == null) return 0M;

       //decimal nabCij = (TheAsEx.PrNabCij.NotZero() ? TheAsEx.PrNabCij : TheAsEx.LastPrNabCij);
         decimal nabCij = TheAsEx.PrNabCij;

         return (nabCij);
      }
   }

   public decimal R_Ira_RBT_PDV { get { return (R_rbtAll / (R_pdvKoef + 1M) * R_pdvKoef); } }

   // za NV 100, PV 120 ovo vraca 20%
   public decimal R_Ira_RUC { get { return ZXC.StopaPromjene(R_Ira_NV, R_Ira_PV); } }

   // za NV 100, PV 120 ovo vraca 20
   public decimal R_Ira_RUV { get { return (R_Ira_PV - R_Ira_NV); } }

   // 24.03.2014: 
 //public bool R_isIrmRoba { get { return (!(T_isIrmUsluga                                )); } }
   public bool R_isIrmRoba { get { return (!(T_isIrmUsluga == true || T_artiklCD.IsEmpty())); } }

   //public string R_SvdArt_Kategorija
   //{
   //   get
   //   {
   //      return R_isSvdArtGR_Ljek ? "LIJEKOVI" : "POTROŠNI";
   //   }
   //}
   //
   //public bool R_isSvdArtGR_Ljek
   //{
   //   get
   //   {
   //      return
   //         this.A_ArtGrCd1 == "90" ||
   //         this.A_ArtGrCd1 == "A0" ||
   //         this.A_ArtGrCd1 == "N0" ||
   //         this.A_ArtGrCd1 == "10"  ;
   //   }
   //}
   //
   //public bool R_isSvdArtGR_Potros
   //{
   //   get
   //   {
   //      return
   //         this.A_ArtGrCd1 == "00" ||
   //         this.A_ArtGrCd1 == "20" ||
   //         this.A_ArtGrCd1 == "30" ||
   //         this.A_ArtGrCd1 == "40" ||
   //         this.A_ArtGrCd1 == "50" ||
   //         this.A_ArtGrCd1 == "60" ||
   //         this.A_ArtGrCd1 == "70" ||
   //         this.A_ArtGrCd1 == "80"  ;
   //   }
   //}

   public bool R_isBadPdvColTip_ForPdvStopaZero
   {
      get
      {
         if(this.T_pdvSt.NotZero()) return false;

         if(this.TtInfo.IsIzlazniPdvTT)
         {
            if(this.T_TT == Faktur.TT_IRM && this.T_pdvColTip == ZXC.PdvKolTipEnum.NIJE) return true; // na IRM, ne moze stopa 0 i prazna PK kolona 

            return
             //this.T_pdvColTip == ZXC.PdvKolTipEnum.NIJE   ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.MOZE   ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.NEMOZE //||
               //15.02.2023. za prolazni 0% pdv povratna ambalaža
             //this.T_pdvColTip == ZXC.PdvKolTipEnum.PROLAZ  
               ;
         }
         else if(this.TtInfo.IsUlazniPdvTT)
         {
            return
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL07 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL08 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL09 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL10 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL11 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL12 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL13 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL14 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL15 ||
               this.T_pdvColTip == ZXC.PdvKolTipEnum.KOL16  ;
         }

         return false;
      }
   }

   //public decimal R_ZCP_DiffCij  { get { return (R_CIJ_MSK - T_doCijMal   ); } }
   //public decimal R_ZCP_UlazVrij { get { return (R_kol     * R_ZCP_DiffCij); } }

   //[VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   // 19.06.2013: ne mogu se sjetiti zasto su ovdje neki R property DevizaConvertibilni a neki NE?! 
   public decimal R_ppmvIzn
   {
      get
      {
         decimal osnovica;

         // 10.01.2017: Od 2017 je u PpmvOsn vec izracunani iznosPpmv a  st1i2 je 100 
         if(ZXC.projectYearFirstDay.Year < 2017) osnovica = Math.Max(T_cij, T_ppmvOsn);
         else                                    osnovica =                 T_ppmvOsn ;

         // 11.09.2015: 
       //return ZXC.VvGet_25_on_100(osnovica, T_ppmvSt1i2);
         decimal ppmvIzn = ZXC.VvGet_25_of_100(osnovica, T_ppmvSt1i2);
         return T_kol.IsNegative() ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
      }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_Pnp
   {
      get { return this._rtrResults._r_pnp; }
      set {        this._rtrResults._r_pnp = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_PnpOsn
   {
      get { return this._rtrResults._r_pnpOsn; }
      set {        this._rtrResults._r_pnpOsn = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_mskPnp
   {
      get { return this._rtrResults._r_mskPnp; }
      set {        this._rtrResults._r_mskPnp = value; }
   }

   // 14.12.2013: nakon uvodjenja pnp-a ovaj 'R_KCRPwoPPMV' je PUSE 
   public decimal R_KCRPwoPPMV     { get { return (R_KCRP - R_ppmvIzn);            } }
 //public decimal R_KCRPwoPPMV_PNP { get { return (R_KCRP - R_ppmvIzn - R_pnpIzn); } }
   public bool    R_isPNP          { get { return this.T_pnpSt.NotZero();          } }

   // JPN - jelo, pice, napitak 
   public bool    R_isJPN          { get { return (R_grName == "PIĆ" || R_grName == "JEL" || R_grName == "NAP"); } } // JPN - jelo, pice, napitak 

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal Rkiz_KC 
   { 
     get { return this._rtrResults._rkiz_KC;         }
     set {        this._rtrResults._rkiz_KC = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal Rkiz_KCR
   {
      get { return this._rtrResults._rkiz_KCR; }
      set {        this._rtrResults._rkiz_KCR = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal Rkiz_rbt1
   {
      get { return this._rtrResults._rkiz_rbt1; }
      set {        this._rtrResults._rkiz_rbt1 = value; }
   }

   #endregion Result Propertiez

   // 11.01.2018: 
 //public /*ZXC.TH_CycleMoment*/string TH_CycleMoment { get { return ZXC.TH_GetCycleMoment_AsNiceString(this.T_skladDate, this.T_skladCD); } }
   public /*ZXC.TH_CycleMoment*/string TH_CycleMoment 
   { 
      get 
      {
         if(ZXC.IsTEXTHOany == false && ZXC.IsTEXTHOany2 == false || ZXC.IsSkladCD_THshop(this.T_skladCD) == false) return "Nepoznat TH_CycleMoment";

         // THPR news: 
       //return ZXC.TH_GetCycleMoment_AsNiceString(this.T_skladDate, this.T_skladCD);
         return TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(this.T_skladCD, this.T_skladDate).Opis;
      } 
   }

   public bool IsNultiZPC { get { return Faktur.Get_IsNultiZPC(this.T_TT, this.T_ttNum); } }

   public string TtAndTtNumAndSerial { get { return Faktur.Set_TT_And_TtNum(this.T_TT, this.T_ttNum) + "-" + this.T_serial; } }
   public string TtAndTtNum          { get { return Faktur.Set_TT_And_TtNum(this.T_TT, this.T_ttNum)                      ; } }

   #region Iznos po mjernim jedinicama (masa, povrsina, zapremina, ...)

   public decimal R_mjMasaN  { get; set; }   public string R_mjMasaNJM  { get; set; }
   public decimal R_mjMasaB  { get; set; }   public string R_mjMasaBJM  { get; set; }
   public decimal R_mjPovrs  { get; set; }   public string R_mjPovrsJM  { get; set; }
   public decimal R_mjZaprem { get; set; }   public string R_mjZapremJM { get; set; }
   public decimal R_mjDuljin { get; set; }   public string R_mjDuljinJM { get; set; }
   public decimal R_mjSirina { get; set; }   public string R_mjSirinaJM { get; set; }
   public decimal R_mjVisina { get; set; }   public string R_mjVisinaJM { get; set; }

 /* public string R_subKolJM   { get; set; } */

   public void CalcUkupnoPoJediniciMjere(Artikl artikl)
   {
      if(artikl == null) return;

      this.R_mjMasaN  = this./*T*/R_kol * artikl.MasaNetto; this.R_mjMasaNJM  = artikl.MasaNettoJM;
      this.R_mjMasaB  = this./*T*/R_kol * artikl.MasaBruto; this.R_mjMasaBJM  = artikl.MasaBrutoJM;
      this.R_mjPovrs  = this./*T*/R_kol * artikl.Povrsina ; this.R_mjPovrsJM  = artikl.PovrsinaJM ;
      this.R_mjZaprem = this./*T*/R_kol * artikl.Zapremina; this.R_mjZapremJM = artikl.ZapreminaJM;
      this.R_mjDuljin = this./*T*/R_kol * artikl.Duljina  ; this.R_mjDuljinJM = artikl.DuljinaJM  ;
      this.R_mjSirina = this./*T*/R_kol * artikl.Sirina   ; this.R_mjSirinaJM = artikl.SirinaJM   ;
      this.R_mjVisina = this./*T*/R_kol * artikl.Visina   ; this.R_mjVisinaJM = artikl.VisinaJM   ;
   }

   public string R_firstNotEmptyPodJM
   {
      get 
      {
         if(this.R_mjMasaNJM .NotEmpty()) return this.R_mjMasaNJM ;
         if(this.R_mjMasaBJM .NotEmpty()) return this.R_mjMasaBJM ;
         if(this.R_mjPovrsJM .NotEmpty()) return this.R_mjPovrsJM ;
         if(this.R_mjZapremJM.NotEmpty()) return this.R_mjZapremJM;
         if(this.R_mjDuljinJM.NotEmpty()) return this.R_mjDuljinJM;
         if(this.R_mjSirinaJM.NotEmpty()) return this.R_mjSirinaJM;
         if(this.R_mjVisinaJM.NotEmpty()) return this.R_mjVisinaJM;

         return "";
      }
   }

   //public decimal /*Calc*/R_orgPak(Artikl artikl)
   //{
   //   if(artikl == null) return 0.00M;

   //   // tko prvi, taj kwachy 

   //   if(artikl.MasaNetto.NotZero()) { this.R_subKolJM = artikl.MasaNettoJM; return /* this.T_kol * */ artikl.MasaNetto; }
   //   if(artikl.MasaBruto.NotZero()) { this.R_subKolJM = artikl.MasaBrutoJM; return /* this.T_kol * */ artikl.MasaBruto; }
   //   if(artikl.Povrsina .NotZero()) { this.R_subKolJM = artikl.PovrsinaJM ; return /* this.T_kol * */ artikl.Povrsina ; }
   //   if(artikl.Zapremina.NotZero()) { this.R_subKolJM = artikl.ZapreminaJM; return /* this.T_kol * */ artikl.Zapremina; }
   //   if(artikl.Duljina  .NotZero()) { this.R_subKolJM = artikl.DuljinaJM  ; return /* this.T_kol * */ artikl.Duljina  ; }
   //   if(artikl.Sirina   .NotZero()) { this.R_subKolJM = artikl.SirinaJM   ; return /* this.T_kol * */ artikl.Sirina   ; }
   //   if(artikl.Visina   .NotZero()) { this.R_subKolJM = artikl.VisinaJM   ; return /* this.T_kol * */ artikl.Visina   ; }

   //   return 0.00M;
   //}

   public bool NeedsPpmvOr_kolOPvalues
   {
      get
      {
         if(ZXC.IsRNMnotRNP) return (this.T_TT == Faktur.TT_PIP || this.T_TT == Faktur.TT_RNU || this.T_ppmvOsn != 1M); // Metaflex 
         else                return true;
      }
   }

   //// Ipak, NE. Nego preko Artstat.Rtr_Kol_Kol2_Ratio 
   //// 15.12.2017. 
   //public decimal R_kol2 { get { return (TtInfo.IsStornoTT ? ForceNegative_T_kol2 /* always negativno */ : T_kol2); } }
   //// 15.12.2017. 
   //public decimal R_kol_kol2_ratio
   //{
   //   get
   //   {
   //      return ZXC.DivSafe(R_kol, R_kol2);
   //   }
   //}


   public decimal R_kolOP
   { 
      get 
      { 
         // 25.01.2017:
         if(ZXC.IsRNMnotRNP) // metaflex 
         {
            return R_kol * R_KgPoKom;
         }

         if(TheAsEx == null) return this.R_kol;

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);

         if(artikl_rec != null) return (R_kol * artikl_rec.R_orgPak);

         return (R_kol * TheAsEx.OrgPak);
      }
   }

   public decimal R_kolOP_reciprocno
   {
      get
      {
         if(TheAsEx == null) return ZXC.DivSafe(1M, this.R_kol);

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);

         if(artikl_rec != null) return ZXC.DivSafe(1M, artikl_rec.R_orgPak);

         return ZXC.DivSafe(1M, TheAsEx.OrgPak);
      }
   }

   public decimal R_kol_via_kolOP
   {
      get
      {
         return (R_kolOP * R_kolOP_reciprocno);
      }
   }

   public decimal R_KgPoKom { get { return T_ppmvOsn.NotZero() ? T_ppmvOsn : 1.00M; } }

   // samo za RiskReport 'RptR_Rekap_RNM'. TmpDecimal glumi 'R_cijOP_OTP' 
   public decimal R_cij_OTP { get { return TmpDecimal * R_KgPoKom; } }

   public decimal R_cijOP
   {
      get
      {
         // 25.01.2017:
         if(ZXC.IsRNMnotRNP) // metaflex 
         {
            return ZXC.DivSafe(T_cij, R_KgPoKom);
         }

         if(TheAsEx == null) return this.T_cij;

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);

         if(artikl_rec != null) return ZXC.DivSafe(T_cij, artikl_rec.R_orgPak);

         return ZXC.DivSafe(T_cij, TheAsEx.OrgPak);
      }
   }

   public decimal R_CIJ_KCR_OP // prodajna cijena po litri (realna, nakon eventualnog rabata)
   {
      get
      {
         if(TheAsEx == null) return this.R_CIJ_KCR;

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);

         if(artikl_rec != null) return ZXC.DivSafe(R_CIJ_KCR, artikl_rec.R_orgPak);

         return ZXC.DivSafe(R_CIJ_KCR, TheAsEx.OrgPak);
      }
   }

   public decimal R_Ira_NC_OP // Nabavna cijena po litri 
   {
      get
      {
         if(TheAsEx == null) return 0M;

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);

         if(artikl_rec != null) return ZXC.DivSafe(R_Ira_NC, artikl_rec.R_orgPak);

         return ZXC.DivSafe(R_Ira_NC, TheAsEx.OrgPak);
      }
   }

   public decimal R_Ira_RUV_poJM { get { return (this.R_CIJ_KCR    - this.R_Ira_NC   ); } } // Razlika izmedju PC i NC po kanti po JedMj  
   public decimal R_Ira_RUV_poOP { get { return (this.R_CIJ_KCR_OP - this.R_Ira_NC_OP); } } // Razlika izmedju PC i NC po litri 

   public decimal R_BOP { get { return ZXC.DivSafe(this.T_kol, this.T_doCijMal); } }
   public decimal R_COP { get { return this.T_cij * this.T_doCijMal; } }

   #endregion Iznos po mjernim jedinicama (masa, povrsina, zapremina, ...)

   #region Kune Backup Values

   public decimal Tkn_cij       { get; set; }
                                
   public decimal Rkn_KC        { get; set; }
   public decimal Rkn_rbt1      { get; set; }
   public decimal Rkn_CIJ_KCR   { get; set; }
   public decimal Rkn_KCR       { get; set; }
   public decimal Rkn_CIJ_KCRP  { get; set; }
   public decimal Rkn_KCRP      { get; set; }

   #endregion Kune Backup Values

   #region Some Util Propertiz

   public decimal TmpDecimal  { get; set; }
   public decimal TmpDecimal2 { get; set; }

   public ZXC.MinusTrouble MinusStatus { get; set; }

   public uint FakRbr { get; set; }

   public string R_kupdobName { get; set; }
   public string R_grName     { get; set; }
   public bool   R_utilBool   { get; set; }

   // 11.07.2018: 
   public string R_utilString   { get; set; }
   public uint   R_utilUint     { get; set; }

   // NE ZABORAVI dodati property u 'Clone()' 
   // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
   // 17.04.2013: vezano na onaj gore NOTA BENE

   #endregion Some Util Propertiz

   #region TheAsEx - Artstat propertiz

public uint     AS_RtransRecID       { get { return this.TheAsEx.RtransRecID        ; } set { this.TheAsEx.RtransRecID         = value; } }
public string   AS_ArtiklCD          { get { return this.TheAsEx.ArtiklCD           ; } set { this.TheAsEx.ArtiklCD            = value; } }
public string   AS_SkladCD           { get { return this.TheAsEx.SkladCD            ; } set { this.TheAsEx.SkladCD             = value; } }
public DateTime AS_SkladDate         { get { return this.TheAsEx.SkladDate          ; } set { this.TheAsEx.SkladDate           = value; } }
public string   AS_TT                { get { return this.TheAsEx.TT                 ; } set { this.TheAsEx.TT                  = value; } }
public Int16    AS_TtSort            { get { return this.TheAsEx.TtSort             ; } set { this.TheAsEx.TtSort              = value; } }
public uint     AS_TtNum             { get { return this.TheAsEx.TtNum              ; } set { this.TheAsEx.TtNum               = value; } }
public ushort   AS_Serial            { get { return this.TheAsEx.Serial             ; } set { this.TheAsEx.Serial              = value; } }
public uint     AS_TransRbr          { get { return this.TheAsEx.TransRbr           ; } set { this.TheAsEx.TransRbr            = value; } }
public string   A_ArtiklTS           { get { return this.TheAsEx.ArtiklTS           ; } set { this.TheAsEx.ArtiklTS            = value; } }
public string   A_ArtiklJM           { get { return this.TheAsEx.ArtiklJM           ; } set { this.TheAsEx.ArtiklJM            = value; } }
public decimal  A_UkPstFinKNJ        { get { return this.TheAsEx.UkPstFinKNJ        ; }                                                   }
public decimal  A_UkUlazFinKNJ       { get { return this.TheAsEx.UkUlazFinKNJ       ; }                                                   }
public decimal  A_UkUlazFinKNJAll    { get { return this.TheAsEx.UkUlazFinKNJAll    ; }                                                   }
public decimal  A_UkIzlazFinKNJ      { get { return this.TheAsEx.UkIzlazFinKNJ      ; }                                                   }
public decimal  A_UkUlazFirmaFinKNJ  { get { return this.TheAsEx.UkUlazFirmaFinKNJ  ; }                                                   }
public decimal  A_UkIzlazFirmaFinKNJ { get { return this.TheAsEx.UkIzlazFirmaFinKNJ ; }                                                   }
public decimal  A_StanjeFinKNJ       { get { return this.TheAsEx.StanjeFinKNJ       ; }                                                   }
public decimal  A_KnjigCij           { get { return this.TheAsEx.KnjigCij           ; }                                                   }
public decimal  A_LastKnjigCij       { get { return this.TheAsEx.LastKnjigCij       ; }                                                   }
public decimal  A_UkPstFinNBC        { get { return this.TheAsEx.UkPstFinNBC        ; } set { this.TheAsEx.UkPstFinNBC         = value; } }
public decimal  A_UkUlazFinNBC       { get { return this.TheAsEx.UkUlazFinNBC       ; } set { this.TheAsEx.UkUlazFinNBC        = value; } }
public decimal  A_UkUlazFinNBCAll    { get { return this.TheAsEx.UkUlazFinNBCAll    ; }                                                   }
public decimal  A_UkIzlazFinNBC      { get { return this.TheAsEx.UkIzlazFinNBC      ; } set { this.TheAsEx.UkIzlazFinNBC       = value; } }
public decimal  A_UkUlazFirmaFinNBC  { get { return this.TheAsEx.UkUlazFirmaFinNBC  ; } set { this.TheAsEx.UkUlazFirmaFinNBC   = value; } }
public decimal  A_UkIzlazFirmaFinNBC { get { return this.TheAsEx.UkIzlazFirmaFinNBC ; } set { this.TheAsEx.UkIzlazFirmaFinNBC  = value; } }
public decimal  A_StanjeFinNBC       { get { return this.TheAsEx.StanjeFinNBC       ; }                                                   }
public decimal  A_PrNabCij           { get { return this.TheAsEx.PrNabCij           ; }                                                   }
public decimal  A_LastPrNabCij       { get { return this.TheAsEx.LastPrNabCij       ; } set { this.TheAsEx.LastPrNabCij        = value; } }
public decimal  A_UkPstFinMPC        { get { return this.TheAsEx.UkPstFinMPC        ; } set { this.TheAsEx.UkPstFinMPC         = value; } }
public decimal  A_UkUlazFinMPC       { get { return this.TheAsEx.UkUlazFinMPC       ; } set { this.TheAsEx.UkUlazFinMPC        = value; } }
public decimal  A_UkUlazFinMPCAll    { get { return this.TheAsEx.UkUlazFinMPCAll    ; }                                                   }
public decimal  A_UkIzlazFinMPC      { get { return this.TheAsEx.UkIzlazFinMPC      ; } set { this.TheAsEx.UkIzlazFinMPC       = value; } }
public decimal  A_UkUlazFirmaFinMPC  { get { return this.TheAsEx.UkUlazFirmaFinMPC  ; } set { this.TheAsEx.UkUlazFirmaFinMPC   = value; } }
public decimal  A_UkIzlazFirmaFinMPC { get { return this.TheAsEx.UkIzlazFirmaFinMPC ; } set { this.TheAsEx.UkIzlazFirmaFinMPC  = value; } }
public decimal  A_StanjeFinMPC       { get { return this.TheAsEx.StanjeFinMPC       ; }                                                   }
public decimal  A_MalopCij           { get { return this.TheAsEx.MalopCij           ; }                                                   }
public decimal  A_PrevMalopCij       { get { return this.TheAsEx.PrevMalopCij       ; }                                                   }
public decimal  A_LastUlazMPC        { get { return this.TheAsEx.LastUlazMPC        ; } set { this.TheAsEx.LastUlazMPC         = value; } }
public decimal  A_UkIzlFinProdKCR    { get { return this.TheAsEx.UkIzlFinProdKCR    ; } set { this.TheAsEx.UkIzlFinProdKCR     = value; } }
public decimal  A_StanjeKol          { get { return this.TheAsEx.StanjeKol          ; }                                                   }
public decimal  A_UkUlazKolAll       { get { return this.TheAsEx.UkUlazKolAll       ; }                                                   }
public decimal  A_UkPstKol           { get { return this.TheAsEx.UkPstKol           ; } set { this.TheAsEx.UkPstKol            = value; } }
public string   A_OrgPakJM           { get { return this.TheAsEx.OrgPakJM           ; } set { this.TheAsEx.OrgPakJM            = value; } }
public decimal  A_OrgPak             { get { return this.TheAsEx.OrgPak             ; } set { this.TheAsEx.OrgPak              = value; } }
public decimal  A_UkUlazKol          { get { return this.TheAsEx.UkUlazKol          ; } set { this.TheAsEx.UkUlazKol           = value; } }
public decimal  A_UkIzlazKol         { get { return this.TheAsEx.UkIzlazKol         ; } set { this.TheAsEx.UkIzlazKol          = value; } }
public decimal  A_UkUlazFirmaKol     { get { return this.TheAsEx.UkUlazFirmaKol     ; } set { this.TheAsEx.UkUlazFirmaKol      = value; } }
public decimal  A_UkIzlazFirmaKol    { get { return this.TheAsEx.UkIzlazFirmaKol    ; } set { this.TheAsEx.UkIzlazFirmaKol     = value; } }

public decimal  A_StanjeKol2         { get { return this.TheAsEx.StanjeKol2         ; }                                                   }
public decimal  A_UkUlazKol2All      { get { return this.TheAsEx.UkUlazKol2All      ; }                                                   }
public decimal  A_UkPstKol2          { get { return this.TheAsEx.UkPstKol2          ; } set { this.TheAsEx.UkPstKol2           = value; } }
public decimal  A_UkUlazKol2         { get { return this.TheAsEx.UkUlazKol2         ; } set { this.TheAsEx.UkUlazKol2          = value; } }
public decimal  A_UkIzlazKol2        { get { return this.TheAsEx.UkIzlazKol2        ; } set { this.TheAsEx.UkIzlazKol2         = value; } }
public decimal  A_UkUlazFirmaKol2    { get { return this.TheAsEx.UkUlazFirmaKol2    ; } set { this.TheAsEx.UkUlazFirmaKol2     = value; } }
public decimal  A_UkIzlazFirmaKol2   { get { return this.TheAsEx.UkIzlazFirmaKol2   ; } set { this.TheAsEx.UkIzlazFirmaKol2    = value; } }
   
public decimal  A_StanjKolFisycal    { get { return this.TheAsEx.StanjKolFisycal    ; }                                                   }
public decimal  A_UkUlazKolAllFisycal{ get { return this.TheAsEx.UkUlazKolAllFisycal; }                                                   }
public decimal  A_UkUlazKolFisycal   { get { return this.TheAsEx.UkUlazKolFisycal   ; } set { this.TheAsEx.UkUlazKolFisycal    = value; } }
public decimal  A_UkIzlazKolFisycal  { get { return this.TheAsEx.UkIzlazKolFisycal  ; } set { this.TheAsEx.UkIzlazKolFisycal   = value; } }
public decimal  A_StanjeKolFree      { get { return this.TheAsEx.StanjeKolFree      ; }                                                   }
public decimal  A_UkRezervKolNaruc   { get { return this.TheAsEx.UkRezervKolNaruc   ; } set { this.TheAsEx.UkRezervKolNaruc    = value; } }
public decimal  A_UkRezervKolIsporu  { get { return this.TheAsEx.UkRezervKolIsporu  ; } set { this.TheAsEx.UkRezervKolIsporu   = value; } }
public decimal  A_UkStanjeKolRezerv  { get { return this.TheAsEx.UkStanjeKolRezerv  ; } set { this.TheAsEx.UkStanjeKolRezerv   = value; } }
public decimal  A_InvKol             { get { return this.TheAsEx.InvKol             ; } set { this.TheAsEx.InvKol              = value; } }
public decimal  A_InvKol2            { get { return this.TheAsEx.InvKol2            ; } set { this.TheAsEx.InvKol2             = value; } }
public decimal  A_InvFinNBC          { get { return this.TheAsEx.InvFinNBC          ; } set { this.TheAsEx.InvFinNBC           = value; } }
public decimal  A_InvFinMPC          { get { return this.TheAsEx.InvFinMPC          ; } set { this.TheAsEx.InvFinMPC           = value; } }
public decimal  A_InvFinKNJ          { get { return this.TheAsEx.InvFinKNJ          ; }                                                   }
public decimal  A_UlazCijMin         { get { return this.TheAsEx.UlazCijMin         ; } set { this.TheAsEx.UlazCijMin          = value; } }
public decimal  A_UlazCijMax         { get { return this.TheAsEx.UlazCijMax         ; } set { this.TheAsEx.UlazCijMax          = value; } }
public decimal  A_UlazCijLast        { get { return this.TheAsEx.UlazCijLast        ; } set { this.TheAsEx.UlazCijLast         = value; } }
public decimal  A_IzlazCijMin        { get { return this.TheAsEx.IzlazCijMin        ; } set { this.TheAsEx.IzlazCijMin         = value; } }
public decimal  A_IzlazCijMax        { get { return this.TheAsEx.IzlazCijMax        ; } set { this.TheAsEx.IzlazCijMax         = value; } }
public decimal  A_IzlazCijLast       { get { return this.TheAsEx.IzlazCijLast       ; } set { this.TheAsEx.IzlazCijLast        = value; } }
public decimal  A_PreDefVpc1         { get { return this.TheAsEx.PreDefVpc1         ; } set { this.TheAsEx.PreDefVpc1          = value; } }
public decimal  A_PreDefVpc2         { get { return this.TheAsEx.PreDefVpc2         ; } set { this.TheAsEx.PreDefVpc2          = value; } }
public decimal  A_PreDefMpc1         { get { return this.TheAsEx.PreDefMpc1         ; } set { this.TheAsEx.PreDefMpc1          = value; } }
public decimal  A_PreDefDevc         { get { return this.TheAsEx.PreDefDevc         ; } set { this.TheAsEx.PreDefDevc          = value; } }
public decimal  A_PreDefRbt1         { get { return this.TheAsEx.PreDefRbt1         ; } set { this.TheAsEx.PreDefRbt1          = value; } }
public decimal  A_PreDefRbt2         { get { return this.TheAsEx.PreDefRbt2         ; } set { this.TheAsEx.PreDefRbt2          = value; } }
public decimal  A_PreDefMinKol       { get { return this.TheAsEx.PreDefMinKol       ; } set { this.TheAsEx.PreDefMinKol        = value; } }
public decimal  A_PreDefMarza        { get { return this.TheAsEx.PreDefMarza        ; } set { this.TheAsEx.PreDefMarza         = value; } }
public string   A_FrsMinTt           { get { return this.TheAsEx.FrsMinTt           ; } set { this.TheAsEx.FrsMinTt            = value; } }
public uint     A_FrsMinTtNum        { get { return this.TheAsEx.FrsMinTtNum        ; } set { this.TheAsEx.FrsMinTtNum         = value; } }
public DateTime A_DateZadUlaz        { get { return this.TheAsEx.DateZadUlaz        ; } set { this.TheAsEx.DateZadUlaz         = value; } }
public DateTime A_DateZadIzlaz       { get { return this.TheAsEx.DateZadIzlaz       ; } set { this.TheAsEx.DateZadIzlaz        = value; } }
public DateTime A_DateZadPst         { get { return this.TheAsEx.DateZadPst         ; } set { this.TheAsEx.DateZadPst          = value; } }
public DateTime A_DateZadInv         { get { return this.TheAsEx.DateZadInv         ; } set { this.TheAsEx.DateZadInv          = value; } }
public decimal  A_RtrPstKol          { get { return this.TheAsEx.RtrPstKol          ; } set { this.TheAsEx.RtrPstKol           = value; } } 
public decimal  A_RtrUlazKol         { get { return this.TheAsEx.RtrUlazKol         ; } set { this.TheAsEx.RtrUlazKol          = value; } } 
public decimal  A_RtrUlazKolFisycal  { get { return this.TheAsEx.RtrUlazKolFisycal  ; } set { this.TheAsEx.RtrUlazKolFisycal   = value; } } 
public decimal  A_RtrKolNaruceno     { get { return this.TheAsEx.RtrKolNaruceno     ; } set { this.TheAsEx.RtrKolNaruceno      = value; } } 
public decimal  A_RtrKolIsporuceno   { get { return this.TheAsEx.RtrKolIsporuceno   ; } set { this.TheAsEx.RtrKolIsporuceno    = value; } } 
public decimal  A_RtrIzlazKolFisycal { get { return this.TheAsEx.RtrIzlazKolFisycal ; } set { this.TheAsEx.RtrIzlazKolFisycal  = value; } } 
public decimal  A_RtrUlazAllKol      { get { return this.TheAsEx.RtrUlazAllKol      ; }                                                   } 
public decimal  A_RtrIzlazKol        { get { return this.TheAsEx.RtrIzlazKol        ; } set { this.TheAsEx.RtrIzlazKol         = value; } } 
public decimal  A_RtrPstKol2         { get { return this.TheAsEx.RtrPstKol2         ; } set { this.TheAsEx.RtrPstKol2          = value; } } 
public decimal  A_RtrUlazKol2        { get { return this.TheAsEx.RtrUlazKol2        ; } set { this.TheAsEx.RtrUlazKol2         = value; } } 
public decimal  A_RtrUlazAllKol2     { get { return this.TheAsEx.RtrUlazAllKol2     ; }                                                   } 
public decimal  A_RtrIzlazKol2       { get { return this.TheAsEx.RtrIzlazKol2       ; } set { this.TheAsEx.RtrIzlazKol2        = value; } } 
public decimal  A_RtrPstVrjNBC       { get { return this.TheAsEx.RtrPstVrjNBC       ; } set { this.TheAsEx.RtrPstVrjNBC        = value; } } 
public decimal  A_RtrUlazVrjNBC      { get { return this.TheAsEx.RtrUlazVrjNBC      ; } set { this.TheAsEx.RtrUlazVrjNBC       = value; } } 
public decimal  A_RtrUlazAllVrjNBC   { get { return this.TheAsEx.RtrUlazAllVrjNBC   ; }                                                   } 
public decimal  A_RtrIzlazVrjNBC     { get { return this.TheAsEx.RtrIzlazVrjNBC     ; } set { this.TheAsEx.RtrIzlazVrjNBC      = value; } } 
public decimal  A_RtrPstCijNBC       { get { return this.TheAsEx.RtrPstCijNBC       ; } set { this.TheAsEx.RtrPstCijNBC        = value; } } 
public decimal  A_RtrUlazCijNBC      { get { return this.TheAsEx.RtrUlazCijNBC      ; } set { this.TheAsEx.RtrUlazCijNBC       = value; } } 
public decimal  A_RtrIzlazCijNBC     { get { return this.TheAsEx.RtrIzlazCijNBC     ; } set { this.TheAsEx.RtrIzlazCijNBC      = value; } } 
public decimal  A_RtrCijenaNBC       { get { return this.TheAsEx.RtrCijenaNBC       ; } set { this.TheAsEx.RtrCijenaNBC        = value; } } 
public decimal  A_RtrPstVrjMPC       { get { return this.TheAsEx.RtrPstVrjMPC       ; } set { this.TheAsEx.RtrPstVrjMPC        = value; } } 
public decimal  A_RtrUlazVrjMPC      { get { return this.TheAsEx.RtrUlazVrjMPC      ; } set { this.TheAsEx.RtrUlazVrjMPC       = value; } } 
public decimal  A_RtrUlazAllVrjMPC   { get { return this.TheAsEx.RtrUlazAllVrjMPC   ; }                                                   } 
public decimal  A_RtrIzlazVrjMPC     { get { return this.TheAsEx.RtrIzlazVrjMPC     ; } set { this.TheAsEx.RtrIzlazVrjMPC      = value; } } 
public decimal  A_RtrPstCijMPC       { get { return this.TheAsEx.RtrPstCijMPC       ; } set { this.TheAsEx.RtrPstCijMPC        = value; } } 
public decimal  A_RtrUlazCijMPC      { get { return this.TheAsEx.RtrUlazCijMPC      ; } set { this.TheAsEx.RtrUlazCijMPC       = value; } } 
public decimal  A_RtrIzlazCijMPC     { get { return this.TheAsEx.RtrIzlazCijMPC     ; } set { this.TheAsEx.RtrIzlazCijMPC      = value; } } 
public decimal  A_RtrCijenaMPC       { get { return this.TheAsEx.RtrCijenaMPC       ; } set { this.TheAsEx.RtrCijenaMPC        = value; } } 
public decimal  A_RtrPstVrjKNJ       { get { return this.TheAsEx.RtrPstVrjKNJ       ; }                                                   } 
public decimal  A_RtrUlazVrjKNJ      { get { return this.TheAsEx.RtrUlazVrjKNJ      ; }                                                   } 
public decimal  A_RtrUlazAllVrjKNJ   { get { return this.TheAsEx.RtrUlazAllVrjKNJ   ; }                                                   } 
public decimal  A_RtrIzlazVrjKNJ     { get { return this.TheAsEx.RtrIzlazVrjKNJ     ; }                                                   } 
public decimal  A_RtrPstCijKNJ       { get { return this.TheAsEx.RtrPstCijKNJ       ; }                                                   } 
public decimal  A_RtrUlazCijKNJ      { get { return this.TheAsEx.RtrUlazCijKNJ      ; }                                                   } 
public decimal  A_RtrIzlazCijKNJ     { get { return this.TheAsEx.RtrIzlazCijKNJ     ; }                                                   } 
public decimal  A_RtrCijenaKNJ       { get { return this.TheAsEx.RtrCijenaKNJ       ; }                                                   } 


public decimal  A_PstCijProsKNJ      { get { return this.TheAsEx.PstCijProsKNJ      ; }                                                   } 
public decimal  A_UlazCijProsKNJ     { get { return this.TheAsEx.UlazCijProsKNJ     ; }                                                   } 
public decimal  A_IzlCijProsKNJ      { get { return this.TheAsEx.IzlCijProsKNJ      ; }                                                   } 
public decimal  A_PstCijProsMPC      { get { return this.TheAsEx.PstCijProsMPC      ; }                                                   } 
public decimal  A_UlazCijProsMPC     { get { return this.TheAsEx.UlazCijProsMPC     ; }                                                   } 
public decimal  A_IzlCijProsMPC      { get { return this.TheAsEx.IzlCijProsMPC      ; }                                                   } 
public decimal  A_PstCijProsNBC      { get { return this.TheAsEx.PstCijProsNBC      ; }                                                   } 
public decimal  A_UlazCijProsNBC     { get { return this.TheAsEx.UlazCijProsNBC     ; }                                                   } 
public decimal  A_IzlCijProsNBC      { get { return this.TheAsEx.IzlCijProsNBC      ; }                                                   } 
public decimal  A_IzlProdCijPros     { get { return this.TheAsEx.IzlProdCijPros     ; }                                                   } 
public decimal  A_IzlazRUVIznos      { get { return this.TheAsEx.IzlazRUVIznos      ; }                                                   } 
public decimal  A_IzlazRUVKoef       { get { return this.TheAsEx.IzlazRUVKoef       ; }                                                   } 
public decimal  A_IzlazRUVStopa      { get { return this.TheAsEx.IzlazRUVStopa      ; }                                                   }
public decimal  A_RucVpc1Iznos       { get { return this.TheAsEx.RucVpc1Iznos       ; }                                                   } 
public decimal  A_RucVpc1Koef        { get { return this.TheAsEx.RucVpc1Koef        ; }                                                   } 
public decimal  A_RucVpc1Stopa       { get { return this.TheAsEx.RucVpc1Stopa       ; }                                                   } 
public decimal  A_PrNabCijPlusMarza  { get { return this.TheAsEx.PrNabCijPlusMarza  ; }                                                   } 
public decimal  A_PrevKolStanje      { get { return this.TheAsEx.PrevKolStanje      ; }                                                   } 
public decimal  A_PrevKolStanje2     { get { return this.TheAsEx.PrevKolStanje2     ; }                                                   } 
public decimal  A_DiffMalopCij       { get { return this.TheAsEx.DiffMalopCij       ; }                                                   } 
public decimal  A_NivelacUlazVrj     { get { return this.TheAsEx.NivelacUlazVrj     ; }                                                   } 
public decimal  A_NivelacIzlazVrj    { get { return this.TheAsEx.NivelacIzlazVrj    ; }                                                   } 
public bool     A_IsMinusOK          { get { return this.TheAsEx.IsMinusOK          ; }                                                   } 
public bool     A_IsMinusNotOK       { get { return this.TheAsEx.IsMinusNotOK       ; }                                                   } 
public bool     A_IsRuc4Usluga       { get { return this.TheAsEx.IsRuc4Usluga       ; }                                                   } 
public bool     A_IsKonto4Usluga     { get { return this.TheAsEx.IsKonto4Usluga     ; }                                                   } 
public bool     A_IsKonto4UslugaDP   { get { return this.TheAsEx.IsKonto4UslugaDP   ; }                                                   } 
public bool     A_IsMaterOrPotros    { get { return this.TheAsEx.IsMaterOrPotros    ; }                                                   } 
public bool     A_IsMaterijal        { get { return this.TheAsEx.IsMaterijal        ; }                                                   } 
public bool     A_IsSitniInv         { get { return this.TheAsEx.IsSitniInv         ; }                                                   } 
public bool     A_IsAllSkladCD       { get { return this.TheAsEx.IsAllSkladCD       ; }                                                   } 
public DateTime A_DateZadPromj       { get { return this.TheAsEx.DateZadPromj       ; }                                                   }
public decimal  A_InvDiff            { get { return this.TheAsEx.InvDiff            ; }                                                   }
public decimal  A_InvDiff2           { get { return this.TheAsEx.InvDiff2           ; }                                                   }

public decimal A_InvKolDiff          { get { return this.TheAsEx.InvKolDiff         ; }                                                   }
public decimal A_InvKol2Diff         { get { return this.TheAsEx.InvKol2Diff        ; }                                                   }
public decimal A_InvFinDiffNBC       { get { return this.TheAsEx.InvFinDiffNBC      ; }                                                   }
public decimal A_InvFinDiffMPC       { get { return this.TheAsEx.InvFinDiffMPC      ; }                                                   }
public decimal A_InvFinDiffKNJ       { get { return this.TheAsEx.InvFinDiffKNJ      ; }                                                   }

public decimal A_InvKol_Visak_AFT    { get { return this.TheAsEx.InvKol_Visak_AFT   ; }                                                   }
public decimal A_InvKol_Manjk_AFT    { get { return this.TheAsEx.InvKol_Manjk_AFT   ; }                                                   }
public decimal A_InvFinNBC_Visak_AFT { get { return this.TheAsEx.InvFinNBC_Visak_AFT; }                                                   }
public decimal A_InvFinNBC_Manjk_AFT { get { return this.TheAsEx.InvFinNBC_Manjk_AFT; }                                                   }
public decimal A_InvFinMPC_Visak_AFT { get { return this.TheAsEx.InvFinMPC_Visak_AFT; }                                                   }
public decimal A_InvFinMPC_Manjk_AFT { get { return this.TheAsEx.InvFinMPC_Manjk_AFT; }                                                   }
public decimal A_InvFinKNJ_Visak_AFT { get { return this.TheAsEx.InvFinKNJ_Visak_AFT; }                                                   }
public decimal A_InvFinKNJ_Manjk_AFT { get { return this.TheAsEx.InvFinKNJ_Manjk_AFT; }                                                   }
public bool    A_IsShadowInventura   { get { return this.TheAsEx.IsShadowInventura;   } set { this.TheAsEx.IsShadowInventura   = value; } }
public decimal A_InvKol_Visak_BEF    { get { return this.TheAsEx.InvKol_Visak_BEF; } }
public decimal A_InvKol_Manjk_BEF    { get { return this.TheAsEx.InvKol_Manjk_BEF   ; }                                                   }
public decimal A_InvFinNBC_Visak_BEF { get { return this.TheAsEx.InvFinNBC_Visak_BEF; }                                                   }
public decimal A_InvFinNBC_Manjk_BEF { get { return this.TheAsEx.InvFinNBC_Manjk_BEF; }                                                   }
public decimal A_InvFinMPC_Visak_BEF { get { return this.TheAsEx.InvFinMPC_Visak_BEF; }                                                   }
public decimal A_InvFinMPC_Manjk_BEF { get { return this.TheAsEx.InvFinMPC_Manjk_BEF; }                                                   }
public decimal A_InvFinKNJ_Visak_BEF { get { return this.TheAsEx.InvFinKNJ_Visak_BEF; }                                                   }
public decimal A_InvFinKNJ_Manjk_BEF { get { return this.TheAsEx.InvFinKNJ_Manjk_BEF; }                                                   }
public decimal A_StanjeKol_INV       { get { return this.TheAsEx.StanjeKol_INV      ; }                                                   }
public decimal A_StanjeFinNBC_INV    { get { return this.TheAsEx.StanjeFinNBC_INV   ; }                                                   }
public decimal A_StanjeFinMPC_INV    { get { return this.TheAsEx.StanjeFinMPC_INV   ; }                                                   }
public decimal A_StanjeFinKNJ_INV    { get { return this.TheAsEx.StanjeFinKNJ_INV   ; }                                                   }

public decimal  A_KnjigCijOP         { get { return this.TheAsEx.KnjigCijOP         ; }                                                   } 
public decimal  A_LastKnjigCijOP     { get { return this.TheAsEx.LastKnjigCijOP     ; }                                                   } 
public decimal  A_PrNabCijOP         { get { return this.TheAsEx.PrNabCijOP         ; }                                                   } 
public decimal  A_LastPrNabCijOP     { get { return this.TheAsEx.LastPrNabCijOP     ; }                                                   } 
public decimal  A_MalopCijOP         { get { return this.TheAsEx.MalopCijOP         ; }                                                   } 
public decimal  A_LastUlazMPCOP      { get { return this.TheAsEx.LastUlazMPCOP      ; }                                                   } 
public decimal  A_PrevMalopCijOP     { get { return this.TheAsEx.PrevMalopCijOP     ; }                                                   } 
public decimal  A_UlazCijMinOP       { get { return this.TheAsEx.UlazCijMinOP       ; }                                                   } 
public decimal  A_UlazCijMaxOP       { get { return this.TheAsEx.UlazCijMaxOP       ; }                                                   } 
public decimal  A_UlazCijLastOP      { get { return this.TheAsEx.UlazCijLastOP      ; }                                                   } 
public decimal  A_IzlazCijMinOP      { get { return this.TheAsEx.IzlazCijMinOP      ; }                                                   } 
public decimal  A_IzlazCijMaxOP      { get { return this.TheAsEx.IzlazCijMaxOP      ; }                                                   } 
public decimal  A_IzlazCijLastOP     { get { return this.TheAsEx.IzlazCijLastOP     ; }                                                   }
public decimal  A_RtrUlazCijKNJOP    { get { return this.TheAsEx.RtrUlazCijKNJOP    ; }                                                   } 
public decimal  A_RtrIzlazCijKNJOP   { get { return this.TheAsEx.RtrIzlazCijKNJOP   ; }                                                   } 
public decimal  A_RtrCijenaKNJOP     { get { return this.TheAsEx.RtrCijenaKNJOP     ; }                                                   } 
public decimal  A_PstCijProsKNJOP    { get { return this.TheAsEx.PstCijProsKNJOP    ; }                                                   } 
public decimal  A_UlazCijProsKNJOP   { get { return this.TheAsEx.UlazCijProsKNJOP   ; }                                                   } 
public decimal  A_IzlCijProsKNJOP    { get { return this.TheAsEx.IzlCijProsKNJOP    ; }                                                   } 
public decimal  A_IzlProdCijProsOP   { get { return this.TheAsEx.IzlProdCijProsOP   ; }                                                   } 
public decimal  A_RucVpc1IznosOP     { get { return this.TheAsEx.RucVpc1IznosOP     ; }                                                   } 
public decimal  A_StanjeKolOP        { get { return this.TheAsEx.StanjeKolOP        ; }                                                   } 

public decimal  A_UkUlazKolAllOP     { get { return this.TheAsEx.UkUlazKolAllOP     ; }                                                   } 
public decimal  A_UkPstKolOP         { get { return this.TheAsEx.UkPstKolOP         ; }                                                   } 
public decimal  A_UkUlazKolOP        { get { return this.TheAsEx.UkUlazKolOP        ; }                                                   } 
public decimal  A_UkIzlazKolOP       { get { return this.TheAsEx.UkIzlazKolOP       ; }                                                   } 

public string   A_ArtGrCd1           { get { return this.TheAsEx.ArtGrCd1           ; } set { this.TheAsEx.ArtGrCd1            = value; } }
public string   A_ArtGrCd2           { get { return this.TheAsEx.ArtGrCd2           ; } set { this.TheAsEx.ArtGrCd2            = value; } }
public string   A_ArtGrCd3           { get { return this.TheAsEx.ArtGrCd3           ; } set { this.TheAsEx.ArtGrCd3            = value; } }

public decimal  A_RtrPdvSt           { get { return this.TheAsEx.RtrPdvSt           ; } set { this.TheAsEx.RtrPdvSt            = value; } }
public bool     A_RtrIsIrmUslug      { get { return this.TheAsEx.RtrIsIrmUslug      ; } set { this.TheAsEx.RtrIsIrmUslug       = value; } }
public uint     A_RtrParentID        { get { return this.TheAsEx.RtrParentID        ; } set { this.TheAsEx.RtrParentID         = value; } }

// 15.12.2017:
public decimal A_Rtr_Ratio_Kol_Kol2         { get { return ZXC.DivSafe(TheAsEx.RtrPstKol + TheAsEx.RtrUlazKol + TheAsEx.RtrIzlazKol, TheAsEx.RtrPstKol2 + TheAsEx.RtrUlazKol2 + TheAsEx.RtrIzlazKol2); } }
public decimal A_Rtr_UlazAll_Ratio_Kol_Kol2 { get { return ZXC.DivSafe(TheAsEx.RtrPstKol + TheAsEx.RtrUlazKol                      , TheAsEx.RtrPstKol2 + TheAsEx.RtrUlazKol2                       ); } }
public decimal A_Rtr_Izlaz_Ratio_Kol_Kol2   { get { return ZXC.DivSafe(TheAsEx.RtrIzlazKol                                         , TheAsEx.RtrIzlazKol2                                           ); } }
public decimal A_UlazAll_Ratio_Kol_Kol2     { get { return ZXC.DivSafe(TheAsEx.UkUlazKolAll                                        , TheAsEx.UkUlazKol2All                                          ); } }
public decimal A_Izlaz_Ratio_Kol_Kol2       { get { return ZXC.DivSafe(TheAsEx.UkIzlazKol                                          , TheAsEx.UkIzlazKol2                                            ); } }

public decimal A_WeeklyIzlazKol             { get { return this.TheAsEx.WeeklyIzlazKol ; }                                                   } 
public decimal A_WeeklyDeviation            { get { return this.TheAsEx.WeeklyDeviation; }                                                   } 

public decimal  A_UkUlazKolBOP              { get { return this.TheAsEx.UkUlazKolBOP         ; }                                                   } 
public decimal  A_UkIzlazKolBOP             { get { return this.TheAsEx.UkIzlazKolBOP        ; }                                                   } 
public decimal  A_PrNabCijCOP               { get { return this.TheAsEx.PrNabCijCOP          ; }                                                   }

public decimal  A_PrNBCBefThisUlaz          { get { return this.TheAsEx.PrNBCBefThisUlaz    ; } set { this.TheAsEx.PrNBCBefThisUlaz   = value; } }

   #endregion TheAsEx - Artstat propertiz

   public bool IsSvdArtGR_Ljek_ { get { return IsSvdArtGR_Ljek(this.A_ArtGrCd1); } }
   public bool IsSvdArtGR_Potr_ { get { return IsSvdArtGR_Potr(this.A_ArtGrCd1); } }

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

   public bool A_HasUselessPST { get { return this.TheAsEx.DateZadPst.NotEmpty() && this.TheAsEx.UkPstKol.IsZero() && this.TheAsEx.UkUlazKol.IsZero() && this.TheAsEx.UkIzlazKol.IsZero(); } }

   public bool Is_URA_Povrat { get { return this.T_TT == Faktur.TT_URA && this.R_kol.IsNegative(); } }

   #endregion Propertiz

   #region ToString

   public override string ToString()
   {
      // for watch (debug) only 
      if(this.TtInfo.IsRNMsetTT)
      return " TT: "     + T_TT + "-" + T_ttNum +
             "/" + R_grName + "/" + T_dokNum + "/" + 
             " (" + T_skladDate.ToShortDateString() + ") " + T_skladCD +
             " Ser: "    + T_serial   + String.Format(" k:{0:N} c:{1:N}", R_kol, T_cij) +
             " Artikl: " + T_artiklCD + "-" + T_artiklName;
      
      return " TT: "     + T_TT + "-" + T_ttNum + " (" + T_skladDate.ToShortDateString() + ") " + T_skladCD +
             " Ser: "    + T_serial   + String.Format(" k:{0:N} c:{1:N}", R_kol, T_cij) +
             " Artikl: " + T_artiklCD + "-" + T_artiklName;
                      
   }

   public string ToShortString()
   {
      return T_TT + "-" + T_ttNum                          + 
             " (" + T_skladDate.ToShortDateString() + ") " + 
             T_skladCD                                     +
             " Artikl: " + T_artiklCD + "-" + T_artiklName + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<RtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<RtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<RtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<RtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<RtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Rtrans newObject = new Rtrans();

      Generic_CloneData<RtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      Generic_CloneResultData<RtransResultStruct>(this._rtrResults, ref newObject._rtrResults);

      if(this.IsExtendable4Read) newObject.TheAsEx = this.TheAsEx.MakeDeepCopy();

      // 1.4.2011: !!! NOTA BENE + PAZI for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      // 17.04.2013: vezano na onaj gore NOTA BENE
      newObject.TmpDecimal   = this.TmpDecimal  ;
      newObject.TmpDecimal2  = this.TmpDecimal2 ;
      newObject.MinusStatus  = this.MinusStatus ;
      newObject.FakRbr       = this.FakRbr      ;
      newObject.R_kupdobName = this.R_kupdobName;
      newObject.R_grName     = this.R_grName    ;
      newObject.R_utilBool   = this.R_utilBool  ;
      newObject.Tkn_cij      = this.Tkn_cij     ;
      newObject.Rkn_KC       = this.Rkn_KC      ;
      newObject.Rkn_rbt1     = this.Rkn_rbt1    ;
      newObject.Rkn_CIJ_KCR  = this.Rkn_CIJ_KCR ;
      newObject.Rkn_KCR      = this.Rkn_KCR     ;
      newObject.Rkn_CIJ_KCRP = this.Rkn_CIJ_KCRP;
      newObject.Rkn_KCRP     = this.Rkn_KCRP    ;

      newObject.R_utilString = this.R_utilString;
      newObject.R_utilUint   = this.R_utilUint  ;

      return newObject;
   }

   public Rtrans MakeDeepCopy()
   {
      return (Rtrans)Clone();
   }

   #endregion

   #region CalcTransResults()

   public override void CalcTransResults(VvDocumentRecord _vvDocumentRecord)
   {
      if(TtInfo.IsForceMalUlazCalc) CalcTrans_MALOP_Results_ULAZ(_vvDocumentRecord as Faktur);
      else if(TtInfo.IsMalopTT)     CalcTrans_MALOP_Results     (_vvDocumentRecord as Faktur);
      else                          CalcTrans_VELEP_Results     (_vvDocumentRecord as Faktur);
   }

   private void INIT_Memset0Rtrans_GetZtr(Faktur faktur_rec)
   {
    //puse: 29.3.2011: dok sam mislio da je u ovome bug, a nije nego je bug u tome sto je 'ci.iT_ztr = -1' tj. koliona nije postojala na DUC-u 
    //Faktur faktur_rec = GetFakturRecForZavisniPurposes(_vvDocumentRecord as Faktur);
      this.RtrResults   = new RtransResultStruct();

      //################################################################################################################################## 

      // TODO: !!! Vidi kasnije sa Delovskim sta ovdje mora Ron2() a sto ne smije! 

      // ako je fak_rec null znaci da smo dosli iz DataLayer-a gdje nas vec ceka usnimljeni T_ztr,
      // a ako ne            znaci da upravo pisemo po ekranu fakture i racunamo ga
      if(faktur_rec != null && faktur_rec.IsZtrPresent)
      {
         CheckZtrColExists();

         //05.03.2012: bijo BUG!
       //T_ztr = GetZtrCut(faktur_rec.S_ukZavisni, /*faktur_rec.TrnSum_KC*/Get_S_KC_fromScreen(), R_KC);
         if(ZXC.RRD.Dsc_IsZtrViaOrgPak) // via orgPak, npr. prema kilogramima
         {
            decimal art_orgPak;
            Artikl artikl_rec;

            artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == T_artiklCD);
   
            if(artikl_rec != null) art_orgPak = artikl_rec.R_orgPak;
            else                   art_orgPak = 1.00M              ;

            T_ztr = GetZtrCut(faktur_rec.S_ukZavisni, /*faktur_rec.TrnSum_KC*/Get_S_OrgPakKol_fromScreen(), (R_kol * art_orgPak).Ron2());
         }
         else // classic via financ. ponderiranje 
         {
            T_ztr = GetZtrCut(faktur_rec.S_ukZavisni, /*faktur_rec.TrnSum_KC*/Get_S_KC_fromScreen(), (R_kol * T_cij).Ron2());
         }
      }
    //else                                              T_ztr = 0.00M;
   }

   private void CalcTrans_VELEP_Results(Faktur faktur_rec)
   {
      INIT_Memset0Rtrans_GetZtr(faktur_rec);

      //24.01.2014: 
    //                           R_KC = (R_kol * T_cij     ).Ron2();     
      if(TtInfo.IsManualPUcijTT) R_KC = (R_kol * T_noCijMal).Ron2();
      else                       R_KC = (R_kol * T_cij     ).Ron2();

      R_rbt1     = ((R_KC)          * T_rbt1St / 100.00M)/*.Ron2()*/;
      R_rbt2     = ((R_KC - R_rbt1) * T_rbt2St / 100.00M)/*.Ron2()*/;
                 
      R_KCR      = (R_KC - (R_rbt1 + R_rbt2) + T_ztr).Ron2();
                 
      R_mrz      = (R_KCR * T_wanted / 100.00M)/*.Ron2()*/;
                 
      R_KCRM     = (R_KCR + R_mrz)/*.Ron2()*/;
                 
      R_pdv      = (R_KCRM * T_pdvSt / 100.00M)/*.Ron2()*/;

      // 24.09.2018:
      // 01.10.2018: micemo odaavde - ide amo u malop jer je pdv u marzi
      //if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN)
      //{
      //   // 26.09.2018. ispravak jer ide pdv  u marzu
      // //decimal pdvOsnova = R_KCRM - T_ppmvOsn;
      //   decimal pdvOsnova = ZXC.VvGet_100_from_125((R_KCRM - T_ppmvOsn),T_pdvSt);
      //
      //   R_pdv = (pdvOsnova * T_pdvSt / 100.00M)/*.Ron2()*/; // T_ppmvOsn je artStat_rec.PrNabCij prema ulazima 
      //}

      R_pdv_jed = ZXC.DivSafe(R_pdv, R_kol);

      R_KCRP     = (R_KCRM + R_pdv)/*.Ron2()*/;

      // 16.03.2018:                                                              // !!! t_mal_cij namjesto t_nab_cij !!! 
      if(ZXC.CURR_prjkt_rec.PdvRTip == ZXC.PdvRTipEnum.NOT_IN_PDV && ZXC.IsSvDUH) // !!! t_mal_cij namjesto t_nab_cij !!! 
      {                                                                           // !!! t_mal_cij namjesto t_nab_cij !!! 
         R_KCR = R_KCRP;                                                          // !!! t_mal_cij namjesto t_nab_cij !!! 
      }

      R_CIJ_KCR = ZXC.DivSafe(R_KCR , R_kol)/*.Ron2()*/;    // PAZI!!! ovo je drugacije nego u Offix-u. Tamo si zaokruzivao.
      R_CIJ_KCRP = ZXC.DivSafe(R_KCRP, R_kol)/*.Ron2()*/;

      // 04.06.2014: 
      #region Komisija Extra ProdCij kao informacija / dogovor komisionom kupcu 'koliko ce ga kostati ako/kada proda'

      if(TtInfo.IsKomisExtraProdCij)
      {
         Rkiz_KC      = (R_kol * T_noCijMal).Ron2();     
         Rkiz_rbt1    = ((Rkiz_KC) * T_pnpSt / 100.00M);
         Rkiz_KCR     = (Rkiz_KC - (Rkiz_rbt1)).Ron2();
                    
         R_pdv      = (Rkiz_KCR * T_pdvSt / 100.00M);

         R_KCRP     = (Rkiz_KCR + R_pdv);

       //R_CIJ_KCR  = ZXC.DivSafe(R_KCR , R_kol);
       //R_CIJ_KCRP = ZXC.DivSafe(R_KCRP, R_kol);
      }

      #endregion

      CalcKuneBackupValues();
   }

   private void CalcKuneBackupValues()
   {
      Rkn_KC = (R_kol * Tkn_cij).Ron2(); // 10.01.2020: tu treba if(FRAG_ANTE) { 'Tkn_cij.Ron2()' } 
                                         // takoder, treba smisliti mjesto gdje ce se i 
                                         // faktur_rec.Skn_ukKCRP = rtransList.Sum(rtr => rtr.Rkn_KCRP);

      Rkn_rbt1 = ((Rkn_KC) * T_rbt1St / 100.00M)/*.Ron2()*/;

      Rkn_KCR  = (Rkn_KC - Rkn_rbt1).Ron2();

      Rkn_KCRP = (Rkn_KCR)/*.Ron2()*/;

      Rkn_CIJ_KCR  = ZXC.DivSafe(Rkn_KCR , R_kol)/*.Ron2()*/;    // PAZI!!! ovo je drugacije nego u Offix-u. Tamo si zaokruzivao.
      Rkn_CIJ_KCRP = ZXC.DivSafe(Rkn_KCRP, R_kol)/*.Ron2()*/;
   }

   private void CalcTrans_MALOP_Results(Faktur faktur_rec)
   {
           if(TtInfo.IsNivelacijaZPC) CalcTrans_MALOP_Results_ULAZ_ZPC(faktur_rec);
      else if(TtInfo.IsFinKol_U     ) CalcTrans_MALOP_Results_ULAZ    (faktur_rec);
      else                            CalcTrans_MALOP_Results_IZLAZ   (faktur_rec);
   }

   private void CalcTrans_MALOP_Results_ULAZ(Faktur faktur_rec)
   {
      if(false/*vrijednosno*/)
      {
         CalcTrans_MALOP_Results_ULAZ_ByVRIJEDNOST(faktur_rec);
      }
      else // cjenovno 
      {
         CalcTrans_MALOP_Results_ULAZ_ByCIJENA(faktur_rec); // CLASSIC 
      }
   }

   private void CalcTrans_MALOP_Results_ULAZ_ByCIJENA(Faktur faktur_rec)
   {
      INIT_Memset0Rtrans_GetZtr(faktur_rec);

      R_KC       = ((R_kol * T_cij)                     ).Ron2()  ; // fak           
      R_rbt1     = ((R_KC         ) * T_rbt1St / 100.00M).Ron2()  ; // rbt1          
      R_rbt2     = ((R_KC - R_rbt1) * T_rbt2St / 100.00M).Ron2()  ; // rbt2          
      R_KCR      = ((R_KC - (R_rbt1 + R_rbt2) + T_ztr)  ).Ron2()  ; // nab           
      R_pdv      = ((R_KCR.VvGet_25_of_100(T_pdvSt))    ).Ron2()  ; // pdv U-RA      
      R_KCRP     = ((R_KCR + R_pdv)                     ).Ron2()  ; // dobavljac     
//                                                                                   
      R_CIJ_KCR  = ZXC.DivSafe(R_KCR , R_kol)          /*.Ron2()*/; // nbc           
      R_CIJ_KCRP = ZXC.DivSafe(R_KCRP, R_kol)          /*.Ron2()*/; // nbc+pdv       

      //decimal pdvPerKom;
      decimal pdvOsnovica, mskPdvBruto, pnpOsnovica;

      switch(this.T_mCalcKind)
      {
         case ZXC.MalopCalcKind.By_MARZA: 
         //                                                                               
            R_mrzSt    = T_wanted; // !!!                                                 

            R_mrz      = (R_KCR .VvGet_25_of_100(R_mrzSt)     )  .Ron2()  ; // mrz        
            R_KCRM     = (R_KCR  + R_mrz                      )  .Ron2()  ; // vel        
            R_CIJ_KCRM = ZXC.DivSafe(R_KCRM, R_kol            )/*.Ron2()*/; // vpc        
          //pdvPerKom  = (R_CIJ_KCRM.VvGet_25_on_100(T_pdvSt) )  .Ron2()  ; // pdv po kom 
          //R_mskPdv   = (R_kol * pdvPerKom                   )  .Ron2()  ; // malSkl pdv  varijanta a 

            if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN) pdvOsnovica = R_mrz ;
            else                                             pdvOsnovica = R_KCRM;
            R_mskPdv   = (pdvOsnovica.VvGet_25_of_100(T_pdvSt))  .Ron2()  ; // malSkl pdv  varijanta b 
            R_mskPnp   = (R_KCRM     .VvGet_25_of_100(T_pnpSt))  .Ron2()  ; // porez na potrosnju      

            R_MSK      = (R_KCRM + R_mskPdv + R_mskPnp        )  .Ron2()  ; // mal        
            R_KCRM     = (R_MSK  - R_mskPdv - R_mskPnp        )  .Ron2()  ; // vel usklad?
         //                                                                               
            R_CIJ_MSK  = ZXC.DivSafe(R_MSK , R_kol            )/*.Ron2()*/; // mpc/mskZad 
         //                                                                               
            break;

         case ZXC.MalopCalcKind.By_VPC  :
         //                                                                               
            R_CIJ_KCRM = T_wanted; // !!!                                                 

            R_KCRM     = (R_kol * R_CIJ_KCRM                  )  .Ron2()  ; // vel        
            R_mrzSt    = ZXC.StopaPromjene(R_KCR, R_KCRM      )/*.Ron2()*/; // izvedena st
            R_mrz      = (R_KCRM - R_KCR                      )  .Ron2()  ; // mrz        
          //pdvPerKom  = (R_CIJ_KCRM.VvGet_25_on_100(T_pdvSt) )  .Ron2()  ; // pdv po kom 
          //R_mskPdv   = (R_kol * pdvPerKom                   )  .Ron2()  ; // malSkl pdv  varijanta a 

            if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN) pdvOsnovica = R_mrz ;
            else                                             pdvOsnovica = R_KCRM;
            R_mskPdv   = (pdvOsnovica.VvGet_25_of_100(T_pdvSt))  .Ron2()  ; // malSkl pdv  varijanta b 
            R_mskPnp   = (R_KCRM     .VvGet_25_of_100(T_pnpSt))  .Ron2()  ; // porez na potrosnju      

            R_MSK      = (R_KCRM + R_mskPdv + R_mskPnp        )  .Ron2()  ; // mal        
         //                                                                               
            R_CIJ_MSK  = ZXC.DivSafe(R_MSK, R_kol             )/*.Ron2()*/; // mpc/mskZad 
         //                                                                               
            break;

         case ZXC.MalopCalcKind.By_MPC  :
         //                                                                               
            R_CIJ_MSK  = T_wanted; // !!!                                                 

            R_MSK      = (R_kol * R_CIJ_MSK                     )  .Ron2()  ; // mal        
          //pdvPerKom  = (R_CIJ_MSK.VvGet_25_from_125(T_pdvSt)  )  .Ron2()  ; // pdv po kom 
          //R_mskPdv   = (R_kol * pdvPerKom                     )  .Ron2()  ; // malSkl pdv  varijanta a 

            pnpOsnovica= R_MSK.VvGet_100_from_128(T_pdvSt, T_pnpSt);
            R_mskPnp   = (pnpOsnovica.VvGet_25_of_100  (T_pnpSt))  .Ron2(); 

            if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN     )   mskPdvBruto = R_MSK - R_KCR   ;
            else                                                    mskPdvBruto = R_MSK - R_mskPnp;
            R_mskPdv   = (mskPdvBruto.VvGet_25_from_125(T_pdvSt))  .Ron2(); // malSkl pdv  varijanta b 

            R_KCRM     = (R_MSK - R_mskPdv - R_mskPnp           )  .Ron2()  ; // vel        
            R_mrzSt    = ZXC.StopaPromjene(R_KCR, R_KCRM        )/*.Ron2()*/; // izvedena st
            R_mrz      = (R_KCRM - R_KCR                        )  .Ron2()  ; // mrz        
         //                                                                                 
            R_CIJ_KCRM = ZXC.DivSafe(R_KCRM, R_kol              )/*.Ron2()*/; // vpc        
         //                                                                                 
            break;
      }
   
    // 19.04.2012
    //if(R_isPdv_0  && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if((R_isPdv_0 || R_isPdv_PR) && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if( R_isPdv_10               && R_isIrmRoba) { R_MSK_10 = R_MSK; R_mskPdv10 = R_mskPdv; }
      if( R_isPdv_23               && R_isIrmRoba) { R_MSK_23 = R_MSK; R_mskPdv23 = R_mskPdv; }
      if( R_isPdv_05               && R_isIrmRoba) { R_MSK_05 = R_MSK; R_mskPdv05 = R_mskPdv; }
      if( R_isPdv_25               && R_isIrmRoba) { R_MSK_25 = R_MSK; R_mskPdv25 = R_mskPdv; }

      // 24.03.2014: !!! 
    //if(T_isIrmUsluga == true)
      if(T_isIrmUsluga == true || T_artiklCD.IsEmpty())
      {
         R_KCR_usl  = R_KCR;
         R_KCRP_usl = R_KCRP;
      }

   }

   private void CalcTrans_MALOP_Results_ULAZ_ZPC(Faktur faktur_rec)
   {
      INIT_Memset0Rtrans_GetZtr(faktur_rec);

      R_KC       = ((R_kol * T_cij)                     ).Ron2()  ; // fak           
      R_rbt1     = ((R_KC         ) * T_rbt1St / 100.00M).Ron2()  ; // rbt1          
      R_rbt2     = ((R_KC - R_rbt1) * T_rbt2St / 100.00M).Ron2()  ; // rbt2          
      R_KCR      = ((R_KC - (R_rbt1 + R_rbt2) + T_ztr)  ).Ron2()  ; // nab           
      R_pdv      = ((R_KCR.VvGet_25_of_100(T_pdvSt))    ).Ron2()  ; // pdv U-RA      
      R_KCRP     = ((R_KCR + R_pdv)                     ).Ron2()  ; // dobavljac     
//                                                                                   
      R_CIJ_KCR  = ZXC.DivSafe(R_KCR , R_kol)          /*.Ron2()*/; // nbc           
      R_CIJ_KCRP = ZXC.DivSafe(R_KCRP, R_kol)          /*.Ron2()*/; // nbc+pdv       

      //decimal pdvPerKom;

      #region local Results for ZPC

      decimal R_noCij_KCR   ;
      decimal R_noCij_MSK   ;
      decimal R_noCij_mskPdv;
      decimal R_noCij_KCRM  ;
      decimal R_noCij_mrz   ;

      #endregion local Results for ZPC

      // overriding ZPC logic 

      R_KC           = 0.00M;                    // fak           
      R_noCij_KCR    = ((R_kol * T_cij).Ron2()); // fak !!!       
      R_rbt1         = 0.00M;                    // rbt1                             
      R_rbt2         = 0.00M;                    // rbt2                             
      R_KCR          = 0.00M;                    // nab                              
      R_pdv          = 0.00M;                    // pdv U-RA                         
      R_KCRP         = 0.00M;                    // dobavljac                        
                                                                        
      switch(this.T_mCalcKind)
      {
         case ZXC.MalopCalcKind.By_MARZA: 
         //                                                                               
            R_mrzSt        = T_wanted; // !!!                                                 
                              
            R_noCij_mrz    = (R_noCij_KCR .VvGet_25_of_100(R_mrzSt))  .Ron2()  ; // mrz        
            R_noCij_KCRM   = (R_noCij_KCR  + R_noCij_mrz           )  .Ron2()  ; // vel        
            R_noCij_mskPdv = (R_noCij_KCRM.VvGet_25_of_100(T_pdvSt))  .Ron2()  ; // malSkl pdv  varijanta b 
            R_noCij_MSK    = (R_noCij_KCRM + R_noCij_mskPdv        )  .Ron2()  ; // mal        
            R_noCij_KCRM   = (R_noCij_MSK  - R_noCij_mskPdv        )  .Ron2()  ; // vel usklad?
         //                                                                                    
          //T_noCijMal     = ZXC.DivSafe(R_noCij_MSK , R_kol       )/*.Ron2()*/; // mpc/mskZad 
            T_noCijMal     = ZXC.DivSafe(R_noCij_MSK , R_kol       )  .Ron (4)  ; // mpc/mskZad 
         //                                                                               
            break;

         case ZXC.MalopCalcKind.By_VPC  :
         //                                                                               
            R_CIJ_KCRM     = T_wanted; // !!!                                                 

            R_noCij_KCRM   = (R_kol * R_CIJ_KCRM                        )  .Ron2()  ; // vel        
            R_mrzSt        = ZXC.StopaPromjene(R_noCij_KCR, R_noCij_KCRM)/*.Ron2()*/; // izvedena st
            R_noCij_mrz    = (R_noCij_KCRM - R_noCij_KCR                )  .Ron2()  ; // mrz        
            R_noCij_mskPdv = (R_noCij_KCRM.VvGet_25_of_100(T_pdvSt)     )  .Ron2()  ; // malSkl pdv  varijanta b 
            R_noCij_MSK    = (R_noCij_KCRM + R_noCij_mskPdv             )  .Ron2()  ; // mal        
         //                                                                               
          //T_noCijMal     = ZXC.DivSafe(R_noCij_MSK , R_kol            )/*.Ron2()*/; // mpc/mskZad 
            T_noCijMal     = ZXC.DivSafe(R_noCij_MSK , R_kol            )  .Ron(4)  ; // mpc/mskZad 
         //                                                                               
            break;

         case ZXC.MalopCalcKind.By_MPC  :
         //                                                                               
         //                                                                               
            break;
      }
   
      //************************************************************************ 
      R_CIJ_MSK      = T_noCijMal - T_doCijMal; // !!!                                                 
                     
      R_MSK          = (R_kol * R_CIJ_MSK                         )  .Ron2()  ; // mal             
      R_noCij_MSK    = (R_kol * T_noCijMal                        )  .Ron2()  ; // mal za ZPC grid 
                                                                  
      R_mskPdv       = (R_MSK.VvGet_25_from_125(T_pdvSt)          )  .Ron2()  ; // malSkl pdv 
      R_noCij_mskPdv = (R_noCij_MSK.VvGet_25_from_125(T_pdvSt)    )  .Ron2()  ; // malSkl pdv 
      R_KCRM         = (R_MSK       - R_mskPdv                    )  .Ron2()  ; // vel        
      R_noCij_KCRM   = (R_noCij_MSK - R_noCij_mskPdv              )  .Ron2()  ; // vel        
      R_mrzSt        = ZXC.StopaPromjene(R_noCij_KCR, R_noCij_KCRM)/*.Ron2()*/; // izvedena st
      R_mrz          = (R_KCRM - R_KCR                            )  .Ron2()  ; // mrz        
   //                                                                                         
      R_CIJ_KCRM     = ZXC.DivSafe(R_noCij_KCRM, R_kol            )/*.Ron2()*/; // vpc        
      //************************************************************************ 

    // 19.04.2012
    //if(R_isPdv_0  && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if((R_isPdv_0 || R_isPdv_PR) && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if( R_isPdv_10               && R_isIrmRoba) { R_MSK_10 = R_MSK; R_mskPdv10 = R_mskPdv; }
      if( R_isPdv_23               && R_isIrmRoba) { R_MSK_23 = R_MSK; R_mskPdv23 = R_mskPdv; }
      if( R_isPdv_05               && R_isIrmRoba) { R_MSK_05 = R_MSK; R_mskPdv05 = R_mskPdv; }
      if( R_isPdv_25               && R_isIrmRoba) { R_MSK_25 = R_MSK; R_mskPdv25 = R_mskPdv; }

      // 24.03.2014: !!! 
    //if(T_isIrmUsluga == true)
      if(T_isIrmUsluga == true || T_artiklCD.IsEmpty())
      {
         R_KCR_usl  = R_KCR;
         R_KCRP_usl = R_KCRP;
      }

   }

   private void CalcTrans_MALOP_Results_ULAZ_ByVRIJEDNOST(Faktur faktur_rec)
   {
      throw new NotImplementedException();
   }

   private void CalcTrans_MALOP_Results_IZLAZ(Faktur faktur_rec)
   {
      decimal pdvBruto, mskPdvBruto, mskPnpOsnovica;

      INIT_Memset0Rtrans_GetZtr(faktur_rec);

      decimal theMPC;
      /*bool isThisMalopTT_MPCnotInT_cij = (this.T_TT == Faktur.TT_MVI);
      if(  isThisMalopTT_MPCnotInT_cij) theMPC = T_wanted;
      else                           */ theMPC = T_cij   ; // old, classic 

      // 19.05.2015: 
    //R_KC        = (R_kol * T_cij)                             .Ron2(); // Razduzenje skladista. Cijena je PRIJE izbijanja eventualnog rabata i PRIJE izbijanja poreza 
      R_KC        = (R_kol * theMPC)                            .Ron2(); // Razduzenje skladista. Cijena je PRIJE izbijanja eventualnog rabata i PRIJE izbijanja poreza 


      R_rbt1      = ZXC.VvGet_25_of_100(R_KC,          T_rbt1St).Ron2(); // maloprodajni rabat1, u njemu JE sadrzan Ppdv i PNP                                          
      R_rbt2      = ZXC.VvGet_25_of_100(R_KC - R_rbt1, T_rbt2St).Ron2(); // maloprodajni rabat2, u njemu JE sadrzan Ppdv i PNP                                          

      // 19.06.2013: 
    //R_KCRP      = (R_KC - R_rbt1 - R_rbt2                    ).Ron2(); // kupacPlatijo ("kolk'o kosta"), ('kupac' / 'dobavljac' iznos)                                
      R_KCRP      = (R_KC - R_rbt1 - R_rbt2 + R_ppmvIzn        ).Ron2(); // kupacPlatijo ("kolk'o kosta"), ('kupac' / 'dobavljac' iznos)                                

      R_PnpOsn       = R_KCRP.VvGet_100_from_128(T_pdvSt, T_pnpSt)/*.Ron2()*/;
      R_Pnp          = (R_PnpOsn      .VvGet_25_of_100(T_pnpSt));
      mskPnpOsnovica = R_KC  .VvGet_100_from_128(T_pdvSt, T_pnpSt)/*.Ron2()*/;
      R_mskPnp       = (mskPnpOsnovica.VvGet_25_of_100(T_pnpSt)); 

      if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN       )   pdvBruto = R_KCRPwoPPMV - T_ppmvOsn; // T_ppmvOsn je artStat_rec.PrNabCij prema ulazima 
      else                                                      pdvBruto = R_KCRPwoPPMV - R_Pnp    ;

      // 27.03.2023: HUGEst NEWS. zasto smo li ikad zaokruzivali? 
    //R_pdv       = ZXC.VvGet_25_from_125(pdvBruto,    T_pdvSt ).Ron2(); // the PDV                                                                                     
      R_pdv       = ZXC.VvGet_25_from_125(pdvBruto,    T_pdvSt )       ; // the PDV                                                                                     
      if(this.T_skladDate < ZXC.Date01042023) R_pdv = R_pdv     .Ron2(); // za stare podatke po starom                                                                  

      if(this.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN       )   mskPdvBruto = R_KC   - T_ppmvOsn; // T_ppmvOsn je artStat_rec.PrNabCij prema ulazima 
      else                                                      mskPdvBruto = R_KC   - R_mskPnp ; 
    //else                                                      mskPdvBruto = R_KCRP - R_mskPnp ; //14.12. umjesto R_KC ide R_KCRP jer na osnocu njega racunamo i osnovicu za pnp
      R_mskPdv    = ZXC.VvGet_25_from_125(mskPdvBruto, T_pdvSt ).Ron2(); // porez u razduzenju skladista                                                                
      R_rbtPdv    = R_mskPdv     - R_pdv                               ; // porez u rabatu                                                                              
      R_KCR       = R_KCRPwoPPMV - R_pdv - R_Pnp                       ; // kupacPlatijo pa bez pdv.                              ("ja zaradijo")                       
                  
      R_KCRM      = R_KCR;
                  
      R_MSK       = R_KC; // !!! 

      R_CIJ_KCR   = ZXC.DivSafe(R_KCR , R_kol)/*.Ron2()*/;    // PAZI!!! ovo je drugacije nego u Offix-u. Tamo si zaokruzivao.
      R_CIJ_KCRP  = ZXC.DivSafe(R_KCRP, R_kol)/*.Ron2()*/;
    //R_CIJ_MSK   = ZXC.DivSafe(R_MSK , R_kol)/*.Ron2()*/;
      R_CIJ_MSK   = theMPC;

      if(R_isIrmRoba == false) 
      { 
         R_mskPnp = 0M; 
      }

      // 30.03.2016: pri IZVOZU - kolona '9' - krivo se racuna R_MSK_00, tj. ne zbraja se jer ga R_isPdv0 izbaci posto je T_pdvColTip == ZXC.PdvKolTipEnum.KOL09
    //if(R_isPdv_0     && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if(T_pdvSt == 0M && R_isIrmRoba) { R_MSK_00 = R_MSK;                        }
      if(R_isPdv_10    && R_isIrmRoba) { R_MSK_10 = R_MSK; R_mskPdv10 = R_mskPdv; }
      if(R_isPdv_23    && R_isIrmRoba) { R_MSK_23 = R_MSK; R_mskPdv23 = R_mskPdv; }
      if(R_isPdv_05    && R_isIrmRoba) { R_MSK_05 = R_MSK; R_mskPdv05 = R_mskPdv; }
      if(R_isPdv_25    && R_isIrmRoba) { R_MSK_25 = R_MSK; R_mskPdv25 = R_mskPdv; }

      // 24.03.2014: !!! 
    //if(T_isIrmUsluga == true)
      if(T_isIrmUsluga == true || T_artiklCD.IsEmpty())
      {
         R_KCR_usl  = R_KCR;
         R_KCRP_usl = R_KCRP;
      }

      // 17.04.2023: 
      if(ShouldAdjust_2i7_MalopCij) // 'za platiti' stavke zavrsava s 2 ili 7 centa 
      {
         R_rbt1 += 0.01M;
         R_KCRP -= 0.01M;

         R_pdv = ZXC.VvGet_25_from_125(R_KCRP, T_pdvSt);
         R_KCR = R_KCRP - R_pdv                        ;
      }

   } // CalcTrans_MALOP_Results_IZLAZ 

   private bool ShouldAdjust_2i7_MalopCij
   {
      get 
      { 
         return 
            T_skladDate > ZXC.Date17042023 &&
          //ZXC.IsTEXTHOany                && 
            T_pdvSt == 25.00M              &&
            R_rbt1.NotZero()               &&
            R_KCRP_endsWith_2or7();
      }
   }

   private bool R_KCRP_endsWith_2or7()
   {
      int kcrp_x_100_as_int = (int)(Math.Floor(R_KCRP * 100.00M));

      string kcrp_x_100_as_str = kcrp_x_100_as_int.ToString();

      char lastChar = ZXC.GetStringsLastChar(kcrp_x_100_as_str);

      return lastChar == '2' || lastChar == '7';
   }

   // 08.05.2020: 'Dubravko na ispisu racuna treba pravi VPC' 
   // Uspjeli pokusaj da iz Malop calc-a dobijemo VPC
   public decimal R_theVPC     { get { return TtInfo.IsMalopFin_I ? ZXC.DivSafe(R_rbt1.NotZero() ? R_KC - R_mskPdv : R_KC - R_pdv, R_kol) : T_cij; } }
   public decimal R_CIJ_KC_IRM { get { return R_theVPC;                                     } } // jer smo vec dodali 'R_theVPC' u kristal, pa da ne moramo rename-ati ... 
   public decimal R_KC_IRM     { get { return (R_kol * R_CIJ_KC_IRM).Ron2();                } }
   public decimal R_rbt1_IRM   { get { return ((R_KC_IRM) * T_rbt1St / 100.00M)/*.Ron2()*/; } }

     // Neuspjeli pokusaji da iz Malop calc-a dobijemo VPC
   //public decimal R_theVPC_1 { get { return ZXC.DivSafe(R_rbt1.NotZero() ? R_KCRP           : R_KCR         , R_kol); } }
   //public decimal R_theVPC_2 { get { return ZXC.DivSafe(R_rbt1.NotZero() ? R_KCR + R_pdv    : R_KCRP - R_pdv, R_kol); } }
   //public decimal R_theVPC_3 { get { return ZXC.DivSafe(R_rbt1.NotZero() ? R_KC  - R_rbt1   : R_KC   - R_pdv, R_kol); } }
   //public decimal R_theVPC_4 { get { return ZXC.DivSafe(R_rbt1.NotZero() ? R_KC  - R_mskPdv : R_KC   - R_pdv, R_kol); } }

#if CalcTrans_BezRabata__DaOffix__
CalcTrans_BezRabata(transPtr, skladPtr, iraPtr)
struct trans *transPtr;
struct sklad *skladPtr;
struct IRA   *iraPtr;
{
   double  k, c, mpc, r1, r2, p1, p2, p3, fak, rbt1, rbt2;
   double  nab, pdv, vel, por2, por3, mal;

   k  = transPtr->t_kol;
   c  = transPtr->t_cij;
 mpc  = transPtr->t_cij_MAL;
   p1 = transPtr->t_pdv_st; 
   p2 = 0.;
   p3 = 0.;
   r1 = transPtr->t_rbt1_st;
   r2 = transPtr->t_rbt2_st;

   fak  = ron2(k * c);

   rbt1 = 0.; /*ron2(fak * r1 / 100.);*/
   rbt2 = 0.; /*ron2((fak - rbt1) * r2 / 100.);*/

   vel  = /*ron2*/((100. * (fak - rbt1 - rbt2) / (100. + p1 + p2)) - p3);

   pdv  = ron2((vel) * p1 / 100.);

   vel = ron2(vel);

   nab  = ron2(vel);

   mal = (vel + pdv);

   iraPtr->BEZRBT_UKUPNO  += fak;
   iraPtr->BEZRBT_OSNOVA  += vel;
   iraPtr->BEZRBT_ROB_PDV += pdv;
   iraPtr->BEZRBT_ROB_NV  += 0.;  /*fuse*/
   iraPtr->BEZRBT_ROB_PV  += fak; 


   return(0);
}
#endif

   #region UTIL Metodz

   public decimal ForceNegative_T_kol     { get { return -1.00M * Math.Abs(T_kol)    ; } }
   public decimal ForceNegative_T_BCKPkol { get { return -1.00M * Math.Abs(T_BCKPkol); } }

   public decimal ForceNegative_T_kol2    { get { return -1.00M * Math.Abs(T_kol2)    ; } }
   public decimal ForceNegative_T_BCKPkol2{ get { return -1.00M * Math.Abs(T_BCKPkol2); } }

   //puse: 29.3.2011: dok sam mislio da je u ovome bug, a nije nego je bug u tome sto je 'ci.iT_ztr = -1' tj. koliona nije postojala na DUC-u 
   private Faktur GetFakturRecForZavisniPurposes(Faktur _faktur_rec)
   {
      if(_faktur_rec != null) return _faktur_rec;

      FakturDUC theDUC = ZXC.TheVvForm.TheVvRecordUC as FakturDUC;

      if(theDUC != null && theDUC.TheVvTabPage.WriteMode != ZXC.WriteMode.None) return theDUC.faktur_rec;

      return null;
   }

   internal /*private*/ static bool CheckZtrColExists()
   {
      FakturDUC theDUC = ZXC.TheVvForm.TheVvRecordUC as FakturDUC;
      // some check 
      Faktur faktur_rec;

      if(ZXC.RISK_CopyToOtherDUC_inProgress)
      {
         faktur_rec = ZXC.FakturRec;
      }
      else
      {
         faktur_rec = theDUC.faktur_rec;
      }

      if(faktur_rec.IsZtrPresent && theDUC.DgvCI.iT_ztr.IsNegative())
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Kolona T_ztr ne postoji na ovome DUC-u. Zavisni neće biti evidentirani korektno!");
         return false;
      }

      return true;
   }

   //private void CalcTrans_MALOP_Results_NewDaDelf(VvDocumentRecord _vvDocumentRecord)
   //{
   //   Faktur faktur_rec = _vvDocumentRecord as Faktur;
   //   this.RtrResults   = new RtransResultStruct();

   //   //################################################################################################################################## 

   //   decimal tmpKCR;
   //   bool    fuseIsAmbalaza50 = false;
   //   decimal fuseAmbalaza50 = 0.50M;
   //   decimal fuseAmbalaza50vr = fuseIsAmbalaza50 ? T_kol * fuseAmbalaza50 : 0.00M;

   //   R_KCRP = R_KCRM = R_KC = (T_kol * T_cij).Ron2();

   //   R_pdv_nominal = GetPdvFrom(R_KC, T_pdvSt) - fuseAmbalaza50vr;

   //   tmpKCR = ValueWOadditions(R_KC  , T_rbt1St); 
   //   R_KCR  = ValueWOadditions(tmpKCR, T_rbt2St); 

   //   R_rbt1 = R_KC - R_KCR;

   //   R_pdv  = GetPdvFrom(R_KCR, T_pdvSt) - fuseAmbalaza50vr;

   //   //   vel  = /*ron2*/((100. * (fak - rbt1 - rbt2) / (100. + p1 + p2)) - p3);
   //   // TODO: !!! R_KCRP_NoRbt = 
   //   // TODO: !!! R_pdv_NoRbt   = 
   //}

   //private void CalcTrans_MALOP_ResultsOFFIX(VvDocumentRecord _vvDocumentRecord) // TEMPORARY!!! samo za Import Izlaznih Maloprodajnih Racuna iz Offix-a. Pitaj Bog-a kako ovo zapravo treba?! 
   //{
   //   Faktur faktur_rec = _vvDocumentRecord as Faktur;
   //   this.RtrResults   = new RtransResultStruct();

   //   //################################################################################################################################## 

   //   // OVO RADI SAMO ZA IZLAZe, za ULAZe ti fali marza! 

   //   // OVDJE JE, dakle, u T_cij sadrzan i PDV i Rabat i Marza, pa se treba izbijati unatraske
   //   /*R_KCRP =*/ R_KC = (T_kol * T_cij).Ron2();

   //   R_rbt1 = ( R_KC           * T_rbt1St / 100.00M).Ron2();
   //   R_rbt2 = ((R_KC - R_rbt1) * T_rbt2St / 100.00M).Ron2();

   //   R_KCRM = ((100.00M * (R_KC - R_rbt1 - R_rbt2) / (100.00M + T_pdvSt)))/*.Ron2()*/;

   //   R_pdv  = (R_KCRM * T_pdvSt / 100.00M).Ron2();

   //   R_KCRM = R_KCRM.Ron2();
   //   R_KCR  = R_KCRM;

   //   R_KCRP = R_KCR + R_pdv;

   //   //// BRISI OVO !!! 
   //   //Delf_IZLAZ_CalcTrans_MALOP_Results(_vvDocumentRecord);

   //   //#region DELMELATTER

   //   //decimal ukSaPdvom = 123.00M;
   //   //decimal pdvStopa  =  23.00M;
   //   //decimal osnovica  = 100.00M;
      
   //   //decimal QQQukSaPdvom = osnovica.VvPdv_UvecajZaIznosPDVa(pdvStopa);
   //   //decimal QQQpdvIznos1 = osnovica.VvPdv_GetPdvIznos      (pdvStopa);
   //   //decimal QQQpdvIznos2 = ukSaPdvom.VvPdv_IzbijPdvIznos   (pdvStopa);
   //   //decimal QQQosnovica  = ukSaPdvom.VvPdv_GetPdvOsnovica  (pdvStopa);

   //   //decimal  QQukSaPdvom = ZXC.VvPdv_UvecajZaIznosPDVa(osnovica , pdvStopa);
   //   //decimal  QQpdvIznos1 = ZXC.VvPdv_GetPdvIznos      (osnovica , pdvStopa);
   //   //decimal  QQpdvIznos2 = ZXC.VvPdv_IzbijPdvIznos    (ukSaPdvom, pdvStopa);
   //   //decimal  QQosnovica  = ZXC.VvPdv_GetPdvOsnovica   (ukSaPdvom, pdvStopa);

   //   //#endregion DELMELATTER
   //}

   private decimal Get_S_KC_fromScreen()
   {
      return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_KC_fromScreen();
   }

   private decimal Get_S_OrgPakKol_fromScreen()
   {
      return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_OrgPakKol_fromScreen();
   }

   public static decimal GetZtrCut(decimal s_ukZavisni, decimal ukupnoSvi, decimal mojDio)
   {
      decimal mojKoef = ZXC.DivSafe(mojDio, ukupnoSvi);

      return (s_ukZavisni * mojKoef);
   }
   
   #endregion UTIL Metodz

   #endregion CalcTransResults()

   #region CheckForMinus

   public bool ThisRtransPovecavaStanje(ZXC.WriteMode writeMode)
   {
      return GetDeltaKol(writeMode).IsZeroOrPositive();
   }

   public bool ThisRtransSmanjujeStanje(ZXC.WriteMode writeMode)
   {
      return !ThisRtransPovecavaStanje(writeMode);
   }

   public bool ThisRtransAddrecPovecavaStanje
   {
      get
      {
         return ThisRtransPovecavaStanje(ZXC.WriteMode.Add);
      }
   }

   public bool ThisRtransAddrecSmanjujeStanje
   {
      get
      {
         return !ThisRtransPovecavaStanje(ZXC.WriteMode.Add);
      }
   }

   // PAZI!!! Ovo ne provjerava promjenu datuma 
   public bool AfterThisRtransMinusWillBeWorse(ZXC.WriteMode writeMode, decimal kolStBeforeThisChange, out decimal deltaKol)
   {
      deltaKol = GetDeltaKol(writeMode);

      // promjena kolicine povecava kolicinsko stanje 
      if(deltaKol.IsZeroOrPositive()) return false;

      // promjena kolicine umanjuje kolicinsko stanje, ali je nakon promjene stanje jos uvijek pozitivno ili nula  
      if((kolStBeforeThisChange + deltaKol).IsZeroOrPositive()) return false;

      // promjena kolicine umanjuje kolicinsko stanje, te je nakon promjene stvoren minus ili je uvecan odprije postojeci minus 
      return true;
   }

   public bool BudeLiOvajRedakRobneKarticeIskazaoMinus(ZXC.WriteMode writeMode, decimal kolStBeforeThisChange, out decimal deltaKol)
   {
      deltaKol = GetDeltaKol2023(writeMode);

      decimal kolStAfterThisChange = kolStBeforeThisChange + deltaKol;

      return kolStAfterThisChange.IsNegative();
   }
   public decimal GetDeltaKol(ZXC.WriteMode writeMode)
   {
      decimal thisRtransKol = (TtInfo.IsStornoTT ? ForceNegative_T_kol     /* always negativno */ : T_kol    );
      decimal oldRtransKol  = (TtInfo.IsStornoTT ? ForceNegative_T_BCKPkol /* always negativno */ : T_BCKPkol);
      decimal deltaKol;

      if(TtInfo.IsFinKol_I)
      {
         thisRtransKol = -thisRtransKol;
         oldRtransKol  = -oldRtransKol;
      }

      switch(writeMode)
      {
         case ZXC.WriteMode.Add   : deltaKol = +thisRtransKol;               break;
         case ZXC.WriteMode.Delete: deltaKol = -thisRtransKol;               break;
         case ZXC.WriteMode.Edit  : deltaKol = thisRtransKol - oldRtransKol; break;
         default                  : deltaKol = 0.00M;                        break;
      }

      return deltaKol;
   }

   public decimal GetDeltaKol2023(ZXC.WriteMode writeMode)
   {
      decimal thisRtransKol = (TtInfo.IsStornoTT ? ForceNegative_T_kol     /* always negativno */ : T_kol    );
    //decimal oldRtransKol  = (TtInfo.IsStornoTT ? ForceNegative_T_BCKPkol /* always negativno */ : T_BCKPkol);
      decimal deltaKol;

      if(TtInfo.IsFinKol_I)
      {
         thisRtransKol = -thisRtransKol;
       //oldRtransKol  = -oldRtransKol ;
      }

      switch(writeMode)
      {
         case ZXC.WriteMode.Add   : deltaKol = +thisRtransKol;                    break;
         case ZXC.WriteMode.Delete: deltaKol = /*-thisRtransKol*/ 0M;             break; // !!! drugacije nego u GetDeltaKol() 
         case ZXC.WriteMode.Edit  : deltaKol = +thisRtransKol /*- oldRtransKol*/; break;
         default                  : deltaKol = 0.00M;                             break;
      }

      return deltaKol;
   }

   public decimal GetDeltaKol2023_BCKP(ZXC.WriteMode writeMode)
   {
      decimal thisRtransBCKPkol = (TtInfo.IsStornoTT ? ForceNegative_T_BCKPkol /* always negativno */ : T_BCKPkol);
      decimal deltaKol;

      if(TtInfo.IsFinKol_I)
      {
         thisRtransBCKPkol = -thisRtransBCKPkol;
      }

      switch(writeMode)
      {
         case ZXC.WriteMode.Add   : deltaKol = +thisRtransBCKPkol; break;
         case ZXC.WriteMode.Delete: deltaKol =                 0M; break; 
         case ZXC.WriteMode.Edit  : deltaKol = +thisRtransBCKPkol; break;
         default: deltaKol = 0.00M; break;
      }

      return deltaKol;
   }

   /// <summary>
   /// Sluzi samo za Check For Minus
   /// </summary>
   public DateTime DatumPocetkaPromjeneRobnojKartici
   {
      get
      {
         switch(SaveTransesWriteMode)
         {
            case ZXC.WriteMode.Add   : 
            case ZXC.WriteMode.Delete: return T_skladDate;

            case ZXC.WriteMode.Edit  : return (T_skladDate < T_BCKPskladDate ? T_skladDate : T_BCKPskladDate);

            default                  : return T_skladDate;
         }
      }
   }

   #endregion CheckForMinus

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Rtrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Rtrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Rtrans)vvDataRecord).currentData;
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


   #region IComparable<Rtrans> Members

   // Jebemumater, me kuzim jel mi ovo treba ili ne? Da li List<Rtrans>.Add(Rtrans) zadrzava poredak kako ih Add-am ili ih pomijesa?
   // Tj. moram li zvati List.Sort()? 
   // (t_artiklCD, t_skladDate, t_skladCD,   t_ttSort, t_ttNum, t_serial) NE!
   // (t_artiklCD, t_skladCD,   t_skladDate, t_ttSort, t_ttNum, t_serial) 

   // NEMOJ ovdje koristiti overridani Equals(), jer onda sjebes nesto za InvokeTransRemove!
   private bool ThisIsEqualTo(Rtrans other)
   {
      if(other == null) return false;

      return 
         (
            T_artiklCD == other.T_artiklCD &&
            T_skladDate  == other.T_skladDate  &&
            T_skladCD  == other.T_skladCD  &&
            T_ttSort   == other.T_ttSort   &&
            T_ttNum    == other.T_ttNum    &&
            T_serial   == other.T_serial
         );
   }

   public int CompareTo(Rtrans other)
   {
           if(ThisIsEqualTo  (other)) return  0;
      else if(ThisIsGreatThen(other)) return  1;
      else if(ThisIsLessThen (other)) return -1;

      else throw new Exception("Rtrans.CompareTo BUMMER!");
   }

   //private bool ORIGThisIsGreatThen(Rtrans other)
   //{
   //   if
   //   (
   //   (T_artiklCD.CompareTo(other.T_artiklCD)  > 0) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate  > other.T_skladDate) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD)  > 0) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort  > other.T_ttSort) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort == other.T_ttSort  && T_ttNum  > other.T_ttNum) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort == other.T_ttSort  && T_ttNum == other.T_ttNum  && T_serial > other.T_serial)
   //   )    return true ;
   //   else return false;
   //}

   private bool ThisIsGreatThen(Rtrans other)
   {
      if
      (
      (T_artiklCD.CompareTo(other.T_artiklCD)  > 0) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD)  > 0) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate  > other.T_skladDate) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort  > other.T_ttSort) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort == other.T_ttSort  && T_ttNum  > other.T_ttNum) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort == other.T_ttSort  && T_ttNum == other.T_ttNum  && T_serial > other.T_serial)
      )    return true ;
      else return false;
   }

   //private bool ORIGThisIsLessThen(Rtrans other)
   //{
   //   if
   //   (
   //   (T_artiklCD.CompareTo(other.T_artiklCD)  < 0) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate  < other.T_skladDate) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD)  < 0) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort  < other.T_ttSort) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort == other.T_ttSort  && T_ttNum  < other.T_ttNum) ||
   //   (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladDate == other.T_skladDate  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_ttSort == other.T_ttSort  && T_ttNum == other.T_ttNum  && T_serial < other.T_serial)
   //   )    return true ;
   //   else return false;
   //}

   private bool ThisIsLessThen(Rtrans other)
   {
      if
      (
      (T_artiklCD.CompareTo(other.T_artiklCD)  < 0) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD)  < 0) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate  < other.T_skladDate) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort  < other.T_ttSort) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort == other.T_ttSort  && T_ttNum  < other.T_ttNum) ||
      (T_artiklCD.CompareTo(other.T_artiklCD) == 0  && T_skladCD.CompareTo(other.T_skladCD) == 0  && T_skladDate == other.T_skladDate  && T_ttSort == other.T_ttSort  && T_ttNum == other.T_ttNum  && T_serial < other.T_serial)
      )    return true ;
      else return false;
   }

   #endregion

   #region IVvExtendableDataRecord Members

   public string ExtenderTableName
   {
      get
      {
         return ArtStat.recordName;
      }
   }

   public string ExtenderTableNameArhiva
   {
      get
      {
         return null;
      }
   }

   public void TakeExtenderDataFrom(VvDataRecord vvExtenderDataRecord)
   {
      this.TheAsEx = (ArtStat)vvExtenderDataRecord.Clone();
   }

   public void TakeExtender_Backup_DataFrom(VvDataRecord vvExtenderDataRecord)
   {
      if(this.IsExtendable) this.TheAsEx.BackupData = ((ArtStat)vvExtenderDataRecord).currentData;
   }

   #endregion

   #region UTIL stuff for 'Delete_Then_Renew_Cache_FromThisRtrans'

   public bool IsThisRtrans_DelrecMarkedWithWrongArtiklCD(VvSQL.DB_RW_ActionType actionType)
   {
      return 

      actionType == VvSQL.DB_RW_ActionType.DEL  &&
      this.T_BCKPartiklCD.NotEmpty()            &&
      this.T_BCKPartiklCD != this.T_artiklCD     ;

   }

   public bool IsSkladCdChanged_OnRwtAction
   {
      get
      {
         return this.T_skladCD != this.T_BCKPskladCD;
      }
   }

   // 04.05.2016: 
   public bool IsAnyOf_SortMembers_ChangedOnRwtAction
   {
      get
      {
         return (
                this.T_skladCD   != this.T_BCKPskladCD   ||
                this.T_artiklCD  != this.T_BCKPartiklCD  ||
                this.T_skladDate != this.T_BCKPskladDate ||
                this.T_ttSort    != this.T_BCKPttSort    ||
                this.T_ttNum     != this.T_BCKPttNum     ||
                this.T_serial    != this.T_BCKPserial    );
      }
   }

   #endregion UTIL stuff for 'Delete_Then_Renew_Cache_FromThisRtrans'


   internal void TransformMalop_T_cij_ToVelep_T_cij()
   {
      this.CalcTransResults(null);
      this.T_cij = ZXC.VvGet_100_from_125(T_cij, T_pdvSt);
      //this.CalcTransResults(null);
   }

   internal void RefreshKunskaCijena(decimal oldDevTec, decimal newDevTec)
   {
      this.T_cij = ZXC.DivSafe(this.T_cij, oldDevTec) * newDevTec;
   }

   internal static void SetRtransArtiklGrupa1CD(Rtrans[] rtranses, List<Artikl> artiklList)
   {
      if(rtranses   == null ||
         artiklList == null) return;

      Artikl artikl;

      foreach(Rtrans rtrans in rtranses)
      {
         artikl = artiklList.SingleOrDefault(art => art.ArtiklCD == rtrans.T_artiklCD);

         if(artikl == null) rtrans.R_grName = "";
         else               rtrans.R_grName = artikl.Grupa1CD;
      }
   }
}
