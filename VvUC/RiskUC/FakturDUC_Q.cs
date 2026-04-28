using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using com.handpoint.api;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.IO;
#endif

public struct TtInfo
{
   #region General Propertiz

   public string TheTT          { get; set; }
   public short  TtSort         { get; set; }
   public bool   IsSklCdInTtNum { get; set; }
   public bool   IsYearInTtNum  { get; set; }

   public Point              DefaultSubModulXY   { get; set; }
   public ZXC.VvSubModulEnum DefaultSubModulEnum { get; set; }

   public bool IsFinKol_U  { get; set; }
   public bool IsFinKol_I  { get; set; }
   public bool IsFinKol_PS { get; set; }
   public bool IsFinKol_TT { get { return (IsFinKol_PS || IsFinKol_U || IsFinKol_I); } }
   
   public bool IsKolOnly_U  { get; set; } // 'Skladistari' ili Korekcija kolicine
   public bool IsKolOnly_I  { get; set; } 
   public bool IsKolOnly_TT { get { return (IsKolOnly_U || IsKolOnly_I); } }

   public bool IsAnyKol_I { get { return IsFinKol_I || IsKolOnly_I; } }

   public string TwinTT  { get; set; }
   public bool HasTwinTT { get { return TwinTT.NotEmpty(); } } // Medjuskladisnice 

   public string SplitTT  { get; set; }
   public bool HasSplitTT { get { return SplitTT.NotEmpty(); } } // Proizvodnja 

   public string ShadowTT { get; set; }
   /// <summary>
   /// PSM, KLK, URM, IRM
   /// </summary>
   public bool HasShadowTT { get { return ShadowTT.NotEmpty(); } } // Implicitne Nivelacije VMU namjerno nije ovdje, do dalnjega 

   public bool IsIzlazniShadowTT   { get { return HasShadowTT && ShadowTT == Faktur.TT_NIV; } }
   public bool IsUlazniShadowTT    { get { return HasShadowTT && ShadowTT == Faktur.TT_NUV; } }
   public bool IsUraPovratShadowTT { get { return HasShadowTT && ShadowTT == Faktur.TT_NUP; } }
   public bool IsMODulazTT         { get { return this.TheTT == Faktur.TT_MOU; } }
   public bool IsMODizlazTT        { get { return this.TheTT == Faktur.TT_MOI; } }
   public bool Is_MOC_or_MOS_TT    { get { return this.TheTT == Faktur.TT_MOC || 
                                                  this.TheTT == Faktur.TT_MOS; } }

   public bool Is_MOD_or_MOC_or_MOS_TT { get { return this.TheTT == Faktur.TT_MOD ||
                                                      this.TheTT == Faktur.TT_MOC ||
                                                      this.TheTT == Faktur.TT_MOS; } }
   public bool IsPreDef        { get; set; }
   public bool IsRezervKol     { get; set; }
   public bool IsInventura     { get; set; }
   public bool IsNivelacijaZPC { get; set; }

   public bool IsInternUlaz { get { return IsInternUlaz_FromOneIzlaz || IsInternUlaz_FromManyIzlaz; } }

   public bool IsInternUI { get { return IsInternUlaz || IsInternIzlaz; } }

   // TODO: !!! da li su KIZ i KUL 'IsAllSkladFinKol_U' 
   /// <summary>
   /// Eksterni (vanjski) promet ONLY. Povecava stanje s obzirom na sva skladista kao jedno. InterniUlaz tu NE spada. PST tu NE spada.
   /// </summary>
   public bool IsAllSkladFinKol_U { get { return (IsFinKol_U == true && IsFinKol_PS == false && IsInternUlaz == false && IsNivelacijaZPC == false); } }

   /// <summary>
   /// Eksterni (vanjski) promet ONLY. Smanjuje stanje s obzirom na sva skladista kao jedno. InterniIzlaz tu NE spada.
   /// </summary>
   public bool IsAllSkladFinKol_I { get { return (IsFinKol_I == true && IsInternIzlaz == false); } }

   /// <summary>
   /// TT izlaznog dokumenta koji je i stvorio ovaj ulaz (ili TwinTT ili SplitTT slucaj)
   /// </summary>
   public string LinkedDefaultTT { get; set; }

   public bool IsArtiklStatusInfluencer { get { return (
      
         IsFinKol_TT   ||
         IsKolOnly_TT  ||
         IsRezervKol   ||
         IsPreDef      ||
         IsInventura   ||

         // 22.04.2012: 
         IsPonudaTT    ||

         // 05.04.2018: SVD 
         (ZXC.IsSvDUH && TheTT == Faktur.TT_NRD)

      ); } }

   /// <summary>
   /// Faktur.TT_MSI:
   /// Faktur.TT_VMI:
   /// Faktur.TT_PIZ:
   /// Faktur.TT_TMU:
   /// Faktur.TT_TMI:
   /// Faktur.TT_IMT:
   /// Faktur.TT_PPR:
   /// Faktur.TT_POV:
   /// Faktur.TT_INV:
   /// Faktur.TT_ZPC:
   /// </summary>
   public bool IsDokCijShouldBePrNabCij 
   { 
      get 
      {
         if(this.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij/*                ||
            this.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrOfPrNabCij_SumeSastojaka*/) return true;
         else                                                                                           return false;
      } 
   }

   public override string ToString()
   {
      return "'" + TheTT + "'";
   }

   #endregion General Propertiz

   #region IsExtendable

   private static string[] arrayExtendables = new string[] { 
      Faktur.TT_IFA, 
      Faktur.TT_IRA, 
//    Faktur.TT_STI, 
      Faktur.TT_IZD, 
      Faktur.TT_POI, 
      Faktur.TT_IMT, 
      Faktur.TT_IZM, 
    //Faktur.TT_IMM, 
      Faktur.TT_RVI, 
      Faktur.TT_IOD, 
      Faktur.TT_IPV, 
      Faktur.TT_IRM, 
      Faktur.TT_UFA,
      Faktur.TT_UPA,
      Faktur.TT_UFM,
      Faktur.TT_URA,
      Faktur.TT_URM,
      Faktur.TT_UPM,
//    Faktur.TT_STU,
      Faktur.TT_UOD,
      Faktur.TT_UPV,
      Faktur.TT_PRI,
      Faktur.TT_POU,
      Faktur.TT_POT,
      Faktur.TT_ZAR,
      Faktur.TT_PRP,
    //Faktur.TT_PIP,
      Faktur.TT_RVU,
      Faktur.TT_KLK,
      Faktur.TT_KKM,
      Faktur.TT_OPN,
      Faktur.TT_PON,
      Faktur.TT_PNM,
      Faktur.TT_INM,
      Faktur.TT_NRD,
      Faktur.TT_NRM,
      Faktur.TT_NRU,
      Faktur.TT_NRS,
      Faktur.TT_NRK,
      Faktur.TT_UPL,
      Faktur.TT_ISP,
      Faktur.TT_BUP,
      Faktur.TT_ABU,
      Faktur.TT_BIS,
      Faktur.TT_ABI,
      Faktur.TT_RNP,
      Faktur.TT_RNS,
      Faktur.TT_PRJ,
      Faktur.TT_RNM,
      Faktur.TT_RNU,
      Faktur.TT_RNZ,
      Faktur.TT_UGO,

      Faktur.TT_KUG, // PCTGO tt 
      Faktur.TT_AUN, // PCTGO tt 
      Faktur.TT_UGN, // PCTGO tt 
      Faktur.TT_DIZ, // PCTGO tt 
      Faktur.TT_PVR, // PCTGO tt 
    //Faktur.TT_PVD, // PCTGO tt 
      Faktur.TT_ZIZ, // PCTGO tt 
      Faktur.TT_MPI, // PCTGO tt 
      Faktur.TT_AU2, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_UG2, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_DI2, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_PV2, // PCTGO tt ovo je mozda visak, al ziheraski 
    //Faktur.TT_PD2, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_ZI2, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_ZUL, // PCTGO tt ovo je mozda visak, al ziheraski 
      Faktur.TT_ZU2, // PCTGO tt ovo je mozda visak, al ziheraski 

      Faktur.TT_MOD, // PCTGO tt 
      Faktur.TT_MOI, // PCTGO tt 
      Faktur.TT_MOU, // PCTGO tt 

      Faktur.TT_KIZ,
      Faktur.TT_KUL,
      Faktur.TT_PIK,
      Faktur.TT_PUK,

      // 10.01.2012: ovi su tu NE zbog kupdob-a, nego zbog SUM-a (S_ukKCRP, ...) 
      Faktur.TT_PSM,
      Faktur.TT_ZPC,
      Faktur.TT_ZKC,
      Faktur.TT_VMI,
      Faktur.TT_VMU,
      Faktur.TT_TRI,
      Faktur.TT_TRM,

      //29.03.2013: 
      Faktur.TT_PIX,
      Faktur.TT_PUX,

      Faktur.TT_BOR,
      Faktur.TT_NOR,
    //Faktur.TT_NOM,
      Faktur.TT_PIM,

      Faktur.TT_MMI,
      Faktur.TT_MVI,
      Faktur.TT_MVU,

      //08.09.2015: 
      Faktur.TT_PIZ,
      Faktur.TT_PUL,

    //Faktur.TT_WFA,
    //Faktur.TT_WRA,
    //Faktur.TT_WRM,
    //Faktur.TT_YFA,
    //Faktur.TT_YRA,
    //Faktur.TT_YRM,
      Faktur.TT_WRN,
      Faktur.TT_YRN,

      Faktur.TT_CKP,
      Faktur.TT_ZAH,

   };

   public bool IsExtendableTT { get { return arrayExtendables.Contains(TheTT); } }

   #endregion IsExtendable

   #region ProposeCijenaKind - TODO 4 MMI, MVI !!! (MVI bi trebao k'o IZM, MMI ko KLK/URM ? )

   public ZXC.TtProposeCijenaKindEnum ProposeCijenaKind
   {
      get
      {
         switch(TheTT)
         {
            case Faktur.TT_IRA:
            case Faktur.TT_IFA:
            case Faktur.TT_IOD:
            case Faktur.TT_IPV:
      case Faktur.TT_IZD:
      case Faktur.TT_POI:
            case Faktur.TT_IRM:
            case Faktur.TT_PIM: // !!! 
            case Faktur.TT_IZM:
            case Faktur.TT_OPN:
            case Faktur.TT_PON:
            case Faktur.TT_PNM:
            case Faktur.TT_NRK:
            case Faktur.TT_INM:
            case Faktur.TT_BOR:
          //case Faktur.TT_KIZ:

            case Faktur.TT_CKP:

            case Faktur.TT_AUN:
            case Faktur.TT_UGN:
            case Faktur.TT_DIZ:
            case Faktur.TT_PVR:
            case Faktur.TT_ZIZ:
            case Faktur.TT_ZUL:

          //case Faktur.TT_PVD:

               return ZXC.TtProposeCijenaKindEnum.Propose_CJENIK;

//case Faktur.TT_IZD: // SVD TEMP!!!

            case Faktur.TT_MSI:
            case Faktur.TT_MPI:
            case Faktur.TT_KIZ:
            case Faktur.TT_PIK:
            case Faktur.TT_VMI:
          //case Faktur.TT_MSU:
            case Faktur.TT_PIZ:
            case Faktur.TT_PIX:
          //case Faktur.TT_PIM:
          //case Faktur.TT_TMU: // remarckirano od 10.12.2015: od kada počesmo (aorist) koristiti korekturne temeljnice 
          //case Faktur.TT_TMI: // remarckirano od 10.12.2015: od kada počesmo (aorist) koristiti korekturne temeljnice 
            case Faktur.TT_IMT:
            case Faktur.TT_PPR:
            case Faktur.TT_POV:
            case Faktur.TT_INV:
            case Faktur.TT_ZPC:
            case Faktur.TT_ZKC:
            case Faktur.TT_NOR:
            case Faktur.TT_TRI:
          //case Faktur.TT_NOM:

          //case Faktur.TT_UPV:
          //case Faktur.TT_UOD:

            case Faktur.TT_MVI: // !!! 

            case Faktur.TT_MOI: 

               return ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij;

            case Faktur.TT_PUL: // Proizvodnja ulaz 
            case Faktur.TT_PUX: // Proizvodnja ulaz 
            case Faktur.TT_PUM: // Proizvodnja ulaz 
            case Faktur.TT_TRM: // Proizvodnja ulaz - MALOPRODAJA !!! ??? 
            case Faktur.TT_RNU: // Proizvodnja ulaz 
            case Faktur.TT_MOU:

               return ZXC.TtProposeCijenaKindEnum.Propose_PrOfPrNabCij_SumeSastojaka;

            default: 
               return ZXC.TtProposeCijenaKindEnum.Propose_NONE;

         }
      }
   }

   #endregion ProposeCijenaKind

   #region IsIRArucable

   private static string[] arrayIRArucables = new string[] { 
      Faktur.TT_IRA, 
//    Faktur.TT_STI, 
      Faktur.TT_IZD, 
      Faktur.TT_POI, 
      Faktur.TT_IZM, 
      Faktur.TT_IOD, 
      Faktur.TT_IPV, 
      Faktur.TT_IRM, 
      Faktur.TT_OPN,
      Faktur.TT_KIZ,
      // NE! nije ArtStatInfluencer Faktur.TT_PON,

      // 22.04.2013: ipak 
      Faktur.TT_PON,
      Faktur.TT_PNM,

      Faktur.TT_AUN,
      Faktur.TT_UGN,
      Faktur.TT_DIZ,
      Faktur.TT_PVR,
    //Faktur.TT_PVD,
   };

   public bool IsIRArucableTT { get { return arrayIRArucables.Contains(TheTT); } }

   #endregion IsIRArucable

   #region IsSkladDateTT

   private static string[] arraySkladDateTT = new string[] { 
      Faktur.TT_IRA, 
      Faktur.TT_KIZ, 
      Faktur.TT_PIK, 
//    Faktur.TT_STI, 
      Faktur.TT_IZD, 
      Faktur.TT_POI, 
      Faktur.TT_IZM, 
      Faktur.TT_RVI, 
      Faktur.TT_IRM, 
      Faktur.TT_IOD,
      Faktur.TT_IPV,
      Faktur.TT_URA,
      Faktur.TT_URM,
//    Faktur.TT_STU,
      Faktur.TT_UOD,
      Faktur.TT_UPV,
      Faktur.TT_UPM,
      Faktur.TT_PRI,
      Faktur.TT_POU,
      Faktur.TT_POT,
      Faktur.TT_PRP,
    //Faktur.TT_PIP,
      Faktur.TT_RVU,
      Faktur.TT_KLK,
      Faktur.TT_KKM,

      Faktur.TT_AUN,
      Faktur.TT_UGN,
      Faktur.TT_DIZ,
      Faktur.TT_PVR,
    //Faktur.TT_PVD,
   };
   /// <summary>
   /// Ovi TT-ovi u T_skladDate napucavaju F_skladDate a ne F_dokDate
   /// </summary>
 //public bool IsSkladDateTT { get { return arraySkladDateTT.Contains(TheTT); } } // 16.05.2023; 
   public bool IsSkladDateTT 
   { 
      get 
      {
         bool twinTTisSkladDateTT = false;

         if(LinkedDefaultTT.NotEmpty())
         {
            TtInfo linkedIzlazttInfo = ZXC.TtInfo(LinkedDefaultTT);

            twinTTisSkladDateTT = linkedIzlazttInfo.IsSkladDateTT;
         }

         return arraySkladDateTT.Contains(TheTT) || twinTTisSkladDateTT; 
      } 
   }

   #endregion IsSkladDateTT

   #region IsArtStatInfoNeeded

   private static string[] arrayArtStatInfoNeededTT = new string[] { 
      Faktur.TT_IFA, // zbog cijene
      Faktur.TT_IRA, 
//    Faktur.TT_STU, 
      Faktur.TT_IZD, 
      Faktur.TT_POI, 
      Faktur.TT_IZM, 
      Faktur.TT_RVI, 
      Faktur.TT_IRM, 
      Faktur.TT_MSI, 
      Faktur.TT_MPI, 
      Faktur.TT_ZIZ, 
      Faktur.TT_ZUL, 
      Faktur.TT_MMI, 
      Faktur.TT_KIZ, 
      Faktur.TT_PIK, 
      Faktur.TT_VMI, 
      Faktur.TT_MVI, 
      Faktur.TT_PIZ, 
      Faktur.TT_PIX, 
      Faktur.TT_PIM, 
      Faktur.TT_OPN, 
      Faktur.TT_PON, 
      Faktur.TT_PNM, 
      Faktur.TT_UOD, 
      Faktur.TT_UPV, 
      Faktur.TT_IOD, 
      Faktur.TT_IPV, 
      Faktur.TT_NRK, 
      Faktur.TT_IMT, 
    //Faktur.TT_IMM, 
      Faktur.TT_PPR, 
      Faktur.TT_POV, 
      Faktur.TT_ZPC, 
      Faktur.TT_ZKC, 
      Faktur.TT_TMU, 
      Faktur.TT_TMI, 
      Faktur.TT_KLK, 
      Faktur.TT_KKM, 
      Faktur.TT_URM, 
      Faktur.TT_UPM, 
      Faktur.TT_INV, 
      Faktur.TT_INM, 
      Faktur.TT_BOR, 
      Faktur.TT_NOR, 
      Faktur.TT_TRI, 
      //07.07.2015.
      Faktur.TT_PSM,

      Faktur.TT_AUN,
      Faktur.TT_UGN,
      Faktur.TT_DIZ,
      Faktur.TT_PVR,
    //Faktur.TT_PVD,
      Faktur.TT_MOI,
   };
   // Za sada ovo sluzi samo pri 'AnyArtiklTextBox_OnGrid_Leave' na FakturDUC-u 
   /// <summary>
   /// Ovi TT-ovi trebaju ArtStat info kod UpdateArtikl na RISK dokumentima
   /// </summary>
   public bool IsArtStatInfoNeededTT { get { return arrayArtStatInfoNeededTT.Contains(TheTT); } }

   #endregion IsArtStatInfoNeeded

   #region IsPonudaTT

   private static string[] arrayPonudaTT = new string[] { 
      Faktur.TT_PON,
      Faktur.TT_OPN,
      Faktur.TT_PNM,
   };
   public bool IsPonudaTT { get { return arrayPonudaTT.Contains(TheTT); } }

   #endregion IsPonudaTT

   #region IsDokKolShouldBePrevKolStanjeTT

   private static string[] arrayDokKolShouldBePrevKolStanjeTT = new string[] { 
      Faktur.TT_ZPC,
    //Faktur.TT_ZKC, ... !!! NIJE jer ovo ide po npr. IRM-u, i treba biti kol NE prevKolst nego dokument t_kol 
   };
   public bool IsDokKolShouldBePrevKolStanjeTT { get { return arrayDokKolShouldBePrevKolStanjeTT.Contains(TheTT); } }

   #endregion IsDokKolShouldBePrevKolStanjeTT

   #region IsStornoTT

   private static string[] arrayStornoTT = new string[] { 
      Faktur.TT_IOD,
      Faktur.TT_IPV,
//    Faktur.TT_STI,
      Faktur.TT_UOD,
      Faktur.TT_UPV,
      Faktur.TT_UPM,
//    Faktur.TT_STU,
      Faktur.TT_POV,
    //Faktur.TT_PIK,
    //Faktur.TT_PUK,
   };
   /// <summary>
   /// Ovi TT-ovi u trebaju t_kol tretirati sa minis predznakom
   /// </summary>
   public bool IsStornoTT { get { return arrayStornoTT.Contains(TheTT); } }

   #endregion IsStornoTT

   #region IsInternUI

   // TODO: !!! da li su TT_TMU, TT_TMI, TT_ZPC InterniUI ili ne?! 
   
   private static string[] arrayIsInternUlazTT_fromOneIzlaz = new string[] { 
      Faktur.TT_MSU, 
      Faktur.TT_MPU, 
      Faktur.TT_MMU, 
      Faktur.TT_KUL, 
      Faktur.TT_PUK, 
      Faktur.TT_VMU, 
      Faktur.TT_MVU, 
    //Faktur.TT_PUL, 
      Faktur.TT_AU2,
      Faktur.TT_UG2,
      Faktur.TT_DI2,
      Faktur.TT_PV2,
    //Faktur.TT_PD2,
      Faktur.TT_ZI2,
      Faktur.TT_ZU2,
    //Faktur.TT_MOU,
   };
   /// <summary>
   /// Ovaj TT utjece na FinSt skladista po PrNabCij koja je iskalkulirana po linkanomIzlazu (cijena na Ulaznoj Medjuskladisnici je PrNabCij sa njegove Izlazne Medjusklad. 'sestre'
   /// </summary>
   public bool IsInternUlaz_FromOneIzlaz { get { return arrayIsInternUlazTT_fromOneIzlaz.Contains(TheTT); } } // U biti, tu idu svi vidovi medjuskladisnice - ulazna komponenta 

   private static string[] arrayIsInternUlazTT_fromManyIzlaz = new string[] { 
      Faktur.TT_PUL, 
      Faktur.TT_PUX, 
      Faktur.TT_PUM, 
      Faktur.TT_TRM, // !!! NE ZABORAVI ako dodajes novu proizvodnju ulaz tt u RtransDao.Delete_Then_Renew_Cache_FromThisRtrans_JOB() ... a i VvSql ...!!!
                     // ...dodati taj novi SplitTT pass za REKURZIJU !!!                                                              
      Faktur.TT_RNU,

      Faktur.TT_MOU,

   };
   /// <summary>
   /// Ovaj TT utjece na FinSt skladista po PrNabCij koja je iskalkulirana po SUMI linkanihIzlaza (cijena na Ulaznoj Proizvodnji je suma PrNabCij sa njegovih Izlaznih Proizvodnji.
   /// </summary>
   public bool IsInternUlaz_FromManyIzlaz { get { return arrayIsInternUlazTT_fromManyIzlaz.Contains(TheTT); } } // Proizvodnja ULAZ  

   private static string[] arrayIsInternIzlazTT = new string[] { 
      Faktur.TT_MSI, 
      Faktur.TT_MPI, 
      Faktur.TT_ZIZ, 
      Faktur.TT_ZU2, 
      Faktur.TT_MMI, 
      Faktur.TT_KIZ, 
      Faktur.TT_PIK, 
      Faktur.TT_VMI, 
      Faktur.TT_MVI, 
      Faktur.TT_TRI, 
      Faktur.TT_PIZ, 
      Faktur.TT_PIX, 
      Faktur.TT_PIM, 
      Faktur.TT_IMT, 
    //Faktur.TT_IMM, 
      Faktur.TT_PPR, 
      Faktur.TT_POV, 
      Faktur.TT_MOI,
   };
   public bool IsInternIzlaz { get { return arrayIsInternIzlazTT.Contains(TheTT); } }

#if _PUSE_
   private static string[] arrayIsPrNcPrm_noStTT = new string[] { 
      Faktur.TT_RVU, // ulaz 
      Faktur.TT_IPV, // izlaz 
//    Faktur.TT_STI, // izlaz 
   };
   /// <summary>
   /// Ovaj TT utjece na FinSt skladista po PrNabCij a legalno se pojavljuje i kada je stanje nula. Npr. stanje skl je nula a nama se nesto vraca reversom, povrat od kupca, storno izlazne fakture, povrat konsignacije
   /// </summary>
   public bool IsPrNcPrm_noSt { get { return arrayIsPrNcPrm_noStTT.Contains(TheTT); } }
#endif

   #endregion IsInternUI

   #region IsMalopTT

   private static string[] arrayMalopTT = new string[] { 
      Faktur.TT_PSM,
      Faktur.TT_URM,
      Faktur.TT_UPM,
      Faktur.TT_PNM,
      Faktur.TT_IRM,
      Faktur.TT_IZM,
    //Faktur.TT_IMM,
      Faktur.TT_KLK,
      Faktur.TT_KKM,
      Faktur.TT_NIV,
      Faktur.TT_NUV,
      Faktur.TT_ZPC,
      Faktur.TT_ZKC,
    //Faktur.TT_VMI,
      Faktur.TT_VMU,
      Faktur.TT_MVI,
      Faktur.TT_MMI,
      Faktur.TT_MMU,
      Faktur.TT_TRM,
      Faktur.TT_NRM,
      Faktur.TT_INM,
    //Faktur.TT_NOM,
    //Faktur.TT_BOR,
      Faktur.TT_PIM,
      Faktur.TT_PUM,
   };
   public bool IsMalopTT           { get { return arrayMalopTT.Contains(TheTT); } }
 //public bool IsMalopTTwoIRM      { get { return TheTT != Faktur.TT_IRM && arrayMalopTT.Contains(TheTT); } }

   public bool IsMalopTTorVMIorTRI { get { return arrayMalopTT.Contains(TheTT) || TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI; } }

   // 30.10.2014: 
 //public bool IsForceMalUlazCalc  { get { return (TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI                          ); } }
   // 14.05.2015: idijote
 //public bool IsForceMalUlazCalc  { get { return (TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI || TheTT == Faktur.TT_MVI); } }
 //public bool IsForceMalUlazCalc  { get { return (TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI                          ); } }
   // 19.05.2015: idijoteIdijote
   public bool IsForceMalUlazCalc  { get { return (TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI || TheTT == Faktur.TT_MVI); } }

   private static string[] arrayMalopTT_ULAZ = new string[] { 
      Faktur.TT_PSM,
      Faktur.TT_URM,
      Faktur.TT_UPM,
      Faktur.TT_KLK,
      Faktur.TT_KKM,
      // 04.11.2016: TT_NIV preseljen u arrayMalopTT_IZLAZ
    //Faktur.TT_NIV,
      Faktur.TT_NUV,
      Faktur.TT_ZPC,
    //Faktur.TT_ZKC,
      Faktur.TT_VMU,
      Faktur.TT_MMU,
      Faktur.TT_TRM,
   };
   private static string[] arrayMalopFak2Nal_TT_ULAZ = new string[] { 
      Faktur.TT_URM,
      Faktur.TT_UPM,
      Faktur.TT_UFM,
      Faktur.TT_ZPC,
      Faktur.TT_ZKC, // ?! 
    //Faktur.TT_VMU,
   };
   private static string[] arrayMalopTT_IZLAZ = new string[] { 
      Faktur.TT_IRM,
      Faktur.TT_IZM,
    //Faktur.TT_IMM,
      Faktur.TT_MMI, 
      Faktur.TT_MVI, 

      // 04.11.2016: TT_NIV preseljen iz arrayMalopTT_ULAZ
      Faktur.TT_NIV,
   };
   private static string[] arrayMalopTT_ULAZ_wIZM_wMVI
   {
      get
      {
         List<string> arrayMalopTT_ULAZlist = new List<string>(arrayMalopTT_ULAZ);

         arrayMalopTT_ULAZlist.Add(Faktur.TT_IZM);
         arrayMalopTT_ULAZlist.Add(Faktur.TT_MVI);

         return arrayMalopTT_ULAZlist.ToArray();
      }
   }
 //public bool IsMalopFin_U           { get { return IsFinKol_U && IsMalopTT; } }
 //public bool IsMalopFin_I           { get { return IsFinKol_I && IsMalopTT; } }
   public bool IsMalopFin_U           { get { return arrayMalopTT_ULAZ .Contains(TheTT); } }
   public bool IsMalopFin_UorVMIorTRI { get { return arrayMalopTT_ULAZ .Contains(TheTT) || TheTT == Faktur.TT_VMI || TheTT == Faktur.TT_TRI; } }
   public bool IsMalopFin_I           { get { return arrayMalopTT_IZLAZ.Contains(TheTT); } }

   public bool IsMalopFak2Nal_U { get { return arrayMalopFak2Nal_TT_ULAZ.Contains(TheTT); } }
   public bool IsMalopFak2Nal_I { get { return arrayMalopTT_IZLAZ       .Contains(TheTT); } } // TODO: 'MVI' ?

   public static string UlazniMALOP_IN_Clause      { get { return GetSql_IN_Clause(arrayMalopTT_ULAZ      ); } }
   public static string UlazniMALOP_IN_Clause_wIZM { get { return GetSql_IN_Clause(arrayMalopTT_ULAZ_wIZM_wMVI ); } }
   public static string IzlazniMALOP_IN_Clause     { get { return GetSql_IN_Clause(arrayMalopTT_IZLAZ     ); } }

   public static string UlazniMALOP_Fak2Nal_IN_Clause  { get { return GetSql_IN_Clause(arrayMalopFak2Nal_TT_ULAZ); } }
   public static string IzlazniMALOP_Fak2Nal_IN_Clause { get { return GetSql_IN_Clause(arrayMalopTT_IZLAZ);        } } // za sada jednako kao IzlazniMALOP_IN_Clause 

   #endregion IsMalopTT

   #region IsUlazniPDV

   private static string[] arrayUlazniPdvTT = new string[] { 
      Faktur.TT_URA,
      Faktur.TT_URM,
      Faktur.TT_UFA,
      Faktur.TT_UFM,
      Faktur.TT_UOD,
      Faktur.TT_UPV,
      Faktur.TT_UPM,
      Faktur.TT_UPA,

    //Faktur.TT_WFA,
    //Faktur.TT_WRA,
    //Faktur.TT_WRM,
      Faktur.TT_WRN,
   };
   public bool IsUlazniPdvTT { get { return arrayUlazniPdvTT.Contains(TheTT); } }

   public static string UlazniPdv_IN_Clause { get { return GetSql_IN_Clause(arrayUlazniPdvTT); } }

   public bool IsVirtualPdvTT { get { return TheTT == Faktur.TT_UPA; } }

   public bool Is_WYRN_TT { get { return TheTT == Faktur.TT_WRN || TheTT == Faktur.TT_YRN; } }

   #endregion IsUlazniPDV

   #region IsIzlazniPDV

   private static string[] arrayIzlazniPdvTT = new string[] { 
      Faktur.TT_IRM,
      Faktur.TT_IRA,
      Faktur.TT_IFA,
      Faktur.TT_IOD,
      Faktur.TT_IPV,
    //Faktur.TT_UPA,

    //Faktur.TT_YFA,
    //Faktur.TT_YRA,
    //Faktur.TT_YRM,
      Faktur.TT_YRN,

      Faktur.TT_ZAR,
   };

   public bool IsIzlazniPdvTT { get { return arrayIzlazniPdvTT.Contains(TheTT); } }

   public static string IzlazniPdv_IN_Clause { get { return GetSql_IN_Clause(arrayIzlazniPdvTT                       ); } }
 //public static string IzlazniPdv_IN_Clause { get { return GetSql_IN_Clause(arrayIzlazniPdvTT_W_VirtualPdvTTaddition); } }

   public bool IsPdvTT { get { return (IsUlazniPdvTT || IsIzlazniPdvTT); } }

   public static string[] arrayIzlazniPdvTT_W_VirtualPdvTTaddition
   {
      get
      {
         List<string> theList = new List<string>(arrayIzlazniPdvTT.Length + 1);

         theList.AddRange(arrayIzlazniPdvTT);
         theList.Add(Faktur.TT_UPA);

         return theList.ToArray();
      }
   }

   #endregion IsIzlazniPDV


   #region IsBlagajna

   private static string[] arrayBlagajnaTT = new string[] { 
      Faktur.TT_ISP,
      Faktur.TT_UPL,
      Faktur.TT_BIS,
      Faktur.TT_BUP,
      Faktur.TT_ABU,
      Faktur.TT_ABI,
   };
   public bool IsBlagajnaTT { get { return arrayBlagajnaTT.Contains(TheTT); } }

   public static string Blagajna_IN_Clause { get { return GetSql_IN_Clause(arrayBlagajnaTT); } }

   #endregion IsBlagajna

   #region IsCash2Blagajna

   private static string[] arrayCash2BlagajnaTT = new string[] { 
      Faktur.TT_POT,
      Faktur.TT_URA,
      Faktur.TT_IRA,
      Faktur.TT_ZAR, // 06.02.2026.
   };
   public bool IsCash2BlagajnaTT { get { return arrayCash2BlagajnaTT.Contains(TheTT); } }

   public static string Cash2BlagajnaTT_IN_Clause { get { return GetSql_IN_Clause(arrayCash2BlagajnaTT); } }

   #endregion IsCash2Blagajna

   #region IsProizvodnjaIzlaz

   private static string[] arrayProizvodnjaIzlazTT = new string[] { 
      Faktur.TT_PPR,
      Faktur.TT_POV,
      Faktur.TT_MOI,
   };
   public bool IsProizvodnjaIzlazTT { get { return arrayProizvodnjaIzlazTT.Contains(TheTT); } }

   public static string ProizvodnjaIzlaz_IN_Clause { get { return GetSql_IN_Clause(arrayProizvodnjaIzlazTT); } }

   #endregion IsProizvodnjaIzlaz

   #region IsProjektTT

   /*private*/internal static string[] arrayProjektTT = new string[] { 
      Faktur.TT_RNP,
      Faktur.TT_RNS,
      Faktur.TT_RNM,
      Faktur.TT_PRJ,
      Faktur.TT_BOR,
      Faktur.TT_RNZ,
      Faktur.TT_UGO,
   };
   public bool IsProjektTT { get { return arrayProjektTT.Contains(TheTT); } }

   #endregion IsProjektTT

   #region IsKorekTemTT

   private static string[] arrayKorekTemTT = new string[] { 
      Faktur.TT_TMI,
      Faktur.TT_TMU,
   };
   public bool IsKorekTemTT { get { return arrayKorekTemTT.Contains(TheTT); } }

   #endregion IsKorekTemTT

   #region IsSplitProizvodnja Ulaz/Izlaz

   private static string[] arraySplitProizvodnjaULAZ = new string[] { 
      Faktur.TT_PUL,
      Faktur.TT_PUX,
      Faktur.TT_PUM,
      Faktur.TT_TRM,
      Faktur.TT_RNU,

      Faktur.TT_MOU,
   };
   public bool IsSplitProizvodnjaULAZ { get { return arraySplitProizvodnjaULAZ.Contains(TheTT); } }

   private static string[] arraySplitProizvodnjaIZLAZ = new string[] { 
      Faktur.TT_PIZ,
      Faktur.TT_PIX,
      Faktur.TT_PIM,
      Faktur.TT_TRI,

      Faktur.TT_MOI,
   };
   public bool IsSplitProizvodnjaIZLAZ { get { return arraySplitProizvodnjaIZLAZ.Contains(TheTT); } }

   public bool IsSplitProizvodnjaUI { get { return IsSplitProizvodnjaULAZ || IsSplitProizvodnjaIZLAZ; } }

   #endregion IsSplitProizvodnja Ulaz/Izlaz

   #region IsPrihodTT

   private static string[] arrayPrihodTT = new string[] { 
      Faktur.TT_IFA,
      Faktur.TT_IRA,
      Faktur.TT_IRM,
      Faktur.TT_IOD,
      Faktur.TT_IPV,
      Faktur.TT_POI, //17.06.2025. Tetragram ?????
   };
   /// <summary>
   /// Ovi TT-ovi su REALIZACIJA / PRIHOD 
   /// </summary>
   public bool IsPrihodTT { get { return arrayPrihodTT.Contains(TheTT); } }
   public bool IsPrihodTTorABx { get { return IsPrihodTT || TheTT == Faktur.TT_ABU || TheTT == Faktur.TT_ABI; } }

   public static string Prihod_IN_Clause { get { return GetSql_IN_Clause(arrayPrihodTT); } }

   #endregion IsPrihodTT

   #region IsNotSyncChkPrNabCij_RtransTT

   // ovo treba kasnije nadopuniti po potrebi 
   private static string[] arrayNotSyncChkPrNabCij_RtransTT = new string[] { 
      Faktur.TT_MSI, Faktur.TT_MSU, 
      Faktur.TT_VMI, Faktur.TT_VMU, 
      Faktur.TT_TRI, Faktur.TT_TRM, 
   };
   /// <summary>
   /// Ovi TT-ovi pri ChkPrNabCij mogu promijeniti t_cij
   /// </summary>
   public bool IsNotSyncChkPrNabCij_RtransTT { get { return arrayNotSyncChkPrNabCij_RtransTT.Contains(TheTT); } }

   #endregion IsPrihodTT

   #region IsRashodTT

   private static string[] arrayRashodTT = new string[] { 
      Faktur.TT_URA,
      Faktur.TT_URM,
      Faktur.TT_UFA,
      Faktur.TT_UFM,
      Faktur.TT_UOD,
      Faktur.TT_UPV,
      Faktur.TT_UPM,
      Faktur.TT_UPA,
      Faktur.TT_POT,//17.06.2025. Tetragram
      Faktur.TT_POU,//17.06.2025. Tetragram??????
   };
   /// <summary>
   /// Ovi TT-ovi su ULAZ, trosak / Rashod 
   /// </summary>
   public bool IsRashodTT { get { return arrayRashodTT.Contains(TheTT); } }

   public static string Rashod_IN_Clause { get { return GetSql_IN_Clause(arrayRashodTT); } }

   #endregion IsRashodTT

   #region IsMalopUlazForPrmArtTT

   private static string[] arrayMalopUlazForPrmArtTT = new string[] { 
      Faktur.TT_KLK,
      Faktur.TT_URM,
   };
   public bool IsMalopUlazForPrmArtTT { get { return arrayMalopUlazForPrmArtTT.Contains(TheTT); } }

   public static string MalopUlazForPrmArt_IN_Clause { get { return GetSql_IN_Clause(arrayMalopUlazForPrmArtTT); } }

   private static string[] arraySklad_VelepUlaz_ForPrmArtTT { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_U && tti.IsMalopTT == false).Select(tti => tti.TheTT).ToArray(); } }
   public static string Sklad_VelepUlaz_ForPrmArt_IN_Clause { get { return GetSql_IN_Clause(arraySklad_VelepUlaz_ForPrmArtTT); } }

   private static string[] arraySklad_VelepIzlaz_ForPrmArtTT { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_I && tti.IsMalopTT == false).Select(tti => tti.TheTT).ToArray(); } }
   public static string Sklad_VelepIzlaz_ForPrmArt_IN_Clause { get { return GetSql_IN_Clause(arraySklad_VelepIzlaz_ForPrmArtTT); } }

   private static string[] arraySklad_MalopUlaz_ForPrmArtTT { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_U && tti.IsMalopTT == true).Select(tti => tti.TheTT).ToArray(); } }
   public static string Sklad_MalopUlaz_ForPrmArt_IN_Clause { get { return GetSql_IN_Clause(arraySklad_MalopUlaz_ForPrmArtTT); } }

   private static string[] arraySklad_MalopIzlaz_ForPrmArtTT { get { return ZXC.TtInfoArray.Where(tti => tti.IsFinKol_I && tti.IsMalopTT == true).Select(tti => tti.TheTT).ToArray(); } }
   public static string Sklad_MalopIzlaz_ForPrmArt_IN_Clause { get { return GetSql_IN_Clause(arraySklad_MalopIzlaz_ForPrmArtTT); } }


   #endregion IsMalopUlazForPrmArtTT

   #region IsZavisniTT

   private static string[] arrayZavisniTT = new string[] { 
      Faktur.TT_PRI,
      Faktur.TT_KLK,
    //Faktur.TT_KKM, // NE za sada? 
   };
   /// <summary>
   /// Ovi TT-ovi u trebaju t_kol tretirati sa minis predznakom
   /// </summary>
   public bool IsZavisniTT { get { return arrayZavisniTT.Contains(TheTT); } }

   #endregion IsZavisniTT

   #region IsManualPUcijTT

   private static string[] arrayManualPUcijTT = new string[] {  
      Faktur.TT_PUX,
   };
   /// <summary>
   /// PUX, rucna korekcija ulazne cijene proizvoda 
   /// </summary>
   public bool IsManualPUcijTT { get { return arrayManualPUcijTT.Contains(TheTT); } }

 //private static string[] arrayKoefPUcijTT = new string[] {  
 //   Faktur.TT_TRM,
 //};
 ///// <summary>
 ///// TRU, koeficijentom korigirana ulazna cijene proizvoda 
 ///// </summary>
 //public bool IsKoefPUcijTT { get { return arrayKoefPUcijTT.Contains(TheTT); } }

   #endregion IsManualPUcijTT

   #region IsRadNalPUcijTT

   private static string[] arrayRadNalPUcijTT = new string[] {  
      // 17.11.2016: !!! BIG NEWS !!!                   
      // Raskidamo vezu PPR/PIP cijene                  
      // TT_PIP vise nije 'arrayRadNalPUcijTT'          
      // Za nekog drugog u buducnosti treba onda uvesti 
      // novi set U/I TT-ova                            

      // 25.11.2016: vraćamo! ipak 
      Faktur.TT_PIP,
   };
   /// <summary>
   /// PUX, rucna korekcija ulazne cijene proizvoda 
   /// </summary>
   public bool IsRadNalPUcijTT { get { return arrayRadNalPUcijTT.Contains(TheTT); } }

   #endregion IsRadNalPUcijTT

   #region IsRNMsetTT

   private static string[] arrayRNMsetTT = new string[] {  
      Faktur.TT_RNM,
      Faktur.TT_RNU,
      Faktur.TT_PIP,
      Faktur.TT_PPR,
   };
   /// <summary>
   /// RNM, RNU, PIP, PPR
   /// </summary>
   public bool IsRNMsetTT { get { return arrayRNMsetTT.Contains(TheTT); } }

   private static string[] arrayRNM_Plan_setTT = new string[] {  
      Faktur.TT_RNM,
      Faktur.TT_RNU,
   };
   /// <summary>
   /// RNM, RNU
   /// </summary>
   public bool IsRNM_Plan_setTT { get { return arrayRNM_Plan_setTT.Contains(TheTT); } }

   private static string[] arrayRNM_Realizacija_setTT = new string[] {  
      Faktur.TT_PIP,
      Faktur.TT_PPR,
   };
   /// <summary>
   /// PPR, PIP
   /// </summary>
   public bool IsRNM_Realizacija_setTT { get { return arrayRNM_Realizacija_setTT.Contains(TheTT); } }

   #endregion IsRNMsetTT

   #region IsKomisExtraProdCij

   private static string[] arrayKomisExtraProdCijTT = new string[] { 
      Faktur.TT_KIZ,
   };
   /// <summary>
   /// PUX, rucna korekcija ulazne cijene proizvoda 
   /// </summary>
   public bool IsKomisExtraProdCij { get { return arrayKomisExtraProdCijTT.Contains(TheTT); } }

   #endregion IsKomisExtraProdCij

   #region IsDokDate2TT ... ULAZ po drugacijem datumu. Za sada samo medjuskladisnice ...

   private static string[] arrayDokDate2TT = new string[] { 
      Faktur.TT_MSU,
      Faktur.TT_VMU,
      Faktur.TT_MVU,
      Faktur.TT_MMU,
   };
   public bool IsDokDate2TT { get { return arrayDokDate2TT.Contains(TheTT); } }

   #endregion IsDokDate2TT

   #region IsSplitTTMalULAZ

 //public bool IsSplitTTMalULAZ { get { return IsSplitProizvodnjaULAZ && IsMalopFin_U; } }
   public bool IsSplitTTMalULAZ { get { return TheTT == Faktur.TT_TRI; } }

   #endregion IsSplitTTMalULAZ

   #region  ... PTG ... SPECIALS

   private static string[] array_isV1andV2specialUseTT = new string[] {
      Mixer .TT_KOP, // PCTGO tt 
      Faktur.TT_KUG, // PCTGO tt 
      Faktur.TT_AUN, // PCTGO tt 
      Faktur.TT_UGN, // PCTGO tt 
      Faktur.TT_DIZ, // PCTGO tt 
      Faktur.TT_PVR, // PCTGO tt 
    //Faktur.TT_PVD, // PCTGO tt 
      Faktur.TT_AU2, // PCTGO tt 
      Faktur.TT_UG2, // PCTGO tt 
      Faktur.TT_DI2, // PCTGO tt 
      Faktur.TT_PV2, // PCTGO tt 
    //Faktur.TT_PD2, // PCTGO tt 
      Faktur.TT_ZIZ, // PCTGO tt 
      Faktur.TT_ZI2, // PCTGO tt 
      Faktur.TT_MOD, // PCTGO tt 
   };
   public bool IsV1andV2specialUseTT { get { return ZXC.IsPCTOGOdomena && array_isV1andV2specialUseTT.Contains(TheTT); } }

   //private static string[] array_hasRtranoForSernoTT = new string[] {
   //   Faktur.TT_AUN, // PCTGO tt 
   //   Faktur.TT_UGN, // PCTGO tt 
   //   Faktur.TT_DIZ, // PCTGO tt 
   //   Faktur.TT_PVR, // PCTGO tt 
   // //Faktur.TT_PVD, // PCTGO tt 
   //   Faktur.TT_PRI, // PCTGO tt 
   //   Faktur.TT_IZD, // PCTGO tt 
   //   Faktur.TT_URA, // PCTGO tt 
   //   Faktur.TT_IRA, // PCTGO tt 
   //   Faktur.TT_MPI, // PCTGO tt 
   //   Faktur.TT_ZIZ, // PCTGO tt 
   //   Faktur.TT_ZUL, // PCTGO tt 
   // //Faktur.TT_MOD, // PCTGO tt NE - namjerno 
   //
   //   // tu jos nismo gotovi ... 
   //};
   //public bool HasRtranoForSernoTT { get { return ZXC.IsPCTOGOdomena && array_hasRtranoForSernoTT.Contains(TheTT); } }

   private static string[] array_FakturUgAnTT = new string[] {
      Faktur.TT_UGN, // PCTGO tt 
      Faktur.TT_AUN, // PCTGO tt 
    //Faktur.TT_DIZ, // PCTGO tt 
    //Faktur.TT_PVR, // PCTGO tt 
    //Faktur.TT_PVD, // PCTGO tt 
    //Faktur.TT_ZIZ, // PCTGO tt 
   };

   /*private*/ public static string[] array_FakturDodTT = new string[] {
    //Faktur.TT_UGN, // PCTGO tt 
    //Faktur.TT_AUN, // PCTGO tt 
      Faktur.TT_DIZ, // PCTGO tt 
      Faktur.TT_PVR, // PCTGO tt 
    //Faktur.TT_PVD, // PCTGO tt 
      Faktur.TT_ZIZ, // PCTGO tt 
   };
   public bool IsPTGFaktur_UgAnTT    { get { return ZXC.IsPCTOGOdomena && array_FakturUgAnTT.Contains(TheTT); } }
   public bool IsPTGFaktur_DodTT     { get { return ZXC.IsPCTOGOdomena && array_FakturDodTT.Contains(TheTT); } }
   public bool IsPTGFaktur_UgAnDodTT { get { return IsPTGFaktur_UgAnTT || IsPTGFaktur_DodTT; } }


   /*private*/ public static string[] array_TwinRtransUgAnTT = new string[] {
      Faktur.TT_UG2, // PCTGO tt 
      Faktur.TT_AU2, // PCTGO tt 
   };

   /*private*/ public static string[] array_TwinRtransDodTT = new string[] {
      Faktur.TT_DI2, // PCTGO tt 
      Faktur.TT_PV2, // PCTGO tt 
      Faktur.TT_ZI2, // PCTGO tt 

      Faktur.TT_ZU2, // PCTGO tt 
   };
   public bool IsPTGTwinRtrans_UgAnTT    { get { return ZXC.IsPCTOGOdomena && array_TwinRtransUgAnTT.Contains(TheTT); } }
   public bool IsPTGTwinRtrans_DodTT     { get { return ZXC.IsPCTOGOdomena && array_TwinRtransDodTT.Contains(TheTT); } }
   public bool IsPTGTwinRtrans_UgAnDodTT { get { return IsPTGTwinRtrans_UgAnTT || IsPTGTwinRtrans_DodTT; } }


   private static string[] array_isPTG_YYinTtNum = new string[] {
      Faktur.TT_MOD, // PCTGO tt 
      Faktur.TT_PRI, // PCTGO tt 
      Faktur.TT_URA, // PCTGO tt 
      Faktur.TT_IZD, // PCTGO tt 
      Faktur.TT_MPI, // PCTGO tt 
      Faktur.TT_IRA, // PCTGO tt 
   };
   public bool IsPTG_YYinTtNum_99999 { get { return ZXC.IsManyYearDB && IsPTG_YYinTtNum && IsPrihodTT; } }
   public bool IsPTG_YYinTtNum       { get { return ZXC.IsManyYearDB && array_isPTG_YYinTtNum.Contains(TheTT); } }
   public bool IsPTG_TT              { get { return ZXC.IsPCTOGOdomena && IsPTGFaktur_UgAnDodTT || IsPTG_YYinTtNum || TheTT == Faktur.TT_KUG; } } // 6 + 6 + 1 = 13 PTG TT-ova 
   public bool IsPTG_KUGinTtNum      { get { return ZXC.IsPCTOGOdomena && (IsPTGFaktur_UgAnDodTT && TheTT != Faktur.TT_UGN)/*|| TheTT == Mixer.TT_KOP*/; } } 

   #endregion ... PTG ... SPECIALS

   #region Constructors

   public TtInfo(string _theTT, /*u*/short _ttSort, bool _isSklCdInTtNum, bool _isFinKol_U, bool _isFinKol_I, ZXC.VvSubModulEnum _defaultSubModulEnum) : this(_theTT, _ttSort, _isSklCdInTtNum, _isFinKol_U, _isFinKol_I, _defaultSubModulEnum, false) { }

   public TtInfo(string _theTT, /*u*/short _ttSort, bool _isSklCdInTtNum, bool _isFinKol_U, bool _isFinKol_I, ZXC.VvSubModulEnum _defaultSubModulEnum, bool _isPreDef/*, bool _isKolOnly_U, bool _isKolOnly_I*/) : this() 
   {
      TheTT       = _theTT;
      TtSort      = _ttSort;

      IsFinKol_U  = _isFinKol_U ;
      IsFinKol_I  = _isFinKol_I ;

      IsSklCdInTtNum = _isSklCdInTtNum;
      
      // 30.12.2012: 
      if(ZXC.IsFikalEra && this.IsIzlazniPdvTT && !this.Is_WYRN_TT) // IRM, IRA, IFA, IOD, IPV 
      {
         IsSklCdInTtNum = true /*_isSklCdInTtNum*/;
      }

      IsPreDef = _isPreDef;

      DefaultSubModulEnum =                            (_defaultSubModulEnum);
      DefaultSubModulXY   = ZXC.TheVvForm.GetSubModulXY(_defaultSubModulEnum);

      #region FinKol_PS

      if(TheTT == Faktur.TT_PST || 
         TheTT == Faktur.TT_PSM) IsFinKol_PS = true;

      #endregion FinKol_PS

      #region RezervKol_I

      if(TheTT == Faktur.TT_OPN) IsRezervKol = true;

      #endregion RezervKol_I

      #region Inventura

      if(TheTT == Faktur.TT_INV ||
         TheTT == Faktur.TT_INM) IsInventura = true;

      #endregion Inventura

      #region IsNivelacijaZPC

      if(TheTT == Faktur.TT_ZPC) IsNivelacijaZPC = true;
      if(TheTT == Faktur.TT_ZKC) IsNivelacijaZPC = true;

      #endregion IsNivelacijaZPC

      #region Meduskladisnice - TwinTrans Technology

      if(TheTT == Faktur.TT_MSI) TwinTT          = Faktur.TT_MSU;
      if(TheTT == Faktur.TT_MSU) LinkedDefaultTT = Faktur.TT_MSI;

      if(TheTT == Faktur.TT_MMI) TwinTT          = Faktur.TT_MMU;
      if(TheTT == Faktur.TT_MMU) LinkedDefaultTT = Faktur.TT_MMI;

      if(TheTT == Faktur.TT_VMI) TwinTT          = Faktur.TT_VMU;
      if(TheTT == Faktur.TT_VMU) LinkedDefaultTT = Faktur.TT_VMI;

      if(TheTT == Faktur.TT_MVI) TwinTT          = Faktur.TT_MVU;
      if(TheTT == Faktur.TT_MVU) LinkedDefaultTT = Faktur.TT_MVI;

      if(TheTT == Faktur.TT_KIZ) TwinTT          = Faktur.TT_KUL;
      if(TheTT == Faktur.TT_KUL) LinkedDefaultTT = Faktur.TT_KIZ;

      if(TheTT == Faktur.TT_PIK) TwinTT          = Faktur.TT_PUK;
      if(TheTT == Faktur.TT_PUK) LinkedDefaultTT = Faktur.TT_PIK;

      if(TheTT == Faktur.TT_UGN) TwinTT          = Faktur.TT_UG2;
      if(TheTT == Faktur.TT_UG2) LinkedDefaultTT = Faktur.TT_UGN;
      if(TheTT == Faktur.TT_AUN) TwinTT          = Faktur.TT_AU2;
      if(TheTT == Faktur.TT_AU2) LinkedDefaultTT = Faktur.TT_AUN;
      if(TheTT == Faktur.TT_DIZ) TwinTT          = Faktur.TT_DI2;
      if(TheTT == Faktur.TT_DI2) LinkedDefaultTT = Faktur.TT_DIZ;
      if(TheTT == Faktur.TT_PVR) TwinTT          = Faktur.TT_PV2;
      if(TheTT == Faktur.TT_PV2) LinkedDefaultTT = Faktur.TT_PVR;
    //if(TheTT == Faktur.TT_PVD) TwinTT          = Faktur.TT_PD2;
    //if(TheTT == Faktur.TT_PD2) LinkedDefaultTT = Faktur.TT_PVD;
      if(TheTT == Faktur.TT_MPI) TwinTT          = Faktur.TT_MPU;
      if(TheTT == Faktur.TT_MPU) LinkedDefaultTT = Faktur.TT_MPI;
      if(TheTT == Faktur.TT_ZIZ) TwinTT          = Faktur.TT_ZI2;
      if(TheTT == Faktur.TT_ZI2) LinkedDefaultTT = Faktur.TT_ZIZ;
      if(TheTT == Faktur.TT_ZUL) TwinTT          = Faktur.TT_ZU2;
      if(TheTT == Faktur.TT_ZU2) LinkedDefaultTT = Faktur.TT_ZUL;

      #endregion Meduskladisnice

      #region Proizvodnja - SplitTrans Technology

      if(TheTT == Faktur.TT_PIZ) SplitTT         = Faktur.TT_PUL;
      if(TheTT == Faktur.TT_PUL) LinkedDefaultTT = Faktur.TT_PIZ;

      if(TheTT == Faktur.TT_PIX) SplitTT         = Faktur.TT_PUX;
      if(TheTT == Faktur.TT_PUX) LinkedDefaultTT = Faktur.TT_PIX;

      if(TheTT == Faktur.TT_PIM) SplitTT         = Faktur.TT_PUM;
      if(TheTT == Faktur.TT_PUM) LinkedDefaultTT = Faktur.TT_PIM;

      if(TheTT == Faktur.TT_TRI) SplitTT         = Faktur.TT_TRM;
      if(TheTT == Faktur.TT_TRM) LinkedDefaultTT = Faktur.TT_TRI;

      if(TheTT == Faktur.TT_RNM) SplitTT         = Faktur.TT_RNU;
      if(TheTT == Faktur.TT_RNU) LinkedDefaultTT = Faktur.TT_RNM;

      if(TheTT == Faktur.TT_MOI) SplitTT         = Faktur.TT_MOU;
      if(TheTT == Faktur.TT_MOU) LinkedDefaultTT = Faktur.TT_MOI;

      if(TheTT == Faktur.TT_ZIZ) SplitTT         = Faktur.TT_ZUL;
      if(TheTT == Faktur.TT_ZUL) LinkedDefaultTT = Faktur.TT_ZIZ;

      #endregion Proizvodnja

      #region Implicitne Nivelacije - ShadowTrans Technology

      if(TheTT == Faktur.TT_PSM) ShadowTT = Faktur.TT_NUV;
      if(TheTT == Faktur.TT_KLK) ShadowTT = Faktur.TT_NUV;
      if(TheTT == Faktur.TT_KKM) ShadowTT = Faktur.TT_NUV;
      if(TheTT == Faktur.TT_URM) ShadowTT = Faktur.TT_NUV;
      if(TheTT == Faktur.TT_IRM) ShadowTT = Faktur.TT_NIV;
      if(TheTT == Faktur.TT_IZM) ShadowTT = Faktur.TT_NIV;
    //if(TheTT == Faktur.TT_TRI) ShadowTT = Faktur.TT_NIV; 
      if(TheTT == Faktur.TT_TRM) ShadowTT = Faktur.TT_NUV; 
    //if(TheTT == Faktur.TT_IMM) ShadowTT = Faktur.TT_NIV;
    //if(TheTT == Faktur.TT_MVU) ShadowTT = Faktur.TT_NUV;
      if(TheTT == Faktur.TT_MVI) ShadowTT = Faktur.TT_NIV; // tek od 09.12.2015: 
      if(TheTT == Faktur.TT_MMI) ShadowTT = Faktur.TT_NIV;
      if(TheTT == Faktur.TT_MMU) ShadowTT = Faktur.TT_NUV;

      if(TheTT == Faktur.TT_URA) ShadowTT = Faktur.TT_NUP;

      // observacija od 07.11.2016:
      // TT_VMU namjerno ne ide u 'HasShadowTT' zbog prirode rada 'Pretank'-a 
      // dakle, VMI ne dize implicitnu nivelaciju jer je tako prilagodeno Francuzovom stilu rada 
      // kod TH ioanko je MPC kolona bijela pa niti ne mogu staviti krivu cijenu ... koja bi onda zahtjevala implicitnu nivelaciju 
      // BE ADVICED!!! Kada dode neka nova firma tipa vpsk centrala puca direktno mpsk poslovnicama 
      // tada treba otvoriti novi par VMI/VMU TT-ova koji ce za razliku od Francuz (Pretank only) / TextileHouse (automatska 'bijela' MPC kolona)
      // imati zuti kolonu MPC i ici ce u 'HasShadowTT' da bi po potrebi diglo implicitnu nivelaciju 

      // ...a do tada: namjerno i zauvijek: 
      //if(TheTT == Faktur.TT_VMU) ShadowTT = Faktur.TT_NUV; 

      #endregion Meduskladisnice

      #region Sklad Kol ONLY

      if(TheTT == Faktur.TT_SKU) IsKolOnly_U = true;
      if(TheTT == Faktur.TT_RVU) IsKolOnly_U = true;
      if(TheTT == Faktur.TT_SKI) IsKolOnly_I = true;
      if(TheTT == Faktur.TT_RVI) IsKolOnly_I = true;

      #endregion Sklad Kol ONLY

      #region IsYearInTtNum

      if(IsProjektTT) IsYearInTtNum = true;
      else            IsYearInTtNum = false;

      #endregion IsYearInTtNum

      if(IsFinKol_U == true  && IsFinKol_I == true ) throw new Exception("Ama nemre " + TheTT + " biti i ulaz i izlaz?!");
    //if(IsFinKol_U == false && IsFinKol_I == false) throw new Exception("Ama nemre " + TheTT + " biti niti ulaz niti izlaz?!");
   }

   #endregion Constructors

   #region Util Metodz (GetSql_IN_Clause())

   public static string GetSql_IN_Clause(string[] TTstrings)
   {
      string sql_IN_Clause = "(";

      bool firstPass = true;
      foreach(string theTT in TTstrings)
      {
         sql_IN_Clause += (firstPass ? "'" : ", '") + theTT + "'";
         if(firstPass) firstPass = false;
      }

      sql_IN_Clause += ")";

      return sql_IN_Clause;
   }

   public static string GetSql_IN_Clause_Integer(int[] integers)
   {
      string sql_IN_Clause = "(";

      bool firstPass = true;
      foreach(int integer in integers)
      {
         sql_IN_Clause += (firstPass ? "" : ", ") + integer + "";
         if(firstPass) firstPass = false;
      }

      sql_IN_Clause += ")";

      return sql_IN_Clause;
   }

   #endregion Util Metodz

}

public abstract partial class FakturDUC : VvPolyDocumRecordUC//, Events.Required, Events.Status
{

   #region FakturDUC_Validating & OnExit Set or Validate some fields

   /// <summary>
   /// Opaska od 03.12.2024: nacelno se NE MOZES u validaciji pozivati na bussiness podatke
   /// jer ti se glavni integralni GetFields() poziva (u SaveVvDataRecord()) tek nakon poziva
   /// validacije, pa bi se ovdje uvijek trebalo koristiti present.layer podacima, tj. Fld_ ovima
   /// ISPRAVLJENO ZA 2026! dodan jos jedan GetFields() prije validacije
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   void FakturDUC_Validating(object sender, CancelEventArgs e)
   {
      // HUGE NEWS FROM 2026:                     
      // DON'T VALIDATE BEFORE USER CLICKS 'SAVE' 

      if(!ZXC.RISK_SaveVvDataRecord_inProgress) return;

      // 12.2025: IMAJ NA UMU DA OVDJE NE MOZES RACUNATI NA TOCAN BUSSINESS, JER SE GETFIELDS JOS NIJE MOZDA DOGODIO! 
      // ISPRAVNO BI BILO FLD_ ove KONFRONTIRATI VALIDACIJI A NE BUSSINESSE                                           
      // ISPRAVLJENO ZA 2026! dodan jos jedan GetFields() prije validacije                                            

      FakturExtDUC theExtDUC = this is FakturExtDUC ? this as FakturExtDUC : null;

      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      // Mozda trerba a mozda ne?! 19.1.2011: NE treba! 19.1.2011 lejter that day: DA treba! More lejter NE!!! 
      //GetFields(false);

      bool isWYRN = this is WYRNDUC;

      DateTime serverNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      #endregion Should validate enivej?

      #region IsDocumentFromLockedPeriod

      // 09.02.2016: 

      if(VvDaoBase.IsDocumentFromLockedPeriod(Fld_DokDate.Date, false)) e.Cancel = true;

      #endregion IsDocumentFromLockedPeriod

      #region for petlja: Check Column ArtiklCD, IsInMinus, Primka vs Narudzba Kol, ...

      //SetSifrarAndAutocomplete<Artikl>(vvtbT_artiklCD, VvSQL.SorterType./*Name*/Code);

      string artiklCD;
      decimal /*maxKol = 0,*/ t_kol = 0, t_cij = 0;
      decimal R_CIJ_KCRP;

      bool isThereAnyPPMVartikl = VvUserControl.ArtiklSifrar.Any(art => art.IsPPMV);

      bool isProductLineChecked;

      // zbog provjere ppmv kataloske cijene, prvo prikazi cijenu u kunama 
      if(isThereAnyPPMVartikl && this.IsShowingConvertedMoney)
      {
         this.TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(this, EventArgs.Empty);
      }

      bool isAvansArtiklFound = false;

      Artikl artikl_rec;

      for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
      {
         artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rowIdx, false);
         t_kol = TheG.GetDecimalCell(ci.iT_kol, rowIdx, false);
         t_cij = TheG.GetDecimalCell(ci.iT_cij, rowIdx, false);

         artikl_rec = Get_Artikl_FromVvUcSifrar/*ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD)*/(artiklCD);

         isProductLineChecked = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isProductLine, rowIdx, false));

         #region IsInMinus - PUSE. Na nivou cijelog dolumenta brigu vodi VvForm.CheckForMinus(ZXC.WriteMode writeMode)

         ////if(faktur_rec.TtInfo.IsFinKol_TT && ThisRowWillProduceMinusKolSt(rowIdx, artiklCD, ref maxKol, ref t_kol))
         ////{
         ////   Issue_ArtiklIsInMinus_ErrorMessage(artiklCD, maxKol, t_kol, rowIdx);

         ////   if(MinusPolicyIsStricktly) e.Cancel = true;
         ////}

         #endregion IsInMinus

         #region ArtiklCD does exist

         if(artiklCD.NotEmpty() && ArtiklSifrar.Select(artikl => artikl.ArtiklCD).Contains(artiklCD) == false)
         {
            //DialogResult result = MessageBox.Show("Artikl ne postoji.\n\nRedak: " + (rIdx + 1) + " ArtiklCD: " + artiklCD + "\n\nŽelite li zaista usnimiti ovaj dokument?", "Potvrdite usnimavanje?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if(result != DialogResult.Yes) e.Cancel = true;

            ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl ne postoji.\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
            e.Cancel = true;
         }

         // 28.01.2014: 
         //if(artiklCD.IsEmpty() && faktur_rec.TtInfo.IsFinKol_TT && t_kol.NotZero()                                               )
         if(artiklCD.IsEmpty() && faktur_rec.TtInfo.IsFinKol_TT && t_kol.NotZero() && ZXC.RISK_CopyToOtherDUC_inProgress == false)
         {
            string artikl = TheG.GetStringCell(ci.iT_artiklName, rowIdx, false);

            DialogResult result = MessageBox.Show("Artikl ne postoji.\n\nRedak: " + (rowIdx + 1) + " Artikl: " + artikl + "\n\nŽelite li zaista usnimiti ovaj dokument?", "Potvrdite usnimavanje?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result != DialogResult.Yes) e.Cancel = true;
         }

         // 23.01.2015:
         if(ZXC.IsTEXTHOshop && artiklCD.IsEmpty() && faktur_rec.TtInfo.IsBlagajnaTT == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati redak bez artikla.\n\nŠifra artikla je prazna u retku {0}", rowIdx + 1);
            e.Cancel = true;
         }

         #endregion artiklCD does exist

         #region Check kol: Primka vs Narudzba

         bool isInPrimkaVsNarudzbaMinus;
         decimal primkaKol, narudzbaKol;

         if(ZXC.CURR_prjkt_rec.IsChkPrKol == true && faktur_rec.TT == Faktur.TT_PRI)
         {
            primkaKol = TheG.GetDecimalCell(ci.iT_kol, rowIdx, true);

            isInPrimkaVsNarudzbaMinus = GetIsInPrimkaVsNarudzbaMinus(artiklCD, primkaKol, faktur_rec, out narudzbaKol);

            if(isInPrimkaVsNarudzbaMinus)
            {
               Issue_ArtiklIsInPrimkaVersusNarudzbaMinus_ErrorMessage(artiklCD, narudzbaKol, primkaKol, rowIdx);
               e.Cancel = true;
            }
         }

         #endregion Check kol: Primka vs Narudzba

         #region PPMV kataloska cijena vs Malop Ulaz Cij

         // 16.04.2018: uvodimo i Tembu ppmv ali on ne koristi externi cjenik u xtrans-u    
         //if(                        isThereAnyPPMVartikl && faktur_rec.TtInfo.IsMalopFin_U) 
         if(ZXC.IsTEMBO == false && isThereAnyPPMVartikl && faktur_rec.TtInfo.IsMalopFin_U)
         {
            //Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

            if(artikl_rec != null && artikl_rec.IsPPMV)
            {
               Xtrans xtrans_rec = XtransDao.Get_PPMV_Xtrans(TheDbConnection, artiklCD, faktur_rec.DokDate);
               if(xtrans_rec != null)
               {
                  decimal kataloskaCij = xtrans_rec.T_moneyA;
                  decimal dokMPC = TheG.GetDecimalCell(ci.iT_cij_MSK, rowIdx, false);

                  if(ZXC.AlmostEqual(kataloskaCij, dokMPC, 0.02M) == false)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "Za motorno vozilo [{0} - {1}]\n\r\n\rulazna MPC {2}\n\r\n\rse razlikuje od deklarirane kataloške cijene {3}!",
                        artiklCD, artikl_rec.ArtiklName, dokMPC.ToStringVv(), kataloskaCij.ToStringVv());
                  }
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći katalošku cijenu (na PMV dokumentu) za motorno vozilo [{0} - {1}]!", artiklCD, artikl_rec.ArtiklName);
               }
            }
         }

         #endregion PPMV kataloska cijena vs Malop Ulaz Cij

         #region IRM rtrans brez kolicine

         if(ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && ZXC.IsTEXTHOshop && faktur_rec.TT == Faktur.TT_IRM)
         {
            if(t_kol.IsZero())
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati stavku sa količinom nula.\n\nRedak {0} artikl {1}", rowIdx + 1, artiklCD);
               e.Cancel = true;
            }
         }

         #endregion IRM rtrans brez kolicine

         #region IRM rtrans brez CIJENE

         // 02.01.2018: 
         if(ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && ZXC.IsTEXTHOshop && faktur_rec.TT == Faktur.TT_IRM)
         {
            R_CIJ_KCRP = TheG.GetDecimalCell(ci.iT_cij_kcrp, rowIdx, false);

            if(R_CIJ_KCRP.IsZero())
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati stavku sa cijenom nula.\n\nRedak {0} artikl {1}", rowIdx + 1, artiklCD);
               e.Cancel = true;
            }
         }

         #endregion IRM rtrans brez kolicine

         #region Oblig Artikl From RRD Rules

         // 07.12.2018: 
         //if(ZXC.RRD.Dsc_IsObligArtikl                                          ) // ArtiklCD muss, kol muss, cij muss 
         if(ZXC.RRD.Dsc_IsObligArtikl && faktur_rec.TtInfo.IsInventura == false) // ArtiklCD muss, kol muss, cij muss 
         {
            if(artiklCD.IsEmpty() && faktur_rec.TtInfo.IsBlagajnaTT == false)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati redak bez artikla.\n\nŠifra artikla je prazna u retku {0}", rowIdx + 1);
               e.Cancel = true;
            }
            // 03.12.2019: 
            //if(t_cij.IsZero())
            if(t_cij.IsZero() && faktur_rec.TT != Faktur.TT_NOR && isProductLineChecked == false)
            {
               // 25.01.2021: dozvoljavamo na SvDUH donacijama 
               // 23.06.2022: 
               //bool isSvDUH_donacije = (ZXC.IsSvDUH && faktur_rec.SkladCD == "20");
               bool isSvDUH_donacije = ZXC.IsSvDUH_donSkl(faktur_rec.SkladCD);

               if(isSvDUH_donacije == false && faktur_rec.TT != Faktur.TT_ZAH)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati stavku sa cijenom nula.\n\nRedak {0} artikl {1}", rowIdx + 1, artiklCD);
                  e.Cancel = true;
               }
            }
            if(t_kol.IsZero())
            {
               //bool ipakSmije = (faktur_rec.TT == Faktur.TT_ZAH && faktur_rec.StatusCD == "P");
               bool ipakSmije = (faktur_rec.TT == Faktur.TT_ZAH && faktur_rec.StatusCD == "P") || faktur_rec.TT == Faktur.TT_PST;
               bool neSmije = !ipakSmije;

               if(neSmije)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Račun ne smije sadržavati stavku bez količine.\n\nRedak {0} artikl {1}", rowIdx + 1, artiklCD);
                  e.Cancel = true;
               }
            }
         }

         #endregion Oblig Artikl From RRD Rules

         #region MSI-MSU Roundtrip

         // 29.01.2025: 
       //if(faktur_rec.TT == Faktur.TT_MSI                               )
       //if(faktur_rec.TtInfo.HasTwinTT                                  )
         if(faktur_rec.TtInfo.HasTwinTT && faktur_rec.TT != Faktur.TT_ZIZ)
         {
            DateTime skladDate = Fld_DokDate ;
            string skladCD     = Fld_SkladCD ;
            string skladCD2    = Fld_SkladCD2;
            string msiTT       = Fld_TT      ;

            List<ZXC.VvUtilDataPackage> theTtAndTtNumList;
            ZXC.MySqlCheck_Kind MySqlCheck_Kind = ZXC.MySqlCheck_Kind.M_MsiMsu_Roundtrip;
            bool hasMsiMsuProblem;

            hasMsiMsuProblem =
               // 29.01.2025: 
             //VvDaoBase.MsiMsu_Roundtrip_CheckBefSave(       TheDbConnection, MySqlCheck_Kind, artiklCD, skladDate, skladCD, skladCD2, out theTtAndTtNumList);
               VvDaoBase.MsiMsu_Roundtrip_CheckBefSave(msiTT, TheDbConnection, MySqlCheck_Kind, artiklCD, skladDate, skladCD, skladCD2, out theTtAndTtNumList);
            if(hasMsiMsuProblem)
            {
               string errMessage = VvForm.GetErrMessageList(theTtAndTtNumList);
               ZXC.aim_emsg(MessageBoxIcon.Error, "{0}\n\nJedan te isti artikl [{2}] ne može u jednom danu doći pa se vratiti na-sa istog skladišta.\n\n{3} <---> {4}\n\nIli povrat stavite na drugi datum\n\nili povrat izvršite na prvom MSI-u sa minus količinom.\n\n{1}",
                  MySqlCheck_Kind, errMessage, artiklCD, skladCD, skladCD2);
               e.Cancel = true;
            }

         }

         #endregion MSI-MSU Roundtrip

         #region KPN Check Rbt

         if(ZXC.IsTEXTHOshop && faktur_rec.TT == Faktur.TT_IRM)
         {
            bool is_KPN_IN_TtNum_IS_EMPTY = Fld_V2_ttNum.IsZero();
            bool is_KPN_IN_TtNum_NOT_EMPTY = Fld_V2_ttNum.NotZero();
            bool is_KPN_IN_RbtSt_IS_EMPTY = Fld_Decimal02.IsZero();
            bool is_KPN_IN_RbtSt_NOT_EMPTY = Fld_Decimal02.NotZero();

            if(is_KPN_IN_TtNum_NOT_EMPTY || is_KPN_IN_RbtSt_NOT_EMPTY)
            {
               //Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

               if(artikl_rec != null)
               {
                  decimal usualRbt1St = Get_TH_IRM_Rabat(Fld_SkladCD, Fld_DokDate, artikl_rec);
                  //decimal KPN_Rbt1St  = Fld_Decimal02;
                  decimal theRbt1St = TheG.GetDecimalCell(ci.iT_rbt1St, rowIdx, true);

                  if(theRbt1St != usualRbt1St /*+ KPN_Rbt1St*/)
                  {
                     // 18.12.2018: 
                     //ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {1}: Nekonzistentan rabat ({2}%) kod ZAPRIMANJA KUPONA.\n\nRabat svake stavke treba biti {0}%!", usualRbt1St /*+ KPN_Rbt1St*/, (rowIdx + 1), theRbt1St);
                     //e.Cancel = true;

                     string warningMessage = string.Format("Redak {1}: Nekonzistentan rabat ({2}%) kod ZAPRIMANJA KUPONA.\n\nRabat svake stavke treba biti {0}%!", usualRbt1St.Ron2() /*+ KPN_Rbt1St*/, (rowIdx + 1), theRbt1St.Ron2());
                     DialogResult result = MessageBox.Show
                        (warningMessage + "\n\nDa li još uvijek želite usnimiti ovaj dokument?",
                        "Nekonzistentne stope rabata?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                     if(result != DialogResult.Yes) e.Cancel = true;

                  }
               }
            }
         }

         #endregion KPN Check Rbt

         #region SVD LJEKOVI / POTROSNI rule

         // 26.08.2020: mix happy bday! 
         // roman oce da se ubuduce smiju mijesati L i P artikli po jednoj URA-i, pa smo ovo komentirali 

         if(ZXC.IsSvDUH && this.HasOrgBopCop)
         {
            artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);
         
            if(artikl_rec != null)
            {
               if((this as FakturExtDUC).Fld_PdvZPkind == ZXC.PdvZPkindEnum.SVD_LJEK && !artikl_rec.IsSvdArtGR_Ljek_)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "Artikl nije LIJEK\n\r\n\r{0}", artikl_rec.ArtiklName);
                  //e.Cancel = true;
               }
               if((this as FakturExtDUC).Fld_PdvZPkind == ZXC.PdvZPkindEnum.SVD_POTR && !artikl_rec.IsSvdArtGR_Potr_)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "Artikl nije POTROŠNI\n\r\n\r{0}", artikl_rec.ArtiklName);
                  //e.Cancel = true;
               }
            }
         }

         #endregion SVD LJEKOVI / POTROSNI rule

         #region Artikl is neaktivan, isRashod, izuzet, ...

         if(artikl_rec != null && artikl_rec.IsRashod)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljeni artikl!\n\n(neaktivan ili izuzet)\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
            e.Cancel = true;
         }

         #endregion Artikl is neaktivan, isRashod, izuzet, ...

         #region On save IRA check unlinked OPN
         
         bool shouldCheckOPNlink = ZXC.RRD.Dsc_IsUseOPN && this.CouldClose_OPN && VezaTtNumForTT(Faktur.TT_OPN).IsZero();

         if(shouldCheckOPNlink)
         {
            List<Rtrans> neZatvorenOPNrtransList = RtransDao.Get_OPN_RtransList_For_Artikl_Sklad_And_Kupdob(TheDbConnection, artiklCD, Fld_SkladCD, (this as FakturExtDUC).Fld_KupdobCd);

            neZatvorenOPNrtransList.RemoveAll(rtr => (rtr.T_kol - rtr.T_kol2).IsZero()); // makni zatvorene

            bool hasOPNrtrans = neZatvorenOPNrtransList.NotEmpty();

            if(hasOPNrtrans)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Nevezan OPN dokument za artikl!?\n\nRedak: {0} ArtiklCD: {1}\n\nOPN {2}", (rowIdx + 1), artiklCD, neZatvorenOPNrtransList[0]);
             //ZXC.aim_emsg(MessageBoxIcon.Error  , "Nedozvoljeni artikl!\n\n(neaktivan ili izuzet)\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
             //e.Cancel = true;
            }
         }

         #endregion On save IRA check unlinked OPN

         #region KPD sifra

         // 07.04.2026: 
       //if(artiklCD.NotEmpty() && IsF012DUC &&  faktur_rec.IsF2                                         )
         if(artiklCD.NotEmpty() && IsF012DUC && (faktur_rec.IsF2 || ZXC.CURR_prjkt_rec.F2_ImaSamo_F2_B2B))
         {
            string kpdSifra = TheG.GetStringCell(ci.iT_KPD, rowIdx, false);

          //if(kpdSifra.IsEmpty())
            if(kpdSifra.IsEmpty() && artikl_rec.TS != ZXC.AVA_TS)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "KPD šifra je obavezna na eRačun dokumentima!\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
               e.Cancel = true;
            }
         }

         if(artikl_rec != null && artikl_rec.TS == ZXC.AVA_TS)
         {
            ZXC.aim_emsg(MessageBoxIcon.Information, "Podsjetnik: račun za predujam mora imati eRproc: '4' i Kod rač: '386'");

            isAvansArtiklFound = true;
         }

         #endregion KPD sifra

         #region Tetragram ZAR artikli

         if(ZXC.IsTETRAGRAM_ANY)
         {
            if(this is ZAR_DUC && artikl_rec != null && artikl_rec.TS != "ZAR")
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl nije 'ZAR' artikl!\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
               e.Cancel = true;
            }
          //if(this is ZAR_DUC == false &&                                    this is PON_MPC_DUC == false && artikl_rec != null && artikl_rec.TS == "ZAR")
            if(this is ZAR_DUC == false && this is BlgUplat_M_DUC == false && this is PON_MPC_DUC == false && artikl_rec != null && artikl_rec.TS == "ZAR")
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl je 'ZAR' artikl!\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
               e.Cancel = true;
            }
         }

         #endregion Tetragram ZAR artikli

      } // for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx) 

      #endregion Check Column ArtiklCD, IsInMinus

      #region IsShowingConvertedMoney

      if(IsShowingConvertedMoney) // kod sejvanja, bili u devizama 
      {
         TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
      }
      // 12.10.2016: BIG NEWS ...da se kune kod zaokruzivanja postave na vrijednost koja ce dati deviznu cijenu na cent tocno 
      else if(ArerWeIn_DevizniDokument())// kod sejvanja, bili u kunama 
      {
         TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
         TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
      }

      // if still... 
      if(IsShowingConvertedMoney)
      {
       //ZXC.aim_emsg(MessageBoxIcon.Error, "Prije usnimavanja preracunajte valutu u kune.");
         ZXC.aim_emsg(MessageBoxIcon.Error, "Prije usnimavanja preracunajte valutu u eure.");
         e.Cancel = true;
      }

      #endregion IsShowingConvertedMoney

      #region Check DokDate, SkladDate, T_pdvSt, OIB

      // 08.04.2016: OIB 
      // 19.04.2016: OIB 
      //if(this is FakturExtDUC && faktur_rec.TtInfo.IsPdvTT)
      // 2026: ukidamo ovaj stari nacin provjere oiba za izlazne, qa za ulazne ostqvljamo 
    //if(this is FakturExtDUC && faktur_rec.TtInfo.IsPdvTT       && ZXC.CURR_prjkt_rec.IS_IN_PDV)
      if(this is FakturExtDUC && faktur_rec.TtInfo.IsUlazniPdvTT && ZXC.CURR_prjkt_rec.IS_IN_PDV)
      {
         if((this as FakturExtDUC).Fld_KdOib.IsEmpty())
         {
            if((this as FakturExtDUC).Fld_KupdobCd != ZXC.RRD.Dsc_MalopKCD)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "OIB Partnera ne smije biti prazan!");
               e.Cancel = true;
            }
         }
      }

      // 06.04.2014:
      if(Fld_DokDate.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Datum dokumenta ne smije biti nedefiniran!");
         e.Cancel = true;
      }

      if(isWYRN == false && Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Date < ZXC.projectYearFirstDay) ZXC.aim_emsg(MessageBoxIcon.Warning, "Datum dokumenta: {0} je stariji od prvog dana u radnoj godini!?", Fld_DokDate.ToString(ZXC.VvDateFormat));
      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Date > DateTime.Now.Date) ZXC.aim_emsg(MessageBoxIcon.Warning, "Datum dokumenta: {0} je iz budućnosti!?", Fld_DokDate.ToString(ZXC.VvDateFormat));

      if(this is FakturExtDUC)
      {
         DateTime fld_SkladDate = (this as FakturExtDUC).Fld_SkladDate;

         if(isWYRN == false && fld_SkladDate != DateTimePicker.MinimumDateTime && fld_SkladDate.Date < ZXC.projectYearFirstDay) ZXC.aim_emsg(MessageBoxIcon.Warning, "Sklad Datum: {0} je stariji od prvog dana u radnoj godini!?", (this as FakturExtDUC).Fld_SkladDate.ToString(ZXC.VvDateFormat));
         if(fld_SkladDate != DateTimePicker.MinimumDateTime && fld_SkladDate.Date > DateTime.Now.Date) ZXC.aim_emsg(MessageBoxIcon.Warning, "Sklad Datum: {0} je iz budućnosti!?", (this as FakturExtDUC).Fld_SkladDate.ToString(ZXC.VvDateFormat));

         if(faktur_rec.TtInfo.IsPdvTT && faktur_rec.TrnNonDel.Any(rtrans => rtrans.R_KC.IsZero()))
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) bez iznosa.");
         }

         if(faktur_rec.TtInfo.IsPdvTT && ZXC.CURR_prjkt_rec.IS_IN_PDV &&
            faktur_rec.TrnNonDel.Any(rtrans => rtrans.T_pdvSt.IsZero() && rtrans.T_kol.NotZero()) &&
            ((this as FakturExtDUC).Fld_DevNameAsEnum == ZXC.ValutaNameEnum.EMPTY ||
             (this as FakturExtDUC).Fld_DevNameAsEnum == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum))
         {
            //01.10.2018. ako je projekt auto kuca grupa/Tip = "A" pustamo da moze bez oznake kolone kada je stopa 0
            //if(faktur_rec.TtInfo.IsIzlazniPdvTT && ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.KROVAL                              // KROVAL-a pustamo da ipak moze (30.11.2016) 
            if(faktur_rec.TtInfo.IsIzlazniPdvTT && ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.KROVAL && IsAutoKucaProjekt == false// KROVAL-a pustamo da ipak moze (30.11.2016) 
                                                                                                                                  /*&& faktur_rec.TrnNonDel.Any(rtrans => rtrans.T_pdvColTip == ZXC.PdvKolTipEnum.NIJE && rtrans.T_kol.NotZero())*/) // Validation error 
            {
               foreach(Rtrans rtrans in faktur_rec.TrnNonDel.Where(rtrans => rtrans.T_pdvSt.IsZero() && rtrans.T_kol.NotZero()))
               {
                  if(rtrans.R_isBadPdvColTip_ForPdvStopaZero)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDokument sadrži stavku(e) sa NEDOZVOLJENOM nultom stopom PDV-a.\n\nProvjerite sadržaj 'PK' kolone.");
                     e.Cancel = true;
                  }
               }
            }
            else // Warning only 
            {
               if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.KROVAL) // KROVAL-a pustamo da ipak moze (30.11.2016) 
               {
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa nultom stopom PDV-a.");
               }
            }
         }

         if(faktur_rec.TtInfo.IsPdvTT && ZXC.CURR_prjkt_rec.IS_IN_PDV && faktur_rec./*Pdv*/DokDate < Faktur.NewPdvStopaDate &&
            faktur_rec.TrnNonDel.Any(rtrans => rtrans.T_pdvSt == 25.00M))
         {
            ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa stopom PDV-a 25%.");
            //e.Cancel = true;
         }

         if(faktur_rec.TtInfo.IsPdvTT && ZXC.CURR_prjkt_rec.IS_IN_PDV && faktur_rec./*Pdv*/DokDate >= Faktur.NewPdvStopaDate &&
            faktur_rec.TrnNonDel.Any(rtrans => rtrans.T_pdvSt == 23.00M))
         {
            ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa stopom PDV-a 23%.");
            //e.Cancel = true;
         }

         // Check FISKAL radno vrijeme za IRM, IRA, IFA, IOD, IPV 
         if(faktur_rec.IsFiskalDutyFaktur)
         {
            TimeSpan timeOfDay_RvrOD = ZXC.CURR_prjkt_rec.RvrOd.TimeOfDay;
            TimeSpan timeOfDay_RvrDO = ZXC.CURR_prjkt_rec.RvrDo.TimeOfDay;

            // 25.05.2017: zmjenio da provjera ne ida via Fld_DokDate nego serwerNow varijable 
            // buduci ce tako i onako nize doci Fld_DokDate = serverNow                        
            //TimeSpan timeOfDay_fak = Fld_DokDate.TimeOfDay;
            TimeSpan timeOfDay_fak = serverNow.TimeOfDay;

            if(timeOfDay_fak < timeOfDay_RvrOD || timeOfDay_fak > timeOfDay_RvrDO)
            {
               ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning,
                  "UPOZORENJE:\n\n Vrijeme izdavanja računa [{0}] je izvan deklariranog radnog vremena [{1}]-[{2}]!",
                     Fld_DokDate.ToShortTimeString(),
                     ZXC.CURR_prjkt_rec.RvrOd.ToShortTimeString(),
                     ZXC.CURR_prjkt_rec.RvrDo.ToShortTimeString());
            }
         }

         if(faktur_rec.IsFiskalDutyFaktur_ONLINE && faktur_rec.NacPlac.IsEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nNije definiran način plaćanja!");
            e.Cancel = true;
         }

         // NEGATIVNA MARZA warning 
         if(faktur_rec.TtInfo.IsMalopFin_U && faktur_rec.TrnNonDel.Any(rtrans => rtrans.R_mrzSt.IsNegative()))
         {
            ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa NEGATIVNOM maržom.");
         }

         // NEGATIVNI RUC 
         if(faktur_rec.TtInfo.IsIzlazniPdvTT && faktur_rec.TrnNonDel./*Where(r => r.T_pdvColTip != ZXC.PdvKolTipEnum.GlassOnIRM).*/Any(rtrans => rtrans.R_Ira_RUV.IsNegative()))
         {
            // 26.01.2024: 
          //if(ZXC.RISK_PromjenaNacPlac_inProgress != true)
            if(ZXC.RISK_PromjenaNacPlac_inProgress != true && faktur_rec.Is_STORNO == false)
            {
               ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa NEGATIVNOM zaradom (RUC).");
            }
         }
         // RUC ispod MINIMUMA 
         decimal minimalRUCpoOP = ZXC.RRD.Dsc_MinimalRUC;
         if(minimalRUCpoOP.NotZero() && faktur_rec.TtInfo.IsIzlazniPdvTT && faktur_rec.TrnNonDel./*Where(r => r.T_pdvColTip != ZXC.PdvKolTipEnum.GlassOnIRM).*/Any(rtrans => rtrans.R_Ira_RUV_poOP < minimalRUCpoOP))
         {
            Rtrans rtrans_rec = faktur_rec.TrnNonDel.First(rtrans => rtrans.R_Ira_RUV_poOP < minimalRUCpoOP);
            ZXC.aim_emsg(MessageBoxIcon./*Error*/Warning, "UPOZORENJE:\n\nDokument sadrži stavku(e) sa zaradom (RUC) ispod minimuma od {0} kn.\r\n[{1}] ruc: {2}",
               minimalRUCpoOP.ToStringVv(), rtrans_rec.T_artiklCD, rtrans_rec.R_Ira_RUV_poOP.ToStringVv());
         }
      }

      #endregion Check DokDate, SkladDate

      #region MALOP TT vs MALOP SKLAD_CD

      if(IsIn_TtVsSkladCD_Problem(faktur_rec.TT, faktur_rec.SkladCD))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Tip Transakcije '{0}' nije korektan za skladiste '{1}'", faktur_rec.TT, ZXC.luiListaSkladista.GetNameForThisCd(faktur_rec.SkladCD));
         e.Cancel = true;
      }

      #endregion MALOP TT vs MALOP SKLAD_CD

      #region Is OTS or SVD FinLimit da Kupdob Overflow

      bool shouldCheckKupdob_OTS_FinLimit = false; // TEMBO 
      bool shouldCheckKupdob_SVD_FinLimit = false; // SvDUH 

      Kupdob kupdob_rec = null;

      // 26.03.2018: 
      //if(this is FakturExtDUC                                )
      if(this is FakturExtDUC && faktur_rec.TtInfo.IsFinKol_I)
      {
         kupdob_rec = Get_Kupdob_FromVvUcSifrar((this as FakturExtDUC).Fld_KupdobCd);

         if(kupdob_rec != null)
         {
            shouldCheckKupdob_OTS_FinLimit = kupdob_rec.FinLimit.NotZero() && ZXC.IsSvDUH == false; // TEMBO 
            shouldCheckKupdob_SVD_FinLimit = kupdob_rec.FinLimit.NotZero() && ZXC.IsSvDUH == true; // SvDUH 

            if(ZXC.IsSvDUH && kupdob_rec.IsMtr == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "Partner nije odjel!\n\nZadajte odjel prije usnimavanja!"); e.Cancel = true; }
         }
      }


      if(shouldCheckKupdob_OTS_FinLimit) // should check enivej 
      {
         decimal leftToSpend, kupdobLimit, saldoOTS;

         string theKonto = (this as FakturExtDUC).Fld_Konto;

         if(theKonto.IsEmpty())
         {
            //KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
            theKonto = ZXC.KSD.Dsc_RKto_Kupca;
         }

         List<Ftrans> theFtransList = NalogDao.GetOTS_FtransByTipBrSortedList(TheDbConnection, theKonto, kupdob_rec.KupdobCD, (this as FakturExtDUC).Fld_DokDate);

         saldoOTS = theFtransList.Sum(ftr => ftr.T_dug) - theFtransList.Sum(ftr => ftr.T_pot);

         kupdobLimit = kupdob_rec.FinLimit;

         leftToSpend = kupdobLimit - saldoOTS;

         if(leftToSpend.IsZeroOrNegative()) // saldo is in overflow 
         {
            if(ZXC.CurrUserHasSuperPrivileges == false)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Račun neće biti moguće usnimiti zbog prekoračenja financijskog limita: {0}.\n\nTrenutni dug partnera: {1}\n\nIznos prekoračenja: {2}", kupdobLimit.ToStringVv(), saldoOTS.ToStringVv(), leftToSpend.ToStringVv());

               e.Cancel = true;
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Da nemate SuperUserPrivilegije,\n\nRačun ne bi bilo moguće usnimiti zbog prekoračenja financijskog limita: {0}.\n\nTrenutni dug partnera: {1}\n\nIznos prekoračenja: {2}", kupdobLimit.ToStringVv(), saldoOTS.ToStringVv(), leftToSpend.ToStringVv());
            }
         }
         else // saldo is OK 
         {
            ZXC.aim_emsg(MessageBoxIcon.Information, "Financijski limit: {0}.\n\nTrenutni dug partnera: {1}\n\nPreostalo: {2}", kupdobLimit.ToStringVv(), saldoOTS.ToStringVv(), leftToSpend.ToStringVv());
         }
      }

      if(shouldCheckKupdob_SVD_FinLimit) // should check enivej 
      {
         decimal leftToSpend, kupdobLimit, spendSoFar, spendSoFarPosto;

         ZXC.SVD_PotrosnjaInfo potrosnjaInfo = RtransDao.Get_SVD_PotrosnjaInfo(TheDbConnection, faktur_rec.TT, kupdob_rec, DateTime.MinValue, Fld_DokDate, VvUserControl.KupdobSifrar, false);
         spendSoFar = potrosnjaInfo.AnaUtrosMM;
         spendSoFarPosto = potrosnjaInfo.AnaPostoUtrosMM;

         kupdobLimit = kupdob_rec.FinLimit;

         leftToSpend = kupdobLimit - spendSoFar;

         if(leftToSpend.IsZeroOrNegative()) // saldo is in overflow 
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Prekoračenje financijskog limita od: {0}.\n\nTrenutna potrošnja: {1} ({3}%)\n\nIznos prekoračenja: {2}",
               kupdobLimit.ToStringVv(), spendSoFar.ToStringVv(), leftToSpend.ToStringVv(), spendSoFarPosto.ToString0Vv());
         }
         //else // saldo is OK 
         //{
         //   ZXC.aim_emsg(MessageBoxIcon.Information, "Financijski limit: {0}.\n\nTrenutni dug partnera: {1}\n\nPreostalo: {2}", kupdobLimit.ToStringVv(), spendSoFar.ToStringVv(), leftToSpend.ToStringVv());
         //}
      }

      #endregion Is FinLimit da Prjkt Overflow

      #region Is FISKAL Time elapsed too much? od 02.01.2015: FORCE 'NOW' as DokDateTime

      // provjera dodana TEK 07.01.2016: 
      if(faktur_rec.IsFiskalDutyFaktur_ONLINE && serverNow.Year != ZXC.projectYearFirstDay.Year)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu mijenjati dokumente iz godine koja nije 'tekuća'!");
         e.Cancel = true;
      }

      if(faktur_rec.IsFiskalDutyFaktur_ONLINE && TheVvTabPage.WriteMode == ZXC.WriteMode.Add)
      {
         // 02.01.2015: 

         //TimeSpan tolerancyTS = TimeSpan.FromMinutes(1);
         //
         //DateTime serverNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);
         //
         //if(serverNow - Fld_DokDate > tolerancyTS)
         //{
         //   DialogResult result = MessageBox.Show(
         //      String.Format("Razlika između vremena računa {0} i vremena 'sad' {1}\n\nje čudno velika.\n\nŽelite li postaviti vrijeme računa na {1}?",
         //         Fld_DokDate.ToString(ZXC.VvTimeOnlyFormat), serverNow.ToString(ZXC.VvTimeOnlyFormat)), 
         //      "Potvrdite usnimavanje?!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
         //
         //   switch(result)
         //   {
         //      case DialogResult.Cancel: e.Cancel = true;         break;
         //      case DialogResult.Yes   : Fld_DokDate = serverNow; break;
         //   }
         //
         //   //if(result != DialogResult.No) e.Cancel = true;
         //}

         // 02.01.2015: od ssada ovako: 
         // 06.08.2015: za intervencije u slucaju 'Nekonzistentnost brojeva racuna' 
         //Fld_DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
       //if((ZXC.IsTEXTHOshop && ZXC.CurrUserHasSuperPrivileges                    ) == false) // dakle, samo za superuser@TEXTHOshop se DOZVOLJAVA rucni datum, tj. ovo dole se NE izvodi 
         if((ZXC.IsTEXTHOshop && ZXC.CURR_userName == ZXC.vvDB_programSuperUserName) == false) // dakle, samo za superuser@TEXTHOshop se DOZVOLJAVA rucni datum, tj. ovo dole se NE izvodi 
         {
            // provjera dodana TEK 07.01.2016: 
            if(serverNow.Year != ZXC.projectYearFirstDay.Year) ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu postaviti datum dokumenta na godinu koja nije 'ProjektYear'!");
          //else                                              Fld_DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
            else if(ZXC.RISK_FiskParagon_InProgress == false) Fld_DokDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
         }

         // 03.09.2018: ne daj sejvati racun usera bez oib-a i/ili BadOib-a 
         //string FiskOibOper = TheVvTabPage.TheVvForm.GetFisk_Oib_Oper(faktur_rec.AddUID         ); 
         string FiskOibOper = TheVvTabPage.TheVvForm.GetFisk_Oib_Oper(ZXC.CURR_user_rec.UserName);
         if(ZXC.IsBadOib(FiskOibOper, false))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Greška fiskalizacije:\n\r\n\rUser {0} ima prazan ili netočan OIB!\n\r\n\rIzađite iz programa i prijavite se kao registrirani korisnik sa pravilno unesenim OIB-om.", /*faktur_rec.AddUID*/ZXC.CURR_user_rec.UserName);

            if((ZXC.CURR_user_rec.UserName != ZXC.vvDB_programSuperUserName)) // da superuser ipak moze spremiti IRM kojega je zbog npr. datuma korigirao 
            {
               e.Cancel = true;
            }
         }

      } // if(faktur_rec.IsFiskalDutyFaktur_ONLINE && TheVvTabPage.WriteMode == ZXC.WriteMode.Add)

      #endregion Is FISKAL Time elapsed too much?

      #region Check DokYear vs ZXC.projectYear

    //if(faktur_rec.DokDate.Year != ZXC.projectYearFirstDay.Year && faktur_rec.DokDate.Date != ZXC.prevYearLastDay)
    //if(isWYRN == false &&                                        faktur_rec.DokDate.Year != ZXC.projectYearFirstDay.Year && faktur_rec.DokDate.Date != ZXC.prevYearLastDay && faktur_rec.TtInfo.IsProjektTT == false)
    //if(isWYRN == false && (this is UGNorAUN_PTG_DUC) == false && faktur_rec.DokDate.Year != ZXC.projectYearFirstDay.Year && faktur_rec.DokDate.Date != ZXC.prevYearLastDay && faktur_rec.TtInfo.IsProjektTT == false)
      if(isWYRN == false && (ZXC.IsPCTOGO            ) == false && faktur_rec.DokDate.Year != ZXC.projectYearFirstDay.Year && faktur_rec.DokDate.Date != ZXC.prevYearLastDay && faktur_rec.TtInfo.IsProjektTT == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadali ste nedozvoljenu godinu!");
         e.Cancel = true;
      }

      if(isWYRN == true && faktur_rec.DokDate.Year == ZXC.projectYearFirstDay.Year)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadali ste nedozvoljenu godinu!");
         e.Cancel = true;
      }

      #endregion Check DokYear vs ZXC.projectYear

      #region Check KOMISIJSKO Stuff

      bool isFakturExtDUC = this is FakturExtDUC;
      // 15.05.2014. kada je posl jedinica komisija
      //kupdob_rec            = isFakturExtDUC     ? Get_Kupdob_FromVvUcSifrar((this as FakturExtDUC).Fld_KupdobCd) : null;
      kupdob_rec = isFakturExtDUC ? Get_Kupdob_FromVvUcSifrar((this as FakturExtDUC).Fld_PosJedCd) : null;
      //bool isKupdobKomisija = isFakturExtDUC     ? kupdob_rec.Komisija != ZXC.KomisijaKindEnum.NIJE               : false;
      bool isKupdobKomisija = kupdob_rec != null ? kupdob_rec.Komisija != ZXC.KomisijaKindEnum.NIJE : false;

      // 22.10.2014: 
      bool isKomisijaInUse = VvUserControl.KupdobSifrar.Any(kpdb => kpdb.Komisija != ZXC.KomisijaKindEnum.NIJE);

      VvLookUpItem skladLUI  = ZXC.luiListaSkladista.GetLuiForThisCd(Fld_SkladCD);
      VvLookUpItem sklad2LUI = ZXC.luiListaSkladista.GetLuiForThisCd(Fld_SkladCD2);
      bool isSklKomisija     = skladLUI  == null ? false : isKomisijaInUse ? skladLUI .Integer > 10 : false;
      bool isSkl2Komisija    = sklad2LUI == null ? false : isKomisijaInUse ? sklad2LUI.Integer > 10 : false;

      // 19.05.2017: bikoz TEMBO, overriding isSklKomisija rule 
      if(isSklKomisija && skladLUI.Cd.ToUpper().StartsWith("VPSK")) isSklKomisija = false; // ako SkladCD1 pocne sa 'VPSK' tada NIJE komisija bez obzira na druge obzire 
      if(isSkl2Komisija && sklad2LUI.Cd.ToUpper().StartsWith("VPSK")) isSkl2Komisija = false; // ako SkladCD1 pocne sa 'VPSK' tada NIJE komisija bez obzira na druge obzire 

      if(this is MedjuSkladDUC)
      {
         if(isSklKomisija == true || isSkl2Komisija == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "MSI nije dozvoljen za komisijsko skladište!");
            e.Cancel = true;
         }
      }


      if(this is KIZDUC) // izdatnica u komisiju. 1. Partner MORA biti komisijski partner, 2. skl NE SMIJE biti, 3. skl2 MORA biti komisijsko skladiste 
      {                  // 4. PartnerTicker MORA biti jednak sklCD2 
                         /* 1. */
         if(isKupdobKomisija == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Partner\n\n{0}\n\nNIJE označen kao komisija!", kupdob_rec);
            e.Cancel = true;
         }
         /* 2. */
         if(isSklKomisija == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Polazno skladište\n\n{0}\n\nNE SMIJE biti komisijsko skladište!\n\n(SklBr iznad 10)", skladLUI.Cd);
            e.Cancel = true;
         }
         /* 3. */
         if(isSkl2Komisija == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Dolazno skladište\n\n{0}\n\nNIJE komisijsko skladište!\n\n(SklBr iznad 10)", sklad2LUI.Cd);
            e.Cancel = true;
         }
         /* 4. */
         if(kupdob_rec.Ticker != sklad2LUI.Cd)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Partner\n\n{0}\n\ni Skladište {1}\n\nNISU upareni kao komisija!", kupdob_rec, sklad2LUI);
            e.Cancel = true;
         }

         if(Fld_SkladBR2 < 11 || Fld_SkladBR2 > 99)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Oznaka KOMISIJSKOG (ulaznog) skladišta mora biti između 11 i 99!");
            e.Cancel = true;
         }
      }

      if(this is PIKDUC) // povrat iz komisije. 1. Partner MORA biti komisijski partner, 2. skl MORA biti, 3. skl2 NE SMIJE biti komisijsko skladiste 
      {                  // 4. PartnerTicker MORA biti jednak sklCD 
                         /* 1. */
         if(isKupdobKomisija == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Partner\n\n{0}\n\nNIJE označen kao komisija!", kupdob_rec);
            e.Cancel = true;
         }
         /* 2. */
         if(isSklKomisija == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Polazno skladište\n\n{0}\n\nNIJE komisijsko skladište!\n\n(SklBr iznad 10)", skladLUI.Cd);
            e.Cancel = true;
         }
         /* 3. */
         if(isSkl2Komisija == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Dolazno skladište\n\n{0}\n\nNE SMIJE biti komisijsko skladište!\n\n(SklBr iznad 10)", sklad2LUI.Cd);
            e.Cancel = true;
         }
         /* 4. */
         if(kupdob_rec.Ticker != skladLUI.Cd)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Partner\n\n{0}\n\ni Skladište {1}\n\nNISU upareni kao komisija!", kupdob_rec, skladLUI);
            e.Cancel = true;
         }

         if(Fld_SkladBR/*2*/ < 11 || Fld_SkladBR/*2*/ > 99)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Oznaka KOMISIJSKOG (izlaznog) skladišta mora biti između 11 i 99!");
            e.Cancel = true;
         }
      }

      // Externi dokument a nije KIZ niti PIK (npr. IRA) 
      if(this is FakturExtDUC       == true  && // e.cancel = true: ako skladLUI NEMA zadan opp  
         this is UFADUC             == false && // e.cancel = true: ako je skl komisijsko  a KupdobTK nije jednak sklCD-u 
         this is KIZDUC             == false && // e.cancel = true: ako je skl komisijsko  a KupdobTK nije jednak sklCD-u 
         this is PocetnoStanjeMPDUC == false &&
         this is PocetnoStanjeDUC   == false &&
         this is CjenikKupca_DUC    == false &&
         this is PIKDUC             == false)   // warning only   : ako je Kupdob komisija a KupdobTK nije jednak sklCD-u 
      {
         if(isSklKomisija && skladLUI.Uinteger.IsZero())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Komisijsko skladište\n\n{0}\n\nnema zadanu OPP!", skladLUI.Cd);
            e.Cancel = true;
         }
         if(isSklKomisija == true && (kupdob_rec == null || kupdob_rec.Ticker != skladLUI.Cd))
         {
            if(kupdob_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Za komisijsko skladište\n\n{0}\n\nNEMA komisijskog partnera!", skladLUI.Cd);
            }
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Za komisijsko skladište\n\n{0}\n\n{1}\n\nNIJE komisijski partner!", skladLUI.Cd, kupdob_rec);
            }

            e.Cancel = true;
         }
         if(isKupdobKomisija == true && kupdob_rec.Ticker != skladLUI.Cd)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje:\n\nZa komisijskog partnera\n\n{0}\n\n{1}\n\nNIJE upareno komisijsko skladište.", kupdob_rec, skladLUI.Cd);
            //e.Cancel = true;
         }
      }

      #endregion Check KOMISIJSKO Skladiste Number

      #region IsNpCash but NOT fiskalize

//#if !DEBUG

      if(faktur_rec.IsFiskalDutyFaktur        == true  && // IRM, IRA, IFA, IOD, IPV                                      

         faktur_rec.IsFiskalDutyFaktur_ONLINE == false && // PREMA Prjkt pravilima ovaj TT NE IDE na OnLine Fiskalizaciju 
       //faktur_rec.R_IsNpCashAny             == true)    // ...a zadao je NP 'Novcanice'                                 
         faktur_rec.Is_NacPlac1i2_Cash_Or_Card == true)    // ...a zadao je NP 'Novcanice'                                 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Prema pravilima u Projektu, ovaj dokument NE MOŽE imati ne virmanski način plaćanja (gotovina, kartice, ...)!");

         e.Cancel = true;
      }

//#endif

      // 30.12.2016: 
      if((ZXC.IsTEXTHOshop || ZXC.CURR_prjkt_rec.IsFiskalOnline) &&
         faktur_rec.TT == Faktur.TT_IRM &&
         faktur_rec.NacPlac.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "NEDEFINIRAN način plaćanja!");

         e.Cancel = true;
      }

      #endregion IsNpCash but NOT fiskalize

      #region PIZ check DOUBLE ROLE Artikl ('X' and non'X')

      if(this is ProizvodnjaDUC || this is PIZpDUC || this is TransformDUC)
      {
         string errMsg;
         if((errMsg = GetPIZ_IntersectTT()).NotEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDokument sadrži stavku(e) sa DUPLOM ULOGOM!\n\n" + errMsg);
            e.Cancel = true;
         }
      }

      #endregion PIZ check DOUBLE ROLE Artikl ('X' and non'X')

      #region Check IsNpCash Flag

      if(this is FakturExtDUC)
      {
         string nacPlac = faktur_rec.NacPlac;
         bool isNpCash  = faktur_rec.IsNpCash;

         bool shouldBeIsNpCash = ZXC.luiListaRiskVrstaPl.GetFlagForThisCd(nacPlac);

       //if(isNpCash != shouldBeIsNpCash                                           )
         if(isNpCash != shouldBeIsNpCash && faktur_rec.TtInfo.IsBlagajnaTT == false && !ZXC.IsPCTOGO)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: Nekonzistentna oznaka GOTOVINE (Novčanica) kod Načina Plaćanja!\n\nPromjenite Način Plaćanja, pa ga vratite na željeni.");
            e.Cancel = true;
         }

         // Novododano u 2024: 
         string nacPlac2 = faktur_rec.NacPlac2;
         bool isNpCash2  = faktur_rec.IsNpCash2;

         bool shouldBeIsNpCash2 = ZXC.luiListaRiskVrstaPl.GetFlagForThisCd(nacPlac2);

       //if(isNpCash2 != shouldBeIsNpCash                                            )
       //if(isNpCash2 != shouldBeIsNpCash2 && faktur_rec.TtInfo.IsBlagajnaTT == false)
         if(isNpCash2 != shouldBeIsNpCash2 && faktur_rec.TtInfo.IsBlagajnaTT == false && !ZXC.IsPCTOGO)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: Nekonzistentna oznaka GOTOVINE (Novčanica) kod Načina Plaćanja2!\n\nPromjenite Način Plaćanja, pa ga vratite na željeni.");
            e.Cancel = true;
         }

         // Novododano u 2024: 
         if(false/*faktur_rec.S_ukKCRP_NP1.NotZero() && faktur_rec.S_ukKCRP_NP1 == faktur_rec.S_ukKCRP*/)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: U PRVI način plaćanja je stavljen cijeli iznos računa!\n\nZadajte takav iznos NP1 da bude manji od ukupnog iznosa računa.");
            e.Cancel = true;
         }

         // Novododano u 2024: 
         if(faktur_rec.S_ukKCRP_NP1.NotZero() && Math.Abs(faktur_rec.S_ukKCRP_NP1) > Math.Abs(faktur_rec.S_ukKCRP))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: U PRVI način plaćanja je stavljen iznos koji je veći od iznosa računa!\n\nZadajte takav iznos NP1 da bude manji od ukupnog iznosa računa.");
            e.Cancel = true;
         }

         // Novododano u 2024: 
         if(false/*faktur_rec.R_ukKCRP_NP2.NotZero() && faktur_rec.NacPlac == faktur_rec.NacPlac2*/)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: Nema smisla navoditi iznos druge vrste plaćanja kada su obe vrste plaćanja jednake!\n\nZadajte takav NP2 da bude drukčiji od NP1.");
            e.Cancel = true;
         }

         // Novododano u 2024: 
         if(faktur_rec.S_ukKCRP_NP1.IsZero() && faktur_rec.NacPlac2.NotEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDogodila se greška: Nema smisla navoditi drugu vrstu plaćanja kada je iznos prve prazan!\n\nPobrišite drugu vrstu plaćanja ili zadajte iznos prve vrste plaćanja.");
            e.Cancel = true;
         }
      }

      #endregion Check IsNpCash Flag

      #region Check IRA-2 rtransKOL vs rtranoKOL

      if(this is IRPDUC || this is BORDUC)
      {
         decimal rtransKOL = faktur_rec.TrnSum_K;
         decimal rtranoKOL = faktur_rec.TrnSum2_K;

         if(ZXC.AlmostEqual(rtransKOL, rtranoKOL, 0.02M) == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nSuma količina prve tablice je {0}, a druge {1}!\n\nRazlika: {2}",
               rtransKOL.ToStringVv(), rtranoKOL.ToStringVv(), (rtransKOL - rtranoKOL).ToStringVv());

            if(this is BORDUC) e.Cancel = true;
         }
      }

      if(this is BORDUC)
      {
         var rtransArtiklCDs = faktur_rec.TrnNonDel.Select(rtr => rtr.T_artiklCD).Distinct();
         var rtranoArtiklCDs = faktur_rec.TrnNonDel2.Select(rto => rto.T_artiklCD).Distinct();
         var intersect = rtransArtiklCDs.Intersect(rtranoArtiklCDs);

         if(rtransArtiklCDs.Count() != intersect.Count() ||
            rtranoArtiklCDs.Count() != intersect.Count())
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nArtikli prve i druge tablice nisu usklađeni!\n\nOdaberite 'Grupiraj'.");
            e.Cancel = true;
         }
      }

      #endregion Check IRA-2 rtransKOL vs rtranoKOL

      #region Check Empty Kupdob

    //if(this is FakturExtDUC)
      if(this is FakturExtDUC && ((this is MOD_PTG_DUC) == false))
      {
         FakturExtDUC theFakturExtDUC = this as FakturExtDUC;

         //if(CtrlOK(theFakturExtDUC.tbx_KupdobCd) && faktur_rec.KupdobCD.IsZero() && (faktur_rec.KupdobName + faktur_rec.KupdobTK).IsEmpty())
         if(CtrlOK(theFakturExtDUC.tbx_KupdobCd) && (faktur_rec.KupdobCD.IsZero() || faktur_rec.KupdobName.IsEmpty() || faktur_rec.KupdobTK.IsEmpty()))
         {
            if(this is ZAR_DUC && (faktur_rec.PrimPlatCD.IsZero() || faktur_rec.PrimPlatName.IsEmpty() || faktur_rec.PrimPlatTK.IsEmpty()))
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nMolim, zadajte ili ZAR kupca ili ZAR dobavljača prije usnimavanja.");
               e.Cancel = true;
            }
            else if(!(this is ZAR_DUC))// classic 
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nMolim, zadajte partnera prije usnimavanja.");
               e.Cancel = true;
            }
         }
      }

      #endregion Check Empty Kupdob

      #region TwinTrans Same SkladCD ?

      if(faktur_rec.TT == Faktur.TT_ZIZ) 
      {
         if(faktur_rec.SkladCD == ZXC.PTG_UNJ || faktur_rec.SkladCD2 == ZXC.PTG_UNJ)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "ZAMJENA NE MOŽE imati UNJ skladište u poljima Izadjemo sa ili Vraća se na!");

            e.Cancel = true;
         }
      }
      else if(faktur_rec.TtInfo.HasTwinTT && faktur_rec.SkladCD == faktur_rec.SkladCD2)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Međuskladišnica NE MOŽE imati isto odlazno i dolazno skladište!");

         e.Cancel = true;
      }

      #endregion TwinTrans Same SkladCD ?

      #region TwinTOrSplitrans NO SkladCD2 ?

      if((faktur_rec.TtInfo.HasTwinTT || faktur_rec.TtInfo.HasSplitTT) && faktur_rec.SkladCD2.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Skladište 2 ne smije biti prazno!");

         e.Cancel = true;
      }

      #endregion TwinTOrSplitrans NO SkladCD2 ?

      #region Fiskal Faktur without Rtrans?

      if(ZXC.CURR_userName != ZXC.vvDB_programSuperUserName &&
         ZXC.RISK_SaveVvDataRecord_inProgress && faktur_rec.IsFiskalDutyFaktur && faktur_rec.IsFiskalDutyFaktur_ONLINE &&
         (faktur_rec.TrnNonDel.Count().IsZero() || faktur_rec.S_ukTrnCount.IsZero()))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema smisla usnimiti fiskalni račun bez stavaka.");

         e.Cancel = true;
      }

      #endregion IRM without Rtrans?

      #region INVENTURA stuff 

      if(faktur_rec.TtInfo.IsInventura)
      {
         // 14.12.2016: ne dozvoljavamo TH-u sejvati INV/INM a da nije dan inventure (31.12.2016) 
         //if(ZXC.IsTEXTHOany && faktur_rec.DokDate.Date != ZXC.projectYearLastDay .Date)
         if(ZXC.IsTEXTHOany && faktur_rec.DokDate.Date != ZXC.TexthoInventuraDate.Date)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljen datum dokumenta inventure!");
            e.Cancel = true;
         }

         if(ZXC.IsTEXTHOany && faktur_rec.TrnNonDel.Count(f => f.T_kol.NotZero() || f.T_kol2.NotZero()).IsZero())
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Usnimavate dokument na kojem niste popunili niti jednu kolicinu!");
            //e.Cancel = true;
         }

         List<Artikl> artiklWithStanjeList = new List<Artikl>();
         ArtiklDao.Get_HasKolStOnly_ArtiklWithArtstatList(TheDbConnection, artiklWithStanjeList, faktur_rec.SkladCD, faktur_rec.DokDate, "", "artiklName ");

         #region Pagging 

         // 10.12.2018: Pagging Additions 
         int startRowIdx;
         int rangeCount;
         if(GetPagging_IndexAndCount_FromNapomena(Fld_Napomena, out startRowIdx, out rangeCount))
         {
            artiklWithStanjeList = artiklWithStanjeList.GetRange(startRowIdx, rangeCount).ToList();
         }

         #endregion Pagging 

         List<string> artiklInTroubleList = ArtiklDao.Get_NoInventura_YesKolSt_ArtiklWithArtstatList(artiklWithStanjeList, faktur_rec.Transes);

         int count = 0, maxCount = 16;
         if(artiklInTroubleList.Count.NotZero()) // znaci, IMA problema 
         {
            string errMessage = "UPOZORENJE:\n\n";

            foreach(string artCD in artiklInTroubleList)
            {
               if(++count <= maxCount)
               {
                  errMessage += "Artikl [" + artCD + "] sklad [" + faktur_rec.SkladCD + "]\n";
               }
               else
               {
                  errMessage += "\n... i još [" + (artiklInTroubleList.Count - count + 1).ToString() + "] artikla ...";
                  break;
               }
            }

            //errMessage += "\n\nIma/imaju količinsko stanje a NEMA ga/ih na ovome dokumentu inventure pa NEĆE izaći na izvještajima inventure!\n\nARTIKLE TREBA DODATI NA OVAJ DOKUMENT INVENTURE!";
            errMessage += "\n\nIma/imaju količinsko stanje a NEMA ga/ih na ovome dokumentu inventure pa će izaći na izvještajima inventure implicitno\n\nkao da je stavljen na ovaj dokument sa količinom 0.";

            MessageBox.Show(errMessage, "ARTIKLI KOJE TREBA DODATI NA DOKUMENT INVENTURE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         } // if(badList.Count.NotZero()) // znaci, IMA problema 

         // check duple pojave artikla na INV/INM dokumentu 
         string innerArtiklCD;
         for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
         {
            artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rowIdx, false);

            for(int inIdx = 0; inIdx < TheG.RowCount - 1; ++inIdx)
            {
               innerArtiklCD = TheG.GetStringCell(ci.iT_artiklCD, inIdx, false);

               if(artiklCD == innerArtiklCD && rowIdx < inIdx)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl [{0}] ima više od jedne pojave.\n\nRedak [{1}] i [{2}]\n\nRješite, molim, konflikt prije usnimavanja.", artiklCD, rowIdx + 1, inIdx + 1);
                  e.Cancel = true;
               }

            } // inner loop 

         } // outer loop for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx) 

      } // if(faktur_rec.TtInfo.IsInventura) 

      // 14.12.2016: ne dozvoljavamo TH-u na dan inventure (31.12.2016) ADD-ati ikoje dokumente osim inventurnih 
      //else if(ZXC.IsTEXTHOany && faktur_rec.TtInfo.IsArtiklStatusInfluencer && faktur_rec.DokDate.Date == ZXC.projectYearLastDay .Date) // This is NOT inventura AND is TexthoAny 
      else if(ZXC.IsTEXTHOany && faktur_rec.TtInfo.IsArtiklStatusInfluencer && faktur_rec.DokDate.Date == ZXC.TexthoInventuraDate.Date) // This is NOT inventura AND is TexthoAny 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljen dokument na dan inventure!");

         // 24.01.2017: da ipak bar na centrali mozemo kemijati: 
         if(ZXC.IsTEXTHOcentrala && ZXC.CURR_userName == ZXC.vvDB_programSuperUserName)
         {
            // dakle, mi mozemo 
         }
         else
         {
            e.Cancel = true;
         }
      }

      #endregion INVENTURA stuff

      #region RED document

      // 29.04.2016: pokusaj onemogucavanja nastanka nezeljeno 'crvenih' dokumenata 
      if(!ZXC.AlmostEqual(faktur_rec.S_ukK.Ron2(), faktur_rec.TrnSum_K.Ron2(), /*tolerancy*/0.01M))
      {
         PutTransSumToDocumentSumFields(); // ziheraski, jos jemput 
         GetFields(false);

         if(!ZXC.AlmostEqual(faktur_rec.S_ukK.Ron2(), faktur_rec.TrnSum_K.Ron2(), /*tolerancy*/0.01M))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Prenos suma stavaka u sumu dokumenta nije točan.\n\nPonovite unos količine na prvoj stavci, te usnimite dokument.\n\nS-ukK {0} TrnSum_K {1}", faktur_rec.S_ukK.Ron2(), faktur_rec.TrnSum_K.Ron2());

            e.Cancel = true;
         }
      }

      #endregion RED document

      #region Za UlazPDV_TT VezniDok vec postoji - warning only

      if(faktur_rec.TtInfo.IsUlazniPdvTT && faktur_rec.VezniDok.NotEmpty() && faktur_rec.KupdobCD.NotZero())
      {
         // 09.02.2020: Goga HZTK-a 
         //List<Faktur> fakturList = RtransDao.GetFakturList_VezniDok_KupdobCD_DokDate(TheDbConnection, faktur_rec.VezniDok, faktur_rec.KupdobCD,   faktur_rec.DokDate);
         List<Faktur> fakturList = RtransDao.GetFakturList_VezniDok_KupdobCD_DokDate(TheDbConnection, faktur_rec.VezniDok, faktur_rec.KupdobCD/*, faktur_rec.DokDate*/);

         if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit) fakturList.RemoveAll(fak => fak.RecID == faktur_rec.RecID);

         if(fakturList.Count.NotZero())
         {
            int count = 0, maxCount = 10;
            //string errMessage = "Za ovog partnera na ovaj dan već ima dukument(i) sa istim OrigBrDok [" + faktur_rec.VezniDok + "]\n\n";
            string errMessage = "Za ovog partnera već ima dukument(i) sa istim OrigBrDok [" + faktur_rec.VezniDok + "]\n\n";

            foreach(Faktur faktur in fakturList)
            {
               if(++count <= maxCount)
               {
                  errMessage += faktur + "\n";
               }
               else
               {
                  errMessage += "\n... i još [" + (fakturList.Count - count + 1).ToString() + "] stavaka ...";
                  break;
               }
            }

            //MessageBox.Show(errMessage, "OTKRIVEN JE VEĆ UNESENI OrigBrDok", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            DialogResult result = MessageBox.Show
               (errMessage + "\n\nDa li još uvijek želite usnimiti ovaj dokument?",
               faktur_rec.VezniDok + " JE VEĆ UNESENI OrigBrDok - ponavljanje već unesenog dokumenta?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(result != DialogResult.Yes) e.Cancel = true;
         }
      }

      // 11.07.2022: check duplicate URA/UFA by kcd date money
      // 20.07.2022: da l ise uopce zeli provjeravati i ovo - npr. Rzelu to smeta pa smo stavili pravila
    //if(                                         faktur_rec.TtInfo.IsUlazniPdvTT && faktur_rec.DokDate.NotEmpty() && faktur_rec.KupdobCD.NotZero())
      if(ZXC.RRD.Dsc_NOcheckDupUbyKMD == false && faktur_rec.TtInfo.IsUlazniPdvTT && faktur_rec.DokDate.NotEmpty() && faktur_rec.KupdobCD.NotZero())
      {
         List<Faktur> fakturList = RtransDao.GetFakturList_TT_KupdobCD_DokDate_KCRP(TheDbConnection, faktur_rec.TT, faktur_rec.KupdobCD, faktur_rec.DokDate, faktur_rec.S_ukKCRP);

         if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit) fakturList.RemoveAll(fak => fak.RecID == faktur_rec.RecID);

         if(fakturList.Count.NotZero())
         {
            int count = 0, maxCount = 10;
            //string errMessage = "Za ovog partnera na ovaj dan već ima dukument(i) sa istim OrigBrDok [" + faktur_rec.VezniDok + "]\n\n";
            string errMessage = "Za ovog partnera već ima ulazni dukument(i) sa istim datumom i iznosom?!\n\n";

            foreach(Faktur faktur in fakturList)
            {
               if(++count <= maxCount)
               {
                  errMessage += faktur + "\n";
               }
               else
               {
                  errMessage += "\n... i još [" + (fakturList.Count - count + 1).ToString() + "] stavaka ...";
                  break;
               }
            }

            DialogResult result = MessageBox.Show
               (errMessage + "\n\nDa li još uvijek želite usnimiti ovaj dokument?",
               "Ponavljanje već unesenog dokumenta?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(result != DialogResult.Yes) e.Cancel = true;
         }
      }

      #endregion Za UlazPDV_TT VezniDok vec postoji - warning only

      #region IsPdvDateTooOld - warning only, SvDUH zabrana proslosti

      // 26.03.2018:
    //if(                                                            faktur_rec.TtInfo.IsPdvTT && faktur_rec.PdvR12 == ZXC.PdvR12Enum.R1 && faktur_rec.IsPdvDateTooOld())
      if(ZXC.CURR_prjkt_rec.PdvRTip != ZXC.PdvRTipEnum.NOT_IN_PDV && faktur_rec.TtInfo.IsPdvTT && faktur_rec.PdvR12 == ZXC.PdvR12Enum.R1 && faktur_rec.IsPdvDateTooOld())
      {
         //ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nPDV datum je 'pre star', tj. račun stavljate\n\nu razdoblje za koje je već trebao biti predan PDV obrazac!?");
         string errMessage;
         DateTime dateNow = DateTime.Now;

         //if(dateNow.Day <= 20)
         //{
         errMessage = String.Format("UPOZORENJE:\n\nNa današnji dan {0}\n\nPDV datum {1} je 'pre star',\n\ntj. račun stavljate\n\nu razdoblje za koje je već trebao biti predan PDV obrazac!?\n\n{2} mjesec",
            dateNow.ToString(ZXC.VvDateFormat),
            faktur_rec.PdvDate.ToString(ZXC.VvDateFormat),
            faktur_rec.PdvDate.Month);
         //}
         //else
         //{
         //   errMessage = String.Format("UPOZORENJE:\n\nNa današnji dan {0}\n\nPDV datum {1} je 'pre star',\n\ntj. račun stavljate\n\nu razdoblje za koje je već trebao biti predan PDV obrazac!?\n\n{2} mjesec",
         //      dateNow           .ToString(ZXC.VvDateFormat), 
         //      faktur_rec.PdvDate.ToString(ZXC.VvDateFormat), 
         //      faktur_rec.PdvDate.Month);
         //}

         DialogResult result = MessageBox.Show
            (errMessage + "\n\nDa li još uvijek želite usnimiti ovaj dokument?",
            "PDV datum je 'pre star'?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

         if(result != DialogResult.Yes) e.Cancel = true;
      }

      // 21.07.2022: 
      if(ZXC.IsSvDUH                                        && 
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName &&
         faktur_rec.TT != Faktur.TT_UGO                     &&
         faktur_rec.TT != Faktur.TT_NOR                     &&
         faktur_rec.TT != Faktur.TT_ZAH                      )
      {
         bool isSvduhDokumentTooOld;

         if(serverNow.Day <= 20) // do  20-og u mjesecu jos uvijek mogu u prosli mjesec 
         {
            isSvduhDokumentTooOld = ZXC.MonthDifference(serverNow, faktur_rec.DokDate) > 1; // 0 and 1 monthDiff is ok 
         }
         else // dan mjeseca je veci od 20 ... mogu samo od ovog mjeseca 
         {
            isSvduhDokumentTooOld = ZXC.MonthDifference(serverNow, faktur_rec.DokDate) >= 1; // 0 monthDiff is ok only 
         }

         // 07.09.2022: privremeno! DELLMELATTER! 
         // evo, deletao 
       //if(ZXC.IsSvDUH_donSkl(faktur_rec.SkladCD)) isSvduhDokumentTooOld = false;

         if(isSvduhDokumentTooOld)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDokument je iz 'zaključanog' razdoblja!\n\n[{0}]\n\nPreporuča se mehanizam 'STORNO Dokumenta'", faktur_rec.DokDate.ToString(ZXC.VvDateFormat));

            e.Cancel = true;
         }
      }

      #endregion IsPdvDateTooOld - warning only

      #region TRI izlazArtGR vs ulazArtGR

      // 09.12.2016: 
      if(Faktur.IsProizvCijByArtGr(faktur_rec.TT)) // Faktur.TT_TRI 
      {
         decimal ukFinIzlaz   ;
         decimal ukFinIzlazMPC;
         decimal ukKolUlaz    ;
         decimal ncPerUlKol   ;
         decimal mcPerUlKol   ;
         /*Artikl*/ artikl_rec = null; 

         // ZELENI ULAZ 
         for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
         {
            if(faktur_rec.TtInfo.IsRadNalPUcijTT == false && VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isProductLine, rIdx, false)) == false) continue;

            Rtrans.SetRtransArtiklGrupa1CD(faktur_rec.TrnNonDel         , VvUserControl.ArtiklSifrar);
            Rtrans.SetRtransArtiklGrupa1CD(faktur_rec.TrnNonDel_PULX_ALL, VvUserControl.ArtiklSifrar);

            artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rIdx, /*false*/true);
            if(artiklCD.IsEmpty()) continue;
            artikl_rec = Get_Artikl_FromVvUcSifrar(artiklCD);
            if(artikl_rec == null)
            {
               /* 23.08.2020: dodan if prije emsg */
               if(!e.Cancel) ZXC.aim_emsg(MessageBoxIcon.Error, "Nema artikla [{0}]", artiklCD); ukFinIzlaz = 0.01M;
            }
            else
            {
               ukFinIzlaz    = faktur_rec.TrnNonDel         .Where(r => r.R_grName == artikl_rec.Grupa1CD).Sum(rtrn => rtrn.R_KCR );
               ukFinIzlazMPC = faktur_rec.TrnNonDel         .Where(r => r.R_grName == artikl_rec.Grupa1CD).Sum(rtrn => rtrn.R_KCRP);
               ukKolUlaz     = faktur_rec.TrnNonDel_PULX_ALL.Where(r => r.R_grName == artikl_rec.Grupa1CD).Sum(rtrn => rtrn.T_kol );

               ncPerUlKol = ZXC.DivSafe(ukFinIzlaz   , ukKolUlaz).Ron(4);
               mcPerUlKol = ZXC.DivSafe(ukFinIzlazMPC, ukKolUlaz).Ron(4);
            }
            if(ukFinIzlaz.IsZero())
            {
               /* 23.08.2020: dodan if prije emsg */
               if(!e.Cancel) ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA!\n\nNema financijskog fonda za artikle kategorije\n\n[{0}]\n\n{1}\n\n...nedostaje artikl u 'bijelome' retku,\n\na koji je kategorije [{0}].", artikl_rec.Grupa1CD, artikl_rec);
               e.Cancel = true;
            }

            // 02.01.2018: ne daj TRM-u MPC 0
          //R_CIJ_KCRP = TheG.GetDecimalCell(ci.iT_cij_kcrp, rIdx, false);
            R_CIJ_KCRP = TheG.GetDecimalCell(ci.iT_cij_MSK, rIdx, false);

            if(R_CIJ_KCRP.IsZero())
            {
               /* 23.08.2020: dodan if prije emsg */
               if(!e.Cancel) ZXC.aim_emsg(MessageBoxIcon.Error, "TRI-k ne smije sadržavati 'zelenu' (ulaznu) stavku sa MPC nula.\n\nRedak {0} artikl {1}", rIdx + 1, artiklCD);
               e.Cancel = true;
            }

         } // Check ZELENI ULAZ for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx) 

         // 12.10.2017:  
         // Bijeli IZLAZ 
         for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
         {
            if(faktur_rec.TtInfo.IsRadNalPUcijTT == false && VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isProductLine, rIdx, false)) == true) continue;

            Rtrans.SetRtransArtiklGrupa1CD(faktur_rec.TrnNonDel         , VvUserControl.ArtiklSifrar);
            Rtrans.SetRtransArtiklGrupa1CD(faktur_rec.TrnNonDel_PULX_ALL, VvUserControl.ArtiklSifrar);

            artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rIdx, /*false*/true);
            if(artiklCD.IsEmpty()) continue;
            artikl_rec = Get_Artikl_FromVvUcSifrar(artiklCD);
            if(artikl_rec == null)
            {
               /* 23.08.2020: dodan if prije emsg */
               if(!e.Cancel) ZXC.aim_emsg(MessageBoxIcon.Error, "Nema artikla [{0}]", artiklCD); ukKolUlaz = 0.01M;
            }
            else
            {
               ukKolUlaz = faktur_rec.TrnNonDel_PULX_ALL.Where(r => r.R_grName == artikl_rec.Grupa1CD).Sum(rtrn => rtrn.T_kol );
            }
            if(ukKolUlaz.IsZero())
            {
               /* 23.08.2020: dodan if prije emsg */
               if(!e.Cancel) ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA!\n\nNema produkta koji bi iscrpio fin. fond izlaza za artikle kategorije\n\n[{0}]\n\n{1}\n\n...nedostaje artikl u 'zelenom' retku,\n\na koji je kategorije [{0}].", artikl_rec.Grupa1CD, artikl_rec);
               e.Cancel = true;
            }
         } // Check ZELENI ULAZ for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx) 

      } // if(Faktur.IsProizvCijByArtGr(faktur_rec.TT)) // Faktur.TT_TRI 

      #endregion TRI izlazArtGR vs ulazArtGR

      #region Check RNM ProjektCD

      // 16.05.2018: 
    //if(this is FakturExtDUC && faktur_rec.TT == Faktur.TT_RNM)
      if(this is FakturExtDUC && faktur_rec.TtInfo.IsProjektTT )
      {
         //FakturExtDUC theFakturExtDUC = this as FakturExtDUC;

         if(Fld_ProjektCD != faktur_rec.TT_And_TtNum) // sori, g ko k, ne znam pametnije 
         {
            Fld_ProjektCD = faktur_rec.ProjektCD  = faktur_rec.TT_And_TtNum;
         }

         if(faktur_rec.ProjektCD != faktur_rec.TT_And_TtNum)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nProjektCD != TT_And_TtNum\n\n[{0}] [{1}]", faktur_rec.ProjektCD, faktur_rec.TT_And_TtNum);

            e.Cancel = true;
         }

         if(ThePolyGridTabControl.SelectedTab.Title == "Realizacija")
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nVratite se sa Tab-a 'Realizacija' na Tab 'Nalog' prije usnimavanja.");

            e.Cancel = true;
         }
      }

      #endregion Check Check RNM ProjektCD

      #region Check PIP SkladCD

      if(this is PIPDUC)
      {
         string fakturSkladKonto = Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(faktur_rec.SkladCD);
         string skladGPROkonto   = ZXC.KSD.Dsc_otp_ktoGotProiz;

         if(fakturSkladKonto != skladGPROkonto)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nPIP dokument može ići samo na skladište gotvih proizvoda konto\n\n[{0}]\n\na ne na skladište {2} koje se vodi po kontu\n\n[{1}]", skladGPROkonto, fakturSkladKonto, faktur_rec.SkladCD);

            e.Cancel = true;
         }
      }

      if(this is RNMDUC)
      {
         string fakturSkladKonto = Faktur2NalogRulesAndData.GetSkladKontoForSkladCD(faktur_rec.SkladCD2);
         string skladGPROkonto = ZXC.KSD.Dsc_otp_ktoGotProiz;

         if(fakturSkladKonto != skladGPROkonto)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nRNM dokument može ići samo na skladište gotvih proizvoda konto\n\n[{0}]\n\na ne na skladište {2} koje se vodi po kontu\n\n[{1}]", skladGPROkonto, fakturSkladKonto, faktur_rec.SkladCD2);

            e.Cancel = true;
         }
      }

      if(this is RNZDUC)
      {
         if((this as RNZDUC).Fld_PersonCD .IsZero ()) { ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nZadajte zaštitara prije usnimavanja!"); e.Cancel = true; }
         if((this as RNZDUC).Fld_VezniDok2.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nPartner nema ugovor!"                ); e.Cancel = true; }
      }

      #endregion Check PIP SkladCD

      #region KPN validating (TH Kupon)

      if(ZXC.IsTEXTHOshop && this is IRMDUC)
      {
         IRMDUC theIRMDUC = this as IRMDUC;
         // LIJEVO: IZDAJ 
         // 1. ako je checkirano; nesmije biti prazan niti RbtSt niti ExpDate
         // 2. ako je checkirano; nesmije biti samo vrecica                  
         if(theIRMDUC.cbx_isKpnOUT.Checked == true)
         {
            if(theIRMDUC.Fld_somePercent.IsZero () ||
               theIRMDUC.Fld_PonudDate  .IsEmpty() ||
               theIRMDUC.Fld_RokIspDate .IsEmpty()  )
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Za IZDAVANJE KUPONA potrebno je zadati i iznos popusta i datum valjanosti!");
               e.Cancel = true;
            }

            bool onIRMisVrecicaOnly = faktur_rec.Transes.Any(rtr => rtr.T_artiklCD.StartsWith("VR") == false) == false;

            if(onIRMisVrecicaOnly)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Na računu je samo vrećica.\n\nPrivremeno odznačite pa nakon izdavanja ovog računa vratite oznaku izdavanje kupona.Za IZDAVANJE KUPONA potrebno je zadati još neki artikl osim vrećice!");
               e.Cancel = true;
            }
         }

         // DESNO: ZAPRIMI 
         // ili i jedno i drugo puno ili i jedno i drugo prazno
         // ... + validacija TtNum-a 

         // 09.04.2025: takoder, ako je aktiviran 'slovacki' kupon (taj nije izdan iz Vektora)      
         bool izdanJe20postotniKupon = Fld_V2_ttNum == 20; // 

         bool is_KPN_IN_TtNum_IS_EMPTY  = theIRMDUC.Fld_V2_ttNum .IsZero ();
         bool is_KPN_IN_TtNum_NOT_EMPTY = theIRMDUC.Fld_V2_ttNum .NotZero();
         bool is_KPN_IN_RbtSt_IS_EMPTY  = theIRMDUC.Fld_Decimal02.IsZero ();
         bool is_KPN_IN_RbtSt_NOT_EMPTY = theIRMDUC.Fld_Decimal02.NotZero();

         if((is_KPN_IN_TtNum_NOT_EMPTY || is_KPN_IN_RbtSt_NOT_EMPTY) &&
            (is_KPN_IN_TtNum_IS_EMPTY  || is_KPN_IN_RbtSt_IS_EMPTY)  &&
            izdanJe20postotniKupon == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Za ZAPRIMANJE KUPONA potrebno je zadati i broj kupona (broj računa) i iznos popusta!");
            e.Cancel = true;
         }

         if((is_KPN_IN_TtNum_NOT_EMPTY || is_KPN_IN_RbtSt_NOT_EMPTY))
         {
            if(theIRMDUC.Is_TH_KPN_IN_TtNum_Invalid())
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Pogrešno upisan broj zaprimljenog kupona (računa)!\n\nNema 7 znamenaka!\n\nIspravni primjeri:1400001, 7601234, 2000001...\n\nNajmanji mogući broj: 1400001\n\nPrepišite, molim, broj kupona točno od znamenke do znamenke.");
               e.Cancel = true;
            }
         }

         // 22.05.2023: 'e nece rabat ici bezuvjetno!'            
         // 27.02.2024: 'e bas hoce rabat ici bezuvjetno!'        
         // 11.03.2024: 'vracamo uvjet ali osim u nekom periodu!' 
         // 20.03.2025: dodajemo bool isKupon                     
       //if(false)
       //if(TexthoRabatIsNeopravdan(                                                          theIRMDUC.Fld_SkladCD, theIRMDUC.Fld_S_ukRbt1, theIRMDUC.Fld_S_ukKC, /*theIRMDUC.Fld_S_ukK*/ faktur_rec.Transes.Where(rtr => rtr.T_artiklCD.StartsWith("VR") == false).ToList().Sum(rtr => rtr.T_kol), theIRMDUC.Fld_DokDate))
         if(TexthoRabatIsNeopravdan((is_KPN_IN_TtNum_NOT_EMPTY && is_KPN_IN_RbtSt_NOT_EMPTY), theIRMDUC.Fld_SkladCD, theIRMDUC.Fld_S_ukRbt1, theIRMDUC.Fld_S_ukKC, /*theIRMDUC.Fld_S_ukK*/ faktur_rec.Transes.Where(rtr => rtr.T_artiklCD.StartsWith("VR") == false).ToList().Sum(rtr => rtr.T_kol), theIRMDUC.Fld_DokDate))
         {
            DialogResult result = MessageBox.Show("Nije ostvaren uvjet za odobravanje rabata!\n\r\n\rŽelite li poništiti rabat?\n\r\n\rDA - poništi rabat i usnimi račun\n\rNE - vrati se na unos računa da bi ga dopunio.)",
               "Nije ostvaren uvjet za odobravanje rabata!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
         
            if(result != DialogResult.Yes)  // "NE" - vrati me na račun 
            {
               e.Cancel = true;
            }
            else // "DA" - ponisti rabat i usnimi racun 
            {
               decimal newRabat = 0.00M;
         
               for(int rIdx = 0; rIdx < theIRMDUC.TheG.RowCount - 1; ++rIdx)
               {
                  theIRMDUC.TheG.PutCell(theIRMDUC.DgvCI.iT_rbt1St, rIdx, newRabat);
               }
         
               theIRMDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
            }
         }

      } // if(ZXC.IsTEXTHOshop && this is IRMDUC)

      #endregion KPN validating (TH Kupon)

      #region Check Dupla Pojava Artikla ... tamo gdje to smeta 

      if(ZXC.IsSvDUH && this is UGODUC) // ovo vec ima i u INV/INM regiji! 
      {
         // check duple pojave artikla na INV/INM dokumentu 
         string innerArtiklCD;
         for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
         {
            artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rowIdx, false);

            for(int inIdx = 0; inIdx < TheG.RowCount - 1; ++inIdx)
            {
               innerArtiklCD = TheG.GetStringCell(ci.iT_artiklCD, inIdx, false);

               if(artiklCD == innerArtiklCD && rowIdx < inIdx)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl [{0}] ima više od jedne pojave.\n\nRedak [{1}] i [{2}]\n\nRješite, molim, konflikt prije usnimavanja.", artiklCD, rowIdx + 1, inIdx + 1);
                  e.Cancel = true;
               }

            } // inner loop 

         } // outer loop for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx) 
      }

      if(ZXC.IsSvDUH && this is URA_SVD_DUC && Fld_ProjektCD.NotEmpty()) // check URA kupdob vs UGO kupdob, URA dokDate vs UGO period 
      {
         Ftrans.ParseTipBr(Fld_ProjektCD, out string ugoTt, out uint ugoTtNum); // primjer inlined variable daclaration-a 

         if(ugoTt.NotEmpty() && ugoTtNum.NotZero())
         {
            Faktur UGOfaktur_rec = new Faktur();

            bool OK = FakturDao.SetMeFaktur(TheDbConnection, UGOfaktur_rec, ugoTt, ugoTtNum, false);

            if(!OK)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji ugovor!\n\n{0}", Fld_ProjektCD);
               e.Cancel = true;
            }
            else if(UGOfaktur_rec.KupdobCD != (this as FakturExtDUC).Fld_KupdobCd)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Dobavljač URA računa se ne podudara sa dobavljačem UGO ugovora!\n\nURA: {0}\n\nUGO: {1}", (this as FakturExtDUC).Fld_KupdobName, UGOfaktur_rec.KupdobName);
               e.Cancel = true;
            }
            else if((this as FakturExtDUC).Fld_DokDate < UGOfaktur_rec.DokDate ||
                    (this as FakturExtDUC).Fld_DokDate > UGOfaktur_rec.DospDate)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Datum URA računa je izvan perioda Ugovora!?\n\nURA: {0}\n\nUGO: {1} - {2}", 
                  (this as FakturExtDUC).Fld_DokDate.ToString(ZXC.VvDateFormat), UGOfaktur_rec.DokDate.ToString(ZXC.VvDateFormat), UGOfaktur_rec.DospDate.ToString(ZXC.VvDateFormat));
               //e.Cancel = true;
            }
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji ugovor!\n\n{0}", Fld_ProjektCD);
            e.Cancel = true;
         }

      }

      if(ZXC.IsSvDUH && Fld_SkladCD.NotEmpty())
      {
         // 20.06.2022: 
         // ovo bi, mozda, trebalo ugasiti nakon sto smo uveli da je 
         // ZXC.luiListaSkladista postala IsYearDependent = true     

         // 25.04.2023: pa smo to sad i napravili 
       //if(!SVD_RptLine.SVD_LegalSkl2022.Contains(Fld_SkladCD))
       //{
       //   ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljeno skladište! Skl: [{0}]", Fld_SkladCD);
       //   e.Cancel = true;
       //}
      }

      #endregion Check Dupla Pojava Artikla ... tamo gdje to smeta 

      #region PCTOGO Validations

      if(ZXC.IsPCTOGO)
      {
         #region Rtrano validacije

         int rowIdx2 = 0;

         if(this is FakturPDUC)
         {
            FakturPDUC thePduc = this as FakturPDUC;
            FakturPDUC.Rtrano_colIdx ci2 = thePduc.DgvCI2;

            Rtrano rtrano_rec;

            if(this is MOD_PTG_DUC)
            {
               MOD_PTG_DUC theDUC = (MOD_PTG_DUC)this;

               #region Check RAM/HDD kind balance

               #region Get Lists 

               Artikl oldArtikl_rec;

               string oldArtiklCD;

               //List<Rtrano> rtranoList = faktur_rec.TrnNonDel2_ALL.ToList();
               theDUC.Get_RtranoDgvList();

               foreach(Rtrano rto in theDUC.RtranoDgvList)
               {
                  rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

                  artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

                  oldArtiklCD = TheG2.GetStringCell(ci2.iR_artiklCD_Old, rowIdx2, false);

                  oldArtikl_rec = Get_Artikl_FromVvUcSifrar(oldArtiklCD);

                  if(oldArtikl_rec != null)
                  {
                     rto.R_RAM_kind = oldArtikl_rec.Grupa2CD;
                     rto.R_HDD_kind = oldArtikl_rec.Grupa3CD;
                  }

                  ++rowIdx2;
               }

               List<string> RAM_kinds = theDUC.RtranoDgvList.Select(rto => rto.R_RAM_kind).Distinct().ToList();
               List<string> HDD_kinds = theDUC.RtranoDgvList.Select(rto => rto.R_HDD_kind).Distinct().ToList();

               // MOI minusi ________________________________________________________________________________________________________________________________________________________________________________________________
               List<Rtrano> MOIrtranoList = theDUC.RtranoDgvList.Where(rto => rto.T_TT == Faktur.TT_MOI).ToList();

               List<VvReportSourceUtil> MOI_RAMkindSumsList = MOIrtranoList.GroupBy(R => R.R_RAM_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_dimY) }).ToList();
               List<VvReportSourceUtil> MOI_HDDkindSumsList = MOIrtranoList.GroupBy(R => R.R_HDD_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_decB) }).ToList();

               // MOU plusevi _______________________________________________________________________________________________________________________________________________________________________________________________
               List<Rtrano> MOUrtranoList = theDUC.RtranoDgvList.Where(rto => rto.T_TT == Faktur.TT_MOU).ToList();

               List<VvReportSourceUtil> MOU_RAMkindSumsList = MOUrtranoList.GroupBy(R => R.R_RAM_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_dimX) }).ToList();
               List<VvReportSourceUtil> MOU_HDDkindSumsList = MOUrtranoList.GroupBy(R => R.R_HDD_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_decA) }).ToList();

               // MOCS minusi _______________________________________________________________________________________________________________________________________________________________________________________________
               List<Rtrano> MOCSrtranoList = theDUC.RtranoDgvList.Where(rto => rto.T_TT == Faktur.TT_MOC || rto.T_TT == Faktur.TT_MOS).ToList();

               List<VvReportSourceUtil> MOCS_MINUS_RAMkindSumsList = MOCSrtranoList.GroupBy(R => R.R_RAM_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_dimY) }).ToList();
               List<VvReportSourceUtil> MOCS_MINUS_HDDkindSumsList = MOCSrtranoList.GroupBy(R => R.R_HDD_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_decB) }).ToList();

               // MOCS plusevi ______________________________________________________________________________________________________________________________________________________________________________________________
               List<VvReportSourceUtil> MOCS_PLUS_RAMkindSumsList = MOCSrtranoList.GroupBy(R => R.R_RAM_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_dimX) }).ToList();
               List<VvReportSourceUtil> MOCS_PLUS_HDDkindSumsList = MOCSrtranoList.GroupBy(R => R.R_HDD_kind).Select(grp => new VvReportSourceUtil { ArtiklGrCD = grp.Key, Count = grp.Sum(R => (int)R.T_decA) }).ToList();

               #endregion Get Lists 

               bool warnOnly_RAM = true; // mozda todo, ako ipak odlucimo NE dat im sejvat 
               bool warnOnly_HDD = true;

               // RAM balance checks 
               foreach(string RAM_kind in RAM_kinds)
               {
                  int RAM_MOCS_plus = MOCS_PLUS_RAMkindSumsList.Where(sume => sume.ArtiklGrCD == RAM_kind).Sum(suma => suma.Count);
                  int RAM_MOCS_minus = MOCS_MINUS_RAMkindSumsList.Where(sume => sume.ArtiklGrCD == RAM_kind).Sum(suma => suma.Count);
                  int RAM_MOU_plus = MOU_RAMkindSumsList.Where(sume => sume.ArtiklGrCD == RAM_kind).Sum(suma => suma.Count);
                  int RAM_MOI_minus = MOI_RAMkindSumsList.Where(sume => sume.ArtiklGrCD == RAM_kind).Sum(suma => suma.Count);

                  int RAM_MOCS_saldo = RAM_MOCS_plus - RAM_MOCS_minus;
                  int RAM_MOUI_saldo = RAM_MOI_minus - RAM_MOU_plus;

                  bool has_RAM_MOCS_MOUI_saldo_descrepancy = RAM_MOCS_saldo != RAM_MOUI_saldo; // do ovoga smo teškom mukom došli :-( 

                  MessageBoxIcon messageBoxIcon = warnOnly_RAM ? MessageBoxIcon.Warning : MessageBoxIcon.Error;

                  if(has_RAM_MOCS_MOUI_saldo_descrepancy)
                  {
                     int RAMplus = RAM_MOCS_plus + RAM_MOU_plus;
                     int RAMminus = RAM_MOCS_minus + RAM_MOI_minus;

                     ZXC.aim_emsg(messageBoxIcon, "Za RAM " + RAM_kind + " nije uspostavljena plus/minus SALDO ravnoteža " + RAMplus + " ≠ " + RAMminus);
                     if(warnOnly_RAM == false)
                     {
                        e.Cancel = true;
                        return;
                     }
                  }

                  bool has_RAM_MOCS_plus_MOI_minus_descrepancy = RAM_MOCS_plus != RAM_MOI_minus; // do ovoga smo teškom mukom došli :-( 
                  bool has_RAM_MOCS_minus_MOU_plus_descrepancy = RAM_MOCS_minus != RAM_MOU_plus; // do ovoga smo teškom mukom došli :-( 

                  if(has_RAM_MOCS_plus_MOI_minus_descrepancy)
                  {
                     ZXC.aim_emsg(messageBoxIcon, "Za RAM " + RAM_kind + " nije uspostavljena MOCS_plus/MOI_minus ravnoteža " + RAM_MOCS_plus + " ≠ " + RAM_MOI_minus);
                     if(warnOnly_RAM == false)
                     {
                        e.Cancel = true;
                        return;
                     }
                  }

                  if(has_RAM_MOCS_minus_MOU_plus_descrepancy)
                  {
                     ZXC.aim_emsg(messageBoxIcon, "Za RAM " + RAM_kind + " nije uspostavljena MOU_plus/MOCS_minus " + RAM_MOU_plus + " ≠ " + RAM_MOCS_minus);
                     if(warnOnly_RAM == false)
                     {
                        e.Cancel = true;
                        return;
                     }
                  }

               } // foreach(string RAM_kind in RAM_kinds) 

               // HDD balance checks 
               foreach(string HDD_kind in HDD_kinds)
               {
                  int HDD_MOCS_plus = MOCS_PLUS_HDDkindSumsList.Where(sume => sume.ArtiklGrCD == HDD_kind).Sum(suma => suma.Count);
                  int HDD_MOCS_minus = MOCS_MINUS_HDDkindSumsList.Where(sume => sume.ArtiklGrCD == HDD_kind).Sum(suma => suma.Count);
                  int HDD_MOU_plus = MOU_HDDkindSumsList.Where(sume => sume.ArtiklGrCD == HDD_kind).Sum(suma => suma.Count);
                  int HDD_MOI_minus = MOI_HDDkindSumsList.Where(sume => sume.ArtiklGrCD == HDD_kind).Sum(suma => suma.Count);

                  int HDD_MOCS_saldo = HDD_MOCS_plus - HDD_MOCS_minus;

                  bool has_HDD_descrepancy = HDD_MOCS_saldo != (HDD_MOI_minus - HDD_MOU_plus); // do ovoga smo teškom mukom došli :-( 

                  MessageBoxIcon messageBoxIcon = warnOnly_HDD ? MessageBoxIcon.Warning : MessageBoxIcon.Error;

                  if(has_HDD_descrepancy)
                  {
                     int HDDplus = HDD_MOCS_plus + HDD_MOU_plus;
                     int HDDminus = HDD_MOCS_minus + HDD_MOI_minus;

                     ZXC.aim_emsg(messageBoxIcon, "Za HDD " + HDD_kind + " nije uspostavljena plus/minus ravnoteža " + HDDplus + " ≠ " + HDDminus);
                     if(warnOnly_HDD == false)
                     {
                        e.Cancel = true;
                        return;
                     }
                  }

               } // foreach(string HDD_kind in HDD_kinds) 

               #endregion Check RAM/HDD kind balance

               #region ADD new MOC/MOS Artikl, check MOC RAM/HDD kind

               List<string> new_MOC_MOS_ArtiklCDlist = new List<string>();

               for(rowIdx2 = 0; rowIdx2 < TheG2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx2)
               {
                  rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

                  artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

                  oldArtiklCD = TheG2.GetStringCell(ci2.iR_artiklCD_Old, rowIdx2, false);

                  // ne daj prazni MOC/MOS NewArtiklCD 
                  if(rtrano_rec.TtInfo.Is_MOC_or_MOS_TT && rtrano_rec.T_artiklCD.IsEmpty())
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "MOC / MOS stavka mora imati definirani 'Artikl NEW'.\n\nRedak {0} Serijski broj {1}", rowIdx2 + 1, rtrano_rec.T_serno);
                     e.Cancel = true;
                     return;
                  }

                  // ne daj prazni OldArtiklCD ako je zadan t_serno 
                  if(rtrano_rec.T_serno.NotEmpty() && oldArtiklCD.IsEmpty())
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "Ne smije se zadati novi serijski broj a da ga se ne upari sa artiklom.\n\nRedak {0} Serijski broj {1}", rowIdx2 + 1, rtrano_rec.T_serno);
                     e.Cancel = true;
                     return;
                  }

                  #region check MOC RAM/HDD kind 

                  string mocRAMkind = theDUC.Fld_PTG_RamKlasa;
                  string mocHDDkind = theDUC.Fld_PTG_HddKlasa;
                  string rtoRAMkind = /*artikl_rec.Grupa2CD    */TheG2.GetStringCell(ci2.iT_ramKlasa, rowIdx2, false);
                  string rtoHDDkind = /*artikl_rec.Grupa3CD    */TheG2.GetStringCell(ci2.iT_hddKlasa, rowIdx2, false);

                  bool ramChanged = (rtrano_rec.T_dimX + rtrano_rec.T_dimY).NotZero();
                  bool hddChanged = (rtrano_rec.T_decA + rtrano_rec.T_decB).NotZero();

                  bool isMocDefined = theDUC.Fld_PTG_MOC_PCK_ArtCD.NotEmpty();

                  if(isMocDefined && ramChanged && mocRAMkind != rtoRAMkind)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "RAM klasa {2} cilja modifikacije ne odgovara RAM klasi {3} stavke.\n\nArtikl [{0}] Redak {1}", oldArtiklCD, rowIdx2 + 1, mocRAMkind, rtoRAMkind);
                     e.Cancel = true;
                     /*break;*/
                     return;
                  }
                  if(isMocDefined && hddChanged && mocHDDkind != rtoHDDkind)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "HDD klasa {2} cilja modifikacije ne odgovara HDD klasi {3} stavke.\n\nArtikl [{0}] Redak {1}", oldArtiklCD, rowIdx2 + 1, mocHDDkind, rtoHDDkind);
                     e.Cancel = true;
                     /*break;*/
                     return;
                  }

                  #endregion check MOC RAM/HDD kind 

                  // dodaj nove MOC/MOS Artikle 
                  if(rtrano_rec.TtInfo.Is_MOC_or_MOS_TT && rtrano_rec.T_artiklCD.NotEmpty())
                  {
                     string newArtiklCD = rtrano_rec.T_artiklCD;

                     if(!e.Cancel && Get_Artikl_FromVvUcSifrar(newArtiklCD) == null)
                     {
                        bool addnewOK = true;
                        string newArtiklName;

                        Artikl MOC_MOS_OLD_artikl_rec = Get_Artikl_FromVvUcSifrar(TheG2.GetStringCell(ci2.iR_artiklCD_Old, rowIdx2, false));

                        if(MOC_MOS_OLD_artikl_rec == null) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak {1} OLD Artikl ne postoji!? [{0}]", MOC_MOS_OLD_artikl_rec, rowIdx2 + 1); e.Cancel = true; break; }

                        (addnewOK, newArtiklName) = theDUC.ADDREC_NewMOC_MOS_PCK_ArtiklFromOld(TheDbConnection, MOC_MOS_OLD_artikl_rec, newArtiklCD);

                        if(addnewOK == false)
                        {
                           ZXC.aim_emsg(MessageBoxIcon.Error, "MOC / MOS stavka rezultira novim artiklom čije dodavanje nije uspjelo.\n\nArtikl [{2}] Redak {0} Serijski broj {1}", rowIdx2 + 1, rtrano_rec.T_serno, newArtiklCD);
                           e.Cancel = true;
                           break;
                        }

                        TheG2.PutCell(ci2.iT_artiklName, rowIdx2, newArtiklName);

                        new_MOC_MOS_ArtiklCDlist.Add("Novi artikl: " + newArtiklCD);

                     } // if(Get_Artikl_FromVvUcSifrar(newArtiklCD) == null) 

                  } // // dodaj nove MOC/MOS Artikle  

               } // for(int rowIdx2 = 0; rowIdx2 < TheG2.RowCount - 1; ++rowIdx2)

               if(!e.Cancel && this is MOD_PTG_DUC && new_MOC_MOS_ArtiklCDlist.NotEmpty())
               {
                  // 12.09.2024: pokusasj da se FORCE ucitavanje sifrara (a s obzirom na [sifrarArtiklLastLoaded < sifrarLastChanged]) 
                  VvUserControl.sifrarArtiklLastLoaded = DateTime.MinValue;
                  SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);

                  ZXC.aim_emsg_List(string.Format("Dodao {0} novih PCK artikla.", new_MOC_MOS_ArtiklCDlist.Count), new_MOC_MOS_ArtiklCDlist);
               }

               #endregion ADD new MOC/MOS Artikl, check MOC RAM/HDD kind

               #region ValidateMOD_dokDate

               DateTime maxDate = theDUC.Get_PTG_MOD_MAX_DokDate();

               if(Fld_DokDate.Date > maxDate)
               {
                  ZXC.RaiseErrorProvider(theDUC.tbx_DokDate, "Datum modifikacije ne smije biti veći od datuma UgAn-a " + maxDate.ToString(ZXC.VvDateFormat));
                  e.Cancel = true;
                  //Fld_DokDate = maxDate;
               }

               #endregion ValidateMOD_dokDate

            } // if(this is MOD_PTG_DUC)

            if(this.IsPTG_Dodaci_DUC) // DIZ, PVR, ZIZ 
            {
               FakturPDUC theDUC = (FakturPDUC)this;

               DateTime minDate = theDUC.Get_PTG_DOD_MIN_DokDate();

               if(Fld_DokDate.Date < minDate)
               {
                  ZXC.RaiseErrorProvider(theDUC.tbx_DokDate, "Datum dodatka ne smije biti manji od datuma UgAn-a " + minDate.ToString(ZXC.VvDateFormat));
                  e.Cancel = true;
                  //Fld_DokDate = maxDate;
               }
            }

            #region PTG Set Artificial Serno

            if(faktur_rec.TtInfo.IsPTGFaktur_UgAnDodTT)
            {
               for(rowIdx2 = 0; rowIdx2 < TheG2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx2)
               {
                  rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

                  if(rtrano_rec.T_serno.IsEmpty() && rtrano_rec.T_TT != Faktur.TT_ZU2)
                  {
                     if(rtrano_rec.T_artiklCD.IsEmpty())
                     {
                        ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{0}] je prazan! Pobrišite ga prije usnimavanja.", rowIdx2 + 1);
                        e.Cancel = true;
                     }

                     artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

                     if(Artikl.DoesThisArtikl_Needs_RtranoRow_ForSerno(rtrano_rec.T_artiklCD, faktur_rec.TT)/* == false*/)
                     {
                        string artificial_serno = rtrano_rec.Get_PTG_tilda_serno(artikl_rec);

                        thePduc.TheG2.PutCell(thePduc.DgvCI2.iT_serno, rowIdx2, artificial_serno);
                     }
                  }

                  // tilda serno. provjeri sadrzaj nakon preffixa 
                  if(Rtrano.IsSernoUnReal(rtrano_rec.T_serno) &&
                     (rtrano_rec.T_TT == Faktur.TT_UG2 ||
                      rtrano_rec.T_TT == Faktur.TT_AU2 ||
                      rtrano_rec.T_TT == Faktur.TT_DI2 ||
                      rtrano_rec.T_TT == Faktur.TT_ZI2  )) 
                  {
                     artikl_rec = Get_Artikl_FromVvUcSifrar(rtrano_rec.T_artiklCD);

                     string shouldBeTildaSerno = rtrano_rec.Get_PTG_tilda_serno(artikl_rec);
                     string actualTildaSerno   = rtrano_rec.T_serno                        ;

                     if(shouldBeTildaSerno != actualTildaSerno)
                     {
                        ZXC.aim_emsg(MessageBoxIcon.Information, "Uslijed promjene parametara, 'tilda' serno\n\r\n\r{0}\n\r\n\rću prepraviti na\n\r\n\r{1}", actualTildaSerno, shouldBeTildaSerno);

                        List<Rtrano> rtranoList = RtranoDao.GetRtranoList_For_SERNO(TheDbConnection, actualTildaSerno);

                        rtranoList.Remove(rtrano_rec); // da ne rwtreca sam sebe 

                        foreach(Rtrano rtrano in rtranoList)
                        {
                           TheVvTabPage.TheVvForm.BeginEdit(rtrano);

                           rtrano.T_serno = shouldBeTildaSerno;

                           rtrano.VvDao.RWTREC(TheDbConnection, rtrano, false, true, false);

                           TheVvTabPage.TheVvForm.EndEdit(rtrano);
                        }

                        thePduc.TheG2.PutCell(thePduc.DgvCI2.iT_serno, rowIdx2, shouldBeTildaSerno);

                        if(rtranoList.Count().NotZero())
                        {
                           ZXC.aim_emsg("Preimenovao sam i {0} prijašnjih stavaka.", rtranoList.Count());
                        }
                     }
                  }
               }

            } // IsPTG_UgAnDodTT

            #endregion PTG Set Artificial Serno

            #region For Left To Right, check Rtrano vs Rtrans 

            if(this.IsPTG_LeftToRight_DUC)
            {
               bool rtransFound;

               for(rowIdx2 = 0; rowIdx2 < TheG2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx2)
               {
                  rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

                  rtransFound = faktur_rec.Transes.Count(rtr => rtr.T_artiklCD == rtrano_rec.T_artiklCD).NotZero();

                  if(rtransFound == false)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "U 'desnoj' tablici ser. brojeva postoji artikl [{0}] redak [{1}] kojeg više nema na 'lijevoj' tablici stavaka.\n\r\n\rPobrišite prvo taj redak u 'desnoj' tablici.",
                        rtrano_rec.T_artiklCD, rowIdx2 + 1);
                     e.Cancel = true;
                  }
               }
            }

            #endregion For Left To Right, check Rtrano vs Rtrans 

         } // if(this is FakturPDUC)

         #endregion Rtrano validacije

      } // if(ZXC.IsPCTOGO) 

      #endregion PCTOGO Validations

      #region Authorize M2PAY credit or debit card
#if NOTJETT
      if((this is FakturExtDUC) && faktur_rec.TtInfo.IsPrihodTT && TheVvTabPage.WriteMode == ZXC.WriteMode.Add && faktur_rec.IsNacPlacKartica)
      {
         bool isAuthorized = TryAuthorizeThisPayment(faktur_rec);

         if(isAuthorized == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Neuspjela AUTORIZACIJA kartičnog plaćanja!");
            e.Cancel = true;
         }
      }
#endif
      #endregion Authorize M2PAY credit or debit card

      #region 2026 F2 validations & setting mandatory fields

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Add &&
         IsF012DUC && faktur_rec.IsF2 && ZXC.CURR_prjkt_rec.F2_RolaKind != ZXC.F2_RolaKind.NEMA_F2 &&
         (ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipA || 
          ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB ))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, $"Ne smije se dodavati F2 B2B račun kada je uloga projekta {ZXC.CURR_prjkt_rec.F2_RolaKind}!");
         e.Cancel = true;
      }

      // Check some F2 eRacun mandatory fields 
    //if(                       Is_F012_OR_Ponuda_DUC &&  ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B                                                                                      )
    //if(                       Is_F012_OR_Ponuda_DUC && (ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B || ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B)                                                 )
    //if(ZXC.IsF2_2026_rules && Is_F012_OR_Ponuda_DUC && (ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B || ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B)                                                 )
      if(ZXC.IsF2_2026_rules && Is_F012_OR_Ponuda_DUC && (ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B || ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B) && Faktur.IsF2_PdvGEOkind(faktur_rec.PdvGEOkind))
      {
         #region Is FIR Settings & projekt_rec OK

         if(!ZXC.IsTEXTHOany && !ZXC.CURR_prjkt_rec.IsNeprofit)
         {
            if(ZXC.RRD.Dsc_F2_TT.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Nije podešena vrsta TT-a za izlazne račune (F2) u FIR postavkama programa.\n\n" + "Ako niste sigurni, kontaktirajte podršku.");
               e.Cancel = true;
            }
            if(ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.NEMA_F2)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Nije podešena 'Uloga Projekta' na 'Fiskal' TAB-u ekrana 'Projekt'.\n\n" + "Ako niste sigurni, kontaktirajte podršku.");
               e.Cancel = true;
            }
         }

         #endregion Is FIR Settings & projekt_rec OK

         #region F2_R1kind 

         if(ZXC.CURR_prjkt_rec.F2_Ima_F1_B2C == false) // nema mogucnosti pojave F1 B2C racuna 
         {
            faktur_rec.F2_R1kind = theExtDUC.Fld_F2_R1kind = ZXC.F2_R1enum.B2B;
         }
         else if(ZXC.CURR_prjkt_rec.F2_ImaF1_B2C_i_F0ili2_B2B)// TETRAGRAM, PANIGALE, FRAG, METAFLEX, PPUK, PLODINE mogu imati i B2C i B2B racune 
         {
            kupdob_rec = Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD);

            if(kupdob_rec != null)
            {
               if(kupdob_rec.R1kind != ZXC.F2_R1enum.Nepoznato) faktur_rec.F2_R1kind = theExtDUC.Fld_F2_R1kind = kupdob_rec.R1kind;
               else
               {
                  faktur_rec.F2_R1kind = theExtDUC.Fld_F2_R1kind = KupdobDao.GetMandatory_Kupdob_R1enum_FromDialog(TheDbConnection, kupdob_rec);
               }
            }
            
            if(faktur_rec.F2_R1kind == ZXC.F2_R1enum.Nepoznato)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu saznati OBAVEZAN podatak da li je kupac B2B ili B2C!");
               e.Cancel = true;
            }
         }

         #endregion F2_R1kind 

         #region Oib operatera

         if(ZXC.IsBadOib(ZXC.CURR_user_rec.Oib, false))
         { 
           ZXC.aim_emsg(MessageBoxIcon.Error, $"Neispravan OIB operatera:[{ZXC.CURR_user_rec.Oib}] za usera {ZXC.CURR_user_rec.RecID} - {ZXC.CURR_user_rec.UserName}! Račun nije moguće usnimiti kao fiskalni Račun.");
            e.Cancel = true;
         }

         #endregion Oib operatera

         #region KPD sifra

         // ma ne tu. gore u for() 

         //if(faktur_rec.Transes.Any(rtr => rtr.T_artiklCD.NotEmpty() && rt)
         //{
         //   ZXC.aim_emsg(MessageBoxIcon.Error, $"Neispravan OIB operatera:[{ZXC.CURR_user_rec.Oib}] za usera {ZXC.CURR_user_rec.RecID} - {ZXC.CURR_user_rec.UserName}! Račun nije moguće usnimiti kao F2 eRačun.");
         //   e.Cancel = true;
         //}

         #endregion KPD sifra

         #region eProces, eStatus

         if(faktur_rec.StatusCD.IsEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Kod tipa računa ('KodRač:') je 0!");
            e.Cancel = true;
         }

         if(faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P00)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovni proces računa ('eRproc:') je nedefiniran!");
            e.Cancel = true;
         }

         #endregion eProces, eStatus

         #region Mora biti bar jedan red i prvi red mora imati ArtiklCD, 

       //if(ZXC.RISK_SaveVvDataRecord_inProgress && IsF012DUC && faktur_rec.IsF2 && faktur_rec.TrnNonDel.Count().IsZero()                                     )
         if(ZXC.RISK_SaveVvDataRecord_inProgress && IsF012DUC && faktur_rec.IsF2 && faktur_rec.TrnNonDel.Count().IsZero() && !ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "F2 račun mora sadržavati bar jednu stavku!");
            e.Cancel = true;
         }

       //if(IsF012DUC && faktur_rec.IsF2 && faktur_rec.TrnNonDel.Count().IsPositive() && faktur_rec.TrnNonDel.OrderBy(rtr => rtr.T_serial).First().T_artiklCD.IsEmpty()                                    )
         if(IsF012DUC && faktur_rec.IsF2 && faktur_rec.TrnNonDel.Count().IsPositive() && faktur_rec.TrnNonDel.OrderBy(rtr => rtr.T_serial).First().T_artiklCD.IsEmpty()&& !ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"Prvi red na F2 računu ne smije biti 'opisni' redak (redak bez šifre artikla){Environment.NewLine}{Environment.NewLine}{faktur_rec.Transes[0].T_artiklName}!");
            e.Cancel = true;
         }

         #endregion Mora biti bar jedan red i prvi red mora imati ArtiklCD, 

         #region KdUlica, KdMjesto, KdZip

       //if(faktur_rec.IsF2 && faktur_rec.KupdobCD != ZXC.RRD.Dsc_MalopKCD && faktur_rec.KdAdresa.IsEmpty()                                     )
         if(faktur_rec.IsF2 && faktur_rec.KupdobCD != ZXC.RRD.Dsc_MalopKCD && faktur_rec.KdAdresa.IsEmpty() && !ZXC.CURR_prjkt_rec.F2_Ima_F0_B2B)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Poštanska adresa kupca je prazna! eRačun mora imati bar 'Mjesto'");
            e.Cancel = true;
         }

         // Provjeravamo CURR_prjkt_rec 
         if(faktur_rec.IsF2 && 
            (ZXC.CURR_prjkt_rec.Ziro1  .IsEmpty() || 
             ZXC.CURR_prjkt_rec.Email  .IsEmpty() || 
             ZXC.CURR_prjkt_rec.Ulica1 .IsEmpty() || 
             ZXC.CURR_prjkt_rec.Grad   .IsEmpty() || 
             ZXC.CURR_prjkt_rec.PostaBr.IsEmpty() || 
             ZXC.CURR_prjkt_rec.Ime    .IsEmpty() || 
             ZXC.CURR_prjkt_rec.Prezime.IsEmpty())
           )
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Projekt ima nepotpune podatke (Ziro, Mail, Ulica, Grad, Post br., Ime, Prezime).");
            e.Cancel = true;
         }

         #endregion KdUlica, KdMjesto, KdZip

      } // Check some F2 eRacun mandatory fields 

      // Spremanje trenutnog TtNumFiskal kao VezniDok za DataLayer 
      if(IsF012DUC && faktur_rec.Is_F2_TtNumFisk_InVezniDok)
      {
         faktur_rec.VezniDok = faktur_rec.TtNumFiskal; // cuvat cemo u data layeru rezultat result propertya 'TtNumFiskal' 
         Fld_VezniDok        = faktur_rec.VezniDok;
      }

      // Uklanjanja F2_Unprocessed iz Napomene 
      if(/*IsF012DUC && faktur_rec.IsF2 &&*/ faktur_rec.Napomena.Contains(ZXC.F2_Unprocessed))
      {
         faktur_rec.Napomena = faktur_rec.Napomena.Replace(ZXC.F2_Unprocessed, "");
         Fld_Napomena        = faktur_rec.Napomena;
         ZXC.aim_emsg(MessageBoxIcon.Information, $"Uklonjena '{ZXC.F2_Unprocessed}' oznaka.");
      }

      // OIB Kupca: namjerno je izvan if-a  Check some F2 eRacun mandatory fields, jer npr ..., Veridian kupuje u TH gaće ... ipak mora imati OIB  - ALI ako je strana firma onda oni imaju drugacije oib-e
    //if(ZXC.IsF2_2026_rules              && faktur_rec.F2_R1kind == ZXC.F2_R1enum.B2B                                                 )
      if(ZXC.IsF2_2026_rules && IsF012DUC && faktur_rec.F2_R1kind == ZXC.F2_R1enum.B2B && Faktur.IsF2_PdvGEOkind(faktur_rec.PdvGEOkind))
      {
         bool badOIB = ZXC.IsBadOib(theExtDUC.Fld_KdOib, false);

         if(badOIB)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"Neispravan OIB kupca!{Environment.NewLine}{Environment.NewLine}{theExtDUC.Fld_KdOib}");
            e.Cancel = true;
         }
      }

      // Check AVANS birth 
      if(IsF012DUC && faktur_rec.IsF2)
      {
         bool mejbiAvans = false;

         #region vidi jel avans

         string avansStr    = "AVANS"   ;
         string predujamStr = "PREDUJAM";

         if(faktur_rec.Napomena .ToUpper().Contains(avansStr   )) mejbiAvans = true;
         if(faktur_rec.Napomena2.ToUpper().Contains(avansStr   )) mejbiAvans = true;
         if(faktur_rec.Napomena .ToUpper().Contains(predujamStr)) mejbiAvans = true;
         if(faktur_rec.Napomena2.ToUpper().Contains(predujamStr)) mejbiAvans = true;

         if(faktur_rec.Transes.Any(rtr => rtr.T_artiklCD  .ToUpper().Contains(avansStr   ))) mejbiAvans = true;
         if(faktur_rec.Transes.Any(rtr => rtr.T_artiklName.ToUpper().Contains(avansStr   ))) mejbiAvans = true;
         if(faktur_rec.Transes.Any(rtr => rtr.T_artiklCD  .ToUpper().Contains(predujamStr))) mejbiAvans = true;
         if(faktur_rec.Transes.Any(rtr => rtr.T_artiklName.ToUpper().Contains(predujamStr))) mejbiAvans = true;

         if(isAvansArtiklFound) mejbiAvans = true;

         if(faktur_rec.S_ukKCRP.IsNegative()) mejbiAvans = false; // STORNO avansa je u pitanju 

         #endregion vidi jel avans

         bool isFinalRn = faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P11;

         if(mejbiAvans && (faktur_rec.PdvKolTip != ZXC.VvUBL_PolsProcEnum.P04 || faktur_rec.StatusCD != "386") && !isFinalRn)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, $"Ovo je, čini se, AVANS (račun za predujam)!?{Environment.NewLine}{Environment.NewLine}A 'eRproc' i 'Kod rač' imaju, čini se, ne adekvatne vrijednosti!?");
         }
      }

      #endregion 2026 F2 validations & setting mandatory fields

      #region Tetragram

      if(ZXC.IsTETRAGRAM_ANY && TheVvTabPage.WriteMode != ZXC.WriteMode.None && theExtDUC != null)
      {
         bool isZAR_SkladCD       = theExtDUC.Fld_SkladCD.StartsWith("Z-ZA-");
         bool isFORBIDDEN_SkladCD = theExtDUC.Fld_SkladCD.StartsWith("Z-ZA-1");

         if(ZXC.vvDB_VvDomena == "vvT3" && isFORBIDDEN_SkladCD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"ZABRANJENO skladište!{Environment.NewLine}{Environment.NewLine}Trenutno skladište: {theExtDUC.Fld_SkladCD}");
            e.Cancel = true;
         }

         if(theExtDUC is ZAR_DUC && isZAR_SkladCD == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"Za Tetragram ZAR, skladište mora počinjati sa 'Z-ZA-'!{Environment.NewLine}{Environment.NewLine}Trenutno skladište: {theExtDUC.Fld_SkladCD}");
            e.Cancel = true;
         }

         if(!(theExtDUC is ZAR_DUC) && !(theExtDUC is PON_MPC_DUC) && isZAR_SkladCD == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, $"Zabranjeno skladište ZAR-a. Dokument nije ZAR!{Environment.NewLine}{Environment.NewLine}Trenutno skladište: {theExtDUC.Fld_SkladCD}");
            e.Cancel = true;
         }

       //if(isZAR_SkladCD == false                                   )
         if(isZAR_SkladCD == false && this is BlgUplat_M_DUC == false)
         {
            if(faktur_rec.Transes.Any(rtr => rtr.T_artiklCD.StartsWith("ZAR")))
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, $"Nedozvoljena kombinacija ZAR artikla sa NE ZAR skladištem!{Environment.NewLine}{Environment.NewLine}Trenutno skladište: {theExtDUC.Fld_SkladCD}");
               e.Cancel = true;
            }
         }
         if(isZAR_SkladCD == true)
         {
            if(faktur_rec.Transes.Any(rtr => rtr.T_artiklCD.StartsWith("ZAR")) == false)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, $"Nedozvoljena kombinacija NE ZAR artikla sa ZAR skladištem!{Environment.NewLine}{Environment.NewLine}Trenutno skladište: {theExtDUC.Fld_SkladCD}");
               e.Cancel = true;
            }
         }

      }

      #endregion Tetragram

      #region BUG Kupdob.TickerToken u Nazivu

      if(faktur_rec.TheEx != null && faktur_rec.KupdobName != null && faktur_rec.KupdobName.Contains(Kupdob.TickerToken))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, $"Greška: Partner naziv sadrži '{Kupdob.TickerToken}'\n\r\n\rPobrišite pa nanovo zadajte partnera.");
         e.Cancel = true;
      }

      #endregion BUG Kupdob.TickerToken u Nazivu

   } // void FakturDUC_Validating(object sender, CancelEventArgs e)

   #region M2PAY Hapi

#if NOTJETT
   private bool TryAuthorizeThisPayment(Faktur faktur_rec)
   {
      if(ZXC.M2PAY_API_Initialized == false)
      {
         Initialize_M2PAY_API();

         ZXC.M2PAY_API_Initialized = true;
      }

      return false;
   }

   private void Initialize_M2PAY_API()
   {
      //// ovaj nacin je sa "https://developer.handpoint.com/windows/windowsintegrationguide#WinPaxIntegration"

      //string sharedSecret = "0102030405060708091011121314151617181920212223242526272829303132";
      //string apikey = "This-is-my-api-key-provided-by-Handpoint";
      //Hapi api = HapiFactory.GetAsyncInterface(this, new HandpointCredentials(sharedSecret, apikey));
      //// The api is now initialized. Yay! we've even set default credentials.
      //// The shared secret is a unique string shared between the payment terminal and your application, it is a free field.
      //// The Api key is a unique key per merchant used to authenticate the terminal against the Cloud.
      //// You should replace the API key with the one sent by the Handpoint support team.



      // ovaj nacin je iz E:\0_DOWNLOAD\M2PAY\Windows-HAPI-Getting-started-masterWindows-Getting-started.sln

      // If using a Handpoint integration card reader update the following string with the supplied shared secret
      //string sharedSecret = "0102030405060708091011121314151617181920212223242526272829303132"; // orig 
      string sharedSecret = "1BBFFC6D1CFE1EC980584A61F802D362D7A193CD530E7993DD95F63C433660BC"; // ovo poslalo onaj Matej 
      Hapi the_hAPI = HapiFactory.GetAsyncInterface(this).DefaultSharedSecret(sharedSecret);
      // The api is now initialized. Yay! we've even set a default shared secret!
      // The shared secret is a unique string shared between the card reader and your mobile application. 
      // It prevents other people to connect to your card reader.

      // Subscribe to the status notifications
      the_hAPI.AddStatusNotificationEventHandler(this);

      DirectConnect_HapiDevice(the_hAPI); // da li ovo tu ili gore u TryAuthorizeThisPayment      
                                          // dakle, da li ovo jednom ili kods svake autorizacije? 
   }

   public void DirectConnect_HapiDevice(Hapi the_hAPI)
   {
      Device device = new Device("PP0513901435", "68:AA:D2:00:D5:27", "", ConnectionMethod.ETHERNET);
      //new Device("name", "address", "port", ConnectionMethod);
      //The address always has to be written in UPPER CASE
      
      the_hAPI.UseDevice(device);
   }
#endif
   #endregion M2PAY Hapi

   protected string  oldSkladCD, oldSkladCD2, oldVezniDok;
   private   uint    oldTtNum;

   // 10.10.2023: abrakadabra_oldRtransKol_forCheckMinus
   private decimal dataLayerT_kol;

   private bool TexthoRabatIsNeopravdan(bool isKupon, string skladCD, decimal ukRbt1, decimal ukKC, decimal ukK, DateTime dokDate)
   {
      if(ukRbt1.IsZero()) return false;

      // 11.03.2024: 'vracamo uvjet ali osim u nekom periodu 5w poslovnice mogu sto hoce!' 
      bool ostvareniSuUvjetiZaBezuvjetniRabat = ZXC.IsTH_5WeekShop(skladCD) && ZXC.IsTH_specialPeriod(dokDate);

      // 09.04.2025: takoder, ako je aktiviran 'slovacki' kupon (taj nije izdan iz Vektora)      
      bool izdanJe20postotniKupon = Fld_V2_ttNum == 20; // 

      if(izdanJe20postotniKupon) ostvareniSuUvjetiZaBezuvjetniRabat = true;

      if(ostvareniSuUvjetiZaBezuvjetniRabat) return false;

      ukKC = Math.Abs(ukKC); // za slucaj kada je storno, ne zelimo proglasavati neopravdanost 
      ukK  = Math.Abs(ukK ); // za slucaj kada je storno, ne zelimo proglasavati neopravdanost 

      // 20.03.2025: ovako, a dole sve remarckiramo 
      if(isKupon)
      {
         return false; // do dalnjega, odobravanje rabata na zaprimljeni kupon ide bezuvjetno 
      }
      else // not kupon, classic rabat
      {
         if(ukKC >= 5.00M) return false;
         else              return true ;
      }

#if nekkad_bilo
      //05.04.2024. Velika Gorica I Koprivnica više nisu special cycle trgovine 
      //if(skladCD == "20M5" || skladCD == "74M5") // Velika Gorica i Koprivnica imaju pravilo 'bar 2 komada'
      //{
      //   if(ukK >= 2) return false;
      //   else         return true ;
      //}
      //else // ostale poslovnice imaju pravilo 'bar 5 eur' 
      //{
      // 10.03.2025: gasimo ovaj uvjet od minimalno 5 EUR, dakle prema njihovim uputama nema vise uvjeta za odobravanje rabata do daljnjega  
      //if(ukKC >= 5.00M) return false;
      //else              return true ;
      return false;
      //}
#endif
   }

   public string GetPIZ_IntersectTT()
   {
      // 23.08.2020: 
    //var sirovineListCD = faktur_rec.Transes  .Where(rtr => rtr.T_TT == faktur_rec.TT             && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklCD.ToUpper());
    //var produktiListCD = faktur_rec.Transes  .Where(rtr => rtr.T_TT == faktur_rec.TtInfo.SplitTT && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklCD.ToUpper());
      var sirovineListCD = faktur_rec.TrnNonDel.Where(rtr => rtr.T_TT == faktur_rec.TT             && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklCD.ToUpper());
      var produktiListCD = faktur_rec.TrnNonDel.Where(rtr => rtr.T_TT == faktur_rec.TtInfo.SplitTT && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklCD.ToUpper());

      var presjekListCD = sirovineListCD.Intersect(produktiListCD);

      string errMsgCD = "";

      if(presjekListCD.Count().NotZero())
      {
         foreach(string rtransDoubler in presjekListCD)
         {
            errMsgCD += rtransDoubler + "\n";
         }

         //ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDokument sadrži stavku(e) sa DUPLOM ULOGOM!\n\n" + errMsg);
         //e.Cancel = true;
      }

      // 23.08.2020: 
    //var sirovineListName = faktur_rec.Transes  .Where(rtr => rtr.T_TT == faktur_rec.TT             && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklName.ToUpper());
    //var produktiListName = faktur_rec.Transes  .Where(rtr => rtr.T_TT == faktur_rec.TtInfo.SplitTT && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklName.ToUpper());
      var sirovineListName = faktur_rec.TrnNonDel.Where(rtr => rtr.T_TT == faktur_rec.TT             && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklName.ToUpper());
      var produktiListName = faktur_rec.TrnNonDel.Where(rtr => rtr.T_TT == faktur_rec.TtInfo.SplitTT && rtr.T_artiklCD.NotEmpty()).Select(rtr => rtr.T_artiklName.ToUpper());

      var presjekListName = sirovineListName.Intersect(produktiListName);

      string errMsgName = "";

      if(presjekListName.Count().NotZero())
      {
         foreach(string rtransDoubler in presjekListName)
         {
            errMsgName += rtransDoubler + "\n";
         }

         //ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nDokument sadrži stavku(e) sa DUPLOM ULOGOM!\n\n" + errMsg);
         //e.Cancel = true;
      }

      return errMsgCD + errMsgName;
   }

   protected void OnExitSkladCD2_SetKomisijaPartner(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(this.Visible == false) return;

      VvTextBox vvtb = sender as VvTextBox;

      if(oldSkladCD2 == vvtb.Text) return; // nepromijenjeno skladiste 

      // 17.06.2025:
      if(IsPTG_ANY_DUC)
      {
         bool isGrid1NotEmpty = TheG .VvEffectiveRowCount.NotZero();
         bool isGrid2NotEmpty = TheG2.VvEffectiveRowCount.NotZero();

         bool doesCareAboutMinus = ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_ALL ||
                                   ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL;


         bool shouldForbid = isGrid2NotEmpty || (isGrid1NotEmpty && doesCareAboutMinus);

         if(shouldForbid)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop,
               " Nedozvoljena promjena skladišta nakon unosa podatka na stavke dokumenta!");
            e.Cancel = true;
            Fld_SkladCD2 = oldSkladCD2;
            return;
         }
      }

      bool isCentToCentMSI = (this is MedjuSkladDUC) &&
                             Fld_SkladCD .StartsWith(ZXC.vvDB_ServerID_CENTRALA.ToString("00")) &&
                             Fld_SkladCD2.StartsWith(ZXC.vvDB_ServerID_CENTRALA.ToString("00"));

      if(ZXC.IsTEXTHOany && isCentToCentMSI && Fld_SkladCD2.StartsWith("12BP"))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, " Skladište POVRATA je nedozvoljeno!");
         e.Cancel = true;
      }

      VvLookUpItem thisSklad2LUI = ZXC.luiListaSkladista.GetLuiForThisCd(Fld_SkladCD2); 
      if(this is FakturExtDUC && thisSklad2LUI != null && thisSklad2LUI.Integer > 10) // '> 10' znaci da je zadano komisijsko skladiste 
      {
         Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.Ticker == Fld_SkladCD2);
         // 21.11.2022: 
       //if(kupdob_rec != null)
         if(kupdob_rec != null && kupdob_rec.Komisija != ZXC.KomisijaKindEnum.NIJE)
            {
            FakturExtDUC theDUC = this as FakturExtDUC;
            theDUC.Fld_KupdobTk = Fld_SkladCD2;
            VvSQL.SorterType origSorter = theDUC.sifrarSorterType;
            theDUC.sifrarSorterType = VvSQL.SorterType.Ticker;
            theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobTk, EventArgs.Empty);
            theDUC.sifrarSorterType = origSorter;
         }
      }
      oldSkladCD2 = Fld_SkladCD2;
   }

   protected /*private*/ void OnExitSkladCD_SetTtNum_And_ValidateSkladCD(object sender, System.ComponentModel.CancelEventArgs e)
   {
      // 29.03.2016: 
      if(this is RNMDUC) return;

      if(this.Visible == false) return;

      VvTextBox vvtb = sender as VvTextBox;

      //uint newTtNum=0;

      if(oldSkladCD != vvtb.Text) // promijenjeno skladiste 
      {

         // 17.06.2025:
         if(IsPTG_ANY_DUC)
         {
            bool isGrid1NotEmpty = TheG .VvEffectiveRowCount.NotZero();
            bool isGrid2NotEmpty = TheG2.VvEffectiveRowCount.NotZero();

            bool doesCareAboutMinus = ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_ALL ||
                                      ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL;


            bool shouldForbid = isGrid2NotEmpty || (isGrid1NotEmpty && doesCareAboutMinus);

            if(shouldForbid)
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop,
                  " Nedozvoljena promjena skladišta nakon unosa podatka na stavke dokumenta!");
               e.Cancel = true;
               Fld_SkladCD = oldSkladCD;
               return;
            }
         }

         bool isCentToCentMSI = (this is MedjuSkladDUC) && 
                                Fld_SkladCD .StartsWith(ZXC.vvDB_ServerID_CENTRALA.ToString("00")) && 
                                Fld_SkladCD2.StartsWith(ZXC.vvDB_ServerID_CENTRALA.ToString("00"));

         if(ZXC.IsTEXTHOany && isCentToCentMSI && Fld_SkladCD.StartsWith("12BP"))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, " Skladište POVRATA je nedozvoljeno!");
            e.Cancel = true;
         }

         string skladCD4_ttNum = GetSkladCD4_ttNum(/*out isSkladCD2*/); // 12.12.2014: Logika za slijednost brojeva: vidi komentar kod GetSkladCD4_ttNum() 

         if(this is ZAH_SVD_DUC)
         {
            skladCD4_ttNum = ZXC.CURR_userName;
         }

         // 28.01.2016: 
         bool isInEditNotInADD    = TheVvTabPage.WriteMode == ZXC.WriteMode.Edit;
         bool IsSklCd_YES_InTtNum =  faktur_rec.TtInfo.IsSklCdInTtNum           ;
         bool IsSklCd_NOT_InTtNum = !faktur_rec.TtInfo.IsSklCdInTtNum           ;

         uint   arhivedTtNum      =  0;
         string arhivedSkladCD    = "";
         int    arhivedDokDate_YY =  0;

         if(isInEditNotInADD)
         {
            arhivedTtNum      = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).TtNum  ;
            arhivedSkladCD    = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).SkladCD;
            arhivedDokDate_YY = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).DokDate.VvDokDate_YY();
         }

         bool isJednakaSlijednost = GetIsJednakaSlijednost(Fld_TT, arhivedSkladCD, vvtb.Text);

         string newSkladCD    = vvtb.Text;
         int    newDokDate_YY = Fld_DokDate.VvDokDate_YY();

         bool is_skladCD_And_dokDate_YY_unchanged = arhivedSkladCD == newSkladCD && arhivedDokDate_YY == newDokDate_YY;

       //if(isInEditNotInADD && arhivedSkladCD == vvtb.Text        )
         if(isInEditNotInADD && is_skladCD_And_dokDate_YY_unchanged)
         {
            Put_NewTT_Num(arhivedTtNum);
         }
       //else if(isInEditNotInADD &&  isJednakaSlijednost                        ) // usli smo i ispravi i promijenili skladiste... ako je novo skl u istoj slijednosti sa starim                                    , OSTAVI isti broj
         else if(isInEditNotInADD && (isJednakaSlijednost || IsSklCd_NOT_InTtNum)) // usli smo i ispravi i promijenili skladiste... ako je novo skl u istoj slijednosti sa starim ili ako uopce nije u korjenu ttNuma, OSTAVI isti broj
         {
            Put_NewTT_Num(arhivedTtNum);
         }
         else
         {
            // 17.02.2025: unificiran nacin new tt num-a
          //string vezniDokAsRNMkind = (this is RNMDUC || this is RNZDUC ? Fld_VezniDok      : "");
          //int    eventualRNZmonth  = (this is RNZDUC                   ? Fld_DokDate.Month : 0 );
          //newTtNum = TheVvDao.GetNextTtNum(TheDbConnection, Fld_TT, skladCD4_ttNum, isCentToCentMSI);

            // 23.06.2025: 
          //                        Put_NewTT_Num(this.GetNewTtNum_2025());
            if(IsSklCd_YES_InTtNum) Put_NewTT_Num(this.GetNewTtNum_2025());
         }

         // 18.03.2014: Komisija News 
         if(ZXC.TtInfo(Fld_TT).IsPrihodTTorABx)
         {
            VvLookUpItem baseSkladLUI = ZXC.luiListaSkladista.GetBaseSkladLUI(Fld_SkladCD); // glavno skladiste 
            if(baseSkladLUI != null)
            {
               Fld_SkladBR = (uint)ZXC.luiListaSkladista.GetBaseSkladLUI(Fld_SkladCD).Integer;
            }
            else // debil ima OPP koje ne postoji kao glavno skladiste 
            {
               Fld_SkladBR = ZXC.luiListaSkladista.GetUintegerForThisCd(Fld_SkladCD); // glavno skladiste 
            }
         }
         // 18.03.2014: Komisija News continued 
         VvLookUpItem thisSkladLUI = ZXC.luiListaSkladista.GetLuiForThisCd(Fld_SkladCD); // zadano/realno skladiste 
         if(this is FakturExtDUC && thisSkladLUI != null && thisSkladLUI.Integer > 10) // '> 10' znaci da je zadano komisijsko skladiste 
         {
            Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.Ticker == Fld_SkladCD);

            // 21.11.2022: 
          //if(kupdob_rec != null)
            if(kupdob_rec != null && kupdob_rec.Komisija != ZXC.KomisijaKindEnum.NIJE)
            {
               FakturExtDUC theDUC = this as FakturExtDUC;
               theDUC.Fld_KupdobTk = Fld_SkladCD;
               this.originalText   = Fld_SkladCD;

               // 18.05.2020: bio BUG bez ovoga ... save ORIG 
               VvSQL.SorterType ORIG_sifrarSorterType = theDUC.sifrarSorterType;

               theDUC.sifrarSorterType = VvSQL.SorterType.Ticker;

               //// 11.07.2019: 
               //theDUC.Fld_KupdobCd   = kupdob_rec.KupdobCD;
               //theDUC.Fld_KupdobTk   = kupdob_rec.Ticker  ;
               //theDUC.Fld_KupdobName = kupdob_rec.Naziv   ;


               theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobTk, EventArgs.Empty);

               // 18.05.2020: bio BUG bez ovoga ... restore ORIG 
               theDUC.sifrarSorterType = ORIG_sifrarSorterType;
            }
         }

         #region if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij) RecalcPrNabCijAndResultFields();

         // 04.11.2021: 
       //if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij)
         if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij || (ZXC.IsSvDUH && faktur_rec.TT == Faktur.TT_IZD))
         {
            RecalcPrNabCijAndResultFields();
         }

         #endregion if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij) RecalcPrNabCijAndResultFields();

      } // if(oldSkladCD != vvtb.Text) // promijenjeno skladiste 

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.Add)
      //{
      //   if(oldSkladCD != vvtb.Text) // promijenjeno skladiste 
      //   {
      //      newTtNum = TheVvDao.GetNextTtNum(conn, Fld_TT, Fld_SkladCD);
      //      Put_NewTT_Num(newTtNum);
      //   }
      //}
      //else if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit)
      //{
      //   string befEditSkladCD = faktur_rec.BackupData._skladCD;

      //   if(faktur_rec.TtInfo.IsSklCdInTtNum == true)
      //   {
      //   }
      //   else
      //   {
      //   }
      //}

      oldTtNum   = Fld_TtNum;
      oldSkladCD = Fld_SkladCD;

      ////if(dbNavigationRestrictor.RestrictedValues.Contains(vvtb.Text) == false)
      ////{
      ////   ZXC.RaiseErrorProvider((Control)sender, "Nedozvoljeni TIP TRANSAKCIJE (TT).");
      ////   e.Cancel = true;
      ////}


      //#region Nepoznata Potreba Ovo je code-irala, ali ispada bug koji ne da ispraviti ttNum, renmarck do dalnjega...
      ////if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit)
      ////{
      ////   if(faktur_rec.BackupData._skladCD == Fld_SkladCD &&
      ////      faktur_rec.BackupData._tt      == Fld_TT)
      ////   {
      ////      Put_NewTT_Num(faktur_rec.BackupData._ttNum);
      ////      return;
      ////   }
      ////}

      //// 30.1.2011:
      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit && faktur_rec.TtInfo.IsSklCdInTtNum == false) return;

      //#endregion Nepoznata Potreba Ovo je code-irala, ali ispada bug koji ne da ispraviti ttNum, renmarck do dalnjega...

      ////if(Fld_TT.IsEmpty()/* || ZXC.TtInfo(Fld_TT).IsSklCdInTtNum == false*/) return;

      //if(TheVvTabPage.WriteMode != ZXC.WriteMode.Edit) Put_NewTT_Num(newTtNum);

      //if(oldTtNum.NotZero() && oldTtNum != newTtNum)
      //{
      //   if(faktur_rec.TtInfo.IsSklCdInTtNum)
      //      ZXC.aim_emsg(MessageBoxIcon.Warning,
      //         "Upozorenje: stari broj dokumenta {0} je u međuvremenu iskorišten, ili je promijenjeno skladište te dokument dobiva novi broj {1}", oldTtNum, newTtNum);
      //   else
      //      ZXC.aim_emsg(MessageBoxIcon.Warning,
      //         "Upozorenje: stari broj dokumenta {0} je u međuvremenu iskorišten, te dokument dobiva novi broj {1}", oldTtNum, newTtNum);
      //}

      if(ZXC.IsPCTOGO)
      {
         //if(faktur_rec.TtInfo.IsPTG_KUGinTtNum     ) ZXC.aim_emsg("todo!");
         //if(faktur_rec.TtInfo.IsPTG_DodTT          ) ZXC.aim_emsg("todo!");
         //if(faktur_rec.TtInfo.IsPTG_YYinTtNum      ) ZXC.aim_emsg("todo!");
         //if(faktur_rec.TtInfo.IsPTG_YYinTtNum_99999) ZXC.aim_emsg("todo!");

       // 06.06.2025. mislim da je ovo ostatak drugog smjera
       //if(this is ZIZ_PTG_DUC)
       //{
       //   Rtrans ZIZ_ZUL_rtrans_rec;
       //
       //   for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
       //   {
       //      ZIZ_ZUL_rtrans_rec = (Rtrans)GetDgvLineFields1(rowIdx, false, null);
       //
       //      if(ZIZ_ZUL_rtrans_rec.T_TT == Faktur.TT_ZIZ) TheG.PutCell(ci.iT_skladCD , rowIdx, Fld_SkladCD );
       //      if(ZIZ_ZUL_rtrans_rec.T_TT == Faktur.TT_ZUL) TheG.PutCell(ci.iT_skladCD2, rowIdx, Fld_SkladCD2);
       //
       //   }
       //}
      }

      // CLASSIC: 
      else if((faktur_rec.TtInfo.IsSklCdInTtNum && Fld_TtNum.ToString().StartsWith(Fld_SkladBR.ToString()) == false) || Fld_TtNum.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error,
            " Brojčana oznaka skladišta {0} mora biti u korjenu broja dokumenta {1}, a nije!", Fld_SkladBR, Fld_TtNum);
         e.Cancel = true;
      }

      if(faktur_rec.TtInfo.IsFinKol_TT && Fld_SkladBR.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error,
            " Brojčana oznaka skladišta {0} nesmije biti 0 - 'Nebitno'!", Fld_SkladBR);
         e.Cancel = true;
      }

      //if(newTtNum.IsZero()) e.Cancel = true;

      // 20.02.2014: Lili :-) 
      ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD = Fld_SkladCD;
   }

   protected /*private*/ void OnExitRNMkind_SetTtNum(object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(this is RNMDUC == false) return;

      if(this.Visible == false) return;

      VvTextBox vvtb = sender as VvTextBox;

      uint newTtNum=0;

      if(oldVezniDok != vvtb.Text) // promijenjeni VezniDok as RNMkind 
      {
         bool isInEditNotInADD = TheVvTabPage.WriteMode == ZXC.WriteMode.Edit;

         uint   arhivedTtNum    =  0;
         string arhivedVezniDok = "";

         if(isInEditNotInADD)
         {
            arhivedTtNum    = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).TtNum   ;
            arhivedVezniDok = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).VezniDok;
         }

       //bool isJednakaSlijednost = GetIsJednakaSlijednost_RNMkind(arhivedVezniDok, vvtb.Text);

         if(isInEditNotInADD && arhivedVezniDok == vvtb.Text)
         {
            Put_NewTT_Num(arhivedTtNum);
         }
       //else if(isInEditNotInADD /*&& isJednakaSlijednost*/) // usli smo i ispravi i promijenili VezniDok 
       //{
       //   Put_NewTT_Num(arhivedTtNum);
       //}
         else
         {
            newTtNum = TheVvDao.GetNextTtNum(TheDbConnection, Fld_TT, /*skladCD4_ttNum*/"", /*isCentToCentMSI*/false, vvtb.Text);
            Put_NewTT_Num(newTtNum);
         }
      }
      oldTtNum    = Fld_TtNum   ;
      oldVezniDok = Fld_VezniDok;
   }

   private bool GetIsJednakaSlijednost(string theTT, string arhivedSkladCD, string newSkladCD)
   {
      VvLookUpItem oldSkladLUI = ZXC.luiListaSkladista.GetLuiForThisCd(arhivedSkladCD);
      VvLookUpItem newSkladLUI = ZXC.luiListaSkladista.GetLuiForThisCd(    newSkladCD);

    //if(ZXC.IsOPPsljednost(theTT, newSkladLUI.Uinteger) == false) return false; // nemoj na promjenu sklCDa NonPrihodTT-ovima () proglasavati zajednicku sljednost iako imaju isti opp 
      if(ZXC.TtInfo(theTT).IsPrihodTTorABx               == false) return false; // nemoj na promjenu sklCDa NonPrihodTT-ovima () proglasavati zajednicku sljednost iako imaju isti opp 

      if(oldSkladLUI == null || newSkladLUI == null) return false;

      if(oldSkladLUI.Uinteger.NotZero() && oldSkladLUI.Uinteger == newSkladLUI.Uinteger) return true;

      VvLookUpItem oldBaseSkladLUI = ZXC.luiListaSkladista.GetBaseSkladLUI(arhivedSkladCD); // glavno skladiste OLD 
      VvLookUpItem newBaseSkladLUI = ZXC.luiListaSkladista.GetBaseSkladLUI(    newSkladCD); // glavno skladiste NEW 
      
      if(oldBaseSkladLUI != null && newBaseSkladLUI != null && oldBaseSkladLUI.Cd == newBaseSkladLUI.Cd) return true;

      return false;
   }

   /// <summary>
   /// Ovime se definira logika slijednosti TtNum-ova za promete koji imaju dva (odlazno i dolazno) skladista
   /// Ako je tako odobreno u RiskRulsima, slijednost odredjuje (njegov SBR ide u korjen TtNum-a) skladiste
   /// koje se odnosi na poslovnicu, ili sta vec, samo da nije neko zajednicko - centralno skladiste
   /// </summary>
   /// <returns></returns>
   private string GetSkladCD4_ttNum(/*out bool isSkladCD2*/)
   {
      //isSkladCD2 = false;

      if(ZXC.RRD.Dsc_IsMSIttNumByPosl == false) return Fld_SkladCD;

      // ako ne postoje 2 sklad na DUCu, onda daj to prvo i jedino,          
      if(Fld_SkladCD2.IsEmpty()) return Fld_SkladCD;

      // ako postoje 2, daj prvoga u kojem se spominje poslovnicaSkladiste   
      // ako postoje 2, a ni jedno nije poslovnicaSkladiste, tada vrati prvo 
      if(Fld_SkladCD2.NotEmpty())
      {
              if(ZXC.IsPoslovnicaSklad(Fld_SkladCD )) {                        return Fld_SkladCD ; }
         else if(ZXC.IsPoslovnicaSklad(Fld_SkladCD2)) { /*isSkladCD2 = true;*/ return Fld_SkladCD2; }
         else                                         {                        return Fld_SkladCD ; }
      }

      return Fld_SkladCD;
   }

   private void RecalcPrNabCijAndResultFields()
   {
      // 08.06.2017: da nulti ZPC ne traje dugo 
      if(ZXC.IsTEXTHOcentrala && this is NivelacijaDUC && Faktur.Get_IsNultiZPC(Fld_TT, Fld_TtNum) == true) return; // oj van, nemoj revalorizirati cijene 

      bool changeOccured = false, changeReported = false;

      Rtrans rtrans_rec;

      for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
      {
         rtrans_rec = (Rtrans)GetDgvLineFields1(rowIdx, false, null);

         ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec);

         if(artStat_rec == null) artStat_rec = new ArtStat();

         if(rtrans_rec.T_cij != artStat_rec.PrNabCij)
         {
            changeOccured = true;

            // 16.10.2015: 
            if(changeReported == false)
            {
             //ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0} cijena je kriva usljed promjene skladišta i/ili datuma.\n\nIspraviti ću je automatski.", rowIdx + 1);
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Uslijed promjene skladišta i/ili datuma, neke su cijene krive.\n\nIspraviti ću ih automatski.");
               changeReported = true;
            }

            TheG.PutCell(ci.iT_cij, rowIdx, artStat_rec.PrNabCij);
            
            // 28.04.2016: tek sada! Bijo BUG!!! 
            GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rowIdx);
            //rtrans_rec.T_cij = artStat_rec.PrNabCij;

            //rtrans_rec.CalcTransResults(null);

            //PutDgvLineResultsFields1(rowIdx, rtrans_rec, true);
         }

         if(changeOccured)
         {
            PutDgvTransSumFields1();
         }
      }
   }

   public /*private*/ void OnExit_ValidateTTrestrictor(object sender, System.ComponentModel.CancelEventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(dbNavigationRestrictor_TT.RestrictedValues.Contains(vvtb.Text) == false)
      {
         ZXC.RaiseErrorProvider((Control)sender, "Nedozvoljeni TIP TRANSAKCIJE (TT).");
         e.Cancel = true;
      }
   }

   public /*private*/ void dtp_DokDate_ValueChanged_SetSkladAndPdvDate(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      // 23.04.2018: da ne provjerava
      bool isProjektTT = ZXC.TtInfo(Fld_TT).IsProjektTT;

    //if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Year != ZXC.projectYearFirstDay.Year                                             ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste godinu koja nije 'radna'.");
    //if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Year != ZXC.projectYearFirstDay.Year && !isProjektTT                             ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste godinu koja nije 'radna'.");
      if(Fld_DokDate != DateTimePicker.MinimumDateTime && Fld_DokDate.Year != ZXC.projectYearFirstDay.Year && !isProjektTT && ZXC.IsManyYearDB == false) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje. Zadali ste godinu koja nije 'radna'.");

      #region if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij) RecalcPrNabCijAndResultFields();

      // 20.08.2011: 
      if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij)
      {
       //// 13.01.2023: privremeno bili stavili ovo pitanje da ubrzamo (npr. Metaflex PSM to ZPC)
       //DialogResult result = MessageBox.Show("Da li zelite RecalcPrNabCijAndResultFields?\n\r\n\rMožda dugo traje.",
       //   "RecalcPrNabCijAndResultFields?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
       //if(result != DialogResult.OK) return; 

         // 18.03.2025: pocinje LJAŠ 2025 
         if(this is FakturPDUC)
         {
            FakturPDUC thePduc = (this as FakturPDUC);

            bool wasG2 = false;

            Crownwood.DotNetMagic.Controls.TabPage currTabPage = thePduc.ThePolyGridTabControl.SelectedTab;

            if(currTabPage.Title != TabPageTitle1) wasG2 = true;

            if(wasG2) thePduc.ThePolyGridTabControl.TabPages[TabPageTitle1].Selected = true;

            RecalcPrNabCijAndResultFields();

            if(wasG2) thePduc.ThePolyGridTabControl.TabPages[TabPageTitle2].Selected = true;

         }

         else RecalcPrNabCijAndResultFields();
      }

      #endregion if(faktur_rec.TtInfo.IsDokCijShouldBePrNabCij) RecalcPrNabCijAndResultFields();

      #region TRI - TRM

      // 20.11.2017:                                                         
      // Pri promjeni datuma, SVE podatke koji dolaze iz Artstat-a / Cache-a 
      // treba revalorizirati jer je artstat ovisan o datumu!!!              

      //if("ima li ijedan ovisan o artstat-u?") // ima li ikoja kolona u koju 'UpdateArtikl' puca nesto iz artstat-a 
      //{
      //   forič(redak)
      //   {
      //      PukniOsvježeniPodatak(rowIdx, podatak);
      //   }
      //}

      // ... ALI SI LIJEN PA CES I OVO SAMO CILJANO ZA TRM ... drz vodu dok majstori (penzija) ne odu 

      if(faktur_rec.TtInfo.TheTT == Faktur.TT_TRI)
      {
         TheVvTabPage.TheVvForm.RISK_SetColumnValues(VvForm.RISK_ColumnKindEnum.MalopCij);
      }


      #endregion TRI - TRM

      FakturDUC fakturDUC = this as FakturDUC;

      //26.05.2015.
    //27.06.2023.
    //if(CtrlOK(fakturDUC.tbx_DokDate2)        ) fakturDUC.Fld_DokDate2 = Fld_DokDate;
      if(CtrlOK(fakturDUC.tbx_DokDate2)     &&
        //this is UGNorAUN_PTG_DUC == false &&
        //this is DIZ_PTG_DUC      == false    
                IsPTG_UgAnDod_DUC == false) fakturDUC.Fld_DokDate2 = Fld_DokDate;

      // 17.02.2025: 
      if(ZXC.TtInfo(Fld_TT).IsPTG_YYinTtNum)
      {
         uint   arhivedTtNum      =  0;
         int    arhivedDokDate_YY =  0;
         string arhivedSkladCD    = "";

         bool isInEditNotInADD = TheVvTabPage.WriteMode == ZXC.WriteMode.Edit;

         DateTime newDokDate    = (sender as VvDateTimePicker).Value;
         int      newDokDate_YY = newDokDate.VvDokDate_YY();
         string   newSkladCD    = Fld_SkladCD;

         if(isInEditNotInADD)
         {
            arhivedTtNum      = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).TtNum  ;
            arhivedDokDate_YY = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).DokDate.VvDokDate_YY();
            arhivedSkladCD    = (TheVvTabPage.TheVvForm.TheArhivedVvDataRecord as Faktur).SkladCD;
         }

         bool is_skladCD_And_dokDate_YY_unchanged = arhivedSkladCD == newSkladCD && arhivedDokDate_YY == newDokDate_YY;

         //if(isInEditNotInADD && arhivedDokDate_YY == newDokDate_YY )
         if(isInEditNotInADD && is_skladCD_And_dokDate_YY_unchanged)
         {
            Put_NewTT_Num(arhivedTtNum);
         }
         else
         {
            Put_NewTT_Num(this.GetNewTtNum_2025());
         }
      }

      if(this is FakturExtDUC == false) return;
      FakturExtDUC fakturExtDUC = this as FakturExtDUC;

      if(CtrlOK(fakturExtDUC.tbx_SkladDate)) fakturExtDUC.Fld_SkladDate = Fld_DokDate;
      if(CtrlOK(fakturExtDUC.tbx_PdvDate  )) fakturExtDUC.Fld_PdvDate   = Fld_DokDate;
    
      // 09.2017: 
      #region RNZ Additions

      if(this is RNZDUC)
      {
         #region TtNum

         uint newTtNum = TheVvDao.GetNextTtNum(TheDbConnection, Fld_TT, /*skladCD4_ttNum*/"", /*isCentToCentMSI*/false, Fld_VezniDok, Fld_DokDate.Month);
         Put_NewTT_Num(newTtNum);

         #endregion TtNum

      } // if(this is RNZDUC) 

      #endregion RNZ Additions

   }

   #region PrimkaVersusNarudzbaMinus
   
   private bool GetIsInPrimkaVsNarudzbaMinus(string artiklCD, decimal primkaKol, Faktur faktur_rec, out decimal narudzbaKol)
   {
      #region Saznaj NarTT i NarTtNum

      narudzbaKol = 0;

      string narTt;
      uint narTtNum;

      narTt = Faktur.TT_NRD;
      narTtNum = faktur_rec.GetFirstVezaTtNumForTT(narTt);
      
      if(narTtNum.IsZero()) // Nije nasao NRD, probaj naci NRU 
      {
         narTt = Faktur.TT_NRU;
         narTtNum = faktur_rec.GetFirstVezaTtNumForTT(narTt);
      }

      if(narTtNum.IsZero()) // NEMA NARUDZBE U VEZI 
      {
         //narTt = "";
         //narTtNum = 0;

         return false;
      }

      #endregion Saznaj NarTT i NarTtNum

      // ako smo dosli do ovdje: narudzba je zadana - nije prazna 

      narudzbaKol = FakturDao.GetNarudzbaKolForArtikl(TheDbConnection, artiklCD, narTt, narTtNum);

      if(primkaKol > narudzbaKol) return true;
      else                        return false;
   }

   private void Issue_ArtiklIsInPrimkaVersusNarudzbaMinus_ErrorMessage(string artiklCD, decimal narudzbaKol, decimal primkaKol, int rowIdx)
   {
      ZXC.aim_emsg(MessageBoxIcon.Error, "VEĆA KOLIČINA NEGO NA NARUDŽBI!\n\nRedak: {0}\n\nArtikl: '{1}'\n\nKoličina sa narudžbe: {2}\n\nA na primku ste zadali količinu: {3}",
         rowIdx + 1, artiklCD, narudzbaKol.ToStringVv(), primkaKol.ToStringVv());
   }

   #endregion PrimkaVersusNarudzbaMinus

   public bool GetPagging_IndexAndCount_FromNapomena(string f_napomena, out int startRowIdx, out int rangeCount)
   {
      startRowIdx = -1;
      rangeCount  = -1;

      if(f_napomena.Replace(" ", "").StartsWith("Red:"))
      {
         string od_do_str = f_napomena.Replace(" ", "").Replace("Red:", "");
         string[] odAndDo = od_do_str.Split(new string[] { ":", "-", "/", "od" }, StringSplitOptions.None );

         if(odAndDo.Length > 1)
         {
            int redOd = ZXC.ValOrZero_Int(odAndDo[0]);
            int redDo = ZXC.ValOrZero_Int(odAndDo[1]);

            if(redDo > redOd)
            {
               startRowIdx = redOd - 1        ;
               rangeCount  = redDo - redOd + 1;

               return true;
            }

            return false;
         }

         return false;
      }
      else
      {
         return false;
      }
   }

   protected /*private*/ void OnExit_Beautify_TtNum(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(this.Visible            == false) return;
      if(vvtb.EditedHasChanges() == false) return;

      uint newTtNum=0;

      uint theUint = ZXC.ValOrZero_UInt(tbx_TtNum.Text);

      if(theUint.NotZero() && faktur_rec.TtInfo.IsSklCdInTtNum && theUint < Faktur.NultiTtNum)
      {
         newTtNum = Faktur.GetTtNumFromRbr(Fld_SkladCD, theUint);
      }
      else
      {
         newTtNum = theUint;
      }

      Put_NewTT_Num(newTtNum);

      oldTtNum = Fld_TtNum;
   }

   internal bool CouldClose_OPN { get { return (this is IRADUC || this is IzdatnicaDUC || this is IRA_MPC_DUC); } }

   #endregion FakturDUC_Validating

   #region OnExit BOR-SOBA-GOST

   public void OnExit_GOST_SOBA_BOR_SetOtherData(object sender, EventArgs e)
   {
      if(this is IZMDUC_2 == false) return;

      VvTextBox vvTB = sender as VvTextBox;

      if(vvTB.EditedHasChanges() == false) return;

      FakturExtDUC theDUC = this as FakturExtDUC;
      uint borTtNum;
      List<Rtrano> rtranoList;

      ZXC.GOST_SOBA_BOR_SetOtherData_InProgress = true;

      switch(vvTB.A0_JAM_Name)
      {
         // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>      
         case "tbx_kupdobCd"  : // Zadano: Ime i Prezime gosta - popuni BOR i sobu                                     
         case "tbx_kupdobName": // Zadano: Ime i Prezime gosta - popuni BOR i sobu                                     
         case "tbx_kupdobTk"  : // Zadano: Ime i Prezime gosta - popuni BOR i sobu                                     

            //if(theDUC.Fld_ProjektCD.NotEmpty() || theDUC.Fld_OsobaX.NotEmpty()) break;

          //rtranoList = RtranoDao.GetRtranoList_For_Gost(TheDbConnection, theDUC.Fld_KupdobCd);
          //if(rtranoList.Count.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Za gosta [{0}]\n\nnema BORavka!", theDUC.Fld_KupdobName); return; }
            borTtNum = RtranoDao.GetBOR_TtNum_ForGost(TheDbConnection, theDUC.Fld_KupdobCd);
            if(borTtNum.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Za gosta [{0}]\n\nnema BORavka!", theDUC.Fld_KupdobName); return; }

            theDUC.Fld_ProjektCD = Faktur.Set_TT_And_TtNum(Faktur.TT_BOR, borTtNum);
            theDUC.Fld_OsobaX    = RtranoDao.GetBOR_Soba_ForGost(TheDbConnection, theDUC.Fld_KupdobCd);

            break;
         // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>      
         case "tbx_osobaX"    : // Zadana: SOBA - popuni BOR i zadnji kupdob (gost) iz sobe                              

            //if(theDUC.Fld_ProjektCD.NotEmpty() || theDUC.Fld_KupdobCd.NotZero()) break;

            if(theDUC.Fld_OsobaX.IsEmpty()) { ZXC.GOST_SOBA_BOR_SetOtherData_InProgress = false; return; }

            rtranoList = RtranoDao.GetRtranoList_For_SobaBr(TheDbConnection, theDUC.Fld_OsobaX);
            if(rtranoList.Count.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Za sobu [{0}]\n\nnema BORavka!", theDUC.Fld_OsobaX); return; }

            theDUC.Fld_ProjektCD = Faktur.Set_TT_And_TtNum(Faktur.TT_BOR, rtranoList.Last().T_ttNum)  ;
            theDUC.Fld_KupdobCd  =                                        rtranoList.Last().T_paletaNo;
            theDUC.sifrarSorterType = VvSQL.SorterType.Code;
            theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobCd, EventArgs.Empty);
            break;
         // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>      
         case "tbx_Projekt"   : // Zadan:  BOR - popuni sobu (zadnji rtrano s BORa), popuni kupdoba (zadnji rtrano s BORa) 

            //if(theDUC.Fld_OsobaX.NotEmpty() || theDUC.Fld_KupdobCd.NotZero()) break;

            if(theDUC.Fld_ProjektCD.IsEmpty()) { ZXC.GOST_SOBA_BOR_SetOtherData_InProgress = false; return; }

            rtranoList = RtranoDao.GetRtranoList_ForProjektCD(TheDbConnection, theDUC.Fld_ProjektCD);
            if(rtranoList.Count.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nema BORavka [{0}]!", theDUC.Fld_ProjektCD); return; }

            theDUC.Fld_OsobaX    = rtranoList.Last().T_serno;
            theDUC.Fld_KupdobCd  = rtranoList.Last().T_paletaNo;
            theDUC.sifrarSorterType = VvSQL.SorterType.Code;
            theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobCd, EventArgs.Empty);

            break;
         // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>      

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "Unknown sender [{0}][{1}]\n\nIn OnExit_GOST_SOBA_BOR_SetOtherData()", vvTB.A0_JAM_Name, sender); break;
      }

      ZXC.GOST_SOBA_BOR_SetOtherData_InProgress = false;

   }

   #endregion OnExit BOR-SOBA-GOST

   #region MinusPolicy OvoOno, CheckPrevCij

   // 10.10.2023: abrakadabra_oldRtransKol_forCheckMinus
   public void OnEntryT_Kol_ForEditWriteMode_Get_dataLayerT_kol    (object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(TheVvTabPage.WriteMode  != ZXC.WriteMode.Edit) return;
      if(vvtb                    == null              ) return;
    //if(vvtb.Text               == this.originalText ) return;
    //if(vvtb.EditedHasChanges() == false             ) return;
      if(vvtb.Text.              IsEmpty()            ) return;
      if(faktur_rec.TtInfo.IsFinKol_TT == false       ) return;

    //this.dataLayerT_kol = ZXC.ValOrZero_Decimal(vvtb.Text, 8);

      uint rtransRecID = TheG.GetUint32Cell(ci.iT_recID, TheG.CurrentRow.Index, false);

      if(rtransRecID.IsZero()) return;

      Rtrans rtrans_rec = new Rtrans(rtransRecID);

      rtrans_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, rtrans_rec, rtransRecID, false);

      this.dataLayerT_kol = rtrans_rec.T_kol;
   }

   public void OnEntryT_Kolg2_ForEditWriteMode_Get_dataLayerT_kolg2(object sender, EventArgs e) // RTRANO !!! 
   {
      FakturPDUC theDUC = this as FakturPDUC;

      VvTextBox vvtb = sender as VvTextBox;

      if(TheVvTabPage.WriteMode  != ZXC.WriteMode.Edit) return;
      if(vvtb                    == null              ) return;
    //if(vvtb.Text               == this.originalText ) return;
    //if(vvtb.EditedHasChanges() == false             ) return;
      if(vvtb.Text.              IsEmpty()            ) return;

      string rtranoTT = TheG2.GetStringCell(theDUC.DgvCI2.iT_TT, TheG2.CurrentRow.Index, false);

      if(rtranoTT != Faktur.TT_MOI) return;

    //this.dataLayerT_kol = ZXC.ValOrZero_Decimal(vvtb.Text, 8);

      uint rtranoRecID = TheG2.GetUint32Cell(theDUC.DgvCI2.iT_recID, TheG2.CurrentRow.Index, false);

      if(rtranoRecID.IsZero()) return;

      Rtrano rtrano_rec = new Rtrano(rtranoRecID);

      rtrano_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, rtrano_rec, rtranoRecID, false);

      this.dataLayerT_kol = rtrano_rec.T_kol;
   }
   private   void OnExitT_Kol_ValidateCheckMinus  (object sender, System.ComponentModel.CancelEventArgs e)
   {
      if(faktur_rec.TtInfo.IsFinKol_TT == false) return;

      decimal maxKol=0, t_kol=0;
      int rowIdx = TheG.CurrentRow.Index;

      string artiklCD   = TheG.GetStringCell(ci.iT_artiklCD  , rowIdx, false);
      string artiklName = TheG.GetStringCell(ci.iT_artiklName, rowIdx, false);

      string skladCD    = HasRtrans_SkladCD_Exposed ? TheG.GetStringCell(ci.iT_skladCD, rowIdx, false) : Fld_SkladCD;

      // 23.04.2024: 
      bool isProductLineChecked = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isProductLine, rowIdx, false));
      if(isProductLineChecked)                 skladCD =        Fld_SkladCD2;
    //else                     dgvRtrans_rec.T_skladCD = faktur_rec.SkladCD ;

      bool isUmjetninaRtrans  = GetPdvKolTipEnumFromFirstLetter(TheG.GetStringCell(ci.iT_pdvKolTip, rowIdx, false)) == ZXC.PdvKolTipEnum.UMJETN;

      if(ThisRowWillProduceMinusKolSt(rowIdx, artiklCD, skladCD, ref maxKol, ref t_kol, false))
      {
         Issue_ArtiklIsInMinus_ErrorMessage(artiklCD, artiklName, maxKol, t_kol, rowIdx, isUmjetninaRtrans);

         if(isUmjetninaRtrans)                                                                                            e.Cancel = true;
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_ALL)                                                   e.Cancel = true;
         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TtInfo.IsMalopTT == false) e.Cancel = true;
      }
   }

   protected void OnExitT_Kolg2_ValidateCheckMinus(object sender, System.ComponentModel.CancelEventArgs e)
   {
      FakturPDUC theDUC = this as FakturPDUC;

      string rtranoTT = TheG2.GetStringCell(theDUC.DgvCI2.iT_TT, TheG2.CurrentRow.Index, false);

    //if(faktur_rec.TtInfo.IsFinKol_TT == false) return;
      if(rtranoTT != Faktur.TT_MOI) return;

      decimal maxKol =0, t_kol=0;
      int rowIdx = TheG2.CurrentRow.Index;

      string artiklCD   = TheG2.GetStringCell(theDUC.DgvCI2.iT_artiklCD  , rowIdx, false);
      string artiklName = TheG2.GetStringCell(theDUC.DgvCI2.iT_artiklName, rowIdx, false);

      string skladCD    = HasRtrano_SkladCD_Exposed ? TheG2.GetStringCell(theDUC.DgvCI2.iT_skladCD, rowIdx, false) : Fld_SkladCD;

      if(ThisRowWillProduceMinusKolSt(rowIdx, artiklCD, skladCD, ref maxKol, ref t_kol, true))
      {
         Issue_ArtiklIsInMinus_ErrorMessage(artiklCD, artiklName, maxKol, t_kol, rowIdx, false);

         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_ALL) e.Cancel = true;

         if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TtInfo.IsMalopTT == false) e.Cancel = true; // ZABRANA samo za VELEP 
      }
   }

   private void Issue_ArtiklIsInMinus_ErrorMessage(string artiklCD, string artiklName, decimal oldKolSt, decimal t_kol, int rowIdx, bool isUmjetninaRtrans)
   {
      if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TtInfo.IsMalopTT && isUmjetninaRtrans == false) // npr. Zagria, za MAL niti ne javljaj minuse 
      {
         return;
      }
      if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.ALOW_ALL_NO_MSG && isUmjetninaRtrans == false) // npr. Zagria, za MAL niti ne javljaj minuse 
      {
         return;
      }

      string minOrMaxStr = faktur_rec.TtInfo.IsFinKol_U ? "Minimalna" : "Maksimalna";

      ZXC.aim_emsg(MessageBoxIcon.Error, "U MINUSU!\n\nRedak: {0}\n\nArtikl: '{1} {4}'\n\n{5} kol: {2}\n\nZadana kol: {3}",
         rowIdx + 1, artiklCD, oldKolSt.ToStringVv(), t_kol.ToStringVv(), artiklName, minOrMaxStr);
   }

   private bool ThisRowWillProduceMinusKolSt(int rowIdx, string artiklCD, string skladCD, ref decimal kolStBeforeThisChange, ref decimal t_kol, bool isRtrano)
   {
      // 10.10.2023: abrakadabra_oldRtransKol_forCheckMinus: palimo svima, a ne samo SvDUH-u 
      //if(ZXC.IsSvDUH == false) return false; // dalje idemo samo ako JESMO SvDUH 

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      if(artikl_rec == null) return false;

      if(artiklCD.IsEmpty() || artikl_rec == null || artikl_rec.IsAllowMinus || artikl_rec.IsMinusOK) return false;

      ArtStat artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artiklCD, /*Fld_SkladCD*/ skladCD);

      if(artstat_rec != null) kolStBeforeThisChange = artstat_rec.StanjeKolFree;
      else                    kolStBeforeThisChange = 0.00M;

      uint recID = isRtrano ? TheG2.GetUint32Cell((this as FakturPDUC).DgvCI2.iT_recID, rowIdx, false) :
                              TheG.GetUint32Cell(ci.iT_recID, rowIdx, false);

      ZXC.WriteMode thisRtransWriteMode = ZXC.WriteMode.Add;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit && recID.IsPositive()) // znaci, ovo smo u ispravi i ovo je stari record 
      {
         thisRtransWriteMode = ZXC.WriteMode.Edit;
      }

      Rtrans rtrans_rec;

      if(isRtrano == false) // Rtrans! 
      {
         rtrans_rec = (Rtrans)GetDgvLineFields1(rowIdx, /*false*/true, null);
      }
      else // Rtrano! 
      {
         Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx, /*false*/true, null);

         rtrans_rec = new Rtrans(rtrano_rec, 0M, "", 0);
      }

      decimal deltaKol = rtrans_rec.GetDeltaKol(thisRtransWriteMode, this.dataLayerT_kol);

      t_kol = rtrans_rec.T_kol;

      decimal newKolSt = kolStBeforeThisChange + deltaKol;

      // 10.10.2023: abrakadabra_oldRtransKol_forCheckMinus: 
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.Edit && recID.IsPositive()) // znaci, ovo smo u ispravi i ovo je stari record 
      {
         kolStBeforeThisChange += this.dataLayerT_kol;
      }

      #region Check for nonUNIKATNI ulaz PPOM artikla

      if(artikl_rec.IsUMJETNINA && faktur_rec.TtInfo.IsFinKol_U)
      {
         decimal ulazKolBeforeThisChange;

         if(artstat_rec != null) ulazKolBeforeThisChange = artstat_rec./*StanjeKolFree*/UkUlazKol;
         else                    ulazKolBeforeThisChange = 0.00M;

         decimal deltaUlazKol = rtrans_rec.GetDeltaKol/*_Ulaz*/(thisRtransWriteMode, this.dataLayerT_kol);

         decimal newUkUlazKol = ulazKolBeforeThisChange + deltaUlazKol;

         if(newUkUlazKol > 1)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "PPOM artikl \n\r\n\r{0}\n\r\n\rne može imati više od jednog ulaza!", artikl_rec);
            return true;
         }
      } 

      #endregion Check for nonUNIKATNI ulaz PPOM artikla

      return (newKolSt.IsNegative());
   }
      
   public void OnExitT_Cij_CheckPrevCij(object sender, EventArgs e)
   {
      bool isSvdNrd    = ZXC.IsSvDUH && Fld_TT == Faktur.TT_NRD;
      bool isNotSvdNrd = !isSvdNrd                             ;
      // 18.04.2018: 
    //if(faktur_rec.TtInfo.IsAllSkladFinKol_U == false               ) return;
      if(faktur_rec.TtInfo.IsAllSkladFinKol_U == false && isNotSvdNrd) return;

      if(ArerWeIn_OutsideEU_DevizniDokument() == true) return;

      int currRow = TheG.CurrentRow.Index;

      ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)GetDgvLineFields1(currRow, false, null));

      if(artStat_rec == null) return;

      // 15.04.2023: 
    //decimal cijToCompare = artStat_rec.PrNabCij;
      decimal cijToCompare = artStat_rec.UlazCijLast;

      if(cijToCompare.IsZero()) return;

    // 12.04.2018:
    //decimal tolerancy =                                                                           50; 
      decimal tolerancy = ZXC.RRD.Dsc_TolerancOdstUlCij.NotZero() ? ZXC.RRD.Dsc_TolerancOdstUlCij : 50; 

      string artiklCD       = TheG.GetStringCell (ci.iT_artiklCD  , currRow, false);
      string artiklName     = TheG.GetStringCell (ci.iT_artiklName, currRow, false);
      decimal newCij        = TheG.GetDecimalCell(ci.iT_cij       , currRow, false);

      if(newCij.IsZero()) return;

      decimal stopaPromjene = ZXC.StopaPromjene(cijToCompare, newCij);

      if(Math.Abs(stopaPromjene) > tolerancy)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE!\n\nRedak: {0}\n\nArtikl: '{1} {5}'\n\nPrethodna ulazna cijena: {2}\n\nNova ulazna cijena: {3}\n\nOdstupanje: {4}%",
            currRow + 1, artiklCD, cijToCompare.ToStringVv(), newCij.ToStringVv(), stopaPromjene.ToString0Vv(), artiklName);
      }
   }

   #endregion MinusPolicy OvoOno

   #region UpdateArtikl

   private void AnyArtiklTextBox_OnGrid2_Leave(object sender, EventArgs e, VvDataGridView theGrid, int currRowIdx, Artikl artikl_rec)
   {
      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      bool isNovoUpareniSerno = false;

      if(artikl_rec != null)
      {
         theGrid.PutCell(ci2.iT_artiklCD   , currRowIdx, artikl_rec.ArtiklCD  );
         theGrid.PutCell(ci2.iT_artiklName , currRowIdx, artikl_rec.ArtiklName);
         theGrid.PutCell(ci2.iT_jm         , currRowIdx, artikl_rec.JedMj     );
         theGrid.PutCell(ci2.iT_artiklTS   , currRowIdx, artikl_rec.TS        );

         if(this is MOD_PTG_DUC)
         {
            theGrid.PutCell(ci2.iR_artiklCD_Old, currRowIdx, artikl_rec.ArtiklCD);
            if(artikl_rec.TS == ZXC.PCK_TS)
            {
               theGrid.PutCell(ci2.iR_ramOld, currRowIdx, artikl_rec.PCK_RAM);
               theGrid.PutCell(ci2.iR_hddOld, currRowIdx, artikl_rec.PCK_HDD);
            }
         }

         if(this is MOD_PTG_DUC && artikl_rec.TS == ZXC.PCK_TS)
         {
            bool isSernoEmpty = theGrid.GetStringCell(ci2.iT_serno, currRowIdx, false).IsEmpty();

            if(isSernoEmpty)
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "PCK Artikl zadajte tek nakon unosa serijskog broja.");

               theGrid.ClearRowContent(currRowIdx);
               (this as MOD_PTG_DUC).Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
               return;
            }
            else if(ThisIs_MOC_rowIndex(currRowIdx)) // serno je zadan ... uparujemo serno i artikl
            {
               isNovoUpareniSerno = true;

               string  cilj_MOC_ArtiklCD = (this as MOD_PTG_DUC).Fld_PrjArtCD;

               decimal cilj_MOC_RAM = Fld_Decimal01;
               decimal cilj_MOC_HDD = Fld_Decimal02;

               decimal artikl_OLD_RAM = artikl_rec.PCK_RAM;
               decimal artikl_OLD_HDD = artikl_rec.PCK_HDD;

               decimal ramPlus  = cilj_MOC_RAM - artikl_OLD_RAM;
               decimal ramMinus = 0M;
               decimal hddPlus  = cilj_MOC_HDD - artikl_OLD_HDD;
               decimal hddMinus = 0M;

               if(ramPlus.IsNegative()) { ramMinus = -ramPlus; ramPlus = 0; }
               if(hddPlus.IsNegative()) { hddMinus = -hddPlus; hddPlus = 0; }

               TheG2.PutCell(ci2.iT_RAM_plus , currRowIdx, ramPlus );
               TheG2.PutCell(ci2.iT_RAM_minus, currRowIdx, ramMinus);
               TheG2.PutCell(ci2.iT_HDD_plus , currRowIdx, hddPlus );
               TheG2.PutCell(ci2.iT_HDD_minus, currRowIdx, hddMinus);

               TheG2.PutCell(ci2.iT_artiklCD, currRowIdx, cilj_MOC_ArtiklCD);
               TheG2.PutCell(ci2.iT_RAM_new , currRowIdx, cilj_MOC_RAM     );
               TheG2.PutCell(ci2.iT_HDD_new , currRowIdx, cilj_MOC_HDD     );

               // ako smo tu onda je novouparenje serno-a sa MOC artiklom 
               for(int i = 1; i <= 6; i++) SendKeys.Send("{TAB}");
            }

         } // if(this is MOD_PTG_DUC && artikl_rec.TS == ZXC.PCK_TS)

       //if(this is MOD_PTG_DUC && ThisIs_MOC_rowIndex(currRowIdx) && artikl_rec.TS != ZXC.PCK_TS)
       //{
       //   ZXC.aim_emsg(MessageBoxIcon.Stop, "Na prvih {0} redaka se očekuje MOC PCK artikl (cilj modifikacije).", (int)(this as MOD_PTG_DUC).Fld_someMoney);
       //
       //   theGrid.ClearRowContent(currRowIdx);
       //   return;
       //}

         if(this is MOD_PTG_DUC && ThisIs_MOC_rowIndex(currRowIdx) && artikl_rec.PCK_BazaCD != (this as MOD_PTG_DUC).Fld_PTG_PCKbaza)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Na prvih {0} redaka se očekuje MOC PCK artikl (cilj modifikacije) PCK baze {1}.", 
               (int)(this as MOD_PTG_DUC).Fld_someMoney, (this as MOD_PTG_DUC).Fld_PTG_PCKbaza);

            theGrid.ClearRowContent(currRowIdx);
            (this as MOD_PTG_DUC).Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
            return;
         }

         if(this is MOD_PTG_DUC && artikl_rec.TS != ZXC.KMP_TS && artikl_rec.TS != ZXC.PCK_TS)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Artikl nije niti 'PCK' niti 'KMP'");

            theGrid.ClearRowContent(currRowIdx);
            (this as MOD_PTG_DUC).Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
            return;
         }

         if(ZXC.IsPCTOGO && (artikl_rec.TS == ZXC.PCK_TS || artikl_rec.TS == ZXC.KMP_TS))
         {
            theGrid.PutCell(ci2.iT_ramKlasa, currRowIdx, artikl_rec.Grupa2CD);
            theGrid.PutCell(ci2.iT_hddKlasa, currRowIdx, artikl_rec.Grupa3CD);
            theGrid.PutCell(ci2.iT_artiklTS, currRowIdx, artikl_rec.TS      );
            theGrid.PutCell(ci2.iT_kol     , currRowIdx, 1.00M              );

            if(artikl_rec.TS == ZXC.PCK_TS && !isNovoUpareniSerno)
            {
               theGrid.PutCell(ci2.iT_RAM_new, currRowIdx, artikl_rec.PCK_RAM);
               theGrid.PutCell(ci2.iT_HDD_new, currRowIdx, artikl_rec.PCK_HDD);
            }

            if(artikl_rec.TS == ZXC.KMP_TS)
            {
               bool isRAM = artikl_rec.Grupa1CD == ZXC.RAM_GR1;
               bool isHDD = artikl_rec.Grupa1CD == ZXC.HDD_GR1;

             //decimal kolPutaKapacitet = /*rtrano_rec.T_kol **/ artikl_rec.Zapremina;
             //
             //if(isRAM) TheG2.PutCell(ci2.iT_dimY, currRow, kolPutaKapacitet);
             //if(isHDD) TheG2.PutCell(ci2.iT_decB, currRow, kolPutaKapacitet);

               if(isRAM) TheG2.PutCell(ci2.iT_dimY, currRowIdx, artikl_rec.PCK_RAM);
               if(isHDD) TheG2.PutCell(ci2.iT_decB, currRowIdx, artikl_rec.PCK_HDD);
            }
            //(this as FakturPDUC).SetColorsPCKartikl();
         }

         #region HasRtrano_SkladCD_Exposed

         if(HasRtrano_SkladCD_Exposed)
         {
            theGrid.PutCell(ci2.iT_skladCD, currRowIdx, Fld_SkladCD);
         }

         #endregion HasRtrano_SkladCD_Exposed

         if(this is MOD_PTG_DUC)
         {
            (this as MOD_PTG_DUC).Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
            (this as MOD_PTG_DUC).Put_MOD_Semafor_Labels();
         }

      } // if(artikl_rec != null)
   }

   // ****** 
   public void AnyArtiklTextBox_OnGrid_Leave(object sender, EventArgs e)
   {
      #region Init stuff
      
      if(isPopulatingSifrar)                           return;
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text == this.originalText) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      int currRowIdx = vvtb_editingControl.EditingControlRowIndex;

      this.originalText = vvtb_editingControl.Text;
      Artikl artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

      // 28.10.2014: 
      if(CheckIsArtiklKindAdequate(currRowIdx) == false) return; // !!! 

      // 14.12.2016: THSHOP na INM/INV MORA prvo stisnuti SubModulAkciju 'Artikli' 
      // pa to pokusavamo detektirati da li je poceo rucno zadavati artikl na prvom redku 
      if(ZXC.IsTEXTHOany && currRowIdx.IsZero() && faktur_rec.TtInfo.IsInventura)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ručno dodavati artikle možete tek nakon akcije 'Artikli'\n\nI to samo artikle koji su, eventualno,\n\npronađeni u skladištu a po programskom stanju ih uopće nema.");
         return;
      }

      #region Check duplicate BarCodes

      bool thisIsBarCodeAction = (artikl_rec != null && ZXC.RRD.Dsc_IsBarCode == true && this.sifrarSorterType == VvSQL.SorterType.BarCode);
      // 16.09.2013:  
      if(thisIsBarCodeAction)
      {
         var artiklsWithSameBarCode = ArtiklSifrar.Where(a => a.BarCode1 == vvtb_editingControl.Text);

         if(artiklsWithSameBarCode.Count() > 1)
         {
            string errMessage = "";

            foreach(Artikl artikl in artiklsWithSameBarCode)
            {
               errMessage += "[" + artikl.ArtiklCD + "] [" + artikl.ArtiklName + "]\n";
            }

            ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: ArtiklSifrar sadrži više od jednog artikla sa BarKodom: [{0}]\n\n{1}", vvtb_editingControl.Text, errMessage);
         }

         // !!! IF thisIsBarCodeAction and artikl already exists on grid, just sum kol 
         if(true) // int he future let the user decide this bihaveour 
         {
            string  exixtingBarCode;
            decimal exixtingKol;
            for(int rIdx = 0; rIdx < theGrid.RowCount - 1; ++rIdx)
            {
               exixtingBarCode = theGrid.GetStringCell(ci.iT_barCode1, rIdx, false);

               if(theGrid.CurrentRow.Index != rIdx && exixtingBarCode == vvtb_editingControl.Text) // This artikl already exists on grid! 
               {
                  exixtingKol = theGrid.GetDecimalCell(ci.iT_kol, rIdx, false);

                  theGrid.PutCell(ci.iT_kol,      rIdx,                     exixtingKol + 1); // increment existing kol  
                  theGrid.PutCell(ci.iT_barCode1, theGrid.CurrentRow.Index, ""             ); // clear CurrentCell value 

                  GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);

                  SendKeys.Send("+({TAB})"); // Shift + Tab 

                  goto END_Action_LABEL;
               }

            } // for(int rIdx = 0; rIdx < theGrid.RowCount - 1; ++rIdx) 
         } // if(true) 
      } // if(thisIsBarCodeAction) 

      #endregion Check duplicate BarCodes

      bool isOutsideEU_DevizniDokument = ArerWeIn_OutsideEU_DevizniDokument();
      
      bool isIzvozniRacun = faktur_rec.TtInfo.IsIzlazniPdvTT && ArerWeIn_DevizniDokument();

      bool isRtranoSecondGrid = theGrid.Name.StartsWith("Rtrano");

      // ##################################################################### 
      if(isRtranoSecondGrid) // ############################################## 
      {
         if(IsPTG_WithSerno_DUC && artikl_rec != null)
         {
            FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

            string theSerno = TheG2.GetStringCell(ci2.iT_serno, currRowIdx, false);
            theGrid.ClearRowContent(currRowIdx);

            if(theSerno.NotEmpty())
            {
               (PCK_Unikat sernoInfo, Rtrano lastRtrano_rec) sernoData = RtranoDao.Get_PCK_Unikat_And_LastRtrano(TheDbConnection, theSerno);

               if(sernoData != (null, null))
               {
                  if(sernoData.sernoInfo.PCK_ArtCD == artikl_rec.ArtiklCD)
                  {
                     theGrid.PutCell(ci2.iT_serno, currRowIdx, theSerno);
                  }
                  else
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Error, "Prethodno upisani serijski broj [{0}] ne odgovara artiklu [{1}]\n\r\n\rnego artiklu [{2}]\n\r\n\rZadajte serijski broj ponovo.",
                        theSerno, artikl_rec.ArtiklCD, sernoData.sernoInfo.PCK_ArtCD);
                  }

               } // if(sernoData != (null, null))
               else // novo uparivanje 
               {
                  theGrid.PutCell(ci2.iT_serno, currRowIdx, theSerno);
               }

               if(artikl_rec.TS == ZXC.PCK_TS)
               {
                  AnyArtiklTextBox_OnGrid2_Leave(sender, e, theGrid, currRowIdx, artikl_rec);
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema smisla uparivati serijski broj sa artiklom koji nije PCK artikl");
               }

            } // if(theSerno.NotEmpty())

            else // theSerno je empty, .. nije zadan 
            {
               AnyArtiklTextBox_OnGrid2_Leave(sender, e, theGrid, currRowIdx, artikl_rec);

               if(this is MOD_PTG_DUC)
               {
                  #region Check For Minus

                  //TtInfo ttInfoOfThisRow = GetTtInfoOfThisRow(currRow);

                  Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(currRowIdx, false, null);

                //if(GetTtInfoOfThisRow(currRowIdx).IsFinKol_I)
                  if(rtrano_rec.T_TT == Faktur.TT_MOI)
                  {
                     Rtrans olfaRtrans_rec = new Rtrans(rtrano_rec, 0M, "", 0);

                     ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, olfaRtrans_rec);

                     if((artStat_rec == null && artikl_rec .IsMinusNotOK) ||
                        (artStat_rec != null && artStat_rec.StanjeKolFree.IsZeroOrNegative() && artStat_rec.IsMinusNotOK))
                     {
                        if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TtInfo.IsMalopTT) goto skiMessageLabel; // NE javljaj ako je malop i DENY_VEL_ALLOW_MAL ('Zagria pattern')
                        if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.ALOW_ALL_NO_MSG                                  ) goto skiMessageLabel; // NE javljaj ako je malop i DENY_VEL_ALLOW_MAL ('Zagria pattern')
                        if(artikl_rec.IsGlass                                                                                 ) goto skiMessageLabel; // NE javljaj ako je čaša u ugostiteljstvu 

                        ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Zadali ste artikl [" + artikl_rec.ArtiklCD + "] kojega nema na stanju skladišta '" + Fld_SkladCD + "'.");

                        skiMessageLabel: ;

                     }
                  }

                  #endregion Check For Minus
               }
            }

         } // if(IsPTG_WithSerno_DUC)

         else // classic 
         {
            theGrid.ClearRowContent(currRowIdx);

            AnyArtiklTextBox_OnGrid2_Leave(sender, e, theGrid, currRowIdx, artikl_rec);
         }

         return;

      } // if(isRtranoSecondGrid) // ############################################## 

      // ##################################################################### 

      #endregion Init stuff

      #region Reset row results, recalc document results (in case this is old DGV row - zamjena artikla na stavci)

      Rtrans                ZIZbackupRtrans_rec = null;
      ZXC.VvUtilDataPackage ZIZbackupValues     = new ZXC.VvUtilDataPackage();

      if(this is ZIZ_PTG_DUC) // save some data before ClearRowContent
      {
         ZIZbackupRtrans_rec = (Rtrans)GetDgvLineFields1(currRowIdx, false, null);

         ZIZbackupValues.TheStr1 = TheG.GetStringCell(ci.iT_opis    , currRowIdx, false);
         ZIZbackupValues.TheStr2 = TheG.GetStringCell(ci.iT_skladCD , currRowIdx, false);
         ZIZbackupValues.TheStr3 = TheG.GetStringCell(ci.iT_TT2     , currRowIdx, false);
         ZIZbackupValues.TheStr4 = TheG.GetStringCell(ci.iT_opis2   , currRowIdx, false);
         ZIZbackupValues.TheStr5 = TheG.GetStringCell(ci.iT_skladCD2, currRowIdx, false);
      }

      theGrid.ClearRowContent(currRowIdx); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 

      if(this is ZIZ_PTG_DUC) // restore some data after ClearRowContent
      {
         // tu vrati kao da inicijaliziras a samo uzmi u obzir sacuvani tt i vidi Fld_ ove skladista
         TheG.PutCell(ci.iT_TT      , currRowIdx, /*Faktur.TT_ZIZ                */ ZIZbackupRtrans_rec.T_TT);
         TheG.PutCell(ci.iT_opis    , currRowIdx, /*ZIZ_PTG_DUC.ZIZ_DUC_izlazText*/ ZIZbackupValues.TheStr1 );
         TheG.PutCell(ci.iT_skladCD , currRowIdx, /*Fld_SkladCD                  */ ZIZbackupValues.TheStr2 );
         TheG.PutCell(ci.iT_TT2     , currRowIdx, /*Faktur.TT_ZI2                */ ZIZbackupValues.TheStr3 );
         TheG.PutCell(ci.iT_opis2   , currRowIdx, /*ZIZ_PTG_DUC.ZIZ_DUC_ulazText */ ZIZbackupValues.TheStr4 );
         TheG.PutCell(ci.iT_skladCD2, currRowIdx, /*luiSkladUNJ.Cd               */ ZIZbackupValues.TheStr5 );
      }

      PutDgvTransSumFields();
      
      PutTransSumToDocumentSumFields();
      
      #endregion Reset row results, recalc document results (in case this is old DGV row - zamjena artikla na stavci)

      #region if(artikl_rec != null)

      TtInfo TtInfoOfThisRow = GetTtInfoOfThisRow(currRowIdx);

      if(artikl_rec != null)
      {
         #region ArtiklCD, ArtiklName, JedMj, Konto, BarCode1

         theGrid.PutCell(ci.iT_artiklCD   , currRowIdx, artikl_rec.ArtiklCD  );
         theGrid.PutCell(ci.iT_artiklName , currRowIdx, artikl_rec.ArtiklName);
         theGrid.PutCell(ci.iT_barCode1   , currRowIdx, artikl_rec.BarCode1  );

         theGrid.PutCell(ci.iT_jedMj      , currRowIdx, artikl_rec.JedMj     );
         theGrid.PutCell(ci.iT_isIrmUsluga, currRowIdx, VvCheckBox.GetString4Bool(artikl_rec.IsMinusOK_or_UDP_Artikl));

         // 25.01.2017: 
         if(ZXC.IsRNMnotRNP) // metaflex
         {
          //theGrid.PutCell(ci.iT_ppmvOsn, currRow, artikl_rec.MasaNetto);
            theGrid.PutCell(ci.iT_ppmvOsn, currRowIdx, artikl_rec.R_orgPak );
         }

         // 11.3.2011:
         if(artikl_rec.TS == "UDP" && faktur_rec.TtInfo.IsIzlazniPdvTT == true) // dakle, UDP - 'Usluga Daljnja Prodaja' na IRA, IFA, IOD, IPV ide prazan konto da bi kod fak2nal dobio default-ni kto byRules... 
         {
            // Don't put konto. Leave it empty 
         }
         else
         {
            theGrid.PutCell(ci.iT_konto, currRowIdx, artikl_rec.Konto);
         }

         if(ZXC.IsPCTOGO && (artikl_rec.TS == ZXC.PCK_TS || artikl_rec.TS == ZXC.KMP_TS))
         { 
            theGrid.PutCell(ci.iT_ramKlasa, currRowIdx, artikl_rec.Grupa2CD);
            theGrid.PutCell(ci.iT_hddKlasa, currRowIdx, artikl_rec.Grupa3CD);

            theGrid.PutCell(ci.iT_doCijMal, currRowIdx, artikl_rec.PCK_RAM);
            theGrid.PutCell(ci.iT_noCijMal, currRowIdx, artikl_rec.PCK_HDD);

            theGrid.PutCell(ci.iT_artiklTS, currRowIdx, artikl_rec.TS);
         }

         #endregion ArtiklCD, ArtiklName, JedMj, Konto

         #region PdvSt, PdvKolTip

         // 26.02.2016: 
         //if(faktur_rec.TtInfo.IsMalopTT || (ZXC.CURR_prjkt_rec.IS_IN_PDV && faktur_rec.IsExtendable && isOutsideEU_DevizniDokument == false                                          ))
         //21.03.2018:                                                                                                                                                                 
         //if(faktur_rec.TtInfo.IsMalopTT || (ZXC.CURR_prjkt_rec.IS_IN_PDV && faktur_rec.IsExtendable && isOutsideEU_DevizniDokument == false && isIzvozniRacun == false               ))
         if(faktur_rec.TtInfo.IsMalopTT || (ZXC.CURR_prjkt_rec.IS_IN_PDV && faktur_rec.IsExtendable && isOutsideEU_DevizniDokument == false && isIzvozniRacun == false) || ZXC.IsSvDUH)
         {
            if(artikl_rec.PdvKat.NotEmpty())
            {
               VvLookUpItem lui = ZXC.luiListaPdvKat.GetLuiForThisCd(artikl_rec.PdvKat);
               theGrid.PutCell(ci.iT_pdvSt, currRowIdx, lui.Number);
            }
          //else   if(currRow == 0)
            else /*if(currRow == 0)*/ // 19.02.2012: uvijek stavi 25 
            {
               theGrid.PutCell(ci.iT_pdvSt, currRowIdx, faktur_rec.CommonPdvSt);
            }

            //theGrid.PutCell(ci.iT_pdvKolTip, currRow, GetOneLetter4PdvKolTip(faktur_rec.PdvKolTip));

            // 14.12.2013: 
            if(artikl_rec.IsPNP)
            {
               theGrid.PutCell(ci.iT_pnpSt, currRowIdx, ZXC.RRD.Dsc_PnpSt);
            }
         }

         #endregion PdvSt, PdvKolTip

         #region Ignore ImportCij 05.03.2018

         if(ZXC.RRD.Dsc_IsIgnoreImportCij)
         {
            artikl_rec.ImportCij = 0.00M;
         }

         #endregion Ignore ImportCij 05.03.2018

         // --- CIJENE Manager 
         #region PreDefs, Check For Minus

         // "CV1"-Cjenik VPC1 "CV2"-Cjenik VPC2 "CM1"-Cjenik MPC  "CDE"-Cjenik Devizni "MNK"-Cjenik MinKol  "RB1"-Cjenik Rabat1  "RB2"-Cjenik Rabat2  "MRZ"-Cjenik MARZA   

         if(faktur_rec.TtInfo.IsArtStatInfoNeededTT) // Ili zbog cjenika ili zbog max Izlaz Kol, ... 
         {
            ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)GetDgvLineFields1(currRowIdx, false, null));

            if(artStat_rec != null)
            {

               if(faktur_rec.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_CJENIK)
               {
                  #region Rabat1&2

                  if(faktur_rec.TT == Faktur.TT_IRM && ((this as FakturExtDUC).Fld_NacPlacRbt).NotZero())
                  {
                     theGrid.PutCell(ci.iT_rbt1St, currRowIdx, (this as FakturExtDUC).Fld_NacPlacRbt);
                  }
                  else
                  {
                     theGrid.PutCell(ci.iT_rbt1St, currRowIdx, artStat_rec.PreDefRbt1);
                     theGrid.PutCell(ci.iT_rbt2St, currRowIdx, artStat_rec.PreDefRbt2);
                  }

                  // rabat ovisan o cycle momentu 
                  // 13.05.2015: 
                //if(ZXC.IsTEXTHOany                                                        && faktur_rec.TT == Faktur.TT_IRM)
                  if(ZXC.IsTEXTHOany && ZXC.IsTEXTHOatypicShop(faktur_rec.SkladCD) == false && faktur_rec.TT == Faktur.TT_IRM)
                  {
                     // 20.12.2017: izloirano u zasebnu metodu. 
                     decimal TH_IRM_Rabat = Get_TH_IRM_Rabat(faktur_rec.SkladCD, faktur_rec.DokDate, artikl_rec);

                     if(TH_IRM_Rabat.NotZero()) theGrid.PutCell(ci.iT_rbt1St, currRowIdx, TH_IRM_Rabat);

                     //QQQ  20.12.2017: Comment START 
                     //QQQ ZXC.TH_CycleMoment TH_CycleMoment = ZXC.TH_GetCycleMoment(faktur_rec.DokDate, ZXC.IsTH_5WeekShop(faktur_rec.SkladCD), faktur_rec.SkladCD);
                     //QQQ 
                     //QQQ decimal thRbt1St = 0.00M;
                     //QQQ 
                     //QQQ if(artikl_rec.Grupa3CD == Artikl.ProdRobaGrCD) // na vrecice ProAndNabGrCD nema rabata 
                     //QQQ {
                     //QQQ    if(TH_CycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_2___20_posto                                    ) thRbt1St = 20.00M;
                     //QQQ    if(TH_CycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_3_Dan_1___happy_hour_30_posto && Fld_IsHappyHour) thRbt1St = 30.00M;
                     //QQQ    if(TH_CycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_3_Dan_2___happy_hour_50_posto && Fld_IsHappyHour) thRbt1St = 50.00M;
                     //QQQ    if(TH_CycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_3_Dan_3_4___30_posto                            ) thRbt1St = 30.00M;
                     //QQQ    if(TH_CycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_3_Dan_5_6___50_posto                            ) thRbt1St = 50.00M;
                     //QQQ 
                     //QQQ    // 13.05.2015: ali samo za 5week
                     //QQQ  //if(TH_CycleMoment == ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_67___do_25kn_happy_hour_20_posto    && Fld_IsHappyHour) thRbt1St = 20.00M;
                     //QQQ  //if(TH_CycleMoment == ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_23___do_20kn_happy_hour_25_posto    && Fld_IsHappyHour) thRbt1St = 25.00M;
                     //QQQ  //if(TH_CycleMoment == ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_567__do_15kn_happy_hour_33_33_posto && Fld_IsHappyHour) thRbt1St = 33.33M;
                     //QQQ  //if(TH_CycleMoment == ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_23___do_10kn_happy_hour_50_posto    && Fld_IsHappyHour) thRbt1St = 50.00M;
                     //QQQ }
                     //QQQ 
                     //QQQ // 30.07.2015: temporary in comment: 
                     //QQQ // 19.05.2016: uncommented: 
                     //QQQ // 13.07.2016: suspend automatic rabat in unusual 6week cycle: 11.07.2016 - 21.08.2016) 
                     //QQQ // 03.10.2016: suspend automatic rabat in unusual 6week cycle: 26.09.2016 - 06.11.2016) 
                     //QQQ //bool is5weekClassic = faktur_rec.DokDate >= ZXC.THcycle_ZeroDate5weekClassicNEW2;
                     //QQQ //bool is5weekClassic = faktur_rec.DokDate >= ZXC.THcycle_ZeroDate5weekClassicNEW3;
                     //QQQ bool is5weekClassic = faktur_rec.DokDate >= ZXC.THcycle_ZeroDate5weekClassicNEW4;
                     //QQQ if(is5weekClassic && thRbt1St.NotZero()) theGrid.PutCell(ci.iT_rbt1St, currRow, thRbt1St);
                     //QQQ  20.12.2017: Comment END 

                  } // if(ZXC.IsTEXTHOany && ZXC.IsTEXTHOatypicShop(faktur_rec.SkladCD) == false && faktur_rec.TT == Faktur.TT_IRM)

                  #endregion Rabat1&2

                  #region Vpc1Policy is PrNabCij uvecana za marzu from PreDef

                  if(artikl_rec.Vpc1Policy == ZXC.ArtiklVpc1Policy.MARZA)
                  {
                     theGrid.PutCell(ci.iT_cij, currRowIdx, artStat_rec.PrNabCijPlusMarza);
                  }

                  #endregion Vpc1Policy is PrNabCij uvecana za marzu

                  #region Vpc1Policy is VP1 or VP2 from PreDef

                  else
                  {
                     switch(faktur_rec.CjenikTT)
                     {
                        case Faktur.TT_CJ_VP1: // VELEPRODAJNI cjenik 1 
                           // 09.06.2014: start _______________________________________________________________________________ 
                           VvLookUpItem skladLUI = ZXC.luiListaSkladista.GetLuiForThisCd(Fld_SkladCD);
                           bool isSklKomisija    = skladLUI  == null ? false : skladLUI.Integer  > 10;
                           if(isSklKomisija && skladLUI .Cd.ToUpper().StartsWith("VPSK")) isSklKomisija  = false; // ako SkladCD1 pocne sa 'VPSK' tada NIJE komisija bez obzira na druge obzire 

                           //25.03.2025. PCTOGO ima dvocifrena skladista ali nema komisiju
                           if(ZXC.IsPCTOGO) isSklKomisija = false; 

                           if(isSklKomisija)
                           {
                              string komArtCD = ((Rtrans)GetDgvLineFields1(currRowIdx, false, null)).T_artiklCD;

                              VvLookUpItem baseSkladLUI = ZXC.luiListaSkladista.GetBaseSkladLUI(skladLUI); // glavno skladiste 
                              string mainSkladCD = baseSkladLUI.Cd;

                              ArtStat komisArtStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, komArtCD, mainSkladCD, Fld_DokDate);

                              theGrid.PutCell(ci.iT_cij, currRowIdx, komisArtStat_rec.PreDefVpc1);
                           }
                           // 09.06.2014: end __________________________________________________________________________________ 
                           else if(artStat_rec.PreDefVpc1.NotZero())
                           {
                              theGrid.PutCell(ci.iT_cij, currRowIdx, artStat_rec.PreDefVpc1);
                           }
                           else /* user ne koristi CjenikDUC nego cijene drzi na samoj skladisnoj kartici (npr. zbog importa) */
                           {
                              decimal theVPC = artikl_rec.ImportCij;

                              #region I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                              if(ZXC.RRD.Dsc_VpcMpcMarzaTheSame4VPC == true) // i u veleprodaji digni VpcMpcMarza pa daj rabat da bude opet isto 
                              {
                                 theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);

                                 decimal rbtSt = ZXC.VvGet_rbtSt_100to90(theVPC, artikl_rec.ImportCij);

                                 theGrid.PutCell(ci.iT_rbt1St, currRowIdx, rbtSt);
                              }

                              #endregion I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                              theGrid.PutCell(ci.iT_cij, currRowIdx, /*artikl_rec.ImportCij*/ theVPC);
                           }
                           break;

                        case Faktur.TT_CJ_VP2: // VELEPRODAJNI cjenik 2 

                           Rtrans rtransVP2_rec = new Rtrans();
                           string artiklCD = TheG.GetStringCell(ci.iT_artiklCD, currRowIdx, false);
                           bool success = FakturDao.SetMeLastRtransForArtiklAndTtNum(TheDbConnection, rtransVP2_rec, faktur_rec.CjenikTT, faktur_rec.CjenTTnum, artiklCD, false);
                           if(success) theGrid.PutCell(ci.iT_cij, currRowIdx, rtransVP2_rec.T_cij);
                           break;

                        case Faktur.TT_CJ_MP: // MALOPRODAJNI cjenik 

                           decimal theMPC = 0.00M;

                           if(artStat_rec.PreDefMpc1.NotZero())
                           {
                              theMPC = artStat_rec.PreDefMpc1;

                           }
                           else /* user ne koristi CjenikDUC nego cijene drzi na samoj skladisnoj kartici (npr. zbog importa) */
                           {
                              // 02.01.2023: 
                              if(ZXC.IsTEXTHOany == false)
                              {
                                 decimal theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);
                                 theMPC = ZXC.VvGet_125_on_100(theVPC, faktur_rec.CommonPdvSt);
                              }

                              decimal theCijPop = ZXC.VvGet_90_from_100(theMPC, TheG.GetDecimalCell(ci.iT_rbt1St, currRowIdx, false));

                              theGrid.PutCell(ci.iT_cij_kcrp, currRowIdx, theCijPop); // IRM! 

                           }

                           if(theMPC.IsZero()) // najder PreDef, najder importCij. Let's 'LastMalopCij' 
                           {
                              theMPC = artStat_rec.LastUlazMPC;
                           }

                           theGrid.PutCell(ci.iT_cij,     currRowIdx, theMPC);

                           // 24.09.2018: cijela regija 'UMJETNINA' preseljena van switch-a 
                           #region UMJETNINA set T_ppmvOsn(tj. nabCij)
                           //
                           //   // 17.09.2018: 
                           // //if(artikl_rec.IsUMJETNINA &&  faktur_rec.TT == Faktur.TT_IRM                                   ) // UMJETNINA               na MALOP IRA 
                           //   if(artikl_rec.IsPDVonRUC  && (faktur_rec.TT == Faktur.TT_IRM || faktur_rec.TT == Faktur.TT_IRA)) // UMJETNINA ili RabVozilo na IRA, IRM  
                           //   {
                           //      theGrid.PutCell(ci.iT_ppmvOsn, currRow, artStat_rec.PrNabCij); // (ovo - theMPC) = osnovica za PDV 
                           //   }

                           #endregion UMJETNINA set T_ppmvOsn(tj. nabCij)

                           break; // case Faktur.TT_CJ_MP: // MALOPRODAJNI cjenik 

                     }

                     #region UMJETNINA set T_ppmvOsn(tj. nabCij)

                     // 17.09.2018: 
                   //if(artikl_rec.IsUMJETNINA &&  faktur_rec.TT == Faktur.TT_IRM                                   ) // UMJETNINA               na MALOP IRA 
                   //if(artikl_rec.IsPDVonRUC  && (faktur_rec.TT == Faktur.TT_IRM || faktur_rec.TT == Faktur.TT_IRA)) // UMJETNINA ili RabVozilo na IRA, IRM  
                     // 01.10.2018: vracamo samo na IRM
                   //if(artikl_rec.IsUMJETNINA &&  faktur_rec.TT == Faktur.TT_IRM                                   ) // UMJETNINA               na MALOP IRA 
                     {
                        theGrid.PutCell(ci.iT_ppmvOsn, currRowIdx, artStat_rec.PrNabCij); // (ovo - theMPC) = osnovica za PDV 
                     }

                     #endregion UMJETNINA set T_ppmvOsn(tj. nabCij)

                  }

                  #endregion Vpc1Policy is VP1 or VP2 from PreDef

               } // if(faktur_rec.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_CJENIK)

               else if(TtInfoOfThisRow.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij)
               {
                  theGrid.PutCell(ci.iT_cij, currRowIdx, artStat_rec.PrNabCij);

                  // ZPC additions START ______________________________
                  if(TtInfoOfThisRow.IsNivelacijaZPC)
                  {
                     theGrid.PutCell(ci.iT_kol     , currRowIdx, artStat_rec.StanjeKol);
                     theGrid.PutCell(ci.iT_doCijMal, currRowIdx, artStat_rec.MalopCij );
                     theGrid.PutCell(ci.iT_noCijMal, currRowIdx, artStat_rec.MalopCij );

                     ZXC.RISK_InitZPCvalues_InProgress = true;
                     ZXC.MalopCalcKind bckpKind = Fld_MalopCalcKind;
                     Fld_MalopCalcKind = ZXC.MalopCalcKind.By_MPC;
                     GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);
                     Fld_MalopCalcKind = bckpKind;
                     ZXC.RISK_InitZPCvalues_InProgress = false;

                  }
                  // ZPC additions END   ______________________________

                  // KIZ additions START ______________________________
                  if(TtInfoOfThisRow.IsKomisExtraProdCij) // iT_noCijMal treba dobiti prodajnu cijenu 
                  {
                     switch(faktur_rec.CjenikTT)
                     {
                        case Faktur.TT_CJ_VP1: // VELEPRODAJNI cjenik 1 

                           if(artStat_rec.PreDefVpc1.NotZero())
                           {
                              theGrid.PutCell(ci.iT_noCijMal, currRowIdx, artStat_rec.PreDefVpc1);
                           }
                           else /* user ne koristi CjenikDUC nego cijene drzi na samoj skladisnoj kartici (npr. zbog importa) */
                           {
                              decimal theVPC = artikl_rec.ImportCij;

                              #region I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                              //if(ZXC.RRD.Dsc_VpcMpcMarzaTheSame4VPC == true) // i u veleprodaji digni VpcMpcMarza pa daj rabat da bude opet isto 
                              //{
                              //   theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);

                              //   decimal rbtSt = ZXC.VvGet_rbtSt_100to90(theVPC, artikl_rec.ImportCij);

                              //   theGrid.PutCell(ci.iT_rbt1St, currRow, rbtSt);
                              //}

                              #endregion I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                              theGrid.PutCell(ci.iT_noCijMal, currRowIdx, /*artikl_rec.ImportCij*/ theVPC);
                           }
                           break;

                        case Faktur.TT_CJ_VP2: // VELEPRODAJNI cjenik 2 

                           Rtrans rtransVP2_rec = new Rtrans();
                           string artiklCD = TheG.GetStringCell(ci.iT_artiklCD, currRowIdx, false);
                           bool success = FakturDao.SetMeLastRtransForArtiklAndTtNum(TheDbConnection, rtransVP2_rec, faktur_rec.CjenikTT, faktur_rec.CjenTTnum, artiklCD, false);
                           if(success) theGrid.PutCell(ci.iT_noCijMal, currRowIdx, rtransVP2_rec.T_cij);
                           break;
                     } // switch 
                  } // if(TtInfoOfThisRow.IsKomisExtraProdCij)
                  // KIZ additions END   ______________________________

                  //// PIM additions START   ______________________________
                  //if(TtInfoOfThisRow.TheTT == Faktur.TT_PIM)
                  //{
                  //   theGrid.PutCell(ci.iT_cij    , currRow, artStat_rec.MalopCij);
                  //   theGrid.PutCell(ci.iT_cij_MSK, currRow, artStat_rec.MalopCij);

                  //   //ZXC.RISK_InitZPCvalues_InProgress = true;
                  //   //ZXC.MalopCalcKind bckpKind = Fld_MalopCalcKind;
                  //   //Fld_MalopCalcKind = ZXC.MalopCalcKind.By_MPC;
                  //   //GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRow);
                  //   //Fld_MalopCalcKind = bckpKind;
                  //   //ZXC.RISK_InitZPCvalues_InProgress = false;

                  //}
                  //// PIM additions END     ______________________________

                  // MVI additions START     ______________________________
                  if(TtInfoOfThisRow.TheTT == Faktur.TT_MVI) // MALOP 2 VELEP medjuskladisnica  
                  {
                     bool noGoodMPC = false;
                     if(artStat_rec != null)
                     {
                        theGrid.PutCell(ci.iT_cij_MSK, currRowIdx, artStat_rec.LastUlazMPC);
                        if(artStat_rec.LastUlazMPC.IsZero()) noGoodMPC = true;
                     }
                     else noGoodMPC = true;

                     if(noGoodMPC) ZXC.aim_emsg(MessageBoxIcon.Error, "Za artikl [{0}] nema MPC!", artikl_rec.ArtiklCD);
                  }
                  // MVI additions END     ______________________________

                  // VMI additions START     ______________________________
                  if(TtInfoOfThisRow.TheTT == Faktur.TT_VMI) // VELEP 2 MALOP medjuskladisnica  
                  {
                     ArtStat artStatTRM_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, Fld_SkladCD2, Fld_DokDate2.NotEmpty() ? Fld_DokDate2 : Fld_DokDate);
                     bool noGoodMPC = false;
                     if(artStatTRM_rec != null)
                     {
                        theGrid.PutCell(ci.iT_cij_MSK, currRowIdx, artStatTRM_rec.LastUlazMPC);
                        if(artStatTRM_rec.LastUlazMPC.IsZero()) noGoodMPC = true;
                     }
                     else noGoodMPC = true;

                     if(noGoodMPC) ZXC.aim_emsg(MessageBoxIcon.Error, "Za artikl [{0}] nema MPC!", artikl_rec.ArtiklCD);
                  }
                  // VMI additions END     ______________________________


               } // else if(TtInfoOfThisRow.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrNabCij)

               else if(faktur_rec.TtInfo.IsMalopFin_U) // KLK & URM 
               {
                  if(IsShowingConvertedMoney)
                  {
                     decimal theCij = artStat_rec.LastUlazMPC;
                     /* daj kune u devizu */ TheG.PutCell(ci.iT_cij_MSK, currRowIdx, ZXC.DivSafe(theCij, DevTecaj));
                  }
                  else
                  {
                     theGrid.PutCell(ci.iT_cij_MSK, currRowIdx, artStat_rec.LastUlazMPC); // propose do_cij_mal as no_cij_mal 
                  }

                  // 17.09.2018: 
                //if(artikl_rec.IsUMJETNINA && faktur_rec.TT == Faktur.TT_UPM) // UMJETNINA               na MALOP POVRAT DOBAVLJACU 
                  if(artikl_rec.IsPDVonRUC  && faktur_rec.TT == Faktur.TT_UPM) // UMJETNINA ili RabVozilo na MALOP POVRAT DOBAVLJACU 
                  {
                     theGrid.PutCell(ci.iT_cij, currRowIdx, artStat_rec.PrNabCij); // (ovo - theMPC) = osnovica za PDV 
                  }

               }

               else if(TtInfoOfThisRow.TheTT == Faktur.TT_TRM) // Zeleni, ulazni redak na dokumentu transformacije 
               {
                  ArtStat artStatTRM_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, Fld_SkladCD2, /*Fld_DokDate2.NotEmpty() ? Fld_DokDate2 :*/ Fld_DokDate);
                //ArtStat artStatTRM_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)GetDgvLineFields1(currRow, false, null));
                  bool noGoodMPC = false;
                  if(artStatTRM_rec != null)
                  {
                     theGrid.PutCell(ci.iT_cij_MSK, currRowIdx, artStatTRM_rec.LastUlazMPC);
                     if(artStatTRM_rec.LastUlazMPC.IsZero()) noGoodMPC = true;
                  }
                  else noGoodMPC = true;

                  if(noGoodMPC) ZXC.aim_emsg(MessageBoxIcon.Error, "Za artikl [{0}] nema MPC!", artikl_rec.ArtiklCD);
               }

            } // if(artStat_rec != null)
            else if(TtInfoOfThisRow.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_PrOfPrNabCij_SumeSastojaka)
            {
               // __?__ 
            }
            else // (artStat_rec == null)
            {
               if(faktur_rec.TT == Faktur.TT_IRM) // MALOP IRA 
               {
                  decimal theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);
                  decimal theMPC = ZXC.VvGet_125_on_100(theVPC              , faktur_rec.CommonPdvSt );

                  #region ČPP or ČNP

                  // 22.12.2013: still nothing. Is this first ulay for ČPP or ČNP? 
                  if(theMPC.IsZero() && artikl_rec.IsGlass) // Let's find bottle 'LastMalopCij' 
                  {
                     Artikl bottleArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artikl_rec.LinkArtCD);
                     if(bottleArtikl_rec != null)
                     {
                        ArtStat bottleArtstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, bottleArtikl_rec.ArtiklCD, faktur_rec.SkladCD, faktur_rec.DokDate);

                        if(bottleArtstat_rec != null) theMPC = bottleArtstat_rec.LastUlazMPCOP * artikl_rec.R_orgPak;
                     }
                  }

                  #endregion ČPP or ČNP

                  theGrid.PutCell(ci.iT_cij, currRowIdx, theMPC);
               }
               else if(faktur_rec.TT == Faktur.TT_PNM || faktur_rec.TT == Faktur.TT_INM) 
               {
                  //decimal theMPC = XtransDao.GetExternCijenaInKune(TheDbConnection, artikl_rec.ArtiklCD, Fld_DokDate); 
                  //theGrid.PutCell(ci.iT_cij, currRow, theMPC);

                  // 09.10.2013: 
                  decimal theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);
                  decimal theMPC = ZXC.VvGet_125_on_100(theVPC              , faktur_rec.CommonPdvSt ); 

                  theGrid.PutCell(ci.iT_cij, currRowIdx, theMPC);
               }
               else
               {
                  decimal theVPC = artikl_rec.ImportCij;

                  #region I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                  if(ZXC.RRD.Dsc_VpcMpcMarzaTheSame4VPC == true) // i u veleprodaji digni VpcMpcMarza pa daj rabat da bude opet isto 
                  {
                     theVPC = ZXC.VvGet_125_on_100(artikl_rec.ImportCij, ZXC.RRD.Dsc_VpcMpcMarza);

                     decimal rbtSt = ZXC.VvGet_rbtSt_100to90(theVPC, artikl_rec.ImportCij);

                     theGrid.PutCell(ci.iT_rbt1St, currRowIdx, rbtSt);
                  }

                  #endregion I u veleprodaji odglumi mpc politiku: digni za VpcMpcMarza pa rabatom spusti nazad

                  if(faktur_rec.TtInfo.ProposeCijenaKind == ZXC.TtProposeCijenaKindEnum.Propose_CJENIK) // ovaj if dodan tek 19.01.2012 
                  {
                     theGrid.PutCell(ci.iT_cij, currRowIdx, /*artikl_rec.ImportCij*/ theVPC);
                  }
               }

               if(faktur_rec.TT == Faktur.TT_IRM && ((this as FakturExtDUC).Fld_NacPlacRbt).NotZero())
               {
                  theGrid.PutCell(ci.iT_rbt1St, currRowIdx, (this as FakturExtDUC).Fld_NacPlacRbt);
               }
            }

            #region Check For Minus

            //TtInfo ttInfoOfThisRow = GetTtInfoOfThisRow(currRow);

            if(TtInfoOfThisRow.IsFinKol_I)
            {
               if((artStat_rec == null && artikl_rec .IsMinusNotOK) ||
                  (artStat_rec != null && artStat_rec.StanjeKolFree.IsZeroOrNegative() && artikl_rec.IsMinusNotOK))
               {
                  if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.DENY_VEL_ALLOW_MAL && faktur_rec.TtInfo.IsMalopTT) goto skiMessageLabel; // NE javljaj ako je malop i DENY_VEL_ALLOW_MAL ('Zagria pattern')
                  if(ZXC.CURR_prjkt_rec.MinusPolicy == ZXC.MinusPolicy.ALOW_ALL_NO_MSG                                  ) goto skiMessageLabel; // NE javljaj ako je malop i DENY_VEL_ALLOW_MAL ('Zagria pattern')
                  if(artikl_rec.IsGlass                                                                                 ) goto skiMessageLabel; // NE javljaj ako je čaša u ugostiteljstvu 

                  ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Zadali ste artikl [" + artikl_rec.ArtiklCD + "] kojega nema na stanju skladišta '" + Fld_SkladCD + "'.");

                  skiMessageLabel: ;

               }
            }

            #endregion Check For Minus

         } // if(faktur_rec.TtInfo.IsArtStatInfoNeededTT) // Ili zbog cjenika ili zbog max Izlaz Kol, ... 

         #endregion PreDefs, Check For Minus

         #region Spritz Artikl Fields in faktur_rec.opis

         if(artikl_rec.Grupa1CD == ZXC.MotVoziloGrCD &&
            (Fld_TT == Faktur.TT_IRM || Fld_TT == Faktur.TT_PNM || Fld_TT == Faktur.TT_IRA)
            ) // TODO: !!! kako iz grupe raspoznavati motocikl 
         {
            Fld_Opis += "BROJ ŠASIJE:\t\t\r\n";
            Fld_Opis += "BOJA:\t\t\t\t\r\n";
            Fld_Opis += "OBUJAM:\t\t\t" + artikl_rec.Zapremina.ToStringVv_NoDecimalNoGroup() + " ccm\r\n";
            Fld_Opis += "SNAGA:\t\t\t"  + artikl_rec.Snaga.ToStringVv_NoDecimalNoGroup() + " kW\r\n";
            //Fld_Opis += "EURO NORMA:\t\tEURO "    + artikl_rec.Garancija.ToString                   () + "\r\n"    ; // FUSE 
            Fld_Opis += "God. proizvodnje:\t\r\n\r\n";
            Fld_Opis += "DRŽAVA PROIZVODNJE: " + (ZXC.IsTEMBO ? "" : "ITALIJA");
         }

         #endregion Spritz Artikl Fields in faktur_rec.opis

         #region UMJETNINA set T_pdvColTip = PdvKolTipEnum.UMJETN

         // 17.09.2018: 
       //if(artikl_rec.IsUMJETNINA) 
         if(artikl_rec.IsPDVonRUC )
         {
            theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.UMJETN));
         }

         #endregion UMJETNINA set T_pdvColTip = PdvKolTipEnum.UMJETN

         #region IsGlassOnIRM set T_pdvColTip = PdvKolTipEnum.GlassOnIRM

         if(artikl_rec.IsGlass && faktur_rec.TtInfo.IsMalopFin_I)
         {
            theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.GlassOnIRM));
         }
         //// 13.01.2014: 
         //else if(artikl_rec.TS == "RPP" && artikl_rec.Grupa1CD == "PIĆ" && artikl_rec.Grupa2CD == "ŽAP" && faktur_rec.TtInfo.IsMalopFin_I)
         //{
         //   Artikl glassArtiklRec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.LinkArtCD == artikl_rec.ArtiklCD);

         //   if(glassArtiklRec != null)
         //   {
         //      ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nZadali ste artikl koji se inace prodaje na čaše!\n\n{0}", glassArtiklRec);
         //   }
         //}

         #endregion IsGlassOnIRM set T_pdvColTip = PdvKolTipEnum.GlassOnIRM

         // 14.03.2013: 
       //if(this is ProizvodnjaDUC == false && this is PIZpDUC == false && this is PIMDUC == false)
         // 02.01.2014: bio bug 
         if(this is ProizvodnjaDUC == false && this is PIZpDUC == false && this is PIMDUC == false && this is TransformDUC == false && TtInfoOfThisRow.IsNivelacijaZPC == false)
         {
            theGrid.PutCell(ci.iT_kol, currRowIdx, 1.00M); 
            GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);
         }

         #region Get PMV Xtrans data for this Artikl

         if(artikl_rec.IsPPMV && Fld_DokDate >= ZXC.PdvEU_EraDate /* 01.07.2013: aka. PpmvEraDate */)
         {

            if(ZXC.IsTEMBO)
            {
               decimal osnovica, stopa1i2;
               osnovica = Artikl.GetMotoPpmv2017(artikl_rec.Zapremina, artikl_rec.EuroNorma);
               stopa1i2 = 100M;
               theGrid.PutCell(ci.iT_ppmvOsn, currRowIdx, osnovica);
               theGrid.PutCell(ci.iT_ppmvSt1i2, currRowIdx, stopa1i2);

               if(this is IRMDUC_2 || this is IRADUC_2)
               {
                //IRMDUC_2 theDUCex = this as IRMDUC_2;
                  FakturExtDUC theDUCex = this as FakturExtDUC;

                  theDUCex.Fld_PrjArtCD = artikl_rec.ArtiklCD;
                  theDUCex.Fld_PrjArtName = artikl_rec.ArtiklName;
               }
            }
            else // not tembo
            {
               Xtrans xtrans_rec = XtransDao.Get_PPMV_Xtrans(TheDbConnection, artikl_rec.ArtiklCD, Fld_DokDate);

               if(xtrans_rec != null)
               {
                  decimal osnovica = xtrans_rec.T_moneyA;
                  bool isDizel = false; // TODO 
                  bool isAuto = false; // TODO 

                  decimal stopaPO = Artikl.GetPpmvStopaFor_PorOsn(osnovica);

                  decimal stopaCO2 = isDizel ?
                                     Artikl.GetPpmvStopaFor_CO2_Dizel(xtrans_rec.T_moneyB, artikl_rec.EuroNorma) :
                                     Artikl.GetPpmvStopaFor_CO2_Benz(xtrans_rec.T_moneyB);

                  decimal stopaCM3 = Artikl.GetPpmvStopaFor_CM3(xtrans_rec.T_moneyC);
                  decimal stopaEN = Artikl.GetPpmvStopaFor_EuroNorma((ZXC.EuroNormaEnum)xtrans_rec.T_intA);

                  decimal stopa1i2 = isAuto ? stopaPO + stopaCO2 : stopaCM3 + stopaEN;

                  // 10.01.2017: overriding old-Bef2017 rules with 2017 rules ___ START ___ 
                  if(ZXC.projectYearFirstDay.Year >= 2017)
                  {
                     decimal kontrolnaOsnovica = Artikl.GetMotoPpmv2017(xtrans_rec.T_moneyC, (ZXC.EuroNormaEnum)xtrans_rec.T_intA);
                     osnovica = Artikl.GetMotoPpmv2017(artikl_rec.Zapremina, artikl_rec.EuroNorma);
                     stopa1i2 = 100M;

                     if(kontrolnaOsnovica != osnovica) ZXC.aim_emsg(MessageBoxIcon.Warning, "Nekonzistentni podaci EuroNorma i/ili Zapremina\n\nArtikl sifrar vs Xtrans ppmv cjenik!");
                  }
                  // 10.01.2017: overriding old-Bef2017 rules with 2017 rules ___  END  ___ 

                  theGrid.PutCell(ci.iT_ppmvOsn, currRowIdx, osnovica);
                  theGrid.PutCell(ci.iT_ppmvSt1i2, currRowIdx, stopa1i2);

                  if(this is IRMDUC_2)
                  {
                     IRMDUC_2 theDUCex = this as IRMDUC_2;

                     theDUCex.Fld_PrjArtCD = artikl_rec.ArtiklCD;
                     theDUCex.Fld_PrjArtName = artikl_rec.ArtiklName;
                  }

               } // if(xtrans_rec != null) 
            } // not tembo

            GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);

         } // if(artikl_rec.IsPPMV) 
         else
         {
            if(this is IRMDUC_2 || this is IRADUC_2)
            {
               FakturExtDUC theDUCex = this as FakturExtDUC;

               theDUCex.Fld_PrjArtCD   = 
               theDUCex.Fld_PrjArtName = "";
            }
         }

         #endregion PPMV - Get Xtrans data for this Rtrans

         #region JAM_IsOnEndEditJump2NextRow) // BarCode column

         if(vvtb_editingControl.JAM_IsOnEndEditJump2NextRow) // BarCode column 
         {
            //theGrid.CurrentCell = theGrid["Q_barCode1", theGrid.CurrentRow.Index + 1];
            //theGrid.CurrentCell.Selected = true;
            //theGrid.Select();
            ////theGrid["Q_barCode1", theGrid.CurrentRow.Index + 1].Selected = true;
            //theGrid.BeginEdit(false);

            //for(int currColIdx = theGrid.CurrentCell.ColumnIndex + 1; currColIdx < theGrid.Columns.Count; ++currColIdx)
            //{
            //   if(theGrid.Columns[currColIdx].Tag == null) continue;

            //   if(theGrid[currColIdx, theGrid.CurrentCell.RowIndex].Visible == true && ((VvTextBox)theGrid.Columns[currColIdx].Tag).JAM_ReadOnly == false) SendKeys.Send("{TAB}");
            //}
            ZXC.SendMultipleTabKey(7);
         }

         #endregion JAM_IsOnEndEditJump2NextRow) // BarCode column

         #region TRI on TransfromDUC

         // 1. Put 'X' if produkt - zeleni red 

         // 2. Vidi kamo s koeficijentom 

         #endregion TRI on TransfromDUC

         #region SvDUH Additions

         if(ZXC.IsSvDUH)
         {
            decimal theORG = 0M;

            if(this is UGODUC)
            {
               theGrid.PutCell(ci.iT_artiklLongOpis, currRowIdx, artikl_rec.LongOpis);
            }
            if(this is IZD_SVD_DUC || this is ZAH_SVD_DUC)
            {
               theGrid.PutCell(ci.iT_kol, currRowIdx, 0.00M); // overrajdanje prethodne jedinice 

               ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)GetDgvLineFields1(currRowIdx, false, null));

               if(artStat_rec != null)
               {
                  theGrid.PutCell(ci.iT_cij, currRowIdx, artStat_rec.PrNabCij);
               }
            }
            if(this.HasOrgBopCop)
            {
               theORG = Get_SVD_theORG(TheDbConnection, artikl_rec.ArtiklCD, false);

               theGrid.PutCell(ci.iT_doCijMal, currRowIdx, theORG);

               // 30.01.2019: 
                    if(artikl_rec.IsSvdArtGR_Ljek_) (this as FakturExtDUC).Fld_PdvZPkind = ZXC.PdvZPkindEnum.SVD_LJEK;
               else if(artikl_rec.IsSvdArtGR_Potr_) (this as FakturExtDUC).Fld_PdvZPkind = ZXC.PdvZPkindEnum.SVD_POTR;

                 // šudajstejoršudajgou? 
                 //if(lastORG != artORG) ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje: zadnje korišteni ORG je različit od ORG-a u šifrarniku.\n\n");
            }

            // 23.09.2019: 
            if(this is NRD_SVD_DUC)
            {
               Rtrans NRDrtrans_rec = new Rtrans();
               Rtrans UGOrtrans_rec = new Rtrans();

               NRD_SVD_DUC theDUCex = this as NRD_SVD_DUC;

               NRDrtrans_rec.T_artiklCD  = artikl_rec.ArtiklCD  ;
               NRDrtrans_rec.T_kupdobCD  = theDUCex.Fld_KupdobCd;
               NRDrtrans_rec.T_skladDate =          Fld_DokDate ;

               bool UGOrtransFound = FakturDao.SetMeLastUGOrtransForURArtrans(TheDbConnection, UGOrtrans_rec, NRDrtrans_rec);

               if(UGOrtransFound)
               {
                  theGrid.PutCell(ci.iT_cij  , currRowIdx, UGOrtrans_rec.T_cij         );
                  theGrid.PutCell(ci.iT_cop  , currRowIdx, UGOrtrans_rec.T_cij * theORG);
                  theGrid.PutCell(ci.iT_pdvSt, currRowIdx, UGOrtrans_rec.T_pdvSt       );
               }
            }

         } // if(ZXC.IsSvDUH) 

         #endregion SvDUH Additions

         #region CKP Additions (FRAG - Cjenik kupca)

       //if(faktur_rec.TtInfo.IsIzlazniPdvTT                                   && this is FakturExtDUC) // TODO: da li da uopce idemo u potragu 
         if((faktur_rec.TtInfo.IsIzlazniPdvTT || faktur_rec.TtInfo.IsPonudaTT) && this is FakturExtDUC) // TODO: da li da uopce idemo u potragu 
         {
            FakturExtDUC theDUCex = this as FakturExtDUC;

            List<Rtrans> Cjenik_RtransList_For_IRA_Artikl = RtransDao.Get_RtransList_For_TT_Artikl_Kupdob(TheDbConnection, Faktur.TT_CKP, artikl_rec.ArtiklCD, theDUCex.Fld_KupdobCd);

            if(Cjenik_RtransList_For_IRA_Artikl.NotEmpty())
            {
               // lista je sortirana po 'Rtrans.artiklOrderBy_ASC' - Robna Kartica Sort 
               Rtrans  lastRtransBy_cjenikKupca = Cjenik_RtransList_For_IRA_Artikl.Last();
               decimal theCij                   = lastRtransBy_cjenikKupca.T_cij      ;

               if(IsShowingConvertedMoney)
               {
                  decimal theTecaj = ZXC.DevTecDao.GetHnbTecaj(theDUCex.Fld_DevNameAsEnum, lastRtransBy_cjenikKupca.T_skladDate);

                  theCij = ZXC.DivSafe(theCij, theTecaj);
               }

             // 19.04.2021. kad je u cjeniku devizna cjena a IRA je kunska
             //theGrid.PutCell(ci.iT_cij, currRow, theCij);
               theGrid.PutCell(ci.iT_cij, currRowIdx, theCij.Ron2());
            }
         }

         #endregion CKP Additions (FRAG - Cjenik kupca)

         #region HasRtrans_SkladCD_Exposed

         if(HasRtrans_SkladCD_Exposed)
         {
            theGrid.PutCell(ci.iT_skladCD, currRowIdx, Fld_SkladCD);
         }

         #endregion HasRtrans_SkladCD_Exposed
         
         #region Tetragram pdvKolTip - oslobođeno od PDVa
       //27.06.2024.
         if(ZXC.IsTETRAGRAM_ANY && artikl_rec.PdvKat == "Ne" && (this is IRA_MPC_DUC || this is PON_MPC_DUC || this is OPN_MPC_DUC || this is IZD_MPC_DUC))
         {
            if(faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.EU)
            { 
               bool isFizicakOsoba    = false;
               bool isPartnerskaFirma = false;
               
             //Kupdob kupdob_rec = Get_Kupdob_FromVvUcSifrar((this as IRA_MPC_DUC).Fld_KupdobCd);
               Kupdob kupdob_rec = Get_Kupdob_FromVvUcSifrar((this as FakturExtDUC).Fld_KupdobCd);
               if(kupdob_rec != null && kupdob_rec.IsZzz)  isFizicakOsoba    = true;
               if(kupdob_rec != null)                      isPartnerskaFirma = kupdob_rec.Ticker.ToUpper().StartsWith("TETR");

                    if(isFizicakOsoba   ) theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.KOL16));
               else if(isPartnerskaFirma) theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, "");
               else                       theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.KOL09));
            }
            else if(faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.HR   ) theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.KOL14));
            else if(faktur_rec.PdvGEOkind == ZXC.PdvGEOkindEnum.WORLD) theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.KOL15));
         }
         //13.02.2026.
         if(ZXC.IsTETRAGRAM_ANY && artikl_rec.PdvKat == "PS" && (this is IRA_MPC_DUC || this is PON_MPC_DUC || this is OPN_MPC_DUC || this is IZD_MPC_DUC))
         {
            theGrid.PutCell(ci.iT_pdvKolTip, currRowIdx, GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.PROLAZ));
         }
         #endregion Tetragram pdvKolTip - oslobođeno od PDVa

            #region KPD 2026

            if(ci.iT_KPD.IsPositive())
         {
            theGrid.PutCell(ci.iT_KPD, currRowIdx, artikl_rec.KPD);
         }

         #endregion KPD 2026

      } // if(artikl_rec != null) 

      else if(this.sifrarSorterType == VvSQL.SorterType.Name && vvtb_editingControl.Text != "") // ako smo dosli iz naziva, a artikl_rec je null, to je onda 'qwe' pattern (ne postoji kao sifrar) 
      {
         theGrid.PutCell(ci.iT_artiklName, currRowIdx, vvtb_editingControl.Text);

         if(ZXC.CURR_prjkt_rec.IS_IN_PDV && isOutsideEU_DevizniDokument == false) theGrid.PutCell(ci.iT_pdvSt, currRowIdx, faktur_rec.CommonPdvSt);
      }

      #endregion if(artikl_rec != null)

      END_Action_LABEL:

      #region Actions independent of Artikl or Artstat (18.06.2020)

      if(ZXC.RRD.Dsc_IsRbtFromPartner)
      {
         if(this is FakturExtDUC && 
            (faktur_rec.TtInfo.IsPrihodTT || faktur_rec.TtInfo.IsPonudaTT ||
             this is UGNorAUN_PTG_DUC     || this is DIZ_PTG_DUC)
            )
         {
            Kupdob kupdob_rec = Get_Kupdob_FromVvUcSifrar((this as FakturExtDUC).Fld_KupdobCd);

            if(kupdob_rec != null && kupdob_rec.StRbt1.NotZero())
            {
               theGrid.PutCell(ci.iT_rbt1St, currRowIdx, kupdob_rec.StRbt1);
            }
         }
      }

      #endregion Actions independent of Artikl or Artstat (18.06.2020)

      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   public static decimal Get_SVD_theORG(XSqlConnection conn, string artiklCD, bool shouldErrorReportToFile)
   {
      decimal lastORG = 0M;
      decimal artORG  = 0M;
      decimal theORG  = 0M;

      Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      if(artikl_rec != null)
      {
         lastORG = RtransDao.GetLastUsed_URA_ORG(conn, artiklCD, artikl_rec.ArtiklCD2, shouldErrorReportToFile);

         artORG = ZXC.ValOrZero_Decimal(artikl_rec.OrgPak, 0);
      }

      theORG = lastORG.NotZero() ? lastORG :
               artORG .NotZero() ? artORG  :
               1;

      return theORG;
   }

   private decimal Get_TH_IRM_Rabat(string skladCD, DateTime dokDate, Artikl artikl_rec)
   {
      bool isVrecice    = (artikl_rec.Grupa3CD == Artikl.ProAndNabGrCD)                  ; // na vrecice ProAndNabGrCD nema rabata 
      bool isODEandAkat = (artikl_rec.Grupa1CD == "Akat" && artikl_rec.Grupa2CD == "ODE"); // Odjeca Exclusive A kategorije (ono sto od 03.2021. hoce promjenjeni tretman cijena) 

      // !!! 
      // !!! 
      // !!! 
      // 15.03.2021: 
    //if(isVrecice                ) return 0.00M; // ne zelimo automatski (preko rule) rabat za ovakve artikle 
      // 11.06.2021: opet vracamo na staro, kak je bilo prije 15.03.2021: 
    //if(isVrecice || isODEandAkat) return 0.00M; // ne zelimo automatski (preko rule) rabat za ovakve artikle 
      if(isVrecice                ) return 0.00M; // ne zelimo automatski (preko rule) rabat za ovakve artikle 
      // !!! 
      // !!! 
      // !!! 





      // Ako smo do tu dosli, znaci da je ovo sve true: TH.IRM.Artikl.ProdRobaGrCD ... 

      TH_PriceRuleForCycleMoment theTHPR = TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(skladCD, dokDate);

      // 07.03.2018: implementiramo KPN logiku (TH Kupon) 
      decimal eventualKPN_RbtSt = Fld_Decimal02;

      // 06.12.2019: implementiramo HH logiku (a mogli smo iprije) 
      decimal eventualHH_RbtSt = (Fld_IsHappyHour ? theTHPR.HHpercent : 0.00M);

      //return theTHPR.RbtSt1                                       ;
      //return theTHPR.RbtSt1 + eventualKPN_RbtSt                   ;

      // 24.06.2022: 
      if(eventualHH_RbtSt.NotZero()) return eventualHH_RbtSt;

      // 24.06.2022: 
    //return theTHPR.RbtSt1 + eventualKPN_RbtSt   + eventualHH_RbtSt  ;
      return theTHPR.RbtSt1 + eventualKPN_RbtSt /*+ eventualHH_RbtSt*/;
   }

   private bool CheckIsArtiklKindAdequate(int currRow)
   {
      if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return true;

      bool isAdequate = true;

      string artiklCD   = TheG.GetStringCell(ci.iT_artiklCD  , currRow, false);
      string artiklName = TheG.GetStringCell(ci.iT_artiklName, currRow, false);

      if((artiklCD + artiklName).IsEmpty()) return isAdequate;

      Artikl artikl_rec;
      if(artiklCD.NotEmpty()) artikl_rec = Get_Artikl_FromVvUcSifrar      (artiklCD  );
      else                    artikl_rec = Get_Artikl_FromVvUcSifrarByName(artiklName);

      if(artikl_rec == null) return isAdequate;

      TtInfo ttInfo = GetTtInfoOfThisRow(currRow);

      string thisKind = artikl_rec.Grupa3CD ;
      string nabKind  = Artikl.NabRobaGrCD  ; // "NBKG"  
      string prodKind = Artikl.ProdRobaGrCD ; // "PRKOM" 
      string nbprKind = Artikl.ProAndNabGrCD; // "NBiPR" 

      // Ovo ti je ujedno i TEXTHO Faktur.TT_ LISTA 
      switch(ttInfo.TheTT)
      {
         case Faktur.TT_MSI: isAdequate =(thisKind == nabKind || thisKind == nbprKind );  break;
         case Faktur.TT_TRI: isAdequate =(thisKind == nabKind                         );  break;
         case Faktur.TT_TRM: isAdequate =(thisKind == prodKind                        );  break;
       //case Faktur.TT_IRM: isAdequate =(thisKind == prodKind                        );  break;
         case Faktur.TT_IRM: isAdequate =(thisKind == prodKind || thisKind == nbprKind);  break;

       //case Faktur.TT_MVI:                              isAdequate =(thisKind == prodKind                        );  break; 
       //case Faktur.TT_MVI:                              isAdequate =(thisKind == prodKind || thisKind == nbprKind);  break; // 10.09.2015. da se mogu vratiti ogrlice
         case Faktur.TT_MVI: if(this is MedjuSkladMVI2DUC) isAdequate =(thisKind == nbprKind                        );// 14.09.2015. komadna roba ide na sklPOVRATA
                             else                          isAdequate =(thisKind == prodKind                        );// 14.09.2015. ogrlice i vrecice idu na GLSK
                             break; 

         // 27.04.2018: uvodimo novi VMI2 za povrat povrata - anti shop return (zatvara se Vukovar i jos jedna) 
       //case Faktur.TT_VMI:                               isAdequate =(thisKind == nbprKind                        );  break; // ! 
         case Faktur.TT_VMI: if(this is MedjuSkladVMI2DUC) isAdequate =(thisKind == prodKind                        ); 
                             else                          isAdequate =(thisKind == nbprKind                        );  break; // ! 

         case Faktur.TT_URA: isAdequate =(thisKind == nbprKind                        );  break; // ! 
         case Faktur.TT_INV: isAdequate =(thisKind == nabKind  || thisKind == nbprKind);  break;
         case Faktur.TT_PSM: isAdequate =(thisKind == prodKind || thisKind == nbprKind);  break;
         case Faktur.TT_INM: isAdequate =(thisKind == prodKind || thisKind == nbprKind);  break;
       //case Faktur.TT_ZPC: isAdequate =(thisKind == prodKind                        );  break; 
         case Faktur.TT_ZPC: isAdequate =(thisKind == prodKind || thisKind == nbprKind);  break;
         case Faktur.TT_PIZ: isAdequate =(thisKind == prodKind                        );  break;
         case Faktur.TT_PUL: isAdequate =(thisKind == nabKind                         );  break;

         // PST, PRI, IZD su poseban slucaj jer se jevljaju u BO - NabKind veleprodaja kao i u 12BPS - ProdKind veleprodaja 
         case Faktur.TT_PST: case Faktur.TT_PRI: case Faktur.TT_IZD: 
            if(faktur_rec.Skl1kind == ZXC.SkySklKind.CentPVSK) isAdequate = true; // tu se mogu pojaviti i vreće i komadi 
            if(faktur_rec.Skl1kind != ZXC.SkySklKind.CentPVSK) isAdequate =(thisKind == nabKind  || thisKind == nbprKind);  
            break;
      }

      if(artikl_rec.IsFKZ) isAdequate = true;

      if(isAdequate == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl [{0}]\n\ngrupa [{1}]\n\nJe NEPRIMJEREN za transakciju\n\n[{2}]",
            artikl_rec.ToString(), artikl_rec.Grupa3CD + " - " + artikl_rec.Grupa3Name, Fld_TT + " - " + ZXC.luiListaFakturType.GetNameForThisCd(Fld_TT));

         TheG.ClearRowContent(currRow); // !!! 
      }

      return isAdequate;
   }

   /*private*/ public TtInfo GetTtInfoOfThisRow(int currRow)
   {
      if(faktur_rec.TtInfo.HasSplitTT == false) return faktur_rec.TtInfo;
      else
      {
         bool isUlazTT = VvCheckBox.GetBool4String(TheG.GetStringCell(ci.iT_isProductLine, currRow, false));

         if(isUlazTT) return ZXC.TtInfo(faktur_rec.TtInfo.SplitTT);
         else         return faktur_rec.TtInfo;
      }
   }

   #region Update_Faktur_4_ProjektCD

   public static ZXC.VvSubModulEnum GetVvSubModulEnum_ForTT(string theTT)
   {
      // 26.12.2013: 
      return ZXC.TtInfo(theTT).DefaultSubModulEnum;

      //switch(theTT)
      //{
      //   case Faktur.TT_UFA: return ZXC.VvSubModulEnum.R_UFA;
      //   case Faktur.TT_UPA: return ZXC.VvSubModulEnum.R_UPA;
      //   case Faktur.TT_UFM: return ZXC.VvSubModulEnum.R_UFM;
      //   case Faktur.TT_URA: return ZXC.VvSubModulEnum.R_URA;
      //   case Faktur.TT_URM: return ZXC.VvSubModulEnum.R_URM;
      //   case Faktur.TT_UOD: return ZXC.VvSubModulEnum.R_UOD;
      //   case Faktur.TT_UPV: return ZXC.VvSubModulEnum.R_UPV;
      //   case Faktur.TT_UPM: return ZXC.VvSubModulEnum.R_UPM;

      //   case Faktur.TT_IFA: return ZXC.VvSubModulEnum.R_IFA;
      //   case Faktur.TT_IRA: return ZXC.VvSubModulEnum.R_IRA;
      //   case Faktur.TT_IOD: return ZXC.VvSubModulEnum.R_IOD;
      //   case Faktur.TT_IPV: return ZXC.VvSubModulEnum.R_IPV;

      //   case Faktur.TT_IRM: return ZXC.VvSubModulEnum.R_IRM;

      //   case Faktur.TT_UPL: return ZXC.VvSubModulEnum.R_UPL;
      //   case Faktur.TT_ISP: return ZXC.VvSubModulEnum.R_ISP;

      //   case Faktur.TT_RNP: return ZXC.VvSubModulEnum.R_RNP;
      //   case Faktur.TT_RNS: return ZXC.VvSubModulEnum.R_RNS;
      //   case Faktur.TT_PRJ: return ZXC.VvSubModulEnum.R_PRJ;

      //   case Faktur.TT_PPR: return ZXC.VvSubModulEnum.R_PPR;
      //   case Faktur.TT_POV: return ZXC.VvSubModulEnum.R_POV;

      //   default: ZXC.aim_emsg(MessageBoxIcon.Error, "GetVvSubModulEnum_ForTT [{0}] sims tubi andifajnd :-(", theTT); return ZXC.VvSubModulEnum.UNDEF;
      //}
   }

   /// <summary>
   /// 'FindVvDataRecord' procedura. Inicirana:
   /// 1. Context menu (Mouse right click)
   /// 2. Mouse click (Ctrl ili Alt click)
   /// 3. Keyboard initiated (Ctrl/Alt + F/Space)
   /// </summary>
   /// <param name="startValue"></param>
   /// <returns></returns>
   public static string Update_Faktur_4_ProjektCD(object startValue)
   {
      Faktur Faktur_rec = new Faktur();
      FakturListUC FakturListUC;
      XSqlConnection dbConnection = ZXC.TheVvForm.TheDbConnection;

      if(ZXC.LastUsedProjektTT_IsDiscovered == false)
      {
         ZXC.LastUsedProjektTT = FakturDao.Discover_LastUsedProjektTT(dbConnection);

         ZXC.LastUsedProjektTT_IsDiscovered = true;
      }

      VvSubModul theVvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(GetVvSubModulEnum_ForTT(ZXC.LastUsedProjektTT));

      VvFindDialog dlg = CreateFind_Faktur_Dialog(theVvSubModul);

      FakturListUC = (FakturListUC)(dlg.TheRecListUC);

      bool backupOf_Supress_ImaLiIjedan_StartField_Neprazan_Action = FakturListUC.Supress_ImaLiIjedan_StartField_Neprazan_Action;
      FakturListUC.Supress_ImaLiIjedan_StartField_Neprazan_Action = false;

      FakturListUC.Fld_FromTtNum = (uint)startValue;
      FakturListUC.rbt_Descending.Checked = true;
      FakturListUC.asc_or_desc = VvSQL.OrderDirectEnum.DESC;

      // 04.05.2018: 
      if(ZXC.IsSvDUH && ZXC.TheVvForm.TheVvUC is FakturExtDUC) // trazimo SVD - UGO - ugovor 
      {
         FakturListUC.Fld_FiltPartnerCD   = (ZXC.TheVvForm.TheVvUC as FakturExtDUC).Fld_KupdobCd  ;
         FakturListUC.Fld_FiltPartnerTick = (ZXC.TheVvForm.TheVvUC as FakturExtDUC).Fld_KupdobTk  ;
         FakturListUC.Fld_FiltPartnerName = (ZXC.TheVvForm.TheVvUC as FakturExtDUC).Fld_KupdobName;
      }

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.FakturDao.SetMe_Record_byRecID(dbConnection, Faktur_rec, (uint)dlg.SelectedRecID, false)) return null;
      }
      else
      {
         Faktur_rec = null;
      }

      //if(dlg.SelectionIsNewlyAddedRecord == true) ZXC.ShouldForceSifrarRefreshing = true;

      dlg.Dispose();

      FakturListUC.Supress_ImaLiIjedan_StartField_Neprazan_Action = backupOf_Supress_ImaLiIjedan_StartField_Neprazan_Action;

      if(Faktur_rec != null) return Faktur_rec.TT_And_TtNum;
      else return null;
   }

   #endregion Update_Faktur_4_ProjektCD

   #region Are we in devizni dokument

   private bool ArerWeIn_OutsideEU_DevizniDokument()
   {
      FakturExtDUC theFaktExtDUC = this as FakturExtDUC;

      if(theFaktExtDUC == null) return false;

      bool isEUpdv = theFaktExtDUC.Fld_PdvGEOkind == ZXC.PdvGEOkindEnum.EU;

      if(isEUpdv) return false;

      return ArerWeIn_DevizniDokument();
   }

   private bool ArerWeIn_DevizniDokument()
   {
      FakturExtDUC theFaktExtDUC = this as FakturExtDUC;

      if(theFaktExtDUC == null) return false;

      bool isDevizniDokument = false;

      if(theFaktExtDUC != null &&
         theFaktExtDUC.Fld_DevNameAsEnum != ZXC.ValutaNameEnum.EMPTY &&
         theFaktExtDUC.Fld_DevNameAsEnum != /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum)
      {
         isDevizniDokument = true;
      }

      return isDevizniDokument;
   }

   #endregion Are we in devizni dokument

   private void OnExit_UGO_SetSVDartiklLongOpis(object sender, EventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar)                           return;
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl                    == null             ) return;
      if(vvtb_editingControl.Text               == this.originalText) return;
      if(vvtb_editingControl.EditedHasChanges() == false            ) return;
      if(vvtb_editingControl.Text.              IsEmpty()           ) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      #endregion Init stuff

      string artiklCD = TheG.GetStringCell(ci.iT_artiklCD, currRow, false);

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      if(artikl_rec == null) return;

      string ducArtiklLongOpis = vvtb_editingControl.Text;
      string oldArtiklLongOpis = artikl_rec.LongOpis     ;

      if(ducArtiklLongOpis == oldArtiklLongOpis) return;

      DialogResult result = MessageBox.Show("Da li želite da zapamtim ugovorni opis artikla i dobavljača?\n\n<" + artikl_rec.ArtiklCD + "> <" + artikl_rec.ArtiklName + ">",
         "Potvrdite Artikl Long Opis!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      artikl_rec.BeginEdit();

      artikl_rec.LongOpis = ducArtiklLongOpis;

      artikl_rec.DobavCD  = (this as FakturExtDUC).Fld_KupdobCd;

      artikl_rec.VvDao.RWTREC(TheDbConnection, artikl_rec, false, true);

      artikl_rec.EndEdit();

   }

   #endregion UpdateArtikl

   #region Update_SERNO PPUK

   protected void OnExitT_Update_SERNO(object sender, EventArgs e)
   {
      if(faktur_rec.TtInfo.IsFinKol_I == false) return; // well, traziti cemo rtrano_rec na osnovu t_serno-a SAMO na izlazima (PIZ, IRA, ...) 

      #region Init stuff

      if(isPopulatingSifrar)                           return;
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text.IsEmpty())            return;
      if(vvtb_editingControl.Text == this.originalText) return;

      VvDataGridView theGrid = ((VvDataGridView)vvtb_editingControl.EditingControlDataGridView);

      this.originalText = vvtb_editingControl.Text;
      
      //Artikl artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

      int currRow = vvtb_editingControl.EditingControlRowIndex;

      //bool isDevizniDokument = ArerWeInDevizniDokument();

      #endregion Init stuff

      List<Rtrano> rtranoList = RtranoDao.GetRtranoList_For_SERNO(TheDbConnection, vvtb_editingControl.Text);

      bool rtranoNOTfound   = rtranoList.Count.IsZero();
      bool rtranoMULTIfound = rtranoList.Count > 1     ;

      theGrid.ClearRowContent(currRow);

      //PutDgvTransSumFields();

      //PutTransSumToDocumentSumFields();

      // ________ Here we go! ________ 

      if(rtranoNOTfound)
      {
         // votafakshouldajdunau? 
         return;
      }

      if(rtranoMULTIfound)
      {
         string rtranos = "";
         foreach(Rtrano rtrano in rtranoList)
         {
            rtranos += rtrano + "\n\n";
         }

         ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nZa serno:\n\n{0}\n\nsam pronašao više ({1}) knjiženja!\n\n{2}\n", vvtb_editingControl.Text, rtranoList.Count, rtranos);
      }

      Rtrano firstFoundRtrano_rec = rtranoList.First();

      PutDgvLineFields2(firstFoundRtrano_rec, currRow, true);

      ZXC.TheVvForm.SetDirtyFlag(sender);
   }

   #endregion Update_SERNO PPUK

   #region Update_SERNO PTG

   public void OnEntry_MOD_Serno_Cell(object sender, EventArgs e) // RTRANO !!! 
   {
      //FakturPDUC theDUC = this as FakturPDUC;
      //
      //VvTextBox vvtb = sender as VvTextBox;
      //
      //if(TheVvTabPage.WriteMode  != ZXC.WriteMode.Edit) return;
      //if(vvtb                    == null              ) return;
    ////if(vvtb.Text               == this.originalText ) return;
    ////if(vvtb.EditedHasChanges() == false             ) return;
    ////if(vvtb.Text.              IsEmpty()            ) return;
      //
      //int rowIdx = TheG2.CurrentRow.Index;
      //
      //Rtrano ovaj_rtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx, false, null);


   }
   public void OnEntry_UgAnDo_Serno_Cell(object sender, EventArgs e) // RTRANO !!! 
   {
      FakturPDUC theDUC = this as FakturPDUC;

      VvTextBox vvtb = sender as VvTextBox;

      if(TheVvTabPage.WriteMode  != ZXC.WriteMode.Edit) return;
      if(vvtb                    == null              ) return;
    //if(vvtb.Text               == this.originalText ) return;
    //if(vvtb.EditedHasChanges() == false             ) return;
    //if(vvtb.Text.              IsEmpty()            ) return;

      int rowIdx = TheG2.CurrentRow.Index;

      uint rtransov_t_serial_od_ovog_rtranoa = TheG2.GetUint32Cell(theDUC.DgvCI2.iT_paletaNo, rowIdx, false);

      if(rtransov_t_serial_od_ovog_rtranoa.IsPositive() == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema vezu na stavku ugovora?!");
         return;
      }

      Rtrans rtrans_rec_od_ovog_rtranoa = faktur_rec.Transes.SingleOrDefault(rtr => rtr.T_serial == rtransov_t_serial_od_ovog_rtranoa);

      if(rtrans_rec_od_ovog_rtranoa == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema rtrans-a?!");
         return;
      }

      Rtrano ovaj_rtrano_rec           = (Rtrano)GetDgvLineFields2(rowIdx, false, null);
      Rtrano kakavTrebaBiti_rtrano_rec = new Rtrano(rtrans_rec_od_ovog_rtranoa);

      if(ovaj_rtrano_rec.T_artiklCD != kakavTrebaBiti_rtrano_rec .T_artiklCD) ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl ovog retka\n\r{0}\n\rse ne podudara sa artiklom retka ugovora\n\r{1}"     , ovaj_rtrano_rec.T_artiklCD, kakavTrebaBiti_rtrano_rec .T_artiklCD);
      if(ovaj_rtrano_rec.T_skladCD  != Fld_SkladCD2                         ) ZXC.aim_emsg(MessageBoxIcon.Error, "Skladiste ovog retka\n\r{0}\n\rse ne podudara sa skladistem retka ugovora\n\r{1}", ovaj_rtrano_rec.T_skladCD , Fld_SkladCD2                         );
      
      decimal PCK_RAM_ovog_rtranoa = ovaj_rtrano_rec          .T_PCK_RAM;
      decimal PCK_HDD_ovog_rtranoa = ovaj_rtrano_rec          .T_PCK_HDD;
      decimal PCK_RAM_rtransa      = kakavTrebaBiti_rtrano_rec.T_PCK_RAM;
      decimal PCK_HDD_rtransa      = kakavTrebaBiti_rtrano_rec.T_PCK_HDD;

      if(PCK_RAM_ovog_rtranoa != PCK_RAM_rtransa) ZXC.aim_emsg(MessageBoxIcon.Error, "PCK_RAM ovog retka\n\r{0}\n\rse ne podudara sa PCK_RAM-om retka ugovora\n\r{1}", PCK_RAM_ovog_rtranoa.ToString0Vv(), PCK_RAM_rtransa.ToString0Vv());
      if(PCK_HDD_ovog_rtranoa != PCK_HDD_rtransa) ZXC.aim_emsg(MessageBoxIcon.Error, "PCK_HDD ovog retka\n\r{0}\n\rse ne podudara sa PCK_HDD-om retka ugovora\n\r{1}", PCK_HDD_ovog_rtranoa.ToString0Vv(), PCK_HDD_rtransa.ToString0Vv());

   }

#if someOLD_WAY
   protected void OnExit_Check_PCK_Serno_For_UgAnDo(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar)                           return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvDataGridView theGrid2 = sender as VvDataGridView;

      int currRowIdx = theGrid2.CurrentRow.Index;

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;
      
      #endregion Init stuff

    //string theSerno = vvtb_editingControl.Text;
      string theSerno = theGrid2.GetStringCell(ci2.iT_serno, currRowIdx, true);

      if(theSerno.IsEmpty()) return;

      #region Check for double serno entry

      List<string> sernosInUseList = new List<string>();

      for(int rowIdx = 0; rowIdx < TheG2.RowCount - 1; ++rowIdx)
      {
         sernosInUseList.Add(TheG2.GetStringCell(ci2.iT_serno, rowIdx, true));
      }

    //int theSernoCount = faktur_rec.TrnNonDel2.Where(rto => rto.T_serno           == theSerno          ).Count();
    //int theSernoCount = faktur_rec.TrnNonDel2.Where(rto => rto.T_serno.ToLower() == theSerno.ToLower()).Count();
      int theSernoCount = sernosInUseList.Where(siu => siu.ToLower() == theSerno.ToLower()).Count();

      if(theSernoCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!");
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      (PCK_Unikat thePCK_Unikat, Rtrano last_rtrano_rec_forThisSerno) = RtranoDao.Get_PCK_Unikat_And_LastRtrano(TheDbConnection, theSerno);

      Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(currRowIdx, false, null);

      string upisaniArtiklCD = theGrid2.GetStringCell(ci2.iT_artiklCD, currRowIdx, false);

      if(upisaniArtiklCD.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Question, "Koji je smisao zadavanja serno-a ako je artiklCD prazan?!");
         return;
      }

      if(thePCK_Unikat == null)
      {
         string upisaniArtiklTS = theGrid2.GetStringCell(ci2.iT_artiklTS, currRowIdx, false);
         
         bool isNOsernoTS  = 
            upisaniArtiklTS == ZXC.USL_TS ||
            upisaniArtiklTS == ZXC.KMP_TS ||
            upisaniArtiklTS == ZXC.OTH_TS  ;

         if(isNOsernoTS)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema smisla uparivati serijski broj sa uslugom, komponentom i 'ostalo'.");

            //theGrid2.PutCell(ci2.iT_serno, currRowIdx, theSerno);
            theGrid2.EditingControl.Text = "";
            return;
         }

         if(theSerno.NotEmpty()) ZXC.aim_emsg(MessageBoxIcon.Information, "Obavili ste inicijalno uparivanje ovog serijskog broja.");

         return; // NOVI serno, ... nije naso nist po tom serno-u 
      }

      bool wasEmptyRow = upisaniArtiklCD.IsEmpty();

      // Provjera da li upisani serno odgovara UgAnDo rtrans, izlaznom skladistu i UgAnDo rtrans PCK potpisu 

      uint rtransov_t_serial_od_ovog_rtranoa = theGrid2.GetUint32Cell (ci2.iT_paletaNo, currRowIdx, false);
      string izlaz_sklCD_ovog_rtranoa        = theGrid2.GetStringCell (ci2.iT_skladCD , currRowIdx, false);
      decimal PCK_RAM_ovog_rtranoa           = theGrid2.GetDecimalCell(ci2.iT_dimZ    , currRowIdx, false);
      decimal PCK_HDD_ovog_rtranoa           = theGrid2.GetDecimalCell(ci2.iT_decC    , currRowIdx, false);

      Rtrans rtrans_od_ovog_rtranoa = faktur_rec.Transes.SingleOrDefault(rtr => rtr.T_serial == rtransov_t_serial_od_ovog_rtranoa);

      if(rtrans_od_ovog_rtranoa != null) // sad provjeri konzistentnost skladista i PCK potpisa 
      {
         if(PCK_RAM_ovog_rtranoa != thePCK_Unikat.PCK_RAM ||
            PCK_HDD_ovog_rtranoa != thePCK_Unikat.PCK_HDD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ovaj serijski broj je zatečen sa PCK potpisom\n\r\n\r[{0}]\n\r\n\rkoji ne odgovara PCK potpisu sa stavke ugovora\n\r\n\r[{1}]",
               UGNorAUN_PTG_DUC.PCK_Signature_ToString(thePCK_Unikat.PCK_RAM   , thePCK_Unikat.PCK_HDD   ), 
               UGNorAUN_PTG_DUC.PCK_Signature_ToString(PCK_RAM_ovog_rtranoa, PCK_HDD_ovog_rtranoa));

            //theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
            theGrid2.EditingControl.Text = "";
            return;
         }

         if(rtrans_od_ovog_rtranoa.T_skladCD != thePCK_Unikat.PCK_SklCD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ovaj serijski broj je zatečen na skladistu [{0}]\n\r\n\rkoje ne odgovara izlaznom skladištu stavke ugovora [{1}]", thePCK_Unikat.PCK_SklCD, rtrans_od_ovog_rtranoa.T_skladCD);

            //theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
            theGrid2.EditingControl.Text = "";
            return;
         }
      }
      else
      {
         // ?! 
      }

      if(wasEmptyRow)
      {
         throw new Exception("Nemoguce?! ... wasEmptyRow is true");
         //theGrid2.ClearRowContent(currRowIdx);
         //
         //Put_PCK_info_DgvLineFields2(lastRtrano_rec_ovog_sernoa, currRowIdx);
      }
      else // konzumiramo prethodno upareni serijski broj 
      {
         if(upisaniArtiklCD == thePCK_Unikat.PCK_ArtCD) // vec prethodno (dobro) uparen sa upisanimArtiklCD-om 
         {
          //ZXC.aim_emsg(MessageBoxIcon.Warning, "Ovaj serijski broj je već ranije bio uparen sa ovim artiklom.");

            last_rtrano_rec_forThisSerno.T_skladCD = Fld_SkladCD2;
         }
         else // vec prethodno uparen sa artiklom RAZLICITIM od upisanimArtiklCD-om 
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ovaj serijski broj je već ranije bio uparen sa DRUGIM artiklom!?\n\r\n\rSerno [{0}]\n\r\n\rUparujete sa[{1}]\n\r\n\rA prethodno je uparen sa [{2}]\n\r\n\rOdbijeno.",
               thePCK_Unikat.PCK_Serno, upisaniArtiklCD, thePCK_Unikat.PCK_ArtCD);
            theGrid2.EditingControl.Text = "";
         }
      }

      ZXC.TheVvForm.SetDirtyFlag(sender);
   }
#endif
   protected void OnExit_Check_PCK_Serno_For_PVR_PTG_DUC(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar)                           return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvDataGridView theGrid2 = sender as VvDataGridView;

            VvTextBoxEditingControl vvTbEc  = theGrid2.EditingControl as VvTextBoxEditingControl;
      bool sernoIsEditedAndHasChanges = vvTbEc.EditedHasChanges();
      bool sernoIsUnchanged           = !sernoIsEditedAndHasChanges;

      bool unchangedSernoinEditWriteMode = sernoIsUnchanged && TheVvTabPage.WriteMode == ZXC.WriteMode.Edit;

      if(unchangedSernoinEditWriteMode) return; // STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP 

      int currRowIdx = theGrid2.CurrentRow.Index;

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      PVR_PTG_DUC thePVR_DUC = this as PVR_PTG_DUC;

      VvTextBox currVvTextBox = theGrid2.EditingControl as VvTextBox;

      if(currVvTextBox.ReadOnly == true) return; // input was disabled, do nothing 

      string theSerno = theGrid2.GetStringCell(ci2.iT_serno, currRowIdx, true);

      theGrid2.ClearRowContent(currRowIdx);
    //the_PVR_DUC.Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
      theGrid2.PutCell(ci2.iT_serno, currRowIdx, theSerno);

      if(theSerno.IsEmpty()) return;

      Rtrano last_rtrano_rec_forThisSerno = new Rtrano();

      #endregion Init stuff

      #region Check for double serno entry

      int theSernoCount = SernoCountOnGrid(theGrid2, theSerno);

      if(theSernoCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", theSerno);
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      #region Postoji li uopce u bazi ovaj serno?

      bool isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(TheDbConnection, last_rtrano_rec_forThisSerno, theSerno, true);

      if(isLastRtrano_ForSerno_found == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nepoznat serijski broj?!");
         e.Cancel = true;
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         return;
      }

      #endregion postoji li uopce u bazi ovaj serno?

      #region Smije li ovaj serno doc na ovaj POVRAT?

      uint UGAN_ttNum = thePVR_DUC.UGAN_ttNum_ofThisDUC;

      List<Rtrano> UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList_stillUNJonly(TheDbConnection, UGAN_ttNum);

      if(UGAN_RtranoList.Contains(last_rtrano_rec_forThisSerno) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj NIJE u ovom najmu?!\n\r\n\r{0}", last_rtrano_rec_forThisSerno);
         e.Cancel = true;
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         return;
      }

      #endregion smije li ovaj serno doc na ovaj POVRAT?

      #region Korigiraj SklaCD, RtrRecID i pukni ga na grid 

      string povratNaSkladCD = thePVR_DUC.faktur_rec.SkladCD2;

      Rtrano newPVR_rtrano_rec = last_rtrano_rec_forThisSerno.MakeDeepCopy();

      newPVR_rtrano_rec.T_skladCD = povratNaSkladCD;

      uint theT_RecID = Get_UgAnDod_Rtrans_T_recID_fromRtrano(newPVR_rtrano_rec);

      newPVR_rtrano_rec.T_rtrRecID = theT_RecID;

      thePVR_DUC.PutDgvLineFields2(newPVR_rtrano_rec, currRowIdx, true);

      theGrid2.PutCell(ci2.iT_skladCD1, currRowIdx, ZXC.PTG_UNJ);

      #endregion korigiraj SklaCD, RtrRecID i pukni ga na grid 
   }

   internal /*bool*/int SernoCountOnGrid(VvDataGridView theGrid2, string theSerno)
   {
      int theSernoCount;

      List<string> sernosInUseList = new List<string>();

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      for(int rowIdx = 0; rowIdx < theGrid2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx)
      {
         sernosInUseList.Add(theGrid2.GetStringCell(ci2.iT_serno, rowIdx, true));
      }

      theSernoCount = sernosInUseList.Where(siu => siu.ToLower() == theSerno.ToLower()).Count();

      return theSernoCount /*> 1*/;
   }

   private void ClearZIZrtranoDGVRow(VvDataGridView theGrid2, FakturPDUC.Rtrano_colIdx ci2, int currRowIdx)
   {
      theGrid2.PutCell(ci2.iT_serno      , currRowIdx, "");

      theGrid2.PutCell(ci2.iT_artiklCD   , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_artiklName , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_jm         , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_artiklTS   , currRowIdx, "");

      theGrid2.PutCell(ci2.iT_RAM_new    , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_HDD_new    , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_grCD       , currRowIdx, "");

      theGrid2.PutCell(ci2.iT_skladCD    , currRowIdx, "");
      theGrid2.PutCell(ci2.iT_skladCD1   , currRowIdx, "");

   }
   protected void OnExit_Check_PCK_Serno_For_ZIZ_PTG_DUC(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvDataGridView theGrid2 = sender as VvDataGridView;

      VvTextBoxEditingControl vvTbEc  = theGrid2.EditingControl as VvTextBoxEditingControl;
      bool sernoIsEditedAndHasChanges = vvTbEc.EditedHasChanges();
      bool sernoIsUnchanged           = !sernoIsEditedAndHasChanges;

      bool unchangedSernoinEditWriteMode = sernoIsUnchanged && TheVvTabPage.WriteMode == ZXC.WriteMode.Edit;

      if(unchangedSernoinEditWriteMode) return; // STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP STOP 

      int currRowIdx = theGrid2.CurrentRow.Index;

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      ZIZ_PTG_DUC theZIZ_DUC = this as ZIZ_PTG_DUC;

      VvTextBox currVvTextBox = theGrid2.EditingControl as VvTextBox;

      if(currVvTextBox.ReadOnly == true) return; // input was disabled, do nothing 

      string tilda_serno = "";

      string theSerno = theGrid2.GetStringCell(ci2.iT_serno, currRowIdx, true);
      string theT_TT  = theGrid2.GetStringCell(ci2.iT_TT   , currRowIdx, true);

      if(theT_TT.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zadajte prvo TT - tip transakcije u prvoj koloni redka!");
         e.Cancel = true;
         theGrid2.EndEdit();
         ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
         return;
      }

      //13.05.2025. zbog 3 space-a 
      //if(theSerno.IsEmpty()) return;
      if(theSerno == "")
      {
         ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);

         return;
      }

      Rtrano last_rtrano_rec_forThisSerno = new Rtrano();

      Artikl artikl_rec;

      int theSernoCount;

      #endregion Init stuff

      #region Check for double serno entry

      theSernoCount = SernoCountOnGrid(theGrid2, theSerno);

      if(theSernoCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", theSerno);
         theGrid2.EndEdit();
         ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
         e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      #region 3 Sp aces - Set Rtrano via Artikl (insted Serno) 

      bool wantsFindArtikl = theSerno.StartsWith("   ");

      if(wantsFindArtikl)
      {
         theGrid2.EndEdit();

         VvTextBox _VvTextBox = theGrid2.Columns[ci2.iT_artiklCD].Tag as VvTextBox;

         object findResult = VvUserControl.UpdateVvDataRecord(_VvTextBox.JAM_AutoCompleteRecordName,
                                                              _VvTextBox.JAM_AutoCompleteSorterType,
                                                              _VvTextBox.JAM_AutoCompleteRestrictor,
                                                            /*_VvTextBox.Text*/theSerno.TrimStart(' '),
                                                              _VvTextBox);
         if(findResult != null)
         {
            artikl_rec = Get_Artikl_FromVvUcSifrar(findResult.ToString());

            theGrid2.PutCell(ci2.iT_artiklCD   , currRowIdx, artikl_rec.ArtiklCD  );
            theGrid2.PutCell(ci2.iT_artiklName , currRowIdx, artikl_rec.ArtiklName);
            theGrid2.PutCell(ci2.iT_jm         , currRowIdx, artikl_rec.JedMj     );
            theGrid2.PutCell(ci2.iT_artiklTS   , currRowIdx, artikl_rec.TS        );

            theGrid2.PutCell(ci2.iT_RAM_new    , currRowIdx, artikl_rec.PCK_RAM   );
            theGrid2.PutCell(ci2.iT_HDD_new    , currRowIdx, artikl_rec.PCK_HDD   );

            Rtrano rtrano_rec = (Rtrano)GetDgvLineFields2(currRowIdx, false, null);

            #region ZI2 Rules - Bijeli redak like DIZ

            if(theT_TT == Faktur.TT_ZI2)
            {
               tilda_serno = rtrano_rec.Get_PTG_tilda_serno(artikl_rec/*.TS*/);
            }

            #endregion ZI2 Rules - Bijeli redak like DIZ

            #region ZU2 Rules - Zeleni redak like PVR

            if(theT_TT == Faktur.TT_ZU2)
            {
               List<Rtrano> chooseFromThis_UNJ_rtranoList = null;

               if(theZIZ_DUC.IsZIZ_Normalan)
               {
                  (string UgAn_TT, uint UGAN_ofThisZIZ_TtNum) = UGNorAUN_PTG_DUC.Get_UgAnFaktur_TtAndTtNum_ForThisRtranoTtAndTtNum(rtrano_rec);
               
                  List<Rtrano> UGAN_stillUNJ_rtranoList = RtranoDao.Get_UGAN_RtranoList_stillUNJonly(TheDbConnection, /*faktur_rec.TtNum*/ UGAN_ofThisZIZ_TtNum);

                  chooseFromThis_UNJ_rtranoList = UGAN_stillUNJ_rtranoList.Where(rto => rto.T_artiklCD == artikl_rec.ArtiklCD).ToList();
               }

               if(theZIZ_DUC.Fld_PTG_IsZIZunaprijed)
               {
                  chooseFromThis_UNJ_rtranoList = RtranoDao.GetRtranoList_For_KupdobCdAndArtiklCd(TheDbConnection, rtrano_rec.T_kupdobCD, rtrano_rec.T_artiklCD);
               }

               Rtrano firstOfThisArtikl_UNJ_rtrano_rec = chooseFromThis_UNJ_rtranoList.FirstOrDefault();
             
               if(firstOfThisArtikl_UNJ_rtrano_rec != null)
               {
                  if(chooseFromThis_UNJ_rtranoList.Count() == 1)
                  {
                     tilda_serno = firstOfThisArtikl_UNJ_rtrano_rec.T_serno;
                  }
                  else // Select serno from ChooseUDP Dialog 
                  {
                     List<ZXC.VvUtilDataPackage> udpList = chooseFromThis_UNJ_rtranoList.Select(rto => new ZXC.VvUtilDataPackage() { TheStr1 = rto.T_serno }).ToList();
             
                     string text = "Tilda serno za artikl: " + artikl_rec.ArtiklName /*+ " na " +  UgAn_TT + "-" + UGAN_ofThisZIZ_TtNum.ToString()*/;
             
                     tilda_serno = UDP_Dlg.ChooseUDP(udpList, text, 1).TheStr1;
             
                     if(tilda_serno == null)
                     { 
                        theGrid2.EndEdit();
             
                        ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
             
                        e.Cancel = true;
                        return;
                     }
             
                     bool artiklHasRealSerno = Artikl.ThisArtikl_Ima_Real_Serno(artikl_rec.ArtiklCD);
                     bool sernoIsReal        = tilda_serno.StartsWith(ZXC.PTG_PENDING_SernoPreffix) == false;
             
                     if(artiklHasRealSerno && sernoIsReal) // za PCK, monitore, ... ne damo da s liste odabera pravi serno neg samo olfaSerno 
                     {
                        ZXC.aim_emsg(MessageBoxIcon.Error, "Na ovaj način smiju se birati samo olfa serijski brojevi.\n\r\n\rRealni serijski broj zadajte barkod readerom.");

                        tilda_serno = "";

                        theGrid2.EndEdit();

                        ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);

                        e.Cancel = true;
                        return;
                     }
                  }
             
               } // if(firstOfThisArtikl_UNJ_rtrano_rec != null)
             
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Artikla\n\r\n\r{0}\n\r\n\rNEMA u najmu?!", artikl_rec);
               
                  tilda_serno = "";
               
                  theGrid2.EndEdit();
               
                  ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
               
                  e.Cancel = true;
                  return;
               }
            }

            #endregion ZU2 Rules - Zeleni redak like PVR

            #region Check for double serno entry

            theSernoCount = SernoCountOnGrid(theGrid2, tilda_serno);

            if(theSernoCount > 0) // nota bene '> 0' a ne od 1 jer serno kojeg ispitujemo jos nije na gridu 
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", tilda_serno);
               theGrid2.EndEdit();

               ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);

               e.Cancel = true;
               return;
            }

            #endregion Check for double serno entry

            else // it's OK, nije double serno entry 
            {
               theSerno = tilda_serno;

               theGrid2.PutCell(ci2.iT_serno, currRowIdx, theSerno);
            }

         } // if(findResult != null)

         else // stisnuo je 'odustani' na find dialogu 
         {
            ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);

            return;
         }

      } // if(wantsFindArtikl) 

      #endregion 3 Spaces - Set Rtrano via Artikl (insted Serno) 

      #region Nađi last_rtrano_rec_forThisSerno i pukni ga na grid

      bool isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(TheDbConnection, last_rtrano_rec_forThisSerno, theSerno, true);

      #endregion Nađi last_rtrano_rec_forThisSerno i pukni ga na grid

      #region ZI2 Rules - Bijeli redak like DIZ

      if(theT_TT == Faktur.TT_ZI2) // ponasaj se (provjeravaj) kao da smo na DIZ-u 
      {
         string izdajemoSaSkladCD = theZIZ_DUC.faktur_rec.SkladCD;

         if(isLastRtrano_ForSerno_found && last_rtrano_rec_forThisSerno.T_skladCD != izdajemoSaSkladCD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne može. Serno nije na skladištu izdavanja [{0}]", izdajemoSaSkladCD);
            e.Cancel = true;
            theGrid2.EndEdit();
            ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
            return;
         }

         #region Korigiraj T_skladCD, T_rtrRecID i pukni ga na grid 

         Rtrano new_ZI2_rtrano_rec;

         if(isLastRtrano_ForSerno_found)
         {
            new_ZI2_rtrano_rec = last_rtrano_rec_forThisSerno.MakeDeepCopy();
         }
         else // uparivanje serno-a sa artiklom 
         {
            string artiklCD = theGrid2.GetStringCell(ci2.iT_artiklCD, currRowIdx, true);

            if(artiklCD.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj je nepoznat. Za uparivanje novog ser. broja prvo sa '3 razmaknice' odaberite artikl.");
               e.Cancel = true;
               theGrid2.EndEdit();
               ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
               return;
            }

            artikl_rec = Get_Artikl_FromVvUcSifrar(artiklCD);

            bool needsRealSerno = Artikl.ThisArtikl_Ima_Real_Serno(artikl_rec.ArtiklCD);

            if(theSerno.NotEmpty() && Rtrano.IsSernoReal(theSerno) && needsRealSerno == false) // nedaj uparivati novi serno sa 'poklopcem' tj. artikl TS-om koji nema serno 
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj je u sustavu nepoznat (novi) te ga nema smisla uparivati sa artiklom koji ga ne treba.");
               e.Cancel = true;
               theGrid2.EndEdit();
               ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
               return;
            }

            new_ZI2_rtrano_rec = new Rtrano()
            {
               T_artiklCD   = artikl_rec.ArtiklCD  ,
               T_artiklName = artikl_rec.ArtiklName,
               T_dimZ       = artikl_rec.PCK_RAM   ,
               T_decC       = artikl_rec.PCK_HDD   ,

               T_serno      = theSerno              
            };
         }

         new_ZI2_rtrano_rec.T_skladCD = ZXC.PTG_UNJ;
         new_ZI2_rtrano_rec.T_TT      = theT_TT    ;

         theZIZ_DUC.PutDgvLineFields2(new_ZI2_rtrano_rec, currRowIdx, true);

         theGrid2.PutCell(ci2.iT_skladCD1, currRowIdx, izdajemoSaSkladCD);

         #endregion korigiraj T_skladCD, T_rtrRecID i pukni ga na grid 

      } // if(theT_TT == Faktur.TT_ZI2) 

      #endregion ZI2 Rules - Bijeli redak like DIZ

      #region ZU2 Rules - Zeleni redak like PVR

      if(theT_TT == Faktur.TT_ZU2)
      {
         #region Postoji li uopce u bazi ovaj serno?

         if(isLastRtrano_ForSerno_found == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Nepoznat serijski broj?!");
            e.Cancel = true;
            theGrid2.EndEdit();
            ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
            return;
         }

         #endregion postoji li uopce u bazi ovaj serno?

         #region Smije li ovaj serno doc na ovaj POVRAT (ZU2)?

         uint UGAN_ttNum = theZIZ_DUC.UGAN_ttNum_ofThisDUC;

         List<Rtrano> UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList_stillUNJonly(TheDbConnection, UGAN_ttNum);

         if(theZIZ_DUC.Fld_PTG_IsZIZunaprijed)
         {
            // Da li je ovaj serno u ikojem najmu kod ovog Kupdoba 
            if(last_rtrano_rec_forThisSerno.T_kupdobCD != theZIZ_DUC.Fld_KupdobCd || 
               last_rtrano_rec_forThisSerno.T_skladCD  != ZXC.PTG_UNJ              )
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj NIJE u najmu kod ovog kupca?!");
               e.Cancel = true;
               theGrid2.EndEdit();
               ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
               return;
            }
         }
         else // Normalan ZIZ, nije unaprijed 
         {
            // Da li je ovaj serno u ovom najmu (UgAn 'ECO Sustavu') 
            if(UGAN_RtranoList.Contains(last_rtrano_rec_forThisSerno) == false)
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj NIJE u ovom najmu?!\n\r\n\r{0}", last_rtrano_rec_forThisSerno);
               e.Cancel = true;
               theGrid2.EndEdit();
               ClearZIZrtranoDGVRow(theGrid2, ci2, currRowIdx);
               return;
            }
         }

         #endregion smije li ovaj serno doc na ovaj POVRAT?

         #region Korigiraj T_skladCD, T_rtrRecID i pukni ga na grid 

         string povratNaSkladCD = theZIZ_DUC.faktur_rec.SkladCD2;

         Rtrano new_ZU2_rtrano_rec = last_rtrano_rec_forThisSerno.MakeDeepCopy();

         uint theRtrans_T_recID = Get_UgAnDod_Rtrans_T_recID_fromRtrano(new_ZU2_rtrano_rec);

         new_ZU2_rtrano_rec.T_TT       = theT_TT          ;
         new_ZU2_rtrano_rec.T_skladCD  = povratNaSkladCD  ;
         new_ZU2_rtrano_rec.T_rtrRecID = theRtrans_T_recID;

         theZIZ_DUC.PutDgvLineFields2(new_ZU2_rtrano_rec, currRowIdx, true);

         theGrid2.PutCell(ci2.iT_skladCD1, currRowIdx, ZXC.PTG_UNJ);

         if(theZIZ_DUC.Fld_PTG_IsZIZunaprijed) // ZIZ unaprijed prelazi u normalnoga 
         {
            (string uganTT, uint UgAn_TtNum2, uint V1_KUGnum, uint V2_UGANnum) = UGNorAUN_PTG_DUC.Get_KUGnum_and_UGANnum_from_UgAnDod_Rtrano(last_rtrano_rec_forThisSerno);

            uint next_ZIZ_dod_TtNum = TheVvDao.GetNext_PTG_KUGinTtNum_TtNum(TheDbConnection, Faktur.TT_ZIZ, V1_KUGnum, V2_UGANnum );

            Fld_V1_ttNum = V1_KUGnum         ;
            Fld_V2_ttNum = V2_UGANnum        ;
            Fld_TtNum    = next_ZIZ_dod_TtNum;

            #region Apply next_ZIZ_dod_TtNum to all 'tilda' sernos

            Rtrano DGVrtrano_rec;
            string oldSerno;
            string newSerno;
            string oldTtNumStr = "1"                          ; // TODO !!! 
            string newTtNumStr = next_ZIZ_dod_TtNum.ToString();

            for(int rowIdx2 = 0; rowIdx2 < TheG2./*RowCount - 1*/VvEffectiveRowCount; ++rowIdx2)
            {
               DGVrtrano_rec = (Rtrano)GetDgvLineFields2(rowIdx2, false, null);

               if(Rtrano.IsSernoUnReal(DGVrtrano_rec.T_serno))
               {
                  oldSerno = DGVrtrano_rec.T_serno;
                  newSerno = DGVrtrano_rec.T_serno.Replace(" " + oldTtNumStr + "_", " " + newTtNumStr + "_");

                  theGrid2.PutCell(ci2.iT_serno, rowIdx2, newSerno);
               }
            }

            #endregion Apply next_ZIZ_dod_TtNum to all 'tilda' sernos

            theZIZ_DUC.Fld_PTG_IsZIZunaprijed = false;

            ZXC.aim_emsg(MessageBoxIcon.Information, "Ovime je ova zamjena 'unaprijed' pretvorena u normalnu zamjenu.\n\r\n\rPridružena je ugovoru\n\r\n\r{0} {1}", uganTT, UgAn_TtNum2);
         }

         #endregion korigiraj T_skladCD, T_rtrRecID i pukni ga na grid 

      } // if(theT_TT == Faktur.TT_ZU2) 

      #endregion ZU2 Rules - Zeleni redak like PVR

   }

   internal uint Get_UgAnDod_Rtrans_T_recID_fromRtrano(Rtrano last_rtrano_rec_forThisSerno)
   {
      return Get_UgAnDod_Rtrans_fromRtrano(last_rtrano_rec_forThisSerno).T_recID;
   }

   internal Rtrans Get_UgAnDod_Rtrans_fromRtrano(Rtrano last_rtrano_rec_forThisSerno)
   {
      Rtrans UgAnDod_rtrans_rec = new Rtrans();

      string theTT       = ZXC.TtInfo(last_rtrano_rec_forThisSerno.T_TT).LinkedDefaultTT; // imam/znam 'UG2' a trebam 'UGN' 
      uint   theTtNum    = last_rtrano_rec_forThisSerno.T_ttNum                         ;
      uint   theSerial   = last_rtrano_rec_forThisSerno.T_paletaNo                      ;

      RtransDao.SetMe_Rtrans_byTt_TtNum_Serial(TheDbConnection, UgAnDod_rtrans_rec, theTT, theTtNum, theSerial);

      return UgAnDod_rtrans_rec;
   }

   protected void OnExit_Check_PCK_Serno_For_MOD(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar)                           return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvDataGridView theGrid2 = sender as VvDataGridView;

      int currRowIdx = theGrid2.CurrentRow.Index;

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      MOD_PTG_DUC the_MOD_DUC = this as MOD_PTG_DUC;

      #endregion Init stuff

      VvTextBox currVvTextBox = theGrid2.EditingControl as VvTextBox;

      if(currVvTextBox.ReadOnly == true) return; // input was disabled, do nothing 

      string theSerno = theGrid2.GetStringCell(ci2.iT_serno, currRowIdx, true);

      theGrid2.ClearRowContent(currRowIdx);
      the_MOD_DUC.Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
      theGrid2.PutCell(ci2.iT_serno, currRowIdx, theSerno);

      if(theSerno.IsEmpty()) return;

      #region Check for double serno entry

      int theSernoCount = SernoCountOnGrid(theGrid2, theSerno);

      if(theSernoCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", theSerno);
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      (PCK_Unikat thePCK_Unikat, Rtrano last_rtrano_rec_forThisSerno) = RtranoDao.Get_PCK_Unikat_And_LastRtrano(TheDbConnection, theSerno);

      Artikl artikl_rec = thePCK_Unikat == null ? null : Get_Artikl_FromVvUcSifrar/*ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD)*/(thePCK_Unikat.PCK_ArtCD);
      if(artikl_rec != null && artikl_rec.TS != ZXC.PCK_TS)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Serijski broj se ne odnosi na PCK artikl!");
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      if(thePCK_Unikat == null)
      {
         return; // NOVI serno, ... nije naso nist po tom serno-u 
      }

      // ak smo dosli do tu, znaci da je u pitanju postojeci serno 
      // pa mu idemo iskoristiti lastRtrano_rec stuff              

      if(last_rtrano_rec_forThisSerno.T_skladCD == ZXC.PTG_UNJ)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne može. Koji li je smisao modificiranja serijskog broja koji se nalazi na 'UNJ' skladištu?!");
         e.Cancel = true;
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         return;
      }

      if(ThisIs_MOC_rowIndex(currRowIdx) && thePCK_Unikat.PCK_BazaCD != the_MOD_DUC.Fld_PTG_MOC_PCK_baseCD) // e, al' nedaj ako ne odgovara PCK baza a na prvih smo n 'MOC' redaka! 
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Na prvih {0} redaka se očekuje MOC PCK artikl ({1}).", the_MOD_DUC.Fld_PTG_MOC_RowCount, the_MOD_DUC.Fld_PTG_MOC_PCK_baseCD);

         theGrid2.EndEdit(); // !!! 

         theGrid2.ClearRowContent(currRowIdx);
         the_MOD_DUC.Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
         return;
      }

      #region U 'Ispravi': Ponovno zadavanje serno-a, kojeg smo prvo pobrisali ili pregazili, a koji je postojao u DataLayeru prije ovog ispravka

      if(this.TheVvTabPage.WriteMode == ZXC.WriteMode.Edit)
      {
         Rtrano previousForThisSerno_Rtrano_rec = null;
         foreach(Rtrano rtrano in faktur_rec.Transes2)
         {
            if(rtrano.backupData._t_serno == theSerno)
            {
               previousForThisSerno_Rtrano_rec = rtrano.MakeDeepCopy();
               previousForThisSerno_Rtrano_rec.RestoreBackupData();
            }
         }

         if(previousForThisSerno_Rtrano_rec != null)
         {
          //string oldArtiklCD = Artikl.Get_PTG_CalculatedArtiklCD_From_SenderArtiklCD_NewRAM_NewHDD(previousForThisSerno_Rtrano_rec.T_artiklCD, previousForThisSerno_Rtrano_rec.R_MOD_RAM_old, previousForThisSerno_Rtrano_rec.R_MOD_HDD_old);
            string oldArtiklCD = previousForThisSerno_Rtrano_rec.R_OldArtiklCD;

            last_rtrano_rec_forThisSerno            = previousForThisSerno_Rtrano_rec.MakeDeepCopy();
            last_rtrano_rec_forThisSerno.T_artiklCD = oldArtiklCD;
            last_rtrano_rec_forThisSerno.T_PCK_RAM  = previousForThisSerno_Rtrano_rec.R_MOD_RAM_old;
            last_rtrano_rec_forThisSerno.T_PCK_HDD  = previousForThisSerno_Rtrano_rec.R_MOD_HDD_old;

            ZXC.aim_emsg(MessageBoxIcon.Information, "Ovaj serijski broj je prije ispravka već bio na ovom dokumentu. Ponavljam njegovo 'OldArtikl' obilježje.");
         }
      }

      #endregion U 'Ispravi': Ponovno zadavanje serno-a, kojeg smo prvo pobrisali ili pregazili, a koji je postojao u DataLayeru prije ovog ispravka

      Put_PCK_info_MOD_DgvLineFields2(last_rtrano_rec_forThisSerno, currRowIdx);

      ZXC.TheVvForm.SetDirtyFlag(sender);

      if(ThisIs_MOC_rowIndex(currRowIdx))
      {
         // 7 puta 
         for(int i = 1; i <= 7; i++) SendKeys.Send("{TAB}"); 
      }
      else
      {
         // 3 puta 
         for(int i = 1; i <= 3; i++) SendKeys.Send("{TAB}");
      }

      // odi na RAM+ 
      //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");
      //theGrid.CurrentCell = theGrid[3, 2];                                                                                                                                                

      the_MOD_DUC.Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
      the_MOD_DUC.Put_MOD_Semafor_Labels();

   }

   protected void OnExit_Check_PCK_Serno_For_PTG_UgAnDo_or_Common_DUC(object sender, System.ComponentModel.CancelEventArgs e)
   {
      #region Init stuff

      if(isPopulatingSifrar)                           return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvDataGridView theGrid2 = sender as VvDataGridView;

      VvTextBoxEditingControl vvtb = theGrid2.EditingControl as VvTextBoxEditingControl;
      if(vvtb.EditedHasChanges() == false) return;

      int currRowIdx = theGrid2.CurrentRow.Index;

      FakturPDUC.Rtrano_colIdx ci2 = (this as FakturPDUC).DgvCI2;

      #endregion Init stuff

      string theSerno = theGrid2.GetStringCell(ci2.iT_serno, currRowIdx, true);

      if(theSerno.IsEmpty()) return;

      #region Check for double serno entry

      int theSernoCount = SernoCountOnGrid(theGrid2, theSerno);

      if(theSernoCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", theSerno);
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      string upisaniArtiklCD = theGrid2.GetStringCell(ci2.iT_artiklCD, currRowIdx, false);

      if(upisaniArtiklCD.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Question, "Koji je smisao zadavanja serno-a ako je artiklCD prazan?!");
         return;
      }

      Rtrano this_rtrano_rec              = (Rtrano)GetDgvLineFields2(currRowIdx, false, null);
      Rtrano last_rtrano_rec_forThisSerno = new Rtrano();
      RtranoDao.Get_LastRtrano_ForSerno(TheDbConnection, last_rtrano_rec_forThisSerno, theSerno, true);

      bool sernoIsOLD = last_rtrano_rec_forThisSerno.T_TT   .NotEmpty() && 
                        last_rtrano_rec_forThisSerno.T_ttNum.NotZero ();

      bool sernoIsNEW = !sernoIsOLD;

      FakturPDUC somePTG_DUC = (this as FakturPDUC);

      bool isUlaz_DUC          = somePTG_DUC.faktur_rec.TtInfo.IsFinKol_U;
      bool isIzlaz_DUC         = somePTG_DUC.faktur_rec.TtInfo.IsFinKol_I;
      bool isUgAnDo_DUC        = somePTG_DUC.IsPTG_UgAnDod_DUC           ;
      bool isIzlazOrUgAnDo_DUC = isIzlaz_DUC || isUgAnDo_DUC             ;

      // ======================================================================================================================= 

      // Inicijalno uparivanje serno-a 
      if(sernoIsNEW)
      {
         // provjera uparujemo li ga s adekvatnim TS-om 
       //string upisaniArtiklTS = theGrid2.GetStringCell(ci2.iT_artiklTS, currRowIdx, false);

       //bool isNO_realSernoArtikl = Artikl.DoesThisArtikl_Needs_RtranoRow_ForSerno(upisaniArtiklCD, faktur_rec.TT) == false;
         bool isNO_realSernoArtikl = Artikl.ThisArtikl_Ima_Real_Serno             (upisaniArtiklCD               ) == false;

         if(isNO_realSernoArtikl)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema smisla uparivati serijski broj sa uslugom, komponentom i 'ostalo'.");

            //theGrid2.PutCell(ci2.iT_serno, currRowIdx, theSerno);
            theGrid2.EditingControl.Text = "";
            return;
         }

         return; // uspjesno upareno
      }

      // Na ulazu serno mora biti nov, nemere postojeći 
      if(sernoIsOLD && isUlaz_DUC)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na ulazni dokument ne možete zadati prethodno već uparen serijski broj.\n\r\n\r{0}", last_rtrano_rec_forThisSerno);
         theGrid2.EndEdit();
         theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
         e.Cancel = true;
         return;
      }

      // Na izlazu ili UgAnDo-u ili MSI-u, serno se mora nalaziti na izlaznom skladistu pripadnog rtrans-a 
      // Te se artikli moraju podudarati                                      
      if(sernoIsOLD && isIzlazOrUgAnDo_DUC)
      {
         string lastRtranoSkladCD = last_rtrano_rec_forThisSerno.T_skladCD;

         Rtrans thisRtrans_rec = this.faktur_rec.Transes.SingleOrDefault(rtr => rtr.T_serial == this_rtrano_rec.T_paletaNo);
         if(thisRtrans_rec == null) throw new Exception("Rtrans is NULL for currRowIdx " + currRowIdx + " ugnSt " + this_rtrano_rec.T_paletaNo);

         string thisRtransIzlazSkladCD = thisRtrans_rec.T_skladCD;
         string thisRtranoIzlazSkladCD = thisRtransIzlazSkladCD;

         string thisRtranoArtiklCD = this_rtrano_rec             .T_artiklCD;
         string lastRtranoArtiklCD = last_rtrano_rec_forThisSerno.T_artiklCD;

         if(thisRtranoIzlazSkladCD != lastRtranoSkladCD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Serijski broj\n\r\n\r{0}\n\r\n\rne može izaći sa skl. {1} budući da se nalazi na skl. {2}\n\r\n\r{3}", 
               theSerno, thisRtranoIzlazSkladCD, lastRtranoSkladCD, last_rtrano_rec_forThisSerno);

            theGrid2.EndEdit();
            theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
            e.Cancel = true;
            return;
         }

         if(thisRtranoArtiklCD != lastRtranoArtiklCD)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ova stavka se odnosi na artikl\n\r\n\r{0}\n\r\n\rA serijski broj\n\r\n\r{1}\n\r\n\revidentiran je kao artikl\n\r\n\r{2}\n\r\n\rpo dokumentu\n\r\n\r{3}",
               thisRtranoArtiklCD, theSerno, lastRtranoArtiklCD, last_rtrano_rec_forThisSerno);

            theGrid2.EndEdit();
            theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
            e.Cancel = true;
            return;
         }

         theGrid2.PutCell(ci2.iT_grCD, currRowIdx, last_rtrano_rec_forThisSerno.T_grCD); // opis 

      } // if(sernoIsOLD && isIzlazOrUgAnDo_DUC) 
   }

   private static void Nullify_MOD_rtranoGridRow(VvDataGridView theGrid, int currRowIdx, FakturPDUC.Rtrano_colIdx ci2)
   {
      theGrid.PutCell(ci2.iT_TT         , currRowIdx, "");
      theGrid.PutCell(ci2.iT_kol        , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_RAM_plus   , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_RAM_minus  , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_RAM_new    , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_HDD_plus   , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_HDD_minus  , currRowIdx, 0M);
      theGrid.PutCell(ci2.iT_HDD_new    , currRowIdx, 0M);
      theGrid.PutCell(ci2.iR_ramOld     , currRowIdx, 0M);
      theGrid.PutCell(ci2.iR_hddOld     , currRowIdx, 0M);
   }

   protected bool ThisIs_MOC_rowIndex(int rIdx)
   {
      return rIdx < (this as MOD_PTG_DUC).Fld_PTG_MOC_RowCount;
   }

   private void Put_PCK_info_MOD_DgvLineFields2(Rtrano oldRtrano_rec, int rIdx)
   {
      MOD_PTG_DUC theMOD_DUC = this as MOD_PTG_DUC;

      Rtrano newRtrano_rec = oldRtrano_rec.MakeDeepCopy();

    //lastRtrano_rec.T_PCK_RAM = lastRtrano_rec.R_PCK_RAM;
    //lastRtrano_rec.T_PCK_HDD = lastRtrano_rec.R_PCK_HDD;

      //lastRtrano_rec.T_skladCD = Fld_SkladCD;

      newRtrano_rec.T_TT      = Faktur.TT_MOS;
      newRtrano_rec.T_kol     = 1M;
      newRtrano_rec.T_dimX    = 
      newRtrano_rec.T_dimY    =
      newRtrano_rec.T_decA    = 
      newRtrano_rec.T_decB    = 0M;

      decimal cilj_MOC_RAM      = theMOD_DUC.Fld_Decimal01;
      decimal cilj_MOC_HDD      = theMOD_DUC.Fld_Decimal02;
      string  cilj_MOC_ArtiklCD = theMOD_DUC.Fld_PrjArtCD ;

    //string MOC_ArtiklCD = Artikl.Get_PTG_CalculatedArtiklCD_From_SenderArtiklCD_NewRAM_NewHDD(oldRtrano_rec.T_artiklCD, cilj_MOC_RAM, cilj_MOC_HDD);

      bool isMOCrow = (ThisIs_MOC_rowIndex(rIdx) && theMOD_DUC.Fld_PrjArtCD == cilj_MOC_ArtiklCD);
         
      if(isMOCrow)
      {
         newRtrano_rec.T_TT = Faktur.TT_MOC;

         // ram ______________________________________________ 
         decimal oldRAM  = newRtrano_rec.T_PCK_RAM;

         decimal ramPlus  = cilj_MOC_RAM - oldRAM;
         decimal ramMinus = 0M;
         if(ramPlus.IsNegative())
         {
            ramMinus = -1M * ramPlus;
            ramPlus  = 0M;
         }
         newRtrano_rec.T_dimX    = ramPlus      ;
         newRtrano_rec.T_dimY    = ramMinus     ;
         newRtrano_rec.T_PCK_RAM = cilj_MOC_RAM ; // !!! 

         // hdd ______________________________________________ 
         decimal oldHDD  = newRtrano_rec.T_PCK_HDD;

         decimal hddPlus  = cilj_MOC_HDD - oldHDD;
         decimal hddMinus = 0M;
         if(hddPlus.IsNegative())
         {
            hddMinus = -1M * hddPlus;
            hddPlus  = 0M;
         }
         newRtrano_rec.T_decA    = hddPlus      ;
         newRtrano_rec.T_decB    = hddMinus     ;
         newRtrano_rec.T_PCK_HDD = cilj_MOC_HDD ; // !!! 
      }

      PutDgvLineFields2(newRtrano_rec, rIdx, true); // classic 

    //PutDgvLineResultsFields2(rIdx, newRtrano_rec, false); // RAMnew, HDDnew 

      FakturPDUC.Rtrano_colIdx localCi2 = (this as FakturPDUC).DgvCI2;

      TheG2.PutCell(localCi2.iR_ramOld, rIdx, /*VvCurrency*/(oldRtrano_rec.T_PCK_RAM    ).ToString0Vv());
      TheG2.PutCell(localCi2.iR_hddOld, rIdx, /*VvCurrency*/(oldRtrano_rec.T_PCK_HDD    ).ToString0Vv());

      // ArtiklCD_OLD 
      TheG2.PutCell(localCi2.iR_artiklCD_Old, rIdx, oldRtrano_rec.T_artiklCD);
      TheG2.PutCell(localCi2.iR_grCD_Old    , rIdx, oldRtrano_rec.T_grCD    );

      // ArtiklCD_NEW 
      if(isMOCrow)
      {
         TheG2.PutCell(localCi2.iT_artiklCD, rIdx, cilj_MOC_ArtiklCD);
      }
   }

   protected void fuse2(object sender, EventArgs e)
   {
      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;
      //
      //int rowIdx = TheG2.CurrentRow.Index;
      //int colIdx = TheG2.CurrentCell.ColumnIndex;
      //
      //TheG2[1, 2].Selected = true;
   }


   #endregion Update_SERNO PTG

   #region GoTo some other document

   public void GoTo_RISK_Dokument_Click(object sender, EventArgs e)
   {
      Button btn = sender as Button;

      int vezaRbr = (int)btn.Tag;

      switch(vezaRbr)
      {
         case 1: if(Fld_V1_tt.NotEmpty() && Fld_V1_ttNum.NotZero()) GoTo_RISK_Dokument(Fld_V1_tt, Fld_V1_ttNum); break;
         case 2: if(Fld_V2_tt.NotEmpty() && Fld_V2_ttNum.NotZero()) GoTo_RISK_Dokument(Fld_V2_tt, Fld_V2_ttNum); break;

         case 3: if((this as FakturExtDUC).Fld_V3_tt.NotEmpty() && (this as FakturExtDUC).Fld_V3_ttNum.NotZero()) GoTo_RISK_Dokument((this as FakturExtDUC).Fld_V3_tt, (this as FakturExtDUC).Fld_V3_ttNum); break;
         case 4: if((this as FakturExtDUC).Fld_V4_tt.NotEmpty() && (this as FakturExtDUC).Fld_V4_ttNum.NotZero()) GoTo_RISK_Dokument((this as FakturExtDUC).Fld_V4_tt, (this as FakturExtDUC).Fld_V4_ttNum); break;
         case 10:
            if((this as FakturExtDUC).Fld_CjenikTT.IsEmpty()) break;
            if((this as FakturExtDUC).Fld_CjenikTT == Faktur.TT_CJ_VP1)
            {
               Faktur cjenik_rec = FakturDao.LSTSET_Faktur(TheDbConnection, Faktur.TT_CJ_VP1, (this as FakturExtDUC).Fld_SkladDate, 0);

               if(cjenik_rec != null) GoTo_RISK_Dokument(cjenik_rec.TT, cjenik_rec.TtNum);
               break;
            }
            if((this as FakturExtDUC).Fld_CjenikTT == Faktur.TT_CJ_VP2)
            {
               if((this as FakturExtDUC).Fld_CjenikTTnum.NotZero()) GoTo_RISK_Dokument((this as FakturExtDUC).Fld_CjenikTT, (this as FakturExtDUC).Fld_CjenikTTnum); break;
            }
            break;

      }
   }

   protected int FirstEmptyVezaField
   {
      get
      {

         if(Fld_V1_tt.IsEmpty() && Fld_V1_ttNum.IsZero()) return 1;
         if(Fld_V2_tt.IsEmpty() && Fld_V2_ttNum.IsZero()) return 2;

         if(this is FakturExtDUC == false) return 0;

         FakturExtDUC theExtDUC = this as FakturExtDUC;

         if(theExtDUC.Fld_V3_tt.IsEmpty() && theExtDUC.Fld_V3_ttNum.IsZero()) return 3;
         if(theExtDUC.Fld_V4_tt.IsEmpty() && theExtDUC.Fld_V4_ttNum.IsZero()) return 4;

         return 0;
      }
   }

   public static void GoTo_RISK_Dokument(string _tt, uint _ttNum)
   {
      Point vvSubModulXY = ZXC.TtInfo(_tt).DefaultSubModulXY;

      // 20.12.2012: start 
      FakturDUC currentFakturDUC = ZXC.TheVvForm.TheVvUC as FakturDUC;
      if(currentFakturDUC != null)
      {
         switch(currentFakturDUC.TheSubModul.subModulEnum)
         {
            case ZXC.VvSubModulEnum.R_KLD:
               if(_tt == Faktur.TT_UFA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_UFAdev);
               break;
            case ZXC.VvSubModulEnum.R_UFAdev:
               if(_tt == Faktur.TT_KLK) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_KLD);
               break;

            case ZXC.VvSubModulEnum.R_IZD_SVD:
               if(_tt == Faktur.TT_URA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_URA_SVD);
               if(_tt == Faktur.TT_NRD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_NRD_SVD);
               break;
            case ZXC.VvSubModulEnum.R_URA_SVD:
               if(_tt == Faktur.TT_NRD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_NRD_SVD);
               if(_tt == Faktur.TT_IZD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IZD_SVD);
               break;
            case ZXC.VvSubModulEnum.R_NRD_SVD:
               if(_tt == Faktur.TT_URA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_URA_SVD);
               if(_tt == Faktur.TT_IZD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IZD_SVD);
               break;
            case ZXC.VvSubModulEnum.R_UGO:
               if(_tt == Faktur.TT_URA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_URA_SVD);
               if(_tt == Faktur.TT_NRD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_NRD_SVD);
               break;
            case ZXC.VvSubModulEnum.R_ZAH_SVD:
               if(_tt == Faktur.TT_URA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_URA_SVD);
               if(_tt == Faktur.TT_IZD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IZD_SVD);
               break;
          //case ZXC.VvSubModulEnum.R_PON_MPC:
          //   if(_tt == Faktur.TT_IRA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IRA_MPC);
          //   break;
          //case ZXC.VvSubModulEnum.R_IRA_MPC:
          //   if(_tt == Faktur.TT_PON) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_PON_MPC);
          //   break;
         }
      }
      // 20.12.2012: end    

      // 26.04.2024: 
      if(ZXC.IsTETRAGRAM_ANY)
      {
         if(_tt == Faktur.TT_IRA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IRA_MPC);
         if(_tt == Faktur.TT_PON) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_PON_MPC);
         if(_tt == Faktur.TT_OPN) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_OPN_MPC);
         if(_tt == Faktur.TT_IZD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IZD_MPC);
      }

      if(ZXC.IsPCTOGO)
      {
         if(_tt == Faktur.TT_PRI) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_PRI_PTG);
         if(_tt == Faktur.TT_IZD) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IZD_PTG);
         if(_tt == Faktur.TT_URA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_URA_PTG);
         if(_tt == Faktur.TT_IRA) vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.R_IRA_PTG);

         // tamara todo!
      }
      if(vvSubModulXY == Point.Empty)
      {
         GoTo_MIXER_Dokument(_tt, _ttNum);

         return;
      }

      VvTabPage existingTabPage = ZXC.TheVvForm.TheVvTabPage.TheVvForm.TheTabControl.Documents.Select(d => d.Control as VvTabPage).Where(p => p != null).FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModulXY);

      Faktur linkedFaktur_rec;

      if(existingTabPage == null) linkedFaktur_rec = new Faktur();
      else                        linkedFaktur_rec = (Faktur)existingTabPage.TheVvDataRecord;

      bool dbOK = FakturDao.SetMeFaktur(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, _tt, _ttNum, false);

      if(dbOK == false) return;

      if(existingTabPage != null)
      {
         existingTabPage.Selected = true;

         string currTT    = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TT;
         uint   currTTnum = ((FakturDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TtNum;

         //if(currTT == _tt && currTTnum == _ttNum) return;

         if(dbOK) 
         {
            existingTabPage.TheVvForm.PutFieldsActions(ZXC.TheVvForm.TheDbConnection, linkedFaktur_rec, existingTabPage.TheVvRecordUC); 
         }
      }
      else
      {
         ZXC.TheVvForm.OpenNew_Record_TabPage(vvSubModulXY, linkedFaktur_rec.RecID);
      }

   }

   private static void GoTo_MIXER_Dokument(string _tt, uint _ttNum)
   {
      Point vvSubModulXY = ZXC.TheVvForm.GetSubModulXY(ZXC.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(_tt));

      if(vvSubModulXY == Point.Empty)
      {
         return;
      }

      VvTabPage existingTabPage = ZXC.TheVvForm.TheVvTabPage.TheVvForm.TheTabControl.Documents.Select(d => d.Control as VvTabPage).Where(p => p != null).FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModulXY);

      Mixer linkedMixer_rec;

      if(existingTabPage == null) linkedMixer_rec = new Mixer();
      else                        linkedMixer_rec = (Mixer)existingTabPage.TheVvDataRecord;

      bool dbOK = MixerDao.SetMeMixer(ZXC.TheVvForm.TheDbConnection, linkedMixer_rec, _tt, _ttNum, false);

      if(dbOK == false) return;

      if(existingTabPage != null)
      {
         existingTabPage.Selected = true;

         string currTT    = ((MixerDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TT;
         uint   currTTnum = ((MixerDUC)(existingTabPage.TheVvDocumentRecordUC)).Fld_TtNum;

         //if(currTT == _tt && currTTnum == _ttNum) return;

         if(dbOK) 
         {
            existingTabPage.TheVvForm.PutFieldsActions(ZXC.TheVvForm.TheDbConnection, linkedMixer_rec, existingTabPage.TheVvRecordUC); 
         }
      }
      else
      {
         ZXC.TheVvForm.OpenNew_Record_TabPage(vvSubModulXY, linkedMixer_rec.RecID);
      }
   }

   public void GoToProjektCD_RISK_Dokument_Click(object sender, EventArgs e)
   {
      string tt;
      uint ttNum;

      Ftrans.ParseTipBr(Fld_ProjektCD, out tt, out ttNum);

      if(tt.IsEmpty()) return;

      GoTo_RISK_Dokument(tt, ttNum);

   }

   #endregion GoTo some other document

   #region VvCurrency converter

   internal ZXC.ValutaNameEnum ValutaNameInUse { get; set; }
   public bool    IsShowingConvertedMoney { get; set; }
   public decimal DevTecaj                { get; set; }

   public ZXC.ValutaNameEnum Fld_ShowInValutaLookUp
   {
      get
      {
         if(tbx_IznosUvaluti.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                                return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_IznosUvaluti.Text, true);
      }
      set
      {
         if(value == ZXC.ValutaNameEnum.EMPTY) tbx_IznosUvaluti.Text = "";
         else                                  tbx_IznosUvaluti.Text = value.ToString();

         ValutaNameInUse = value;
      }
   }

   void tbx_IznosUvaluti_TextChanged(object sender, EventArgs e)
   {
      ValutaNameInUse = Fld_ShowInValutaLookUp;

      if(ValutaNameInUse == ZXC.ValutaNameEnum.EMPTY || ValutaNameInUse == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum) IsShowingConvertedMoney = false;
      else                                                                                                                  IsShowingConvertedMoney = true;

      if(IsShowingConvertedMoney == true) // idemo iz kuna u neku drugu valutu 
      {
         DevTecaj = ZXC.DevTecDao.GetHnbTecaj(ValutaNameInUse, Fld_DokDate);

         if(ZXC.RISK_DisableCacheTemporarily) DevTecaj = 1.00M / ZXC.DevTecDao.GetHnbTecaj(ValutaNameInUse, Fld_DokDate);
      }

      PutFields(faktur_rec, false);

      TheVvTabPage.TheVvForm.ToggleDevizaVisualApperiance(IsShowingConvertedMoney, ValutaNameInUse.ToString(), Fld_DevTecaj);
   }


   internal decimal VvCurrency(decimal _money)
   {
      if(ValutaNameInUse         == ZXC.ValutaNameEnum.EMPTY ||
         ValutaNameInUse         == ZXC.EURorHRK_NameEnum    ||
         IsShowingConvertedMoney == false                     )
      {
         return _money;
      }  
      else if(TheVvTabPage.WriteMode != ZXC.WriteMode.None)
      {
         return _money;
      }
      else
      {
         if(IsShowingConvertedMoney) // daj kune u devizu 
         {
            return ZXC.DivSafe(_money, DevTecaj);
         }
         else // daj devizu u kune
         {
            return (_money * DevTecaj);
         }
      }
   }

   #endregion VvCurrency converter

   #region IsOkToInitiateThisAction (Da li je dok vec proknjizen u Financijsko)

   public int NumOf_T_cij_decimalPlaces { get { return vvtbT_cij == null ? -1 :  vvtbT_cij.JAM_NumberOfDecimalPlaces; } }

   public override bool IsOkToInitiateThisAction(ZXC.WriteMode writeMode)
   {
      switch(writeMode)
      {
         case ZXC.WriteMode.Add:///////////////////////////////////////////////////////////////////////////////////////////////// 
            if(IsShowingConvertedMoney)
            {
               TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
               return true;
            }
            else return true;

         case ZXC.WriteMode.Edit: ///////////////////////////////////////////////////////////////////////////////////////////////// 

          //if(faktur_rec.Is_F2_AlreadySent                                )
            if(faktur_rec.Is_F2_AlreadySent && faktur_rec.F2_StatusCD != 50)
            {
               if(ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipA ||
                  ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB ||
                  ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipC  )
               {
                  //ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje.\n\nDokument je već poslan kao eRačun.");
               }
               else // vlastito knjig. 
               {
                  ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljen ispravak.\n\nDokument je već poslan kao eRačun.");
                  return false;
               }
            }

          //if(ZXC.IsSvDUH_ZAHonly && faktur_rec.TT == Faktur.TT_ZAH && faktur_rec.StatusCD != "O")
            if(ZXC.IsSvDUH_ZAHonly && faktur_rec.TT == Faktur.TT_ZAH && faktur_rec.StatusCD != "O" && faktur_rec.StatusCD != "P")
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena akcija.\n\nZahtjevnica je već poslana u apoteku.\n\n.");
               return false;
            }


            // 05.04.2016: need for numberOfDecimalPlaces discrepancy (devizni vs nonDevizni DUC) start 

            int thisDUC_MaxDecimalPlaces              = /*vvtbT_cij.JAM_NumberOfDecimalPlaces*/ NumOf_T_cij_decimalPlaces;
            int thisFakturMaxSignificantDecimalPlaces = Faktur.GetMaxCountOfSignificantDecimalPlaces(this.faktur_rec);



            // 13.04.2016: !!! privremeno suspendirano jer u fajlu je 8 dacimalnih ___ start 
          //if(thisFakturMaxSignificantDecimalPlaces > thisDUC_MaxDecimalPlaces)
          //{
          //   ZXC.aim_emsg(MessageBoxIcon.Error, "Premali broj decimalnih mjesta za cijenu.\n\nIspravite dokument na orginalnom ekranu.\n\nOvaj dokument\tMAX decimal. mj.: [{0}]\nvs\nOvaj ekran\tMAX decimal. mj.: [{1}]",
          //      thisFakturMaxSignificantDecimalPlaces, thisDUC_MaxDecimalPlaces);
          //   return false;
          //}
            // 13.04.2016: !!! privremeno suspendirano jer u fajlu je 8 dacimalnih ___ end 



            // 05.04.2016: need for numberOfDecimalPlaces discrepancy (devizni vs nonDevizni DUC) end 



            if(FakturOpeningExistsInFtrans(faktur_rec.TT, faktur_rec./*TipBr*/TT_And_TtNum) == true) 
            {
               if(ZXC.CURR_userName == ZXC.vvDB_systemSuperUserName ||
                  ZXC.CURR_userName == ZXC.vvDB_programSuperUserName/* || ZXC.CURR_user_rec.IsSuper*/)
               {
                  IssueMessageWarning();            return true;
               }
               else
               {
                  IssueUnauthorizedActionMessage(); return false;
               }
            }
            else {                                  return true ; }

         case ZXC.WriteMode.Delete: ///////////////////////////////////////////////////////////////////////////////////////////////// 

            if(faktur_rec.Is_F2_AlreadySent && ZXC.CURR_prjkt_rec.F2_RolaKind != ZXC.F2_RolaKind.KlijentServisa_TipA && ZXC.CURR_prjkt_rec.F2_RolaKind != ZXC.F2_RolaKind.KlijentServisa_TipC)
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljeno brisanje.\n\nDokument je već poslan kao eRačun.");
               return false;
            }

            if(FakturOpeningExistsInFtrans(faktur_rec.TT, faktur_rec.TipBr) == true) 
            { 
               IssueUnauthorizedActionMessage(); 
               return false; 
            }
            else if(faktur_rec.TtInfo.IsExtendableTT && faktur_rec.IsAlreadyFiskalized)
            {
#if(DEBUG) // >__________________________________________________________________________________________________________________________________________< 
               return true;
#else //                                                                                                                                                   
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena akcija.\n\nDokument je fiskaliziran.\n\nPreostaje opcija 'STORNO'.");
               return false;
#endif // >_______________________________________________________________________________________________________________________________________________<
            }
            else if(faktur_rec.TtInfo.IsIzlazniPdvTT && !faktur_rec.TtInfo.Is_WYRN_TT && faktur_rec.TtNum != TheVvDao.GetLastTtNum(TheDbConnection, faktur_rec.TT, faktur_rec.SkladCD)) // well, dozvoliti ce se brisanje samo zadnje IRA, IRM, ...
            {
#if(DEBUG) // >__________________________________________________________________________________________________________________________________________<
               return true;
#else//                                                                                                                                                   
               ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena akcija.\n\nOvo je izlazni račun koji nije zadnji, te će prouzrokovati rupu u brojevima.\n\nPreostaje opcija 'STORNO'.");
               return false;
#endif // >______________________________________________________________________________________________________________________________________________<
            }
            else
            {                                   
               return true; 
            }


         default: return true;
      }
   }

   private void IssueUnauthorizedActionMessage()
   {
      ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena akcija.\n\nDokument je već proknjižen u financijsko knjigovodstvo.\n\nDetalje o knjiženjima možete vidjeti na tab-u 'SaldaKonti'\nkraj tab-a 'Zoom' na ovome ekranu.\n\nPreostaje opcija 'STORNO'.");
   }

   private void IssueMessageWarning()
   {
      ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE!\n\nDokument je već proknjižen u financijsko knjigovodstvo.\n\nDetalje o knjiženjima možete vidjeti na tab-u 'SaldaKonti'\nkraj tab-a 'Zoom' na ovome ekranu.");
   }

   // puse: 
   //private bool FakturExistsInFtrans(uint fakturRecID)
   //{
   //   List<VvSqlFilterMember> filterMembers =  new List<VvSqlFilterMember>(1);

   //   filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_fakRecID], false, "", fakturRecID, "", "", " = ", ""));

   //   int? count = VvDaoBase.CountRecords(TheDbConnection, filterMembers);

   //   if(count == null) return false;

   //   return count.Value.IsPositive();
   //}

   private bool FakturOpeningExistsInFtrans(/*uint fakturRecID*/ string _tt, string _tipBr)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

    //filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_fakRecID], false, "", fakturRecID, "", "", " = ", ""));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tipBr], "elTipBr",   _tipBr                   , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tt   ], "elNalogTT", GetNalogTT_ForRiskTT(_tt), " = "));

      int? count = VvDaoBase.CountRecords(TheDbConnection, filterMembers);

      if(count == null) return false;

      return count.Value.IsPositive();
   }

   public string GetNalogTT_ForRiskTT(string riskTT)
   {
      Faktur2NalogRulesAndData theRules = new Faktur2NalogRulesAndData();
      theRules.Fak2nalSet = ZXC.Faktur2NalogSetEnum.OneExactTT;
      theRules.ThisTT_Only = riskTT;
      theRules.SetNalogTT(riskTT);

      return theRules.NalogTT;
   }

   internal static bool IsIn_TtVsSkladCD_Problem(string _tt, string _skladCD)
   {
      TtInfo ttInfo = ZXC.TtInfo(_tt);

      if(ttInfo.IsKorekTemTT) return false;

      bool isMalopSklad = ZXC.luiListaSkladista.GetFlagForThisCd(_skladCD);

      // 29.10.2014: komentirao '&& ttInfo.IsTwinTT == false' jer ne radi dobro kod TEXTHO 
      if(((ttInfo.IsMalopTT /*&& ttInfo.IsTwinTT == false*/) == true  && isMalopSklad == false) ||
         ((ttInfo.IsMalopTT /*&& ttInfo.IsTwinTT == false*/) == false && isMalopSklad == true))
      {
         // 25.11.2016: izuzatak za blagTT 
         if(ttInfo.IsBlagajnaTT) return false;

         return true;
      }

      return false;
   }

   #endregion IsOkToInitiateThisAction (Da li je dok vec proknjizen u Financijsko)

   #region Open ArtiklUC or KupdobUC

   private void TheG_CellMouseDoubleClick_OpenArtiklUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex != ci.iT_artiklCD &&
         e.ColumnIndex != ci.iT_artiklName) return;

      string artiklCD   = TheG.GetStringCell(ci.iT_artiklCD  , rowIdx, false);
      // 19.12.2014: 
      if(artiklCD.IsEmpty()) return; // znaci da smo u zutome probali doubleclickom inicirati editiranje cell-a 

      //Artikl artikl_rec = new Artikl();
      //artikl_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, artikl_rec, artiklCD, ZXC.ArtiklSchemaRows[ZXC.ArtCI.artiklCD], false);

      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

    // 18.08.2011: bijo BUG! Kako je reference type, saljuci element ArtiklSifrar-a kao TheVvDataRecord, svaka promkena TheVvDataRecord-a je mijenjala i element ArtiklSifrar-a! 
    //Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD);

      Artikl artikl_rec;
      try
      {
         artikl_rec = ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artiklCD).MakeDeepCopy();
      }
      catch(Exception ex)
      {
         artikl_rec = null;
      }

      if(artikl_rec != null)
      {
         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.ART), artikl_rec);
      }
   }

   private void RealizacGrid_CellMouseDoubleClick_OpenFakturDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      uint selectedRecID = 0;
      string selectedTT = "";

      if(RealizacGrid.CurrentRow != null) selectedTT    = RealizacGrid.CurrentRow.Cells["TT"].Value.ToString();
      if(RealizacGrid.CurrentRow != null) selectedRecID = ZXC.ValOrZero_UInt(RealizacGrid.CurrentRow.Cells[0].Value.ToString());

      if(selectedRecID.IsZero() || selectedTT.IsEmpty()) return;

      ZXC.VvSubModulEnum vvSubModulEnum = TheVvTabPage.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(selectedTT);
      Point xy = TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum);
      TheVvTabPage.TheVvForm.OpenNew_Record_TabPage(xy, (uint?)selectedRecID);
   }

   private void FakLink_Grid_CellMouseDoubleClick_OpenFakturDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      uint selectedRecID = 0;
      string selectedTT = "";

      if(fakLinkGrid.CurrentRow != null) selectedTT    = fakLinkGrid.CurrentRow.Cells["TT"].Value.ToString();
      if(fakLinkGrid.CurrentRow != null) selectedRecID = ZXC.ValOrZero_UInt(fakLinkGrid.CurrentRow.Cells[0].Value.ToString());

      if(selectedRecID.IsZero() || selectedTT.IsEmpty()) return;

      ZXC.VvSubModulEnum vvSubModulEnum = TheVvTabPage.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(selectedTT);
      Point xy = TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum);
      TheVvTabPage.TheVvForm.OpenNew_Record_TabPage(xy, (uint?)selectedRecID);
   }

   protected void AnyKupdobTextBox_DoubleClick(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) return;

      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

    //Kupdob artikl_rec = KupdobSifrar.SingleOrDefault(a => a.KupdobCD == (this as FakturExtDUC).Fld_KupdobCd);
      Kupdob kupdobFromSifrar_rec = KupdobSifrar.SingleOrDefault(k => k.KupdobCD == (this as FakturExtDUC).Fld_KupdobCd);
      Kupdob kupdob_rec;

      if(kupdobFromSifrar_rec == null) return;

      kupdob_rec = kupdobFromSifrar_rec.MakeDeepCopy();

      Point xy = TheVvTabPage.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.KID);

      // 13.11.2014: 
      if(xy.IsEmpty) return;

      TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(xy, kupdob_rec);
   }

   #endregion Open ArtiklUC or KupdobUC

   #region PrnFakDsc Utils

   public static VvLookUpLista GetDscLuiListForThisTT(string theTT, ushort subDsc)
   {
      switch(theTT)
      {
         case Faktur.TT_IRA: 
            switch(subDsc)
            {
               case 0:  case 1: return ZXC.dscLuiLst_IRA_1;
               case 2:          return ZXC.dscLuiLst_IRA_2;
               case 3:          return ZXC.dscLuiLst_IRA_3;
               case 4:          return ZXC.dscLuiLst_IRA_4;
               case 5:          return ZXC.dscLuiLst_IRA_5; //15.06.2020.
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }
         case Faktur.TT_IFA:
            switch(subDsc)
            {
               case 0: case 1: return ZXC.dscLuiLst_IFA;
               case 2:         return ZXC.dscLuiLst_IFA_2;
               case 3:         return ZXC.dscLuiLst_IFA_3;
               case 4:         return ZXC.dscLuiLst_IFA_4;
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }
            
         case Faktur.TT_IRM:
            switch(subDsc)
            {
               case 0: case 1: return ZXC.dscLuiLst_IRM;
               case 2:         return ZXC.dscLuiLst_IRM_2;
               case 3:         return ZXC.dscLuiLst_IRM_3;
               case 4:         return ZXC.dscLuiLst_IRM_4;
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }

         case Faktur.TT_PON   : 
         case Faktur.TT_OPN   :
            switch(subDsc)
            {
               case 0:
               case 1: return ZXC.dscLuiLst_PON;
               case 2: return ZXC.dscLuiLst_PON_2;
               case 3: return ZXC.dscLuiLst_PON_3;
               case 4: return ZXC.dscLuiLst_PON_4;
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }

         case Faktur.TT_PNM   :
            switch(subDsc)
            {
               case 0:
               case 1: return ZXC.dscLuiLst_PNM;
               case 2: return ZXC.dscLuiLst_PNM_2;
               case 3: return ZXC.dscLuiLst_PNM_3;
               case 4: return ZXC.dscLuiLst_PNM_4;
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }

         case Faktur.TT_IZM:
            switch(subDsc)
            {
               case 0: case 1: return ZXC.dscLuiLst_IZM;
               case 2:         return ZXC.dscLuiLst_IZM_2;
               default: ZXC.aim_emsg("FakturDUC_Q.GetDscLuiListForThisName(): LookUpListName (" + theTT + ", subDSC: " + subDsc + ") still undone!"); return null;
            }

         case Faktur.TT_URA   : return ZXC.dscLuiLst_URA;
         case Faktur.TT_UFA   : return ZXC.dscLuiLst_UFA;
         case Faktur.TT_UPA   : return ZXC.dscLuiLst_UFA; // upaTODO: !!!!!! 
         case Faktur.TT_UFM   : return ZXC.dscLuiLst_UFM;
         case Faktur.TT_PRI   : return ZXC.dscLuiLst_PRI;
         case Faktur.TT_POU   : return ZXC.dscLuiLst_PRI;
         case Faktur.TT_KLK   : return ZXC.dscLuiLst_KLK;
         case Faktur.TT_KKM   : return ZXC.dscLuiLst_KKM;
         case Faktur.TT_POT   : return ZXC.dscLuiLst_URA; // zato jer treba pfd radi deviznog
         case Faktur.TT_ZAR   : return ZXC.dscLuiLst_URA; //TamaraZAR ovo bas nisam sigurna!!!! zato jer treba pfd radi deviznog

         case Faktur.TT_IZD   : return ZXC.dscLuiLst_IZD;
         case Faktur.TT_POI   : return ZXC.dscLuiLst_IZD;
         case Faktur.TT_UOD   : return ZXC.dscLuiLst_UOD;
         case Faktur.TT_UPV   : return ZXC.dscLuiLst_UPV;
         case Faktur.TT_UPM   : return ZXC.dscLuiLst_UPM;
         case Faktur.TT_IOD   : return ZXC.dscLuiLst_IOD;
         case Faktur.TT_IPV   : return ZXC.dscLuiLst_IPV;
         case Faktur.TT_NRD   : return ZXC.dscLuiLst_NRD;
         case Faktur.TT_NRM   : return ZXC.dscLuiLst_NRM;
         case Faktur.TT_NRU   : return ZXC.dscLuiLst_NRU;
         case Faktur.TT_NRS   : return ZXC.dscLuiLst_NRS;
         case Faktur.TT_NRK   : return ZXC.dscLuiLst_NRK;
//       case Faktur.TT_STU   : return ZXC.dscLuiLst_STU;
//       case Faktur.TT_STI   : return ZXC.dscLuiLst_STI;
         case Faktur.TT_RVI   : return ZXC.dscLuiLst_RVI;
         case Faktur.TT_RVU   : return ZXC.dscLuiLst_RVU;
         case Faktur.TT_UPL   : return ZXC.dscLuiLst_UPL;
         case Faktur.TT_ISP   : return ZXC.dscLuiLst_ISP;
         case Faktur.TT_BUP   : return ZXC.dscLuiLst_BUP;
         case Faktur.TT_BIS   : return ZXC.dscLuiLst_BIS;
         case Faktur.TT_ABU   : return ZXC.dscLuiLst_BUP;
         case Faktur.TT_ABI   : return ZXC.dscLuiLst_BIS;
         case Faktur.TT_PST   : return ZXC.dscLuiLst_PST;
         case Faktur.TT_INV   : return ZXC.dscLuiLst_INV;
         case Faktur.TT_INM   : return ZXC.dscLuiLst_INM;
         case Faktur.TT_PPR   : return ZXC.dscLuiLst_PPR;
         case Faktur.TT_PIP   : return ZXC.dscLuiLst_PIP;
         case Faktur.TT_POV   : return ZXC.dscLuiLst_POV;
         case Faktur.TT_MSI   : return ZXC.dscLuiLst_MSI;
         case Faktur.TT_KIZ   : return ZXC.dscLuiLst_KIZ;
         case Faktur.TT_PIK   : return ZXC.dscLuiLst_PIK;
         
         case Faktur.TT_CJ_VP1: 
         case Faktur.TT_CJ_VP2: 
         case Faktur.TT_CJ_DE : 
         case Faktur.TT_CJ_MK : 
         case Faktur.TT_CJ_MP : 
         case Faktur.TT_CJ_MRZ: 
         case Faktur.TT_CJ_RB1: 
         case Faktur.TT_CJ_RB2: return ZXC.dscLuiLst_CJE;

         default: return null;
            //if(ZXC.TtInfo(theTT).IsExtendableTT)            return ZXC.dscLuiLst_XYZ;
            //else                                            return ZXC.dscLuiLst_XYZ;
      }

   }

   public void LoadDscLuiList_And_PutFilterFields(ushort subDsc)
   {
      FakturDocFilter theRptFilter = VirtualRptFilter as FakturDocFilter;

      VvLookUpLista theDscLuiList = GetDscLuiListForThisTT(faktur_rec.TT, subDsc/*theRptFilter.ChoseObrazac*/);

      if(theDscLuiList == null || VirtualFilterUC == null) return;

      theRptFilter.PFD           = new PrnFakDsc(theDscLuiList);
      theRptFilter.ChosenObrazac = subDsc;

      VirtualFilterUC.PutFilterFields(theRptFilter);
   }

   //06.10.2015: 
   internal List<Ftrans> TheTheFtransList;
   //29.10.2015: 
   internal decimal OTS_saldo { get { return TheTheFtransList == null ? 0M : TheTheFtransList.Sum(ftr => ftr.R_DugMinusPot); } }

   #endregion PrnFakDsc Utils

   #region T_serlot

   public void OnSerlotEnter_FillDataSource(object sender, EventArgs e)
   {
      // /* temp. For deploy: */ return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;
      
    // 04.02.2021. dodajemo novi RRD koji vrijedi za PIZ i za izlaze - pratimo samo izlaz proizvoda 
    //if(ZXC.RRD.Dsc_IsSerlotVisible == false                                            ) return;
    //if(ZXC.RRD.Dsc_IsSerlotVisible == false && ZXC.RRD.Dsc_IsVisibleLotOnIzlaz == false) return;
      if(ZXC.RRD.Is_Serlot_Active == false) return;
      
      DataGridView dgv = null;
      
      if(sender is VvTextBoxEditingControl)
      {
         VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
         dgv = vtbec.EditingControlDataGridView;
      
       //ZXC.aim_emsg("row: <{0}> col: <{1}> row: <{2}> col: <{3}>",
       //   TheG.CurrentCellAddress.Y + 1, TheG.CurrentCellAddress.X + 1,
       //    dgv.CurrentCellAddress.Y + 1,  dgv.CurrentCellAddress.X + 1);
      }
      else return;
      
      int rIdx = dgv.CurrentRow.Index;
      
      string artiklCD = TheG.GetStringCell(ci.iT_artiklCD, rIdx, true);

      // clear 
      ZXC.luiListaSerlot.Clear(); vvtbT_serlot.JAM_Set_LookUpTable(ZXC.luiListaSerlot, (int)ZXC.Kolona.prva);

      if(artiklCD.IsEmpty() || Fld_SkladCD.IsEmpty()) return;
      
      List<ZXC.VvUtilDataPackage> availableSerlots = RtransDao.GetFreeSerlotList_ForArtikl(TheDbConnection, artiklCD, Fld_SkladCD, Fld_DokDate);
      
      #region Ako zelimo via AutoComplete

      //var strings = availableSerlots.Select(s => "RGC-" + s.TheName + " : " + s.TheDecimal.ToStringVv() + "kg");
      
      //string[] stringArray = strings.ToArray();
      //
      //vvtbT_serlot.AutoCompleteCustomSource.Clear();
      //vvtbT_serlot.AutoCompleteCustomSource.AddRange(stringArray);
      
      #endregion Ako zelimo via AutoComplete
      
      ZXC.luiListaSerlot.Clear();
      
      availableSerlots.ForEach(sl => ZXC.luiListaSerlot.Add(new VvLookUpItem(sl.TheStr1, sl.TheStr2, sl.TheDecimal, false, 0, sl.TheDate, 0, "")));

      vvtbT_serlot.JAM_Set_LookUpTable(ZXC.luiListaSerlot, (int)ZXC.Kolona.prva);

   }

   //public void OnSerlotExit_ClearSuffixAndPreffix(object sender, EventArgs e)
   //{
   //   if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

   //   VvTextBoxEditingControl vvTbSerlot = sender as VvTextBoxEditingControl;

   //   string dirtyString = vvTbSerlot.Text, cleanString;

   //   int spaceIdx = dirtyString.IndexOf(' ');

   //   if(dirtyString.Length.IsZero() || spaceIdx.IsNegative()) return;

   // //cleanString = dirtyString.SubstringSafe(0, spaceIdx);
   //   cleanString = "fakju";

   //   //vvTbKonto.Text = cleanString;
   //   //TheG.EditingControl.Text = cleanString;
   //   TheG.PutCell(ci.iT_serlot, vvTbSerlot.EditingControlDataGridView.CurrentRow.Index, cleanString);
   //}

   #endregion T_serlot

   #region Veza Has ...

   public uint VezaTtNumForTT(string theTT)
   {
      if(faktur_rec.V1_tt == theTT) return faktur_rec.V1_ttNum;
      if(faktur_rec.V2_tt == theTT) return faktur_rec.V2_ttNum;
      if(faktur_rec.V3_tt == theTT) return faktur_rec.V3_ttNum;
      if(faktur_rec.V4_tt == theTT) return faktur_rec.V4_ttNum;

      return 0;
   }

   #endregion Veza Has ...

   #region ORG BOP COP + SVD Grid_CellMouseDoubleClick

   public virtual bool HasOrgBopCop { get { return false; } }

   public void OnExitT_ORG_BOP_COP_Calc_Kol_Cij(object sender, EventArgs e)
   {
      if(this.HasOrgBopCop == false) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      int currRow = TheG.CurrentRow.Index;

      decimal org = TheG.GetDecimalCell(ci.iT_doCijMal, currRow, false);
      decimal bop = TheG.GetDecimalCell(ci.iT_bop     , currRow, false);
      decimal cop = TheG.GetDecimalCell(ci.iT_cop     , currRow, false);

      decimal R_kol =             bop * org ;
      decimal R_cij = ZXC.DivSafe(cop , org);

      TheG.PutCell(ci.iT_kol, currRow, R_kol);
      TheG.PutCell(ci.iT_cij, currRow, R_cij);
   }

   private void URA_NRD_SVD_Grid_CellMouseDoubleClick_Open_UGO_DUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      if((this is URA_SVD_DUC) == false && this is NRD_SVD_DUC == false) return;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex != ci.iT_utilUint) return;

      uint UGO_TtNum = TheG.GetUint32Cell(ci.iT_utilUint, rowIdx, false);

      if(UGO_TtNum.IsZero()) return;

      Faktur faktur_rec = new Faktur();

      bool fakturOK = ZXC.FakturDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, faktur_rec, Faktur.TT_UGO, UGO_TtNum, false, false);

      if(fakturOK)
      {
         ZXC.VvSubModulEnum vvSubModulEnum = FakturDUC.GetVvSubModulEnum_ForTT(Faktur.TT_UGO);

         if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum), faktur_rec);
      }
   }

   private void ZAH_SVD_Grid_CellMouseDoubleClick_Open_IZD_SVD_DUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      if((this is ZAH_SVD_DUC) == false) return;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex != ci.iT_utilUint) return;

      uint IZD_TtNum = TheG.GetUint32Cell(ci.iT_utilUint, rowIdx, false);

      if(IZD_TtNum.IsZero()) return;

      Faktur faktur_rec = new Faktur();

      bool fakturOK = ZXC.FakturDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, faktur_rec, Faktur.TT_IZD, IZD_TtNum, false, false);

      if(fakturOK)
      {
         ZXC.VvSubModulEnum vvSubModulEnum = FakturDUC.GetVvSubModulEnum_ForTT(Faktur.TT_IZD);

         if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

         TheVvTabPage.TheVvForm.OpenNew_Record_TabPage_wInitialRecord(TheVvTabPage.TheVvForm.GetSubModulXY(vvSubModulEnum), faktur_rec);
      }
   }

   #endregion ORG BOP COP + SVD Grid_CellMouseDoubleClick

   #region Some PTG virtual bools
   public virtual bool HasRtrans_SkladCD_Exposed     { get { return false; } }
   public virtual bool HasRtrano_SkladCD_Exposed     { get { return false; } }
   public virtual bool HasRtrano_TT_Exposed          { get { return false; } }
   public virtual bool IsRtransTT_MOD_kindDependable { get { return false; } }
   /// <summary>
   /// DUC se popunjava 's desna na lijevo', nema <F3>, MOD i PVR
   /// </summary>
   public virtual bool Is_Rtrans_Readonly            { get { return false; } }

   public uint UGAN_ttNum_ofThisDUC
   {
      get { return this.faktur_rec.V1_ttNum * ZXC.Base10TtNumBuffer(5) + this.faktur_rec.V2_ttNum; }
   }

   #endregion Some PTG virtual bools

   #region Implementation of Events.Required, Events.Statu (M2PAY stuff)

#if NOTJETT
   public void EndOfTransaction(TransactionResult transactionResult, Device device)
   {
      //Window.DisplayReceipts(transactionResult.MerchantReceipt, transactionResult.CustomerReceipt);
   }

   // ConnectionStatusChanged will get called if the connections status changes
   public void ConnectionStatusChanged(ConnectionStatus connectionStatus, Device device)
   {
      //String status = device.Name + " is " + connectionStatus.ToString();
      //System.Diagnostics.Debug.WriteLine(status);
      //Window.DisplayStatus(status);
   }

   // CurrentTransactionStatus notifies about changes to the current transaction status
   public void CurrentTransactionStatus(StatusInfo statusInfo, Device device)
   {
      //String status = statusInfo.Message;
      //System.Diagnostics.Debug.WriteLine(status);
      //Window.DisplayStatus(status);
   }

   public void DeviceDiscoveryFinished(List<Device> devices)
   {
      // Only needed when using a payment terminal, here you get a list of Bluetooth payment terminals paired with your computer
      // You can also get a list of serial / USB payment terminals attached to your computer

   }

   public void SignatureRequired(SignatureRequest signatureRequest, Device device)
   {
      // You'll be notified here if a sale process needs a signature verification
      // A signature verification is only needed if the cardholder uses a magnetic stripe card or a chip and signature card for the payment
      // This method will not be invoked if a transaction is made with a Chip & Pin card

      // remarck byQ:
      //api.SignatureResult(true); // This line means that the cardholder ALWAYS accepts to sign the receipt.
                                 // A specific line will be displayed on the merchant receipt for the cardholder to be able to sign it
   }
#endif
   #endregion Implementation of Events.Required, Events.Statu (M2PAY stuff)

}

public partial class FakturExtDUC : FakturDUC
{
   public static List<VvMigrator> GetMigratorList(string theTT)
   {
      switch(theTT)
      {
         case Faktur.TT_IFA: return ZXC.TheVvForm.VvPref.fakturIRaDUC      .MigratorStates;
         case Faktur.TT_IRA: return ZXC.TheVvForm.VvPref.fakturIRbDUC      .MigratorStates;
         case Faktur.TT_UFA: return ZXC.TheVvForm.VvPref.fakturURaDUC      .MigratorStates;
         case Faktur.TT_UPA: return ZXC.TheVvForm.VvPref.fakturUPaDUC      .MigratorStates;
         case Faktur.TT_UFM: return ZXC.TheVvForm.VvPref.fakturUFMDUC      .MigratorStates;
         case Faktur.TT_URA: return ZXC.TheVvForm.VvPref.fakturURbDUC      .MigratorStates;
         case Faktur.TT_URM: return ZXC.TheVvForm.VvPref.fakturURmDUC      .MigratorStates;
         case Faktur.TT_PRI: return ZXC.TheVvForm.VvPref.fakturPrimkaDUC   .MigratorStates;
         case Faktur.TT_POU: return ZXC.TheVvForm.VvPref.fakturPrimkaDUC   .MigratorStates;
         case Faktur.TT_KLK: return ZXC.TheVvForm.VvPref.fakturKalkDUC     .MigratorStates;
         case Faktur.TT_KKM: return ZXC.TheVvForm.VvPref.fakturKKMDUC      .MigratorStates;
         case Faktur.TT_IZD: return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates;
         case Faktur.TT_POI: return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates;
         case Faktur.TT_IRM: return ZXC.TheVvForm.VvPref.fakturIRMDUC      .MigratorStates;
         case Faktur.TT_IOD: return ZXC.TheVvForm.VvPref.fakturOdobrKupDUC .MigratorStates;
         case Faktur.TT_IPV: return ZXC.TheVvForm.VvPref.fakturPovKupDUC   .MigratorStates;
         case Faktur.TT_UOD: return ZXC.TheVvForm.VvPref.fakturOdobrDobDUC .MigratorStates;
         case Faktur.TT_UPV: return ZXC.TheVvForm.VvPref.fakturPovDobDUC   .MigratorStates;
         case Faktur.TT_UPM: return ZXC.TheVvForm.VvPref.fakturPovDobMalDUC.MigratorStates; //koji ca nam ovo k? 
         case Faktur.TT_OPN: return ZXC.TheVvForm.VvPref.fakturObavPonDUC  .MigratorStates;
         case Faktur.TT_PON: return ZXC.TheVvForm.VvPref.fakturPonudaDUC   .MigratorStates;
         case Faktur.TT_PNM: return ZXC.TheVvForm.VvPref.fakturPonMalDUC   .MigratorStates;
         case Faktur.TT_NRD: return ZXC.TheVvForm.VvPref.fakturNarDobDUC   .MigratorStates;
         case Faktur.TT_NRM: return ZXC.TheVvForm.VvPref.fakturNRMDUC      .MigratorStates;
         case Faktur.TT_NRU: return ZXC.TheVvForm.VvPref.fakturNarDobUvDUC .MigratorStates;
         case Faktur.TT_NRS: return ZXC.TheVvForm.VvPref.fakturNarDobUslDUC.MigratorStates;
         case Faktur.TT_NRK: return ZXC.TheVvForm.VvPref.fakturNarKupDUC   .MigratorStates;
         case Faktur.TT_RVI: return ZXC.TheVvForm.VvPref.fakturReversDUC   .MigratorStates;
         case Faktur.TT_RVU: return ZXC.TheVvForm.VvPref.fakturPovRevDUC   .MigratorStates;
         case Faktur.TT_UPL: return ZXC.TheVvForm.VvPref.fakturBlgUplDUC   .MigratorStates;
         case Faktur.TT_ISP: return ZXC.TheVvForm.VvPref.fakturBlgIspDUC   .MigratorStates;
         case Faktur.TT_BUP: return ZXC.TheVvForm.VvPref.fakturBlgUplMDUC  .MigratorStates;
         case Faktur.TT_BIS: return ZXC.TheVvForm.VvPref.fakturBlgIspMDUC  .MigratorStates;
         case Faktur.TT_ABU: return ZXC.TheVvForm.VvPref.fakturBlgUplMDUC  .MigratorStates;
         case Faktur.TT_ABI: return ZXC.TheVvForm.VvPref.fakturBlgIspMDUC  .MigratorStates;
         case Faktur.TT_RNP: return ZXC.TheVvForm.VvPref.fakturRNpDUC      .MigratorStates;
         case Faktur.TT_RNM: return ZXC.TheVvForm.VvPref.fakturRNmDUC      .MigratorStates;
         case Faktur.TT_RNS: return ZXC.TheVvForm.VvPref.fakturRNsDUC      .MigratorStates;
         case Faktur.TT_RNZ: return ZXC.TheVvForm.VvPref.fakturRNzDUC      .MigratorStates;
         case Faktur.TT_PRJ: return ZXC.TheVvForm.VvPref.fakturPRjDUC      .MigratorStates;
         case Faktur.TT_KIZ: return ZXC.TheVvForm.VvPref.fakturKIZDUC      .MigratorStates;
         case Faktur.TT_CKP: return ZXC.TheVvForm.VvPref.fakturCjKupcaDUC  .MigratorStates;

         default: throw new Exception("Za TT " + theTT + " nedefiniran MigratorList u FakturExtDUC.GetMigratorList()");
      }
   }

   #region PutAllKupdobFields

   /*protected*/ public void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text == this.originalText) return;

      //----------------------------------------------------

      this.originalText = tb.Text;

      // 8.6.2011: SetSifrarAndAutocomplete<Artikl> zjebe kupdobSifrar u PutDgvFields (Artikl treba za TS na gridu)
      if(ZXC.RISK_CopyToOtherDUC_inProgress      ) OnVvTBEnter_SetAutocmplt_Kupdob_sorterName(tbx_KupdobName, EventArgs.Empty);

      kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

      ClearAllKupdobFields();

      if(kupdob_rec != null)
      {
         PutAllKupdobFields(kupdob_rec);
      }

      // 27.12.2013: 
      if(ZXC.GOST_SOBA_BOR_SetOtherData_InProgress == false)
      {
         OnExit_GOST_SOBA_BOR_SetOtherData(sender, e);
      }

      #region Check Kupdob OTS

    //if(kupdob_rec != null && faktur_rec.TT == Faktur.TT_IRA && (ZXC.VvDeploymentSite == ZXC.VektorSiteEnum.TEMBO || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO")))
      if(kupdob_rec != null && faktur_rec.TT == Faktur.TT_IRA && ZXC.RRD.Dsc_IsCheckOpenSaldo)
      {
         // 22.01.2015: 'OPREZ' & 'PAZITI' tembo 
         if(kupdob_rec.Napom1  .ToUpper().Contains("OPREZ") || kupdob_rec.Napom1  .ToUpper().Contains("PAZITI")) ZXC.aim_emsg(MessageBoxIcon.Warning, kupdob_rec.Napom1  );
         if(kupdob_rec.Napom2  .ToUpper().Contains("OPREZ") || kupdob_rec.Napom2  .ToUpper().Contains("PAZITI")) ZXC.aim_emsg(MessageBoxIcon.Warning, kupdob_rec.Napom2  );
         if(kupdob_rec.Komentar.ToUpper().Contains("OPREZ") || kupdob_rec.Komentar.ToUpper().Contains("PAZITI")) ZXC.aim_emsg(MessageBoxIcon.Warning, kupdob_rec.Komentar);

         // 04.12.2015: 'PRESTAO SA RADOM' tembo 
         if(kupdob_rec.Napom1  .ToUpper().Contains("PRESTAO") || 
            kupdob_rec.Napom2  .ToUpper().Contains("PRESTAO") || 
            kupdob_rec.Komentar.ToUpper().Contains("PRESTAO"))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Partner je oznacen kao 'Prestao sa radom'!\n\nFakturiranje je onemogućeno.");
            ClearAllKupdobFields();
         }

         decimal saldoOTS;
         string theKonto = (this as FakturExtDUC).Fld_Konto;
         if(theKonto.IsEmpty()) { /*KtoShemaDsc KSD = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);*/ theKonto = ZXC.KSD.Dsc_RKto_Kupca; }

         List<Ftrans> theFtransList = NalogDao.GetOTS_FtransByTipBrSortedList(TheDbConnection, theKonto, kupdob_rec.KupdobCD, (this as FakturExtDUC).Fld_DokDate);

         saldoOTS = theFtransList.Sum(ftr => ftr.T_dug) - theFtransList.Sum(ftr => ftr.T_pot);
         bool isOtsDospOnly = true; // ! 

         if(saldoOTS.NotZero())
         {
            List<Ftrans> ftransesToRemoveList = null;

            if(isOtsDospOnly) ftransesToRemoveList = new List<Ftrans>();

            foreach(Ftrans ftrans_rec in theFtransList)
            {
               NalogDao.SetOtsInfo_IOS(ftrans_rec, ""/*kontraKontoSet*/, theFtransList, /*dateOTS*/faktur_rec.DokDate);

               // 26.4.2011: remarkirano jer kada je karticna kuca onda ufa ide na 1200, ... za sada nemozemo razlikovati korektno od greske, pa necemo javljati poruke upozorenja do daljnjega... 
               //CheckFtrans_Ots(ftrans_rec, isOtsKupaca);

               // Remove nedospjele 
               if(isOtsDospOnly && ftrans_rec.OtsZakas.IsNegative()) ftransesToRemoveList.Add(ftrans_rec);
            }

            if(isOtsDospOnly)
            {
               foreach(Ftrans ftransToRemove in ftransesToRemoveList)
               {
                  theFtransList.Remove(ftransToRemove);
               }
            }

            saldoOTS = theFtransList.Sum(ftr => ftr.T_dug) - theFtransList.Sum(ftr => ftr.T_pot);

            // 18.05.2020: aim_emsg opkoljen if()-om 
            if(saldoOTS.NotZero())
            {
               ZXC.aim_emsg(MessageBoxIcon.Information, "Trenutni dug partnera: {0}", saldoOTS.ToStringVv());
               tbx_DokDate.Visible = true; // tbx nestaje bez ovoga?! 
            }
         }

      } // if(kupdob_rec != null && faktur_rec.TT == Faktur.TT_IRA && ZXC.RRD.Dsc_IsCheckOpenSaldo) 

      #endregion Check Kupdob OTS

   }

   private void PutAllKupdobFields_From_ROOT_prjkt_rec()
   {
      PutAllKupdobFields(ZXC.ROOT_prjkt_rec);
   }

   /*private*/ internal void PutAllKupdobFields(Kupdob _kupdob_rec)
   {
      #region Classic & Poslovna Jedinica operations 

      if(_kupdob_rec.CentrID.NotZero()) // kupdob_rec je, znaci, Poslovna Jedinica! 
      {
         Kupdob kupdobCentrala_rec = KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == _kupdob_rec.CentrID);

         if(kupdobCentrala_rec == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nema CENTRALE (KCD: [{0}])!", _kupdob_rec.CentrID);
            
            PutCentralaKupdobFields(_kupdob_rec);
         }
         else
         {
            PutCentralaKupdobFields(kupdobCentrala_rec);
            PutPosJedinKupdobFields(_kupdob_rec);
         }
      } // artikl_rec je, znaci, Poslovna Jedinica! 
      else // Classic KupDob 
      {
         PutCentralaKupdobFields(_kupdob_rec);

         // 08.03.2013:
         if(ZXC.LoadPoprat_InProgress == false || _kupdob_rec.KupdobCD.IsNegative()) // ako je negative znaci da smo pozvani od ClearKupdobFields 
         {
            VvHamper.ClearFieldContents(hamp_posJedCd);

            PutPosJedinKupdobFields(_kupdob_rec);
         }
      }

      #endregion Classic & Poslovna Jedinica operations

      #region Putnik 06.11.2013.

      if(_kupdob_rec.PutnikID.NotZero()) faktur_rec.PersonCD   = Fld_PersonCD   = _kupdob_rec.PutnikID;
      if(_kupdob_rec.PutName.NotEmpty()) faktur_rec.PersonName = Fld_PersonName = _kupdob_rec.PutName;

      #endregion Putnik

      #region EU VAT Code Action

      if(ZXC.EU_VatCodes_woHR.Contains(_kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsUlazniPdvTT)
      {
         Fld_PdvKnjiga  = ZXC.PdvKnjigaEnum .NIJEDNA;
         Fld_PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
         Fld_PdvR12     = ZXC.PdvR12Enum    .R1;
      }

      if(ZXC.EU_VatCodes_woHR.Contains(_kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsIzlazniPdvTT)
      {
         Fld_PdvKnjiga  = ZXC.PdvKnjigaEnum .REDOVNA;
         Fld_PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
         Fld_PdvR12     = ZXC.PdvR12Enum    .R1;
      }

      #endregion EU VAT Code Action

      #region RNZ - radni nalog zastitara (ugovor i nacin sticenja)

      if(this is RNZDUC) // ugovor + nacin zastite 
      {
         Mixer  prevRNZmixer_rec       = new Mixer();
         bool   prevRNZmixer_rec_found = MixerDao.SetMeLast_MixerUgovor_ByKupdobCD(TheDbConnection, prevRNZmixer_rec, Fld_KupdobCd, true);
         if(prevRNZmixer_rec_found)
         {
            Fld_VezniDok2 = prevRNZmixer_rec.StrF_64;
            Fld_Fco       = prevRNZmixer_rec.StrH_32;
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Greška!\n\nZa partnera [{0}]\n\nnema ugovora.", _kupdob_rec.ToString());

            Fld_VezniDok2 = 
            Fld_Fco       = "";
            tbx_DokDate.Visible = true;
         }
      }

      #endregion RNZ - radni nalog zastitara (ugovor i nacin sticenja)

      #region KUPDOB NOT IN PDV 17.12.2019

      if(_kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.NOT_IN_PDV && faktur_rec.TtInfo.IsUlazniPdvTT)
      {
         Fld_PdvKnjiga = ZXC.PdvKnjigaEnum.NIJEDNA;
         Fld_PdvR12    = ZXC.PdvR12Enum   .NIJEDNO;
      }

      // 30.03.2021: poboljsavamo logiku Fld_PdvKnjiga
      // ALI ODUSTAJEMO 
      //if(_kupdob_rec.IsHRVATSKA && faktur_rec.TtInfo.IsUlazniPdvTT)
      //{
      //   //Fld_PdvGEOkind = ZXC.PdvGEOkindEnum.HR;
      //
      //   // Fld_PdvKnjiga: 
      //   if(_kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.PODUZECE_R1 ||
      //      _kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.POD_PO_NAPL ||
      //      _kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R1     ||
      //      _kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2      )
      //   {
      //      Fld_PdvKnjiga = ZXC.PdvKnjigaEnum.REDOVNA;
      //   }
      //
      //   // Fld_PdvR12: 
      //   if(_kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.PODUZECE_R1 ||
      //      _kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R1      )
      //   {
      //      Fld_PdvR12 = ZXC.PdvR12Enum.R1;
      //   }
      //   else if(_kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.POD_PO_NAPL ||
      //           _kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2      )
      //   {
      //      Fld_PdvR12 = ZXC.PdvR12Enum.R2;
      //   }
      //
      //}

      #endregion KUPDOB NOT IN PDV 17.12.2019

      //14.03.2024. za TG POT

      #region POT_DUC

      if(this is POT_DUC)
      {
         if(_kupdob_rec.Napom1.NotEmpty()) faktur_rec.VezniDok  = Fld_VezniDok  = _kupdob_rec.Napom1;
         if(_kupdob_rec.Tel1  .NotEmpty()) faktur_rec.VezniDok2 = Fld_VezniDok2 = _kupdob_rec.Tel1  ;
         if(_kupdob_rec.Email .NotEmpty()) faktur_rec.Fco       = Fld_Fco       = _kupdob_rec.Email ;
         if(_kupdob_rec.Ziro1 .NotEmpty()) faktur_rec.ZiroRn    = Fld_ZiroRn    = _kupdob_rec.Ziro1 ;
      }

      #endregion POT_DUC

      #region ZAR_DUC

      if(this is ZAR_DUC)
      {
         if(_kupdob_rec.Tel1  .NotEmpty()) faktur_rec.VezniDok2 = Fld_VezniDok2 = _kupdob_rec.Tel1  ;
         if(_kupdob_rec.Email .NotEmpty()) faktur_rec.Fco       = Fld_Fco       = _kupdob_rec.Email ;
      }

      #endregion ZAR_DUC


      #region PON_MPC_DUC, OPN_MPC_DUC, IZD_MPC_DUC

      //27.06.2024.
      if(ZXC.EU_VatCodes_woHR.Contains(_kupdob_rec.VatCntryCode) && (this is PON_MPC_DUC || this is OPN_MPC_DUC || this is IZD_MPC_DUC))
      {
         Fld_PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
      }

      #endregion PON_MPC_DUC, OPN_MPC_DUC, IZD_MPC_DUC

      #region R1kind

    //if(ZXC.IsF2_2026_rules && faktur_rec.TtInfo.IsIzlazniPdvTT) 
      if(ZXC.IsF2_2026_rules && Is_F012_OR_Ponuda_DUC           )
      {
         faktur_rec.F2_R1kind = Fld_F2_R1kind = _kupdob_rec.R1kind;

         TheVvTabPage.Fld_Col4 = faktur_rec.F012kind.ToString(); 
       //TheVvTabPage.Fld_Col5 = _kupdob_rec.R1kind .ToString(); obavlja se u SET od Fld_F2_R1kind

         //PutIdentityFields_7Col(faktur_rec, _kupdob_rec.R1kind);
      }

      #endregion R1kind

      #region KOMISIJA

      if((this is KIZDUC || this is PIKDUC || this is IRADUC || this is IRADUC_2) && _kupdob_rec.KupdobCD.IsPositive()) // ako nije positive znaci da smo pozvani od ClearKupdobFields 
      {
         if(_kupdob_rec.Komisija == ZXC.KomisijaKindEnum.NIJE)
         {
            if(this is KIZDUC || this is PIKDUC)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Partner [{0}]\n\nNIJE označen kao KOMISIJA u Adresaru Partnera!", _kupdob_rec.ToString());
            }
            tbx_DokDate.Visible = true;
            return; // !!! 
         }

         // ako smo dosli do ovdje, znaci, partner JE komisija
         VvLookUpItem theLui = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Cd == _kupdob_rec.Ticker);

         if(theLui == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći skladište [{0}] vezano na ovoga Partnera [{1}]!", _kupdob_rec.Ticker, _kupdob_rec.ToString());
            tbx_DokDate.Visible = true;
            return;
         }

         // so far, so good ... 

         if(this is KIZDUC) // Izdatnica U Komisiju 
         {
            Fld_SkladCD2   = theLui.Cd;
            Fld_Sklad2Opis = theLui.Name;
            Fld_SkladBR2   = (uint)theLui.Integer;
         }
         else // PIKDUC // Povrat Iz Komisije ili IRA iz komisije 
         {
            Fld_SkladCD   = theLui.Cd;
            Fld_SkladOpis = theLui.Name;
            Fld_SkladBR   = (uint)theLui.Integer;
         }
         this.oldSkladCD  = "";
         this.oldSkladCD2 = "";
         OnExitSkladCD_SetTtNum_And_ValidateSkladCD(tbx_SkladCd, new CancelEventArgs());
      }

      #endregion KOMISIJA

      // !!! PAZI !!! 
      // #region KOMISIJA mora biti zadnja stvar u metodi jer sadrzi return-ove, 
      // pa se tako onda eventualno niza logika nece dogoditi
      // ... 
   }

   private void PutPosJedinKupdobFields(Kupdob _kupdob_rec)
   {
      // 29.6.2011: 
      //Fld_PosJedCd     = _kupdob_rec.KupdobCD;
      //Fld_PosJedTk     = _kupdob_rec.Ticker;
      //Fld_PosJedName   = _kupdob_rec.Naziv;

      //Fld_PosJedUlica  = _kupdob_rec.Ulica2;
      //Fld_PosJedZip    = _kupdob_rec.PostaBr;
      //Fld_PosJedMjesto = _kupdob_rec.Grad;
      
      faktur_rec.PosJedCD     = Fld_PosJedCd     = _kupdob_rec.KupdobCD;
      faktur_rec.PosJedTK     = Fld_PosJedTk     = _kupdob_rec.Ticker;
      faktur_rec.PosJedName   = Fld_PosJedName   = _kupdob_rec.Naziv;
      faktur_rec.PosJedUlica  = Fld_PosJedUlica  = _kupdob_rec.Ulica2;
      faktur_rec.PosJedZip    = Fld_PosJedZip    = _kupdob_rec.PostaBr;
      faktur_rec.PosJedMjesto = Fld_PosJedMjesto = _kupdob_rec.Grad;
      
      Fld_PosJedAdresa = Faktur.GetAdresa(Fld_PosJedUlica, Fld_PosJedZip, Fld_PosJedMjesto);

   }

   private void PutCentralaKupdobFields(Kupdob kupdob_rec)
   {
      Fld_KupdobCd     = kupdob_rec.KupdobCD;
      Fld_KupdobTk     = kupdob_rec.Ticker;
      Fld_KupdobName   = kupdob_rec.Naziv;
      Fld_KdOib        = kupdob_rec.Oib;
      Fld_VatCntryCode = kupdob_rec.VatCntryCode;

      //mixer_rec.KdUlica  = artikl_rec.Ulica2;
      //mixer_rec.KdZip    = artikl_rec.PostaBr;
      //mixer_rec.KdMjesto = artikl_rec.Grad;
      Fld_KupdobUlica  = kupdob_rec.Ulica2;
      Fld_KupdobZip    = kupdob_rec.PostaBr;
      Fld_KupdobMjesto = kupdob_rec.Grad;

      //Fld_KdAdresa = Faktur.GetAdresa(mixer_rec.KdUlica, mixer_rec.KdZip, mixer_rec.KdMjesto);
      Fld_KdAdresa = Faktur.GetAdresa(Fld_KupdobUlica, Fld_KupdobZip, Fld_KupdobMjesto);

      // 14.09.2011: ova tri su stavljena u 'if' 
      if(ZXC.RISK_CopyToOtherDUC_inProgress == false)
      {
         Fld_RokPlac  = kupdob_rec.ValutaPl;

         //ptg 01.10.2021.
         //                                                                   Fld_DospDate = Fld_DokDate + new TimeSpan(Fld_RokPlac, 0, 0, 0);
         if(IsPTG_UgAnDod_DUC == false/*this is UGNorAUN_PTG_DUC == false*/ ) Fld_DospDate = Fld_DokDate + new TimeSpan(Fld_RokPlac, 0, 0, 0);

         Fld_DevName  = kupdob_rec.DevName;
      }

      if(Fld_DevNameAsEnum != ZXC.ValutaNameEnum.EMPTY &&
         Fld_DevNameAsEnum != /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum)
      {
         // jer ako je RecID 0, onda znaci da smo dosli iz 'ClearAllKupdobFields()', tj. ovdje se kod kopiraj dolazi davput (prvo clear, onda put) pa se ovo toggla 2 puta pa je to dreku pljuska... 
         if(kupdob_rec.RecID.NotZero())
            TheVvTabPage.TheVvForm.RISK_ToggleKnDeviza(null, EventArgs.Empty);
      }

      #region ZiroRnList

      //if(ZXC.TtInfo(Fld_TT).IsFinKol_I) FillZiroList(Fld_TT, ZXC.CURR_prjkt_rec);
      //else                              FillZiroList(Fld_TT, artikl_rec);  


      if(Fld_TT == Faktur.TT_IFA ||   Fld_TT == Faktur.TT_IRA ||
       /*Fld_TT == Faktur.TT_STI ||*/ Fld_TT == Faktur.TT_IOD ||
         Fld_TT == Faktur.TT_IPV)
      {

         Kupdob.FillLookUpItemZiroList(ZXC.CURR_prjkt_rec);

       //27.05.2013.IBAN
       //Fld_ZiroRn = ZXC.CURR_prjkt_rec.Ziro1;
         Fld_ZiroRn = ZXC.GetIBANfromOldZiro(ZXC.CURR_prjkt_rec.Ziro1);
      }
      else if(Fld_TT == Faktur.TT_UFA || Fld_TT == Faktur.TT_UFM || Fld_TT == Faktur.TT_URA ||
       /*Fld_TT == Faktur.TT_STU ||*/    Fld_TT == Faktur.TT_UOD ||
         Fld_TT == Faktur.TT_UPV || Fld_TT == Faktur.TT_UPM)
      {
         Kupdob.FillLookUpItemZiroList(kupdob_rec);

       //27.05.2013.IBAN
       //Fld_ZiroRn = kupdob_rec.Ziro1;
         Fld_ZiroRn = ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1);

      }

      #endregion ZiroRnList

      #region Konto

      if(Fld_TT == Faktur.TT_IFA ||   Fld_TT == Faktur.TT_IRA ||
       /*Fld_TT == Faktur.TT_STI ||*/ Fld_TT == Faktur.TT_IOD ||
         Fld_TT == Faktur.TT_IPV)
      {
         if(kupdob_rec.KontoPot.NotEmpty()) Fld_Konto = kupdob_rec.KontoPot;
      }
      else if(Fld_TT == Faktur.TT_UFA ||   Fld_TT == Faktur.TT_UFM || Fld_TT == Faktur.TT_URA ||
            /*Fld_TT == Faktur.TT_STU ||*/ Fld_TT == Faktur.TT_UOD ||
              Fld_TT == Faktur.TT_UPV ||   Fld_TT == Faktur.TT_UPM)
      {
         if(kupdob_rec.KontoDug.NotEmpty()) Fld_Konto = kupdob_rec.KontoDug;
      }

      else if(Fld_TT == Faktur.TT_UPA)
      {
         if(kupdob_rec.KontoDug.NotEmpty()) Fld_Konto = kupdob_rec.KontoDug; // upaTODO: !!!!!! 
      }

      #endregion Konto

      #region PCTOGO UGAN
      if(Fld_TT == Faktur.TT_KUG)
      {
         (this as KUG_PTG_DUC).PTG_DanFakturiranjaString = kupdob_rec.Fuse1;
      }

      if(Fld_TT == Faktur.TT_UGN || Fld_TT == Faktur.TT_AUN )
      {
         (this as UGNorAUN_PTG_DUC).Fld_somePercent        = kupdob_rec.StRbt1;
         (this as UGNorAUN_PTG_DUC).Fld_NapFromPartner_PTG = kupdob_rec.Napom1; //??? mozda je premala

         if(Fld_TT == Faktur.TT_UGN)
         {
            (this as UGNorAUN_PTG_DUC).Fld_PTG_DanFakturiranjaString = kupdob_rec.Fuse1.NotEmpty() ? kupdob_rec.Fuse1 : ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca.ToString();
            (this as UGNorAUN_PTG_DUC).Fld_PTG_DanFakturiranjaOpis   = ZXC.luiListaPTG_DanZaFaktur.GetNameForThisCd((this as UGNorAUN_PTG_DUC).Fld_PTG_DanFakturiranjaString);
         }
      }
      
      #endregion PCTOGO UGAN


      // 29.1.2011: 
      if(kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 &&
         faktur_rec.TtInfo.IsUlazniPdvTT)
      {
         Fld_PdvR12 = ZXC.PdvR12Enum.R2;
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje: prema adresaru partnera, PDV Rtip za ovog dobavljača je R2.");
      }
   }

   private void ClearAllKupdobFields()
   {
      PutAllKupdobFields(new Kupdob(0));

      if(this is RNZDUC) // ugovor + nacin zastite 
      {
         Fld_VezniDok2 = 
         Fld_Fco       = "";
      }

   }

   #endregion PutAllKupdobFields

   private void AnyArtiklTextBox_OnZaglavlje_Leave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Artikl  artikl_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         artikl_rec = ArtiklSifrar.Find(FoundInSifrar<Artikl>);

         if(artikl_rec != null && tb.Text != "")
         {
            Fld_PrjArtCD   = artikl_rec.ArtiklCD;
            Fld_PrjArtName = artikl_rec.ArtiklName;
            if(this is NORDUC)
            {
               Fld_PrjArtOP   = artikl_rec.R_orgPak;
               Fld_PrjArtOpJM = artikl_rec.R_orgPakJM;
            }

            if(this is MOD_PTG_DUC)
            {
               (this as MOD_PTG_DUC).Fld_PTG_RamKlasa = artikl_rec.Grupa2CD   ;
               (this as MOD_PTG_DUC).Fld_PTG_HddKlasa = artikl_rec.Grupa3CD   ;
               (this as MOD_PTG_DUC).Fld_Decimal01    = artikl_rec.PCK_RAM    ;
               (this as MOD_PTG_DUC).Fld_Decimal02    = artikl_rec.PCK_HDD    ;
               (this as MOD_PTG_DUC).Fld_PTG_PCKbaza  = artikl_rec.PCK_BazaCD ;
            }

         }
         else if(this.sifrarSorterType == VvSQL.SorterType.Name && tb.Text != "") // ako smo dosli iz naziva, a artikl_rec je null, to je onda 'qwe' pattern (ne postoji kao sifrar) 
         {
          //Fld_PrjArtName = tb.Text;
            Fld_PrjArtCD = "";
         }
         else
         {
            Fld_PrjArtCD = Fld_PrjArtName = "";

            if(this is MOD_PTG_DUC)
            {
               (this as MOD_PTG_DUC).Fld_PTG_RamKlasa = "";
               (this as MOD_PTG_DUC).Fld_PTG_HddKlasa = "";
               (this as MOD_PTG_DUC).Fld_Decimal01    = 0M;
               (this as MOD_PTG_DUC).Fld_Decimal02    = 0M;
               (this as MOD_PTG_DUC).Fld_PTG_PCKbaza  = "";
               (this as MOD_PTG_DUC).Fld_someMoney    = 0M;
            }
         }
      }
   }

   public void Link_ExternDokument_Click(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      #region fieldz

      Button btn = sender as Button;
      int linkId = ZXC.ValOrZero_Int(btn.Tag.ToString()); if(linkId != 1 && linkId != 2) throw new Exception("Link_ExternDokument_Click: linkId unknown! (" + linkId.ToString() + ")");

      string thisDocumentID = faktur_rec.TipBr;

      #endregion fieldz

      #region FileDialog

      OpenFileDialog openFileDialog = new OpenFileDialog();

      switch(linkId)
      {
         case 1: openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eksternLinks1.DirectoryName; break;
         case 2: openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eksternLinks2.DirectoryName; break;
      }

      // volia 
      Clipboard.SetText(thisDocumentID); // !!! 

      //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
      //openFileDialog.Filter = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.Filter = "Datoteke " + thisDocumentID + "|" + "*" + thisDocumentID + "*" + "|Sve datoteke (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;


      #endregion FileDialog
      
      if(openFileDialog.ShowDialog() == DialogResult.OK)
      {
         string fullPathName = openFileDialog.FileName;
         DirectoryInfo dInfo = new DirectoryInfo(fullPathName);

         string fileName = dInfo.Name;
         string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));
         
         switch(linkId)
         {
            case 1: ZXC.TheVvForm.VvPref.eksternLinks1.DirectoryName = directoryName; Fld_externLink1 = fullPathName; break;
            case 2: ZXC.TheVvForm.VvPref.eksternLinks2.DirectoryName = directoryName; Fld_externLink2 = fullPathName; break;
         }
      }

      openFileDialog.Dispose();
   }

   public void Show_ExternDokument_Click(object sender, EventArgs e)
   {
      #region fieldz

      Button btn = sender as Button;
      int linkId = ZXC.ValOrZero_Int(btn.Tag.ToString()); if(linkId != 1 && linkId != 2) throw new Exception("Link_ExternDokument_Click: linkId unknown! (" + linkId.ToString() + ")");

      string fullPathFileName = "";

      #endregion fieldz

      switch(linkId)
      {
         case 1: fullPathFileName = Fld_externLink1; break;
         case 2: fullPathFileName = Fld_externLink2; break;
      }

      if(fullPathFileName.IsEmpty()) return;

      // here we go 
      try
      {
         System.Diagnostics.Process.Start(fullPathFileName);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);
      }
   }

   private void OnExitOIB_SaveToKupdob(object sender, CancelEventArgs e)
   {
      if(TheVvTabPage == null || TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      // 2026: 
      if(!this.Visible) return;

    //TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

    //kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);
      kupdob_rec = Get_Kupdob_FromVvUcSifrar(Fld_KupdobCd);

      // 19.01.2026:
      if(kupdob_rec == null)
      {
         if(!(this is ZAR_DUC))
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne mogu pronaći partnera u adresaru!? OIB neće biti zapamćen.");
         return;
      }

    //bool notEmpty   = Fld_KdOib.NotEmpty()        || Fld_VatCntryCode.NotEmpty()                ;
      bool hasChanges = Fld_KdOib != kupdob_rec.Oib || Fld_VatCntryCode != kupdob_rec.VatCntryCode;

    //if(kupdob_rec != null && tb.Text.NotEmpty() && tb.Text != kupdob_rec.Oib)
      if(kupdob_rec != null && hasChanges                                     )
      {
         if(ZXC.IsBadOib(Fld_KdOib, false))
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Neispravan OIB: [{0}]!", Fld_KdOib);
            e.Cancel = true;
            return;
         }
         // za 2026 nema vise ocu necu. odma spremi! 
         //DialogResult result = MessageBox.Show("Da li zelite usnimiti VATc / OIB na ovoga partnera?",
         //   "Potvrdite novi VATc / OIB?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         //
         //if(result != DialogResult.Yes) return;

         // here we go 
         RwtrecKupdob_SaveOIB(kupdob_rec, /*tb.Text*/Fld_KdOib, Fld_VatCntryCode);
      }
   }

   private void RwtrecKupdob_SaveOIB(Kupdob kupdob_rec, string newOIB, string newVatCntryCode)
   {
      TheVvTabPage.TheVvForm.BeginEdit(kupdob_rec);

      kupdob_rec.Oib          = newOIB;
      kupdob_rec.VatCntryCode = newVatCntryCode;

      kupdob_rec.VvDao.RWTREC(TheDbConnection, kupdob_rec, false, true);

      TheVvTabPage.TheVvForm.EndEdit(kupdob_rec);
   }

   public override bool IsSomeCrutialFieldIrregularyChanged() // "Trebam li zabraniti ispravak ovog racuna?" 
   {
      if(faktur_rec.IsFiskalDutyFaktur_ONLINE == false) return false;

//if(faktur_rec.IsAlreadyFiskalized == false) return false;

      // 08.04.2024: 
      if(ZXC.IsTETRAGRAM_ANY && faktur_rec.IsAlreadyFiskalized == false) return false;
      
      FaktEx fx = faktur_rec.TheEx;

      if(faktur_rec.CurrentData._dokDate != faktur_rec.BackupData._dokDate) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena datuma nakon fiskalizacije!"  ); return true; }
      if(faktur_rec.CurrentData._dokNum  != faktur_rec.BackupData._dokNum)  { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena broja rn nakon fiskalizacije!"); return true; }
      if(faktur_rec.CurrentData._skladCD != faktur_rec.BackupData._skladCD) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena opp-a nakon fiskalizacije!"   ); return true; }

      if(fx.CurrentData._s_ukKCRP != fx.BackupData._s_ukKCRP)               { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa nakon fiskalizacije!"  ); return true; }

      // 10.11.2015: 
    //if(fx.CurrentData._nacPlac    != fx.BackupData._nacPlac         ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena nacina placanja nakon fiskalizacije!"); return true; }
      if(faktur_rec.FiskNacPlacEnum != faktur_rec.FiskNacPlac_BKP_Enum) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena nacina placanja nakon fiskalizacije!"); return true; }

      if(fx.CurrentData._s_ukPdv25m != fx.BackupData._s_ukPdv25m) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }
      if(fx.CurrentData._s_ukPdv23m != fx.BackupData._s_ukPdv23m) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }
      if(fx.CurrentData._s_ukPdv22m != fx.BackupData._s_ukPdv22m) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }
      if(fx.CurrentData._s_ukPdv10m != fx.BackupData._s_ukPdv10m) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }

  //if(fx.CurrentData._s_ukOsn08 + fx.CurrentData._s_ukOsn09 + fx.CurrentData._s_ukOsn10 + fx.CurrentData._s_ukOsn11
  //   !=
  //   fx.BackupData._s_ukOsn08  + fx.BackupData._s_ukOsn09  + fx.BackupData._s_ukOsn10  + fx.BackupData._s_ukOsn11) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }
    if(fx.CurrentData._s_ukOsn08 + fx.CurrentData._s_ukOsn09 + fx.CurrentData._s_ukOsn10 + fx.CurrentData._s_ukOsn11 + fx.CurrentData._s_ukOsn0
       !=
       fx.BackupData._s_ukOsn08  + fx.BackupData._s_ukOsn09  + fx.BackupData._s_ukOsn10  + fx.BackupData._s_ukOsn11  + fx.BackupData._s_ukOsn0) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }
      
      if(fx.CurrentData._s_ukOsn07  != fx.BackupData._s_ukOsn07 ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedozvoljena promjena iznosa pdv-a nakon fiskalizacije!"); return true; }

      return false;
   }
 
   public void OnExitCijenaSpopustom(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      VvTextBoxEditingControl vvTb_cijSaPop = sender as VvTextBoxEditingControl;

      int rIdx = TheG.CurrentCell.RowIndex;
      int cIdx = TheG.CurrentCell.ColumnIndex;

      decimal ppmvIzn        = TheG.GetDecimalCell(ci.iT_ppmvIzn , rIdx, false);
      decimal wantedCijSaPop = TheG.GetDecimalCell(ci.iT_cij_kcrp, rIdx, false);

      if(Fld_IsViaRabat == false)
      {
         decimal rabatSt1       = TheG.GetDecimalCell(ci.iT_rbt1St  , rIdx, false);
         decimal new_t_cij      = ZXC.DivSafe((100M * wantedCijSaPop), (100M - rabatSt1));
         TheG.PutCell(ci.iT_cij, rIdx, new_t_cij.Ron(4) - ppmvIzn);
      }
      else
      {
         decimal t_cij     = TheG.GetDecimalCell(ci.iT_cij, rIdx, false);
         decimal newRbtSt1 = ZXC.VvGet_rbtSt_100to90(t_cij, wantedCijSaPop - ppmvIzn); // !!! ovo ne radi ako je i ppmv u pitanju, te ako novi ppmv bude veci (umjetno povecava ciljanu MPC)
         TheG.PutCell(ci.iT_rbt1St, rIdx, newRbtSt1);
      }
   }
  
   public TransactionResult M2P_TransactionResult { get; set; }
   public uint M2P_Xtrano_Current_TtNum           { get; set; }
}

public class NewRecordEventArgs : EventArgs // Fuse, ali primjer kako inheritirati EventArgs te ga koristiti kao additional info u nekom EventHandler-u 
{
   public VvDataRecord VvDataRecord          { get; set; }
   public VvRecordUC   RecordUC              { get; set; }
   public bool         IsCopyingToAnotherDUC { get; set; }

   public NewRecordEventArgs(VvDataRecord _vvDataRecord, VvRecordUC _recordUC, bool _isCopyingToAnotherDUC)
   {
      this.VvDataRecord          = _vvDataRecord;
      this.RecordUC              = _recordUC;
      this.IsCopyingToAnotherDUC = _isCopyingToAnotherDUC;
   }
}

public class PrnFakDsc : VvLookupAsDsc
{
   #region DataLayer Propertiz

   public string  Dsc_AdresaLeftRight      { get; set; }
   public string  Dsc_RptOrientation       { get; set; } 
   public bool    Dsc_VertikalLine         { get; set; } 
   public bool    Dsc_TableBorder          { get; set; } // TableHeaderColor
   public bool    Dsc_TekstualIznos        { get; set; }
   public bool    Dsc_HorizontalLine       { get; set; }
   public bool    Dsc_RazmakRows           { get; set; }

   public string  Dsc_Title                { get; set; }

   
   
   // 10.11.2020: nije se pamtio kao uint jer: 1. nije bio dodan 'theUinteger' u doticnoj tablici                        
   //                                          2. lista nije bila dodana u eksplicitno navođenje korisnika theUinteger-a 
   //                                             u VvDaoBase.ThisLuiList_DoesntNeed_ExtraData(string luiListTitle);     
 //public uint    Dsc_PnbM                 { get; set; } 
   public int     Dsc_PnbM                 { get; set; }



   public string  Dsc_Separ1_Rn            { get; set; } 
   public string  Dsc_Separ1_Pn            { get; set; }
   public uint    Dsc_Prefix1              { get; set; } //brisat FUSE
   public string  Dsc_Prefix1Rn            { get; set; } 
   public string  Dsc_Separ2_Rn            { get; set; } 
   public string  Dsc_Separ2_Pn            { get; set; } 
   public uint    Dsc_Prefix2              { get; set; } //brisat FUSE
   public string  Dsc_Prefix2Rn            { get; set; } 
   public bool    Dsc_IsAddTtNum           { get; set; } 
   public string  Dsc_SeparIfTtNum_Rn      { get; set; } 
   public string  Dsc_SeparIfTtNum_Pn      { get; set; } 
   public bool    Dsc_IsAddYear            { get; set; } 
   public string  Dsc_SeparIfYear_Rn       { get; set; } 
   public string  Dsc_SeparIfYear_Pn       { get; set; } 
   public bool    Dsc_IsAddKupDobCd        { get; set; } 
   public string  Dsc_SeparIfKDcd_Rn       { get; set; } 
   public string  Dsc_SeparIfKDcd_Pn       { get; set; } 
   public bool    Dsc_IsAddTT              { get; set; }
   public string  Dsc_SeparIfTT_Rn         { get; set; } 
   public uint    Dsc_PrefixIR1Pb          { get; set; } 
   public uint    Dsc_PrefixIR2Pb          { get; set; } //brisat FUSE
   public string  Dsc_PrefixIR2PbTx        { get; set; } 
   public bool    Dsc_IsAddTtNum_Pb        { get; set; } 
   public bool    Dsc_IsAddYear_Pb         { get; set; } 
   public bool    Dsc_IsAddKupDobCd_Pb     { get; set; } 

   public string  Dsc_LblPrjktPerson      { get; set; } 
   public string  Dsc_LblUserPerson       { get; set; } 
   public string  Dsc_LblOsobaA           { get; set; } 
   public string  Dsc_LblOsobaB           { get; set; } 
   public bool    Dsc_SignPrimaoc         { get; set; } 
   public string  Dsc_LblPrimio           { get; set; } 
   public string  Dsc_LblOpciA            { get; set; } 
   public string  Dsc_LblOpciB            { get; set; } 

   public bool    Dsc_T_artiklCD          { get; set; }     
   public bool    Dsc_T_artiklName        { get; set; }   
   public bool    Dsc_NapomenaArt         { get; set; }  // saArtikla - stavi u CR  
   public bool    Dsc_BarCode1            { get; set; }       
   public bool    Dsc_ArtiklCD2           { get; set; }      
   public bool    Dsc_ArtiklName2         { get; set; }    
   public bool    Dsc_BarCode2            { get; set; }       
   public bool    Dsc_LongOpis            { get; set; }       
   public bool    Dsc_SerNo               { get; set; }          
   public bool    Dsc_T_jedMj             { get; set; }        
   public bool    Dsc_T_kol               { get; set; }          
   public bool    Dsc_T_cij               { get; set; }          
   public bool    Dsc_R_KC                { get; set; }           
   public bool    Dsc_T_rbt1St            { get; set; }       
   public bool    Dsc_R_rbt1              { get; set; }         
   public bool    Dsc_T_rbt2St            { get; set; }       
   public bool    Dsc_R_rbt2              { get; set; }         
   public bool    Dsc_R_cij_KCR           { get; set; }
   public bool    Dsc_R_KCR               { get; set; }          
   public bool    Dsc_T_mrzSt             { get; set; }    //26.02.2016. T_serlot // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture   
   public bool    Dsc_R_mrz               { get; set; } // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture     
   public bool    Dsc_R_cij_KCRM          { get; set; }   // 29.08.2013. za Kn Tkn_Cij // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture
   public bool    Dsc_R_KCRM              { get; set; }   // 29.08.2013. za Kn Rkn_KCRP// 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture    
   public bool    Dsc_R_ztr               { get; set; } // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture     
   public bool    Dsc_T_pdvSt             { get; set; }     
   public bool    Dsc_R_pdv               { get; set; }     
   public bool    Dsc_R_cij_KCRMP         { get; set; }   
   public bool    Dsc_R_KCRMP             { get; set; }   
   public bool    Dsc_T_doCijMal          { get; set; }    // ArtiklTS 
   public bool    Dsc_T_noCijMal          { get; set; }    //11.05.2020. theVPC on IRM
   public bool    Dsc_Ocu_VPCnaIRM        { get; set; }    //11.05.2020. kad sam ga vec dodala da ga ne brisemo iz svih lookUplista
   public bool    Dsc_R_mjMasaN           { get; set; }
   public bool    Dsc_T_garancija         { get; set; }

   public int     Dsc_NumDecT_kol         { get; set; }          
   public int     Dsc_NumDecT_cij         { get; set; }          
   public int     Dsc_NumDecR_KC          { get; set; }           
   public int     Dsc_NumDecT_rbt1St      { get; set; }       
   public int     Dsc_NumDecR_rbt1        { get; set; }         
   public int     Dsc_NumDecT_rbt2St      { get; set; }       
   public int     Dsc_NumDecR_rbt2        { get; set; }         
   public int     Dsc_NumDecR_cij_KCR     { get; set; }      
   public int     Dsc_NumDecR_KCR         { get; set; }          
   public int     Dsc_NumDecT_mrzSt       { get; set; } // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture      
   public int     Dsc_NumDecR_mrz         { get; set; }    // 18.12.2013. R_cijOP       
   public int     Dsc_NumDecR_cij_KCRM    { get; set; }    // 29.08.2013. za Kn Tkn_Cij 
   public int     Dsc_NumDecR_KCRM        { get; set; }    // 29.08.2013. za Kn Rkn_KCRP
   public int     Dsc_NumDecR_ztr         { get; set; } // 10.07.2012. FUSE za nekaj jer su se bezrazlozno nasli na printu fakture          
   public int     Dsc_NumDecT_pdvSt       { get; set; }     
   public int     Dsc_NumDecR_pdv         { get; set; }     
   public int     Dsc_NumDecR_cij_KCRMP   { get; set; }   
   public int     Dsc_NumDecR_KCRMP       { get; set; }   
   public int     Dsc_NumDecT_doCijMal    { get; set; }   
   public int     Dsc_NumDecT_noCijMal    { get; set; }

   public string  Dsc_ValName             { get; set; }
   public string  Dsc_NazivPoslJed        { get; set; }

   public string  Dsc_BelowGrid           { get; set; }

   public bool    Dsc_OcuHeader           { get; set; }
   public bool    Dsc_OcuFooter           { get; set; }
   public bool    Dsc_OcuFooter2          { get; set; }
   public bool    Dsc_OcuLogo             { get; set; }
   public int     Dsc_ScalingLogo         { get; set; }
   public decimal Dsc_ScalingPostoLogo    { get; set; }
   public bool    Dsc_OcuIspisPnb         { get; set; }

   public int     Dsc_FontOpis            { get; set; }
   public int     Dsc_FontColumns         { get; set; }
   public int     Dsc_FontBelGr           { get; set; }

   public bool    Dsc_OcuR12              { get; set; }
   public string  Dsc_JezikReport         { get; set; }
  
   public bool    Dsc_BlgOcuColKonto      { get; set; }
   public bool    Dsc_BlgOcuColRacun      { get; set; }
   public bool    Dsc_BlgOcu2na1strani    { get; set; }
   public bool    Dsc_BlgOcuOkvirUplsp    { get; set; }
   public bool    Dsc_BlgOcuSvrhaUplsp    { get; set; }

   // new________________________________________________

   public bool    Dsc_OcuKupDobTel           { get; set; }   
   public bool    Dsc_OcuProjektTel          { get; set; }
   public bool    Dsc_OcuKupDobFax           { get; set; }
   public bool    Dsc_OcuProjektFax          { get; set; }
   public bool    Dsc_OcuKupDobOib           { get; set; }
   public bool    Dsc_OcuProjektOib          { get; set; }
   public bool    Dsc_OcuKupDobMail          { get; set; }
   public bool    Dsc_OcuProjektMail         { get; set; }
   public bool    Dsc_OcuKupDobNr            { get; set; }
   public bool    Dsc_PotvrdaNarudzbe        { get; set; }
   public bool    Dsc_OcuIspisDospjecePl     { get; set; }
   public bool    Dsc_OcuIspisVeze1          { get; set; }
   public bool    Dsc_OcuIspisVeze2          { get; set; }
   public bool    Dsc_OcuIspisVeze3          { get; set; }
   public bool    Dsc_OcuIspisVeze4          { get; set; }
   public bool    Dsc_OcuIspisVezDok2        { get; set; }
   public bool    Dsc_OcuIspisNapomena2      { get; set; }
   public bool    Dsc_OcuIspisDokNum2        { get; set; }
   public bool    Dsc_OcuMemoHOnAllPages     { get; set; }
   public bool    Dsc_OcuMemoFOnAllPages     { get; set; }
   public string  Dsc_LblVeze1               { get; set; }
   public string  Dsc_LblVeze2               { get; set; }
   public string  Dsc_LblVeze3               { get; set; }
   public string  Dsc_LblVeze4               { get; set; }
   public string  Dsc_LblVezDok2             { get; set; }
   public string  Dsc_LblOsobaX              { get; set; }
   public string  Dsc_AboveGrid              { get; set; }
   public string  Dsc_AlignmentPageNum       { get; set; }
   public string  Dsc_PrintPageNum           { get; set; }
   public string  Dsc_AlignmentDokNum        { get; set; }
   public bool    Dsc_AdresOkvir             { get; set; }
   public bool    Dsc_AdresOnlyPartner       { get; set; }
   public bool    Dsc_PrintDokNum_BeforAdres { get; set; }
   public bool    Dsc_MigPositionHeader      { get; set; }
   public bool    Dsc_OcuIspisLblNapomena    { get; set; } // OcuOrigBrDok  vezniDOk
   public bool    Dsc_OcuTitleOkvir          { get; set; }
   public bool    Dsc_OcuTitleBoja           { get; set; }
   public bool    Dsc_OcuKupDobBoja          { get; set; }
   public bool    Dsc_OcuPrjktBoja           { get; set; }
   public bool    Dsc_OcuLinijeHeader        { get; set; }
   public bool    Dsc_OcuLinijeFooter        { get; set; }
   public string  Dsc_BeforNRD               { get; set; }
   public string  Dsc_AfterNRD               { get; set; }
   public bool    Dsc_OcuIspisVirmana        { get; set; }
   public string  Dsc_ObrazacA               { get; set; }
   public string  Dsc_ObrazacB               { get; set; }
   public string  Dsc_ObrazacC               { get; set; }
   public bool    Dsc_OcuKDZiro_Vir          { get; set; }

   public int     Dsc_LeftMargin             { get; set; }
   public int     Dsc_RightMargin            { get; set; }
   public bool    Dsc_OcuLinijePerson        { get; set; }
   public bool    Dsc_OcuZiroFromFak         { get; set; }
   public bool    Dsc_IspisNapomene          { get; set; }
   public bool    Dsc_OcuTextNap2            { get; set; }
   public bool    Dsc_PositionPersonR        { get; set; }
   public bool    Dsc_OcuFirmuUpotpis        { get; set; }
   public bool    Dsc_OcuTitulu              { get; set; }
   public string  Dsc_ObrazacD               { get; set; }
   public bool    Dsc_OcuJednakiFtTxt2Red    { get; set; }

   public bool    Dsc_FakAdresKaoPoslJed     { get; set; }
   public bool    Dsc_PlVirUrokuDana         { get; set; }
   public bool    Dsc_IspisOtpremnicaBr      { get; set; }
   public bool    Dsc_IspisUgovora           { get; set; }
   public bool    Dsc_OnlyArtiklLongOpis     { get; set; } //Glupaco to imas rjesno kroz kolone SAD JE TO UKUPNA TEZINA ONLY BEY KOLONE TEZINE
   public bool    Dsc_CentarNapKaoNaslov     { get; set; }
   public bool    Dsc_OibIspodAdreseOnlyP    { get; set; }
   public bool    Dsc_VisibleProjektCd       { get; set; }
   public bool    Dsc_OcuDevCijAndDevTec     { get; set; }
   public bool    Dsc_OcuRokIsporDokDate     { get; set; }
   public bool    Dsc_OcuMtrosName           { get; set; }
   public string  Dsc_TextPostoBefore        { get; set; }
   public string  Dsc_TextPostoAfter         { get; set; }
   public bool    Dsc_OcuPomakVirmana        { get; set; }
   public bool    Dsc_OcuDateX               { get; set; }
   public string  Dsc_LblDateX               { get; set; }
   public bool    Dsc_OcuPosPrint            { get; set; }
   public bool    Dsc_OcuMojuPoslJed         { get; set; }
   public string  Dsc_BelowOnPOS             { get; set; }
   public string  Dsc_MemoPOS                { get; set; }
   public string  Dsc_MemoAdd                { get; set; }
   public bool    Dsc_OcuDatumRacuna         { get; set; }   
   public bool    Dsc_OcuNapomUmjKupDob      { get; set; }   
   public bool    Dsc_OcuLikvidator          { get; set; }
   public decimal Dsc_ScalingLogo2_FP        { get; set; }
   public string  Dsc_AlignmentLogo2_FP      { get; set; }
   public string  Dsc_IsLogo2_FPN            { get; set; }
   public string  Dsc_AlignmentMemoAdd       { get; set; }
   public bool    Dsc_OcuOTS_saldo           { get; set; }
   public bool    Dsc_OcuDugoImeOnlyP        { get; set; }
   public bool    Dsc_OcuBarkodTtNum         { get; set; }
   public bool    Dsc_OcuBarKodPDF417        { get; set; }
   public bool    Dsc_OcuMemoAddGore         { get; set; }
   public bool    Dsc_NecuFiskalDodatak      { get; set; }
   public string  Dsc_TekstOslobodenPDV      { get; set; }
   public bool    Dsc_Necu_prikazEUR         { get; set; }
   public bool    Dsc_OcuSkladDate           { get; set; }

   #endregion DataLayer Propertiz

   #region Constructor

   public PrnFakDsc(VvLookUpLista vvLookUpLista) : base(vvLookUpLista)
   {
   }

   public PrnFakDsc(VvLookUpLista vvLookUpLista, bool weNeedDefaultList) : base(vvLookUpLista, weNeedDefaultList)
   {
   }

   #endregion Constructor

   #region override SetDefaultValues

   public override void SetDefaultValues(VvLookUpLista luiList)
   {
      #region string initializations

       Dsc_AdresaLeftRight    = 
       Dsc_RptOrientation     = 
       Dsc_Title              = 
       Dsc_Separ1_Rn          = 
       Dsc_Separ1_Pn          = 
       Dsc_Separ2_Rn          = 
       Dsc_Separ2_Pn          = 
       Dsc_SeparIfTtNum_Rn    = 
       Dsc_SeparIfTtNum_Pn    = 
       Dsc_SeparIfYear_Rn     = 
       Dsc_SeparIfYear_Pn     = 
       Dsc_SeparIfKDcd_Rn     = 
       Dsc_SeparIfKDcd_Pn     = 
       Dsc_LblPrjktPerson     = 
       Dsc_LblUserPerson      = 
       Dsc_LblOsobaA          = 
       Dsc_LblOsobaB          = 
       Dsc_ValName            =
       Dsc_JezikReport        =
       Dsc_NazivPoslJed       =
       Dsc_LblPrimio          = 
       Dsc_LblOpciA           = 
       Dsc_LblOpciB           = 
       Dsc_BelowGrid          =  
       Dsc_LblVeze1           =
       Dsc_LblVeze2           =
       Dsc_LblVeze3           =
       Dsc_LblVeze4           =
       Dsc_LblVezDok2         =
       Dsc_LblOsobaX          =
       Dsc_AboveGrid          =
       Dsc_AlignmentPageNum   =
       Dsc_PrintPageNum       =
       Dsc_AlignmentDokNum    = 
       Dsc_BeforNRD           = 
       Dsc_AfterNRD           = 
       Dsc_ObrazacA           = 
       Dsc_ObrazacB           = 
       Dsc_ObrazacC           = 
       Dsc_ObrazacD           = 
       Dsc_TextPostoBefore    =
       Dsc_TextPostoAfter     = 
       Dsc_LblDateX           =
       Dsc_BelowOnPOS         = 
       Dsc_MemoPOS            = 
       Dsc_MemoAdd            = 
       Dsc_AlignmentLogo2_FP  = 
       Dsc_IsLogo2_FPN        = 
       Dsc_AlignmentMemoAdd   = 
       Dsc_TekstOslobodenPDV  = "";

      #endregion string initializations

      #region defautValue

      Dsc_AdresaLeftRight = "R";    
      Dsc_RptOrientation  = "P";
     
      Dsc_PnbM = 2;
              
      Dsc_IsAddTtNum      = true;
      Dsc_IsAddKupDobCd   = true;     
      Dsc_SeparIfKDcd_Rn  = "-";    
      Dsc_SeparIfKDcd_Pn  = "-";   
         
      Dsc_NumDecT_kol       = 
      Dsc_NumDecT_rbt1St    =
      Dsc_NumDecT_rbt2St    =
      Dsc_NumDecT_mrzSt     =
      Dsc_NumDecT_pdvSt     = 0;   
      Dsc_NumDecT_cij       =    
      Dsc_NumDecR_KC        =    
      Dsc_NumDecR_rbt1      =    
      Dsc_NumDecR_rbt2      =    
      Dsc_NumDecR_cij_KCR   =    
      Dsc_NumDecR_KCR       =    
      Dsc_NumDecR_mrz       =    
      Dsc_NumDecR_cij_KCRM  =    
      Dsc_NumDecR_KCRM      =    
      Dsc_NumDecR_ztr       =    
      Dsc_NumDecR_pdv       =    
      Dsc_NumDecR_cij_KCRMP =    
      Dsc_NumDecR_KCRMP     =    
      Dsc_NumDecT_doCijMal  =    
      Dsc_NumDecT_noCijMal  = 2;

      Dsc_LblOpciA         = "Opci A:";
      Dsc_LblOpciB         = "Opci B:";
      Dsc_ScalingPostoLogo = 100.00M;

      Dsc_OcuHeader = 
      Dsc_OcuFooter = 
      Dsc_OcuLogo   = true;

      Dsc_OcuFooter2 = false;

      Dsc_OcuKupDobTel           =        
      Dsc_OcuProjektTel          =      
      Dsc_OcuKupDobFax           =      
      Dsc_OcuProjektFax          =      
      Dsc_OcuProjektOib          =      
      Dsc_OcuKupDobMail          =      
      Dsc_OcuProjektMail         =      
      Dsc_OcuKupDobNr            =      
      Dsc_PotvrdaNarudzbe        = false;
      Dsc_OcuKupDobOib           =
      Dsc_OcuIspisDospjecePl     = true;   
      Dsc_OcuIspisVeze1          =    
      Dsc_OcuIspisVeze2          =    
      Dsc_OcuIspisVeze3          =    
      Dsc_OcuIspisVeze4          =    
      Dsc_OcuIspisVezDok2        = false;   
      Dsc_OcuIspisNapomena2      = false;  
      Dsc_OcuIspisDokNum2        = true;  
      Dsc_OcuMemoHOnAllPages     = true;  
      Dsc_OcuMemoFOnAllPages     = true;  
      Dsc_AlignmentPageNum       = "R";  
      Dsc_PrintPageNum           = "H";  
      Dsc_AlignmentDokNum        = "L";  
      Dsc_AdresOkvir             = false;  
      Dsc_AdresOnlyPartner       = true;  
      Dsc_PrintDokNum_BeforAdres = true;
      Dsc_MigPositionHeader      = true;

      Dsc_OcuLinijeFooter     = true;
      Dsc_OcuLinijeHeader     = true;
      Dsc_OcuTitleOkvir       = false;
      Dsc_OcuTitleBoja        = false;
      Dsc_OcuKupDobBoja       = false;
      Dsc_OcuPrjktBoja        = false;
      Dsc_OcuIspisVirmana     = false;
      Dsc_OcuKDZiro_Vir       = false;
      Dsc_OcuIspisLblNapomena = false;

      Dsc_ObrazacA = "ObrazacA";
      Dsc_ObrazacB = "ObrazacB";
      Dsc_ObrazacC = "ObrazacC";
      Dsc_ObrazacD = "ObrazacD";

      Dsc_FontOpis    = 
      Dsc_FontColumns = 
      Dsc_FontBelGr   = 9;

      Dsc_LeftMargin          = 1    ;
      Dsc_RightMargin         = 1    ;
      Dsc_OcuLinijePerson     = false;
      Dsc_OcuZiroFromFak      = false;
      Dsc_IspisNapomene       = true ;
      Dsc_OcuTextNap2         = true ;
      Dsc_PositionPersonR     = true ;
      Dsc_OcuFirmuUpotpis     = false;
      Dsc_OcuTitulu           = false;
      Dsc_OcuJednakiFtTxt2Red = false;

      Dsc_R_mjMasaN           = false;
      Dsc_T_garancija         = false;

      Dsc_FakAdresKaoPoslJed  = false;   
      Dsc_PlVirUrokuDana      = false;   
      Dsc_IspisOtpremnicaBr   = false;   
      Dsc_IspisUgovora        = false;   
      Dsc_OnlyArtiklLongOpis  = false;   // UKUPNA TEZINA ONLY BEZ KOLONE
      Dsc_CentarNapKaoNaslov  = false;
      Dsc_OibIspodAdreseOnlyP = false;
      Dsc_VisibleProjektCd    = false;

      Dsc_OcuPomakVirmana     = false;
      Dsc_OcuDateX            = false;
      Dsc_LblDateX            = "";
      Dsc_OcuPosPrint         = false;
      Dsc_OcuMojuPoslJed      = false;
      Dsc_OcuDatumRacuna      = false;
      Dsc_OcuNapomUmjKupDob   = false;
      Dsc_OcuLikvidator       = false;

      Dsc_ScalingLogo2_FP     =   0.00M;
      Dsc_AlignmentLogo2_FP   = "R";
      Dsc_IsLogo2_FPN         = "N";
      Dsc_AlignmentMemoAdd    = "L";
                              
      Dsc_OcuOTS_saldo        = false;
      Dsc_OcuBarkodTtNum      = false;
      Dsc_OcuBarKodPDF417     = false;
      Dsc_T_mrzSt             = false; //26.02.2016. serlot
      Dsc_OcuDugoImeOnlyP     = false; // 09.05.2018. Tvrtaka - dugi opis partnera a printu dokumenata
      Dsc_OcuMemoAddGore      = false; // 11.04.2019. 
      Dsc_NecuFiskalDodatak   = false; // 15.06.2020. za dostavnicu za koju to ne treba 

      Dsc_Necu_prikazEUR      = false;
      Dsc_OcuSkladDate        = false;

      #endregion defautValue

      //erwer 

      if(TT == Faktur.TT_IRA)
      {
         if(luiList == ZXC.dscLuiLst_IRA_1)
         {
            Dsc_Title = "RAČUN - Izdatnica";
           
            Dsc_T_artiklCD   =          
            Dsc_T_artiklName =          
            Dsc_T_jedMj      =          
            Dsc_T_kol        =          
            Dsc_T_cij        =          
            Dsc_T_rbt1St     =          
            Dsc_R_KCR        =          
            Dsc_T_pdvSt      =          
            Dsc_R_KCRMP      = true;         
            
            Dsc_SignPrimaoc  = true;         
            Dsc_LblPrimio    = "Robu preuzeo:";
            Dsc_OcuR12       = true;
         }
         else if(luiList == ZXC.dscLuiLst_IRA_2)
         {
            Dsc_Title        = "Izdatnica";

            Dsc_T_artiklCD   =          
            Dsc_T_artiklName =          
            Dsc_T_jedMj      =          
            Dsc_T_kol        =          
            Dsc_T_cij        =          
            Dsc_T_rbt1St     =          
            Dsc_R_KCR        =          
            Dsc_R_KCRMP      = true;
           
            Dsc_SignPrimaoc  = true;         
            Dsc_LblPrimio    = "Robu preuzeo:";

         }
         else if(luiList == ZXC.dscLuiLst_IRA_3)
         {
            Dsc_Title = "RAČUN";

            Dsc_T_artiklCD   =          
            Dsc_T_artiklName =          
            Dsc_T_jedMj      =          
            Dsc_T_kol        =          
            Dsc_T_cij        =          
            Dsc_T_rbt1St     =          
            Dsc_R_KCR        =          
            Dsc_T_pdvSt      =          
            Dsc_R_KCRMP      = true;         

         }
         else if(luiList == ZXC.dscLuiLst_IRA_4)
         {
            Dsc_Title = "RAČUN";

            Dsc_T_artiklCD =
            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_T_rbt1St =
            Dsc_R_KCR =
            Dsc_T_pdvSt =
            Dsc_R_KCRMP = true;

         }
         else if(luiList == ZXC.dscLuiLst_IRA_5)
         {
            Dsc_Title = "DOSTAVNICA";

            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =  true;
         }
      }
      else if(TT == Faktur.TT_PON || TT == Faktur.TT_OPN)
      {
         Dsc_Title = "PONUDA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KCR        =          
         Dsc_R_KCRMP      = true;         
         Dsc_OcuR12       = false;
      }
      else if(luiList == ZXC.dscLuiLst_PNM)
      {
         Dsc_Title = "PONUDA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KCR        =          
         Dsc_R_KCRMP      = true;         
         Dsc_OcuR12       = false;
      }
      else if(luiList == ZXC.dscLuiLst_INM)
      {
         Dsc_Title = "INVENTURA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KCR        =          
         Dsc_R_KCRMP      = true;         
         Dsc_OcuR12       = false;
      }
      else if(luiList == ZXC.dscLuiLst_PNM_2)
      {
         Dsc_Title = "PONUDA";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_T_rbt1St =
         Dsc_R_KCR =
         Dsc_R_KCRMP = true;
         Dsc_OcuR12 = false;
      }
      else if(luiList == ZXC.dscLuiLst_PNM_3)
      {
         Dsc_Title = "PONUDA";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_T_rbt1St =
         Dsc_R_KCR =
         Dsc_R_KCRMP = true;
         Dsc_OcuR12 = false;
      }
      else if(luiList == ZXC.dscLuiLst_PNM_4)
      {
         Dsc_Title = "PONUDA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KCR        =          
         Dsc_R_KCRMP      = true;         
         Dsc_OcuR12       = false;
      }

      else if(TT == Faktur.TT_IFA)
      {
         if(luiList == ZXC.dscLuiLst_IFA)
         {

            Dsc_Title = "RAČUN";

            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_R_KCR =
            Dsc_T_pdvSt =
            Dsc_R_KCRMP = true;
            Dsc_OcuR12 = true;
         }
         else if(luiList == ZXC.dscLuiLst_IFA_2)
         {
            Dsc_Title = "RAČUN";

            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_R_KCR =
            Dsc_T_pdvSt =
            Dsc_R_KCRMP = true;
            Dsc_OcuR12  = false;

         }
         else if(luiList == ZXC.dscLuiLst_IFA_3)
         {
            Dsc_Title = "RAČUN";

            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_R_KCR =
            Dsc_T_pdvSt =
            Dsc_R_KCRMP = true;
            Dsc_OcuR12 = false;

         }
         else if(luiList == ZXC.dscLuiLst_IFA_4)
         {
            Dsc_Title = "RAČUN";

            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_R_KCR =
            Dsc_T_pdvSt =
            Dsc_R_KCRMP = true;
            Dsc_OcuR12 = false;

         }

      }
      else if(TT == Faktur.TT_IZD)
      { 
         Dsc_Title = "Izdatnica";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_T_rbt1St =
         Dsc_R_KCR =
         Dsc_R_KCRMP = true;

         Dsc_SignPrimaoc = true;
         Dsc_LblPrimio = "Robu preuzeo:";

         Dsc_OcuR12 = false;

      }
      else if(TT == Faktur.TT_POI)
      { 
         Dsc_Title = "Povrat Posudbe";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_T_rbt1St =
         Dsc_R_KCR =
         Dsc_R_KCRMP = true;

         Dsc_SignPrimaoc = true;
         Dsc_LblPrimio = "Robu preuzeo:";

         Dsc_OcuR12 = false;

      }
     
     
      else if(TT == Faktur.TT_IRM) 
      {
         if(luiList == ZXC.dscLuiLst_IRM)
         {
            Dsc_Title        = "Račun";
            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =
            Dsc_T_cij        =
            Dsc_T_rbt1St     =
            Dsc_R_KCR        =
            Dsc_R_KCRMP      = true;
         }
         else if(luiList == ZXC.dscLuiLst_IRM_2)
         {
            Dsc_Title        = "Račun";
            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =
            Dsc_T_cij        =
            Dsc_T_rbt1St     =
            Dsc_R_KCR        =
            Dsc_R_KCRMP      = true;
         }
         else if(luiList == ZXC.dscLuiLst_IRM_3)
         {
            Dsc_Title        = "Račun";
            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =
            Dsc_T_cij        =
            Dsc_T_rbt1St     =
            Dsc_R_KCR        =
            Dsc_R_KCRMP      = true;
         }
         else if(luiList == ZXC.dscLuiLst_IRM_4)
         {
            Dsc_Title        = "Račun";
            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =
            Dsc_T_cij        =
            Dsc_T_rbt1St     =
            Dsc_R_KCR        =
            Dsc_R_KCRMP      = true;
         }


      }
      else if(TT == Faktur.TT_IZM)
      {
         if(luiList == ZXC.dscLuiLst_IZM)
         { 
            Dsc_Title = "Izdatnica";

            Dsc_T_artiklCD =
            Dsc_T_artiklName =
            Dsc_T_jedMj =
            Dsc_T_kol =
            Dsc_T_cij =
            Dsc_T_rbt1St =
            Dsc_R_KCR =
            Dsc_R_KCRMP = true;

            Dsc_SignPrimaoc = true;
            Dsc_LblPrimio = "Robu preuzeo:";

            Dsc_OcuR12 = false;
         }
         else if(luiList == ZXC.dscLuiLst_IZM_2)
         { 
            Dsc_Title = "Izdatnica";

            Dsc_T_artiklCD   =
            Dsc_T_artiklName =
            Dsc_T_jedMj      =
            Dsc_T_kol        =
            Dsc_T_cij        =
            Dsc_T_rbt1St     =
            Dsc_R_KCR        =
            Dsc_R_KCRMP      = true;

            Dsc_SignPrimaoc = true;
            Dsc_LblPrimio   = "Robu preuzeo:";

            Dsc_OcuR12 = false;
         }

      }

      else if(TT == Faktur.TT_URA) { Dsc_Title = "URA"; }
      else if(TT == Faktur.TT_UFA) { Dsc_Title = "UFA"; }
      else if(TT == Faktur.TT_UPA) { Dsc_Title = "UFA"; } // upaTODO !!!!!! 
      else if(TT == Faktur.TT_UFM) { Dsc_Title = "UFM"; }
      else if(TT == Faktur.TT_PRI) { Dsc_Title = "PRIMKA"; }
      else if(TT == Faktur.TT_POU) { Dsc_Title = "POSUDBA"; }
      else if(TT == Faktur.TT_KLK) { Dsc_Title = "KALKULACIJA"; }
      else if(TT == Faktur.TT_KKM) { Dsc_Title = "KOMISIJSKA KALKULACIJA"; }
      else if(TT == Faktur.TT_UOD) { Dsc_Title = "ODOBRENJE"; }
      else if(TT == Faktur.TT_UPV) { Dsc_Title = "POVRAT"; }
      else if(TT == Faktur.TT_UPM) { Dsc_Title = "POVRAT MALOPRODAJNI"; }
      else if(TT == Faktur.TT_IOD) { Dsc_Title = "ODOBRENJE"; }
      else if(TT == Faktur.TT_IPV) { Dsc_Title = "POVRAT"; }
      else if(TT == Faktur.TT_NRD) 
      { 
         Dsc_Title = "NARUDŽBA";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_R_KCRMP = true;

         Dsc_OcuR12 = false;

      }
      else if(TT == Faktur.TT_NRM)
      {
         Dsc_Title = "NARUDŽBA";

         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      =
         Dsc_T_kol        =
         Dsc_T_cij        =
         Dsc_R_KCRMP      = true;

         Dsc_OcuR12 = false;

      }

      else if(TT == Faktur.TT_NRU) 
      { 
         Dsc_Title = "NARUDŽBA";

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_R_KCRMP = true;

         Dsc_OcuR12 = false;


      }
      else if(TT == Faktur.TT_NRS) 
      {
         Dsc_Title = "NARUDŽBA"; 

         Dsc_T_artiklCD =
         Dsc_T_artiklName =
         Dsc_T_jedMj =
         Dsc_T_kol =
         Dsc_T_cij =
         Dsc_R_KCRMP = true;

         Dsc_OcuR12 = false;


      }
      else if(TT == Faktur.TT_NRK) { Dsc_Title = "NARUDŽBA"; }
      //else if(TT == Faktur.TT_STU) { Dsc_Title = "STU"; }
      //else if(TT == Faktur.TT_STI) { Dsc_Title = "STI"; }
      else if(TT == Faktur.TT_RVI) 
      { 
         Dsc_Title = "REVERS"; 
      
         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      =
         Dsc_T_kol        = true;

      }
      else if(TT == Faktur.TT_RVU)
      { 
         Dsc_Title = "POVRAT REVERSA";
         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      =
         Dsc_T_kol        = true;

      }
      else if(TT == Faktur.TT_UPL || TT == Faktur.TT_ISP || TT == Faktur.TT_BUP || TT == Faktur.TT_BIS || TT == Faktur.TT_ABU || TT == Faktur.TT_ABI) 
      {

         Dsc_OcuHeader        =
         Dsc_OcuFooter        =
         Dsc_OcuLogo          =
         Dsc_BlgOcuColKonto   =
         Dsc_BlgOcuColRacun   =
         Dsc_BlgOcu2na1strani =
         Dsc_BlgOcuOkvirUplsp =
         Dsc_TableBorder      = true;
         Dsc_OcuFooter2       = false;
         Dsc_BlgOcuSvrhaUplsp = false;
      }
      
      else if(TT == "CJE" /*Faktur.TT_CJ_DE || TT == Faktur.TT_CJ_MK ||TT == Faktur.TT_CJ_MP ||TT == Faktur.TT_CJ_MRZ ||TT == Faktur.TT_CJ_RB1 ||TT == Faktur.TT_CJ_RB2 ||TT == Faktur.TT_CJ_VP1 ||TT == Faktur.TT_CJ_VP2*/)
      {
         Dsc_Title        = "CJENIK";

         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      = true;
         Dsc_T_kol        = false;          
         Dsc_T_cij        = true;         
         Dsc_R_KC         = false;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd = false;     

      }
      else if(TT == Faktur.TT_PST)
      {
         Dsc_Title        = "POČETNO STANJE";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_R_KC         = true;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd = false;     

      }

      else if(TT == Faktur.TT_INV )
      {
         Dsc_Title        = "INVENTURA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_R_KC         = true;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd = false;     
         
      }
      else if(TT == Faktur.TT_PPR)
      {
         Dsc_Title = "PREDATNICA U PROIZVODNJU";

         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      =
         Dsc_T_kol        = true;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd = false;

      }
      else if(TT == Faktur.TT_PIP)
      {
         Dsc_Title = "PRIMKA IZ PROIZVODNJE";

         Dsc_T_artiklCD     =
         Dsc_T_artiklName   =
         Dsc_T_jedMj        =
         Dsc_T_kol          = true;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd  = false;

      } 
      else if(TT == Faktur.TT_POV)
      {
         Dsc_Title = "POVRATNICA";

         Dsc_T_artiklCD   =
         Dsc_T_artiklName =
         Dsc_T_jedMj      =
         Dsc_T_kol        = true;
         Dsc_SeparIfKDcd_Rn = "";
         Dsc_IsAddKupDobCd = false;

      }
      else if(TT == Faktur.TT_MSI || TT == Faktur.TT_MSU)
      {
         Dsc_Title = "MEĐUSKLADIŠNICA";

         Dsc_T_artiklCD     =
         Dsc_T_artiklName   =
         Dsc_T_jedMj        =
         Dsc_T_kol          =
         Dsc_T_cij          =
         Dsc_R_KC           = true;
         Dsc_SeparIfKDcd_Rn = "";
      }
      else if(TT == Faktur.TT_KIZ)
      {
         Dsc_Title = "IZDATNICA";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KC         = true;         
         Dsc_R_KCR        = true;         
         Dsc_OcuR12       = false;
         Dsc_SignPrimaoc  = true;
         Dsc_LblPrimio    = "Robu preuzeo:";
         Dsc_OcuIspisDospjecePl = false;

      }
      else if(TT == Faktur.TT_PIK)
      {
         Dsc_Title = "POVRAT";
           
         Dsc_T_artiklCD   =          
         Dsc_T_artiklName =          
         Dsc_T_jedMj      =          
         Dsc_T_kol        =          
         Dsc_T_cij        =          
         Dsc_T_rbt1St     =          
         Dsc_R_KC         = true;         
         Dsc_R_KCR        = true;         
         Dsc_OcuR12       = false;
         Dsc_OcuIspisDospjecePl = false;
      }



      //else if(TT == Faktur.TT_IMT ) 
      //else if(TT == Faktur.TT_PIZ )
      //else if(TT == Faktur.TT_PUL ) 
      //else if(TT == Faktur.TT_SKI )
      //else if(TT == Faktur.TT_SKU )
      //else if(TT == Faktur.TT_TMI )
      //else if(TT == Faktur.TT_TMU )
      //else if(TT == Faktur.TT_ZPC )
   }

   #endregion override SetDefaultValues
}

public class KtoShemaDsc : VvLookupAsDsc
{
   #region DataLayer Propertiz

   #region KontoSchema

   public string Dsc_RKto_Dobav        { get; set; }
   public string Dsc_RKto_Osn_Ura      { get; set; }
   public string Dsc_RKto_Pdv10m_Ura   { get; set; }
   public string Dsc_RKto_Pdv22m_Ura   { get; set; }
   public string Dsc_RKto_Pdv23m_Ura   { get; set; }
   public string Dsc_RKto_Pdv25m_Ura   { get; set; }
   public string Dsc_RKto_Pdv10n_Ura   { get; set; }
   public string Dsc_RKto_Pdv22n_Ura   { get; set; }
   public string Dsc_RKto_Pdv23n_Ura   { get; set; }
   public string Dsc_RKto_Pdv25n_Ura   { get; set; }
   public string Dsc_RKto_Pdv10R2_Ura  { get; set; }
   public string Dsc_RKto_Pdv22R2_Ura  { get; set; }
   public string Dsc_RKto_Pdv23R2_Ura  { get; set; }
   public string Dsc_PKto_Dobav        { get; set; }
   public string Dsc_PKto_Osn_Ura      { get; set; }
   public string Dsc_PKto_Pdv10m_Ura   { get; set; }
   public string Dsc_PKto_Pdv22m_Ura   { get; set; }
   public string Dsc_PKto_Pdv23m_Ura   { get; set; }
   public string Dsc_PKto_Pdv25m_Ura   { get; set; }
   public string Dsc_PKto_Pdv10n_Ura   { get; set; }
   public string Dsc_PKto_Pdv22n_Ura   { get; set; }
   public string Dsc_PKto_Pdv23n_Ura   { get; set; }
   public string Dsc_PKto_Pdv25n_Ura   { get; set; }
   public string Dsc_UKto_Dobav        { get; set; }
   public string Dsc_UKto_Osn_Ura      { get; set; }
   public string Dsc_UKto_Pdv10m_Ura   { get; set; }
   public string Dsc_UKto_Pdv22m_Ura   { get; set; }
   public string Dsc_UKto_Pdv23m_Ura   { get; set; }
   public string Dsc_UKto_Pdv25m_Ura   { get; set; }
   public string Dsc_UKto_Pdv10n_Ura   { get; set; }
   public string Dsc_UKto_Pdv22n_Ura   { get; set; }
   public string Dsc_UKto_Pdv23n_Ura   { get; set; }
   public string Dsc_UKto_Pdv25n_Ura   { get; set; }
   public string Dsc_SKto_Dobav        { get; set; }
   public string Dsc_SKto_Osn_Ura      { get; set; }
   public string Dsc_SKto_Pdv10m_Ura   { get; set; }
   public string Dsc_SKto_Pdv22m_Ura   { get; set; }
   public string Dsc_SKto_Pdv23m_Ura   { get; set; }
   public string Dsc_SKto_Pdv25m_Ura   { get; set; }
   public string Dsc_SKto_Pdv10n_Ura   { get; set; }
   public string Dsc_SKto_Pdv22n_Ura   { get; set; }
   public string Dsc_SKto_Pdv23n_Ura   { get; set; }
   public string Dsc_SKto_Pdv25n_Ura   { get; set; }
   public string Dsc_RKto_Kupca        { get; set; }
   public string Dsc_RKto_Osn_Ira      { get; set; }
   public string Dsc_RKto_Pdv10_Ira    { get; set; }
   public string Dsc_RKto_Pdv22_Ira    { get; set; }
   public string Dsc_RKto_Pdv23_Ira    { get; set; }
   public string Dsc_RKto_Pdv25_Ira    { get; set; }
   public string Dsc_PKto_Kupca        { get; set; }
   public string Dsc_PKto_Osn_Ira      { get; set; }
   public string Dsc_PKto_Pdv10_Ira    { get; set; }
   public string Dsc_PKto_Pdv22_Ira    { get; set; }
   public string Dsc_PKto_Pdv23_Ira    { get; set; }
   public string Dsc_PKto_Pdv25_Ira    { get; set; }
   public string Dsc_Mir_Kupci         { get; set; }
   public string Dsc_KtoOsn_Mir        { get; set; }
   public string Dsc_KtoPdv10_Mir      { get; set; }
   public string Dsc_KtoPdv22_Mir      { get; set; }
   public string Dsc_KtoPdv23_Mir      { get; set; }
   public string Dsc_KtoPdv25_Mir      { get; set; }
   public string Dsc_Blg_Promet        { get; set; }
   public string Dsc_Blg_Uplat         { get; set; }
   public string Dsc_Blg_Isplat        { get; set; }
   public string Dsc_Kto_Skladiste     { get; set; }
   public string Dsc_Kto_Realizacija   { get; set; }
   public string Dsc_RKto_Osn_Ira_Roba { get; set; }
   public string Dsc_PKto_Osn_Ira_Roba { get; set; }
   public bool   Dsc_HocuRealizaciju   { get; set; }
   public bool   Dsc_MirSumMonthly     { get; set; }
   public bool   Dsc_MirGroupByNacPlac { get; set; }
   public string Dsc_KtoOsnIra_UslgDP  { get; set; }
   public string Dsc_KtoMatSklad       { get; set; }
   public string Dsc_KtoMatTrsk        { get; set; }
   public string Dsc_KtoSInvSklad      { get; set; }
   public string Dsc_KtoSInvTrsk       { get; set; }
   public bool   Dsc_IsCheckAvanses    { get; set; }

   public int                          Dsc_Fak2NalTime_Ulaz { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum     Fak2NalTime_Ulaz 
   { 
      get { return (ZXC.Faktur2NalogTimeRuleEnum)Dsc_Fak2NalTime_Ulaz; }  
      set {                                      Dsc_Fak2NalTime_Ulaz = (int)value; } 
   }

   public int                          Dsc_Fak2NalTime_IzlazVP  { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum     Fak2NalTime_IzlazVP  
   { 
      get { return (ZXC.Faktur2NalogTimeRuleEnum)Dsc_Fak2NalTime_IzlazVP; }  
      set {                                      Dsc_Fak2NalTime_IzlazVP = (int)value; } 
   }

   public int                          Dsc_Fak2NalTime_IzlazMP  { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum     Fak2NalTime_IzlazMP  
   {
      get { return (ZXC.Faktur2NalogTimeRuleEnum)Dsc_Fak2NalTime_IzlazMP; }  
      set {                                      Dsc_Fak2NalTime_IzlazMP = (int)value; } 
   }

   public int                          Dsc_Fak2NalTime_Blagajna  { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum     Fak2NalTime_Blagajna
   {
      get { return (ZXC.Faktur2NalogTimeRuleEnum)Dsc_Fak2NalTime_Blagajna; }
      set {                                      Dsc_Fak2NalTime_Blagajna = (int)value; }
   }

   public int                          Dsc_Fak2NalTime_InterniDok  { get; set; }
   public ZXC.Faktur2NalogTimeRuleEnum     Fak2NalTime_InterniDok
   {
      get { return (ZXC.Faktur2NalogTimeRuleEnum)Dsc_Fak2NalTime_InterniDok; }
      set {                                      Dsc_Fak2NalTime_InterniDok = (int)value; }
   }

   public string Dsc_PNTukupno     { get; set; }
   public string Dsc_PNTacc        { get; set; }
   public string Dsc_PNTdnevnice   { get; set; }
   public string Dsc_PNTprijevTr   { get; set; }
   public string Dsc_PNTostaliTr   { get; set; }
   public string Dsc_PNIukupno     { get; set; }
   public string Dsc_PNIacc        { get; set; }
   public string Dsc_PNIdnevnice   { get; set; }
   public string Dsc_PNIprijevTr   { get; set; }
   public string Dsc_PNIostaliTr   { get; set; }
   public string Dsc_LokoUkup      { get; set; }
   public string Dsc_LokoPrijevTr  { get; set; }
   public string Dsc_LokoOstTr     { get; set; }

   public string Dsc_IrmKupciCash    { get; set; }  
   public string Dsc_KtoIrmOsnUsl    { get; set; } 
   public string Dsc_MSK_25          { get; set; } 
   public string Dsc_MSK_23          { get; set; } 
   public string Dsc_MSK_10          { get; set; } 
   public string Dsc_MSK_00          { get; set; } 
   public string Dsc_MskPdv_25       { get; set; } 
   public string Dsc_MskPdv_23       { get; set; } 
   public string Dsc_MskPdv_10       { get; set; } 
   public string Dsc_Mrz             { get; set; }
   public bool   Dsc_KnjiziMSK_ulaz  { get; set; }
   public bool   Dsc_KnjiziMSK_izlaz { get; set; }


   public string Dsc_RKto_Pdv05_Ira  { get; set; }
   public string Dsc_PKto_Pdv05_Ira  { get; set; }
   public string Dsc_RKto_Pdv05m_Ura { get; set; }
   public string Dsc_RKto_Pdv05n_Ura { get; set; }
   public string Dsc_PKto_Pdv05m_Ura { get; set; }
   public string Dsc_PKto_Pdv05n_Ura { get; set; }
   public string Dsc_KtoPdv05_Mir    { get; set; }
   public string Dsc_MSK_05          { get; set; }
   public string Dsc_MskPdv_05       { get; set; }

   public string Dsc_KtoPpmv         { get; set; }
   public string Dsc_PdvR10m_EU      { get; set; }
   public string Dsc_PdvR25m_EU      { get; set; }
   public string Dsc_PdvR10n_EU      { get; set; }
   public string Dsc_PdvR25n_EU      { get; set; }
   public string Dsc_PdvR05m_EU      { get; set; }
   public string Dsc_PdvR05n_EU      { get; set; }
   public string Dsc_PdvU10m_EU      { get; set; }
   public string Dsc_PdvU25m_EU      { get; set; }
   public string Dsc_PdvU10n_EU      { get; set; }
   public string Dsc_PdvU25n_EU      { get; set; }
   public string Dsc_PdvU05m_EU      { get; set; }
   public string Dsc_PdvU05n_EU      { get; set; }
   public string Dsc_Pdv10m_BS       { get; set; }
   public string Dsc_Pdv25m_BS       { get; set; }
   public string Dsc_Pdv10n_BS       { get; set; }
   public string Dsc_Pdv25n_BS       { get; set; }
   public string Dsc_Pdv25m_TP       { get; set; }
   public string Dsc_Pdv25n_TP       { get; set; }
   public string Dsc_KupDobR_EU      { get; set; }
   public string Dsc_TrosakR_EU      { get; set; }
   public string Dsc_ObrPdvR25_EU    { get; set; }
   public string Dsc_ObrPdvR10_EU    { get; set; }
   public string Dsc_ObrPdvR05_EU    { get; set; }
   public string Dsc_KupDobU_EU      { get; set; }
   public string Dsc_TrosakU_EU      { get; set; }
   public string Dsc_ObrPdvU25_EU    { get; set; }
   public string Dsc_ObrPdvU10_EU    { get; set; }
   public string Dsc_ObrPdvU05_EU    { get; set; }
   public string Dsc_KupDob_BS       { get; set; }
   public string Dsc_Trosak_BS       { get; set; }
   public string Dsc_ObrPdv10_BS     { get; set; }
   public string Dsc_ObrPdv25_BS     { get; set; }
   public string Dsc_KupDob_TP       { get; set; }
   public string Dsc_Trosak_TP       { get; set; }
   public string Dsc_ObrPdv25_TP     { get; set; }
   public string Dsc_UKto_Pdv05m_Ura { get; set; }
   public string Dsc_UKto_Pdv05n_Ura { get; set; }
   public string Dsc_SKto_Pdv05m_Ura { get; set; }
   public string Dsc_SKto_Pdv05n_Ura { get; set; }
   public string Dsc_ukIznPNP        { get; set; }
   public string Dsc_Msk_PNP         { get; set; }
   public string Dsc_KtoIZD          { get; set; }
   public string Dsc_KtoIZM          { get; set; }
   public bool   Dsc_OcuPrihLikeSklad{ get; set; }

   public string Dsc_KupacKontaIOS   { get; set; }
   public string Dsc_DobavKontaIOS   { get; set; }
   public string Dsc_KtoRnmPprUSL    { get; set; }

   public bool Dsc_IsVisibleColPozicija { get; set; }
   public bool Dsc_IsVisibleColMtrosCD  { get; set; }
   public bool Dsc_IsVisibleColMtrosTK  { get; set; }
   public bool Dsc_IsVisibleColProjekt  { get; set; }
   public bool Dsc_IsVisibleColFond     { get; set; }
   public bool Dsc_IsVisibleColObrpdv   { get; set; }
   public bool Dsc_IsVisibleColObr037   { get; set; }
   public bool Dsc_IsPlanViaMtros       { get; set; }
   public bool Dsc_NoPrintPozicCd       { get; set; }
   public bool Dsc_IsVisibleColPozName  { get; set; }
   public bool Dsc_IsVisibleColFondName { get; set; }
   public bool Dsc_IsVisibleColProgAkt  { get; set; }
   public bool Dsc_isExtValidation      { get; set; }

   public bool Dsc_IsNeGrupTrosak       { get; set; }

   public bool Dsc_IsOnlyIOSknjizenje   { get; set; }
   public bool Dsc_IsIFAtoUPL_napomena  { get; set; }
   public bool Dsc_Is_OTSviaMtrosCD     { get; set; }
   public bool Dsc_ForceIRMkaoIRA       { get; set; }
   public bool Dsc_NePrikazujKDC        { get; set; }
   public bool Dsc_IsKPI24              { get; set; }
   public bool Dsc_IsPsOrigBr           { get; set; }
   public string Dsc_MAP_TTs            { get; set; } // IZ, KMP

   #endregion KontoSchema

   #region Obrada Troskova Proizvodnje

   public string   Dsc_otp_obrProTT        { get; set; }
   public string   Dsc_otp_niKtoRoot       { get; set; } 
   public string   Dsc_otp_niAnaGR         { get; set; }
   public string   Dsc_otp_rdKtoRoot       { get; set; }
   public string   Dsc_otp_rdAnaGR         { get; set; }
   public string   Dsc_otp_ktoObrade       { get; set; }
   public string   Dsc_otp_ktoPrerspTrsk   { get; set; }
   public string   Dsc_otp_kto7trosProizv  { get; set; }
   public string   Dsc_otp_ktoGotProiz     { get; set; }
   public string   Dsc_otp_ktoIndirEnd     { get; set; }
   public bool     Dsc_otp_IsSkladGotProizv{ get; set; }
   public bool     Dsc_otp_IsDirekt        { get; set; }
   public bool     Dsc_otp_IsKtoEndSame    { get; set; }
   public bool     Dsc_otp_IsAutoSave      { get; set; }
   public string   Dsc_otp_skipSatus       { get; set; }

   #endregion Obrada Troskova Proizvodnje

   #region Analiza Troskova Proizvodnje

   public decimal Dsc_atp_postoRr         { get; set; }
   public string  Dsc_atp_ktoAmKorjen     { get; set; }
   public string  Dsc_atp_rrAnaGR         { get; set; }
   public bool    Dsc_atp_hocuAmort       { get; set; }

   public string  Dsc_PrimAvansKonta      { get; set; }
   public string  Dsc_DaniAvansKonta      { get; set; }

   #endregion Analiza Troskova Proizvodnje

   #endregion DataLayer Propertiz

   #region Constructor

   public KtoShemaDsc(VvLookUpLista vvLookUpLista) : base(vvLookUpLista)
   {
   }

   public KtoShemaDsc(VvLookUpLista vvLookUpLista, bool weNeedDefaultList) : base(vvLookUpLista, weNeedDefaultList)
   {
   }

   #endregion Constructor

   #region override SetDefaultValues

   public override void SetDefaultValues(VvLookUpLista luiList)
   {
      #region defautValue

      Fak2NalTime_Ulaz       =
      Fak2NalTime_IzlazVP    =
      Fak2NalTime_IzlazMP    =
      Fak2NalTime_Blagajna   = 
      Fak2NalTime_InterniDok = ZXC.Faktur2NalogTimeRuleEnum.DoIt_NEVER;

      Dsc_RKto_Dobav        = "2200";
      Dsc_RKto_Osn_Ura      = "4000";
      Dsc_RKto_Pdv10m_Ura   = "14001";
      Dsc_RKto_Pdv22m_Ura   = "14002";
      Dsc_RKto_Pdv23m_Ura   = "14003";
      Dsc_RKto_Pdv25m_Ura   = "14005";
      Dsc_RKto_Pdv10n_Ura   = "14001";
      Dsc_RKto_Pdv22n_Ura   = "14002";
      Dsc_RKto_Pdv23n_Ura   = "14003";
      Dsc_RKto_Pdv25n_Ura   = "14005";
      Dsc_RKto_Pdv10R2_Ura  = "14001";
      Dsc_RKto_Pdv22R2_Ura  = "14002";
      Dsc_RKto_Pdv23R2_Ura  = "14003";
      Dsc_PKto_Dobav        = "2200";
      Dsc_PKto_Osn_Ura      = "4000";
      Dsc_PKto_Pdv10m_Ura   = "14001";
      Dsc_PKto_Pdv22m_Ura   = "14002";
      Dsc_PKto_Pdv23m_Ura   = "14003";
      Dsc_PKto_Pdv25m_Ura   = "14005";
      Dsc_PKto_Pdv10n_Ura   = "14001";
      Dsc_PKto_Pdv22n_Ura   = "14002";
      Dsc_PKto_Pdv23n_Ura   = "14003";
      Dsc_PKto_Pdv25n_Ura   = "14005";
      Dsc_UKto_Dobav        = "2210";
      Dsc_UKto_Osn_Ura      = "6600";
      Dsc_UKto_Pdv10m_Ura   = "1401";
      Dsc_UKto_Pdv22m_Ura   = "14002";
      Dsc_UKto_Pdv23m_Ura   = "1401";
      Dsc_UKto_Pdv25m_Ura   = "1405";
      Dsc_UKto_Pdv10n_Ura   = "14001";
      Dsc_UKto_Pdv22n_Ura   = "14002";
      Dsc_UKto_Pdv23n_Ura   = "14003";
      Dsc_UKto_Pdv25n_Ura   = "14005";
      Dsc_SKto_Dobav        = "2210";
      Dsc_SKto_Osn_Ura      = "4000";
      Dsc_SKto_Pdv10m_Ura   = "14001";
      Dsc_SKto_Pdv22m_Ura   = "14002";
      Dsc_SKto_Pdv23m_Ura   = "14003";
      Dsc_SKto_Pdv25m_Ura   = "14005";
      Dsc_SKto_Pdv10n_Ura   = "14001";
      Dsc_SKto_Pdv22n_Ura   = "14002";
      Dsc_SKto_Pdv23n_Ura   = "14003";
      Dsc_SKto_Pdv25n_Ura   = "14005";
      Dsc_RKto_Kupca        = "1200";
      Dsc_RKto_Osn_Ira      = "7500";
      Dsc_RKto_Pdv10_Ira    = "24001";
      Dsc_RKto_Pdv22_Ira    = "24002";
      Dsc_RKto_Pdv23_Ira    = "24003";
      Dsc_RKto_Pdv25_Ira    = "24005";
      Dsc_PKto_Kupca        = "1200";
      Dsc_PKto_Osn_Ira      = "7500";
      Dsc_PKto_Pdv10_Ira    = "24001";
      Dsc_PKto_Pdv22_Ira    = "24002";
      Dsc_PKto_Pdv23_Ira    = "24003";
      Dsc_PKto_Pdv25_Ira    = "24005";
      Dsc_Mir_Kupci         = "1200";
      Dsc_KtoOsn_Mir        = "7600";
      Dsc_KtoPdv10_Mir      = "24001";
      Dsc_KtoPdv22_Mir      = "24002";
      Dsc_KtoPdv23_Mir      = "24003";
      Dsc_KtoPdv25_Mir      = "24005";
      Dsc_Blg_Promet        = "1020";
      Dsc_Blg_Uplat         = "1009";
      Dsc_Blg_Isplat        = "4000";
      Dsc_Kto_Skladiste     = "6600";
      Dsc_Kto_Realizacija   = "7100";
      Dsc_RKto_Osn_Ira_Roba = "7600";
      Dsc_PKto_Osn_Ira_Roba = "7600";
      Dsc_KtoOsnIra_UslgDP  = "7511";
      Dsc_KtoMatSklad       = "3100";
      Dsc_KtoMatTrsk        = "4001";
      Dsc_KtoSInvSklad      = "3500";
      Dsc_KtoSInvTrsk       = "4040";

      Dsc_HocuRealizaciju   = false;
      Dsc_MirSumMonthly     = false;
      Dsc_MirGroupByNacPlac = false;
                            
      Dsc_PNTukupno         = "2305";
      Dsc_PNTacc            = "1240";
      Dsc_PNTdnevnice       = "4600";
      Dsc_PNTprijevTr       = "4602";
      Dsc_PNTostaliTr       = "4604";
      Dsc_PNIukupno         = "2305";
      Dsc_PNIacc            = "1240";
      Dsc_PNIdnevnice       = "4601";
      Dsc_PNIprijevTr       = "4602";
      Dsc_PNIostaliTr       = "4604";
      Dsc_LokoUkup          = "2305";
      Dsc_LokoPrijevTr      = "4603";
      Dsc_LokoOstTr         = "4604";

      Dsc_IrmKupciCash      = "1021";
      Dsc_KtoIrmOsnUsl      = "7510";
      Dsc_MSK_25            = "6635";
      Dsc_MSK_23            = "6630";
      Dsc_MSK_10            = "6631";
      Dsc_MSK_00            = "6632";
      Dsc_MskPdv_25         = "6645";
      Dsc_MskPdv_23         = "6640";
      Dsc_MskPdv_10         = "6641";
      Dsc_Mrz               = "6681";
      Dsc_KnjiziMSK_ulaz    = false;
      Dsc_KnjiziMSK_izlaz   = false;

      Dsc_otp_obrProTT        = "RNP" ;
      Dsc_otp_niKtoRoot       = ""    ; 
      Dsc_otp_niAnaGR         = ""    ;
      Dsc_otp_rdKtoRoot       = ""    ;
      Dsc_otp_rdAnaGR         = ""    ;
      Dsc_otp_ktoObrade       = "6000";
      Dsc_otp_ktoPrerspTrsk   = "4901";
      Dsc_otp_kto7trosProizv  = "7000";
      Dsc_otp_ktoGotProiz     = "6300";
      Dsc_otp_ktoIndirEnd     = ""    ;
      Dsc_otp_IsSkladGotProizv= false ;
      Dsc_otp_IsDirekt        = true  ;
      Dsc_otp_IsKtoEndSame    = false ;
      Dsc_otp_IsAutoSave      = false ;
      Dsc_otp_skipSatus       = "";

      Dsc_atp_postoRr      = 100.00M;
      Dsc_atp_ktoAmKorjen  = "430";
      Dsc_atp_rrAnaGR      = "";
      Dsc_atp_hocuAmort    = false;

      Dsc_RKto_Pdv05_Ira  = "240005";
      Dsc_PKto_Pdv05_Ira  = "240005";
      Dsc_RKto_Pdv05m_Ura = "140005";
      Dsc_RKto_Pdv05n_Ura = "140005";
      Dsc_PKto_Pdv05m_Ura = "140005";
      Dsc_PKto_Pdv05n_Ura = "140005";
      Dsc_KtoPdv05_Mir    = "140005";
      Dsc_MSK_05          = "6636"  ;
      Dsc_MskPdv_05       = "6646"  ;
      Dsc_KtoPpmv         = ""      ;
      Dsc_PdvR10m_EU      = "14010";
      Dsc_PdvR25m_EU      = "14025";
      Dsc_PdvR10n_EU      = "14010";
      Dsc_PdvR25n_EU      = "14025";
      Dsc_PdvR05m_EU      = "14005";
      Dsc_PdvR05n_EU      = "14005";
      Dsc_PdvU10m_EU      = "14010";
      Dsc_PdvU25m_EU      = "14025";
      Dsc_PdvU10n_EU      = "14010";
      Dsc_PdvU25n_EU      = "14025";
      Dsc_PdvU05m_EU      = "14005";
      Dsc_PdvU05n_EU      = "14005";
      Dsc_Pdv10m_BS       = "14010";
      Dsc_Pdv25m_BS       = "14025";
      Dsc_Pdv10n_BS       = "14010";
      Dsc_Pdv25n_BS       = "14025";
      Dsc_Pdv25m_TP       = "14025";
      Dsc_Pdv25n_TP       = "14025";
      Dsc_KupDobR_EU      = "2210" ;
      Dsc_TrosakR_EU      = "4000" ;
      Dsc_ObrPdvR25_EU    = "24025";
      Dsc_ObrPdvR10_EU    = "24010";
      Dsc_ObrPdvR05_EU    = "24005";
      Dsc_KupDobU_EU      = "2210" ;
      Dsc_TrosakU_EU      = "4000" ;
      Dsc_ObrPdvU25_EU    = "24025";
      Dsc_ObrPdvU10_EU    = "24010";
      Dsc_ObrPdvU05_EU    = "24005";
      Dsc_KupDob_BS       = "2200" ;
      Dsc_Trosak_BS       = "4000" ;
      Dsc_ObrPdv10_BS     = "24010";
      Dsc_ObrPdv25_BS     = "24025";
      Dsc_KupDob_TP       = "2200" ;
      Dsc_Trosak_TP       = "4000" ;
      Dsc_ObrPdv25_TP     = "24025";
      Dsc_UKto_Pdv05m_Ura = "1405";
      Dsc_UKto_Pdv05n_Ura = "1405";
      Dsc_SKto_Pdv05m_Ura = "1405";
      Dsc_SKto_Pdv05n_Ura = "1405";

      Dsc_ukIznPNP          = "2484";
      Dsc_Msk_PNP           = "6642";
      Dsc_KtoIZD            = "4000";
      Dsc_KtoIZM            = "4000";
      Dsc_OcuPrihLikeSklad  = true;

      Dsc_KupacKontaIOS     = "120, 121, 122";
      Dsc_DobavKontaIOS     = "220, 221, 222";

      Dsc_MAP_TTs           = "IZ, KP";

      Dsc_KtoRnmPprUSL      = "4000";

      Dsc_IsVisibleColPozicija = 
      Dsc_IsVisibleColMtrosCD  = 
      Dsc_IsVisibleColMtrosTK  = 
      Dsc_IsVisibleColProjekt  = 
      Dsc_IsVisibleColFond     = 
      Dsc_IsVisibleColObrpdv   = 
      Dsc_IsVisibleColObr037   = 
      Dsc_IsPlanViaMtros       = 
      Dsc_IsVisibleColPozName  =
      Dsc_IsVisibleColFondName =
      Dsc_IsVisibleColProgAkt  =
      Dsc_isExtValidation      =
      Dsc_NoPrintPozicCd       = 
      Dsc_IsNeGrupTrosak       = 
      Dsc_IsOnlyIOSknjizenje   = 
      Dsc_ForceIRMkaoIRA       = 
      Dsc_NePrikazujKDC        =
      Dsc_IsKPI24              =
      Dsc_IsPsOrigBr           =
      Dsc_IsIFAtoUPL_napomena  = false;
      Dsc_Is_OTSviaMtrosCD     = false;

      Dsc_PrimAvansKonta       = "230";
      Dsc_DaniAvansKonta       = "130";

      #endregion defautValue
   }

   #endregion override SetDefaultValues
}


public class RiskRulesDsc : VvLookupAsDsc
{
   #region DataLayer Propertiz

   public int      Dsc_MalopCalcKind { get; set; }
   public ZXC.MalopCalcKind MalopCalcKind 
   {
      get { return (ZXC.MalopCalcKind)Dsc_MalopCalcKind; }  
      set {                           Dsc_MalopCalcKind = (int)value; } 
   }

   public decimal Dsc_VpcMpcMarza            { get; set; }
   public int     Dsc_MalopKCD               { get; set; }
   public bool    Dsc_IsViaRabat             { get; set; }
   public Kupdob  MalopKupdob_rec            { get; set; }
   public bool    Dsc_VpcMpcMarzaTheSame4VPC { get; set; }

   public bool    Dsc_IsBarCode              { get; set; }
   public bool    Dsc_IsPKvisible            { get; set; }
   public bool    Dsc_IsZtrViaOrgPak         { get; set; }
   public bool    Dsc_IsOrgPakVisible        { get; set; }
   public string  Dsc_OrgPakText             { get; set; }

   //public bool    Dsc_IsSupressSHADOWing     { 
   //   get; 
   //   set; 
   //}


   private bool dsc_IsSupressSHADOWing;
   public  bool Dsc_IsSupressSHADOWing
   {
      get { return dsc_IsSupressSHADOWing; }
      set {        dsc_IsSupressSHADOWing = value; }
   }

   public int  Dsc_KolNumOfDecimalPlaces   { get; set; }
   public int  Dsc_FISK_TimeOutSeconds     { get; set; }
                                           
   public bool    Dsc_IsPnpStVisible       { get; set; }
   public decimal Dsc_PnpSt                { get; set; }
   public bool    Dsc_IsDateXDateIzd       { get; set; }
   public bool    Dsc_IsAnaPrihodIRM       { get; set; }
                                           
   public bool    Dsc_IsCheckOpenSaldo     { get; set; }
   public bool    Dsc_IsPrintOTSafterIRA   { get; set; }
   public decimal Dsc_KomProvizSt          { get; set; }
   public decimal Dsc_MinimalRUC           { get; set; }
                                           
   public bool    Dsc_IsMtrosColVisible    { get; set; }
   public bool    Dsc_IsKol2Visible        { get; set; }

   public int     Dsc_AmbKolNumOfDecimalPlaces { get; set; }
   public bool    Dsc_IsIRMttNum7              { get; set; }
   public bool    Dsc_IsProizvCijByArtGr       { get; set; }
   public bool    Dsc_IsDokDate2               { get; set; }

   public bool    Dsc_IsRetMoneyCalc           { get; set; }
   public bool    Dsc_IsIrmQuickPrint          { get; set; }
   public bool    Dsc_IsSklRestrictor          { get; set; }
   public bool    Dsc_IsMSIttNumByPosl         { get; set; }
   public bool    Dsc_IsRbtFromPartner         { get; set; }

   public decimal Dsc_OmjerPdv                 { get; set; }
   public decimal Dsc_BlgMin                   { get; set; }
   public decimal Dsc_KoefPlanCijProizv        { get; set; }

   public bool Dsc_IsSerlotVisible             { get; set; }
   public bool Dsc_IsRbt2ColVisible            { get; set; }

   public bool Dsc_IsCentralaFindFaktur        { get; set; }
   public bool Dsc_IsOibOznOper                { get; set; }

   public bool Dsc_IsIgnoreImportCij           { get; set; }

   public bool    Dsc_IsObligArtikl            { get; set; }
   public decimal Dsc_TolerancOdstUlCij        { get; set; }
   public bool    Dsc_IsPamtiPrintDate         { get; set; }
   public bool    Dsc_IsBlgOrderByDokNum       { get; set; }
   public bool    Dsc_IsCashFakturToBlagajna    { get; set; }
   public int     Dsc_NorKolNumOfDecimalPlaces  { get; set; }
   // 30.09.2020: 
   public string  Dsc_PreferedSkladOnUCLoad    { get; set; }
   public bool Dsc_IsVisibleLotOnIzlaz         { get; set; }
   public bool Dsc_IsUseNAK                    { get; set; }
   public bool Dsc_IsSintArt4Print             { get; set; }
   public bool Is_Serlot_Active
   {
      get
      {
         return this.Dsc_IsSerlotVisible || Dsc_IsVisibleLotOnIzlaz;
      }
   }
   public string Dsc_OpcinaCd_PNP              { get; set; }
   public bool Dsc_NOcheckDupUbyKMD            { get; set; } //check duplicate URA/UFA by kcd date money
   public decimal Dsc_PdvMathTolerancy         { get; set; }
   public bool Dsc_IsIntrastat                 { get; set; }
   public bool Dsc_IsM2PAY                     { get; set; }
   public int  Dsc_M2P_TimeOutSeconds          { get; set; }
   public bool Dsc_IsUseOPN                    { get; set; }
   public string Dsc_DefaultKPD                { get; set; }
   public string Dsc_Default_eRposProc         { get; set; }
   public int Dsc_F2_NumOfRows                 { get; set; }
   public string Dsc_F2_TT                     { get; set; }

   // 02.03.2026: 
 //public bool Dsc_F2_IsAsc                    { get; set; }
   public bool Dsc_F2_IsAsc                    { get => false; set { } }
   public bool Dsc_F2_IsAutoSend               { get; set; }
   public bool Dsc_F2_IsAutoMAP                { get; set; }
   public bool Dsc_F2_IsAutoImport             { get; set; }
   public bool Dsc_F2_IsNIR                    { get; set; }
   public bool Dsc_F2_IsNUR                    { get; set; }

   #endregion DataLayer Propertiz

   #region Constructor

   public RiskRulesDsc(VvLookUpLista vvLookUpLista) : base(vvLookUpLista)
   {
      //SetMalopKupdob_rec();
   }

   public RiskRulesDsc(VvLookUpLista vvLookUpLista, bool weNeedDefaultList) : base(vvLookUpLista, weNeedDefaultList)
   {
   }

   #endregion Constructor

   #region override SetDefaultValues

   public override void SetDefaultValues(VvLookUpLista luiList)
   {
      #region defautValue

      MalopCalcKind   = ZXC.MalopCalcKind.By_MPC;
      Dsc_IsViaRabat  = true;
      Dsc_IsBarCode   = false;
      Dsc_IsPKvisible = false;

      Dsc_VpcMpcMarza = 14.00M;

      Dsc_MalopKCD = 1;
      
      Dsc_VpcMpcMarzaTheSame4VPC = false;
      
      Dsc_KolNumOfDecimalPlaces = 2;
      Dsc_NorKolNumOfDecimalPlaces = 2;

      Dsc_FISK_TimeOutSeconds = 16;

      Dsc_IsZtrViaOrgPak  = false;
      Dsc_IsOrgPakVisible = false;
      Dsc_IsPnpStVisible  = false;
      Dsc_IsDateXDateIzd  = false;
      Dsc_IsAnaPrihodIRM  = false;

      Dsc_PnpSt = 3.00M;

      Dsc_IsCheckOpenSaldo   = false;
      Dsc_IsPrintOTSafterIRA = false;
      Dsc_KomProvizSt        = 0.00M;   
      Dsc_MinimalRUC         = 0.00M;

      Dsc_IsMtrosColVisible        = false;
      Dsc_IsKol2Visible            = false;
      Dsc_IsIRMttNum7              = false;
      Dsc_AmbKolNumOfDecimalPlaces = 0;
      Dsc_IsProizvCijByArtGr       = false;
      Dsc_IsDokDate2               = false;
      Dsc_IsRetMoneyCalc           = false;
      Dsc_IsIrmQuickPrint          = false;
      Dsc_IsSklRestrictor          = false;
      Dsc_IsMSIttNumByPosl         = false;
      Dsc_IsRbtFromPartner         = false;

      Dsc_OmjerPdv                 = 0.00M;
      Dsc_BlgMin                   = 400.00M;
      Dsc_KoefPlanCijProizv        = 0.00M;

      Dsc_OrgPakText               = "";
      Dsc_IsCentralaFindFaktur     = false;
      Dsc_IsOibOznOper             = false;
      Dsc_IsIgnoreImportCij        = false;
      Dsc_IsObligArtikl            = false;
      Dsc_TolerancOdstUlCij        = 0.00M;
      Dsc_IsPamtiPrintDate         = false;
      Dsc_IsVisibleLotOnIzlaz      = false;
      Dsc_IsUseNAK                 = false;
      Dsc_IsSintArt4Print          = false;
      Dsc_OpcinaCd_PNP             = "";
      Dsc_IsSintArt4Print          = false;

      Dsc_PreferedSkladOnUCLoad    = "";

      Dsc_PdvMathTolerancy         = 0.99M;

      Dsc_IsIntrastat              = false;
      Dsc_IsCashFakturToBlagajna   = false;
      Dsc_IsM2PAY                  = false;
      Dsc_M2P_TimeOutSeconds       = 2;

      Dsc_IsUseOPN                 = false;
      Dsc_DefaultKPD               = "";
      Dsc_Default_eRposProc        = "3";

      Dsc_F2_TT                    = "";
      Dsc_F2_NumOfRows             = 100;
      Dsc_F2_IsAsc                = false;
      Dsc_F2_IsAutoSend           = false;
      Dsc_F2_IsAutoImport         = false;
      Dsc_F2_IsAutoMAP            = false;
      Dsc_F2_IsNIR                = false;
      Dsc_F2_IsNUR                = false;

      #endregion defautValue
   }

   #endregion override SetDefaultValues

   internal void SetMalopKupdob_rec(FakturDUC theDUC)
   {
      if(theDUC == null) theDUC = ZXC.TheVvForm.TheVvUC as FakturDUC;

      MalopKupdob_rec = theDUC.Get_Kupdob_FromVvUcSifrar((uint)Dsc_MalopKCD);

      if(MalopKupdob_rec == null) MalopKupdob_rec = new Kupdob();
   }

}

public class VvRiskMacro
{
   public uint              RecID         { get; set; } // direct dataLayers
   public string            MacroName     { get; set; } // direct dataLayers
   public int               ReportZ       { get; set; } // direct dataLayers PUSE !!! 
   public bool              UseMacroDates { get; set; } // direct dataLayers
   //--------------------------------------------------                     
   public VvRpt_RiSk_Filter RptFilter     { get; set; } // every property is surficed 4 dataLayer 

   public VvRiskMacro()
   {
      this.RptFilter = new VvRpt_RiSk_Filter(false);
   }

   public VvRiskMacro(string macroName, string shortReportName, int reportZ, bool useMacroDates, VvRpt_RiSk_Filter rptFilter)
   {
      this.MacroName     = macroName    ;
      this.ReportZ       = reportZ      ; // PUSE !!! 
      this.UseMacroDates = useMacroDates;
      this.RptFilter     = rptFilter    ;

      // 25.11.2012: 
      this.RptFilter.FuseStr2 = shortReportName;
   }

   public override string ToString()
   {
      return MacroName;
   }

}

public interface IVvRealizableFakturDUC
{

   List<Rtrans> RealizRtrList_AllYears
   {
      get;
      set;
   }

   List<Rtrans> RealizRtrList_ThisYear
   {
      get;
      set;
   }

}
