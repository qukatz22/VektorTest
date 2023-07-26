using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using static ArtiklDao;
using Microsoft.Office.Interop.Excel;
#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                   MySql.Data.MySqlClient;
using XSqlConnection  = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand     = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
#endif

public partial class MixerDUC : VvPolyDocumRecordUC
{
   // Ak bu gos'n Qukatz tu jos nekaj trebao ... 

   // UrudzbeniDUC ONLY, PutNalDUC! 
   void MixerDUC_Validating(object sender, CancelEventArgs e)
   {
      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible == false) return;

      GetFields(false);

      #endregion Should validate enivej?

      #region IsDocumentFromLockedPeriod

      // 09.02.2016: 

      if(VvDaoBase.IsDocumentFromLockedPeriod(Fld_DokDate.Date, false)) e.Cancel = true;

      #endregion IsDocumentFromLockedPeriod

      // ========================================================== 
      // ========================================================== 
      // ========================================================== 
      // Named DUC Validations:                                     

      if(this is UrudzbeniDUC == false &&
         this is PutNalDUC    == false &&
         this is VirmanDUC    == false &&
         this is UgovoriDUC   == false) return; // !!! 

      #region UrudzbeniDUC

      if(this is UrudzbeniDUC)
      {
         UrudzbeniDUC theUrudzbeniDUC = this as UrudzbeniDUC;

         if(theUrudzbeniDUC.Fld_StrD_32.IsEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Urudžbeni broj ne smije biti prazan.\n\nPopunite ga sa zelenim trokutićem.");
            e.Cancel = true;
         }

         Mixer mixerInFile_rec = new Mixer();

         bool found = ZXC.MixerDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, mixerInFile_rec, theUrudzbeniDUC.Fld_StrD_32, ZXC.MixerSchemaRows[ZXC.MixCI.strD_32], false, true);

         // nasao je vec upotrebljeni urbr, a to nije ovaj doticni record
         if(found && (TheVvTabPage.WriteMode == ZXC.WriteMode.Add || mixer_rec.TT != mixerInFile_rec.TT || mixer_rec.TtNum != mixerInFile_rec.TtNum) && theUrudzbeniDUC.Fld_StrD_32.NotEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Urudžbeni broj je već iskorišten.\n\nOsvježite ga sa zelenim trokutićem.");
            e.Cancel = true;
         }
      } // if(this is UrudzbeniDUC)

      #endregion UrudzbeniDUC

      #region PutNalDUC

    // 31.05.2017. 
    //if(this is PutNalDUC) tu su svi, i loko i prl a treba samo pn 
      if(this is PutNalTuzDUC || this is PutNalInoDUC)
      {
         PutNalDUC thePutNalDUC = this as PutNalDUC;

         if(thePutNalDUC.Fld_DateOd.IsEmpty() || thePutNalDUC.Fld_DateDo.IsEmpty())
         {
            thePutNalDUC.Fld_BrSati = 0;
            thePutNalDUC.Fld_BrDnev = 0;

         }

      } // if(this is PutNalDUC)

      #endregion PutNalDUC

      if(this is UgovoriDUC && ZXC.IsZASTITARI)
      {
         if(mixer_rec.StrH_32.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nZadajte vrstu štićenja prije usnimavanja!"); e.Cancel = true; }
         if(mixer_rec.StrF_64.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA:\n\nZadajte broj ugovora prije usnimavanja!"  ); e.Cancel = true; }
      }

      if(this is VirmanDUC)
      {
         string primIBAN, primNaziv;

         Kupdob kupdob_rec;

         for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
         {
            primIBAN  = TheG.GetStringCell(ci.iT_kpdbZiroB_32, rowIdx, false);
            primNaziv = TheG.GetStringCell(ci.iT_kpdbNameB_50, rowIdx, false);

            if(primIBAN.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "IBAN je prazan!\n\nRedak {0}", rowIdx + 1);

               //e.Cancel = true;
            }
            else
            {
               kupdob_rec = this.Get_Kupdob_FromVvUcSifrar_byIBAN(primIBAN);

               if(kupdob_rec == null)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "IBAN ne postoji u adresaru.\n\n{0}\n\nRedak {1}", primIBAN, rowIdx + 1);

                  //e.Cancel = true;
               }
             //else if(ZXC.CompareStrings(primNaziv, kupdob_rec.Naziv).NotZero()                                                         )
               else if(ZXC.CompareStrings(primNaziv, kupdob_rec.Naziv).NotZero() && primIBAN != VvPlacaReport.standardGovernmentZiroRacun)
               {
                  ZXC.aim_emsg(MessageBoxIcon.Warning, "Za zadani IBAN [{0}]\n\nNaziv primatelja na virmanu\n\n[{1}]\n\nse ne podudara sa nazivom partnera iz adresara.\n\n[{2}]\n\nRedak {3}",
                     primIBAN, primNaziv, kupdob_rec.Naziv, rowIdx + 1);

                  //e.Cancel = true;
               }
            }
         }

      } // if(this is VirmanDUC) 

   } // void MixerDUC_Validating(object sender, CancelEventArgs e) 

}

public class FINA_STAT_Row
{
   public int                   AOP          { get; set; }
   public int                   SUB_AOP      { get; set; }
   public bool                  IsMinus      { get; set; }
   public GFI_TSI_DUC.MoneyKind MoneyKind    { get; set; }
   
   public string  Konto    { get; set; }
   public string  KontoName{ get; set; }

   public decimal PsDug    { get; set; }
   public decimal PsPot    { get; set; }
   public decimal PrmDug   { get; set; }
   public decimal PrmPot   { get; set; }
   public decimal KumDug   { get { return PsDug + PrmDug; } }
   public decimal KumPot   { get { return PsPot + PrmPot; } }
   public decimal SaldoDug { get { return (KumDug - KumPot).IsZeroOrPositive() ? KumDug - KumPot : 0.00M; } }
   public decimal SaldoPot { get { return (KumPot - KumDug).IsPositive      () ? KumPot - KumDug : 0.00M; } }

   public string Pozicija { get; set; }

   public /*decimal*/int TheAOPValue
   {
      get
      {
         decimal theMoney = 0.00M;

         switch(MoneyKind)
         {
            case GFI_TSI_DUC.MoneyKind.kumDug                 : theMoney = KumDug                       ; break;
            case GFI_TSI_DUC.MoneyKind.kumPot                 : theMoney = KumPot                       ; break;
            case GFI_TSI_DUC.MoneyKind.saldoDug_minus_saldoPot: theMoney = Math.Abs(SaldoDug - SaldoPot); break;
            case GFI_TSI_DUC.MoneyKind.prmDug                 : theMoney = PrmDug                       ; break;
            case GFI_TSI_DUC.MoneyKind.prmPot                 : theMoney = PrmPot                       ; break;}

         return IsMinus ? (int)(-theMoney.Ron(0)) : (int)(theMoney.Ron(0));
      }
   }

   public string MoneyKindStr 
   {
      get
      {
         switch(MoneyKind)
         {
             case GFI_TSI_DUC.MoneyKind.saldoDug_minus_saldoPot: return "S";
             case GFI_TSI_DUC.MoneyKind.kumDug                 : return "D";
             case GFI_TSI_DUC.MoneyKind.kumPot                 : return "P";
             case GFI_TSI_DUC.MoneyKind.prmDug                 : return "d";
             case GFI_TSI_DUC.MoneyKind.prmPot                 : return "p";
         }
         return "S";
      }
   }

   public FINA_STAT_Row(string kontoWnaziv, int aop, int subAop, string pozicija, bool isMinus, GFI_TSI_DUC.MoneyKind moneyKind, decimal psDug, decimal psPot, decimal prmDug, decimal prmPot)
   {
      string[] kompozicija = kontoWnaziv.Split('-');

      this.Konto     = kompozicija[0];
      this.KontoName = kompozicija[1];
      this.AOP       = aop           ;
      this.SUB_AOP   = subAop        ;
      this.Pozicija  = pozicija      ;
      this.IsMinus   = isMinus       ;
      this.MoneyKind = moneyKind     ;
      this.PsDug     = psDug         ;
      this.PsPot     = psPot         ;
      this.PrmDug    = prmDug        ;
      this.PrmPot    = prmPot        ;
   }

   public override string ToString()
   {
      return AOP.ToString("000") + "-" + SUB_AOP.ToString() + "/" + IsMinus + "/" + MoneyKind + "/" + Konto;
   }

}

public class GFI_TSI_BilancaRow
{
   public int                   AOP          { get; set; }
   public int                   SUB_AOP      { get; set; }
   public bool                  IsMinus      { get; set; }
   public GFI_TSI_DUC.MoneyKind MoneyKind    { get; set; }
   
   public string  Konto    { get; set; }
   public string  KontoName{ get; set; }

   public decimal PsDug    { get; set; }
   public decimal PsPot    { get; set; }
   public decimal PrmDug   { get; set; }
   public decimal PrmPot   { get; set; }
   public decimal KumDug   { get { return PsDug + PrmDug; } }
   public decimal KumPot   { get { return PsPot + PrmPot; } }
   public decimal SaldoDug { get { return (KumDug - KumPot).IsZeroOrPositive() ? KumDug - KumPot : 0.00M; } }
   public decimal SaldoPot { get { return (KumPot - KumDug).IsPositive      () ? KumPot - KumDug : 0.00M; } }

   public string Pozicija { get; set; }

   public /*decimal*/int TheAOPValue
   {
      get
      {
         decimal theMoney = 0.00M;

         switch(MoneyKind)
         {
            case GFI_TSI_DUC.MoneyKind.kumDug                 : theMoney = KumDug                       ; break;
            case GFI_TSI_DUC.MoneyKind.kumPot                 : theMoney = KumPot                       ; break;
            case GFI_TSI_DUC.MoneyKind.saldoDug_minus_saldoPot: theMoney = Math.Abs(SaldoDug - SaldoPot); break;
            case GFI_TSI_DUC.MoneyKind.prmDug                 : theMoney = PrmDug                       ; break;
            case GFI_TSI_DUC.MoneyKind.prmPot                 : theMoney = PrmPot                       ; break;}

         return IsMinus ? (int)(-theMoney.Ron(0)) : (int)(theMoney.Ron(0));
      }
   }

   public string MoneyKindStr 
   {
      get
      {
         switch(MoneyKind)
         {
             case GFI_TSI_DUC.MoneyKind.saldoDug_minus_saldoPot: return "S";
             case GFI_TSI_DUC.MoneyKind.kumDug                 : return "D";
             case GFI_TSI_DUC.MoneyKind.kumPot                 : return "P";
             case GFI_TSI_DUC.MoneyKind.prmDug                 : return "d";
             case GFI_TSI_DUC.MoneyKind.prmPot                 : return "p";
         }
         return "S";
      }
   }

   public GFI_TSI_BilancaRow(string kontoWnaziv, int aop, int subAop, string pozicija, bool isMinus, GFI_TSI_DUC.MoneyKind moneyKind, decimal psDug, decimal psPot, decimal prmDug, decimal prmPot)
   {
      string[] kompozicija = kontoWnaziv.Split('-');

      this.Konto     = kompozicija[0];
      this.KontoName = kompozicija[1];
      this.AOP       = aop           ;
      this.SUB_AOP   = subAop        ;
      this.Pozicija  = pozicija      ;
      this.IsMinus   = isMinus       ;
      this.MoneyKind = moneyKind     ;
      this.PsDug     = psDug         ;
      this.PsPot     = psPot         ;
      this.PrmDug    = prmDug        ;
      this.PrmPot    = prmPot        ;
   }

   public override string ToString()
   {
      return AOP.ToString("000") + "-" + SUB_AOP.ToString() + "/" + IsMinus + "/" + MoneyKind + "/" + Konto;
   }

}

public partial class GFI_TSI_DUC : MixerDUC
{
   public List<GFI_TSI_BilancaRow> GFI_TSI_BilancaRows;

   protected List<string> AllTokensKontaList;

   public enum MoneyKind 
   { 
      /* d    */ prmDug, 
      /* p    */ prmPot, 
      /* D    */ kumDug, 
      /* P    */ kumPot, 
      /* S, s */ saldoDug_minus_saldoPot,
   }

   protected bool Is_FINA_Statistika { get; set; }

   protected void ExternLink1Exit(object sender, EventArgs e)
   {
      if(Fld_ExternLink2.IsEmpty()) Fld_ExternLink2 = Fld_ExternLink1.Replace(".xls", "_" + ZXC.CURR_prjkt_rec.Ticker + "_.xls");
   }

   #region About DataSet

   protected Vektor.DataLayer.DS_Reports.DS_Dnevnik ds_Dnevnik;

   public void GFI_TSI_LoadBilancaData()
   {
      InitList_And_FillDataSet(TheDbConnection, /*Is_FINA_Statistika ? mixer_rec.DokDate :*/ mixer_rec.DateA);

      foreach(Xtrans xtrans_rec in mixer_rec.Transes)
      {
         Add_ThisAOP_KontoList(xtrans_rec, ds_Dnevnik, GFI_TSI_BilancaRows, false, false, "", Is_FINA_Statistika);
      }

      DisposeDataSet();
   }

   protected int InitList_And_FillDataSet(XSqlConnection conn, DateTime dateDO)
   {
      SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.Konto);
      
      GFI_TSI_BilancaRows = new List<GFI_TSI_BilancaRow>();

      AllTokensKontaList = new List<string>();

      ds_Dnevnik = new Vektor.DataLayer.DS_Reports.DS_Dnevnik();

      int norr = 0;

      using(XSqlCommand mySelectCommand = GetReportCommand(conn, dateDO))
      {
         if(mySelectCommand != null)
         {
            using(XSqlDataAdapter myDataAdapter = new XSqlDataAdapter(mySelectCommand))
            {
               norr = myDataAdapter.Fill(ds_Dnevnik, ZXC.vvReport_TableName);
            }
         }
      }

      return norr;
   }

   protected void DisposeDataSet()
   {
      this.ds_Dnevnik.Dispose();
   }

   protected XSqlCommand GetReportCommand(XSqlConnection conn, DateTime dateDO)
   {
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      if(dateDO.NotEmpty()) filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtransDao.CI.t_dokDate], false, "elDateDO", dateDO, "", "", " <= ", ""));

      cmd.CommandText =

         "SELECT       t_konto, \n" +
         "SUM(IF(t_tt  = '" + Nalog.PS_TT + "', t_dug, 0)) AS ps_dug, \n" +
         "SUM(IF(t_tt  = '" + Nalog.PS_TT + "', t_pot, 0)) AS ps_pot, \n" +
         "SUM(IF(t_tt != '" + Nalog.PS_TT + "', t_dug, 0)) AS  t_dug, \n" +
         "SUM(IF(t_tt != '" + Nalog.PS_TT + "', t_pot, 0)) AS  t_pot  \n" +

         "FROM " + Ftrans.recordName +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +
         
         "GROUP BY " + Ftrans.KplanForeignKey;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return cmd;
   }

   #endregion About DataSet

   /*private*/ public static void Add_ThisAOP_KontoList(Xtrans xtrans_rec, Vektor.DataLayer.DS_Reports.DS_Dnevnik local_ds_Dnevnik, List<GFI_TSI_BilancaRow> local_GFI_TSI_BilancaRows, bool isByMtros, bool isByFond, string _fond, bool Is_FINA_Statistika) // VOILA 
   {
      if(xtrans_rec == null) return;

      // 16.06.2015: 
      if(Is_FINA_Statistika && xtrans_rec.T_strA_2 == "S") // S - sinteticki redak 
      {
         return;
      }

      string theRule  = xtrans_rec.T_kpdbNameA_50; // konto ili korjen konta + moneyKind definition 
      uint   mtros_cd = xtrans_rec.T_kupdobCD    ;

      if(theRule.IsEmpty()) return;

      string[]  rawTokens = theRule.Replace(" ", "").Split(','), ktoRoots;
      string    rangeStr = "do", cleanToken;

      bool      isMinus     ;
      MoneyKind moneyKind   ;

      IEnumerable<string> thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List;

      int subAop = 0;

      foreach(string rawToken in rawTokens)
      {
         // minus or plus manager 
         if(rawToken.Contains('-')) 
         {
            if(rawToken.IndexOf('-') != 0)
            {
               isMinus    = false; 
               cleanToken = rawToken.Replace("-", "do");

               ZXC.aim_emsg(MessageBoxIcon.Warning, "Minus mora biti na prvom mjestu!\n\nOvoga pretvaram u 'do' (oznaku ranga)!\n\n[{0}] ---> [{1}]", rawToken, cleanToken);
            }
            else
            {
               isMinus    = true; 
               cleanToken = rawToken.Replace("-", "");
            }
         }
         else
         { 
            isMinus    = false; 
            cleanToken = rawToken;                  
         }

         // MoneyKind manager 
         if(Char.IsLetter(ZXC.GetStringsLastChar(cleanToken)) == false) // ako na kraju tokena nema S, s, d, p, D, P onda je default 'S'
         {
            moneyKind = MoneyKind.saldoDug_minus_saldoPot;
         }
         else
         {
            switch(ZXC.GetStringsLastChar(cleanToken))
            {
               case 'S':
               case 's': moneyKind = MoneyKind.saldoDug_minus_saldoPot; break; // Math.Abs(SaldoDug - SaldoPot) 
               case 'D': moneyKind = MoneyKind.kumDug                 ; break;
               case 'P': moneyKind = MoneyKind.kumPot                 ; break;
               case 'd': moneyKind = MoneyKind.prmDug                 ; break;
               case 'p': moneyKind = MoneyKind.prmPot                 ; break;

               default : ZXC.aim_emsg(MessageBoxIcon.Warning, "Zadnja znamenka pravila [{0}] je nepoznata.\n\nPostavljam na 'S' (saldoDug - saldoPot)!", cleanToken); 
                         moneyKind = MoneyKind.saldoDug_minus_saldoPot; break;
            }

            cleanToken = cleanToken.TrimEnd(ZXC.GetStringsLastChar(cleanToken));
         }

         // kontaList manager 
         if(cleanToken.Contains(rangeStr)) // zadan je rang. npr. -12do14
         {
            ktoRoots = cleanToken.Split(rangeStr.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if(ktoRoots.Length > 2) ZXC.aim_emsg(MessageBoxIcon.Error, "Krivo zadani rang[{0}]!", cleanToken);

            subAop++;

            if(isByMtros)
            {
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar
                  .Where(kpl => kpl.Tip == "A" && 
                                 ZXC.CompareStrings(kpl.Konto, ktoRoots[0]).IsZeroOrPositive() && 
                                (ZXC.CompareStrings(kpl.Konto, ktoRoots[1]).IsZeroOrNegative() || kpl.Konto.StartsWith(ktoRoots[1]))
                         )
                  .OrderBy(kpl => kpl.Konto)
                  .Select (kpl => kpl.Konto + "-" + mtros_cd.ToString("000000"))
                  /*.ToList()*/;
            }
            else if(isByFond)
            {
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar
                  .Where(kpl => kpl.Tip == "A" &&
                                 ZXC.CompareStrings(kpl.Konto, ktoRoots[0]).IsZeroOrPositive() &&
                                (ZXC.CompareStrings(kpl.Konto, ktoRoots[1]).IsZeroOrNegative() || kpl.Konto.StartsWith(ktoRoots[1]))
                         )
                  .OrderBy(kpl => kpl.Konto)
                  .Select(kpl => kpl.Konto + "-" + _fond)
                  /*.ToList()*/;
            }
            else
            {
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar
                  .Where(kpl => kpl.Tip == "A" && 
                                 ZXC.CompareStrings(kpl.Konto, ktoRoots[0]).IsZeroOrPositive() && 
                                (ZXC.CompareStrings(kpl.Konto, ktoRoots[1]).IsZeroOrNegative() || kpl.Konto.StartsWith(ktoRoots[1]))
                         )
                  .OrderBy(kpl => kpl.Konto)
                  .Select (kpl => kpl.Konto + "-" + kpl.Naziv)
                  /*.ToList()*/;
            }
         }
         else // classic, kotno root
         {
            subAop++;

            if(isByMtros)     thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar.Where(kpl => kpl.Tip == "A" && kpl.Konto.StartsWith(cleanToken)).Select(kpl => kpl.Konto + "-" + mtros_cd.ToString("000000"));
            else if(isByFond) thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar.Where(kpl => kpl.Tip == "A" && kpl.Konto.StartsWith(cleanToken)).Select(kpl => kpl.Konto + "-" + _fond                      );
            else              thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List = KplanSifrar.Where(kpl => kpl.Tip == "A" && kpl.Konto.StartsWith(cleanToken)).Select(kpl => kpl.Konto + "-" + kpl.Naziv                  );
         }

         if(isByMtros) // news in 2015 for plan - hztk 
         {
            var thisTokenBilancaRows = 
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List
               .Join(local_ds_Dnevnik.IzvjTable, 
                  /* key 1 */ kontoWmtrosCd => kontoWmtrosCd, 
                  /* key 2 */ dt            => dt.t_konto + "-" + dt.t_mtros_cd.ToString("000000"), 
               (kWm, dt) =>
                  new GFI_TSI_BilancaRow(kWm, xtrans_rec.T_intA, subAop, xtrans_rec.T_kpdbZiroA_32, isMinus, moneyKind, dt.ps_dug, dt.ps_pot, dt.t_dug, dt.t_pot));

            local_GFI_TSI_BilancaRows.AddRange(thisTokenBilancaRows);
         }
         else if(isByFond) // news in 2015 for plan - kerempuh 
         {
            var thisTokenBilancaRows = 
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List
               .Join(local_ds_Dnevnik.IzvjTable, 
                  /* key 1 */ kontoWfond => kontoWfond, 
                  /* key 2 */ dt         => dt.t_konto + "-" + (dt.t_fond.NotEmpty() ? dt.t_fond : "A"), // dojebi t_fond; ako je prazan kao da je 'A' 
               (kWf, dt) =>
                  new GFI_TSI_BilancaRow(kWf, xtrans_rec.T_intA, subAop, xtrans_rec.T_kpdbZiroA_32, isMinus, moneyKind, dt.ps_dug, dt.ps_pot, dt.t_dug, dt.t_pot));

            local_GFI_TSI_BilancaRows.AddRange(thisTokenBilancaRows);
         }
         else // old classic for GFI 
         {
            var thisTokenBilancaRows = 
               thisToken_KontoWithNaziv_Or_MtrosCd_Or_Fond_List
             //.Join(      ds_Dnevnik.IzvjTable, kontoWnaziv => kontoWnaziv.Split('-')[0], dt => dt.t_konto, (kWn, dt) =>
               .Join(local_ds_Dnevnik.IzvjTable, kontoWnaziv => kontoWnaziv.Split('-')[0], dt => dt.t_konto, (kWn, dt) =>
                  new GFI_TSI_BilancaRow(kWn, xtrans_rec.T_intA, subAop, xtrans_rec.T_kpdbZiroA_32, isMinus, moneyKind, dt.ps_dug, dt.ps_pot, dt.t_dug, dt.t_pot));

            local_GFI_TSI_BilancaRows.AddRange(thisTokenBilancaRows);
         }
      } // foreach(string rawToken in rawTokens) 

      return;
   }

   public override void PutDgvLineResultsFields1(int rowIdx, VvTransRecord trans_rec, bool passPtrResultsToZaglavljeTranses)
   {
      Xtrans xtrans_rec = trans_rec as Xtrans;

      // 1606.2015: 4
      if(xtrans_rec.T_strA_2 == "S") return;

      xtrans_rec./*T*/R_moneyB = GetTheAOPValue(xtrans_rec);

      TheG.PutCell(ci.iT_moneyB, rowIdx, xtrans_rec./*T*/R_moneyB);

   }

   public /*decimal*/ int GetTheAOPValue(Xtrans xtrans_rec)
   {
      // Ako je Tip redka 'N' znaci da podatak nije iz Bilance nego je literal/konstanta/neizvedeni podatak kojega doslovno upisujemo u 'TheRule' kolonu 
      // 17.06.2015. ako je tip redka "O" ne desava se nista - to je samo opisni redak koji nema nikakav iznos                                           
      if(xtrans_rec.T_strA_2.StartsWith("N") || xtrans_rec.T_strA_2.StartsWith("O")) return ZXC.ValOrZero_Int(xtrans_rec.T_kpdbNameA_50) /* theRule */;
      else                                                                           return GFI_TSI_BilancaRows.Where(gfi_tsi => gfi_tsi.AOP == xtrans_rec.T_intA).Sum(gfi_tsi => gfi_tsi.TheAOPValue);
   }

   public void Load_GFI_TSI_FromLookUpList(VvLookUpLista theLookUpList)
   {
      Mixer  mixer_rec  = new Mixer ();
      Xtrans xtrans_rec = new Xtrans();

      ushort line = 0;

      switch(theLookUpList.Title)
      {                                                                 // sheetName                      offset                     
         case ZXC.luiListaTSI_Podaci_Name : mixer_rec.TT = Mixer.TT_TSI; mixer_rec.StrC_32 = "Podaci" ; mixer_rec.IntA =  2; break;
         case ZXC.luiListaGFI_Bilanca_Name: mixer_rec.TT = Mixer.TT_GFI; mixer_rec.StrC_32 = "Bilanca"; mixer_rec.IntA =  3; break;
         case ZXC.luiListaGFI_RDG_Name    : mixer_rec.TT = Mixer.TT_GFI; mixer_rec.StrC_32 = "RDG"    ; mixer_rec.IntA =  3; break;
         case ZXC.luiListaGFI_PodDop_Name : mixer_rec.TT = Mixer.TT_GFI; mixer_rec.StrC_32 = "PodDop" ; mixer_rec.IntA =  2; break;
         case ZXC.luiListaGFI_NT_I_Name   : mixer_rec.TT = Mixer.TT_GFI; mixer_rec.StrC_32 = "NT_I"   ; mixer_rec.IntA =  3; break;
         case ZXC.luiListaGFI_NT_D_Name   : mixer_rec.TT = Mixer.TT_GFI; mixer_rec.StrC_32 = "NT_D"   ; mixer_rec.IntA =  3; break;
      }

      mixer_rec.DokDate  = DateTime.Now;
      mixer_rec.Napomena = mixer_rec.StrC_32;

      foreach(VvLookUpItem lui in theLookUpList)
      {
         xtrans_rec.Memset0(0);

         xtrans_rec.T_kpdbUlBrA_32 = lui.Cd     ; // Cell XY - 
         xtrans_rec.T_intA         = lui.Integer; // AOP     - 
         xtrans_rec.T_opis_128     = ZXC.LenLimitedStr(lui.Name, 128); // NazivPozicije - 
         xtrans_rec.T_strA_2       = lui.Number.IsZero() ? "" : lui.Number == 1.00M ? "S" : "N"  ; // Tip     - 

         MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
      }

   }

   internal void GFI_TSI_SINT_BilancaData(List<Xtrans> xtransList)
   {
      /* phase A */ xtransList.Where(x => x.T_strA_2 == "S").Reverse().ToList().ForEach(x => FINA_Stat_SINT_BilancaData(xtransList, x, "A"));

      // za [case "9": pravilo =  "x"; break; // suma koja ovisi o 'višoj' sume (jer je xtransList.Reverse()), pa ju treba naknadno resumirati ] 
      /* phase B */ xtransList.Where(x => x.T_kpdbNameA_50.Contains("x")).ToList().ForEach(x => FINA_Stat_SINT_BilancaData(xtransList, x, "B"));

      // sredi AOP01 - AOP02 ili AOP02 - AOP01 ... tako da iz zrcalnih blizanaca negativna brojka nestane 
      /* phase C */ KillNegativeTwins(xtransList);

      // za [case "8": pravilo =  "y"; break; // kokos jaje 
      /* phase D */ xtransList.Where(x => x.T_kpdbNameA_50.Contains("y")).ToList().ForEach(x => FINA_Stat_SINT_BilancaData(xtransList, x, "D"));

      // sredi AOP01 - AOP02 ili AOP02 - AOP01 ... tako da iz zrcalnih blizanaca negativna brojka nestane 
      /* phase E */ KillNegativeTwins(xtransList); // agejn, jer si tek u 'y' D -fazi dobio nove blizance, a njih si dobio tek nakon C - faze koji su pak morali biti pozvani nakon B - faze 

      foreach(Xtrans xtrans_rec in xtransList.Where(x => x.T_strA_2 == "S").Reverse())
      {
         TheG.PutCell(ci.iT_moneyA, /*rowIdx*/xtrans_rec.T_serial - 1, xtrans_rec./*T*/R_moneyA);
         TheG.PutCell(ci.iT_moneyB, /*rowIdx*/xtrans_rec.T_serial - 1, xtrans_rec./*T*/R_moneyB);
      }
   }

   private static void KillNegativeTwins(List<Xtrans> xtransList)
   {
      for(int i = 0; i < xtransList.Count; ++i)
      {
         if(xtransList[i].R_moneyA.IsNegative())
         {
            if(i > 0                && -xtransList[i - 1].R_moneyA == xtransList[i].R_moneyA) xtransList[i].R_moneyA = 0.00M;
            if(i < xtransList.Count && -xtransList[i + 1].R_moneyA == xtransList[i].R_moneyA) xtransList[i].R_moneyA = 0.00M;
         }

         if(xtransList[i].R_moneyB.IsNegative())
         {
            if(i > 0                && -xtransList[i - 1].R_moneyB == xtransList[i].R_moneyB) xtransList[i].R_moneyB = 0.00M;
            if(i < xtransList.Count && -xtransList[i + 1].R_moneyB == xtransList[i].R_moneyB) xtransList[i].R_moneyB = 0.00M;
         }
      }
   }

   private void FINA_Stat_SINT_BilancaData(List<Xtrans> xtransList, Xtrans xtrans_rec, string phase)
   {
      #region Init stuff

      string opis = xtrans_rec.T_opis_128;
      string aopSumStart = "(AOP ";

      if(opis.Contains(aopSumStart) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "[{0}]\n\n{1}\n\nNema '{2}'", opis, xtrans_rec, aopSumStart);
      }

      int idxOfOpenZagrada  = opis.IndexOf(aopSumStart);
      int idxOfCloseZagrada = opis.IndexOf(")");
      string zagrada = opis.Substring(idxOfOpenZagrada, opis.Length - idxOfOpenZagrada).Replace(" ", "").Replace(")", "").Replace("(AOP", "");

      bool isRange = zagrada.ToLower().Contains("do");

      xtrans_rec.T_moneyA = xtrans_rec.T_moneyB = 0.00M;

      #endregion Inigt stuff

      if(isRange) SetFINA_MoneyAiB_ViaRange (zagrada, xtransList, xtrans_rec, phase);
      else        SetFINA_MoneyAiB_Tokenized(zagrada, xtransList, xtrans_rec, phase);
   }

   private void SetFINA_MoneyAiB_ViaRange(string zagrada, List<Xtrans> xtransList, Xtrans xtrans_rec, string phase)
   {
      string[] AOPgraniceStrArray = zagrada.Split(new string[] { "do" }, StringSplitOptions.None);
      
      int startAOP = ZXC.ValOrZero_Int(AOPgraniceStrArray[0]);
      int endAOP   = ZXC.ValOrZero_Int(AOPgraniceStrArray[1]);

      decimal aopAnaliticMoneyA, aopAnaliticMoneyB;

      Xtrans analiticXtrans_rec;

      xtrans_rec./*T*/R_moneyA =
      xtrans_rec./*T*/R_moneyB = 0.00M;

      for(int aop = endAOP; aop >= startAOP; aop--) 
      {
         analiticXtrans_rec = xtransList.Single(x => x.T_intA == aop);

         if(analiticXtrans_rec.T_strA_2 == "S") // suma od sume 
         aopAnaliticMoneyA = analiticXtrans_rec./*T*/R_moneyA;
         else
         aopAnaliticMoneyA = analiticXtrans_rec./*T*/T_moneyA;

         aopAnaliticMoneyB = analiticXtrans_rec./*T*/R_moneyB;

         xtrans_rec./*T*/R_moneyA += aopAnaliticMoneyA;
         xtrans_rec./*T*/R_moneyB += aopAnaliticMoneyB;
      }
   }

   private void SetFINA_MoneyAiB_Tokenized(string zagrada, List<Xtrans> xtransList, Xtrans xtrans_rec, string phase)
   {
      bool isMinusToken; int aop;
      decimal aopAnaliticMoneyA, aopAnaliticMoneyB;

      string[] AOPtokenStrArray = zagrada.Replace("-", "+-").Split(new char[] { '+' }, StringSplitOptions.None);

      Xtrans analiticXtrans_rec;

      xtrans_rec./*T*/R_moneyA =
      xtrans_rec./*T*/R_moneyB = 0.00M;

      foreach(string aopStr in AOPtokenStrArray)
      {
         if(aopStr.StartsWith("-")) isMinusToken = true ;
         else                       isMinusToken = false;

         aop = ZXC.ValOrZero_Int(aopStr.Replace("-", ""));

         analiticXtrans_rec = xtransList.Single(x => x.T_intA == aop);

         if(analiticXtrans_rec.T_strA_2 == "S") // suma od sume 
         aopAnaliticMoneyA = analiticXtrans_rec./*T*/R_moneyA;
         else
         aopAnaliticMoneyA = analiticXtrans_rec./*T*/T_moneyA;

         aopAnaliticMoneyB = analiticXtrans_rec./*T*/R_moneyB;

         if(isMinusToken) // (004+008-017) ... ovaj aopstr je npr '-017'
         {
            xtrans_rec./*T*/R_moneyA -= aopAnaliticMoneyA;
            xtrans_rec./*T*/R_moneyB -= aopAnaliticMoneyB;
         }
         else // (004+008-017) ... ovaj aopstr je npr '004'
         {
            xtrans_rec./*T*/R_moneyA += aopAnaliticMoneyA;
            xtrans_rec./*T*/R_moneyB += aopAnaliticMoneyB;
         }
      }
   }

}

public /*struct*/ class VvManyDecimalsReportSourceRow
{
   #region Propertiz

   public uint     RowUintCD   { get; set; }
   public string   RowStringCD { get; set; }
   public string   RowName     { get; set; }

   public decimal  DecimA01    { get; set; }
   public decimal  DecimA02    { get; set; }
   public decimal  DecimA03    { get; set; }
   public decimal  DecimA04    { get; set; }
   public decimal  DecimA05    { get; set; }
   public decimal  DecimA06    { get; set; }
   public decimal  DecimA07    { get; set; }
   public decimal  DecimA08    { get; set; }
   public decimal  DecimA09    { get; set; }
   public decimal  DecimA10    { get; set; }
   public decimal  DecimA11    { get; set; }
   public decimal  DecimA12    { get; set; }
   public decimal  DecimA13    { get; set; }
   public decimal  DecimA14    { get; set; }
   public decimal  DecimA15    { get; set; }
   public decimal  DecimA16    { get; set; }
   public decimal  DecimA17    { get; set; }
   public decimal  DecimA18    { get; set; }
   public decimal  DecimA19    { get; set; }
   public decimal  DecimA20    { get; set; }
   public decimal  DecimA21    { get; set; }
                               
   public DateTime Date_1      { get; set; }
   public DateTime Date_2      { get; set; }
   public DateTime Date_3      { get; set; }
                               
   public bool     IsXxx       { get; set; }
   public bool     IsYyy       { get; set; }
   public string   TheStr      { get; set; }

   //public decimal R_MVR_PFS_all { get { return this.DecimA03; } }

   //public decimal R_MVR_PFS_priznati
   //{
   //   get
   //   {
   //      return this.DecimA03 > Placa.MAXdozvoljeniPrekovrSati ? Placa.MAXdozvoljeniPrekovrSati : DecimA03;
   //   }
   //}

   //public decimal R_MVR_PFS_visak
   //{
   //   get
   //   {
   //      return visakPrekovrSati = prekovrSati - MAXdozvoljeniPrekovrSati;
   //   }
   //}

   #endregion Propertiz

   #region Constructors

   public VvManyDecimalsReportSourceRow(string vrCD, decimal sati, VvLookUpLista MVRlokupList, string rbrDana, uint personCD, string imePrezime)
   {
      VvLookUpItem lui;

      if(vrCD.IsEmpty()) lui = new VvLookUpItem("", "Redovan Rad", 1M /* !!! */, false, 0, DateTime.MinValue, 0, "");
      else               lui = MVRlokupList.GetLuiForThisCd(vrCD);

      if(lui == null)
      {
         ZXC.aim_emsg("Ne mogu upariti vrstu rada [{0}]\n\nsa retkom LookupListe {1}", vrCD, ZXC.luiListaMixRadVrijemeMVR_Name);
         return;
      }

      this.RowStringCD = rbrDana   ;
      this.RowUintCD   = personCD  ;
      this.RowName     = imePrezime;

      uint rbrReportKolone = (uint)lui.Number;

      switch(rbrReportKolone)
      {
         case 01: this.DecimA01 = sati; break;
         case 02: this.DecimA02 = sati; break;
         case 03: this.DecimA03 = sati; break;
         case 04: this.DecimA04 = sati; break;
         case 05: this.DecimA05 = sati; break;
         case 06: this.DecimA06 = sati; break;
         case 07: this.DecimA07 = sati; break;
         case 08: this.DecimA08 = sati; break;
         case 09: this.DecimA09 = sati; break;
         case 10: this.DecimA10 = sati; break;
         case 11: this.DecimA11 = sati; break;
         case 12: this.DecimA12 = sati; break;
         case 13: this.DecimA13 = sati; break;
         case 14: this.DecimA14 = sati; break;
         case 15: this.DecimA15 = sati; break;
         case 16: this.DecimA16 = sati; break;
         case 17: this.DecimA17 = sati; break;
         case 18: this.DecimA18 = sati; break;
         case 19: this.DecimA19 = sati; break;
         case 20: this.DecimA20 = sati; break;
      }
      
   }

   public VvManyDecimalsReportSourceRow()
   {
   }

   public VvManyDecimalsReportSourceRow(uint rowUintCD, string rowStringCD, string rowName, 
      decimal decimA01,
      decimal decimA02,
      decimal decimA03,
      decimal decimA04,
      decimal decimA05,
      decimal decimA06,
      decimal decimA07,
      decimal decimA08,
      decimal decimA09,
      decimal decimA10,
      decimal decimA11,
      decimal decimA12,
      decimal decimA13,
      decimal decimA14,
      decimal decimA15,
      decimal decimA16,
      decimal decimA17,
      decimal decimA18,
      decimal decimA19,
      decimal decimA20
      )
   {
      this.RowUintCD   = rowUintCD  ;
      this.RowStringCD = rowStringCD;
      this.RowName     = rowName    ;

      this.DecimA01 = decimA01;
      this.DecimA02 = decimA02;
      this.DecimA03 = decimA03;
      this.DecimA04 = decimA04;
      this.DecimA05 = decimA05;
      this.DecimA06 = decimA06;
      this.DecimA07 = decimA07;
      this.DecimA08 = decimA08;
      this.DecimA09 = decimA09;
      this.DecimA10 = decimA10;
      this.DecimA11 = decimA11;
      this.DecimA12 = decimA12;
      this.DecimA13 = decimA13;
      this.DecimA14 = decimA14;
      this.DecimA15 = decimA15;
      this.DecimA16 = decimA16;
      this.DecimA17 = decimA17;
      this.DecimA18 = decimA18;
      this.DecimA19 = decimA19;
      this.DecimA20 = decimA20;
   }

   public VvManyDecimalsReportSourceRow(
      DateTime date1, DateTime date2, DateTime date3, 
      uint rowUintCD, string rowStringCD, string rowName, 
      decimal decimA01,
      decimal decimA02,
      decimal decimA03,
      decimal decimA04,
      decimal decimA05,
      decimal decimA06,
      decimal decimA07,
      decimal decimA08,
      decimal decimA09,
      decimal decimA10,
      decimal decimA11,
      decimal decimA12,
      decimal decimA13,
      decimal decimA14,
      decimal decimA15,
      decimal decimA16,
      decimal decimA17,
      decimal decimA18,
      decimal decimA19,
      decimal decimA20
      ) : this(rowUintCD, rowStringCD, rowName, 
               decimA01,
               decimA02,
               decimA03,
               decimA04,
               decimA05,
               decimA06,
               decimA07,
               decimA08,
               decimA09,
               decimA10,
               decimA11,
               decimA12,
               decimA13,
               decimA14,
               decimA15,
               decimA16,
               decimA17,
               decimA18,
               decimA19,
               decimA20) // : this( ... 
   {
      this.Date_1 = date1;
      this.Date_2 = date2;
      this.Date_3 = date3;
   }

   #endregion Constructors

   public override string ToString()
   {
      // todo: 
      return base.ToString();
   }

}

public class PozicijaPlana
{

   #region Constructors

   public PozicijaPlana() : this("", "", 0, DateTime.Now, 0.00M)
   {
   }

   public PozicijaPlana(string pozCD, string pozName, int luiInteger, DateTime onThisDay, decimal pozLook) : base()
   {
      this.PozCD      = pozCD     ;
      this.PozName    = pozName   ;
      this.LuiInteger = luiInteger;
      this.OnThisDay  = onThisDay ;
      this.PozLook    = pozLook   ;

      //this.currentData = new MixerStruct();
      //
      //Memset0(ID);
   }

   public override string ToString()
   {
      return PozCD + " p1: " + Pln1.ToString0Vv() + " pZ: " + PlnZ.ToString0Vv() + " rZ: " + RblZ.ToString0Vv() + " d: " + OnThisDay.ToString(ZXC.VvDateFormat);
   }

   #endregion Constructors

   #region propertiz

   public string  PozCD   { get; set; }
   public string  PozName { get; set; }
   public decimal PozLook { get; set; }

   public DateTime OnThisDay  { get; set; }
   public int      LuiInteger { get; set; }

   // 2znamenkasti integer xy: x - isAnalitika za PLAN, y - isAnalitika za REALIZACIJU 
   // znaci da bi doaso na ovaj PlanDUC, mora biti x=1 (10 ili 11)                     
   public bool IsPlanANA   { get { return LuiInteger >= 10;                      } }
 //public bool IsRealizANA { get { return LuiInteger == 11;                      } } 27.02.2015.
   public bool IsRealizANA { get { return (LuiInteger == 11 || LuiInteger == 1); } }
   public bool IsPlanSIN   { get { return !IsPlanANA;                            } }
   public bool IsRealizSIN { get { return !IsRealizANA;                          } }

   public bool IsPrihod    { get { return PozCD.StartsWith("P"); } }
   public bool IsRashod    { get { return PozCD.StartsWith("R"); } }

   public string NicePozCD  { get { return PozCD.SubstringSafe(2); } }
 //public string NicePozCDF { get { return PozCD.Length > 2 ? PozCD.Substring(3) : PozCD.Substring(2); } }
   public string NicePozCDF 
   { 
      get 
      {
       //return PozCD.tokLength > 2 ? PozCD.Substring(3) : PozCD.Substring(2);
         int spaceIdx = PozCD.IndexOf(' ');
    
         if(PozCD.Length.IsZero() || spaceIdx.IsNegative()) return "";

         return PozCD.Substring(spaceIdx+1);
      } 
   }

 // 13.12.2016. ako je u rebalansu 0 a u planu nesto postoji u izvjestaju izlazi ono iz plana a trebala bi izaci 0 kao sto je u rebalansu!!!!
 
 //public decimal ThePln   { get { return  (RblZ  .NotZero()                      ? RblZ   : PlnZ  ); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_A { get { return  (RblZ_A.NotZero()                      ? RblZ_A : PlnZ_A); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_B { get { return  (RblZ_B.NotZero()                      ? RblZ_B : PlnZ_B); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_C { get { return  (RblZ_C.NotZero()                      ? RblZ_C : PlnZ_C); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_D { get { return  (RblZ_D.NotZero()                      ? RblZ_D : PlnZ_D); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_E { get { return  (RblZ_E.NotZero()                      ? RblZ_E : PlnZ_E); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_F { get { return  (RblZ_F.NotZero()                      ? RblZ_F : PlnZ_F); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_G { get { return  (RblZ_G.NotZero()                      ? RblZ_G : PlnZ_G); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln   { get { return ((RblZ  .NotZero() || RblZ   != PlnZ  ) ? RblZ   : PlnZ  ); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_A { get { return ((RblZ_A.NotZero() || RblZ_A != PlnZ_A) ? RblZ_A : PlnZ_A); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_B { get { return ((RblZ_B.NotZero() || RblZ_B != PlnZ_B) ? RblZ_B : PlnZ_B); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_C { get { return ((RblZ_C.NotZero() || RblZ_C != PlnZ_C) ? RblZ_C : PlnZ_C); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_D { get { return ((RblZ_D.NotZero() || RblZ_D != PlnZ_D) ? RblZ_D : PlnZ_D); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_E { get { return ((RblZ_E.NotZero() || RblZ_E != PlnZ_E) ? RblZ_E : PlnZ_E); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_F { get { return ((RblZ_F.NotZero() || RblZ_F != PlnZ_F) ? RblZ_F : PlnZ_F); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
 //public decimal ThePln_G { get { return ((RblZ_G.NotZero() || RblZ_G != PlnZ_G) ? RblZ_G : PlnZ_G); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln   { get { return ((Has_RblZ_A ||
                                            Has_RblZ_B ||
                                            Has_RblZ_C ||
                                            Has_RblZ_D ||
                                            Has_RblZ_E ||
                                            Has_RblZ_F ||
                                            Has_RblZ_G  )                   ? RblZ   : PlnZ  ); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_A { get { return  (Has_RblZ_A                      ? RblZ_A : PlnZ_A); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_B { get { return  (Has_RblZ_B                      ? RblZ_B : PlnZ_B); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_C { get { return  (Has_RblZ_C                      ? RblZ_C : PlnZ_C); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_D { get { return  (Has_RblZ_D                      ? RblZ_D : PlnZ_D); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_E { get { return  (Has_RblZ_E                      ? RblZ_E : PlnZ_E); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_F { get { return  (Has_RblZ_F                      ? RblZ_F : PlnZ_F); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 
   public decimal ThePln_G { get { return  (Has_RblZ_G                      ? RblZ_G : PlnZ_G); } } // Plan 'Na Snazi' (Tekuci plan). Ako nema rerbalansa niti novih PLN-ova, onda je to Pln1 

   #region Has Analitika

   public bool Has_PlnPG_A { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_A { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_A { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_A { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_A { get; set; } // Prvi ikad 27.12.21. 
   public bool Has_PlnPG_B { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_B { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_B { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_B { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_B { get; set; } // Prvi ikad 27.12.21. 
   public bool Has_PlnPG_C { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_C { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_C { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_C { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_C { get; set; } // Prvi iakd 27.12.21. 
   public bool Has_PlnPG_D { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_D { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_D { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_D { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_D { get; set; } // Prvi iakd 27.12.21. 
   public bool Has_PlnPG_E { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_E { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_E { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_E { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_E { get; set; } // Prvi iakd 27.12.21. 
   public bool Has_PlnPG_F { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_F { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_F { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_F { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_F { get; set; } // Prvi iakd 27.12.21. 
   public bool Has_PlnPG_G { get; set; } // Proslogodisnji plan 
   public bool  Has_Pln1_G { get; set; } // Prvi ikad           
   public bool  Has_PlnZ_G { get; set; } // Zadnji na dan       
   public bool  Has_RblZ_G { get; set; } // Zadnji na dan       
   public bool  Has_Rbl1_G { get; set; } // Prvi iakd 27.12.21. 
   
   #endregion Has Analitika

   public decimal Pln1 { get { return Pln1_A + Pln1_B + Pln1_C + Pln1_D + Pln1_E + Pln1_F + Pln1_G; } }     // Prvi ikad           
   public decimal Pln1_A { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_B { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_C { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_D { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_E { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_F { get; set; }                                                                      // Prvi ikad           
   public decimal Pln1_G { get; set; }                                                                      // Prvi ikad           
                                                                          
   public decimal PlnZ   { get { return PlnZ_A + PlnZ_B + PlnZ_C + PlnZ_D + PlnZ_E + PlnZ_F + PlnZ_G; } }   // Zadnji na dan       
   public decimal PlnZ_A { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_B { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_C { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_D { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_E { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_F { get; set; }                                                                      // Zadnji na dan       
   public decimal PlnZ_G { get; set; }                                                                      // Zadnji na dan       
                                                                          
   public decimal RblZ   { get { return RblZ_A + RblZ_B + RblZ_C + RblZ_D + RblZ_E + RblZ_F + RblZ_G; } }   // Zadnji na dan       
   public decimal RblZ_A { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_B { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_C { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_D { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_E { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_F { get; set; }                                                                      // Zadnji na dan       
   public decimal RblZ_G { get; set; }                                                                      // Zadnji na dan       

   //27.12.2021.
   public decimal Rbl1   { get { return Rbl1_A + Rbl1_B + Rbl1_C + Rbl1_D + Rbl1_E + Rbl1_F + Rbl1_G; } }   // Prvi RBL       
   public decimal Rbl1_A { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_B { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_C { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_D { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_E { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_F { get; set; }                                                                      // Prvi RBL       
   public decimal Rbl1_G { get; set; }                                                                      // Prvi RBL       


   public decimal PlnPG   { get { return PlnPG_A + PlnPG_B + PlnPG_C+ PlnPG_D + PlnPG_E+ PlnPG_F + PlnPG_G; } } // Proslogodisnji plan 
   public decimal PlnPG_A { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_B { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_C { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_D { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_E { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_F { get; set; }                                                                         // Proslogodisnji plan 
   public decimal PlnPG_G { get; set; }                                                                         // Proslogodisnji plan 

   public decimal Reali { get { return Reali_A + Reali_B + Reali_C + Reali_D + Reali_E + Reali_F + Reali_G; } } // Realizacija         
   public decimal Reali_A { get; set; }                                                                         // Realizacija         
   public decimal Reali_B { get; set; }                                                                         // Realizacija         
   public decimal Reali_C { get; set; }                                                                         // Realizacija         
   public decimal Reali_D { get; set; }                                                                         // Realizacija         
   public decimal Reali_E { get; set; }                                                                         // Realizacija         
   public decimal Reali_F { get; set; }                                                                         // Realizacija         
   public decimal Reali_G { get; set; }                                                                         // Realizacija         

   // 01.03.2021: udio pojedinog retka prihoda ili rashoda u SVEUKUPNO OSTVARENOM-REALIZIRANOM prihodu ili rashodu 
   public decimal Reali_Udio { get; set; }                                                                         // Realizacija         

   public decimal NjvS  { get; set; } // Suma na dan         
   public decimal Money { get; set; } // Placeno, ziro racun 
   public decimal PsOTS { get; set; } // PS potrazivanja i obveze 

   public decimal PlnPBN        { get; set; } // Plan bagatelne nabave bez PDV-a
   public decimal PlnPBNwithPdv { get { return ZXC.VvGet_125_on_100(PlnPBN, Faktur.CommonPdvStForThisDate(OnThisDay, false)); } } // Plan bagatelne nabave sa PDV-om
   public decimal Reali_PlnPBN  { get; set; }
   public decimal Dif_PBN_Reali { get { return PlnPBN - Reali_PlnPBN; } } // Razlika - neto - plana i realizacije

   public decimal OckvS { get { return (Reali + NjvS)        ; } } // ocekivani trosak/prihod - realizacija + najava
   public decimal OckvM { get { return (Money + NjvS + PsOTS); } } // ocekivani trosak/prihod - money + najava + psOTS

   public decimal Reali_Pln1     { get { return (ZXC.DivSafe(Reali  , Pln1    ) * 100M); } }
   public decimal ThePln_Pln1    { get { return (ZXC.DivSafe(ThePln , Pln1    ) * 100M); } }

   public decimal Reali_ThePln   { get { return (ZXC.DivSafe(Reali  , ThePln  ) * 100M); } }
   public decimal Reali_ThePln_A { get { return (ZXC.DivSafe(Reali_A, ThePln_A) * 100M); } }
   public decimal Reali_ThePln_B { get { return (ZXC.DivSafe(Reali_B, ThePln_B) * 100M); } }
   public decimal Reali_ThePln_C { get { return (ZXC.DivSafe(Reali_C, ThePln_C) * 100M); } }
   public decimal Reali_ThePln_D { get { return (ZXC.DivSafe(Reali_D, ThePln_D) * 100M); } }
   public decimal Reali_ThePln_E { get { return (ZXC.DivSafe(Reali_E, ThePln_E) * 100M); } }
   public decimal Reali_ThePln_F { get { return (ZXC.DivSafe(Reali_F, ThePln_F) * 100M); } }
   public decimal Reali_ThePln_G { get { return (ZXC.DivSafe(Reali_G, ThePln_G) * 100M); } }

   public decimal PlnPG_Pln1   { get { return (ZXC.DivSafe(Pln1  , PlnPG ) * 100M); } }
   public decimal PlnPG_PlnZ   { get { return (ZXC.DivSafe(PlnZ  , PlnPG ) * 100M); } }
   public decimal PlnPG_ThePln { get { return (ZXC.DivSafe(ThePln, PlnPG ) * 100M); } }
   public decimal OckvS_ThePln { get { return (ZXC.DivSafe(OckvS , ThePln) * 100M); } }
   public decimal OckvM_ThePln { get { return (ZXC.DivSafe(OckvM , ThePln) * 100M); } }

   public decimal Dif_ThePln_Reali { get { return ThePln - Reali; } } // Razlika - zadnjeg plana i realizacije

   //_______LOOK_____________________________________________________________________________________________________________________
   // -1 skipaj                                                                                                                      
   //  1 boldaj                                                                                                                      
   //  2 daj prazan red iza                                                                                                          
   //  3 pageBreak                                                                                                                   
   
   //                                                           skipano(-1)             boldano(1)            empty(2)              pageBreak(3)
   /* (-1) */   public bool IsSkip           { get { return ( PozLook ==  -1.00M || PozLook == -11.00M || PozLook == -12.00M  || PozLook == -13.00M ) ? true : false; } } // isSkip print - ne printaj tu stavku osim kad je kliknuto sa Izuzetim u filteru
   /* ( 1) */   public bool IsBold           { get { return ( PozLook == -11.00M || PozLook ==   1.00M || PozLook ==  12.00M  || PozLook ==  13.00M ) ? true : false; } } // poboldaj tu stavku kod printa
   /* ( 2) */   public bool IsEmptyLineAfter { get { return ( PozLook == -12.00M || PozLook ==  12.00M || PozLook ==   2.00M                        ) ? true : false; } } // nakon stavke dodaj praznu liniju
   /* ( 3) */   public bool IsNewPageAfter   { get { return ( PozLook == -13.00M || PozLook ==  13.00M ||                        PozLook ==   3.00M ) ? true : false; } } // new page after

   //_______LOOK_____________________________________________________________________________________________________________________

   #endregion propertiz

}

public class SVD_RptLine
{
   public string  GrupCD   { get; set; }
   public string  GrupName { get; set; }

   public string  LineCD   { get; set; }
   public string  LineName { get; set; }

   public decimal ArtGr_00 { get; set; }
   public decimal ArtGr_10 { get; set; }
   public decimal ArtGr_20 { get; set; }
   public decimal ArtGr_30 { get; set; }
   public decimal ArtGr_40 { get; set; }
   public decimal ArtGr_50 { get; set; }
   public decimal ArtGr_60 { get; set; }
   public decimal ArtGr_70 { get; set; }
   public decimal ArtGr_80 { get; set; }
   public decimal ArtGr_90 { get; set; }
   public decimal ArtGr_A0 { get; set; }
   public decimal ArtGr_N0 { get; set; }
   public decimal ArtGr_LP { get; set; }

   public decimal ArtGr_LJ { get { return ArtGr_10 + ArtGr_90 + ArtGr_N0 + ArtGr_A0                                                       ; } }
   public decimal ArtGr_PM { get { return ArtGr_00 + ArtGr_20 + ArtGr_30 + ArtGr_40 + ArtGr_50 + ArtGr_60 + ArtGr_70 + ArtGr_80 + ArtGr_LP; } }

   public decimal Gr_LJ_Posto { get; set; }
   public decimal Gr_PM_Posto { get; set; }
   public decimal Gr_UK_Posto { get; set; }

   public decimal Lim_URA_Posto { get { return ZXC.DivSafe(ALL_GrSUM, SVDLimitPP) * 100M; } } // Za TOP listu ulaznih partnera 

   public decimal SklGr_10 { get; set; }
   public decimal SklGr_20 { get; set; }
   public decimal SklGr_22 { get; set; }
   public decimal SklGr_24 { get; set; }
   public decimal SklGr_26 { get; set; }
   public decimal SklGr_30 { get; set; }
   public decimal SklGr_40 { get; set; }
   public decimal SklGr_50 { get; set; }
   public decimal SklGr_60 { get; set; }
   public decimal SklGr_61 { get; set; }
   public decimal SklGr_70 { get; set; }
   public decimal SklGr_77 { get; set; }
   public decimal SklGr_80 { get; set; }
   public decimal SklGr_90 { get; set; }
   public decimal SklGr_88 { get; set; }

   public decimal SklPosto_10 { get { return ZXC.DivSafe(SklGr_10, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_20 { get { return ZXC.DivSafe(SklGr_20, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_22 { get { return ZXC.DivSafe(SklGr_22, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_24 { get { return ZXC.DivSafe(SklGr_24, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_26 { get { return ZXC.DivSafe(SklGr_26, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_30 { get { return ZXC.DivSafe(SklGr_30, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_40 { get { return ZXC.DivSafe(SklGr_40, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_50 { get { return ZXC.DivSafe(SklGr_50, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_60 { get { return ZXC.DivSafe(SklGr_60, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_61 { get { return ZXC.DivSafe(SklGr_61, ALL_GrSUM) * 100M; } } 
   public decimal SklPosto_70 { get { return ZXC.DivSafe(SklGr_70, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_77 { get { return ZXC.DivSafe(SklGr_77, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_80 { get { return ZXC.DivSafe(SklGr_80, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_90 { get { return ZXC.DivSafe(SklGr_90, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_88 { get { return ZXC.DivSafe(SklGr_88, ALL_GrSUM) * 100M; } }

   public decimal ALL_GrSUM
   {
      get
      {
         return
               ArtGr_00 + ArtGr_10 + ArtGr_20 + ArtGr_30 + ArtGr_40 +  ArtGr_50 + ArtGr_60 + ArtGr_70 + ArtGr_80 + ArtGr_90 + ArtGr_A0 + ArtGr_N0 + ArtGr_LP +
               SklGr_10 + SklGr_20 + SklGr_22 + SklGr_24 + SklGr_26 + SklGr_30 + SklGr_40 +  SklGr_50 + SklGr_60 + SklGr_61 + SklGr_70 + SklGr_77 + SklGr_80 + SklGr_90 + SklGr_88;
      }
   }

   public decimal TOTAL { get; set; }

   public ZXC.SVD_PotrosnjaInfo PotrosnjaInfo { get; set; }

   public decimal SVDLimitPP      { get { return this.PotrosnjaInfo.SVDLimitPP     ; } }

   public decimal AnaTotalPP      { get { return this.PotrosnjaInfo.AnaTotalPP     ; } } // totalna potrosnja / izlaz 

   public decimal AnaUtrosPP      { get { return this.PotrosnjaInfo.AnaUtrosPP     ; } } // limitirana potrisnja - samo sa skl '10' 
   public decimal AnaPostoUtrosPP { get { return this.PotrosnjaInfo.AnaPostoUtrosPP; } }
   public decimal AnaLimitPP      { get { return this.PotrosnjaInfo.AnaLimitPP     ; } }

   public override string ToString()
   {
      return LineCD + " " + LineName;
   }

   public static string[] LegalArtGrs = { "00", "10", "20", "30", "40", "50", "60", "70", "80", "90", "A0", "N0", "LP" };
   public static string[] LegalSklGrs = {
                                          /* SklGr_10 */ "10", 
                                          /* SklGr_20 */ "20", 
                                          /* SklGr_22 */ "22", 
                                          /* SklGr_24 */ "24", 
                                          /* SklGr_26 */ "26", 
                                          /* SklGr_30 */ "30", 
                                          /* SklGr_40 */ "40", 
                                          /* SklGr_50 */ "50", 
                                          /* SklGr_60 */ "60", 
                                          /* SklGr_61 */ "61", 
                                          /* SklGr_70 */ "70", 
                                          /* SklGr_77 */ "77", 
                                          /* SklGr_80 */ "80", 
                                          /* SklGr_90 */ "90",
                                          /* SklGr_88 */ "88" };

   // 25.04.2023: ovo vise ne treba 
 //public static string[] SVD_LegalSkl2022 = {
 //                                       /* SklGr_10 */ "10", 
 //                                       /* SklGr_20 */ "20", 
 //                                       /* SklGr_22 */ "22", 
 //                                       /* SklGr_24 */ "24", 
 //                                       /* SklGr_30 */ "30", 
 //                                       /* SklGr_40 */ "40", 
 //                                       /* SklGr_50 */ "50", 
 //                                       /* SklGr_80 */ "80" };
 //
public SVD_RptLine(XSqlConnection conn, List<Rtrans> rtransList, Kupdob kupdob_rec, bool isByAgr, bool isTOPlista, bool isNEW_4KNJ, DateTime dateOD, DateTime dateDO, bool isChk0, decimal totalReportSUM_LJ, decimal totalReportSUM_PM, string skladCD)
   {
      GrupCD   = kupdob_rec.Ulica1;
      GrupName = kupdob_rec.Email;

      //// Nerjesiva besmislica; 'cijena' artikla za inventuru na odjelima; 
      //// ukoliko ginekologija u 1. mjesecu zaduzi i ne potrosi jedan komad Abaktala po 11 kn
      //// kojem kasnije cijena nabave naraste na npr 44kn... 
      //// KOJU CIJENU PRIKAZATI NA 'ARTIKLI PO ZAVODU/KLINICI (INVENTURA)' 11 ILI 44?!!!
      //// ovisno kako gledamo i jedno i drugo je krivo. 
      //// ovo dole je solucija '11': 
      //foreach(var artiklGR in rtransList.GroupBy(rtr => rtr.T_artiklCD))
      //{
      //   foreach(Rtrans rtrans_rec in artiklGR)
      //   {
      //      rtrans_rec.TmpDecimal = ZXC.DivSafe(artiklGR.Sum(rtr => rtr.R_KCRP), artiklGR.Sum(rtr => rtr.R_kol)); // "Prosjek cijena sa dokumenata" ... onaj '11' 
      //   }
      //}
      //
      //// 22.10.2018: za potrebe SvDuh ovske 'Inventure' da rtrans zna svoju GrupCD, GrupName 
      //rtransList.ForEach(rtr => { rtr.R_grName = GrupCD; rtr.R_kupdobName = GrupName; } );

      if(isTOPlista)
      {
         LineCD   = kupdob_rec.Ticker;
         LineName = kupdob_rec.Naziv;
      }
      else
      {
         LineCD   = kupdob_rec.Ticker;
         LineName = kupdob_rec.Url/*Naziv*/ ;
      }

      // 04.02.2020: 
      if(isNEW_4KNJ)
      {

         string firstRtransSkladCD = rtransList.FirstOrDefault().T_skladCD;
         string firstRtransSkladName = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Cd == firstRtransSkladCD).Name;

         GrupCD   = firstRtransSkladCD;
         GrupName = firstRtransSkladName;


         // glupo, ali brzo ... 
         Kupdob klinikaKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.Naziv == kupdob_rec.Naziv.SubstringSafe(0, 1));

         if(klinikaKupdob_rec != null)
         {
            LineCD   = klinikaKupdob_rec.Ime   ;
            LineName = klinikaKupdob_rec.Ulica1;
         }
         else
         {
            LineCD = LineName = "NEPOZNATO";
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu saznati kliniku za \n\n{0}", kupdob_rec.ToString());
         }
      }


      if(isByAgr) SetArtiklGrSum(rtransList);
      else        SetSkladGrSum (rtransList);

      this.TOTAL = rtransList.Sum(r => r.R_KCRP); // ovo je total grupe tj redka, a NE svih rtransa

      string                    theTT = Faktur.TT_IZD; // za pocetak 
      if(rtransList.NotEmpty()) theTT = rtransList[0].T_TT;

      this.PotrosnjaInfo = RtransDao.Get_SVD_PotrosnjaInfo(conn, theTT, kupdob_rec, dateOD, dateDO, VvUserControl.KupdobSifrar, true);

      #region Some Checks

      // 04.05.2020: 
    //if(isNEW_4KNJ == false && /*isChk0 &&*/ !ZXC.AlmostEqual(AnaTotalPP, ALL_GrSUM, 1.00M)                     ) ZXC.aim_emsg(MessageBoxIcon.Error, "AnaTotalPP != ALL_GrSUM\n\n{0} {1}", AnaTotalPP.ToStringVv(), ALL_GrSUM.ToStringVv());
      if(isNEW_4KNJ == false && /*isChk0 &&*/ !ZXC.AlmostEqual(AnaTotalPP, ALL_GrSUM, 1.00M) && skladCD.IsEmpty()) ZXC.aim_emsg(MessageBoxIcon.Error, "AnaTotalPP != ALL_GrSUM\n\n{0} {1}", AnaTotalPP.ToStringVv(), ALL_GrSUM.ToStringVv());

      if(rtransList.Any(rtr => LegalArtGrs.Contains(rtr.A_ArtGrCd1) == false))
      {
         var badRtransList = rtransList.Where(rtr => LegalArtGrs.Contains(rtr.A_ArtGrCd1) == false);

         foreach(var rtrans in badRtransList)
         {
            // if ipak nije potreban
          //if(rtrans.T_artiklName != "SVD_dummy") // "SVD_dummy" je rtrans umjetno ubacen u TheRtransList-u da izadju svi odjeli iako nemaju prometa.
          //{
               ZXC.aim_emsg(MessageBoxIcon.Error, "Nepoznata grupa artikla!\n\nGrupa: [{0}]\n\n{1}", rtrans.A_ArtGrCd1, rtrans);
          //}
         }
      }

      if(rtransList.Any(rtr => LegalSklGrs.Contains(rtr.T_skladCD) == false))
      {
         var badRtransList = rtransList.Where(rtr => LegalSklGrs.Contains(rtr.T_skladCD) == false);

         foreach(var rtrans in badRtransList)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nepoznato skladište!\n\n{0}\n\n{1}", rtrans.T_skladCD, rtrans);
         }
      }

      #endregion Some Checks

      // za TOP Listu
      Gr_LJ_Posto = ZXC.DivSafe(ArtGr_LJ           , totalReportSUM_LJ                    ) * 100M;
      Gr_PM_Posto = ZXC.DivSafe(ArtGr_PM           , totalReportSUM_PM                    ) * 100M;
      Gr_UK_Posto = ZXC.DivSafe(ArtGr_LJ + ArtGr_PM, totalReportSUM_LJ + totalReportSUM_PM) * 100M;

   }

   private void SetArtiklGrSum(List<Rtrans> rtransList)
   {
      ArtGr_00 = rtransList.Where(r => r.A_ArtGrCd1 == "00").Sum(r => r.R_KCRP);
      ArtGr_10 = rtransList.Where(r => r.A_ArtGrCd1 == "10").Sum(r => r.R_KCRP);
      ArtGr_20 = rtransList.Where(r => r.A_ArtGrCd1 == "20").Sum(r => r.R_KCRP);
      ArtGr_30 = rtransList.Where(r => r.A_ArtGrCd1 == "30").Sum(r => r.R_KCRP);
      ArtGr_40 = rtransList.Where(r => r.A_ArtGrCd1 == "40").Sum(r => r.R_KCRP);
      ArtGr_50 = rtransList.Where(r => r.A_ArtGrCd1 == "50").Sum(r => r.R_KCRP);
      ArtGr_60 = rtransList.Where(r => r.A_ArtGrCd1 == "60").Sum(r => r.R_KCRP);
      ArtGr_70 = rtransList.Where(r => r.A_ArtGrCd1 == "70").Sum(r => r.R_KCRP);
      ArtGr_80 = rtransList.Where(r => r.A_ArtGrCd1 == "80").Sum(r => r.R_KCRP);
      ArtGr_90 = rtransList.Where(r => r.A_ArtGrCd1 == "90").Sum(r => r.R_KCRP);
      ArtGr_A0 = rtransList.Where(r => r.A_ArtGrCd1 == "A0").Sum(r => r.R_KCRP);
      ArtGr_N0 = rtransList.Where(r => r.A_ArtGrCd1 == "N0").Sum(r => r.R_KCRP);
      ArtGr_LP = rtransList.Where(r => r.A_ArtGrCd1 == "LP").Sum(r => r.R_KCRP);
   }

   private void SetSkladGrSum (List<Rtrans> rtransList)
   {
      SklGr_10 = rtransList.Where(r => r.T_skladCD == "10").Sum(r => r.R_KCRP);
      SklGr_20 = rtransList.Where(r => r.T_skladCD == "20").Sum(r => r.R_KCRP);
      SklGr_22 = rtransList.Where(r => r.T_skladCD == "22").Sum(r => r.R_KCRP);
      SklGr_24 = rtransList.Where(r => r.T_skladCD == "24").Sum(r => r.R_KCRP);
      SklGr_26 = rtransList.Where(r => r.T_skladCD == "26").Sum(r => r.R_KCRP);
      SklGr_30 = rtransList.Where(r => r.T_skladCD == "30").Sum(r => r.R_KCRP);
      SklGr_40 = rtransList.Where(r => r.T_skladCD == "40").Sum(r => r.R_KCRP);
      SklGr_50 = rtransList.Where(r => r.T_skladCD == "50").Sum(r => r.R_KCRP);
      SklGr_60 = rtransList.Where(r => r.T_skladCD == "60").Sum(r => r.R_KCRP);
      SklGr_61 = rtransList.Where(r => r.T_skladCD == "61").Sum(r => r.R_KCRP);
      SklGr_70 = rtransList.Where(r => r.T_skladCD == "70").Sum(r => r.R_KCRP);
      SklGr_77 = rtransList.Where(r => r.T_skladCD == "77").Sum(r => r.R_KCRP);
      SklGr_80 = rtransList.Where(r => r.T_skladCD == "80").Sum(r => r.R_KCRP);
      SklGr_90 = rtransList.Where(r => r.T_skladCD == "90").Sum(r => r.R_KCRP);
      SklGr_88 = rtransList.Where(r => r.T_skladCD == "88").Sum(r => r.R_KCRP);
   }

}

public class SVD_SubRptLine_Mon : SVD_SubRptLine
{
   // Novo: 
   public int Month { get; set; }

   public SVD_SubRptLine_Mon(List<Rtrans> rtransList, VvLookUpItem theLui, bool isColByAgr) : base(rtransList, theLui, isColByAgr)
   {

   }

}

public class SVD_SubRptLine
{
   public string  LineCD   { get; set; }
   public string  LineName { get; set; }

   public decimal ArtGr_00 { get; set; }
   public decimal ArtGr_10 { get; set; }
   public decimal ArtGr_20 { get; set; }
   public decimal ArtGr_30 { get; set; }
   public decimal ArtGr_40 { get; set; }
   public decimal ArtGr_50 { get; set; }
   public decimal ArtGr_60 { get; set; }
   public decimal ArtGr_70 { get; set; }
   public decimal ArtGr_80 { get; set; }
   public decimal ArtGr_90 { get; set; }
   public decimal ArtGr_A0 { get; set; }
   public decimal ArtGr_N0 { get; set; }
   public decimal ArtGr_LP { get; set; }

   public decimal ArtGr_LJ { get { return ArtGr_10 + ArtGr_90 + ArtGr_N0 + ArtGr_A0                                                       ; } }
   public decimal ArtGr_PM { get { return ArtGr_00 + ArtGr_20 + ArtGr_30 + ArtGr_40 + ArtGr_50 + ArtGr_60 + ArtGr_70 + ArtGr_80 + ArtGr_LP; } }

   public decimal SklGr_10 { get; set; }
   public decimal SklGr_20 { get; set; }
   public decimal SklGr_22 { get; set; }
   public decimal SklGr_24 { get; set; }
   public decimal SklGr_26 { get; set; }
   public decimal SklGr_30 { get; set; }
 //public decimal SklGr_40 { get; set; }
   public decimal SklGr_40 { get; set; }
   public decimal SklGr_50 { get; set; }
 //public decimal SklGr_60 { get; set; }
   public decimal SklGr_60 { get; set; }
   public decimal SklGr_61 { get; set; }
   public decimal SklGr_70 { get; set; }
   public decimal SklGr_77 { get; set; }
   public decimal SklGr_80 { get; set; }
   public decimal SklGr_90 { get; set; }
   public decimal SklGr_88 { get; set; }

   public decimal SklPosto_10 { get { return ZXC.DivSafe(SklGr_10, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_20 { get { return ZXC.DivSafe(SklGr_20, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_22 { get { return ZXC.DivSafe(SklGr_22, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_24 { get { return ZXC.DivSafe(SklGr_24, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_26 { get { return ZXC.DivSafe(SklGr_26, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_30 { get { return ZXC.DivSafe(SklGr_30, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_40 { get { return ZXC.DivSafe(SklGr_40, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_50 { get { return ZXC.DivSafe(SklGr_50, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_60 { get { return ZXC.DivSafe(SklGr_60, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_61 { get { return ZXC.DivSafe(SklGr_61, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_70 { get { return ZXC.DivSafe(SklGr_70, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_77 { get { return ZXC.DivSafe(SklGr_77, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_80 { get { return ZXC.DivSafe(SklGr_80, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_90 { get { return ZXC.DivSafe(SklGr_90, ALL_GrSUM) * 100M; } }
   public decimal SklPosto_88 { get { return ZXC.DivSafe(SklGr_88, ALL_GrSUM) * 100M; } }

   // novo u 2022: 
   public decimal Skl50MaxPrice_Money    { get; set; }
   public string  Skl50MaxPrice_ArtiklCD { get; set; }
   public string  Skl50MaxPrice_Name     { get; set; }

   public decimal ALL_GrSUM
   {
      get
      {
         return
               ArtGr_00 + ArtGr_10 + ArtGr_20 + ArtGr_30 + ArtGr_40 + ArtGr_50 + ArtGr_60 + ArtGr_70 + ArtGr_80 + ArtGr_90 + ArtGr_A0 + ArtGr_N0 + ArtGr_LP +
               SklGr_10 + SklGr_20 + SklGr_22 + SklGr_24 + SklGr_26 + SklGr_30 + SklGr_40 + SklGr_50 + SklGr_60 + SklGr_61 + SklGr_70 + SklGr_77 + SklGr_80 + SklGr_90 + SklGr_88;
      }
   }

   public override string ToString()
   {
      return LineCD + " " + LineName;
   }

   public SVD_SubRptLine(List<Rtrans> rtransList, VvLookUpItem theLui, bool isColByAgr)
   {
      LineCD   = theLui.Cd  ;
      LineName = theLui.Name;

      if(isColByAgr) SetArtiklGrSum(rtransList);
      else           SetSkladGrSum (rtransList);
   }

   private void SetArtiklGrSum(List<Rtrans> rtransList)
   {
      ArtGr_00 = rtransList.Where(r => r.A_ArtGrCd1 == "00").Sum(r => r.R_KCRP);
      ArtGr_10 = rtransList.Where(r => r.A_ArtGrCd1 == "10").Sum(r => r.R_KCRP);
      ArtGr_20 = rtransList.Where(r => r.A_ArtGrCd1 == "20").Sum(r => r.R_KCRP);
      ArtGr_30 = rtransList.Where(r => r.A_ArtGrCd1 == "30").Sum(r => r.R_KCRP);
      ArtGr_40 = rtransList.Where(r => r.A_ArtGrCd1 == "40").Sum(r => r.R_KCRP);
      ArtGr_50 = rtransList.Where(r => r.A_ArtGrCd1 == "50").Sum(r => r.R_KCRP);
      ArtGr_60 = rtransList.Where(r => r.A_ArtGrCd1 == "60").Sum(r => r.R_KCRP);
      ArtGr_70 = rtransList.Where(r => r.A_ArtGrCd1 == "70").Sum(r => r.R_KCRP);
      ArtGr_80 = rtransList.Where(r => r.A_ArtGrCd1 == "80").Sum(r => r.R_KCRP);
      ArtGr_90 = rtransList.Where(r => r.A_ArtGrCd1 == "90").Sum(r => r.R_KCRP);
      ArtGr_A0 = rtransList.Where(r => r.A_ArtGrCd1 == "A0").Sum(r => r.R_KCRP);
      ArtGr_N0 = rtransList.Where(r => r.A_ArtGrCd1 == "N0").Sum(r => r.R_KCRP);
      ArtGr_LP = rtransList.Where(r => r.A_ArtGrCd1 == "LP").Sum(r => r.R_KCRP);

      #region Skl50MaxPrice

      var skl50_rtransList = rtransList.Where(r => r.T_skladCD == "50").ToList();

      if(skl50_rtransList.IsEmpty())
      {
         Skl50MaxPrice_ArtiklCD = 
         Skl50MaxPrice_Name     = "";
         Skl50MaxPrice_Money    = 0M;
      }
      else
      {
         decimal maxPrice = skl50_rtransList.Max(r => r.R_CIJ_KCRP);

         Rtrans maxPriceRtrans = skl50_rtransList.First(r => r.R_CIJ_KCRP == maxPrice);

         Skl50MaxPrice_ArtiklCD = maxPriceRtrans.T_artiklCD  ;
         Skl50MaxPrice_Name     = maxPriceRtrans.T_artiklName;
       //Skl50MaxPrice_Money    = maxPriceRtrans.R_KCRP      ;
         Skl50MaxPrice_Money    = skl50_rtransList.Where(r => r.T_artiklCD == Skl50MaxPrice_ArtiklCD).Sum(r => r.R_KCRP);

      }

      #endregion Skl50MaxPrice

   }

   private void SetSkladGrSum (List<Rtrans> rtransList)
   {
      SklGr_10 = rtransList.Where(r => r.T_skladCD == "10").Sum(r => r.R_KCRP);
      SklGr_20 = rtransList.Where(r => r.T_skladCD == "20").Sum(r => r.R_KCRP);
      SklGr_22 = rtransList.Where(r => r.T_skladCD == "22").Sum(r => r.R_KCRP);
      SklGr_24 = rtransList.Where(r => r.T_skladCD == "24").Sum(r => r.R_KCRP);
      SklGr_26 = rtransList.Where(r => r.T_skladCD == "26").Sum(r => r.R_KCRP);
      SklGr_30 = rtransList.Where(r => r.T_skladCD == "30").Sum(r => r.R_KCRP);

    //SklGr_40 = rtransList.Where(r => r.T_skladCD == "40").Sum(r => r.R_KCRP);
      SklGr_40 = rtransList.Where(r => r.T_skladCD == "40").Sum(r => r.R_KCRP);

      SklGr_50 = rtransList.Where(r => r.T_skladCD == "50").Sum(r => r.R_KCRP);
    //SklGr_60 = rtransList.Where(r => r.T_skladCD == "60").Sum(r => r.R_KCRP);
      SklGr_60 = rtransList.Where(r => r.T_skladCD == "60").Sum(r => r.R_KCRP);
      SklGr_61 = rtransList.Where(r => r.T_skladCD == "61").Sum(r => r.R_KCRP);
      SklGr_70 = rtransList.Where(r => r.T_skladCD == "70").Sum(r => r.R_KCRP);
      SklGr_77 = rtransList.Where(r => r.T_skladCD == "77").Sum(r => r.R_KCRP);
      SklGr_80 = rtransList.Where(r => r.T_skladCD == "80").Sum(r => r.R_KCRP);
      SklGr_90 = rtransList.Where(r => r.T_skladCD == "90").Sum(r => r.R_KCRP);
      SklGr_88 = rtransList.Where(r => r.T_skladCD == "88").Sum(r => r.R_KCRP);
   }

}

public abstract class PCK_BASE_InfoLine
{
   public string  PCK_ArtCD    { get; set; }
   public string  PCK_ArtName  { get; set; }
   public string  PCK_RAMkind  { get; set; }
   public string  PCK_HDDkind  { get; set; }
   public string  PCK_SklCD    { get; set; }
   public decimal PCK_RAM      { get; set; }
   public decimal PCK_HDD      { get; set; }

   public PCK_BASE_InfoLine(string _PCK_ArtCD, string _PCK_ArtName, string _PCK_RAMkind, string _PCK_HDDkind, string _PCK_SklCD, decimal _PCK_RAM, decimal _PCK_HDD)
   {
      this.PCK_ArtCD    = _PCK_ArtCD   ;
      this.PCK_ArtName  = _PCK_ArtName ;
      this.PCK_RAMkind  = _PCK_RAMkind ;
      this.PCK_HDDkind  = _PCK_HDDkind ;
      this.PCK_SklCD    = _PCK_SklCD   ;
      this.PCK_RAM      = _PCK_RAM     ;
      this.PCK_HDD      = _PCK_HDD     ;
   }

   //public override string ToString()
   //{
   //   return @""" + PCK_Serno + @"" " +  PCK_ArtCD + " [" + PCK_ArtName + "]" + " [" + PCK_RAMkind + "]" + " RAM: " + PCK_RAM.ToString0Vv() + "Gb [" + PCK_HDDkind + "] HDD: " + PCK_HDD.ToString0Vv() + " Gb";
   //}
}

public class PCK_ArtiklInfo_Line : PCK_BASE_InfoLine
{
   //public decimal UkPstKol     { get; set; }
   //public decimal UkUlazKol    { get; set; }
   //public decimal UkIzlazKol   { get; set; }
   //public decimal StanjeKol    { get { return /* UkPstKol + */ UkUlazKol - UkIzlazKol; } }
   public int StanjeKol { get { return PCK_SernoInfo_List.Count; } }

   public List<PCK_SernoInfo_Line> PCK_SernoInfo_List { get; set; }

   public PCK_ArtiklInfo_Line(string _PCK_ArtCD, string _PCK_ArtName, string _PCK_RAMkind, string _PCK_HDDkind, string _PCK_SklCD, decimal _PCK_RAM, decimal _PCK_HDD) : 
                                base(_PCK_ArtCD,        _PCK_ArtName,        _PCK_RAMkind,        _PCK_HDDkind,        _PCK_SklCD,         _PCK_RAM,         _PCK_HDD)
   {
   }

   public override string ToString()
   {
      return PCK_ArtCD + " [" + PCK_ArtName + "]" + " [" + PCK_RAMkind + "]" + " RAM: " + PCK_RAM.ToString0Vv() + "Gb [" + PCK_HDDkind + "] HDD: " + PCK_HDD.ToString0Vv() + " Gb";
   }
}

public class PCK_SernoInfo_Line : PCK_BASE_InfoLine
{
   public string PCK_Serno     { get; set; }
   public PCK_SernoInfo_Line(string _PCK_serno, string _PCK_ArtCD, string _PCK_ArtName, string _PCK_RAMkind, string _PCK_HDDkind, string _PCK_SklCD, decimal _PCK_RAM, decimal _PCK_HDD) : 
                                                  base(_PCK_ArtCD,        _PCK_ArtName,        _PCK_RAMkind,        _PCK_HDDkind,        _PCK_SklCD,         _PCK_RAM,         _PCK_HDD)
   {
      this.PCK_Serno = _PCK_serno;
   }

   public override string ToString()
   {
      return @"{" + PCK_Serno + @"} " +  PCK_ArtCD + " [" + PCK_ArtName + "]" + " [" + PCK_RAMkind + "]" + " RAM: " + PCK_RAM.ToString0Vv() + "Gb [" + PCK_HDDkind + "] HDD: " + PCK_HDD.ToString0Vv() + " Gb";
   }
}

#if NEYNAM_KAJCEMIOVO

public class PCK_ArtiklInfo_Dao
{
   public List<PCK_ArtiklInfo_Line> PCK_ArtiklInfo_Lines { get; set; }

   public PCK_ArtiklInfo_Dao(XSqlConnection conn, string _PCK_ArtCD, string _PCK_sklCD, string _PCK_ArtKlasa)// : base()
   {
      List<Artikl> PCKartikls = 
         
         _PCK_ArtCD   .NotEmpty() ? VvUserControl.ArtiklSifrar.Where(art => art.ArtiklCD == _PCK_ArtCD                      ).ToList() :
         _PCK_ArtKlasa.NotEmpty() ? VvUserControl.ArtiklSifrar.Where(art => art.Grupa3CD == _PCK_ArtKlasa && art.TS == "PCK").ToList() :
                                    VvUserControl.ArtiklSifrar.Where(art =>                                  art.TS == "PCK").ToList() ;

      PCK_ArtiklInfo_Lines = new List<PCK_ArtiklInfo_Line>();

      List<Rtrans> thisArtiklRtranses;

      List<string> skladCDlist;

      foreach(Artikl artikl in PCKartikls)
      {
         if(_PCK_sklCD.NotEmpty()) skladCDlist = new List<string> { _PCK_sklCD };
         else                      skladCDlist = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, artikl.ArtiklCD);

         foreach(string currSkladCD in skladCDlist)
         {
            thisArtiklRtranses = GetArtiklRtranses(conn, artikl.ArtiklCD, /*_PCK_sklCD*/currSkladCD);

            thisArtiklRtranses.ForEach(rtr => 
            { 
               rtr.T_konto  = artikl.Grupa2CD; // RAM klasa, RAM kind 
               rtr.T_serlot = artikl.Grupa3CD; // HDD klasa, HDD kind 
            } ); 

            //ALL_ArtiklRtranses.AddRange(thisArtiklRtranses);

            PCK_ArtiklInfo_Lines.AddRange(GetThisArtikls_PCK_ArtiklInfo_lines(thisArtiklRtranses));

         } // foreach(string currSkladCD in skladCDlist)

      } // foreach(Artikl artikl in PCKartikls) 

   }

   List<Rtrans> GetArtiklRtranses(XSqlConnection conn, string artiklCD, string skladCD)
   {
      List<Rtrans> rtranses = new List<Rtrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

    //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_skladDate], "elDateOd", dateOd,   " >= "));
    //filterMembers.Add(new VvSqlFilterMember(ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_skladDate], "elDateDo", dateDo,   " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_artiklCD ], false, "elArtCD", artiklCD, "", "", " = ", "", "R"));
      if(skladCD.NotEmpty())
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_skladCD  ], false, "elSklCD", skladCD,  "", "", " = ", "", "R"));

      string orderBy = Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_");

      RtransDao.GetRtransWithArtstatList(conn, rtranses, "", filterMembers, orderBy);

      return rtranses;
   }

   // Voila! 
   private List<PCK_ArtiklInfo_Line> GetThisArtikls_PCK_ArtiklInfo_lines(List<Rtrans> thisArtiklRtranses)
   {
      var anaGRps = thisArtiklRtranses.GroupBy(rtr => rtr.T_doCijMal.ToString0Vv() + " / " + rtr.T_noCijMal.ToString0Vv());

      List<PCK_ArtiklInfo_Line> thisArtiklsPCKlines = new List<PCK_ArtiklInfo_Line>();

      PCK_ArtiklInfo_Line pck_rec;

      foreach(var anaGR in anaGRps)
      {
         pck_rec = new PCK_ArtiklInfo_Line(anaGR.First().T_artiklCD, anaGR.First().T_artiklName, anaGR.First().T_konto, anaGR.First().T_serlot, anaGR.First().T_skladCD, anaGR.First().T_doCijMal, anaGR.First().T_noCijMal);

         pck_rec.UkPstKol   = anaGR.Where(r => r.TtInfo.IsFinKol_PS).Sum(r => r.T_kol);
         pck_rec.UkUlazKol  = anaGR.Where(r => r.TtInfo.IsFinKol_U ).Sum(r => r.T_kol);
         pck_rec.UkIzlazKol = anaGR.Where(r => r.TtInfo.IsFinKol_I ).Sum(r => r.T_kol);

         thisArtiklsPCKlines.Add(pck_rec);
      }

      return thisArtiklsPCKlines;
   }

}

public class PCK_SernoInfo_Dao
{

   public List<PCK_ArtiklInfo_Line> PCK_ArtiklInfo_List { get; set; }
   public List<PCK_SernoInfo_Line>  PCK_SernoInfo_List  { get; set; }

   public PCK_SernoInfo_Dao(XSqlConnection conn, string _PCK_Serno, string _PCK_SklCD)
   {
      PCK_SernoInfo_List = new List<PCK_SernoInfo_Line>();

      List<Rtrano> thisSernoRtranos;

      thisSernoRtranos = GetSernoRtranosForSklad(conn, _PCK_Serno, _PCK_SklCD);

      if(thisSernoRtranos.IsEmpty()) return;

      Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == thisSernoRtranos.First().T_artiklCD);

      if(artikl_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "PCK_SernoInfo_Dao: nema artikla za rtrano\n\r\n\r{0}", thisSernoRtranos.First());
         return;
      }

      PCK_SernoInfo_Line theLine;

      foreach(Rtrano rtrano_rec in thisSernoRtranos)
      {
         theLine = new PCK_SernoInfo_Line(_PCK_Serno, rtrano_rec.T_artiklCD, rtrano_rec.T_artiklName, artikl_rec.Grupa2CD, artikl_rec.Grupa3CD, "", rtrano_rec.T_dimZ, rtrano_rec.T_decC);

         PCK_SernoInfo_List.Add(theLine);
      }

   }

   private List<Rtrano> GetSernoRtranosForSklad(XSqlConnection conn, string _PCK_Serno, string _PCK_SklCD)
   {
      List<Rtrano> rtranos = new List<Rtrano>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtranoDao.TheSchemaTable.Rows[ZXC.RtranoDao.CI.t_serno  ], false, "theSerno", _PCK_Serno, "", "", " = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(ZXC.RtranoDao.TheSchemaTable.Rows[ZXC.RtranoDao.CI.t_skladCD], false, "theSklCD", _PCK_SklCD, "", "", " = ", "", "R"));

      string orderBy = Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_");

      VvDaoBase.LoadGenericVvDataRecordList(conn, rtranos, filterMembers, orderBy);

      return rtranos;
   }

}
#endif

