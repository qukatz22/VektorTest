   using System;
using System.Data;
using System.Collections.Generic;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using                  MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using XSqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;
#endif

public sealed class PrjktDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static PrjktDao instance;

   private static KupdobDao theKupdobDao;

   private PrjktDao(XSqlConnection conn, string dbName) : base(dbName, Prjkt.recordNameArhiva, conn)// nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PrjktDao Instance(XSqlConnection conn, string dbName, KupdobDao _kupdobDao)
   {
      theKupdobDao = _kupdobDao;
      // Uses lazy initialization
      if (instance == null)
      {
         instance = new PrjktDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePrjkt

   public static   uint TableVersionStatic { get { return 1; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_PrjktExtensions_definition()
   {
      return (
         "sudCity  varchar(32)   NOT NULL default ''           COMMENT 'trgovacki sud u ...',\n" +
         "mbsRbu   varchar(10)   NOT NULL default ''           COMMENT 'MatBrSubj/RegBrUloska',\n" +
         "dateOsn  date          NOT NULL default '0001-01-01' COMMENT 'Datum osnivanja',\n" +
         "placaTip char(1)       NOT NULL default '',\n" +
         "toPayA   decimal(12,2) NOT NULL default '0.00' COMMENT 'Za fakturirati',\n" +
         "toPayB   decimal(12,2) NOT NULL default '0.00' COMMENT 'fuse decimal 1',\n" +
         "toPayC   decimal(12,2) NOT NULL default '0.00' COMMENT 'fuse decimal 2',\n" +
         "temKapit decimal(12,2) NOT NULL default '0.00' COMMENT 'temeljni kapital',\n" +
         "pidSume  decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr za sume',\n" +
         "pidSRent decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr spomenRenta',\n" +
         "pidTurst decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr za turisticku',\n" +
         "pidDobit decimal(12,2) NOT NULL default '0.00' COMMENT 'porezNaDobit',\n" +
         "pidKomor decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr komorski',\n" +
         "pidDohod decimal(12,2) NOT NULL default '0.00' COMMENT 'porezNaDohodak',\n" +
         "pidMO1   decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr mio 1',\n" +
         "pidMO2   decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr mio 2',\n" +
         "pidZdr   decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr zdravstvo',\n" +
         "pidZor   decimal(12,2) NOT NULL default '0.00' COMMENT 'dopr ozlj na radu',\n" +
         "isSkip   tinyint(1)   unsigned NOT NULL default 0 COMMENT 'izuzet - should skip',\n" +
         "isOver20 tinyint(1)   unsigned NOT NULL default 0 COMMENT 'preko 20 zaposlenih',\n" +
         "isZorNa  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'placa li dopr zor',\n" +
         "isJednK  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'jednostrano knjigovodstvo',\n" +
         "isDobit  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'obrtnik dobitas',\n" +
         "isAuthn  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'should authorize',\n" +
         "isArhiv  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'should arhivize',\n" +
         "isTrgRs  tinyint(1)   unsigned NOT NULL default 0 COMMENT 'placa: trgovacki fond sati',\n" +
         "memoHeader varchar(2048)       NOT NULL default '',\n" +
         "memoFooter varchar(2048)       NOT NULL default '',\n" +
         "theLogo    MEDIUMBLOB                             ,\n" +
         "belowGrid  varchar(2048)       NOT NULL default '',\n" +
         "isNoMinus  tinyint(1) unsigned NOT NULL default 0 COMMENT 'ne dozvoli minus',\n" +
         "porIspost  varchar(32)         NOT NULL default '',\n" +
         "isChkPrKol tinyint(1) unsigned NOT NULL default 0 COMMENT 'provjeri prim vs narudzb kol',\n" +
         "rvrOd      datetime            NOT NULL default '0001-01-01 00:00:00' COMMENT 'Radno Vrijeme OD',\n" +
         "rvrDo      datetime            NOT NULL default '0001-01-01 00:00:00' COMMENT 'Radno Vrijeme DO',\n" +
         "isFiskalOnline tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "certFile       MEDIUMBLOB                                ,\n" +
         "certPasswd     varchar(32)         NOT NULL default ''   ,\n" +
         "isNoTtNumChk   tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "isFiskCashOnly tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "fiskTtOnly     char(3)             NOT NULL default ''   ,\n" +
         "isNeprofit     tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "skySrvrHost    varchar(32)         NOT NULL default ''   ,\n" +
         "skyPassword    varchar(128)        NOT NULL default ''   ,\n" +
         "skyVvDomena    varchar(6)          NOT NULL default ''   ,\n" +
         "vrKoefBr1      decimal(12,2)       NOT NULL default '0.0',\n" +
       //"stStz2029      decimal(4,2)        NOT NULL default '0.0',\n" +
       //"stStz3034      decimal(4,2)        NOT NULL default '0.0',\n" +
       //"stStz3500      decimal(4,2)        NOT NULL default '0.0',\n" +
         "isObustOver3   tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "isCheckStaz    tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "isObrStazaLast tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "isSkipStzOnBol tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "isFullStzOnPol tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "theLogo2       MEDIUMBLOB                                ,\n" +
         "rnoRkp         varchar(32)         NOT NULL default ''   ,\n" +
         "shouldPeriodLock tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "periodLockDay    tinyint(2) unsigned NOT NULL default 0    ,\n" +
         "isBtchBookg      tinyint(1) unsigned NOT NULL default 0    ,\n" +
         "memoFooter2      varchar(2048)       NOT NULL default ''   ,\n" +
         "isNoAutoFiskal   tinyint(1) unsigned NOT NULL default 0    ,\n" +

         "m2pShaSec        varchar(128)        NOT NULL default ''   ,\n" +
         "m2pApikey        varchar(128)        NOT NULL default ''   ,\n" +
         "m2pSerno         varchar(16)         NOT NULL default ''   ,\n" +
         "m2pModel         varchar(16)         NOT NULL default ''   ,\n" +
         "f2_Provider      tinyint(1) unsigned NOT NULL default '0'  ,\n" +

         ""
      );
   }

   #endregion CreateTablePrjkt

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool isArhiva)
   {
      string preffix;
      Prjkt prjkt = (Prjkt)vvDataRecord;

      theKupdobDao.SetCommandParamValues(cmd, vvDataRecord, plt, isArhiva);
      
      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.SudCity,  TheSchemaTable.Rows[CI.sudCity]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.MbsRbu,   TheSchemaTable.Rows[CI.mbsRbu]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.DateOsn,  TheSchemaTable.Rows[CI.dateOsn]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PlacaTip, TheSchemaTable.Rows[CI.placaTip]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.ToPayA,   TheSchemaTable.Rows[CI.toPayA]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.ToPayB,   TheSchemaTable.Rows[CI.toPayB]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.ToPayC,   TheSchemaTable.Rows[CI.toPayC]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.TemKapit, TheSchemaTable.Rows[CI.temKapit]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidSume,  TheSchemaTable.Rows[CI.pidSume]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidSRent, TheSchemaTable.Rows[CI.pidSRent]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidTurst, TheSchemaTable.Rows[CI.pidTurst]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidDobit, TheSchemaTable.Rows[CI.pidDobit]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidKmDopr, TheSchemaTable.Rows[CI.pidKmDopr]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidKmClan, TheSchemaTable.Rows[CI.pidKmClan]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidMO1,   TheSchemaTable.Rows[CI.pidMO1]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidMO2,   TheSchemaTable.Rows[CI.pidMO2]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidZdr,   TheSchemaTable.Rows[CI.pidZdr]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PidZor,   TheSchemaTable.Rows[CI.pidZor]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsSkip,   TheSchemaTable.Rows[CI.isSkip]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsOver20, TheSchemaTable.Rows[CI.isOver20]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsZorNa,  TheSchemaTable.Rows[CI.isZorNa]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsJednK,  TheSchemaTable.Rows[CI.isJednK]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsDobit,  TheSchemaTable.Rows[CI.isDobit]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsAuthn,  TheSchemaTable.Rows[CI.isAuthn]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.UrSkipKolStSkl,  TheSchemaTable.Rows[CI.isArhiv]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IrSkipKolStSkl,  TheSchemaTable.Rows[CI.isTrgRs]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.MemoHeader    ,  TheSchemaTable.Rows[CI.memoHeader]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.MemoFooter    ,  TheSchemaTable.Rows[CI.memoFooter]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.MemoFooter2   ,  TheSchemaTable.Rows[CI.memoFooter2]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.TheLogo       ,  TheSchemaTable.Rows[CI.theLogo   ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.BelowGrid     ,  TheSchemaTable.Rows[CI.belowGrid ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsNoMinus     ,  TheSchemaTable.Rows[CI.isNoMinus ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PorezIspostCD ,  TheSchemaTable.Rows[CI.porIspost ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsChkPrKol    , TheSchemaTable.Rows[CI.isChkPrKol ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.RvrOd         , TheSchemaTable.Rows[CI.rvrOd      ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.RvrDo         , TheSchemaTable.Rows[CI.rvrDo      ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsFiskalOnline, TheSchemaTable.Rows[CI.isFiskalOnline]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.CertFile      , TheSchemaTable.Rows[CI.certFile]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.CertPasswdEncodedAsInFile, TheSchemaTable.Rows[CI.certPasswd]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsNoTtNumChk  , TheSchemaTable.Rows[CI.isNoTtNumChk]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsFiskCashOnly, TheSchemaTable.Rows[CI.isFiskCashOnly]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.FiskTtOnly    , TheSchemaTable.Rows[CI.fiskTtOnly]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsNeprofit    , TheSchemaTable.Rows[CI.isNeprofit]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.SkySrvrHostEncodedAsInFile, TheSchemaTable.Rows[CI.skySrvrHost]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.SkyPasswordEncodedAsInFile   , TheSchemaTable.Rows[CI.skyPassword]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.SkyVvDomena   , TheSchemaTable.Rows[CI.skyVvDomena]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.VrKoefBr1     , TheSchemaTable.Rows[CI.vrKoefBr1  ]);

    //VvSQL.CreateCommandParameter(cmd, preffix, prjkt.StStz2029     , TheSchemaTable.Rows[CI.stStz2029     ]);
    //VvSQL.CreateCommandParameter(cmd, preffix, prjkt.StStz3034     , TheSchemaTable.Rows[CI.stStz3034     ]);
    //VvSQL.CreateCommandParameter(cmd, preffix, prjkt.StStz3500     , TheSchemaTable.Rows[CI.stStz3500     ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsObustOver3  , TheSchemaTable.Rows[CI.isObustOver3  ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsCheckStaz   , TheSchemaTable.Rows[CI.isCheckStaz   ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsObrStazaLast, TheSchemaTable.Rows[CI.isObrStazaLast]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsSkipStzOnBol, TheSchemaTable.Rows[CI.isSkipStzOnBol]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsFullStzOnPol, TheSchemaTable.Rows[CI.isFullStzOnPol]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.TheLogo2      , TheSchemaTable.Rows[CI.theLogo2      ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.RnoRkp        , TheSchemaTable.Rows[CI.rnoRkp        ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.ShouldPeriodLock, TheSchemaTable.Rows[CI.shouldPeriodLock]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.PeriodLockDay   , TheSchemaTable.Rows[CI.periodLockDay   ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsBtchBookg     , TheSchemaTable.Rows[CI.isBtchBookg     ]);
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.IsNoAutoFiskal  , TheSchemaTable.Rows[CI.isNoAutoFiskal  ]);

      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.M2PshaSecEncodedAsInFile, TheSchemaTable.Rows[CI.m2pShaSec]  );
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.M2PapikeyEncodedAsInFile, TheSchemaTable.Rows[CI.m2pApikey]  );
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.M2Pserno                , TheSchemaTable.Rows[CI.m2pSerno]   );
      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.M2Pmodel                , TheSchemaTable.Rows[CI.m2pModel]   );

      VvSQL.CreateCommandParameter(cmd, preffix, prjkt.F2_Provider             , TheSchemaTable.Rows[CI.f2_Provider]);

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      theKupdobDao.FillFromDataReader(vvDataRecord, reader, /*isArhiva*/ false /* hardcodirano jer ce ovaj lokalni if(isArhiva) obaviti posel */ );

      PrjktExtensionStruct rdrData = new PrjktExtensionStruct();

	   rdrData._sudCity  = reader.GetString  (CI.sudCity);
	   rdrData._mbsRbu   = reader.GetString  (CI.mbsRbu);
	   rdrData._dateOsn  = reader.GetDateTime(CI.dateOsn);
	   rdrData._placaTip = reader.GetChar    (CI.placaTip);
	   rdrData._toPayA   = reader.GetDecimal (CI.toPayA);  
	   rdrData._toPayB   = reader.GetDecimal (CI.toPayB);  
	   rdrData._toPayC   = reader.GetDecimal (CI.toPayC);  
	   rdrData._temKapit = reader.GetDecimal (CI.temKapit);
	   rdrData._pidSume  = reader.GetDecimal (CI.pidSume); 
	   rdrData._pidSRent = reader.GetDecimal (CI.pidSRent);
	   rdrData._pidTurst = reader.GetDecimal (CI.pidTurst);
	   rdrData._pidDobit = reader.GetDecimal (CI.pidDobit);
	   rdrData._pidKmDopr= reader.GetDecimal (CI.pidKmDopr);
	   rdrData._pidKmClan= reader.GetDecimal (CI.pidKmClan);
	   rdrData._pidMO1   = reader.GetDecimal (CI.pidMO1);  
	   rdrData._pidMO2   = reader.GetDecimal (CI.pidMO2);  
	   rdrData._pidZdr   = reader.GetDecimal (CI.pidZdr);  
	   rdrData._pidZor   = reader.GetDecimal (CI.pidZor);
	   rdrData._isSkip   = reader.GetBoolean (CI.isSkip);
      rdrData._isOver20 = reader.GetBoolean (CI.isOver20);
      rdrData._isZorNa  = reader.GetBoolean (CI.isZorNa);
      rdrData._isJednK  = reader.GetBoolean (CI.isJednK);
      rdrData._isDobit  = reader.GetBoolean (CI.isDobit);
      rdrData._isAuthn  = reader.GetBoolean (CI.isAuthn);
      rdrData._isArhiv  = reader.GetBoolean (CI.isArhiv); 
      rdrData._isTrgRS  = reader.GetBoolean (CI.isTrgRs); 

	   rdrData._memoHeader = reader.GetString(CI.memoHeader);
      rdrData._memoFooter = reader.GetString(CI.memoFooter);
      rdrData._memoFooter2= reader.GetString(CI.memoFooter2);
      rdrData._isNoAutoisFiskal = reader.GetBoolean(CI.isNoAutoFiskal);

      rdrData._m2pShaSec = reader.GetString(CI.m2pShaSec);
      rdrData._m2pApikey = reader.GetString(CI.m2pApikey);
      rdrData._m2pSerno  = reader.GetString(CI.m2pSerno );
      rdrData._m2pModel  = reader.GetString(CI.m2pModel );

      rdrData._f2_Provider = reader.GetUInt16(CI.f2_Provider);

      #region Tu budalasaš dok ne savladas tehniku ucitavanja slika...

      try
      {
         long bytesNeeded = reader.GetBytes(CI.theLogo, 0, null, 0, 0); // If you pass a null array to GetBytes or GetChars, the long value returned will be the total number of bytes or characters in the BLOB
       //if(bytesNeeded >   65535) // 64KiloBytes
         if(bytesNeeded > 1048576) // Megabyte   
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Slika logo-a na Prjkt-u je prevelika (" + bytesNeeded + ").");
            throw new Exception("Slika ti je prevelika.");
         }
         /*if(rdrData._theLogo == null)*/ rdrData._theLogo = new byte[bytesNeeded]; 
         rdrData._theLogo = (byte[])reader[CI.theLogo];
         //reader.GetBytes(CI.theLogo, 0, rdrData._theLogo, 0, 0);
      }
      catch
      {
         rdrData._theLogo = null;
      }
      #endregion Tu budalasaš dok ne savladas tehniku ucitavanja slika...

      rdrData._belowGrid  = reader.GetString (CI.belowGrid);
    //rdrData._isNoMinus  = reader.GetBoolean(CI.isNoMinus);
      rdrData._isNoMinus  = reader.GetUInt16 (CI.isNoMinus);
      rdrData._porIspost  = reader.GetString (CI.porIspost);
      rdrData._isChkPrKol = reader.GetBoolean(CI.isChkPrKol);
	   rdrData._rvrOd      = reader.GetDateTime(CI.rvrOd);
	   rdrData._rvrDo      = reader.GetDateTime(CI.rvrDo);
      rdrData._isFiskalOnline = reader.GetBoolean(CI.isFiskalOnline);

      #region Tu budalasaš dok ne savladas tehniku ucitavanja blob...

      try
      {
         long bytesNeeded = reader.GetBytes(CI.certFile, 0, null, 0, 0); // If you pass a null array to GetBytes or GetChars, the long value returned will be the total number of bytes or characters in the BLOB
         //if(bytesNeeded >   65535) // 64KiloBytes
         if(bytesNeeded > 1048576) // Megabyte   
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Certifikat file na Prjkt-u je prevelik (" + bytesNeeded + ").");
            throw new Exception("CertFile ti je prevelik.");
         }
         /*if(rdrData._theLogo == null)*/
         rdrData._certFile = new byte[bytesNeeded];
         rdrData._certFile = (byte[])reader[CI.certFile];
         //reader.GetBytes(CI.theLogo, 0, rdrData._theLogo, 0, 0);
      }
      catch
      {
         rdrData._certFile = null;
      }

      #endregion Tu budalasaš dok ne savladas tehniku ucitavanja slika...

	   rdrData._certPasswd     = reader.GetString (CI.certPasswd    );
      rdrData._isNoTtNumChk   = reader.GetBoolean(CI.isNoTtNumChk  );
      rdrData._isFiskCashOnly = reader.GetBoolean(CI.isFiskCashOnly);
      rdrData._fiskTtOnly     = reader.GetString(CI.fiskTtOnly     );
      rdrData._isNeprofit     = reader.GetBoolean(CI.isNeprofit    );
	   rdrData._skySrvrHost    = reader.GetString (CI.skySrvrHost   );
	   rdrData._skyPassword    = reader.GetString (CI.skyPassword   );
	   rdrData._skyVvDomena    = reader.GetString (CI.skyVvDomena   );
	   rdrData._vrKoefBr1      = reader.GetDecimal(CI.vrKoefBr1     );
	   
    //rdrData._stStz2029      = reader.GetDecimal(CI.stStz2029     );
    //rdrData._stStz3034      = reader.GetDecimal(CI.stStz3034     );
    //rdrData._stStz3500      = reader.GetDecimal(CI.stStz3500     );
      rdrData._isObustOver3   = reader.GetBoolean(CI.isObustOver3  );
      rdrData._isCheckStaz    = reader.GetBoolean(CI.isCheckStaz   );
      rdrData._isObrStazaLast = reader.GetBoolean(CI.isObrStazaLast);
      rdrData._isSkipStzOnBol = reader.GetBoolean(CI.isSkipStzOnBol);
      rdrData._isFullStzOnPol = reader.GetBoolean(CI.isFullStzOnPol);

      #region Tu budalasaš dok ne savladas tehniku ucitavanja slika...

      try
      {
         long bytesNeeded = reader.GetBytes(CI.theLogo2, 0, null, 0, 0); // If you pass a null array to GetBytes or GetChars, the long value returned will be the total number of bytes or characters in the BLOB
       //if(bytesNeeded >   65535) // 64KiloBytes
         if(bytesNeeded > 1048576) // Megabyte   
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Slika logo-a na Prjkt-u je prevelika (" + bytesNeeded + ").");
            throw new Exception("Slika ti je prevelika.");
         }
         /*if(rdrData._theLogo2 == null)*/ rdrData._theLogo2 = new byte[bytesNeeded]; 
         rdrData._theLogo2 = (byte[])reader[CI.theLogo2];
         //reader.GetBytes(CI.theLogo2, 0, rdrData._theLogo2, 0, 0);
      }
      catch
      {
         rdrData._theLogo2 = null;
      }

      #endregion Tu budalasaš dok ne savladas tehniku ucitavanja slika...

      rdrData._rnoRkp         = reader.GetString (CI.rnoRkp        );

      rdrData._shouldPeriodLock= reader.GetBoolean(CI.shouldPeriodLock);
      rdrData._periodLockDay   = reader.GetUInt16 (CI.periodLockDay   );

      rdrData._isBtchBookg     = reader.GetBoolean(CI.isBtchBookg     );

      ((Prjkt)vvDataRecord).CurrentExtData = rdrData;

      if(isArhiva)
      {
         VvArhivaStruct rdrDataArhiva = new VvArhivaStruct();

         rdrDataArhiva._origRecID = reader.GetUInt32  (CI.origRecID);
         rdrDataArhiva._recVer    = reader.GetUInt32  (CI.recVer);
         rdrDataArhiva._arAction  = reader.GetString  (CI.arAction);
         rdrDataArhiva._arTS      = reader.GetDateTime(CI.arTS);
         rdrDataArhiva._arUID     = reader.GetString  (CI.arUID);

         vvDataRecord.TheArhivaData = rdrDataArhiva;
      }

      return;
   }

   #endregion FillFromDataReader

   #region PrjktCI struct & InitializeSchemaColumnIndexes()

   public struct PrjktCI
   {
	   internal int sudCity;
	   internal int mbsRbu;
	   internal int dateOsn;
	   internal int placaTip;
	   internal int toPayA;  
	   internal int toPayB;  
	   internal int toPayC;  
	   internal int temKapit;
	   internal int pidSume; 
	   internal int pidSRent;
	   internal int pidTurst;
	   internal int pidDobit;
	   internal int pidKmDopr;
	   internal int pidKmClan;
	   internal int pidMO1;  
	   internal int pidMO2;  
	   internal int pidZdr;  
	   internal int pidZor;
	   internal int isSkip;  
	   internal int isOver20;
	   internal int isZorNa; 
	   internal int isJednK; 
	   internal int isDobit; 
	   internal int isAuthn;
      internal int isArhiv;
      internal int isTrgRs;
      internal int memoHeader;
      internal int memoFooter;
      internal int memoFooter2;
      internal int theLogo;
      internal int belowGrid;
      internal int isNoMinus;
      internal int porIspost;
      internal int isChkPrKol;
      internal int rvrOd;
      internal int rvrDo;
      internal int isFiskalOnline;
      internal int certFile;
      internal int certPasswd;
      internal int isNoTtNumChk;
      internal int isFiskCashOnly;
      internal int fiskTtOnly;
      internal int isNeprofit;
      internal int skySrvrHost;
      internal int skyPassword;
      internal int skyVvDomena;
      internal int vrKoefBr1  ;
    //internal int stStz2029     ;
    //internal int stStz3034     ;
    //internal int stStz3500     ;
      internal int isObustOver3  ;
      internal int isCheckStaz   ;
      internal int isObrStazaLast;
      internal int isSkipStzOnBol;
      internal int isFullStzOnPol;
      internal int theLogo2      ;
      internal int rnoRkp        ;
      internal int shouldPeriodLock;
      internal int periodLockDay   ;
      internal int isBtchBookg     ;
      internal int isNoAutoFiskal  ;
      internal int m2pShaSec       ;
      internal int m2pApikey       ;
      internal int m2pSerno        ;
      internal int m2pModel        ;
      internal int f2_Provider     ;

      internal int origRecID;
      internal int recVer;
      internal int arAction;
      internal int arTS;
      internal int arUID;
   }

   /// <summary>
   /// CI ti je Column Indexes in SchemaTable
   /// </summary>
   public PrjktCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.sudCity  = GetSchemaColumnIndex("sudCity");
      CI.mbsRbu   = GetSchemaColumnIndex("mbsRbu");
      CI.dateOsn  = GetSchemaColumnIndex("dateOsn");
      CI.placaTip = GetSchemaColumnIndex("placaTip");
      CI.toPayA   = GetSchemaColumnIndex("toPayA");
      CI.toPayB   = GetSchemaColumnIndex("toPayB");
      CI.toPayC   = GetSchemaColumnIndex("toPayC");
      CI.temKapit = GetSchemaColumnIndex("temKapit");
      CI.pidSume  = GetSchemaColumnIndex("pidSume");
      CI.pidSRent = GetSchemaColumnIndex("pidSRent");
      CI.pidTurst = GetSchemaColumnIndex("pidTurst");
      CI.pidDobit = GetSchemaColumnIndex("pidDobit");
      CI.pidKmDopr= GetSchemaColumnIndex("pidKomor");
      CI.pidKmClan= GetSchemaColumnIndex("pidDohod");
      CI.pidMO1   = GetSchemaColumnIndex("pidMO1");
      CI.pidMO2   = GetSchemaColumnIndex("pidMO2");
      CI.pidZdr   = GetSchemaColumnIndex("pidZdr");
      CI.pidZor   = GetSchemaColumnIndex("pidZor");
      CI.isSkip   = GetSchemaColumnIndex("isSkip");
      CI.isOver20 = GetSchemaColumnIndex("isOver20");
      CI.isZorNa  = GetSchemaColumnIndex("isZorNa");
      CI.isJednK  = GetSchemaColumnIndex("isJednK");
      CI.isDobit  = GetSchemaColumnIndex("isDobit");
      CI.isAuthn  = GetSchemaColumnIndex("isAuthn");
      CI.isArhiv  = GetSchemaColumnIndex("isArhiv");
      CI.isTrgRs  = GetSchemaColumnIndex("isTrgRs");
      CI.memoHeader = GetSchemaColumnIndex("memoHeader");
      CI.memoFooter = GetSchemaColumnIndex("memoFooter");
      CI.memoFooter2= GetSchemaColumnIndex("memoFooter2");
      CI.theLogo    = GetSchemaColumnIndex("theLogo");
      CI.theLogo    = GetSchemaColumnIndex("theLogo");
      CI.belowGrid  = GetSchemaColumnIndex("belowGrid");
      CI.isNoMinus  = GetSchemaColumnIndex("isNoMinus");
      CI.porIspost  = GetSchemaColumnIndex("porIspost");
      CI.isChkPrKol = GetSchemaColumnIndex("isChkPrKol");
      CI.rvrOd      = GetSchemaColumnIndex("rvrOd");
      CI.rvrDo      = GetSchemaColumnIndex("rvrDo");
      CI.isFiskalOnline = GetSchemaColumnIndex("isFiskalOnline");
      CI.certFile       = GetSchemaColumnIndex("certFile");
      CI.certPasswd     = GetSchemaColumnIndex("certPasswd");
      CI.isNoTtNumChk   = GetSchemaColumnIndex("isNoTtNumChk");
      CI.isFiskCashOnly = GetSchemaColumnIndex("isFiskCashOnly");
      CI.fiskTtOnly     = GetSchemaColumnIndex("fiskTtOnly");
      CI.isNeprofit     = GetSchemaColumnIndex("isNeprofit");
      CI.skySrvrHost    = GetSchemaColumnIndex("skySrvrHost"     );
      CI.skyPassword    = GetSchemaColumnIndex("skyPassword"     );
      CI.skyVvDomena    = GetSchemaColumnIndex("skyVvDomena"     );
      CI.vrKoefBr1      = GetSchemaColumnIndex("vrKoefBr1"       );
    //CI.stStz2029      = GetSchemaColumnIndex("stStz2029"       );
    //CI.stStz3034      = GetSchemaColumnIndex("stStz3034"       );
    //CI.stStz3500      = GetSchemaColumnIndex("stStz3500"       );
      CI.isObustOver3   = GetSchemaColumnIndex("isObustOver3"    );
      CI.isCheckStaz    = GetSchemaColumnIndex("isCheckStaz"     );
      CI.isObrStazaLast = GetSchemaColumnIndex("isObrStazaLast"  );
      CI.isSkipStzOnBol = GetSchemaColumnIndex("isSkipStzOnBol"  );
      CI.isFullStzOnPol = GetSchemaColumnIndex("isFullStzOnPol"  );
      CI.theLogo2       = GetSchemaColumnIndex("theLogo2"        );
      CI.rnoRkp         = GetSchemaColumnIndex("rnoRkp"          );
      CI.shouldPeriodLock= GetSchemaColumnIndex("shouldPeriodLock");
      CI.periodLockDay   = GetSchemaColumnIndex("periodLockDay");
      CI.isBtchBookg     = GetSchemaColumnIndex("isBtchBookg");
      CI.isNoAutoFiskal  = GetSchemaColumnIndex("isNoAutoFiskal");
      CI.m2pShaSec       = GetSchemaColumnIndex("m2pShaSec");
      CI.m2pApikey       = GetSchemaColumnIndex("m2pApikey");
      CI.m2pSerno        = GetSchemaColumnIndex("m2pSerno" );
      CI.m2pModel        = GetSchemaColumnIndex("m2pModel" );
      CI.f2_Provider     = GetSchemaColumnIndex("f2_Provider");

      CI.origRecID      = GetSchemaColumnIndex("origRecID");
      CI.recVer         = GetSchemaColumnIndex("recVer");
      CI.arAction       = GetSchemaColumnIndex("arAction");
      CI.arTS           = GetSchemaColumnIndex("arTS");
      CI.arUID          = GetSchemaColumnIndex("arUID");
   }

   #endregion PrjktCI struct & InitializeSchemaColumnIndexes()


   #region IsThisRecordInSomeRelation

   public override bool IsThisRecordInSomeRelation(ZXC.PrivilegedAction action, XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      bool inRelation;
      int? recCount;
      Prjkt prjkt_rec = (Prjkt)vvDataRecord;

      //uint prjktID = prjkt_rec.KupdobCD/*RecID*/;
      uint kupdobCD = (action == ZXC.PrivilegedAction.DELREC ? prjkt_rec.KupdobCD : prjkt_rec.BackupedKupdobCD);
      string ticker = (action == ZXC.PrivilegedAction.DELREC ? prjkt_rec.Ticker   : prjkt_rec.BackupedTicker  );

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted prjkt only                                                                                                                                            
      DataRow drSchema = ZXC.PrvlgDao.TheSchemaTable.Rows[ZXC.PrvlgDao.CI.prjktID];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPrjkt", kupdobCD, " = "));

      recCount = CountRecords(conn, filterMembers);

      if(recCount > 0) inRelation = true;
      else             inRelation = false;

      if(inRelation)
      {
         string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(drSchema["BaseTableName"]));
         Issue_RecordIsInSomeRelation_Mesage(recordName, kupdobCD, (int)recCount);
      }

      //========================================
      string dbName = ZXC.VvDB_NameConstructor(ZXC.projectYear, ticker, kupdobCD);
      bool dbExists = VvSQL.CHECK_DATABASE_EXISTS(conn, dbName);
      if(dbExists)
      {
         System.Windows.Forms.MessageBox.Show("DATABASE: <" + dbName + "> POSTOJI!\n\n AKCIJA ODBIJENA.",
            "Akcija odbijena radi narušavanja integriteta podataka!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
      }

      //========================================

      return(inRelation || dbExists);
   }

   #endregion IsThisRecordInSomeRelation


   #region Set_IMPORT_OFFIX_Columns

   //  //____ Specifics 2 start ______________________________________________________

   
   //if(koris_rec[0].g_dummy1[0]) continue; // !!! 'izuzet' 
   
   //fprintf(device, "%s\t", koris_rec[0].g_koris_cd);
   //fprintf(device, "%s\t", koris_rec[0].g_koris);
   //fprintf(device, "%s\t", koris_rec[0].g_adr2);
   //fprintf(device, "%s\t", koris_rec[0].g_prezime);
   //fprintf(device, "%s\t", koris_rec[0].g_adr1);
   //fprintf(device, "%s\t", koris_rec[0].g_ime);
   //fprintf(device, "%s\t", koris_rec[0].g_koris_long);
   //fprintf(device, "%d\t", koris_rec[0].g_pdv[0] == 'N' ? 0 : 1);
   //fprintf(device, "%s\t", koris_rec[0].g_zip);
   //fprintf(device, "%s\t", koris_rec[0].g_opcina);
   //fprintf(device, "%s\t", koris_rec[0].g_opcina_cd);
   //fprintf(device, "%s\t", koris_rec[0].g_zupanija);
   //fprintf(device, "%s\t", koris_rec[0].g_tel);
   //fprintf(device, "%s\t", koris_rec[0].g_fax);
   //fprintf(device, "%s%s\t", koris_rec[0].g_ziro1, koris_rec[0].g_ziro1add);
   //fprintf(device, "%s%s\t", koris_rec[0].g_ziro2, koris_rec[0].g_ziro2add);
   //fprintf(device, "%s%s\t", koris_rec[0].g_ziro3, koris_rec[0].g_ziro3add);
   //fprintf(device, "%s\t", koris_rec[0].g_banka1);
   //fprintf(device, "%s\t", koris_rec[0].g_banka2);
   //fprintf(device, "%s\t", koris_rec[0].g_banka3);
   //fprintf(device, "%s\t", koris_rec[0].g_napomena[0]);
   //fprintf(device, "%s\t", koris_rec[0].g_napomena[1]);
   //fprintf(device, "%s\t", koris_rec[0].g_date[0] ? GetMySqlDate(koris_rec[0].g_date) : "0001-01-01");
   //fprintf(device, "%s\t", koris_rec[0].g_sud);
   //fprintf(device, "%s\t", koris_rec[0].g_mbs);
   //fprintf(device, "%s\t", koris_rec[0].g_djelat_cd);
   //fprintf(device, "%s\t", koris_rec[0].g_djelat);
   //fprintf(device, "%s\t", koris_rec[0].g_regob);
   //fprintf(device, "%s\t", koris_rec[0].g_matbr);
   //fprintf(device, "%d\t", koris_rec[0].g_strangers[0] == 'X' ? 1 : 0);
   //fprintf(device, "%d\t", koris_rec[0].g_tip[0] == 'X' ? 1 : 0); // isObrt 
	//fprintf(device, "%s\t", koris_rec[0].g_dummy1); // IZUZET 
   //fprintf(device, "%s\t", koris_rec[0].g_dummy2); // sifra zupanije 
   //fprintf(device, "%d\t", koris_rec[0].g_zorNa[0] == 'X' ? 1 : 0);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_kapital);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_dob);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_fak);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_tur);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_num1); // komorski
   //fprintf(device, "%.2lf\t", koris_rec[0].g_num2); // komorski dohodak 
   //fprintf(device, "%s\t", koris_rec[0].g_someFuseFlag1); // TipPlace !? 
   //fprintf(device, "%d\t", koris_rec[0].g_someFuseFlag2[0] == 'X' ? 1 : 0); // preko 20 radnika 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double1); // spomen renta 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double2); // sume 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double3); // harac
   //fprintf(device, "%d\t", koris_rec[0].g_primitives[0] == 'X' ? 1 : 0); // jednostrano Knjizenja 
   //fprintf(device, "%s\t", koris_rec[0].g_oib);
   //fprintf(device, "%s\t", koris_rec[0].g_hnb); // daLije Materijalno i devizno. TO NEMAS u Vektor.Prjkt bussiness-u 
   //fprintf(device, "%d\t", koris_rec[0].g_dobitas[0] == 'X' ? 1 : 0); // obrtnik DOBITAS 
   //fprintf(device, "%s\t", koris_rec[0].g_email);
   //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[0]); // MO1 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[1]); // MO2 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[2]); // ZDR 
   //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[3]); // ZOR 
	
   //fprintf(device, "\n");
	
   //  //____ Specifics 2 end   ______________________________________________________

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "@KCD, "     + //fprintf(device, "%s\t", koris_rec[0].g_koris_cd);
         "naziv, "    + //fprintf(device, "%s\t", koris_rec[0].g_koris);
         "grad, "     + //fprintf(device, "%s\t", koris_rec[0].g_adr2);
         "prezime, "  + //fprintf(device, "%s\t", koris_rec[0].g_prezime);
         "ulica1, "   + //fprintf(device, "%s\t", koris_rec[0].g_adr1);
         "ime, "      + //fprintf(device, "%s\t", koris_rec[0].g_ime);
         "dugoIme, "  + //fprintf(device, "%s\t", koris_rec[0].g_koris_long);
         "isPdv, "    + //fprintf(device, "%s\t", koris_rec[0].g_pdv[0] == 'N' ? 0 : 1);
         "postaBr, "  + //fprintf(device, "%s\t", koris_rec[0].g_zip);
         "opcina, "   + //fprintf(device, "%s\t", koris_rec[0].g_opcina);
         "opcCd, "    + //fprintf(device, "%s\t", koris_rec[0].g_opcina_cd);
         "zupan, "    + //fprintf(device, "%s\t", koris_rec[0].g_zupanija);
         "tel1, "     + //fprintf(device, "%s\t", koris_rec[0].g_tel);
         "fax, "      + //fprintf(device, "%s\t", koris_rec[0].g_fax);
         "ziro1, "    + //fprintf(device, "%s%s\t", koris_rec[0].g_ziro1, koris_rec[0].g_ziro1add);
         "ziro2, "    + //fprintf(device, "%s%s\t", koris_rec[0].g_ziro2, koris_rec[0].g_ziro2add);
         "ziro3, "    + //fprintf(device, "%s%s\t", koris_rec[0].g_ziro3, koris_rec[0].g_ziro3add);
         "ziro1By, "  + //fprintf(device, "%s\t", koris_rec[0].g_banka1);
         "ziro2By, "  + //fprintf(device, "%s\t", koris_rec[0].g_banka2);
         "ziro3By, "  + //fprintf(device, "%s\t", koris_rec[0].g_banka3);
         "napom1, "   + //fprintf(device, "%s\t", koris_rec[0].g_napomena[0]);
         "napom2, "   + //fprintf(device, "%s\t", koris_rec[0].g_napomena[1]);
         "dateOsn, "  + //fprintf(device, "%s\t", koris_rec[0].g_date[0] ? GetMySqlDate(koris_rec[0].g_date) : "0001-01-01");
         "sudCity, "  + //fprintf(device, "%s\t", koris_rec[0].g_sud);
         "mbsRbu, "   + //fprintf(device, "%s\t", koris_rec[0].g_mbs);
         "sifDcd, "   + //fprintf(device, "%s\t", koris_rec[0].g_djelat_cd);
         "sifDname, " + //fprintf(device, "%s\t", koris_rec[0].g_djelat);
         "regob, "    + //fprintf(device, "%s\t", koris_rec[0].g_regob);
         "matbr, "    + //fprintf(device, "%s\t", koris_rec[0].g_matbr);
         "isFrgn, "   + //fprintf(device, "%d\t", koris_rec[0].g_strangers[0] == 'X' ? 1 : 0);
         "isObrt, "   + //fprintf(device, "%d\t", koris_rec[0].g_tip[0] == 'X' ? 1 : 0); // isObrt 
         "isSkip, "   + //fprintf(device, "%d\t", koris_rec[0].g_dummy1); // IZUZET 
         "zupCd, "    + //fprintf(device, "%s\t", koris_rec[0].g_dummy2); // sifra zupanije 
         "isZorNa, "  + //fprintf(device, "%d\t", koris_rec[0].g_zorNa[0] == 'X' ? 1 : 0);
         "temKapit, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_kapital);
         "pidDobit, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_dob);
         "toPayA, "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_fak);
         "pidTurst, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_tur);
         "pidKomor, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_num1); // komorski
         "pidDohod, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_num2); // komorski dohodak 
         "placaTip, " + //fprintf(device, "%s\t", koris_rec[0].g_someFuseFlag1); // TipPlace !? 
         "isOver20, " + //fprintf(device, "%d\t", koris_rec[0].g_someFuseFlag2[0] == 'X' ? 1 : 0); // preko 20 radnika 
         "pidSRent, " + //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double1); // spomen renta 
         "pidSume, "  + //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double2); // sume 
         "toPayB, "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_new_double3); // harac // TODO: !!! na UC-u! 
         "isJednK, "  + //fprintf(device, "%d\t", koris_rec[0].g_primitives[0] == 'X' ? 1 : 0); // jednostrano Knjizenja 
         "oib, "      + //fprintf(device, "%s\t", koris_rec[0].g_oib);
         "isXxx, "    + //fprintf(device, "%s\t", koris_rec[0].g_hnb); // daLije Materijalno i devizno. TO NEMAS u Vektor.Prjkt bussiness-u          TODO: !!! na UC-u! 
         "isDobit, "  + //fprintf(device, "%d\t", koris_rec[0].g_dobitas[0] == 'X' ? 1 : 0); // obrtnik DOBITAS 
         "email, "    + //fprintf(device, "%s\t", koris_rec[0].g_email);
         "pidMO1, "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[0]); // MO1 
         "pidMO2, "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[1]); // MO2 
         "pidZdr, "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[2]); // ZDR 
         "pidZor  "   + //fprintf(device, "%.2lf\t", koris_rec[0].g_fuse_d[3]); // ZOR 

         ")"    + "\n" +

         "SET " + "\n" +

         "kupdobCD = @KCD, " + "\n" +

         "ticker = CONCAT(SUBSTRING(naziv, 1, 3), SUBSTRING(@KCD, 4, 3)), " + "\n" +

         "ulica2 = ulica1, " + "\n" +

         "addTS = CURRENT_TIMESTAMP, modTS = addTS," + "\n" +
         "addUID = '" + ZXC.vvDB_User + "'";
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Prjkt>(conn, Translate437, null, "", ZXC.VvDB_prjktDB_Name /*ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName*/, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Prjkt prjkt_rec = vvDataRecord as Prjkt;

      prjkt_rec.Ticker  = prjkt_rec.Ticker .VvTranslate437ToLatin2();
      prjkt_rec.Naziv   = prjkt_rec.Naziv  .VvTranslate437ToLatin2();
      prjkt_rec.Ulica1  = prjkt_rec.Ulica1 .VvTranslate437ToLatin2();
      prjkt_rec.Ulica2  = prjkt_rec.Ulica2 .VvTranslate437ToLatin2();
      prjkt_rec.DugoIme = prjkt_rec.DugoIme.VvTranslate437ToLatin2();
      prjkt_rec.Grad    = prjkt_rec.Grad   .VvTranslate437ToLatin2();
      prjkt_rec.Ime     = prjkt_rec.Ime    .VvTranslate437ToLatin2();
      prjkt_rec.Opcina  = prjkt_rec.Opcina .VvTranslate437ToLatin2();
      prjkt_rec.Prezime = prjkt_rec.Prezime.VvTranslate437ToLatin2();
      prjkt_rec.Ziro1By = prjkt_rec.Ziro1By.VvTranslate437ToLatin2();
      prjkt_rec.Ziro2By = prjkt_rec.Ziro2By.VvTranslate437ToLatin2();
      prjkt_rec.Ziro3By = prjkt_rec.Ziro3By.VvTranslate437ToLatin2();
      prjkt_rec.Ziro4By = prjkt_rec.Ziro4By.VvTranslate437ToLatin2();
      prjkt_rec.Zupan   = prjkt_rec.Zupan  .VvTranslate437ToLatin2();
      prjkt_rec.SudCity = prjkt_rec.SudCity.VvTranslate437ToLatin2();
      
      return prjkt_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}
