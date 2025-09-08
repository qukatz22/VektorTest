using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
//using static Vektor.DataLayer.DS_Reports.DS_Placa;

public sealed class ZapIzvod
{
   #region ZI PartLengths 2012

   private int[] ZapIzv2012_900_slog_paketa_LABELA_PartLenghts =  
   {
//SADRŽAJ SLOGA ZA TIP SLOGA „900“ – slog paketa – „LABELA“
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ900DVBDIPOS      VBDI – pošiljatelja (banke)             M N   1   7    */ 7           , //Upisuje se VBDI banke
/*2.  IZ900NAZBAN        Naziv banke                             M C   8   57   */ 50          , //Upisuje se naziv banke
/*3.  IZ9000OIBBNK       OIB banke                               M N   58  68   */ 11          , //Upisuje se OIB banke
/*4.  IZ900VRIZ          Vrsta izvatka                           O N   69  72   */ 4           , //Upisuje se 1000 i označava duljinu sloga odnosno poziciju na koji se nalazi tip sloga (IZ900TIPSL)
/*5.  IZ900DATUM         Datum obrade – tekući datumGGGGMMDD     M C   73  80   */ 8           , //Upisuje se datum kreiranja izvatka na elektronskom mediju u formatu GGGGMMDD 
/*6.  IZ900REZ2          Rezerva                                 O C   81  997  */ 917         , //
/*7.  IZ900TIPSL         Tip sloga                               M N   998 1000 */ 3           , //
   };

   private int[] ZapIzv2012_903_vodeći_slog_grupe_podataka_PartLenghts =  
   {
//SADRŽAJ SLOGA ZA TIP SLOGA „903“ – vodeći slog grupe podataka
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ903VBDI          Vodeći broj banke                       M N   1   7    */ 7           , //
/*2.  IZ903BIC           BIC - Identifikacijska šifra banke      O C   8   18   */ 11          , //
/*3.  IZ903RACUN         Transakcijski račun klijenta            M C   19  39   */ 21          , //
/*4.  IZ903VLRN          Valuta transakcijskog računa            M C   40  42   */ 3           , //
/*5.  IZ903NAZKLI        Naziv klijenta                          M C   43  112  */ 70          , //
/*6.  IZ903SJEDKLI       Sjedište klijenta                       M C   113 147  */ 35          , //
/*7.  IZ903MB            Matični broj                            O N   148 155  */ 8           , //
/*8.  IZ903OIBKLI        OIB klijenta                            O N   156 166  */ 11          , //
/*9.  IZ903RBIZV         Redni broj izvatka                      M N   167 169  */ 3           , //
/*10. IZ903PODBR         Podbroj izvatka                         M N   170 172  */ 3           , //
/*11. IZ903DATUM         Datum izvatka                           M N   173 180  */ 8           , //Prikazuje se u formatu GGGGMMDD
/*12. IZ903BRGRU         Redni broj grupe paketa                 M N   181 184  */ 4           , //
/*13. IZ903VRIZ          Vrsta izvatka                           M N   185 188  */ 4           , //Upisuje se oznaka 1000
/*14. IZ903REZ           Rezerva                                 O C   189 997  */ 809         , //
/*15. IZ903TIPSL         Tip sloga                               M N   998 1000 */ 3           , //
   };

   private int[] ZapIzv2012_905_slog_pojedinacne_transakcije_PartLenghts =  
   {
//SADRŽAJ SLOGA ZA TIP SLOGA „905“ – slog pojedinačne transakcije
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE

//kao oznaka vrste transakcije čime se imatelju transakcijskog računa daje objašnjenje oznake vrste transakcije - izvršenje naloga „NA TERET“ transakcijskog računa korisnika 
//(DUGUJE) označeno je u slogu pojedinačne transakcije oznakom transakcije  „10“. Tada se u polju „IZ905RNPRPL“ nalazi transakcijski račun primatelja- izvršenje naloga 
//„U KORIST“ transakcijskog računa korisnika (POTRAŽUJE) označeno je u slogu pojedinačne transakcije oznakom transakcije „20“. Tada se u polju „IZ905RNPRPL“ nalazi 
//transakcijski račun platitelja
/*1.  IZ905OZTRA         Oznaka transakcije                      M N   1   2    */ 2           , //- upisuje se podatak “10“ ili „20“ qukatz vidi dva reda gore pojasnjenje
/*2.  IZ905RNPRPL        Račun primatelja-platitelja             M C   3   36   */ 34          , //
/*3.  IZ905NAZPRPL       Naziv primatelja-platitelja             M C   37  106  */ 70          , //
/*4.  IZ905ADRPRPL       Adresa primatelja-platitelja            O C   107 141  */ 35          , //
/*5.  IZ905SJPRPL        Sjedište primatelja-platitelja          O C   142 176  */ 35          , //
/*6.  IZ905DATVAL        Datum valute (GGGGMMDD)                 M C   177 184  */ 8           , //Prikazuje se u formatu GGGGMMDD
/*7.  IZ905DATIZVR       Datum izvršenja (GGGGMMDD)              M C   185 192  */ 8           , //Prikazuje se u formatu GGGGMMDD
/*8.  IZ905VLPL          Valuta pokrića                          O C   193 195  */ 3           , //
/*9.  IZ905TECAJ         Tečaj/koeficijent                       O N   196 210  */ 15          , //9+6 znakova – 6 su decimale
/*10. IZ905PREDZN        Predznak „+“ a u slučaju storna „-„     O C   211 211  */ 1           , //
/*11. IZ905IZNOSPPVALUTE Iznos u valuti pokrića                  O N   212 226  */ 15          , //
/*12. IZ905PREDZN        Predznak „+“ a u slučaju storna „-„     M C   227 227  */ 1           , //
/*13. IZ905IZNOS         Iznos                                   M N   228 242  */ 15          , //Iznos u valuti transakcijskog računa iz polja IZ903VLRN
/*14. IZ905PNBPL         Poziv na broj platitelja                O C   243 268  */ 26          , //
/*15. IZ905PNBPR         Poziv na broj primatelja                O C   269 294  */ 26          , //
/*16. IZ905SIFNAM        Šifra namjene                           O N   295 298  */ 4           , //Prikazuje se šifra namjene transakcije šrema ISO standardu 20022
/*17. IZ905OPISPL        Opis plaćanja                           O C   299 438  */ 140         , //
/*18. IZ905IDTRFINA      Identifik transak–inicirano u FINI      O C   439 480  */ 42          , //Upisuje se identifikator transakcije inicirane u FINI - broj za reklamaciju
/*19. IZ905IDTRBAN       Identifik transak–inicirano izvan FINE  M C   481 515  */ 35          , //Upisuje se identifikator transakcije banke 
/*20. IZ905REZ2          Rezerva                                 O N   516 997  */ 482         , //
/*21. IZ905TIPSL         Tip sloga                               M N   998 1000 */ 3           , //
   };

   private int[] ZapIzv2012_907_zaključni_slog_grupe_podataka_PartLenghts =  
   {
//SADRŽAJ SLOGA ZA TIP SLOGA „907“– zaključni slog grupe podataka
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ907RAČUN         Transakcijski račun klijenta            M C   1   21   */ 21          , //
/*2.  IZ907VLRN          Valuta transakcijskog računa            M C   22  24   */ 3           , //
/*3.  IZ907NAZKLI        Naziv klijenta                          M C   25  94   */ 70          , //
/*4.  IZ907RBIZV         Redni broj Izvatka                      M N   95  97   */ 3           , //
/*5.  IZ907PRRBIZV       Redni broj prethodnog Izvatka           O N   98  100  */ 3           , //
/*6.  IZ907DATUM         Datum izvatka                           M N   101 108  */ 8           , //Prikazuje se u formatu GGGGMMDD GGGGMMDD
/*7.  IZ907DATPRSAL      Datum prethodnog stanja (GGGGMMDD)      O C   109 116  */ 8           , //Prikazuje se u formatu GGGGMMDD
/*8.  IZ907PPPOS         Predznak prethodnog stanja              M C   117 117  */ 1           , //
/*9.  IZ907PRSAL         Prethodno stanje                        M N   118 132  */ 15          , //
/*10. IZ907PREREZ        Predznak rezervacije                    O C   133 133  */ 1           , //
/*11. IZ907IZNREZ        Iznos rezervacije                       O N   134 148  */ 15          , //
/*12. IZ907DATOKV        Datum dozvoljenog prekoračenja (GGGMMDD)O C   149 156  */ 8           , //
/*13. IZ907IZNOKV        Dozvoljeno prekoračenje (okvirni kredit)O N   157 171  */ 15          , //
/*14. IZ907IZNZAPSR      Iznos zaplijenjenih sredstava           O C   172 186  */ 15          , //
/*15. IZ907PRASPSTA      Predznak raspoloživog stanja            O C   187 187  */ 1           , //
/*16. IZ907IZNRASP       Iznos raspoloživog stanja               O N   188 202  */ 15          , //
/*17. IZ907PDUGU         Predznak ukupnog dugovnog prometa       M C   203 203  */ 1           , //
/*18. IZ907KDUGU         Ukupni dugovni promet                   M N   204 218  */ 15          , //
/*19. IZ907PPOTR         Predznak ukupnog potražnog prometa      M C   219 219  */ 1           , //
/*20. IZ907KPOTR         Ukupni potražni promet                  M N   220 234  */ 15          , //
/*21. IZ07PRNOS          Predznak novog stanja                   M C   235 235  */ 1           , //
/*22. IZ907KOSAL         Novo stanje                             M N   236 250  */ 15          , //
/*23. IZ907BRGRU         Redni broj grupe u paketu               O N   251 254  */ 4           , //
/*24. IZ907BRSTA         Broj stavaka u grupi                    O N   255 260  */ 6           , //
/*25. IZ907TEKST         Tekstualna poruka                       O C   261 680  */ 420         , //Upisuje u jednom retku
/*26. IZ907REZ2          Rezerva                                 O C   681 997  */ 317         , //
/*27. IZ907TIPSL         Tip sloga                               M N   998 1000 */ 3           , //
   };

   private int[] ZapIzv2012_909_zakljucni_slog_paketa_PartLenghts =  
   {
//SADRŽAJ OZNAKA ZA TIP  SLOGA „909“ – zaključni slog paketa
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ909DATUM         Datum obrade                            M N   1   8    */ 8           , //Upisuje se u formatu: GGGGMMDD
/*2.  IZ909UKGRU         Ukupan broj grupa/paket                 M N   9   13   */ 5           , //
/*3.  IZ909UKSLG         Ukupan broj slog/paket                  M N   14  19   */ 6           , //
/*4.  IZ909REZ3          Rezerva                                 O C   20  997  */ 978         , //
/*5.  IZ909TIPSL         Tip sloga                               M N   998 1000 */ 3           , //
   };

   private int[] ZapIzv2012_999_zakljucni_slog_datoteke_PartLenghts =  
   {
//SADRŽAJ OZNAKA ZA TIP  SLOGA „999“ – zaključni slog datoteke
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ999REZ1          Rezervirana mjesta                      O C   1   997  */ 997         ,
/*2.  IZ999TIPSL         Tip sloga – oznaka 999                  M C   998 1000 */ 3           ,
   };

   #endregion 2012

   #region ZI PartLengths OLD: Bef 2012

   // zih 
   private int[] ZapIzvodHead_PartLenghts =  
   {
      /*  1 IZ0BRZAP*/     5,      //       1   5  Fiksno 00000
      /*  2 IZ0TKS  */     1,      //       6   6  Prazno
      /*  3 IZ0VDP  */     1,      //       7   7  Prazno
      /*  4 IZ0DATUM*/     6,      //   	  8  13  Datum izrade izvadka (u formatu DDMMGG) // File Creation
      /*  5 IZ0MPRIM*/     7,      //   	 14  20	Fiksno 0000000
      /*  6 IZ0VRIZ */     3,      //      21  23 +Fiksno 250
      /*  7 IZ0REZ1 */   168,      //      23 191	Prazno
      /*  8 IZ0D2000*/     8,      //   	192 199 +Datum izrade izvadka (u formatu DDMMGGGG)
      /*  9 IZ0REZ2 */    50,      //   	200 249	Prazno
      /* 10 IZ0TIPSL*/     1,      //   	250 250 +Fiksno 0
   };

   // zids 
   private int[] ZapIzvodDateStart_PartLenghts =  
   {

      /*  1 IZ3RNPRI */   18,      //   	  1    18  +Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      /*  2 IZ3NAPRI */   35,      //   	 19    53  +Naziv vlasnika transakcijskog računa
      /*  3 IZ3RBIZV */    3,      //   	 54    56  +Redni broj izvatka (popunjeno sa vodećim nulama)
      /*  4 IZ3DATUM */    6,      //   	 57    62   Datum izvatka (u formatu DDMMGG)
      /*  5 IZ3PRETI */    5,      //   	 63    67	Fiksno 00000
      /*  6 IZ3BRGRU */    4,      //   	 68    71  +Redni broj grupe podataka u datoteci izvatka (popunjeno sa vodećim nulama)
      /*  7 IZ3NAZAP */   35,      //   	 72   106	Prazno
      /*  8 IZ3PRESJ */    3,      //   	107	109	Fiksno 000
      /*  9 IZ3TKS   */    1,      //   	110	110	Prazno
      /* 10 IZ3VDP   */    1,      //   	111	111	Prazno
      /* 11 IZ3VRIZ  */    3,      //   	112	114  +Fiksno 250
      /* 12 IZ3VBDI  */    7,      //   	115	121  +Vodeći broj depozitne istitucije (fiksno 2484008)
      /* 13 IZ3KONSTR*/    1,      //   	122	122	TT konstrukcije računa (fiksno N)
      /* 14 IZ3REZ1  */   69,      //   	123	191	Prazno
      /* 15 IZ3D2000 */    8,      //   	192	199  +Datum izvatka (u formatu DDMMGGGG)
      /* 16 IZ3REZ2  */   50,      //   	200	249	Prazno
      /* 17 IZ3TIPSL */    1,      //   	250	250  +Fiksno 3

   };

   // zid
   private int[] ZapIzvodData_PartLenghts =  
   {

      /*  1 IZ5OZTRA  */    2,      //      1	  2  +Vrsta transakcije (10 - duguje, 20 - potra3/4uje)
      /*  2 IZ5RNPVD  */   18,      //      3	 20  +Račun vjerovnika  / du3/4nika
      /*  3 IZ5NAZVD  */   35,      //     21	 55  +Naziv vjerovnika  / du3/4nika
      /*  4 IZ5MJEVD  */   10,      //     56	 65  +Mjesto vjerovnika / du3/4nika
      /*  5 IZ5IZNOS  */   13,      //   	 66	 78  +Iznos (popunjeno sa vodećim nulama)
      /*  6 IZ5PNBZA  */   24,      //   	 79	102  +Poziv na broj zadu3/4enja (prve dvije znamenke predstavljau model poziva)
      /*  7 IZ5PNBOD  */   24,      //   	103	126  +Poziv na broj odobrenja (prve dvije znamenke predstavljau model poziva)
      /*  8 IZ5VEZOZ  */    2,      //   	127	128  ?Vezna oznaka
      /*  9 IZ5SVDOZ  */   36,      //   	129	164  +Opis svrhe doznake
      /* 10 IZ5KONSTR */    1,      //   	165	165  +Oznaka konstrukcije računa (N-nova / S-stara konstrukcija)
      /* 11 IZ5REKLA  */   32,      //   	166	197  +Broj za reklamaciju
      /* 12 IZ5VBDI   */    7,      //   	198	204  +Vodeći broj depozitne institucije vjerovnika / du3/4nika (samo za novu konstrukciju računa, inače popunjeno sa 0000000)
      /* 13 IZ5VRPRIH */    3,      //   	205	207	Fiksno 100
      /* 14 IZV5IZVDOK*/    3,      //   	208	210	Fiksno 100
      /* 15 IZ5DATVAL */    8,      //   	211	218  +Datum valute transakcije (u formatu DDMMGGGG)
      /* 16 IZ5REZ2   */   31,      //   	219	249	Prazno
      /* 17 IZ5TIPSL  */    1,      //   	250	250  +Fiksno 5

   };

   // zide 
   private int[] ZapIzvodDateEnd_PartLenghts =  
   {

      /*  1 IZ7RNPRI */   18,       //    1   18  -Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      /*  2 IZ7NAPRI */   35,       //   19	 53  -Naziv vlasnika transakcijskog računa
      /*  3 IZ7RBIZV */    3,       //   54	 56  -Redni broj izvatka (popunjeno sa vodećim nulama)
      /*  4 IZ7DATUM */    6,       //   57	 62	Datum izvatka (u formatu DDMMGG)
      /*  5 IZ7PRSAL */   15,       //   63	 77  +Prethodni saldoOTS (popunjeno sa vodećim nulama)
      /*  6 IZ7KDUGU */   15,       //   78	 92  +Ukupni dnevni dugovni promet (popunjeno sa vodećim nulama)
      /*  7 IZ7KPOTR */   15,       //   93	107  +Ukupni dnevni potra3/4ni promet (popunjeno sa vodećim nulama)
      /*  8 IZ7KOSAL */   15,       //  108	122  +Novi saldoOTS (popunjeno sa vodećim nulama)
      /*  9 IZ7BRGRU */    4,       //  123	126  -Redni broj grupe u datoteci (popunjeno sa vodećim nulama)
      /* 10 IZ7BRSTA */    6,       //  127	132  +Broj stavaka u grupi (popunjeno sa vodećim nulama)
      /* 11 IZ7NAZAP */   35,       //  133	167	Prazno
      /* 12 IZ7PRESJ */    3,       //  168	170	Fiksno 000
      /* 13 IZ7PPPOS */    1,       //  171	171  +Predznak početnog stanja (+ ili -)
      /* 14 IZ7PRNOS */    1,       //  172	172  +Predznak novog stanja (+ ili -)
      /* 15 IZ7RAZSD */    1,       //  173	173	Prazno
      /* 16 IZ7RAZSP */    1,       //  174	174	Prazno
      /* 17 IZ7REZ1  */   17,       //  175	191	Prazno
      /* 18 IZ7D2000 */    8,       //  192	199  -Datum izvatka  (u formatu DDMMGGGG)
      /* 19 IZ7VBDI  */    7,       //  200	206  -Vodeći broj depozitne istitucije  (fiksno 2484008)
      /* 20 IZ7KONSTR*/    1,       //  207	207  -TT konstrukcije računa (fiksno N) // Ours
      /* 21 IZ7REZ2  */   42,       //  208	249	Prazno
      /* 22 IZ7TIPSL */    1,       //  250	250  +Fiksno 7

   };

   // zieof 
   private int[] ZapIzvodDateEOF_PartLenghts =  
   {

      /*  1 IZ9BRZAP*/     5,        //    1   5	Fiksno 00000
      /*  2 IZ9REZ  */     2,        //    6   7	Prazno
      /*  3 IZ9DATUM*/     6,        //    8  13	Datum izrade izvadka (u formatu DDMMGG)
      /*  4 IZ9MPRIM*/     7,        //   14  20	Fiksno 0000000
      /*  5 IZ9UKGRU*/     5,        //   21  25	Ukupan broj grupa u datoteci (popunjeno sa vodećim nulama)
      /*  6 IZ9UKSLG*/     6,        //   26  31  +Ukupan broj svih slogova u datoteci (popunjeno sa vodećim nulama)
      /*  7 IZ9REZ2 */   160,        //   32	191	Prazno
      /*  8 IZ9D2000*/     8,        //  192	199	Datum izrade izvadka (u formatu DDMMGGGG)
      /*  9 IZ9REZ3 */    50,        //  200	249	Prazno
      /* 10 IZ9TIPSL*/     1,        //  250	250  +Fiksno 9

   };

   #endregion ZI PartLengths

   #region InnerStructures - PUSE 

#if DELOVSJI   
   // Struktura ZAP Izvoda

   // zih 
   private struct ZapIzvodHead
   {
      internal char[] IZ0BRZAP/*[5]*/;       //      1   5  Fiksno 00000
      internal char[] IZ0TKS/*[1]*/;         //      6   6  Prazno
      internal char[] IZ0VDP/*[1]*/;         //      7   7  Prazno
      internal char[] IZ0DATUM/*[6]*/;       //   	  8  13  Datum izrade izvadka (u formatu DDMMGG) // File Creation
      internal char[] IZ0MPRIM/*[7]*/;       //   	 14  20	Fiksno 0000000
      internal char[] IZ0VRIZ/*[3]*/;        //     21  23 +Fiksno 250
      internal char[] IZ0REZ1/*[168]*/;      //     23 191	Prazno
      internal char[] IZ0D2000/*[8]*/;       //   	192 199 +Datum izrade izvadka (u formatu DDMMGGGG)
      internal char[] IZ0REZ2/*[50]*/;       //   	200 249	Prazno
      internal char[] IZ0TIPSL/*[1]*/;       //   	250 250 +Fiksno 0

      char cr, lf;

   };  

   // zids 
   private struct ZapIzvodDateStart
   {

      internal char[] IZ3RNPRI/*[18]*/;      //   	  1    18  +Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      internal char[] IZ3NAPRI/*[35]*/;      //   	 19    53  +Naziv vlasnika transakcijskog računa
      internal char[] IZ3RBIZV/*[3]*/;       //   	 54    56  +Redni broj izvatka (popunjeno sa vodećim nulama)
      internal char[] IZ3DATUM/*[6]*/;       //   	 57    62   Datum izvatka (u formatu DDMMGG)
      internal char[] IZ3PRETI/*[5]*/;       //   	 63    67	Fiksno 00000
      internal char[] IZ3BRGRU/*[4]*/;       //   	 68    71  +Redni broj grupe podataka u datoteci izvatka (popunjeno sa vodećim nulama)
      internal char[] IZ3NAZAP/*[35]*/;      //   	 72   106	Prazno
      internal char[] IZ3PRESJ/*[3]*/;       //   	107	109	Fiksno 000
      internal char[] IZ3TKS/*[1]*/;         //   	110	110	Prazno
      internal char[] IZ3VDP/*[1]*/;         //   	111	111	Prazno
      internal char[] IZ3VRIZ/*[3]*/;        //   	112	114  +Fiksno 250
      internal char[] IZ3VBDI/*[7]*/;        //   	115	121  +Vodeći broj depozitne istitucije (fiksno 2484008)
      internal char[] IZ3KONSTR/*[1]*/;      //   	122	122	Tip konstrukcije računa (fiksno N)
      internal char[] IZ3REZ1/*[69]*/;       //   	123	191	Prazno
      internal char[] IZ3D2000/*[8]*/;       //   	192	199  +Datum izvatka (u formatu DDMMGGGG)
      internal char[] IZ3REZ2/*[50]*/;       //   	200	249	Prazno
      internal char[] IZ3TIPSL/*[1]*/;       //   	250	250  +Fiksno 3

      char cr, lf;

   };  

   // zid
   private struct ZapIzvodData
   {

      internal char[] IZ5OZTRA/*[2]*/;       //      1	  2  +Vrsta transakcije (10 - duguje, 20 - potra3/4uje)
      internal char[] IZ5RNPVD/*[18]*/;      //      3	 20  +Račun vjerovnika  / du3/4nika
      internal char[] IZ5NAZVD/*[35]*/;      //     21	 55  +Naziv vjerovnika  / du3/4nika
      internal char[] IZ5MJEVD/*[10]*/;      //     56	 65  +Mjesto vjerovnika / du3/4nika
      internal char[] IZ5IZNOS/*[13]*/;      //   	 66	 78  +Iznos (popunjeno sa vodećim nulama)
      internal char[] IZ5PNBZA/*[24]*/;      //   	 79	102  +Poziv na broj zadu3/4enja (prve dvije znamenke predstavljau model poziva)
      internal char[] IZ5PNBOD/*[24]*/;      //   	103	126  +Poziv na broj odobrenja (prve dvije znamenke predstavljau model poziva)
      internal char[] IZ5VEZOZ/*[2]*/;       //   	127	128  ?Vezna oznaka
      internal char[] IZ5SVDOZ/*[36]*/;      //   	129	164  +Opis svrhe doznake
      internal char[] IZ5KONSTR/*[1]*/;      //   	165	165  +Oznaka konstrukcije računa (N-nova / S-stara konstrukcija)
      internal char[] IZ5REKLA/*[32]*/;      //   	166	197  +Broj za reklamaciju
      internal char[] IZ5VBDI/*[7]*/;        //   	198	204  +Vodeći broj depozitne institucije vjerovnika / du3/4nika (samo za novu konstrukciju računa, inače popunjeno sa 0000000)
      internal char[] IZ5VRPRIH/*[3]*/;      //   	205	207	Fiksno 100
      internal char[] IZV5IZVDOK/*[3]*/;     //   	208	210	Fiksno 100
      internal char[] IZ5DATVAL/*[8]*/;      //   	211	218  +Datum valute transakcije (u formatu DDMMGGGG)
      internal char[] IZ5REZ2/*[31]*/;       //   	219	249	Prazno
      internal char[] IZ5TIPSL/*[1]*/;       //   	250	250  +Fiksno 5

      char cr, lf;

   };      

   // zide 
   private struct ZapIzvodDateEnd
   {

      internal char[] IZ7RNPRI/*[18]*/;       //    1  18  -Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      internal char[] IZ7NAPRI/*[35]*/;       //   19	 53  -Naziv vlasnika transakcijskog računa
      internal char[] IZ7RBIZV/*[3]*/;        //   54	 56  -Redni broj izvatka (popunjeno sa vodećim nulama)
      internal char[] IZ7DATUM/*[6]*/;        //   57	 62	Datum izvatka (u formatu DDMMGG)
      internal char[] IZ7PRSAL/*[15]*/;       //   63	 77  +Prethodni saldo (popunjeno sa vodećim nulama)
      internal char[] IZ7KDUGU/*[15]*/;       //   78	 92  +Ukupni dnevni dugovni promet (popunjeno sa vodećim nulama)
      internal char[] IZ7KPOTR/*[15]*/;       //   93	107  +Ukupni dnevni potra3/4ni promet (popunjeno sa vodećim nulama)
      internal char[] IZ7KOSAL/*[15]*/;       //  108	122  +Novi saldo (popunjeno sa vodećim nulama)
      internal char[] IZ7BRGRU/*[4]*/;        //  123	126  -Redni broj grupe u datoteci (popunjeno sa vodećim nulama)
      internal char[] IZ7BRSTA/*[6]*/;        //  127	132  +Broj stavaka u grupi (popunjeno sa vodećim nulama)
      internal char[] IZ7NAZAP/*[35]*/;       //  133	167	Prazno
      internal char[] IZ7PRESJ/*[3]*/;        //  168	170	Fiksno 000
      internal char[] IZ7PPPOS/*[1]*/;        //  171	171  +Predznak početnog stanja (+ ili -)
      internal char[] IZ7PRNOS/*[1]*/;        //  172	172  +Predznak novog stanja (+ ili -)
      internal char[] IZ7RAZSD/*[1]*/;        //  173	173	Prazno
      internal char[] IZ7RAZSP/*[1]*/;        //  174	174	Prazno
      internal char[] IZ7REZ1/*[17]*/;        //  175	191	Prazno
      internal char[] IZ7D2000/*[8]*/;        //  192	199  -Datum izvatka  (u formatu DDMMGGGG)
      internal char[] IZ7VBDI/*[7]*/;         //  200	206  -Vodeći broj depozitne istitucije  (fiksno 2484008)
      internal char[] IZ7KONSTR/*[1]*/;       //  207	207  -Tip konstrukcije računa (fiksno N) // Ours
      internal char[] IZ7REZ2/*[42]*/;        //  208	249	Prazno
      internal char[] IZ7TIPSL/*[1]*/;        //  250	250  +Fiksno 7

      char cr, lf;

   };      

   // zieof 
   private struct ZapIzvodDateEOF
   {

      internal char[] IZ9BRZAP/*[5]*/;        //    1   5	Fiksno 00000
      internal char[] IZ9REZ/*[2]*/;          //    6   7	Prazno
      internal char[] IZ9DATUM/*[6]*/;        //    8  13	Datum izrade izvadka (u formatu DDMMGG)
      internal char[] IZ9MPRIM/*[7]*/;        //   14  20	Fiksno 0000000
      internal char[] IZ9UKGRU/*[5]*/;        //   21  25	Ukupan broj grupa u datoteci (popunjeno sa vodećim nulama)
      internal char[] IZ9UKSLG/*[6]*/;        //   26  31  +Ukupan broj svih slogova u datoteci (popunjeno sa vodećim nulama)
      internal char[] IZ9REZ2/*[160]*/;       //   32	191	Prazno
      internal char[] IZ9D2000/*[8]*/;        //  192	199	Datum izrade izvadka (u formatu DDMMGGGG)
      internal char[] IZ9REZ3/*[50]*/;        //  200	249	Prazno
      internal char[] IZ9TIPSL/*[1]*/;        //  250	250  +Fiksno 9

      char cr, lf;

   };

   private ZapIzvodHead zih;  // Ň0Ó (labela) - javlja se samo jednom na početku datoteke
   private ZapIzvodDateStart zids; // Ň3Ó (vodeći slog grupe podataka) - javlja se jednom za svaki datum knji3/4enja
   private ZapIzvodData zid;  // Ň5Ó (pojedinačni nalog) - predstavlja jednu stavku unutar logičke cjeline
   private ZapIzvodDateEnd zide; // Ň7Ó (zavrąni slog grupe podataka) - javlja se jednom za svaki datum knji3/4enja
   private ZapIzvodDateEOF zieof;// Ň9Ó (zavrąni slog datoteke) - javlja se samo jednom kao posljednji fizički slog datoteke

   private void InitializeCharArrays()
   {
   #region CharArrayInitialization

      // zih 
      zih.IZ0BRZAP = new char[5];        //      1     5  Fiksno 00000
      zih.IZ0TKS   = new char[1];        //      6     6  Prazno
      zih.IZ0VDP   = new char[1];        //      7     7  Prazno
      zih.IZ0DATUM = new char[6];        //      8    13  Datum izrade izvadka (u formatu DDMMGG) // File Creation
      zih.IZ0MPRIM = new char[7];        //      14   20  Fiksno 0000000
      zih.IZ0VRIZ  = new char[3];        //      21   23 +Fiksno 250
      zih.IZ0REZ1  = new char[168];      //      23  191  Prazno
      zih.IZ0D2000 = new char[8];        //   	192  199 +Datum izrade izvadka (u formatu DDMMGGGG)
      zih.IZ0REZ2  = new char[50];       //   	200  249  Prazno
      zih.IZ0TIPSL = new char[1];        //   	250  250 +Fiksno 0

      // zids 
      zids.IZ3RNPRI  = new char[18];      //   	  1    18  +Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      zids.IZ3NAPRI  = new char[35];      //   	 19    53  +Naziv vlasnika transakcijskog računa
      zids.IZ3RBIZV  = new char[3];       //   	 54    56  +Redni broj izvatka (popunjeno sa vodećim nulama)
      zids.IZ3DATUM  = new char[6];       //   	 57    62   Datum izvatka (u formatu DDMMGG)
      zids.IZ3PRETI  = new char[5];       //   	 63    67	Fiksno 00000
      zids.IZ3BRGRU  = new char[4];       //   	 68    71  +Redni broj grupe podataka u datoteci izvatka (popunjeno sa vodećim nulama)
      zids.IZ3NAZAP  = new char[35];      //   	 72   106	Prazno
      zids.IZ3PRESJ  = new char[3];       //   	107	109	Fiksno 000
      zids.IZ3TKS    = new char[1];       //   	110	110	Prazno
      zids.IZ3VDP    = new char[1];       //   	111	111	Prazno
      zids.IZ3VRIZ   = new char[3];       //   	112	114  +Fiksno 250
      zids.IZ3VBDI   = new char[7];       //   	115	121  +Vodeći broj depozitne istitucije (fiksno 2484008)
      zids.IZ3KONSTR = new char[1];       //   	122	122	Tip konstrukcije računa (fiksno N)
      zids.IZ3REZ1   = new char[69];      //   	123	191	Prazno
      zids.IZ3D2000  = new char[8];       //   	192	199  +Datum izvatka (u formatu DDMMGGGG)
      zids.IZ3REZ2   = new char[50];      //   	200	249	Prazno
      zids.IZ3TIPSL  = new char[1];       //   	250	250  +Fiksno 3

      // zid
      zid.IZ5OZTRA   = new char[2];       //      1	  2  +Vrsta transakcije (10 - duguje, 20 - potra3/4uje)
      zid.IZ5RNPVD   = new char[18];      //      3	 20  +Račun vjerovnika  / du3/4nika
      zid.IZ5NAZVD   = new char[35];      //     21	 55  +Naziv vjerovnika  / du3/4nika
      zid.IZ5MJEVD   = new char[10];      //     56	 65  +Mjesto vjerovnika / du3/4nika
      zid.IZ5IZNOS   = new char[13];      //   	 66	 78  +Iznos (popunjeno sa vodećim nulama)
      zid.IZ5PNBZA   = new char[24];      //   	 79	102  +Poziv na broj zadu3/4enja (prve dvije znamenke predstavljau model poziva)
      zid.IZ5PNBOD   = new char[24];      //   	103	126  +Poziv na broj odobrenja (prve dvije znamenke predstavljau model poziva)
      zid.IZ5VEZOZ   = new char[2];       //   	127	128  ?Vezna oznaka
      zid.IZ5SVDOZ   = new char[36];      //   	129	164  +Opis svrhe doznake
      zid.IZ5KONSTR  = new char[1];       //   	165	165  +Oznaka konstrukcije računa (N-nova / S-stara konstrukcija)
      zid.IZ5REKLA   = new char[32];      //   	166	197  +Broj za reklamaciju
      zid.IZ5VBDI    = new char[7];       //   	198	204  +Vodeći broj depozitne institucije vjerovnika / du3/4nika (samo za novu konstrukciju računa, inače popunjeno sa 0000000)
      zid.IZ5VRPRIH  = new char[3];       //   	205	207	Fiksno 100
      zid.IZV5IZVDOK = new char[3];       //   	208	210	Fiksno 100
      zid.IZ5DATVAL  = new char[8];       //   	211	218  +Datum valute transakcije (u formatu DDMMGGGG)
      zid.IZ5REZ2    = new char[31];      //   	219	249	Prazno
      zid.IZ5TIPSL   = new char[1];       //   	250	250  +Fiksno 5

      // zide 
      zide.IZ7RNPRI = new char[18];       //    1   18  -Broj transakcijskog računa (popunjeno prvih 10 mjesta, ostalih 8 mjesta su praznine)
      zide.IZ7NAPRI = new char[35];       //   19	 53  -Naziv vlasnika transakcijskog računa
      zide.IZ7RBIZV = new char[3];        //   54	 56  -Redni broj izvatka (popunjeno sa vodećim nulama)
      zide.IZ7DATUM = new char[6];        //   57	 62	Datum izvatka (u formatu DDMMGG)
      zide.IZ7PRSAL = new char[15];       //   63	 77  +Prethodni saldo (popunjeno sa vodećim nulama)
      zide.IZ7KDUGU = new char[15];       //   78	 92  +Ukupni dnevni dugovni promet (popunjeno sa vodećim nulama)
      zide.IZ7KPOTR = new char[15];       //   93	107  +Ukupni dnevni potra3/4ni promet (popunjeno sa vodećim nulama)
      zide.IZ7KOSAL = new char[15];       //  108	122  +Novi saldo (popunjeno sa vodećim nulama)
      zide.IZ7BRGRU = new char[4];        //  123	126  -Redni broj grupe u datoteci (popunjeno sa vodećim nulama)
      zide.IZ7BRSTA = new char[6];        //  127	132  +Broj stavaka u grupi (popunjeno sa vodećim nulama)
      zide.IZ7NAZAP = new char[35];       //  133	167	Prazno
      zide.IZ7PRESJ = new char[3];        //  168	170	Fiksno 000
      zide.IZ7PPPOS = new char[1];        //  171	171  +Predznak početnog stanja (+ ili -)
      zide.IZ7PRNOS = new char[1];        //  172	172  +Predznak novog stanja (+ ili -)
      zide.IZ7RAZSD = new char[1];        //  173	173	Prazno
      zide.IZ7RAZSP = new char[1];        //  174	174	Prazno
      zide.IZ7REZ1  = new char[17];       //  175	191	Prazno
      zide.IZ7D2000 = new char[8];        //  192	199  -Datum izvatka  (u formatu DDMMGGGG)
      zide.IZ7VBDI  = new char[7];        //  200	206  -Vodeći broj depozitne istitucije  (fiksno 2484008)
      zide.IZ7KONSTR= new char[1];        //  207	207  -Tip konstrukcije računa (fiksno N) // Ours
      zide.IZ7REZ2  = new char[42];       //  208	249	Prazno
      zide.IZ7TIPSL = new char[1];        //  250	250  +Fiksno 7

      // zieof 
      zieof.IZ9BRZAP = new char[5];        //    1   5	Fiksno 00000
      zieof.IZ9REZ   = new char[2];        //    6   7	Prazno
      zieof.IZ9DATUM = new char[6];        //    8  13	Datum izrade izvadka (u formatu DDMMGG)
      zieof.IZ9MPRIM = new char[7];        //   14  20	Fiksno 0000000
      zieof.IZ9UKGRU = new char[5];        //   21  25	Ukupan broj grupa u datoteci (popunjeno sa vodećim nulama)
      zieof.IZ9UKSLG = new char[6];        //   26  31  +Ukupan broj svih slogova u datoteci (popunjeno sa vodećim nulama)
      zieof.IZ9REZ2  = new char[160];      //   32	191	Prazno
      zieof.IZ9D2000 = new char[8];        //  192	199	Datum izrade izvadka (u formatu DDMMGGGG)
      zieof.IZ9REZ3  = new char[50];       //  200	249	Prazno
      zieof.IZ9TIPSL = new char[1];        //  250	250  +Fiksno 9

   #endregion CharArrayInitialization
   }

#endif

   #endregion InnerStructures - PUSE

   #region Public Structures

   public struct HeadRecordStruct //NetizvRecord;
   {

      internal uint     iz_br;
      internal DateTime iz_date;
      
      internal char     iz_prev_sign;
      internal char     iz_new_sign;
      
      internal string   iz_firma/*[36]*/;
      internal string   bankaName/*[36]*/;
      internal string   bankaVbdi/*[8]*/;
      internal string   iz_ziro_rnpri/*[19+1]*/;
      internal string   iz_ziro_racun/*[24]*/;
      //internal string   iz_broj_zapa/*[6]*/;
      
      //internal decimal  iz_prev_dug;
      //internal decimal  iz_prev_pot;

      internal decimal  iz_uk_dug;
      internal decimal  iz_uk_pot;

      internal decimal  iz_prev_saldo;
      internal decimal  iz_saldo;

      // ZNP additions: 
      internal string   iz_mjesto;
      internal DateTime iz_dateIn6;
      internal uint     iz_numOfTranses;

      // new fields in 2012:
      internal string  bankaOIB  ;
      internal string  firmaOIB  ;
      internal string  valutaName;
   }; 

   public struct TransRecordStruct // NITransRecord
   {

      internal long     t_br;  // kopija odozgo;

      internal int      t_serial;
      internal DateTime t_valuta/*[9]*/;

      internal string  t_rbr/*[6]*/;
      internal string  t_kd_ziro/*[32]*/;
      internal string  t_kupdob/*[36]*/;
      internal string  t_kd_mjesto/*[16]*/;
      internal string  t_kd_adresa/*[16]*/;

      internal string  t_pnb0_zad/*[4]*/;
      internal string  t_pnb1_zad/*[24]*/;
      internal string  t_pnb0_odob/*[4]*/;
      internal string  t_pnb1_odob/*[24]*/;

      internal string  t_vezna_oznaka/*[4]*/;
      internal string  t_svrha_doz/*[40]*/;

      internal char    t_new_old_rn;
      //internal char    t_filler;

      internal string  t_reklamacija/*[36]*/;

      internal decimal t_iznos;
      internal decimal t_dug;
      internal decimal t_pot;
      internal decimal t_devTecaj;
      internal decimal t_devMoney;

      internal string  t_vrsta_transakcije;
      internal string  t_rnpvd;
      internal string  t_kd_vbdi;

      internal bool IsDugovniPromet  { get { return t_vrsta_transakcije == "10"; } }
      internal bool IsPotrazniPromet { get { return t_vrsta_transakcije == "20"; } }

      /// <summary>
      /// if vrsta_transakcije == 10 return t_dug
      /// if vrsta_transakcije == 20 return t_pot
      /// </summary>
      internal decimal TransMoney
      {
         get
         {
            if      (IsDugovniPromet) return t_dug;
            else if(IsPotrazniPromet) return t_pot;
            else
               ZXC.aim_emsg("Vrsta transakcije [" + t_vrsta_transakcije + "] nije niti [10] niti [20]!"); return decimal.Zero;
         }
      }

   };

   #endregion Public Structures

   #region fieldz

   string[] txtLines;

   #endregion fieldz

   #region Propertiez

   private HeadRecordStruct headRecord;
   public  HeadRecordStruct HeadRecord
   {
      get { return headRecord; }
   }

   private TransRecordStruct[] transes;
   public  TransRecordStruct[] Transes
   {
      get { return transes; }
   }

   public string FullPathFileName { get; set; }
   public string FileName         { get; set; }
   public string DirectoryName    { get; set; }

   public int? NumOfIzvodFileLines { get; private set; }
   
   public int NumOfTransLines     { get; private set; }

   private bool zihOK = true, zidsOK = true, zidOK = true, zideOK = true, zieofOK = true, linesOK = true, fileOK = true;

   public  bool BadData
   {
      get
      {
         return (!fileOK || !zihOK || !zidsOK || !zidOK || !zideOK || !zieofOK || !linesOK);
      }
   }

   #region 20.02.2012

/* NAZIV BANKE                                                                                                             VBDI             SWIFT        

/* BANCO POPOLARE CROATIA d.d. Zagreb	         */ public bool IsBancoPopol { get { return this.HeadRecord.bankaVbdi == "4115008";	 } } /* BPCR HR 22 */
/* BANKA BROD d.d. Slavonski Brod	               */ public bool IsBankaBrod  { get { return this.HeadRecord.bankaVbdi == "4124003";	 } } /* BBRD HR 22 */
/* BANKA KOVANICA d.d. Varaždin	               */ public bool IsBankaKovan { get { return this.HeadRecord.bankaVbdi == "4133006";	 } } /* SKOV HR 22 */
/* BANKA SPLITSKO-DALMATINSKA d.d. Split	      */ public bool IsSplitska   { get { return this.HeadRecord.bankaVbdi == "4109006";	 } } /* DALM HR 22 */
/* BKS BANK d.d. Rijeka	                        */ public bool IsBksBank    { get { return this.HeadRecord.bankaVbdi == "2488001";	 } } /* BFKK HR 22 */
/* CENTAR BANKA d.d. Zagreb	                     */ public bool IsCentarBank { get { return this.HeadRecord.bankaVbdi == "2382001";	 } } /* CBZG HR 2X */
/* CROATIA BANKA d.d. Zagreb	                  */ public bool IsCroatiaBan { get { return this.HeadRecord.bankaVbdi == "2485003";	 } } /* CROA HR 2X */
/* ERSTE & STEIERMÄRKISCHE BANK d.d. Rijeka	   */ public bool IsErste      { get { return this.HeadRecord.bankaVbdi == "2402006";	 } } /* ESBC HR 22 */
/* HRVATSKA BANKA ZA OBNOVU I RAZVITAK Zagreb	   */ public bool IsHBOR       { get { return this.HeadRecord.bankaVbdi == "2493003";	 } } /* HKBO HR 2X */
/* HRVATSKA NARODNA BANKA 	                     */ public bool IsHNB        { get { return this.HeadRecord.bankaVbdi == "1001005";	 } } /* NBHR HR 2D */
/* HRVATSKA POŠTANSKA BANKA d.d. Zagreb	         */ public bool IsHPB        { get { return this.HeadRecord.bankaVbdi == "2390001";	 } } /* HPBZ HR 2X */
/* HYPO ALPE-ADRIA-BANK d.d. Zagreb	            */ public bool IsHYPO       { get { return this.HeadRecord.bankaVbdi == "2500009";	 } } /* HAAB HR 22 */
/* IMEX BANKA d.d. Split	                        */ public bool IsIMEX       { get { return this.HeadRecord.bankaVbdi == "2492008";	 } } /* IMXX HR 22 */
/* ISTARSKA KREDITNA BANKA UMAG d.d. Umag	      */ public bool IsIstarskaKr { get { return this.HeadRecord.bankaVbdi == "2380006";	 } } /* ISKB HR 2X */
/* JADRANSKA BANKA d.d. Šibenik	               */ public bool IsJadranska  { get { return this.HeadRecord.bankaVbdi == "2411006";	 } } /* JADR HR 2X */
/* KARLOVAČKA BANKA d.d. Karlovac	               */ public bool IsKarlovacka { get { return this.HeadRecord.bankaVbdi == "2400008";	 } } /* KALC HR 2X */
/* KREDITNA BANKA ZAGREB d.d. Zagreb	            */ public bool IsKreditnaZG { get { return this.HeadRecord.bankaVbdi == "2481000";	 } } /* KREZ HR 2X */
/* MEĐIMURSKA BANKA d.d. Čakovec	               */ public bool IsMedimurska { get { return this.HeadRecord.bankaVbdi == "2392007";	 } } /* MBCK HR 2X */
/* NAVA BANKA d.d. Zagreb	                     */ public bool IsNavaBanka  { get { return this.HeadRecord.bankaVbdi == "2495009";	 } } /* NAVB HR 22 */
/* OTP BANKA HRVATSKA d.d. Zadar	               */ public bool IsOTP        { get { return this.HeadRecord.bankaVbdi == "2407000";	 } } /* OTPV HR 2X */
/* PARTNER BANKA d.d. Zagreb	                  */ public bool IsPartner    { get { return this.HeadRecord.bankaVbdi == "2408002";	 } } /* PAZG HR 2X */
/* PODRAVSKA BANKA d.d. Koprivnica	            */ public bool IsPodravska  { get { return this.HeadRecord.bankaVbdi == "2386002";	 } } /* PDKC HR 2X */
/* PRIMORSKA BANKA d.d. Rijeka	                  */ public bool IsPrimorska  { get { return this.HeadRecord.bankaVbdi == "4132003";	 } } /* SPRM HR 22 */
/* PRIVREDNA BANKA ZAGREB d.d. Zagreb	         */ public bool IsPrivredna  { get { return this.HeadRecord.bankaVbdi == "2340009";	 } } /* PBZG HR 2X */
/* RAIFFEISENBANK AUSTRIA d.d. Zagreb	         */ public bool IsRBA        { get { return this.HeadRecord.bankaVbdi == "2484008";	 } } /* RZBH HR 2X */
/* SAMOBORSKA BANKA d.d. Samobor	               */ public bool IsSamoborska { get { return this.HeadRecord.bankaVbdi == "2403009";	 } } /* SMBR HR 22 */
/* SLATINSKA BANKA d.d. Slatina	               */ public bool IsSlatinska  { get { return this.HeadRecord.bankaVbdi == "2412009";	 } } /* SBSL HR 2X */
/* SOCIETE GENERALE- SPLITSKA BANKA d.d. Split	*/ public bool IsSocieteGen { get { return this.HeadRecord.bankaVbdi == "2330003";	 } } /* SOGE HR 22 */
/* ŠTEDBANKA d.d. Zagreb	                        */ public bool IsStedbanka  { get { return this.HeadRecord.bankaVbdi == "2483005";	 } } /* STED HR 22 */
/* TESLA ŠTEDNA BANKA d.d. Zagreb	               */ public bool IsTeslaSted  { get { return this.HeadRecord.bankaVbdi == "6717002";	 } } /* ASBZ HR 22 */
/* VABA d.d. BANKA Varaždin	                     */ public bool IsVABA       { get { return this.HeadRecord.bankaVbdi == "2489004";	 } } /* VBVZ HR 22 */
/* VENETO BANKA d.d.. Zagreb	                  */ public bool IsVeneto     { get { return this.HeadRecord.bankaVbdi == "2381009";	 } } /* CCBZ HR 2X */
/* VOLKSBANK d.d. Zagreb	                        */ public bool IsVolksbank  { get { return this.HeadRecord.bankaVbdi == "2503007";	 } } /* VBCR HR 22 */
/* ZAGREBAČKA BANKA d.d. Zagreb	               */ public bool IsZABA       { get { return this.HeadRecord.bankaVbdi == "2360000";	 } } /* ZABA HR 2X */

   #endregion 20.02.2012

   //public static DateTime NewIzvodFormatDate = new DateTime(2012, 06, 01);

   public static int OldZapLineLength = 250;
   public static int NewZapLineLength = 1000;

   public static bool IsNew2012Format = true;

   #endregion Propertiez

   #region Constructor

   public ZapIzvod(string _fullPathFileName)
   {
      DirectoryInfo dInfo;
      char          lineID_OLD;
      string        lineID_NEW;

      // 20.04.2012:
      try
      {
         NumOfIzvodFileLines = LoadIzvodFromFile(_fullPathFileName);
      }
      catch(Exception ex)
      {
         System.Windows.Forms.MessageBox.Show("LoadIzvodFromFile PROBLEM:\n\n" + ex.Message);
         fileOK = false; return;
      }

      if(NumOfIzvodFileLines == null) { fileOK = false;  return; }

      FullPathFileName = _fullPathFileName;
      dInfo            = new DirectoryInfo(FullPathFileName);
      FileName         = dInfo.Name;
      DirectoryName    = FullPathFileName.Substring(0, FullPathFileName.Length - (FileName.Length+1));

      this.transes     = new TransRecordStruct[(int)NumOfIzvodFileLines - 4]; 

      NumOfTransLines = 0;

      // 20.04.2012:
      if(txtLines[0].Length == NewZapLineLength) IsNew2012Format = true;
      else                                       IsNew2012Format = false;

      foreach(string line in txtLines)
      {
         if(BadData) break;

         if(IsNew2012Format)
         {
            lineID_NEW = GetLineID_NEW(line);

            switch(lineID_NEW)
            {
               // zih - ZapIzvodHead 
               case "900": zihOK   = Take_900_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzv2012_900_slog_paketa_LABELA_PartLenghts));                              break;
               // zids - ZapIzvodDateStart 
               case "903": zidsOK  = Take_903_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzv2012_903_vodeći_slog_grupe_podataka_PartLenghts));                      break;
               // zid - ZapIzvodData 
               case "905": zidOK =   Take_905_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzv2012_905_slog_pojedinacne_transakcije_PartLenghts), NumOfTransLines++); break;
               // zide - ZapIzvodDateEnd 
               case "907": zideOK  = Take_907_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzv2012_907_zaključni_slog_grupe_podataka_PartLenghts));                   break;
               //zieof - ZapIzvodDateEOF 
               case "909": zieofOK = Take_909_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzv2012_909_zakljucni_slog_paketa_PartLenghts));                           break;
               //fuse
               case "999":                                                                                                                                                    break;
            }
         }
         else
         {
            lineID_OLD = GetLineID_OLD(line);

            switch(lineID_OLD)
            {
               // zih - ZapIzvodHead 
               case '0': zihOK   = Take_ZIH_Data  (VvImpExp.SeparateStringsFromImportLine(line, ZapIzvodHead_PartLenghts));                    break;
               // zids - ZapIzvodDateStart 
               case '3': zidsOK  = Take_ZIDS_Data (VvImpExp.SeparateStringsFromImportLine(line, ZapIzvodDateStart_PartLenghts));               break;
               // zid - ZapIzvodData 
               case '5': zidOK =   Take_ZID_Data  (VvImpExp.SeparateStringsFromImportLine(line, ZapIzvodData_PartLenghts), NumOfTransLines++); break;
               // zide - ZapIzvodDateEnd 
               case '7': zideOK  = Take_ZIDE_Data (VvImpExp.SeparateStringsFromImportLine(line, ZapIzvodDateEnd_PartLenghts));                 break;
               //zieof - ZapIzvodDateEOF 
               case '9': zieofOK = Take_ZIEOF_Data(VvImpExp.SeparateStringsFromImportLine(line, ZapIzvodDateEOF_PartLenghts));                 break;
            }
         }
      }

      // Ako u Izvodu neko ima dva ziroracuna onda ova logika linesOK=false je KRIVA 
      //if(NumOfIzvodFileLines < 5 || NumOfIzvodFileLines - NumOfTransLines != 4) linesOK = false;
      if(NumOfIzvodFileLines < 5 /*|| NumOfIzvodFileLines - NumOfTransLines != 4*/) linesOK = false;
      else                                                                          linesOK = true;
   }

   #endregion Constructor

   #region Methods

   #region Take_IZVOD_Data OLD

   private bool Take_ZIH_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*  8 IZ0D2000*/
            case 8: headRecord.iz_date = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_ZIDS_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*  1 IZ3RNPRI */
            case 1: headRecord.iz_ziro_rnpri = field; break;

            /* 12 IZ3VBDI  */
            case 12: headRecord.bankaVbdi = field;
               headRecord.iz_ziro_racun = headRecord.bankaVbdi + "-" + headRecord.iz_ziro_rnpri; break;

            /*  2 IZ3NAPRI */
            case 2: headRecord.iz_firma = field; break;

            /*  7 IZ3NAZAP */
            case 7: headRecord.bankaName = field; break;

            /*  3 IZ3RBIZV */
            case 3: headRecord.iz_br = ZXC.ValOrZero_UInt(field); break;

         }

         colNum++;
      }

      return OK;
   }

   private bool Take_ZID_Data(string[] inFileData, int serial)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      transes[serial].t_br     = headRecord.iz_br;
      transes[serial].t_serial = serial;
      transes[serial].t_rbr    = (serial + 1).ToString() + ".";

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*  1 IZ5OZTRA  */
            // "10" - Duguje, "20" - Potrazuje 
            case 1: transes[serial].t_vrsta_transakcije = field; break;

            /*  2 IZ5RNPVD  */
            case 2: transes[serial].t_rnpvd = field; break;

            /*  3 IZ5NAZVD  */
            case 3: transes[serial].t_kupdob = field; break;

            /*  4 IZ5MJEVD  */
            case 4: transes[serial].t_kd_mjesto = field; break;

            /*  5 IZ5IZNOS  */
            case 5: transes[serial].t_iznos = ZXC.ValOrZero_Decimal(field, 2) / 100.00M;

               if     (transes[serial].t_vrsta_transakcije == "10") transes[serial].t_dug = transes[serial].t_iznos;
               else if(transes[serial].t_vrsta_transakcije == "20") transes[serial].t_pot = transes[serial].t_iznos;
               break;

            /*  6 IZ5PNBZA  */
            case 6:
               if(field.Length < 2)
               {
                  transes[serial].t_pnb0_zad = field;
                  transes[serial].t_pnb1_zad = "";
               }
               else
               {
                  transes[serial].t_pnb0_zad = field.Substring(0, 2);
                  transes[serial].t_pnb1_zad = field.Substring(2);
               }
               break;

            /*  7 IZ5PNBOD  */
            case 7:
               if(field.Length < 2)
               {
                  transes[serial].t_pnb0_odob= field;
                  transes[serial].t_pnb1_odob= "";
               }
               else
               {
                  transes[serial].t_pnb0_odob= field.Substring(0, 2);
                  transes[serial].t_pnb1_odob= field.Substring(2);
               }
               break;

            /*  8 IZ5VEZOZ  */
            case 8: transes[serial].t_vezna_oznaka = field; break;

            /*  9 IZ5SVDOZ  */
            case 9: transes[serial].t_svrha_doz = field; break;

            /* 10 IZ5KONSTR */
            // (N-nova / S-stara konstrukcija)
            case 10: transes[serial].t_new_old_rn = field[0]; break;

            /* 11 IZ5REKLA  */
            case 11: transes[serial].t_reklamacija = field; break;

            /* 12 IZ5VBDI   */
            case 12: transes[serial].t_kd_vbdi = field;

               if(transes[serial].t_new_old_rn == 'S') // 'stara' konstrukcija racuna 
               {
                  //strNCpy(ziroRnpvd, zid->IZ5RNPVD, 18);  // ipak 18 za STARI ZIRO
                  //strNCpy(ziroVbdi, zid->IZ5VBDI, 7);
                  //sprintf(sfNITrans[sfLine].t_kd_ziro, "%5.5s-%3.3s-%6.6s",
                  //     ziroRnpvd, ziroRnpvd + 5, ziroRnpvd + 12);

                  ZXC.aim_emsg("{0}\n\n 'Stara' konstrukcija racuna nepodrzana!", ZXC.GetMethodNameDaStack());

                  transes[serial].t_kd_ziro = "abcdefg!1234567890";
               }
               else // 'nova' konstrukcija racuna 
               {
                  // Saznaj koje jos banke osim Privredne i Erste u ZapIzvod-u (Magnetni Izvadak za Korisnike) 
                  // koriste fullZiro (npr Zagrebacka NE koristi) 
                  bool isFullZiro = IsPrivredna || IsErste;

                  string ziroVbdi = transes[serial].t_kd_vbdi;
                  string ziroRnpvd;

                  // Privedna sprema full ziro na poziciji IZ5RNPVD (npr [23400091000000013 ])
                  // a ostale rade bec vbdi-ja                      (npr [1700052620        ])
                  if(isFullZiro) ziroRnpvd = transes[serial].t_rnpvd.Substring(7);
                  else           ziroRnpvd = transes[serial].t_rnpvd;

                  transes[serial].t_kd_ziro = ziroVbdi + "-" + ziroRnpvd;
               }
               break;

            /* 15 IZ5DATVAL */
            case 15: transes[serial].t_valuta = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;

         }

         colNum++;
      }

      return OK;
   }

   private bool Take_ZIDE_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*  5 IZ7PRSAL */
            case 5: headRecord.iz_prev_saldo = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /*  6 IZ7KDUGU */
            case 6: headRecord.iz_uk_dug = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /*  7 IZ7KPOTR */
            case 7: headRecord.iz_uk_pot = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /*  8 IZ7KOSAL */
            case 8: headRecord.iz_saldo = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /* 13 IZ7PPPOS */
            // Predznak poeetnog stanja (+ ili -)
            case 13: headRecord.iz_prev_sign = field[0];
               if(headRecord.iz_prev_sign == '-') headRecord.iz_prev_saldo *= -1.00M; break;

            /* 14 IZ7PRNOS */
            // Predznak novog stanja (+ ili -)
            case 14: headRecord.iz_new_sign = field[0];
               if(headRecord.iz_new_sign == '-') headRecord.iz_saldo *= -1.00M; break;

         }

         colNum++;
      }

      return OK;
   }

   private bool Take_ZIEOF_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*  6 IZ9UKSLG*/
            case 6:
               int IZ9UKSLG = ZXC.ValOrZero_Int(field);
               if(IZ9UKSLG != this.NumOfIzvodFileLines) return false;
               break;
         }

         colNum++;
      }

      return OK;
   }

   #endregion Take_IZVOD_Data

   #region Take_IZVOD_Data NEW

   private bool Take_900_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

//SADRŽAJ SLOGA ZA TIP SLOGA „900“ – slog paketa – „LABELA“
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ900DVBDIPOS      VBDI – pošiljatelja (banke)             M N   1   7     7           ,*/ //Upisuje se VBDI banke
/*2.  IZ900NAZBAN        Naziv banke                             M C   8   57    50          ,*/ //Upisuje se naziv banke
/*3.  IZ9000OIBBNK       OIB banke                               M N   58  68    11          ,*/ //Upisuje se OIB banke
/*4.  IZ900VRIZ          Vrsta izvatka                           O N   69  72    4           ,*/ //Upisuje se 1000 i označava duljinu sloga odnosno poziciju na koji se nalazi tip sloga (IZ900TIPSL)
/*5.  IZ900DATUM         Datum obrade – tekući datumGGGGMMDD     M C   73  80    8           ,*/ //Upisuje se datum kreiranja izvatka na elektronskom mediju u formatu GGGGMMDD 
/*6.  IZ900REZ2          Rezerva                                 O C   81  997   917         ,*/ //
/*7.  IZ900TIPSL         Tip sloga                               M N   998 1000  3           ,*/ //
      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case 1: headRecord.bankaVbdi = field; break;
            case 2: headRecord.bankaName = field; break;
            case 3: headRecord.bankaOIB  = field; break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_903_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

//SADRŽAJ SLOGA ZA TIP SLOGA „903“ – vodeći slog grupe podataka
//
//RBR NAZIV POLJA        OPIS POLJA             Mandatory/Optional TIP OD  DO   BROJ MJESTA POJAŠNJENJE
//
/*1.  IZ903VBDI          Vodeći broj banke                       M N   1   7    7           ,*/  //
/*2.  IZ903BIC           BIC - Identifikacijska šifra banke      O C   8   18   11          ,*/  //
/*3.  IZ903RACUN         Transakcijski račun klijenta            M C   19  39   21          ,*/  //
/*4.  IZ903VLRN          Valuta transakcijskog računa            M C   40  42   3           ,*/  //
/*5.  IZ903NAZKLI        Naziv klijenta                          M C   43  112  70          ,*/  //
/*6.  IZ903SJEDKLI       Sjedište klijenta                       M C   113 147  35          ,*/  //
/*7.  IZ903MB            Matični broj                            O N   148 155  8           ,*/  //
/*8.  IZ903OIBKLI        OIB klijenta                            O N   156 166  11          ,*/  //
/*9.  IZ903RBIZV         Redni broj izvatka                      M N   167 169  3           ,*/  //
/*10. IZ903PODBR         Podbroj izvatka                         M N   170 172  3           ,*/  //
/*11. IZ903DATUM         Datum izvatka                           M N   173 180  8           ,*/  //Prikazuje se u formatu GGGGMMDD
/*12. IZ903BRGRU         Redni broj grupe paketa                 M N   181 184  4           ,*/  //
/*13. IZ903VRIZ          Vrsta izvatka                           M N   185 188  4           ,*/  //Upisuje se oznaka 1000
/*14. IZ903REZ           Rezerva                                 O C   189 997  809         ,*/  //
/*15. IZ903TIPSL         Tip sloga                               M N   998 1000 3           ,*/  //

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case  3: headRecord.iz_ziro_rnpri = field;
                     headRecord.iz_ziro_racun = headRecord.bankaVbdi + "-" + headRecord.iz_ziro_rnpri; break;
            case  4: headRecord.valutaName = field; break;
            case  5: headRecord.iz_firma   = field; break;
            case  8: headRecord.firmaOIB   = field; break;
            case  9: headRecord.iz_br = ZXC.ValOrZero_UInt(field); break;
            case 11: headRecord.iz_date = ZXC.ValOr_01010001_DateTime_Import_yyyyMMdd_Format(field); break;



         }

         colNum++;
      }

      return OK;
   }

   private bool Take_905_Data(string[] inFileData, int serial) 
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      transes[serial].t_br     = headRecord.iz_br;
      transes[serial].t_serial = serial;
      transes[serial].t_rbr    = (serial + 1).ToString() + ".";

//kao oznaka vrste transakcije čime se imatelju transakcijskog računa daje objašnjenje oznake vrste transakcije - izvršenje naloga „NA TERET“ transakcijskog računa korisnika 
//(DUGUJE) označeno je u slogu pojedinačne transakcije oznakom transakcije  „10“. Tada se u polju „IZ905RNPRPL“ nalazi transakcijski račun primatelja- izvršenje naloga 
//„U KORIST“ transakcijskog računa korisnika (POTRAŽUJE) označeno je u slogu pojedinačne transakcije oznakom transakcije „20“. Tada se u polju „IZ905RNPRPL“ nalazi transakcijski račun platitelja

/*1.  IZ905OZTRA         Oznaka transakcije                      M N   1   2     2           ,*/ //- upisuje se podatak “10“ ili „20“ qukatz vidi dva reda gore pojasnjenje
/*2.  IZ905RNPRPL        Račun primatelja-platitelja             M C   3   36    34          ,*/ //
/*3.  IZ905NAZPRPL       Naziv primatelja-platitelja             M C   37  106   70          ,*/ //
/*4.  IZ905ADRPRPL       Adresa primatelja-platitelja            O C   107 141   35          ,*/ //
/*5.  IZ905SJPRPL        Sjedište primatelja-platitelja          O C   142 176   35          ,*/ //
/*6.  IZ905DATVAL        Datum valute (GGGGMMDD)                 M C   177 184   8           ,*/ //Prikazuje se u formatu GGGGMMDD
/*7.  IZ905DATIZVR       Datum izvršenja (GGGGMMDD)              M C   185 192   8           ,*/ //Prikazuje se u formatu GGGGMMDD
/*8.  IZ905VLPL          Valuta pokrića                          O C   193 195   3           ,*/ //
/*9.  IZ905TECAJ         Tečaj/koeficijent                       O N   196 210   15          ,*/ //9+6 znakova – 6 su decimale
/*10. IZ905PREDZN        Predznak „+“ a u slučaju storna „-„     O C   211 211   1           ,*/ //
/*11. IZ905IZNOSPPVALUTE Iznos u valuti pokrića                  O N   212 226   15          ,*/ //
/*12. IZ905PREDZN        Predznak „+“ a u slučaju storna „-„     M C   227 227   1           ,*/ //
/*13. IZ905IZNOS         Iznos                                   M N   228 242   15          ,*/ //Iznos u valuti transakcijskog računa iz polja IZ903VLRN
/*14. IZ905PNBPL         Poziv na broj platitelja                O C   243 268   26          ,*/ //
/*15. IZ905PNBPR         Poziv na broj primatelja                O C   269 294   26          ,*/ //
/*16. IZ905SIFNAM        Šifra namjene                           O N   295 298   4           ,*/ //Prikazuje se šifra namjene transakcije šrema ISO standardu 20022
/*17. IZ905OPISPL        Opis plaćanja                           O C   299 438   140         ,*/ //
/*18. IZ905IDTRFINA      Identifik transak–inicirano u FINI      O C   439 480   42          ,*/ //Upisuje se identifikator transakcije inicirane u FINI - broj za reklamaciju
/*19. IZ905IDTRBAN       Identifik transak–inicirano izvan FINE  M C   481 515   35          ,*/ //Upisuje se identifikator transakcije banke 
/*20. IZ905REZ2          Rezerva                                 O N   516 997   482         ,*/ //
/*21. IZ905TIPSL         Tip sloga                               M N   998 1000  3           ,*/ //

      foreach(string field in inFileData)
      {
         //            2 2 7      10
         // 2012 IBAN: HRXX23600001234567890
         switch(colNum)
         {
            case  1: transes[serial].t_vrsta_transakcije = field; break;
            case  2: transes[serial].t_rnpvd             = field;

               bool isIBAN = field.StartsWith("HR");

               // Privedna sprema full ziro na poziciji IZ5RNPVD (npr [23400091000000013 ])
               // a ostale rade bec vbdi-ja                      (npr [1700052620        ])
               // novo! prije je bilo t_kd_ziro = t_kd_vbdi + "-" + t_rnpvd, a sada je (valjda?) direktno

                                                      // HRXX23600001234567890
               if(isIBAN) transes[serial].t_kd_ziro = field.Substring(4, 7) + "-" + field.Substring(4 + 7);
               
               //else       transes[serial].t_kd_ziro = headRecord.bankaVbdi  + "-" + field; staro, prije 1.6.2012 
               // 23600001234567890
               else
               {
                  string fullZiro = field.Replace("-", "");
                  if(field.Length > 10) transes[serial].t_kd_ziro = fullZiro.Substring(0, 7) + "-" + fullZiro.Substring(7); 
                  else                  transes[serial].t_kd_ziro = headRecord.bankaVbdi     + "-" + fullZiro             ;
               }
               break;
            case  3: transes[serial].t_kupdob            = field; break;
            case  4: transes[serial].t_kd_adresa         = field; break;
            case  5: transes[serial].t_kd_mjesto         = field; break;
            case  6: transes[serial].t_valuta            = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;
            case  9: transes[serial].t_devTecaj          = ZXC.ValOrZero_Decimal(field, 6) / 100.00M; break; // TODO: !! provjeri jel ovo sljaka 
            case 11: transes[serial].t_devMoney          = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break; // TODO: !! provjeri jel ovo sljaka 
            case 13: transes[serial].t_iznos             = ZXC.ValOrZero_Decimal(field, 2) / 100.00M;
               if     (transes[serial].t_vrsta_transakcije == "10") transes[serial].t_dug = transes[serial].t_iznos;
               else if(transes[serial].t_vrsta_transakcije == "20") transes[serial].t_pot = transes[serial].t_iznos;
               break;
            case 14:
               if(field.Length < 2)
               {
                  transes[serial].t_pnb0_zad = field;
                  transes[serial].t_pnb1_zad = "";
               }
               else
               {
                  transes[serial].t_pnb0_zad = field.Substring(0, 2);
                  transes[serial].t_pnb1_zad = field.Substring(2);
               }
               break;
            case 15:

               // 04.07.2012: 
               string field_WO_HR = field.Replace("HR", "");

               if(field_WO_HR.Length < 2)
               {
                  transes[serial].t_pnb0_odob = field_WO_HR;
                  transes[serial].t_pnb1_odob= "";
               }
               else
               {
                  transes[serial].t_pnb0_odob = field_WO_HR.Substring(0, 2);
                  transes[serial].t_pnb1_odob = field_WO_HR.Substring(2);
               }
               break;

            case 17: transes[serial].t_svrha_doz = field; break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_907_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case  8: headRecord.iz_prev_sign = field[0];                                    break;
            case  9: headRecord.iz_prev_saldo = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; 
                     if(headRecord.iz_prev_sign == '-') headRecord.iz_prev_saldo *= -1.00M; break;
            case 18: headRecord.iz_uk_dug     = ZXC.ValOrZero_Decimal(field, 2) / 100.00M;  break;
            case 20: headRecord.iz_uk_pot     = ZXC.ValOrZero_Decimal(field, 2) / 100.00M;  break;
            case 21: headRecord.iz_new_sign = field[0];                                     break;
            case 22: headRecord.iz_saldo      = ZXC.ValOrZero_Decimal(field, 2) / 100.00M;  
                     if(headRecord.iz_new_sign == '-') headRecord.iz_saldo *= -1.00M;       break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_909_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            case 3:
               int IZ9UKSLG = ZXC.ValOrZero_Int(field);

               if(IZ9UKSLG != this.NumOfIzvodFileLines)
               // 04.06.2012: neke banke drugacije shvacasju pojam 'IZ909UKSLG Ukupan broj slog/paket'
               {
                  if(this.NumOfIzvodFileLines - IZ9UKSLG > 1)
                  return false;
               }

               break;
         }

         colNum++;
      }

      return OK;
   }

   #endregion Take_IZVOD_Data

   private int? LoadIzvodFromFile(string fName)
   {
      txtLines = VvImpExp.GetFileLinesAsStringArray(fName);

      if(txtLines == null) return null;
      else                 return txtLines.Length;
   }

   private char GetLineID_OLD(string line)
   {
      return ZXC.GetStringsLastChar(line);
   }

   private string GetLineID_NEW(string line)
   {
      return ZXC.GetStringsLast3Char(line);
   }

   public static uint GetIzvodNumFromFile(string fName)
   {
      uint   izvodNum=0, i=0;
      string linePart;

      if(!File.Exists(fName))
      {
         VvSQL.ReportGenericError("OPEN FILE", string.Format("Ne mogu otvoriti datoteku!\n\n\"{0}\"", Environment.CurrentDirectory + "\\" + fName), System.Windows.Forms.MessageBoxButtons.OK);
         return 0;
      }

      // 27.4.2011: 
    //using(StreamReader sr = File.OpenText(fName))
      using(StreamReader sr = new StreamReader(fName, Encoding.GetEncoding(1250)))
      {
         string line;

         while((line = sr.ReadLine()) != null)
         {
            if(line.Length != /*250*/OldZapLineLength &&
               line.Length !=        NewZapLineLength) return 0; // file nije izvod 
            if(++i == 2) break;
         }
         sr.Close();

         //21.97.2912: 
         if(line == null) return 0;

         if(line.Length == NewZapLineLength) IsNew2012Format = true ;
         else                                IsNew2012Format = false;

         if(IsNew2012Format)
         {
            linePart = line.Substring(166, 3);
         }
         else
         {
            //  3 IZ3RBIZV     3,      //   	 54    56  +Redni broj izvatka (popunjeno sa vodećim nulama)
            linePart = line.Substring(53, 3);
         }

      }


      if(i == 2) uint.TryParse(linePart, out izvodNum); // ako nije 2 znaci da je file imao manje od 2 reda...

      return izvodNum;
   }

   public static string GetIzvodFileName_ContainingIzvodNumWeWant(string directoryPath, uint izvodNumWeWant)
   {
      string   izvodFileName_ContainingWantedIzvodNum = "";

      uint     izvodNumInFile;
      string[] izvodFNames    = VvImpExp.GetAllFileNamesInDirectory(directoryPath);

      if(izvodFNames == null) return null;

      foreach(string fName in izvodFNames.OrderByDescending(f => f))
      {
         izvodNumInFile = GetIzvodNumFromFile(fName);

         if(izvodNumInFile == izvodNumWeWant)
         {
            izvodFileName_ContainingWantedIzvodNum = fName;
            break;
         }
      }

      return izvodFileName_ContainingWantedIzvodNum;
   }

   #endregion Methods

};

public sealed class ZbrojniNalogZaPrijenos
{
   #region Fields Arrays NEW

   VvImpExp.ImpExpField[] LabelaFields_300 = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("S300DATSL"   ,   1,   8), 
      new VvImpExp.ImpExpField("S300VRSTNAL" ,   2,   1), 
      new VvImpExp.ImpExpField("S300IZDOK"   ,   3,   3),
      // 07.01.2015:                         
    //new VvImpExp.ImpExpField("S300REZERVA" ,   4, 985), UBACIO 5 NOVIH KOJI SU PRIJE BILI NEBITNI pa kao takvi sadrzani u REZERVI
      new VvImpExp.ImpExpField("S300NACIZVR" ,   4,   1), 
      new VvImpExp.ImpExpField("S300OIBPOS"  ,   5,  11), 
      new VvImpExp.ImpExpField("S300MBRPOS"  ,   6,  11), 
      new VvImpExp.ImpExpField("S300INSIFPOS",   7,  11), 
      new VvImpExp.ImpExpField("S300OIBUPL"  ,   8,  11), 
      new VvImpExp.ImpExpField("S300REZERVA" ,   9, 940), 
      new VvImpExp.ImpExpField("S300TIPSLOG" ,  10,   3), 
   };


   VvImpExp.ImpExpField[] LeadFields_301 = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("S301IBANPLAT",  1,  21),
      new VvImpExp.ImpExpField("S301VALPL"   ,  2,   3), 
      new VvImpExp.ImpExpField("S301RNNAK"   ,  3,  21),
      new VvImpExp.ImpExpField("S301VALNAK"  ,  4,   3),
      new VvImpExp.ImpExpField("S301BRNALUK" ,  5,   5),
      new VvImpExp.ImpExpField("S301IZNNALUK",  6,  20),
      new VvImpExp.ImpExpField("S301DATIZVR" ,  7,   8),
      new VvImpExp.ImpExpField("S301REZERVA" ,  8, 916),
      new VvImpExp.ImpExpField("S301TIPSLOG" ,  9,   3),
   };

   VvImpExp.ImpExpField[] TransFields_309 = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("S309IBANRNPRIM" , 01,  34), 
      new VvImpExp.ImpExpField("S309NAZIVPRIM"  , 02,  70), 
      new VvImpExp.ImpExpField("S309ADRPRIM"    , 03,  35), 
      new VvImpExp.ImpExpField("S309SJEDPRIM"   , 04,  35), 
      new VvImpExp.ImpExpField("S309SFZEMPRIM"  , 05,   3), 
      new VvImpExp.ImpExpField("S309BRMODPLAT"  , 06,   4), 
      new VvImpExp.ImpExpField("S309PNBPLAT"    , 07,  22), 
      new VvImpExp.ImpExpField("S309SIFNAM"     , 08,   4), 
      new VvImpExp.ImpExpField("S309OPISPL"     , 09, 140), 
      new VvImpExp.ImpExpField("S309IZN"        , 10,  15), 
      new VvImpExp.ImpExpField("S309BRMODPRIM"  , 11,   4), 
      new VvImpExp.ImpExpField("S309PNBPRIM"    , 12,  22), 
      new VvImpExp.ImpExpField("S309BICBANPRIM" , 13,  11), 
      new VvImpExp.ImpExpField("S309NAZBANPRIM" , 14,  70), 
      new VvImpExp.ImpExpField("S309ADRBNPRIM"  , 15,  35), 
      new VvImpExp.ImpExpField("S309SJEDBNPRIM" , 16,  35), 
      new VvImpExp.ImpExpField("S309SFZEMBNPRIM", 17,   3), 
      new VvImpExp.ImpExpField("S309VRSTAPRIM"  , 18,   1),
      new VvImpExp.ImpExpField("S309VALPOKR"    , 19,   3), 
      new VvImpExp.ImpExpField("S309TROSOP"     , 20,   1), 
      new VvImpExp.ImpExpField("S309OZNHITN"    , 21,   1), 
      // 07.01.2015: 
    //new VvImpExp.ImpExpField("S309REZERVA"    , 22, 449), UBACIO 1 NOVog KOJI je PRIJE BIo NEBITaN pa kao takav sadrzan u REZERVI 
      new VvImpExp.ImpExpField("S309SIFPRIM"    , 22,   3), 
      new VvImpExp.ImpExpField("S309REZERVA"    , 23, 446), 
      new VvImpExp.ImpExpField("S309TIPSLOG"    , 24,   3), 
   };

   VvImpExp.ImpExpField[] ClosingFields_399 = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("S399REZERVA" , 01, 997), 
      new VvImpExp.ImpExpField("S399PTIPSLOG", 02,   3), 
   };

   #endregion Fields Arrays NEW

   #region Fields Arrays OLD

   VvImpExp.ImpExpField[] LabelaFields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("ZN0OPBRO",   1,   1,   5), 
      new VvImpExp.ImpExpField("ZN0IDKOR",   2,   6,  13), 
      new VvImpExp.ImpExpField("ZN0NAZKO",   3,  19,  50),
      new VvImpExp.ImpExpField("ZN0MJEST",   4,  69,  10), 
      new VvImpExp.ImpExpField("ZN0DATUM",   5,  79,   6), 
      new VvImpExp.ImpExpField("ZN0BRPAK",   6,  85,   3), 
      new VvImpExp.ImpExpField("ZN0ADDRKO",  7,  88,   7),
      new VvImpExp.ImpExpField("ZN0KODSTR",  8,  95,   1), 
      new VvImpExp.ImpExpField("ZN0TIPDP",   9,  96,   2), 
      new VvImpExp.ImpExpField("ZN0IZDOK",  10,  98,   3), 
      new VvImpExp.ImpExpField("ZN0VBDI",   11, 101,   7), 
      new VvImpExp.ImpExpField("ZN0NIDKOR", 12, 108,  18), 
      new VvImpExp.ImpExpField("ZN0REZER",  13, 126, 116),
      new VvImpExp.ImpExpField("ZN0D2000",  14, 242,   8), 
      new VvImpExp.ImpExpField("ZN0TIPSL",  15, 250,   1) 
   };


   VvImpExp.ImpExpField[] LeadFields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("ZN9OPBRO",  1,   1,   5),
      new VvImpExp.ImpExpField("ZN9IDNAL",  2,   6,  13), 
      new VvImpExp.ImpExpField("ZN9NAZIV",  3,  19,  50),
      new VvImpExp.ImpExpField("ZN9MJEST",  4,  69,  10),
      new VvImpExp.ImpExpField("ZN9SVOTA",  5,  79,  15),
      new VvImpExp.ImpExpField("ZN9BRNAL",  6,  94,   5),
      new VvImpExp.ImpExpField("ZN9VBDI",   7,  99,   7),
      new VvImpExp.ImpExpField("ZN9NOID",   8, 106,  18),
      new VvImpExp.ImpExpField("ZN9DATUM",  9, 124,   8),
      new VvImpExp.ImpExpField("ZN9REZER", 10, 132, 118),
      new VvImpExp.ImpExpField("ZN9TIPSL", 11, 250,   1)
   };

   VvImpExp.ImpExpField[] TransFields = new VvImpExp.ImpExpField[]
   {
      new VvImpExp.ImpExpField("ZN1OPZAP", 01,   1,   5), 
      new VvImpExp.ImpExpField("ZN1IDNAL", 02,   6,  13), 
      new VvImpExp.ImpExpField("ZN1NAZIV", 03,  19,  50), 
      new VvImpExp.ImpExpField("ZN1MJEST", 04,  69,  10), 
      new VvImpExp.ImpExpField("ZN1MOBRZ", 05,  79,   2), 
      new VvImpExp.ImpExpField("ZN1PNBZA", 06,  81,  22), 
      new VvImpExp.ImpExpField("ZN1SVRHA", 07, 103,  36), 
      new VvImpExp.ImpExpField("ZN1VEZOZ", 08, 139,   2), 
      new VvImpExp.ImpExpField("ZN1DRZAD", 09, 141,   2), 
      new VvImpExp.ImpExpField("ZN1DRODO", 10, 143,   2), 
      new VvImpExp.ImpExpField("ZN1SVOTA", 11, 145,  13), 
      new VvImpExp.ImpExpField("ZN1MOBRO", 12, 158,   2), 
      new VvImpExp.ImpExpField("ZN1PNBOD", 13, 160,  22), 
      new VvImpExp.ImpExpField("ZN1VBDI",  14, 182,   7), 
      new VvImpExp.ImpExpField("ZN1NOID",  15, 189,  18), 
      new VvImpExp.ImpExpField("ZN1OZPRI", 16, 207,   2), 
      new VvImpExp.ImpExpField("ZN1REZER", 17, 209,  41), 
      new VvImpExp.ImpExpField("ZN1TIPSL", 18, 250,   1) 
   };

   #endregion Fields Arrays OLD

   #region Propertiz

   public Dictionary<string, VvImpExp.ImpExpField> LabelaDict { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> LeadDict   { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> TransDict  { get; set; }

   public Dictionary<string, VvImpExp.ImpExpField> Dict300 { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> Dict301 { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> Dict309 { get; set; }
   public Dictionary<string, VvImpExp.ImpExpField> Dict399 { get; set; }

   // 28.12.2012: 
 //public static bool IsNewZNP2013Format = false;
   public static bool IsNewZNP2013Format = true;

   public const int OldLineFixedLength = 250;
   public const int NewLineFixedLength = 1000;

   string[] txtLines;

   public int? NumOfZnpFileLines { get; private set; }

   public int NumOfTransLinesSoFar { get; private set; }

   private decimal MoneySUM { get; set; }

   private bool labelaOK = true, leadOK = true, transOK = true, linesOK = true, fileOK = true;

   public  bool BadData
   {
      get
      {
         return (!fileOK || !labelaOK || !leadOK || !transOK || !linesOK);
      }
   }

   private ZapIzvod.HeadRecordStruct headRecord;
   public  ZapIzvod.HeadRecordStruct HeadRecord
   {
      get { return headRecord; }
   }

   private ZapIzvod.TransRecordStruct[] transes;
   public  ZapIzvod.TransRecordStruct[] Transes
   {
      get { return transes; }
   }

   public string FullPathFileName { get; set; }
   public string FileName         { get; set; }
   public string DirectoryName    { get; set; }

   public DateTime ZnpDate { get; private set; }

   private List<VirmanStruct> VirmanList;

   #endregion Propertiz

   #region Constructors

   /// <summary>
   /// Constructor for WRITE to ZNP
   /// </summary>
   public ZbrojniNalogZaPrijenos(string _fullPathFileName, string _fileName, DateTime _znpDate, List<VirmanStruct> _virmanList, ZXC.ZNP_Kind ZNPkind)
   {
      this.FullPathFileName = _fullPathFileName;
      this.FileName         = _fileName;
      this.ZnpDate          = _znpDate;
      this.VirmanList       = _virmanList;

    //if(ZnpDate < NewIzvodFormatDate) IsNewZNP2012Format = false; bug. XP Win krivo vidi adtum?
      // komentirao 28.12.2012: 
      //if(/*ZnpDate < NewIzvodFormatDate*/true) IsNewZNP2013Format = false; // od 1.1.2013 ovo promjeni 
      //else                                     IsNewZNP2013Format = true ;

      this.NumOfTransLinesSoFar = 0;
      this.MoneySUM             = 0.00M;

      if(IsNewZNP2013Format)
      {
         Dict300 = VvImpExp.CreateDictionary(LabelaFields_300 , "300", NewLineFixedLength);
         Dict301 = VvImpExp.CreateDictionary(LeadFields_301   , "301", NewLineFixedLength);
         Dict309 = VvImpExp.CreateDictionary(TransFields_309  , "309", NewLineFixedLength);
         Dict399 = VvImpExp.CreateDictionary(ClosingFields_399, "399", NewLineFixedLength);

         SetLabelaAndLeadFieldsValues_2013(ZnpDate, ZNPkind);
      }
      else
      {
         LabelaDict = VvImpExp.CreateDictionary(LabelaFields, "LABELA", OldLineFixedLength);
         LeadDict   = VvImpExp.CreateDictionary(LeadFields,   "LEAD"  , OldLineFixedLength);
         TransDict  = VvImpExp.CreateDictionary(TransFields,  "TRANS" , OldLineFixedLength);

         SetLabelaAndLeadFieldsValues_OLD(ZnpDate);
      }

   }

   /// <summary>
   /// Constructor for READ from ZNP
   /// TODO new format in 2012!!!
   /// </summary>
   public ZbrojniNalogZaPrijenos(string _fullPathFileName) // TODO: new format in 2012!!! 
   {
      DirectoryInfo dInfo;
      char          lineID;

      NumOfZnpFileLines = LoadZnpFromFile(_fullPathFileName);

      if(NumOfZnpFileLines == null) { fileOK = false;  return; }

      FullPathFileName = _fullPathFileName;
      dInfo            = new DirectoryInfo(FullPathFileName);
      FileName         = dInfo.Name;
      DirectoryName    = FullPathFileName.Substring(0, FullPathFileName.Length - (FileName.Length+1));

      this.transes     = new ZapIzvod.TransRecordStruct[(int)NumOfZnpFileLines - 2]; 

      NumOfTransLinesSoFar = 0;

      int[] LabelaPartLengths = LabelaFields.Select(el => el.FldLength).ToArray();
      int[] LeadPartLengths   = LeadFields  .Select(el => el.FldLength).ToArray();
      int[] TransPartLengths  = TransFields .Select(el => el.FldLength).ToArray();

      foreach(string line in txtLines)
      {
         if(BadData) break;

         lineID = GetLineID(line);

         switch(lineID)
         {
            // LABELA 
            case '0': labelaOK = Take_LABELA_Data(VvImpExp.SeparateStringsFromImportLine(line, LabelaPartLengths)); break;
            // LEAD 
            case '9': leadOK   = Take_LEAD_Data  (VvImpExp.SeparateStringsFromImportLine(line, LeadPartLengths)); break;
            // TRANS 
            case '1': transOK  = Take_TRANS_Data (VvImpExp.SeparateStringsFromImportLine(line, TransPartLengths), NumOfTransLinesSoFar++); break;

            default : VvSQL.ReportGenericError("LOAD ZNP", string.Format("Tip sloga: {0} nepoznat!\n\n", lineID), System.Windows.Forms.MessageBoxButtons.OK); break;
         }
      }

      if(NumOfZnpFileLines < 3 || NumOfZnpFileLines - NumOfTransLinesSoFar != 2) linesOK = false;
      else                                                                       linesOK = true;
   }

   #endregion Constructors

   #region Methodz

   #region IMPORT

   private int? LoadZnpFromFile(string fName)
   {
      txtLines = VvImpExp.GetFileLinesAsStringArray(fName);

      if(txtLines == null) return null;
      else                 return txtLines.Length;
   }

   private char GetLineID(string line)
   {
      return ZXC.GetStringsLastChar(line);
   }

   private bool Take_LABELA_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*    3 ZN0NAZKO*/
            case  3: headRecord.iz_firma = field; break;

            /*    4 ZN0MJEST*/
            case  4: headRecord.iz_mjesto = field; break;

            /*    5 ZN0DATUM*/
            case  5: headRecord.iz_dateIn6 = ZXC.ValOr_01010001_DateTime_Import_ddMMyy_Format(field); break;

            /*   11 ZN0VBDI*/
            case 11: headRecord.bankaVbdi = field; break;

            /*   12 ZN0NIDKOR*/
            case 12: headRecord.iz_ziro_rnpri = field;
                     headRecord.iz_ziro_racun = headRecord.bankaVbdi + "-" + headRecord.iz_ziro_rnpri; 
                     break;

            /*   14 ZN0D2000*/
            case 14: headRecord.iz_date = ZXC.ValOr_01010001_DateTime_Import_ddMMyyyy_Format(field); break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_LEAD_Data(string[] inFileData)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      foreach(string field in inFileData)
      {
         switch(colNum)
         {
            /*   5 ZN9SVOTA*/
            case 5: headRecord.iz_saldo = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /*   6 ZN9BRNAL*/
            case 6: headRecord.iz_numOfTranses = ZXC.ValOrZero_UInt(field); break;
         }

         colNum++;
      }

      return OK;
   }

   private bool Take_TRANS_Data(string[] inFileData, int serial)
   {
      bool OK = true;
      int colNum = 1;

      if(inFileData == null) return false;

      //transes[serial].t_br     = headRecord.iz_br;
      transes[serial].t_serial = serial;
      transes[serial].t_rbr    = (serial + 1).ToString() + ".";

      foreach(string field in inFileData)
      {
         switch(colNum)
         {

            /*    3 ZN1NAZIV  */
            case  3: transes[serial].t_kupdob = field; break;

            /*    4 ZN1MJEST  */
            case  4: transes[serial].t_kd_mjesto = field; break;

            /*    5 ZN1MOBRZ  */
            case  5: transes[serial].t_pnb0_zad = field; break;

            /*    6 ZN1PNBZA  */
            case  6: transes[serial].t_pnb1_zad = field; break;

            /*    7 ZN1SVRHA  */
            case  7: transes[serial].t_svrha_doz = field; break;

            /*    8 ZN1VEZOZ  */
            case  8: transes[serial].t_vezna_oznaka = field; break;

            /*   11 ZN1SVOTA  */
            case 11: transes[serial].t_iznos = ZXC.ValOrZero_Decimal(field, 2) / 100.00M; break;

            /*   12 ZN1MOBRO  */
            case 12: transes[serial].t_pnb0_odob = field; break;

            /*   13 ZN1PNBOD  */
            case 13: transes[serial].t_pnb1_odob = field; break;

            /*   14 ZN1VBDI   */
            case 14: transes[serial].t_kd_vbdi = field; break;

            /*   15 ZN1NOID   */
            case 15: transes[serial].t_rnpvd = field;

                  string ziroVbdi  = transes[serial].t_kd_vbdi;
                  string ziroRnpvd = transes[serial].t_rnpvd;

                  transes[serial].t_kd_ziro = ziroVbdi + "-" + ziroRnpvd;
               break;

         } // switch(colNum) 

         colNum++;
      }

      return OK;
   }

   #endregion IMPORT

   #region EXPORT

   #region SetFieldsValues

   private void SetLabelaAndLeadFieldsValues_OLD(DateTime znpDate)
   {
      // ___ LABELA START _________________________________________
      LabelaDict["ZN0NAZKO"] .FldValue = VirmanList[0].Plat1;
      LabelaDict["ZN0MJEST"] .FldValue = VirmanList[0].Mjesto1;
      LabelaDict["ZN0DATUM"] .SetDDMMYYFldValue(znpDate);
      LabelaDict["ZN0BRPAK"] .FldValue = "000";
      LabelaDict["ZN0IZDOK"] .FldValue = "701";
      LabelaDict["ZN0VBDI"]  .SetZirornPartFldValue(VirmanList[0].Ziro1, ZXC.Redak.prvi);
      LabelaDict["ZN0NIDKOR"].SetZirornPartFldValue(VirmanList[0].Ziro1, ZXC.Redak.drugi);
      LabelaDict["ZN0D2000"] .SetDDMMYYYYFldValue(znpDate);
      LabelaDict["ZN0TIPSL"] .FldValue = "0";
      // ___ LABELA END   _________________________________________

      // ___ LEAD START _________________________________________
      LeadDict["ZN9NAZIV"].FldValue = VirmanList[0].Plat1;
      LeadDict["ZN9MJEST"].FldValue = VirmanList[0].Mjesto1;
      LeadDict["ZN9SVOTA"].FldValue = "XxxxxxxxxxxxxxX";
      LeadDict["ZN9BRNAL"].FldValue = "YyyyY";
      LeadDict["ZN9VBDI"] .SetZirornPartFldValue(VirmanList[0].Ziro1, ZXC.Redak.prvi);
      LeadDict["ZN9NOID"] .SetZirornPartFldValue(VirmanList[0].Ziro1, ZXC.Redak.drugi);
      LeadDict["ZN9DATUM"].SetDDMMYYYYFldValue(znpDate);
      LeadDict["ZN9TIPSL"].FldValue = "9";
      // ___ LEAD END   _________________________________________
   }

   private void SetLabelaAndLeadFieldsValues_2013(DateTime znpDate, ZXC.ZNP_Kind ZNPkind) // TODO: ZNP_Kind.Placa_Specifikacija 
   {
      // ___ 300 _______________________________________________________
      Dict300["S300DATSL"   ].FldValue = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(znpDate);
    //Dict300["S300VRSTNAL" ].FldValue = "1";
      Dict300["S300VRSTNAL" ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "1" : "4";
      Dict300["S300IZDOK"   ].FldValue = "";
      Dict300["S300NACIZVR" ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : (ZNPkind == ZXC.ZNP_Kind.Placa_Specifikacija ? "1" : "2"); // 
      Dict300["S300OIBPOS"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : ZXC.CURR_prjkt_rec.Oib                                   ; // 
      Dict300["S300MBRPOS"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : ZXC.CURR_prjkt_rec.Matbr + /* podbroj? */ "000"          ; // 
      Dict300["S300INSIFPOS"].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : ""                                                       ; // 
      Dict300["S300OIBUPL"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : ZXC.CURR_prjkt_rec.Oib                                   ; // 
      Dict300["S300REZERVA" ].FldValue = "";
      Dict300["S300TIPSLOG" ].FldValue = "300";

      // ___ 301 _______________________________________________________
      Dict301["S301IBANPLAT"].FldValue = VirmanList[0].Ziro1;
      Dict301["S301VALPL"   ].FldValue = /*"HRK"*/ ZXC.EURorHRKstr;
      Dict301["S301RNNAK"   ].FldValue = "";
      Dict301["S301VALNAK"  ].FldValue = "";
    //Dict301["S301BRNALUK" ].FldValue = VirmanList[0].Mjesto1;
    //Dict301["S301IZNNALUK"].FldValue = VirmanList[0].Mjesto1;
      Dict301["S301DATIZVR" ].FldValue = ZXC.ValOrEmpty_YyyyMMddDateTime_AsText(VirmanList[0].DateValuta.NotEmpty() ? VirmanList[0].DateValuta : znpDate);
      Dict301["S301REZERVA" ].FldValue = "";
      Dict301["S301TIPSLOG" ].FldValue = "301";

      // ___ 399 _______________________________________________________
      Dict399["S399REZERVA" ].FldValue = ""   ;
      Dict399["S399PTIPSLOG"].FldValue = "399";
   }

   private void SetTransFieldsValues_OLD(VirmanStruct virman_rec)
   {
      VvImpExp.ClearFldValues(this.TransFields);

      NumOfTransLinesSoFar++;

      MoneySUM += virman_rec.Money;

      TransDict["ZN1SVOTA"].SetDecimalFldValue(virman_rec.Money);

      TransDict["ZN1DRZAD"].FldValue = "99";
      TransDict["ZN1DRODO"].FldValue = "19";
      TransDict["ZN1TIPSL"].FldValue = "1";

      TransDict["ZN1NAZIV"].FldValue = virman_rec.Prim1;
      TransDict["ZN1MJEST"].FldValue = virman_rec.Mjesto2;
      TransDict["ZN1VBDI"] .SetZirornPartFldValue(virman_rec.Ziro2, ZXC.Redak.prvi);
      TransDict["ZN1NOID"] .SetZirornPartFldValue(virman_rec.Ziro2, ZXC.Redak.drugi);
      TransDict["ZN1VEZOZ"].FldValue = virman_rec.SifraPl;
      TransDict["ZN1MOBRO"].FldValue = virman_rec.PnboMod;
      TransDict["ZN1PNBOD"].FldValue = virman_rec.Pnbo;
      TransDict["ZN1SVRHA"].FldValue = virman_rec.OpisPl;
   
      // 01.06.2012: 
      TransDict["ZN1MOBRZ"].FldValue = virman_rec.PnbzMod;
      TransDict["ZN1PNBZA"].FldValue = virman_rec.Pnbz;
   }

   private void SetTransFieldsValues_2013(VirmanStruct virman_rec, ZXC.ZNP_Kind ZNPkind) // TODO: ZNP_Kind.Placa_Specifikacija 
   {
      VvImpExp.ClearFldValues(this.TransFields);

      NumOfTransLinesSoFar++;

      MoneySUM += virman_rec.Money;

      Dict309["S309IBANRNPRIM" ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Ziro2   : virman_rec.Pnbo  ;
      Dict309["S309NAZIVPRIM"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Prim1   : virman_rec.OpisPl; // ... 
      Dict309["S309ADRPRIM"    ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Prim2   : ""               ;
      Dict309["S309SJEDPRIM"   ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Prim3   : ""               ;
      Dict309["S309SFZEMPRIM"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? ""                 : "000"            ;
      Dict309["S309BRMODPLAT"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.PnbzMod : ""               ;
      Dict309["S309PNBPLAT"    ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Pnbz    : ""               ;
      Dict309["S309SIFNAM"     ].FldValue = "";
      Dict309["S309OPISPL"     ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.OpisPl  : "4 - Plaće, ostala redovna i povremena primanja";
      // 03.03.2017. 
    //Dict309["S309IZN"        ].SetDecimalFldValue(virman_rec.Money       );
      Dict309["S309IZN"        ].SetDecimalFldValue(virman_rec.Money.Ron2());
      Dict309["S309BRMODPRIM"  ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.PnboMod : ""               ;
      Dict309["S309PNBPRIM"    ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? virman_rec.Pnbo    : ""               ;
      Dict309["S309BICBANPRIM" ].FldValue = ""; // SWIFT 
      Dict309["S309NAZBANPRIM" ].FldValue = "";
      Dict309["S309ADRBNPRIM"  ].FldValue = "";
      Dict309["S309SJEDBNPRIM" ].FldValue = "";
      Dict309["S309SFZEMBNPRIM"].FldValue = "";
      Dict309["S309VRSTAPRIM"  ].FldValue = "";
      Dict309["S309VALPOKR"    ].FldValue = "";
      Dict309["S309TROSOP"     ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "3"                : "";
      Dict309["S309OZNHITN"    ].FldValue = "0";
      Dict309["S309SIFPRIM"    ].FldValue = ZNPkind == ZXC.ZNP_Kind.Classic ? "" : Ptrans.GetVrstaOsobPrimanjaForZNPplaca(); // TODO: ??? za sada fiksno "100" 
      Dict309["S309REZERVA"    ].FldValue = "";
      Dict309["S309TIPSLOG"    ].FldValue = "309";
   }

   #endregion SetFieldsValues

   private void DumpZnpDataInFile()
   {
      bool   isInitializing;
      bool   isFinalizing  ;

      if(NumOfTransLinesSoFar == 1) isInitializing = true;
      else                          isInitializing = false;

      if(NumOfTransLinesSoFar == VirmanList.Count) isFinalizing = true ;
      else                                         isFinalizing = false;

      if(isInitializing)
      {
         using(StreamWriter sw = new StreamWriter(FullPathFileName, false, System.Text.Encoding.GetEncoding(1250)))
         {
            if(IsNewZNP2013Format)
            {
               VvImpExp.DumpFields(sw, LabelaFields_300 );
               VvImpExp.DumpFields(sw, LeadFields_301   );
               VvImpExp.DumpFields(sw, TransFields_309  );
            }
            else
            {
               VvImpExp.DumpFields(sw, LabelaFields);
               VvImpExp.DumpFields(sw, LeadFields  );
               VvImpExp.DumpFields(sw, TransFields );
            }
         }
      }
      else
      {
         using(StreamWriter sw = new StreamWriter(FullPathFileName, true, System.Text.Encoding.GetEncoding(1250)))
         {
            if(IsNewZNP2013Format)
               VvImpExp.DumpFields(sw, TransFields_309);
            else
               VvImpExp.DumpFields(sw, TransFields);
         }
      }

      if(isFinalizing && IsNewZNP2013Format)
      {
         using(StreamWriter sw = new StreamWriter(FullPathFileName, true, System.Text.Encoding.GetEncoding(1250)))
         {
            VvImpExp.DumpFields(sw, ClosingFields_399);
         }
      }
   }

   public void SetAndDumpZnpLine(VirmanStruct virman_rec, ZXC.ZNP_Kind ZNPkind)
   {
      if(IsNewZNP2013Format)
         SetTransFieldsValues_2013(virman_rec, ZNPkind);
      else
         SetTransFieldsValues_OLD(virman_rec);

      DumpZnpDataInFile();
   }

   public void CloseZNP()
   {
      byte[] bytes;

      using(FileStream fs = new FileStream(FullPathFileName, FileMode.Open))
      {
         if(IsNewZNP2013Format)
         {
            string S301BRNALUK  = Dict301["S301BRNALUK" ].SetIntgerFldValue (NumOfTransLinesSoFar);
            string S301IZNNALUK = Dict301["S301IZNNALUK"].SetDecimalFldValue(MoneySUM            );

            fs.Seek(1 * (NewLineFixedLength + 2) + 48, SeekOrigin.Begin);

            bytes = new System.Text.UTF8Encoding(true).GetBytes(S301BRNALUK);

            fs.Write(bytes, 0, Dict301["S301BRNALUK"].FldLength);

            bytes = new System.Text.UTF8Encoding(true).GetBytes(S301IZNNALUK);

            fs.Write(bytes, 0, Dict301["S301IZNNALUK"].FldLength);
         }
         else
         {
            string ZN9SVOTA = LeadDict["ZN9SVOTA"].SetDecimalFldValue(MoneySUM);
            string ZN9BRNAL = LeadDict["ZN9BRNAL"].SetIntgerFldValue(NumOfTransLinesSoFar);

            fs.Seek(1 * (OldLineFixedLength + 2) + 78, SeekOrigin.Begin);

            bytes = new System.Text.UTF8Encoding(true).GetBytes(ZN9SVOTA);
         
            fs.Write(bytes, 0, LeadDict["ZN9SVOTA"].FldLength);

            bytes = new System.Text.UTF8Encoding(true).GetBytes(ZN9BRNAL);

            fs.Write(bytes, 0, LeadDict["ZN9BRNAL"].FldLength);
         }
      }
   }

   #endregion EXPORT
   
   #endregion Methodz

}

// 15.07.2020: VirmanStruct postaje class za potrebe da se vidi crystalima kao reportSource 
// naziv se mijenja iz 'VirmanStruct' u 'Virman' 
// a dodan i ovaj parameterless constructor jerbo struct - class konverzija ima znasveckajovoono sa constructorima ... 
public /*struct*/ class VirmanStruct
{
   #region Propertiz

   public string BtchBookgID { get; set; }

   public string Plat1   { get; set; }
   public string Plat2   { get; set; }
   public string Plat3   { get; set; }
   public string Mjesto1 { get; set; }


   public DateTime DateValuta { get; set; }
   public DateTime DatePodnos { get; set; }

   public string Prim1   { get; set; }
   public string Prim2   { get; set; }
   public string Prim3   { get; set; }
   public string Mjesto2 { get; set; }

   public string Pnbz    { get; set; }
   public string Pnbo    { get; set; }

   public string StatOb  { get; set; }
   public string SifraPl { get; set; }
   public string OpisPl  { get; set; }
   
   public decimal Money  { get; set; }

   // 23.05.2016: 
   public ZXC.VirmanEnum VirKind { get; set; }

   public Ptrans.PtranoKind PtranoKind { get; set; }
   public bool IsSALA { get { return (this.PtranoKind == Ptrans.PtranoKind.NEZASTICENIrn || this.PtranoKind == Ptrans.PtranoKind.ZASTICENIrn); } }

   private string ziro1;
   public  string Ziro1
   {
      get { return ziro1; }
      set 
      {
         if(value.Contains("-") && value.StartsWith("HR") == false) // stara, NE IBAN konstrukcija nekog ziroRacuna 
         {
            string IBAN_KBR = ZXC.GetIBAN_KBR(value);

            ziro1 = ZXC.GetIBANfromOldZiro(value);
         }
         else
         {
            ziro1 = value;
         }
      }
   }

   private string ziro2;
   public  string Ziro2
   {
      get { return ziro2; }
      set 
      {
         if(value.Contains("-") && value.StartsWith("HR") == false) // stara, NE IBAN konstrukcija nekog ziroRacuna 
         {
            string IBAN_KBR = ZXC.GetIBAN_KBR(value);

            ziro2 = ZXC.GetIBANfromOldZiro(value);
         }
         else
         {
            ziro2 = value;
         }
      }
   }

   private string pnboMod;
   public  string PnboMod
   {
      get { return pnboMod; }
      set 
      {
         if(value.Length == 2 && value.StartsWith("HR") == false) // stara, NE IBAN konstrukcija nekog ziroRacuna DrzavnogProracuna 
         {
            pnboMod = "HR" + value;
         }
         else
         {
            pnboMod = value;
         }
      }
   }

   private string pnbzMod;
   
   public  string PnbzMod
   {
      get { return pnbzMod; }
      set 
      {
         if(value.Length == 2 && value.StartsWith("HR") == false) // stara, NE IBAN konstrukcija nekog ziroRacuna DrzavnogProracuna 
         {
            pnbzMod = "HR" + value;
         }
         else
         {
            pnbzMod = value;
         }
      }
   }

   public PAIN_001_001_09.PostalAddress24 SEPA_PstlAdr {  get; set; }

   #endregion Propertiz

   #region Constructors

   public VirmanStruct()
   {
      this.SEPA_PstlAdr = new PAIN_001_001_09.PostalAddress24();
   }


   public VirmanStruct(object[] stdParams) : this()
   {
      Plat1   = (string)stdParams[0];
      Plat2   = (string)stdParams[1];
      Plat3   = (string)stdParams[2];
      Ziro1   = (string)stdParams[3];
      Mjesto1 = (string)stdParams[4];

      DateValuta = (DateTime)stdParams[5];
      DatePodnos = (DateTime)stdParams[6];


   }

   public VirmanStruct(string _plat1, string _plat2, string _plat3, string _ziro1, DateTime _dateValuta, DateTime _datePodnos, string _mjesto1) : this()
   {
      Plat1 = _plat1;
      Plat2 = _plat2;
      Plat3 = _plat3;
      Ziro1 = _ziro1;

      DateValuta = _dateValuta;
      DatePodnos = _datePodnos;

      Mjesto1 = _mjesto1;
   }

   public VirmanStruct(Xtrans xtrans_rec) : this()
   {
      Plat1   = xtrans_rec.T_kpdbNameA_50;
      Plat2   = xtrans_rec.T_kpdbUlBrA_32;
      Plat3   = xtrans_rec.T_kpdbMjestoA_32;
      Ziro1   = xtrans_rec.T_kpdbZiroA_32;
      Mjesto1 = xtrans_rec.T_kpdbMjestoA_32;

      Prim1   = xtrans_rec.T_kpdbNameB_50;
      Prim2   = xtrans_rec.T_kpdbUlBrB_32;
      Prim3   = xtrans_rec.T_kpdbMjestoB_32;
      Ziro2   = xtrans_rec.T_kpdbZiroB_32;
      Mjesto2 = xtrans_rec.T_kpdbMjestoB_32;

      DateValuta = xtrans_rec.T_dateOd;
      DatePodnos = xtrans_rec.T_dateDo;

      PnbzMod    = RptP_SEPA.GetPnbM(xtrans_rec.T_strA_2, xtrans_rec.T_vezniDokA_64)/*xtrans_rec.T_strA_2*/;
      Pnbz       = xtrans_rec.T_vezniDokA_64;
      PnboMod    = RptP_SEPA.GetPnbM(xtrans_rec.T_strB_2, xtrans_rec.T_vezniDokB_64)/*xtrans_rec.T_strB_2*/;
      Pnbo       = xtrans_rec.T_vezniDokB_64;

      xtrans_rec.T_strA_2 = PnbzMod;
      xtrans_rec.T_strB_2 = PnboMod;

      OpisPl     = xtrans_rec.T_opis_128;
      SifraPl    = xtrans_rec.T_strC_2;

      Money      = xtrans_rec.T_moneyA.Ron2();

    // 01.09.2025. upali ovo za strukturiranu sepu samo sa gradom i državom
    //SEPA_PstlAdr.Ctry  = xtrans_rec.T_konto;
    //SEPA_PstlAdr.TwnNm = xtrans_rec.T_kpdbMjestoB_32;
   }

   public VirmanStruct(Vektor.DataLayer.DS_Reports.DS_Placa.virmanRow virmanRow) : this()
   {
      Plat1      = virmanRow.  plat1      ;
      Plat2      = virmanRow.  plat2      ;
      Plat3      = virmanRow.  plat3      ;
      Ziro1      = virmanRow.  ziro1      ;
      Mjesto1    = virmanRow.  mjesto1    ;
      Prim1      = virmanRow.  prim1      ;
      Prim2      = virmanRow.  prim2      ;
      Prim3      = virmanRow.  prim3      ;
      Ziro2      = virmanRow.  ziro2      ;
      Mjesto2    = virmanRow.  mjesto2    ;
      DateValuta = virmanRow.  dateValuta ;
      DatePodnos = virmanRow.  datePodnos ;
      PnbzMod    = virmanRow.  pnbzMod    ;
      Pnbz       = virmanRow.  pnbz       ;
      PnboMod    = virmanRow.  pnboMod    ;
      Pnbo       = virmanRow.  pnbo       ;
      OpisPl     = virmanRow.  opisPl     ;
      SifraPl    = virmanRow.  sifraPl    ;
      Money      = virmanRow.  money      ;
   }

   #endregion Constructors

   #region Onaj Barkod za placanje preko mFotoPlati

   // ovo ide kristalima: 
   public byte[] HUB3Data_PDF417
   {
      get
      {
         System.Drawing.Image theImage = HUB3Data_PDF417_Image;

         if(theImage == null) return null;

         using(System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
         {
            theImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream.ToArray();
         }
      }
   }

   public/*private*/ System.Drawing.Image HUB3Data_PDF417_Image
   {
      get
      {
         System.Drawing.Image PDF417Image = null;

         string pdf417Str = Get_PDF417_string();

         // ovaj try blok je izveden iz:                                                                                 
         // E:\0_DOWNLOAD\Code Project Stuff\QRCode And PDF_417\ZXing\DELME\ZXing.Net-master\Clients\WindowsFormsDemo.cs 
         // private void btnEncode_Click(object sender, EventArgs e)                                                     
         try
         {
            Type          Renderer = typeof(BitmapRenderer);
            BarcodeWriter writer   = new BarcodeWriter
            {
               Format  = BarcodeFormat.PDF_417,
               //Options = new EncodingOptions
               //{
               //   Height = /*picEncodedBarCode.Height*/ 100, // todo 
               //   Width  = /*picEncodedBarCode.Width */ 100  // todo 
               //},
               //Renderer = (IBarcodeRenderer<Bitmap>)Activator.CreateInstance(Renderer)
            };

            PDF417Image = writer.Write(pdf417Str);
         }
         catch(Exception exc)
         {
            ZXC.aim_emsg("Error: ", exc.Message);
         }

         return PDF417Image;
      }
   }

   private string Get_PDF417_string()
   {
      string  opis     = this.OpisPl ;

      string pdf417Str = "";

      #region set data

      // TODO 
      // imas PDF: E:\0_DOWNLOAD\Code Project Stuff\QRCode And PDF_417\PDF_417\2dbc_0.pdf 

      // Redni broj polja Naziv polja                      Duljina polja 
      //       1          Zaglavlje                                    8 
      //       2          Valuta                                       3 
      //       3          Iznos                                       15 
      //       4          Ime i prezime platitelja                    30 
      //       5          Adresa platitelja (ulica i broj)            27 
      //       6          Adresa platitelja (poštanski broj i mjesto) 27 
      //       7          Naziv primatelja                            25 
      //       8          Adresa primatelja (ulica i broj)            25 
      //       9          Adresa primatelja (poštanski broj i mjesto) 27 
      //       10         IBAN ili raĉun primatelja                   21 
      //       11         Model raĉuna primatelja                      4 
      //       12         Poziv na broj primatelja                    22 
      //       13         Šifra namjene                                4 
      //       14         Opis plaćanja                               35 
      // Ukupno znakova 273

      string fld_ZAGLAVLJE = "HRVHUB30"                       ;
      string fld_VALUTA    = /*"HRK"*/ ZXC.EURorHRKstr        ;
      string fld_IZNOS     = VvImpExp.ImpExpField.SetDecimalFldValue_Static(this.Money, 15);
      string fld_PLAT1     = this.Plat1; //ZXC.CURR_prjkt_rec.Naziv       ;
      string fld_PLAT2     = this.Plat2; //ZXC.CURR_prjkt_rec.Ulica1      ;
      string fld_PLAT3     = this.Plat3; //ZXC.CURR_prjkt_rec.ZipAndMjesto;
      string fld_PRIM1     = this.Prim1                        ;
      string fld_PRIM2     = this.Prim2                        ;
      string fld_PRIM3     = this.Prim3                        ;
      string fld_PRIM_IBAN = this.Ziro2                        ;
      string fld_PRIM_MOD  = this.PnboMod                      ;
      string fld_PRIM_PNB  = this.Pnbo                         ;
      string fld_SIF_NAMJ  = this.SifraPl                      ;
      string fld_OPIS      = ZXC.LenLimitedStr(this.OpisPl, 35);

      pdf417Str =
                  fld_ZAGLAVLJE + "\n" +
                  fld_VALUTA    + "\n" +
                  fld_IZNOS     + "\n" +
                  fld_PLAT1     + "\n" +
                  fld_PLAT2     + "\n" +
                  fld_PLAT3     + "\n" +
                  fld_PRIM1     + "\n" +
                  fld_PRIM2     + "\n" +
                  fld_PRIM3     + "\n" +
                  fld_PRIM_IBAN + "\n" +
                  fld_PRIM_MOD  + "\n" +
                  fld_PRIM_PNB  + "\n" +
                  fld_SIF_NAMJ  + "\n" +
                  fld_OPIS      + "\n" ;

      // pdf417Str =
      //       "HRVHUB30\n" +
      //       "HRK\n" +
      //       ZXC.ToStringVv_HUB3_PDF417(faktur_rec.S_ukKCRP, 15) +  //"000000000012355\n" 
      //       "ŽELJKO SENEKOVIĆ\n" +
      //       "IVANEČKA ULICA 125\n" +
      //       "42000 VARAŽDIN\n" +
      //       "2DBK d.d.\n" +
      //       "ALKARSKI PROLAZ 13B\n" +
      //       "21230 SINJ\n" +
      //       "HR1210010051863000160\n" +
      //       "HR01\n" +
      //       "7269-68949637676-00019\n" +
      //       "COST\n" +
      //       "Troškovi za 1. mjesec\n";

      #endregion set data

      return pdf417Str;
   }

   #endregion Onaj Barkod za placanje preko mFotoPlati

}
