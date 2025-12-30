using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct PrjktStruct

public struct PrjktExtensionStruct
{
   internal string   _sudCity;
   internal string   _mbsRbu;
   internal DateTime _dateOsn;
   internal char     _placaTip;
   internal decimal  _toPayA;  
   internal decimal  _toPayB;  
   internal decimal  _toPayC;  
   internal decimal  _temKapit;
   internal decimal  _pidSume; 
   internal decimal  _pidSRent;
   internal decimal  _pidTurst;
   internal decimal  _pidDobit;
   internal decimal  _pidKmDopr;
   internal decimal  _pidKmClan;
   internal decimal  _pidMO1;  
   internal decimal  _pidMO2;  
   internal decimal  _pidZdr;  
   internal decimal  _pidZor;
   internal bool     _isSkip;  
   internal bool     _isOver20;
   internal bool     _isZorNa; 
   internal bool     _isJednK; 
   internal bool     _isDobit; 
   internal bool     _isAuthn; 
   internal bool     _isArhiv; // UrSkipKolStSkl 
   internal bool     _isTrgRS; // IrSkipKolStSkl 
   internal string   _memoHeader; 
   internal string   _memoFooter; 
   internal byte[]   _theLogo;   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 

   internal string   _belowGrid;
 //internal bool     _isNoMinus; 
   internal ushort   _isNoMinus; 
   internal string   _porIspost; 
   internal bool     _isChkPrKol;
   internal DateTime _rvrOd    ;
   internal DateTime _rvrDo    ;
   internal bool     _isFiskalOnline;
   internal byte[]   _certFile;    // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   internal string   _certPasswd;    
   internal bool     _isNoTtNumChk;
   internal bool     _isFiskCashOnly;
   internal string   _fiskTtOnly;
   internal bool     _isNeprofit;

   internal string   _skySrvrHost;
   internal string   _skyPassword;
   internal string   _skyVvDomena;
   internal decimal  _vrKoefBr1  ;
   
 //internal decimal  _stStz2029     ; // 
 //internal decimal  _stStz3034     ; // 
 //internal decimal  _stStz3500     ; // 
   internal bool     _isObustOver3  ; // 
   internal bool     _isCheckStaz   ; // provjeravaj staz (npr Josavac, ZarPtica, ...)
   internal bool     _isObrStazaLast; // 
   internal bool     _isSkipStzOnBol; // 
   internal bool     _isFullStzOnPol; // 
   internal byte[]   _theLogo2;   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 

   internal string _rnoRkp    ;
   internal bool   _shouldPeriodLock;
   internal uint   _periodLockDay   ;

   internal string _eSgnCertPasswd;
   internal byte[] _eSgnCertFile  ; // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 

   internal bool fiskalCertifikatLoadedFromBLOB; // NON data layer! 
   internal bool eSgnCertifikatLoadedFromBLOB  ; // NON data layer! 

   internal bool _isBtchBookg;

   internal string _memoFooter2;
   internal bool   _isNoAutoisFiskal;

   internal string _m2pShaSec;
   internal string _m2pApikey;
   internal string _m2pSerno ;
   internal string _m2pModel ;

   internal ushort _f2_Provider;
   internal ushort _f2_RolaKind;

   internal string _f2_UserName;
   internal string _f2_Password;

}

#endregion struct PrjktStruct

public class Prjkt : Kupdob
{

   #region Fildz

   new public const string recordName = "prjkt";
   new public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private PrjktExtensionStruct currentExtData;
   private PrjktExtensionStruct backupExtData;

   #endregion Fildz

   #region Constructors

   public Prjkt() : this(0)
   {
   }

   public Prjkt(uint ID) : base(ID) 
   {
      this.currentExtData = new PrjktExtensionStruct();

      this.currentExtData._sudCity  = "";
      this.currentExtData._mbsRbu   = "";
      this.currentExtData._dateOsn  = DateTime.MinValue;
      //this.currentExtData._placaTip = ' ';
      this.currentExtData._placaTip = char.MinValue; // Mozda ovako treba inicializirati sve CHAR(1)?!!!, bez ovoga si dobivao IndexOutOfRangeException kod GetChar u FillFromDataReader 
      this.currentExtData._toPayA   = decimal.Zero;  
      this.currentExtData._toPayB   = decimal.Zero;  
      this.currentExtData._toPayC   = decimal.Zero;  
      this.currentExtData._temKapit = decimal.Zero;
      this.currentExtData._pidSume  = decimal.Zero; 
      this.currentExtData._pidSRent = decimal.Zero;
      this.currentExtData._pidTurst = decimal.Zero;
      this.currentExtData._pidDobit = decimal.Zero;
      this.currentExtData._pidKmDopr = decimal.Zero;
      this.currentExtData._pidKmClan = decimal.Zero;
      this.currentExtData._pidMO1   = decimal.Zero;  
      this.currentExtData._pidMO2   = decimal.Zero;  
      this.currentExtData._pidZdr   = decimal.Zero;  
      this.currentExtData._pidZor   = decimal.Zero;
      this.currentExtData._isSkip   = false;
      this.currentExtData._isOver20 = false;
      this.currentExtData._isZorNa  = false; 
      this.currentExtData._isJednK  = false; 
      this.currentExtData._isDobit  = false; 
      this.currentExtData._isAuthn  = false; 
      this.currentExtData._isArhiv  = false;
      this.currentExtData._isTrgRS  = false;
      this.currentExtData._memoHeader = "";
      this.currentExtData._memoFooter = "";
      this.currentExtData._memoFooter2 = "";
      this.currentExtData._theLogo    = null;
      this.currentExtData._belowGrid  = "";
    //this.currentExtData._isNoMinus = false;
      this.currentExtData._isNoMinus = 0;
      this.currentExtData._porIspost = "";
      this.currentExtData._isChkPrKol= false;
      this.currentExtData._rvrOd     = DateTime.MinValue;
      this.currentExtData._rvrDo     = DateTime.MinValue;

      this.currentExtData._isFiskalOnline = false;
      this.currentExtData._certFile       = null;
      this.currentExtData._certPasswd     = "";
      this.currentExtData._isNoTtNumChk   = false;
      this.currentExtData._isFiskCashOnly = false;
      this.currentExtData._fiskTtOnly     = "";
      this.currentExtData._isNeprofit     = false;
      this.currentExtData._skySrvrHost    = "";
      this.currentExtData._skyPassword    = "";
      this.currentExtData._skyVvDomena    = "";
      this.currentExtData._vrKoefBr1      = 0M;

    //this.currentExtData._stStz2029      = 0M;   
    //this.currentExtData._stStz3034      = 0M;   
    //this.currentExtData._stStz3500      = 0M;   
      this.currentExtData._isObustOver3   = false;
      this.currentExtData._isCheckStaz    = false;
      this.currentExtData._isObrStazaLast = false;
      this.currentExtData._isSkipStzOnBol = false;
      this.currentExtData._isFullStzOnPol = false;
      this.currentExtData._theLogo2       = null;
      this.currentExtData._rnoRkp         = "";

      this.currentExtData._shouldPeriodLock = false;
      this.currentExtData._periodLockDay    = 0;

      this.currentExtData._eSgnCertFile       = null;
      this.currentExtData._eSgnCertPasswd     = "";

      this.currentExtData._isBtchBookg        = false;
      this.currentExtData._isNoAutoisFiskal     = false;

      this.currentExtData._m2pShaSec = "";
      this.currentExtData._m2pApikey = "";
      this.currentExtData._m2pSerno  = "";
      this.currentExtData._m2pModel  = "";

      this.currentExtData._f2_Provider = 0;
      this.currentExtData._f2_RolaKind = 0;
      this.currentExtData._f2_UserName = "";
      this.currentExtData._f2_Password = "";

      sorterKCD = Kupdob.sorterKCD;
      sorterCity    = Kupdob.sorterCity;
      sorterOIB     = Kupdob.sorterOIB;
      sorterNaziv   = Kupdob.sorterNaziv;
      sorterPrezime = Kupdob.sorterPrezime;
      sorterTicker  = Kupdob.sorterTicker;

      sorterKCD    .RecName = Prjkt.recordName;
      sorterOIB    .RecName = Prjkt.recordName;
      sorterNaziv  .RecName = Prjkt.recordName;
      sorterPrezime.RecName = Prjkt.recordName;
      sorterTicker .RecName = Prjkt.recordName;
      sorterCity   .RecName = Prjkt.recordName;

      sorterKCD    .RecNameArhiva = Prjkt.recordNameArhiva;
      sorterOIB    .RecNameArhiva = Prjkt.recordNameArhiva;
      sorterNaziv  .RecNameArhiva = Prjkt.recordNameArhiva;
      sorterPrezime.RecNameArhiva = Prjkt.recordNameArhiva;
      sorterTicker .RecNameArhiva = Prjkt.recordNameArhiva;
      sorterCity   .RecNameArhiva = Prjkt.recordNameArhiva;
   }

   #endregion Constructors

   #region propertiz 

   internal PrjktExtensionStruct CurrentExtData // cijela PrjktStruct struct-ura 
   {
      get { return this.currentExtData; }
      set {        this.currentExtData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.PrjktDao; }
   }

   public override string VirtualRecordName
   {
      get { return Prjkt.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Prjkt.recordNameArhiva; }
   }

   private List<Prvlg> _privileges;
   /// <summary>
   /// Gets or sets a list of privileges (line items) for this projekt.
   /// </summary>
   public List<Prvlg> Privileges
   {
      get { return _privileges; }
      set {        _privileges = value; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "pr"; }
   }

   public override bool IsPrjkt_NonPUG_DataRecord { get { return (true); } }

   //===================================================================
   //===================================================================
   //===================================================================



   public string SudCity
   {
      get { return this.currentExtData._sudCity; }
      set { this.currentExtData._sudCity = value; }
   }

   public string MbsRbu
   {
      get { return this.currentExtData._mbsRbu; }
      set { this.currentExtData._mbsRbu = value; }
   }

   public DateTime DateOsn
   {
      get { return this.currentExtData._dateOsn; }
      set { this.currentExtData._dateOsn = value; }
   }

   public char PlacaTip
   {
      get { return this.currentExtData._placaTip; }
      set { this.currentExtData._placaTip = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal ToPayA
   {
      get { return this.currentExtData._toPayA; }
      set { this.currentExtData._toPayA = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal ToPayB
   {
      get { return this.currentExtData._toPayB; }
      set { this.currentExtData._toPayB = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal ToPayC
   {
      get { return this.currentExtData._toPayC; }
      set { this.currentExtData._toPayC = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal TemKapit
   {
      get { return this.currentExtData._temKapit; }
      set { this.currentExtData._temKapit = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidSume
   {
      get { return this.currentExtData._pidSume; }
      set { this.currentExtData._pidSume = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidSRent
   {
      get { return this.currentExtData._pidSRent; }
      set { this.currentExtData._pidSRent = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidTurst
   {
      get { return this.currentExtData._pidTurst; }
      set { this.currentExtData._pidTurst = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidDobit
   {
      get { return this.currentExtData._pidDobit; }
      set { this.currentExtData._pidDobit = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidKmDopr
   {
      get { return this.currentExtData._pidKmDopr; }
      set { this.currentExtData._pidKmDopr = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidKmClan
   {
      get { return this.currentExtData._pidKmClan; }
      set { this.currentExtData._pidKmClan = value; }
   }
   
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidMO1 
   {
      get { return this.currentExtData._pidMO1; }
      set { this.currentExtData._pidMO1 = value; }
   }
   
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidMO2 
   {
      get { return this.currentExtData._pidMO2; }
      set { this.currentExtData._pidMO2 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidZdr
   {
      get { return this.currentExtData._pidZdr; }
      set { this.currentExtData._pidZdr = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal PidZor
   {
      get { return this.currentExtData._pidZor; }
      set { this.currentExtData._pidZor = value; }
   }

   public bool IsSkip
   {
      get { return this.currentExtData._isSkip; }
      set { this.currentExtData._isSkip = value; }
   }

   //public bool IsSkip_asB
   //{
   //   get
   //   {
   //      if (this.IsSkip == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsSkip = 0;
   //      else this.IsSkip = 1;
   //   }
   //}

   public bool IsOver20
   {
      get { return this.currentExtData._isOver20; }
      set { this.currentExtData._isOver20 = value; }
   }

   //public bool IsOver20_asB
   //{
   //   get
   //   {
   //      if (this.IsOver20 == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsOver20 = 0;
   //      else this.IsOver20 = 1;
   //   }
   //}

   public bool IsZorNa
   {
      get { return this.currentExtData._isZorNa; }
      set { this.currentExtData._isZorNa = value; }
   }
   //public bool IsZorNa_asB
   //{
   //   get
   //   {
   //      if (this.IsZorNa == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsZorNa = 0;
   //      else this.IsZorNa = 1;
   //   }
   //}

   public bool IsJednK
   {
      get { return this.currentExtData._isJednK; }
      set { this.currentExtData._isJednK = value; }
   }
   //public bool IsJednK_asB
   //{
   //   get
   //   {
   //      if (this.IsJednK == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsJednK = 0;
   //      else this.IsJednK = 1;
   //   }
   //}

   public bool IsDobit
   {
      get { return this.currentExtData._isDobit; }
      set { this.currentExtData._isDobit = value; }
   }
   //public bool IsDobit_asB
   //{
   //   get
   //   {
   //      if (this.IsDobit == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsDobit = 0;
   //      else this.IsDobit = 1;
   //   }
   //}

   public bool IsAuthn
   {
      get { return this.currentExtData._isAuthn; }
      set { this.currentExtData._isAuthn = value; }
   }
   //public bool IsAuthn_asB
   //{
   //   get
   //   {
   //      if (this.IsAuthn == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsAuthn = 0;
   //      else this.IsAuthn = 1;
   //   }
   //}

   // Bivsi 'IsArhiv'
   /// <summary>
   /// Ako je check-iran, URn NE utjece na ArtStat.KolSt_Free,
   /// a defaultno je neCheck-iran sto znaci da URn istovremeno dira i KolSt i KolSt_Free
   /// </summary>
   public bool UrSkipKolStSkl
   {
      get { return this.currentExtData._isArhiv; }
      set { this.currentExtData._isArhiv = value; }
   }
   //public bool IsArhiv_asB
   //{
   //   get
   //   {
   //      if (this.IsArhiv == 0) return false;
   //      else return true;
   //   }
   //   set
   //   {
   //      if (value == false) this.IsArhiv = 0;
   //      else this.IsArhiv = 1;
   //   }
   //}

   // Bivsi 'IsTrgRs' 
   /// <summary>
   /// Ako je check-iran, IRn NE utjece na ArtStat.KolSt_Free,
   /// a defaultno je neCheck-iran sto znaci da IRn istovremeno dira i KolSt i KolSt_Free
   /// </summary>
   public bool IrSkipKolStSkl
   {
      get { return this.currentExtData._isTrgRS; }
      set { this.currentExtData._isTrgRS = value; }
   }

   public string MemoHeader
   {
      get { return this.currentExtData._memoHeader; }
      set { this.currentExtData._memoHeader = value; }
   }

   public string MemoFooter
   {
      get { return this.currentExtData._memoFooter; }
      set { this.currentExtData._memoFooter = value; }
   }

   public string MemoFooter2
   {
      get { return this.currentExtData._memoFooter2; }
      set { this.currentExtData._memoFooter2 = value; }
   }

   public bool IsNoAutoFiskal
   {
      get { return this.currentExtData._isNoAutoisFiskal; }
      set { this.currentExtData._isNoAutoisFiskal = value; }
   }


   /// <summary>
   /// Decryptiran, decoded, desifrirani M2PshaSec
   /// </summary>
   public string M2PshaSecDecrypted
   {
      get
      {
       // 13.11.2014: odjedamput za ove Sky-eve propertye SkySrvrHostDecrypted i SkyPasswordDecrypted ovaj 
       // VvAES.DecryptData na kraju stringa nalijepi '\0' nekoliko puta npr. "192.168.0.1\0\0\0" 
       //return (currentExtData._m2pShaSec.NotEmpty()) ? VvAES.DecryptData(currentExtData._m2pShaSec, ZXC.vv_Login_AES_key)                   : "";
         return (currentExtData._m2pShaSec.NotEmpty()) ? VvAES.DecryptData(currentExtData._m2pShaSec, ZXC.vv_Login_AES_key).Replace("\0", "") : "";
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }

   public string M2PshaSecEncodedAsInFile { get { return this.currentExtData._m2pShaSec; } set { this.currentExtData._m2pShaSec = value; } }

   /// <summary>
   /// Decryptiran, decoded, desifrirani M2Papikey
   /// </summary>
   public string M2PapikeyDecrypted
   {
      get
      {
       // 13.11.2014: odjedamput za ove Sky-eve propertye SkySrvrHostDecrypted i SkyPasswordDecrypted ovaj 
       // VvAES.DecryptData na kraju stringa nalijepi '\0' nekoliko puta npr. "192.168.0.1\0\0\0" 
       //return (currentExtData._m2pApikey.NotEmpty()) ? VvAES.DecryptData(currentExtData._m2pApikey, ZXC.vv_Login_AES_key)                   : "";
         return (currentExtData._m2pApikey.NotEmpty()) ? VvAES.DecryptData(currentExtData._m2pApikey, ZXC.vv_Login_AES_key).Replace("\0", "") : "";
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }

   public string M2PapikeyEncodedAsInFile { get { return this.currentExtData._m2pApikey; } set { this.currentExtData._m2pApikey = value; } }
   public string M2Pserno  { get { return this.currentExtData._m2pSerno ; } set { this.currentExtData._m2pSerno  = value; } }
   public string M2Pmodel  { get { return this.currentExtData._m2pModel ; } set { this.currentExtData._m2pModel  = value; } }

   #region F2 - FIR / FUR ovo ono
   public ZXC.F2_Provider_enum F2_Provider { get { return (ZXC.F2_Provider_enum)this.currentExtData._f2_Provider; } set { this.currentExtData._f2_Provider = (ushort)value; } }
   public ZXC.F2_RolaKind      F2_RolaKind { get { return (ZXC.F2_RolaKind     )this.currentExtData._f2_RolaKind; } set { this.currentExtData._f2_RolaKind = (ushort)value; } }

   public bool F2_RolaKindDodajeF2IzlazRn  { get { return F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL && F2_Provider != ZXC.F2_Provider_enum.UNKNOWN; } } // NE Veleform, NE TH Cent., NE Senso Micro 
   public bool F2_Ima_F2_B2B               { get { return F2_RolaKindDodajeF2IzlazRn == true                                                                        ; } } // Just ALIAS                           
   public bool F2_IsServis                 { get { return F2_RolaKind != ZXC.F2_RolaKind.NEMA_F2 && F2_RolaKind != ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL     ; } } // Veleform (MER), Ljekarna Mamiæ (PND), ... 
   public bool F2_IsSebi                   { get { return                                           F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL     ; } } // Frigoterm, Metaflex, Panigale,        ... 
   public bool F2_Ima_F1_B2C               { get { return IsFiskalOnline                                                                                            ; } } // Textho,    Metaflex, Panigale,        ... 
   public bool F2_ImaSamoB2C               { get { return F2_Ima_F1_B2C && !F2_Ima_F2_B2B                                                                           ; } } // Textho                                ... 
   public bool F2_ImaSamo_F2_B2B           { get { return F2_Ima_F2_B2B && !F2_Ima_F1_B2C                                                                           ; } } // Frigoterm, Viper                      ... 
   public bool F2_ImaF1_B2C_i_F2_B2B       { get { return F2_Ima_F1_B2C && F2_Ima_F2_B2B                                                                            ; } } //            Metaflex, Panigale,        ... 
   public bool F2_NEma_ni_B2C_ni_B2B       { get { return F2_Ima_F1_B2C == false && F2_Ima_F2_B2B == false                                                          ; } } // HZTK, TZGML?                          ... 
   public bool F2_NEma_ni_F1_ni_F2         { get { return IsFiskalOnline == false && F2_RolaKind == ZXC.F2_RolaKind.NEMA_F2                                         ; } } // HZTK, TZGML?                          ... 
   public bool F2_IsKlijentServisaNaMERu   { get { return F2_IsServis && F2_Provider == ZXC.F2_Provider_enum.MER                                                    ; } } // Veleform                              ... 
   public bool F2_IsKlijentServisaNE_NaMERu{ get { return F2_IsServis && F2_Provider != ZXC.F2_Provider_enum.MER                                                    ; } } // Ljekarna Mamiæ (PND), Senso Micro     ... 

   public string F2_UserName               { get { return this.currentExtData._f2_UserName; } set { this.currentExtData._f2_UserName = value; } }
   public string F2_PasswordEncodedAsInFile{ get { return this.currentExtData._f2_Password; } set { this.currentExtData._f2_Password = value; } }
   public string F2_PasswordDecrypted
   {
      get
      {
         return (currentExtData._f2_Password.NotEmpty()) ? VvAES.DecryptData(currentExtData._f2_Password, ZXC.vv_Login_AES_key).Replace("\0", "") : "";
      }
   }

   // ovo je TODO property jer nez jos za sto cemo ga koristiti pa da postavimo bool 
   public bool F2_IsF2_FromVektor
   { 
      get 
      { 
         return 
            
            F2_Provider != ZXC.F2_Provider_enum.UNKNOWN && 
            F2_RolaKind != ZXC.F2_RolaKind.NEMA_F2 
            ; 
      } 
   } 

   #endregion F2 - FIR / FUR ovo ono

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public byte[] TheLogo
   {
      get { return this.currentExtData._theLogo;         }
      set {        this.currentExtData._theLogo = value; }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public byte[] TheLogo2
   {
      get { return this.currentExtData._theLogo2; }
      set { this.currentExtData._theLogo2 = value; }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public System.Drawing.Image TheLogoImage
   {
      get
      {
         if(TheLogo == null) return null;

         System.Drawing.Image returnImage = null;

         using(System.IO.MemoryStream ms = new System.IO.MemoryStream(TheLogo))
         {
            returnImage = System.Drawing.Image.FromStream(ms);
         }

         return returnImage;
      }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public System.Drawing.Image TheLogoImage2
   {
      get
      {
         if(TheLogo2 == null) return null;

         System.Drawing.Image returnImage = null;

         using(System.IO.MemoryStream ms = new System.IO.MemoryStream(TheLogo2))
         {
            returnImage = System.Drawing.Image.FromStream(ms);
         }

         return returnImage;
      }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public byte[] CertFile
   {
      get { return this.currentExtData._certFile;         }
      set {        this.currentExtData._certFile = value; }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public byte[] ESgnCertFile
   {
      get { return this.currentExtData._eSgnCertFile;         }
      set {        this.currentExtData._eSgnCertFile = value; }
   }

   // NE ZABORAVI u VvDaoBase.WHERE_or_SET_Clause_Specifics 'if(colName == "theLogo") continue;' za svaki BLOB / MEDIUM BLOB 
   public string BelowGrid
   {
      get { return this.currentExtData._belowGrid; }
      set { this.currentExtData._belowGrid = value; }
   }

   public /*bool*/ ushort IsNoMinus
   {
      get { return this.currentExtData._isNoMinus; }
      set { this.currentExtData._isNoMinus = value; }
   }
   public ZXC.MinusPolicy MinusPolicy
   {
      get { return (ZXC.MinusPolicy)this.currentExtData._isNoMinus; }
      set { this.currentExtData._isNoMinus = (ushort)value; }
   }

   public bool IsUnacceptableMinus(bool isMalopTT)
   {
      switch(this.MinusPolicy)
      {
         case ZXC.MinusPolicy.ALLOW_ALL:          return false;
         case ZXC.MinusPolicy.ALOW_ALL_NO_MSG:    return false;
         case ZXC.MinusPolicy.DENY_ALL :          return true ;
         case ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL: return (isMalopTT ? false : true);
      }

      return false;
   }

   public string PorezIspostCD
   {
      get { return this.currentExtData._porIspost; }
      set { this.currentExtData._porIspost = value; }
   }

   public string PorezIspostName { get { return ZXC.luiListaIspostava.GetNameForThisCd(this.PorezIspostCD); } }

   public bool NoNeedFor_WRN_UFRA { get { return (this.PdvRTip != ZXC.PdvRTipEnum.POD_PO_NAPL && this.PdvRTip != ZXC.PdvRTipEnum.OBRT_R2); } }

   public bool IsChkPrKol
   {
      get { return this.currentExtData._isChkPrKol; }
      set { this.currentExtData._isChkPrKol = value; }
   }

   public DateTime RvrOd
   {
      get { return this.currentExtData._rvrOd; }
      set { this.currentExtData._rvrOd = value; }
   }

   public DateTime RvrDo
   {
      get { return this.currentExtData._rvrDo; }
      set { this.currentExtData._rvrDo = value; }
   }

   public bool IsFiskalOnline
   {
      get { return this.currentExtData._isFiskalOnline; }
      set { this.currentExtData._isFiskalOnline = value; }
   }

   //public string CertPasswd
   //{
   //   get { return this.currentExtData._certPasswd; }
   //   set { this.currentExtData._certPasswd = value; }
   //}

   /// <summary>
   /// Cryptiran, encoded, sifrirani password
   /// </summary>
   public string CertPasswdEncodedAsInFile
   {
      get { return this.currentExtData._certPasswd; }
      set { this.currentExtData._certPasswd = value; }
   }

   /// <summary>
   /// Decryptiran, decoded, desifrirani password
   /// </summary>
   public string CertPasswdDecrypted
   {
      get 
      {
         return (currentExtData._certPasswd.NotEmpty()) ? VvAES.DecryptData(currentExtData._certPasswd, ZXC.vv_Login_AES_key) : ""; 
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }

   /// <summary>
   /// Cryptiran, encoded, sifrirani password
   /// </summary>
   public string ESgnCertPasswdEncodedAsInFile
   {
      get { return this.currentExtData._eSgnCertPasswd; }
      set { this.currentExtData._eSgnCertPasswd = value; }
   }

   /// <summary>
   /// Decryptiran, decoded, desifrirani password
   /// </summary>
   public string ESgnCertPasswdDecrypted
   {
      get 
      {
         return (currentExtData._eSgnCertPasswd.NotEmpty()) ? VvAES.DecryptData(currentExtData._eSgnCertPasswd, ZXC.vv_Login_AES_key) : ""; 
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }


   /// <summary>
   /// Cryptiran, encoded, sifrirani password
   /// </summary>
   public string SkyPasswordEncodedAsInFile
   {
      get { return this.currentExtData._skyPassword; }
      set { this.currentExtData._skyPassword = value; }
   }

   /// <summary>
   /// Decryptiran, decoded, desifrirani password
   /// </summary>
   public string SkyPasswordDecrypted
   {
      get
      {
         // 13.11.2014: odjedamput za ove Sky-eve propertye SkySrvrHostDecrypted i SkyPasswordDecrypted ovaj 
         // VvAES.DecryptData na kraju stringa nalijepi '\0' nekoliko puta npr. "192.168.0.1\0\0\0" 
       //return (currentExtData._skyPassword.NotEmpty()) ? VvAES.DecryptData(currentExtData._skyPassword, ZXC.vv_Login_AES_key)                   : "";
         return (currentExtData._skyPassword.NotEmpty()) ? VvAES.DecryptData(currentExtData._skyPassword, ZXC.vv_Login_AES_key).Replace("\0", "") : "";
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }

   /// <summary>
   /// Cryptiran, encoded, sifrirani SrvrHost
   /// </summary>
   public string SkySrvrHostEncodedAsInFile
   {
      get { return this.currentExtData._skySrvrHost; }
      set { this.currentExtData._skySrvrHost = value; }
   }

   /// <summary>
   /// Decryptiran, decoded, desifrirani SrvrHost
   /// </summary>
   public string SkySrvrHostDecrypted
   {
      get
      {
         // 13.11.2014: odjedamput za ove Sky-eve propertye SkySrvrHostDecrypted i SkyPasswordDecrypted ovaj 
         // VvAES.DecryptData na kraju stringa nalijepi '\0' nekoliko puta npr. "192.168.0.1\0\0\0" 
       //return (currentExtData._skySrvrHost.NotEmpty()) ? VvAES.DecryptData(currentExtData._skySrvrHost, ZXC.vv_Login_AES_key)                   : "";
         return (currentExtData._skySrvrHost.NotEmpty()) ? VvAES.DecryptData(currentExtData._skySrvrHost, ZXC.vv_Login_AES_key).Replace("\0", "") : "";
      }
      //set 
      //{ 
      //   this.currentData._passwd = ZXC.NotEmpty(value) ? VvAES.EncryptData(value, ZXC.vv_AES_key) : ""; 
      //}
   }
   
   
   public bool IsNoTtNumChk
   {
      get { return this.currentExtData._isNoTtNumChk; }
      set { this.currentExtData._isNoTtNumChk = value; }
   }

   public bool IsFiskCashOnly
   {
      get 
      {
         if(ZXC.projectYearAsInt > 2025) return false; //od 2026. B2C svi idu u F1 pa i transakcijski NP 

         return this.currentExtData._isFiskCashOnly; 
      }
      set { this.currentExtData._isFiskCashOnly = value; }
   }

   public string FiskTtOnly
   {
      get { return this.currentExtData._fiskTtOnly; }
      set { this.currentExtData._fiskTtOnly = value; }
   }

   public bool IsNeprofit
   {
      get { return this.currentExtData._isNeprofit; }
      set { this.currentExtData._isNeprofit = value; }
   }

 //public string SkySrvrHost { get { return this.currentExtData._skySrvrHost; } set { this.currentExtData._skySrvrHost = value; } }
 //public string SkyPassword { get { return this.currentExtData._skyPassword; } set { this.currentExtData._skyPassword = value; } }
   public string SkyVvDomena { get { return this.currentExtData._skyVvDomena; } set { this.currentExtData._skyVvDomena = value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal VrKoefBr1  { get { return this.currentExtData._vrKoefBr1  ; } set { this.currentExtData._vrKoefBr1   = value; } }

 //public decimal StStz2029       { get { return this.currentExtData._stStz2029      ; } set { this.currentExtData._stStz2029      = value; } }     
 //public decimal StStz3034       { get { return this.currentExtData._stStz3034      ; } set { this.currentExtData._stStz3034      = value; } }
 //public decimal StStz3500       { get { return this.currentExtData._stStz3500      ; } set { this.currentExtData._stStz3500      = value; } }
   public bool    IsObustOver3    { get { return this.currentExtData._isObustOver3   ; } set { this.currentExtData._isObustOver3   = value; } }
   public bool    IsCheckStaz     { get { return this.currentExtData._isCheckStaz    ; } set { this.currentExtData._isCheckStaz    = value; } }
   public bool    IsObrStazaLast  { get { return this.currentExtData._isObrStazaLast ; } set { this.currentExtData._isObrStazaLast = value; } }
   public bool    IsSkipStzOnBol  { get { return this.currentExtData._isSkipStzOnBol ; } set { this.currentExtData._isSkipStzOnBol = value; } }
   public bool    IsFullStzOnPol  { get { return this.currentExtData._isFullStzOnPol ; } set { this.currentExtData._isFullStzOnPol = value; } }
   public string  RnoRkp          { get { return this.currentExtData._rnoRkp         ; } set { this.currentExtData._rnoRkp         = value; } }

 //public ZXC.PeriodLockKind PeriodLock { get { return ZXC.PeriodLockKind.LK_fromPrevMonth; } }
   public bool ShouldPeriodLock { get { return this.currentExtData._shouldPeriodLock ; } set { this.currentExtData._shouldPeriodLock= value; } }
   public uint PeriodLockDay    { get { return this.currentExtData._periodLockDay    ; } set { this.currentExtData._periodLockDay   = value; } }

   public bool IsBtchBookg
   {
      get { return this.currentExtData._isBtchBookg; }
      set { this.currentExtData._isBtchBookg = value; }
   }

   // TODO: as data layer 
   // 22.01.2018: 
 //public bool ShouldPrevYearLock_RISK { get { return true            ; } }
   // a od 12.12.2024 ubijamo i koristimo 'ShouldPeriodLock' 
   //public bool ShouldPrevYearLock_RISK { get { return ShouldPeriodLock; } } // Koristimo 'ShouldPeriodLock' za jedno i drugo 

   #endregion propertiz 

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<KupdobStruct>        (ref this.currentData,    ref this.backupData);
      Generic_RestoreBackupData<PrjktExtensionStruct>(ref this.currentExtData, ref this.backupExtData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<KupdobStruct>        (ref this.currentData,    ref this.backupData);

      this.editInProgress = false; // roll back for next line usage, inace ne obavi dolnji red. 

      Generic_BeginEdit<PrjktExtensionStruct>(ref this.currentExtData, ref this.backupExtData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<KupdobStruct>        (ref this.currentData,    ref this.backupData);

      this.editInProgress = true; // roll back for next line usage, inace ne obavi dolnji red. 
      
      Generic_CancelEdit<PrjktExtensionStruct>(ref this.currentExtData, ref this.backupExtData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<KupdobStruct>        (ref this.currentData,    ref this.backupData);

      this.editInProgress = true; // roll back for next line usage, inace ne obavi dolnji red. 

      Generic_EndEdit<PrjktExtensionStruct>(ref this.currentExtData, ref this.backupExtData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<KupdobStruct>        (this.currentData,    this.backupData) ||
             Generic_EditedHasChanges<PrjktExtensionStruct>(this.currentExtData, this.backupExtData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Prjkt newObject = new Prjkt();

      Generic_CloneData<KupdobStruct>        (this.currentData,    this.backupData,    ref newObject.currentData,    ref newObject.backupData);
      Generic_CloneData<PrjktExtensionStruct>(this.currentExtData, this.backupExtData, ref newObject.currentExtData, ref newObject.backupExtData);

      return newObject;
   }

   public new Prjkt MakeDeepCopy()
   {
      return (Prjkt)Clone();
   }

   #endregion

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Prjkt>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Prjkt>(fileName);
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


   // BE ADVICED!!! 
   // BE ADVICED!!! 
   // BE ADVICED!!! 
   //
   // VisualStudio (ili Vektor kao takav) NE RADE ISTO u DEBUG i RELEASE varijanti!!!!!!!!!!!???????? 
   // ako ostavis ove remarkirane assignmente dole aktivne, onda ti kod pokretanje RELEASE varijante skace 'TypeInitializerException'
   // docim kod pokretanja DEBUG varijante, Exceptionu niti traga niti glasa !!!!???? 

   public new static VvSQL.RecordSorter sorterKCD     ;//= Kupdob.sorterKCD;
   public new static VvSQL.RecordSorter sorterCity    ;//= Kupdob.sorterCity;
   public new static VvSQL.RecordSorter sorterOIB     ;//= Kupdob.sorterMatbr;
   public new static VvSQL.RecordSorter sorterNaziv   ;//= Kupdob.sorterNaziv;
   public new static VvSQL.RecordSorter sorterPrezime ;//= Kupdob.sorterPrezime;
   public new static VvSQL.RecordSorter sorterTicker  ;//= Kupdob.sorterTicker;

   // za RISK Report Kompenzacije 
   public string OtsSaldoKompenzacijeAsText { get; set; }

   public bool NOT_IN_PDV
   {
      get
      {
       //return this.PdvRTip == ZXC.PdvRTipEnum.NOT_IN_PDV; 19.04.2012.
         return ((this.PdvRTip == ZXC.PdvRTipEnum.NOT_IN_PDV || this.PdvRTip == ZXC.PdvRTipEnum.OBRT_NOT_PDV) ? true : false);
     }
   }
   public bool IS_IN_PDV
   {
      get
      {
         return !this.NOT_IN_PDV;
      }
   }

   #region Plan Kind

   public /*static*/ ZXC.PlanKindEnum PlanKind
   {
      get
      {
         ZXC.PlanKindEnum planKind = ZXC.PlanKindEnum.NO_PLAN;

         //KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);

         if(this.IsNeprofit == true && ZXC.KSD.Dsc_IsPlanViaMtros       == true) return ZXC.PlanKindEnum.PlnBy_MTROS   ; // HZTK   
         if(this.IsNeprofit == true && ZXC.KSD.Dsc_IsVisibleColPozicija == true) return ZXC.PlanKindEnum.PlnBy_POZICIJA; // TURZML 
         if(                           ZXC.KSD.Dsc_IsVisibleColFond     == true) return ZXC.PlanKindEnum.PlnBy_FOND    ; // KEREMP 

         return planKind;
      }
   }

   #endregion Plan Kind

   #region About BLOBs

   #region FiskalCertifikat

 //private bool fiskalCertifikatLoadedFromBLOB = false;
   public bool FiskalCertifikatLoadedFromBLOB // Memset0 se NE poziva npr na prethodni - sljedeci ali se 
                                              // PrjktExtensionStruct = new PrjktExtensionStruct() poziva uvijek
                                              // ... zato ti se ovaj bool 'fiskalCertifikatLoaded' ne nulira kako si ocekivao 
                                              // pa si morao dodati NON data layer bool u PrjktExtensionStruct 
   {
      get { return this.currentExtData.fiskalCertifikatLoadedFromBLOB; }
      set { this.currentExtData.fiskalCertifikatLoadedFromBLOB = value; }
   }
   private X509Certificate2 fiskalCertifikat;
   public  X509Certificate2 FiskalCertifikat
   {
      get 
      { 
     //if(FiskalCertifikatLoadedFromBLOB                            ) return fiskalCertifikat;
       if(FiskalCertifikatLoadedFromBLOB && fiskalCertifikat != null) return fiskalCertifikat;

       FiskalCertifikatLoadedFromBLOB = true;

         if(this.CertFile == null) fiskalCertifikat = null;

         else
         {
            fiskalCertifikat = null;

            try
            {
               // 31.01.2024: pokusaj ispravka Access Denied poruke 
              //fiskalCertifikat = new X509Certificate2(this.CertFile, this.CertPasswdDecrypted);
                fiskalCertifikat = new X509Certificate2(this.CertFile, this.CertPasswdDecrypted, X509KeyStorageFlags.MachineKeySet);
            }
            catch(System.Security.Cryptography.CryptographicException cEx)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, cEx.Message);
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
            }
         }

         return fiskalCertifikat;
      }
   }

   public string   FiskalCertifikat_Issuer  { get { return (FiskalCertifikat == null ? "" : FiskalCertifikat.IssuerName.Name          ); } }
   public string   FiskalCertifikat_Subject { get { return (FiskalCertifikat == null ? "" : FiskalCertifikat.Subject                  ); } }
   public string   FiskalCertifikat_Expire  { get { return (FiskalCertifikat == null ? "" : FiskalCertifikat.GetExpirationDateString()); } }
   public DateTime FiskalCertifikat_ExpireD { get { return (FiskalCertifikat == null ? DateTime.MinValue : FiskalCertifikat.NotAfter  ); } }

#if nijeovodobro
   public string   FiskalCertifikat_Issuer  
   { 
      get 
      {
         if(FiskalCertifikat == null) return "";

         try
         {
            return FiskalCertifikat.IssuerName.Name;
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "FiskalCertifikat.IssuerName.Name error\n\r\n\r{0}", ex.Message);
            ZXC.aim_emsg_VvException(ex);
            return "";
         }
      }
   }
   public string   FiskalCertifikat_Subject 
   { 
      get 
      {
         if(FiskalCertifikat == null) return "";

         try
         {
            return FiskalCertifikat.Subject;
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "FiskalCertifikat.Subject error\n\r\n\r{0}", ex.Message);
            ZXC.aim_emsg_VvException(ex);
            return "";
         }
      }
   }
   public string   FiskalCertifikat_Expire  
   { 
      get 
      {
         if(FiskalCertifikat == null) return "";

         return (FiskalCertifikat == null ? "" : FiskalCertifikat.GetExpirationDateString());
         try
         {
            return FiskalCertifikat.GetExpirationDateString();
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "FiskalCertifikat.GetExpirationDateString() error\n\r\n\r{0}", ex.Message);
            ZXC.aim_emsg_VvException(ex);
            return "";
         }
      }
   }
   public DateTime FiskalCertifikat_ExpireD 
   { 
      get 
      { 
         if(FiskalCertifikat == null) return DateTime.MinValue;
         try
         {
            return FiskalCertifikat.NotAfter;
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "FiskalCertifikat.NotAfter error\n\r\n\r{0}", ex.Message);
            ZXC.aim_emsg_VvException(ex);
            return DateTime.MinValue;
         }
      }
   }

#endif
   #endregion FiskalCertifikat

   #region ESgnCertifikat

   //private bool eSgnCertifikatLoadedFromBLOB = false;
   public bool eSgnCertifikatLoadedFromBLOB // Memset0 se NE poziva npr na prethodni - sljedeci ali se 
                                              // PrjktExtensionStruct = new PrjktExtensionStruct() poziva uvijek
                                              // ... zato ti se ovaj bool 'eSgnCertifikatLoaded' ne nulira kako si ocekivao 
                                              // pa si morao dodati NON data layer bool u PrjktExtensionStruct 
   {
      get { return this.currentExtData.eSgnCertifikatLoadedFromBLOB; }
      set { this.currentExtData.eSgnCertifikatLoadedFromBLOB = value; }
   }
   private X509Certificate2 eSgnCertifikat;
   public  X509Certificate2 ESgnCertifikat
   {
      get 
      { 
       if(eSgnCertifikatLoadedFromBLOB) return eSgnCertifikat;

       eSgnCertifikatLoadedFromBLOB = true;

         if(this.CertFile == null) eSgnCertifikat = null;

         else
         {
            eSgnCertifikat = null;

            try
            {
                eSgnCertifikat = new X509Certificate2(this.CertFile, this.CertPasswdDecrypted);
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
            }
         }

         return eSgnCertifikat;
      }
   }

   public string   ESgnCertifikat_Issuer  { get { return (ESgnCertifikat == null ? "" : eSgnCertifikat.IssuerName.Name          ); } }
   public string   ESgnCertifikat_Subject { get { return (ESgnCertifikat == null ? "" : eSgnCertifikat.Subject                  ); } }
   public string   ESgnCertifikat_Expire  { get { return (ESgnCertifikat == null ? "" : eSgnCertifikat.GetExpirationDateString()); } }

   #endregion ESgnCertifikat

   #endregion About BLOBs
}
