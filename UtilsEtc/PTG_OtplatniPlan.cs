using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;

public class PTG_OtplatniPlan
{
   #region Propertiz

 //public Faktur           UGNorAUNfaktur_rec;
   public      PTG_Ugovor  UGANfaktur_rec    ;
   public List<PTG_Ugovor> DODfakturList     ;
   public List<Mixer>      KOPMixer_List     ;

 //public List<Rtrans>   AnaliticRtransList;
   public List<PTG_Rata> UGAN_RateList     ;
   public List<PTG_Rata> UGANwoKOP_RateList;

   public List<PTG_Rata> UGAN_NeFakturirane_RateList     { get { return                            UGAN_RateList.Where(rata => rata.IRA_TtNum.IsZero()).ToList()       ; } }
   public List<PTG_Rata> UGAN_NeFakturirane_RateList_UGN { get { return UGAN_TT == Faktur.TT_UGN ? UGAN_RateList.Where(rata => rata.IRA_TtNum.IsZero()).ToList() : null; } }
   public List<PTG_Rata> UGAN_NeFakturirane_RateList_AUN { get { return UGAN_TT == Faktur.TT_AUN ? UGAN_RateList.Where(rata => rata.IRA_TtNum.IsZero()).ToList() : null; } }

   //private XSqlConnection theDbConnection;

   public DateTime DatumPrveRate   { get { return UGAN_RateList.IsEmpty() ? DateTime.MinValue : UGAN_RateList.First().RataDate; } }
   public DateTime DatumZadnjeRate { get { return UGAN_RateList.IsEmpty() ? DateTime.MinValue : UGAN_RateList.Last ().RataDate; } }

   public decimal UkMoney       { get { return UGAN_RateList.Sum(r => r.RataMoney); } }
   public int     SastavZadRate { get { return UGAN_RateList.IsEmpty() ? 0 : UGAN_RateList.Last().RataSastav; } }

   public ZXC.PTG_DanFakturiranjaEnum UGANdanFakturiranja { get { return UGANfaktur_rec.PTG_DanFakturiranja; } }

   public bool IsNaDanUgovora { get { return UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora; } }

   public string   UGAN_TT        { get { return UGANfaktur_rec.TT          ; } }
   public DateTime UGANdate       { get { return UGANfaktur_rec.DokDate     ; } }
   public decimal  UGANmoney      { get { return UGANfaktur_rec.S_ukKCR     ; } }
   public uint     UGANttNum      { get { return UGANfaktur_rec.TtNum       ; } }
   public string   UGANrnNapomena { get { return UGANfaktur_rec.Napomena2   ; } }
   public int      UGANrokPlac    { get { return UGANfaktur_rec.RokPlac     ; } }
   public string   UGANnarudzba   { get { return UGANfaktur_rec.PTG_Narudzba; } }

   // UGN's KupdobCD 4 Fakturiranje is UGN.KupdobCD, ANU's KupdobCD 4 fakturiranje is AUN.Konto - (KUG partner's ticker)
   public uint     UGANkupdobCD   { get { return UGANfaktur_rec.KupdobCD ; } }
   public string   KUGkupdobTK    { get { return UGANfaktur_rec.Konto    ; } }
 
   //public uint     KUGkupdobCD
   //{
   //   get
   //   {
   //      if(UGANfaktur_rec.TT == Faktur.TT_AUN)
   //      {
   //         return UGANfaktur_rec.KupdobCD;
   //      }
   //      else // NIJE AUN 
   //      {
   //         return 0;
   //      }
   //   }
   //}

   public bool UGANdate_equals_LastDayOfMonth
   {
      get
      {
         int UGANday        = UGANdate.Day;
         int lastDayOfMonth = DateTime.DaysInMonth(UGANdate.Year, UGANdate.Month);

         return UGANday == lastDayOfMonth;
      }
   }

   public bool UGANdate_equals_FirstDayOfMonth
   {
      get
      {
         int UGANday         = UGANdate.Day;
         int firstDayOfMonth = 1;

         return UGANday == firstDayOfMonth;
      }
   }

   public bool UGANdate_equals_FirstRataDate
   {
      get
      {
         if(IsNaDanUgovora                                                                                        ) return true;
         if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca && UGANdate_equals_LastDayOfMonth ) return true;
         if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca   && UGANdate_equals_FirstDayOfMonth) return true;

         return false;
      }
   }

   public int NumOf_OtPlan_Rows_UgPredRata  { get { return (UGANdate_equals_FirstRataDate || UGANdate_equals_FirstDayOfMonth) ? 0 : 1 ; } }

   public int NumOf_OtPlan_Rows_UgBrojRata  { get { return      UGANfaktur_rec.PTG_BrojRata     ; } }
   public int NumOf_OtPlan_Rows_DodPredRata { get; set; }
   public int NumOf_OtPlan_Rows_KorekRata   { get { return (int)UGANfaktur_rec.PTG_BrojNovihRata; } }
   public int NumOf_OtPlan_Rows_OtkupRata   { get { return      UGANfaktur_rec.PTG_HasOtkup ? 1 : 0 ; } }

   public int NumOf_OtPlan_Rows_OsnovRate
   {
      get
      {
         return
         NumOf_OtPlan_Rows_UgBrojRata +
         NumOf_OtPlan_Rows_KorekRata  ;
      }
   }

   public int NumOf_OtPlan_Rows_UK
   {
      get
      {
         return
         NumOf_OtPlan_Rows_UgPredRata  +
         NumOf_OtPlan_Rows_UgBrojRata  +
         NumOf_OtPlan_Rows_DodPredRata +
         NumOf_OtPlan_Rows_KorekRata   +
         NumOf_OtPlan_Rows_OtkupRata   ;
      }
   }

   #endregion Propertiz

   #region Constructors

   /// <summary>
   /// UGN ili AUN tt i ttNum kao parametri
   /// </summary>
   /// <param name="tt"></param>
   /// <param name="ttNum"></param>
   public PTG_OtplatniPlan(XSqlConnection conn, string tt, uint ttNum)
   {
      SetMe_UGANfaktur_rec(conn, tt, ttNum);

      CreateRate_CalcOtplatniPlan(conn);
   }

   /// <summary>
   /// UGN ili AUN faktur_rec kao parametar
   /// </summary>
   /// <param name="UGNorAUNfaktur_rec"></param>
   public PTG_OtplatniPlan(XSqlConnection conn, Faktur _UGNorAUNfaktur_rec)
   {
      Faktur faktur_rec = (Faktur)_UGNorAUNfaktur_rec.CreateNewRecordAndCloneItComplete();
      this.UGANfaktur_rec = new PTG_Ugovor(faktur_rec);

      CreateRate_CalcOtplatniPlan(conn);
   }

   public PTG_OtplatniPlan(XSqlConnection conn, PTG_Ugovor _PTG_Ugovor_rec)
   {
      this.UGANfaktur_rec = _PTG_Ugovor_rec;

      CreateRate_CalcOtplatniPlan(conn);
   }

   #endregion Constructors

   #region SetMe UGNorANU, DOD, KOP

   private void SetMe_UGANfaktur_rec(XSqlConnection conn, string tt, uint ttNum)
   {
      this.UGANfaktur_rec = new PTG_Ugovor();

      bool OK = FakturDao.SetMeFaktur(conn, UGANfaktur_rec, tt, ttNum, false);

      if(!OK) { ZXC.aim_emsg("Nema UGN / ANU tt [{0}] ttNum [{1}]", tt, ttNum); return; }

      UGANfaktur_rec.VvDao.LoadTranses(conn, UGANfaktur_rec, false);
   }

   private void Get_DOD_FakturList(XSqlConnection conn, List<PTG_Ugovor> _DODfakturList, bool isZIZwanted)
   {
      uint wantedKUG  = UGANfaktur_rec.V1_ttNum;
      uint wantedUoA  = UGANfaktur_rec.V2_ttNum;

      List<VvSqlFilterMember> filterMembers = GetFilterMembers_DOD_FakturList(wantedKUG, wantedUoA, isZIZwanted);

      VvDaoBase.LoadGenericVvDataRecordList<PTG_Ugovor>(conn, _DODfakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      foreach(Faktur faktur_rec in _DODfakturList)
      {
         faktur_rec.VvDao.LoadTranses(conn, faktur_rec, false);
      }

      //return _DODorKOPfakturList;
   }

   private void SetMe_KOPmixerList(XSqlConnection conn, List<Mixer> _KOPmixerList, string wantedTT)
   {
      uint wantedKUG  = UGANfaktur_rec.V1_ttNum;
      uint wantedUoA  = UGANfaktur_rec.V2_ttNum;

      List<VvSqlFilterMember> filterMembers = GetFilterMembers_KOPmixerList(wantedTT, wantedKUG, wantedUoA);

      VvDaoBase.LoadGenericVvDataRecordList<Mixer>(conn, _KOPmixerList, filterMembers, "", "dokDate, tt, ttNum", false);

      foreach(Mixer KOPmixer_rec in _KOPmixerList)
      {
         KOPmixer_rec.VvDao.LoadTranses(conn, KOPmixer_rec, false);
      }

      //return _DODorKOPfakturList;
   }

   internal static List<VvSqlFilterMember> GetFilterMembers_DOD_FakturList(uint wantedKUG, uint wantedUoA, bool isZIZwanted)
   {
    // 20.02.2025.
    //List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(5);
    
    //filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ],                                 "theTT" , wantedTT              , " = "    ));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], ZXC.FM_OR_Enum.OPEN_OR , false, "theTT" , Faktur.TT_DIZ, "", "", "  = ", ""));
      if(isZIZwanted)
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], ZXC.FM_OR_Enum.NONE    , false, "theTT3", Faktur.TT_ZIZ, "", "", "  = ", ""));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], ZXC.FM_OR_Enum.CLOSE_OR, false, "theTT2", Faktur.TT_PVR, "", "", "  = ", ""));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v1_ttNum], "KUGnum", wantedKUG, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.v2_ttNum], "UoAnum", wantedUoA, " = "));

      return filterMembers;
   }

   internal static List<VvSqlFilterMember> GetFilterMembers_KOPxtransList(string wantedTT, uint wantedKUG, uint wantedUGANttNum)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      uint wantedTTnum = wantedKUG * 100000 + wantedUGANttNum;

      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt   ], "theTT" , wantedTT   , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_ttNum], "UoAnum", wantedTTnum, " = "));

      return filterMembers;
   }

   internal static List<VvSqlFilterMember> GetFilterMembers_KOPmixerList(string wantedTT, uint wantedKUG, uint wantedUoA)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt      ], "theTT" , wantedTT , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.v1_ttNum], "KUGnum", wantedKUG, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.v2_ttNum], "UoAnum", wantedUoA, " = "));

      return filterMembers;
   }

   #endregion SetMe UGNorANU, DOD, KOP

   private void CreateRate_CalcOtplatniPlan(XSqlConnection conn)
   {
      #region Init

      PTG_Rata rata;

      int      rataRbr  ;
      DateTime rataDate ;
      decimal  rataMoney;
      decimal  rataKoef ;

      int brojFaktura;

      int      UGANbrojRata_Orig = UGANfaktur_rec.PTG_BrojRata          ; // inicijalni broj rata 'Trajanje Ugovora'  
      int      UGANbrojRata_Add  = (int)UGANfaktur_rec.PTG_BrojNovihRata; // novododane ili oduzete rate              
      int      UGANbrojRata_UK   = UGANbrojRata_Orig + UGANbrojRata_Add ; // sveukupan broj rata                      

      bool     isFirstRata;
      bool     isLastRata ;

      bool shouldSkipLastRata = (UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca || 
                                 UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca) && UGANdate.Day == 1;

      //bool isNaDanUgovora = UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora;

      bool Is_DOD_initialRata_ExtraFaktur = false;

      #endregion Init

      #region SetMe_DODandKOP_fakturList, AnaliticRtransList

      DODfakturList = new List<PTG_Ugovor>();
      KOPMixer_List = new List<Mixer     >();

    //SetMe_DODfakturList(conn, DODfakturList, Faktur.TT_DIZ);
      Get_DOD_FakturList (conn, DODfakturList, false        );
      SetMe_KOPmixerList (conn, KOPMixer_List, Mixer .TT_KOP);

      #region AnaliticRtransList
      // ... zapravo ti ne treba Rtrans neg Vaktur list will do 
      //AnaliticRtransList = new List<Rtrans>(UGANfaktur_rec.Transes.Count);
      //
      //AnaliticRtransList.AddRange(UGANfaktur_rec.Transes);
      //
      //foreach(Faktur DODfaktur in DODfakturList)
      //{
      //   AnaliticRtransList.AddRange(DODfaktur.Transes);
      //}
      //
      //foreach(Faktur KOPfaktur in KOPfakturList)
      //{
      //   AnaliticRtransList.AddRange(KOPfaktur.Transes);
      //}
      #endregion AnaliticRtransList

      #endregion SetMe_DODandKOP_fakturList, AnaliticRtransList

      #region PTG_isJednokratnoPl ... Add one rata then return

      if(UGANfaktur_rec.PTG_b9_isJednokratPl)
      {
         rataRbr   = 1        ;
         rataDate  = UGANdate ;
         rataMoney = UGANmoney;
         rataKoef  = 1.00M    ;

         UGAN_RateList = new List<PTG_Rata>(1)
         {
            new PTG_Rata(rataRbr, UGANttNum, UGANrnNapomena, UGANrokPlac, UGANnarudzba, rataDate, rataMoney, rataKoef, true, UGANdate, UGANkupdobCD, KUGkupdobTK)
         };

         return;
      }

      #endregion PTG_isJednokratnoPl

      #region Many Ratas ... Voila 1

      //switch(UGANdanFakturiranja) // brojFaktura 
      //{
      //   case ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora    : brojFaktura = UGANbrojRata    ; break;
      //   case ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca  : 
      //   case ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca: brojFaktura = UGANbrojRata + 1; break;
      //   default                                          : brojFaktura =                1; break;
      //}

      if(UGANdate_equals_FirstRataDate || UGANdate_equals_FirstDayOfMonth) brojFaktura = UGANbrojRata_UK    ;
      else                                                                 brojFaktura = UGANbrojRata_UK + 1;

      UGAN_RateList = new List<PTG_Rata>(brojFaktura);

      bool brojFaktura_JE_BrojRata = brojFaktura == UGANbrojRata_UK;

      decimal firstRataKoef = Calc_firstRataKoef(UGANdate)/*.Ron2()*/;
      decimal lastRataKoef  = (1.00M - firstRataKoef)     /*.Ron2()*/;

    //if(IsNaDanUgovora)          firstRataKoef = lastRataKoef = 1.00M;
      if(brojFaktura_JE_BrojRata) firstRataKoef = lastRataKoef = 1.00M;

      for(int fakRbr = 1; fakRbr <= brojFaktura; ++fakRbr)
      {
         //// DELME!!!:
         //if(fakRbr >= 3)
         //{
         //   UGANfaktur_rec.PTG_DanFakturiranjaString = "NaDanUgovora";
         //
         //}

         isFirstRata = fakRbr == 1;
         isLastRata  = fakRbr == brojFaktura;

         rataDate    = CalcRataDate(fakRbr, UGANdanFakturiranja, UGANdate, isFirstRata);
         rataMoney   = CalcRataMoney(UGANmoney, rataDate, isFirstRata, isLastRata, firstRataKoef, lastRataKoef, out rataKoef, UGANdanFakturiranja, false);

         rata = new PTG_Rata(fakRbr, UGANttNum, UGANrnNapomena, UGANrokPlac, UGANnarudzba, rataDate, rataMoney, isFirstRata, isLastRata, rataKoef, UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora, UGANdate, UGANkupdobCD, KUGkupdobTK);

         rata.UGANrataMoney = rataMoney;

         //// ne dodaj '13' praznu ratu 
         //if(isLastRata && shouldSkipLastRata) 
         //{
         //   continue; // nemoj dodavati ovu ratu u RateList 
         //}

         UGAN_RateList.Add(rata);

      } // for(int fakRbr = 1; fakRbr <= brojFaktura; ++fakRbr)

      #endregion Many Ratas ... Voila 1

      #region Dodaci ... Voila 2

      NumOf_OtPlan_Rows_DodPredRata = 0;

      if(DODfakturList.NotEmpty())
      {
         List<PTG_Rata> DOD_rateList;

         foreach(PTG_Ugovor DODfaktur_rec in DODfakturList)
         {
            #region Create Independent OP for thid DOD

            DOD_rateList = new List<PTG_Rata>(UGAN_RateList.Count);

            DateTime DODdate  = DODfaktur_rec.DokDate;

            // dodatno izdavanje / zaduženje kupca 
            decimal DODmoney = DODfaktur_rec.S_ukKCR;

            // povrat / razduženje kupca 
          //if(DODfaktur_rec.TtInfo.IsFinKol_U /* Faktur.TT_PVR */) ovo treba provjeriti ?!?!? 21.02.2025.
            if(DODfaktur_rec.TT == Faktur.TT_PVR) 
            {
               DODmoney *= -1M; // mjenjamo predznak 
            }

            DateTime firstNext_UGANrataDate, startDate;

            firstNext_UGANrataDate = UGAN_RateList.First/*OrDefault*/(r => r.RataDate > DODdate).RataDate; // nek skace exception aknema

            if(IsNaDanUgovora) firstRataKoef = Calc_firstRataKoef_DOD_naDanUgovora(DODdate, firstNext_UGANrataDate)/*.Ron2()*/;
            else               firstRataKoef = Calc_firstRataKoef                 (DODdate                        )/*.Ron2()*/;
            //lastRataKoef  = (1.00M - firstRataKoef)                                                              /*.Ron2()*/;
            //lastRataKoef  = (1.00M                ) /* !!! */                                                    /*.Ron2()*/; ... koristi se UGAN-ova lastRataKoef gore izracunana 

            DateTime tempDODRataDate;

            for(int fakRbr = 1; fakRbr <= brojFaktura; ++fakRbr) // brojFaktura je preuzet od UGAN-a, treba brejkati kad premasi 
            {
               isFirstRata = fakRbr == 1;

               if(IsNaDanUgovora) startDate = isFirstRata ? DODdate : firstNext_UGANrataDate;
               else               startDate = DODdate;

               if(IsNaDanUgovora && isFirstRata == false) // samo prvu ratu za IsNaDanUgovora racunaj drudrugacije 
               {
                  tempDODRataDate = CalcRataDate(fakRbr, UGANdanFakturiranja, startDate, isFirstRata);

                  rataDate = UGAN_RateList.First/*OrDefault*/(r => r.RataDate >= tempDODRataDate).RataDate; // nek skace exception aknema
               }
               else
               {
                  rataDate = CalcRataDate(fakRbr, UGANdanFakturiranja, startDate, isFirstRata);
               }

               isLastRata = rataDate == DatumZadnjeRate; // !!!

               rataMoney = CalcRataMoney(DODmoney, rataDate, isFirstRata, isLastRata, firstRataKoef, lastRataKoef, out rataKoef, UGANdanFakturiranja, true);

               rata = new PTG_Rata(fakRbr, UGANttNum, UGANrnNapomena, UGANrokPlac, UGANnarudzba, rataDate, rataMoney, isFirstRata, isLastRata, rataKoef, IsNaDanUgovora, UGANdate, UGANkupdobCD, KUGkupdobTK);

               //// ne dodaj '13' praznu ratu 
               //if(isLastRata && shouldSkipLastRata) 
               //{
               //   continue; // nemoj dodavati ovu ratu u RateList 
               //}

               DOD_rateList.Add(rata);

               if(isLastRata) break; // !!! 

            } // for(int fakRbr = 1; fakRbr <= brojFaktura; ++fakRbr)

            #region Is_DOD_initialRata_ExtraFaktur

            bool DOD_initialRataDate_ExistsIn_UGAN_RateList = UGAN_RateList.Any(rt => rt.RataDate.Date == DOD_rateList.First().RataDate.Date);
            bool DOD_initialRataDate_IsNotIn_UGAN_RateList  = !DOD_initialRataDate_ExistsIn_UGAN_RateList;

            Is_DOD_initialRata_ExtraFaktur = DOD_initialRataDate_IsNotIn_UGAN_RateList;

            if(Is_DOD_initialRata_ExtraFaktur)
            { 
               brojFaktura++;

               NumOf_OtPlan_Rows_DodPredRata++;
            }

            #endregion Is_DOD_initialRata_ExtraFaktur

            #region Insert additional rata on isNaDanUgovora after first rata

            if(IsNaDanUgovora)
            {
               DOD_rateList.Add(new PTG_Rata(/*0*/ -123 /*fakRbr*/, UGANttNum, UGANrnNapomena, /*rataDate*/UGANrokPlac, UGANnarudzba, firstNext_UGANrataDate, /*rataMoney*/ DODmoney, /*isFirstRata*/ false, /*isLastRata*/ false, /*rataKoef*/ 1.00M, IsNaDanUgovora, UGANdate, UGANkupdobCD, KUGkupdobTK));

               DOD_rateList = DOD_rateList.OrderBy(r => r.RataDate)/*.ThenBy(r => r.RataRbr)*/.ToList();
            }

            #endregion Insert additional rata on isNaDanUgovora after first rata

            #endregion Create Independent OP for thid DOD

            #region INJECT / MERGE DOD_rateList to RateList

            PTG_Rata UGAN_rata;

            foreach(PTG_Rata DOD_rata in DOD_rateList)
            {
               UGAN_rata = UGAN_RateList.SingleOrDefault(uRata => uRata.RataDate == DOD_rata.RataDate);

               if(UGAN_rata == null) // nema UGAN rate na DOD rata datum 
               {
                  PTG_Rata newDODrata = new PTG_Rata(/*0*/ -((int)DODfaktur_rec.PTG_DOKOnum), UGANttNum, UGANrnNapomena, UGANrokPlac, UGANnarudzba, DOD_rata.RataDate, DOD_rata.RataMoney, DOD_rata.RataKoef, IsNaDanUgovora, UGANdate, UGANkupdobCD, KUGkupdobTK);

                  newDODrata.DODrataMoneyList.Add(DOD_rata.RataMoney);

                  // 02.06.2022. tu nam treba TipBr
                //newDODrata.SomeInfo = "DOD-" + DODfaktur_rec.PTG_DOKOnum.ToString();
                  newDODrata.SomeInfo = DODfaktur_rec.TipBr;
                  
                  UGAN_RateList.Add(newDODrata);
               }
               else // nasao je UGAN ratu na datum ove DOD rate; pribroji money 
               {
                  UGAN_rata.RataMoney += DOD_rata.RataMoney;

                  UGAN_rata.RataSastav++;

                  UGAN_rata.DODrataMoneyList.Add(DOD_rata.RataMoney);
                  UGAN_rata.DODrataKoefList .Add(DOD_rata.RataKoef );
               }
            }

            UGAN_RateList = UGAN_RateList.OrderBy(r => r.RataDate)/*.ThenBy(r => r.RataRbr)*/.ToList();

            #endregion INJECT / MERGE DOD_rateList to RateList

         } // foreach(PTG_Ugovor DODfaktur_rec in DODfakturList) 

      } // if(DODfakturList.NotEmpty()) 

      #endregion Dodaci ... Voila 2

      #region Za 'isNaDanUgovora' treba posebna logika 'DatumDo'

      if(IsNaDanUgovora)
      {
         for(int rataIdx = 0; rataIdx < UGAN_RateList.Count; ++rataIdx)
         {
            if(UGAN_RateList[rataIdx].IsLastRata)
            {
               UGAN_RateList[rataIdx].DatumDo = UGAN_RateList[rataIdx].RataDate.AddMonths(1).AddDays(-1);
            }
            else
            {
               UGAN_RateList[rataIdx].DatumDo = UGAN_RateList[rataIdx + 1].RataDate.AddDays(-1);
            }
         }
      }

      #endregion Za 'isNaDanUgovora' treba posebna logika 'DatumDo'

      #region Set serial

      for(int i = 0; i < UGAN_RateList.Count; ++i)
      {
         UGAN_RateList[i].Serial = (ushort)(i + 1);
      }

      #endregion Set serial

      #region ADD eventual OTKUP rata Voila 3

      if(UGANfaktur_rec.PTG_HasOtkup) 
      {
         decimal otkupRataMoney = UGANfaktur_rec.GetOtkupRataMoney(conn);

         if(otkupRataMoney.NotZero())
         {
            DateTime otkupRataDate = UGAN_RateList.Last().DatumDo + ZXC.OneDaySpan; 
            int      otkupRataRbr  = UGAN_RateList.Last().RataRbr ;

            PTG_Rata otkupRata  = new PTG_Rata(otkupRataRbr /*fakRbr*/, UGANttNum, UGANrnNapomena, /*rataDate*/UGANrokPlac, UGANnarudzba, otkupRataDate, /*rataMoney*/ otkupRataMoney, /*isFirstRata*/ false, /*isLastRata*/ false, /*rataKoef*/ 1.00M, IsNaDanUgovora, UGANdate, UGANkupdobCD, KUGkupdobTK);

            otkupRata.IsOtkup  = true;
            otkupRata.SomeInfo = "OTKUP";
            otkupRata.DatumOd =
            otkupRata.DatumDo = otkupRataDate/*UGAN_RateList.Last().DatumDo*/;

            otkupRata.RataRbr =          UGAN_RateList.Last().RataRbr + 1;
            otkupRata.Serial  = (ushort)(UGAN_RateList.Last().Serial  + 1);

            UGAN_RateList.Add(otkupRata);
         }
      }

      #endregion ADD eventual OTKUP rata Voila 3

      #region MERGE eventual KOP xtranses Voila 4

    //UGANwoKOP_RateList = new List<PTG_Rata>(UGAN_RateList).ToList(); // ToList() da prekine shallow copy 
      UGANwoKOP_RateList = UGAN_RateList.ConvertAll(r => ZXC.DeepCopy(r));

      if(KOPMixer_List.NotEmpty())
      {
         Mixer KOPmixer_rec = KOPMixer_List.First();

         Merge_KOP(KOPmixer_rec, false);

      } // if(KOPMixer_List.NotEmpty()) 

      #endregion MERGE eventual KOP xtranses Voila 4

   } // private void CalcOtplatniPlan(XSqlConnection conn) 

   internal void Merge_KOP(Mixer KOPmixer_rec, bool isSetDatumOdDo_sazeteRate)
   {
      List<Xtrans> KOPxtransList = KOPmixer_rec.Transes;

      Xtrans thisRata_KOPxtrans_rec;

      foreach(PTG_Rata UGAN_rata in UGAN_RateList)
      {
         thisRata_KOPxtrans_rec = KOPxtransList.SingleOrDefault(x => x.T_intA == UGAN_rata.RataRbr);

         if(thisRata_KOPxtrans_rec != null)
         {
            Overwrite_UGANrataDataWithKOPrataData(UGAN_rata, thisRata_KOPxtrans_rec);
         }
      }

      if(isSetDatumOdDo_sazeteRate == false) return;

      #region Za eventualne SAŽETE otplatne planove korigiraj DatumOd ili DatumDo

      #region Set bool IsPrvaUjedinjenaRata

      for(int i = 0; i < UGAN_RateList.Count; ++i)
      {
         if(IsUjedinjenaRata(UGAN_RateList[i].Serial))
         {
            UGAN_RateList[i].IsPrvaUjedinjenaRata = true;
            break;
         }
      }

      #endregion Set bool IsPrvaUjedinjenaRata

      #region Set bool IsJadnickaRata

      for(int i = 0; i < UGAN_RateList.Count; ++i)
      {
         UGAN_RateList[i].IsJadnickaRata = IsJadnickaRata(UGAN_RateList[i].Serial);
      }

      #endregion Set bool IsJadnickaRata

      foreach(PTG_Rata UGAN_rata in UGAN_RateList)
      {
         if(IsFrajerskaRata(UGAN_rata.Serial)) // placaju se rate UNAPRIJED jer smo se upravo obogatili 
         {
            UGAN_rata.DatumDo = GetFrajerskiDatumDo(UGAN_rata.Serial); // Daj mi DatumDo od zadnje ZeroMoney rate od sažetih 
         }
         if(IsJadnickaRata(UGAN_rata.Serial)) // placaju se rate UNAZAD ... nakon sto smo se obogatili 
         {
            UGAN_rata.DatumOd = GetJadnickiDatumOd(UGAN_rata.Serial); // Daj mi DatumOd od prve   ZeroMoney rate od sažetih 
         }
      }

      #endregion Za eventualne SAŽETE otplatne planove korigiraj DatumOd ili DatumDo
   }

   /*private*/
   internal static void Overwrite_UGANrataDataWithKOPrataData(PTG_Rata UGAN_rata, Xtrans thisRata_KOPxtrans_rec)
   {
      UGAN_rata.RataMoney          = thisRata_KOPxtrans_rec.T_moneyA        ;
      UGAN_rata.RataDate           = thisRata_KOPxtrans_rec.T_date3         ;
      UGAN_rata.KOPxtransNapomena  = thisRata_KOPxtrans_rec.T_opis_128      ;
      UGAN_rata.KOPxtransDate      = thisRata_KOPxtrans_rec.T_date4         ;
      UGAN_rata.KOPxtransAdduid    = thisRata_KOPxtrans_rec.T_kpdbNameA_50  ;
      UGAN_rata.KOPfakStop         = thisRata_KOPxtrans_rec.T_intB.NotZero();
      UGAN_rata.HasKOP             = true                                   ;

      UGAN_rata.DatumOd            = thisRata_KOPxtrans_rec.T_dateOd        ;
      UGAN_rata.DatumDo            = thisRata_KOPxtrans_rec.T_dateDo        ;

      UGAN_rata.xtrans_rec         = (Xtrans)thisRata_KOPxtrans_rec.CreateNewRecordAndCloneItComplete();
   }

   private DateTime CalcRataDate(int fakRbr, ZXC.PTG_DanFakturiranjaEnum UGANdanFakturiranja, DateTime UGANdate, bool isFirstRata)
   {
      DateTime rataDate         = new DateTime();
      DateTime datumPrveFakture = new DateTime();

      DateTime nextMonthSameDay;

      int monthsToAdd = fakRbr - 1;

      if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora)
      {
         datumPrveFakture = UGANdate;

         rataDate = datumPrveFakture.AddMonths(monthsToAdd);
      }
      else if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca)
      {
         datumPrveFakture = UGANdate.ThisMonthLastDay();

         nextMonthSameDay = datumPrveFakture.AddMonths(monthsToAdd);

         rataDate = new DateTime(nextMonthSameDay.Year, nextMonthSameDay.Month, DateTime.DaysInMonth(nextMonthSameDay.Year, nextMonthSameDay.Month));
      }
      else if(UGANfaktur_rec.PTG_DanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca)
      {
         if(isFirstRata) { datumPrveFakture = UGANdate                    ; monthsToAdd = 0; } // !!! 
         else            { datumPrveFakture = UGANdate.ThisMonthFirstDay();                  }

         rataDate = datumPrveFakture.AddMonths(monthsToAdd);
      }

      return rataDate; 
   }

   private decimal CalcRataMoney(decimal UGANMoney, DateTime rataDate, bool isFirstRata, bool isLastRata, decimal firstRataKoef, decimal lastRataKoef, out decimal rataKoef, ZXC.PTG_DanFakturiranjaEnum UGANdanFakturiranja, bool isDOD)
   {
      #region puse delmelater
      //if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora)
      //{
      //   rataKoef = 1.00M;
      //}
      //else if(UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca ||
      //        UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca)
      //{
      //        if(isFirstRata) rataKoef = firstRataKoef;
      //   else if(isLastRata)  rataKoef = lastRataKoef ; // tu jos treba vidjeti oce li nas zaokruzivanje zajebavati pa ako da, treba rataMoney = UGANmoney - prvaRataMoney 
      //   else                 rataKoef = 1.00M        ;
      //}
      #endregion puse delmelater

      rataKoef = 1.00M;

      if(
         (         UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.ZadnjiDanMjeseca) ||
         (         UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca  ) ||
         (isDOD && UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora    )
         )
      {
              if(isFirstRata) rataKoef = firstRataKoef;
         else if(isLastRata)  rataKoef = lastRataKoef ; // tu jos treba vidjeti oce li nas zaokruzivanje zajebavati pa ako da, treba rataMoney = UGANmoney - prvaRataMoney 
      }

      if(isDOD && UGANdanFakturiranja == ZXC.PTG_DanFakturiranjaEnum.NaDanUgovora)
      {

      }

    //return (UGANMoney * rataKoef)       ; 
      return (UGANMoney * rataKoef).Ron2();
   }

   private decimal Calc_firstRataKoef(DateTime UGANdate)
   {
      int mjesecImaDana = DateTime.DaysInMonth(UGANdate.Year, UGANdate.Month);
      int brojDana      = mjesecImaDana - UGANdate.Day + 1;

      return ZXC.DivSafe(brojDana, mjesecImaDana);
   }

   private decimal Calc_firstRataKoef_DOD_naDanUgovora(DateTime DODdate, DateTime firstNext_UGANdate)
   {
      int mjesecImaDana = DateTime.DaysInMonth(DODdate.Year, DODdate.Month);

      TimeSpan timeSpan = firstNext_UGANdate.Date - DODdate.Date /* + 1*/;
      int brojDana      = (int)timeSpan.TotalDays;

      return ZXC.DivSafe(brojDana, mjesecImaDana);
   }

   internal void GetAllRate_IRA_TtNum(XSqlConnection conn)
   {
      foreach(PTG_Rata rata in this.UGAN_RateList)
      {
         rata.IRA_TtNum = GetIRA_TtNum_ForThisRata(conn, /*UGANfaktur_rec.TtNum, rata.RataRbr*/rata);
      }
   }

   private uint GetIRA_TtNum_ForThisRata(XSqlConnection conn, PTG_Rata rata)
   {
      bool           isSomePreviousYear;
      XSqlConnection          conn4year;

      string serlot4Rtrans = rata.Serlot4Rtrans;

      if(rata.IsOtkup) serlot4Rtrans += "/1";

    //List<Rtrans> rtransList = RtransDao.GetRtransList_ForSerlot       (conn,                serlot4Rtrans);
    //List<Rtrans> rtransList = RtransDao.GetRtransList_ForTT_And_Serlot(conn, Faktur.TT_IFA, serlot4Rtrans);
      List<Rtrans> rtransList = new List<Rtrans>();

      for(int year = rata.UGANdate.Year; year <= ZXC.projectYearFirstDay.Year; ++year)
      {
         isSomePreviousYear = year < ZXC.projectYearFirstDay.Year;

         conn4year = isSomePreviousYear ? ZXC.TheSecondDbConn_SameDB_OtherYear(year) : conn;

         rtransList.AddRange(RtransDao.GetRtransList_ForTT_And_Serlot(conn4year, Faktur.TT_IRA, serlot4Rtrans));
      }

      if(rtransList.IsEmpty()) return 0;
      else
      {
         if(rtransList.Count > 1 && !rata.IsOtkup) { ZXC.aim_emsg(MessageBoxIcon.Warning, "Pojava rate \n\t{0} - {1} \n\rna više od 1 fakture?!", rata.UGANttNum, rata.RataRbr); }
         return rtransList.First().T_ttNum;
      }
   }

   public Xtrans Create_Xtrans_FromPTG_Rata(PTG_Rata rata)
   {
      Xtrans xtrans_rec = new Xtrans()
      {
         T_serial       = rata.Serial,
         T_intA         = rata.RataRbr,
         T_date3        = rata.RataDate,
         T_moneyA       = rata.RataMoney,
         T_intB         =(rata.KOPfakStop == true ? 1 : 0),
         T_opis_128     = rata.KOPxtransNapomena,
         T_date4        = rata.KOPxtransDate,
         T_kpdbNameA_50 = rata.KOPxtransAdduid,
         T_isXxx        = rata.HasKOP,

         T_dateOd       = rata.DatumOd,
         T_dateDo       = rata.DatumDo,
      };

      return xtrans_rec;
   }

   private PTG_Rata RataOfThisSerial(ushort serial)
   {
      return this.UGAN_RateList[serial - 1];
   }

   #region Frajerska / Jadnicka rata

   private DateTime GetFrajerskiDatumDo(ushort serial) // Daj mi DatumDo od zadnje ZeroMoney rate od sažetih 
   {
      PTG_Rata zadnja_rata = UGAN_RateList.Where(rata => rata.Serial > serial).TakeWhile(rata => rata.RataMoney.IsZero()).Last();

      return zadnja_rata.DatumDo;
   }

   private DateTime GetJadnickiDatumOd (ushort serial) // Daj mi DatumOd od prve   ZeroMoney rate od sažetih 
   {
      var naglavacke       = UGAN_RateList.OrderByDescending(rata => rata.Serial);
      var manjiSeriali     = naglavacke.Where(rata => rata.Serial < serial);
      var skupDoPrveNeNule = manjiSeriali.TakeWhile(rata => rata.RataMoney.IsZero());

      PTG_Rata prva_rata   = skupDoPrveNeNule.Last(); // last jer je na pocetku presortirano naglavacke da bi mogao 'TakeWhile' 

      return prva_rata.DatumOd;
   }

   private bool IsPrvaUjedinjenaRata_Frajerska()
   {
      PTG_Rata prvaUjedinjenaRata = UGAN_RateList.FirstOrDefault(rata => rata.IsPrvaUjedinjenaRata == true);

      if(prvaUjedinjenaRata == null) return false;

      if(prvaUjedinjenaRata.Serial == 1) return true;

      if(IsPrevRataZeroMoney(prvaUjedinjenaRata.Serial) == false) return true;

      return false;
   }

   private bool IsUjedinjenaRata(ushort serial)
   {
      PTG_Rata rata = RataOfThisSerial(serial);

      return

         rata.HasKOP &&
         rata.RataMoney > (rata.UGANrataMoney + rata.UkDODrataMoney) &&

         (IsPrevRataZeroMoney(rata.Serial) || IsNextRataZeroMoney(rata.Serial));
   }

   private bool IsPrevRataZeroMoney(ushort serial)
   {
      PTG_Rata this_rata = RataOfThisSerial(serial);

      if(this_rata.IsFirstRata) return false;

      PTG_Rata prev_rata = UGAN_RateList[serial - 2];

      return prev_rata.RataMoney.IsZero();
   }

   private bool IsNextRataZeroMoney(ushort serial)
   {
      PTG_Rata this_rata = RataOfThisSerial(serial);

      if(this_rata.IsLastRata) return false;

      PTG_Rata next_rata = UGAN_RateList[serial /*- 1*/];

      return next_rata.RataMoney.IsZero();
   }

   private bool IsFrajerskaRata(ushort serial)
   {
    //return IsUjedinjenaRata(serial) && IsNextRataZeroMoney(serial);
      return IsUjedinjenaRata(serial) && IsPrvaUjedinjenaRata_Frajerska() == true;
   }

   private bool IsJadnickaRata(ushort serial)
   {
    //return IsUjedinjenaRata(serial) && IsPrevRataZeroMoney(serial);
      return IsUjedinjenaRata(serial) && IsPrvaUjedinjenaRata_Frajerska() == false;
   }

   #endregion Frajerska / Jadnicka rata

} // public class PCT_OtplatniPlan 

public class PTG_Rata
{
   #region propertiz

   public int      RataRbr  ;
   public DateTime RataDate ;
   public decimal  RataMoney;
   public decimal  RataKoef ;

   public int      RataSastav; // od koliko dokumenata se sastoji (UGAN + DOD1 + DOD2 + ...) 

   public bool IsFirstRata;
   public bool IsLastRata ;
   public bool IsNaDanUgovora;

   public bool IsJadnickaRata;
   public bool IsPrvaUjedinjenaRata;
   public bool IsOtkup;

   public string UGANtt
   {
      get
      {
         if(UGANttNum <= 99999) return Faktur.TT_UGN;
         else                   return Faktur.TT_AUN;
      }
   }

   public      uint     UGANttNum       ;
   public      string   UGANrnNapomena  ;
   public DateTime      UGANdate        ;
   public      decimal  UGANrataMoney   ;
   public      int      UGANrokPlac     ;
   public      string   UGANnarudzba    ;

   public List<decimal> DODrataMoneyList;
   public List<decimal> DODrataKoefList ;
   public decimal       UkDODrataMoney  {  get { return DODrataMoneyList.Sum(); } }

 //public string        TipBr           { get { return PTG_Ugovor.GetTipBrFromTtNum(        UGANttNum); } }
   public string        TipBr           { get { return Faktur.Set_TT_And_TtNum     (UGANtt, UGANttNum); } }

   public uint IRA_TtNum;

   public string   KOPxtransNapomena;
   public DateTime KOPxtransDate    ;
   public bool     KOPfakStop       ;
   public bool     HasKOP           ;
   public string   KOPxtransAdduid  ;

   public Xtrans   xtrans_rec       ;

   public string   SomeInfo         ;

   private DateTime datumOd_rucnoPostavljen;
   public  DateTime DatumOd
   {
      get
      {
         if(IsJadnickaRata || HasKOP || IsOtkup)
         {
            return datumOd_rucnoPostavljen;
         }

         DateTime dateOd;

         if(IsNaDanUgovora)
         {
            dateOd = RataDate;
         }
         else
         {
            if(IsFirstRata) dateOd = UGANdate;
            else            dateOd = new DateTime(RataDate.Year, RataDate.Month, 1);
         }

         return dateOd;
      }
      set { datumOd_rucnoPostavljen = value; }
   }

   private DateTime datumDo_rucnoPostavljen;
   public  DateTime DatumDo
   {
      get
      {
         DateTime dateDo;

         if(IsNaDanUgovora || HasKOP || IsOtkup)
         {
            //dateDo = RataDate.AddMonths(1).AddDays(-1);
            //if(dateDo > qweqwe nemas tu datumLastRate niti isLastRata ,... ?)

            return datumDo_rucnoPostavljen;
         }
         else
         {
            if(IsLastRata && UGANdate.Day > 1) dateDo = new DateTime(RataDate.Year, RataDate.Month, UGANdate.Day-1);
            else                               dateDo = RataDate.ThisMonthLastDay();
         }

         return dateDo;
      }
      set { datumDo_rucnoPostavljen = value; }
   }

   public int RataDani
   {
      get
      {
         TimeSpan timeSpan = DatumDo.Date - DatumOd.Date;
         int brojDana = (int)timeSpan.TotalDays + 1;
         return brojDana;
      }
   }

   public ushort Serial;

   public Kupdob TheKupdob     ;
   public uint   TheKupdobCD   ;
   public string TheKUGkupdobTK;

 //public bool   IsForEmail { get { return TheKupdob == null ? false : TheKupdob.Fuse2 == "e-mail"  ; } }
   public string Putnik     { get { return TheKupdob == null ? ""    : TheKupdob.PutName;             } }

   public string Serlot4Rtrans
   {
      get
      {
       //return TipBr + (IsOtkup ? "" : ("/" + RataRbr.ToString()));
         return TipBr + (false   ? "" : ("/" + RataRbr.ToString()));
      }
   }

   public string GrupFakGrID  { get { return TheKupdob.KupdobCD.ToString()                             ; } }
   public string PojedFakGrID { get { return TheKupdob.KupdobCD.ToString() + "-" + UGANttNum.ToString(); } }

   #endregion propertiz

   #region Constructors

   public PTG_Rata()
   {
      DODrataMoneyList = new List<decimal>();
      DODrataKoefList  = new List<decimal>();

      RataSastav = 1;

      KOPxtransNapomena = "";
   }

   public PTG_Rata(int rbr, uint ttNum, string rnNapomena, int rokPlac, string narudzba, DateTime date, decimal money,                                    decimal koef, bool isNaDanUgovora, DateTime dokDate, uint kupdobCD, string KUGkupdobTK) : this()
   {
      RataRbr   = rbr  ;
      RataDate  = date ;
      RataMoney = money;
      RataKoef  = koef ;

      UGANttNum      = ttNum     ;
      UGANrnNapomena = rnNapomena;
      UGANrokPlac    = rokPlac   ;
      UGANnarudzba   = narudzba  ;

      IsLastRata  =
      IsFirstRata = false;

      IsNaDanUgovora = isNaDanUgovora;
    //IsJadnickaRata = isJadnickaRata;

      UGANdate       = dokDate    ;
      TheKupdobCD    = kupdobCD   ;
      TheKUGkupdobTK = KUGkupdobTK;
   }

   public PTG_Rata(int rbr, uint ttNum, string rnNapomena, int rokPlac, string narudzba, DateTime date, decimal money, bool isFirstRata, bool isLastRata, decimal koef, bool isNaDanUgovora, DateTime dokDate, uint kupdobCD, string KUGkupdobTK) : this()
   {
      RataRbr   = rbr  ;
      RataDate  = date ;
      RataMoney = money;
      RataKoef  = koef ;

      UGANttNum      = ttNum     ;
      UGANrnNapomena = rnNapomena;
      UGANrokPlac    = rokPlac   ;
      UGANnarudzba   = narudzba  ;

      IsFirstRata = isFirstRata;
      IsLastRata  = isLastRata ;

      IsNaDanUgovora = isNaDanUgovora;
    //IsJadnickaRata = isJadnickaRata;

      UGANdate = dokDate    ;
      TheKupdobCD    = kupdobCD   ;
      TheKUGkupdobTK = KUGkupdobTK;
   }

   #endregion Constructors

   public override string ToString()
   {
      return Serial + ": " + RataRbr.ToString() + ". rata " + RataDate.ToString(ZXC.VvDateFormat) + " " + RataMoney.ToStringVv() + " (" + (RataKoef*100.00M).ToStringVv() + "%)" + " " + RataSastav;
   }

   public bool IsEqual(PTG_Rata another_rata)
   {
      return

         this.RataRbr   == another_rata.RataRbr  &&
         this.RataDate  == another_rata.RataDate &&
         this.RataMoney == another_rata.RataMoney ;
   }

   public bool IsEqual(Xtrans xtrans_another_rata)
   {
      return

       //this.RataRbr           == xtrans_another_rata.T_serial         &&
         this.KOPxtransNapomena == xtrans_another_rata.T_opis_128       &&
         this.KOPfakStop        == xtrans_another_rata.T_intB.NotZero() &&
         this.RataDate          == xtrans_another_rata.T_date3          &&

         this.DatumOd           == xtrans_another_rata.T_dateOd         &&
         this.DatumDo           == xtrans_another_rata.T_dateDo         &&

         this.RataMoney         == xtrans_another_rata.T_moneyA          ;
   }

   public bool IsFakKindOK(PTG_Ugovor.PTG_FakturiranjeKind ptgFakKind, bool thisRataisOtkup)
   {
      if(ptgFakKind == PTG_Ugovor.PTG_FakturiranjeKind.Otkup)
      {
         if(thisRataisOtkup) return true ;
         else                return false;
      }

      else // NIJE otkup, klasicne rate 
      {
         if(thisRataisOtkup) return false;

         if(ptgFakKind == PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno)
         {
            if(TheKupdob.IsYyy == true) return true ;
            else                        return false;
         }
         if(ptgFakKind == PTG_Ugovor.PTG_FakturiranjeKind.Grupno)
         {
            if(TheKupdob.IsYyy == true) return false;
            else                        return true ;
         }
      }

      return false;
   }
}

public class PTG_Ugovor : Faktur
{
   public PTG_Ugovor(Faktur THE_faktur_rec)
   {
      // PAZI! Nemoj da te ovo zavara. Tu s ponekad misliti da si prenio 
      // PTG_Ugovor-u Faktur-ove currentData, TheEx-a i Transove,
      // a kad tamo, u ovome trenutku ih jos niti nemas pa ih ne mozes ni prenjeti! 
      // Rjeswenje je da ciljano kad ih napunis ponovo zoves 
      // TakeDataFromFaktur
      // ...kao npr: nadji FindAllReferences od TakeDataFromFaktur pas vidit 

      this.TakeDataFromFaktur(THE_faktur_rec);
   }

   public PTG_Ugovor()
   {
   }

   public void TakeDataFromFaktur(Faktur /*THE_faktur_rec*/faktur_rec)
   {
    //Faktur faktur_rec = (Faktur)THE_faktur_rec.CreateNewRecordAndCloneItComplete();
    //Faktur faktur_rec =         THE_faktur_rec                                    ;

      this.      CurrentData = faktur_rec.      CurrentData;
      this.TheEx.CurrentData = faktur_rec.TheEx.CurrentData;
      this.Transes           = faktur_rec.Transes;
      this.Transes2          = faktur_rec.Transes2; // ali fali ti tu ove eksplicitne kao za trans1

      // !!! NOTA BENE + PAZI for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 

      this.Skn_ukKC = faktur_rec.Skn_ukKC;

      for(int i=0; i < Transes.Count; ++i)
      {
         this.Transes[i].SaveTransesWriteMode = faktur_rec.Transes[i].SaveTransesWriteMode;
         this.Transes[i].TmpDecimal           = faktur_rec.Transes[i].TmpDecimal          ;
         this.Transes[i].TmpDecimal2          = faktur_rec.Transes[i].TmpDecimal2         ;
         this.Transes[i].MinusStatus          = faktur_rec.Transes[i].MinusStatus         ;
         this.Transes[i].FakRbr               = faktur_rec.Transes[i].FakRbr              ;
         this.Transes[i].R_kupdobName         = faktur_rec.Transes[i].R_kupdobName        ;
         this.Transes[i].R_grName             = faktur_rec.Transes[i].R_grName            ;
         this.Transes[i].R_utilBool           = faktur_rec.Transes[i].R_utilBool          ;
         this.Transes[i].Tkn_cij              = faktur_rec.Transes[i].Tkn_cij             ;
         this.Transes[i].Rkn_KC               = faktur_rec.Transes[i].Rkn_KC              ;
         this.Transes[i].Rkn_rbt1             = faktur_rec.Transes[i].Rkn_rbt1            ;
         this.Transes[i].Rkn_CIJ_KCR          = faktur_rec.Transes[i].Rkn_CIJ_KCR         ;
         this.Transes[i].Rkn_KCR              = faktur_rec.Transes[i].Rkn_KCR             ;
         this.Transes[i].Rkn_CIJ_KCRP         = faktur_rec.Transes[i].Rkn_CIJ_KCRP        ;
         this.Transes[i].Rkn_KCRP             = faktur_rec.Transes[i].Rkn_KCRP            ;
         this.Transes[i].R_utilString         = faktur_rec.Transes[i].R_utilString        ;

         this.Transes[i].CalcTransResults(faktur_rec);
      }
   }

   #region PTG 

   public PTG_OtplatniPlan TheOtplatniPlan { get; set; }

   private bool GetIndexedBool_From_PdvNum(int positionFromRight)
   {
      if(ZXC.IsPCTOGO == false && this.TT != TT_UGN && this.TT != TT_AUN) return false; // !!! PROVJERI 

      if(PdvNum.IsZero())
      {
         if(ZXC.IsPCTOGOdomena == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "PdvNum is zero");
         }
         return false;
      }

      string pdvNumStr = this.PdvNum.ToString();

      if(positionFromRight.IsZero() || positionFromRight > pdvNumStr.Length)
      {
         if(ZXC.IsPCTOGOdomena == true)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Position From Left ({0})\n\r\n\r NE POSTOJI", positionFromRight);
         }
         return false;
      }

      // "1000000101"
      int positionFromLeft = 10 - positionFromRight;

    //return pdvNumStr[positionFromRight - 1] == '1';
      return pdvNumStr[positionFromLeft     ] == '1';
   }

   public bool PTG_b1_povOprBezPen { get { return GetIndexedBool_From_PdvNum(1); } } // 1. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b2_podnajam     { get { return GetIndexedBool_From_PdvNum(2); } } // 2. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b3_osigOpreme   { get { return GetIndexedBool_From_PdvNum(3); } } // 3. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b4_novaOprema   { get { return GetIndexedBool_From_PdvNum(4); } } // 4. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b5_potpisan     { get { return GetIndexedBool_From_PdvNum(5); } } // 5. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b6_vracenaOprema{ get { return GetIndexedBool_From_PdvNum(6); } } // 6. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b7_otkup        { get { return GetIndexedBool_From_PdvNum(7); } } // 7. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b8_ispisnoRj    { get { return GetIndexedBool_From_PdvNum(8); } } // 8. pozicija (gledano s desna, NOT index (zero) based addressing) 
   public bool PTG_b9_isJednokratPl{ get { return GetIndexedBool_From_PdvNum(9); } } // 9. pozicija (gledano s desna, NOT index (zero) based addressing) 

   public string PTG_DanFakturiranjaString//_opciAvalue
   { get { return this.TheEx.currentData._opciAvalue; } set { this.TheEx.currentData._opciAvalue = value; } }

   public ZXC.PTG_DanFakturiranjaEnum PTG_DanFakturiranja //opciAvalue
   {
      get
      {
         if(PTG_DanFakturiranjaString.IsEmpty()) return ZXC.PTG_DanFakturiranjaEnum.PrviDanMjeseca;
         else                                    return (ZXC.PTG_DanFakturiranjaEnum)Enum.Parse(typeof(ZXC.PTG_DanFakturiranjaEnum), PTG_DanFakturiranjaString, true);
      }
   }

   public string PTG_VrstaNajma//_opciBlabel 
   {
      get { return this.TheEx.currentData._opciBlabel; } set { this.TheEx.currentData._opciBlabel = value; }
   }

   public ZXC.PTG_VrstaNajmaEnum PTG_VrstaNajmaAsEnum
   {
      get
      {
         if(PTG_VrstaNajma.IsEmpty()) return ZXC.PTG_VrstaNajmaEnum.Mjesecni;
         else return (ZXC.PTG_VrstaNajmaEnum)Enum.Parse(typeof(ZXC.PTG_VrstaNajmaEnum), PTG_VrstaNajma, true);
      }
   }

   public string PTG_NajamNaRok//_opciBvalue
   { get { return this.TheEx.currentData._opciBvalue; } set { this.TheEx.currentData._opciBvalue = value; } }

   public ZXC.PTG_NajamNaRokEnum PTG_NajamNaRokAsEnum
   {
      get
      {
         if(PTG_NajamNaRok.IsEmpty()) return ZXC.PTG_NajamNaRokEnum.EMPTY;
         else                         return (ZXC.PTG_NajamNaRokEnum)Enum.Parse(typeof(ZXC.PTG_NajamNaRokEnum), PTG_NajamNaRok, true);
      }
   }

   public ZXC.PTG_MjestoIsporukeEnum PTG_MjestoIsporuke  { get { return (ZXC.PTG_MjestoIsporukeEnum)this.TheEx.currentData._pdvZPkind; } set { this.TheEx.currentData._pdvZPkind = (ushort)value; } }

   public int PTG_BrojRata      { get { return this.TheEx.currentData._rokPonude;   } set { this.TheEx.currentData._rokPonude   = value; } }
   public int PTG_BrojNovihRata { get { return this.TheEx.currentData._rokIsporuke; } set { this.TheEx.currentData._rokIsporuke = value; } }

   public uint PTG_TrajanjeUg   { get { return this.TheEx.currentData._cjenTTnum;   } set { this.TheEx.currentData._cjenTTnum   = value; } }
   public int  PTG_OdgodaPl     { get { return this.TheEx.currentData._rokPlac;     } set { this.TheEx.currentData._rokPlac     = value; } }

   public DateTime PTG_DatUgovora     { get { return this.      currentData._dokDate;    } set { this.      currentData._dokDate    = value; } }
   public DateTime PTG_DatDostave     { get { return this.      currentData._dokDate2;   } set { this.      currentData._dokDate2   = value; } }
   public DateTime PTG_DatSkidSaSklad { get { return this.TheEx.currentData._skladDate;  } set { this.TheEx.currentData._skladDate  = value; } }
   public DateTime PTG_DatPrvogRn     { get { return this.TheEx.currentData._dospDate;   } set { this.TheEx.currentData._dospDate   = value; } }
   public DateTime PTG_DatZadnjegRn   { get { return this.TheEx.currentData._ponudDate;  } set { this.TheEx.currentData._ponudDate  = value; } }
   public DateTime PTG_datIstekaUg    { get { return this.TheEx.currentData._rokIspDate; } set { this.TheEx.currentData._rokIspDate = value; } }
   public DateTime PTG_DatPovrataOpr  { get { return this.TheEx.currentData._dateX;      } set { this.TheEx.currentData._dateX      = value; } }

   public decimal PTG_RabatKupcaPosto { get { return this.TheEx.currentData._somePercent; } set { this.TheEx.currentData._somePercent = value; } }
   public decimal PTG_OtkupPosto      { get { return this.currentData      ._decimal01;   } set { this.      currentData._decimal01   = value; } }
   public decimal PTG_PosredProvPosto { get { return this.currentData      ._decimal02;   } set { this.      currentData._decimal02   = value; } }

   public string PTG_Narudzba       { get { return this.TheEx.currentData._opciAlabel;   } set { this.TheEx.currentData._opciAlabel   = value; } }
   public string PTG_Lokacija       { get { return this.TheEx.currentData._dostAddr;     } set { this.TheEx.currentData._dostAddr     = value; } }
   public string PTG_TekstZaRn      { get { return this.TheEx.currentData._napomena2;    } set { this.TheEx.currentData._napomena2    = value; } }
   public string PTG_PTG_KDCnaziv   { get { return this.TheEx.currentData._odgvPersName; } set { this.TheEx.currentData._odgvPersName = value; } }

   public string PTG_Napomena       { get { return this.currentData._opis;                } set { this.currentData._opis              = value; } }

   public string PTG_OpaskaServisa  { get { return this.currentData._napomena;            } set { this.currentData._napomena          = value; } }


   // Korisnik je KupDob i sve sto ide sa njime, Valuta je Valuta, Napomena je Napomena ...
   // Da li je Poslovnica = Poslovna Jedinica ????
   // Kontakt osoba - birat ce se od nekuda ali da li su onda ostala polja readOnly 

   public uint PTG_KUGnum  { get { return this.currentData._v1_ttNum; } set { this.currentData._v1_ttNum = value; } }
   public uint PTG_UGANnum { get { return this.currentData._v2_ttNum; } set { this.currentData._v2_ttNum = value; } }

   public uint PTG_DOKOnum
   {
      get
      {
         return ZXC.GetDesnoOdZareza_asUint(this.currentData._ttNum, 1000);
      }
   }
   //public uint PTG_R_iznosOtkupa
   //{
   //}

   public string PTG_osigPlacanja { get { return this.currentData._osobaX; } set { this.currentData._osobaX = value; } }

   public bool   PTG_HasOtkup         { get { return this.PTG_OtkupPosto.NotZero() && this.PTG_b7_otkup == true;         } }

   public bool   PTG_isXXX        { get { return this.TheEx.currentData._isNpCash2; } set { this.TheEx.currentData._isNpCash2 = value; } }

   #endregion PTG 

   public PTG_OtplatniPlan LoadOtplatniPlan(XSqlConnection conn)
   {
      this.TheOtplatniPlan = new PTG_OtplatniPlan(conn, this);

      return this.TheOtplatniPlan;
   }

   internal uint Count_DODorKOPfakturList(XSqlConnection conn, string wantedTT)
   {
      uint wantedKUGttNum  = this.V1_ttNum;
      uint wantedUGANttNum = this.V2_ttNum;

      List<VvSqlFilterMember> filterMembers;

      if(wantedTT == Faktur.TT_DIZ) filterMembers = PTG_OtplatniPlan.GetFilterMembers_DOD_FakturList(          wantedKUGttNum, wantedUGANttNum, true);
      else                          filterMembers = PTG_OtplatniPlan.GetFilterMembers_KOPxtransList (wantedTT, wantedKUGttNum, wantedUGANttNum      );

      int? count = VvDaoBase.CountRecords(conn, filterMembers);

      return (count == null ? 0 : (uint)count);
   }

   public enum PTG_FakturiranjeKind { Grupno, Pojedinacno, Otkup, UslugeA1grupno }
   public enum PTG_SlanjeFakKind    { eMail, Knjigovodstvo, Print, Pdf, eRacun }

   internal static List<PTG_Rata> Get_PTG_Rata_List_ZaFakturiranje(XSqlConnection conn, DateTime dateDO, PTG_FakturiranjeKind fakturiranjeKind)
   {
      List<PTG_Rata> the_Rata_List_ZaFakturiranje = new List<PTG_Rata>();

      List<PTG_Ugovor> UGANList = new List<PTG_Ugovor>();
      List<PTG_Ugovor> UGANList_UGN;
      List<PTG_Ugovor> UGANList_AUN;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>();

      System.Data.DataRowCollection FakSch   = ZXC.FakturDao.TheSchemaTable.Rows;
      FakturDao.FakturCI            FakCI    = ZXC.FakturDao.CI;

      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt]     , ZXC.FM_OR_Enum.OPEN_OR , false, "TT1"    , Faktur.TT_UGN, "", "", "  = ", ""));
      filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.tt]     , ZXC.FM_OR_Enum.CLOSE_OR, false, "TT2"    , Faktur.TT_AUN, "", "", "  = ", ""));
    //filterMembers.Add(new VvSqlFilterMember(FakSch[FakCI.dokDate],                          false, "dokdate", dateDO       , "", "", " <= ", ""));

      VvDaoBase.LoadGenericVvDataRecordList<PTG_Ugovor>(conn, UGANList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      UGANList.ForEach(ugan => ugan.VvDao.LoadTranses(conn, ugan, false));

      UGANList.ForEach(ugan => ugan.LoadOtplatniPlan(conn));

      UGANList.ForEach(ugan => ugan.TheOtplatniPlan.GetAllRate_IRA_TtNum(conn));

      UGANList_UGN = UGANList.Where(ugan => ugan.TT == Faktur.TT_UGN).ToList();
      UGANList_AUN = UGANList.Where(ugan => ugan.TT == Faktur.TT_AUN).ToList();

      UGANList_UGN.ForEach(ugan => ugan.TheOtplatniPlan.UGAN_NeFakturirane_RateList.ForEach(rata => rata.TheKupdob = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == rata.TheKupdobCD   )));
      UGANList_AUN.ForEach(ugan => ugan.TheOtplatniPlan.UGAN_NeFakturirane_RateList.ForEach(rata => rata.TheKupdob = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.Ticker   == rata.TheKUGkupdobTK)));

      UGANList.ForEach(ugan => the_Rata_List_ZaFakturiranje.AddRange(ugan.TheOtplatniPlan.UGAN_NeFakturirane_RateList

         .Where(rata => rata.RataDate <= dateDO && rata.IsFakKindOK(fakturiranjeKind, rata.IsOtkup))));

      return the_Rata_List_ZaFakturiranje.OrderBy(rata => rata.TheKupdob.Naziv).ThenBy(rata => rata.UGANttNum).ThenBy(rata => rata.Serial).ToList();
   }

   // treba tu jos vidit da li da ovo bude tuten ili ga dat static pa mu faktur_rec slati kao parametaar 
   internal decimal GetOtkupRataMoney(XSqlConnection conn)
   {
      decimal UGAN_artikliUnajmuMoney = this.Transes                              .Sum(rtr => rtr.R_KCRP);
      decimal DOD_artikliUnajmuMoney  = RtransDao.Get_DOD_RtransList(conn, this).Sum(rtr => rtr.R_KCRP);
      decimal THE_artikliUnajmuMoney  = UGAN_artikliUnajmuMoney + DOD_artikliUnajmuMoney;

      decimal otkupRataMoney = ZXC.VvGet_25_of_100(THE_artikliUnajmuMoney, PTG_OtkupPosto); 

      return otkupRataMoney;
   }

   internal decimal GetOtkupArtiklMoney(decimal artiklRataMoney)
   {
      if(PTG_HasOtkup) return ZXC.VvGet_25_of_100(artiklRataMoney, PTG_OtkupPosto);
      else         return                                                0.00M;
   }

}

