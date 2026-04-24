using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
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
#endif

public abstract class VvUserControl              : UserControl
{
   /// <summary>
   /// Ako ces ikada trebati Unique Identifier za VvUC
   /// On je TimStamp trenutka nastanka objekta 
   /// </summary>
   public DateTime JMBG_UUID { get; private set; }

   public VvUserControl()
   {
      JMBG_UUID = DateTime.Now;
   }

   public void InitializeVvUserControl(Control parent)
   {
      // 14.02.2022: za potrebe 'FakturList_To_PDF' 
      if(parent == null) return;

      ZXC.ZaUpis zaUpis;

      this.Parent = parent;

      // 14.11. mislim da je ovo razlog blesiranja kod resizanja uc-a (lijevo)
      //if (parent.Width < 792) this.Size = new Size(parent.Width, parent.Height);
      //else this.Size = new Size(792, parent.Height);
      this.Size = new Size(parent.Width, parent.Height);

      this.Anchor     = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      this.BackColor  = ZXC.vvColors.userControl_BackColor;
      this.AutoScroll = true;

      toolTip            = new ToolTip();
      toolTip.ShowAlways = true;

      switch(this.TheVvTabPage.TabPageKind)
      {
         case ZXC.VvTabPageKindEnum.RECORD_TabPage:
            zaUpis = ZXC.ZaUpis.Zatvoreno;
            pck    = ZXC.ParentControlKind.VvRecordUC;
    
            ((VvRecordUC)this).Create_DummyForDefaultFocus();
             break;

         case ZXC.VvTabPageKindEnum.RECORD_TabPage_INTERACTIVE:
             zaUpis = ZXC.ZaUpis.Otvoreno;
             pck    = ZXC.ParentControlKind.VvRecordUC;

             ((VvRecordUC)this).Create_DummyForDefaultFocus();
             break;

         case ZXC.VvTabPageKindEnum.RecLIST_TabPage:
            zaUpis = ZXC.ZaUpis.Otvoreno;
            pck    = ZXC.ParentControlKind.VvFindDialog;
            break;
         
         case ZXC.VvTabPageKindEnum.REPORT_TabPage:
            zaUpis = ZXC.ZaUpis.Otvoreno;
            pck    = ZXC.ParentControlKind.VvReportUC;
            break;

         default:
            zaUpis = ZXC.ZaUpis.Zatvoreno;
            pck    = ZXC.ParentControlKind.VvOtherUC;
            break;
      }

      if(this.TheVvTabPage.TabPageKind != ZXC.VvTabPageKindEnum.REPORT_TabPage)
          VvHamper.Open_Close_Fields_ForWriting(this, zaUpis, pck);

   }

   //public abstract void PutFields();
   public abstract void GetFields(bool dirtyFlagging);

   private ZXC.ParentControlKind pck;
   public  ZXC.ParentControlKind ParentControlKind
   {
      //get { return pck; }
      set { pck = value; }
   }

   public Control ControlForInitialFocus;
   public ToolTip toolTip;

   // ------------------------------------------------------------------
   // C12 (Faza 1c, V4 §3.1c) — decoupling od fragilne Parent.Parent navigacije.
   //
   // Tri settable property-ja s fallback-safe ponasanjem:
   //   - DocumentHost   : default = ZXC.TheVvForm (postojece ponasanje).
   //                      U Fazi 3 VvFloatingForm postavlja sebe kao host.
   //   - TheVvTabPage   : default = (VvTabPage)this.Parent.Parent (postojece).
   //                      U Fazi 3 reparent u VvFloatingForm: setter se koristi
   //                      da UC zadrzi referencu na ishodisni tab.
   //   - TheDbConnection: fallback lanac prosiren s DocumentHost?.TheDbConnection
   //                      izmedu tab-a i ZXC.TheVvForm (globalno) fallbacka.
   //
   // Nijedan postojeci call-site ne mijenja ponasanje dok property nije eksplicitno
   // postavljen — zato C12 moze biti atomic commit bez regresija.
   // ------------------------------------------------------------------

   private IVvDocumentHost _documentHost;
   public  IVvDocumentHost DocumentHost
   {
      get { return _documentHost ?? (IVvDocumentHost)ZXC.TheVvForm; }
      set { _documentHost = value; }
   }

   private VvTabPage _theVvTabPage;
   public  VvTabPage TheVvTabPage
   {
      get
      {
         if(_theVvTabPage != null) return _theVvTabPage;

         if(this.Parent != null && this.Parent.Parent != null)
            return ((VvTabPage)(this.Parent.Parent));
         else
            return null;
      }
      set { _theVvTabPage = value; }
   }

   /*protected*/internal XSqlConnection TheDbConnection
   {
      get
      {
         if(pck == ZXC.ParentControlKind.VvFindDialog || TheVvTabPage == null)
         {
            // C12: prvo pokusaj kroz DocumentHost (u Fazi 3 VvFloatingForm),
            // pa tek onda globalni ZXC.TheVvForm fallback.
            return DocumentHost?.TheDbConnection ?? ZXC.TheVvForm.TheVvTabPage.TheDbConnection;
         }
         else
         {
            return this.TheVvTabPage.TheDbConnection;
         }
      }
   }

   public bool IsOnReportTabPage
   {
      get { return TheVvTabPage.IsForReport; }
   }

   public virtual bool OldVvXmlDRfilesDeleted { get; set; }

   public VvSubModul TheSubModul { get; set; }

   public virtual VvRptFilter VirtualRptFilter
   {
      get { return null; }
      set {              }
   }

   public virtual Size ThisUcSize
   {
      get { return Size.Empty; }
      set { ; }
   }

   public virtual ZXC.DbNavigationRestrictor VvNavRestrictor_TT
   {
      get { return ZXC.DbNavigationRestrictor.Empty; }
      //set { }
   }

   public virtual ZXC.DbNavigationRestrictor VvNavRestrictor_SKL
   {
      get { return ZXC.DbNavigationRestrictor.Empty; }
      //set { }
   }

   public virtual ZXC.DbNavigationRestrictor VvNavRestrictor_SKL2
   {
      get { return ZXC.DbNavigationRestrictor.Empty; }
      //set { }
   }


   #region Common DataGridWiev Columns Formatting

   public static string GetDgvCellStyleFormat_Number(int numOfDecimalPlaces, bool clearIfZero)
   {
      return GetDgvCellStyleFormat_Number(numOfDecimalPlaces, clearIfZero, false);
   }

   public static string GetDgvCellStyleFormat_Number(int numOfDecimalPlaces, bool clearIfZero, bool isForPercent)
   {
      string numericFormat="", zeroFormat, percentSign;

      if(clearIfZero) zeroFormat = "#";
      else            zeroFormat = "";

      if(isForPercent) percentSign = "'%'";
      else             percentSign = "";

      switch(numOfDecimalPlaces)
      {
         case 0: numericFormat = "#,##0"          + percentSign + ";;" + zeroFormat; break;
         case 1: numericFormat = "#,##0.0"        + percentSign + ";;" + zeroFormat; break;
         case 2: numericFormat = "#,##0.00"       + percentSign + ";;" + zeroFormat; break;
         case 3: numericFormat = "#,##0.000"      + percentSign + ";;" + zeroFormat; break;
         case 4: numericFormat = "#,##0.0000"     + percentSign + ";;" + zeroFormat; break;
         case 5: numericFormat = "#,##0.00000"    + percentSign + ";;" + zeroFormat; break;
         case 6: numericFormat = "#,##0.000000"   + percentSign + ";;" + zeroFormat; break;
         case 7: numericFormat = "#,##0.0000000"  + percentSign + ";;" + zeroFormat; break;
         case 8: numericFormat = "#,##0.00000000" + percentSign + ";;" + zeroFormat; break;

         default: ZXC.aim_emsg("GetDgvCellStyleFormat: broj decimala <{0}> still napodrzan!", numOfDecimalPlaces); break;
      }

      return numericFormat;
   }

   public static IFormatProvider GetDgvCellStyleFormatProvider(int numOfDecimalPlaces)
   {
      IFormatProvider nfi = null;
      switch(numOfDecimalPlaces)
      {
         case 0: nfi = ZXC.VvNumberFormatInfo0; break;
         case 1: nfi = ZXC.VvNumberFormatInfo1; break;
         case 2: nfi = ZXC.VvNumberFormatInfo2; break;
         case 3: nfi = ZXC.VvNumberFormatInfo3; break;
         case 4: nfi = ZXC.VvNumberFormatInfo4; break;
         case 5: nfi = ZXC.VvNumberFormatInfo5; break;
         case 6: nfi = ZXC.VvNumberFormatInfo6; break;
         case 7: nfi = ZXC.VvNumberFormatInfo7; break;
         case 8: nfi = ZXC.VvNumberFormatInfo8; break;

         default: ZXC.aim_emsg("GetDgvCellStyleFormatProvider: broj decimala <{0}> still napodrzan!", numOfDecimalPlaces); break;
      }

      return nfi;
   }

   public static string GetDgvCellStyleFormat_ZeroFillInt(int maxLength, bool clearIfZero)
   {
      string zeros, zeroFormat;

      if(clearIfZero) zeroFormat = "#";
      else            zeroFormat = "";

      zeros = "".PadLeft(maxLength, '0');

      return zeros + ";;" + zeroFormat;
   }

   public static int GetDGVsIdxCorrrector(DataGridView dgv)
   {
      if(dgv.AllowUserToAddRows == false) return 1;
      else                                return 2;
   }

   #endregion Common DataGridWiev Columns Formatting

   public virtual void OnCopyRecordPutSomeSpecificFields() { }

   public virtual void OpenCloseForWriting_AdditionalAction_UCspecific(ZXC.WriteMode writeMode, bool isESC) { }

   #region LoadSifarnik

   #region The SIFRARs Fieldz and Propertiez

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Kupdob> KupdobSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Kplan> KplanSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<User> UserSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Prjkt> PrjktSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Osred> OsredSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Person> PersonSifrar { get; set; }

   /// <summary>
   /// Lista svih recorda u datoteci, namijenjeno za LookUp ili Autocompletion operacije / izbore
   /// </summary>
   public static List<Artikl> ArtiklSifrar { get; set; }

   ///// <summary>
   ///// Lista svih OtsInfoa
   ///// </summary>
   //public List<ZXC.OtsTipBrGroupInfo> OtsInfoSifrar { get; set; }

   public bool isPopulatingSifrar = false;

   /// <summary>
   /// Namijenjen je cuvanju orginala za provjeru kod izlaska; da li je text changed
   /// </summary>
   public string originalText;

   protected string                     sifrarRecordName;
   /*protected*/public VvSQL.SorterType sifrarSorterType;
   protected ZXC.AutoCompleteRestrictor sifrarRestrictor = ZXC.AutoCompleteRestrictor.No_Restrictions;

   private   static DateTime         sifrarKplanLastLoaded  = DateTime.MinValue;
   private   static DateTime         sifrarKupdobLastLoaded = DateTime.MinValue;
   private   static DateTime         sifrarUserLastLoaded   = DateTime.MinValue;
   private   static DateTime         sifrarPrjktLastLoaded  = DateTime.MinValue;
   private   static DateTime         sifrarOsredLastLoaded  = DateTime.MinValue;
   private   static DateTime         sifrarPersonLastLoaded = DateTime.MinValue;
   /*private*/ internal   static DateTime         sifrarArtiklLastLoaded = DateTime.MinValue; // 12.09.2024 

   #endregion The SIFRARs Fieldz and Propertiez

   private string SifrarToString<T>(T sifrar_rec) where T : VvDataRecord
   {
      switch(sifrarRecordName)
      {
         case Kupdob.recordName: return Kupdob.ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Kplan .recordName: return Kplan .ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case User  .recordName: return User  .ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Prjkt .recordName: return Prjkt .ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Osred .recordName: return Osred .ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Person.recordName: return Person.ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Artikl.recordName: return Artikl.ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);
         case Xtrans.recordName: return Xtrans.ToSifrarString(sifrar_rec, sifrarSorterType, sifrarRestrictor);

         default: ZXC.aim_emsg("CCC: SetAutocomplete in VvUserControl doesn't yet support record {0}!", sifrarRecordName); return "che?";
      }
   }

   public static void NullifyAllSifrars()
   {
      VvUserControl.KupdobSifrar = null;
      VvUserControl.KplanSifrar  = null;
      VvUserControl.UserSifrar   = null;
      VvUserControl.PrjktSifrar  = null;
      VvUserControl.OsredSifrar  = null;
      VvUserControl.PersonSifrar = null;
      VvUserControl.ArtiklSifrar = null;

      sifrarKplanLastLoaded  = DateTime.MinValue;
      sifrarKupdobLastLoaded = DateTime.MinValue;
      sifrarUserLastLoaded   = DateTime.MinValue;
      sifrarPrjktLastLoaded  = DateTime.MinValue;
      sifrarOsredLastLoaded  = DateTime.MinValue;
      sifrarPersonLastLoaded = DateTime.MinValue;
      sifrarArtiklLastLoaded = DateTime.MinValue;

   }

   /// <summary>
   /// (Iskljucivo za VvDataRecord-e a ne za VvLookUpItem-e)
   /// Das tableName, das TextBox koji ce to koristiti...
   /// ili
   /// Nedas TextBox, znaci da smo ovdje upali iz nekog PutFields-a koji treba neki Naziv a ne iz vvTbEnter eventa. 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="textBox"></param>
   /// <param name="sifrarType"></param>
   public List<T> SetSifrarAndAutocomplete<T>(VvTextBox vvTB, VvSQL.SorterType _sifrarType) where T : VvDataRecord, new()
   {
      return SetSifrarAndAutocomplete<T>(vvTB, _sifrarType, false /* forceLoad */ );
   }

   /// <summary>
   /// (Iskljucivo za VvDataRecord-e a ne za VvLookUpItem-e)
   /// Das tableName, das TextBox koji ce to koristiti...
   /// ili
   /// Nedas TextBox, znaci da smo ovdje upali iz nekog PutFields-a koji treba neki Naziv a ne iz vvTbEnter eventa. 
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="textBox"></param>
   /// <param name="sifrarType"></param>
   public List<T> SetSifrarAndAutocomplete<T>(VvTextBox vvTB, VvSQL.SorterType _sifrarType, bool forceLoad) where T : VvDataRecord, new()
   {
      List<T>  sifrar;
      bool     shouldLoad     = false, OK = true;
      bool     isDataRequired = false;
      DateTime sifrarLastLoaded;

      sifrarRecordName = typeof(T).FullName.ToLower();
      sifrarSorterType       = _sifrarType;

      isPopulatingSifrar = true;

      if(typeof(T) == typeof(Artikl)) OK = true;
      if(vvTB != null)
      {
         // save original text 
         this.originalText = vvTB.Text;

         isDataRequired = vvTB.JAM_DataRequired; // backup 
         if(isDataRequired) vvTB.JAM_DataRequired = false; // temporarily suspend 
      }

      switch(sifrarRecordName)
      {
         case Kupdob.recordName: sifrar = (List<T>)(IList<T>)KupdobSifrar; sifrarLastLoaded = sifrarKupdobLastLoaded; break;
         case Kplan .recordName: sifrar = (List<T>)(IList<T>)KplanSifrar ; sifrarLastLoaded = sifrarKplanLastLoaded ; break;
         case User  .recordName: sifrar = (List<T>)(IList<T>)UserSifrar  ; sifrarLastLoaded = sifrarUserLastLoaded  ; break;
         case Prjkt .recordName: sifrar = (List<T>)(IList<T>)PrjktSifrar ; sifrarLastLoaded = sifrarPrjktLastLoaded ; break;
         case Osred .recordName: sifrar = (List<T>)(IList<T>)OsredSifrar ; sifrarLastLoaded = sifrarOsredLastLoaded ; break;
         case Person.recordName: sifrar = (List<T>)(IList<T>)PersonSifrar; sifrarLastLoaded = sifrarPersonLastLoaded; break;
         case Artikl.recordName: sifrar = (List<T>)(IList<T>)ArtiklSifrar; sifrarLastLoaded = sifrarArtiklLastLoaded; break;
         case Xtrans.recordName: sifrar = null;                            sifrarLastLoaded = DateTime.MinValue     ; break; // ! 

         default: ZXC.aim_emsg("AAA: SetAutocomplete in VvUserControl doesn't yet support record {0}!", sifrarRecordName); return null;
      }

      if(sifrar == null)
      {
         sifrar = new List<T>();
         shouldLoad = true;
      }
      else
      {
         // 02.05.2025: dodana forceLoad logika jer kod uzastopnih ADDREC-ova istoga addTS im je jednak pa ovaj SifrarNeedsRefreshing ne kuzi da ima novosti 
         if(forceLoad) shouldLoad = true;
         else          shouldLoad = SifrarNeedsRefreshing<T>(sifrar);
      }

      if(shouldLoad)
      {
         if(sifrar.Count > 0) sifrar.Clear();


         Cursor.Current = Cursors.WaitCursor;

         //bool localOK;

         // 25.11.2014:
         string origDatabase = ZXC.TheMainDbConnection.Database;
         ZXC.TheMainDbConnection.ChangeDatabase(VvSQL.GetDbNameForThisTableName(sifrarRecordName));

         // 15.04.2022: 
         //OK = VvDaoBase.LoadGenericVvDataRecordList<T>(ZXC.TheMainDbConnection, sifrar, null, "");
         
         if(sifrarRecordName == Xtrans.recordName)

         #region KDCxtrans_ByKupdobCD
         {
            vvTB.AutoCompleteCustomSource.Clear();

            List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

            DataRowCollection  XtrSch = ZXC.XtransDao.TheSchemaTable.Rows;
            XtransDao.XtransCI XtrCI  = ZXC.XtransDao.CI;

            UGNorAUN_PTG_DUC theDUC = this as UGNorAUN_PTG_DUC;

            filterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_tt          ], "theTT"    , Mixer.TT_KDC       , " = "));
            filterMembers.Add(new VvSqlFilterMember(XtrSch[XtrCI.t_ttNum       ], "theTTnum" , theDUC.Fld_KupdobCd, " = "));

            OK = VvDaoBase.LoadGenericVvDataRecordList<T>(ZXC.TheMainDbConnection, sifrar, filterMembers, "t_kpdbNameA_50");
         }

         #endregion KDCxtrans_ByKupdobCD

         else
         {
            OK = VvDaoBase.LoadGenericVvDataRecordList<T>(ZXC.TheMainDbConnection, sifrar, null, "");
         }

         ZXC.TheMainDbConnection.ChangeDatabase(origDatabase);

         //ZXC.aim_emsg("{0} Loaded.", sifrarRecordName);

         Cursor.Current = Cursors.Default;

         //if(!localOK)
         //{
         //   sifrar = null;
         //   isPopulatingSifrar = false;
         //   return;
         //}

         // 18.10.2017: 
         if(sifrarRecordName == User.recordName) ZXC.userSifrarLoaded = true; // userSifrar cemo ucitavati samo jednom! 

         //sifrarLastLoaded = DateTime.Now;
         sifrarLastLoaded = VvSQL.GetServer_DateTime_Now(TheDbConnection);

         switch(sifrarRecordName)
         {
            case Kupdob.recordName: KupdobSifrar = (List<Kupdob>)(IList<T>)sifrar; sifrarKupdobLastLoaded = sifrarLastLoaded; break;
            case Kplan .recordName: KplanSifrar  = (List<Kplan> )(IList<T>)sifrar; sifrarKplanLastLoaded  = sifrarLastLoaded; break;
            case User  .recordName: UserSifrar   = (List<User>  )(IList<T>)sifrar; sifrarUserLastLoaded   = sifrarLastLoaded; break;
            case Prjkt .recordName: PrjktSifrar  = (List<Prjkt> )(IList<T>)sifrar; sifrarPrjktLastLoaded  = sifrarLastLoaded; break;
            case Osred .recordName: OsredSifrar  = (List<Osred> )(IList<T>)sifrar; sifrarOsredLastLoaded  = sifrarLastLoaded; break;
            case Person.recordName: PersonSifrar = (List<Person>)(IList<T>)sifrar; sifrarPersonLastLoaded = sifrarLastLoaded; break;
            case Artikl.recordName: ArtiklSifrar = (List<Artikl>)(IList<T>)sifrar; sifrarArtiklLastLoaded = sifrarLastLoaded; break;
            case Xtrans.recordName:                                                                                         ; break;

            default: ZXC.aim_emsg("BBB: SetAutocomplete in VvUserControl doesn't yet support record {0}!", sifrarRecordName); return null;
         }

      } // if(shouldLoad)

      // ako je vvTB null, znaci da smo ovdje upali iz nekog PutFields-a koji treba neki Naziv a ne iz vvTbEnter eventa. 
      if(OK && vvTB != null && (shouldLoad || vvTB.AutoCompleteCustomSource.Count == 0))
      {
         /*@*/ this.sifrarRestrictor = vvTB.JAM_AutoCompleteRestrictor; // eventualni restrictor - filter za sifrar (npr. 'KID_Centrala_Only') 
                                                                        // koristiti ce ga SifrarToString(), dole...                           
         string[]     stringArray = new string[sifrar.Count];
         List<string> stringList  = sifrar.ConvertAll(new Converter<T, string>(SifrarToString));

         /*@*/ this.sifrarRestrictor = ZXC.AutoCompleteRestrictor.No_Restrictions; // default back 

         stringList.CopyTo(stringArray);
         vvTB.AutoCompleteCustomSource.Clear();
         vvTB.AutoCompleteCustomSource.AddRange(stringArray);
         //ZXC.aim_emsg("{0} refreshed", textBox.ToString());
      }

      if(vvTB != null && isDataRequired) vvTB.JAM_DataRequired = true; // restore previously temporarily suspended 

      isPopulatingSifrar = false;

      return sifrar;
   }

   public bool FoundInSifrar<T>(T sifrar_rec) where T : VvDataRecord
   {
      // 8.6.2011: zbog povremene pojave Exceptiona pri otvaranju novog TabPage-a a dok je stari TP u žutome writeModu...
      //if(typeof(T).ToString().ToLower() != sifrarRecordName) /*sifrarRecordName = typeof(T).ToString().ToLower();*/ return false;

      switch(sifrarRecordName)
      {
         case Kupdob.recordName:
              Kupdob kupdob_rec = (Kupdob)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               // 10.08.2024: Buon compleano! Nono Lorenzo :-) 
               // Autocomplete duplicates pokusaj rjesenja     
               case VvSQL.SorterType.Name:
                  string cleanNaziv, theTicker;
                  int visakIdx = originalText.IndexOf(Kupdob.TickerToken);
                  if(originalText.Length.NotZero() && visakIdx.IsZeroOrPositive()) //return;
                  {
                     cleanNaziv = originalText.Substring(0, visakIdx);
                     theTicker  = originalText.Substring(visakIdx + Kupdob.TickerToken.Length);
                     //return String.Compare(/*this.originalText*/cleanNaziv, kupdob_rec.Naziv, true) == 0;
                     return String.Compare(theTicker, kupdob_rec.NazivUniqueAddition, true) == 0;
                  }
                  else
                  {
                     cleanNaziv = originalText;
                     return String.Compare(/*this.originalText*/cleanNaziv, kupdob_rec.Naziv, true) == 0;
                  }

             //case VvSQL.SorterType.Name:   return String.Compare(this.originalText, kupdob_rec.Naziv,   true) == 0;
               case VvSQL.SorterType.Ticker: return String.Compare(this.originalText, kupdob_rec.Ticker,  true) == 0;
             //case VvSQL.SorterType.RecID:  return (ZXC.ValOrZero_UInt(originalText) == artikl_rec.RecID);
               case VvSQL.SorterType.Code:   return (ZXC.ValOrZero_UInt(originalText) == kupdob_rec.KupdobCD/*RecID*/);
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case Prjkt.recordName:
              Prjkt prjkt_rec = (Prjkt)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               case VvSQL.SorterType.Name:   return String.Compare(this.originalText, prjkt_rec.Naziv,   true) == 0;
               case VvSQL.SorterType.Ticker: return String.Compare(this.originalText, prjkt_rec.Ticker,  true) == 0;
               //case VvSQL.SorterType.RecID:  return (ZXC.ValOrZero_UInt(originalText) == prjkt_rec.RecID);
               case VvSQL.SorterType.Code:   return (ZXC.ValOrZero_UInt(originalText) == prjkt_rec.KupdobCD/*RecID*/);
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case Kplan.recordName:
              Kplan kplan_rec = (Kplan)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               case VvSQL.SorterType.Name: return String.Compare(this.originalText,   kplan_rec.Naziv, true) == 0;
               case VvSQL.SorterType.Konto/*Code*/: return               (this.originalText == kplan_rec.Konto);
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case User.recordName:
              User user_rec = (User)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               case VvSQL.SorterType.Person: return String.Compare(this.originalText, user_rec.Prezime,  true) == 0;
               case VvSQL.SorterType.Code:   return String.Compare(this.originalText, user_rec.UserName, true) == 0;
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case Osred.recordName:
              Osred osred_rec = (Osred)(VvDataRecord)sifrar_rec;
            switch (sifrarSorterType)
            {
               case VvSQL.SorterType.Name: return String.Compare(this.originalText, osred_rec.Naziv  , true) == 0;
               case VvSQL.SorterType.Code: return               (this.originalText == osred_rec.OsredCD);
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
        
         case Person.recordName:
              Person person_rec = (Person)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               // 13.02.2020: HZTK... 
             //case VvSQL.SorterType.Person: return String.Compare(this.originalText, person_rec.Prezime, true) == 0;
               case VvSQL.SorterType.Person: return String.Compare(this.originalText, person_rec.PrezimeIme, true) == 0;
               case VvSQL.SorterType.Code  : return (ZXC.ValOrZero_UInt(originalText) == person_rec.PersonCD);
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case Artikl.recordName:
              Artikl artikl_rec = (Artikl)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               case VvSQL.SorterType.Name   : return String.Compare(this.originalText, artikl_rec.ArtiklName, true) == 0;
               case VvSQL.SorterType.Code   : return String.Compare(this.originalText, artikl_rec.ArtiklCD  , true) == 0;
               case VvSQL.SorterType.BarCode: return String.Compare(this.originalText, artikl_rec.BarCode1  , true) == 0;
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }
         case Rtrans.recordName:
              Rtrans rtrans_rec = (Rtrans)(VvDataRecord)sifrar_rec;
            switch(sifrarSorterType)
            {
               case VvSQL.SorterType.Name    : return String.Compare             (this.originalText,    rtrans_rec.T_artiklName, true) == 0;
               case VvSQL.SorterType.Code    : return String.Compare             (this.originalText,    rtrans_rec.T_artiklCD  , true) == 0;
               case VvSQL.SorterType.TtNum   : return ZXC.ValOrZero_UInt         (this.originalText) == rtrans_rec.T_ttNum                 ;
               case VvSQL.SorterType.KpdbName: return ZXC.ValOrZero_UInt         (this.originalText) == rtrans_rec.T_kupdobCD              ;
               case VvSQL.SorterType.Serlot  : return String.Compare             (this.originalText,    rtrans_rec.T_serlot    , true) == 0;
               case VvSQL.SorterType.DokDate : return ZXC.ValOr_01010001_DateTime(this.originalText) == rtrans_rec.T_skladDate             ;
               default: ZXC.aim_emsg("d1: SetAutocomplete in VvUserControl doesn't yet support sifrarType {0}!", sifrarSorterType.ToString()); return false;
            }

         default: ZXC.aim_emsg("DDD: SetAutocomplete in VvUserControl doesn't yet support record {0}!", sifrarRecordName); return false;
      }
   }

   private bool SifrarNeedsRefreshing<T>(IList<T> sifrar) where T : VvDataRecord
   {
      DateTime sifrarLastChanged;
      bool yes_no = false;

      // 18.10.2017: 
      if(sifrarRecordName == User.recordName && ZXC.userSifrarLoaded == true) return false;

      sifrarLastChanged = VvDaoBase.GetTableLastChangeTimestamp(sifrarRecordName);

      switch(sifrarRecordName)
      {
         case Kupdob.recordName: yes_no = sifrarKupdobLastLoaded < sifrarLastChanged; break;
         case Kplan .recordName: yes_no = sifrarKplanLastLoaded  < sifrarLastChanged; break;
         case User  .recordName: yes_no = sifrarUserLastLoaded   < sifrarLastChanged; break;
         case Prjkt .recordName: yes_no = sifrarPrjktLastLoaded  < sifrarLastChanged; break;
         case Osred .recordName: yes_no = sifrarOsredLastLoaded  < sifrarLastChanged; break;
         case Person.recordName: yes_no = sifrarPersonLastLoaded < sifrarLastChanged; break;
         case Artikl.recordName: yes_no = sifrarArtiklLastLoaded < sifrarLastChanged; break;

         default: ZXC.aim_emsg("A43: SetAutocomplete in VvUserControl doesn't yet support record {0}!", sifrarRecordName); break;
      }
      
      return(yes_no);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na: NULL
   /// A trazi i sortiraj po Tt + TtNum
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Faktur_DUMMY(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //SetSifrarAndAutocomplete<Kplan>((VvTextBox)sender, VvSQL.SorterType.Konto/*Code*/);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Kplan data file, trazi i sortiraj po Kontu
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Kplan_sorterCode(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Kplan>((VvTextBox)sender, VvSQL.SorterType.Konto/*Code*/);
   }

   public void OnVvTBEnter_SetAutocmplt_KplanNaziv_sorterCode(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Kplan>((VvTextBox)sender, VvSQL.SorterType.KontoNaziv);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Kupdob data file, trazi i sortiraj po Nazivu kupDob-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Kupdob_sorterName(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Kupdob>((VvTextBox)sender, VvSQL.SorterType.Name);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Kupdob data file, trazi i sortiraj po Sifri kupDob-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Kupdob>((VvTextBox)sender, VvSQL.SorterType.Code/*RecID*/);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Kupdob data file, trazi i sortiraj po Ticker-u kupDob-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Kupdob>((VvTextBox)sender, VvSQL.SorterType.Ticker);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// User data file, trazi i sortiraj po Username (Code-u) user-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_User_sorterUserName(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<User>((VvTextBox)sender, VvSQL.SorterType.Code);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Prjkt data file, trazi i sortiraj po Sifri (prjktKupdobCD) prjkta-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Prjkt>((VvTextBox)sender, VvSQL.SorterType.Code/*RecID*/);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Prjkt data file, trazi i sortiraj po Ticker-u prjkta-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Prjkt>((VvTextBox)sender, VvSQL.SorterType.Ticker);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Prjkt data file, trazi i sortiraj po nazivu prjkta-a
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Prjkt>((VvTextBox)sender, VvSQL.SorterType.Name);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Osred data file, trazi i sortiraj po Nazivu
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Osred_sorterName(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Osred>((VvTextBox)sender, VvSQL.SorterType.Name);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Osred data file, trazi i sortiraj po OsredCD
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Osred_sorterSifra(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Osred>((VvTextBox)sender, VvSQL.SorterType.Code);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Person data file, trazi i sortiraj po personCD
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Person_sorterSifra(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Person>((VvTextBox)sender, VvSQL.SorterType.Code);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Person data file, trazi i sortiraj po PersonPrezimenu
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Person_sorterPrezime(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Person>((VvTextBox)sender, VvSQL.SorterType.Person);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Artikl data file, trazi i sortiraj po Nazivu
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Artikl_sorterName(object sender, EventArgs e)
   {
      if (isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Artikl>((VvTextBox)sender, VvSQL.SorterType.Name);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Artikl data file, trazi i sortiraj po ArtiklCD
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Artikl_sorterSifra(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Artikl>((VvTextBox)sender, VvSQL.SorterType.Code);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Artikl data file, trazi i sortiraj po BarCode1
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Artikl_sorterBCode(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Artikl>((VvTextBox)sender, VvSQL.SorterType.BarCode);
   }

   /// <summary>
   /// OnEnter u ovi VvTextBox, postavi AutoCompleteSource na:
   /// Kplan data file, trazi i sortiraj po Kontu
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   public void OnVvTBEnter_SetAutocmplt_Xtrans_sorterKCDnaziv(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      SetSifrarAndAutocomplete<Xtrans>((VvTextBox)sender, VvSQL.SorterType.KCDnaziv);
   }


   // From 30.11.2011: use this in future! 
   public Kupdob Get_Kupdob_FromVvUcSifrar(uint kupdobCD)
   {
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      
      Kupdob kupdob_rec = KupdobSifrar.SingleOrDefault(k => k.KupdobCD == kupdobCD);

      if(kupdob_rec == null) return null;

      return kupdob_rec.MakeDeepCopy();
   }

   public Kupdob Get_Kupdob_FromVvUcSifrar_byTicker(string _ticker)
   {
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Ticker);

      Kupdob kupdob_rec = KupdobSifrar.SingleOrDefault(k => k.Ticker == _ticker);

      if(kupdob_rec == null) return null;

      return kupdob_rec.MakeDeepCopy();
   }

   public Kupdob Get_Kupdob_FromVvUcSifrar_byIBAN(string kupdobIBAN)
   {
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      Kupdob kupdob_rec = null;

      try
      {
         kupdob_rec = KupdobSifrar.SingleOrDefault(k => k.Ziro1_asIBAN == kupdobIBAN);
      }
      catch (System.InvalidOperationException ex)
      {
         if(ex.Message.Contains("more than one"))
         {
            if(ZXC.IsTEXTHOany || kupdobIBAN != VvPlacaReport.standardGovernmentZiroRacun)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Vise od jednog partnera sa IBAN-om {0}", kupdobIBAN);
            }

            kupdob_rec = KupdobSifrar.FirstOrDefault(k => k.Ziro1_asIBAN == kupdobIBAN);

         }
         else return null;
      }

      if(kupdob_rec == null) return null;

      return kupdob_rec.MakeDeepCopy();
   }

   public Kupdob Get_Kupdob_FromVvUcSifrar(string nameRoot)
   {
      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      Kupdob kupdob_rec = KupdobSifrar.FirstOrDefault(k => k.Naziv.ToLower().StartsWith(nameRoot.ToLower()));

      if(kupdob_rec == null) return null;

      return kupdob_rec.MakeDeepCopy();
   }

   public Kupdob Get_Kupdob_FromVvUcSifrar_byOIB(string theOib)
   {
      // 09.02.2026: 
      if(theOib.IsEmpty()) return null;

      SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);

      Kupdob kupdob_rec = KupdobSifrar.FirstOrDefault(k => k.Oib == theOib);

      if(kupdob_rec == null) return null;

      return kupdob_rec.MakeDeepCopy();
   }

   public Artikl Get_Artikl_FromVvUcSifrar(string artiklCD)
   {
      // 12.03.2024: 
      if(artiklCD.IsEmpty()) return null;

      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD.ToUpper() == artiklCD.ToUpper());

      if(artikl_rec == null) return null;

      return artikl_rec.MakeDeepCopy();
   }

   // HRD_2023: 
   public Artikl Get_Artikl_FromVvUcSifrar(decimal niceEURcij)
   {
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(a => a.ImportCij == niceEURcij);

      if(artikl_rec == null) return null;

      return artikl_rec.MakeDeepCopy();
   }

   public Artikl Get_Artikl_FromVvUcSifrarBC1(string barCode1)
   {
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(a => a.BarCode1.ToUpper() == barCode1.ToUpper());

      if(artikl_rec == null) return null;

      return artikl_rec.MakeDeepCopy();
   }

   public Artikl Get_Artikl_FromVvUcSifrarByName(string artiklName)
   {
      SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      Artikl artikl_rec = ArtiklSifrar.SingleOrDefault(a => a.ArtiklName.ToUpper() == artiklName.ToUpper());

      if(artikl_rec == null) return null;

      return artikl_rec.MakeDeepCopy();
   }

   public Person Get_Person_FromVvUcSifrar(uint personCD)
   {
      SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Name);

      Person person_rec = PersonSifrar.SingleOrDefault(a => a.PersonCD == personCD);

      if(person_rec == null) return null;

      return person_rec.MakeDeepCopy();
   }

   public User Get_User_FromVvUcSifrar(string userName)
   {
      SetSifrarAndAutocomplete<User>(null, VvSQL.SorterType.Code);

      User user_rec = UserSifrar.SingleOrDefault(a => a.UserName == userName);

      if(user_rec == null) return null;

      return user_rec.MakeDeepCopy();
   }

   public Kplan Get_Kplan_FromVvUcSifrar(string konto)
   {
      SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.Konto);

      Kplan Kplan_rec = KplanSifrar.SingleOrDefault(a => a.Konto == konto);

      if(Kplan_rec == null) return null;

      return Kplan_rec.MakeDeepCopy();
   }

   #endregion LoadSifarnik

   #region UpdateVvDataRecord

   //public static object UpdateVvDataRecord(string tableName, object startValue)
   //{
   //   return UpdateVvDataRecord(tableName, VvSQL.SorterType_Dokument.None, startValue);
   //}

   public static List<OtsTipBrGroupInfo> UpdateOtsInfo(VvTextBox vvTextBox)
   {
      if(vvTextBox.OtsKonto.IsEmpty() || vvTextBox.OtsKupdobCd.IsZero())
      {
         VvSQL.ReportGenericError("Odabir otvorene stavke", string.Format("Ne mogu pretrazivati otvorene stavke za konto [{0}] / partner RecID [{1}]!", 
            vvTextBox.OtsKonto, vvTextBox.OtsKupdobCd), System.Windows.Forms.MessageBoxButtons.OK);
         return null;
      }

      SelectOTSdlg dlg = new SelectOTSdlg(vvTextBox);
      
      DialogResult result = dlg.ShowDialog();

      if(result != DialogResult.OK) { dlg.Dispose(); return null; }

      VvDataGridView dgv           = dlg.TheUC.TheGrid;
      BindingSource  bindingSource = dlg.TheUC.TheBindingSource;

      Dictionary<int, bool> theCheckState = dlg.TheUC.TheCheckState;

      List<OtsTipBrGroupInfo> choosenOtsList = new List<OtsTipBrGroupInfo>();
      OtsTipBrGroupInfo       otsInfo;

      decimal MoneyFond       = dlg.TheUC.Fld_FndUkupno;
      decimal MoneySpendSoFar = 0.00M;
      decimal MoneyLeft;

      foreach(DataGridViewRow row in dgv.Rows)
      {
         //if((bool)(row.Cells[0].Value) == true) // CheckBox is checked 
         if(theCheckState.ContainsKey(row.Index) && theCheckState[row.Index] == true) // CheckBox is checked 
         {
            otsInfo = (OtsTipBrGroupInfo)bindingSource[row.Index];

            MoneySpendSoFar += otsInfo.UkSaldo;

            MoneyLeft = MoneyFond - MoneySpendSoFar;

            if(MoneyFond.NotZero() && MoneyLeft < 0.00M)
            {
               string massaze = String.Format("Zatvaranjem racuna {0} idemo u minus za {1}.\n\nDa li da umanjim zatvaranje pa idemo na nulu?", otsInfo.TipBr, MoneyLeft);

               if(VvSQL.ReportGenericError("Fond uplate premašen", massaze, System.Windows.Forms.MessageBoxButtons.YesNo) == true)
               {
                  MoneySpendSoFar += MoneyLeft; // MoneyLeft je negativan 
                  otsInfo.UkSaldo += MoneyLeft;
                  MoneyLeft        = 0.00M;
               }
            }

            choosenOtsList.Add(otsInfo);
         }
      }

      dlg.Dispose();

      return choosenOtsList;
   }

   public static object UpdateVvDataRecord(string recordName, VvSQL.SorterType whichInformation, ZXC.AutoCompleteRestrictor sifrarRestrictor, object startValue, object sender)
   {
      object findResult = null;

      // ovo kontrolira da ako je VvFindDialog.SelectionIsNewlyAddedRecord == true, onda treba forsati refreshing AutoCompleta... 
      ZXC.ShouldForceSifrarRefreshing = false;

      switch(recordName)
      {
         case Kupdob.recordName:
            
            if(whichInformation == VvSQL.SorterType.Name) startValue = Kupdob.GetCleanKupdobNameFromTokenized((string)startValue);

            findResult = KupdobUC.Update_Kupdob(whichInformation, startValue, sifrarRestrictor); 
            break;

         case Kplan .recordName: findResult = KplanUC .Update_Kplan (                  startValue);                   break;
         case User  .recordName: findResult = UserUC  .Update_User  (                  startValue);                   break;
         case Prjkt .recordName: findResult = PrjktUC .Update_Prjkt (whichInformation, startValue);                   break;
         case Osred .recordName: findResult = OsredUC .Update_Osred (whichInformation, startValue);                   break;
         case Person.recordName: findResult = PersonUC.Update_Person(whichInformation, startValue, sifrarRestrictor); break;
         case Artikl.recordName: findResult = ArtiklUC.Update_Artikl(whichInformation, startValue, sifrarRestrictor); break;

         case Faktur.recordName: findResult = FakturDUC.Update_Faktur_4_ProjektCD(uint.MaxValue); break;

         case Xtrans.recordName: findResult = UGNorAUN_PTG_DUC.Update_KdcXtrans(startValue, sender); break;

         default: ZXC.aim_emsg("Record [{0}] still undone in VvUserControl.UpdateVvDataRecord() method!", recordName); break;
      }

      return findResult;
   }

   public static bool TriggerKey_ForUpdateVvDataRecord(KeyEventArgs e)
   {
      if((e.Control || e.Alt) && (e.KeyCode == Keys.F || e.KeyCode == Keys.Space)) 
         return true;
      else 
         return false;
   }

   #endregion UpdateVvDataRecord

   #region UpdateVvLookUpItem

   public static VvLookUpItem UpdateVvLookUpItem(VvTextBox vvtb, out List<VvLookUpItem> selectedItems)
   {
      VvLookUpItem               lui;
      LookUpItem_ListView_Dialog dlg = null;

      dlg = ZXC.TheVvForm.Create_LookUpItem_ListView_Dialog_ProjectDependent(vvtb);

      if(dlg == null) 
      { 
         selectedItems = null; 
         return null; 
      }

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         lui           = dlg.SelectedItem;
         selectedItems = dlg.SelectedItemsList;
      }
      else
      {
         lui = null;
         selectedItems = null;
      }

      if(dlg.ShouldRefreshCallerLookupList)
      {
         vvtb.JAM_Set_LookUpTable(dlg.LookUpList /*ZXC.luiListaNalogTT*/, /*(int)ZXC.Kolona.druga*/ vvtb.JAM_lookUpTableColIdx);
      }

      dlg.Dispose();

      return lui;
   }

   public static /*void*/VvLookUpItem BtnChangeLookUpSelection_Click(object sender, EventArgs e)
   {
      VvTextBox vvtb;
      List<VvLookUpItem> selectedItems;
      string lookUpItemText = "";

      if(sender is VvTextBox)
         vvtb = sender as VvTextBox;
      else
         vvtb = ((Control)sender).Parent as VvTextBox;

      VvLookUpItem lui = VvUserControl.UpdateVvLookUpItem(vvtb, out selectedItems);

      vvtb.JAM_ChosenLookUpItem = null;

      if(lui != null)
      {
         //if(selectedItems.Count > 1 && vvtb.Multiline == true)
         if(selectedItems.Count > 1 && vvtb.JAM_lookUp_MultiSelection == true)
         {
            foreach(VvLookUpItem luiInList in selectedItems)
            {
               lookUpItemText = GetLookUpItemText(luiInList, vvtb.JAM_lookUpTableColIdx);

               vvtb.Text += lookUpItemText + (vvtb.Multiline ? "\r\n" : "");
            }
         }
         else
         {
            lookUpItemText = GetLookUpItemText(lui, vvtb.JAM_lookUpTableColIdx);

            vvtb.JAM_ChosenLookUpItem = lui;

            //if(vvtb.Multiline == true)
            if(vvtb.JAM_lookUp_MultiSelection == true)
               vvtb.Text += lookUpItemText;
            else
               vvtb.Text  = lookUpItemText;
         }

         //SetEventualLookupDataTakers(vvtb); preseljeno 

         SendKeys.Send("{TAB}");

      } // if(lui != null) 

      //return lookUpItemText;

      // 23.02.2016: 
      return lui;
   }

   public static string GetLookUpItemText(VvLookUpItem lui, uint colIdx)
   {
      // 02.06.2009: 
      //if(eventualNameTaker != null)
      //{
      //   return lui.Cd;
      //}
      //else
      //{
      //   return lui.Name;
      //}

      switch(colIdx)
      {
         // string Name 
         case (uint)ZXC.Kolona.prva:    return lui.Cd;
         // string Cd 
         case (uint)ZXC.Kolona.druga:   return lui.Name;
         // double Number 
         case (uint)ZXC.Kolona.treća:   return lui.Number.ToString(ZXC.GetNumberFormatInfo(2));
         // bool Flag 
         case (uint)ZXC.Kolona.četvrta: return lui.Flag.ToString();
         // int Integer 
         case (uint)ZXC.Kolona.peta:    return lui.Integer.ToString();
         // DateTime DateT 
         case (int)ZXC.Kolona.šesta:    return lui.DateT.ToString(ZXC.VvDateFormat);

         default: ZXC.aim_emsg("GetLookUpItemText(): ColIdx <{0}> unkonow!", colIdx); return "";
      }
   }

   #endregion UpdateVvLookUpItem

   #region AddDGV_Column_4GridReadOnly

   // news: 22.10.2010 DataGridViewFind ---> VvDataGridView
   protected VvDataGridViewFind CreateDataGridView_ReadOnly(Control _parent, string _name)
   {
      VvDataGridViewFind grid_ReadOnly         = new VvDataGridViewFind();
      grid_ReadOnly.Parent                   = _parent;
      grid_ReadOnly.Name                     = _name;
      grid_ReadOnly.Location                 = new Point(ZXC.QunMrgn, ZXC.QunMrgn);
      grid_ReadOnly.AutoGenerateColumns      = false;
      grid_ReadOnly.AllowUserToAddRows       =
      grid_ReadOnly.AllowUserToResizeRows    =
      grid_ReadOnly.AllowUserToDeleteRows    = false;
      grid_ReadOnly.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      grid_ReadOnly.ReadOnly                 = true;
      grid_ReadOnly.Tag                      = "ROnly";
      grid_ReadOnly.RowHeadersWidth          = ZXC.Q3un + ZXC.Qun4;
      grid_ReadOnly.AllowDrop                = false;
      grid_ReadOnly.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      grid_ReadOnly.RowHeadersBorderStyle                = DataGridViewHeaderBorderStyle.Single;


      if(_parent is VvRecLstUC)
      {
         grid_ReadOnly.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


         grid_ReadOnly.DefaultCellStyle.SelectionBackColor =
         grid_ReadOnly.RowHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gold;

         grid_ReadOnly.DefaultCellStyle.SelectionForeColor =
         grid_ReadOnly.RowHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.Highlight;
      }
      else
      {
         // micanje tamnoplavog polja iz datagrida 
         //
         grid_ReadOnly.TabStop = false;
         grid_ReadOnly.ClearSelection();
         //
         //                                         
      }

      grid_ReadOnly.BackgroundColor     = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
      grid_ReadOnly.ColumnHeadersHeight = ZXC.QUN;
      grid_ReadOnly.RowTemplate.Height  = ZXC.QUN;

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(grid_ReadOnly);
      VvHamper.Open_Close_Fields_ForWriting(grid_ReadOnly, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvRecordUC);

      grid_ReadOnly.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_ReadOnly_CellFormatting);
      return grid_ReadOnly;
   }

   static void grid_ReadOnly_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
   {
      DataGridView dgv = sender as DataGridView;
      DataGridViewColumn column = dgv.Columns[e.ColumnIndex];

      if(column.ValueType == typeof(DateTime) && e.Value != null)
      {
         DateTime dateTime = ZXC.ValOr_01010001_DateTime(e.Value.ToString());

         if(dateTime == DateTime.MinValue)
         {
            e.CellStyle.Format = "' '";
         }
         else
         {
            // Dakle, ovo je trik kako ergulirati oces li samo datum ili i vrijeme (za npr. TimeStamp' podatak) 
            if(column.Width < ZXC.Q5un)
               e.CellStyle.Format = ZXC.VvDateFormat;
            else
            {
               e.CellStyle.Format = "G";
            }
         }
      }
   }

   protected DataGridViewTextBoxColumn AddDGVColum_String_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool _isAutoSizeModeFill)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(string), _grid, _headerText, _width, _isAutoSizeModeFill, true, 0, false, 0, "");
   }
   protected DataGridViewTextBoxColumn AddDGVColum_String_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool _isAutoSizeModeFill, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(string), _grid, _headerText, _width, _isAutoSizeModeFill, true, 0, false, 0, dataPropertyame);
   }

   protected DataGridViewTextBoxColumn AddDGVColum_DateTime_4GridReadOnly(DataGridView _grid, string _headerText, int _width)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(DateTime), _grid, _headerText, _width, false, true, 0, false, 0, "");
   }

   protected DataGridViewTextBoxColumn AddDGVColum_DateTime_4GridReadOnly(DataGridView _grid, string _headerText, int _width, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(DateTime), _grid, _headerText, _width, false, true, 0, false, 0, dataPropertyame);
   }

   protected DataGridViewTextBoxColumn AddDGVColum_Integer_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool oce_nece_leadingZero, int _numOfCharacter)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(uint), _grid, _headerText, _width, false, true, 0, oce_nece_leadingZero, _numOfCharacter, "");
   }
   protected DataGridViewTextBoxColumn AddDGVColum_Integer_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool oce_nece_leadingZero, int _numOfCharacter, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(uint), _grid, _headerText, _width, false, true, 0, oce_nece_leadingZero, _numOfCharacter, dataPropertyame);
   }

   protected DataGridViewTextBoxColumn AddDGVColum_RecID_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool oce_nece_leadingZero, int _numOfCharacter)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(uint), _grid, _headerText, _width, false, false, 0, oce_nece_leadingZero, _numOfCharacter, "");
   }
   protected DataGridViewTextBoxColumn AddDGVColum_RecID_4GridReadOnly(DataGridView _grid, string _headerText, int _width, bool oce_nece_leadingZero, int _numOfCharacter, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(uint), _grid, _headerText, _width, false, false, 0, oce_nece_leadingZero, _numOfCharacter, dataPropertyame);
   }
   protected DataGridViewTextBoxColumn AddDGVColum_RecID_4GridReadOnly_Visible(DataGridView _grid, string _headerText, int _width, bool oce_nece_leadingZero, int _numOfCharacter, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(uint), _grid, _headerText, _width, false, true, 0, oce_nece_leadingZero, _numOfCharacter, dataPropertyame);
   }

   protected DataGridViewTextBoxColumn AddDGVColum_Decimal_4GridReadOnly(DataGridView _grid, string _headerText, int _width, int numberOfDecimalPlaces)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(decimal), _grid, _headerText, _width, false, true, numberOfDecimalPlaces, false, 0, "");
   }
   protected DataGridViewTextBoxColumn AddDGVColum_Decimal_4GridReadOnly(DataGridView _grid, string _headerText, int _width, int numberOfDecimalPlaces, string dataPropertyame)
   {
      return CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(typeof(decimal), _grid, _headerText, _width, false, true, numberOfDecimalPlaces, false, 0, dataPropertyame);
   }

      
   private DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn_JOB_4GridReadOnly(Type valueType, DataGridView grid, string headerText, int width, bool isAutoSizeModeFill, bool isVisible, int numberOfDecimalPlaces, bool oce_nece_leadingZero, int numOfCharacter, string dataPropertyame)
   {
      DataGridViewTextBoxColumn colText = new DataGridViewTextBoxColumn();
      colText.Name             = headerText;
      colText.HeaderText       = headerText;
      colText.Width            = width;
      colText.ReadOnly         = true;
      colText.ValueType        = valueType;
      colText.Visible          = isVisible;
      colText.DataPropertyName = dataPropertyame;

      if(dataPropertyame != "") colText.Name = dataPropertyame;
      
      if(isAutoSizeModeFill)
      { 
         colText.MinimumWidth = width;
         colText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      }

      //if(valueType == typeof(DateTime))
      //{
      //   // NE, NE! Ovdje je jaci-kvaci 'grid_ReadOnly_CellFormatting'
      //   if(width < ZXC.Q5un)
      //      colText.DefaultCellStyle.Format = "G"/*ZXC.VvDateFormat*/;
      //   else
      //   {
      //      colText.DefaultCellStyle.Format = "G";
      //      colText.DefaultCellStyle.FormatProvider = new DateTimeFormatInfo();
      //   }
      //}

    //if(valueType == typeof(decimal))
      if(colText.ValueType == typeof(decimal))
      {
         colText.DefaultCellStyle.Alignment      =
         colText.HeaderCell.Style.Alignment      = DataGridViewContentAlignment.MiddleRight;
         colText.DefaultCellStyle.Format         = GetDgvCellStyleFormat_Number (numberOfDecimalPlaces, true);
         colText.DefaultCellStyle.FormatProvider = GetDgvCellStyleFormatProvider(numberOfDecimalPlaces);
       //26.10.2021. ipaak smo maknuli jer SvDuh hoce sortirati po kolicini pa onda stavljaj pojedinacno tamop gdje ne zelis da se sortira decimalna kolona
       //colText.SortMode                        = DataGridViewColumnSortMode.NotSortable; // ovo je da makne onaj trokutic za sort !!!!! zato headeri decimala nisu bili skroz desno
      }

      if(valueType == typeof(uint))
      {
         colText.DefaultCellStyle.Alignment =
         colText.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
      }
      
      if(oce_nece_leadingZero)
         colText.DefaultCellStyle.Format = GetDgvCellStyleFormat_ZeroFillInt(numOfCharacter, true);

      grid.Columns.Add(colText);

      return colText;

   }
   // GridOnSIfrar je dockiran fill od kad smo stavili filter - al neko ovo ostane za svaki slucaj ak se ukaze potreba
   protected void GridOnSIfrar_SizeAnchorAndScroll(DataGridView grid, int minGridWIdth)
   {
      //TheG.Size   = new Size(TheG.Parent.Width - 2 * ZXC.QunMrgn, TheG.Parent.Height - 2 * ZXC.QunMrgn);
      //TheG.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      grid.Dock = DockStyle.Fill;
      //VvHamper.Create_Label4PrisilniScrollZbogGrida(TheG, minGridWIdth);
   }

   protected DataGridViewCheckBoxColumn AddDGV_CheckBoxColumn_4GridReadOnly(DataGridView grid, string headerText, int colWidth, string dataPropertyame)
   {
      DataGridViewCheckBoxColumn colCbox = new DataGridViewCheckBoxColumn();
      colCbox.DataPropertyName           = dataPropertyame;
      colCbox.Name                       = dataPropertyame;
      colCbox.HeaderText                 = headerText;
      colCbox.Width                      = colWidth;
      colCbox.ReadOnly                   = true;

      grid.Columns.Add(colCbox);

      return colCbox;
   }

   #endregion AddDGV_Column_4GridReadOnly

   #region Util Methods

   public static void PutSaldoKontaFields(decimal saldo, VvTextBox tbDug, VvTextBox tbPot)
   {
      tbDug.PutDecimalField(saldo > decimal.Zero ? saldo : decimal.Zero);
      tbPot.PutDecimalField(saldo < decimal.Zero ? saldo * -1.00M : decimal.Zero);
   }

   public static IVvPrintableUC GetIVvPrintableUC(Control control)
   {
      foreach(Control childControl in control.Controls)
      {
         if(childControl is IVvPrintableUC) return (IVvPrintableUC)childControl;
      }

      return null;
   }

   internal bool CtrlOK(Control control)
   {
      // control.Parent = hamper a hamper.Parent = null dok mu se neda parent na pojedinom DUC-u
      return (control != null && control.Parent.Parent != null);
   }

   public virtual void SetWarningColorsAndLabel() { }

   public bool NeedsProizvodCIJ
   {
      get
      {
         return (
               (this is FakturDUC == true ) &&
               (this is RNMDUC    == false) && 
               (((FakturDUC)(this)).faktur_rec.TtInfo.HasSplitTT || this is PIPDUC)
            );
      }
   }

   #endregion Util Methods

   public bool IsHRDkontra { get; set; } = false;

   public virtual bool IsM2PAY_DUC { get { return (false); } } // IRM, IRM2, ... overrides as true 

}

public abstract class VvFilterUC                 : UserControl
{
   public int MaxHamperWidth { get; set; }

   public int razmakIzmjedjuHampera = ZXC.Qun2 - ZXC.Qun5;
   public int nextX                 = ZXC.Qun2 - ZXC.Qun4;
   public int nextY                 = ZXC.Qun2 - ZXC.Qun4;
   public int razmakHampGroup       = ZXC.Qun10;

   public  VvHamper hamper4buttons, hamperHorLine, hamperPrintDoc, hamperPrintZoom;
   public  Button   btn_Reset;
   public  Button   btn_GO;
   public  CheckBox cb_Line, cb_zaExport, cb_isHrdKontra;

   public VvDateTimePicker dtp_DatumOD, dtp_DatumDO, dtp_dateIzvj;
   public VvTextBox        tbx_DatumOD, tbx_DatumDO, tbx_dateIzvj;
   
  // public RadioButton   rbt_PgW, rbt_WhP, rbt_100;
   
   public VvFilterUC()
   {
      this.SuspendLayout();

      CreateHamper_4ButtonsResetGo_Height();
      nextY = hamper4buttons.Bottom + razmakIzmjedjuHampera;

      CreateHamper_HorLine();
      CreateHamper4printDocument();

      this.ResumeLayout();
   }

   /// <summary>
   /// nastaje preko constructora SubKlasa VvFilterUC-a
   /// </summary>
   /*protected*/ public VvUserControl TheVvUC { get; set; }

   #region CreateHamper_4ButtonsResetGo_

   private void CreateHamper_4ButtonsResetGo_Height()
   {
      hamper4buttons = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper4buttons.VvRowHgt       = new int[] { ZXC.QunBtnH - ZXC.Qun4};
      hamper4buttons.VvSpcBefRow    = new int[] { ZXC.Qun10 };
      hamper4buttons.VvBottomMargin = hamper4buttons.VvTopMargin;
   }

   protected void CreateHamper_4ButtonsResetGo_Width(int maxHamperWidth)
   {
      int vvspecBeforZeroButton  = 0;
      int vvspecBeforFirstButton = ZXC.Qun4;
      int eventualHampWidth, vvspecBeforLastButton;

      hamper4buttons.VvColWdt = new int[] { ZXC.QunBtnW, ZXC.QunBtnW };

      eventualHampWidth = hamper4buttons.VvColWdt[0] + hamper4buttons.VvColWdt[1] + 
                          2*vvspecBeforZeroButton    + vvspecBeforFirstButton;

      if(maxHamperWidth >(eventualHampWidth))
         vvspecBeforLastButton = maxHamperWidth - eventualHampWidth + vvspecBeforFirstButton;
      else
         vvspecBeforLastButton = vvspecBeforFirstButton;


      hamper4buttons.VvSpcBefCol   = new int[] { 0, vvspecBeforLastButton };
      hamper4buttons.VvRightMargin = hamper4buttons.VvLeftMargin;
       
      btn_GO    = hamper4buttons.CreateVvButton(0, 0, new EventHandler(ButtonIzlistaj_Click), "Izlistaj");
      btn_Reset = hamper4buttons.CreateVvButton(1, 0, new EventHandler(ButtonRESET_Click)   , "R&eset");

      VvHamper.HamperStyling(hamper4buttons);
   }

   private void CreateHamper_HorLine()
   {
      // 18.02.2019. dodajemo ovdje i zaExport 
    //hamperHorLine = new VvHamper(1, 1, "", this, false);
    //hamperHorLine = new VvHamper(2, 1, "", this, false); 02.01.2023. dodajemo i HRK/EUR
      hamperHorLine = new VvHamper(3, 1, "", this, false);

    //hamperHorLine.VvColWdt      = new int[] { ZXC.Q6un                     };
    //hamperHorLine.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q5un           };
      hamperHorLine.VvColWdt      = new int[] { ZXC.Q6un-ZXC.Qun8 , ZXC.Q4un - ZXC.Qun8 , ZXC.Q3un  };
      hamperHorLine.VvSpcBefCol   = new int[] {          ZXC.Qun12,            ZXC.Qun12, ZXC.Qun12 };
      hamperHorLine.VvRightMargin = hamperHorLine.VvLeftMargin;

      hamperHorLine.VvRowHgt       = new int[] { ZXC.QUN };
      hamperHorLine.VvSpcBefRow    = new int[] { ZXC.Qun12 };
      hamperHorLine.VvBottomMargin = hamperHorLine.VvTopMargin;

      cb_Line     = hamperHorLine.CreateVvCheckBox_OLD(0, 0, null, "Horizontalne crte", RightToLeft.No);

      if(this is RiskFilterUC) // za sada samo na RiskFilterUC
      {
         cb_zaExport = hamperHorLine.CreateVvCheckBox_OLD(1, 0, null, "Za Export", RightToLeft.No);
         cb_isHrdKontra   = hamperHorLine.CreateVvCheckBox_OLD(2, 0, null, (ZXC.projectYearAsInt <= 2022 ? "EUR":"HRK"), RightToLeft.No);
      }

      VvHamper.HamperStyling(hamperHorLine);
   }

   protected int LocationOfHamper_HorLine(int nextX, int nextY, int MaxHamperWidth)
   {
      hamperHorLine.Location = new Point(nextX, nextY);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamperHorLine, MaxHamperWidth, razmakIzmjedjuHampera);

      return hamperHorLine.Bottom;
   }

   private void CreateHamper4printDocument()
   {
      hamperPrintDoc = new VvHamper();
   }

   protected int LocationOfHamper_PrintDocument(int nextX, int nextY, int MaxHamperWidth)
   {
      hamperPrintDoc.Location = new Point(nextX, nextY);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamperPrintDoc, MaxHamperWidth, razmakIzmjedjuHampera);

      return hamperPrintDoc.Bottom;
   }

   #endregion CreateHamper_4ButtonsResetGo_

   #region ButtonIzlistaj_Click    ButtonRESET_Click

   public void ButtonIzlistaj_Click(object sender, EventArgs e)
   {
      if(TheVvUC is VvReportUC)
      {
         ZXC.TheVvForm.ReportWanted_Click_FromReportUC(sender, e);
      }
      else
      {
         ((VvRecordUC)TheVvUC).SetFilterRecordDependentDefaults();

         VvInnerTabPage innerTabPage = (VvInnerTabPage)((VvRecordUC)TheVvUC).TheTabControl.SelectedTab;

         if(innerTabPage.TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.TransGrid_TabPage)
         {
            // nepotrebno: ((VvSifrarRecordUC)TheVvUC).aTransesLoaded[0] = false;

            ((VvSifrarRecordUC)TheVvUC).LoadRecordList_AND_PutTransDgvFields();
         }
         else if(innerTabPage.TheInnerTabPageKindEnum == ZXC.VvInnerTabPageKindEnum.ReportViewer_TabPage)
         {
            // nepotrebno: ((VvRecordUC)TheVvUC).recordReportLoaded = false;

            ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(true);
         }
      }
   }

   public void ButtonRESET_Click(object obj, EventArgs ea)
   {
      if(TheVvUC is VvReportUC)
      {
         VvHamper.ClearFieldContents(TheVvUC); 
      }
      else
      {
         VvHamper.ClearFieldContents(this); 
      }

      // Ovaj ledo GetFields sluzi da na osnovu praznoga UC-a ocisti i bussiness object VvRptFilter 
      GetFilterFields();

      if(TheVvUC is VvReportUC)
      {   
         ((VvReportUC)TheVvUC).ResetRptFilterRbCbControls();
      }
      else
      {
         ((VvRecordUC)TheVvUC).SetFilterRecordDependentDefaults();
      }

      TheVvUC.VirtualRptFilter.SetDefaultFilterValues();
      PutFilterFields(TheVvUC.VirtualRptFilter);


      // ha, ha lukavo. 
      // 14.4.2011: pjesnik se vise ne sjeca sta je tu 'ha, ha lukavo.' pa ubijamo ovaj PerformClick do dalnjeg jer samo smeta.
      // ako se kasnije uspostavi da je ipak zbog necega PerformClick bio potreban, bumo odremarckirali.   
      //this.btn_GO.PerformClick();    //if(TheVvUC is VvRecordUC) mozda bi to trebalo

      //// 06.04.2012: dodao, pa zatim remarkirao. ne treba. vidi RiskFilterUC.GetFilterFields prva linija komentar 
      //if(this is RiskFilterUC)
      //{
      //   (this as RiskFilterUC).LoadVvRiskMacroList();
      //   (this as RiskFilterUC).InitializeVvRiskMacroList_ComboBox();
      //}
      
   }

   #endregion ButtonIzlistaj_Click    ButtonRESET_Click


   public abstract void AddFilterMemberz(VvRptFilter rptFilter, VvReport report);

   public abstract void PutFilterFields(VvRptFilter rptFilter);
   public abstract void GetFilterFields();

   public void SetUpAsWriteOnlyTbx(VvHamper hamper)
   {
      foreach(Control ctrl in hamper.Controls)
      {
         if(ctrl is VvTextBox) ((VvTextBox)ctrl).JAM_WriteOnly = true;
      }
   }

   #region Datumi_ContexMenu

   public VvStandardTextBoxContextMenu CreateNewContexMenu_Date()
   {
      VvStandardTextBoxContextMenu date_ContexMenu = new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem("Danas"            , IspuniDatume),
               new MenuItem("Jučer"            , IspuniDatume),
               new MenuItem("Tekuća godina"    , IspuniDatume),
               new MenuItem("Tekući mjesec"    , IspuniDatume),
               new MenuItem("Prvo polugodište" , IspuniDatume),
               new MenuItem("Drugo polugodište", IspuniDatume),
               new MenuItem("Prvi kvartal"     , IspuniDatume),
               new MenuItem("Drugi kvartal"    , IspuniDatume),
               new MenuItem("Treći kvartal"    , IspuniDatume),
               new MenuItem("Četvrti kvartal"  , IspuniDatume),
               new MenuItem("1 -11 mjesec"     , IspuniDatume),
               new MenuItem("1 -10 mjesec"     , IspuniDatume),
               new MenuItem("1 - 9 mjesec"     , IspuniDatume),
               new MenuItem("1 - 8 mjesec"     , IspuniDatume),
               new MenuItem("1 - 7 mjesec"     , IspuniDatume),
               new MenuItem("1 - 6 mjesec"     , IspuniDatume),
               new MenuItem("1 - 5 mjesec"     , IspuniDatume),
               new MenuItem("1 - 4 mjesec"     , IspuniDatume),
               new MenuItem("1 - 3 mjesec"     , IspuniDatume),
               new MenuItem("1 - 2 mjesec"     , IspuniDatume),
               new MenuItem("Siječanj"         , IspuniDatume),
               new MenuItem("Veljača"          , IspuniDatume),
               new MenuItem("Ožujak"           , IspuniDatume),
               new MenuItem("Travanj"          , IspuniDatume),
               new MenuItem("Svibanj"          , IspuniDatume),
               new MenuItem("Lipanj"           , IspuniDatume),
               new MenuItem("Srpanj"           , IspuniDatume),
               new MenuItem("Kolovoz"          , IspuniDatume),
               new MenuItem("Rujan"            , IspuniDatume),
               new MenuItem("Listopad"         , IspuniDatume),
               new MenuItem("Studeni"          , IspuniDatume),
               new MenuItem("Prosinac"         , IspuniDatume),
               new MenuItem("-")
            });

      return date_ContexMenu;
   }

   public void IspuniDatume(object sender, EventArgs e)
   {
      MenuItem tsmi = sender as MenuItem;

      string text = tsmi.Text;
      string textOd = "";
      string textDo = "";
      string mj02 = "28"; ;

      // ovo je dobro. ostavi ovako (TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;) 
      string godina = TheVvUC.TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.ProjectYear;
      int god = int.Parse(godina);

      if(DateTime.IsLeapYear(god)) mj02 = "29";
      else mj02 = "28";

      switch(text)
      { //                              mmOD           dd.mmDO
         case "Tekuća godina"    : textOd = "01"; textDo = "31.12"; break;
         case "Tekući mjesec"    : textOd = DateTime.Today.Month.ToString(); textDo = DateTime.DaysInMonth(god, DateTime.Today.Month).ToString() + "." + DateTime.Today.Month.ToString(); break;
         case "Prvi kvartal"     : textOd = "01"; textDo = "31.03"; break;
         case "Drugi kvartal"    : textOd = "04"; textDo = "30.06"; break;
         case "Treći kvartal"    : textOd = "07"; textDo = "30.09"; break;
         case "Četvrti kvartal"  : textOd = "10"; textDo = "31.12"; break;
         case "Prvo polugodište" : textOd = "01"; textDo = "30.06"; break;
         case "Drugo polugodište": textOd = "07"; textDo = "31.12"; break;
         case "1 -11 mjesec"     : textOd = "01"; textDo = "30.11"; break;
         case "1 -10 mjesec"     : textOd = "01"; textDo = "31.10"; break;
         case "1 - 9 mjesec"     : textOd = "01"; textDo = "30.09"; break;
         case "1 - 8 mjesec"     : textOd = "01"; textDo = "31.08"; break;
         case "1 - 7 mjesec"     : textOd = "01"; textDo = "31.07"; break;
         case "1 - 6 mjesec"     : textOd = "01"; textDo = "30.06"; break;
         case "1 - 5 mjesec"     : textOd = "01"; textDo = "31.05"; break;
         case "1 - 4 mjesec"     : textOd = "01"; textDo = "30.04"; break;
         case "1 - 3 mjesec"     : textOd = "01"; textDo = "31.03"; break;
         case "1 - 2 mjesec"     : textOd = "01"; textDo = mj02 + ".02"; break;
         case "Siječanj"         : textOd = "01"; textDo = "31.01"; break;
         case "Veljača"          : textOd = "02"; textDo = mj02 + ".02"; break;
         case "Ožujak"           : textOd = "03"; textDo = "31.03"; break;
         case "Travanj"          : textOd = "04"; textDo = "30.04"; break;
         case "Svibanj"          : textOd = "05"; textDo = "31.05"; break;
         case "Lipanj"           : textOd = "06"; textDo = "30.06"; break;
         case "Srpanj"           : textOd = "07"; textDo = "31.07"; break;
         case "Kolovoz"          : textOd = "08"; textDo = "31.08"; break;
         case "Rujan"            : textOd = "09"; textDo = "30.09"; break;
         case "Listopad"         : textOd = "10"; textDo = "31.10"; break;
         case "Studeni"          : textOd = "11"; textDo = "30.11"; break;
         case "Prosinac"         : textOd = "12"; textDo = "31.12"; break;
         case "Danas"            : textOd = ""  ; textDo = ""     ; break;
      }

      if(text == "Danas")
      {
         tbx_DatumOD.Text = DateTime.Today.ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.ToString("dd.MM.yyyy");
      }
      else if(text == "Jučer")
      {
         tbx_DatumOD.Text = DateTime.Today.DayBefore().ToString("dd.MM.yyyy");
         tbx_DatumDO.Text = DateTime.Today.DayBefore().ToString("dd.MM.yyyy");
      }
      else
      {
         tbx_DatumOD.Text = "01." + textOd + "." + godina;
         tbx_DatumDO.Text = textDo + "." + godina;
      }
      
      dtp_DatumOD.Value = DateTime.Parse(tbx_DatumOD.Text);
      dtp_DatumDO.Value = DateTime.Parse(tbx_DatumDO.Text);

   }

   #endregion Datumi_ContexMenu


}

public          class VvStandAloneReportViewerUC : UserControl, IVvPrintableUC
{
   private bool alreadyDisposed = false;

   private ReportDocument TheReportDocument;
   private DataSet        TheDataset;

   
   public CrystalReportViewer TheReportViewer    { get; set; }
   public ToolStripPanel      tsPanel;

   public VvStandAloneReportViewerUC()
   {
      CreateReportViewer();
      CreateToolStripPanel();
   }

   #region CreateReportViewer_And_CreateToolStripPanel

   private void CreateReportViewer()
   {
      TheReportViewer                  = new CrystalReportViewer();
      TheReportViewer.Parent           = this;
      TheReportViewer.Dock             = DockStyle.Fill;
     // TheReportViewer.DisplayGroupTree = false;
      TheReportViewer.ToolPanelView = ToolPanelViewType.None;
      TheReportViewer.DisplayToolbar   = false;
      TheReportViewer.DisplayStatusBar = true;
   }
   private void CreateToolStripPanel()
   {
      tsPanel           = new ToolStripPanel();
      tsPanel.Parent    = this;
      tsPanel.Dock      = DockStyle.Top;
      tsPanel.Name      = "tsPanel_PrevReport";
      tsPanel.BackColor = ZXC.vvColors.tsPanel_BackColor;

      ZXC.TheVvForm.ts_Report.Parent = tsPanel;
      ZXC.TheVvForm.ts_Report.Visible = true;

      // ovime se odredjuje visibilti pojedinih buttona ovisno o tome kome "pripada" ts_Report
      //ZXC.TheVvForm.ToolStripReportVisible_tsBtnOnTsReportVisible(false, ZXC.ReportMode.Done, true);
      // ali premjestila na forme(parente) jer svak ima svoje
   }

   #endregion CreateReportViewer_And_CreateToolStripPanel

   #region SetDataSource_And_AssignReportSource

   public void SetDataSource_And_AssignReportSource(ReportDocument reportDocument, DataSet dataset)
   {
      TheReportDocument = reportDocument;
      TheDataset        = dataset;

      TheReportDocument.SetDataSource(TheDataset);

      TheReportViewer.ReportSource = reportDocument;
   }

   // 11.05.2012. add Tamara 
   public void SetNonDatasetDataSource_And_AssignReportSource(ReportDocument reportDocument, System.Collections.IList bussinessList, string reportTableName)
   {
      TheReportDocument = reportDocument;


      if(bussinessList != null && bussinessList.Count.NotZero() &&
         reportDocument.Database.Tables.Cast<Table>().SingleOrDefault(table => table.Name == reportTableName) != null)
      {
         reportDocument.Database.Tables[reportTableName].SetDataSource(bussinessList);
         TheReportViewer.ReportSource = reportDocument;
      }
      else
      {
         MessageBox.Show("Nema se što prikazati za odabrani Tab");
         this.ParentForm.Close();
      }
   }

   #endregion SetDataSource_And_AssignReportSource

   #region Dispose

   protected override void Dispose(bool disposing)
   {
      if(!this.alreadyDisposed)
      {
         try
         {
            if(disposing)
            {
               // Release the managed resources you added in
               // this derived class here.

               //addedManaged.Dispose();
            }
            // Release the native unmanaged resources you added
            // in this derived class here.

            //===================================================

            //if(this.conn != null) this.conn.Close();

            //if(this.IsForReport && TheVvReportUC.TheVvReport != null)
            //{
            if(TheReportDocument != null) TheReportDocument.Close();

            if(TheDataset != null)
            {
               foreach(System.Data.DataTable dt in TheDataset.Tables)
               {
                  dt.Dispose();
               }

               TheDataset.Dispose();
            }
            //}

            //===================================================

            this.alreadyDisposed = true;
         }
         finally
         {
            // Call Dispose on your base class.
            base.Dispose(disposing);
         }
      }
   }

   #endregion Dispose

   #region IVvPrintableUC Members

   CrystalReportViewer IVvPrintableUC.VirtualReportViewer
   {
      get { return this.TheReportViewer; }
   }

   #endregion
}

public /*struct*/ class VvMigrator
{
   [System.Xml.Serialization.XmlIgnore]
   public VvHamper TheHamper  { get; set; }

   // Samo ova dva idu u XML perzistentnost 
   public bool     IsMigrated { get; set; }
   public string   MigName    { get; set; }

   // Ovaj parameterless constructor mora biti zbog Serialization, ona inzistira na takvom constructoru. Zasto ne kuzim, ali mu ga jos das da je private, pa je bezopasan
   private VvMigrator() { }

   public VvMigrator(VvHamper hamper, bool isMigr, string name) : this() // bez ovoga : this() skace Compiler Error CS0843 
   {
      this.TheHamper     = hamper;
      this.IsMigrated    = isMigr;
      this.MigName       = name;
   }

   //public VvMigrator(VvHamper hamper, bool isMigr) : this(hamper, isMigr, hamper.Name) 
   //{
   //}

   public VvMigrator(VvHamper hamper) : this(hamper, false, hamper.Name) 
   {
   }
}

