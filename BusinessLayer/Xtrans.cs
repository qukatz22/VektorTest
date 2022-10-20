using System;
using System.Linq;
using System.Collections.Generic;

#region struct XtransStruct

public struct XtransStruct
{
   internal uint     _recID;

   /*  01 */   internal uint      _t_parentID;
   /*  02 */   internal uint      _t_dokNum;
   /*  03 */   internal ushort    _t_serial;
   /*  04 */   internal DateTime  _t_dokDate;
   /*  05 */   internal string    _t_tt;
   /*  06 */   internal uint      _t_ttNum;
                                                      /* VIRMAN       | RASTER       | PUTNI NALOG          |  SMD    |  PMV     */   
   /*  07 */   internal DateTime  _t_dateOd        ;  /* dateValuta   |              |                      |         |          */   
   /*  08 */   internal DateTime  _t_dateDo        ;  /* datePodnos   |              |                      |         |          */   
   /*  09 */   internal decimal   _t_moneyA        ;  /* kn           | t_cij        | cijena               |         | ppmvOsn  */   
   /*  10 */   internal decimal   _t_kol           ;  /*              | t_kol        | Kilomet. ili Kol     | Sati    |          */   
   /*  11 */   internal string    _t_opis_128      ;  /* opplac       |              | Relacija             | Kometar |          */   
   /*  12 */   internal string    _t_kpdbNameA_50  ;  /* platName     |              |                      |         |          */
   /*  13 */   internal string    _t_kpdbUlBrA_32  ;  /* platUlicBr   |              |                      |         |          */
   /*  14 */   internal string    _t_kpdbMjestoA_32;  /* platMjesto   |              |                      |         |          */
   /*  15 */   internal string    _t_kpdbZiroA_32  ;  /* platZiro     |              |                      |         |          */
   /*  16 */   internal string    _t_kpdbNameB_50  ;  /* primName     |              |                      |         |          */
   /*  17 */   internal string    _t_kpdbUlBrB_32  ;  /* primUlicBr   |              |                      |         |          */
   /*  18 */   internal string    _t_kpdbMjestoB_32;  /* primMjesto   |              |                      |         |          */
   /*  19 */   internal string    _t_kpdbZiroB_32  ;  /* primZiro     |              | devName-Valuta4PNI   |         |          */   
   /*  20 */   internal string    _t_vezniDokA_64  ;  /* pnbzV        |              | Zadatak Loko Voznje  |         |          */   
   /*  21 */   internal string    _t_vezniDokB_64  ;  /* pnboV        |              |                      |         |          */
   /*  22 */   internal string    _t_strA_2 /* 4 */;  /* pnbzM        |              |                      |         |          */
   /*  23 */   internal string    _t_strB_2 /* 4 */;  /* pnboM        |              |                      |         |          */
   /*  24 */   internal string    _t_strC_2        ;  /* sifraPl      |              |                      |         |          */
   /*  25 */   internal uint      _t_kupdobCD      ;  /*              | kupdobCD     |                      |         |          */
   /*  26 */   internal string    _t_artiklCD      ;  /*              | t_artiklCD   |                      |         |          */
   /*  27 */   internal string    _t_artiklName    ;  /*              | t_artiklName |                      |         |          */
   /*  28 */   internal bool      _t_isXxx         ;  /*              | should skip  |                      |         |          */
         
   /*  29 */   internal int       _t_intA          ;  /*              |              | PocSt Kilomet.       |         | EuroNorma*/
   /*  30 */   internal int       _t_intB          ;  /*              |              | ZavSt Kilomet.       |         |          */
   /*  31 */   internal string    _t_konto         ;  /*              |              |                      |         |          */
   /*  32 */   internal uint      _t_personCD      ;  /*              |              |                      | X       |          */
   /*  33 */   internal decimal   _t_moneyB        ;  /*              |              |                      |         |  CO2     */   
   /*  34 */   internal decimal   _t_moneyC        ;  /*              |              |                      |         |  cm3     */   
   /*  35 */   internal decimal   _t_moneyD        ;  /*              |              |                      |         |          */   
       
   /*  36 */   internal decimal   _t_dec01         ;
   /*  37 */   internal decimal   _t_dec02         ;
   /*  38 */   internal decimal   _t_dec03         ;
   /*  39 */   internal decimal   _t_dec04         ;
   /*  40 */   internal decimal   _t_dec05         ;
   /*  41 */   internal decimal   _t_dec06         ;
   /*  42 */   internal decimal   _t_dec07         ;
   /*  43 */   internal decimal   _t_dec08         ;
   /*  44 */   internal decimal   _t_dec09         ;
   /*  45 */   internal decimal   _t_dec10         ;
   /*  46 */   internal decimal   _t_dec11         ;
   /*  47 */   internal decimal   _t_dec12         ;
   /*  48 */   internal decimal   _t_dec13         ;
   /*  49 */   internal decimal   _t_dec14         ;
   /*  50 */   internal decimal   _t_dec15         ;
   /*  51 */   internal decimal   _t_dec16         ;
   /*  52 */   internal decimal   _t_dec17         ;
   /*  53 */   internal decimal   _t_dec18         ;
   /*  54 */   internal decimal   _t_dec19         ;
   /*  55 */   internal decimal   _t_dec20         ;
   /*  56 */   internal decimal   _t_dec21         ;
   /*  57 */   internal decimal   _t_dec22         ;
   /*  58 */   internal decimal   _t_dec23         ;
   /*  59 */   internal decimal   _t_dec24         ;
   /*  60 */   internal decimal   _t_dec25         ;
   /*  61 */   internal decimal   _t_dec26         ;
   /*  62 */   internal decimal   _t_dec27         ;
   /*  63 */   internal decimal   _t_dec28         ;
   /*  64 */   internal decimal   _t_dec29         ;
   /*  65 */   internal decimal   _t_dec30         ;
   /*  66 */   internal decimal   _t_dec31         ;
   /*  67 */   internal string    _t_str01         ;
   /*  68 */   internal string    _t_str02         ;
   /*  69 */   internal string    _t_str03         ;
   /*  70 */   internal string    _t_str04         ;
   /*  71 */   internal string    _t_str05         ;
   /*  72 */   internal string    _t_str06         ;
   /*  73 */   internal string    _t_str07         ;
   /*  74 */   internal string    _t_str08         ;
   /*  75 */   internal string    _t_str09         ;
   /*  76 */   internal string    _t_str10         ;
   /*  77 */   internal string    _t_str11         ;
   /*  78 */   internal string    _t_str12         ;
   /*  79 */   internal string    _t_str13         ;
   /*  70 */   internal string    _t_str14         ;
   /*  81 */   internal string    _t_str15         ;
   /*  82 */   internal string    _t_str16         ;
   /*  83 */   internal string    _t_str17         ;
   /*  84 */   internal string    _t_str18         ;
   /*  85 */   internal string    _t_str19         ;
   /*  86 */   internal string    _t_str20         ;
   /*  87 */   internal string    _t_str21         ;
   /*  88 */   internal string    _t_str22         ;
   /*  89 */   internal string    _t_str23         ;
   /*  90 */   internal string    _t_str24         ;
   /*  91 */   internal string    _t_str25         ;
   /*  92 */   internal string    _t_str26         ;
   /*  93 */   internal string    _t_str27         ;
   /*  94 */   internal string    _t_str28         ;
   /*  95 */   internal string    _t_str29         ;
   /*  96 */   internal string    _t_str30         ;
   /*  97 */   internal string    _t_str31         ;
   /*  98 */   internal decimal   _t_dec01_2       ;
   /*  99 */   internal decimal   _t_dec02_2       ;
   /* 100 */   internal decimal   _t_dec03_2       ;
   /* 101 */   internal decimal   _t_dec04_2       ;
   /* 102 */   internal decimal   _t_dec05_2       ;
   /* 103 */   internal decimal   _t_dec06_2       ;
   /* 104 */   internal decimal   _t_dec07_2       ;
   /* 105 */   internal decimal   _t_dec08_2       ;
   /* 106 */   internal decimal   _t_dec09_2       ;
   /* 107 */   internal decimal   _t_dec10_2       ;
   /* 108 */   internal decimal   _t_dec11_2       ;
   /* 109 */   internal decimal   _t_dec12_2       ;
   /* 110 */   internal decimal   _t_dec13_2       ;
   /* 111 */   internal decimal   _t_dec14_2       ;
   /* 112 */   internal decimal   _t_dec15_2       ;
   /* 113 */   internal decimal   _t_dec16_2       ;
   /* 114 */   internal decimal   _t_dec17_2       ;
   /* 115 */   internal decimal   _t_dec18_2       ;
   /* 116 */   internal decimal   _t_dec19_2       ;
   /* 117 */   internal decimal   _t_dec20_2       ;
   /* 118 */   internal decimal   _t_dec21_2       ;
   /* 119 */   internal decimal   _t_dec22_2       ;
   /* 120 */   internal decimal   _t_dec23_2       ;
   /* 121 */   internal decimal   _t_dec24_2       ;
   /* 122 */   internal decimal   _t_dec25_2       ;
   /* 123 */   internal decimal   _t_dec26_2       ;
   /* 124 */   internal decimal   _t_dec27_2       ;
   /* 125 */   internal decimal   _t_dec28_2       ;
   /* 126 */   internal decimal   _t_dec29_2       ;
   /* 127 */   internal decimal   _t_dec30_2       ;
   /* 128 */   internal decimal   _t_dec31_2       ;
   
   /* 129 */   internal DateTime  _t_date3         ;  
   /* 130 */   internal DateTime  _t_date4         ;
}

public struct XtransResultStruct
{
  internal decimal _t_FUSE_calcProperty;	   
}

#endregion struct XtransStruct

public class Xtrans : VvTransRecord
{

   #region Fildz

   public const string recordName = "xtrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private XtransStruct currentData;
   private XtransStruct backupData;

   private XtransResultStruct _xtrResult;

   #endregion Fildz

   #region Constructors

   public Xtrans() : this(0)
   {
   }

   public Xtrans(uint ID) : base()
   {
      this.currentData = new XtransStruct();

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

      /*  01 */ this.currentData._t_parentID         = 0;
      /*  02 */ this.currentData._t_dokNum           = 0;
      /*  03 */ this.currentData._t_serial           = 0;
      /*  04 */ this.currentData._t_dokDate          = DateTime.MinValue;
      /*  05 */ this.currentData._t_tt               = "";
      /*  06 */ this.currentData._t_ttNum            = 0;
      /*  07 */ this.currentData._t_moneyA           = 0.00M;
      /*  08 */ this.currentData._t_opis_128         = "";
      /*  09 */ this.currentData._t_kol              = 0.00M;
      /*  10 */ this.currentData._t_kpdbNameA_50     = ""; 
      /*  11 */ this.currentData._t_kpdbUlBrA_32     = ""; 
      /*  12 */ this.currentData._t_kpdbMjestoA_32   = ""; 
      /*  13 */ this.currentData._t_kpdbZiroA_32     = ""; 
      /*  14 */ this.currentData._t_kpdbNameB_50     = ""; 
      /*  15 */ this.currentData._t_kpdbUlBrB_32     = ""; 
      /*  16 */ this.currentData._t_kpdbMjestoB_32   = ""; 
      /*  17 */ this.currentData._t_kpdbZiroB_32     = ""; 
      /*  18 */ this.currentData._t_dateOd           = DateTime.MinValue;
      /*  19 */ this.currentData._t_dateDo           = DateTime.MinValue;
      /*  10 */ this.currentData._t_strA_2           = ""; 
      /*  11 */ this.currentData._t_strB_2           = ""; 
      /*  12 */ this.currentData._t_vezniDokA_64     = ""; 
      /*  13 */ this.currentData._t_vezniDokB_64     = ""; 
      /*  24 */ this.currentData._t_strC_2           = ""; 
      /*  25 */ this.currentData._t_kupdobCD         = 0;
      /*  26 */ this.currentData._t_artiklCD         = "";
      /*  27 */ this.currentData._t_artiklName       = "";
      /*  28 */ this.currentData._t_isXxx            = false;
      /*  29 */ this.currentData._t_intA             = 0;
      /*  30 */ this.currentData._t_intB             = 0;
      /*  31 */ this.currentData._t_konto            = "";
      /*  32 */ this.currentData._t_personCD         = 0;
      /*  33 */ this.currentData._t_moneyB           = 0.00M;
      /*  34 */ this.currentData._t_moneyC           = 0.00M;
      /*  35 */ this.currentData._t_moneyD           = 0.00M;
          
      /*  36 */ this.currentData._t_dec01            = 0.00M;
      /*  37 */ this.currentData._t_dec02            = 0.00M;
      /*  38 */ this.currentData._t_dec03            = 0.00M;
      /*  39 */ this.currentData._t_dec04            = 0.00M;
      /*  40 */ this.currentData._t_dec05            = 0.00M;
      /*  41 */ this.currentData._t_dec06            = 0.00M;
      /*  42 */ this.currentData._t_dec07            = 0.00M;
      /*  43 */ this.currentData._t_dec08            = 0.00M;
      /*  44 */ this.currentData._t_dec09            = 0.00M;
      /*  45 */ this.currentData._t_dec10            = 0.00M;
      /*  46 */ this.currentData._t_dec11            = 0.00M;
      /*  47 */ this.currentData._t_dec12            = 0.00M;
      /*  48 */ this.currentData._t_dec13            = 0.00M;
      /*  49 */ this.currentData._t_dec14            = 0.00M;
      /*  50 */ this.currentData._t_dec15            = 0.00M;
      /*  51 */ this.currentData._t_dec16            = 0.00M;
      /*  52 */ this.currentData._t_dec17            = 0.00M;
      /*  53 */ this.currentData._t_dec18            = 0.00M;
      /*  54 */ this.currentData._t_dec19            = 0.00M;
      /*  55 */ this.currentData._t_dec20            = 0.00M;
      /*  56 */ this.currentData._t_dec21            = 0.00M;
      /*  57 */ this.currentData._t_dec22            = 0.00M;
      /*  58 */ this.currentData._t_dec23            = 0.00M;
      /*  59 */ this.currentData._t_dec24            = 0.00M;
      /*  60 */ this.currentData._t_dec25            = 0.00M;
      /*  61 */ this.currentData._t_dec26            = 0.00M;
      /*  62 */ this.currentData._t_dec27            = 0.00M;
      /*  63 */ this.currentData._t_dec28            = 0.00M;
      /*  64 */ this.currentData._t_dec29            = 0.00M;
      /*  65 */ this.currentData._t_dec30            = 0.00M;
      /*  66 */ this.currentData._t_dec31            = 0.00M;
      /*  67 */ this.currentData._t_str01            = "";
      /*  68 */ this.currentData._t_str02            = "";
      /*  69 */ this.currentData._t_str03            = "";
      /*  70 */ this.currentData._t_str04            = "";
      /*  71 */ this.currentData._t_str05            = "";
      /*  72 */ this.currentData._t_str06            = "";
      /*  73 */ this.currentData._t_str07            = "";
      /*  74 */ this.currentData._t_str08            = "";
      /*  75 */ this.currentData._t_str09            = "";
      /*  76 */ this.currentData._t_str10            = "";
      /*  77 */ this.currentData._t_str11            = "";
      /*  78 */ this.currentData._t_str12            = "";
      /*  79 */ this.currentData._t_str13            = "";
      /*  70 */ this.currentData._t_str14            = "";
      /*  81 */ this.currentData._t_str15            = "";
      /*  82 */ this.currentData._t_str16            = "";
      /*  83 */ this.currentData._t_str17            = "";
      /*  84 */ this.currentData._t_str18            = "";
      /*  85 */ this.currentData._t_str19            = "";
      /*  86 */ this.currentData._t_str20            = "";
      /*  87 */ this.currentData._t_str21            = "";
      /*  88 */ this.currentData._t_str22            = "";
      /*  89 */ this.currentData._t_str23            = "";
      /*  90 */ this.currentData._t_str24            = "";
      /*  91 */ this.currentData._t_str25            = "";
      /*  92 */ this.currentData._t_str26            = "";
      /*  93 */ this.currentData._t_str27            = "";
      /*  94 */ this.currentData._t_str28            = "";
      /*  95 */ this.currentData._t_str29            = "";
      /*  96 */ this.currentData._t_str30            = "";
      /*  97 */ this.currentData._t_str31            = "";
      /*  98 */ this.currentData._t_dec01_2          = 0.00M;
      /*  99 */ this.currentData._t_dec02_2          = 0.00M;
      /* 100 */ this.currentData._t_dec03_2          = 0.00M;
      /* 101 */ this.currentData._t_dec04_2          = 0.00M;
      /* 102 */ this.currentData._t_dec05_2          = 0.00M;
      /* 103 */ this.currentData._t_dec06_2          = 0.00M;
      /* 104 */ this.currentData._t_dec07_2          = 0.00M;
      /* 105 */ this.currentData._t_dec08_2          = 0.00M;
      /* 106 */ this.currentData._t_dec09_2          = 0.00M;
      /* 107 */ this.currentData._t_dec10_2          = 0.00M;
      /* 108 */ this.currentData._t_dec11_2          = 0.00M;
      /* 109 */ this.currentData._t_dec12_2          = 0.00M;
      /* 110 */ this.currentData._t_dec13_2          = 0.00M;
      /* 111 */ this.currentData._t_dec14_2          = 0.00M;
      /* 112 */ this.currentData._t_dec15_2          = 0.00M;
      /* 113 */ this.currentData._t_dec16_2          = 0.00M;
      /* 114 */ this.currentData._t_dec17_2          = 0.00M;
      /* 115 */ this.currentData._t_dec18_2          = 0.00M;
      /* 116 */ this.currentData._t_dec19_2          = 0.00M;
      /* 117 */ this.currentData._t_dec20_2          = 0.00M;
      /* 118 */ this.currentData._t_dec21_2          = 0.00M;
      /* 119 */ this.currentData._t_dec22_2          = 0.00M;
      /* 120 */ this.currentData._t_dec23_2          = 0.00M;
      /* 121 */ this.currentData._t_dec24_2          = 0.00M;
      /* 122 */ this.currentData._t_dec25_2          = 0.00M;
      /* 123 */ this.currentData._t_dec26_2          = 0.00M;
      /* 124 */ this.currentData._t_dec27_2          = 0.00M;
      /* 125 */ this.currentData._t_dec28_2          = 0.00M;
      /* 126 */ this.currentData._t_dec29_2          = 0.00M;
      /* 127 */ this.currentData._t_dec30_2          = 0.00M;
      /* 128 */ this.currentData._t_dec31_2          = 0.00M;

      /* 129 */ this.currentData._t_date3            = DateTime.MinValue;
      /* 130 */ this.currentData._t_date4            = DateTime.MinValue;
   }

   #endregion Constructors

   #region Sorters

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
      //get { return Xtrans.sorter_Person_DokDate_DokNum; }
      get
      {
         return Rtrans.sorterArtiklCD;
         //throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/
      }
   }

   protected static System.Data.DataTable TheSchemaTable = ZXC.XtransDao.TheSchemaTable;
   protected static XtransDao.XtransCI CI = ZXC.XtransDao.CI;

   public static VvSQL.RecordSorter sorterKDCnaziv = new VvSQL.RecordSorter(Xtrans.recordName, Xtrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_kpdbNameA_50]),
       //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Naziv", VvSQL.SorterType.KCDnaziv, false);


   #endregion Sorters

   #region propertiz

   internal XtransStruct CurrentData // cijela XtransStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.XtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Xtrans.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ot"; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Xtrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Mixer.recordName; }
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

   public static string MixerForeignKey
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
      set { this.currentData._t_tt = value; }
   }
   /* 06 */ public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public decimal T_moneyA
   {
      get { return this.currentData._t_moneyA; }
      set {        this.currentData._t_moneyA = value; }
   }
   /* 08 */ public string T_opis_128
   {
      get { return this.currentData._t_opis_128; }
      set {        this.currentData._t_opis_128 = value; }
   }
   /* 09 */ public decimal T_kol
   {
      get { return this.currentData._t_kol; }
      set {        this.currentData._t_kol = value; }
   }
   /* 10 */ public string T_kpdbNameA_50
   {
      get { return this.currentData._t_kpdbNameA_50; }
      set {        this.currentData._t_kpdbNameA_50 = value; }
   }
   /* 11 */ public string T_kpdbUlBrA_32
   {
      get { return this.currentData._t_kpdbUlBrA_32; }
      set {        this.currentData._t_kpdbUlBrA_32 = value; }
   }
   /* 12 */ public string T_kpdbMjestoA_32
   {
      get { return this.currentData._t_kpdbMjestoA_32; }
      set {        this.currentData._t_kpdbMjestoA_32 = value; }
   }
   /* 13 */ public string T_kpdbZiroA_32
   {
      get { return this.currentData._t_kpdbZiroA_32; }
      set {        this.currentData._t_kpdbZiroA_32 = value; }
   }
   /* 14 */ public string T_kpdbNameB_50
   {
      get { return this.currentData._t_kpdbNameB_50; }
      set {        this.currentData._t_kpdbNameB_50 = value; }
   }
   /* 15 */ public string T_kpdbUlBrB_32
   {
      get { return this.currentData._t_kpdbUlBrB_32; }
      set {        this.currentData._t_kpdbUlBrB_32 = value; }
   }
   /* 16 */ public string T_kpdbMjestoB_32
   {
      get { return this.currentData._t_kpdbMjestoB_32; }
      set {        this.currentData._t_kpdbMjestoB_32 = value; }
   }
   /* 17 */ public string T_kpdbZiroB_32
   {
      get { return this.currentData._t_kpdbZiroB_32; }
      set {        this.currentData._t_kpdbZiroB_32 = value; }
   }

   //================================================================ 
   //= DateOd - DateDo trtMrt  START ================================ 
   //================================================================ 

   /* 18 */ public DateTime T_dateOd
   {
      get { return this.currentData._t_dateOd; }
      set {        this.currentData._t_dateOd = value; }
   }
   /* 19 */ public DateTime T_dateDo
   {
      get { return this.currentData._t_dateDo; }
      set {        this.currentData._t_dateDo = value; }
   }

   public DateTime T_dateOd_RAD // daj DateTime novoKomponovan od Datuma od T_dokDate-a, a Time-a od T_dateOd  (treba za TT_RVR) 
   {
      get 
      {
         int ddY  = this.T_dokDate.Year;
         int ddM  = this.T_dokDate.Month;
         int ddD  = this.T_dokDate.Day;
         int dOdH = this.T_dateOd .Hour;
         int dOdM = this.T_dateOd .Minute;

         return new DateTime(ddY, ddM, ddD, dOdH, dOdM, 0); 
      }
   }
   public DateTime T_dateDo_RAD // daj DateTime novoKomponovan od Datuma od T_dokDate-a, a Time-a od T_dateDo (treba za TT_RVR) 
   {
      get 
      {
         int ddY  = this.T_dokDate.Year;
         int ddM  = this.T_dokDate.Month;
         int ddD  = this.T_dokDate.Day;
         int dDoH = this.T_dateDo .Hour;
         int dDoM = this.T_dateDo .Minute;

         return new DateTime(ddY, ddM, ddD, dDoH, dDoM, 0); 
      }
   }
   //================================================================ 
   //= DateOd - DateDo trtMrt  END   ================================ 
   //================================================================ 

   /* 20 */
   public string T_strA_2
   {
      get { return this.currentData._t_strA_2; }
      set {        this.currentData._t_strA_2 = value; }
   }
   /* 21 */ public string T_strB_2
   {
      get { return this.currentData._t_strB_2; }
      set {        this.currentData._t_strB_2 = value; }
   }
   /* 22 */ public string T_vezniDokA_64
   {
      get { return this.currentData._t_vezniDokA_64; }
      set {        this.currentData._t_vezniDokA_64 = value; }
   }
   /* 23 */ public string T_vezniDokB_64
   {
      get { return this.currentData._t_vezniDokB_64; }
      set {        this.currentData._t_vezniDokB_64 = value; }
   }
   /* 24 */ public string T_strC_2
   {
      get { return this.currentData._t_strC_2; }
      set {        this.currentData._t_strC_2 = value; }
   }

   /* 25 */ public uint T_kupdobCD
   {
      get { return this.currentData._t_kupdobCD; }
      set {        this.currentData._t_kupdobCD = value; }
   }

   /* 26 */ public string T_artiklCD
   {
      get { return this.currentData._t_artiklCD; }
      set {        this.currentData._t_artiklCD = value; }
   }

   /* 27 */ public string T_artiklName
   {
      get { return this.currentData._t_artiklName; }
      set {        this.currentData._t_artiklName = value; }
   }

   /* 28 */
   public bool T_isXxx
   {
      get { return this.currentData._t_isXxx; }
      set { this.currentData._t_isXxx = value; }
   }

   /* 29 */
   public int T_intA
   {
      get { return this.currentData._t_intA; }
      set { this.currentData._t_intA = value; }
   }

   /* 30 */
   public int T_intB
   {
      get { return this.currentData._t_intB; }
      set { this.currentData._t_intB = value; }
   }

   /* 31 */
   public string T_konto
   {
      get { return this.currentData._t_konto; }
      set { this.currentData._t_konto = value; }
   }

   /*  32 */ public uint T_personCD
   {
      get { return this.currentData._t_personCD; }
      set {        this.currentData._t_personCD = value; }
   }
       
   /*  33 */ public decimal T_moneyB
   {
      get { return this.currentData._t_moneyB; }
      set {        this.currentData._t_moneyB = value; }
   }
   /*  34 */ public decimal T_moneyC
   {
      get { return this.currentData._t_moneyC; }
      set {        this.currentData._t_moneyC = value; }
   }
   /*  35 */ public decimal T_moneyD   
   {      
      get { return this.currentData._t_moneyD; }      
      set {        this.currentData._t_moneyD = value; }   }
   /*  */
                                                                                                                         
   /*  36 */   public decimal T_dec01   { get { return this.currentData._t_dec01  ; }  set { this.currentData._t_dec01   = value; } }
   /*  37 */   public decimal T_dec02   { get { return this.currentData._t_dec02  ; }  set { this.currentData._t_dec02   = value; } }
   /*  38 */   public decimal T_dec03   { get { return this.currentData._t_dec03  ; }  set { this.currentData._t_dec03   = value; } }
   /*  39 */   public decimal T_dec04   { get { return this.currentData._t_dec04  ; }  set { this.currentData._t_dec04   = value; } }
   /*  40 */   public decimal T_dec05   { get { return this.currentData._t_dec05  ; }  set { this.currentData._t_dec05   = value; } }
   /*  41 */   public decimal T_dec06   { get { return this.currentData._t_dec06  ; }  set { this.currentData._t_dec06   = value; } }
   /*  42 */   public decimal T_dec07   { get { return this.currentData._t_dec07  ; }  set { this.currentData._t_dec07   = value; } }
   /*  43 */   public decimal T_dec08   { get { return this.currentData._t_dec08  ; }  set { this.currentData._t_dec08   = value; } }
   /*  44 */   public decimal T_dec09   { get { return this.currentData._t_dec09  ; }  set { this.currentData._t_dec09   = value; } }
   /*  45 */   public decimal T_dec10   { get { return this.currentData._t_dec10  ; }  set { this.currentData._t_dec10   = value; } }
   /*  46 */   public decimal T_dec11   { get { return this.currentData._t_dec11  ; }  set { this.currentData._t_dec11   = value; } }
   /*  47 */   public decimal T_dec12   { get { return this.currentData._t_dec12  ; }  set { this.currentData._t_dec12   = value; } }
   /*  48 */   public decimal T_dec13   { get { return this.currentData._t_dec13  ; }  set { this.currentData._t_dec13   = value; } }
   /*  49 */   public decimal T_dec14   { get { return this.currentData._t_dec14  ; }  set { this.currentData._t_dec14   = value; } }
   /*  50 */   public decimal T_dec15   { get { return this.currentData._t_dec15  ; }  set { this.currentData._t_dec15   = value; } }
   /*  51 */   public decimal T_dec16   { get { return this.currentData._t_dec16  ; }  set { this.currentData._t_dec16   = value; } }
   /*  52 */   public decimal T_dec17   { get { return this.currentData._t_dec17  ; }  set { this.currentData._t_dec17   = value; } }
   /*  53 */   public decimal T_dec18   { get { return this.currentData._t_dec18  ; }  set { this.currentData._t_dec18   = value; } }
   /*  54 */   public decimal T_dec19   { get { return this.currentData._t_dec19  ; }  set { this.currentData._t_dec19   = value; } }
   /*  55 */   public decimal T_dec20   { get { return this.currentData._t_dec20  ; }  set { this.currentData._t_dec20   = value; } }
   /*  56 */   public decimal T_dec21   { get { return this.currentData._t_dec21  ; }  set { this.currentData._t_dec21   = value; } }
   /*  57 */   public decimal T_dec22   { get { return this.currentData._t_dec22  ; }  set { this.currentData._t_dec22   = value; } }
   /*  58 */   public decimal T_dec23   { get { return this.currentData._t_dec23  ; }  set { this.currentData._t_dec23   = value; } }
   /*  59 */   public decimal T_dec24   { get { return this.currentData._t_dec24  ; }  set { this.currentData._t_dec24   = value; } }
   /*  60 */   public decimal T_dec25   { get { return this.currentData._t_dec25  ; }  set { this.currentData._t_dec25   = value; } }
   /*  61 */   public decimal T_dec26   { get { return this.currentData._t_dec26  ; }  set { this.currentData._t_dec26   = value; } }
   /*  62 */   public decimal T_dec27   { get { return this.currentData._t_dec27  ; }  set { this.currentData._t_dec27   = value; } }
   /*  63 */   public decimal T_dec28   { get { return this.currentData._t_dec28  ; }  set { this.currentData._t_dec28   = value; } }
   /*  64 */   public decimal T_dec29   { get { return this.currentData._t_dec29  ; }  set { this.currentData._t_dec29   = value; } }
   /*  65 */   public decimal T_dec30   { get { return this.currentData._t_dec30  ; }  set { this.currentData._t_dec30   = value; } }
   /*  66 */   public decimal T_dec31   { get { return this.currentData._t_dec31  ; }  set { this.currentData._t_dec31   = value; } }
   /*  67 */   public string  T_str01   { get { return this.currentData._t_str01  ; }  set { this.currentData._t_str01   = value; } }
   /*  68 */   public string  T_str02   { get { return this.currentData._t_str02  ; }  set { this.currentData._t_str02   = value; } }
   /*  69 */   public string  T_str03   { get { return this.currentData._t_str03  ; }  set { this.currentData._t_str03   = value; } }
   /*  70 */   public string  T_str04   { get { return this.currentData._t_str04  ; }  set { this.currentData._t_str04   = value; } }
   /*  71 */   public string  T_str05   { get { return this.currentData._t_str05  ; }  set { this.currentData._t_str05   = value; } }
   /*  72 */   public string  T_str06   { get { return this.currentData._t_str06  ; }  set { this.currentData._t_str06   = value; } }
   /*  73 */   public string  T_str07   { get { return this.currentData._t_str07  ; }  set { this.currentData._t_str07   = value; } }
   /*  74 */   public string  T_str08   { get { return this.currentData._t_str08  ; }  set { this.currentData._t_str08   = value; } }
   /*  75 */   public string  T_str09   { get { return this.currentData._t_str09  ; }  set { this.currentData._t_str09   = value; } }
   /*  76 */   public string  T_str10   { get { return this.currentData._t_str10  ; }  set { this.currentData._t_str10   = value; } }
   /*  77 */   public string  T_str11   { get { return this.currentData._t_str11  ; }  set { this.currentData._t_str11   = value; } }
   /*  78 */   public string  T_str12   { get { return this.currentData._t_str12  ; }  set { this.currentData._t_str12   = value; } }
   /*  79 */   public string  T_str13   { get { return this.currentData._t_str13  ; }  set { this.currentData._t_str13   = value; } }
   /*  70 */   public string  T_str14   { get { return this.currentData._t_str14  ; }  set { this.currentData._t_str14   = value; } }
   /*  81 */   public string  T_str15   { get { return this.currentData._t_str15  ; }  set { this.currentData._t_str15   = value; } }
   /*  82 */   public string  T_str16   { get { return this.currentData._t_str16  ; }  set { this.currentData._t_str16   = value; } }
   /*  83 */   public string  T_str17   { get { return this.currentData._t_str17  ; }  set { this.currentData._t_str17   = value; } }
   /*  84 */   public string  T_str18   { get { return this.currentData._t_str18  ; }  set { this.currentData._t_str18   = value; } }
   /*  85 */   public string  T_str19   { get { return this.currentData._t_str19  ; }  set { this.currentData._t_str19   = value; } }
   /*  86 */   public string  T_str20   { get { return this.currentData._t_str20  ; }  set { this.currentData._t_str20   = value; } }
   /*  87 */   public string  T_str21   { get { return this.currentData._t_str21  ; }  set { this.currentData._t_str21   = value; } }
   /*  88 */   public string  T_str22   { get { return this.currentData._t_str22  ; }  set { this.currentData._t_str22   = value; } }
   /*  89 */   public string  T_str23   { get { return this.currentData._t_str23  ; }  set { this.currentData._t_str23   = value; } }
   /*  90 */   public string  T_str24   { get { return this.currentData._t_str24  ; }  set { this.currentData._t_str24   = value; } }
   /*  91 */   public string  T_str25   { get { return this.currentData._t_str25  ; }  set { this.currentData._t_str25   = value; } }
   /*  92 */   public string  T_str26   { get { return this.currentData._t_str26  ; }  set { this.currentData._t_str26   = value; } }
   /*  93 */   public string  T_str27   { get { return this.currentData._t_str27  ; }  set { this.currentData._t_str27   = value; } }
   /*  94 */   public string  T_str28   { get { return this.currentData._t_str28  ; }  set { this.currentData._t_str28   = value; } }
   /*  95 */   public string  T_str29   { get { return this.currentData._t_str29  ; }  set { this.currentData._t_str29   = value; } }
   /*  96 */   public string  T_str30   { get { return this.currentData._t_str30  ; }  set { this.currentData._t_str30   = value; } }
   /*  97 */   public string  T_str31   { get { return this.currentData._t_str31  ; }  set { this.currentData._t_str31   = value; } }
   /*  98 */   public decimal T_dec01_2 { get { return this.currentData._t_dec01_2; }  set { this.currentData._t_dec01_2 = value; } }
   /*  99 */   public decimal T_dec02_2 { get { return this.currentData._t_dec02_2; }  set { this.currentData._t_dec02_2 = value; } }
   /* 100 */   public decimal T_dec03_2 { get { return this.currentData._t_dec03_2; }  set { this.currentData._t_dec03_2 = value; } }
   /* 101 */   public decimal T_dec04_2 { get { return this.currentData._t_dec04_2; }  set { this.currentData._t_dec04_2 = value; } }
   /* 102 */   public decimal T_dec05_2 { get { return this.currentData._t_dec05_2; }  set { this.currentData._t_dec05_2 = value; } }
   /* 103 */   public decimal T_dec06_2 { get { return this.currentData._t_dec06_2; }  set { this.currentData._t_dec06_2 = value; } }
   /* 104 */   public decimal T_dec07_2 { get { return this.currentData._t_dec07_2; }  set { this.currentData._t_dec07_2 = value; } }
   /* 105 */   public decimal T_dec08_2 { get { return this.currentData._t_dec08_2; }  set { this.currentData._t_dec08_2 = value; } }
   /* 106 */   public decimal T_dec09_2 { get { return this.currentData._t_dec09_2; }  set { this.currentData._t_dec09_2 = value; } }
   /* 107 */   public decimal T_dec10_2 { get { return this.currentData._t_dec10_2; }  set { this.currentData._t_dec10_2 = value; } }
   /* 108 */   public decimal T_dec11_2 { get { return this.currentData._t_dec11_2; }  set { this.currentData._t_dec11_2 = value; } }
   /* 109 */   public decimal T_dec12_2 { get { return this.currentData._t_dec12_2; }  set { this.currentData._t_dec12_2 = value; } }
   /* 110 */   public decimal T_dec13_2 { get { return this.currentData._t_dec13_2; }  set { this.currentData._t_dec13_2 = value; } }
   /* 111 */   public decimal T_dec14_2 { get { return this.currentData._t_dec14_2; }  set { this.currentData._t_dec14_2 = value; } }
   /* 112 */   public decimal T_dec15_2 { get { return this.currentData._t_dec15_2; }  set { this.currentData._t_dec15_2 = value; } }
   /* 113 */   public decimal T_dec16_2 { get { return this.currentData._t_dec16_2; }  set { this.currentData._t_dec16_2 = value; } }
   /* 114 */   public decimal T_dec17_2 { get { return this.currentData._t_dec17_2; }  set { this.currentData._t_dec17_2 = value; } }
   /* 115 */   public decimal T_dec18_2 { get { return this.currentData._t_dec18_2; }  set { this.currentData._t_dec18_2 = value; } }
   /* 116 */   public decimal T_dec19_2 { get { return this.currentData._t_dec19_2; }  set { this.currentData._t_dec19_2 = value; } }
   /* 117 */   public decimal T_dec20_2 { get { return this.currentData._t_dec20_2; }  set { this.currentData._t_dec20_2 = value; } }
   /* 118 */   public decimal T_dec21_2 { get { return this.currentData._t_dec21_2; }  set { this.currentData._t_dec21_2 = value; } }
   /* 119 */   public decimal T_dec22_2 { get { return this.currentData._t_dec22_2; }  set { this.currentData._t_dec22_2 = value; } }
   /* 120 */   public decimal T_dec23_2 { get { return this.currentData._t_dec23_2; }  set { this.currentData._t_dec23_2 = value; } }
   /* 121 */   public decimal T_dec24_2 { get { return this.currentData._t_dec24_2; }  set { this.currentData._t_dec24_2 = value; } }
   /* 122 */   public decimal T_dec25_2 { get { return this.currentData._t_dec25_2; }  set { this.currentData._t_dec25_2 = value; } }
   /* 123 */   public decimal T_dec26_2 { get { return this.currentData._t_dec26_2; }  set { this.currentData._t_dec26_2 = value; } }
   /* 124 */   public decimal T_dec27_2 { get { return this.currentData._t_dec27_2; }  set { this.currentData._t_dec27_2 = value; } }
   /* 125 */   public decimal T_dec28_2 { get { return this.currentData._t_dec28_2; }  set { this.currentData._t_dec28_2 = value; } }
   /* 126 */   public decimal T_dec29_2 { get { return this.currentData._t_dec29_2; }  set { this.currentData._t_dec29_2 = value; } }
   /* 127 */   public decimal T_dec30_2 { get { return this.currentData._t_dec30_2; }  set { this.currentData._t_dec30_2 = value; } }
   /* 128 */   public decimal T_dec31_2 { get { return this.currentData._t_dec31_2; }  set { this.currentData._t_dec31_2 = value; } }

   /* 129 */ public DateTime T_date3
   {
      get { return this.currentData._t_date3; }
      set {        this.currentData._t_date3 = value; }
   }
   /* 130*/ public DateTime T_date4
   {
      get { return this.currentData._t_date4; }
      set {        this.currentData._t_date4 = value; }
   }

 
   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " TT: "     + T_TT + "-" + T_ttNum + " (" + T_dokDate.ToShortDateString() + ")" +
             " Ser: "    + T_serial + String.Format(" k:{0:N} kn:{1:N}", T_kol, T_moneyA);
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Xtrans xtrans_rec = (Xtrans)vvDataRecord;

    //if(restrictor == ZXC.AutoCompleteRestrictor.KPL_Analitika_Only && xtrans_rec.Tip != "A") return "";

      switch(sifrarType)
      {
         case VvSQL.SorterType.KCDnaziv: return xtrans_rec.T_kpdbNameA_50;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Kplan.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<XtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<XtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<XtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<XtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<XtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Xtrans newObject = new Xtrans();

      Generic_CloneData<XtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      newObject.R_moneyA = this.R_moneyA;
      newObject.R_moneyB = this.R_moneyB;

      return newObject;
   }

   public Xtrans MakeDeepCopy()
   {
      return (Xtrans)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Xtrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Xtrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Xtrans)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory

   #region CalcTransResults()

   public override void CalcTransResults(VvDocumentRecord _vvDocumentRecord)
   {
      Mixer mixer_rec = _vvDocumentRecord as Mixer;

      // FUSE 

      return;

      switch(mixer_rec.TT)
      {
         case Mixer.TT_PUTN_T:
         case Mixer.TT_PUTN_I:
         case Mixer.TT_PUTN_L: CalcPutNalXtransResults(mixer_rec); break;
      }
   }

   // FUSE 
   private void CalcPutNalXtransResults(Mixer mixer_rec)
   {
      R_FUSE_calcProperty = 123.123M;
   }

   #endregion CalcTransResults()

   #region Result Propertiz

   #region Blabla

   public XtransResultStruct XtrResults
   {
      get { return this._xtrResult;         }
      set {        this._xtrResult = value; }
   }

   public decimal R_KolMoneyA { get { return this.T_kol * this.T_moneyA; } }

   public decimal R_FUSE_calcProperty
   {
      get { return this._xtrResult._t_FUSE_calcProperty; }
      set {        this._xtrResult._t_FUSE_calcProperty = value; }
   }

   public decimal R_KolMoneyABC { get { return this.R_KolMoneyA + this.T_moneyB + this.T_moneyC; } }

   public decimal R_SumMoney    { get { return this.T_moneyA + this.T_moneyB + this.T_moneyC + this.T_moneyD + this.T_dec01 + this.T_dec02 + this.T_dec03; } } // suma svih fondova na PlanDUC-u
   
   public decimal R_IndeksBA    { get { return (ZXC.DivSafe(this.RptFINA_moneyB, this.RptFINA_moneyA) * 100.00M ); } }

   public decimal R_moneyA { get; set; }
   public decimal R_moneyB { get; set; }

   public decimal RptFINA_moneyA { get { return this.T_strA_2 == "S"   ? this.R_moneyA :   this.T_moneyA; } }
   public decimal RptFINA_moneyB { get { return /*this.T_strA_2 == "N" ? this.T_moneyB :*/ this.R_moneyB; } } // ipak jer i N nastaje kao result onoga sto je uneseno u pravila

   public uint    R_externTtNum { get; set; }
   public uint    R_externKCD   { get; set; }
   public decimal R_externCij   { get; set; }

   #endregion Blabla

   #region RVR & MVR Propertiz

   #region RVR props

   public decimal RVR_4  { get; set; }
   public decimal RVR_4a { get; set; }// only 4 old RVR
   public decimal RVR_4b { get; set; }// only 4 old RVR
   public decimal RVR_4c { get; set; }// only 4 old RVR
   public decimal RVR_4d { get; set; }// only 4 old RVR
   public decimal RVR_5  { get; set; }
   public decimal RVR_6  { get; set; }
   public decimal RVR_7  { get; set; }
   public decimal RVR_8  { get; set; }
   public decimal RVR_9  { get; set; }
   public decimal RVR_10 { get; set; }
   public decimal RVR_11 { get; set; }
   public decimal RVR_12 { get; set; }
   public decimal RVR_13 { get; set; }
   public decimal RVR_14 { get; set; }
   public decimal RVR_15 { get; set; }
   public decimal RVR_16 { get; set; }
   public decimal RVR_17 { get; set; }
   public decimal RVR_18 { get; set; }
   public decimal RVR_19 { get; set; }
   public decimal RVR_20 { get; set; }
   public decimal RVR_21 { get; set; }
   public decimal RVR_22 { get; set; }
   public decimal RVR_23 { get; set; }
   public decimal RVR_24 { get; set; }
   public decimal RVR_25 { get; set; }
   public decimal RVR_26 { get; set; }
   public decimal RVR_27 { get; set; }
   public decimal RVR_28 { get; set; }
   public decimal RVR_29 { get; set; }
   public decimal RVR_30 { get; set; }
   public decimal RVR_31 { get; set; }
   public decimal RVR_32 { get; set; }
   public decimal RVR_33 { get; set; }
   
   public string RvrCd_as_PlacaEvrVrCd
   {
      get
      {
         // 26.09.2014: 
         if(this.T_strA_2.IsEmpty()) return "01";
         
         // 01.06.2015.
         string placaEvrVrCd = (this.T_dokDate < ZXC.Date01042015) ? ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd(this.T_strA_2).ToString("00") : 
                                                                     ZXC.luiListaMixRadVrijemRVR2.GetIntegerForThisCd(this.T_strA_2).ToString("00");

       //return ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd(this.T_strA_2).ToString("00");
         return placaEvrVrCd;
      }
   }
   public string MvrCd_as_PlacaMvrVrCd
   {
      get
      {
         if(this.T_strA_2.IsEmpty()) return "01";

         return ZXC.luiListaMixRadVrijemeMVR.GetIntegerForThisCd(this.T_strA_2).ToString("00");
      }
   }

   #endregion RVR props

   #region MVR props

   public List<ZXC.NameAndDecimal_CommonStruct> /*Get*/MVRsVRlist
   {
      get
      {
         List<ZXC.NameAndDecimal_CommonStruct> MVRsVRlist = new List<ZXC.NameAndDecimal_CommonStruct>();

         int daysInMonth = DateTime.DaysInMonth(this.T_dokDate.Year, this.T_dokDate.Month);

         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str01, this.T_dec01));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str02, this.T_dec02));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str03, this.T_dec03));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str04, this.T_dec04));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str05, this.T_dec05));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str06, this.T_dec06));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str07, this.T_dec07));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str08, this.T_dec08));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str09, this.T_dec09));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str10, this.T_dec10));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str11, this.T_dec11));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str12, this.T_dec12));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str13, this.T_dec13));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str14, this.T_dec14));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str15, this.T_dec15));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str16, this.T_dec16));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str17, this.T_dec17));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str18, this.T_dec18));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str19, this.T_dec19));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str20, this.T_dec20));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str21, this.T_dec21));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str22, this.T_dec22));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str23, this.T_dec23));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str24, this.T_dec24));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str25, this.T_dec25));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str26, this.T_dec26));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str27, this.T_dec27));
         MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str28, this.T_dec28));

         if(daysInMonth > 28) MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str29, this.T_dec29));
         if(daysInMonth > 29) MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str30, this.T_dec30));
         if(daysInMonth > 30) MVRsVRlist.Add(new ZXC.NameAndDecimal_CommonStruct(this.T_str31, this.T_dec31));

         return MVRsVRlist;
      }
   }

   public decimal MVRsVRlistKolSum
   {
      get
      {
         return MVRsVRlist.Sum(mvr => mvr.TheDecimal);
      }
   }

   public decimal R_MVR_MFS // 'osobni' mjesecni fond predvidjenih radnih sati po prijavljenoj satnici (8satno, 3satno, Xsatno radno vrijeme)
   {
      get
      {
         bool    isTrgovac                     = this.T_isXxx ;
         string  zaMMYYYY                      = this.T_konto ;
         decimal sluzbeniMjesecniFondRadniSati = Math.Abs(this.T_moneyB); //17.06.2015. zbog statistike jer moze biti negativan 
         decimal sluzbeniDnevniFondRadniSati   = ZXC.GetSluzbeniDnevniFondRadniSati(isTrgovac, zaMMYYYY, sluzbeniMjesecniFondRadniSati);

         uint radniDaniZaMMYYYY = ZXC.GetMFD(sluzbeniMjesecniFondRadniSati, sluzbeniDnevniFondRadniSati);

         return radniDaniZaMMYYYY * this.T_moneyA; // u T_moneyA se krije zadnje detektirani 'radnikov' prevPtrans_rec.T_dnFondSati via MixerDUC.GetLastPersonsPtransT_dfs 
      }
   }

   public decimal R_MVR_RFS // realno evidentirano radnih sati u mjesecu 
   {
      get
      {
         return MVRsVRlist.Sum(mvr => mvr.TheDecimal);
      }
   }

   public decimal R_MVR_PFS // izracunati prekovremeni radni sati 
   {
      get
      {
         decimal prekovremeni = this.R_MVR_RFS - this.R_MVR_MFS;

         return prekovremeni.IsPositive() ? prekovremeni : 0M;
      }
   }

   public decimal GetMonthlyPFS(List<Xtrans> everibodysMVR_XtransList)
   {
      var thisPersonXtransList = everibodysMVR_XtransList.Where(xtr => xtr.T_personCD == this.T_personCD);

      if(thisPersonXtransList.Any(xtr => xtr.T_moneyD.NotZero())) return 0M; // dodaj pfs samo jednoj pojavi ovog persona 

      decimal PrekovremeniFS, RealniFS;

      var thisPersonUnitedMVRsVRlist = thisPersonXtransList.SelectMany(xtr => xtr.MVRsVRlist);

      RealniFS = thisPersonUnitedMVRsVRlist.Sum(mvr => mvr.TheDecimal);

      PrekovremeniFS = RealniFS - this.R_MVR_MFS;

      return PrekovremeniFS;
   }

   #endregion MVR props

   #endregion RVR & MVR Propertiz

   #endregion Result Propertiz

   #region AVR - 2017 News

   // Primjer kada zastitar radi od 18.00. do 06.00 drugi dan: 
   // ovdje se dolazi 3 puta - treba nastati 3 RVRxtransa      
   // 18.00h - 22.00h                                          
   // 22.00h - 00.00h                                          
   // 00.00h - 06.00h (next day)                               

   internal static Xtrans Get_RVRxtrans_from_AVRdata(uint personCD, string personIme, string personPrezime, 

      string  MMYYYY    , 
      string  dayInMonth, 
      decimal vrijemeOD , 
      decimal vrijemeDO , 
      string  vrstaRada )
   {
      Xtrans RVRxtrans_rec = new Xtrans();

      RVRxtrans_rec.T_TT           = Mixer.TT_RVR ;
      RVRxtrans_rec.T_personCD     = personCD     ; 
      RVRxtrans_rec.T_kpdbNameB_50 = personIme    ; 
      RVRxtrans_rec.T_kpdbNameA_50 = personPrezime; 

      RVRxtrans_rec.T_dokDate = Placa.GetDateTimeFromMMYYYY(MMYYYY, false, dayInMonth);

      #region T_dateOd - T_dateDo

      int ddY  = RVRxtrans_rec.T_dokDate.Year        ;
      int ddM  = RVRxtrans_rec.T_dokDate.Month       ;
      int ddD  = RVRxtrans_rec.T_dokDate.Day         ;
      int dOdH = GetHourPart_fromDecimal  (vrijemeOD);
      int dOdM = GetMinutePart_fromDecimal(vrijemeOD);

      RVRxtrans_rec.T_dateOd =  new DateTime(ddY, ddM, ddD, dOdH, dOdM, 0); 

          dOdH = GetHourPart_fromDecimal  (vrijemeDO);
          dOdM = GetMinutePart_fromDecimal(vrijemeDO);

      RVRxtrans_rec.T_dateDo =  new DateTime(ddY, ddM, ddD, dOdH, dOdM, 0); 

      #endregion T_dateOd - T_dateDo

    //RVRxtrans_rec.T_kol = 0M; ovako racunamao na RvrDUC TheG_CellLeave_CalcSati
      TimeSpan ts = RVRxtrans_rec.T_dateDo.Subtract(RVRxtrans_rec.T_dateOd);
      RVRxtrans_rec.T_kol = System.Convert.ToDecimal(ts.TotalHours);

      RVRxtrans_rec.T_strA_2 = vrstaRada;  

      return RVRxtrans_rec;
   }

   private static int GetMinutePart_fromDecimal(decimal vrijemeDO)
   {
      throw new NotImplementedException();
   }

   private static int GetHourPart_fromDecimal(decimal vrijemeDO)
   {
      throw new NotImplementedException();
   }

   #endregion AVR - 2017 News
}