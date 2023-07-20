using System;

#region struct RtranoStruct

public struct RtranoStruct
{
   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   internal uint     _recID;

   /* 01 */  /*internal*/ public uint      _t_parentID;
   /* 02 */  /*internal*/ public uint      _t_dokNum;
   /* 03 */  /*internal*/ public ushort    _t_serial;
   /* 04 */  /*internal*/ public DateTime  _t_skladDate;
   /* 05 */  /*internal*/ public string    _t_tt;
   /* 06 */  /*internal*/ public uint      _t_ttNum;
   /* 07 */  /*internal*/ public Int16     _t_ttSort    ;
   /* 08 */  /*internal*/ public string    _t_artiklCD  ;
   /* 09 */  /*internal*/ public string    _t_skladCD   ;
   /* 10 */  /*internal*/ public string    _t_artiklName;
   /* 11 */  /*internal*/ public uint      _t_kupdob_cd ;
   /* 12 */  /*internal*/ public string    _t_serno     ;
   /* 13 */  /*internal*/ public uint      _t_paletaNo  ;
   /* 14 */  /*internal*/ public decimal   _t_dimX      ;
   /* 15 */  /*internal*/ public decimal   _t_dimY      ;
   /* 16 */  /*internal*/ public decimal   _t_dimZ      ;
   /* 17 */  /*internal*/ public decimal   _t_komada    ;
   /* 18 */  /*internal*/ public decimal   _t_kol       ;
   /* 19 */  /*internal*/ public string    _t_grCD      ;
   /* 20 */  /*internal*/ public bool      _t_isKomDummy;
   /* 21 */  /*internal*/ public decimal   _t_decA      ;
   /* 22 */  /*internal*/ public decimal   _t_decB      ;
   /* 23 */  /*internal*/ public decimal   _t_decC      ;
   /* 24 */  /*internal*/ public uint      _t_rtrRecID  ;

             //internal RtransResultStruct _ptrResult;

}

#endregion struct RtranoStruct

public class Rtrano : VvTransRecord
{

   #region Fildz

   public const string recordName = "rtrano";
   public const string recordNameArhiva = recordName + VvDataRecord.ArhRecNameExstension;

   private RtranoStruct currentData;
   private RtranoStruct backupData;

   protected static System.Data.DataTable TheSchemaTable = ZXC.RtranoDao.TheSchemaTable;
   protected static RtranoDao.RtranoCI    CI             = ZXC.RtranoDao.CI;

   #endregion Fildz

   #region Constructors

   public Rtrano() : this(0)
   {
   }

   public Rtrano(uint ID) : base()
   {
      this.currentData = new RtranoStruct();

      Memset0(ID);
   }

   public Rtrano(Rtrans rtrans_rec/*, ushort newSerial*/) : base()
   {
      this.currentData = new RtranoStruct();

      Memset0(0);

    //this.T_serial     = newSerial              ; 

      this.T_parentID   = rtrans_rec.T_parentID  ;
      this.T_dokNum     = rtrans_rec.T_dokNum    ;
      this.T_skladDate  = rtrans_rec.T_skladDate ;
      this.T_TT         = rtrans_rec.T_TT        ;
      this.T_ttNum      = rtrans_rec.T_ttNum     ;
      this.T_ttSort     = rtrans_rec.T_ttSort    ;
      this.T_artiklCD   = rtrans_rec.T_artiklCD  ;
      this.T_skladCD    = rtrans_rec.T_skladCD   ;
      this.T_artiklName = rtrans_rec.T_artiklName;
      this.T_kupdobCD   = rtrans_rec.T_kupdobCD  ;

      this.T_paletaNo   = rtrans_rec.T_serial    ; // ! veza

      this.T_dimZ       = rtrans_rec.T_doCijMal  ;
      this.T_decC       = rtrans_rec.T_noCijMal  ;

   }

   public override void Memset0(uint ID)
   {
      this.currentData._recID = ID;

      // well, svi reference types (string, date, ...)

      /* 01 */   this.currentData._t_parentID  = 0;
      /* 02 */   this.currentData._t_dokNum    = 0;
      /* 03 */   this.currentData._t_serial    = 0;
      /* 04 */   this.currentData._t_skladDate   = DateTime.MinValue;
      /* 05 */   this.currentData._t_tt        = "";
      /* 06 */   this.currentData._t_ttNum     = 0;
      /* 07 */   this.currentData._t_ttSort    = 0;
      /* 08 */   this.currentData._t_artiklCD  = "";
      /* 09 */   this.currentData._t_skladCD   = "";
      /* 10 */   this.currentData._t_artiklName= "";
      /* 11 */   this.currentData._t_kupdob_cd = 0;
      /* 12 */   this.currentData._t_serno     = "";

      /* 13 */   this.currentData._t_paletaNo  = 0;
      /* 14 */   this.currentData._t_dimX      = 0.00M;
      /* 15 */   this.currentData._t_dimY      = 0.00M;
      /* 16 */   this.currentData._t_dimZ      = 0.00M;
      /* 17 */   this.currentData._t_komada    = 0.00M;
      /* 18 */   this.currentData._t_kol       = 0.00M;
      /* 19 */   this.currentData._t_grCD      = "";
      /* 20 */   this.currentData._t_isKomDummy= false;
      /* 21 */   this.currentData._t_decA      = 0.00M;
      /* 22 */   this.currentData._t_decB      = 0.00M;
      /* 23 */   this.currentData._t_decC      = 0.00M;
      /* 24 */   this.currentData._t_rtrRecID  = 0;


   }

   #endregion Constructors

   #region Sorters

   private VvSQL.RecordSorter[] _sorters = null;

   public override VvSQL.RecordSorter[] Sorters
   {
      get { return _sorters; }
   }


   public override object[] SorterCurrVal(VvSQL.SorterType sortType)
   {
      throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet.");
   }

   public override VvSQL.RecordSorter DefaultSorter
   {
      //get { return Rtrano.sorter_Person_DokDate_DokNum; }
      get 
      { 
         return Rtrans.sorterArtiklCD;
         //throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet."); 
         /*return new VvSQL.RecordSorter();*/ 
      }
   }

   #endregion Sorters

   #region Propertiz 

   #region General Propertiz

   internal RtranoStruct CurrentData // cijela RtranoStruct struct-ura 
   {
      get { return this.currentData; }
      set {        this.currentData = value; }
   }

   public override IVvDao VvDao
   {
      get { return ZXC.RtranoDao; }
   }

   public override string VirtualRecordName
   {
      get { return Rtrano.recordName; }
   }

   public override string VirtualRecordNameArhiva
   {
      get { return Rtrano.recordNameArhiva; }
   }

   public override string DocumentRecordName
   {
      get { return Faktur.recordName; }
   }

   public override string VirtualLegacyRecordPreffix
   {
      get { return "ts"; }
   }

   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public override uint VirtualRecID
   {
      get { return this.T_recID; }
      set {        this.T_recID = value; }
   }

   public override uint VirtualParentRecID
   {
      get { return this.T_parentID; }
      set {        this.T_parentID = value; }
   }

   public override ushort VirtualT_Serial
   {
      get { return this.T_serial; }
      set {        this.T_serial = value; }
   }

   public override uint VirtualT_dokNum
   {
      get { return this.T_dokNum; }
      set {        this.T_dokNum = value; }
   }

   public override uint VirtualT_ttNum
   {
      get { return this.T_ttNum;         }
      set {        this.T_ttNum = value; }
   }

   public static string ArtiklForeignKey
   {
      get { return "t_artiklCD"; }
   }

   public static string FakturForeignKey
   {
      get { return "t_parentID"; }
   }

 //public TtInfo TtInfo { get { try { return                        ZXC.RiskTT[this.T_TT]               ; } catch(Exception) { return new TtInfo(); } } }
   public TtInfo TtInfo { get { try { return this.T_TT.NotEmpty() ? ZXC.RiskTT[this.T_TT] : new TtInfo(); } catch(Exception) { return new TtInfo(); } } }

   #endregion General Propertiz

   #region DataLayer Propertiez

   [System.Xml.Serialization.XmlIgnoreAttribute()] // Da GetDgvLineFields ne misli krivo: "if(recID > 0) // Postojeci redak" 
   public uint T_recID
   {
      get { return this.currentData._recID; }
      set {        this.currentData._recID = value; }
   }

   /* 01 */ public uint T_parentID
   {
      get { return this.currentData._t_parentID; }
      set {        this.currentData._t_parentID = value; }
   }
   /* 02 */ public uint T_dokNum
   {
      get { return this.currentData._t_dokNum; }
      set {        this.currentData._t_dokNum = value; }
   }
   /* 03 */ public ushort T_serial
   {
      get { return this.currentData._t_serial; }
      set {        this.currentData._t_serial = value; }
   }
   /* 04 */ public DateTime T_skladDate
   {
      get { return this.currentData._t_skladDate; }
      set {        this.currentData._t_skladDate = value; }
   }
   /* 05 */ public string T_TT
   {
      get { return this.currentData._t_tt; }
      set {        this.currentData._t_tt = value; }
   }
   /* 06 */ public uint T_ttNum
   {
      get { return this.currentData._t_ttNum; }
      set {        this.currentData._t_ttNum = value; }
   }
   /* 07 */ public short T_ttSort
   {
      get { return this.currentData._t_ttSort; }
      set {        this.currentData._t_ttSort = value; }
   }
   /* 08 */ public string T_artiklCD
   {
      get { return this.currentData._t_artiklCD; }
      set {        this.currentData._t_artiklCD = value; }
   }
   /* 09 */ public string T_skladCD
   {
      get { return this.currentData._t_skladCD; }
      set {        this.currentData._t_skladCD = value; }
   }
   /* 10 */ public string T_artiklName
   {
      get { return this.currentData._t_artiklName; }
      set {        this.currentData._t_artiklName = value; }
   }
   /* 11 */ public uint T_kupdobCD
   {
      get { return this.currentData._t_kupdob_cd; }
      set {        this.currentData._t_kupdob_cd = value; }
   }
   /* 12 */ public string T_serno
   {
      get { return this.currentData._t_serno; }
      set {        this.currentData._t_serno = value; }
   }
   /* 13 */ public uint T_paletaNo
   {
      get { return this.currentData._t_paletaNo; }
      set {        this.currentData._t_paletaNo = value; }
   }
   /* 14 */ public decimal T_dimX
   {
      get { return this.currentData._t_dimX; }
      set {        this.currentData._t_dimX = value; }
   }
   /* 15 */ public decimal T_dimY
   {
      get { return this.currentData._t_dimY; }
      set {        this.currentData._t_dimY = value; }
   }
   /* 16 */ public decimal T_dimZ
   {
      get { return this.currentData._t_dimZ; }
      set {        this.currentData._t_dimZ = value; }
   }
   /* 17 */ public decimal T_komada
   {
      get { return this.currentData._t_komada; }
      set {        this.currentData._t_komada = value; }
   }

   /* 18 */ public decimal T_kol
   {
      get { return this.currentData._t_kol; }
      set {        this.currentData._t_kol = value; }
   }

   /* 12 */ public string T_grCD
   {
      get { return this.currentData._t_grCD; }
      set {        this.currentData._t_grCD = value; }
   }

   /* 20 */ public bool T_isKomDummy
   {
      get { return this.currentData._t_isKomDummy; }
      set {        this.currentData._t_isKomDummy = value; }
   }
   /* 21 */ public decimal T_decA
   {
      get { return this.currentData._t_decA; }
      set {        this.currentData._t_decA = value; }
   }
   /* 22 */ public decimal T_decB
   {
      get { return this.currentData._t_decB; }
      set {        this.currentData._t_decB = value; }
   }
   /* 23 */ public decimal T_decC
   {
      get { return this.currentData._t_decC; }
      set {        this.currentData._t_decC = value; }
   }
   /* 24 */ public uint T_rtrRecID
   {
      get { return this.currentData._t_rtrRecID; }
      set {        this.currentData._t_rtrRecID = value; }
   }
   #endregion DataLayer Propertiez

   public string R_artCDAndGrCD
   {
      get
      {
         return T_artiklCD + T_grCD;
      }
   }

   public decimal R_povrsina
   {
      get
      {
         decimal pretvornik = 1000.00M;

         return T_dimX * T_dimY / pretvornik;
      }
   }

   //public string R_PCK_RAMkind { get; set; }
   //public string R_PCK_HDDkind { get; set; }

   #endregion propertiz 

   #region ToString

   public override string ToString()
   {
      return T_serno + " TT: "     + T_TT + "-" + T_ttNum + " (" + T_skladDate.ToShortDateString() + ")" +
             " Red: "    + T_serial +
             " Artikl: " + T_artiklCD + "-" + T_artiklName;
                      
   }

   #endregion ToString

   #region Implements IEditableObject

   #region Utils
   /// <summary>
   /// this.currentData = this.backupData;
   /// </summary>
   public override void RestoreBackupData()
   {
      Generic_RestoreBackupData<RtranoStruct>(ref this.currentData, ref this.backupData);
   }

   #endregion Utils

   public override void /*IEditableObject.*/BeginEdit()
   {
      Generic_BeginEdit<RtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/CancelEdit()
   {
      Generic_CancelEdit<RtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override void /*IEditableObject.*/EndEdit()
   {
      Generic_EndEdit<RtranoStruct>(ref this.currentData, ref this.backupData);
   }

   public override bool EditedHasChanges()
   {
      return Generic_EditedHasChanges<RtranoStruct>(this.currentData, this.backupData);
   }

   #endregion

   #region ICloneable Members

   public override object Clone()
   {
      Rtrano newObject = new Rtrano();

      Generic_CloneData<RtranoStruct>(this.currentData, this.backupData, ref newObject.currentData, ref newObject.backupData);

      // 1.4.2011: !!! NOTA BENE for future; VvDataRecord.Clone() ti ne klonira eventualne property-e koji nisu u currentData strukturi, a zivi su podatak a ne izvedenice npr: 
      newObject.SaveTransesWriteMode = this.SaveTransesWriteMode;

      return newObject;
   }

   public Rtrano MakeDeepCopy()
   {
      return (Rtrano)Clone();
   }

   #endregion

   #region VvDataRecordFactory

   public override VvDataRecord VvDataRecordFactory()
   {
      return new Rtrano();
   }

   public override void TakeCurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.currentData = ((Rtrano)vvDataRecord).currentData;
   }

   public override void TakeInBackupData_CurrentDataFrom(VvDataRecord vvDataRecord)
   {
      this.backupData = ((Rtrano)vvDataRecord).currentData;
   }

   #endregion VvDataRecordFactory


   #region CalcVolumen (this.T_kol)

   internal void CalcVolumen(ZXC.Rtrano_CalcVolumenEnum calcKind, bool komIsDummy)
   {
      switch(calcKind)
      {
         case ZXC.Rtrano_CalcVolumenEnum.TRUPAC: CalcTrupacVolumen(komIsDummy); break;
         case ZXC.Rtrano_CalcVolumenEnum. LETVA: CalcLetvaVolumen (komIsDummy); break;
      }
   }

   private void CalcLetvaVolumen(bool komIsDummy)
   {
      if(T_dimX.IsZero() || T_dimY.IsZero() || T_dimZ.IsZero()) return;

      decimal pretvornik = 1000.00M;

      decimal komada = T_komada.IsZero() || komIsDummy ? 1.00M : T_komada;

      T_kol = (T_dimX * T_dimY * T_dimZ * komada) 
              /
              (pretvornik * pretvornik * pretvornik);
      T_kol = T_kol.Ron(3);
   }

   private void CalcTrupacVolumen(bool komIsDummy)
   {
      // 313.04.2013: ako je PIX i zadan je t_serno, NE mnozi parametre vec t_kol uzmi zdravo za gotovo sa 'PRI-S'-a tj return-aj se odavdje 
      if(T_serno.NotEmpty())                                        return;
      if(T_dimX.IsZero() || /*T_dimY.IsZero() ||*/ T_dimZ.IsZero()) return;

      decimal pretvornik = 100.00M, radius, PI = (decimal)Math.PI;

      radius = T_dimZ / 2.00M;

      decimal komada = T_komada.IsZero() || komIsDummy ? 1.00M : T_komada;

      T_kol = (radius * radius * PI * T_dimX * komada) 
              /
              (pretvornik * pretvornik * pretvornik);
      T_kol = T_kol.Ron(3);
   }

#if CalcPPUK_da_Offix


   xDim - Duzina
   yDim - Sirina
   zDim - Debljina, Promjer
   kDim - Komada

   noKom - bool koji kaze // dakle ako juzer upise 'N' onda nemoj mnoziti kDim sa xyz-ovima

double GetPPukKolVia_LetvaMath(double xDim, double yDim, double zDim, double kDim, int noKom)
{
   double pretvornik = 1000.00;
   
   if(noKom && notZero(kDim)) kDim = 1.00;
   
   return /*ron2*/
      (
         (ron0/*2*/(xDim) * ron0(yDim) * ron0(zDim) * ron0(kDim)) / 
         (pretvornik * pretvornik * pretvornik)
      ); 
}

double GetPPukKolVia_TrupacMath(double xDim, double yDim, double zDim, double kDim, int noKom)
{
   double pretvornik = 100.00, radius;
   
   radius = ron0(zDim) / 2.00;
   
   if(noKom && notZero(kDim)) kDim = 1.00;
   
   return /*ron2*/
      (
         (ron0/*2*/(xDim) * pow(radius, 2) * ron0(kDim)) * M_PI / 
         (pretvornik * pretvornik * pretvornik)
      ); 
}

int HasSense_GetPPukKolVia_LetvaMath(double xDim, double yDim, double zDim, double kDim, int noKom)
{
   if(noKom && notZero(kDim)) kDim = 1.00;

   if(isZero(xDim) ||
      isZero(yDim) ||
      isZero(zDim) ||
      isZero(kDim)) return(0);
   else             return(1);
}

HasSense_GetPPukKolVia_TrupacMath(double xDim, double yDim, double zDim, double kDim, int noKom)
{
   if(noKom && notZero(kDim)) kDim = 1.00;

   if(isZero(xDim) ||
      notZero(yDim)||
      isZero(zDim) ||
      isZero(kDim)) return(0);
   else             return(1);
}

RaiseTrupacMathFlag(i)
int                 i;
{
   sm_i_putfield("x_trupacMathFlag",  i, "T");
   sm_i_putfield("x_centimetarFlag1", i, "c");
   sm_i_putfield("x_centimetarFlag2", i, "c");
}

DropTrupacMathFlag(i)
int                i;
{
   sm_i_putfield("x_trupacMathFlag",  i, "");
   sm_i_putfield("x_centimetarFlag1", i, "");
   sm_i_putfield("x_centimetarFlag2", i, "");
}

Is_kDim_Dummy(xDim, yDim, zDim, kDim, kol)
double        xDim, yDim, zDim, kDim, kol;
{
   double pretvornik = 1000.00;
   double theoreticKol;
   
   theoreticKol = (ron0(xDim) * ron0(yDim) * ron0(zDim) * 1.00/*ron0(kDim)*/) / 
                  (pretvornik * pretvornik * pretvornik);
                  
   if(fabs(theoreticKol - kol) < 0.01) return(1);
   else                                return(0); 
}

#endif

   #endregion CalcVolumen (this.T_kol)

}

