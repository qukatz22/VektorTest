using System;
using System.Linq;
using System.Collections.Generic;
using Vektor.DataLayer.DS_Reports;
using System.Data;

#region struct PtransStruct

public struct PtransStruct
{
   internal uint     _recID;

   /* 01 */  internal uint      _t_parentID;
   /* 02 */  internal uint      _t_dokNum;
   /* 03 */  internal ushort    _t_serial;
   /* 04 */  internal DateTime  _t_dokDate;
   /* 05 */  internal string    _t_tt;
   /* 06 */  internal uint      _t_ttNum;
   /* 07 */  internal string    _t_mmyyyy;
   /* 08 */  internal decimal   _t_fondSati;
   /* 09 */  internal string    _t_rSm_ID;
   /* 10 */  internal uint      _t_personCD;
   /* 11 */  internal string    _t_ime;
   /* 12 */  internal string    _t_prezime;

   /* 13 */  internal decimal   _t_brutoOsn;   
   /* 14 */  internal decimal   _t_topObrok;   
   /* 15 */  internal decimal   _t_godStaza;   
   /* 16 */  internal decimal   _t_dodBruto;   
   /* 17 */  internal bool      _t_isMioII;   
   /* 18 */  internal ushort    _t_spc;
   /* 19 */  internal decimal   _t_koef;
   /* 20 */  internal decimal   _t_zivotno;   
   /* 21 */  internal decimal   _t_dopZdr;   
   /* 22 */  internal decimal   _t_dobMIO;   
   /* 23 */  internal decimal   _t_koefHRVI;
   /* 24 */  internal ushort    _t_invalidTip;
   /* 25 */  internal string    _t_opcCD;
   /* 26 */  internal string    _t_opcName;
   /* 27 */  internal string    _t_opcRadCD;
   /* 28 */  internal string    _t_opcRadName;
   /* 39 */  internal decimal   _t_stPrirez;
   /* 30 */  internal decimal   _t_netoAdd;   
   /* 31 */  internal bool      _t_isDirNeto;   
   /* 32 */  internal decimal   _t_prijevoz;
   /* 33 */  internal bool      _t_isPoluSat;
   /* 34 */  internal uint      _t_rsB;   

   /* 35 */  internal string    _t_nacIsplCD;
   /* 36 */  internal string    _t_neoPrimCD;
   /* 37 */  internal string    _t_dokumCD  ;

   /* 38 */  internal decimal   _t_brutoDodSt ;
   /* 39 */  internal decimal   _t_brDodPoloz ;
   /* 40 */  internal decimal   _t_koefBruto1 ;
   /* 41 */  internal decimal   _t_dnFondSati ;   
   /* 42 */  internal decimal   _t_thisStazSt ;   
   /* 43 */  internal decimal   _t_brutoDodSt2;
   /* 44 */  internal decimal   _t_brutoDodSt3;
   /* 45 */  internal decimal   _t_pr3mjBruto ;
   /* 46 */  internal decimal   _t_brutoKorekc;

   /* 47 */  internal decimal   _t_dopZdr2020 ;   

             //internal PtransResultStruct _ptrResult;
}

public struct PtransResultStruct
{

  /* 01 */ internal decimal _t_bruto100;	   
  /* 02 */ internal decimal _t_theBruto;

  /* 03 */ internal decimal _t_mioOsn;   

  /* 04 */ internal decimal _t_mio1stup;	   
  /* 05 */ internal decimal _t_mio2stup;	
  /* 06 */ internal decimal _t_mioAll;	
  /* 07 */ internal decimal _t_doprIz;	
   
  /* 08 */ internal decimal _t_odbitak;	
  /* 09 */ internal decimal _t_premije;	
  /* 10 */ internal decimal _t_dohodak;	

  /* 11 */ internal decimal _t_porOsnAll;
  /* 12 */ internal decimal _t_porOsn1;	
  /* 13 */ internal decimal _t_porOsn2;	
  /* 14 */ internal decimal _t_porOsn3;	
  /* 15 */ internal decimal _t_porOsn4;	

  /* 16 */ internal decimal _t_por1uk;	
  /* 17 */ internal decimal _t_por2uk;	
  /* 18 */ internal decimal _t_por3uk;	
  /* 19 */ internal decimal _t_por4uk;	
  /* 20 */ internal decimal _t_porezAll;	

  /* 21 */ internal decimal _t_prirez;	
  /* 22 */ internal decimal _t_porPrirez;

  /* 23 */ internal decimal _t_netto;	   
  /* 24 */ internal decimal _t_obustave;	
  /* 25 */ internal decimal _t_2Pay;		
  /* 26 */ internal decimal _t_naRuke;	
 
  /* 27 */ internal decimal _t_zdrNa;	   
  /* 28 */ internal decimal _t_zorNa;	   
  /* 29 */ internal decimal _t_zapNa;	   
  /* 30 */ internal decimal _t_zapII;    
  /* 31 */ internal decimal _t_zapAll;	
  /* 32 */ internal decimal _t_doprNa;	
 
  /* 33 */ internal decimal _t_doprAll;  

  /* 34 */ internal decimal _t_satiR;  
  /* 35 */ internal decimal _t_satiB;  
  /* 36 */ internal decimal _t_satiUk;  

  /* 37 */ internal int     _t_numOfLinesPtrane;  
  /* 38 */ internal int     _t_numOfLinesPtrano;  
  /* 39 */ internal decimal _t_fondSatiDiff;  
  /* xx */ internal decimal _t_fondSatiDiffABS;  

  /* 40 */ internal decimal _t_mio1stupNa;  
  /* 41 */ internal decimal _t_mio2stupNa;  
  /* 42 */ internal decimal _t_mioAllNa;	

  /* 43 */ internal decimal _t_krizPorUk;	
  /* 44 */ internal decimal _t_krizPorOsn;	
  /* 45 */ internal decimal _t_zpiUk;	
  /* 46 */ internal decimal _t_daniZpi;  
  /* 47 */ internal decimal _t_nettoWoAdd;	   
  /* 48 */ internal decimal _t_AHizdatak;	   
  /* 49 */ internal decimal _t_nettoAftKrp;	   
  /* 50 */ internal decimal _t_brtDodNaStaz;	   
  /* 51 */ internal int     _t_min_rsOd;	   
  /* 52 */ internal int     _t_max_rsDo;	   
  /* 53 */ internal decimal _t_praznikHrs;	   

  /* 54 */ internal decimal _t_thisStazDod;	   

  /* 55 */ internal decimal _t_brutoDod2;	   
  /* 56 */ internal decimal _t_brutoDod3;
  /* 57 */ internal decimal _t_satiNeR;  
  /* 58 */ internal decimal _t_ukBrutoKoef;  
  
  /* 59 */ internal decimal _t_satiOnlyRad;  
  /* 60 */ internal decimal _t_satiOnlyRadBruto;  
  /* 61 */ internal decimal _t_praznikHrsBruto;  
  /* 62 */ internal decimal _t_satiNeRBruto;  
  /* 63 */ internal decimal _t_brutoPoEVR;  
  /* 64 */ internal decimal _t_osnovicaDop;  
   

  /* 65 */ internal decimal _t_Sati12	     ;
  /* 66 */ internal decimal _t_Bruto12      ;
  /* 67 */ internal decimal _t_Sati13	     ;
  /* 68 */ internal decimal _t_Bruto13      ;
  /* 69 */ internal decimal _t_Sati14	     ;
  /* 70 */ internal decimal _t_Bruto14      ;
  /* 71 */ internal decimal _t_Sati15	     ;
  /* 72 */ internal decimal _t_Bruto15      ;
  /* 73 */ internal decimal _t_Sati16       ;
  /* 74 */ internal decimal _t_Bruto16      ;
  /* 75 */ internal decimal _t_Sati17       ;
  /* 76 */ internal decimal _t_Bruto17      ;
  /* 77 */ internal decimal _t_Sati20       ;
  /* 78 */ internal decimal _t_Bruto20      ;
  /* 79 */ internal decimal _t_Sati30       ;
  /* 80 */ internal decimal _t_Bruto30      ;
  /* 81 */ internal decimal _t_Sati40       ;
  /* 82 */ internal decimal _t_Bruto40      ;
  /* 83 */ internal decimal _t_Sum1do3      ;
  /* 84 */ internal decimal _t_ZasticeniNeto;
  /* 85 */ internal decimal _t_NetoIsplata  ;
  /* 86 */ internal decimal _t_OnlyObustave ;   
  /* 87 */ internal decimal _t_Bruto30E     ;
  /* 88 */ internal decimal _t_Bruto30D     ;
  /* 89 */ internal decimal _t_Sati18       ;
  /* 90 */ internal decimal _t_Bruto18      ;

  /* 91 */ internal string  _t_porIBAN      ;
  /* 92 */ internal decimal _t_NetoWoZast   ;
  /* 93 */ internal bool    _t_hasNonObust  ;

  /* 94 */ internal bool    _t_isONPN       ;   // Ostale naknade, potpore i nagrade

  /* 95 */ internal decimal _t_netto_EUR    ;	   
  /* 96 */ internal decimal _t_netto_Kn     ;	   

}

#endregion struct PtransStruct

public class Ptrans : VvTransRecord
{

   #region Fildz

   public const string recordName = "ptrans";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PtransStruct currentData;
   private PtransStruct backupData;

   private PtransResultStruct _ptrResult;

   // spec za ptrans: 
   protected static System.Data.DataTable TheSchemaTable = ZXC.PtransDao.TheSchemaTable;
   protected static PtransDao.PtransCI    CI             = ZXC.PtransDao.CI;

   //MINMIONE - ne uzima minMioOsn vec stvarin bruto koji je u biti manji od MinMioOSn
   //CLANUPRAVE - ako ima manji ili jednak bruto propisanom min osnovici za doprinose
   //NOVO_MINMIONE - novozaposleni kojem se ne smije uzeti minimalna mio osnovica već stvarni bruto (koji je obično manji od minmio osnovice)18.12.2019.
   public enum SpecEnum
   {
      NOVOZAPOSL, PENZ, XNIJE, MINMIONE, MAXMIONE, CLANUPRAVE, NOVO_MINMIONE
   }
   public enum InvalidEnum
   {
      HRVI, INVALID, NIJE
   }

   public enum PtranoKind
   {
      OBUSTAVA,
      ZASTICENIrn,
      NEZASTICENIrn,
      NIJE_PTRANO
   }

   #endregion Fildz

   #region Constructors

   public Ptrans() : this(0)
   {
   }

   public Ptrans(uint ID) : base()
   {
      this.currentData = new PtransStruct();

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
      /* 07 */   this.currentData._t_mmyyyy    = "";
      /* 08 */   this.currentData._t_fondSati  = decimal.Zero;
      /* 09 */   this.currentData._t_rSm_ID    = "";
      /* 10 */   this.currentData._t_personCD  = 0;
      /* 11 */   this.currentData._t_ime       = "";
      /* 12 */   this.currentData._t_prezime   = "";

      /* 13 */  this.currentData._t_brutoOsn   = decimal.Zero;   
      /* 14 */  this.currentData._t_topObrok   = decimal.Zero;   
      /* 15 */  this.currentData._t_godStaza   = decimal.Zero;   
      /* 16 */  this.currentData._t_dodBruto   = decimal.Zero;   
      /* 17 */  this.currentData._t_isMioII    = false;   
      /* 18 */  this.currentData._t_spc        = 0;
      /* 19 */  this.currentData._t_koef       = decimal.Zero;
      /* 20 */  this.currentData._t_zivotno    = decimal.Zero;   
      /* 21 */  this.currentData._t_dopZdr     = decimal.Zero;   
      /* 22 */  this.currentData._t_dobMIO     = decimal.Zero;   
      /* 23 */  this.currentData._t_koefHRVI   = decimal.Zero;
      /* 24 */  this.currentData._t_invalidTip = 0;
      /* 25 */  this.currentData._t_opcCD      = "";
      /* 26 */  this.currentData._t_opcName    = "";
      /* 27 */  this.currentData._t_opcRadCD   = "";
      /* 28 */  this.currentData._t_opcRadName = "";
      /* 29 */  this.currentData._t_stPrirez   = decimal.Zero;
      /* 30 */  this.currentData._t_netoAdd    = decimal.Zero;   
      /* 31 */  this.currentData._t_isDirNeto  = false;   
      /* 32 */  this.currentData._t_prijevoz   = decimal.Zero;   
      /* 33 */  this.currentData._t_isPoluSat  = false;   
      /* 34 */  this.currentData._t_rsB        = 0;   

      /* 35 */  this.currentData._t_nacIsplCD  = "";
      /* 36 */  this.currentData._t_neoPrimCD  = "";
      /* 37 */  this.currentData._t_dokumCD    = "";
      /* 38 */  this.currentData._t_brutoDodSt = decimal.Zero;   
      /* 39 */  this.currentData._t_brDodPoloz = decimal.Zero;   
      /* 40 */  this.currentData._t_koefBruto1 = decimal.Zero;   
      /* 41 */  this.currentData._t_dnFondSati = decimal.Zero;   
      /* 42 */  this.currentData._t_thisStazSt = decimal.Zero;   
      /* 43 */  this.currentData._t_brutoDodSt2 = decimal.Zero;   
      /* 44 */  this.currentData._t_brutoDodSt3 = decimal.Zero;   
      /* 45 */  this.currentData._t_pr3mjBruto  = decimal.Zero;   
      /* 46 */  this.currentData._t_brutoKorekc = decimal.Zero;   
      /* 47 */  this.currentData._t_dopZdr2020  = decimal.Zero;   

   }

   #endregion Constructors

   #region Sorters

   // sluzi samo za GTEREC kod PersonDao.GetPrevPtransForPerson(); 
   public static VvSQL.RecordSorter sorter_Person_DokDate_DokNum = new VvSQL.RecordSorter(Ptrans.recordName, Ptrans.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_personCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_dokNum]),
         //new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.t_recVer], true)
      }, "PrsnDokDateNum", VvSQL.SorterType.DokDate, false);

   private VvSQL.RecordSorter[] _sorters = null;

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.DokDate: return new object[] { this.T_personCD, this.T_dokDate, this.T_dokNum, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      //get { return Ptrans.sorter_Person_DokDate_DokNum; }
      get 
      { 
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region propertiz

   internal PtransStruct CurrentData // cijela PtransStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PtransDao; }
   }

   public override string VirtualRecordName
   {
      get { return Ptrans.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Ptrans.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Placa.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "pt"; }
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
   /* 07 */
   public string T_MMYYYY
   {
      get { return this.currentData._t_mmyyyy; }
      set {        this.currentData._t_mmyyyy = value; }
   }
   public DateTime T_MMYYYY_asDateTime
   {
      get { return Placa.GetDateTimeFromMMYYYY(this.currentData._t_mmyyyy, false); }
   }

   /* 08 */
   public decimal T_FondSati
   {
      get { return this.currentData._t_fondSati; }
      set {        this.currentData._t_fondSati = value; }
   }
   /* 09 */
   public string T_RSm_ID
   {
      get { return this.currentData._t_rSm_ID; }
      set {        this.currentData._t_rSm_ID = value; }
   }
   /* 10 */ public uint T_personCD
   {
      get { return this.currentData._t_personCD; }
      set {        this.currentData._t_personCD = value; }
   }
   /* 11 */ public string T_ime
   {
      get { return this.currentData._t_ime; }
      set {        this.currentData._t_ime = value; }
   }
   /* 12 */ public string T_prezime
   {
      get { return this.currentData._t_prezime; }
      set {        this.currentData._t_prezime = value; }
   }

   public string T_prezimeIme
   {
      get { return Person.GetPrezimeIme(this.currentData._t_prezime, this.currentData._t_ime); }
   }

   /* 13 */public decimal T_brutoOsn
   {
      get { return this.currentData._t_brutoOsn; }
      set {        this.currentData._t_brutoOsn = value; }
   }
   /* 14 */public decimal T_topObrok
   {
      get { return this.currentData._t_topObrok; }
      set {        this.currentData._t_topObrok = value; }
   }
   /* 15 */public decimal T_godStaza
   {
      get { return this.currentData._t_godStaza; }
      set {        this.currentData._t_godStaza = value; }
   }
   /* 16 */public decimal T_dodBruto
   {
      get { return this.currentData._t_dodBruto; }
      set {        this.currentData._t_dodBruto = value; }
   }
   /* 17 */public bool T_isMioII
   {
      get { return this.currentData._t_isMioII; }
      set {        this.currentData._t_isMioII = value; }
   }
   /* 18 */public Ptrans.SpecEnum T_spc
   {
      get { return (Ptrans.SpecEnum)this.currentData._t_spc; }
      set {                         this.currentData._t_spc =(ushort) value; }
   }
   /* 19 */public decimal T_koef
   {
      get { return this.currentData._t_koef; }
      set {        this.currentData._t_koef = value; }
   }
   /* 20 */public decimal T_zivotno
   {
      get { return this.currentData._t_zivotno; }
      set {        this.currentData._t_zivotno = value; }
   }
   
   /* 21 */public decimal T_dopZdr
   {
      get { return this.currentData._t_dopZdr; }
      set {        this.currentData._t_dopZdr = value; }
   }
   public decimal T_np63
   {
      get { return (ZXC.projectYearAsInt >= 2019) ? this.T_dopZdr : 0.00M; }
   }


   /* 22 */public decimal T_dobMIO   
   {
      get { return this.currentData._t_dobMIO; }
      set {        this.currentData._t_dobMIO = value; }
   }
   public decimal T_npTobrok
   {
      get { return (ZXC.projectYearAsInt >= 2019) ? this.T_dobMIO : 0.00M; }
   }



   /* 23 */
   public decimal T_koefHRVI
   {
      get { return this.currentData._t_koefHRVI; }
      set {        this.currentData._t_koefHRVI = value; }
   }
   /* 24 */public Ptrans.InvalidEnum T_invalidTip
   {
      get { return (Ptrans.InvalidEnum)this.currentData._t_invalidTip; }
      set {                            this.currentData._t_invalidTip = (ushort)value; }
   }
   /* 25 */public string T_opcCD
   {
      get { return this.currentData._t_opcCD; }
      set {        this.currentData._t_opcCD = value; }
   }
   /* 26 */public string T_opcName
   {
      get { return this.currentData._t_opcName; }
      set {        this.currentData._t_opcName = value; }
   }
   /* 27 */public string T_opcRadCD
   {
      get { return this.currentData._t_opcRadCD; }
      set {        this.currentData._t_opcRadCD = value; }
   }

   /* 28 */public string T_opcRadName
   {
      get { return this.currentData._t_opcRadName; }
      set {        this.currentData._t_opcRadName = value; }
   }

   /* 29 */public decimal T_stPrirez
   {
      get { return this.currentData._t_stPrirez; }
      set {        this.currentData._t_stPrirez = value; }
   }
   /* 30 */public decimal T_NetoAdd
   {
      get { return this.currentData._t_netoAdd; }
      set {        this.currentData._t_netoAdd = value; }
   }
   /* 31 */public bool T_isDirNeto
   {
      get { return this.currentData._t_isDirNeto; }
      set {        this.currentData._t_isDirNeto = value; }
   }
   /* 32 */public decimal T_prijevoz
   {
      get { return this.currentData._t_prijevoz; }
      set {        this.currentData._t_prijevoz = value; }
   }
   /* 33 */public bool T_IsPoluSat
   {
      get { return this.currentData._t_isPoluSat; }
      set {        this.currentData._t_isPoluSat = value; }
   }
   /* 34 */public uint T_rsB
   {
      get { return this.currentData._t_rsB; }
      set {        this.currentData._t_rsB = value; }
   }

   /* 35 */public string T_nacIsplCD { get { return this.currentData._t_nacIsplCD; } set { this.currentData._t_nacIsplCD = value; } }
   /* 36 */public string T_neoPrimCD { get { return this.currentData._t_neoPrimCD; } set { this.currentData._t_neoPrimCD = value; } }
   /* 37 */public string T_dokumCD   { get { return this.currentData._t_dokumCD  ; } set { this.currentData._t_dokumCD   = value; } }

   /* 38 */public decimal T_brutoDodSt
   {
      get { return this.currentData._t_brutoDodSt; }
      set {        this.currentData._t_brutoDodSt = value; }
   }

   /* 39 */public decimal T_brDodPoloz
   {
      get { return this.currentData._t_brDodPoloz; }
      set {        this.currentData._t_brDodPoloz = value; }
   }

   /* 40 */public decimal T_koefBruto1
   {
      get { return this.currentData._t_koefBruto1; }
      set {        this.currentData._t_koefBruto1 = value; }
   }

   /* 41 */public decimal T_dnFondSati
   {
      get { return this.currentData._t_dnFondSati; }
      set {        this.currentData._t_dnFondSati = value; }
   }

   /* 42 */public decimal T_thisStazSt
   {
      get { return this.currentData._t_thisStazSt; }
      set {        this.currentData._t_thisStazSt = value; }
   }

   /* 43 */public decimal T_brutoDodSt2 { get { return this.currentData._t_brutoDodSt2; } set { this.currentData._t_brutoDodSt2 = value; } }
   /* 44 */public decimal T_brutoDodSt3 { get { return this.currentData._t_brutoDodSt3; } set { this.currentData._t_brutoDodSt3 = value; } }
   /* 45 */public decimal T_pr3mjBruto  { get { return this.currentData._t_pr3mjBruto ; } set { this.currentData._t_pr3mjBruto  = value; } }
   /* 46 */public decimal T_brutoKorekc { get { return this.currentData._t_brutoKorekc; } set { this.currentData._t_brutoKorekc = value; } }
   /* 47 */public decimal T_dopZdr2020  { get { return this.currentData._t_dopZdr2020 ; } set { this.currentData._t_dopZdr2020  = value; } }

   /* */

   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   public PtransResultStruct PtrResults
   {
      get { return this._ptrResult;         }
      set {        this._ptrResult = value; }
   }

   /* 01 */ public decimal R_Bruto100 
   { 
     get { return this._ptrResult._t_bruto100;         }
     set {        this._ptrResult._t_bruto100 = value; }
   }	   
   /* 02 */ public decimal R_TheBruto
   { 
      get { return this._ptrResult._t_theBruto;         }
      set {        this._ptrResult._t_theBruto = value; }
   }
   /* 03 */ public decimal R_MioOsn
   { 
      get { return this._ptrResult._t_mioOsn;         }
      set {        this._ptrResult._t_mioOsn = value; }
   }   
   /* 04 */ public decimal R_Mio1stup
   { 
      get { return this._ptrResult._t_mio1stup;         }
      set {        this._ptrResult._t_mio1stup = value; }
   }	   
   /* 05 */ public decimal R_Mio2stup
   { 
      get { return this._ptrResult._t_mio2stup;         }
      set {        this._ptrResult._t_mio2stup = value; }
   }	
   /* 06 */ public decimal R_MioAll
   { 
      get { return this._ptrResult._t_mioAll;         }
      set {        this._ptrResult._t_mioAll = value; }
   }	
   /* 07 */ public decimal R_DoprIz
   { 
      get { return this._ptrResult._t_doprIz;         }
      set {        this._ptrResult._t_doprIz = value; }
   }	
   /* 08 */ public decimal R_Odbitak
   { 
      get { return this._ptrResult._t_odbitak;         }
      set {        this._ptrResult._t_odbitak = value; }
   }	
   /* 09 */ public decimal R_Premije
   { 
      get { return this._ptrResult._t_premije;         }
      set {        this._ptrResult._t_premije = value; }
   }	
   /* 10 */ public decimal R_Dohodak
   { 
      get { return this._ptrResult._t_dohodak;         }
      set {        this._ptrResult._t_dohodak = value; }
   }	
   /* 11 */ public decimal R_PorOsnAll
   { 
      get { return this._ptrResult._t_porOsnAll;         }
      set {        this._ptrResult._t_porOsnAll = value; }
   }
   /* 12 */ public decimal R_PorOsn1
   { 
      get { return this._ptrResult._t_porOsn1;         }
      set {        this._ptrResult._t_porOsn1 = value; }
   }	
   /* 13 */ public decimal R_PorOsn2
   { 
      get { return this._ptrResult._t_porOsn2;         }
      set {        this._ptrResult._t_porOsn2 = value; }
   }	
   /* 14 */ public decimal R_PorOsn3
   { 
      get { return this._ptrResult._t_porOsn3;         }
      set {        this._ptrResult._t_porOsn3 = value; }
   }	
   /* 15 */ public decimal R_PorOsn4
   { 
      get { return this._ptrResult._t_porOsn4;         }
      set {        this._ptrResult._t_porOsn4 = value; }
   }	
 
   /* 16 */ public decimal R_Por1Uk
   {
      get { return this._ptrResult._t_por1uk;         }
      set {        this._ptrResult._t_por1uk = value; }
   }
   /* 17 */ public decimal R_Por2Uk
   { 
      get { return this._ptrResult._t_por2uk;         }
      set {        this._ptrResult._t_por2uk = value; }
   }	
   /* 18 */ public decimal R_Por3Uk
   { 
      get { return this._ptrResult._t_por3uk;         }
      set {        this._ptrResult._t_por3uk = value; }
   }	
   /* 19 */ public decimal R_Por4Uk
   { 
      get { return this._ptrResult._t_por4uk;         }
      set {        this._ptrResult._t_por4uk = value; }
   }	
   /* 20 */ public decimal R_PorezAll
   { 
      get { return this._ptrResult._t_porezAll;         }
      set {        this._ptrResult._t_porezAll = value; }
   }	
   /* 21 */ public decimal R_Prirez
   { 
      get { return this._ptrResult._t_prirez;         }
      set {        this._ptrResult._t_prirez = value; }
   }	
   /* 22 */ public decimal R_PorPrirez
   { 
      get { return this._ptrResult._t_porPrirez;         }
      set {        this._ptrResult._t_porPrirez = value; }
   }
   /* 23 */ public decimal R_Netto
   { 
      get { return this._ptrResult._t_netto;         }
      set {        this._ptrResult._t_netto = value; }
   }	   
   /* 24 */ public decimal R_Obustave
   { 
      get { return this._ptrResult._t_obustave;         }
      set {        this._ptrResult._t_obustave = value; }
   }	
   /* 25 */ public decimal R_2Pay
   { 
      get { return this._ptrResult._t_2Pay;         }
      set {        this._ptrResult._t_2Pay = value; }
   }		
   /* 26 */ public decimal R_NaRuke
   { 
      get { return this._ptrResult._t_naRuke;         }
      set {        this._ptrResult._t_naRuke = value; }
   }	
   /* 27 */ public decimal R_ZdrNa
   { 
      get { return this._ptrResult._t_zdrNa;         }
      set {        this._ptrResult._t_zdrNa = value; }
   }	   
   /* 28 */ public decimal R_ZorNa
   { 
      get { return this._ptrResult._t_zorNa;         }
      set {        this._ptrResult._t_zorNa = value; }
   }	   
   /* 29 */ public decimal R_ZapNa
   { 
      get { return this._ptrResult._t_zapNa;         }
      set {        this._ptrResult._t_zapNa = value; }
   }	   
   /* 30 */ public decimal R_ZapII
   { 
      get { return this._ptrResult._t_zapII;         }
      set {        this._ptrResult._t_zapII = value; }
   }    
   /* 31 */ public decimal R_ZapAll
   { 
      get { return this._ptrResult._t_zapAll;         }
      set {        this._ptrResult._t_zapAll = value; }
   }	
   /* 32 */ public decimal R_DoprNa
   { 
      get { return this._ptrResult._t_doprNa;         }
      set {        this._ptrResult._t_doprNa = value; }
   }	
   /* 33 */ public decimal R_DoprAll
   { 
      get { return this._ptrResult._t_doprAll;         }
      set {        this._ptrResult._t_doprAll = value; }
   }  
   /* 34 */ public decimal R_SatiR
   { 
      get { return this._ptrResult._t_satiR;         }
      set {        this._ptrResult._t_satiR = value; }
   }
   /* 35 */ public decimal R_SatiB
   {
      get { return this._ptrResult._t_satiB;         }
      set {        this._ptrResult._t_satiB = value; }
   }  
   /* 36 */ public decimal R_SatiUk
   {
      get { return this._ptrResult._t_satiUk;         }
      set {        this._ptrResult._t_satiUk = value; }
   }  

   /* 37 */ public int     R_PtranEsCount
   {
      get { return this._ptrResult._t_numOfLinesPtrane; }
      set {        this._ptrResult._t_numOfLinesPtrane = value; }
   }  

   /* 38 */ public int     R_PtranOsCount
   {
      get { return this._ptrResult._t_numOfLinesPtrano; }
      set {        this._ptrResult._t_numOfLinesPtrano = value; }
   }  

   /* 39 */ public decimal R_FondSatiDiff
   {
      get { return this._ptrResult._t_fondSatiDiff; }
      set {        this._ptrResult._t_fondSatiDiff = value; }
   }  
   /* 39 */ public decimal R_FondSatiDiffABS
   {
      get { return this._ptrResult._t_fondSatiDiffABS; }
      set {        this._ptrResult._t_fondSatiDiffABS = value; }
   }  
   /* 40 */ public decimal R_Mio1stupNa
   { 
      get { return this._ptrResult._t_mio1stupNa;         }
      set {        this._ptrResult._t_mio1stupNa = value; }
   }	   
   /* 41 */ public decimal R_Mio2stupNa
   {
      get { return this._ptrResult._t_mio2stupNa; }
      set {        this._ptrResult._t_mio2stupNa = value; }
   }	
   /* 42 */ public decimal R_MioAllNa
   {
      get { return this._ptrResult._t_mioAllNa; }
      set {        this._ptrResult._t_mioAllNa = value; }
   }

   /* 43 */ public decimal R_KrizPorOsn
   {
      get { return this._ptrResult._t_krizPorOsn; }
      set {        this._ptrResult._t_krizPorOsn = value; }
   }

   /* 44 */ public decimal R_KrizPorUk
   {
      get { return this._ptrResult._t_krizPorUk; }
      set {        this._ptrResult._t_krizPorUk = value; }
   }

   /* 45 */ public decimal R_ZpiUk
   {
      get { return this._ptrResult._t_zpiUk; }
      set {        this._ptrResult._t_zpiUk = value; }
   }
   /* 46 */ public decimal R_DaniZpi
   { 
      get { return this._ptrResult._t_daniZpi;         }
      set {        this._ptrResult._t_daniZpi = value; }
   }

   /* 47 */ public decimal R_NettoWoAdd
   { 
      get { return this._ptrResult._t_nettoWoAdd;         }
      set {        this._ptrResult._t_nettoWoAdd = value; }
   }	   

   /* 48 */ public decimal R_AHizdatak
   { 
      get { return this._ptrResult._t_AHizdatak;         }
      set {        this._ptrResult._t_AHizdatak = value; }
   }	   

   /* 49 */ public decimal R_NettoAftKrp
   { 
      get { return this._ptrResult._t_nettoAftKrp;         }
      set {        this._ptrResult._t_nettoAftKrp = value; }
   }	   

   /* 50 */ public decimal R_BrtDodNaStaz
   { 
      get { return this._ptrResult._t_brtDodNaStaz;         }
      set {        this._ptrResult._t_brtDodNaStaz = value; }
   }

   /* 51 */ public int R_min_rsOd
   { 
      get { return this._ptrResult._t_min_rsOd;         }
      set {        this._ptrResult._t_min_rsOd = value; }
   }

   /* 52 */ public int R_max_rsDo
   { 
      get { return this._ptrResult._t_max_rsDo;         }
      set {        this._ptrResult._t_max_rsDo = value; }
   }

   /* 53 */ public decimal R_praznikHrs
   { 
      get { return this._ptrResult._t_praznikHrs;         }
      set {        this._ptrResult._t_praznikHrs = value; }
   }

   /* 54 */ public decimal R_thisStazDod
   { 
      get { return this._ptrResult._t_thisStazDod;         }
      set {        this._ptrResult._t_thisStazDod = value; }
   }

   /* 55 */ public decimal R_brutoDod2 { get { return this._ptrResult._t_brutoDod2; } set { this._ptrResult._t_brutoDod2 = value; } }
   /* 56 */ public decimal R_brutoDod3 { get { return this._ptrResult._t_brutoDod3; } set { this._ptrResult._t_brutoDod3 = value; } }

   /* 57 */ public decimal R_SatiNeR     { get { return this._ptrResult._t_satiNeR;    } set {  this._ptrResult._t_satiNeR     = value;  } }
   /* 58 */ public decimal R_ukBrutoKoef { get { return this._ptrResult._t_ukBrutoKoef;} set {  this._ptrResult._t_ukBrutoKoef = value;  } }

   /* 59 */ public decimal R_SatiOnlyRad      { get { return this._ptrResult._t_satiOnlyRad     ;} set {  this._ptrResult._t_satiOnlyRad      = value;  } }
   /* 60 */ public decimal R_SatiOnlyRadBruto { get { return this._ptrResult._t_satiOnlyRadBruto;} set {  this._ptrResult._t_satiOnlyRadBruto = value;  } }
   /* 61 */ public decimal R_PraznikHrsBruto  { get { return this._ptrResult._t_praznikHrsBruto ;} set {  this._ptrResult._t_praznikHrsBruto  = value;  } }
   /* 62 */ public decimal R_SatiNeRBruto     { get { return this._ptrResult._t_satiNeRBruto    ;} set {  this._ptrResult._t_satiNeRBruto     = value;  } }
   /* 63 */ public decimal R_brutoPoEVR       { get { return this._ptrResult._t_brutoPoEVR      ;} set {  this._ptrResult._t_brutoPoEVR       = value;  } } // BRUTO na osnovu EVR, odnosno T_BrutoOsn  - prije svih mogucih dodataka
   /* 64 */ public decimal R_osnovicaDop      { get { return this._ptrResult._t_osnovicaDop     ;} set {  this._ptrResult._t_osnovicaDop      = value;  } } // osnovica za doprinose NA placu koja moze biti razlicita od osnMio

   
  /* 65 */ public decimal R_Sati12	         { get { return this._ptrResult._t_Sati12	         ;} set {  this._ptrResult._t_Sati12	          = value;  } } // Obrazac IP1/NP1  Sati12	       
  /* 66 */ public decimal R_Bruto12          { get { return this._ptrResult._t_Bruto12          ;} set {  this._ptrResult._t_Bruto12          = value;  } } // Obrazac IP1/NP1  Bruto12       
  /* 67 */ public decimal R_Sati13	         { get { return this._ptrResult._t_Sati13	         ;} set {  this._ptrResult._t_Sati13	          = value;  } } // Obrazac IP1/NP1  Sati13	       
  /* 68 */ public decimal R_Bruto13          { get { return this._ptrResult._t_Bruto13          ;} set {  this._ptrResult._t_Bruto13          = value;  } } // Obrazac IP1/NP1  Bruto13       
  /* 69 */ public decimal R_Sati14	         { get { return this._ptrResult._t_Sati14	         ;} set {  this._ptrResult._t_Sati14	          = value;  } } // Obrazac IP1/NP1  Sati14	       
  /* 70 */ public decimal R_Bruto14          { get { return this._ptrResult._t_Bruto14          ;} set {  this._ptrResult._t_Bruto14          = value;  } } // Obrazac IP1/NP1  Bruto14       
  /* 71 */ public decimal R_Sati15	         { get { return this._ptrResult._t_Sati15	         ;} set {  this._ptrResult._t_Sati15	          = value;  } } // Obrazac IP1/NP1  Sati15	       
  /* 72 */ public decimal R_Bruto15          { get { return this._ptrResult._t_Bruto15          ;} set {  this._ptrResult._t_Bruto15          = value;  } } // Obrazac IP1/NP1  Bruto15       
  /* 73 */ public decimal R_Sati16           { get { return this._ptrResult._t_Sati16           ;} set {  this._ptrResult._t_Sati16           = value;  } } // Obrazac IP1/NP1  Sati16        
  /* 74 */ public decimal R_Bruto16          { get { return this._ptrResult._t_Bruto16          ;} set {  this._ptrResult._t_Bruto16          = value;  } } // Obrazac IP1/NP1  Bruto16       
  /* 75 */ public decimal R_Sati17           { get { return this._ptrResult._t_Sati17           ;} set {  this._ptrResult._t_Sati17           = value;  } } // Obrazac IP1/NP1  Sati17        
  /* 76 */ public decimal R_Bruto17          { get { return this._ptrResult._t_Bruto17          ;} set {  this._ptrResult._t_Bruto17          = value;  } } // Obrazac IP1/NP1  Bruto17       
  /* 77 */ public decimal R_Sati20           { get { return this._ptrResult._t_Sati20           ;} set {  this._ptrResult._t_Sati20           = value;  } } // Obrazac IP1/NP1  Sati20        
  /* 78 */ public decimal R_Bruto20          { get { return this._ptrResult._t_Bruto20          ;} set {  this._ptrResult._t_Bruto20          = value;  } } // Obrazac IP1/NP1  Bruto20       
  /* 79 */ public decimal R_Sati30           { get { return this._ptrResult._t_Sati30           ;} set {  this._ptrResult._t_Sati30           = value;  } } // Obrazac IP1/NP1  Sati30        
  /* 80 */ public decimal R_Bruto30          { get { return this._ptrResult._t_Bruto30          ;} set {  this._ptrResult._t_Bruto30          = value;  } } // Obrazac IP1/NP1  Bruto30       
  /* 81 */ public decimal R_Sati40           { get { return this._ptrResult._t_Sati40           ;} set {  this._ptrResult._t_Sati40           = value;  } } // Obrazac IP1/NP1  Sati40        
  /* 82 */ public decimal R_Bruto40          { get { return this._ptrResult._t_Bruto40          ;} set {  this._ptrResult._t_Bruto40          = value;  } } // Obrazac IP1/NP1  Bruto40       
  /* 83 */ public decimal R_Sum1do3          { get { return this._ptrResult._t_Sum1do3          ;} set {  this._ptrResult._t_Sum1do3          = value;  } } // Obrazac IP1/NP1  Sum1do3       
  /* 84 */ public decimal R_ZasticeniNeto    { get { return this._ptrResult._t_ZasticeniNeto    ;} set {  this._ptrResult._t_ZasticeniNeto    = value;  } } // Obrazac IP1/NP1  ZasticeniNeto - iznos zasticenog neta koji radnik dobiva na svoj zasticeni racun (preko Z-tocan iznos iz ptrano ili preko N-zasticeni iznos je ostatak i racun je prikazan na radniku)
  /* 85 */ public decimal R_NetoIsplata      { get { return this._ptrResult._t_NetoIsplata      ;} set {  this._ptrResult._t_NetoIsplata      = value;  } } // Obrazac IP1/NP1  NetoIsplata  - ono sto je radnik dobio na svoje racune > na zasticeni i na nezasticeni  (ukpan neto umanjen za obustave)
  /* 86 */ public decimal R_OnlyObustave     { get { return this._ptrResult._t_OnlyObustave     ;} set {  this._ptrResult._t_OnlyObustave     = value;  } } // Obrazac IP1/NP1  OnlyObustave - ciste obustave - nema oznake Z ili N 
  /* 87 */ public decimal R_Bruto30E         { get { return this._ptrResult._t_Bruto30E         ;} set {  this._ptrResult._t_Bruto30E         = value;  } } // Obrazac IP1/NP1  Bruto30E      
  /* 88 */ public decimal R_Bruto30D         { get { return this._ptrResult._t_Bruto30D         ;} set {  this._ptrResult._t_Bruto30D         = value;  } } // Obrazac IP1/NP1  Bruto30D      
  /* 89 */ public decimal R_Sati18           { get { return this._ptrResult._t_Sati18           ;} set {  this._ptrResult._t_Sati18           = value;  } } // Obrazac IP1/NP1  Sati18        
  /* 90 */ public decimal R_Bruto18          { get { return this._ptrResult._t_Bruto18          ;} set {  this._ptrResult._t_Bruto18          = value;  } } // Obrazac IP1/NP1  Bruto18       

  /* 91 */ public string  R_porIBAN          { get { return this._ptrResult._t_porIBAN          ;} set {  this._ptrResult._t_porIBAN          = value;  } } // Obrazac NP1  porezPrirez IBAN  
  /* 92 */ public decimal R_NetoWoZast       { get { return this._ptrResult._t_NetoWoZast       ;} set {  this._ptrResult._t_NetoWoZast       = value;  } } // Obrazac IP1  neto koji ide na nezasticeni racun uvecan za hzzo i prijevoz (za one kojei koriste Z - trebali bi u Z unjeti i te neto dodatke u zasticeni rn kao Z)       
  /* 93 */ public bool    R_hasNonObust      { get { return this._ptrResult._t_hasNonObust      ;} set {  this._ptrResult._t_hasNonObust      = value;  } } // Ima li ptrano ikoji da nije obustava, vec tekuci za neto (zasticeni ili obicni) 

   /* 95 */ public decimal R_Netto_EUR
   { 
      get { return this._ptrResult._t_netto_EUR;         }
      set {        this._ptrResult._t_netto_EUR = value; }
   }	   

   /* 96 */ public decimal R_Netto_Kn
   { 
      get { return this._ptrResult._t_netto_Kn;         }
      set {        this._ptrResult._t_netto_Kn = value; }
   }	   

  // !!! Ubuduce ako treba neka 'R_' varijabla, NE treba je dodavati u _ptrResult structuru ?! 
  // OSIM ako ne trebas taj R_ pokazati na DUC-u u 'PutDgvLineResultsFields1' 
  // 05.04.2017: !!! ALI PAZI !!! jer se numericke vrijable ne NULIRAJU ako su izvan PtrResult strukture a sto 
  // se dogada na kriticnom npr. mjestu u CalcTransResults() sa 'this.PtrResults = new PtransResultStruct();'
  public bool R_hasNonObustN { get; set; }     // Obrazac IP1 i NP1 - nezasticeni racun na Obustavama "N"
  public bool R_hasNonObustZ { get; set; }     // Obrazac IP1 i NP1 -   zasticeni racun na Obustavama "Z"

  public decimal R_NetoALLNeto  { get; set; }  // obrazac NP1 - svi ali bas svi neto > i onaj dobiven iz bruta i onaj od hzzo i prijevoz 01.07.2015.
  public decimal R_NezasticNeto { get; set; }  // cisti neto koji ide na nezasticeni racun - tocan iznos koji sjedan an racun

  public bool    R_isONPN     { get; set; }                                           // RAD1 - Ostale naknade, potpore i nagrade       
  public decimal R_netto_ONPN { get { return R_isONPN ? R_Netto    : 0M;         } }  // RAD1 - Ostale naknade, potpore i nagrade       
  public decimal R_bruto_ONPN { get { return R_isONPN ? R_TheBruto : 0M;         } }  // RAD1 - Ostale naknade, potpore i nagrade       
  public decimal R_satiR_ONPN { get { return R_isONPN ? R_SatiR    : 0M;         } }  // RAD1 - Ostale naknade, potpore i nagrade       
  public decimal R_netto_PIVR { get { return R_isONPN ? 0M         : R_Netto   ; } }  // RAD1 - Placa i isplate vezane za rad (classic) 
  public decimal R_bruto_PIVR { get { return R_isONPN ? 0M         : R_TheBruto; } }  // RAD1 - Placa i isplate vezane za rad (classic) 
  public decimal R_satiR_PIVR { get { return R_isONPN ? 0M         : R_SatiR   ; } }  // RAD1 - Placa i isplate vezane za rad (classic) 

  public uint R_daniR { get; set; } //TT_PODUZETPLACA, TT_SAMODOPRINOS 
  public uint R_daniB { get; set; } //TT_PODUZETPLACA, TT_SAMODOPRINOS 

   /// <summary>
   /// Osnovica za iznos obustave po postotku iz neta / iz dohodka
   /// </summary>
   public decimal R_ObustOsn
   {
      get
      {
         // staro pravilo - osnovica 
         if(this.T_dokDate < ZXC.Date01122016) return this.R_Netto;

          KtoShemaPlacaDsc kspl = new KtoShemaPlacaDsc(ZXC.dscLuiLst_KtoShemaPlaca);

          if(kspl.Dsc_IsObustOsnDoh == false) return this.R_Netto;
          else                                return this.R_Dohodak;
      }
   }

   // !!! Kada dodajes ovdje neki 'R_zxc', ne zaboravi i u PtransDao.FillTypedDataRowResults()

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return " Placa: " + T_dokNum + " (" + T_dokDate.ToShortDateString() + ")" + 
           "\n   Ser: " + T_serial + 
           "\n PerCd: " + T_personCD + 
           "\nPerson: " + T_prezimeIme + "\n";
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<PtransStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<PtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<PtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<PtransStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<PtransStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Ptrans newObject = new Ptrans();

      Generic_CloneData<PtransStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      Generic_CloneResultData<PtransResultStruct>(this._ptrResult, ref newObject._ptrResult);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Ptrans MakeDeepCopy()
   {
      return (Ptrans)Clone();
   }

   #endregion

   #region CalcBrutoDaNetto

   public decimal CalcBrutoDaNetto(decimal wantedNetto, bool isAfterKrizPor, Placa _placa_rec)
   {
      Placa        placa_rec;
      AlreadySpentPtransInThisMonthStruct spent;

      placa_rec       = _placa_rec;
      PrulesStruct pr = placa_rec.Prules;
      spent           = PtransDao.GetAlreadySpentPtransInThisMonth(/*ZXC.TheMainDbConnection*/ZXC.TheVvForm.TheDbConnection, T_personCD, T_dokNum, T_dokDate);

      decimal zbStDopIz, X, Y, A, B, C, D, N, O, R, P, M, Z, U, 
              max1, max2, max3, maxO, maxP, dx1, dx2, dx3, dO, dP,
              calcBruto, a, b, c, d, dOP, 
              KP1, KP2, minKrizPor, maxKrizPor, granKrizPor,
              ux1, ux2, ux3, uO, uM, uP;
           

      ux1 = ux2 = ux3 = uO = uM = uP = 0.00M;   
          
   //isKoris4PlacaSpecial(&CURR_koris_rec, &pf);

      uO  = spent.Odbitak;
      ux1 = spent.PorOsn1;
      ux2 = spent.PorOsn2;
      ux3 = spent.PorOsn3;
      uM  = spent.MioOsn;

      zbStDopIz = pr._stMio1stup + pr._stMio2stup;
      Z =        zbStDopIz    / 100.00M;
      X = 1.00M - (zbStDopIz) / 100.00M;
      Y = 1.00M + T_stPrirez  / 100.00M;
      A = pr._stpor1 / 100.00M;
      B = pr._stpor2 / 100.00M;
      C = pr._stpor3 / 100.00M;
      D = pr._stpor4 / 100.00M;
      N = wantedNetto;
      O = pr._osnOdb;
      R = T_koef;
    //P = T_zivotno + T_dopZdr + T_dobMIO; 22.04.2016. t_zivtno koristimo za rbrJop pa de se ne bi ovdje uzimalo u obzir
      P = (T_dokDate < new DateTime(2010, 07, 01)) ? T_zivotno + T_dopZdr + T_dobMIO : 0.00M;
      M = pr._maxMioOsn - uM;
      U = pr._stOthOlak / 100.00M;
      
      KP1 = pr._stKrizPor1 / 100.00M;
      KP2 = pr._stKrizPor2 / 100.00M;
      minKrizPor   = 3000.00M;
      maxKrizPor   = 6000.00M;

      granKrizPor = maxKrizPor * (1- KP1);

      a = 1.00M - A * Y;
      b = 1.00M - B * Y;
      c = 1.00M - C * Y;
      d = 1.00M - D * Y;

      if(P > (0.70M * pr._osnOdb)) P = 0.70M * pr._osnOdb;

      max1 = pr._maxPorOsn1;
      max2 = pr._maxPorOsn2 - pr._maxPorOsn1;
      max3 = pr._maxPorOsn3 - pr._maxPorOsn2;
      maxO = O * R;
      maxP = P;

      dx1 = max1 - ux1;
      dx2 = max2 - ux2;
      dx3 = max3 - ux3;
      dO  = maxO - uO;
      dP  = maxP - uP;
      dOP = dO   + dP;

      // TTDO: dodaci na bruto ....
      // TTDO: dodaci na neto  ....

      if(isAfterKrizPor) // mora se korigirat sa P kad premije "utjecu" na krizni porez
      {
         granKrizPor = maxKrizPor * (1 - KP1) + P + P * (1 - KP1);

         if(minKrizPor < wantedNetto && wantedNetto <= granKrizPor)
         {
            N = wantedNetto / (1 - KP1) + P - P / (1 - KP1); 
         }
         else if(wantedNetto > granKrizPor)
         {
            N = wantedNetto / (1 - KP2) + P - P / (1 - KP2);
         }
         else
         {
            N = wantedNetto;
         }
      }
      else
      {
         N = wantedNetto;
      }


      if(placa_rec.TT == Placa.TT_UGOVORODJELU || placa_rec.TT == Placa.TT_NADZORODBOR || placa_rec.TT == Placa.TT_TURSITVIJECE)
      {
         if(this.T_spc == SpecEnum.PENZ) // PENZici NE placaju MIO i ZDR
         {
            X = 1.00M;
         }

         if((X * (1.00M - B * Y)) != 0.00M)
         {
            calcBruto = N / (X * (1.00M - B * Y));// if(calcBruto > M) calcBruto = (N + M*Z * (1.0 - B*Y)) / (1.0 -B*Y);
         }
         else
         {
            calcBruto = 0.00M;
         }
      }
      else if(placa_rec.TT == Placa.TT_AUTORHONOR || placa_rec.TT == Placa.TT_AUTORHONUMJ || placa_rec.TT == Placa.TT_AUVECASTOPA)
      {
         if((1.00M - (1.00M - U) * B * Y) != 0.00M)
         {
            calcBruto = N / (1.00M - (1.00M - U) * B * Y);
         }
         else
         {
            calcBruto = 0.00M;
         }
      }
      else
      { // pravi 'ordinary' obracun bruta iz neta, dakle nije ni 'N' ni 'U' ni 'A' ni whatever... 

         if(N <= (dOP))
         {
            if(X != 0.00M) calcBruto = N / X;
            else calcBruto = 0.00M;
         }
         else if(dOP < N && N <= (dx1 * a + dOP))
         {
            if((X * a) != 0.00M)
            {
               calcBruto = (N - dOP * A * Y) / (X * a);

               if(calcBruto > M)
                  calcBruto = (N + M * Z * a - dOP * A * Y) / (a);
            }
            else calcBruto = 0.00M;
         }
         else if((dx1 * a + dOP) < N && N <= (dx1 * a + dx2 * b + dOP))
         {
            if((X * b) != 0.00M)
            {
               calcBruto = (N - dOP * B * Y - dx1 * Y * (B - A)) / (X * b);

               if(calcBruto > M)
                  calcBruto = (N + M * Z * b - dOP * B * Y - dx1 * Y * (B - A)) / (b);
            }
            else calcBruto = 0.00M;

         }
         else if((dx1 * a + dx2 * b + dOP) < N && N <= (dx1 * a + dx2 * b + dx3 * c + dOP))
         {
            if((X * c) != 0.00M)
            {
               calcBruto = (N - dOP * C * Y - dx1 * Y * (C - A) - dx2 * Y * (C - B)) / (X * c);
               if(calcBruto > M)
                  calcBruto = (N + M * Z * c - dOP * C * Y - dx1 * Y * (C - A) - dx2 * Y * (C - B)) / (c);
            }
            else calcBruto = 0.00M;
         }
         else if(N > (dx1 * a + dx2 * b + dx3 * c + dOP))
         {
            if((X * d) != 0.00M)
            {
               calcBruto = (N - dOP * D * Y - dx1 * Y * (D - A) - dx2 * Y * (D - B) - dx3 * Y * (D - C)) / (X * d);
               if(calcBruto > M)
                  calcBruto = (N + M * Z * d - dOP * D * Y - dx1 * Y * (D - A) - dx2 * Y * (D - B) - dx3 * Y * (D - C)) / (d);
            }
            else calcBruto = 0.00M;
         }
         else
            calcBruto = 0.00M;
      }

      return calcBruto;
   }

   #endregion CalcBrutoDaNetto

   #region CalcTransResults()

   decimal maxOsn1, maxOsn2, maxOsn3;

   internal static string GetVrstaOsobPrimanjaForZNPplaca()
   {
      return "100"; // TODO: ??? 

      // ŠIFARNIK VRSTA OSOBNIH PRIMANJA
      // svibanj 2013.
      // Šifra Vrste osobnih primanja

      // 100 Osobno primanje isplaćeno u cijelosti
      // 110 Isplata dijela osobnog primanja
      // 120 Osobno primanje umanjeno za zaštićeni dio
      // 130 Ugovor o djelu
      // 140 Rad za vrijeme školovanja
      // 150 Isplata dividende
      // 160 Naknada članova Upravnog vijeća, Skupština, Nadzornih odbora
      // 170 Primanja od iznajmljivanja turističkih kapaciteta
      // 180 Najam
      // 190 Prijevoz
      // 200 Službeni put
      // 210 Terenski dodatak
      // 220 Naknada za odvojeni život
      // 230 Naknada za bolovanje
      // 240 Naknada za korištenje privatnog automobila u službene svrhe
      // 250 Naknada za prekovremeni rad, bonusi, stimulacije, ostale nagrade
      // 260 Regres
      // 270 Božićnica, uskrsnica
      // 280 Dječji dar
      // 290 Stipendije, pomoć studentima/učenicima za opremu, knjige i ostalo
      // 300 Pomoć u slučaju stupanja u brak, smrti zaposlenika/člana obitelji zaposlenika
      // 310 Pomoć u slučaju rođenja djeteta
      // 320 Otpremnina
      // 399 Ostala osobna primanja
   }

   internal struct AlreadySpentPtransInThisMonthStruct
   {
      internal decimal Odbitak    { get; set; }
      internal decimal PorOsn1    { get; set; }
      internal decimal PorOsn2    { get; set; }
      internal decimal PorOsn3    { get; set; }
      internal decimal MioOsn     { get; set; }
      internal decimal KrizPorOsn { get; set; }
      internal decimal KrizPorUk  { get; set; }
   }

   public override void CalcTransResults(VvDocumentRecord _vvDocumentRecord)
   {
      Placa        placa_rec;
      Ptrane[] ptranEsOfThisPtrans;
      Ptrano[] ptranOsOfThisPtrans;
      AlreadySpentPtransInThisMonthStruct spent;

      placa_rec           = _vvDocumentRecord as Placa;
      /*!!*/PrulesStruct pRules = placa_rec.Prules;
      
      // 14.02.2014: 
    //ptranEsOfThisPtrans = placa_rec.Transes2.Where(ptrane => ptrane.T_personCD == this.T_personCD)                                   .ToArray();
    //ptranOsOfThisPtrans = placa_rec.Transes3.Where(ptrano => ptrano.T_personCD == this.T_personCD)                                   .ToArray();
      // 05.03.2015: 
    //ptranEsOfThisPtrans = placa_rec.TransesNonDeleted2.Where(ptrane => ptrane.T_personCD == this.T_personCD)                         .ToArray();
    //ptranOsOfThisPtrans = placa_rec.TransesNonDeleted3.Where(ptrano => ptrano.T_personCD == this.T_personCD)                         .ToArray();
      ptranEsOfThisPtrans = placa_rec.TransesNonDeleted2.Where(ptrane => ptrane.T_personCD == this.T_personCD).OrderBy(p => p.T_serial).ToArray();
      ptranOsOfThisPtrans = placa_rec.TransesNonDeleted3.Where(ptrano => ptrano.T_personCD == this.T_personCD).OrderBy(p => p.T_serial).ToArray();

      // 30.03.2021: opkolili get spent if() om ... da racuna samo za RR,PP,PN,ND,BR
      if(placa_rec.IsRRsetTT)
      {
       //spent = PtransDao.GetAlreadySpentPtransInThisMonth(ZXC.TheMainDbConnection      , T_personCD, T_dokNum, T_dokDate);
         spent = PtransDao.GetAlreadySpentPtransInThisMonth(ZXC.TheVvForm.TheDbConnection, T_personCD, T_dokNum, T_dokDate);
      }
      else
      {
         spent = new AlreadySpentPtransInThisMonthStruct();
      }

      bool anyT_staz_Exists = placa_rec.TransesNonDeleted.Any(ptr => ptr.T_godStaza.NotZero());

      //################################################################################################################################## 

      this.PtrResults = new PtransResultStruct();

      // 05.04.2017: start
      // tu traba nulirati sve ikada novododane get; set; numericke propertyje 
      R_NetoALLNeto  = 0M;
      R_NezasticNeto = 0M;
      R_daniR        = 0 ;
      R_daniB        = 0 ;
      // 05.04.2017: end

      #region isTURZGML

      // 28.10.2015: suzy u 10.2015. mijenja Prjkt IsOvoOno pa treba moci rekonstruirati stare place START 
      bool isTURZGML = ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TURZML;
      bool orig_IsObrStazaLast = ZXC.CURR_prjkt_rec.IsObrStazaLast;
      bool orig_IsSkipStzOnBol = ZXC.CURR_prjkt_rec.IsSkipStzOnBol;
      bool orig_IsFullStzOnPol = ZXC.CURR_prjkt_rec.IsFullStzOnPol;
      if(isTURZGML && T_dokDate < ZXC.Date02102015)
      {
         ZXC.CURR_prjkt_rec.IsObrStazaLast = false;
         ZXC.CURR_prjkt_rec.IsSkipStzOnBol = false;
         ZXC.CURR_prjkt_rec.IsFullStzOnPol = false;
      }
      // 28.10.2015: suzy u 10.2015. mijenja Prjkt IsOvoOno pa treba moci rekonstruirati stare place END 

      #endregion isTURZGML

      Calc_Sati                                  (    pRules, ptranEsOfThisPtrans, placa_rec.IsTrgFondSati, placa_rec.FondSati, DateTime.DaysInMonth(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month));
      if(T_dokDate < ZXC.Date01032015) // OLD - za stare, krive place ali da ostanu nepromjenjene 
         Calc_TheBrutoOLD                        (    pRules, ptranEsOfThisPtrans, placa_rec.FondSati, anyT_staz_Exists);
      else // classic - NEW 
         Calc_TheBruto                           (    pRules, ptranEsOfThisPtrans, placa_rec.FondSati, anyT_staz_Exists);

      if(T_dokDate < ZXC.Date01012017)
      {
         Calc_OtherDohodakOrPenzOrNovozap_Overriders_Bef2017(ref pRules,        placa_rec.TT);
         Calc_Doprinosi_Bef2017                             (    pRules, spent, placa_rec.TT);
      }
      else
      {
         Calc_OtherDohodakOrPenzOrNovozap_Overriders(ref pRules,        placa_rec.TT);
         Calc_Doprinosi                             (    pRules, spent, placa_rec.TT);
      }

      Calc_DohodakOdbitakPorOsn                  (    pRules, spent, placa_rec.TT);
      Calc_PorezPrirez                           (    pRules, spent, placa_rec.TT);
      Calc_Netto                                 (            ptranOsOfThisPtrans);
      Calc_KrizniPorez                           (    pRules, spent              );
      Calc_Obustave                              (            ptranOsOfThisPtrans);
      Calc_ToPay_NaRuke                          (                               );
    //Calc_RSm_StranaBvalues                     (            ptranEsOfThisPtrans);

      // 29.06.2015: 
      Set_PorIBAN();

      #region isTURZGML

      if(isTURZGML && T_dokDate < ZXC.Date02102015)
      {
         ZXC.CURR_prjkt_rec.IsObrStazaLast = orig_IsObrStazaLast;
         ZXC.CURR_prjkt_rec.IsSkipStzOnBol = orig_IsSkipStzOnBol;
         ZXC.CURR_prjkt_rec.IsFullStzOnPol = orig_IsFullStzOnPol;
      }

      #endregion isTURZGML

      //################################################################################################################################## 
   }

   private void Set_PorIBAN()
   {
      string ziro2;

      string str4ISOcheck = ZXC.GetStr4ISOcheck(this.T_opcCD, "1200");

      ziro2 = "1001005-" + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);

      ziro2 = ZXC.GetIBANfromOldZiro(ziro2);

      R_porIBAN = ziro2;
   }

   private void Calc_Sati(PrulesStruct pR, Ptrane[] ptranesOfThisPtrans, bool isTrgFondSati, decimal fondSati, int daysInMonth)
   {
      VvLookUpItem theLui;
      bool jeBolovanje = false;

      R_PtranEsCount = ptranesOfThisPtrans.Count();

      // 10.09.2014: 
      if(this.T_dnFondSati.NotZero())
      {
         fondSati *= (decimal)((decimal)this.T_dnFondSati / (decimal)Placa.SluzbeniDnevniFondSati);
      }

      if(ptranesOfThisPtrans.Length == 0) // ova persona nema nista u EVR. SatiR bude cijeli fond radnih sati doticnog mjeseca 
      {
         R_SatiR = fondSati;
         R_SatiB = 0;

         R_min_rsOd = 1;
         R_max_rsDo = daysInMonth;

         R_daniR = (uint)daysInMonth;
         
         R_daniB = 0                ;

         R_praznikHrs = ZXC.GetSumaBlagdanskihRadnihSatiZaMjesec(T_MMYYYY, isTrgFondSati, T_IsPoluSat, T_dnFondSati);

         //28.01.2015. neodradeni sati rada
         R_SatiNeR = R_praznikHrs;

         R_isONPN  = false;
      }
      else
      {
         try
         {
         R_min_rsOd = (int)ptranesOfThisPtrans.Where(p => p.T_rsOD.NotZero()).Select(ptrane => ptrane.T_rsOD).Min();
         }
         catch { R_min_rsOd = 0; } // ako greskom neko upise nesto u evr, a nigdje ne popuni T_rsOD 
         R_max_rsDo = (int)ptranesOfThisPtrans                               .Select(ptrane => ptrane.T_rsDO).Max();

         foreach(Ptrane ptrane_rec in ptranesOfThisPtrans)
         {

            #region ZPI

            if(ptrane_rec.T_vrstaR_cd == Ptrane.VrstaR_cd_thisIs_ZPI)
            {

               decimal mothHasDays_4ZPI = DateTime.DaysInMonth(this.T_MMYYYY_asDateTime.Year, this.T_MMYYYY_asDateTime.Month);
               decimal ZPI_osnovaPerDay = (pR._minMioOsn / mothHasDays_4ZPI);
               decimal daniZpi          = ptrane_rec.T_rsDO - ptrane_rec.T_rsOD + 1;
               decimal stopaZpi         = pR._stZpi/100;

               R_DaniZpi += daniZpi;

//               R_ZpiUk += (daniZpi * ZPI_osnovaPerDay).Ron2();
               R_ZpiUk += (daniZpi * ZPI_osnovaPerDay * stopaZpi).Ron2();
            }

            #endregion ZPI

            if(ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND) continue;

            theLui = ZXC.luiListaVrstaRadaEVR.SingleOrDefault(lui => lui.Cd == ptrane_rec.T_vrstaR_cd);

            if(theLui != null) jeBolovanje = theLui.Flag;

            if(jeBolovanje)
            {
             //R_SatiB += ZXC.GetWorkHoursCount(ZXC.CURR_prjkt_rec.IsTrgRs, T_IsPoluSat, T_MMYYYY, ptrano_rec.T_rsOD, ptrano_rec.T_rsDO);
               R_SatiB += ptrane_rec.T_sati;

               R_daniB += ptrane_rec.T_rsDO - ptrane_rec.T_rsOD + 1;
            }
            else
            {
             //R_SatiR += ZXC.GetWorkHoursCount(ZXC.CURR_prjkt_rec.IsTrgRs, T_IsPoluSat, T_MMYYYY, ptrano_rec.T_rsOD, ptrano_rec.T_rsDO);
               R_SatiR += ptrane_rec.T_sati;

               R_daniR += ptrane_rec.T_rsDO - ptrane_rec.T_rsOD + 1;

             //if(ptrane_rec.T_primDohCD == Ptrane.RAD1_ONPN_rootCD)          R_isONPN = true;
               if(ptrane_rec.T_primDohCD.StartsWith(Ptrane.RAD1_ONPN_rootCD)) R_isONPN = true;
            }

         } // foreach(Ptrane ptrane_rec in ptranesOfThisPerson) 
      }

      R_SatiUk = R_SatiR + R_SatiB;

      R_FondSatiDiff    = R_SatiUk - (T_IsPoluSat ? fondSati / 2 : fondSati);
      R_FondSatiDiffABS = R_SatiUk - (                             fondSati);
   }

   private void Calc_TheBruto(PrulesStruct pR, Ptrane[] ptranesOfThisPtrans, decimal fondSati, bool anyT_staz_Exists)
   {
      bool isJosavac = ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.JOSAVC;

      bool isNaProsjeku  = T_pr3mjBruto.NotZero();
      decimal brutoProsjekRadZaDodNaStaz = 0.00M;

      //24.01.2017. i strucno se od 01.01.2017. racuna prema stvarnim mjesecnim DANIMA a ne satima !!!!!
      bool isObr4Days = this.T_dokDate < ZXC.Date01012017 ? this.T_TT == Placa.TT_SAMODOPRINOS || this.T_TT == Placa.TT_PODUZETPLACA : 
                                                            this.T_TT == Placa.TT_SAMODOPRINOS || this.T_TT == Placa.TT_PODUZETPLACA || this.T_TT == Placa.TT_STRUCNOOSPOS;

      #region this.T_dnFondSati.NotZero()

         // 10.09.2014: 
         if(this.T_dnFondSati.NotZero())
      {
         fondSati *= (decimal)((decimal)this.T_dnFondSati / (decimal)Placa.SluzbeniDnevniFondSati);
      }

      #endregion this.T_dnFondSati.NotZero()

      #region ZXC.CURR_prjkt_rec.IsCheckStaz == true

      //R_Bruto100 = (T_brutoOsn + T_topObrok + T_dodBruto) * (1.00M + (T_godStaza * pR._stDodStaz / 100.00M));

      //14.04.2014. 
      // obr po starom ako je T_brutoDodSt =  0 : osnZaStaz = (T_brutoOsn + T_topObrok + T_dodBruto) a T_dodBruto je ukljucen u osnovicu 
      // obr new       ako je T_brutoDodSt <> 0 : osnZaStaz = (T_brutoOsn + T_topObrok)              a T_dodBruto se racuna u %         +
      //  T_brDodPoloz dodatak na bruto koji ne reagira na staz i slicno nego je jednostavan dodatak fiksni u punom iznosu

      // 19.09.2014: start      
      if(/*T_godStaza.NotZero()*/ ZXC.CURR_prjkt_rec.IsCheckStaz == true && anyT_staz_Exists == true)
      {
         Person person_rec = VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == this.T_personCD);

         if(person_rec != null)
         {
            uint calculatedGodineStaza = person_rec.CalcGodineStaza(this.T_MMYYYY, false);

            if(calculatedGodineStaza != T_godStaza)
            {
               person_rec.IssueGodineStazaWarning(T_godStaza, calculatedGodineStaza);
            }

         } // if(person_rec != null)
      } // if(T_godStaza.NotZero())
      // 19.09.2014: end      

      #endregion ZXC.CURR_prjkt_rec.IsCheckStaz == true

      #region brutoDod_aci
      // 26.01.2015: (Kerempuh) 
      R_brutoDod2 = ZXC.VvGet_25_of_100(T_brutoOsn, T_brutoDodSt2).Ron2();
      R_brutoDod3 = ZXC.VvGet_25_of_100(T_brutoOsn, T_brutoDodSt3).Ron2();

      //28.01.2015. dodani novi 
      R_ukBrutoKoef = T_koefBruto1 + ZXC.VvGet_25_of_100(T_koefBruto1, T_brutoDodSt2).Ron2() + ZXC.VvGet_25_of_100(T_koefBruto1, T_brutoDodSt3).Ron2();

      //11.02.2014: 
      if(T_brutoDodSt.NotZero())
      {
       //decimal osnovicaZaBrutoDodatak = T_brutoOsn + T_topObrok;
      // 12.02.2014. ovako bi trebalo biti da izracun bude ok ali mi javlja greske kod stare place
         decimal dodStaz = (T_brutoOsn + T_topObrok) * (T_godStaza * pR._stDodStaz / 100.00M);
         decimal osnovicaZaBrutoDodatak = T_brutoOsn + T_topObrok + dodStaz;

         // 28.02.2014: !!! dok nisi dodao ron2, nije mogao refreshati record jer je u calcu neki put dolazilo npr. 1340.2350000 a u fajlu je 1340.24 !!!!!!!!
       //T_dodBruto = ZXC.VvGet_25_on_100(osnovicaZaBrutoDodatak, T_brutoDodSt);
         T_dodBruto = ZXC.VvGet_25_of_100(osnovicaZaBrutoDodatak, T_brutoDodSt).Ron2(); // !!! 15.10.2014: OVO JE VELIKI POTERNCIJALNI IZVOR PROBLEMA !!! 
      }

      #endregion brutoDod_aci



      //==== Calc TheBruto via EVR   START ============================================================ 

      decimal cumulatedThe_Bruto = 0.00M;
      decimal Bkratko_sati       = 0M; 
      bool    isBkratko          = false;
      bool    isGodOdm           = false;

      decimal cijenaSata100 = ZXC.DivSafe(T_brutoOsn, (T_IsPoluSat ? fondSati / 2 : fondSati));

      // dani  09.02.2016:                                     
      decimal mmyyyy_fondDana = DateTime.DaysInMonth(this.T_MMYYYY_asDateTime.Year, this.T_MMYYYY_asDateTime.Month);
      decimal cijenaDana100   = 0M;

    //if(this.T_TT == Placa.TT_SAMODOPRINOS || this.T_TT == Placa.TT_PODUZETPLACA) 24.01.2017.
      if(isObr4Days)
      {
         cijenaDana100 = ZXC.DivSafe(T_brutoOsn, (T_IsPoluSat ? mmyyyy_fondDana / 2 : mmyyyy_fondDana));

         // !!! tu smo odlucili da za samo_doprin potrebe je dovoljno samo cijeniSata100 podvaliti cijenuDana100        
         // ubuduce, ako ispadne da ipak treba nesto dole diferencirati... imas i cijenaSata100 i cijenaDana100 pa vidi 
         cijenaSata100 = cijenaDana100;
      }


      R_SatiOnlyRad  = R_SatiR;//02.02.2015. sati cistog rada
      R_SatiNeRBruto = 0;      // bruto sati koje radnika nije odradio - godisnji, bolovanje ....

      decimal evrCijenaOsnovica;

      foreach(Ptrane ptrane_rec in ptranesOfThisPtrans)
      {
         #region isBkratko
         // 23.09.2014: 
       //isBkratko = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
       //             ptrane_rec.T_cijPerc < 100M &&                              // Cijena manja od 100%
       //             ptrane_rec.T_vrstaR_name.ToUpper().Contains("BOLOVANJE"));
         isBkratko = Ptrane.IsBolDo42(ptrane_rec.T_rsOO, ptrane_rec.T_cijPerc, ptrane_rec.T_vrstaR_name);

         if(isBkratko) Bkratko_sati += ptrane_rec.T_sati;
         #endregion isBkratko

         #region isGodOdm

       //isGodOdm = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
       //            ptrane_rec.T_vrstaR_name.ToUpper().Contains("GODIŠNJI"));
         isGodOdm = Ptrane.IsGodisnjiOdmor(ptrane_rec.T_rsOO, ptrane_rec.T_vrstaR_name);

         #endregion isGodOdm

         #region HZTK prekovremeno

         //28.01.2021. HZTK hoce dodatak na staz i na prekovremene ali kada ima isNaProsjeku = true onda je program povecao dodatak na staz 
         bool isHZTKprekovremeno = ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_MTROS && ptrane_rec.T_rsOO == "98";
         
         #endregion HZTK prekovremeno


         // do 28.01.2015. a ovdje dodajemo obracun bolovanja na osnovu prosjeka tri zadnje place
         //if(ptrane_rec.T_rsOO != "99") ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc          ) / 100.00M * cijenaSata;
         //else /* prekovr., bolNaPosl */ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata;

         #region cijena sata

         if(ptrane_rec.T_rsOO != "99") // znaci ne ubraja se u redovan rad
         {
            evrCijenaOsnovica = cijenaSata100;

            // 16.12.2019: HZTK-i korigiramo osnovicu za prekovremene 
          //if(/*isHZTK*/ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_MTROS && ptrane_rec.T_rsOO == "98")
            if(isHZTKprekovremeno                                                                                ) //28.01.2021. samo sazeto
            {
               // problem je: R_BrtDodNaStaz 
               decimal BrtDodNaStaz;

               BrtDodNaStaz = (T_brutoOsn + T_topObrok) * (T_godStaza * pR._stDodStaz / 100.00M);
             //this.T_dokDate >= ZXC.Date01042021
             //31.03.2021. Filip kaze da polozajni dodatak T_brDodPoloz treba izbiti iz racunice za prekovremeno!!!!
             //decimal onoStoTrebaBitiUosnovici_aJosNije =                                     T_topObrok + T_dodBruto + BrtDodNaStaz +  T_brDodPoloz+/*R_thisStazDod +*/ T_brutoKorekc;
               decimal onoStoTrebaBitiUosnovici_aJosNije = this.T_dokDate < ZXC.Date01042021 ? T_topObrok + T_dodBruto + BrtDodNaStaz +   T_brDodPoloz                +   T_brutoKorekc:
                                                                                                            T_dodBruto + BrtDodNaStaz   /*T_brDodPoloz*/              +   T_brutoKorekc;

               decimal HZTK_korekcija_cijene_sata = ZXC.DivSafe(onoStoTrebaBitiUosnovici_aJosNije, (T_IsPoluSat ? fondSati / 2 : fondSati));

               evrCijenaOsnovica += HZTK_korekcija_cijene_sata;

            }

            ptrane_rec.R_EvrCijena  = (ptrane_rec.T_cijPerc) / 100.00M * evrCijenaOsnovica/*cijenaSata100*/;

            ptrane_rec.R_ThisEvrCijena = ptrane_rec.R_EvrCijena; //02.02.2015.
         
         }
         else if(isBkratko && T_pr3mjBruto.NotZero()) // obracun bolovanja na temelju prosjeka zadnje 3 place bruto
         {
            decimal cijSatPr3 = ZXC.DivSafe(T_pr3mjBruto, (T_IsPoluSat ? fondSati / 2 : fondSati)); //26.01.2015. cijena sata na osnovu prosjeka 3 mjesecne place

          // 20.03.2015.za obrazac CR_080710 - kerempuh da se vidi korekcija sata 
          //ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3;
            decimal cijSatPr3Evr    = cijSatPr3     + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3    ;
            decimal korekcijaCijeneSata = cijSatPr3Evr - cijenaSata100;
            ptrane_rec.R_EvrCijena = korekcijaCijeneSata;

            ptrane_rec.R_ThisEvrCijena = cijSatPr3 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3; //02.02.2015.
         }
         else if(isGodOdm && T_pr3mjBruto.NotZero()) // obracun godisnjeg odmora na temelju prosjeka zadnje 3 place bruto
         {
            decimal cijSatPr3 = ZXC.DivSafe(T_pr3mjBruto, (T_IsPoluSat ? fondSati / 2 : fondSati)); //04.02.2015. cijena sata na osnovu prosjeka 3 mjesecne place
            
            if(ptrane_rec.T_dokDate <= new DateTime(2015, 03, 04)) ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3; //05.03. za godisnji je 100% pa u ispada 0
          //else                                                   ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc == 100.00M) ? cijSatPr3 : (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3;
          //20.03.2015 za obrazac CR_080710 - kerempuh da se vidi korekcija sata
            else                                                   ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc == 100.00M) ? cijSatPr3-cijenaSata100 : (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3;

            ptrane_rec.R_ThisEvrCijena = cijSatPr3 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3; //04.02.2015.
         }
         else /* prekovr., bolNaPosl */
         {
            ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata100;

            ptrane_rec.R_ThisEvrCijena = cijenaSata100 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata100; //02.02.2015.
         }
         #endregion cijena sata

         ptrane_rec.R_EvrBruto = ptrane_rec.R_EvrCijena * ptrane_rec.T_sati;

         // dani  09.02.2016:                                     
       //if(this.T_TT == Placa.TT_SAMODOPRINOS || this.T_TT == Placa.TT_PODUZETPLACA) ptrane_rec.R_EvrBruto = ptrane_rec.R_EvrCijena * this.R_daniR; 24.01.2017.
         if(isObr4Days)                                                               ptrane_rec.R_EvrBruto = ptrane_rec.R_EvrCijena * this.R_daniR;
         
        // cumulatedThe_Bruto += ptrane_rec.R_EvrBruto.Ron2(); preselila dolje 05.03.2015.

         ptrane_rec.R_ThisEvrBruto = ptrane_rec.R_ThisEvrCijena * ptrane_rec.T_sati;


         //28.01.2015. neodradeni sati rada za novi JOPPD podatak 10.0.
         bool isDaniNerada = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND           &&   // "99" 
                             (ptrane_rec.T_cijPerc < 100M || ptrane_rec.T_cijPerc == 100M) &&   // Cijena manja ili jednaka 100% - ovo nsam sigurna jel potrebno
                             (ptrane_rec.T_vrstaR_cd != Ptrane.VrstaR_cd_thisIs_ZPI)       &&   // nije ZPI
                             (ptrane_rec.T_vrstaR_name.ToUpper().Contains("RAD") == false) 
                             );
         if(isDaniNerada)
         {
            R_SatiNeR      += ptrane_rec.T_sati;
            R_SatiNeRBruto += ptrane_rec.R_ThisEvrBruto;
         }
         
       //if(ptrane_rec.T_rsOO == "99") R_SatiOnlyRad -= ptrane_rec.T_sati; //02.02.2015. sati cistog rada 01-99 03.02.2015. i prekovremeni se oduzimaju
         if(ptrane_rec.T_rsOO == "99" || ptrane_rec.T_rsOO == "98" && ptrane_rec.T_vrstaR_cd != Ptrane.VrstaR_cd_thisIs_ZPI) R_SatiOnlyRad -= ptrane_rec.T_sati; //03.02.2015. sati cistog rada 01-99-98 jer i 89 pribraja na sate

         // 05.03.2015.
         if(T_pr3mjBruto.NotZero())
         {
            if(ptrane_rec.T_dokDate <= new DateTime(2015, 03, 04)) // jer su stare place po tome napravljene 05.03.2015. kako pomiriti i dragicu koja je isplatila i placu za 2 mjesec u trecem
            {
               cumulatedThe_Bruto += ptrane_rec.R_EvrBruto.Ron2();
            }
            else
            {
               if(ptrane_rec.T_rsOO == "10") cumulatedThe_Bruto += (ptrane_rec.R_EvrCijena * R_SatiOnlyRad).Ron2();
               else                          cumulatedThe_Bruto += ptrane_rec.R_ThisEvrBruto.Ron2();
            }
         }
         else
         {
            cumulatedThe_Bruto += ptrane_rec.R_EvrBruto.Ron2();
         }

         // do 17.03.
         //if(T_pr3mjBruto.NotZero() || ZXC.CURR_prjkt_rec.IsSkipStzOnBol/*dodano 13.03.2015 ako nije pr3mj a ipak se preskace bolovanje a racuna se dodatak na staz*/)
         //{
         //        if(isBkratko || isGodOdm    ) brutoProsjekRadZaDodNaStaz += 0.00M;
         //   else if(ptrane_rec.T_rsOO == "10") brutoProsjekRadZaDodNaStaz += (ptrane_rec.R_EvrCijena * R_SatiOnlyRad).Ron2();
         //   else                               brutoProsjekRadZaDodNaStaz += ptrane_rec.R_ThisEvrBruto.Ron2();
         //}
         // ovo dolje 2 ifa je glupo ALI onaj gornji if je bio dobar za kerempuh uyimao je i onaj IsSkipStzOnBol a treba ga uzeti i u obzir i kda nema bolovanja

         if(T_pr3mjBruto.NotZero())
         {
                 if(isBkratko || isGodOdm                                   ) brutoProsjekRadZaDodNaStaz += 0.00M;
            else if(ptrane_rec.T_rsOO == "10"                               ) brutoProsjekRadZaDodNaStaz += (ptrane_rec.R_EvrCijena * R_SatiOnlyRad).Ron2();
            else if(isHZTKprekovremeno && this.T_dokDate >= ZXC.Date01022021) //28.01.2021. korekcija korekcije prekovremenog koje je obracunat na osnovicom sa DodNaStaz da se taj dodatak izbaci kod obracuna osnovice dodZaStaz, linija 1746
            {
             // 12.02.2021. ovdje je krivo jer je dodavao na dodatak na satž i onaj koji je već obračunat u prekovremenom
             //             i zapravo ga treba izbiti, tj ne uračunati                                                   
             //brutoProsjekRadZaDodNaStaz += ((ptrane_rec.T_cijPerc) / 100.00M * cijenaSata100 * ptrane_rec.T_sati).Ron2(); 
               brutoProsjekRadZaDodNaStaz += 0.00M;

            }
            else brutoProsjekRadZaDodNaStaz += ptrane_rec.R_ThisEvrBruto.Ron2();
         }
         if(T_pr3mjBruto.IsZero() && ZXC.CURR_prjkt_rec.IsSkipStzOnBol)  
         {                       // ovo je zato sto josavac ima bolovanje preko 100%
                 if(isBkratko || ptrane_rec.T_vrstaR_name.ToUpper().Contains("BOLOVANJE")) brutoProsjekRadZaDodNaStaz += 0.00M;
            else if(ptrane_rec.T_rsOO == "10")                                             brutoProsjekRadZaDodNaStaz += (ptrane_rec.R_EvrCijena * R_SatiOnlyRad).Ron2();
            else                                                                           brutoProsjekRadZaDodNaStaz += ptrane_rec.R_ThisEvrBruto.Ron2();
         }

      } // foreach(Ptrane ptrane_rec in ptranesOfThisPtrans) 

      //==== Calc TheBruto via EVR   END ============================================================ 


      #region osnZaStaz

      // 24.09.2014: 
      R_thisStazDod = T_brutoOsn * (T_thisStazSt / 100.00M);

      decimal osnZaStaz;
      if(T_brutoDodSt.NotZero()) osnZaStaz = (T_brutoOsn + T_topObrok);              // HZTK 
      else                       osnZaStaz = (T_brutoOsn + T_topObrok + T_dodBruto); // Classic, kao uvijek do hztke 

      // 22.04.2015: 
      if(ZXC.CURR_prjkt_rec.IsObrStazaLast && ptranesOfThisPtrans.Length != 0) // ima nesto u EVR 
      {
         if((isJosavac && T_dokDate < ZXC.Date15032015) == false) // ispravak bug-a da josavac 'ZXC.CURR_prjkt_rec.IsObrStazaLast' nije imao cekirano prije 15.3.2015 
         osnZaStaz = (cumulatedThe_Bruto + T_topObrok + T_dodBruto);
      }

      R_BrtDodNaStaz = osnZaStaz * (T_godStaza * pR._stDodStaz / 100.00M);

      // ako nije radio cio mjesec: start 
      decimal priznatiRsati = R_SatiUk;

      //if(ZXC.CURR_prjkt_rec.IsSkipStzOnBol) priznatiRsati -= Bkratko_sati; 06.01.2015.  to je ok kada nebi ostatak bio na teret HZZO-a
      if(ZXC.CURR_prjkt_rec.IsSkipStzOnBol)
      {
         if((isJosavac && T_dokDate < ZXC.Date15032015) == false) // ispravak bug-a da josavac 'ZXC.CURR_prjkt_rec.IsObrStazaLast' nije imao cekirano prije 15.3.2015 
         {
            priznatiRsati -= Bkratko_sati;
            priznatiRsati -= R_SatiB;
         }
      }
      decimal ratio = ZXC.DivSafe(priznatiRsati, fondSati);
      // 23.092014: 
    //R_BrtDodNaStaz *= ratio;
      // 01.02.2016: 
    //if( T_IsPoluSat && ZXC.CURR_prjkt_rec.IsFullStzOnPol                                   )
      // 03.03.2016. Dragica - kada je radnik dosao na pola mjesca a (ZXC.CURR_prjkt_rec.IsObrStazaLast && ptranesOfThisPtrans.Length != 0) // ima nesto u EVR i onda je osnZaStaz = (cumulatedThe_Bruto + T_topObrok + T_dodBruto);
      //                       i u osnZaStaz je vec umanjena R_BrtDodNaStaz = osnZaStaz * (T_godStaza * pR._stDodStaz / 100.00M); a ovdje bi je dodatno umanjili ako bi otisli u else
    //if((T_IsPoluSat && ZXC.CURR_prjkt_rec.IsFullStzOnPol) || R_FondSatiDiffABS.IsPositive()) // dodajemo da u ovaj if ulazima i u slucaju prekovremenih 
      if((T_IsPoluSat && ZXC.CURR_prjkt_rec.IsFullStzOnPol) || R_FondSatiDiffABS.IsPositive() || (ZXC.CURR_prjkt_rec.IsObrStazaLast && ptranesOfThisPtrans.Length != 0)) 
      {
         // NE utjeci na R_BrtDodNaStaz ako je polusatnoRV i u Prjkt je zadano IsFullStzOnPol = true 
         //R_BrtDodNaStaz *= ratio;
      }
      else
      {
         R_BrtDodNaStaz *= ratio;
      }
      // ako nije radio cio mjesec: end 

      //!!!!!! mora prvo na evr staviti sve a O1 redovan rda mora doci kao zadnja stavka da bi dobro izracunao
      //if(isNaProsjeku) // !!!!!! mora prvo na evr staviti sve a O1 redovan rda mora doci kao zadnja stavka da bi dobro izracunao
      // 17.03.2015. nedobro za josavac
      //if(isNaProsjeku || ZXC.CURR_prjkt_rec.IsSkipStzOnBol/*dodano 13.03.2015 ako nije pr3mj a ipak se preskace bolovanje a racuna se dodatak na staz*/)
      //{
      //   R_BrtDodNaStaz = brutoProsjekRadZaDodNaStaz * (T_godStaza * pR._stDodStaz / 100.00M);
      //}

      if(isNaProsjeku)
      {
         R_BrtDodNaStaz = brutoProsjekRadZaDodNaStaz * (T_godStaza * pR._stDodStaz / 100.00M);
      }

      #region 'Josavac' & u nekom periodu, a krivo, i ostali

      bool periodBUGa = (ZXC.Date01042015 <= this.T_dokDate && this.T_dokDate <= ZXC.Date07042015);

      // 22.04.2015: 
    //if( isJosavac                                  || periodBUGa) //  za 'Josavac' uvijek, a i za ostale, greskom isplacene, u periodu 01.04.2015 - 07.04.2015 
      if((isJosavac && T_dokDate > ZXC.Date15032015) || periodBUGa) //  za 'Josavac' uvijek, a i za ostale, greskom isplacene, u periodu 01.04.2015 - 07.04.2015 
      {
         if(isNaProsjeku == false && ZXC.CURR_prjkt_rec.IsSkipStzOnBol && T_IsPoluSat == false)//17.03.2015. josavac ima komplikacije place
         {
            // 14.05.2015: 
            if(T_dokDate < ZXC.Date12052015) // 14.05. deployamo ispravak bug-a, a da stare ostanu krive 
               R_BrtDodNaStaz =  brutoProsjekRadZaDodNaStaz                            * (T_godStaza * pR._stDodStaz / 100.00M); 
            else
               R_BrtDodNaStaz = (brutoProsjekRadZaDodNaStaz + T_topObrok + T_dodBruto) * (T_godStaza * pR._stDodStaz / 100.00M);
         }
      }

      #endregion 'Josavac' & u nekom periodu, a krivo, i ostali

      #endregion osnZaStaz

      decimal osnObrokDodaci = (T_brutoOsn + T_topObrok + T_dodBruto);

    // 14.02.2014. 
    //R_Bruto100 = osnObrokDodaci + R_BrtDodNaStaz;
      R_Bruto100 = osnObrokDodaci + R_BrtDodNaStaz + T_brDodPoloz;

      R_Bruto100 = R_Bruto100.Ron2();

    //// 08.02.2016: 
    //decimal SAMODOPRINOS_ratio = 0M;
    //if(this.T_TT == Placa.TT_SAMODOPRINOS)
    //{
    //   SAMODOPRINOS_ratio = ZXC.DivSafe(R_daniR, DateTime.DaysInMonth(this.T_MMYYYY_asDateTime.Year, this.T_MMYYYY_asDateTime.Month));
    //
    //   R_Bruto100 *= SAMODOPRINOS_ratio;
    //}

      R_brutoPoEVR = cumulatedThe_Bruto; // cista osnovica po osnovu rada za one koji imaju evr a ako nemjau onda dojde dolje


      // 28.06.2015. za obrazac ip1 kada ima dodatke a nema evr
    //R_Bruto30D = T_topObrok + T_dodBruto + R_BrtDodNaStaz + T_brDodPoloz + R_thisStazDod + T_brutoKorekc + T_prijevoz + T_np63 + T_npTobrok               ; // 12.03.2019: dodan '+ T_np63' 06.09.2019. dodan T_npTobrok
      R_Bruto30D = T_topObrok + T_dodBruto + R_BrtDodNaStaz + T_brDodPoloz + R_thisStazDod + T_brutoKorekc + T_prijevoz + T_np63 + T_npTobrok + T_dopZdr2020; // 03.03.2020: dodan '+ T_dopZdr2020'

      if(ptranesOfThisPtrans.Length == 0)
      {
       // 30.09.2014. dodali mi R_thisStazDod na TheBruto a ovaj se nigdje ne kumulira dakle ide kao cisti dodatak na kraju svega,
       //             ali kad nema ptrane-a onda ni ne dojde do doljnjeg TheBruta
       //R_TheBruto = R_Bruto100;
         R_TheBruto = R_Bruto100 + R_thisStazDod + T_brutoKorekc;

       //02.02.2015. sati cistog rada bez praznika - ovo je kada nema evr a se sati praznika automatski racunaju

         //29.02.2016.
       //if(this.T_TT == Placa.TT_SAMODOPRINOS || this.T_TT == Placa.TT_PODUZETPLACA) 24.01.2017.
         if(isObr4Days)
         {
            R_SatiOnlyRad      = R_daniR;
            R_SatiOnlyRadBruto = T_brutoOsn;
            R_PraznikHrsBruto  = 0;
            R_SatiNeRBruto     = 0; 
            R_brutoPoEVR       = T_brutoOsn; // cista osnovica po osnovu rada

            #region ObrazacIP1 28.06.2015.

            R_Bruto30 = R_Bruto30D; // samo + 30D jer eventualno ima dodatke a nema evr
            R_Sati17  = 0;
            R_Bruto17 = 0.00M;
            R_Sum1do3 = R_SatiOnlyRadBruto + R_Bruto17 + R_Bruto30;

            #endregion ObrazacIP1 28.06.2015.

         }
         else
         {


            R_SatiOnlyRad     -= R_praznikHrs;
            R_SatiOnlyRadBruto = R_SatiOnlyRad * cijenaSata100;
            R_PraznikHrsBruto  = R_praznikHrs * cijenaSata100;
            R_SatiNeRBruto     = R_PraznikHrsBruto; //03.02.2015.

            R_brutoPoEVR       = T_brutoOsn; // cista osnovica po osnovu rada

            #region ObrazacIP1 28.06.2015.

            R_Bruto30 = R_Bruto30D; // samo + 30D jer eventualno ima dodatke a nema evr
            R_Sati17 = R_praznikHrs;
            R_Bruto17 = R_PraznikHrsBruto;
            R_Sum1do3 = R_SatiOnlyRadBruto + R_Bruto17 + R_Bruto30;

            #endregion ObrazacIP1 28.06.2015.

         }

         // 03.03.2017: !!! !!! !!! 
         R_TheBruto = R_TheBruto.Ron2();

         return;
      }

      R_SatiOnlyRadBruto = R_SatiOnlyRad * cijenaSata100;


      #region Obrazac IP1 ...28.06.2015.

      foreach(Ptrane ptrane_rec in ptranesOfThisPtrans)
      {
         switch(ptrane_rec.T_ip1gr)
         {
            case 12: R_Sati12 += ptrane_rec.T_sati; R_Bruto12  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 13: R_Sati13 += ptrane_rec.T_sati; R_Bruto13  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 14: R_Sati14 += ptrane_rec.T_sati; R_Bruto14  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 15: R_Sati15 += ptrane_rec.T_sati; R_Bruto15  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 16: R_Sati16 += ptrane_rec.T_sati; R_Bruto16  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 17: R_Sati17 += ptrane_rec.T_sati; R_Bruto17  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 18: R_Sati18 += ptrane_rec.T_sati; R_Bruto18  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 20: R_Sati20 += ptrane_rec.T_sati; R_Bruto20  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 30: R_Sati30 += ptrane_rec.T_sati; R_Bruto30E += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            case 40: R_Sati40 += ptrane_rec.T_sati; R_Bruto40  += ptrane_rec.R_ThisEvrBruto.Ron2(); break;
            default: break;
         }

      }
      R_Bruto30 = R_Bruto30E + R_Bruto30D;
      R_Bruto17 += T_NetoAdd;
    //R_Sum1do3 = R_SatiOnlyRadBruto + R_Bruto12 + R_Bruto13 + R_Bruto14 + R_Bruto15 + R_Bruto16 + R_Bruto17 + R_Bruto20 + R_Bruto30;
      R_Sum1do3 = cumulatedThe_Bruto + R_Bruto30D + T_NetoAdd;

      #endregion Obrazac IP1 ...
      
    //R_TheBruto = cumulatedThe_Bruto + T_topObrok + T_dodBruto + R_BrtDodNaStaz + T_brDodPoloz + R_thisStazDod                ; 
      R_TheBruto = cumulatedThe_Bruto + T_topObrok + T_dodBruto + R_BrtDodNaStaz + T_brDodPoloz + R_thisStazDod + T_brutoKorekc;
      R_TheBruto = R_TheBruto.Ron2();

      // 26.10.2015: !!! PAZI !!! na onaj return par linija gore u 'if(ptranesOfThisPtrans.Length == 0)' 

   }

   /// <summary>
   /// STARO!
   /// </summary>
   /// <param name="pR"></param>
   /// <param name="ptranesOfThisPtrans"></param>
   /// <param name="fondSati"></param>
   /// <param name="anyT_staz_Exists"></param>
   private void Calc_TheBrutoOLD(PrulesStruct pR, Ptrane[] ptranesOfThisPtrans, decimal fondSati, bool anyT_staz_Exists)
   {

   #region this.T_dnFondSati.NotZero()

      // 10.09.2014: 
      if(this.T_dnFondSati.NotZero())
      {
         fondSati *= (decimal)((decimal)this.T_dnFondSati / (decimal)Placa.SluzbeniDnevniFondSati);
      }

   #endregion this.T_dnFondSati.NotZero()

   #region ZXC.CURR_prjkt_rec.IsCheckStaz == true

      //R_Bruto100 = (T_brutoOsn + T_topObrok + T_dodBruto) * (1.00M + (T_godStaza * pR._stDodStaz / 100.00M));

      //14.04.2014. 
      // obr po starom ako je T_brutoDodSt =  0 : osnZaStaz = (T_brutoOsn + T_topObrok + T_dodBruto) a T_dodBruto je ukljucen u osnovicu 
      // obr new       ako je T_brutoDodSt <> 0 : osnZaStaz = (T_brutoOsn + T_topObrok)              a T_dodBruto se racuna u %         +
      //  T_brDodPoloz dodatak na bruto koji ne reagira na staz i slicno nego je jednostavan dodatak fiksni u punom iznosu

      // 19.09.2014: start      
      if(/*T_godStaza.NotZero()*/ ZXC.CURR_prjkt_rec.IsCheckStaz == true && anyT_staz_Exists == true)
      {
         Person person_rec = VvUserControl.PersonSifrar.SingleOrDefault(per => per.PersonCD == this.T_personCD);

         if(person_rec != null)
         {
            uint calculatedGodineStaza = person_rec.CalcGodineStaza(this.T_MMYYYY, false);

            if(calculatedGodineStaza != T_godStaza)
            {
               person_rec.IssueGodineStazaWarning(T_godStaza, calculatedGodineStaza);
            }

         } // if(person_rec != null)
      } // if(T_godStaza.NotZero())
      // 19.09.2014: end      

   #endregion ZXC.CURR_prjkt_rec.IsCheckStaz == true

   #region brutoDod_aci
      // 26.01.2015: (Kerempuh) 
      R_brutoDod2 = ZXC.VvGet_25_of_100(T_brutoOsn, T_brutoDodSt2).Ron2();
      R_brutoDod3 = ZXC.VvGet_25_of_100(T_brutoOsn, T_brutoDodSt3).Ron2();

      //28.01.2015. dodani novi 
      R_ukBrutoKoef = T_koefBruto1 + ZXC.VvGet_25_of_100(T_koefBruto1, T_brutoDodSt2).Ron2() + ZXC.VvGet_25_of_100(T_koefBruto1, T_brutoDodSt3).Ron2();

      //11.02.2014: 
      if(T_brutoDodSt.NotZero())
      {
       //decimal osnovicaZaBrutoDodatak = T_brutoOsn + T_topObrok;
      // 12.02.2014. ovako bi trebalo biti da izracun bude ok ali mi javlja greske kod stare place
         decimal dodStaz = (T_brutoOsn + T_topObrok) * (T_godStaza * pR._stDodStaz / 100.00M);
         decimal osnovicaZaBrutoDodatak = T_brutoOsn + T_topObrok + dodStaz;

         // 28.02.2014: !!! dok nisi dodao ron2, nije mogao refreshati record jer je u calcu neki put dolazilo npr. 1340.2350000 a u fajlu je 1340.24 !!!!!!!!
       //T_dodBruto = ZXC.VvGet_25_on_100(osnovicaZaBrutoDodatak, T_brutoDodSt);
         T_dodBruto = ZXC.VvGet_25_of_100(osnovicaZaBrutoDodatak, T_brutoDodSt).Ron2(); // !!! 15.10.2014: OVO JE VELIKI POTERNCIJALNI IZVOR PROBLEMA !!! 
      }

   #endregion brutoDod_aci


      //==== Calc TheBruto via EVR   START ============================================================ 

      decimal cumulatedThe_Bruto = 0.00M;
      decimal Bkratko_sati       = 0M; 
      bool    isBkratko          = false;
      bool    isGodOdm           = false;

      // ===   
      /* === */ decimal cijenaSata100 = ZXC.DivSafe(T_brutoOsn, (T_IsPoluSat ? fondSati / 2 : fondSati));
      // ===   

      R_SatiOnlyRad  = R_SatiR;//02.02.2015. sati cistog rada
      R_SatiNeRBruto = 0;      // bruto sati koje radnika nije odradio - godisnji, bolovanje ....

      foreach(Ptrane ptrane_rec in ptranesOfThisPtrans)
      {
   #region isBkratko
         // 23.09.2014: 
         isBkratko = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
                      ptrane_rec.T_cijPerc < 100M &&                              // Cijena manja od 100%
                      ptrane_rec.T_vrstaR_name.ToUpper().Contains("BOLOVANJE"));

         if(isBkratko) Bkratko_sati += ptrane_rec.T_sati;
   #endregion isBkratko

   #region isGodOdm

         isGodOdm = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND &&       // "99" 
                     ptrane_rec.T_vrstaR_name.ToUpper().Contains("GODIŠNJI"));
   #endregion isGodOdm


         // do 28.01.2015. a ovdje dodajemo obracun bolovanja na osnovu prosjeka tri zadnje place
         //if(ptrane_rec.T_rsOO != "99") ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc          ) / 100.00M * cijenaSata;
         //else /* prekovr., bolNaPosl */ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata;

         if(ptrane_rec.T_rsOO != "99") // znaci ne ubraja se u redovan rad
         {
            ptrane_rec.R_EvrCijena  = (ptrane_rec.T_cijPerc) / 100.00M * cijenaSata100;

            ptrane_rec.R_ThisEvrCijena = ptrane_rec.R_EvrCijena; //02.02.2015.
         
         }
         else if(isBkratko && T_pr3mjBruto.NotZero()) // obracun bolovanja na temelju prosjeka zadnje 3 place bruto
         {
            decimal cijSatPr3 = ZXC.DivSafe(T_pr3mjBruto, (T_IsPoluSat ? fondSati / 2 : fondSati)); //26.01.2015. cijena sata na osnovu prosjeka 3 mjesecne place
            ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3;

            ptrane_rec.R_ThisEvrCijena = cijSatPr3 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3; //02.02.2015.
         }
         else if(isGodOdm && T_pr3mjBruto.NotZero()) // obracun godisnjeg odmora na temelju prosjeka zadnje 3 place bruto
         {
            decimal cijSatPr3 = ZXC.DivSafe(T_pr3mjBruto, (T_IsPoluSat ? fondSati / 2 : fondSati)); //04.02.2015. cijena sata na osnovu prosjeka 3 mjesecne place
            ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3;

            ptrane_rec.R_ThisEvrCijena = cijSatPr3 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijSatPr3; //04.02.2015.
         }
         else /* prekovr., bolNaPosl */
         {
            ptrane_rec.R_EvrCijena = (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata100;

            ptrane_rec.R_ThisEvrCijena = cijenaSata100 + (ptrane_rec.T_cijPerc - 100.00M) / 100.00M * cijenaSata100; //02.02.2015.
         }

         ptrane_rec.R_EvrBruto  = ptrane_rec.R_EvrCijena * ptrane_rec.T_sati;
         cumulatedThe_Bruto += ptrane_rec.R_EvrBruto.Ron2();

         ptrane_rec.R_ThisEvrBruto = ptrane_rec.R_ThisEvrCijena * ptrane_rec.T_sati;


         //28.01.2015. neodradeni sati rada za novi JOPPD podatak 10.0.
         bool isDaniNerada = (ptrane_rec.T_rsOO == Ptrane.RSOO_NE_ZBRAJAJ_U_FOND           &&   // "99" 
                             (ptrane_rec.T_cijPerc < 100M || ptrane_rec.T_cijPerc == 100M) &&   // Cijena manja ili jednaka 100% - ovo nsam sigurna jel potrebno
                             (ptrane_rec.T_vrstaR_cd != Ptrane.VrstaR_cd_thisIs_ZPI)       &&   // nije ZPI
                             (ptrane_rec.T_vrstaR_name.ToUpper().Contains("RAD") == false) 
                             );
         if(isDaniNerada)
         {
            R_SatiNeR      += ptrane_rec.T_sati;
            R_SatiNeRBruto += ptrane_rec.R_ThisEvrBruto;
         }
         
       //if(ptrane_rec.T_rsOO == "99") R_SatiOnlyRad -= ptrane_rec.T_sati; //02.02.2015. sati cistog rada 01-99 03.02.2015. i prekovremeni se oduzimaju
         if(ptrane_rec.T_rsOO == "99" || ptrane_rec.T_rsOO == "98" && ptrane_rec.T_vrstaR_cd != Ptrane.VrstaR_cd_thisIs_ZPI) R_SatiOnlyRad -= ptrane_rec.T_sati; //03.02.2015. sati cistog rada 01-99-98 jer i 89 pribraja na sate

      }

   #region osnZaStaz

      // 24.09.2014: 
      R_thisStazDod = T_brutoOsn * (T_thisStazSt / 100.00M);

      decimal osnZaStaz;
      if(T_brutoDodSt.NotZero()) osnZaStaz = (T_brutoOsn + T_topObrok);              // HZTK 
      else                       osnZaStaz = (T_brutoOsn + T_topObrok + T_dodBruto); // Classic, kao uvijek do hztke 

      if(ZXC.CURR_prjkt_rec.IsObrStazaLast && ptranesOfThisPtrans.Length != 0) // ima nesto u EVR 
      {
         osnZaStaz = (cumulatedThe_Bruto + T_topObrok + T_dodBruto);
      }

      R_BrtDodNaStaz = osnZaStaz * (T_godStaza * pR._stDodStaz / 100.00M);

      // ako nije radio cio mjesec: start 
      decimal priznatiRsati = R_SatiUk;

      //if(ZXC.CURR_prjkt_rec.IsSkipStzOnBol) priznatiRsati -= Bkratko_sati; 06.01.2015.  to je ok kada nebi ostatak bio na teret HZZO-a
      if(ZXC.CURR_prjkt_rec.IsSkipStzOnBol)
      {
         priznatiRsati -= Bkratko_sati;
         priznatiRsati -= R_SatiB;
      }

      decimal ratio = ZXC.DivSafe(priznatiRsati, fondSati);
      // 23.092014: 
    //R_BrtDodNaStaz *= ratio;
      if(T_IsPoluSat && ZXC.CURR_prjkt_rec.IsFullStzOnPol)
      {
         // NE utjeci na R_BrtDodNaStaz ako je polusatnoRV i u Prjkt je zadano IsFullStzOnPol = true 
         //R_BrtDodNaStaz *= ratio;
      }
      else
      {
         R_BrtDodNaStaz *= ratio;
      }
      // ako nije radio cio mjesec: end 

   #endregion osnZaStaz

      decimal osnObrokDodaci = (T_brutoOsn + T_topObrok + T_dodBruto);

    // 14.02.2014. 
    //R_Bruto100 = osnObrokDodaci + R_BrtDodNaStaz;
      R_Bruto100 = osnObrokDodaci + R_BrtDodNaStaz + T_brDodPoloz;

      R_Bruto100 = R_Bruto100.Ron2();

      if(ptranesOfThisPtrans.Length == 0)
      {
       // 30.09.2014. dodali mi R_thisStazDod na TheBruto a ovaj se nigdje ne kumulira dakle ide kao cisti dodatak na kraju svega,
       //             ali kad nema ptrane-a onda ni ne dojde do doljnjeg TheBruta
       //R_TheBruto = R_Bruto100;
         R_TheBruto = R_Bruto100 + R_thisStazDod;

       //02.02.2015. sati cistog rada bez praznika - ovo je kada nema evr a se sati praznika automatski racunaju
         R_SatiOnlyRad     -= R_praznikHrs;
         R_SatiOnlyRadBruto = R_SatiOnlyRad * cijenaSata100; 
         R_PraznikHrsBruto  = R_praznikHrs  * cijenaSata100;
         R_SatiNeRBruto     = R_PraznikHrsBruto; //03.02.2015.

         return;
      }

      R_SatiOnlyRadBruto = R_SatiOnlyRad * cijenaSata100;

      R_TheBruto = cumulatedThe_Bruto + T_topObrok + T_dodBruto + R_BrtDodNaStaz + T_brDodPoloz + R_thisStazDod;
      R_TheBruto = R_TheBruto.Ron2();

      // 26.10.2015: !!! PAZI !!! na onaj return par linija gore 
   }

   private void Calc_OtherDohodakOrPenzOrNovozap_Overriders_Bef2017(ref PrulesStruct pR, string placaTT)
   {
      #region Ako ovdje nemas sta raditi: return;

      // Ovdje popisati sve moguce 'specijalnost', te ako nije nijedna od njih - return! 

      if(placaTT != Placa.TT_UGOVORODJELU &&
         placaTT != Placa.TT_NADZORODBOR  &&
         placaTT != Placa.TT_AUTORHONOR   &&
         placaTT != Placa.TT_AUTORHONUMJ  &&
         placaTT != Placa.TT_PODUZETPLACA &&
         placaTT != Placa.TT_IDD_KOLONA_4 &&
         placaTT != Placa.TT_IDD_KOLONA_4 &&
         placaTT != Placa.TT_SEZZAPPOLJOP &&
         placaTT != Placa.TT_POREZNADOBIT &&  // 07.03.2014.
         placaTT != Placa.TT_STRUCNOOSPOS &&  // 25.11.2014.
         placaTT != Placa.TT_NEPLACDOPUST &&  // 25.11.2014.
         placaTT != Placa.TT_TURSITVIJECE &&  // 12.03.2015.
         placaTT != Placa.TT_SAMODOPRINOS &&  // 09.02.2016.

         this.T_spc != SpecEnum.NOVOZAPOSL &&
         this.T_spc != SpecEnum.PENZ)
      { 
         return;
      }

      #endregion Ako ovdje nemas sta raditi: return;

      #region Za Copy Paste-anje

      // fuse, svi rulzi:

   ///* 17 */ pr.Rule_StPor1       = 0.00M;
   ///* 18 */ pr.Rule_StPor2       = 0.00M;
   ///* 19 */ pr.Rule_StPor3       = 0.00M;
   ///* 20 */ pr.Rule_StPor4       = 0.00M;
   ///* 21 */ pr.Rule_OsnOdb       = 0.00M;
   ///* 22 */ pr.Rule_StMio1stup   = 0.00M;
   ///* 23 */ pr.Rule_StMio2stup   = 0.00M;
   ///* 24 */ pr.Rule_StZdrNa      = 0.00M;
   ///* 25 */ pr.Rule_StZorNa      = 0.00M;
   ///* 26 */ pr.Rule_StZapNa      = 0.00M;
   ///* 27 */ pr.Rule_StZapII      = 0.00M;
   ///* 28 */ pr.Rule_MinMioOsn    = 0.00M;
   ///* 29 */ pr.Rule_MaxMioOsn    = 0.00M;
   ///* 30 */ pr.Rule_MaxPorOsn1   = 0.00M;
   ///* 31 */ pr.Rule_MaxPorOsn2   = 0.00M;
   ///* 32 */ pr.Rule_MaxPorOsn3   = 0.00M;
   ///* 33 */ pr.Rule_StZpi        = 0.00M;
   ///* 34 */ pr.Rule_StOthOlak    = 0.00M;
   ///* 35 */ pr.Rule_StDodStaz    = 0.00M;
   ///* 36 */ pr.Rule_GranBrRad    = 0.00M;
   ///* 37 */ pr.Rule_StMioNaB1    = 0.00M;
   ///* 38 */ pr.Rule_StMioNa2B1   = 0.00M;
   ///* 39 */ pr.Rule_StMioNaB2    = 0.00M;
   ///* 40 */ pr.Rule_StMioNa2B2   = 0.00M;
   ///* 41 */ pr.Rule_StMioNaB3    = 0.00M;
   ///* 42 */ pr.Rule_StMioNa2B3   = 0.00M;
   ///* 43 */ pr.Rule_StMioNaB4    = 0.00M;
   ///* 44 */ pr.Rule_StMioNa2B4   = 0.00M;
   ///* 45 */ pr.Rule_ProsPlaca    = 0.00M;
   ///* 46 */ pr.Rule_StMioNa2B5   = 0.00M;

   #endregion Za Copy Paste-anje

      #region OtherDohodak & PENZ Overriders

      if(placaTT == Placa.TT_UGOVORODJELU ||
         placaTT == Placa.TT_NADZORODBOR  ||
         placaTT == Placa.TT_TURSITVIJECE ||
         placaTT == Placa.TT_IDD_KOLONA_4 ||
         placaTT == Placa.TT_AUTORHONUMJ  ||
         placaTT == Placa.TT_SEZZAPPOLJOP ||
         placaTT == Placa.TT_POREZNADOBIT ||
         placaTT == Placa.TT_AUTORHONOR)
      {
   //// 07.03.2014. maknuto i dodano za porez na isplacenu dobit
   ///////* 17 */ pR._stpor1       = 0.00M;
   ///////* 18 */ //pr._StPor2     = 0.00M; Znaci ostaje onakva kakva je i inace 

        if(placaTT == Placa.TT_POREZNADOBIT)  // 07.03.2014.
        {
        /* 17 */  //pR._stpor1    = 0.00M; Znaci ostaje onakva kakva je i inace 
        /* 18 */  pR._stpor2      = 0.00M; 
        }
        else
        { 
        /* 17 */ pR._stpor1       = 0.00M;
        /* 18 */ //pr._StPor2     = 0.00M; Znaci ostaje onakva kakva je i inace 
        }


         /* 19 */ pR._stpor3       = 0.00M;
         /* 20 */ pR._stpor4       = 0.00M;

         /* 21 */ pR._osnOdb       = 0.00M;

         /* 25 */ pR._stZorNa      = 0.00M;
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;

         /* 37 */ pR._stMioNaB1    = 0.00M;
         /* 38 */ pR._stMioNa2B1   = 0.00M;
         /* 39 */ pR._stMioNaB2    = 0.00M;
         /* 40 */ pR._stMioNa2B2   = 0.00M;
         /* 41 */ pR._stMioNaB3    = 0.00M;
         /* 42 */ pR._stMioNa2B3   = 0.00M;
         /* 43 */ pR._stMioNaB4    = 0.00M;
         /* 44 */ pR._stMioNa2B4   = 0.00M;
         /* 45 */ pR._prosPlaca    = 0.00M;
         /* 46 */ pR._stMioNa2B5   = 0.00M;

         /* 33 */ // pr._StZpi        = 0.00M; // ostaje nepromjenjeno 
         /* 34 */ // pr._StOthOlak    = 0.00M; // ostaje nepromjenjeno 
         /* 35 */ // pr._StDodStaz    = 0.00M; // ostaje nepromjenjeno 
         /* 36 */ // pr._GranBrRad    = 0.00M; // ostaje nepromjenjeno 
         /* 28 */ // pr._MinMioOsn    = 0.00M; // ostaje nepromjenjeno 
         /* 29 */ // pr._MaxMioOsn    = 0.00M; // ostaje nepromjenjeno 

         if(placaTT == Placa.TT_AUTORHONOR   || placaTT == Placa.TT_AUTORHONUMJ || placaTT == Placa.TT_SEZZAPPOLJOP || placaTT == Placa.TT_POREZNADOBIT ||
            placaTT == Placa.TT_IDD_KOLONA_4 || this.T_spc == SpecEnum.PENZ ) // Autorski i PENZici NE placaju MIO i ZDR, 
         {                                                                   // dok UGOVOR i NADZORNI DA - placaju       
            /* 22 */ pR._stMio1stup   = 0.00M;
            /* 23 */ pR._stMio2stup   = 0.00M;
            /* 24 */ pR._stZdrNa      = 0.00M;
         }
         else // UGOVOR i NADZORNI 
         {
            /* 22 */ // pr._StMio1stup   = 0.00M; // ostaje nepromjenjeno 
            /* 23 */ // pr._StMio2stup   = 0.00M; // ostaje nepromjenjeno 
            /* 24 */ // pr._StZdrNa      = 0.00M; // ostaje nepromjenjeno 
         }

      }

      if(placaTT == Placa.TT_STRUCNOOSPOS)
      {
         pR._stpor1   = 0.00M;
         pR._stpor2   = 0.00M;
         pR._stZapNa  = 0.00M;
         pR._stZapII  = 0.00M;

      }
      if(placaTT == Placa.TT_NEPLACDOPUST)
      {
         pR._stpor1 = 0.00M;
         pR._stpor2 = 0.00M;
      }

      #endregion OtherDohodak & PENZ Overriders

      #region NOVOZAPOSLENI Overriders

      if(this.T_spc == SpecEnum.NOVOZAPOSL) // Novozaposleni NE placa doprinose NA placu 
      {
         /* 24 */ pR._stZdrNa      = 0.00M;
         /* 25 */ pR._stZorNa      = 0.00M;
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;
         /* 37 */ pR._stMioNaB1    = 0.00M;
         /* 38 */ pR._stMioNa2B1   = 0.00M;
         /* 39 */ pR._stMioNaB2    = 0.00M;
         /* 40 */ pR._stMioNa2B2   = 0.00M;
         /* 41 */ pR._stMioNaB3    = 0.00M;
         /* 42 */ pR._stMioNa2B3   = 0.00M;
         /* 43 */ pR._stMioNaB4    = 0.00M;
         /* 44 */ pR._stMioNa2B4   = 0.00M;
         /* 45 */ pR._prosPlaca    = 0.00M;
         /* 46 */ pR._stMioNa2B5   = 0.00M;
      }

      #endregion NOVOZAPOSLENI Overriders

      #region PODUZETNICKA PLACA Overriders

      // 04.02.2014. od place za 012014 poduzetnik placa doprinos za zaposljavanje ali jos ne znamo da li se djeli na dva ili ne
      if(placaTT == Placa.TT_PODUZETPLACA && this.T_MMYYYY_asDateTime < ZXC.Date01012014) // Poduzetnik NE placa doprinose za zaposljavanje
      {
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;
      }
      // 07.03.2014. sad kazu da se ne odvaja tj da ukupno ide na prvi doprinos
      if(placaTT == Placa.TT_PODUZETPLACA && this.T_MMYYYY_asDateTime >= ZXC.Date01012014) // Poduzetnik NE placa doprinose za zaposljavanje
      {
         /* 26 */ pR._stZapNa      = pR._stZapNa + pR._stZapII;
         /* 27 */ pR._stZapII      = 0.00M;
      }

      #endregion PODUZETNICKA PLACA Overriders

      #region TT_SAMODOPRINOS Overriders

      if(placaTT == Placa.TT_SAMODOPRINOS)
      {
         pR._stpor1       = 0.00M;
         pR._stpor2       = 0.00M;
         pR._stpor3       = 0.00M;
         pR._stpor4       = 0.00M;
         pR._osnOdb       = 0.00M;

         pR._stZapII      = 0.00M;

         pR._stMioNaB1    = 0.00M;
         pR._stMioNa2B1   = 0.00M;
         pR._stMioNaB2    = 0.00M;
         pR._stMioNa2B2   = 0.00M;
         pR._stMioNaB3    = 0.00M;
         pR._stMioNa2B3   = 0.00M;
         pR._stMioNaB4    = 0.00M;
         pR._stMioNa2B4   = 0.00M;
         pR._prosPlaca    = 0.00M;
         pR._stMioNa2B5   = 0.00M;

      }
      #endregion TT_SAMODOPRINOS Overriders

   }

   private void Calc_OtherDohodakOrPenzOrNovozap_Overriders(ref PrulesStruct pR, string placaTT)
   {
      #region Ako ovdje nemas sta raditi: return;

      // Ovdje popisati sve moguce 'specijalnost', te ako nije nijedna od njih - return! 

      if(placaTT != Placa.TT_UGOVORODJELU  &&
         placaTT != Placa.TT_BIVSIRADNIK   &&
         placaTT != Placa.TT_NADZORODBOR   &&
         placaTT != Placa.TT_AUTORHONOR    &&
         placaTT != Placa.TT_AUTORHONUMJ   &&
         placaTT != Placa.TT_AHSAMOSTUMJ   &&
         placaTT != Placa.TT_PODUZETPLACA  &&
         placaTT != Placa.TT_IDD_KOLONA_4  &&
         placaTT != Placa.TT_IDD_KOLONA_4  &&
         placaTT != Placa.TT_SEZZAPPOLJOP  &&
         placaTT != Placa.TT_POREZNADOBIT  &&  // 07.03.2014.
         placaTT != Placa.TT_STRUCNOOSPOS  &&  // 25.11.2014.
         placaTT != Placa.TT_NEPLACDOPUST  &&  // 25.11.2014.
         placaTT != Placa.TT_TURSITVIJECE  &&  // 12.03.2015.
         placaTT != Placa.TT_SAMODOPRINOS  &&  // 09.02.2016.
         placaTT != Placa.TT_DDBEZDOPRINO  &&  // 12.2018.
         placaTT != Placa.TT_AUVECASTOPA   &&  // 12.2018.
         placaTT != Placa.TT_NR1_PX1NEDOP  &&  // 12.2018.
         placaTT != Placa.TT_NR2_P01NEDOP  &&  // 12.2018.
         placaTT != Placa.TT_NR3_PX1DADOP  &&  // 12.2018.

         this.T_spc != SpecEnum.NOVOZAPOSL    &&
         this.T_spc != SpecEnum.NOVO_MINMIONE && //18.12.2019
         this.T_spc != SpecEnum.PENZ)
      {
         return;
      }

      #endregion Ako ovdje nemas sta raditi: return;

      #region Za Copy Paste-anje

      // fuse, svi rulzi:

      ///* 17 */ pr.Rule_StPor1       = 0.00M;
      ///* 18 */ pr.Rule_StPor2       = 0.00M;
      ///* 19 */ pr.Rule_StPor3       = 0.00M;
      ///* 20 */ pr.Rule_StPor4       = 0.00M;
      ///* 21 */ pr.Rule_OsnOdb       = 0.00M;
      ///* 22 */ pr.Rule_StMio1stup   = 0.00M;
      ///* 23 */ pr.Rule_StMio2stup   = 0.00M;
      ///* 24 */ pr.Rule_StZdrNa      = 0.00M;
      ///* 25 */ pr.Rule_StZorNa      = 0.00M;
      ///* 26 */ pr.Rule_StZapNa      = 0.00M;
      ///* 27 */ pr.Rule_StZapII      = 0.00M;
      ///* 28 */ pr.Rule_MinMioOsn    = 0.00M;
      ///* 29 */ pr.Rule_MaxMioOsn    = 0.00M;
      ///* 30 */ pr.Rule_MaxPorOsn1   = 0.00M;
      ///* 31 */ pr.Rule_MaxPorOsn2   = 0.00M;
      ///* 32 */ pr.Rule_MaxPorOsn3   = 0.00M;
      ///* 33 */ pr.Rule_StZpi        = 0.00M;
      ///* 34 */ pr.Rule_StOthOlak    = 0.00M;
      ///* 35 */ pr.Rule_StDodStaz    = 0.00M;
      ///* 36 */ pr.Rule_GranBrRad    = 0.00M;
      ///* 37 */ pr.Rule_StMioNaB1    = 0.00M;
      ///* 38 */ pr.Rule_StMioNa2B1   = 0.00M;
      ///* 39 */ pr.Rule_StMioNaB2    = 0.00M;
      ///* 40 */ pr.Rule_StMioNa2B2   = 0.00M;
      ///* 41 */ pr.Rule_StMioNaB3    = 0.00M;
      ///* 42 */ pr.Rule_StMioNa2B3   = 0.00M;
      ///* 43 */ pr.Rule_StMioNaB4    = 0.00M;
      ///* 44 */ pr.Rule_StMioNa2B4   = 0.00M;
      ///* 45 */ pr.Rule_ProsPlaca    = 0.00M;
      ///* 46 */ pr.Rule_StMioNa2B5   = 0.00M;

      #endregion Za Copy Paste-anje

      #region OtherDohodak & PENZ Overriders

      if(placaTT == Placa.TT_UGOVORODJELU ||
       //placaTT == Placa.TT_BIVSIRADNIK  ||
         placaTT == Placa.TT_NADZORODBOR  ||
         placaTT == Placa.TT_TURSITVIJECE ||
         placaTT == Placa.TT_IDD_KOLONA_4 ||
         placaTT == Placa.TT_AUTORHONUMJ  ||
         placaTT == Placa.TT_AHSAMOSTUMJ  ||
         placaTT == Placa.TT_SEZZAPPOLJOP ||
         placaTT == Placa.TT_POREZNADOBIT ||
         placaTT == Placa.TT_AUTORHONOR   ||
         placaTT == Placa.TT_DDBEZDOPRINO || // 12.2018.
         placaTT == Placa.TT_AUVECASTOPA  || // 12.2018.
         placaTT == Placa.TT_NR1_PX1NEDOP || // 12.2018. porez X bez pausala, bez doprinosa                          
         placaTT == Placa.TT_NR2_P01NEDOP || // 12.2018. porez 1 sa pausalom, bez doprinosa                          
         placaTT == Placa.TT_NR3_PX1DADOP    // 12.2018. porez X bez pausala, doprinosi na osn umanjenu za pausal 30%
         )
      {
        
         // Samo za TT_POREZNADOBIT treba korigirati 1. stopu, dok za ostale je ona dobra )_
         if(placaTT == Placa.TT_POREZNADOBIT) // za sada _stpor1 fiksna, NE iz rulsa 
         {
            pR._stpor1 = T_dokDate < ZXC.Date01012021 ? 12.00M : 10.00M;
         }
         if(placaTT == Placa.TT_AUVECASTOPA) //12.2018. racuna porez 1 kao stopu 2 
         {
            pR._stpor1 = pR._stpor2;
         }
         if(placaTT == Placa.TT_NR1_PX1NEDOP || placaTT == Placa.TT_NR3_PX1DADOP) // racuna porez 1 kao stopu 2 
         {
            pR._stpor1 = 10.00M; //12.2018. za sada je 10 iako jos moze biti i 5 
         }

         /* 19 */ pR._stpor2       = 0.00M;
         /* 19 */ pR._stpor3       = 0.00M;
         /* 20 */ pR._stpor4       = 0.00M;

         /* 21 */ pR._osnOdb       = 0.00M;

         /* 25 */ pR._stZorNa      = 0.00M;
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;

         /* 37 */ pR._stMioNaB1    = 0.00M;
         /* 38 */ pR._stMioNa2B1   = 0.00M;
         /* 39 */ pR._stMioNaB2    = 0.00M;
         /* 40 */ pR._stMioNa2B2   = 0.00M;
         /* 41 */ pR._stMioNaB3    = 0.00M;
         /* 42 */ pR._stMioNa2B3   = 0.00M;
         /* 43 */ pR._stMioNaB4    = 0.00M;
         /* 44 */ pR._stMioNa2B4   = 0.00M;
         /* 45 */ pR._prosPlaca    = 0.00M;
         /* 46 */ pR._stMioNa2B5   = 0.00M;

         /* 33 */ // pr._StZpi        = 0.00M; // ostaje nepromjenjeno 
         /* 34 */ // pr._StOthOlak    = 0.00M; // ostaje nepromjenjeno 
         /* 35 */ // pr._StDodStaz    = 0.00M; // ostaje nepromjenjeno 
         /* 36 */ // pr._GranBrRad    = 0.00M; // ostaje nepromjenjeno 
         /* 28 */ // pr._MinMioOsn    = 0.00M; // ostaje nepromjenjeno 
         /* 29 */ // pr._MaxMioOsn    = 0.00M; // ostaje nepromjenjeno 

       // ne placaju se doprinosi                     
        if(placaTT == Placa.TT_POREZNADOBIT || 
           placaTT == Placa.TT_AHSAMOSTUMJ  ||  //neki autori se izborili da ni oni ne placaju doprinose
           placaTT == Placa.TT_DDBEZDOPRINO ||  //12.2018. drugi dohodak bez obveze doprinosa
           placaTT == Placa.TT_NR1_PX1NEDOP ||  //12.2018. nerezidenti bez obveze doprinosa
           placaTT == Placa.TT_NR2_P01NEDOP     //12.2018. nerezidenti bez obveze doprinosa
           ) 
         {                                       
            /* 22 */ pR._stMio1stup = 0.00M;
            /* 23 */ pR._stMio2stup = 0.00M;
            /* 24 */ pR._stZdrNa    = 0.00M;
         }
         else // za ostale, stopa se prepolavlja 
         {
            /* 22 */ pR._stMio1stup /= 2.00M;
            /* 23 */ pR._stMio2stup /= 2.00M;

            //16.01.2019. za 2019 i dalje je pola od onoga koliko je bilo (15%) bez povecanja koji ide na placu 
            ////* 24 */ pR._stZdrNa    /= 2.00M;
            if(T_MMYYYY_asDateTime >= ZXC.Date01012019) pR._stZdrNa  = pR._stZdrDD; // 'od 2019'   
            else                                        pR._stZdrNa /=       2.00M; // 'po starom' 
         }
      }
      //if(placaTT == Placa.TT_UGOVORODJELU ||
      //   placaTT == Placa.TT_NADZORODBOR ||
      //   placaTT == Placa.TT_TURSITVIJECE ||
      //   placaTT == Placa.TT_IDD_KOLONA_4 ||
      //   placaTT == Placa.TT_AUTORHONUMJ ||
      //   placaTT == Placa.TT_SEZZAPPOLJOP ||
      //   placaTT == Placa.TT_POREZNADOBIT ||
      //   placaTT == Placa.TT_AUTORHONOR)

      if(placaTT == Placa.TT_STRUCNOOSPOS)
      {
         pR._stpor1   = 0.00M;
         pR._stpor2   = 0.00M;
         pR._stZapNa  = 0.00M;
         pR._stZapII  = 0.00M;

      }
      if(placaTT == Placa.TT_NEPLACDOPUST)
      {
         pR._stpor1 = 0.00M;
         pR._stpor2 = 0.00M;
      }

      #endregion OtherDohodak & PENZ Overriders

      #region NOVOZAPOSLENI Overriders

    //if(this.T_spc == SpecEnum.NOVOZAPOSL                                        ) // Novozaposleni NE placa doprinose NA placu 
      if(this.T_spc == SpecEnum.NOVOZAPOSL || this.T_spc == SpecEnum.NOVO_MINMIONE) // Novozaposleni NE placa doprinose NA placu18.12.2019. 
      {
         /* 24 */ pR._stZdrNa      = 0.00M;
         /* 25 */ pR._stZorNa      = 0.00M;
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;
         /* 37 */ pR._stMioNaB1    = 0.00M;
         /* 38 */ pR._stMioNa2B1   = 0.00M;
         /* 39 */ pR._stMioNaB2    = 0.00M;
         /* 40 */ pR._stMioNa2B2   = 0.00M;
         /* 41 */ pR._stMioNaB3    = 0.00M;
         /* 42 */ pR._stMioNa2B3   = 0.00M;
         /* 43 */ pR._stMioNaB4    = 0.00M;
         /* 44 */ pR._stMioNa2B4   = 0.00M;
         /* 45 */ pR._prosPlaca    = 0.00M;
         /* 46 */ pR._stMioNa2B5   = 0.00M;
      }

      #endregion NOVOZAPOSLENI Overriders

      #region PODUZETNICKA PLACA Overriders

      // 04.02.2014. od place za 012014 poduzetnik placa doprinos za zaposljavanje ali jos ne znamo da li se djeli na dva ili ne
      if(placaTT == Placa.TT_PODUZETPLACA && this.T_MMYYYY_asDateTime < ZXC.Date01012014) // Poduzetnik NE placa doprinose za zaposljavanje
      {
         /* 26 */ pR._stZapNa      = 0.00M;
         /* 27 */ pR._stZapII      = 0.00M;
      }
      // 07.03.2014. sad kazu da se ne odvaja tj da ukupno ide na prvi doprinos
      if(placaTT == Placa.TT_PODUZETPLACA && this.T_MMYYYY_asDateTime >= ZXC.Date01012014) // Poduzetnik NE placa doprinose za zaposljavanje
      {
         /* 26 */ pR._stZapNa      = pR._stZapNa + pR._stZapII;
         /* 27 */ pR._stZapII      = 0.00M;
      }

      #endregion PODUZETNICKA PLACA Overriders

      #region TT_SAMODOPRINOS Overriders

      if(placaTT == Placa.TT_SAMODOPRINOS)
      {
         pR._stpor1       = 0.00M;
         pR._stpor2       = 0.00M;
         pR._stpor3       = 0.00M;
         pR._stpor4       = 0.00M;
         pR._osnOdb       = 0.00M;

         pR._stZapII      = 0.00M;

         pR._stMioNaB1    = 0.00M;
         pR._stMioNa2B1   = 0.00M;
         pR._stMioNaB2    = 0.00M;
         pR._stMioNa2B2   = 0.00M;
         pR._stMioNaB3    = 0.00M;
         pR._stMioNa2B3   = 0.00M;
         pR._stMioNaB4    = 0.00M;
         pR._stMioNa2B4   = 0.00M;
         pR._prosPlaca    = 0.00M;
         pR._stMioNa2B5   = 0.00M;

      }
      #endregion TT_SAMODOPRINOS Overriders

      #region TT_BIVSIRADNIK Overriders
      //23.12.2019.Obračun primitaka prema kojima se doprinosi obračunavaju na način koji ima obilježje drugog dohotka, a porez na dohodak prema primitcima od kojih se utvrđuje dohodak od nesamostalnog rada
      if(placaTT == Placa.TT_BIVSIRADNIK)
      {
        /* 22 */ pR._stMio1stup /= 2.00M;
        /* 23 */ pR._stMio2stup /= 2.00M;

        //16.01.2019. za 2019 i dalje je pola od onoga koliko je bilo (15%) bez povecanja koji ide na placu 
        ////* 24 */ pR._stZdrNa    /= 2.00M;
        if(T_MMYYYY_asDateTime >= ZXC.Date01012019) pR._stZdrNa  = pR._stZdrDD; // 'od 2019'   
        else                                        pR._stZdrNa /=       2.00M; // 'po starom' 


      }
      #endregion TT_BIVSIRADNIK Overriders

   }

   private void Calc_Doprinosi_Bef2017(PrulesStruct pR, AlreadySpentPtransInThisMonthStruct spent, string placaTT)
   {

      #region MIOiz & MIOna

      decimal maxMioOsnova = pR._maxMioOsn - spent.MioOsn;
      decimal osnovicaDop; // trbala bi biti jednaka 'R_MioOsn' ali NE smije trzati na 'maxMioOsnova' tj. NEMA gornje granice 

      if(placaTT == Placa.TT_UGOVORODJELU ||
         placaTT == Placa.TT_NADZORODBOR  ||
         placaTT == Placa.TT_TURSITVIJECE   )
      {
         if(T_spc == SpecEnum.PENZ) osnovicaDop = R_MioOsn = 0.00M;
         else                       osnovicaDop = R_MioOsn = R_TheBruto; 
      }
      else if(placaTT == Placa.TT_AUTORHONOR || placaTT == Placa.TT_AUTORHONUMJ || placaTT == Placa.TT_IDD_KOLONA_4 || placaTT == Placa.TT_SEZZAPPOLJOP || placaTT == Placa.TT_POREZNADOBIT)
      {
         osnovicaDop = R_MioOsn = 0.00M;
      }
      else if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST)
      {
         osnovicaDop = R_MioOsn = pR._minMioOsn;
      }
      else
      {//qweqwe
         R_MioOsn    = R_TheBruto > maxMioOsnova ? maxMioOsnova : R_TheBruto;
         osnovicaDop =                                            R_TheBruto;
      }

      #region staro
      // od 4.9.2006: 
      // IPAK NE   R_MioOsn = R_TheBruto > pRules._MaxMioOsn ? pRules._MaxMioOsn : R_TheBruto < pRules._MinMioOsn ? pRules._MinMioOsn : R_TheBruto;

      // IPAK DA 21.03.2013. a treba i tu mioOsn postaviti i za druge doprinose
      // ako je Bruto>MaxMioOsn onda daj MaxMioOsn else ako je Bruto<MinMioOsn ona daj MinMioOsn a else daj Bruto
      //R_MioOsn = R_TheBruto > pR._maxMioOsn ? pR._maxMioOsn : R_TheBruto < pR._minMioOsn ? pR._minMioOsn : R_TheBruto;

      //27.03.2012. ali treba paziti kada je TheBruto = 0 da onda ne racuna doprinose

      ////28.03.2013: start 
      //bool isNormalniFondSati = false;
      //if((placaTT == Placa.TT_REDOVANRAD || placaTT == Placa.TT_PODUZETPLACA) && // mora biti ili 'RR' ili 'PP'
      //    R_FondSatiDiffABS.IsZero() && R_SatiB.IsZero())                        // NIJE polusatno RV i NIJE poceo raditi na pola mjeseca i NIJE pola mj. na dugom bolovanju na teret HZZOa 
      //{
      //   isNormalniFondSati = true;
      //}
      //bool isSiromasnaFirma = isNormalniFondSati && R_TheBruto < pR._minMioOsn && R_TheBruto.NotZero(); // Boba otkrila bug 

      //if(isSiromasnaFirma) R_MioOsn = pR._minMioOsn; // ovo je kada je normalno satno rv i kada nema nikakvog dugog bolovanja - ybrckano je tu cijeli obracun jer se to opet ponavlja dolje ....
      
      ////28.03.2013: end 

      //// 11.09.2014: start 
      //bool isNepunoRadnoVrijeme = T_dnFondSati.NotZero() && T_dnFondSati != Placa.SluzbeniDnevniFondSati; // nepuno je kad nije 8 satno 

      //if(isNepunoRadnoVrijeme) // tu pobijamo pravilo min mioOsnovice (isSiromasnaFirma)
      //{
      //   R_MioOsn = R_TheBruto;
      //}
      //// 11.09.2014: end 
      #endregion staro

      #region  novo 26.11.2014.

      // uvijek treba uzimati razmjerni dio minMioOsn u odnosu na to koliko je radnik sati RADIO - stvarni sati rada koji se ubrajju u R_SatiR
      if(placaTT == Placa.TT_REDOVANRAD || placaTT == Placa.TT_PODUZETPLACA || placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_STRUCNOOSPOS)
      {
         decimal minMioOsnZaPuniFond;
         decimal satiRadaZaRazmjerniDio;
         decimal satiRadaBezPrekovrIznadFonda;
         decimal razmjerniDioMinMioOsn;
         bool hasPrekovremeneIznadFonda;
      
         hasPrekovremeneIznadFonda    = R_FondSatiDiffABS.IsPositive(); // da li ima prekovremene preko fonda da ih maknemo jer cisti fond ulayi u obracun

         satiRadaBezPrekovrIznadFonda = hasPrekovremeneIznadFonda ? R_SatiUk - R_FondSatiDiffABS : R_SatiR;

         minMioOsnZaPuniFond          = (T_dnFondSati.NotZero() ? ((pR._minMioOsn / ((decimal)Placa.SluzbeniDnevniFondSati)) * T_dnFondSati) : pR._minMioOsn); // iznos minMioOsn ovisno o tome na koliko je sati radnik prijavljen

         satiRadaZaRazmjerniDio       = R_SatiUk.NotZero() ? (R_FondSatiDiffABS.NotZero() ? R_SatiUk - R_FondSatiDiffABS : R_SatiUk) : 0.00M; // ukupni fond sati radnika umanjen za prekovremene

         razmjerniDioMinMioOsn        = ZXC.DivSafe(minMioOsnZaPuniFond, satiRadaZaRazmjerniDio) * satiRadaBezPrekovrIznadFonda; // razmjerni dio MinMioOsn na osnovu koliko je sati radnik radio ali 

         if(placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_STRUCNOOSPOS)
         {
            osnovicaDop = R_MioOsn = razmjerniDioMinMioOsn;
         }
         else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.MINMIONE) // 29.12.2014. kada na bolovanje hoce dati a da ne uzima minMioOsn//MINMIONE - ne uzima minMioOsn vec stvarin bruto koji je u biti manji od MinMioOSn
         {
            osnovicaDop = R_MioOsn = R_TheBruto;
         }
         else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.MAXMIONE) // 13.03.2015. ne uzimaj u obzir maxMioOsn kod ovog obracuna - npr. za otpremninu i sl.
         {
            osnovicaDop = R_MioOsn = R_TheBruto;
         }
         else
         { //qweqwe
            if(R_TheBruto < razmjerniDioMinMioOsn) osnovicaDop = R_MioOsn = razmjerniDioMinMioOsn;
          //else                                   R_MioOsn    = R_TheBruto; 12.03.2015. jer kod velikih placa peko maxMioOsn nije dobro radilo
            else                                 { R_MioOsn    = R_TheBruto > maxMioOsnova ? maxMioOsnova : R_TheBruto;
                                                   osnovicaDop =                                            R_TheBruto; }
         }
      }

      #endregion novo 26.11.2014.

      // 21.12.2013: 
      if(placaTT == Placa.TT_PLACAUNARAVI) osnovicaDop = R_MioOsn = R_TheBruto;

      if(T_isMioII == true) // covjek JE u II MIO stupu 
      {
         R_Mio1stup = R_MioOsn * pR._stMio1stup / 100.00M;
         R_Mio2stup = R_MioOsn * pR._stMio2stup / 100.00M;
      }
      else // covjek NIJE u II MIO stupu 
      {
         R_Mio1stup = R_MioOsn * (pR._stMio1stup + pR._stMio2stup) / 100.00M;
         R_Mio2stup = 0.00M;
      }
      R_Mio1stup = R_Mio1stup.Ron2()      ;
      R_Mio2stup = R_Mio2stup.Ron2()      ;
      R_MioAll   = R_Mio1stup + R_Mio2stup;


      #region T_rsB.NotZero - radnik ima po nekoj osnovi beneficirani radni staz
      if(T_rsB.NotZero()) // radnik ima po nekoj osnovi beneficirani radni staz 
      {
         decimal stMioNa1, stMioNa2;

         switch(T_rsB)
         {
            case 1: stMioNa1 = pR._stMioNaB1; stMioNa2 = pR._stMioNa2B1; break;
            case 2: stMioNa1 = pR._stMioNaB2; stMioNa2 = pR._stMioNa2B2; break;
            case 3: stMioNa1 = pR._stMioNaB3; stMioNa2 = pR._stMioNa2B3; break;
            case 4: stMioNa1 = pR._stMioNaB4; stMioNa2 = pR._stMioNa2B4; break;
            case 5: stMioNa1 = pR._prosPlaca; stMioNa2 = pR._stMioNa2B5; break;

            default: ZXC.aim_emsg("T_rsB nije od 1 do 5!"); stMioNa1 = stMioNa2 = 0; break;
         }

         if(T_isMioII == true) // covjek JE u II MIO stupu 
         {
            R_Mio1stupNa = R_MioOsn * stMioNa1 / 100.00M;
            R_Mio2stupNa = R_MioOsn * stMioNa2 / 100.00M;
         }
         else // covjek NIJE u II MIO stupu 
         {
            R_Mio1stupNa = R_MioOsn * (stMioNa1 + stMioNa2) / 100.00M;
            R_Mio2stupNa = 0.00M;
         }
         R_Mio1stupNa = R_Mio1stupNa.Ron2();
         R_Mio2stupNa = R_Mio2stupNa.Ron2();
         R_MioAllNa   = R_Mio1stupNa + R_Mio2stupNa;
      }
      #endregion T_rsB.NotZero - radnik ima po nekoj osnovi beneficirani radni staz

      #endregion MIOiz & MIOna

      // 22.03.2013. ako je TheBruto manji od MinMioOsn onda se doprinosi obracunavaju na MinMioOsn
      //decimal osnovicaDop = R_TheBruto < pR._minMioOsn ? pR._minMioOsn : R_TheBruto;
      //27.03.2013. ali treba paziti kada je TheBruto=0 da nis ne racuna
    //decimal osnovicaDop = (isSiromasnaFirma == true)                                  ? pR._minMioOsn : R_TheBruto;
      
      // 26.11.2014. mislim da to ne treba
    //decimal osnovicaDop = (isSiromasnaFirma == true && isNepunoRadnoVrijeme == false) ? pR._minMioOsn : R_TheBruto;
      // 12.05.2015: 
    //decimal osnovicaDop = R_MioOsn;
      if(T_dokDate <= ZXC.Date12052015) osnovicaDop = R_MioOsn; // do 12.5.2015 je bio bug da su i Dop trzali na maxMioOsn a nisu smjeli            
                                                                // od 'danas' smo popravili te razdvojili maxMioOsn od utjecaja na ostale doprinose 
      #region ZDR & ZOR

    //R_ZdrNa = R_TheBruto * pR._stZdrNa / 100.00M;
    //22.03.2013.                                  
      R_ZdrNa = osnovicaDop   * pR._stZdrNa / 100.00M;
      
      R_ZdrNa = R_ZdrNa.Ron2();

    //R_ZorNa = R_TheBruto * pR._stZorNa / 100.00M;
    //22.03.2013.
      R_ZorNa = osnovicaDop * pR._stZorNa / 100.00M;
      R_ZorNa = R_ZorNa.Ron2();

      #endregion ZDR & ZOR

      #region ZAPna ZAP2

    //R_ZapNa  = R_TheBruto * pR._stZapNa / 100.00M;
    //R_ZapII  = R_TheBruto * pR._stZapII / 100.00M;
    //22.03.2013.
      R_ZapNa = osnovicaDop * pR._stZapNa / 100.00M;
      R_ZapII = osnovicaDop * pR._stZapII / 100.00M;


      R_ZapNa = R_ZapNa.Ron2();
      R_ZapII = R_ZapII.Ron2();

      R_ZapAll = R_ZapNa + R_ZapII; // !? treba li ovaj zapII ovdje? 

      #endregion ZAPna ZAP2

      #region DoprIZ, DoprNA, DoprALL

      R_DoprIz = R_Mio1stup   + R_Mio2stup;
      R_DoprNa = R_Mio1stupNa + R_Mio2stupNa + R_ZdrNa + R_ZorNa + R_ZapNa + R_ZapII + R_ZpiUk ;

      R_DoprAll = R_DoprIz + R_DoprNa;

      #endregion DoprIZ, DoprNA, DoprALL

      R_osnovicaDop = osnovicaDop.Ron2();  // 13.05.2015. da se moze prikazati u joppd obrascu

   }

   private void Calc_Doprinosi(PrulesStruct pR, AlreadySpentPtransInThisMonthStruct spent, string placaTT)
   {

      #region MIOiz & MIOna

      decimal maxMioOsnova = pR._maxMioOsn - spent.MioOsn;
      decimal osnovicaDop; // trbala bi biti jednaka 'R_MioOsn' ali NE smije trzati na 'maxMioOsnova' tj. NEMA gornje granice 
      
      //24.01.2017.
      bool isSO_bef2017 = placaTT == Placa.TT_STRUCNOOSPOS && this.T_dokDate < ZXC.Date01012017;

      if(placaTT == Placa.TT_UGOVORODJELU ||
         placaTT == Placa.TT_NADZORODBOR  ||
         placaTT == Placa.TT_TURSITVIJECE ||  
         placaTT == Placa.TT_IDD_KOLONA_4 ||
         placaTT == Placa.TT_BIVSIRADNIK  || //23.12.2019.
         placaTT == Placa.TT_SEZZAPPOLJOP
         )
      {
         osnovicaDop = R_MioOsn = R_TheBruto; 
      }
      else if(placaTT == Placa.TT_AUTORHONOR || placaTT == Placa.TT_NR3_PX1DADOP/*12.2018*/)
      {
         R_AHizdatak = (R_TheBruto * pR._stOthOlak) / 100.00M;
         R_AHizdatak = R_AHizdatak.Ron2();

         osnovicaDop = R_MioOsn = R_TheBruto - R_AHizdatak;
      }
      else if(placaTT == Placa.TT_AUTORHONUMJ || placaTT == Placa.TT_AUVECASTOPA/*12.2018*/)
      {
         R_AHizdatak = (R_TheBruto * (pR._stOthOlak + 25.00M)) / 100.00M;
         R_AHizdatak = R_AHizdatak.Ron2();

         osnovicaDop = R_MioOsn = R_TheBruto - R_AHizdatak;
      }
      else if(placaTT == Placa.TT_AHSAMOSTUMJ)
      {
         R_AHizdatak = (R_TheBruto * (pR._stOthOlak + 25.00M)) / 100.00M;
         R_AHizdatak = R_AHizdatak.Ron2();

         osnovicaDop = R_MioOsn = 0.00M;
      }
      else if(placaTT == Placa.TT_POREZNADOBIT || 
              placaTT == Placa.TT_DDBEZDOPRINO || //12.2018.
              placaTT == Placa.TT_NR1_PX1NEDOP || //12.2018.
              placaTT == Placa.TT_NR2_P01NEDOP    //12.2018.
              )
      {
         osnovicaDop = R_MioOsn = 0.00M;
      }
    //else if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST)  24.01.2017.
      else if(isSO_bef2017                     || placaTT == Placa.TT_NEPLACDOPUST)
      {
         osnovicaDop = R_MioOsn = pR._minMioOsn;
      }
      else
      {//qweqwe
         R_MioOsn    = R_TheBruto > maxMioOsnova ? maxMioOsnova : R_TheBruto;
         osnovicaDop =                                            R_TheBruto;
      }

      #region  novo 26.11.2014.

      // uvijek treba uzimati razmjerni dio minMioOsn u odnosu na to koliko je radnik sati RADIO - stvarni sati rada koji se ubrajju u R_SatiR
    //if(placaTT == Placa.TT_REDOVANRAD || placaTT == Placa.TT_PODUZETPLACA || placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_STRUCNOOSPOS)  24.01.2017.
      if(placaTT == Placa.TT_REDOVANRAD || placaTT == Placa.TT_PODUZETPLACA || placaTT == Placa.TT_NEPLACDOPUST || isSO_bef2017                    )
      {
         decimal minMioOsnZaPuniFond;
         decimal satiRadaZaRazmjerniDio;
         decimal satiRadaBezPrekovrIznadFonda;
         decimal razmjerniDioMinMioOsn;
         bool hasPrekovremeneIznadFonda;
         // 30.01.2017.
         decimal minDopOsnZaPuniFondClUp;
         decimal razmjerniDioMinMioOsnZaClUpr; 

      
         hasPrekovremeneIznadFonda    = R_FondSatiDiffABS.IsPositive(); // da li ima prekovremene preko fonda da ih maknemo jer cisti fond ulayi u obracun

         satiRadaBezPrekovrIznadFonda = hasPrekovremeneIznadFonda ? R_SatiUk - R_FondSatiDiffABS : R_SatiR;

         minMioOsnZaPuniFond          = (T_dnFondSati.NotZero() ? ((pR._minMioOsn / ((decimal)Placa.SluzbeniDnevniFondSati)) * T_dnFondSati) : pR._minMioOsn); // iznos minMioOsn ovisno o tome na koliko je sati radnik prijavljen

         satiRadaZaRazmjerniDio       = R_SatiUk.NotZero() ? (R_FondSatiDiffABS.NotZero() ? R_SatiUk - R_FondSatiDiffABS : R_SatiUk) : 0.00M; // ukupni fond sati radnika umanjen za prekovremene

         razmjerniDioMinMioOsn        = ZXC.DivSafe(minMioOsnZaPuniFond, satiRadaZaRazmjerniDio) * satiRadaBezPrekovrIznadFonda; // razmjerni dio MinMioOsn na osnovu koliko je sati radnik radio ali 

         minDopOsnZaPuniFondClUp      = (T_dnFondSati.NotZero() ? ((pR._stMioNa2B5 / ((decimal)Placa.SluzbeniDnevniFondSati)) * T_dnFondSati) : pR._stMioNa2B5); // iznos minDopOsn ovisno o tome na koliko je sati CLAN UPRAVE prijavljen

         razmjerniDioMinMioOsnZaClUpr = ZXC.DivSafe(minDopOsnZaPuniFondClUp, satiRadaZaRazmjerniDio) * satiRadaBezPrekovrIznadFonda; // razmjerni dio osnovice za doprinose na osnovu koliko je sati CLAN UPRAVE radio


         if(placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_STRUCNOOSPOS)
         {
            osnovicaDop = R_MioOsn = razmjerniDioMinMioOsn;
         }
       //else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.MINMIONE     ) // 29.12.2014. kada na bolovanje hoce dati a da ne uzima minMioOsn//MINMIONE - ne uzima minMioOsn vec stvarin bruto koji je u biti manji od MinMioOSn
         else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.MINMIONE ||
                 placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.NOVO_MINMIONE) // 18.12.2019.i za novozaposlene da ne uzima minMioOsn//MINMIONE - ne uzima minMioOsn vec stvarin bruto koji je u biti manji od MinMioOSn
         {
            osnovicaDop = R_MioOsn = R_TheBruto;
         }
         else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.MAXMIONE) // 13.03.2015. ne uzimaj u obzir maxMioOsn kod ovog obracuna - npr. za otpremninu i sl.
         {
            osnovicaDop = R_MioOsn = R_TheBruto;
         }
         else if(placaTT == Placa.TT_REDOVANRAD && T_spc == SpecEnum.CLANUPRAVE) // 30.01.2017. ako je clan  uprave i bruto mu je manji ili jednak propisanoj osnovici za doprinose takvih onda mu uzimaj to za osnovicu ali trebalo bi izci u omjeru sati
         {
            osnovicaDop = R_MioOsn = razmjerniDioMinMioOsnZaClUpr;
         }
         else
         { //qweqwe
            if(R_TheBruto < razmjerniDioMinMioOsn) osnovicaDop = R_MioOsn = razmjerniDioMinMioOsn;
          //else                                   R_MioOsn    = R_TheBruto; 12.03.2015. jer kod velikih placa peko maxMioOsn nije dobro radilo
            else                                 { R_MioOsn    = R_TheBruto > maxMioOsnova ? maxMioOsnova : R_TheBruto;
                                                   osnovicaDop =                                            R_TheBruto; }
         }
      }

      #endregion novo 26.11.2014.

      // 21.12.2013: 
      if(placaTT == Placa.TT_PLACAUNARAVI) osnovicaDop = R_MioOsn = R_TheBruto;

      if(T_isMioII == true) // covjek JE u II MIO stupu 
      {
         R_Mio1stup = R_MioOsn * pR._stMio1stup / 100.00M;
         R_Mio2stup = R_MioOsn * pR._stMio2stup / 100.00M;
      }
      else // covjek NIJE u II MIO stupu 
      {
         R_Mio1stup = R_MioOsn * (pR._stMio1stup + pR._stMio2stup) / 100.00M;
         R_Mio2stup = 0.00M;
      }
      R_Mio1stup = R_Mio1stup.Ron2()      ;
      R_Mio2stup = R_Mio2stup.Ron2()      ;
      R_MioAll   = R_Mio1stup + R_Mio2stup;


      #region T_rsB.NotZero - radnik ima po nekoj osnovi beneficirani radni staz
      if(T_rsB.NotZero()) // radnik ima po nekoj osnovi beneficirani radni staz 
      {
         decimal stMioNa1, stMioNa2;

         switch(T_rsB)
         {
            case 1: stMioNa1 = pR._stMioNaB1; stMioNa2 = pR._stMioNa2B1; break;
            case 2: stMioNa1 = pR._stMioNaB2; stMioNa2 = pR._stMioNa2B2; break;
            case 3: stMioNa1 = pR._stMioNaB3; stMioNa2 = pR._stMioNa2B3; break;
            case 4: stMioNa1 = pR._stMioNaB4; stMioNa2 = pR._stMioNa2B4; break;
            case 5: stMioNa1 = pR._prosPlaca; stMioNa2 = pR._stMioNa2B5; break;

            default: ZXC.aim_emsg("T_rsB nije od 1 do 5!"); stMioNa1 = stMioNa2 = 0; break;
         }

         if(T_isMioII == true) // covjek JE u II MIO stupu 
         {
            R_Mio1stupNa = R_MioOsn * stMioNa1 / 100.00M;
            R_Mio2stupNa = R_MioOsn * stMioNa2 / 100.00M;
         }
         else // covjek NIJE u II MIO stupu 
         {
            R_Mio1stupNa = R_MioOsn * (stMioNa1 + stMioNa2) / 100.00M;
            R_Mio2stupNa = 0.00M;
         }
         R_Mio1stupNa = R_Mio1stupNa.Ron2();
         R_Mio2stupNa = R_Mio2stupNa.Ron2();
         R_MioAllNa   = R_Mio1stupNa + R_Mio2stupNa;
      }
      #endregion T_rsB.NotZero - radnik ima po nekoj osnovi beneficirani radni staz

      #endregion MIOiz & MIOna

      if(T_dokDate <= ZXC.Date12052015) osnovicaDop = R_MioOsn; // do 12.5.2015 je bio bug da su i Dop trzali na maxMioOsn a nisu smjeli            
                                                                // od 'danas' smo popravili te razdvojili maxMioOsn od utjecaja na ostale doprinose 
      #region ZDR & ZOR

    //R_ZdrNa = R_TheBruto * pR._stZdrNa / 100.00M;
    //22.03.2013.                                  
      R_ZdrNa = osnovicaDop   * pR._stZdrNa / 100.00M;
      
      R_ZdrNa = R_ZdrNa.Ron2();

    //R_ZorNa = R_TheBruto * pR._stZorNa / 100.00M;
    //22.03.2013.
      R_ZorNa = osnovicaDop * pR._stZorNa / 100.00M;
      R_ZorNa = R_ZorNa.Ron2();

      #endregion ZDR & ZOR

      #region ZAPna ZAP2

    //R_ZapNa  = R_TheBruto * pR._stZapNa / 100.00M;
    //R_ZapII  = R_TheBruto * pR._stZapII / 100.00M;
    //22.03.2013.
      R_ZapNa = osnovicaDop * pR._stZapNa / 100.00M;
      R_ZapII = osnovicaDop * pR._stZapII / 100.00M;


      R_ZapNa = R_ZapNa.Ron2();
      R_ZapII = R_ZapII.Ron2();

      R_ZapAll = R_ZapNa + R_ZapII; // !? treba li ovaj zapII ovdje? 

      #endregion ZAPna ZAP2

      #region DoprIZ, DoprNA, DoprALL

      R_DoprIz = R_Mio1stup   + R_Mio2stup;
      R_DoprNa = R_Mio1stupNa + R_Mio2stupNa + R_ZdrNa + R_ZorNa + R_ZapNa + R_ZapII + R_ZpiUk ;

      R_DoprAll = R_DoprIz + R_DoprNa;

      #endregion DoprIZ, DoprNA, DoprALL

      R_osnovicaDop = osnovicaDop.Ron2();  // 13.05.2015. da se moze prikazati u joppd obrascu

   }

   private void Calc_DohodakOdbitakPorOsn(PrulesStruct pR, AlreadySpentPtransInThisMonthStruct spent, string placaTT)
   {
      // 22.04.2016. koristimo t_zivotno za redni broj u iyvornom Joppd obrascu te moramo prilikom obra;una neta nulirati zivotno da s ne bi zbrajalo u olaksice 
      // ukidanje poreznih olakšica za životno, dobrovoljno mirovinsko i dobrovoljno zdravstveno osiguranje nakon 1. srpnja 2010

      R_Premije = T_zivotno + T_dopZdr + T_dobMIO;

      if(R_Premije > (0.70M * pR._osnOdb)) R_Premije = 0.70M * pR._osnOdb;
      R_Premije = R_Premije.Ron2();

      if(T_dokDate > ZXC.Date01072010) R_Premije = 0.0M; // 22.04.2016.

      R_Odbitak = T_koef * pR._osnOdb;

      #region R_Odbitak in 2017 News and news 2020!!!!

      // 2017 NEWS: R_Odbitak = alfa + beta.                     
      // osnOdb2017 = 2.500                                      
      // alfa = osnOdb2017 x 1.5 ... zaokruzen na 100 kn = 3.800 
      // beta = osnOdb2017 x koefOvisanOdDjece                   

      // T_koef se koristi i kao informacija:
      // radnik sa 1 djetetom (klasika)          ...    ide mu alfa     T_koef = 1.7 
      // radnik bez djece                        ...    ide mu alfa     T_koef = 1.0 
      // radnik zaposlen i kod drugog poslodavca ... NE ide mu alfa     T_koef = 0.0 
      
      if(T_dokDate >= ZXC.Date01012017)
      {
         decimal koefZaOsnOdb = T_dokDate < ZXC.Date01012020 ? 1.50M : 1.60M; // 23.12.2019. od 01.01.2020. je ovaj koef 1.60 tj osnovni osobni odbitak je 4000
       // 23.12.2019.
       //decimal alfa = ZXC.Ron((pR._osnOdb * 1.50M       ) / 100M, 0) * 100M; // alfa = osnOdb2017 x 1.5 ... zaokruzen na 100 kn = 3.800                                       
         decimal alfa = ZXC.Ron((pR._osnOdb * koefZaOsnOdb) / 100M, 0) * 100M; // alfa = osnOdb2017 x 1.5 ... zaokruzen na 100 kn = 3.800 , od 01.01.2020. je 1.6 sto daje 4.000
         decimal beta =          pR._osnOdb * (T_koef - 1.00M)               ; // beta = osnOdb2017 x koefOvisanOdDjece                                                         

       //15.11.2022.Danijel ima da je T_koef manji od 1 i onda krivo racuna
       //R_Odbitak    = T_koef.NotZero() ? alfa + beta : 0.00M;
             if(T_koef < 1.00M)  R_Odbitak = ZXC.Ron2(alfa * T_koef);
       else if (T_koef.IsZero()) R_Odbitak = 0.00M                  ;
       else                      R_Odbitak = alfa + beta            ;

      }

      #endregion R_Odbitak in 2017 News and news 2020!!!!

      R_Odbitak = R_Odbitak.Ron2();

      R_Odbitak                -= spent.Odbitak;
      maxOsn1 = pR._maxPorOsn1 - spent.PorOsn1;
      maxOsn2 = pR._maxPorOsn2 - spent.PorOsn2 - pR._maxPorOsn1;
      maxOsn3 = pR._maxPorOsn3 - spent.PorOsn3 - pR._maxPorOsn2;

      if(R_Odbitak < 0.00M) R_Odbitak = 0.00M;
      if(maxOsn1   < 0.00M) maxOsn1   = 0.00M;
      if(maxOsn2   < 0.00M) maxOsn2   = 0.00M;
      if(maxOsn3   < 0.00M) maxOsn3   = 0.00M;

      R_Dohodak = R_TheBruto - R_DoprIz - R_Premije;

      if(R_Dohodak < 0.00M) R_Dohodak = 0.00M;

      if(R_Odbitak > R_Dohodak) R_Odbitak = R_Dohodak;

      R_PorOsnAll = R_Dohodak - R_Odbitak;

      if(T_invalidTip == InvalidEnum.HRVI) R_PorOsnAll *= ((100.00M - T_koefHRVI) / 100.00M);

      if(placaTT == Placa.TT_AUTORHONOR )
      {
       //R_AHizdatak = (R_PorOsnAll * pR._stOthOlak) / 100.00M; 26.12.2016.
         if(T_dokDate < ZXC.Date01012017) R_AHizdatak = (R_PorOsnAll * pR._stOthOlak) / 100.00M;
         
         R_PorOsnAll -= R_AHizdatak;
         R_AHizdatak = R_AHizdatak.Ron2();
      }

      if(placaTT == Placa.TT_NR2_P01NEDOP)
      {
         R_AHizdatak = (R_PorOsnAll * pR._stOthOlak) / 100.00M;

         R_PorOsnAll -= R_AHizdatak;
         R_AHizdatak = R_AHizdatak.Ron2();
      }

      if(placaTT == Placa.TT_AUTORHONUMJ || placaTT == Placa.TT_AUVECASTOPA/*12.2018.*/)
      {
         //R_AHizdatak = (R_PorOsnAll * (pR._stOthOlak + 25.00M) /* TODO: ovih 25 staviti u pRulse */) / 100.00M; 26.12.2016.
         if(T_dokDate < ZXC.Date01012017) R_AHizdatak = (R_PorOsnAll * (pR._stOthOlak + 25.00M) /* TODO: ovih 25 staviti u pRulse */) / 100.00M; 
         
         R_PorOsnAll -= R_AHizdatak;
         R_AHizdatak = R_AHizdatak.Ron2();
      }

      if(placaTT == Placa.TT_AHSAMOSTUMJ)
      {
         R_AHizdatak = (R_PorOsnAll * (pR._stOthOlak + 25.00M) /* TODO: ovih 25 staviti u pRulse */) / 100.00M;

         R_PorOsnAll -= R_AHizdatak;
         R_AHizdatak = R_AHizdatak.Ron2();
      }

      if(placaTT == Placa.TT_NR3_PX1DADOP)//12.2018.
      {
         R_PorOsnAll += R_DoprIz;
      }

      R_PorOsnAll = R_PorOsnAll.Ron2();

    //if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST) R_Dohodak = R_PorOsnAll = 0.00M;                                     //                25.11.2014.
      if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_SAMODOPRINOS) R_Dohodak = R_PorOsnAll = 0.00M; // 09.02.2016. SD 25.11.2014.


   }

   private void Calc_PorezPrirez(PrulesStruct pR, AlreadySpentPtransInThisMonthStruct spent, string placaTT)
   {
      decimal ostaloZaOporezirati;

      ostaloZaOporezirati = R_PorOsnAll;

      if(placaTT == Placa.TT_UGOVORODJELU ||
         placaTT == Placa.TT_NADZORODBOR  ||
         placaTT == Placa.TT_TURSITVIJECE ||
         placaTT == Placa.TT_IDD_KOLONA_4 ||
         placaTT == Placa.TT_SEZZAPPOLJOP ||
         placaTT == Placa.TT_AUTORHONUMJ  ||
         placaTT == Placa.TT_AHSAMOSTUMJ  ||
         placaTT == Placa.TT_AUTORHONOR   ||
         placaTT == Placa.TT_DDBEZDOPRINO || //12.2018.
         placaTT == Placa.TT_AUVECASTOPA  || //12.2018.
         placaTT == Placa.TT_NR1_PX1NEDOP || //12.2018.
         placaTT == Placa.TT_NR2_P01NEDOP || //12.2018.
         placaTT == Placa.TT_NR3_PX1DADOP    //12.2018.
         )
      {

      // 26.12.2016. od 01.01.2017. osnovica za prvu stopu poreza
         if(T_dokDate >= ZXC.Date01012017)
         {
            R_PorOsn1 = R_PorOsnAll;
            R_PorOsn2 =
            R_PorOsn3 =
            R_PorOsn4 = 0.00M;
         }
         else
         {
            R_PorOsn2 = R_PorOsnAll;
            R_PorOsn1 =
            R_PorOsn3 =
            R_PorOsn4 = 0.00M;
         }
      }
      else if(placaTT == Placa.TT_POREZNADOBIT) // 07.03.2014. obracun poreza za isplatu oporezive dobiti
      {
         R_PorOsn1 = R_PorOsnAll;

         R_PorOsn2 =
         R_PorOsn3 =
         R_PorOsn4 = 0.00M;
      }
    //else if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST)                                     //                      25.11.2014. nema poreza za strucno osposobljavanje bez yasnivanja ro
      else if(placaTT == Placa.TT_STRUCNOOSPOS || placaTT == Placa.TT_NEPLACDOPUST || placaTT == Placa.TT_SAMODOPRINOS) // 09.02.2016. dodan SD 25.11.2014. nema poreza za strucno osposobljavanje bez yasnivanja ro
      {
         R_PorOsn1 = 
         R_PorOsn2 =
         R_PorOsn3 =
         R_PorOsn4 = 0.00M;
      }

      else // REDOVNA PLACA ________________________________________ +BivsiRadnik od 23.02.2019.
      {
         if(ostaloZaOporezirati > maxOsn1)
         { // 15% 
            R_PorOsn1 = maxOsn1;
            ostaloZaOporezirati -= R_PorOsn1;
         }
         else
         {
            R_PorOsn1 = ostaloZaOporezirati;
            ostaloZaOporezirati = 0.00M;
         }

         if(ostaloZaOporezirati > maxOsn2)
         { // 25% 
            R_PorOsn2 = maxOsn2;
            ostaloZaOporezirati -= R_PorOsn2;
         }
         else
         {
            R_PorOsn2 = ostaloZaOporezirati;
            ostaloZaOporezirati = 0.00M;
         }

         if(ostaloZaOporezirati > maxOsn3)
         { // 35% 
            R_PorOsn3 = maxOsn3;
            ostaloZaOporezirati -= R_PorOsn3;
         }
         else
         {
            R_PorOsn3 = ostaloZaOporezirati;
            ostaloZaOporezirati = 0.00M;
         }

         if(ostaloZaOporezirati > 0.01M)
         {    // 45% 
            R_PorOsn4 = ostaloZaOporezirati;
         }
         else
         {
            R_PorOsn4 = 0.00M;
         }
      } // REDOVNA PLACA ________________________________________ 

      // TODO: DELLMELATTER 
      if(Math.Abs((R_PorOsnAll) - (R_PorOsn1+R_PorOsn2+R_PorOsn3+R_PorOsn4)) > 0.00M) ZXC.aim_emsg("oAll<{0}> o1+other+o3+o4<{1}> !!!", R_PorOsnAll, R_PorOsn1 + R_PorOsn2 + R_PorOsn3 + R_PorOsn4);

      R_Por1Uk = R_PorOsn1 * pR._stpor1 / 100.00M;
      R_Por2Uk = R_PorOsn2 * pR._stpor2 / 100.00M;
      R_Por3Uk = R_PorOsn3 * pR._stpor3 / 100.00M;
      R_Por4Uk = R_PorOsn4 * pR._stpor4 / 100.00M;

      R_Por1Uk = R_Por1Uk.Ron2();
      R_Por2Uk = R_Por2Uk.Ron2();
      R_Por3Uk = R_Por3Uk.Ron2();
      R_Por4Uk = R_Por4Uk.Ron2(); 

      R_PorezAll  = R_Por1Uk + R_Por2Uk + R_Por3Uk + R_Por4Uk;

      R_Prirez    = R_PorezAll * T_stPrirez / 100.00M;
      R_Prirez    = R_Prirez.Ron2();

      R_PorPrirez = R_PorezAll + R_Prirez;
   }

   private void Calc_Netto(Ptrano[] ptranosOfThisPtrans)
   {
      R_Netto = (R_Dohodak + R_Premije) -
                (R_PorezAll + R_Prirez);


    //if(this.T_TT == Placa.TT_STRUCNOOSPOS || this.T_TT == Placa.TT_NEPLACDOPUST) R_Netto = 0.00M;                                      //                               25.11.2014. nema isplate neta za strucno osposobljavanje bez yasnivanja ro
      if(this.T_TT == Placa.TT_STRUCNOOSPOS || this.T_TT == Placa.TT_NEPLACDOPUST || this.T_TT == Placa.TT_SAMODOPRINOS) R_Netto = 0.00M;//  09.02.2016. i SD nema poreza 25.11.2014. nema isplate neta za strucno osposobljavanje bez yasnivanja ro

      //   // 11.09.2006 (andk\jelka brutoDaNetto :
      //   if(rcnt == 2) ptr->t_netto    = sm_i_dblval("t_netto",    i);

      //// 11.02.2014: 
      ////decimal ptranoNetto;
      //foreach(Ptrano ptrano_rec in ptranosOfThisPtrans)
      //{
      //   if(ptrano_rec.T_izNetoaSt.IsZero()) continue;

      //   // 
      //   //ptranoNetto = ZXC.VvGet_100_from_25and25(ptrano_rec.T_iznosOb, ptrano_rec.T_izNetoaSt);
      //   //if(ZXC.AlmostEqual(ptranoNetto, R_Netto, 0.02M) == false)
      //   //{
      //   //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "{0}\n\nGREŠKA! Promjena neta nakon zadavanja OBUSTAVE koja ovisi o iznosu neta.\n\nPonovite zadavanje obustave!\n\nStari neto: {1}\n\nNovi neto: {2}",
      //   //      T_prezimeIme, ptranoNetto.ToStringVv(), R_Netto.ToStringVv());
      //   //}
      //   // 14.02.2014. 
      //   decimal iznosObust = ZXC.VvGet_25_on_100(R_Netto, ptrano_rec.T_izNetoaSt);
      //   if(ZXC.AlmostEqual(iznosObust, ptrano_rec.T_iznosOb, 0.02M) == false)
      //   {
      //      ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "{0}\n\nGREŠKA! Promjena neta nakon zadavanja OBUSTAVE koja ovisi o iznosu neta.\n\nPonovite zadavanje obustave!\n\nStara obustava: {1}\n\nNova obustava: {2}",
      //         T_prezimeIme, ptrano_rec.T_iznosOb.ToStringVv(), iznosObust.ToStringVv());
      //   }
      //}

      // _HRD_ 
      // ... naknadno: koji k ce nam i jadno i drugo, a treba se i 'NetoIsplata' prikazati u kontra valuti 
      // pa smo izmijenili na nacin da je u R_Netto_EUR ---> R_Netto       u 'kontra' valuti a u           
      //                                    R_Netto_Kn  ---> R_NetoIsplata u 'kontra' valuti a u           
      R_Netto_EUR = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(R_Netto      );
      // premjesteno u Calc_ToPay_NaRuke gdje se i racuna 
    //R_Netto_Kn = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(R_NetoIsplata);
      
   }

   private void Calc_KrizniPorez(PrulesStruct pR, AlreadySpentPtransInThisMonthStruct spent)
   {
      R_KrizPorOsn = R_Netto - R_Premije;

      R_KrizPorOsn += spent.KrizPorOsn; // kumuliraj osnovicu za eventualno vec obracunate u ovome mjesecu 

      decimal prvaGranica, drugaGranica;
      bool    isBef010710 = T_dokDate < new DateTime(2010, 07, 01);

      if(isBef010710)
      {
         prvaGranica  = 3000.005M;
         drugaGranica = 6000.005M;
      }
      else
      {
         prvaGranica  = 6000.005M;
         drugaGranica =    0.005M;
      }

      if(R_KrizPorOsn < prvaGranica)
      {
         R_KrizPorUk  = 0.00M;
         R_KrizPorOsn = 0.00M; 
      }
      else if(R_KrizPorOsn < drugaGranica && isBef010710)
      {
         R_KrizPorUk = R_KrizPorOsn * (pR._stKrizPor1 / 100.00M); // 2% 
      }
      else
      {
         R_KrizPorUk = R_KrizPorOsn * (pR._stKrizPor2 / 100.00M); // 4% 
      }

      R_KrizPorUk -= spent.KrizPorUk; // umanji za vec uplaceno u ovome mjesecu 

      // !!! R_Netto -= R_KrizPorUk; !!! 

      R_NettoWoAdd = R_Netto;

      R_KrizPorUk = R_KrizPorUk.Ron2();

      if(pR._stKrizPor1 + pR._stKrizPor2 == 0.00M) R_KrizPorOsn = 0.00M; // od 01.11.2010. nema vise kriznog poreza - stope su 0 pa nuluramo i osnovicu

       R_NettoAftKrp = (R_Netto - R_KrizPorUk).Ron2(); 

      if(T_isDirNeto == true) R_Netto += T_NetoAdd;

   }

   private void Calc_Obustave(Ptrano[] ptranosOfThisPtrans)
   {
      R_PtranOsCount = ptranosOfThisPtrans.Count();

      R_hasNonObust = R_hasNonObustN = R_hasNonObustZ = false;
    //R_ptranoKind = PtranoKind.OBUSTAVA;

      if(ptranosOfThisPtrans.Length == 0) // ova persona nema nista u Obustavama. 
      {
         return;
      }

      // 30.06.2015: 
      R_hasNonObust  = ptranosOfThisPtrans.Any(ptrano => ptrano.T_ptranoKind != PtranoKind.OBUSTAVA     );
      R_hasNonObustN = ptranosOfThisPtrans.Any(ptrano => ptrano.T_ptranoKind == PtranoKind.NEZASTICENIrn);
      R_hasNonObustZ = ptranosOfThisPtrans.Any(ptrano => ptrano.T_ptranoKind == PtranoKind.ZASTICENIrn  );
    //R_ptranoKind   = R_hasNonObustN ? PtranoKind.NEZASTICENIrn : R_hasNonObustZ ? PtranoKind.ZASTICENIrn : PtranoKind.OBUSTAVA;


      R_Obustave = ptranosOfThisPtrans.Sum(ptrn => ptrn.T_iznosOb);
      
      // Obrazac IP1 29.06.2015.
      R_OnlyObustave  = ptranosOfThisPtrans.Where(ptrn => ptrn.T_ptranoKind == PtranoKind.OBUSTAVA   ).Sum(ptrn => ptrn.T_iznosOb);
    //R_ZasticeniNeto = ptranosOfThisPtrans.Where(ptrn => ptrn.T_ptranoKind == PtranoKind.ZASTICENIrn).Sum(ptrn => ptrn.T_iznosOb);
      // 30.06.2015. zasticeni preko persona(neto na ruke) ili preko ptrano
      
      decimal zasticeniZ    = ptranosOfThisPtrans.Where(ptrn => ptrn.T_ptranoKind == PtranoKind.ZASTICENIrn  ).Sum(ptrn => ptrn.T_iznosOb); // ozanka Z na ptrano - zasticeni ide sa ptrano-a
      decimal netoN         = ptranosOfThisPtrans.Where(ptrn => ptrn.T_ptranoKind == PtranoKind.NEZASTICENIrn).Sum(ptrn => ptrn.T_iznosOb); // oznaka N na ptrano - zasticeni ide preko persona a ovo je preostali neto koji ide na nezasticeni racun - klasicni tekuci
      // 12.03.2019: dodan '+ T_np63' 06.09.2019: dodan '+ T_npTobrok' ... 03.03.2020.T_dopZdr2020
    //decimal zasticeniN    = R_Netto - netoN - R_OnlyObustave + T_prijevoz + T_np63 + T_npTobrok +                (T_isDirNeto == true ? 0.00M : T_NetoAdd);                    // iznos zasticeni koji ne nastaje preko ptrano-a
      decimal zasticeniN    = R_Netto - netoN - R_OnlyObustave + T_prijevoz + T_np63 + T_npTobrok + T_dopZdr2020 + (T_isDirNeto == true ? 0.00M : T_NetoAdd);                    // iznos zasticeni koji ne nastaje preko ptrano-a

      R_ZasticeniNeto = R_hasNonObustZ ? zasticeniZ : R_hasNonObustN ? zasticeniN : 0.00M;
      
      //if(ZXC.CURR_prjkt_rec.IsObustOver3 == false) //23.09.2014.
      //{
         foreach(Ptrano ptrano in ptranosOfThisPtrans)
         {
            if(ptrano.T_ukBrRata.NotZero() && ptrano.T_rbrRate > ptrano.T_ukBrRata)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE!!!:\n\nDjelatnik <{0}>\n\nObustava [{3}] ima redni broj obustave {1},\n\nšto je veće od ukupnog broja rata {2}!",
                  ptrano.T_PrezimeIme, ptrano.T_rbrRate, ptrano.T_ukBrRata, ptrano.T_opisOb + "." + ptrano.T_partija);
            }
         }
      //}

   }

   private void Calc_ToPay_NaRuke()
   {
    //R_2Pay = R_TheBruto + R_DoprNa + T_NetoAdd + T_prijevoz + T_np63 + T_npTobrok               ; // 12.03.2019: dodan '+ T_np63', 06.09.2019: dodan '+ T_npTobrok' 
      R_2Pay = R_TheBruto + R_DoprNa + T_NetoAdd + T_prijevoz + T_np63 + T_npTobrok + T_dopZdr2020; // 03.03.2020: dodan '+ T_dopZdr2020'

      R_NaRuke = R_Netto      - 
                 R_KrizPorUk  + 
                 T_np63       + // 12.03.2019: dodan '+ T_np63' 
                 T_npTobrok   + // 06.09.2019: dodan '+ T_npTobrok' 
                 T_dopZdr2020 + // 03.03.2020: dodan '+ T_dopZdr2020' 
                 T_prijevoz   - 
                 R_Obustave   +
                 (T_isDirNeto == true ? 0.00M : T_NetoAdd); // ako je direkt neto add da ga ne zbraja dvaput 

      // =================================================================================================================

      if(R_NaRuke < 0.00M) {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE: djelatnik <{0}> 'Za isplatu' <{1:N}>!!! (Zbog obustava?)", 
            T_prezimeIme, R_NaRuke);
         R_NaRuke = 0.00M;
      }
      if(ZXC.CURR_prjkt_rec.IsObustOver3 == false)//23.09.2014.
      {
         if((R_Netto / 3.00M).Ron2() < R_Obustave.Ron2())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE: djelatnik <{0}> Suma obustava <{1:N}> je veća od 1/3 neto plaće<{2:N}>!!!",
               T_prezimeIme, R_Obustave, (R_Netto / 3.00M));
         }
      }                 
   /* provjera nekka 2Pay_v1 vs 2Pay_v2: */

      //if(Math.Abs((R_DoprAll  + R_PorPrirez + R_NaRuke  + R_Obustave + R_KrizPorUk) -
      //            (R_TheBruto + R_DoprNa    + T_NetoAdd + T_prijevoz              )) > 0.05M)

      //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "2Pay vs 2Pay <{0}><{1:N}><{2:N}>",
      //      T_prezimeIme,
      //      (R_DoprAll  + R_PorPrirez + R_NaRuke  + R_Obustave + R_KrizPorUk),
      //      (R_TheBruto + R_DoprNa    + T_NetoAdd + T_prijevoz              ));

      // Obrazac IP1 29.06.2015.
    //R_NetoWoZast   = R_Netto - R_ZasticeniNeto + T_prijevoz + T_np63 + T_npTobrok +                (T_isDirNeto == true ? 0.00M : T_NetoAdd); // 12.03.2019: dodan '+ T_np63' 06.09.2019: dodan '+ T_npTobrok'
      R_NetoWoZast   = R_Netto - R_ZasticeniNeto + T_prijevoz + T_np63 + T_npTobrok + T_dopZdr2020 + (T_isDirNeto == true ? 0.00M : T_NetoAdd); // 03.03.2020: dodan '+ T_dopZdr2020'
      R_NetoIsplata  = R_NetoWoZast + R_ZasticeniNeto - R_OnlyObustave;
    // Obrazac NP1 01.07.2015. // 12.03.2019: dodan '+ T_np63'// 06.09.2019: dodan '+ T_npTobrok' 
    //R_NetoALLNeto  = R_Netto + T_prijevoz + T_np63 + T_npTobrok +                (T_isDirNeto == true ? 0.00M : T_NetoAdd); // 01.07.2015. iznos sveukupnog neta i onog dobivenog iz bruta i raznih neto dodataka (hzzo) i prjevoz
      R_NetoALLNeto  = R_Netto + T_prijevoz + T_np63 + T_npTobrok + T_dopZdr2020 + (T_isDirNeto == true ? 0.00M : T_NetoAdd); // 03.03.2020. dodan T_dopZdr2020
      R_NezasticNeto = R_NetoWoZast - R_OnlyObustave;

      R_Netto_Kn = ZXC.EURiIzKuna_ILI_KuneIzEURa_HRD_(R_NetoIsplata);
   }

   //private void Calc_RSm_StranaBvalues(Ptrane[] ptranEsOfThisPtrans)
   //{
   //   bool the10touched = false;
   //   bool the50touched = false;

   //   var ooDistinctList = ptranEsOfThisPtrans.Select(ptrane => ptrane.T_rsOO).Distinct();

   //   Dictionary<string, bool> ooTouched = new Dictionary<string, bool>(ooDistinctList.Count());

   //   foreach(var oo in ooDistinctList)
   //   {
   //      ooTouched.Add(oo, false);
   //   }

   //   foreach(Ptrane ptrane_rec in ptranEsOfThisPtrans)
   //   {
   //      #region R_RSm_Netto

   //      if(ptrane_rec.IsThis_rsOO_HZZO_Pays == true)
   //      {
   //         if(the50touched == false)
   //         {
   //            ptrane_rec.R_RSm_Netto = this.T_NetoAdd;

   //            the50touched = true;
   //         }
   //         else
   //         {
   //            ptrane_rec.R_RSm_Netto = 0.00M;
   //         }
   //      }
   //      else // IsThis_rsOO_HZZO_Pays == false 
   //      {
   //         if(the10touched == false)
   //         {
   //            ptrane_rec.R_RSm_Netto = this.R_NettoWoAdd;

   //            the10touched = true;
   //         }
   //         else
   //         {
   //            ptrane_rec.R_RSm_Netto = 0.00M;
   //         }
   //      }

   //      #endregion R_RSm_Netto

   //      if(ooTouched[ptrane_rec.T_rsOO] == false)
   //      {
   //         ooTouched[ptrane_rec.T_rsOO] = true;

   //         ptranEsOfThisPtrans[0].R_RSm_Bruto  = this.R_TheBruto;
   //         ptranEsOfThisPtrans[0].R_RSm_MioOsn = this.R_MioOsn;
   //         ptranEsOfThisPtrans[0].R_RSm_Mio1Uk = this.R_Mio1stup;
   //         ptranEsOfThisPtrans[0].R_RSm_Mio2Uk = this.R_Mio2stup;
   //      }
   //      else
   //      {
   //         ptranEsOfThisPtrans[0].R_RSm_Bruto  += this.R_TheBruto;
   //         ptranEsOfThisPtrans[0].R_RSm_MioOsn += this.R_MioOsn;
   //         ptranEsOfThisPtrans[0].R_RSm_Mio1Uk += this.R_Mio1stup;
   //         ptranEsOfThisPtrans[0].R_RSm_Mio2Uk += this.R_Mio2stup;

   //         ptrane_rec.R_RSm_Bruto  = this.R_TheBruto;
   //         ptrane_rec.R_RSm_MioOsn = this.R_MioOsn;
   //         ptrane_rec.R_RSm_Mio1Uk = this.R_Mio1stup;
   //         ptrane_rec.R_RSm_Mio2Uk = this.R_Mio2stup;
   //      }

   //   } // foreach(Ptrane ptrane_rec in ptranesOfThisPerson) 

   //}

   #endregion CalcTransResults()

   #region CalcTransResults_FromReportDataRowParameters 

   public void FillFromDataRow_CalcResults_SetRowResults(DS_Placa.IzvjTableRow ptransRow, Placa placa_rec, EnumerableRowCollection<DS_Placa.ptraneRow> ptraneRowsOfThisPerson)
   {
      PtransDao.FillFromTypedPtransDataRow(this, ptransRow);

      this.CalcTransResults(placa_rec);

      PtransDao.FillTypedDataRowResults(ptransRow, this, ptraneRowsOfThisPerson, placa_rec);
   }

   #endregion CalcTransResults_FromReportDataRowParameters

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Ptrans();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Ptrans)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Ptrans)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory


}