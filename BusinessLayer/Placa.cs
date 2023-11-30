using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct PlacaStruct

public struct PlacaStruct
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
   /* 09 */   internal string   _vrstaObr;
   /* 10 */   internal string /*DateTime*/ _mmyyyy;
   /* 11 */   internal decimal  _fondSati;
   /* 12 */   internal uint     _mtros_cd;
   /* 13 */   internal string   _mtros_tk;
   /* 14 */   internal string   _napomena;
   /* 15 */   internal bool     _flagA;
   /* 16 */   internal string   _rSm_ID;
   /* 17 */   internal bool     _isTrgFondSati; 
   /* 18 */   internal string   _vrstaJOPPD;
   /* 19 */   internal bool     _isLocked; 

              internal PrulesStruct      _pRules;
              //internal PlacaResultStruct _plaResult;
}

public struct PrulesStruct
{
  
   /* 17 */ internal decimal _stpor1    ;
   /* 18 */ internal decimal _stpor2    ;
   /* 19 */ internal decimal _stpor3    ;
   /* 20 */ internal decimal _stpor4    ;
   /* 21 */ internal decimal _osnOdb    ;
   /* 22 */ internal decimal _stMio1stup;
   /* 23 */ internal decimal _stMio2stup;
   /* 24 */ internal decimal _stZdrNa   ;
   /* 25 */ internal decimal _stZorNa   ;
   /* 26 */ internal decimal _stZapNa   ;
   /* 27 */ internal decimal _stZapII   ;
   /* 28 */ internal decimal _minMioOsn ;
   /* 29 */ internal decimal _maxMioOsn ;
   /* 30 */ internal decimal _maxPorOsn1;
   /* 31 */ internal decimal _maxPorOsn2;
   /* 32 */ internal decimal _maxPorOsn3;
   /* 33 */ internal decimal _stZpi     ;
   /* 34 */ internal decimal _stOthOlak ;   //_OTHER_DOHODAK_IZDATAK_PERCENT
   /* 35 */ internal decimal _stDodStaz ;
   /* 36 */ internal uint    _granBrRad ;

   /* 37 */ internal decimal _stMioNaB1 ;
   /* 38 */ internal decimal _stMioNa2B1;
   /* 39 */ internal decimal _stMioNaB2 ;
   /* 40 */ internal decimal _stMioNa2B2;
   /* 41 */ internal decimal _stMioNaB3 ;
   /* 42 */ internal decimal _stMioNa2B3;
   /* 43 */ internal decimal _stMioNaB4 ;
   /* 44 */ internal decimal _stMioNa2B4;
   /* 45 */ internal decimal _prosPlaca ; // notFUSE: ProsPlaca 
   /* 46 */ internal decimal _stMioNa2B5; // FUSE // 30.01.2017. ja bi ovo za clanove uprave:?!
   /* 47 */ internal decimal _stKrizPor1;
   /* 48 */ internal decimal _stKrizPor2;
   /* 50 */ internal decimal _vrKoefBr1 ;

   // 16.01.2019: 
   /* 51 */ internal decimal _stZdrDD   ;

#if nottjetttt
   /* 52 */ internal decimal _mio1Granica1  ;
   /* 53 */ internal decimal _mio1Granica2  ;
   /* 54 */ internal decimal _mio1FiksOlk;
   /* 55 */ internal decimal _mio1KoefOlk;
#endif
}

//public struct PlacaResultStruct
//{
//   //___________PtransStruct

//   /* 01 */  internal decimal  _brutoOsn;   
//   /* 02 */  internal decimal  _topObrok;   
//   /* 03 */  internal decimal  _dodBruto;   
//   /* 04 */  internal decimal  _zivotno;   
//   /* 05 */  internal decimal  _dopZdr;   
//   /* 06 */  internal decimal  _dobMIO;   
//   /* 07 */  internal decimal  _netoAdd;   
//   /* 08 */  internal decimal  _prijevoz;

//   //___________PtransResultStruct

//   /* 09 */ internal decimal _bruto100;	   
//   /* 10 */ internal decimal _theBruto;
//   /* 11 */ internal decimal _mioOsn;   
//   /* 12 */ internal decimal _mioIz;	   
//   /* 13 */ internal decimal _mioIz_II;	
//   /* 14 */ internal decimal _mioAll;	
//   /* 15 */ internal decimal _doprIz;	
//   /* 16 */ internal decimal _odbitak;	
//   /* 17 */ internal decimal _premije;	
//   /* 18 */ internal decimal _dohodak;	
//   /* 19 */ internal decimal _porOsnAll;
//   /* 20 */ internal decimal _porOsn1;	
//   /* 21 */ internal decimal _porOsn2;	
//   /* 22 */ internal decimal _porOsn3;	
//   /* 23 */ internal decimal _porOsn4;	
//   /* 24 */ internal decimal _por1uk;	
//   /* 25 */ internal decimal _por2uk;	
//   /* 26 */ internal decimal _por3uk;	
//   /* 27 */ internal decimal _por4uk;	
//   /* 28 */ internal decimal _porezAll;	
//   /* 29 */ internal decimal _prirez;	
//   /* 30 */ internal decimal _porPrirez;
//   /* 31 */ internal decimal _netto;	   
//   /* 32 */ internal decimal _obustave;	
//   /* 33 */ internal decimal _2Pay;		
//   /* 34 */ internal decimal _naRuke;	
//   /* 35 */ internal decimal _zdrNa;	   
//   /* 36 */ internal decimal _zorNa;	   
//   /* 37 */ internal decimal _zapNa;	   
//   /* 38 */ internal decimal _zapII;    
//   /* 39 */ internal decimal _zapAll;	
//   /* 40 */ internal decimal _doprNa;	
//   /* 41 */ internal decimal _doprAll;  

//   /* 42 */ internal decimal _satiR;	
//   /* 43 */ internal decimal _satiB;  

//}

#endregion struct PlacaStruct

public class Placa : VvPolyDocumRecord
{

   #region Fildz

   public const string recordName       = "placa";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   public const string TT_REDOVANRAD   = "RR"; // redovan rad                                                              
   public const string TT_AUTORHONOR   = "AH"; // autorski honorar                                                         
   public const string TT_NADZORODBOR  = "NO"; // nadzorni odbor                                                           
   public const string TT_PODUZETPLACA = "PP"; // poduzetnicka placa                                                       
   public const string TT_UGOVORODJELU = "UD"; // ugovor o djelu                                                           
   public const string TT_IDD_KOLONA_4 = "K4"; // sportski suci ..                                                         
   public const string TT_AUTORHONUMJ  = "AU"; // autorski honorar umjetnika                                               
   public const string TT_NEOPOREZPRIM = "NP"; // neoporezivi primici                                                      
   public const string TT_PLACAUNARAVI = "PN"; // placa u naravi                                                           
   public const string TT_SEZZAPPOLJOP = "SZ"; // sezonsko zaposljavanje u poljoprivredi 23.01.2014.                       
   public const string TT_POREZNADOBIT = "PD"; // obracun poreza za isplatu dobiti                                         
   public const string TT_STRUCNOOSPOS = "SO"; // 25.11.14. strucno osposobljavanje za rad bez zasnivanja radnog odnosa    
   public const string TT_NEPLACDOPUST = "ND"; // 25.11.14. neplaceni dopust                                               
   public const string TT_TURSITVIJECE = "TV"; // 12.03.15. turisticko vijece                                              
   public const string TT_SAMODOPRINOS = "SD"; // 05.02.16. Samo doprinosi                                                 
   public const string TT_AHSAMOSTUMJ  = "SU"; // autorski honorar SAMOSTALNOG umjetnika kojem drzava placa doprinose      

   public const string TT_DDBEZDOPRINO = "DD"; // drugi dohodak bez obveze doprinosa bez pausala pa se razlikuje od N          
   public const string TT_AUVECASTOPA  = "A2"; // autorski honorar umjetnika po vecoj stopi poreza                             
   public const string TT_NR1_PX1NEDOP = "N1"; // NEREZIDENTI porez X 10% bez pausala, bez doprinosa                           
   public const string TT_NR2_P01NEDOP = "N2"; // NEREZIDENTI porez 1 st1 sa pausalom, bez doprinosa                           
   public const string TT_NR3_PX1DADOP = "N3"; // NEREZIDENTI porez X 10% bez pausala, doprinosi na osn umanjenu za pausal 30% 

   public const string TT_BIVSIRADNIK  = "BR"; // 23.12.2019. Obračun primitaka prema kojima se doprinosi obračunavaju na način koji ima obilježje drugog dohotka, a porez na dohodak prema primitcima od kojih se utvrđuje dohodak od nesamostalnog rada

   // 30.03.2021: za potrebe 'GetAlreadySpentPtransInThisMonth' da/ne, a i za eventualne buduce                     
   // ovo je lista placa_TT ova koji se odnose na zaposlene a ne neke 'vanjske' oblike angazmana koje treba platiti 
   private static string[] arrayRRsetTT = new string[] {
      Placa.TT_REDOVANRAD   /*"RR"*/, // redovan rad                                                              
      Placa.TT_PODUZETPLACA /*"PP"*/, // poduzetnicka placa                                                       
      Placa.TT_PLACAUNARAVI /*"PN"*/, // placa u naravi                                                           
      Placa.TT_NEPLACDOPUST /*"ND"*/, // 25.11.14. neplaceni dopust                                               
      Placa.TT_BIVSIRADNIK  /*"BR"*/, // 23.12.2019. Obračun primitaka prema kojima se doprinosi obračunavaju na način koji ima obilježje drugog dohotka, a porez na dohodak prema primitcima od kojih se utvrđuje dohodak od nesamostalnog rada
   };
   /// <summary>
   /// RR, PP, PN, ND, BR
   /// </summary>
   public bool IsRRsetTT { get { return arrayRRsetTT.Contains(this.TT); } }


   public const string VrObr_NeisplacenaPlaca = "00";
   public const string VrObr_NadoplacenaPlaca = "77";

   private PlacaStruct currentData;
   private PlacaStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.PlacaDao.TheSchemaTable;
   protected static PlacaDao.PlacaCI      CI             = ZXC.PlacaDao.CI;

   public static string rSm_ID_colName = ZXC.PlacaDao.GetSchemaColumnName(CI.rSm_ID);
   public static string tt_colName     = ZXC.PlacaDao.GetSchemaColumnName(CI.tt);

   public const uint SluzbeniDnevniFondSati = 8;

   public const decimal MAXdozvoljeniPrekovrSati = 32M; // TODO: !!!!!!!!!! staviti u Prjkt kada dode neko novi (AG Sjaju je to 32)

   #endregion Fildz

   #region Constructors

   public Placa() : this(0)
   {
   }

   public Placa(uint ID) : base()
   {
      this.currentData = new PlacaStruct();

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
      /* 09 */      this.currentData._vrstaObr     = "";
      /* 10 */      this.currentData._mmyyyy       = "";
      /* 11 */      this.currentData._fondSati     = 0;
      /* 12 */      this.currentData._mtros_cd     = 0;
      /* 13 */      this.currentData._mtros_tk     = "";
      /* 14 */      this.currentData._napomena     = "";
      /* 16 */      this.currentData._rSm_ID       = "";
      /* 17 */      this.currentData._isTrgFondSati = false;
      /* 49 */      this.currentData._vrstaJOPPD    = "";
      /* 19 */      this.currentData._isLocked      = false;

                    //this._transes.Clear();
                    //this.Transes = null;
                    // 21.4.2009: 
                    this.Transes  = new List<Ptrans>();
                    this.Transes2 = new List<Ptrane>();
                    this.Transes3 = new List<Ptrano>();

      /* 17 */     this.currentData._pRules._stpor1     = 
      /* 18 */     this.currentData._pRules._stpor2     = 
      /* 19 */     this.currentData._pRules._stpor3     = 
      /* 20 */     this.currentData._pRules._stpor4     = 
      /* 21 */     this.currentData._pRules._osnOdb     = 
      /* 22 */     this.currentData._pRules._stMio1stup = 
      /* 23 */     this.currentData._pRules._stMio2stup = 
      /* 24 */     this.currentData._pRules._stZdrNa    = 
      /* 25 */     this.currentData._pRules._stZorNa    = 
      /* 26 */     this.currentData._pRules._stZapNa    = 
      /* 27 */     this.currentData._pRules._stZapII    = 
      /* 28 */     this.currentData._pRules._minMioOsn  = 
      /* 29 */     this.currentData._pRules._maxMioOsn  = 
      /* 30 */     this.currentData._pRules._maxPorOsn1 = 
      /* 31 */     this.currentData._pRules._maxPorOsn2 = 
      /* 32 */     this.currentData._pRules._maxPorOsn3 =
      /* 33 */     this.currentData._pRules._stZpi      = 
      /* 34 */     this.currentData._pRules._stOthOlak  = 
      /* 35 */     this.currentData._pRules._stDodStaz  = decimal.Zero;
      /* 36 */     this.currentData._pRules._granBrRad  = 0;
      /* 37 */     this.currentData._pRules._stMioNaB1  = 
      /* 38 */     this.currentData._pRules._stMioNa2B1 = 
      /* 39 */     this.currentData._pRules._stMioNaB2  = 
      /* 40 */     this.currentData._pRules._stMioNa2B2 = 
      /* 41 */     this.currentData._pRules._stMioNaB3  = 
      /* 42 */     this.currentData._pRules._stMioNa2B3 = 
      /* 43 */     this.currentData._pRules._stMioNaB4  = 
      /* 44 */     this.currentData._pRules._stMioNa2B4 = 
      /* 45 */     this.currentData._pRules._prosPlaca  = 
      /* 46 */     this.currentData._pRules._stMioNa2B5 = 
      /* 47 */     this.currentData._pRules._stKrizPor1 = 
      /* 48 */     this.currentData._pRules._stKrizPor2 = decimal.Zero;
      /* 50 */     this.currentData._pRules._vrKoefBr1  = decimal.Zero;

   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterDokNum = new VvSQL.RecordSorter(Placa.recordName, Placa.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Br Dok", VvSQL.SorterType.DokNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Placa.recordName, Placa.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Placa.recordName, Placa.recordNameArhiva, new VvSQL.IndexSegment[]
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
      get { return Placa.sorterDokNum; }
   }

   #endregion Sorters

   #region Overriders And Specifics

   internal PlacaStruct CurrentData // cijela PlacaStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PlacaDao; }
   }

   public override string VirtualRecordName
   {
      get { return Placa.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Placa.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "pl"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   public override DateTime VirtualAddTS  { get { return this.AddTS;  } }
   public override DateTime VirtualModTS  { get { return this.ModTS;  } }
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

   public override bool VirtualIsLocked
   {
      get
      {
         return this.IsLocked;
      }
   }

   //=== About Transes ======================================================== 

   public override string TransRecordName
   {
      get { return Ptrans.recordName; }
   }

   public override string TransRecordName2
   {
      get { return Ptrane.recordName; }
   }

   public override string TransRecordName3
   {
      get { return Ptrano.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Ptrans.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva2
   {
      get { return Ptrane.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva3
   {
      get { return Ptrano.recordNameArhiva; }
   }

   /// <summary>
   /// Gets or sets a list of ptrans (line items) for the placa.
   /// </summary>
   public List<Ptrans> Transes  { get; set; }
   public List<Ptrane> Transes2 { get; set; }
   public List<Ptrano> Transes3 { get; set; }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Ptrans> Transes' konvertiran u List<VvTransRecord>
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
      get
      {
         if(this.Transes != null) return Transes.ConvertAll(ftr => ftr as VvTransRecord);
         else                     return null;
      }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override List<VvTransRecord> VirtualTranses2
   {
      get
      {
         if(this.Transes2 != null) return Transes2.ConvertAll(ftr => ftr as VvTransRecord);
         else                      return null;
      }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override List<VvTransRecord> VirtualTranses3
   {
      get
      {
         if(this.Transes3 != null) return Transes3.ConvertAll(ftr => ftr as VvTransRecord);
         else                      return null;
      }
   }

   public override void InvokeTransClear()
   {
      if(this.Transes != null) this.Transes.Clear();
   }

   public override void InvokeTransClear2()
   {
      if(this.Transes2 != null) this.Transes2.Clear();
   }

   public override void InvokeTransClear3()
   {
      if(this.Transes3 != null) this.Transes3.Clear();
   }

   public override void InvokeTransRemove(VvTransRecord trans_rec)
   {
      this.Transes.Remove((Ptrans)trans_rec);
   }

   public override void InvokeTransRemove2(VvTransRecord trans_rec)
   {
      this.Transes2.Remove((Ptrane)trans_rec);
   }

   public override void InvokeTransRemove3(VvTransRecord trans_rec)
   {
      this.Transes3.Remove((Ptrano)trans_rec);
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

   public static bool IsSunday(string DD, string MMYYYY)
   {
      return GetDateTimeFromMMYYYY(MMYYYY, false, DD).IsSunday();
   }

   public static DateTime GetDateTimeFromMMYYYY(string MMYYYY, bool weWantLastDayInsteadOfFirstDay)
   {
      return GetDateTimeFromMMYYYY(MMYYYY, weWantLastDayInsteadOfFirstDay, "");
   }

   public static DateTime GetDateTimeFromMMYYYY(string MMYYYY, bool weWantLastDayInsteadOfFirstDay, string weWantThisDay)
   {
      //int yy, mm, dd;

      if(MMYYYY.IsEmpty() || MMYYYY.Length < 6) return DateTime.MinValue;

      //dd = 01;
      //mm = ZXC.ValOrZero_Int(MMYYYY.Substring(0, 2));
      //yy = ZXC.ValOrZero_Int(MMYYYY.Substring(2, 4));

      //return new DateTime(yy, mm, dd);

      string   dd = weWantThisDay.NotEmpty() ? weWantThisDay : "01";
      DateTime theDateTime = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(dd + MMYYYY);

      if(weWantLastDayInsteadOfFirstDay == true)
      {
         int lastDayOfMonth = DateTime.DaysInMonth(theDateTime.Year, theDateTime.Month);

         theDateTime = new DateTime(theDateTime.Year, theDateTime.Month, lastDayOfMonth);
      }

      return theDateTime;
   }

   public static DateTime GetDateTimeFromYYYY(string YYYY, bool weWantLastDayInsteadOfFirstDay)
   {
      //int yy, mm, dd;

      if(YYYY.IsEmpty() || YYYY.Length < 4) return DateTime.MinValue;

      //dd = 01;
      //mm = ZXC.ValOrZero_Int(MMYYYY.Substring(0, 2));
      //yy = ZXC.ValOrZero_Int(MMYYYY.Substring(2, 4));

      //return new DateTime(yy, mm, dd);

      string   ddmm;
      if(weWantLastDayInsteadOfFirstDay) ddmm = "3112";
      else                               ddmm = "0101";

      DateTime theDateTime = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(ddmm + YYYY);

      return theDateTime;
   }

   public static string Get_uMMYYYY(DateTime dateDok)
   {
      return dateDok.ToString("MMyyyy");
   }

   public static string Get_uMM(DateTime dateDok)
   {
      return dateDok.ToString("MM");
   }

   public static string Get_uYYYY(DateTime dateDok)
   {
      return dateDok.ToString("yyyy");
   }

   public static string Get_zaYYYY(string MMYYYY)
   {
      return MMYYYY.Substring(2, 4);
   }

   public static string Get_zaMM(string MMYYYY)
   {
      return MMYYYY.Substring(0, 2);
   }

   public static int Get_zaYYYYint(string MMYYYY)
   {
      return ZXC.ValOrZero_Int(Get_zaYYYY(MMYYYY));
   }

   public static int Get_zaMMint(string MMYYYY)
   {
      return ZXC.ValOrZero_Int(Get_zaMM(MMYYYY));
   }

   internal static decimal GetPriznatiPrekovrSati(decimal prekovrSati)
   {
      return prekovrSati > Placa.MAXdozvoljeniPrekovrSati ? Placa.MAXdozvoljeniPrekovrSati : prekovrSati;
   }

   internal static decimal GetVisakPrekovrSati(decimal prekovrSati)
   {
      decimal visak = prekovrSati - Placa.MAXdozvoljeniPrekovrSati;

      return visak.IsPositive() ? visak : 0M;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Placa>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Placa>(fileName);
   }


   #endregion Overriders And Specifics

   #region propertiz

   //===================================================================
   //===================================================================
   //===================================================================

   #region Common Data Layer Columns

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

   #endregion Common Data Layer Columns

   //===================================================================

   #region Data Layer Columns

   /**/

   /* 05 */ public uint DokNum
   {
      get { return this.currentData._dokNum; }
      set {        this.currentData._dokNum = value; }
   }
   /* 06 */ public DateTime DokDate
   {
      get { return this.currentData._dokDate; }
      set {        this.currentData._dokDate = value; }
   }

   public string Umjesecu_DokDateAsMMYYYY
   {
      get { return this.DokDateAsMMYYYY; }
   }
   public string DokDateAsMMYYYY
   {
      get { return this.currentData._dokDate.ToString("MMyyyy"); }
   }
   /* 07 */
   public string TT
   {
      get { return this.currentData._tt; }
      set {        this.currentData._tt = value; }
   }
   /* 08 */ public uint TtNum
   {
      get { return this.currentData._ttNum; }
      set {        this.currentData._ttNum = value; }
   }
   /* 09 */ public string VrstaObr
   {
      get { return this.currentData._vrstaObr; }
      set {        this.currentData._vrstaObr = value; }
   }
   /* 10 */ public string /*DateTime*/ MMYYYY
   {
      get { return this.currentData._mmyyyy; }
      set {        this.currentData._mmyyyy = value; }
   }
            public string MMYY
   {
      get { return this.currentData._mmyyyy.Substring(0, 2) + this.currentData._mmyyyy.Substring(4); }
   }
   public DateTime MMYYYY_asDateTime
   {
      get { return GetDateTimeFromMMYYYY(this.currentData._mmyyyy, false); }
   }

   /* 11 */ public decimal FondSati
   {
      get { return this.currentData._fondSati; }
      set {        this.currentData._fondSati = value; }
   }
   /* 12 */ public uint MtrosCd
   {
      get { return this.currentData._mtros_cd; }
      set {        this.currentData._mtros_cd = value; }
   }
   /* 13 */ public string MtrosTk
   {
      get { return this.currentData._mtros_tk; }
      set {        this.currentData._mtros_tk = value; }
   }
   /* 14 */ public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }
   /* 15 */ public bool FlagA
   {
      get { return this.currentData._flagA; }
      set {        this.currentData._flagA = value; }
   }
   /* 16 */
   public string RSm_ID
   {
      get { return this.currentData._rSm_ID; }
      set { this.currentData._rSm_ID = value; }
   }
   /* 17 */ public bool IsTrgFondSati
   {
      get { return this.currentData._isTrgFondSati; }
      set {        this.currentData._isTrgFondSati = value; }
   }

   /* 49 */
   public string VrstaJOPPD
   {
      get { return this.currentData._vrstaJOPPD; }
      set { this.currentData._vrstaJOPPD = value; }
   }

   /* 17 */ public bool IsLocked
   {
      get { return this.currentData._isLocked; }
      set {        this.currentData._isLocked = value; }
   }

   #endregion Data Layer Columns

   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   #region Rules Data Layer Columns

   public PrulesStruct Prules
   {
      get { return this.currentData._pRules; }
      set {        this.currentData._pRules = value; }
   }

   /* 17 */ public decimal Rule_StPor1
   {
      get { return this.currentData._pRules._stpor1; }
      set {        this.currentData._pRules._stpor1 = value; }
   }

   /* 18 */ public decimal Rule_StPor2
   {
      get { return this.currentData._pRules._stpor2; }
      set {        this.currentData._pRules._stpor2 = value; }
   }
   /* 19 */ public decimal Rule_StPor3
   {
      get { return this.currentData._pRules._stpor3; }
      set {        this.currentData._pRules._stpor3 = value; }
   }
   /* 20 */ public decimal Rule_StPor4
   {
      get { return this.currentData._pRules._stpor4; }
      set {        this.currentData._pRules._stpor4 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 21 */ public decimal Rule_OsnOdb
   {
      get { return this.currentData._pRules._osnOdb; }
      set {        this.currentData._pRules._osnOdb = value; }
   }
   /* 22 */ public decimal Rule_StMio1stup
   {
      get { return this.currentData._pRules._stMio1stup; }
      set {        this.currentData._pRules._stMio1stup = value; }
   }
   /* 23 */ public decimal Rule_StMio2stup
   {
      get { return this.currentData._pRules._stMio2stup; }
      set {        this.currentData._pRules._stMio2stup = value; }
   }
   /* 24 */ public decimal Rule_StZdrNa
   {
      get { return this.currentData._pRules._stZdrNa; }
      set {        this.currentData._pRules._stZdrNa = value; }
   }
   /* 25 */ public decimal Rule_StZorNa
   {
      get { return this.currentData._pRules._stZorNa; }
      set {        this.currentData._pRules._stZorNa = value; }
   }
   /* 26 */ public decimal Rule_StZapNa
   {
      get { return this.currentData._pRules._stZapNa; }
      set {        this.currentData._pRules._stZapNa = value; }
   }
   /* 27 */ public decimal Rule_StZapII
   {
      get { return this.currentData._pRules._stZapII; }
      set {        this.currentData._pRules._stZapII = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 28 */ public decimal Rule_MinMioOsn
   {
      get { return this.currentData._pRules._minMioOsn; }
      set {        this.currentData._pRules._minMioOsn = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 29 */ public decimal Rule_MaxMioOsn
   {
      get { return this.currentData._pRules._maxMioOsn; }
      set {        this.currentData._pRules._maxMioOsn = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 30 */ public decimal Rule_MaxPorOsn1
   {
      get { return this.currentData._pRules._maxPorOsn1; }
      set {        this.currentData._pRules._maxPorOsn1 = value; }
   }
   //[VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 31 */ public decimal Rule_MaxPorOsn2
   {
      get { return this.currentData._pRules._maxPorOsn2; }
      set {        this.currentData._pRules._maxPorOsn2 = value; }
   }
   //[VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 32 */ public decimal Rule_MaxPorOsn3
   {
      get { return this.currentData._pRules._maxPorOsn3; }
      set {        this.currentData._pRules._maxPorOsn3 = value; }
   }
   /* 33 */ public decimal Rule_StZpi
   {
      get { return this.currentData._pRules._stZpi; }
      set {        this.currentData._pRules._stZpi = value; }
   }
   /* 34 */ public decimal Rule_StOthOlak
   {
      get { return this.currentData._pRules._stOthOlak; }
      set {        this.currentData._pRules._stOthOlak = value; }
   }
   /* 35 */ public decimal Rule_StDodStaz
   {
      get { return this.currentData._pRules._stDodStaz; }
      set {        this.currentData._pRules._stDodStaz = value; }
   }
   /* 36 */ public uint    Rule_GranBrRad
   {
      get { return this.currentData._pRules._granBrRad; }
      set {        this.currentData._pRules._granBrRad = value; }
   }
   /* 37 */ public decimal Rule_StMioNaB1
   {
      get { return this.currentData._pRules._stMioNaB1; }
      set {        this.currentData._pRules._stMioNaB1 = value; }
   }
   /* 38 */ public decimal Rule_StMioNa2B1
   {
      get { return this.currentData._pRules._stMioNa2B1; }
      set {        this.currentData._pRules._stMioNa2B1 = value; }
   }
   /* 39 */ public decimal Rule_StMioNaB2
   {
      get { return this.currentData._pRules._stMioNaB2; }
      set {        this.currentData._pRules._stMioNaB2 = value; }
   }
   /* 40 */ public decimal Rule_StMioNa2B2
   {
      get { return this.currentData._pRules._stMioNa2B2; }
      set {        this.currentData._pRules._stMioNa2B2 = value; }
   }
   /* 41 */ public decimal Rule_StMioNaB3
   {
      get { return this.currentData._pRules._stMioNaB3; }
      set {        this.currentData._pRules._stMioNaB3 = value; }
   }
   /* 42 */ public decimal Rule_StMioNa2B3
   {
      get { return this.currentData._pRules._stMioNa2B3; }
      set {        this.currentData._pRules._stMioNa2B3 = value; }
   }
   /* 43 */ public decimal Rule_StMioNaB4
   {
      get { return this.currentData._pRules._stMioNaB4; }
      set {        this.currentData._pRules._stMioNaB4 = value; }
   }
   /* 44 */ public decimal Rule_StMioNa2B4
   {
      get { return this.currentData._pRules._stMioNa2B4; }
      set {        this.currentData._pRules._stMioNa2B4 = value; }
   }
   
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 45 */ public decimal Rule_ProsPlaca
   {
      get { return this.currentData._pRules._prosPlaca; }
      set {        this.currentData._pRules._prosPlaca = value; }
   }
   /* 46 */
   /// <summary>
   /// FUSE! 31.01.2017. NOT FUSE anymore 
   /// </summary>
 //public decimal Rule_StMioNa2B5
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal Rule_OsnDopClUp
   {
      get { return this.currentData._pRules._stMioNa2B5; }
      set {        this.currentData._pRules._stMioNa2B5 = value; }
   }
   /* 47 */ public decimal Rule_StKrizPor1
   {
      get { return this.currentData._pRules._stKrizPor1; }
      set {        this.currentData._pRules._stKrizPor1 = value; }
   }
   /* 48 */ public decimal Rule_StKrizPor2
   {
      get { return this.currentData._pRules._stKrizPor2; }
      set {        this.currentData._pRules._stKrizPor2 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 50 */ public decimal Rule_VrKoefBr1
   {
      get { return this.currentData._pRules._vrKoefBr1; }
      set {        this.currentData._pRules._vrKoefBr1 = value; }
   }
   /* 51 */ public decimal Rule_StZdrDD
   {
      get { return this.currentData._pRules._stZdrDD; }
      set {        this.currentData._pRules._stZdrDD = value; }
   }
#if nottjetttt
   /* 52 */ public decimal Rule_Mio1Gran1
   {
      get { return this.currentData._pRules._mio1Granica1; }
      set {        this.currentData._pRules._mio1Granica1 = value; }
   }
   /* 53 */ public decimal Rule_Mio1Gran2
   {
      get { return this.currentData._pRules._mio1Granica2; }
      set {        this.currentData._pRules._mio1Granica2 = value; }
   }
   /* 54 */ public decimal Rule_Mio1FiksOlk
   {
      get { return this.currentData._pRules._mio1FiksOlk; }
      set {        this.currentData._pRules._mio1FiksOlk = value; }
   }
   /* 55 */ public decimal Rule_Mio1KoefOlk
   {
      get { return this.currentData._pRules._mio1KoefOlk; }
      set {        this.currentData._pRules._mio1KoefOlk = value; }
   }
#endif

   #endregion Rules Data Layer Columns

   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   #region Result Sums - NON Data Layer Columns

   //private List<Ptrans> TransesNonDeleted
   public Ptrans[] TransesNonDeleted
   {
      get
      {
         return this.Transes.Where(ptrn => ptrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   public Ptrane[] TransesNonDeleted2
   {
      get
      {
         return this.Transes2.Where(ptrne => ptrne.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   public Ptrano[] TransesNonDeleted3
   {
      get
      {
         return this.Transes3.Where(ptrno => ptrno.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   /* 01 */
   public decimal S_tBrutoOsn 
   { 
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_brutoOsn); }
   }
   /* 02 */   public decimal S_tTopObrok 
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_topObrok);  }   
   }   
   /* 03 */   public decimal S_tDodBruto 
   { 
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_dodBruto);  }   
   }     
   /* 04 */   public decimal S_tZivotno
   { 
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_zivotno);   }   
   }   
   /* 05 */   public decimal S_tDopZdr
   { 
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_dopZdr);    }   
   }   
   /* 06 */   public decimal S_tDobMIO
   { 
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_dobMIO);    }   
   }   
   /* 07 */   public decimal S_tNetoAdd
   {
         get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_NetoAdd); }
   }    
   /* 08 */   public decimal  S_tPrijevoz
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_prijevoz); }
   }
   /* 09 */   public decimal S_rBruto100
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Bruto100);  }
   }
   /* 10 */   public decimal S_rTheBruto
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_TheBruto); }
   }
   /* 11 */   public decimal  S_rMioOsn
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_MioOsn); }
   }   
   /* 12 */   public decimal  S_rMio1stup
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Mio1stup); }
   }	   
   /* 13 */   public decimal  S_rMio2stup
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Mio2stup); }
   }	
   /* 14 */   public decimal  S_rMioAll
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_MioAll); }
   }	
   /* 15 */   public decimal  S_rDoprIz
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_DoprIz); }
   }	
   /* 16 */   public decimal  S_rOdbitak
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Odbitak); }
   }	
   /* 17 */   public decimal  S_rPremije
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Premije); }
   }	
   /* 18 */   public decimal  S_rDohodak
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Dohodak); }
   }	
   /* 19 */   public decimal  S_rPorOsnAll
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorOsnAll); }
   }
   /* 20 */   public decimal  S_rPorOsn1
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorOsn1); }
   }
   /* 21 */   public decimal  S_rPorOsn2
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorOsn2); }
   }	
   /* 22 */   public decimal S_rPorOsn3
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorOsn3); }
   }	
   /* 23 */   public decimal  S_rPorOsn4
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorOsn4); }
   }	
   /* 24 */   public decimal  S_rPor1uk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Por1Uk); }
   }	
   /* 25 */ public decimal  S_rPor2uk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Por2Uk); }
   }	
   /* 26 */ public decimal  S_rPor3uk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Por3Uk); }
   }	
   /* 27 */ public decimal  S_rPor4uk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Por4Uk); }
   }	
   /* 28 */ public decimal  S_rPorezAll
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorezAll); }
   }	
   /* 29 */ public decimal  S_rPrirez
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Prirez); }
   } 	
   /* 30 */ public decimal  S_rPorPrirez
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PorPrirez); }
   }
   /* 31 */ public decimal  S_rNetto
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Netto); }
   }	   
   /* 32 */ public decimal  S_rObustave
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Obustave); }
   }	
   /* 33 */ public decimal  S_r2Pay
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_2Pay); }
   }	
   /* 34 */ public decimal  S_rNaRuke
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_NaRuke); }
   }
   /* 35 */ public decimal  S_rZdrNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZdrNa); }
   }	   
   /* 36 */ public decimal  S_rZorNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZorNa); }
   }	   
   /* 37 */ public decimal  S_rZapNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZapNa); }
   }	   
   /* 38 */ public decimal  S_rZapII
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZapII); }
   }    
   /* 39 */ public decimal  S_rZapAll
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZapAll); }
   }	
   /* 40 */ public decimal  S_rDoprNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_DoprNa); }
   }
   /* 41 */ public decimal  S_rDoprAll
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_DoprAll); }
   }  

   /* 42 */   public decimal  S_rMio1stupNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Mio1stupNa); }
   }	   
   /* 43 */   public decimal  S_rMio2stupNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_Mio2stupNa); }
   }	
   /* 44 */   public decimal  S_rMioAllNa
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_MioAllNa); }
   }	
   /* 45 */   public decimal  S_rKrizPorOsn
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_KrizPorOsn); }
   }	
   /* 46 */   public decimal  S_rKrizPorUk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_KrizPorUk); }
   }

   /* 47 */ public decimal  S_rSatiR
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_SatiR); }
   }	
   /* 48 */ public decimal  S_rSatiB
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_SatiB); }
   }

   /* 49 */ public decimal  S_rZpiUk
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_ZpiUk); }
   }

   /* 50 */ public decimal  S_rDaniZpi
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_DaniZpi); }
   }

   /* 51 */ public decimal  S_rNettoWoAdd
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_NettoWoAdd); }
   }

   /* 52 */ public decimal  S_rAHizdatak
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_AHizdatak); }
   }

   /* 53 */ public decimal  S_rNettoAftKrp
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_NettoAftKrp); }
   }

   /* 54 */ public decimal  S_rBrtDodNaStaz
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_BrtDodNaStaz); }
   }

   /* 55 */   public decimal S_rTheBruto_WoNZ // bez novozaposlenih 
   {
    // 18.12.2019.
    //get { return this.TransesNonDeleted.Where(ptrn =>  ptrn.T_spc != Ptrans.SpecEnum.NOVOZAPOSL                                                ).Sum(ptrn => ptrn.R_TheBruto); }
      get { return this.TransesNonDeleted.Where(ptrn => (ptrn.T_spc != Ptrans.SpecEnum.NOVOZAPOSL && ptrn.T_spc != Ptrans.SpecEnum.NOVO_MINMIONE)).Sum(ptrn => ptrn.R_TheBruto); }
   }

   /* 56 */   public decimal S_tBrDodPoloz
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_brDodPoloz); }   
   }

   /* 57 */   public decimal S_rSatiNeR
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_SatiNeR); }
   }

   /* 58 */   public decimal S_rSatiOnlyRad     
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_SatiOnlyRad); }
   }
   /* 59 */   public decimal S_rSatiOnlyRadBruto
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_SatiOnlyRadBruto); }
   }
   /* 60 */   public decimal S_rPraznikHrsBruto 
   {
      get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_PraznikHrsBruto); }
   }

   public decimal Srad1_PIVRnetto { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_netto_PIVR); } }
   public decimal Srad1_PIVRbruto { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_bruto_PIVR); } }
   public decimal Srad1_PIVRsatiR { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_satiR_PIVR); } }
   public decimal Srad1_ONPNnetto { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_netto_ONPN); } }
   public decimal Srad1_ONPNbruto { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_bruto_ONPN); } }
   public decimal Srad1_ONPNsatiR { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.R_satiR_ONPN); } }

   public decimal S_tDopZdr2020   { get { return this.TransesNonDeleted.Sum(ptrn => ptrn.T_dopZdr2020); } }


   // !!! Kada dodajes ovdje neki 'S_zxc', ne zaboravi i u PlacaDao.FillTypedDataRowSumResults()

   #endregion Result Sums - NON Data Layer Columns

   #region Puse

   //public PlacaResultStruct PlaResult
   //{
   //   get { return this.currentData._plaResult; }
   //   set {        this.currentData._plaResult = value; }
   //}

   ///* 01 */ public decimal  X_BrutoOsn
   //{
   //   get { return this.currentData._plaResult._brutoOsn; }
   //   set {        this.currentData._plaResult._brutoOsn = value; }
   //}   
   ///* 02 */ public decimal  X_TopObrok
   //{
   //   get { return this.currentData._plaResult._topObrok; }
   //   set {        this.currentData._plaResult._topObrok = value; } 
   //}   
   ///* 03 */ public decimal  X_DodBruto
   //{
   //   get { return this.currentData._plaResult._dodBruto; }
   //   set {        this.currentData._plaResult._dodBruto = value; } 
   //}     
   ///* 04 */ public decimal  X_Zivotno
   //{
   //   get { return this.currentData._plaResult._zivotno; }
   //   set {        this.currentData._plaResult._zivotno = value; } 
   //}   
   ///* 05 */ public decimal  X_DopZdr
   //{
   //   get { return this.currentData._plaResult._dopZdr; }
   //   set {        this.currentData._plaResult._dopZdr = value; }
   //}   
   ///* 06 */ public decimal  X_DobMIO
   //{
   //   get { return this.currentData._plaResult._dobMIO; }
   //   set {        this.currentData._plaResult._dobMIO = value; } 
   //}   
   ///* 07 */ public decimal  X_NetoAdd
   //{
   //   get { return this.currentData._plaResult._netoAdd; }
   //   set {        this.currentData._plaResult._netoAdd = value; }
   //}      
   ///* 08 */ public decimal  X_Prijevoz
   //{
   //   get { return this.currentData._plaResult._prijevoz; }
   //   set {        this.currentData._plaResult._prijevoz = value; } 
   //}
   ///* 09 */ public decimal  X_Bruto100
   //{
   //   get { return this.currentData._plaResult._bruto100; }
   //   set {        this.currentData._plaResult._bruto100 = value; } 

   //}  
   ///* 10 */ public decimal X_TheBruto
   //{
   //   get { return this.currentData._plaResult._theBruto; }
   //   set {        this.currentData._plaResult._theBruto = value; }
   //}
   ///* 11 */ public decimal  X_MioOsn
   //{
   //   get { return this.currentData._plaResult._mioOsn; }
   //   set {        this.currentData._plaResult._mioOsn = value; } 
   //}   
   ///* 12 */ public decimal  X_MioIz
   //{
   //   get { return this.currentData._plaResult._mioIz; }
   //   set {        this.currentData._plaResult._mioIz = value; }
   //}	   
   ///* 13 */ public decimal  X_MioIz_II
   //{
   //   get { return this.currentData._plaResult._mioIz_II; }
   //   set {        this.currentData._plaResult._mioIz_II = value; }
   //}	
   ///* 14 */ public decimal  X_MioAll
   //{
   //   get { return this.currentData._plaResult._mioAll; }
   //   set {        this.currentData._plaResult._mioAll = value; } 
   //}	
   ///* 15 */ public decimal  X_DoprIz
   //{
   //   get { return this.currentData._plaResult._doprIz; }
   //   set {        this.currentData._plaResult._doprIz = value; }
   //}	
   ///* 16 */ public decimal  X_Odbitak
   //{
   //   get { return this.currentData._plaResult._odbitak; }
   //   set {        this.currentData._plaResult._odbitak = value; } 
   //}	
   ///* 17 */ public decimal  X_Premije
   //{
   //   get { return this.currentData._plaResult._premije; }
   //   set {        this.currentData._plaResult._premije = value; } 
   //}	
   ///* 18 */ public decimal  X_Dohodak
   //{
   //   get { return this.currentData._plaResult._dohodak; }
   //   set {        this.currentData._plaResult._dohodak = value; } 
   //}	
   ///* 19 */ public decimal  X_PorOsnAll
   //{
   //   get { return this.currentData._plaResult._porOsnAll; }
   //   set {        this.currentData._plaResult._porOsnAll = value; }
   //}
   ///* 20 */ public decimal  X_PorOsn1
   //{
   //   get { return this.currentData._plaResult._porOsn1; }
   //   set {        this.currentData._plaResult._porOsn1 = value; } 
   //}	
   ///* 21 */ public decimal  X_PorOsn2
   //{
   //   get { return this.currentData._plaResult._porOsn2; }
   //   set {        this.currentData._plaResult._porOsn2 = value; } 
   //}	
   ///* 22 */ public decimal  X_PorOsn3
   //{
   //   get { return this.currentData._plaResult._porOsn3; }
   //   set {        this.currentData._plaResult._porOsn3 = value; } 
   //}	
   ///* 23 */ public decimal  X_PorOsn4
   //{
   //   get { return this.currentData._plaResult._porOsn4; }
   //   set {        this.currentData._plaResult._porOsn4 = value; } 
   //}	
   ///* 24 */ public decimal  X_Por1uk
   //{
   //   get { return this.currentData._plaResult._por1uk; }
   //   set {        this.currentData._plaResult._por1uk = value; } 
   //}	
   ///* 25 */ public decimal  X_Por2uk
   //{
   //   get { return this.currentData._plaResult._por2uk; }
   //   set {        this.currentData._plaResult._por2uk = value; }
   //}	
   ///* 26 */ public decimal  X_Por3uk
   //{
   //   get { return this.currentData._plaResult._por3uk; }
   //   set {        this.currentData._plaResult._por3uk = value; }
   //}	
   ///* 27 */ public decimal  X_Por4uk
   //{
   //   get { return this.currentData._plaResult._por4uk; }
   //   set {        this.currentData._plaResult._por4uk = value; } 
   //}	
   ///* 28 */ public decimal  X_PorezAll
   //{
   //   get { return this.currentData._plaResult._porezAll; }
   //   set {        this.currentData._plaResult._porezAll = value; }
   //}	
   ///* 29 */ public decimal  X_Prirez
   //{
   //   get { return this.currentData._plaResult._prirez; }
   //   set {        this.currentData._plaResult._prirez = value; }
   //} 	
   ///* 30 */ public decimal  X_PorPrirez
   //{
   //   get { return this.currentData._plaResult._porPrirez; }
   //   set {        this.currentData._plaResult._porPrirez = value; } 
   //}
   ///* 31 */ public decimal  X_Netto
   //{
   //   get { return this.currentData._plaResult._netto; }
   //   set {        this.currentData._plaResult._netto = value; }
   //}	   
   ///* 32 */ public decimal  X_Obustave
   //{
   //   get { return this.currentData._plaResult._obustave; }
   //   set {        this.currentData._plaResult._obustave = value; }
   //}	
   ///* 33 */ public decimal  X_2Pay
   //{
   //   get { return this.currentData._plaResult._2Pay; }
   //   set {        this.currentData._plaResult._2Pay = value; } 
   //}		
   ///* 34 */ public decimal  X_NaRuke
   //{
   //   get { return this.currentData._plaResult._naRuke; }
   //   set {        this.currentData._plaResult._naRuke = value; }
   //}	
   ///* 35 */ public decimal  X_ZdrNa
   //{
   //   get { return this.currentData._plaResult._zdrNa; }
   //   set {        this.currentData._plaResult._zdrNa = value; } 
   //}	   
   ///* 36 */ public decimal  X_ZorNa
   //{
   //   get { return this.currentData._plaResult._zorNa; }
   //   set {        this.currentData._plaResult._zorNa = value; }
   //}	   
   ///* 37 */ public decimal  X_ZapNa
   //{
   //   get { return this.currentData._plaResult._zapNa; }
   //   set {        this.currentData._plaResult._zapNa = value; } 
   //}	   
   ///* 38 */ public decimal  X_ZapII
   //{
   //   get { return this.currentData._plaResult._zapII; }
   //   set {        this.currentData._plaResult._zapII = value; }
   //}    
   ///* 39 */ public decimal  X_ZapAll
   //{
   //   get { return this.currentData._plaResult._zapAll; }
   //   set {        this.currentData._plaResult._zapAll = value; }
   //}	
   ///* 40 */ public decimal  X_DoprNa
   //{
   //   get { return this.currentData._plaResult._doprNa; }
   //   set {        this.currentData._plaResult._doprNa = value; }
   //}	
   ///* 41 */ public decimal  X_DoprAll
   //{
   //   get { return this.currentData._plaResult._doprAll; }
   //   set {        this.currentData._plaResult._doprAll = value; } 
   //}  
   ///* 42 */ public decimal  X_SatiR
   //{
   //   get { return this.currentData._plaResult._satiR; }
   //   set {        this.currentData._plaResult._satiR = value; }
   //}	
   ///* 43 */ public decimal  X_SatiB
   //{
   //   get { return this.currentData._plaResult._satiB; }
   //   set {        this.currentData._plaResult._satiB = value; }
   //}

   #endregion Puse

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      //return "dokNum: " + DokNum + " (" + DokDate.ToShortDateString() + ")" + " RecID: " + RecID;
      return string.Format("dokNum: {0} ({1}), MMGGGG: {2}, TT: {3} ({4})", DokNum, DokDate.ToShortDateString(), MMYYYY, TT, TtNum);
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PlacaStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PlacaStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PlacaStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PlacaStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PlacaStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Placa newObject = new Placa();

      Generic_CloneData<PlacaStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Placa MakeDeepCopy()
   {
      return (Placa)Clone();
   }

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Ptrans);
   }

   public override void TakeTheseTranses2(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes2 = transList.ConvertAll(trans => trans as Ptrane);
   }

   public override void TakeTheseTranses3(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes3 = transList.ConvertAll(trans => trans as Ptrano);
   }

   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Ptrans);
   }

   public override void TakeTransesFrom2(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses2 == null) return;

      this.Transes2 = _vvPolyDocumRecord.CloneTranses2().ConvertAll(trans2 => trans2 as Ptrane);
   }

   public override void TakeTransesFrom3(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses3 == null) return;

      this.Transes3 = _vvPolyDocumRecord.CloneTranses3().ConvertAll(trans3 => trans3 as Ptrano);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Ptrans> newList = new List<Ptrans>(this.Transes.Count);

      foreach(Ptrans ptrans_rec in this.Transes)
      {
         newList.Add((Ptrans)ptrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses2()
   {
      if(this.Transes2 == null) return null;

      List<Ptrane> newList = new List<Ptrane>(this.Transes2.Count);

      foreach(Ptrane ptrans_rec in this.Transes2)
      {
         newList.Add((Ptrane)ptrans_rec.Clone());
      }

      return (newList.ConvertAll(trans2 => trans2 as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses3()
   {
      if(this.Transes3 == null) return null;

      List<Ptrano> newList = new List<Ptrano>(this.Transes3.Count);

      foreach(Ptrano ptrans_rec in this.Transes3)
      {
         newList.Add((Ptrano)ptrans_rec.Clone());
      }

      return (newList.ConvertAll(trans3 => trans3 as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Placa();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Placa)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Placa)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Ptrans();
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


}
