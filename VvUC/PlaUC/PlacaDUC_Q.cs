using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Reflection;

//public partial class PlacaDUC : VvPolyDocumRecordUC
public partial class PlacaBaseDUC : VvPolyDocumRecordUC
{
   public virtual List<VvPref.VVColChooserStates> TheColChooserStates { get; set; }

   public List<VvPref.VVColChooserStates> PlacaColChDefaultsList;

   protected bool IsBruto1ViaKoef { get { return ZXC.CURR_prjkt_rec.VrKoefBr1.NotZero(); } }

   #region Q's UC specific util methods

   #region CalcSati - JAM_FieldExitWithValidationMethod

   void CalcSati_forOD(object sender, System.ComponentModel.CancelEventArgs e)
   {
      CalcSati_forOdDo(sender, e, true);
   }

   void CalcSati_forDO(object sender, System.ComponentModel.CancelEventArgs e)
   {
      CalcSati_forOdDo(sender, e, false);
   }

   void CalcSati_forOdDo(object sender, System.ComponentModel.CancelEventArgs e, bool isCallerOD)
   {
      VvDataGridView dgv = sender as VvDataGridView;

      VvTextBox vvTextBox = dgv.EditingControl as VvTextBox;

      int rowIdx = dgv.CurrentCell.RowIndex;

      uint ddOD, ddDO;

      if(isCallerOD == true)
      {
         ddOD = ZXC.ValOrZero_UInt(vvTextBox.Text);
         ddDO = dgv.GetUint32Cell(ci2.iT_rsDO, rowIdx, false);
      }
      else
      {
         ddOD = dgv.GetUint32Cell(ci2.iT_rsOD, rowIdx, false);
         ddDO = ZXC.ValOrZero_UInt(vvTextBox.Text);
      }

      if(vvTextBox.IsNonEmpty())
      {
         int daysInMM = DateTime.DaysInMonth(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month);
         uint minDO = (ddOD.NotZero() ? ddOD : 1);
         uint maxOD = (ddDO.NotZero() ? ddDO : (uint)daysInMM);

         if(isCallerOD)
         {
            if(ddOD < 1 || ddOD > maxOD)
            {
               ZXC.RaiseErrorProvider(vvTextBox, "Pogrešan pocetni dan " + vvTextBox.Text + ". Mora biti [01 - " + maxOD.ToString("00") + "]!");
               e.Cancel = true;
            }
         }
         else
         {
            if(ddDO < minDO || ddDO > daysInMM)
            {
               ZXC.RaiseErrorProvider(vvTextBox, "Pogrešan završni dan " + vvTextBox.Text + ". Mora biti [" + minDO.ToString("00") + " - " + daysInMM.ToString("00") + "]!");
               e.Cancel = true;
            }
         }
      } // if(vvTextBox.IsNonEmpty()) 

      Ptrans ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == dgv.GetUint32Cell(ci.iT_personCD, rowIdx, false));
      // 05.05.2015: 
      if(ptrans_rec == null)
      {
         ZXC.aim_emsg("Radnik sifra[{0}] ne postoji na prvome grid-u!", dgv.GetUint32Cell(ci.iT_personCD, rowIdx, false));
         return;
      }
      bool isPoluSat = ptrans_rec == null ? false : ptrans_rec.T_IsPoluSat;
    //decimal workHours = ZXC.GetWorkHoursCount(ZXC.CURR_prjkt_rec.IsTrgRs, isPoluSat, placa_rec.MMYYYY, ddOD, ddDO);
      decimal workHours = ZXC.GetWorkHoursCount(Fld_IsTrgFondSati, isPoluSat, placa_rec.MMYYYY, ddOD, ddDO, ptrans_rec.T_dnFondSati);

      dgv.PutCell(ci2.iT_sati, rowIdx, workHours);
   }

   #endregion CalcSati - JAM_FieldExitWithValidationMethod

   private void OnExit_TtOrMMYYYY_Set_RS_ID(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_TT.IsEmpty() || Fld_MMYYYY.IsEmpty()) return;

      if(Fld_DokDate < ZXC.Date01012014) Fld_RSm_ID = ZXC.PlacaDao.GetNextRS_ID(TheDbConnection, Fld_TT, Fld_MMYYYY).ToString("0000");
    //else                               Fld_RSm_ID = placa_rec.DokDateAsMMYYYY.Substring(4 ,2) + placa_rec.DokDate.DayOfYear.ToString("000");

   }

   private void OnExit_Tt_SetVrstaObracuna_And_PersonRestrictor(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_TT.IsEmpty()) return;

      Fld_VrstaObr = ((Fld_TT == Placa.TT_REDOVANRAD || Fld_TT == Placa.TT_PODUZETPLACA || Fld_TT == Placa.TT_NEOPOREZPRIM) ? "01" : "03");

      switch(Fld_TT)
      {
         case Placa.TT_REDOVANRAD  : SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor.PER_RR_Only   ); break;
         case Placa.TT_AUTORHONOR  :
         case Placa.TT_AHSAMOSTUMJ :
         case Placa.TT_AUTORHONUMJ : SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor.PER_AHiAU_Only); break;
         case Placa.TT_UGOVORODJELU: SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor.PER_UD_Only   ); break;
         case Placa.TT_PODUZETPLACA: SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor.PER_PP_Only   ); break;
         case Placa.TT_NADZORODBOR : SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor.PER_NO_Only   ); break;
      }
   }

   private void SetPersonCd_AutocompleteCustomSource_Restricted(ZXC.AutoCompleteRestrictor restrictor)
   {
      vvtbT_personCD.JAM_AutoCompleteRestrictor = 
      vvtbT_prezime .JAM_AutoCompleteRestrictor = restrictor;

      vvtbT_personCD.AutoCompleteCustomSource.Clear();
      vvtbT_prezime .AutoCompleteCustomSource.Clear();
      if(this is PlacaNPDUC) return;

      vvtbT_personCD2.JAM_AutoCompleteRestrictor = 
      vvtbT_personCD3.JAM_AutoCompleteRestrictor = restrictor;
      vvtbT_prezime2 .JAM_AutoCompleteRestrictor =
      vvtbT_prezime3 .JAM_AutoCompleteRestrictor = restrictor;

      vvtbT_personCD2.AutoCompleteCustomSource.Clear();
      vvtbT_prezime2 .AutoCompleteCustomSource.Clear();
      vvtbT_personCD3.AutoCompleteCustomSource.Clear();
      vvtbT_prezime3 .AutoCompleteCustomSource.Clear();
   }

   //private void OLD_OnExit_DokDateOrMMYYYY_Set_PlacaRules(object sender, EventArgs e)
   //{
   //   bool          isMMYYYY;
   //   VvTextBox     vvtb    = sender as VvTextBox;
   //   VvLookUpLista luiList = ZXC.luiListaPRules;
   //   PropertyInfo  pInfo;
   //   Type          plType  = pr.GetType();

   //   // ovo je sa DTP pa je malo mutno kada ima/nema changes pa idemo ziheraski - uvijek 
   //   //if(vvtb.EditedHasChanges() == false) return;

   //        if(vvtb.A0_JAM_Name == tbx_dokDate     .A0_JAM_Name) isMMYYYY = false;
   //   else if(vvtb.A0_JAM_Name == tbx_lookUpMMYYYY.A0_JAM_Name) isMMYYYY = true;
   //   else throw new Exception("Sender nije niti dokDate niti MMYYYY!");

   //   luiList.LazyLoad();
   //   var pRulesLUitems = luiList.Where(lui => lui.Flag == isMMYYYY);

   //   foreach(var item in pRulesLUitems)
   //   {
   //      pInfo = plType.GetProperty(item.Cd);

   //      if(pInfo == null)
   //      {
   //         ZXC.aim_emsg("Set_PlacaRules via VvLookupList ERROR!\n\nNe postoji PROPERTY naziva [" + item.Cd + "]!");
   //         continue;
   //      }

   //      pInfo.SetValue(pr, item.Number, null);

   //   }

   //   PutPlacRulesFields();

   //}

   // NotaBene: kada si koristio ovu gore 'OLD' koja ide preko biznisa onda bi ti PutPlacRulesFields bilo kojeg VvTextBox-a, dignuo event OnTextChanged_SetDirtyFlag koji pak digne GetFields koji pak pobrise polja biznisa jer je UC jos uvijek prazan! 

   private void OnExit_DokDateOrMMYYYY_Set_PlacaRules(object sender, EventArgs e)
   {
      bool          isMMYYYY;
      VvTextBox     vvtb;
      VvLookUpLista luiList = ZXC.luiListaPRules;
      PropertyInfo  pInfo;
      Type          plType  = this.GetType(); // PlacaDUC 
      DateTime      onDate;

      // JOPPD_ID: !!! 
      if(Fld_DokDate >= ZXC.Date01012014) Fld_RSm_ID = placa_rec.DokDateAsMMYYYY.Substring(4, 2) + placa_rec.DokDate.DayOfYear.ToString("000");

      // ovo je sa DTP pa je malo mutno kada ima/nema changes pa idemo ziheraski - uvijek 
      //if(vvtb.EditedHasChanges() == false) return;

      if(sender is VvTextBox)              vvtb = sender as VvTextBox;
      else /* sender is VvDateTimePicker*/ vvtb = ((VvDateTimePicker)sender).TheVvTextBox;

           if(vvtb.A0_JAM_Name == tbx_dokDate     .A0_JAM_Name) { isMMYYYY = false; onDate = Fld_DokDate; }
      else if(vvtb.A0_JAM_Name == tbx_lookUpMMYYYY.A0_JAM_Name) { isMMYYYY = true;  onDate = Placa.GetDateTimeFromMMYYYY(Fld_MMYYYY, false); }
      else throw new Exception("Sender nije niti dokDate niti MMYYYY!");

      //ZXC.aim_emsg(onDate.ToShortDateString());

      luiList.LazyLoad();

      #region Clear All First (ali samo paket 'ZA' ili 'U')

      // ovisno o senderu, ovdje filtriramo one koji trzaju ili na 'U mjesecu' ili na 'ZA mjesec',  
      var pRulesLUitems_distinctCd = luiList.Distinct().Where(lui => (lui.Flag != isMMYYYY)).Select(lui => lui.Cd);

      #region 2022 ---> 2023 additions

      var EURtwin_CdList = pRulesLUitems_distinctCd.Where(pr => pr.EndsWith("_EUR")).ToList();
    //var KNtwin_CdList  = new List<string>(EURtwin_CdList.Select(pr => pr.Replace("_EUR", "")));

      bool isEURtwin;
      bool isKNtwin;

      #endregion 2022 ---> 2023 additions

      foreach(string distinctCd in pRulesLUitems_distinctCd)
      {
         pInfo = plType.GetProperty("Fld_" + distinctCd);

         #region 2022 ---> 2023 additions

         isEURtwin = distinctCd.EndsWith("_EUR");
         isKNtwin  = EURtwin_CdList.Contains(distinctCd + "_EUR");

         #endregion 2022 ---> 2023 additions

         if(pInfo == null)
         {
            if(distinctCd != "StMioNaB5" && distinctCd != "StMioNa2B5") ZXC.aim_emsg("Clear All First: Set_PlacaRules via VvLookupList ERROR!\n\nNe postoji PROPERTY naziva [" + distinctCd + "]!");
            continue;
         }

         if(ZXC.projectYearAsInt <= 2022 && isEURtwin) continue; // stara kune era 
         if(ZXC.projectYearAsInt >= 2023 && isKNtwin)  continue; // nova  euri era 

         if(pInfo.PropertyType == typeof(uint)) pInfo.SetValue(this, (uint)0    , null);
         else                                   pInfo.SetValue(this,       0.00M, null);
      }

      #endregion Clear All First (ali samo paket 'ZA' ili 'U')

      // ovisno o senderu, ovdje filtriramo one koji trzaju ili na 'U mjesecu' ili na 'ZA mjesec',  
      // a onda jos i filtriramo po doticnom datumu tako da odozgo odsjecemo 'previse nova' pravila 
      var pRulesLUitems_filtered = luiList.Where(lui => (lui.Flag != isMMYYYY && onDate >= lui.DateT));


      var pRulesLUitems_ruleGroups = pRulesLUitems_filtered.GroupBy(lui => lui.Cd);

      foreach(var ruleGroup in pRulesLUitems_ruleGroups)
      {
         var lui = ruleGroup.OrderBy(l => l.DateT).Last();

         pInfo = plType.GetProperty("Fld_" + lui.Cd);

         #region 2022 ---> 2023 additions

         isEURtwin = lui.Cd.EndsWith("_EUR");
         isKNtwin  = EURtwin_CdList.Contains(lui.Cd + "_EUR");

         #endregion 2022 ---> 2023 additions

         if(pInfo == null)
         {
          // 01.02.2017.
          //if(lui.Cd != "StMioNaB5"                          ) ZXC.aim_emsg("Set_PlacaRules via VvLookupList ERROR!\n\nNe postoji PROPERTY naziva [" + lui.Cd + "]!");
            if(lui.Cd != "StMioNaB5" && lui.Cd != "StMioNa2B5") ZXC.aim_emsg("Set_PlacaRules via VvLookupList ERROR!\n\nNe postoji PROPERTY naziva [" + lui.Cd + "]!");
 
            continue;
         }

         if(ZXC.projectYearAsInt <= 2022 && isEURtwin) continue; // stara kune era 
         if(ZXC.projectYearAsInt >= 2023 && isKNtwin)  continue; // nova  euri era 

         if(pInfo.PropertyType == typeof(uint)) pInfo.SetValue(this, (uint)lui.Number, null);
         else                                   pInfo.SetValue(this,       lui.Number, null);
      }

      if(ZXC.CURR_prjkt_rec.IsOver20 == false)
      {
         Fld_StZapNa += Fld_StZapII;

         Fld_StZapII = decimal.Zero;
      }

      // 08.09.2014: 
      Fld_VrKoefBr1 = ZXC.CURR_prjkt_rec.VrKoefBr1;
   }

   private string GetOneLetter4Invalid(Ptrans.InvalidEnum invalidEnum)
   {
      switch(invalidEnum)
      {
         case Ptrans.InvalidEnum.HRVI   : return "H";
         case Ptrans.InvalidEnum.INVALID: return "I";

         default                        : return "";
      }
   }

   private string GetOneLetter4PtranoKind(Ptrans.PtranoKind ptranoKindEnum)
   {
      switch(ptranoKindEnum)
      {
         case Ptrans.PtranoKind.OBUSTAVA     : return "" ;
         case Ptrans.PtranoKind.ZASTICENIrn  : return "Z";
         case Ptrans.PtranoKind.NEZASTICENIrn: return "N";

         default: return "";
      }
   }
   private Ptrans.PtranoKind GetPtranoKindEnumFromFirstLetter(string firtsLetter)
   {
      switch(firtsLetter)
      {
         case "Z": return Ptrans.PtranoKind.ZASTICENIrn  ;
         case "N": return Ptrans.PtranoKind.NEZASTICENIrn;

         default: return Ptrans.PtranoKind.OBUSTAVA;
      }
   }

   private string GetOneLetter4Spc(Ptrans.SpecEnum specEnum)
   {
      switch(specEnum)
      { 
         case Ptrans.SpecEnum.NOVOZAPOSL   : return "N"; 
         case Ptrans.SpecEnum.PENZ         : return "U"; 
         case Ptrans.SpecEnum.MINMIONE     : return "M"; 
         case Ptrans.SpecEnum.MAXMIONE     : return "O"; 
         case Ptrans.SpecEnum.CLANUPRAVE   : return "C"; 
         case Ptrans.SpecEnum.NOVO_MINMIONE: return "X";
         case Ptrans.SpecEnum.IZASLANRADNIK: return "I";

         default: return "";  
      }
   }

   private Ptrans.SpecEnum GetSpecEnumFromFirstLetter(string firtsLetter)
   {
      switch(firtsLetter)
      {
         case "N": return Ptrans.SpecEnum.NOVOZAPOSL;
         case "U": return Ptrans.SpecEnum.PENZ;
         case "M": return Ptrans.SpecEnum.MINMIONE;
         case "O": return Ptrans.SpecEnum.MAXMIONE;
         case "C": return Ptrans.SpecEnum.CLANUPRAVE;
         case "X": return Ptrans.SpecEnum.NOVO_MINMIONE;
         case "I": return Ptrans.SpecEnum.IZASLANRADNIK;

         default: return Ptrans.SpecEnum.XNIJE;
      }
   }

   private Ptrans.InvalidEnum GetInvalidEnumFromFirstLetter(string firtsLetter)
   {
      switch(firtsLetter)
      {
         case "H": return Ptrans.InvalidEnum.HRVI;
         case "I": return Ptrans.InvalidEnum.INVALID;

         default : return Ptrans.InvalidEnum.NIJE;
      }
   }

   private int GetOneInteger4Mio1OlkKindEnum(Ptrans.Mio1OlkKindEnum mioEnum)
   {
      switch(mioEnum)
      {
         case Ptrans.Mio1OlkKindEnum.NIJE  : return 0;
         case Ptrans.Mio1OlkKindEnum.Do0700: return 1;
         case Ptrans.Mio1OlkKindEnum.Do1300: return 2;
         case Ptrans.Mio1OlkKindEnum.Izjava: return 3;
         case Ptrans.Mio1OlkKindEnum.PorUpr: return 4;

         default: return 0;
      }
   }

   private Ptrans.Mio1OlkKindEnum GetMio1OlkKindEnumFromInteger(int num)
   {
      switch(num)
      {
         case 0: return Ptrans.Mio1OlkKindEnum.NIJE;
         case 1: return Ptrans.Mio1OlkKindEnum.Do0700;
         case 2: return Ptrans.Mio1OlkKindEnum.Do1300;
         case 3: return Ptrans.Mio1OlkKindEnum.Izjava;
         case 4: return Ptrans.Mio1OlkKindEnum.PorUpr;

         default: return Ptrans.Mio1OlkKindEnum.NIJE;
      }
   }


   public void SelectVisibleColumns(ZXC.VvColSetVisible wantedSet)
   {
      TheG.TheSumOfPreferredWidths = 0;

      foreach(DataGridViewColumn col in TheG.Columns)
      {
         if(col.Tag != null && col.Tag.ToString() == VvDocumentRecordUC.AlwaysInvinsibleStr) col.Visible = false;
         else                                                                                col.Visible = GetColumnVisibility(col, wantedSet);

         TheSumGrid.Columns[col.Name].Visible = col.Visible;
         
         if(col.Visible) TheG.TheSumOfPreferredWidths += col.Width;
      }

      switch(wantedSet)
      {
         case ZXC.VvColSetVisible.AllVisible:
            TheG.ColumnHeadersDefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor;
            DisableView_ChBoxDataGrid(true);
            break;
         
         case ZXC.VvColSetVisible.BlueVisible:
            TheG.ColumnHeadersDefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor_Blue;
            DisableView_ChBoxDataGrid(false);
            break;
         
         case ZXC.VvColSetVisible.RedVisible:
            TheG.ColumnHeadersDefaultCellStyle.BackColor = ZXC.vvColors.dataGridColumnHeaders_BackColor_Red;
            DisableView_ChBoxDataGrid(false);
            break;
         
         default:
            break;
      }
   }
   
   public void DisableView_ChBoxDataGrid(bool visEnable)
   {
      ZXC.VvSubModulEnum subModulEnum = TheSubModul.subModulEnum;

      Point xy =  ZXC.TheVvForm.GetSubModulXY(subModulEnum);

      ZXC.TheVvForm.ats_SubModulSet[xy.X][xy.Y].Items["viewChGrid"].Enabled = visEnable;

      if(!visEnable)
      {
         TheColChooserGrid.Visible = visEnable;
         TheG.Location             = new Point(TheG.Location.X, TheColChooserGrid.Top);
         TheG.Height               = TheG.Parent.Height - 2 * ZXC.QUN - TheSumGrid.Height;
      }
   }

   private bool GetColumnVisibility(DataGridViewColumn col, ZXC.VvColSetVisible wantedSet)
   {                                                                        // dodanoDanas - vidi se odmah i ako je prvi puta        
      if(TheColChooserStates == null || TheColChooserStates.Count.IsZero()) // Defaultna Postavka 
      {
         TheColChooserStates = new List<VvPref.VVColChooserStates>(PlacaColChDefaultsList);
         PutFields_TheColChooserGrid();
      }

      VvPref.VVColChooserStates colVisiblePref = TheColChooserStates.SingleOrDefault(item => item.ColumnName == col.Name);

      if(colVisiblePref.ColumnName == null) return false;

      switch(wantedSet)
      {
         case ZXC.VvColSetVisible.AllVisible : return true;
         case ZXC.VvColSetVisible.BlueVisible: return colVisiblePref.VisibleInBlue;
         case ZXC.VvColSetVisible.RedVisible : return colVisiblePref.VisibleInRed;
      }

      return false;
   }

   public void PutFields_TheColChooserGrid()
   {
      string rowHeader;
      bool   isChecked;

      List<VvPref.VVColChooserStates> colChooserStates = TheColChooserStates;
      VvPref.VVColChooserStates       colVisiblePref;
      //var pero = colChooserStates.Where(item => item.ColumnName == "t_isMioII");

      if(colChooserStates == null || colChooserStates.Count.IsZero()) colChooserStates = new List<VvPref.VVColChooserStates>(PlacaColChDefaultsList);

      foreach(DataGridViewRow row in TheColChooserGrid.Rows)
      {
         rowHeader = row.HeaderCell.Value.ToString();

         foreach(DataGridViewCell cell in row.Cells)
         {
            colVisiblePref = colChooserStates.SingleOrDefault(item => item.ColumnName == cell.OwningColumn.Name);

                 if(rowHeader == "Red"  && colVisiblePref.VisibleInRed  == true) isChecked = true;
            else if(rowHeader == "Blue" && colVisiblePref.VisibleInBlue == true) isChecked = true;
            else                                                                 isChecked = false;

            cell.Value = isChecked;
         }
      }
   }

   protected bool IsThisOznakaStjecatelja_OK_ForThisPlacaTT(string oznakaStjecatelja, string placaTT)
   {
      uint stjecCD = ZXC.ValOrZero_UInt(oznakaStjecatelja);

      if(stjecCD.IsZero()) return false;

      switch(placaTT)
      {
         case Placa.TT_REDOVANRAD  : if(stjecCD >=  1 && stjecCD <=  9) return true;
                                     else                               return false;

         case Placa.TT_PODUZETPLACA: if(stjecCD >= 31 && stjecCD <= 39) return true;
                                     else                               return false;

         default: return false;
      }
   }

   #endregion Q's UC specific util methods

   void PlacaDUC_Validating(object sender, CancelEventArgs e)
   {

      #region Should validate enivej?

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None   ||
         TheVvTabPage.WriteMode == ZXC.WriteMode.Delete ||
         this.Visible           == false) return;

      // Mozda trerba a mozda ne?! 19.1.2011: NE treba! 19.1.2011 lejter that day: DA treba! More lejter NE!!! 
      // a za placu da?! 
      GetFields(false);

      #endregion Should validate enivej?

      #region IsDocumentFromLockedPeriod

      // 09.02.2016: 

      if(VvDaoBase.IsDocumentFromLockedPeriod(Fld_DokDate.Date, false)) e.Cancel = true;

      #endregion IsDocumentFromLockedPeriod

      // 11.02.2016: provjera joppd broja 
      if(Fld_DokDate >= ZXC.Date01012014)
      {
         string OKjoppdBr = placa_rec.DokDateAsMMYYYY.Substring(4, 2) + placa_rec.DokDate.DayOfYear.ToString("000");

         if(OKjoppdBr != Fld_RSm_ID)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Joppd broj [{0}]\n\nne odgovara s obzirom na datum.\n\nTreba biti [{1}]", Fld_RSm_ID, OKjoppdBr);
            e.Cancel = true;
         }
      }

//      for(int rowIdx = 0; rowIdx < TheG.RowCount - 1; ++rowIdx)
//      {
//         artiklCD = TheG.GetStringCell (ci.iT_artiklCD, rowIdx, false);
//         t_kol    = TheG.GetDecimalCell(ci.iT_kol     , rowIdx, false);
//
//         if(artiklCD.NotEmpty() && ArtiklSifrar.Select(artikl => artikl.ArtiklCD).Contains(artiklCD) == false)
//         {
//            //DialogResult result = MessageBox.Show("Artikl ne postoji.\n\nRedak: " + (rIdx + 1) + " ArtiklCD: " + artiklCD + "\n\nŽelite li zaista usnimiti ovaj dokument?", "Potvrdite usnimavanje?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
//            //if(result != DialogResult.Yes) e.Cancel = true;
//
//            ZXC.aim_emsg(MessageBoxIcon.Error, "Artikl ne postoji.\n\nRedak: {0} ArtiklCD: {1}", (rowIdx + 1), artiklCD);
//            e.Cancel = true;
//         }

//      }

      // 11.02.2014: 
      //decimal ptranoNetto;
      foreach(Ptrano ptrano_rec in /*ptranosOfThisPtrans*/placa_rec.TransesNonDeleted3)
      {
         if(ptrano_rec.T_izNetoaSt.IsZero()) continue;

         // 
         //ptranoNetto = ZXC.VvGet_100_from_25and25(ptrano_rec.T_iznosOb, ptrano_rec.T_izNetoaSt);
         //if(ZXC.AlmostEqual(ptranoNetto, R_Netto, 0.02M) == false)
         //{
         //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "{0}\n\nGREŠKA! Promjena neta nakon zadavanja OBUSTAVE koja ovisi o iznosu neta.\n\nPonovite zadavanje obustave!\n\nStari neto: {1}\n\nNovi neto: {2}",
         //      T_prezimeIme, ptranoNetto.ToStringVv(), R_Netto.ToStringVv());
         //}

         // 14.02.2014. 
         Ptrans ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == ptrano_rec.T_personCD);
         if(ptrans_rec == null) continue;

          // 09.11.2016: 
       //decimal iznosObust = ZXC.VvGet_25_on_100(ptrans_rec.R_Netto   , ptrano_rec.T_izNetoaSt);
         decimal iznosObust = ZXC.VvGet_25_of_100(ptrans_rec.R_ObustOsn, ptrano_rec.T_izNetoaSt);

         if(ZXC.AlmostEqual(iznosObust, ptrano_rec.T_iznosOb, 0.02M) == false)
         {
          //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "{0}\n\nGREŠKA! Promjena neta nakon zadavanja OBUSTAVE koja ovisi o iznosu neta.\n\nPonovite zadavanje obustave!\n\nStara obustava: {1}\n\nNova obustava: {2}",
          //   ptrans_rec.T_prezimeIme, ptrano_rec.T_iznosOb.ToStringVv(), iznosObust.ToStringVv());
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE. Promjena neta nakon zadavanja OBUSTAVE koja ovisi o iznosu neta.\n\nPonoviti cu obracun obustave!");

            TheVvTabPage.TheVvForm.RecalcObustViaPostoIzNeto("PlacaDUC_Validating", EventArgs.Empty);

            e.Cancel = true;

            break;
         }
      } // foreach(Ptrano ptrano_rec in placa_rec.TransesNonDeleted3)

      // 05.05.2015: 
      // evr: dayDO overflow 
      int daysInMM = DateTime.DaysInMonth(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month);
      foreach(Ptrane ptrane_rec in placa_rec.TransesNonDeleted2)
      {
         // 15.12.2023: 
       //if(ptrane_rec.T_rsOO == "10")
         if(ptrane_rec.T_rsOO != "98" && ptrane_rec.T_rsOO != "99")
         {
            if(ptrane_rec.T_rsOD.IsZero() || ptrane_rec.T_rsDO.IsZero())
            {
             //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "GRESKA. Za redovan rad '10' niti rsOD niti rsDO ne smiju biti '0'!\n\n{0}!", ptrane_rec.T_PrezimeIme);
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE. Niti rsOD niti rsDO ne smiju biti '0'!\n\n{0}!", ptrane_rec.T_PrezimeIme);

               // 15.12.2023: mjenjamo iz errora u warning only 
             //e.Cancel = true;
               break;
            }
         }

         if(ptrane_rec.T_rsDO.IsZero()) continue;

         if(ptrane_rec.T_rsDO > daysInMM)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "GRESKA. Max dan u mjesecu je [{0}]\n\na stavka [{1}] ima \n\n{2}!", daysInMM, ptrane_rec.T_PrezimeIme, ptrane_rec.T_rsDO);

            e.Cancel = true;

            break;
         }
      } // foreach(Ptrane ptrane_rec in placa_rec.TransesNonDeleted2)

      // 19.02.2020: 
      decimal maxHZZO_NetoAdd = 4257.28M;

      foreach(Ptrans ptrans_rec in placa_rec.TransesNonDeleted)
      {
         if(placa_rec.TT == Placa.TT_REDOVANRAD && ptrans_rec.T_koef.IsZero())
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Warning, "UPOZORENJE. Djelatnik {0}\n\r\n\rKoef. osn. odbitka je nula!?", ptrans_rec.T_prezimeIme);

            //e.Cancel = true;
            //break;
         }

         bool daLiJeZaDugoBolovanjeNetoAdd_PREVELIK = placa_rec.TT == Placa.TT_REDOVANRAD && ptrans_rec.T_neoPrimCD == "12" /* dugo bolovanje*/ && ptrans_rec.T_NetoAdd > maxHZZO_NetoAdd;

         bool daLiJeBolovanjeZbogOzljedeNaRadu = Get_daLiJeBolovanjeZbogOzljedeNaRadu(ptrans_rec);

         // 26.08.2021: Maks :-) 
       //if(daLiJeZaDugoBolovanjeNetoAdd_PREVELIK)
         if(daLiJeZaDugoBolovanjeNetoAdd_PREVELIK && daLiJeBolovanjeZbogOzljedeNaRadu == false)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška! Djelatnik {0}\n\r\n\riznos naknade za bolovanje ('NetoDod') {1} je prevelik.\n\r\n\r(max: {2})", 
               ptrans_rec.T_prezimeIme, ptrans_rec.T_NetoAdd.ToStringVv(), maxHZZO_NetoAdd.ToStringVv());

            e.Cancel = true;
            break;
         }
      }
   }

   private bool Get_daLiJeBolovanjeZbogOzljedeNaRadu(Ptrans ptrans_rec)
   {
      bool daLiJeBolovanjeZbogOzljedeNaRadu = false;

      foreach(Ptrane ptrane_rec in placa_rec.TransesNonDeleted2.Where(pte => pte.T_personCD == ptrans_rec.T_personCD))
      {
         if(ptrane_rec.T_stjecatCD == "5209")
         {
            daLiJeBolovanjeZbogOzljedeNaRadu = true;
         }
      }

      return daLiJeBolovanjeZbogOzljedeNaRadu;
   }

   public override void OnCopyRecordPutSomeSpecificFields()
   {
      IncrementPtranoRbrRate();

      if(ZXC.CURR_prjkt_rec.IsCheckStaz)
      {
         CheckGodineStaza();
      }
   }

   private void CheckGodineStaza()
   {
      Ptrans ptrans_rec;
      Person person_rec;
      int rIdx;

      uint calculatedGodineStaza;

      for(rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         ptrans_rec = (Ptrans)GetDgvLineFields1(rIdx, false, null);

         person_rec = Get_Person_FromVvUcSifrar(ptrans_rec.T_personCD);

         if(ptrans_rec.T_godStaza.IsZero() || person_rec.DatePri.IsEmpty()) continue;

         if(person_rec == null)
         {
            ZXC.aim_emsg("Nema persona {0}!", ptrans_rec.T_personCD);
            continue;
         }

         calculatedGodineStaza = person_rec.CalcGodineStaza(ptrans_rec.T_MMYYYY, false);

         if(calculatedGodineStaza != ptrans_rec.T_godStaza)
         {
            person_rec.IssueGodineStazaWarning(ptrans_rec.T_godStaza, calculatedGodineStaza);
         }

      }
   }

   private void IncrementPtranoRbrRate()
   {
      Ptrano ptrano_rec;
      uint lastRbr;
      int rIdx;

      for(rIdx = 0; rIdx < TheG3.RowCount - 1; ++rIdx)
      {
         ptrano_rec = (Ptrano)GetDgvLineFields3(rIdx, false, null);

         lastRbr = PtranoDao.GetMaxObustavaRBR(TheDbConnection, ptrano_rec.T_personCD, ptrano_rec.T_kupdob_cd, ptrano_rec.T_partija);

         if(lastRbr.NotZero())
         {
            TheG3.PutCell(ci3.iT_rbrRate, rIdx, lastRbr + 1);
         }
      }
   }

   public override void CleanUniqueFieldsOnCopyFromOtherRecord()
   {
      Fld_IsLocked = false;
   }

}
