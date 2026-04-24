using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class AvrDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName,
                     tbx_PosJedCd, tbx_PosJedTk, tbx_PosJedName, tbx_PosJedAdresa,
                     tbx_lookUpMMYYYY;
   public VvHamper hamp_kupDob, hamp_posJed, hamp_zaMjesec;
   
   #endregion Fieldz

   #region Constructor

   public AvrDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul): base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_AVR
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_KupDob(out hamp_kupDob);
      InitializeHamper_PosJed(out hamp_posJed);
      InitializeHamper_ZaMjesec(out hamp_zaMjesec);

      hamp_napomena.Location = new Point(nextX, hamp_zaMjesec.Bottom);

      nextY = hamp_posJed.Bottom;

      this.ControlForInitialFocus = tbx_lookUpMMYYYY;
   }

   public void InitializeHamper_KupDob(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, hamp_tt.Right, hamp_tt.Top, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel(0, 0, "Partner:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera ugovaratelja posla", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera ugovaratelja posla", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera ugovaratelja posla", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

   }

   public void InitializeHamper_PosJed(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, hamp_tt.Right, hamp_kupDob.Bottom, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Lokacija:"     , ContentAlignment.MiddleRight);
      tbx_PosJedCd   = hamper.CreateVvTextBox(1, 0, "tbx_posJedCd"  , "Sifra  lokacije - radnog mjesta", 6);
      tbx_PosJedTk   = hamper.CreateVvTextBox(2, 0, "tbx_posJedTk"  , "Ticker lokacije - radnog mjesta", 6);
      tbx_PosJedName = hamper.CreateVvTextBox(3, 0, "tbx_posJedName", "Naziv  lokacije - radnog mjesta");

                         hamper.CreateVvLabel  (0, 1, "Adresa:"        , ContentAlignment.MiddleRight);
      tbx_PosJedAdresa = hamper.CreateVvTextBox(1, 1, "tbx_posJedUlica", "Naziv  lokacije - radnog mjesta", 74, 2, 0);

      tbx_PosJedCd    .JAM_ReadOnly =
      tbx_PosJedTk    .JAM_ReadOnly =
      tbx_PosJedName  .JAM_ReadOnly =
      tbx_PosJedAdresa.JAM_ReadOnly = true;

   }

   public void InitializeHamper_ZaMjesec(out VvHamper hamper)
   {
      hamper = new VvHamper(5, 1, "", TheTabControl.TabPages[0], false, hamp_dokNum.Left, hamp_dokNum.Bottom, razmakHamp);
      //                                     0                 1                   2                  3        
      hamper.VvColWdt     = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q3un - ZXC.Qun2, ZXC.Q2un, ZXC.Q4un };
      hamper.VvSpcBefCol  = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel        (0, 0, "Za Mjesec:", ContentAlignment.MiddleRight);
      tbx_lookUpMMYYYY = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_MMYYYY", "Obračun plaće za zadani mjesec u godini");
      tbx_lookUpMMYYYY.Font = ZXC.vvFont.LargeBoldFont;
      tbx_lookUpMMYYYY.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_lookUpMMYYYY.JAM_DataRequired = true;
      tbx_lookUpMMYYYY.JAM_FieldExitMethod_2 = new EventHandler(OnExit_MMYYYY_Set_Colors_Headers_DFS);

      VvLookUpLista fondSatiLista = ZXC.luiListaFondSati_NOR;

      tbx_lookUpMMYYYY.JAM_Set_LookUpTable(fondSatiLista, (int)ZXC.Kolona.prva);
      tbx_lookUpMMYYYY.JAM_MustTabOutBeforeSubmit = true; // zbog pravilnog GetNextRS_ID 

   }

   private void OnExit_MMYYYY_Set_Colors_Headers_DFS(object sender, EventArgs e)
   {
      VvTextBox vvtb = sender as VvTextBox;

      if(vvtb.EditedHasChanges() == false) return;

      if(Fld_TT.IsEmpty() || Fld_MMYYYY.IsEmpty() || Fld_MMYYYY.Length < 6) return;

      int month = int.Parse(vvtb.Text.Substring(0, 2));
      int year  = int.Parse(vvtb.Text.Substring(2, 4));
      int days  = DateTime.DaysInMonth(year, month);

      CreateColumnsHeaderText(days, month, year);
   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
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
      Kupdob kupdob_rec;

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

      mixer_rec.V2_ttNum = Fld_PosJedCd      = _kupdob_rec.KupdobCD;
      mixer_rec.StrC_32  = Fld_PosJedTk      = _kupdob_rec.Ticker;
      mixer_rec.StrF_64  = Fld_PosJedName    = _kupdob_rec.Naziv;
      mixer_rec.StrB_128 = Fld_PosJedAdresa  = _kupdob_rec.Grad + ", " + _kupdob_rec.Ulica1;

   }

   private void PutCentralaKupdobFields(Kupdob kupdob_rec)
   {
      Fld_KupDobCd = kupdob_rec.KupdobCD;
      Fld_KupDobTk = kupdob_rec.Ticker;
      Fld_KupDobName = kupdob_rec.Naziv;
   }

   private void ClearAllKupdobFields()
   {
      PutAllKupdobFields(new Kupdob(0));
   }


   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_personCD_CreateColumn    (ZXC.Q2un           , "Šifra", "Šifra");
      T_kpdbNameA_50_CreateColumn(ZXC.Q4un + ZXC.Qun2, "Prezime", "Prezime", false);
      T_kpdbNameB_50_CreateColumn(ZXC.Q3un           , "Ime", "Ime");

      vvtbT_kpdbNameA_50.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonDgvTextBoxLeave));
      vvtbT_kpdbNameB_50.JAM_ReadOnly = true;

      int decColW = ZXC.Q2un + ZXC.Qun2;
      int strColW = ZXC.QUN  + ZXC.Qun4;

      T_Time_01_CreateColumn(decColW, "", "Sati od za 01 dan u mjesecu"); T_Time_01_2_CreateColumn(decColW, "", "Sati do za 01 dan u mjesecu"); T_str01_CreateColumn(strColW, "01" + "\n" + "VR", "Vrsta radnog vremena za 01 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_02_CreateColumn(decColW, "", "Sati od za 02 dan u mjesecu"); T_Time_02_2_CreateColumn(decColW, "", "Sati do za 02 dan u mjesecu"); T_str02_CreateColumn(strColW, "02" + "\n" + "VR", "Vrsta radnog vremena za 02 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_03_CreateColumn(decColW, "", "Sati od za 03 dan u mjesecu"); T_Time_03_2_CreateColumn(decColW, "", "Sati do za 03 dan u mjesecu"); T_str03_CreateColumn(strColW, "03" + "\n" + "VR", "Vrsta radnog vremena za 03 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_04_CreateColumn(decColW, "", "Sati od za 04 dan u mjesecu"); T_Time_04_2_CreateColumn(decColW, "", "Sati do za 04 dan u mjesecu"); T_str04_CreateColumn(strColW, "04" + "\n" + "VR", "Vrsta radnog vremena za 04 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_05_CreateColumn(decColW, "", "Sati od za 05 dan u mjesecu"); T_Time_05_2_CreateColumn(decColW, "", "Sati do za 05 dan u mjesecu"); T_str05_CreateColumn(strColW, "05" + "\n" + "VR", "Vrsta radnog vremena za 05 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_06_CreateColumn(decColW, "", "Sati od za 06 dan u mjesecu"); T_Time_06_2_CreateColumn(decColW, "", "Sati do za 06 dan u mjesecu"); T_str06_CreateColumn(strColW, "06" + "\n" + "VR", "Vrsta radnog vremena za 06 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_07_CreateColumn(decColW, "", "Sati od za 07 dan u mjesecu"); T_Time_07_2_CreateColumn(decColW, "", "Sati do za 07 dan u mjesecu"); T_str07_CreateColumn(strColW, "07" + "\n" + "VR", "Vrsta radnog vremena za 07 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_08_CreateColumn(decColW, "", "Sati od za 08 dan u mjesecu"); T_Time_08_2_CreateColumn(decColW, "", "Sati do za 08 dan u mjesecu"); T_str08_CreateColumn(strColW, "08" + "\n" + "VR", "Vrsta radnog vremena za 08 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_09_CreateColumn(decColW, "", "Sati od za 09 dan u mjesecu"); T_Time_09_2_CreateColumn(decColW, "", "Sati do za 09 dan u mjesecu"); T_str09_CreateColumn(strColW, "09" + "\n" + "VR", "Vrsta radnog vremena za 09 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_10_CreateColumn(decColW, "", "Sati od za 10 dan u mjesecu"); T_Time_10_2_CreateColumn(decColW, "", "Sati do za 10 dan u mjesecu"); T_str10_CreateColumn(strColW, "10" + "\n" + "VR", "Vrsta radnog vremena za 10 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_11_CreateColumn(decColW, "", "Sati od za 11 dan u mjesecu"); T_Time_11_2_CreateColumn(decColW, "", "Sati do za 11 dan u mjesecu"); T_str11_CreateColumn(strColW, "11" + "\n" + "VR", "Vrsta radnog vremena za 11 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_12_CreateColumn(decColW, "", "Sati od za 12 dan u mjesecu"); T_Time_12_2_CreateColumn(decColW, "", "Sati do za 12 dan u mjesecu"); T_str12_CreateColumn(strColW, "12" + "\n" + "VR", "Vrsta radnog vremena za 12 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_13_CreateColumn(decColW, "", "Sati od za 13 dan u mjesecu"); T_Time_13_2_CreateColumn(decColW, "", "Sati do za 13 dan u mjesecu"); T_str13_CreateColumn(strColW, "13" + "\n" + "VR", "Vrsta radnog vremena za 13 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_14_CreateColumn(decColW, "", "Sati od za 14 dan u mjesecu"); T_Time_14_2_CreateColumn(decColW, "", "Sati do za 14 dan u mjesecu"); T_str14_CreateColumn(strColW, "14" + "\n" + "VR", "Vrsta radnog vremena za 14 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_15_CreateColumn(decColW, "", "Sati od za 15 dan u mjesecu"); T_Time_15_2_CreateColumn(decColW, "", "Sati do za 15 dan u mjesecu"); T_str15_CreateColumn(strColW, "15" + "\n" + "VR", "Vrsta radnog vremena za 15 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_16_CreateColumn(decColW, "", "Sati od za 16 dan u mjesecu"); T_Time_16_2_CreateColumn(decColW, "", "Sati do za 16 dan u mjesecu"); T_str16_CreateColumn(strColW, "16" + "\n" + "VR", "Vrsta radnog vremena za 16 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_17_CreateColumn(decColW, "", "Sati od za 17 dan u mjesecu"); T_Time_17_2_CreateColumn(decColW, "", "Sati do za 17 dan u mjesecu"); T_str17_CreateColumn(strColW, "17" + "\n" + "VR", "Vrsta radnog vremena za 17 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_18_CreateColumn(decColW, "", "Sati od za 18 dan u mjesecu"); T_Time_18_2_CreateColumn(decColW, "", "Sati do za 18 dan u mjesecu"); T_str18_CreateColumn(strColW, "18" + "\n" + "VR", "Vrsta radnog vremena za 18 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_19_CreateColumn(decColW, "", "Sati od za 19 dan u mjesecu"); T_Time_19_2_CreateColumn(decColW, "", "Sati do za 19 dan u mjesecu"); T_str19_CreateColumn(strColW, "19" + "\n" + "VR", "Vrsta radnog vremena za 19 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_20_CreateColumn(decColW, "", "Sati od za 20 dan u mjesecu"); T_Time_20_2_CreateColumn(decColW, "", "Sati do za 20 dan u mjesecu"); T_str20_CreateColumn(strColW, "20" + "\n" + "VR", "Vrsta radnog vremena za 20 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_21_CreateColumn(decColW, "", "Sati od za 21 dan u mjesecu"); T_Time_21_2_CreateColumn(decColW, "", "Sati do za 21 dan u mjesecu"); T_str21_CreateColumn(strColW, "21" + "\n" + "VR", "Vrsta radnog vremena za 21 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_22_CreateColumn(decColW, "", "Sati od za 22 dan u mjesecu"); T_Time_22_2_CreateColumn(decColW, "", "Sati do za 22 dan u mjesecu"); T_str22_CreateColumn(strColW, "22" + "\n" + "VR", "Vrsta radnog vremena za 22 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_23_CreateColumn(decColW, "", "Sati od za 23 dan u mjesecu"); T_Time_23_2_CreateColumn(decColW, "", "Sati do za 23 dan u mjesecu"); T_str23_CreateColumn(strColW, "23" + "\n" + "VR", "Vrsta radnog vremena za 23 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_24_CreateColumn(decColW, "", "Sati od za 24 dan u mjesecu"); T_Time_24_2_CreateColumn(decColW, "", "Sati do za 24 dan u mjesecu"); T_str24_CreateColumn(strColW, "24" + "\n" + "VR", "Vrsta radnog vremena za 24 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_25_CreateColumn(decColW, "", "Sati od za 25 dan u mjesecu"); T_Time_25_2_CreateColumn(decColW, "", "Sati do za 25 dan u mjesecu"); T_str25_CreateColumn(strColW, "25" + "\n" + "VR", "Vrsta radnog vremena za 25 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_26_CreateColumn(decColW, "", "Sati od za 26 dan u mjesecu"); T_Time_26_2_CreateColumn(decColW, "", "Sati do za 26 dan u mjesecu"); T_str26_CreateColumn(strColW, "26" + "\n" + "VR", "Vrsta radnog vremena za 26 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_27_CreateColumn(decColW, "", "Sati od za 27 dan u mjesecu"); T_Time_27_2_CreateColumn(decColW, "", "Sati do za 27 dan u mjesecu"); T_str27_CreateColumn(strColW, "27" + "\n" + "VR", "Vrsta radnog vremena za 27 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_28_CreateColumn(decColW, "", "Sati od za 28 dan u mjesecu"); T_Time_28_2_CreateColumn(decColW, "", "Sati do za 28 dan u mjesecu"); T_str28_CreateColumn(strColW, "28" + "\n" + "VR", "Vrsta radnog vremena za 28 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_29_CreateColumn(decColW, "", "Sati od za 29 dan u mjesecu"); T_Time_29_2_CreateColumn(decColW, "", "Sati do za 29 dan u mjesecu"); T_str29_CreateColumn(strColW, "29" + "\n" + "VR", "Vrsta radnog vremena za 29 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_30_CreateColumn(decColW, "", "Sati od za 30 dan u mjesecu"); T_Time_30_2_CreateColumn(decColW, "", "Sati do za 30 dan u mjesecu"); T_str30_CreateColumn(strColW, "30" + "\n" + "VR", "Vrsta radnog vremena za 30 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); ColumnForScroll_CreateColumn(ZXC.Qun8);
      T_Time_31_CreateColumn(decColW, "", "Sati od za 31 dan u mjesecu"); T_Time_31_2_CreateColumn(decColW, "", "Sati do za 31 dan u mjesecu"); T_str31_CreateColumn(strColW, "31" + "\n" + "VR", "Vrsta radnog vremena za 31 dan u mjesecu", null /*ZXC.luiListaMixRadVrijemRVR2*/, false); 

      TheG.ColumnHeadersHeight = ZXC.Q2un;

      TheG.Tag = Mixer.TT_AVR; // za boju kolona str, SU, NE

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
      TheG.Columns["T_dec01"].HeaderText = TheG.Columns["T_dec01_2"].HeaderText = "01\n" + CovertToCro(new DateTime(year, month,  1));
      TheG.Columns["T_dec02"].HeaderText = TheG.Columns["T_dec02_2"].HeaderText = "02\n" + CovertToCro(new DateTime(year, month,  2));
      TheG.Columns["T_dec03"].HeaderText = TheG.Columns["T_dec03_2"].HeaderText = "03\n" + CovertToCro(new DateTime(year, month,  3));
      TheG.Columns["T_dec04"].HeaderText = TheG.Columns["T_dec04_2"].HeaderText = "04\n" + CovertToCro(new DateTime(year, month,  4));
      TheG.Columns["T_dec05"].HeaderText = TheG.Columns["T_dec05_2"].HeaderText = "05\n" + CovertToCro(new DateTime(year, month,  5));
      TheG.Columns["T_dec06"].HeaderText = TheG.Columns["T_dec06_2"].HeaderText = "06\n" + CovertToCro(new DateTime(year, month,  6));
      TheG.Columns["T_dec07"].HeaderText = TheG.Columns["T_dec07_2"].HeaderText = "07\n" + CovertToCro(new DateTime(year, month,  7));
      TheG.Columns["T_dec08"].HeaderText = TheG.Columns["T_dec08_2"].HeaderText = "08\n" + CovertToCro(new DateTime(year, month,  8));
      TheG.Columns["T_dec09"].HeaderText = TheG.Columns["T_dec09_2"].HeaderText = "09\n" + CovertToCro(new DateTime(year, month,  9));
      TheG.Columns["T_dec10"].HeaderText = TheG.Columns["T_dec10_2"].HeaderText = "10\n" + CovertToCro(new DateTime(year, month, 10));
      TheG.Columns["T_dec11"].HeaderText = TheG.Columns["T_dec11_2"].HeaderText = "11\n" + CovertToCro(new DateTime(year, month, 11));
      TheG.Columns["T_dec12"].HeaderText = TheG.Columns["T_dec12_2"].HeaderText = "12\n" + CovertToCro(new DateTime(year, month, 12));
      TheG.Columns["T_dec13"].HeaderText = TheG.Columns["T_dec13_2"].HeaderText = "13\n" + CovertToCro(new DateTime(year, month, 13));
      TheG.Columns["T_dec14"].HeaderText = TheG.Columns["T_dec14_2"].HeaderText = "14\n" + CovertToCro(new DateTime(year, month, 14));
      TheG.Columns["T_dec15"].HeaderText = TheG.Columns["T_dec15_2"].HeaderText = "15\n" + CovertToCro(new DateTime(year, month, 15));
      TheG.Columns["T_dec16"].HeaderText = TheG.Columns["T_dec16_2"].HeaderText = "16\n" + CovertToCro(new DateTime(year, month, 16));
      TheG.Columns["T_dec17"].HeaderText = TheG.Columns["T_dec17_2"].HeaderText = "17\n" + CovertToCro(new DateTime(year, month, 17));
      TheG.Columns["T_dec18"].HeaderText = TheG.Columns["T_dec18_2"].HeaderText = "18\n" + CovertToCro(new DateTime(year, month, 18));
      TheG.Columns["T_dec19"].HeaderText = TheG.Columns["T_dec19_2"].HeaderText = "19\n" + CovertToCro(new DateTime(year, month, 19));
      TheG.Columns["T_dec20"].HeaderText = TheG.Columns["T_dec20_2"].HeaderText = "20\n" + CovertToCro(new DateTime(year, month, 20));
      TheG.Columns["T_dec21"].HeaderText = TheG.Columns["T_dec21_2"].HeaderText = "21\n" + CovertToCro(new DateTime(year, month, 21));
      TheG.Columns["T_dec22"].HeaderText = TheG.Columns["T_dec22_2"].HeaderText = "22\n" + CovertToCro(new DateTime(year, month, 22));
      TheG.Columns["T_dec23"].HeaderText = TheG.Columns["T_dec23_2"].HeaderText = "23\n" + CovertToCro(new DateTime(year, month, 23));
      TheG.Columns["T_dec24"].HeaderText = TheG.Columns["T_dec24_2"].HeaderText = "24\n" + CovertToCro(new DateTime(year, month, 24));
      TheG.Columns["T_dec25"].HeaderText = TheG.Columns["T_dec25_2"].HeaderText = "25\n" + CovertToCro(new DateTime(year, month, 25));
      TheG.Columns["T_dec26"].HeaderText = TheG.Columns["T_dec26_2"].HeaderText = "26\n" + CovertToCro(new DateTime(year, month, 26));
      TheG.Columns["T_dec27"].HeaderText = TheG.Columns["T_dec27_2"].HeaderText = "27\n" + CovertToCro(new DateTime(year, month, 27));
      TheG.Columns["T_dec28"].HeaderText = TheG.Columns["T_dec28_2"].HeaderText = "28\n" + CovertToCro(new DateTime(year, month, 28));
      
      TheG.Columns["T_dec29"].Visible    = TheG.Columns["T_dec29_2"].Visible = days > 28;
      TheG.Columns["T_dec30"].Visible    = TheG.Columns["T_dec30_2"].Visible = days > 29;
      TheG.Columns["T_dec31"].Visible    = TheG.Columns["T_dec31_2"].Visible = days > 30;

      TheG.Columns["T_str29"].Visible    = true ? (TheG.Columns["T_str29"].Visible && TheG.Columns["T_dec29"].Visible) : false;
      TheG.Columns["T_str30"].Visible    = true ? (TheG.Columns["T_str30"].Visible && TheG.Columns["T_dec30"].Visible) : false;
      TheG.Columns["T_str31"].Visible    = true ? (TheG.Columns["T_str31"].Visible && TheG.Columns["T_dec31"].Visible) : false;

      if(TheG.Columns["T_dec29"].Visible) TheG.Columns["T_dec29"].HeaderText = TheG.Columns["T_dec29_2"].HeaderText = "29\n" + CovertToCro(new DateTime(year, month, 29));
      if(TheG.Columns["T_dec30"].Visible) TheG.Columns["T_dec30"].HeaderText = TheG.Columns["T_dec30_2"].HeaderText = "30\n" + CovertToCro(new DateTime(year, month, 30));
      if(TheG.Columns["T_dec31"].Visible) TheG.Columns["T_dec31"].HeaderText = TheG.Columns["T_dec31_2"].HeaderText = "31\n" + CovertToCro(new DateTime(year, month, 31));

      TheSumGrid.Columns["T_dec29"].Visible = days > 28;
      TheSumGrid.Columns["T_dec30"].Visible = days > 29;
      TheSumGrid.Columns["T_dec31"].Visible = days > 30;

      VvTextBox vvtb;
      foreach(DataGridViewColumn col in TheG.Columns)
      {
         vvtb = col.Tag as VvTextBox;

         col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

         //if(vvtb != null)
         if(col != null)
         {
                 if(col.HeaderText.Contains("NE"   )         ) col.DefaultCellStyle.BackColor = clr_MVR_NEdjelja;
            else if(col.HeaderText.Contains("SU"   )         ) col.DefaultCellStyle.BackColor = clr_MVR_SUbota;
            else if(col.Name.StartsWith    ("t_str")         ) col.DefaultCellStyle.BackColor = clr_MVR_VR_Back;
            else if(col.Name.StartsWith    ("scrol")         ) 
                 {
                    col.DefaultCellStyle.BackColor = Color.Indigo;
                    col.HeaderCell.Style.BackColor = Color.Indigo;
                 }
          //else if(vvtb != null && vvtb.JAM_ReadOnly == true) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
            else
            {
               if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_True_BackColor;
               else                                                           col.DefaultCellStyle.BackColor = ZXC.vvColors.dataGridCellReadOnly_False_BackColor;
            }

            // Qukatz: isPraznik 
                 if(vvtb != null && col.HeaderText.EndsWith("*")) col.DefaultCellStyle.BackColor = Color.FromArgb(217, 139, 217 /*253, 185, 253*/);
         }
      }
   }

   public string CovertToCro(/*string header*/DateTime date)
   {
      bool isPraznik = ZXC.IsThisDanPraznik(date, date.Year);

      string dayName = date.DayOfWeek.ToString();
      string praznikFlag = isPraznik ? "*" : "";

      switch(/*header*/dayName)
      {
         case "Monday"   : return "PON" + praznikFlag;
         case "Tuesday"  : return "UTO" + praznikFlag;
         case "Wednesday": return "SRI" + praznikFlag;
         case "Thursday" : return "ČET" + praznikFlag;
         case "Friday"   : return "PET" + praznikFlag;
         case "Saturday" : return "SUB" + praznikFlag;
         case "Sunday"   : return "NED" + praznikFlag;
         default         : return "";
      }
   }


   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint   Fld_KupDobCd      { get { return tbx_kupDobCd    .GetSomeRecIDField(); } set { tbx_kupDobCd    .PutSomeRecIDField(value); } }
   public string Fld_KupDobCdAsTxt { get { return tbx_kupDobCd    .Text;                } set { tbx_kupDobCd    .Text =            value ; } }
   public string Fld_KupDobName    { get { return tbx_kupDobName  .Text;                } set { tbx_kupDobName  .Text =            value ; } }
   public string Fld_KupDobTk      { get { return tbx_kupDobTk    .Text;                } set { tbx_kupDobTk    .Text =            value ; } }
   public uint   Fld_PosJedCd      { get { return tbx_PosJedCd    .GetSomeRecIDField(); } set { tbx_PosJedCd    .PutSomeRecIDField(value); } }
   public string Fld_PosJedTk      { get { return tbx_PosJedTk    .Text;                } set { tbx_PosJedTk    .Text =            value ; } }
   public string Fld_PosJedName    { get { return tbx_PosJedName  .Text;                } set { tbx_PosJedName  .Text =            value ; } }
   public string Fld_MMYYYY        { get { return tbx_lookUpMMYYYY.Text;                } set { tbx_lookUpMMYYYY.Text =            value ; } }
   public string Fld_PosJedAdresa  { get { return tbx_PosJedAdresa.Text;                } set { tbx_PosJedAdresa.Text =            value ; } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/
   public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd    )) Fld_KupDobCd     = mixer_rec.KupdobCD  ;
      if(CtrlOK(tbx_kupDobName  )) Fld_KupDobName   = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk    )) Fld_KupDobTk     = mixer_rec.KupdobTK  ;
      if(CtrlOK(tbx_PosJedCd    )) Fld_PosJedCd     = mixer_rec.V2_ttNum  ;
      if(CtrlOK(tbx_PosJedTk    )) Fld_PosJedTk     = mixer_rec.StrC_32   ;
      if(CtrlOK(tbx_PosJedName  )) Fld_PosJedName   = mixer_rec.StrF_64   ;
      if(CtrlOK(tbx_PosJedAdresa)) Fld_PosJedAdresa = mixer_rec.StrB_128  ;
      if(CtrlOK(tbx_lookUpMMYYYY)) Fld_MMYYYY       = mixer_rec.StrD_32   ;

      Fld_TtOpis = ZXC.luiListaMixTypeEvidencija.GetNameForThisCd(mixer_rec.TT);
      OnExit_MMYYYY_Set_Colors_Headers_DFS(tbx_lookUpMMYYYY, null);

   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd    )) mixer_rec.KupdobCD   = Fld_KupDobCd    ;
      if(CtrlOK(tbx_kupDobName  )) mixer_rec.KupdobName = Fld_KupDobName  ;
      if(CtrlOK(tbx_kupDobTk    )) mixer_rec.KupdobTK   = Fld_KupDobTk    ;
      if(CtrlOK(tbx_PosJedCd    )) mixer_rec.V2_ttNum   = Fld_PosJedCd    ;
      if(CtrlOK(tbx_PosJedTk    )) mixer_rec.StrC_32    = Fld_PosJedTk    ;
      if(CtrlOK(tbx_PosJedName  )) mixer_rec.StrF_64    = Fld_PosJedName  ;
      if(CtrlOK(tbx_PosJedAdresa)) mixer_rec.StrB_128   = Fld_PosJedAdresa;
      if(CtrlOK(tbx_lookUpMMYYYY)) mixer_rec.StrD_32    = Fld_MMYYYY      ;

   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public AvrFilterUC TheAvrFilterUC { get; set; }
   public AvrFilter   TheAvrFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheAvrFilter = new AvrFilter(this);

      TheAvrFilterUC        = new AvrFilterUC(this);
      TheAvrFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheAvrFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      AvrFilter mixerFilter = (AvrFilter)vvRptFilter;
      
       switch(mixerFilter.PrintAvr)
       {
         case AvrFilter.PrintAvrEnum.Avr: specificMixerReport = new RptX_AvrReport(new Vektor.Reports.XIZ.CR_AvrDUC(), "Avr", mixerFilter); break;

         //default: ZXC.aim_emsg("{0}\nPrintSomeAvrDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintAvr); return null;
         default: ZXC.aim_emsg("{0}\nPrintSomeAvrDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintAvr); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheAvrFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheAvrFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_Zahtj, Color.Empty, clr_Zahtj);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return "Raspored"; }
   }

   #endregion overrideTabPageTitle

}

public class AvrFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public AvrFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();
      TheVvUC = vvUC;

      CreateHamper_4ButtonsResetGo_Width(hamper4buttons.Width);

      hamperHorLine.Visible = false;

      this.Width = hamper4buttons.Width + ZXC.QUN;
      this.Height = hamper4buttons.Bottom + ZXC.QUN;

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);

      this.ResumeLayout();
   }

   #endregion  Constructor

   #region Put & GetFilterFields

   private AvrFilter TheAvrFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as AvrFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheAvrFilter = (AvrFilter)_filter_data;

      if(TheAvrFilter != null)
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

public class AvrFilter : VvRpt_Mix_Filter
{

   public enum PrintAvrEnum
   {
      Avr
   }

   public PrintAvrEnum PrintAvr { get; set; }

   public AvrDUC theDUC;

   public AvrFilter(AvrDUC _theDUC)
   {
      this.theDUC = _theDUC;
      SetDefaultFilterValues();
   }

   #region SetDefaultFilterValues()

   public override void SetDefaultFilterValues()
   {
      ZXC.VvDataBaseInfo vvDBinfo = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects;
      int projectYear = int.Parse(vvDBinfo.ProjectYear);
      int thisYear = DateTime.Now.Year;
      PrintAvr     = PrintAvrEnum.Avr;
   }

   #endregion SetDefaultFilterValues()

}
