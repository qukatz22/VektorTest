using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;

#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.Collections.Generic;
#endif

public class VvTextBox : TextBox, IEditableObject
{
   #region Constructor

   public VvTextBox()
   {
      this.KeyDown += new KeyEventHandler(JAM_EnterSendsTab_OnKeyDown);
   }

   #endregion Constructor

   #region private fieldz

   private const int  defaultNumberOfDecimalPlaces = 2;
   private const bool defaultOceNeceZareze         = true;
   private const bool defaultClearIfZero           = true;

   private bool      resultBox;
   private bool      readOnly;
   private bool      writeOnly;
   private bool      isForDateTimePicker;
   private bool      isForCheckBox;
   private bool      isForDateTimePicker_WithTimeDisplay;
   private bool      isForDateTimePicker_TimeOnlyDisplay;
   private bool      isForDateTimePicker_YearOnly;
   private bool      lookUp_NOTobligatory;
   private bool      lookUp_MultiSelection;
   private uint      lookUpTableColIdx;
   private VvLookUpLista lookUpList;
   private string[]  lookUpTable;
   private string    lookUpTableTitle;
   private string[]  helpTable;
   private string    statusText;
 //private bool      upperCase;
 //private bool      lowerCase;
   private bool      nonDisplay;
   private bool      reverseVideo;
   private bool      highlighted;
   private bool      mustFill;
   private bool      dataRequired;
   private bool      isPasswd;
   private char      fillCharacter = ' ';

   private bool      mustTabOutBeforeSubmit;
   private bool      isInDataGridView;
   private bool      isNumericTextBox;
   private bool      addCommas;
   private bool      clearIfZero;
   private int       numberOfDecimalPlaces;
   private decimal   minNumericValue;
   private decimal   maxNumericValue;
   private bool      isAutocomplete;
   private bool      isOtsInfo;
   private bool      shouldCopyPrevRow;
   private bool      shouldCopyPrevRowUnCond;
   private bool      shouldSumGrid;
   private bool      shouldCalcTrans;
   private bool      disableUpdateVvDataRecordActions;
   private bool      disableUpdateVvLookUpItemActions;
   private string    autoCompleteRecordName;
   private string    allowedCharacters;
   private string    otsKonto;
   private uint      otsKupdobCd;
   private DateTime  otsDateDo;
   private decimal   otsMoney;
   private VvSQL.SorterType           autoCompleteSorterType;
   private ZXC.AutoCompleteRestrictor autoCompleteRestrictor;

   private string    twinTaker_JAM_Name;
   private string    ttNumTaker_JAM_Name;
   private string    lui_CdTaker_JAM_Name;
   private string    lui_NameTaker_JAM_Name;
   private string    lui_NumberTaker_JAM_Name;
   private string    lui_FlagTaker_JAM_Name;
   private string    lui_IntegerTaker_JAM_Name;
   private string    lui_UintegerTaker_JAM_Name;
   private string    lui_DateTaker_JAM_Name;
   private VvLookUpItem chosenLookUpItem;
   private bool      isForPostotak;
   private bool      isSupressTab;
   private bool      isOnEndEditJump2NextRow;

   private ZXC.JAM_CharEdits charEdits = ZXC.JAM_CharEdits.Unfiltered;

   private Type valueType;

   /*private*/public Button vvLookUpButton;

   private Color  backColor;
   private Color  foreColor;

   #region IEditableObject fieldz

   private bool   editInProgress = false;
   private string backupText;

   #endregion IEditableObject fieldz

   private VvTextBox nextTextBox;

   #endregion private fieldz

   #region Public Properties

   // 19.12.20011:

   /// <summary>
   /// Util, additional Tag opce namjene 
   /// </summary>
   public object VVtag2 { get; set; } // FUSE !!! 

   /// <summary>
   /// True if tekstBoks iz placed inD DejtaGridVju cell
   /// </summary>
   public bool JAM_IsInDataGridView
   {
      get { return isInDataGridView; }
      set { isInDataGridView = value; }
   }

   /// <summary>
   /// Samo za TipBr textBox da OnEnter sacuva konto i kupdobCd za UpdateOtsInfo()
   /// </summary>
   public string OtsKonto
   {
      get { return otsKonto; }
      set { otsKonto = value; }
   }

   /// <summary>
   /// Samo za TipBr textBox da OnEnter sacuva konto i kupdobCd za UpdateOtsInfo()
   /// </summary>
   public uint OtsKupdobCd
   {
      get { return otsKupdobCd; }
      set { otsKupdobCd = value; }
   }

   /// <summary>
   /// Samo za TipBr textBox da OnEnter sacuva konto i kupdobCd i DateDo za UpdateOtsInfo()
   /// </summary>
   public DateTime OtsDateDo
   {
      get { return otsDateDo; }
      set { otsDateDo = value; }
   }

   /// <summary>
   /// Samo za TipBr textBox da OnEnter sacuva konto i kupdobCd i DateDo za UpdateOtsInfo()
   /// </summary>
   public decimal OtsMoney
   {
      get { return otsMoney; }
      set { otsMoney = value; }
   }

   /// <summary>
   /// Ako nije true, ne dodaje mu se Validating += JAM_Validating() u VvHamper.Open_Close_Fields_ForWriting()
   /// </summary>
   public bool JAM_ShouldValidateOnExit
   {
      get 
      {
         if(JAM_IsInDataGridView == true) return false;

         // ovdje u OR-u navedi sve razloge kada je SHOULD true 
         if(JAM_LookUpTable                   != null ||
            JAM_DataRequired                  == true ||
            JAM_MustFill                      == true ||
            JAM_IsNumericTextBox              == true ||
            JAM_MustTabOutBeforeSubmit        == true ||
            JAM_FieldExitWithValidationMethod != null)
         {
            return true;
         }
         else
         {
            return false;
         }
      }
   }

   /// <summary>
   /// Tablica za omoc pri unosu (neobavezno iz tablice) 
   /// </summary>
   public string[] JAM_HelpTable
   {
      get { return helpTable; }
      set { helpTable = value; }
   }

   public VvLookUpLista JAM_LookUpList
   {
      get { return lookUpList; }
      set {        lookUpList = value; }
   }

   /// <summary>
   /// Tablica dozvoljenih odgovora (obavezno iz tablice) za JAM_ValidatingActions
   /// Pazi!: ovo nije izvor otkuda ce se puniti this OnExit. O tome brigu vodi GetLookUpItemText() u VvUserControl.cs
   /// </summary>
   public string[] JAM_LookUpTable
   {
      get { return lookUpTable;         }
      set {        lookUpTable = value; }
   }

   /// <summary>
   /// Naziv LookUp Tablice
   /// </summary>
   public string JAM_LookUpTableTitle
   {
      get { return lookUpTableTitle; }
      set { lookUpTableTitle = value; }
   }

   public bool JAM_ResultBox
   {
      get { return this.resultBox;         }
      set 
      {        
         this.resultBox = value;
         if(value == true)
         {
            this.ReadOnly  = true;
            this.BackColor = ZXC.vvColors.vvTBoxResultBox_True_BackColor;
            this.ForeColor = ZXC.vvColors.vvTBoxResultBox_True_ForeColor;
         }
      }
   }

   public bool JAM_ReadOnly
   {
      get { return this.readOnly;         }
      set {        this.readOnly = value; }
   }

   public bool JAM_WriteOnly
   {
      get { return this.writeOnly; }
      set {        this.writeOnly = value; }
   }

   /// <summary>
   /// VvLookUpTable nije obavezan, vec pomocni unos 
   /// </summary>
   public bool JAM_lookUp_NOTobligatory
   {
      get { return this.lookUp_NOTobligatory; }
      set {        this.lookUp_NOTobligatory = value; }
   }
   /// <summary>
   /// VvLookUpTable je multi selektivan; u jedan ili vise redaka
   /// </summary>
   public bool JAM_lookUp_MultiSelection
   {
      get 
      {
         if(this.lookUp_MultiSelection == true || this.Multiline == true)
            return true;
         else
            return false;
      }
      set { this.lookUp_MultiSelection = value; }
   }

   /// <summary>
   /// thisTakesDataFromColIdx iz koje 'this' VvTextBox crpi informaciju na OnExit...
   /// </summary>
   public uint JAM_lookUpTableColIdx
   {
      get { return this.lookUpTableColIdx; }
      set {        this.lookUpTableColIdx = value; }
   }

   /// <summary>
   /// OVaj ce se TextBox togglati sa DateTimePickerom kod editiranja
   /// </summary>
   public bool JAM_IsForDateTimePicker
   {
      get { return this.isForDateTimePicker;         }
      set {        this.isForDateTimePicker = value; }
   }

   /// <summary>
   /// OVaj ce se TextBox togglati sa CheckBoxom kod editiranja
   /// </summary>
   public bool JAM_IsForCheckBox
   {
      get { return this.isForCheckBox;         }
      set {        this.isForCheckBox = value; }
   }

   /// <summary>
   /// TIME DISPLAY DTP + OVaj ce se TextBox togglati sa DateTimePickerom kod editiranja 
   /// </summary>
   public bool JAM_IsForDateTimePicker_WithTimeDisplay
   {
      get { return this.isForDateTimePicker_WithTimeDisplay;         }
      set {        this.isForDateTimePicker_WithTimeDisplay = value; }
   }

   /// <summary>
   /// TIME DISPLAY DTP + OVaj ce se TextBox togglati sa DateTimePickerom kod editiranja 
   /// </summary>
   public bool JAM_IsForDateTimePicker_TimeOnlyDisplay
   {
      get { return this.isForDateTimePicker_TimeOnlyDisplay;         }
      set {        this.isForDateTimePicker_TimeOnlyDisplay = value; }
   }

   /// <summary>
   /// YEAR DISPLAY DTP + OVaj ce se TextBox togglati sa DateTimePickerom kod editiranja 
   /// </summary>
   public bool JAM_IsForDateTimePicker_YearOnly
   {
      get { return this.isForDateTimePicker_YearOnly; }
      set { this.isForDateTimePicker_YearOnly = value; }
   }

   /// <summary>
   /// Ovaj ce se text OnEnter pojaviti na StatusStripu, a po izlasku (OnLeave) se restor-a stari staus text
   /// </summary>
   public string JAM_StatusText
   {
      get { return this.statusText; }
      set 
      {
         statusText = value;
         this.Enter += new EventHandler(OnEnterSetStatusText);
         this.Leave += new EventHandler(OnExitRestoreStatusText);
      }
   }

   /// <summary>
   /// Svi se characteri prikazuju upper case (pazi!, ovo se ne odnosi samo na unos, vec i na vec postojeci podatak (za razliku od JAM-a)
   /// </summary>
   public CharacterCasing JAM_CharacterCasing
   {
      get { return this.CharacterCasing; }
      set 
      {
         this.CharacterCasing = value;

         //     if(upperCase == true)  this.CharacterCasing = CharacterCasing.Upper;
         ////17.02.2011: zasto li sam ovo u proslosti komentirao?
         ////else if(upperCase == false) this.CharacterCasing = CharacterCasing.Normal;
         //else                        this.CharacterCasing = CharacterCasing.Normal;
      }
   }

   /// <summary>
   /// Svi se characteri prikazuju lower case (pazi!, ovo se ne odnosi samo na unos, vec i na vec postojeci podatak (za razliku od JAM-a)
   /// </summary>
   //public bool JAM_LowerCase
   //{
   //   get { return lowerCase; }
   //   set
   //   {
   //      lowerCase = value;

   //           if(lowerCase == true)  this.CharacterCasing = CharacterCasing.Lower;
   //      //17.02.2011: zasto li sam ovo u proslosti komentirao?
   //      //else if(lowerCase == false) this.CharacterCasing = CharacterCasing.Normal;
   //      else                        this.CharacterCasing = CharacterCasing.Normal;
   //   }
   //}

   /// <summary>
   /// Ako je true, umjesto slova vidi se znak ' '
   /// </summary>
   public bool JAM_NonDisplay
   {
      get { return nonDisplay; }
      set 
      { 
         nonDisplay = value;
         if(nonDisplay == true)  this.PasswordChar = ' ';
      }
   }

   /// <summary>
   /// Zamijeni BackColor / ForeColor
   /// </summary>
   public bool JAM_ReverseVideo
   {
      get { return reverseVideo; }
      set 
      { 
         reverseVideo = value;
         if(reverseVideo == true)
         {
            Color tmpColor = new Color();
            tmpColor = this.BackColor;
            this.BackColor = this.ForeColor;
            this.ForeColor = tmpColor;
         }
      }
   }

   /// <summary>
   /// Ako je true, bu nam font boldan
   /// </summary>
   public bool JAM_Highlighted
   {
      get { return highlighted; }
      set 
      { 
         highlighted = value;
         if(highlighted == true) this.Font = ZXC.vvFont.BaseBoldFont;
         //else                    this.Font = ZXC.opt.BaseFont;
      }
   }

   /// <summary>
   /// Puni polje slijeva sos ovi znak
   /// </summary>
   public char JAM_FillCharacter
   {
      get { return fillCharacter; }
      set { fillCharacter = value; }
   }

   /// <summary>
   /// TextAlignment, Numeric keys filter, ...
   /// </summary>
   public bool JAM_IsNumericTextBox
   {
      get { return isNumericTextBox;  }
      private set 
      { 
         isNumericTextBox = value; 

         if(this.MaxLength == 32767 || this.MaxLength < 1) 
            throw new Exception("VvTextBox-u [" + this.Name + "] MaxLength nedefiniran!");
      }
   }

   /// <summary>
   /// Ocemo znak '%' na kraju
   /// </summary>
   public bool JAM_IsForPercent
   {
      get { return isForPostotak;  }
      set {        isForPostotak = value; }
   }

   /// <summary>
   /// Nema taba u ovo polje
   /// </summary>
   public bool JAM_IsSupressTab
   {
      get { return isSupressTab; }
      set {        isSupressTab = value; }
   }

   /// <summary>
   /// Npr BarCode: Nakon EndEdit skoci na pocetak sljedeceg redka 
   /// </summary>
   public bool JAM_IsOnEndEditJump2NextRow
   {
      get { return isOnEndEditJump2NextRow; }
      set {        isOnEndEditJump2NextRow = value; }
   }

   public override int MaxLength
   {
      get 
      {
         if(base.MaxLength == 32767 || base.MaxLength < 1)
            throw new Exception("VvTextBox-u [" + this.Name + "] MaxLength nedefiniran!");
         return base.MaxLength;  
      }
      set { base.MaxLength = value; }
   }

   public int JAM_NumberOfDecimalPlaces
   {
      get { return numberOfDecimalPlaces;  }
      private set { numberOfDecimalPlaces = value; }
   }

   /// <summary>
   /// Postavi JAM_MinNumericValue na decimal.Zero
   /// </summary>
   public bool JAM_DisableNegativeNumberValues
   {
      get 
      {
         if(JAM_IsNumericTextBox && JAM_MinNumericValue == decimal.Zero) return true;
         else                                                            return false; 
      }
      set 
      {
         if(JAM_IsNumericTextBox && value == true) JAM_MinNumericValue = decimal.Zero; 
      }
   }

   public decimal JAM_MinNumericValue
   {
      get 
      { 
         if(JAM_IsNumericTextBox) return minNumericValue; 
         else                     return Decimal.MinValue;
      }
      private set 
      {
         decimal calculatedBasedOnFieldLength = JAM_IsNumericTextBox ? CalcMinNumericValue_BasedOnMaxLength() : Decimal.MinValue;

              if(value == decimal.MaxValue)            this.minNumericValue = calculatedBasedOnFieldLength;
         else if(value < calculatedBasedOnFieldLength) throw new Exception(String.Format("<{0}> Predugacki MinNumericValue s obzirom na duzinu polja", this.Name));
         else                                          this.minNumericValue = value; 
      }
   }

   public decimal JAM_MaxNumericValue
   {
      get 
      {
         if(JAM_IsNumericTextBox) return maxNumericValue; 
         else                     return Decimal.MaxValue;
      }
      private set 
      {
         decimal calculatedBasedOnFieldLength = JAM_IsNumericTextBox ? CalcMaxNumericValue_BasedOnMaxLength() : Decimal.MaxValue;

              if(value == decimal.MinValue)            this.maxNumericValue = calculatedBasedOnFieldLength;
         else if(value > calculatedBasedOnFieldLength) throw new Exception(String.Format("<{0}> PredugackiPredugacki MaxNumericValue s obzirom na duzinu polja", this.Name));
         else                                          this.maxNumericValue = value; 
      }
   }

   /// <summary>
   /// Oce nece zareze za tisucice
   /// </summary>
   public bool JAM_AddCommas
   {
      get { return addCommas; }
      private set { addCommas = value; }
   }

   /// <summary>
   /// Ako je true, kada je value == 0 onda je (vizualno) polje prazno
   /// </summary>
   public bool JAM_ClearIfZero
   {
      get { return clearIfZero; }
      private set { clearIfZero = value; }
   }

   /// <summary>
   /// User mora popuniti sva mjesta field-a
   /// </summary>
   public bool JAM_MustFill
   {
      get { return mustFill; }
      set { mustFill = value; }
   }

   /// <summary>
   /// Nemoze ostati prazno
   /// </summary>
   public bool JAM_DataRequired
   {
      get { return dataRequired; }
      set { dataRequired = value; }
   }

   /// <summary>
   /// Unfiltered, DigitsOnly, Numeric, ...
   /// </summary>
   public ZXC.JAM_CharEdits JAM_CharEdits
   {
      get { return charEdits; }
      set { charEdits = value; }
   }

   /// <summary>
   /// Type supposed tu bi in dis tekstboks
   /// </summary>
   public Type JAM_ValueType
   {
      get { return valueType; }
      set { valueType = value; }
   }

   /// <summary>
   /// If tru, ODJEDNOM (kratica) should bi sumirano na TheSumGrid-i i CalcTrans
   /// </summary>
   public bool JAM_ShouldCalcTransAndSumGrid
   {
      //get { return shouldSumGrid && shouldCalcTrans; }
      set { shouldSumGrid = shouldCalcTrans = value; }
   }

   /// <summary>
   /// If tru, should copyrati prev row value ali samo ako je zadnji red
   /// (bez obzira je li zadnji radi JAM_ShouldCopyPrevRowUnCond)
   /// </summary>
   public bool JAM_ShouldCopyPrevRow
   {
      get { return shouldCopyPrevRow; }
      set { shouldCopyPrevRow = value; }
   }

   /// <summary>
   /// If tru, should copyrati prev row value, bez obzira je li zadnji red ili ne 
   /// (samo zadnji radi JAM_ShouldCopyPrevRow)
   /// </summary>
   public bool JAM_ShouldCopyPrevRowUnCond
   {
      get { return shouldCopyPrevRowUnCond; }
      set { shouldCopyPrevRowUnCond = value; }
   }

   /// <summary>
   /// If tru, should bi sumirano na TheSumGrid-i
   /// </summary>
   public bool JAM_ShouldSumGrid
   {
      get { return shouldSumGrid; }
      set { shouldSumGrid = value; }
   }

   /// <summary>
   /// If tru, should raise CalcTransResults
   /// </summary>
   public bool JAM_ShouldCalcTrans
   {
      get { return shouldCalcTrans; }
      set { shouldCalcTrans = value; }
   }

   /// <summary>
   /// Kao sto i naziv kaze... za VvDataRecord
   /// </summary>
   public bool JAM_DisableUpdateVvDataRecordActions
   {
      get { return disableUpdateVvDataRecordActions; }
      set { disableUpdateVvDataRecordActions = value; }
   }

   /// <summary>
   /// Kao sto i naziv kaze... za LookUpItem
   /// </summary>
   public bool JAM_DisableUpdateVvLookUpItemActions
   {
      get { return disableUpdateVvLookUpItemActions; }
      set { disableUpdateVvLookUpItemActions = value; }
   }

   /// <summary>
   /// get { return this.JAM_LookUpTable != null; }
   /// </summary>
   public bool JAM_IsVvLookUp
   {
      get { return this.JAM_LookUpTable != null; }
   }

   /// <summary>
   /// Just for surfacing Control.Name 
   /// </summary>
   public string JAM_Name { get { return this.Name; } }

   /// <summary>
   /// Just for surfacing Control.Name 
   /// </summary>
   public string A0_JAM_Name { get { return this.JAM_Name; } }

   public bool JAM_IsOtsInfo
   {
      get { return this.isOtsInfo; }
      set { this.isOtsInfo = value; }
   }

   /// <summary>
   /// Postavlja enum AutoCompleteMode.SuggestAppend i enum AutoCompleteSource.CustomSource;
   /// </summary>
   public bool JAM_IsAutocomplete
   {
      get { return isAutocomplete; }
      set
      {
         isAutocomplete = value;
         if(value == true)
         {
            this.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
            this.AutoCompleteSource = AutoCompleteSource.CustomSource;
         }
      }
   }

   /// <summary>
   /// Naziv VvDataRecorda odakle se puni ovi TextBox (FindKupdob, FindSklad, ...)
   /// </summary>
   public string JAM_AutoCompleteRecordName
   {
      get { return autoCompleteRecordName; }
      //set { autoCompleteRecordName = value; }
   }

   /// <summary>
   /// TT podatka unutar sifarnika odakle se puni ovi TextBox (sifra, naziv, ticker, ...)
   /// </summary>
   public VvSQL.SorterType JAM_AutoCompleteSorterType
   {
      get { return autoCompleteSorterType; }
      //set { autoCompleteSorterType = value; }
   }

   /// <summary>
   /// Npr. KID_Centrala_Only, ...
   /// </summary>
   public ZXC.AutoCompleteRestrictor JAM_AutoCompleteRestrictor
   {
      get { return autoCompleteRestrictor; }

      // 09.06.2015: otvorio i set. do tada bio u komentaru  
      set { autoCompleteRestrictor = value; }
   }

   /// <summary>
   /// Ono kao u JAM-ovom CharEdits regular expression pa npr [AUI]
   /// </summary>
   public string JAM_AllowedInputCharacters
   {
      get { return allowedCharacters; }
      set 
      {
         if(string.IsNullOrEmpty(value)) this.JAM_CharEdits = ZXC.JAM_CharEdits.Unfiltered;
         else                            this.JAM_CharEdits = ZXC.JAM_CharEdits.RegularExpression;

         allowedCharacters = value; 
      }
   }

   /// <summary>
   /// Setting this property to true will:
   /// 1 - this.UseSystemPasswordChar = true, 
   /// 2 - this.ContextMenu = new ContextMenu(); (disabling right click)
   /// 3 - this.JAM_DataRequired = true,
   /// 4 - OnOpenOrCloseForEditActions(): this.KeyDown += new KeyEventHandler(DisablePaste_OnKeyPress); 
   /// </summary>
   public bool JAM_IsPassword
   {
      get { return this.isPasswd; }
      set 
      {
         if(value == true)
         {
            // 23.09.2016: da moze, ipak, ma right click dobiti 'Paste' 
            //this.ContextMenu = new ContextMenu();

            //this.JAM_DataRequired      = true; // ovo je naporno... 
            this.UseSystemPasswordChar = true;

// s ovim dole osiguravas da je u RELEASE uvijek nevidljiv a kod tebe, doma kak; pise red prije. 
#if (!DEBUG)
            this.UseSystemPasswordChar = true;
#endif
         }
         this.isPasswd = value; 
      }
   }

   /// <summary>
   /// NE za DataGridWiev! tamo ne treba jer se Leave event digne sam od sebe, za razliku od VvTextBox-a koji nije na DGV-u
   /// koji istoga NE digne.
   /// O ovome polju ovise neka druga polja ili stanja, stoga se prije sejvanja
   /// ili drugog oblika submittanja mora prvo TAB out iz polja da se digne validacija
   /// Npr. odnos sadrzaja polja Ticker - sifra - naziv partnera
   /// </summary>
   public bool JAM_MustTabOutBeforeSubmit
   {
      get { return mustTabOutBeforeSubmit; }
      set {        mustTabOutBeforeSubmit = value; }
   }

   /// <summary>
   /// this.vvtb je twin TextBox koji ce pak ovome vvtb-u predati svoj sadrzaj OnExit
   /// </summary>
   public string JAM_twinTaker_JAM_Name
   {
      get { return twinTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetTwin_DataTaker);

         twinTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati ttNum na osnovi tt-a iz LookUp liste
   /// </summary>
   public string JAM_ttNumTaker_JAM_Name
   {
      get { return ttNumTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         ttNumTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati Name kolonu LookUp liste
   /// </summary>
   public string JAM_lui_NameTaker_JAM_Name
   {
      get { return lui_NameTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_NameTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati CD kolonu LookUp liste
   /// </summary>
   public string JAM_lui_CdTaker_JAM_Name
   {
      get { return lui_CdTaker_JAM_Name; }
      set
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_CdTaker_JAM_Name = value;
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati Number kolonu LookUp liste
   /// </summary>
   public string JAM_lui_NumberTaker_JAM_Name
   {
      get { return lui_NumberTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_NumberTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati Flag kolonu LookUp liste
   /// </summary>
   public string JAM_lui_FlagTaker_JAM_Name
   {
      get { return lui_FlagTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_FlagTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati Integer kolonu LookUp liste
   /// </summary>
   public string JAM_lui_IntegerTaker_JAM_Name
   {
      get { return lui_IntegerTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_IntegerTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati UInteger kolonu LookUp liste
   /// </summary>
   public string JAM_lui_UintegerTaker_JAM_Name
   {
      get { return lui_UintegerTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_UintegerTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// this.vvtb je VvLookUp odabir koji ce pak ovome vvtb-u predati Date kolonu LookUp liste
   /// </summary>
   public string JAM_lui_DateTaker_JAM_Name
   {
      get { return lui_DateTaker_JAM_Name; }
      set 
      {
         JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);

         lui_DateTaker_JAM_Name = value; 
      }
   }

   /// <summary>
   /// Selektirani VvLookupItem, potencijalni 'davaoc' za NameTakera, DecimalTakera, ...
   /// </summary>
   public VvLookUpItem JAM_ChosenLookUpItem
   {
      get { return chosenLookUpItem; }
      set {        chosenLookUpItem = value; }
   }

   public Color JAM_BackColor
   {
      get { return backColor; }
      set {        backColor = value; }
   }

   public Color JAM_ForeColor
   {
      get { return foreColor; }
      set {        foreColor = value; }
   }

   public VvTextBox JAM_NextTextBox
   {
      get { return nextTextBox; }
      set {        nextTextBox = value; }
   }

   #endregion Public Properties

   #region Delegates

   /// <summary>
   /// Ova ce se metoda pozvati u Enter eventu TextBox-a
   /// VAZNO!!!: 
   /// ako je vvTB na DataGridView-u, sender je VvTextBoxEditingControl
   /// a ako ne,                      sender je VvTextBox
   /// </summary>
   public EventHandler JAM_FieldEntryMethod;

   /// <summary>
   /// Ova ce se metoda pozvati u Leave eventu TextBox-a
   /// VAZNO!!!: 
   /// ako je vvTB na DataGridView-u, sender je VvTextBoxEditingControl
   /// a ako ne,                      sender je VvTextBox
   /// </summary>
   public EventHandler JAM_FieldExitMethod;

   /// <summary>
   /// Ako neki VvTb treba dvije JAM_FieldExitMethod 
   /// </summary>
   public EventHandler JAM_FieldExitMethod_2;

   /// <summary>
   /// Ako neki VvTb treba tri JAM_FieldExitMethod 
   /// </summary>
   public EventHandler JAM_FieldExitMethod_3;

   /// <summary>
   /// Ova ce se metoda pozvati u Validadting eventu TextBox-a
   /// VAZNO!!!: 
   /// ako je vvTB na DataGridView-u, sender je DataGridView
   /// a ako ne,                      sender je VvTextBox
   /// </summary>
   public CancelEventHandler JAM_FieldExitWithValidationMethod;

   public EventHandler JAM_PasswdField_TextChanged_Method;

   #endregion Delegates

   #region EventHandlers

   private void OnEnterSetStatusText(object sender, EventArgs e)
   {
      if(this.JAM_IsForDateTimePicker) return;

      if(ZXC.TheVvForm.TStripStatusLabel != null)
      {
         ZXC.TheVvForm.statusTextBackup       = ZXC.TheVvForm.TStripStatusLabel.Text;
         ZXC.TheVvForm.TStripStatusLabel.Text = statusText;
      }
   }
   private void OnExitRestoreStatusText(object sender, EventArgs e)
   {
      if(this.JAM_IsForDateTimePicker) return;

      if(ZXC.TheVvForm.TStripStatusLabel != null)
      {
       //17.05.2019. da ne skace
       //                                             ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup;
         if(ZXC.TheVvForm.statusTextBackup.IsEmpty()) ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup = "...";
         else                                         ZXC.TheVvForm.TStripStatusLabel.Text = ZXC.TheVvForm.statusTextBackup        ;
      }
   }

   private void JAM_EnterSendsTab_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(e.KeyCode == Keys.Enter && this.Multiline == false) SendKeys.Send("{TAB}");
   }

   public /*private*/ void EH_JAM_Validating(object sender, System.ComponentModel.CancelEventArgs e)
   {
      // 28.04.2016: 
      if(ZXC.RewriteAllDocuments_InProgress) return;

      e.Cancel = this.JAM_ValidatingActions();
   }

   public /*private*/ void EH_HotOn_GotFocus(object sender, EventArgs e)
   {
      if(this.JAM_IsAutocomplete || this.JAM_IsOtsInfo || this.JAM_IsVvLookUp || this.JAM_HelpTable != null)
      {
         this.BackColor = ZXC.vvColors.vvTBoxHotOn_Find_BackColor;
         this.ForeColor = ZXC.vvColors.vvTBoxHotOn_Find_ForeColor;
      }
      else
      {
         this.BackColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_BackColor; //Color.LightSkyBlue;
         this.ForeColor = ZXC.vvColors.vvTBoxHotOn_GotFocus_ForeColor;
      }
   }

   private void EH_HotOff_LostFocus(object sender, EventArgs e)
   {
      if(this != null && this.JAM_ForeColor != Color.Empty) this.ForeColor = this.JAM_ForeColor;
      else                                                  this.ForeColor = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor;//vvTBoxHotOff_LostFocus_ForeColor;

      if(this != null && this.JAM_BackColor != Color.Empty) this.backColor = this.JAM_BackColor;
      else                                                  this.BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor;//vvTBoxHotOff_LostFocus_BackColor;// ProfessionalColors.MenuItemSelectedGradientBegin;
   }

   public void EH_JAM_FormatNiceNumber(object sender, EventArgs e)
   {
      if(this.IsEmpty()) return;

      decimal dNum;

      string  textToParse;

      if(this.JAM_IsForPercent)
      {
         textToParse = this.Text.TrimEnd('%');
      }
      else
      {
         textToParse = this.Text;
      }
      try
      {
         dNum = decimal.Parse(textToParse, ZXC.GetNumberFormatInfo(this.JAM_NumberOfDecimalPlaces));
      }
      catch(System.FormatException)
      {
         dNum = decimal.Zero;
      }

      PutDecimalField(dNum);
   }

   public void EH_JAM_FormatFillCharacter(object sender, EventArgs e)
   {
      if(this.IsEmpty()) return;

      string filledStr = this.Text.PadLeft(this.MaxLength, this.JAM_FillCharacter);

      this.Text = filledStr;
   }

   private void EH_OnTextChanged_SetDirtyFlag(object sender, System.EventArgs e)
   {
      //ZXC.aim_emsg("Text changed: {0} Text: <{1}>", ((Control)sender).Name, ((Control)sender).Text);
      ZXC.TheVvForm.SetDirtyFlag(((Control)sender).Name);
   }

   private KeyPressEventHandler GetVvTextBoxKeyPressEventHandler()
   {
      switch(this.JAM_CharEdits)
      {
         case ZXC.JAM_CharEdits.NumericOnly:       return new KeyPressEventHandler(KeyPress_Numeric_Filter);
         case ZXC.JAM_CharEdits.DigitsOnly:        return new KeyPressEventHandler(KeyPress_Digits_Filter);
         case ZXC.JAM_CharEdits.LettersOnly:       return new KeyPressEventHandler(KeyPress_Letters_Filter);
         case ZXC.JAM_CharEdits.AlphaNumericOnly:  return new KeyPressEventHandler(KeyPress_AlphaNumeric_Filter);
         case ZXC.JAM_CharEdits.RegularExpression: return new KeyPressEventHandler(KeyPress_RegularExpression_Filter);

         default: throw new Exception("[" + this.JAM_CharEdits.ToString() + "] CharEdits undefined in VvTextBox.GetVvTextBoxKeyPressEventHandler()!");
      }
   }

   private void KeyPress_Numeric_Filter(object sender, KeyPressEventArgs e)
   {
      int imaNema;
      char currSystemRegionalSettingsDecimalSeparator = ZXC.VvCultureInfo0.NumberFormat.NumberDecimalSeparator[0];

      if(e.KeyChar == currSystemRegionalSettingsDecimalSeparator)
      {
         imaNema = this.Text.IndexOf(currSystemRegionalSettingsDecimalSeparator, 0);
         if(imaNema != -1) e.Handled = true;
      }
      else if(e.KeyChar == '-')
      {
         if(this.SelectionStart != 0) e.Handled = true;
         imaNema = this.Text.IndexOf('-', 0);
         if(imaNema != -1) e.Handled = true;
      }
      else
      {
         if(!(char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar))) e.Handled = true;
      }
   }

   private void KeyPress_Digits_Filter(object sender, KeyPressEventArgs e)
   {
      if(!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))) e.Handled = true;
   }

   private void KeyPress_Letters_Filter(object sender, KeyPressEventArgs e)
   {
      if(!(char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar))) e.Handled = true;
   }

   private void KeyPress_AlphaNumeric_Filter(object sender, KeyPressEventArgs e)
   {
      if(!(char.IsLetterOrDigit(e.KeyChar) || char.IsControl(e.KeyChar))) e.Handled = true;
   }

   private void KeyPress_RegularExpression_Filter(object sender, KeyPressEventArgs e)
   {
      string testingStr;

      if(this.JAM_CharacterCasing == CharacterCasing.Upper) testingStr = e.KeyChar.ToString().ToUpper();
      else                                                  testingStr = e.KeyChar.ToString();

      if(!(allowedCharacters.Contains(testingStr) || char.IsControl(e.KeyChar))) e.Handled = true;
   }

   private void DisablePaste_OnKeyPress(object sender, KeyPressEventArgs e)
   {
      if(char.IsControl(e.KeyChar) && e.KeyChar != '\b') // '\b' is backspace 
         e.Handled = true;
   }

   private void OnEnter_BeginEdit(object sender, EventArgs e)
   {
      this.BeginEdit();
   }

   private void OnLeave_EndEdit(object sender, EventArgs e)
   {
      this.EndEdit();
   }

   private void JAM_OnLeaveSelectNextTextBox(object sender, EventArgs e)
   {
      this.JAM_NextTextBox.Focus();
   }

   #region Eventi UpdateVvDataRecord

   private void UpdateVvDataRecord_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(VvUserControl.TriggerKey_ForUpdateVvDataRecord(e) == true) UpdateVvDataRecord_4VvTextBox_Core(sender, e);
   }

   private void UpdateOtsInfo_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(VvUserControl.TriggerKey_ForUpdateVvDataRecord(e) == true) UpdateOtsInfo_4VvTextBox_Core(sender, e);
   }

   private void UpdateVvDataRecord_OnClick(object sender, EventArgs e)
   {
      if(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
      {
         UpdateVvDataRecord_4VvTextBox_Core(sender, e);
      }
   }

   private void UpdateOtsInfo_OnClick(object sender, EventArgs e)
   {
      if(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
      {
         UpdateOtsInfo_4VvTextBox_Core(sender, e);
      }
   }

   //private void UpdateVvDataRecord_OnDoubleClick(object sender, EventArgs e)
   //{
   //   UpdateVvDataRecord_4VvTextBox_Core(sender, e);
   //}

   private void UpdateVvDataRecord_4VvTextBox_Core(object sender, EventArgs e)
   {
      object findResult = VvTextBox.UpdateVvDataRecord_And_SetSifrarAndAutocomplete_Core(this);

      if(findResult != null)
      {
         this.Text = findResult.ToString();
         SendKeys.Send("{TAB}");
      }
   }

   public static object UpdateVvDataRecord_And_SetSifrarAndAutocomplete_Core(VvTextBox _VvTextBox)
   {
      object findResult = VvUserControl.UpdateVvDataRecord(_VvTextBox.JAM_AutoCompleteRecordName,
                                                           _VvTextBox.JAM_AutoCompleteSorterType,
                                                           _VvTextBox.JAM_AutoCompleteRestrictor,
                                                           _VvTextBox.Text,
                                                           _VvTextBox);

      if(ZXC.ShouldForceSifrarRefreshing == true)
      {
         switch(_VvTextBox.JAM_AutoCompleteRecordName)
         {
            case Kplan .recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kplan> (_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case Kupdob.recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Kupdob>(_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case User  .recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<User>  (_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case Prjkt .recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Prjkt> (_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case Osred .recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Osred> (_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case Person.recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Person>(_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;
            case Artikl.recordName: ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(_VvTextBox, _VvTextBox.JAM_AutoCompleteSorterType); break;

            default: ZXC.aim_emsg("Record [{0}] still undone in VvTextBox.UpdateVvDataRecord_4VvTextBox_Core() method!", _VvTextBox.JAM_AutoCompleteRecordName); break;
         }
      }
      return findResult;
   }

   private void UpdateOtsInfo_4VvTextBox_Core(object sender, EventArgs e)
   {
      List<OtsTipBrGroupInfo> choosenOtsList = VvUserControl.UpdateOtsInfo(this);

      ZXC.TheVvForm.DumpChosenOtsList_OnNalogDUC(choosenOtsList);

      //if(choosenOtsList != null)
      //{
      //   this.Text = findResult.ToString();
      //   SendKeys.Send("{TAB}");
      //}
   }

   #endregion Eventi UpdateVvDataRecord

   #region Eventi UpdateVvLookUpItem

   private void UpdateVvLookUpItem_OnKeyDown(object sender, KeyEventArgs e)
   {
      if(VvUserControl.TriggerKey_ForUpdateVvDataRecord(e) == true) UpdateVvLookUpItem_4VvTextBox_Core(this, e);
   }

   private void UpdateVvLookUpItem_OnClick(object sender, EventArgs e)
   {
      if(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
      {
         UpdateVvLookUpItem_4VvTextBox_Core(this, e);
      }
   }

   private void UpdateVvLookUpItem_OnDoubleClick(object sender, EventArgs e)
   {
      UpdateVvLookUpItem_4VvTextBox_Core(this, e);
   }

   public static void UpdateVvLookUpItem_4VvTextBox_Core(object sender, EventArgs e)
   {
      VvTextBox vvtb;

      if(sender is Button)
      {
         vvtb = ((Button)sender).Parent as VvTextBox;
      }
      else if(sender is VvTextBox)
      {
         vvtb = sender as VvTextBox;
      }
      else throw new Exception("UpdateVvLookUpItem_4VvTextBox_Core: sender nije Button niti VvTextBox");

      //================================================= 

      if(vvtb.JAM_IsInDataGridView == false)
      {
         VvUserControl.BtnChangeLookUpSelection_Click(vvtb, EventArgs.Empty);
      }
      else
      {
         VvDataGridView theGrid;

         if(ZXC.TheVvForm.TheVvDataRecord.IsPolyDocument)
         {
            theGrid = ZXC.TheVvForm.TheVvPolyDocumRecordUC.TheCurrentG;
         }
         else
         {
            theGrid = ZXC.TheVvForm.TheVvDocumentRecordUC.TheG;
         }

         theGrid.UpdateVvLookUpItem_4DataGridViewEnter_Core(vvtb);
      }

      //VvLookUpItem lui = VvUserControl.UpdateVvLookUpItem(this);

      //this.Focus();

      //if(lui != null)
      //{
      //   this.Text = lui.Cd;
      //   SendKeys.Send("{TAB}");
      //}
   }

   #endregion Eventi UpdateVvLookUpItem

   #endregion EventHandlers

   #region Methods

   public override string ToString()
   {
      return this.A0_JAM_Name + " [" + base.ToString() + "]";
   }


   public void JAM_Set_LookUpTable(VvLookUpLista luList, uint thisTextBoxTakesDataFromThisColIdx)
   {
      bool shouldAC = this.isInDataGridView == false;

      if(shouldAC) JAM_Set_LookUpTable(luList, thisTextBoxTakesDataFromThisColIdx, true );
      else         JAM_Set_LookUpTable(luList, thisTextBoxTakesDataFromThisColIdx, false);
   }
   /// <summary>
   /// Neku VvLookupLista postavi kao obavezan unos.
   /// </summary>
   /// <param rptLabel="luList"></param>
   /// <param rptLabel="thisTakesDataFromColIdx"></param>
   public void JAM_Set_LookUpTable(VvLookUpLista luList, uint thisTextBoxTakesDataFromThisColIdx, bool shouldAC)
   {
      JAM_LookUpList       = luList;
      JAM_LookUpTable      = CopyLookUpListColumnToStringArray(luList, thisTextBoxTakesDataFromThisColIdx);
      JAM_LookUpTableTitle = luList.Title;

      // 17.02.2020: dodajemo SVIM VvLookUp VvTextBox-ovima auocomplete funkcionalnost
      if(shouldAC)
      { 
         this.AutoCompleteMode         = AutoCompleteMode.SuggestAppend ;
         this.AutoCompleteSource       = AutoCompleteSource.CustomSource;
         this.AutoCompleteCustomSource = new AutoCompleteStringCollection();
         this.AutoCompleteCustomSource.AddRange(JAM_LookUpTable);
      }

   }

   /// <summary>
   /// Neku VvLookupLista postavi kao POMOCNI unos. + Sredi da NEMA 'Primaoca' tj. primaoc je on sam
   /// </summary>
   /// <param rptLabel="luList"></param>
   /// <param rptLabel="thisTakesDataFromColIdx"></param>
   public void JAM_Set_NOTobligatory_LookUpTable(VvLookUpLista luList, uint colIdx)
   {
      JAM_Set_LookUpTable(luList, colIdx);
      
      this.JAM_lookUp_NOTobligatory = true;
   }

   public void Force_OnExitSetLookUp_Name_As_JAM_FieldExitMethod()
   {
      this.JAM_FieldExitMethod = new EventHandler(OnExitSetLookUp_DataTakers);
   }

   //private void OnExitSetLookUp_DataTakers(object sender, EventArgs e)
   //{
   //   if(EditedHasChanges() == false) return;

   //   VvLookUpLista luList = null;

   //   // next lajn iz project dependent JAM_LookUpTable factory (virtuals or overriders are in VvForm_Q / VsrForm, ... 
   //   luList = ZXC.TheVvForm.Set_JAM_LookUpTable_ProjectDependent(JAM_LookUpTableTitle);

   //   if(luList != null)
   //   {
   //      VvLookUpItem lui = luList.VvFindByCd(this.Text);

   //      if(lui != null) lookUpNameTakerVvTb.Text = lui.Name;
   //      else            lookUpNameTakerVvTb.Text = "";
   //   }
   //   else
   //   {
   //      lookUpNameTakerVvTb.Text = "";
   //   }
   //}

   // 09.11.2011: 
   private Control GetRootParent(Control control)
   {
      if(control.Parent == null) return control;

      return GetRootParent(control.Parent); // REKURZIJA ! 
   }

   private void OnExitSetTwin_DataTaker(object sender, EventArgs e)
   {
      if(EditedHasChanges() == false) return;

      Control rootCtrl = GetRootParent(this);

      VvTextBox twinTextBox = GetTwinVvTextBoxTaker(/*ZXC.TheVvForm.TheVvUC*/rootCtrl, this.JAM_twinTaker_JAM_Name); // 09.11.2011: jer kada je twinTB na VvRecListUC-u onda je 'ZXC.TheVvForm.TheVvUC' krivi rootParent
      
      if(twinTextBox != null) twinTextBox.Text = this.Text;
   }

   public VvTextBox GetTwinVvTextBoxTaker(Control thisControl, string twinTaker_JAM_Name)
   {
      VvTextBox twinTextBox = null;

      if(thisControl is VvTextBox && ((VvTextBox)thisControl).JAM_Name == twinTaker_JAM_Name) return thisControl as VvTextBox;

      // REKURZIJA 
      foreach(Control childControl in thisControl.Controls)
      {
         twinTextBox = GetTwinVvTextBoxTaker(childControl, twinTaker_JAM_Name);

         if(twinTextBox != null) break;
      }

      return twinTextBox;
   }

   public void JAM_PairThisWithTwin(VvTextBox twinTextBox)
   {
      this       .JAM_twinTaker_JAM_Name = twinTextBox.JAM_Name;
      twinTextBox.JAM_twinTaker_JAM_Name = this       .JAM_Name;
   }

   private void OnExitSetLookUp_DataTakers(object sender, EventArgs e)
   {
      if(EditedHasChanges() == false) return;

      if(JAM_IsInDataGridView)
      {
         VvDataGridView theGrid;

         if(ZXC.TheVvForm.TheVvDataRecord.IsPolyDocument)
         {
            theGrid = ZXC.TheVvForm.TheVvPolyDocumRecordUC.TheCurrentG;
         }
         else
         {
            theGrid = ZXC.TheVvForm.TheVvDocumentRecordUC.TheG;
         }

         this.JAM_ChosenLookUpItem = this.JAM_LookUpList.GetLuiForThisCd(theGrid.EditingControl.Text);

         // 19.10.2011: 
         if(this.JAM_ChosenLookUpItem == null) // treba traziti po Name-u? 
         {
            string luiCD = this.JAM_LookUpList.GetCdForThisName(theGrid.EditingControl.Text);

            this.JAM_ChosenLookUpItem = this.JAM_LookUpList.GetLuiForThisCd(luiCD);
         }

         theGrid.SetEventualLookupDataTakers(this);
      }
      else
      {
         if(this.JAM_LookUpList != null)
         {
            this.JAM_ChosenLookUpItem = this.JAM_LookUpList.GetLuiForThisCd(this.Text);
         }

         SetEventualLookupDataTakers(this);
      }

      //if(chosenLui != null) lookUpNameTakerVvTb.Text = chosenLui.Name;
      //else                  lookUpNameTakerVvTb.Text = "";
   }

   private static void SetEventualLookupDataTakers(VvTextBox vvtb)
   {
      VvLookUpItem chosenLui = vvtb.JAM_ChosenLookUpItem;

      VvTextBox siblingTextBox;

      int sendTabCount = -1; // prvi sendTab obavlja pozivaoc ove metode (dva su), a eventualne ostale ova metoda.

      // 08092010: abrakadadbra?! 
      int readOnlyTakerCount = 0;

      if(vvtb.JAM_ttNumTaker_JAM_Name.NotEmpty())
      {
         // 15.12.2010
         string eventualSkladCD = (ZXC.TheVvForm.TheVvRecordUC is FakturDUC ? ((FakturDUC)ZXC.TheVvForm.TheVvRecordUC).Fld_SkladCD : null);

         uint ttNum = ZXC.TheVvForm.TheVvDao.GetNextTtNum(ZXC.TheVvForm.TheDbConnection, ((VvDocumentRecord)ZXC.TheVvForm.TheVvDataRecord).VirtualTT, eventualSkladCD);

         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_ttNumTaker_JAM_Name);
         siblingTextBox.Text = ttNum.ToString("0000");

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      // 26.10.2010:
      //if(chosenLui == null) return;

      if(vvtb.JAM_lui_CdTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_CdTaker_JAM_Name);

         siblingTextBox.Text = (chosenLui == null ? "" : chosenLui.Cd);

         if(siblingTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
         {
            VvTextBox twinTextBox = siblingTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, siblingTextBox.JAM_twinTaker_JAM_Name);

            if(twinTextBox != null) twinTextBox.Text = siblingTextBox.Text;
         }

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      if(vvtb.JAM_lui_NameTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_NameTaker_JAM_Name);

         siblingTextBox.Text = (chosenLui == null ? "" : chosenLui.Name);

         if(siblingTextBox.JAM_twinTaker_JAM_Name.NotEmpty())
         {
            VvTextBox twinTextBox = siblingTextBox.GetTwinVvTextBoxTaker(ZXC.TheVvForm.TheVvUC, siblingTextBox.JAM_twinTaker_JAM_Name);

            if(twinTextBox != null) twinTextBox.Text = siblingTextBox.Text;
         }

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      if(vvtb.JAM_lui_NumberTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_NumberTaker_JAM_Name);
         siblingTextBox.PutDecimalField((chosenLui == null ? 0.00M : chosenLui.Number));

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      /*Vv*/CheckBox siblingCheckBox;
      if(vvtb.JAM_lui_FlagTaker_JAM_Name.NotEmpty())
      {
         siblingCheckBox = GetSiblingCheckBoxTaker(vvtb, vvtb.JAM_lui_FlagTaker_JAM_Name);
         siblingCheckBox.Checked = ((chosenLui == null ? false : chosenLui.Flag));

         //if(siblingCheckBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      if(vvtb.JAM_lui_IntegerTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_IntegerTaker_JAM_Name);
         siblingTextBox.PutIntField((chosenLui == null ? 0 : chosenLui.Integer));

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      if(vvtb.JAM_lui_UintegerTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_UintegerTaker_JAM_Name);
         siblingTextBox.PutUintField((chosenLui == null ? 0 : chosenLui.Uinteger));

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      if(vvtb.JAM_lui_DateTaker_JAM_Name.NotEmpty())
      {
         siblingTextBox = GetSiblingVvTextBoxTaker(vvtb, vvtb.JAM_lui_DateTaker_JAM_Name);
         siblingTextBox.Text = (chosenLui == null ? "" : chosenLui.DateT.ToString(ZXC.VvDateFormat));

         if(siblingTextBox.JAM_ReadOnly == true) readOnlyTakerCount++;
         sendTabCount++;
      }

      //08092010: abrakadabra?!
      if(readOnlyTakerCount > 1) sendTabCount--;

      for(int i = 0; i < sendTabCount; ++i) SendKeys.Send("{TAB}");
   }

   private static VvTextBox GetSiblingVvTextBoxTaker(VvTextBox vvtbSibling, string fieldName) // The Linq! 
   {
      return vvtbSibling.Parent.Controls.OfType<VvTextBox>().Single(vvtb => vvtb.JAM_Name == fieldName);
   }

   private static /*Vv*/CheckBox GetSiblingCheckBoxTaker(VvTextBox vvtbSibling, string fieldName) // The Linq! 
   {
      return vvtbSibling.Parent.Controls.OfType</*Vv*/CheckBox>().Single(vvChkBox => vvChkBox.Name == fieldName);
   }

   /// <summary>
   /// Neku VvLookupLista postavi kao pomocni unos.
   /// </summary>
   /// <param rptLabel="luList"></param>
   /// <param rptLabel="thisTakesDataFromColIdx"></param>
   public void JAM_Set_HelpTable(VvLookUpLista luList, uint colIdx)
   {
      JAM_HelpTable = CopyLookUpListColumnToStringArray(luList, colIdx);
   }

   private string[] CopyLookUpListColumnToStringArray(VvLookUpLista luList, uint colIdx)
   {
      luList.LazyLoad();

      // !!!: 
      this.JAM_lookUpTableColIdx = colIdx;

      string[] aStr = new string[luList.Count];

      int i = 0;
      foreach(VvLookUpItem luItem in luList)
      {
         aStr[i++] = LookUpItemMemberAsString(luItem, colIdx);
      }

      return aStr;
   }

   private string LookUpItemMemberAsString(VvLookUpItem luItem, uint colIdx)
   {
      switch(colIdx)
      {
         // od 02.06.2009: 
         //case  0: return (luItem.Name);
         //case  1: return (luItem.Cd);
         case  0: return (luItem.Cd);
         case  1: return (luItem.Name);
         default: return "";
      }
   }

   public void JAM_SetAutoCompleteData(string recordName, VvSQL.SorterType sorterType, EventHandler onVvTextBoxEnter_SetAutocomplete, EventHandler onVvTextBoxLeave_SetDependendTakers)
   {
      JAM_SetAutoCompleteData(recordName, sorterType, ZXC.AutoCompleteRestrictor.No_Restrictions, onVvTextBoxEnter_SetAutocomplete, onVvTextBoxLeave_SetDependendTakers);
   }

   public void JAM_SetAutoCompleteData(string                     recordName,
                                       VvSQL.SorterType           sorterType,
                                       ZXC.AutoCompleteRestrictor restrictor,
                                       EventHandler               onVvTextBoxEnter_SetAutocomplete,
                                       EventHandler               onVvTextBoxLeave_SetDependendTakers)
   {


      this.JAM_IsAutocomplete = true;

      // ne, pusti da JAM_IsAutocomplete to obavi jer inace u DGV-u ne radi pri TakeJAM_MembersFrom 
      //this.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
      //this.AutoCompleteSource = AutoCompleteSource.CustomSource;

      this.autoCompleteRecordName = recordName;
      this.autoCompleteSorterType = sorterType;
      this.autoCompleteRestrictor = restrictor;

      this.JAM_FieldEntryMethod = new EventHandler(onVvTextBoxEnter_SetAutocomplete);

      if(onVvTextBoxLeave_SetDependendTakers != null)
         this.JAM_FieldExitMethod = new EventHandler(onVvTextBoxLeave_SetDependendTakers);
   }

   public void JAM_SetOtsInfoData(/*EventHandler onVvTextBoxLeave_SetDependendTakers*/)
   {
      this.JAM_IsOtsInfo = true;

      //if(onVvTextBoxLeave_SetDependendTakers != null)
      //   this.JAM_FieldExitMethod = new EventHandler(onVvTextBoxLeave_SetDependendTakers);
   }

   private void ChangeStandardTextBoxContextMenu(string menuText, EventHandler updateMethod)
   {
      //if(A0_JAM_Name.Contains("vvtb4ColT_konto")) ZXC.aim_emsg("trlababa [{0}]", this.A0_JAM_Name);

      if(menuText.IsEmpty())
      {
         this.ContextMenu = new VvStandardTextBoxContextMenu();
      }
      else
      {
         this.ContextMenu =
            new VvStandardTextBoxContextMenu(new MenuItem[] 
            { 
               new MenuItem(menuText, updateMethod),
               new MenuItem("-"),
               new MenuItem("-")
            });
      }

      //this.ContextMenuStrip.Items.Add(menuText, null, updateMethod);
      //this.ContextMenuStrip = new ContextMenuStrip(new MenuItem[] { new MenuItem(menuText, updateMethod) } );
   }

   /// <summary>
   /// Da VvTextBox-u vlastiti text kao ToolTip (npr. u slucaju da ne stane cijeli...)
   /// </summary>
   /// <param name="tt"></param>
   public void TextAsToolTip(ToolTip tt)
   {
      tt.SetToolTip(this, this.Text + "\n(" + this.JAM_StatusText + ")");
   }

   /// <summary>
   /// Bu True ako (this.Text != null && this.Text != "")
   /// </summary>
   /// <returns></returns>
   public bool IsNonEmpty()
   {
      return (this.Text != null && this.Text != "");
   }

   /// <summary>
   /// Bu True ako (this.Text == null || this.Text == "")
   /// </summary>
   /// <returns></returns>
   public bool IsEmpty()
   {
      return (!IsNonEmpty());
   }

   private decimal CalcMaxNumericValue_BasedOnMaxLength()
   {
      int len = this.MaxLength;
      decimal maxValue;

      if(JAM_NumberOfDecimalPlaces > 0) len -= JAM_NumberOfDecimalPlaces + 1;

      if(len < 1) throw new Exception(this.Name  +" MaxLength " + MaxLength + " premali za " + JAM_NumberOfDecimalPlaces+" decimale u CalcMaxNumericValue_BasedOnMaxLength()!");

      // 02.10.2012: tu si dodao 17, 18, 19, pa ko zna jel dobro?! 
      switch(len)
      {
         case  1: maxValue = !JAM_AddCommas ? 9                     : 9; break;
         case  2: maxValue = !JAM_AddCommas ? 99                    : 99; break;
         case  3: maxValue = !JAM_AddCommas ? 999                   : 999; break;
         case  4: maxValue = !JAM_AddCommas ? 9999                  : 999; break;
         case  5: maxValue = !JAM_AddCommas ? 99999                 : 9999; break;
         case  6: maxValue = !JAM_AddCommas ? 999999                : 99999; break;
         case  7: maxValue = !JAM_AddCommas ? 9999999               : 999999; break;
         case  8: maxValue = !JAM_AddCommas ? 99999999              : 999999; break;
         case  9: maxValue = !JAM_AddCommas ? 999999999             : 9999999; break;
         case 10: maxValue = !JAM_AddCommas ? 9999999999            : 99999999; break;
         case 11: maxValue = !JAM_AddCommas ? 99999999999           : 999999999; break;
         case 12: maxValue = !JAM_AddCommas ? 999999999999          : 999999999; break;
         case 13: maxValue = !JAM_AddCommas ? 9999999999999         : 9999999999; break;
         case 14: maxValue = !JAM_AddCommas ? 99999999999999        : 99999999999; break;
         case 15: maxValue = !JAM_AddCommas ? 999999999999999       : 999999999999; break;
         case 16: maxValue = !JAM_AddCommas ? 9999999999999999      : 999999999999; break;
         case 17: maxValue = !JAM_AddCommas ? 99999999999999999     : 9999999999999; break;
         case 18: maxValue = !JAM_AddCommas ? 999999999999999999    : 99999999999999; break;
         case 19: maxValue = !JAM_AddCommas ? 9999999999999999999   : 999999999999999; break;
         default: throw new Exception(this.Name + " len " + len + " prevelik u CalcMaxNumericValue_BasedOnMaxLength()!");
      }

      switch(JAM_NumberOfDecimalPlaces)
      {
         case 0: break;
         case 1: maxValue += 0.9M; break;
         case 2: maxValue += 0.99M; break;
         case 3: maxValue += 0.999M; break;
         case 4: maxValue += 0.9999M; break;
         case 5: maxValue += 0.99999M; break;
         case 6: maxValue += 0.999999M; break;
         case 7: maxValue += 0.9999999M; break;
         case 8: maxValue += 0.99999999M; break;
         default: throw new Exception(this.Name + " JAM_NumberOfDecimalPlaces " + JAM_NumberOfDecimalPlaces + " prevelik u CalcMaxNumericValue_BasedOnMaxLength()!");
      }

      return maxValue;
   }

   private decimal CalcMinNumericValue_BasedOnMaxLength()
   {
      int len = this.MaxLength;
      decimal minValue;

      if(JAM_NumberOfDecimalPlaces > 0) len -= JAM_NumberOfDecimalPlaces + 1;

      if(len < 1) throw new Exception(this.Name + " MaxLength " + MaxLength + " premali za " + JAM_NumberOfDecimalPlaces + " decimale u CalcMaxNumericValue_BasedOnMaxLength()!");

      // 02.10.2012: tu si dodao 17, 18, 19, pa ko zna jel dobro?! 
      switch(len)
      {
         case  1: minValue = 0; break;
         case  2: minValue = !JAM_AddCommas ? -9                  : -9; break;
         case  3: minValue = !JAM_AddCommas ? -99                 : -99; break;
         case  4: minValue = !JAM_AddCommas ? -999                : -999; break;
         case  5: minValue = !JAM_AddCommas ? -9999               : -999; break;
         case  6: minValue = !JAM_AddCommas ? -99999              : -9999; break;
         case  7: minValue = !JAM_AddCommas ? -999999             : -99999; break;
         case  8: minValue = !JAM_AddCommas ? -9999999            : -999999; break;
         case  9: minValue = !JAM_AddCommas ? -99999999           : -999999; break;
         case 10: minValue = !JAM_AddCommas ? -999999999          : -9999999; break;
         case 11: minValue = !JAM_AddCommas ? -9999999999         : -99999999; break;
         case 12: minValue = !JAM_AddCommas ? -99999999999        : -999999999; break;
         case 13: minValue = !JAM_AddCommas ? -999999999999       : -999999999; break;
         case 14: minValue = !JAM_AddCommas ? -9999999999999      : -9999999999; break;
         case 15: minValue = !JAM_AddCommas ? -99999999999999     : -99999999999; break;
         case 16: minValue = !JAM_AddCommas ? -999999999999999    : -999999999999; break;
         case 17: minValue = !JAM_AddCommas ? -99999999999999999  : -9999999999999; break;
         case 18: minValue = !JAM_AddCommas ? -999999999999999999 : -99999999999999; break;
         case 19: minValue = !JAM_AddCommas ? -999999999999999999 : -99999999999999; break;
         default: throw new Exception(this.Name + " len " + len + " prevelik u CalcMinNumericValue_BasedOnMaxLength()!");
      }

      switch(JAM_NumberOfDecimalPlaces)
      {
         case 0: break;
         case 1: minValue -= 0.9M; break;
         case 2: minValue -= 0.99M; break;
         case 3: minValue -= 0.999M; break;
         case 4: minValue -= 0.9999M; break;
         case 5: minValue -= 0.99999M; break;
         case 6: minValue -= 0.999999M; break;
         case 7: minValue -= 0.9999999M; break;
         case 8: minValue -= 0.99999999M; break;
         default: throw new Exception(this.Name + " JAM_NumberOfDecimalPlaces " + JAM_NumberOfDecimalPlaces + " prevelik u CalcMaxNumericValue_BasedOnMaxLength()!");
      }

      return minValue;
   }

   /// <summary>
   /// IsNumeric = true, NumOfDecimalPLaces = param, TextAlignment, KeyPress, ...
   /// maxValue postavi na 'decimal.MinValue' ako zelis da sam izracuna basedOnMaxLength
   /// </summary>
   /// <param rptLabel="numberOfDecimalPlaces"></param>
   public void JAM_MarkAsNumericTextBox()
   {
      JAM_MarkAsNumericTextBox(VvTextBox.defaultNumberOfDecimalPlaces, VvTextBox.defaultOceNeceZareze, decimal.MaxValue, decimal.MinValue, VvTextBox.defaultClearIfZero);
   }

   /// <summary>
   /// IsNumeric = true, NumOfDecimalPLaces = param, TextAlignment, KeyPress, ...
   /// maxValue postavi na 'decimal.MinValue' ako zelis da sam izracuna basedOnMaxLength
   /// </summary>
   /// <param rptLabel="numberOfDecimalPlaces"></param>
   public void JAM_MarkAsNumericTextBox(int numberOfDecimalPlaces, bool oceNeceCommas)
   {
      JAM_MarkAsNumericTextBox(numberOfDecimalPlaces, oceNeceCommas, decimal.MaxValue, decimal.MinValue, VvTextBox.defaultClearIfZero);
   }

   /// <summary>
   /// IsNumeric = true, NumOfDecimalPLaces = param, TextAlignment, KeyPress, ...
   /// maxValue postavi na 'decimal.Zero' ako zelis da sam izracuna basedOnMaxLength
   /// minValue postavi na 'decimal.MaxValue' ako zelis da sam izracuna basedOnMaxLength
   /// </summary>
   /// <param rptLabel="numberOfDecimalPlaces"></param>
   public void JAM_MarkAsNumericTextBox(int numberOfDecimalPlaces, bool oceNeceGroupSeparator, decimal minValue, decimal maxValue, bool clearIfZero)
   {
      JAM_NumberOfDecimalPlaces  = numberOfDecimalPlaces;
      JAM_AddCommas              = oceNeceGroupSeparator;
      JAM_IsNumericTextBox       = true;
      JAM_MaxNumericValue        = maxValue;
      //JAM_MinNumericValue        = CalcMinNumericValue_BasedOnMaxLength();
      JAM_MinNumericValue        = minValue;
      JAM_ClearIfZero            = clearIfZero;

      TextAlign      = HorizontalAlignment.Right;
      JAM_CharEdits  = ZXC.JAM_CharEdits.NumericOnly;
   }

   public void PutSomeRecIDField(uint num)
   {
      this.Text = num.ToString("000000;;#");
   }

   public uint GetSomeRecIDField()
   {
      return ZXC.ValOrZero_UInt(this.Text);
   }

   public void PutShortField(short num)
   {
      this.Text = num.ToString("0;;#");
   }

   public short GetShortField()
   {
      return ZXC.ValOrZero_Short(this.Text);
   }

   public void PutUintField(uint num)
   {
      this.Text = num.ToString("0;;#");
   }

   public uint GetUintField()
   {
      return ZXC.ValOrZero_UInt(this.Text);
   }

   public ushort GetUshortField()
   {
      return ZXC.ValOrZero_Ushort(this.Text);
   }

   public void PutIntField(int num)
   {
      this.Text = num.ToString("0;;#");
   }

   public void PutUshortField(ushort num)
   {
      this.Text = num.ToString("0;;#");
   }

   public int GetIntField()
   {
      return ZXC.ValOrZero_Int(this.Text);
   }

   public void PutDecimalField(decimal dNum)
   {
      if(this.JAM_ClearIfZero == true && dNum.Equals(decimal.Zero))
      {
         this.Text = "";
      }
      else
      {
         string standardNumericFormatString;
         decimal divider;

         if(this.JAM_IsForPercent == true)
         {
            standardNumericFormatString = "P";
            divider = 100.00M;
         }
         else
         {
            standardNumericFormatString = "N";
            divider = 1.00M;
         }

         if(this.JAM_AddCommas == true)
         {
            this.Text = (dNum / divider).ToString(standardNumericFormatString, ZXC.GetNumberFormatInfo(this.JAM_NumberOfDecimalPlaces));
         }
         else
         {
            System.Globalization.NumberFormatInfo nfi = ZXC.GetNumberFormatInfo(this.JAM_NumberOfDecimalPlaces);
            string originalGroupSeparator = nfi.NumberGroupSeparator;
            nfi.NumberGroupSeparator = "";
            this.Text = (dNum / divider).ToString(standardNumericFormatString, ZXC.GetNumberFormatInfo(this.JAM_NumberOfDecimalPlaces));
            nfi.NumberGroupSeparator = originalGroupSeparator;
         }
      }
   }

   public decimal GetDecimalField()
   {
      string textToParse;

      if(this.JAM_IsForPercent) textToParse = this.Text.TrimEnd('%');
      else                       textToParse = this.Text;

      return ZXC.ValOrZero_Decimal(textToParse, this.JAM_NumberOfDecimalPlaces);
   }

   public bool JAM_ValidatingActions()
   {
      //...JAM_LookUpTable provjera...............................................................
      if(this.JAM_lookUp_NOTobligatory == false && this.JAM_LookUpTable != null && this.IsNonEmpty())
      {
         if(!((System.Collections.IList)this.JAM_LookUpTable).Contains(this.Text))
         {
            ZXC.RaiseErrorProvider(this, "Podatak [" + this.Text + "] ne postoji u listi dozvoljenih podataka (" + this.JAM_LookUpTableTitle + ")");
            return true; // za e.Cancel 
         }
      } //----------------------------------------------------------------------------------------

      //...JAM_Min And MAX NumericValue provjera..................................................
      if(this.JAM_IsNumericTextBox && this.IsNonEmpty())
      {
         decimal dNum;

         try
         {
            string textToParse;

            if(this.JAM_IsForPercent)  textToParse = this.Text.TrimEnd('%');
            else                       textToParse = this.Text;

            dNum = decimal.Parse(textToParse, System.Globalization.NumberStyles.Number, ZXC.GetNumberFormatInfo(this.JAM_NumberOfDecimalPlaces));
         }
         catch(System.FormatException)
         {
            ZXC.RaiseErrorProvider(this, "Nemogu pretvoriti izraz [" + this.Text + "] u decimalni broj!");
            return true; // za e.Cancel 
         }

         if(dNum > this.JAM_MaxNumericValue)
         {
            ZXC.RaiseErrorProvider(this, "Iznos [" + this.Text + "] prevelik! Max [" + this.JAM_MaxNumericValue + "].");
            return true; // za e.Cancel 
         }
         if(dNum < this.JAM_MinNumericValue)
         {
            ZXC.RaiseErrorProvider(this, "Iznos [" + this.Text + "] premali! Min [" + this.JAM_MinNumericValue + "].");
            return true; // za e.Cancel 
         }

         //19.12.2011:
         int decimalPlaces;
         if((decimalPlaces = ZXC.GetNumOfUpisanihDecimala(dNum)) > this.JAM_NumberOfDecimalPlaces)
         {
            ZXC.RaiseErrorProvider(this, "Duljina decimalnih mjesta [" + decimalPlaces + "] prevelika! Max [" + this.JAM_NumberOfDecimalPlaces + "] mjesta.");
            return true; // za e.Cancel 
         }


      } //----------------------------------------------------------------------------------------

      //...JAM_DataRequired.......................................................................
      if(this.JAM_DataRequired == true && this.IsEmpty() && /* 11.11.2011:*/ this.Visible == true)
      {
         ZXC.RaiseErrorProvider(this, "Polje mora sadrzavati neki podatak!");
         return true; // za e.Cancel 
      } //----------------------------------------------------------------------------------------

      //...JAM_MustFill...........................................................................
      if(this.JAM_MustFill == true && this.IsNonEmpty() && this.Text.Length != this.MaxLength)
      {
         ZXC.RaiseErrorProvider(this, "Mora se ispuniti svih [" + this.MaxLength + "] znakova!");
         return true; // za e.Cancel 
      } //----------------------------------------------------------------------------------------

      //...JAM_MustTabOutBeforeSubmit.............................................................
      if(this.JAM_MustTabOutBeforeSubmit == true)
      {
         if(editInProgress)
         {
            if(EditedHasChanges())
            {
               //ZXC.RaiseErrorProvider(this, "TAB out needed!");
               ZXC.RaiseErrorProvider(this, "TAB OUT NEEDED!\n\nMolim, potvrdite sadržaj izlaskom iz polja tipkom <Tab> ili <Enter> ili mišem.");
               //   SendKeys.Send("{TAB}"); ... NE, NE, NE! zaboravi! 
               return true; // za e.Cancel 
            }
         }
      } //----------------------------------------------------------------------------------------

      return false;
   }
   
   public void TakeJAM_MembersFrom(VvTextBox vvtbJAM_MemberProvider)
   {
      this.MaxLength = vvtbJAM_MemberProvider.MaxLength;
      this.Name      = "DGV_Exposed_" + vvtbJAM_MemberProvider.Name;

      this.lookUpList               = vvtbJAM_MemberProvider.lookUpList;
      this.helpTable                = vvtbJAM_MemberProvider.helpTable;
      this.lookUpTable              = vvtbJAM_MemberProvider.lookUpTable;
      this.lookUpTableTitle         = vvtbJAM_MemberProvider.lookUpTableTitle;
      this.readOnly                 = vvtbJAM_MemberProvider.readOnly;
      this.writeOnly                = vvtbJAM_MemberProvider.writeOnly;
      this.isForCheckBox            = vvtbJAM_MemberProvider.isForCheckBox;
      this.isForDateTimePicker      = vvtbJAM_MemberProvider.isForDateTimePicker;
      this.isForDateTimePicker_WithTimeDisplay = vvtbJAM_MemberProvider.isForDateTimePicker_WithTimeDisplay;
      this.isForDateTimePicker_TimeOnlyDisplay = vvtbJAM_MemberProvider.isForDateTimePicker_TimeOnlyDisplay;
      this.isForDateTimePicker_YearOnly        = vvtbJAM_MemberProvider.isForDateTimePicker_YearOnly;
      this.lookUp_NOTobligatory     = vvtbJAM_MemberProvider.lookUp_NOTobligatory;
      this.statusText               = vvtbJAM_MemberProvider.statusText;
      this.JAM_CharacterCasing      = vvtbJAM_MemberProvider.JAM_CharacterCasing;
//    this.JAM_LowerCase            = vvtbJAM_MemberProvider.lowerCase;
      this.JAM_NonDisplay           = vvtbJAM_MemberProvider.nonDisplay;
      this.JAM_ReverseVideo         = vvtbJAM_MemberProvider.reverseVideo;
      this.JAM_Highlighted          = vvtbJAM_MemberProvider.highlighted;
      this.fillCharacter            = vvtbJAM_MemberProvider.fillCharacter;
      this.isNumericTextBox         = vvtbJAM_MemberProvider.isNumericTextBox;
      this.isForPostotak            = vvtbJAM_MemberProvider.isForPostotak;
      this.isSupressTab             = vvtbJAM_MemberProvider.isSupressTab;
      this.isOnEndEditJump2NextRow  = vvtbJAM_MemberProvider.isOnEndEditJump2NextRow;
      this.numberOfDecimalPlaces    = vvtbJAM_MemberProvider.numberOfDecimalPlaces;
      this.minNumericValue          = vvtbJAM_MemberProvider.minNumericValue;
      this.maxNumericValue          = vvtbJAM_MemberProvider.maxNumericValue;
      this.addCommas                = vvtbJAM_MemberProvider.addCommas;
      this.clearIfZero              = vvtbJAM_MemberProvider.clearIfZero;
      this.mustFill                 = vvtbJAM_MemberProvider.mustFill;
      this.dataRequired             = vvtbJAM_MemberProvider.dataRequired;
      this.charEdits                = vvtbJAM_MemberProvider.charEdits;
      this.isInDataGridView         = vvtbJAM_MemberProvider.JAM_IsInDataGridView;
      this.JAM_FieldEntryMethod     = vvtbJAM_MemberProvider.JAM_FieldEntryMethod;
      this.JAM_FieldExitMethod      = vvtbJAM_MemberProvider.JAM_FieldExitMethod;
      this.JAM_FieldExitMethod_2    = vvtbJAM_MemberProvider.JAM_FieldExitMethod_2;
      this.JAM_FieldExitMethod_3    = vvtbJAM_MemberProvider.JAM_FieldExitMethod_3;
      this.valueType                = vvtbJAM_MemberProvider.valueType;
      this.shouldCopyPrevRow        = vvtbJAM_MemberProvider.shouldCopyPrevRow;
      this.shouldCopyPrevRowUnCond  = vvtbJAM_MemberProvider.shouldCopyPrevRowUnCond;
      this.shouldSumGrid            = vvtbJAM_MemberProvider.shouldSumGrid;
      this.shouldCalcTrans          = vvtbJAM_MemberProvider.shouldCalcTrans;
      this.JAM_IsAutocomplete       = vvtbJAM_MemberProvider.isAutocomplete;
      this.JAM_IsOtsInfo            = vvtbJAM_MemberProvider.isOtsInfo;
      this.JAM_IsPassword           = vvtbJAM_MemberProvider.isPasswd;
      this.AutoCompleteCustomSource = vvtbJAM_MemberProvider.AutoCompleteCustomSource;
      this.ContextMenu              = vvtbJAM_MemberProvider.ContextMenu;
      this.autoCompleteRecordName   = vvtbJAM_MemberProvider.autoCompleteRecordName;
      this.autoCompleteSorterType   = vvtbJAM_MemberProvider.autoCompleteSorterType;
      this.autoCompleteRestrictor   = vvtbJAM_MemberProvider.autoCompleteRestrictor;
      this.allowedCharacters        = vvtbJAM_MemberProvider.allowedCharacters;
      this.otsKonto                 = vvtbJAM_MemberProvider.otsKonto;
      this.otsKupdobCd              = vvtbJAM_MemberProvider.otsKupdobCd;
      this.otsDateDo                = vvtbJAM_MemberProvider.otsDateDo;
      this.otsMoney                 = vvtbJAM_MemberProvider.otsMoney;
      this.mustTabOutBeforeSubmit   = vvtbJAM_MemberProvider.JAM_MustTabOutBeforeSubmit;
      this.JAM_DisableUpdateVvDataRecordActions  = vvtbJAM_MemberProvider.JAM_DisableUpdateVvDataRecordActions;
      this.JAM_DisableUpdateVvLookUpItemActions  = vvtbJAM_MemberProvider.JAM_DisableUpdateVvLookUpItemActions;

      this.JAM_ResultBox            = vvtbJAM_MemberProvider.resultBox;
      this.lookUp_MultiSelection    = vvtbJAM_MemberProvider.lookUp_MultiSelection;
      this.lookUpTableColIdx        = vvtbJAM_MemberProvider.lookUpTableColIdx;

      this.twinTaker_JAM_Name        = vvtbJAM_MemberProvider.JAM_twinTaker_JAM_Name;
      this.ttNumTaker_JAM_Name       = vvtbJAM_MemberProvider.JAM_ttNumTaker_JAM_Name;
      this.lui_CdTaker_JAM_Name      = vvtbJAM_MemberProvider.JAM_lui_CdTaker_JAM_Name;
      this.lui_NameTaker_JAM_Name    = vvtbJAM_MemberProvider.JAM_lui_NameTaker_JAM_Name;
      this.lui_NumberTaker_JAM_Name  = vvtbJAM_MemberProvider.JAM_lui_NumberTaker_JAM_Name;
      this.lui_FlagTaker_JAM_Name    = vvtbJAM_MemberProvider.JAM_lui_FlagTaker_JAM_Name;
      this.lui_IntegerTaker_JAM_Name = vvtbJAM_MemberProvider.JAM_lui_IntegerTaker_JAM_Name;
      this.lui_UintegerTaker_JAM_Name= vvtbJAM_MemberProvider.JAM_lui_UintegerTaker_JAM_Name;
      this.lui_DateTaker_JAM_Name    = vvtbJAM_MemberProvider.JAM_lui_DateTaker_JAM_Name;
      this.backColor                 = vvtbJAM_MemberProvider.backColor;
      this.foreColor                 = vvtbJAM_MemberProvider.foreColor;
      this.nextTextBox               = vvtbJAM_MemberProvider.nextTextBox;
      this.VVtag2                    = vvtbJAM_MemberProvider.VVtag2;

      //________ VvLookuUpButton in VvDataGridView shit ___ start __________ 

      // 19.10.2011: Nakon sati i sati debuggiranja otkrili da ovo 'this.vvLookUpButton = vvtbJAM_MemberProvider.vvLookUpButton'   
      // stvara veeeeliki bug, te odlucili da VvLookUpTextBox na DGV-u nece imati lookupButton, nego samo klasiku kao i findRecord 
      // 21.10.2011: mozda ipak nije bug, nego je probleme stvaro VvDocumentRecordUC.grid_MouseMove() !!! 

      this.Controls.Clear();
      this.vvLookUpButton = vvtbJAM_MemberProvider.vvLookUpButton;
      if(this.vvLookUpButton != null)
      {
         SetLookUpButton_ParentAnchorSizeLocation(this, false);
      }
      //________ VvLookuUpButton in VvDataGridView shit ___ end ____________ 

      // 17.02.2020: 
      if(vvtbJAM_MemberProvider.AutoCompleteMode != AutoCompleteMode.None)
      { 
         this.AutoCompleteMode         = vvtbJAM_MemberProvider.AutoCompleteMode        ;
         this.AutoCompleteSource       = vvtbJAM_MemberProvider.AutoCompleteSource      ;
         this.AutoCompleteCustomSource = vvtbJAM_MemberProvider.AutoCompleteCustomSource;
         this.AutoCompleteCustomSource = vvtbJAM_MemberProvider.AutoCompleteCustomSource;
      }
   }

   public static VvTextBox VvTextBoxLookUpFactory(string name, string text, int maxLength, int numOfMergedHeights)
   {
      VvTextBox tbxLookUp   = new VvTextBox();
      tbxLookUp.AutoSize    = false;
      tbxLookUp.TextAlign   = HorizontalAlignment.Left;
      tbxLookUp.MaxLength   = maxLength;
     // tbxLookUp.Text        = text;
      tbxLookUp.BorderStyle = BorderStyle.Fixed3D;
      tbxLookUp.Font        = ZXC.vvFont.BaseFont;
      tbxLookUp.Name        = name;
      tbxLookUp.JAM_StatusText = text;

      CreateLookUp_Button(tbxLookUp, (numOfMergedHeights > 0));

      return tbxLookUp;
   }

   private static void CreateLookUp_Button(VvTextBox tbxLookUp, bool isMultiline)
   {
      tbxLookUp.vvLookUpButton = new Button();

      SetLookUpButton_ParentAnchorSizeLocation(tbxLookUp, isMultiline);

      tbxLookUp.vvLookUpButton.BackColor  = SystemColors.InactiveCaption;
      tbxLookUp.vvLookUpButton.ForeColor  = SystemColors.Desktop;
      tbxLookUp.vvLookUpButton.FlatStyle  = FlatStyle.Popup;
      // 13.12.2012: 
    //tbxLookUp.vvLookUpButton.Font       = new Font("Wingdings 3", ZXC.vvFont.LargeLargeFont.Size, FontStyle.Bold);
    //tbxLookUp.vvLookUpButton.Text       = ",";
    // 26.04.2016. nakon Application.SetCompatibleTextRenderingDefault(false);
    //tbxLookUp.vvLookUpButton.Font       = new Font("Arial", ZXC.vvFont.LargeFont.Size, FontStyle.Bold);
      tbxLookUp.vvLookUpButton.Font       = new Font("Arial", ZXC.vvFont.SmallSmallFont.Size , FontStyle.Bold);
      tbxLookUp.vvLookUpButton.Text       = "<";
      tbxLookUp.vvLookUpButton.TextAlign  = ContentAlignment.MiddleCenter;
      tbxLookUp.vvLookUpButton.Cursor     = Cursors.Arrow;
      tbxLookUp.vvLookUpButton.TabStop    = false;
      tbxLookUp.vvLookUpButton.Name       = "btnChangeLookUpSelection";

      //tbxLookUp.vvLookUpButton.Click += new EventHandler(VvUserControl.BtnChangeLookUpSelection_Click);
      tbxLookUp.vvLookUpButton.Click += new EventHandler(UpdateVvLookUpItem_4VvTextBox_Core);
   }

   private static void SetLookUpButton_ParentAnchorSizeLocation(VvTextBox tbxLookUp, bool isMultiline)
   {
      tbxLookUp.vvLookUpButton.Parent = tbxLookUp;

      if(isMultiline) // znaci da je tbxLookUp multiline
      {
         tbxLookUp.vvLookUpButton.Size = new Size(ZXC.QUN - ZXC.Qun5 + ZXC.Qun12, ZXC.QUN - ZXC.Qun5 + ZXC.Qun12);
         tbxLookUp.vvLookUpButton.Location = new Point(tbxLookUp.ClientSize.Width - tbxLookUp.vvLookUpButton.Width - ZXC.Qun12, 0);
      }
      else
      {
         tbxLookUp.vvLookUpButton.Size = new Size(tbxLookUp.ClientSize.Height, tbxLookUp.ClientSize.Height);
         tbxLookUp.vvLookUpButton.Location = new Point(tbxLookUp.ClientSize.Width - tbxLookUp.vvLookUpButton.Width, 0);
      }

      tbxLookUp.vvLookUpButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

   }

   public VvTextBox Kloniraj()
   {
      return this.MemberwiseClone() as VvTextBox;
   }

   public void OnOpenOrCloseForEditActions(ZXC.ZaUpis zaUpis, ZXC.ParentControlKind parentControlKind)
   {
      this.ReadOnly = JAM_WriteOnly ? false : (zaUpis == ZXC.ZaUpis.Zatvoreno ? true : false);

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(!this.JAM_ReverseVideo)
      {
         if(zaUpis == ZXC.ZaUpis.Otvoreno)
         {
            this.BackColor = ZXC.vvColors.vvTBoxReadOnly_False_BackColor; //ProfessionalColors.MenuItemSelectedGradientBegin;
            this.ForeColor = ZXC.vvColors.vvTBoxReadOnly_False_ForeColor; 
         }
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno)
         {
            this.BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;//SystemColors.ControlLightLight;
            this.ForeColor = ZXC.vvColors.vvTBoxReadOnly_True_ForeColor;
         }
     }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_IsSupressTab)
      {
         this.TabStop = false;
      }
      else
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.TabStop = true;
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.TabStop = false;
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(parentControlKind == ZXC.ParentControlKind.VvRecordUC)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.TextChanged += new EventHandler(this.EH_OnTextChanged_SetDirtyFlag);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.TextChanged -= new EventHandler(this.EH_OnTextChanged_SetDirtyFlag);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
           if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Enter += new EventHandler(this.EH_HotOn_GotFocus);
      else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Enter -= new EventHandler(this.EH_HotOn_GotFocus);
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
           if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Leave += new EventHandler(this.EH_HotOff_LostFocus);
      else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Leave -= new EventHandler(this.EH_HotOff_LostFocus);
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_ShouldValidateOnExit)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Validating += new CancelEventHandler(this.EH_JAM_Validating);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Validating -= new CancelEventHandler(this.EH_JAM_Validating);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FieldExitWithValidationMethod != null)
      {
               if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Validating += new CancelEventHandler(this.JAM_FieldExitWithValidationMethod);
         else  if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Validating -= new CancelEventHandler(this.JAM_FieldExitWithValidationMethod);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FieldEntryMethod != null)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Enter += new EventHandler(this.JAM_FieldEntryMethod);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Enter -= new EventHandler(this.JAM_FieldEntryMethod);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FieldExitMethod != null)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Leave += new EventHandler(this.JAM_FieldExitMethod);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Leave -= new EventHandler(this.JAM_FieldExitMethod);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FieldExitMethod_2 != null)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Leave += new EventHandler(this.JAM_FieldExitMethod_2);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Leave -= new EventHandler(this.JAM_FieldExitMethod_2);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FieldExitMethod_3 != null)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Leave += new EventHandler(this.JAM_FieldExitMethod_3);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Leave -= new EventHandler(this.JAM_FieldExitMethod_3);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_PasswdField_TextChanged_Method != null)
      {
              if (zaUpis == ZXC.ZaUpis.Otvoreno)  this.TextChanged += new EventHandler(this.JAM_PasswdField_TextChanged_Method);
         else if (zaUpis == ZXC.ZaUpis.Zatvoreno) this.TextChanged -= new EventHandler(this.JAM_PasswdField_TextChanged_Method);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_IsNumericTextBox == true && 
         this.JAM_IsInDataGridView == false)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Validated  += new EventHandler(this.EH_JAM_FormatNiceNumber);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Validated  -= new EventHandler(this.EH_JAM_FormatNiceNumber);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_FillCharacter != ' ' &&
         this.JAM_IsInDataGridView == false)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Validated += new EventHandler(this.EH_JAM_FormatFillCharacter);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Validated -= new EventHandler(this.EH_JAM_FormatFillCharacter);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      //if(this.JAM_IsForDateTimePicker == true && this.JAM_WriteOnly == false) 
      //{
      //        if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Visible = false;
      //   else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Visible = true;
      //}
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_CharEdits != ZXC.JAM_CharEdits.Unfiltered)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.KeyPress += this.GetVvTextBoxKeyPressEventHandler();
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.KeyPress -= this.GetVvTextBoxKeyPressEventHandler();
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      // 23.09.3026: 
      //if(this.JAM_IsPassword == true)
      //{
      //        if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.KeyPress += new KeyPressEventHandler(DisablePaste_OnKeyPress);
      //   else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.KeyPress -= new KeyPressEventHandler(DisablePaste_OnKeyPress);
      //}
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(!String.IsNullOrEmpty(this.JAM_StatusText) &&  this.JAM_IsInDataGridView == false)
      {
         if(zaUpis == ZXC.ZaUpis.Otvoreno)
         {
            this.Enter += new EventHandler(this.OnEnterSetStatusText);
            this.Leave += new EventHandler(this.OnExitRestoreStatusText);
         }
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno)
         {
            this.Enter -= new EventHandler(this.OnEnterSetStatusText);
            this.Leave -= new EventHandler(this.OnExitRestoreStatusText);
         }
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_IsOtsInfo == true && /*this.JAM_IsInDataGridView == false &&*/ this.JAM_DisableUpdateVvDataRecordActions != true)
      {
         if(zaUpis == ZXC.ZaUpis.Otvoreno)  ChangeStandardTextBoxContextMenu("Traži " + "OTS Info", UpdateOtsInfo_4VvTextBox_Core);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) ChangeStandardTextBoxContextMenu("",      null);

         if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.KeyDown += new KeyEventHandler(UpdateOtsInfo_OnKeyDown);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.KeyDown -= new KeyEventHandler(UpdateOtsInfo_OnKeyDown);

         // nota bene, mouse click je jaci - kvaci je DataGrdiViwe-ev, tako da je za TextBox nepotreban - visak.                                
         // dok je sa KeyDown drugacije. Ako je cell u edit modu onda je key TextBox-ov a ako je 'plavi' tj nije u edit modu onda je key DGV-ov 
         // zakljucak: KeyDown response moras dati i za CellIsInEditMode i za CellIs NOT InEditMode, dok je click dovoljan samo za DGV jer se on digne bijo ti u edit mode-u ili ne. 
         if(this.JAM_IsInDataGridView == false)
         {
            if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Click   += new EventHandler   (UpdateOtsInfo_OnClick);
            if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Click   -= new EventHandler   (UpdateOtsInfo_OnClick);

            //if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.DoubleClick += new EventHandler(UpdateVvDataRecord_OnDoubleClick);
            //if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.DoubleClick -= new EventHandler(UpdateVvDataRecord_OnDoubleClick);
         }

      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_IsAutocomplete == true && /*this.JAM_IsInDataGridView == false &&*/ this.JAM_DisableUpdateVvDataRecordActions != true)
      {
         if(zaUpis == ZXC.ZaUpis.Otvoreno)  ChangeStandardTextBoxContextMenu("Traži " + JAM_AutoCompleteRecordName, UpdateVvDataRecord_4VvTextBox_Core);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) ChangeStandardTextBoxContextMenu("",      null);

         if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.KeyDown += new KeyEventHandler(UpdateVvDataRecord_OnKeyDown);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.KeyDown -= new KeyEventHandler(UpdateVvDataRecord_OnKeyDown);

         // nota bene, mouse click je jaci - kvaci je DataGrdiViwe-ev, tako da je za TextBox nepotreban - visak.                                
         // dok je sa KeyDown drugacije. Ako je cell u edit modu onda je key TextBox-ov a ako je 'plavi' tj nije u edit modu onda je key DGV-ov 
         // zakljucak: KeyDown response moras dati i za CellIsInEditMode i za CellIs NOT InEditMode, dok je click dovoljan samo za DGV jer se on digne bijo ti u edit mode-u ili ne. 
         if(this.JAM_IsInDataGridView == false)
         {
            if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Click   += new EventHandler   (UpdateVvDataRecord_OnClick);
            if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Click   -= new EventHandler   (UpdateVvDataRecord_OnClick);

            //if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.DoubleClick += new EventHandler(UpdateVvDataRecord_OnDoubleClick);
            //if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.DoubleClick -= new EventHandler(UpdateVvDataRecord_OnDoubleClick);
         }

      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_IsVvLookUp == true && /*this.JAM_IsInDataGridView == false &&*/ this.JAM_DisableUpdateVvLookUpItemActions != true)
      {
         if(zaUpis == ZXC.ZaUpis.Otvoreno)  ChangeStandardTextBoxContextMenu("Traži " + JAM_LookUpTableTitle, UpdateVvLookUpItem_4VvTextBox_Core);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) ChangeStandardTextBoxContextMenu("",      null);

         if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.KeyDown += new KeyEventHandler(UpdateVvLookUpItem_OnKeyDown);
         if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.KeyDown -= new KeyEventHandler(UpdateVvLookUpItem_OnKeyDown);

         if(this.JAM_IsInDataGridView == false)
         {
            if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Click   += new EventHandler   (UpdateVvLookUpItem_OnClick);
            if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Click   -= new EventHandler   (UpdateVvLookUpItem_OnClick);

            //if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.DoubleClick += new EventHandler(UpdateVvLookUpItem_OnDoubleClick);
            //if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.DoubleClick -= new EventHandler(UpdateVvLookUpItem_OnDoubleClick);
         }

      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(zaUpis == ZXC.ZaUpis.Otvoreno) // IEditable shit for 'JAM_MustTabOutBeforeSubmit' . Ne seri. Ovo je korisna fora. OSTAVI je zadnju
                                        // jer redosljed += vazan. Zelis da 'OnLeave_EndEdit' obavi svoje tek kada su sve eventualne prijasnje zavrsile
      {
         this.Enter += new EventHandler(this.OnEnter_BeginEdit);
         this.Leave += new EventHandler(this.OnLeave_EndEdit);
      }
      else if(zaUpis == ZXC.ZaUpis.Zatvoreno)
      {
         this.Enter -= new EventHandler(this.OnEnter_BeginEdit);
         this.Leave -= new EventHandler(this.OnLeave_EndEdit);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
      if(this.JAM_NextTextBox != null)
      {
              if(zaUpis == ZXC.ZaUpis.Otvoreno)  this.Leave += new EventHandler(this.JAM_OnLeaveSelectNextTextBox);
         else if(zaUpis == ZXC.ZaUpis.Zatvoreno) this.Leave -= new EventHandler(this.JAM_OnLeaveSelectNextTextBox);
      }
      //---------------------------------------------------------------------------------------------------------------------------------------------------------------
   }

   #endregion Methods

   #region IEditableObject Members

   public void BeginEdit()
   {
      if(editInProgress == false)
      {
         backupText     = this.Text;
         editInProgress = true;
      }
   }

   public void CancelEdit()
   {
      if(editInProgress == true)
      {
         this.Text      = backupText;
         editInProgress = false;
      }

      throw new Exception("Mislim da se ovo nema zasto ikada pozivati?!. not really sure yet.");
   }

   public void EndEdit()
   {
      if(editInProgress == true)
      {
         backupText     = null;
         editInProgress = false;
      }
   }

   // Vv extension for IEditableObject: 
   public bool EditedHasChanges()
   {
      return !(this.Text == backupText);
   }

   #endregion

}
