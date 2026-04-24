using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class RvrMjesecDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName,
                     tbx_PosJedCd, tbx_PosJedTk, tbx_PosJedName,
                     tbx_lookUpMMYYYY, tbx_fondSati;
   public VvHamper   hamp_kupDob,hamp_posJed, hamp_zaMjesec;
   private CheckBox  cbx_isTrgFondSati;


   #endregion Fieldz

   #region Constructor

   public RvrMjesecDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul)  : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_MVR
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_KupDob(out hamp_kupDob);
      InitializeHamper_PosJed(out hamp_posJed);
      InitializeHamper_ZaMjesec(out hamp_zaMjesec);

      hamp_mjTroska.Visible = true;
      hamp_mjTroska.Location = new Point(hamp_napomena.Right, hamp_posJed.Bottom);

      hamp_napomena.Location = new Point(nextX, hamp_zaMjesec.Bottom);

      nextY = hamp_mjTroska.Bottom;

      this.ControlForInitialFocus = tbx_lookUpMMYYYY;
   }
   
   public void InitializeHamper_KupDob(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, hamp_tt.Right, hamp_tt.Top, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un- ZXC.Qun4, ZXC.Q3un- ZXC.Qun4, ZXC.Q10un- ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
                       hamper.CreateVvLabel  (0, 0, "Partner:"      , ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

   }
 
   public void InitializeHamper_PosJed(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, hamp_tt.Right, hamp_kupDob.Bottom, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un- ZXC.Qun4, ZXC.Q3un- ZXC.Qun4, ZXC.Q10un- ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,           ZXC.Qun4,           ZXC.Qun4,            ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "PoslJed:"      , ContentAlignment.MiddleRight);
      tbx_PosJedCd   = hamper.CreateVvTextBox(1, 0, "tbx_posJedCd"  , "Sifra poslovne jedinice" , 6);
      tbx_PosJedTk   = hamper.CreateVvTextBox(2, 0, "tbx_posJedTk"  , "Ticker poslovne jedinice", 6);
      tbx_PosJedName = hamper.CreateVvTextBox(3, 0, "tbx_posJedName", "Naziv oslovne jedinice" );

      tbx_PosJedCd  .JAM_ReadOnly =
      tbx_PosJedTk  .JAM_ReadOnly =
      tbx_PosJedName.JAM_ReadOnly = true;

   }

   public void InitializeHamper_ZaMjesec(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", TheTabControl.TabPages[0], false, hamp_dokNum.Left, hamp_dokNum.Bottom, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q3un-ZXC.Qun2, ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,          ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }      
      hamper.VvBottomMargin = hamper.VvTopMargin;

                              hamper.CreateVvLabel        (0, 0, "Za Mjesec:", ContentAlignment.MiddleRight);
      tbx_lookUpMMYYYY      = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_MMYYYY", "Obračun plaće za zadani mjesec u godini");
      tbx_lookUpMMYYYY.Font = ZXC.vvFont.LargeBoldFont;
      tbx_lookUpMMYYYY.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_lookUpMMYYYY.JAM_DataRequired = true;
      tbx_lookUpMMYYYY.JAM_FieldExitMethod_2 = new EventHandler(OnExit_MMYYYY_Set_Colors_Headers_DFS);

                     hamper.CreateVvLabel  (2, 0, "Fond Sati:", ContentAlignment.MiddleRight);
      tbx_fondSati = hamper.CreateVvTextBox(3, 0, "tbx_sati", "Fond radin sati za zadani mjesec prema tablici");
      tbx_fondSati.JAM_ReadOnly = true;

      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;

      tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
      tbx_lookUpMMYYYY.JAM_lui_NameTaker_JAM_Name = tbx_fondSati.JAM_Name;
      tbx_lookUpMMYYYY.JAM_MustTabOutBeforeSubmit = true; // zbog pravilnog GetNextRS_ID 

      cbx_isTrgFondSati = hamper.CreateVvCheckBox_OLD(4, 0, null, "TrgFS", System.Windows.Forms.RightToLeft.No);
      cbx_isTrgFondSati.CheckStateChanged += new EventHandler(cbx_isTrgFondSati_CheckStateChanged_SetFondSati);
      cbx_isTrgFondSati.Enabled = false;


   }
   private void cbx_isTrgFondSati_CheckStateChanged_SetFondSati(object sender, EventArgs e)
   {
      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      CheckBox cbx = sender as CheckBox;
      VvLookUpLista fondSatiLista = /*ZXC.CURR_prjkt_rec.IsTrgRs*/ cbx.Checked ? ZXC.luiListaFondSati_TRG : ZXC.luiListaFondSati_NOR;

      tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);

      Fld_MMYYYY = "";

   }

   private void OnExit_MMYYYY_Set_Colors_Headers_DFS(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_TT.IsEmpty() || Fld_MMYYYY.IsEmpty() || Fld_MMYYYY.Length < 6) return;

      int month     = int.Parse(vvtb.Text.Substring(0,2));
      int year      = int.Parse(vvtb.Text.Substring(2,4));
      int days      = DateTime.DaysInMonth(year, month);

      CreateColumnsHeaderText(days, month, year);

      if(ZXC.TheVvForm.TheVvTabPage.WriteMode != ZXC.WriteMode.None)
      {
         Revalorize_MVRparameters(); 
      }

   }

   private void Revalorize_MVRparameters()
   {
      //decimal lastPersonsPtransT_dfs = ZXC.GetSluzbeniDnevniFondRadniSati(Fld_IsTrgFondSati, Fld_MMYYYY, Fld_FondSati);

      for(int rIdx = 0; rIdx < TheG.RowCount - 1; ++rIdx)
      {
         //TheG.PutCell(ci.iT_moneyA, rIdx, lastPersonsPtransT_dfs);
         TheG.PutCell(ci.iT_moneyB, rIdx, Fld_FondSati          );

         GetDgvLineFields1(rIdx, false, null); // business ok 

         TheG.PutCell(ci.iT_RFS, rIdx, dgvXtrans_rec.R_MVR_RFS);
         TheG.PutCell(ci.iT_PFS, rIdx, dgvXtrans_rec.R_MVR_PFS);
         TheG.PutCell(ci.iT_MFS, rIdx, dgvXtrans_rec.R_MVR_MFS);
      }
   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      //if(isPopulatingSifrar) return;

      ////if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      //TextBox tb = sender as TextBox;
      //Kupdob kupdob_rec;

      //if(tb.Text != this.originalText)
      //{
      //   originalText = tb.Text;
      //   kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

      //   if(kupdob_rec != null && tb.Text != "")
      //   {
      //      Fld_KupDobName = kupdob_rec.Naziv;
      //      Fld_KupDobCd = kupdob_rec.KupdobCD/*RecID*/;
      //      Fld_KupDobTk = kupdob_rec.Ticker;
      //   }
      //   else
      //   {
      //      Fld_KupDobName = Fld_KupDobTk = Fld_KupDobCdAsTxt = "";
      //   }
      //}

      if(isPopulatingSifrar) return;

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob  kupdob_rec;

      if(tb.Text == this.originalText) return;

      this.originalText = tb.Text;

      kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

      ClearAllKupdobFields();

      if(kupdob_rec != null)
      {
         PutAllKupdobFields(kupdob_rec);
      }

   }

   private void PutAllKupdobFields(Kupdob _kupdob_rec)
   {
      #region Classic & Poslovna Jedinica operations 

      if(_kupdob_rec.CentrID.NotZero()) // kupdob_rec je, znaci, Poslovna Jedinica! 
      {
         Kupdob kupdobCentrala_rec = KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == _kupdob_rec.CentrID);

         if(kupdobCentrala_rec == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nema CENTRALE (KCD: [{0}])!", _kupdob_rec.CentrID);
            
            PutCentralaKupdobFields(_kupdob_rec);
         }
         else
         {
            PutCentralaKupdobFields(kupdobCentrala_rec);
            PutPosJedinKupdobFields(_kupdob_rec);
         }
      } // artikl_rec je, znaci, Poslovna Jedinica! 
      else // Classic KupDob 
      {
         PutCentralaKupdobFields(_kupdob_rec);

         // 08.03.2013:
         if(ZXC.LoadPoprat_InProgress == false || _kupdob_rec.KupdobCD.IsNegative()) // ako je negative znaci da smo pozvani od ClearKupdobFields 
         {
            VvHamper.ClearFieldContents(hamp_posJed);

            PutPosJedinKupdobFields(_kupdob_rec);
         }
      }

      #endregion Classic & Poslovna Jedinica operations

   }

   private void PutPosJedinKupdobFields(Kupdob _kupdob_rec)
   {
      
      mixer_rec.V2_ttNum  = Fld_PosJedCd     = _kupdob_rec.KupdobCD;
      mixer_rec.StrC_32   = Fld_PosJedTk     = _kupdob_rec.Ticker;
      mixer_rec.StrF_64   = Fld_PosJedName   = _kupdob_rec.Naziv;
      
   }

   private void PutCentralaKupdobFields(Kupdob kupdob_rec)
   {
      Fld_KupDobCd     = kupdob_rec.KupdobCD;
      Fld_KupDobTk     = kupdob_rec.Ticker;
      Fld_KupDobName   = kupdob_rec.Naziv;

      }

   private void ClearAllKupdobFields()
   {
      PutAllKupdobFields(new Kupdob(0));
   }


   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_personCD_CreateColumn       (ZXC.Q2un           , "Šifra"  , "Šifra");
      T_kpdbNameA_50_CreateColumn   (ZXC.Q4un + ZXC.Qun2, "Prezime", "Prezime", false);
      T_kpdbNameB_50_CreateColumn   (ZXC.Q3un           , "Ime"    , "Ime");

      vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonDgvTextBoxLeave));
      vvtbT_kpdbNameB_50.JAM_ReadOnly = true;

      int decColW = ZXC.Q2un - ZXC.Qun2;
      int strColW = ZXC.QUN  + ZXC.Qun4;

      T_dec01_CreateColumn(decColW, "", "Sati za 01 dan u mjesecu", 1);
      T_str01_CreateColumn(strColW, "01" + "\n" + "VR", "Vrsta radnog vremena za 01 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec02_CreateColumn(decColW, "", "Sati za 02 dan u mjesecu", 1);
      T_str02_CreateColumn(strColW, "02" + "\n" + "VR", "Vrsta radnog vremena za 02 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec03_CreateColumn(decColW, "", "Sati za 03 dan u mjesecu", 1);
      T_str03_CreateColumn(strColW, "03" + "\n" + "VR", "Vrsta radnog vremena za 03 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec04_CreateColumn(decColW, "", "Sati za 04 dan u mjesecu", 1);
      T_str04_CreateColumn(strColW, "04" + "\n" + "VR", "Vrsta radnog vremena za 04 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec05_CreateColumn(decColW, "", "Sati za 05 dan u mjesecu", 1);
      T_str05_CreateColumn(strColW, "05" + "\n" + "VR", "Vrsta radnog vremena za 05 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec06_CreateColumn(decColW, "", "Sati za 06 dan u mjesecu", 1);
      T_str06_CreateColumn(strColW, "06" + "\n" + "VR", "Vrsta radnog vremena za 06 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec07_CreateColumn(decColW, "", "Sati za 07 dan u mjesecu", 1);
      T_str07_CreateColumn(strColW, "07" + "\n" + "VR", "Vrsta radnog vremena za 07 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec08_CreateColumn(decColW, "", "Sati za 08 dan u mjesecu", 1);
      T_str08_CreateColumn(strColW, "08" + "\n" + "VR", "Vrsta radnog vremena za 08 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec09_CreateColumn(decColW, "", "Sati za 09 dan u mjesecu", 1);
      T_str09_CreateColumn(strColW, "09" + "\n" + "VR", "Vrsta radnog vremena za 09 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec10_CreateColumn(decColW, "", "Sati za 10 dan u mjesecu", 1);
      T_str10_CreateColumn(strColW, "10" + "\n" + "VR", "Vrsta radnog vremena za 10 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec11_CreateColumn(decColW, "", "Sati za 11 dan u mjesecu", 1);
      T_str11_CreateColumn(strColW, "11" + "\n" + "VR", "Vrsta radnog vremena za 11 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec12_CreateColumn(decColW, "", "Sati za 12 dan u mjesecu", 1);
      T_str12_CreateColumn(strColW, "12" + "\n" + "VR", "Vrsta radnog vremena za 12 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec13_CreateColumn(decColW, "", "Sati za 13 dan u mjesecu", 1);
      T_str13_CreateColumn(strColW, "13" + "\n" + "VR", "Vrsta radnog vremena za 13 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec14_CreateColumn(decColW, "", "Sati za 14 dan u mjesecu", 1);
      T_str14_CreateColumn(strColW, "14" + "\n" + "VR", "Vrsta radnog vremena za 14 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec15_CreateColumn(decColW, "", "Sati za 15 dan u mjesecu", 1);
      T_str15_CreateColumn(strColW, "15" + "\n" + "VR", "Vrsta radnog vremena za 15 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec16_CreateColumn(decColW, "", "Sati za 16 dan u mjesecu", 1);
      T_str16_CreateColumn(strColW, "16" + "\n" + "VR", "Vrsta radnog vremena za 16 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec17_CreateColumn(decColW, "", "Sati za 17 dan u mjesecu", 1);
      T_str17_CreateColumn(strColW, "17" + "\n" + "VR", "Vrsta radnog vremena za 17 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec18_CreateColumn(decColW, "", "Sati za 18 dan u mjesecu", 1);
      T_str18_CreateColumn(strColW, "18" + "\n" + "VR", "Vrsta radnog vremena za 18 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec19_CreateColumn(decColW, "", "Sati za 19 dan u mjesecu", 1);
      T_str19_CreateColumn(strColW, "19" + "\n" + "VR", "Vrsta radnog vremena za 19 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec20_CreateColumn(decColW, "", "Sati za 20 dan u mjesecu", 1);
      T_str20_CreateColumn(strColW, "20" + "\n" + "VR", "Vrsta radnog vremena za 20 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec21_CreateColumn(decColW, "", "Sati za 21 dan u mjesecu", 1);
      T_str21_CreateColumn(strColW, "21" + "\n" + "VR", "Vrsta radnog vremena za 21 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec22_CreateColumn(decColW, "", "Sati za 22 dan u mjesecu", 1);
      T_str22_CreateColumn(strColW, "22" + "\n" + "VR", "Vrsta radnog vremena za 22 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec23_CreateColumn(decColW, "", "Sati za 23 dan u mjesecu", 1);
      T_str23_CreateColumn(strColW, "23" + "\n" + "VR", "Vrsta radnog vremena za 23 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec24_CreateColumn(decColW, "", "Sati za 24 dan u mjesecu", 1);
      T_str24_CreateColumn(strColW, "24" + "\n" + "VR", "Vrsta radnog vremena za 24 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec25_CreateColumn(decColW, "", "Sati za 25 dan u mjesecu", 1);
      T_str25_CreateColumn(strColW, "25" + "\n" + "VR", "Vrsta radnog vremena za 25 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec26_CreateColumn(decColW, "", "Sati za 26 dan u mjesecu", 1);
      T_str26_CreateColumn(strColW, "26" + "\n" + "VR", "Vrsta radnog vremena za 26 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec27_CreateColumn(decColW, "", "Sati za 27 dan u mjesecu", 1);
      T_str27_CreateColumn(strColW, "27" + "\n" + "VR", "Vrsta radnog vremena za 27 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec28_CreateColumn(decColW, "", "Sati za 28 dan u mjesecu", 1);
      T_str28_CreateColumn(strColW, "28" + "\n" + "VR", "Vrsta radnog vremena za 28 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec29_CreateColumn(decColW, "", "Sati za 29 dan u mjesecu", 1);
      T_str29_CreateColumn(strColW, "29" + "\n" + "VR", "Vrsta radnog vremena za 29 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec30_CreateColumn(decColW, "", "Sati za 30 dan u mjesecu", 1);
      T_str30_CreateColumn(strColW, "30" + "\n" + "VR", "Vrsta radnog vremena za 30 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);
      T_dec31_CreateColumn(decColW, "", "Sati za 31 dan u mjesecu", 1);
      T_str31_CreateColumn(strColW, "31" + "\n" + "VR", "Vrsta radnog vremena za 31 dan u mjesecu", ZXC.luiListaMixRadVrijemeMVR, false);

    //T_moneyB_CreateColumn(ZXC.Q2un,  1, "SMFS", "SluzbeniMjFondSati", true, false);         // unvisible! 
      T_moneyB_CreateColumn(ZXC.Qun8, 0, "", "SluzbeniMjFondSati", true, /*false*/true); // NE. onda ne sljaka copyranje reda...?! 

      T_moneyA_CreateColumn(ZXC.Q2un - ZXC.Qun4, "DFS", "Dnevni fond sati radnika", 1);
      R_RFS_CreateColumn   (ZXC.Q2un           , "RFS", "Stvarno ostvareni fond sati"         , 1);
      R_MFS_CreateColumn   (ZXC.Q2un           , "MFS", "Mjesecčni službeni fond sati radnika", 1);
      R_PFS_CreateColumn   (ZXC.Q2un           , "PFS", "Prekovremeni sati (MFS - RFS)"       , 1);

      vvtbT_moneyA.JAM_ReadOnly = true;
      vvtbT_moneyA.JAM_ForeColor = Color.DarkBlue;

      TheG.ColumnHeadersHeight = ZXC.Q2un;

      TheG.Tag = Mixer.TT_MVR;

      foreach(DataGridViewColumn col in TheG.Columns)
      {
         if(col != null)
         {
            if(col.Name.StartsWith("t_str")) col.DefaultCellStyle.Font = ZXC.vvFont.SmallBoldFont;
            else                             col.DefaultCellStyle.Font = ZXC.vvFont.SmallFont;
         }
      }
      foreach(DataGridViewColumn col in TheSumGrid.Columns)
      {
         if(col != null)
         {
            col.DefaultCellStyle.Font = ZXC.vvFont.SmallFont;
         }
      }

      vvtbT_str01.JAM_CharacterCasing = 
      vvtbT_str02.JAM_CharacterCasing = 
      vvtbT_str03.JAM_CharacterCasing = 
      vvtbT_str04.JAM_CharacterCasing = 
      vvtbT_str05.JAM_CharacterCasing = 
      vvtbT_str06.JAM_CharacterCasing = 
      vvtbT_str07.JAM_CharacterCasing = 
      vvtbT_str08.JAM_CharacterCasing = 
      vvtbT_str09.JAM_CharacterCasing = 
      vvtbT_str10.JAM_CharacterCasing = 
      vvtbT_str11.JAM_CharacterCasing = 
      vvtbT_str12.JAM_CharacterCasing = 
      vvtbT_str13.JAM_CharacterCasing = 
      vvtbT_str14.JAM_CharacterCasing = 
      vvtbT_str15.JAM_CharacterCasing = 
      vvtbT_str16.JAM_CharacterCasing = 
      vvtbT_str17.JAM_CharacterCasing = 
      vvtbT_str18.JAM_CharacterCasing = 
      vvtbT_str19.JAM_CharacterCasing = 
      vvtbT_str20.JAM_CharacterCasing = 
      vvtbT_str21.JAM_CharacterCasing = 
      vvtbT_str22.JAM_CharacterCasing = 
      vvtbT_str23.JAM_CharacterCasing = 
      vvtbT_str24.JAM_CharacterCasing = 
      vvtbT_str25.JAM_CharacterCasing = 
      vvtbT_str26.JAM_CharacterCasing = 
      vvtbT_str27.JAM_CharacterCasing = 
      vvtbT_str28.JAM_CharacterCasing = 
      vvtbT_str29.JAM_CharacterCasing = 
      vvtbT_str30.JAM_CharacterCasing = 
      vvtbT_str31.JAM_CharacterCasing = CharacterCasing.Upper;

   }
   
   private void CreateColumnsHeaderText(int days, int month, int year)
   { 
      TheG.Columns["T_dec01"].HeaderText = "01\n" + CovertToCro(new DateTime(year, month, 1).DayOfWeek.ToString());
      TheG.Columns["T_dec02"].HeaderText = "02\n" + CovertToCro(new DateTime(year, month, 2).DayOfWeek.ToString());
      TheG.Columns["T_dec03"].HeaderText = "03\n" + CovertToCro(new DateTime(year, month, 3).DayOfWeek.ToString());
      TheG.Columns["T_dec04"].HeaderText = "04\n" + CovertToCro(new DateTime(year, month, 4).DayOfWeek.ToString());
      TheG.Columns["T_dec05"].HeaderText = "05\n" + CovertToCro(new DateTime(year, month, 5).DayOfWeek.ToString());
      TheG.Columns["T_dec06"].HeaderText = "06\n" + CovertToCro(new DateTime(year, month, 6).DayOfWeek.ToString());
      TheG.Columns["T_dec07"].HeaderText = "07\n" + CovertToCro(new DateTime(year, month, 7).DayOfWeek.ToString());
      TheG.Columns["T_dec08"].HeaderText = "08\n" + CovertToCro(new DateTime(year, month, 8).DayOfWeek.ToString());
      TheG.Columns["T_dec09"].HeaderText = "09\n" + CovertToCro(new DateTime(year, month, 9).DayOfWeek.ToString());
      TheG.Columns["T_dec10"].HeaderText = "10\n" + CovertToCro(new DateTime(year, month, 10).DayOfWeek.ToString());
      TheG.Columns["T_dec11"].HeaderText = "11\n" + CovertToCro(new DateTime(year, month, 11).DayOfWeek.ToString());
      TheG.Columns["T_dec12"].HeaderText = "12\n" + CovertToCro(new DateTime(year, month, 12).DayOfWeek.ToString());
      TheG.Columns["T_dec13"].HeaderText = "13\n" + CovertToCro(new DateTime(year, month, 13).DayOfWeek.ToString());
      TheG.Columns["T_dec14"].HeaderText = "14\n" + CovertToCro(new DateTime(year, month, 14).DayOfWeek.ToString());
      TheG.Columns["T_dec15"].HeaderText = "15\n" + CovertToCro(new DateTime(year, month, 15).DayOfWeek.ToString());
      TheG.Columns["T_dec16"].HeaderText = "16\n" + CovertToCro(new DateTime(year, month, 16).DayOfWeek.ToString());
      TheG.Columns["T_dec17"].HeaderText = "17\n" + CovertToCro(new DateTime(year, month, 17).DayOfWeek.ToString());
      TheG.Columns["T_dec18"].HeaderText = "18\n" + CovertToCro(new DateTime(year, month, 18).DayOfWeek.ToString());
      TheG.Columns["T_dec19"].HeaderText = "19\n" + CovertToCro(new DateTime(year, month, 19).DayOfWeek.ToString());
      TheG.Columns["T_dec20"].HeaderText = "20\n" + CovertToCro(new DateTime(year, month, 20).DayOfWeek.ToString());
      TheG.Columns["T_dec21"].HeaderText = "21\n" + CovertToCro(new DateTime(year, month, 21).DayOfWeek.ToString());
      TheG.Columns["T_dec22"].HeaderText = "22\n" + CovertToCro(new DateTime(year, month, 22).DayOfWeek.ToString());
      TheG.Columns["T_dec23"].HeaderText = "23\n" + CovertToCro(new DateTime(year, month, 23).DayOfWeek.ToString());
      TheG.Columns["T_dec24"].HeaderText = "24\n" + CovertToCro(new DateTime(year, month, 24).DayOfWeek.ToString());
      TheG.Columns["T_dec25"].HeaderText = "25\n" + CovertToCro(new DateTime(year, month, 25).DayOfWeek.ToString());
      TheG.Columns["T_dec26"].HeaderText = "26\n" + CovertToCro(new DateTime(year, month, 26).DayOfWeek.ToString());
      TheG.Columns["T_dec27"].HeaderText = "27\n" + CovertToCro(new DateTime(year, month, 27).DayOfWeek.ToString());
      TheG.Columns["T_dec28"].HeaderText = "28\n" + CovertToCro(new DateTime(year, month, 28).DayOfWeek.ToString());
      TheG.Columns["T_dec29"].Visible = days > 28;
      TheG.Columns["T_dec30"].Visible = days > 29;
      TheG.Columns["T_dec31"].Visible = days > 30;

      TheSumGrid.Columns["T_dec29"].Visible = days > 28;
      TheSumGrid.Columns["T_dec30"].Visible = days > 29;
      TheSumGrid.Columns["T_dec31"].Visible = days > 30;


      TheG.Columns["T_str29"].Visible = true ? (TheG.Columns["T_str29"].Visible && TheG.Columns["T_dec29"].Visible) : false;
      TheG.Columns["T_str30"].Visible = true ? (TheG.Columns["T_str29"].Visible && TheG.Columns["T_dec30"].Visible) : false;
      TheG.Columns["T_str31"].Visible = true ? (TheG.Columns["T_str29"].Visible && TheG.Columns["T_dec31"].Visible) : false;

      if(TheG.Columns["T_dec29"].Visible) TheG.Columns["T_dec29"].HeaderText = "29\n" + CovertToCro(new DateTime(year, month, 29).DayOfWeek.ToString());
      if(TheG.Columns["T_dec30"].Visible) TheG.Columns["T_dec30"].HeaderText = "30\n" + CovertToCro(new DateTime(year, month, 30).DayOfWeek.ToString());
      if(TheG.Columns["T_dec31"].Visible) TheG.Columns["T_dec31"].HeaderText = "31\n" + CovertToCro(new DateTime(year, month, 31).DayOfWeek.ToString());


      VvTextBox vvtb;
      foreach(DataGridViewColumn col in TheG.Columns)
      {
         vvtb = col.Tag as VvTextBox;

         //if(vvtb != null)
         if(col != null)
         {
                 if(col.HeaderText.Contains("NE"))             col.DefaultCellStyle.BackColor = clr_MVR_NEdjelja;
            else if(col.HeaderText.Contains("SU"))             col.DefaultCellStyle.BackColor = clr_MVR_SUbota  ;
            else if(col.Name.StartsWith("t_str"))              col.DefaultCellStyle.BackColor = clr_MVR_VR_Back ;
            else if(vvtb != null && vvtb.JAM_ReadOnly == true) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
            else
            {
               if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
               else                                                           col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
            }
         }
      }
   }

   private string CovertToCro(string header)
   {
      switch(header)
      {
         case "Monday"   : return "PO";
         case "Tuesday"  : return "UT";
         case "Wednesday": return "SR";
         case "Thursday" : return "ČE";
         case "Friday"   : return "PE";
         case "Saturday" : return "SU";
         case "Sunday"   : return "NE";
         default         : return "";
      }
   }


   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd          { get { return tbx_kupDobCd         .GetSomeRecIDField(); } set { tbx_kupDobCd         .PutSomeRecIDField(value) ; } }
   public string  Fld_KupDobCdAsTxt     { get { return tbx_kupDobCd         .Text               ; } set { tbx_kupDobCd         .Text = value             ; } }
   public string  Fld_KupDobName        { get { return tbx_kupDobName       .Text               ; } set { tbx_kupDobName       .Text = value             ; } }
   public string  Fld_KupDobTk          { get { return tbx_kupDobTk         .Text               ; } set { tbx_kupDobTk         .Text = value             ; } }
   public uint    Fld_PosJedCd          { get { return tbx_PosJedCd         .GetSomeRecIDField(); } set { tbx_PosJedCd         .PutSomeRecIDField(value) ; } }
   public string  Fld_PosJedTk          { get { return tbx_PosJedTk         .Text               ; } set { tbx_PosJedTk         .Text = value             ; } }
   public string  Fld_PosJedName        { get { return tbx_PosJedName       .Text               ; } set { tbx_PosJedName       .Text = value             ; } }
   public string  Fld_MMYYYY            { get { return tbx_lookUpMMYYYY     .Text               ; } set { tbx_lookUpMMYYYY     .Text = value             ; } }
   public decimal Fld_FondSati          { get { return tbx_fondSati         .GetDecimalField()  ; } set { tbx_fondSati         .PutDecimalField(value)   ; } }
   public bool    Fld_IsTrgFondSati     { get { return cbx_isTrgFondSati    .Checked            ; } set { cbx_isTrgFondSati    .Checked = value          ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd    )) Fld_KupDobCd      = mixer_rec.KupdobCD  ;
      if(CtrlOK(tbx_kupDobName  )) Fld_KupDobName    = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk    )) Fld_KupDobTk      = mixer_rec.KupdobTK  ;
                                                     
      if(CtrlOK(tbx_PosJedCd    )) Fld_PosJedCd      = mixer_rec.V2_ttNum  ;
      if(CtrlOK(tbx_PosJedTk    )) Fld_PosJedTk      = mixer_rec.StrC_32   ;
      if(CtrlOK(tbx_PosJedName  )) Fld_PosJedName    = mixer_rec.StrF_64   ;
      if(CtrlOK(tbx_lookUpMMYYYY)) Fld_MMYYYY        = mixer_rec.StrD_32   ;
      if(CtrlOK(tbx_fondSati    )) Fld_FondSati      = mixer_rec.MoneyA    ;

                                   Fld_IsTrgFondSati = mixer_rec.IsXxx    ;

      Fld_TtOpis = ZXC.luiListaMixTypeEvidencija.GetNameForThisCd(mixer_rec.TT);
      OnExit_MMYYYY_Set_Colors_Headers_DFS(tbx_lookUpMMYYYY, null);

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd    )) mixer_rec.KupdobCD   = Fld_KupDobCd  ;
      if(CtrlOK(tbx_kupDobName  )) mixer_rec.KupdobName = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk    )) mixer_rec.KupdobTK   = Fld_KupDobTk  ;
   
      if(CtrlOK(tbx_PosJedCd    )) mixer_rec.V2_ttNum   = Fld_PosJedCd   ;
      if(CtrlOK(tbx_PosJedTk    )) mixer_rec.StrC_32    = Fld_PosJedTk   ;
      if(CtrlOK(tbx_PosJedName  )) mixer_rec.StrF_64    = Fld_PosJedName ;
      if(CtrlOK(tbx_lookUpMMYYYY)) mixer_rec.StrD_32    = Fld_MMYYYY     ;
      if(CtrlOK(tbx_fondSati    )) mixer_rec.MoneyA     = Fld_FondSati   ;
      
                                   mixer_rec.IsXxx      = Fld_IsTrgFondSati;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public RvrMjesecFilterUC TheRvrMjesecFilterUC { get; set; }
   public RvrMjesecFilter   TheRvrMjesecFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheRvrMjesecFilter = new RvrMjesecFilter(this);

      TheRvrMjesecFilterUC        = new RvrMjesecFilterUC(this);
      TheRvrMjesecFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheRvrMjesecFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //RvrMjesecFilter mixerFilter = (RvrMjesecFilter)vvRptFilter;
      RvrFilter mixerFilter = (RvrFilter)vvRptFilter;

      //switch(mixerFilter.PrintRvrMjesec)
      switch(mixerFilter.PrintRvr)
      {
         //case RvrMjesecFilter.PrintRvrMjesecEnum.RvrMjesec: specificMixerReport = new RptX_RvrMjesec(new Vektor.Reports.XIZ.CR_RvrMjesecduc(), "RvrMjesec", mixerFilter); break;
         case RvrFilter.PrintRvrEnum.RVR: specificMixerReport = new RptX_RVR(new Vektor.Reports.XIZ.CR_RVRduc(), "RvrMjesec", mixerFilter); break;

         //default: ZXC.aim_emsg("{0}\nPrintSomeRvrMjesecDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintRvrMjesec); return null;
         default: ZXC.aim_emsg("{0}\nPrintSomeRvrMjesecDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintRvr); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheRvrMjesecFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheRvrMjesecFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors
   
   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RVR, Color.Empty, clr_RVR);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "MjEvidencija"; }
   }

   #endregion overrideTabPageTitle

}

public class RvrMjesecFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public RvrMjesecFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width  = hamper4buttons.Width  + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private RvrMjesecFilter TheRvrMjesecFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as RvrMjesecFilter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRvrMjesecFilter = (RvrMjesecFilter)_filter_data;

      if(TheRvrMjesecFilter != null)
      {
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
   }

   #endregion Put & GetFilterFields

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
   }

   #endregion AddFilterMemberz()

}

public class RvrMjesecFilter : VvRpt_Mix_Filter
{

   public enum PrintRvrMjesecEnum
   {
      RvrMjesec
   }

   public PrintRvrMjesecEnum PrintRvrMjesec { get; set; }

   public RvrMjesecDUC theDUC;

   public RvrMjesecFilter(RvrMjesecDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear    = DateTime.Now.Year;
      PrintRvrMjesec        = PrintRvrMjesecEnum.RvrMjesec;
   }

   #endregion SetDefaultFilterValues()

}
