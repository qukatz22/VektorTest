using System;
using System.Data;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using XSqlException  = MySql.Data.MySqlClient.MySqlException;
using System.Collections.Generic;
using Vektor.DataLayer.DS_Reports;
#endif

public sealed class RtranoDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static RtranoDao instance;

   private RtranoDao(XSqlConnection conn, string dbName) : base(dbName, Rtrano.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static RtranoDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new RtranoDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableRtrano

   public static   uint TableVersionStatic { get { return 8; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID        int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID   int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum     int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial     smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_skladDate  date                  NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt         char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 07 */  "t_ttSort     tinyint(4)            NOT NULL default '0'   ,\n" +
         /* 08 */  "t_artiklCD   varchar(32)           NOT NULL default ''    ,\n" +
         /* 09 */  "t_skladCD    varchar( 6)           NOT NULL default ''    ,\n" +
         /* 00 */  "t_artiklName varchar(80)           NOT NULL default ''    ,\n" +
         /* 11 */  "t_kupdob_cd  int(6)       unsigned NOT NULL default '0'   ,\n" +
         /* 12 */  "t_serno      varchar(64)           NOT NULL default ''    ,\n" +
         /* 13 */  "t_paletaNo   int(10)      unsigned NOT NULL               ,\n" +
         /* 14 */  "t_dimX       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 15 */  "t_dimY       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 16 */  "t_dimZ       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 17 */  "t_komada     decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 18 */  "t_kol        decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 19 */  "t_grCD       varchar(16)           NOT NULL default ''    ,\n" +
         /* 20 */  "t_isKomDummy tinyint(1)   unsigned NOT NULL default '0'   ,\n" +
         /* 21 */  "t_decA       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 22 */  "t_decB       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 23 */  "t_decC       decimal(12,4)         NOT NULL default '0.00',\n" +
         /* 24 */  "t_rtrRecID   int(10)      unsigned NOT NULL               ,\n" +

          "PRIMARY KEY                   (recID)                                                          ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                           ,\n" +
          /*"UNIQUE*/" KEY BY_ARTIKL     (t_artiklCD, t_skladCD, t_skladDate, t_ttSort, t_ttNum, t_serial),\n" +
          /*"UNIQUE*/" KEY BY_SERNO      (t_serno)                                                         \n" 
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Rtrano.recordNameArhiva;
      else         tableName = Rtrano.recordName;

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN t_paletaNo   int(10)      unsigned NOT NULL                AFTER t_serno   ,    " +
                         "ADD COLUMN t_dimX       decimal(12,4)         NOT NULL default '0.00' AFTER t_paletaNo,    " +
                         "ADD COLUMN t_dimY       decimal(12,4)         NOT NULL default '0.00' AFTER t_dimX    ,    " +
                         "ADD COLUMN t_dimZ       decimal(12,4)         NOT NULL default '0.00' AFTER t_dimY    ,    " +
                         "ADD COLUMN t_komada     decimal(12,4)         NOT NULL default '0.00' AFTER t_dimZ    ;\n");

         case 3: return ("ADD COLUMN t_kol        decimal(12,4)         NOT NULL default '0.00' AFTER t_komada  ;\n");

         case 4: return ("ADD INDEX BY_SERNO      (t_serno);\n");

         case 5: return ("ADD COLUMN t_grCD       varchar(16)           NOT NULL default '' AFTER t_kol  ;\n");

         case 6: return ("ADD COLUMN t_isKomDummy tinyint(1)  unsigned NOT NULL default '0' AFTER t_grCD ;\n");

         case 7: return ("MODIFY COLUMN t_ttSort  tinyint(4)           NOT NULL default '0'   ;");

         case 8: return ("ADD COLUMN t_decA       decimal(12,4)         NOT NULL default '0.00' AFTER t_isKomDummy,    " +
                         "ADD COLUMN t_decB       decimal(12,4)         NOT NULL default '0.00' AFTER t_decA      ,    " +
                         "ADD COLUMN t_decC       decimal(12,4)         NOT NULL default '0.00' AFTER t_decB      ,    " +
                         "ADD COLUMN t_rtrRecID   int(10)      unsigned NOT NULL                AFTER t_decC      ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableRtrano

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Rtrano rtrano = (Rtrano)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_parentID,     TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_dokNum,       TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_serial,       TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_skladDate,    TheSchemaTable.Rows[CI.t_skladDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_TT,           TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_ttNum,        TheSchemaTable.Rows[CI.t_ttNum]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_ttSort      , TheSchemaTable.Rows[CI.t_ttSort    ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_artiklCD    , TheSchemaTable.Rows[CI.t_artiklCD  ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_skladCD     , TheSchemaTable.Rows[CI.t_skladCD   ]);
      /* 00 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_artiklName  , TheSchemaTable.Rows[CI.t_artiklName]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_kupdobCD    , TheSchemaTable.Rows[CI.t_kupdobCD  ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_serno       , TheSchemaTable.Rows[CI.t_serno     ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_paletaNo    , TheSchemaTable.Rows[CI.t_paletaNo  ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_dimX        , TheSchemaTable.Rows[CI.t_dimX      ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_dimY        , TheSchemaTable.Rows[CI.t_dimY      ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_dimZ        , TheSchemaTable.Rows[CI.t_dimZ      ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_komada      , TheSchemaTable.Rows[CI.t_komada    ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_kol         , TheSchemaTable.Rows[CI.t_kol       ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_grCD        , TheSchemaTable.Rows[CI.t_grCD      ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_isKomDummy  , TheSchemaTable.Rows[CI.t_isKomDummy]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_decA        , TheSchemaTable.Rows[CI.t_decA      ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_decB        , TheSchemaTable.Rows[CI.t_decB      ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_decC        , TheSchemaTable.Rows[CI.t_decC      ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, rtrano.T_rtrRecID    , TheSchemaTable.Rows[CI.t_rtrRecID  ]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      RtranoStruct rdrData = new RtranoStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */ rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */ rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */ rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */ rdrData._t_skladDate  = reader.GetDateTime(CI.t_skladDate);
      /* 05 */ rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */ rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum);
      /* 07 */ rdrData._t_ttSort     = reader.GetInt16   (CI.t_ttSort      );
      /* 08 */ rdrData._t_artiklCD   = reader.GetString  (CI.t_artiklCD    );
      /* 09 */ rdrData._t_skladCD    = reader.GetString  (CI.t_skladCD     );
      /* 00 */ rdrData._t_artiklName = reader.GetString  (CI.t_artiklName  );
      /* 11 */ rdrData._t_kupdob_cd  = reader.GetUInt32  (CI.t_kupdobCD    );
      /* 12 */ rdrData._t_serno      = reader.GetString  (CI.t_serno       );
      /* 13 */ rdrData._t_paletaNo   = reader.GetUInt32  (CI.t_paletaNo    );
      /* 14 */ rdrData._t_dimX       = reader.GetDecimal (CI.t_dimX        );
      /* 15 */ rdrData._t_dimY       = reader.GetDecimal (CI.t_dimY        );
      /* 16 */ rdrData._t_dimZ       = reader.GetDecimal (CI.t_dimZ        );
      /* 17 */ rdrData._t_komada     = reader.GetDecimal (CI.t_komada      );
      /* 18 */ rdrData._t_kol        = reader.GetDecimal (CI.t_kol         );
      /* 19 */ rdrData._t_grCD       = reader.GetString  (CI.t_grCD        );
      /* 19 */ rdrData._t_grCD       = reader.GetString  (CI.t_grCD        );
      /* 20 */ rdrData._t_isKomDummy = reader.GetBoolean (CI.t_isKomDummy  );
      /* 21 */ rdrData._t_decA       = reader.GetDecimal (CI.t_decA        );
      /* 22 */ rdrData._t_decB       = reader.GetDecimal (CI.t_decB        );
      /* 23 */ rdrData._t_decC       = reader.GetDecimal (CI.t_decC        );
      /* 24 */ rdrData._t_rtrRecID   = reader.GetUInt32  (CI.t_rtrRecID    );

      ((Rtrano)vvDataRecord).CurrentData = rdrData;

      // NE !: ((Rtrano)vvDataRecord).CalcTransResults();
      
      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct RtranoCI
   {
      internal int recID;

      /* 01 */  internal int t_parentID;
      /* 02 */  internal int t_dokNum;
      /* 03 */  internal int t_serial;
      /* 04 */  internal int t_skladDate;
      /* 05 */  internal int t_tt;
      /* 06 */  internal int t_ttNum;
      /* 07 */  internal int t_ttSort    ;
      /* 08 */  internal int t_artiklCD  ;
      /* 09 */  internal int t_skladCD   ;
      /* 00 */  internal int t_artiklName;
      /* 11 */  internal int t_kupdobCD  ;
      /* 02 */  internal int t_serno     ;
      /* 02 */  internal int t_paletaNo  ;
      /* 02 */  internal int t_dimX      ;
      /* 02 */  internal int t_dimY      ;
      /* 02 */  internal int t_dimZ      ;
      /* 02 */  internal int t_komada    ;
      /* 02 */  internal int t_kol       ;
      /* 02 */  internal int t_grCD      ;
      /* 20 */  internal int t_isKomDummy;
      /* 02 */  internal int t_decA      ;
      /* 02 */  internal int t_decB      ;
      /* 02 */  internal int t_decC      ;
      /* 02 */  internal int t_rtrRecID  ;

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public RtranoCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */ CI.t_parentID     = GetSchemaColumnIndex("t_parentID"  );
      /* 02 */ CI.t_dokNum       = GetSchemaColumnIndex("t_dokNum"    );
      /* 03 */ CI.t_serial       = GetSchemaColumnIndex("t_serial"    );
      /* 04 */ CI.t_skladDate    = GetSchemaColumnIndex("t_skladDate" );
      /* 05 */ CI.t_tt           = GetSchemaColumnIndex("t_tt"        );
      /* 06 */ CI.t_ttNum        = GetSchemaColumnIndex("t_ttNum"     );
      /* 07 */ CI.t_ttSort       = GetSchemaColumnIndex("t_ttSort"    );
      /* 08 */ CI.t_artiklCD     = GetSchemaColumnIndex("t_artiklCD"  );
      /* 09 */ CI.t_skladCD      = GetSchemaColumnIndex("t_skladCD"   );
      /* 00 */ CI.t_artiklName   = GetSchemaColumnIndex("t_artiklName");
      /* 11 */ CI.t_kupdobCD     = GetSchemaColumnIndex("t_kupdob_cd" );
      /* 12 */ CI.t_serno        = GetSchemaColumnIndex("t_serno"     );

      /* 13 */ CI.t_paletaNo     = GetSchemaColumnIndex("t_paletaNo"  );
      /* 14 */ CI.t_dimX         = GetSchemaColumnIndex("t_dimX"      );
      /* 15 */ CI.t_dimY         = GetSchemaColumnIndex("t_dimY"      );
      /* 16 */ CI.t_dimZ         = GetSchemaColumnIndex("t_dimZ"      );
      /* 17 */ CI.t_komada       = GetSchemaColumnIndex("t_komada"    );
      /* 18 */ CI.t_kol          = GetSchemaColumnIndex("t_kol"       );
      /* 19 */ CI.t_grCD         = GetSchemaColumnIndex("t_grCD"      );
      /* 20 */ CI.t_isKomDummy   = GetSchemaColumnIndex("t_isKomDummy");
      /* 21 */ CI.t_decA         = GetSchemaColumnIndex("t_decA"      );
      /* 22 */ CI.t_decB         = GetSchemaColumnIndex("t_decB"      );
      /* 23 */ CI.t_decC         = GetSchemaColumnIndex("t_decC"      );
      /* 24 */ CI.t_rtrRecID     = GetSchemaColumnIndex("t_rtrRecID"  );

   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region GetRtranoList_For_SERNO or t_paletaNo or ProjektCD

   // JOSAVAC: sobaBr          = t_serno    
   // JOSAVAC: kupdobCD (gost) = t_paletaNo 

   internal static List<Rtrano> GetRtranoList_For_SobaBr(XSqlConnection dbConnection, string _sobaBr) { return GetRtranoList_For_SERNO(dbConnection, _sobaBr); }
   internal static List<Rtrano> GetRtranoList_For_SERNO (XSqlConnection dbConnection, string _serno)
   {
      List<Rtrano> rtranoList = new List<Rtrano>();

      if(_serno.IsEmpty()) return rtranoList;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtranoSchemaRows[ZXC.RtoCI.t_serno], "elSerno", _serno, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrano>(dbConnection, rtranoList, filterMembers, Rtrans.artiklOrderBy_ASC);

      return rtranoList;
   }

   internal static List<Rtrano> GetRtranoList_For_Gost    (XSqlConnection dbConnection, uint _kupdobCD) { return GetRtranoList_For_PaletaNo(dbConnection, _kupdobCD); }
   internal static List<Rtrano> GetRtranoList_For_PaletaNo(XSqlConnection dbConnection, uint _paletaNo)
   {
      List<Rtrano> rtranoList = new List<Rtrano>();

      if(_paletaNo.IsZero()) return rtranoList;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      filterMembers.Add(new VvSqlFilterMember(ZXC.RtranoSchemaRows[ZXC.RtoCI.t_paletaNo], "elPaletaNo", _paletaNo, " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Rtrano>(dbConnection, rtranoList, filterMembers, Rtrans.artiklOrderBy_ASC);

      return rtranoList;
   }

   internal static uint GetBOR_TtNum_ForSoba(XSqlConnection dbConnection, string sobaBr)
   {
      if(sobaBr.IsEmpty()) return 0;

      List<Rtrano> rtranoList = GetRtranoList_For_SobaBr(dbConnection, sobaBr);
      if(rtranoList != null && rtranoList.Count.NotZero()) return rtranoList.Last().T_ttNum;
      else                                                 return 0;
   }

   internal static uint GetBOR_TtNum_ForGost(XSqlConnection dbConnection, uint kupdobCD)
   {
      if(kupdobCD.IsZero()) return 0;

      List<Rtrano> rtranoList = GetRtranoList_For_Gost(dbConnection, kupdobCD);
      if(rtranoList != null && rtranoList.Count.NotZero()) return rtranoList.Last().T_ttNum;
      else                                                 return 0;
   }

   internal static string GetBOR_Soba_ForGost(XSqlConnection dbConnection, uint kupdobCD)
   {
      if(kupdobCD.IsZero()) return "";

      List<Rtrano> rtranoList = GetRtranoList_For_Gost(dbConnection, kupdobCD);
      if(rtranoList != null && rtranoList.Count.NotZero()) return rtranoList.Last().T_serno;
      else                                                 return "";
   }

   internal static List<Rtrano> GetRtranoList_ForProjektCD(XSqlConnection conn, string projektTT_And_TtNum)
   {
      List<Rtrano> projektRtranoList = new List<Rtrano>();

      if(projektTT_And_TtNum.IsEmpty()) return projektRtranoList;

      VvRptFilter rptFilter = new VvRptFilter();

      string tt;
      uint ttNum;

      Ftrans.ParseTipBr(projektTT_And_TtNum, out tt, out ttNum);

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt   ], "elTT"   , tt   , " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.ttNum], "elTtNum", ttNum, " = "));

      rptFilter.FilterMembers = filterMembers;

      ZXC.RtranoDao.LoadManyDocumentsTtranses(conn, projektRtranoList, rptFilter, "t_skladDate, t_ttSort, t_ttNum, t_serial");

      return projektRtranoList;
   }

   #endregion GetRtranoList_For_SERNO  or t_paletaNo

   #region Get_PCK_ArtiklInfo_List_ForArtiklAndSklad

#if STARI_NACIN_DELME

   public static List<PCK_Artikl> Get_PCK_ArtiklList_ByPCK_Baza_AndSklad_OLD_DELME(XSqlConnection conn, string _PCK_ArtCD, string _PCK_SklCD)
   {
      List<string> istaBazaArtiklCDlist = VvUserControl.ArtiklSifrar.Where(a => a.PCK_BazaCD == Artikl.Get_PCK_BazaCD(_PCK_ArtCD)).Select(a => a.ArtiklCD).ToList();

      List<PCK_Artikl> istaBaza_PCK_ArtiklList = new List<PCK_Artikl>();

      foreach(string currArtCD in istaBazaArtiklCDlist)
      {
         istaBaza_PCK_ArtiklList.AddRange(Get_PCK_Artikl_List_ByArtiklAndSklad(conn, currArtCD, _PCK_SklCD));
      }

      return istaBaza_PCK_ArtiklList;
   }

   public static List<PCK_Artikl> Get_PCK_Artikl_List_ByArtiklAndSklad/*_OLD_DELME*/(XSqlConnection conn, string _PCK_ArtCD, string _PCK_SklCD)
   {
      if(_PCK_ArtCD.IsEmpty()) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Stop, "Zadajte artikl!"); return null; }

      List<string> skladCDlist;

      if(_PCK_SklCD.NotEmpty()) skladCDlist = new List<string> { _PCK_SklCD };
      else                      skladCDlist = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, _PCK_ArtCD);

      List<PCK_Artikl>  ALL_SklCD_PCK_ArtiklList = new List<PCK_Artikl>();
      List<PCK_Artikl>  currSklCD_PCK_ArtiklList;
      List<PCK_Unikat>  currSklCD_PCK_UnikatList;

      foreach(string currSkladCD in skladCDlist)
      {
         currSklCD_PCK_ArtiklList = new List<PCK_Artikl>();
         currSklCD_PCK_UnikatList = new List<PCK_Unikat>();

         List<string> theSernoList = MixerDao.GetDistinctRtranoSernoForArtiklAndSklad(conn, _PCK_ArtCD, currSkladCD);

         Rtrano rtrano_rec;

         PCK_Artikl artiklInfoLine;
         PCK_Unikat  sernoInfoLine ;

         Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == _PCK_ArtCD);

         bool isTT_ulaz_or_MOC_or_MOS;

         foreach(string theSerno in theSernoList)
         {
            rtrano_rec = new Rtrano();

            MixerDao.Get_LastRtrano_ForSerno(conn, rtrano_rec, theSerno);

            if(rtrano_rec.T_artiklCD != artikl_rec.ArtiklCD) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Zadnja pojava serijskog broja [{0}] NIJE za artikl [{1}]\n\r\n\r{2}", theSerno, _PCK_ArtCD, rtrano_rec);

            isTT_ulaz_or_MOC_or_MOS = rtrano_rec.TtInfo.IsFinKol_U || rtrano_rec.TtInfo.Is_MOC_or_MOS_TT;

            if(rtrano_rec.T_skladCD == currSkladCD && // ovo izbacuje serno-ove rtrano-e koji nisu                        na zadanom skladistu 
               isTT_ulaz_or_MOC_or_MOS              ) // ovo izbacuje serno-ove rtrano-e koji nisu (ULAZ ili MOC ili MOS) na zadanom skladistu 
            {

               if(artikl_rec == null)
               {
                  ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "PCK_SernoInfo_Dao: nema artikla za rtrano\n\r\n\r{0}", rtrano_rec);
                  return null;
               }

             //sernoInfoLine = new PCK_SernoInfo_Line(theSerno, rtrano_rec.T_artiklCD, rtrano_rec.T_artiklName, artikl_rec.Grupa2CD, artikl_rec.Grupa3CD, "", rtrano_rec.R_PCK_RAM, rtrano_rec.R_PCK_HDD);
               sernoInfoLine = new PCK_Unikat        (theSerno, rtrano_rec.T_artiklCD, rtrano_rec.T_artiklName, artikl_rec.Grupa2CD, artikl_rec.Grupa3CD, "", rtrano_rec.T_PCK_RAM, rtrano_rec.T_PCK_HDD);

               currSklCD_PCK_UnikatList.Add(sernoInfoLine);
            }
         }

         var sernoInfo_GRoups = currSklCD_PCK_UnikatList.GroupBy(sil => sil.PCK_RAM.ToString0Vv() + " / " + sil.PCK_HDD.ToString0Vv());

         foreach(var sernoInfo_GR in sernoInfo_GRoups)
         {
            artiklInfoLine = new PCK_Artikl(_PCK_ArtCD, artikl_rec.ArtiklName, sernoInfo_GR.First().PCK_RAMkind, sernoInfo_GR.First().PCK_HDDkind, currSkladCD, sernoInfo_GR.First().PCK_RAM, sernoInfo_GR.First().PCK_HDD);

            artiklInfoLine.PCK_Unikat_List = sernoInfo_GR.ToList();

            currSklCD_PCK_ArtiklList.Add(artiklInfoLine);
         }

         ALL_SklCD_PCK_ArtiklList.AddRange(currSklCD_PCK_ArtiklList);

      } // foreach(string currSkladCD in skladCDlist) 

      return ALL_SklCD_PCK_ArtiklList;
   }


#endif
 //public static List<PCK_Artikl> Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(XSqlConnection conn, Artikl artikl_rec, string skladCD, ZXC.PCK_Info_Kind info_Kind, bool wantsThisRAMkindOnly, bool wantsThisHDDkindOnly)
   public static List<PCK_Artikl> Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(XSqlConnection conn, Artikl artikl_rec, string skladCD, ZXC.PCK_Info_Kind info_Kind, string wantsThisRAMkindOnly, string wantsThisHDDkindOnly)
   {
      List<Artikl>     currSklCD_artiklList;
      List<PCK_Artikl> all_SklCD_PCK_ArtiklList = new List<PCK_Artikl>();


      List<string> skladCDlist;

      if(skladCD.NotEmpty())
      {
         skladCDlist = new List<string> { skladCD };
      }
      else
      {
         if(info_Kind == ZXC.PCK_Info_Kind.OvaBazaOnly) skladCDlist = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, artikl_rec.PCK_BazaCD, true);
         else                                           skladCDlist = ArtiklDao.GetDistinctSkladCdListForArtikl(conn, "");
      }

      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter() /*{ DatumOd = ZXC.projectYearFirstDay, DatumDo = DateTime.Today, Artikl_TS = ZXC.PCK_TS }*/;

      DataRowCollection   ArtSch = ZXC.ArtiklSchemaRows;
      ArtiklDao.ArtiklCI   ArtCI = ZXC.ArtiklDao.CI;
      DataRowCollection   ArsSch = ZXC.ArtStatSchemaRows;
      ArtStatDao.ArtStatCI ArsCI = ZXC.ArtStatDao.CI;

      switch(info_Kind)
      {
         case ZXC.PCK_Info_Kind.OvaBazaOnly:

            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artiklTS ], "theTS"      , ZXC.PCK_TS                         , " = "));
            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.carTarifa], "thePCK_Baza", artikl_rec.PCK_BazaCD              , " = "));
            break;

         case ZXC.PCK_Info_Kind.SveBazeOnly:

            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artiklTS ], "theTS"      , ZXC.PCK_TS                         , " = "));
            break;

         case ZXC.PCK_Info_Kind.SveBazeIkomponente:

            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artiklTS], ZXC.FM_OR_Enum.OPEN_OR , false, "theTS" , ZXC.PCK_TS , "", "", " = ", ""));
            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd1], ZXC.FM_OR_Enum.NONE    , false, "RAMgr1", ZXC.RAM_GR1, "", "", " = ", ""));
            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd1], ZXC.FM_OR_Enum.CLOSE_OR, false, "HDDgr1", ZXC.HDD_GR1, "", "", " = ", ""));
            break;

         case ZXC.PCK_Info_Kind.KomponenteOnly:

            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd1], ZXC.FM_OR_Enum.OPEN_OR , false, "RAMgr1", ZXC.RAM_GR1, "", "", " = ", ""));
            rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[ArsCI.artGrCd1], ZXC.FM_OR_Enum.CLOSE_OR, false, "HDDgr1", ZXC.HDD_GR1, "", "", " = ", ""));
            break;

      }

      if(artikl_rec != null && wantsThisRAMkindOnly.NotEmpty()) rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.grupa2CD], "wantsThisRAMkindOnly", wantsThisRAMkindOnly, " = "));
      if(artikl_rec != null && wantsThisHDDkindOnly.NotEmpty()) rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArtSch[ArtCI.grupa3CD], "wantsThisHDDkindOnly", wantsThisHDDkindOnly, " = "));
 
      foreach(string currSkladCD in skladCDlist.OrderBy(skl => skl))
      {
         currSklCD_artiklList = new List<Artikl>();

         ArtiklDao.GetArtiklWithArtstatList(conn, currSklCD_artiklList, currSkladCD, DateTime.Today, rptFilter, "", /*ArtiklOrderBy*/ "artiklCD");

         currSklCD_artiklList.ForEach(art => all_SklCD_PCK_ArtiklList.Add(new PCK_Artikl(art, currSkladCD, art.AS_StanjeKol)));
      }

      return all_SklCD_PCK_ArtiklList;
   }

   public static (PCK_Unikat thePCK_Unikat, Rtrano lastRtrano_rec) Get_PCK_Unikat_And_LastRtrano(XSqlConnection conn, string _theSerno)
   {
      List<Rtrano> thisSerno_All_RtranoList = GetRtranoList_For_SERNO(conn, _theSerno);

      if(thisSerno_All_RtranoList.IsEmpty()) return (null, null);

      Rtrano firstRtrano_rec = thisSerno_All_RtranoList.First();
      Rtrano lastRtrano_rec  = thisSerno_All_RtranoList.Last ();

      Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == lastRtrano_rec.T_artiklCD);

      PCK_Unikat thePCK_Unikat = new PCK_Unikat(_theSerno, lastRtrano_rec.T_artiklCD, lastRtrano_rec.T_artiklName, artikl_rec.Grupa2CD, artikl_rec.Grupa3CD, lastRtrano_rec.T_skladCD, lastRtrano_rec.T_PCK_RAM, lastRtrano_rec.T_PCK_HDD);

      thePCK_Unikat.PCK_SernoInfo_RtranoList = thisSerno_All_RtranoList;

      #region Check For Errors or Indiscrepancy

      string currSernoSignature = firstRtrano_rec.PCK_ArtiklInfo_Signature;
      string currSernoArtiklCD  = firstRtrano_rec.T_artiklCD              ;

      foreach(Rtrano rtrano in thisSerno_All_RtranoList)
      {
       //if(currSernoArtiklCD != rtrano.T_artiklCD) 
         if(Artikl.Has_equal_PCK_base(currSernoArtiklCD, rtrano.T_artiklCD) == false)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Serijskom broju: [{3}]\n\r\n\rje izmijenjena PCK Baza!!!\n\r\n\rSTARO: {0}\n\r\n\ra od stavke[{1}]\n\r\n\rNOVO: {2}",
               Artikl.Get_PCK_BazaCD(currSernoArtiklCD), rtrano, Artikl.Get_PCK_BazaCD(rtrano.T_artiklCD), _theSerno);
         }
         else if(currSernoSignature != rtrano.PCK_ArtiklInfo_Signature && !rtrano.TtInfo.Is_MOC_or_MOS_TT)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Serijskom broju: [{3}]\n\r\n\rPCK 'potpis' je izmijenjen bez modifikacije.\n\r\n\rSTARO: {0}\n\r\n\ra od stavke[{1}]\n\r\n\rNOVO: {2}", currSernoSignature, rtrano, rtrano.PCK_ArtiklInfo_Signature, _theSerno);
         }

         currSernoArtiklCD  = rtrano.T_artiklCD              ;
         currSernoSignature = rtrano.PCK_ArtiklInfo_Signature;
      }

      #endregion Check For Errors or Indiscrepancy

      return (thePCK_Unikat, lastRtrano_rec);
   }

   public static string GetR_opisOLD(XSqlConnection conn, Rtrano rtrano_rec)
   {
      (PCK_Unikat thePCK_Unikat, Rtrano lastRtrano_rec) = RtranoDao.Get_PCK_Unikat_And_LastRtrano(conn, rtrano_rec.T_serno);

      List<Rtrano> thisSernoRtranoList = thePCK_Unikat.PCK_SernoInfo_RtranoList;

      int idxOfThisRtrano = thisSernoRtranoList.IndexOf(rtrano_rec);

      if(idxOfThisRtrano < 1) return "";

      return thisSernoRtranoList[idxOfThisRtrano - 1].T_grCD;
   }

   public static List<string> GetDistinctRtranoSernoForArtiklAndSklad(XSqlConnection conn, string _PCK_ArtCD, string _PCK_SklCD)
   {
      string theSerno;
      List<string> theSernoList = new List<string>();

      using(XSqlCommand cmd = VvSQL.GetDistinctRtranoSernoForArtiklAndSklad_Command(conn, _PCK_ArtCD, _PCK_SklCD))
      {
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult /*| CommandBehavior.SingleRow*/))
         {
            while(reader.HasRows && reader.Read())
            {
               theSerno = reader.GetString(0);

               if(theSerno.NotEmpty())
               {
                  theSernoList.Add(theSerno);
               }
            }
            reader.Close();
         }
      }

      return theSernoList;
   }

   public static Rtrano Get_LastRtrano_ForSerno(XSqlConnection conn, Rtrano rtrano_rec, string _theSerno)
   {
      return Get_LastRtrano_ForSerno(conn, rtrano_rec, _theSerno, false);
   }
   public static Rtrano Get_LastRtrano_ForSerno(XSqlConnection conn, Rtrano rtrano_rec, string _theSerno, bool shouldBeSilent) 
   { 
      bool success = true;

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      DataRowCollection  RtoSch = ZXC.RtranoDao.TheSchemaTable.Rows;
      RtranoDao.RtranoCI RtoCI  = ZXC.RtranoDao.CI;

      filterMembers.Add(new VvSqlFilterMember(RtoSch[RtoCI.t_serno], "theSerno", _theSerno, " = "));

      string orderBy = Rtrans.artiklOrderBy_DESC.Replace("t_serial DESC ", "t_serial "); // jer VvSql.GetLastRecordBySomeOrder_Command doda "DESC LIMIT 1" 

      success = ZXC.RtranoDao.GetLastRecordBySomeOrder(conn, rtrano_rec, filterMembers, orderBy, shouldBeSilent, false);

      return rtrano_rec;
   }

   #endregion Get_PCK_ArtiklInfo_List_ForArtiklAndSklad

}
