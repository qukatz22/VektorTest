using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

[Serializable]
public class VvLookUpItem : IDisposable, IEditableObject
{
   #region fildz i konstruktor 

   private string    _name;
   private string    _cd;
   private decimal   _number;
   private bool      _flag;
   private int       _integer;
   private DateTime  _dateT;
   private uint      _uinteger;
   private string    _string2 ;
 //private double    _double  ;

   private bool _modified;
   private bool _editing;


   public VvLookUpItem(): this("", "", 0.00M, false, 0, DateTime.Today, 0, "")
   {
   }

   public VvLookUpItem(string cd, string name): this(cd, name, 0.00M, false, 0, DateTime.Now, 0, "")
   {
   }

   public VvLookUpItem(string cd, string name, decimal number, bool flag, int integer, DateTime dateT, uint uinteger, string string2)
   {
      this._name     = name    ;
      this._cd       = cd      ;
      this._number   = number  ;
      this._flag     = flag    ;
      this._integer  = integer ;
      this._dateT    = dateT   ;
      this._uinteger = uinteger;
      this._string2  = string2 ;

      _modified = false;
      _editing = false;
   }

   public void Dispose()
   {
      // ne znam kaj s ovim 
   }
   #endregion fildz i konstruktor 

   #region propertiz 

   public string Name
   {
      get { return this._name; }
      set {        this._name = value; }
   }

   public string Cd
   {
      get { return this._cd; }
      set {        this._cd = value; }
   }

   public decimal Number
   {
      get { return this._number; }
      set {        this._number = value; }
   }

   public bool Flag
   {
      get { return this._flag; }
      set {        this._flag = value; }
   }

   public int Integer
   {
      get { return this._integer; }
      set {        this._integer = value; }
   }

   public DateTime DateT
   {
      get { return this._dateT; }
      set {        this._dateT = value; }
   }

   public uint Uinteger
   {
      get { return this._uinteger; }
      set {        this._uinteger = value; }
   }

   public string String2
   {
      get { return this._string2; }
      set {        this._string2 = value; }
   }

   [System.Xml.Serialization.XmlIgnore]
   public bool R_Bool { get; set; }

   public static bool AreEqual(VvLookUpItem aLui, VvLookUpItem bLui)
   {
      if(aLui == null || bLui == null) return false;

      if(aLui.Name               == bLui.Name             &&
         aLui.Cd                 == bLui.Cd               &&
         aLui.Number             == bLui.Number           &&
         aLui.Flag               == bLui.Flag             &&
         aLui.Integer            == bLui.Integer          &&
         aLui.DateT              == bLui.DateT            &&
         aLui.Uinteger           == bLui.Uinteger         &&
         aLui.String2.NullSafe() == bLui.String2.NullSafe()) return true;

      return false;
   }

   // Object class overrides
   public override bool Equals(object obj)
   {
      if (obj is VvLookUpItem)
      {
         VvLookUpItem lui = (VvLookUpItem)obj;

         return(this._name.ToLower().Equals(lui.Name.ToLower()));
         //return(this._cd.Equals(lui.Cd));
      }

      return false;
   }

   public override int GetHashCode()
   {
      return this.Cd.GetHashCode();
   }

   public override string ToString()
   {
      //return String.Format("{0,-20} ({1})", rptLabel, cd); 
      return "Cd: " + this.Cd + " / Name: " + this.Name;
   }

   #endregion propertiz 

   #region IEditableObject implementation

   // Members for IEditableObject implementation: 

   private string   _editName;
   private string   _editCd;
   private decimal  _editNumber;
   private bool     _editFlag;
   private int      _editInteger;
   private DateTime _editDateT;
   private uint     _editUinteger;
   private string   _editString2;
 
   public void BeginEdit()
   {
      if (!_editing)
      {
         _editName     = Name;
         _editCd       = Cd;
         _editNumber   = Number;
         _editFlag     = Flag;
         _editInteger  = Integer;
         _editDateT    = DateT;
         _editUinteger = Uinteger;
         _editString2  = String2;
         _editing      = true;
      }
   }

   public void CancelEdit()
   {
      if (_editing)
      {
         Name     = _editName;
         Cd       = _editCd;
         Number   = _editNumber;
         Flag     = _editFlag;
         Integer  = _editInteger;
         Uinteger = _editUinteger;
         DateT    = _editDateT;
         String2  = _editString2;
         _editing = false;
      }
   }

   public void EndEdit()
   {
      if (_editing)
      {
         _modified |=   ((Name     != _editName    )
                      || (Cd       != _editCd      )
                      || (Number   != _editNumber  )
                      || (Flag     != _editFlag    )
                      || (Integer  != _editInteger )
                      || (Uinteger != _editUinteger)
                      || (String2  != _editString2 )
                      || (DateT    != _editDateT   ));
         _editing = false;
      }
   }

   [System.Xml.Serialization.XmlIgnore]
   public bool HasEdits
   {
      get { return _modified; }
      set {        _modified = value; }
   }

   #endregion IEditableObject implementation

   #region Obsolete - sepsolete
//   /// <summary>
///// Mozda zatreba, a obsolete odkada rabimo XML format :-) 
///// </summary>
///// <param rptLabel="row"></param>
///// <returns></returns>
//   public static VvLookUpItem ReadLuiLine(System.Data.DataRow row)
//   {
//      VvLookUpItem lui = new VvLookUpItem();

//      int tmpInt;

//      lui.Name = row[0].ToString();
//      lui.Cd = row[1].ToString();
//      try
//      {
//         lui.Number = Double.Parse(row[2].ToString());
//      }
//      catch /*(FormatException, IndexOutOfRangeException)*/
//      {
//         lui.Number = 0.00;
//      }
//      try
//      {
//         tmpInt = Int32.Parse(row[3].ToString());
//      }
//      catch(FormatException)
//      {
//         //CIA.aim_emsg("FormatException, vjerovatno prazan red na kraju txt datoteke.");
//         tmpInt = 0;
//      }
//      catch
//      {
//         tmpInt = 0;
//      }

//      lui.Flag = tmpInt == 1 ? true : false;

//      try
//      {
//         lui.DateT = DateTime.Parse(row[4].ToString().TrimEnd('.')); // ovo TrimEnd jerbo mu smeta zadnja tocka poslije godine u ShortDateFormatu 
//      }
//      catch /*(FormatException, IndexOutOfRangeException)*/
//      {
//         lui.DateT = DateTime.Now;
//      }

//      return (lui);
//   }

   #endregion Obsolete - sepsolete
   
}
