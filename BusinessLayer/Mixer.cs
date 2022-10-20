using System;
using System.Collections.Generic;
using System.Linq;

#region struct MixerStruct

public struct MixerStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;
                                             /* RASTER   | PUT NALOG           | ZAHTJEV           | ZAHTJEVRNM |SMD      | GOST        | URZ    | RUG     | RGC        */
   /* 05 */ internal uint     _dokNum      ;                                                                                    
   /* 06 */ internal DateTime _dokDate     ;                                                                                    
   /* 07 */ internal string   _tt          ;                                                                                    
   /* 08 */ internal uint     _ttNum       ;                                                                                    
   /* 09 */ internal string   _napomena    ;                                                                                    
   /* 10 */ internal int      _intA        ; /* rokPlac  | Trajanje puta       | PlanBrojRadSati   |            |          | Objekt      |        |          |            */
   /* 11 */ internal DateTime _dateA       ; /* dospDate | Datum puta          | TrazeniRokIzvrs   | RokIzvoden |          |             | datNast|          | AtestDate  */
                                                                                                                                
   /* 12 */ internal uint     _kupdobCD    ;                                                                                    
   /* 13 */ internal uint     _mtrosCD     ;                                                                                    
   /* 14 */ internal uint     _personCD    ;                                                                                    
   /* 15 */ internal int      _intB        ; /*          | Sati za dnevnice    |                   |            |          |             |        |           |           */
   /* 16 */ internal bool     _isXxx       ; /*isGot     | Is privat/osob. voz |                   |            |          | isEU        |        | IsAktivan |           */
   /* 17 */ internal string   _konto       ; /*          |                     |                   | IzlazSKL   |          |Drzavljanstvo|        |           |           */
   /* 18 */ internal string   _kupdobTK    ; /*          |                     |                   |            |          | TipObjekta  |        |           |           */ 
   /* 19 */ internal string   _kupdobName  ;                                                                                                                   
   /* 20 */ internal string   _mtrosTK     ; /*          |                     |                   |            |          | BrojSobe    |        |           |           */                                                                               
   /* 21 */ internal string   _personIme   ;                                                                                                     
   /* 22 */ internal string   _personPrezim; /*          |                     | noviUgovorNaZRN   |            |          |             |        |           | ArtiklCd  */                
   /* 23 */ internal string   _devName     ; /*          | Valuta      !!!!! OVO SMIJES KORISTITI SAMO ZA VALUTU !!!!!      |        |           |           */
   /* 24 */ internal string   _strA_40     ; /*ziroRn    | Person radMjesto    | OdgovorIzvrsitelj |            |          | Ime         | KlasOzn| KlasOzn   | BrAtesta  */
   /* 25 */ internal string   _strB_128    ; /*          | Zadatak             | noviRN            |            |          | BrojPutIsp  | Predmet|           | Standard  */
   /* 26 */ internal string   _strC_32     ; /*objekt    | Odrediste           | ProjektCD saRN    |            |          | VrstaGosta  | KlSdrOz| CuvanjeUg | BrSarze   */
   /* 27 */ internal string   _strD_32     ; /*nacPlac   | Na terret           | Kontrola/Izvrsit1 | Vrsta RNM  |          | StatusGosta | UrBr   | UrBr      | Materijal */
   /* 28 */ internal string   _strE_256    ; /*          | izvjest             | Primjedba         |            |          | AdresaGosta |Prijenos| Predmet   |           */
   /* 29 */ internal string   _strF_64     ; /*          | Prilog              | Kontrola/Izvrsit2 | Standard   |          | Prezime     |        | OrigBrUg  | CijevAName*/
   /* 30 */ internal string   _strG_40     ; /*          | Vozilo-naziv        | VoditeljProjekta  |            |          |             | UstrJed| TrajanjeUg| Dimenzija */
   /* 31 */ internal DateTime _dateTimeA   ; /*          | DatumVrijeme odlaz  |                   |            |          | DateTUlaska |        |           |           */
   /* 32 */ internal DateTime _dateTimeB   ; /*          | DatumVrijeme dolaz  |                   |            |          | DateTOdjave |        |           |           */
   /* 33 */ internal DateTime _dateB       ; /*          | Datum obracuna      | MoguciRokIzvrsenj |            |          | DatRodjena  |DatRazv |           |           */
   /* 34 */ internal decimal  _moneyA      ; /*          | Akontacija          |                   |            |          |             |        | IznosUg   | Kolicina  */
   /* 35 */ internal decimal  _moneyB      ; /*          | Iznos dnevnice      |                   |            |          |             |        |           |           */
   /* 36 */ internal decimal  _moneyC      ; /*          | Broj  dnevnica      |                   |            |          |             |        |           |           */
   /* 37 */ internal string   _strH_32     ; /*          | VoziloCD            | Status            |            |          | MjestoRod   |OznRazv | VrstaUG   | JedMjere  */
   /* 38 */ internal string   _projektCD   ; /*          |      !!!!! OVO SMIJES KORISTITI SAMO ZA projektCD !!!!!          |        |           |           */ 
   /* 39 */ internal string   _v1_tt       ; /*          |                     |                   |            |          | VrstPutIspr |   X    |    X      |           */
   /* 40 */ internal uint     _v1_ttNum    ; /*          |                     |                   |            |          |             |   X    |    X      |           */
   /* 41 */ internal string   _v2_tt       ; /*          |                     |                   |            |          | MjestUlaska |   X    |    X      |           */
   /* 42 */ internal uint     _v2_ttNum    ; /*          |                     |                   |            |          |             |   X    |    X      |           */
   /* 43 */ internal string   _konto2      ; /*          |                     |                   |  UlazSKL   |          | DrzavaRodj  |        |           |           */
   /* 44 */ internal string   _externLink1 ; /*          |                     |                   |            |          |             |   X    |    X      |           */
   /* 45 */ internal string   _externLink2 ; /*          |                     |                   |            |          |             |   X    |    X      |           */
}

#endregion struct MixerStruct

public class Mixer : VvPolyDocumRecord
{

   #region Fildz

   public const string recordName       = "mixer";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   public const string TT_VIRMAN  = "VIR";
   public const string TT_RASTERF = "RSF";
   public const string TT_RASTERB = "RSB";

   public const string TT_PUTN_T = "PNT";
   public const string TT_PUTN_I = "PNI";
   public const string TT_PUTN_L = "PNL";
   public const string TT_PUTN_R = "PNR";

   public const string TT_ZAHT_NABD = "ZND";  // za domacu nabavu          
   public const string TT_ZAHT_NABU = "ZNU";  // za nabavu - uvoz          
   public const string TT_ZAHT_FAKT = "ZFA";  // za fakturiranje           
   public const string TT_ZAHT_IPON = "ZIP";  // za izradu ponude          
   public const string TT_ZAHT_PROJ = "ZPJ";  // za projektiranje          
   public const string TT_ZAHT_OTRN = "ZRN";  // za otvaranje RN           
   public const string TT_ZAHT_ROBA = "ZSI";  // za izdavanje robe sa sklad
   public const string TT_ZAHT_MONT = "ZMO";  // za montazu                
   public const string TT_ZAHT_SERV = "ZSR";  // za servis                 
   public const string TT_ZAHT_KONS = "ZKO";  // konstrukcijskom odjelu    
   public const string TT_ZAHT_PROZ = "ZPR";  // za proizvodnju            
   public const string TT_ZAHT_ORAD = "ZOR";  // za odobrenje rada         
   public const string TT_ZAHT_USLG = "ZUS";  // za usluu                  

   public const string TT_SMD       = "SMD";  // servisno montažni dnevnik

   public const string TT_RVR       = "RVR";  // evidencija Radnog VRemena Zakonska 
   public const string TT_IRV       = "IRV";  // evidencija Radnog VRemena Interna  
   public const string TT_MVR       = "MVR";  // evidencija Mjesecna radnog VRemena 
   public const string TT_AVR       = "AVR";  // analiticki raspored mjesecni radnog VRemena 

   public static string[] aTT_ZAHT = new string[] 
   {
      TT_ZAHT_NABD,
      TT_ZAHT_NABU,
      TT_ZAHT_FAKT,
      TT_ZAHT_IPON,
      TT_ZAHT_PROJ,
      TT_ZAHT_OTRN,
      TT_ZAHT_ROBA,
      TT_ZAHT_MONT,
      TT_ZAHT_SERV,
      TT_ZAHT_KONS,
      TT_ZAHT_PROZ,
      TT_ZAHT_ORAD,
      TT_ZAHT_USLG
   };

   public const string TT_EVD_PON = "EPO";   // PONUDE   
   public const string TT_EVD_UGV = "EUG";   // UGOVORI  
   public const string TT_EVD_ZAP = "EZP";   // ZAPISNICI
   public const string TT_EVD_PRJ = "EPP";   // PROJEKTI 
   public const string TT_EVD_TPL = "ETP";   // T PLAN   
   public const string TT_EVD_MCD = "EMC";   // MATH CAD 
   public const string TT_EVD_PRG = "EPG";   // PRIJAVA GRAD
   public const string TT_EVD_ATS = "EAT";   // ATESTI
   public const string TT_EVD_USV = "ESV";   // UVJERENJA SV
   public const string TT_EVD_UVR = "EUV";   // UVJERENJA RADNICI
   public const string TT_EVD_RDK = "ERD";   // RAD DOKUM
   public const string TT_EVD_ODL = "EOD";   // ODLUKE
   public const string TT_EVD_KMP = "EKO";   // KOMPENZACIJE
   public const string TT_EVD_PUN = "EPU";   // PUNOMOCI
   public const string TT_EVD_OST = "EOS";   // OSTALO
   public const string TT_EVD_N10 = "EN1";   // NACRTI - 
   public const string TT_EVD_N20 = "EN2";   // NACRTI - MONTAZNI
   public const string TT_EVD_N30 = "EN3";   // NACRTI - DISPOZIC
   public const string TT_EVD_N40 = "EN4";   // NACRTI - SHEME
   public const string TT_EVD_N50 = "EN5";   // NACRTI - 
   public const string TT_EVD_N60 = "EN6";   // NACRTI - SCAN SKICA

   public static string[] aTT_EVD = new string[] 
   {
      TT_EVD_PON,
      TT_EVD_UGV,
      TT_EVD_ZAP,
      TT_EVD_PRJ,
      TT_EVD_TPL,
      TT_EVD_MCD,
      TT_EVD_PRG,
      TT_EVD_ATS,
      TT_EVD_USV,
      TT_EVD_UVR,
      TT_EVD_RDK,
      TT_EVD_ODL,
      TT_EVD_KMP,
      TT_EVD_PUN,
      TT_EVD_OST,
      TT_EVD_N10,
      TT_EVD_N20,
      TT_EVD_N30,
      TT_EVD_N40,
      TT_EVD_N50,
      TT_EVD_N60

   };

   public const string TT_GFI = "GFI";
   public const string TT_TSI = "TSI";
   public const string TT_OPD = "OPD";

   public const string TT_EXT_CIJ = "CIJ"; // EXTERNI CIJENIK (npr DUCATI da ITALY MSRP u EUR) 
   public const string TT_PMV     = "PMV";
   public const string TT_GST     = "GST"; // evidencija gostiju

   public const string TT_PLN = "PLN"; // plan                 
   public const string TT_RBL = "RBL"; // rebalans             
   public const string TT_NJV = "NJV"; // najava               
   public const string TT_PBN = "PBN"; // plan bagatelne nabave

   public const string TT_NPF_SPR = "SPR"; // Neprofitne statistika S-PR-RAS 
   public const string TT_NPF_PPR = "PPR"; // Neprofitne statistika PR-RAS   
   public const string TT_NPF_GPR = "GPR"; // Neprofitne statistika G-IZ     
   public const string TT_NPF_BIL = "BIL"; // Neprofitne statistika BIL      

   public const string TT_URZ     = "URZ"; // Urudzbeni zapisnik
   public const string TT_RUG     = "RUG"; // Registar ugovora
 
   public const string TT_RGC     = "RGC"; // Registar cijevi
 
   public const string TT_NZI     = "NZI"; // MFX - Nalog za izradu - iz njega nastane RNM

   public const string TT_ZLJ     = "ZLJ"; // liječnički pregled
   public const string TT_ZPG     = "ZPG"; // provjera gađanja
                                  
   public const string TT_BMW     = "BMW"; // 
   public const string TT_PNA     = "PNA"; // prednarudzba SVD 

   public const string TT_NAK     = "NAK"; // Nazivi Artikala Za Kupca 

   public const string TT_KOP     = "KOP"; // PCTOGO Korekcija otplatnog plana

   public const string TT_KDC     = "KDC"; // KONTAKTI kupdoba                


   private MixerStruct currentData;
   private MixerStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.MixerDao.TheSchemaTable;
   protected static MixerDao.MixerCI      CI             = ZXC.MixerDao.CI;

   public static string tt_colName = ZXC.MixerDao.GetSchemaColumnName(CI.tt);

   #endregion Fildz

   #region Constructors

   public Mixer() : this(0)
   {
   }

   public Mixer(uint ID) : base()
   {
      this.currentData = new MixerStruct();

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
      /* 09 */      this.currentData._napomena     = "";
      /* 10 */      this.currentData._intA         = 0;
      /* 11 */      this.currentData._dateA        = DateTime.MinValue;

      /* 12 */      this.currentData._kupdobCD     = 0;
      /* 13 */      this.currentData._mtrosCD      = 0;
      /* 14 */      this.currentData._personCD     = 0;
      /* 15 */      this.currentData._intB         = 0;
      /* 16 */      this.currentData._isXxx        = false;
      /* 17 */      this.currentData._konto        = "";
      /* 18 */      this.currentData._kupdobTK     = "";
      /* 19 */      this.currentData._kupdobName   = "";
      /* 20 */      this.currentData._mtrosTK      = "";
      /* 21 */      this.currentData._personIme    = "";
      /* 22 */      this.currentData._personPrezim = "";
      /* 23 */      this.currentData._devName      = "";
      /* 24 */      this.currentData._strA_40      = "";
      /* 25 */      this.currentData._strB_128     = "";
      /* 26 */      this.currentData._strC_32      = "";
      /* 27 */      this.currentData._strD_32      = "";
      /* 28 */      this.currentData._strE_256     = "";
      /* 29 */      this.currentData._strF_64      = "";
      /* 30 */      this.currentData._strG_40      = "";
      /* 31 */      this.currentData._dateTimeA    = DateTime.MinValue;
      /* 32 */      this.currentData._dateTimeB    = DateTime.MinValue;
      /* 33 */      this.currentData._dateB        = DateTime.MinValue;
      /* 34 */      this.currentData._moneyA       = 0.00M;
      /* 35 */      this.currentData._moneyB       = 0.00M;
      /* 36 */      this.currentData._moneyC       = 0.00M;
      /* 37 */      this.currentData._strH_32      = "";
      /* 38 */      this.currentData._projektCD    = "";
      /* 39 */      this.currentData._v1_tt        = "";
      /* 40 */      this.currentData._v1_ttNum     = 0;
      /* 41 */      this.currentData._v2_tt        = "";
      /* 42 */      this.currentData._v2_ttNum     = 0;
      /* 43 */      this.currentData._konto2       = "";
      /* 44 */      this.currentData._externLink1  = "";
      /* 45 */      this.currentData._externLink2  = "";

                    this.Transes  = new List<Xtrans>();
                    this.Transes2 = new List<Xtrano>();
                    this.Transes3 = new List<Xtrano>();
   }

   #endregion Constructors

   #region Sorters

   public static VvSQL.RecordSorter sorterDokNum = new VvSQL.RecordSorter(Mixer.recordName, Mixer.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Br Dok", VvSQL.SorterType.DokNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Mixer.recordName, Mixer.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Mixer.recordName, Mixer.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.tt]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttNum]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "TT Br", VvSQL.SorterType.TtNum, false);

   public static VvSQL.RecordSorter sorterKpdbName = new VvSQL.RecordSorter(Mixer.recordName, Mixer.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.tt          ]),
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.kupdobName  ]),
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.ttNum       ]),
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.recVer      ] , true)
      }, "Partner", VvSQL.SorterType.KpdbName, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      {   
         //sorterRecID, 
         sorterDokNum,
         sorterDokDate,
         sorterTtNum,
         sorterKpdbName
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.DokNum : return new object[] { this.DokNum,               RecVer };
         case VvSQL.SorterType.DokDate: return new object[] { this.DokDate, this.DokNum, RecVer };
         case VvSQL.SorterType.TtNum  : return new object[] { this.TT,      this.TtNum,  RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Mixer.sorterDokNum; }
   }

   #endregion Sorters

   #region propertiz

   #region Standard Propertiz

   internal MixerStruct CurrentData // cijela MixerStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.MixerDao; }
   }

   public override string VirtualRecordName
   {
      get { return Mixer.recordName; }
   }

   //public override string VirtualLegacyRecordPreffix
   //{
   //   get { return "ot"; }
   //}

   public override string VirtualRecordNameArhiva
   {
      get { return Mixer.recordNameArhiva; }
   }

   public override string TransRecordName
   {
      get { return Xtrans.recordName; }
   }

   public override string TransRecordName2
   {
      get { return Xtrano.recordName; }
   }

   public override string TransRecordName3
   {
      get { return Xtrano.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Xtrans.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva2
   {
      get { return Xtrano.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva3
   {
      get { return Xtrano.recordNameArhiva; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS;  } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

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

   /// <summary>
   /// Gets or sets a list of realXtrans (line items) for the mixer.
   /// </summary>
   public List<Xtrans> Transes { get; set; }
   public List<Xtrano> Transes2 { get; set; }
   public List<Xtrano> Transes3 { get; set; }


   private Xtrans[] TransesNonDeleted
   {
      get
      {
         return this.Transes.Where(atrn => atrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   private Xtrano[] TranseoNonDeleted
   {
      get
      {
         return this.Transes2.Where(xtrn => xtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete).ToArray();
      }
   }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Xtrans> Transes' konvertiran u List<VvTransRecord>
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
         else return null;
      }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()]
   public override List<VvTransRecord> VirtualTranses3
   {
      get
      {
         if(this.Transes3 != null) return Transes3.ConvertAll(ftr => ftr as VvTransRecord);
         else return null;
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
      this.Transes.Remove((Xtrans)trans_rec);
   }

   public override void InvokeTransRemove2(VvTransRecord trans_rec)
   {
      this.Transes2.Remove((Xtrano)trans_rec);
   }

   public override void InvokeTransRemove3(VvTransRecord trans_rec)
   {
      this.Transes3.Remove((Xtrano)trans_rec);
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

   //===================================================================

   #endregion Standard Propertiz

   #region Business Propertiz

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
   /* 07 */ public string TT
   {
      get { return this.currentData._tt; }
      set {        this.currentData._tt = value; }
   }
   /* 08 */ public uint TtNum
   {
      get { return this.currentData._ttNum; }
      set {        this.currentData._ttNum = value; }
   }
   /* 09 */ public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }
   /* 10 */ public int IntA
   {
      get { return this.currentData._intA; }
      set {        this.currentData._intA = value; }
   }
   /* 11 */ public DateTime DateA
   {
      get { return this.currentData._dateA; }
      set {        this.currentData._dateA = value; }
   }

   public uint   KupdobCD     { get { return this.currentData._kupdobCD    ; } set { this.currentData._kupdobCD     = value; } }
   public uint   MtrosCD      { get { return this.currentData._mtrosCD     ; } set { this.currentData._mtrosCD      = value; } }
   public uint   PersonCD     { get { return this.currentData._personCD    ; } set { this.currentData._personCD     = value; } }
   public int    IntB         { get { return this.currentData._intB        ; } set { this.currentData._intB         = value; } }
   public bool   IsXxx        { get { return this.currentData._isXxx       ; } set { this.currentData._isXxx        = value; } }
   public string Konto        { get { return this.currentData._konto       ; } set { this.currentData._konto        = value; } }
   public string KupdobTK     { get { return this.currentData._kupdobTK    ; } set { this.currentData._kupdobTK     = value; } }
   public string KupdobName   { get { return this.currentData._kupdobName  ; } set { this.currentData._kupdobName   = value; } }
   public string MtrosTK      { get { return this.currentData._mtrosTK     ; } set { this.currentData._mtrosTK      = value; } }
   public string PersonIme    { get { return this.currentData._personIme   ; } set { this.currentData._personIme    = value; } }
   public string PersonPrezim { get { return this.currentData._personPrezim; } set { this.currentData._personPrezim = value; } }
   public string StrA_40      { get { return this.currentData._strA_40     ; } set { this.currentData._strA_40      = value; } }
   public string StrB_128     { get { return this.currentData._strB_128    ; } set { this.currentData._strB_128     = value; } }
   public string StrC_32      { get { return this.currentData._strC_32     ; } set { this.currentData._strC_32      = value; } }
   public string StrD_32      { get { return this.currentData._strD_32     ; } set { this.currentData._strD_32      = value; } }
   public string StrE_256     { get { return this.currentData._strE_256    ; } set { this.currentData._strE_256     = value; } }
   public string StrF_64      { get { return this.currentData._strF_64     ; } set { this.currentData._strF_64      = value; } }
   public string StrG_40      { get { return this.currentData._strG_40     ; } set { this.currentData._strG_40      = value; } }
   public DateTime DateTimeA  { get { return this.currentData._dateTimeA   ; } set { this.currentData._dateTimeA    = value; } }
   public DateTime DateTimeB  { get { return this.currentData._dateTimeB   ; } set { this.currentData._dateTimeB    = value; } }
   public DateTime DateB      { get { return this.currentData._dateB       ; } set { this.currentData._dateB        = value; } }
   public decimal MoneyA      { get { return this.currentData._moneyA      ; } set { this.currentData._moneyA       = value; } }
   public decimal MoneyB      { get { return this.currentData._moneyB      ; } set { this.currentData._moneyB       = value; } }
   public decimal MoneyC      { get { return this.currentData._moneyC      ; } set { this.currentData._moneyC       = value; } }
   public string  StrH_32     { get { return this.currentData._strH_32     ; } set { this.currentData._strH_32      = value; } }
   public string  ProjektCD   { get { return this.currentData._projektCD   ; } set { this.currentData._projektCD    = value; } }
   public string  V1_tt       { get { return this.currentData._v1_tt       ; } set { this.currentData._v1_tt        = value; } }
   public uint    V1_ttNum    { get { return this.currentData._v1_ttNum    ; } set { this.currentData._v1_ttNum     = value; } }
   public string  V2_tt       { get { return this.currentData._v2_tt       ; } set { this.currentData._v2_tt        = value; } }
   public uint    V2_ttNum    { get { return this.currentData._v2_ttNum    ; } set { this.currentData._v2_ttNum     = value; } }
   public string  Konto2      { get { return this.currentData._konto2      ; } set { this.currentData._konto2       = value; } }
   public string  ExternLink1 { get { return this.currentData._externLink1 ; } set { this.currentData._externLink1  = value; } }
   public string  ExternLink2 { get { return this.currentData._externLink2 ; } set { this.currentData._externLink2  = value; } }

   public string DevName      { get { return this.currentData._devName     ; } set { this.currentData._devName      = value; } }
   public ZXC.ValutaNameEnum DevNameAsEnum
   {
      get
      {
         if(DevName.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                  return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), DevName, true);
      }
   }
   public decimal DevTecaj { get { return ZXC.DevTecDao.GetHnbTecaj(this.DevNameAsEnum, this.DokDate); } }

   /**/

   #endregion Business Propertiz

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return string.Format("{1}-{2} {0}", DokDate.ToShortDateString(), TT, TtNum);
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<MixerStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<MixerStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<MixerStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<MixerStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<MixerStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Mixer newObject = new Mixer();

      Generic_CloneData<MixerStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Mixer MakeDeepCopy()
   {
      return (Mixer)Clone();
   }

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Xtrans);
   }

   public override void TakeTheseTranses2(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes2 = transList.ConvertAll(trans => trans as Xtrano);
   }

   public override void TakeTheseTranses3(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes3 = transList.ConvertAll(trans => trans as Xtrano);
   }


   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Xtrans);
   }

   public override void TakeTransesFrom2(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses2 == null) return;

      this.Transes2 = _vvPolyDocumRecord.CloneTranses2().ConvertAll(trans2 => trans2 as Xtrano);
   }

   public override void TakeTransesFrom3(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses3 == null) return;

      this.Transes3 = _vvPolyDocumRecord.CloneTranses3().ConvertAll(trans3 => trans3 as Xtrano);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Xtrans> newList = new List<Xtrans>(this.Transes.Count);

      foreach(Xtrans xtrans_rec in this.Transes)
      {
         newList.Add((Xtrans)xtrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses2()
   {
      if(this.Transes2 == null) return null;

      List<Xtrano> newList = new List<Xtrano>(this.Transes2.Count);

      foreach(Xtrano trans_rec in this.Transes2)
      {
         newList.Add((Xtrano)trans_rec.Clone());
      }

      return (newList.ConvertAll(trans2 => trans2 as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses3()
   {
      if(this.Transes3 == null) return null;

      List<Xtrano> newList = new List<Xtrano>(this.Transes3.Count);

      foreach(Xtrano trans_rec in this.Transes3)
      {
         newList.Add((Xtrano)trans_rec.Clone());
      }

      return (newList.ConvertAll(trans3 => trans3 as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Mixer();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Mixer)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Mixer)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Xtrans();
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Mixer>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Mixer>(fileName);
   }


   #endregion VvDataRecordFactory

   #region PrjFaktur_rec (Other Linked Faktur)

   /*private*/
   public bool prjFaktur_rec_LOADED = false;
   
   private Faktur prjFaktur_rec;
   /*public*/ private  Faktur PrjFaktur_rec
   {
      get 
      {
         if(prjFaktur_rec_LOADED == false)
         {
            prjFaktur_rec = new Faktur();

            if(this.ProjektCD.NotEmpty())
            {
               string prjTt;
               uint prjTtNum;

               Ftrans.ParseTipBr(this.ProjektCD, out prjTt, out prjTtNum);

               FakturDao.SetMeFaktur(ZXC.TheMainDbConnection, prjFaktur_rec, prjTt, prjTtNum, false);
            }

            prjFaktur_rec_LOADED = true;
         }

         return prjFaktur_rec; 
      }
   }

   public string ProjektIdent1 
   {
      get
      {
         if(this.ProjektCD.IsEmpty()) return "";
         if((PrjFaktur_rec.TipBr + PrjFaktur_rec.KupdobName + PrjFaktur_rec.VezniDok +  PrjFaktur_rec.PrjArtName).IsEmpty()) return "";
         
         return PrjFaktur_rec.TipBr + "-" + PrjFaktur_rec.KupdobName + "-" + PrjFaktur_rec.VezniDok + "-" + PrjFaktur_rec.PrjArtName;
      }
   }

   public string ProjektIdent2
   {
      get
      {
         if(this.ProjektCD.IsEmpty()) return "";
         if((PrjFaktur_rec.TipBr + PrjFaktur_rec.KupdobName + PrjFaktur_rec.VezniDok + PrjFaktur_rec.PrjArtName).IsEmpty()) return "";

         return PrjFaktur_rec.TipBr + "-" + PrjFaktur_rec.KupdobName + "-" + PrjFaktur_rec.VezniDok + "-" + PrjFaktur_rec.KupdobCD + "-" + PrjFaktur_rec.V1_tt + "-" + PrjFaktur_rec.V1_ttNum;
      }
   }

   public string ProjektOsobaX
   {
      get
      {
         if(this.ProjektCD.IsEmpty()) return "";

         return PrjFaktur_rec.OsobaX;
      }
   }


   //*****************************************************************************************************************
   //*** pomocni ProjektCD2
   //*****************************************************************************************************************


   /*private*/public bool prjFaktur_rec_LOADED2 = false;
   private Faktur prjFaktur_rec2;
   /*public*/ private  Faktur PrjFaktur_rec2
   {
      get 
      {
         // 06.12.2017: idijote, pa ovaj property ide samovoljno u dataLayer!     
         // uvodim ogranicenje da radi nesto samo ako je aktivan UC - ZahtjeviDUC 
         if(!(ZXC.TheVvForm.TheVvUC is ZahtjeviDUC)) return new Faktur();

         if(prjFaktur_rec_LOADED2 == false)
         {
            prjFaktur_rec2 = new Faktur();

            if(this.StrC_32.NotEmpty())
            {
               string prjTt;
               uint prjTtNum;

               Ftrans.ParseTipBr(this.StrC_32, out prjTt, out prjTtNum);

               FakturDao.SetMeFaktur(ZXC.TheMainDbConnection, prjFaktur_rec2, prjTt, prjTtNum, false);
            }

            prjFaktur_rec_LOADED2 = true;
         }

         return prjFaktur_rec2; 
      }
   }

   public string ProjektIdent1_2 
   {
      get
      {
         if(!this.TT.StartsWith("Z")) return "";
         if(this.StrC_32.IsEmpty())   return "";
         if((PrjFaktur_rec2.TipBr + PrjFaktur_rec2.PrjArtName).IsEmpty()) return "";
         
         return /*PrjFaktur_rec2.TipBr + "-" +*/ PrjFaktur_rec2.PrjArtName;
      }
   }

   #endregion prjFaktur_rec

   #region TransSums and Result propertiz

   #region PutNal & Utils

   public decimal Sum_Money1        { get { return this.TransesNonDeleted.Sum(atrn => atrn.T_moneyA)      ; } }
   public decimal Sum_Kol           { get { return this.TransesNonDeleted.Sum(atrn => atrn.T_kol)         ; } }
   public decimal Sum_KolMoneyA     { get { return this.TransesNonDeleted.Sum(atrn => atrn.R_KolMoneyA)   ; } }
   public decimal Sum_MoneyB        { get { return this.TransesNonDeleted.Sum(atrn => atrn.T_moneyB)   ; } }
   public decimal Sum_MoneyC        { get { return this.TransesNonDeleted.Sum(atrn => atrn.T_moneyC)   ; } }
   public decimal Sum_MoneyD        { get { return this.TransesNonDeleted.Sum(atrn => atrn.T_moneyD)   ; } }
   public decimal Sum_KolMoneyABC   { get { return this.TransesNonDeleted.Sum(atrn => atrn.R_KolMoneyABC); } }

   public decimal Sum2_MoneyA   { get { return this.TranseoNonDeleted.Sum(xtrn => xtrn.T_moneyA)         ; } }

   public decimal R_moneyBxC    { get { return this.MoneyB * this.MoneyC                                 ; } }
   public decimal R_PutNalAllTr { get { return this.R_moneyBxC    + this.Sum_KolMoneyA + this.Sum2_MoneyA; } }
   public decimal R_PutNalToPay { get { return this.R_PutNalAllTr - this.MoneyA                          ; } }


   /// <summary>
   /// On DateB - datum obrazuna dnevnice
   /// </summary>
   /// <param name="onDate"></param>
   /// <returns></returns>
   internal decimal SumAllDevXtransInKuneOnDateB() // NAMJERNO metoda a ne property 
   {
      decimal sumKune = 0.00M;

      foreach(Xtrans xtrans_rec in Transes)
      {
         sumKune += xtrans_rec.R_KolMoneyA * ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrans_rec.T_kpdbZiroB_32), this.DateB);
      }

      return sumKune;
   }

   /// <summary>
   /// On DateB - datum obrazuna dnevnice
   /// </summary>
   /// <param name="onDate"></param>
   /// <returns></returns>
   internal decimal SumAllDevXtranoInKuneOnDateB() // NAMJERNO metoda a ne property 
   {
      decimal sumKune = 0.00M;

      foreach(Xtrano xtrano_rec in Transes2)
      {
         sumKune += xtrano_rec.T_moneyA * ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrano_rec.T_devName), this.DateB);
      }

      return sumKune;
   }

   internal List<VvReportSourceUtil> GetDevizniToPayList() // NAMJERNO metoda a ne property 
   {
      List<VvReportSourceUtil> devizneSume = new List<VvReportSourceUtil>();

      // --- Glavna Valute - START --- 

      VvReportSourceUtil deviznaSuma = new VvReportSourceUtil();

      deviznaSuma.SetDeviza(this.DevName);      // daklem, 'this.DevName' je sigurno prisutna valuta 

    //deviznaSuma.TheMoney += this.Sum2_MoneyA; // ostali troskovi 
      deviznaSuma.TheMoney += Transes2.Where(xtro => xtro.T_devName == this.DevName).Sum(xtro => xtro.T_moneyA); // ostali troskovi 
      deviznaSuma.TheMoney += this.R_moneyBxC ; // dnevnice        
      deviznaSuma.TheMoney -= this.MoneyA     ; // akontacija      

      deviznaSuma.TheMoney += Transes.Where(xtr => xtr.T_kpdbZiroB_32/*realXtrans.devName*/ == this.DevName).Sum(atrn => atrn.R_KolMoneyA);

      devizneSume.Add(deviznaSuma);

      // --- Glavna Valute - END --- 

      // --- Xtrans Eventualne Ostale Valute - START --- 

      List<string> ostaleValute = Transes.Select(trn => trn.T_kpdbZiroB_32).Distinct().ToList();
      
      ostaleValute.Remove(this.DevName);

      foreach(string otherDevName in ostaleValute)
      {
         deviznaSuma = new VvReportSourceUtil();

         deviznaSuma.SetDeviza(otherDevName);

         deviznaSuma.TheMoney = Transes.Where(xtr => xtr.T_kpdbZiroB_32/*realXtrans.devName*/ == otherDevName).Sum(atrn => atrn.R_KolMoneyA);

         devizneSume.Add(deviznaSuma);
      }

      // --- Xtrans Eventualne Ostale Valute - END   --- 

      // --- Xtrano Eventualne Ostale Valute - START --- 

      List<string> ostaleValute2 = Transes2.Select(trno => trno.T_devName).Distinct().ToList();
      
      ostaleValute2.Remove(this.DevName);

      foreach(string otherDevName in ostaleValute2)
      {
         deviznaSuma = new VvReportSourceUtil();

         deviznaSuma.SetDeviza(otherDevName);

         deviznaSuma.TheMoney = Transes2.Where(xtro => xtro.T_devName == otherDevName).Sum(trno => trno.T_moneyA);

         devizneSume.Add(deviznaSuma);
      }

      // --- Xtrans Eventualne Ostale Valute - END   --- 

      return devizneSume;
   }

   public string  R_intA_AsText          { get;  set; }
   public decimal KonvertXtranoSum       { get;  set; }
   public decimal KonvertMoneyPrevoz     { get;  set; }
   public decimal KonvertMoneyAkont      { get;  set; }
   public decimal KonvertMoneyDnevn      { get;  set; }
   public decimal KonvertMoneyUkupno     { get;  set; }

   public void ConvertPutNalInKune()
   {
      ZXC.ValutaNameEnum generalValutaNameEnum = ZXC.GetValutaNameEnumFromValutaName(this.DevName);

    //KonvertXtranoSum       = this.Transes2.Sum(xtrano => xtrano.T_moneyA * ZXC.DevTecDao.GetHnbTecaj(generalValutaNameEnum, DateB));
      KonvertXtranoSum       = this.SumAllDevXtranoInKuneOnDateB();
      KonvertMoneyPrevoz     = this.SumAllDevXtransInKuneOnDateB();
      KonvertMoneyAkont      = this.MoneyA        * ZXC.DevTecDao.GetHnbTecaj(generalValutaNameEnum, DateB);
      KonvertMoneyDnevn      = this.R_moneyBxC    * ZXC.DevTecDao.GetHnbTecaj(generalValutaNameEnum, DateB);

      KonvertMoneyUkupno     = (KonvertMoneyDnevn + KonvertMoneyPrevoz + KonvertXtranoSum) - KonvertMoneyAkont;
   }

   internal string TT_And_TtNum { get { return Set_TT_And_TtNum(this.TT, this.TtNum); } }

   internal static string Set_TT_And_TtNum(string _tt, uint _ttNum)
   {
      if(_tt.IsEmpty() && _ttNum.IsZero()) return "";

      return _tt + "-" + _ttNum;
   }

   internal string TipBr        
   { 
      get 
      {
         /*if(TT == TT_IRM) return TT;
         else*/             return TT_And_TtNum; 
      } 
   }

   public int FirstEmptyVezaLine
   {
      get
      {
         if(V1_tt.IsEmpty() && V1_ttNum.IsZero()) return 1;
         if(V2_tt.IsEmpty() && V2_ttNum.IsZero()) return 2;

         return 0;
      }
   }

   #endregion PutNal & Utils

   #region MVR Sum_DecXY

   public decimal Sum_Dec01 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec01); } }
   public decimal Sum_Dec02 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec02); } }
   public decimal Sum_Dec03 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec03); } }
   public decimal Sum_Dec04 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec04); } }
   public decimal Sum_Dec05 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec05); } }
   public decimal Sum_Dec06 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec06); } }
   public decimal Sum_Dec07 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec07); } }
   public decimal Sum_Dec08 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec08); } }
   public decimal Sum_Dec09 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec09); } }
   public decimal Sum_Dec10 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec10); } }
   public decimal Sum_Dec11 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec11); } }
   public decimal Sum_Dec12 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec12); } }
   public decimal Sum_Dec13 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec13); } }
   public decimal Sum_Dec14 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec14); } }
   public decimal Sum_Dec15 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec15); } }
   public decimal Sum_Dec16 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec16); } }
   public decimal Sum_Dec17 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec17); } }
   public decimal Sum_Dec18 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec18); } }
   public decimal Sum_Dec19 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec19); } }
   public decimal Sum_Dec20 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec20); } }
   public decimal Sum_Dec21 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec21); } }
   public decimal Sum_Dec22 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec22); } }
   public decimal Sum_Dec23 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec23); } }
   public decimal Sum_Dec24 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec24); } }
   public decimal Sum_Dec25 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec25); } }
   public decimal Sum_Dec26 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec26); } }
   public decimal Sum_Dec27 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec27); } }
   public decimal Sum_Dec28 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec28); } }
   public decimal Sum_Dec29 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec29); } }
   public decimal Sum_Dec30 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec30); } }
   public decimal Sum_Dec31 { get { return this.TransesNonDeleted.Sum(xtrn => xtrn.T_dec31); } }

   #endregion MVR Sum_DecXY

   #region GST nocenjaCount, moneyCount, ...

   public string R_GSTdrzavaCD { get { return this.Konto; } } 

   public static bool IsDomaci(string drzavaCD)
   {
      return drzavaCD.IsEmpty() || drzavaCD == "HRV";
   }

   public bool R_GST_IsDomaci { get { return IsDomaci(this.R_GSTdrzavaCD); } } 
   public bool R_GST_IsStrani { get { return !this.R_GST_IsDomaci;         } }

   public bool IsDomaciDolazak(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsDomaci && this.DokDate >= dateOD && this.DokDate <= dateDO;
   }

   public bool IsStraniDolazak(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsStrani && this.DokDate >= dateOD && this.DokDate <= dateDO;
   }

   public uint GetGSTdomacaNocenjaCount(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsDomaci ? GetGSTnocenjaCount(dateOD, dateDO) : 0;
   }

   public uint GetGSTstranaNocenjaCount(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsStrani ? GetGSTnocenjaCount(dateOD, dateDO) : 0;
   }

   public uint GetGSTnocenjaCount(DateTime dateOD, DateTime dateDO)
   {
      uint nightCount = 0;

      DateTime dateODJAVA = this.DateTimeB.IsEmpty() ? dateDO : this.DateTimeB;

      for(DateTime currDate = this.DokDate.Date.AddDays(1); currDate.Date <= dateODJAVA.Date; currDate = currDate.Date.AddDays(1))
      {
         if(currDate.Date >= dateOD.Date && currDate.Date <= dateDO.Date) nightCount++;
      }
         
      return nightCount;
   }

   public decimal GetGSTdomacaNocenjaMoney(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsDomaci ? GetGSTnocenjaMoney(dateOD, dateDO) : 0M;
   }

   public decimal GetGSTstranaNocenjaMoney(DateTime dateOD, DateTime dateDO)
   {
      return R_GST_IsStrani ? GetGSTnocenjaMoney(dateOD, dateDO) : 0M;
   }

   public decimal GetGSTnocenjaMoney(DateTime dateOD, DateTime dateDO)
   {
      decimal nocenjaMoney = 0M;

      DateTime dateODJAVA = this.DateTimeB.IsEmpty() ? dateDO : this.DateTimeB;

      for(DateTime currDate = this.DokDate.Date.AddDays(1); currDate.Date <= dateODJAVA.Date; currDate = currDate.Date.AddDays(1))
      {
         if(currDate.Date >= dateOD.Date && currDate.Date <= dateDO.Date) nocenjaMoney += this.MoneyA;
      }

      return nocenjaMoney;
   }

   #endregion GST nocenjaCount, moneyCount, ...

   //#region RUG - Registar Ugovora
   //
   //private TimeSpan RUG_RokTrajanja 
   //{ 
   //   get 
   //   { 
   //      return new TimeSpan(); 
   //   } 
   //}
   //
   //public bool IsRUG_Active
   //{
   //   get
   //   {
   //      return true;
   //   }
   //}
   //
   //public bool IsRUG_NOT_Active
   //{
   //   get
   //   {
   //      return !IsRUG_Active;
   //   }
   //}
   //
   //#endregion RUG - Registar Ugovora

   #endregion TransSums and Result propertiz

}

