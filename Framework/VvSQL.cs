using System;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using static ArtiklDao;

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
using System.Text;
#endif

public static class VvSQL
{

   #region Enums and Constants

   public enum ParamListType { Complete, Without_ID, ID_Only, Old_Values, LanSrvAndRecID_only };
   public enum DB_RW_ActionType { ADD, RWT, RWT_2, DEL, EQL, UTIL, NONE };
   public enum DB_RW_SyncType { T1_IRA_2_T2_URA };
   public enum DBNavigActionType { FRS, PRV, NXT, LST };
   public enum OrderDirectEnum { ASC, DESC };

   #endregion Enums and Constants

   #region ZXC like Commands

   public static XSqlCommand InitCommand(XSqlConnection connection)
   {
      XSqlCommand cmd = connection.CreateCommand();

      cmd.CommandType = CommandType.Text;

      cmd.UpdatedRowSource = UpdateRowSource.None;

      // 15.12.2017: 
      //cmd.CommandTimeout = 120;
      cmd.CommandTimeout = 480; // 8 min 
                                //cmd.CommandTimeout =  60; // 1 min 

      return (cmd);
   }

   //public static string Get_Sql_UserID(XSqlConnection conn)
   //{
   //   return Get_Sql_UserID(conn, false);
   //}

   public static string Get_Sql_UserID(XSqlConnection conn, bool hostNameWanted)
   {
      object retValAsObject;
      string strUser;

      using(XSqlCommand cmd = InitCommand(conn))
      {
         if(hostNameWanted)
            cmd.CommandText = "SELECT USER()";
         else
            cmd.CommandText = "SELECT SUBSTRING_INDEX(USER(),_utf8'@',1)";

         retValAsObject = cmd.ExecuteScalar();
      }

      strUser = /*uint.Parse*/(retValAsObject.ToString());

      return (strUser);
   }

   // OVO VRACA CLIENT NAME 
   public static string Get_Sql_Hostname(XSqlConnection conn)
   {
      object retValAsObject;
      string strClient;

      using(XSqlCommand cmd = InitCommand(conn))
      {
         cmd.CommandText = "SELECT SUBSTRING_INDEX(USER(),_utf8'@',-1)";

         retValAsObject = cmd.ExecuteScalar();
      }

      strClient = /*uint.Parse*/(retValAsObject.ToString());

      return (strClient);
   }

   // OVO VRACA SERVER NAME 
   public static string Get_Sql_ServerName(XSqlConnection conn)
   {
      object retValAsObject;
      string strServer;

      using(XSqlCommand cmd = InitCommand(conn))
      {
         cmd.CommandText = @"SELECT @@hostname";

         retValAsObject = cmd.ExecuteScalar();
      }

      strServer = /*uint.Parse*/(retValAsObject.ToString());

      return (strServer);
   }

   public static DateTime GetServer_DateTime_Now(XSqlConnection conn)
   {
      //MySql.Data.Types.MySqlDateTime  mySqlDateTime;
      DateTime dNetDateTime;

      using(XSqlCommand cmd = InitCommand(conn))
      {
         cmd.CommandText = "SELECT NOW()";
         using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
         {
            if(reader.HasRows && reader.Read())
            {
               //mySqlDateTime = reader.GetMySqlDateTime(0);
               //dNetDateTime  = mySqlDateTime.GetDateTime();

               dNetDateTime = reader.GetDateTime(0);

            }
            else dNetDateTime = DateTime.MinValue;

            reader.Close();

         } // using reader
      } // using cmd 

      return (dNetDateTime);
   }

   // OVO VRACA SERVER ID 
   public static uint Get_Sql_ServerID(XSqlConnection conn)
   {
      object retValAsObject;
      uint serverID;

      using(XSqlCommand cmd = InitCommand(conn))
      {
         cmd.CommandText = @"SELECT @@server_id";

         retValAsObject = cmd.ExecuteScalar();
      }

      serverID = ZXC.ValOrZero_UInt(retValAsObject.ToString());

      return (serverID);
   }

   // OVO VRACA NEXT RecID 
   public static uint Get_Sql_NextAutoIncrementRecID(XSqlConnection conn/*, string dbName*/, string tableName)
   {
      object retValAsObject;
      uint nextRecID;

      string dbName = conn.Database; // provjeri jel' ovo ok! 

      using(XSqlCommand cmd = InitCommand(conn))
      {
         cmd.CommandText =

            "SELECT AUTO_INCREMENT \n" +
            "FROM information_schema.TABLES \n" +
            "WHERE TABLE_SCHEMA = '" + dbName + "' \n" +
            "AND   TABLE_NAME   = '" + tableName + "' \n";

         retValAsObject = cmd.ExecuteScalar();
      }

      nextRecID = ZXC.ValOrZero_UInt(retValAsObject.ToString());

      return (nextRecID);
   }

   #endregion ZXC like Commands

   #region EQLREC_Command

   public static XSqlCommand EQLREC_byRecID_Command(XSqlConnection conn, VvDataRecord vvDataRecord, bool isArhiva)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecord.VirtualRecordName;

      cmd.CommandText = "SELECT * FROM " + tableName + " WHERE recID = ?prm_recID";

      //arhivedDataRecord.SetCommandParamValues(cmd, arhivedDataRecord, ParamListType.ID_Only);
      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.ID_Only, isArhiva);
      return (cmd);
   }

   public static XSqlCommand EQLREC_byLanSrvIDAndLanRecID_Command(XSqlConnection conn, VvDataRecord vvDataRecord, uint lanSrvID, uint lanRecID, bool isArhiva)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecord.VirtualRecordName;

      cmd.CommandText = "SELECT * FROM " + tableName + " WHERE lanSrvID = " + lanSrvID + " AND lanRecID = " + lanRecID;

      return (cmd);
   }

   public static XSqlCommand GetRecID_byLanSrvAndLanRecID_Command(XSqlConnection conn, VvDataRecord vvDataRecord, bool isArhiva)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecord.VirtualRecordName;

      cmd.CommandText = "SELECT recID FROM " + tableName + " WHERE lanSrvID = ?prm_lanSrvID AND lanRecID =?prm_lanRecID";

      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.Complete, isArhiva);
      return (cmd);
   }

   public static XSqlCommand GetExtenderRecID_ByFatherRecID_Command(XSqlConnection conn, VvDataRecord vvDataRecordFather, bool isArhiva)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecordFather.VirtualExtenderRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecordFather.VirtualExtenderRecord.VirtualRecordName;

      cmd.CommandText = "SELECT recID FROM " + tableName + " WHERE " + ((IVvExtenderDataRecord)(vvDataRecordFather.VirtualExtenderRecord)).JoinedColName + " = " + vvDataRecordFather.VirtualRecID;

      return (cmd);
   }

   public static XSqlCommand EQLREC_byEverything_Command(XSqlConnection conn, VvDataRecord vvDataRecord/*, bool isArhiva*/)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT * FROM " + vvDataRecord.VirtualRecordName + " WHERE " +
                        CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, DB_RW_ActionType.EQL, false /*isArhiva*/, false /*identifyByRecIDonly*/, false /*isSkyTraffic*/);

      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.Complete, false /*isArhiva*/);

      return (cmd);
   }

   public static XSqlCommand EQLREC_bySomeUniqueColumn_Command(XSqlConnection conn, object byThisValue, DataRow drSchemaSomeUniqueColumn, VvDataRecord vvDataRecord, bool isArhiva)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecord.VirtualRecordName;

      CreateCommandNamedParameter(cmd, "", "wantedValue", byThisValue, drSchemaSomeUniqueColumn);

      cmd.CommandText = "SELECT * FROM " + tableName + " WHERE " + drSchemaSomeUniqueColumn["ColumnName"] + " = ?wantedValue";

      return (cmd);
   }

   #endregion EQLREC_Command

   #region RecordSorter

   public enum SorterType { None, /*RecID, */Code, Ticker, Matbr, Name, DokNum, DokDate, TtNum, TtNum2, Person, City, Konto, KontoNaziv, OIB, BarCode, KpdbName, ArtStat, ArtTopByKol, ArtTopByFin, ArtTopByRuc, Serlot, NewRecID, s_lio, KCDnaziv, Name2, Code2, Serno };

   public enum RptOrderBy
   {
      XYZ_UNDEFINED,
      FIZ_Dnevnik_DokNum,
      FIZ_Dnevnik_DokDate,
      FIZ_KKonta_DokNum,
      FIZ_KKonta_DokDate,
      FIZ_Sconti_TipBr,
      FIZ_Sconti_DokDate,
      FIZ_ScontiMT_TipBr,
      FIZ_ScontiMT_DokDate,
      FIZ_AnalitikaProrTroska_DokDate,
      //FIZ_RekapNal_DokDate,
      //FIZ_RekapNal_DokNum,

      AIZ_OrderBy_OsredCD,
      AIZ_OrderBy_OsredNaziv,
      //AIZ_Dnevnik_DokNum,
      //AIZ_Dnevnik_DokDate,
      //AIZ_RekapAmo_DokDate,
      //AIZ_RekapAmo_DokNum
   };

   public struct IndexSegment
   {
      string fldName;
      XSqlDbType dbType;
      int paramSize;
      DataRow schemaTableDT;
      bool isArhiva;

      //public IndexSegment(string _fldName, XSqlDbType _dbType, int _paramSize)
      //{
      //   this.fldName   = _fldName;
      //   this.dbType    = _dbType;
      //   this.paramSize = _paramSize;
      //}

      public IndexSegment(DataRow schemaTableDataRow) : this(schemaTableDataRow, false)
      {
      }

      public IndexSegment(DataRow schemaTableDataRow, bool _isArhiva)
      {
         string _fldName = (string)schemaTableDataRow["ColumnName"];
         XSqlDbType _dbType = (XSqlDbType)schemaTableDataRow["ProviderType"];
         int _paramSize = (int)schemaTableDataRow["ColumnSize"];

         this.schemaTableDT = schemaTableDataRow;

         this.fldName = _fldName;
         this.dbType = _dbType;
         this.paramSize = _paramSize;

         this.isArhiva = _isArhiva;
      }

      public string FldName
      {
         get { return this.fldName; }
      }

      public XSqlDbType DbType
      {
         get { return this.dbType; }
      }

      public int ParamSize
      {
         get { return this.paramSize; }
      }

      public bool IsArhiva
      {
         get { return this.isArhiva; }
      }

      public DataRow DrSchema
      {
         get { return this.schemaTableDT; }
      }

   }

   public struct RecordSorter
   {
      string recName;
      string recNameArhiva;
      string comboText;
      bool biloGdjeUnazivu;
      VvSQL.SorterType sortType;
      VvSQL.IndexSegment[] iSegs;

      public RecordSorter(string _recName, string _recNameArhiva, VvSQL.IndexSegment[] _iSegs, string _comboText, VvSQL.SorterType _sortType, bool _biloGdjeUnazivu)
      {
         this.recName = _recName;
         this.recNameArhiva = _recNameArhiva;
         this.iSegs = _iSegs;
         this.comboText = _comboText;
         this.sortType = _sortType;
         this.biloGdjeUnazivu = _biloGdjeUnazivu;
      }


      public bool BiloGdjeU_Tekstu
      {
         get { return biloGdjeUnazivu; }
         set { biloGdjeUnazivu = value; }
      }

      public string RecName
      {
         get { return this.recName; }
         set { this.recName = value; }
      }

      public string RecNameArhiva
      {
         get { return this.recNameArhiva; }
         set { this.recNameArhiva = value; }
      }

      public VvSQL.IndexSegment[] IdxSegments
      {
         get { return this.iSegs; }
      }

      public string ComboText
      {
         get { return this.comboText; }
      }

      public VvSQL.SorterType SortType
      {
         get { return this.sortType; }
      }

      public override string ToString()
      {
         return this.ComboText;
      }
   }

   #endregion RecordSorter

   #region PRVREC_NXTREC_Command

   public static XSqlCommand PRVNXT_Command(XSqlConnection conn,
                                            VvDataRecord vvDataRecord,
                                            DBNavigActionType action,
                                            RecordSorter sorter,
                                            bool isArhiva,
                                            ZXC.DbNavigationRestrictor dbNavRestrictor_TT,
                                            ZXC.DbNavigationRestrictor dbNavRestrictor_SKL,
                                            ZXC.DbNavigationRestrictor dbNavRestrictor_SKL2)
   {
      string strSql;
      bool shouldRestrict_TT;
      bool shouldRestrict_SKL  = false;
      bool shouldRestrict_SKL2 = false;
      XSqlCommand cmd          = VvSQL.InitCommand(conn);

      string tableName;

      if(dbNavRestrictor_TT.Equals(ZXC.DbNavigationRestrictor.Empty)) shouldRestrict_TT = false;
      else                                                            shouldRestrict_TT = true;

           if(dbNavRestrictor_SKL.Equals(ZXC.DbNavigationRestrictor.Empty)) shouldRestrict_SKL = false;
      else if(ZXC.RRD.Dsc_IsSklRestrictor == true)                          shouldRestrict_SKL = true ; // Napravljeno za Likum, a smeta vecini ostalih pa treba u rulse... 

           if(dbNavRestrictor_SKL2.Equals(ZXC.DbNavigationRestrictor.Empty)) shouldRestrict_SKL2 = false;
      else if(ZXC.RRD.Dsc_IsSklRestrictor == true)                           shouldRestrict_SKL2 = true ; // Napravljeno za Likum, a smeta vecini ostalih pa treba u rulse... 
                                                                               // ... ma jebote, vec si u FakturDucBabies na vecini DUCeva overridao ovo ponasanje sa 'dbNavigationRestrictor_SKL = ZXC.DbNavigationRestrictor.Empty;'
                                                                               // ostalo je na IRMu pa si dosao ovdje. Ubuduce izjednaci nekako tu logiku i to preko rulsa 

      // 08.04.2018: 
      if(ZXC.IsSvDUH && action == DBNavigActionType.LST && dbNavRestrictor_SKL.ColName != null) shouldRestrict_SKL = true; // pokusaj, da im dolazi defaultno na SKL 10, a ne na ona veca 80, ... ali da ne ogranicima kad bas traze 

      if(ZXC.IsSvDUH_ZAHonly) shouldRestrict_SKL2 = true;

      // 12.20.2021: 
      shouldRestrict_TT   = shouldRestrict_TT   && dbNavRestrictor_TT  .NotEmpty;
      shouldRestrict_SKL  = shouldRestrict_SKL  && dbNavRestrictor_SKL .NotEmpty;
      shouldRestrict_SKL2 = shouldRestrict_SKL2 && dbNavRestrictor_SKL2.NotEmpty;

      // 16.05.2024: 
      if(ZXC.IsTETRAGRAM_ANY && ZXC.GetTetragram_PreferredSkladCD_LookUpItem() == null)
      {
         shouldRestrict_SKL  = 
         shouldRestrict_SKL2 = false;

      }

      // 21.10.2016: za 'CalcIBAN_2017_TEKUCI' potrebe 

      if(vvDataRecord is Ptrano && dbNavRestrictor_SKL.ColName == "t_isZastRn") shouldRestrict_SKL = true;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else         tableName = vvDataRecord.VirtualRecordName;

      strSql = "SELECT * FROM " + tableName;

      // 11.10.2021: kad je vvDataRecord Faktur, IsExtendable je ipak ovdje false?! pa OR-amo 
      if((vvDataRecord.IsExtendable || vvDataRecord is Faktur) && vvDataRecord.VirtualExtenderRecord != null)
         //if(vvDataRecord is Faktur) 
         strSql += " L " + "\n" +
                   "LEFT JOIN  " + vvDataRecord.VirtualExtenderRecord.VirtualRecordName + " R " + "\n" + "\n" +
                   "ON L.RecID = R." + ((IVvExtenderDataRecord)vvDataRecord.VirtualExtenderRecord).JoinedColName + "\n";

      switch(action)
      {
         case DBNavigActionType.PRV:
            strSql += " WHERE " + VvSQL.PRVNXT_WhereConditon(sorter.IdxSegments, " < ", isArhiva) +
                      (shouldRestrict_TT ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_TT) : "") +
                      (shouldRestrict_SKL ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL) : "") +
                      (shouldRestrict_SKL2 ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL2) : "") +
                      " ORDER BY " + VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "DESC", isArhiva) + " LIMIT 1";
            break;

         case DBNavigActionType.NXT:
            strSql += " WHERE " + VvSQL.PRVNXT_WhereConditon(sorter.IdxSegments, " > ", isArhiva) +
                      (shouldRestrict_TT ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_TT) : "") +
                      (shouldRestrict_SKL ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL) : "") +
                      (shouldRestrict_SKL2 ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL2) : "") +
                      " ORDER BY " + VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "ASC", isArhiva) + " LIMIT 1";
            break;

         case DBNavigActionType.FRS:
            strSql += (shouldRestrict_TT ? " WHERE " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_TT) : "") +
                      (shouldRestrict_SKL ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL) : "") +
                      (shouldRestrict_SKL2 ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL2) : "") +
                      " ORDER BY " + VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "ASC", isArhiva) + " LIMIT 1";
            break;

         case DBNavigActionType.LST:
            strSql += (shouldRestrict_TT ? " WHERE " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_TT) : "") +
                      (shouldRestrict_SKL ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL) : "") +
                      (shouldRestrict_SKL2 ? " AND " + VvSQL.NavRestrictorWhereCondition(dbNavRestrictor_SKL2) : "") +
                      " ORDER BY " + VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "DESC", isArhiva) + " LIMIT 1";

            // 26.04.2022: 
            //if(ZXC.IsSvDUH && dbNavRestrictor_SKL.RestrictedValues != null && dbNavRestrictor_SKL.RestrictedValues[0] == "10")
            // 07.06.2022:
            if(ZXC.IsSvDUH && dbNavRestrictor_SKL.RestrictedValues != null && dbNavRestrictor_SKL.RestrictedValues[0] == "10" &&
                              dbNavRestrictor_TT.RestrictedValues != null && dbNavRestrictor_TT.RestrictedValues[0] == Faktur.TT_IZD)
            {
               strSql = strSql.Replace("ORDER BY", "AND ttnum < 200000 ORDER BY"); // jer smo u sve dokumenta sa skl 70 i 90 prebacili na 10 a ostali su ttNum-ovi 7xxxxx i 9xxxxx 
            }

            break;

         default: System.Windows.Forms.MessageBox.Show("PRVNXT_Command: prv_or_nxt not set!");
            strSql = null;
            break;
      }

      SetNavigationalParam(vvDataRecord.SorterCurrVal(sorter.SortType), cmd, strSql, sorter, isArhiva, false);

      if(shouldRestrict_TT  ) SetRestrictorParam(cmd, dbNavRestrictor_TT  );
      if(shouldRestrict_SKL ) SetRestrictorParam(cmd, dbNavRestrictor_SKL );
      if(shouldRestrict_SKL2) SetRestrictorParam(cmd, dbNavRestrictor_SKL2);

      return cmd;
   }

   //private static string NavRestrictorWhereCondition(ZXC.DbNavigationRestrictor dbNavigationRestrictor)
   //{
   //   string strWhereCondition = "", fldName, paramName;

   //   fldName = dbNavigationRestrictor.ColName;

   //   paramName = "?restrictor_" + dbNavigationRestrictor.ColName;

   //   strWhereCondition = fldName + " = " + paramName;

   //   return strWhereCondition;
   //}

   private static string NavRestrictorWhereCondition(ZXC.DbNavigationRestrictor dbNavigationRestrictor)
   {
      // 12.10.2021:
      if(dbNavigationRestrictor.IsEmpty) return "";

      string fldName, paramName;
      System.Text.StringBuilder sbWhereCondition = new System.Text.StringBuilder();

      fldName = dbNavigationRestrictor.ColName;

      sbWhereCondition.Append("(");

      for(int i = 0; i < dbNavigationRestrictor.RestrictedValues.Length; ++i)
      {
         paramName = "?restrictor_" + dbNavigationRestrictor.ColName + i;

         sbWhereCondition.Append(fldName + " = " + paramName);

         if(i < dbNavigationRestrictor.RestrictedValues.Length - 1)
         {
            sbWhereCondition.Append(" OR ");
         }
      }

      sbWhereCondition.Append(")");

      return sbWhereCondition.ToString();
   }

   //private static void SetRestrictorParam(MySqlCommand cmd, ZXC.DbNavigationRestrictor dbNavigationRestrictor)
   //{
   //   CreateCommandParameter(cmd, "restrictor_", dbNavigationRestrictor.ColName, dbNavigationRestrictor.RestrictedValues, XSqlDbType.String, 16);
   //}

   private static /*int*/ void SetRestrictorParam(MySqlCommand cmd, ZXC.DbNavigationRestrictor dbNavigationRestrictor/*, int initI*/)
   {
      int i;
      for(i = /*initI*/0; i < dbNavigationRestrictor.RestrictedValues.Length; ++i)
      {
         CreateCommandParameter(cmd, "restrictor_", dbNavigationRestrictor.ColName + i, dbNavigationRestrictor.RestrictedValues[i], XSqlDbType.String, 16);
      }

      //return i;
   }

   private static string SorterSegmentFldNames(VvSQL.IndexSegment[] iSegs, string strAscDesc, bool isArhiva)
   {
      bool firstPass = true;
      string strFldNames = "";

      foreach(VvSQL.IndexSegment iSeg in iSegs)
      {
         if(isArhiva == false && iSeg.IsArhiva == true) continue;

         if(!firstPass)
         {
            strFldNames += ", ";
         }
         else
         {
            firstPass = false;
         }

         // 26.03.2018: sorter sorterTtRecID additions ... kasnije abortirao kao NEUSPJEH 
         strFldNames += iSeg.FldName + " " + strAscDesc;
         //strFldNames += (iSeg.FldName == "recID" ? "L." : "") + iSeg.FldName + " " + strAscDesc;
      }

      return strFldNames;
   }

   private static string PRVNXT_WhereConditon(VvSQL.IndexSegment[] iSegs, string comparer, bool isArhiva)
   {
      bool firstPass = true;
      string strWhereCondition = "(", fldName, paramName;

      for(int i = 0; i < iSegs.Length; ++i)
      {
         if(isArhiva == false && iSegs[i].IsArhiva == true) continue;

         if(!firstPass)
         {
            strWhereCondition += " OR ";
         }
         else
         {
            firstPass = false;
         }

         fldName = iSegs[i].FldName;
         paramName = "?curr" + iSegs[i].FldName;

         strWhereCondition += "(";

         strWhereCondition += fldName + comparer + paramName;

         for(int j = i - 1; j >= 0; --j)
         {
            strWhereCondition += " && ";

            fldName = iSegs[j].FldName;
            paramName = "?curr" + iSegs[j].FldName;

            strWhereCondition += fldName + " = " + paramName;
         }
         strWhereCondition += ")";
      }

      strWhereCondition += ")";

      return strWhereCondition;
   }

   private static void SetNavigationalParam(object[] paramValues, XSqlCommand cmd, string strSql, RecordSorter sorter, bool isArhiva, bool biloGdjeUnazivu)
   {
      int i = 0;

      cmd.CommandText = strSql;
      cmd.CommandType = CommandType.Text;

      int idxOfSegFr_biloGdjeUnazivu;

      if(sorter.RecName == Faktur.recordName) idxOfSegFr_biloGdjeUnazivu = 1;
      else idxOfSegFr_biloGdjeUnazivu = 0;

      if(biloGdjeUnazivu) paramValues[idxOfSegFr_biloGdjeUnazivu] = "%" + paramValues[idxOfSegFr_biloGdjeUnazivu] + "%";

      foreach(VvSQL.IndexSegment iSeg in sorter.IdxSegments)
      {
         if(isArhiva == false && iSeg.IsArhiva == true) { ++i; continue; }

         CreateCommandParameter(cmd, "curr", iSeg.FldName, paramValues[i], iSeg.DbType, iSeg.ParamSize);
         ++i;
      }
   }

   #endregion PRVREC_NXTREC_Command

   #region PRV_NXT_ARHIVED_VERSIONS

   public static XSqlCommand PRVNXT_ArhivedVersion_Command(XSqlConnection conn, VvDataRecord vvDataRecord, DBNavigActionType action)
   {
      string strSql;
      XSqlCommand cmd = VvSQL.InitCommand(conn);

      string tableName;

      tableName = vvDataRecord.VirtualRecordNameArhiva;

      strSql = "SELECT * FROM " + tableName;

      switch(action)
      {
         case DBNavigActionType.PRV:
            strSql += " WHERE origRecID = " + vvDataRecord.TheArhivaData._origRecID + " AND recVer < " + vvDataRecord.TheArhivaData._recVer +
                      " ORDER BY recVer DESC LIMIT 1";
            break;

         case DBNavigActionType.NXT:
            strSql += " WHERE origRecID = " + vvDataRecord.TheArhivaData._origRecID + " AND recVer > " + vvDataRecord.TheArhivaData._recVer +
                      " ORDER BY recVer ASC LIMIT 1";
            break;

         case DBNavigActionType.FRS:
            strSql += " WHERE origRecID = " + vvDataRecord./*TheArhivaData._origRecID*/VirtualRecID +  // jer kada oces FRS/LST onda dolazis iz aktivnog record-a 
                      " ORDER BY recVer ASC LIMIT 1";
            break;

         case DBNavigActionType.LST:
            strSql += " WHERE origRecID = " + vvDataRecord./*TheArhivaData._origRecID*/VirtualRecID +  // jer kada oces FRS/LST onda dolazis iz aktivnog record-a 
                      " ORDER BY recVer DESC LIMIT 1";
            break;

         default: System.Windows.Forms.MessageBox.Show("PRVNXT_ArhivedVersion_Command: prv_or_nxt not set!");
            strSql = null;
            break;
      }

      cmd.CommandText = strSql;

      return cmd;
   }

   #endregion PRV_NXT_ARHIVED_VERSIONS

   #region GTEREC_Command

   public static XSqlCommand GTEREC_Command(XSqlConnection conn,
                                            string selectWhat,
                                            object[] paramValues,
                                            List<VvSqlFilterMember> filterMembers,
                                            OrderDirectEnum direction,
                                            RecordSorter sorter,
                                            int offset,
                                            int pageSize,
                                            bool countOnly,
                                            bool isArhiva,
                                            VvRecLstUC theRecListUC)
   {
      string strSql, orderByAndLimit = "", strVeceManje = "";
      string tableName;
      bool biloGdjeUnazivu = sorter.BiloGdjeU_Tekstu;

      XSqlCommand cmd = VvSQL.InitCommand(conn);

      if(countOnly) selectWhat = " COUNT(*) ";

      if(pageSize < 1) pageSize = Int32.MaxValue;

      orderByAndLimit += " ORDER BY ";

      switch(direction)
      {
         case OrderDirectEnum.ASC: orderByAndLimit += VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "ASC", isArhiva) + " LIMIT ";
            strVeceManje = " >";
            break;

         case OrderDirectEnum.DESC: orderByAndLimit += VvSQL.SorterSegmentFldNames(sorter.IdxSegments, "DESC", isArhiva) + " LIMIT ";
            strVeceManje = " <";
            break;

         default: System.Windows.Forms.MessageBox.Show("PRVNXT_Command: asc_desc not set!");
            orderByAndLimit = strVeceManje = null;
            break;
      }

      orderByAndLimit += offset + "," + pageSize;

      if(countOnly) orderByAndLimit = "";

      if(isArhiva) tableName = sorter.RecNameArhiva;
      else tableName = sorter.RecName;

      if(sorter.SortType == SorterType.Person) RemoveEventualImeRequestFromPrezimeParamValue(ref paramValues[0]);

      strSql = "SELECT " + selectWhat + " FROM " + tableName + " " + sorter.RecName + "\n" +

      #region Artikl's artstat additions (3.1.2011)

               (tableName == Artikl.recordName && theRecListUC != null ? VvSQL.EventualRelatedArtstat_ForWhereClause_FromFilterMembers(cmd, theRecListUC as ArtiklListUC) : "") +

      #endregion Artikl's artstat additions

      #region Fakturs's FaktExt additions (8.1.2011)

               // 17.06.2012: tu je bio bug kada si dosao iz 'faktur_ar' - a
               //(tableName == Faktur.recordName ? "LEFT JOIN " + FaktEx.recordName + " ext ON " + sorter.RecName + ".RecID = ext.fakturRecID \n" : "") +
               (tableName.StartsWith(Faktur.recordName) ? "LEFT JOIN " + (tableName == Faktur.recordName ? FaktEx.recordName : FaktEx.recordNameArhiva) + " ext ON " + sorter.RecName + ".RecID = ext.fakturRecID \n" : "") +

      #endregion Fakturs's FaktExt additions (8.1.2011)

      #region Rtrans's Kupdob additions (17.3.2016)

               (tableName.StartsWith(Rtrans.recordName) ? "LEFT JOIN " + Kupdob.recordName + " ext ON " + sorter.RecName + ".t_kupdob_CD = ext.kupdobCD \n" : "") +

               (tableName.StartsWith(Rtrano.recordName) ? "LEFT JOIN " + Kupdob.recordName + " ext ON " + sorter.RecName + ".t_kupdob_CD = ext.kupdobCD \n" : "") +

      #endregion Rtrans's Kupdob additions (17.3.2016)

              " WHERE " + VvSQL.GTEREC_WhereConditon(sorter/*.IdxSegments*/, filterMembers, strVeceManje, isArhiva, biloGdjeUnazivu) + orderByAndLimit;


      SetNavigationalParam(paramValues, cmd, strSql, sorter, isArhiva, biloGdjeUnazivu); // ovo je za IndexSegments 

      if(filterMembers != null && filterMembers.Count > 0)
         SetReportCommandParamValues(cmd, filterMembers);     // a ovo za FilterMemberz 

      return cmd;
   }

   private static void RemoveEventualImeRequestFromPrezimeParamValue(ref object paramValue)
   {
      string komplet, prezimePart, imePart = "";

      komplet = prezimePart = paramValue.ToString();

      ZXC.SplitImePrezime(komplet, ref imePart, ref prezimePart);

      paramValue = prezimePart;
   }

   public static int CountGTEREC_Rows(XSqlConnection conn, RecordSorter sorter, object[] paramValues, List<VvSqlFilterMember> filterMembers, OrderDirectEnum asc_or_desc, VvRecLstUC theRecListUC)
   {
      if(false/*ZXC.IsSvDUH_ZAHonly*/) return 15000;

      int count;

      using(XSqlCommand cmd = VvSQL.GTEREC_Command(conn, "", paramValues, filterMembers, asc_or_desc, sorter, 0, 0, true, ZXC.TheVvForm.TheVvTabPage.IsArhivaTabPage, theRecListUC))
      {
         count = int.Parse(cmd.ExecuteScalar().ToString());
      }

      return (count);
   }

   private static string GTEREC_WhereConditon(RecordSorter sorter, List<VvSqlFilterMember> filterMembers, string comparer, bool isArhiva, bool biloGdjeUnazivu)
   {
      VvSQL.IndexSegment[] iSegs = sorter.IdxSegments;

      bool firstPass = true;
      bool lastPass = false;
      string strWhereCondition = "(", fldName, paramName;
      int numOfSigificantSegments = isArhiva ? iSegs.Length : iSegs.Length - 1;
      int idxOfLastSignificantSegment = isArhiva ? iSegs.Length - 1 : iSegs.Length - 2;

      if(biloGdjeUnazivu == true)
      {
         int idxOfSegFr_biloGdjeUnazivu;

         if(sorter.RecName == Faktur.recordName) idxOfSegFr_biloGdjeUnazivu = 1;
         else idxOfSegFr_biloGdjeUnazivu = 0;

         fldName = iSegs[idxOfSegFr_biloGdjeUnazivu].FldName;

         paramName = "?curr" + fldName;

         strWhereCondition += fldName + " LIKE " + paramName + ")";
      }
      else
      {
         for(int i = 0; i < iSegs.Length; ++i)
         {
            if(isArhiva == false && iSegs[i].IsArhiva == true) continue;

            //if(i == iSegs.Length - 1) lastPass = true;
            if(i == idxOfLastSignificantSegment) lastPass = true;

            if(!firstPass)
            {
               strWhereCondition += " OR ";
            }
            else
            {
               firstPass = false;
            }

            fldName = iSegs[i].FldName;
            paramName = "?curr" + iSegs[i].FldName;

            strWhereCondition += "(";

            strWhereCondition += fldName + comparer + (lastPass ? "= " : " ") + paramName;

            for(int j = i - 1; j >= 0; --j)
            {
               strWhereCondition += " AND ";

               fldName = iSegs[j].FldName;
               paramName = "?curr" + iSegs[j].FldName;

               strWhereCondition += fldName + " = " + paramName;
            }
            strWhereCondition += ")";
         }
         strWhereCondition += ") ";
      } // normal, NOT biloGdjeUnazivu case 

      if(filterMembers != null && filterMembers.Count > 0)
      {
         strWhereCondition += ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true);
      }

      return strWhereCondition;
   }

   // OLD GTEREC_WhereConditon until 5.8.2011: (BiloGdjeUNazivu logika promijenjena za Faktur case)
   //private static string GTEREC_WhereConditon(VvSQL.IndexSegment[] iSegs, List<VvSqlFilterMember> filterMembers, string comparer, bool isArhiva, bool biloGdjeUnazivu)
   //{
   //   bool firstPass = true;
   //   bool lastPass  = false;
   //   string strWhereCondition = "(", fldName, paramName;
   //   int    numOfSigificantSegments     = isArhiva ? iSegs.Length     : iSegs.Length - 1;
   //   int    idxOfLastSignificantSegment = isArhiva ? iSegs.Length - 1 : iSegs.Length - 2;

   //   if(biloGdjeUnazivu == true)
   //   {
   //      fldName = iSegs[0].FldName;
   //      paramName = "?curr" + iSegs[0].FldName;

   //      strWhereCondition += fldName + " LIKE " + paramName + ")";
   //   }
   //   else
   //   {
   //      for(int i = 0; i < iSegs.Length; ++i)
   //      {
   //         if(isArhiva == false && iSegs[i].IsArhiva == true) continue;

   //         //if(i == iSegs.Length - 1) lastPass = true;
   //         if(i == idxOfLastSignificantSegment) lastPass = true;

   //         if(!firstPass)
   //         {
   //            strWhereCondition += " OR ";
   //         }
   //         else
   //         {
   //            firstPass = false;
   //         }

   //         fldName = iSegs[i].FldName;
   //         paramName = "?curr" + iSegs[i].FldName;

   //         strWhereCondition += "(";

   //         strWhereCondition += fldName + comparer + (lastPass ? "= " : " ") + paramName;

   //         for(int j = i - 1; j >= 0; --j)
   //         {
   //            strWhereCondition += " AND ";

   //            fldName = iSegs[j].FldName;
   //            paramName = "?curr" + iSegs[j].FldName;

   //            strWhereCondition += fldName + " = " + paramName;
   //         }
   //         strWhereCondition += ")";
   //      }
   //      strWhereCondition += ") ";
   //   } // normal, NOT biloGdjeUnazivu case 

   //   if(filterMembers != null && filterMembers.Count > 0)
   //   {
   //      strWhereCondition += ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true);
   //   }

   //   return strWhereCondition;
   //}

   #endregion GTEREC_Command()

   #region ADDREC_RWTREC_DELREC_Command() 

   public static XSqlCommand ADDREC_Command(XSqlConnection conn, VvDataRecord vvDataRecord, bool isArhiva, bool isSkyTraffic)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName;

      if(isArhiva) tableName = vvDataRecord.VirtualRecordNameArhiva;
      else tableName = vvDataRecord.VirtualRecordName;

      cmd.CommandText = "INSERT INTO " + tableName + " SET " +
                        CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, DB_RW_ActionType.ADD, isArhiva, false, isSkyTraffic) + ";\n" +

         "SELECT @@IDENTITY"; // ovo ce pokupiti return od ExecuteScalar();

      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.Without_ID, isArhiva);

      return (cmd);
   }

   public static XSqlCommand RWTREC_Command(XSqlConnection conn, VvDataRecord vvDataRecord, bool isSkyTraffic)
   {
      VvDataRecord oldValues_dataRec;
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "UPDATE " + vvDataRecord.VirtualRecordName + "\n" +
                        " SET " + CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, DB_RW_ActionType.RWT, false, false, isSkyTraffic) + "\n" +
                        " WHERE " + CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, DB_RW_ActionType.RWT_2, false, false, isSkyTraffic);

      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.Without_ID, false);

      // tu sad pozoves RestoreBackupData na new Kplan ili na this vidi malo shqllow, deep, reference ili koji kurac copy, 
      // a da ne sjebes orginalni rekord a da ipak podmetnes backupData u SetCommandParamValues 

      /* */
      oldValues_dataRec = (VvDataRecord)vvDataRecord.Clone();
      oldValues_dataRec.RestoreBackupData();
      vvDataRecord.VvDao.SetCommandParamValues(cmd, oldValues_dataRec, ParamListType.Old_Values, false);
      /* */

      return (cmd);
   }

   public static XSqlCommand DELREC_Command(XSqlConnection conn, VvDataRecord vvDataRecord, bool identifyByRecIDonly, bool isSkyTraffic)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DELETE FROM " + vvDataRecord.VirtualRecordName + " WHERE " +
                         CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, DB_RW_ActionType.DEL, false, identifyByRecIDonly, isSkyTraffic);

      vvDataRecord.VvDao.SetCommandParamValues(cmd, vvDataRecord, ParamListType.Complete, false);

      return (cmd);
   }

   internal static XSqlCommand Rwtrec_BLOBsingleColumn_Command(XSqlConnection conn, VvDataRecord vvDataRecord, string blobColName, byte[] blobValue)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "UPDATE " + vvDataRecord.VirtualRecordName + "\n" +
                        "SET " + blobColName + " = ?prm_blobValue " + "\n" +
                        "WHERE recID = ?prm_recID " + "\n";

      CreateCommandParameter(cmd, "recID", vvDataRecord.VirtualRecID, XSqlDbType.Int32, 10);
      CreateCommandParameter(cmd, "blobValue", blobValue, XSqlDbType.MediumBlob, 1048576 * 32); // 32 MB 

      return (cmd);
   }

   #endregion ADDREC_RWTREC_DELREC_Command() 

   #region CreateStr4_WHERE_or_SET_Clause

   //private static string CreateStr4_WHERE_or_SET_Clause(XSqlConnection conn, VvDataRecord vvDataRecord, DB_RW_ActionType actionType, bool isArhiva)
   //{
   //   return CreateStr4_WHERE_or_SET_Clause(conn, vvDataRecord, actionType, isArhiva, false, false);
   //}

   private static string CreateStr4_WHERE_or_SET_Clause(XSqlConnection conn, VvDataRecord vvDataRecord, DB_RW_ActionType actionType, bool isArhiva, bool identifyByRecIDonly, bool isSkyTraffic)
   {
      string clause, preffix, suffix, firstLine;

      if(actionType == DB_RW_ActionType.RWT_2) // WHERE 
      {
         preffix = "old_";
         suffix = " AND ";
      }
      else if(actionType == DB_RW_ActionType.DEL ||
              actionType == DB_RW_ActionType.EQL) // WHERE 
      {
         preffix = "prm_";
         suffix = " AND ";
      }
      else if(actionType == DB_RW_ActionType.ADD ||
              actionType == DB_RW_ActionType.RWT) // SET 
      {
         preffix = "prm_";
         suffix = ", ";
      }
      else return ("ActionType UNDEFINED!");

      if(actionType == DB_RW_ActionType.ADD)
      {
         if(vvDataRecord.IsTrans == false && vvDataRecord.IsExtender == false /*2.2.2011:&& vvDataRecord.IsCacheForStatus == false*/)
         {
            if(isArhiva)
            {
               // SkyNews 
               //firstLine = "addTS = ?"  + preffix + "addTS "  + suffix + "\n" +
               //            "modTS = ?"  + preffix + "modTS "  + suffix + "\n" +
               //            "addUID = ?" + preffix + "addUID " + suffix + "\n" +
               //            "modUID = ?" + preffix + "modUID " + suffix + "\n" +
               //            "arTS   = CURRENT_TIMESTAMP "      + suffix + "\n"; 
               firstLine = "addTS = ?" + preffix + "addTS " + suffix + "\n" +
                           "modTS = ?" + preffix + "modTS " + suffix + "\n" +
                           "addUID = ?" + preffix + "addUID " + suffix + "\n" +
                           "modUID = ?" + preffix + "modUID " + suffix + "\n" +
                           (ZXC.ThisIsVektorProject
                              ?
                              "lanSrvID = ?" + preffix + "lanSrvID " + suffix + "\n" +
                              "lanRecID = ?" + preffix + "lanRecID " + suffix + "\n"
                              : ""
                           ) +

                           "arTS   = CURRENT_TIMESTAMP " + suffix + "\n";
            }
            else // classic NOT arhiva 
            {
               // SkyNews 
               //firstLine = "addTS  = CURRENT_TIMESTAMP "       + suffix +
               //            "addUID = '" + ZXC.vvDB_User + "' " + suffix + "\n";

               if(isSkyTraffic) // SkyLab vrsi ADDREC recorda. Treba sacuvati sve orginalne meta podatke. 
               {
                  firstLine = "addTS    = ?" + preffix + "addTS " + suffix + "\n" +
                              "modTS    = ?" + preffix + "modTS " + suffix + "\n" +
                              "addUID   = ?" + preffix + "addUID " + suffix + "\n" +
                              "modUID   = ?" + preffix + "modUID " + suffix + "\n" +
                              "lanSrvID = ?" + preffix + "lanSrvID " + suffix + "\n" +
                              "lanRecID = ?" + preffix + "lanRecID " + suffix + "\n";
               }
               else // Classic - classic. NOT arhiva, NOT SkyLab 
               {
                  uint lanRecID;
                  //bool isRecordBornInSkyEnvironment = vvDataRecord.VirtualLanRecID.IsZero() && ZXC.IsSkyEnvironment; // maybe todo? 

                  if(ZXC.IsSkyEnvironment) // Daj se odluci. Da li kod CopyOut-a cuvamo ili ne lanRecID!?!? ODGOVOR: do daljnjega ZABRANJUJEMO copy out u SkyEnvironmentu!!! 
                  {
                     // 29.12.2017: 
                     //if(ZXC.CopyOut_InProgress) lanRecID = vvDataRecord.VirtualLanRecID;
                     //else                       lanRecID = VvSQL.Get_Sql_NextAutoIncrementRecID(conn, vvDataRecord.VirtualRecordName);
                     if(ZXC.CopyOut_InProgress)
                     {
                        if(ZXC.IsTEXTHOany && vvDataRecord is Faktur && (vvDataRecord as Faktur).TT == Faktur.TT_ZPC)
                        {
                           lanRecID = VvSQL.Get_Sql_NextAutoIncrementRecID(conn, vvDataRecord.VirtualRecordName);
                        }
                        else
                        {
                           lanRecID = vvDataRecord.VirtualLanRecID;
                        }
                     }
                     else
                     {
                        lanRecID = VvSQL.Get_Sql_NextAutoIncrementRecID(conn, vvDataRecord.VirtualRecordName);
                     }
                  }
                  else
                  {
                     //lanRecID = vvDataRecord.VirtualLanRecID;
                     lanRecID = 0;
                  }

                  firstLine = "addTS = CURRENT_TIMESTAMP " + suffix +
                              "addUID = '" + ZXC.vvDB_User + "' " + suffix +
                              (vvDataRecord.IsCacheForStatus || ZXC.ThisIsVektorProject == false ? "" :
                              "lanRecID = '" + lanRecID + "' " + suffix +
                              "lanSrvID = '" + ZXC.vvDB_ServerID + "' " + suffix) + "\n";

               } // Classic - classic. NOT arhiva, NOT SkyLab 

            } // classic NOT arhiva 
         }
         else
         {
            firstLine = "";
         }
      } // if(actionType == DB_RW_ActionType.ADD)  

      else if(actionType == DB_RW_ActionType.RWT)
      {
         if(vvDataRecord.EditedHasChanges() && !vvDataRecord.IsTrans && !vvDataRecord.IsExtender && !vvDataRecord.IsCacheForStatus) // ############################################ 
         {
            if(isSkyTraffic) // SkyLab vrsi RWTREC recorda. Treba sacuvati sve orginalne meta podatke. 
            {
               firstLine = "addTS    = ?" + preffix + "addTS " + suffix + "\n" +
                           "modTS    = ?" + preffix + "modTS " + suffix + "\n" +
                           "addUID   = ?" + preffix + "addUID " + suffix + "\n" +
                           "modUID   = ?" + preffix + "modUID " + suffix + "\n" +
                           "lanSrvID = ?" + preffix + "lanSrvID " + suffix + "\n" +
                           "lanRecID = ?" + preffix + "lanRecID " + suffix + "\n";
            }
            else // classic, NOT skyTraffic 
            {
               firstLine = "modUID = '" + ZXC.vvDB_User + "' " + suffix + "\n";
            }
         }
         else if(vvDataRecord.IsExtendable && vvDataRecord.VirtualExtenderRecord.EditedHasChanges()) // ############################################################################# 
         {
            if(isSkyTraffic) // SkyLab vrsi RWTREC recorda. Treba sacuvati sve orginalne meta podatke. 
            {
               firstLine = "addTS    = ?" + preffix + "addTS " + suffix + "\n" +
                           "modTS    = ?" + preffix + "modTS " + suffix + "\n" +
                           "addUID   = ?" + preffix + "addUID " + suffix + "\n" +
                           "modUID   = ?" + preffix + "modUID " + suffix + "\n" +
                           "lanSrvID = ?" + preffix + "lanSrvID " + suffix + "\n" +
                           "lanRecID = ?" + preffix + "lanRecID " + suffix + "\n";
            }
            else // classic, NOT skyTraffic 
            {
               firstLine = "modUID = '" + ZXC.vvDB_User + "' " + suffix + "\n";
            }
         }
         else if((vvDataRecord.IsDocument && ((VvDocumentRecord)vvDataRecord).EditedTransesHaveChanges()) ||
                 (vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).EditedTransesHaveChanges2()) ||
                 (vvDataRecord.IsPolyDocument && ((VvPolyDocumRecord)vvDataRecord).EditedTransesHaveChanges3())) // ################################################################## 
         {
            if(isSkyTraffic) // SkyLab vrsi RWTREC recorda. Treba sacuvati sve orginalne meta podatke. 
            {
               firstLine = "addTS    = ?" + preffix + "addTS " + suffix + "\n" +
                           "modTS    = ?" + preffix + "modTS " + suffix + "\n" +
                           "addUID   = ?" + preffix + "addUID " + suffix + "\n" +
                           "modUID   = ?" + preffix + "modUID " + suffix + "\n" +
                           "lanSrvID = ?" + preffix + "lanSrvID " + suffix + "\n" +
                           "lanRecID = ?" + preffix + "lanRecID " + suffix + "\n";
            }
            else // classic, NOT skyTraffic 
            {
               // default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP
               //The column is automatically updated to the current date
               //and time when you change any other column in the row from its
               //current value. The update happens ONLY if you actually
               //change a column value; setting a column to
               //its current value doesn’t update the TIMESTAMP.
               // ...pa znaci za slucaj kada je documant ostao isti a transovi ne; moramo rucno srediti 'modTS' 
               firstLine = "modTS = CURRENT_TIMESTAMP " + suffix +
                           "modUID = '" + ZXC.vvDB_User + "' " + suffix + "\n";
            }
         }
         else
         {
            firstLine = "";
         }
      } // else if(actionType == DB_RW_ActionType.RWT) 

      else
      {
         if(identifyByRecIDonly == true)
         {
            // 17.12.2014: 
            if(actionType == DB_RW_ActionType.DEL && isSkyTraffic == true && vvDataRecord.IsTrans == false && vvDataRecord.IsExtender == false)
            {
               return ("lanSrvID = ?" + preffix + "lanSrvID AND lanRecID =?" + preffix + "lanRecID ");
            }
            else // orig, default case 
            {
               /* if(isSkyTraffic) return ("lanSrvID = ?" + preffix + "lanSrvID AND lanRecID =?" + preffix + "lanRecID ");
               else */
               return ("recID = ?" + preffix + "recID "); // Orig, default case 
            }
         }

         firstLine = "recID = ?" + preffix + "recID " + suffix + "\n";
      }

      clause = firstLine + vvDataRecord.VvDao.WHERE_or_SET_Clause_Specifics(preffix, suffix, isArhiva);

      return (clause);
   }

   public static string ColumnAndParam(string columnName, string preffix, string suffix)
   {
      return (columnName + " = ?" + preffix + columnName + suffix);
   }

   public static void CreateCommandParameter(XSqlCommand cmd, string paramName, object paramValue, XSqlDbType dbType, int size)
   {
      VvSQL.CreateCommandParameter(cmd, "prm_", paramName, paramValue, dbType, size);
   }

   /// <summary>
   /// Ovoga kasnije izbaci iz upotrebe jer ovdje se dosta budalasa sa nepotrebnim parametrima (15.03.2009)
   /// </summary>
   /// <param name="cmd"></param>
   /// <param name="preffix"></param>
   /// <param name="paramName"></param>
   /// <param name="paramValue"></param>
   /// <param name="dbType"></param>
   /// <param name="size"></param>
   public static void CreateCommandParameter(XSqlCommand cmd, string preffix, string paramName, object paramValue, XSqlDbType dbType, int size)
   {
      XSqlParameter theParameter = new XSqlParameter("?" + preffix + paramName, dbType, size, ParameterDirection.Input, false, 0, 0, paramName, DataRowVersion.Original, null);
      cmd.Parameters.Add(theParameter);
      cmd.Parameters["?" + preffix + paramName].Value = paramValue;
   }

   public static void CreateCommandParameter_withoutValue(XSqlCommand cmd, string preffix, string paramName, XSqlDbType dbType, int size)
   {
      XSqlParameter theParameter = new XSqlParameter("?" + preffix + paramName, dbType, size);
      cmd.Parameters.Add(theParameter);
   }

   /// <summary>
   /// kada je jedan parametar = jedan dataColumn, pa je paramName = ?+where_or_and+columnName
   /// </summary>
   /// <param name="cmd"></param>
   /// <param name="where_or_and"></param>
   /// <param name="paramValue"></param>
   /// <param name="drSchema"></param>
   public static void CreateCommandParameter(XSqlCommand cmd, string preffix, object paramValue, DataRow drSchema)
   {
      string paramName = (string)drSchema[0];

      CreateCommandNamedParameter(cmd, preffix, paramName, paramValue, drSchema);
   }

   /// <summary>
   /// kada jedan parametar moze ici prema vise od jednog dataColumn-a, pa paramName dolazi kao dolazni parametar metode
   /// </summary>
   /// <param name="cmd"></param>
   /// <param name="where_or_and"></param>
   /// <param name="paramName"></param>
   /// <param name="paramValue"></param>
   /// <param name="drSchema"></param>
   public static void CreateCommandNamedParameter(XSqlCommand cmd, string preffix, string paramName, object paramValue, DataRow drSchema)
   {
      //string      paramName  = (string)     drSchema["ColumnName"];
      //MySqlDbType dbType     = (MySqlDbType)drSchema["ProviderType"];
      //int         size       = (int)        drSchema["ColumnSize"];
      //bool        isNullable = (bool)       drSchema["AllowDBNull"];
      //byte        precision  = (byte)(int)  drSchema["NumericPrecision"];
      //byte        scale      = (byte)(int)  drSchema["NumericScale"];

      // ovo je malo tanak led. Ako bu bug, pogledaj u debuggeru TheTableShema selectColumns layout
      // ili F1: SqlDataReader.GetSchemaTable Method 
      // ovo je kak'ti brze 

      //string      paramName  = (string)     drSchema[0];
      XSqlDbType dbType = (XSqlDbType)drSchema[13];
      int size = (int)drSchema[2];
      bool isNullable = (bool)drSchema[12];
      byte precision = (byte)(int)drSchema[3];
      byte scale = (byte)(int)drSchema[4];

      //if(dbType == XSqlDbType.VarChar)
      //{
      //   Encoding encodingUTF8 = Encoding.UTF8;
      //   Encoding encoding1250 = Encoding.GetEncoding(1250);
      //   byte[] bytes;
      //   bytes = encodingUTF8.GetBytes((string)paramValue);
      //   paramValue = encodingUTF8.GetString(bytes);
      //   //bytes = encoding1250.GetBytes((string)paramValue);
      //   //paramValue = encoding1250.GetString(bytes);
      //}

      // 26.06.2023: 'devtec mnozenje' peripetije od pocetka 2023
      if(paramValue != null && paramValue.GetType() == typeof(decimal))
      {
         decimal origValue = (decimal)paramValue;
         decimal roundValue = ZXC.Ron(origValue, 8);

         if(origValue != roundValue)
         {
            paramValue = roundValue; // NEW rounded paramValue! 
         }
      }

      XSqlParameter theParameter =
         new XSqlParameter("?" + preffix + paramName, dbType, size, ParameterDirection.Input, isNullable, precision, scale, paramName, DataRowVersion.Original, paramValue);

      cmd.Parameters.Add(theParameter);
   }

   #endregion CreateStr4_WHERE_or_SET_Clause

   #region ReportError

   public static bool ReportSqlError(string actionName, XSqlException ex, MessageBoxButtons msgBoxButtons)
   {
      string theMessage = actionName + " Greška! Br: [" + ex.Number + "]\n\n" + "Method: " + ZXC.GetMethodNameDaStack() + "\n\n Poruka: \n\n[" +
         (ex.Number != 1045 ? ex.Message + "\n" + ex.InnerException : "Krivi USER NAME i/ili PASSWORD!") + "]";

      // 01.05.2015: 
      if(ZXC.ThisIsSkyLabProject)
      {
         ZXC.aim_log(theMessage);
         return true;
      }

      // 02.11.2015: da ne javlja KDUP error za CACHE na poslovnici (vatrogasna mjera)                                   
      // 03.11.2015: da ne javlja KDUP error za CACHE igdje         (vatrogasna mjera)                                   
      //if(  ZXC.IsTEXTHOshop &&   ex.Number == 1062 /* K_DUP err */ && actionName.ToLower().Contains(ArtStat.recordName)) 
      if(/*ZXC.IsTEXTHOshop &&*/ ex.Number == 1062 /* K_DUP err */ && actionName.ToLower().Contains(ArtStat.recordName))
      {
         ZXC.aim_log(theMessage);
         return true;
      }

      DialogResult result = MessageBox.Show(theMessage, "Database error!", msgBoxButtons, MessageBoxIcon.Error);

      if(result == DialogResult.OK) return true;
      if(result == DialogResult.Retry) return true;
      if(result == DialogResult.Yes) return true;

      if(result == DialogResult.Abort) return false;
      if(result == DialogResult.Cancel) return false;
      if(result == DialogResult.No) return false;

      return true;
   }

   public static bool ReportGeneric_DB_Error(string actionName, string errMessage, MessageBoxButtons msgBoxButtons)
   {
      string theMessage = actionName + " Greška! Poruka: [" + errMessage + "]\n\n" + "Method: " + ZXC.GetMethodNameDaStack();

      // 01.05.2015: 
      if(ZXC.ThisIsSkyLabProject)
      {
         ZXC.aim_log(theMessage);
         return true;
      }

      DialogResult result =
         MessageBox.Show(theMessage, "Database error!", msgBoxButtons, MessageBoxIcon.Error);

      if(result == DialogResult.OK) return true;
      if(result == DialogResult.Retry) return true;
      if(result == DialogResult.Yes) return true;

      if(result == DialogResult.Abort) return false;
      if(result == DialogResult.Cancel) return false;
      if(result == DialogResult.No) return false;

      return true;
   }

   public static bool ReportGenericError(string actionName, string errMessage, MessageBoxButtons msgBoxButtons)
   {
      string theMessage = "Akcija: [" + actionName + "]\n\n" + "Method: " + ZXC.GetMethodNameDaStack() + "\n\nPoruka:\n\n" + errMessage/* + "]"*/;

      // 01.05.2015: 
      if(ZXC.ThisIsSkyLabProject)
      {
         ZXC.aim_log(theMessage);
         return true;
      }

      DialogResult result =
         MessageBox.Show(theMessage, "Greška!", msgBoxButtons, MessageBoxIcon.Error);

      if(result == DialogResult.OK) return true;
      if(result == DialogResult.Retry) return true;
      if(result == DialogResult.Yes) return true;

      if(result == DialogResult.Abort) return false;
      if(result == DialogResult.Cancel) return false;
      if(result == DialogResult.No) return false;

      return true;
   }

   #endregion ReportError

   #region CREATE_AND_OPEN_XSqlConnection, CREATE_TEMP_XSqlConnection

   public static XSqlConnection CREATE_AND_OPEN_XSqlConnection(string _host, string _user, string _password, string _dbName)
   {
      return CREATE_AND_OPEN_XSqlConnection(_host, _user, _password, _dbName, true);
   }
   public static XSqlConnection CREATE_AND_OPEN_XSqlConnection(string _host, string _user, string _password, string _dbName, bool shouldReport1045)
   {
      //bool isOK = true;
      XSqlConnection connection = null;

      try
      {
         string connectionString = VvSQL.GetConnectionString(_host, _user, _password, _dbName);
         connection = new XSqlConnection(connectionString);
         connection.Open();
      }
      catch(System.Net.Sockets.SocketException ex)
      {
         if(_dbName != ZXC.vv_MBF_DbName)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Socket Exception.\n\nDataBase server nedostupan!\n\n" + ex.ToString());
         }
         //isOK = false;
      }
      catch(XSqlException ex)
      {
         if(shouldReport1045)
         {
#if(DEBUG)
            VvSQL.ReportSqlError("CREATE_AND_OPEN_XSqlConnection", ex, System.Windows.Forms.MessageBoxButtons.OK);
#else
            if(_dbName != ZXC.vv_MBF_DbName)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Greška pri konektiranju na DataBase server.\n\n" + ex.Message + (ex.Number == 1045 ? "\n\nNepoznat username i/ili password?" : ""));
            }
            else
            {
               ZXC.aim_log("Greška pri konektiranju na DataBase server.\n\n" + ex.Message + (ex.Number == 1045 ? "\n\nNepoznat username i/ili password?" : ""));
            }
#endif
         }
         //isOK = false;
      }

      catch(Exception ex)
      {
         if(_dbName != ZXC.vv_MBF_DbName)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "General Exception. " + ex.Message);
         }
         else
         {
            ZXC.aim_log("General Exception. " + ex.Message);
         }
         //isOK = false;
      }
      //return (isOK);

      return connection;
   }

   private static string GetConnectionString(string _server, string _user, string _password, string _dbName)
   {
      string theUser = ZXC.GetUserNameWithWwwPreffix(_user);

      if(_dbName.IsEmpty())
      {
         //return String.Format("Server = {0}; UID = {1}; Password = {2}; Pooling = false; Connect Timeout = 4;",
         //   _server, _user, _password, ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName);
         //return String.Format("Server = {0}; UID = {1}; Port = {3} ; Password = {2};                                 Connect Timeout = 10; Allow User Variables=True;                                                            ", _server, theUser, _password, ZXC.vvDB_Port);
         //return String.Format("Server = {0}; UID = {1}; Port = {3} ; Password = {2};                 Keepalive=10;   Connect Timeout = 10; Allow User Variables=True;                                                            ", _server, theUser, _password, ZXC.vvDB_Port);
         //return String.Format("Server = {0}; UID = {1}; Port = {3} ; Password = {2};                 Keepalive=4 ;   Connect Timeout = 16; Allow User Variables=True; default command timeout=20;                             {4}", _server, theUser, _password, ZXC.vvDB_Port, ZXC.ThisIsHektorProject ? "CharSet=latin1; " : "");
         return String.Format("Server = {0}; UID = {1}; Port = {3} ; Password = {2};                 Keepalive=4 ;   Connect Timeout = 16; Allow User Variables=True; default command timeout=20; convert zero datetime=True; {4}", _server, theUser, _password, ZXC.vvDB_Port, ZXC.ThisIsHektorProject ? "CharSet=latin1; " : "");

         // TIMEOUT problem NE RJESAVAS OVDJE nego u InitCommand()! 
      }
      else
      {
         // 29.12.2016: 
         _dbName = _dbName.AsUTF8();

         //return String.Format("Server = {0}; UID = {1}; Password = {2}; DataBase = {3}; Pooling = false; Connect Timeout = 4;",
         //   _server, _user, _password, ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName);
         //return String.Format("Server = {0}; UID = {1}; Port = {4} ; Password = {2}; DataBase = {3};               Connect Timeout = 10; Allow User Variables=True;                                                              ", _server, theUser, _password, _dbName, ZXC.vvDB_Port);
         //return String.Format("Server = {0}; UID = {1}; Port = {4} ; Password = {2}; DataBase = {3}; Keepalive=10; Connect Timeout = 10; Allow User Variables=True;                                                              ", _server, theUser, _password, _dbName, ZXC.vvDB_Port);
         //return String.Format("Server = {0}; UID = {1}; Port = {4} ; Password = {2}; DataBase = {3}; Keepalive= 4; Connect Timeout = 16; Allow User Variables=True; default command timeout=20;                                  ", _server, theUser, _password, _dbName, ZXC.vvDB_Port);
         return String.Format("Server = {0}; UID = {1}; Port = {4} ; Password = {2}; DataBase = {3}; Keepalive= 4; Connect Timeout = 16; Allow User Variables=True; default command timeout=20; convert zero datetime=True       ", _server, theUser, _password, _dbName, ZXC.vvDB_Port);

         // TIMEOUT problem NE RJESAVAS OVDJE nego u InitCommand()! 
      }
   }

   public static XSqlConnection CREATE_TEMP_XSqlConnection(string dbName)
   {
      XSqlConnection tempConnection = new XSqlConnection(VvSQL.GetConnectionString(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, dbName));

      try { tempConnection.Open(); }
      catch(XSqlException ex) { VvSQL.ReportSqlError("CREATE_TEMP_XSqlConnection.Open", ex, System.Windows.Forms.MessageBoxButtons.OK); }

      tempConnection.ChangeDatabase(dbName);

      return tempConnection;
   }

   public static bool TestAnd_CREATE_ThePrjktDB_Connection_4ZXC()
   {
      bool OK;

      ZXC.TheMainDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, null);

      OK = (ZXC.TheMainDbConnection != null && ZXC.TheMainDbConnection.State == ConnectionState.Open);

      return OK;
   }

   #endregion CREATE_AND_OPEN_XSqlConnection, CREATE_TEMP_XSqlConnection

   #region GetVVDatabaseNamesList

   public static List<string> GetVVDatabaseNamesList(XSqlConnection tmpConn, string dbNamePreffix)
   {
      bool success = true;
      string currDbName;
      List<string> dbNames = new List<string>();

      using(XSqlCommand cmd = CHECK_DATABASE_EXISTS_Command(tmpConn, dbNamePreffix + '%'))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult/* | CommandBehavior.SingleRow*/))
            {

               success = reader.HasRows;

               while(success && reader.Read())
               {
                  currDbName = reader.GetString(0);

                  if(ZXC.vvDB_is_www) currDbName = currDbName.Replace(ZXC.vvDB_www_preffix, "");

                  if(currDbName[2] == '2') dbNames.Add(currDbName); // npr vv2009_VIPER_000001, na trecem mjestu mora biti prva znamenka godine, malo fuzzy ti je ovo! 
                  if(currDbName[2] == '1') dbNames.Add(currDbName); // npr vv2009_VIPER_000001, na trecem mjestu mora biti prva znamenka godine, malo fuzzy ti je ovo! 
               }

               reader.Close();

            } // using reader 
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetVVDatabaseNamesList: " + dbNamePreffix, ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            success = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      return (dbNames);
   }

   public static List<string> GetVVDatabaseNamesList_viaSuffix(XSqlConnection tmpConn, string dbNameSuffix)
   {
      bool success = true;
      string currDbName;
      List<string> dbNames = new List<string>();

      using(XSqlCommand cmd = CHECK_DATABASE_EXISTS_Command(tmpConn, '%' + dbNameSuffix))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult/* | CommandBehavior.SingleRow*/))
            {

               success = reader.HasRows;

               while(success && reader.Read())
               {
                  currDbName = reader.GetString(0);

                  // PAZI OVO JER DRUGACIJE od GetVVDatabaseNamesList()!
                  //if(ZXC.vvDB_is_www) currDbName = currDbName.Replace(ZXC.vvDB_www_preffix, "");

                  /*if(currDbName[2] == '2')*/ dbNames.Add(currDbName); 
                  //if(currDbName[2] == '1') dbNames.Add(currDbName); // npr vv2009_VIPER_000001, na trecem mjestu mora biti prva znamenka godine, malo fuzzy ti je ovo! 
               }

               reader.Close();

            } // using reader 
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("GetVVDatabaseNamesList: " + dbNameSuffix, ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            success = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      return (dbNames);
   }

   #endregion GetVVDatabaseNamesList

   #region CREATE_DATABASE

   public static bool CheckDatabase(XSqlConnection conn, string dbName, bool shouldAsk)
   {
      bool OK = VvSQL.CHECK_DATABASE_EXISTS(conn, dbName);

      if(!OK)
      {
         OK = WhatToDoWhenDatabaseIsMissing(conn, dbName, shouldAsk);
      }

      return (OK);
   }

   public static bool CHECK_DATABASE_EXISTS(XSqlConnection tmpConn, string dbName)
   {
      bool success = true;
      string returnedName = "";

      using(XSqlCommand cmd = CHECK_DATABASE_EXISTS_Command(tmpConn, dbName))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
               success = reader.HasRows;
               if(reader.Read()) returnedName = reader.GetString(0);
               else success = false;
               reader.Close();

            } // using reader 
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("CHECK_DATABASE_EXISTS DB: " + dbName, ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            success = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      return (success);
   }

   private static XSqlCommand CHECK_DATABASE_EXISTS_Command(XSqlConnection conn, string dbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SHOW DATABASES LIKE '" + dbName + "'";

      return (cmd);
   }

   static bool WhatToDoWhenDatabaseIsMissing(XSqlConnection conn, string dbName, bool shouldAsk)
   {
      bool OK = true;

      if(shouldAsk)
      {
         string message = "DataBase: [" + dbName + "] ne postoji! Da li da kreiram novu?";

         DialogResult result = MessageBox.Show(message, "Kreirati DataBase " + dbName + "?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return false;
      }

      OK = VvSQL.CREATE_DATABASE(conn, dbName);

      return OK;
   }

   public static bool CREATE_DATABASE(XSqlConnection tempConnection, string dbName)
   {

      bool success = true;
      int nora = -1; // number of rows affected 


      using(XSqlCommand cmd = VvSQL.CREATE_DATABASE_Command(tempConnection, dbName))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            VvSQL.ReportSqlError("CREATE_DATABASE " + dbName, ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      if(nora != 1)
      {
         success = false;
         VvSQL.ReportGeneric_DB_Error("CREATE_DB", "DB nije kreirana! (Da li vec postoji?)", System.Windows.Forms.MessageBoxButtons.OK);
      }

      return (success);
   }

   public static XSqlCommand CREATE_DATABASE_Command(XSqlConnection conn, string dbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      //cmd.CommandText = "CREATE DATABASE IF NOT EXISTS " + dbName +
      //                  " DEFAULT CHARACTER SET latin2" +
      //                  " DEFAULT COLLATE       latin2_croatian_ci";
      // ako ostavis IF NOT EXISTS onda ne javi err ako vec potoji pa user ne zna da mu nije uspjelo 

      cmd.CommandText = "CREATE DATABASE " + dbName +
                        " DEFAULT CHARACTER SET latin2" +
                        " DEFAULT COLLATE latin2_croatian_ci";
      return (cmd);
   }

   public static ZXC.VvDataBaseInfo? GetVvDataBaseInfoForKupdobCD(XSqlConnection tmpConn, string year, uint KupdobCD)
   {
      bool success = true;
      string returnedName = "", searchPattern = ZXC.vvDB_www_preffix + ZXC.TheVvForm.GetvvDB_prefix() + year + "_%_" + KupdobCD.ToString("000000");

      using(XSqlCommand cmd = CHECK_DATABASE_EXISTS_Command(tmpConn, searchPattern))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
               success = reader.HasRows;
               if(reader.Read()) returnedName = reader.GetString(0);
               else success = false;
               reader.Close();

            } // using reader 
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("CHECK_DATABASE_EXISTS DB: " + searchPattern, ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            success = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      if(success == false) return null;

      ZXC.VvDataBaseInfo? vvDBinfo = new ZXC.VvDataBaseInfo(returnedName);

      return (vvDBinfo);
   }

   #endregion CREATE_DATABASE

   #region CREATE_TABLE

   //public static bool CheckTable(XSqlConnection conn, string dbName, string tableName/*, bool shouldAsk*/) // shouldAsk is PUSE argument)
   //{
   //   bool OK = true;// CHECK_TABLE_EXISTS(conn, dbName, tableName);

   //   //if(!OK)
   //   //{
   //   //   OK = WhatToDoWhenTableIsMissing(conn, dbName, tableName, shouldAsk);

   //   //   return (OK);
   //   //}

   //   // Ovo gore remarkirano 28.12.2010: vidi komentar u VvDaoBase.GetVvTableMetadata 

   //   if(OK) // tablica postoji odprije, nije novokreirana, provjeri verziju! 
   //   {
   //      OK = CheckTableVersion_AndCatchUpIfNeeded(conn, dbName, tableName/*, shouldAsk*/);
   //   }

   //   return (OK);
   //}

   public/*private*/ static bool CheckTableVersion_AndCatchUpIfNeeded(XSqlConnection conn, string dbName, string tableName/*, bool shouldAsk*/)
   {
      if((tableName.StartsWith("vv") && tableName != ZXC.vvDB_ucListTableName) || // za vvusercontrol treba! 
          tableName.StartsWith("zz")) return true; // vvlog, vvlocker, zzLookUpTables, ...

      ZXC.SetStatusText("CheckTableVersion_AndCatchUpIfNeeded");

      VvDaoBase.VvTableMetaData vvTableMetaData = VvDaoBase.GetVvTableMetadata(conn, dbName, tableName);

      uint lastNewestCurrentTableVersion = ZXC.TheVvForm.GetLastNewestCurrentTableVersionForMetaData(tableName);
      uint actualInFileTableVersion = vvTableMetaData.TableVersion;

      // 11.02.2015:                                                                                             ovo IsNotRipley7 dodano 08.11.2016 da ubrzam ulazak u vvTH 
      if(ZXC.ThisIsSkyLabProject == false && tableName != DevTec2.recordName && tableName != Htrans2.recordName && ZXC.IsRipleyOrKristal == false &&
         // Za 'vvDB_ServerID_CENTRALA' NEMOJ provjeravati OSIM ako si superuser na localhostu)
         //(ZXC.vvDB_ServerID != ZXC.vvDB_ServerID_CENTRALA ||  ZXC.vvDB_IsLocalhost                                                       )) VvSqlMaintenanceCheckTable(conn, dbName, tableName);
         (ZXC.vvDB_ServerID != ZXC.vvDB_ServerID_CENTRALA || (ZXC.vvDB_IsLocalhost && ZXC.CURR_userName == ZXC.vvDB_programSuperUserName))) VvSqlMaintenanceCheckTable(conn, dbName, tableName);

      if(actualInFileTableVersion == lastNewestCurrentTableVersion)
      {
         ZXC.ClearStatusText();
         return (true);
      }
      else // GO! ALTER this TABLE 
      {
         bool OK = ALTER_TABLE_ForCatchUp(conn, dbName, tableName, actualInFileTableVersion, lastNewestCurrentTableVersion, vvTableMetaData);

         #region Roll In Past

         if(OK && dbName != ZXC.VvDB_prjktDB_Name)
         {
            ZXC.VvDataBaseInfo dbi = new ZXC.VvDataBaseInfo(dbName);

            List<string> dbNames_prevYears = GetVVDatabaseNamesList_viaSuffix(conn, dbi.ProjectName + "_" + dbi.ProjectCode); 

            dbNames_prevYears.Remove(dbName); // makni tekucu godinu 


            XSqlConnection currPrevYear_conn = null;

            foreach(string currPrervYear_dbname in dbNames_prevYears)
            {
               if(currPrervYear_dbname == ZXC.VvDB_prjktDB_Name) continue;
               if(dbi.ProjectYearAsUInt < 2010)                  continue; // safety 

               dbi = new ZXC.VvDataBaseInfo(currPrervYear_dbname);

               currPrevYear_conn = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, currPrervYear_dbname);

               // dodano tek 29.01.2024: 
               vvTableMetaData = VvDaoBase.GetVvTableMetadata(/*conn*/currPrevYear_conn, /*dbName*/currPrervYear_dbname, tableName);
               actualInFileTableVersion = vvTableMetaData.TableVersion;

               ALTER_TABLE_ForCatchUp(currPrevYear_conn, currPrervYear_dbname, tableName, actualInFileTableVersion, lastNewestCurrentTableVersion, vvTableMetaData);

              } // foreach(string currPrervYear_dbname in dbNames_prevYears)

            if(currPrevYear_conn != null) currPrevYear_conn.Close();
         }

         #endregion Roll In Past

         ZXC.ClearStatusText();

         return OK;
      }

   }

   private static bool VvSqlMaintenanceCheckTable(XSqlConnection conn, string dbName, string tableName)
   {
      if(ZXC.VvTableMaintenanceList == null) ZXC.VvTableMaintenanceList = new List<ZXC.CdAndName_CommonStruct>();

      if(ZXC.VvTableMaintenanceList.Contains(new ZXC.CdAndName_CommonStruct(dbName, tableName))) return true;

      bool checkOK = true;

      ZXC.SetStatusText("VvSqlMaintenanceCheckTable " + tableName);
    //ZXC.aim_log      ("VvSqlMaintenanceCheckTable " + tableName);

      List<ZXC.VvUtilDataPackage> messageRows = new List<ZXC.VvUtilDataPackage>();
      ZXC.VvUtilDataPackage messageRow;

      using(XSqlCommand cmd = VvSqlMaintenanceCheckTable_Command(conn, dbName, tableName))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult/* | CommandBehavior.SingleRow*/))
            {
               while(reader.HasRows && reader.Read())
               {
                  messageRow = new ZXC.VvUtilDataPackage();
                                                            //Column   Value 
                  messageRow.TheStr1 = reader.GetString(0); //Table    The table name 
                  messageRow.TheStr2 = reader.GetString(1); //Op       Always check 
                  messageRow.TheStr3 = reader.GetString(2); //Msg_type One of status, error, info, or warning 
                  messageRow.TheStr4 = reader.GetString(3); //Msg_text The message 

                  messageRows.Add(messageRow);
               }
               reader.Close();
            } // using reader 
         }
         catch(XSqlException ex)
         {
            checkOK = false;
            VvSQL.ReportSqlError("VvSqlMaintenanceCheckTable error", ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            checkOK = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      if(messageRows.Count() != 1 || messageRows[0].TheStr4 != "OK")
      {
         checkOK = false;
         string message = "";
         messageRows.ForEach(mr => message += mr.TheStr1 + " Msg_type: " + mr.TheStr3 + " Msg_text: [" + mr.TheStr4 + "]\n");
         ZXC.aim_emsg(MessageBoxIcon.Error, "KONTAKTIRAJTE SUPPORT!\n\nVvSqlMaintenanceCheckTable ERROR:\n\n" + message);
      }

      ZXC.ClearStatusText();

      ZXC.VvTableMaintenanceList.Add(new ZXC.CdAndName_CommonStruct(dbName, tableName));

      return (checkOK);
   }

   private static XSqlCommand VvSqlMaintenanceCheckTable_Command(XSqlConnection conn, string dbName, string tblName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "CHECK TABLE  " + dbName + "." + tblName;

      return (cmd);
   }

   private static bool ALTER_TABLE_ForCatchUp(XSqlConnection conn, string dbName, string tableName, uint actualInFileTableVersion, uint lastNewestCurrentTableVersion, VvDaoBase.VvTableMetaData vvTableMetaData)
   {
      bool OK;

      for(uint catchingVersion = actualInFileTableVersion + 1; catchingVersion <= lastNewestCurrentTableVersion; catchingVersion++)
      {
         OK = ALTER_TABLE_ForCatchUp_JOB(conn, dbName, tableName, catchingVersion, vvTableMetaData);

         if(!OK)
         {
            return false;
         }
         else
         {
#if(DEBUG)
            ZXC.aim_emsg(MessageBoxIcon.Information, "ALTER TABLE [{0}] na verziju [{1}] uspjesno obavljeno.", tableName, catchingVersion);
#endif

            // 12.12.2011: VvRECREATE CACHE 
            if(tableName == ArtStat.recordName)
            {
               if(CHECK_TABLE_EXISTS(conn, dbName, tableName) == false)
               {
                  WhatToDoWhenTableIsMissing(conn, dbName, ArtStat.recordName, false);
               }
            }
         }
      }

      return true;
   }

   public static bool CHECK_TABLE_EXISTS(XSqlConnection tmpConn, string dbName, string tblName)
   {
      bool success = true;
      string returnedName = "";

      using(XSqlCommand cmd = CHECK_TABLE_EXISTS_Command(tmpConn, dbName, tblName))
      {
         try
         {
            using(XSqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
            {
               success = reader.HasRows;
               if(reader.Read()) returnedName = reader.GetString(0);
               else              success = false;
               reader.Close();

            } // using reader 
         }
         catch(XSqlException ex)
         {
            success = false;
            VvSQL.ReportSqlError("CHECK_TABLES_EXISTS DB: " + dbName + " TBL: " + tblName, ex, System.Windows.Forms.MessageBoxButtons.OK);
         }
         catch(Exception ex)
         {
            success = false;
            System.Windows.Forms.MessageBox.Show(ex.Message);
         }

      } // using cmd  

      return (success);
   }

   private static XSqlCommand CHECK_TABLE_EXISTS_Command(XSqlConnection conn, string dbName, string tblName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SHOW TABLES FROM " + dbName + " LIKE '" + tblName + "'";

      return (cmd);
   }

   public static XSqlCommand GetVvTableMetadata_Command(XSqlConnection conn, string dbName, string tblName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SHOW TABLE STATUS FROM " + dbName + " LIKE '" + tblName + "'";

      return (cmd);
   }

   public static bool WhatToDoWhenTableIsMissing(XSqlConnection conn, string dbName, string tableName, bool shouldAsk)
   {
      bool OK = true;

      if(shouldAsk)
      {
         string message = "Tablica: [" + dbName + "." + tableName + "] ne postoji! Da li da kreiram novu?";

         DialogResult result = MessageBox.Show(message, "Kreirati tablicu " + tableName + "?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return false;
      }
      
    //5.1.2010:
    //OK = VvSQL.CREATE_TABLE(conn, dbName, tableName, (tableName == ArtStat.recordName ? true : false)); // Ovdje se, dakle, odredjuje da je   ArtStat tablica - MEMORY table 
      OK = VvSQL.CREATE_TABLE(conn, dbName, tableName, (                                         false)); // Ovdje se, dakle, odredjuje da NIJE ArtStat tablica - MEMORY table 

      return OK;
   }

   public static bool CREATE_TABLE(XSqlConnection tempConnection, string dbName, string recordName, bool isMemory)
   {

      bool success = true;
      int nora = -1; // number of rows affected 


      using(XSqlCommand cmd = VvSQL.CREATE_TABLE_Command(tempConnection, dbName, recordName, isMemory))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            VvSQL.ReportSqlError("CREATE_TABLE " + dbName + "." + recordName + " :", ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   public static XSqlCommand CREATE_TABLE_Command(XSqlConnection conn, string dbName, string tableName, bool isMemoryEngine)
   {
      XSqlCommand cmd = InitCommand(conn);

      //cmd.CommandText = "CREATE DATABASE IF NOT EXISTS " + dbName +
      //                  " DEFAULT CHARACTER SET latin2" +
      //                  " DEFAULT COLLATE       latin2_croatian_ci";
      // ako ostavis IF NOT EXISTS onda ne javi err ako vec potoji pa user ne zna da mu nije uspjelo 


      // op.a.: tu maksimalno stedis prostor stringa, buduci da COMMENT kod MySQL-a je ogranicen na 60 znakova! 
      VvDaoBase.VvTableMetaData vvTableMetaData = new VvDaoBase.VvTableMetaData(/*tableName      ,*/ ZXC.TheVvForm.GetLastNewestCurrentTableVersionForMetaData(tableName), 
                                                                                VvSQL.GetServer_DateTime_Now(conn), DateTime.MinValue,       ZXC.vvDB_Server, 
                                                                                ZXC.vvDB_User,                      Environment.MachineName, ZXC.projectYear.Substring(2)); 


      cmd.CommandText = "CREATE TABLE " + dbName + "." + tableName + "(\n" +
                        ZXC.TheVvForm.Create_table_definition(dbName, tableName) +
                        ") ENGINE=" + (isMemoryEngine ? "MEMORY" : "MyISAM") + " DEFAULT CHARSET=latin2 COLLATE=latin2_croatian_ci " +
                        "COMMENT '" + vvTableMetaData.ToString() + "'";

      //------------------------ for debugging: 
      //VvDaoBase.VvTableMetaData testTableMetaData = new VvDaoBase.VvTableMetaData(vvTableMetaData.ToString());
      //------------------------ 

      return (cmd);
   }

   public static uint GetLastNewestCurrentTableVersionForMetaData(string recordName)
   {
      if(recordName == ZXC.vvDB_ucListTableName) return VvUcList_AddNew.TableVersionStatic_VEKTOR;

      if(recordName.StartsWith("vv") || recordName.StartsWith("zz")) return 1; // vvlog, vvlocker, vvusercontrol, zzLookUpTables, ...
      if(recordName.StartsWith(ZXC.vv_MBF_DbName))                   return 1; // MBF table 

      switch(recordName)
      {
         case Kplan.recordName:        
         case Kplan.recordNameArhiva:  return (KplanDao .TableVersionStatic);

         case Nalog .recordName:       
         case Nalog .recordNameArhiva: return (NalogDao .TableVersionStatic);

         case Ftrans.recordName:
         case Ftrans.recordNameArhiva: return (FtransDao.TableVersionStatic);

         case Kupdob.recordName:
         case Kupdob.recordNameArhiva: return (KupdobDao.TableVersionStatic);

         case Osred.recordName:
         case Osred.recordNameArhiva:  return (OsredDao .TableVersionStatic);

         case Amort.recordName:        
         case Amort.recordNameArhiva:  return (AmortDao .TableVersionStatic);

         case Atrans.recordName:
         case Atrans.recordNameArhiva: return (AtransDao.TableVersionStatic);

         case Person.recordName:
         case Person.recordNameArhiva: return (PersonDao.TableVersionStatic);

         case Placa.recordName:        
         case Placa.recordNameArhiva:  return (PlacaDao .TableVersionStatic);

         case Ptrans.recordName:
         case Ptrans.recordNameArhiva: return (PtransDao.TableVersionStatic);

         case Ptrane.recordName:
         case Ptrane.recordNameArhiva: return (PtraneDao.TableVersionStatic);

         case Ptrano.recordName:
         case Ptrano.recordNameArhiva: return (PtranoDao.TableVersionStatic);

         // ========================================================================================== 

         case Prjkt.recordName:        
         case Prjkt.recordNameArhiva:  return (KupdobDao.TableVersionStatic);

         case User .recordName:        
         case User .recordNameArhiva:  return (UserDao  .TableVersionStatic);

         case Prvlg.recordName:        
         case Prvlg.recordNameArhiva:  return (PrvlgDao .TableVersionStatic);

         case SkyRule.recordName:        
         case SkyRule.recordNameArhiva:return (SkyRuleDao.TableVersionStatic);

         case DevTec2.recordName:
         case DevTec2.recordNameArhiva: return (DevTecDao.TableVersionStatic);

         case Htrans2.recordName:
         case Htrans2.recordNameArhiva: return (HtransDao.TableVersionStatic);

         case Artikl.recordName:
         case Artikl.recordNameArhiva: return (ArtiklDao.TableVersionStatic);

         case ArtStat.recordName     : return (ArtStatDao.TableVersionStatic);

         case Faktur.recordName:
         case Faktur.recordNameArhiva: return (FakturDao.TableVersionStatic);

         case FaktEx.recordName:
         case FaktEx.recordNameArhiva: return (FaktExDao.TableVersionStatic);

         case Rtrans.recordName:
         case Rtrans.recordNameArhiva: return (RtransDao.TableVersionStatic);

         case Rtrano.recordName:
         case Rtrano.recordNameArhiva: return (RtranoDao.TableVersionStatic);

         case Mixer.recordName:        
         case Mixer.recordNameArhiva:  return (MixerDao .TableVersionStatic);

         case Xtrans.recordName:
         case Xtrans.recordNameArhiva: return (XtransDao.TableVersionStatic);

         case Xtrano.recordName:
         case Xtrano.recordNameArhiva: return (XtranoDao.TableVersionStatic);


         //case ZXC.vvDB_procTableName:   return (Process_Ex .Create_table_definition());
         //case ZXC.vvDB_logTableName:    return (VvLog_Ex   .Create_table_definition());
         //case ZXC.vvDB_lockerTableName: return (VvLocker_Ex.Create_table_definition());
         //case ZXC.vvDB_ucListTableName: return (VvUcList_Ex.Create_table_definition());
         //case ZXC.vvDB_initParamTblName:return (VvInit_Ex  .Create_table_definition());

         default:
               ZXC.aim_emsg("recordName: [" + recordName + "] nedefiniran u VvSQL.TableVersionForMetaData");
               return 0;
      }
   }

   public static string Create_table_definition(string dbName, string recordName)
   {
           if(recordName.StartsWith(ZXC.vvDB_SKYlogTableName   )) return (VvSkyLog_Ex.Create_table_definition());
      else if(recordName.StartsWith(ZXC.vvDB_ERRlogTableName   )) return (VvSkyLog_Ex.Create_table_definition());
      else if(recordName.StartsWith(ZXC.vvDB_CpyERRlogTableName)) return (VvSkyLog_Ex.Create_table_definition());
      else if(recordName.StartsWith(ZXC.vvDB_LANlogTableName   )) return (VvLog_Ex   .Create_table_definition());
      else if(dbName    .StartsWith(ZXC.vv_MBF_DbName          )) return (VvMBF_Ex   .Create_table_definition());
      else switch(recordName)
      {
         case Kplan.recordName:        return (KplanDao.Create_table_definition(false));
         case Kplan.recordNameArhiva:  return (KplanDao.Create_table_definition(true));

         case Nalog .recordName:       return (NalogDao .Create_table_definition(false));
         case Nalog .recordNameArhiva: return (NalogDao .Create_table_definition(true));

         case Ftrans.recordName:       return (FtransDao.Create_table_definition());
         case Ftrans.recordNameArhiva: return (FtransDao.Create_table_definition());

         case Kupdob.recordName:       return (KupdobDao.Create_table_definition(false, false));
         case Kupdob.recordNameArhiva: return (KupdobDao.Create_table_definition(false, true));

         case Osred.recordName:        return (OsredDao.Create_table_definition(false));
         case Osred.recordNameArhiva:  return (OsredDao.Create_table_definition(true));

         case Amort.recordName:        return (AmortDao.Create_table_definition(false));
         case Amort.recordNameArhiva:  return (AmortDao.Create_table_definition(true));

         case Atrans.recordName:       return (AtransDao.Create_table_definition());
         case Atrans.recordNameArhiva: return (AtransDao.Create_table_definition());

         case Person.recordName:       return (PersonDao.Create_table_definition(false));
         case Person.recordNameArhiva: return (PersonDao.Create_table_definition(true));

         case Placa.recordName:        return (PlacaDao.Create_table_definition(false));
         case Placa.recordNameArhiva:  return (PlacaDao.Create_table_definition(true));

         case Ptrans.recordName:       return (PtransDao.Create_table_definition());
         case Ptrans.recordNameArhiva: return (PtransDao.Create_table_definition());

         case Ptrane.recordName:       return (PtraneDao.Create_table_definition());
         case Ptrane.recordNameArhiva: return (PtraneDao.Create_table_definition());

         case Ptrano.recordName:       return (PtranoDao.Create_table_definition());
         case Ptrano.recordNameArhiva: return (PtranoDao.Create_table_definition());

         // ========================================================================================== 

         case Prjkt.recordName:        return (KupdobDao.Create_table_definition(true, false));
         case Prjkt.recordNameArhiva:  return (KupdobDao.Create_table_definition(true, true));

         case User .recordName:        return (UserDao  .Create_table_definition(false));
         case User .recordNameArhiva:  return (UserDao  .Create_table_definition(true));

         case Prvlg.recordName:        return (PrvlgDao .Create_table_definition(false));
         case Prvlg.recordNameArhiva:  return (PrvlgDao .Create_table_definition(true));

         case SkyRule.recordName:      return (SkyRuleDao.Create_table_definition(false));
         case SkyRule.recordNameArhiva:return (SkyRuleDao.Create_table_definition(true));

         case DevTec2.recordName:       return (DevTecDao.Create_table_definition(false));
         case DevTec2.recordNameArhiva: return (DevTecDao.Create_table_definition(true));

         case Htrans2.recordName:       return (HtransDao.Create_table_definition());
         case Htrans2.recordNameArhiva: return (HtransDao.Create_table_definition());

         case ArtStat.recordName:      return (ArtStatDao.Create_table_definition(false));

         case Artikl.recordName:       return (ArtiklDao.Create_table_definition(false));
         case Artikl.recordNameArhiva: return (ArtiklDao.Create_table_definition(true));

         case Faktur.recordName:       return (FakturDao.Create_table_definition(false));
         case Faktur.recordNameArhiva: return (FakturDao.Create_table_definition(true));

         case FaktEx.recordName:       return (FaktExDao.Create_table_definition(false));
         case FaktEx.recordNameArhiva: return (FaktExDao.Create_table_definition(true));

         case Rtrans.recordName:       return (RtransDao.Create_table_definition());
         case Rtrans.recordNameArhiva: return (RtransDao.Create_table_definition());

         case Rtrano.recordName:       return (RtranoDao.Create_table_definition());
         case Rtrano.recordNameArhiva: return (RtranoDao.Create_table_definition());

         case Mixer.recordName:        return (MixerDao.Create_table_definition(false));
         case Mixer.recordNameArhiva:  return (MixerDao.Create_table_definition(true));

         case Xtrans.recordName:       return (XtransDao.Create_table_definition());
         case Xtrans.recordNameArhiva: return (XtransDao.Create_table_definition());

         case Xtrano.recordName:       return (XtranoDao.Create_table_definition());
         case Xtrano.recordNameArhiva: return (XtranoDao.Create_table_definition());


         case ZXC.vvDB_procTableName:   return (Process_Ex .Create_table_definition());
       //case ZXC.vvDB_logTableName:    return (VvLog_Ex   .Create_table_definition());
         case ZXC.vvDB_lockerTableName: return (VvLocker_Ex.Create_table_definition());
         case ZXC.vvDB_SKYlockerTableName: return (VvSKYlocker_Ex.Create_table_definition());
         case ZXC.vvDB_riskMacroTableName: return (VvRiskMacro_Ex.Create_table_definition());
         case ZXC.vvDB_ucListTableName: return (VvUcList_Ex.Create_table_definition());

         default:
            if(recordName.StartsWith(ZXC.vvDB_luiPrefix))
            {
               return (VvLookUp_Ex.Create_table_definition(recordName));
            }
            else
            {
               ZXC.aim_emsg("recordName: [" + recordName + "] nedefiniran u VvSQL.CREATE_TABLE_definition");
               return String.Empty;
            }
      }
   }

   #region AddNewVvUserControl

   public static bool ALTER_TABLE_AddVvUserControl_JOB(XSqlConnection tempConnection, uint catchingVersion)
   {

      bool success = true;
      int nora = -1; // number of rows affected 

      using(XSqlCommand cmd = VvSQL.ALTER_TABLE_AddVvUserControl_Command(tempConnection, catchingVersion))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            VvSQL.ReportSqlError("ADD NEW VvUserControl " + " catchingVersion " + catchingVersion + " :", ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   private static XSqlCommand ALTER_TABLE_AddVvUserControl_Command(XSqlConnection conn, uint catchingVersion)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = ZXC.TheVvForm.GetALTER_TABLE_AddVvUserControl_Command(catchingVersion);

      return (cmd);
   }

   #endregion AddNewVvUserControl

   public static bool ALTER_TABLE_ForCatchUp_JOB(XSqlConnection tempConnection, string dbName, string recordName, uint catchingVersion, VvDaoBase.VvTableMetaData vvTableMetaData)
   {

      bool success = true;
      int nora = -1; // number of rows affected 

      #region AddNewVvUserControl

      if(recordName == ZXC.vvDB_ucListTableName) // tu treba ADDREC-ati neki novi UC 
      {
         success = ALTER_TABLE_AddVvUserControl_JOB(tempConnection, catchingVersion);

         if(!success) return false;
      }

      #endregion AddNewVvUserControl

      using(XSqlCommand cmd = VvSQL.ALTER_TABLE_ForCatchUp_Command(tempConnection, dbName, recordName, catchingVersion, vvTableMetaData))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            if(ex.Number == 1146) // Error: 1146 SQLSTATE: 42S02 (ER_NO_SUCH_TABLE)
            {
            }
            VvSQL.ReportSqlError("ALTER_TABLE " + dbName + "." + recordName + " catchingVersion " + catchingVersion + " :", ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   public static XSqlCommand ALTER_TABLE_ForCatchUp_Command(XSqlConnection conn, string dbName, string tableName, uint catchingVersion, VvDaoBase.VvTableMetaData vvTableMetaData)
   {
      XSqlCommand cmd = InitCommand(conn);

      // 14.05.2013: 
      string alterTableDefinition = ZXC.TheVvForm.Alter_table_definition(tableName, catchingVersion);
      bool   skip_me              = alterTableDefinition.Contains("skip_me");

      vvTableMetaData.TableVersion = catchingVersion;
      vvTableMetaData.DateModifyed = VvSQL.GetServer_DateTime_Now(conn);

      string commaOrNot = (tableName == ZXC.vvDB_ucListTableName ? "" : ",");

      if(tableName.StartsWith(Kupdob.recordName) && 
         (catchingVersion == 6  ||
          catchingVersion == 7  ||
          catchingVersion == 9  ||
          catchingVersion == 12 ||
          catchingVersion == 13 ||
          catchingVersion == 14 ||
          catchingVersion == 15 ||
          catchingVersion == 17 ||
          catchingVersion == 21 ||
          catchingVersion == 23 ||
          catchingVersion == 25 ||
          catchingVersion == 26 ||
          catchingVersion == 27 ||
          catchingVersion == 28 ||
          catchingVersion == 29 ||
          catchingVersion == 30 ||
          catchingVersion == 31 ||
          catchingVersion == 32 ||
          catchingVersion == 34 ||
          catchingVersion == 35 ||
          catchingVersion == 36 )) commaOrNot = ""; // ova verzija alter-a samo Prjkt pa zarez smeta 

      if(skip_me == true) commaOrNot = ""; // ova verzija alter-a samo table_ar pa zarez smeta 

      // 12.12.2011: VvRECREATE CACHE 
      if(tableName == ArtStat.recordName && alterTableDefinition.StartsWith("VvRECREATE"))
      {
         cmd.CommandText = "DROP TABLE IF EXISTS " + dbName + "." + tableName;
      }
      else // DEFAULT, NORMAL CASE for ALTER TABLE 
      {
         cmd.CommandText = "ALTER TABLE " + dbName + "." + tableName + " \n" +
                           "COMMENT '" + vvTableMetaData.ToString() + "'" + commaOrNot + "\n" +
                           (skip_me ? "" : alterTableDefinition);
      }

      return (cmd);
   }

   public static bool AlterTo_NEW_lanlog_table_definition(XSqlConnection tempConnection, string dbName)
   {

      bool success = true;
      int nora = -1; // number of rows affected 

      using(XSqlCommand cmd = VvSQL.AlterTo_NEW_lanlog_table_definition_Command(tempConnection, dbName))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            if(ex.Number == 1146) // Error: 1146 SQLSTATE: 42S02 (ER_NO_SUCH_TABLE)
            {
            }
            VvSQL.ReportSqlError("ALTER_TABLE " + dbName + "." + ZXC.vvDB_LANlogTableName + " :", ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   public static XSqlCommand AlterTo_NEW_lanlog_table_definition_Command(XSqlConnection conn, string dbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = VvLog_Ex.AlterTo_NEW_lanlog_table_definitionText(dbName);
      
      return (cmd);
   }

   public static string Alter_table_definition(string recordName, uint catchingVersion)
   {
      switch(recordName)
      {
         case Kupdob.recordName:        return (KupdobDao.Alter_table_definition(catchingVersion, false, false));
         case Kupdob.recordNameArhiva:  return (KupdobDao.Alter_table_definition(catchingVersion, false, true));

         case Prjkt.recordName:         return (KupdobDao.Alter_table_definition(catchingVersion, true, false));
         case Prjkt.recordNameArhiva:   return (KupdobDao.Alter_table_definition(catchingVersion, true, true));

         case Kplan.recordName:         return (KplanDao.Alter_table_definition(catchingVersion, false));
         case Kplan.recordNameArhiva:   return (KplanDao.Alter_table_definition(catchingVersion, true));

         case Ptrano.recordName:        return (PtranoDao.Alter_table_definition(catchingVersion, false));
         case Ptrano.recordNameArhiva:  return (PtranoDao.Alter_table_definition(catchingVersion, true));
                                        
         case Placa.recordName:         return (PlacaDao.Alter_table_definition(catchingVersion, false));
         case Placa.recordNameArhiva:   return (PlacaDao.Alter_table_definition(catchingVersion, true));
                                        
         case Nalog.recordName:         return (NalogDao.Alter_table_definition(catchingVersion, false));
         case Nalog.recordNameArhiva:   return (NalogDao.Alter_table_definition(catchingVersion, true));
                                        
         case Osred.recordName:         return (OsredDao.Alter_table_definition(catchingVersion, false));
         case Osred.recordNameArhiva:   return (OsredDao.Alter_table_definition(catchingVersion, true));
                                        
         case Amort.recordName:         return (AmortDao.Alter_table_definition(catchingVersion, false));
         case Amort.recordNameArhiva:   return (AmortDao.Alter_table_definition(catchingVersion, true));
                                        
         case Ftrans.recordName:        return (FtransDao.Alter_table_definition(catchingVersion, false));
         case Ftrans.recordNameArhiva:  return (FtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Atrans.recordName:        return (AtransDao.Alter_table_definition(catchingVersion, false));
         case Atrans.recordNameArhiva:  return (AtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Faktur.recordName:        return (FakturDao.Alter_table_definition(catchingVersion, false));
         case Faktur.recordNameArhiva:  return (FakturDao.Alter_table_definition(catchingVersion, true));
                                        
         case Person.recordName:        return (PersonDao.Alter_table_definition(catchingVersion, false));
         case Person.recordNameArhiva:  return (PersonDao.Alter_table_definition(catchingVersion, true));
                                        
         case FaktEx.recordName:        return (FaktExDao.Alter_table_definition(catchingVersion, false));
         case FaktEx.recordNameArhiva:  return (FaktExDao.Alter_table_definition(catchingVersion, true));

         case Rtrans.recordName:        return (RtransDao.Alter_table_definition(catchingVersion, false));
         case Rtrans.recordNameArhiva:  return (RtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Rtrano.recordName:        return (RtranoDao.Alter_table_definition(catchingVersion, false));
         case Rtrano.recordNameArhiva:  return (RtranoDao.Alter_table_definition(catchingVersion, true));
                                        
         case Ptrans.recordName:        return (PtransDao.Alter_table_definition(catchingVersion, false));
         case Ptrans.recordNameArhiva:  return (PtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Ptrane.recordName:        return (PtraneDao.Alter_table_definition(catchingVersion, false));
         case Ptrane.recordNameArhiva:  return (PtraneDao.Alter_table_definition(catchingVersion, true));
                                        
         case ArtStat.recordName:       return (ArtStatDao.Alter_table_definition(catchingVersion));
                                        
         case Artikl.recordName:        return (ArtiklDao.Alter_table_definition(catchingVersion, false));
         case Artikl.recordNameArhiva:  return (ArtiklDao.Alter_table_definition(catchingVersion, true));

         case User.recordName:          return (UserDao.Alter_table_definition(catchingVersion, false));
         case User.recordNameArhiva:    return (UserDao.Alter_table_definition(catchingVersion, true));

         case Prvlg.recordName:         return (PrvlgDao.Alter_table_definition(catchingVersion, false));
         case Prvlg.recordNameArhiva:   return (PrvlgDao.Alter_table_definition(catchingVersion, true));
                                        
         case SkyRule.recordName:       return (SkyRuleDao.Alter_table_definition(catchingVersion, false));
         case SkyRule.recordNameArhiva: return (SkyRuleDao.Alter_table_definition(catchingVersion, true));
                                        
         case DevTec2.recordName:        return (DevTecDao.Alter_table_definition(catchingVersion, false));
         case DevTec2.recordNameArhiva:  return (DevTecDao.Alter_table_definition(catchingVersion, true));
                                        
         case Htrans2.recordName:        return (HtransDao.Alter_table_definition(catchingVersion, false));
         case Htrans2.recordNameArhiva:  return (HtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Mixer.recordName:         return (MixerDao.Alter_table_definition(catchingVersion, false));
         case Mixer.recordNameArhiva:   return (MixerDao.Alter_table_definition(catchingVersion, true));
                                        
         case Xtrans.recordName:        return (XtransDao.Alter_table_definition(catchingVersion, false));
         case Xtrans.recordNameArhiva:  return (XtransDao.Alter_table_definition(catchingVersion, true));
                                        
         case Xtrano.recordName:        return (XtranoDao.Alter_table_definition(catchingVersion, false));
         case Xtrano.recordNameArhiva:  return (XtranoDao.Alter_table_definition(catchingVersion, true));
                                        
         case ZXC.vvDB_ucListTableName: return "";


         
         default: ZXC.aim_emsg("recordName: [" + recordName + "] nedefiniran u VvSQL.Alter_table_definition");
                  return String.Empty;
      }
   }

   #region PUSE 

   //public static bool CREATE_ALL_PUG_TABLES(MySqlConnection tempConnection, string dbName)
   //{
   //   bool success = true;

   //   // tu navedi kasnije komplet svih tablica koje se otvaraju za novi PUG (Projekt u godini) 

   //   //if(success) success = VvSQL.CREATE_TABLE(tempConnection, dbName, Kplan.tableName, false);
   //   //...if(success) success = VvSQL.CREATE_TABLE(tempConnection, dbName, Xy.tableName);

   //   foreach(VvDataRecord dataRec in ZXC.PUG_RecordList)
   //   {
   //      if(success) success = VvSQL.CREATE_TABLE(tempConnection, dbName, dataRec.VirtualRecordName, false);

   //      ZXC.aim_emsg(dataRec.GetType().ToString());
   //   }

   //   if(success) success = VvSQL.CREATE_TABLE(tempConnection, dbName, ZXC.vvDB_logTableName, false);

   //   return (success);
   //}

   //public static bool CREATE_ALL_PRJKT_TABLES(MySqlConnection tempConnection)
   //{
   //   bool success = true;

   //   if(success) success = VvSQL.CREATE_TABLE(tempConnection, ZXC.vvDB_prjktName, Prjkt.tableName, false);
   //   if(success) success = VvSQL.CREATE_TABLE(tempConnection, ZXC.vvDB_prjktName, ZXC.vvDB_logTableName, false);
   //   if(success) success = VvSQL.CREATE_TABLE(tempConnection, ZXC.vvDB_prjktName, ZXC.vvDB_procTableName, true);

   //   return (success);
   //}

   #endregion PUSE

   public static bool DROP_TABLE(XSqlConnection conn, string dbName, string tableName)
   {

      bool success = true;
      int nora = -1; // number of rows affected 


      using(XSqlCommand cmd = VvSQL.DROP_TABLE_Command(conn, dbName, tableName))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            VvSQL.ReportSqlError("DROP_TABLE " + dbName + "." + tableName + " :", ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   public static XSqlCommand DROP_TABLE_Command(XSqlConnection conn, string dbName, string tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DROP TABLE IF EXISTS " + dbName + "." + tableName;

      return (cmd);
   }

   #endregion CREATE_TABLE

   #region CreateMySql_XmlSchema

   public static void CreateMySql_XmlSchema(string tblName, string xmlFileName) //KplanMySql_DataSet - xmlFileName example 
   {


      // Create the dataset and adapters 
      DataSet ds = new DataSet("TypedDataSet_" + tblName);
      //MySqlDataAdapter da = CreateSceppaDataAdapter(); 
      XSqlDataAdapter da = new XSqlDataAdapter(@"SELECT * FROM " + tblName, ZXC.TheVvForm.TheDbConnection);
      // upali ovu dole umjesto ove gora ako kreiras shemu za prjkt 
      //MySqlDataAdapter da = new MySqlDataAdapter(@"SELECT * FROM " + "vvprojekti." + tblName, ZXC.TheVvForm.conn);

      // Use the adapters to fill schemas for each table 
      ds.EnforceConstraints = false;
      da.FillSchema(ds, SchemaType.Source, tblName);
      ds.EnforceConstraints = false;


      // Set up variables for creating relationships 
      //DataColumn parent; 
      //DataColumn child; 
      //DataRelation relation; 

      // Create the relationship 
      //parent = ds.Tables["table1"].Columns["RelationshipID"]; 
      //child = ds.Tables["table2"].Columns["RelationshipID"]; 
      //relation = new DataRelation("table1_table2",parent,child); 
      //ds.Relations.Add(relation); 

      /**/
      DataColumn dcRowNum = new DataColumn("rowNum", typeof(UInt32), null, System.Data.MappingType.Element);
      dcRowNum.AutoIncrement = true;
      dcRowNum.AutoIncrementSeed = 1;
      // bez ovoga nece da bide uint nago int: (mada si mu gore dao 'typeof(UInt32)') 
      dcRowNum.DataType = typeof(uint);
      ds.Tables[tblName].Columns.Add(dcRowNum);
      /**/


      ds.WriteXmlSchema(xmlFileName);

   }

   #endregion CreateMySql_XmlSchema

   #region GetNextDokNum_Command

   public const string dokNumColumnName = "dokNum";

   public static XSqlCommand GetNextDokNum_Command(XSqlConnection conn, string recordName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(" + VvSQL.dokNumColumnName + ") FROM " + recordName;

      return (cmd);
   }

   public static XSqlCommand GetNextSifra_Uint_Command(XSqlConnection conn, string recordName, string sifraColName, uint rootNum, uint baseFactor)
   {
      XSqlCommand cmd = InitCommand(conn);

      bool isInBounds = rootNum.NotZero() && baseFactor.NotZero();

      uint lowerBound =  rootNum      * baseFactor; // npr: 12 * 1000 = 12000 
      uint upperBound = (rootNum + 1) * baseFactor; // npr: 13 * 1000 = 13000 

      if(isInBounds == false) // classic 
         cmd.CommandText = "SELECT MAX(" + sifraColName + ") FROM " + recordName;

      else
         cmd.CommandText = "SELECT MAX(" + sifraColName + ") FROM " + recordName + "\n"    + 
                           "WHERE "      + sifraColName + " > "     + lowerBound + " AND " + sifraColName + " < " + upperBound;

      return (cmd);
   }

   public static XSqlCommand GetNextSifra_String_Command(XSqlConnection conn, string recordName, string sifraColName)
   {
      XSqlCommand cmd = InitCommand(conn);

    //cmd.CommandText = "SELECT CAST(MAX(" + sifraColName + ") AS UNSIGNED) FROM " + recordName +
    //                  " WHERE TRIM(LEADING '0' FROM " + sifraColName + ") = CAST(CAST(" + sifraColName + " AS UNSIGNED) AS CHAR)";
    //cmd.CommandText = "SELECT CAST(MAX(" + sifraColName + ") AS UNSIGNED) FROM " + recordName +
    //                  " WHERE STRCMP(TRIM(LEADING '0' FROM " + sifraColName + "), CAST(CAST(" + sifraColName + " AS UNSIGNED) AS CHAR)) = 0";
      cmd.CommandText = "SELECT CAST(MAX(" + sifraColName + ") AS UNSIGNED) FROM " + recordName + " WHERE " + sifraColName + " REGEXP '^[0-9]*$'";

      return (cmd);
   }

   public static XSqlCommand GetNextSifraWroot_String_Command(XSqlConnection conn, string recordName, string sifraColName, string rootPart)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(REPLACE(UCASE(" + sifraColName + "), '" + rootPart + "', '')) FROM " + 

                         recordName + " WHERE " + sifraColName + " REGEXP '" + rootPart + "[0-9]*$'";

      return (cmd);
   }

   public static XSqlCommand GetNext_PTG_YYinTtNum_Command(XSqlConnection conn, string wantedTT, uint rootPartNum_min, uint rootPartNum_max, string eventualSkladCD, bool isPTG_YYinTtNum_99999, string[] skladCDarray)
   {
      XSqlCommand cmd = InitCommand(conn);

      // SELECT MAX(TtNum) FROM faktur         
      //                                       
      // WHERE TT = 'PRI'                      
      //                                       
      // AND TtNum > 24110000 #rootPartNum_Min 
      // AND TtNum < 25110000 #rootPartNum_Max 

      string skladOrOPP_restrictor = isPTG_YYinTtNum_99999 ?

         "AND " + Faktur.skladCd_colName + " IN " + TtInfo.GetSql_IN_Clause(skladCDarray) : 

         "AND " + Faktur.skladCd_colName + " = '" + eventualSkladCD + "'";

      cmd.CommandText = "SELECT MAX(" + Faktur.ttNum_colName + ") FROM " + Faktur.recordName + "\n\n" +

                        " WHERE TT = '" + wantedTT + "'\n\n" +

                       skladOrOPP_restrictor       + " \n\n" +

                        "AND " + Faktur.ttNum_colName + " > " + rootPartNum_min + "\n" +
                        "AND " + Faktur.ttNum_colName + " < " + rootPartNum_max        ;

      return (cmd);
   }

   public static XSqlCommand GetFirstDokDate_Command(XSqlConnection conn, string recordName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MIN(" + VvSQL.dokDateColName + ") FROM " + recordName;

      return (cmd);
   }

   public const string ttColName        = "tt"       ;
   public const string ttNumColName     = "ttNum"    ;
   public const string recVerColName    = "recVer"   ;
   public const string origRecIDColName = "origRecID";
   public const string dokDateColName   = "dokDate"  ;

   public static XSqlCommand GetNextTtNum_Command(XSqlConnection conn, string recordName, string wantedTT, string wantedSkladCD, bool isCentToCentMSI/*, string eventualRNMkind*/)
   {
      XSqlCommand cmd = InitCommand(conn);

      // 18.03.2014: Komisija News 
      if(wantedSkladCD.NotEmpty())
      {
         List<string> OPPskladCDlist = ZXC.luiListaSkladista.GetOPPskladCDlist(wantedSkladCD, wantedTT);

         bool isMultiSklad = OPPskladCDlist != null && OPPskladCDlist.Count > 1;

         string skladCDinOrEqualClause;

       //if(isCentToCentMSI == false                                        ) // classic
         if(isCentToCentMSI == false && ZXC.RRD.Dsc_IsMSIttNumByPosl == true) // classic
         {
            skladCDinOrEqualClause = "AND (skladCD " + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : " = '" + wantedSkladCD) + 
                                          (isMultiSklad ? " OR " : "' OR ")   + "skladCD2" + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : " = '" + wantedSkladCD) + 
                                          (isMultiSklad ? ")   " : "')   ");
         }
         else // isCentToCentMSI 
         {
            skladCDinOrEqualClause = "AND (skladCD " + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : " = '" + wantedSkladCD) +                                     
                                          (isMultiSklad ? ")   " : "')   ");
         }


         cmd.CommandText = "SELECT MAX(" + VvSQL.ttNumColName + ") FROM " + recordName + "\n " +
                           "WHERE "      + VvSQL.ttColName    + " = '"    + wantedTT   + "'\n" +

                         //v1: (skladCD.NotEmpty() ? "AND skladCD = '"  + skladCD + "' " : "");
                         //v2: "AND " + (isSkladCD2 ? "skladCD2" : "skladCD") + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : 
                         //v2:                                                                  " = '" + wantedSkladCD + "' ");

                         skladCDinOrEqualClause +

                         // 05.02.2015: 
                         (isCentToCentMSI ? "\n\nAND SUBSTRING(skladCD,  1, 2) = '" + ZXC.vvDB_ServerID_CENTRALA.ToString("00") + "'" + 
                                               " AND SUBSTRING(skladCD2, 1, 2) = '" + ZXC.vvDB_ServerID_CENTRALA.ToString("00") + "'" : "");


            // Sada gore osposobi IN_clause i
            // SREDI DA OPP_BR ide SAMO ZA PRIHOD_TT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // da si uspio, izmedju ostaloga, isprobaj dodati novo PocetnoStanje na skl GLSK. Tamo te jebalo OPP...
         //.Replace(")'", ")")
      }
      else
      {
         cmd.CommandText = "SELECT MAX(" + VvSQL.ttNumColName + ") FROM " + recordName + " \n" +
                           "WHERE "      + VvSQL.ttColName    + " = '"    + wantedTT   + "'\n" ;
      }

      // 26.04.2022: 
      if(ZXC.IsSvDUH && wantedSkladCD == "10")
      {
         cmd.CommandText += "\n\nAND ttnum < 200000"; // jer smo u sve dokumenta sa skl 70 i 90 prebacili na 10 a ostali su ttNum-ovi 7xxxxx i 9xxxxx 
      }

      return (cmd);
   }

   public static XSqlCommand GetNext_UrudzbeniBroj_Command(XSqlConnection conn, string rootOfNum)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(SUBSTRING(strD_32, 11)) FROM " + Mixer.recordName + "\n" +
                        "WHERE tt = '" + Mixer.TT_URZ + "' AND SUBSTRING(strD_32, 1, 10) = '" + rootOfNum + "'";

      return (cmd);
   }

   public static XSqlCommand GetNext_UrudzbeniBroj_Od_19032025_Command(XSqlConnection conn, string recordName, string ttColName, string ttNumColName, string wantedTT)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(" + ttNumColName + ") FROM " + recordName + "\n" +
                        "WHERE " + ttColName + " = '" + wantedTT + "' ";

      return (cmd);
   }


   public static XSqlCommand GetNextArhivaRecordVersion_Command(XSqlConnection conn, string recordArhivaName, uint origRecID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(" + VvSQL.recVerColName + ") FROM " + recordArhivaName + "\n" +
                        "WHERE "      + VvSQL.origRecIDColName + " = " + origRecID;

      return (cmd);
   }

   public static XSqlCommand GetNextOvrDokNum_Command(XSqlConnection conn, string recordName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(" + "ovrDokNum" + ") FROM " + recordName;

      return (cmd);
   }

   public static XSqlCommand GetNext_KUGinTtNum_TtNum_Command(XSqlConnection conn, string recordName, /*string wantedTT,*/ uint KUGnum, uint UGANnum, bool isDODnumWanted)
   {
      XSqlCommand cmd = InitCommand(conn);

    //string whichTT = isDODnumWanted ? "(TT = 'DIZ' OR TT = 'PVR' OR TT = 'ZIZ')"    : "TT = 'AUN'";
      string whichTT = isDODnumWanted ? "TT IN " + GetInSetClause(TtInfo.array_FakturDodTT) : "TT = 'AUN'";

      cmd.CommandText = "SELECT MAX(" + VvSQL.ttNumColName + ") FROM " + recordName + " \n" +
                        "WHERE "      + whichTT                                     + " \n" +
                        "AND   "      + "v1_ttNum"         + " =  "    + KUGnum     + " \n" +
                      (isDODnumWanted?
                        "AND   "      + "v2_ttNum"         + " =  "    + UGANnum    + " \n" : "");

      return (cmd);
   }

   public static XSqlCommand GetNext_ZAH_TtNum_Command(XSqlConnection conn, string recordName, string wantedTT, string ticker)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(" + VvSQL.ttNumColName + ") FROM " + recordName     + " F\n" +
                        "LEFT JOIN  " + FaktEx.recordName  + " X ON X.FakturRecID = F.RecID  \n" +
                        "WHERE "      + VvSQL.ttColName    + " = '"    + wantedTT       + "'\n" +
                        "AND   "      + "KupdobTK"         + " = '"    + ticker         + "'\n" ;

      return (cmd);
   }

   public static XSqlCommand GetNext_ABx_TtNum_Command(XSqlConnection conn, string recordName, string wantedTT, string wantedSkladCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      List<string> OPPskladCDlist = ZXC.luiListaSkladista.GetOPPskladCDlist(wantedSkladCD, wantedTT);

      bool isMultiSklad = OPPskladCDlist != null && OPPskladCDlist.Count > 1;

      string skladCDinOrEqualClause = "AND (skladCD " + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : " = '" + wantedSkladCD) + 
                                    (isMultiSklad ? ")   " : "')   ");

      cmd.CommandText = "SELECT MAX(" + VvSQL.ttNumColName + ") FROM " + recordName + "\n " +
                        "WHERE "      + VvSQL.ttColName    + " = '"    + wantedTT   + "'\n" +

                      skladCDinOrEqualClause + "";

      return (cmd);
   }

   #endregion GetNextDokNum_Command

   #region GetTranses_Command

   public static XSqlCommand GetTranses_Command(XSqlConnection conn, string transRecordName, uint documentRecID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT * FROM " + transRecordName + " WHERE t_parentID = " + documentRecID + " ORDER BY t_serial";

      return (cmd);
   }

   #endregion GetTranses_Command

   #region GetSchemaTable_Command
   
   public static string GetDbNameForThisTableName(string tableName)
   {
      // Ovo StartsWith je da uzme u obzir i User i User_ar varijante tableName-a 
      if(tableName.StartsWith(Prjkt  .recordName) ||
         tableName.StartsWith(User   .recordName) ||
         tableName.StartsWith(Prvlg  .recordName) ||
         tableName.StartsWith(SkyRule.recordName) ||
         tableName.StartsWith(DevTec2 .recordName) ||
         tableName.StartsWith(Htrans2 .recordName) || // dodano tek 14.10.2017 
         tableName.StartsWith(ZXC.vvDB_luiPrefix) /*||
         tableName.StartsWith(ZXC.vvDB_SKYlogTableName)*/) 
      {
         return ZXC.TheVvForm.GetvvDB_prjktDB_name();
      }
      else
      {
         if(ZXC.TheVvForm.TheVvTabPage == null)
         {
            return ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName;
         }
         else
         {
            return ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName;
         }
      }
   }

   public static XSqlCommand GetSchemaTable_Command(XSqlConnection tmpConn, string dbName, string tableName)
   {
      //string dbName;
      XSqlCommand cmd;

      //dbName = VvSQL.GetDbNameForThisTableName(tableName);

      cmd = tmpConn.CreateCommand();

      cmd.CommandType = CommandType.Text;

      cmd.UpdatedRowSource = UpdateRowSource.None;

      cmd.CommandText = "SELECT * FROM " + dbName + "." + tableName + " LIMIT 1\n";

      return (cmd);
   }

   #endregion GetSchemaTable_Command

   #region GetAllDataTableColumnNames_4Select

   public static string GetAllDataTableColumnNames_4Select(DataTable dt, string tableNameForPreffix, bool skipNon_t_, bool isArhiva)
   {
      return GetAllDataTableColumnNames_4Select(dt, tableNameForPreffix, "", skipNon_t_, isArhiva, "");
   }

   public static string GetAllDataTableColumnNames_4Select(DataTable dt, string tableNameForPreffix, string skipThisPreffix, bool isArhiva)
   {
      return GetAllDataTableColumnNames_4Select(dt, tableNameForPreffix, skipThisPreffix, false, isArhiva, "");
   }

   public static string GetAllDataTableColumnNames_4Select(DataTable dt, string tableNameForPreffix, bool isArhiva, string skipAllButThisPreffix)
   {
      return GetAllDataTableColumnNames_4Select(dt, tableNameForPreffix, "", false, isArhiva, skipAllButThisPreffix);
   }

   public static string GetAllDataTableColumnNames_4Select(DataTable dt, string tableNameForPreffix, string skipThisPreffix, bool skipNon_t_, bool isArhiva, string skipAllButThisPreffix)
   {
      System.Text.StringBuilder sb = new System.Text.StringBuilder("\n");

      string theColName;

      foreach(DataColumn dc in dt.Columns)
      {
         if(!dc.ColumnName.StartsWith("t_") && skipNon_t_) continue; // skipaj 'kpdb_name', 'kpln_name', itd, ... 

         if(skipThisPreffix.NotEmpty() && dc.ColumnName.StartsWith(skipThisPreffix)) continue;
         
         if(skipAllButThisPreffix.NotEmpty() && dc.ColumnName.StartsWith(skipAllButThisPreffix) == false) continue; 

         if(isArhiva == false)
         {
            if(VvDataRecord.arhivaColumnNames  .Contains(dc.ColumnName)) continue;
            if(VvDataRecord.metadataColumnNames.Contains(dc.ColumnName)) continue;
         }

         // 11.03.2009 zbog surgera (ttOP col. na DS_findOperAna): 
         if(dc.Expression.NotEmpty()) continue;

         if(skipAllButThisPreffix.NotEmpty()) theColName = dc.ColumnName.Replace(skipAllButThisPreffix, "") + " AS " + dc.ColumnName;
         else                                 theColName = dc.ColumnName;

         sb.Append(tableNameForPreffix + "." + theColName + ", ");
      }

      sb.Remove(sb.Length - 2, 2); // makni zadnja 2 znaka sa zadnjega 
      sb.Append("\n");

      return sb.ToString();
   }

   #endregion GetAllDataTableColumnNames_4Select

   #region Reports (WhereClauseFromVvSqlFilter)

   public static string ParameterizedWhereClauseFromVvSqlFilter(List<VvSqlFilterMember> filterMembers, bool whereAlreadyInserted)
   {
      if(filterMembers == null) return "";

      string columnName, fullColumnName, where_or_and, nonParameterizedValue;
      System.Text.StringBuilder sb = new System.Text.StringBuilder(" \n");

      bool OR_active = false;

      if(!whereAlreadyInserted) where_or_and = "WHERE ";
      else                      where_or_and = "AND   ";
      
      foreach(VvSqlFilterMember member in filterMembers)
      {
         if(member.isDummy) continue;

         if(member.drSchema != null)
         {
            columnName = (string)(member.drSchema["ColumnName"]);

            if(member.relatedTable.NotEmpty())
            {
               string preffix = VvSQL.GetShortRecordName(member.relatedTable);
               fullColumnName = preffix + "." + columnName;
            }
            else if(member.forcedPreffix.NotEmpty())
            {
               fullColumnName = member.forcedPreffix + "." + columnName;
            }
            else
            {
               fullColumnName = columnName;
            }

            if(member.or_state == ZXC.FM_OR_Enum.OPEN_OR) // start OR 
            {
               sb.Append(where_or_and + "(" + fullColumnName + member.operand + "?filter_" + member.name + " \n");
               OR_active = true;
            }
            else if(OR_active)
            {
               sb.Append("OR " + fullColumnName + member.operand + "?filter_" + member.name + " \n");

               if(member.or_state == ZXC.FM_OR_Enum.CLOSE_OR) // end OR 
               {
                  sb.Append(")\n");
                  OR_active = false;
               }
            }
            else // default, standard case 
            {
               sb.Append(where_or_and + fullColumnName + member.operand + "?filter_" + member.name + " \n");
            }

         }

         else // doslovni columnName i ne parametrizirani value 
         {
            if(member.value is decimal) nonParameterizedValue = ((decimal)member.value).ToString(ZXC.VvNumberFormatInfo2_ForceDot);
            else                        nonParameterizedValue = member.value.ToString();

            if(member.or_state == ZXC.FM_OR_Enum.OPEN_OR) // start OR 
            {
               sb.Append(where_or_and + "(" + member.forcedColName + member.operand + nonParameterizedValue + " \n");
               OR_active = true;
            }
            else if(OR_active)
            {
               sb.Append("OR " + member.forcedColName + member.operand + nonParameterizedValue + " \n");

               if(member.or_state == ZXC.FM_OR_Enum.CLOSE_OR) // end OR 
               {
                  sb.Append(")\n");
                  OR_active = false;
               }
            }
            else // default, standard case 
            {
               sb.Append(where_or_and + member.forcedColName + member.operand + nonParameterizedValue + " \n");
            }

         } // doslovni columnName i ne parametrizirani value 

         if(where_or_and == "WHERE ") where_or_and = "AND   ";
      }

      return sb.ToString();
   }

   private static string GetShortRecordName(string recordName)
   {
      switch(recordName)
      {
         case Kupdob.recordName         : 
         case Kupdob.recordName + "_fak": return "kpdb";

         case Kplan  .recordName: return "kpln" ;
         case Osred  .recordName: return "osred";
         case Person .recordName: return "prsn" ; 
         case Placa  .recordName: return "plca" ; 
         case "debit"           : return "debit";
         case Ptrano .recordName: return "ptrno";
         case ArtStat.recordName: return "artst";
         case Faktur .recordName: return "fakt" ; 
         case Ftrans .recordName: return "ftrn" ; 

         default: ZXC.aim_emsg("VvSQL.GetShortRecordName: UInknown recordName! " + recordName); return "XY";
      }
   }

   public static void SetReportCommandParamValues(XSqlCommand cmd, List<VvSqlFilterMember> filterMembers)
   {
      if(filterMembers == null) return;

      foreach(VvSqlFilterMember member in filterMembers)
      {
         // 15.11.2011: 
         //if(member.isDummy) continue;
         if(member.isDummy)
         {
            if(member.name == "dateOTS") // specijal kejs gdje treba samo paramtear value ali ne i u 'ParameterizedWhereClauseFromVvSqlFilter' (otsSubQuerry)
            {
               CreateCommandNamedParameter(cmd, "prm_", member.name, member.value, member.drSchema);
            }
            else
            {
               continue;
            }
         }

         if(member.drSchema == null) continue;

         CreateCommandNamedParameter(cmd, "filter_", member.name, member.value, member.drSchema);
      }
   }

   public static void ClearAllColumnsExpressions(DataColumnCollection dataColumnCollection)
   {
      foreach(DataColumn dc in dataColumnCollection)
      {
         if(dc.Expression.NotEmpty()) dc.Expression = "";
      }
   }

   internal static string GetInSetClause(IEnumerable<string> distinctWantedMemberList)
   {
      bool touched = false;

      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      sb.Append("(");

      foreach(string member in distinctWantedMemberList)
      {
         touched = true;
         sb.Append("'" + member + "', ");
      }

      //sb.Remove(sb.Length - suffix.Length, suffix.Length); // makni suffix sa zadnjega 
      if(touched) sb.Remove(sb.Length - 2, 2);
      else        return "('')";

      sb.Append(")");

      return sb.ToString();
   }

   internal static string GetInSetClause_AsInt(IEnumerable<string> distinctWantedMemberList)
   {
      bool touched = false;

      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      sb.Append("(");

      foreach(string member in distinctWantedMemberList)
      {
         touched = true;
         sb.Append(/*"'" +*/ member + ", ");
      }

      //sb.Remove(sb.Length - suffix.Length, suffix.Length); // makni suffix sa zadnjega 
      if(touched) sb.Remove(sb.Length - 2, 1);
      else        return "('')";

      sb.Append(")");

      return sb.ToString();
   }

   public static string EventualRelatedTblName_ForWhereClause_FromFilterMembers(VvRptFilter filter)
   {
      
      if(filter.FilterMembers == null || filter.FilterMembers.Count.IsZero()) return ""; // news 21.10.2010 

      //System.Text.StringBuilder sb = new System.Text.StringBuilder();
      string sb = "";

      foreach(VvSqlFilterMember member in filter.FilterMembers)
      {
         if(member.isDummy) continue;

         if(member.relatedTable.NotEmpty())
         {
            switch(member.relatedTable)
            {
               case Kupdob.recordName:
                  if(!filter.FromClauseGot_Kupdob_TableName)
                  {
                     filter.FromClauseGot_Kupdob_TableName = true;
                     sb +=("\nJOIN " + Kupdob.recordName + " kpdb ON " + Ftrans.KupdobForeignKey + " = kpdb.kupdobCD \n");
                  }
                  break;

               case Kupdob.recordName + "_fak":
                  if(!filter.FromClauseGot_Kupdob_TableName)
                  {
                     string fakExPreffix = ZXC.ForceXtablePreffix ? "X" : "R";
                     filter.FromClauseGot_Kupdob_TableName = true;
                     sb += ("\nJOIN " + Kupdob.recordName + " kpdb ON " + fakExPreffix + "." + /*Ftrans.KupdobForeignKey*/"kupdobCD" + " = kpdb.kupdobCD \n");
                  }
                  break;

               case Kplan.recordName:
                  if(!filter.FromClauseGot_Kplan_TableName)
                  {
                     filter.FromClauseGot_Kplan_TableName = true;
                     sb +=("\nJOIN " + Kplan.recordName + " kpln ON " + Ftrans.KplanForeignKey + " = kpln.konto \n");
                  }
                  break;

               case Osred.recordName:
                  if(!filter.FromClauseGot_Osred_TableName)
                  {
                     filter.FromClauseGot_Osred_TableName = true;
                     sb +=("\n JOIN " + Osred.recordName       + " "   + GetShortRecordName(Osred.recordName) + 
                           "   ON "   + Atrans.OsredForeignKey + " = " + GetShortRecordName(Osred.recordName) + ".osredCD \n");
                  }
                  break;

               case Person.recordName:
                  if(!filter.FromClauseGot_Person_TableName)
                  {
                     filter.FromClauseGot_Person_TableName = true;
                     sb +=("\n JOIN " + Person.recordName       + " "   + GetShortRecordName(Person.recordName) + 
                           "   ON "   + Ptrans.recordName + "." + Ptrans.PersonForeignKey + 
                           " = "      + GetShortRecordName(Person.recordName) + ".personCD \n");
                  }
                  break;

               case Placa.recordName:
                  if(!filter.FromClauseGot_Placa_TableName)
                  {
                     filter.FromClauseGot_Placa_TableName = true;
                     sb +=("\n JOIN " + Placa .recordName       + " "   + GetShortRecordName(Placa.recordName) + 
                           "   ON "   + Ptrans.recordName + "." + Ptrans.PlacaForeignKey + 
                           " = "      + GetShortRecordName(Placa.recordName) + ".recID \n");
                  }
                  break;

               case /*Debit.recordName*/"debit":
                  if(!filter.FromClauseGot_Debit_TableName)
                  {
                     filter.FromClauseGot_Debit_TableName = true;
                     sb +=("\n JOIN " + "debit"  + " " + GetShortRecordName("debit") +
                           "   ON "   + "dtrans" + "." + "t_parentID" +
                           " = "      + GetShortRecordName("debit")   + ".recID \n");
                  }
                  break;

               //case Ptrano.recordName:
               //   if(!filter.FromClauseGot_Ptrano_TableName)
               //   {
               //      filter.FromClauseGot_Ptrano_TableName = true;
               //      sb +=("\n JOIN " + Ptrano.recordName       + " "   + GetShortRecordName(Ptrano.recordName) +
               //            " USING (" + Ptrans.PlacaForeignKey + ", " + Ptrans.PersonForeignKey + ")\n");
               //   }
               //   break;

               case ArtStat.recordName:
                  throw new Exception("tu si nesto prtljao pa si odustao");
                  //if(!filter.FromClauseGot_ArtStat_TableName)
                  //{
                  //   filter. FromClauseGot_ArtStat_TableName = true;
                  //   sb += ("\n LEFT JOIN " + 
                        
                  //      ArtStat.recordName +
                  //      //ArtiklDao.GetSubQueryText4FindArtikl() + 
                        
                  //      " "   + GetShortRecordName(ArtStat.recordName) +
                        
                  //      "   ON " + ArtStat.ArtiklForeignKey + " = " + Artikl.recordName + ".artiklCD \n");
                  //}
                  //break;

               case Faktur.recordName:
                  if(!filter.FromClauseGot_Faktur_TableName)
                  {
                     filter.FromClauseGot_Faktur_TableName = true;
                     sb +=("\n JOIN " + Faktur.recordName       + " "   + GetShortRecordName(Faktur.recordName) + 
                           "   ON "   + Rtrans.recordName + "." + Rtrans.FakturForeignKey + 
                           " = "      + GetShortRecordName(Faktur.recordName) + ".recID \n");
                  }
                  break;

               case Ftrans.recordName: // exclusively 4 FtransDao.GetDirektTroskoviProizvodnjeFtransList() 
                  if(!filter.FromClauseGot_Ftrans_TableName)
                  {
                     filter.FromClauseGot_Ftrans_TableName = true;

                     // JOIN ON ftrans.t_projektCD = faktur.TT + "-" + faktur.TtNum
                     sb +=("\n JOIN " + Ftrans.recordName + " " + GetShortRecordName(Ftrans.recordName) + 
                           "   ON CONCAT(L.TT, '-', L.TtNum)" + 
                           " = "      + GetShortRecordName(Ftrans.recordName) + ".t_projektCD \n");
                  }
                  break;

               default: 
                  ZXC.aim_emsg("Klipping: FilterMember.RelatedTable '{0}' still UNDONE!", member.relatedTable);
                  ZXC.aim_emsg("Ne zaboravi dodati i u: _theFilter.ClearAllFilters_FromClauseGot(); DON'T FORGET!!!");
                  break;
            }
         }
      }
      return sb/*.ToString()*/;
   }

   public static string EventualRelatedArtstat_ForWhereClause_FromFilterMembers(XSqlCommand cmd, ArtiklListUC artiklListUC)
   {
      DateTime dateDo;

      // 16.01.2013: 
    //     if(ZXC.TheVvForm.TheVvUC is FakturExtDUC) dateDo = ((FakturExtDUC)ZXC.TheVvForm.TheVvUC).Fld_SkladDate; 
    //else if(ZXC.TheVvForm.TheVvUC is FakturDUC   ) dateDo = ((FakturDUC)   ZXC.TheVvForm.TheVvUC).Fld_DokDate; 
    //else                                           dateDo = DateTime.MinValue;
      if(ZXC.TheVvForm.TheVvUC is FakturExtDUC) 
      {
         FakturExtDUC theDUC = (FakturExtDUC)ZXC.TheVvForm.TheVvUC;
         if(theDUC.CtrlOK(theDUC.tbx_SkladDate))     dateDo = ((FakturExtDUC)ZXC.TheVvForm.TheVvUC).Fld_SkladDate; 
         else                                        dateDo = ((FakturExtDUC)ZXC.TheVvForm.TheVvUC).Fld_DokDate  ; 
      }
      else if(ZXC.TheVvForm.TheVvUC is FakturDUC   ) dateDo = ((FakturDUC)   ZXC.TheVvForm.TheVvUC).Fld_DokDate; 
      else                                           dateDo = DateTime.MinValue;

      string sqlStr =

         "\nLEFT JOIN artstat " + VvSQL.GetShortRecordName(ArtStat.recordName) + " ON" + "\n" +

         VvSQL.GetShortRecordName(ArtStat.recordName) + ".recID = (" + "\n" +

         "SELECT RecID FROM artstat WHERE t_artiklCD = artiklCD" + "\n" +

         (artiklListUC.Fld_SituacijaZaSkladCD.NotEmpty() ? "AND t_skladCD     = '" + artiklListUC.Fld_SituacijaZaSkladCD + "' " : "") + "\n" +
         (dateDo != DateTime.MinValue                    ? "AND t_skladDate  <= ?prm_t_skladDate"                               : "") + "\n" +
         (artiklListUC.Fld_IsMinusEver                   ? "AND frsMinTtNum   > 0"                                              : "") + "\n" +

         "ORDER BY " + Rtrans.artiklOrderBy_DESC + "\n" +

         "LIMIT 1" + "\n" +

         ")" + "\n";

      if(dateDo != DateTime.MinValue) CreateCommandParameter(cmd, "prm_", dateDo, ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate]);

      if(artiklListUC.Fld_IsShowSomeOfStatusData)
      {
         return sqlStr;
      }

      foreach(VvSqlFilterMember member in artiklListUC.TheFilterMembers)
      {
         if(member.isDummy) continue;

         if(member.relatedTable.NotEmpty())
         {
            return sqlStr;
         }
      }

      return "";
   }
  
   #endregion Reports (WhereClauseFromVvRptFilter)

   #region GetGenericVvDataRecordList_Command

   internal static XSqlCommand Get_SELECTzvjezdica_AndJOIN_FROM_Command(XSqlConnection conn, VvDataRecord vvDataRecord, string selectWhat, List<VvSqlFilterMember> filterMembers, string orderByColumnNames, string anotherJoinClause, string groupByColumnNames)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT " + selectWhat + " FROM  " + vvDataRecord                        .VirtualRecordName + " L " + "\n" + 
                        "LEFT JOIN  "                      + vvDataRecord.VirtualExtenderRecord  .VirtualRecordName + " R " + "\n" + "\n" + 
                        "ON L.RecID = R." + ((IVvExtenderDataRecord)vvDataRecord.VirtualExtenderRecord).JoinedColName       + "\n" + 
                        
                        anotherJoinClause + 

                        (filterMembers == null ? "" : VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter)) +

                         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +

                        (orderByColumnNames.NotEmpty() ? "ORDER BY " + orderByColumnNames : "") +

                        // 24.02.2017: 
                        (groupByColumnNames.NotEmpty() ? " GROUP BY " + groupByColumnNames : "");

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static List<VvSqlFilterMember> CleanFilterMembers4_IRMs(List<VvSqlFilterMember> filterMembers)
   {
      //List<VvSqlFilterMember> filterMembers4_IRMs = filterMembers.Where(fm => fm.name.StartsWith("PdvFtr") == false).ToList();

      //filterMembers4_IRMs.ForEach(fm => fm.or_state = ZXC.FM_OR_Enum.NONE); ... ovo nece, or_state ostane isti pa moras ovako preko delegate-a: 

      //filterMembers4_IRMs.ForEach(delegate(VvSqlFilterMember fm) ... MA NITI OVO NECE! 
      //{
      //   fm.or_state = ZXC.FM_OR_Enum.NONE;
      //});

      //for(int i = 0; i < filterMembers4_IRMs.Count; ++i) filterMembers4_IRMs[i].or_state = ZXC.FM_OR_Enum.NONE;

      // je'ote nemres bolivit; sve ovo gore NE RADI nego morqs ovako: ... u biti kad malo razmislim mozda ne radi jer imas List-u struct-a kao value type a ne reference type pa onda neda. 
      List<VvSqlFilterMember> filterMembers4_IRMs = new List<VvSqlFilterMember>();
      VvSqlFilterMember fm4irm;

      foreach(VvSqlFilterMember origFM in filterMembers.Where(fm => fm.name.StartsWith("PdvFtr") == false))
      {
         fm4irm = origFM;
         fm4irm.or_state = ZXC.FM_OR_Enum.NONE;
         filterMembers4_IRMs.Add(fm4irm);
      }

      return filterMembers4_IRMs;
   }

   // 02.02.2016: 
 //internal static string JoinClauseFor_R2_fakturs { get { return "LEFT JOIN ftrans N ON (L.RecID = N.t_fakRecID                                       AND pdvR12 = 2 AND N.t_otsKind = 2 AND t_dokDate >= ?filter_PdvFtrDateOD AND t_dokDate <= ?filter_PdvFtrDateDO)\n\n"; } }
   // 04.02.2016:                                                                                                                   
 //internal static string JoinClauseFor_R2_fakturs { get { return "LEFT JOIN ftrans N ON (L.RecID = N.t_fakRecID                    AND N.t_tt != 'PS' AND pdvR12 = 2 AND N.t_otsKind = 2 AND t_dokDate >= ?filter_PdvFtrDateOD AND t_dokDate <= ?filter_PdvFtrDateDO)\n\n"; } }
   internal static string JoinClauseFor_R2_fakturs { get { return "LEFT JOIN ftrans N ON (L.RecID = N.t_fakRecID AND N.t_tt != 'OK' AND N.t_tt != 'PS' AND pdvR12 = 2 AND N.t_otsKind = 2 AND t_dokDate >= ?filter_PdvFtrDateOD AND t_dokDate <= ?filter_PdvFtrDateDO)\n\n"; } }

   internal static XSqlCommand LoadIraUnionIrmGroupedFakturList_Command(XSqlConnection conn, string selectWhat, List<VvSqlFilterMember> filterMembers)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      #region Clean filterMembers4_IRMs

      List<VvSqlFilterMember> filterMembers4_IRMs = CleanFilterMembers4_IRMs(filterMembers);

      #endregion Clean filterMembers4_IRMs

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "( \n" +
                        "SELECT " + selectWhat.Replace("COUNT(*)", "1").Replace("SUM", "")/*.Replace(")", "")*/ + " FROM  faktur L      \n" + 
                        "LEFT JOIN                                    faktEx R    \n\n" + 
                        "ON L.RecID = R.fakturRecID \n\n" +
                        JoinClauseFor_R2_fakturs          +
                        // 06.09.2018: 
                      //"WHERE  tt IN " + TtInfo.IzlazniPdv_IN_Clause.Replace("'IRM', ", "") + "\n" +
                        "WHERE (tt IN " + TtInfo.IzlazniPdv_IN_Clause.Replace("'IRM', ", "") + " OR (tt = 'IRM' AND UPPER(SUBSTRING(nacPlac,1,6)) = 'VIRMAN'))\n" +

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true) +
                        ")     \n\n" +
         
                        // 22.11.2012:
                      //"UNION \n\n" +
                        "UNION ALL \n\n" +

                        "(       \n" +
                        "SELECT " +
                        selectWhat.Replace("L.recID", "0").Replace("recID", "dummy").Replace("* " + FakturDao.ftrUplata + " / s_ukKCRMP", "")/*.Replace("" + ftrUplata + "", "0").Replace("r2_uplata", "dummy2")*/
                        .Replace("N.t_dokDate", "pdvDate")
                        + 
                        " FROM  faktur L      \n" +
                        "LEFT JOIN                       faktEx R    \n\n" +
                        "ON L.RecID = R.fakturRecID \n\n" +
                        // 06.09.2018: 
                      //"WHERE tt = 'IRM'                                              \n" +
                        "WHERE tt = 'IRM' AND UPPER(SUBSTRING(nacPlac,1,6)) != 'VIRMAN'\n" +

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers4_IRMs, true) +
                        "GROUP BY DATE(pdvDate)            \n" +
                        ")     \n\n" +

                        "ORDER BY dokDate, ttSort, ttNum \n"; //!!! Dakle, PDV knjiga IRA se za razliku od URA soritra po dokDate, tt, ttNum  (VIDI i RptR_PDV_Knjiga.FillRiskReportLists) 
                        //"ORDER BY recID \n"; 

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand Get_PdvIraSUM_Command(XSqlConnection conn, string selectWhat, List<VvSqlFilterMember> filterMembers, bool needsLineCount)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      List<VvSqlFilterMember> filterMembers4COUNT = new List<VvSqlFilterMember>(filterMembers);
      // ovdje sad treba izbiti filterMember 'tt IN ('IRM', 'IRA', 'IFA', 'IOD', 'IPV')'
      filterMembers4COUNT.RemoveAt(0);

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT " + selectWhat.Replace("COUNT(*)", "").Replace("as s_lineCount,", "").Replace("L.recID", "").Replace("as recID", "") + "\n" +

         #region Count IRA analitic and IRM grouped lines - double subQuery

         (!needsLineCount ? "0 AS s_lineCount\n" :

         "  (                                             \n" +
         "    SELECT                                      \n" +
       //"    # number of IRA,IFA,IOD,IPV lines           \n" +
         "    COUNT(*) +                                  \n" +
         "                                                \n" +
         "    (                                           \n" +
         "      SELECT COUNT(DISTINCT pdvDate)            \n" +
       //"      # number of IRM groupped by pdvDate lines \n" +
         "      FROM        faktur L                      \n" +
         "      LEFT  JOIN  faktEx R                      \n" +
         "                                                \n" +
         "      ON L.RecID = R.fakturRecID                \n" +
         "                                                \n" +
         "      WHERE tt = 'IRM'                          \n" +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(CleanFilterMembers4_IRMs(filterMembers4COUNT), true) +

         "    )                                           \n" +
         "                                                \n" +
         "    FROM       faktur L                         \n" +
         "    LEFT JOIN  faktEx R                         \n" +
         "                                                \n" +
         "    ON L.RecID = R.fakturRecID                  \n" +
         "                                                \n" +
         JoinClauseFor_R2_fakturs                             +
         "                                                \n" +
         // 18.03.2016: tek! 
       //"    WHERE tt IN ('IRA', 'IFA', 'IOD', 'IPV'       )    \n" +
         "    WHERE tt IN ('IRA', 'IFA', 'IOD', 'IPV', 'YRN')    \n" +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers4COUNT, true) +

         "  ) AS s_lineCount                              \n") +

         #endregion Count IRA analitic and IRM grouped lines - double subQuery

                        "FROM      faktur L           \n" +
                        "LEFT JOIN faktEx R         \n\n" +
                        "ON L.RecID = R.fakturRecID \n\n" +
                        JoinClauseFor_R2_fakturs          +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetPdvKnjigaRbrForThisFaktur_Command(XSqlConnection conn, Faktur faktur_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      if(faktur_rec.TtInfo.IsUlazniPdvTT)
      {
         cmd.CommandText = "SELECT COUNT(*) FROM  faktur L LEFT JOIN  faktEx R ON L.RecID = R.fakturRecID \n" +
                           "WHERE tt IN "     + TtInfo.UlazniPdv_IN_Clause + "\n" + 
                           "AND pdvKnjiga = " + faktur_rec.PdvKnjiga_u     + "\n" + 
                           "AND L.recID  <= " + faktur_rec.RecID;
      }
      else
      {
         cmd.CommandText = "SELECT COUNT(*) + \n" +
                           // subQuery for IRM grouped 
                           "(                                                                 \n" +
                           "      SELECT COUNT(DISTINCT pdvDate)                              \n" +
                           "      FROM faktur L LEFT JOIN faktEx R ON L.RecID = R.fakturRecID \n" +
                           "      WHERE tt = 'IRM'                                            \n" +
                           "      AND pdvKnjiga = " + faktur_rec.PdvKnjiga_u + "              \n" +
                           "      AND                                                         \n" +
                           "      (                                                           \n" +
                           "      (dokDate < ?prm_dokDate) OR (dokDate = ?prm_dokDate && ttSort < ?prm_ttSort) OR (dokDate = ?prm_dokDate && ttSort = ?prm_ttSort && ttNum <= ?prm_ttNum)\n" + 
                           "      )                                                           \n" +
                           ")                                                                 \n" +

                           "FROM  faktur L LEFT JOIN  faktEx R ON L.RecID = R.fakturRecID \n" +
                           "WHERE tt IN "     + TtInfo.IzlazniPdv_IN_Clause.Replace("'IRM', ", "") + "\n" +
                           "AND pdvKnjiga = " + faktur_rec.PdvKnjiga_u + "\n" +
                           "AND \n" +
                           "(   \n" +
                           "(dokDate < ?prm_dokDate) OR (dokDate = ?prm_dokDate && ttSort < ?prm_ttSort) OR (dokDate = ?prm_dokDate && ttSort = ?prm_ttSort && ttNum <= ?prm_ttNum)\n" + 
                           ")   \n";

         CreateCommandParameter(cmd, "prm_", faktur_rec.DokDate, ZXC.FakturSchemaRows[ZXC.FakCI.dokDate]);
         CreateCommandParameter(cmd, "prm_", faktur_rec.TtSort , ZXC.FakturSchemaRows[ZXC.FakCI.ttSort ]);
         CreateCommandParameter(cmd, "prm_", faktur_rec.TtNum  , ZXC.FakturSchemaRows[ZXC.FakCI.ttNum  ]); 
      }

      return (cmd);
   }

   internal static XSqlCommand FakturExistsFor_Sklad_And_TT_And_Date_Command(XSqlConnection conn, string _skladCD, short _ttSort, DateTime _dokDate)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT COUNT(*) FROM  faktur     \n" +
                        "WHERE      ttSort   =      ?prm_ttSort  " + "\n" +
                        "AND        skladCD  =      ?prm_skladCD " + "\n" +
                        "AND   DATE(dokDate) = DATE(?prm_dokDate)" + "\n" ; // DATE() funkcija dodana tek 28.12.2017. 

      CreateCommandParameter(cmd, "prm_", _dokDate, ZXC.FakturSchemaRows[ZXC.FakCI.dokDate]);
      CreateCommandParameter(cmd, "prm_", _ttSort , ZXC.FakturSchemaRows[ZXC.FakCI.ttSort ]);
      CreateCommandParameter(cmd, "prm_", _skladCD, ZXC.FakturSchemaRows[ZXC.FakCI.skladCD]);

      return (cmd);
   }

   internal static XSqlCommand SetMeFaktur_BySklad_And_TT_And_Date_Command(XSqlConnection conn, string _skladCD, short _ttSort, DateTime _dokDate)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT       *  FROM  faktur     \n" +
                        "WHERE ttSort  = ?prm_ttSort"  + "\n" +
                        "AND   skladCD = ?prm_skladCD" + "\n" +
                        "AND   dokDate = ?prm_dokDate" + "\n" ;

      CreateCommandParameter(cmd, "prm_", _dokDate, ZXC.FakturSchemaRows[ZXC.FakCI.dokDate]);
      CreateCommandParameter(cmd, "prm_", _ttSort , ZXC.FakturSchemaRows[ZXC.FakCI.ttSort ]);
      CreateCommandParameter(cmd, "prm_", _skladCD, ZXC.FakturSchemaRows[ZXC.FakCI.skladCD]);

      return (cmd);
   }

   internal static XSqlCommand GetFaktursRtransWithArtstatList_Command(XSqlConnection conn, string anotherJoinClause, List<VvSqlFilterMember> filterMembers, string orderByColumnNames/*, uint limitOffset, uint limitRowCount*/)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT R.*, A.* FROM  faktur L \n" +
                        "LEFT  JOIN  faktEx  X ON L.RecID = X.fakturRecID \n" +
         /* RIGHT JOIN,odervajs*/"RIGHT JOIN  rtrans  R ON L.RecID = R.t_parentID  \n" +
         /* je null exc. kada  */"LEFT  JOIN  artstat A ON R.RecID = A.rtransRecID \n" +
         /* fak nema stavaka   */
                        (filterMembers == null ? "" : VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter)) +

                         anotherJoinClause + // kocnica za vec prebacene dane ili mjesece (dakle da ne moze dva puta prenijeti)

                         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +

                        (orderByColumnNames.NotEmpty() ? " ORDER BY " + orderByColumnNames : "") + "\n" /*+

                        "LIMIT " + limitOffset + ", " + limitRowCount*/;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand Get_UnusefulZPC_RtransList_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, string orderByColumnNames)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT * FROM  rtrans \n" +
                         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) + "\n" +
                        " ORDER BY " + orderByColumnNames;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand LoadManyDocumentsTtranses_Command(XSqlConnection conn, string documTableName, string transTableName, VvRptFilter rptFilter, string orderByColumnNames)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT R.*  FROM  " + documTableName + " L \n" +
/* RIGHT JOIN,odervajs*/"RIGHT JOIN        " + transTableName + " R ON L.RecID = R.t_parentID  \n" + 
/* je null exc. kada document nema stavaka   */

                        // 26.10.2020: Zvonko?
                        // zbog eventualne potrebe kupdobCD-a u filteru dodajemo i FaktEx
                        (documTableName == Faktur.recordName ? "LEFT  JOIN  faktEx  X ON L.RecID = X.fakturRecID \n" : "\n" ) +

                        (rptFilter.FilterMembers == null ? "" : VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(rptFilter)) +

                         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(rptFilter.FilterMembers, false) +

                        (orderByColumnNames.NotEmpty() ? " ORDER BY " + orderByColumnNames : "");

      VvSQL.SetReportCommandParamValues(cmd, rptFilter.FilterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetArtiklWithArtstatList_Command(XSqlConnection conn, string _skladCD, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter, /*fuse*/ string artiklColumns, string orderByColumnNames)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT ART.*, A.* FROM  artikl  ART \n" +
                        "LEFT  JOIN              artstat A   ON A.RecID = ( \n" +

                        "SELECT RecID FROM artstat              \n" +
                        "WHERE t_artiklCD   = artiklCD          \n" +
                        "AND   t_skladCD    = ?prm_AS_t_skladCD \n" +
                        (RptFilter.SviArtikli == false ? "AND   t_skladDate <= ?prm_AS_t_skladDate \n" : "") + "\n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + "\n" +

                        "LIMIT 1" + "\n" +
                        ")" + "\n" +

                        (RptFilter.SviArtikli == false ? "WHERE A.recID IS NOT NULL \n" : "") + "\n" +
                        // 19.02.2016: 
                      //"AND ART.TS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK        ArtiklTS_array) + " \n" +
                        "AND ART.TS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK_or_UDP_ArtiklTS_array) + " \n" +

      VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
 
      VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, /*RptFilter.SviArtikli == false*/ true) +

      (orderByColumnNames.NotEmpty() ? " ORDER BY " + orderByColumnNames : "");

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      CreateCommandParameter(cmd, "prm_AS_", /*RptFilter.SkladCD*/ _skladCD, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladCD  ]); // kada ja za all_skald onda ide petlja po svakom skladistu zasebno a ne 'RptFilter.SkladCD' 

      // 16.09.2016: nemoj skladDate 
      if(RptFilter.SviArtikli) return (cmd);

      CreateCommandParameter(cmd, "prm_AS_", /*RptFilter.DatumDo*/ _dateDo, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladDate]);
      return (cmd);
   }

   internal static XSqlCommand GetArtstat_SUM_list_Command(XSqlConnection conn, bool isPrmRazdoblja, string _skladCD, DateTime _dateOd, DateTime _dateDo, VvRpt_RiSk_Filter RptFilter, string ulazShadowTT_IN_Clause, string izlazShadowTT_IN_Clause, string uraPovratShadowTT_IN_Clause, bool isForceMPSK_by_NBC)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT t_tt,                                                \n" +
         "t_skladCD,                                                  \n" +
         "SUM(rtrPstKol   ), SUM(rtrUlazKol   ), SUM(rtrIzlazKol   ), \n" +
         "SUM(rtrPstVrjNBC), SUM(rtrUlazVrjNBC), SUM(rtrIzlazVrjNBC), \n" +
         "SUM(rtrPstVrjMPC), SUM(rtrUlazVrjMPC), SUM(rtrIzlazVrjMPC)  \n" +

         "FROM artstat                                                \n" +

         "WHERE   t_skladCD  = ?prm_AS_t_skladCD                      \n" +
         "AND   t_skladDate <= ?prm_AS_t_skladDate                    \n" +
         "AND artiklTS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK_or_UDP_ArtiklTS_array) + " \n" +

         (isPrmRazdoblja ? "AND t_skladDate >= ?prm_theDateOd " : "") + " \n" +

         (RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_LJEK ? "AND (artGrCd1  = '90' || artGrCd1  = 'A0' || artGrCd1  = 'N0' || artGrCd1  = '10')" : "") + " \n" +
         (RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_POTR ? "AND (artGrCd1 != '90' && artGrCd1 != 'A0' && artGrCd1 != 'N0' && artGrCd1 != '10')" : "") + " \n" +

         "GROUP BY t_tt                                               \n" +

         (
         isForceMPSK_by_NBC ? "" : // ako je isForceMPSK_by_NBC, tada NE trebamo UNION za implicitne nivelacije

         // Observacija od 24.05.2024: ova ovdje NUP logika se razlikuje od NUP logike u 'SumFromRtrans'          
         // pa je moguca pojava crvene brojke razlike u Bilanci skladista kada 'isThisDirektStornoRtrans' postoji 
         // uraPovratShadowTT_IN_Clause 
         "UNION                                                                                                     \n" +
         "SELECT 'NUP', t_skladCD AS SkladCD, 0, 0, 0, 0, SUM(((pstKol+ulazKol-izlazKol)-(rtrUlazKol))*(rtrCijenaNBC-prNBCBefThisUlaz)), 0, 0, 0, 0 \n" +
         "FROM artstat a                                                                                            \n" +
         "WHERE   t_skladCD  = ?prm_AS_t_skladCD                                                                    \n" +
         "AND   t_skladDate <= ?prm_AS_t_skladDate                                                                  \n" +

         // tek od 07.10.2024: 
         "AND artiklTS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK_or_UDP_ArtiklTS_array)                   + " \n" +

 /*!!!*/ "AND   rtrUlazKol  < 0                                                                                     \n" + // !!!
         "AND t_tt IN " + uraPovratShadowTT_IN_Clause +                                                            "\n" +
         (isPrmRazdoblja ? "AND t_skladDate >= ?prm_theDateOd " : "")                                           + " \n" +

         (RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_LJEK ? "AND (artGrCd1  = '90' || artGrCd1  = 'A0' || artGrCd1  = 'N0' || artGrCd1  = '10')" : "") + " \n" +
         (RptFilter.SVD_LiP == ZXC.PdvZPkindEnum.SVD_POTR ? "AND (artGrCd1 != '90' && artGrCd1 != 'A0' && artGrCd1 != 'N0' && artGrCd1 != '10')" : "") + " \n" +

         "GROUP BY t_skladCD                                                                                        \n" +

         // ulazShadowTT_IN_Clause 
         "UNION                                                                                                     \n" +
         "SELECT 'NUV', t_skladCD AS SkladCD, 0, 0, 0, 0, 0, 0, 0, SUM(((pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol))*(rtrCijenaMPC-prevMalopCij)), 0 \n" +
         "FROM artstat a                                                                                            \n" +
         "WHERE   t_skladCD  = ?prm_AS_t_skladCD                                                                    \n" +
         "AND   t_skladDate <= ?prm_AS_t_skladDate                                                                  \n" +

         // tek od 07.10.2024: 
         "AND artiklTS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK_or_UDP_ArtiklTS_array)                   + " \n" +

         "AND t_tt IN " + ulazShadowTT_IN_Clause +                                                                 "\n" +
         (isPrmRazdoblja ? "AND t_skladDate >= ?prm_theDateOd " : "")                                           + " \n" +
         "GROUP BY t_skladCD                                                                                        \n" +

         // izlazShadowTT_IN_Clause 
         "UNION                                                                                                     \n" +
         "SELECT 'NIV', t_skladCD AS SkladCD, 0, 0, 0, 0, 0, 0, 0, 0, - SUM(rtrIzlazKol*(rtrCijenaMPC-prevMalopCij))\n" + // ! pazi na ovaj * -1 predznak 
         "FROM artstat a                                                                                            \n" +
         "WHERE   t_skladCD  = ?prm_AS_t_skladCD                                                                    \n" +
         "AND   t_skladDate <= ?prm_AS_t_skladDate                                                                  \n" +

         // tek od 07.10.2024: 
         "AND artiklTS NOT IN" + TtInfo.GetSql_IN_Clause(ZXC.IsMinusOK_or_UDP_ArtiklTS_array)                   + " \n" +

         "AND t_tt IN " + izlazShadowTT_IN_Clause +                                                                "\n" +
         (isPrmRazdoblja ? "AND t_skladDate >= ?prm_theDateOd " : "")                                           + " \n" +
         "GROUP BY t_skladCD                                                                                        \n");


      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      CreateCommandParameter(cmd, "prm_AS_", /*RptFilter.SkladCD*/ _skladCD, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladCD  ]); // kada ja za all_skald onda ide petlja po svakom skladistu zasebno a ne 'RptFilter.SkladCD' 
      CreateCommandParameter(cmd, "prm_AS_", /*RptFilter.DatumDo*/ _dateDo , ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladDate]);

      CreateCommandParameter(cmd, "theDateOd", /*RptFilter.DatumDo*/ _dateOd , XSqlDbType.Date, 32);

      return (cmd);
   }

   internal static XSqlCommand GetOsredWithOsrstatList_Command(XSqlConnection conn, DateTime _dateDo, VvRpt_Osred_Filter RptFilter, DateTime _dateAmRazdobljeStart, string orderByColumnNames)
   {
      XSqlCommand cmd = InitCommand(conn);

      bool weCareAboutIsRashodovan = RptFilter.IsRashodSignif;

      cmd.CommandText = "SELECT O.*, t_osredCD, \n" +
     
              VvOsredReport.Rule_IsRashodovan +
     
              "SUM" + VvOsredReport.Rule_InvSt +
              "SUM" + VvOsredReport.Rule_KolSt +
              "SUM" + VvOsredReport.Rule_UkNabDug +
              "SUM" + VvOsredReport.Rule_UkNabPot +
              "SUM" + VvOsredReport.Rule_UkRasDug +
              "SUM" + VvOsredReport.Rule_UkRasPot +
              "SUM" + VvOsredReport.Rule_OldAmDug +
              "SUM" + VvOsredReport.Rule_OldAmPot +
              "SUM" + VvOsredReport.Rule_NewAmDug +
              "SUM" + VvOsredReport.Rule_NewAmPot +

              ", CAST(MIN(IF(t_tt = 'NB', t_dokDate, '2999-12-31')) AS DATE) AS DateNabava \n" +
              ", CAST(MIN(IF(t_tt = 'RS', t_dokDate, '2999-12-31')) AS DATE) AS DateRashod \n" + 

             //", (SELECT MIN(t_dokDate) FROM atrans WHERE t_tt = 'NB') AS DateNabava, \n" + 
             //"  (SELECT MIN(t_dokDate) FROM atrans WHERE t_tt = 'RS') AS DateRashod  \n" +

              "FROM  osred O LEFT  JOIN atrans ON O.osredCD = t_osredCD \n" +

      VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter) +
 
      VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, false) +

      (orderByColumnNames.NotEmpty() ? " GROUP BY " + orderByColumnNames + " " : "") +

      (weCareAboutIsRashodovan ? VvOsredReport.Rule_HavingAdequateRashodState(RptFilter.IsRashodovan) + " OR KolSt != 0 " : "");

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      // jos jemput rucno za parametar '?dateStartAm' 
      DataRow drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_dokDate];
      CreateCommandNamedParameter(cmd, "filter_", "dateStartAm", _dateAmRazdobljeStart, drSchema);

      return (cmd);
   }

   internal static XSqlCommand GetKretanjeSkladList_Command(XSqlConnection conn, VvRpt_RiSk_Filter RptFilter, string ulazClause, string izlazClause)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT       \n" +
                        "t_skladDate, \n" +
                        "SUM(" + ulazClause  + ") AS ULAZ, \n" +
                        "SUM(" + izlazClause + ") AS IZLAZ \n" +

                        "FROM      artstat A        \n" +
/* Upali ako zatreba*///"LEFT JOIN artikl S         \n" +
                      //"ON S.artiklCD = t_artiklCD \n" +

                        "WHERE t_skladCD    = ?prm_AS_t_skladCD   \n" +
                        "AND   t_skladDate <= ?prm_AS_t_skladDate \n" +

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, /*RptFilter.SviArtikli == false*/ true) +
                        
                        "GROUP BY DATE(t_skladDate) \n" +
                        "ORDER BY " + Rtrans.artiklOrderBy_ASC;

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      CreateCommandParameter(cmd, "prm_AS_", RptFilter.SkladCD/* _skladCD*/, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladCD]); // kada ja za all_skald onda ide petlja po svakom skladistu zasebno a ne 'RptFilter.SkladCD' 
      CreateCommandParameter(cmd, "prm_AS_", RptFilter.DatumDo/* _dateDo */, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladDate]); // vidi gore kod GetArtiklWithArtstatList_Command 

      return (cmd);
   }

   internal static XSqlCommand GetStanjeSklPoRNPList_Command(XSqlConnection conn, VvRpt_RiSk_Filter RptFilter, string ulazTTlist, string izlazTTlist, string unionTTlist)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT L.ProjektCD, R.t_artiklCD,\n" +
                        
                        "SUM(IF(R.t_tt IN " + ulazTTlist  + ", R.t_kol, 0)) AS ulazKol  ,\n" +
                        "SUM(IF(R.t_tt IN " + izlazTTlist + ", R.t_kol, 0)) AS izlazKol  \n" +

                        "FROM        faktur  L                            \n" +
                        "LEFT  JOIN  faktEx  X ON L.RecID = X.fakturRecID \n" +
                        "RIGHT JOIN  rtrans  R ON L.RecID = R.t_parentID  \n" +
                        "LEFT  JOIN  artstat A ON R.RecID = A.rtransRecID \n" +

                        "WHERE R.t_skladCD    = ?prm_AS_t_skladCD   \n" +
                        "AND   R.t_skladDate <= ?prm_AS_t_skladDate \n" +
                        "AND   L.projektCD  != ''                   \n" +
                        "AND   R.t_artiklCD != ''                   \n" +
                        "AND   A.artiklTS NOT IN " + TtInfo.GetSql_IN_Clause(ZXC.NoNeedForZaliha_ArtiklTSs) + "\n" +
                        "AND   R.t_tt         IN " + unionTTlist                                            + "\n" +


                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, /*RptFilter.SviArtikli == false*/ true) +
                        
                        "GROUP BY L.ProjektCD, R.t_artiklCD \n";

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      CreateCommandParameter(cmd, "prm_AS_", RptFilter.SkladCD/* _skladCD*/, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladCD]); // kada ja za all_skald onda ide petlja po svakom skladistu zasebno a ne 'RptFilter.SkladCD' 
      CreateCommandParameter(cmd, "prm_AS_", RptFilter.DatumDo/* _dateDo */, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladDate]); // vidi gore kod GetArtiklWithArtstatList_Command 

      return (cmd);
   }

   internal static XSqlCommand GetStanjeReversaList_Command(XSqlConnection conn, VvRpt_RiSk_Filter RptFilter, string ulazTTlist, string izlazTTlist, string unionTTlist)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT X.KupDobCD, R.t_artiklCD,\n" +
                        
                        "SUM(IF(R.t_tt IN " + ulazTTlist  + ", R.t_kol, 0)) AS ulazKol,\n" +
                        "SUM(IF(R.t_tt IN " + izlazTTlist + ", R.t_kol, 0)) AS izlazKol\n" +

                        "FROM        faktur  L                            \n" +
                        "LEFT  JOIN  faktEx  X ON L.RecID = X.fakturRecID \n" +
                        "RIGHT JOIN  rtrans  R ON L.RecID = R.t_parentID  \n" +
                        "LEFT  JOIN  artstat A ON R.RecID = A.rtransRecID \n" +

                        "WHERE R.t_skladCD    = ?prm_AS_t_skladCD   \n" +
                        "AND   R.t_skladDate <= ?prm_AS_t_skladDate \n" +
                        "AND   R.t_artiklCD != ''                   \n" +
                        "AND   A.artiklTS NOT IN " + TtInfo.GetSql_IN_Clause(ZXC.NoNeedForZaliha_ArtiklTSs) + "\n" +
                        "AND   R.t_tt         IN " + unionTTlist                                            + "\n" +


                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(RptFilter.FilterMembers, /*RptFilter.SviArtikli == false*/ true) +

                        "GROUP BY X.KupdobCD, R.t_artiklCD \n";

      VvSQL.SetReportCommandParamValues(cmd, RptFilter.FilterMembers);

      CreateCommandParameter(cmd, "prm_AS_", RptFilter.SkladCD/* _skladCD*/, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladCD]); // kada ja za all_skald onda ide petlja po svakom skladistu zasebno a ne 'RptFilter.SkladCD' 
      CreateCommandParameter(cmd, "prm_AS_", RptFilter.DatumDo/* _dateDo */, ZXC.ArtStatSchemaRows[ZXC.AstCI.t_skladDate]); // vidi gore kod GetArtiklWithArtstatList_Command 

      return (cmd);
   }

   public   static XSqlCommand Get_SELECTzvjezdicaFROM_Command(XSqlConnection conn, string recordName, string eventualTblNameSuffix, List<VvSqlFilterMember> filterMembers, string orderByColumnNames, string anotherJoinClause, string groupByColumnNames)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      string selectWhat;
      if(anotherJoinClause.IsEmpty() || eventualTblNameSuffix.IsEmpty()) selectWhat = "*";
      else                                                               selectWhat = eventualTblNameSuffix + ".*";

      cmd.CommandText = "SELECT " + selectWhat + " FROM " + recordName + " " + eventualTblNameSuffix + " " +

                        (filterMembers == null ? "" : VvSQL.EventualRelatedTblName_ForWhereClause_FromFilterMembers(RptFilter)) +

                        anotherJoinClause + // npr za ORDER BY po kupdob.naziv-u a kojega nema u ftrans-u (dakle, linked table ne treba za WHERE nego za ORDER BY. Da je za WHERE onda bi to islo preko filterMembers-a 

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +

                        (orderByColumnNames.NotEmpty() ? " ORDER BY " + orderByColumnNames : "") +

                        // 26.05.2015: 
                        (groupByColumnNames.NotEmpty() ? " GROUP BY " + groupByColumnNames : "");

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetDistinctKupdobCDInWantedSet_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT DISTINCT t_kupdob_cd from ftrans ftr \nLEFT JOIN     kupdob k ON ftr.t_kupdob_cd = k.kupdobCD \nLEFT JOIN faktur L ON ftr.t_fakRecID = L.RecID\nLEFT JOIN faktEx R ON ftr.t_fakRecID = R.fakturRecID\n\n" +

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetWantedSetWithKontraSet_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, string wantedKontoSet, string kontraKontoSet, string orderBy, string kupdobListForKontra)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT ftr.* FROM ftrans ftr \nLEFT JOIN     kupdob k ON ftr.t_kupdob_cd = k.kupdobCD \nLEFT JOIN faktur L ON ftr.t_fakRecID = L.RecID\nLEFT JOIN faktEx R ON ftr.t_fakRecID = R.fakturRecID\n\n" +

                        "WHERE                                                      " + "\n" +
                        "(                                                          " + "\n" +
                        "   SUBSTRING(ftr.t_konto, 1, 3) IN (" + wantedKontoSet + ")" + "\n" +
                        "                                                           " + "\n" +
                        "   OR                                                      " + "\n" +
                        "   (                                                       " + "\n" +
                        "      SUBSTRING(ftr.t_konto, 1, 3) IN (" + kontraKontoSet + ")" + "\n" +
                        "      AND                                                  " + "\n" +
                        "      t_kupdob_cd IN " + kupdobListForKontra                 + "\n" +
                        "   )                                                       " + "\n" +
                        ")                                                          " + "\n" +

                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true) +

                        "ORDER BY " + orderBy;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetTableLastChangeTimestamp_Command(XSqlConnection conn, string tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT MAX(logTS) FROM " + ZXC.vvDB_LANlogTableName + 
                        " WHERE record = '" + tableName + "' ";

      return (cmd);
   }

   internal static XSqlCommand CheckTtNum_Slijednost_Command(XSqlConnection conn, string skladCD, string wantedTT, bool isDateXDateIzd)
   {
      XSqlCommand cmd = InitCommand(conn);

      string theDateName = "dokDate";
      
      if(isDateXDateIzd) theDateName = "dateX";

      List<string> OPPskladCDlist = ZXC.luiListaSkladista.GetOPPskladCDlist(skladCD, wantedTT);
      bool isMultiSklad = OPPskladCDlist != null && OPPskladCDlist.Count > 1;

      string nowYear = ZXC.NowYearFirstDay.Year.ToString();

      cmd.CommandText = "SELECT         \n" +
                        theDateName + ", ttNum \n" +

                        "FROM      faktur L \n" +
                        "LEFT JOIN faktEx R \n\n" +
                        "ON L.RecID = R.fakturRecID \n\n" +

                        "WHERE tt      = '" + wantedTT + "'\n" +
                        "AND skladCD " + (isMultiSklad ? " IN " + GetInSetClause(OPPskladCDlist) : 
                                                         " = '" + skladCD + "' ") + "\n\n" +

                        // 18.02.2025: 
                        (ZXC.TtInfo(wantedTT).IsPTG_YYinTtNum_99999 ? "AND YEAR(DokDate) = " + nowYear + "\n\n" : "") +

                        "ORDER BY " + theDateName + ", ttNum\n"; // u biti ovaj ORDER BY ti niti ne treba, jer kasnije presortiravas 
      return (cmd);
   }

   internal static XSqlCommand GetPrihodTT_Skladista_InUse_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      string IN_clause = TtInfo.Prihod_IN_Clause;

      cmd.CommandText = "SELECT TT, SkladCD FROM faktur F  \n" +
                        "WHERE TT IN " + IN_clause      + "\n" +
                        "GROUP BY TT, SkladCD              \n" ; 

      return (cmd);
   }

   #region KPM & Fak2Nal MALOP

   #region Common SELECT-JOIN-SUBQUERY strings

   private const string faktur_JOIN_faktEx_clause =
         "     FROM  faktur  F                                                                                                                   \n" +
         "LEFT JOIN  faktEx  X ON F.RecID = X.fakturRecID                                                                                        \n" ;

   private const string faktur_JOIN_faktEx_JOIN_artstat_clause =
         "     FROM  faktur  F                                                                                                                   \n" +
         "LEFT JOIN  faktEx  X ON F.RecID = X.fakturRecID                                                                                        \n" +
         "LEFT JOIN  artstat A ON F.RecID = A.rtrParentID                                                                                        \n";

#if OldKPMversion
   private const string malop_RtransArtstat_subquery_ULAZ =
         // BUG!!!!!!!!!!!!!!!!! VIDI http://wikido.isoftdata.com/index.php/The_GROUPing_pitfall
         // How do you make sure that your aggregate functions return the data you want? Well, until MySQL starts reading minds (7.0?), 
         // you make sure that only one of the data sources in your query has a one-to-many relationship with your result. 
         //"     FROM  faktur  F                           \n" +
         //"LEFT JOIN  faktEx  X ON F.RecID = X.fakturRecID\n" +
         //"LEFT JOIN  rtrans  R ON F.RecID = R.t_parentID \n" +
         //"LEFT JOIN  artstat A ON R.RecID = A.rtransRecID\n" ;
         "LEFT JOIN(                                                                                                                              \n" +
         "  SELECT R.t_parentID,                                                                                                                  \n" +
         "  SUM(IF(t_pdvSt =  0, (IF(R.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj00, \n" +
         "  SUM(IF(t_pdvSt = 10, (IF(R.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj10, \n" +
         "  SUM(IF(t_pdvSt = 23, (IF(R.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj23, \n" +
         "  SUM(IF(t_pdvSt = 25, (IF(R.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj25, \n" +
         "  SUM(IF(t_pdvSt = 05, (IF(R.t_tt = 'IZM', rtrIzlazKol, (pstKol+ulazKol-izlazKol)-(rtrPstKol + rtrUlazKol-rtrIzlazKol)))*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj05  \n" +
         "       FROM rtrans  R                                                                                                                   \n" +
         "  LEFT JOIN artstat A ON R.RecID = A.rtransRecID                                                                                        \n" +
         "  WHERE R.t_tt != 'VMI' AND R.t_isIrmUslug = 0                                                                                          \n" +
         "  GROUP BY R.t_parentID                                                                                                                 \n" +
         ") AS R ON F.RecID = R.t_parentID                                                                                                        \n" ;

   private const string malop_RtransArtstat_subquery_IZLAZ =
         "LEFT JOIN(                                                                                                                              \n" +
         "  SELECT R.t_parentID, R.t_kol, A.rtrCijenaNBC,                                                                                         \n" +
         "  SUM(IF(t_pdvSt =  0,                                                    (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj00, \n" +
         "  SUM(IF(t_pdvSt = 10,                                                    (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj10, \n" +
         "  SUM(IF(t_pdvSt = 23,                                                    (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj23, \n" +
         "  SUM(IF(t_pdvSt = 25,                                                    (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj25, \n" +
         "  SUM(IF(t_pdvSt = 05,                                                    (rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 0)) as K_NivVrj05  \n" +
         "       FROM rtrans  R                                                                                                                   \n" +
         "  LEFT JOIN artstat A ON R.RecID = A.rtransRecID                                                                                        \n" +
         "  WHERE R.t_isIrmUslug = 0                                                                                                              \n" +
         "  GROUP BY R.t_parentID                                                                                                                 \n" +
         ") AS R ON F.RecID = R.t_parentID                                                                                                        \n";

#endif


   //private const string malop_RtransArtstat_subquery_IZLAZ =
   //      "LEFT JOIN(                                                                                                                                        \n" +
   //      "  SELECT R.t_parentID, R.t_kol, A.rtrCijenaNBC,                                                                                                   \n" +
   //      "  SUM(IF(t_pdvSt =  0,                                                    ROUND((rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 2), 0)) as K_NivVrj00, \n" +
   //      "  SUM(IF(t_pdvSt = 10,                                                    ROUND((rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 2), 0)) as K_NivVrj10, \n" +
   //      "  SUM(IF(t_pdvSt = 23,                                                    ROUND((rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 2), 0)) as K_NivVrj23, \n" +
   //      "  SUM(IF(t_pdvSt = 25,                                                    ROUND((rtrIzlazKol)*(rtrCijenaMPC-prevMalopCij), 2), 0)) as K_NivVrj25  \n" +
   //      "       FROM rtrans  R                                                                                                                             \n" +
   //      "  LEFT JOIN artstat A ON R.RecID = A.rtransRecID                                                                                                  \n" +
   //      "  WHERE R.t_isIrmUslug = 0                                                                                                                        \n" +
   //      "  GROUP BY R.t_parentID                                                                                                                           \n" +
   //      ") AS R ON F.RecID = R.t_parentID                                                                                                                  \n";

   #endregion Common SELECT-JOIN-SUBQUERY strings

#if OldKPM
   internal static XSqlCommand Load_KPM_MalUlazUnionIrmGroupedFakturList_Command(XSqlConnection conn, string selectWhat_ULAZ, string selectWhat_IZLAZ, List<VvSqlFilterMember> filterMembers)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "(SELECT \n"                                                                         + 
                        selectWhat_ULAZ                                                                      +
                        faktur_JOIN_faktEx_clause                                                            +
                        (ZXC.RRD.Dsc_IsSupressSHADOWing ? "" : malop_RtransArtstat_subquery_ULAZ)            +
                        "WHERE tt IN "                                                                       +
                      //TtInfo.UlazniMALOP_IN_Clause     .Replace("VMU", "VMI")                       + "\n" +
                      //TtInfo.UlazniMALOP_IN_Clause_wIZM.Replace("VMU", "VMI")                       + "\n" +
                        TtInfo.UlazniMALOP_IN_Clause_wIZM.Replace("VMU", "VMI").Replace("TRM", "TRI") + "\n" +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true)                   +
                        "GROUP BY DATE(dokDate), ttSort, ttNum) \n"                                          +

                        //"UNION #===============================================================================\n" +
                      // 22.11.2012:
                      //"\nUNION \n\n" +
                        "\nUNION ALL \n\n" +

                        "(SELECT \n"                                                               + 
                        selectWhat_IZLAZ                                                           +
                        faktur_JOIN_faktEx_clause                                                  +
                        (ZXC.RRD.Dsc_IsSupressSHADOWing ? "" : malop_RtransArtstat_subquery_IZLAZ) +
                      //"WHERE tt IN " + TtInfo.IzlazniMALOP_IN_Clause +  "\n"                     + // IZM ne smije ovdje doci 
                        "WHERE tt = '" + Faktur.TT_IRM                 + "'\n"                     +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true)         +
                        "GROUP BY DATE(dokDate)) \n"                                               +
                        
                        //"UNION #===============================================================================\n" +
                        "ORDER BY dokDate, ttSort, ttNum \n"; 

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand Load_Fak2Nal_URM_FakturList_Command(XSqlConnection conn, string selectWhat_ULAZ, List<VvSqlFilterMember> filterMembers, string anotherJoinClause)
   {
      VvRptFilter RptFilter = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT \n"                                                         + 
                        selectWhat_ULAZ                                                     +
                        faktur_JOIN_faktEx_clause                                           +
                        malop_RtransArtstat_subquery_ULAZ                                   +
                        anotherJoinClause                                                   +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +
                        "GROUP BY DATE(dokDate), ttSort, ttNum \n"                          ;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

#endif

   internal static XSqlCommand Load_KPM_MalUlazUnionIrmGroupedFakturList_Command(XSqlConnection conn, string selectWhat_ULAZ, string selectWhat_IZLAZ, string selectWhat_IRMniv, List<VvSqlFilterMember> filterMembers)
   {
      VvRptFilter RptFilter   = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "(SELECT \n" +
                        selectWhat_ULAZ +
                        faktur_JOIN_faktEx_JOIN_artstat_clause +
                        "WHERE tt IN " +
                        TtInfo.UlazniMALOP_IN_Clause_wIZM.Replace("VMU", "VMI").Replace("TRM", "TRI") + "\n" +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true) +
                        "GROUP BY DATE(dokDate), ttSort, ttNum) \n" +

                        "\nUNION ALL \n\n" +

                        "(SELECT \n" +
                        selectWhat_IZLAZ +
                        faktur_JOIN_faktEx_clause +
                        "WHERE tt = '" + Faktur.TT_IRM + "'\n" +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true) +
                        "GROUP BY DATE(dokDate)) \n" +

                        "\nUNION ALL \n\n" +

                        "(SELECT \n" +
                        selectWhat_IRMniv +
                        faktur_JOIN_faktEx_JOIN_artstat_clause +
                        "WHERE tt = '" + Faktur.TT_IRM + "'\n" +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, true) +
                        "GROUP BY DATE(dokDate)) \n" +

                        "ORDER BY dokDate, ttSort, ttNum \n";

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand Load_Fak2Nal_URM_FakturList_Command(XSqlConnection conn, string selectWhat_ULAZ, List<VvSqlFilterMember> filterMembers, string anotherJoinClause)
   {
      VvRptFilter RptFilter   = new VvRptFilter();
      RptFilter.FilterMembers = filterMembers;

      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT \n"                                                         + 
                        selectWhat_ULAZ                                                     +
                        faktur_JOIN_faktEx_JOIN_artstat_clause                              +
                        anotherJoinClause                                                   +
                        VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +
                        "GROUP BY DATE(dokDate), ttSort, ttNum \n"                          ;

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   // dis is PUSE ! 
   //internal static XSqlCommand Load_GroupedIRM_4Fak2Nal_Command(XSqlConnection conn, string selectWhat, List<VvSqlFilterMember> filterMembers, string anotherJoinClause, Faktur2NalogRulesAndData theRules)
   //{
   //   VvRptFilter RptFilter = new VvRptFilter();
   //   RptFilter.FilterMembers = filterMembers;

   //   string groupByClause     = "GROUP BY MONTH(dokDate), nacPlac    \n";

   //   if(theRules.KtoShemaDsc.Dsc_MirSumMonthly == false)
   //   {
   //      anotherJoinClause = anotherJoinClause.Replace("MONTH", "");
   //      groupByClause     = groupByClause    .Replace("MONTH", "");
   //   }
   //   if(theRules.KtoShemaDsc.Dsc_MirGroupByNacPlac == false)
   //   {
   //      groupByClause = groupByClause.Replace(", nacPlac", "");
   //   }

   //   XSqlCommand cmd = InitCommand(conn);

   //   cmd.CommandText = "SELECT " + selectWhat.Replace("L.recID", "0").Replace("recID", "dummy") + " FROM  faktur L      \n" +
   //                     "LEFT JOIN                       faktEx R    \n\n" +
   //                     "ON L.RecID = R.fakturRecID \n\n" +

   //                     anotherJoinClause + // kocnica za vec prebacene dane ili mjesece (dakle da ne moze dva puta prenijeti)

   //                     VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +
                        
   //                     groupByClause;

   //   VvSQL.SetReportCommandParamValues(cmd, filterMembers);

   //   return (cmd);
   //}

   #endregion KPM & Fak2Nal MALOP

   #endregion GetGenericVvDataRecordList_Command

   #region VvLocker

   #region VvLockerInfo

   public class /*struct*/ VvLockerInfo
   {
      public string tableName;
      public uint   recID; // ID lockiranog recorda

      public string   editorUID;
      public DateTime inEditTS;
      public string   clientMachineName;
      public string   clientUserName;
      public uint     lockID; // from DB (PRIMARY KEY) 

      public VvLockerInfo(string _tableName, uint _recID)
      {
         tableName = _tableName;
         recID     = _recID;

         //editorUID          = "";
         //inEditTS           = DateTime.MinValue;
         //environMachineName = "";
         //environUserName    = "";
         //_enumName             = 0;
      }
   }

   public class /*struct*/ VvSKYlockerInfo
   {
      public uint                   lockID           ; // PRIMARY KEY
      public DateTime               lockTS           ; // CURRENT_TIMESTAMP
      public string                 record           ; // from SkyRule 
      public string                 documTT          ; // from SkyRule 
      public ZXC.LanSrvKind         birthLoc         ; // from SkyRule 
      public ZXC.SkySklKind         skl1Kind         ; // from SkyRule 
      public ZXC.SkySklKind         skl2Kind         ; // from SkyRule 
      public uint                   origSrvID        ; // (recordov LanSrvID)
      public uint                   origRecID        ; // (recordov LanRecID)
      public string                 skladCD          ; // (recordov sklCD 1) 
      public string                 skladCD2         ; // (recordov sklCD 2) 
      public VvSQL.DB_RW_ActionType lockAction       ; // lan WRITE action   
      public string                 lockUID          ; // ZXC.vvDB_User      
      public string                 clientCompterName; // Environment.MachineName



      public VvSKYlockerInfo(string _tableName, uint _recID)
      {
         record = _tableName;
         origRecID     = _recID;

         //editorUID          = "";
         //inEditTS           = DateTime.MinValue;
         //environMachineName = "";
         //environUserName    = "";
         //_enumName             = 0;
      }
   }

   #endregion VvLockerInfo

   public static XSqlCommand FindInLocker_Command(XSqlConnection conn, VvLockerInfo lockerInfo)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT * FROM "      + ZXC.vvDB_lockerTableName + " \n" + 
                        "WHERE tableName = '" + lockerInfo.tableName     + "'\n" +
                        "AND   recID     = '" + lockerInfo.recID         + "'\n" +
                        "ORDER BY inEditTS DESC";
      return (cmd);
   }

   public static XSqlCommand DeleteFromLocker_Command(XSqlConnection conn, VvLockerInfo lockerInfo)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DELETE FROM "        + ZXC.vvDB_lockerTableName + " \n" + 
                        "WHERE tableName = '" + lockerInfo.tableName     + "'\n" +
                        "AND   recID     = '" + lockerInfo.recID         + "'";
      return (cmd);
   }

   public static XSqlCommand EQLREC_VvLocker_byRecID_Command(XSqlConnection conn, uint lockID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = String.Format("SELECT * FROM {0} WHERE lockID = '{1}'", ZXC.vvDB_lockerTableName, lockID);

      return (cmd);
   }

   public static XSqlCommand InsertInLocker_Command(XSqlConnection conn, VvLockerInfo lockerInfo)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "INSERT INTO " + ZXC.vvDB_lockerTableName + " SET \n" +
                        "tableName          = ?prm_tableName, \n"             +
                        "recID              = ?prm_recID, \n"                 +
                        "editorUID          = ?prm_editorUID, \n"             +
                        //"inEditTS           = ?prm_inEditTS, \n"            +
                        "environMachineName = ?prm_environMachineName, \n"    +
                        "environUserName    = ?prm_environUserName;\n"        +

         "SELECT @@IDENTITY"; // ovo ce pokupiti return od ExecuteScalar();

      VvSQL.CreateCommandParameter(cmd, "tableName",          lockerInfo.tableName,          XSqlDbType.VarChar,  16);
      VvSQL.CreateCommandParameter(cmd, "recID",              lockerInfo.recID,              XSqlDbType.Int32,    10);
      VvSQL.CreateCommandParameter(cmd, "editorUID",          lockerInfo.editorUID,          XSqlDbType.VarChar,  16);
    //VvSQL.CreateCommandParameter(cmd, "inEditTS",           ucList.inEditTS,               XSqlDbType.Datetime, 32);
      VvSQL.CreateCommandParameter(cmd, "environMachineName", lockerInfo.clientMachineName, XSqlDbType.VarChar,   32);
      VvSQL.CreateCommandParameter(cmd, "environUserName",    lockerInfo.clientUserName ,   XSqlDbType.VarChar,   32);

      return (cmd);
   }

   #endregion VvLocker

   #region VvLanLog Entry

   public struct VvLanLogEntry
   {
      public bool             isSkyTraffic ;
      public string           record       ;
      public string           tt           ;
      public string           skladCD      ;
      public string           skladCD2     ;
      public DB_RW_ActionType action       ;
      public DateTime         logTS        ;
      public string           logUID       ;
      public string           clientName   ;
      public string           lanServerName;
      public uint             lanServerID  ; 
      public uint             recID        ; //ID recorda na koji se entry odnosi
      public uint             logID        ; // log entry ID-a (primary key)
                                           
      public DateTime         addTS        ; // VvDataRecord metadata 
      public DateTime         modTS        ; // VvDataRecord metadata 
      public string           addUID       ; // VvDataRecord metadata 
      public string           modUID       ; // VvDataRecord metadata 
      public uint             origSrvID    ; // VvDataRecord metadata 
      public uint             origRecID    ; // VvDataRecord metadata 

      public string ActionAsStr
      {
         get { return action.ToString(); }
      }

      public override string ToString()
      {
         return record + "[rID:" + recID + "]." + action + "." + logTS + "." + logUID + "." + lanServerName;
      }

      public uint SrvRecID { get { return ZXC.GetSrvRecID(this.origSrvID, this.origRecID); } }

   }
   
   public static XSqlCommand LANLOG_ADDREC_Command(XSqlConnection conn, VvLanLogEntry logEntry)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "INSERT INTO " + ZXC.vvDB_LANlogTableName  + 
                        " SET \n"                               + 
                        " record        = ?prm_record, "        +
                        " isSkyTraffic  = ?prm_isSkyTraffic, "  +
                        " origSrvID     = ?prm_origSrvID, "     +
                        " origRecID     = ?prm_origRecID, "     +
                        " tt            = ?prm_tt, "            +
                        " skladCD       = ?prm_skladCD, "       +
                        " skladCD2      = ?prm_skladCD2,"       +
                        " action        = ?prm_action, "        +
                        " logUID        = ?prm_logUID, "        +
                        " addTS         = ?prm_addTS   , "      +
                        " modTS         = ?prm_modTS   , "      +
                        " addUID        = ?prm_addUID  , "      +
                        " modUID        = ?prm_modUID  , "      +
                        " lanServerName = ?prm_lanServerName, " +
                        " lanServerID   = ?prm_lanServerID, "   +
                        " clientName    = ?prm_clientName, "    +
                        " recID         = ?prm_recID  ;"        +
                        (ZXC.IsSkyEnvironment ? " SELECT @@IDENTITY " :"");

      VvSQL.CreateCommandParameter(cmd, "record",        logEntry.record,        XSqlDbType.VarChar, 16);
      VvSQL.CreateCommandParameter(cmd, "isSkyTraffic",  logEntry.isSkyTraffic,  XSqlDbType.UInt16 , 10);
      VvSQL.CreateCommandParameter(cmd, "tt",            logEntry.tt,            XSqlDbType.VarChar,  3);
      VvSQL.CreateCommandParameter(cmd, "skladCD",       logEntry.skladCD,       XSqlDbType.VarChar,  8);
      VvSQL.CreateCommandParameter(cmd, "skladCD2",      logEntry.skladCD2,      XSqlDbType.VarChar,  8);
      VvSQL.CreateCommandParameter(cmd, "action",        logEntry.ActionAsStr,   XSqlDbType.Enum,     8);
      VvSQL.CreateCommandParameter(cmd, "logUID",        logEntry.logUID,        XSqlDbType.VarChar, 16);
      VvSQL.CreateCommandParameter(cmd, "clientName",    logEntry.clientName,    XSqlDbType.VarChar, 32);
      VvSQL.CreateCommandParameter(cmd, "lanServerName", logEntry.lanServerName, XSqlDbType.VarChar, 32);
      VvSQL.CreateCommandParameter(cmd, "lanServerID",   logEntry.lanServerID,   XSqlDbType.UInt32,  10);
      VvSQL.CreateCommandParameter(cmd, "recID",         logEntry.recID,         XSqlDbType.Int32,   10);
      VvSQL.CreateCommandParameter(cmd, "addTS",         logEntry.addTS    ,     XSqlDbType.DateTime,32);
      VvSQL.CreateCommandParameter(cmd, "modTS",         logEntry.modTS    ,     XSqlDbType.DateTime,32);
      VvSQL.CreateCommandParameter(cmd, "addUID",        logEntry.addUID   ,     XSqlDbType.VarChar, 16);
      VvSQL.CreateCommandParameter(cmd, "modUID",        logEntry.modUID   ,     XSqlDbType.VarChar, 16);
      VvSQL.CreateCommandParameter(cmd, "origSrvID",     logEntry.origSrvID,     XSqlDbType.UInt32 , 16);
      VvSQL.CreateCommandParameter(cmd, "origRecID",     logEntry.origRecID,     XSqlDbType.UInt32 , 16);

      return(cmd);
   }

   public static XSqlCommand GetNonSynchronizedLANchanges_LogList_Command(XSqlConnection conn, SkyRule skyRule_rec, ZXC.SkyOperation wantedOperation)
   {
      XSqlCommand cmd = InitCommand(conn);

      string myServerID = ZXC.vvDB_ServerID.ToString();

      string dbName = conn.Database;
#if OLDddd
        cmd.CommandText =              "SELECT LAN.* FROM "     + ZXC.vvDB_LANlogTableName + " LAN"                               + " \n" +
                                       "LEFT JOIN     "         + ZXC.vvDB_SKYlogTableName + " SKY"                               + " \n" +
                                       "ON LAN.logID = SKY.lanLogID"                                                              + " \n" +
                                       "WHERE SKY.skyLogID  IS NULL"                                                              + " \n" +
                                     //"AND SKY.syncErrNo != 0     "                                                              + " \n" +
                                       "AND LAN.record     = '" + skyRule_rec.Record                                              + "'\n" +
  (skyRule_rec.DocumTT   .NotEmpty() ? "AND LAN.tt         = '" + skyRule_rec.DocumTT                                             + "'\n" : "\n") +
//(skyRule_rec.FrsSklKind.NotEmpty() ? "AND LAN.skladCD    = '" + Faktur.GetSklad1CDfromSkyFrsSklKind    (skyRule_rec.FrsSklKind) + "'\n" : "\n") +
  (skyRule_rec.IsOnly4LocSk == true  ? "AND (LAN.skladCD LIKE '" + myServerID + "%' OR LAN.skladCD2 LIKE '" + myServerID + "%')"  + " \n" : "\n") +
                                       "ORDER BY LAN.logTS ASC";
                                     //"GROUP BY LAN.logID ASC";
#endif 

      // BIO BUG!!! dodano tek 18.01.2015: 
      bool shouldConsiderShopRCVkind = wantedOperation == ZXC.SkyOperation.RECEIVE && ZXC.IsTEXTHOshop && skyRule_rec.ShopRCVkind.NotEmpty();
         
          cmd.CommandText =                      "SELECT LAN.* FROM "      + dbName + "." + ZXC.vvDB_LANlogTableName + " LAN"                 + " \n" +
                                                 "LEFT JOIN     "          + dbName + "." + ZXC.vvDB_SKYlogTableName + " SKY"                 + " \n" +
                                                 "ON LAN.logID = SKY.lanLogID"                                                                + " \n" +
                                                 "WHERE SKY.skyLogID  IS NULL"                                                                + " \n" +
                                                 "AND LAN.record      = '" + skyRule_rec.Record                                               + "'\n" +
  (skyRule_rec.DocumTT    .NotEmpty()          ? "AND LAN.tt          = '" + skyRule_rec.DocumTT                                              + "'\n" : "\n") +
  (skyRule_rec.Skl1kind   .NotEmpty()          ? "AND LAN.skladCD  LIKE '" + Faktur.GetLikeSkladCDfromSkyRule_SklKind(skyRule_rec.Skl1kind)   + "'\n" : "\n") +
  (skyRule_rec.Skl2kind   .NotEmpty()          ? "AND LAN.skladCD2 LIKE '" + Faktur.GetLikeSkladCDfromSkyRule_SklKind(skyRule_rec.Skl2kind)   + "'\n" : "\n") +
//(skyRule_rec.ShopRCVkind.NotEmpty()          ?                             Faktur.GetLikeSkladXSkyRule_ShopRCVkind(skyRule_rec.ShopRCVkind) + " \n" : "\n") +
  (shouldConsiderShopRCVkind                   ?                             Faktur.GetLikeSkladXSkyRule_ShopRCVkind(skyRule_rec.ShopRCVkind) + " \n" : "\n") +
  (wantedOperation == ZXC.SkyOperation.SEND    ? "AND LAN.isSkyTraffic  = 0"                                                                  + " \n" : "\n") + // izbaci logLAN novosti (lokalne),  a koje su stigle sa SKY-a 
  (wantedOperation == ZXC.SkyOperation.RECEIVE ? "AND LAN.lanServerID  != " + ZXC.vvDB_ServerID                                               + " \n" : "\n") + // izbaci logLAN novosti (sa Sky-a), a koje sam SAM izazvao    
  
                                                 "ORDER BY LAN.logTS ASC";
                                               //"GROUP BY LAN.logID ASC";
        return (cmd);
   }

   public static XSqlCommand Get_AfterInitNY_LANchanges_LogList_Command(XSqlConnection conn, DateTime nyTableCreationTime, string tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      string dbName = conn.Database;
         
          cmd.CommandText = "SELECT LAN.* FROM "  + dbName + "." + ZXC.vvDB_LANlogTableName + " LAN"                + " \n" +
                            "WHERE record = '"    + tableName                                                       + "'\n" +
                            "AND   LAN.logTS > '" + nyTableCreationTime.ToString(ZXC.VvDateYyyyMmDdMySQL_TS_Format) + "'\n" +
  
                            "ORDER BY LAN.logTS ASC";
        return (cmd);
   }

   public static XSqlCommand SetMe_LanLogEntry_byLogID_Command(XSqlConnection conn, uint logID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT LAN.* FROM " + ZXC.vvDB_LANlogTableName + " LAN" + " \n" +
                                     "WHERE LAN.logID = " + logID;
      return (cmd);
   }

   public static XSqlCommand SetMe_LanLogEntryByLanSrvRecID_Command(XSqlConnection conn, VvDataRecord theVvDataRecord, VvSQL.DB_RW_ActionType dB_RW_ActionType)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT LAN.* FROM "      + ZXC.vvDB_LANlogTableName + " LAN" + " \n" +
                        "WHERE  LAN.origSrvID = " + theVvDataRecord.VirtualLanSrvID   + " \n" +
                        "AND    LAN.origRecID = " + theVvDataRecord.VirtualLanRecID   + " \n" +
                        "AND    LAN.record    ='" + theVvDataRecord.VirtualRecordName + "'\n" +
                        "AND    LAN.action    = " + "'ADD'";
      return (cmd);
   }

   #endregion VvLog Entry

   #region VvSkyLog Entry

   public struct VvSkyLogEntry
   {
          public uint             syncErrNo    ; // COMMENT 'Eventualni error number. 0 if no error',                 
          public string           syncMessage  ; // COMMENT 'Error ili neki drugi message',                           
          public ZXC.SkyOperation operation    ; // COMMENT 'SEND, RECEIVE, SendAndReceive',                          
          public DB_RW_ActionType resultAction ; // COMMENT 'resulting action za eventualni paket pojava recID-a'     
          public DateTime         thisSyncTS   ; // COMMENT 'TS ove       sinhronizacije',                            
          public DateTime         prevSyncTS   ; // COMMENT 'TS prethodne sinhronizacije',                            
          public DateTime         skyLogTS     ; // COMMENT 'TS nastanka ovog SkyLogEntrya',                          
          public uint             skyLogID     ; // log entry ID-a (primary key)
          public uint             ruleRecID    ; // COMMENT 'Veza na SkyRule record koji je stvorio ovaj skyLog entry'

/*lanLog*/public bool             isSkyTraffic ;
/*lanLog*/public string           record       ;
/*lanLog*/public string           tt           ;
/*lanLog*/public string           skladCD      ;
/*lanLog*/public string           skladCD2     ;
/*lanLog*/public DB_RW_ActionType action       ;
/*lanLog*/public DateTime         lanLogTS     ;
/*lanLog*/public string           lanLogUID    ;
/*lanLog*/public string           lanClientName;
/*lanLog*/public string           lanServerName;
/*lanLog*/public uint             lanServerID  ; 
/*lanLog*/public uint             currRecID    ; //lokalni ID recorda na koji se entry odnosi
/*lanLog*/public uint             lanLogID     ; // za JOIN VvLanLog entry ID-a (primary key) !!! stavi i INDEX na SkyLog record 
/*lanLog*/public DateTime         addTS        ; // VvDataRecord metadata 
/*lanLog*/public DateTime         modTS        ; // VvDataRecord metadata 
/*lanLog*/public string           addUID       ; // VvDataRecord metadata 
/*lanLog*/public string           modUID       ; // VvDataRecord metadata 
/*lanLog*/public uint             origSrvID    ; // VvDataRecord metadata 
/*lanLog*/public uint             origRecID    ; // VvDataRecord metadata 

      public override string ToString()
      {
         return "op: " + operation + " [record:" + record + " TT:" + tt + " skladCD:" + skladCD + "] errNo:" + syncErrNo + "TS: " + thisSyncTS.ToString(ZXC.VvTimeStampFormat);
      }

      public bool Is_Shop_SEND_ADDaction_IRM
      {
         get
         {
            return operation    == ZXC.SkyOperation.SEND &&
                   resultAction == DB_RW_ActionType.ADD  &&
                   tt           == Faktur.TT_IRM          ;
         }
      }
   }

   public static XSqlCommand SKYLOG_ADDREC_Command(XSqlConnection conn, VvSkyLogEntry skyLogEntry, bool isERR)
   {
      return SKYLOG_ADDREC_Command(conn, skyLogEntry, isERR, false);
   }

   public static XSqlCommand SKYLOG_ADDREC_Command(XSqlConnection conn, VvSkyLogEntry skyLogEntry, bool isERR, bool isCpyERR)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName = isERR ? ZXC.vvDB_ERRlogTableName : ZXC.vvDB_SKYlogTableName;

      if(isCpyERR) tableName = ZXC.vvDB_CpyERRlogTableName;

      cmd.CommandText = "INSERT INTO " + tableName + 
                        " SET \n"  +
                        " syncErrNo     = ?prm_syncErrNo    , " +
                        " syncMessage   = ?prm_syncMessage  , " +
                        " operation     = ?prm_operation    , " +
                        " resultAction  = ?prm_resultAction , " +
                        " thisSyncTS    = ?prm_thisSyncTS   , " +
                        " prevSyncTS    = ?prm_prevSyncTS   , " +
                        " ruleRecID     = ?prm_ruleRecID    , " +
                        " isSkyTraffic  = ?prm_isSkyTraffic , " +
                        " record        = ?prm_record       , " +
                        " tt            = ?prm_tt           , " +
                        " skladCD       = ?prm_skladCD      , " +
                        " skladCD2      = ?prm_skladCD2     , " +
                        " action        = ?prm_action       , " +
                        " lanLogTS      = ?prm_lanLogTS     , " +
                        " lanLogUID     = ?prm_lanLogUID    , " +
                        " lanClientName = ?prm_lanClientName, " +
                        " lanServerName = ?prm_lanServerName, " +
                        " lanServerID   = ?prm_lanServerID  , " +
                        " currRecID     = ?prm_currRecID    , " +
                        " lanLogID      = ?prm_lanLogID     , " +
                        " addTS         = ?prm_addTS        , " +
                        " modTS         = ?prm_modTS        , " +
                        " addUID        = ?prm_addUID       , " +
                        " modUID        = ?prm_modUID       , " +
                        " origSrvID     = ?prm_origSrvID    , " +
                        " origRecID     = ?prm_origRecID      ";

      VvSQL.CreateCommandParameter(cmd, "syncErrNo"    , skyLogEntry.syncErrNo    , XSqlDbType.UInt32   ,   16);
      VvSQL.CreateCommandParameter(cmd, "syncMessage"  , skyLogEntry.syncMessage  , XSqlDbType.VarChar  , 1024);
      VvSQL.CreateCommandParameter(cmd, "operation"    , skyLogEntry.operation    , XSqlDbType.Enum     ,   32);
      VvSQL.CreateCommandParameter(cmd, "resultAction",  skyLogEntry.resultAction , XSqlDbType.Enum     ,    8);
      VvSQL.CreateCommandParameter(cmd, "thisSyncTS"   , skyLogEntry.thisSyncTS   , XSqlDbType.Timestamp,   32);
      VvSQL.CreateCommandParameter(cmd, "prevSyncTS"   , skyLogEntry.prevSyncTS   , XSqlDbType.Timestamp,   32);
      VvSQL.CreateCommandParameter(cmd, "record",        skyLogEntry.record       , XSqlDbType.VarChar  ,   16);
      VvSQL.CreateCommandParameter(cmd, "ruleRecID"   ,  skyLogEntry.ruleRecID    , XSqlDbType.UInt32   ,   16);
      VvSQL.CreateCommandParameter(cmd, "isSkyTraffic",  skyLogEntry.isSkyTraffic , XSqlDbType.UInt16   ,   10);
      VvSQL.CreateCommandParameter(cmd, "tt",            skyLogEntry.tt           , XSqlDbType.VarChar  ,    3);
      VvSQL.CreateCommandParameter(cmd, "skladCD",       skyLogEntry.skladCD      , XSqlDbType.VarChar  ,    8);
      VvSQL.CreateCommandParameter(cmd, "skladCD2",      skyLogEntry.skladCD2     , XSqlDbType.VarChar  ,    8);
      VvSQL.CreateCommandParameter(cmd, "action",        skyLogEntry.action       , XSqlDbType.Enum     ,    8);
      VvSQL.CreateCommandParameter(cmd, "lanLogTS",      skyLogEntry.lanLogTS     , XSqlDbType.Timestamp,   32);
      VvSQL.CreateCommandParameter(cmd, "lanLogUID",     skyLogEntry.lanLogUID    , XSqlDbType.VarChar  ,   16);
      VvSQL.CreateCommandParameter(cmd, "lanClientName", skyLogEntry.lanClientName, XSqlDbType.VarChar  ,   32);
      VvSQL.CreateCommandParameter(cmd, "lanServerName", skyLogEntry.lanServerName, XSqlDbType.VarChar  ,   32);
      VvSQL.CreateCommandParameter(cmd, "lanServerID",   skyLogEntry.lanServerID  , XSqlDbType.UInt32   ,   10);
      VvSQL.CreateCommandParameter(cmd, "currRecID",     skyLogEntry.currRecID    , XSqlDbType.Int32    ,   10);
      VvSQL.CreateCommandParameter(cmd, "lanLogID",      skyLogEntry.lanLogID     , XSqlDbType.UInt32   ,   16);
      VvSQL.CreateCommandParameter(cmd, "addTS",         skyLogEntry.addTS        , XSqlDbType.DateTime ,   32);
      VvSQL.CreateCommandParameter(cmd, "modTS",         skyLogEntry.modTS        , XSqlDbType.DateTime ,   32);
      VvSQL.CreateCommandParameter(cmd, "addUID",        skyLogEntry.addUID       , XSqlDbType.VarChar  ,   16);
      VvSQL.CreateCommandParameter(cmd, "modUID",        skyLogEntry.modUID       , XSqlDbType.VarChar  ,   16);
      VvSQL.CreateCommandParameter(cmd, "origSrvID",     skyLogEntry.origSrvID    , XSqlDbType.UInt32   ,   16);
      VvSQL.CreateCommandParameter(cmd, "origRecID",     skyLogEntry.origRecID    , XSqlDbType.UInt32   ,   16);

      return(cmd);
   }

   public static XSqlCommand SKYLOG_DELREC_Command(XSqlConnection conn, VvSkyLogEntry skyLogEntry, bool isCloseSyncTransaction)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName = ZXC.vvDB_SKYlogTableName;

      cmd.CommandText =   "DELETE from " + tableName + 
                          " WHERE \n"  +
(isCloseSyncTransaction ? " operation     = ?prm_operation AND" : "") +
                          " origSrvID     = ?prm_origSrvID AND " +
                          " record        = ?prm_record AND "    + // ovaj 'record' je dodan tek 18.10.2020 ! 
                          " origRecID     = ?prm_origRecID     ";

      VvSQL.CreateCommandParameter(cmd, "origSrvID",     skyLogEntry.origSrvID    , XSqlDbType.UInt32   ,   16);
      VvSQL.CreateCommandParameter(cmd, "origRecID", skyLogEntry.origRecID, XSqlDbType.UInt32, 16);

      VvSQL.CreateCommandParameter(cmd, "record", skyLogEntry.record, XSqlDbType.String, 16);

      if(isCloseSyncTransaction)
      VvSQL.CreateCommandParameter(cmd, "operation", ZXC.SkyOperation.OpenSyncTran, XSqlDbType.Enum, 32);

      return(cmd);
   }

   public static XSqlCommand SKYLOG_DELREC_AsDISPACHED_ToShop_OrCentr_Command(XSqlConnection conn, VvSkyLogEntry skyLogEntry, string SKY_log_tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName = /*ZXC.vvDB_SKYlogTableName*/ SKY_log_tableName;

      cmd.CommandText =   "DELETE from " + tableName + 
                          " WHERE \n"  +
                          " record        = ?prm_record AND " +
                          " origSrvID     = ?prm_origSrvID AND " +
                          " origRecID     = ?prm_origRecID     ";

      VvSQL.CreateCommandParameter(cmd, "record"   , skyLogEntry.record   , XSqlDbType.String, 16);
      VvSQL.CreateCommandParameter(cmd, "origSrvID", skyLogEntry.origSrvID, XSqlDbType.UInt32, 16);
      VvSQL.CreateCommandParameter(cmd, "origRecID", skyLogEntry.origRecID, XSqlDbType.UInt32, 16);
      
      return(cmd);
   }

   public static XSqlCommand GetLastSkyOrErrLogEntry_Command(XSqlConnection conn, bool isERR, SkyRule skyRule_rec, ZXC.SkyOperation operation/*, ZXC.ErrorStatus wantedErrorStatus*/)
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName = isERR ? ZXC.vvDB_ERRlogTableName : ZXC.vvDB_SKYlogTableName;

      cmd.CommandText = "SELECT * FROM "         + tableName                + " \n" +
                        "WHERE ruleRecID    = '" + skyRule_rec.RecID        + "'\n" +
                        "AND   operation    = '" + operation                + "'\n" + // treba li mozda ToString() jer parametriziras po enum-u 
                        /*(wantedErrorStatus == ZXC.ErrorStatus.NO_ERROR ? "AND syncErrNo =  0 " :
                         wantedErrorStatus == ZXC.ErrorStatus.IN_ERROR ? "AND syncErrNo != 0 " : "") +*/ " \n" +

                        "ORDER BY skyLogTS DESC LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMe_SkyLogEntryByLanSrvRecID_Command(XSqlConnection conn, VvDataRecord theVvDataRecord, VvSQL.DB_RW_ActionType dB_RW_ActionType, string SKY_log_tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

    //cmd.CommandText = "SELECT SKY.* FROM "          + ZXC.vvDB_SKYlogTableName + " SKY" + " \n" +
      cmd.CommandText = "SELECT SKY.* FROM "          + SKY_log_tableName        + " SKY" + " \n" +
                        "WHERE  SKY.origSrvID     = " + theVvDataRecord.VirtualLanSrvID   + " \n" +
                        "AND    SKY.origRecID     = " + theVvDataRecord.VirtualLanRecID   + " \n" +
                        "AND    SKY.record        ='" + theVvDataRecord.VirtualRecordName + "'\n" +
                      //"AND    SKY.action        = " + "'ADD'";
                        "AND    SKY.resultAction  = " + "'" + dB_RW_ActionType + "'";
      return (cmd);
   }

   internal static XSqlCommand Get_TODAY_ErrLogEntryList_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      // 29.10.2018: !!! BIG NEWS !!! javljaj postojanje ERR redaka samo za ovodnevne errore. 
    //cmd.CommandText = "SELECT ERR.* FROM " + ZXC.vvDB_ERRlogTableName + " ERR" + " \n";
      cmd.CommandText = "SELECT ERR.* FROM " + ZXC.vvDB_ERRlogTableName + " ERR" + " \n" +
                        "WHERE DATE(skyLogTS) = '" + DateTime.Today.ToString(ZXC.VvDateYyyyMmDdMySQLFormat) + "'";
      return (cmd);
   }

   internal static XSqlCommand GetClientIPaddress_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT SUBSTRING_INDEX(host,':',1) AS 'ip' FROM information_schema.processlist WHERE ID=connection_id()" + " \n";

      return (cmd);
   }

   //SELECT SUBSTRING_INDEX(host,':',1) AS 'ip' FROM information_schema.processlist WHERE ID=connection_id();

   internal static XSqlCommand MBF_ADDREC_Command(XSqlConnection conn, VvDaoBase.MBF_Info theMBF_Info, string tableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "INSERT INTO " + tableName           + 
                        " SET \n"                            + 
                        " serverName    = ?prm_serverName, " +
                        " serverID      = ?prm_serverID  , " +
                        " vvDomena      = ?prm_vvDomena  , " +
                        " year          = ?prm_year      , " +
                        " userName      = ?prm_userName  , " +
                        " prjktCD       = ?prm_prjktCD   , " +
                        " prjktTK       = ?prm_prjktTK   , " + 
                        " prjktName     = ?prm_prjktName , " +
                        " clientName    = ?prm_clientName, " +
                        " appVersion    = ?prm_appVersion, " +
                        " rtrCount      = ?prm_rtrCount  , " +
                        " ftrCount      = ?prm_ftrCount  , " +
                        " ptrCount      = ?prm_ptrCount  , " +
                        " atrCount      = ?prm_atrCount  , " +
                        " xtrCount      = ?prm_xtrCount  , " +
                        " clientIP      = ?prm_clientIP    "; // pazi da zadnji nema zarez 

      VvSQL.CreateCommandParameter(cmd, "serverName",    theMBF_Info.serverName, XSqlDbType.VarChar, 32);
      VvSQL.CreateCommandParameter(cmd, "serverID"  ,    theMBF_Info.serverID  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "rtrCount"  ,    theMBF_Info.rtrCount  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "ftrCount"  ,    theMBF_Info.ftrCount  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "ptrCount"  ,    theMBF_Info.ptrCount  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "atrCount"  ,    theMBF_Info.atrCount  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "xtrCount"  ,    theMBF_Info.xtrCount  , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "vvDomena"  ,    theMBF_Info.vvDomena  , XSqlDbType.VarChar,  6);
      VvSQL.CreateCommandParameter(cmd, "year"      ,    theMBF_Info.year      , XSqlDbType.VarChar,  6);
      VvSQL.CreateCommandParameter(cmd, "userName"  ,    theMBF_Info.userName  , XSqlDbType.VarChar, 16);
      VvSQL.CreateCommandParameter(cmd, "prjktCD"   ,    theMBF_Info.prjktCD   , XSqlDbType.UInt32 , 10);
      VvSQL.CreateCommandParameter(cmd, "prjktTK"   ,    theMBF_Info.prjktTK   , XSqlDbType.VarChar,  6);
      VvSQL.CreateCommandParameter(cmd, "prjktName" ,    theMBF_Info.prjktName , XSqlDbType.VarChar, 50);
      VvSQL.CreateCommandParameter(cmd, "clientName",    theMBF_Info.clientName, XSqlDbType.VarChar, 32);
      VvSQL.CreateCommandParameter(cmd, "appVersion",    theMBF_Info.appVersion, XSqlDbType.VarChar, 32);
      VvSQL.CreateCommandParameter(cmd, "clientIP"  ,    theMBF_Info.clientIP  , XSqlDbType.VarChar, 32);

      return(cmd);
   }

   internal static XSqlCommand Get_HALMEDartikl_List_Command(XSqlConnection conn, string thisATKonly)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT h.* FROM " + ZXC.vvDB_HALMEDartiklTableName + " h" + " \n" +

                        (thisATKonly.NotEmpty() ? " WHERE s_atk = '" + thisATKonly + "'" : "");

      return (cmd);
   }

   internal static XSqlCommand GetFirstActivePCKartiklCD_Command(XSqlConnection conn, string skladCD, string PCK_baza)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = @"SELECT t_artiklCD FROM artstat WHERE t_artiklCD LIKE '" + PCK_baza + @"%' AND artiklTS = 'PCK' AND t_skladCD = '" + skladCD + @"' ORDER BY t_artiklCD LIMIT 1" + " \n";

      return (cmd);
   }

   #endregion VvSkyLog Entry

   #region VvUcListMember

   public struct VvUcListMember
   {
      public uint   recID     ; // 'log entry ID-a',\n"                  +
      public string enumName  ; // 'PrgEnum: ZXC.VvSubModulEnum',\n"     +
      public char   mDigit    ; // 'PrgEnum: ZXC.VvModulEnum',\n"        +
      public char   smDigit   ; // 'PrgEnum: ZXC.VvSubModulKindEnum',\n" +
      public string ucName    ; // 'Imeto na uc-ot',\n"                  +
      public bool   rootOnly  ; // 'if true: uc is for root only',\n"    +
      public string okLogin1  ; // 'only one of this okLogins can use it'+
      public string okLogin2  ; // 'only one of this okLogins can use it'+
      public string okLogin3  ; // 'only one of this okLogins can use it'+
      public string okLogin4  ; // 'only one of this okLogins can use it'+
      public string stopLogin1; // 'forbidden for this login',\n"        +
      public string stopLogin2; // 'forbidden for this login',\n"        +
      public string stopLogin3; // 'forbidden for this login',\n"        +
      public string stopLogin4; // 'forbidden for this login',\n"        +
   }

   //public static XSqlCommand EQLREC_VvUcListMember_byEnumName_Command(XSqlConnection conn, string enumName)
   //{
   //   XSqlCommand cmd = InitCommand(conn);

   //   cmd.CommandText = String.Format("SELECT * FROM {0} WHERE enumName = '{1}'", ZXC.vvDB_ucListTableName, enumName);

   //   return (cmd);
   //}

   #endregion VvUcListMember

   #region VvRiskReportMacro

   //public static XSqlCommand FindInLocker_Command(XSqlConnection conn, VvRiskMacro riskMacro)
   //{
   //   XSqlCommand cmd = InitCommand(conn);

   //   cmd.CommandText = "SELECT * FROM " + ZXC.vvDB_riskReportMacroTableName + " \n" + 
   //                     "WHERE tableName = '" + riskMacro.tableName   + "'\n" +
   //                     "AND   recID     = '" + riskMacro.recID       + "'\n" +
   //                     "ORDER BY inEditTS DESC";
   //   return (cmd);
   //}

   public static XSqlCommand DeleteFromRiskMacro_Command(XSqlConnection conn, VvRiskMacro riskMacro)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DELETE FROM "        + ZXC.vvDB_riskMacroTableName + " \n" + 
                        "WHERE recID     = '" + riskMacro.RecID                   + "'";
      return (cmd);
   }

   public static XSqlCommand EQLREC_VvRiskMacro_byRecID_Command(XSqlConnection conn, uint recID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = String.Format("SELECT * FROM {0} WHERE recID = '{1}'", ZXC.vvDB_riskMacroTableName, recID);

      return (cmd);
   }

   public static XSqlCommand EQLREC_VvRiskMacro_byMacroName_Command(XSqlConnection conn, string macroName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = String.Format("SELECT * FROM {0} WHERE MacroName = '{1}'", ZXC.vvDB_riskMacroTableName, macroName);

      return (cmd);
   }

   public static XSqlCommand InsertInRiskMacro_Command(XSqlConnection conn, VvRiskMacro riskMacro)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "INSERT INTO " + ZXC.vvDB_riskMacroTableName + " SET \n" +
        "RecID                      = ?prm_RecID                     ,\n" +
        "MacroName                  = ?prm_MacroName                 ,\n" +
        "ReportZ                    = ?prm_ReportZ                   ,\n" +
        "UseMacroDates              = ?prm_UseMacroDates             ,\n" +
        "DatumDo                    = ?prm_DatumDo                   ,\n" +
        "DatumOd                    = ?prm_DatumOd                   ,\n" +
        "ArtiklCdOD                 = ?prm_ArtiklCdOD                ,\n" +
        "ArtiklCdDO                 = ?prm_ArtiklCdDO                ,\n" +
        "ArtNameOD                  = ?prm_ArtNameOD                 ,\n" +
        "ArtNameDO                  = ?prm_ArtNameDO                 ,\n" +
        "SviArtikli                 = ?prm_SviArtikli                ,\n" +
        "IznosOd                    = ?prm_IznosOd                   ,\n" +
        "IznosDo                    = ?prm_IznosDo                   ,\n" +
        "KD_naziv                   = ?prm_KD_naziv                  ,\n" +
        "KD_ticker                  = ?prm_KD_ticker                 ,\n" +
        "KD_sifra                   = ?prm_KD_sifra                  ,\n" +
        "TT                         = ?prm_TT                        ,\n" +
        "TtNumOd                    = ?prm_TtNumOd                   ,\n" +
        "TtNumDo                    = ?prm_TtNumDo                   ,\n" +
        "MT_naziv                   = ?prm_MT_naziv                  ,\n" +
        "MT_ticker                  = ?prm_MT_ticker                 ,\n" +
        "MT_sifra                   = ?prm_MT_sifra                  ,\n" +
        "SkladCD                    = ?prm_SkladCD                   ,\n" +
        "Napomena                   = ?prm_Napomena                  ,\n" +
        "GrupaKupDob                = ?prm_GrupaKupDob               ,\n" +
        "NacPlac                    = ?prm_NacPlac                   ,\n" +
        "VezniDok                   = ?prm_VezniDok                  ,\n" +
        "DokNumOd                   = ?prm_DokNumOd                  ,\n" +
      //"DokNumDo                   = ?prm_DokNumDo                  ,\n" + // 04.12.2015: DokNumOdDo spremati u macro je besmisleno. DokNumDo je od sada FUSE           
        "GrupiranjeDokum            = ?prm_GrupiranjeDokum           ,\n" +
        "GrupiranjeArtikla          = ?prm_GrupiranjeArtikla         ,\n" +
        "AnalitSintet               = ?prm_AnalitSintet              ,\n" +
        "AnaGrupaPoStranici         = ?prm_AnaGrupaPoStranici        ,\n" +
        "VisiblePostoGrupFooter     = ?prm_VisiblePostoGrupFooter    ,\n" +
        "VisibleOnlyTopGroups       = ?prm_VisibleOnlyTopGroups      ,\n" +
        "NumOfTopGroups             = ?prm_NumOfTopGroups            ,\n" +
        "TopSort                    = ?prm_TopSort                   ,\n" +
        "PdvKnjiga                  = ?prm_PdvKnjiga                 ,\n" +
        "PdvKredit                  = ?prm_PdvKredit                 ,\n" +
        "PdvPovrat                  = ?prm_PdvPovrat                 ,\n" +
        "PdvPredujam                = ?prm_PdvPredujam               ,\n" +
        "PdvUstup                   = ?prm_PdvUstup                  ,\n" +
        "ImaPorezZastupnika         = ?prm_ImaPorezZastupnika        ,\n" +
        "IsAutoPorezniKredit        = ?prm_IsAutoPorezniKredit       ,\n" +
        "NacinPlacanja              = ?prm_NacinPlacanja             ,\n" +
        "PdvF_Osn                   = ?prm_PdvF_Osn                  ,\n" +
        "PdvF_Pdv                   = ?prm_PdvF_Pdv                  ,\n" +
        "PdvIspravak                = ?prm_PdvIspravak               ,\n" +
        "PdvObrSastavio             = ?prm_PdvObrSastavio            ,\n" +
        "IsPrjktTel                 = ?prm_IsPrjktTel                ,\n" +
        "IsPrjktFax                 = ?prm_IsPrjktFax                ,\n" +
        "IsPrjktMail                = ?prm_IsPrjktMail               ,\n" +
        "Tel                        = ?prm_Tel                       ,\n" +
        "Fax                        = ?prm_Fax                       ,\n" +
        "Mail                       = ?prm_Mail                      ,\n" +
        "IsVisibleTT                = ?prm_IsVisibleTT               ,\n" +
        "IsVisibleAdress            = ?prm_IsVisibleAdress           ,\n" +
        "IsUserSastavio             = ?prm_IsUserSastavio            ,\n" +
        "IsOtsAnalitKontre          = ?prm_IsOtsAnalitKontre         ,\n" +
        "IsOtsDospjecaPoDan         = ?prm_IsOtsDospjecaPoDan        ,\n" +
        "IsOtsKontakt               = ?prm_IsOtsKontakt              ,\n" +
        "IsOtsLineTipBr             = ?prm_IsOtsLineTipBr            ,\n" +
        "IsOtsDospOnly              = ?prm_IsOtsDospOnly             ,\n" +
        "KompenzacijaBroj           = ?prm_KompenzacijaBroj          ,\n" +
        "ProjektCD                  = ?prm_ProjektCD                 ,\n" +
        "OtsSaldoKompenzacijaAsText = ?prm_OtsSaldoKompenzacijaAsText,\n" +
      //"OtsDate                    = ?prm_OtsDate                   ,\n" + // 04.12.2015: OtsDate spremati u macro je besmisleno. OtsDate je od sada FUSE 
        "NeedsOTSFormular           = ?prm_NeedsOTSFormular          ,\n" +
        "PdvR1                      = ?prm_PdvR1                     ,\n" +
        "IsPrihodTT                 = ?prm_IsPrihodTT                ,\n" +
        "OcuGraf                    = ?prm_OcuGraf                   ,\n" +
        "FuseStr1                   = ?prm_FuseStr1                  ,\n" +
        "FuseStr2                   = ?prm_FuseStr2                  ,\n" +
        "FuseStr3                   = ?prm_FuseStr3                  ,\n" +
        "FuseStr4                   = ?prm_FuseStr4                  ,\n" +
        "FuseBool1                  = ?prm_FuseBool1                 ,\n" +
        "FuseBool2                  = ?prm_FuseBool2                 ,\n" +
        "FuseBool3                  = ?prm_FuseBool3                 ,\n" +
        "FuseBool4                  = ?prm_FuseBool4                 ,\n" +
        "FuseDeciml1                = ?prm_FuseDeciml1               ,\n" +
        "FuseDeciml2                = ?prm_FuseDeciml2               ,\n" +
        "FuseDeciml3                = ?prm_FuseDeciml3               ,\n" +
        "FuseDeciml4                = ?prm_FuseDeciml4               ,\n" +
        "Date2                      = ?prm_Date2                     ,\n" +
        "Date3                      = ?prm_Date3                     ;\n" +   

         "SELECT @@IDENTITY"; // ovo ce pokupiti return od ExecuteScalar();

        VvSQL.CreateCommandParameter(cmd, "RecID"                      , riskMacro.          RecID                     , XSqlDbType.Int32, 10  );
        VvSQL.CreateCommandParameter(cmd, "MacroName"                  , riskMacro.          MacroName                 , XSqlDbType.String, 64  );
        VvSQL.CreateCommandParameter(cmd, "ReportZ"                    , riskMacro.          ReportZ                   , XSqlDbType.Int32, 3    );
        VvSQL.CreateCommandParameter(cmd, "UseMacroDates"              , riskMacro.          UseMacroDates             , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "DatumDo"                    , riskMacro.RptFilter.DatumDo                   , XSqlDbType.Date , 32   );
        VvSQL.CreateCommandParameter(cmd, "DatumOd"                    , riskMacro.RptFilter.DatumOd                   , XSqlDbType.Date , 32      );
        VvSQL.CreateCommandParameter(cmd, "ArtiklCdOD"                 , riskMacro.RptFilter.ArtiklCdOD                , XSqlDbType.String, 32    );
        VvSQL.CreateCommandParameter(cmd, "ArtiklCdDO"                 , riskMacro.RptFilter.ArtiklCdDO                , XSqlDbType.String, 32    );
        VvSQL.CreateCommandParameter(cmd, "ArtNameOD"                  , riskMacro.RptFilter.ArtNameOD                 , XSqlDbType.String, 80    );
        VvSQL.CreateCommandParameter(cmd, "ArtNameDO"                  , riskMacro.RptFilter.ArtNameDO                 , XSqlDbType.String, 80    );
        VvSQL.CreateCommandParameter(cmd, "SviArtikli"                 , riskMacro.RptFilter.SviArtikli                , XSqlDbType.Int16, 1      );
        VvSQL.CreateCommandParameter(cmd, "IznosOd"                    , riskMacro.RptFilter.IznosOd                   , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "IznosDo"                    , riskMacro.RptFilter.IznosDo                   , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "KD_naziv"                   , riskMacro.RptFilter.KD_naziv                  , XSqlDbType.String, 50   );
        VvSQL.CreateCommandParameter(cmd, "KD_ticker"                  , riskMacro.RptFilter.KD_ticker                 , XSqlDbType.String, 6      );
        VvSQL.CreateCommandParameter(cmd, "KD_sifra"                   , riskMacro.RptFilter.KD_sifra                  , XSqlDbType.Int32 ,  6     );
        VvSQL.CreateCommandParameter(cmd, "TT"                         , riskMacro.RptFilter.TT                        , XSqlDbType.String, 3       );
        VvSQL.CreateCommandParameter(cmd, "TtNumOd"                    , riskMacro.RptFilter.TtNumOd                   , XSqlDbType.Int32, 10       );
        VvSQL.CreateCommandParameter(cmd, "TtNumDo"                    , riskMacro.RptFilter.TtNumDo                   , XSqlDbType.Int32, 10       );
        VvSQL.CreateCommandParameter(cmd, "MT_naziv"                   , riskMacro.RptFilter.MT_naziv                  , XSqlDbType.String, 50   );
        VvSQL.CreateCommandParameter(cmd, "MT_ticker"                  , riskMacro.RptFilter.MT_ticker                 , XSqlDbType.String, 6       );
        VvSQL.CreateCommandParameter(cmd, "MT_sifra"                   , riskMacro.RptFilter.MT_sifra                  , XSqlDbType.Int32, 6       );
        VvSQL.CreateCommandParameter(cmd, "SkladCD"                    , riskMacro.RptFilter.SkladCD                   , XSqlDbType.String, 6    );
        VvSQL.CreateCommandParameter(cmd, "Napomena"                   , riskMacro.RptFilter.Napomena                  , XSqlDbType.String, 256  );
        VvSQL.CreateCommandParameter(cmd, "GrupaKupDob"                , riskMacro.RptFilter.GrupaKupDob               , XSqlDbType.String, 4       );
        VvSQL.CreateCommandParameter(cmd, "NacPlac"                    , riskMacro.RptFilter.NacPlac                   , XSqlDbType.String, 24   );
        VvSQL.CreateCommandParameter(cmd, "VezniDok"                   , riskMacro.RptFilter.VezniDok                  , XSqlDbType.String, 32   );
      //VvSQL.CreateCommandParameter(cmd, "DokNumOd"                   , riskMacro.RptFilter.DokNumOd                  , XSqlDbType.Int32, 10       ); // 04.12.2015: DokNumOdDo spremati u macro je besmisleno. DokNumOd je od sada PutnikPersonCD 
        VvSQL.CreateCommandParameter(cmd, "DokNumOd"                   , riskMacro.RptFilter.Putnik_PersonCD           , XSqlDbType.Int32, 10       ); // 04.12.2015: DokNumOdDo spremati u macro je besmisleno. DokNumOd je od sada PutnikPersonCD 
      //VvSQL.CreateCommandParameter(cmd, "DokNumDo"                   , riskMacro.RptFilter.DokNumDo                  , XSqlDbType.Int32, 10       ); // 04.12.2015: DokNumOdDo spremati u macro je besmisleno. DokNumDo je od sada FUSE           
        VvSQL.CreateCommandParameter(cmd, "GrupiranjeDokum"            , riskMacro.RptFilter.GrupiranjeDokum           , XSqlDbType.String, 32   );
        VvSQL.CreateCommandParameter(cmd, "GrupiranjeArtikla"          , riskMacro.RptFilter.GrupiranjeArtikla         , XSqlDbType.String, 32   );
        VvSQL.CreateCommandParameter(cmd, "AnalitSintet"               , riskMacro.RptFilter.AnalitSintet              , XSqlDbType.String, 1       );
        VvSQL.CreateCommandParameter(cmd, "AnaGrupaPoStranici"         , riskMacro.RptFilter.AnaGrupaPoStranici        , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "VisiblePostoGrupFooter"     , riskMacro.RptFilter.VisiblePostoGrupFooter    , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "VisibleOnlyTopGroups"       , riskMacro.RptFilter.VisibleOnlyTopGroups      , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "NumOfTopGroups"             , riskMacro.RptFilter.NumOfTopGroups            , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "TopSort"                    , riskMacro.RptFilter.TopSort                   , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "PdvKnjiga"                  , riskMacro.RptFilter.PdvKnjiga                 , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "PdvKredit"                  , riskMacro.RptFilter.PdvKredit                 , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvPovrat"                  , riskMacro.RptFilter.PdvPovrat                 , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvPredujam"                , riskMacro.RptFilter.PdvPredujam               , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvUstup"                   , riskMacro.RptFilter.PdvUstup                  , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "ImaPorezZastupnika"         , riskMacro.RptFilter.ImaPorezZastupnika        , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsAutoPorezniKredit"        , riskMacro.RptFilter.IsAutoPorezniKredit       , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "NacinPlacanja"              , riskMacro.RptFilter.NacinPlacanja             , XSqlDbType.String, 24   );
        VvSQL.CreateCommandParameter(cmd, "PdvF_Osn"                   , riskMacro.RptFilter.PdvF_Osn                  , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvF_Pdv"                   , riskMacro.RptFilter.PdvF_Pdv                  , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvIspravak"                , riskMacro.RptFilter.PdvIspravak               , XSqlDbType.Decimal, 12 );
        VvSQL.CreateCommandParameter(cmd, "PdvObrSastavio"             , riskMacro.RptFilter.PdvObrSastavio            , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "IsPrjktTel"                 , riskMacro.RptFilter.IsPrjktTel                , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsPrjktFax"                 , riskMacro.RptFilter.IsPrjktFax                , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsPrjktMail"                , riskMacro.RptFilter.IsPrjktMail               , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "Tel"                        , riskMacro.RptFilter.Tel                       , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "Fax"                        , riskMacro.RptFilter.Fax                       , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "Mail"                       , riskMacro.RptFilter.Mail                      , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "IsVisibleTT"                , riskMacro.RptFilter.IsVisibleTT               , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsVisibleAdress"            , riskMacro.RptFilter.IsVisibleAdress           , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsUserSastavio"             , riskMacro.RptFilter.IsUserSastavio            , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsOtsAnalitKontre"          , riskMacro.RptFilter.IsOtsAnalitKontre         , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsOtsDospjecaPoDan"         , riskMacro.RptFilter.IsOtsDospjecaPoDan        , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsOtsKontakt"               , riskMacro.RptFilter.IsOtsKontakt              , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsOtsLineTipBr"             , riskMacro.RptFilter.IsOtsLineTipBr            , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsOtsDospOnly"              , riskMacro.RptFilter.IsOtsDospOnly             , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "KompenzacijaBroj"           , riskMacro.RptFilter.KompenzacijaBroj          , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "ProjektCD"                  , riskMacro.RptFilter.ProjektCD                 , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "OtsSaldoKompenzacijaAsText" , riskMacro.RptFilter.OtsSaldoKompenzacijaAsText, XSqlDbType.String, 64   );
      //VvSQL.CreateCommandParameter(cmd, "OtsDate"                    , riskMacro.RptFilter.OtsDate                   , XSqlDbType.Date  , 32   ); // 04.12.2015: OtsDate spremati u macro je besmisleno. OtsDate je od sada FUSE 
        VvSQL.CreateCommandParameter(cmd, "NeedsOTSFormular"           , riskMacro.RptFilter.NeedsOTSFormular          , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "PdvR1"                      , riskMacro.RptFilter.PdvR12                    , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "IsPrihodTT"                 , riskMacro.RptFilter.IsPrihodTT                , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "OcuGraf"                    , riskMacro.RptFilter.OcuGraf                   , XSqlDbType.Int16, 1    );
        VvSQL.CreateCommandParameter(cmd, "FuseStr1"                   , riskMacro.RptFilter.FuseStr1                  , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "FuseStr2"                   , riskMacro.RptFilter.FuseStr2                  , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "FuseStr3"                   , riskMacro.RptFilter.FuseStr3                  , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "FuseStr4"                   , riskMacro.RptFilter.FuseStr4                  , XSqlDbType.String, 64   );
        VvSQL.CreateCommandParameter(cmd, "FuseBool1"                  , riskMacro.RptFilter.FuseBool1                 , XSqlDbType.Int16, 1     );
        VvSQL.CreateCommandParameter(cmd, "FuseBool2"                  , riskMacro.RptFilter.FuseBool2                 , XSqlDbType.Int16, 1     );
        VvSQL.CreateCommandParameter(cmd, "FuseBool3"                  , riskMacro.RptFilter.FuseBool3                 , XSqlDbType.Int16, 1     );
        VvSQL.CreateCommandParameter(cmd, "FuseBool4"                  , riskMacro.RptFilter.FuseBool4                 , XSqlDbType.Int16, 1     );
        VvSQL.CreateCommandParameter(cmd, "FuseDeciml1"                , riskMacro.RptFilter.FuseDeciml1               , XSqlDbType.Decimal, 12  );
        VvSQL.CreateCommandParameter(cmd, "FuseDeciml2"                , riskMacro.RptFilter.FuseDeciml2               , XSqlDbType.Decimal, 12  );
        VvSQL.CreateCommandParameter(cmd, "FuseDeciml3"                , riskMacro.RptFilter.FuseDeciml3               , XSqlDbType.Decimal, 12  );
        VvSQL.CreateCommandParameter(cmd, "FuseDeciml4"                , riskMacro.RptFilter.FuseDeciml4               , XSqlDbType.Decimal, 12  );
        VvSQL.CreateCommandParameter(cmd, "Date2"                      , riskMacro.RptFilter.Date2                     , XSqlDbType.Date , 32   );
        VvSQL.CreateCommandParameter(cmd, "Date3"                      , riskMacro.RptFilter.Date3                     , XSqlDbType.Date , 32   );

      return (cmd);
   }

   #endregion VvRiskReportMacro

   #region User Administration (ADD, RWT, DEL)

   public static bool DropUser(XSqlConnection conn, User user_rec)
   {
      bool success = true;
      int nora = -1; // number of rows affected 

      using(XSqlCommand cmd = VvSQL.DropUser_Command(conn, user_rec))
      {
         try
         {
            nora = cmd.ExecuteNonQuery();
         }
         catch(XSqlException ex)
         {
            VvSQL.ReportSqlError("DROP USER " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK);
            success = false;
         }
         catch(Exception ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            success = false;
         }

      } // using

      return (success);
   }

   public static XSqlCommand DropUser_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DROP USER '" + user_rec.UserName4Sql + "' ";

      return (cmd);
   }

   //###################################################################################################################### 
   //###################################################################################################################### 
   //###################################################################################################################### 

   public static bool CreateUser(XSqlConnection conn, User user_rec)
   {
      bool success = true; int nora = -1; // number of rows affected 

      using(XSqlCommand cmd = VvSQL.CreateUser_Command(conn, user_rec))
      {
         try                      { nora = cmd.ExecuteNonQuery(); }
         catch(XSqlException ex)  { VvSQL.ReportSqlError("CREATE USER " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
         catch(Exception ex)      { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }

      } // using

      if(success == false) return false; nora = -1;

      using(XSqlCommand cmd = VvSQL.GrantPrivileges_Command(conn, user_rec))
      {
         try                     { nora = cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { VvSQL.ReportSqlError("GRANT PRIVILEGES for " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
         catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }

      } // using

      if(success == false) return false; nora = -1;

      // 15.11.2014: 
      if(user_rec.UserName == ZXC.vvDB_programSuperUserName || user_rec.IsSuper)
      {
         using(XSqlCommand cmd = VvSQL.GrantPrivilegesWgrant_Command(conn, user_rec))
         {
            try                     { nora = cmd.ExecuteNonQuery(); }
            catch(XSqlException ex) { VvSQL.ReportSqlError("GRANT PRIVILEGES WITH GRANT for " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
            catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }

         } // using

         if(success == false) return false; nora = -1;

         if(user_rec.UserName == ZXC.vvDB_programSuperUserName/* || user_rec.IsSuper*/)
         {
            using(XSqlCommand cmd = VvSQL.CreateAndGrantPrivileges4vvbackuper_Command(conn))
            {
               try                     { nora = cmd.ExecuteNonQuery(); }
               catch(XSqlException ex) { VvSQL.ReportSqlError("CreateAndGrantPrivileges4vvbackuper_Command ", ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
               catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }

            } // using
         }

         if(success == false) return false; nora = -1;

         using(XSqlCommand cmd = VvSQL.FLUSH_PRIVILEGES_Command(conn))
         {
            try                     { nora = cmd.ExecuteNonQuery(); }
            catch(XSqlException ex) { VvSQL.ReportSqlError("CreateAndGrantPrivileges4vvbackuper_Command ", ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
            catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }

         } // using

      } // if(user_rec.UserName == ZXC.vvDB_programSuperUserName || user_rec.IsSuper) 
      
      return (success);
   }

   public static XSqlCommand CreateUser_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      //string decodedPasswd = ZXC.NotEmpty(user_rec.Passwd) ? VvAES.DecryptData(user_rec.Passwd, ZXC.vv_AES_key) : "";
      // namjerno, ocemo onemoguciti da user sa svojim passwordom moze uci u MySql client, moze samo preko Vektor-a         
      string decodedPasswd = user_rec.PasswdEncodedAsInFile;

      cmd.CommandText = "CREATE USER '" + user_rec.UserName4Sql +
                        (decodedPasswd.NotEmpty() ? "' IDENTIFIED BY '" + decodedPasswd + "'" : "'");

      return (cmd);
   }

   public static XSqlCommand GrantPrivileges_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      // 27.02.2024: 
    //cmd.CommandText = "GRANT ALL ON `" + ZXC.TheVvForm.GetvvDB_prefix() + "%`.* TO "  + user_rec.UserName4Sql      ;
      cmd.CommandText = "GRANT ALL ON `" + ZXC.TheVvForm.GetvvDB_prefix() + "%`.* TO '" + user_rec.UserName4Sql + "'";

      return (cmd);
   }

   public static XSqlCommand GrantPrivilegesWgrant_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "GRANT ALL PRIVILEGES ON *.* TO " + user_rec.UserName4Sql + " WITH GRANT OPTION ";

      return (cmd);
   }

   public static XSqlCommand CreateAndGrantPrivileges4vvbackuper_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = @"GRANT SHOW DATABASES, SELECT, LOCK TABLES, RELOAD ON *.* to vvbackuper@localhost IDENTIFIED BY 'vvbackuper' ";

      return (cmd);
   }

   public static XSqlCommand FLUSH_PRIVILEGES_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = @"FLUSH PRIVILEGES";

      return (cmd);
   }

   //###################################################################################################################### 
   //###################################################################################################################### 
   //###################################################################################################################### 

   public static bool RenameUserAndSetPassword(XSqlConnection conn, User user_rec)
   {
      bool success = true;
      int nora = -1; // number of rows affected 

      if(user_rec.BackupedUserName4Sql != user_rec.UserName4Sql) // dakle userName je promijenjen 
      {
         using(XSqlCommand cmd = VvSQL.RenameUser_Command(conn, user_rec))
         {
            try                     { nora = cmd.ExecuteNonQuery(); }
            catch(XSqlException ex) { VvSQL.ReportSqlError("RenameUser " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
            catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }
         } // using
      }

      if(success == false) return false;

      nora = -1;

      using(XSqlCommand cmd = VvSQL.SetPassword_Command(conn, user_rec))
      {
         try                     { nora = cmd.ExecuteNonQuery(); }
         catch(XSqlException ex) { VvSQL.ReportSqlError("SetPassword " + user_rec.UserName4Sql, ex, System.Windows.Forms.MessageBoxButtons.OK); success = false; }
         catch(Exception ex)     { System.Windows.Forms.MessageBox.Show(ex.Message); success = false; }
      } // using

      return(success);
   }

   public static XSqlCommand RenameUser_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "RENAME USER '" + user_rec.BackupedUserName4Sql + "' TO '" + user_rec.UserName4Sql + "' ";

      return (cmd);
   }

   public static XSqlCommand SetPassword_Command(XSqlConnection conn, User user_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      //string decodedPasswd = ZXC.NotEmpty(user_rec.Passwd) ? VvAES.DecryptData(user_rec.Passwd, ZXC.vv_AES_key) : "";
      // namjerno, ocemo onemoguciti da user sa svojim passwordom moze uci u MySql client, moze samo preko Vektor-a         
      string decodedPasswd = user_rec.PasswdEncodedAsInFile;

      cmd.CommandText = "SET PASSWORD FOR '" + user_rec.UserName4Sql + "' = PASSWORD('" + decodedPasswd + "')";

      return(cmd);
   }

   #endregion User Administration (ADD, RWT, DEL)

   #region CountRecords Command

   internal static XSqlCommand CountAnyRecords_Command(XSqlConnection conn, string tableName)
   {
      XSqlCommand cmd   = InitCommand(conn);

      cmd.CommandText = "SELECT COUNT(*) FROM " + tableName;

      return(cmd);
   }

   internal static XSqlCommand CountRecords_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers)
   {
      if(filterMembers.Count < 1) throw new Exception(string.Format("CountRecords_Command: filterMembers.Count: <{0}>!!!", filterMembers.Count));

      XSqlCommand cmd = InitCommand(conn);
      string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(filterMembers[0].drSchema["BaseTableName"]));

      cmd.CommandText = "SELECT COUNT(*) FROM " + recordName + VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand GetMaxUsedValueFromSomeColumn_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, string colName)
   {
      if(filterMembers.Count < 1) throw new Exception(string.Format("GetMaxUsedValueFromUintColumn_Command: filterMembers.Count: <{0}>!!!", filterMembers.Count));

      XSqlCommand cmd = InitCommand(conn);
      string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(filterMembers[0].drSchema["BaseTableName"]));

      cmd.CommandText = "SELECT MAX(" + colName + ") FROM " + recordName + VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   internal static XSqlCommand CountArhivedVersions_Command(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      XSqlCommand cmd   = InitCommand(conn);

      string tableName = vvDataRecord.VirtualRecordNameArhiva;

      cmd.CommandText = "SELECT COUNT(*) FROM " + tableName + " WHERE origRecID = " + vvDataRecord.VirtualRecID;

      return(cmd);
   }

   internal static XSqlCommand CountActiveVersions_Command(XSqlConnection conn, VvDataRecord arhivedDataRecord) // moze biti samo 0 ili 1 
   {
      XSqlCommand cmd = InitCommand(conn);

      string tableName = arhivedDataRecord.VirtualRecordName;

      cmd.CommandText = "SELECT COUNT(*) FROM " + tableName + " WHERE recID = " + arhivedDataRecord.TheArhivaData._origRecID;

      return (cmd);
   }

   internal static XSqlCommand Count_PRV_NXT_ArhivedVersions_Command(XSqlConnection conn, VvDataRecord arhivedDataRecord, DBNavigActionType direction)
   {
      XSqlCommand cmd = InitCommand(conn);
      string comparer = "";

      if     (direction == DBNavigActionType.PRV) comparer = " < ";
      else if(direction == DBNavigActionType.NXT) comparer = " > ";

      string tableName = arhivedDataRecord.VirtualRecordNameArhiva;

      cmd.CommandText = "SELECT COUNT(*) FROM " + tableName + " WHERE origRecID = " + arhivedDataRecord.TheArhivaData._origRecID + 
                        " AND recVer " + comparer + arhivedDataRecord.TheArhivaData._recVer;

      return (cmd);
   }

   #endregion CountRecords Command

   #region RenameForeignKey Command

   internal static XSqlCommand RenameForeignKey_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, DataRow drSchemaOfColToBeChanged, string newValue)
   {
      if(filterMembers.Count < 1) throw new Exception(string.Format("RenameForeignKey_Command: filterMembers.Count: <{0}>!!!", filterMembers.Count));

      XSqlCommand cmd = InitCommand(conn);

      // 11.10.2013: 
    //string recordName = (string)(drSchemaOfColToBeChanged["BaseTableName"]);
      string recordName =((string)(drSchemaOfColToBeChanged["BaseTableName"])).Replace(VvDataRecord.ArhRecNameExstension, "");
      string columnName = (string)(drSchemaOfColToBeChanged["ColumnName"]);
      string paramName  = "foreignData";

      cmd.CommandText = "UPDATE " + recordName + " SET " + columnName + " = " + "?new_" + paramName + VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);

      // za "?new_": 
      CreateCommandNamedParameter(cmd, "new_", paramName, newValue, drSchemaOfColToBeChanged); 
      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   #endregion RenameForeignKey Command

   #region VvDataRecord CalcStatus Commands

   #region GetKplan_SUM Command

   internal static XSqlCommand GetKplan_SUM_Command(XSqlConnection conn, ZXC.SaldoOrDugOrPot saldoORdugORpot, List<VvSqlFilterMember> filterMembers)
   {
      XSqlCommand cmd = InitCommand(conn);

      if(saldoORdugORpot == ZXC.SaldoOrDugOrPot.SALDO)
      {
         cmd.CommandText = "SELECT SUM(t_dug - t_pot) FROM " + Ftrans.recordName + VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);
      }
      else
      {
         string colName = (saldoORdugORpot == ZXC.SaldoOrDugOrPot.DUG ? "t_dug" : "t_pot");

         cmd.CommandText = "SELECT SUM(" + colName + ") FROM " + Ftrans.recordName + VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false);
      }

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return(cmd);
   }

   #endregion GetKplan_SUM Command

   #region GetOsredStatusList_SUM_Command

   internal static XSqlCommand GetOsredStatusList_SUM_Command(XSqlConnection conn, List<VvSqlFilterMember> filterMembers, DateTime _dateAmRazdobljeStart)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =
         
         "SELECT t_osredCD, \n" +

         VvOsredReport.Rule_IsRashodovan +

         "SUM" + VvOsredReport.Rule_InvSt +
         "SUM" + VvOsredReport.Rule_KolSt +
         "SUM" + VvOsredReport.Rule_UkNabDug +
         "SUM" + VvOsredReport.Rule_UkNabPot +
         "SUM" + VvOsredReport.Rule_UkRasDug +
         "SUM" + VvOsredReport.Rule_UkRasPot +
         "SUM" + VvOsredReport.Rule_OldAmDug +
         "SUM" + VvOsredReport.Rule_OldAmPot +
         "SUM" + VvOsredReport.Rule_NewAmDug +
         "SUM" + VvOsredReport.Rule_NewAmPot +

         "FROM " + Atrans.recordName + " \n" +
         
         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) +

         "GROUP BY " + Atrans.OsredForeignKey + "\n";

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      // jos jemput rucno za parametar '?dateStartAm' 
      DataRow drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_dokDate];
      CreateCommandNamedParameter(cmd, "filter_", "dateStartAm", _dateAmRazdobljeStart, drSchema);

      return(cmd);
   }

   #endregion GetOsredStatusList_SUM_Command

   #region The ARTIKL STATUS CACHE! OvoOno_Commands

   internal static XSqlCommand CountMissingArtstat_Command(XSqlConnection conn, string artstatInfluencerTTList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT COUNT(*)" + "\n" +

         From_Join_Where_ClauseForMissingArtstatCommand(artstatInfluencerTTList);

      return (cmd);
   }

   internal static XSqlCommand Count_BadMSU_ArtstatNabCij_Command(XSqlConnection conn, string twinUlazTTList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT COUNT(*)" + "\n" +

         From_Join_Where_ClauseForBadMSU_ArtstatNabCijCommand(twinUlazTTList);

      return (cmd);
   }

   internal static XSqlCommand CountDescrepanciesInPrNabDokCij_Command(XSqlConnection conn, string dokCijShouldBePrNabCijTTList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT COUNT(*)" + "\n" +

         From_Join_Where_ClauseForDescrepanciesInPrNabDokCijCommand(dokCijShouldBePrNabCijTTList);

      return (cmd);
   }

   internal static XSqlCommand DistinctListOfMissingArtstat_Command(XSqlConnection conn, string artstatInfluencerList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT DISTINCT R.t_artiklCD, R.t_skladCD" + "\n" +

         From_Join_Where_ClauseForMissingArtstatCommand(artstatInfluencerList);

      return (cmd);
   }

   internal static XSqlCommand DistinctListOfBadMSU_ArtstatNabCij_Command(XSqlConnection conn, string twinUlazTTList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

       //"SELECT DISTINCT R.t_artiklCD, R.t_skladCD" + "\n" +
         "SELECT * "                                 + "\n" +

         From_Join_Where_ClauseForBadMSU_ArtstatNabCijCommand(twinUlazTTList);

      return (cmd);
   }

   private static string From_Join_Where_ClauseForMissingArtstatCommand(string artstatInfluencerList)
   {
      return
         "FROM      rtrans  R                   " + "\n" +
         "LEFT JOIN artstat A                   " + "\n" +
         "ON R.recID = A.rtransRecID            " + "\n" +
         "WHERE A.recID IS NULL                 " + "\n" +
       //"AND   LOWER(R.t_artiklCD) != 'qwe'    "    + "\n" +
         "AND R.t_artiklCD != ''                " + "\n" +
         // 11.07.2015: 
         // 12.07.2015: ipak NE! 
       //"AND SUBSTRING(R.t_ttNUm, 4) != '000'  " + "\n" + // skip nulte ZPC-ove 
         "AND R.t_tt IN " + artstatInfluencerList + "\n\n" +

      // 22.02.2024: HUGE NEWS!!!                                                                             
      // Pojavom UgAnDo-a, u fajlu ce postojati TT-ovi - ArtStat influenceri koji se 'vuku' iz proslih godina 
      // pa ih treba odavdje amputirati jer pocetno stanje vec donosi njihov utjecaj iz proslosti             
      // pa bi se odervajs to dupliralo                                                                       
         " AND R.t_skladDate >= '" + ZXC.projectYearFirstDay.ToString(ZXC.VvDateYyyyMmDdMySQLFormat) + "'\n\n";
   }

   private static string From_Join_Where_ClauseForBadMSU_ArtstatNabCijCommand(string twinUlazTTList)
   {
      return
         "FROM      rtrans  R                                 "  + "\n" +
         "LEFT JOIN artstat A                                 "  + "\n" +
         "ON R.recID = A.rtransRecID                          "  + "\n" +
       //"WHERE  ABS(r.t_kol*(r.t_cij - a.rtrCijenaNBC)) > 0.15" + "\n" +
       //"WHERE  ABS(r.t_kol*(r.t_cij - a.rtrCijenaNBC)) > 0.30" + "\n" +
         "WHERE (ABS(r.t_kol*(r.t_cij - a.rtrCijenaNBC)) > 0.30" + "\n" +
         "OR A.RecID IS NULL)                                 "  + "\n" +
         "AND R.t_artiklCD != ''                              "  + "\n" +
         "AND R.t_tt IN                                       "  + twinUlazTTList + " ";
   }

   private static string From_Join_Where_ClauseForDescrepanciesInPrNabDokCijCommand(string dokCijShouldBePrNabCijTTList)
   {
      return
         "FROM      rtrans  R                   "     + "\n" +
         "LEFT JOIN artstat A                   "     + "\n" +
         "ON R.recID = A.rtransRecID            "     + "\n" +
      // 14.11.2014: dizalo nepotrebno npr 6.4444 vs 6.4445 => 6.444 != 6.445
       //"WHERE ROUND(t_cij, 3) != ROUND(lastPrNabCij, 3)\n" +
       //"WHERE ROUND(t_cij, 2) != ROUND(lastPrNabCij, 2)\n" +
         "WHERE ABS  (t_cij - lastPrNabCij) > " + RtransDao.ChkPrNbC_diff_tolerancy + " \n" +
         "AND R.t_artiklCD != ''                "     + "\n" +
         "AND R.t_tt IN " + dokCijShouldBePrNabCijTTList + " ";
   }

   internal static XSqlCommand GetPulRtranses_HavingDescrepancies_Command(XSqlConnection conn, string pulxTT, string pizxTT, bool isByGR)
   {
      XSqlCommand cmd = InitCommand(conn);

      string pulxRtransesSubQuery = isByGR ? GetPulxRtransesSubQuery_ByGR(pulxTT, pizxTT) : GetPulxRtransesSubQuery(pulxTT, pizxTT);

      string eventualJIONclause = "LEFT JOIN " + Artikl.recordName + " a1 ON r1.t_artiklCD = a1.artiklCD";

      cmd.CommandText =

      "SELECT r1.*, " + "\n" +
      pulxRtransesSubQuery + "\n" +
      "FROM " + Rtrans.recordName + " r1 " + "\n" +
      (isByGR ? eventualJIONclause : "") + "\n" +
      "WHERE r1.t_tt = '" + pulxTT + "' " + "\n" +
      "AND ABS (r1.t_cij - " + pulxRtransesSubQuery + ") > " + RtransDao.ChkPrNbC_diff_tolerancy + " ";

      return (cmd);
   }

   // !!! PAZI !!! BE ADVICED !!! TODO: !!!!!!!!!!!! 
   // Metaflexu se rucno dodaje faktur index 'BY_projektCD' jerbo inace ovaj querry traje beskonacno s

   private static string GetPulxRtransesSubQuery(string pulxTT, string pizxTT)
   {
      return
      "("                                                                                                                                    + "\n" + 
      "  (SELECT SUM(t_kol * t_cij) FROM " + Rtrans.recordName + " r2 WHERE r1.t_parentID = r2.t_parentID AND   r2.t_tt = '" + pizxTT + "')" + "\n" + 
      "  /"                                                                                                                                  + "\n" + 
      "  (SELECT SUM(t_kol)         FROM " + Rtrans.recordName + " r3 WHERE r1.t_parentID = r3.t_parentID AND   r3.t_tt = '" + pulxTT + "')" + "\n" + 
      ")" + "\n";
   }

   private static string GetPulxRtransesSubQuery_ByGR(string pulxTT, string pizxTT)
   {
      return
      "("                                                                                                                                    + "\n" +
      "  (SELECT SUM(t_kol * t_cij) FROM " + Rtrans.recordName + " r2 LEFT JOIN " + Artikl.recordName + " a2 ON r2.t_artiklCD = a2.artiklCD WHERE r1.t_parentID = r2.t_parentID AND   r2.t_tt = '" + pizxTT + "' AND a2.grupa1CD = a1.grupa1CD)" + "\n" + 
      "  /"                                                                                                                                  + "\n" +
      "  (SELECT SUM(t_kol)         FROM " + Rtrans.recordName + " r3 LEFT JOIN " + Artikl.recordName + " a3 ON r3.t_artiklCD = a3.artiklCD WHERE r1.t_parentID = r3.t_parentID AND   r3.t_tt = '" + pulxTT + "' AND a3.grupa1CD = a1.grupa1CD)" + "\n" + 
      ")" + "\n";
   }

   #region Bef_TheCIJ 

   internal static XSqlCommand Bef_TheCIJ_GetPIP_Rtranses_HavingDescrepancies_Command(XSqlConnection conn, string pulxTT, string pizxTT, bool isByGR)
   {
      XSqlCommand cmd = InitCommand(conn);

      // dok ne zatreba isByGR, sa lijeve i desna strane od ':' je ISTA metoda! 
      string pipRtransesSubQuery = isByGR ? Bef_TheCIJ_GetPIP_RtransesSubQuery/*_ByGR*/(pulxTT, pizxTT) : Bef_TheCIJ_GetPIP_RtransesSubQuery(pulxTT, pizxTT);

      //string eventualJIONclause = "LEFT JOIN " + Artikl.recordName + " a1 ON r1.t_artiklCD = a1.artiklCD";

      cmd.CommandText =

      "SELECT r1.*, " + "\n" +
      pipRtransesSubQuery + "\n" +
      "FROM rtrans r1 LEFT JOIN faktur f1 ON f1.recID = r1.t_parentID" + "\n" +

#if !OldPIP_2016
      // PIP 2017 
      "LEFT JOIN faktur theRNM  ON f1.projektCD = theRNM.projektCD AND theRNM.tt = 'RNM'" + "\n" + // ovo je NOVO 26.01.2017 
      "LEFT JOIN faktEx thexRNM ON theRNM.RecID = thexRNM.fakturRecID "                   + "\n" + // ovo je NOVO 26.01.2017 
      // PIP 2017 
#endif
      
      "WHERE f1.tt = '" + pulxTT + "' AND f1.projektCD != ''" + "\n" +
      "AND ABS (r1.t_cij - " + pipRtransesSubQuery + ") > " + RtransDao.ChkPrNbC_diff_tolerancy + " ";

      return (cmd);
   }

#if !OldPIP_2016
   // PIP 2017 
   private static string Bef_TheCIJ_GetPIP_RtransesSubQuery/*_2017*/(string pulxTT, string pizxTT)
   {
      return
      "("             + "\n" +
      "  (SELECT (theRNM.Decimal01 + COALESCE(SUM(t_kol * t_cij), 0)) * \n" +
      KoeficijentDovrsenostiSubQuery      +
      "   FROM rtrans r2 LEFT JOIN faktur f2 ON f2.recID = r2.t_parentID WHERE f1.projektCD = f2.projektCD AND f2.tt = '" + pizxTT + "')" + "\n" + 
      "  /"           + "\n" +
      "#_NAZIVNIK___ START ______________________________________________________________________________________\n" +
      "  (theRNM.Decimal02 + (SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r3 LEFT JOIN faktur f3 ON f3.recID = r3.t_parentID WHERE f1.projektCD = f3.projektCD AND f3.tt = '" + pulxTT + "'))" + "\n" + 
      "#_NAZIVNIK___  END ______________________________________________________________________________________ \n" +
      "  * t_ppmvOsn" + "\n" +
      ")"             + "\n" ;
   }
#else
   // PIP 2016 
   private static string GetPIP_RtransesSubQuery/*_BEF_2017*/(string pulxTT, string pizxTT)
   {
      return
      "("                                                                                                                                    + "\n" + 
      "  (SELECT SUM(t_kol * t_cij) FROM rtrans r2 LEFT JOIN faktur f2 ON f2.recID = r2.t_parentID WHERE f1.projektCD = f2.projektCD AND f2.tt = '" + pizxTT + "')" + "\n" + 
      "  /"                                                                                                                                  + "\n" + 
      "  (SELECT SUM(t_kol)         FROM rtrans r3 LEFT JOIN faktur f3 ON f3.recID = r3.t_parentID WHERE f1.projektCD = f3.projektCD AND f3.tt = '" + pulxTT + "')" + "\n" + 
      ")" + "\n";
   }
#endif

   private static string KoeficijentDovrsenostiSubQuery { get { return
"\n#koefDovrsenosti ___ START ___ \n" +
"   IF(thexRNM.StatusCD = 'R', 1, # koef = 1 kada je status R, a ELSE ide ovaj brojnik: "                                                                                                            + "\n" +
"     ((SELECT theRNM.Decimal02 + SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r4 LEFT JOIN faktur f4 ON f4.recID = r4.t_parentID WHERE f1.projektCD = f4.projektCD AND f4.tt = 'PIP')" + "\n" +
"      /"                                                                                                                                                                                            + "\n" +
"      (SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r5 LEFT JOIN faktur fRNM5 ON fRNM5.recID = r5.t_parentID WHERE f1.projektCD = fRNM5.projektCD AND r5.t_tt = 'RNU')))"       + "\n" +
"#koefDovrsenosti ___  END  ___  \n\n";
      } }

   #endregion Bef_TheCIJ

   internal static XSqlCommand Get_RNU_PIP_Rtranses_MissingData_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT r.t_tt, r.t_ttNum                                                            \n" + // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
                                                                                                    // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
         "FROM rtrans r LEFT JOIN artikl a                                                    \n" + // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
         "ON r.t_artiklCD = a.artiklCD                                                        \n" + // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
                                                                                                    // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
         "WHERE (r.t_tt = 'RNU' OR r.t_tt = 'PIP')                                            \n" + // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
                                                                                                    // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
         "AND   r.T_ppmvOsn != IF(COALESCE(a.masaNetto, 1) != 0, COALESCE(a.masaNetto, 1), 1) \n" + // Da li nekom RNU iil PIP rtransu nedostaje T_ppmvOsn 
         
         "UNION ALL                                                                           \n" + // UNION ALL                                           

         "SELECT f.Tt, f.TtNum FROM faktur f WHERE TT = 'RNM' AND ProjektCD = ''              \n" ; // Da li nekom RNM-u nedostaje ProjektCD               

      return (cmd);
   }

   private static string GetPIP_RtransesSubQuery_OG/*_2017*/()
   {
      return
"SELECT r1.*,                                                                                                                                                                                   " + "\n" +
"(                                                                                                                                                                                              " + "\n" +

(ZXC.projectYearFirstDay.Year != 2017 ?

"#_BROJNIK___ START ______________________________________________________________________________________                                                                                      " + "\n" +
"  (SELECT (COALESCE(SUM(t_kol * t_cij), 0)) *                                                                                                                                                  " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"   #koefDovrsenosti ___ START ___                                                                                                                                                              " + "\n" +
"      IF(thexRNM.StatusCD = 'R', 1, # koef = 1 kada je status R, a ELSE ide ovaj brojnik:                                                                                                      " + "\n" +
"        ((SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r4 LEFT JOIN faktur f4    ON f4   .recID = r4.t_parentID WHERE f1.projektCD = f4   .projektCD AND f4.tt   = 'PIP')  " + "\n" +
"         /                                                                                                                                                                                     " + "\n" +
"         (SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r5 LEFT JOIN faktur fRNM5 ON fRNM5.recID = r5.t_parentID WHERE f1.projektCD = fRNM5.projektCD AND r5.t_tt = 'RNU')))" + "\n" +
"   #koefDovrsenosti ___  END  ___                                                                                                                                                              " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"   FROM rtrans r2 LEFT JOIN faktur f2 ON f2.recID = r2.t_parentID WHERE f1.projektCD = f2.projektCD AND f2.tt = 'PPR') # ukFinIzlazOG                                                          " + "\n" +
"#_BROJNIK___  END ______________________________________________________________________________________                                                                                       " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"  /                                                                                                                                                                                            " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"#_NAZIVNIK___ ukKol_Kg_UlazOG ___ START ______________________________________________________________________________________                                                                 " + "\n" +
"  ((SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r3 LEFT JOIN faktur f3 ON f3.recID = r3.t_parentID WHERE f1.projektCD = f3.projektCD AND f3.tt = 'PIP'))                  " + "\n" +
"#_NAZIVNIK___ ukKol_Kg_UlazOG ___  END  ______________________________________________________________________________________                                                                 " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"  * t_ppmvOsn                                                                                                                                                                                                  " + "\n" +
"                                                                                                                                                                                                               " + "\n"
: "0") +

 ") AS TheCIJ                                                                                                                                                                                                    " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"FROM rtrans r1 LEFT JOIN faktur f1 ON f1.recID = r1.t_parentID                                                                                                                                 " + "\n" +
"LEFT JOIN faktur theRNM  ON f1.projektCD = theRNM.projektCD AND theRNM.tt = 'RNM'                                                                                                              " + "\n" +
"LEFT JOIN faktEx thexRNM ON theRNM.RecID = thexRNM.fakturRecID                                                                                                                                 " + "\n" +
"WHERE YEAR(theRNM.DokDate) = " + ZXC.projectYear + "                                                                                                                                           " + "\n" +
"AND f1.tt = 'PIP' AND f1.projektCD != ''                                                                                                                                                       " + "\n" +
"                                                                                                                                                                                               " + "\n" +
"HAVING ABS (r1.t_cij - TheCIJ) > " + RtransDao.ChkPrNbC_diff_tolerancy + "                                                                                                                     " + "\n";
   }

   private static string GetPIP_RtransesSubQuery_PG/*_2017*/()
   {
      return 
"SELECT r1.*,                                                                                                                                                                                                   " + "\n" +
"(                                                                                                                                                                                                              " + "\n" +

(ZXC.projectYearFirstDay.Year != 2017 ?

"#_BROJNIK___ START ______________________________________________________________________________________                                                                                                      " + "\n" +
"  (SELECT                                                                                                                                                                                                      " + "\n" +
"  (                                                                                                                                                                                                            " + "\n" +
"     theRNM.Decimal01                                                                                                                                                                                          " + "\n" +
"     -                                                                                                                                                                                                         " + "\n" +
"     (theRNM.Decimal01 *                                                                                                                                                                                       " + "\n" +
"        #koefDovrsenosti PG ___ START ___                                                                                                                                                                      " + "\n" +
"        (                                                                                                                                                                                                      " + "\n" +
"         theRNM.Decimal02                                                                                                                                                                                      " + "\n" +
"         /                                                                                                                                                                                                     " + "\n" +
"         (SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r5 LEFT JOIN faktur fRNM5 ON fRNM5.recID = r5.t_parentID WHERE f1.projektCD = fRNM5.projektCD AND r5.t_tt = 'RNU')                  " + "\n" +
"        )                                                                                                                                                                                                      " + "\n" +
"        #koefDovrsenosti PG ___  END  ___                                                                                                                                                                      " + "\n" +
"     )                                                                                                                                                                                                         " + "\n" +
"     +                                                                                                                                                                                                         " + "\n" +
"     (COALESCE(SUM(t_kol * t_cij), 0)) # ukFinIzlazOG                                                                                                                                                          " + "\n" +
"  )                                                                                                                                                                                                            " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"  *                                                                                                                                                                                                            " + "\n" +
"   #koefDovrsenosti OG ___ START ___                                                                                                                                                                           " + "\n" +
"   IF(thexRNM.StatusCD = 'R', 1, # koef = 1 kada je status R, a ELSE ide ovaj brojnik:                                                                                                                         " + "\n" +
"     ((SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1))                    FROM rtrans r4 LEFT JOIN faktur f4    ON f4   .recID = r4.t_parentID WHERE f1.projektCD = f4   .projektCD AND f4.tt   = 'PIP')  " + "\n" +
"      /                                                                                                                                                                                                        " + "\n" +
"      (SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) - theRNM.Decimal02 FROM rtrans r5 LEFT JOIN faktur fRNM5 ON fRNM5.recID = r5.t_parentID WHERE f1.projektCD = fRNM5.projektCD AND r5.t_tt = 'RNU')))" + "\n" +
"   #koefDovrsenosti OG ___  END  ___                                                                                                                                                                           " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"   FROM rtrans r2 LEFT JOIN faktur f2 ON f2.recID = r2.t_parentID WHERE f1.projektCD = f2.projektCD AND f2.tt = 'PPR') # ukFinIzlazOG                                                                          " + "\n" +
"#_BROJNIK___  END ______________________________________________________________________________________                                                                                                       " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"  /                                                                                                                                                                                                            " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"#_NAZIVNIK___ ukKol_Kg_UlazOG ___ START ______________________________________________________________________________________                                                                                 " + "\n" +
"  ((SELECT SUM(t_kol * IF(t_ppmvOsn != 0, t_ppmvOsn, 1)) FROM rtrans r3 LEFT JOIN faktur f3 ON f3.recID = r3.t_parentID WHERE f1.projektCD = f3.projektCD AND f3.tt = 'PIP'))                                  " + "\n" +
"#_NAZIVNIK___ ukKol_Kg_UlazOG ___  END  ______________________________________________________________________________________                                                                                 " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"  * t_ppmvOsn                                                                                                                                                                                                  " + "\n" +
"                                                                                                                                                                                                               " + "\n" 
: "0" ) +

 ") AS TheCIJ                                                                                                                                                                                                    " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"FROM rtrans r1 LEFT JOIN faktur f1 ON f1.recID = r1.t_parentID                                                                                                                                                 " + "\n" +
"LEFT JOIN faktur theRNM  ON f1.projektCD = theRNM.projektCD AND theRNM.tt = 'RNM'                                                                                                                              " + "\n" +
"LEFT JOIN faktEx thexRNM ON theRNM.RecID = thexRNM.fakturRecID                                                                                                                                                 " + "\n" +
"WHERE YEAR(theRNM.DokDate) != " + ZXC.projectYear + "                                                                                                                                                          " + "\n" +
"AND f1.tt = 'PIP' AND f1.projektCD != ''                                                                                                                                                                       " + "\n" +
"                                                                                                                                                                                                               " + "\n" +
"HAVING ABS (r1.t_cij - TheCIJ) > " + RtransDao.ChkPrNbC_diff_tolerancy + "                                                                                                                                     " + "\n";
   }

   internal static XSqlCommand GetPIP_Rtranses_HavingDescrepancies_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

             GetPIP_RtransesSubQuery_OG() + "\n" 
             +
             "UNION ALL                      \n" 
             +
             GetPIP_RtransesSubQuery_PG()      
             ;

      return (cmd);
   }







   // dis iz: "LSTSET + PRVSET, Get just one previous record" - PATTERN
   internal static XSqlCommand GetLastFromCache_Command(XSqlConnection conn, string _artiklCD, string _skladCD, DateTime _dateDo, short? _ttSort, uint? _ttNum, ushort? _serial, string orderBy)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.ArtStatDao.TheSchemaTable.Rows;
      ArtStatDao.ArtStatCI ci      = ZXC.AstCI;

      string part1, part2="", part3;

      bool isForKnownDate   = (_dateDo != DateTime.MinValue);
      bool isForExactRtrans = (_ttSort != null && _ttNum != null && _serial != null);

      #endregion local variablez

      //___ part1 ______________________________________________________________________________________________________________________
      part1 =
         "SELECT * FROM " + ArtStat.recordName + " WHERE \n\n" +

         "t_artiklCD = ?prm_t_artiklCD                   \n" +
         "AND                                            \n" +
         "t_skladCD  = ?prm_t_skladCD                    \n\n";

      VvSQL.CreateCommandParameter(cmd, preffix, _artiklCD, rows[ci.t_artiklCD]);
      VvSQL.CreateCommandParameter(cmd, preffix, _skladCD , rows[ci.t_skladCD ]);

      //___ part2 ______________________________________________________________________________________________________________________
      if(isForKnownDate == true) // Znaci, zadali smo dateDo, odervajs se podrazumijeva da nisu zadani niti ttSort, niti ttNum, niti serial 
      {
         VvSQL.CreateCommandParameter(cmd, preffix, _dateDo, rows[ci.t_skladDate]);

         if(isForExactRtrans == false) // Znaci, NISMO zadali egzaktni rtrans_rec, nego samo datum 
         {
            part2 =
               "AND \n" +
               "(\n" +
               "  (t_skladDate <= ?prm_t_skladDate) \n" +
               ")\n\n";
         }
         else // Znaci, zadali smo i egzaktni rtrans_rec, a ne samo datum; Zelimo otici na zadnjeg PRIJE zadanog rtrans_rec-a, ne ukljucujuci njega! 
         {
            VvSQL.CreateCommandParameter(cmd, preffix, _ttSort  , rows[ci.t_ttSort]);
            VvSQL.CreateCommandParameter(cmd, preffix, _ttNum   , rows[ci.t_ttNum ]);
            VvSQL.CreateCommandParameter(cmd, preffix, _serial  , rows[ci.t_serial]);
            part2 =
               "AND \n" +
               "(\n" +
               "  (t_skladDate < ?prm_t_skladDate)                                                                                      OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort < ?prm_t_ttSort)                                                         OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum < ?prm_t_ttNum)                              OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum = ?prm_t_ttNum AND t_serial < ?prm_t_serial)    \n" + // vidis, za serial '<' a ne '<=' 
               ")\n\n";
         }
      }

      //___ part3 ______________________________________________________________________________________________________________________
      part3 =
         "ORDER BY " + orderBy + " LIMIT 1"; // ovaj Replace ti umjesto svakog ASC stavi DESC na svaki segment orderBy_ASC-a 

      cmd.CommandText = part1 + part2 + part3;

      return (cmd);
   }

   // dis iz: "FRSSET + NXTSET based on foundInCacheRec or FRSSET if NOT foundInCache, and do something on that one and all consecutive records" - PATTERN
   internal static XSqlCommand GetRemainingRtransesForCalcArtiklStatus_Command(XSqlConnection conn, ArtStat artStat_rec, string selectColumns, string _artiklCD, string _skladCD, DateTime _dateDo, short? _ttSort, uint? _ttNum, ushort? _serial, string orderBy)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.RtransDao.TheSchemaTable.Rows;
      RtransDao.RtransCI   ci      = ZXC.RtrCI;

      string part1, part2="", part3="", part4;

      bool isForKnownDate    = (_dateDo != DateTime.MinValue);
      bool isForExactRtrans  = (_ttSort != null && _ttNum != null && _serial != null);
      bool isFoundInCache    = (artStat_rec != null);
      bool isFor_All_SkladCD = _skladCD.IsEmpty();
      bool isFor_One_SkladCD = _skladCD.NotEmpty();

      #endregion local variablez

      #region part1
      //___ part1 ______________________________________________________________________________________________________________________
      
      part1 =
         "SELECT " + selectColumns     + "\n" +
         "FROM "   + Rtrans.recordName + " WHERE \n\n" +

         "t_artiklCD = ?prm_t_artiklCD                   \n" +
         (isFor_One_SkladCD ?
         "AND                                            \n" +
         "t_skladCD  = ?prm_t_skladCD                    \n\n" : "") /*;*/ +

         " AND t_skladDate >= ?prm_projYearFirstDay \n\n";

      VvSQL.CreateCommandParameter(cmd, preffix, _artiklCD, rows[ci.t_artiklCD]);
      VvSQL.CreateCommandParameter(cmd, preffix, _skladCD , rows[ci.t_skladCD ]);

      // 22.02.2024: HUGE NEWS!!!                                                                             
      // Pojavom UgAnDo-a, u fajlu ce postojati TT-ovi - ArtStat influenceri koji se 'vuku' iz proslih godina 
      // pa ih treba odavdje amputirati jer pocetno stanje vec donosi njihov utjecaj iz proslosti             
      // pa bi se odervajs to dupliralo                                                                       

      VvSQL.CreateCommandParameter(cmd, preffix, "projYearFirstDay", ZXC.projectYearFirstDay, XSqlDbType.DateTime, 32);

      #endregion part1

      #region part2
      //___ part2 ______________________________________________________________________________________________________________________

      if(isForKnownDate == true) // Znaci, zadali smo dateDo, odervajs se podrazumijeva da nisu zadani niti ttSort, niti ttNum, niti serial 
      {
         VvSQL.CreateCommandParameter(cmd, preffix, _dateDo, rows[ci.t_skladDate]);

         if(isForExactRtrans == false) // Znaci, NISMO zadali egzaktni rtrans_rec, nego samo datum 
         {
            part2 =
               "AND \n" +
               "(\n" +
               "  (t_skladDate <= ?prm_t_skladDate) \n" +
               ")\n\n";
         }
         else // Znaci, zadali smo i egzaktni rtrans_rec, a ne samo datum; Zelimo otici na zadnjeg PRIJE zadanog rtrans_rec-a, ne ukljucujuci njega! 
         {
            VvSQL.CreateCommandParameter(cmd, preffix, _ttSort  , rows[ci.t_ttSort]);
            VvSQL.CreateCommandParameter(cmd, preffix, _ttNum   , rows[ci.t_ttNum ]);
            VvSQL.CreateCommandParameter(cmd, preffix, _serial  , rows[ci.t_serial]);
            part2 =
               "AND \n" +
               "(\n" +
               "  (t_skladDate < ?prm_t_skladDate)                                                                                      OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort < ?prm_t_ttSort)                                                         OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum < ?prm_t_ttNum)                              OR \n" +
               "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum = ?prm_t_ttNum AND t_serial < ?prm_t_serial)    \n" + // vidis, za serial '<' a ne '<=' 
               ")\n\n";
         }
      }
      #endregion part2

      #region part3
      //___ part3 ______________________________________________________________________________________________________________________

      if(isFoundInCache == true) // Znaci, dolazni artStat_rec NIJE null.  
      {
         VvSQL.CreateCommandParameter(cmd, "prm2_", artStat_rec.SkladDate, rows[ci.t_skladDate]);
         VvSQL.CreateCommandParameter(cmd, "prm2_", artStat_rec.TtSort   , rows[ci.t_ttSort   ]);
         VvSQL.CreateCommandParameter(cmd, "prm2_", artStat_rec.TtNum    , rows[ci.t_ttNum    ]);
         VvSQL.CreateCommandParameter(cmd, "prm2_", artStat_rec.Serial   , rows[ci.t_serial   ]);
         part3 =
            "AND \n" +
            "(\n" +
            "  (t_skladDate > ?prm2_t_skladDate)                                                                                          OR \n" +
            "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort > ?prm2_t_ttSort)                                                            OR \n" +
            "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum > ?prm2_t_ttNum)                                OR \n" +
            "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum = ?prm2_t_ttNum AND t_serial > ?prm2_t_serial)     \n" + // vidis, za serial '>' a ne '>=' 
            ")\n\n";
      }
      #endregion part3

      #region part4
      //___ part4 ______________________________________________________________________________________________________________________

      part4 =
         "ORDER BY " + orderBy;

      #endregion part4

      // part1: determinira SET (artiklCD + skladCD) ako je 'isFor_All_SkladCD == true' onda se skladCD ommit-a                                                      
      // part2: limitira do kuda se zeli doci (ili do kraja nekog datuma, ili do nekog egzaktnog Rtrans-a koji ostaje NEukljucen tj break PRIJE njega.               
      // part3: limitira od kuda krecemo racunati ArtiklStatus; ako dolazni 'artStat_rec' NIJE null onda od njegovog NXTSET-a, a ako JE null onda od pocetka fajla   
      // part4: order by                                                                                                                                             

      cmd.CommandText = part1 + part2 + part3 + part4;

      return (cmd);
   }

   // dis iz: "FRSSET + NXTSET and do something on all consecutive records" - PATTERN
   internal static XSqlCommand GetFirst_IzlazOrUlaz_OnToOtherSkladCD_Command(XSqlConnection conn, string otherSkladCD, Rtrans rtrans_rec, bool isULAZinstedOfIZLAZ)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.RtransDao.TheSchemaTable.Rows;
      RtransDao.RtransCI   ci      = ZXC.RtrCI;

      string part1, part2 = "", part3 = "", part4;

      #endregion local variablez

      #region part1
      //___ part1 ______________________________________________________________________________________________________________________
      
      part1 =
         "SELECT "    + " * "             + "\n" +
         "FROM "      + Rtrans.recordName + "\n" +
         "LEFT JOIN " + Faktur.recordName + "\n" +
         "ON "        + Rtrans.recordName + "." + Rtrans.FakturForeignKey + 
         " = "        + Faktur.recordName + ".recID \n"                   +

         " WHERE                                         \n\n"+
         "t_artiklCD = ?prm_t_artiklCD                   \n"  +
         "AND                                            \n"  +
         // 24.05.2015: 
         (isULAZinstedOfIZLAZ ? "t_skladCD  = ?prm_skladCD2                     \n\n"  :
                                "t_skladCD  = ?prm_t_skladCD                    \n\n") ; // classic, old - for izlaz 

      VvSQL.CreateCommandParameter(cmd, preffix, rtrans_rec.T_artiklCD, rows[ci.t_artiklCD]);
      VvSQL.CreateCommandParameter(cmd, preffix, rtrans_rec.T_skladCD , rows[ci.t_skladCD]);

      #endregion part1

      #region part2
      //___ part1 ______________________________________________________________________________________________________________________
      
      part2 =
         "AND                                            \n"  +
         "skladCD2  = ?prm_skladCD2                      \n\n";

      VvSQL.CreateCommandParameter(cmd, preffix, otherSkladCD , ZXC.FakturSchemaRows[ZXC.FakCI.skladCD2]);

      #endregion part2

      #region part3
      //___ part3 ______________________________________________________________________________________________________________________

      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_skladDate, rows[ci.t_skladDate]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_ttSort   , rows[ci.t_ttSort   ]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_ttNum    , rows[ci.t_ttNum    ]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_serial   , rows[ci.t_serial   ]);
      part3 =
         "AND \n" +
         "(\n" +
         "  (t_skladDate > ?prm2_t_skladDate)                                                                                          OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort > ?prm2_t_ttSort)                                                            OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum > ?prm2_t_ttNum)                                OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum = ?prm2_t_ttNum AND t_serial > ?prm2_t_serial)     \n" + // vidis, za serial '>' a ne '>=' 
         ")\n\n";
      #endregion part3

      #region part4
      //___ part4 ______________________________________________________________________________________________________________________

      part4 =
         "ORDER BY " + Rtrans.artiklOrderBy_ASC + "\n" +
         "LIMIT 1";

      #endregion part4

      // part1: determinira SET (artiklCD + skladCD) 
      // part2: determinira otherSkladCD via faktur  
      // part3: limitira od kuda krecemo             
      // part4: order by AND LIMIT 1                 

      cmd.CommandText = part1 + part2 + part3 + part4;

      return (cmd);
   }

   // dis iz: INCLUSIVE! "FRSSET + NXTSET and do something on all consecutive records" - PATTERN
   internal static XSqlCommand GetThisAndRemainingRtransesForTT_Command(XSqlConnection conn, string theTT, Rtrans rtrans_rec)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.RtransDao.TheSchemaTable.Rows;
      RtransDao.RtransCI   ci      = ZXC.RtrCI;

      string part1, part2 = "", part3 = "", part4;

      #endregion local variablez

      #region part1
      //___ part1 ______________________________________________________________________________________________________________________
      
      part1 =
         "SELECT "    + " * "             + "\n" +
         "FROM "      + Rtrans.recordName + "\n" +

         " WHERE                                         \n\n"+
         "t_artiklCD = ?prm_t_artiklCD                   \n"  +
         "AND                                            \n"  +
         "t_skladCD  = ?prm_t_skladCD                    \n\n";

      VvSQL.CreateCommandParameter(cmd, preffix, rtrans_rec.T_artiklCD, rows[ci.t_artiklCD]);
      VvSQL.CreateCommandParameter(cmd, preffix, rtrans_rec.T_skladCD , rows[ci.t_skladCD]);

      #endregion part1

      #region part2
      //___ part1 ______________________________________________________________________________________________________________________
      
      part2 =
         "AND                                            \n"  +
         "t_tt  = ?prm_t_tt                              \n\n";

      VvSQL.CreateCommandParameter(cmd, preffix, theTT, rows[ci.t_tt]);

      #endregion part2

      #region part3
      //___ part3 ______________________________________________________________________________________________________________________

      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_skladDate, rows[ci.t_skladDate]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_ttSort   , rows[ci.t_ttSort   ]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_ttNum    , rows[ci.t_ttNum    ]);
      VvSQL.CreateCommandParameter(cmd, "prm2_", rtrans_rec.T_serial   , rows[ci.t_serial   ]);
      part3 =
         "AND \n" +
         "(\n" +
         "  (t_skladDate > ?prm2_t_skladDate)                                                                                          OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort > ?prm2_t_ttSort)                                                            OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum > ?prm2_t_ttNum)                                OR \n" +
         "  (t_skladDate = ?prm2_t_skladDate AND t_ttSort = ?prm2_t_ttSort AND t_ttNum = ?prm2_t_ttNum AND t_serial >= ?prm2_t_serial)    \n" + // vidis, za serial '=>' a ne '>' 
         ")\n\n";
      #endregion part3

      #region part4
      //___ part4 ______________________________________________________________________________________________________________________

      part4 =
         "ORDER BY " + Rtrans.artiklOrderBy_ASC + "\n" +
         "";

      #endregion part4

      // part1: determinira SET (artiklCD + skladCD) 
      // part2: determinira otherTT                  
      // part3: limitira od kuda krecemo             
      // part4: order by AND LIMIT 1                 

      cmd.CommandText = part1 + part2 + part3 + part4;

      return (cmd);
   }

   // dis iz: "FRSSET on first and do something on that one and all consecutive records" - PATTERN
   internal static  XSqlCommand DELREC_FromCacheCommand(XSqlConnection conn, Rtrans rtrans_rec, VvSQL.DB_RW_ActionType actionType)
   {
      XSqlCommand cmd = InitCommand(conn);

      string preffix = "prm_";
      DataRowCollection    rows = ZXC.ArtStatDao.TheSchemaTable.Rows;
      ArtStatDao.ArtStatCI ci   = ZXC.AstCI;

      bool deleteAllRecords = (rtrans_rec.T_skladDate == DateTime.MinValue);

      // Tu moras obrisati dva seta; jedan za t_sklad_cd i jedan gdje je sklad_cd prazan.

      cmd.CommandText =

         "DELETE FROM " + ArtStat.recordName + " WHERE         \n" +
         " t_artiklCD = ?prm_t_artiklCD                    AND \n" +

         // 09.04.2015: koji je ovo ku'raty? 
       //"(t_skladCD  = ?prm_t_skladCD OR t_skladCD  = '')     \n" +
         "(t_skladCD  = ?prm_t_skladCD                   )     \n" +
         
         (deleteAllRecords == true ? "" :
         "AND \n" +

         "(\n"                                                     +
         "  (t_skladDate > ?prm_t_skladDate)                                                                                      OR \n" +
         "  (t_skladDate = ?prm_t_skladDate AND t_ttSort > ?prm_t_ttSort)                                                         OR \n" +
         "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum > ?prm_t_ttNum)                              OR \n" +
         "  (t_skladDate = ?prm_t_skladDate AND t_ttSort = ?prm_t_ttSort AND t_ttNum = ?prm_t_ttNum AND t_serial >= ?prm_t_serial)   \n" + // serial >= ?prm_serial jer brisemo ukljucujuci i za dolazni rtrans_rec 
         ")\n");

      // NOTA BENE! Ovdje trebas (a i dajes) BACKUPED vrijednosti (jer mozebitni RWTREC ima 'ko zna koje vrijednosti... 
      // Idijote, ali samo ako je rwtrec inicirao, odervajs currentData is needed.                       bijo bug 

      // 08.09.2011: Ali kada u ispravi zamijenis artikl onda stari redak dobije actionType.DEL a u current data ti je sje'an artiklCD 
      bool delrecMarkedWithWrongArtiklCD = rtrans_rec.IsThisRtrans_DelrecMarkedWithWrongArtiklCD(actionType);

      // 09.05.2016: !!! BIG NEWS !!! ___ START ___ 

      bool isDELrtransActionFromRWTdocument = 

         actionType == VvSQL.DB_RW_ActionType.DEL && 

         ((rtrans_rec.T_BCKPartiklCD .NotEmpty() && rtrans_rec.T_BCKPartiklCD !=  rtrans_rec.T_artiklCD ) ||
          (rtrans_rec.T_BCKPskladCD  .NotEmpty() && rtrans_rec.T_BCKPskladCD  !=  rtrans_rec.T_skladCD  ) ||
          (rtrans_rec.T_BCKPskladDate.NotEmpty() && rtrans_rec.T_BCKPskladDate!=  rtrans_rec.T_skladDate) ||
          (rtrans_rec.T_BCKPttSort   .NotZero () && rtrans_rec.T_BCKPttSort   !=  rtrans_rec.T_ttSort   ) ||
          (rtrans_rec.T_BCKPttNum    .NotZero () && rtrans_rec.T_BCKPttNum    !=  rtrans_rec.T_ttNum    ) ||
          (rtrans_rec.T_BCKPserial   .NotZero () && rtrans_rec.T_BCKPserial   !=  rtrans_rec.T_serial   )  );

      // 09.05.2016: !!! BIG NEWS !!! ___  END  ___ 

      VvSQL.CreateCommandParameter   (cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPartiklCD  : rtrans_rec.T_artiklCD ), rows[ci.t_artiklCD ]);
      VvSQL.CreateCommandParameter   (cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPskladCD   : rtrans_rec.T_skladCD  ), rows[ci.t_skladCD  ]);
      if(deleteAllRecords == false)                                                                                                                       
      {                                                                                                                                                   
         VvSQL.CreateCommandParameter(cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPskladDate : rtrans_rec.T_skladDate), rows[ci.t_skladDate]);
         VvSQL.CreateCommandParameter(cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPttSort    : rtrans_rec.T_ttSort   ), rows[ci.t_ttSort   ]);
         VvSQL.CreateCommandParameter(cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPttNum     : rtrans_rec.T_ttNum    ), rows[ci.t_ttNum    ]);
         VvSQL.CreateCommandParameter(cmd, preffix, (actionType == DB_RW_ActionType.RWT || isDELrtransActionFromRWTdocument/*delrecMarkedWithWrongArtiklCD*/ ? rtrans_rec.T_BCKPserial    : rtrans_rec.T_serial   ), rows[ci.t_serial   ]);
      }
      return(cmd);
   }

   internal static XSqlCommand GetDistinctSkladCdForArtikl_Command(XSqlConnection conn, string _artiklCD, bool isRootCD)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.RtransDao.TheSchemaTable.Rows;
      RtransDao.RtransCI   ci      = ZXC.RtrCI;

      #endregion local variablez

      cmd.CommandText = "SELECT DISTINCT t_skladCD FROM " + Rtrans.recordName + (_artiklCD.NotEmpty() ? isRootCD ? @" WHERE t_artiklCD LIKE '" + _artiklCD + "%'\n" : " WHERE t_artiklCD = ?prm_t_artiklCD\n" : "\n");

      if(_artiklCD.NotEmpty() && !isRootCD)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, _artiklCD, rows[ci.t_artiklCD]);
      }

      return (cmd);
   }

   internal static XSqlCommand GetDistinctFakturSkladCd_Command(XSqlConnection conn, string fakDbName, string vvDbName, bool malopOnly)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      //string               preffix = "prm_";
      //DataRowCollection    rows    = ZXC.FakturDao.TheSchemaTable.Rows;
      //FakturDao.FakturCI   ci      = ZXC.FakCI;

      #endregion local variablez

      cmd.CommandText = "SELECT DISTINCT SkladCD FROM " + fakDbName + ".faktur F"       + "\n" +
                        "                   LEFT JOIN " + vvDbName  + ".zz_Skladista S" + "\n" +
                        "ON F.SkladCD = S.TheCd       "                                 + "\n" +
                        (malopOnly ? "WHERE S.TheFlag = 1" : "")                        + "\n" +
                        "ORDER BY SkladCD"   ;
      return (cmd);
   }

   internal static XSqlCommand GetNultiZpcTtNums_Command(XSqlConnection conn)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      //string               preffix = "prm_";
      //DataRowCollection    rows    = ZXC.FakturDao.TheSchemaTable.Rows;
      //FakturDao.FakturCI   ci      = ZXC.FakCI;

      #endregion local variablez

      cmd.CommandText = "SELECT DISTINCT TtNum FROM faktur F" + "\n" +
                        "WHERE TT = 'ZPC'                   " + "\n" +
                        "AND TtNum MOD 10000 = 0            " + "\n" +
                        "ORDER BY TtNum                     ";
      return (cmd);
   }

   internal static XSqlCommand GetDistinctRtranoSernoForArtiklAndSklad_Command(XSqlConnection conn, string _PCK_ArtCD, string _PCK_SklCD)
   {
      #region local variablez 

      XSqlCommand cmd = InitCommand(conn);

      string               preffix = "prm_";
      DataRowCollection    rows    = ZXC.RtranoDao.TheSchemaTable.Rows;
      RtranoDao.RtranoCI   ci      = ZXC.RtoCI;

      #endregion local variablez

      cmd.CommandText = "SELECT DISTINCT t_serno FROM " + Rtrano.recordName + " WHERE t_skladCD = ?prm_t_skladCD AND t_artiklCD = ?prm_t_artiklCD\n";

      VvSQL.CreateCommandParameter(cmd, preffix, _PCK_SklCD, rows[ci.t_skladCD] );
      VvSQL.CreateCommandParameter(cmd, preffix, _PCK_ArtCD, rows[ci.t_artiklCD]);

      return (cmd);
   }

   #endregion The ARTIKL STATUS CACHE! OvoOno_Commands

   internal static XSqlCommand CountNOTfiskalizedRns_Command(XSqlConnection conn, string fiskTTList)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

         "SELECT COUNT(*)" + "\n" +

         From_Join_Where_ClauseForNOTfiskalizedRnsCommand(fiskTTList);

      return (cmd);
   }

   private static string From_Join_Where_ClauseForNOTfiskalizedRnsCommand(string fiskTTList)
   {
      return
         "FROM        faktur F       "     + "\n" +
         "LEFT  JOIN  faktEx X       "     + "\n" +
         "ON F.RecID = X.fakturRecID "     + "\n" +
         "WHERE F.tt IN " + fiskTTList   + "\n" +
         "AND   X.fiskJIR = ''";
   }

   #endregion VvDataRecord CalcStatus Commands

   #region IMPORT_OFFIX_Command

   public static XSqlCommand IMPORT_OFFIX_Command(XSqlConnection conn, VvDataRecord vvDataRecord, string fileName, bool shouldReplace)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "LOAD DATA LOCAL INFILE "                      + "\n" + 
                        "'" + fileName + "'"                           + "\n" + 
                        (shouldReplace == true ? "REPLACE" : "IGNORE") + "\n" +
                        "INTO TABLE " + vvDataRecord.VirtualRecordName + "\n" +

   //"CHARACTER SET latin2 \n" +
     "CHARACTER SET cp1250 \n" + // makni ovo kada se ide iz Offix-a (ali prvo probaj, mozda i ne treba micati)
   //"CHARACTER SET cp852 \n" +

                       @"FIELDS OPTIONALLY ENCLOSED BY '""'" + "\n" + // ovo jer Excel kada naidje na navodnik u nazivu onda enclosa cio naziv sos navodnici 

                       @"LINES TERMINATED BY '\n'" + "\n" + 
                        vvDataRecord.VvDao.Set_IMPORT_OFFIX_Columns();

      return(cmd);
   }

   #endregion IMPORT_OFFIX_Command

   #region RISK specials

   public static XSqlCommand SetMeVvDocument_Command(XSqlConnection conn, string tt, uint ttNum, VvDocumentRecord document_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "tt"    , tt    , ZXC.FakturSchemaRows[ZXC.FakCI.tt    ]); // pretpostavka je da svi DocumentRecordi imaju istu schemu za tt i ttNum 
      CreateCommandNamedParameter(cmd, "", "ttnUM" , ttNum , ZXC.FakturSchemaRows[ZXC.FakCI.ttNum ]); // pretpostavka je da svi DocumentRecordi imaju istu schemu za tt i ttNum 

      cmd.CommandText = "SELECT * FROM " + document_rec.VirtualRecordName + "\n" + 

                        " WHERE " + "tt = ?tt AND ttNum = ?ttNum";
      return (cmd);
   }

   public static XSqlCommand SetMeVvTrans_Command(XSqlConnection conn, string tt, uint ttNum, VvTransRecord trans_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "t_tt"    , tt    , ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt    ]); // pretpostavka je da svi TransRecordi imaju istu schemu za tt i ttNum 
      CreateCommandNamedParameter(cmd, "", "t_ttnUM" , ttNum , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum ]); // pretpostavka je da svi TransRecordi imaju istu schemu za tt i ttNum 

      cmd.CommandText = "SELECT * FROM " + trans_rec.VirtualRecordName + "\n" + 

                        " WHERE " + "t_tt = ?t_tt AND t_ttNum = ?t_ttNum";
      return (cmd);
   }

   public static XSqlCommand SetMeFaktur_Command(XSqlConnection conn, short ttSort, uint ttNum, Faktur faktur_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "ttSort", ttSort, ZXC.FakturSchemaRows[ZXC.FakCI.ttSort]);
      CreateCommandNamedParameter(cmd, "", "ttnUM" , ttNum , ZXC.FakturSchemaRows[ZXC.FakCI.ttNum ]);

      cmd.CommandText = "SELECT * FROM " + Faktur.recordName + " L " + "\n" + 
                        (
                           ZXC.TtInfo(ttSort).IsExtendableTT ?

                           "LEFT JOIN "      + FaktEx.recordName + " R " + "\n" + "\n" +
                           "ON L.RecID = R." + 
                           // 28.03.2016: 
                         //((IVvExtenderDataRecord)faktur_rec.VirtualExtenderRecord).JoinedColName
                           "fakturRecID"
                           : 
                           ""
                        ) + "\n" + "\n" + 

                        " WHERE " + "ttSort = ?ttSort AND ttNum = ?ttNum";
      return (cmd);
   }

   public static XSqlCommand SetMeFakturByKupdobCdAndVezniDok2_Command(XSqlConnection conn, uint kupdobCD, string vezniDok2)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "kupdobCD" , kupdobCD , ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD ]);
      CreateCommandNamedParameter(cmd, "", "vezniDok2", vezniDok2, ZXC.FaktExSchemaRows[ZXC.FexCI.vezniDok2]);

      cmd.CommandText = "SELECT * FROM "  + Faktur.recordName + " L " + "\n" +
                        "LEFT     JOIN "  + FaktEx.recordName + " R " + "\n" +
                        "ON L.RecID = R.fakturRecID"                  + "\n" +
                        " WHERE " + "kupdobCD = ?kupdobCD AND vezniDok2 = ?vezniDok2";
      return (cmd);
   }

   public static XSqlCommand SetMeLastRtransForArtiklAndTtNum_Command(XSqlConnection conn, short ttSort, uint ttNum, string artiklCD, Rtrans rtrans_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "ttSort", ttSort  , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttSort  ]);
      CreateCommandNamedParameter(cmd, "", "ttnUM" , ttNum   , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum   ]);
      CreateCommandNamedParameter(cmd, "", "artCD" , artiklCD, ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +

                        " WHERE " + "t_ttSort = ?ttSort AND t_ttNum = ?ttNum AND t_artiklCD = ?artCD" + "\n" +

                        "ORDER BY t_serial DESC LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMeLastRtransForTtAndTtNum_Command(XSqlConnection conn, string tt, uint ttNum, Rtrans rtrans_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "tt"    , tt      , ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt  ]);
      CreateCommandNamedParameter(cmd, "", "ttnUM" , ttNum   , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum   ]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +

                        " WHERE " + "t_tt = ?tt AND t_ttNum = ?ttNum " + "\n" +

                        "ORDER BY t_serial DESC LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMeLastRtransForArtiklAndTT_Command(XSqlConnection conn, short ttSort, string artiklCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "ttSort", ttSort  , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttSort  ]);
      CreateCommandNamedParameter(cmd, "", "artCD" , artiklCD, ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +

                        " WHERE " + "t_ttSort = ?ttSort AND t_artiklCD = ?artCD" + "\n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMeLastRtransForArtiklAnd_anyOfTheseTTs_Command(XSqlConnection conn, string[] _ttArray, string artiklCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      int i = 0;

      foreach(string _tt in _ttArray)
      {
         CreateCommandNamedParameter(cmd, "", "theTT" + (++i).ToString(), _tt, ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt]);
      }

      CreateCommandNamedParameter(cmd, "", "artCD" , artiklCD, ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +

                        " WHERE "  + "t_artiklCD = ?artCD" + " AND \n" +
                        " t_tt IN" + TtInfo.GetSql_IN_Clause(_ttArray) + "\n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMeLastUGOrtransForURArtrans_Command(XSqlConnection conn, Rtrans URArtrans_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "tt"      , Faktur.TT_UGO            , ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt       ]);
      CreateCommandNamedParameter(cmd, "", "artCD"   , URArtrans_rec.T_artiklCD , ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD ]);
      CreateCommandNamedParameter(cmd, "", "kpdCD"   , URArtrans_rec.T_kupdobCD , ZXC.RtransSchemaRows[ZXC.RtrCI.t_kupdobCD ]);
      CreateCommandNamedParameter(cmd, "", "URAdate" , URArtrans_rec.T_skladDate, ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +
                        
                        // 27.09.2018: dodan JOIN 
                        " LEFT JOIN  faktEx             \n" +
                        "                               \n" +
                        " ON t_parentID = fakturRecID   \n" +

                        " WHERE t_tt         = ?tt      \n" +
                        " AND   t_artiklCD   = ?artCD   \n" +
                        " AND   t_kupdob_cd  = ?kpdCD   \n" +
                        " AND   t_skladDate <= ?URAdate \n" + // UgovorOD 
                        // 27.09.2018: dodan via JOIN 
                        " AND   DospDate    >= ?URAdate \n" + // UgovorDO 

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMeLast_UmjPOTrtrans_For_UmjIZLAZrtrans_Command(XSqlConnection conn, string artiklCD, string skladCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "tt"      , Faktur.TT_POT            , ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt       ]);
      CreateCommandNamedParameter(cmd, "", "artCD"   ,                 artiklCD , ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD ]);
      CreateCommandNamedParameter(cmd, "", "sklCD"   ,                 skladCD  , ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD  ]);

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +
                        
                        " LEFT JOIN  faktEx             \n" +
                        "                               \n" +
                        " ON t_parentID = fakturRecID   \n" +

                        " WHERE t_tt         = ?tt      \n" +
                        " AND   t_artiklCD   = ?artCD   \n" +
                        " AND   t_skladCD    = ?sklCD   \n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }

   internal static XSqlCommand GetFakturList_ByVezaTtAndTtNumOrProjektCD_Command(XSqlConnection conn, Faktur faktur_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "v1_tt"    , faktur_rec.TT       , ZXC.FakturSchemaRows[ZXC.FakCI.v1_tt    ]);
      CreateCommandNamedParameter(cmd, "", "v1_ttNum" , faktur_rec.TtNum    , ZXC.FakturSchemaRows[ZXC.FakCI.v1_ttNum ]);
      CreateCommandNamedParameter(cmd, "", "v2_tt"    , faktur_rec.TT       , ZXC.FakturSchemaRows[ZXC.FakCI.v2_tt    ]);
      CreateCommandNamedParameter(cmd, "", "v2_ttNum" , faktur_rec.TtNum    , ZXC.FakturSchemaRows[ZXC.FakCI.v2_ttNum ]);
      CreateCommandNamedParameter(cmd, "", "v3_tt"    , faktur_rec.TT       , ZXC.FaktExSchemaRows[ZXC.FexCI.v3_tt    ]);
      CreateCommandNamedParameter(cmd, "", "v3_ttNum" , faktur_rec.TtNum    , ZXC.FaktExSchemaRows[ZXC.FexCI.v3_ttNum ]);
      CreateCommandNamedParameter(cmd, "", "v4_tt"    , faktur_rec.TT       , ZXC.FaktExSchemaRows[ZXC.FexCI.v4_tt    ]);
      CreateCommandNamedParameter(cmd, "", "v4_ttNum" , faktur_rec.TtNum    , ZXC.FaktExSchemaRows[ZXC.FexCI.v4_ttNum ]);
      if(faktur_rec.ProjektCD.NotEmpty())
      CreateCommandNamedParameter(cmd, "", "projektCD", faktur_rec.ProjektCD, ZXC.FakturSchemaRows[ZXC.FakCI.projektCD]);

      cmd.CommandText = "SELECT * FROM "  + Faktur.recordName + " L " + "\n" +
                        "LEFT     JOIN "  + FaktEx.recordName + " R " + "\n" +
                        "ON L.RecID = R.fakturRecID"                  + "\n" +
                        " WHERE                                          \n" +

                        "(v1_tt = ?v1_tt AND v1_ttNum = ?v1_ttNum) \n" +
                        "OR                                        \n" +
                        "(v2_tt = ?v2_tt AND v2_ttNum = ?v2_ttNum) \n" + 
                        "OR                                        \n" +
                        "(v3_tt = ?v3_tt AND v3_ttNum = ?v3_ttNum) \n" + 
                        "OR                                        \n" +
                        "(v4_tt = ?v4_tt AND v4_ttNum = ?v4_ttNum) \n" + 
                        (faktur_rec.ProjektCD.NotEmpty() ?
                        "OR                                        \n" +
                        "(projektCD = ?projektCD                 ) \n" :
                        ""                                            );
      return (cmd);
   }

   public static XSqlCommand SetMePreviousRtransForArtiklRobnaKarticaRtrans_Command(XSqlConnection conn, string artiklCD, string skladCD, Rtrans forThisRtrans_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "artCD" , /*forThisRtrans_rec.T_*/artiklCD  , ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD ]); // = 
      CreateCommandNamedParameter(cmd, "", "sklCD" , /*forThisRtrans_rec.T_*/skladCD   , ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladCD  ]); // = 

      CreateCommandNamedParameter(cmd, "", "ttSort", forThisRtrans_rec.T_ttSort        , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttSort   ]); // prev 
      CreateCommandNamedParameter(cmd, "", "ttNum" , forThisRtrans_rec.T_ttNum         , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum    ]); // prev 
      CreateCommandNamedParameter(cmd, "", "date"  , forThisRtrans_rec.T_skladDate     , ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate]); // prev 
      CreateCommandNamedParameter(cmd, "", "serial", forThisRtrans_rec.T_serial        , ZXC.RtransSchemaRows[ZXC.RtrCI.t_serial   ]); // prev 

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" +

                        " WHERE " + "t_artiklCD = ?artCD AND t_skladCD = ?sklCD AND " + "\n" +

                        "((                                                                 T_skladDate < ?date) OR " + "\n" +
                        " (                                           T_ttSort < ?ttSort && T_skladDate = ?date) OR " + "\n" +
                        " (                       T_ttNum < ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date) OR " + "\n" +
                      //" (T_serial <= ?serial && T_ttNum = ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date))   " + "\n" +
                        " (T_serial <  ?serial && T_ttNum = ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date))   " + "\n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }

   public static XSqlCommand SetMe_Rtrans_byTt_TtNum_Serial_Command(XSqlConnection conn, string wanted_tt, uint wanted_ttNum, uint wanted_serial)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "t_tt"    , wanted_tt    , ZXC.RtransSchemaRows[ZXC.RtrCI.t_tt    ]);
      CreateCommandNamedParameter(cmd, "", "t_ttnUM" , wanted_ttNum , ZXC.RtransSchemaRows[ZXC.RtrCI.t_ttNum ]);
      CreateCommandNamedParameter(cmd, "", "t_serial", wanted_serial, ZXC.RtransSchemaRows[ZXC.RtrCI.t_serial]); 

      cmd.CommandText = "SELECT * FROM " + Rtrans.recordName + "\n" + 

                        " WHERE " + "t_tt = ?t_tt AND t_ttNum = ?t_ttNum AND t_serial = ?t_serial";
      return (cmd);
   }

   public static XSqlCommand Getfirst_UgAn_rec_forThis_KupdobAndArtikl_Command(XSqlConnection conn, uint kupdobCD, string artiklCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "kupdobCD"  , kupdobCD, ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD  ]);
      CreateCommandNamedParameter(cmd, "", "t_artiklCD", artiklCD, ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD]); 

      cmd.CommandText = 

      "SELECT f.* FROM faktur f "                             + "\n" +

      "LEFT JOIN faktEx x ON f.recID = x.fakturRecID "        + "\n" +
      "LEFT JOIN rtrans r ON f.recID = r.t_parentID "         + "\n" +

      "WHERE (t_tt = '" + Faktur.TT_UGN + "' OR t_tt = '" + Faktur.TT_AUN + "')"  + "\n" +

     (kupdobCD.NotZero() ?
      "AND   kupdobCD   = ?kupdobCD " : " ")   + "\n" +

      "AND   t_artiklCD = ?t_artiklCD " + "\n" +

      "ORDER BY ttNum " + "\n" +
      "LIMIT 1 "        + "\n";

      return (cmd);
   }

   public static XSqlCommand GetUgAnFakturList_forThis_KupdobAndArtikl_Command(XSqlConnection conn, uint kupdobCD, string artiklCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "kupdobCD"  , kupdobCD, ZXC.FaktExSchemaRows[ZXC.FexCI.kupdobCD  ]);
      CreateCommandNamedParameter(cmd, "", "t_artiklCD", artiklCD, ZXC.RtransSchemaRows[ZXC.RtrCI.t_artiklCD]); 

      cmd.CommandText = 

      "SELECT f.*, x.* FROM faktur f "                             + "\n" +

      "LEFT JOIN faktEx x ON f.recID = x.fakturRecID "        + "\n" +
      "LEFT JOIN rtrans r ON f.recID = r.t_parentID "         + "\n" +

      "WHERE (t_tt = '" + Faktur.TT_UGN + "' OR t_tt = '" + Faktur.TT_AUN + "')"  + "\n" +

     (kupdobCD.NotZero() ?
      "AND   kupdobCD   = ?kupdobCD " : " ")   + "\n" +

      "AND   t_artiklCD = ?t_artiklCD " + "\n" +

      "ORDER BY ttNum " + "\n" /*+
      "LIMIT 1 "        + "\n"*/;

      return (cmd);
   }

   // 11.04.2024: za potrebu nadji mi gdje je ovaj serno u ovom trenutku / ili prije ovog (rtrano-a ili rtrans-a ... jos nisi odlucio)
   public static XSqlCommand SetMePreviousRtranoForThisSerno_Command(XSqlConnection conn, string theSerno, Rtrano forThisRtrano_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "serno" , theSerno                          , ZXC.RtranoSchemaRows[ZXC.RtoCI.t_skladCD  ]); // = 
      CreateCommandNamedParameter(cmd, "", "ttSort", forThisRtrano_rec.T_ttSort        , ZXC.RtranoSchemaRows[ZXC.RtoCI.t_ttSort   ]); // prev 
      CreateCommandNamedParameter(cmd, "", "ttNum" , forThisRtrano_rec.T_ttNum         , ZXC.RtranoSchemaRows[ZXC.RtoCI.t_ttNum    ]); // prev 
      CreateCommandNamedParameter(cmd, "", "date"  , forThisRtrano_rec.T_skladDate     , ZXC.RtranoSchemaRows[ZXC.RtoCI.t_skladDate]); // prev 
      CreateCommandNamedParameter(cmd, "", "serial", forThisRtrano_rec.T_serial        , ZXC.RtranoSchemaRows[ZXC.RtoCI.t_serial   ]); // prev 

      cmd.CommandText = "SELECT * FROM " + Rtrano.recordName + "\n" +

                        " WHERE " + "t_serno = ?serno AND " + "\n" +

                        "((                                                                 T_skladDate < ?date) OR " + "\n" +
                        " (                                           T_ttSort < ?ttSort && T_skladDate = ?date) OR " + "\n" +
                        " (                       T_ttNum < ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date) OR " + "\n" +
                      //" (T_serial <= ?serial && T_ttNum = ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date))   " + "\n" +
                        " (T_serial <  ?serial && T_ttNum = ?ttNum && T_ttSort = ?ttSort && T_skladDate = ?date))   " + "\n" +

                        "ORDER BY " + Rtrans.artiklOrderBy_DESC + " LIMIT 1";
      return (cmd);
   }


   #endregion RISK specials

   #region    MIXER specials

   public static XSqlCommand SetMeMixer_Command(XSqlConnection conn, string tt, uint ttNum, Mixer Mixer_rec)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "tt"   , tt   , ZXC.MixerSchemaRows[ZXC.MixCI.tt   ]);
      CreateCommandNamedParameter(cmd, "", "ttnUM", ttNum, ZXC.MixerSchemaRows[ZXC.MixCI.ttNum]);

      cmd.CommandText = "SELECT * FROM " + Mixer.recordName + "\n" +
                        " WHERE " + "tt = ?tt AND ttNum = ?ttNum";
      return (cmd);
   }

   public static XSqlCommand GetZadnjeStanjeBrojila_Command(XSqlConnection conn, string voziloCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "voziloCD", voziloCD, ZXC.MixerSchemaRows[ZXC.MixCI.strH_32]);

      cmd.CommandText = "SELECT MAX(t_intB) " + "\n" +
                        "FROM      mixer  M " + "\n" +
                        "LEFT JOIN xtrans X ON M.recID = X.t_parentID" + "\n" +
                        "WHERE strH_32 = ?voziloCD"                    + "\n";
      return (cmd);
   }

   public static XSqlCommand GetLastXtransByDateAndArtikl_Command(XSqlConnection conn, string artiklCD, DateTime dokDate, string theTT)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "artiklCD", artiklCD, ZXC.XtransSchemaRows[ZXC.XtrCI.t_artiklCD]);
      CreateCommandNamedParameter(cmd, "", "theTT"   , theTT   , ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt      ]);
      CreateCommandNamedParameter(cmd, "", "dokDate" , dokDate , ZXC.XtransSchemaRows[ZXC.XtrCI.t_dokDate ]);

      cmd.CommandText = "SELECT *                        \n" +
                        "FROM xtrans  X                  \n" +
                        "WHERE t_artiklCD  = ?artiklCD   \n" +
                        "AND   t_tt        = ?theTT      \n" +
                        "AND   t_dokDate  <= ?dokDate    \n" +
                        "ORDER BY t_dokDate DESC LIMIT 1 \n";
      return (cmd);
   }

   internal static XSqlCommand GetLastRecordBySomeOrder_Command(XSqlConnection conn, VvDataRecord vvDataRecord, List<VvSqlFilterMember> filterMembers, string orderBy, bool needsExtender)
   {
      XSqlCommand cmd = InitCommand(conn);

      string recordName = VvDataRecord.ExtractNormalTableNameFromArhivaTableName((string)(filterMembers[0].drSchema["BaseTableName"]));

      cmd.CommandText = "SELECT * FROM " + recordName                        +  "\n" +

         (!needsExtender ? "" : 
         " L \n LEFT JOIN " + vvDataRecord.VirtualExtenderRecord.VirtualRecordName + " R \n" +
         " ON L.RecID = R." + ((IVvExtenderDataRecord)vvDataRecord.VirtualExtenderRecord).JoinedColName) + "\n" +

         VvSQL.ParameterizedWhereClauseFromVvSqlFilter(filterMembers, false) + "\n" +
         
         (orderBy.NotEmpty() ? " ORDER BY " + orderBy : "")                  + "\n" +
         
         "DESC LIMIT 1";

      VvSQL.SetReportCommandParamValues(cmd, filterMembers);

      return (cmd);
   }

   public static XSqlCommand GetNazivArtiklaZaKupca_Command(XSqlConnection conn, uint kupdobCD, string artiklCD)
   {
      XSqlCommand cmd = InitCommand(conn);

      CreateCommandNamedParameter(cmd, "", "artiklCD", artiklCD     , ZXC.XtransSchemaRows[ZXC.XtrCI.t_artiklCD]);
      CreateCommandNamedParameter(cmd, "", "theTT"   , Mixer.TT_NAK , ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt      ]);
      CreateCommandNamedParameter(cmd, "", "kupdobCD", kupdobCD     , ZXC.XtransSchemaRows[ZXC.XtrCI.t_kupdobCD]);

      cmd.CommandText = "SELECT t_vezniDokA_64           \n" +
                        "FROM xtrans  X                  \n" +
                        "WHERE t_artiklCD  = ?artiklCD   \n" +
                        "AND   t_tt        = ?theTT      \n" +
                        "AND   t_kupdobCD  = ?kupdobCD   \n" +
                        "ORDER BY t_dokDate DESC LIMIT 1 \n";
      return (cmd);
   }

   #endregion MIXER specials

   #region COPY_TABLE_Command

   public static XSqlCommand CLONE_TABLE_Command(XSqlConnection conn, string existingTableName, string cloneToTableName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DROP TABLE IF EXISTS " + cloneToTableName + "; CREATE TABLE " + cloneToTableName + " AS SELECT * FROM " + existingTableName + "\n";

      return(cmd);
   }

   public static XSqlCommand COPY_TABLE_Command(XSqlConnection conn, string destDbName, string destTblname, string srcDbName, string srcTblname)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "INSERT INTO " + destDbName + "." + destTblname + " SELECT * FROM " + "\n" + 
                                          srcDbName + "." +  srcTblname ;

      return(cmd);
   }

   public static XSqlCommand COPY_DECEMBAR_PLACA_TABLE_Command(XSqlConnection conn, string tblname, string destDbName, string srcDbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      string dateCol = (tblname == Placa.recordName ? "dokDate" : "t_dokDate");

      string lastDecembarMysqlDay = ZXC.projectYearLastDay.Year.ToString() + "-" + ZXC.projectYearLastDay.Month.ToString() + "-" + ZXC.projectYearLastDay.Day.ToString();

      cmd.CommandText = "INSERT INTO " + destDbName + "." + tblname + " SELECT * FROM " + "\n" +
                                          srcDbName + "." + tblname                     + "\n" + 
                        "WHERE " + dateCol + " > '" + lastDecembarMysqlDay + "'\n";

      return (cmd);
   }

   public static XSqlCommand COPY_KDC_MIXER_TABLE_Command(XSqlConnection conn, string tblname, string destDbName, string srcDbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      string ttCol = (tblname == Mixer.recordName ? "tt" : "t_tt");


      cmd.CommandText = "INSERT INTO " + destDbName + "." + tblname + " SELECT * FROM " + "\n" +
                                          srcDbName + "." + tblname                     + "\n" + 
                        "WHERE " + ttCol + " = '" + Mixer.TT_KDC + "'\n";

      return (cmd);
   }

   public static XSqlCommand COPY_RASTER_MIXER_TABLE_Command(XSqlConnection conn, string tblname, string destDbName, string srcDbName)
   {
      XSqlCommand cmd = InitCommand(conn);

      string ttCol   = (tblname == Mixer.recordName ? "tt"      : "t_tt"     );
      string dateCol = (tblname == Mixer.recordName ? "dokDate" : "t_dokDate");


      cmd.CommandText = "INSERT INTO " + destDbName + "." + tblname + " SELECT * FROM " + "\n" +
                                          srcDbName + "." + tblname + "\n" +
                        "WHERE (" + ttCol + " = '" + Mixer.TT_RASTERB + "' OR " + ttCol + " = '" + Mixer.TT_RASTERF + "') AND " +
                        "MONTH(" + dateCol + ") = 12 AND YEAR(" + dateCol + ") = " + ZXC.projectYear + " \n";

      return (cmd);
   }

   //public static XSqlCommand COPY_0ZPC_FAKTUR_TABLE_Command(XSqlConnection conn, string tblname, string destDbName, string srcDbName)
   //{
   //   XSqlCommand cmd = InitCommand(conn);
   //
   //   string ttCol   = (tblname == Mixer.recordName ? "tt"      : "t_tt"     );
   //   string dateCol = (tblname == Mixer.recordName ? "dokDate" : "t_dokDate");
   //
   //
   //   cmd.CommandText = "INSERT INTO " + destDbName + "." + tblname + " SELECT * FROM " + "\n" +
   //                                       srcDbName + "." + tblname + "\n" +
   //
   //                     "WHERE " + ttCol   + " = '" + Faktur.TT_ZPC + "'\n" +
   //                     "AND   " + dateCol + " = '" + ZXC.projectYearFirstDay + "'\n";
   //
   //   return (cmd);
   //}

   public static XSqlCommand Get_TABLE_Create_Time_Command(XSqlConnection conn, string dbName, string tblname)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = 

         "SELECT create_time FROM INFORMATION_SCHEMA.TABLES " + " \n" +
         "WHERE table_schema = '" + dbName                    + "'\n" +
         "AND table_name     = '" + tblname                   + "'\n" ;

      return (cmd);
   }


   #endregion COPY_TABLE_Command

   #region SqlSomeCheckQuery_Command

   public static XSqlCommand CheckForUnlinkedTranses_Command(XSqlConnection conn, string documentName, string transesName)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "SELECT "     + transesName  + ".recID FROM " + documentName + "\n" +
                        "RIGHT JOIN " + transesName                                  + "\n" +
                        "ON "         + documentName + ".recID = t_parentID"         + "\n" +
                        "WHERE "      + documentName + ".recID IS NULL"              + "\n" +
                        "AND   "      + transesName  + ".t_tt != 'PIM'"              + "\n" +
                        "AND   "      + transesName  + ".t_tt != 'PUM'"              + "\n" +
                        "AND   "      + transesName  + ".t_tt != 'THL'"              + "\n" +
                        "ORDER BY "   + transesName  + ".recID"                             ;

      return (cmd);
   }

   public static XSqlCommand DELETE_UnlinkedTranses_Command(XSqlConnection conn, string transesName, int theRecID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText = "DELETE FROM " + transesName + " WHERE recID = '" + theRecID + "'";
      return (cmd);
   }

   public static XSqlCommand DELETE_E_DuplicateTranses_Command(XSqlConnection conn, uint t_parentID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =
         "DELETE FROM rtrans                                                       " + "\n" +
         "WHERE t_parentID = " + t_parentID + "                                    " + "\n" +
         "AND recID IN                                                             " + "\n" +
         "(                                                                        " + "\n" +
         "SELECT MIN(subRtr.recID)                                                 " + "\n" +
         "FROM (SELECT * FROM rtrans WHERE t_parentID = " + t_parentID + ") subRtr " + "\n" + 
         "GROUP BY subRtr.t_tt, subRtr.t_ttNum, subRtr.t_serial                    " + "\n" +
         "HAVING COUNT(*) > 1                                                      " + "\n" +
         ")                                                                        " + "\n" ;
         //# You can replace min(e.id) to max(e.id) to remove newest records.
         //#PAZI!!! Na 2 mjesta moras zadati t_parentID!      

      return (cmd);
   }

   internal static XSqlCommand SqlSomeCheckQuery_Command(XSqlConnection conn, ZXC.MySqlCheck_Kind mySqlCheck_Kind)
   {
      XSqlCommand cmd = InitCommand(conn);

      switch(mySqlCheck_Kind)
      {
         case ZXC.MySqlCheck_Kind.A_FaktEx_without_Faktur: cmd.CommandText =
            "SELECT X.* FROM faktur f   \n" +
            "RIGHT JOIN      faktEx x   \n" +
            "ON f.recID = x.fakturRecID \n" +
            "WHERE f.RecID IS NULL      \n" +
            "ORDER BY x.fakturRecID     \n" ; break;

         case ZXC.MySqlCheck_Kind.B_Faktur_without_KupdobCD_OR_FaktEx: cmd.CommandText =
            "SELECT f.tt, f.ttNum, f.lanSrvID, f.lanRecID, f.* FROM faktur f \n" +
            "LEFT JOIN     faktEx x                  \n" +
            "ON f.recID = x.fakturRecID              \n" +
            "WHERE f.tt IN                           \n" +
            "('IFA', 'IRA', 'IZD', 'IMT', 'IZM', 'RVI', 'IOD', 'IPV', 'IRM', 'UFA', 'UPA', 'UFM', 'URA', 'URM', 'UPM', 'UOD', 'UPV', 'PRI', 'RVU', 'KLK', 'KKM', 'OPN', 'PON', 'PNM', 'INM', 'NRD', 'NRM', \n" +
            " 'NRU', 'NRS', 'NRK', 'UPL', 'ISP', 'RNP', 'RNM', 'RNS', 'PRJ', 'KIZ', 'KUL', 'PIK', 'PUK', 'PSM', 'ZPC', 'VMI', 'VMU', 'TRI', 'TRM', 'PIX', 'PUX', 'BOR', 'NOR', 'PIM', 'MMI', 'MVI', 'MVU')        \n" +
            "AND (x.fakturRecID IS NULL OR (tt = 'IRM' AND kupdobCD = 0))\n" +
            "ORDER BY f.RecID                        \n" ; break;

         case ZXC.MySqlCheck_Kind.C_Faktur_withMore_FaktEx: cmd.CommandText =
          //"SELECT                       COUNT(*) as counter, f.*, X.* FROM faktur f \n" +
            "SELECT 'qwe', x.fakturRecID, COUNT(*) as counter, f.*, X.* FROM faktur f \n" +
            "RIGHT JOIN                                faktEx x \n" +
            "ON f.recID = x.fakturRecID \n" +
            "GROUP BY x.fakturRecID     \n" +
            "HAVING counter > 1         \n"; break;

         case ZXC.MySqlCheck_Kind.D_Rtrans_without_Faktur: cmd.CommandText =
            "SELECT r.* FROM faktur f  \n" +
            "RIGHT JOIN      rtrans r  \n" +
            "ON f.recID = r.t_parentID \n" +
            "WHERE f.recID IS NULL     \n" +
            "AND   r.t_tt != 'PIM'     \n" +
            "AND   r.t_tt != 'PUM'     \n" +
            "ORDER BY r.recID          \n"; break;

         case ZXC.MySqlCheck_Kind.E_Rtrans_duplicates: cmd.CommandText =
            "SELECT R.t_tt, R.t_ttNum, COUNT(*) as counter, R.* FROM rtrans r \n" +
            "GROUP BY t_tt, t_ttNum, t_serial              \n" +
            "HAVING counter > 1                            \n" ; break;

         case ZXC.MySqlCheck_Kind.F_Faktur_without_vvLogLAN: cmd.CommandText =
            "SELECT f.tt, f.ttNum, f.lanSrvID, f.lanRecID, f.* FROM faktur f\n" +
            "LEFT JOIN   vvLogLAN l                                   \n" +
            "ON f.LanSrvID = l.origSRVID AND f.LanRecID = l.origRecID \n" +
            "WHERE l.logID IS NULL                                    \n"; break;

         case ZXC.MySqlCheck_Kind.G_Rtrans_without_TwinRtr: cmd.CommandText =
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'MSI' or t_tt = 'MSU'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" +
            "UNION                                      \n" +
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'MMI' or t_tt = 'MMU'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" +
            "UNION                                      \n" +
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'VMI' or t_tt = 'VMU'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" +
            "UNION                                      \n" +
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'MVI' or t_tt = 'MVU'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" +
            "UNION                                      \n" +
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'KIZ' or t_tt = 'KUL'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" +
            "UNION                                      \n" +
            "SELECT COUNT(*) as C, R.* FROM rtrans r    \n" +
            "WHERE t_tt = 'PIK' or t_tt = 'PUK'         \n" +
            "GROUP BY t_parentID, t_serial HAVING C < 2 \n" ; break;

         case ZXC.MySqlCheck_Kind.H_vvLogERR_xy_exists: cmd.CommandText =
            "SHOW TABLES LIKE 'vvlog%ERR%'\n" ; break;

         case ZXC.MySqlCheck_Kind.I_Rtrans_with_wrong_TwinID: cmd.CommandText =
            "SELECT R.t_tt, R.t_ttNum, R.* FROM rtrans R              \n" +
            "LEFT JOIN       rtrans T                                 \n" +
            "ON R.t_twinID = T.RecID                                  \n" +
            "WHERE R.t_tt IN('MSI', 'MMI', 'VMI', 'MVI', 'KIZ', 'PIK')\n" +
            "AND (T.RecID IS NULL OR T.t_parentID != R.t_parentID)    \n" ; break;

         case ZXC.MySqlCheck_Kind.J_FakturSUM_vs_RtransSUM: cmd.CommandText =
            "SELECT f.tt, f.ttNum, f.s_ukK, SUM(r.t_kol), (f.s_ukKC), (SUM(r.t_kol * r.t_cij)), f.* FROM faktur f                         \n" +
            "LEFT JOIN     rtrans r                                                                                                       \n" +
            "ON f.recID = r.t_parentID                                                                                                    \n" +
            "WHERE r.t_tt != 'ZPC' AND r.t_tt != 'PUL' AND r.t_tt != 'PUX' AND r.t_tt != 'PUM' AND r.t_tt != 'TRM' AND r.t_tt != 'RNU' AND\n" +
            "      r.t_tt != 'MSU' AND r.t_tt != 'MMU' AND r.t_tt != 'VMU' AND r.t_tt != 'MVU' AND r.t_tt != 'KUL' AND r.t_tt != 'PUK'    \n" +
            "GROUP BY f.recID                                                                                                             \n" +
            "HAVING SUM(r.t_kol) IS NOT NULL                                                                                              \n" +
            // 02.01.2024: 
          //"AND (ABS(f.s_ukK - SUM(r.t_kol)) >= 1 OR ABS(ABS(f.s_ukKC) - ABS(SUM(      r.t_kol * r.t_cij    ))) >= 1)                              \n" ; break;
            "AND (ABS(f.s_ukK - SUM(r.t_kol)) >= 1 OR ABS(ABS(f.s_ukKC) - ABS(SUM(ROUND(r.t_kol * r.t_cij, 2)))) >= 1)                              \n"; break;

         case ZXC.MySqlCheck_Kind.K_TwinRtransVsArtstatCij: cmd.CommandText =
            "SELECT R.t_tt, R.t_ttNum, R.*                       " + "\n" +
            "FROM      rtrans  R                                 " + "\n" +
            "LEFT JOIN artstat A                                 " + "\n" +
            "ON R.recID = A.rtransRecID                          " + "\n" +
          //"WHERE ABS(r.t_kol*(r.t_cij - a.rtrCijenaNBC)) > 0.15" + "\n" +
            "WHERE ABS(r.t_kol*(r.t_cij - a.rtrCijenaNBC)) > 0.30" + "\n" +
            "AND R.t_artiklCD != ''                              " + "\n" +
            "AND R.t_tt IN                                       " + TtInfo.GetSql_IN_Clause(new string[] { "MSU", "VMU", "MVU", "MMU", "KUL", "PUK" }) + " "; break;

         case ZXC.MySqlCheck_Kind.L_NotFiskalizedIRMs: cmd.CommandText =
            "SELECT f.tt, f.ttNum       \n" +
            "FROM        faktur F       \n" +
            "LEFT  JOIN  faktEx X       \n" +
            "ON F.RecID = X.fakturRecID \n" +
            "WHERE F.tt = 'IRM'         \n" +
            "AND   X.fiskJIR = ''       \n" ; break;

         case ZXC.MySqlCheck_Kind.M_MsiMsu_Roundtrip: cmd.CommandText =

            "SELECT RtrMSI_1.t_tt, RtrMSI_1.t_ttNum,                                                                                                 \n" +
            "#RtrMSU_1.t_ttNUm AS MS_1, RtrMSI_2.t_ttNUm AS MS_2,                                                                                    \n" +
            "#RtrMSU_1.t_artiklCD,  RtrMSI_2.t_artiklCD,                                                                                             \n" +
            "#RtrMSU_1.t_skladDate, RtrMSI_2.t_skladDate,                                                                                            \n" +
            "#RtrMSU_1.t_skladCD,   RtrMSI_2.t_skladCD,                                                                                              \n" +
            "#RtrMSI_1.t_skladCD,   (SELECT RtrMSU_2.t_skladCD FROM rtrans RtrMSU_2 WHERE RtrMSI_2.t_twinID = RtrMSU_2.RecID) AS RtrMSU_2_t_skladCD, \n" +
            "                                                                                                                                        \n" +
            "RtrMSU_1.*                                                                                                                              \n" +
            "                                                                                                                                        \n" +
            "FROM      rtrans RtrMSU_1                                                                                                               \n" +
            "LEFT JOIN rtrans RtrMSI_1                                                                                                               \n" +
            "                                                                                                                                        \n" +
            "ON RtrMSI_1.t_twinID = RtrMSU_1.RecID                                                                                                   \n" +
            "                                                                                                                                        \n" +
            "LEFT JOIN rtrans RtrMSI_2                                                                                                               \n" +
            "                                                                                                                                        \n" +
            "ON (                                                                                                                                    \n" +
            "RtrMSI_2.t_tt        = 'MSI'                AND                                                                                         \n" +
            "RtrMSI_2.t_artiklCD  = RtrMSU_1.t_artiklCD  AND                                                                                         \n" +
            "RtrMSI_2.t_skladDate = RtrMSU_1.t_skladDate AND                                                                                         \n" +
            "RtrMSI_2.t_skladCD   = RtrMSU_1.t_skladCD     )                                                                                         \n" +
            "                                                                                                                                        \n" +
            "                                                                                                                                        \n" +
            "WHERE RtrMSI_1.t_tt = 'MSI' AND                                                                                                         \n" +
            "                                                                                                                                        \n" +
            "RtrMSI_1.t_skladCD = (SELECT RtrMSU_2.t_skladCD FROM rtrans RtrMSU_2 WHERE RtrMSI_2.t_twinID = RtrMSU_2.RecID)                          \n" ; break;
      }

      return (cmd);
   }

   internal static XSqlCommand MsiMsu_Roundtrip_CheckBefSave_Command(string msiTT, XSqlConnection conn, string artiklCD_MsiNEW, DateTime skladDadte_MsiNEW, string skladCD_MsiNEW, string skladCD2_MsiNEW)
   { 
      XSqlCommand cmd = InitCommand(conn); 

      cmd.CommandText =
         "SELECT MsiPrevRtrans.t_TT, MsiPrevRtrans.t_TtNum, MsiPrevRtrans.*                          \n" +

         "FROM      rtrans MsiPrevRtrans                                                             \n" +
         "LEFT JOIN faktur MsiPrevFaktur                                                             \n" +
         "ON MsiPrevFaktur.RecID = MsiPrevRtrans.t_parentID                                          \n" +
                                                                                                     
         "WHERE MsiPrevRtrans.t_artiklCD      =      ?prm_artiklCD_MsiNEW    # DUC T_artiklCD        \n" +
         "AND DATE(MsiPrevRtrans.t_skladDate) = DATE(?prm_skladDadte_MsiNEW) # DUC T_skladDate       \n" +
       //"AND MsiPrevRtrans.t_tt              = 'MSI'                        # DUC T_tt              \n" +
         "AND MsiPrevRtrans.t_tt              = ?prm_msiTT                   # DUC T_tt              \n" +
         "AND MsiPrevFaktur.SkladCD2          = ?prm_skladCD_MsiNEW          # DUC F_skladCD         \n" +
         "AND MsiPrevFaktur.SkladCD           = ?prm_skladCD2_MsiNEW         # DUC F_SkladCD2        \n" ;

      CreateCommandParameter(cmd, "artiklCD_MsiNEW"  , artiklCD_MsiNEW  , XSqlDbType.VarChar, 32);
      CreateCommandParameter(cmd, "skladDadte_MsiNEW", skladDadte_MsiNEW, XSqlDbType.Date   , 12);
      CreateCommandParameter(cmd, "skladCD_MsiNEW"   , skladCD_MsiNEW   , XSqlDbType.VarChar,  6);
      CreateCommandParameter(cmd, "skladCD2_MsiNEW"  , skladCD2_MsiNEW  , XSqlDbType.VarChar,  6);
      CreateCommandParameter(cmd, "msiTT"            , msiTT            , XSqlDbType.VarChar,  6);

      return (cmd);
   }

   internal static XSqlCommand SqlDeleteOrphanRows_Command(XSqlConnection conn, ZXC.MySqlCheck_Kind mySqlCheck_Kind)
   {
      XSqlCommand cmd = InitCommand(conn);

      switch(mySqlCheck_Kind)
      {
         case ZXC.MySqlCheck_Kind.A_FaktEx_without_Faktur: cmd.CommandText =
            "delete X   FROM faktur f   \n" +
            "RIGHT JOIN      faktEx x   \n" +
            "ON f.recID = x.fakturRecID \n" +
            "WHERE f.RecID IS NULL      \n" ; break;

         case ZXC.MySqlCheck_Kind.D_Rtrans_without_Faktur: cmd.CommandText =
            "DELETE r   FROM faktur f  \n" +
            "RIGHT JOIN      rtrans r  \n" +
            "ON f.recID = r.t_parentID \n" +
            "WHERE f.recID IS NULL     \n" +
            "AND   r.t_tt != 'PIM'     \n" +
            "AND   r.t_tt != 'PUM'     \n" ; break;

#if NottayettA
         case ZXC.MySqlCheck_Kind.B_Faktur_without_FaktEx: cmd.CommandText =
            "SELECT f.tt, f.ttNum, f.* FROM faktur f \n" +
            "LEFT JOIN     faktEx x                  \n" +
            "ON f.recID = x.fakturRecID              \n" +
            "WHERE f.tt IN                           \n" +
            "('IFA', 'IRA', 'IZD', 'IMT', 'IZM', 'RVI', 'IOD', 'IPV', 'IRM', 'UFA', 'UPA', 'UFM', 'URA', 'URM', 'UPM', 'UOD', 'UPV', 'PRI', 'RVU', 'KLK', 'KKM', 'OPN', 'PON', 'PNM', 'INM', 'NRD', 'NRM', \n" +
            " 'NRU', 'NRS', 'NRK', 'UPL', 'ISP', 'RNP', 'RNS', 'PRJ', 'KIZ', 'KUL', 'PIK', 'PUK', 'PSM', 'ZPC', 'VMI', 'VMU', 'TRI', 'TRM', 'PIX', 'PUX', 'BOR', 'NOR', 'PIM', 'MMI', 'MVI', 'MVU')        \n" +
            "AND x.fakturRecID IS NULL               \n" +
            "ORDER BY f.RecID                        \n" ; break;

         case ZXC.MySqlCheck_Kind.C_Faktur_withMore_FaktEx: cmd.CommandText =
            "SELECT COUNT(*) as counter, f.*, X.* FROM faktur f \n" +
            "RIGHT JOIN                                faktEx x \n" +
            "ON f.recID = x.fakturRecID \n" +
            "GROUP BY x.fakturRecID     \n" +
            "HAVING counter > 1         \n"; break;

         case ZXC.MySqlCheck_Kind.E_Rtrans_duplicates: cmd.CommandText =
            "SELECT COUNT(*) as counter, R.* FROM rtrans r \n" +
            "GROUP BY t_tt, t_ttNum, t_serial              \n" +
            "HAVING counter > 1                            \n"; break;

         case ZXC.MySqlCheck_Kind.F_Faktur_without_vvLogLAN: cmd.CommandText =
            "SELECT * FROM faktur f                                   \n" +
            "LEFT JOIN   vvLogLAN l                                   \n" +
            "ON f.LanSrvID = l.origSRVID AND f.LanRecID = l.origRecID \n" +
            "WHERE l.logID IS NULL                                    \n"; break;
#endif
      }

      return (cmd);
   }

   internal static XSqlCommand SqlDeleteTooManyFaktExRows_Command(XSqlConnection conn, uint fakturRecID)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

            "DELETE FROM faktEx                     \n" +
            "WHERE fakturRecID = " + fakturRecID + "\n" +
            "AND DATE(pdvDate) = '0001-01-01'       \n" +
            "AND kupdobCD      = 0                  \n" ;

            // 20.12.2018: izbacen s_ukKCR uvjet
          //"AND s_ukKCR       = 0                  \n" ;

      return (cmd);
   }

   public static XSqlCommand GetCheckSyncList_Command(XSqlConnection conn, DateTime dateDO, string[] skladCdList, string grordBy, string selectWhat, string skladCD, string tt, DateTime onlyDate)
   {
      XSqlCommand cmd = InitCommand(conn);

      string id = ZXC.vvDB_ServerID.ToString("00");

      cmd.CommandText = 
         
                       "SELECT " + selectWhat + " FROM rtrans"                         +         "\n"  +
(onlyDate.NotEmpty() ? "WHERE t_skladDate =  ?prm_t_skladDate"                         + " \n" : "\n") +
(onlyDate.IsEmpty()  ? "WHERE t_skladDate <= ?prm_t_skladDate"                         + " \n" : "\n") +
                       "AND   t_skladCD IN "   + TtInfo.GetSql_IN_Clause(skladCdList)  +         "\n"  +
(skladCD .NotEmpty() ? "AND   t_skladCD   = '" + skladCD                               + "'\n" : "\n") +
(tt      .NotEmpty() ? "AND   t_tt        = '" + tt                                    + "'\n" : "\n") +
/* 13.12.2015: */      "AND   t_tt IN " + TtInfo.GetSql_IN_Clause(ZXC.CURR_SkyRules_RtransTT_Array) +   "\n"  + /* 13.12.2015: */ 

                       // Ne provjeravaj skl povrata 12BPx, osim ako nije SHOPov MVU u pitanju
                       "AND (IF(t_tt = 'MVU', SUBSTRING(t_ttNum, 1, 2) = '" + id + "', t_skladCD NOT LIKE '12BP_'))" + "\n" +

                       grordBy;

      CreateCommandParameter(cmd, "prm_", (onlyDate.NotEmpty() ? onlyDate : dateDO), ZXC.RtransSchemaRows[ZXC.RtrCI.t_skladDate]);

      return (cmd);
   }

 //internal static XSqlCommand UndoReceive_RollItBack_Command(XSqlConnection connLAN, uint recID)
   internal static XSqlCommand UndoReceive_RollItBack_Command(XSqlConnection connLAN, uint lanSrvID, uint lanRecID)
   {
      XSqlCommand cmd = InitCommand(connLAN);

      cmd.CommandText =
         "DELETE  v, r, x, f         \n" +
         "FROM      faktur f         \n" +
         "LEFT JOIN faktEx x         \n" +
         "ON x.fakturRecId = f.recID \n" +
         "LEFT JOIN rtrans r         \n" +
         "ON r.t_parentId  = f.recID \n" +
         "LEFT JOIN vvlogLAN v       \n" +
         "ON v.recId  = f.recID      \n" +

       //"WHERE f.recID = " + recID.ToString();
         "WHERE f.lanSrvID = " + lanSrvID.ToString() + "\n" +
         "AND   f.lanRecID = " + lanRecID.ToString();

      return (cmd);
   }

   internal static XSqlCommand DeleteDeadArtikls_Command(XSqlConnection conn)
   {
      XSqlCommand cmd = InitCommand(conn);

      cmd.CommandText =

      "DELETE art FROM rtrans rtr      \n" +
      "RIGHT JOIN artikl art           \n" +
      "ON art.ArtiklCD = rtr.t_artiklCD\n" +
      "WHERE rtr.recID IS NULL         \n" ;
      
      return (cmd);
   }

   #endregion SqlSomeCheckQuery_Command

}
