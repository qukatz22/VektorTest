using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Prng;
using static ZXC;

public class UFADUC              : FakturExtDUC
{
   #region Constructor

   public UFADUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UFA
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
 
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX, hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, 
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un            , isVisible, "Šifra"           , "Šifra artikla"                                    );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"           , "Naziv artikla ili proizvoljan opis"               );
      T_artiklTS_CreateColumn      (ZXC.Q2un            , isVisible, "TS"              , "Tip artikla"                                      );
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"             , "Usluga"                                           );
      T_konto_CreateColumn         (ZXC.Q3un            , isVisible, "Konto"           , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un            , ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD"    , "Šifra Mjesta troška"               );
      R_mtros_tk_CreateColumn      (ZXC.Q3un            , ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška"   , "Tiker Mjesta troška"               );
      T_kol_CreateColumn           (ZXC.Q3un,          2, isVisible, "Kol"             , "Količina"                                         );
      T_jedMj_CreateColumn         (ZXC.Q2un            , isVisible, "JM"              , "Jedinica mjere"                                   );
      T_cij_CreateColumn           (ZXC.Q4un,          4, isVisible, "Cijena"          , "Jedinična cijena"                                 );
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"             , "Stopa rabata 1"                                   );
    //T_rbt2St_CreateColumn        (ZXC.Q2un,          0, isVisible, "Rb2"             , "Stopa rabata 2"                                   );
      R_KCR_CreateColumn           (ZXC.Q4un,          2, isVisible, "Uk bez Pdv"      , "Ukupan iznos bez PDV-a"                           );
      T_pdvSt_CreateColumn         (ZXC.Q2un,          0, isVisible, "PdvSt"           , "Stopa PDV-a"                                      );
      T_pdvKolTip_CreateColumn     (ZXC.QUN             , isVisible                                                                         );
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 ,          2, isVisible        , "Uk s PDV-om", "Ukupno s PDV-om"                   );
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, Color.Empty, clr_Ulaz);
   }
}

public class UPADUC              : FakturExtDUC
{
   #region Constructor

   public UPADUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UPA
         });
      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
 
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX, hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, 
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un            , isVisible, "Šifra"      , "Šifra artikla"                                    );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis"               );
      T_artiklTS_CreateColumn      (ZXC.Q2un            , isVisible, "TS"         , "Tip artikla"                                      );
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un            , isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_jedMj_CreateColumn         (ZXC.Q2un            , isVisible, "JM"         , "Jedinica mjere"                                   );
      T_kol_CreateColumn           (ZXC.Q3un,          2, isVisible, "Kol"        , "Količina"                                         );
      T_cij_CreateColumn           (ZXC.Q4un,          4, isVisible, "Cijena"     , "Jedinična cijena"                                 );
    //  T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1"                                   );
    //T_rbt2St_CreateColumn        (ZXC.Q2un,          0, isVisible, "Rb2"        , "Stopa rabata 2"                                   );
      R_KCR_CreateColumn           (ZXC.Q4un,          2, isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a"                           );
      T_pdvSt_CreateColumn         (ZXC.Q2un,          0, isVisible, "PdvSt"      , "Stopa PDV-a"                                      );
      T_pdvKolTip_CreateColumn     (ZXC.QUN             , isVisible                                                                    );
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 ,          2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om"                                  );
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturUPaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_UPA, clr_UPA, clr_UPA);
   }
}

public class UFMDUC              : FakturExtDUC
{
   #region Constructor

   public UFMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UFM
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, true);

      // 11.04.2019. kada se na nbc auta zeli pridodati i zavisni troskovi morta se ici preko klk-dev a zato treba obicna UFA tj UFM kad ga vec imamo
      if(IsAutoKucaProjekt)
      {
         hamp_prjArtName.Visible = true;
         hamp_prjArtName.Location = new Point(hamp_kupdobNaziv.Left, hamp_napomena.Top);
         hamp_prjArtName.BringToFront();
         hamp_napomena.Location = new Point(hamp_prjArtName.Right, hamp_prjArtName.Top);
         hamp_napomena.VvColWdt[1] = ZXC.Q10un + ZXC.QUN;
      }
      else
      {
         hamp_prjArtName.Visible = false;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV, hamp_pdvGeokind, hamp_prjArtName ,hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX, hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, 
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un            , isVisible, "Šifra"      , "Šifra artikla"                                    );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis"               );
      T_artiklTS_CreateColumn      (ZXC.Q2un            , isVisible, "TS"         , "Tip artikla"                                      );
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un            , isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn           (ZXC.Q3un,          2, isVisible, "Kol"        , "Količina"                                         );
      T_jedMj_CreateColumn         (ZXC.Q2un            , isVisible, "JM"         , "Jedinica mjere"                                   );
      T_cij_CreateColumn           (ZXC.Q4un,          4, isVisible, "Cijena"     , "Jedinična cijena"                                 );
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1"                                   );
    //T_rbt2St_CreateColumn        (ZXC.Q2un,          0, isVisible, "Rb2"        , "Stopa rabata 2"                                   );
      R_KCR_CreateColumn           (ZXC.Q4un,          2, isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a"                           );
      T_pdvSt_CreateColumn         (ZXC.Q2un,          0, isVisible, "PdvSt"      , "Stopa PDV-a"                                      );
      T_pdvKolTip_CreateColumn     (ZXC.QUN             , isVisible                                                                    );
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 ,          2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om"                                  );
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, Color.Empty, clr_Ulaz);
   }
}

public class UFAdevDUC           : UFADUC
{
   #region Constructor

   public UFAdevDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,               isVisible, "Šifra", "Šifra artikla");
      T_artiklName_CreateColumnFill(                        isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4  ,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un,               isVisible, "Konto", "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un            ,   ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD"    , "Šifra Mjesta troška"               );
      R_mtros_tk_CreateColumn      (ZXC.Q3un            ,   ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška"   , "Tiker Mjesta troška"               );
      T_kol_CreateColumn           (ZXC.Q3un,            2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,               isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q6un,            8, isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un - ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn        (ZXC.Q2un,            0, isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn           (ZXC.Q4un,            2, isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn         (ZXC.Q2un,            0, isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn     (ZXC.QUN,                isVisible);
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 ,            2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns

}

public class URADUC              : FakturExtDUC
{
   #region Constructor

   public URADUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_URA
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor
  
   #region HamperLocation
   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_tipOtpreme, 
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, hamp_eRproc,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_tipOtpreme, 
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_eRproc,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   ,             isVisible                  , "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible                  , "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_doCijMal_CreateColumn  (ZXC.Q3un, 0,          ZXC.IsPCTOGO,                   "RAM", "RAM", false);
      T_noCijMal_CreateColumn  (ZXC.Q3un, 0,          ZXC.IsPCTOGO,                   "HDD", "HDD");
      T_serlot_CreateColumn    (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_isIrmUsluga_CreateColumn(ZXC.QUN + ZXC.Qun4,     isVisible                  , "Usl"        , "Usluga");
      T_konto_CreateColumn     (ZXC.Q2un ,               isVisible                  , "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol2_CreateColumn      (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol"     , "Ambalažna količina");
      T_kol_CreateColumn       (ZXC.Q3un, 2,             isVisible                , "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,             isVisible                , "JM"         , "Jedinica mjere");

      T_cij_CreateColumn       (ZXC.Q4un, ZXC.IsTETRAGRAM_ANY ? 5 : 4, isVisible, "Cijena", "Jedinična cijena");

      T_rbt1St_CreateColumn    (ZXC.Q3un - ZXC.Qun4, 2,  isVisible                , "Rb1"        , "Stopa rabata 1");
 //   T_rbt2St_CreateColumn    (ZXC.Q2un, 0,             isVisible                , "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,             isVisible                , "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn     (ZXC.Q2un, 0,             isVisible                , "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,             isVisible);
      R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2,             isVisible, "Uk s PDV-om", "Ukupno s PDV-om");


   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }

}

public class URMDUC              : FakturExtDUC
{
   #region Constructor

   public URMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_URM
         });
   }

   #endregion Constructor
  
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, true);

      hamp_obrMPC.Visible = true;

      if(IsAutoKucaProjekt)
      {
         hamp_prjArtName.Visible = true;
         hamp_prjArtName.Location = new Point(hamp_kupdobNaziv.Left, hamp_napomena.Top);
         hamp_prjArtName.BringToFront();
         hamp_napomena.Location = new Point(hamp_prjArtName.Right, hamp_prjArtName.Top);
      }
      else
      {
         hamp_prjArtName.Visible = false;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena,
                                    hamp_prjArtName , hamp_skladCd, hamp_v1TT   , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible     , "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible     , "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, /*2*/3, isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible     , "Cijena"     , "Jedinična 'FAKTURNA' cijena");
      T_rbt1St_CreateColumn    (ZXC.Q4un, /*2mfx*/ 4, isVisible     , "Rabat %"    , "Stopa rabata 1");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible     , "Pdv %"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN,     isVisible);
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible     , "Marža %"    , "Stopa marže");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible     , "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible     , "MP Vrij"    , "");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_malop, clr_Ulaz);
   }

}
public class URMDUC_2            : URMDUC
{
   #region Constructor

   public URMDUC_2(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor
  
   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumn(ZXC.Q5un   , isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
    //T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, /*2*/ 3, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
    //R_KC_CreateColumn        (ZXC.Q4un, 2, isVisible, "FAK Vrij"   , "Iznos FAK");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 4, isVisible, "Rabat %"    , "Stopa rabata 1");
    //R_rbt1_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab1"  , "Iznos rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q4un, 2, isVisible, "Rb2"        , "Stopa rabata 2");
    //R_rbt2_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab2"  , "Iznos rabata 2");
      R_cij_kcr_CreateColumn   (ZXC.Q4un, 2, isVisible, "NBC"        , "Cijena NAB");
      R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
    //R_pdv_CreateColumn       (ZXC.Q4un, 2, isVisible, "Iznos PDV"  , "Iznos PDV-a");
      R_cij_kcrp_CreateColumn  (ZXC.Q4un, 2, isVisible, "DBC"        , "Cijena sa PDV-om");
      R_KCRP_CreateColumn      (ZXC.Q5un, 2, isVisible, "Dobavljač"  , "Ukupno s PDV-om");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
    //R_mrz_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosMarže" , "Iznos marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
    //R_ztr_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosZTR"   , "Iznos zavisnih troškova");
    //R_mskPdv_CreateColumn    (ZXC.Q4un, 2, isVisible, "mskPDV"     , "");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC", "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
      vvtbR_cij_uk.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns
}
public class URMDUC_Dev          : URMDUC
{
   #region Constructor

   public URMDUC_Dev(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor
  
   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q6un, 8, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 4, isVisible, "Rabat %"    , "Stopa rabata 1");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   }

   #endregion TheG_Specific_Columns
}

public class PrimkaVpDUC         : FakturExtDUC
{
   #region Constructor

   public PrimkaVpDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PRI
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      if(ZXC.IsTEXTHOshop)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfhampers();
      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

      hamp_pdvGeokind.Visible = ZXC.RRD.Dsc_IsIntrastat;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, hamp_pdvGeokind,/*za Tg intrastat*/
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,/* hamp_rokIsporuke, hamp_rokIspDate,*/ hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate,*/ hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
    //23.04.2025. za Tetragram
      bool isVisibleZTR = ZXC.IsTETRAGRAM_ANY;

      T_artiklCD_CreateColumn      (ZXC.Q4un   ,          isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_kol2_CreateColumn          (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol", "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2,          isVisible, "Iznos"      , "Iznos");

      // 29.3.2011:
    //R_ztr_CreateColumn(ZXC.Q4un, 4, /*isVisible*/ false, "ZTR", "Zavisni troškovi");
      //23.04.2025.
      R_ztr_CreateColumn(ZXC.Q4un, 4, isVisibleZTR, "ZTR", "Zavisni troškovi");
      // ili mozda dovoljno ovo dole?
      //CreateAllwaysInvisibleDataGridViewColumn(TheG, "R_ztr");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPrimkaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_Sklad, clr_klkPri);
   }
}
public class PrimkaBcDUC         : PrimkaVpDUC
{ 
   #region Constructor

   public PrimkaBcDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PRI
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor
   
   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un, isVisible   , "Šifra", "Šifra artikla" );
      T_artiklName_CreateColumnFill(          isVisible   , "Naziv", "Naziv artikla" );
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_artiklTS_CreateColumn      (ZXC.Q2un, isVisible   , "TS"   , "Tip artikla"   );
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"  , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un, isVisible   , "JM"   , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

}
public class PrimkaDevDUC        : PrimkaVpDUC
{
   #region Constructor

   public PrimkaDevDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
      (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PRI
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      // 18.05.2022: zbog ZTR u INIT_Memset0Rtrans_GetZtr treba ArtiklSifrar 
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
   }

   #endregion Constructor

   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un                                    ,                 isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                                              isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");
      T_serlot_CreateColumn        (ZXC.Q4un,                                   ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
 
      T_kol2_CreateColumn      (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible  , "AmbKol" , "Ambalažna količina");
      T_kol_CreateColumn       (ZXC.Q3un,                                    2, isVisible                  , "Kol"    , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un                                      , isVisible                  , "JM"     , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q6un,                                    8, isVisible                  , "Cijena" , "Jedinična cijena");

      R_kolOP_CreateColumn     (ZXC.Q3un,                                    2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn     (ZXC.Q4un,                                    2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja"  );
      
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"   , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"   , "Stopa rabata 2");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2,          isVisible, "Iznos" , "Iznos");
      // 29.3.2011:                    //4.3.2012
      R_ztr_CreateColumn(ZXC.Q4un, 2, isVisible /*false*/, "ZTR", "Zavisni troškovi");
      // ili mozda dovoljno ovo dole?
      //CreateAllwaysInvisibleDataGridViewColumn(TheG, "R_ztr");
   }

   #endregion TheG_Specific_Columns

}

public class KalkulacijaMpDUC    : FakturExtDUC
{
   #region Constructor

   public KalkulacijaMpDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_KLK
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfhampers();
      SetLocationMigrators();

      SetSumeHampers(false, true, true, true);

      hamp_obrMPC.Visible = true;

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd.JAM_ReadOnly = true;
         tbx_TtNum  .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,/*hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX,/*hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 4, isVisible, "Rabat %"    , "Stopa rabata 1");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,          isVisible);
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturKalkDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_malop, clr_klkPri);
   }
}
public class KalkulacijaMpDUC_2  : KalkulacijaMpDUC
{
   #region Constructor

   public KalkulacijaMpDUC_2(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumn(ZXC.Q5un   , isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
    //T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
    //R_KC_CreateColumn        (ZXC.Q4un, 2, isVisible, "FAK Vrij"   , "Iznos FAK");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 4, isVisible, "Rabat %"    , "Stopa rabata 1");
    //R_rbt1_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab1"  , "Iznos rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q4un, 2, isVisible, "Rb2"        , "Stopa rabata 2");
    //R_rbt2_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab2"  , "Iznos rabata 2");
      R_cij_kcr_CreateColumn   (ZXC.Q4un, 2, isVisible, "NBC"        , "Cijena NAB");
      R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    , isVisible);
    //R_pdv_CreateColumn       (ZXC.Q4un, 2, isVisible, "Iznos PDV"  , "Iznos PDV-a");
      R_cij_kcrp_CreateColumn  (ZXC.Q4un, 2, isVisible, "DBC"        , "Cijena sa PDV-om");
      R_KCRP_CreateColumn      (ZXC.Q5un, 2, isVisible, "Dobavljač"  , "Ukupno s PDV-om");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
    //R_mrz_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosMarže" , "Iznos marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
    //R_ztr_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosZTR"   , "Iznos zavisnih troškova");
    //R_mskPdv_CreateColumn    (ZXC.Q4un, 2, isVisible, "mskPDV"     , "");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   
      vvtbR_cij_uk.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

}
public class KalkulacijaMpDUC_Dev: KalkulacijaMpDUC
{
   #region Constructor

   public KalkulacijaMpDUC_Dev(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_KLK
         });
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q6un, 8, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 4, isVisible, "Rabat %"    , "Stopa rabata 1");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_mrzSt_CreateColumn     (ZXC.Q4un, 4, isVisible, "Marža %"    , "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q6un, 8, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju"); 
      R_cij_MSK_CreateColumn   (ZXC.Q6un, 8, isVisible, "MPC", "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
      R_ztr_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosZTR", "Iznos zavisnih troškova");
   }

   #endregion TheG_Specific_Columns

}

public class KKMDUC              : FakturExtDUC
{
   #region Constructor

   public KKMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_KKM
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfhampers();
      SetLocationMigrators();

      SetSumeHampers(false, true, true, true);

      hamp_obrMPC.Visible = true;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,/*hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX,/*hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
    //T_artiklName_CreateColumn(ZXC.Q5un   , isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
    //R_KC_CreateColumn        (ZXC.Q4un, 2, isVisible, "FAK Vrij"   , "Iznos FAK");
    //T_rbt1St_CreateColumn    (ZXC.Q4un, 1, isVisible, "Rabat %"    , "Stopa rabata 1");
    //R_rbt1_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab1"  , "Iznos rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q4un, 2, isVisible, "Rb2"        , "Stopa rabata 2");
    //R_rbt2_CreateColumn      (ZXC.Q4un, 2, isVisible, "IznosRab2"  , "Iznos rabata 2");
      R_cij_kcr_CreateColumn   (ZXC.Q4un, 2, isVisible, "NBC"        , "Cijena NAB");
    //R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    , isVisible);
    //R_pdv_CreateColumn       (ZXC.Q4un, 2, isVisible, "Iznos PDV"  , "Iznos PDV-a");
    //R_cij_kcrp_CreateColumn  (ZXC.Q4un, 2, isVisible, "DBC"        , "Cijena sa PDV-om");
    //R_KCRP_CreateColumn      (ZXC.Q5un, 2, isVisible, "Dobavljač"  , "Ukupno s PDV-om");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 4, isVisible, "Marža %"    , "Stopa marže");
    //R_mrz_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosMarže" , "Iznos marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
    //R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
    //R_ztr_CreateColumn       (ZXC.Q4un, 2, isVisible, "IznosZTR"   , "Iznos zavisnih troškova");
    //R_mskPdv_CreateColumn    (ZXC.Q4un, 2, isVisible, "mskPDV"     , "");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   
    //vvtbR_cij_uk.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturKKMDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_malop, clr_klkPri);
   }
}

public class PovratDobMalDUC     : FakturExtDUC
{
   #region Constructor

   public PovratDobMalDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UPM
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfhampers();
      SetLocationMigrators();

      SetSumeHampers(false, true, true, true);

      hamp_obrMPC.Visible = true;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,/*hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX,/*hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična 'FAKTURNA' cijena");
      T_rbt1St_CreateColumn    (ZXC.Q4un, 2, isVisible, "Rabat %"    , "Stopa rabata 1");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,          isVisible);
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPovDobMalDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_malop, clr_klkPri);
   }
}

public class IFADUC              : FakturExtDUC
{
   #region Constructor

   public IFADUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IFA
         });

      if(ZXC.IsPCTOGO) TheG.CellMouseDoubleClick += TheG_CellMouseDoubleClick_ShowFakturDUC_For_TipBr;

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;

   }

   private void TheG_CellMouseDoubleClick_ShowFakturDUC_For_TipBr(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      string tipBr = theG.GetStringCell(ci.iT_serlot, rowIdx, false);

      ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
   
      SetSumeHampers(true, true, true, false);
   
   }

   private void CreateArrOfHampers()
   {
      if(ZXC.IsFikalEra)
      {
         hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, hamp_Status  ,
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    ,hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_eRproc,
                                    hamp_NacPlac, hamp_fiskJIR
                                  };
      }
      else
      {
         hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV,  hamp_PonudDate, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac
                                  };
      }
      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,hamp_carinaKind,
                                    hamp_OpciA, hamp_OpciB, hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,/*hamp_eRproc,*/
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_carinaKind,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp,/*hampCbxM_eRproc,*/ hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isSerlotVisible = ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible || ZXC.IsPCTOGO;
      string serlotCol  = ZXC.IsPCTOGO ? "UGAN Rata" : "Šarža/LOT";
      string serlotOpis = ZXC.IsPCTOGO ? "Broj ugovora i rate" : "Broj Šarže/Lota";
      bool isKpdVisible = ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B;

      T_artiklCD_CreateColumn      (ZXC.IsPCTOGO ? ZXC.Q2un : ZXC.Q4un, isVisible, "Šifra"         , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                    isVisible, "Naziv"         , "Naziv artikla ili proizvoljan opis");
      T_KPD_CreateColumn           (ZXC.Q3un   ,              ZXC.IsF2_2026_rules, "KPD"           , "KPD sifra");
      T_serlot_CreateColumn        (ZXC.IsPCTOGO ? ZXC.Q6un : ZXC.Q4un, isSerlotVisible, serlotCol , serlotOpis);
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,                 isVisible, "Usl"           , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un   ,                        isVisible, "Konto"         , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                        isVisible, "JM"            , "Jedinica mjere");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                        isVisible, "Kol"           , "Količina"      );
      T_cij_CreateColumn           (ZXC.Q4un, 4,                        isVisible, "Cijena"        , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,               isVisible, "Rb1"           , "Stopa rabata 1");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,                        isVisible, "Uk bez Pdv"    , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,                        isVisible, "PdvSt"         , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn     (ZXC.QUN    ,                        isVisible);                
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 , 2,            isVisible, "Uk s PDV-om"   , "Ukupno s PDV-om");
   
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, Color.Empty, clr_Izlaz);
   }

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB         ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }
}

public class IFAdevDUC           : IFADUC
{
   #region Constructor

   public IFAdevDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn   (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      //T_KPD_CreateColumn        (ZXC.Q3un   ,isVisible, "KPD"         , "KPD");
      T_isIrmUsluga_CreateColumn(ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn      (ZXC.Q3un   ,          isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_jedMj_CreateColumn      (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_kol_CreateColumn        (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn        (ZXC.Q6un, 8,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn     (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCR_CreateColumn        (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn      (ZXC.Q2un, 0,          isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn  (ZXC.QUN    ,          isVisible);
      R_KCRP_CreateColumn       (ZXC.Q4un + ZXC.Qun2 , 2,          isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns
}

public class IRADUC              : FakturExtDUC
{
   #region Constructor

   public IRADUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRA
          //, Faktur.TT_YRA
         });

      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      //isThisMalopDUC = ZXC.TtInfo(this.dbNavigationRestrictor_TT.RestrictedValues[0]).IsMalopTT;
      //isThisVelepDUC = !isThisMalopDUC;

      //bool isLastUsedSkladCd_MAL = ZXC.luiListaSkladista.GetFlagForThisCd(ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD);

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    /*hamp_eRproc,*/ hamp_fiskPrgBr, hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        /*hampCbxM_eRproc,*/ hampCbxM_fiskPrgBr, hampCbxM_opis
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un          ,                                                      isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                                                         isVisible, "Naziv"      , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un          ,ZXC.IsF2_2026_rules                                    , "KPD"         , "KPD");
      T_serlot_CreateColumn        (ZXC.Q4un          , ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible, "Šarža/LOT"  , "Broj Šarže/Lota");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,                                                      isVisible, "Usl"        , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un          ,                                                      isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol2_CreateColumn          (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces         , ZXC.RRD.Dsc_IsKol2Visible, "AmbKol"     , "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                                                             isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                                                             isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,                                                        isVisible, "Cijena"     , "Jedinična cijena");

      R_kolOP_CreateColumn         (ZXC.Q3un, 2,                                  ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2,                                  ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");
                                   
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,                                           isVisible, "Rb1"        , "Stopa rabata 1");
      T_rbt2St_CreateColumn        (ZXC.Q2un, 0,                                 ZXC.RRD.Dsc_IsRbt2ColVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,                                                    isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");

      R_cij_kcr_CreateColumn       (ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn            (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn            (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn           (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn           (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn         (ZXC.Q2un, 0, isVisible, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN    , isVisible);
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

      T_ppmvOsn_CreateColumn       (ZXC.Q5un, 2, false, "Osnovica", "Osnovica za obračun posebnog poreza na motorna vozila", true);

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

   public override bool HasDscSubVariants
   {
      get
      {
         return true;
      }
   }

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB         ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class IRADUC_2              : FakturExtDUC
{
   #region Constructor

   public IRADUC_2(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRA
         });

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);

      hamp_S_ppmv.Visible  = !ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_ppmv.Location = new Point(hamp_S_ukPdv.Left, 0);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR, hamp_prjArtName
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_eRproc, hamp_fiskPrgBr, hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_eRproc, hampCbxM_fiskPrgBr, hampCbxM_opis
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un          ,                                                      isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                                                         isVisible, "Naziv"      , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un          ,isVisible                                    , "KPD"         , "KPD");
      T_serlot_CreateColumn        (ZXC.Q4un          , ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible, "Šarža/LOT"  , "Broj Šarže/Lota");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,                                                      isVisible, "Usl"        , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un          ,                                                      isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol2_CreateColumn          (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces         , ZXC.RRD.Dsc_IsKol2Visible, "AmbKol"     , "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                                                             isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                                                             isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,                                                        isVisible, "Cijena"     , "Jedinična cijena");

      R_kolOP_CreateColumn         (ZXC.Q3un, 2,                                  ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2,                                  ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");
                                   
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,                                           isVisible, "Rb1"        , "Stopa rabata 1");
      T_rbt2St_CreateColumn        (ZXC.Q2un, 0,                                 ZXC.RRD.Dsc_IsRbt2ColVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,                                                    isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");

      R_cij_kcr_CreateColumn       (ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn            (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn            (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn           (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn           (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn         (ZXC.Q2un, 0, isVisible, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN    , isVisible);
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
            
      T_ppmvOsn_CreateColumn  (ZXC.Q5un, 2, false, "Kataloška cijena", "Osnovica za obračun posebnog poreza na motorna vozila", true);
      T_ppmvSt1i2_CreateColumn(ZXC.Q3un, 1, false, "ZbirSt"          , "Zbirna stopa poreza na motorna vozila");
      R_ppmvIzn_CreateColumn  (ZXC.Q3un, 2, false, "PPMV iznos"      , "Iznos posebnog poreza na motorna vozila");

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

   public override bool HasDscSubVariants
   {
      get
      {
         return true;
      }
   }

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB         ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class IzdatnicaDUC        : FakturExtDUC
{
   #region Constructor

   public IzdatnicaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IZD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

      if(ZXC.IsTEXTHOshop)
      {
         tbx_SkladCd.JAM_ReadOnly = true;
         tbx_TtNum  .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PonudDate, hamp_kupdobOther,hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla");
      T_serlot_CreateColumn(ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible, "Šarža/LOT", "Broj Šarže/Lota");

      T_kol2_CreateColumn(ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol", "Ambalažna količina");
      T_kol_CreateColumn  (ZXC.Q3un, 2, isVisible, "Kol", "Količina");
      T_jedMj_CreateColumn(ZXC.Q2un,    isVisible, "JM", "Jedinica mjere");

    //T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
    //T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_cij_CreateColumn       (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv", "Ukupan iznos bez PDV-a");
   //   R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "Uk bez Pdv", "Ukupan iznos bez PDV-a");

      R_NC_CreateColumn (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn(ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn(ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");
   
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, Color.Empty);
   }
}

public class IRMDUC              : FakturExtDUC
{
   #region Constructor

   public IRMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRM
         });

      // 13.01.2026: zbog sendanja eRacuna 
    //if(ZXC.RRD.Dsc_IsM2PAY == true)
      if(ZXC.RRD.Dsc_IsM2PAY == true && TheVvTabPage != null)
      {
         TheVvTabPage.TheVvForm.M2PAY_DirectConnect(false);
      }

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      //13.11.2014.
      //SetParentOfhampers();
      
      if(ZXC.IsTEXTHOany) SetParentOfHamperLeftHampers();
      else                SetParentOfhampers();


      SetLocationMigrators();

      SetSumeHampers(true, true, true, true);

      hamp_ciljaniMPC.Visible = true;

      hamp_S_pnp_IZL.Visible = ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_pnp_IZL.Location = new Point(hamp_S_ukPdv.Left, 0);

      hamp_m2payConected.Visible = ZXC.RRD.Dsc_IsM2PAY;
      hamp_m2payStatus  .Visible = ZXC.RRD.Dsc_IsM2PAY;

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;

         hamp_irmInfoLabela.Visible = true;

         //if(false) // qKUPON ODGODA
         {
            hamp_irmKuponOUT.Visible = true;
            hamp_irmKuponIN.Visible  = true;
         }

         //Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, false, clr_Izlaz);

         //hamp_ciljaniMPC   .Visible = false;
         hamp_IznosUvaluti.Visible = false;

         // THPR news: 
         if(ZXC.IsTEXTHOshop == true)
         {
            TH_PriceRuleForCycleMoment theTHPR = TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD, DateTime.Now);
   
            if(theTHPR != null)
            {
               hamp_HappyHour.Visible = (theTHPR.HHpercent.NotZero());
            }
         }

         tbx_DokDate .JAM_ReadOnly = tbx_TtNum       .JAM_ReadOnly =
       //tbx_KupdobCd.JAM_ReadOnly = tbx_KupdobTk    .JAM_ReadOnly = tbx_KupdobName.JAM_ReadOnly =
         tbx_CjenikTT.JAM_ReadOnly = tbx_CjenikTTnum .JAM_ReadOnly =
         tbx_v1_tt   .JAM_ReadOnly = tbx_v1_ttNum    .JAM_ReadOnly = true;
        //tbx_KdOib   .JAM_ReadOnly = tbx_vatCntryCode.JAM_ReadOnly = 

         /* !!! */  // 06.08.2015: za intervencije u slucaju 'Nekonzistentnost brojeva racuna' 
         /* !!! *///if(ZXC.CurrUserHasSuperPrivileges)
         /* !!! */  if(ZXC.CurrUserHasSuperPrivileges)
         /* !!! */  {
         /* !!! */     tbx_DokDate .JAM_ReadOnly = tbx_TtNum.JAM_ReadOnly = false;
         /* !!! */  }

         //26.02.2020.
         tbx_vatCntryCode.JAM_ReadOnly = tbx_KdOib.JAM_ReadOnly = true;

         hamp_eRproc.Visible = hamp_Status.Visible = false;
      } // if(ZXC.IsTEXTHOany) 

      tbx_twinS_ukKCRP.JAM_BackColor = Color.Aquamarine;
      tbx_twinS_ukKCRP.Tag           = Color.Aquamarine;
      tbx_twinS_ukKCRP.Font          = ZXC.vvFont.LargeLargeFont;
      tbx_twinS_ukKCRP.TextAlign     = HorizontalAlignment.Center;
        
      if(ZXC.CURR_prjkt_rec.F2_ImaSamo_F1_B2C) hamp_eRproc.Visible = hamp_Status.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    //hamp_kupdobOther,  
                                    hamp_dokDate    , hamp_dokNum, 
                                    hamp_kupdobOther, hamp_Cjenik,  hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT, hamp_NacPlac, hamp_fiskJIR , hamp_Status, hamp_eRproc
                                  };

      if(ZXC.IsTEXTHOany != true)
      {
         hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hampCbxM_osobaX, hamp_carinaKind,
                                    hamp_externLink1, hamp_externLink2,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr
                                    ,hamp_opis
                                  };

         hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
         hamperNoMigLeftRight = new VvHamper[] { hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb,/* hamp_Status  ,*/ hamp_vezniDok, hamp_projekt, 
                                         hamp_RokPlac,hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_PonudDate, 
                                         hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                       };
      }
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      bool isBarCode1Visible = ZXC.RRD.Dsc_IsBarCode;
      bool isPkVisible       = ZXC.RRD.Dsc_IsPKvisible;
      bool isPnpStVisible    = ZXC.RRD.Dsc_IsPnpStVisible;
      bool isOrgPakVisible   = ZXC.RRD.Dsc_IsOrgPakVisible;

      if(isBarCode1Visible) T_barCode1_CreateColumn(ZXC.Q3un, isVisible, "BarKod", "Bar Kod artikla");

      T_artiklCD_CreateColumn      (ZXC.Q4un,             isVisible,      "Šifra"      , "Šifra artikla");
      T_artiklName_CreateColumnFill(                      isVisible,      "Naziv"      , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un   ,              ZXC.IsF2_2026_rules, "KPD"           , "KPD sifra");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible,      "Usl"        , "Usluga");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible,      "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible,      "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,          isVisible,      "Cjenik MPC" , "Jedinična cijena prema cjeniku");

      R_kolOP_CreateColumn         (ZXC.Q3un, 2,    isOrgPakVisible,      R_kolOP_ColName , "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2,    isOrgPakVisible,      R_cijOP_ColName , "Cijena originalnog pakiranja");

      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible,      "Popust"     , "Popust");
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,          isVisible,      "StPdv"      , "Stopa PDV-a");
      T_pnpSt_CreateColumn         (ZXC.Q2un, 0,          isPnpStVisible, "StPnp"      , "Stopa posebnog poreza na potrošnju");         
      T_pdvKolTip_CreateColumn     (ZXC.QUN    ,          isPkVisible                                 );
      R_cij_kcrp_CreateColumn      (ZXC.Q5un, 2,          isVisible,      "ZaPlatitiMPC", "Cijena sa popustom - za platit");

      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      R_KCRP_CreateColumn(ZXC.Q5un, 2, isVisible, "Iznos", "Iznos");

      // 13.09.2013: zbog umjetnina 
      T_ppmvOsn_CreateColumn(ZXC.Q5un, 2, false, "Nabavna cijena", "(ProdCijBezPDVa - NabCij) = Osnovica za obračun PDV-a", true);

      vvtbR_cij_uk.JAM_FieldExitMethod = new EventHandler(OnExitCijenaSpopustom);
      vvtbR_cij_uk.JAM_ShouldCalcTransAndSumGrid = true;

      if(ZXC.IsTEXTHOany)
      {
         vvtbT_cij   .JAM_ReadOnly = true;
         vvtbR_cij_uk.JAM_ReadOnly = true;
         vvtbT_pdvSt .JAM_ReadOnly = true;
      }
      this.ControlForInitialFocus = TheG;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRMDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_malop, clr_Izlaz);
   }

   // Set of Last Values Used 
   // Ovo NISU Fld-ovi!!!     
   public bool     KPN_isOn      = false;
   public decimal  Kpn_Min       = 0.00M;
   public decimal  Kpn_OUT_RbtSt = 0.00M;
   public DateTime Kpn_DateDo           ;
   public DateTime Kpn_DateOd           ;

   public override void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC)
   {
      // qKUPON ODGODA 
      //return;

      if(ZXC.IsTEXTHOany)
      {
         if(writeMode == ZXC.WriteMode.None) // izlazak iz zutog u bijelo 
         {
          //lbl_kpnOut1.Text = "IZDAN"     ;
            lbl_kpnOut1.Visible = true       ;
            lbl_kpnIn1 .Text    = "ZAPRIMLJEN";

            if(isESC == false) // NIJE ESC 
            {
               if(cbx_isKpnOUT.Checked == true) GetLastUsedKPNfieldz(this);
               else                             KPN_isOn = false          ;

               Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, true/*false*/, clr_Izlaz);
            }
            else // JE ESC 
            {
               if(KPN_isOn) Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, true         , clr_Izlaz/*Color.FromArgb(233, 183, 244)*/);
               else         Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, true/*false*/, clr_Izlaz                                 );
            }

            cbx_isKpnOUT./*Enabled*/Visible = cbx_isKpnOUT.Checked = false; // pri izlasku iz zutoga u bijelo, a da ne zbunjuje, uvijek cbx odcheckiravamo (a kod slijed ulaska u zuto ce biti kako je zapamceno)
            //ClearKPNfieldz();
         }
         else  // ulazak u zuto iz bijelog 
         {
          //lbl_kpnOut1.Text = "IZDAJ"  ;
            lbl_kpnOut1.Visible = false     ;
            lbl_kpnIn1 .Text    = "ZAPRIMI";

            if(writeMode == ZXC.WriteMode.Add)
            {
               PutLastUsedKPNfieldz(this, false);
               cbx_isKpnOUT./*Enabled*/Visible = true;

               if(cbx_isKpnOUT.Checked == false) Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, false, clr_Izlaz                    );
               else                              Set_KPN_Out_labelsEnabledState(hamp_irmKuponOUT, true , Color.FromArgb(233, 183, 244));
            }
         }
      }
   }

   internal bool Is_TH_KPN_IN_TtNum_Invalid()
   {
      if(ZXC.IsTEXTHOshop == false) return false;

      // 09.04.2025: slovacki kupon od 20%
      if(Fld_V2_ttNum == 20) return false;

      if(Fld_V2_ttNum < 1400001 || Fld_V2_ttNum > 9999999) return true;

      return false;
   }

   public override bool IsM2PAY_DUC { get { return (true); } }

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB  ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class IRMDUC_2            : FakturExtDUC
{
   #region Constructor

   public IRMDUC_2(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRM
         });

      // 13.01.2026: zbog sendanja eRacuna 
    //if(ZXC.RRD.Dsc_IsM2PAY == true)
      if(ZXC.RRD.Dsc_IsM2PAY == true && TheVvTabPage != null)
      {
         TheVvTabPage.TheVvForm.M2PAY_DirectConnect(false);
      }

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, true);
      hamp_ciljaniMPC.Visible = true;

      hamp_S_ppmv.Visible  =! ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_ppmv.Location = new Point(hamp_S_ukPdv.Left, 0);

      hamp_S_pnp_IZL.Visible  = ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_pnp_IZL.Location = new Point(hamp_S_ukPdv.Left, 0);

      hamp_m2payConected.Visible = ZXC.RRD.Dsc_IsM2PAY;
      hamp_m2payStatus.Visible   = ZXC.RRD.Dsc_IsM2PAY;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, /*hamp_PonudDate,*/ hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR, hamp_prjArtName, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_DatumX,/*hamp_NacPlac,*/  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_DatumX,/*hampCbxM_NacPlac,*/ hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible       = true;
      bool isPkVisible     = ZXC.RRD.Dsc_IsPKvisible;
      bool isPnpStVisible  = ZXC.RRD.Dsc_IsPnpStVisible;
      bool isKontoVisible  = ZXC.RRD.Dsc_IsAnaPrihodIRM;
      bool isOrgPakVisible = ZXC.RRD.Dsc_IsOrgPakVisible;

      T_artiklCD_CreateColumn      (ZXC.Q4un,             isVisible,      "Šifra"       , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible,      "Naziv"       , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un   ,              ZXC.IsF2_2026_rules, "KPD"           , "KPD sifra");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible,      "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un   ,          isKontoVisible, "Konto"       , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible,      "Kol"         , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible,      "JM"          , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,          isVisible,      "Cijena"      , "Jedinična cijena");

      R_kolOP_CreateColumn         (ZXC.Q3un, 2,         isOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2,         isOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      T_rbt1St_CreateColumn        (ZXC.Q4un, 2,          isVisible,      "Popust"      , "Popust");
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,          isVisible,      "StPdv"       , "Stopa PDV-a");
      T_pnpSt_CreateColumn         (ZXC.Q2un, 0,          isPnpStVisible, "StPnp"       , "Stopa posebnog poreza na potrošnju");
      T_pdvKolTip_CreateColumn     (ZXC.QUN,              isPkVisible);
      R_cij_kcrp_CreateColumn      (ZXC.Q5un, 2,          isVisible,      "Cij-Pop+Ppmv", "Cijena sa popustom + Poseban porez na motorna vozila");

      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      R_KCRP_CreateColumn(ZXC.Q5un, 2, isVisible, "Iznos", "Iznos");

      T_ppmvOsn_CreateColumn  (ZXC.Q5un, 2, false, "Kataloška cijena", "Osnovica za obračun posebnog poreza na motorna vozila", true);
      T_ppmvSt1i2_CreateColumn(ZXC.Q3un, 1, false, "ZbirSt"          , "Zbirna stopa poreza na motorna vozila");
      R_ppmvIzn_CreateColumn  (ZXC.Q3un, 2, false, "PPMV iznos"      , "Iznos posebnog poreza na motorna vozila");

      vvtbR_cij_uk.JAM_FieldExitMethod = new EventHandler(OnExitCijenaSpopustom);
      vvtbR_cij_uk.JAM_ShouldCalcTransAndSumGrid = true;
      
      //this.ControlForInitialFocus = TheG;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRMDUC2.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_malop, clr_Izlaz);
   }
   public override bool IsM2PAY_DUC { get { return (true); } }

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB         ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class IZMDUC              : FakturExtDUC
{
   #region Constructor

   public IZMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IZM
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, true);

      hamp_ciljaniMPC.Visible = true;

      if(ZXC.IsTEXTHOany) 
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_PonudDate, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/ hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn      (ZXC.Q4un, isVisible, "Šifra", "Šifra artikla");
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"   , "Naziv artikla");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"     , "Usluga");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"     , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible, "JM"      , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,          isVisible, "Cijena"  , "Jedinična cijena");
      T_pnpSt_CreateColumn         (ZXC.Q2un, 0,     isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      //T_rbt1St_CreateColumn(ZXC.Q4un, 2, isVisible, "Popust", "Popust"); 22.01.2014.
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,          isVisible, "StPdv"   , "Stopa PDV-a");
      R_cij_kcrp_CreateColumn      (ZXC.Q5un, 2,          isVisible, "CijSaPop", "Cijena sa popustom");

      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC", "Veleprodajna cijena");
      R_NC_CreateColumn(ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn(ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn(ZXC.Q4un, 2, false, "RUC", "RUC - razlika u cijeni");
      R_RUV_CreateColumn(ZXC.Q4un, 2, false, "RUV", "RUV - razlika u vrijednosti");

      R_KCRP_CreateColumn(ZXC.Q5un, 2, isVisible, "Iznos", "Iznos");

      //vvtbR_cij_uk.JAM_FieldExitMethod = new EventHandler(OnExitCijenaSpopustom);
      vvtbR_cij_uk.JAM_ShouldCalcTransAndSumGrid = true;
      
      //this.ControlForInitialFocus = TheG;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRMDUC2.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_malop, clr_Izlaz);
   }
}

public class IZMDUC_2            : IZMDUC
{
   #region Constructor

   public IZMDUC_2(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, true);

      //hamp_ciljaniMPC.Visible = true;

      hamp_osobaX .Location  = new Point(hamp_kupdobNaziv.Left            , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_projekt.Location  = new Point(hamp_kupdobNaziv.Left            , hamp_osobaX     .Bottom - ZXC.Qun4);
      hamp_Cjenik.Location   = new Point(hamp_projekt    .Right - ZXC.Qun2, hamp_osobaX     .Bottom - ZXC.Qun4);
      hamp_prjIdent.Location = new Point(hamp_kupdobNaziv.Left            , hamp_projekt    .Bottom           );
      hamp_napomena.Location = new Point(hamp_kupdobNaziv.Left            , hamp_prjIdent   .Bottom           );
      nextY = hamp_napomena.Bottom;
      
      hamp_osobaX  .Parent = TheTabControl.TabPages[0];
      hamp_prjIdent.Parent = TheTabControl.TabPages[0];
      panel_MigratorsLeftB.SendToBack();
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_osobaX, hamp_projekt, hamp_prjIdent,  
                                    hamp_dokDate, hamp_dokNum, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd, hamp_v1TT, hamp_v2TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_carinaKind,
                                    hamp_dostava,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/ hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_carinaKind,
                                        hampCbxM_dostava,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation
   
}

public class OdobrKupcuDUC       : FakturExtDUC
{
   #region Constructor

   public OdobrKupcuDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul)
      : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IOD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
   
      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_napomena, hamp_kupdobOther, hamp_Cjenik,
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB , hamp_osobaX, hamp_carinaKind,
                                    hamp_OpciA, hamp_OpciB,	hamp_externLink1, hamp_externLink2, hamp_prjIdent, hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB ,  hampCbxM_osobaX,hampCbxM_carinaKind,
                                        hampCbxM_OpciA, hampCbxM_OpciB	,hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent, hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn (ZXC.Q4un,              isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn    (ZXC.Q2un   ,           isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn      (ZXC.Q3un, 2,           isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn    (ZXC.Q2un   ,           isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn      (ZXC.Q4un, 4,           isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn   (ZXC.Q3un-ZXC.Qun4, 2,  isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn   (ZXC.Q2un, 0,           isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn      (ZXC.Q4un, 2,           isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn    (ZXC.Q2un, 0,           isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn(ZXC.QUN    ,           isVisible);

      R_NC_CreateColumn   (ZXC.Q4un, 2,     false, "NabCij"     , "Nabavna cijena");
      R_NV_CreateColumn   (ZXC.Q4un, 2,     false, "NabVri"     , "Nabavna vrijednost");
      R_RUC_CreateColumn  (ZXC.Q4un, 2,     false, "RUC"        , "RUC - razlika u cijeni");
      R_RUV_CreateColumn  (ZXC.Q4un, 2,     false, "RUV"        , "RUV - razlika u vrijednosti");
      R_KCRP_CreateColumn (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
 
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturOdobrKupDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, Color.Empty, clr_Izlaz);
   }
}

public class PovratKupcaDUC      : FakturExtDUC
{
   #region Constructor

   public PovratKupcaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IPV
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_napomena, hamp_kupdobOther,hamp_Cjenik,
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB , hamp_osobaX, hamp_carinaKind,
                                    hamp_OpciA, hamp_OpciB, hamp_externLink1, hamp_externLink2,hamp_prjIdent, hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_OpciA, hampCbxM_OpciB	, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn     (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_isIrmUsluga_CreateColumn(ZXC.QUN + ZXC.Qun4,  isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn     (ZXC.Q3un   ,          isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn     (ZXC.Q2un, 0,          isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,          isVisible);

      R_NC_CreateColumn (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn(ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn(ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");
      
      R_KCRP_CreateColumn (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPovKupDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }
}

public class OdobrDobavDUC       : FakturExtDUC
{
   #region Constructor

   public OdobrDobavDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UOD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
  
      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX, hamp_carinaKind, hamp_externLink1, hamp_externLink2, hamp_prjIdent,hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB	, hampCbxM_osobaX, hampCbxM_carinaKind, hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent,hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   ,          isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn     (ZXC.Q2un   ,          isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn     (ZXC.Q2un, 0,          isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,          isVisible);
      R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2,          isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturOdobrDobDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, Color.Empty, clr_Ulaz);
   }
}

public class PovratDobaDUC       : FakturExtDUC
{
   #region Constructor

   public PovratDobaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UPV
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
   
      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,hamp_carinaKind,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX, hamp_externLink1, hamp_externLink2,hamp_prjIdent,	hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_carinaKind,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_osobaX, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn      (ZXC.Q4un,                   isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                            isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,         isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q2un   ,                isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,                isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,       isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn        (ZXC.Q2un, 0,                isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,                isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,                isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn     (ZXC.QUN    ,                isVisible);
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 , 2,    isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
  
      
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPovDobDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }
}

public class ObvezPonudaDUC      : FakturExtDUC
{
   #region Constructor

   public ObvezPonudaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_OPN
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
      hamp_PonudDate.Location = new Point(hamp_DospDate.Left, hamp_DospDate.Bottom - ZXC.Qun4);
      hamp_RokPonude.Location = new Point(hamp_RokPlac .Left, hamp_DospDate.Bottom - ZXC.Qun4);

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd,
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PonudDate, hamp_RokPonude,hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,                      isVisible, "Šifra"        , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                           isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      T_KPD_CreateColumn       (ZXC.Q3un,                      isVisible, "KPD"         , "KPD");
      T_kol_CreateColumn       (ZXC.Q3un, 2,                   isVisible, "Kol"          , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,                   isVisible, "JM"           , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,                   isVisible, "Cijena"       , "Jedinična cijena");

      R_kolOP_CreateColumn     (ZXC.Q3un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn     (ZXC.Q4un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      T_kol2_CreateColumn      (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces,   isVisible, "IspKol"     , "Isporučena količina");
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      
      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn(ZXC.Q2un, 0, isVisible, "PdvSt", "Stopa PDV-a");
      R_KCRP_CreateColumn(ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturObavPonDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class PonudaDUC           : FakturExtDUC
{
   #region Constructor

   public PonudaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PON
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
     
      SetSumeHampers(false, true, true, false);

      hamp_PonudDate.Location = new Point(hamp_DospDate.Left, hamp_DospDate.Bottom - ZXC.Qun4);
      hamp_RokPonude.Location = new Point(hamp_RokPlac.Left, hamp_DospDate.Bottom - ZXC.Qun4);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd,
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PonudDate, hamp_RokPonude, hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent, hamp_DatumX,/*ovaj DatumX dodan 01.02.24.*///hamp_PonudDate, 08.11.2013. on je na osnovnom
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_DatumX, //hampCbxM_PonudDate, 08.11.2013 on je na sonovnom
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible       = true;
      bool isOrgPakVisible = ZXC.RRD.Dsc_IsOrgPakVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un,                  isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                       isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_KPD_CreateColumn       (ZXC.Q3un,                  isVisible, "KPD"         , "KPD");
      T_kol_CreateColumn       (ZXC.Q3un, 2,               isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,               isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,               isVisible, "Cijena"     , "Jedinična cijena");

      R_kolOP_CreateColumn(ZXC.Q3un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn(ZXC.Q4un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
  
      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn(ZXC.Q2un, 0, isVisible, "PdvSt", "Stopa PDV-a");
      R_KCRP_CreateColumn(ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPonudaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class PonMalDUC           : FakturExtDUC
{
   #region Constructor

   public PonMalDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PNM
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
     
      SetSumeHampers(false, true, true, false);
      hamp_ciljaniMPC.Visible = true;

      hamp_PonudDate.Location = new Point(hamp_DospDate.Left, hamp_DospDate.Bottom - ZXC.Qun4);
      hamp_RokPonude.Location = new Point(hamp_RokPlac.Left, hamp_DospDate.Bottom - ZXC.Qun4);

      hamp_ciljaniMPC.Visible = true;

      hamp_S_ppmv.Visible  =! ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_ppmv.Location = new Point(hamp_S_ukPdv.Left, 0);

      hamp_S_pnp_IZL.Visible  = ZXC.RRD.Dsc_IsPnpStVisible;
      hamp_S_pnp_IZL.Location = new Point(hamp_S_ukPdv.Left, 0);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd,
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PonudDate, hamp_RokPonude, hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2, hamp_somePercent,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2, hampCbxM_somePercent,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPkVisible = ZXC.RRD.Dsc_IsPKvisible; //20.11.2013. zbog umjetnina

      T_artiklCD_CreateColumn      (ZXC.Q4un,                  isVisible, "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                           isVisible, "Naziv"   , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un,isVisible, "KPD"         , "KPD");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,        isVisible, "Usl"     , "Usluga");
      T_kol_CreateColumn           (ZXC.Q3un, 2,               isVisible, "Kol"     , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,               isVisible, "JM"      , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,               isVisible, "Cijena"  , "Jedinična cijena");
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2,      isVisible, "Popust"  , "Popust");
      T_pdvSt_CreateColumn         (ZXC.Q2un, 0,               isVisible, "StPdv"   , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN    ,               isPkVisible);
      R_cij_kcrp_CreateColumn      (ZXC.Q5un, 2,               isVisible, "CijSaPop", "Cijena sa popustom");

      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      R_KCRP_CreateColumn(ZXC.Q5un, 2, isVisible, "Iznos", "Iznos");

      T_ppmvOsn_CreateColumn  (ZXC.Q5un, 2, false, "Kataloška cijena", "Osnovica za obračun posebnog poreza na motorna vozila", true);
      T_ppmvSt1i2_CreateColumn(ZXC.Q3un, 1, false, "ZbirSt"          , "Zbirna stopa poreza na motorna vozila");
      R_ppmvIzn_CreateColumn  (ZXC.Q3un, 2, false, "PPMV iznos"      , "Iznos posebnog poreza na motorna vozila");

      vvtbR_cij_uk.JAM_FieldExitMethod = new EventHandler(OnExitCijenaSpopustom);
      vvtbR_cij_uk.JAM_ShouldCalcTransAndSumGrid = true;
      
      //this.ControlForInitialFocus = TheG;
   }


   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPonMalDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class ReversDUC           : FakturExtDUC
{
   #region Constructor

   public ReversDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RVI
         });
   }

   #endregion Constructor
 
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PonudDate, hamp_kupdobOther,hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,                       isVisible, "Šifra"    , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                isVisible, "Naziv"    , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz, "Šarža/LOT", "Broj Šarže/Lota");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                    isVisible, "Kol"      , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,                       isVisible, "JM"       , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, Color.Empty);
   }
}

public class PovReversaDUC       : FakturExtDUC
{
   #region Constructor

   public PovReversaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RVU
         });
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfhampers();
      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_kupdobOther,hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_osobaX,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,                       isVisible, "Šifra"    , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                isVisible, "Naziv"    , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz, "Šarža/LOT", "Broj Šarže/Lota");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                    isVisible, "Kol"      , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,                       isVisible, "JM"       , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPrimkaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, Color.Empty);
   }
}

public class NarudzbaDobavDUC    : FakturExtDUC
{
   #region Constructor

   public NarudzbaDobavDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_kupdobOther,hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX, 
                                    hamp_dostava, hamp_externLink1, hamp_externLink2,hamp_prjIdent,	hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_dostava, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      if(ZXC.IsSvDUH)
      {
         T_artiklCD_CreateColumn      (ZXC.Q4un           ,    isVisible, "Šifra"      , "Šifra artikla");
         T_artiklName_CreateColumnFill(isVisible          ,               "Naziv"      , "Naziv artikla ili proizvoljan opis");
         R_ORG_T_doCijMal_CreateColumn(ZXC.Q4un           , 0, isVisible, "ORG"        , "Originalno pakiranje");
         R_BOP_CreateColumn           (ZXC.Q4un           , 2, isVisible, "BOP"        , "Broj Originalnog pakiranja");
         R_COP_CreateColumn           (ZXC.Q4un           , 2, isVisible, "COP"        , "Cijena Originalnog pakiranja");
         T_kol_CreateColumn           (ZXC.Q3un           , 2, isVisible, "Kol"        , "Količina");
         T_cij_CreateColumn           (ZXC.Q4un           , 4, isVisible, "Cijena"     , "Jedinična cijena");
         T_rbt1St_CreateColumn        (ZXC.Q3un - ZXC.Qun4, 2, isVisible, "Rb"         , "Stopa rabata 1");
         T_pdvSt_CreateColumn         (ZXC.Q2un           , 0, isVisible, "PdvSt"      , "Stopa PDV-a");
         R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
      }
      else
      { 
          T_artiklCD_CreateColumn(ZXC.Q4un,                                   isVisible, "Šifra"      , "Šifra artikla"                     );
          T_artiklName_CreateColumnFill(                                       isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
          T_kol_CreateColumn      (ZXC.Q3un, 2,                                isVisible, "Kol"        , "Količina"      );
          T_jedMj_CreateColumn    (ZXC.Q2un,                                   isVisible, "JM"         , "Jedinica mjere");
          T_cij_CreateColumn      (ZXC.Q4un, 4,                                isVisible, "Cijena"     , "Jedinična cijena");
          T_rbt1St_CreateColumn   (ZXC.Q3un - ZXC.Qun4, 2,                     isVisible, "Rb1"        , "Stopa rabata 1");
    //    T_rbt2St_CreateColumn   (ZXC.Q2un, 0,                                isVisible, "Rb2"        , "Stopa rabata 2");
          R_KCR_CreateColumn      (ZXC.Q4un, 2,                                isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
          T_kol2_CreateColumn     (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces,isVisible, "IspKol"     , "Isporučena količina");
      }
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturNarDobDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class NarudzDobUvozDUC    : FakturExtDUC
{
   #region Constructor

   public NarudzDobUvozDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRU
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_kupdobOther,hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX, 
                                    hamp_dostava, hamp_externLink1, hamp_externLink2,hamp_prjIdent,	hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme, hampCbxM_osobaX, 
                                        hampCbxM_dostava, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn (ZXC.Q4un,                                    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                        isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn      (ZXC.Q3un, 2,                                 isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn    (ZXC.Q2un,                                    isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn      (ZXC.Q4un, 4,                                 isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn   (ZXC.Q3un - ZXC.Qun4, 2,                      isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn   (ZXC.Q2un, 0,                                 isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn      (ZXC.Q4un, 2,                                 isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_kol2_CreateColumn     (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces, isVisible, "IspKol"     , "Isporučena količina");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturNarDobUvDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class NarudzDobUslugaDUC  : FakturExtDUC
{
   #region Constructor

   public NarudzDobUslugaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRS
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_kupdobOther,hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX, 
                                    hamp_dostava, hamp_externLink1, hamp_externLink2,hamp_prjIdent,	hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme, hampCbxM_osobaX, 
                                        hampCbxM_dostava, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn (ZXC.Q4un,                                   isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                       isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn      (ZXC.Q3un, 2,                                isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn    (ZXC.Q2un,                                   isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn      (ZXC.Q4un, 4,                                isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn   (ZXC.Q3un - ZXC.Qun4, 2,                     isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn   (ZXC.Q2un, 0,                                isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn      (ZXC.Q4un, 2,                                isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_kol2_CreateColumn     (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces,isVisible, "IspKol"     , "Isporučena količina");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturNarDobUslDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class NarudzbaKupcaDUC    : FakturExtDUC
{
   #region Constructor

   public NarudzbaKupcaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRK
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                     hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_kupdobOther,hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_napomena, hamp_Cjenik, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_OpciA, hamp_OpciB,	hamp_osobaX, hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_osobaX, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,                                    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2                                , isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un                                   , isVisible, "JM"         , "Jedinica mjere");
      T_kol2_CreateColumn      (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces, isVisible, "IspKol"     , "Isporučena količina");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturNarKupDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class NRMDUC              : FakturExtDUC
{
   #region Constructor

   public NRMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRM
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate,  hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, 
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn (ZXC.Q4un,               isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                   isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn      (ZXC.Q3un, 2,            isVisible, "Kol"   , "Količina"      );
      T_jedMj_CreateColumn    (ZXC.Q2un,               isVisible, "JM"    , "Jedinica mjere");
      T_cij_CreateColumn      (ZXC.Q4un, 4,            isVisible, "Cijena", "Jedinična cijena");
      T_rbt1St_CreateColumn   (ZXC.Q3un - ZXC.Qun4, 2, isVisible, "Rb1"   , "Stopa rabata 1");
      R_KCR_CreateColumn      (ZXC.Q4un, 2,            isVisible, "Iznos" , "");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturNRMDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class BlgUplatDUC         : FakturExtDUC
{
   #region Constructor

   public BlgUplatDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UPL
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, false, false);

   }
 
   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, /*hamp_ValName ,*/ hamp_Status  , hamp_vezniDok, //hamp_projekt, 
                                    hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  //, hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_VezniDok2,
                                    hamp_externLink1, hamp_externLink2, hamp_carinaKind,
                                    hamp_osobaA, hamp_OsobaB , hamp_osobaX,
                                    //hamp_OpciA, hamp_OpciB,  hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_VezniDok2,
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_carinaKind,
                                        hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_osobaX,
                                        //hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible,                     "Šifra"   , "Šifra artikla");
      T_artiklName_CreateColumnFill(             isVisible,                     "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_konto_CreateColumn         (ZXC.Q4un   , isVisible,                     "Konto"   , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD" , "Šifra Mjesta troška" );
      R_mtros_tk_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška", "Tiker Mjesta troška" );
      T_jedMj_CreateColumn         (ZXC.Q4un   , isVisible,                     "Račun"   , "Broj računa");
      T_kol_CreateColumn           (ZXC.Q3un, 0, isVisible,                     "Kol"     , "Količina");
      T_cij_CreateColumn           (ZXC.Q5un, 4, isVisible,                     "Cijena"  , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q5un, 2, isVisible,                     "Iznos"   , "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturBlgUplDUC.MigratorStates; }
   }

   #endregion overrideMigratorList
  
}

public class BlgUplat_M_DUC      : FakturExtDUC
{
   #region Constructor

   public BlgUplat_M_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            (ZXC.RRD.Dsc_IsCashFakturToBlagajna ? Faktur.TT_ABU : Faktur.TT_BUP)
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, false, false);

      hamp_ValName     .Visible =
      hamp_IznosUvaluti.Visible = !ZXC.IsTETRAGRAM_ANY;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_ValName, hamp_Status  , hamp_vezniDok, //hamp_projekt, 
                                    hamp_dokDate    , hamp_dokNum , hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT   , hamp_v2TT   
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_VezniDok2,
                                    hamp_externLink1, hamp_externLink2, hamp_carinaKind,
                                    hamp_osobaA, hamp_OsobaB , hamp_osobaX,
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_VezniDok2,
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_carinaKind,
                                        hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_osobaX,
                                      };

      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible                    , "Šifra", "Šifra artikla");
      T_artiklName_CreateColumnFill(isVisible,    "Naziv"                     , "Naziv artikla ili proizvoljan opis");
      T_konto_CreateColumn         (ZXC.Q4un,    isVisible                    , "Konto", "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un,    ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD", "Šifra Mjesta troška");
      R_mtros_tk_CreateColumn      (ZXC.Q3un,    ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška", "Tiker Mjesta troška");
      T_jedMj_CreateColumn         (ZXC.Q4un,    isVisible                    , "Račun", "Broj računa");
      T_kol_CreateColumn           (ZXC.Q3un, 0, isVisible                    , "Kol", "Količina");
      T_cij_CreateColumn           (ZXC.Q5un, 4, isVisible                    , "Cijena", "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q5un, 2, isVisible                    , "Iznos", "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturBlgUplMDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

}

public class BlgIsplatDUC        : FakturExtDUC
{
   #region Constructor

   public BlgIsplatDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_ISP
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, false, false);

   }
  
   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, /*hamp_ValName ,*/ hamp_Status  , hamp_vezniDok, //hamp_projekt, 
                                    hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  //, hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_VezniDok2,
                                    hamp_externLink1, hamp_externLink2, hamp_carinaKind,
                                    hamp_osobaA, hamp_OsobaB , hamp_osobaX,
                                    //hamp_OpciA, hamp_OpciB,  hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_VezniDok2,
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_carinaKind,
                                        hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_osobaX,
                                        //hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_konto_CreateColumn         (ZXC.Q4un   , isVisible, "Konto" , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD"    , "Šifra Mjesta troška"               );
      R_mtros_tk_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška"   , "Tiker Mjesta troška"               );
      T_jedMj_CreateColumn         (ZXC.Q4un   , isVisible, "Račun" , "Broj računa");
      T_kol_CreateColumn           (ZXC.Q3un, 0, isVisible, "Kol"   , "Količina"      );
      T_cij_CreateColumn           (ZXC.Q5un, 4, isVisible, "Cijena", "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q5un, 2, isVisible, "Iznos" , "Iznos");
 }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturBlgIspDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

}

public class BlgIsplat_M_DUC     : FakturExtDUC
{
   #region Constructor

   public BlgIsplat_M_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            (ZXC.RRD.Dsc_IsCashFakturToBlagajna ? Faktur.TT_ABI : Faktur.TT_BIS)
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, false, false);

      hamp_ValName     .Visible = 
      hamp_IznosUvaluti.Visible = !ZXC.IsTETRAGRAM_ANY;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_ValName , hamp_Status  , hamp_vezniDok, //hamp_projekt, 
                                    hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT  , hamp_v2TT   
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_VezniDok2,
                                    hamp_externLink1, hamp_externLink2, hamp_carinaKind,
                                    hamp_osobaA, hamp_OsobaB , hamp_osobaX,
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_VezniDok2,
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_carinaKind,
                                        hampCbxM_OsobaA, hampCbxM_osobaB, hampCbxM_osobaX,
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_konto_CreateColumn         (ZXC.Q4un   , isVisible, "Konto" , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD"    , "Šifra Mjesta troška"               );
      R_mtros_tk_CreateColumn      (ZXC.Q3un   , ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška"   , "Tiker Mjesta troška"               );
      T_jedMj_CreateColumn         (ZXC.Q4un   , isVisible, "Račun" , "Broj računa");
      T_kol_CreateColumn           (ZXC.Q3un, 0, isVisible, "Kol"   , "Količina"      );
      T_cij_CreateColumn           (ZXC.Q5un, 4, isVisible, "Cijena", "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q5un, 2, isVisible, "Iznos" , "Iznos");
 }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturBlgIspMDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

}

public class RNPDUC              : FakturExtDUC
{
   #region Constructor

   public RNPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul)
      : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RNP
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);

      hamp_decimal.Parent = TheTabControl.TabPages[1];
      hamp_decimal.Visible = true;
      hamp_decimal.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_prjArtName, hamp_VezniDok2, hamp_vezniDok, hamp_osobaX, hamp_OpciA,
                                    hamp_dokDate    , hamp_DospDate,hamp_PonudDate, hamp_RokPlac, hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT,  hamp_someMoney, hamp_somePercent, hamp_Status    , hamp_napomena
                                   };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_osobaA ,hamp_OsobaB ,
                                    hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_OsobaA,hampCbxM_osobaB,
                                        hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,    isVisible, "JM"         , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturRNpDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }
}

public class RNSDUC              : FakturExtDUC
{
   #region Constructor

   public RNSDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul)
      : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RNS
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {

      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_prjArtName, hamp_VezniDok2, hamp_vezniDok, hamp_osobaX, hamp_OpciA,
                                    hamp_dokDate    , hamp_DospDate,hamp_PonudDate, hamp_RokPlac, hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT,  hamp_someMoney, hamp_somePercent, hamp_Status    , hamp_napomena
                                   };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_osobaA ,hamp_OsobaB ,
                                    hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_OsobaA,hampCbxM_osobaB,
                                        hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,    isVisible, "JM"         , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturRNsDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }
}

public class PRJDUC              : FakturExtDUC
{
   #region Constructor

   public PRJDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul)
      : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PRJ
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_prjArtName, hamp_VezniDok2, hamp_vezniDok, hamp_osobaX, hamp_OpciA,
                                    hamp_dokDate    , hamp_DospDate,hamp_PonudDate, hamp_RokPlac, hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT,  hamp_someMoney, hamp_somePercent, hamp_Status    , hamp_napomena
                                   };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_osobaA ,hamp_OsobaB ,
                                    hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_OsobaA,hampCbxM_osobaB,
                                        hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un,    isVisible, "JM"         , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPRjDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }
}

public class RNMDUC              : FakturExtDUC, IVvRealizableFakturDUC
{
   
   #region Fieldz

   public VvTextBox tbx_ukFinIzlaz, tbx_ukFinUlaz, tbx_RNM_StupanjDovrsenostiPG, tbx_ukKolUlaz, tbx_finDiff, tbx_ncPerUlKol, tbx_diffAKS, tbx_RNM_StupanjDovrsenosti, tbx_RNM_StupanjDovrsenostiTwin,
                    tbx_RNM_RealStupDovrsenosti, tbx_RNM_RealStupDovrsenostiTwin,
                    tbx_ncPerUlKolKg, tbx_ukKolKgUlazOG, tbx_RNM_StupanjDovrsenostiOG,
                    tbx_ukFinIzlazPG,tbx_ukKolOpUlazPG, tbx_finAIndir,
                    tbx_ukFinNedovrIzlPG, tbx_ukFinIzlazOG ;

   public VvHamper  hamp_SumeRealRNM, hamp_diffAKS, hamp_stupDovrsen, hamp_SumeRealRNMAndPg, hamp_stupDovrTwin, hamp_pg;
   public Label     lbl_donosPG, lbl_ukFinIzlazPG,lbl_ukKolOpUlazPG, lbl_stDovPG, lbl_finNedovrPG,
      lbl_aog,
      lbl_sog,
      lbl_auk,
      lbl_bog,
      lbl_cog,
      lbl_aB ,
      lbl_cA 
      
      
      ;

   #endregion Fieldz

   #region Constructor

   public RNMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RNM
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
      CreateHamperSume_Realizacija();
      CreateLabel_StupanjDovrsenosti();
      CreateLabel_StupanjDovrsenostiTwin();
      CreateHamperDiff_AKS();
      CreateHamperPG();

      ThePolyGridTabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(IfShould_Load_RealizacGrid);
      hamp_IznosUvaluti.Visible = false;

      hamp_diffAKS.Location = new Point(TheG.Right - hamp_diffAKS.Width + ZXC.Qun4, 0);
      hamp_diffAKS.Anchor = AnchorStyles.Top | AnchorStyles.Right;

      //*****Lokacija**********************27.01.2017.

      //hamp_stupDovrsen.Location = new Point(RNM_Grid.Right - hamp_stupDovrsen.Width + ZXC.Qun4, nextY);

      RealizacGrid.Location = new Point(ZXC.QunMrgn, ZXC.Q2un);
      RealizacGrid.Size     = new Size(RealizacGrid.Parent.Width - 2*ZXC.QunMrgn, RealizacGrid.Parent.Height - hamp_SumeRealRNM.Height - ZXC.Q2un);

      RealizacGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

      hamp_stupDovrTwin.Location = new Point(RealizacGrid.Left, ZXC.Qun8);
      hamp_stupDovrTwin.Anchor   = AnchorStyles.Top | AnchorStyles.Left;

      hamp_SumeRealRNM.Location = new Point(RealizacGrid.Right - hamp_SumeRealRNM.Width + ZXC.Qun4, RealizacGrid.Bottom);
      hamp_SumeRealRNM.Anchor   = AnchorStyles.Bottom | AnchorStyles.Right;

      hamp_pg.Location = new Point(RealizacGrid.Right - hamp_pg.Width, ZXC.Qun8);
      hamp_pg.Anchor   = AnchorStyles.Top | AnchorStyles.Left;

      //*****************************

   }
   
   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);

      hamp_decimal.Parent = TheTabControl.TabPages[1];
      hamp_decimal.Visible = true;
      hamp_decimal.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamp_sumeRNM.BringToFront();

      hamp_RokPlac.BringToFront();
   }

   private void CreateArrOfHampers()
   {

      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd, hamp_sklad2Cd,
                                    hamp_VezniDok2, hamp_vezniDok, hamp_osobaX, hamp_OpciA, hamp_prjIdent, hamp_projekt,
                                    hamp_dokDate    , hamp_DospDate,hamp_PonudDate, hamp_RokPlac, hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT,  hamp_v3TT  , hamp_v4TT, hamp_Status    , hamp_napomena/*, hamp_someMoney*/// 24.11.2016. treba li im uopce prodajna cijena ovdje?
                                   };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_osobaA ,hamp_OsobaB ,
                                    hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_OsobaA,hampCbxM_osobaB,
                                        hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2,
                                        hampCbxM_opis                                   
                                      };
   }

   private void CreateHamperSume_Realizacija()
   {
      hamp_SumeRealRNM = new VvHamper(14, 3, "", ThePolyGridTabControl.TabPages[3], false);
      //                                                         0          1              2              3                4             5               6              7               8            9           10       11               12            13   
      hamp_SumeRealRNM.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun2, ZXC.Q4un, ZXC.Q4un - ZXC.Qun2, ZXC.Q4un, ZXC.Q8un + ZXC.Qun2, ZXC.Q4un, ZXC.Q4un - ZXC.Qun2, ZXC.Q4un, ZXC.Q4un- ZXC.Qun2, ZXC.Q4un , ZXC.Q6un, ZXC.Q4un, ZXC.Q5un - ZXC.Qun4, ZXC.Q4un };
      hamp_SumeRealRNM.VvSpcBefCol   = new int[] { ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12,            ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 , ZXC.Qun12, ZXC.Qun12, ZXC.Qun12, ZXC.Qun12 };
      hamp_SumeRealRNM.VvRightMargin = hamp_SumeRealRNM.VvLeftMargin;

      for(int i = 0; i < hamp_SumeRealRNM.VvNumOfRows; i++)
      {
         hamp_SumeRealRNM.VvRowHgt[i]    = ZXC.QUN;
         hamp_SumeRealRNM.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamp_SumeRealRNM.VvBottomMargin = hamp_SumeRealRNM.VvTopMargin;

      lbl_aog = hamp_SumeRealRNM.CreateVvLabel( 0, 0, "PPR Fin(Aog):"                                   , ContentAlignment.MiddleRight);
      lbl_sog = hamp_SumeRealRNM.CreateVvLabel( 2, 0, "StDovr(Sog):"                                    , ContentAlignment.MiddleRight);
      lbl_auk = hamp_SumeRealRNM.CreateVvLabel( 4, 0, "PPR FinUk(Auk=(Apg+Aog)*Sog)):"                  , ContentAlignment.MiddleRight);
      lbl_bog = hamp_SumeRealRNM.CreateVvLabel( 6, 0, "PIP " + ZXC.RRD.Dsc_OrgPakText + "(Bog):"        , ContentAlignment.MiddleRight);
      lbl_cog = hamp_SumeRealRNM.CreateVvLabel( 8, 0, "PIP Fin(Cog):"                                   , ContentAlignment.MiddleRight);
      lbl_aB  = hamp_SumeRealRNM.CreateVvLabel(10, 0, "PIP Cij " + ZXC.RRD.Dsc_OrgPakText + "(Auk/Bog):", ContentAlignment.MiddleRight);
      lbl_cA  = hamp_SumeRealRNM.CreateVvLabel(12, 0, "Razlika(Cog-Auk):"                               , ContentAlignment.MiddleRight);

      tbx_ukFinIzlazOG             = hamp_SumeRealRNM.CreateVvTextBox( 1, 0, "tbx_ukFinIzlazOG" , "ukFinIzlazOG"          , 12);
      tbx_RNM_StupanjDovrsenostiOG = hamp_SumeRealRNM.CreateVvTextBox( 3, 0, "tbx_stDovOG"      , "ukFinNedovrIzlPG"      , 12);
      tbx_ukFinIzlaz               = hamp_SumeRealRNM.CreateVvTextBox( 5, 0, "tbx_ukFinIzlaz"   , "UkFinIzlaz"            , 12);
      tbx_ukKolKgUlazOG            = hamp_SumeRealRNM.CreateVvTextBox( 7, 0, "tbx_ukKolKgUlazOG", "UkKolUlaz PodJm"       , 12);
      tbx_ukFinUlaz                = hamp_SumeRealRNM.CreateVvTextBox( 9, 0, "tbx_ukFinUlaz"    , "UkFinUlaz"             , 12);
      tbx_ncPerUlKolKg             = hamp_SumeRealRNM.CreateVvTextBox(11, 0, "tbx_ncPerUlKolOp" , "IzlazFin/UlazKol PodJm", 12);
      tbx_finDiff                  = hamp_SumeRealRNM.CreateVvTextBox(13, 0, "tbx_finDiff"      , "FinRazlika"            , 12);
      tbx_RNM_StupanjDovrsenostiOG.JAM_IsForPercent = true;

            
      //**** za sada samo visibiliti jer mi fldovi idu u izvj
      tbx_ukKolUlaz                = hamp_SumeRealRNM.CreateVvTextBox( 1, 1, "tbx_ukKolUlaz"   , "UkKolUlaz"             , 12);
      tbx_ncPerUlKol               = hamp_SumeRealRNM.CreateVvTextBox( 1, 1, "tbx_ncPerUlKol"  , "IzlazFin/UlazKol"      , 12);
      tbx_ukKolUlaz   .Visible = 
      tbx_ncPerUlKol  .Visible =     false;
      //**** za sada samo visibiliti jer mi fldovi idu u izvj

      tbx_ukFinIzlazOG             .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukFinIzlaz               .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukKolUlaz                .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ncPerUlKol               .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukFinUlaz                .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_finDiff                  .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_StupanjDovrsenostiOG .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukKolKgUlazOG            .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ncPerUlKolKg             .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_ukFinIzlazOG            .JAM_ReadOnly = true;
      tbx_RNM_StupanjDovrsenostiOG.JAM_ReadOnly = true;
      tbx_ukFinIzlaz              .JAM_ReadOnly = true;
      tbx_ukKolUlaz               .JAM_ReadOnly = true;
      tbx_ncPerUlKol              .JAM_ReadOnly = true;
      tbx_ukFinUlaz               .JAM_ReadOnly = true;
      tbx_finDiff                 .JAM_ReadOnly = true;
      tbx_ukKolKgUlazOG           .JAM_ReadOnly = true;
      tbx_ncPerUlKolKg            .JAM_ReadOnly = true;

      tbx_finDiff      .JAM_ForeColor = Color.Red;

      tbx_ukKolUlaz    .JAM_BackColor = 
      tbx_ncPerUlKol   .JAM_BackColor = 
      tbx_ukFinUlaz    .JAM_BackColor = 
      tbx_ukKolKgUlazOG.JAM_BackColor =  Color.PaleGreen;

      VvHamper.Open_Close_Fields_ForWriting(hamp_SumeRealRNM, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void CreateHamperDiff_AKS()
   {
      hamp_diffAKS = new VvHamper(2, 1, "", ThePolyGridTabControl.TabPages[0], false);
      //                                           0        1      
      hamp_diffAKS.VvColWdt      = new int[] { ZXC.Q10un*2, ZXC.Q3un };
      hamp_diffAKS.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamp_diffAKS.VvRightMargin = hamp_diffAKS.VvLeftMargin;

      for(int i = 0; i < hamp_diffAKS.VvNumOfRows; i++)
      {
         hamp_diffAKS.VvRowHgt[i]    = ZXC.QUN;
         hamp_diffAKS.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamp_diffAKS.VvBottomMargin = hamp_SumeRealRNM.VvTopMargin;

                    hamp_diffAKS.CreateVvLabel  (0, 0, "Razlika Nalog / Realizacija (A - artikli, K - količine, S - RGC/LOT):", ContentAlignment.MiddleRight);
      tbx_diffAKS = hamp_diffAKS.CreateVvTextBox(1, 0, "tbx_diffAKS", "", 12);
      tbx_diffAKS.JAM_Highlighted = true;
      //tbx_diffAKS.JAM_BackColor   = Color.Tomato;
      VvHamper.Open_Close_Fields_ForWriting(hamp_diffAKS, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void CreateLabel_StupanjDovrsenosti()
   {
      hamp_stupDovrsen = new VvHamper(4, 1, "", ThePolyGridTabControl.TabPages[0], false, 0, 0, 0);
      //                                           0        1      
      hamp_stupDovrsen.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un, ZXC.Q6un, ZXC.Q3un };
      hamp_stupDovrsen.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamp_stupDovrsen.VvRightMargin = hamp_stupDovrsen.VvLeftMargin;

      for(int i = 0; i < hamp_stupDovrsen.VvNumOfRows; i++)
      {
         hamp_stupDovrsen.VvRowHgt[i]    = ZXC.QUN;
         hamp_stupDovrsen.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamp_stupDovrsen.VvBottomMargin = hamp_SumeRealRNM.VvTopMargin;

                                   hamp_stupDovrsen.CreateVvLabel  (0, 0, "StDovršenosti KNJ:", ContentAlignment.MiddleRight);
      tbx_RNM_StupanjDovrsenosti = hamp_stupDovrsen.CreateVvTextBox(1, 0, "tbx_stDovr", "", 12);
      tbx_RNM_StupanjDovrsenosti.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_StupanjDovrsenosti.JAM_IsForPercent = true;

                                    hamp_stupDovrsen.CreateVvLabel  (2, 0, "StDovršenosti realan:", ContentAlignment.MiddleRight);
      tbx_RNM_RealStupDovrsenosti = hamp_stupDovrsen.CreateVvTextBox(3, 0, "tbx_RNM_RealStupDovrsenosti", "", 12);
      tbx_RNM_RealStupDovrsenosti.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_RealStupDovrsenosti.JAM_IsForPercent = true;

      VvHamper.Open_Close_Fields_ForWriting(hamp_stupDovrsen, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

   }

   private void CreateLabel_StupanjDovrsenostiTwin()
   {
      hamp_stupDovrTwin = new VvHamper(6, 1, "", ThePolyGridTabControl.TabPages[3], false);
      //                                           0        1      
      hamp_stupDovrTwin.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun8, ZXC.Q3un, ZXC.Q3un - ZXC.Qun8, ZXC.Q3un, ZXC.Q3un, ZXC.Q4un };
      hamp_stupDovrTwin.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamp_stupDovrTwin.VvRightMargin = hamp_stupDovrTwin.VvLeftMargin;

      for(int i = 0; i < hamp_stupDovrTwin.VvNumOfRows; i++)
      {
         hamp_stupDovrTwin.VvRowHgt[i]    = ZXC.QUN;
         hamp_stupDovrTwin.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamp_stupDovrTwin.VvBottomMargin = hamp_stupDovrTwin.VvTopMargin;

                                       hamp_stupDovrTwin.CreateVvLabel  (0, 0, "StDovrKNJ:", ContentAlignment.MiddleRight);
      tbx_RNM_StupanjDovrsenostiTwin = hamp_stupDovrTwin.CreateVvTextBox(1, 0, "tbx_stDovrTwin", "", 12);
      tbx_RNM_StupanjDovrsenostiTwin.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_StupanjDovrsenostiTwin.JAM_IsForPercent = true;
                         
                                        hamp_stupDovrTwin.CreateVvLabel  (2, 0, "StDovrUK:", ContentAlignment.MiddleRight);
      tbx_RNM_RealStupDovrsenostiTwin = hamp_stupDovrTwin.CreateVvTextBox(3, 0, "tbx_RNM_RealStupDovrsenostiTwin", "", 12);
      tbx_RNM_RealStupDovrsenostiTwin.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_RealStupDovrsenostiTwin.JAM_IsForPercent = true;

                      hamp_stupDovrTwin.CreateVvLabel(4, 0, "Aindir:", ContentAlignment.MiddleRight);
      tbx_finAIndir = hamp_stupDovrTwin.CreateVvTextBox        (5, 0, "tbx_aIndir", "AIndir", 12);
      tbx_finAIndir.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_finAIndir.JAM_ReadOnly = true;
      tbx_finAIndir.JAM_ForeColor = Color.Blue;

      VvHamper.Open_Close_Fields_ForWriting(hamp_stupDovrTwin, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
   }

   private void CreateHamperPG()
   {
      hamp_pg = new VvHamper(9, 2, "", ThePolyGridTabControl.TabPages[3], false);
      //                                        0        1               2             3          4            5               6           7       8                9         10     
      hamp_pg.VvColWdt      = new int[] { ZXC.Q3un - ZXC.Qun2, ZXC.Q5un, ZXC.Q4un, ZXC.Q3un + ZXC.Qun2, ZXC.Q4un, ZXC.Q8un, ZXC.Q4un, ZXC.Q3un -ZXC.Qun2, ZXC.Q4un };
      hamp_pg.VvSpcBefCol   = new int[] {            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,            ZXC.Qun8, ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,           ZXC.Qun8, ZXC.Qun8 };
      hamp_pg.VvRightMargin = hamp_pg.VvLeftMargin;

      for(int i = 0; i < hamp_pg.VvNumOfRows; i++)
      {
         hamp_pg.VvRowHgt[i]    = ZXC.QUN;
         hamp_pg.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamp_pg.VvBottomMargin = hamp_pg.VvTopMargin;

      lbl_donosPG       = hamp_pg.CreateVvLabel  (0, 0, "IZ PG:"                                  , ContentAlignment.MiddleRight);
      lbl_ukFinIzlazPG  = hamp_pg.CreateVvLabel  (1, 0, "PPR FinUk(ApgUk):"                       , ContentAlignment.MiddleRight);
      lbl_stDovPG       = hamp_pg.CreateVvLabel  (3, 0, "StDovr(Spg):"                            , ContentAlignment.MiddleRight);
      lbl_finNedovrPG   = hamp_pg.CreateVvLabel  (5, 0, "FinNedv(Apg=ApgUk*(1-Spg)):"             , ContentAlignment.MiddleRight);
      lbl_ukKolOpUlazPG = hamp_pg.CreateVvLabel  (7, 0, "PIP " + ZXC.RRD.Dsc_OrgPakText + ":", ContentAlignment.MiddleRight);
      lbl_donosPG.Font = ZXC.vvFont.SmallBoldFont;

      tbx_ukFinIzlazPG             = hamp_pg.CreateVvTextBox(2, 0, "tbx_ukFinIzlazPG"    , "ukFinIzlazPG "   , 12);
      tbx_RNM_StupanjDovrsenostiPG = hamp_pg.CreateVvTextBox(4, 0, "tbx_ukKolIzlaz"      , "UkKolIzlaz"      , 12);
      tbx_ukFinNedovrIzlPG         = hamp_pg.CreateVvTextBox(6, 0, "tbx_ukFinNedovrIzlPG", "ukFinNedovrIzlPG", 12);
      tbx_ukKolOpUlazPG            = hamp_pg.CreateVvTextBox(8, 0, "tbx_ukKolOpUlazPG"   , "ukKolOpUlazPG"   , 12);
      tbx_RNM_StupanjDovrsenostiPG.JAM_IsForPercent = true;

      tbx_ukFinIzlazPG            .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukKolOpUlazPG           .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_RNM_StupanjDovrsenostiPG.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
      tbx_ukFinNedovrIzlPG        .JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      tbx_ukFinIzlazPG            .JAM_ReadOnly = true;
      tbx_ukKolOpUlazPG           .JAM_ReadOnly = true;
      tbx_RNM_StupanjDovrsenostiPG.JAM_ReadOnly = true;
      tbx_ukFinNedovrIzlPG        .JAM_ReadOnly = true;

      tbx_ncPerUlKolKg .JAM_BackColor = 
      tbx_ukKolOpUlazPG.JAM_BackColor = Color.PaleGreen;

      VvHamper.Open_Close_Fields_ForWriting(hamp_pg, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

   }

   public decimal Fld_ukFinIzlazUK                   { get { return tbx_ukFinIzlaz                 .GetDecimalField(); } set { tbx_ukFinIzlaz                    .PutDecimalField(value); } }
   public decimal Fld_ukFinUlazOG                    { get { return tbx_ukFinUlaz                  .GetDecimalField(); } set { tbx_ukFinUlaz                     .PutDecimalField(value); } }
   public decimal Fld_RNM_StupanjDovrsenostiPG       { get { return tbx_RNM_StupanjDovrsenostiPG   .GetDecimalField(); } set { tbx_RNM_StupanjDovrsenostiPG      .PutDecimalField(value); } }
   public decimal Fld_ukKolUlazOG                    { get { return tbx_ukKolUlaz                  .GetDecimalField(); } set { tbx_ukKolUlaz                     .PutDecimalField(value); } }
   public decimal Fld_finDiff                        { get { return tbx_finDiff                    .GetDecimalField(); } set { tbx_finDiff                       .PutDecimalField(value); } }
   public decimal Fld_ncPerUlKol                     { get { return tbx_ncPerUlKol                 .GetDecimalField(); } set { tbx_ncPerUlKol                    .PutDecimalField(value); } }
   public decimal Fld_RNM_StupanjDovrsenostiOG       { get { return tbx_RNM_StupanjDovrsenostiOG   .GetDecimalField(); } set { tbx_RNM_StupanjDovrsenostiOG      .PutDecimalField(value); } }
   public decimal Fld_ukKolKgUlazOG                  { get { return tbx_ukKolKgUlazOG                .GetDecimalField(); } set { tbx_ukKolKgUlazOG                   .PutDecimalField(value); } }
   public decimal Fld_ncPerUlKolKg                   { get { return tbx_ncPerUlKolKg               .GetDecimalField(); } set { tbx_ncPerUlKolKg                  .PutDecimalField(value); } }
   public decimal Fld_RNM_StupanjDovrsenosti         { get { return tbx_RNM_StupanjDovrsenosti     .GetDecimalField(); } set { tbx_RNM_StupanjDovrsenosti        .PutDecimalField(value); } }
   public decimal Fld_RNM_StupanjDovrsenostiTwin     { get { return tbx_RNM_StupanjDovrsenostiTwin .GetDecimalField(); } set { tbx_RNM_StupanjDovrsenostiTwin    .PutDecimalField(value); } }
   public decimal Fld_RNM_RealStupanjDovrsenosti     { get { return tbx_RNM_RealStupDovrsenosti    .GetDecimalField(); } set { tbx_RNM_RealStupDovrsenosti       .PutDecimalField(value); } }
   public decimal Fld_RNM_RealStupanjDovrsenostiTwin { get { return tbx_RNM_RealStupDovrsenostiTwin.GetDecimalField(); } set { tbx_RNM_RealStupDovrsenostiTwin   .PutDecimalField(value); } }
   public decimal Fld_ukFinIzlazPG                   { get { return tbx_ukFinIzlazPG               .GetDecimalField(); } set { tbx_ukFinIzlazPG                  .PutDecimalField(value); } }
   public decimal Fld_ukKolKgUlazPG                  { get { return tbx_ukKolOpUlazPG              .GetDecimalField(); } set { tbx_ukKolOpUlazPG                 .PutDecimalField(value); } }
   public decimal Fld_AIndir                         { get { return tbx_finAIndir                  .GetDecimalField(); } set { tbx_finAIndir                     .PutDecimalField(value); } }
   public decimal Fld_ukFinNedovrIzlPG               { get { return tbx_ukFinNedovrIzlPG           .GetDecimalField(); } set { tbx_ukFinNedovrIzlPG              .PutDecimalField(value); } }
   public decimal Fld_ukFinIzlazOG                   { get { return tbx_ukFinIzlazOG               .GetDecimalField(); } set { tbx_ukFinIzlazOG                  .PutDecimalField(value); } }


   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_isProdukt_CreateColumn     (ZXC.Q3un,                   isVisible, "Produkt", "Produkt proizvodnje"                      );
      T_artiklCD_CreateColumn      (ZXC.Q4un,                   isVisible, "Šifra"  , "Šifra artikla"                            );
      T_artiklName_CreateColumnFill(                            isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis"       );
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_kol_CreateColumn           (ZXC.Q5un, 2,                isVisible, "PlanKol", "Planirana količina"                       );
      R_razlKol_CreateColumn       (ZXC.Q5un, 2,                isVisible, "RazlKol", "Razlika planirane i realizirane količine"  );
      T_jedMj_CreateColumn         (ZXC.Q2un,                   isVisible, "JM"     , "Jedinica mjere"                           );

      // 25.01.2017. Metaflex artikl.masaNetto ide u rtrans.t_ppmvOsn
      if(ZXC.IsRNMnotRNP) T_ppmvOsn_CreateColumn(ZXC.Q4un, 2, true, R_ppmvOP_ColName, R_ppmvOP_ColName, false);

      // 24.11.2016. do daljnjega bez kolOP
      // 25.01.2017. ipak op
      R_kolOP_CreateColumn         (ZXC.Q5un, 2,   ZXC.RRD.Dsc_IsOrgPakVisible, "Plan" + R_kolOP_ColName, "Planirana količina u " + R_kolOP_ColName);
  }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturRNmDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }

   #region IVvRealizabeFakturDUC Members

   public List<Rtrans> RealizRtrList_AllYears { get; set; }
   public List<Rtrans> RealizRtrList_ThisYear { get; set; }

   #endregion
}

public class RNZDUC              : FakturExtDUC
{
   #region Constructor

   public RNZDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_RNZ
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);

      hamp_napomena.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_osobaA ,hamp_kupdobNaziv, hamp_posJedCd,
                                    hamp_tt ,  
                                    hamp_dokDate, hamp_dokDate2,
                                    hamp_vezniDok,
                                    hamp_VezniDok2, hamp_Fco,
                                    hamp_napomena2,
                                    hamp_prjIdent, hamp_projekt,
                                    hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT, hamp_v3TT       , /*hamp_v4TT,  hamp_Status    ,*/ hamp_napomena, hamp_opis
                                   };

      hamperMigr = new VvHamper[] { /*hamp_posJedCd,*/ hamp_Mtros, hamp_PrimPlat, /*hamp_napomena2,*/
                                    /*hamp_osobaA ,*/hamp_OsobaB ,
                                    hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
                                    hamp_externLink1, hamp_externLink2
                                    //hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { /*hampCbxM_posJedCd,*/ hampCbxM_Mtros, hampCbxM_PrimPlat, /*hampCbxM_napomena2,*/
                                        /*hampCbxM_OsobaA,*/hampCbxM_osobaB,
                                        hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
                                        hampCbxM_externLink1, hampCbxM_externLink2
                                        //hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      //bool isVisible = true;

      //T_artiklCD_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"      , "Šifra artikla"                     );
      //T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      //T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      //T_jedMj_CreateColumn         (ZXC.Q2un,    isVisible, "JM"         , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturRNzDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }
}

public class PocetnoStanjeDUC    : FakturDUC
{
   #region Constructor

   public PocetnoStanjeDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PST
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
  
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);

      //if(ZXC.IsTEXTHOany)
      if(ZXC.IsTEXTHOshop)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;

      }
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,             isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
    //T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
    //T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible, "Šarža/LOT RGC", "Broj Šarže/Lota, RGC");
      T_kol2_CreateColumn(ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol", "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q4un, 2, isVisible, "Kol", "Količina");
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2, isVisible, "JM", "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4, isVisible, "Cijena", "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un           , 2, isVisible, "Iznos" , "Iznos");

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class InventuraDUC        : FakturDUC
{
   #region Constructor

   public InventuraDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_INV
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);

      if(ZXC.IsTEXTHOshop)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
       //tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;

      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn(ZXC.Q3un,            isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(               isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
      T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");

      T_kol2_CreateColumn (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol", "Ambalažna količina");
      T_kol_CreateColumn  (ZXC.Q4un,         2, isVisible, "Kol", "Količina");
      T_jedMj_CreateColumn(ZXC.Q2un + ZXC.Qun2, isVisible, "JM", "Jedinica mjere");

      T_cij_CreateColumn   (ZXC.Q5un          , 4, isVisible, "Cijena", "Jedinična cijena");
      R_KC_CreateColumn    (ZXC.Q4un          , 2, isVisible, "Iznos" , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;

      // 20.12.2024: metaflex treba, a mozda zatreba i drugima da mogu pisati negativne brojke 
      // buduci sr INV, INM moze kumulirati ... pa za ispravke ...                             
      // ... TH-u i dalje branimo                                                              
      //vvtbT_kol.JAM_DisableNegativeNumberValues = true;
      if(ZXC.IsTEXTHOany)
      {
         vvtbT_kol.JAM_DisableNegativeNumberValues = true;
      }

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class MedjuSkladDUC       : FakturDUC
{

   #region Constructor

   public MedjuSkladDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_MSI,
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation
   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_opis.VvColWdt[1] = 2 * ZXC.Q10un + ZXC.Q5un + ZXC.Qun4;
      hamp_opis.Location = new Point(0, hamp_skladCd.Bottom - ZXC.Qun4);

      nextY = hamp_opis.Bottom;

      if(ZXC.IsTEXTHOshop)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_DokDate2.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokDate2, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT 
                                   };
      

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns
      protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      T_artiklCD_CreateColumn       (ZXC.Q3un           , isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
    //T_doCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "RAM", "RAM", false);
    //T_noCijMal_CreateColumn      (ZXC.Q3un, 0,       ZXC.IsPCTOGO, "HDD", "HDD");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz || ZXC.RRD.Dsc_IsSerlotVisible, "Šarža/LOT/ RGC", "Broj Šarže/Lota, RGC");

      T_kol2_CreateColumn(ZXC.Q3un            , ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible,      "AmbKol" , "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q4un            ,                                    2, isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2 ,                                       isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un            , 4         , isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un            , 2         , isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Thistle);
   }
}

public class MedjuSklad2DUC      : MedjuSkladDUC
{

   #region Constructor

   public MedjuSklad2DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor
   
//   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.LightGreen);
   }
}


public class CjenikDUC           : FakturDUC
{

   #region Constructor

   public CjenikDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor(Faktur.tt_colName, new string[] 
      { 
         Faktur.TT_CJ_VP1, Faktur.TT_CJ_VP2, Faktur.TT_CJ_MP , Faktur.TT_CJ_DE, 
         Faktur.TT_CJ_MK , Faktur.TT_CJ_RB1, Faktur.TT_CJ_RB2, Faktur.TT_CJ_MRZ
      });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_SukKC_K.Visible = false;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_projekt,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      T_artiklCD_CreateColumn(ZXC.Q3un,    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(       isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_cij_CreateColumn     (ZXC.Q5un, 4, isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn  (ZXC.Q5un, 4, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn  (ZXC.Q5un, 4, isVisible, "Rb2"        , "Stopa rabata 2");
   }

   #endregion TheG_Specific_Columns
 
}

public class ProizvodnjaDUC      : FakturExtDUC //FakturDUC
{
   #region Constructor

   public VvTextBox tbx_ncPerUlKol;

   public ProizvodnjaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PIZ
         });
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_isProdukt_CreateColumn(ZXC.Q3un,         isVisible, "Produkt", "Produkt proizvodnje");
      T_artiklCD_CreateColumn(ZXC.Q3un,          isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");

      T_serlot_CreateColumn     (ZXC.Q4un, ZXC.RRD.Dsc_IsVisibleLotOnIzlaz, "Šarža/LOT", "Broj Šarže/Lota");

      T_kol_CreateColumn(ZXC.Q4un, 2,         isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn (ZXC.Q2un + ZXC.Qun2, isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn   (ZXC.Q5un, 4,         isVisible, "Cijena" , "Jedinična cijena");

      // dodano 17.06.2019. za Tembo
      R_kolOP_CreateColumn(ZXC.Q3un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn(ZXC.Q4un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      R_KC_CreateColumn(ZXC.Q4un, 2,         isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_opis.VvColWdt[1] = 2 * ZXC.Q10un + ZXC.Q5un + ZXC.Qun4;
      hamp_opis.Location   = new Point(0, hamp_skladCd.Bottom - ZXC.Qun4);
        
      nextY = hamp_opis.Bottom;

      hamp_S_pix.Visible = true;
      SetSumeHampers(false, false, false, false);

      //04.12.2019.
      if(ZXC.IsSvDUH)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
      }

      //if(ZXC.IsTEXTHOany) //06.02.2015. shop ovo niti nema a dodali smo jos jedno sklad pa treba mogucnost biranja
      //{
      //   tbx_SkladCd .JAM_ReadOnly = true;
      //   tbx_Sklad2Cd.JAM_ReadOnly = true;
      //}
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis, hamp_projekt
                                   };
   }

   public decimal Fld_ncPerUlKol { get { return tbx_ncPerUlKol.GetDecimalField(); } set { tbx_ncPerUlKol.PutDecimalField(value); } }

   #endregion HamperLocation

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.LightSteelBlue);
   }
}

public class SkladOnlyDUC        : FakturDUC
{
   #region Constructor

   public SkladOnlyDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_SKI,
            Faktur.TT_SKU,
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
      hamp_SukKC_K.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      T_artiklCD_CreateColumn(ZXC.Q6un, isVisible, "Šifra", "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(    isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn(ZXC.Q3un, 2,   isVisible, "Kol"  , "Količina"      );
      T_jedMj_CreateColumn(ZXC.Q2un,    isVisible, "JM"   , "Jedinica mjere");
   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class KorTemeljnicaDUC    : FakturDUC
{
   #region Constructor

   public KorTemeljnicaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
          //Faktur.TT_TMI, 10.12.2015. to zapravo na treba
            Faktur.TT_TMU,
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");

      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,    isVisible, "Šifra"   , "Šifra artikla"                     );
    //T_artiklName_CreateColumn    (ZXC.Q7un,    isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "KorKol"  , "Korekturna Količina"               );
      T_jedMj_CreateColumn         (ZXC.Q2un,    isVisible, "JM"      , "Jedinica mjere"                    );
      T_cij_CreateColumn           (ZXC.Q6un, 8, isVisible, "KorIznos", "Korekturni Iznos"                  );
   }

   #endregion TheG_Specific_Columns
}

public class NivelacijaDUC       : FakturExtDUC
{
   #region Constructor

   public NivelacijaDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_ZPC
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {

      CreateArrOfHampers();

      SetParentOfHamperLeftHampers();

      SetLocationToHamperDeda();

      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width, 0);
      hamp_obrMPC.Visible = true;
      nextY = hamp_napomena.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, true);

  }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena, 
                                    hamp_v1TT, hamp_v2TT
                                   };


   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn   (ZXC.Q4un  , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"  , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un,    isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "NBC"        , "Jedinična 'FAKTURNA' cijena");//"Cijena"
      //R_cij_kcr_CreateColumn   (ZXC.Q4un, 2, isVisible, "NBC"        , "Cijena NAB");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "New Marža %", "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "New VPC"    , "Cijena nakon utjecaja marže");
      T_doCijMal_CreateColumn  (ZXC.Q4un, 4, isVisible, "Old MPC"    , "Stara Cijena", true);
    //R_zpc_diff_CreateColumn  (ZXC.Q4un, 2, isVisible, "RazlikaCij" , "");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 4, isVisible, "RazlikaCij" , "");
      T_noCijMal_CreateColumn  (ZXC.Q4un, 2, isVisible, "New MPC"    , "Nova Cijena");

      vvtbR_cij_MSK.JAM_ReadOnly =
      vvtbT_cij.JAM_ReadOnly     = 
      vvtbT_kol.JAM_ReadOnly     = 
      vvtbT_pdvSt.JAM_ReadOnly   = true;

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Empty);
   }
}

public class IzdatnicaNaMjTRrDUC : FakturDUC
{
   #region Constructor

   public IzdatnicaNaMjTRrDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IMT
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;

      T_artiklCD_CreateColumn(ZXC.Q3un,            isVisible, "Šifra", "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(               isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn   (ZXC.Q4un          , 2, isVisible, "Kol"  , "Količina"      );
      T_jedMj_CreateColumn (ZXC.Q2un + ZXC.Qun2  , isVisible, "JM"   , "Jedinica mjere");
      //      T_cij_CreateColumn   (ZXC.Q5un        , 4, isVisible, "Cijena"     , "Jedinična cijena");
      R_KC_CreateColumn    (ZXC.Q4un          , 2, isVisible, "Iznos", "Iznos");

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class PredatUProizDUC     : FakturDUC
{
   #region Constructor

   public PredatUProizDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PPR
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_projekt, hamp_vezniDok, hamp_osobaX,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis, hamp_prjIdent
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,                   isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                            isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_artiklTS_CreateColumn      (ZXC.Q2un           ,        isVisible, "TS"     , "Tip artikla");
      T_kol_CreateColumn           (ZXC.Q4un           , 2,     isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2,        isVisible, "JM"     , "Jedinica mjere");

      // 25.01.2017. ipak idemo na op
      if(ZXC.IsRNMnotRNP) T_ppmvOsn_CreateColumn(ZXC.Q4un, 2, true, R_ppmvOP_ColName, R_ppmvOP_ColName, false);
      R_kolOP_CreateColumn(ZXC.Q3un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      T_cij_CreateColumn           (ZXC.Q5un           , 4,     isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un           , 2,     isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.YellowGreen, clr_Sklad, Color.YellowGreen);
   }
}

public class PovratInterDUC      : FakturDUC
{
   #region Constructor

   public PovratInterDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_POV
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_projekt,hamp_vezniDok, hamp_osobaX,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis, hamp_prjIdent
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,                   isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                            isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_artiklTS_CreateColumn      (ZXC.Q2un,                   isVisible, "TS"     , "Tip artikla");
      T_kol_CreateColumn           (ZXC.Q4un          , 2,      isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2  ,      isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,                isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un, 2,                isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.IndianRed, clr_Sklad, Color.IndianRed);
   }
}

public class PIPDUC     : FakturDUC
{
   #region Constructor

   public PIPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PIP
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
      if(ZXC.projectYearFirstDay.Year == 2017) // WAR_PIP_2017 
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Obavijest.\n\nPIP cijenu program trenutačno ne može pokazati\n\njer je u postupku izmjena algortima obračuna\n\ndotične cijene.");
      }
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width - ZXC.Qun4, 0);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_projekt, hamp_vezniDok, hamp_osobaX,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis, hamp_prjIdent
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,                   isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                            isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_serlot_CreateColumn        (ZXC.Q4un, ZXC.RRD.Dsc_IsSerlotVisible, "RGC/LOT", "Serlot : Broj registra cijevi / Broj Lota");
      T_artiklTS_CreateColumn      (ZXC.Q2un           ,        isVisible, "TS"     , "Tip artikla");
      T_kol_CreateColumn           (ZXC.Q4un           , 2,     isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2,        isVisible, "JM"     , "Jedinica mjere");

      // 24.11.2016. do daljnjega bez kolOP
      // 25.01.2017. ipak idemo na op
      if(ZXC.IsRNMnotRNP) T_ppmvOsn_CreateColumn(ZXC.Q4un, 2, true, R_ppmvOP_ColName, R_ppmvOP_ColName, false);

      R_kolOP_CreateColumn         (ZXC.Q3un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja");
      R_cijOP_CreateColumn         (ZXC.Q4un, 2, ZXC.RRD.Dsc_IsOrgPakVisible, R_cijOP_ColName, "Cijena originalnog pakiranja");

      T_cij_CreateColumn           (ZXC.Q5un           , 4,     isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un           , 2,     isVisible, "Iznos"  , "Iznos");

      //25.11.2016. vraceno da bude bijelo
      vvtbT_cij.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.LightGreen, clr_Sklad, Color.LightGreen);
   }
}

public class PocetnoStanjeMPDUC  : FakturExtDUC
{
   #region Constructor

   public PocetnoStanjeMPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PSM
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
     
      SetParentOfHamperLeftHampers();

      SetLocationToHamperDeda();
  
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width, 0);
      hamp_obrMPC.Visible = true;
      nextY = hamp_napomena.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, true);

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena, 
                                    hamp_v1TT, hamp_v2TT
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      //T_artiklName_CreateColumn(ZXC.Q5un, isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "NBC"        , "Jedinična NABAVNA cijena");
      R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, 2, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      T_pnpSt_CreateColumn     (ZXC.Q2un, 0, isPnpStVisible, "Pnp %", "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn   (ZXC.Q4un, 4, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");
   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Empty);
   }
}

public class InventuraMPDUC      : FakturExtDUC
{
   #region Constructor

   public InventuraMPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_INM
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
     
      SetParentOfHamperLeftHampers();

      SetLocationToHamperDeda();
  
      hamp_dokNum.Location = new Point(hamp_napomena.Right - hamp_dokNum.Width, 0);
     // hamp_obrMPC.Visible = true;
      nextY = hamp_napomena.Bottom;
      panel_MigratorsLeftB.SendToBack();

      hamp_Cjenik.Visible  = true;
      hamp_Cjenik.Location = new Point(hamp_v1TT.Left + ZXC.Qun2, hamp_v1TT.Bottom - ZXC.Qun8);
      hamp_Cjenik.BringToFront();

      SetSumeHampers(false, true, true, true);

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_Cjenik
                                   };
      
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      //T_artiklName_CreateColumn(ZXC.Q5un, isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena", "Jedinična cijena");
      R_KCRP_CreateColumn      (ZXC.Q5un, 2, isVisible, "Iznos", "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;

      // 20.12.2024: metaflex treba, a mozda zatreba i drugima da mogu pisati negativne brojke 
      // buduci sr INV, INM moze kumulirati ... pa za ispravke ...                             
      // ... TH-u i dalje branimo                                                              
      //vvtbT_kol.JAM_DisableNegativeNumberValues = true;
      if(ZXC.IsTEXTHOany)
      {
         vvtbT_kol.JAM_DisableNegativeNumberValues = true;
      }

   }

   #endregion TheG_Specific_Columns

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Empty);
   }
}

public class MedjuSkladVMIuDUC   : FakturExtDUC
{
   #region Constructor

   public MedjuSkladVMIuDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_VMI,
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_obrMPC.Visible = true;
      nextY = hamp_sklad2Cd.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, true);

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_DokDate2.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
         hamp_obrMPC .Visible = false;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokDate2, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT 
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      
      T_artiklCD_CreateColumn  (ZXC.Q4un                            ,                              isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                                               isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un                            , 2,                           isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un                            ,                              isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un                            , 4,                           isVisible, "NBC"        , "Jedinična NABAVNA cijena");
      R_KCR_CreateColumn       (ZXC.Q4un                            , 2,                           isVisible, "NAB Vrij"   , "Iznos NAB");
      T_mrzSt_CreateColumn     (ZXC.Q4un                            , 2, ZXC.IsTEXTHOany ? false : isVisible, "Marža %"    , "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un                            , 4, ZXC.IsTEXTHOany ? false : isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      R_KCRM_CreateColumn      (ZXC.Q4un                            , 2, ZXC.IsTEXTHOany ? false : isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
      T_pdvSt_CreateColumn     (ZXC.IsTEXTHOany ? ZXC.Qun8: ZXC.Q3un, 0,                           isVisible, "Pdv %"      , "Stopa PDV-a"           );
      R_cij_MSK_CreateColumn   (ZXC.Q4un                            , 4,                           isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un                            , 2,                           isVisible, "MP Vrij"    , "");

      if(ZXC.IsTEXTHOany)
      {
         vvtbT_cij    .JAM_ReadOnly =
         vvtbT_pdvSt  .JAM_ReadOnly =
         vvtbR_cij_MSK.JAM_ReadOnly = true;
      }
   }

   #endregion TheG_Specific_Columns

   public override bool IsSkl_2_malop { get { return true; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Thistle);
   }
}

public class MedjuSkladVMI2DUC   : MedjuSkladVMIuDUC
{
   #region Constructor

   public MedjuSkladVMI2DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.LightGreen);
   }
}


public class KIZDUC              : FakturExtDUC
{
   #region Constructor

   public KIZDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_KIZ
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

      hamp_S_KIZ_cij.Visible = true;
      hamp_S_KIZ_cij.Location = new Point(hamp_S_ukPdv.Left, 0);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_posJedCd, hamp_tt, hamp_skladCd,
                                    hamp_dokDate, hamp_dokNum, hamp_SkladDate,  hamp_napomena, 
                                     hamp_Cjenik, /*hamp_Status  ,*/ hamp_sklad2Cd, hamp_v1TT  , hamp_v2TT , hamp_v3TT     
                                  };

      hamperMigr = new VvHamper[] { /*hamp_posJedCd,*/ hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { /*hampCbxM_posJedCd,*/ hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };

      hamperNoMigLeftRight = new VvHamper[] { hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb,  hamp_vezniDok, hamp_projekt, 
                                              hamp_RokPlac,hamp_DospDate, hamp_PDV, hamp_PonudDate, hamp_kupdobOther, 
                                              hamp_v4TT
                                            };

   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,             isVisible, "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"   , "Naziv artikla");
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"     , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"      , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,          isVisible, "NabCij", "Nabavna cijena");

      T_noCijMal_CreateColumn  (ZXC.Q4un, 2, isVisible, "ProdCij"   , "Prodajna Cijena");
    //R_kiz_KC_CreateColumn    (ZXC.Q4un, 2, true     , "Iznos"    , "Veleprodajna cijena");
      T_pnpSt_CreateColumn     (ZXC.Q3un, 2, true     , "Rb1"       , "Stopa rabata");
      R_kiz_KCR_CreateColumn   (ZXC.Q4un, 2, true     , "Uk bez Pdv", "Ukupan iznos bez PDV-a");

//    R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
//    R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
//    R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
//    R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

    //T_pdvSt_CreateColumn     (ZXC.Q2un, 0, isVisible, "PdvSt"      , "Stopa PDV-a");
    //R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturKIZDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_komis, clr_Izlaz);
   }

}

public class PIKDUC              : FakturExtDUC
{
   #region Constructor

   public PIKDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PIK
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_posJedCd, hamp_tt, hamp_skladCd,
                                    hamp_dokDate, hamp_dokNum, hamp_SkladDate,  hamp_napomena, 
                                    hamp_Status  , hamp_sklad2Cd, hamp_v1TT  , hamp_v2TT , hamp_v3TT     
                                  };

      hamperMigr = new VvHamper[] { /*hamp_posJedCd,*/ hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { /*hampCbxM_posJedCd,*/ hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };

      hamperNoMigLeftRight = new VvHamper[] { hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb,  hamp_vezniDok, hamp_projekt, 
                                              hamp_RokPlac,hamp_DospDate, hamp_PDV, hamp_PonudDate, hamp_kupdobOther, 
                                              hamp_v4TT
                                            };

   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      T_artiklCD_CreateColumn(ZXC.Q3un,           isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(              isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn   (ZXC.Q4un, 2         , isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn (ZXC.Q2un + ZXC.Qun2 , isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn   (ZXC.Q5un, 4         , isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn    (ZXC.Q4un, 2         , isVisible, "Iznos"  , "Iznos");

      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturKIZDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_komis, clr_Ulaz);
   }

}

#region PPUK

public class URPDUC              : FakturPDUC
{
   #region Constructor

   public URPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_URA
         });
   }

   #endregion Constructor
  
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvGeokind, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   ,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn     (ZXC.Q2un ,               isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn       (ZXC.Q3un, 2,             isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,             isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,             isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un - ZXC.Qun4, 4,  isVisible, "Rb1"        , "Stopa rabata 1");
      //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,             isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,             isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
      T_pdvSt_CreateColumn     (ZXC.Q2un, 0,             isVisible, "PdvSt"      , "Stopa PDV-a"           );
      T_pdvKolTip_CreateColumn (ZXC.QUN    ,             isVisible);
      R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2,             isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"   , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_serno_CreateColumn          (ZXC.Q6un,    isVisible, "SerBroj" , "Serijski broj artikla"             );
      D_JM_CreateColumn             (ZXC.Q3un,    isVisible, "JM"      , "Jedinica mjere"                    );
      T_dimX_CreateColumn           (ZXC.Q3un, 2, isVisible, "Duljina" , "Duljina"                           );
      T_dimZ_CreateColumn           (ZXC.Q3un, 2, isVisible, "Promjer" , "Promjer"                           );
      T_kolG2_CreateColumn          (ZXC.Q3un, /* 3 ??? */ 2, isVisible, "Količina", "Količina"                          );
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURPDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }

}

public class PRIpDUC             : FakturPDUC
{
   #region Constructor

   public PRIpDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PRI
         });
   }

   #endregion Constructor
  
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra" , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv" , "Naziv artikla ili proizvoljan opis");
      if(ZXC.IsPCTOGO == false) T_konto_CreateColumn     (ZXC.Q2un   , ZXC.IsPCTOGO ? false : true, "Konto" , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"   , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"    , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "Cijena", "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un, 4, isVisible, "Rb1"   , "Stopa rabata 1");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "Iznos" , "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"   , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_serno_CreateColumn          (ZXC.Q6un,    isVisible, "SerBroj" , "Serijski broj artikla"             );
      D_JM_CreateColumn             (ZXC.Q3un,    isVisible, "JM"      , "Jedinica mjere"                    );
      T_dimX_CreateColumn           (ZXC.Q3un, 2, isVisible, "Duljina" , "Duljina"                           );
      T_dimZ_CreateColumn           (ZXC.Q3un, 2, isVisible, "Promjer" , "Promjer"                           );
      T_kolG2_CreateColumn          (ZXC.Q3un, /* 3 ??? */ 2, isVisible, "Količina", "Količina"              );
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturURPDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }

}

public class IRPDUC              : FakturPDUC
{
   #region Constructor

   public IRPDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRA
         });

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR, hamp_eRproc, 
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/hamp_DatumX,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/hampCbxM_DatumX, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un,                  isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                           isVisible, "Naziv"      , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un   ,              ZXC.IsF2_2026_rules, "KPD"           , "KPD sifra");
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,        isVisible, "Usl"         , "Usluga");
      T_konto_CreateColumn         (ZXC.Q3un   ,               isVisible, "Konto"      , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
     
    //if(ZXC.IsPCTOGO)
    //{
    //   T_artiklTS_CreateColumn(ZXC.Q2un,    true, "Tip", "Tip artikla");
    //   T_doCijMal_CreateColumn(ZXC.Q3un, 0, true, "RAM", "RAM", false);
    //   T_noCijMal_CreateColumn(ZXC.Q3un, 0, true, "HDD", "HDD");
    //}
      
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
    //T_rbt2St_CreateColumn    (ZXC.Q2un, 0,          isVisible, "Rb2"        , "Stopa rabata 2");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");

      R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
      R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn     (ZXC.Q2un, 0, isVisible, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn (ZXC.QUN    , isVisible);
      R_KCRP_CreateColumn      (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

      R_grNameG1_CreateColumn(ZXC.Q4un, false, "GrName", "GrName");

   }

   #endregion TheG_Specific_Columns
 
   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

     //if(ZXC.IsPCTOGO)
     //{ 
     //   T_serno_CreateColumn          (ZXC.Q8un,    isVisible, "Serijski broj", "Serijski broj artikla"             );
     //   T_artiklCD2_CreateColumn      (ZXC.Q5un,    isVisible, "Šifra"        , "Šifra artikla"                     );
     //   T_artiklName2_CreateColumnFill(             isVisible, "Naziv"        , "Naziv artikla ili proizvoljan opis");
     //   R_artiklTS_CreateColumn       (ZXC.Q3un - ZXC.Qun2,    isVisible, "Tip"          , "Tip artikla");
     //   R_ramKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "RAM klasa"    , "RAM klasa");
     //   R_hddKlasa2_CreateColumn      (ZXC.Q3un,    isVisible, "HDD klasa"    , "RAM klasa");
     //   T_skladCD2_CreateColumn       (ZXC.Q3un,    isVisible, "Sklad"        , "Izlazno skladište"                 );
     //   T_dimZ_CreateColumn           (ZXC.Q3un, 0, isVisible, "RAM"         , "RAM"                           );
     //   T_decC_CreateColumn           (ZXC.Q3un, 0, isVisible, "HDD"         , "HDD old"                           );
     //   T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "Stavka"       , "UGANDO stavka"                     );
     //}
     //else //PPUK
     //{
         T_artiklCD2_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"   , "Šifra artikla"                     );
         T_artiklName2_CreateColumnFill(             isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
         T_grCD_CreateColumn           (ZXC.Q4un,    isVisible, "Grupa"   , "Oznaka za grupiranje"              , true);
         R_grNameG2_CreateColumn       (ZXC.Q8un,    isVisible, "Opis"    , "Opis grupe"                        );
         T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "Paleta"  , "Broj palete"                       );
         D_JM_CreateColumn             (ZXC.Q3un,    isVisible, "JM"      , "Jedinica mjere"                    );
         T_dimX_CreateColumn           (ZXC.Q3un, 2, isVisible, "Duljina" , "Duljina"                           );
         T_dimY_CreateColumn           (ZXC.Q3un, 2, isVisible, "Širina"  , "Širina"                            );
         T_dimZ_CreateColumn           (ZXC.Q3un, 2, isVisible, "Debljina", "Debljina"                          );
         T_komada_CreateColumn         (ZXC.Q3un, 2, isVisible, "Komada"  , "Komada"                            );
         T_kolG2_CreateColumn          (ZXC.Q3un, 3, isVisible, "Količina", "Količina"                          );
         T_isKomDummy_CreateColumn     (ZXC.Q3un   , isVisible, "Kom=1"   , "Komada=1"                          );
      //}
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRPDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

   //public override bool HasDscSubVariants nemam pojma za kaj to sluzi
   //{
   //   get
   //   {
   //      return true;
   //   }
   //}

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB  ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class PIZpDUC             : FakturPDUC
{
   #region Constructor

   public PIZpDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PIX
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_opis.VvColWdt[1] = 2 * ZXC.Q10un + ZXC.Q5un + ZXC.Qun4;
      hamp_opis.Location = new Point(0, hamp_skladCd.Bottom - ZXC.Qun4);

      nextY = hamp_opis.Bottom;
      panel_MigratorsLeftB.SendToBack();

      hamp_S_pix.Visible = true;
      SetSumeHampers(false, false, false, false);

   }
  
   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, hamp_opis, hamp_projekt
                                   };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_isProdukt_CreateColumn     (ZXC.Q3un,            isVisible, "Produkt"  , "Produkt proizvodnje");
      T_artiklCD_CreateColumn      (ZXC.Q3un,            isVisible, "Šifra"    , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible, "Naziv"    , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q4un, 3,         isVisible, "Kol"      , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2, isVisible, "JM"       , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q5un, 4,         isVisible, "Cijena"   , "Jedinična cijena");
      R_KC_CreateColumn            (ZXC.Q4un, 2,         isVisible, "Iznos"    , "Iznos");
      T_noCijMal_CreateColumn      (ZXC.Q4un, 2,         isVisible, "KorCijena", "Korekciaj cijene");

      vvtbT_cij     .JAM_ReadOnly = true;
      vvtbT_noCijMal.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns
 
   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q4un,    isVisible, "Šifra"   , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(             isVisible, "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_serno_CreateColumn          (ZXC.Q6un,    isVisible, "SerBroj" , "Serijski broj artikla"             );
      D_JM_CreateColumn             (ZXC.Q3un,    isVisible, "JM"      , "Jedinica mjere"                    );
      //T_paletaNo_CreateColumn       (ZXC.Q3un,    isVisible, "Paleta"  , "Broj palete"                       );
      T_dimX_CreateColumn           (ZXC.Q3un, 2, isVisible, "Duljina" , "Duljina"                           );
      T_dimY_CreateColumn           (ZXC.Q3un, 2, isVisible, "Širina"  , "Širina"                            );
      T_dimZ_CreateColumn           (ZXC.Q3un, 2, isVisible, "Deblj/Prom", "Debljina ili Promjer"                          );
      T_komada_CreateColumn         (ZXC.Q3un, 2, isVisible, "Komada"  , "Komada"                            );
      T_kolG2_CreateColumn          (ZXC.Q3un, 3, isVisible, "Količina", "Količina"                          );
      T_isKomDummy_CreateColumn     (ZXC.Q3un   , isVisible, "Kom=1"   , "Komada=1"                          ); //07.12.2015
    }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturIRPDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

}

#endregion PPUK

public class BORDUC              : FakturPDUC
{
   #region Constructor

   public BORDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_BOR
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

      //ThePolyGridTabControl.TabPages[TabPageTitle1].Visible = false;
      //ThePolyGridTabControl.TabPages[TabPageTitle2].Visible = true;
      ThePolyGridTabControl.SelectedTab = ThePolyGridTabControl.TabPages[TabPageTitle2];


   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();


      hamp_DatumX .Location = new Point(hamp_kupdobOther.Right - ZXC.Qun2          , hamp_tt    .Bottom - ZXC.Qun4);
      hamp_dokDate.Location = new Point(hamp_kupdobNaziv.Right - hamp_dokDate.Width, hamp_tt    .Bottom - ZXC.Qun4);
     
      hamp_dokNum.Location  = new Point(hamp_kupdobOther.Right - hamp_dokNum .Width, hamp_DatumX.Bottom - ZXC.Qun4);
      
      hamp_v1TT  .Location = new Point(hamp_tt.Left                          , hamp_tt     .Bottom - ZXC.Qun4);
      hamp_v2TT  .Location = new Point(hamp_tt.Left                          , hamp_v1TT   .Bottom - ZXC.Qun4);
      hamp_v3TT  .Location = new Point(hamp_tt.Left                          , hamp_v2TT   .Bottom - ZXC.Qun4);
      hamp_Status.Location = new Point(hamp_dokDate.Right - hamp_Status.Width, hamp_dokDate.Bottom - ZXC.Qun4);

      hamp_externLink1.Location = new Point(hamp_kupdobNaziv.Left            , hamp_v2TT.Bottom );
      hamp_externLink2.Location = new Point(hamp_externLink1.Right - ZXC.Qun4, hamp_v2TT.Bottom );
      hamp_napomena   .Location = new Point(hamp_kupdobNaziv.Left            , hamp_v3TT.Bottom -ZXC.Qun4);
      
      hamp_DatumX.Parent = TheTabControl.TabPages[0];
      hamp_dokNum.BringToFront();

      nextY = hamp_napomena.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, false, false, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_kupdobOther, hamp_tt,
                                    hamp_dokDate/*date do*/  , hamp_DatumX/*date od*/, hamp_dokNum,   
                                    hamp_v1TT       , hamp_v2TT, hamp_v3TT, hamp_Status  , hamp_napomena,
                                    hamp_externLink1, hamp_externLink2
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn(ZXC.Q3un,          isVisible, "Šifra"  , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"  , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn   (ZXC.Q4un, 3,         isVisible, "Kol"    , "Količina"      );
      T_jedMj_CreateColumn (ZXC.Q2un + ZXC.Qun2, isVisible, "JM"     , "Jedinica mjere");
      T_cij_CreateColumn   (ZXC.Q5un, 4,         isVisible, "Cijena" , "Jedinična cijena");
      R_KC_CreateColumn    (ZXC.Q4un, 2,         isVisible, "Iznos"  , "Iznos");
     }

   #endregion TheG_Specific_Columns

   #region TheG_Specific_Columns2

   protected override void InitializeDUC_Specific_Columns2()
   {
      bool isVisible = true;

      T_artiklCD2_CreateColumn      (ZXC.Q3un,             isVisible, "Šifra"              , "Šifra artikla"                     );
      T_artiklName2_CreateColumnFill(                      isVisible, "Naziv"              , "Naziv artikla ili proizvoljan opis");
      T_serno_CreateColumn          (ZXC.Q4un,             isVisible, "Soba"               , "Broj sobe"                         );
      T_paletaNo_CreateColumn       (ZXC.Q3un,             isVisible, "Gost"               , "Ime gosta"                         );
      R_grNameG2_CreateColumn       (ZXC.Q10un + ZXC.Q3un, isVisible, "Ime i prezime gosta", "Ime i prezime gosta"               );
      T_kolG2_CreateColumn          (ZXC.Q3un, 2,          isVisible, "Količina"           , "Količina"                          );
      //T_dimX_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Cijena"             , "Cijena"                            );
      //T_dimZ_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Ukupno"             , "Ukupno"                            );

      vvtbT_paletaNo.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave_G2));

   }
   
   private void AnyKupdobTextBoxLeave_G2(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      Kupdob kupdob_rec;

      VvTextBoxEditingControl vvtb_editingControl = sender as VvTextBoxEditingControl;

      if(vvtb_editingControl == null) return;

      if(vvtb_editingControl.Text != this.originalText)
      {
         this.originalText = vvtb_editingControl.Text;
         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         int currRow = vvtb_editingControl.EditingControlRowIndex;

         if(kupdob_rec != null && vvtb_editingControl.Text != "")
         {
            TheG2.PutCell(ci2.iR_grName, currRow, kupdob_rec.Naziv);
         }
         else
         {
            TheG2.PutCell(ci2.iR_grName, currRow, "");
         }

         // samo za DUC-eve
         ZXC.TheVvForm.SetDirtyFlag(sender);
      }
   }

   #endregion TheG_Specific_Columns2

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturBORDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }

   public override string TabPageTitle2
   {
      get { return "Detalji"; }
   }
   public override string TabPageTitle1
   {
      get { return "Zbirno"; }
   }

}


public class PIMDUC              : FakturExtDUC
{
   #region Constructor

   public PIMDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PIM
         });
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      bool isPnpStVisible = ZXC.RRD.Dsc_IsPnpStVisible;

      T_isProdukt_CreateColumn     (ZXC.Q3un,            isVisible,      "Produkt" , "Produkt proizvodnje");
      T_artiklCD_CreateColumn      (ZXC.Q3un,            isVisible,      "Šifra"   , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible,      "Naziv"   , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q4un, 4,         isVisible,      "Kol"     , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2, isVisible,      "JM"      , "Jedinica mjere");
      R_kolOP_CreateColumn         (ZXC.Q3un, 4,         isVisible,      "KolOP"   , "Količina originalnog pakiranja"    );
      T_cij_CreateColumn           (ZXC.Q5un, 4,         isVisible,      "Cijena"  , "Jedinična cijena");
      T_pdvSt_CreateColumn         (ZXC.Q3un, 0,         isVisible,      "Pdv %"   , "Stopa PDV-a"           );
      T_pnpSt_CreateColumn         (ZXC.Q2un, 0,         isPnpStVisible, "Pnp %"   , "Stopa posebnog poreza na potrošnju");
      R_cij_MSK_CreateColumn       (ZXC.Q4un, 2,         isVisible,       "MPC"    , "");
      R_MSK_CreateColumn           (ZXC.Q5un, 2,         isVisible,       "MP Vrij", "");

   }

   #endregion TheG_Specific_Columns

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      //hamp_opis.VvColWdt[1] = 2 * ZXC.Q10un + ZXC.Q5un + ZXC.Qun4;
      //hamp_opis.Location   = new Point(0, hamp_skladCd.Bottom - ZXC.Qun4);

      nextY = hamp_projekt.Bottom;
      panel_MigratorsLeftB.SendToBack();
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT, /*hamp_opis,*/ hamp_projekt, hamp_Cjenik
                                   };
   }

   #endregion HamperLocation

   public override bool IsSkl_2_malop { get { return true; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.LightSteelBlue);
   }
}

public class NORDUC              : FakturExtDUC
{
   #region Constructor

   public NORDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NOR
         });
   }

   #endregion Constructor

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q3un,            isVisible                  , "Šifra"        , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                     isVisible                  , "Naziv"        , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q4un, 4,         isVisible                  , "Kol"          , "Količina"                          );
      T_jedMj_CreateColumn         (ZXC.Q2un + ZXC.Qun2, isVisible                  , "JM"           , "Jedinica mjere"                    );
      R_kolOP_CreateColumn         (ZXC.Q3un, 4,         ZXC.RRD.Dsc_IsOrgPakVisible, R_kolOP_ColName, "Količina originalnog pakiranja"    );
   }

   #endregion TheG_Specific_Columns

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();

      hamp_dokDate .Location = new Point(hamp_prjArtName.Left        , hamp_tt.Bottom - ZXC.Qun4);
      hamp_dokNum  .Location = new Point(hamp_dokDate.Right          , hamp_tt.Bottom - ZXC.Qun4);
      hamp_v1TT    .Location = new Point(hamp_dokNum.Right + ZXC.Q2un, hamp_tt.Bottom - ZXC.Qun4);
      hamp_v2TT    .Location = new Point(hamp_tt.Left                , hamp_tt.Bottom - ZXC.Qun4);

      hamp_napomena.Location = new Point(hamp_prjArtName.Left, hamp_dokDate.Bottom - ZXC.Qun4);
      nextY = hamp_napomena.Bottom;
      panel_MigratorsLeftB.SendToBack();
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_tt , hamp_prjArtName, 
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT 
                                   };
   }

   #endregion HamperLocation

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.LightSteelBlue);
   }
}

public class TransformDUC        : FakturExtDUC
{
   #region Constructor

   public TransformDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_TRI,
         });

      SuspendLayout();

    //  ThePolyGridTabControl.TabPages.RemoveAt(1); ???

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      //hamp_obrMPC.Visible = true;
      nextY = hamp_sklad2Cd.Bottom;
      panel_MigratorsLeftB.SendToBack();

      hamp_S_pix.Visible = true;
      SetSumeHampers(false, false, false, true);

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT//,  hamp_projekt
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;
      
      T_isProdukt_CreateColumn     (ZXC.Q3un,    isVisible, "Produkt"    , "Produkt proizvodnje");
      T_artiklCD_CreateColumn      (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol2_CreateColumn          (ZXC.Q3un, ZXC.RRD.Dsc_AmbKolNumOfDecimalPlaces, ZXC.RRD.Dsc_IsKol2Visible, "AmbKol"     , "Ambalažna količina");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4, isVisible, "NBC"        , "Jedinična NABAVNA cijena");
      R_KCR_CreateColumn           (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_pdvSt_CreateColumn         (ZXC.Qun8, 0, isVisible, ""           , ""         );
      R_cij_MSK_CreateColumn       (ZXC.Q4un, 2, isVisible, "MPC"        , "");
      R_MSK_CreateColumn           (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");

      vvtbT_cij    .JAM_ReadOnly     = 
      vvtbT_pdvSt  .JAM_ReadOnly     = 
      vvtbR_cij_MSK.JAM_ReadOnly     = true;


   }

   #endregion TheG_Specific_Columns

   public override bool IsSkl_2_malop { get { return true; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.LightSteelBlue);
   }
}

public class MedjuSkladMVIDUC    : FakturExtDUC
{
   #region Constructor

   public MedjuSkladMVIDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_MVI,
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_obrMPC.Visible = false;
      nextY = hamp_sklad2Cd.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, true);

      if(ZXC.IsTEXTHOany)
      {
         tbx_SkladCd .JAM_ReadOnly = true;
         tbx_Sklad2Cd.JAM_ReadOnly = true;
         tbx_DokDate2.JAM_ReadOnly = true;
         tbx_TtNum   .JAM_ReadOnly = true;
      }

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokDate2, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT
                                   };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      
      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      //T_artiklName_CreateColumn(ZXC.Q5un, isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "NBC"        , "Jedinična NABAVNA cijena");
      R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_pdvSt_CreateColumn     (ZXC.Qun8, 0, isVisible, ""      , "Stopa PDV-a"           );
      R_cij_MSK_CreateColumn   (ZXC.Q4un, /*2*/4, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");

      vvtbT_cij    .JAM_ReadOnly     = 
      vvtbT_pdvSt  .JAM_ReadOnly     = 
      vvtbR_cij_MSK.JAM_ReadOnly     = true;
   }

   #endregion TheG_Specific_Columns

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Thistle);
   }
}

public class MedjuSkladMVI2DUC   : MedjuSkladMVIDUC
{

   #region Constructor

   public MedjuSkladMVI2DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
   }

   #endregion Constructor

   //   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.LightGreen);
   }
}


public class MedjuSkladMMIDUC    : FakturExtDUC
{
   #region Constructor

   public MedjuSkladMMIDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul): base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_MMI,
         });

      SuspendLayout();

      ThePolyGridTabControl.TabPages.RemoveAt(1);

      ResumeLayout();
   }

   #endregion Constructor
   
   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
      SetParentOfHamperLeftHampers();
      SetLocationToHamperDeda();

      hamp_obrMPC.Visible = true;
      nextY = hamp_sklad2Cd.Bottom;
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, true);
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_skladCd, hamp_tt , hamp_sklad2Cd,
                                    hamp_dokDate, hamp_dokDate2, hamp_dokNum, hamp_napomena,
                                    hamp_v1TT, hamp_v2TT
                                   };

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      CreateAllwaysInvisibleDataGridViewColumn(TheG, "t_twinID");
      bool isVisible = true;
      
      T_artiklCD_CreateColumn  (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(         isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      //T_artiklName_CreateColumn(ZXC.Q5un, isVisible, "Naziv", "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un, 4, isVisible, "NBC"        , "Jedinična NABAVNA cijena");
      R_KCR_CreateColumn       (ZXC.Q4un, 2, isVisible, "NAB Vrij"   , "Iznos NAB");
      T_mrzSt_CreateColumn     (ZXC.Q4un, 2, isVisible, "Marža %"    , "Stopa marže");
      R_cij_kcrm_CreateColumn  (ZXC.Q4un, /*2*/4, isVisible, "VPC"        , "Cijena nakon utjecaja marže");
      R_KCRM_CreateColumn      (ZXC.Q4un, 2, isVisible, "VP Vrij"    , "Iznos nakon utjecaja marže");
      T_pdvSt_CreateColumn     (ZXC.Q3un, 0, isVisible, "Pdv %"      , "Stopa PDV-a"           );
      R_cij_MSK_CreateColumn   (ZXC.Q4un, /*2*/4, isVisible, "MPC"        , "");
      R_MSK_CreateColumn       (ZXC.Q5un, 2, isVisible, "MP Vrij"    , "");

    //  vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   public override bool IsSkl_2_malop { get { return false; } }

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_malop, Color.Thistle);
   }
}

public class WYRNDUC             : FakturExtDUC
{
   #region Constructor

   public WYRNDUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_WRN,
            Faktur.TT_YRN
         });

      // 31.10.2013: 
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);
      
      hamp_VezniDok2.Location = new Point(hamp_vezniDok.Left, hamp_vezniDok.Bottom);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_PDV, hamp_pdvZPkind,hamp_pdvGeokind, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_VezniDok2
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    /*hamp_VezniDok2,*/ hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB, hamp_osobaX, hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
                                    hamp_externLink1, hamp_externLink2, hamp_prjIdent, 
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        /*hampCbxM_VezniDok2,*/ hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
                                        hampCbxM_externLink1, hampCbxM_externLink2, hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un            , isVisible, "Šifra"           , "Šifra artikla"                                    );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"           , "Naziv artikla ili proizvoljan opis"               );
      T_artiklTS_CreateColumn      (ZXC.Q2un            , isVisible, "TS"              , "Tip artikla"                                      );
      T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"             , "Usluga"                                           );
    //T_konto_CreateColumn         (ZXC.Q3un            , isVisible, "Konto"           , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_mtros_cd_CreateColumn      (ZXC.Q3un            , ZXC.RRD.Dsc_IsMtrosColVisible, "MtrosCD"    , "Šifra Mjesta troška"               );
      R_mtros_tk_CreateColumn      (ZXC.Q3un            , ZXC.RRD.Dsc_IsMtrosColVisible, "MjTroška"   , "Tiker Mjesta troška"               );
      T_kol_CreateColumn           (ZXC.Q3un,          2, isVisible, "Kol"             , "Količina"                                         );
      T_jedMj_CreateColumn         (ZXC.Q2un            , isVisible, "JM"              , "Jedinica mjere"                                   );
      T_cij_CreateColumn           (ZXC.Q4un,          4, isVisible, "Cijena"          , "Jedinična cijena"                                 );
      T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"             , "Stopa rabata 1"                                   );
      R_KCR_CreateColumn           (ZXC.Q4un,          2, isVisible, "Uk bez Pdv"      , "Ukupan iznos bez PDV-a"                           );
      T_pdvSt_CreateColumn         (ZXC.Q2un,          0, isVisible, "PdvSt"           , "Stopa PDV-a"                                      );
      T_pdvKolTip_CreateColumn     (ZXC.QUN             , isVisible                                                                         );
      R_KCRP_CreateColumn          (ZXC.Q4un + ZXC.Qun2 ,          2, isVisible        , "Uk s PDV-om", "Ukupno s PDV-om"                   );
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturWYRDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, Color.Empty, clr_Ulaz);
   }
}


public class CjenikKupca_DUC         : FakturExtDUC
{
   #region Constructor

   public CjenikKupca_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_CKP
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, false, false, false);

      hamp_dokNum  .Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_ValName .Location = new Point(hamp_kupdobNaziv.Left                      , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_napomena.Location = new Point(hamp_kupdobNaziv.Left                      , hamp_dokDate    .Bottom - ZXC.Qun4);

      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - ZXC.Q4un + ZXC.Qun4 + ZXC.Qun8;
      hamp_napomena.BringToFront();

      nextY = hamp_napomena.Bottom;

      hamp_IznosUvaluti.Visible = false;
      hamp_SukKC_K     .Visible = false;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_dokDate    , hamp_dokNum,  hamp_ValName , hamp_napomena, 
                                    /*hamp_kupdobOther, hamp_skladCd, hamp_v1TT, hamp_v2TT, hamp_v3TT  , hamp_v4TT*/
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent//, hamp_opis
                                  };
      
      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent//, hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un,    true, "Šifra"   , "Šifra artikla"         );
      T_artiklName_CreateColumnFill(             true, "Naziv"   , "Naziv artikla"         );
      T_jedMj_CreateColumn         (ZXC.Q2un,    true, "JM"      , "Jedinica mjere"        );
      T_cij_CreateColumn           (ZXC.Q4un, 4, true, "Cijena"  , "Jedinična cijena"      );
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturCjKupcaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_None, clr_Sklad, clr_None);
   }
}


#region SVD
public class URA_SVD_DUC         : FakturExtDUC
{
 //public override bool HasOrgBopCop => true;
   public override bool HasOrgBopCop { get { return true; } }

   #region Constructor

   public URA_SVD_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_URA
         });
      // 31.10.2013: nakon Tembo problem (ubuduce bi trebalo nekako preko rulsa raci koji DUC-evi mogu a koji ne kroz vise skladista)
      //dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;
   }

   #endregion Constructor
  
   #region HamperLocation
   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

    // 08.10.2021. mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
    //SetParentOfhampers();
    //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();
           
      SetSumeHampers(true, true, true, false);

      hamp_dokNum   .Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width    , hamp_dokDate    .Bottom - ZXC.Qun4);
      hamp_SkladDate.Location = new Point(hamp_kupdobNaziv.Right - hamp_SkladDate.Width , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
    //hamp_pdvZPkind.Location = new Point(hamp_kupdobNaziv.Right - hamp_pdvZPkind.Width , hamp_dokNum     .Bottom - ZXC.Qun4);
      hamp_vezniDok .Location = new Point(hamp_kupdobNaziv.Left                         , hamp_dokDate    .Bottom - ZXC.Qun4);
      hamp_PDV      .Location = new Point(hamp_kupdobNaziv.Left                         , hamp_vezniDok   .Bottom - ZXC.Qun4);

      hamp_VezniDok2.Location = new Point(hamp_dokDate.Left                          , hamp_dokDate    .Bottom - ZXC.Qun8);
      hamp_pdvZPkind.Location = new Point(hamp_VezniDok2.Right + ZXC.QUN - ZXC.Qun4  , hamp_dokDate    .Bottom - ZXC.Qun4);
      hamp_napomena .Location = new Point(hamp_PDV     .Right                        , hamp_vezniDok   .Bottom - ZXC.Qun4);

      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - ZXC.Q4un + ZXC.Qun4 + ZXC.Qun8 - hamp_PDV.Width;
      hamp_vezniDok.BringToFront();
      
      nextY = hamp_napomena.Bottom;

      tbx_SkladDate.JAM_ReadOnly = !ZXC.CurrUserHasSuperPrivileges;

      hamp_IznosUvaluti.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_kupdobOther, 
                                    hamp_dokDate, hamp_SkladDate, hamp_vezniDok, hamp_VezniDok2, /*hamp_projekt,*/hamp_pdvZPkind, hamp_PDV,
                                    hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd, hamp_v1TT, hamp_v2TT /*, hamp_v3TT  , hamp_v4TT*/
                                  };

      //hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
      //                              /*hamp_VezniDok2,*/ hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
      //                              hamp_OpciA, hamp_OpciB,  hamp_osobaX,hamp_carinaKind,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
      //                              hamp_externLink1, hamp_externLink2, hamp_prjIdent
      //                              //, hamp_opis
      //                            };

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
      //                                  /*hampCbxM_VezniDok2,*/ hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
      //                                  hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX, hampCbxM_carinaKind,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
      //                                  hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent
      //                                  //, hampCbxM_opis                                   
      //                                };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn       (ZXC.Q3un              , true, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill (                        true, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      R_ORG_T_doCijMal_CreateColumn (ZXC.Q3un           , 0, true, "ORG"        , "Originalno pakiranje");
      R_BOP_CreateColumn            (ZXC.Q4un           , 2, true, "BOP"        , "Broj Originalnog pakiranja");
      R_COP_CreateColumn            (ZXC.Q4un           , 4, true, "COP"        , "Cijena Originalnog pakiranja");
      T_kol_CreateColumn            (ZXC.Q4un           , 2, true, "Količina"   , "Količina"      );
      T_jedMj_CreateColumn          (ZXC.Q2un           ,    true, "JM"         , "Jedinica mjere"        );
      T_cij_CreateColumn            (ZXC.Q4un           , 4, true, "Cijena"     , "Jedinična cijena");
      T_rbt1St_CreateColumn         (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"        , "Stopa rabata 1");
      T_pdvSt_CreateColumn          (ZXC.Q2un           , 0, true, "PdvSt"      , "Stopa PDV-a"           );
      R_KCRP_CreateColumn           (ZXC.Q4un + ZXC.Qun2, 2, true, "Uk s PDV-om", "Ukupno s PDV-om");
      R_utilString_CreateColumn     (ZXC.Q5un, true, "OrigUgovor", "Originalni broj ugovora");
      R_utilUint_CreateColumn       (ZXC.Q3un           ,    true, "UGO"        , "UGO broj");

      vvtbT_cij            .JAM_ReadOnly = true;
      vvtbT_kol            .JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturURbDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Ulaz, clr_Sklad, clr_Ulaz);
   }

}

public class IZD_SVD_DUC         : FakturExtDUC
{
   #region Fld

   public VvHamper hamp_pacijent;

   #endregion Fld

   #region Constructor

   public IZD_SVD_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IZD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Pacijent_ZAH(out hamp_pacijent);

      CreateArrOfHampers();

      // 08.10.2021. mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_dokNum  .Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_napomena.Location = new Point(hamp_kupdobNaziv.Left                      , hamp_dokDate    .Bottom - ZXC.Qun4);
      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - ZXC.Q4un + ZXC.Qun4 + ZXC.Qun8;
      hamp_napomena.BringToFront();

      hamp_pacijent.Location = new Point(hamp_kupdobNaziv.Left, hamp_napomena.Bottom - ZXC.Qun4);

      nextY = hamp_pacijent.Bottom;

      hamp_IznosUvaluti.Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_pacijent,
                                    hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT 
                                  };

      //hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
      //                              hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
      //                              hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,
      //                              hamp_externLink1, hamp_externLink2,hamp_prjIdent//, hamp_opis
      //                            };

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
      //                                  hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
      //                                  hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
      //                                  hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent//, hampCbxM_opis                                   
      //                                };
   }


   private void InitializeHamper_Pacijent_ZAH(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth   , ZXC.Q6un - ZXC.Qun4 -ZXC.Qun8, ZXC.Q4un, ZXC.Q8un, ZXC.Q3un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Pac.Ime:"    , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (2, 0, "Pac.Prezime:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (4, 0, "Pac.MBO:"    , ContentAlignment.MiddleRight);
      tbx_PersonAName = hamper.CreateVvTextBox(1, 0, "tbx_PersonAName", "Pacijent Ime:"    , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.personName));
      tbx_OpciAlabel  = hamper.CreateVvTextBox(3, 0, "tbx_OpciAlabel ", "Pacijent Prezime:", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAlabel ));
      tbx_OpciAvalue  = hamper.CreateVvTextBox(5, 0, "tbx_OpciAvalue ", "Pacijent MBO:"    , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAvalue));
      tbx_OpciAvalue.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un,    true, "Šifra"   , "Šifra artikla"         );
      T_artiklName_CreateColumnFill(             true, "Naziv"   , "Naziv artikla"         );
      T_kol_CreateColumn           (ZXC.Q4un, 2, true, "Količina", "Količina"              );
      T_jedMj_CreateColumn         (ZXC.Q2un,    true, "JM"      , "Jedinica mjere"        );
      T_cij_CreateColumn           (ZXC.Q4un, 6, true, "Cijena"  , "Jedinična cijena"      );
    //R_KCRM_CreateColumn          (ZXC.Q4un, 2, true, "Iznos"   , "Ukupan iznos bez PDV-a");
      R_KC_CreateColumn            (ZXC.Q4un, 2, true, "Iznos"   , "Ukupan iznos bez PDV-a");

      //R_NC_CreateColumn      (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
      //R_NV_CreateColumn      (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
      //R_RUC_CreateColumn     (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
      //R_RUV_CreateColumn     (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");
      vvtbT_cij.JAM_ReadOnly = true;

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }
}

public class NRD_SVD_DUC         : FakturExtDUC
{
 //public override bool HasOrgBopCop => true;
   public override bool HasOrgBopCop { get { return true; } }

   #region Constructor

   public NRD_SVD_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_NRD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      // 08.10.2021. mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_dokNum   .Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_pdvZPkind.Location = new Point(hamp_dokDate.Right                         , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_vezniDok .Location = new Point(hamp_kupdobNaziv.Left                      , hamp_dokDate    .Bottom - ZXC.Qun4);

      hamp_projekt.Location  = new Point(hamp_vezniDok.Right                        , hamp_dokDate    .Bottom - ZXC.Qun4);
      hamp_napomena.Location = new Point(hamp_kupdobNaziv.Left                      , hamp_vezniDok   .Bottom - ZXC.Qun4);

      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - ZXC.Q4un + ZXC.Qun4 + ZXC.Qun8;
      hamp_vezniDok.BringToFront();
      
      nextY = hamp_napomena.Bottom;


      hamp_IznosUvaluti.Visible = false;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_dokDate    ,hamp_vezniDok, hamp_projekt, hamp_pdvZPkind,
                                    hamp_kupdobOther, hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   /*, hamp_v3TT  , hamp_v4TT*/
                                  };

      //hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
      //                              hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
      //                              hamp_OpciA, hamp_OpciB, hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX, 
      //                              hamp_dostava, hamp_externLink1, hamp_externLink2,hamp_prjIdent//,	hamp_opis
      //                            };

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
      //                                  hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
      //                                  hampCbxM_OpciA, hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme, hampCbxM_osobaX,
      //                                  hampCbxM_dostava, hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent//, hampCbxM_opis                                   
      //                                };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
 
      T_artiklCD_CreateColumn       (ZXC.Q3un           ,    true, "Šifra"       , "Šifra artikla");
      T_artiklName_CreateColumnFill (                        true, "Naziv"       , "Naziv artikla ili proizvoljan opis");
      R_ORG_T_doCijMal_CreateColumn (ZXC.Q3un           , 0, true, "ORG"         , "Originalno pakiranje");
      R_BOP_CreateColumn            (ZXC.Q4un           , 2, true, "BOP"         , "Broj Originalnog pakiranja");
      R_COP_CreateColumn            (ZXC.Q4un           , 4, true, "COP"         , "Cijena Originalnog pakiranja");
      T_kol_CreateColumn            (ZXC.Q4un           , 2, true, "Količina"    , "Količina");
      T_jedMj_CreateColumn          (ZXC.Q2un,               true, "JM"          , "Jedinica mjere"        );
      T_cij_CreateColumn            (ZXC.Q4un           , 4, true, "Cijena"      , "Jedinična cijena");
      T_rbt1St_CreateColumn         (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"         , "Stopa rabata 1");
      R_KCRM_CreateColumn           (ZXC.Q4un + ZXC.Qun2, 2, true, "Uk bez PDV-a", "Ukupno bez PDV-om");
      T_pdvSt_CreateColumn          (ZXC.Q2un           , 0, true, "PdvSt"       , "Stopa PDV-a");
      R_KCRP_CreateColumn           (ZXC.Q4un + ZXC.Qun2, 2, true, "Uk s PDV-om" , "Ukupno s PDV-om");
      R_utilString_CreateColumn     (ZXC.Q5un           ,    true, "OrigUgovor"  , "Originalni broj ugovora");
      R_utilUint_CreateColumn       (ZXC.Q3un           ,    true, "UGO"         , "UGO broj");

      vvtbT_cij.JAM_ReadOnly = true;
      vvtbT_kol.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturNarDobDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class UGODUC              : FakturExtDUC, IVvRealizableFakturDUC
{
   #region Constructor

   public UGODUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_UGO
         });

      RealizacGrid.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);
      RealizacGrid.Size     = new Size(RealizacGrid.Parent.Width - 2 * ZXC.QunMrgn, RealizacGrid.Parent.Height - 3 * ZXC.QunMrgn);

      RealizacGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      // 08.10.2021. mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, false, false, false);

      hamp_vezniDok .Location = new Point(hamp_kupdobNaziv.Left                     , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_someMoney.Location = new Point(hamp_kupdobNaziv.Left                     , hamp_vezniDok   .Bottom - ZXC.Qun4);
      hamp_dokNum   .Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width, hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_Status   .Location = new Point(hamp_kupdobNaziv.Right - hamp_Status.Width, hamp_dokNum     .Bottom - ZXC.Qun4);
      hamp_napomena .Location = new Point(hamp_kupdobNaziv.Left                     , hamp_Status     .Bottom - ZXC.Qun4);

      hamp_v1TT.Location     = new Point(hamp_tt.Left                              , hamp_tt  .Bottom - ZXC.Qun4);
      hamp_v2TT.Location     = new Point(hamp_tt.Left                              , hamp_v1TT.Bottom - ZXC.Qun4);
      hamp_v3TT.Location     = new Point(hamp_tt.Left                              , hamp_v2TT.Bottom - ZXC.Qun4);

      hamp_napomena .VvColWdt[1] = hamp_kupdobNaziv.Width - ZXC.Q4un + ZXC.Qun4 + ZXC.Qun8;
      hamp_vezniDok.BringToFront();

      nextY = hamp_napomena.Bottom;

      hamp_IznosUvaluti.Visible = false;
      tbx_someMoney.JAM_ReadOnly = true;

      if(ZXC.IsSvDUH)hamp_ugoSvDuh.Visible = true;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_vezniDok, 
                                    hamp_dokDate    , hamp_DospDate, hamp_dokNum, //hamp_someMoney,
                                    hamp_v1TT       , hamp_v2TT, hamp_v3TT, hamp_Status, hamp_napomena
                                   };

      //hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
      //                              hamp_osobaA ,hamp_OsobaB ,
      //                              hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,
      //                              hamp_externLink1, hamp_externLink2,
      //                              hamp_opis
      //                            };

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
      //                                  hampCbxM_OsobaA,hampCbxM_osobaB,
      //                                  hampCbxM_OpciB, hampCbxM_rokIspAndDate, hampCbxM_tipOtpreme,
      //                                  hampCbxM_externLink1, hampCbxM_externLink2,
      //                                  hampCbxM_opis                                   
      //                                };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn          (ZXC.Q3un           ,    true, "Šifra"        , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill    (                        true, "Naziv"        , "Naziv artikla ili proizvoljan opis");
      R_artiklLongOpis_CreateColumnFill(                        true, "Externi naziv", "Externi naziv artikla - Opis na samom artiklu");
      T_kol_CreateColumn               (ZXC.Q3un           , 2, true, "UgKol"        , "Ugovorena Količina"      );
      R_SVD_RealizKol_CreateColumn     (ZXC.Q3un           , 2, true, "UraKol"       , "Ostvarena Količina"      );
      T_jedMj_CreateColumn             (ZXC.Q2un           ,    true, "JM"           , "Jedinica mjere");
      T_cij_CreateColumn               (ZXC.Q4un           , 4, true, "Cijena"       , "Jedinična cijena");
     //T_rbt1St_CreateColumn            (ZXC.Q3un - ZXC.Qun4, 2, true, "Rbt"          , "Stopa rabata 1");
      T_pdvSt_CreateColumn             (ZXC.Q2un           , 0, true, "PdvSt"        , "Stopa PDV-a");
      R_KCRP_CreateColumn              (ZXC.Q4un + ZXC.Qun2, 2, true, "Ugovoreno"    , "Ukupno s PDV-om");

      R_SVDartRealizOG                 (ZXC.Q4un + ZXC.Qun2, 2, true, "Ostvareno"    , "Ostvareno");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturPRjDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RN, Color.Empty, clr_RN);
   }

   #region IVvRealizabeFakturDUC Members

   public List<Rtrans> RealizRtrList_AllYears { get; set; }
   public List<Rtrans> RealizRtrList_ThisYear { get; set; }

   #endregion

}

public class ZAH_SVD_DUC         : FakturExtDUC
{
   #region Fld

   public VvHamper hamp_statusZAH, hamp_klinika, hamp_odobrio, hamp_pacijent;
  
   #endregion Fld


   #region Constructor

   public ZAH_SVD_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_ZAH
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      InitializeHamper_Status_ZAH_SVD(out hamp_statusZAH);
      InitializeHamper_Klinika_ZAH(out hamp_klinika);
      InitializeHamper_Pacijent_ZAH(out hamp_pacijent);
      InitializeHamper_Odobrio_ZAH(out hamp_odobrio);

      CreateArrOfHampers();

      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();


      SetSumeHampers(false, true, true, false);

      hamp_dokDate.VvColWdt[0] = labelWidth - ZXC.Qun2;

      hamp_klinika.Location   = new Point(hamp_kupdobNaziv.Left                        , hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_statusZAH.Location = new Point(hamp_kupdobNaziv.Right - hamp_statusZAH.Width, hamp_kupdobNaziv.Bottom - ZXC.Qun4);
      hamp_dokDate.Location   = new Point(hamp_kupdobNaziv.Right - hamp_statusZAH.Width - hamp_dokDate.Width, hamp_kupdobNaziv.Bottom - ZXC.Qun4);


      hamp_napomena .Location = new Point(hamp_kupdobNaziv.Left                        , hamp_dokDate.Bottom - ZXC.Qun4);
    //hamp_napomena.VvColWdt[1] = ZXC.Q10un * 2 + ZXC.Q6un + ZXC.Q3un;
      
      hamp_dokNum.Location      = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width - ZXC.Qun2 - ZXC.Q3un, hamp_dokDate.Bottom - ZXC.Qun4);
      hamp_dokNum.SendToBack();
      hamp_napomena.BringToFront();

      hamp_pacijent.Location = new Point(hamp_kupdobNaziv.Left, hamp_napomena.Bottom - ZXC.Qun4);
      hamp_odobrio .Location = new Point(hamp_tt         .Left, hamp_napomena.Bottom - ZXC.Qun4);


      nextY = hamp_pacijent.Bottom;

      hamp_IznosUvaluti.Visible = false;

      tbx_KupdobCd  .JAM_ReadOnly = true;
      tbx_KupdobTk  .JAM_ReadOnly = true;
      tbx_KupdobName.JAM_ReadOnly = true;
      // 30.08.2022: na njihov zahtjev omogucavamo zadavanje skadista zahtjevnicarima 
    //tbx_SkladCd   .JAM_ReadOnly = true;
      tbx_TtNum     .JAM_ReadOnly = true;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt ,
                                    hamp_klinika,  hamp_statusZAH,
                                    hamp_dokDate    , hamp_dokNum,  hamp_napomena, 
                                    hamp_odobrio, hamp_pacijent,
                                    hamp_skladCd    , hamp_v1TT 
                                  };
   }

   private void InitializeHamper_Status_ZAH_SVD(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth- ZXC.Qun2, ZXC.QUN  + ZXC.Qun8, ZXC.Q5un- ZXC.Qun2 + ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel        (0, 0, "Status:", ContentAlignment.MiddleRight);
      tbx_Status     = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_Status", "Status", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.statusCD));
      tbx_StatusOpis = hamper.CreateVvTextBox      (2, 0, "tbx_StatusOpis", "Status ZAH opis", 32);
      tbx_Status.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Status.JAM_Set_LookUpTable(ZXC.luiListaRiskStatus, (int)ZXC.Kolona.prva);

      tbx_Status.JAM_lui_NameTaker_JAM_Name = tbx_StatusOpis.JAM_Name;
      tbx_StatusOpis.JAM_ReadOnly = true;
    //tbx_Status.JAM_lookUp_NOTobligatory = true;
       tbx_Status.JAM_IsSupressTab = true;

       tbx_Status.JAM_ReadOnly = true;
       tbx_Status.Font = ZXC.vvFont.BaseBoldFont;
       tbx_Status.TextAlign = HorizontalAlignment.Center;
   }

   private void InitializeHamper_Klinika_ZAH(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);

      hamper.VvColWdt    = new int[] { labelWidth  , ZXC.Q9un};
      hamper.VvSpcBefCol = new int[] { faBefFirstCol, faBefCol};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      tbx_KupdobUlica  = hamper.CreateVvTextBox(1, 0, "tbx_KupdobUlica ", "KupdobUlica ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdUlica));
      tbx_KupdobZip    = hamper.CreateVvTextBox(1, 0, "tbx_KupdobZip   ", "KupdobZip   ", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdZip));
      tbx_KupdobMjesto = hamper.CreateVvTextBox(1, 0, "tbx_KupdobMjesto", "KupdobMjesto", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.kdMjesto));

      
                     hamper.CreateVvLabel  (0, 0, "", ContentAlignment.MiddleRight);
      tbx_KdAdresa = hamper.CreateVvTextBox(1, 0, "kdAdresa", "", 74);

      tbx_KdAdresa.JAM_ReadOnly = true;
      tbx_KupdobUlica.Visible =
      tbx_KupdobZip.Visible =
      tbx_KupdobMjesto.Visible = false;
   }

   private void InitializeHamper_Odobrio_ZAH(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth, ZXC.Q10un +ZXC.QUN + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Odobr.:", ContentAlignment.MiddleRight);
      //tbx_PersonBName = hamper.CreateVvTextBox(1, 0, "tbx_odgvPersName", "Odobrio/la", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.odgvPersName));
      //tbx_PersonBName.JAM_ReadOnly = true;
      tbx_Napomena2 = hamper.CreateVvTextBox(1, 0, "tbx_odgvPersName", "Odobrio/la", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.napomena2));
      tbx_Napomena2.JAM_ReadOnly = true;

   }

   private void InitializeHamper_Pacijent_ZAH(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", null, false);

      hamper.VvColWdt      = new int[] { labelWidth,    ZXC.Q6un, ZXC.Q4un - ZXC.Qun8, ZXC.Q10un + ZXC.Qun2 + ZXC.Qun4 - ZXC.Qun10, ZXC.Q3un, ZXC.Q5un+ ZXC.Qun12 };
      hamper.VvSpcBefCol   = new int[] { faBefFirstCol, faBefCol, faBefCol, faBefCol, faBefCol, faBefCol };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Pac.Ime:"    , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (2, 0, "Pac.Prezime:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel  (4, 0, "Pac.MBO:"    , ContentAlignment.MiddleRight);
      tbx_PersonAName = hamper.CreateVvTextBox(1, 0, "tbx_PersonAName", "Pacijent Ime:"    , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.personName));
      tbx_OpciAlabel  = hamper.CreateVvTextBox(3, 0, "tbx_OpciAlabel ", "Pacijent Prezime:", GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAlabel ));
      tbx_OpciAvalue  = hamper.CreateVvTextBox(5, 0, "tbx_OpciAvalue ", "Pacijent MBO:"    , GetDB_ColSize_namedDao(TheVvDaoExt, DB_ciex.opciAvalue));
      tbx_OpciAvalue.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
   }


   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q3un,    true, "Šifra"    , "Šifra artikla"         );
      T_artiklName_CreateColumnFill(             true, "Naziv"    , "Naziv artikla"         );
      T_kol_CreateColumn           (ZXC.Q4un, 2, true, "Količina" , "Količina"              );
      T_jedMj_CreateColumn         (ZXC.Q2un,    true, "JM"       , "Jedinica mjere"        );
      T_cij_CreateColumn           (ZXC.Q4un, 6, true, "Cijena"   , "Jedinična cijena"      );
      R_KC_CreateColumn            (ZXC.Q4un, 2, true, "Iznos"    , "Ukupan iznos bez PDV-a");
      R_utilUint_CreateColumn      (ZXC.Q4un   , true, "Izdatnica", "IZDATNICA broj"        );

      vvtbT_cij.JAM_ReadOnly = true;
      this.ControlForInitialFocus = TheG;

   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   //internal /*protected*/ override List<VvMigrator> MigratorList
   //{
   //   get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   //}

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_UPA, clr_RUC);
   }
}

#endregion SVD


#region TETRAGRAM

public class PON_MPC_DUC           : FakturExtDUC
{
   #region Constructor

   public PON_MPC_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_PON
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
     
      SetSumeHampers(false, true, true, false);

      hamp_PonudDate.Location = new Point(hamp_DospDate.Left, hamp_DospDate.Bottom - ZXC.Qun4);
      hamp_RokPonude.Location = new Point(hamp_RokPlac.Left, hamp_DospDate.Bottom - ZXC.Qun4);
     
    //hamp_opis.Location = new Point(hamp_tt.Right, hamp_tt.Top);
      hamp_opis.Location = new Point(hamp_v1TT.Right, hamp_tt.Top);
      hamp_opis.BringToFront();

      hamp_incognitoPrint.Location = new Point(ZXC.Qun4, hamp_IznosUvaluti.Top);
      hamp_incognitoPrint.Visible = true;
      cbx_PrintIzjava    .Visible = false;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd,
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_pdvGeokind, hamp_PonudDate, hamp_RokPonude, hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_opis, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent, hamp_DatumX//,/*ovaj DatumX dodan 01.02.24.*///hamp_PonudDate, 08.11.2013. on je na osnovnom
                                    //hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_DatumX//, //hampCbxM_PonudDate, 08.11.2013 on je na sonovnom
                                        //hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible       = true;
      bool isOrgPakVisible = ZXC.RRD.Dsc_IsOrgPakVisible;

      T_artiklCD_CreateColumn  (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_KPD_CreateColumn       (ZXC.Q3un,             isVisible, "KPD"         , "KPD");
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un+ ZXC.Qun4, 2,isVisible, "Cij bez PDV"     , "Jedinična cijena");

    //T_rbt1St_CreateColumn    (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
  
    //R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
    //R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
    //R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
    //R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
    //R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn    (ZXC.Q2un, 0, isVisible, "PdvSt", "Stopa PDV-a");
      T_pdvKolTip_CreateColumn(ZXC.QUN, isVisible);

      T_IRA_MPC_CreateColumn(ZXC.Q4un+ ZXC.Qun4, 2, isVisible, "Cijena s PDV", "Jedinična cijena");
      
      R_KCRP_CreateColumn(ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
      T_ppmvOsn_CreateColumn(ZXC.Q5un, 2, false, "Osnovica", "Osnovica za obračun pdv-a na umjetninu", false);


      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPonudaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class OPN_MPC_DUC           : FakturExtDUC
{
   #region Constructor

   public OPN_MPC_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_OPN
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();
     
      SetSumeHampers(false, true, true, false);

      hamp_PonudDate.Location = new Point(hamp_DospDate.Left, hamp_DospDate.Bottom - ZXC.Qun4);
      hamp_RokPonude.Location = new Point(hamp_RokPlac.Left, hamp_DospDate.Bottom - ZXC.Qun4);
     
    //hamp_opis.Location = new Point(hamp_tt.Right, hamp_tt.Top);
      hamp_opis.Location = new Point(hamp_v1TT.Right, hamp_tt.Top);
      hamp_opis.BringToFront();

      hamp_incognitoPrint.Location = new Point(ZXC.Qun4, hamp_IznosUvaluti.Top);
      hamp_incognitoPrint.Visible = true;
      cbx_PrintIzjava    .Visible = false;

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , hamp_skladCd,
                                    hamp_kupdobOther, hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_pdvGeokind, hamp_PonudDate, hamp_RokPonude, hamp_Cjenik, hamp_napomena, 
                                    hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_opis, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme, hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent, hamp_DatumX//,/*ovaj DatumX dodan 01.02.24.*///hamp_PonudDate, 08.11.2013. on je na osnovnom
                                    //hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme, hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent, hampCbxM_DatumX//, //hampCbxM_PonudDate, 08.11.2013 on je na sonovnom
                                        //hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible       = true;
      bool isOrgPakVisible = ZXC.RRD.Dsc_IsOrgPakVisible;

      T_artiklCD_CreateColumn      (ZXC.Q4un,                                    isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                                             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_KPD_CreateColumn           (ZXC.Q3un,                                    isVisible, "KPD"         , "KPD");
      T_kol_CreateColumn           (ZXC.Q3un, 2,                                 isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,                                 isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un+ ZXC.Qun4, 2                      , isVisible, "Cij bez PDV", "Jedinična cijena");
      T_kol2_CreateColumn          (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces, isVisible, "IspKol"     , "Isporučena količina");
      R_OPN_neispkol_CreateColumn  (ZXC.Q3un, ZXC.RRD.Dsc_KolNumOfDecimalPlaces, isVisible, "NeIspKol" , "NE Isporučena količina");
    //T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"        , "Stopa rabata 1");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
  
    //R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
    //R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
    //R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
    //R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
    //R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn    (ZXC.Q2un, 0, isVisible, "PdvSt", "Stopa PDV-a");
      T_pdvKolTip_CreateColumn(ZXC.QUN, isVisible);

      T_IRA_MPC_CreateColumn(ZXC.Q4un+ ZXC.Qun4, 2, isVisible, "Cijena s PDV", "Jedinična cijena");
      
      R_KCRP_CreateColumn(ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
      T_ppmvOsn_CreateColumn(ZXC.Q5un, 2, false, "Osnovica", "Osnovica za obračun pdv-a na umjetninu", false);


      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPonudaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(Color.Empty, clr_Sklad, Color.Empty);
   }
}

public class IRA_MPC_DUC              : FakturExtDUC
{
   #region Constructor

   public IRA_MPC_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IRA
         });

      tbx_VezniDok.JAM_ReadOnly = VezniDokIsReadOnly;
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(true, true, true, false);

      hamp_DatumX     .Location = new Point(hamp_NacPlac  .Left , hamp_NacPlac  .Bottom - ZXC.Qun4);
      hamp_VezniDok2  .Location = new Point(hamp_fiskJIR  .Left , hamp_fiskJIR  .Bottom);
      hamp_Fco        .Location = new Point(hamp_VezniDok2.Right, hamp_fiskJIR  .Bottom);
      hamp_napomena2  .Location = new Point(hamp_fiskJIR  .Left , hamp_VezniDok2.Bottom);
      hamp_tipOtpreme .Location = new Point(hamp_napomena2.Right, hamp_VezniDok2.Bottom);
      hamp_eRproc     .Location = new Point(                           0, hamp_DatumX.Bottom - ZXC.Qun4);
      hamp_Status     .Location = new Point(hamp_eRproc.Right - ZXC.Qun4, hamp_DatumX.Bottom - ZXC.Qun4);

    //hamp_opis       .Location = new Point(hamp_tt       .Right, hamp_tt       .Top);
      hamp_opis       .Location = new Point(hamp_v1TT.Right     , hamp_tt.Top);
      hamp_opis.BringToFront();

    //nextY = hamp_DatumX.Bottom;
      nextY = hamp_eRproc.Bottom;

      if(ZXC.RRD.Dsc_IsIntrastat == false)
      {
         hamp_VezniDok2.Visible = false;
         hamp_Fco       .Visible = false;
         hamp_napomena2 .Visible = false;
         hamp_tipOtpreme.Visible = false;
      }

      hamp_incognitoPrint.Location = new Point(ZXC.Qun4, hamp_IznosUvaluti.Top);
      hamp_incognitoPrint.Visible = true;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_SkladDate, hamp_PDV, hamp_pdvZPkind, hamp_pdvGeokind, hamp_kupdobOther, hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT, hamp_NacPlac, hamp_fiskJIR,
                                    hamp_DatumX, hamp_VezniDok2, hamp_napomena2, hamp_Fco, hamp_tipOtpreme, hamp_opis, hamp_eRproc
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat,/* hamp_napomena2,*/
                                    /*hamp_VezniDok2, hamp_Fco, hamp_DatumX,*/  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, /*hamp_tipOtpreme,*/  hamp_osobaX,hamp_carinaKind,
                                    hamp_dostava, hamp_PonudDate,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,hamp_fiskMsgID    , hamp_fiskOibOp,     hamp_fiskPrgBr,
                                    /*hamp_eRproc,*/ hamp_fiskPrgBr//, hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, /*hampCbxM_napomena2,*/
                                        /*hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_DatumX,*/ hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, /*hampCbxM_tipOtpreme,*/ hampCbxM_osobaX, hampCbxM_carinaKind,
                                        hampCbxM_dostava, hampCbxM_PonudDate,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,hampCbxM_fiskMsgID, hampCbxM_fiskOibOp, hampCbxM_fiskPrgBr,
                                        /*hampCbxM_eRproc,*/ hampCbxM_fiskPrgBr//, hampCbxM_opis
                                      };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un          ,   isVisible, "Šifra"           , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"           , "Naziv artikla");
      T_KPD_CreateColumn           (ZXC.Q3un  , ZXC.IsF2_2026_rules, "KPD"           , "KPD sifra");
    //T_isIrmUsluga_CreateColumn   (ZXC.QUN + ZXC.Qun4,   isVisible, "Usl"             , "Usluga");
    //T_konto_CreateColumn         (ZXC.Q3un          ,   isVisible, "Konto"           , "Konto knjiženja retka (trošak/prihod/sklad/ ....)");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"             , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible, "JM"              , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un+ ZXC.Qun4, 2,isVisible, "Cij bez PDV", "Jedinična cijena");
    //T_rbt1St_CreateColumn        (ZXC.Q3un-ZXC.Qun4, 2, isVisible, "Rb1"             , "Stopa rabata 1");
      R_KCR_CreateColumn           (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv"      , "Ukupan iznos bez PDV-a");

      R_cij_kcr_CreateColumn       (ZXC.Q4un, 2          , false, "VPC"      , "Veleprodajna cijena"        );
      R_NC_CreateColumn            (ZXC.Q4un, 2          , false, "NabCij"   , "Nabavna cijena"             );
      R_NV_CreateColumn            (ZXC.Q4un, 2          , false, "NabVri"   , "Nabavna vrijednost"         );
      R_RUC_CreateColumn           (ZXC.Q4un, 2          , false, "RUC"      , "RUC - razlika u cijeni"     );
      R_RUV_CreateColumn           (ZXC.Q4un, 2          , false, "RUV"      , "RUV - razlika u vrijednosti");
      R_utilString_CreateColumn    (ZXC.Q5un             , false, "UlazniDok", "Broj ulaznog dokumenta"     );

      T_pdvSt_CreateColumn         (ZXC.Q2un, 0          , isVisible, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN              , isVisible);

      T_IRA_MPC_CreateColumn      (ZXC.Q4un+ ZXC.Qun4,  2,  isVisible, "Cijena s PDV"     , "Jedinična cijena");
     
      R_KCRP_CreateColumn         (ZXC.Q4un + ZXC.Qun2 , 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");

      T_ppmvOsn_CreateColumn(ZXC.Q5un, 2, false, "Osnovica", "Osnovica za obračun pdv-a na umjetninu", false);

      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRbDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, clr_Izlaz);
   }

   //public override bool HasDscSubVariants
   //{
   //   get
   //   {
   //      return true;
   //   }
   //}

   protected override bool VezniDokIsReadOnly 
   { 
      get 
      {
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.VlastitoKnjigovodstvo_F2_ALL) return true; // Tamara, Mirjana, ... VezniDok nastaje automatski pri sejvanju 
         if(ZXC.IsF2_2026_rules && ZXC.CURR_prjkt_rec.F2_RolaKind == ZXC.F2_RolaKind.KlijentServisa_TipB  ) return true; // IMA  importa Izlaznih racuna ... Tetragram .

         return false; 
      } 
   }

}

public class IZD_MPC_DUC        : FakturExtDUC
{
   #region Constructor

   public IZD_MPC_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_IZD
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      SetParentOfhampers();

      SetLocationMigrators();

      SetSumeHampers(false, true, true, false);

   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    /*hamp_kupdobOther,*/ hamp_konto  , hamp_ZiroRn, hamp_ValName , hamp_Pnb, hamp_Status  , hamp_vezniDok, hamp_projekt, 
                                    hamp_dokDate    , hamp_RokPlac, hamp_dokNum, hamp_DospDate, hamp_pdvGeokind, hamp_SkladDate, hamp_PonudDate, hamp_kupdobOther,hamp_Cjenik, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   , hamp_v3TT  , hamp_v4TT
                                  };

      hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
                                    hamp_VezniDok2, hamp_Fco, hamp_NacPlac,  hamp_osobaA, hamp_OsobaB ,
                                    hamp_OpciA, hamp_OpciB,  hamp_rokIspAndDate, hamp_tipOtpreme,  hamp_osobaX,
                                    hamp_externLink1, hamp_externLink2,hamp_prjIdent,
                                    hamp_opis
                                  };

      hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
                                        hampCbxM_VezniDok2, hampCbxM_Fco, hampCbxM_NacPlac, hampCbxM_OsobaA, hampCbxM_osobaB,
                                        hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_rokIspAndDate	, hampCbxM_tipOtpreme,  hampCbxM_osobaX,
                                        hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
                                        hampCbxM_opis                                   
                                      };
   }

   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible       = true;

      T_artiklCD_CreateColumn  (ZXC.Q4un,             isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                  isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn       (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn     (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn       (ZXC.Q4un+ ZXC.Qun4, /*2*/6,isVisible, "Cij bez PDV"     , "Jedinična cijena");

      R_KCR_CreateColumn       (ZXC.Q4un, 2,          isVisible, "Uk bez Pdv" , "Ukupan iznos bez PDV-a");
  
    //R_cij_kcr_CreateColumn(ZXC.Q4un, 2, false, "VPC"   , "Veleprodajna cijena");
    //R_NC_CreateColumn     (ZXC.Q4un, 2, false, "NabCij", "Nabavna cijena");
    //R_NV_CreateColumn     (ZXC.Q4un, 2, false, "NabVri", "Nabavna vrijednost");
    //R_RUC_CreateColumn    (ZXC.Q4un, 2, false, "RUC"   , "RUC - razlika u cijeni");
    //R_RUV_CreateColumn    (ZXC.Q4un, 2, false, "RUV"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn    (ZXC.Q2un, 0, isVisible, "PdvSt", "Stopa PDV-a");
      T_pdvKolTip_CreateColumn(ZXC.QUN, isVisible);

      T_IRA_MPC_CreateColumn  (ZXC.Q4un+ ZXC.Qun4, /*2*/6, isVisible, "Cijena s PDV", "Jedinična cijena");
      
      R_KCRP_CreateColumn     (ZXC.Q4un + ZXC.Qun2, 2, isVisible, "Uk s PDV-om", "Ukupno s PDV-om");
    //T_ppmvOsn_CreateColumn  (ZXC.Q5un, 2, false, "Osnovica", "Osnovica za obračun pdv-a na umjetninu", false);

      vvtbT_cij.JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, Color.Empty);
   }
}

public class POT_DUC         : FakturExtDUC
{
   #region Constructor

   public POT_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_POT
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      // kao kod svd  mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_SkladDate .Location = new Point(hamp_dokDate.Right   , hamp_dokDate    .Top   );

      hamp_ValName   .Location = new Point(hamp_dokDate.Left   , hamp_dokDate    .Bottom - ZXC.Qun4);

      hamp_vezniDok  .Location = new Point(hamp_kupdobOther.Left, hamp_kupdobOther.Bottom- ZXC.Qun4);
      hamp_VezniDok2 .Location = new Point(hamp_kupdobOther.Left, hamp_vezniDok   .Bottom);
      hamp_Fco       .Location = new Point(hamp_kupdobOther.Left, hamp_VezniDok2  .Bottom);

      hamp_ZiroRn    .Location = new Point(hamp_kupdobOther.Right - ZXC.Qun8 - ZXC.Qun12, hamp_kupdobOther.Bottom - ZXC.Qun4);
      hamp_NacPlac   .Location = new Point(hamp_kupdobOther.Right - ZXC.Qun8 - ZXC.Qun12, hamp_ZiroRn.Bottom      - ZXC.Qun4);
      hamp_prjArtName.Location = new Point(hamp_v4TT.Left                        , hamp_v4TT.Bottom               - ZXC.Qun4);

      hamp_opis.Location    = new Point(hamp_v1TT.Right, hamp_tt.Top);
      hamp_opis.BringToFront();
      hamp_NacPlac.BringToFront();
      
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_ZiroRn, hamp_vezniDok,  hamp_VezniDok2, hamp_Fco,
                                    hamp_dokDate    , hamp_ValName , hamp_dokNum,  hamp_SkladDate, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT   , hamp_v2TT   , hamp_v3TT  , hamp_v4TT,
                                    hamp_NacPlac, hamp_opis, hamp_prjArtName, hamp_posJedCd
                                  };

      //hamperMigr = new VvHamper[] { hamp_posJedCd, hamp_Mtros, hamp_PrimPlat, hamp_napomena2,
      //                              hamp_VezniDok2, hamp_Fco, /*hamp_NacPlac,*/  hamp_osobaA, hamp_OsobaB ,
      //                              hamp_OpciA, hamp_OpciB,  hamp_osobaX,/* hamp_rokIsporuke, hamp_rokIspDate, hamp_tipOtpreme,*/
      //                              hamp_externLink1, hamp_externLink2,hamp_prjIdent/*, hamp_opis*/
      //                            };

      //hamperCbx4Migr = new VvHamper[] { hampCbxM_posJedCd, hampCbxM_Mtros, hampCbxM_PrimPlat, hampCbxM_napomena2,
      //                                  hampCbxM_VezniDok2, hampCbxM_Fco, /*hampCbxM_NacPlac,*/ hampCbxM_OsobaA, hampCbxM_osobaB,
      //                                  hampCbxM_OpciA, hampCbxM_OpciB,  hampCbxM_osobaX,/* hampCbxM_rokIsporuke, hampCbxM_rokIspDate	, hampCbxM_tipOtpreme,*/
      //                                  hampCbxM_externLink1, hampCbxM_externLink2,hampCbxM_prjIdent,
      //                                  hampCbxM_opis                                   
      //                                };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un   , isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2, isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   , isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4, isVisible, "Cijena"     , "Jedinična cijena");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2, isVisible, "Iznos"      , "Iznos");
      T_pdvKolTip_CreateColumn     (ZXC.QUN    , isVisible);
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPrimkaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_Sklad, clr_klkPri);
   }
}

public class POU_DUC         : FakturExtDUC
{
   #region Constructor

   public POU_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_POU
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
     
      // kao kod svd  mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();

      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_dokNum.Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width, hamp_kupdobOther.Top);
      

      hamp_napomena.Location = new Point(hamp_kupdobOther.Left, hamp_kupdobOther.Bottom - ZXC.Qun4);
      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - labelWidth - ZXC.QUN;

      hamp_opis.Location = new Point(hamp_kupdobOther.Left, hamp_napomena.Bottom);
      hamp_opis.BringToFront();

      nextY = hamp_opis.Bottom;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   /*, hamp_v3TT  , hamp_v4TT*/, hamp_opis
                                  };

   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un   ,          isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2,          isVisible, "Iznos"      , "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturPrimkaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_klkPri, clr_Sklad, clr_klkPri);
   }
}

public class POI_DUC         : FakturExtDUC
{
   #region Constructor

   public POI_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_POI
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();
     
      // kao kod svd  mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();

      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_dokNum.Location = new Point(hamp_kupdobNaziv.Right - hamp_dokNum.Width, hamp_kupdobOther.Top);
      

      hamp_napomena.Location = new Point(hamp_kupdobOther.Left, hamp_kupdobOther.Bottom - ZXC.Qun4);
      hamp_napomena.VvColWdt[1] = hamp_kupdobNaziv.Width - labelWidth - ZXC.QUN;

      hamp_opis.Location = new Point(hamp_kupdobOther.Left, hamp_napomena.Bottom);
      hamp_opis.BringToFront();

      nextY = hamp_opis.Bottom;
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_tt , 
                                    hamp_kupdobOther, hamp_dokDate    , hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT       , hamp_v2TT   /*, hamp_v3TT  , hamp_v4TT*/, hamp_opis
                                  };

   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      bool isVisible = true;

      T_artiklCD_CreateColumn      (ZXC.Q4un   ,          isVisible, "Šifra"      , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(                      isVisible, "Naziv"      , "Naziv artikla ili proizvoljan opis");
      T_kol_CreateColumn           (ZXC.Q3un, 2,          isVisible, "Kol"        , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   ,          isVisible, "JM"         , "Jedinica mjere");
      T_cij_CreateColumn           (ZXC.Q4un, 4,          isVisible, "Cijena"     , "Jedinična cijena");
      R_KCRM_CreateColumn          (ZXC.Q4un, 2,          isVisible, "Iznos"      , "Iznos");
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIzdatnicaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Izlaz, clr_Sklad, Color.Empty);
   }
}

public class ZAR_DUC         : FakturExtDUC
{
   #region Constructor

   public ZAR_DUC(Control parent, Faktur _faktur, VvForm.VvSubModul vvSubModul) : base(parent, _faktur, vvSubModul)
   {
      dbNavigationRestrictor_TT = new ZXC.DbNavigationRestrictor
         (Faktur.tt_colName, new string[] 
         { 
            Faktur.TT_ZAR
         });
   }

   #endregion Constructor

   #region HamperLocation

   protected override void SetLocationAndParentOfHampersOnBaby()
   {
      CreateArrOfHampers();

      // kao kod svd  mičemo migratore sa proširenog i dodajemo ove dvije metode dolje kako bi ispravno radilo
      //SetParentOfhampers();
      //SetLocationMigrators();
      SetParentOfHamperLeftHampers();
      panel_MigratorsLeftB.SendToBack();

      SetSumeHampers(false, true, true, false);

      hamp_SkladDate .Location = new Point(hamp_dokDate.Left   , hamp_dokDate    .Bottom - ZXC.Qun4);
      
      hamp_ValName   .Location = new Point(hamp_dokDate.Left   , hamp_dokDate    .Bottom - ZXC.Qun4);
 
      hamp_VezniDok2 .Location = new Point(hamp_kupdobOther.Left, hamp_kupdobOther.Bottom); //telefon
      hamp_Fco       .Location = new Point(hamp_kupdobOther.Left, hamp_VezniDok2  .Bottom); //e-mail

      hamp_NacPlac   .Location = new Point(hamp_kupdobOther.Right - ZXC.Qun8 - ZXC.Qun12, hamp_kupdobOther.Bottom - ZXC.Qun4);

      hamp_PrimPlat  .Location = new Point(hamp_kupdobOther.Left, hamp_Fco     .Bottom + ZXC.Qun4); // dobavljac             
      hamp_dostava   .Location = new Point(hamp_kupdobOther.Left, hamp_PrimPlat.Bottom + ZXC.Qun8); // adresa, OIB dobavljaca
      
      hamp_napomena .Location = new Point(hamp_kupdobOther.Left, hamp_dostava.Bottom);
      hamp_DatumX   .Location = new Point(hamp_v1TT       .Left, hamp_v4TT   .Bottom + ZXC.Qun8);
      hamp_napomena2.Location = new Point(hamp_v1TT       .Left, hamp_DatumX .Bottom + ZXC.Qun8);

      hamp_opis.Location    = new Point(hamp_v1TT.Right, hamp_tt.Top);
      hamp_opis.BringToFront();
      hamp_NacPlac.BringToFront();
      
      nextY = hamp_napomena.Bottom;

      //hamp_ValName.BringToFront();
   }

   private void CreateArrOfHampers()
   {
      hamperLeft = new VvHamper[] { hamp_kupdobNaziv, hamp_kupdobOther, 
                                    hamp_PrimPlat, hamp_dostava     ,                      
                                    hamp_tt , hamp_SkladDate,
                                    hamp_VezniDok2, hamp_Fco, hamp_DatumX,
                                    hamp_dokDate    , hamp_ValName , hamp_dokNum, hamp_napomena, 
                                    hamp_skladCd    , hamp_v1TT   , hamp_v2TT   , hamp_v3TT  , hamp_v4TT,
                                    hamp_NacPlac, hamp_napomena2, hamp_opis
                                  };
   }
 
   #endregion HamperLocation

   #region TheG_Specific_Columns

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn      (ZXC.Q4un,    true, "Šifra"           , "Šifra artikla"                     );
      T_artiklName_CreateColumnFill(             true, "Naziv"           , "Naziv artikla");
      T_kol_CreateColumn           (ZXC.Q3un, 2, true, "Kol"             , "Količina"      );
      T_jedMj_CreateColumn         (ZXC.Q2un   , true, "JM"              , "Jedinica mjere");
    
      T_ppmvOsn_CreateColumn       (ZXC.Q5un,  2, true, "NabavnaCij", "Nabavna cijena", false);
      R_NV_ZAR_CreateColumn        (ZXC.Q4un,  2, true, "NabavnaVri"   , "Nabavna vrijednost"         );

      T_cij_CreateColumn(ZXC.Q4un+ ZXC.Qun4 , 2, true, "Cij bez PDV", "Jedinična cijena");
      R_KCR_CreateColumn           (ZXC.Q4un, 2, true, "Uk bez Pdv"      , "Ukupan iznos bez PDV-a");

      R_RUV_ZAR_CreateColumn       (ZXC.Q4un, 2, true , "PdvOsn"   , "RUV - razlika u vrijednosti");

      T_pdvSt_CreateColumn         (ZXC.Q2un, 0, true, "PdvSt"      , "Stopa PDV-a");
      T_pdvKolTip_CreateColumn     (ZXC.QUN    , true);

      T_IRA_MPC_CreateColumn      (ZXC.Q5un, 2, true, "Cijena s PDV"     , "Jedinična cijena");
      R_KCRP_CreateColumn         (ZXC.Q5un, 2, true, "Uk s PDV-om", "Ukupno s PDV-om");

      vvtbT_cij      .JAM_ReadOnly = true;
      vvtbT_pdvKolTip.JAM_ReadOnly = true;
      vvtbT_pdvSt    .JAM_ReadOnly = true;
   }

   #endregion TheG_Specific_Columns

   #region overrideMigratorList

   internal /*protected*/ override List<VvMigrator> MigratorList
   {
      get { return ZXC.TheVvForm.VvPref.fakturIRaDUC.MigratorStates; }
   }

   #endregion overrideMigratorList

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_OPL_PTG, Color.Empty, clr_OPL_PTG);
   }
}

#endregion TETRAGRAM


#region Fiskalizacija F2

public class F2_FUR_addNpomenaUFA_Dlg : VvDialog
{
   #region Filedz

   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;
   private VvTextBox tbx_napomena;
   private Label     lbl_racun;

   #endregion Filedz

   #region Constructor

   public F2_FUR_addNpomenaUFA_Dlg(Faktur faktur_rec)
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Dodaj napomenu na ulazni račun " + faktur_rec.TT + "-" + faktur_rec.TtNum.ToString();

      CreateHamper(faktur_rec);

      dlgWidth  = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_napomena , ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

      Fld_Napomena = faktur_rec.Napomena;
   }

   #endregion Constructor
   
   #region hamper

   private void CreateHamper(Faktur faktur_rec)
   {
      hamper          = new VvHamper(2, 2, "", this, false);
      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QUN);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un*2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4    };
      hamper.VvRightMargin = 0;

      hamper.VvRowHgt       = new int[] { ZXC.QUN , ZXC.QUN  };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string label = "Račun " + faktur_rec.TT + "-" +  faktur_rec.TtNum.ToString() + " " + faktur_rec.KupdobName + " ";

      lbl_racun = hamper.CreateVvLabel(0, 0, label, 1, 0, ContentAlignment.MiddleLeft);
      lbl_racun.Font = ZXC.vvFont.BaseBoldFont;

                     hamper.CreateVvLabel  (0, 1, "Napomena:", ContentAlignment.MiddleRight);
      tbx_napomena = hamper.CreateVvTextBox(1, 1, "tbx_napomena", "Napomena");
   }
   
   #endregion hamper
   
   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_

   public string Fld_Napomena { get { return tbx_napomena.Text; } set { tbx_napomena.Text = value; }   }

   #endregion Fld_
}

public class F2_NIR_MapajRazdoblje_Dlg : VvDialog
{
   #region Filedz

   private Button    okButton, cancelButton;
   private VvHamper  hamper;
   private int       dlgWidth, dlgHeight;

   private VvDateTimePicker dtp_DatumOD, dtp_DatumDO;
   private VvTextBox        tbx_DatumOD, tbx_DatumDO;

   #endregion Filedz

   #region Constructor

   public F2_NIR_MapajRazdoblje_Dlg()
   {
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text          = "Ödaberite razdoblje za prijavu naplate";

      CreateHamper();

      dlgWidth  = hamper.Right + ZXC.QunMrgn;
      dlgHeight = hamper.Bottom + ZXC.QunMrgn * 2 + ZXC.QunBtnH;
      this.ClientSize = new Size(dlgWidth, dlgHeight);

      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      VvHamper.Open_Close_Fields_ForWriting(tbx_DatumOD, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);
      VvHamper.Open_Close_Fields_ForWriting(tbx_DatumDO, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvDialog);

   }

   #endregion Constructor
   
   #region hamper

   private void CreateHamper()
   {
      hamper = new VvHamper(4, 3, "", this, false);

      hamper.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2, ZXC.Qun8, ZXC.Qun2, ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.QUN;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Razdoblje uplata za prijavu naplata:", 3, 0, ContentAlignment.MiddleLeft);


                    hamper.CreateVvLabel  (0, 1, "OD datuma:", ContentAlignment.MiddleRight);
      tbx_DatumOD = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

                    hamper.CreateVvLabel  (2, 1, "DO datuma:", ContentAlignment.MiddleRight);
      tbx_DatumDO = hamper.CreateVvTextBox(3, 1, "tbx_datumDo", "", 12);
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO = hamper.CreateVvDateTimePicker(3, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag = tbx_DatumDO;
      tbx_DatumDO.Tag = dtp_DatumDO;

      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu =
      tbx_DatumDO.ContextMenu = dtp_DatumDO.ContextMenu = CreateNewContexMenu_Date();
   }
  
   private VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"           , IspuniDatume),
               new MenuItem("Tekuća godina"   , IspuniDatume),
               new MenuItem("Tekući mjesec"   , IspuniDatume),
               new MenuItem("Prvi kvartal"    , IspuniDatume),
               new MenuItem("Drugi kvartal"   , IspuniDatume),
               new MenuItem("Treći kvartal"   , IspuniDatume),
               new MenuItem("Četvrti kvartal" , IspuniDatume),
               new MenuItem("1 -11 mjesec"    , IspuniDatume),
               new MenuItem("1 -10 mjesec"    , IspuniDatume),
               new MenuItem("1 - 9 mjesec"    , IspuniDatume),
               new MenuItem("1 - 8 mjesec"    , IspuniDatume),
               new MenuItem("1 - 7 mjesec"    , IspuniDatume),
               new MenuItem("1 - 6 mjesec"    , IspuniDatume),
               new MenuItem("1 - 5 mjesec"    , IspuniDatume),
               new MenuItem("1 - 4 mjesec"    , IspuniDatume),
               new MenuItem("1 - 3 mjesec"    , IspuniDatume),
               new MenuItem("1 - 2 mjesec"    , IspuniDatume),
               new MenuItem("Siječanj"        , IspuniDatume),
               new MenuItem("Veljača"         , IspuniDatume),
               new MenuItem("Ožujak"          , IspuniDatume),
               new MenuItem("Travanj"         , IspuniDatume),
               new MenuItem("Svibanj"         , IspuniDatume),
               new MenuItem("Lipanj"          , IspuniDatume),
               new MenuItem("Srpanj"          , IspuniDatume),
               new MenuItem("Kolovoz"         , IspuniDatume),
               new MenuItem("Rujan"           , IspuniDatume),
               new MenuItem("Listopad"        , IspuniDatume),
               new MenuItem("Studeni"         , IspuniDatume),
               new MenuItem("Prosinac"        , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }
   private void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = ZXC.projectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"  : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"  : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"   : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"  : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"  : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal": textOd = "10"; textDo = "31.12"; break;
         case "1 -11 mjesec"   : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"   : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"   : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"   : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"   : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"   : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"   : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"   : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"   : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"   : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"       : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"        : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"         : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"        : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"        : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"         : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"         : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"        : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"          : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"       : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"        : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"       : textOd = "12"; textDo = "31.12"; break;
         case "Danas"          : textOd = ""  ; textDo = ""     ; break;
      }

      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion hamper
   
   #region Button_Click

   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   #endregion Button_Click

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   #endregion Fld_
}


public class F2_Izlaz_UC : VvUserControl
{
   #region Fieldz
   public VvDataGridView TheG { get; set; }

   private VvTextBox vvtb_tt       , 
                     vvtb_ttNum    , 
                     vvtb_fiskTtNum,
                     vvtb_extBrRn  ,
                     vvtb_date     , 
                     vvtb_partner  , 
                     vvtb_ams      ,
                     vvtb_iznos    ,
                      
                     vvtb_electrID , 
                     vvtb_dateSlanja,
                     vvtb_status,
                     vvtb_stFisk,
                     vvtb_reject,

                     vvtb_uplata,
                     //vvtb_dateUpl,
                     vvtb_markPaid,
                     vvtb_razlikaUpl,
                     vvtb_arhiv,
                     vvtb_FIRtip

                     ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol, colRazmak;

   Color clr_colHeader_Back, clr_colHeader_Fore, clr_rowHeader_Back, clr_rowHeader_Fore, clr_colIfa_Back, clr_eRacun_Back, clr_Mp_Back;
   Image img_red, img_yellow, img_green, img_empty, img_blue, img_darkGreen;

   DataGridViewImageColumn ams, fisk, eIzvj, arh, map, reject, preuzet;
  
   //private VvCheckBoxColumn colfakStop;
   //private CheckBox cbx_selection;
   //private DataGridViewCheckBoxColumn colCbox;

   internal List<Faktur> TheFakturList { get; set; }

   #endregion Fieldz

   #region Constructor

   public F2_Izlaz_UC(Control _parent, VvForm.VvSubModul vvSubModul)
   {
      this.SuspendLayout();

      this.Parent = _parent;
      this.Dock = DockStyle.Fill;

      SetColors();

      CreateTheGrid();
      this.ResumeLayout();

      CreateColumn(TheG);

      //this.ResumeLayout();

      SetColumnIndexes();

      TheVvTabPage.TheVvUC = this; // !!! ??? (treba ti za GetFisk_RecID_Oper) 

#if !DEBUG
      INIT_FIR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
#endif
      //PutDgvFields();

      TheG.TabStop = false;
      TheG.ClearSelection();

      TheG.CellMouseDoubleClick += TheGrid_CellMouseDoubleClick_OpenSomeDUC;

   }

   internal void INIT_FIR()
   {
      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      TheVvTabPage.ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet();

      #region Check Tables 

      string tableName, dbName;

      tableName = Xtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Faktur.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = FaktEx.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrans.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false); 

      tableName = Kupdob.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Nalog.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Ftrans.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      #endregion Check Tables 

      Vv_eRacun_HTTP.InitProjectData();

      int newsCount = 0;

      /* AAA */ newsCount += Vv_eRacun_HTTP.Load_IRn_FakturList(this);

      if(newsCount.IsNegative()) return; // neinicijalizirani projekt - nema jos table-ova 

    //if(ZXC.CURR_prjkt_rec.F2_IsKlijentServisaNaMERu)
      if(ZXC.CURR_prjkt_rec.F2_RolaKind == F2_RolaKind.KlijentServisa_TipA)
      {
      /* XXX */ newsCount += Vv_eRacun_HTTP.WS_Ufati_Veleform_Ritam(this);
      }

      if(ZXC.RRD.Dsc_F2_IsAutoSend)
      {
      /* BBB */ newsCount += Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_SEND_eRacune(this, false);
      }

      /* CCC */ newsCount += Vv_eRacun_HTTP.WS_Refresh_ALL_FIR_Statuses_AndArhiviraj(this); // TRN + DPS + Fisk_Fisk + Fisk_Reject + Fisk_MAP + Arhiva 

      if(ZXC.RRD.Dsc_F2_IsAutoMAP)
      {
      /* DDD */ newsCount +=  Vv_eRacun_HTTP.WS_Discover_Candidates_And_Eventually_MAPaj_uplate(this, false, false);
      }

      ZXC.SetStatusText("");

      if(newsCount.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema novosti.");
      }
   }

#endregion Constructor

   #region TheGrid and columns

   private void CreateTheGrid()
   {
      TheG      = CreateDataGridView_ReadOnly(this, "FIR");
      TheG.Dock = DockStyle.Fill;
      TheG.ColumnHeadersDefaultCellStyle.BackColor = clr_colHeader_Back;
      TheG.ColumnHeadersDefaultCellStyle.ForeColor = clr_colHeader_Fore;
      TheG.RowHeadersDefaultCellStyle.BackColor    = clr_rowHeader_Back;
      TheG.RowHeadersDefaultCellStyle.ForeColor    = clr_rowHeader_Fore;

      TheG.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
      TheG.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
      TheG.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      TheG.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      TheG.ColumnHeadersHeight         = ZXC.Q2un;

      TheG.ReadOnly = true;

      TheG.AllowUserToOrderColumns = false;
   }

   private void CreateColumn(VvDataGridView theGrid)
   {
      //cbx_selection        = new CheckBox();
      //colCbox            = new DataGridViewCheckBoxColumn();
      //colCbox.Tag        = cbx_selection;
      //colCbox.Name       = "cbx";
      //colCbox.HeaderText = ""   ;
      //colCbox.ValueType  = typeof(bool);
      //colCbox.Width      = ZXC.QUN;
      //colCbox.Visible    = false;
      //colCbox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      //theGrid.Columns.Add(colCbox);

    //vvtb_FIRtip    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_FIRtip"   , null, -12, "FIR Tip"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_FIRtip    , null, "R_FIRtip"   , "FIR Tip"       , ZXC.Q3un           ); vvtb_FIRtip   .JAM_ReadOnly = true; colVvText.ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_tt        = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_tt"       , null, -12, "Tip"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tt        , null, "R_tt"       , "Tip"           , ZXC.Q2un           ); vvtb_tt       .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_ttNum     = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_ttNum"    , null, -12, "Interni broj" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_ttNum     , null, "R_ttNum"    , "Interni Broj"  , ZXC.Q3un           ); vvtb_ttNum    .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_fiskTtNum = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_fiskTtNum", null, -12, "Fiskalni broj"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_fiskTtNum , null, "R_fiskTtNum", "Fiskalni IntBr", ZXC.Q4un           ); vvtb_fiskTtNum.JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      // u vvtb_fiskTtNum imamao fiskalni broj koji je zaparvo u vezniDok po novome
    //vvtb_extBrRn   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_origBrRn" , null, -12, "Externi broj" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_extBrRn   , null, "R_extBrRn"  , "Externi Broj"  , ZXC.Q4un           ); vvtb_extBrRn  .JAM_ReadOnly = true; colVvText.ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_date      = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_date"     , null, -12, "Datum"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_date      , null, "R_date"     , "Datum"         , ZXC.Q4un + ZXC.Qun4); vvtb_date     .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      
    //vvtb_ams         = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_ams"      , null, -12, "AMS"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_ams       , null, "R_ams"      , "AMS"           , ZXC.Q2un - ZXC.Qun4); vvtb_ams      .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      //ams            = new DataGridViewImageColumn();
      //ams.Name       = "R_isAMS";
      //ams.HeaderText = "AMS";
      //ams.Width      = ZXC.Q2un;
      //ams.Image      =  VvIco.UnMarkAsSENDed.ToBitmap();
      //ams.DefaultCellStyle.BackColor = clr_colIfa_Back;
      //theGrid.Columns.Add(ams);

      vvtb_partner   = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_partner"  , null, -12, "Partner"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_partner   , null, "R_partner"  , "Partner"       , ZXC.Q9un           ); vvtb_partner  .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q7un;    colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_iznos     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_iznos"    , null, -12, "Iznos"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_iznos     , null, "R_iznos"    , "Iznos"         , ZXC.Q4un           ); vvtb_iznos    .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; // colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
     
      colRazmak        = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);
      
      vvtb_electrID    = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_electrID"   , null, -12, "Electronic ID"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_electrID  , null, "R_electrID"   , "ElectronicID", ZXC.Q4un); vvtb_electrID  .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_dateSlanja  = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_dateSlanja" , null, -12, "Datum Slanja Dokumenta"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dateSlanja, null, "R_dateSlanja" , "Datum Slanja", ZXC.Q6un); vvtb_dateSlanja.JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_status      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_stDokumenta", null, -12, "Status Dokumenta"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_status    , null, "R_status"     , "Status opis" , ZXC.Q6un); vvtb_status    .JAM_ReadOnly = true; /*colVvText.ReadOnly = true;*/ colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;

      preuzet            = new DataGridViewImageColumn();
      preuzet.Name       = "R_isPreuzet";
      preuzet.HeaderText = "Status";
      preuzet.Width      = ZXC.Q2un;
      preuzet.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      preuzet.DefaultCellStyle.BackColor = clr_eRacun_Back;
      theGrid.Columns.Add(preuzet);

      fisk            = new DataGridViewImageColumn();
      fisk.Name       = "R_isFisk";
      fisk.HeaderText = "FISK";
      fisk.Width      = ZXC.Q2un;
      fisk.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      fisk.DefaultCellStyle.BackColor = clr_eRacun_Back;
      theGrid.Columns.Add(fisk);

      arh            = new DataGridViewImageColumn();
      arh.Name       = "R_isArhiv";
      arh.HeaderText = "ARH";
      arh.Width      = ZXC.Q2un;
      arh.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      arh.DefaultCellStyle.BackColor = clr_eRacun_Back;
      theGrid.Columns.Add(arh);

      eIzvj            = new DataGridViewImageColumn();
      eIzvj.Name       = "R_isEizvj";
      eIzvj.HeaderText = "eIzvj";
      eIzvj.Width      = ZXC.Q2un;
      eIzvj.Image      = img_empty/*VvIco.MarkAsRECEIVEd.ToBitmap()*/;
      eIzvj.DefaultCellStyle.BackColor = clr_eRacun_Back;
      theGrid.Columns.Add(eIzvj);

      reject            = new DataGridViewImageColumn();
      reject.Name       = "R_isReject";
      reject.HeaderText = "ODB.";
      reject.Width      = ZXC.Q2un;
      reject.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      reject.DefaultCellStyle.BackColor = clr_eRacun_Back;
      theGrid.Columns.Add(reject);

      colRazmak        = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);

      vvtb_uplata     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (   2, "vvtb_uplata"    , null, -12, "Uplaćeno"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_uplata    , null, "R_uplata"    , "Naplaćeno"  , ZXC.Q4un); /*vvtb_uplata    .JAM_ReadOnly = true;*/  colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_markPaid   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (   2, "vvtb_markPaid"  , null, -12, "Prijavljeno"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_markPaid  , null, "R_markPaid"  , "Prijavljeno", ZXC.Q4un); /*vvtb_markPaid  .JAM_ReadOnly = true;*/  colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_razlikaUpl = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (   2, "vvtb_razlikaUpl", null, -12, "Razlika"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_razlikaUpl, null, "R_razlikaUpl", "Razlika"    , ZXC.Q4un); /*vvtb_razlikaUpl.JAM_ReadOnly = true;*/  colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  //colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;

      //colVvText.DefaultCellStyle.Format = VvUserControl.GetDgvCellStyleFormat_Number(vvtb_uplata.JAM_NumberOfDecimalPlaces, true, false); // da ne prikaze 0.00
      vvtb_uplata.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      map = new DataGridViewImageColumn();
      map.Name       = "R_isMAP";
      map.HeaderText = "OKP";
      map.Width      = ZXC.Q2un;
      map.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      map.DefaultCellStyle.BackColor = clr_Mp_Back;
      theGrid.Columns.Add(map);

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

      colScrol.DefaultCellStyle.BackColor = TheG.ColumnHeadersDefaultCellStyle.BackColor;


      foreach(DataGridViewColumn column in TheG.Columns)
      {
         column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }

   }

   private void SetColors()
   {
      clr_colHeader_Back = Color.LightBlue;// Color.FromArgb(123, 170, 238);
      clr_colHeader_Fore = Color.DarkSlateGray;
      clr_rowHeader_Back = Color.LightBlue; //Color.FromArgb(123, 170, 238); //Color.PowderBlue;
      clr_rowHeader_Fore = Color.DarkSlateGray;

      clr_colIfa_Back = Color.LightCyan;
      clr_eRacun_Back = Color.Honeydew;
      clr_Mp_Back     = Color.Linen;//LavenderBlush
      //clr_arhiva_Back = Color.LightYellow;

      img_empty     = VvIco.EmptyIcon      .ToBitmap();
      img_red       = VvIco.MarkAsSENDed   .ToBitmap();
      img_yellow    = VvIco.CheckPrNabCij  .ToBitmap();
      img_green     = VvIco.MarkAsRECEIVEd .ToBitmap();
      img_blue      = VvIco.LogLANADDaction.ToBitmap();
      img_darkGreen = VvIco.UnMarkAsRECEIVEd.ToBitmap();

   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private Izlaz_colIdx ci;
   public Izlaz_colIdx DgvCI { get { return ci; } }
   public struct Izlaz_colIdx
   {
      internal int iT_tt        ;
      internal int iT_ttNum     ;
      internal int iT_fiskTtNum ;
      internal int iT_date      ;
      internal int iT_partner   ;
      internal int iT_iznos     ;
      internal int iT_electrID  ;
      internal int iT_dateSlanja;
      internal int iT_status    ;
      internal int iT_isFisk    ;
      internal int iT_isEizvj   ;
      internal int iT_isReject  ;
      internal int iT_uplata    ;
      internal int iT_isMAP     ;
      internal int iT_markPaid  ;
      internal int iT_razlikaUpl;
      internal int iT_isArhiv   ;
      internal int iT_isPreuzet    ;
   }

   private void SetColumnIndexes()
   {
      ci = new Izlaz_colIdx();

      ci.iT_tt         = TheG.IdxForColumn("R_tt"         );
      ci.iT_ttNum      = TheG.IdxForColumn("R_ttNum"      );
      ci.iT_fiskTtNum  = TheG.IdxForColumn("R_fiskTtNum"  );
      ci.iT_date       = TheG.IdxForColumn("R_date"       );
      ci.iT_partner    = TheG.IdxForColumn("R_partner"    );
      ci.iT_iznos      = TheG.IdxForColumn("R_iznos"      );
      ci.iT_electrID   = TheG.IdxForColumn("R_electrID"   );
      ci.iT_dateSlanja = TheG.IdxForColumn("R_dateSlanja" );
      ci.iT_status     = TheG.IdxForColumn("R_status"     );
      ci.iT_isFisk     = TheG.IdxForColumn("R_isFisk"     );
      ci.iT_isEizvj    = TheG.IdxForColumn("R_isEizvj"    );
      ci.iT_isReject   = TheG.IdxForColumn("R_isReject"   );
      ci.iT_uplata     = TheG.IdxForColumn("R_uplata"     );
      ci.iT_markPaid   = TheG.IdxForColumn("R_markPaid"   );
      ci.iT_razlikaUpl = TheG.IdxForColumn("R_razlikaUpl" );
      ci.iT_isMAP      = TheG.IdxForColumn("R_isMAP"      );
      ci.iT_isArhiv    = TheG.IdxForColumn("R_isArhiv"    );
      ci.iT_isPreuzet    = TheG.IdxForColumn("R_isPreuzet"  );
   }

   #endregion SetColumnIndexes)

   #region PutDgvFields

   public void PutDgvFields()
   {
      int rowIdx;

      TheG.Rows.Clear();

      if(TheFakturList.IsEmpty()) return;

      for(rowIdx = 0; rowIdx < TheFakturList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
      {
         TheG.Rows.Add();

         PutDgvLineFields(rowIdx, TheFakturList[rowIdx]);

         TheG.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

   }

   /*private*/internal void PutDgvLineFields(int rowIdx, Faktur faktur_rec)
   {
      bool isF1 = faktur_rec.IsF1;

      TheG.PutCell(ci.iT_tt         , rowIdx, faktur_rec.TT                );
      TheG.PutCell(ci.iT_ttNum      , rowIdx, faktur_rec.TtNum             );
      TheG.PutCell(ci.iT_fiskTtNum  , rowIdx, faktur_rec.VezniDok          );
      TheG.PutCell(ci.iT_date       , rowIdx, faktur_rec.DokDate           );
      TheG.PutCell(ci.iT_partner    , rowIdx, faktur_rec.KupdobName        );
      TheG.PutCell(ci.iT_iznos      , rowIdx, faktur_rec.S_ukKCRP          );

      TheG.PutCell(ci.iT_electrID   , rowIdx, faktur_rec.F2_ElectronicID);
      TheG.PutCell(ci.iT_dateSlanja , rowIdx, faktur_rec.F2_SentTS.IsEmpty() ? "" : faktur_rec.F2_SentTS.ToString(ZXC.VvDateAndTimeFormat_NoSec));

      string trnStatus = Vv_eRacun_HTTP.Get_MER_TransportStatus_Safe(faktur_rec.F2_StatusCD);

      TheG.PutCell(ci.iT_status     , rowIdx, faktur_rec.F2_ElectronicID.IsZero() ? "" : trnStatus); // TODO !!! provider dependent 

      TheG.PutCell(ci.iT_uplata     , rowIdx, "");
      TheG.PutCell(ci.iT_markPaid   , rowIdx, "");
      TheG.PutCell(ci.iT_razlikaUpl , rowIdx, "");


      if(faktur_rec.F2_IsARHIVED) ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isArhiv]).Value = img_green;
      else                        ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isArhiv]).Value = img_empty;

           if(faktur_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.DA_JE     )      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_green;
      else if(faktur_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.Na_cekanju)      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_yellow;
      else if(faktur_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.NE_NIJE   )      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_red;
      else                                                                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_empty;

           if(faktur_rec.F2_IsRejected == F2_StatusInAndOutBoxEnum.DA_JE     )  ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_green;
      else if(faktur_rec.F2_IsRejected == F2_StatusInAndOutBoxEnum.Na_cekanju)  ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_yellow;
      else if(faktur_rec.F2_IsRejected == F2_StatusInAndOutBoxEnum.NE_NIJE   )  ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_red;
      else                                                                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_empty;

           if(faktur_rec.F2_IsMarkAsPaid == F2_StatusInAndOutBoxEnum.DA_JE     )((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isMAP]).Value = img_green;
      else if(faktur_rec.F2_IsMarkAsPaid == F2_StatusInAndOutBoxEnum.Na_cekanju)((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isMAP]).Value = img_yellow;
      else if(faktur_rec.F2_IsMarkAsPaid == F2_StatusInAndOutBoxEnum.NE_NIJE   )((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isMAP]).Value = img_red;
      else                                                                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isMAP]).Value = img_empty;
    
           if(faktur_rec.F2_IsEizvj == F2_StatusInAndOutBoxEnum.DA_JE     )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isEizvj]).Value = img_green;
      else if(faktur_rec.F2_IsEizvj == F2_StatusInAndOutBoxEnum.Na_cekanju)     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isEizvj]).Value = img_yellow;
      else if(faktur_rec.F2_IsEizvj == F2_StatusInAndOutBoxEnum.NE_NIJE   )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isEizvj]).Value = img_red;
      else                                                                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isEizvj]).Value = img_empty;

           if(faktur_rec.F2_StatusCD == 40                                )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_green;
      else if(faktur_rec.F2_StatusCD == 30                                )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_darkGreen;
      else if(faktur_rec.F2_StatusCD == 50                                )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_red;
      else if(faktur_rec.F2_StatusCD == 20                                )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_yellow;
      else if(faktur_rec.F2_StatusCD == 70                                )     ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_red;
      else                                                                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isPreuzet]).Value = img_empty;


      if(faktur_rec.F2_ElectronicID.IsZero()) TheG.Rows[rowIdx].Cells[ci.iT_electrID].Value = "";
   }

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   #endregion PutDgvFields1

   private void TheGrid_CellMouseDoubleClick_OpenSomeDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;
    
      if(e.ColumnIndex == ci.iT_tt || e.ColumnIndex == ci.iT_ttNum || e.ColumnIndex == ci.iT_fiskTtNum)
      {
         string tipBr = theG.GetStringCell(ci.iT_tt, rowIdx, false) + "-" + theG.GetStringCell(ci.iT_ttNum, rowIdx, false);

         ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);
      }

      if(e.ColumnIndex == ci.iT_uplata )
      {
         string tt    = theG.GetStringCell(ci.iT_tt   , rowIdx, false);
         uint   ttNum = theG.GetUint32Cell(ci.iT_ttNum, rowIdx, false);

         Nalog nalog_rec = FtransDao.GetNalog_ForIFA_TtAndTtNum(TheDbConnection, tt, ttNum);

         ZXC.TheVvForm.ShowNalogDUC_For_DokNum(nalog_rec.DokNum.ToString());
      }

   }

   public /*override*/ void Refresh_FIR(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(newPage is VvTabPage == false) return;

#if !DEBUG
      if((newPage as VvTabPage).TheVvUC is F2_Izlaz_UC) ((newPage as VvTabPage).TheVvUC as F2_Izlaz_UC).INIT_FIR();
#endif
   }
}

public class F2_Ulaz_UC : VvUserControl
{
   #region Fieldz
   public VvDataGridView TheG { get; set; }

   private VvTextBox vvtb_senderName , vvtb_senderOIB, vvtb_statusID,
                     vvtb_kupDob  ,
                     vvtb_documentNr,
                     vvtb_vezDok  , 
                     vvtb_elID    ,
                     vvtb_iznos, 
                     vvtb_iznosRn,
                     vvtb_fisk    ,
                     vvtb_odbijen ,
                     vvtb_tt      ,
                     vvtb_ttNum   ,
                     vvtb_sentDate ,
                     vvtb_dokDate,
      vvtb_dateRn,
                     vvtb_napomena
                     ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol, colRazmak;
   DataGridViewImageColumn fisk, arh, reject;

   Color clr_colHeader_Back, clr_colHeader_Fore, clr_rowHeader_Back, clr_rowHeader_Fore, clr_colIfa_Back, clr_eRacun_Back, clr_Mp_Back;

   internal List<Xtrano> TheXtranoList { get; set; }
   
   Image img_red, img_yellow, img_green, img_empty;

   #endregion Fieldz

   #region Constructor

   public F2_Ulaz_UC(Control _parent, VvForm.VvSubModul vvSubModul)
   {
      this.SuspendLayout();

      this.Parent = _parent;
      this.Dock = DockStyle.Fill;

      SetColors();

      CreateTheGrid();
      this.ResumeLayout();

      CreateColumn(TheG);

      //this.ResumeLayout();

      SetColumnIndexes();

      TheVvTabPage.TheVvUC = this; // !!! ??? (treba ti za GetFisk_RecID_Oper) 

#if !DEBUG
      INIT_FUR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
#endif

      //PutDgvFields();

      TheG.TabStop = false;
      TheG.ClearSelection();

      TheG.CellMouseDoubleClick += TheGrid_CellMouseDoubleClick_OpenSomeDUC;

   }

   internal void INIT_FUR()
   {
      if(Vv_eRacun_HTTP.Is_FUR_ON() == false) return;

      TheVvTabPage.ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet();

      #region Check Tables 

      string tableName, dbName;

      tableName = Xtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Faktur.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = FaktEx.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrans.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Kupdob.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      #endregion Check Tables 

      Vv_eRacun_HTTP.InitProjectData();

      int newsCount = 0;

      /* 111 */              Vv_eRacun_HTTP.Load_AUR_XtranoList(this);

      /* YYY */ newsCount += Vv_eRacun_HTTP.WS_QueryInbox_Receive_StatusInbox(this);

      ZXC.SetStatusText("");

      if(newsCount.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Nema novosti.");
      }
   }

#endregion Constructor

   #region TheGrid and columns

   private void CreateTheGrid()
   {
      TheG = CreateDataGridView_ReadOnly(this, "FUR");
      TheG.Dock = DockStyle.Fill;
      TheG.ColumnHeadersDefaultCellStyle.BackColor = Color.MediumAquamarine;
      TheG.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
      TheG.RowHeadersDefaultCellStyle.BackColor    = Color.MediumAquamarine;
      TheG.RowHeadersDefaultCellStyle.ForeColor    = Color.Black;

      TheG.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
      TheG.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
      TheG.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      TheG.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      TheG.ColumnHeadersHeight         = ZXC.Q2un;
   }

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_elID       = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_elID"      , null, -12, "elID"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_elID      , null, "R_elID"       , "ElectronicID"  , ZXC.Q4un           ); vvtb_elID      .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back;//  colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_sentDate   = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_sentDate"  , null, -12, "Datum slanja"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_sentDate  , null, "R_sentDate"   , "Datum slanja"  , ZXC.Q3un + ZXC.Qun2); vvtb_sentDate  .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back;//  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_dateRn     = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_dateRn"    , null, -12, "Datum Rn"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dateRn    , null, "R_datumRn"    , "Datum Računa"  , ZXC.Q4un           ); vvtb_dateRn    .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_senderName = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_senderName", null, -12, "Dobavljač"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_senderName, null, "R_partner"    , "Dobavljač"     , ZXC.Q10un          ); vvtb_senderName.JAM_ReadOnly = true; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q7un ; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back;  //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_senderOIB  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_senderOIB" , null, -12, "Dobavljač"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_senderOIB , null, "R_partnerOIB" , "OIB dobavljača", ZXC.Q4un           ); vvtb_senderOIB .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back;  //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_documentNr = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_documentNr", null, -12, "Broj Računa" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_documentNr, null, "R_origBrRn"   , "Broj Računa"   , ZXC.Q8un           ); vvtb_documentNr.JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_iznosRn    = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_iznosRn"   , null, -12, "IznosRn"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_iznosRn   , null, "R_iznosRn"    , "IznosRn"       , ZXC.Q4un           ); vvtb_iznosRn   .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_statusID   = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_statusID"  , null, -12, "Status"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_statusID  , null, "R_statusID"   , "Status"        , ZXC.Q3un           ); vvtb_statusID  .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_eRacun_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
 
      colRazmak = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);

      fisk            = new DataGridViewImageColumn();
      fisk.Name       = "fis";
      fisk.HeaderText = "FISK";
      fisk.Width      = ZXC.Q2un;
      fisk.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      fisk.DefaultCellStyle.BackColor = clr_Mp_Back;
      theGrid.Columns.Add(fisk);

      arh            = new DataGridViewImageColumn();
      arh.Name       = "arh";
      arh.HeaderText = "ARH";
      arh.Width      = ZXC.Q2un;
      arh.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      arh.DefaultCellStyle.BackColor = clr_Mp_Back;
      theGrid.Columns.Add(arh);
   
      reject            = new DataGridViewImageColumn();
      reject.Name       = "reject";
      reject.HeaderText = "ODB.";
      reject.Width      = ZXC.Q2un;
      reject.Image      = VvIco.MarkAsRECEIVEd.ToBitmap();
      reject.DefaultCellStyle.BackColor = clr_Mp_Back;
      theGrid.Columns.Add(reject);

      colRazmak        = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);
   
      vvtb_tt      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_tt"     , null, -12, "Tip"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tt     , null, "R_tt"     , "Tip"               , ZXC.Q2un); vvtb_tt    .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_ttNum   = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_ttNum"  , null, -12, "Broj"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_ttNum  , null, "R_ttNum"  , "Broj Računa"       , ZXC.Q4un); vvtb_ttNum .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_dokDate = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_dokDate", null, -12, "DokDate" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dokDate, null, "R_dokDate", "Datum Računa"      , ZXC.Q4un); vvtb_dokDate.JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_kupDob  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_kupDob" , null, -12, "KupDob"  ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_kupDob , null, "R_kupDob" , "Partner"           , ZXC.Q7un); vvtb_kupDob.JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_vezDok  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_vezDok" , null, -12, "OrigBrRn"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_vezDok , null, "R_vezDok" , "OrigBroj Dokumenta", ZXC.Q8un); vvtb_vezDok.JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_iznos   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_iznos"  , null, -12, "Iznos"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_iznos  , null, "R_iznos"  , "Iznos"             , ZXC.Q4un); vvtb_iznos .JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;

      colRazmak = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);

      vvtb_napomena = theGrid.CreateVvTextBoxFor_String_ColumnTemplate("vvtb_napomena", null, -12, "Napomena"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_napomena, null, "R_napomena", "Napomena"              , ZXC.Q8un); vvtb_napomena.JAM_ReadOnly = true; colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; //colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      colScrol.DefaultCellStyle.BackColor = TheG.ColumnHeadersDefaultCellStyle.BackColor;

      foreach(DataGridViewColumn column in TheG.Columns)
      {
         column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }

   }

   private void SetColors()
   {
      clr_colHeader_Back = Color.LightBlue    ;// Color.FromArgb(123, 170, 238);
      clr_colHeader_Fore = Color.DarkSlateGray;
      clr_rowHeader_Back = Color.LightBlue    ; //Color.FromArgb(123, 170, 238); //Color.PowderBlue;
      clr_rowHeader_Fore = Color.DarkSlateGray;

      clr_colIfa_Back = Color.LightCyan;
      clr_eRacun_Back = Color.Honeydew;
      clr_Mp_Back     = Color.LightYellow;

      img_empty  = VvIco.EmptyIcon     .ToBitmap();
      img_red    = VvIco.MarkAsSENDed  .ToBitmap();
      img_yellow = VvIco.CheckPrNabCij .ToBitmap();
      img_green  = VvIco.MarkAsRECEIVEd.ToBitmap();
   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private Ulaz_colIdx ci;
   public Ulaz_colIdx DgvCI { get { return ci; } }
   public struct Ulaz_colIdx
   {
      internal int iT_elID     ; 
      internal int iT_sentDate ; 
      internal int iT_sender   ; 
      internal int iT_senderOIB;
      internal int iT_documNr  ; 
      internal int iT_statusID ; 
      internal int iT_iznosRn  ; 
      internal int iT_datumRn  ;

      internal int iT_isFisk   ;
      internal int iT_isReject ;
      internal int iT_isArhiv  ;

      internal int iT_tt       ; 
      internal int iT_ttNum    ; 
      internal int iT_kupDob   ;
      internal int iT_dokDate  ; 
      internal int iT_vezDok   ;
      internal int iT_iznos    ;
      internal int iT_napomena ; 

   }

   private void SetColumnIndexes()
   {
      ci = new Ulaz_colIdx();

      ci.iT_elID      = TheG.IdxForColumn("R_elID"      );
      ci.iT_sentDate  = TheG.IdxForColumn("R_sentDate"  );
      ci.iT_sender    = TheG.IdxForColumn("R_partner"   );
      ci.iT_senderOIB = TheG.IdxForColumn("R_partnerOIB");
      ci.iT_documNr   = TheG.IdxForColumn("R_origBrRn"  );
      ci.iT_statusID  = TheG.IdxForColumn("R_statusID"  );
      ci.iT_iznosRn   = TheG.IdxForColumn("R_iznosRn"   );
      ci.iT_datumRn   = TheG.IdxForColumn("R_datumRn"   );

      ci.iT_isFisk    = TheG.IdxForColumn("fis");
      ci.iT_isArhiv   = TheG.IdxForColumn("arh");
      ci.iT_isReject  = TheG.IdxForColumn("reject");

      ci.iT_tt        = TheG.IdxForColumn("R_tt"      );
      ci.iT_ttNum     = TheG.IdxForColumn("R_ttNum"   );
      ci.iT_dokDate   = TheG.IdxForColumn("R_dokDate" );
      ci.iT_kupDob    = TheG.IdxForColumn("R_kupDob"  );
      ci.iT_vezDok    = TheG.IdxForColumn("R_vezDok"  );
      ci.iT_iznos     = TheG.IdxForColumn("R_iznos"   );
      
      ci.iT_napomena  = TheG.IdxForColumn("R_napomena");
   }

   #endregion SetColumnIndexes()

   #region PutDgvFields

   public void PutDgvFields()
   {
      int rowIdx;

      TheG.Rows.Clear();

      if(TheXtranoList.IsEmpty()) return;

      for(rowIdx = 0; rowIdx < TheXtranoList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
      {
         TheG.Rows.Add();

         PutDgvLineFields(rowIdx, TheXtranoList[rowIdx]);

         TheG.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }
   }

   internal void PutDgvLineFields(int rowIdx, Xtrano xtrano_rec)
   {
      TheG.PutCell(ci.iT_elID     , rowIdx, xtrano_rec.F2_ElectronicID);
      TheG.PutCell(ci.iT_sentDate , rowIdx, xtrano_rec.T_dokDate      );
      TheG.PutCell(ci.iT_sender   , rowIdx, xtrano_rec.T_opis_128     );
      TheG.PutCell(ci.iT_documNr  , rowIdx, xtrano_rec.T_theString    );
      TheG.PutCell(ci.iT_senderOIB, rowIdx, xtrano_rec.T_konto        );
      TheG.PutCell(ci.iT_statusID , rowIdx, xtrano_rec.T_devName      );
      TheG.PutCell(ci.iT_iznosRn  , rowIdx, xtrano_rec.T_moneyA       );
      TheG.PutCell(ci.iT_datumRn  , rowIdx, xtrano_rec.T_dokDate2     );

           if(xtrano_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.DA_JE     ) ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_green ;
      else if(xtrano_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.Na_cekanju) ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_yellow;
      else if(xtrano_rec.F2_IsFisk == F2_StatusInAndOutBoxEnum.NE_NIJE   ) ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_red   ;
      else                                                                 ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isFisk]).Value = img_empty ;

      if(xtrano_rec.T_XmlZip != null) ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isArhiv]).Value = img_green;
      else                            ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isArhiv]).Value = img_red;
 
      if(xtrano_rec.F2_IsReject)((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_green;
      else                      ((DataGridViewImageCell)TheG.Rows[rowIdx].Cells[ci.iT_isReject]).Value = img_empty;

      #region Check For Duplicate Inbox entries

      bool duplicateInbox_FOUND = TheXtranoList.Count(xtr => xtr.T_konto == xtrano_rec.T_konto && xtr.T_theString == xtrano_rec.T_theString) > 1;

      if(duplicateInbox_FOUND)
      {
         TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.LightPink;
      }

      #endregion Check For Duplicate Inbox entries

      #region From Faktur DataLayer

      bool fakturDataLayer_FOUNDv1 = false;
      bool fakturDataLayer_FOUNDv2 = false;

      uint   fakturRecID    = xtrano_rec.T_parentID ;
      string fakturKdOIB    = xtrano_rec.T_konto    ;
      string fakturVezniDok = xtrano_rec.T_theString;

      Faktur faktur_rec = new Faktur();

      if(fakturRecID.NotZero() && fakturRecID != UInt32.MaxValue)
      {
         fakturDataLayer_FOUNDv1 = faktur_rec.VvDao.SetMe_Record_byRecID(TheDbConnection, faktur_rec, fakturRecID, false);

         if(!fakturDataLayer_FOUNDv1)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, $"U Inbox-u je za ulaz. rn. {Environment.NewLine}{Environment.NewLine}{xtrano_rec.T_opis_128 + " rn: " + xtrano_rec.T_theString}{Environment.NewLine}{Environment.NewLine}IZBRISAN prethodno AUTOMATSKI importiran račun.");

            #region RWTREC Xtrano ... remove old T_parentID

            TheVvTabPage.TheVvForm.BeginEdit(xtrano_rec);

            xtrano_rec.T_parentID = 0;
            xtrano_rec.T_ttNum    = 0;

            xtrano_rec.VvDao.RWTREC(TheDbConnection, xtrano_rec, false, false, false);

            TheVvTabPage.TheVvForm.EndEdit(xtrano_rec);

            #endregion RWTREC Xtrano ... remove old T_parentID

         }
      }

      if(!fakturDataLayer_FOUNDv1)
      {
         fakturDataLayer_FOUNDv2 = FakturDao.SetMeFaktur_ByKupdobOIBAndVezniDok(TheDbConnection, faktur_rec, fakturKdOIB, fakturVezniDok);
      }

      if(fakturDataLayer_FOUNDv2)
      {
         int idxOfTHisXtrano = TheXtranoList.IndexOf(xtrano_rec);

         TheXtranoList[idxOfTHisXtrano].T_parentID = UInt32.MaxValue; // signaliziraj u TheXtranoList da je faktur nađen ByKupdobOIBAndVezniDok, nije import ali ga ipak ima  
      }

      if(fakturDataLayer_FOUNDv1 || fakturDataLayer_FOUNDv2)
      { 
         faktur_rec.VvDao.LoadExtender(TheDbConnection, faktur_rec, false);

         TheG.PutCell(ci.iT_tt      , rowIdx, faktur_rec.TT        );
         TheG.PutCell(ci.iT_ttNum   , rowIdx, faktur_rec.TtNum     );
         TheG.PutCell(ci.iT_dokDate , rowIdx, faktur_rec.DokDate   );
         TheG.PutCell(ci.iT_kupDob  , rowIdx, faktur_rec.KupdobName);
         TheG.PutCell(ci.iT_vezDok  , rowIdx, faktur_rec.VezniDok  );
         TheG.PutCell(ci.iT_iznos   , rowIdx, faktur_rec.S_ukKCRP  );
         TheG.PutCell(ci.iT_napomena, rowIdx, faktur_rec.Napomena  );

         DataGridViewCell cell;

         for(int i = 0; i < TheG.Rows[rowIdx].Cells.Count; ++i)
         {
            if(i == ci.iT_tt || i == ci.iT_ttNum || i == ci.iT_dokDate || i == ci.iT_kupDob || i == ci.iT_vezDok || i == ci.iT_iznos || i == ci.iT_napomena)
            {
               cell = TheG.Rows[rowIdx].Cells[i];

               if(faktur_rec.S_ukKCRP != xtrano_rec.T_moneyA) cell.Style.ForeColor = Color.Red;

               else if(faktur_rec.KupdobName.ToUpper() != xtrano_rec.T_opis_128.ToUpper())
               {
                  // Ipak ne do daljnjega                                                  cell.Style.ForeColor = Color.DarkRed; 
                  //if(faktur_rec.KupdobName.ToUpper().StartsWith("ELECT"))
                  //ZXC.aim_emsg($"[{faktur_rec.KupdobName.ToUpper()}] [{xtrano_rec.T_opis_128.ToUpper()}]");
               }

               else if(fakturDataLayer_FOUNDv2) cell.Style.ForeColor = Color.FromArgb(82, 122, 122); //Color.DarkGray;
            }
         }
      }

      else
      {
         TheG.PutCell(ci.iT_tt      , rowIdx, "");
         TheG.PutCell(ci.iT_ttNum   , rowIdx, "");
         TheG.PutCell(ci.iT_dokDate , rowIdx, "");
         TheG.PutCell(ci.iT_kupDob  , rowIdx, "");
         TheG.PutCell(ci.iT_vezDok  , rowIdx, "");
         TheG.PutCell(ci.iT_iznos   , rowIdx, "");
         TheG.PutCell(ci.iT_napomena, rowIdx, "");
      }

      #endregion From Faktur DataLayer

   }

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   #endregion PutDgvFields1

   public /*override*/ void Refresh_FUR(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
   {
      if(newPage is VvTabPage == false) return;

#if !DEBUG
      if((newPage as VvTabPage).TheVvUC is F2_Ulaz_UC) ((newPage as VvTabPage).TheVvUC as F2_Ulaz_UC).INIT_FUR();
#endif
   }

   private void TheGrid_CellMouseDoubleClick_OpenSomeDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;
    
      if(e.ColumnIndex == ci.iT_ttNum || e.ColumnIndex == ci.iT_tt)
      {
         string tipBr = theG.GetStringCell(ci.iT_tt, rowIdx, false) + "-" + theG.GetStringCell(ci.iT_ttNum, rowIdx, false);

         ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);
      }
   }
}

public class F2_NIR_UC : VvUserControl
{
   #region Fieldz
   public VvDataGridView TheG { get; set; }

   private VvTextBox vvtb_tt       , 
                     vvtb_ttNum    , 
                     vvtb_fiskTtNum,
                     vvtb_extBrRn  ,
                     vvtb_date     ,
                     vvtb_dateUpl,
                     vvtb_partner  , 
                     vvtb_iznos    ,
                     vvtb_nalog, vvtb_konto, vvtb_opis, vvtb_tipUpl,
                     vvtb_uplata,
                     vvtb_markPaid,
                     vvtb_razlikaUpl
                     ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol, colRazmak;

   Color clr_colHeader_Back, clr_colHeader_Fore, clr_rowHeader_Back, clr_rowHeader_Fore, clr_colIfa_Back, clr_eRacun_Back, clr_Mp_Back;

   internal List<Ftrans> TheFtransList { get; set; }

   #endregion Fieldz

   #region Constructor

   public F2_NIR_UC(Control _parent, VvForm.VvSubModul vvSubModul)
   {
      this.SuspendLayout();

      this.Parent = _parent;
      this.Dock = DockStyle.Fill;

      SetColors();

      CreateTheGrid();
      this.ResumeLayout();

      CreateColumn(TheG);

      SetColumnIndexes();

#if !DEBUG
      INIT_NIR(); // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX 
#endif

      TheG.TabStop = false;
      TheG.ClearSelection();

      TheG.CellMouseDoubleClick += TheGrid_CellMouseDoubleClick_OpenSomeDUC;

   }
   internal void INIT_NIR()
   {
      if(Vv_eRacun_HTTP.Is_FIR_ON() == false) return;

      TheVvTabPage.ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet();

      #region Check Tables 

      string tableName, dbName;

      tableName = Xtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Faktur.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = FaktEx.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrans.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Rtrano.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false); 

      tableName = Kupdob.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Nalog.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      tableName = Ftrans.recordName; dbName = VvSQL.GetDbNameForThisTableName(tableName);
      if(!VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, dbName, tableName)) VvSQL.CREATE_TABLE(TheDbConnection, dbName, tableName, false);

      #endregion Check Tables 

      Vv_eRacun_HTTP.InitProjectData();

      int newsCount = 0;

      /* AAA */ newsCount += Vv_eRacun_HTTP.Load_MAP_FtransList(this);

      if(newsCount.IsNegative()) return; // neinicijalizirani projekt - nema jos table-ova 

      ZXC.SetStatusText("");

      //if(newsCount.IsZero())
      //{
      //   ZXC.aim_emsg(MessageBoxIcon.Information, "Nema novosti.");
      //}
   }

   private void TheGrid_CellMouseDoubleClick_OpenSomeDUC(object sender, DataGridViewCellMouseEventArgs e)
   {
      VvDataGridView theG = sender as VvDataGridView;

      int rowIdx = e.RowIndex;

      if(rowIdx.IsNegative()) return;

      if(e.ColumnIndex == ci.iT_tt || e.ColumnIndex == ci.iT_fiskTtNum)
      {
         string tipBr = theG.GetStringCell(ci.iT_tt, rowIdx, false);

         ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);
      }

      if(e.ColumnIndex == ci.iT_uplata || e.ColumnIndex == ci.iT_nalog)
      {
         ZXC.TheVvForm.ShowNalogDUC_For_DokNum(TheFtransList[rowIdx].T_dokNum.ToString());
      }

   }

   #endregion Constructor

   #region TheGrid and columns

   private void CreateTheGrid()
   {
      TheG      = CreateDataGridView_ReadOnly(this, "NIR");
      TheG.Dock = DockStyle.Fill;
      TheG.ColumnHeadersDefaultCellStyle.BackColor = clr_colHeader_Back;
      TheG.ColumnHeadersDefaultCellStyle.ForeColor = clr_colHeader_Fore;
      TheG.RowHeadersDefaultCellStyle.BackColor    = clr_rowHeader_Back;
      TheG.RowHeadersDefaultCellStyle.ForeColor    = clr_rowHeader_Fore;

      TheG.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
      TheG.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
      TheG.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      TheG.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      TheG.ColumnHeadersHeight         = ZXC.Q2un;

      TheG.ReadOnly = true;

      TheG.AllowUserToOrderColumns = false;
   }

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_fiskTtNum  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_fiskTtNum"    , null, -12, "BrojRn"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_fiskTtNum , null, "R_fiskTtNum" , "BrojRn"        , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_date       = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(    "vvtb_date"         , null, -12, "DatumRn"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_date      , null, "R_date"      , "DatumRn"       , ZXC.Q4un + ZXC.Qun4); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_partner    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_partner"      , null, -12, "Partner"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_partner   , null, "R_partner"   , "Partner"       , ZXC.Q9un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_iznos      = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate ( 2, "vvtb_iznos"        , null, -12, "IznosRn"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_iznos     , null, "R_iznos"     , "IznosRn"       , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
 
      colRazmak        = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);
                                                                                                                                                                                                             
      vvtb_tt         = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_tt"           , null, -12, "TipBr"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tt        , null, "R_tt"        , "TipBr"         , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_dateUpl    = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(    "vvtb_dateUpl"      , null, -12, "DatumUpl"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_dateUpl   , null, "R_dateUpl"   , "DatumUpl"      , ZXC.Q4un + ZXC.Qun4); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_nalog      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_nalog "       , null, -12, "Nalog/Red"  ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_nalog     , null, "R_nalog"     , "Nalog/Red"     , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_konto      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_konto "       , null, -12, "Konto"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_konto     , null, "R_konto"     , "Konto"         , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 
      vvtb_opis       = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_opis  "       , null, -12, "Opis uplate"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_opis      , null, "R_opis"      , "Opis uplate"   , ZXC.Q10un          ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q7un;
      vvtb_tipUpl     = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (    "vvtb_tipUpl"       , null, -12, "TipUpl"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tipUpl    , null, "R_tipUpl"    , "TipUpl"        , ZXC.Q2un           ); colVvText.DefaultCellStyle.BackColor = clr_colIfa_Back; 

      colRazmak        = theGrid.CreateScrollColumn("razmak", ZXC.Qun4);

      vvtb_uplata     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate ( 2, "vvtb_uplata"       , null, -12, "Uplaćeno"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_uplata    , null, "R_uplata"    , "Naplaćeno"     , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  
      vvtb_markPaid   = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate ( 2, "vvtb_markPaid"     , null, -12, "Prijavljeno"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_markPaid  , null, "R_markPaid"  , "Prijavljeno"   , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  
      vvtb_razlikaUpl = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate ( 2, "vvtb_razlikaUpl"   , null, -12, "Razlika"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_razlikaUpl, null, "R_razlikaUpl", "Razlika"       , ZXC.Q4un           ); colVvText.DefaultCellStyle.BackColor = clr_Mp_Back;  

      vvtb_uplata.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

      colScrol.DefaultCellStyle.BackColor = TheG.ColumnHeadersDefaultCellStyle.BackColor;

      foreach(DataGridViewColumn column in TheG.Columns)
      {
         column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }

   }

   private void SetColors()
   {
      clr_colHeader_Back = Color.LightBlue;// Color.FromArgb(123, 170, 238);
      clr_colHeader_Fore = Color.DarkSlateGray;
      clr_rowHeader_Back = Color.LightBlue; //Color.FromArgb(123, 170, 238); //Color.PowderBlue;
      clr_rowHeader_Fore = Color.DarkSlateGray;

      clr_colIfa_Back = Color.LightCyan;
      clr_eRacun_Back = Color.Honeydew;
      clr_Mp_Back     = Color.Linen;//LavenderBlush
   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private Izlaz_colIdx ci;
   public Izlaz_colIdx DgvCI { get { return ci; } }
   public struct Izlaz_colIdx
   {
      internal int iT_tt        ;
    //internal int iT_ttNum     ;
      internal int iT_fiskTtNum ;
      internal int iT_date      ;
      internal int iT_dateUpl;
      internal int iT_partner   ;
      internal int iT_iznos     ;
      internal int iT_uplata    ;
      internal int iT_markPaid  ;
      internal int iT_razlikaUpl;
      internal int iT_nalog     ;
      internal int iT_konto     ;
      internal int iT_opis      ;
      internal int iT_tipUpl    ;

   }

   private void SetColumnIndexes()
   {
      ci = new Izlaz_colIdx();

      ci.iT_tt         = TheG.IdxForColumn("R_tt"         );
    //ci.iT_ttNum      = TheG.IdxForColumn("R_ttNum"      );
      ci.iT_fiskTtNum  = TheG.IdxForColumn("R_fiskTtNum"  );
      ci.iT_date       = TheG.IdxForColumn("R_date"       );
      ci.iT_dateUpl    = TheG.IdxForColumn("R_dateUpl"    );
      ci.iT_partner    = TheG.IdxForColumn("R_partner"    );
      ci.iT_iznos      = TheG.IdxForColumn("R_iznos"      );
      ci.iT_uplata     = TheG.IdxForColumn("R_uplata"     );
      ci.iT_markPaid   = TheG.IdxForColumn("R_markPaid"   );
      ci.iT_razlikaUpl = TheG.IdxForColumn("R_razlikaUpl" );
      ci.iT_nalog      = TheG.IdxForColumn("R_nalog"      );
      ci.iT_konto      = TheG.IdxForColumn("R_konto"      );
      ci.iT_opis       = TheG.IdxForColumn("R_opis"       );
      ci.iT_tipUpl     = TheG.IdxForColumn("R_tipUpl"     );
   }

   #endregion SetColumnIndexes)

   #region PutDgvFields

   public void PutDgvFields()
   {
      int rowIdx;

      TheG.Rows.Clear();

      if(TheFtransList.IsEmpty()) return;

      for(rowIdx = 0; rowIdx < TheFtransList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
      {
         TheG.Rows.Add();
      
         PutDgvLineFields(rowIdx, TheFtransList[rowIdx]);
      
         TheG.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();
      }

   }

   /*private*/
   internal void PutDgvLineFields(int rowIdx, Ftrans ftrans_rec)
   {
      //Kupdob kupdob_rec = Get_Kupdob_FromVvUcSifrar(ftrans_rec.T_kupdob_cd);
      string tipUpl/* = ftrans_rec.T_TT == Nalog.IZ_TT ? "T" : ftrans_rec.T_TT == Nalog.KP_TT ? "O" : "Z"; //16.12.2025.*/;

      Faktur faktur_rec = Vv_eRacun_HTTP.GetFakturFromPaymentFtrans(ftrans_rec, TheDbConnection, out tipUpl);

      if(faktur_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, $"Ne mogu naći Faktur: {ftrans_rec.T_tipBr}\n\r\n\rFakRecID: {ftrans_rec.T_fakRecID} FakYear: {ftrans_rec.T_fakYear}");
      }
      else
      { 
         // Faktur 
         TheG.PutCell(ci.iT_fiskTtNum  , rowIdx, faktur_rec.VezniDok   );
         TheG.PutCell(ci.iT_date       , rowIdx, faktur_rec.DokDate    );
         TheG.PutCell(ci.iT_partner    , rowIdx, faktur_rec.KupdobName );
         TheG.PutCell(ci.iT_iznos      , rowIdx, faktur_rec.S_ukKCRP   );
      }

      // Ftrans 
      TheG.PutCell(ci.iT_tt         , rowIdx, ftrans_rec.T_tipBr);
      TheG.PutCell(ci.iT_dateUpl    , rowIdx, ftrans_rec.T_dokDate );
      TheG.PutCell(ci.iT_nalog      , rowIdx, ftrans_rec.T_dokNum + "/" + ftrans_rec.T_serial);
      TheG.PutCell(ci.iT_konto      , rowIdx, ftrans_rec.T_konto );
      TheG.PutCell(ci.iT_opis       , rowIdx, ftrans_rec.T_opis );
      TheG.PutCell(ci.iT_tipUpl     , rowIdx, tipUpl);
      TheG.PutCell(ci.iT_uplata     , rowIdx, ftrans_rec.T_pot);

      // Xtrano 
      Xtrano xtrano_rec = XtranoDao.SetMe_MAP_XtranoForThis_FtransRecID(TheDbConnection, ftrans_rec.T_recID);
      
      bool xtranoFound = xtrano_rec != null;

      decimal razlika = 0M;

      if(xtranoFound)
      {
         razlika = ftrans_rec.T_pot - xtrano_rec.T_moneyA;

         TheG.PutCell(ci.iT_markPaid  , rowIdx, xtrano_rec.T_moneyA);
         TheG.PutCell(ci.iT_razlikaUpl, rowIdx, razlika            );
      }
      else
      {
         TheG.PutCell(ci.iT_markPaid  , rowIdx, 0M);
         TheG.PutCell(ci.iT_razlikaUpl, rowIdx, 0M);
      }

      if(xtranoFound) TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(204, 255, 204);
      else            TheG.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(255, 194, 179);
   }

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   #endregion PutDgvFields1
}

#endregion Fiskalizacija F2
