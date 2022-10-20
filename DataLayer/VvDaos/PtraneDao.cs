using System;
using System.Data;

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

public sealed class PtraneDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static PtraneDao instance;

   private PtraneDao(XSqlConnection conn, string dbName) : base(dbName, Ptrane.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PtraneDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new PtraneDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePtrane

   public static   uint TableVersionStatic { get { return 4; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID         int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID    int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial      smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_dokDate     date                  NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt          char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum       int(10)     unsigned  NOT NULL               ,\n" +
         /* 07 */  "t_personCD    int(6)      unsigned  NOT NULL default '0'   ,\n" +
         /* 08 */  "t_ime         varchar(24)           NOT NULL default ''    ,\n" +
         /* 09 */  "t_prezime     varchar(32)           NOT NULL default ''    ,\n" +

         /* 10 */  "t_vrstaR_cd   char(3)               NOT NULL default ''    ,\n" +
         /* 11 */  "t_vrstaR_name varchar(64)           NOT NULL default ''    ,\n" +
         /* 12 */  "t_cijPerc     decimal( 6,2)         NOT NULL default '0.00',\n" +
         /* 13 */  "t_sati        decimal( 6,2)         NOT NULL default '0.00',\n" +
         /* 14 */  "t_rsOO        char(3)               NOT NULL default ''    ,\n" +
         /* 15 */  "t_rsOD        tinyint(2)  unsigned  NOT NULL default '0'   ,\n" +
         /* 16 */  "t_rsDO        tinyint(2)  unsigned  NOT NULL default '0'   ,\n" +

         /* 17 */  "t_stjecatCD char(5)                 NOT NULL default ''    ,\n" +
         /* 18 */  "t_primDohCD char(5)                 NOT NULL default ''    ,\n" +
         /* 19 */  "t_pocKrajCD char(5)                 NOT NULL default ''    ,\n" +
         /* 20 */  "t_ip1gr       int(10)     unsigned  NOT NULL default '0'   ,\n" +
         /* 21 */  "t_rbrIsprJop  int(10)               NOT NULL default '0'   ,\n" +

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_PERSON     (t_personCD, t_dokDate,   t_dokNum,  t_serial)           \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Ptrane.recordNameArhiva;
      else         tableName = Ptrane.recordName;

      switch(catchingVersion)
      {

         case 2: return ("ADD COLUMN t_stjecatCD char(5)  NOT NULL default '' AFTER t_rsDO     ,  " +
                         "ADD COLUMN t_primDohCD char(5)  NOT NULL default '' AFTER t_stjecatCD,  " +
                         "ADD COLUMN t_pocKrajCD char(5)  NOT NULL default '' AFTER t_primDohCD;\n");

         case 3: return ("ADD COLUMN t_ip1gr       int(10)     unsigned  NOT NULL default '0' AFTER t_pocKrajCD;\n");

         case 4: return ("ADD COLUMN t_rbrIsprJop  int(10)               NOT NULL default '0' AFTER t_ip1gr    ;\n");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePtrane

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Ptrane ptrane = (Ptrane)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_parentID,   TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_dokNum,     TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_serial,     TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_dokDate,    TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_TT,         TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_ttNum,      TheSchemaTable.Rows[CI.t_ttNum]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_personCD,   TheSchemaTable.Rows[CI.t_personCD]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_ime,        TheSchemaTable.Rows[CI.t_ime]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_prezime,    TheSchemaTable.Rows[CI.t_prezime]);

      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_vrstaR_cd  , TheSchemaTable.Rows[CI.t_vrstaR_cd  ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_vrstaR_name, TheSchemaTable.Rows[CI.t_vrstaR_name]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_cijPerc    , TheSchemaTable.Rows[CI.t_cijPerc    ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_sati       , TheSchemaTable.Rows[CI.t_sati       ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_rsOO       , TheSchemaTable.Rows[CI.t_rsOO       ]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_rsOD       , TheSchemaTable.Rows[CI.t_rsOD       ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_rsDO       , TheSchemaTable.Rows[CI.t_rsDO       ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_stjecatCD  , TheSchemaTable.Rows[CI.t_stjecatCD  ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_primDohCD  , TheSchemaTable.Rows[CI.t_primDohCD  ]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_pocKrajCD  , TheSchemaTable.Rows[CI.t_pocKrajCD  ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_ip1gr      , TheSchemaTable.Rows[CI.t_ip1gr      ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrane.T_rbrIsprJop , TheSchemaTable.Rows[CI.t_rbrIsprJop ]);

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      PtraneStruct rdrData = new PtraneStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */ rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */ rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */ rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */ rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate);
      /* 05 */ rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */ rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum);
      /* 07 */ rdrData._t_personCD   = reader.GetUInt32  (CI.t_personCD);
      /* 08 */ rdrData._t_ime        = reader.GetString  (CI.t_ime);
      /* 09 */ rdrData._t_prezime    = reader.GetString  (CI.t_prezime);

      /* 10 */ rdrData._t_vrstaR_cd   = reader.GetString (CI.t_vrstaR_cd  );
      /* 11 */ rdrData._t_vrstaR_name = reader.GetString (CI.t_vrstaR_name);
      /* 12 */ rdrData._t_cijPerc     = reader.GetDecimal(CI.t_cijPerc    );
      /* 13 */ rdrData._t_sati        = reader.GetDecimal(CI.t_sati       );
      /* 14 */ rdrData._t_rsOO        = reader.GetString (CI.t_rsOO       );
      /* 15 */ rdrData._t_rsOD        = reader.GetUInt16 (CI.t_rsOD       );
      /* 16 */ rdrData._t_rsDO        = reader.GetUInt16 (CI.t_rsDO       );
      /* 17 */ rdrData._t_stjecatCD   = reader.GetString (CI.t_stjecatCD  );
      /* 18 */ rdrData._t_primDohCD   = reader.GetString (CI.t_primDohCD  );
      /* 19 */ rdrData._t_pocKrajCD   = reader.GetString (CI.t_pocKrajCD  );
      /* 20 */ rdrData._t_ip1gr       = reader.GetUInt32 (CI.t_ip1gr      );
      /* 21 */ rdrData._t_rbrIsprJop  = reader.GetInt32  (CI.t_rbrIsprJop );

      ((Ptrane)vvDataRecord).CurrentData = rdrData;

      //((Ptrane)vvDataRecord).CalcTransResults();

      return;
   }

   #endregion FillFromDataReader

   #region FillFromTypedPtraneDataRow

   // TODO: vidi treba li mozda za ubuduce genericki pa overridano                   
   // FillFromTypedPtraneDataRow(VvDataRecord vvDataRecord, SomeTypedDataRow dataRow) 
   // fali li ti tu mozda i onih prvih 5 univerzalinih RecID, AddTS, ModTS, ...      

   public static void FillFromTypedPtraneDataRow(Ptrane ptrane_rec, Vektor.DataLayer.DS_Reports.DS_Placa.ptraneRow ptraneRow)
   {
      ptrane_rec.Memset0(0);
   
      /* 01 */   ptrane_rec.T_parentID    = ptraneRow.t_parentID   ;
      /* 02 */   ptrane_rec.T_dokNum      = ptraneRow.t_dokNum     ;
      /* 03 */   ptrane_rec.T_serial      = ptraneRow.t_serial     ;
      /* 04 */   ptrane_rec.T_dokDate     = ptraneRow.t_dokDate    ;
      /* 05 */   ptrane_rec.T_TT          = ptraneRow.t_tt         ;
      /* 06 */   ptrane_rec.T_ttNum       = ptraneRow.t_ttNum      ;
      /* 07 */   ptrane_rec.T_personCD    = ptraneRow.t_personCD   ;
      /* 08 */   ptrane_rec.T_ime         = ptraneRow.t_ime        ;
      /* 09 */   ptrane_rec.T_prezime     = ptraneRow.t_prezime    ;
      /* 10 */   ptrane_rec.T_vrstaR_cd   = ptraneRow.t_vrstaR_cd  ;
      /* 11 */   ptrane_rec.T_vrstaR_name = ptraneRow.t_vrstaR_name;
      /* 12 */   ptrane_rec.T_cijPerc     = ptraneRow.t_cijPerc    ;
      /* 13 */   ptrane_rec.T_sati        = ptraneRow.t_sati       ;
      /* 14 */   ptrane_rec.T_rsOO        = ptraneRow.t_rsOO       ;
      /* 15 */   ptrane_rec.T_rsOD        = ptraneRow.t_rsOD       ;
      /* 16 */   ptrane_rec.T_rsDO        = ptraneRow.t_rsDO       ;
      /* 17 */   ptrane_rec.T_stjecatCD   = ptraneRow.t_stjecatCD  ;
      /* 18 */   ptrane_rec.T_primDohCD   = ptraneRow.t_primDohCD  ;
      /* 19 */   ptrane_rec.T_pocKrajCD   = ptraneRow.t_pocKrajCD  ;
      /* 20 */   ptrane_rec.T_ip1gr       = ptraneRow.t_ip1gr      ;
      /* 21 */   ptrane_rec.T_rbrIsprJop  = ptraneRow.t_rbrIsprJop ;
   }

   #endregion FillFromTypedPtraneDataRow

   #region FillTypedDataRowResults

   //public static void FillTypedDataRowResults(Vektor.DataLayer.DS_Reports.DS_Placa.ptraneRow ptraneRow, Ptrane ptrane_rec)
   //{
   //   /* 01 */ ptraneRow.R_EvrCijena = ptrane_rec.R_EvrCijena;
   //   /* 02 */ ptraneRow.R_EvrBruto  = ptrane_rec.R_EvrBruto ;
   //}

   #endregion FillTypedDataRowResults

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct PtraneCI
   {
      internal int recID;

      /* 01 */  internal int t_parentID;
      /* 02 */  internal int t_dokNum;
      /* 03 */  internal int t_serial;
      /* 04 */  internal int t_dokDate;
      /* 05 */  internal int t_tt;
      /* 06 */  internal int t_ttNum;
      /* 07 */  internal int t_personCD;
      /* 08 */  internal int t_ime;
      /* 09 */  internal int t_prezime;

      /* 10 */  internal int t_vrstaR_cd  ;   
      /* 11 */  internal int t_vrstaR_name;   
      /* 12 */  internal int t_cijPerc    ;   
      /* 13 */  internal int t_sati       ;   
      /* 14 */  internal int t_rsOO       ;   
      /* 15 */  internal int t_rsOD       ;
      /* 16 */  internal int t_rsDO       ;   
      /* 17 */  internal int t_stjecatCD  ;   
      /* 18 */  internal int t_primDohCD  ;   
      /* 19 */  internal int t_pocKrajCD  ;   
      /* 20 */  internal int t_ip1gr      ;   
      /* 21 */  internal int t_rbrIsprJop ;   

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PtraneCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */ CI.t_parentID   = GetSchemaColumnIndex("t_parentID"    );
      /* 02 */ CI.t_dokNum     = GetSchemaColumnIndex("t_dokNum"      );
      /* 03 */ CI.t_serial     = GetSchemaColumnIndex("t_serial"      );
      /* 04 */ CI.t_dokDate    = GetSchemaColumnIndex("t_dokDate"     );
      /* 05 */ CI.t_tt         = GetSchemaColumnIndex("t_tt"          );
      /* 06 */ CI.t_ttNum      = GetSchemaColumnIndex("t_ttNum"       );
      /* 07 */ CI.t_personCD   = GetSchemaColumnIndex("t_personCD"    );
      /* 08 */ CI.t_ime        = GetSchemaColumnIndex("t_ime"         );
      /* 09 */ CI.t_prezime    = GetSchemaColumnIndex("t_prezime"     );

      /* 10 */ CI.t_vrstaR_cd   = GetSchemaColumnIndex("t_vrstaR_cd"  );
      /* 11 */ CI.t_vrstaR_name = GetSchemaColumnIndex("t_vrstaR_name");
      /* 12 */ CI.t_cijPerc     = GetSchemaColumnIndex("t_cijPerc"    );
      /* 13 */ CI.t_sati        = GetSchemaColumnIndex("t_sati"       );
      /* 14 */ CI.t_rsOO        = GetSchemaColumnIndex("t_rsOO"       );
      /* 15 */ CI.t_rsOD        = GetSchemaColumnIndex("t_rsOD"       );
      /* 16 */ CI.t_rsDO        = GetSchemaColumnIndex("t_rsDO"       );
      /* 17 */ CI.t_stjecatCD   = GetSchemaColumnIndex("t_stjecatCD"  );
      /* 18 */ CI.t_primDohCD   = GetSchemaColumnIndex("t_primDohCD"  );
      /* 19 */ CI.t_pocKrajCD   = GetSchemaColumnIndex("t_pocKrajCD"  );
      /* 20 */ CI.t_ip1gr       = GetSchemaColumnIndex("t_ip1gr"      );
      /* 21 */ CI.t_rbrIsprJop  = GetSchemaColumnIndex("t_rbrIsprJop" );

   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region Set_IMPORT_OFFIX_Columns

//     //____ Specifics 2 start ______________________________________________________
          
//   if(notZero(ptrans_rec[0].t_evr_hrs1) || ptrans_rec[0].t_evr_OO1[0])
//   {
//"t_dokNum     , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_br);	
//"t_serial     , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_serial);
//"t_dokDate    , "*/ fprintf(device, "%s\t"   , GetMySqlDate(ptrans_rec[0].t_date));
//"t_personCD   , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_odohod_cd);
//"t_ime        , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_ime);
//"t_prezime    , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_prezime);
//"t_vrstaR_cd  , "*/ fprintf(device, "%s\t"   , MakeUpVrstaRadaCD(ptrans_rec[0].t_evr_name1));
//"t_vrstaR_name, "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_name1);
//"t_cijPerc    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_stopa1);
//"t_sati       , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_hrs1);
//"t_rsOO       , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_OO1);
//"t_rsOD       , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_evr_od1);
//"t_rsDO       , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_evr_do1);
//
//      fprintf(device, "\n");
//   }

   public override string Set_IMPORT_OFFIX_Columns()
   {
      return

         "(" +

         "t_dokNum     , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_br);	
         "t_serial     , " + // fprintf(device, "%d\t"   , ptrans_rec[0].t_serial);
         "t_dokDate    , " + // fprintf(device, "%s\t"   , GetMySqlDate(ptrans_rec[0].t_date));
         "t_personCD   , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_odohod_cd);
         "t_ime        , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_ime);
         "t_prezime    , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_prezime);
         "t_vrstaR_cd  , " + // fprintf(device, "%s\t"   , MakeUpVrstaRadaCD(ptrans_rec[0].t_evr_name1));
         "t_vrstaR_name, " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_name1);
         "t_cijPerc    , " + // fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_stopa1);
         "t_sati       , " + // fprintf(device, "%.2lf\t", ptrans_rec[0].t_evr_hrs1);
         "t_rsOO       , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_OO1);
         "t_rsOD       , " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_od1);
         "t_rsDO         " + // fprintf(device, "%s\t"   , ptrans_rec[0].t_evr_do1);
         
         ")"    + "\n" +

         "SET " + "\n" +

         "t_tt       = 'RR'      , " +
         "t_ttNum    = t_dokNum  , " +
         "t_parentID = t_dokNum + 1"; // zbog obracuna br 0. a RecId nemre biti 0!
   }

   internal static void ImportFromOffix_Translate437(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Ptrane>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Ptrane ptrane_rec = vvDataRecord as Ptrane;

      ptrane_rec.T_ime       = ptrane_rec.T_ime    .VvTranslate437ToLatin2();
      ptrane_rec.T_prezime   = ptrane_rec.T_prezime.VvTranslate437ToLatin2();

      return ptrane_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns

}
