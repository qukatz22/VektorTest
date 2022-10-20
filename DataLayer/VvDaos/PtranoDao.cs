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
using System.Collections.Generic;
#endif

public sealed class PtranoDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static PtranoDao instance;

   private PtranoDao(XSqlConnection conn, string dbName) : base(dbName, Ptrano.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static PtranoDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new PtranoDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTablePtrano

   public static   uint TableVersionStatic { get { return 5; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID        int(10)      unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID   int(10)      unsigned NOT NULL               ,\n" +
         /* 02 */  "t_dokNum     int(10)      unsigned NOT NULL               ,\n" +
         /* 03 */  "t_serial     smallint(5)  unsigned NOT NULL               ,\n" +
         /* 04 */  "t_dokDate    date                  NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt         char(3)               NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum      int(10)      unsigned NOT NULL               ,\n" +
         /* 07 */  "t_personCD   int(6)       unsigned NOT NULL default '0'   ,\n" +
         /* 08 */  "t_ime        varchar(24)           NOT NULL default ''    ,\n" +
         /* 09 */  "t_prezime    varchar(32)           NOT NULL default ''    ,\n" +

         /* 10 */  "t_dateStart  date                  NOT NULL default '0001-01-01',\n" +
         /* 11 */  "t_ukBrRata   smallint(4)  unsigned NOT NULL default '0'    ,\n" +
         /* 12 */  "t_opisOb     varchar(64)           NOT NULL default ''     ,\n" +
         /* 13 */  "t_kupdob_cd  int(6)       unsigned NOT NULL default '0'    ,\n" +
         /* 14 */  "t_kupdob_tk  char(6)               NOT NULL default ''     ,\n" +
         /* 15 */  "t_iznosOb    decimal(12,2)         NOT NULL default '0.00' ,\n" +
         /* 16 */  "t_isZbir     tinyint(1)   unsigned NOT NULL default '0'    ,\n" +
         /* 17 */  "t_partija    varchar(32)           NOT NULL default ''     ,\n" +
         /* 18 */  "t_izNetoaSt  decimal(5,2)          NOT NULL default '0.00' ,\n" +
         /* 19 */  "t_rbrRate    int(4)       unsigned NOT NULL default '0'    ,\n" +
         /* 20 */  "t_isZastRn   tinyint(1)   unsigned NOT NULL default '0'    ,\n" + // ptranoKind

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_dokNum,    t_serial)                     ,\n" +
          /*"UNIQUE*/" KEY BY_PERSON     (t_personCD, t_dokDate,   t_dokNum,  t_serial)           \n"
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Ptrano.recordNameArhiva;
      else         tableName = Ptrano.recordName;

      string dbName = VvSQL.GetDbNameForThisTableName(tableName);

      // Just few remarcks: 
      // 1. Ovo gore table name i dbName ti nece trebati za druge tablice. Ovdje je to specificno zbog Kupdob--->Prjkt inheritance-a 
      //    a i inace ti tableName i dbName trebaju samo zbog UPDATE clauzule a ne i za 'obicni'ADD COLUMN (UPDATE ti je isao da popunis novododanu kolonu kupdobCD sa unikatnim prjktKupdobCD vrijednostima) 
      // 2. Ovu pretumbaciju sa dodavanjem kupdobCD-a si morao u dvije faze (ver. 2, pa ver. 3) jer nemozes imati UNIQUE index dok je kupdobCD prazan.

      switch(catchingVersion)
      {
         case 2: return ("ADD COLUMN t_partija    varchar(32)           NOT NULL default ''     AFTER t_isZbir   ; \n");
         case 3: return ("ADD COLUMN t_izNetoaSt  decimal(5,2)          NOT NULL default '0.00' AFTER t_partija  ; \n");
         case 4: return ("ADD COLUMN t_rbrRate    int(4)       unsigned NOT NULL default '0'    AFTER t_izNetoaSt; \n");
         case 5: return ("ADD COLUMN t_isZastRn   tinyint(1)   unsigned NOT NULL default '0'    AFTER t_rbrRate  ; \n");

         //case 4: trlababalan itd..., return ("DROP COLUMN pero, DROP COLUMN jozo");

         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTablePtrano

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Ptrano ptrano = (Ptrano)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_parentID,   TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_dokNum,     TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_serial,     TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_dokDate,    TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_TT,         TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_ttNum,      TheSchemaTable.Rows[CI.t_ttNum]);
      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_personCD,   TheSchemaTable.Rows[CI.t_personCD]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_ime,        TheSchemaTable.Rows[CI.t_ime]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_prezime,    TheSchemaTable.Rows[CI.t_prezime]);

      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_dateStart  , TheSchemaTable.Rows[CI.t_dateStart]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_ukBrRata   , TheSchemaTable.Rows[CI.t_ukBrRata ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_opisOb     , TheSchemaTable.Rows[CI.t_opisOb   ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_kupdob_cd  , TheSchemaTable.Rows[CI.t_kupdob_cd]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_kupdob_tk  , TheSchemaTable.Rows[CI.t_kupdob_tk]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_iznosOb    , TheSchemaTable.Rows[CI.t_iznosOb  ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_isZbir     , TheSchemaTable.Rows[CI.t_isZbir   ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_partija    , TheSchemaTable.Rows[CI.t_partija  ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_izNetoaSt  , TheSchemaTable.Rows[CI.t_izNetoaSt]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_rbrRate    , TheSchemaTable.Rows[CI.t_rbrRate  ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, ptrano.T_ptranoKind , TheSchemaTable.Rows[CI.t_isZastRn ]); // ptranoKind

      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      PtranoStruct rdrData = new PtranoStruct();

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

      /* 10 */ rdrData._t_dateStart  = reader.GetDateTime(CI.t_dateStart);
      /* 11 */ rdrData._t_ukBrRata   = reader.GetUInt32  (CI.t_ukBrRata );
      /* 12 */ rdrData._t_opisOb     = reader.GetString  (CI.t_opisOb   );
      /* 13 */ rdrData._t_kupdob_cd  = reader.GetUInt32  (CI.t_kupdob_cd);
      /* 14 */ rdrData._t_kupdob_tk  = reader.GetString  (CI.t_kupdob_tk);
      /* 15 */ rdrData._t_iznosOb    = reader.GetDecimal (CI.t_iznosOb  );
      /* 16 */ rdrData._t_isZbir     = reader.GetBoolean (CI.t_isZbir   );
      /* 17 */ rdrData._t_partija    = reader.GetString  (CI.t_partija  );
      /* 18 */ rdrData._t_izNetoaSt  = reader.GetDecimal (CI.t_izNetoaSt);
      /* 19 */ rdrData._t_rbrRate    = reader.GetUInt32  (CI.t_rbrRate  );
      /* 20 */ rdrData._t_isZastRn   = reader.GetUInt16  (CI.t_isZastRn ); // ptranoKind

      ((Ptrano)vvDataRecord).CurrentData = rdrData;

      //((Ptrano)vvDataRecord).CalcTransResults();

      return;
   }

   #endregion FillFromDataReader

   #region FillFromTypedPtranoDataRow

   // TODO: vidi treba li mozda za ubuduce genericki pa overridano                   
   // FillFromTypedPtranoDataRow(VvDataRecord vvDataRecord, SomeTypedDataRow dataRow) 
   // fali li ti tu mozda i onih prvih 5 univerzalinih RecID, AddTS, ModTS, ...      

   public static void FillFromTypedPtranoDataRow(Ptrano ptrano_rec, Vektor.DataLayer.DS_Reports.DS_Placa.ptranoRow ptranoRow)
   {
      ptrano_rec.Memset0(0);
   
      /* 01 */   ptrano_rec.T_parentID  = ptranoRow.t_parentID ;
      /* 02 */   ptrano_rec.T_dokNum    = ptranoRow.t_dokNum   ;
      /* 03 */   ptrano_rec.T_serial    = ptranoRow.t_serial   ;
      /* 04 */   ptrano_rec.T_dokDate   = ptranoRow.t_dokDate  ;
      /* 05 */   ptrano_rec.T_TT        = ptranoRow.t_tt       ;
      /* 06 */   ptrano_rec.T_ttNum     = ptranoRow.t_ttNum    ;
      /* 07 */   ptrano_rec.T_personCD  = ptranoRow.t_personCD ;
      /* 08 */   ptrano_rec.T_ime       = ptranoRow.t_ime      ;
      /* 09 */   ptrano_rec.T_prezime   = ptranoRow.t_prezime  ;
      /* 10 */   ptrano_rec.T_dateStart = ptranoRow.t_dateStart;
      /* 11 */   ptrano_rec.T_ukBrRata  = ptranoRow.t_ukBrRata ;
      /* 12 */   ptrano_rec.T_opisOb    = ptranoRow.t_opisOb   ;
      /* 13 */   ptrano_rec.T_kupdob_cd = ptranoRow.t_kupdob_cd;
      /* 14 */   ptrano_rec.T_kupdob_tk = ptranoRow.t_kupdob_tk;
      /* 15 */   ptrano_rec.T_iznosOb   = ptranoRow.t_iznosOb  ;
      /* 16 */   ptrano_rec.T_isZbir    = Convert.ToBoolean(ptranoRow.t_isZbir);
      /* 17 */   ptrano_rec.T_partija   = ptranoRow.t_partija  ;
      /* 18 */   ptrano_rec.T_izNetoaSt = ptranoRow.t_izNetoaSt;
      /* 19 */   ptrano_rec.T_rbrRate   = ptranoRow.t_rbrRate  ;
      /* 20 */   ptrano_rec.T_ptranoKind= (Ptrans.PtranoKind)ptranoRow.t_isZastRn; // ptranoKind
   }

   #endregion FillFromTypedPtranoDataRow

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct PtranoCI
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

      /* 10 */  internal int t_dateStart;   
      /* 11 */  internal int t_ukBrRata ;   
      /* 12 */  internal int t_opisOb   ;   
      /* 13 */  internal int t_kupdob_cd;   
      /* 14 */  internal int t_kupdob_tk;  
      /* 15 */  internal int t_iznosOb  ;
      /* 16 */  internal int t_isZbir   ;
      /* 17 */  internal int t_partija  ;
      /* 18 */  internal int t_izNetoaSt;
      /* 19 */  internal int t_rbrRate  ;
      /* 20 */  internal int t_isZastRn ; // ptranoKind

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public PtranoCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      /* 01 */ CI.t_parentID   = GetSchemaColumnIndex("t_parentID"  );
      /* 02 */ CI.t_dokNum     = GetSchemaColumnIndex("t_dokNum"    );
      /* 03 */ CI.t_serial     = GetSchemaColumnIndex("t_serial"    );
      /* 04 */ CI.t_dokDate    = GetSchemaColumnIndex("t_dokDate"   );
      /* 05 */ CI.t_tt         = GetSchemaColumnIndex("t_tt"        );
      /* 06 */ CI.t_ttNum      = GetSchemaColumnIndex("t_ttNum"     );
      /* 07 */ CI.t_personCD   = GetSchemaColumnIndex("t_personCD"  );
      /* 08 */ CI.t_ime        = GetSchemaColumnIndex("t_ime"       );
      /* 09 */ CI.t_prezime    = GetSchemaColumnIndex("t_prezime"   );

      /* 10 */ CI.t_dateStart  = GetSchemaColumnIndex("t_dateStart");
      /* 11 */ CI.t_ukBrRata   = GetSchemaColumnIndex("t_ukBrRata" );
      /* 12 */ CI.t_opisOb     = GetSchemaColumnIndex("t_opisOb"   );
      /* 13 */ CI.t_kupdob_cd  = GetSchemaColumnIndex("t_kupdob_cd");
      /* 14 */ CI.t_kupdob_tk  = GetSchemaColumnIndex("t_kupdob_tk");
      /* 15 */ CI.t_iznosOb    = GetSchemaColumnIndex("t_iznosOb"  );
      /* 16 */ CI.t_isZbir     = GetSchemaColumnIndex("t_isZbir"   );
      /* 17 */ CI.t_partija    = GetSchemaColumnIndex("t_partija"  );
      /* 18 */ CI.t_izNetoaSt  = GetSchemaColumnIndex("t_izNetoaSt");
      /* 19 */ CI.t_rbrRate    = GetSchemaColumnIndex("t_rbrRate"  );
      /* 20 */ CI.t_isZastRn   = GetSchemaColumnIndex("t_isZastRn" ); // ptranoKind

   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region GetMaxObustavaRBR

   internal static uint GetMaxObustavaRBR(XSqlConnection conn, uint personCD, uint kupdobCD, string t_partija)
   {
      #region Filter Memberz

      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      // For wanted personCD only                                                                                                                                            
      drSchema = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_personCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPerson", personCD, " = "));

      // For wanted kupdobCD only                                                                                                                                            
      drSchema = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_kupdob_cd];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elkupdobCD", kupdobCD, " = "));

      // For wanted t_partija only                                                                                                                                            
      drSchema = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_partija];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elT_partija", t_partija, " = "));

      #endregion Filter Memberz

      return GetMaxUsedValueFromUintColumn(conn, filterMembers, "t_rbrRate");
   }

   #endregion GetMaxObustavaRBR

   #region Set_IMPORT_OFFIX_Columns

   //     //____ Specifics 2 start ______________________________________________________
          
//   if(notZero(ptrans_rec[0].t_ob_izn1))
//   {
//      if(SetMe_odohod(ptrans_rec[0].t_odohod_cd, 0)) memset(&odohod_rec[0], 0, sizeof(struct odohod));
   
//"t_dokNum     , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_br);	
//"t_serial     , "*/ fprintf(device, "%d\t"   , ptrans_rec[0].t_serial);
//"t_dokDate    , "*/ fprintf(device, "%s\t"   , GetMySqlDate(ptrans_rec[0].t_date));
//"t_personCD   , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_odohod_cd);
//"t_ime        , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_ime);
//"t_prezime    , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_prezime);

//"t_dateStart  , "*/ fprintf(device, "%s\t"   , GetMySqlDate(odohod_rec[0].o_ob_date1));	
//"t_ukBrRata   , "*/ fprintf(device, "%.0lf\t", odohod_rec[0].o_ob_brR1);	
//"t_opisOb     , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_ob_naz1);	
//"t_kupdob_cd  , "*/ fprintf(device, "%s\t"   , ptrans_rec[0].t_ob_kcd1);	
//"t_iznosOb    , "*/ fprintf(device, "%.2lf\t", ptrans_rec[0].t_ob_izn1);	

//      fprintf(device, "\n");
//      ++num;
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
         "t_dateStart  , " + //fprintf(device, "%s\t"   , GetMySqlDate(odohod_rec[0].o_ob_date1));	
         "t_ukBrRata   , " + //fprintf(device, "%.0lf\t", odohod_rec[0].o_ob_brR1);	
         "t_opisOb     , " + //fprintf(device, "%s\t"   , ptrans_rec[0].t_ob_naz1);	
         "t_kupdob_cd  , " + //fprintf(device, "%s\t"   , ptrans_rec[0].t_ob_kcd1);	
         "t_iznosOb      " + //fprintf(device, "%.2lf\t", ptrans_rec[0].t_ob_izn1);	

         ")" + "\n" +

         "SET " + "\n" +

         "t_tt       = 'RR'      , " +
         "t_ttNum    = t_dokNum  , " +
         "t_parentID = t_dokNum + 1"; // zbog obracuna br 0. a RecId nemre biti 0!
   }

   internal static void ImportFromOffix_Translate437_SetTickers(XSqlConnection conn)
   {
      int debugCount;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Ptrano>(conn, Translate437, null, "", ZXC.TheVvForm.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);
   }

   static bool Translate437(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Ptrano ptrano_rec = vvDataRecord as Ptrano;

      ptrano_rec.T_ime       = ptrano_rec.T_ime    .VvTranslate437ToLatin2();
      ptrano_rec.T_prezime   = ptrano_rec.T_prezime.VvTranslate437ToLatin2();
      ptrano_rec.T_opisOb    = ptrano_rec.T_opisOb .VvTranslate437ToLatin2();

      if(ptrano_rec.T_kupdob_cd.NotZero())
      {
         Kupdob kupdob_rec = new Kupdob(ptrano_rec.T_kupdob_cd);

         bool OK = kupdob_rec.VvDao.SetMe_Record_bySomeUniqueColumn(conn, kupdob_rec, ptrano_rec.T_kupdob_cd, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false, false);

         if(OK)
         {
            ptrano_rec.T_kupdob_tk = kupdob_rec.Ticker;
         }
      }

      return ptrano_rec.EditedHasChanges();
   }

   #endregion Set_IMPORT_OFFIX_Columns
}
