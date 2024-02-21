using System;
using System.Collections.Generic;

#region struct KplanStruct

public struct KplanStruct

{
   internal uint     _recID;
   internal DateTime _addTS;
   internal DateTime _modTS;
   internal string   _addUID;
   internal string   _modUID;

   internal uint     _lanSrvID;
   internal uint     _lanRecID;

   internal string _konto;
   internal string _tip;
   internal string _naziv;
   internal ushort _psRule;
   internal string _anaGr;
   internal string _opis;
   internal string _naziv2;
   internal string _naziv3;
   internal string _fond  ;
}

#endregion struct KplanStruct

public class Kplan : VvSifrarRecord
{

   #region Fildz

   public const string recordName = "kplan";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private KplanStruct currentData;
   private KplanStruct backupData;

   public enum PsRuleEnum
   {
      NO_OVERRIDE,
      SUPRESS_SaldaKontiKTO,
      FORCE_SaldaKontiKTO,
   }

   protected static System.Data.DataTable TheSchemaTable = ZXC.KplanDao.TheSchemaTable;
   protected static KplanDao.KplanCI CI = ZXC.KplanDao.CI;

   #endregion Fildz

   #region Constructors

   public Kplan()
      : this(0)
   {
   }

   public Kplan(uint ID)
      : base()
   {
      this.currentData = new KplanStruct();

      Memset0(ID);
   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;

      this.currentData._addTS = DateTime.MinValue;
      this.currentData._modTS = DateTime.MinValue;
      this.currentData._addUID = "";
      this.currentData._modUID = "";
      this.currentData._lanSrvID = 0;
      this.currentData._lanRecID = 0;

      this.currentData._konto = "";
      this.currentData._naziv = "";
      this.currentData._tip = "";
      this.currentData._psRule = 0;
      this.currentData._anaGr = "";
      this.currentData._opis = "";
      this.currentData._naziv2 = "";
      this.currentData._naziv3 = "";
      this.currentData._fond = "";
   }

   #endregion Constructors

   #region ToString

   public override string ToString()
   {
      return Konto + " (" + Naziv + ")";
   }

   public static string ToSifrarString(VvDataRecord vvDataRecord, VvSQL.SorterType sifrarType, ZXC.AutoCompleteRestrictor restrictor)
   {
      Kplan kplan_rec = (Kplan)vvDataRecord;

      if(restrictor == ZXC.AutoCompleteRestrictor.KPL_Analitika_Only && kplan_rec.Tip != "A") return "";

      switch(sifrarType)
      {
         case VvSQL.SorterType.Name      : return kplan_rec.Naziv;
         case VvSQL.SorterType.Konto     : return kplan_rec.Konto;
         case VvSQL.SorterType.KontoNaziv: return kplan_rec.Konto + " - " + kplan_rec.Naziv;

         default: throw new Exception(sifrarType.ToString() + " NOT DEFINED in Kplan.ToSifrarString(VvSQL.DokumentSorterType sifrarType)");
      }
   }

   #endregion ToString

   #region propertiz

   internal KplanStruct CurrentData // cijela KplanStruct struct-ura 
   {
      get { return this.currentData; }
      set { this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.KplanDao; }
   }

   public override string VirtualRecordName
   {
      get { return Kplan.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Kplan.recordNameArhiva; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "fk"; }
   }

   public override uint VirtualRecID
   {
      get { return this.RecID; }
      set { this.RecID = value; }
   }

   public override bool IsAutoSifra
   {
      get { return false; }
   }

   // Dummy for Kplan: 
   public override string SifraColName
   {
      get { return ""; }
   }

   public override string SifraColValue
   {
      get { return this.Konto; }
   }

   public override DateTime VirtualAddTS { get { return this.AddTS; } }
   public override DateTime VirtualModTS { get { return this.ModTS; } }
   public override string VirtualAddUID { get { return this.AddUID; } }
   public override string VirtualModUID { get { return this.ModUID; } }

   public override uint VirtualLanSrvID { get { return this.LanSrvID; } set { this.LanSrvID = value; } }
   public override uint VirtualLanRecID { get { return this.LanRecID; } set { this.LanRecID = value; } }

   public override VvSQL.RecordSorter DefaultSorter
   {
      get { return Kplan.sorterKonto; }
   }

   /// <summary>
   /// A je podatak 'konto' kao link (foreign key) za druge tablice,
   /// izmijenjen u operaciji 'Edit'? 
   /// </summary>
   public override bool IsSomeOfPossibleForeignKeyFieldsChanged
   {
      get
      {
         return this.currentData._konto != this.backupData._konto;
      }
   }

   private List<Ftrans> _ftranses;
   /// <summary>
   /// Gets or sets a list of Ftrans (ala customers orders) for this Kplan.
   /// </summary>
   public List<Ftrans> Ftranses
   {
      get { return _ftranses; }
      set { _ftranses = value; }
   }

   public override void InvokeTransClear()
   {
      if(this.Ftranses != null) this.Ftranses.Clear();
   }

   //===================================================================
   //===================================================================
   //===================================================================


   public uint RecID
   {
      get { return this.currentData._recID; }
      set { this.currentData._recID = value; }
   }

   public DateTime AddTS
   {
      get { return this.currentData._addTS; }
      set { this.currentData._addTS = value; }
   }

   public DateTime ModTS
   {
      get { return this.currentData._modTS; }
      set { this.currentData._modTS = value; }
   }

   public string AddUID
   {
      get { return this.currentData._addUID; }
      set { this.currentData._addUID = value; }
   }

   public string ModUID
   {
      get { return this.currentData._modUID; }
      set { this.currentData._modUID = value; }
   }

   public uint LanSrvID { get { return this.currentData._lanSrvID; } set { this.currentData._lanSrvID = value; } }
   public uint LanRecID { get { return this.currentData._lanRecID; } set { this.currentData._lanRecID = value; } }

   public string Konto
   {
      get { return this.currentData._konto; }
      set { this.currentData._konto = value; }
   }

   public string BackupedKonto
   {
      get { return this.backupData._konto; }
   }

   public string Tip
   {
      get { return this.currentData._tip; }
      set { this.currentData._tip = value; }
   }

   public string Naziv
   {
      get { return this.currentData._naziv; }
      set { this.currentData._naziv = value; }
   }

   public string Naziv2
   {
      get { return this.currentData._naziv2; }
      set { this.currentData._naziv2 = value; }
   }

   public string Naziv3
   {
      get { return this.currentData._naziv3; }
      set { this.currentData._naziv3 = value; }
   }

   public string Fond
   {
      get { return this.currentData._fond; }
      set { this.currentData._fond = value; }
   }

   public Kplan.PsRuleEnum PsRule
   {
      get { return (Kplan.PsRuleEnum)this.currentData._psRule; }
      set { this.currentData._psRule = (ushort)value; }
   }

   public string AnaGr
   {
      get { return this.currentData._anaGr; }
      set { this.currentData._anaGr = value; }
   }

   public string Opis
   {
      get { return this.currentData._opis; }
      set { this.currentData._opis = value; }
   }

   #endregion propertiz

   #region Implements IEditableObject

   #region Utils

   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<KplanStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<KplanStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<KplanStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<KplanStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<KplanStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Kplan newObject = new Kplan();

      Generic_CloneData<KplanStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      return newObject;
   }

   public Kplan MakeDeepCopy()
   {
      return (Kplan)Clone();
   }

   #endregion

   #region SorterCurrVal

   public static VvSQL.RecordSorter sorterKonto = new VvSQL.RecordSorter(Kplan.recordName, Kplan.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.konto]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Konto", VvSQL.SorterType.Konto/*Code*/, false);

   public static VvSQL.RecordSorter sorterNaziv = new VvSQL.RecordSorter(Kplan.recordName, Kplan.recordNameArhiva, new VvSQL.IndexSegment[]
      {
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.naziv]),
         new VvSQL.IndexSegment(TheSchemaTable.Rows[CI.recVer], true)
      }, "Naziv", VvSQL.SorterType.Name, false);

   private VvSQL.RecordSorter[] _sorters =
      new VvSQL.RecordSorter[]
      { 
         sorterKonto, 
         sorterNaziv
      };

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }

   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      switch(sortType)
      {
         case VvSQL.SorterType.Konto/*Code*/: return new object[] { this.Konto, RecVer };
         case VvSQL.SorterType.Name: return new object[] { this.Naziv, RecVer };

         default: ZXC.aim_emsg(recordName + " Nema definiran sorter " + sortType.ToString());
            return null;
      }
   }

   #endregion SorterCurrVal

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Kplan();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Kplan)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Kplan)vvDataRecord).currentData;
   }

   public override string SaveSerialized_VvDataRecord_ToXmlFile(string fileName, bool _isAutoCreat)
   {
      return SaveSerialized_VvDataRecord_ToXmlFile_JOB<Kplan>(fileName, _isAutoCreat);
   }

   public override VvDataRecord Deserialize_VvDataRecord_FromXmlFile(string fileName)
   {
      return DeserializeFromXmlFile<Kplan>(fileName);
   }


   #endregion VvDataRecordFactory


#if PUSE__GetPlacaKonta
   public static string GetPlacaKonta(ZXC.VirmanEnum virmanEnum, /* fuse*/ string ROOT_Ticker, ZXC.SaldoOrDugOrPot dug_pot)
   {
               string[] [] [] PLACA_KTA = new string[][][]
/*                    \  \  \      */
/*                    \  \  \      */
/*                    \  \  \      */
//           BOB/OTHER\  \  \        
//              DUG / POT\  \        
//  POR,PRI,MIO,............\        
//           2300,..............     
      {
            
         /*BOBESIC*/ 
         new string[][] {
         //                      POR     PRI     MIO      MII     ZDR       ZOR     ZPI      ZAP     ZPP      NET     KRP    PREVOZ    M1Na       M2Na
         /*DUG*/ new string[] { "4210", "4210", "4211",  "4212", "4220",   "4221", "4223",  "4222", "zpp??", "4200", "4252", "4610", "m1Na??"  , "m2Na??"},
         /*POT*/ new string[] { "2410", "2411", "2420",  "2421", "2423",   "2424", "2428",  "2426", "zpp??", "2300", "2414", "2300", "m1Na??"  , "m2Na??"}
         }, 
         /*OTHER*/ 
         new string[][] {
         //                      POR     PRI     MIO      MII      ZDR      ZOR      ZPI       ZAP      ZPP      NET 
         /*DUG*/ new string[] { "4250", "4251", "4270" , "4271",  "4260",  "4265",  "4266",  "4262",  "4263",  "4200", "4252", "4610", "42711"  , "42712"},
         /*POT*/ new string[] { "2410", "2411", "24204", "24205", "24200", "24212", "24213", "24202", "24203", "2300", "2414", "2300", "24221"  , "24222"}
         },
         /*KOLIC*/ 
         new string[][] {
         //                      POR     PRI     MIO      MII      ZDR      ZOR      ZPI       ZAP      ZPP      NET 
         /*DUG*/ new string[] { "4250", "4251", "4260",   "4261", "4270",  "4271",  "4266",  "4272",  "4263",  "4200", "4252", "4610", "m1Na??"  , "m2Na??"},
         /*POT*/ new string[] { "2410", "2411", "2420",   "2421", "2423",  "2424",  "24213", "2426",  "24203", "2300", "2414", "2300", "m1Na??"  , "m2Na??"}
         },
         /*MANDAR*/ 
         new string[][] {
         //                      POR     PRI     MIO      MII      ZDR      ZOR      ZPI       ZAP      ZPP      NET 
         /*DUG*/ new string[] { "4210", "4210", "4210",   "4210", "4210",  "4210",  "4210",  "4210",  "4210",  "4200", "4252", "4610", "m1Na??"  , "m2Na??"},
         /*POT*/ new string[] { "2410", "2410", "2420",   "2421", "2422",  "2423",  "24213", "2424",  "2424", "2300", "2414", "2300", "m1Na??"  , "m2Na??"}
         }

      };

      int firmaIdx, dugPotIdx, virEnumIdx;

      //switch(ROOT_Ticker)
      //{
      //   case "RAUBOB": firmaIdx = 0; break;
      //   case "KROVAL": firmaIdx = 2; break;
      //   default      : firmaIdx = 1; break;
      //}

      switch(ZXC.VvDeploymentSite)
      {

         case ZXC.VektorSiteEnum.BOBESIC: firmaIdx = 0; break;
         case ZXC.VektorSiteEnum.KROVAL : firmaIdx = 2; break;
         case ZXC.VektorSiteEnum.MANDAR : firmaIdx = 3; break;
         default                        : firmaIdx = 1; break;
      }

      if(dug_pot == ZXC.SaldoOrDugOrPot.POT) dugPotIdx = 1;
      else                                   dugPotIdx = 0;

      virEnumIdx = (int)virmanEnum;

      return PLACA_KTA[firmaIdx][dugPotIdx][virEnumIdx];

   }
#endif

   public static string GetPlacaKonta_New(ZXC.VirmanEnum virmanEnum, ZXC.SaldoOrDugOrPot dug_pot, string tt)
   {
      KtoShemaPlacaDsc kspl = new KtoShemaPlacaDsc(ZXC.dscLuiLst_KtoShemaPlaca); 
               string[] [] [] PLACA_KTA = new string[][][]
/*                    \  \  \      */
/*                    \  \  \      */
/*                    \  \  \      */
//           BOB/OTHER\  \  \        
//              DUG / POT\  \        
//  POR,PRI,MIO,............\        
//           2300,..............     
      {
            
         /*RR*/ 
         new string[][] {
         //                         POR               PRI                MIO               MII                ZDR                 ZOR             ZPI             ZAP             ZPP             NET            KRP     PREVOZ             M1Na                 M2Na            
    //   /*DUG*/ new string[] { kspl.Dsc_Porez_D, kspl.Dsc_Prirez_D, kspl.Dsc_MIO_I_D, kspl.Dsc_MIO_II_D, kspl.Dsc_ZdrOsig_D, kspl.Dsc_ZOR_D, kspl.Dsc_ZPI_D, kspl.Dsc_ZAP_D, kspl.Dsc_ZPP_D, kspl.Dsc_NETO_D, "4252", kspl.Dsc_PREVOZ_D, kspl.Dsc_MIO_1Na_D, kspl.Dsc_MIO_2Na_D},
    //   /*POT*/ new string[] { kspl.Dsc_Porez_P, kspl.Dsc_Prirez_P, kspl.Dsc_MIO_I_P, kspl.Dsc_MIO_II_P, kspl.Dsc_ZdrOsig_P, kspl.Dsc_ZOR_P, kspl.Dsc_ZPI_P, kspl.Dsc_ZAP_P, kspl.Dsc_ZPP_P, kspl.Dsc_NETO_P, "4252", kspl.Dsc_PREVOZ_P, kspl.Dsc_MIO_1Na_P, kspl.Dsc_MIO_2Na_P}
         //05.03.2020. za knjizenje nocih NP63, NP65, NP71
         //                         POR               PRI                MIO               MII                ZDR                 ZOR             ZPI             ZAP             ZPP             NET            KRP     PREVOZ             M1Na                 M2Na                  np63                     np65                   np71        
   //    /*DUG*/ new string[] { kspl.Dsc_Porez_D, kspl.Dsc_Prirez_D, kspl.Dsc_MIO_I_D, kspl.Dsc_MIO_II_D, kspl.Dsc_ZdrOsig_D, kspl.Dsc_ZOR_D, kspl.Dsc_ZPI_D, kspl.Dsc_ZAP_D, kspl.Dsc_ZPP_D, kspl.Dsc_NETO_D, "4252", kspl.Dsc_PREVOZ_D, kspl.Dsc_MIO_1Na_D, kspl.Dsc_MIO_2Na_D, kspl.Dsc_NP63_nagr_D, kspl.Dsc_NP65_preh_D, kspl.Dsc_NP71_dZdr_D},
   //    /*POT*/ new string[] { kspl.Dsc_Porez_P, kspl.Dsc_Prirez_P, kspl.Dsc_MIO_I_P, kspl.Dsc_MIO_II_P, kspl.Dsc_ZdrOsig_P, kspl.Dsc_ZOR_P, kspl.Dsc_ZPI_P, kspl.Dsc_ZAP_P, kspl.Dsc_ZPP_P, kspl.Dsc_NETO_P, "4252", kspl.Dsc_PREVOZ_P, kspl.Dsc_MIO_1Na_P, kspl.Dsc_MIO_2Na_P, kspl.Dsc_NP63_nagr_P, kspl.Dsc_NP65_preh_P, kspl.Dsc_NP71_dZdr_P}
         //25.01.2022. jos neki neoporezivi primici 
         //                         POR               PRI                MIO               MII                ZDR                 ZOR             ZPI             ZAP             ZPP             NET            KRP     PREVOZ             M1Na                 M2Na                  np63                     np65                   np71                  np17                  np18                  np21                  np22                  np26                  np60        
         /*DUG*/ new string[] { kspl.Dsc_Porez_D, kspl.Dsc_Prirez_D, kspl.Dsc_MIO_I_D, kspl.Dsc_MIO_II_D, kspl.Dsc_ZdrOsig_D, kspl.Dsc_ZOR_D, kspl.Dsc_ZPI_D, kspl.Dsc_ZAP_D, kspl.Dsc_ZPP_D, kspl.Dsc_NETO_D, "4252", kspl.Dsc_PREVOZ_D, kspl.Dsc_MIO_1Na_D, kspl.Dsc_MIO_2Na_D, kspl.Dsc_NP63_nagr_D, kspl.Dsc_NP65_preh_D, kspl.Dsc_NP71_dZdr_D, kspl.Dsc_NP17_dnev_D, kspl.Dsc_NP18_loko_D, kspl.Dsc_NP21_darD_D, kspl.Dsc_NP22_regB_D, kspl.Dsc_NP26_otpr_D, kspl.Dsc_NP60_jubi_D},
         /*POT*/ new string[] { kspl.Dsc_Porez_P, kspl.Dsc_Prirez_P, kspl.Dsc_MIO_I_P, kspl.Dsc_MIO_II_P, kspl.Dsc_ZdrOsig_P, kspl.Dsc_ZOR_P, kspl.Dsc_ZPI_P, kspl.Dsc_ZAP_P, kspl.Dsc_ZPP_P, kspl.Dsc_NETO_P, "4252", kspl.Dsc_PREVOZ_P, kspl.Dsc_MIO_1Na_P, kspl.Dsc_MIO_2Na_P, kspl.Dsc_NP63_nagr_P, kspl.Dsc_NP65_preh_P, kspl.Dsc_NP71_dZdr_P, kspl.Dsc_NP17_dnev_P, kspl.Dsc_NP18_loko_P, kspl.Dsc_NP21_darD_P, kspl.Dsc_NP22_regB_P, kspl.Dsc_NP26_otpr_P, kspl.Dsc_NP60_jubi_P}
         }, 
         /*PP*/ 
         new string[][] {
         //                            POR               PRI                MIO               MII                ZDR                 ZOR             ZPI             ZAP             ZPP             NET              KRP     PREVOZ             M1Na                 M2Na                                 n63 n65 n71 n17 n18 n21 n22 n26 n60   
         /*DUG*/ new string[] { kspl.Dsc_PpPorez_D, kspl.Dsc_PpPrirez_D, kspl.Dsc_PpMIO_I_D, kspl.Dsc_PpMIO_II_D, kspl.Dsc_PpZdrOsig_D, kspl.Dsc_PpZOR_D, kspl.Dsc_PpZPI_D, kspl.Dsc_PpZAP_D, kspl.Dsc_PpZPP_D, kspl.Dsc_PpNETO_D, "4252", kspl.Dsc_PpPREVOZ_D, kspl.Dsc_PpMIO_1Na_D, kspl.Dsc_PpMIO_2Na_D, "", "", "", "", "", "", "", "", "" },
         /*POT*/ new string[] { kspl.Dsc_PpPorez_P, kspl.Dsc_PpPrirez_P, kspl.Dsc_PpMIO_I_P, kspl.Dsc_PpMIO_II_P, kspl.Dsc_PpZdrOsig_P, kspl.Dsc_PpZOR_P, kspl.Dsc_PpZPI_P, kspl.Dsc_PpZAP_P, kspl.Dsc_PpZPP_P, kspl.Dsc_PpNETO_P, "4252", kspl.Dsc_PpPREVOZ_P, kspl.Dsc_PpMIO_1Na_P, kspl.Dsc_PpMIO_2Na_P, "", "", "", "", "", "", "", "", "" }
         },
         /*AH*/ 
         new string[][] {
         //                     POR                 PRI                          MIO                  MII                 ZDR           ZOR ZPI               ZAP  ZPP  NET                 KRP    PREVOZ  M1Na  M2Na n63 n65 n71 n17 n18 n21 n22 n26 n60 
         /*DUG*/ new string[] { kspl.Dsc_AhPorez_D, kspl.Dsc_AhPrirez_D, kspl.Dsc_AhMIO_I_D, kspl.Dsc_AhMIO_II_D, kspl.Dsc_AhZdrOsig_D, "", kspl.Dsc_AhZPI_D, "",  "",  kspl.Dsc_AhNETO_D, "4252", "",     "",   ""  , "", "", "", "", "", "", "", "", "" },
         /*POT*/ new string[] { kspl.Dsc_AhPorez_P, kspl.Dsc_AhPrirez_P, kspl.Dsc_AhMIO_I_P, kspl.Dsc_AhMIO_II_P, kspl.Dsc_AhZdrOsig_P, "", kspl.Dsc_AhZPI_P, "",  "",  kspl.Dsc_AhNETO_P, "4252", "",     "",   ""  , "", "", "", "", "", "", "", "", "" }
         },
         /*UD*/ 
         new string[][] {
         //                     POR                 PRI                  MIO                 MII                  ZDR                   ZOR  ZPI               ZAP  ZPP NET                KRP     PREVOZ  M1Na  M2Na n63 n65 n71 n17 n18 n21 n22 n26 n60 
         /*DUG*/ new string[] { kspl.Dsc_UdPorez_D, kspl.Dsc_UdPrirez_D, kspl.Dsc_UdMIO_I_D, kspl.Dsc_UdMIO_II_D, kspl.Dsc_UdZdrOsig_D, "" , kspl.Dsc_UdZPI_D, "" , "", kspl.Dsc_UdNETO_D, "4252", ""    , ""  , ""  , "", "", "", "", "", "", "", "", "" },
         /*POT*/ new string[] { kspl.Dsc_UdPorez_P, kspl.Dsc_UdPrirez_P, kspl.Dsc_UdMIO_I_P, kspl.Dsc_UdMIO_II_P, kspl.Dsc_UdZdrOsig_P, "" , kspl.Dsc_UdZPI_P, "" , "", kspl.Dsc_UdNETO_P, "4252", ""    , ""  , ""  , "", "", "", "", "", "", "", "", "" }
         },
         /*NO*/ 
         new string[][] {
         //                     POR                 PRI                  MIO                 MII                  ZDR                   ZOR  ZPI               ZAP  ZPP NET                 KRP    PREVOZ M1Na  M2Na n63 n65 n71 n17 n18 n21 n22 n26 n60 
         /*DUG*/ new string[] { kspl.Dsc_NoPorez_D, kspl.Dsc_NoPrirez_D, kspl.Dsc_NoMIO_I_D, kspl.Dsc_NoMIO_II_D, kspl.Dsc_NoZdrOsig_D, "" , kspl.Dsc_NoZPI_D, "" , "", kspl.Dsc_NoNETO_D, "4252", ""    ,""  , ""  , "", "", "", "", "", "", "", "", "" },
         /*POT*/ new string[] { kspl.Dsc_NoPorez_P, kspl.Dsc_NoPrirez_P, kspl.Dsc_NoMIO_I_P, kspl.Dsc_NoMIO_II_P, kspl.Dsc_NoZdrOsig_P, "" , kspl.Dsc_NoZPI_P, "" , "", kspl.Dsc_NoNETO_P, "4252", ""    ,""  , ""  , "", "", "", "", "", "", "", "", "" }
         },
         /*SZ*/ 
         new string[][] {
         //                     POR                 PRI                  MIO                 MII                  ZDR                   ZOR  ZPI               ZAP  ZPP NET                 KRP    PREVOZ M1Na  M2Na n63 n65 n71 n17 n18 n21 n22 n26 n60 
         /*DUG*/ new string[] { kspl.Dsc_SzPorez_D, kspl.Dsc_SzPrirez_D, ""                ,                  "",                   "", "" ,               "", "" , "", kspl.Dsc_SzNETO_D,    "",     "",   "", ""  , "", "", "", "", "", "", "", "", "" },
         /*POT*/ new string[] { kspl.Dsc_SzPorez_P, kspl.Dsc_SzPrirez_P, ""                ,                  "",                   "", "" ,               "", "" , "", kspl.Dsc_SzNETO_P,    "",     "",   "", ""  , "", "", "", "", "", "", "", "", "" }
         }
      };

      int placaTTidx, dugPotIdx, virEnumIdx;

      switch(tt)
      {

         case Placa.TT_REDOVANRAD  : placaTTidx = 0; break;
         case Placa.TT_PODUZETPLACA: placaTTidx = 1; break;
         case Placa.TT_IDD_KOLONA_4:
         case Placa.TT_AUTORHONUMJ :
         case Placa.TT_AHSAMOSTUMJ:
         case Placa.TT_AUTORHONOR  : placaTTidx = 2; break;
         case Placa.TT_UGOVORODJELU: placaTTidx = 3; break;
         case Placa.TT_NADZORODBOR : placaTTidx = 4; break;
         case Placa.TT_SEZZAPPOLJOP: placaTTidx = 5; break;
         default                   : placaTTidx = 0; break;
      }

      if(dug_pot == ZXC.SaldoOrDugOrPot.POT) dugPotIdx = 1;
      else                                   dugPotIdx = 0;

      virEnumIdx = (int)virmanEnum;

      return PLACA_KTA[placaTTidx][dugPotIdx][virEnumIdx];

   }

   public static bool GetIsKontoDobav(string konto)
   {
      foreach(string ktoRoot in Ftrans.WantedDobavKontaStringArray) if(konto.StartsWith(ktoRoot)) return true;

      return false;
   }
   public static bool GetIsKontoKupac(string konto)
   {
      foreach(string ktoRoot in Ftrans.WantedKupciKontaStringArray) if(konto.StartsWith(ktoRoot)) return true;

      return false;
   }
   public bool IsKontoDobav { get { return GetIsKontoDobav(this.Konto); } }
   public bool IsKontoKupac { get { return GetIsKontoKupac(this.Konto); } }

}

///// <summary>
///// Dakle, dok ne dodje C# 3.0 sa LINQ-om, da bi VvListu customizirano pretrazivao po npr. kontu, moras ovako 
///// raditi inheritirane klase koje onda extendiras methodom za customizirano pretrazivanje npr. 'FindByKonto'
///// Kad stigne LINQ, ovu ces klasu pobrisati!
///// </summary>
//public class KplanList : VvList<Kplan>
//{
//   public KplanList() : base()
//   {
//   }

//   public KplanList(int initialCapacity) : base(initialCapacity)
//   {
//   }

//   public Kplan FindByKonto(string wantedKonto)
//   {
//      return Find(
//         delegate(Kplan kplan_rec) // ovo je kak ti anonimous method 
//         {
//            return kplan_rec.Konto == wantedKonto;
//         }
//      );
//   }

//   public static KplanList Convert_VvList_To_KplanList(VvList<Kplan> vvList)
//   {
//      KplanList outputList = new KplanList(vvList.Count);

//      outputList.AddRange(vvList);

//      return outputList;
//   }
//}

