using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using CrystalDecisions.Windows.Forms;
using Vektor.Reports.FIZ;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                   MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader  = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlDbType      = MySql.Data.MySqlClient.MySqlDbType;
using XSqlParameter   = MySql.Data.MySqlClient.MySqlParameter;
using XSqlException   = MySql.Data.MySqlClient.MySqlException;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
#endif

public /*struct*/ class OTPdata
{
   public string  ProjektCD    { get; set; } // 1 
   public string  Investitor   { get; set; } // 2 
   public decimal PostDovr     { get; set; } // 3 
   public decimal PlanCijena   { get; set; } // 4 
   public decimal PUT_dug      { get; set; } // 5 ProizvtijekuPR 
   public decimal PUT_pot      { get; set; } // 6 ProizvedenoPR  
   public decimal RD_razd      { get; set; } // 7 DirektniTr     

   //public decimal R_RD_year    { get { return PUT_dug    + RD_razd               ; } }
   //public decimal R_NewTr_razd { get { return RD_razd    + R_TheOTP              ; } }
   //public decimal R_NewTr_year { get { return PUT_pot    + R_NewTr_razd          ; } }
   //public decimal R_OTP_year   { get { return PostDovr   * R_NewTr_year / 100.00M; } }
   //public decimal R_OTP_razd   { get { return R_OTP_year - PUT_pot               ; } }

   //28.03.2017.
   public decimal R_TheOTP_Dovrs   { get { return  ZXC.VvGet_25_of_100(R_TheOTP, PostDovr)     ; } } // Y IndirektniTr po StDovrs 
   
   public decimal RD_razdDovrs     { get { return  ZXC.VvGet_25_of_100(RD_razd, PostDovr)      ; } } // X DirektniTr po StDovrs 
   public decimal R_RD_year        { get { return  PUT_dug  +  RD_razd                         ; } } //  8 (5+7) RasporedeniTr    
   public decimal R_NewTr_razd     { get { return  RD_razd  +  R_TheOTP                        ; } } // 11 UkTroskoviUR  (7+9)    
   public decimal R_OTP_razd       { get { return  (R_NewTr_year * PostDovr/100.00M) - PUT_pot ; } } // 12 ProizvedenoUR (13*3-6) 
   public decimal R_NewTr_year     { get { return  PUT_dug  +  R_NewTr_razd                    ; } } // 13 UkTroskoviKR  (5+11)   
   public decimal R_OTP_year       { get { return  PUT_pot  +  R_OTP_razd                      ; } } // 14 ProizvedenoKR (6+12)   

   // 10.04.2017: !!! BIG NEWS !!! 
   public decimal R_tamaraTodo       { get { return  R_TheOTP_Dovrs +  RD_razdDovrs; } } // y+x TamaraTODO (y+x)   


   // 06.03.2017: 
 //public decimal R_RI_razd        { get { return R_OTP_razd - RD_razd     ; } } // Za zadnji redak naloga 'Indirektni Troskovi' (kada dir. idu na konto skladista) 
   public decimal R_RI_razd        { get { return R_OTP_razd - RD_razdDovrs; } } // Za zadnji redak naloga 'Indirektni Troskovi' (kada dir. idu na konto skladista) 

   public decimal R_TheOTP   { get; set; } //  9 NovoRspTr (8*koef)
   public decimal R_PostRasp { get; set; } // 10 Udio %            

   public decimal R_Rezervacija_1 { get; set; }
   public decimal R_Rezervacija_2 { get; set; }

   public bool    R_IsGotovRNMnoPIP    { get; set; }

   public OTPdata(string _ProjektCD, string _Investitor, decimal _PostDovr, decimal _PlanCijena, decimal _PUT_dug, decimal _PUT_pot, decimal _RD_razd, bool _isGotovRNMnoPIP) /*: this()*/
   {
      this.ProjektCD    = _ProjektCD ;
      this.Investitor   = _Investitor;
      this.PostDovr     = _PostDovr  ;
      this.PlanCijena   = _PlanCijena;
      this.PUT_dug      = _PUT_dug   ;
      this.PUT_pot      = _PUT_pot   ;
      this.RD_razd      = _RD_razd   ;

      this.R_IsGotovRNMnoPIP = _isGotovRNMnoPIP;
   }

   // IATP additions: 
   public decimal RI_razd     { get; set; } // prethodni (OTP-om) rasporedjeni INDIREKTNI troskovi 
   public decimal R_TheREZIJE { get; set; }
   public decimal R_TheAMORT  { get; set; }
   public decimal R_ukKrajR   { get { return RD_razd    + RI_razd + R_TheREZIJE + R_TheAMORT; } } // uk. tr. na kraju razdoblja  
   public decimal R_ukIATP    { get { return R_ukKrajR  + /*R_*/PUT_PS                      ; } } // sveukupni trosak proizvodnje = troskovi ovoga razdoblja sa sadrzanim RR i AM + troskovi DO tog razdoblja
   public decimal R_razlikaPL { get { return PlanCijena - R_ukIATP                          ; } } // RUC: razlika Planirane (pa valjda i naplacene cijene) i IATP-a
 //public decimal R_PUT_PS    { get { return PUT_dug - RD_razd - RI_razd                    ; } } // Pocetno stanje PUT odnosno troskovi nastali DO zadanog razdoblja
   public decimal PUT_PS      { get; set; }

   //public OTPdata(string _ProjektCD, string _Investitor, decimal _PlanCijena, decimal _PUT_PS, decimal _RD_razd, decimal _RI_razd, decimal _R_TheREZIJE, decimal _R_TheAMORT) /*: this()*/
   //{
   //   this.ProjektCD   = _ProjektCD  ;
   //   this.Investitor  = _Investitor ;
   //   this.PlanCijena  = _PlanCijena ;
   // //this.PUT_dug     = _PUT_dug    ;
   //   this.PUT_PS      = _PUT_PS     ;
   //   this.RD_razd     = _RD_razd    ;
   //   this.RI_razd     = _RI_razd    ;
   //   this.R_TheREZIJE = _R_TheREZIJE;
   //   this.R_TheAMORT  = _R_TheAMORT ;
   //}

   public OTPdata(string _ProjektCD, string _Investitor, decimal _PlanCijena, decimal _PUT_PS, decimal _RD_razd, decimal _RI_razd, decimal _R_TheREZIJE, decimal _R_TheAMORT, decimal rez1, decimal rez2) /*: this()*/
   {
      this.ProjektCD   = _ProjektCD  ;
      this.Investitor  = _Investitor ;
      this.PlanCijena  = _PlanCijena ;
    //this.PUT_dug     = _PUT_dug    ;
      this.PUT_PS      = _PUT_PS     ;
      this.RD_razd     = _RD_razd    ;
      this.RI_razd     = _RI_razd    ;
      this.R_TheREZIJE = _R_TheREZIJE;
      this.R_TheAMORT  = _R_TheAMORT ;

      this.R_Rezervacija_1 = rez1;
      this.R_Rezervacija_2 = rez2;
   }

   //public override string ToString()
   //{
   //   return "(" + TheStr1 + ")" + "(" + TheStr2 + ")" + TheDecimal.ToStringVv() + "/" + TheBool;
   //}

   //internal static void AnalizeThisRnpOtpData(List<OTPdata> theOTPdataList, Faktur faktur_rec)
   //{
   //   // qweqwe 
   //}
}

public partial class ObrProDLG : Form// VvDialog{
{
   #region OTP data

   public List<OTPdata> TheOtpList;

   // 03.03.2017: 
   internal CrystalDecisions.CrystalReports.Engine.ReportDocument theReportDocument;

   #endregion OTP struct

   #region Buttons_clik

   public void UcitajButton_Click(object sender, EventArgs e)
   {
      if(TheUC.GetKtoShemaDscFields()) return; // GetKtoShemaDscFields() ujedno i validira 

      Cursor.Current = Cursors.WaitCursor;

      bool loadOK =

      FtransDao.GetObracunTroskovaProizvodnjeListe(
         theDbConnection     , 
         TheUC.Fld_DatumOd   , 
         TheUC.Fld_DatumDo   , 
         TheUC.KSD           ,
         out nrspIndirFtrList, 
         out raspDirktFtrList, 
         out proUtijekFtrList, 
         out rnpFakturList   );

      Cursor.Current = Cursors.Default;

      EnableDisable_tsBtn_Nalog(loadOK);

      List<string> suspendedRnpList = CheckSuspendedRnp(TheUC.KSD.Dsc_otp_skipSatus,
                                                        raspDirktFtrList           ,
                                                        proUtijekFtrList           ,
                                                        rnpFakturList              );

      if(suspendedRnpList.Count.NotZero())
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         foreach(string suspendedRnp in suspendedRnpList)
         {
            sb.Append(suspendedRnp + "\n");
         }

         ZXC.aim_emsg(MessageBoxIcon.Error, "U zadanom razdoblju postoje direktni troškovi koji se odnose na suspendirane (pauzirane, 'na čekanju') radne naloge.\n\n" +
                                            "Promijenite status radnog naloga ili preknjižite troškove sa suspendiranih na otvorene radne naloge.\n\nRadni nalozi:\n\n" +
                                            sb);
         return; // !!! 
      }

      PutLoadedLists(); // VOILA! 

      if(loadOK == false) ZXC.aim_emsg(MessageBoxIcon.Information, "Za zadane kriterije nema se što ili nema se na što raspoređivati.\n\nBroj NERASPOREĐENIH knjiženja: {0}\n\nBroj RADNIH NALOGA: {1}", nrspIndirFtrList.Count, rnpFakturList.Count);

   }

   private List<string> CheckSuspendedRnp(string suspendedTT, List<Ftrans> raspDirktFtrList, List<Ftrans> proUtijekFtrList, List<Faktur> rnpFakturList)
   {
      List<string> suspendedRnpFakturList = new List<string>();
      Faktur faktur_rec;

      // kreiraj listu eventualnih pojava suspendiranih u raspDirktFtrList 
      foreach(Ftrans ftrans_rec in raspDirktFtrList)
      {
         faktur_rec = rnpFakturList.SingleOrDefault(fak => fak.TT_And_TtNum == ftrans_rec.T_projektCD);

         if(faktur_rec != null && faktur_rec.StatusCD == suspendedTT)
         {
            suspendedRnpFakturList.Add(faktur_rec.TT_And_TtNum);
         }
      }

      if(suspendedRnpFakturList.Count.IsZero()) // ovo radi samo ako nema prethodnog problema (pojave suspendiranih u raspDirktFtrList) 
      {
         // izbaci suspendirane iz proUtijekFtrList 
         for(int i = 0; i < proUtijekFtrList.Count; ++i)
         {
            faktur_rec = rnpFakturList.SingleOrDefault(fak => fak.TT_And_TtNum == proUtijekFtrList[i].T_projektCD);

            if(faktur_rec != null && faktur_rec.StatusCD == suspendedTT)
            {
               proUtijekFtrList.RemoveAt(i--);
               rnpFakturList   .Remove  (faktur_rec);
            }
         }
      }

      return suspendedRnpFakturList.Distinct().ToList();
   }

   private void PutLoadedLists()
   {
      TheUC.Fld_A_IznosIndirTr = nrspIndirFtrList.Sum(ftr => ftr.R_DugMinusPot);
      TheUC.Fld_DirektniTr     = raspDirktFtrList.Sum(ftr => ftr.R_DugMinusPot) + 
                                 proUtijekFtrList.Sum(ftr => ftr.T_dug        );
      TheUC.Fld_KoefDir        = ZXC.DivSafe(TheUC.Fld_A_IznosIndirTr, TheUC.Fld_DirektniTr);

      TheOtpList = GetOtpList(rnpFakturList, raspDirktFtrList, proUtijekFtrList);

      TheUC.PutDgvFields1(TheOtpList      );

      TheUC.PutDgvFields2(rnpFakturList   );
      TheUC.PutDgvFields3(nrspIndirFtrList);
      TheUC.PutDgvFields4(raspDirktFtrList);
      TheUC.PutDgvFields5(proUtijekFtrList);

      TheUC.SetSifrarAndAutocomplete<Kplan> (null, VvSQL.SorterType.Konto);
      TheUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name );

      TheUC.PutDgvFields6(GetKontaSume(nrspIndirFtrList));

      CheckRaspored();
   }

   private void CheckRaspored()
   {
      if(TheUC.Fld_IsDirekt)
      {
         decimal left, right;

         left  = TheUC.Fld_A_IznosIndirTr;
         right = TheOtpList.Sum(otp => otp.R_TheOTP);

         if(ZXC.AlmostEqual(left, right, 0.02M) == false) 
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje!\n\n Suma neraspoređenih indirektnih troškova {0}\n\nnije jednaka sumi raspoređenih {1}",
               left, right);
      }
      else // TODO: !!! ? 
      {
      }
   }

   private List<ZXC.VvUtilDataPackage> GetKontaSume(List<Ftrans> nrspIndirFtrList)
   {
      List<ZXC.VvUtilDataPackage> kontaSumeList = new List<ZXC.VvUtilDataPackage>();

      kontaSumeList =
         nrspIndirFtrList
         .GroupBy(ftr => ftr.T_konto)
         .Select(grp => new ZXC.VvUtilDataPackage(grp.Key, grp.Sum(ftr => ftr.T_dug), grp.Sum(ftr => ftr.T_pot)))
         .OrderBy(grp => grp.TheStr1)
         .ToList();         

      return kontaSumeList;
   }

   private List<OTPdata> GetOtpList(List<Faktur> rnpFakturList, List<Ftrans> raspDirktFtrList, List<Ftrans> proUtijekFtrList)
   {
      TheOtpList = new List<OTPdata>(rnpFakturList.Count);

      decimal PUT_dug, PUT_pot, RD_razd;
      
      bool isGotovRNMnoPIP = false; // 10.04.2017: !!! BIG NEWS !!! 

    //foreach(Faktur faktur_rec[i] in rnpFakturList)
      for(int i = 0; i < rnpFakturList.Count; ++i)
      {
         PUT_dug = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_dug        );
         PUT_pot = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_pot        );
         RD_razd = raspDirktFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.R_DugMinusPot);

         // 29.03.2017: 
       //if((PUT_dug - PUT_pot).IsZero () &&
       //   (PUT_dug + PUT_pot).NotZero() &&
       //   (RD_razd          ).IsZero ())
         if((PUT_dug - PUT_pot).AlmostZero(0.50M) &&
            (PUT_dug + PUT_pot).NotZero() &&
            (RD_razd          ).IsZero ())
         {
            rnpFakturList.RemoveAt(i--);
         }
         else
         {
            if(ZXC.IsRNMnotRNP) // Metaflex 
            {
               ZXC.FakturDao.LoadGenericTransesList<Rtrans>(theDbConnection, rnpFakturList[i].Transes, rnpFakturList[i].RecID, /*isArhiva*/ false);

               List<Rtrans> realizacijaRtransList = RtransDao.GetRtransList_ForProjektCD(theDbConnection, rnpFakturList[i]./*ProjektCD*/TT_And_TtNum, "t_tt DESC, " + Rtrans.artiklOrderBy_ASC);

               // 10.04.2017: !!! BIG NEWS !!! 
               isGotovRNMnoPIP = Get_isGotovRNMnoPIP(rnpFakturList[i], realizacijaRtransList);

               realizacijaRtransList.RemoveAll(rtr => rtr.R_utilBool == true); // izbaci proslogodisnje rtranse 

             //rnpFakturList[i].SomePercent = rnpFakturList[i].StatusCD == "R" ? 100M :
             //                               100M * RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list(true , rnpFakturList[i], realizacijaRtransList, rnpFakturList[i].Decimal02       );
               rnpFakturList[i].SomePercent = 100M * RtransDao.Get_KoefDovrsenosti_From_RNM_PPR_PIP_list(false, rnpFakturList[i], realizacijaRtransList, rnpFakturList[i].Decimal02, false); // za Suk      

            }
            else // Frigoterm 
            {
               if(rnpFakturList[i].SomePercent.IsZero()) rnpFakturList[i].SomePercent = 100.00M;
            }

            TheOtpList.Add(
                  new OTPdata(rnpFakturList[i].TT_And_TtNum, 
                              rnpFakturList[i].KupdobName,
                            //rnpFakturList[i].SomePercent.NotZero() ? rnpFakturList[i].SomePercent : 100.00M,
                              rnpFakturList[i].SomePercent                                                   , 
                              rnpFakturList[i].SomeMoney, 
                              PUT_dug, 
                              PUT_pot, 
                              RD_razd,
                              isGotovRNMnoPIP)
            );
         }

      } // for(int i = 0; i < rnpFakturList.Count; ++i)

      return TheOtpList;
   }

   // 10.04.2017: !!! BIG NEWS !!! 
   private bool Get_isGotovRNMnoPIP(Faktur faktur, List<Rtrans> realizacijaRtransList)
   {
      bool isGotov;
      bool isNoPIP;

      isGotov = faktur.StatusCD == "R"; // RNM ima oznaku StatusCD-a 'R' ... znaci 100% dovrsen. 

      isNoPIP = realizacijaRtransList.Any(rtr => rtr.T_TT == Faktur.TT_PIP) == false;

      return isGotov && isNoPIP;
   }
   
   private void AbortButton_Click(object sender, EventArgs e)
   {
      this.Close();

      //ZXC.TheVvForm.EscapeAction_OnClick(this, EventArgs.Empty);
   }

   private void AutoSetNalogOTP(
      ref ushort line, 
      Nalog    nalog_rec,       
      string   t_projektCD,
      string   t_konto,
      string   t_opis,
      decimal  t_dug,
      decimal  t_pot)
   {
      if((t_dug + t_pot).IsZero()) return;

      NalogDao.AutoSetNalog(theDbConnection, ref line, nalog_rec.DokDate, nalog_rec.TT, nalog_rec.Napomena, t_projektCD, t_konto, t_opis, t_dug, t_pot);
   }

   private void NaNalogButton_Click(object sender, EventArgs e)
   {
      #region Init stuff

      TheUC.GetKtoShemaDscFields();

      ushort line = 0;
      Nalog  nalog_rec  = new Nalog ();
      Ftrans ftrans_rec = new Ftrans();

      decimal dug, pot, sumDug, sumPot, kontraSum;
      string konto;
      string opis  = "Auto OTP";

      // 28.03.2017:
    //nalog_rec.TT       = "TM";
      if(TheUC.KSD.Dsc_otp_IsSkladGotProizv) // Metaflex 
      {
         nalog_rec.TT = "OTP";
      }
      else // Frigoterm 
      {
         nalog_rec.TT = "OP";
      }

      nalog_rec.DokDate  = TheUC.Fld_DatumDo;
      nalog_rec.Napomena = "Raspored indirektnih troškova i obračun proizvodnje za razdoblje " + TheUC.Fld_DatumOd.ToString(ZXC.VvDateDdMmFormat) + " - " + TheUC.Fld_DatumDo.ToString(ZXC.VvDateFormat);

      // 08.09.2017: 
      bool isMetaflex  =  ZXC.IsRNMnotRNP;
      bool isFrigoterm = !ZXC.IsRNMnotRNP;

      #endregion Init stuff

      if(TheUC.Fld_A_IznosIndirTr.IsZero()) // Nema se sto za rasporedivati od INDIREKTNIH (ili ih nema za razdoblje, ili se neko zaje. pa radi uduplo)
      {
         DialogResult result = MessageBox.Show("U ovome razdoblju nema neraspoređenih indirektnih troškova.\n\nDa li zaista zelite kreirati nalog?", "Potvrdite NALOG?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         if(result != DialogResult.Yes) return;
      }

      #region Anuliranje, minusiranje sintetike nerasp. indir. troskova

      // 1. === Za svaki redak 6 Tab-a minusirati konto retka DUG
      
      
      List<ZXC.VvUtilDataPackage> kontaSumeList = GetKontaSume(nrspIndirFtrList);

      foreach(ZXC.VvUtilDataPackage kontoSuma in kontaSumeList)
      {
         konto = kontoSuma.TheStr1;
         dug   = -(kontoSuma.TheDecimal - kontoSuma.TheDecimal2);
         pot   = 0.00M;

         AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);
      }

      #endregion Anuliranje, minusiranje sintetike nerasp. indir. troskova

      #region Razrada foreach rnp: foreach konto indirekt

      // 2. === Za svaki sint minus tocke 1. analiticki po RNP a po koeficijentu 'R_PostRasp'.. pazi 'isti' konti ili 'zamjena zadnje znamenka' DUG 
      sumDug = sumPot = 0.00M;
      ZXC.VvUtilDataPackage tmpUDP;
      foreach(OTPdata otp_rec in TheOtpList) 
      {
       //foreach(ZXC.VvUtilDataPackage kontoSuma in kontaSumeList)
         for(int i = 0; i < kontaSumeList.Count; ++i )
         {
            konto = TheUC.KSD.Dsc_otp_IsKtoEndSame ? kontaSumeList[i].TheStr1 : kontaSumeList[i].TheStr1.Substring(0, kontaSumeList[i].TheStr1.Length - 1) + TheUC.KSD.Dsc_otp_ktoIndirEnd;
            dug = ((kontaSumeList[i].TheDecimal - kontaSumeList[i].TheDecimal2) * otp_rec.R_PostRasp / 100.00M).Ron2();
            pot = 0.00M;
            sumDug += dug; sumPot += pot;

            if(otp_rec.ProjektCD == TheOtpList.Last().ProjektCD &&
               kontaSumeList[i].TheStr1 == kontaSumeList.Last().TheStr1) // last iteration pass 
            {
               kontraSum = kontaSumeList.Sum(ks => ks.TheDecimal - ks.TheDecimal2);

               if(sumDug != kontraSum) // korekcija zaokruzivanja po cijelom nalogu 
               {
                  dug += kontraSum - sumDug;
               }
            }

            AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);

            // 06.03.2017: za korekciju zaokruzivanja po kontu 
            tmpUDP = kontaSumeList[i];
            tmpUDP.TheDecimal4 += dug;
            kontaSumeList[i] = tmpUDP;
            //To modify the struct, first assign it to a local variable, modify the variable, then assign the variable back to the item in the collection.
            //
            //List<myStruct> list = {…};
            //MyStruct ms = list[0];
            //ms.Name = "MyStruct42";
            //list[0] = ms;  

         }
      }

      // 06.03.2017: korekcija zaokruzivanja po kontu ___ START ___ 
      foreach(ZXC.VvUtilDataPackage kontoSuma in kontaSumeList)
      {
         konto = kontoSuma.TheStr1;
         dug = (kontoSuma.TheDecimal - kontoSuma.TheDecimal2 - kontoSuma.TheDecimal4);
         pot   = 0.00M;

         if(dug.NotZero()) // u 'dug' se nalazi RAZLIKA za korekciju zaokruzivanja po kontu 
         {
            AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);
         }
      }
      // 06.03.2017: korekcija zaokruzivanja po kontu ___  END  ___ 

      #endregion Razrada foreach rnp: foreach konto indirekt

      #region Phase 3

      // 3. === za savki RNP iznos R_NewTr_razd na kto KSD.Dsc_otp_ktoObrade na DUG
      //        a onda to sumarno na KSD.Dsc_otp_ktoPrerspTrsk POT

      sumDug = sumPot = 0.00M;
      foreach(OTPdata otp_rec in TheOtpList)
      {
         konto = TheUC.KSD.Dsc_otp_ktoObrade;
         dug   = otp_rec.R_NewTr_razd;
         pot   = 0.00M;
         sumDug += dug; sumPot += pot;

         if(otp_rec.ProjektCD == TheOtpList.Last().ProjektCD) // last iteration pass 
         {
            kontraSum = TheOtpList.Sum(otp => otp.R_NewTr_razd);
                         
            if(sumDug != kontraSum) // korekcija zaokruzivanja 
            {
               dug += kontraSum - sumDug; 
            }
         }

         AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);
      }

      konto = TheUC.KSD.Dsc_otp_ktoPrerspTrsk;
      dug   = 0.00M;
      pot   = TheOtpList.Sum(otp => otp.R_NewTr_razd);

      AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);

      #endregion Phase 3

      #region Phase 4

      // 4. === za svaki RNP izmos R_OTP_razd na kto KSD.Dsc_otp_ktoObrade na POT
      //        a onda to sumarno na 
                  // a. Ako je KSD.Dsc_otp_IsSkladGotProizv true : na kto KSD.Dsc_otp_ktoGotProiz DUG
                  // b.                       else               : na kto KSD.Dsc_otp_kto7trosProizv DUG

      sumDug = sumPot = 0.00M;
      foreach(OTPdata otp_rec in TheOtpList)
      {
         konto = TheUC.KSD.Dsc_otp_ktoObrade;
         dug   = 0.00M;

       //pot   = otp_rec.R_OTP_razd;  28.03.2017.

         // 10.04.2017: !!! BIG NEWS !!! 
       //pot = otp_rec.R_TheOTP_Dovrs;

       // 08.09.2017: razluceno na Metaflex / Frigoterm 
       //if(otp_rec.R_IsGotovRNMnoPIP)    pot = otp_rec.R_NewTr_razd  ; // RNMnoPIP    
       //else                             pot = otp_rec.R_TheOTP_Dovrs; // old default 
         if(isMetaflex)
         {
            if(otp_rec.R_IsGotovRNMnoPIP) pot = otp_rec.R_NewTr_razd  ; // RNMnoPIP    
            else                          pot = otp_rec.R_TheOTP_Dovrs; // old default 
         }
         else // Frigoterm 
         {
                                          pot = otp_rec.R_OTP_razd;
         }

         sumDug += dug; sumPot += pot;

         if(otp_rec.ProjektCD == TheOtpList.Last().ProjektCD) // last iteration pass 
         {
          //   kontraSum = TheOtpList.Sum(otp => otp.R_OTP_razd    );
          //   kontraSum = TheOtpList.Sum(otp => otp.R_TheOTP_Dovrs);
          // 08.09.2017: razluceno na Metaflex / Frigoterm 
          //   kontraSum = TheOtpList.Where(otp =>  otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_NewTr_razd  )  // RNMnoPIP    
          //             + TheOtpList.Where(otp => !otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_TheOTP_Dovrs); // old default 
            if(isMetaflex)
            {
               kontraSum = TheOtpList.Where(otp =>  otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_NewTr_razd  )  // RNMnoPIP    
                         + TheOtpList.Where(otp => !otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_TheOTP_Dovrs); // old default 
            }
            else // Frigoterm 
            {
               kontraSum = TheOtpList.Sum(otp => otp.R_OTP_razd);
            }

            if(sumPot != kontraSum) // korekcija zaokruzivanja 
            {
               pot += kontraSum - sumPot; 
            }
         }

         AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);

      } // foreach(OTPdata otp_rec in TheOtpList)

      // ********* ZADNJI RED OTP NALOGA ______ START ______ 

      pot = 0.00M;

      // 06.03.2017: dodan if()
      if(TheUC.KSD.Dsc_otp_IsSkladGotProizv) // Metaflex 
      {
       // 28.03.2017.
       //konto = TheUC.KSD.Dsc_otp_ktoGotProiz;
       //// 27.03.2017:
       ////dug = TheOtpList.Sum(otp => otp.RD_razd     );
       //dug = TheOtpList.Sum(otp => otp.RD_razdDovrs);
       //AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);

         konto = TheUC.KSD.Dsc_otp_kto7trosProizv;

       // 28.03.2017.
       //dug   = TheOtpList.Sum(otp => otp.R_RI_razd);
         // 10.04.2017: !!! BIG NEWS !!! 
       //dug = TheOtpList.Sum(otp => otp.R_TheOTP_Dovrs);
         dug = TheOtpList.Where(otp =>  otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_NewTr_razd  )  // RNMnoPIP    
             + TheOtpList.Where(otp => !otp.R_IsGotovRNMnoPIP).Sum(otp => otp.R_TheOTP_Dovrs); // old default 

         AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);
      }
      else // Frigoterm 
      {
         konto = TheUC.KSD.Dsc_otp_kto7trosProizv; 
         dug   = TheOtpList.Sum(otp => otp.R_OTP_razd);
         AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);
      }

      // ********* ZADNJI RED OTP NALOGA ______  END  ______ 

      #endregion Phase 4

      AbortButton_Click(null, EventArgs.Empty);

    //ZXC.TheVvForm.OpenNew_Record_TabPage(ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.NAL_P), ZXC.NalogRec.RecID);
      ZXC.TheVvForm.LastRecord_OnClick(null, EventArgs.Empty);
   }

   private void QPrint_Click(object sender, EventArgs e)
   {
      SuspendLayout();

      #region Set Form

      Form ThePreviewForm = new Form();
      ThePreviewForm.Show();
      

      ThePreviewForm.Font      = ZXC.vvFont.BaseFont;
      ThePreviewForm.BackColor = ZXC.vvColors.userControl_BackColor;

      ThePreviewForm.FormClosing += new FormClosingEventHandler(ThePreviewIzvodForm_FormClosing_EnabledPreviewButton);

      ThePreviewForm.Location = Point.Empty;
      ThePreviewForm.Size     = new Size(SystemInformation.WorkingArea.Width - ZXC.Q10un * 3, SystemInformation.WorkingArea.Height);
      this.Location           = new Point(ThePreviewForm.Right, 0);

      #endregion Set Form

      ResumeLayout();

      #region FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      SetReportAndReportName_ForThisInnerTabPage(TheUC.ThePolyGridTabControl.SelectedTabPage.Title, ThePreviewForm);

      #endregion FillDataset, CreateReportDocument, SetDataSource, AssignReportSource

      ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, false);

   }

   private void SetReportAndReportName_ForThisInnerTabPage(string selectedTabName, Control parent)
   {
      VvStandAloneReportViewerUC thePreviewUC = new VvStandAloneReportViewerUC();
      thePreviewUC.Parent = parent;
      thePreviewUC.Dock   = DockStyle.Fill;

      CR_OTP_dgv36   otp_dgv36Kto = new CR_OTP_dgv36  ();
      CR_OTP_dgv45   otp_dgv45PCd = new CR_OTP_dgv45  ();
      CR_OTP_RnList  otp_RnList   = new CR_OTP_RnList ();
      CR_OTP_Obracun otp_Obracun  = new CR_OTP_Obracun();

      List<Kplan>  kplanList  = VvUserControl.KplanSifrar.Join(nrspIndirFtrList, kpln => kpln.Konto, ftr => ftr.T_konto, (kpln, ftr) => kpln).Distinct().ToList();

      if     (selectedTabName == TheUC.TabPageTitle1) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_Obracun , TheOtpList           , typeof(OTPdata).Name);                                SetReportNameAndDr(otp_Obracun , TheUC.TabPageTitle1); }
      else if(selectedTabName == TheUC.TabPageTitle2) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_RnList  , this.rnpFakturList   , typeof(Faktur ).Name); SetReportNameAndDr(otp_RnList  , TheUC.TabPageTitle2); }
      else if(selectedTabName == TheUC.TabPageTitle3) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv36Kto, this.nrspIndirFtrList, typeof(Ftrans ).Name); 
                                                        thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv36Kto, kplanList            , typeof(Kplan  ).Name);SetReportNameAndDr(otp_dgv36Kto, TheUC.TabPageTitle3); }
      else if(selectedTabName == TheUC.TabPageTitle4) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv45PCd, this.raspDirktFtrList, typeof(Ftrans ).Name); SetReportNameAndDr(otp_dgv45PCd, TheUC.TabPageTitle4); }
      else if(selectedTabName == TheUC.TabPageTitle5) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv45PCd, this.proUtijekFtrList, typeof(Ftrans ).Name); SetReportNameAndDr(otp_dgv45PCd, TheUC.TabPageTitle5); }
      else if(selectedTabName == TheUC.TabPageTitle6) { thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv36Kto, this.nrspIndirFtrList, typeof(Ftrans ).Name); 
                                                        thePreviewUC.SetNonDatasetDataSource_And_AssignReportSource(otp_dgv36Kto, kplanList            , typeof(Kplan  ).Name); SetReportNameAndDr(otp_dgv36Kto, TheUC.TabPageTitle6); }
   }

   private void SetReportNameAndDr(CrystalDecisions.CrystalReports.Engine.ReportDocument vvReportDocument,  string rptName)
   {
      CrystalDecisions.CrystalReports.Engine.ReportObjects reportObj = vvReportDocument.ReportDefinition.ReportObjects;

      vvReportDocument.DataDefinition.FormulaFields["Fof_ReportTitle"].Text = "'" + rptName                                   + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_NazivFirme" ].Text = "'" + ZXC.CURR_prjkt_rec.Naziv                  + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_DatOd"      ].Text = "'" + TheUC.Fld_DatumOd.ToString("dd.MM.yyyy.") + "'";
      vvReportDocument.DataDefinition.FormulaFields["Fof_DatDo"      ].Text = "'" + TheUC.Fld_DatumDo.ToString("dd.MM.yyyy.") + "'";

      int report_Width, razmak;
      razmak = 112;

      report_Width = vvReportDocument.PrintOptions.PageContentWidth;

      reportObj["Tobj_Razdoblje"].Width = report_Width;
      reportObj["Tobj_Razdoblje"].Left = razmak;
      (reportObj["Tobj_Razdoblje"] as CrystalDecisions.CrystalReports.Engine.TextObject).ObjectFormat.HorizontalAlignment = CrystalDecisions.Shared.Alignment.HorizontalCenterAlign;

      if(rptName == TheUC.TabPageTitle3) vvReportDocument.DataDefinition.FormulaFields["Fof_AnaSin"].Text = "'" + "A" + "'";
      if(rptName == TheUC.TabPageTitle6) vvReportDocument.DataDefinition.FormulaFields["Fof_AnaSin"].Text = "'" + "S" + "'";
      if(rptName == TheUC.TabPageTitle1) vvReportDocument.DataDefinition.FormulaFields["Fof_Koef"  ].Text = "'" + TheUC.Fld_KoefDir.ToString("N", ZXC.GetNumberFormatInfo(6)) + "'";

      vvReportDocument.DataDefinition.FormulaFields["Fof_ReportTitle"].Text = "'" + rptName + "'";

      // 03.03.2017: 
      theReportDocument = vvReportDocument;
   }

   #endregion Buttons_clik
}

public partial class ObrProUC : VvOtherUC
{

   #region TheG Util Metodz

   public static void RenumerateLineNumbers(DataGridView dgv, int fromRowIndex)
   {
      if(fromRowIndex == -1) fromRowIndex = 0; // malo tu trkeljimo ali zasada tako. 
      if(fromRowIndex < 0) return;

      for(int rowIndex = fromRowIndex; rowIndex < dgv.Rows.Count; ++rowIndex)
      {
         if(!dgv.Rows[rowIndex].IsNewRow) dgv.Rows[rowIndex].HeaderCell.Value = (rowIndex + 1).ToString();
      }
   }

   protected void UpdateLineCount(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      int idxCorrector;

      if(theGrid.AllowUserToAddRows == false) idxCorrector = 0;
      else                                    idxCorrector = 1;

      theSumGrid.Rows[0].HeaderCell.Value = (theGrid.RowCount - idxCorrector).ToString();
   }

   #endregion TheG Util Metodz

   private ObrProDLG                   TheDLG     { get { return this.Parent as ObrProDLG; } }
   private List</*ObrProDLG.*/OTPdata> TheOtpList { get { return TheDLG.TheOtpList       ; } }

   private void CalcRaspored(int rowIdx, List</*ObrProDLG.*/OTPdata> otpList)
   {
      /*ObrProDLG.*/OTPdata otp_rec = otpList[rowIdx];

      decimal theOTP, postRasp;
      decimal planCijSum = otpList.Sum(otp => otp.PlanCijena);
      decimal rdYearSum  = otpList.Sum(otp => otp.R_RD_year);

      if(Fld_IsDirekt == true) postRasp = ZXC.DivSafe(otp_rec.R_RD_year , rdYearSum  /** otp_rec.PostDovr / 100.00M*/) * 100.00M;
      else                     postRasp = ZXC.DivSafe(otp_rec.PlanCijena, planCijSum /** otp_rec.PostDovr / 100.00M*/) * 100.00M;

      theOTP = Fld_A_IznosIndirTr * postRasp / 100.00M;

//To modify the struct, first assign it to a local variable, modify the variable, then assign the variable back to the item in the collection.
//
//List<myStruct> list = {…};
//MyStruct ms = list[0];
//ms.Name = "MyStruct42";
//list[0] = ms;  

      otp_rec.R_TheOTP   = theOTP.Ron2();
      otp_rec.R_PostRasp = postRasp;

      otpList[rowIdx] = otp_rec;
   }

   private void CalcOnExitPostoDovrseno(object sender, EventArgs e)
   {
      VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
      //VvDataGridView dgv = (VvDataGridView)vtbec.EditingControlDataGridView;
      int rowIdx = vtbec.EditingControlRowIndex;

      GetDgvLineFields1(TheOtpList, rowIdx);

      TheGrid_1.PutCell(ci1.iT_Otp_year, rowIdx, TheOtpList[rowIdx].R_OTP_year);
      TheGrid_1.PutCell(ci1.iT_Otp_razd, rowIdx, TheOtpList[rowIdx].R_OTP_razd);

      TheSumGrid_1.PutCell(ci1.iT_Otp_razd, 0, TheOtpList.Sum(otp => otp.R_OTP_razd));
      TheSumGrid_1.PutCell(ci1.iT_Otp_year, 0, TheOtpList.Sum(otp => otp.R_OTP_year));
   }

   private void DisableNalogAction(object sender, EventArgs e)
   {
      if(IsPutKtoShemaDscFields_InProgress) return;

      //TheDLG.UcitajButton_Click(sender, e);
      TheDLG.EnableDisable_tsBtn_Nalog(false);
   }

   #region PutDGVfields

   #region TheG 1

   public void PutDgvFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      int rowIdx, idxCorrector;

      TheGrid_1.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_1);

      if(otpList != null)
      {
         for(rowIdx = 0; rowIdx < otpList.Count; ++rowIdx )  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_1.Rows.Add();

            PutDgvLineFields1(rowIdx, otpList);
         }

         int maxValueIdx;
         decimal rasporedDiff = Get_OTP_RasporedDiff(Fld_A_IznosIndirTr, otpList, out maxValueIdx);
         if(rasporedDiff.NotZero())
         {
            otpList[maxValueIdx].R_TheOTP += rasporedDiff;

            TheGrid_1.PutCell(ci1.iT_NIraspored, maxValueIdx, otpList[maxValueIdx].R_TheOTP);
         }
      }

      RenumerateLineNumbers(TheGrid_1, 0);

      UpdateLineCount(TheGrid_1);

      PutDgvTransSumFields1(otpList);
   }

   private decimal Get_OTP_RasporedDiff(decimal iznosIndirTr, List<OTPdata> otpList, out int maxValueIdx)
   {
      decimal rasporedDiff = 0M;
      maxValueIdx = 0;

      if(iznosIndirTr.IsZero() || otpList == null || otpList.Count.IsZero()) return 0;

      rasporedDiff = iznosIndirTr - otpList.Sum(otp => otp.R_TheOTP);

      decimal maxValue   = otpList.Max  (otp => otp.R_TheOTP);
      OTPdata maxOTPdata = otpList.First(otp => otp.R_TheOTP == maxValue);
      maxValueIdx        = otpList.IndexOf(maxOTPdata);

      return rasporedDiff;
   }

   public void PutDgvLineFields1(int rowIdx, List</*ObrProDLG.*/OTPdata> otpList) 
   {
      CalcRaspored(rowIdx, otpList);

      TheGrid_1.PutCell(ci1.iT_projektCD     /* cIdx  0 */, rowIdx, otpList[rowIdx].ProjektCD   ); //  1
      TheGrid_1.PutCell(ci1.it_Investit_otp  /* cIdx  1 */, rowIdx, otpList[rowIdx].Investitor  ); //  2
      TheGrid_1.PutCell(ci1.iT_dovrsenost    /* cIdx  2 */, rowIdx, otpList[rowIdx].PostDovr    ); //  3
      TheGrid_1.PutCell(ci1.iT_PlanCijena    /* cIdx  3 */, rowIdx, otpList[rowIdx].PlanCijena  ); //  4
      TheGrid_1.PutCell(ci1.iT_PUT_dug_year  /* cIdx  4 */, rowIdx, otpList[rowIdx].PUT_dug     ); //  5
      TheGrid_1.PutCell(ci1.iT_PUT_pot_year  /* cIdx  5 */, rowIdx, otpList[rowIdx].PUT_pot     ); //  6
      TheGrid_1.PutCell(ci1.iT_RD_razd       /* cIdx  6 */, rowIdx, otpList[rowIdx].RD_razd     ); //  7
      TheGrid_1.PutCell(ci1.iT_RD_year       /* cIdx  7 */, rowIdx, otpList[rowIdx].R_RD_year   ); //  8
      TheGrid_1.PutCell(ci1.iT_NIraspored    /* cIdx  8 */, rowIdx, otpList[rowIdx].R_TheOTP    ); //  9
      TheGrid_1.PutCell(ci1.iT_PostoRaspored /* cIdx  9 */, rowIdx, otpList[rowIdx].R_PostRasp  ); // 10
      TheGrid_1.PutCell(ci1.iT_NewTr_razd    /* cIdx 10 */, rowIdx, otpList[rowIdx].R_NewTr_razd); // 11
      TheGrid_1.PutCell(ci1.iT_Otp_razd      /* cIdx 11 */, rowIdx, otpList[rowIdx].R_OTP_razd  ); // 12
      TheGrid_1.PutCell(ci1.iT_NewTr_year    /* cIdx 12 */, rowIdx, otpList[rowIdx].R_NewTr_year); // 13
      TheGrid_1.PutCell(ci1.iT_Otp_year      /* cIdx 13 */, rowIdx, otpList[rowIdx].R_OTP_year  ); // 14

   }

   public void PutDgvTransSumFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      TheSumGrid_1.PutCell(ci1.iT_PlanCijena   , 0, otpList.Sum(otp => otp.PlanCijena  ));
      TheSumGrid_1.PutCell(ci1.iT_PUT_dug_year , 0, otpList.Sum(otp => otp.PUT_dug     ));
      TheSumGrid_1.PutCell(ci1.iT_PUT_pot_year , 0, otpList.Sum(otp => otp.PUT_pot     ));
      TheSumGrid_1.PutCell(ci1.iT_RD_razd      , 0, otpList.Sum(otp => otp.RD_razd     ));
      TheSumGrid_1.PutCell(ci1.iT_RD_year      , 0, otpList.Sum(otp => otp.R_RD_year   ));
      TheSumGrid_1.PutCell(ci1.iT_PostoRaspored, 0, otpList.Sum(otp => otp.R_PostRasp  ));
      TheSumGrid_1.PutCell(ci1.iT_NIraspored   , 0, otpList.Sum(otp => otp.R_TheOTP    ));
      TheSumGrid_1.PutCell(ci1.iT_NewTr_razd   , 0, otpList.Sum(otp => otp.R_NewTr_razd));
      TheSumGrid_1.PutCell(ci1.iT_NewTr_year   , 0, otpList.Sum(otp => otp.R_NewTr_year));
      TheSumGrid_1.PutCell(ci1.iT_Otp_razd     , 0, otpList.Sum(otp => otp.R_OTP_razd  ));
      TheSumGrid_1.PutCell(ci1.iT_Otp_year     , 0, otpList.Sum(otp => otp.R_OTP_year  ));
   }

   internal void GetDgvFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      for(int rIdx = 0; rIdx < TheGrid_1.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields1(otpList, rIdx);
      }
   }

   private /*ObrProDLG.*/OTPdata GetDgvLineFields1(List</*ObrProDLG.*/OTPdata> otpList, int rowIdx)
   {
      /*ObrProDLG.*/OTPdata otp_rec;

      otp_rec = otpList[rowIdx];
      otp_rec.PostDovr = TheGrid_1.GetDecimalCell(ci1.iT_dovrsenost, rowIdx, false);
      otpList[rowIdx] = otp_rec;

      return otp_rec;
   }

   #endregion TheG 1

   #region TheG 2

   public void PutDgvFields2(List<Faktur> fakturList)
   {
      int rowIdx, idxCorrector;

      TheGrid_2.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_2);

      if(fakturList != null)
      {
         foreach(Faktur faktur_rec in fakturList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_2.Rows.Add();

            rowIdx = TheGrid_2.RowCount - idxCorrector;

            PutDgvLineFields2(faktur_rec, rowIdx, false);
         }
      }

      RenumerateLineNumbers(TheGrid_2, 0);

      UpdateLineCount(TheGrid_2);

      PutDgvTransSumFields2(fakturList);
   }

   public void PutDgvLineFields2(Faktur faktur_rec, int rowIdx, bool skipRecID_andSerial_Columns) 
   {
      if(skipRecID_andSerial_Columns == false)
      {
         TheGrid_2.PutCell(ci2.iT_recID , rowIdx, faktur_rec.RecID);
       //TheGrid_2.PutCell(ci2.iT_serial, rowIdx, faktur_rec.T_serial);
      }

      TheGrid_2.PutCell(ci2.iT_projektCD    , rowIdx, faktur_rec.TT_And_TtNum);
      TheGrid_2.PutCell(ci2.iT_narInv_fa    , rowIdx, faktur_rec.KupdobName);
      TheGrid_2.PutCell(ci2.iT_objektCd_fa  , rowIdx, faktur_rec.PrjArtCD);
      TheGrid_2.PutCell(ci2.iT_objektName_fa, rowIdx, faktur_rec.PrjArtName);
      TheGrid_2.PutCell(ci2.iT_refDok_fa    , rowIdx, faktur_rec.VezniDok);

      if(faktur_rec.PonudDate != DateTimePicker.MinimumDateTime)
         TheGrid_2.PutCell(ci2.iT_dateStart_fa , rowIdx, faktur_rec.PonudDate);
      
      if(faktur_rec.RokIspDate != DateTimePicker.MinimumDateTime)
         TheGrid_2.PutCell(ci2.iT_dateRokIsp_fa, rowIdx, faktur_rec.RokIspDate);

      TheGrid_2.PutCell(ci2.iT_ugCijena_fa  , rowIdx, faktur_rec.SomeMoney);
      TheGrid_2.PutCell(ci2.iT_dovrsenost_fa, rowIdx, faktur_rec.SomePercent);
      TheGrid_2.PutCell(ci2.iT_status_fa    , rowIdx, faktur_rec.StatusCD);
   }

   public void PutDgvTransSumFields2(List<Faktur> fakturList)
   {
      TheSumGrid_2.PutCell(ci2.iT_ugCijena_fa, 0, fakturList.Sum(fak => fak.SomeMoney));
   }

   #endregion TheG 2

   #region TheG 3

   public void PutDgvFields3(List<Ftrans> ftransList)
   {
      int rowIdx, idxCorrector;

      TheGrid_3.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_3);

      if(ftransList != null)
      {
         foreach(Ftrans ftrans_rec in ftransList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_3.Rows.Add();

            rowIdx = TheGrid_3.RowCount - idxCorrector;

            PutDgvLineFields3(ftrans_rec, rowIdx, false);
         }
      }

      RenumerateLineNumbers(TheGrid_3, 0);

      UpdateLineCount(TheGrid_3);

      PutDgvTransSumFields3(ftransList);
   }

   public void PutDgvLineFields3(Ftrans ftrans_rec, int rowIdx, bool skipRecID_andSerial_Columns) 
   {
      if(skipRecID_andSerial_Columns == false)
      {
         TheGrid_3.PutCell(ci3.iT_recID , rowIdx, ftrans_rec.T_recID);
       //TheGrid_3.PutCell(ci3.iT_serial, rowIdx, ftrans_rec.T_serial);
      }

      TheGrid_3.PutCell(ci3.iT_konto       , rowIdx, ftrans_rec.T_konto);
      TheGrid_3.PutCell(ci3.iT_dokDate     , rowIdx, ftrans_rec.T_dokDate);
      TheGrid_3.PutCell(ci3.iT_dokNum      , rowIdx, ftrans_rec.T_dokNum);
      TheGrid_3.PutCell(ci3.iT_ttNum       , rowIdx, ftrans_rec.T_TT);
      TheGrid_3.PutCell(ci3.iT_opis        , rowIdx, ftrans_rec.T_opis);
      TheGrid_3.PutCell(ci3.iT_kupdob_cd   , rowIdx, ftrans_rec.T_kupdob_cd);

      Kupdob kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == ftrans_rec.T_kupdob_cd);

      if(kupdobSifrar_rec != null) TheGrid_3.PutCell(ci3.iT_kupdob_name, rowIdx, kupdobSifrar_rec.Naziv);
      else                         TheGrid_3.PutCell(ci3.iT_kupdob_name, rowIdx, "");

      TheGrid_3.PutCell(ci3.iT_ticker      , rowIdx, ftrans_rec.T_ticker);
      TheGrid_3.PutCell(ci3.iT_tipBr       , rowIdx, ftrans_rec.T_tipBr);
      TheGrid_3.PutCell(ci3.iT_dug         , rowIdx, ftrans_rec.T_dug);
      TheGrid_3.PutCell(ci3.iT_pot         , rowIdx, ftrans_rec.T_pot);
      TheGrid_3.PutCell(ci3.iT_projektCD   , rowIdx, ftrans_rec.T_projektCD);

   }

   public void PutDgvTransSumFields3(List<Ftrans> ftransList)
   {
      TheSumGrid_3.PutCell(ci3.iT_dug, 0, ftransList.Sum(ftr => ftr.T_dug));
      TheSumGrid_3.PutCell(ci3.iT_pot, 0, ftransList.Sum(ftr => ftr.T_pot));
   }

   #endregion TheG 3

   #region TheG 4

   public void PutDgvFields4(List<Ftrans> ftransList)
   {
      int rowIdx, idxCorrector;

      TheGrid_4.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_4);

      if(ftransList != null)
      {
         foreach(Ftrans ftrans_rec in ftransList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_4.Rows.Add();

            rowIdx = TheGrid_4.RowCount - idxCorrector;

            PutDgvLineFields4(ftrans_rec, rowIdx, false);
         }
      }

      RenumerateLineNumbers(TheGrid_4, 0);

      UpdateLineCount(TheGrid_4);

      PutDgvTransSumFields4(ftransList);
   }

   public void PutDgvLineFields4(Ftrans ftrans_rec, int rowIdx, bool skipRecID_andSerial_Columns) 
   {
      if(skipRecID_andSerial_Columns == false)
      {
         TheGrid_4.PutCell(ci4.iT_recID , rowIdx, ftrans_rec.T_recID);
       //TheGrid_4.PutCell(ci4.iT_serial, rowIdx, ftrans_rec.T_serial);
      }

      TheGrid_4.PutCell(ci4.iT_konto       , rowIdx, ftrans_rec.T_konto);
      TheGrid_4.PutCell(ci4.iT_dokDate     , rowIdx, ftrans_rec.T_dokDate);
      TheGrid_4.PutCell(ci4.iT_dokNum      , rowIdx, ftrans_rec.T_dokNum);
      TheGrid_4.PutCell(ci4.iT_ttNum       , rowIdx, ftrans_rec.T_TT);
      TheGrid_4.PutCell(ci4.iT_opis        , rowIdx, ftrans_rec.T_opis);
      TheGrid_4.PutCell(ci4.iT_kupdob_cd   , rowIdx, ftrans_rec.T_kupdob_cd);

      Kupdob kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == ftrans_rec.T_kupdob_cd);

      if(kupdobSifrar_rec != null) TheGrid_4.PutCell(ci4.iT_kupdob_name, rowIdx, kupdobSifrar_rec.Naziv);
      else                         TheGrid_4.PutCell(ci4.iT_kupdob_name, rowIdx, "");

      TheGrid_4.PutCell(ci4.iT_ticker      , rowIdx, ftrans_rec.T_ticker);
      TheGrid_4.PutCell(ci4.iT_tipBr       , rowIdx, ftrans_rec.T_tipBr);
      TheGrid_4.PutCell(ci4.iT_dug         , rowIdx, ftrans_rec.T_dug);
      TheGrid_4.PutCell(ci4.iT_pot         , rowIdx, ftrans_rec.T_pot);
      TheGrid_4.PutCell(ci4.iT_projektCD   , rowIdx, ftrans_rec.T_projektCD);

   }

   public void PutDgvTransSumFields4(List<Ftrans> ftransList)
   {
      TheSumGrid_4.PutCell(ci4.iT_dug, 0, ftransList.Sum(ftr => ftr.T_dug));
      TheSumGrid_4.PutCell(ci4.iT_pot, 0, ftransList.Sum(ftr => ftr.T_pot));
   }

   #endregion TheG 4

   #region TheG 5

   public void PutDgvFields5(List<Ftrans> ftransList)
   {
      int rowIdx, idxCorrector;

      TheGrid_5.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_5);

      if(ftransList != null)
      {
         foreach(Ftrans ftrans_rec in ftransList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_5.Rows.Add();

            rowIdx = TheGrid_5.RowCount - idxCorrector;

            PutDgvLineFields5(ftrans_rec, rowIdx, false);
         }
      }

      RenumerateLineNumbers(TheGrid_5, 0);

      UpdateLineCount(TheGrid_5);

      PutDgvTransSumFields5(ftransList);
   }

   public void PutDgvLineFields5(Ftrans ftrans_rec, int rowIdx, bool skipRecID_andSerial_Columns) 
   {
      if(skipRecID_andSerial_Columns == false)
      {
         TheGrid_5.PutCell(ci5.iT_recID , rowIdx, ftrans_rec.T_recID);
       //TheGrid_5.PutCell(ci5.iT_serial, rowIdx, ftrans_rec.T_serial);
      }

      TheGrid_5.PutCell(ci5.iT_konto       , rowIdx, ftrans_rec.T_konto);
      TheGrid_5.PutCell(ci5.iT_dokDate     , rowIdx, ftrans_rec.T_dokDate);
      TheGrid_5.PutCell(ci5.iT_dokNum      , rowIdx, ftrans_rec.T_dokNum);
      TheGrid_5.PutCell(ci5.iT_ttNum       , rowIdx, ftrans_rec.T_TT);
      TheGrid_5.PutCell(ci5.iT_opis        , rowIdx, ftrans_rec.T_opis);
      TheGrid_5.PutCell(ci5.iT_kupdob_cd   , rowIdx, ftrans_rec.T_kupdob_cd);

      Kupdob kupdobSifrar_rec = KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == ftrans_rec.T_kupdob_cd);

      if(kupdobSifrar_rec != null) TheGrid_5.PutCell(ci5.iT_kupdob_name, rowIdx, kupdobSifrar_rec.Naziv);
      else                         TheGrid_5.PutCell(ci5.iT_kupdob_name, rowIdx, "");

      TheGrid_5.PutCell(ci5.iT_ticker      , rowIdx, ftrans_rec.T_ticker);
      TheGrid_5.PutCell(ci5.iT_tipBr       , rowIdx, ftrans_rec.T_tipBr);
      TheGrid_5.PutCell(ci5.iT_dug         , rowIdx, ftrans_rec.T_dug);
      TheGrid_5.PutCell(ci5.iT_pot         , rowIdx, ftrans_rec.T_pot);
      TheGrid_5.PutCell(ci5.iT_projektCD   , rowIdx, ftrans_rec.T_projektCD);

   }

   public void PutDgvTransSumFields5(List<Ftrans> ftransList)
   {
      TheSumGrid_5.PutCell(ci5.iT_dug, 0, ftransList.Sum(ftr => ftr.T_dug));
      TheSumGrid_5.PutCell(ci5.iT_pot, 0, ftransList.Sum(ftr => ftr.T_pot));
   }

   #endregion TheG 5

   #region TheG 6

   public void PutDgvFields6(List<ZXC.VvUtilDataPackage> kontaSumeList)
   {
      int rowIdx, idxCorrector;

      TheGrid_6.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_6);

      if(kontaSumeList != null)
      {
         foreach(ZXC.VvUtilDataPackage data_rec in kontaSumeList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_6.Rows.Add();

            rowIdx = TheGrid_6.RowCount - idxCorrector;

            PutDgvLineFields6(data_rec, rowIdx);
         }
      }

      RenumerateLineNumbers(TheGrid_6, 0);

      UpdateLineCount(TheGrid_6);

      PutDgvTransSumFields6(kontaSumeList);
   }

   public void PutDgvLineFields6(ZXC.VvUtilDataPackage data_rec, int rowIdx) 
   {
      TheGrid_6.PutCell(ci6.iT_konto       , rowIdx, data_rec.TheStr1    );
      TheGrid_6.PutCell(ci6.iT_dug         , rowIdx, data_rec.TheDecimal );
      TheGrid_6.PutCell(ci6.iT_pot         , rowIdx, data_rec.TheDecimal2);

      Kplan kplanSifrar_rec = KplanSifrar.SingleOrDefault(vvDR => vvDR.Konto == data_rec.TheStr1);

      if(kplanSifrar_rec != null) TheGrid_6.PutCell(ci6.iT_opis, rowIdx, kplanSifrar_rec.Naziv);
      else                        TheGrid_6.PutCell(ci6.iT_opis, rowIdx, "");
   }

   public void PutDgvTransSumFields6(List<ZXC.VvUtilDataPackage> kontaSumeList)
   {
      TheSumGrid_6.PutCell(ci6.iT_dug, 0, kontaSumeList.Sum(grp => grp.TheDecimal ));
      TheSumGrid_6.PutCell(ci6.iT_pot, 0, kontaSumeList.Sum(grp => grp.TheDecimal2));
   }

   #endregion TheG 6

   #endregion PutDGVfields

}

public partial class AnalizaProizDLG : Form// VvDialog{
{
   #region OTP data

   public List<OTPdata> TheOtpList;

   // 03.03.2017: 
   internal CrystalDecisions.CrystalReports.Engine.ReportDocument theReportDocument;

   #endregion OTP struct

   #region Buttons_clik

   public void UcitajButton_Click(object sender, EventArgs e)
   {
      if(TheUC.GetKtoShemaDscFields()) return; // GetKtoShemaDscFields() ujedno i validira 

      Cursor.Current = Cursors.WaitCursor;

      bool loadOK =

      FtransDao.GetAnalizaTroskovaProizvodnjeListe(
         theDbConnection,
         TheUC.Fld_DatumOd,
         TheUC.Fld_DatumDo,
         TheUC.KSD,
         out raspIndirFtrList,
         out raspDirktFtrList,
         out nrspRezskFtrList,
         out nrspAmortFtrList,
         out proUtijekFtrList,
         out rnpFakturList);

      Cursor.Current = Cursors.Default;

      EnableDisable_tsBtn_Nalog(loadOK);

      List<string> suspendedRnpList = CheckSuspendedRnp(TheUC.KSD.Dsc_otp_skipSatus,
                                                        raspDirktFtrList           ,
                                                        proUtijekFtrList           ,
                                                        rnpFakturList              );

      if(suspendedRnpList.Count.NotZero())
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         foreach(string suspendedRnp in suspendedRnpList)
         {
            sb.Append(suspendedRnp + "\n");
         }

         ZXC.aim_emsg(MessageBoxIcon.Error, "U zadanom razdoblju postoje direktni troškovi koji se odnose na suspendirane (pauzirane, 'na čekanju') radne naloge.\n\n" +
                                            "Promijenite status radnog naloga ili preknjižite troškove sa suspendiranih na otvorene radne naloge.\n\nRadni nalozi:\n\n" +
                                            sb);
         return; // !!! 
      }

      PutLoadedLists(); // VOILA! 

      if(loadOK == false) ZXC.aim_emsg(MessageBoxIcon.Information, "Za zadane kriterije nema se što ili nema se na što raspoređivati.\n\nBroj NERASPOREĐENIH knjiženja: {0}\n\nBroj RADNIH NALOGA: {1}", nrspRezskFtrList.Count, rnpFakturList.Count);

      //if(IsRNP_Analiza) // ugasi i predaj liste 
      //{
      //   this.Close();
      //}
   }

   private List<string> CheckSuspendedRnp(string suspendedTT, List<Ftrans> raspDirktFtrList, List<Ftrans> proUtijekFtrList, List<Faktur> rnpFakturList)
   {
      List<string> suspendedRnpFakturList = new List<string>();
      Faktur faktur_rec;

      // kreiraj listu eventualnih pojava suspendiranih u raspDirktFtrList 
      foreach(Ftrans ftrans_rec in raspDirktFtrList)
      {
         faktur_rec = rnpFakturList.SingleOrDefault(fak => fak.TT_And_TtNum == ftrans_rec.T_projektCD);

         if(faktur_rec != null && faktur_rec.StatusCD == suspendedTT)
         {
            suspendedRnpFakturList.Add(faktur_rec.TT_And_TtNum);
         }
      }

      if(suspendedRnpFakturList.Count.IsZero()) // ovo radi samo ako nema prethodnog problema (pojave suspendiranih u raspDirktFtrList) 
      {
         // izbaci suspendirane iz proUtijekFtrList 
         for(int i = 0; i < proUtijekFtrList.Count; ++i)
         {
            faktur_rec = rnpFakturList.SingleOrDefault(fak => fak.TT_And_TtNum == proUtijekFtrList[i].T_projektCD);

            if(faktur_rec != null && faktur_rec.StatusCD == suspendedTT)
            {
               proUtijekFtrList.RemoveAt(i--);
               rnpFakturList   .Remove  (faktur_rec);
            }
         }
      }

      return suspendedRnpFakturList.Distinct().ToList();
   }

   private void PutLoadedLists()
   {
      // 22.04.2015: 
    //TheUC.Fld_A_IznosREZIJSKIH_Tr = nrspRezskFtrList.Sum(ftr => ftr.R_DugMinusPot);
    //TheUC.Fld_A_IznosAMORT_Tr     = nrspAmortFtrList.Sum(ftr => ftr.R_DugMinusPot);
      TheUC.Fld_A_IznosREZIJSKIH_Tr = nrspRezskFtrList.Sum(ftr => ftr.T_dug        );
      TheUC.Fld_A_IznosAMORT_Tr     = nrspAmortFtrList.Sum(ftr => ftr.T_dug        );

      TheUC.Fld_A_IznPostoREZIJSKIH_Tr = TheUC.Fld_A_IznosREZIJSKIH_Tr * (TheUC.Fld_PostoRr / 100.00M);
      TheUC.Fld_A_IznPostoAMORT_Tr     = TheUC.Fld_A_IznosAMORT_Tr     * (TheUC.Fld_PostoRr / 100.00M);
      
      

      TheOtpList = GetOtpList
         (
            rnpFakturList, 
            raspDirktFtrList, 
            raspIndirFtrList, 
            proUtijekFtrList, 
            nrspRezskFtrList,
            nrspAmortFtrList,
            TheUC.Fld_DatumOd, 
            TheUC.Fld_DatumDo
         );

      TheUC.PutDgvFields1(TheOtpList      );

      TheUC.PutDgvFields2(rnpFakturList   );

      TheUC.SetSifrarAndAutocomplete<Kplan> (null, VvSQL.SorterType.Konto);
      TheUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name );

      CheckRaspored();
   }

   private void CheckRaspored()
   {
      if(TheUC.Fld_IsDirekt)
      {
         decimal left, right;

         // check REZIJSKI =================================== 

         left = TheUC.Fld_A_IznosREZIJSKIH_Tr * (TheUC.Fld_PostoRr / 100.00M);
         right = TheOtpList.Sum(otp => otp.R_TheREZIJE);

         if(ZXC.AlmostEqual(left, right, 0.02M) == false) 
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje!\n\n Suma neraspoređenih režijskih troškova {0}\n\nnije jednaka sumi raspoređenih {1}\n\n...Da niste zaboravili napraviti mjesečne rasporede indirektnih troškova?",
               left.ToStringVv(), right.ToStringVv());

         // check AMORT =================================== 

         left  = TheUC.Fld_A_IznosAMORT_Tr;
         right = TheOtpList.Sum(otp => otp.R_TheAMORT);

         if(ZXC.AlmostEqual(left, right, 0.02M) == false) 
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Upozorenje!\n\n Suma neraspoređenih amortizacijskih troškova {0}\n\nnije jednaka sumi raspoređenih {1}",
               left.ToStringVv(), right.ToStringVv());
      }
      else // TODO: !!! ? 
      {
      }
   }

   private List<ZXC.VvUtilDataPackage> GetKontaSume(List<Ftrans> nrspIndirFtrList)
   {
      List<ZXC.VvUtilDataPackage> kontaSumeList = new List<ZXC.VvUtilDataPackage>();

      kontaSumeList =
         nrspIndirFtrList
         .GroupBy(ftr => ftr.T_konto)
         .Select(grp => new ZXC.VvUtilDataPackage(grp.Key, grp.Sum(ftr => ftr.T_dug), grp.Sum(ftr => ftr.T_pot)))
         .OrderBy(grp => grp.TheStr1)
         .ToList();         

      return kontaSumeList;
   }

   /*****/private List<OTPdata> GetOtpList
      (
         List<Faktur> rnpFakturList   ,
         List<Ftrans> raspDirktFtrList,
         List<Ftrans> raspIndirFtrList,
         List<Ftrans> proUtijekFtrList,
         List<Ftrans> nrspRezskFtrList,
         List<Ftrans> nrspAmortFtrList, 
         DateTime     dateOD          , 
         DateTime     dateDO
      )
   {
      TheOtpList = new List<OTPdata>(rnpFakturList.Count);

      decimal PUT_PS, PUT_dug, PUT_pot, RD_razd, RI_razd;
      bool konto6000seNeJavljaUrazdoblju;
      decimal R_TheREZIJE, R_TheAMORT;

    //foreach(Faktur faktur_rec[i] in rnpFakturList)
      for(int i = 0; i < rnpFakturList.Count; ++i)
      {
         PUT_dug = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_dug);
         PUT_pot = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_pot);

         konto6000seNeJavljaUrazdoblju = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TipBr && ftr.T_dokDate >= dateOD && ftr.T_dokDate <= dateDO).Count().IsZero();

         // pravilo: ako je RNP 'gotov, zavrsen' znaci saldo 6000 je na nuli, onda treba zakljuciti da li je umro u prethodnom razdoblju
         // ili u razdoblju koje pormatramo. Ako je umro u prethodnom, tada ga treba izbaciti iz rnpFakturList-e 
         if((PUT_dug - PUT_pot).IsZero() && konto6000seNeJavljaUrazdoblju)
         {
            proUtijekFtrList = proUtijekFtrList.Where(ftr => ftr.T_projektCD != rnpFakturList[i].TipBr).ToList(); // izbaci iz proUtijekFtrList sve ftr-ove koji su po ovom RNP-u 
            rnpFakturList.RemoveAt(i--);
         }
      }

      DateTime dateFD  = ZXC.projectYearFirstDay;
      TimeSpan oneDay  = new TimeSpan(1, 0, 0, 0);
      DateTime proizDO = (dateOD == dateFD ? dateFD : dateOD - oneDay); // ako je razd od 1.1. onda na 1.1, a ako je razd npr. od 1.4. onda do 31.3 

      for(int i = 0; i < rnpFakturList.Count; ++i)
      {
       //PUT_dug = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_dug);
       //PUT_pot = proUtijekFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.T_pot);

         var pero = proUtijekFtrList.Where
            (ftr =>
               ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum &&
               ftr.T_dokDate >= dateFD &&
               ftr.T_dokDate <= proizDO
            );

         PUT_PS = proUtijekFtrList.Where
            (ftr => 
               ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum &&
               ftr.T_dokDate >= dateFD &&
               ftr.T_dokDate <= proizDO
            ).Sum(ftr => ftr.T_dug);

         RD_razd = raspDirktFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.R_DugMinusPot);
         RI_razd = raspIndirFtrList.Where(ftr => ftr.T_projektCD == rnpFakturList[i].TT_And_TtNum).Sum(ftr => ftr.R_DugMinusPot);

         Calc_IATP_Raspored
         (
            rnpFakturList    ,
            rnpFakturList[i] ,
            proUtijekFtrList ,
            nrspRezskFtrList ,
            nrspAmortFtrList ,
            dateOD           ,
            dateDO           ,
            TheUC.Fld_PostoRr,
            out R_TheREZIJE  ,
            out R_TheAMORT
         );

         TheOtpList.Add
            (
               new OTPdata
                  (
                     rnpFakturList[i].TT_And_TtNum, 
                     rnpFakturList[i].KupdobName, 
                     rnpFakturList[i].SomeMoney, 
                     PUT_PS,
                     RD_razd, 
                     RI_razd,
                     R_TheREZIJE,
                     R_TheAMORT,
                     TheUC.Fld_Rezervacija1,
                     TheUC.Fld_Rezervacija2
                  )
            );

      } // for(int i = 0; i < rnpFakturList.Count; ++i)

      return TheOtpList;
   }

   private void Calc_IATP_Raspored(List<Faktur> rnpFakturList, Faktur faktur_rec /* to je RNP */, List<Ftrans> proUtijekFtrList, List<Ftrans> nrspRezskFtrList, List<Ftrans> nrspAmortFtrList, DateTime dateOD, DateTime dateDO, decimal stopaPriznatihRez, out decimal R_TheREZIJE, out decimal R_TheAMORT)
   {
      R_TheREZIJE = R_TheAMORT = 0.00M;

      DateTime dateStartM, dateEndM, dateStartPUT, dateEndPUT;
      decimal koefR, koefA;
      decimal mon_rezijski, mon_amortizac, all_PUT, thisRN_PUT;

      List<Ftrans> y2monthisRN_proUtijekFtrList, thisRN_proUtijekFtrList, y2mon_proUtijekFtrList, mon_proUtijekFtrList, mon_nrspRezskFtrList, mon_nrspAmortFtrList;
      List<string> mon_RNPs;

      dateStartPUT = ZXC.projectYearFirstDay;

      for(int month = dateOD.Month; month <= dateDO.Month; ++month)
      {
         dateStartM = new DateTime(dateOD.Year, month, 1);
         dateEndM   = new DateTime(dateOD.Year, month, DateTime.DaysInMonth(dateOD.Year, month));

         dateEndPUT = dateEndM;

         thisRN_proUtijekFtrList      = proUtijekFtrList       .Where(ftr => ftr.T_projektCD == faktur_rec.TipBr).ToList();
         y2monthisRN_proUtijekFtrList = thisRN_proUtijekFtrList.Where(ftr => ftr.T_dokDate >= dateStartPUT && ftr.T_dokDate <= dateEndPUT).ToList();
         y2mon_proUtijekFtrList       = proUtijekFtrList       .Where(ftr => ftr.T_dokDate >= dateStartPUT && ftr.T_dokDate <= dateEndPUT).ToList();
         mon_proUtijekFtrList         = proUtijekFtrList       .Where(ftr => ftr.T_dokDate >= dateStartM   && ftr.T_dokDate <= dateEndM  ).ToList();

         mon_RNPs = mon_proUtijekFtrList.Select(ftr => ftr.T_projektCD).Distinct().ToList();
         y2mon_proUtijekFtrList.RemoveAll(ftr => mon_RNPs.Contains(ftr.T_projektCD) == false);

         mon_nrspRezskFtrList = nrspRezskFtrList.Where(ftr => ftr.T_dokDate >= dateStartM   && ftr.T_dokDate <= dateEndM  ).ToList();
         mon_nrspAmortFtrList = nrspAmortFtrList.Where(ftr => ftr.T_dokDate >= dateStartM   && ftr.T_dokDate <= dateEndM  ).ToList();

         mon_rezijski  = mon_nrspRezskFtrList.Sum(ftr => ftr.R_DugMinusPot);
         mon_rezijski *= stopaPriznatihRez / 100.00M; // korekcija za stopaPriznatihRez
         mon_amortizac = mon_nrspAmortFtrList.Sum(ftr => ftr.R_DugMinusPot);

       //all_PUT       = proUtijekFtrList            .Sum(ftr => ftr.T_dug);
         all_PUT       = y2mon_proUtijekFtrList      .Sum(ftr => ftr.T_dug);
       //thisRN_PUT    = thisRN_proUtijekFtrList     .Sum(ftr => ftr.T_dug);
         thisRN_PUT    = y2monthisRN_proUtijekFtrList.Sum(ftr => ftr.T_dug);

         koefR = ZXC.DivSafe(mon_rezijski,  all_PUT);
         koefA = ZXC.DivSafe(mon_amortizac, all_PUT);

         if(y2mon_proUtijekFtrList.Count(ftr => ftr.T_projektCD == faktur_rec.TipBr).IsZero()) continue;

         R_TheREZIJE += koefR * thisRN_PUT;
         R_TheAMORT  += koefA * thisRN_PUT;
      }

   }
   
   private void AbortButton_Click(object sender, EventArgs e)
   {
      this.Close();

      //ZXC.TheVvForm.EscapeAction_OnClick(this, EventArgs.Empty);
   }

   private void AutoSetNalogOTP(
      ref ushort line, 
      Nalog    nalog_rec,       
      string   t_projektCD,
      string   t_konto,
      string   t_opis,
      decimal  t_dug,
      decimal  t_pot)
   {
      if((t_dug + t_pot).IsZero()) return;

      NalogDao.AutoSetNalog(theDbConnection, ref line, nalog_rec.DokDate, nalog_rec.TT, nalog_rec.Napomena, t_projektCD, t_konto, t_opis, t_dug, t_pot);
   }

   //private void NaNalogButton_Click(object sender, EventArgs e)
   //{
   //   #region Init stuff

   //   TheUC.GetKtoShemaDscFields();

   //   ushort line = 0;
   //   Nalog  nalog_rec  = new Nalog ();
   //   Ftrans ftrans_rec = new Ftrans();

   //   decimal dug, pot, sumDug, sumPot, kontraSum;
   //   string konto;
   //   string opis  = "Auto OTP";

   //   nalog_rec.TT       = "TM";
   //   nalog_rec.DokDate  = TheUC.Fld_DatumDo;
   //   nalog_rec.Napomena = "Raspored indirektnih troškova i obračun proizvodnje za razdoblje " + TheUC.Fld_DatumOd.ToString(ZXC.VvDateDdMmFormat) + " - " + TheUC.Fld_DatumDo.ToString(ZXC.VvDateFormat);

   //   #endregion Init stuff

   //   if(TheUC.Fld_A_IznosIndirTr.IsZero()) // Nema se sto za rasporedivati od INDIREKTNIH (ili ih nema za razdoblje, ili se neko zaje. pa radi uduplo)
   //   {
   //      DialogResult result = MessageBox.Show("U ovome razdoblju nema neraspoređenih indirektnih troškova.\n\nDa li zaista zelite kreirati nalog?", "Potvrdite NALOG?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
   //      if(result != DialogResult.Yes) return;
   //   }

   //   #region Anuliranje, minusiranje sintetike nerasp. indir. troskova

   //   // 1. === Za svaki redak 6 Tab-a minusirati konto retka DUG


   //   List<ZXC.VvUtilDataPackage> kontaSumeList = GetKontaSume(nrspRezskFtrList);

   //   foreach(ZXC.VvUtilDataPackage kontoSuma in kontaSumeList)
   //   {
   //      konto = kontoSuma.TheStr1;
   //      dug   = -(kontoSuma.TheDecimal - kontoSuma.TheDecimal2);
   //      pot   = 0.00M;

   //      AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);
   //   }

   //   #endregion Anuliranje, minusiranje sintetike nerasp. indir. troskova

   //   #region Razrada foreach rnp: foreach konto indirekt

   //   // 2. === Za svaki sint minus tocke 1. analiticki po RNP a po koeficijentu 'R_PostRasp'.. pazi 'isti' konti ili 'zamjena zadnje znamenka' DUG 
   //   sumDug = sumPot = 0.00M;
   //   foreach(OTPdata otp_rec in TheOtpList) 
   //   {
   //      foreach(ZXC.VvUtilDataPackage kontoSuma in kontaSumeList)
   //      {
   //         konto = TheUC.KSD.Dsc_otp_IsKtoEndSame ? kontoSuma.TheStr1 : kontoSuma.TheStr1.Substring(0, kontoSuma.TheStr1.Length -1) + TheUC.KSD.Dsc_otp_ktoIndirEnd;
   //         dug   = ((kontoSuma.TheDecimal - kontoSuma.TheDecimal2) * otp_rec.R_PostRasp / 100.00M).Ron2();
   //         pot   = 0.00M;
   //         sumDug += dug; sumPot += pot;

   //         if(otp_rec.ProjektCD == TheOtpList   .Last().ProjektCD &&
   //            kontoSuma.TheStr1 == kontaSumeList.Last().TheStr1     ) // last iteration pass 
   //         {
   //            kontraSum = kontaSumeList.Sum(ks => ks.TheDecimal - ks.TheDecimal2);

   //            if(sumDug != kontraSum) // korekcija zaokruzivanja 
   //            {
   //               dug += kontraSum - sumDug; 
   //            }
   //         }

   //         AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);
   //      }
   //   }

   //   #endregion Razrada foreach rnp: foreach konto indirekt

   //   #region Phase 3

   //   // 3. === za savki RNP iznos R_NewTr_razd na kto KSD.Dsc_otp_ktoObrade na DUG
   //   //        a onda to sumarno na KSD.Dsc_otp_ktoPrerspTrsk POT

   //   sumDug = sumPot = 0.00M;
   //   foreach(OTPdata otp_rec in TheOtpList)
   //   {
   //      konto = TheUC.KSD.Dsc_otp_ktoObrade;
   //      dug   = otp_rec.R_NewTr_razd;
   //      pot   = 0.00M;
   //      sumDug += dug; sumPot += pot;

   //      if(otp_rec.ProjektCD == TheOtpList.Last().ProjektCD) // last iteration pass 
   //      {
   //         kontraSum = TheOtpList.Sum(otp => otp.R_NewTr_razd);
                         
   //         if(sumDug != kontraSum) // korekcija zaokruzivanja 
   //         {
   //            dug += kontraSum - sumDug; 
   //         }
   //      }

   //      AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);
   //   }

   //   konto = TheUC.KSD.Dsc_otp_ktoPrerspTrsk;
   //   dug   = 0.00M;
   //   pot   = TheOtpList.Sum(otp => otp.R_NewTr_razd);

   //   AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);

   //   #endregion Phase 3

   //   #region Phase 4

   //   // 4. === za svaki RNP izmos R_OTP_razd na kto KSD.Dsc_otp_ktoObrade na POT
   //   //        a onda to sumarno na 
   //               // a. Ako je KSD.Dsc_otp_IsSkladGotProizv true : na kto KSD.Dsc_otp_ktoGotProiz DUG
   //               // b.                       else               : na kto KSD.Dsc_otp_kto7trosProizv DUG

   //   sumDug = sumPot = 0.00M;
   //   foreach(OTPdata otp_rec in TheOtpList)
   //   {
   //      konto = TheUC.KSD.Dsc_otp_ktoObrade;
   //      dug   = 0.00M;
   //      pot   = otp_rec.R_OTP_razd;
   //      sumDug += dug; sumPot += pot;

   //      if(otp_rec.ProjektCD == TheOtpList.Last().ProjektCD) // last iteration pass 
   //      {
   //         kontraSum = TheOtpList.Sum(otp => otp.R_OTP_razd);
            
   //         if(sumPot != kontraSum) // korekcija zaokruzivanja 
   //         {
   //            pot += kontraSum - sumPot; 
   //         }
   //      }

   //      AutoSetNalogOTP(ref line, nalog_rec, otp_rec.ProjektCD, konto, opis, dug, pot);
   //   }

   //   if(TheUC.KSD.Dsc_otp_IsSkladGotProizv) konto = TheUC.KSD.Dsc_otp_ktoGotProiz;
   //   else                                   konto = TheUC.KSD.Dsc_otp_kto7trosProizv;
      
   //   dug = TheOtpList.Sum(otp => otp.R_OTP_razd);
   //   pot = 0.00M;

   //   AutoSetNalogOTP(ref line, nalog_rec, "", konto, opis, dug, pot);

   //   #endregion Phase 4

   //   AbortButton_Click(null, EventArgs.Empty);

   // //ZXC.TheVvForm.OpenNew_Record_TabPage(ZXC.TheVvForm.GetSubModulXY(ZXC.VvSubModulEnum.NAL_P), ZXC.NalogRec.RecID);
   //   ZXC.TheVvForm.LastRecord_OnClick(null, EventArgs.Empty);
   //}


   #endregion Buttons_clik
}

public partial class AnalizaProizUC : VvOtherUC
{

   #region TheG Util Metodz

   public static void RenumerateLineNumbers(DataGridView dgv, int fromRowIndex)
   {
      if(fromRowIndex == -1) fromRowIndex = 0; // malo tu trkeljimo ali zasada tako. 
      if(fromRowIndex < 0) return;

      for(int rowIndex = fromRowIndex; rowIndex < dgv.Rows.Count; ++rowIndex)
      {
         if(!dgv.Rows[rowIndex].IsNewRow) dgv.Rows[rowIndex].HeaderCell.Value = (rowIndex + 1).ToString();
      }
   }

   protected void UpdateLineCount(VvDataGridView theGrid)
   {
      DataGridView theSumGrid = theGrid.TheLinkedGrid_Sum;

      int idxCorrector;

      if(theGrid.AllowUserToAddRows == false) idxCorrector = 0;
      else                                    idxCorrector = 1;

      theSumGrid.Rows[0].HeaderCell.Value = (theGrid.RowCount - idxCorrector).ToString();
   }

   #endregion TheG Util Metodz

   private ObrProDLG                   TheDLG     { get { return this.Parent as ObrProDLG; } }
   private List</*ObrProDLG.*/OTPdata> TheOtpList { get { return TheDLG.TheOtpList       ; } }

//   /*****/private void Calc_OTP_Raspored(int rowIdx, List</*ObrProDLG.*/OTPdata> otpList)
//   {
//      /*ObrProDLG.*/OTPdata otp_rec = otpList[rowIdx];

//      decimal theOTP, postRasp;
//      decimal planCijSum = otpList.Sum(otp => otp.PlanCijena);
//      decimal rdYearSum  = otpList.Sum(otp => otp.R_RD_year);

//      if(Fld_IsDirekt == true) postRasp = ZXC.DivSafe(otp_rec.R_RD_year , rdYearSum  /** otp_rec.PostDovr / 100.00M*/) * 100.00M;
//      else                     postRasp = ZXC.DivSafe(otp_rec.PlanCijena, planCijSum /** otp_rec.PostDovr / 100.00M*/) * 100.00M;

//      theOTP = Fld_A_IznosREZIJSKIH_Tr * postRasp / 100.00M;

////To modify the struct, first assign it to a local variable, modify the variable, then assign the variable back to the item in the collection.
////
////List<myStruct> list = {…};
////MyStruct ms = list[0];
////ms.Name = "MyStruct42";
////list[0] = ms;  

//      otp_rec.R_TheOTP   = theOTP.Ron2();
//      otp_rec.R_PostRasp = postRasp;

//      otpList[rowIdx] = otp_rec;
//   }

//   private void CalcOnExitPostoDovrseno(object sender, EventArgs e)
//   {
//      VvTextBoxEditingControl vtbec = sender as VvTextBoxEditingControl;
//      //VvDataGridView dgv = (VvDataGridView)vtbec.EditingControlDataGridView;
//      int rowIdx = vtbec.EditingControlRowIndex;

//      GetDgvLineFields1(TheOtpList, rowIdx);
//   }

   private void DisableNalogAction(object sender, EventArgs e)
   {
      if(IsPutKtoShemaDscFields_InProgress) return;

      //TheDLG.UcitajButton_Click(sender, e);
      TheDLG.EnableDisable_tsBtn_Nalog(false);
   }

   #region PutDGVfields

   #region TheG 1

   public void PutDgvFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      int rowIdx, idxCorrector;

      TheGrid_1.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_1);

      if(otpList != null)
      {
         for(rowIdx = 0; rowIdx < otpList.Count; ++rowIdx )  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_1.Rows.Add();

            PutDgvLineFields1(rowIdx, otpList);
         }
      }

      RenumerateLineNumbers(TheGrid_1, 0);

      UpdateLineCount(TheGrid_1);

      PutDgvTransSumFields1(otpList);
   }

   public void PutDgvLineFields1(int rowIdx, List</*ObrProDLG.*/OTPdata> otpList) 
   {
      // Calc_OTP_Raspored(rowIdx, otpList); ... ovdje NE! nego se ranije obavi 'Calc_IATP_Raspored' 

      TheGrid_1.PutCell(ci1.iT_projektCD    , rowIdx, otpList[rowIdx].ProjektCD   );
      TheGrid_1.PutCell(ci1.it_Investit_otp , rowIdx, otpList[rowIdx].Investitor  );
      TheGrid_1.PutCell(ci1.iT_PlanCijena   , rowIdx, otpList[rowIdx].PlanCijena  );
      TheGrid_1.PutCell(ci1.iT_PUT_dug_year , rowIdx, otpList[rowIdx]./*R_*/PUT_PS);
      TheGrid_1.PutCell(ci1.iT_RD_razd      , rowIdx, otpList[rowIdx].RD_razd     );
      TheGrid_1.PutCell(ci1.iT_NIraspored   , rowIdx, otpList[rowIdx].RI_razd     );
      TheGrid_1.PutCell(ci1.iT_NewTr_year   , rowIdx, otpList[rowIdx].R_ukKrajR   );
      TheGrid_1.PutCell(ci1.it_RRraspored   , rowIdx, otpList[rowIdx].R_TheREZIJE );
      TheGrid_1.PutCell(ci1.it_AMraspored   , rowIdx, otpList[rowIdx].R_TheAMORT  );
      TheGrid_1.PutCell(ci1.it_UkTrRN       , rowIdx, otpList[rowIdx].R_ukIATP    );
      TheGrid_1.PutCell(ci1.it_RazlikaPlan  , rowIdx, otpList[rowIdx].R_razlikaPL );


   }

   public void PutDgvTransSumFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      TheSumGrid_1.PutCell(ci1.iT_PlanCijena   , 0, otpList.Sum(otp => otp.PlanCijena  ));
      TheSumGrid_1.PutCell(ci1.iT_PUT_dug_year , 0, otpList.Sum(otp => otp./*R_*/PUT_PS));
      TheSumGrid_1.PutCell(ci1.iT_RD_razd      , 0, otpList.Sum(otp => otp.RD_razd     ));
      TheSumGrid_1.PutCell(ci1.iT_NIraspored   , 0, otpList.Sum(otp => otp.RI_razd     ));
      TheSumGrid_1.PutCell(ci1.iT_NewTr_year   , 0, otpList.Sum(otp => otp.R_NewTr_year));
      TheSumGrid_1.PutCell(ci1.it_RRraspored   , 0, otpList.Sum(otp => otp.R_TheREZIJE ));
      TheSumGrid_1.PutCell(ci1.it_AMraspored   , 0, otpList.Sum(otp => otp.R_TheAMORT  ));
      TheSumGrid_1.PutCell(ci1.it_UkTrRN       , 0, otpList.Sum(otp => otp.R_ukIATP    ));
      TheSumGrid_1.PutCell(ci1.it_RazlikaPlan  , 0, otpList.Sum(otp => otp.R_razlikaPL ));

   
   }

   internal void GetDgvFields1(List</*ObrProDLG.*/OTPdata> otpList)
   {
      for(int rIdx = 0; rIdx < TheGrid_1.RowCount - 1; ++rIdx)
      {
         GetDgvLineFields1(otpList, rIdx);
      }
   }

   private /*ObrProDLG.*/OTPdata GetDgvLineFields1(List</*ObrProDLG.*/OTPdata> otpList, int rowIdx)
   {
      /*ObrProDLG.*/OTPdata otp_rec;

      otp_rec = otpList[rowIdx];
      otpList[rowIdx] = otp_rec;

      return otp_rec;
   }

   #endregion TheG 1

   #region TheG 2

   public void PutDgvFields2(List<Faktur> fakturList)
   {
      int rowIdx, idxCorrector;

      TheGrid_2.Rows.Clear();

      idxCorrector = GetDGVsIdxCorrrector(TheGrid_2);

      if(fakturList != null)
      {
         foreach(Faktur faktur_rec in fakturList)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid_2.Rows.Add();

            rowIdx = TheGrid_2.RowCount - idxCorrector;

            PutDgvLineFields2(faktur_rec, rowIdx, false);
         }
      }

      RenumerateLineNumbers(TheGrid_2, 0);

      UpdateLineCount(TheGrid_2);

      PutDgvTransSumFields2(fakturList);
   }

   public void PutDgvLineFields2(Faktur faktur_rec, int rowIdx, bool skipRecID_andSerial_Columns) 
   {
      if(skipRecID_andSerial_Columns == false)
      {
         TheGrid_2.PutCell(ci2.iT_recID , rowIdx, faktur_rec.RecID);
       //TheGrid_2.PutCell(ci2.iT_serial, rowIdx, faktur_rec.T_serial);
      }

      TheGrid_2.PutCell(ci2.iT_projektCD    , rowIdx, faktur_rec.TT_And_TtNum);
      TheGrid_2.PutCell(ci2.iT_narInv_fa    , rowIdx, faktur_rec.KupdobName);
      TheGrid_2.PutCell(ci2.iT_objektCd_fa  , rowIdx, faktur_rec.PrjArtCD);
      TheGrid_2.PutCell(ci2.iT_objektName_fa, rowIdx, faktur_rec.PrjArtName);
      TheGrid_2.PutCell(ci2.iT_refDok_fa    , rowIdx, faktur_rec.VezniDok);

      if(faktur_rec.PonudDate != DateTimePicker.MinimumDateTime)
         TheGrid_2.PutCell(ci2.iT_dateStart_fa , rowIdx, faktur_rec.PonudDate);
      
      if(faktur_rec.RokIspDate != DateTimePicker.MinimumDateTime)
         TheGrid_2.PutCell(ci2.iT_dateRokIsp_fa, rowIdx, faktur_rec.RokIspDate);

      TheGrid_2.PutCell(ci2.iT_ugCijena_fa  , rowIdx, faktur_rec.SomeMoney);
      TheGrid_2.PutCell(ci2.iT_dovrsenost_fa, rowIdx, faktur_rec.SomePercent);
      TheGrid_2.PutCell(ci2.iT_status_fa    , rowIdx, faktur_rec.StatusCD);
   }

   public void PutDgvTransSumFields2(List<Faktur> fakturList)
   {
      TheSumGrid_2.PutCell(ci2.iT_ugCijena_fa, 0, fakturList.Sum(fak => fak.SomeMoney));
   }

   #endregion TheG 2

   #endregion PutDGVfields

}