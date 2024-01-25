using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

#region struct FakturStruct & FaktExStruct

public struct FakturStruct
{
            /*internal*/ public uint     _recID     ;
            /*internal*/ public DateTime _addTS     ;
            /*internal*/ public DateTime _modTS     ;
            /*internal*/ public string   _addUID    ;
            /*internal*/ public string   _modUID    ;
            /*internal*/ public uint     _lanSrvID  ;
            /*internal*/ public uint     _lanRecID  ;
   /* 05 */ /*internal*/ public uint     _dokNum    ;
   /* 06 */ /*internal*/ public DateTime _dokDate   ;
   /* 07 */ /*internal*/ public string   _tt        ;
   /* 08 */ /*internal*/ public uint     _ttNum     ;
   /* 09 */ /*internal*/ public short    _ttSort    ;
   /* 10 */ /*internal*/ public string   _skladCD   ;
   /* 11 */ /*internal*/ public string   _skladCD2  ;
   /* 12 */ /*internal*/ public string   _vezniDok  ;
   /* 13 */ /*internal*/ public string   _napomena  ;
   /* 14 */ /*internal*/ public string   _opis      ;
   /* 15 */ /*internal*/ public string   _konto     ;
   /* 16 */ /*internal*/ public string   _projektCD ;
   /* 17 */ /*internal*/ public string   _v1_tt     ;
   /* 18 */ /*internal*/ public uint     _v1_ttNum  ;
   /* 19 */ /*internal*/ public string   _v2_tt     ;
   /* 20 */ /*internal*/ public uint     _v2_ttNum  ;
   /* 21 */ /*internal*/ public decimal  _s_ukKC    ;
   /* 22 */ /*internal*/ public decimal  _s_ukK     ;
   /* 23 */ /*internal*/ public uint     _s_trnCount;
   /* 24 */ /*internal*/ public string   _osobaX    ;
   /* 25 */ /*internal*/ public DateTime _dokDate2  ;
   /* 26 */ /*internal*/ public decimal  _s_ukK2    ;
   /* 27 */ /*internal*/ public decimal  _decimal01 ;
   /* 28 */ /*internal*/ public decimal  _decimal02 ;
}

public struct FaktExStruct
{
            /*internal*/ public uint     _recID        ;
   /* 01 */ /*internal*/ public uint     _fakturRecID  ;
   /* 02 */ /*internal*/ public uint     _pdvNum       ; // Fuse 
   /* 03 */ /*internal*/ public DateTime _pdvDate      ;
   /* 04 */ /*internal*/ public uint     _kupdobCD     ;
   /* 05 */ /*internal*/ public string   _kupdobTK     ;
   /* 06 */ /*internal*/ public string   _kupdobName   ;
   /* 07 */ /*internal*/ public string   _kdUlica      ;
   /* 08 */ /*internal*/ public string   _kdMjesto     ;
   /* 09 */ /*internal*/ public string   _kdZip        ;
   /* 10 */ /*internal*/ public string   _kdOib        ;
   /* 11 */ /*internal*/ public uint     _posJedCD     ;
   /* 12 */ /*internal*/ public string   _posJedTK     ;
   /* 13 */ /*internal*/ public string   _posJedName   ;
   /* 14 */ /*internal*/ public string   _posJedUlica  ;
   /* 15 */ /*internal*/ public string   _posJedMjesto ;
   /* 16 */ /*internal*/ public string   _posJedZip    ;
   /* 17 */ /*internal*/ public string   _vezniDok2    ;
   /* 18 */ /*internal*/ public string   _fco          ;
   /* 19 */ /*internal*/ public int      _rokPlac      ;
   /* 20 */ /*internal*/ public DateTime _dospDate     ;
   /* 21 */ /*internal*/ public DateTime _skladDate    ;
   /* 22 */ /*internal*/ public string   _nacPlac      ;
   /* 23 */ /*internal*/ public string   _ziroRn       ;
   /* 24 */ /*internal*/ public string   _devName      ;
   /* 25 */ /*internal*/ public string   _pnbM         ;
   /* 26 */ /*internal*/ public string   _pnbV         ;
   /* 27 */ /*internal*/ public uint     _personCD     ;
   /* 28 */ /*internal*/ public string   _personName   ;
   /* 29 */ /*internal*/ public string   _napomena2    ;
   /* 30 */ /*internal*/ public string   _cjenikTT     ;
   /* 31 */ /*internal*/ public string   _statusCD     ;
   /* 32 */ /*internal*/ public int      _rokPonude    ;
   /* 33 */ /*internal*/ public DateTime _ponudDate    ;
   /* 34 */ /*internal*/ public uint     _mtrosCD      ;
   /* 35 */ /*internal*/ public string   _mtrosTK      ;
   /* 36 */ /*internal*/ public string   _mtrosName    ;
   /* 37 */ /*internal*/ public uint     _primPlatCD   ;
   /* 38 */ /*internal*/ public string   _primPlatTK   ;
   /* 39 */ /*internal*/ public string   _primPlatName ;
   /* 40 */ /*internal*/ public ushort   _pdvKnjiga    ;
   /* 41 */ /*internal*/ public bool     _isNpCash     ; 
   /* 42 */ /*internal*/ public ushort   _pdvR12       ;
   /* 43 */ /*internal*/ public ushort   _pdvKolTip    ; // FUSE 
   /* 44 */ /*internal*/ public decimal  _s_ukKCRP     ;
   /* 45 */ /*internal*/ public decimal  _s_ukKCRM     ;
   /* 46 */ /*internal*/ public decimal  _s_ukKCR      ;
   /* 47 */ /*internal*/ public decimal  _s_ukRbt1     ;
   /* 48 */ /*internal*/ public decimal  _s_ukRbt2     ;
   /* 49 */ /*internal*/ public decimal  _s_ukZavisni  ;
   /* 50 */ /*internal*/ public decimal  _s_ukProlazne ;
   /* 51 */ /*internal*/ public decimal  _s_ukPdv23m   ;
   /* 52 */ /*internal*/ public decimal  _s_ukPdv23n   ;
   /* 53 */ /*internal*/ public decimal  _s_ukPdv22m   ;
   /* 54 */ /*internal*/ public decimal  _s_ukPdv22n   ;
   /* 55 */ /*internal*/ public decimal  _s_ukPdv10m   ;
   /* 56 */ /*internal*/ public decimal  _s_ukPdv10n   ;
   /* 57 */ /*internal*/ public decimal  _s_ukOsn23m   ;
   /* 58 */ /*internal*/ public decimal  _s_ukOsn23n   ;
   /* 59 */ /*internal*/ public decimal  _s_ukOsn22m   ;
   /* 60 */ /*internal*/ public decimal  _s_ukOsn22n   ;
   /* 61 */ /*internal*/ public decimal  _s_ukOsn10m   ;
   /* 62 */ /*internal*/ public decimal  _s_ukOsn10n   ;
   /* 63 */ /*internal*/ public decimal  _s_ukOsn0     ;
   /* 64 */ /*internal*/ public decimal  _s_ukOsnPr    ;
   /* 65 */ /*internal*/ public string   _opciAlabel   ;
   /* 66 */ /*internal*/ public string   _opciAvalue   ;
   /* 67 */ /*internal*/ public string   _opciBlabel   ;
   /* 68 */ /*internal*/ public string   _opciBvalue   ;
   /* 69 */ /*internal*/ public uint     _odgvPersCD   ;
   /* 70 */ /*internal*/ public string   _odgvPersName ;
   /* 71 */ /*internal*/ public uint     _cjenTTnum    ;
   /* 72 */ /*internal*/ public string   _v3_tt        ;
   /* 73 */ /*internal*/ public uint     _v3_ttNum     ;
   /* 74 */ /*internal*/ public string   _v4_tt        ;
   /* 75 */ /*internal*/ public uint     _v4_ttNum     ;
   /* 76 */ /*internal*/ public decimal  _s_ukMrz      ;
   /* 77 */ /*internal*/ public decimal  _s_ukPdv      ;
   /* 78 */ /*internal*/ public string   _tipOtpreme   ;
   /* 79 */ /*internal*/ public int      _rokIsporuke  ;
   /* 80 */ /*internal*/ public DateTime _rokIspDate   ;
   /* 81 */ /*internal*/ public string   _dostName     ;
   /* 82 */ /*internal*/ public string   _dostAddr     ;
   /* 83 */ /*internal*/ public decimal  _s_ukOsn07    ;
   /* 84 */ /*internal*/ public decimal  _s_ukOsn08    ;
   /* 85 */ /*internal*/ public decimal  _s_ukOsn09    ;
   /* 86 */ /*internal*/ public decimal  _s_ukOsn10    ;
   /* 87 */ /*internal*/ public decimal  _s_ukOsn11    ;
   /* 88 */ /*internal*/ public decimal  _s_ukOsnUr23  ;
   /* 89 */ /*internal*/ public decimal  _s_ukOsnUu10  ;
   /* 90 */ /*internal*/ public decimal  _s_ukOsnUu22  ;
   /* 91 */ /*internal*/ public decimal  _s_ukOsnUu23  ;
   /* 92 */ /*internal*/ public decimal  _s_ukPdvUr23  ;
   /* 93 */ /*internal*/ public decimal  _s_ukPdvUu10  ;
   /* 94 */ /*internal*/ public decimal  _s_ukPdvUu22  ;
   /* 95 */ /*internal*/ public decimal  _s_ukPdvUu23  ;
   /* 96 */ /*internal*/ public ushort   _carinaKind   ;
   /* 97 */ /*internal*/ public string   _prjArtCD     ;
   /* 98 */ /*internal*/ public string   _prjArtName   ;
   /* 99 */ /*internal*/ public string   _externLink1  ;
   /*100 */ /*internal*/ public string   _externLink2  ;
   /*101 */ /*internal*/ public decimal  _someMoney    ;
   /*102 */ /*internal*/ public decimal  _somePercent  ;
   /*103 */ /*internal*/ public decimal  _s_ukMskPdv10 ;
   /*104 */ /*internal*/ public decimal  _s_ukMskPdv23 ;
   /*105 */ /*internal*/ public decimal  _s_ukMSK_00   ;
   /*106 */ /*internal*/ public decimal  _s_ukMSK_10   ;
   /*107 */ /*internal*/ public decimal  _s_ukMSK_23   ;
   /*108 */ /*internal*/ public decimal  _s_ukKCR_usl  ;
   /*109 */ /*internal*/ public decimal  _s_ukKCRP_usl ;
   /*110 */ /*internal*/ public decimal  _s_ukPdv25m   ;
   /*111 */ /*internal*/ public decimal  _s_ukPdv25n   ;
   /*112 */ /*internal*/ public decimal  _s_ukOsn25m   ;
   /*113 */ /*internal*/ public decimal  _s_ukOsn25n   ;
   /*114 */ /*internal*/ public decimal  _s_ukOsnUr25  ; 
   /*115 */ /*internal*/ public decimal  _s_ukOsnUu25  ;
   /*116 */ /*internal*/ public decimal  _s_ukPdvUr25  ; 
   /*117 */ /*internal*/ public decimal  _s_ukPdvUu25  ;
   /*118 */ /*internal*/ public decimal  _s_ukMskPdv25 ;
   /*119 */ /*internal*/ public decimal  _s_ukMSK_25   ;
   /*120 */ /*internal*/ public string   _fiskJIR      ;
   /*121 */ /*internal*/ public string   _fiskZKI      ;
   /*122 */ /*internal*/ public string   _fiskMsgID    ;
   /*123 */ /*internal*/ public string   _fiskOibOp    ;
   /*124 */ /*internal*/ public string   _fiskPrgBr    ;
   /*125 */ /*internal*/ public decimal  _s_ukPdv05m   ;
   /*126 */ /*internal*/ public decimal  _s_ukPdv05n   ;
   /*127 */ /*internal*/ public decimal  _s_ukOsn05m   ;
   /*128 */ /*internal*/ public decimal  _s_ukOsn05n   ;
   /*129 */ /*internal*/ public decimal  _s_ukMskPdv05 ;
   /*130 */ /*internal*/ public decimal  _s_ukMSK_05   ;
   /*131 */ /*internal*/ public decimal  _s_ukOsnUr05  ;
   /*132 */ /*internal*/ public decimal  _s_ukPdvUr05  ;
   /*133 */ /*internal*/ public decimal  _s_pixK       ;
   /*134 */ /*internal*/ public decimal  _s_puxK_P     ; // produkt 
   /*135 */ /*internal*/ public decimal  _s_puxK_All   ; // produkt + otpad 
   /*136 */ /*internal*/ public decimal  _s_pixKC      ;
   /*137 */ /*internal*/ public decimal  _s_puxKC_P    ; // produkt 
   /*138 */ /*internal*/ public decimal  _s_puxKC_All  ; // produkt + otpad 
   /*139 */ /*internal*/ public decimal  _s_ukPpmvOsn  ; // PPMV 
   /*140 */ /*internal*/ public decimal  _s_ukPpmvSt1i2; // PPMV 
   /*141 */ /*internal*/ public DateTime _dateX        ;
   /*142 */ /*internal*/ public string   _vatCntryCode ;
   /*143 */ /*internal*/ public ushort  _pdvGEOkind    ;
   /*144 */ /*internal*/ public ushort  _pdvZPkind     ;
   /*145 */ /*internal*/ public decimal _s_ukOsnR25m_EU;
   /*146 */ /*internal*/ public decimal _s_ukOsnR25n_EU;
   /*147 */ /*internal*/ public decimal _s_ukOsnU25m_EU;
   /*148 */ /*internal*/ public decimal _s_ukOsnU25n_EU;
   /*149 */ /*internal*/ public decimal _s_ukOsnR10m_EU;
   /*150 */ /*internal*/ public decimal _s_ukOsnR10n_EU;
   /*151 */ /*internal*/ public decimal _s_ukOsnU10m_EU;
   /*152 */ /*internal*/ public decimal _s_ukOsnU10n_EU;
   /*153 */ /*internal*/ public decimal _s_ukOsnR05m_EU;
   /*154 */ /*internal*/ public decimal _s_ukOsnR05n_EU;
   /*155 */ /*internal*/ public decimal _s_ukOsnU05m_EU;
   /*156 */ /*internal*/ public decimal _s_ukOsnU05n_EU;
   /*157 */ /*internal*/ public decimal _s_ukOsn25m_BS ;
   /*158 */ /*internal*/ public decimal _s_ukOsn25n_BS ;
   /*159 */ /*internal*/ public decimal _s_ukOsn10m_BS ;
   /*160 */ /*internal*/ public decimal _s_ukOsn10n_BS ;
   /*161 */ /*internal*/ public decimal _s_ukOsn25m_TP ;
   /*162 */ /*internal*/ public decimal _s_ukOsn25n_TP ;
   /*163 */ /*internal*/ public decimal _s_ukPdvR25m_EU;
   /*164 */ /*internal*/ public decimal _s_ukPdvR25n_EU;
   /*165 */ /*internal*/ public decimal _s_ukPdvU25m_EU;
   /*166 */ /*internal*/ public decimal _s_ukPdvU25n_EU;
   /*167 */ /*internal*/ public decimal _s_ukPdvR10m_EU;
   /*168 */ /*internal*/ public decimal _s_ukPdvR10n_EU;
   /*169 */ /*internal*/ public decimal _s_ukPdvU10m_EU;
   /*170 */ /*internal*/ public decimal _s_ukPdvU10n_EU;
   /*171 */ /*internal*/ public decimal _s_ukPdvR05m_EU;
   /*172 */ /*internal*/ public decimal _s_ukPdvR05n_EU;
   /*173 */ /*internal*/ public decimal _s_ukPdvU05m_EU;
   /*174 */ /*internal*/ public decimal _s_ukPdvU05n_EU;
   /*175 */ /*internal*/ public decimal _s_ukPdv25m_BS ;
   /*176 */ /*internal*/ public decimal _s_ukPdv25n_BS ;
   /*177 */ /*internal*/ public decimal _s_ukPdv10m_BS ;
   /*178 */ /*internal*/ public decimal _s_ukPdv10n_BS ;
   /*179 */ /*internal*/ public decimal _s_ukPdv25m_TP ;
   /*180 */ /*internal*/ public decimal _s_ukPdv25n_TP ;
   /*181 */ /*internal*/ public decimal _s_ukOsnZP_11  ;
   /*182 */ /*internal*/ public decimal _s_ukOsnZP_12  ;
   /*183 */ /*internal*/ public decimal _s_ukOsnZP_13  ;
   /*184 */ /*internal*/ public decimal _s_ukOsn12     ;
   /*185 */ /*internal*/ public decimal _s_ukOsn13     ;
   /*186 */ /*internal*/ public decimal _s_ukOsn14     ;
   /*187 */ /*internal*/ public decimal _s_ukOsn15     ;
   /*188 */ /*internal*/ public decimal _s_ukOsn16     ;
   /*189 */ /*internal*/ public decimal _s_ukOsnPNP    ; // izlaz 
   /*190 */ /*internal*/ public decimal _s_ukIznPNP    ; // izlaz 
   /*191 */ /*internal*/ public decimal _s_ukMskPNP    ; // ulaz & izlaz 
   /*192 */ /*internal*/ public decimal _skiz_ukKC     ;
   /*193 */ /*internal*/ public decimal _skiz_ukKCR    ;
   /*194 */ /*internal*/ public decimal _skiz_ukRbt1   ;

   /*195 */ /*internal*/ public decimal _s_ukKCRP_NP2  ;
   /*196 */ /*internal*/ public string  _nacPlac2      ;
   /*197 */ /*internal*/ public bool    _isNpCash2     ; 
                                    
}

#endregion struct FakturStruct & FaktExStruct

public class Faktur : VvPolyDocumRecord, IVvExtendableDataRecord, IComparable<Faktur>
{
   #region Fildz

   public const string recordName       = "faktur";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   public static DateTime NewPdvStopaDate = new DateTime(2012, 03, 01);
   public static decimal  CommonPdvStForThisDate(DateTime date, bool isPovlastenaStopa)
   {
      if(isPovlastenaStopa) return 13.00M;
      else                  return date < NewPdvStopaDate ? 23.00M : 25.00M;
   }
   public static decimal  CommonPdvStForThisDate(DateTime date)
   {
      return CommonPdvStForThisDate(date, false);
   }
   public decimal  CommonPdvSt { get { return CommonPdvStForThisDate(this./*PdvDate*/DokDate); } }


 //public const string TT_WFA = "WFA"; // iz proslih godina Ulazni Racun NE Sklad 
 //public const string TT_WRA = "WRA"; // iz proslih godina Ulazni Racun u VELEP + Primka 
 //public const string TT_WRM = "WRM"; // iz proslih godina Ulazni Racun u MALOP + Kalkulacija 
 //public const string TT_YFA = "YFA"; // iz proslih godina Izlazni Racun NE Sklad 
 //public const string TT_YRA = "YRA"; // iz proslih godina Izlazni Racun + Izdatnica 
 //public const string TT_YRM = "YRM"; // iz proslih godina Malop Racun 

   public const string TT_WRN = "WRN"; // iz proslih godina Ulazni Racun NE Sklad 
   public const string TT_YRN = "YRN"; // iz proslih godina Izlazni Racun NE Sklad 

   public const string TT_UPA = "UPA"; // PDV Temeljnica - EU virtualni porez, PDV pri uvozu iz NOT EU 

   public const string TT_PST = "PST"; // Pocetno Stanje 

   public const string TT_UFA = "UFA"; // Ulazni Racun NE Sklad 
   public const string TT_UFM = "UFM"; // Ulazni Racun NE Sklad - mALOP 

   public const string TT_URA = "URA"; // Ulazni Racun u VELEP + Primka 
   public const string TT_NUP = "NUP"; // Implicitna Nivelacija - ULAZ Povrata  
   public const string TT_PRI = "PRI"; // Primka u Veleprodaju NE Pdv 
   public const string TT_PIP = "PIP"; // Primka iz PROIZVODNJE 

   public const string TT_PSM = "PSM"; // Pocetno Stanje MALOPRODAJE 
   public const string TT_URM = "URM"; // Ulazni Racun u MALOP + Kalkulacija 
   public const string TT_KLK = "KLK"; // Primka u Maloprodaju - Kalkulacija NE Pdv 
   public const string TT_KKM = "KKM"; // KOMISIJSKA Primka u Maloprodaju - Kalkulacija NE Pdv 
   public const string TT_ZPC = "ZPC"; // Explicitna Nivelacija - Zapisnik o PromjeniCijena 
   public const string TT_ZKC = "ZKC"; // Explicitna Nivelacija - KOREKCIJA IZLAZA po krivom MPC-u 
   public const string TT_NIV = "NIV"; // Implicitna Nivelacija - IZLAZ
   public const string TT_NUV = "NUV"; // Implicitna Nivelacija - ULAZ 
   public const string TT_IRM = "IRM"; // Malop Racun 
   public const string TT_VMI = "VMI"; // Meduskladisnica VELEP 2 MALOP - IZLAZ ex 'G' 
   public const string TT_VMU = "VMU"; // Meduskladisnica VELEP 2 MALOP - ULAZ  ex 'R' 
   public const string TT_MVI = "MVI"; // Meduskladisnica MALOP 2 VELEP - IZLAZ        
   public const string TT_MVU = "MVU"; // Meduskladisnica MALOP 2 VELEP - ULAZ         

 
 //public const string TT_STU = "STU"; // STORNO Ulaza (i sklad i pdv) 
   public const string TT_RVU = "RVU"; // Revers Povrat - Fisycal ONLY ulaz 

   public const string TT_IFA = "IFA"; // Izlazni Racun NE Sklad 

   public const string TT_IRA = "IRA"; // Izlazni Racun + Izdatnica 
   public const string TT_IZD = "IZD"; // Izdatnica iz Veleprodaje NE Pdv - ProdCij
   public const string TT_IZM = "IZM"; // Izdatnica iz Maloprodaje NE Pdv - ProdCij
 //public const string TT_STI = "STI"; // STORNO Izlaza (i sklad i pdv) 
   public const string TT_RVI = "RVI"; // Revers - Fisycal ONLY izlaz

   public const string TT_IMT = "IMT"; // Izdatnica na MjestoTroska VELEP NE Pdv - PrNabCij
 //public const string TT_IMM = "IMM"; // Izdatnica na MjestoTroska MALOP NE Pdv - PrNabCij FUSE? 
   public const string TT_PPR = "PPR"; // Predatnica u Proizvodnju NE Pdv 
   public const string TT_UPL = "UPL"; // Blagajna - ulaz  (Uplatnica) 
   public const string TT_ISP = "ISP"; // Blagajna - izlaz (Isplatnica) 
   public const string TT_BUP = "BUP"; // Blagajna - ulaz  (Uplatnica)  - MULTI (vise blagajni)
   public const string TT_BIS = "BIS"; // Blagajna - izlaz (Isplatnica) - MULTI (vise blagajni)
   public const string TT_TMU = "TMU"; // Korektura ulaza  - TEMELJNICA 
   public const string TT_TMI = "TMI"; // Korektura izlaza - TEMELJNICA 

   public const string TT_SKU = "SKU"; // Fisycal ONLY ulaz
   public const string TT_SKI = "SKI"; // Fisycal ONLY izlaz

   public const string TT_RNP = "RNP"; // Radni Nalog Proizvodnje - Frigoterm (VISE  sirovina/mat ---> JEDAN proizvod) 
   public const string TT_RNM = "RNM"; // Radni Nalog Proizvodnje - Metaflex  (JEDNA sirovina/mat ---> VISE proizvoda) - IZLAZ 
   public const string TT_RNU = "RNU"; // Radni Nalog Proizvodnje - Metaflex  (JEDNA sirovina/mat ---> VISE proizvoda) - ULAZ  
   public const string TT_RNS = "RNS"; // Radni Nalog Servisa     
   public const string TT_PRJ = "PRJ"; // Projekt (opci Radni Nalog) 
   public const string TT_BOR = "BOR"; // Boravak Gostiju (Hotel - Recepcija) 
   public const string TT_RNZ = "RNZ"; // Radni Nalog Zaštitara 
   public const string TT_UGO = "UGO"; // Ugovor - Tender - SvDUH ...
   public const string TT_ZAH = "ZAH"; // Zahtjevnica     - SvDUH ...

   public const string TT_KUG = "KUG"; // PTG - Krovni  Ugovor  za najam                        
   public const string TT_AUN = "AUN"; // PTG - Aneks   Ugovora za najam  - međuskladišni IZLAZ 
   public const string TT_UGN = "UGN"; // PTG -         Ugovor  za najam  - međuskladišni IZLAZ 
   public const string TT_DOD = "DOD"; // PTG - Dodatak Ugovoru za najam  - međuskladišni IZLAZ 
   public const string TT_AU2 = "AU2"; // PTG - Aneks   Ugovora za najam  - međuskladišni ULAZ  ... ovo su ujedno i Rtrano TT-ovi 
   public const string TT_UG2 = "UG2"; // PTG -         Ugovor  za najam  - međuskladišni ULAZ  ... ovo su ujedno i Rtrano TT-ovi 
   public const string TT_DO2 = "DO2"; // PTG - Dodatak Ugovoru za najam  - međuskladišni ULAZ  ... ovo su ujedno i Rtrano TT-ovi 
   public const string TT_PVR = "PVR"; // PTG - Povrat po Ugovoru za najam- međuskladišni IZLAZ 
   public const string TT_PV2 = "PV2"; // PTG - Povrat po Ugovora za najam- međuskladišni ULAZ  
 //public const string TT_KOP = "KOP"; // PTG - Korekcija Otplatnog plana  preseljeno u Mixer   
   public const string TT_MOD = "MOD"; // PTG - Modifikator računala ('PCK' - ova; desktopa ili notebooka) - npr. modidiciramo kapacitet RAM-a računalu - skladišni NEUTRAL  - samo zaglavlje (faktur_rec) 
   public const string TT_MOU = "MOU"; // PTG - Modifikator računala ('PCK' - ova; desktopa ili notebooka) - npr. memorija sa skl rez. djelova - ULAZ,  nije međuskladišnica - samo NOT PCK-ovi            
   public const string TT_MOI = "MOI"; // PTG - Modifikator računala ('PCK' - ova; desktopa ili notebooka) - npr. memorija sa skl rez. djelova - IZLAZ, nije međuskladišnica - samo NOT PCK-ovi            
   public const string TT_MOC = "MOC"; // PTG - Modifikator računala ('PCK' - ova; desktopa ili notebooka) -                                                                 - samo PCK-ovi                
   public const string TT_MOS = "MOS"; // PTG - Modifikator računala ('PCK' - ova; desktopa ili notebooka) -                                                                 - samo PCK-ovi                

   public const string TT_MSI = "MSI"; // Meduskladisnica VELEP - IZLAZ 
   public const string TT_MSU = "MSU"; // Meduskladisnica VELEP - ULAZ  

   public const string TT_MMI = "MMI"; // Meduskladisnica MALOP - IZLAZ 
   public const string TT_MMU = "MMU"; // Meduskladisnica MALOP - ULAZ  

   public const string TT_KIZ = "KIZ"; // Meduskladisnica KOMISIJA - IZLAZ 
   public const string TT_KUL = "KUL"; // Meduskladisnica KOMISIJA - ULAZ  

   public const string TT_PIK = "PIK"; // Meduskladisnica KOMISIJA - POVRAT - IZLAZ 
   public const string TT_PUK = "PUK"; // Meduskladisnica KOMISIJA - POVRAT - ULAZ  

   public const string TT_PIZ = "PIZ"; // Proizvodnja - IZLAZ 
   public const string TT_PUL = "PUL"; // Proizvodnja - ULAZ  
   public const string TT_PIX = "PIX"; // Proizvodnja - IZLAZ - Externi-Extendable 
   public const string TT_PUX = "PUX"; // Proizvodnja - ULAZ  - Externi-Extendable 
   public const string TT_PIM = "PIM"; // Proizvodnja - IZLAZ - Maloprodaja (Ugostitelji - kafići - žestica na čaše, gemišti, ...) 
   public const string TT_PUM = "PUM"; // Proizvodnja - ULAZ  - Maloprodaja (Ugostitelji - kafići - žestica na čaše, gemišti, ...) 

   public const string TT_TRI = "TRI"; // Proizvodnja VELEP 2 MALOP - IZLAZ - Transformacija - pretvorba npr VP kilogrami vreća u MP komade 
   public const string TT_TRM = "TRM"; // Proizvodnja VELEP 2 MALOP - ULAZ  - Transformacija - pretvorba npr VP kilogrami vreća u MP komade 

   public const string TT_NOR = "NOR"; // Proizvodnja - NORMATIV za SASTAVNICU 
 //public const string TT_NOM = "NOM"; // Proizvodnja - NORMATIV za SASTAVNICU - MALOPRODAJA 

   public const string TT_CJ_VP1 = "CV1"; // Cjenik VPC1 
   public const string TT_CJ_VP2 = "CV2"; // Cjenik VPC2 
   public const string TT_CJ_MP  = "CM1"; // Cjenik MPC  
   public const string TT_CJ_DE  = "CDE"; // Cjenik Devizni 
   public const string TT_CJ_MK  = "MNK"; // Cjenik MinKol  
   public const string TT_CJ_RB1 = "RB1"; // Cjenik Rabat1  
   public const string TT_CJ_RB2 = "RB2"; // Cjenik Rabat2  
   public const string TT_CJ_MRZ = "MRZ"; // Cjenik MARZA   
   public const string TT_CKP    = "CKP"; // Cjenik KUPCA (kunski ili devizni) 

   /// <summary>
   /// OBVEZUJUCE PONUDE (REZERVACIJE)
   /// </summary>
   public const string TT_OPN    = "OPN"; // Obvezujuce ponude (REZERVACIJE) 
   /// <summary>
   /// INVENTURA
   /// </summary>
   public const string TT_INV = "INV"; // INVENTURA 
   public const string TT_INM = "INM"; // INVENTURA MALOPRODAJNA 

   public const string TT_UOD = "UOD"; // Ulazno Odobrenje od Dobavljaca (umanjenje URA-a) 
 //public const string TT_UTR = "UTR"; // Ulazno Terecenje od Dobavljaca 
   public const string TT_UPV = "UPV"; // Ulazno Povrat Dobavljacu (storno URA-a) 
   public const string TT_UPM = "UPM"; // Malop Ulazno Povrat Dobavljacu (storno URM-a) 

   public const string TT_IOD = "IOD"; // Izlazno Odobrenje Kupcu (umanjenje IRA-e)
 //public const string TT_ITR = "ITR"; // Izlazno Terecenje Kupcu 
   public const string TT_IPV = "IPV"; // Izlazno Povrat od Kupca (storno IRA-e)
   public const string TT_POV = "POV"; // Izlazno Povrat na skladiste (npr iz proizvodnje) (storno izdatnice)

   public const string TT_PON = "PON"; // Ponuda Kupcu
   public const string TT_PNM = "PNM"; // Ponuda Kupcu - MALOPRODAJNA (MPC) 
   public const string TT_NRK = "NRK"; // Narudzba od Kupca
   public const string TT_NRD = "NRD"; // Narudzba Dobavljacu
   public const string TT_NRU = "NRU"; // Narudzba Uvoza Dobavljacu
   public const string TT_NRS = "NRS"; // Narudzba Za Uslugu Dobavljacu
   public const string TT_NRM = "NRM"; // Narudzba Maloprodajna 

 //private FakturStruct currentData; //17.09.2021 za potrebe PTG mapiranja
   public  FakturStruct currentData; //17.09.2021 za potrebe PTG mapiranja

   private FakturStruct backupData;

   protected static System.Data.DataTable TheSchemaTable   = ZXC.FakturDao.TheSchemaTable;
   protected static System.Data.DataTable TheSchemaTableEx = ZXC.FaktExDao.TheSchemaTable;
   protected static FakturDao.FakturCI CI                  = ZXC.FakturDao.CI;
   protected static FaktExDao.FaktExCI CIex                = ZXC.FaktExDao.CI;

   public static string tt_colName       = ZXC.FakturDao.GetSchemaColumnName(CI.tt      );
   public static string skladCd_colName  = ZXC.FakturDao.GetSchemaColumnName(CI.skladCD );
   public static string skladCd2_colName = ZXC.FakturDao.GetSchemaColumnName(CI.skladCD2);

   #endregion Fildz

   #region Constructors

   public Faktur() : this(0)
   {
   }

   public Faktur(uint ID) : base()
   {
      this.currentData = new FakturStruct();

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

      /* 05 */     this.currentData._dokNum      = 0;
      /* 06 */     this.currentData._dokDate     = DateTime.MinValue;
      /* 07 */     this.currentData._tt          = "";
      /* 08 */     this.currentData._ttNum       = 0;
      /* 09 */     this.currentData._ttSort      = 0;
      /* 10 */     this.currentData._skladCD     = "";
      /* 11 */     this.currentData._skladCD2    = "";
      /* 12 */     this.currentData._vezniDok    = "";
      /* 13 */     this.currentData._napomena    = "";
      /* 14 */     this.currentData._opis        = "";
      /* 15 */     this.currentData._konto       = "";
      /* 16 */     this.currentData._projektCD   = "";
      /* 17 */     this.currentData._v1_tt       = "";
      /* 18 */     this.currentData._v1_ttNum    = 0;
      /* 19 */     this.currentData._v2_tt       = "";
      /* 20 */     this.currentData._v2_ttNum    = 0;
      /* 21 */     this.currentData._s_ukKC      = decimal.Zero;
      /* 22 */     this.currentData._s_ukK       = decimal.Zero;
      /* 23 */     this.currentData._s_trnCount  = 0;
      /* 24 */     this.currentData._osobaX      = "";
      /* 25 */     this.currentData._dokDate2    = DateTime.MinValue;
      /* 26 */     this.currentData._s_ukK2      = decimal.Zero;
      /* 27 */     this.currentData._decimal01   = decimal.Zero;
      /* 28 */     this.currentData._decimal02   = decimal.Zero;

                   this.Transes  = new List<Rtrans>();
                   this.Transes2 = new List<Rtrano>();
                   this.Transes3 = new List<Rtrano>();

                   this.TheEx = new FaktEx();
   }


   public Faktur
      (
      DateTime        _DokDate      ,
      uint            _RecID        ,
      string          _TT           ,
      uint            _TtNum        ,
      DateTime        _DospDate     ,
      uint            _MtrosCD      ,
      string          _MtrosTK      ,
      string          _ProjektCD    ,
ZXC.PdvKnjigaEnum     _PdvKnjiga    ,
ZXC.ShouldFak2NalEnum _ShouldFak2Nal,
      string          _Napomena     ,
      string          _V1_tt        ,
      uint            _V1_ttNum     ,
      string          _V2_tt        ,
      uint            _V2_ttNum     ,
      string          _V3_tt        ,
      uint            _V3_ttNum     ,
      string          _V4_tt        ,
      uint            _V4_ttNum     ,
      uint            _PrimPlatCD   ,
      string          _PrimPlatTK   ,
      uint            _KupdobCD     ,
      string          _KupdobTK     ,
      string          _SkladCD      ,
      decimal         _S_ukKCRP     ,
      decimal         _S_ukOsn23m   ,
      decimal         _S_ukOsn23n   ,
      decimal         _S_ukOsn22m   ,
      decimal         _S_ukOsn22n   ,
      decimal         _S_ukOsn10m   ,
      decimal         _S_ukOsn10n   ,
      string          _Konto        ,
      decimal         _S_ukPdv23m   ,
      decimal         _S_ukPdv23n   ,
      decimal         _S_ukPdv22m   ,
      decimal         _S_ukPdv22n   ,
      decimal         _S_ukPdv10m   ,
      decimal         _S_ukPdv10n   ,
      string          _VezniDok     ,
                      
      bool            _IsNpCash     ,
      string          _NacPlac      ,
      decimal         _S_ukKCR      ,
      decimal         _S_ukKCR_usl  ,
      decimal         _S_ukKCRP_usl ,
      decimal         _S_ukMSK_00   ,
      decimal         _S_ukMSK_10   ,
      decimal         _S_ukMSK_23   ,
      decimal         _S_ukMskPdv10 ,
      decimal         _S_ukMskPdv23 ,
      decimal         _K_NivVrj00   ,
      decimal         _K_NivVrj10   ,
      decimal         _K_NivVrj23   ,
      decimal         _Ira_ROB_NV   ,
                      
      decimal         _S_ukPdv25m   ,
      decimal         _S_ukPdv25n   ,
      decimal         _S_ukOsn25m   ,
      decimal         _S_ukOsn25n   ,
      decimal         _S_ukMskPdv25 ,
      decimal         _S_ukMSK_25   ,
      decimal         _K_NivVrj25   ,
      decimal         _S_ukPdv05m   ,
      decimal         _S_ukPdv05n   ,
      decimal         _S_ukOsn05m   ,
      decimal         _S_ukOsn05n   ,
      decimal         _S_ukMskPdv05 ,
      decimal         _S_ukMSK_05   ,
      decimal         _K_NivVrj05   ,
      decimal         _X_ukPpmvIzn  ,
      //ZXC.PdvGEOkindEnum PdvGEOkind,
      //ZXC.PdvZPkindEnum  PdvZPkind ,
      //decimal         S_ukOsnR25m_EU,
      //decimal         S_ukOsnR25n_EU,
      //decimal         S_ukOsnU25m_EU,
      //decimal         S_ukOsnU25n_EU,
      //decimal         S_ukOsnR10m_EU,
      //decimal         S_ukOsnR10n_EU,
      //decimal         S_ukOsnU10m_EU,
      //decimal         S_ukOsnU10n_EU,
      //decimal         S_ukOsnR05m_EU,
      //decimal         S_ukOsnR05n_EU,
      //decimal         S_ukOsnU05m_EU,
      //decimal         S_ukOsnU05n_EU,
      //decimal         S_ukOsn25m_BS ,
      //decimal         S_ukOsn25n_BS ,
      //decimal         S_ukOsn10m_BS ,
      //decimal         S_ukOsn10n_BS ,
      //decimal         S_ukOsn25m_TP ,
      //decimal         S_ukOsn25n_TP ,
      //decimal         S_ukPdvR25m_EU,
      //decimal         S_ukPdvR25n_EU,
      //decimal         S_ukPdvU25m_EU,
      //decimal         S_ukPdvU25n_EU,
      //decimal         S_ukPdvR10m_EU,
      //decimal         S_ukPdvR10n_EU,
      //decimal         S_ukPdvU10m_EU,
      //decimal         S_ukPdvU10n_EU,
      //decimal         S_ukPdvR05m_EU,
      //decimal         S_ukPdvR05n_EU,
      //decimal         S_ukPdvU05m_EU,
      //decimal         S_ukPdvU05n_EU,
      //decimal         S_ukPdv25m_BS ,
      //decimal         S_ukPdv25n_BS ,
      //decimal         S_ukPdv10m_BS ,
      //decimal         S_ukPdv10n_BS ,
      //decimal         S_ukPdv25m_TP ,
      //decimal         S_ukPdv25n_TP ,
      //decimal         S_ukOsnZP_11  ,
      //decimal         S_ukOsnZP_12  ,
      //decimal         S_ukOsnZP_13  ,
      //decimal         S_ukOsn12     ,
      //decimal         S_ukOsn13     ,
      //decimal         S_ukOsn14     ,
      //decimal         S_ukOsn15     ,
      //decimal         S_ukOsn16     
      decimal           _S_ukOsnPNP    ,
      decimal           _S_ukIznPNP    ,
      decimal           _S_ukMskPNP    ,
      decimal           _Skiz_ukKC     ,
      decimal           _Skiz_ukKCR    ,
      decimal           _Skiz_ukRbt1   ,
      decimal           _S_ukKCRP_NP2  ,
      string            _NacPlac2      ,
      bool              _IsNpCash2
      )
      : this(0)
   {
      this.DokDate       = _DokDate      ;
      this.RecID         = _RecID        ;
      this.TT            = _TT           ;
      this.TtNum         = _TtNum        ;
      this.DospDate      = _DospDate     ;
      this.MtrosCD       = _MtrosCD      ;
      this.MtrosTK       = _MtrosTK      ;
      this.ProjektCD     = _ProjektCD    ;
      this.PdvKnjiga     = _PdvKnjiga    ;
      this.ShouldFak2Nal = _ShouldFak2Nal;
      this.Napomena      = _Napomena     ;
      this.V1_tt         = _V1_tt        ;
      this.V1_ttNum      = _V1_ttNum     ;
      this.V2_tt         = _V2_tt        ;
      this.V2_ttNum      = _V2_ttNum     ;
      this.V3_tt         = _V3_tt        ;
      this.V3_ttNum      = _V3_ttNum     ;
      this.V4_tt         = _V4_tt        ;
      this.V4_ttNum      = _V4_ttNum     ;
      this.PrimPlatCD    = _PrimPlatCD   ;
      this.PrimPlatTK    = _PrimPlatTK   ;
      this.KupdobCD      = _KupdobCD     ;
      this.KupdobTK      = _KupdobTK     ;
      this.SkladCD       = _SkladCD      ;
      this.S_ukKCRP      = _S_ukKCRP     ;
      this.S_ukOsn23m    = _S_ukOsn23m   ;
      this.S_ukOsn23n    = _S_ukOsn23n   ;
      this.S_ukOsn22m    = _S_ukOsn22m   ;
      this.S_ukOsn22n    = _S_ukOsn22n   ;
      this.S_ukOsn10m    = _S_ukOsn10m   ;
      this.S_ukOsn10n    = _S_ukOsn10n   ;
      this.Konto         = _Konto        ;
      this.S_ukPdv23m    = _S_ukPdv23m   ;
      this.S_ukPdv23n    = _S_ukPdv23n   ;
      this.S_ukPdv22m    = _S_ukPdv22m   ;
      this.S_ukPdv22n    = _S_ukPdv22n   ;
      this.S_ukPdv10m    = _S_ukPdv10m   ;
      this.S_ukPdv10n    = _S_ukPdv10n   ;
      this.VezniDok      = _VezniDok     ;
                           
      this.IsNpCash      = _IsNpCash     ;
      this.NacPlac       = _NacPlac      ;
      this.S_ukKCR       = _S_ukKCR      ;
      this.S_ukKCR_usl   = _S_ukKCR_usl  ;
      this.S_ukKCRP_usl  = _S_ukKCRP_usl ;
      this.S_ukMSK_00    = _S_ukMSK_00   ;
      this.S_ukMSK_10    = _S_ukMSK_10   ;
      this.S_ukMSK_23    = _S_ukMSK_23   ;
      this.S_ukMskPdv10  = _S_ukMskPdv10 ;
      this.S_ukMskPdv23  = _S_ukMskPdv23 ;
      this.K_NivVrj00    = _K_NivVrj00   ;
      this.K_NivVrj10    = _K_NivVrj10   ;
      this.K_NivVrj23    = _K_NivVrj23   ;
      this.Ira_ROB_NV    = _Ira_ROB_NV   ;
                           
      this.S_ukPdv25m    = _S_ukPdv25m   ;
      this.S_ukPdv25n    = _S_ukPdv25n   ;
      this.S_ukOsn25m    = _S_ukOsn25m   ;
      this.S_ukOsn25n    = _S_ukOsn25n   ;
      this.S_ukMskPdv25  = _S_ukMskPdv25 ;
      this.S_ukMSK_25    = _S_ukMSK_25   ;
      this.K_NivVrj25    = _K_NivVrj25   ;
                           
      this.S_ukPdv05m    = _S_ukPdv05m   ;
      this.S_ukPdv05n    = _S_ukPdv05n   ;
      this.S_ukOsn05m    = _S_ukOsn05m   ;
      this.S_ukOsn05n    = _S_ukOsn05n   ;
      this.S_ukMskPdv05  = _S_ukMskPdv05 ;
      this.S_ukMSK_05    = _S_ukMSK_05   ;
      this.K_NivVrj05    = _K_NivVrj05   ;
      this.X_ukPpmvIzn   = _X_ukPpmvIzn  ;

      this.PdvGEOkind     = PdvGEOkind    ; // TODO: !!!!!!!!!!!! koji K je ovo this a ne parametar? 
      this.PdvZPkind      = PdvZPkind     ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR25m_EU = S_ukOsnR25m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR25n_EU = S_ukOsnR25n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU25m_EU = S_ukOsnU25m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU25n_EU = S_ukOsnU25n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR10m_EU = S_ukOsnR10m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR10n_EU = S_ukOsnR10n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU10m_EU = S_ukOsnU10m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU10n_EU = S_ukOsnU10n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR05m_EU = S_ukOsnR05m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnR05n_EU = S_ukOsnR05n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU05m_EU = S_ukOsnU05m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnU05n_EU = S_ukOsnU05n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn25m_BS  = S_ukOsn25m_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn25n_BS  = S_ukOsn25n_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn10m_BS  = S_ukOsn10m_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn10n_BS  = S_ukOsn10n_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn25m_TP  = S_ukOsn25m_TP ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn25n_TP  = S_ukOsn25n_TP ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR25m_EU = S_ukPdvR25m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR25n_EU = S_ukPdvR25n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU25m_EU = S_ukPdvU25m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU25n_EU = S_ukPdvU25n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR10m_EU = S_ukPdvR10m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR10n_EU = S_ukPdvR10n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU10m_EU = S_ukPdvU10m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU10n_EU = S_ukPdvU10n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR05m_EU = S_ukPdvR05m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvR05n_EU = S_ukPdvR05n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU05m_EU = S_ukPdvU05m_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdvU05n_EU = S_ukPdvU05n_EU; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv25m_BS  = S_ukPdv25m_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv25n_BS  = S_ukPdv25n_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv10m_BS  = S_ukPdv10m_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv10n_BS  = S_ukPdv10n_BS ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv25m_TP  = S_ukPdv25m_TP ; // -||-   -||-   -||-                                  ? 
      this.S_ukPdv25n_TP  = S_ukPdv25n_TP ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnZP_11   = S_ukOsnZP_11  ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnZP_12   = S_ukOsnZP_12  ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsnZP_13   = S_ukOsnZP_13  ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn12      = S_ukOsn12     ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn13      = S_ukOsn13     ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn14      = S_ukOsn14     ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn15      = S_ukOsn15     ; // -||-   -||-   -||-                                  ? 
      this.S_ukOsn16      = S_ukOsn16     ; // -||-   -||-   -||-                                  ?  
      this.S_ukOsnPNP     = _S_ukOsnPNP   ; 
      this.S_ukIznPNP     = _S_ukIznPNP   ; 
      this.S_ukMskPNP     = _S_ukMskPNP   ; 
      this.Skiz_ukKC      = _Skiz_ukKC    ; 
      this.Skiz_ukKCR     = _Skiz_ukKCR   ; 
      this.Skiz_ukRbt1    = _Skiz_ukRbt1  ;
      this.S_ukKCRP_NP1   = _S_ukKCRP_NP2 ;
      this.NacPlac2       = _NacPlac2     ;
      this.IsNpCash2      = _IsNpCash2    ;
   }

   #endregion Constructors

   #region Sorters

   // 26.03.2018: sorter sorterTtRecID additions ... kasnije abortirao kao NEUSPJEH 
   //public static VvSQL.RecordSorter sorterTtRecID = new VvSQL.RecordSorter(Faktur.recordName, Faktur.recordNameArhiva, new VvSQL.IndexSegment[]
   //   {
   //      new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttSort]),
   //      new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recID ]),
   //      new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer] , true)
   //   }, "TtRecID", VvSQL.SorterType.NewRecID, false);

   public static VvSQL.RecordSorter sorterTtNum = new VvSQL.RecordSorter(Faktur.recordName, Faktur.recordNameArhiva, new VvSQL.IndexSegment[]  
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttSort]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttNum ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer] , true)
      }, "TtNum", VvSQL.SorterType.TtNum, false);

   public static VvSQL.RecordSorter sorterDokDate = new VvSQL.RecordSorter(Faktur.recordName, Faktur.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttSort ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.dokDate]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.ttNum  ]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer ] , true)
      }, "Datum", VvSQL.SorterType.DokDate, false);

   public static VvSQL.RecordSorter sorterKpdbName = new VvSQL.RecordSorter(Faktur.recordName, Faktur.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.ttSort      ]),
         new VvSQL.IndexSegment(TheSchemaTableEx.Rows[CIex.kupdobName]),
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.ttNum       ]),
         new VvSQL.IndexSegment(TheSchemaTable  .Rows[CI.recVer      ] , true)
      }, "Partner", VvSQL.SorterType.KpdbName, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      {   
         sorterTtNum,
         sorterDokDate,
         sorterKpdbName/*,
         sorterTtRecID*/
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
       //case VvSQL.SorterType.NewRecID: return new object[] { this.TtSort, this.RecID                 , RecVer };
         case VvSQL.SorterType.TtNum   : return new object[] { this.TtSort, this.TtNum                 , RecVer };
         case VvSQL.SorterType.DokDate : return new object[] { this.TtSort, this.DokDate   , this.TtNum, RecVer };
         case VvSQL.SorterType.KpdbName: return new object[] { this.TtSort, (IsExtendable ? this.KupdobName : ""), this.TtNum, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get
      {
         // 26.03.2018: sorter sorterTtRecID additions ... kasnije abortirao kao NEUSPJEH 
         return                                      Faktur.sorterTtNum;
       //return ZXC.IsSvDUH ? Faktur.sorterTtRecID : Faktur.sorterTtNum;
      }
   }

   #endregion Sorters

   #region Overriders And Specifics

   public FakturStruct CurrentData // cijela FakturStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   internal FakturStruct BackupData 
   {
      get { return this.backupData; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.FakturDao; }
   }

   public override string VirtualRecordName
   {
      get { return Faktur.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Faktur.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "fa"; }
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

   public override uint   VirtualLanSrvID{ get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint   VirtualLanRecID{ get { return this.LanRecID; } set { this.LanRecID = value; } }

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

   //=== About Transes ======================================================== 

   public override string TransRecordName
   {
      get { return Rtrans.recordName; }
   }

   public override string TransRecordName2
   {
      get { return Rtrano.recordName; }
   }

   public override string TransRecordName3
   {
      get { return Rtrano.recordName; }
   }

   public override string TransRecordNameArhiva
   {
      get { return Rtrans.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva2
   {
      get { return Rtrano.recordNameArhiva; }
   }

   public override string TransRecordNameArhiva3
   {
      get { return Rtrano.recordNameArhiva; }
   }

   /// <summary>
   /// Gets or sets a list of rtrans (line items) for the faktur.
   /// </summary>
   public List<Rtrans> Transes  { get; set; }

   public List<Rtrano> Transes2 { get; set; }
   public List<Rtrano> Transes3 { get; set; }

   /// <summary>
   /// PAZI!!! Ovdje a'o po jajima. Metode nemozes pozivati nego Invoke()... vidi dolje.
   /// get {};  vraca zapravo 'List<Rtrans> Transes' konvertiran u List<VvTransRecord>
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
      this.Transes.Remove((Rtrans)trans_rec);
   }

   public override void InvokeTransRemove2(VvTransRecord trans_rec)
   {
      this.Transes2.Remove((Rtrano)trans_rec);
   }

   public override void InvokeTransRemove3(VvTransRecord trans_rec)
   {
      this.Transes3.Remove((Rtrano)trans_rec);
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

   // 13.07.2015: 
 //public TtInfo TtInfo      { get { try { return                      ZXC.RiskTT[this.TT];                } catch(Exception) { return new TtInfo(); } } }
   public TtInfo TtInfo      { get { try { return this.TT.NotEmpty() ? ZXC.RiskTT[this.TT] : new TtInfo(); } catch(Exception) { return new TtInfo(); } } }
 //public TtInfo TtInfo4Twin { get { try { return                      ZXC.RiskTT[ZXC.RiskTT[this.TT].TwinTT]               ; } catch(Exception) { return new TtInfo(); } } }
   public TtInfo TtInfo4Twin { get { try { return this.TT.NotEmpty() ? ZXC.RiskTT[ZXC.RiskTT[this.TT].TwinTT] : new TtInfo(); } catch(Exception) { return new TtInfo(); } } }

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

   public override bool IsExtendable
   {
      get
      {
         return this.TtInfo.IsExtendableTT;
      }
   }

   public FaktEx TheEx { get; set; }

   public override VvDataRecord VirtualExtenderRecord
   {
      get
      {
         return this.TheEx;
      }
      set
      {
         this.TheEx = (FaktEx)value;
      }
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
    //return                      SaveSerialized_VvDataRecord_ToXmlFile_JOB<Faktur>(fileName, _isAutoCreat);
      return new FakturType(this).SaveSerialized_VvDataRecord_ToXmlFile            (fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
    //return DeserializeFromXmlFile<Faktur>(fileName);
      FakturType fakturType = DeserializeFromXmlFile<FakturType>(fileName);

      // 13.10.2022: 
      if(fakturType == null) return null;

      Faktur faktur_rec = fakturType.CreateFakturFromFakturSer();

      //faktur_rec.TurnNullValuesToEmptyString();

      return faktur_rec;
   }


   #endregion Overriders And Specifics

   #region Propertiz

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

   #region Kune Backup Values

   public decimal Skn_ukKC   { get; set; }
   public decimal Skn_ukRbt1 
   {
      get { return this.TheEx.Skn_ukRbt1; }
      set {        this.TheEx.Skn_ukRbt1 = value; }
   }
   public decimal Skn_ukKCR  
   {
      get { return this.TheEx.Skn_ukKCR; }
      set {        this.TheEx.Skn_ukKCR = value; }
   }
   public decimal Skn_ukKCRP 
   {
      get { return this.TheEx.Skn_ukKCRP; }
      set {        this.TheEx.Skn_ukKCRP = value; }
   }

   #endregion Kune Backup Values

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

   public string DokDate_Full__AsString             { get { return this.DokDate      .ToString(ZXC.VvDateFormat); } }
   public string DokDate_Month_AsString             { get { return this.DokDate.Month.ToString("00")            ; } }
   public string DokDate_Year_AsString              { get { return this.DokDate.Year .ToString("0000")          ; } }
   public string DokDate_Year_LastDigit             { get { return this.DokDate_Year_AsString.SubstringSafe(3, 1);} }
   public string DDMM                               { get { return this.DokDate.Day.ToString() + "." + DokDate.Month.ToString() + "."; } }
   public string DokDate_DDMMYY                     { get { return this.DokDate      .ToString(ZXC.VvDateDdMmYyFormat); } }


   public string DokDate_Full__AsString_And_NacPlac { get { return this.DokDate_Full__AsString  + "/" + this.NacPlac; } }
   public string DokDate_Month_AsString_And_NacPlac { get { return this.DokDate_Month_AsString  + "/" + this.NacPlac; } }

   public string SKL_DokDate_Full__AsString             { get { return this.SkladCD                + "-" + DokDate_Full__AsString; } }
   public string SKL_DokDate_Month_AsString             { get { return this.SkladCD                + "-" + DokDate_Month_AsString; } }
   public string DokDate_Month_SKL_AsString             { get { return this.DokDate_Month_AsString + "-" + SkladCD;                } }

   public string SKL_DokDate_Full__AsString_And_NacPlac { get { return this.SkladCD + "-" + DokDate_Full__AsString_And_NacPlac; } }
   public string SKL_DokDate_Month_AsString_And_NacPlac { get { return this.SkladCD + "-" + DokDate_Month_AsString_And_NacPlac; } }

   /* 07 */ public string TT
   {
      get { return this.currentData._tt; }
      set {        this.currentData._tt = value; }
   }
   public string TTopis { get { return ZXC.luiListaFakturType.GetNameForThisCd(this.TT); } }

   /* 08 */ public uint TtNum
   {
      get { return this.currentData._ttNum; }
      set {        this.currentData._ttNum = value; }
   }
   /* 09 */ public Int16 TtSort
   {
      get { return this.currentData._ttSort; }
      set {        this.currentData._ttSort = value; }
   }

   /* 10 */ public string SkladCD
   {
      get { return this.currentData._skladCD; }
      set {        this.currentData._skladCD = value; }
   }
   public string SkladName  { get { return ZXC.luiListaSkladista.GetNameForThisCd(this.SkladCD ); } }

   /* 11 */ public string SkladCD2
   {
      get { return this.currentData._skladCD2; }
      set {        this.currentData._skladCD2 = value; }
   }
   public string Sklad2Name { get { return ZXC.luiListaSkladista.GetNameForThisCd(this.SkladCD2); } }

   /* 12 */ public string VezniDok
   {
      get { return this.currentData._vezniDok; }
      set {        this.currentData._vezniDok = value; }
   }
   /* 13 */ public string Napomena
   {
      get { return this.currentData._napomena; }
      set {        this.currentData._napomena = value; }
   }
   /* 14 */ public string Opis
   {
      get { return this.currentData._opis; }
      set {        this.currentData._opis = value; }
   }

   /* 15 */ public string Konto
   {
      get { return this.currentData._konto; }
      set {        this.currentData._konto = value; }
   }
   /* 16 */ public string ProjektCD     
   {
      get { return this.currentData._projektCD; }
      set {        this.currentData._projektCD = value; }
   }
   /* 17 */ public string V1_tt       
   {
      get { return this.currentData._v1_tt; }
      set {        this.currentData._v1_tt = value; }
   }
   /* 18 */ public uint   V1_ttNum
   {
      get { return this.currentData._v1_ttNum; }
      set {        this.currentData._v1_ttNum = value; }
   }
   /* 19 */ public string V2_tt
   {
      get { return this.currentData._v2_tt; }
      set {        this.currentData._v2_tt = value; }
   }
   /* 20 */ public uint   V2_ttNum
   {  
      get { return this.currentData._v2_ttNum; }
      set {        this.currentData._v2_ttNum = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 21 */ public decimal S_ukKC
   {
      get { return this.currentData._s_ukKC; }
      set {        this.currentData._s_ukKC = value; }
   }
   /* 22 */ public decimal S_ukK
   {
      get { return this.currentData._s_ukK; }
      set {        this.currentData._s_ukK = value; }
   }
   /* 23 */ public uint S_ukTrnCount
   {
      get { return this.currentData._s_trnCount; }
      set {        this.currentData._s_trnCount = value; }
   }
   /* 24 */ public string OsobaX
   {
      get { return this.currentData._osobaX; }
      set {        this.currentData._osobaX = value; }
   }
   /* 25 */ public DateTime DokDate2
   {
      get { return this.currentData._dokDate2; }
      set {        this.currentData._dokDate2 = value; }
   }
   /* 26 */ public decimal S_ukK2
   {
      get { return this.currentData._s_ukK2; }
      set {        this.currentData._s_ukK2 = value; }
   }

   /* 27 */ public decimal Decimal01 { get { return this.currentData._decimal01; } set { this.currentData._decimal01 = value; } }
   /* 28 */ public decimal Decimal02 { get { return this.currentData._decimal02; } set { this.currentData._decimal02 = value; } }

   public DateTime DokDate2Safe            { get { return this.DokDate2.            IsEmpty() ? this.DokDate             : this.DokDate2            ; } }
   public DateTime BackupData_DokDate2Safe { get { return this.BackupData._dokDate2.IsEmpty() ? this.BackupData._dokDate : this.BackupData._dokDate2; } }

   #endregion Data Layer Columns

   #region Ext Data Layer Columns

   /* 01 */ public uint FakturRecID
   {
      get { return this.TheEx.currentData._fakturRecID; }
      set {        this.TheEx.currentData._fakturRecID = value; }
   }
   /* 02 */ public uint PdvNum
   {
      get { return this.TheEx.currentData._pdvNum; }
      set {        this.TheEx.currentData._pdvNum = value; }
   }
   /* 03 */ public DateTime PdvDate
   {
      get { return this.TheEx.currentData._pdvDate; }
      set {        this.TheEx.currentData._pdvDate = value; }
   }
   /* 04 */ public uint KupdobCD
   {
      get { return this.TheEx.currentData._kupdobCD; }
      set {        this.TheEx.currentData._kupdobCD = value; }
   }
   /* 05 */ public string KupdobTK
   {
      get { return this.TheEx.currentData._kupdobTK; }
      set {        this.TheEx.currentData._kupdobTK = value; }
   }
   /* 06 */ public string KupdobName
   {
      get { return this.TheEx.currentData._kupdobName; }
      set {        this.TheEx.currentData._kupdobName = value; }
   }
   /* 07 */ public string KdUlica
   {
      get { return this.TheEx.currentData._kdUlica; }
      set {        this.TheEx.currentData._kdUlica = value; }
   }
   /* 08 */ public string KdMjesto
   {
      get { return this.TheEx.currentData._kdMjesto; }
      set {        this.TheEx.currentData._kdMjesto = value; }
   }
   /* 09 */ public string KdZip
   {
      get { return this.TheEx.currentData._kdZip; }
      set {        this.TheEx.currentData._kdZip = value; }
   }
   public static string GetAdresa(string ulica, string zip, string grad)
   {
      string zarez  = ((zip + grad).NotEmpty() ? ", " : "");
      string razmak = ((      grad).NotEmpty() ? " "  : "");
      return (ulica + zarez + zip + razmak + grad);
   }
   public string KdAdresa
   {
      get { return GetAdresa(this.TheEx.currentData._kdUlica, this.TheEx.currentData._kdZip, this.TheEx.currentData._kdMjesto); }
   }
   /* 10 */ public string KdOib
   {
      get { return this.TheEx.currentData._kdOib; }
      set {        this.TheEx.currentData._kdOib = value; }
   }
   /* 11 */ public uint PosJedCD
   {
      get { return this.TheEx.currentData._posJedCD; }
      set {        this.TheEx.currentData._posJedCD = value; }
   }
   /* 12 */ public string PosJedTK
   {
      get { return this.TheEx.currentData._posJedTK; }
      set {        this.TheEx.currentData._posJedTK = value; }
   }
   /* 13 */ public string PosJedName
   {
      get { return this.TheEx.currentData._posJedName; }
      set {        this.TheEx.currentData._posJedName = value; }
   }
   /* 14 */ public string PosJedUlica
   {
      get { return this.TheEx.currentData._posJedUlica; }
      set {        this.TheEx.currentData._posJedUlica = value; }
   }
   /* 15 */ public string PosJedMjesto
   {
      get { return this.TheEx.currentData._posJedMjesto; }
      set {        this.TheEx.currentData._posJedMjesto = value; }
   }
   /* 16 */ public string PosJedZip
   {
      get { return this.TheEx.currentData._posJedZip; }
      set {        this.TheEx.currentData._posJedZip = value; }
   }
   public string PosJedAdresa
   {
      get { return GetAdresa(this.TheEx.currentData._posJedUlica, this.TheEx.currentData._posJedZip, this.TheEx.currentData._posJedMjesto); }
   }
   /* 17 */ public string VezniDok2
   {
      get { return this.TheEx.currentData._vezniDok2; }
      set {        this.TheEx.currentData._vezniDok2 = value; }
   }
   /* 18 */ public string Fco
   {
      get { return this.TheEx.currentData._fco; }
      set {        this.TheEx.currentData._fco = value; }
   }
   /* 19 */ public int RokPlac
   {
      get { return this.TheEx.currentData._rokPlac; }
      set {        this.TheEx.currentData._rokPlac = value; }
   }
   /* 20 */ public DateTime DospDate
   {
      get { return this.TheEx.currentData._dospDate; }
      set {        this.TheEx.currentData._dospDate = value; }
   }
   /* 21 */ public DateTime SkladDate
   {
      get { return this.TheEx.currentData._skladDate; }
      set {        this.TheEx.currentData._skladDate = value; }
   }
   /* 22 */ public string NacPlac
   {
      get { return this.TheEx.currentData._nacPlac; }
      set {        this.TheEx.currentData._nacPlac = value; }
   }
   /* 23 */ public string ZiroRn
   {
      get { return this.TheEx.currentData._ziroRn; }
      set {        this.TheEx.currentData._ziroRn = value; }
   }
   /* 24 */ public string DevName
   {
      get 
      { 
         if(TheEx == null) return "";
         else              return this.TheEx.currentData._devName;
      }
      set {        this.TheEx.currentData._devName = value; }
   }
   public string CurrencyID
   {
      get 
      { 
         if(DevName.IsEmpty()) return /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum.ToString();
         else                  return DevName;
      }
   }
   public ZXC.ValutaNameEnum DevNameAsEnum
   {
      get
      {
         if(DevName.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                  return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), DevName, true);
      }
   }
   public decimal DevTecaj    { get { return ZXC.DevTecDao.GetHnbTecaj(this.DevNameAsEnum, this.DokDate); } }

   /* 25 */ public string PnbM
   {
      get { return this.TheEx.currentData._pnbM; }
      set {        this.TheEx.currentData._pnbM = value; }
   }
   /* 26 */ public string PnbV
   {
      get { return this.TheEx.currentData._pnbV; }
      set {        this.TheEx.currentData._pnbV = value; }
   }
   /* 27 */ public uint PersonCD
   {
      get { return this.TheEx.currentData._personCD; }
      set {        this.TheEx.currentData._personCD = value; }
   }
   /* 28 */ public string PersonName
   {
      get { return this.TheEx.currentData._personName; }
      set {        this.TheEx.currentData._personName = value; }
   }
   /* 29 */ public string Napomena2
   {
      get { return this.TheEx.currentData._napomena2; }
      set {        this.TheEx.currentData._napomena2 = value; }
   }
   /* 30 */ public string CjenikTT
   {
      get { return this.TheEx.currentData._cjenikTT; }
      set {        this.TheEx.currentData._cjenikTT = value; }
   }
   /* 31 */ public string StatusCD
   {
      get { return this.TheEx.currentData._statusCD; }
      set {        this.TheEx.currentData._statusCD = value; }
   }
   /* 32 */ public int RokPonude
   {
      get { return this.TheEx.currentData._rokPonude; }
      set {        this.TheEx.currentData._rokPonude = value; }
   }
   /* 33 */ public DateTime PonudDate
   {
      get { return this.TheEx.currentData._ponudDate; }
      set {        this.TheEx.currentData._ponudDate = value; }
   }
   /* 34 */ public uint MtrosCD
   {
      get { return this.TheEx.currentData._mtrosCD; }
      set {        this.TheEx.currentData._mtrosCD = value; }
   }
   /* 35 */ public string MtrosTK
   {
      get { return this.TheEx.currentData._mtrosTK; }
      set {        this.TheEx.currentData._mtrosTK = value; }
   }
   /* 36 */ public string MtrosName
   {
      get { return this.TheEx.currentData._mtrosName; }
      set {        this.TheEx.currentData._mtrosName = value; }
   }
   /* 37 */ public uint PrimPlatCD
   {
      get { return this.TheEx.currentData._primPlatCD; }
      set {        this.TheEx.currentData._primPlatCD = value; }
   }
   /* 38 */ public string PrimPlatTK
   {
      get { return this.TheEx.currentData._primPlatTK; }
      set {        this.TheEx.currentData._primPlatTK = value; }
   }
   /* 39 */ public string PrimPlatName
   {
      get { return this.TheEx.currentData._primPlatName; }
      set {        this.TheEx.currentData._primPlatName = value; }
   }
   /* 40 */ public ZXC.PdvKnjigaEnum PdvKnjiga
   {
      get { return (ZXC.PdvKnjigaEnum)this.TheEx.currentData._pdvKnjiga; }
      set {                           this.TheEx.currentData._pdvKnjiga = (ushort)value; }
   }
   /*    */ public ushort PdvKnjiga_u
   {
      get { return                    this.TheEx.currentData._pdvKnjiga; }
      set {                           this.TheEx.currentData._pdvKnjiga =         value; }
   }
   /* 41 */ public bool  IsNpCash
   {
      get { return                  this.TheEx.currentData._isNpCash; }
      set {                         this.TheEx.currentData._isNpCash =         value; }
   }
   /* 42 */ public ZXC.PdvR12Enum PdvR12
   {
      get { return (ZXC.PdvR12Enum)this.TheEx.currentData._pdvR12; }
      set {                        this.TheEx.currentData._pdvR12 = (ushort)value; }
   }
   /*    */ public ushort PdvR12_u
   {
      get { return                 this.TheEx.currentData._pdvR12; }
      set {                        this.TheEx.currentData._pdvR12 =         value; }
   }

   // 22.11.2018: bio puse/fuse pa poceli koristiti za VvUBL_PolsProc 
 //   43    public ZXC.PdvKolTipEnum PdvKolTip
 //{
 //   get { return (ZXC.PdvKolTipEnum)this.TheEx.currentData._pdvKolTip; }
 //   set {                           this.TheEx.currentData._pdvKolTip = (ushort)value; }
 //}
   /* 43 */ public ZXC.VvUBL_PolsProc PdvKolTip
   {
      get { return (ZXC.VvUBL_PolsProc)this.TheEx.currentData._pdvKolTip; }
      set {                            this.TheEx.currentData._pdvKolTip = (ushort)value; }
   }
   /*    */ public ushort PdvKolTip_u
   {
      get { return                    this.TheEx.currentData._pdvKolTip; }
      set {                           this.TheEx.currentData._pdvKolTip =         value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 44 */ public decimal S_ukKCRP
   {
      get { if(TheEx == null) return S_ukKC; else return this.TheEx.currentData._s_ukKCRP; }
      set {                                              this.TheEx.currentData._s_ukKCRP = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 45 */ public decimal S_ukKCRM
   {
      get { if(TheEx == null) return S_ukKC; else return this.TheEx.currentData._s_ukKCRM; }
      set {                                              this.TheEx.currentData._s_ukKCRM = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 46 */ public decimal S_ukKCR
   {
      // 29.04.2016: 
    //get { if(TheEx == null || TheEx.currentData._s_ukKCR.IsZero()) return S_ukKC; else return this.TheEx.currentData._s_ukKCR; }
      get 
      {

         // 05.07.2019: dodjosmo ode i cudom se cudimo za koji li je k ikada ovo bilo ovako, 
         // tj. zasto umjesto kcr-a vracamo kc ajko je ovaj nula?
         // calc bi trebao voditi brigu o tocnim izracunima kc-a i kcr-a 
         // as ovo stvara BUG kada imas, say, zamjenu artikala, sKCR je nula a jedna stavka ima a druge nema RABAT! 
         // ... pa cemo ovo sada ugasiti i unormaliti da radi kao i npr 'S_ukKC'
       //if(TheEx == null || TheEx.currentData._s_ukKCR.IsZero())
       //{
       //   if(TtInfo.IsMalopTT) return 0M;
       //   else                 return S_ukKC;
       //}

         if(TheEx == null) return this.      currentData._s_ukKC ;
         else              return this.TheEx.currentData._s_ukKCR;
      }
      set {                                                                                     this.TheEx.currentData._s_ukKCR = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 47 */ public decimal S_ukRbt1
   {
      get { return this.TheEx.currentData._s_ukRbt1; }
      set {        this.TheEx.currentData._s_ukRbt1 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 48 */ public decimal S_ukRbt2
   {
      get { return this.TheEx.currentData._s_ukRbt2; }
      set {        this.TheEx.currentData._s_ukRbt2 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 49 */ public decimal S_ukZavisni
   {
      get { return this.TheEx.currentData._s_ukZavisni; }
      set {        this.TheEx.currentData._s_ukZavisni = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 50 */ public decimal S_ukProlazne
   {
      get { return this.TheEx.currentData._s_ukProlazne; }
      set {        this.TheEx.currentData._s_ukProlazne = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 51 */ public decimal S_ukPdv23m
   {
      get { return this.TheEx.currentData._s_ukPdv23m; }
      set {        this.TheEx.currentData._s_ukPdv23m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 52 */ public decimal S_ukPdv23n
   {
      get { return this.TheEx.currentData._s_ukPdv23n; }
      set {        this.TheEx.currentData._s_ukPdv23n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 53 */ public decimal S_ukPdv22m
   {
      get { return this.TheEx.currentData._s_ukPdv22m; }
      set {        this.TheEx.currentData._s_ukPdv22m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 54 */ public decimal S_ukPdv22n
   {
      get { return this.TheEx.currentData._s_ukPdv22n; }
      set {        this.TheEx.currentData._s_ukPdv22n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 55 */ public decimal S_ukPdv10m
   {
      get { return this.TheEx.currentData._s_ukPdv10m; }
      set {        this.TheEx.currentData._s_ukPdv10m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 56 */ public decimal S_ukPdv10n
   {
      get { return this.TheEx.currentData._s_ukPdv10n; }
      set {        this.TheEx.currentData._s_ukPdv10n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 57 */ public decimal S_ukOsn23m
   {
      get { return this.TheEx.currentData._s_ukOsn23m; }
      set {        this.TheEx.currentData._s_ukOsn23m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 58 */ public decimal S_ukOsn23n
   {
      get { return this.TheEx.currentData._s_ukOsn23n; }
      set {        this.TheEx.currentData._s_ukOsn23n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 59 */ public decimal S_ukOsn22m
   {
      get { return this.TheEx.currentData._s_ukOsn22m; }
      set {        this.TheEx.currentData._s_ukOsn22m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 60 */ public decimal S_ukOsn22n
   {
      get { return this.TheEx.currentData._s_ukOsn22n; }
      set {        this.TheEx.currentData._s_ukOsn22n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 61 */ public decimal S_ukOsn10m
   {
      get { return this.TheEx.currentData._s_ukOsn10m; }
      set {        this.TheEx.currentData._s_ukOsn10m = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 62 */ public decimal S_ukOsn10n
   {
      get { return this.TheEx.currentData._s_ukOsn10n; }
      set {        this.TheEx.currentData._s_ukOsn10n = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 63 */ public decimal S_ukOsn0
   {
      get { return this.TheEx.currentData._s_ukOsn0; }
      set {        this.TheEx.currentData._s_ukOsn0 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 64 */ public decimal S_ukOsnPr
   {
      get { return this.TheEx.currentData._s_ukOsnPr; }
      set {        this.TheEx.currentData._s_ukOsnPr = value; }
   }
   /* 65 */ public string OpciAlabel
   {
      get { return this.TheEx.currentData._opciAlabel; }
      set {        this.TheEx.currentData._opciAlabel = value; }
   }
   /* 66 */ public string OpciAvalue
   {
      get { return this.TheEx.currentData._opciAvalue; }
      set {        this.TheEx.currentData._opciAvalue = value; }
   }
   /* 67 */ public string OpciBlabel
   {
      get { return this.TheEx.currentData._opciBlabel; }
      set {        this.TheEx.currentData._opciBlabel = value; }
   }
   /* 68 */ public string OpciBvalue
   {
      get { return this.TheEx.currentData._opciBvalue; }
      set {        this.TheEx.currentData._opciBvalue = value; }
   }
   /* 69 */ public uint OdgvPersCD  
   {
      get { return this.TheEx.currentData._odgvPersCD; }
      set {        this.TheEx.currentData._odgvPersCD = value; }
   }
   /* 70 */ public string   OdgvPersName
   {
      get { return this.TheEx.currentData._odgvPersName; }
      set {        this.TheEx.currentData._odgvPersName = value; }
   }
   /* 71 */ public uint     CjenTTnum
   {
      get { return this.TheEx.currentData._cjenTTnum; }
      set {        this.TheEx.currentData._cjenTTnum = value; }
   }
   /* 72 */ public string V3_tt       
   {
      get { return this.TheEx.currentData._v3_tt; }
      set {        this.TheEx.currentData._v3_tt = value; }
   }
   /* 73 */ public uint   V3_ttNum
   {
      get { return this.TheEx.currentData._v3_ttNum; }
      set {        this.TheEx.currentData._v3_ttNum = value; }
   }
   /* 74 */ public string V4_tt
   {
      get { return this.TheEx.currentData._v4_tt; }
      set {        this.TheEx.currentData._v4_tt = value; }
   }
   /* 75 */ public uint   V4_ttNum
   {
      get { return this.TheEx.currentData._v4_ttNum; }
      set {        this.TheEx.currentData._v4_ttNum = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 76 */ public decimal S_ukMrz
   {
      get { return this.TheEx.currentData._s_ukMrz; }
      set {        this.TheEx.currentData._s_ukMrz = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 77 */ public decimal S_ukPdv
   {
      get { return this.TheEx.currentData._s_ukPdv; }
      set {        this.TheEx.currentData._s_ukPdv = value; }
   }

   /* 78 */ public string TipOtpreme
   {
      get { return this.TheEx.currentData._tipOtpreme; }
      set {        this.TheEx.currentData._tipOtpreme = value; }
   }
   /* 79 */ public int RokIsporuke
   {
      get { return this.TheEx.currentData._rokIsporuke; }
      set {        this.TheEx.currentData._rokIsporuke = value; }
   }
   /* 80 */ public DateTime RokIspDate
   {
      get { return this.TheEx.currentData._rokIspDate; }
      set {        this.TheEx.currentData._rokIspDate = value; }
   }

   /* 81 */ public string DostName
   {
      get { return this.TheEx.currentData._dostName; }
      set {        this.TheEx.currentData._dostName = value; }
   }

   /* 82 */ public string DostAddr
   {
      get { return this.TheEx.currentData._dostAddr; }
      set {        this.TheEx.currentData._dostAddr = value; }
   }

   public string S_ukKCRP_AsText
   {
      get { return ZXC.KuneIlipe(this.S_ukKCRP); }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 83 */ public decimal S_ukOsn07
   {
      get { return this.TheEx.currentData._s_ukOsn07; }
      set {        this.TheEx.currentData._s_ukOsn07 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 84 */ public decimal S_ukOsn08
   {
      get { return this.TheEx.currentData._s_ukOsn08; }
      set {        this.TheEx.currentData._s_ukOsn08 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 85 */ public decimal S_ukOsn09
   {
      get { return this.TheEx.currentData._s_ukOsn09; }
      set {        this.TheEx.currentData._s_ukOsn09 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 86 */ public decimal S_ukOsn10
   {
      get { return this.TheEx.currentData._s_ukOsn10; }
      set {        this.TheEx.currentData._s_ukOsn10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 87 */ public decimal S_ukOsn11
   {
      get { return this.TheEx.currentData._s_ukOsn11; }
      set {        this.TheEx.currentData._s_ukOsn11 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 88 */ public decimal S_ukOsnUr23
   {
      get { return this.TheEx.currentData._s_ukOsnUr23; }
      set {        this.TheEx.currentData._s_ukOsnUr23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 89 */ public decimal S_ukOsnUu10
   {
      get { return this.TheEx.currentData._s_ukOsnUu10; }
      set {        this.TheEx.currentData._s_ukOsnUu10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 90 */ public decimal S_ukOsnUu22
   {
      get { return this.TheEx.currentData._s_ukOsnUu22; }
      set {        this.TheEx.currentData._s_ukOsnUu22 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 91 */ public decimal S_ukOsnUu23
   {
      get { return this.TheEx.currentData._s_ukOsnUu23; }
      set {        this.TheEx.currentData._s_ukOsnUu23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 92 */ public decimal S_ukPdvUr23
   {
      get { return this.TheEx.currentData._s_ukPdvUr23; }
      set {        this.TheEx.currentData._s_ukPdvUr23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 93 */ public decimal S_ukPdvUu10
   {
      get { return this.TheEx.currentData._s_ukPdvUu10; }
      set {        this.TheEx.currentData._s_ukPdvUu10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 94 */ public decimal S_ukPdvUu22
   {
      get { return this.TheEx.currentData._s_ukPdvUu22; }
      set {        this.TheEx.currentData._s_ukPdvUu22 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 95 */ public decimal S_ukPdvUu23
   {
      get { return this.TheEx.currentData._s_ukPdvUu23; }
      set {        this.TheEx.currentData._s_ukPdvUu23 = value; }
   }

   /* 96 */ public ushort CarinaKind_u
   {
      get { return this.TheEx.currentData._carinaKind; }
      set {        this.TheEx.currentData._carinaKind = value; }
   }

   /// <summary>
   /// Bivsi naziv: "CarinaKind"
   /// </summary>
   public ZXC.ShouldFak2NalEnum ShouldFak2Nal
   {
      get { return (ZXC.ShouldFak2NalEnum)this.TheEx.currentData._carinaKind; }
      set {                               this.TheEx.currentData._carinaKind = (ushort)value; }
   }

   /* 97 */ public string PrjArtCD
   {
      get { return this.TheEx.currentData._prjArtCD; }
      set {        this.TheEx.currentData._prjArtCD = value; }
   }
   /* 98 */ public string PrjArtName
   {
      get { return this.TheEx.currentData._prjArtName; }
      set {        this.TheEx.currentData._prjArtName = value; }
   }

   /* 99 */ public string ExternLink1
   {
      get { return this.TheEx.currentData._externLink1; }
      set {        this.TheEx.currentData._externLink1 = value; }
   }
   /*100 */ public string ExternLink2
   {
      get { return this.TheEx.currentData._externLink2; }
      set {        this.TheEx.currentData._externLink2 = value; }
   }

   /*101 */ public decimal SomeMoney
   {
      get { return this.TheEx.currentData._someMoney; }
      set {        this.TheEx.currentData._someMoney = value; }
   }

   /*102 */ public decimal SomePercent
   {
      get { return this.TheEx.currentData._somePercent; }
      set {        this.TheEx.currentData._somePercent = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 103 */ public decimal S_ukMskPdv10 { 
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukMskPdv10; } }
      set {                                                this.TheEx.currentData._s_ukMskPdv10 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 104 */ public decimal S_ukMskPdv23 {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukMskPdv23; } }
      set {                                                this.TheEx.currentData._s_ukMskPdv23 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 105 */ public decimal S_ukMSK_00 {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukMSK_00; } }
      set {                                                this.TheEx.currentData._s_ukMSK_00 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 106 */ public decimal S_ukMSK_10 {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukMSK_10; } }
      set {                                                this.TheEx.currentData._s_ukMSK_10 = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 107 */ public decimal S_ukMSK_23 {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukMSK_23; } }
      set {                                                this.TheEx.currentData._s_ukMSK_23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 108 */ public decimal S_ukKCR_usl {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukKCR_usl; } }
      set {                                                this.TheEx.currentData._s_ukKCR_usl = value; }
   }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 109 */ public decimal S_ukKCRP_usl {
      get { if(TheEx == null) { return 0M; } else { return this.TheEx.currentData._s_ukKCRP_usl; } }
      set {                                                this.TheEx.currentData._s_ukKCRP_usl = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 110 */ public decimal S_ukPdv25m   { get { return this.TheEx.currentData._s_ukPdv25m  ; } set { this.TheEx.currentData._s_ukPdv25m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 111 */ public decimal S_ukPdv25n   { get { return this.TheEx.currentData._s_ukPdv25n  ; } set { this.TheEx.currentData._s_ukPdv25n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 112 */ public decimal S_ukOsn25m   { get { return this.TheEx.currentData._s_ukOsn25m  ; } set { this.TheEx.currentData._s_ukOsn25m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 113 */ public decimal S_ukOsn25n   { get { return this.TheEx.currentData._s_ukOsn25n  ; } set { this.TheEx.currentData._s_ukOsn25n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 114 */ public decimal S_ukOsnUr25  { get { return this.TheEx.currentData._s_ukOsnUr25 ; } set { this.TheEx.currentData._s_ukOsnUr25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 115 */ public decimal S_ukOsnUu25  { get { return this.TheEx.currentData._s_ukOsnUu25 ; } set { this.TheEx.currentData._s_ukOsnUu25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 116 */ public decimal S_ukPdvUr25  { get { return this.TheEx.currentData._s_ukPdvUr25 ; } set { this.TheEx.currentData._s_ukPdvUr25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 117 */ public decimal S_ukPdvUu25  { get { return this.TheEx.currentData._s_ukPdvUu25 ; } set { this.TheEx.currentData._s_ukPdvUu25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 118 */ public decimal S_ukMskPdv25 { get { return this.TheEx.currentData._s_ukMskPdv25; } set { this.TheEx.currentData._s_ukMskPdv25 = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 119 */ public decimal S_ukMSK_25   { get { return this.TheEx.currentData._s_ukMSK_25  ; } set { this.TheEx.currentData._s_ukMSK_25   = value; }}

   /*120 */ public string FiskJIR   { get { return this.TheEx.currentData._fiskJIR  ; } set { this.TheEx.currentData._fiskJIR   = value; } }
   /*121 */ public string FiskZKI   { get { return this.TheEx.currentData._fiskZKI  ; } set { this.TheEx.currentData._fiskZKI   = value; } }
   /*122 */ public string FiskMsgID { get { return this.TheEx.currentData._fiskMsgID; } set { this.TheEx.currentData._fiskMsgID = value; } }
   /*123 */ public string FiskOibOp { get { return this.TheEx.currentData._fiskOibOp; } set { this.TheEx.currentData._fiskOibOp = value; } }
   /*124 */ public string FiskPrgBr { get { return this.TheEx.currentData._fiskPrgBr; } set { this.TheEx.currentData._fiskPrgBr = value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 105 */ public decimal S_ukPdv05m   { get { return this.TheEx.currentData._s_ukPdv05m  ; } set { this.TheEx.currentData._s_ukPdv05m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 126 */ public decimal S_ukPdv05n   { get { return this.TheEx.currentData._s_ukPdv05n  ; } set { this.TheEx.currentData._s_ukPdv05n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 127 */ public decimal S_ukOsn05m   { get { return this.TheEx.currentData._s_ukOsn05m  ; } set { this.TheEx.currentData._s_ukOsn05m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 128 */ public decimal S_ukOsn05n   { get { return this.TheEx.currentData._s_ukOsn05n  ; } set { this.TheEx.currentData._s_ukOsn05n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 129 */ public decimal S_ukMskPdv05 { get { return this.TheEx.currentData._s_ukMskPdv05; } set { this.TheEx.currentData._s_ukMskPdv05 = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 130 */ public decimal S_ukMSK_05   { get { return this.TheEx.currentData._s_ukMSK_05  ; } set { this.TheEx.currentData._s_ukMSK_05   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 131 */ public decimal S_ukOsnUr05  { get { return this.TheEx.currentData._s_ukOsnUr05 ; } set { this.TheEx.currentData._s_ukOsnUr05  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 132 */ public decimal S_ukPdvUr05  { get { return this.TheEx.currentData._s_ukPdvUr05 ; } set { this.TheEx.currentData._s_ukPdvUr05  = value; }}

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 133 */ public decimal S_pixK      { get { return this.TheEx.currentData._s_pixK     ; } set { this.TheEx.currentData._s_pixK    = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 134 */ public decimal S_puxK_P    { get { return this.TheEx.currentData._s_puxK_P   ; } set { this.TheEx.currentData._s_puxK_P  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 135 */ public decimal S_puxK_All  { get { return this.TheEx.currentData._s_puxK_All ; } set { this.TheEx.currentData._s_puxK_All  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 136 */ public decimal S_pixKC     { get { return this.TheEx.currentData._s_pixKC    ; } set { this.TheEx.currentData._s_pixKC   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 137 */ public decimal S_puxKC_P   { get { return this.TheEx.currentData._s_puxKC_P  ; } set { this.TheEx.currentData._s_puxKC_P = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 138 */ public decimal S_puxKC_All { get { return this.TheEx.currentData._s_puxKC_All; } set { this.TheEx.currentData._s_puxKC_All = value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 139 */ public decimal S_ukPpmvOsn   { get { return this.TheEx.currentData._s_ukPpmvOsn  ; } set { this.TheEx.currentData._s_ukPpmvOsn   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 140 */ public decimal S_ukPpmvSt1i2 { get { return this.TheEx.currentData._s_ukPpmvSt1i2; } set { this.TheEx.currentData._s_ukPpmvSt1i2 = value; } }

   /* 141 */ public DateTime DateX
   {
      get { return this.TheEx.currentData._dateX; }
      set {        this.TheEx.currentData._dateX = value; }
   }

   /*142 */ public string VatCntryCode
   {
      get { return this.TheEx.currentData._vatCntryCode; }
      set {        this.TheEx.currentData._vatCntryCode = value; }
   }

   /*143 */ public ZXC.PdvGEOkindEnum PdvGEOkind { get { return (ZXC.PdvGEOkindEnum)this.TheEx.currentData._pdvGEOkind; } set { this.TheEx.currentData._pdvGEOkind = (ushort)value; } }
   /*144 */ public ZXC.PdvZPkindEnum  PdvZPkind  { get { return (ZXC.PdvZPkindEnum) this.TheEx.currentData._pdvZPkind ; } set { this.TheEx.currentData._pdvZPkind  = (ushort)value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*145 */ public decimal S_ukOsnR25m_EU    { get { return this.TheEx.currentData._s_ukOsnR25m_EU; } set { this.TheEx.currentData._s_ukOsnR25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*146 */ public decimal S_ukOsnR25n_EU    { get { return this.TheEx.currentData._s_ukOsnR25n_EU; } set { this.TheEx.currentData._s_ukOsnR25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*147 */ public decimal S_ukOsnU25m_EU    { get { return this.TheEx.currentData._s_ukOsnU25m_EU; } set { this.TheEx.currentData._s_ukOsnU25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*148 */ public decimal S_ukOsnU25n_EU    { get { return this.TheEx.currentData._s_ukOsnU25n_EU; } set { this.TheEx.currentData._s_ukOsnU25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*149 */ public decimal S_ukOsnR10m_EU    { get { return this.TheEx.currentData._s_ukOsnR10m_EU; } set { this.TheEx.currentData._s_ukOsnR10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*150 */ public decimal S_ukOsnR10n_EU    { get { return this.TheEx.currentData._s_ukOsnR10n_EU; } set { this.TheEx.currentData._s_ukOsnR10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*151 */ public decimal S_ukOsnU10m_EU    { get { return this.TheEx.currentData._s_ukOsnU10m_EU; } set { this.TheEx.currentData._s_ukOsnU10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*152 */ public decimal S_ukOsnU10n_EU    { get { return this.TheEx.currentData._s_ukOsnU10n_EU; } set { this.TheEx.currentData._s_ukOsnU10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*153 */ public decimal S_ukOsnR05m_EU    { get { return this.TheEx.currentData._s_ukOsnR05m_EU; } set { this.TheEx.currentData._s_ukOsnR05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*154 */ public decimal S_ukOsnR05n_EU    { get { return this.TheEx.currentData._s_ukOsnR05n_EU; } set { this.TheEx.currentData._s_ukOsnR05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*155 */ public decimal S_ukOsnU05m_EU    { get { return this.TheEx.currentData._s_ukOsnU05m_EU; } set { this.TheEx.currentData._s_ukOsnU05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*156 */ public decimal S_ukOsnU05n_EU    { get { return this.TheEx.currentData._s_ukOsnU05n_EU; } set { this.TheEx.currentData._s_ukOsnU05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*157 */ public decimal S_ukOsn25m_BS     { get { return this.TheEx.currentData._s_ukOsn25m_BS ; } set { this.TheEx.currentData._s_ukOsn25m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*158 */ public decimal S_ukOsn25n_BS     { get { return this.TheEx.currentData._s_ukOsn25n_BS ; } set { this.TheEx.currentData._s_ukOsn25n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*159 */ public decimal S_ukOsn10m_BS     { get { return this.TheEx.currentData._s_ukOsn10m_BS ; } set { this.TheEx.currentData._s_ukOsn10m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*160 */ public decimal S_ukOsn10n_BS     { get { return this.TheEx.currentData._s_ukOsn10n_BS ; } set { this.TheEx.currentData._s_ukOsn10n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*161 */ public decimal S_ukOsn25m_TP     { get { return this.TheEx.currentData._s_ukOsn25m_TP ; } set { this.TheEx.currentData._s_ukOsn25m_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*162 */ public decimal S_ukOsn25n_TP     { get { return this.TheEx.currentData._s_ukOsn25n_TP ; } set { this.TheEx.currentData._s_ukOsn25n_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*163 */ public decimal S_ukPdvR25m_EU    { get { return this.TheEx.currentData._s_ukPdvR25m_EU; } set { this.TheEx.currentData._s_ukPdvR25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*164 */ public decimal S_ukPdvR25n_EU    { get { return this.TheEx.currentData._s_ukPdvR25n_EU; } set { this.TheEx.currentData._s_ukPdvR25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*165 */ public decimal S_ukPdvU25m_EU    { get { return this.TheEx.currentData._s_ukPdvU25m_EU; } set { this.TheEx.currentData._s_ukPdvU25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*166 */ public decimal S_ukPdvU25n_EU    { get { return this.TheEx.currentData._s_ukPdvU25n_EU; } set { this.TheEx.currentData._s_ukPdvU25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*167 */ public decimal S_ukPdvR10m_EU    { get { return this.TheEx.currentData._s_ukPdvR10m_EU; } set { this.TheEx.currentData._s_ukPdvR10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*168 */ public decimal S_ukPdvR10n_EU    { get { return this.TheEx.currentData._s_ukPdvR10n_EU; } set { this.TheEx.currentData._s_ukPdvR10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*169 */ public decimal S_ukPdvU10m_EU    { get { return this.TheEx.currentData._s_ukPdvU10m_EU; } set { this.TheEx.currentData._s_ukPdvU10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*170 */ public decimal S_ukPdvU10n_EU    { get { return this.TheEx.currentData._s_ukPdvU10n_EU; } set { this.TheEx.currentData._s_ukPdvU10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*171 */ public decimal S_ukPdvR05m_EU    { get { return this.TheEx.currentData._s_ukPdvR05m_EU; } set { this.TheEx.currentData._s_ukPdvR05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*172 */ public decimal S_ukPdvR05n_EU    { get { return this.TheEx.currentData._s_ukPdvR05n_EU; } set { this.TheEx.currentData._s_ukPdvR05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*173 */ public decimal S_ukPdvU05m_EU    { get { return this.TheEx.currentData._s_ukPdvU05m_EU; } set { this.TheEx.currentData._s_ukPdvU05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*174 */ public decimal S_ukPdvU05n_EU    { get { return this.TheEx.currentData._s_ukPdvU05n_EU; } set { this.TheEx.currentData._s_ukPdvU05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*175 */ public decimal S_ukPdv25m_BS     { get { return this.TheEx.currentData._s_ukPdv25m_BS ; } set { this.TheEx.currentData._s_ukPdv25m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*176 */ public decimal S_ukPdv25n_BS     { get { return this.TheEx.currentData._s_ukPdv25n_BS ; } set { this.TheEx.currentData._s_ukPdv25n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*177 */ public decimal S_ukPdv10m_BS     { get { return this.TheEx.currentData._s_ukPdv10m_BS ; } set { this.TheEx.currentData._s_ukPdv10m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*178 */ public decimal S_ukPdv10n_BS     { get { return this.TheEx.currentData._s_ukPdv10n_BS ; } set { this.TheEx.currentData._s_ukPdv10n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*179 */ public decimal S_ukPdv25m_TP     { get { return this.TheEx.currentData._s_ukPdv25m_TP ; } set { this.TheEx.currentData._s_ukPdv25m_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*180 */ public decimal S_ukPdv25n_TP     { get { return this.TheEx.currentData._s_ukPdv25n_TP ; } set { this.TheEx.currentData._s_ukPdv25n_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*181 */ public decimal S_ukOsnZP_11      { get { return this.TheEx.currentData._s_ukOsnZP_11  ; } set { this.TheEx.currentData._s_ukOsnZP_11   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*182 */ public decimal S_ukOsnZP_12      { get { return this.TheEx.currentData._s_ukOsnZP_12  ; } set { this.TheEx.currentData._s_ukOsnZP_12   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*183 */ public decimal S_ukOsnZP_13      { get { return this.TheEx.currentData._s_ukOsnZP_13  ; } set { this.TheEx.currentData._s_ukOsnZP_13   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*184 */ public decimal S_ukOsn12         { get { return this.TheEx.currentData._s_ukOsn12     ; } set { this.TheEx.currentData._s_ukOsn12      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*185 */ public decimal S_ukOsn13         { get { return this.TheEx.currentData._s_ukOsn13     ; } set { this.TheEx.currentData._s_ukOsn13      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*186 */ public decimal S_ukOsn14         { get { return this.TheEx.currentData._s_ukOsn14     ; } set { this.TheEx.currentData._s_ukOsn14      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*187 */ public decimal S_ukOsn15         { get { return this.TheEx.currentData._s_ukOsn15     ; } set { this.TheEx.currentData._s_ukOsn15      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*188 */ public decimal S_ukOsn16         { get { return this.TheEx.currentData._s_ukOsn16     ; } set { this.TheEx.currentData._s_ukOsn16      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*189 */ public decimal S_ukOsnPNP        { get { return this.TheEx.currentData._s_ukOsnPNP    ; } set { this.TheEx.currentData._s_ukOsnPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*190 */ public decimal S_ukIznPNP        { get { return this.TheEx.currentData._s_ukIznPNP    ; } set { this.TheEx.currentData._s_ukIznPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*191 */ public decimal S_ukMskPNP        { get { return this.TheEx.currentData._s_ukMskPNP    ; } set { this.TheEx.currentData._s_ukMskPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*192 */ public decimal Skiz_ukKC         { get { return this.TheEx.currentData._skiz_ukKC     ; } set { this.TheEx.currentData._skiz_ukKC      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*193 */ public decimal Skiz_ukKCR        { get { return this.TheEx.currentData._skiz_ukKCR    ; } set { this.TheEx.currentData._skiz_ukKCR     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*194 */ public decimal Skiz_ukRbt1       { get { return this.TheEx.currentData._skiz_ukRbt1   ; } set { this.TheEx.currentData._skiz_ukRbt1    = value; } }

   // pazi, u data layeru si krenuo u krivom smjeru pa se ovaj property tam zove 's_ukKCRP_NP2' 
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*195 */ public decimal S_ukKCRP_NP1      { get { return this.TheEx.currentData._s_ukKCRP_NP2  ; } set { this.TheEx.currentData._s_ukKCRP_NP2   = value; } }
                                                      /*196 */ public string  NacPlac2          { get { return this.TheEx.currentData._nacPlac2      ; } set { this.TheEx.currentData._nacPlac2       = value; } }

   /* 197 */ public bool  IsNpCash2
   {
      get { return                  this.TheEx.currentData._isNpCash2; }
      set {                         this.TheEx.currentData._isNpCash2 =         value; }
   }

   #endregion Data Layer Columns

   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   #region 2013 EU PDV NEWS
                                   
   public decimal TrnSum_OsnR25m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25m && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnR25n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25n && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU25m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25m && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU25n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25n && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnR10m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10m && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnR10n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10n && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU10m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10m && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU10n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10n && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnR05m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05m && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnR05n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05n && rtr.T_isIrmUsluga == false).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU05m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05m && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnU05n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05n && rtr.T_isIrmUsluga == true ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
                                    
   public decimal TrnSum_PdvR25m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25m && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvR25n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25n && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU25m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25m && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU25n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25n && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvR10m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10m && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvR10n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10n && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU10m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10m && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU10n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10n && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvR05m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05m && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvR05n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05n && rtr.T_isIrmUsluga == false).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU05m    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05m && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_PdvU05n    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05n && rtr.T_isIrmUsluga == true ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }

   public decimal TrnSum_OsnR25m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR25m : 0.00M); } }
   public decimal TrnSum_OsnR25n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR25n : 0.00M); } }
   public decimal TrnSum_OsnU25m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU25m : 0.00M); } }
   public decimal TrnSum_OsnU25n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU25n : 0.00M); } }
   public decimal TrnSum_OsnR10m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR10m : 0.00M); } }
   public decimal TrnSum_OsnR10n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR10n : 0.00M); } }
   public decimal TrnSum_OsnU10m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU10m : 0.00M); } }
   public decimal TrnSum_OsnU10n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU10n : 0.00M); } }
   public decimal TrnSum_OsnR05m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR05m : 0.00M); } }
   public decimal TrnSum_OsnR05n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnR05n : 0.00M); } }
   public decimal TrnSum_OsnU05m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU05m : 0.00M); } }
   public decimal TrnSum_OsnU05n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_OsnU05n : 0.00M); } }
   public decimal TrnSum_Osn25m_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Osn25m  : 0.00M); } }
   public decimal TrnSum_Osn25n_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Osn25n  : 0.00M); } }
   public decimal TrnSum_Osn10m_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Osn10m  : 0.00M); } }
   public decimal TrnSum_Osn10n_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Osn10n  : 0.00M); } }
   public decimal TrnSum_Osn25m_TP  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.TP ? this.TrnSum_Osn25m  : 0.00M); } }
   public decimal TrnSum_Osn25n_TP  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.TP ? this.TrnSum_Osn25n  : 0.00M); } }

   public decimal TrnSum_PdvR25m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR25m : 0.00M); } }
   public decimal TrnSum_PdvR25n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR25n : 0.00M); } }
   public decimal TrnSum_PdvU25m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU25m : 0.00M); } }
   public decimal TrnSum_PdvU25n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU25n : 0.00M); } }
   public decimal TrnSum_PdvR10m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR10m : 0.00M); } }
   public decimal TrnSum_PdvR10n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR10n : 0.00M); } }
   public decimal TrnSum_PdvU10m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU10m : 0.00M); } }
   public decimal TrnSum_PdvU10n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU10n : 0.00M); } }
   public decimal TrnSum_PdvR05m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR05m : 0.00M); } }
   public decimal TrnSum_PdvR05n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvR05n : 0.00M); } }
   public decimal TrnSum_PdvU05m_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU05m : 0.00M); } }
   public decimal TrnSum_PdvU05n_EU { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.EU ? this.TrnSum_PdvU05n : 0.00M); } }
   public decimal TrnSum_Pdv25m_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Pdv25m  : 0.00M); } }
   public decimal TrnSum_Pdv25n_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Pdv25n  : 0.00M); } }
   public decimal TrnSum_Pdv10m_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Pdv10m  : 0.00M); } }
   public decimal TrnSum_Pdv10n_BS  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.BS ? this.TrnSum_Pdv10n  : 0.00M); } }
   public decimal TrnSum_Pdv25m_TP  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.TP ? this.TrnSum_Pdv25m  : 0.00M); } }
   public decimal TrnSum_Pdv25n_TP  { get { return (this.PdvGEOkind == ZXC.PdvGEOkindEnum.TP ? this.TrnSum_Pdv25n  : 0.00M); } }

   public decimal TrnSum_OsnZP_11   { get { return (this.PdvZPkind == ZXC.PdvZPkindEnum.KOL_11 ? this.TrnSum_Osn09 : 0.00M); } }
   public decimal TrnSum_OsnZP_12   { get { return (this.PdvZPkind == ZXC.PdvZPkindEnum.KOL_12 ? this.TrnSum_Osn09 : 0.00M); } }
   public decimal TrnSum_OsnZP_13   { get { return (this.PdvZPkind == ZXC.PdvZPkindEnum.KOL_13 ? this.TrnSum_Osn09 : 0.00M); } }

   public decimal TrnSum_Osn12      { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol12).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn13      { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol13).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn14      { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol14).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn15      { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol15).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn16      { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol16).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }

   #endregion 2013 EU PDV NEWS

   #region Result Sums - NON Data Layer Columns

   //private List<Rtrans> TransesNonDeleted
   /*private*/public Rtrans[] TrnNonDel  // postalo public tek 05.01.2012 
   { 
      get 
      {
         Rtrans[]  rt = this.Transes.Where
            (rtrn =>
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
                  //rtrn.T_TT                 != Faktur.TT_PUL   // proizvodnju ulaz na zbrajaj 
               rtrn.TtInfo.IsSplitProizvodnjaULAZ == false  // proizvodnju ulaz na zbrajaj PUL, PIX 
            ).ToArray();

         //if(rt == null || this.Transes == null)
         //{
         //   System.Diagnostics.StackFrame sf0 = new System.Diagnostics.StackTrace(1, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf1 = new System.Diagnostics.StackTrace(2, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf2 = new System.Diagnostics.StackTrace(3, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf3 = new System.Diagnostics.StackTrace(4, true).GetFrame(0);

         //   ZXC.aim_log("// TrnNonDel() RESULT or TRANSES is NULL, called from {0} -> {1} -> {2} -> {3}//",
         //      rt.Count().ToString(), this.Transes.Count.ToString(), sf0.GetMethod().Name, sf3.GetMethod().Name, sf2.GetMethod().Name, sf1.GetMethod().Name);
         //}
         //else if(rt.Count() != this.Transes.Count)
         //{
         //   System.Diagnostics.StackFrame sf0 = new System.Diagnostics.StackTrace(1, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf1 = new System.Diagnostics.StackTrace(2, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf2 = new System.Diagnostics.StackTrace(3, true).GetFrame(0);
         //   System.Diagnostics.StackFrame sf3 = new System.Diagnostics.StackTrace(4, true).GetFrame(0);

         //   ZXC.aim_log("// TrnNonDel() {0} of {1} called from {2} -> {3} -> {4} -> {5}//",
         //      rt.Count().ToString(), this.Transes.Count.ToString(), sf0.GetMethod().Name, sf3.GetMethod().Name, sf2.GetMethod().Name, sf1.GetMethod().Name);
         //}

         return rt;

         //return this.Transes.Where
         //   (rtrn =>
         //      rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
         //         //rtrn.T_TT                 != Faktur.TT_PUL   // proizvodnju ulaz na zbrajaj 
         //      rtrn.TtInfo.IsSplitProizvodnjaULAZ == false  // proizvodnju ulaz na zbrajaj PUL, PIX 
         //   ).ToArray();
      } 
   }

   public Rtrans[] TrnNonDel_ALL 
   { 
      get 
      {
         Rtrans[]  rt = this.Transes.Where
            (rtrn =>
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete
            ).ToArray();

         return rt;
      } 
   }

   public Rtrano[] TrnNonDel2
   {
      get
      {
         return this.Transes2.Where
            (rtrn =>
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
             //rtrn.T_TT                 != Faktur.TT_PUL   // proizvodnju ulaz na zbrajaj 
               rtrn.TtInfo.IsSplitProizvodnjaULAZ == false  // proizvodnju ulaz na zbrajaj PUL, PIX 
            ).ToArray();
      }
   }

   /*private*/public Rtrans[] TrnNonDel_PULX_ALL
   { 
      get 
      { 
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
             //rtrn.T_TT                 == Faktur.TT_PUL   // proizvodnju ulaz zbrajaj 
               rtrn.TtInfo.IsSplitProizvodnjaULAZ == true   // proizvodnju ulaz zbrajaj PUL, PIX 
            ).ToArray(); 
      } 
   }

   private Rtrans[] TrnNonDel_PULX_WO_OTPADPILJ
   { 
      get 
      { 
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
             //rtrn.T_TT                 == Faktur.TT_PUL   // proizvodnju ulaz zbrajaj 
               rtrn.TtInfo.IsSplitProizvodnjaULAZ == true &&// proizvodnju ulaz zbrajaj PUL, PIX 
               rtrn.T_artiklCD                    != Artikl.OtpadArtiklCD && // NE zbrajaj 'otpad' 
               rtrn.T_artiklCD                    != Artikl.PljvnArtiklCD    // NE zbrajaj 'otpad' 
            ).ToArray(); 
      } 
   }

   public Rtrans[] TransesWOtwins
   { 
      get 
      {
         if(TtInfo.HasTwinTT == false) return this.Transes.ToArray();

         // FallDown: dakle ovo JESU TwinTT-ovi (MSI, VMI)
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
               rtrn.T_TT                 != TtInfo.TwinTT // odbaci MSU, VMU, ostavi samo MSI, MSU 
            ).ToArray(); 
      } 
   }

   private Rtrans[] TrnNonDel_AVANS_STORNO
   { 
      get 
      { 
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
               rtrn.T_pdvColTip == ZXC.PdvKolTipEnum.AVANS_STORNO
            ).ToArray(); 
      } 
   }

   public Rtrans[] TrnNonDel_WO_AVANS_STORNO
   { 
      get 
      { 
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
               rtrn.T_pdvColTip != ZXC.PdvKolTipEnum.AVANS_STORNO
            ).ToArray(); 
      } 
   }

   private Rtrans[] TrnNonDel_UMJETNINA // FUSE 
   { 
      get 
      { 
         return this.Transes.Where
            (rtrn => 
               rtrn.SaveTransesWriteMode != ZXC.WriteMode.Delete &&
               rtrn.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN
            ).ToArray(); 
      } 
   }

   //NE! [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   public decimal R_ukRbt1i2    { get { return this.S_ukRbt1 + this.S_ukRbt2; } }

   // 07.09.2015: 
   public decimal R_ukKCRwoZtr  { get { return this.S_ukKCR      - this.S_ukZavisni                                 ; } }

   public decimal R_ukOsn25     { get { return this.S_ukOsn25m   + this.S_ukOsn25n                                  ; } }
   public decimal R_ukOsn05     { get { return this.S_ukOsn05m   + this.S_ukOsn05n                                  ; } }
   public decimal R_ukOsn23     { get { return this.S_ukOsn23m   + this.S_ukOsn23n                                  ; } }
   public decimal R_ukOsn22     { get { return this.S_ukOsn22m   + this.S_ukOsn22n                                  ; } }
   public decimal R_ukOsn10     { get { return this.S_ukOsn10m   + this.S_ukOsn10n                                  ; } }
   public decimal R_ukKCRPwoPr  { get { return this.S_ukKCRP     - this.S_ukOsnPr                                   ; } }

   public decimal R_ukOsn25_23_22_10_05 { get { return this.R_ukOsn25 + this.R_ukOsn05 + this.R_ukOsn23 + this.R_ukOsn22 + this.R_ukOsn10; } }

   public decimal R_ukOsn25_UMJETNINA { get { return IsUMJETNINA == true ? this.S_ukOsn25m : 0.00M; } }

   public uint    TrnSum_Count { get { return (uint)this.TrnNonDel.Count()            ; } }

   public decimal TrnSum_K     { get { return this.TrnNonDel .Sum(rtrn  => rtrn .T_kol)  ; } }
   public decimal TrnSum_K2    { get { return this.TrnNonDel .Sum(rtrn  => rtrn .T_kol2) ; } }
   public decimal TrnSum2_K    { get { return this.TrnNonDel2.Sum(rtrno => rtrno.T_kol)  ; } }
   public decimal TrnSum2_MOD_RAM_saldo
   { 
      get 
      {
         if(this.TT != Faktur.TT_MOD) return 0M;

         return this.TrnNonDel2.Sum(rtrno => rtrno.T_dimX) - this.TrnNonDel2.Sum(rtrno => rtrno.T_dimY); 
      } 
   }

   public decimal TrnSum2_MOD_HDD_saldo
   {
      get
      {
         if(this.TT != Faktur.TT_MOD) return 0M;

         return this.TrnNonDel2.Sum(rtrno => rtrno.T_decA) - this.TrnNonDel2.Sum(rtrno => rtrno.T_decB);
      }
   }

   public decimal TrnSum2_dimX { get { return this.TrnNonDel2.Sum(rtrno => rtrno.T_dimX); } }
   public decimal TrnSum2_dimY { get { return this.TrnNonDel2.Sum(rtrno => rtrno.T_dimY); } }
   public decimal TrnSum2_decA { get { return this.TrnNonDel2.Sum(rtrno => rtrno.T_decA); } }
   public decimal TrnSum2_decB { get { return this.TrnNonDel2.Sum(rtrno => rtrno.T_decB); } }





   #region AVANS_STORNO

   public decimal R_ukKC_AVANS_STORNO      { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Sum(rtrn => rtrn.R_KC  ); } }
   public decimal R_ukKCR_AVANS_STORNO     { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Sum(rtrn => rtrn.R_KCR ); } }
   public decimal R_ukPdv_AVANS_STORNO     { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Sum(rtrn => rtrn.R_pdv ); } }
   public decimal R_ukKCRP_AVANS_STORNO    { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Sum(rtrn => rtrn.R_KCRP); } }

   public decimal R_ukKCR_25m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_25m).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)      ); } }
   public decimal R_ukPdv_25m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_25m).Sum(rtrn =>                          rtrn.R_pdv                ).Ron2(); } }

   public decimal R_ukKCR_10m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_10m).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)      ); } }
   public decimal R_ukPdv_10m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_10m).Sum(rtrn => rtrn.R_pdv                                         ).Ron2(); } }
   public decimal R_ukKCR_05m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_05m).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)      ); } }
   public decimal R_ukPdv_05m_AVANS_STORNO { get { return -1.00M * this.TrnNonDel_AVANS_STORNO.Where(rtr => rtr.R_isPdv_05m).Sum(rtrn => rtrn.R_pdv                                         ).Ron2(); } }

   public decimal R_ukKC_SUM_AVANS         { get { return this.S_ukKC     + this.R_ukKC_AVANS_STORNO  ; } }
   public decimal R_ukKCR_SUM_AVANS        { get { return this.S_ukKCR    + this.R_ukKCR_AVANS_STORNO ; } }
   public decimal R_ukPdv_SUM_AVANS        { get { return this.S_ukPdv    + this.R_ukPdv_AVANS_STORNO ; } }
   public decimal R_ukKCRP_SUM_AVANS       { get { return this.S_ukKCRP   + this.R_ukKCRP_AVANS_STORNO; } }

   public decimal R_ukKCR_25m_SUM_AVANS    { get { return this.S_ukOsn25m + this.R_ukKCR_25m_AVANS_STORNO ; } }
   public decimal R_ukPdv_25m_SUM_AVANS    { get { return this.S_ukPdv25m + this.R_ukPdv_25m_AVANS_STORNO ; } }

   public decimal R_ukKCR_10m_SUM_AVANS    { get { return this.S_ukOsn10m + this.R_ukKCR_10m_AVANS_STORNO ; } }
   public decimal R_ukPdv_10m_SUM_AVANS    { get { return this.S_ukPdv10m + this.R_ukPdv_10m_AVANS_STORNO ; } }
   public decimal R_ukKCR_05m_SUM_AVANS    { get { return this.S_ukOsn05m + this.R_ukKCR_05m_AVANS_STORNO ; } }
   public decimal R_ukPdv_05m_SUM_AVANS    { get { return this.S_ukPdv05m + this.R_ukPdv_05m_AVANS_STORNO ; } }

   #endregion AVANS_STORNO

   // --- PIZX & PULX shit start 

   public decimal TrnSum_K_PULX_ALL             { get { return this.TrnNonDel_PULX_ALL     .Sum(rtrn => rtrn.T_kol); } }
   public decimal TrnSum_K_PULX_P               { get { return this.TrnNonDel_PULX_WO_OTPADPILJ.Sum(rtrn => rtrn.T_kol); } } // WO OTPAD 
   public decimal TrnSum_K_PIZX                 { get { return this.TrnSum_K /*- this.TrnSum_K_PULX*/              ; } } // TrnSum_K je vec bez PULX-ova 
   public decimal TrnSum_K2_PIZX                { get { return this.TrnSum_K2/*- this.TrnSum_K_PULX*/              ; } } // TrnSum_K je vec bez PULX-ova 
   public decimal TrnSum_K_PULX_REAL_OTPAD      { get { return this.TrnSum_K_PULX_ALL - this.TrnSum_K_PULX_P  ; } }
   public decimal TrnSum_K_PIZX_PULX_DIFF_ALL   { get { return this.TrnSum_K_PIZX     - this.TrnSum_K_PULX_ALL; } }

   public decimal TrnSum_K_PULX_THEORETIC_OTPAD { get { return this.TrnSum_K_PIZX     - this.TrnSum_K_PULX_P  ; } }

   public decimal TrnSum_KC_PULX_ALL            { get { return this.TrnNonDel_PULX_ALL     .Sum(rtrn => rtrn.R_KC)   ; } }
   public decimal TrnSum_KC_PULX_P              { get { return this.TrnNonDel_PULX_WO_OTPADPILJ.Sum(rtrn => rtrn.R_KC)   ; } } // WO OTPAD 
   public decimal TrnSum_KC_PIZX                { get { return this.TrnSum_KC /*- this.TrnSum_K_PULX*/               ; } } // TrnSum_K je vec bez PULX-ova 
   public decimal TrnSum_KC_PULX_REAL_OTPAD     { get { return this.TrnSum_KC_PULX_ALL - this.TrnSum_KC_PULX_P       ; } }
   public decimal TrnSum_KC_PIZX_PULX_DIFF_ALL  { get { return this.TrnSum_KC_PIZX     - this.TrnSum_KC_PULX_ALL     ; } }

   public decimal R_K_PULX_REAL_OTPAD     { get { return this.S_puxK_All  - this.S_puxK_P ; } }
   public decimal R_KC_PULX_REAL_OTPAD    { get { return this.S_puxKC_All - this.S_puxKC_P; } }
                                          
   public decimal R_K_PIZX_PULX_DIFF_ALL  { get { return this.S_pixK  - this.S_puxK_All   ; } }
   public decimal R_KC_PIZX_PULX_DIFF_ALL { get { return this.S_pixKC - this.S_puxKC_All  ; } }

   // offix: if(notZero(ppukSumIkol)) iskoristeno = ppukSumUkol / ppukSumIkol * 100.00;
   public decimal R_PIXPUX_ISKORISTIVOST  { get { return ZXC.DivSafe(this.S_puxK_P, this.S_pixK) * 100.00M; } }

   // --- PIZX & PULX shit end 


   // --- PPMV shit start _=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_= 

   #region Bef 2017

   public decimal FirstTrn_PpmvOsn_bef2017   
   { 
      get 
      { 
         Rtrans rtrans_rec = this.TrnNonDel.OrderBy(r => r.T_serial).FirstOrDefault();

         if(rtrans_rec != null && rtrans_rec.T_ppmvSt1i2.NotZero()) return Math.Max(rtrans_rec.T_cij, rtrans_rec.T_ppmvOsn);
         else                                                       return 0.00M;
      } 
   }

   public decimal FirstTrn_PpmvSt1i2_bef2017 { get { Rtrans rtrans_rec = this.TrnNonDel.OrderBy(r => r.T_serial).FirstOrDefault(); return rtrans_rec == null ? 0.00M : rtrans_rec.T_ppmvSt1i2; } }

   // 11.09.2015: 
 //public decimal FirstTrn_PpmvIzn   { get { return ZXC.VvGet_25_on_100(FirstTrn_PpmvOsn, FirstTrn_PpmvSt1i2); } }
   public decimal FirstTrn_PpmvIzn_bef2017   
   { 
      get 
      {
         decimal ppmvIzn = ZXC.VvGet_25_of_100(FirstTrn_PpmvOsn, FirstTrn_PpmvSt1i2);
         return S_ukK.IsNegative() ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
      } 
   }

   // 11.09.2015: 
 //public decimal R_ukPpmvIzn { get { return ZXC.VvGet_25_on_100(this.S_ukPpmvOsn, this.S_ukPpmvSt1i2); } }
   public decimal R_ukPpmvIzn_bef2017 
   { 
      get 
      {
         decimal ppmvIzn = ZXC.VvGet_25_of_100(this.S_ukPpmvOsn, this.S_ukPpmvSt1i2);
         return S_ukK.IsNegative() ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
      } 
   }

   #endregion Bef 2017

   public decimal FirstTrn_PpmvOsn   
   { 
      get 
      { 
         Rtrans rtrans_rec = this.TrnNonDel.OrderBy(r => r.T_serial).FirstOrDefault();

       //if(rtrans_rec != null && rtrans_rec.T_ppmvSt1i2.NotZero()) return Math.Max(rtrans_rec.T_cij, rtrans_rec.T_ppmvOsn);
         if(rtrans_rec != null && rtrans_rec.T_ppmvSt1i2.NotZero()) return                            rtrans_rec.T_ppmvOsn ; // Od 2017 je u PpmvOsn vec izracunani iznosPpmv 
         else                                                       return 0.00M;
      } 
   }

   public decimal FirstTrn_PpmvSt1i2 { get { Rtrans rtrans_rec = this.TrnNonDel.OrderBy(r => r.T_serial).FirstOrDefault(); return rtrans_rec == null ? 0.00M : rtrans_rec.T_ppmvSt1i2; } }

   // 11.09.2015: 
 //public decimal FirstTrn_PpmvIzn   { get { return ZXC.VvGet_25_on_100(FirstTrn_PpmvOsn, FirstTrn_PpmvSt1i2); } }
   public decimal FirstTrn_PpmvIzn   
   { 
      get 
      {
       //decimal ppmvIzn = ZXC.VvGet_25_on_100(FirstTrn_PpmvOsn, FirstTrn_PpmvSt1i2);
         decimal ppmvIzn =                     FirstTrn_PpmvOsn                     ; // Od 2017 je u PpmvOsn vec izracunani iznosPpmv 

         return S_ukK.IsNegative() ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
      } 
   }

   // 11.09.2015: 
 //public decimal R_ukPpmvIzn { get { return ZXC.VvGet_25_on_100(this.S_ukPpmvOsn, this.S_ukPpmvSt1i2); } }
   public decimal R_ukPpmvIzn 
   { 
      get 
      {
       //decimal ppmvIzn = ZXC.VvGet_25_on_100(this.S_ukPpmvOsn, this.S_ukPpmvSt1i2);
         decimal ppmvIzn =                     this.S_ukPpmvOsn                     ; // Od 2017 je u PpmvOsn vec izracunani iznosPpmv 

         // 11.05.2021. kod Ducatija je cesto S_ukK = 0 jer na istom racnu ima i +1 motor i -1 neki avans 
         //return S_ukK.IsNegative()                   ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
           return S_ukK.IsNegative() || this.Is_STORNO ? -ppmvIzn : ppmvIzn; // kada je storno racuna 
      } 
   }

   public decimal X_ukPpmvIzn        { get; set; }

   // 19.03.2018: data layer 'PdvNum' property oslobađamo od dosadasnje upotrebe da bi ga mogli koristiti za zaista data layer upotrebu 
   public uint    X_PdvNum           { get; set; }

   public decimal R_ukKCRPwoPPMV     { get { return (this.S_ukKCRP - this.R_ukPpmvIzn); } }

   // --- PPMV shit end _=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_=_= 


   public decimal TrnSum_KC     { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KC    )     ; } }
   public decimal TrnSum_KC_IRM { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KC_IRM)     ; } }
                                                                                             
   public decimal TrnSum_KCR      { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KCR)      ; } }
   public decimal TrnSum_KCRM     { get { ChkOsnSum();  
                                          return this.TrnNonDel.Sum(rtrn => rtrn.R_KCRM) ; } }
   public decimal TrnSum_KCRP     { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KCRP)     ; } }
   public decimal TrnSum_Rbt1     { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_rbt1)     ; } }
   public decimal TrnSum_Rbt1_IRM { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_rbt1_IRM) ; } }
   public decimal TrnSum_Rbt2     { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_rbt2)     ; } }
   public decimal TrnSum_Mrz      { get { return (TtInfo.IsSplitTTMalULAZ ? this.TrnNonDel_PULX_ALL.Sum(rtrn => rtrn.R_mrz   ) : this.TrnNonDel.Sum(rtrn => rtrn.R_mrz))  ; } }
   public decimal TrnSum_Ztr      { get { return this.TrnNonDel.Sum(rtrn => rtrn.T_ztr)      ; } }

   public decimal TrnSum_kizKC    { get { return this.TrnNonDel.Sum(rtrn => rtrn.Rkiz_KC)   ; } }
   public decimal TrnSum_kizKCR   { get { return this.TrnNonDel.Sum(rtrn => rtrn.Rkiz_KCR)  ; } }
   public decimal TrnSum_kizRbt1  { get { return this.TrnNonDel.Sum(rtrn => rtrn.Rkiz_rbt1) ; } }
   public decimal TrnSum_MSK   { get { return (TtInfo.IsSplitTTMalULAZ ? this.TrnNonDel_PULX_ALL.Sum(rtrn => rtrn.R_MSK   ) : this.TrnNonDel.Sum(rtrn => rtrn.R_MSK   ));} }
   public decimal TrnSum_MSK_00{ get { return this.TrnNonDel.Sum(rtrn => rtrn.R_MSK_00);} }
   public decimal TrnSum_MSK_10{ get { return this.TrnNonDel.Sum(rtrn => rtrn.R_MSK_10);} }
   public decimal TrnSum_MSK_23{ get { return this.TrnNonDel.Sum(rtrn => rtrn.R_MSK_23);} }
   public decimal TrnSum_MSK_05{ get { return this.TrnNonDel.Sum(rtrn => rtrn.R_MSK_05);} }
   public decimal TrnSum_MSK_25{ get { return (TtInfo.IsSplitTTMalULAZ ? this.TrnNonDel_PULX_ALL.Sum(rtrn => rtrn.R_MSK_25) : this.TrnNonDel.Sum(rtrn => rtrn.R_MSK_25)); } }

   public decimal TrnSum_MskPdv  { get { ChkPdvSum(); return (TtInfo.IsSplitTTMalULAZ ? this.TrnNonDel_PULX_ALL.Sum(rtrn => rtrn.R_mskPdv  ) : this.TrnNonDel.Sum(rtrn => rtrn.R_mskPdv  )); } }
   public decimal TrnSum_MskPdv10{ get { ChkPdvSum(); 
                                       return this.TrnNonDel.Sum(rtrn => rtrn.R_mskPdv10); } }
   public decimal TrnSum_MskPdv23{ get { ChkPdvSum(); 
                                       return this.TrnNonDel.Sum(rtrn => rtrn.R_mskPdv23); } }
   public decimal TrnSum_MskPdv05{ get { ChkPdvSum(); 
                                       return this.TrnNonDel.Sum(rtrn => rtrn.R_mskPdv05); } }
   public decimal TrnSum_MskPdv25
   { 
      get { ChkPdvSum();

      return (TtInfo.IsSplitTTMalULAZ ? this.TrnNonDel_PULX_ALL.Sum(rtrn => rtrn.R_mskPdv25) :
                                                    this.TrnNonDel         .Sum(rtrn => rtrn.R_mskPdv25)); 
      } 
   }

   public decimal TrnSum_KCR_usl  { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KCR_usl ); } }
   public decimal TrnSum_KCRP_usl { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_KCRP_usl); } }

   // PDV 'S_' calc SUMe, ovo NISU DataLayer S_SUMe 

   public decimal TrnSum_OsnPNP  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPNP).Sum(rtrn => (rtrn.R_PnpOsn)); } }
   public decimal TrnSum_IznPNP  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPNP).Sum(rtrn => (rtrn.R_Pnp   )); } }
   public decimal TrnSum_MskPNP  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPNP).Sum(rtrn => (rtrn.R_mskPnp)); } }

   public decimal TrnSum_Osn25m  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25m  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn25n  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_25n  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn25   { get { return (this.TrnSum_Osn25m + this.TrnSum_Osn25n).Ron2(); } }

   public decimal TrnSum_Osn05m  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05m  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn05n  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_05n  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn05   { get { return (this.TrnSum_Osn05m + this.TrnSum_Osn05n).Ron2(); } }

   public decimal TrnSum_Osn23m  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_23m  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn23n  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_23n  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn23   { get { return (this.TrnSum_Osn23m + this.TrnSum_Osn23n).Ron2(); } }

   public decimal TrnSum_Osn22m  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_22m  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn22n  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_22n  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn22   { get { return (this.TrnSum_Osn22m + this.TrnSum_Osn22n).Ron2(); } }

   public decimal TrnSum_Osn10m  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10m  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn10n  { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_10n  ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn10x  { get { return (this.TrnSum_Osn10m + this.TrnSum_Osn10n).Ron2(); } }

   public decimal TrnSum_Osn0    { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_0    ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_OsnPr   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_PR   ).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }

   public decimal TrnSum_Osn07   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol07).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn08   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol08).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn09   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol09).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn10   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol10).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }
   public decimal TrnSum_Osn11   { get { return this.TrnNonDel.Where(rtr => rtr.R_isPdv_kol11).Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)); } }

   public decimal TrnSum_OsnUr25 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_ROB ? this.TrnSum_Osn25m : 0.00M); } }
   public decimal TrnSum_OsnUr23 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_ROB ? this.TrnSum_Osn23m : 0.00M); } }
   public decimal TrnSum_OsnUu25 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_USL ? this.TrnSum_Osn25m : 0.00M); } }
   public decimal TrnSum_OsnUu23 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_USL ? this.TrnSum_Osn23m : 0.00M); } }
   public decimal TrnSum_OsnUu22 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_USL ? this.TrnSum_Osn22m : 0.00M); } }
   public decimal TrnSum_OsnUu10 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_USL ? this.TrnSum_Osn10m : 0.00M); } }
   public decimal TrnSum_OsnUr05 { get { return (this.PdvKnjiga == ZXC.PdvKnjigaEnum.UVOZ_ROB ? this.TrnSum_Osn05m : 0.00M); } }

#if _PUSE_
   public decimal TrnSum_Pdv23m { get { return (this.TrnSum_Osn23m.Ron2() * 0.23M).Ron2(); } }
   public decimal TrnSum_Pdv23n { get { return (this.TrnSum_Osn23         * 0.23M).Ron2() - this.TrnSum_Pdv23m; } }

   public decimal TrnSum_Pdv22m { get { return (this.TrnSum_Osn22m.Ron2() * 0.22M).Ron2(); } }
   public decimal TrnSum_Pdv22n { get { return (this.TrnSum_Osn22         * 0.22M).Ron2() - this.TrnSum_Pdv22m; } }

   public decimal TrnSum_Pdv10m { get { return (this.TrnSum_Osn10m.Ron2() * 0.10M).Ron2(); } }
   public decimal TrnSum_Pdv10n { get { return (this.TrnSum_Osn10x        * 0.10M).Ron2() - this.TrnSum_Pdv10m; } }

   // Uvoz 
   public decimal TrnSum_PdvUr23 { get { return (this.TrnSum_OsnUr23.Ron2() * 0.23M).Ron2(); } }
   public decimal TrnSum_PdvUu23 { get { return (this.TrnSum_OsnUu23.Ron2() * 0.23M).Ron2(); } }
   public decimal TrnSum_PdvUu22 { get { return (this.TrnSum_OsnUu22.Ron2() * 0.22M).Ron2(); } }
   public decimal TrnSum_PdvUu10 { get { return (this.TrnSum_OsnUu10.Ron2() * 0.10M).Ron2(); } }
#endif

 //public decimal TrnSum_Pdv     { get { ChkPdvSum(); 
 //                                return this.TrnNonDel.Sum(rtrn => rtrn.R_pdv); } }
   public decimal TrnSum_Pdv     { get { return this.Ss_ukPdvAll; } }

   public decimal TrnSum_Pdv25   { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_25 ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv25m  { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_25m).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv25n  { get { return this.TrnSum_Pdv25 - this.TrnSum_Pdv25m; } }

   public decimal TrnSum_Pdv05   { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_05 ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv05m  { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_05m).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv05n  { get { return this.TrnSum_Pdv05 - this.TrnSum_Pdv05m; } }

   public decimal TrnSum_Pdv23   { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_23 ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv23m  { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_23m).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv23n  { get { return this.TrnSum_Pdv23 - this.TrnSum_Pdv23m; } }

   public decimal TrnSum_Pdv22   { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_22 ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv22m  { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_22m).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv22n  { get { return this.TrnSum_Pdv22 - this.TrnSum_Pdv22m; } }

   public decimal TrnSum_Pdv10   { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_10 ).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv10m  { get { return this.TrnNonDel.Where(rtrans => rtrans.R_isPdv_10m).Sum(rtrn => rtrn.R_pdv).Ron2(); } }
   public decimal TrnSum_Pdv10n  { get { return this.TrnSum_Pdv10 - this.TrnSum_Pdv10m; } }

   public decimal TrnSum_PdvUr25 { get { return (this.TrnSum_OsnUr25.Ron2() * 0.25M).Ron2(); } }
   public decimal TrnSum_PdvUr23 { get { return (this.TrnSum_OsnUr23.Ron2() * 0.23M).Ron2(); } }
   public decimal TrnSum_PdvUu25 { get { return (this.TrnSum_OsnUu25.Ron2() * 0.25M).Ron2(); } }
   public decimal TrnSum_PdvUu23 { get { return (this.TrnSum_OsnUu23.Ron2() * 0.23M).Ron2(); } }
   public decimal TrnSum_PdvUu22 { get { return (this.TrnSum_OsnUu22.Ron2() * 0.22M).Ron2(); } }
   public decimal TrnSum_PdvUr05 { get { return (this.TrnSum_OsnUr05.Ron2() * 0.05M).Ron2(); } }
   public decimal TrnSum_PdvUu10 
   { 
      get 
      {
         if(IsOld10Pdv) return (this.TrnSum_OsnUu10.Ron2() * 0.10M).Ron2();
         else           return (this.TrnSum_OsnUu10.Ron2() * 0.13M).Ron2(); 
      } 
   }

   // 22.11.2018: eRacun UBL additions: start 
   public decimal TrnSum_Rbt25  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_25).Sum(rtrn => rtrn.R_rbtAll).Ron2(); } }
   public decimal TrnSum_Rbt10  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_10).Sum(rtrn => rtrn.R_rbtAll).Ron2(); } }
   public decimal TrnSum_Rbt05  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_05).Sum(rtrn => rtrn.R_rbtAll).Ron2(); } }
   public decimal TrnSum_Rbt00  { get { return this.TrnNonDel.Where(rtrans => !rtrans.R_isPdv_25 &&
                                                                              !rtrans.R_isPdv_10 &&
                                                                              !rtrans.R_isPdv_05).Sum(rtrn => rtrn.R_rbtAll).Ron2(); } }

 //public decimal TrnSum_Ztr25  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_25).Sum(rtrn => rtrn.T_ztr).Ron2(); } }
 //public decimal TrnSum_Ztr10  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_10).Sum(rtrn => rtrn.T_ztr).Ron2(); } }
 //public decimal TrnSum_Ztr05  { get { return this.TrnNonDel.Where(rtrans =>  rtrans.R_isPdv_05).Sum(rtrn => rtrn.T_ztr).Ron2(); } }
 //public decimal TrnSum_Ztr00  { get { return this.TrnNonDel.Where(rtrans => !rtrans.R_isPdv_25 &&
 //                                                                           !rtrans.R_isPdv_10 &&
 //                                                                           !rtrans.R_isPdv_05).Sum(rtrn => rtrn.T_ztr).Ron2(); } }

   // 22.11.2018: eRacun UBL additions:  end  

   // 08.11.2016: 
   //public bool IsOld10Pdv { get { return PdvDate < ZXC.Date01012014; } }
   public bool IsOld10Pdv 
   { 
      get 
      {
         if(PdvDate.IsEmpty()) return DokDate < ZXC.Date01012014; 
         else                  return PdvDate < ZXC.Date01012014; 
      } 
   }

   // NAMJERNO nije property nego fnct - da ge implicitno pozivanje 
   // ne zahvati. npr. pri predaju SetReportDataSource kristalima   
   public bool IsPdvDateTooOld()
   {
      if(this.TtInfo.IsPdvTT == false || this.PdvR12 != ZXC.PdvR12Enum.R1) return false;

      bool isPdvDateTooOld;

      if(PdvDate.IsEmpty())
      {
         ZXC.aim_emsg("PdvDate is empty! {0}\n\n{1}", PdvDate, this);
         return false;
      }

      DateTime dateNow = DateTime.Now;

      if(dateNow.Year  == PdvDate.Year && 
         dateNow.Month == PdvDate.Month) return false;

      if(dateNow.Day <= 20) // do  20-og u mjesecu jos uvijek mogu u prosli mjesec 
      {
         isPdvDateTooOld = ZXC.MonthDifference(dateNow, PdvDate) >  1; // 0 and 1 monthDiff is ok 
      }
      else // dan mjeseca je veci od 20 ... mogu samo od ovog mjeseca 
      {
         isPdvDateTooOld = ZXC.MonthDifference(dateNow, PdvDate) >= 1; // 0 monthDiff is ok only 
      }

  //  // ova 'safe' djidja rjesava sijecanj vs prosinac komplikacije 
  //  int thisMonthSafe = DateTime.Now.Month == 1 ? 13 : DateTime.Now.Month;
  ////int pdvMonthSafe  = PdvDate.     Month == 1 ? 13 : PdvDate.Month     ;
  //
  //  if(dateNow.Day <= 20) // do  20-og u mjesecu jos uvijek mogu u prosli mjesec 
  //  {
  //     isPdvDateTooOld = PdvDate.Month < thisMonthSafe - 1;
  //  }
  //  else // dan mjeseca je veci od 20 ... mogu samo od ovog mjeseca 
  //  {
  //     isPdvDateTooOld = PdvDate.Month < thisMonthSafe;
  //  }

      return isPdvDateTooOld;
   }

   public decimal Ss_ukOsnAll   { get { ChkOsnSum(); return TrnSum_Osn25m + TrnSum_Osn25n + TrnSum_Osn05m + TrnSum_Osn05n + TrnSum_Osn23m + TrnSum_Osn23n + TrnSum_Osn22m + TrnSum_Osn22n + TrnSum_Osn10m + TrnSum_Osn10n + TrnSum_Osn0 + TrnSum_OsnPr; } }
   public decimal Ss_ukPdvAll   { get { ChkPdvSum(); return TrnSum_Pdv25m + TrnSum_Pdv25n + TrnSum_Pdv05m + TrnSum_Pdv05n + TrnSum_Pdv23m + TrnSum_Pdv23n + TrnSum_Pdv22m + TrnSum_Pdv22n + TrnSum_Pdv10m + TrnSum_Pdv10n; } }
   public decimal Ss_ukPdvAll2  { get {              return TrnSum_Pdv25m + TrnSum_Pdv25n + TrnSum_Pdv05m + TrnSum_Pdv05n + TrnSum_Pdv23m + TrnSum_Pdv23n + TrnSum_Pdv22m + TrnSum_Pdv22n + TrnSum_Pdv10m + TrnSum_Pdv10n; } }

   public decimal S_ukKC_OR_ukKCR
   {
      get
      {
         if(IsExtendable) return this.TheEx.currentData._s_ukKCR;
         else             return this.      currentData._s_ukKC ;
      }
   }

   public decimal S_ukKC_OR_ukKCRP
   {
      get
      {
         if(IsExtendable) return this.TheEx.currentData._s_ukKCRP;
         else             return this.      currentData._s_ukKC  ;
      }
   }

   // ova dva su samo za svrhu Reporta 
   public decimal S_ukBlg_UPL { get; set; }
   public decimal S_ukBlg_ISP { get; set; }

   public decimal S_ukBlg_SALDO { get { return S_ukBlg_UPL - S_ukBlg_ISP; } }

   public decimal S_Blg_UPL
   {
      get
      {
         if(this.TT == TT_UPL || this.TT == TT_BUP) return this.currentData._s_ukKC;
         else                  return 0.00M;
      }
   }

   public decimal S_Blg_ISP
   {
      get
      {
         if(this.TT == TT_ISP || this.TT == TT_BIS) return this.currentData._s_ukKC;
         else                  return 0.00M;
      }
   }

   private void ChkOsnSum()
   {
      if(ZXC.RISK_CopyToOtherDUC_inProgress) return;

      decimal rtransSum = TrnNonDel.Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR)).Ron2();
      decimal tarifeSum = (TrnSum_Osn25m + TrnSum_Osn25n + TrnSum_Osn05m + TrnSum_Osn05n + TrnSum_Osn23m + TrnSum_Osn23n + TrnSum_Osn22m + TrnSum_Osn22n + TrnSum_Osn10m + TrnSum_Osn10n + TrnSum_Osn0 + TrnSum_OsnPr +
                           TrnSum_Osn07  + TrnSum_Osn08  + TrnSum_Osn09  + TrnSum_Osn10  + TrnSum_Osn11  + 
                           TrnSum_Osn12  + TrnSum_Osn13  + TrnSum_Osn14  + TrnSum_Osn15  + TrnSum_Osn16).Ron2();

      if(tarifeSum != rtransSum)

         ZXC.aim_emsg(MessageBoxIcon.Error, this + "\n\nOsnSum MISMATCH.\n\n" +
            "RtransSum: " + rtransSum.ToStringVv() + "\n" +
            "TarifeSum: " + tarifeSum.ToStringVv());
   }

   private void ChkPdvSum()
   {
      if(ZXC.RISK_CopyToOtherDUC_inProgress) return;

      decimal rtransSum = TrnNonDel.Sum(rtrn => rtrn.R_pdv).Ron2();
    //decimal tarifeSum = (TrnSum_Pdv23m + TrnSum_Pdv23n + TrnSum_Pdv22m + TrnSum_Pdv22n + TrnSum_Pdv10m + TrnSum_Pdv10n).Ron2();
      decimal tarifeSum = (TrnSum_Pdv25m + TrnSum_Pdv25n + TrnSum_Pdv05m + TrnSum_Pdv05n + TrnSum_Pdv23m + TrnSum_Pdv23n + TrnSum_Pdv22m + TrnSum_Pdv22n + TrnSum_Pdv10m + TrnSum_Pdv10n).Ron2();

      decimal tolerancy = 0.01M;
    //decimal tolerancy = 0.00M;

      if(ZXC.OffixImport_InProgress) tolerancy = 0.05M;

    //if(tarifeSum != rtransSum)
      if(ZXC.AlmostEqual(tarifeSum, rtransSum, tolerancy) == false)

         ZXC.aim_emsg(MessageBoxIcon.Error, this + "\n\nPdvSum MISMATCH.\n\n" +
            "RtransSum: " + rtransSum.ToStringVv() + "\n" +
            "TarifeSum: " + tarifeSum.ToStringVv());
   }

   private static DateTime dateOfNewSumOrder  = new DateTime(2011, 2, 20);
   private static DateTime dateOfNewSumOrder2 = new DateTime(2012, 1, 20);
   
 //public string Has_TrnSum_vs_S_Sum_Discrepancy()
   public string Has_TrnSum_vs_S_Sum_Discrepancy
   {
   get
   {
      if(ZXC.RISK_CopyToOtherDUC_inProgress) return "";

      if(this.TT == Faktur.TT_MOD) return "";

      decimal tolerancy = 0.00M;

    //if(TtInfo.IsMalopTT) tolerancy = 0.01M; // od 20.04.2023.: 
      if(TtInfo.IsMalopTT) tolerancy = 0.00M;

      /*    */ if(!ZXC.AlmostEqual(S_ukK       .Ron2(), TrnSum_K      .Ron2(), tolerancy)) return "S-uk " + S_ukK       .ToStringVv() + " ≠ TrnSum " + TrnSum_K      .ToStringVv() + " (S_ukK) suma količina"  ;
    ///*    */ if(!ZXC.AlmostEqual(S_ukK2      .Ron2(), TrnSum_K2     .Ron2(), tolerancy)) return "S-uk " + S_ukK2      .ToStringVv() + " ≠ TrnSum " + TrnSum_K2     .ToStringVv() + " (S_ukK2) suma količina2" ;
      /*    */ if(!ZXC.AlmostEqual(S_ukKC      .Ron2(), TrnSum_KC     .Ron2(), tolerancy)) return "S-uk " + S_ukKC      .ToStringVv() + " ≠ TrnSum " + TrnSum_KC     .ToStringVv() + " (S_ukKC) ukupno kol*cij";
                                                                                                                                          
      if(IsExtendable == false) return "";                                                                                                
                                                                                                                                          
      /* 44 */ if(!ZXC.AlmostEqual(S_ukKCRP    .Ron2(), TrnSum_KCRP   .Ron2(), tolerancy)) return "S-uk " + S_ukKCRP    .ToStringVv() + " ≠ TrnSum " + TrnSum_KCRP   .ToStringVv() + " (S_ukKCRP) sveukupno sa pdv-om";
      /* 45 */ if(!ZXC.AlmostEqual(S_ukKCRM    .Ron2(), TrnSum_KCRM   .Ron2(), tolerancy)) return "S-uk " + S_ukKCRM    .ToStringVv() + " ≠ TrnSum " + TrnSum_KCRM   .ToStringVv() + " (S_ukKCRM) ukupno sa marzom, prije pdv-a";
      /* 46 */ if(!ZXC.AlmostEqual(S_ukKCR     .Ron2(), TrnSum_KCR    .Ron2(), tolerancy)) return "S-uk " + S_ukKCR     .ToStringVv() + " ≠ TrnSum " + TrnSum_KCR    .ToStringVv() + " (S_ukKCR) ukupno prije pdv-a";
      /* 47 */ if(!ZXC.AlmostEqual(S_ukRbt1    .Ron2(), TrnSum_Rbt1   .Ron2(), tolerancy)) return "S-uk " + S_ukRbt1    .ToStringVv() + " ≠ TrnSum " + TrnSum_Rbt1   .ToStringVv() + " (S_ukRbt1) iznos rbt1";
      /* 48 */ if(!ZXC.AlmostEqual(S_ukRbt2    .Ron2(), TrnSum_Rbt2   .Ron2(), tolerancy)) return "S-uk " + S_ukRbt2    .ToStringVv() + " ≠ TrnSum " + TrnSum_Rbt2   .ToStringVv() + " (S_ukRbt2) iznos rbt2";
      /* 49 */ if(!ZXC.AlmostEqual(S_ukZavisni .Ron2(), TrnSum_Ztr    .Ron2(), tolerancy)) return "S-uk " + S_ukZavisni .ToStringVv() + " ≠ TrnSum " + TrnSum_Ztr    .ToStringVv() + " (S_ukZavisni) iznos zavTros";
   // /* 50 */ if(!ZXC.AlmostEqual(S_ukProlazne.Ron2(), TrnSum_       .Ron2(), tolerancy)) return "S-uk " + S_ukProlazne.ToStringVv() + " ≠ TrnSum " + TrnSum_       .ToStringVv() + " (S_ukProlazne) ";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdv25m  .Ron2(), TrnSum_Pdv25m .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25m .ToStringVv() + " (S_ukPdv25m)";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdv25n  .Ron2(), TrnSum_Pdv25n .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25n .ToStringVv() + " (S_ukPdv25n)";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdv05m  .Ron2(), TrnSum_Pdv05m .Ron2(), tolerancy)) return "S-uk " + S_ukPdv05m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv05m .ToStringVv() + " (S_ukPdv05m)";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdv05n  .Ron2(), TrnSum_Pdv05n .Ron2(), tolerancy)) return "S-uk " + S_ukPdv05n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv05n .ToStringVv() + " (S_ukPdv05n)";
      /* 51 */ if(!ZXC.AlmostEqual(S_ukPdv23m  .Ron2(), TrnSum_Pdv23m .Ron2(), tolerancy)) return "S-uk " + S_ukPdv23m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv23m .ToStringVv() + " (S_ukPdv23m)";
      /* 52 */ if(!ZXC.AlmostEqual(S_ukPdv23n  .Ron2(), TrnSum_Pdv23n .Ron2(), tolerancy)) return "S-uk " + S_ukPdv23n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv23n .ToStringVv() + " (S_ukPdv23n)";
      /* 53 */ if(!ZXC.AlmostEqual(S_ukPdv22m  .Ron2(), TrnSum_Pdv22m .Ron2(), tolerancy)) return "S-uk " + S_ukPdv22m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv22m .ToStringVv() + " (S_ukPdv22m)";
      /* 54 */ if(!ZXC.AlmostEqual(S_ukPdv22n  .Ron2(), TrnSum_Pdv22n .Ron2(), tolerancy)) return "S-uk " + S_ukPdv22n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv22n .ToStringVv() + " (S_ukPdv22n)";
      /* 55 */ if(!ZXC.AlmostEqual(S_ukPdv10m  .Ron2(), TrnSum_Pdv10m .Ron2(), tolerancy)) return "S-uk " + S_ukPdv10m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv10m .ToStringVv() + " (S_ukPdv10m)";
      /* 56 */ if(!ZXC.AlmostEqual(S_ukPdv10n  .Ron2(), TrnSum_Pdv10n .Ron2(), tolerancy)) return "S-uk " + S_ukPdv10n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv10n .ToStringVv() + " (S_ukPdv10n)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsn25m  .Ron2(), TrnSum_Osn25m .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25m .ToStringVv() + " (S_ukOsn25m)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsn25n  .Ron2(), TrnSum_Osn25n .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25n .ToStringVv() + " (S_ukOsn25n)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsn05m  .Ron2(), TrnSum_Osn05m .Ron2(), tolerancy)) return "S-uk " + S_ukOsn05m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn05m .ToStringVv() + " (S_ukOsn05m)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsn05n  .Ron2(), TrnSum_Osn05n .Ron2(), tolerancy)) return "S-uk " + S_ukOsn05n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn05n .ToStringVv() + " (S_ukOsn05n)";
      /* 57 */ if(!ZXC.AlmostEqual(S_ukOsn23m  .Ron2(), TrnSum_Osn23m .Ron2(), tolerancy)) return "S-uk " + S_ukOsn23m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn23m .ToStringVv() + " (S_ukOsn23m)";
      /* 58 */ if(!ZXC.AlmostEqual(S_ukOsn23n  .Ron2(), TrnSum_Osn23n .Ron2(), tolerancy)) return "S-uk " + S_ukOsn23n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn23n .ToStringVv() + " (S_ukOsn23n)";
      /* 59 */ if(!ZXC.AlmostEqual(S_ukOsn22m  .Ron2(), TrnSum_Osn22m .Ron2(), tolerancy)) return "S-uk " + S_ukOsn22m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn22m .ToStringVv() + " (S_ukOsn22m)";
      /* 60 */ if(!ZXC.AlmostEqual(S_ukOsn22n  .Ron2(), TrnSum_Osn22n .Ron2(), tolerancy)) return "S-uk " + S_ukOsn22n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn22n .ToStringVv() + " (S_ukOsn22n)";
      /* 61 */ if(!ZXC.AlmostEqual(S_ukOsn10m  .Ron2(), TrnSum_Osn10m .Ron2(), tolerancy)) return "S-uk " + S_ukOsn10m  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn10m .ToStringVv() + " (S_ukOsn10m)";
      /* 62 */ if(!ZXC.AlmostEqual(S_ukOsn10n  .Ron2(), TrnSum_Osn10n .Ron2(), tolerancy)) return "S-uk " + S_ukOsn10n  .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn10n .ToStringVv() + " (S_ukOsn10n)";
      /* 63 */ if(!ZXC.AlmostEqual(S_ukOsn0    .Ron2(), TrnSum_Osn0   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn0    .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn0   .ToStringVv() + " (S_ukOsn0)";
      /* 64 */ if(!ZXC.AlmostEqual(S_ukOsnPr   .Ron2(), TrnSum_OsnPr  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnPr   .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnPr  .ToStringVv() + " (S_ukOsnPr)";
      /* 76 */ if(!ZXC.AlmostEqual(S_ukMrz     .Ron2(), TrnSum_Mrz    .Ron2(), tolerancy)) return "S-uk " + S_ukMrz     .ToStringVv() + " ≠ TrnSum " + TrnSum_Mrz    .ToStringVv() + " (S_ukMrz)";
      /* 77 */ if(!ZXC.AlmostEqual(S_ukPdv     .Ron2(), TrnSum_Pdv    .Ron2(), tolerancy)) return "S-uk " + S_ukPdv     .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv    .ToStringVv() + " (S_ukPdv)";
                                                                                                                                          
      if(this.AddTS <= dateOfNewSumOrder) return "";                                                                                           
                                                                                                                                          
      /* 83 */ if(!ZXC.AlmostEqual(S_ukOsn07   .Ron2(), TrnSum_Osn07  .Ron2(), tolerancy)) return "S-uk " + S_ukOsn07   .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn07  .ToStringVv() + " (ukOsn07)";
      /* 84 */ if(!ZXC.AlmostEqual(S_ukOsn08   .Ron2(), TrnSum_Osn08  .Ron2(), tolerancy)) return "S-uk " + S_ukOsn08   .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn08  .ToStringVv() + " (ukOsn08)";
      /* 85 */ if(!ZXC.AlmostEqual(S_ukOsn09   .Ron2(), TrnSum_Osn09  .Ron2(), tolerancy)) return "S-uk " + S_ukOsn09   .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn09  .ToStringVv() + " (ukOsn09)";
      /* 86 */ if(!ZXC.AlmostEqual(S_ukOsn10   .Ron2(), TrnSum_Osn10  .Ron2(), tolerancy)) return "S-uk " + S_ukOsn10   .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn10  .ToStringVv() + " (ukOsn10)";
      /* 87 */ if(!ZXC.AlmostEqual(S_ukOsn11   .Ron2(), TrnSum_Osn11  .Ron2(), tolerancy)) return "S-uk " + S_ukOsn11   .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn11  .ToStringVv() + " (ukOsn11)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsnUr05 .Ron2(), TrnSum_OsnUr05.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUr05 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUr05.ToStringVv() + " (ukOsnUr05)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsnUr25 .Ron2(), TrnSum_OsnUr25.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUr25 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUr25.ToStringVv() + " (ukOsnUr25)";
      /* 88 */ if(!ZXC.AlmostEqual(S_ukOsnUr23 .Ron2(), TrnSum_OsnUr23.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUr23 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUr23.ToStringVv() + " (ukOsnUr23)";
      /* 89 */ if(!ZXC.AlmostEqual(S_ukOsnUu10 .Ron2(), TrnSum_OsnUu10.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUu10 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUu10.ToStringVv() + " (ukOsnUu10)";
      /* 90 */ if(!ZXC.AlmostEqual(S_ukOsnUu22 .Ron2(), TrnSum_OsnUu22.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUu22 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUu22.ToStringVv() + " (ukOsnUu22)";
      /*    */ if(!ZXC.AlmostEqual(S_ukOsnUu25 .Ron2(), TrnSum_OsnUu25.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUu25 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUu25.ToStringVv() + " (ukOsnUu25)";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdvUr05 .Ron2(), TrnSum_PdvUr05.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUr05 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUr05.ToStringVv() + " (ukPdvUr05)";
      /*    */ if(!ZXC.AlmostEqual(S_ukPdvUr25 .Ron2(), TrnSum_PdvUr25.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUr25 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUr25.ToStringVv() + " (ukPdvUr25)";
      /* 91 */ if(!ZXC.AlmostEqual(S_ukOsnUu23 .Ron2(), TrnSum_OsnUu23.Ron2(), tolerancy)) return "S-uk " + S_ukOsnUu23 .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnUu23.ToStringVv() + " (ukOsnUu23)";
      /* 92 */ if(!ZXC.AlmostEqual(S_ukPdvUr23 .Ron2(), TrnSum_PdvUr23.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUr23 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUr23.ToStringVv() + " (ukPdvUr23)";
      /* 93 */ if(!ZXC.AlmostEqual(S_ukPdvUu10 .Ron2(), TrnSum_PdvUu10.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUu10 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUu10.ToStringVv() + " (ukPdvUu10)";
      /* 94 */ if(!ZXC.AlmostEqual(S_ukPdvUu22 .Ron2(), TrnSum_PdvUu22.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUu22 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUu22.ToStringVv() + " (ukPdvUu22)";
      /* 95 */ if(!ZXC.AlmostEqual(S_ukPdvUu23 .Ron2(), TrnSum_PdvUu23.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUu23 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUu23.ToStringVv() + " (ukPdvUu23)";
      /* 95 */ if(!ZXC.AlmostEqual(S_ukPdvUu25 .Ron2(), TrnSum_PdvUu25.Ron2(), tolerancy)) return "S-uk " + S_ukPdvUu25 .ToStringVv() + " ≠ TrnSum " + TrnSum_PdvUu25.ToStringVv() + " (ukPdvUu25)";

      if(this.TT == Faktur.TT_IRM)
      {
         foreach(Rtrans rtrans in TrnNonDel)
         {
            if(rtrans.TheAsEx == null) continue;

            if(ZXC.AlmostEqual(rtrans.TheAsEx.LastUlazMPC.Ron2(), rtrans.T_cij.Ron2(), tolerancy) == false && rtrans.TheAsEx.LastUlazMPC.NotZero())
            {
               return String.Format("Redak {0} MPC sa dokumenta {1} ≠ MPC po ulazima {2}", rtrans.T_serial, rtrans.T_cij.ToStringVv(), rtrans.TheAsEx.LastUlazMPC.ToStringVv());
            }
         }
      }

      //if(this.AddTS <= dateOfNewSumOrder2) return "";                                                                                           
                                                                                                                                          
      /*    */ if(!ZXC.AlmostEqual(S_ukMskPdv25   .Ron2(), TrnSum_MskPdv25  .Ron2(), tolerancy)) return "S-uk " + S_ukMskPdv25.ToStringVv() + " ≠ TrnSum " + TrnSum_MskPdv25.ToStringVv() + " (S_ukMskPdv25)";
      /*    */ if(!ZXC.AlmostEqual(S_ukMskPdv05   .Ron2(), TrnSum_MskPdv05  .Ron2(), tolerancy)) return "S-uk " + S_ukMskPdv05.ToStringVv() + " ≠ TrnSum " + TrnSum_MskPdv05.ToStringVv() + " (S_ukMskPdv05)";
      /* 103*/ if(!ZXC.AlmostEqual(S_ukMskPdv23   .Ron2(), TrnSum_MskPdv23  .Ron2(), tolerancy)) return "S-uk " + S_ukMskPdv23.ToStringVv() + " ≠ TrnSum " + TrnSum_MskPdv23.ToStringVv() + " (S_ukMskPdv23)";
      /* 103*/ if(!ZXC.AlmostEqual(S_ukMskPdv10   .Ron2(), TrnSum_MskPdv10  .Ron2(), tolerancy)) return "S-uk " + S_ukMskPdv10.ToStringVv() + " ≠ TrnSum " + TrnSum_MskPdv10.ToStringVv() + " (S_ukMskPdv10)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukMSK_00     .Ron2(), TrnSum_MSK_00    .Ron2(), tolerancy)) return "S-uk " + S_ukMSK_00  .ToStringVv() + " ≠ TrnSum " + TrnSum_MSK_00  .ToStringVv() + " (S_ukMSK_00)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukMSK_10     .Ron2(), TrnSum_MSK_10    .Ron2(), tolerancy)) return "S-uk " + S_ukMSK_10  .ToStringVv() + " ≠ TrnSum " + TrnSum_MSK_10  .ToStringVv() + " (S_ukMSK_10)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukMSK_23     .Ron2(), TrnSum_MSK_23    .Ron2(), tolerancy)) return "S-uk " + S_ukMSK_23  .ToStringVv() + " ≠ TrnSum " + TrnSum_MSK_23  .ToStringVv() + " (S_ukMSK_23)";
      /*    */ if(!ZXC.AlmostEqual(S_ukMSK_25     .Ron2(), TrnSum_MSK_25    .Ron2(), tolerancy)) return "S-uk " + S_ukMSK_25  .ToStringVv() + " ≠ TrnSum " + TrnSum_MSK_25  .ToStringVv() + " (S_ukMSK_25)";
      /*    */ if(!ZXC.AlmostEqual(S_ukMSK_05     .Ron2(), TrnSum_MSK_05    .Ron2(), tolerancy)) return "S-uk " + S_ukMSK_05  .ToStringVv() + " ≠ TrnSum " + TrnSum_MSK_05  .ToStringVv() + " (S_ukMSK_05)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukKCR_usl    .Ron2(), TrnSum_KCR_usl   .Ron2(), tolerancy)) return "S-uk " + S_ukKCR_usl .ToStringVv() + " ≠ TrnSum " + TrnSum_KCR_usl .ToStringVv() + " (S_ukKCR_usl)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukKCRP_usl   .Ron2(), TrnSum_KCRP_usl  .Ron2(), tolerancy)) return "S-uk " + S_ukKCRP_usl.ToStringVv() + " ≠ TrnSum " + TrnSum_KCRP_usl.ToStringVv() + " (S_ukKCRP_usl)";

      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR25m_EU.Ron2(), TrnSum_OsnR25m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR25m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR25m_EU.ToStringVv() + " (S_ukOsnR25m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR25n_EU.Ron2(), TrnSum_OsnR25n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR25n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR25n_EU.ToStringVv() + " (S_ukOsnR25n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU25m_EU.Ron2(), TrnSum_OsnU25m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU25m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU25m_EU.ToStringVv() + " (S_ukOsnU25m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU25n_EU.Ron2(), TrnSum_OsnU25n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU25n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU25n_EU.ToStringVv() + " (S_ukOsnU25n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR10m_EU.Ron2(), TrnSum_OsnR10m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR10m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR10m_EU.ToStringVv() + " (S_ukOsnR10m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR10n_EU.Ron2(), TrnSum_OsnR10n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR10n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR10n_EU.ToStringVv() + " (S_ukOsnR10n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU10m_EU.Ron2(), TrnSum_OsnU10m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU10m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU10m_EU.ToStringVv() + " (S_ukOsnU10m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU10n_EU.Ron2(), TrnSum_OsnU10n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU10n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU10n_EU.ToStringVv() + " (S_ukOsnU10n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR05m_EU.Ron2(), TrnSum_OsnR05m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR05m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR05m_EU.ToStringVv() + " (S_ukOsnR05m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnR05n_EU.Ron2(), TrnSum_OsnR05n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnR05n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnR05n_EU.ToStringVv() + " (S_ukOsnR05n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU05m_EU.Ron2(), TrnSum_OsnU05m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU05m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU05m_EU.ToStringVv() + " (S_ukOsnU05m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnU05n_EU.Ron2(), TrnSum_OsnU05n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukOsnU05n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_OsnU05n_EU.ToStringVv() + " (S_ukOsnU05n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn25m_BS .Ron2(), TrnSum_Osn25m_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25m_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25m_BS .ToStringVv() + " (S_ukOsn25m_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn25n_BS .Ron2(), TrnSum_Osn25n_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25n_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25n_BS .ToStringVv() + " (S_ukOsn25n_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn10m_BS .Ron2(), TrnSum_Osn10m_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn10m_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn10m_BS .ToStringVv() + " (S_ukOsn10m_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn10n_BS .Ron2(), TrnSum_Osn10n_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn10n_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn10n_BS .ToStringVv() + " (S_ukOsn10n_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn25m_TP .Ron2(), TrnSum_Osn25m_TP   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25m_TP .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25m_TP .ToStringVv() + " (S_ukOsn25m_TP )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn25n_TP .Ron2(), TrnSum_Osn25n_TP   .Ron2(), tolerancy)) return "S-uk " + S_ukOsn25n_TP .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn25n_TP .ToStringVv() + " (S_ukOsn25n_TP )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR25m_EU.Ron2(), TrnSum_PdvR25m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR25m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR25m_EU.ToStringVv() + " (S_ukPdvR25m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR25n_EU.Ron2(), TrnSum_PdvR25n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR25n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR25n_EU.ToStringVv() + " (S_ukPdvR25n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU25m_EU.Ron2(), TrnSum_PdvU25m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU25m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU25m_EU.ToStringVv() + " (S_ukPdvU25m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU25n_EU.Ron2(), TrnSum_PdvU25n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU25n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU25n_EU.ToStringVv() + " (S_ukPdvU25n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR10m_EU.Ron2(), TrnSum_PdvR10m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR10m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR10m_EU.ToStringVv() + " (S_ukPdvR10m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR10n_EU.Ron2(), TrnSum_PdvR10n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR10n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR10n_EU.ToStringVv() + " (S_ukPdvR10n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU10m_EU.Ron2(), TrnSum_PdvU10m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU10m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU10m_EU.ToStringVv() + " (S_ukPdvU10m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU10n_EU.Ron2(), TrnSum_PdvU10n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU10n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU10n_EU.ToStringVv() + " (S_ukPdvU10n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR05m_EU.Ron2(), TrnSum_PdvR05m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR05m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR05m_EU.ToStringVv() + " (S_ukPdvR05m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvR05n_EU.Ron2(), TrnSum_PdvR05n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvR05n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvR05n_EU.ToStringVv() + " (S_ukPdvR05n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU05m_EU.Ron2(), TrnSum_PdvU05m_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU05m_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU05m_EU.ToStringVv() + " (S_ukPdvU05m_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdvU05n_EU.Ron2(), TrnSum_PdvU05n_EU  .Ron2(), tolerancy)) return "S-uk " + S_ukPdvU05n_EU.ToStringVv() + " ≠ TrnSum " + TrnSum_PdvU05n_EU.ToStringVv() + " (S_ukPdvU05n_EU)";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv25m_BS .Ron2(), TrnSum_Pdv25m_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25m_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25m_BS .ToStringVv() + " (S_ukPdv25m_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv25n_BS .Ron2(), TrnSum_Pdv25n_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25n_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25n_BS .ToStringVv() + " (S_ukPdv25n_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv10m_BS .Ron2(), TrnSum_Pdv10m_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv10m_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv10m_BS .ToStringVv() + " (S_ukPdv10m_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv10n_BS .Ron2(), TrnSum_Pdv10n_BS   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv10n_BS .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv10n_BS .ToStringVv() + " (S_ukPdv10n_BS )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv25m_TP .Ron2(), TrnSum_Pdv25m_TP   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25m_TP .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25m_TP .ToStringVv() + " (S_ukPdv25m_TP )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukPdv25n_TP .Ron2(), TrnSum_Pdv25n_TP   .Ron2(), tolerancy)) return "S-uk " + S_ukPdv25n_TP .ToStringVv() + " ≠ TrnSum " + TrnSum_Pdv25n_TP .ToStringVv() + " (S_ukPdv25n_TP )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnZP_11  .Ron2(), TrnSum_OsnZP_11    .Ron2(), tolerancy)) return "S-uk " + S_ukOsnZP_11  .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnZP_11  .ToStringVv() + " (S_ukOsnZP_11  )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnZP_12  .Ron2(), TrnSum_OsnZP_12    .Ron2(), tolerancy)) return "S-uk " + S_ukOsnZP_12  .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnZP_12  .ToStringVv() + " (S_ukOsnZP_12  )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnZP_13  .Ron2(), TrnSum_OsnZP_13    .Ron2(), tolerancy)) return "S-uk " + S_ukOsnZP_13  .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnZP_13  .ToStringVv() + " (S_ukOsnZP_13  )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn12     .Ron2(), TrnSum_Osn12       .Ron2(), tolerancy)) return "S-uk " + S_ukOsn12     .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn12     .ToStringVv() + " (S_ukOsn12     )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn13     .Ron2(), TrnSum_Osn13       .Ron2(), tolerancy)) return "S-uk " + S_ukOsn13     .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn13     .ToStringVv() + " (S_ukOsn13     )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn14     .Ron2(), TrnSum_Osn14       .Ron2(), tolerancy)) return "S-uk " + S_ukOsn14     .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn14     .ToStringVv() + " (S_ukOsn14     )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn15     .Ron2(), TrnSum_Osn15       .Ron2(), tolerancy)) return "S-uk " + S_ukOsn15     .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn15     .ToStringVv() + " (S_ukOsn15     )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsn16     .Ron2(), TrnSum_Osn16       .Ron2(), tolerancy)) return "S-uk " + S_ukOsn16     .ToStringVv() + " ≠ TrnSum " + TrnSum_Osn16     .ToStringVv() + " (S_ukOsn16     )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukOsnPNP    .Ron2(), TrnSum_OsnPNP      .Ron2(), tolerancy)) return "S-uk " + S_ukOsnPNP    .ToStringVv() + " ≠ TrnSum " + TrnSum_OsnPNP    .ToStringVv() + " (S_ukOsnPNP    )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukIznPNP    .Ron2(), TrnSum_IznPNP      .Ron2(), tolerancy)) return "S-uk " + S_ukIznPNP    .ToStringVv() + " ≠ TrnSum " + TrnSum_IznPNP    .ToStringVv() + " (S_ukIznPNP    )";
      /* 104*/ if(!ZXC.AlmostEqual(S_ukMskPNP    .Ron2(), TrnSum_MskPNP      .Ron2(), tolerancy)) return "S-uk " + S_ukMskPNP    .ToStringVv() + " ≠ TrnSum " + TrnSum_MskPNP    .ToStringVv() + " (S_ukMskPNP    )";
      /* 104*/ if(!ZXC.AlmostEqual(Skiz_ukKC     .Ron2(), TrnSum_kizKC       .Ron2(), tolerancy)) return "S-uk " + Skiz_ukKC     .ToStringVv() + " ≠ TrnSum " + TrnSum_kizKC     .ToStringVv() + " (Skiz_ukKC     )";
      /* 104*/ if(!ZXC.AlmostEqual(Skiz_ukKCR    .Ron2(), TrnSum_kizKCR      .Ron2(), tolerancy)) return "S-uk " + Skiz_ukKCR    .ToStringVv() + " ≠ TrnSum " + TrnSum_kizKCR    .ToStringVv() + " (Skiz_ukKCR    )";
      /* 104*/ if(!ZXC.AlmostEqual(Skiz_ukRbt1   .Ron2(), TrnSum_kizRbt1     .Ron2(), tolerancy)) return "S-uk " + Skiz_ukRbt1   .ToStringVv() + " ≠ TrnSum " + TrnSum_kizRbt1   .ToStringVv() + " (Skiz_ukRbt1   )";

      return "";
   } // get 
   }

   // NotaBene, 31.01.2012 ne znam vise koji li mi je ku'ac trebalo ovo 'shouldChangePdvStuff', ali stvara probleme pa cu ga ukinuti (u RtransDao-u) 
   public void TakeTransesSumToDokumentSum(bool shouldChangePdvStuff) 
   {
      
      /*    */ S_ukK       = TrnSum_K     .Ron2();
      /*    */ S_ukK2      = TrnSum_K2    .Ron2();
      /*    */ S_ukKC      = TrnSum_KC    .Ron2();
               S_ukTrnCount= (uint)TrnSum_Count;
      
      if(IsExtendable == false) return;

      if(shouldChangePdvStuff)
      {
         /* 44 */ S_ukKCRP    = TrnSum_KCRP   .Ron2(); 
         /* 57 */ S_ukOsn23m  = TrnSum_Osn23m .Ron2(); 
         /* 58 */ S_ukOsn23n  = TrnSum_Osn23n .Ron2(); 
         /* 51 */ S_ukPdv23m  = TrnSum_Pdv23m .Ron2(); 
         /* 52 */ S_ukPdv23n  = TrnSum_Pdv23n .Ron2(); 

         /* 57 */ S_ukOsn25m  = TrnSum_Osn25m .Ron2(); 
         /* 58 */ S_ukOsn25n  = TrnSum_Osn25n .Ron2(); 
         /* 51 */ S_ukPdv25m  = TrnSum_Pdv25m .Ron2(); 
         /* 52 */ S_ukPdv25n  = TrnSum_Pdv25n .Ron2(); 

         /* 57 */ S_ukOsn05m  = TrnSum_Osn05m .Ron2(); 
         /* 58 */ S_ukOsn05n  = TrnSum_Osn05n .Ron2(); 
         /* 51 */ S_ukPdv05m  = TrnSum_Pdv05m .Ron2(); 
         /* 52 */ S_ukPdv05n  = TrnSum_Pdv05n .Ron2(); 
      }

      /* 45 */ S_ukKCRM    = TrnSum_KCRM   .Ron2(); 
      /* 46 */ S_ukKCR     = TrnSum_KCR    .Ron2(); 
      /* 47 */ S_ukRbt1    = TrnSum_Rbt1   .Ron2(); 
      /* 48 */ S_ukRbt2    = TrnSum_Rbt2   .Ron2(); 
      /* 49 */ S_ukZavisni = TrnSum_Ztr    .Ron2(); 
   // /* 50 */ S_ukProlazne= TrnSum_       .Ron2(); 
      /* 53 */ S_ukPdv22m  = TrnSum_Pdv22m .Ron2(); 
      /* 54 */ S_ukPdv22n  = TrnSum_Pdv22n .Ron2(); 
      /* 55 */ S_ukPdv10m  = TrnSum_Pdv10m .Ron2(); 
      /* 56 */ S_ukPdv10n  = TrnSum_Pdv10n .Ron2(); 
      /* 59 */ S_ukOsn22m  = TrnSum_Osn22m .Ron2(); 
      /* 60 */ S_ukOsn22n  = TrnSum_Osn22n .Ron2(); 
      /* 61 */ S_ukOsn10m  = TrnSum_Osn10m .Ron2(); 
      /* 62 */ S_ukOsn10n  = TrnSum_Osn10n .Ron2(); 
      /* 63 */ S_ukOsn0    = TrnSum_Osn0   .Ron2(); 
      /* 64 */ S_ukOsnPr   = TrnSum_OsnPr  .Ron2(); 
      /* 76 */ S_ukMrz     = TrnSum_Mrz    .Ron2(); 
      /* 77 */ S_ukPdv     = TrnSum_Pdv    .Ron2();
      /* 77 */ S_ukOsn07   = TrnSum_Osn07  .Ron2();
      /* 77 */ S_ukOsn08   = TrnSum_Osn08  .Ron2();
      /* 77 */ S_ukOsn09   = TrnSum_Osn09  .Ron2();
      /* 77 */ S_ukOsn10   = TrnSum_Osn10  .Ron2();
      /* 77 */ S_ukOsn11   = TrnSum_Osn11  .Ron2();
      /* 77 */ S_ukOsnUr25 = TrnSum_OsnUr25.Ron2();
      /* 77 */ S_ukOsnUr05 = TrnSum_OsnUr05.Ron2();
      /* 77 */ S_ukOsnUr23 = TrnSum_OsnUr23.Ron2();
      /* 77 */ S_ukOsnUu10 = TrnSum_OsnUu10.Ron2();
      /* 77 */ S_ukOsnUu22 = TrnSum_OsnUu22.Ron2();
      /* 77 */ S_ukOsnUu23 = TrnSum_OsnUu23.Ron2();
      /* 77 */ S_ukPdvUr23 = TrnSum_PdvUr23.Ron2();
      /* 77 */ S_ukOsnUu25 = TrnSum_OsnUu25.Ron2();
      /* 77 */ S_ukPdvUr25 = TrnSum_PdvUr25.Ron2();
      /* 77 */ S_ukPdvUr05 = TrnSum_PdvUr05.Ron2();
      /* 77 */ S_ukPdvUu10 = TrnSum_PdvUu10.Ron2();
      /* 77 */ S_ukPdvUu22 = TrnSum_PdvUu22.Ron2();
      /* 77 */ S_ukPdvUu23 = TrnSum_PdvUu23.Ron2();
      /* 77 */ S_ukPdvUu25 = TrnSum_PdvUu25.Ron2();

      /*103 */ S_ukMskPdv10= TrnSum_MskPdv10.Ron2(); 
      /*103 */ S_ukMskPdv23= TrnSum_MskPdv23.Ron2(); 
      /*103 */ S_ukMskPdv05= TrnSum_MskPdv05.Ron2(); 
      /*103 */ S_ukMskPdv25= TrnSum_MskPdv25.Ron2(); 
      /*104 */ S_ukMSK_00  = TrnSum_MSK_00  .Ron2(); 
      /*104 */ S_ukMSK_10  = TrnSum_MSK_10  .Ron2(); 
      /*104 */ S_ukMSK_23  = TrnSum_MSK_23  .Ron2(); 
      /*104 */ S_ukMSK_05  = TrnSum_MSK_05  .Ron2(); 
      /*104 */ S_ukMSK_25  = TrnSum_MSK_25  .Ron2(); 
      /*104 */ S_ukKCR_usl = TrnSum_KCR_usl .Ron2(); 
      /*104 */ S_ukKCRP_usl= TrnSum_KCRP_usl.Ron2(); 

      /*133 */ S_pixK      = TrnSum_K_PIZX          .Ron2(); 
      /*134 */ S_puxK_P    = TrnSum_K_PULX_P .Ron2(); 
      /*135 */ S_puxK_All  = TrnSum_K_PULX_ALL      .Ron2(); 
      /*136 */ S_pixKC     = TrnSum_KC_PIZX         .Ron2(); 
      /*137 */ S_puxKC_P   = TrnSum_KC_PULX_P.Ron2(); 
      /*138 */ S_puxKC_All = TrnSum_KC_PULX_ALL     .Ron2(); 

      /*139 */ S_ukPpmvOsn   = FirstTrn_PpmvOsn     .Ron2(); 
      /*140 */ S_ukPpmvSt1i2 = FirstTrn_PpmvSt1i2   .Ron2(); 

      /*104 */ S_ukOsnR25m_EU= TrnSum_OsnR25m_EU.Ron2(); 
      /*104 */ S_ukOsnR25n_EU= TrnSum_OsnR25n_EU.Ron2(); 
      /*104 */ S_ukOsnU25m_EU= TrnSum_OsnU25m_EU.Ron2(); 
      /*104 */ S_ukOsnU25n_EU= TrnSum_OsnU25n_EU.Ron2(); 
      /*104 */ S_ukOsnR10m_EU= TrnSum_OsnR10m_EU.Ron2(); 
      /*104 */ S_ukOsnR10n_EU= TrnSum_OsnR10n_EU.Ron2(); 
      /*104 */ S_ukOsnU10m_EU= TrnSum_OsnU10m_EU.Ron2(); 
      /*104 */ S_ukOsnU10n_EU= TrnSum_OsnU10n_EU.Ron2(); 
      /*104 */ S_ukOsnR05m_EU= TrnSum_OsnR05m_EU.Ron2(); 
      /*104 */ S_ukOsnR05n_EU= TrnSum_OsnR05n_EU.Ron2(); 
      /*104 */ S_ukOsnU05m_EU= TrnSum_OsnU05m_EU.Ron2(); 
      /*104 */ S_ukOsnU05n_EU= TrnSum_OsnU05n_EU.Ron2(); 
      /*104 */ S_ukOsn25m_BS = TrnSum_Osn25m_BS .Ron2(); 
      /*104 */ S_ukOsn25n_BS = TrnSum_Osn25n_BS .Ron2(); 
      /*104 */ S_ukOsn10m_BS = TrnSum_Osn10m_BS .Ron2(); 
      /*104 */ S_ukOsn10n_BS = TrnSum_Osn10n_BS .Ron2(); 
      /*104 */ S_ukOsn25m_TP = TrnSum_Osn25m_TP .Ron2(); 
      /*104 */ S_ukOsn25n_TP = TrnSum_Osn25n_TP .Ron2(); 
      /*104 */ S_ukPdvR25m_EU= TrnSum_PdvR25m_EU.Ron2(); 
      /*104 */ S_ukPdvR25n_EU= TrnSum_PdvR25n_EU.Ron2(); 
      /*104 */ S_ukPdvU25m_EU= TrnSum_PdvU25m_EU.Ron2(); 
      /*104 */ S_ukPdvU25n_EU= TrnSum_PdvU25n_EU.Ron2(); 
      /*104 */ S_ukPdvR10m_EU= TrnSum_PdvR10m_EU.Ron2(); 
      /*104 */ S_ukPdvR10n_EU= TrnSum_PdvR10n_EU.Ron2(); 
      /*104 */ S_ukPdvU10m_EU= TrnSum_PdvU10m_EU.Ron2(); 
      /*104 */ S_ukPdvU10n_EU= TrnSum_PdvU10n_EU.Ron2(); 
      /*104 */ S_ukPdvR05m_EU= TrnSum_PdvR05m_EU.Ron2(); 
      /*104 */ S_ukPdvR05n_EU= TrnSum_PdvR05n_EU.Ron2(); 
      /*104 */ S_ukPdvU05m_EU= TrnSum_PdvU05m_EU.Ron2(); 
      /*104 */ S_ukPdvU05n_EU= TrnSum_PdvU05n_EU.Ron2(); 
      /*104 */ S_ukPdv25m_BS = TrnSum_Pdv25m_BS .Ron2(); 
      /*104 */ S_ukPdv25n_BS = TrnSum_Pdv25n_BS .Ron2(); 
      /*104 */ S_ukPdv10m_BS = TrnSum_Pdv10m_BS .Ron2(); 
      /*104 */ S_ukPdv10n_BS = TrnSum_Pdv10n_BS .Ron2(); 
      /*104 */ S_ukPdv25m_TP = TrnSum_Pdv25m_TP .Ron2(); 
      /*104 */ S_ukPdv25n_TP = TrnSum_Pdv25n_TP .Ron2(); 
      /*104 */ S_ukOsnZP_11  = TrnSum_OsnZP_11  .Ron2(); 
      /*104 */ S_ukOsnZP_12  = TrnSum_OsnZP_12  .Ron2(); 
      /*104 */ S_ukOsnZP_13  = TrnSum_OsnZP_13  .Ron2(); 
      /*104 */ S_ukOsn12     = TrnSum_Osn12     .Ron2(); 
      /*104 */ S_ukOsn13     = TrnSum_Osn13     .Ron2(); 
      /*104 */ S_ukOsn14     = TrnSum_Osn14     .Ron2(); 
      /*104 */ S_ukOsn15     = TrnSum_Osn15     .Ron2(); 
      /*104 */ S_ukOsn16     = TrnSum_Osn16     .Ron2(); 
      /*104 */ S_ukOsnPNP    = TrnSum_OsnPNP    .Ron2(); 
      /*104 */ S_ukIznPNP    = TrnSum_IznPNP    .Ron2(); 
      /*104 */ S_ukMskPNP    = TrnSum_MskPNP    .Ron2(); 
      /*191 */ Skiz_ukKC     = TrnSum_kizKC     .Ron2(); 
      /*192 */ Skiz_ukKCR    = TrnSum_kizKCR    .Ron2(); 
      /*194 */ Skiz_ukRbt1   = TrnSum_kizRbt1   .Ron2(); 

   }

   public bool    IsZtrPresent  
   { 
      get 
      {
         if(this.IsExtendable == false || this.TheEx == null) return false;

         return S_ukZavisni.NotZero() || TrnNonDel.Sum(rtr => rtr.T_ztr).NotZero(); 
      } 
   }

   public override bool IsOneTransChangeShouldRecalcOtherAllTranses { get { return this.IsZtrPresent; } }

   public decimal R2_uplata
   {
      get { return this.TheEx.R2_uplata; }
      set {        this.TheEx.R2_uplata = value; }
   }

   #region Iznos po mjernim jedinicama (masa, povrsina, zapremina, ...)

   public decimal TrnSum_mjMasaN   { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjMasaN ); } }  public string TrnSum_mjMasaNJM  { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjMasaNJM .NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjMasaNJM .NotEmpty()).R_mjMasaNJM ); } }
   public decimal TrnSum_mjMasaB   { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjMasaB ); } }  public string TrnSum_mjMasaBJM  { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjMasaBJM .NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjMasaBJM .NotEmpty()).R_mjMasaBJM ); } }
   public decimal TrnSum_mjPovrs   { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjPovrs ); } }  public string TrnSum_mjPovrsJM  { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjPovrsJM .NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjPovrsJM .NotEmpty()).R_mjPovrsJM ); } }
   public decimal TrnSum_mjZaprem  { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjZaprem); } }  public string TrnSum_mjZapremJM { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjZapremJM.NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjZapremJM.NotEmpty()).R_mjZapremJM); } }
   public decimal TrnSum_mjDuljin  { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjDuljin); } }  public string TrnSum_mjDuljinJM { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjDuljinJM.NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjDuljinJM.NotEmpty()).R_mjDuljinJM); } }
   public decimal TrnSum_mjSirina  { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjSirina); } }  public string TrnSum_mjSirinaJM { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjSirinaJM.NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjSirinaJM.NotEmpty()).R_mjSirinaJM); } }
   public decimal TrnSum_mjVisina  { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_mjVisina); } }  public string TrnSum_mjVisinaJM { get { return(TrnNonDel == null || TrnNonDel.Length.IsZero() || TrnNonDel.FirstOrDefault(trn => trn.R_mjVisinaJM.NotEmpty()) == null ? "" : TrnNonDel.First(trn => trn.R_mjVisinaJM.NotEmpty()).R_mjVisinaJM); } }

   // tek? 14.03.2014: 
   public decimal TrnSum_R_kolOP { get { return this.TrnNonDel.Sum(rtrn => rtrn.R_kolOP); } }
   
   // 31.01.2017: 
   public decimal TrnSum_ForTT_R_kolOP(string theTT) 
   {
      return this.TrnNonDel_ALL.Where(rtr => rtr.T_TT == theTT).Sum(rtrn => rtrn.R_kolOP); 
   }

   public string R_firstRtransPodJM 
   { 
      get 
      { 
         var firstRtrans = this.TrnNonDel.FirstOrDefault();

         if(firstRtrans == null) return "";
         else                    return firstRtrans.R_firstNotEmptyPodJM;
      } 
   }  

   public void SetAllRtrans_R_mjData(List<Artikl> artiklList)
   {
      if(artiklList == null || artiklList.Count.IsZero()) return;

      Artikl artikl;

      foreach(Rtrans rtrans in TrnNonDel)
      {
         artikl = artiklList.SingleOrDefault(art => art.ArtiklCD == rtrans.T_artiklCD);

         if(artikl == null) continue;

         rtrans.CalcUkupnoPoJediniciMjere(artikl);
      }
   }

   #endregion Iznos po mjernim jedinicama (masa, povrsina, zapremina, ...)

   public int DokMonth { get { return this.DokDate.Month; } }

 //public /*ZXC.TH_CycleMoment*/string TH_CycleMoment { get { return ZXC.TH_GetCycleMoment(this.DokDate, this.SkladCD).ToString(); } }
   // 11.01.2018: 
 //public /*ZXC.TH_CycleMoment*/string TH_CycleMoment { get { return ZXC.TH_GetCycleMoment_AsNiceString(this.DokDate, this.SkladCD); } }
   public /*ZXC.TH_CycleMoment*/string TH_CycleMoment
   {
      get
      {
         if(ZXC.IsTEXTHOany == false && ZXC.IsTEXTHOany2 == false || ZXC.IsSkladCD_THshop(this.SkladCD) == false) return "Nepoznat TH_CycleMoment";

         // THPR news: 
       //return ZXC.TH_GetCycleMoment_AsNiceString(this.DokDate, this.SkladCD);
         return TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(this.SkladCD, this.DokDate)./*ToString()*/Opis;
      }
   }

   public bool IsNultiZPC { get { return Get_IsNultiZPC(this.TT, this.TtNum); } }

   public static bool Get_IsNultiZPC(string tt, uint ttNum)
   {
      if(tt == Faktur.TT_ZPC && ttNum.ToString().EndsWith("0000")) return true ;
      else                                                         return false;
   }

   #endregion Result Sums - NON Data Layer Columns

   #region IRA/IRM & KPM Info Propertiz And Metodz Result Sums - NON Data Layer Columns

   // TODO: !!! sta kada je Malop? PO SVOJ PRILICI OVDJE KRIVO RACUNAS NV iPV kada je MALOP u pitanjyu. Ask Delovski 

   public decimal TrnSum_Ira_NV          { get { return TrnNonDel.                                                Sum(rtrn => rtrn.R_Ira_NV     ); } }
   public decimal TrnSum_Ira_PV          { get { return TrnNonDel.                                                Sum(rtrn => rtrn.R_Ira_PV     ); } }

   public decimal TrnSum_Ira_ROB_NV      { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == false).Sum(rtrn => rtrn.R_Ira_NV     ); } }
   public decimal TrnSum_Ira_ROB_PV      { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == false).Sum(rtrn => rtrn.R_Ira_PV     ); } }
   public decimal TrnSum_Ira_ROB_PDV     { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == false).Sum(rtrn => rtrn.R_pdv        ); } }
   public decimal TrnSum_Ira_ROB_RBT     { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == false).Sum(rtrn => rtrn.R_rbtAll     ); } }
   public decimal TrnSum_Ira_ROB_RBT_PDV { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == false).Sum(rtrn => rtrn.R_Ira_RBT_PDV); } }

   public decimal TrnSum_Ira_USL_PV      { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == true ).Sum(rtrn => rtrn.R_Ira_PV     ); } }
   public decimal TrnSum_Ira_USL_PDV     { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == true ).Sum(rtrn => rtrn.R_pdv        ); } }
   public decimal TrnSum_Ira_USL_RBT     { get { return TrnNonDel.Where(rtr => rtr.TheAsEx.IsRuc4Usluga == true ).Sum(rtrn => rtrn.R_rbtAll     ); } }

   // Fuse: public decimal Ira_PRO_NV      { get; set; } 
   // Fuse: public decimal Ira_PRO_PV      { get; set; } 

   // za NV 100, PV 120 ovo vraca 20%
   public decimal TrnSum_Ira_RUC { get { return ZXC.StopaPromjene(TrnSum_Ira_NV, TrnSum_Ira_PV); } }

   // za NV 100, PV 120 ovo vraca 20
   public decimal TrnSum_Ira_RUV { get { return (TrnSum_Ira_PV - TrnSum_Ira_NV); } }

   public decimal   Ira_ROB_KC   { get; set; }     public decimal Ira_USL_KC     { get; set; }     
   public decimal   Ira_ROB_Rbt12{ get; set; }     public decimal Ira_USL_Rbt12  { get; set; }     
   public decimal   Ira_ROB_PV   { get; set; }     public decimal Ira_USL_PV     { get; set; }     
 //public decimal   R_Ira_PV     { get { return TrnSum_Ira_PV; } } 
   public decimal   Ira_ROB_NV   { get { return this.TheEx.Ira_ROB_NV; } 
                                   set {        this.TheEx.Ira_ROB_NV = value; } }  // za Reporte 'K_' tj. 'Ira_ROB_NV' a za ostale bussiness upotrebe 'R_' 
   public decimal   R_Ira_NV     { get { return TrnSum_Ira_NV; } }                  // za Reporte 'K_' tj. 'Ira_ROB_NV' a za ostale bussiness upotrebe 'R_' 
   public decimal   Ira_ROB_Ruc  { get { return ZXC.StopaPromjene(Ira_ROB_NV, Ira_ROB_PV); } }
   public decimal   R_Ira_Ruc    { get { return ZXC.StopaPromjene(R_Ira_NV  , /*R_Ira_PV*/ R_ukKCR_rob); } }
   public decimal   Ira_ROB_Ruv  { get { return (Ira_ROB_PV - Ira_ROB_NV); } }
   public decimal   R_Ira_Ruv    { get { return (/*R_Ira_PV*/ R_ukKCR_rob - R_Ira_NV  ); } }

   public decimal Ira_RUV        { get { return Ira_ROB_Ruv + Ira_USL_PV; } }
   public bool    R_IsNpMix      { get { return this.S_ukKCRP_NP1.NotZero(); } }
   public bool    R_IsNpCashAny  { get { return this.IsNpCash || this.IsNpCash2; } }

   public decimal R_ukKCRP_NP2  { get { return ( this.S_ukKCRP - this.S_ukKCRP_NP1); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 

 //public decimal R_ukKCRP_cash { get { return ( this.IsNpCash  ? this.S_ukKCRP     : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal R_ukKCRP_cash { get { return ( this.IsNpCash  ? this.R_ukKCRP_NP2 : 0M) +
                                                (this.IsNpCash2 ? this.S_ukKCRP_NP1 : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_ukKCRP_cash { get; set;                                             } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 

 //public decimal R_ukKCRP_ziro { get { return (!this.IsNpCash  ? this.S_ukKCRP     : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal R_ukKCRP_ziro { get { return (!this.IsNpCash  ? this.R_ukKCRP_NP2 : 0M) +
                                               (!this.IsNpCash2 ? this.S_ukKCRP_NP1 : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_ukKCRP_ziro { get; set;                                             } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 

   // 21.09.2022: 
 //public decimal R_ukKCRP_storno { get { return (this.Is_STORNO              ? this.S_ukKCRP : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal R_ukKCRP_storno { get { return (this.Is_STORNO_NOT_forNewNP ? this.S_ukKCRP : 0M); } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 

   // !!! 
   public decimal R_pnpSt{ get { return /*TrnNonDel.Any(rtr => rtr.R_isPNP)*/ this.S_ukMskPNP.NotZero() ? ZXC.RRD.Dsc_PnpSt : 0.00M; } }

   public decimal K_NivVrj00    { get { return this.TheEx.K_NivVrj00.Ron2(); } set { this.TheEx.K_NivVrj00 = value; } }   // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_NivVrj10    { get { return this.TheEx.K_NivVrj10.Ron2(); } set { this.TheEx.K_NivVrj10 = value; } }   // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_NivVrj23    { get { return this.TheEx.K_NivVrj23.Ron2(); } set { this.TheEx.K_NivVrj23 = value; } }   // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_NivVrj05    { get { return this.TheEx.K_NivVrj05.Ron2(); } set { this.TheEx.K_NivVrj05 = value; } }   // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_NivVrj25    { get { return this.TheEx.K_NivVrj25.Ron2(); } set { this.TheEx.K_NivVrj25 = value; } }   // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 
   public decimal K_NivVrj      { get { return this.K_NivVrj00 + this.K_NivVrj10 + 
                                               this.K_NivVrj23 + this.K_NivVrj25 + this.K_NivVrj05; } } // za Reporte 'K_' a za ostale bussiness upotrebe 'R_' 

   public decimal K_NivMskPdv23 { get { return ZXC.VvGet_25or3_from_128(this.K_NivVrj23, 23M, R_pnpSt); } }
   public decimal K_NivMskPdv05 { get { return ZXC.VvGet_25or3_from_128(this.K_NivVrj05,  5M, R_pnpSt); } }
   public decimal K_NivMskPdv25 { get { return ZXC.VvGet_25or3_from_128(this.K_NivVrj25, 25M, R_pnpSt); } }
   public decimal K_NivMskPdv   { get { return this.K_NivMskPdv10 + this.K_NivMskPdv23 + this.K_NivMskPdv25 + this.K_NivMskPdv05; } }
   public decimal K_NivMskPdv10 
   { 
      get 
      {
         if(IsOld10Pdv) return ZXC.VvGet_25or3_from_128(this.K_NivVrj10, 10M, R_pnpSt);
         else           return ZXC.VvGet_25or3_from_128(this.K_NivVrj10, 13M, R_pnpSt); 
      } 
   }

   public decimal K_NivMskPNP   { get { return ZXC.VvGet_25_from_125(this.K_NivVrj - this.K_NivMskPdv, R_pnpSt); } }
   public decimal K_NivMrz      { get { return this.K_NivVrj - this.K_NivMskPdv - this.K_NivMskPNP; } }

   // 31.05.2021: vidi promjenu u Rtrans.cs od 30.03.2016: gdje smo odluku punjenja 
   // 'R_MSK_00' promjenili a tu nismo, pa evo sad smo. 
 //public decimal R_NivVrj00    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.R_isPdv_0    ).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj00    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.T_pdvSt == 0M).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj10    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.R_isPdv_10).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj23    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.R_isPdv_23).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj05    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.R_isPdv_05).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj25    { get { return this.TrnNonDel.Where(rtr => rtr.R_isIrmRoba && rtr.R_isPdv_25).Sum(rtrn => (rtrn.TtInfo.IsFinKol_U ? rtrn.A_NivelacUlazVrj : rtrn.A_NivelacIzlazVrj)).Ron2(); } }
   public decimal R_NivVrj      { get { return this.R_NivVrj00 + this.R_NivVrj10 + this.R_NivVrj23 + this.R_NivVrj25 + this.R_NivVrj05; } }

   public decimal R_NivMskPdv23 { get { return ZXC.VvGet_25or3_from_128(this.R_NivVrj23, 23M, R_pnpSt); } }
   public decimal R_NivMskPdv05 { get { return ZXC.VvGet_25or3_from_128(this.R_NivVrj05,  5M, R_pnpSt); } }
   public decimal R_NivMskPdv25 { get { return ZXC.VvGet_25or3_from_128(this.R_NivVrj25, 25M, R_pnpSt); } }
   public decimal R_NivMskPdv   { get { return this.R_NivMskPdv10 + this.R_NivMskPdv23 + this.R_NivMskPdv25 + this.R_NivMskPdv05; } }
   public decimal R_NivMskPdv10 
   { 
      get 
      {
         if(IsOld10Pdv) return ZXC.VvGet_25or3_from_128(this.R_NivVrj10, 10M, R_pnpSt);
         else           return ZXC.VvGet_25or3_from_128(this.R_NivVrj10, 13M, R_pnpSt); 
      } 
   }

   public decimal R_NivMskPNP   { get { return ZXC.VvGet_25_from_125(this.R_NivVrj - this.R_NivMskPdv, R_pnpSt); } }
   public decimal R_NivMrz      { get { return this.R_NivVrj - this.R_NivMskPdv - this.R_NivMskPNP; } }

   public uint    K_dokCount   { get; set; }

   public decimal R_ukMskMrz { get { return this.R_ukMSK - this.R_ukMskPdv - this.S_ukMskPNP - this./*Ira_ROB_NV*/R_Ira_NV; } } // za Reporte 'K_' (tamo di NEMAS rtrans-ove) a za ostale bussiness upotrebe 'R_' 
   public decimal K_ukMskMrz { get { return this.R_ukMSK - this.R_ukMskPdv - this.S_ukMskPNP - this.Ira_ROB_NV/*R_Ira_NV*/; } } // za Reporte 'K_' (tamo di NEMAS rtrans-ove) a za ostale bussiness upotrebe 'R_' 

   public decimal R_ukMSK      { get { return this.S_ukMSK_00 + this.S_ukMSK_10   + this.S_ukMSK_23     + this.S_ukMSK_25   + this.S_ukMSK_05  ; } }
   public decimal R_ukMskPdv   { get { return                   this.S_ukMskPdv10 + this.S_ukMskPdv23   + this.S_ukMskPdv25 + this.S_ukMskPdv05; } }
                               
   public decimal R_ukKCR_rob  { get { return this.S_ukKCR        - this.S_ukKCR_usl ; } }
 //public decimal R_ukKCRP_rob { get { return this.S_ukKCRP       - this.S_ukKCRP_usl; } }
   public decimal R_ukKCRP_rob { get { return this.R_ukKCRPwoPPMV - this.S_ukKCRP_usl; } }

   public decimal R_IrmRobRbtZad { get { return -1M * (this.R_ukMSK - this.R_ukKCRP_rob); } } // sluzi za KPM_zaduzenje 
   public decimal R_IrmRobRbt    { get { return       (this.R_ukMSK - this.R_ukKCRP_rob); } }

   public decimal KPM_zaduzenje
   {
      get
      {
              if(this.TtInfo.IsNivelacijaZPC) return R_ukMSK ;
         // 04.11.2016: dodan NIV (prebacen iz zaduzenja u razduzenje)
         // 02.06.2021: overridano ovo od 4.11.(else if(this.TT == TT_NIV) return 0.00M;) pa opet vraceno na staro 
       //else if(this.TT == TT_NIV       )    return 0.00M   ;
         else if(this.TT == TT_NIV       )    return R_ukMSK ;
         else if(this.TT == TT_NUV       )    return R_ukMSK ;
         
         else if(this.TtInfo.IsMalopFin_U)    return R_ukMSK ;
         else if(this.TT == TT_VMI       )    return R_ukMSK ;
         else if(this.TT == TT_TRI       )    return R_ukMSK ;
         else if(this.TT == TT_IZM       )    return -R_ukMSK;
         else if(this.TT == TT_MVI       )    return 0.00M   ;
         else if(this.TtInfo.IsMalopFin_I)    return R_IrmRobRbtZad;
         //else throw new Exception("'KPM_zaduzenje' nema smisla ako NIJE Malop!");
         else return 0M;
      }
   }

   public decimal KPM_razduzenje_rob
   {
      get
      {
            //if(this.TtInfo.IsMalopFin_U)                      return 0.00M;
              if(this.TtInfo.IsMalopFin_U || this.TT == TT_IZM) return 0.00M;
         else if(this.TT == TT_MVI       ) return this.R_ukMSK     ;
         // 04.11.2016: dodan NIV (prebacen iz zaduzenja u razduzenje)
         // 02.06.2021: overridano ovo od 4.11.(else if(this.TT == TT_NIV) return -this.R_ukMSK;) pa opet vraceno na staro 
       //else if(this.TT == TT_NIV       ) return -this.R_ukMSK    ; // SAMO za shadowFaktur! 
         else if(this.TT == TT_NIV       ) return 0.00M            ; // SAMO za shadowFaktur! 
         else if(this.TT == TT_NUV       ) return 0.00M            ; // SAMO za shadowFaktur! 

         // 04.11.2016: za Rekap IRM: CrossData: 
       //else if(this.TtInfo.IsMalopFin_I) return this.R_ukKCRP_rob;
         else if(this.TtInfo.IsMalopFin_I) return this.R_ukKCRP_rob - R_NivVrj;
         //else throw new Exception("'KPM_razduzenje_rob' nema smisla ako NIJE Malop!");
         else return 0M;
      }
   }

   public decimal KPM_razduzenje_usl
   {
      get
      {
            //if(this.TtInfo.IsMalopFin_U)                      return 0.00M;
              if(this.TtInfo.IsMalopFin_U || this.TT == TT_IZM) return 0.00M;
         else if(this.TtInfo.IsMalopFin_I) return this.S_ukKCRP_usl;
         //else throw new Exception("'KPM_razduzenje_usl' nema smisla ako NIJE Malop!");
         else return 0M;
      }
   }

   public bool IsFillingFrom_Fak2Nal_URM { get { return this.TheEx.IsFillingFrom_Fak2Nal_URM; } set { this.TheEx.IsFillingFrom_Fak2Nal_URM = value; } }
   public bool IsFillingFrom_Fak2Nal_IRM { get { return this.TheEx.IsFillingFrom_Fak2Nal_IRM; } set { this.TheEx.IsFillingFrom_Fak2Nal_IRM = value; } }

   public bool IsVirmanIRM { get { return this.NacPlac.ToLower().Contains("virman") || this.NacPlac.ToLower().Contains("transakcijski"); } }

   #endregion IRA/IRM & KPM Info Propertiz And Metodz

   #region PPMV Result (Broj Sasije, Godina Proizvodnje) + VATnumber

   public bool IsUMJETNINA 
   { 
      get 
      {
         if(this.Transes == null || this.Transes.Count.IsZero()) return false;
         
         return this.Transes.Any(rtr => rtr.T_pdvColTip == ZXC.PdvKolTipEnum.UMJETN); 
      } 
   }

   public bool IsAVANS_STORNO
   {
      get
      {
         if(this.Transes == null || this.Transes.Count.IsZero()) return false;

         return this.Transes.Any(rtr => rtr.T_pdvColTip == ZXC.PdvKolTipEnum.AVANS_STORNO);
      }
   }

   public bool IsPPMV { get { return this.S_ukPpmvOsn.NotZero(); } }

   public string R_BrojSasije
   {
      get
      {
         try
         {
            if(this.IsPPMV == false) return "";

            string[] lines = this.Opis.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if(lines.Length < 5) return "";

            string sasijaString = lines.SingleOrDefault(str => str.Contains("ASIJE"));

            sasijaString = sasijaString.Replace("BROJ ŠASIJE:", "").Replace("\t", "").TrimStart(' ').TrimEnd(' '); ;

            return sasijaString;
         }
         catch
         {
            return "";
         }
      }
   }

   public string R_GodProizv
   {
      get
      {
         try
         {
            if(this.IsPPMV == false) return "";

            string[] lines = this.Opis.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if(lines.Length < 5) return "";

            string godinaString = lines.SingleOrDefault(str => str.Contains("proizvodnje"));

            godinaString = godinaString.Replace("God. proizvodnje:", "").Replace("\t", "").TrimStart(' ').TrimEnd(' '); ;

            return godinaString;
         }
         catch
         {
            return "";
         }
      }
   }

   /// <summary>
   /// Ovaj sluzi da se covjek ne muci, pa ako i nije upis'o 'HR' da mu ga sam ispunis
   /// </summary>
   public string VATnumber
   {
      get
      {
         string _vatCntryCode = this.VatCntryCode.IsEmpty() ? "HR" : this.VatCntryCode;

         return _vatCntryCode + this.KdOib;
      }
   }

   /// <summary>
   /// Ovaj sluzi da očisti eventualnu upisninu 'HR'-a u VATcode a u tuzemnom poslovanju je višak
   /// </summary>
   public string PDV_Broj
   {
      get
      {
         string _vatCntryCode = this.VatCntryCode == "HR" ? "" : this.VatCntryCode;

         return _vatCntryCode + this.KdOib;
      }
   }

   // 5paaaa       
   // 5pIRM-400010 
   // 5pIRA-100002 
   // 4pPS-IRA123  
   // 5pIOD-100001 
   // 5pXY:2015-69 
   public string OpzStat1TtNum
   {
      get
      {
         // classic ovogodisnji racun: 
         if(TtInfo.Is_WYRN_TT == false) return this.TtNumRbr.ToString();


         string vezniDok2_asTtNum = VezniDok2;

         int minusIdx = VezniDok2.LastIndexOf('-');
         if(minusIdx.IsZeroOrPositive() && VezniDok2.Length > (minusIdx + 1) && Char.IsDigit(VezniDok2[minusIdx + 1]))
         {
            string odMinusaDoKraja = VezniDok2.SubstringSafe(minusIdx+1);
            if(odMinusaDoKraja.Length >= 6) vezniDok2_asTtNum = odMinusaDoKraja.SubstringSafe(2).TrimStart('0');
            else                            vezniDok2_asTtNum = odMinusaDoKraja                 .TrimStart('0');
         }

         if(vezniDok2_asTtNum != VezniDok2) return vezniDok2_asTtNum;

       //for(int i = VezniDok2.Length - 1;                                              Char.IsDigit(VezniDok2[VezniDok2.Length - 1]) && i.IsZeroOrPositive(); --i)
         for(int i = VezniDok2.Length - 1; (VezniDok2.Length - 1).IsZeroOrPositive() && Char.IsDigit(VezniDok2[VezniDok2.Length - 1]) && i.IsZeroOrPositive(); --i)
         {
            if(Char.IsDigit(VezniDok2[i]) == false)
            {
               if(i + 1 != VezniDok2.Length) { vezniDok2_asTtNum = VezniDok2.SubstringSafe(i+1); break; }
            }
         }

         if(vezniDok2_asTtNum != VezniDok2) return vezniDok2_asTtNum;

         // jos neka kemijanja? 

         return vezniDok2_asTtNum;
      }
   }

   #endregion PPMV Result (Broj Sasije, Godina Proizvodnje)

   #region Textho_IRM_KPN, SVD_LJEK_POTROS, Is_AfterAvans_PrihodTTa

   // 07.03.2018: 
   public bool IsTextho_IRM_KPN_Izdan
   {
      get
      {
         if(this.TT != TT_IRM || ZXC.IsTEXTHOany == false) return false;

         return this.SomePercent.NotZero() && this.S_ukKCRP >= this.SomeMoney;
      }
   }

   // 07.03.2018: 
   public bool IsTextho_IRM_KPN_Zaprimljen
   {
      get
      {
         if(this.TT != TT_IRM || ZXC.IsTEXTHOany == false) return false;

         return this.Decimal02.NotZero();
      }
   }

   // 30.01.2019: 
   public bool IsSVD_LJEK
   {
      get
      {
         if(ZXC.IsSvDUH == false)                   return false;
         if(this.TT != TT_URA && this.TT != TT_NRD) return false;

         return this.PdvZPkind == ZXC.PdvZPkindEnum.SVD_LJEK;
      }
   }

   public bool IsSVD_POTR
   {
      get
      {
         if(ZXC.IsSvDUH == false)                   return false;
         if(this.TT != TT_URA && this.TT != TT_NRD) return false;

         return this.PdvZPkind == ZXC.PdvZPkindEnum.SVD_POTR;
      }
   }

   public string SVD_Knjigovod_SkladCD
   {
      get
      {
       //if(ZXC.IsSvDUH == false || SkladCD != "10") return SkladCD;
         if(ZXC.IsSvDUH == false                   ) return SkladCD;

         return SkladCD + SVD_Knjigovod_L_Or_P_Str;
      }
   }

   public string SVD_Knjigovod_TtNum
   {
      get
      {
         if(ZXC.IsSvDUH == false) return TT_And_TtNum;

         return VezniDok2 + SVD_Knjigovod_L_Or_P_Str;
      }
   }

   public string SVD_Knjigovod_L_Or_P_Str
   {
      get
      {
         if(ZXC.IsSvDUH == false) return "";

         return IsSVD_LJEK ? "L" : "P";
      }
   }

   // 14.06.2019: za potrebe eRacuna:
   public bool Is_AfterAvans_PrihodTTa
   {
      get
      {
         if(this.TtInfo.IsPrihodTT == false) return false;

         return this.Napomena.ToUpper().Contains("NAKON AVANSA");
      }
   }

   public bool Is_STORNO
   {
      get
      {
         if(this.Napomena.IsEmpty()) return false;

         return this.Napomena.ToUpper().Contains("STORNO");
      }
   }

   public bool Is_STORNOforNewNP
   {
      get
      {
         if(this.Napomena.IsEmpty()) return false;

         return this.Is_STORNO && this.Napomena.ToUpper().Contains("ZANOVINP");
      }
   }

   public bool Is_STORNO_NOT_forNewNP // real storno 
   {
      get
      {
         if(this.Napomena.IsEmpty()) return false;

         return this.Is_STORNO         == true && 
                this.Is_STORNOforNewNP == false;
      }
   }

   #endregion Textho_IRM_KPN, SVD_LJEK_POTROS, Is_AfterAvans_PrihodTTa

   #region HRD

   /// <summary>
   /// Kune u EURe, EURi iz Kuna
   /// </summary>
   public decimal S_ukKCRP_HRD_K2E { get { return ZXC.EURiIzKuna_HRD_(S_ukKCRP); } }
   /// <summary>
   /// EURi u Kune,  Kune iz EURa
   /// </summary>
   public decimal S_ukKCRP_HRD_E2K { get { return ZXC.KuneIzEURa_HRD_(S_ukKCRP); } }

   public string S_ukKCRP_HRD_K2Es { get { return S_ukKCRP_HRD_K2E.ToStringVv() + " EUR"; } }
   public string S_ukKCRP_HRD_E2Ks { get { return S_ukKCRP_HRD_E2K.ToStringVv() + " HRK"; } }

   #endregion HRD

   public int MER_ElectronicID
   {
      get
      {
         if(FiskPrgBr.IsEmpty()         ) return 0;
         if(TT != TT_IFA && TT != TT_IRA) return 0;

         int braceOpenIdx  = this.FiskPrgBr.IndexOf('[');
         int braceCloseIdx = this.FiskPrgBr.IndexOf(']');
         int digitStrLen   = braceCloseIdx - braceOpenIdx - 1;

         string elID_digits = this.FiskPrgBr.SubstringSafe(braceOpenIdx + 1, digitStrLen);

         return ZXC.ValOrZero_Int(elID_digits);
      }
   }

   #region PCTOGO / PTG

   internal int PTG_NEEDS_This_RtranoWith_Serno_Count 
   { 
      get 
      {
         if(this.TtInfo.HasRtranoForSernoTT == false) return 0;

         return (int)this.TrnNonDel.Where(rtr => VvForm.Does_thisRtransNeeds_RtranoRow_ForSerno(rtr.T_artiklCD)).Sum(rtr => rtr.T_kol); 
      } 
   }
   internal int PTG_HAS_This_RtranoWith_Serno_Count 
   { 
      get 
      {
         if(this.TtInfo.HasRtranoForSernoTT == false) return 0;

         return this.TrnNonDel2.Count(rto => rto.T_serno.NotEmpty()); 
      } 
   }

   /// <summary>
   /// koliko ih nedostaje
   /// </summary>
   internal int PTG_MISSES_This_RtranoWith_Serno_Count
   {
      get
      {
         return PTG_NEEDS_This_RtranoWith_Serno_Count - PTG_HAS_This_RtranoWith_Serno_Count;
      }
   }

   #endregion PCTOGO / PTG

   #endregion Propertiz

   #region ToString

   public override string ToString()
   {
      return string.Format("TT: {1} ({2}) ({0}) ({3}) ", DokDate.ToShortDateString(), TT, TtNum, (this.IsExtendable ? KupdobName : ""));
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<FakturStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<FakturStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<FakturStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<FakturStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<FakturStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Faktur newObject = new Faktur();

      Generic_CloneData<FakturStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      if(this.IsExtendable) newObject.TheEx = this.TheEx.MakeDeepCopy();

      // !!! NOTA BENE + PAZI for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 

      newObject.Skn_ukKC   = this.Skn_ukKC  ;

      return newObject;
   }

   public Faktur MakeDeepCopy()
   {
      return (Faktur)Clone();
   }

   public override void TakeTheseTranses(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes = transList.ConvertAll(trans => trans as Rtrans);
   }

   public override void TakeTheseTranses2(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes2 = transList.ConvertAll(trans => trans as Rtrano);
   }

   public override void TakeTheseTranses3(List<VvTransRecord> transList)
   {
      if(transList.IsEmpty()) return;

      this.Transes3 = transList.ConvertAll(trans => trans as Rtrano);
   }

   public override void TakeTransesFrom(VvDocumentRecord _vvDocumentRecord)
   {
      if(_vvDocumentRecord.VirtualTranses == null) return;

      this.Transes = _vvDocumentRecord.CloneTranses().ConvertAll(trans => trans as Rtrans);
   }

   public override void TakeTransesFrom2(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses2 == null) return;

      this.Transes2 = _vvPolyDocumRecord.CloneTranses2().ConvertAll(trans2 => trans2 as Rtrano);
   }

   public override void TakeTransesFrom3(VvPolyDocumRecord _vvPolyDocumRecord)
   {
      if(_vvPolyDocumRecord.VirtualTranses3 == null) return;

      this.Transes3 = _vvPolyDocumRecord.CloneTranses3().ConvertAll(trans3 => trans3 as Rtrano);
   }

   public override List<VvTransRecord> CloneTranses()
   {
      if(this.Transes == null) return null;

      List<Rtrans> newList = new List<Rtrans>(this.Transes.Count);

      foreach(Rtrans rtrans_rec in this.Transes)
      {
         newList.Add((Rtrans)rtrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   public /*override*/ List<VvTransRecord> CloneTransesNonDeletedWithPULX() // 23.02.2013 
   {
      if(this.TrnNonDel == null) return null;

      List<Rtrans> newList = new List<Rtrans>(this.TrnNonDel.Count() + this.TrnNonDel_PULX_ALL.Count());

      foreach(Rtrans rtrans_rec in this.TrnNonDel)
      {
         newList.Add((Rtrans)rtrans_rec.Clone());
      }

      foreach(Rtrans rtrans_rec in this.TrnNonDel_PULX_ALL)
      {
         newList.Add((Rtrans)rtrans_rec.Clone());
      }

      return (newList.ConvertAll(trans => trans as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses2()
   {
      if(this.Transes2 == null) return null;

      List<Rtrano> newList = new List<Rtrano>(this.Transes2.Count);

      foreach(Rtrano trans_rec in this.Transes2)
      {
         newList.Add((Rtrano)trans_rec.Clone());
      }

      return (newList.ConvertAll(trans2 => trans2 as VvTransRecord));
   }

   public override List<VvTransRecord> CloneTranses3()
   {
      if(this.Transes3 == null) return null;

      List<Rtrano> newList = new List<Rtrano>(this.Transes3.Count);

      foreach(Rtrano trans_rec in this.Transes3)
      {
         newList.Add((Rtrano)trans_rec.Clone());
      }

      return (newList.ConvertAll(trans3 => trans3 as VvTransRecord));
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Faktur();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)  
   {
      uint origFakturRecID = this.currentData._recID;

      this.currentData = ((Faktur)vvDataRecord).currentData;

      this.currentData._recID = origFakturRecID;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      Faktur incoming_faktur_rec = ((Faktur)vvDataRecord);

      this.backupData = ((Faktur)vvDataRecord).currentData;
   }

   public override VvTransRecord VvTransRecordFactory()
   {
      return new Rtrans();
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
               try
               {
                  if(IsExtendable == false && pInfo.Name != "S_ukKC") continue;

                  pInfo.SetValue(this, ZXC.EURiIzKuna_HRD_((decimal)pInfo.GetValue(this)));
               }
               catch { }
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
               try
               {
                  if(IsExtendable == false && pInfo.Name != "S_ukKC") continue;

                  pInfo.SetValue(this, ZXC.KuneIzEURa_HRD_((decimal)pInfo.GetValue(this)));
               }
               catch { }
            }
         }
      }

      return this.EditedHasChanges();
   }

   #endregion VvDataRecordFactory
   
   #region IVvExtendableDataRecord Members

   public string ExtenderTableName
   {
      get
      {
         return FaktEx.recordName;
      }
   }

   public string ExtenderTableNameArhiva
   {
      get
      {
         return FaktEx.recordNameArhiva;
      }
   }

   public void TakeExtenderDataFrom(VvDataRecord vvExtenderDataRecord)
   {
      this.TheEx = (FaktEx)vvExtenderDataRecord.Clone();
   }

   public void TakeExtender_Backup_DataFrom(VvDataRecord vvExtenderDataRecord)
   {
      if(this.IsExtendable) this.TheEx.BackupData = ((FaktEx)vvExtenderDataRecord).currentData;
   }

   #endregion

   #region IComparable<Faktur> Members

   // (t_artiklCD, t_skladDate, t_skladCD,   t_ttSort, t_ttNum, t_serial) NE!
   // (t_artiklCD, t_skladCD,   t_skladDate, t_ttSort, t_ttNum, t_serial) 

   // NEMOJ ovdje koristiti overridani Equals(), jer onda sjebes nesto za InvokeTransRemove!
   private bool ThisIsEqualTo(Faktur other)
   {
      if(other == null) return false;

      return
         (
            PdvDate == other.PdvDate &&
            TtSort  == other.TtSort  &&
            TtNum   == other.TtNum
         );
   }

   public int CompareTo(Faktur other)
   {
           if(ThisIsEqualTo  (other)) return  0;
      else if(ThisIsGreatThen(other)) return  1;
      else if(ThisIsLessThen (other)) return -1;

      else throw new Exception("Faktur.CompareTo BUMMER!");
   }

   private bool ThisIsGreatThen(Faktur other)
   {
      if
      (
      (PdvDate >  other.PdvDate) ||
      (PdvDate == other.PdvDate && TtSort  > other.TtSort) ||
      (PdvDate == other.PdvDate && TtSort == other.TtSort && TtNum > other.TtNum)
      )    return true;
      else return false;
   }

   private bool ThisIsLessThen(Faktur other)
   {
      if
      (
      (PdvDate <  other.PdvDate) ||
      (PdvDate == other.PdvDate && TtSort  < other.TtSort) ||
      (PdvDate == other.PdvDate && TtSort == other.TtSort && TtNum < other.TtNum)
      )    return true;
      else return false;
   }

   #endregion IComparable<Faktur> Members

   #region Custom Metodz

   public int FirstEmptyVezaLine
   {
      get
      {

         if(V1_tt.IsEmpty() && V1_ttNum.IsZero()) return 1;
         if(V2_tt.IsEmpty() && V2_ttNum.IsZero()) return 2;

         if(this.TheEx == null) return 0;

         if(V3_tt.IsEmpty() && V3_ttNum.IsZero()) return 3;
         if(V4_tt.IsEmpty() && V4_ttNum.IsZero()) return 4;

         return 0;
      }
   }

   internal uint GetFirstVezaTtNumForTT(string theTT)
   {
      if(V1_tt == theTT) return V1_ttNum;
      if(V2_tt == theTT) return V2_ttNum;

      if(this.TheEx == null) return 0;

      if(V3_tt == theTT) return V3_ttNum;
      if(V4_tt == theTT) return V4_ttNum;

      return 0;

   }

   public void ConvertBussinessValuesToDeviza(decimal tecaj, ZXC.ValutaNameEnum presentValutaEnum)
   {

      ConvertAllConvertibilePropertiesOf(this, typeof(Faktur), tecaj, presentValutaEnum);

      foreach(Rtrans rtrans in Transes)
      {
         ConvertAllConvertibilePropertiesOf(rtrans, typeof(Rtrans), tecaj, presentValutaEnum);
      }
   }

   private void ConvertAllConvertibilePropertiesOf(object objToChange, Type typeOfObjToChange, decimal tecaj, ZXC.ValutaNameEnum presentValutaEnum)
   {
      foreach(PropertyInfo pInfo in typeOfObjToChange.GetProperties())
      {
         if(pInfo.PropertyType != typeof(decimal)) continue;

         foreach(Attribute attr in pInfo.GetCustomAttributes(typeof(VvIsDevizaConvertibileAttribute), false))
         {
            VvIsDevizaConvertibileAttribute isConvertibileAttr = attr as VvIsDevizaConvertibileAttribute;

            if(isConvertibileAttr != null && isConvertibileAttr.JeLiJeTakav == ZXC.JeliJeTakav.JE_TAKAV)
            {
               pInfo.SetValue(objToChange, ConvertToOtherValuta((decimal)pInfo.GetValue(objToChange, null), tecaj, presentValutaEnum), null);
            }
         }
      }

   }

   private decimal ConvertToOtherValuta(decimal money, decimal tecaj, ZXC.ValutaNameEnum presentValutaEnum)
   {
      if(presentValutaEnum == ZXC.ValutaNameEnum.EMPTY ||
         presentValutaEnum == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum)
      {
         return money * tecaj;
      }
      else
      {
         return ZXC.DivSafe(money, tecaj);
      }
   }

   internal void SumValuesFromList(List<Faktur> fakturList)
   {
      if(fakturList == null) return;

      this.S_ukK       = fakturList.Sum(f => f.S_ukK      );
      this.S_ukK2      = fakturList.Sum(f => f.S_ukK2     );
      this.S_ukKC      = fakturList.Sum(f => f.S_ukKC     );

      this.S_ukKCRP    = fakturList.Sum(f => f.S_ukKCRP   );
      this.S_ukKCRM    = fakturList.Sum(f => f.S_ukKCRM   );
      this.S_ukKCR     = fakturList.Sum(f => f.S_ukKCR    );
      this.S_ukRbt1    = fakturList.Sum(f => f.S_ukRbt1   );
      this.S_ukRbt2    = fakturList.Sum(f => f.S_ukRbt2   );
      this.S_ukZavisni = fakturList.Sum(f => f.S_ukZavisni);
      this.S_ukPdv25m  = fakturList.Sum(f => f.S_ukPdv25m );
      this.S_ukPdv25n  = fakturList.Sum(f => f.S_ukPdv25n );
      this.S_ukPdv05m  = fakturList.Sum(f => f.S_ukPdv05m );
      this.S_ukPdv05n  = fakturList.Sum(f => f.S_ukPdv05n );
      this.S_ukPdv23m  = fakturList.Sum(f => f.S_ukPdv23m );
      this.S_ukPdv23n  = fakturList.Sum(f => f.S_ukPdv23n );
      this.S_ukPdv22m  = fakturList.Sum(f => f.S_ukPdv22m );
      this.S_ukPdv22n  = fakturList.Sum(f => f.S_ukPdv22n );
      this.S_ukPdv10m  = fakturList.Sum(f => f.S_ukPdv10m );
      this.S_ukPdv10n  = fakturList.Sum(f => f.S_ukPdv10n );
      this.S_ukOsn25m  = fakturList.Sum(f => f.S_ukOsn25m );
      this.S_ukOsn25n  = fakturList.Sum(f => f.S_ukOsn25n );
      this.S_ukOsn05m  = fakturList.Sum(f => f.S_ukOsn05m );
      this.S_ukOsn05n  = fakturList.Sum(f => f.S_ukOsn05n );
      this.S_ukOsn23m  = fakturList.Sum(f => f.S_ukOsn23m );
      this.S_ukOsn23n  = fakturList.Sum(f => f.S_ukOsn23n );
      this.S_ukOsn22m  = fakturList.Sum(f => f.S_ukOsn22m );
      this.S_ukOsn22n  = fakturList.Sum(f => f.S_ukOsn22n );
      this.S_ukOsn10m  = fakturList.Sum(f => f.S_ukOsn10m );
      this.S_ukOsn10n  = fakturList.Sum(f => f.S_ukOsn10n );
      this.S_ukOsn0    = fakturList.Sum(f => f.S_ukOsn0   );
      this.S_ukOsnPr   = fakturList.Sum(f => f.S_ukOsnPr  );
      this.S_ukPdv     = fakturList.Sum(f => f.S_ukPdv    );
   
      this.S_ukOsn07   = fakturList.Sum(f => f.S_ukOsn07  );
      this.S_ukOsn08   = fakturList.Sum(f => f.S_ukOsn08  );
      this.S_ukOsn09   = fakturList.Sum(f => f.S_ukOsn09  );
      this.S_ukOsn10   = fakturList.Sum(f => f.S_ukOsn10  );
      this.S_ukOsn11   = fakturList.Sum(f => f.S_ukOsn11  );
      this.S_ukOsnUr05 = fakturList.Sum(f => f.S_ukOsnUr05);
      this.S_ukOsnUr25 = fakturList.Sum(f => f.S_ukOsnUr25);
      this.S_ukOsnUr23 = fakturList.Sum(f => f.S_ukOsnUr23);
      this.S_ukOsnUu10 = fakturList.Sum(f => f.S_ukOsnUu10);
      this.S_ukOsnUu22 = fakturList.Sum(f => f.S_ukOsnUu22);
      this.S_ukOsnUu25 = fakturList.Sum(f => f.S_ukOsnUu25);
      this.S_ukPdvUr05 = fakturList.Sum(f => f.S_ukPdvUr05);
      this.S_ukPdvUr25 = fakturList.Sum(f => f.S_ukPdvUr25);
      this.S_ukOsnUu23 = fakturList.Sum(f => f.S_ukOsnUu23);
      this.S_ukPdvUr23 = fakturList.Sum(f => f.S_ukPdvUr23);
      this.S_ukPdvUu10 = fakturList.Sum(f => f.S_ukPdvUu10);
      this.S_ukPdvUu22 = fakturList.Sum(f => f.S_ukPdvUu22);
      this.S_ukPdvUu23 = fakturList.Sum(f => f.S_ukPdvUu23);
      this.S_ukPdvUu25 = fakturList.Sum(f => f.S_ukPdvUu25);

      this.S_ukBlg_UPL = fakturList.Sum(f => f.S_Blg_UPL);
      this.S_ukBlg_ISP = fakturList.Sum(f => f.S_Blg_ISP);

      this.S_pixK    = fakturList.Sum(f => f.S_pixK   );
      this.S_puxK_P  = fakturList.Sum(f => f.S_puxK_P );
      this.S_puxK_All  = fakturList.Sum(f => f.S_puxK_All );
      this.S_pixKC   = fakturList.Sum(f => f.S_pixKC  );
      this.S_puxKC_P = fakturList.Sum(f => f.S_puxKC_P);
      this.S_puxKC_All = fakturList.Sum(f => f.S_puxKC_All);
      this.S_ukPpmvOsn   = fakturList.Sum(f => f.S_ukPpmvOsn  );
    //this.S_ukPpmvSt1i2 = fakturList.Sum(f => f.S_ukPpmvSt1i2); tu si malo zapeo s idejom 
    //this.R_ukPpmvIzn   = fakturList.Sum(f => f.R_ukPpmvIzn);

      this.S_ukOsnR25m_EU = fakturList.Sum(f => f.S_ukOsnR25m_EU);
      this.S_ukOsnR25n_EU = fakturList.Sum(f => f.S_ukOsnR25n_EU);
      this.S_ukOsnU25m_EU = fakturList.Sum(f => f.S_ukOsnU25m_EU);
      this.S_ukOsnU25n_EU = fakturList.Sum(f => f.S_ukOsnU25n_EU);
      this.S_ukOsnR10m_EU = fakturList.Sum(f => f.S_ukOsnR10m_EU);
      this.S_ukOsnR10n_EU = fakturList.Sum(f => f.S_ukOsnR10n_EU);
      this.S_ukOsnU10m_EU = fakturList.Sum(f => f.S_ukOsnU10m_EU);
      this.S_ukOsnU10n_EU = fakturList.Sum(f => f.S_ukOsnU10n_EU);
      this.S_ukOsnR05m_EU = fakturList.Sum(f => f.S_ukOsnR05m_EU);
      this.S_ukOsnR05n_EU = fakturList.Sum(f => f.S_ukOsnR05n_EU);
      this.S_ukOsnU05m_EU = fakturList.Sum(f => f.S_ukOsnU05m_EU);
      this.S_ukOsnU05n_EU = fakturList.Sum(f => f.S_ukOsnU05n_EU);
      this.S_ukOsn25m_BS  = fakturList.Sum(f => f.S_ukOsn25m_BS );
      this.S_ukOsn25n_BS  = fakturList.Sum(f => f.S_ukOsn25n_BS );
      this.S_ukOsn10m_BS  = fakturList.Sum(f => f.S_ukOsn10m_BS );
      this.S_ukOsn10n_BS  = fakturList.Sum(f => f.S_ukOsn10n_BS );
      this.S_ukOsn25m_TP  = fakturList.Sum(f => f.S_ukOsn25m_TP );
      this.S_ukOsn25n_TP  = fakturList.Sum(f => f.S_ukOsn25n_TP );
      this.S_ukPdvR25m_EU = fakturList.Sum(f => f.S_ukPdvR25m_EU);
      this.S_ukPdvR25n_EU = fakturList.Sum(f => f.S_ukPdvR25n_EU);
      this.S_ukPdvU25m_EU = fakturList.Sum(f => f.S_ukPdvU25m_EU);
      this.S_ukPdvU25n_EU = fakturList.Sum(f => f.S_ukPdvU25n_EU);
      this.S_ukPdvR10m_EU = fakturList.Sum(f => f.S_ukPdvR10m_EU);
      this.S_ukPdvR10n_EU = fakturList.Sum(f => f.S_ukPdvR10n_EU);
      this.S_ukPdvU10m_EU = fakturList.Sum(f => f.S_ukPdvU10m_EU);
      this.S_ukPdvU10n_EU = fakturList.Sum(f => f.S_ukPdvU10n_EU);
      this.S_ukPdvR05m_EU = fakturList.Sum(f => f.S_ukPdvR05m_EU);
      this.S_ukPdvR05n_EU = fakturList.Sum(f => f.S_ukPdvR05n_EU);
      this.S_ukPdvU05m_EU = fakturList.Sum(f => f.S_ukPdvU05m_EU);
      this.S_ukPdvU05n_EU = fakturList.Sum(f => f.S_ukPdvU05n_EU);
      this.S_ukPdv25m_BS  = fakturList.Sum(f => f.S_ukPdv25m_BS );
      this.S_ukPdv25n_BS  = fakturList.Sum(f => f.S_ukPdv25n_BS );
      this.S_ukPdv10m_BS  = fakturList.Sum(f => f.S_ukPdv10m_BS );
      this.S_ukPdv10n_BS  = fakturList.Sum(f => f.S_ukPdv10n_BS );
      this.S_ukPdv25m_TP  = fakturList.Sum(f => f.S_ukPdv25m_TP );
      this.S_ukPdv25n_TP  = fakturList.Sum(f => f.S_ukPdv25n_TP );
      this.S_ukOsnZP_11   = fakturList.Sum(f => f.S_ukOsnZP_11  );
      this.S_ukOsnZP_12   = fakturList.Sum(f => f.S_ukOsnZP_12  );
      this.S_ukOsnZP_13   = fakturList.Sum(f => f.S_ukOsnZP_13  );
      this.S_ukOsn12      = fakturList.Sum(f => f.S_ukOsn12     );
      this.S_ukOsn13      = fakturList.Sum(f => f.S_ukOsn13     );
      this.S_ukOsn14      = fakturList.Sum(f => f.S_ukOsn14     );
      this.S_ukOsn15      = fakturList.Sum(f => f.S_ukOsn15     );
      this.S_ukOsn16      = fakturList.Sum(f => f.S_ukOsn16     );
      this.S_ukOsnPNP     = fakturList.Sum(f => f.S_ukOsnPNP    );
      this.S_ukIznPNP     = fakturList.Sum(f => f.S_ukIznPNP    );
      this.S_ukMskPNP     = fakturList.Sum(f => f.S_ukMskPNP    );
      this.Skiz_ukKC      = fakturList.Sum(f => f.Skiz_ukKC     );
      this.Skiz_ukKCR     = fakturList.Sum(f => f.Skiz_ukKCR    );
      this.Skiz_ukRbt1    = fakturList.Sum(f => f.Skiz_ukRbt1   );
      this.S_ukKCRP_NP1   = fakturList.Sum(f => f.S_ukKCRP_NP1  );
   }

   public void RatioValuesOnR2Uplata()
   {
      decimal ratio = ZXC.DivSafe(this.R2_uplata, this.S_ukKCRP);

    //this.S_ukK       = fakturList.Sum(f => f.S_ukK      );
      this.S_ukKC      *= ratio;
      this.S_ukKCRP    *= ratio;
      this.S_ukKCRM    *= ratio;
      this.S_ukKCR     *= ratio;
      this.S_ukRbt1    *= ratio;
      this.S_ukRbt2    *= ratio;
      this.S_ukZavisni *= ratio;
      this.S_ukPdv25m  *= ratio;
      this.S_ukPdv25n  *= ratio;
      this.S_ukPdv05m  *= ratio;
      this.S_ukPdv05n  *= ratio;
      this.S_ukPdv23m  *= ratio;
      this.S_ukPdv23n  *= ratio;
      this.S_ukPdv22m  *= ratio;
      this.S_ukPdv22n  *= ratio;
      this.S_ukPdv10m  *= ratio;
      this.S_ukPdv10n  *= ratio;
      this.S_ukOsn25m  *= ratio;
      this.S_ukOsn25n  *= ratio;
      this.S_ukOsn05m  *= ratio;
      this.S_ukOsn05n  *= ratio;
      this.S_ukOsn23m  *= ratio;
      this.S_ukOsn23n  *= ratio;
      this.S_ukOsn22m  *= ratio;
      this.S_ukOsn22n  *= ratio;
      this.S_ukOsn10m  *= ratio;
      this.S_ukOsn10n  *= ratio;
      this.S_ukOsn0    *= ratio;
      this.S_ukOsnPr   *= ratio;
      this.S_ukPdv     *= ratio;
      this.S_ukOsn07   *= ratio;
      this.S_ukOsn08   *= ratio;
      this.S_ukOsn09   *= ratio;
      this.S_ukOsn10   *= ratio;
      this.S_ukOsn11   *= ratio;
      this.S_ukOsnUr05 *= ratio;
      this.S_ukOsnUr25 *= ratio;
      this.S_ukOsnUr23 *= ratio;
      this.S_ukOsnUu10 *= ratio;
      this.S_ukOsnUu22 *= ratio;
      this.S_ukOsnUu25 *= ratio;
      this.S_ukPdvUr05 *= ratio;
      this.S_ukPdvUr25 *= ratio;
      this.S_ukOsnUu23 *= ratio;
      this.S_ukPdvUr23 *= ratio;
      this.S_ukPdvUu10 *= ratio;
      this.S_ukPdvUu22 *= ratio;
      this.S_ukPdvUu23 *= ratio;
      this.S_ukPdvUu25 *= ratio;

      this.S_pixK    *= ratio;
      this.S_puxK_P  *= ratio;
      this.S_puxK_All  *= ratio;
      this.S_pixKC   *= ratio;
      this.S_puxKC_P *= ratio;
      this.S_puxKC_All *= ratio;
      this.S_ukPpmvOsn *= ratio;  // ovo je bezveze ali neka je tu 
      this.S_ukPpmvSt1i2 *= ratio;// ovo je bezveze ali neka je tu 

      this.S_ukOsnR25m_EU *= ratio;
      this.S_ukOsnR25n_EU *= ratio;
      this.S_ukOsnU25m_EU *= ratio;
      this.S_ukOsnU25n_EU *= ratio;
      this.S_ukOsnR10m_EU *= ratio;
      this.S_ukOsnR10n_EU *= ratio;
      this.S_ukOsnU10m_EU *= ratio;
      this.S_ukOsnU10n_EU *= ratio;
      this.S_ukOsnR05m_EU *= ratio;
      this.S_ukOsnR05n_EU *= ratio;
      this.S_ukOsnU05m_EU *= ratio;
      this.S_ukOsnU05n_EU *= ratio;
      this.S_ukOsn25m_BS  *= ratio;
      this.S_ukOsn25n_BS  *= ratio;
      this.S_ukOsn10m_BS  *= ratio;
      this.S_ukOsn10n_BS  *= ratio;
      this.S_ukOsn25m_TP  *= ratio;
      this.S_ukOsn25n_TP  *= ratio;
      this.S_ukPdvR25m_EU *= ratio;
      this.S_ukPdvR25n_EU *= ratio;
      this.S_ukPdvU25m_EU *= ratio;
      this.S_ukPdvU25n_EU *= ratio;
      this.S_ukPdvR10m_EU *= ratio;
      this.S_ukPdvR10n_EU *= ratio;
      this.S_ukPdvU10m_EU *= ratio;
      this.S_ukPdvU10n_EU *= ratio;
      this.S_ukPdvR05m_EU *= ratio;
      this.S_ukPdvR05n_EU *= ratio;
      this.S_ukPdvU05m_EU *= ratio;
      this.S_ukPdvU05n_EU *= ratio;
      this.S_ukPdv25m_BS  *= ratio;
      this.S_ukPdv25n_BS  *= ratio;
      this.S_ukPdv10m_BS  *= ratio;
      this.S_ukPdv10n_BS  *= ratio;
      this.S_ukPdv25m_TP  *= ratio;
      this.S_ukPdv25n_TP  *= ratio;
      this.S_ukOsnZP_11   *= ratio;
      this.S_ukOsnZP_12   *= ratio;
      this.S_ukOsnZP_13   *= ratio;
      this.S_ukOsn12      *= ratio;
      this.S_ukOsn13      *= ratio;
      this.S_ukOsn14      *= ratio;
      this.S_ukOsn15      *= ratio;
      this.S_ukOsn16      *= ratio;
      this.S_ukOsnPNP     *= ratio;
      this.S_ukIznPNP     *= ratio;
      this.S_ukMskPNP     *= ratio;
      this.Skiz_ukKC      *= ratio;
      this.Skiz_ukKCR     *= ratio;
      this.Skiz_ukRbt1    *= ratio;
      this.S_ukKCRP_NP1   *= ratio;
   }

   internal string TT_And_TtNum 
   { 
      get 
      {
         string theTT_And_TtNum = Set_TT_And_TtNum(this.TT, this.TtNum);

       //if(this.TT == Faktur.TT_RNM) // check ProjektCD 
       //{
       //   if(theTT_And_TtNum != ProjektCD)
       //   {
       //      ZXC.aim_emsg(MessageBoxIcon.Error, "ProjektCD != TT_And_TtNum\n\n[{0}] [{1}]", ProjektCD, theTT_And_TtNum);
       //   }
       //}

         return theTT_And_TtNum; 
      } 
   }

   public static string Set_TT_And_TtNum(string _tt, uint _ttNum)
   {
      if(_tt.IsEmpty() && _ttNum.IsZero()) return "";

      // 28.02.2020: 
      if(_tt == Faktur.TT_RNZ || _tt == Faktur.TT_UGN || _tt == Faktur.TT_AUN)
      {
         return _tt + "-" + _ttNum.ToString("0000000");
      }

      return _tt + "-" + _ttNum;
   }

   internal string TipBr        
   { 
      get 
      {
       //26.10.2021. za knizenje samo ios-a svi imaju puni broj                                             
       //if(TT == TT_IRM)                                            return IsVirmanIRM ? TT_And_TtNum : TT;
       //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA
       //if(TT == TT_IRM && ZXC.KSD.Dsc_IsOnlyIOSknjizenje == false) return IsVirmanIRM ? TT_And_TtNum : TT;
         if(TT == TT_IRM && ZXC.KSD.Dsc_ForceIRMkaoIRA     == false) return IsVirmanIRM ? TT_And_TtNum : TT;
         else                                                        return               TT_And_TtNum; 
      } 
   }
   internal string TipBr4RUC
   {
      get
      {
         // 26.02.2020: ovo ubacivanje '9p' na pocetak smeta kada uplata zatvara YRN-ove 
         if(TtInfo.Is_WYRN_TT) return TipBr;

         if(this.DokDate.Year < ZXC.projectYearFirstDay.Year) return NalogDao.GetTBRforPsNalog(TipBr);
         else                                                 return TipBr                           ;
      }
   }

   //internal string OTS_ID
   //{
   //   get
   //   {
   //      return KupdobCD.ToString() + "_" + TipBr4RUC;
   //   }
   //}

   internal string TtSort_And_TtNum { get { return (this.TtSort.ToString() + this.TtNum.ToString()); } }

   #region Faktur 2 DSC link

   private bool dscLoaded = false;

   private PrnFakDsc thePFD;
   public  PrnFakDsc ThePFD 
   {
      get
      {
         if(dscLoaded == false)
         {
            dscLoaded = true;

            #region subDsc

            ushort subDsc = 0;

            // ... tu bu jos sranja!

            FakturDocFilter theFilter = null;

            if(ZXC.TheVvForm.TheVvUC is FakturDUC) theFilter = (ZXC.TheVvForm.TheVvRecordUC as FakturDUC).VirtualRptFilter as FakturDocFilter;
            

            if(theFilter != null) subDsc = theFilter.ChosenObrazac;

            // 16.02.2022:
            if(ZXC.FakturList_To_PDF_InProgress)
            {
               subDsc = ZXC.FakturList_To_PDF_subDsc;
            }

            #endregion subDsc

            thePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(this.TT, subDsc));
         }

         return thePFD;
      }
      //set
      //{
      //   thePFD = value;
      //}
   }

   public string VvDocumIdent 
   {
      get 
      { 
         System.Text.StringBuilder theIdent = new System.Text.StringBuilder("");

         if(ThePFD.Dsc_OcuR12)               theIdent.Append("R-"                    + this.PdvR12_u);
         if(ThePFD.Dsc_Title.NotEmpty())     theIdent.Append("    " + ThePFD.Dsc_Title + "  ");
         if(ThePFD.Dsc_IsAddTT)              theIdent.Append(ThePFD.Dsc_SeparIfTT_Rn    + this.TT);
         if(ThePFD.Dsc_Prefix1Rn.NotEmpty()) theIdent.Append(ThePFD.Dsc_Separ1_Rn       + ThePFD.Dsc_Prefix1Rn);
         if(ThePFD.Dsc_Prefix2Rn.NotEmpty()) theIdent.Append(ThePFD.Dsc_Separ2_Rn       + ThePFD.Dsc_Prefix2Rn);
         if(ThePFD.Dsc_IsAddTtNum)           theIdent.Append(ThePFD.Dsc_SeparIfTtNum_Rn + (ZXC.CURR_prjkt_rec.IsNeprofit ? this.TtNumRbr.ToString() : this.TtNumFiskal));
         if(ThePFD.Dsc_IsAddKupDobCd)        theIdent.Append(ThePFD.Dsc_SeparIfKDcd_Rn  + (this.TtInfo.IsExtendableTT ? this.KupdobCD.ToString("000000") : ""));
         if(ThePFD.Dsc_IsAddYear)            theIdent.Append(ThePFD.Dsc_SeparIfYear_Rn  + ZXC.projectYear);

         return theIdent.ToString();
      } 
   }

   public string VvDocumIdentOhneTitle
   {
      get 
      { 
         System.Text.StringBuilder theIdent = new System.Text.StringBuilder("");

         if(ThePFD.Dsc_IsAddTT)              theIdent.Append(ThePFD.Dsc_SeparIfTT_Rn    + this.TT);
         if(ThePFD.Dsc_Prefix1Rn.NotEmpty()) theIdent.Append(ThePFD.Dsc_Separ1_Rn       + ThePFD.Dsc_Prefix1Rn);
         if(ThePFD.Dsc_Prefix2Rn.NotEmpty()) theIdent.Append(ThePFD.Dsc_Separ2_Rn       + ThePFD.Dsc_Prefix2Rn);
         if(ThePFD.Dsc_IsAddTtNum)           theIdent.Append(ThePFD.Dsc_SeparIfTtNum_Rn + this.TtNumFiskal);
         if(ThePFD.Dsc_IsAddKupDobCd)        theIdent.Append(ThePFD.Dsc_SeparIfKDcd_Rn  + (this.TtInfo.IsExtendableTT ? this.KupdobCD.ToString("000000") : ""));
         if(ThePFD.Dsc_IsAddYear)            theIdent.Append(ThePFD.Dsc_SeparIfYear_Rn  + ZXC.projectYear);

         return theIdent.ToString();
      } 
   }

   public string VvPnb
   {
      get
      {
         // 03.09.2019: print ce ranije ispitati hoce li uopce ovaj properti (pa ako nece, necemo ovdje uopce niti doci)                      
         //             a punjenje BT083 eRacun XML-a (buduci da nemre biti prazno) ce onda uzeti ovaj defaultni ["HR99 " + this.TtNumFiskal] 
         if(ThePFD.Dsc_OcuIspisPnb == false)
         {
          //return "HR99 " + this.TtNumFiskal; //28.08.2020 ovo 99 je kada ne bi bilo ttnuma
            return "HR00 " + this.TtNumFiskal;
         }

         System.Text.StringBuilder thePnb = new System.Text.StringBuilder("");

       //11.12.2020. radi bar koda ako je ThePFD.Dsc_OcuIspisPnb == false a ThePFD.Dsc_PnbM.IsZero()onda dolazi krivo
       //            dakle treba dolazi uvijek HRxx pa makar i nula jer kasnije se vadi van sve stop treba              
       //if(ThePFD.Dsc_PnbM.NotZero())          thePnb.Append("HR" + ThePFD.Dsc_PnbM.ToString("00") + " ");
         if(ThePFD.Dsc_PnbM.NotZero())          thePnb.Append("HR" + ThePFD.Dsc_PnbM.ToString("00") + " ");
         else                                   thePnb.Append("HR" + "00"                           + " ");

         if(ThePFD.Dsc_PrefixIR1Pb.NotZero())   thePnb.Append(ThePFD.Dsc_Separ1_Pn       + ThePFD.Dsc_PrefixIR1Pb.ToString());
         if(ThePFD.Dsc_PrefixIR2PbTx.NotEmpty())thePnb.Append(ThePFD.Dsc_Separ2_Pn       + ThePFD.Dsc_PrefixIR2PbTx          );
         if(ThePFD.Dsc_IsAddTtNum_Pb)           thePnb.Append(ThePFD.Dsc_SeparIfTtNum_Pn + this.TtNum.ToString(/*"000000"*/));
         if(ThePFD.Dsc_IsAddKupDobCd_Pb)        thePnb.Append(ThePFD.Dsc_SeparIfKDcd_Pn  + (this.TtInfo.IsExtendableTT ? this.KupdobCD.ToString("000000") : ""));
         if(ThePFD.Dsc_IsAddYear_Pb)            thePnb.Append(ThePFD.Dsc_SeparIfYear_Pn  + ZXC.projectYear);

         return thePnb.ToString();
      }
   }

   #endregion Faktur 2 DSC link

   // 16.01.2014: 
   internal bool SkladCD2_HasChanged { get { return (this.SkladCD2 != this.backupData._skladCD2); } }
   internal bool DokDate2_HasChanged { get { return (this.DokDate2 != this.backupData._dokDate2); } }

   public static bool IsProizvCijByArtGr(string theTT)
   {
      return (ZXC.RRD.Dsc_IsProizvCijByArtGr && theTT == Faktur.TT_TRI);
   }

   public static int GetMaxCountOfSignificantDecimalPlaces(Faktur faktur_rec)
   {
      if(faktur_rec.Transes == null || faktur_rec.Transes.Count.IsZero()) return -1;

      return faktur_rec.Transes.Max(rtr => rtr.T_cij.CountOfSignificantDecimalPlaces());
   }

   public Faktur Convert_IRM_Faktur_To_IRA_Faktur() // assuming; this is IRM 
   {
      Faktur IRAfaktur_rec = (Faktur)this.CreateNewRecordAndCloneItComplete();

      IRAfaktur_rec.TT = Faktur.TT_IRA;

      for(int i = 0; i < this.Transes.Count; ++i)
      {
         IRAfaktur_rec.Transes[i].T_TT  = IRAfaktur_rec.TT;
         IRAfaktur_rec.Transes[i].T_cij = ZXC.VvGet_100_from_125(this.Transes[i].T_cij, this.Transes[i].T_pdvSt);
         IRAfaktur_rec.Transes[i].CalcTransResults(IRAfaktur_rec);
      }

      IRAfaktur_rec.TakeTransesSumToDokumentSum(true);

      return IRAfaktur_rec;
   }

   #endregion Custom Metodz

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
         // 29.03.2016: 
       //if(this.ProjektCD.IsEmpty()                                                        ) return "";
       //if(this.ProjektCD.IsEmpty() && this.TT != Faktur.TT_RNM                            ) return "";
         if(this.ProjektCD.IsEmpty() && this.TT != Faktur.TT_RNM && this.TT != Faktur.TT_RNZ) return "";

         // 29.03.2016: start 

         if(this.ProjektCD.StartsWith(Faktur.TT_RNM)) // neki drugi (ne RNM) dokument vezan na RNM 
         {
            string prjCDTtNum = this.ProjektCD.SubstringSafe(4);
            string retStr = "RN " + prjCDTtNum.SubstringSafe(0, 2) + "-" + prjCDTtNum.SubstringSafe(2, 2) + "-" + prjCDTtNum.SubstringSafe(4);

            return retStr;
         }

         if(this.TT == Faktur.TT_RNM) // sam RNM dokument 
         {
            string prjCDTtNum = this.TtNum.ToString();
            string retStr = "RN " + prjCDTtNum.SubstringSafe(0, 2) + "-" + prjCDTtNum.SubstringSafe(2, 2) + "-" + prjCDTtNum.SubstringSafe(4);

            return retStr;
         }

         if(this.ProjektCD.StartsWith(Faktur.TT_RNZ)) // neki drugi (ne RNM) dokument vezan na RNM 
         {
            string prjCDTtNum = this.ProjektCD.SubstringSafe(4);
            string retStr = this.VezniDok + "-" + prjCDTtNum.SubstringSafe(5, 2) + "-" + prjCDTtNum.SubstringSafe(1, 2) + "/" + this.DokDate.ToString(ZXC.VvDateYYFormat);

            return retStr;
         }

         // 28.02.2020: observacija, WTF, pa ovdje nikad ni ne dolazimo zbogradi ovog gornjeg if()-a i njegovog returna!? 
         if(this.TT == Faktur.TT_RNZ) // sam RNM dokument 
         {
            string prjCDTtNum = this.TtNum.ToString    ();

            string retStr = this.VezniDok + "-" + prjCDTtNum.SubstringSafe(5, 2) + "-" + prjCDTtNum.SubstringSafe(1, 2) + "/" + this.DokDate.ToString(ZXC.VvDateYYFormat);

            return retStr;
         }

         // 29.03.2016: end 

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

   #endregion prjFaktur_rec

   #region FISKAL STUFF

   public static uint   BaseTtNum            = 100000;
   public static string TtNumFiskalSeparator = "-";
   public static string TtNumFiskalONU       = "1";

   private static bool IsOkRegardingPrjktFiskThisTTonly(string theTT)
   {
      if(ZXC.CURR_prjkt_rec.FiskTtOnly.IsEmpty()) return true;

      if(ZXC.CURR_prjkt_rec.FiskTtOnly == theTT) return true;

      return false;
   }

      // 21.10.2013: 
 //private static bool IsOkRegardingPrjktIsFiskCashTTonly(bool _isNpCash)
   private static bool IsOkRegardingPrjktIsFiskCashTTonly(string _nacPlac, string theTT)
   {
      if(ZXC.CURR_prjkt_rec.IsFiskCashOnly == false) return true;

    //if(ZXC.CURR_prjkt_rec.IsFiskCashOnly == true &&  _isNpCash                       == true ) return true;
    //if(ZXC.CURR_prjkt_rec.IsFiskCashOnly == true && IsNacPlacVirman(_nacPlac)        == false) return true;
      if(ZXC.CURR_prjkt_rec.IsFiskCashOnly == true && IsNacPlacVirman(_nacPlac, theTT) == false) return true;

      return false;
   }

   private static bool IsNacPlacVirman(string _nacPlac, string theTT)
   {
      // 30.12.2016: 
      if(ZXC.IsTEXTHOshop && theTT == Faktur.TT_IRM) return false;

      string nacPlac = _nacPlac.NullSafe().ToUpper();

      if(nacPlac.IsEmpty()            || 
         nacPlac.Contains("VIRMAN")   || 
         nacPlac.Contains("TRANSAKC") || 
         nacPlac.Contains("POUZE")) 
         return true;
      else
         return false;

      // POUZEĆE: HPT upla'uje virmanski u ime primatelja posiljke, t. kao da je covjek sam otisao na postu/banku i platio uplatnicom 
   }

   public static bool IsFiskalDutyTT(DateTime dokDate, string theTT)
   {
      if(ZXC.IsFikalEra && dokDate >= ZXC.FiskalEraDate && ZXC.TtInfo(theTT).IsIzlazniPdvTT && !ZXC.TtInfo(theTT).Is_WYRN_TT) // IRM, IRA, IFA, IOD, IPV 
         return true;
      else
         return false;
   }

   public static bool IsFiskalDutyTT(string theTT)
   {
      if(ZXC.IsFikalEra &&                                 ZXC.TtInfo(theTT).IsIzlazniPdvTT && !ZXC.TtInfo(theTT).Is_WYRN_TT) // IRM, IRA, IFA, IOD, IPV 
         return true;
      else
         return false;
   }

   public bool IsFiskalDutyFaktur
   {
      get
      {
         if(ZXC.IsFikalEra && this.DokDate >= ZXC.FiskalEraDate && this.TtInfo.IsIzlazniPdvTT && !this.TtInfo.Is_WYRN_TT) // IRM, IRA, IFA, IOD, IPV 
            return true;
         else
            return false;
      }
   }

   public static bool IsFiskalDutyTT_ONLINE(string theTT, /*bool isNpCash*/ string nacPlac)
   {
      return IsFiskalDutyTT(theTT) && ZXC.CURR_prjkt_rec.IsFiskalOnline && IsOkRegardingPrjktFiskThisTTonly(theTT) && IsOkRegardingPrjktIsFiskCashTTonly(/*isNpCash*/nacPlac, theTT);
   }

   public bool IsFiskalDutyFaktur_ONLINE 
   { 
      get 
      {
         return IsFiskalDutyFaktur && ZXC.CURR_prjkt_rec.IsFiskalOnline && IsOkRegardingPrjktFiskThisTTonly(this.TT) && IsOkRegardingPrjktIsFiskCashTTonly(this./*IsNpCash*/NacPlac, this.TT); 
      } 
   }

   public static bool IsFiskalHamperWanted(string theTT)
   {
      return IsFiskalDutyTT(theTT) && ZXC.CURR_prjkt_rec.IsFiskalOnline && IsOkRegardingPrjktFiskThisTTonly(theTT);
   }

   public bool IsAlreadyFiskalized
   {
      get
      {
         return (FiskJIR /*+ FiskZKI*/).NotEmpty();
      }
   }

   // TtNum = TtNumSkBr * BaseTtNum + TtNumRbr 

   public uint TtNumRbr
   {
      get
      {
         if(this.TtNum < BaseTtNum) return this.TtNum;

         return this.TtNum % BaseTtNum;
      }
   }

   public uint TtNumSkPp
   {
      get
      {
         if(this.TtNum < BaseTtNum) return 0;

         return (this.TtNum - this.TtNumRbr) / BaseTtNum;
      }
   }

   public static uint GetTtNumFromRbr(string skladCD, uint rbr)
   {
      uint skladBR = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(skladCD);

      return skladBR * /*100000*/ Faktur.BaseTtNum + rbr;
     
   }

   public string TtNumFiskal
   {
      get
      {
         if(ZXC.IsFikalEra && this.DokDate >= ZXC.FiskalEraDate && this.TtInfo.IsIzlazniPdvTT && !this.TtInfo.Is_WYRN_TT) // IRM, IRA, IFA, IOD, IPV 
            return this.TtNumRbr + TtNumFiskalSeparator + this.TtNumSkPp + TtNumFiskalSeparator + TtNumFiskalONU;
         else
            return this.TtNum.ToString(/*"000000"*/);
      }
   }

   public Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType FiskNacPlacEnum
   {
      get
      {
         // G – novčanice           
         // K – kartica             
         // C – ček                 
         // T – transakcijski račun 
         // O – ostalo              

         // U slučaju više načina plaćanja po jednom
         // računu, isto je potrebno prijaviti pod
         // 'Ostalo'.
         // Za sve načine plaćanja koji nisu prije
         // navedeni koristiti će se oznaka ‘Ostalo’.

         // Novo u 2024: 
         if(this.R_IsNpMix == true) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.O; // O – ostalo              

         if(this.IsNpCash == true) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.G; // G – gotovina 
            
         if(
            this.NacPlac.ToUpper().Contains("PAYPAL")   || // 12.01.2018: 
            this.NacPlac.ToUpper().Contains("MASTER")   ||
            this.NacPlac.ToUpper().Contains("DINERS")   ||
            this.NacPlac.ToUpper().Contains("VISA")     ||
            this.NacPlac.ToUpper().Contains("AMERICAN") ||
            this.NacPlac.ToUpper().Contains("CARD")     ||
            this.NacPlac.ToUpper().Contains("MAESTRO")

         ) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.K; // K – kartice 

         //switch(faktur_rec.NacPlac)
         //{
         //}

         return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.T; // default say T – transakcijski račun (virman) 
      }
   }

   public Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType FiskNacPlac_BKP_Enum
   {
      get
      {
         // 11.11.2015: 
         if(this.TheEx == null || /*this.TheEx.BackupData == null ||*/ this.TheEx.BackupData._nacPlac == null) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.O;

         // G – novčanice           
         // K – kartica             
         // C – ček                 
         // T – transakcijski račun 
         // O – ostalo              

         // U slučaju više načina plaćanja po jednom
         // računu, isto je potrebno prijaviti pod
         // 'Ostalo'.
         // Za sve načine plaćanja koji nisu prije
         // navedeni koristiti će se oznaka ‘Ostalo’.

         // Novo u 2024: 
         if(this.TheEx.BackupData._s_ukKCRP_NP2.NotZero()) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.O; // O – ostalo              

         if(this.TheEx.BackupData._isNpCash == true) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.G; // G – gotovina 
            
         if(
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("MASTER")   ||
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("DINERS")   ||
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("VISA")     ||
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("AMERICAN") ||
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("CARD")     ||
            this.TheEx.BackupData._nacPlac.ToUpper().Contains("MAESTRO")

         ) return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.K; // K – kartice 

         //switch(faktur_rec.NacPlac)
         //{
         //}

         return Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.T; // default say T – transakcijski račun (virman) 
      }
   }

   public string FiskNacPlacText
   {
      get
      {
         switch(FiskNacPlacEnum)
         {
            case Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.G: return "NOVČANICE"          ;
            case Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.K: return "KARTICA"            ;
            case Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.C: return "ČEK"                ;
            case Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.T: return "TRANSAKCIJSKI RAČUN";
            case Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.O: return "OSTALO"             ;
            default:                                                  return "TRANSAKCIJSKI RAČUN";
         }
      }
   }

   #endregion FISKAL STUFF

   #region All About SKY

   public bool IsSklSkyCentrala
   {
      get
      {
         return 
            
            ZXC.IsSkyEnvironment &&

            this.SkladCD.Length > 1 &&
            // this.SkladCD.Substring(0,2)      ---> 12BGS  => 12 
            // this.SkladCD.Substring(0,2)      ---> 14B5   => 14 
            // (ZXC.vvDB_ServerID_CENTRALA + 1) ---> 12 + 1 => 13 
            ZXC.ValOrZero_UInt(this.SkladCD.Substring(0,2)) <= (ZXC.vvDB_ServerID_CENTRALA + 1); 
      }
   }

   // PUSE ?! 
   //public static string GetSklad1CDfromSkyFrsSklKind(ZXC.SkySklKind frsSklKind)
   //{
   //   if(frsSklKind == ZXC.SkySklKind.NONE) return "";

   //   string vvDB_ServerID_skl1;
   //   string sklCdRoot;

   //   switch(frsSklKind)
   //   {
   //      case ZXC.SkySklKind.ShopVPSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID.ToString("00"); sklCdRoot = vvDB_ServerID_skl1 + "B"; break; // 98B5  
   //      case ZXC.SkySklKind.ShopMPSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID.ToString("00"); sklCdRoot = vvDB_ServerID_skl1 + "M"; break; // 98M5  
   //      case ZXC.SkySklKind.CentGLSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); sklCdRoot = vvDB_ServerID_skl1 + "BG"; break; // 12BGS 
   //      case ZXC.SkySklKind.CentPVSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); sklCdRoot = vvDB_ServerID_skl1 + "BP"; break; // 12BPS 

   //      default: sklCdRoot = ""; break;
   //   }

   //   VvLookUpItem lui = ZXC.luiListaSkladista./*Single*/FirstOrDefault(l => l.Cd.StartsWith(sklCdRoot));

   //   return lui == null ? "" : lui.Cd;
   //}

   public static string GetLikeSkladCDfromSkyRule_SklKind(ZXC.SkySklKind skl1Kind)
   {
      if(skl1Kind == ZXC.SkySklKind.NONE) return "";

      string vvDB_ServerID_skl1;
      string LIKEsklCD;

      switch(skl1Kind)
      {
         case ZXC.SkySklKind.ShopVPSK:                                                                 LIKEsklCD = "__B_" ;                    break; // npr 98B5  
         case ZXC.SkySklKind.ShopMPSK:                                                                 LIKEsklCD = "__M_" ;                    break; // npr 98M5  
         // 05.02.2015: 
       //case ZXC.SkySklKind.CentGLSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); LIKEsklCD = vvDB_ServerID_skl1 + "BGS"; break; // exact 12BGS 
       //case ZXC.SkySklKind.CentPVSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); LIKEsklCD = vvDB_ServerID_skl1 + "BPS"; break; // exact 12BPS 
         case ZXC.SkySklKind.CentGLSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); LIKEsklCD = vvDB_ServerID_skl1 + "BG_"; break; // like  12BGx 
         case ZXC.SkySklKind.CentPVSK: vvDB_ServerID_skl1 = ZXC.vvDB_ServerID_CENTRALA.ToString("00"); LIKEsklCD = vvDB_ServerID_skl1 + "BP_"; break; // like  12BPx 

         default: LIKEsklCD = ""; break;
      }  

      return LIKEsklCD;
   }

   public static string GetLikeSkladXSkyRule_ShopRCVkind(ZXC.SkyReceiveKind shopRCVkind)
   {
      if(ZXC.IsTEXTHOshop == false || shopRCVkind == ZXC.SkyReceiveKind.NONE) return "";

      string vvDB_ServerID = ZXC.vvDB_ServerID.ToString("00");
      string LIKEclause;

      switch(shopRCVkind)
      {
         case ZXC.SkyReceiveKind.OnlyLOCALskl1: LIKEclause = "AND LAN.skladCD      LIKE '" + vvDB_ServerID + "%'"; break; // npr '98%' 
         case ZXC.SkyReceiveKind.OnlyLOCALskl2: LIKEclause = "AND LAN.skladCD2     LIKE '" + vvDB_ServerID + "%'"; break; // npr '98%' 
         case ZXC.SkyReceiveKind.OnlyOTHERskl1: LIKEclause = "AND LAN.skladCD  NOT LIKE '" + vvDB_ServerID + "%'"; break; // npr '98%' 
         case ZXC.SkyReceiveKind.OnlyOTHERskl2: LIKEclause = "AND LAN.skladCD2 NOT LIKE '" + vvDB_ServerID + "%'"; break; // npr '98%' 

         default: LIKEclause = ""; break;
      }  

      return LIKEclause;
   }

   internal Faktur SplitThisFakturForNpMix()
   {
      Faktur np2Faktur_rec = (Faktur)this.CreateNewRecordAndCloneItComplete();

      this.S_ukKCRP = this.R_ukKCRP_NP2;

      np2Faktur_rec.NacPlac  = np2Faktur_rec.NacPlac2    ;
      np2Faktur_rec.S_ukKCRP = np2Faktur_rec.S_ukKCRP_NP1;

      this.NacPlac2     = "";
      this.S_ukKCRP_NP1 =  0;

      np2Faktur_rec.NacPlac2     = "";
      np2Faktur_rec.S_ukKCRP_NP1 =  0;

      return np2Faktur_rec;
   }

   public ZXC.SkySklKind Skl1kind
   {
      get
      {
         if(ZXC.IsSkyEnvironment == false) return ZXC.SkySklKind.NONE;

         if(this.SkladCD.IsEmpty() || this.SkladCD.Length < 4) return ZXC.SkySklKind.NONE;

         uint   first2chars = ZXC.ValOrZero_UInt(this.SkladCD.Substring(0, 2));
         string thirdChar   =                   (this.SkladCD.Substring(2, 1));
         string fourthChar  =                   (this.SkladCD.Substring(3, 1));

         if(first2chars == ZXC.vvDB_ServerID_CENTRALA)
         {
            if(fourthChar == "G") return ZXC.SkySklKind.CentGLSK; // 12BGS ili 12BG2 
            if(fourthChar == "P") return ZXC.SkySklKind.CentPVSK; // 12BPS ili 12BP2 
         }

         if(first2chars != ZXC.vvDB_ServerID_CENTRALA)
         {
            if(thirdChar == "B") return ZXC.SkySklKind.ShopVPSK; // 98B5  
            if(thirdChar == "M") return ZXC.SkySklKind.ShopMPSK; // 98M5  
         }

         return ZXC.SkySklKind.NONE;
      }
   }

   public ZXC.SkySklKind Skl2kind
   {
      get
      {
         if(this.SkladCD2.IsEmpty()) return ZXC.SkySklKind.NONE;

         // 29.03.2016: 
         if(ZXC.IsTEXTHOany2 == false) return ZXC.SkySklKind.NONE;

         uint   first2chars = ZXC.ValOrZero_UInt(this.SkladCD2.Substring(0, 2));
         string thirdChar   =                   (this.SkladCD2.Substring(2, 1));
         string fourthChar  =                   (this.SkladCD2.Substring(3, 1));

         if(first2chars == ZXC.vvDB_ServerID_CENTRALA)
         {
            if(fourthChar == "G") return ZXC.SkySklKind.CentGLSK; // 12BGS ili 12BG2 
            if(fourthChar == "P") return ZXC.SkySklKind.CentPVSK; // 12BPS ili 12BP2 
         }

         if(first2chars != ZXC.vvDB_ServerID_CENTRALA)
         {
            if(thirdChar == "B") return ZXC.SkySklKind.ShopVPSK; // 98B5  
            if(thirdChar == "M") return ZXC.SkySklKind.ShopMPSK; // 98M5  
         }

         return ZXC.SkySklKind.NONE;
      }
   }

   //public ZXC.LanSrvKind RuleFor
   //{
   //   get
   //   {
   //   }
   //}

   #endregion All About SKY

   //public System.Drawing.Image TtSort_And_TtNum_EAN8_Image
   //{
   //   get
   //   {
   //      string bcStr = this.TtSort_And_TtNum;
   //
   //      BarcodeLib.Barcode barcode = new BarcodeLib.Barcode(/*"30583306", BarcodeLib.TYPE.EAN8*/);
   //      barcode.IncludeLabel = true;
   //      barcode.AlternateLabel = bcStr;
   //      System.Drawing.Image barcodeImage = barcode.Encode(BarcodeLib.TYPE.EAN8/*13*/, bcStr, System.Drawing.Color.DarkBlue, System.Drawing.Color.LightBlue, 100, 100);
   //      return barcodeImage;
   //   }
   //}
   //
   //public byte[] TtSort_And_TtNum_EAN8
   //{
   //   get
   //   {
   //      using(System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
   //      {
   //         TtSort_And_TtNum_EAN8_Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
   //         return memoryStream.ToArray();
   //      }
   //   }
   //}

}

public class FaktEx : VvDataRecord, IVvExtenderDataRecord
{

   #region Fildz

   public const string recordName       = "faktEx";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   /*private*/ public FaktExStruct currentData;
   private FaktExStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.FaktExDao.TheSchemaTable;
   protected static FaktExDao.FaktExCI CI                = ZXC.FaktExDao.CI;

   #endregion Fildz

   #region Constructors

   public FaktEx() : this(0)
   {
   }

   public FaktEx(uint ID) : base()
   {
      this.currentData = new FaktExStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID       = ID;

      /* 01 */    this.currentData._fakturRecID = 0;
      /* 02 */    this.currentData._pdvNum      = 0;
      /* 03 */    this.currentData._pdvDate     = DateTime.MinValue;
      /* 04 */    this.currentData._kupdobCD    = 0;
      /* 05 */    this.currentData._kupdobTK    = "";
      /* 06 */    this.currentData._kupdobName  = "";
      /* 07 */    this.currentData._kdUlica     = "";
      /* 08 */    this.currentData._kdMjesto    = "";
      /* 09 */    this.currentData._kdZip       = "";
      /* 10 */    this.currentData._kdOib       = "";
      /* 11 */    this.currentData._posJedCD    = 0;
      /* 12 */    this.currentData._posJedTK    = "";
      /* 13 */    this.currentData._posJedName  = "";
      /* 14 */    this.currentData._posJedUlica = "";
      /* 15 */    this.currentData._posJedMjesto= "";
      /* 16 */    this.currentData._posJedZip   = "";
      /* 17 */    this.currentData._vezniDok2   = "";
      /* 18 */    this.currentData._fco         = "";
      /* 19 */    this.currentData._rokPlac     = 0;
      /* 20 */    this.currentData._dospDate    = DateTime.MinValue;
      /* 21 */    this.currentData._skladDate = DateTime.MinValue;
      /* 22 */    this.currentData._nacPlac     = "";
      /* 23 */    this.currentData._ziroRn      = "";
      /* 24 */    this.currentData._devName     = "";
      /* 25 */    this.currentData._pnbM        = "";
      /* 26 */    this.currentData._pnbV        = "";
      /* 27 */    this.currentData._personCD    = 0;
      /* 28 */    this.currentData._personName  = "";
      /* 29 */    this.currentData._napomena2   = "";
      /* 30 */    this.currentData._cjenikTT    = "";
      /* 31 */    this.currentData._statusCD    = "";
      /* 32 */    this.currentData._rokPonude   = 0;
      /* 33 */    this.currentData._ponudDate   = DateTime.MinValue;
      /* 34 */    this.currentData._mtrosCD     = 0;
      /* 35 */    this.currentData._mtrosTK     = "";
      /* 36 */    this.currentData._mtrosName   = "";
      /* 37 */    this.currentData._primPlatCD  = 0;
      /* 38 */    this.currentData._primPlatTK  = "";
      /* 39 */    this.currentData._primPlatName= "";
      /* 40 */    this.currentData._pdvKnjiga   = 0;
      /* 41 */    this.currentData._isNpCash    = false;
      /* 42 */    this.currentData._pdvR12      = 0;
      /* 43 */    this.currentData._pdvKolTip   = 0;
      /* 44 */    this.currentData._s_ukKCRP    = decimal.Zero;
      /* 45 */    this.currentData._s_ukKCRM    = decimal.Zero;
      /* 46 */    this.currentData._s_ukKCR     = decimal.Zero;
      /* 47 */    this.currentData._s_ukRbt1    = decimal.Zero;
      /* 48 */    this.currentData._s_ukRbt2    = decimal.Zero;
      /* 49 */    this.currentData._s_ukZavisni = decimal.Zero;
      /* 50 */    this.currentData._s_ukProlazne= decimal.Zero;
      /* 51 */    this.currentData._s_ukPdv23m  = decimal.Zero;
      /* 52 */    this.currentData._s_ukPdv23n  = decimal.Zero;
      /* 53 */    this.currentData._s_ukPdv22m  = decimal.Zero;
      /* 54 */    this.currentData._s_ukPdv22n  = decimal.Zero;
      /* 55 */    this.currentData._s_ukPdv10m  = decimal.Zero;
      /* 56 */    this.currentData._s_ukPdv10n  = decimal.Zero;
      /* 57 */    this.currentData._s_ukOsn23m  = decimal.Zero;
      /* 58 */    this.currentData._s_ukOsn23n  = decimal.Zero;
      /* 59 */    this.currentData._s_ukOsn22m  = decimal.Zero;
      /* 60 */    this.currentData._s_ukOsn22n  = decimal.Zero;
      /* 61 */    this.currentData._s_ukOsn10m  = decimal.Zero;
      /* 62 */    this.currentData._s_ukOsn10n  = decimal.Zero;
      /* 63 */    this.currentData._s_ukOsn0    = decimal.Zero;
      /* 64 */    this.currentData._s_ukOsnPr   = decimal.Zero;
      /* 65 */    this.currentData._opciAlabel  = "";
      /* 66 */    this.currentData._opciAvalue  = "";
      /* 67 */    this.currentData._opciBlabel  = "";
      /* 68 */    this.currentData._opciBvalue  = "";
      /* 69 */    this.currentData._odgvPersCD  = 0;
      /* 70 */    this.currentData._odgvPersName= "";
      /* 71 */    this.currentData._cjenTTnum   = 0;
      /* 72 */    this.currentData._v3_tt       = "";
      /* 73 */    this.currentData._v3_ttNum    = 0;
      /* 74 */    this.currentData._v4_tt       = "";
      /* 75 */    this.currentData._v4_ttNum    = 0;
      /* 76 */    this.currentData._s_ukMrz     = decimal.Zero;
      /* 77 */    this.currentData._s_ukPdv     = decimal.Zero;
      /* 78 */    this.currentData._tipOtpreme  = "";
      /* 79 */    this.currentData._rokIsporuke = 0;
      /* 80 */    this.currentData._rokIspDate  = DateTime.MinValue;
      /* 81 */    this.currentData._dostName    = "";
      /* 82 */    this.currentData._dostAddr    = "";
      
      /* 83 */    this.currentData._s_ukOsn07    = decimal.Zero;
      /* 84 */    this.currentData._s_ukOsn08    = decimal.Zero;
      /* 85 */    this.currentData._s_ukOsn09    = decimal.Zero;
      /* 86 */    this.currentData._s_ukOsn10    = decimal.Zero;
      /* 87 */    this.currentData._s_ukOsn11    = decimal.Zero;
      /* 88 */    this.currentData._s_ukOsnUr23  = decimal.Zero;
      /* 89 */    this.currentData._s_ukOsnUu10  = decimal.Zero;
      /* 90 */    this.currentData._s_ukOsnUu22  = decimal.Zero;
      /* 91 */    this.currentData._s_ukOsnUu23  = decimal.Zero;
      /* 92 */    this.currentData._s_ukPdvUr23  = decimal.Zero;
      /* 93 */    this.currentData._s_ukPdvUu10  = decimal.Zero;
      /* 94 */    this.currentData._s_ukPdvUu22  = decimal.Zero;
      /* 95 */    this.currentData._s_ukPdvUu23  = decimal.Zero;
      /* 96 */    this.currentData._carinaKind   = 0;
      /* 97 */    this.currentData._prjArtCD     = "";
      /* 98 */    this.currentData._prjArtName   = "";
      /* 99 */    this.currentData._externLink1  = "";
      /*100 */    this.currentData._externLink2  = "";
      /*101 */    this.currentData._someMoney    = decimal.Zero;
      /*102 */    this.currentData._somePercent  = decimal.Zero;
      /*103 */    this.currentData._s_ukMskPdv10 = decimal.Zero;
      /*104 */    this.currentData._s_ukMskPdv23 = decimal.Zero;
      /*105 */    this.currentData._s_ukMSK_00   = decimal.Zero;
      /*106 */    this.currentData._s_ukMSK_10   = decimal.Zero;
      /*107 */    this.currentData._s_ukMSK_23   = decimal.Zero;
      /*108 */    this.currentData._s_ukKCR_usl  = decimal.Zero;
      /*109 */    this.currentData._s_ukKCRP_usl = decimal.Zero;

      /*101 */    this.currentData._s_ukPdv25m   = decimal.Zero;
      /*101 */    this.currentData._s_ukPdv25n   = decimal.Zero;
      /*102 */    this.currentData._s_ukOsn25m   = decimal.Zero;
      /*103 */    this.currentData._s_ukOsn25n   = decimal.Zero;
      /*104 */    this.currentData._s_ukOsnUr25  = decimal.Zero;
      /*105 */    this.currentData._s_ukOsnUu25  = decimal.Zero;
      /*106 */    this.currentData._s_ukPdvUr25  = decimal.Zero;
      /*107 */    this.currentData._s_ukPdvUu25  = decimal.Zero;
      /*108 */    this.currentData._s_ukMskPdv25 = decimal.Zero;
      /*109 */    this.currentData._s_ukMSK_25   = decimal.Zero;

      /*120 */    this.currentData._fiskJIR     = "";
      /*121 */    this.currentData._fiskZKI     = "";
      /*122 */    this.currentData._fiskMsgID   = "";
      /*123 */    this.currentData._fiskOibOp   = "";
      /*124 */    this.currentData._fiskPrgBr   = "";

      /*125 */    this.currentData._s_ukPdv05m   = decimal.Zero;
      /*126 */    this.currentData._s_ukPdv05n   = decimal.Zero;
      /*127 */    this.currentData._s_ukOsn05m   = decimal.Zero;
      /*128 */    this.currentData._s_ukOsn05n   = decimal.Zero;
      /*129 */    this.currentData._s_ukMskPdv05 = decimal.Zero;
      /*130 */    this.currentData._s_ukMSK_05   = decimal.Zero;
      /*131 */    this.currentData._s_ukOsnUr05  = decimal.Zero;
      /*132 */    this.currentData._s_ukPdvUr05  = decimal.Zero;

      /*133 */    this.currentData._s_pixK        = decimal.Zero;
      /*134 */    this.currentData._s_puxK_P      = decimal.Zero;
      /*135 */    this.currentData._s_puxK_All    = decimal.Zero;
      /*136 */    this.currentData._s_pixKC       = decimal.Zero;
      /*137 */    this.currentData._s_puxKC_P     = decimal.Zero;
      /*138 */    this.currentData._s_puxKC_All   = decimal.Zero;
      /*139 */    this.currentData._s_ukPpmvOsn   = decimal.Zero;
      /*140 */    this.currentData._s_ukPpmvSt1i2 = decimal.Zero;
      /*141 */    this.currentData._dateX         = DateTime.MinValue;
      /*142 */    this.currentData._vatCntryCode  = "";

      /*143 */    this.currentData._pdvGEOkind     = 0;
      /*144 */    this.currentData._pdvZPkind      = 0;
      /*145 */    this.currentData._s_ukOsnR25m_EU = decimal.Zero;
      /*146 */    this.currentData._s_ukOsnR25n_EU = decimal.Zero;
      /*147 */    this.currentData._s_ukOsnU25m_EU = decimal.Zero;
      /*148 */    this.currentData._s_ukOsnU25n_EU = decimal.Zero;
      /*149 */    this.currentData._s_ukOsnR10m_EU = decimal.Zero;
      /*150 */    this.currentData._s_ukOsnR10n_EU = decimal.Zero;
      /*151 */    this.currentData._s_ukOsnU10m_EU = decimal.Zero;
      /*152 */    this.currentData._s_ukOsnU10n_EU = decimal.Zero;
      /*153 */    this.currentData._s_ukOsnR05m_EU = decimal.Zero;
      /*154 */    this.currentData._s_ukOsnR05n_EU = decimal.Zero;
      /*155 */    this.currentData._s_ukOsnU05m_EU = decimal.Zero;
      /*156 */    this.currentData._s_ukOsnU05n_EU = decimal.Zero;
      /*157 */    this.currentData._s_ukOsn25m_BS  = decimal.Zero;
      /*158 */    this.currentData._s_ukOsn25n_BS  = decimal.Zero;
      /*159 */    this.currentData._s_ukOsn10m_BS  = decimal.Zero;
      /*160 */    this.currentData._s_ukOsn10n_BS  = decimal.Zero;
      /*161 */    this.currentData._s_ukOsn25m_TP  = decimal.Zero;
      /*162 */    this.currentData._s_ukOsn25n_TP  = decimal.Zero;
      /*163 */    this.currentData._s_ukPdvR25m_EU = decimal.Zero;
      /*164 */    this.currentData._s_ukPdvR25n_EU = decimal.Zero;
      /*165 */    this.currentData._s_ukPdvU25m_EU = decimal.Zero;
      /*166 */    this.currentData._s_ukPdvU25n_EU = decimal.Zero;
      /*167 */    this.currentData._s_ukPdvR10m_EU = decimal.Zero;
      /*168 */    this.currentData._s_ukPdvR10n_EU = decimal.Zero;
      /*169 */    this.currentData._s_ukPdvU10m_EU = decimal.Zero;
      /*170 */    this.currentData._s_ukPdvU10n_EU = decimal.Zero;
      /*171 */    this.currentData._s_ukPdvR05m_EU = decimal.Zero;
      /*172 */    this.currentData._s_ukPdvR05n_EU = decimal.Zero;
      /*173 */    this.currentData._s_ukPdvU05m_EU = decimal.Zero;
      /*174 */    this.currentData._s_ukPdvU05n_EU = decimal.Zero;
      /*175 */    this.currentData._s_ukPdv25m_BS  = decimal.Zero;
      /*176 */    this.currentData._s_ukPdv25n_BS  = decimal.Zero;
      /*177 */    this.currentData._s_ukPdv10m_BS  = decimal.Zero;
      /*178 */    this.currentData._s_ukPdv10n_BS  = decimal.Zero;
      /*179 */    this.currentData._s_ukPdv25m_TP  = decimal.Zero;
      /*180 */    this.currentData._s_ukPdv25n_TP  = decimal.Zero;
      /*181 */    this.currentData._s_ukOsnZP_11   = decimal.Zero;
      /*182 */    this.currentData._s_ukOsnZP_12   = decimal.Zero;
      /*183 */    this.currentData._s_ukOsnZP_13   = decimal.Zero;
      /*184 */    this.currentData._s_ukOsn12      = decimal.Zero;
      /*185 */    this.currentData._s_ukOsn13      = decimal.Zero;
      /*186 */    this.currentData._s_ukOsn14      = decimal.Zero;
      /*187 */    this.currentData._s_ukOsn15      = decimal.Zero;
      /*188 */    this.currentData._s_ukOsn16      = decimal.Zero;
      /*189 */    this.currentData._s_ukOsnPNP     = decimal.Zero;
      /*190 */    this.currentData._s_ukIznPNP     = decimal.Zero;
      /*191 */    this.currentData._s_ukMskPNP     = decimal.Zero;
      /*192 */    this.currentData._skiz_ukKC      = decimal.Zero;
      /*193 */    this.currentData._skiz_ukKCR     = decimal.Zero;
      /*194 */    this.currentData._skiz_ukRbt1    = decimal.Zero;
      /*195 */    this.currentData._s_ukKCRP_NP2   = decimal.Zero;
      /*196 */    this.currentData._nacPlac2       = ""          ;
      /*197 */    this.currentData._isNpCash2      = false       ;
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
      //get { return Atrans.sorter_Person_DokDate_DokNum; }
      get
      {
         throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet.");
         /*return new VvSQL.RecordSorter();*/
      }
   }

   #endregion Sorters

   #region Overriders And Specifics

   public override bool IsExtender
   {
      get { return true; }
   }

   internal FaktExStruct CurrentData // cijela FaktExStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   // ovo dodano tek 21.01.2013: ?! 
   internal FaktExStruct BackupData // cijela FaktExStruct struct-ura 
   {
      get { return this.backupData; }
      set { this.backupData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.FaktExDao; }
   }

   public override string VirtualRecordName
   {
      get { return FaktEx.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return FaktEx.recordNameArhiva; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set {        this.RecID = value; }
   }


   public override DateTime VirtualAddTS { get { return DateTime.MinValue; } }
   public override DateTime VirtualModTS { get { return DateTime.MinValue;  } }
   public override string   VirtualAddUID{ get { return null; } }
   public override string   VirtualModUID{ get { return null; } }

   public override uint   VirtualLanSrvID{ get { return 0; } set {;} }
   public override uint   VirtualLanRecID{ get { return 0; } set {;} }

   public override bool IsSifrar
   {
      get { return false; }
   }

   public override bool IsAutoSifra
   {
      get { return false; }
   }

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

   public override bool IsArhivable
   {
      get { return false; }
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

   public override string VirtualIDstring { get { return ""; } }


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

   #endregion Common Data Layer Columns

   //===================================================================
//#if Njett

   #region Data Layer Columns

   /* 01 */ public uint FakturRecID
   {
      get { return this.currentData._fakturRecID; }
      set {        this.currentData._fakturRecID = value; }
   }
   /* 02 */ public uint PdvNum
   {
      get { return this.currentData._pdvNum; }
      set {        this.currentData._pdvNum = value; }
   }
   /* 03 */ public DateTime PdvDate
   {
      get { return this.currentData._pdvDate; }
      set {        this.currentData._pdvDate = value; }
   }
   /* 04 */ public uint KupdobCD
   {
      get { return this.currentData._kupdobCD; }
      set {        this.currentData._kupdobCD = value; }
   }
   /* 05 */ public string KupdobTK
   {
      get { return this.currentData._kupdobTK; }
      set {        this.currentData._kupdobTK = value; }
   }
   /* 06 */ public string KupdobName
   {
      get { return this.currentData._kupdobName; }
      set {        this.currentData._kupdobName = value; }
   }
   /* 07 */ public string KdUlica
   {
      get { return this.currentData._kdUlica; }
      set {        this.currentData._kdUlica = value; }
   }
   /* 08 */ public string KdMjesto
   {
      get { return this.currentData._kdMjesto; }
      set {        this.currentData._kdMjesto = value; }
   }
   /* 09 */ public string KdZip
   {
      get { return this.currentData._kdZip; }
      set {        this.currentData._kdZip = value; }
   }
   /* 10 */ public string KdOib
   {
      get { return this.currentData._kdOib; }
      set {        this.currentData._kdOib = value; }
   }
   public string KdAdresa
   {
      get { return Faktur.GetAdresa(this.currentData._kdUlica, this.currentData._kdZip, this.currentData._kdMjesto); }
   }
   /* 11 */ public uint PosJedCD
   {
      get { return this.currentData._posJedCD; }
      set {        this.currentData._posJedCD = value; }
   }
   /* 12 */ public string PosJedTK
   {
      get { return this.currentData._posJedTK; }
      set {        this.currentData._posJedTK = value; }
   }
   /* 13 */ public string PosJedName
   {
      get { return this.currentData._posJedName; }
      set {        this.currentData._posJedName = value; }
   }
   /* 14 */ public string PosJedUlica
   {
      get { return this.currentData._posJedUlica; }
      set {        this.currentData._posJedUlica = value; }
   }
   /* 15 */ public string PosJedMjesto
   {
      get { return this.currentData._posJedMjesto; }
      set {        this.currentData._posJedMjesto = value; }
   }
   /* 16 */ public string PosJedZip
   {
      get { return this.currentData._posJedZip; }
      set {        this.currentData._posJedZip = value; }
   }
   public string PosJedAdresa
   {
      get { return Faktur.GetAdresa(this.currentData._posJedUlica, this.currentData._posJedZip, this.currentData._posJedMjesto); }
   }
   /* 17 */ public string VezniDok2
   {
      get { return this.currentData._vezniDok2; }
      set {        this.currentData._vezniDok2 = value; }
   }
   /* 18 */ public string Fco
   {
      get { return this.currentData._fco; }
      set {        this.currentData._fco = value; }
   }
   /* 19 */ public int RokPlac
   {
      get { return this.currentData._rokPlac; }
      set {        this.currentData._rokPlac = value; }
   }
   /* 20 */ public DateTime DospDate
   {
      get { return this.currentData._dospDate; }
      set {        this.currentData._dospDate = value; }
   }
   /* 21 */ public DateTime SkladDate
   {
      get { return this.currentData._skladDate; }
      set {        this.currentData._skladDate = value; }
   }
   /* 22 */ public string NacPlac
   {
      get { return this.currentData._nacPlac; }
      set {        this.currentData._nacPlac = value; }
   }
   /* 23 */ public string ZiroRn
   {
      get { return this.currentData._ziroRn; }
      set {        this.currentData._ziroRn = value; }
   }
   /* 24 */ public string DevName
   {
      get { return this.currentData._devName; }
      set {        this.currentData._devName = value; }
   }
   /* 25 */ public string PnbM
   {
      get { return this.currentData._pnbM; }
      set {        this.currentData._pnbM = value; }
   }
   /* 26 */ public string PnbV
   {
      get { return this.currentData._pnbV; }
      set {        this.currentData._pnbV = value; }
   }
   /* 27 */ public uint PersonCD
   {
      get { return this.currentData._personCD; }
      set {        this.currentData._personCD = value; }
   }
   /* 28 */ public string PersonName
   {
      get { return this.currentData._personName; }
      set {        this.currentData._personName = value; }
   }
   /* 29 */ public string Napomena2
   {
      get { return this.currentData._napomena2; }
      set {        this.currentData._napomena2 = value; }
   }
   /* 30 */ public string CjenikTT
   {
      get { return this.currentData._cjenikTT; }
      set {        this.currentData._cjenikTT = value; }
   }
   /* 31 */ public string StatusCD
   {
      get { return this.currentData._statusCD; }
      set {        this.currentData._statusCD = value; }
   }
   /* 32 */ public int RokPonude
   {
      get { return this.currentData._rokPonude; }
      set {        this.currentData._rokPonude = value; }
   }
   /* 33 */ public DateTime PonudDate
   {
      get { return this.currentData._ponudDate; }
      set {        this.currentData._ponudDate = value; }
   }
   /* 34 */ public uint MtrosCD
   {
      get { return this.currentData._mtrosCD; }
      set {        this.currentData._mtrosCD = value; }
   }
   /* 35 */ public string MtrosTK
   {
      get { return this.currentData._mtrosTK; }
      set {        this.currentData._mtrosTK = value; }
   }
   /* 36 */ public string MtrosName
   {
      get { return this.currentData._mtrosName; }
      set {        this.currentData._mtrosName = value; }
   }
   /* 37 */ public uint PrimPlatCD
   {
      get { return this.currentData._primPlatCD; }
      set {        this.currentData._primPlatCD = value; }
   }
   /* 38 */ public string PrimPlatTK
   {
      get { return this.currentData._primPlatTK; }
      set {        this.currentData._primPlatTK = value; }
   }
   /* 39 */ public string PrimPlatName
   {
      get { return this.currentData._primPlatName; }
      set {        this.currentData._primPlatName = value; }
   }
   /* 40 */ public ZXC.PdvKnjigaEnum PdvKnjiga
   {
      get { return (ZXC.PdvKnjigaEnum)this.currentData._pdvKnjiga; }
      set {                           this.currentData._pdvKnjiga = (ushort)value; }
   }
   /*    */ public ushort PdvKnjiga_u
   {
      get { return                    this.currentData._pdvKnjiga; }
      set {                           this.currentData._pdvKnjiga =         value; }
   }
   /* 41 */ public bool IsNpCash
   {
      get { return                  this.currentData._isNpCash; }
      set {                         this.currentData._isNpCash =         value; }
   }
   /* 42 */ public ZXC.PdvR12Enum PdvR12
   {
      get { return (ZXC.PdvR12Enum)this.currentData._pdvR12; }
      set {                        this.currentData._pdvR12 = (ushort)value; }
   }
   /*    */ public ushort PdvR12_u
   {
      get { return                 this.currentData._pdvR12; }
      set {                        this.currentData._pdvR12 =         value; }
   }

   /* 43 */ public ZXC.PdvKolTipEnum PdvKolTip
   {
      get { return (ZXC.PdvKolTipEnum)this.currentData._pdvKolTip; }
      set {                           this.currentData._pdvKolTip = (ushort)value; }
   }
   /*   */ public ushort PdvKolTip_u
   {
      get { return                    this.currentData._pdvKolTip; }
      set {                           this.currentData._pdvKolTip =         value; }
   }

   /* 44 */ public decimal S_ukKCRP
   {
      get { return this.currentData._s_ukKCRP; }
      set {        this.currentData._s_ukKCRP = value; }
   }
   /* 45 */ public decimal S_ukKCRM
   {
      get { return this.currentData._s_ukKCRM; }
      set {        this.currentData._s_ukKCRM = value; }
   }
   /* 46 */ public decimal S_ukKCR
   {
      get { return this.currentData._s_ukKCR; }
      set {        this.currentData._s_ukKCR = value; }
   }
   /* 47 */ public decimal S_ukRbt1
   {
      get { return this.currentData._s_ukRbt1; }
      set {        this.currentData._s_ukRbt1 = value; }
   }
   /* 48 */ public decimal S_ukRbt2
   {
      get { return this.currentData._s_ukRbt2; }
      set {        this.currentData._s_ukRbt2 = value; }
   }
   /* 49 */ public decimal S_ukZavisni
   {
      get { return this.currentData._s_ukZavisni; }
      set {        this.currentData._s_ukZavisni = value; }
   }
   /* 50 */ public decimal S_ukProlazne
   {
      get { return this.currentData._s_ukProlazne; }
      set {        this.currentData._s_ukProlazne = value; }
   }
   /* 51 */ public decimal S_ukPdv23m
   {
      get { return this.currentData._s_ukPdv23m; }
      set {        this.currentData._s_ukPdv23m = value; }
   }
   /* 52 */ public decimal S_ukPdv23n
   {
      get { return this.currentData._s_ukPdv23n; }
      set {        this.currentData._s_ukPdv23n = value; }
   }
   /* 53 */ public decimal S_ukPdv22m
   {
      get { return this.currentData._s_ukPdv22m; }
      set {        this.currentData._s_ukPdv22m = value; }
   }
   /* 54 */ public decimal S_ukPdv22n
   {
      get { return this.currentData._s_ukPdv22n; }
      set {        this.currentData._s_ukPdv22n = value; }
   }
   /* 55 */ public decimal S_ukPdv10m
   {
      get { return this.currentData._s_ukPdv10m; }
      set {        this.currentData._s_ukPdv10m = value; }
   }
   /* 56 */ public decimal S_ukPdv10n
   {
      get { return this.currentData._s_ukPdv10n; }
      set {        this.currentData._s_ukPdv10n = value; }
   }
   /* 57 */ public decimal S_ukOsn23m
   {
      get { return this.currentData._s_ukOsn23m; }
      set {        this.currentData._s_ukOsn23m = value; }
   }
   /* 58 */ public decimal S_ukOsn23n
   {
      get { return this.currentData._s_ukOsn23n; }
      set {        this.currentData._s_ukOsn23n = value; }
   }
   /* 59 */ public decimal S_ukOsn22m
   {
      get { return this.currentData._s_ukOsn22m; }
      set {        this.currentData._s_ukOsn22m = value; }
   }
   /* 60 */ public decimal S_ukOsn22n
   {
      get { return this.currentData._s_ukOsn22n; }
      set {        this.currentData._s_ukOsn22n = value; }
   }
   /* 61 */ public decimal S_ukOsn10m
   {
      get { return this.currentData._s_ukOsn10m; }
      set {        this.currentData._s_ukOsn10m = value; }
   }
   /* 62 */ public decimal S_ukOsn10n
   {
      get { return this.currentData._s_ukOsn10n; }
      set {        this.currentData._s_ukOsn10n = value; }
   }
   /* 63 */ public decimal S_ukOsn0
   {
      get { return this.currentData._s_ukOsn0; }
      set {        this.currentData._s_ukOsn0 = value; }
   }
   /* 64 */ public decimal S_ukOsnPr
   {
      get { return this.currentData._s_ukOsnPr; }
      set {        this.currentData._s_ukOsnPr = value; }
   }
   /* 65 */ public string OpciAlabel
   {
      get { return this.currentData._opciAlabel; }
      set {        this.currentData._opciAlabel = value; }
   }
   /* 66 */ public string OpciAvalue
   {
      get { return this.currentData._opciAvalue; }
      set {        this.currentData._opciAvalue = value; }
   }
   /* 67 */ public string OpciBlabel
   {
      get { return this.currentData._opciBlabel; }
      set {        this.currentData._opciBlabel = value; }
   }
   /* 68 */ public string OpciBvalue
   {
      get { return this.currentData._opciBvalue; }
      set {        this.currentData._opciBvalue = value; }
   }

   /* 69 */ public uint OdgvPersCD  
   {
      get { return this.currentData._odgvPersCD; }
      set {        this.currentData._odgvPersCD = value; }
   }
   /* 70 */ public string   OdgvPersName
   {
      get { return this.currentData._odgvPersName; }
      set {        this.currentData._odgvPersName = value; }
   }
   /* 71 */ public uint     CjenTTnum
   {
      get { return this.currentData._cjenTTnum; }
      set {        this.currentData._cjenTTnum = value; }
   }
   /* 72 */ public string V3_tt       
   {
      get { return this.currentData._v3_tt; }
      set {        this.currentData._v3_tt = value; }
   }
   /* 73 */ public uint   V3_ttNum
   {
      get { return this.currentData._v3_ttNum; }
      set {        this.currentData._v3_ttNum = value; }
   }
   /* 74 */ public string V4_tt
   {
      get { return this.currentData._v4_tt; }
      set {        this.currentData._v4_tt = value; }
   }
   /* 75 */ public uint   V4_ttNum
   {
      get { return this.currentData._v4_ttNum; }
      set {        this.currentData._v4_ttNum = value; }
   }
   /* 76 */ public decimal S_ukMrz
   {
      get { return this.currentData._s_ukMrz; }
      set {        this.currentData._s_ukMrz = value; }
   }
   /* 77 */ public decimal S_ukPdv
   {
      get { return this.currentData._s_ukPdv; }
      set {        this.currentData._s_ukPdv = value; }
   }
   /* 78 */ public string TipOtpreme
   {
      get { return this.currentData._tipOtpreme; }
      set {        this.currentData._tipOtpreme = value; }
   }
   /* 79 */ public int RokIsporuke
   {
      get { return this.currentData._rokIsporuke; }
      set {        this.currentData._rokIsporuke = value; }
   }
   /* 80 */ public DateTime RokIspDate
   {
      get { return this.currentData._rokIspDate; }
      set {        this.currentData._rokIspDate = value; }
   }

   /* 81 */ public string DostName
   {
      get { return this.currentData._dostName; }
      set {        this.currentData._dostName = value; }
   }

   /* 82 */ public string DostAddr
   {
      get { return this.currentData._dostAddr; }
      set {        this.currentData._dostAddr = value; }
   }

   public decimal R_ukRbt1i2 { get { return this.S_ukRbt1 + this.S_ukRbt2; } }

   public string S_ukKCRP_AsText
   {
      get { return ZXC.KuneIlipe(this.currentData._s_ukKCRP); }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 83 */ public decimal S_ukOsn07
   {
      get { return this.currentData._s_ukOsn07; }
      set {        this.currentData._s_ukOsn07 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 84 */ public decimal S_ukOsn08
   {
      get { return this.currentData._s_ukOsn08; }
      set {        this.currentData._s_ukOsn08 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 85 */ public decimal S_ukOsn09
   {
      get { return this.currentData._s_ukOsn09; }
      set {        this.currentData._s_ukOsn09 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 86 */ public decimal S_ukOsn10
   {
      get { return this.currentData._s_ukOsn10; }
      set {        this.currentData._s_ukOsn10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 87 */ public decimal S_ukOsn11
   {
      get { return this.currentData._s_ukOsn11; }
      set {        this.currentData._s_ukOsn11 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 88 */ public decimal S_ukOsnUr23
   {
      get { return this.currentData._s_ukOsnUr23; }
      set {        this.currentData._s_ukOsnUr23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 89 */ public decimal S_ukOsnUu10
   {
      get { return this.currentData._s_ukOsnUu10; }
      set {        this.currentData._s_ukOsnUu10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 90 */ public decimal S_ukOsnUu22
   {
      get { return this.currentData._s_ukOsnUu22; }
      set {        this.currentData._s_ukOsnUu22 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 91 */ public decimal S_ukOsnUu23
   {
      get { return this.currentData._s_ukOsnUu23; }
      set {        this.currentData._s_ukOsnUu23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 92 */ public decimal S_ukPdvUr23
   {
      get { return this.currentData._s_ukPdvUr23; }
      set {        this.currentData._s_ukPdvUr23 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 93 */ public decimal S_ukPdvUu10
   {
      get { return this.currentData._s_ukPdvUu10; }
      set {        this.currentData._s_ukPdvUu10 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 94 */ public decimal S_ukPdvUu22
   {
      get { return this.currentData._s_ukPdvUu22; }
      set {        this.currentData._s_ukPdvUu22 = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)]
   /* 95 */ public decimal S_ukPdvUu23
   {
      get { return this.currentData._s_ukPdvUu23; }
      set {        this.currentData._s_ukPdvUu23 = value; }
   }

   /* 96 */ public ushort CarinaKind_u
   {
      get { return this.currentData._carinaKind; }
      set {        this.currentData._carinaKind = value; }
   }

   public ZXC.ShouldFak2NalEnum CarinaKind
   {
      get { return (ZXC.ShouldFak2NalEnum)this.currentData._carinaKind; }
      set {                               this.currentData._carinaKind = (ushort)value; }
   }

   /* 97 */ public string PrjArtCD
   {
      get { return this.currentData._prjArtCD; }
      set {        this.currentData._prjArtCD = value; }
   }
   /* 98 */ public string PrjArtName
   {
      get { return this.currentData._prjArtName; }
      set {        this.currentData._prjArtName = value; }
   }

   /* 99 */ public string ExternLink1
   {
      get { return this.currentData._externLink1; }
      set {        this.currentData._externLink1 = value; }
   }
   /*100 */ public string ExternLink2
   {
      get { return this.currentData._externLink2; }
      set {        this.currentData._externLink2 = value; }
   }

   /*101 */ public decimal SomeMoney
   {
      get { return this.currentData._someMoney; }
      set {        this.currentData._someMoney = value; }
   }

   /*102 */ public decimal SomePercent
   {
      get { return this.currentData._somePercent; }
      set {        this.currentData._somePercent = value; }
   }

   /* 103 */ public decimal S_ukMskPdv10
   {
      get { return this.currentData._s_ukMskPdv10; }
      set {        this.currentData._s_ukMskPdv10 = value; }
   }
   /* 104 */ public decimal S_ukMskPdv23
   {
      get { return this.currentData._s_ukMskPdv23; }
      set {        this.currentData._s_ukMskPdv23 = value; }
   }
   /* 105 */ public decimal S_ukMSK_00
   {
      get { return this.currentData._s_ukMSK_00; }
      set {        this.currentData._s_ukMSK_00 = value; }
   }
   /* 106 */ public decimal S_ukMSK_10
   {
      get { return this.currentData._s_ukMSK_10; }
      set {        this.currentData._s_ukMSK_10 = value; }
   }
   /* 107 */ public decimal S_ukMSK_23
   {
      get { return this.currentData._s_ukMSK_23; }
      set {        this.currentData._s_ukMSK_23 = value; }
   }
   /* 108 */ public decimal S_ukKCR_usl
   {
      get { return this.currentData._s_ukKCR_usl; }
      set {        this.currentData._s_ukKCR_usl = value; }
   }
   /* 109 */ public decimal S_ukKCRP_usl
   {
      get { return this.currentData._s_ukKCRP_usl; }
      set {        this.currentData._s_ukKCRP_usl = value; }
   }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 110 */ public decimal S_ukPdv25m   { get { return this.currentData._s_ukPdv25m  ; } set { this.currentData._s_ukPdv25m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 111 */ public decimal S_ukPdv25n   { get { return this.currentData._s_ukPdv25n  ; } set { this.currentData._s_ukPdv25n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 112 */ public decimal S_ukOsn25m   { get { return this.currentData._s_ukOsn25m  ; } set { this.currentData._s_ukOsn25m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 113 */ public decimal S_ukOsn25n   { get { return this.currentData._s_ukOsn25n  ; } set { this.currentData._s_ukOsn25n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 114 */ public decimal S_ukOsnUr25  { get { return this.currentData._s_ukOsnUr25 ; } set { this.currentData._s_ukOsnUr25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 115 */ public decimal S_ukOsnUu25  { get { return this.currentData._s_ukOsnUu25 ; } set { this.currentData._s_ukOsnUu25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 116 */ public decimal S_ukPdvUr25  { get { return this.currentData._s_ukPdvUr25 ; } set { this.currentData._s_ukPdvUr25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 117 */ public decimal S_ukPdvUu25  { get { return this.currentData._s_ukPdvUu25 ; } set { this.currentData._s_ukPdvUu25  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 118 */ public decimal S_ukMskPdv25 { get { return this.currentData._s_ukMskPdv25; } set { this.currentData._s_ukMskPdv25 = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 119 */ public decimal S_ukMSK_25   { get { return this.currentData._s_ukMSK_25  ; } set { this.currentData._s_ukMSK_25   = value; }}

   /*120 */ public string FiskJIR   { get { return this.currentData._fiskJIR  ; } set { this.currentData._fiskJIR   = value; } }
   /*121 */ public string FiskZKI   { get { return this.currentData._fiskZKI  ; } set { this.currentData._fiskZKI   = value; } }
   /*122 */ public string FiskMsgID { get { return this.currentData._fiskMsgID; } set { this.currentData._fiskMsgID = value; } }
   /*123 */ public string FiskOibOp { get { return this.currentData._fiskOibOp; } set { this.currentData._fiskOibOp = value; } }
   /*124 */ public string FiskPrgBr { get { return this.currentData._fiskPrgBr; } set { this.currentData._fiskPrgBr = value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 125 */ public decimal S_ukPdv05m   { get { return this.currentData._s_ukPdv05m  ; } set { this.currentData._s_ukPdv05m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 126 */ public decimal S_ukPdv05n   { get { return this.currentData._s_ukPdv05n  ; } set { this.currentData._s_ukPdv05n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 127 */ public decimal S_ukOsn05m   { get { return this.currentData._s_ukOsn05m  ; } set { this.currentData._s_ukOsn05m   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 128 */ public decimal S_ukOsn05n   { get { return this.currentData._s_ukOsn05n  ; } set { this.currentData._s_ukOsn05n   = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 129 */ public decimal S_ukMskPdv05 { get { return this.currentData._s_ukMskPdv05; } set { this.currentData._s_ukMskPdv05 = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 130 */ public decimal S_ukMSK_05   { get { return this.currentData._s_ukMSK_05  ; } set { this.currentData._s_ukMSK_05   = value; }}
   
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 131 */ public decimal S_ukOsnUr05  { get { return this.currentData._s_ukOsnUr05 ; } set { this.currentData._s_ukOsnUr05  = value; }}
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] 
   /* 132 */ public decimal S_ukPdvUr05  { get { return this.currentData._s_ukPdvUr05 ; } set { this.currentData._s_ukPdvUr05  = value; }}

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 133 */ public decimal S_pixK      { get { return this.currentData._s_pixK     ; } set { this.currentData._s_pixK      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 134 */ public decimal S_puxK_P    { get { return this.currentData._s_puxK_P   ; } set { this.currentData._s_puxK_P    = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 135 */ public decimal S_puxK_All  { get { return this.currentData._s_puxK_All ; } set { this.currentData._s_puxK_All  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 136 */ public decimal S_pixKC     { get { return this.currentData._s_pixKC    ; } set { this.currentData._s_pixKC     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 137 */ public decimal S_puxKC_P   { get { return this.currentData._s_puxKC_P  ; } set { this.currentData._s_puxKC_P   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 138 */ public decimal S_puxKC_All { get { return this.currentData._s_puxKC_All; } set { this.currentData._s_puxKC_All = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 139 */ public decimal S_ukPpmvOsn   { get { return this.currentData._s_ukPpmvOsn  ; } set { this.currentData._s_ukPpmvOsn   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /* 140 */ public decimal S_ukPpmvSt1i2 { get { return this.currentData._s_ukPpmvSt1i2; } set { this.currentData._s_ukPpmvSt1i2 = value; } }

   /* 141 */ public DateTime DateX
   {
      get { return this.currentData._dateX; }
      set {        this.currentData._dateX = value; }
   }

   /* 142 */ public string VatCntryCode
   {
      get { return this.currentData._vatCntryCode; }
      set {        this.currentData._vatCntryCode = value; }
   }

   /*143 */ public ZXC.PdvGEOkindEnum PdvGEOkind { get { return (ZXC.PdvGEOkindEnum)this.currentData._pdvGEOkind; } set { this.currentData._pdvGEOkind = (ushort)value; } }
   /*144 */ public ZXC.PdvZPkindEnum  PdvZPkind  { get { return (ZXC.PdvZPkindEnum) this.currentData._pdvZPkind ; } set { this.currentData._pdvZPkind  = (ushort)value; } }

   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*145 */ public decimal S_ukOsnR25m_EU    { get { return this.currentData._s_ukOsnR25m_EU; } set { this.currentData._s_ukOsnR25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*146 */ public decimal S_ukOsnR25n_EU    { get { return this.currentData._s_ukOsnR25n_EU; } set { this.currentData._s_ukOsnR25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*147 */ public decimal S_ukOsnU25m_EU    { get { return this.currentData._s_ukOsnU25m_EU; } set { this.currentData._s_ukOsnU25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*148 */ public decimal S_ukOsnU25n_EU    { get { return this.currentData._s_ukOsnU25n_EU; } set { this.currentData._s_ukOsnU25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*149 */ public decimal S_ukOsnR10m_EU    { get { return this.currentData._s_ukOsnR10m_EU; } set { this.currentData._s_ukOsnR10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*150 */ public decimal S_ukOsnR10n_EU    { get { return this.currentData._s_ukOsnR10n_EU; } set { this.currentData._s_ukOsnR10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*151 */ public decimal S_ukOsnU10m_EU    { get { return this.currentData._s_ukOsnU10m_EU; } set { this.currentData._s_ukOsnU10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*152 */ public decimal S_ukOsnU10n_EU    { get { return this.currentData._s_ukOsnU10n_EU; } set { this.currentData._s_ukOsnU10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*153 */ public decimal S_ukOsnR05m_EU    { get { return this.currentData._s_ukOsnR05m_EU; } set { this.currentData._s_ukOsnR05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*154 */ public decimal S_ukOsnR05n_EU    { get { return this.currentData._s_ukOsnR05n_EU; } set { this.currentData._s_ukOsnR05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*155 */ public decimal S_ukOsnU05m_EU    { get { return this.currentData._s_ukOsnU05m_EU; } set { this.currentData._s_ukOsnU05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*156 */ public decimal S_ukOsnU05n_EU    { get { return this.currentData._s_ukOsnU05n_EU; } set { this.currentData._s_ukOsnU05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*157 */ public decimal S_ukOsn25m_BS     { get { return this.currentData._s_ukOsn25m_BS ; } set { this.currentData._s_ukOsn25m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*158 */ public decimal S_ukOsn25n_BS     { get { return this.currentData._s_ukOsn25n_BS ; } set { this.currentData._s_ukOsn25n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*159 */ public decimal S_ukOsn10m_BS     { get { return this.currentData._s_ukOsn10m_BS ; } set { this.currentData._s_ukOsn10m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*160 */ public decimal S_ukOsn10n_BS     { get { return this.currentData._s_ukOsn10n_BS ; } set { this.currentData._s_ukOsn10n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*161 */ public decimal S_ukOsn25m_TP     { get { return this.currentData._s_ukOsn25m_TP ; } set { this.currentData._s_ukOsn25m_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*162 */ public decimal S_ukOsn25n_TP     { get { return this.currentData._s_ukOsn25n_TP ; } set { this.currentData._s_ukOsn25n_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*163 */ public decimal S_ukPdvR25m_EU    { get { return this.currentData._s_ukPdvR25m_EU; } set { this.currentData._s_ukPdvR25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*164 */ public decimal S_ukPdvR25n_EU    { get { return this.currentData._s_ukPdvR25n_EU; } set { this.currentData._s_ukPdvR25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*165 */ public decimal S_ukPdvU25m_EU    { get { return this.currentData._s_ukPdvU25m_EU; } set { this.currentData._s_ukPdvU25m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*166 */ public decimal S_ukPdvU25n_EU    { get { return this.currentData._s_ukPdvU25n_EU; } set { this.currentData._s_ukPdvU25n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*167 */ public decimal S_ukPdvR10m_EU    { get { return this.currentData._s_ukPdvR10m_EU; } set { this.currentData._s_ukPdvR10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*168 */ public decimal S_ukPdvR10n_EU    { get { return this.currentData._s_ukPdvR10n_EU; } set { this.currentData._s_ukPdvR10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*169 */ public decimal S_ukPdvU10m_EU    { get { return this.currentData._s_ukPdvU10m_EU; } set { this.currentData._s_ukPdvU10m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*170 */ public decimal S_ukPdvU10n_EU    { get { return this.currentData._s_ukPdvU10n_EU; } set { this.currentData._s_ukPdvU10n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*171 */ public decimal S_ukPdvR05m_EU    { get { return this.currentData._s_ukPdvR05m_EU; } set { this.currentData._s_ukPdvR05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*172 */ public decimal S_ukPdvR05n_EU    { get { return this.currentData._s_ukPdvR05n_EU; } set { this.currentData._s_ukPdvR05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*173 */ public decimal S_ukPdvU05m_EU    { get { return this.currentData._s_ukPdvU05m_EU; } set { this.currentData._s_ukPdvU05m_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*174 */ public decimal S_ukPdvU05n_EU    { get { return this.currentData._s_ukPdvU05n_EU; } set { this.currentData._s_ukPdvU05n_EU = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*175 */ public decimal S_ukPdv25m_BS     { get { return this.currentData._s_ukPdv25m_BS ; } set { this.currentData._s_ukPdv25m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*176 */ public decimal S_ukPdv25n_BS     { get { return this.currentData._s_ukPdv25n_BS ; } set { this.currentData._s_ukPdv25n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*177 */ public decimal S_ukPdv10m_BS     { get { return this.currentData._s_ukPdv10m_BS ; } set { this.currentData._s_ukPdv10m_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*178 */ public decimal S_ukPdv10n_BS     { get { return this.currentData._s_ukPdv10n_BS ; } set { this.currentData._s_ukPdv10n_BS  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*179 */ public decimal S_ukPdv25m_TP     { get { return this.currentData._s_ukPdv25m_TP ; } set { this.currentData._s_ukPdv25m_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*180 */ public decimal S_ukPdv25n_TP     { get { return this.currentData._s_ukPdv25n_TP ; } set { this.currentData._s_ukPdv25n_TP  = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*181 */ public decimal S_ukOsnZP_11      { get { return this.currentData._s_ukOsnZP_11  ; } set { this.currentData._s_ukOsnZP_11   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*182 */ public decimal S_ukOsnZP_12      { get { return this.currentData._s_ukOsnZP_12  ; } set { this.currentData._s_ukOsnZP_12   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*183 */ public decimal S_ukOsnZP_13      { get { return this.currentData._s_ukOsnZP_13  ; } set { this.currentData._s_ukOsnZP_13   = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*184 */ public decimal S_ukOsn12         { get { return this.currentData._s_ukOsn12     ; } set { this.currentData._s_ukOsn12      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*185 */ public decimal S_ukOsn13         { get { return this.currentData._s_ukOsn13     ; } set { this.currentData._s_ukOsn13      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*186 */ public decimal S_ukOsn14         { get { return this.currentData._s_ukOsn14     ; } set { this.currentData._s_ukOsn14      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*187 */ public decimal S_ukOsn15         { get { return this.currentData._s_ukOsn15     ; } set { this.currentData._s_ukOsn15      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*188 */ public decimal S_ukOsn16         { get { return this.currentData._s_ukOsn16     ; } set { this.currentData._s_ukOsn16      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*189 */ public decimal S_ukOsnPNP        { get { return this.currentData._s_ukOsnPNP    ; } set { this.currentData._s_ukOsnPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*190 */ public decimal S_ukIznPNP        { get { return this.currentData._s_ukIznPNP    ; } set { this.currentData._s_ukIznPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*191 */ public decimal S_ukMskPNP        { get { return this.currentData._s_ukMskPNP    ; } set { this.currentData._s_ukMskPNP     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*192 */ public decimal Skiz_ukKC         { get { return this.currentData._skiz_ukKC     ; } set { this.currentData._skiz_ukKC      = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*193 */ public decimal Skiz_ukKCR        { get { return this.currentData._skiz_ukKCR    ; } set { this.currentData._skiz_ukKCR     = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*194 */ public decimal Skiz_ukRbt1       { get { return this.currentData._skiz_ukRbt1   ; } set { this.currentData._skiz_ukRbt1    = value; } }
   [VvIsDevizaConvertibile(ZXC.JeliJeTakav.JE_TAKAV)] /*195 */ public decimal S_ukKCRP_NP2      { get { return this.currentData._s_ukKCRP_NP2  ; } set { this.currentData._s_ukKCRP_NP2   = value; } }
                                                      /*196 */ public string NacPlac2           { get { return this.currentData._nacPlac2      ; } set { this.currentData._nacPlac2       = value; } }

   /* 197 */ public bool IsNpCash2
   {
      get { return                  this.currentData._isNpCash2; }
      set {                         this.currentData._isNpCash2 =         value; }
   }

   #endregion Data Layer Columns

   public decimal R2_uplata  { get; set; }

   public decimal K_NivVrj00 { get; set; }
   public decimal K_NivVrj10 { get; set; }
   public decimal K_NivVrj23 { get; set; }
   public decimal K_NivVrj25 { get; set; }
   public decimal K_NivVrj05 { get; set; }

   public decimal Ira_ROB_NV { get; set; }

   public bool IsFillingFrom_Fak2Nal_URM { get; set; }
   public bool IsFillingFrom_Fak2Nal_IRM { get; set; }

   //#endif
   /* ============================================================== */
   /* ============================================================== */
   /* ============================================================== */

   #region Kune Backup Values

   public decimal Skn_ukRbt1 { get; set; }
   public decimal Skn_ukKCR  { get; set; }
   public decimal Skn_ukKCRP { get; set; }

   #endregion Kune Backup Values

   #endregion propertiz

   #region ToString

   public override string ToString()
   {
      return string.Format("RecID: {0} FakturRecID: {1} ", RecID, FakturRecID);
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<FaktExStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<FaktExStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<FaktExStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<FaktExStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<FaktExStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      FaktEx newObject = new FaktEx();

      Generic_CloneData<FaktExStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      newObject.Skn_ukRbt1 = this.Skn_ukRbt1;
      newObject.Skn_ukKCR  = this.Skn_ukKCR ;
      newObject.Skn_ukKCRP = this.Skn_ukKCRP;

      return newObject;
   }

   public FaktEx MakeDeepCopy()
   {
      return (FaktEx)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new FaktEx();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      uint origFakturRecID = this.currentData._fakturRecID;

      this.currentData = ((FaktEx)vvDataRecord).currentData;

      this.currentData._fakturRecID = origFakturRecID;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((FaktEx)vvDataRecord).currentData;
   }


   #endregion VvDataRecordFactory

   #region IVvExtenderDataRecord Members

   public string ParentTableName
   {
      get { return Faktur.recordName; }
   }

   public string JoinedColName
   {
      get { return "fakturRecID"; }
   }

   public uint ParentRecID
   {
      get { return FakturRecID; }
      set {        FakturRecID = value; }
   }

   #endregion

}

public class VvIsDevizaConvertibileAttribute : Attribute
{
   public VvIsDevizaConvertibileAttribute(ZXC.JeliJeTakav _jeLiJeTakav)
   {
      this.JeLiJeTakav = _jeLiJeTakav;
   }

   public ZXC.JeliJeTakav JeLiJeTakav;
}

//public class RtransType
//{
//   public RtransType(Rtrans rtrans_rec)
//   {
//      this.RtransData           = rtrans_rec.CurrentData         ;
//      this.RecID_InXML          = rtrans_rec.T_recID             ;
//      this.SaveTransesWriteMode = rtrans_rec.SaveTransesWriteMode;
//   }
//
//   public RtransType()
//   {
//   }
//
//   public RtransStruct  RtransData           { get; set; }
//   public uint          RecID_InXML          { get; set; }
//   public ZXC.WriteMode SaveTransesWriteMode { get; set; }
//}

public class FakturType : VvDataRecordType
{
   public FakturStruct       Faktur   { get; set; }
   public FaktExStruct       FaktEx   { get; set; }
   public List<RtransStruct> Transes  { get; set; }
 //public List<RtransType>   Rtranses { get; set; }
   public List<RtranoStruct> Transes2 { get; set; }

   public FakturType() : this(new Faktur())
   {
   }

   // Connstructor For SERIALIZE 
   public FakturType(Faktur faktur_rec)
   {
      this.TheVvDataRecord = faktur_rec;

                                   this.Faktur = faktur_rec.      CurrentData;
      if(faktur_rec.TheEx != null) this.FaktEx = faktur_rec.TheEx.CurrentData;

      if(faktur_rec.Transes != null)
      {
         this.Transes  = new List<RtransStruct>(faktur_rec.Transes.Count);
       //this.Rtranses = new List<RtransType  >(faktur_rec.Transes.Count);

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            this.Transes .Add(rtrans_rec.CurrentData);
          //this.Rtranses.Add(new RtransType(rtrans_rec));
         }

      } // if(this.faktur_rec.Transes != null)

      if(faktur_rec.Transes2 != null)
      {
         this.Transes2 = new List<RtranoStruct>(faktur_rec.Transes2.Count);

         foreach(Rtrano rtrano_rec in faktur_rec.Transes2)
         {
            this.Transes2.Add(rtrano_rec.CurrentData);
         }

      } // if(this.faktur_rec.Transes != null)

   } // Connstructor For SERIALIZE 

   // Create Faktur From FakturSer - DESERIALIZE 

   public Faktur CreateFakturFromFakturSer()
   {
      Faktur faktur_rec = new Faktur();

                                   faktur_rec.      CurrentData = this.Faktur;
      if(faktur_rec.TheEx != null) faktur_rec.TheEx.CurrentData = this.FaktEx;

      if(this.Transes  != null)
    //if(this.Rtranses != null)
      {
         faktur_rec.Transes = new List<Rtrans/*Struct*/>(this.Transes .Count);
       //faktur_rec.Transes = new List<Rtrans/*Struct*/>(this.Rtranses.Count);

         foreach(RtransStruct thisRtrans in this.Transes )
       //foreach(RtransType   thisRtrans in this.Rtranses)
         {
            faktur_rec.Transes.Add(new Rtrans { CurrentData = thisRtrans } );
          //faktur_rec.Transes.Add(new Rtrans { CurrentData          = thisRtrans.RtransData,
          //                                    SaveTransesWriteMode = thisRtrans.SaveTransesWriteMode,
          //                         /*T_recID*/RecID_InXML          = thisRtrans.RecID_InXML } );
         }

      } // if(this.Transes != null) 

      if(this.Transes2 != null)
      {
         faktur_rec.Transes2 = new List<Rtrano/*Struct*/>(this.Transes2.Count);

         foreach(RtranoStruct thisRtrano in this.Transes2 )
         {
            faktur_rec.Transes2.Add(new Rtrano { CurrentData = thisRtrano } );
         }

      } // if(this.Transes2 != null) 


      return faktur_rec;

   } // CreateFakturFromFakturSer - DESERIALIZE 

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<FakturType>(fileName, _isAutoCreat, TheVvDataRecord);
   }

} // public class FakturType : VvDataRecordType 

