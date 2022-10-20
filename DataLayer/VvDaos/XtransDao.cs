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

public sealed class XtransDao : VvDaoBase, IVvDao
{

   #region Singleton Constructor & instancer

   private static XtransDao instance;

   private XtransDao(XSqlConnection conn, string dbName) : base(dbName, Xtrans.recordName, conn) // nedas da se moze instancirati, a ono sealed goreje da se neda inheritirati  
   {
   }

   public static XtransDao Instance(XSqlConnection conn, string dbName)
   {
      // Uses lazy initialization
      if(instance == null)
      {
         instance = new XtransDao(conn, dbName);
      }

      return instance;
   }

   #endregion Singleton Constructor & instancer

   #region CreateTableXtrans

   public static   uint TableVersionStatic { get { return 7; } }

   public override uint TableVersion       { get { return TableVersionStatic; } }

   public static string Create_table_definition()
   {
      return (
         /* 00 */  "recID       int(10)     unsigned NOT NULL auto_increment,\n" +
         /* 01 */  "t_parentID  int(10)     unsigned NOT NULL,\n" +
         /* 02 */  "t_dokNum    int(10)     unsigned NOT NULL,\n" +
         /* 03 */  "t_serial    smallint(5) unsigned NOT NULL,\n" +
         /* 04 */  "t_dokDate   date                 NOT NULL default '0001-01-01',\n" +
         /* 05 */  "t_tt        char(3)              NOT NULL default ''    ,\n" +
         /* 06 */  "t_ttNum     int(10)     unsigned NOT NULL default '0'   ,\n" +

         /* 07 */  "t_dateOd         datetime        NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 08 */  "t_dateDo         datetime        NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 09 */  "t_moneyA         decimal(14,6)   NOT NULL default '0.00',\n" +
         /* 10 */  "t_kol            decimal(12,4)   NOT NULL default '0.00',\n" +     
         /* 11 */  "t_opis_128       varchar(128)    NOT NULL default ''    ,\n" +
         /* 12 */  "t_kpdbNameA_50   varchar( 50)    NOT NULL default ''    ,\n" +
         /* 13 */  "t_kpdbUlBrA_32   varchar( 32)    NOT NULL default ''    ,\n" +
         /* 14 */  "t_kpdbMjestoA_32 varchar( 32)    NOT NULL default ''    ,\n" +
         /* 15 */  "t_kpdbZiroA_32   varchar( 32)    NOT NULL default ''    ,\n" +
         /* 16 */  "t_kpdbNameB_50   varchar( 50)    NOT NULL default ''    ,\n" +
         /* 17 */  "t_kpdbUlBrB_32   varchar( 32)    NOT NULL default ''    ,\n" +
         /* 18 */  "t_kpdbMjestoB_32 varchar( 32)    NOT NULL default ''    ,\n" +
         /* 19 */  "t_kpdbZiroB_32   varchar( 32)    NOT NULL default ''    ,\n" +
         /* 20 */  "t_vezniDokA_64   varchar( 64)    NOT NULL default ''    ,\n" +
         /* 21 */  "t_vezniDokB_64   varchar( 64)    NOT NULL default ''    ,\n" +
         /* 22 */  "t_strA_2            char(  4)    NOT NULL default ''    ,\n" +
         /* 23 */  "t_strB_2            char(  4)    NOT NULL default ''    ,\n" +
         /* 24 */  "t_strC_2            char(  2)    NOT NULL default ''    ,\n" +
         /* 25 */  "t_kupdobCD       int(6) unsigned NOT NULL default '0'   ,\n" +
         /* 26 */  "t_artiklCD       varchar(32)     NOT NULL default ''    ,\n" +
         /* 27 */  "t_artiklName     varchar(80)     NOT NULL default ''    ,\n" +
         /* 28 */  "t_isXxx  tinyint(1)     unsigned NOT NULL default '0'   ,\n" +
         /* 29 */  "t_intA           int(10)         NOT NULL default '0'   ,\n" +
         /* 30 */  "t_intB           int(10)         NOT NULL default '0'   ,\n" +
         /* 31 */  "t_konto          varchar(8)      NOT NULL default ''    ,\n" +
         /* 32 */  "t_personCD       int(6) unsigned NOT NULL default '0'   ,\n" +
         /* 33 */  "t_moneyB         decimal(14,6)   NOT NULL default '0.00',\n" +
         /* 34 */  "t_moneyC         decimal(14,6)   NOT NULL default '0.00',\n" +
         /* 35 */  "t_moneyD         decimal(14,6)   NOT NULL default '0.00',\n" +

         /* 36 */ "t_dec01           decimal(10,2)    NOT NULL default '0.00',\n" +
         /* 37 */ "t_dec02           decimal(10,2)    NOT NULL default '0.00',\n" +
         /* 38 */ "t_dec03           decimal(10,2)    NOT NULL default '0.00',\n" +
         /* 39 */ "t_dec04           decimal(10,2)    NOT NULL default '0.00',\n" +
         /* 40 */ "t_dec05           decimal(10,2)    NOT NULL default '0.00',\n" +
         /* 41 */ "t_dec06           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 42 */ "t_dec07           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 43 */ "t_dec08           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 44 */ "t_dec09           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 45 */ "t_dec10           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 46 */ "t_dec11           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 47 */ "t_dec12           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 48 */ "t_dec13           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 49 */ "t_dec14           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 50 */ "t_dec15           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 51 */ "t_dec16           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 52 */ "t_dec17           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 53 */ "t_dec18           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 54 */ "t_dec19           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 55 */ "t_dec20           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 56 */ "t_dec21           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 57 */ "t_dec22           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 58 */ "t_dec23           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 59 */ "t_dec24           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 60 */ "t_dec25           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 61 */ "t_dec26           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 62 */ "t_dec27           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 63 */ "t_dec28           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 64 */ "t_dec29           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 65 */ "t_dec30           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 66 */ "t_dec31           decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 67 */ "t_str01           char(3)         NOT NULL default ''     ,\n" +
         /* 68 */ "t_str02           char(3)         NOT NULL default ''     ,\n" +
         /* 69 */ "t_str03           char(3)         NOT NULL default ''     ,\n" +
         /* 70 */ "t_str04           char(3)         NOT NULL default ''     ,\n" +
         /* 71 */ "t_str05           char(3)         NOT NULL default ''     ,\n" +
         /* 72 */ "t_str06           char(3)         NOT NULL default ''     ,\n" +
         /* 73 */ "t_str07           char(3)         NOT NULL default ''     ,\n" +
         /* 74 */ "t_str08           char(3)         NOT NULL default ''     ,\n" +
         /* 75 */ "t_str09           char(3)         NOT NULL default ''     ,\n" +
         /* 76 */ "t_str10           char(3)         NOT NULL default ''     ,\n" +
         /* 77 */ "t_str11           char(3)         NOT NULL default ''     ,\n" +
         /* 78 */ "t_str12           char(3)         NOT NULL default ''     ,\n" +
         /* 79 */ "t_str13           char(3)         NOT NULL default ''     ,\n" +
         /* 70 */ "t_str14           char(3)         NOT NULL default ''     ,\n" +
         /* 81 */ "t_str15           char(3)         NOT NULL default ''     ,\n" +
         /* 82 */ "t_str16           char(3)         NOT NULL default ''     ,\n" +
         /* 83 */ "t_str17           char(3)         NOT NULL default ''     ,\n" +
         /* 84 */ "t_str18           char(3)         NOT NULL default ''     ,\n" +
         /* 85 */ "t_str19           char(3)         NOT NULL default ''     ,\n" +
         /* 86 */ "t_str20           char(3)         NOT NULL default ''     ,\n" +
         /* 87 */ "t_str21           char(3)         NOT NULL default ''     ,\n" +
         /* 88 */ "t_str22           char(3)         NOT NULL default ''     ,\n" +
         /* 89 */ "t_str23           char(3)         NOT NULL default ''     ,\n" +
         /* 90 */ "t_str24           char(3)         NOT NULL default ''     ,\n" +
         /* 91 */ "t_str25           char(3)         NOT NULL default ''     ,\n" +
         /* 92 */ "t_str26           char(3)         NOT NULL default ''     ,\n" +
         /* 93 */ "t_str27           char(3)         NOT NULL default ''     ,\n" +
         /* 94 */ "t_str28           char(3)         NOT NULL default ''     ,\n" +
         /* 95 */ "t_str29           char(3)         NOT NULL default ''     ,\n" +
         /* 96 */ "t_str30           char(3)         NOT NULL default ''     ,\n" +
         /* 97 */ "t_str31           char(3)         NOT NULL default ''     ,\n" +
         /* 98 */ "t_dec01_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /* 99 */ "t_dec02_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*100 */ "t_dec03_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*101 */ "t_dec04_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*102 */ "t_dec05_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*103 */ "t_dec06_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*104 */ "t_dec07_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*105 */ "t_dec08_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*106 */ "t_dec09_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*107 */ "t_dec10_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*108 */ "t_dec11_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*109 */ "t_dec12_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*110 */ "t_dec13_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*111 */ "t_dec14_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*112 */ "t_dec15_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*113 */ "t_dec16_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*114 */ "t_dec17_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*115 */ "t_dec18_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*116 */ "t_dec19_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*117 */ "t_dec20_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*118 */ "t_dec21_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*119 */ "t_dec22_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*120 */ "t_dec23_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*121 */ "t_dec24_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*122 */ "t_dec25_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*123 */ "t_dec26_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*124 */ "t_dec27_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*125 */ "t_dec28_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*126 */ "t_dec29_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*127 */ "t_dec30_2         decimal( 6,2)    NOT NULL default '0.00',\n" +
         /*128 */ "t_dec31_2         decimal( 6,2)    NOT NULL default '0.00',\n" +

         /* 129*/ "t_date3           datetime         NOT NULL default '0001-01-01 00:00:00',\n" +
         /* 130*/ "t_date4           datetime         NOT NULL default '0001-01-01 00:00:00',\n" +

          "PRIMARY KEY                   (recID)                                                 ,\n" +
          /*"UNIQUE*/" KEY BY_LINKER     (t_parentID, t_serial)                                  ,\n" +
          /*"UNIQUE*/" KEY BY_DOKDATE    (t_dokDate,  t_tt, t_ttNum, t_serial)                   ,\n" +
          /*"UNIQUE*/" KEY BY_TTNUM      (            t_tt, t_ttNum, t_serial)                    \n" 
         );
   }

   public static string Alter_table_definition(uint catchingVersion, bool isArhiva)
   {
      string tableName;

      if(isArhiva) tableName = Xtrans.recordNameArhiva;
      else         tableName = Xtrans.recordName;

      switch(catchingVersion)
      {

         case 2: return ("ADD t_moneyB         decimal(14,6)   NOT NULL default '0.00' AFTER t_personCD,  " +
                         "ADD t_moneyC         decimal(14,6)   NOT NULL default '0.00' AFTER t_moneyB  ,  " +
                         "ADD t_moneyD         decimal(14,6)   NOT NULL default '0.00' AFTER t_moneyC  ;\n");

         case 3: return ("MODIFY COLUMN t_strA_2            char(  4)         NOT NULL default '', " +
                         "MODIFY COLUMN t_strB_2            char(  4)         NOT NULL default ''  ");

         case 4: return ("ADD t_dec01           decimal(6,2)    NOT NULL default '0.00' AFTER t_moneyD, " +
                         "ADD t_dec02           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec01,  " +
                         "ADD t_dec03           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec02,  " +
                         "ADD t_dec04           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec03,  " +
                         "ADD t_dec05           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec04,  " +
                         "ADD t_dec06           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec05,  " +
                         "ADD t_dec07           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec06,  " +
                         "ADD t_dec08           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec07,  " +
                         "ADD t_dec09           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec08,  " +
                         "ADD t_dec10           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec09,  " +
                         "ADD t_dec11           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec10,  " +
                         "ADD t_dec12           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec11,  " +
                         "ADD t_dec13           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec12,  " +
                         "ADD t_dec14           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec13,  " +
                         "ADD t_dec15           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec14,  " +
                         "ADD t_dec16           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec15,  " +
                         "ADD t_dec17           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec16,  " +
                         "ADD t_dec18           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec17,  " +
                         "ADD t_dec19           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec18,  " +
                         "ADD t_dec20           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec19,  " +
                         "ADD t_dec21           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec20,  " +
                         "ADD t_dec22           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec21,  " +
                         "ADD t_dec23           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec22,  " +
                         "ADD t_dec24           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec23,  " +
                         "ADD t_dec25           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec24,  " +
                         "ADD t_dec26           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec25,  " +
                         "ADD t_dec27           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec26,  " +
                         "ADD t_dec28           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec27,  " +
                         "ADD t_dec29           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec28,  " +
                         "ADD t_dec30           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec29,  " +
                         "ADD t_dec31           decimal(6,2)    NOT NULL default '0.00' AFTER t_dec30,  " +
                         "ADD t_str01           char(3)         NOT NULL default ''     AFTER t_dec31,  " +
                         "ADD t_str02           char(3)         NOT NULL default ''     AFTER t_str01,  " +
                         "ADD t_str03           char(3)         NOT NULL default ''     AFTER t_str02,  " +
                         "ADD t_str04           char(3)         NOT NULL default ''     AFTER t_str03,  " +
                         "ADD t_str05           char(3)         NOT NULL default ''     AFTER t_str04,  " +
                         "ADD t_str06           char(3)         NOT NULL default ''     AFTER t_str05,  " +
                         "ADD t_str07           char(3)         NOT NULL default ''     AFTER t_str06,  " +
                         "ADD t_str08           char(3)         NOT NULL default ''     AFTER t_str07,  " +
                         "ADD t_str09           char(3)         NOT NULL default ''     AFTER t_str08,  " +
                         "ADD t_str10           char(3)         NOT NULL default ''     AFTER t_str09,  " +
                         "ADD t_str11           char(3)         NOT NULL default ''     AFTER t_str10,  " +
                         "ADD t_str12           char(3)         NOT NULL default ''     AFTER t_str11,  " +
                         "ADD t_str13           char(3)         NOT NULL default ''     AFTER t_str12,  " +
                         "ADD t_str14           char(3)         NOT NULL default ''     AFTER t_str13,  " +
                         "ADD t_str15           char(3)         NOT NULL default ''     AFTER t_str14,  " +
                         "ADD t_str16           char(3)         NOT NULL default ''     AFTER t_str15,  " +
                         "ADD t_str17           char(3)         NOT NULL default ''     AFTER t_str16,  " +
                         "ADD t_str18           char(3)         NOT NULL default ''     AFTER t_str17,  " +
                         "ADD t_str19           char(3)         NOT NULL default ''     AFTER t_str18,  " +
                         "ADD t_str20           char(3)         NOT NULL default ''     AFTER t_str19,  " +
                         "ADD t_str21           char(3)         NOT NULL default ''     AFTER t_str20,  " +
                         "ADD t_str22           char(3)         NOT NULL default ''     AFTER t_str21,  " +
                         "ADD t_str23           char(3)         NOT NULL default ''     AFTER t_str22,  " +
                         "ADD t_str24           char(3)         NOT NULL default ''     AFTER t_str23,  " +
                         "ADD t_str25           char(3)         NOT NULL default ''     AFTER t_str24,  " +
                         "ADD t_str26           char(3)         NOT NULL default ''     AFTER t_str25,  " +
                         "ADD t_str27           char(3)         NOT NULL default ''     AFTER t_str26,  " +
                         "ADD t_str28           char(3)         NOT NULL default ''     AFTER t_str27,  " +
                         "ADD t_str29           char(3)         NOT NULL default ''     AFTER t_str28,  " +
                         "ADD t_str30           char(3)         NOT NULL default ''     AFTER t_str29,  " +
                         "ADD t_str31           char(3)         NOT NULL default ''     AFTER t_str30;\n");

         case 5: return ("MODIFY COLUMN t_dec01           decimal(10,2)    NOT NULL default '0.00', " +
                         "MODIFY COLUMN t_dec02           decimal(10,2)    NOT NULL default '0.00', " +
                         "MODIFY COLUMN t_dec03           decimal(10,2)    NOT NULL default '0.00', " +
                         "MODIFY COLUMN t_dec04           decimal(10,2)    NOT NULL default '0.00', " +
                         "MODIFY COLUMN t_dec05           decimal(10,2)    NOT NULL default '0.00'  ");

         case 6: return ("ADD t_dec01_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_str31  , " +
                         "ADD t_dec02_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec01_2,  " +
                         "ADD t_dec03_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec02_2,  " +
                         "ADD t_dec04_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec03_2,  " +
                         "ADD t_dec05_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec04_2,  " +
                         "ADD t_dec06_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec05_2,  " +
                         "ADD t_dec07_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec06_2,  " +
                         "ADD t_dec08_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec07_2,  " +
                         "ADD t_dec09_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec08_2,  " +
                         "ADD t_dec10_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec09_2,  " +
                         "ADD t_dec11_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec10_2,  " +
                         "ADD t_dec12_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec11_2,  " +
                         "ADD t_dec13_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec12_2,  " +
                         "ADD t_dec14_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec13_2,  " +
                         "ADD t_dec15_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec14_2,  " +
                         "ADD t_dec16_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec15_2,  " +
                         "ADD t_dec17_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec16_2,  " +
                         "ADD t_dec18_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec17_2,  " +
                         "ADD t_dec19_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec18_2,  " +
                         "ADD t_dec20_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec19_2,  " +
                         "ADD t_dec21_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec20_2,  " +
                         "ADD t_dec22_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec21_2,  " +
                         "ADD t_dec23_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec22_2,  " +
                         "ADD t_dec24_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec23_2,  " +
                         "ADD t_dec25_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec24_2,  " +
                         "ADD t_dec26_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec25_2,  " +
                         "ADD t_dec27_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec26_2,  " +
                         "ADD t_dec28_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec27_2,  " +
                         "ADD t_dec29_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec28_2,  " +
                         "ADD t_dec30_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec29_2,  " +
                         "ADD t_dec31_2         decimal(6,2)    NOT NULL default '0.00' AFTER t_dec30_2;\n");

         case 7: return ("ADD t_date3           datetime        NOT NULL default '0001-01-01 00:00:00' AFTER t_dec31_2,  " +
                         "ADD t_date4           datetime        NOT NULL default '0001-01-01 00:00:00' AFTER t_date3  ;\n");


         default: throw new Exception("For table " + tableName + " version no. " + catchingVersion + " doesn't exists!");
      }
   }

   #endregion CreateTableXtrans

   #region SetCommandParamValues

   public void SetCommandParamValues(XSqlCommand cmd, VvDataRecord vvDataRecord, VvSQL.ParamListType plt, bool dummy)
   {
      string preffix;
      Xtrans xtrans = (Xtrans)vvDataRecord;

      if(plt == VvSQL.ParamListType.Old_Values)
         preffix = "old_";
      else
         preffix = "prm_";

      if(plt == VvSQL.ParamListType.Complete || 
         plt == VvSQL.ParamListType.ID_Only  ||
         plt == VvSQL.ParamListType.Old_Values)
      {
         VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_recID, TheSchemaTable.Rows[CI.recID]);
      }

      if(plt == VvSQL.ParamListType.Complete   || 
         plt == VvSQL.ParamListType.Without_ID ||
         plt == VvSQL.ParamListType.Old_Values)
      {

      /* 01 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_parentID,  TheSchemaTable.Rows[CI.t_parentID]); 
      /* 02 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dokNum,    TheSchemaTable.Rows[CI.t_dokNum]);
      /* 03 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_serial,    TheSchemaTable.Rows[CI.t_serial]);
      /* 04 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dokDate,   TheSchemaTable.Rows[CI.t_dokDate]);
      /* 05 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_TT,        TheSchemaTable.Rows[CI.t_tt]);
      /* 06 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_ttNum,     TheSchemaTable.Rows[CI.t_ttNum]);

      /* 07 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dateOd        ,  TheSchemaTable.Rows[CI.t_dateOd        ]);
      /* 08 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dateDo        ,  TheSchemaTable.Rows[CI.t_dateDo        ]);
      /* 09 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_moneyA        ,  TheSchemaTable.Rows[CI.t_moneyA        ]);
      /* 10 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kol           ,  TheSchemaTable.Rows[CI.t_kol           ]);
      /* 11 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_opis_128      ,  TheSchemaTable.Rows[CI.t_opis_128      ]);
      /* 12 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbNameA_50  ,  TheSchemaTable.Rows[CI.t_kpdbNameA_50  ]);
      /* 13 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbUlBrA_32  ,  TheSchemaTable.Rows[CI.t_kpdbUlBrA_32  ]);
      /* 14 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbMjestoA_32,  TheSchemaTable.Rows[CI.t_kpdbMjestoA_32]);
      /* 15 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbZiroA_32  ,  TheSchemaTable.Rows[CI.t_kpdbZiroA_32  ]);
      /* 16 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbNameB_50  ,  TheSchemaTable.Rows[CI.t_kpdbNameB_50  ]);
      /* 17 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbUlBrB_32  ,  TheSchemaTable.Rows[CI.t_kpdbUlBrB_32  ]);
      /* 18 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbMjestoB_32,  TheSchemaTable.Rows[CI.t_kpdbMjestoB_32]);
      /* 19 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kpdbZiroB_32  ,  TheSchemaTable.Rows[CI.t_kpdbZiroB_32  ]);
      /* 20 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_vezniDokA_64  ,  TheSchemaTable.Rows[CI.t_vezniDokA_64  ]);
      /* 21 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_vezniDokB_64  ,  TheSchemaTable.Rows[CI.t_vezniDokB_64  ]);
      /* 22 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_strA_2        ,  TheSchemaTable.Rows[CI.t_strA_2        ]);
      /* 23 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_strB_2        ,  TheSchemaTable.Rows[CI.t_strB_2        ]);
      /* 24 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_strC_2        ,  TheSchemaTable.Rows[CI.t_strC_2        ]);
      /* 25 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_kupdobCD      ,  TheSchemaTable.Rows[CI.t_kupdobCD      ]);
      /* 26 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_artiklCD      ,  TheSchemaTable.Rows[CI.t_artiklCD      ]);
      /* 27 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_artiklName    ,  TheSchemaTable.Rows[CI.t_artiklName    ]);
      /* 28 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_isXxx         ,  TheSchemaTable.Rows[CI.t_isXxx         ]);
      /* 29 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_intA          ,  TheSchemaTable.Rows[CI.t_intA          ]);
      /* 30 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_intB          ,  TheSchemaTable.Rows[CI.t_intB          ]);
      /* 31 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_konto         ,  TheSchemaTable.Rows[CI.t_konto         ]);
      /* 32 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_personCD      ,  TheSchemaTable.Rows[CI.t_personCD      ]);
      /* 33 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_moneyB        ,  TheSchemaTable.Rows[CI.t_moneyB        ]);
      /* 34 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_moneyC        ,  TheSchemaTable.Rows[CI.t_moneyC        ]);
      /* 35 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_moneyD        ,  TheSchemaTable.Rows[CI.t_moneyD        ]);

      /* 36 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec01         ,  TheSchemaTable.Rows[CI.t_dec01]);
      /* 37 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec02         ,  TheSchemaTable.Rows[CI.t_dec02]);
      /* 38 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec03         ,  TheSchemaTable.Rows[CI.t_dec03]);
      /* 39 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec04         ,  TheSchemaTable.Rows[CI.t_dec04]);
      /* 40 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec05         ,  TheSchemaTable.Rows[CI.t_dec05]);
      /* 41 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec06         ,  TheSchemaTable.Rows[CI.t_dec06]);
      /* 42 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec07         ,  TheSchemaTable.Rows[CI.t_dec07]);
      /* 43 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec08         ,  TheSchemaTable.Rows[CI.t_dec08]);
      /* 44 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec09         ,  TheSchemaTable.Rows[CI.t_dec09]);
      /* 45 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec10         ,  TheSchemaTable.Rows[CI.t_dec10]);
      /* 46 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec11         ,  TheSchemaTable.Rows[CI.t_dec11]);
      /* 47 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec12         ,  TheSchemaTable.Rows[CI.t_dec12]);
      /* 48 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec13         ,  TheSchemaTable.Rows[CI.t_dec13]);
      /* 49 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec14         ,  TheSchemaTable.Rows[CI.t_dec14]);
      /* 50 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec15         ,  TheSchemaTable.Rows[CI.t_dec15]);
      /* 51 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec16         ,  TheSchemaTable.Rows[CI.t_dec16]);
      /* 52 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec17         ,  TheSchemaTable.Rows[CI.t_dec17]);
      /* 53 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec18         ,  TheSchemaTable.Rows[CI.t_dec18]);
      /* 54 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec19         ,  TheSchemaTable.Rows[CI.t_dec19]);
      /* 55 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec20         ,  TheSchemaTable.Rows[CI.t_dec20]);
      /* 56 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec21         ,  TheSchemaTable.Rows[CI.t_dec21]);
      /* 57 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec22         ,  TheSchemaTable.Rows[CI.t_dec22]);
      /* 58 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec23         ,  TheSchemaTable.Rows[CI.t_dec23]);
      /* 59 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec24         ,  TheSchemaTable.Rows[CI.t_dec24]);
      /* 60 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec25         ,  TheSchemaTable.Rows[CI.t_dec25]);
      /* 61 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec26         ,  TheSchemaTable.Rows[CI.t_dec26]);
      /* 62 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec27         ,  TheSchemaTable.Rows[CI.t_dec27]);
      /* 63 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec28         ,  TheSchemaTable.Rows[CI.t_dec28]);
      /* 64 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec29         ,  TheSchemaTable.Rows[CI.t_dec29]);
      /* 65 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec30         ,  TheSchemaTable.Rows[CI.t_dec30]);
      /* 66 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec31         ,  TheSchemaTable.Rows[CI.t_dec31]);
      /* 67 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str01         ,  TheSchemaTable.Rows[CI.t_str01]);
      /* 68 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str02         ,  TheSchemaTable.Rows[CI.t_str02]);
      /* 69 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str03         ,  TheSchemaTable.Rows[CI.t_str03]);
      /* 70 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str04         ,  TheSchemaTable.Rows[CI.t_str04]);
      /* 71 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str05         ,  TheSchemaTable.Rows[CI.t_str05]);
      /* 72 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str06         ,  TheSchemaTable.Rows[CI.t_str06]);
      /* 73 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str07         ,  TheSchemaTable.Rows[CI.t_str07]);
      /* 74 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str08         ,  TheSchemaTable.Rows[CI.t_str08]);
      /* 75 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str09         ,  TheSchemaTable.Rows[CI.t_str09]);
      /* 76 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str10         ,  TheSchemaTable.Rows[CI.t_str10]);
      /* 77 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str11         ,  TheSchemaTable.Rows[CI.t_str11]);
      /* 78 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str12         ,  TheSchemaTable.Rows[CI.t_str12]);
      /* 79 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str13         ,  TheSchemaTable.Rows[CI.t_str13]);
      /* 70 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str14         ,  TheSchemaTable.Rows[CI.t_str14]);
      /* 81 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str15         ,  TheSchemaTable.Rows[CI.t_str15]);
      /* 82 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str16         ,  TheSchemaTable.Rows[CI.t_str16]);
      /* 83 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str17         ,  TheSchemaTable.Rows[CI.t_str17]);
      /* 84 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str18         ,  TheSchemaTable.Rows[CI.t_str18]);
      /* 85 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str19         ,  TheSchemaTable.Rows[CI.t_str19]);
      /* 86 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str20         ,  TheSchemaTable.Rows[CI.t_str20]);
      /* 87 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str21         ,  TheSchemaTable.Rows[CI.t_str21]);
      /* 88 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str22         ,  TheSchemaTable.Rows[CI.t_str22]);
      /* 89 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str23         ,  TheSchemaTable.Rows[CI.t_str23]);
      /* 90 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str24         ,  TheSchemaTable.Rows[CI.t_str24]);
      /* 91 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str25         ,  TheSchemaTable.Rows[CI.t_str25]);
      /* 92 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str26         ,  TheSchemaTable.Rows[CI.t_str26]);
      /* 93 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str27         ,  TheSchemaTable.Rows[CI.t_str27]);
      /* 94 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str28         ,  TheSchemaTable.Rows[CI.t_str28]);
      /* 95 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str29         ,  TheSchemaTable.Rows[CI.t_str29]);
      /* 96 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str30         ,  TheSchemaTable.Rows[CI.t_str30]);
      /* 97 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_str31         ,  TheSchemaTable.Rows[CI.t_str31]);

      /*  98 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec01_2       ,  TheSchemaTable.Rows[CI.t_dec01_2]);
      /*  99 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec02_2       ,  TheSchemaTable.Rows[CI.t_dec02_2]);
      /* 100 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec03_2       ,  TheSchemaTable.Rows[CI.t_dec03_2]);
      /* 101 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec04_2       ,  TheSchemaTable.Rows[CI.t_dec04_2]);
      /* 102 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec05_2       ,  TheSchemaTable.Rows[CI.t_dec05_2]);
      /* 103 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec06_2       ,  TheSchemaTable.Rows[CI.t_dec06_2]);
      /* 104 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec07_2       ,  TheSchemaTable.Rows[CI.t_dec07_2]);
      /* 105 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec08_2       ,  TheSchemaTable.Rows[CI.t_dec08_2]);
      /* 106 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec09_2       ,  TheSchemaTable.Rows[CI.t_dec09_2]);
      /* 107 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec10_2       ,  TheSchemaTable.Rows[CI.t_dec10_2]);
      /* 108 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec11_2       ,  TheSchemaTable.Rows[CI.t_dec11_2]);
      /* 109 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec12_2       ,  TheSchemaTable.Rows[CI.t_dec12_2]);
      /* 110 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec13_2       ,  TheSchemaTable.Rows[CI.t_dec13_2]);
      /* 111 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec14_2       ,  TheSchemaTable.Rows[CI.t_dec14_2]);
      /* 112 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec15_2       ,  TheSchemaTable.Rows[CI.t_dec15_2]);
      /* 113 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec16_2       ,  TheSchemaTable.Rows[CI.t_dec16_2]);
      /* 114 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec17_2       ,  TheSchemaTable.Rows[CI.t_dec17_2]);
      /* 115 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec18_2       ,  TheSchemaTable.Rows[CI.t_dec18_2]);
      /* 116 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec19_2       ,  TheSchemaTable.Rows[CI.t_dec19_2]);
      /* 117 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec20_2       ,  TheSchemaTable.Rows[CI.t_dec20_2]);
      /* 118 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec21_2       ,  TheSchemaTable.Rows[CI.t_dec21_2]);
      /* 119 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec22_2       ,  TheSchemaTable.Rows[CI.t_dec22_2]);
      /* 120 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec23_2       ,  TheSchemaTable.Rows[CI.t_dec23_2]);
      /* 121 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec24_2       ,  TheSchemaTable.Rows[CI.t_dec24_2]);
      /* 122 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec25_2       ,  TheSchemaTable.Rows[CI.t_dec25_2]);
      /* 123 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec26_2       ,  TheSchemaTable.Rows[CI.t_dec26_2]);
      /* 124 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec27_2       ,  TheSchemaTable.Rows[CI.t_dec27_2]);
      /* 125 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec28_2       ,  TheSchemaTable.Rows[CI.t_dec28_2]);
      /* 126 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec29_2       ,  TheSchemaTable.Rows[CI.t_dec29_2]);
      /* 127 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec30_2       ,  TheSchemaTable.Rows[CI.t_dec30_2]);
      /* 128 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_dec31_2       ,  TheSchemaTable.Rows[CI.t_dec31_2]);

      /* 129 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_date3         ,  TheSchemaTable.Rows[CI.t_date3  ]);
      /* 130 */ VvSQL.CreateCommandParameter(cmd, preffix, xtrans.T_date4         ,  TheSchemaTable.Rows[CI.t_date4  ]);
      }

   }

   #endregion SetCommandParamValues

   #region FillFromDataReader

   public override void FillFromDataReader(VvDataRecord vvDataRecord, XSqlDataReader reader, bool isArhiva /* dummy */)
   {
      XtransStruct rdrData = new XtransStruct();

      rdrData._recID = reader.GetUInt32(CI.recID);

      //rdrData._addTS = reader.GetDateTime(1);
      //rdrData._modTS = reader.GetDateTime(2);
      //rdrData._addUID = reader.GetString(3);
      //rdrData._modUID = reader.GetString(4);


      /* 01 */      rdrData._t_parentID   = reader.GetUInt32  (CI.t_parentID);
      /* 02 */      rdrData._t_dokNum     = reader.GetUInt32  (CI.t_dokNum);
      /* 03 */      rdrData._t_serial     = reader.GetUInt16  (CI.t_serial);
      /* 04 */      rdrData._t_dokDate    = reader.GetDateTime(CI.t_dokDate);
      /* 05 */      rdrData._t_tt         = reader.GetString  (CI.t_tt);
      /* 06 */      rdrData._t_ttNum      = reader.GetUInt32  (CI.t_ttNum);

      /* 07 */      rdrData._t_dateOd         = reader.GetDateTime(CI.t_dateOd        );
      /* 08 */      rdrData._t_dateDo         = reader.GetDateTime(CI.t_dateDo        );
      /* 09 */      rdrData._t_moneyA         = reader.GetDecimal (CI.t_moneyA        );
      /* 10 */      rdrData._t_kol            = reader.GetDecimal (CI.t_kol           );
      /* 11 */      rdrData._t_opis_128       = reader.GetString  (CI.t_opis_128      );
      /* 12 */      rdrData._t_kpdbNameA_50   = reader.GetString  (CI.t_kpdbNameA_50  );
      /* 13 */      rdrData._t_kpdbUlBrA_32   = reader.GetString  (CI.t_kpdbUlBrA_32  );
      /* 14 */      rdrData._t_kpdbMjestoA_32 = reader.GetString  (CI.t_kpdbMjestoA_32);
      /* 15 */      rdrData._t_kpdbZiroA_32   = reader.GetString  (CI.t_kpdbZiroA_32  );
      /* 16 */      rdrData._t_kpdbNameB_50   = reader.GetString  (CI.t_kpdbNameB_50  );
      /* 17 */      rdrData._t_kpdbUlBrB_32   = reader.GetString  (CI.t_kpdbUlBrB_32  );
      /* 18 */      rdrData._t_kpdbMjestoB_32 = reader.GetString  (CI.t_kpdbMjestoB_32);
      /* 19 */      rdrData._t_kpdbZiroB_32   = reader.GetString  (CI.t_kpdbZiroB_32  );
      /* 20 */      rdrData._t_vezniDokA_64   = reader.GetString  (CI.t_vezniDokA_64  );
      /* 21 */      rdrData._t_vezniDokB_64   = reader.GetString  (CI.t_vezniDokB_64  );
      /* 22 */      rdrData._t_strA_2         = reader.GetString  (CI.t_strA_2        );
      /* 23 */      rdrData._t_strB_2         = reader.GetString  (CI.t_strB_2        );
      /* 24 */      rdrData._t_strC_2         = reader.GetString  (CI.t_strC_2        );
      /* 25 */      rdrData._t_kupdobCD       = reader.GetUInt32  (CI.t_kupdobCD      );
      /* 26 */      rdrData._t_artiklCD       = reader.GetString  (CI.t_artiklCD      );
      /* 27 */      rdrData._t_artiklName     = reader.GetString  (CI.t_artiklName    );
      /* 28 */      rdrData._t_isXxx          = reader.GetBoolean (CI.t_isXxx         );
      /* 29 */      rdrData._t_intA           = reader.GetInt32   (CI.t_intA          );
      /* 30 */      rdrData._t_intB           = reader.GetInt32   (CI.t_intB          );
      /* 31 */      rdrData._t_konto          = reader.GetString  (CI.t_konto         );
      /* 32 */      rdrData._t_personCD       = reader.GetUInt32  (CI.t_personCD      );
      /* 33 */      rdrData._t_moneyB         = reader.GetDecimal (CI.t_moneyB        );
      /* 34 */      rdrData._t_moneyC         = reader.GetDecimal (CI.t_moneyC        );
      /* 35 */      rdrData._t_moneyD         = reader.GetDecimal (CI.t_moneyD        );

      /* 36 */   rdrData._t_dec01 = reader.GetDecimal(CI.t_dec01);
      /* 37 */   rdrData._t_dec02 = reader.GetDecimal(CI.t_dec02);
      /* 38 */   rdrData._t_dec03 = reader.GetDecimal(CI.t_dec03);
      /* 39 */   rdrData._t_dec04 = reader.GetDecimal(CI.t_dec04);
      /* 40 */   rdrData._t_dec05 = reader.GetDecimal(CI.t_dec05);
      /* 41 */   rdrData._t_dec06 = reader.GetDecimal(CI.t_dec06);
      /* 42 */   rdrData._t_dec07 = reader.GetDecimal(CI.t_dec07);
      /* 43 */   rdrData._t_dec08 = reader.GetDecimal(CI.t_dec08);
      /* 44 */   rdrData._t_dec09 = reader.GetDecimal(CI.t_dec09);
      /* 45 */   rdrData._t_dec10 = reader.GetDecimal(CI.t_dec10);
      /* 46 */   rdrData._t_dec11 = reader.GetDecimal(CI.t_dec11);
      /* 47 */   rdrData._t_dec12 = reader.GetDecimal(CI.t_dec12);
      /* 48 */   rdrData._t_dec13 = reader.GetDecimal(CI.t_dec13);
      /* 49 */   rdrData._t_dec14 = reader.GetDecimal(CI.t_dec14);
      /* 50 */   rdrData._t_dec15 = reader.GetDecimal(CI.t_dec15);
      /* 51 */   rdrData._t_dec16 = reader.GetDecimal(CI.t_dec16);
      /* 52 */   rdrData._t_dec17 = reader.GetDecimal(CI.t_dec17);
      /* 53 */   rdrData._t_dec18 = reader.GetDecimal(CI.t_dec18);
      /* 54 */   rdrData._t_dec19 = reader.GetDecimal(CI.t_dec19);
      /* 55 */   rdrData._t_dec20 = reader.GetDecimal(CI.t_dec20);
      /* 56 */   rdrData._t_dec21 = reader.GetDecimal(CI.t_dec21);
      /* 57 */   rdrData._t_dec22 = reader.GetDecimal(CI.t_dec22);
      /* 58 */   rdrData._t_dec23 = reader.GetDecimal(CI.t_dec23);
      /* 59 */   rdrData._t_dec24 = reader.GetDecimal(CI.t_dec24);
      /* 60 */   rdrData._t_dec25 = reader.GetDecimal(CI.t_dec25);
      /* 61 */   rdrData._t_dec26 = reader.GetDecimal(CI.t_dec26);
      /* 62 */   rdrData._t_dec27 = reader.GetDecimal(CI.t_dec27);
      /* 63 */   rdrData._t_dec28 = reader.GetDecimal(CI.t_dec28);
      /* 64 */   rdrData._t_dec29 = reader.GetDecimal(CI.t_dec29);
      /* 65 */   rdrData._t_dec30 = reader.GetDecimal(CI.t_dec30);
      /* 66 */   rdrData._t_dec31 = reader.GetDecimal(CI.t_dec31);
      /* 67 */   rdrData._t_str01 = reader.GetString (CI.t_str01);
      /* 68 */   rdrData._t_str02 = reader.GetString (CI.t_str02);
      /* 69 */   rdrData._t_str03 = reader.GetString (CI.t_str03);
      /* 70 */   rdrData._t_str04 = reader.GetString (CI.t_str04);
      /* 71 */   rdrData._t_str05 = reader.GetString (CI.t_str05);
      /* 72 */   rdrData._t_str06 = reader.GetString (CI.t_str06);
      /* 73 */   rdrData._t_str07 = reader.GetString (CI.t_str07);
      /* 74 */   rdrData._t_str08 = reader.GetString (CI.t_str08);
      /* 75 */   rdrData._t_str09 = reader.GetString (CI.t_str09);
      /* 76 */   rdrData._t_str10 = reader.GetString (CI.t_str10);
      /* 77 */   rdrData._t_str11 = reader.GetString (CI.t_str11);
      /* 78 */   rdrData._t_str12 = reader.GetString (CI.t_str12);
      /* 79 */   rdrData._t_str13 = reader.GetString (CI.t_str13);
      /* 70 */   rdrData._t_str14 = reader.GetString (CI.t_str14);
      /* 81 */   rdrData._t_str15 = reader.GetString (CI.t_str15);
      /* 82 */   rdrData._t_str16 = reader.GetString (CI.t_str16);
      /* 83 */   rdrData._t_str17 = reader.GetString (CI.t_str17);
      /* 84 */   rdrData._t_str18 = reader.GetString (CI.t_str18);
      /* 85 */   rdrData._t_str19 = reader.GetString (CI.t_str19);
      /* 86 */   rdrData._t_str20 = reader.GetString (CI.t_str20);
      /* 87 */   rdrData._t_str21 = reader.GetString (CI.t_str21);
      /* 88 */   rdrData._t_str22 = reader.GetString (CI.t_str22);
      /* 89 */   rdrData._t_str23 = reader.GetString (CI.t_str23);
      /* 90 */   rdrData._t_str24 = reader.GetString (CI.t_str24);
      /* 91 */   rdrData._t_str25 = reader.GetString (CI.t_str25);
      /* 92 */   rdrData._t_str26 = reader.GetString (CI.t_str26);
      /* 93 */   rdrData._t_str27 = reader.GetString (CI.t_str27);
      /* 94 */   rdrData._t_str28 = reader.GetString (CI.t_str28);
      /* 95 */   rdrData._t_str29 = reader.GetString (CI.t_str29);
      /* 96 */   rdrData._t_str30 = reader.GetString (CI.t_str30);
      /* 97 */   rdrData._t_str31 = reader.GetString (CI.t_str31);

      /*  98 */  rdrData._t_dec01_2 = reader.GetDecimal(CI.t_dec01_2);
      /*  99 */  rdrData._t_dec02_2 = reader.GetDecimal(CI.t_dec02_2);
      /* 100 */  rdrData._t_dec03_2 = reader.GetDecimal(CI.t_dec03_2);
      /* 101 */  rdrData._t_dec04_2 = reader.GetDecimal(CI.t_dec04_2);
      /* 102 */  rdrData._t_dec05_2 = reader.GetDecimal(CI.t_dec05_2);
      /* 103 */  rdrData._t_dec06_2 = reader.GetDecimal(CI.t_dec06_2);
      /* 104 */  rdrData._t_dec07_2 = reader.GetDecimal(CI.t_dec07_2);
      /* 105 */  rdrData._t_dec08_2 = reader.GetDecimal(CI.t_dec08_2);
      /* 106 */  rdrData._t_dec09_2 = reader.GetDecimal(CI.t_dec09_2);
      /* 107 */  rdrData._t_dec10_2 = reader.GetDecimal(CI.t_dec10_2);
      /* 108 */  rdrData._t_dec11_2 = reader.GetDecimal(CI.t_dec11_2);
      /* 109 */  rdrData._t_dec12_2 = reader.GetDecimal(CI.t_dec12_2);
      /* 110 */  rdrData._t_dec13_2 = reader.GetDecimal(CI.t_dec13_2);
      /* 111 */  rdrData._t_dec14_2 = reader.GetDecimal(CI.t_dec14_2);
      /* 112 */  rdrData._t_dec15_2 = reader.GetDecimal(CI.t_dec15_2);
      /* 113 */  rdrData._t_dec16_2 = reader.GetDecimal(CI.t_dec16_2);
      /* 114 */  rdrData._t_dec17_2 = reader.GetDecimal(CI.t_dec17_2);
      /* 115 */  rdrData._t_dec18_2 = reader.GetDecimal(CI.t_dec18_2);
      /* 116 */  rdrData._t_dec19_2 = reader.GetDecimal(CI.t_dec19_2);
      /* 117 */  rdrData._t_dec20_2 = reader.GetDecimal(CI.t_dec20_2);
      /* 118 */  rdrData._t_dec21_2 = reader.GetDecimal(CI.t_dec21_2);
      /* 119 */  rdrData._t_dec22_2 = reader.GetDecimal(CI.t_dec22_2);
      /* 120 */  rdrData._t_dec23_2 = reader.GetDecimal(CI.t_dec23_2);
      /* 121 */  rdrData._t_dec24_2 = reader.GetDecimal(CI.t_dec24_2);
      /* 122 */  rdrData._t_dec25_2 = reader.GetDecimal(CI.t_dec25_2);
      /* 123 */  rdrData._t_dec26_2 = reader.GetDecimal(CI.t_dec26_2);
      /* 124 */  rdrData._t_dec27_2 = reader.GetDecimal(CI.t_dec27_2);
      /* 125 */  rdrData._t_dec28_2 = reader.GetDecimal(CI.t_dec28_2);
      /* 126 */  rdrData._t_dec29_2 = reader.GetDecimal(CI.t_dec29_2);
      /* 127 */  rdrData._t_dec30_2 = reader.GetDecimal(CI.t_dec30_2);
      /* 128 */  rdrData._t_dec31_2 = reader.GetDecimal(CI.t_dec31_2);

      /* 129 */  rdrData._t_date3   = reader.GetDateTime(CI.t_date3 );
      /* 130 */  rdrData._t_date4   = reader.GetDateTime(CI.t_date4 );

      ((Xtrans)vvDataRecord).CurrentData = rdrData;

      return;
   }

   #endregion FillFromDataReader

   #region FtrCI struct & InitializeSchemaColumnIndexes()

   public struct XtransCI
   {
   internal int recID;

   /* 01 */   internal int  t_parentID;
   /* 02 */   internal int  t_dokNum;
   /* 03 */   internal int  t_serial;
   /* 04 */   internal int  t_dokDate;
   /* 05 */   internal int  t_tt;
   /* 06 */   internal int  t_ttNum;

   /* 07 */   internal int  t_dateOd        ;
   /* 08 */   internal int  t_dateDo        ;
   /* 09 */   internal int  t_moneyA        ;
   /* 10 */   internal int  t_kol           ;
   /* 11 */   internal int  t_opis_128      ;
   /* 12 */   internal int  t_kpdbNameA_50  ;
   /* 13 */   internal int  t_kpdbUlBrA_32  ;
   /* 14 */   internal int  t_kpdbMjestoA_32;
   /* 15 */   internal int  t_kpdbZiroA_32  ;
   /* 16 */   internal int  t_kpdbNameB_50  ;
   /* 17 */   internal int  t_kpdbUlBrB_32  ;
   /* 18 */   internal int  t_kpdbMjestoB_32;
   /* 19 */   internal int  t_kpdbZiroB_32  ;
   /* 20 */   internal int  t_vezniDokA_64  ;
   /* 21 */   internal int  t_vezniDokB_64  ;
   /* 22 */   internal int  t_strA_2        ;
   /* 23 */   internal int  t_strB_2        ;
   /* 24 */   internal int  t_strC_2        ;
   /* 25 */   internal int  t_kupdobCD      ;
   /* 26 */   internal int  t_artiklCD      ;
   /* 27 */   internal int  t_artiklName    ;
   /* 28 */   internal int  t_isXxx         ;
   /* 29 */   internal int  t_intA          ;
   /* 30 */   internal int  t_intB          ;
   /* 31 */   internal int  t_konto         ;
   /* 32 */   internal int  t_personCD      ;
   /* 33 */   internal int  t_moneyB        ;
   /* 34 */   internal int  t_moneyC        ;
   /* 35 */   internal int  t_moneyD        ;

   /* 36 */   internal int   t_dec01         ;
   /* 37 */   internal int   t_dec02         ;
   /* 38 */   internal int   t_dec03         ;
   /* 39 */   internal int   t_dec04         ;
   /* 40 */   internal int   t_dec05         ;
   /* 41 */   internal int   t_dec06         ;
   /* 42 */   internal int   t_dec07         ;
   /* 43 */   internal int   t_dec08         ;
   /* 44 */   internal int   t_dec09         ;
   /* 45 */   internal int   t_dec10         ;
   /* 46 */   internal int   t_dec11         ;
   /* 47 */   internal int   t_dec12         ;
   /* 48 */   internal int   t_dec13         ;
   /* 49 */   internal int   t_dec14         ;
   /* 50 */   internal int   t_dec15         ;
   /* 51 */   internal int   t_dec16         ;
   /* 52 */   internal int   t_dec17         ;
   /* 53 */   internal int   t_dec18         ;
   /* 54 */   internal int   t_dec19         ;
   /* 55 */   internal int   t_dec20         ;
   /* 56 */   internal int   t_dec21         ;
   /* 57 */   internal int   t_dec22         ;
   /* 58 */   internal int   t_dec23         ;
   /* 59 */   internal int   t_dec24         ;
   /* 60 */   internal int   t_dec25         ;
   /* 61 */   internal int   t_dec26         ;
   /* 62 */   internal int   t_dec27         ;
   /* 63 */   internal int   t_dec28         ;
   /* 64 */   internal int   t_dec29         ;
   /* 65 */   internal int   t_dec30         ;
   /* 66 */   internal int   t_dec31         ;
   /* 67 */   internal int   t_str01         ;
   /* 68 */   internal int   t_str02         ;
   /* 69 */   internal int   t_str03         ;
   /* 70 */   internal int   t_str04         ;
   /* 71 */   internal int   t_str05         ;
   /* 72 */   internal int   t_str06         ;
   /* 73 */   internal int   t_str07         ;
   /* 74 */   internal int   t_str08         ;
   /* 75 */   internal int   t_str09         ;
   /* 76 */   internal int   t_str10         ;
   /* 77 */   internal int   t_str11         ;
   /* 78 */   internal int   t_str12         ;
   /* 79 */   internal int   t_str13         ;
   /* 70 */   internal int   t_str14         ;
   /* 81 */   internal int   t_str15         ;
   /* 82 */   internal int   t_str16         ;
   /* 83 */   internal int   t_str17         ;
   /* 84 */   internal int   t_str18         ;
   /* 85 */   internal int   t_str19         ;
   /* 86 */   internal int   t_str20         ;
   /* 87 */   internal int   t_str21         ;
   /* 88 */   internal int   t_str22         ;
   /* 89 */   internal int   t_str23         ;
   /* 90 */   internal int   t_str24         ;
   /* 91 */   internal int   t_str25         ;
   /* 92 */   internal int   t_str26         ;
   /* 93 */   internal int   t_str27         ;
   /* 94 */   internal int   t_str28         ;
   /* 95 */   internal int   t_str29         ;
   /* 96 */   internal int   t_str30         ;
   /* 97 */   internal int   t_str31         ;

   /*  98 */  internal int   t_dec01_2       ;
   /*  99 */  internal int   t_dec02_2       ;
   /* 100 */  internal int   t_dec03_2       ;
   /* 101 */  internal int   t_dec04_2       ;
   /* 102 */  internal int   t_dec05_2       ;
   /* 103 */  internal int   t_dec06_2       ;
   /* 104 */  internal int   t_dec07_2       ;
   /* 105 */  internal int   t_dec08_2       ;
   /* 106 */  internal int   t_dec09_2       ;
   /* 107 */  internal int   t_dec10_2       ;
   /* 108 */  internal int   t_dec11_2       ;
   /* 109 */  internal int   t_dec12_2       ;
   /* 110 */  internal int   t_dec13_2       ;
   /* 111 */  internal int   t_dec14_2       ;
   /* 112 */  internal int   t_dec15_2       ;
   /* 113 */  internal int   t_dec16_2       ;
   /* 114 */  internal int   t_dec17_2       ;
   /* 115 */  internal int   t_dec18_2       ;
   /* 116 */  internal int   t_dec19_2       ;
   /* 117 */  internal int   t_dec20_2       ;
   /* 118 */  internal int   t_dec21_2       ;
   /* 119 */  internal int   t_dec22_2       ;
   /* 120 */  internal int   t_dec23_2       ;
   /* 121 */  internal int   t_dec24_2       ;
   /* 122 */  internal int   t_dec25_2       ;
   /* 123 */  internal int   t_dec26_2       ;
   /* 124 */  internal int   t_dec27_2       ;
   /* 125 */  internal int   t_dec28_2       ;
   /* 126 */  internal int   t_dec29_2       ;
   /* 127 */  internal int   t_dec30_2       ;
   /* 128 */  internal int   t_dec31_2       ;

   /* 129 */  internal int   t_date3         ;
   /* 130 */  internal int   t_date4         ;

   }

   /// <summary>
   /// FtrCI ti je Column Indexes in SchemaTable
   /// </summary>
   public XtransCI CI;

   protected override void InitializeSchemaColumnIndexes()
   {
      CI.recID       = GetSchemaColumnIndex("recID");
      //FtrCI.addTS       = GetSchemaColumnIndex("addTS");
      //FtrCI.modTS       = GetSchemaColumnIndex("modTS");
      //FtrCI.addUID      = GetSchemaColumnIndex("addUID");
      //FtrCI.modUID      = GetSchemaColumnIndex("modUID");

      CI.t_parentID       = GetSchemaColumnIndex("t_parentID");
      CI.t_dokNum         = GetSchemaColumnIndex("t_dokNum");
      CI.t_serial         = GetSchemaColumnIndex("t_serial");
      CI.t_dokDate        = GetSchemaColumnIndex("t_dokDate");
      CI.t_tt             = GetSchemaColumnIndex("t_tt");
      CI.t_ttNum          = GetSchemaColumnIndex("t_ttNum");
      CI.t_dateOd         = GetSchemaColumnIndex("t_dateOd");
      CI.t_dateDo         = GetSchemaColumnIndex("t_dateDo");
      CI.t_moneyA         = GetSchemaColumnIndex("t_moneyA");
      CI.t_kol            = GetSchemaColumnIndex("t_kol");
      CI.t_opis_128       = GetSchemaColumnIndex("t_opis_128");
      CI.t_kpdbNameA_50   = GetSchemaColumnIndex("t_kpdbNameA_50");
      CI.t_kpdbUlBrA_32   = GetSchemaColumnIndex("t_kpdbUlBrA_32");
      CI.t_kpdbMjestoA_32 = GetSchemaColumnIndex("t_kpdbMjestoA_32");
      CI.t_kpdbZiroA_32   = GetSchemaColumnIndex("t_kpdbZiroA_32");
      CI.t_kpdbNameB_50   = GetSchemaColumnIndex("t_kpdbNameB_50");
      CI.t_kpdbUlBrB_32   = GetSchemaColumnIndex("t_kpdbUlBrB_32");
      CI.t_kpdbMjestoB_32 = GetSchemaColumnIndex("t_kpdbMjestoB_32");
      CI.t_kpdbZiroB_32   = GetSchemaColumnIndex("t_kpdbZiroB_32");
      CI.t_vezniDokA_64   = GetSchemaColumnIndex("t_vezniDokA_64");
      CI.t_vezniDokB_64   = GetSchemaColumnIndex("t_vezniDokB_64");
      CI.t_strA_2         = GetSchemaColumnIndex("t_strA_2");
      CI.t_strB_2         = GetSchemaColumnIndex("t_strB_2");
      CI.t_strC_2         = GetSchemaColumnIndex("t_strC_2");
      CI.t_kupdobCD       = GetSchemaColumnIndex("t_kupdobCD");
      CI.t_artiklCD       = GetSchemaColumnIndex("t_artiklCD");
      CI.t_artiklName     = GetSchemaColumnIndex("t_artiklName");
      CI.t_isXxx          = GetSchemaColumnIndex("t_isXxx");
      CI.t_intA           = GetSchemaColumnIndex("t_intA");
      CI.t_intB           = GetSchemaColumnIndex("t_intB");
      CI.t_konto          = GetSchemaColumnIndex("t_konto");
      CI.t_personCD       = GetSchemaColumnIndex("t_personCD");
      CI.t_moneyB         = GetSchemaColumnIndex("t_moneyB");
      CI.t_moneyC         = GetSchemaColumnIndex("t_moneyC");
      CI.t_moneyD         = GetSchemaColumnIndex("t_moneyD");

      CI.t_dec01       = GetSchemaColumnIndex("t_dec01");
      CI.t_dec02       = GetSchemaColumnIndex("t_dec02");
      CI.t_dec03       = GetSchemaColumnIndex("t_dec03");
      CI.t_dec04       = GetSchemaColumnIndex("t_dec04");
      CI.t_dec05       = GetSchemaColumnIndex("t_dec05");
      CI.t_dec06       = GetSchemaColumnIndex("t_dec06");
      CI.t_dec07       = GetSchemaColumnIndex("t_dec07");
      CI.t_dec08       = GetSchemaColumnIndex("t_dec08");
      CI.t_dec09       = GetSchemaColumnIndex("t_dec09");
      CI.t_dec10       = GetSchemaColumnIndex("t_dec10");
      CI.t_dec11       = GetSchemaColumnIndex("t_dec11");
      CI.t_dec12       = GetSchemaColumnIndex("t_dec12");
      CI.t_dec13       = GetSchemaColumnIndex("t_dec13");
      CI.t_dec14       = GetSchemaColumnIndex("t_dec14");
      CI.t_dec15       = GetSchemaColumnIndex("t_dec15");
      CI.t_dec16       = GetSchemaColumnIndex("t_dec16");
      CI.t_dec17       = GetSchemaColumnIndex("t_dec17");
      CI.t_dec18       = GetSchemaColumnIndex("t_dec18");
      CI.t_dec19       = GetSchemaColumnIndex("t_dec19");
      CI.t_dec20       = GetSchemaColumnIndex("t_dec20");
      CI.t_dec21       = GetSchemaColumnIndex("t_dec21");
      CI.t_dec22       = GetSchemaColumnIndex("t_dec22");
      CI.t_dec23       = GetSchemaColumnIndex("t_dec23");
      CI.t_dec24       = GetSchemaColumnIndex("t_dec24");
      CI.t_dec25       = GetSchemaColumnIndex("t_dec25");
      CI.t_dec26       = GetSchemaColumnIndex("t_dec26");
      CI.t_dec27       = GetSchemaColumnIndex("t_dec27");
      CI.t_dec28       = GetSchemaColumnIndex("t_dec28");
      CI.t_dec29       = GetSchemaColumnIndex("t_dec29");
      CI.t_dec30       = GetSchemaColumnIndex("t_dec30");
      CI.t_dec31       = GetSchemaColumnIndex("t_dec31");
      CI.t_str01       = GetSchemaColumnIndex("t_str01");
      CI.t_str02       = GetSchemaColumnIndex("t_str02");
      CI.t_str03       = GetSchemaColumnIndex("t_str03");
      CI.t_str04       = GetSchemaColumnIndex("t_str04");
      CI.t_str05       = GetSchemaColumnIndex("t_str05");
      CI.t_str06       = GetSchemaColumnIndex("t_str06");
      CI.t_str07       = GetSchemaColumnIndex("t_str07");
      CI.t_str08       = GetSchemaColumnIndex("t_str08");
      CI.t_str09       = GetSchemaColumnIndex("t_str09");
      CI.t_str10       = GetSchemaColumnIndex("t_str10");
      CI.t_str11       = GetSchemaColumnIndex("t_str11");
      CI.t_str12       = GetSchemaColumnIndex("t_str12");
      CI.t_str13       = GetSchemaColumnIndex("t_str13");
      CI.t_str14       = GetSchemaColumnIndex("t_str14");
      CI.t_str15       = GetSchemaColumnIndex("t_str15");
      CI.t_str16       = GetSchemaColumnIndex("t_str16");
      CI.t_str17       = GetSchemaColumnIndex("t_str17");
      CI.t_str18       = GetSchemaColumnIndex("t_str18");
      CI.t_str19       = GetSchemaColumnIndex("t_str19");
      CI.t_str20       = GetSchemaColumnIndex("t_str20");
      CI.t_str21       = GetSchemaColumnIndex("t_str21");
      CI.t_str22       = GetSchemaColumnIndex("t_str22");
      CI.t_str23       = GetSchemaColumnIndex("t_str23");
      CI.t_str24       = GetSchemaColumnIndex("t_str24");
      CI.t_str25       = GetSchemaColumnIndex("t_str25");
      CI.t_str26       = GetSchemaColumnIndex("t_str26");
      CI.t_str27       = GetSchemaColumnIndex("t_str27");
      CI.t_str28       = GetSchemaColumnIndex("t_str28");
      CI.t_str29       = GetSchemaColumnIndex("t_str29");
      CI.t_str30       = GetSchemaColumnIndex("t_str30");
      CI.t_str31       = GetSchemaColumnIndex("t_str31");

      CI.t_dec01_2     = GetSchemaColumnIndex("t_dec01_2");
      CI.t_dec02_2     = GetSchemaColumnIndex("t_dec02_2");
      CI.t_dec03_2     = GetSchemaColumnIndex("t_dec03_2");
      CI.t_dec04_2     = GetSchemaColumnIndex("t_dec04_2");
      CI.t_dec05_2     = GetSchemaColumnIndex("t_dec05_2");
      CI.t_dec06_2     = GetSchemaColumnIndex("t_dec06_2");
      CI.t_dec07_2     = GetSchemaColumnIndex("t_dec07_2");
      CI.t_dec08_2     = GetSchemaColumnIndex("t_dec08_2");
      CI.t_dec09_2     = GetSchemaColumnIndex("t_dec09_2");
      CI.t_dec10_2     = GetSchemaColumnIndex("t_dec10_2");
      CI.t_dec11_2     = GetSchemaColumnIndex("t_dec11_2");
      CI.t_dec12_2     = GetSchemaColumnIndex("t_dec12_2");
      CI.t_dec13_2     = GetSchemaColumnIndex("t_dec13_2");
      CI.t_dec14_2     = GetSchemaColumnIndex("t_dec14_2");
      CI.t_dec15_2     = GetSchemaColumnIndex("t_dec15_2");
      CI.t_dec16_2     = GetSchemaColumnIndex("t_dec16_2");
      CI.t_dec17_2     = GetSchemaColumnIndex("t_dec17_2");
      CI.t_dec18_2     = GetSchemaColumnIndex("t_dec18_2");
      CI.t_dec19_2     = GetSchemaColumnIndex("t_dec19_2");
      CI.t_dec20_2     = GetSchemaColumnIndex("t_dec20_2");
      CI.t_dec21_2     = GetSchemaColumnIndex("t_dec21_2");
      CI.t_dec22_2     = GetSchemaColumnIndex("t_dec22_2");
      CI.t_dec23_2     = GetSchemaColumnIndex("t_dec23_2");
      CI.t_dec24_2     = GetSchemaColumnIndex("t_dec24_2");
      CI.t_dec25_2     = GetSchemaColumnIndex("t_dec25_2");
      CI.t_dec26_2     = GetSchemaColumnIndex("t_dec26_2");
      CI.t_dec27_2     = GetSchemaColumnIndex("t_dec27_2");
      CI.t_dec28_2     = GetSchemaColumnIndex("t_dec28_2");
      CI.t_dec29_2     = GetSchemaColumnIndex("t_dec29_2");
      CI.t_dec30_2     = GetSchemaColumnIndex("t_dec30_2");
      CI.t_dec31_2     = GetSchemaColumnIndex("t_dec31_2");

      CI.t_date3       = GetSchemaColumnIndex("t_date3"  );
      CI.t_date4       = GetSchemaColumnIndex("t_date4"  );
   }

   #endregion FtrCI struct & InitializeSchemaColumnIndexes()

   #region Vozilo - GetZadnjeStanjeBrojila

   public static decimal? GetZadnjeStanjeBrojila(XSqlConnection conn, string voziloCD, bool shouldBeSilent)
   {
      object obj;
      decimal? zadnjeStanje;

      using(XSqlCommand cmd = VvSQL.GetZadnjeStanjeBrojila_Command(conn, voziloCD))
      {
         try              { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return(null); }

         try              { zadnjeStanje = decimal.Parse(obj.ToString()); }
         catch(Exception) { zadnjeStanje = null; }
      }

      if(zadnjeStanje == null && shouldBeSilent == false) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Za vozilo [{0}] ne mogu naći prethodno stanje brojila!", voziloCD);
      
      return (zadnjeStanje);
   }

   #endregion Vozilo - GetZadnjeStanjeBrojila

   #region GetExterniCjenik XTRANS

   public static /*bool*/ decimal GetExterniCjenik(XSqlConnection conn, Xtrans xtrans_rec, string _artiklCD, DateTime dokDate, bool _shouldBeSilent)
   {
      bool success = true;

      using(XSqlCommand cmd = VvSQL.GetLastXtransByDateAndArtikl_Command(conn, _artiklCD, dokDate, Mixer.TT_EXT_CIJ))
      {
         success = ZXC.XtransDao.ExecuteSingleFillFromDataReader(xtrans_rec, false, cmd);
      } // using cmd 

      if(!success && !_shouldBeSilent)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + _artiklCD + "] ne postoji u datoteci eksternih cjenika[" + Xtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
      }

    //return success;
      return xtrans_rec.T_moneyA;
   }

   internal static decimal GetExternCijenaInKune(XSqlConnection conn, string artiklCD, DateTime dokDate)
   {
      decimal theMSRP = 0.00M;

      Xtrans xtrans_rec = new Xtrans();

      theMSRP = XtransDao.GetExterniCjenik(conn, xtrans_rec, artiklCD, dokDate, false);

      decimal devTecaj = ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrans_rec.T_konto), dokDate);

      return theMSRP * devTecaj;
   }
   
   #endregion GetExterniCjenik XTRANS

   #region Get_PPMV_Xtrans

   internal static Xtrans Get_PPMV_Xtrans(XSqlConnection conn, string artiklCD, DateTime dokDate)
   {
      Xtrans xtrans_rec = new Xtrans();

      bool success = true;

      using(XSqlCommand cmd = VvSQL.GetLastXtransByDateAndArtikl_Command(conn, artiklCD, dokDate, Mixer.TT_PMV))
      {
         success = ZXC.XtransDao.ExecuteSingleFillFromDataReader(xtrans_rec, false, cmd);
      } // using cmd 

      if(!success /*&& !_shouldBeSilent*/)
      {
         VvSQL.ReportGeneric_DB_Error("", "Podatak: [" + artiklCD + "] ne postoji u datoteci cjenika Motornih Vozila [" + Xtrans.recordName + ".", System.Windows.Forms.MessageBoxButtons.OK);
         xtrans_rec = null;
      }

      return xtrans_rec;
   }

   #endregion Get_PPMV_Xtrans


   internal static bool Addrec_ESC_DRW_Log(XSqlConnection conn, string actionName, Faktur faktur_rec, int rowIdx)
   {
      Xtrans xtrans_rec = new Xtrans();

      xtrans_rec.T_dateDo       =      DateTime.Now           ;
      xtrans_rec.T_TT           =      "THL"                  ;
      xtrans_rec.T_strA_2       =      actionName             ;
      xtrans_rec.T_intA         =      rowIdx                 ;
      xtrans_rec.T_opis_128     =      ZXC.vvDB_User          ;
      xtrans_rec.T_personCD     =      ZXC.vvDB_ServerID      ;
      xtrans_rec.T_intB         = (int)faktur_rec.TtNum       ;
      xtrans_rec.T_dateOd       =      faktur_rec.DokDate     ;
      xtrans_rec.T_konto        =      faktur_rec.SkladCD     ;
      xtrans_rec.T_kpdbZiroA_32 =      faktur_rec.NacPlac     ;
      xtrans_rec.T_isXxx        =      faktur_rec.IsNpCash    ;
      xtrans_rec.T_kupdobCD     =      faktur_rec.S_ukTrnCount;
      xtrans_rec.T_moneyA       =      faktur_rec.S_ukKCRP    ;

      bool OK = ZXC.XtransDao.ADDREC(conn, xtrans_rec, false, false, false, false);

      return OK;
   }

   #region NAK - Get Naziv Artikla Za Kupca

   public static string GetNazivArtiklaZaKupca(XSqlConnection conn, uint kupdobCD, string artiklCD, string origArtiklName)
   {
      object obj;
      string nazivZaKupca = origArtiklName;

      using(XSqlCommand cmd = VvSQL.GetNazivArtiklaZaKupca_Command(conn, kupdobCD, artiklCD))
      {
         try              { obj = cmd.ExecuteScalar(); }
         catch(Exception) { return(origArtiklName); }

         try              { nazivZaKupca = obj.ToString(); }
         catch(Exception) { nazivZaKupca = origArtiklName; }
      }

    //if(nazivZaKupca == null && shouldBeSilent == false) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "Za vozilo [{0}] ne mogu naći prethodno stanje brojila!", voziloCD);
      
      return (nazivZaKupca.NotEmpty() ? nazivZaKupca : origArtiklName);
   }

   #endregion Vozilo - GetZadnjeStanjeBrojila


}
