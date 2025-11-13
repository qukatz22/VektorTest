using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct KupdobStruct

public struct KupdobStruct
{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   /* 05 */ internal string   _naziv    ;    
   /* 06 */ internal string   _ticker   ;     
   /* 07 */ internal uint     _kupdobCD ;    
   /* 08 */ internal string   _matbr    ;    
   /* 09 */ internal string   _tip      ;      
   /* 10 */ internal string   _dugoIme  ;  
   /* 11 */ internal string   _ulica1   ;   
   /* 12 */ internal string   _ulica2   ;   
   /* 13 */ internal string   _grad     ;     
   /* 14 */ internal string   _postaBr  ;  
   /* 15 */ internal string   _opcina   ;   
   /* 16 */ internal string   _opcCd    ;    
   /* 17 */ internal string   _zupan    ;    
   /* 18 */ internal string   _zupCd    ;    
   /* 19 */ internal string   _ime      ;      
   /* 20 */ internal string   _prezime  ;  
   /* 21 */ internal string   _tel1     ;     
   /* 22 */ internal string   _tel2     ;     
   /* 23 */ internal string   _fax      ;      
   /* 24 */ internal string   _gsm      ;      
   /* 25 */ internal string   _email    ;    
   /* 26 */ internal string   _url      ;      
   /* 27 */ internal string   _ziro1    ;    
   /* 28 */ internal string   _ziro1By  ;  
   /* 29 */ internal string   _ziro1PnbM;
   /* 30 */ internal string   _ziro1PnbV;
   /* 31 */ internal string   _ziro2    ;    
   /* 32 */ internal string   _ziro2By  ;  
   /* 33 */ internal string   _ziro2PnbM;
   /* 34 */ internal string   _ziro2PnbV;
   /* 35 */ internal string   _ziro3    ;    
   /* 36 */ internal string   _ziro3By  ;  
   /* 37 */ internal string   _ziro3PnbM;
   /* 38 */ internal string   _ziro3PnbV;
   /* 39 */ internal string   _ziro4    ;    
   /* 40 */ internal string   _ziro4By  ;  
   /* 41 */ internal string   _ziro4PnbM;
   /* 42 */ internal string   _ziro4PnbV;
   /* 43 */ internal string   _kontoDug ;    
   /* 44 */ internal string   _regob    ;    
   /* 45 */ internal string   _sifDcd   ;   
   /* 46 */ internal string   _sifDname ; 
   /* 47 */ internal DateTime _date     ;
   /* 48 */ internal uint     _putnikID ; 
   /* 49 */ internal string   _putName  ;  
   /* 50 */ internal string   _fuse1    ;    
   /* 51 */ internal string   _fuse2    ;    
   /* 52 */ internal string   _napom1   ;   
   /* 53 */ internal string   _napom2   ;   
   /* 54 */ internal string   _komentar ; 
  // /* 55 */ internal bool     _isObrt   ;   
   /* 55 */ internal ushort   _isObrt   ;   
   /* 56 */ internal bool     _isFrgn   ;   
   /* 57 */ internal bool     _isPdv    ;    // FUSE !!! 
   /* 58 */ internal bool     _isXxx    ;    
   /* 59 */ internal bool     _isYyy    ;    
   /* 60 */ internal bool     _isZzz    ;    
   /* 61 */ internal decimal  _stRbt1   ;   
   /* 62 */ internal decimal  _stRbt2   ;   
   /* 63 */ internal decimal  _stSRbt   ;   
   /* 64 */ internal decimal  _stCsSc   ;   
   /* 65 */ internal decimal  _stProviz ; 
   /* 66 */ internal string   _pnbMProv ; 
   /* 67 */ internal string   _pnbVProv ; 
   /* 68 */ internal string   _pnbMPlaca;
   /* 69 */ internal string   _pnbVPlaca;
   /* 70 */ internal short    _valutaPl ; 
   /* 71 */ internal short    _rokOtprm ; 
   /* 72 */ internal bool     _isCentr  ;  
   /* 73 */ internal uint     _centrID  ;  
   /* 74 */ internal string   _centrTick;
   /* 75 */ internal string   _kontoPot ;    
   /* 76 */ internal bool     _isMtr  ;  
   /* 77 */ internal bool     _isKupac;  
   /* 78 */ internal bool     _isDobav;  
   /* 79 */ internal bool     _isBanka;  
   /* 80 */ internal string   _oib     ;
   /* 81 */ internal string   _drzava  ;
   /* 82 */ internal string   _swift   ;
   /* 83 */ internal string   _iban    ;
   /* 84 */ internal string   _devName ;
   /* 85 */ internal decimal  _finLimit;
   /* 86 */ internal string   _ugovorNo;
   /* 87 */ internal ushort   _komisija; 
   /* 88 */ internal string   _sklKonto;
   /* 89 */ internal uint     _sklNum  ;
   /* 90 */ internal string   _vatCntryCode;

   /* 91 */ internal decimal _mitoIzn ; 
   /* 92 */ internal decimal _mitoSt  ; 
   /* 93 */ internal decimal _investTr; 
   /* 94 */ internal decimal _trecaStr; 
   // +2 
   /* 95 */ internal TimeSpan _timeOd_1; // 1 - ponedjeljak, 2 - utorak, ... 
   /* 96 */ internal TimeSpan _timeDo_1;
   /* 97 */ internal TimeSpan _timeOd_2;
   /* 98 */ internal TimeSpan _timeDo_2;
   /* 99 */ internal TimeSpan _timeOd_3;
   /*100 */ internal TimeSpan _timeDo_3;
   /*101 */ internal TimeSpan _timeOd_4;
   /*102 */ internal TimeSpan _timeDo_4;
   /*103 */ internal TimeSpan _timeOd_5;
   /*104 */ internal TimeSpan _timeDo_5;
   /*105 */ internal TimeSpan _timeOd_6;
   /*106 */ internal TimeSpan _timeDo_6;
   /*107 */ internal TimeSpan _timeOd_7;
   /*108 */ internal TimeSpan _timeDo_7;

   /*109 */ internal /*bool*/ ushort _isAMS;

   /*110 */ internal bool     _idIsPolStmnt;
   /*111 */ internal DateTime _idBirthDate ;
   /*112 */ internal DateTime _idExpDate   ;
   /*113 */ internal string   _idNumber    ;
   /*114 */ internal string   _idIssuer    ;
   /*115 */ internal string   _idCitizenshp;
}

#endregion struct KupdobStruct

public class Kupdob : VvSifrarRecord
{

   #region Fildz

   public const string recordName = "kupdob";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   //public const string mtrTip = "MTR";

   /*private*/protected KupdobStruct currentData;
   /*private*/protected KupdobStruct backupData;


   // BE ADVICED!!! BE ADVICED!!! BE ADVICED!!! BE ADVICED!!! 
   // Ovo si malo zajeb'o. Ubacio si direktno DataLayer u BussinessLogic sa ovim ZXC.KupdobDao.
   // Neki puta ti treba (kokos/jaje) Kupdob objekt prije nego li si inicijalizovao VvDao pa ti
   // ove dolnje recenice bacaju exception. Anyway, svi ti ovo Dao sranje ovdje treba za sortere/indexe
   // jer si lijen (ali i pametan) rucno definirati maxSize, dbType, etc kod iSeg-a    

   protected static System.Data.DataTable TheSchemaTable = ZXC.KupdobDao.TheSchemaTable;
   protected static KupdobDao.KupdobCI    CI             = ZXC.KupdobDao.CI;

   //// ne bu ovo sljakalo kokos/jaje? 
   //private static int GetSchemaColumnSize(int columnIndex)
   //{
   //   return ZXC.KupdobDao.GetSchemaColumnSize(columnIndex);
   //}

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   #endregion Fildz

   #region Constructors

   public Kupdob() : this(0)
   {
   }

   public Kupdob(uint ID) : base() 
   {
      this.currentData          = new KupdobStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID   = ID;
      this.currentData._addTS   = DateTime.MinValue;
      this.currentData._modTS   = DateTime.MinValue;
      this.currentData._addUID  = "";
      this.currentData._modUID  = "";
      this.currentData._lanSrvID = 0;
      this.currentData._lanRecID = 0;

      // well, svi reference types (string, date, ...)

      /* 05 */ this.currentData._naziv     = "";    
      /* 06 */ this.currentData._ticker    = "";     
      /* 07 */ this.currentData._kupdobCD  = 0;     
      /* 08 */ this.currentData._matbr     = "";    
      /* 09 */ this.currentData._tip       = "";      
      /* 00 */ this.currentData._dugoIme   = "";  
      /* 11 */ this.currentData._ulica1    = "";   
      /* 12 */ this.currentData._ulica2    = "";   
      /* 13 */ this.currentData._grad      = "";     
      /* 14 */ this.currentData._postaBr   = "";  
      /* 15 */ this.currentData._opcina    = "";   
      /* 16 */ this.currentData._opcCd     = "";    
      /* 17 */ this.currentData._zupan     = "";    
      /* 18 */ this.currentData._zupCd     = "";    
      /* 19 */ this.currentData._ime       = "";      
      /* 10 */ this.currentData._prezime   = "";  
      /* 21 */ this.currentData._tel1      = "";     
      /* 22 */ this.currentData._tel2      = "";     
      /* 23 */ this.currentData._fax       = "";      
      /* 24 */ this.currentData._gsm       = "";      
      /* 25 */ this.currentData._email     = "";    
      /* 26 */ this.currentData._url       = "";      
      /* 27 */ this.currentData._ziro1     = "";    
      /* 28 */ this.currentData._ziro1By   = "";  
      /* 29 */ this.currentData._ziro1PnbM = "";
      /* 20 */ this.currentData._ziro1PnbV = "";
      /* 31 */ this.currentData._ziro2     = "";    
      /* 32 */ this.currentData._ziro2By   = "";  
      /* 33 */ this.currentData._ziro2PnbM = "";
      /* 34 */ this.currentData._ziro2PnbV = "";
      /* 35 */ this.currentData._ziro3     = "";    
      /* 36 */ this.currentData._ziro3By   = "";  
      /* 37 */ this.currentData._ziro3PnbM = "";
      /* 38 */ this.currentData._ziro3PnbV = "";
      /* 39 */ this.currentData._ziro4     = "";    
      /* 30 */ this.currentData._ziro4By   = "";  
      /* 41 */ this.currentData._ziro4PnbM = "";
      /* 42 */ this.currentData._ziro4PnbV = "";
      /* 43 */ this.currentData._kontoDug  = "";    
      /* 44 */ this.currentData._regob     = "";    
      /* 45 */ this.currentData._sifDcd    = "";   
      /* 46 */ this.currentData._sifDname  = ""; 
      /* 47 */ this.currentData._date      = DateTime.MinValue;
      /* 48 */ this.currentData._putnikID  = 0; 
      /* 49 */ this.currentData._putName   = "";  
      /* 40 */ this.currentData._fuse1     = "";    
      /* 51 */ this.currentData._fuse2     = "";    
      /* 52 */ this.currentData._napom1    = "";   
      /* 53 */ this.currentData._napom2    = "";   
      /* 54 */ this.currentData._komentar  = ""; 
      ///* 55 */ this.currentData._isObrt    = false;   
      /* 55 */ this.currentData._isObrt    = 0;   
      /* 56 */ this.currentData._isFrgn    = false;   
      /* 57 */ this.currentData._isPdv     = false;    
      /* 58 */ this.currentData._isXxx     = false;    
      /* 59 */ this.currentData._isYyy     = false;    
      /* 50 */ this.currentData._isZzz     = false;    
      /* 61 */ this.currentData._stRbt1    = 0m;   
      /* 62 */ this.currentData._stRbt2    = 0m;   
      /* 63 */ this.currentData._stSRbt    = 0m;   
      /* 64 */ this.currentData._stCsSc    = 0m;   
      /* 65 */ this.currentData._stProviz  = 0m; 
      /* 66 */ this.currentData._pnbMProv  = ""; 
      /* 67 */ this.currentData._pnbVProv  = ""; 
      /* 68 */ this.currentData._pnbMPlaca = "";
      /* 69 */ this.currentData._pnbVPlaca = "";
      /* 60 */ this.currentData._valutaPl  = 0; 
      /* 71 */ this.currentData._rokOtprm  = 0; 
      /* 72 */ this.currentData._isCentr   = false;  
      /* 73 */ this.currentData._centrID   = 0;  
      /* 74 */ this.currentData._centrTick = "";
      /* 75 */ this.currentData._kontoPot  = "";    
      /* 76 */ this.currentData._isMtr     = false;  
      /* 77 */ this.currentData._isKupac   = false;  
      /* 78 */ this.currentData._isDobav   = false;  
      /* 79 */ this.currentData._isBanka   = false;  
      /* 80 */ this.currentData._oib       = "";    
      /* 81 */ this.currentData._drzava    = "";    
      /* 82 */ this.currentData._swift     = "";    
      /* 83 */ this.currentData._iban      = "";    
      /* 84 */ this.currentData._devName   = "";    
      /* 85 */ this.currentData._finLimit  = 0m;    
      /* 86 */ this.currentData._ugovorNo  = "";    
      /* 87 */ this.currentData._komisija  = 0;  
      /* 88 */ this.currentData._sklKonto  = "";
      /* 89 */ this.currentData._sklNum    = 0;  
      /* 90 */ this.currentData._vatCntryCode  = "";

      /* 91 */ this.currentData._mitoIzn   = 0m;    
      /* 92 */ this.currentData._mitoSt    = 0m;    
      /* 93 */ this.currentData._investTr  = 0m;    
      /* 94 */ this.currentData._trecaStr  = 0m;    
   
               // 09.08.2018: 
               this.currentData._timeOd_1  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_1  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_2  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_2  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_3  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_3  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_4  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_4  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_5  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_5  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_6  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_6  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeOd_7  = TimeSpan./*MinValue*/Zero;
               this.currentData._timeDo_7  = TimeSpan./*MinValue*/Zero;

      /*109 */ this.currentData._isAMS        = /*false*/0;    
      /*110 */ this.currentData._idIsPolStmnt = false;    
      /*111 */ this.currentData._idBirthDate  = DateTime.MinValue;    
      /*112 */ this.currentData._idExpDate    = DateTime.MinValue;    
      /*113 */ this.currentData._idNumber     = "";    
      /*114 */ this.currentData._idIssuer     = "";    
      /*115 */ this.currentData._idCitizenshp = "";    
   }

   #endregion Constructors

   #region propertiz 

   internal KupdobStruct CurrentData // cijela KupdobStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.KupdobDao; }
   }

   public override string VirtualRecordName
   {
      get { return Kupdob.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Kupdob.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "fp"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set { this.RecID = value; }
   }

   /// <summary>
   /// Overrajdamo defaultno VvSifrarDataRecord ponasanje gdje je ovo true (za npr. sklad i osred)
   /// </summary>
   public override bool IsStringAutoSifra // !!! DODANO tek 23.12.2014: do tada je kupdob bio IsStringAutoSifra = true !!! 
   {
      get { return false; }
   }

   public override string SifraColName
   {
      get { return "kupdobCD"; }
   }

   public override string SifraColValue
   {
      get { return this.KupdobCD.ToString() + "_" + this.Ticker; }
   }

   public override object SifraUniqueSingleColValue
   {
      get { return this./*SifraColValue*/KupdobCD; }
   }

   public override System.Data.DataRow SifraUniqueSingleColDrSchema
   {
      get { return ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD]; }
   }

   public override uint UintSifraRootNum    { get { return (ZXC.IsSkyEnvironment ? ZXC.vvDB_ServerID : 0); } }
   public override uint UintSifraBaseFactor { get { return (ZXC.IsSkyEnvironment ? 10000u            : 0); } } // npr 12 * 10000 = 120000 ... 6 znamenki za sifru 

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS;  } }
   public override string   VirtualAddUID{ get { return this.AddUID; } }
   public override string   VirtualModUID{ get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Kupdob.sorterNaziv; }
   }

   /// <summary>
   /// Ovo je specijalan slucaj. Iako 'Ticker' nije foreign key (nego RecID) ipak se upisuje u ftrans, ...
   /// Pa nedam da sa mijenja na ispravi
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         if(this is Prjkt)
         {
            return (this.currentData._kupdobCD         != this.backupData._kupdobCD || 
                  //this.currentData._naziv            != this.backupData._naziv    || 
                    this.currentData._ticker.ToUpper() != this.backupData._ticker.ToUpper());
         }
         else // Kupdob 
         {
            return (this.currentData._kupdobCD         != this.backupData._kupdobCD || 
                    this.currentData._naziv            != this.backupData._naziv    || 
                    this.currentData._ticker.ToUpper() != this.backupData._ticker.ToUpper());
         }
      }
   }

   /// <summary>
   /// Gets or sets a list of Ftrans (ala customers orders) for this Kupdob.
   /// </summary>
   public List<Ftrans> Ftranses { get; set; }

   public override void InvokeTransClear()
   {
      if(this.Ftranses != null) this.Ftranses.Clear();
   }

   //===================================================================
   //===================================================================
   //===================================================================


   public uint BackupedKupdobCD
   {
      get { return this.backupData._kupdobCD; }
   }

   public string BackupedTicker
   {
      get { return this.backupData._ticker; }
   }

   public uint RecID
   {
      get { return this.currentData._recID; }
      set {        this.currentData._recID = value; }
   }

   public DateTime AddTS
   {
      get { return this.currentData._addTS; }
      set {        this.currentData._addTS = value; }
   }

   public DateTime ModTS
   {
      get { return this.currentData._modTS; }
      set {        this.currentData._modTS = value; }
   }

   public string AddUID
   {
      get { return this.currentData._addUID; }
      set {        this.currentData._addUID = value; }
   }

   public string ModUID
   {
      get { return this.currentData._modUID; }
      set {        this.currentData._modUID = value; }
   }

   public uint LanSrvID { get { return this.currentData._lanSrvID; } set { this.currentData._lanSrvID = value; } }
   public uint LanRecID { get { return this.currentData._lanRecID; } set { this.currentData._lanRecID = value; } }

/**/

   /* 05 */ public string  Naziv          
   {   
      get { return this.currentData._naziv; }
      set {        this.currentData._naziv = value; }
   }
   
   /* 06 */ 
   public string  Ticker            
   {   
      get { return this.currentData._ticker; }
      set {        this.currentData._ticker = value; }
   }
   /* 07 */
   public uint KupdobCD
   {
      get { return this.currentData._kupdobCD; }
      set { this.currentData._kupdobCD = value; }
   }
   /* 08 */
   public string Matbr          
   {   
      get { return this.currentData._matbr; }
      set {        this.currentData._matbr = value; }
   }
   /* 09 */ public string  Tip              
   {   
      get { return this.currentData._tip; }
      set {        this.currentData._tip = value; }
   }
   /* 10 */ public string  DugoIme      
   {   
      get { return this.currentData._dugoIme; }
      set {        this.currentData._dugoIme = value; }
   }
   /* 11 */ public string  Ulica1        
   {   
      get { return this.currentData._ulica1; }
      set {        this.currentData._ulica1 = value; }
   }
   /* 12 */ public string  Ulica2        
   {   
      get { return this.currentData._ulica2; }
      set {        this.currentData._ulica2 = value; }
   }
   /* 13 */ public string  Grad            
   {   
      get { return this.currentData._grad/*.Replace((char)0x01, ' ')*/; } // primjer kada 0x01 smeta pri XML serialize 
      set {        this.currentData._grad = value; }
   }
   /* 14 */ public string  PostaBr      
   {   
      get { return this.currentData._postaBr; }
      set {        this.currentData._postaBr = value; }
   }
   /* 15 */ public string  Opcina        
   {   
      get { return this.currentData._opcina; }
      set {        this.currentData._opcina = value; }
   }
   /* 16 */ public string  OpcCd          
   {   
      get { return this.currentData._opcCd; }
      set {        this.currentData._opcCd = value; }
   }
   /* 17 */ public string  Zupan          
   {   
      get { return this.currentData._zupan; }
      set {        this.currentData._zupan = value; }
   }
   /* 18 */ public string  ZupCd          
   {   
      get { return this.currentData._zupCd; }
      set {        this.currentData._zupCd = value; }
   }
   /* 19 */ public string  Ime              
   {   
      get { return this.currentData._ime; }
      set {        this.currentData._ime = value; }
   }
   /* 20 */ public string  Prezime      
   {   
      get { return this.currentData._prezime; }
      set {        this.currentData._prezime = value; }
   }
   /* 21 */ public string  Tel1            
   {   
      get { return this.currentData._tel1; }
      set {        this.currentData._tel1 = value; }
   }
   /* 22 */ public string  Tel2            
   {   
      get { return this.currentData._tel2; }
      set {        this.currentData._tel2 = value; }
   }
   /* 23 */ public string  Fax              
   {   
      get { return this.currentData._fax; }
      set {        this.currentData._fax = value; }
   }
   /* 24 */ public string  Gsm              
   {   
      get { return this.currentData._gsm; }
      set {        this.currentData._gsm = value; }
   }
   /* 25 */ public string  Email          
   {   
      get { return this.currentData._email; }
      set {        this.currentData._email = value; }
   }
   /* 26 */ public string  Url              
   {   
      get { return this.currentData._url; }
      set {        this.currentData._url = value; }
   }
   /* 27 */ public string  Ziro1          
   {   
      get { return this.currentData._ziro1; }
      set {        this.currentData._ziro1 = value; }
   }
   public string  Ziro1_asIBAN { get { return ZXC.GetIBANfromOldZiro(this.currentData._ziro1); } }

   /* 28 */ public string  Ziro1By      
   {   
      get { return this.currentData._ziro1By; }
      set {        this.currentData._ziro1By = value; }
   }
   /* 29 */ public string  Ziro1PnbM
   {   
      get { return this.currentData._ziro1PnbM; }
      set {        this.currentData._ziro1PnbM = value; }
   }
   /* 30 */ public string  Ziro1PnbV
   {   
      get { return this.currentData._ziro1PnbV; }
      set {        this.currentData._ziro1PnbV = value; }
   }
   /* 31 */ public string  Ziro2          
   {   
      get { return this.currentData._ziro2; }
      set {        this.currentData._ziro2 = value; }
   }
   /* 32 */ public string  Ziro2By      
   {   
      get { return this.currentData._ziro2By; }
      set {        this.currentData._ziro2By = value; }
   }
   /* 33 */ public string  Ziro2PnbM
   {   
      get { return this.currentData._ziro2PnbM; }
      set {        this.currentData._ziro2PnbM = value; }
   }
   /* 34 */ public string  Ziro2PnbV   
   {   
      get { return this.currentData._ziro2PnbV; }
      set {        this.currentData._ziro2PnbV = value; }
   }
   /* 35 */ public string  Ziro3          
   {   
      get { return this.currentData._ziro3; }
      set {        this.currentData._ziro3 = value; }
   }
   /* 36 */ public string  Ziro3By      
   {   
      get { return this.currentData._ziro3By; }
      set {        this.currentData._ziro3By = value; }
   }
   /* 37 */ public string  Ziro3PnbM   
   {   
      get { return this.currentData._ziro3PnbM; }
      set {        this.currentData._ziro3PnbM = value; }
   }
   /* 38 */ public string  Ziro3PnbV   
   {   
      get { return this.currentData._ziro3PnbV; }
      set {        this.currentData._ziro3PnbV = value; }
   }
   /* 39 */ public string  Ziro4          
   {   
      get { return this.currentData._ziro4; }
      set {        this.currentData._ziro4 = value; }
   }
   /* 40 */ public string  Ziro4By      
   {   
      get { return this.currentData._ziro4By; }
      set {        this.currentData._ziro4By = value; }
   }
   /* 41 */ public string  Ziro4PnbM   
   {   
      get { return this.currentData._ziro4PnbM; }
      set {        this.currentData._ziro4PnbM = value; }
   }
   /* 42 */ public string  Ziro4PnbV   
   {   
      get { return this.currentData._ziro4PnbV; }
      set {        this.currentData._ziro4PnbV = value; }
   }
   /* 43 */ public string  KontoDug          
   {   
      get { return this.currentData._kontoDug; }
      set {        this.currentData._kontoDug = value; }
   }
   /* 44 */ public string  Regob          
   {   
      get { return this.currentData._regob; }
      set {        this.currentData._regob = value; }
   }
   /* 45 */ public string  SifDcd        
   {   
      get { return this.currentData._sifDcd; }
      set {        this.currentData._sifDcd = value; }
   }
   /* 46 */ public string  SifDname    
   {   
      get { return this.currentData._sifDname; }
      set {        this.currentData._sifDname = value; }
   }
   /* 47 */ public DateTime Date       
   {   
      get { return this.currentData._date; }
      set {        this.currentData._date = value; }
   }
   /* 48 */ public uint  PutnikID    
   {   
      get { return this.currentData._putnikID; }
      set {        this.currentData._putnikID = value; }
   }
   /* 49 */ public string  PutName      
   {   
      get { return this.currentData._putName; }
      set {        this.currentData._putName = value; }
   }
   /* 50 */ public string  Fuse1          
   {   
      get { return this.currentData._fuse1; }
      set {        this.currentData._fuse1 = value; }
   }
   /* 51 */ public string  Fuse2          
   {   
      get { return this.currentData._fuse2; }
      set {        this.currentData._fuse2 = value; }
   }
   /* 52 */ public string  Napom1        
   {   
      get { return this.currentData._napom1; }
      set {        this.currentData._napom1 = value; }
   }
   /* 53 */ public string  Napom2        
   {   
      get { return this.currentData._napom2; }
      set {        this.currentData._napom2 = value; }
   }
   /* 54 */ public string  Komentar    
   {   
      get { return this.currentData._komentar; }
      set {        this.currentData._komentar = value; }
   }
   public bool IsObrt
   {
      //get { return this.currentData._isObrt.NotZero(); }
      get 
      { 
         return (this.currentData._isObrt == 1 ||
                 this.currentData._isObrt == 2 ||
                 this.currentData._isObrt == 3);
      }
   }
   public bool IsFizickaOsoba
   {
      //get { return this.currentData._isObrt.NotZero(); }
      get 
      { 
         return (this.currentData._isObrt == /*4*/ 6);
      }
   }
   /* 55 */ public ZXC.PdvRTipEnum PdvRTip
   {
      get { return (ZXC.PdvRTipEnum)this.currentData._isObrt; }
      set {                         this.currentData._isObrt = (ushort)value; }
   }

   //public bool IsObrt_asB
   //{
   //   get 
   //   { 
   //      if(this.IsObrt == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsObrt = 0; 
   //      else               this.IsObrt = 1; 
   //   }
   //}
   /* 56 */ public bool    IsFrgn        
   {   
      get { return this.currentData._isFrgn; }
      set {        this.currentData._isFrgn = value; }
   }
   //public bool IsFrgn_asB
   //{
   //   get 
   //   { 
   //      if(this.IsFrgn == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsFrgn = 0; 
   //      else               this.IsFrgn = 1; 
   //   }
   //}
   /* 57 */ public bool    IsPdv // FUSE !!! 
   {   
      get { return this.currentData._isPdv; }
      set {        this.currentData._isPdv = value; }
   }
   //public bool IsPdv_asB
   //{
   //   get 
   //   { 
   //      if(this.IsPdv == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsPdv = 0; 
   //      else               this.IsPdv = 1; 
   //   }
   //}
   /* 58 */ public bool    IsDevizno          
   {   
      get { return this.currentData._isXxx; }
      set {        this.currentData._isXxx = value; }
   }
   //public bool IsXxx_asB
   //{
   //   get 
   //   { 
   //      if(this.IsXxx == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsXxx = 0; 
   //      else               this.IsXxx = 1; 
   //   }
   //}
   /* 59 */ public bool    IsYyy          
   {   
      get { return this.currentData._isYyy; }
      set {        this.currentData._isYyy = value; }
   }
   //public bool IsYyy_asB
   //{
   //   get 
   //   { 
   //      if(this.IsYyy == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsYyy = 0; 
   //      else               this.IsYyy = 1; 
   //   }
   //}
   /* 60 */ public bool    IsZzz          
   {   
      get { return this.currentData._isZzz; }
      set {        this.currentData._isZzz = value; }
   }
   //public bool IsZzz_asB
   //{
   //   get 
   //   { 
   //      if(this.IsZzz == 0)  return false; 
   //      else                  return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsZzz = 0; 
   //      else               this.IsZzz = 1; 
   //   }
   //}
   /* 61 */ public decimal StRbt1        
   {   
      get { return this.currentData._stRbt1; }
      set {        this.currentData._stRbt1 = value; }
   }
   /* 62 */ public decimal StRbt2        
   {   
      get { return this.currentData._stRbt2; }
      set {        this.currentData._stRbt2 = value; }
   }
   /* 63 */ public decimal StSRbt        
   {   
      get { return this.currentData._stSRbt; }
      set {        this.currentData._stSRbt = value; }
   }
   /* 64 */ public decimal StCsSc        
   {   
      get { return this.currentData._stCsSc; }
      set {        this.currentData._stCsSc = value; }
   }
   /* 65 */ public decimal StProviz    
   {   
      get { return this.currentData._stProviz; }
      set {        this.currentData._stProviz = value; }
   }
   /* 66 */ public string  PnbMProv    
   {   
      get { return this.currentData._pnbMProv; }
      set {        this.currentData._pnbMProv = value; }
   }
   /* 67 */ public string  PnbVProv    
   {   
      get { return this.currentData._pnbVProv; }
      set {        this.currentData._pnbVProv = value; }
   }
   /* 68 */ public string  PnbMPlaca
   {   
      get { return this.currentData._pnbMPlaca; }
      set {        this.currentData._pnbMPlaca = value; }
   }
   /* 69 */ public string  PnbVPlaca
   {   
      get { return this.currentData._pnbVPlaca; }
      set {        this.currentData._pnbVPlaca = value; }
   }
   /* 70 */ public short   ValutaPl    
   {   
      get { return this.currentData._valutaPl; }
      set {        this.currentData._valutaPl = value; }
   }
   /* 71 */ public short   RokOtprm    
   {   
      get { return this.currentData._rokOtprm; }
      set {        this.currentData._rokOtprm = value; }
   }
   /* 72 */ public bool    IsCentr      
   {   
      get { return this.currentData._isCentr; }
      set {        this.currentData._isCentr = value; }
   }
   //public bool IsCentr_asB
   //{
   //   get 
   //   { 
   //      if(this.IsCentr == 0)  return false; 
   //      else                   return true; 
   //   }
   //   set 
   //   {
   //      if(value == false) this.IsCentr = 0; 
   //      else               this.IsCentr = 1; 
   //   }
   //}
   /* 73 */ public uint    CentrID      
   {   
      get { return this.currentData._centrID; }
      set {        this.currentData._centrID = value; }
   }
   /* 74 */ public string  CentrTick
   {   
      get { return this.currentData._centrTick; }
      set {        this.currentData._centrTick = value; }
   }

   /* 75 */ public string  KontoPot
   {   
      get { return this.currentData._kontoPot; }
      set {        this.currentData._kontoPot = value; }
   }

   /* 76 */ public bool    IsMtr        
   {   
      get { return this.currentData._isMtr; }
      set {        this.currentData._isMtr = value; }
   }
   /* 77 */ public bool    IsKupac
   {   
      get { return this.currentData._isKupac; }
      set {        this.currentData._isKupac = value; }
   }
   /* 78 */ public bool    IsDobav
   {   
      get { return this.currentData._isDobav; }
      set {        this.currentData._isDobav = value; }
   }
   /* 79 */ public bool    IsBanka
   {   
      get { return this.currentData._isBanka; }
      set {        this.currentData._isBanka = value; }
   }
   /* 80 */ public string Oib
   {
      get { return this.currentData._oib; }
      set {        this.currentData._oib = value; }
   }
   /* 81 */ public string Drzava
   {
      get { return this.currentData._drzava; }
      set {        this.currentData._drzava = value; }
   }

   /* 82 */ public string Swift
   {
      get { return this.currentData._swift; }
      set {        this.currentData._swift = value; }
   }

   /* 83 */ public string Iban
   {
      get { return this.currentData._iban; }
      set {        this.currentData._iban = value; }
   }

   /* 84 */ public string DevName
   {
      // 12.01.2023: HB Tamsi :-) 
    //get { return this.currentData._devName; }
      get 
      {
         bool isUseless_EURoDevName = (ZXC.IsEURoERA_projectYear && currentData._devName.ToUpper() == "EUR") ? true : false;
         bool isUseless_KuneDevName =                               currentData._devName.ToUpper() == "HRK";

         bool isUselessDevName = isUseless_EURoDevName || isUseless_KuneDevName;

         return isUselessDevName ? "" : this.currentData._devName; 
      }

      set {        this.currentData._devName = value; }
   }

   /* 84 */ public string DevName_OLD
   {
      get {return this.currentData._devName;         }
      set {       this.currentData._devName = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 85 */ public decimal FinLimit
   {
      get { return this.currentData._finLimit; }
      set {        this.currentData._finLimit = value; }
   }


   /* 86 */ public string UgovorNo
   {
      get { return this.currentData._ugovorNo; }
      set {        this.currentData._ugovorNo = value; }
   }

   /* 87 */ public ZXC.KomisijaKindEnum Komisija
   {
      get { return (ZXC.KomisijaKindEnum)this.currentData._komisija; }
      set {                              this.currentData._komisija = (ushort)value; }
   }

   public bool IsKomisMalopSkl { get { return Komisija == ZXC.KomisijaKindEnum.MALOPRODAJNA ? true : false; } }

   /* 88 */ public string SklKonto
   {
      get { return this.currentData._sklKonto; }
      set {        this.currentData._sklKonto = value; }
   }

   /* 89 */ public uint SklNum
   {
      get { return this.currentData._sklNum; }
      set {        this.currentData._sklNum = value; }
   }

   /* 90 */ public string VatCntryCode
   {
      get { return this.currentData._vatCntryCode; }
      set {        this.currentData._vatCntryCode = value; }
   }

   public string VatCntryCode_NonEmpty
   {
      get { return this.VatCntryCode.IsEmpty() ? "HR" : this.VatCntryCode; }
   }

   public string VATnumber
   {
      get
      {
         // 26.11.2018: 
       //string vatCntryCode = this.VatCntryCode.IsEmpty() ? "HR" : this.VatCntryCode;
       //return vatCntryCode + this.Oib;
         return VatCntryCode_NonEmpty + this.Oib;
      }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 91 */ public decimal MitoIzn // iznos po litri 
   {
      get { return this.currentData._mitoIzn; }
      set {        this.currentData._mitoIzn = value; }
   }
   /* 92 */ public decimal MitoSt // posto od fakture bez pdv-a (s_ukKCR) 
   {
      get { return this.currentData._mitoSt; }
      set {        this.currentData._mitoSt = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 93 */ public decimal InvestTr // iznos po litri 
   {
      get { return this.currentData._investTr; }
      set {        this.currentData._investTr = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 94 */ public decimal TrecaStr // iznos po partneru 
   {
      get { return this.currentData._trecaStr; }
      set {        this.currentData._trecaStr = value; }
   }

   /* 95 */ public TimeSpan TimeOd_1 { get { return this.currentData._timeOd_1; } set { this.currentData._timeOd_1 = value; } }
   /* 95 */ public TimeSpan TimeDo_1 { get { return this.currentData._timeDo_1; } set { this.currentData._timeDo_1 = value; } }
   /* 95 */ public TimeSpan TimeOd_2 { get { return this.currentData._timeOd_2; } set { this.currentData._timeOd_2 = value; } }
   /* 95 */ public TimeSpan TimeDo_2 { get { return this.currentData._timeDo_2; } set { this.currentData._timeDo_2 = value; } }
   /* 95 */ public TimeSpan TimeOd_3 { get { return this.currentData._timeOd_3; } set { this.currentData._timeOd_3 = value; } }
   /* 95 */ public TimeSpan TimeDo_3 { get { return this.currentData._timeDo_3; } set { this.currentData._timeDo_3 = value; } }
   /* 95 */ public TimeSpan TimeOd_4 { get { return this.currentData._timeOd_4; } set { this.currentData._timeOd_4 = value; } }
   /* 95 */ public TimeSpan TimeDo_4 { get { return this.currentData._timeDo_4; } set { this.currentData._timeDo_4 = value; } }
   /* 95 */ public TimeSpan TimeOd_5 { get { return this.currentData._timeOd_5; } set { this.currentData._timeOd_5 = value; } }
   /* 95 */ public TimeSpan TimeDo_5 { get { return this.currentData._timeDo_5; } set { this.currentData._timeDo_5 = value; } }
   /* 95 */ public TimeSpan TimeOd_6 { get { return this.currentData._timeOd_6; } set { this.currentData._timeOd_6 = value; } }
   /* 95 */ public TimeSpan TimeDo_6 { get { return this.currentData._timeDo_6; } set { this.currentData._timeDo_6 = value; } }
   /* 95 */ public TimeSpan TimeOd_7 { get { return this.currentData._timeOd_7; } set { this.currentData._timeOd_7 = value; } }
   /* 95 */ public TimeSpan TimeDo_7 { get { return this.currentData._timeDo_7; } set { this.currentData._timeDo_7 = value; } }

   /*109 */ public /*bool*/ ushort IsAMS        { get { return this.currentData._isAMS       ; } set { this.currentData._isAMS        = value; } }

   /*109 */ public ZXC.AMSstatus AMSstatus
   {
      get { return (ZXC.AMSstatus)this.currentData._isAMS; } set { this.currentData._isAMS = (ushort)value; } //tam ?????
   }


   /*110 */ public bool     IdIsPolStmnt { get { return this.currentData._idIsPolStmnt; } set { this.currentData._idIsPolStmnt = value; } }
   /*111 */ public DateTime IdBirthDate  { get { return this.currentData._idBirthDate ; } set { this.currentData._idBirthDate  = value; } }
   /*112 */ public DateTime IdExpDate    { get { return this.currentData._idExpDate   ; } set { this.currentData._idExpDate    = value; } }
   /*113 */ public string   IdNumber     { get { return this.currentData._idNumber    ; } set { this.currentData._idNumber     = value; } }
   /*114 */ public string   IdIssuer     { get { return this.currentData._idIssuer    ; } set { this.currentData._idIssuer     = value; } }
   /*115 */ public string   IdCitizenshp { get { return this.currentData._idCitizenshp; } set { this.currentData._idCitizenshp = value; } }

   //private bool ziroListLoaded = false;
   private List<ZXC.CdAndName_CommonStruct> ziroList;
   public  List<ZXC.CdAndName_CommonStruct> ZiroList
   {
      get
      {
         //if(ziroList == null)
         //if(ziroListLoaded == false)
         {
            //ziroListLoaded = true;
            if(ziroList == null) ziroList = new List<ZXC.CdAndName_CommonStruct>(4);
            else                 ziroList.Clear();

            if(Ziro1.NotEmpty()) ziroList.Add(new ZXC.CdAndName_CommonStruct(Ziro1, Ziro1By));
            if(Ziro2.NotEmpty()) ziroList.Add(new ZXC.CdAndName_CommonStruct(Ziro2, Ziro2By));
            if(Ziro3.NotEmpty()) ziroList.Add(new ZXC.CdAndName_CommonStruct(Ziro3, Ziro3By));
            if(Ziro4.NotEmpty()) ziroList.Add(new ZXC.CdAndName_CommonStruct(Ziro4, Ziro4By));
         }

         return ziroList;
      }
   }

   public string UlicniBroj_1
   {
      get
      {
         if(this.Ulica1.IsEmpty()) return "";

         if(this.Ulica1.ToLower().EndsWith("bb")) return "BB";

         string[] splitters = this.Ulica1.Split(' ');

         string ulicniBroj = splitters[splitters.Length - 1];

         if(ulicniBroj.Length.IsZero() || char.IsDigit(ulicniBroj[0]) == false) return "0";

         return ulicniBroj;
      }
   }

   public string UlicniBroj_1_BezDodatka
   {
      get
      {
         return UlicniBroj_1; // TODO: 
      }
   }

   public string UlicniBroj_1_Dodatak
   {
      get
      {
         return ""; // TODO: 
      }
   }

   public string UlicaBezBroja_1
   {
      get
      {
         if(this.Ulica1.IsEmpty()) return "";

         string[] splitters = this.Ulica1.Split(' ');

         string ulicaBezBroja = "";
         foreach(string part in splitters)
         {
            if(part.Length.NotZero() && char.IsDigit(part[0]) == false) ulicaBezBroja += part;
         }

         return ulicaBezBroja;
      }
   }

   public string UlicniBroj_2
   {
      get
      {
         if(this.Ulica2.IsEmpty()) return "";

         if(this.Ulica2.ToLower().EndsWith("bb")) return "BB";

         string[] splitters = this.Ulica2.Split(' ');

         string ulicniBroj = splitters[splitters.Length - 1];

         if(ulicniBroj.Length.IsZero() || char.IsDigit(ulicniBroj[0]) == false) return "0";

         return ulicniBroj;
      }
   }

   public string UlicaBezBroja_2
   {
      get
      {
         if(this.Ulica2.IsEmpty()) return "";

         string[] splitters = this.Ulica2.Split(' ');

         string ulicaBezBroja = "";
         foreach(string part in splitters)
         {
            if(part.Length.NotZero() && char.IsDigit(part[0]) == false) ulicaBezBroja += part;
         }

         return ulicaBezBroja;
      }
   }

   public string OznPorBr_opzStat1
   {
      get
      {
       //bool isHRVATSKA = VatCntryCode == ("HR") || 
       //                  (VatCntryCode.IsEmpty() && Drzava.IsEmpty()) ||
       //                  Drzava.ToUpper().StartsWith("HRVAT");
       //
              if(IsHRVATSKA                                 ) return "1";
         else if(ZXC.EU_VatCodes_woHR.Contains(VatCntryCode)) return "2";
         else                                                 return "3";
      }
   }

   public string OIB_opzStat1
   {
      get
      {
         if(OznPorBr_opzStat1 == "2") return VATnumber;
         else                         return Oib      ;
      }
   }

   public bool IsHRVATSKA
   {
      get
      {
       //// Polzela BiH pattern: VATcode empty & Drzava notEmpty & Drzava does NOT starts with 'HRVAT' 
       //if(VatCntryCode.IsEmpty() && 
       //   Drzava.NotEmpty     () && 
       //   Drzava.ToUpper      ().StartsWith("HRVAT") == false) return false;
       //
       //// else 

         return 
            
            VatCntryCode == ("HR")                       ||
            (VatCntryCode.IsEmpty() && Drzava.IsEmpty()) ||
            Drzava.ToUpper().StartsWith("HRVAT");
      }
   }

   public string ZipAndMjesto
   {
      get
      {
         if(PostaBr.NotEmpty()) return PostaBr + " " + Grad;
         else                 return                   Grad;
      }
   }

   #endregion propertiz 

   #region ToString

   public override string ToString()
   {
      return Ticker + " (" + Naziv + ")" + " (šif: " + /*RecID*/KupdobCD.ToString("000000") + ")" + " (oib: " + Oib + ")";
   }

 //public static string TickerToken = " T: ";
   public static string TickerToken = " Sif: ";

 //public string NazivUniqueAddition { get { return this.Ticker; } }
   public string NazivUniqueAddition { get { return this.KupdobCD.ToString("000000"); } }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Kupdob kupdob_rec = (Kupdob)vvDataRecord;

      // proba za filtriranje: 
      // if(artikl_rec.TT != "ZXC" && artikl_rec.IsCentr_asB != true) return "";

      if(restrictor == ZXC.AutoCompleteRestrictor.KID_Centrala_Only && kupdob_rec.IsCentr == false) return "";
    //if(restrictor == ZXC.AutoCompleteRestrictor.KID_Mtros_Only    && artikl_rec.Tip     != Kupdob.mtrTip) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.KID_Mtros_Only    && kupdob_rec.IsMtr     == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.KID_Kupac_Only    && kupdob_rec.IsKupac   == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.KID_Dobav_Only    && kupdob_rec.IsDobav   == false) return "";
      if(restrictor == ZXC.AutoCompleteRestrictor.KID_Banka_Only    && kupdob_rec.IsBanka   == false) return "";

      switch(sifrarType)
      {
         // 10.08.2024: Buon compleano! Nono Lorenzo :-) 
         // Autocomplete duplicates pokusaj rjesenja     
       //case VvSQL.SorterType.Name      : return kupdob_rec.Naziv;
         case VvSQL.SorterType.Name      : return kupdob_rec.Naziv + TickerToken + kupdob_rec.NazivUniqueAddition;
       //case VvSQL.SorterType.RecID     : return artikl_rec.RecID.ToString();
         case VvSQL.SorterType.Code      : return kupdob_rec.KupdobCD.ToString();
         case VvSQL.SorterType.Person    : return kupdob_rec.Prezime;
         case VvSQL.SorterType.OIB       : return kupdob_rec.Oib;
         case VvSQL.SorterType.Ticker    : return kupdob_rec.Ticker;
         case VvSQL.SorterType.City      : return kupdob_rec.Grad;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Kupdob.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   public static string GetCleanKupdobNameFromTokenized(string dirtyName)
   {
      string cleanNaziv;

      int visakIdx = dirtyName.IndexOf(Kupdob.TickerToken);

      if(dirtyName.Length.NotZero() && visakIdx.IsZeroOrPositive()) //return;
      {
         cleanNaziv = dirtyName.Substring(0, visakIdx);
      }
      else
      {
         cleanNaziv = dirtyName;
      }

      return cleanNaziv;
   }

   #endregion ToString

      #region Implements IEditableObject

      #region Utils
      /// <summary>
      /// this.currentData = this.backupData;
      /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<KupdobStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<KupdobStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<KupdobStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<KupdobStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<KupdobStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Kupdob newObject = new Kupdob();

      Generic_CloneData<KupdobStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Kupdob MakeDeepCopy()
   {
      return (Kupdob)Clone();
   }

   #endregion

   #region SorterCurrVal

   public static VvSQL.RecordSorter sorterKCD = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI./*prjktKupdobCD*/kupdobCD]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Sifra", VvSQL.SorterType.Code/*RecID*/, false);

   public static VvSQL.RecordSorter sorterNaziv = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.naziv]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Naziv", VvSQL.SorterType.Name, false);

   public static VvSQL.RecordSorter sorterTicker = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ticker]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Ticker", VvSQL.SorterType.Ticker, false);

   public static VvSQL.RecordSorter sorterPrezime = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.prezime]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.kupdobCD/*prjktKupdobCD*/]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Prezime", VvSQL.SorterType.Person, false);

   public static VvSQL.RecordSorter sorterOIB = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.oib]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.kupdobCD/*prjktKupdobCD*/]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "OIB", VvSQL.SorterType.OIB, false);

   public static VvSQL.RecordSorter sorterCity = new VvSQL.RecordSorter(Kupdob.recordName, Kupdob.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.grad]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.kupdobCD/*prjktKupdobCD*/]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Grad", VvSQL.SorterType.City, false);

   private VvSQL.RecordSorter[] _sorters = 
      new VvSQL.RecordSorter[]
      { 
         sorterNaziv, 
         sorterTicker, 
         sorterKCD, 
         sorterOIB, 
         sorterPrezime, 
         sorterCity
      };

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         //case VvSQL.SorterType.RecID:  return new object[] { this.RecID,               RecVer };
         case VvSQL.SorterType.Code  : return new object[] { this.KupdobCD,            RecVer };
         case VvSQL.SorterType.Name  : return new object[] { this.Naziv,               RecVer };
         case VvSQL.SorterType.Ticker: return new object[] { this.Ticker,              RecVer };
         case VvSQL.SorterType.Person: return new object[] { this.Prezime, this.KupdobCD/*RecID*/, RecVer };
         case VvSQL.SorterType.OIB   : return new object[] { this.Oib,     this.KupdobCD/*RecID*/, RecVer };
         case VvSQL.SorterType.City  : return new object[] { this.Grad,    this.KupdobCD/*RecID*/, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion SorterCurrVal

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Kupdob();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Kupdob)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Kupdob)vvDataRecord).currentData;
   }


   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Kupdob>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Kupdob>(fileName);
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


   internal string GenerateTicker(int numOfCdDigits)
   {
      int maxNamePartLen = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ticker) - numOfCdDigits;
      
      string namePart;

      namePart = this.Naziv.Length < maxNamePartLen ? this.Naziv.ToUpper() : this.Naziv.ToUpper().Substring(0, maxNamePartLen);
      
      string codeAsStr = this.KupdobCD.ToString().PadLeft(numOfCdDigits, '0');

      string codePart = codeAsStr.Substring(codeAsStr.Length - numOfCdDigits, numOfCdDigits);

      return namePart + codePart;
   }

   internal static void FillLookUpItemZiroList(Kupdob kupdob_rec)
   {
      ZXC.luiListaRiskZiroRn.Clear();

      if(kupdob_rec == null) return;

      //27.05.2013.IBAN
      if(kupdob_rec.Ziro1.NotEmpty()) ZXC.luiListaRiskZiroRn.Add(new VvLookUpItem(/**/ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1), kupdob_rec.Ziro1By));
      if(kupdob_rec.Ziro2.NotEmpty()) ZXC.luiListaRiskZiroRn.Add(new VvLookUpItem(/**/ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro2), kupdob_rec.Ziro2By));
      if(kupdob_rec.Ziro3.NotEmpty()) ZXC.luiListaRiskZiroRn.Add(new VvLookUpItem(/**/ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro3), kupdob_rec.Ziro3By));
      if(kupdob_rec.Ziro4.NotEmpty()) ZXC.luiListaRiskZiroRn.Add(new VvLookUpItem(/**/ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro4), kupdob_rec.Ziro4By));
   }

   internal static Kupdob SetKupdobForVirmanFromFaktur(Faktur faktur_rec)
   {
      Kupdob kupdob_rec = new Kupdob();

      kupdob_rec.Naziv  = faktur_rec.KupdobName;
      //artikl_rec.Ulica1 = mixer_rec.KdAdresa;
      kupdob_rec.Ulica1 = faktur_rec.KdUlica;
      kupdob_rec.Grad   = faktur_rec.KdMjesto;
      kupdob_rec.Ziro1  = ZXC.GetIBANfromOldZiro(faktur_rec.ZiroRn);

      return kupdob_rec;
   }

   internal static Kupdob GetKupdobFromSkladCD(string skladCD)
   {
      uint skladKupdobCD = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(skladCD);

      Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == skladKupdobCD);

      if(kupdob_rec == null) kupdob_rec = new Kupdob();

      return kupdob_rec;
   }

   internal static Kupdob GetKupdobFromSkladCD_TEXTHO(string skladCD)
   {
      uint THskladCDroot = ZXC.ValOrZero_UInt(skladCD.SubstringSafe(0, 2));

      // centrala 
      if(THskladCDroot == ZXC.vvDB_ServerID_CENTRALA) return VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == 120012); // 120012 je KupdobCD centrale na Krugama u partnerima 

      // shopovi 
      Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.KupdobCD == THskladCDroot);

      if(kupdob_rec == null) kupdob_rec = new Kupdob();

      return kupdob_rec;
   }

   internal TimeSpan HHMM_OD(int dayOfWeek) // dayOfWeek prema DateTime.DayOfWeek gdje kad casting from DOW to int: 0 - NED, 1 - PON, ... 7 - SUB 
   {
      switch(dayOfWeek)
      {
         case 0: return this.TimeOd_7;
         case 1: return this.TimeOd_1;
         case 2: return this.TimeOd_2;
         case 3: return this.TimeOd_3;
         case 4: return this.TimeOd_4;
         case 5: return this.TimeOd_5;
         case 6: return this.TimeOd_6;

         default: return new TimeSpan(0);
      }
   }

   internal TimeSpan HHMM_DO(int dayOfWeek) // dayOfWeek prema DateTime.DayOfWeek gdje kad casting from DOW to int: 0 - NED, 1 - PON, ... 7 - SUB 
   {
      switch(dayOfWeek)
      {
         case 0: return this.TimeDo_7;
         case 1: return this.TimeDo_1;
         case 2: return this.TimeDo_2;
         case 3: return this.TimeDo_3;
         case 4: return this.TimeDo_4;
         case 5: return this.TimeDo_5;
         case 6: return this.TimeDo_6;

         default: return new TimeSpan(0);
      }
   }

   public const string PTG_NacSlanja_eMail  = "na mail";
   public const string PTG_NacSlanja_eRacun = "eRacun" ;
   public const string PTG_NacSlanja_posta  = "poštom" ;

   internal bool IsPTG_NacSlanja_eMail  { get { return this.Fuse2 == PTG_NacSlanja_eMail ; } }
   internal bool IsPTG_NacSlanja_eRacun { get { return this.Fuse2 == PTG_NacSlanja_eRacun; } }
   internal bool IsPTG_NacSlanja_posta  { get { return this.Fuse2 == PTG_NacSlanja_posta ; } }
}
