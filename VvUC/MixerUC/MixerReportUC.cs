using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

public class MixerReportUC              : VvReportUC
{
   #region Fieldz


   private int razmakIzmjedjuHampera;
   private int nextX, nextY , razmakHamp = ZXC.Qun10;
   private int maxHampWidth;

   public MixerFilterUC TheMixerFilterUC { get; set; }


   #endregion Fieldz

   #region Constructor

   public MixerReportUC(Control parent, VvRpt_Mix_Filter _rptFilter, VvForm.VvSubModul vvSubModul)
   {
      this.TheRptFilter = _rptFilter;
      this.TheSubModul  = vvSubModul;

      this.SuspendLayout();

      TheMixerFilterUC        = new MixerFilterUC(this);
      TheMixerFilterUC.Parent = TheFilterPanel;
      maxHampWidth            = TheMixerFilterUC.MaxHamperWidth;
      razmakIzmjedjuHampera   = TheMixerFilterUC.razmakIzmjedjuHampera;

      nextX = razmakIzmjedjuHampera;
      nextY = TheMixerFilterUC.Height;

      InitializeVvUserControl(parent);
   
      //  sirina FilterPanela
      TheFilterPanel_Width = TheMixerFilterUC.Width;
      CalcTheFilterPanelWidth();

      this.ResumeLayout();

   }


   #endregion Constructor

   #region hamperDrillDown 

   private void InitializeHamper_DrillDown(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", TheFilterPanel, false, nextX, nextY, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q6un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;
      
      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

  //    cb_DrillDown       = hamper.CreateVvCheckBox_OLD(0, 0, null, "Omogući DrillDown", RightToLeft.No);
  //    cb_zapamtiPostavke = hamper.CreateVvCheckBox_OLD(0, 1, null, "Zapamti postavke" , RightToLeft.No);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, maxHampWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   #endregion hamperDrillDown
   
   #region PutFields(), GetFields()

   public override void GetFields(bool fuse)
   {
      GetFilterFields();
   }

   public override void GetFilterFields()
   {
      TheMixerFilterUC.GetFilterFields();

     // TheRptFilter.NeedsDrillDown   = Fld_NeedsDrillDown;
   }

   #region PutFields(_vvRptFilter)

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRptFilter = (VvRpt_Mix_Filter)_filter_data;

      if(TheRptFilter != null)
      {
         TheMixerFilterUC.PutFilterFields(TheRptFilter);
         
       //  Fld_NeedsDrillDown    = TheRptFilter.NeedsDrillDown;
      }
      
      // Za JAM_... : 
      this.ValidateChildren();
   }

   //ubuduce koristi static method: VvHamper.Set_ControlText_ThreadSafe(Control theControl, string theText) 

   #endregion PutFields(_vvRptFilter)

   #endregion PutFields(), GetFields()

   #region VvMixerReport

   public VvMixerReport TheVvMixReport
   {
      get { return TheVvReport as VvMixerReport; }
   }

   public VvRpt_Mix_Filter TheRptFilter
   {
      get;
      set;
   }

   #endregion VvMixerReport

   #region Override

   public override VvRptFilter VirtualRptFilter
   {
      get { return this.TheRptFilter; }
   }

   public override void AddFilterMemberz()
   {
      TheMixerFilterUC.AddFilterMemberz(TheRptFilter, TheVvMixReport);
   }

   public override void ResetRptFilterRbCbControls()
   {
    
   }

   #endregion Override

}

public class MixerFilterUC              : VvFilterUC
{
   #region Fieldz

   private VvHamper hamper_OdDo, hamper_TT, hamper_Partner, hamper_MjTroska, hamper_Person, hamper_PN, hamper_Napomena, hamper_grupZahtjev,
                    hamp_grupPn, hamper_zahtjev, hamper_grupEVD, hamper_grupSMD, hamper_grupIRV, hamper_MVR, hamper_RUG, hamper_StrH_32, hamper_grupUG, 
                    hamper_sort, hamper_isAktivni;

   private VvTextBox tbx_KD_naziv, tbx_KD_sifra, tbx_KD_ticker,
                     tbxLookUp_TT, tbx_TTOpis,
                     tbx_MT_naziv, tbx_MT_sifra, tbx_MT_ticker,
                     tbx_TTnumOD,  tbx_TTnumDO,
                     tbx_PersonCD, tbx_PersonPrezime,
                     tbx_strC_32, tbx_strH_32, tbx_strA_40, tbx_strG_40, tbx_Napomena, tbx_ProjektCD, tbx_zaMMYYYY;

   public CheckBox cbx_biloGdjeUnapomeni, cbx_biloGdjeUstrC_32, cbx_biloGdjeUStrH_32, cbx_prekoPFS, cbx_grupaPoStr, cbx_onlyUkPers;
   private RadioButton  rbt_grupNull, rbt_grupPartner, rbt_grupMjTros, rbt_grupProjekt, rbt_grupStatus,
                        rbt_grupPnNull, rbt_grupPnPerson, rbt_grupPnVozilo , rbt_grupPnMjTros, rbt_grupPnProjekt, rbt_grupPnValuta,
                        rbt_grupEvNull, rbt_grupEvPartner, rbt_grupEvProjekt, rbt_grupEvOsoba, rbt_grupEvOrmar, rbt_grupEvStatus, rbt_grupEvMjTros,
                        rbt_grupSMDNull, rbt_grupSMDPartner, rbt_grupSMDProjekt, rbt_grupSMDPersonCD, rbt_grupSMDMjTros,
                        rbt_irvNull, rbt_irvProjekt, rbt_irvMjesto, rbt_irvZadatak, 
                        rbt_mvrPoDanima, rbt_mvrSviRadnici,
                        rbt_grUgNull, rbt_grUgPartner, rbt_grUgVrstUg,rbSort_Datum, rbSort_Broj,
                        rbt_sviRUG, rbt_isAktivan, rbt_isNeaktivan;

   #endregion Fieldz

   #region Constructor

   public MixerFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC     = vvUC;

      CreateHampers();

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);// jer se ponasa ko reportFilter

      this.ResumeLayout();
   }

   #endregion Constructor

   #region hampers
   
   private void CreateHampers()
   {
      razmakIzmjedjuHampera = ZXC.Qun2 - ZXC.Qun4;

      InitializeHamper_OdDo(out hamper_OdDo);
    
      MaxHamperWidth = hamper_OdDo.Width;
      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);
     
      nextY = hamper_OdDo.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_TT(out hamper_TT);

      nextY = hamper_OdDo.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_MVR(out hamper_MVR);

      nextY = hamper_MVR.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Napomena(out hamper_Napomena);
      
      nextY = hamper_Napomena.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_MjTroska(out hamper_MjTroska);
      
      nextY = hamper_MjTroska.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_Partner(out hamper_Partner);
      nextY = hamper_Partner.Bottom + razmakIzmjedjuHampera;

      nextY = hamper_MjTroska.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Djelatnik(out hamper_Person);

      nextY = hamper_Person.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_PutNalog(out hamper_PN);

      nextY = hamper_PN.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_GrupirajDokPutNal(out hamp_grupPn);

      nextY = hamper_Partner.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Zahtjev(out hamper_zahtjev);
      nextY = hamper_zahtjev.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_GrupirajZahtjeve(out hamper_grupZahtjev);
      nextY = hamper_grupZahtjev.Bottom + razmakIzmjedjuHampera;

      nextY = hamper_Partner.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_GrupirajEvidencije(out hamper_grupEVD);
     
      InitializeHamper_Sort(out hamper_sort);
      
      InitializeHamper_GrupirajUgovore(out hamper_grupUG);

    //nextY = hamper_grupUG.Bottom + razmakIzmjedjuHampera;
    //InitializeHamper_Sort           (out hamper_sort);

      InitializeHamper_IsAktivni(out hamper_isAktivni);

    //nextY = hamper_grupEVD.Bottom + razmakIzmjedjuHampera;

      nextY = hamper_Partner.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_GrupirajSMD(out hamper_grupSMD);

      nextY = hamper_Person.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_GrupirajIRV(out hamper_grupIRV);

      InitializeHamper_StrH_32(out hamper_StrH_32); // PN-vozilo, RUG-VrstaUG


      nextY =  SetVisibilitiOfHampers(this.TheVvUC.TheSubModul.subModulEnum) + ZXC.Qun4;

      hamperHorLine.Location = new Point(nextX, nextY);
      hamperHorLine.Parent   = this;

      nextY = LocationOfHamper_HorLine      (nextX, nextY, MaxHamperWidth) + ZXC.QUN;
   }

   private int SetVisibilitiOfHampers(ZXC.VvSubModulEnum vvSubModulEnum)
   {
      if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_P)
      {
         hamper_Person  .Visible =
         hamper_PN      .Visible =
         hamp_grupPn    .Visible = true ;
         hamper_MVR     .Visible = false;
         hamper_StrH_32 .Visible = true ;
         
         hamp_grupPn.Location = new Point(nextX, hamper_StrH_32.Bottom);

         return hamp_grupPn.Bottom;
      }
      else if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_Z)
      {
         hamper_Partner .Visible = hamper_grupZahtjev.Visible = true;
         hamper_zahtjev .Visible =  true;
         hamper_MVR     .Visible = false;
         hamper_StrH_32 .Visible = false;

         return hamper_grupZahtjev.Bottom;
      }
      else if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_S)
      {
         hamper_Partner .Visible = true;
         hamper_grupSMD .Visible = true;
         hamper_TT      .Visible = false;
         hamper_MVR     .Visible = false;
         hamper_StrH_32 .Visible = false;

         return hamper_grupSMD.Bottom;
      }
      else if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_E)
      {
         hamper_Partner  .Visible = true;
         hamper_TT       .Visible = true;
         hamper_grupEVD  .Visible = true;
         hamper_MVR      .Visible = false;
         hamper_StrH_32  .Visible = true;
         hamper_grupUG   .Visible = true;
         hamper_sort     .Visible = true;
         hamper_isAktivni.Visible = true;

         return hamper_isAktivni.Bottom;
      }
      else if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_R)
      {
         hamper_Person  .Visible = true;
         hamper_TT      .Visible = false;
         hamper_grupIRV .Visible = true;
         hamper_MVR     .Visible = true;
         hamper_StrH_32 .Visible = false;

         return hamper_grupIRV.Bottom;
      }
      else if(vvSubModulEnum == ZXC.VvSubModulEnum.XIZ_B)
      {
         hamper_Person   .Visible = true;
         hamper_Partner  .Visible = true;
     hamper_TT       .Visible = true;
         hamper_StrH_32  .Visible = true;
         hamper_grupUG   .Visible = true;
         hamper_sort     .Visible = true;
         hamper_isAktivni.Visible = true;

         hamper_MjTroska   .Visible = false;
         hamper_PN         .Visible = false; 
         hamper_grupZahtjev.Visible = false;
         hamp_grupPn       .Visible = false; 
         hamper_zahtjev    .Visible = false; 
         hamper_grupEVD    .Visible = false; 
         hamper_grupSMD    .Visible = false; 
         hamper_grupIRV    .Visible = false; 
         hamper_MVR        .Visible = false;

         
         hamper_TT       .Visible = true;
         hamper_Person   .Location = new Point(hamper_OdDo.Left               , hamper_TT       .Bottom + ZXC.Qun4);
         hamper_Partner  .Location = new Point(hamper_OdDo.Left               , hamper_Person   .Bottom + ZXC.Qun4);
         hamper_StrH_32  .Location = new Point(hamper_OdDo.Left               , hamper_Partner  .Bottom + ZXC.Qun4);
         hamper_grupUG   .Location = new Point(hamper_OdDo.Left               , hamper_StrH_32  .Bottom + ZXC.Qun4);
         hamper_isAktivni.Location = new Point(hamper_grupUG.Right + ZXC.Qun8 , hamper_StrH_32  .Bottom + ZXC.Qun4);
         hamper_sort     .Location = new Point(hamper_OdDo.Left               , hamper_isAktivni.Bottom + ZXC.Qun4);
         hamper_Napomena .Location = new Point(hamper_OdDo.Left               , hamper_sort     .Bottom + ZXC.Qun4);
        
         return hamper_Napomena.Bottom;
      }

      else 
      {
         return hamper_MjTroska.Bottom;
      }
   }

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", this, false, nextX, nextY, razmakHampGroup);
       
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un,ZXC.Qun8,ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8,ZXC.Qun8,ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel (1, 0, "OD:", ContentAlignment.MiddleCenter);
      hamper.CreateVvLabel (3, 0, "DO:", ContentAlignment.MiddleCenter);

                     hamper.CreateVvLabel  (0, 1, "Datum:", ContentAlignment.MiddleRight);
      tbx_DatumOD  = hamper.CreateVvTextBox(1, 1, "tbx_datumOd", "Od datuma");
      tbx_DatumOD.JAM_IsForDateTimePicker = true;
      dtp_DatumOD      = hamper.CreateVvDateTimePicker(1, 1, "", tbx_DatumOD);
      dtp_DatumOD.Name = "dtp_DatumOD";
      dtp_DatumOD.Tag  = tbx_DatumOD;
      tbx_DatumOD.Tag  = dtp_DatumOD;

      tbx_DatumDO      = hamper.CreateVvTextBox       (3, 1, "tbx_datumDo", "");
      tbx_DatumDO.JAM_IsForDateTimePicker = true;
      dtp_DatumDO      = hamper.CreateVvDateTimePicker(3, 1, "", tbx_DatumDO);
      dtp_DatumDO.Name = "dtp_DatumDO";
      dtp_DatumDO.Tag  = tbx_DatumDO;
      tbx_DatumDO.Tag  = dtp_DatumDO;

                    hamper.CreateVvLabel  (0, 2, "TT broj:"   , ContentAlignment.MiddleRight);
      tbx_TTnumOD = hamper.CreateVvTextBox(1, 2, "tbx_TTnumOD", "Od dokumenta broj",6);
      tbx_TTnumOD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TTnumOD.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);

      tbx_TTnumDO = hamper.CreateVvTextBox(3, 2, "tbx_TTnumDO", "Do dokumenta broj",6);
      tbx_TTnumDO.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_TTnumDO.JAM_MarkAsNumericTextBox(0, false, decimal.MaxValue, decimal.MinValue, true);


      tbx_DatumOD.ContextMenu = dtp_DatumOD.ContextMenu = 
      tbx_DatumDO.ContextMenu = dtp_DatumDO.ContextMenu =  CreateNewContexMenu_Date();

      SetUpAsWriteOnlyTbx(hamper);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_MVR(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 3, "", this, false, nextX, nextY, razmakHampGroup);
       
      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q4un,ZXC.Qun8,ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8,ZXC.Qun8,ZXC.Qun8};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      hamper.CreateVvLabel(0, 0, "MVR Za mjesec:", 1, 0, ContentAlignment.MiddleRight);

      tbx_zaMMYYYY = hamper.CreateVvTextBoxLookUp("tbx_zaMj",2, 0,  "Obračun plaće ZA zadani mjesec u godini", 6, 1, 0);
      tbx_zaMMYYYY.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_zaMMYYYY.JAM_Set_LookUpTable(ZXC.luiListaFondSati_NOR, (int)ZXC.Kolona.prva);

      bool isAG = ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"); // zapravo za old MVR koji je koristio only Ag sjaj koji dalje na placa ali to su platili ...

                          hamper.CreateVvLabel      (0, 1, isAG ? "MVR:" : "RVR:", ContentAlignment.MiddleRight);
      rbt_mvrPoDanima   = hamper.CreateVvRadioButton(1, 1, null, "Radnik/Dan", TextImageRelation.ImageBeforeText);
      rbt_mvrSviRadnici = hamper.CreateVvRadioButton(3, 1, null, "Radnici/Mj", TextImageRelation.ImageBeforeText);

      rbt_mvrSviRadnici.Checked =  isAG;
      rbt_mvrSviRadnici.Tag     =  isAG;
      rbt_mvrPoDanima.Checked   = !isAG;
      rbt_mvrPoDanima.Tag       = !isAG;

      cbx_prekoPFS = hamper.CreateVvCheckBox_OLD(0, 2, null, 3, 0, isAG ? "Prikaži prekoPrekovremene" : "Prikaži početak i završetak rada", RightToLeft.Yes);

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_Partner  (out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q2un,   ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,   ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun8};

      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "Partner:"     , ContentAlignment.MiddleRight);
      tbx_KD_sifra  = hamper.CreateVvTextBox(1, 0, "tbx_KD_sifra" , "Šifra partnera", 6);
      tbx_KD_ticker = hamper.CreateVvTextBox(3, 0, "tbx_KD_ticker", "Ticker partnera", 6);
      tbx_KD_naziv  = hamper.CreateVvTextBox(1, 1, "tbx_KupDob"   , "Naziv partnera", 32, 2, 0);

      tbx_KD_sifra.JAM_MustTabOutBeforeSubmit  = true;
      tbx_KD_ticker.JAM_MustTabOutBeforeSubmit = true;
      tbx_KD_naziv.JAM_MustTabOutBeforeSubmit  = true;

      tbx_KD_sifra.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_KD_naziv.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      tbx_KD_ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_KD_sifra.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_KD_sifra.JAM_MarkAsNumericTextBox(0, false);
      tbx_KD_sifra.TextAlign = HorizontalAlignment.Left;
      tbx_KD_sifra.JAM_FillCharacter = '0';

      tbx_KD_sifra.JAM_WriteOnly = tbx_KD_ticker.JAM_WriteOnly = tbx_KD_naziv.JAM_WriteOnly = true;

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_MjTroska (out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q2un,   ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,   ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun8 };

      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel  (0, 0, "MjTroška:"    , ContentAlignment.MiddleRight);
      tbx_MT_sifra  = hamper.CreateVvTextBox(1, 0, "tbx_MT_sifra" , "Šifra mjesta troška" , 6);
      tbx_MT_ticker = hamper.CreateVvTextBox(3, 0, "tbx_MT_ticker", "Ticker mjesta troška", 6);
      tbx_MT_naziv  = hamper.CreateVvTextBox(1, 1, "tbx_MTnaziv"  , "Naziv mjesta troška" , 30, 2, 0);

      tbx_MT_sifra.JAM_MarkAsNumericTextBox(0, false);
      tbx_MT_sifra.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_MT_sifra.JAM_FillCharacter = '0';
      tbx_MT_sifra.TextAlign         = HorizontalAlignment.Left;
      tbx_MT_ticker.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_MT_sifra.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterKCD.SortType   , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyMtrosTextBoxLeave));
      tbx_MT_ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyMtrosTextBoxLeave));
      tbx_MT_naziv.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Mtros_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyMtrosTextBoxLeave));

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }
   
   private void InitializeHamper_Djelatnik(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un, ZXC.Q2un,   ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8,   ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4};

      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel  (0, 0, "Djelatnik:"       , System.Drawing.ContentAlignment.MiddleRight);
      tbx_PersonCD      = hamper.CreateVvTextBox(1, 0, "tbx_PersonCD"     , "Šifra djelatnika");
      tbx_PersonPrezime = hamper.CreateVvTextBox(2, 0, "tbx_PersonPrezime", "Naziv djelatnika", 40, 1, 0);
      tbx_PersonCD.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

      tbx_PersonCD     .JAM_SetAutoCompleteData(Person.recordName, Person.sorterCD.SortType     , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterSifra)  , new EventHandler(AnyPersonTextBoxLeave));
      tbx_PersonPrezime.JAM_SetAutoCompleteData(Person.recordName, Person.sorterPrezime.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Person_sorterPrezime), new EventHandler(AnyPersonTextBoxLeave));

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_TT(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] {ZXC.Q3un, ZXC.Q2un + ZXC.Qun2, ZXC.Q6un - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel        (0, 0, "TipTrans:"   , ContentAlignment.MiddleRight);
      tbxLookUp_TT = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_TT", "Tip transakcije");
      tbx_TTOpis   = hamper.CreateVvTextBox      (2, 0, "tbx_TTOpis_InVisible", "Opis tipa transkacije");
      tbxLookUp_TT.JAM_CharacterCasing = CharacterCasing.Upper;

      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_P) tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypePutNal    , (int)ZXC.Kolona.prva);
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_Z) tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeZahtjev   , (int)ZXC.Kolona.prva);
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_E) tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeEvidencija, (int)ZXC.Kolona.prva);
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_B) tbxLookUp_TT.JAM_Set_LookUpTable(ZXC.luiListaMixTypeZastitari , (int)ZXC.Kolona.prva);

      tbxLookUp_TT.JAM_lui_NameTaker_JAM_Name = tbx_TTOpis.JAM_Name;
      tbx_TTOpis.JAM_ReadOnly = true;
      tbx_TTOpis.Tag          = ZXC.vvColors.userControl_BackColor;
      
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Napomena(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] {ZXC.Q3un, ZXC.Q7un + ZXC.Qun2, ZXC.QUN - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                     hamper.CreateVvLabel  (0, 0, "Napomena:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_Napomena = hamper.CreateVvTextBox(1, 0, "tbx_napomena",  "Napomena");
      cbx_biloGdjeUnapomeni = hamper.CreateVvCheckBox_OLD(2, 0, null, "", RightToLeft.Yes, true);
                     
                      hamper.CreateVvLabel  (0, 1, "Projekt:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_ProjektCD = hamper.CreateVvTextBox(1, 1, "tbx_ProjektCD", "ProjektCD");
      tbx_ProjektCD.JAM_SetAutoCompleteData(Faktur.recordName, Faktur.sorterTtNum.SortType, ZXC.AutoCompleteRestrictor.FAK_ExactTT_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Faktur_DUMMY), null);

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_PutNalog(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] {ZXC.Q3un, ZXC.Q7un + ZXC.Qun2, ZXC.QUN - ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                      hamper.CreateVvLabel        (0, 0, "Odredište:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_strC_32 = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_odrediste",  "Odredište");
      tbx_strC_32.JAM_Set_LookUpTable(ZXC.luiListaMixerOdrediste, (int)ZXC.Kolona.prva);
      tbx_strC_32.JAM_lookUp_NOTobligatory = true;
      cbx_biloGdjeUstrC_32 = hamper.CreateVvCheckBox_OLD(2, 0, null, "", RightToLeft.Yes, true);

   //if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_P)
   //{
   //   hamper.CreateVvLabel(0, 1, "Vozilo:", System.Drawing.ContentAlignment.MiddleRight);
   //   tbx_strH_32 = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_vozilo", "Vozilo");
   //   tbx_strH_32.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
   //   tbx_strH_32.JAM_lookUp_NOTobligatory = true;
   //   cbx_biloGdjeUStrH_32 = hamper.CreateVvCheckBox_OLD(2, 1, null, "", RightToLeft.Yes, true);
   //}
   //else
   //{
   //   tbx_strH_32 = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_strH_32", "Vozilo");
   //}

      hamper.Visible = false;
      
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_GrupirajDokPutNal(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel      (0, 0, "Grupiraj Dokumente:", ContentAlignment.MiddleLeft);
      rbt_grupPnNull    = hamper.CreateVvRadioButton(0, 1, null, "NE", TextImageRelation.ImageBeforeText);
      rbt_grupPnPerson  = hamper.CreateVvRadioButton(0, 2, null, "Po Djelatniku", TextImageRelation.ImageBeforeText);
      rbt_grupPnVozilo  = hamper.CreateVvRadioButton(0, 3, null, "Po Vozilu"    , TextImageRelation.ImageBeforeText);
      rbt_grupPnMjTros  = hamper.CreateVvRadioButton(0, 4, null, "Po MjTroška"  , TextImageRelation.ImageBeforeText);
      rbt_grupPnProjekt = hamper.CreateVvRadioButton(0, 5, null, "Po Projektu"  , TextImageRelation.ImageBeforeText);
      rbt_grupPnValuta  = hamper.CreateVvRadioButton(0, 6, null, "Po IzvValuti" , TextImageRelation.ImageBeforeText);

      rbt_grupPnNull.Checked = true;
      rbt_grupPnNull.Tag     = true;

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }
    
   private void InitializeHamper_GrupirajZahtjeve(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel      (0, 0, "Grupiraj Zahtjeve:", ContentAlignment.MiddleLeft);
      rbt_grupNull    = hamper.CreateVvRadioButton(0, 1, null, "NE"         , TextImageRelation.ImageBeforeText);
      rbt_grupPartner = hamper.CreateVvRadioButton(0, 2, null, "Po Partneru", TextImageRelation.ImageBeforeText);
      rbt_grupProjekt = hamper.CreateVvRadioButton(0, 5, null, "Po Projektu", TextImageRelation.ImageBeforeText);
      rbt_grupStatus  = hamper.CreateVvRadioButton(0, 3, null, "Po Statusu" , TextImageRelation.ImageBeforeText);
      rbt_grupMjTros  = hamper.CreateVvRadioButton(0, 4, null, "Po MjTroška", TextImageRelation.ImageBeforeText);

      rbt_grupNull.Checked = true;
      rbt_grupNull.Tag     = true;

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }
  
   private void InitializeHamper_GrupirajEvidencije(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4, ZXC.Q5un  + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4           , ZXC.Qun4            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                          hamper.CreateVvLabel      (0, 0, "Grupiraj Evidencije:", ContentAlignment.MiddleLeft);
      rbt_grupEvNull    = hamper.CreateVvRadioButton(1, 0, null, "NE"            , TextImageRelation.ImageBeforeText);
      rbt_grupEvPartner = hamper.CreateVvRadioButton(0, 1, null, "Po Partneru"   , TextImageRelation.ImageBeforeText);
      rbt_grupEvProjekt = hamper.CreateVvRadioButton(0, 2, null, "Po Projektu"   , TextImageRelation.ImageBeforeText);
      rbt_grupEvOsoba   = hamper.CreateVvRadioButton(0, 3, null, "Po Osobi"      , TextImageRelation.ImageBeforeText);
      rbt_grupEvOrmar   = hamper.CreateVvRadioButton(1, 1, null, "Po Ormaru"     , TextImageRelation.ImageBeforeText);
      rbt_grupEvStatus  = hamper.CreateVvRadioButton(1, 2, null, "Po Statusu"    , TextImageRelation.ImageBeforeText);
      rbt_grupEvMjTros  = hamper.CreateVvRadioButton(1, 3, null, "Po MjTroška"   , TextImageRelation.ImageBeforeText);

      rbt_grupEvNull.Checked = true;
      rbt_grupEvNull.Tag     = true;

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_GrupirajSMD(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 7, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
       
                            hamper.CreateVvLabel      (0, 0, "Grupiraj SMD:"       , ContentAlignment.MiddleLeft);
      rbt_grupSMDNull     = hamper.CreateVvRadioButton(0, 1, null, "NE"            , TextImageRelation.ImageBeforeText);
      rbt_grupSMDPartner  = hamper.CreateVvRadioButton(0, 2, null, "Po Partneru"   , TextImageRelation.ImageBeforeText);
      rbt_grupSMDProjekt  = hamper.CreateVvRadioButton(0, 3, null, "Po Projektu"   , TextImageRelation.ImageBeforeText);
      rbt_grupSMDMjTros   = hamper.CreateVvRadioButton(0, 4, null, "Po MjTroška"   , TextImageRelation.ImageBeforeText);

      rbt_grupSMDPersonCD = hamper.CreateVvRadioButton(0, 6, null, "Po Djelatniku", TextImageRelation.ImageBeforeText);

      rbt_grupSMDNull.Checked = true;
      rbt_grupSMDNull.Tag     = true;

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Zahtjev(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] {ZXC.Q4un, ZXC.Q7un };
      hamper.VvSpcBefCol   = new int[] {ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for (int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                    hamper.CreateVvLabel        (0, 0, "OdgovIzvršitelj:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_strA_40 = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_strA_40",  "Odgovorni izvršitelj");
      tbx_strA_40.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

                    hamper.CreateVvLabel        (0, 1, "VoditProjekta:", System.Drawing.ContentAlignment.MiddleRight);
      tbx_strG_40 = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_strG_40",  "Voditelj projekta");
      tbx_strG_40.JAM_Set_LookUpTable(ZXC.luiListaRiskVodPrjkt, (int)ZXC.Kolona.prva);

      hamper.Visible = false;
      
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_GrupirajIRV(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 5, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un + ZXC.Qun4, ZXC.Q5un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 + ZXC.Qun5, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
       
                        hamper.CreateVvLabel      (0, 0, "Grupiraj IRV:"    , ContentAlignment.MiddleLeft);
      rbt_irvNull     = hamper.CreateVvRadioButton(0, 1, null, "NE"         , TextImageRelation.ImageBeforeText);
 //     rbt_irvPartner  = hamper.CreateVvRadioButton(0, 2, null, "Po Partneru", TextImageRelation.ImageBeforeText);
      rbt_irvProjekt  = hamper.CreateVvRadioButton(0, 2, null, "Po Projektu", TextImageRelation.ImageBeforeText);
      rbt_irvMjesto   = hamper.CreateVvRadioButton(0, 3, null, "Po Mjestu"  , TextImageRelation.ImageBeforeText);
      rbt_irvZadatak  = hamper.CreateVvRadioButton(0, 4, null, "Po Zadatku" , TextImageRelation.ImageBeforeText);

      rbt_irvNull.Checked = true;
      rbt_irvNull.Tag     = true;

      cbx_grupaPoStr = hamper.CreateVvCheckBox_OLD(1, 0, null, "Grupa po strani", RightToLeft.Yes);
      cbx_onlyUkPers = hamper.CreateVvCheckBox_OLD(1, 1, null, "Samo ukupno", RightToLeft.Yes);

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_StrH_32(out VvHamper hamper) // PN-vozilo, RUG-VrstaUG
   {
           if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_E) nextY = hamper_TT.Bottom + ZXC.Qun2;
      else if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_P) nextY = hamper_PN.Bottom;
      else                                                                  nextY = nextY;

      hamper = new VvHamper(3, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q6un + ZXC.Qun2, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_P)
      {
                       hamper.CreateVvLabel        (0, 0, "Vozilo:", System.Drawing.ContentAlignment.MiddleRight);
         tbx_strH_32 = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_strH_32", "Vozilo");
         tbx_strH_32.JAM_Set_LookUpTable(ZXC.luiListaMixerVozilo, (int)ZXC.Kolona.prva);
         tbx_strH_32.JAM_lookUp_NOTobligatory = true;
         cbx_biloGdjeUStrH_32 = hamper.CreateVvCheckBox_OLD(2, 0, null, "", RightToLeft.Yes, true);
      }
      else if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_E || TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_B)
      {
                       hamper.CreateVvLabel        (0, 0, "Vrsta ugovora:", System.Drawing.ContentAlignment.MiddleRight);
         tbx_strH_32 = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_strH_32", "Vrsta ugovora");
         tbx_strH_32.JAM_Set_LookUpTable(ZXC.luiListaVrstaUgovora, (int)ZXC.Kolona.prva);
      }
      else
      {
                       hamper.CreateVvLabel        (0, 0, "", System.Drawing.ContentAlignment.MiddleRight);
         tbx_strH_32 = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_strH_32", "");
      }

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_GrupirajUgovore(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, hamper_grupEVD.Left, hamper_sort.Bottom + ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel      (0, 0, "Grupiraj Ugovore:", ContentAlignment.MiddleLeft);
      rbt_grUgNull    = hamper.CreateVvRadioButton(0, 1, null, "NE"            , TextImageRelation.ImageBeforeText);
      rbt_grUgPartner = hamper.CreateVvRadioButton(0, 2, null, "Po Partneru"   , TextImageRelation.ImageBeforeText);
      rbt_grUgVrstUg  = hamper.CreateVvRadioButton(0, 3, null, "Po Vrsti Ug"   , TextImageRelation.ImageBeforeText);

      rbt_grUgNull.Checked = true;
      rbt_grUgNull.Tag = true;

      hamper.Visible = false;

    //VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void InitializeHamper_Sort(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 1, "", this, false, hamper_grupEVD.Left, hamper_grupEVD.Bottom + ZXC.Qun4, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4, ZXC.Q4un- ZXC.Qun4, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun12, ZXC.Qun12 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      hamper.CreateVvLabel(0, 0, "Slijed ispisa:", ContentAlignment.MiddleLeft);

      rbSort_Datum = hamper.CreateVvRadioButton(1, 0, null, "Po datumu", TextImageRelation.ImageBeforeText);
      rbSort_Datum.Checked = true;
      rbSort_Broj = hamper.CreateVvRadioButton(2, 0, null, "Po TT broju", TextImageRelation.ImageBeforeText);

      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void InitializeHamper_IsAktivni(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, hamper_grupUG.Right + ZXC.Qun4, hamper_grupUG.Top, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN-ZXC.Qun8;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      
      hamper.CreateVvLabel(0, 0, "Ispis ugovora:", ContentAlignment.MiddleLeft);

      rbt_sviRUG      = hamper.CreateVvRadioButton(0, 1, null, "Sve"           , TextImageRelation.ImageBeforeText);
      rbt_sviRUG.Checked = true;                                                 
      rbt_isAktivan   = hamper.CreateVvRadioButton(0, 2, null, "Samo Aktivni"  , TextImageRelation.ImageBeforeText);
      rbt_isNeaktivan = hamper.CreateVvRadioButton(0, 3, null, "Samo Neaktivni", TextImageRelation.ImageBeforeText);
      rbt_sviRUG.Tag = true;
      hamper.Visible = false;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

   }

   private void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KD_naziv = kupdob_rec.Naziv;
            Fld_KD_sifra = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KD_ticker = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KD_naziv = Fld_KD_ticker = Fld_KD_SifraAsTxt = "";
         }
      }
   }
   
   private void AnyMtrosTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;

         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_MT_sifra  = kupdob_rec.KupdobCD/*RecID*/;
            Fld_MT_ticker = kupdob_rec.Ticker;
            Fld_MT_naziv  = kupdob_rec.Naziv;
         }
         else
         {
            Fld_MT_SifraAsTxt = Fld_MT_ticker = Fld_MT_naziv = "";
         }
      }
   }

   private void AnyPersonTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      //if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Person person_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;

         person_rec = VvUserControl.PersonSifrar.Find(TheVvUC.FoundInSifrar<Person>);

         if(person_rec != null && tb.Text != "")
         {
            Fld_PersonCd      = person_rec.PersonCD/*RecID*/;
            Fld_PersonPrezime = person_rec.PrezimeIme;
         }
         else
         {
            Fld_PersonCdAsTx = Fld_PersonPrezime = "";
         }
      }
   }

   #endregion hampers

   #region Fld_

   public DateTime Fld_DatumOd
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumOD.Value);
      }
      set
      {
         dtp_DatumOD.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumOD.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumOD);
      }
   }

   public DateTime Fld_DatumDo
   {
      get
      {
         return ZXC.ValOr_01010001_DtpDateTime(dtp_DatumDO.Value);
      }
      set
      {
         dtp_DatumDO.Value = ZXC.ValOr_01011753_DateTime(value);
         tbx_DatumDO.Text  = ZXC.ValOrEmpty_DtpDateTime_AsText(dtp_DatumDO);
      }
   }

   public string Fld_TT            { get { return tbxLookUp_TT.Text;                     } set { tbxLookUp_TT .Text = value;                    } }
   public string Fld_TtOpis        {                                                       set { tbx_TTOpis   .Text = value;                    } }
   public uint   Fld_TtNumOd       { get { return ZXC.ValOrZero_UInt(tbx_TTnumOD.Text);  } set { tbx_TTnumOD  .Text = value.ToString();         } }
   public uint   Fld_TtNumDo       { get { return ZXC.ValOrZero_UInt(tbx_TTnumDO.Text);  } set { tbx_TTnumDO  .Text = value.ToString();         } }
   public string Fld_KD_ticker     { get { return tbx_KD_ticker.Text;                    } set { tbx_KD_ticker.Text = value;                    } }
   public uint   Fld_KD_sifra      { get { return ZXC.ValOrZero_UInt(tbx_KD_sifra.Text); } set { tbx_KD_sifra .Text = value.ToString("000000"); } }
   public string Fld_KD_SifraAsTxt { get { return tbx_KD_sifra.Text;                     } set { tbx_KD_sifra .Text = value;                    } }
   public string Fld_KD_naziv      { get { return tbx_KD_naziv.Text;                     } set { tbx_KD_naziv .Text = value;                    } }
   public string Fld_MT_ticker     { get { return tbx_MT_ticker.Text;                    } set { tbx_MT_ticker.Text = value;                    } }
   public uint   Fld_MT_sifra      { get { return ZXC.ValOrZero_UInt(tbx_MT_sifra.Text); } set { tbx_MT_sifra .Text = value.ToString("000000"); } }
   public string Fld_MT_SifraAsTxt { get { return tbx_MT_sifra.Text;                     } set { tbx_MT_sifra .Text = value;                    } }
   public string Fld_MT_naziv      { get { return tbx_MT_naziv.Text;                     } set { tbx_MT_naziv .Text = value;                    } }
   public uint   Fld_PersonCd      { get { return tbx_PersonCD.GetSomeRecIDField();      } set { tbx_PersonCD.PutSomeRecIDField(value);         } }
   public string Fld_PersonCdAsTx  { get { return tbx_PersonCD.Text;                     } set { tbx_PersonCD     .Text       = value;          } }
   public string Fld_PersonPrezime { get { return tbx_PersonPrezime.Text;                } set { tbx_PersonPrezime.Text       = value;          } }

   public string Fld_GrupiranjePutNal
   { 
      get
      {
              if(rbt_grupPnNull   .Checked) return "";
         else if(rbt_grupPnPerson .Checked) return "PersonCD" ;
         else if(rbt_grupPnVozilo .Checked) return "StrH_32" ;
         else if(rbt_grupPnMjTros .Checked) return "MtrosTK" ;
         else if(rbt_grupPnValuta .Checked) return "DevName" ;
         else if(rbt_grupPnProjekt.Checked) return "ProjektCD" ;
         else                             return "";
      }
   }
   
   public string Fld_GrupiranjeZahtjeva
   { 
      get
      {
              if(rbt_grupNull   .Checked) return "";
         else if(rbt_grupPartner.Checked) return "KupdobName" ;
         else if(rbt_grupStatus .Checked) return "StrH_32" ;
         else if(rbt_grupMjTros .Checked) return "MtrosTK" ;
         else if(rbt_grupProjekt.Checked) return "ProjektCD" ;
         else                             return "";
      }
   }
   
   public string Fld_GrupiranjeEvidencija
   { 
      get
      {
              if(rbt_grupEvNull   .Checked) return "";
         else if(rbt_grupEvPartner.Checked) return "KupdobName" ;
         else if(rbt_grupEvStatus .Checked) return "StrH_32" ;
         else if(rbt_grupEvMjTros .Checked) return "MtrosTK" ;
         else if(rbt_grupEvProjekt.Checked) return "ProjektCD";
         else if(rbt_grupEvOsoba  .Checked) return "StrG_40" ;
         else if(rbt_grupEvOrmar  .Checked) return "StrC_32" ;
         else                               return "";
      }
   }
   
   public string Fld_GrupiranjeUgovora
   { 
      get
      {
              if(rbt_grUgNull   .Checked) return "";
         else if(rbt_grUgPartner.Checked) return "KupdobName" ;
         else if(rbt_grUgVrstUg .Checked) return "StrH_32" ;
         else                             return "";
      }
   }

   public string Fld_GrupiranjeSMD
   { 
      get
      {
              if(rbt_grupSMDNull    .Checked) return "";
         else if(rbt_grupSMDPartner .Checked) return "KupdobName" ;
         else if(rbt_grupSMDPersonCD.Checked) return "PersonCD" ;
         else if(rbt_grupSMDMjTros  .Checked) return "MtrosTK" ;
         else if(rbt_grupSMDProjekt .Checked) return "ProjektCD";
         else                                 return "";
      }
   }

   public bool   Fld_BiloGdjeUnapomeni  { get { return cbx_biloGdjeUnapomeni.Checked; } set {cbx_biloGdjeUnapomeni.Checked = value; } }
   public string Fld_Napomena           { get { return tbx_Napomena.Text;             } set {tbx_Napomena.Text = value;             } }
   public string Fld_ProjektCD          { get { return tbx_ProjektCD.Text;            } set {tbx_ProjektCD.Text = value;             } }
   public bool   Fld_BiloGdjeUstrC_32   { get { return cbx_biloGdjeUstrC_32.Checked;  } set {cbx_biloGdjeUstrC_32.Checked = value;  } }
   public string Fld_StrC_32            { get { return tbx_strC_32.Text;              } set {tbx_strC_32.Text = value;              } }
   public bool   Fld_BiloGdjestrH_32    { get { return cbx_biloGdjeUStrH_32.Checked;  } set {cbx_biloGdjeUStrH_32.Checked = value;  } }
   public string Fld_StrH_32            { get { return tbx_strH_32.Text;              } set {tbx_strH_32.Text = value;              } }
   public string Fld_StrA_40            { get { return tbx_strA_40.Text;              } set {tbx_strA_40.Text = value;              } }
   public string Fld_StrG_40            { get { return tbx_strG_40.Text;              } set {tbx_strG_40.Text = value;              } }
   public bool   Fld_NeedsHorizontalLine{ get { return cb_Line.Checked;               } set { cb_Line.Checked = value;              } }

   public string Fld_GrupiranjeIRV
   { 
      get
      {
              if(rbt_irvNull   .Checked) return "";
//         else if(rbt_irvPartner.Checked) return "T_kupdobCd" ;
         else if(rbt_irvProjekt.Checked) return "T_kpdbUlBrA_32";
         else if(rbt_irvMjesto .Checked) return "T_kpdbUlBrB_32" ;
         else if(rbt_irvZadatak.Checked) return "T_opis_128";
         else                            return "";
      }
   }

   public string Fld_ZaMMYYYY { get { return tbx_zaMMYYYY.Text; } set { tbx_zaMMYYYY.Text = value; } }

   public bool Fld_IsMVR_RadMj
   {
      get
      {
         if(rbt_mvrSviRadnici.Checked) return true;
         else                          return false;
      }
   }

   public bool Fld_IsPrekoPFS   { get { return cbx_prekoPFS  .Checked; } set { cbx_prekoPFS  .Checked = value; } }
   public bool Fld_IsGrupaPoStr { get { return cbx_grupaPoStr.Checked; } set { cbx_grupaPoStr.Checked = value; } }
   public bool Fld_IsOnlyUkPers { get { return cbx_onlyUkPers.Checked; } set { cbx_onlyUkPers.Checked = value; } }

   public VvSQL.SorterType Fld_Sort
   {
      get
      {
              if(rbSort_Datum.Checked) return VvSQL.SorterType.DokDate;
         else if(rbSort_Broj .Checked)  return VvSQL.SorterType.DokNum ;
         else                          return VvSQL.SorterType.None   ;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.DokDate: rbSort_Datum.Checked = true; break;
            case VvSQL.SorterType.DokNum : rbSort_Broj.Checked  = true; break;
         }
      }
   }

   public ZXC.JeliJeTakav Fld_IsAktivan
   {
      get
      {
              if(rbt_isAktivan  .Checked) return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_isNeaktivan.Checked) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_sviRUG     .Checked) return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsAktivan: who df is checked?");
      }
   }



   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private VvRpt_Mix_Filter TheXtransFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VvRpt_Mix_Filter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheXtransFilter = (VvRpt_Mix_Filter)_filter_data;

      if(TheXtransFilter != null)
      {
         Fld_TtNumOd       = TheXtransFilter.TtNumOd; 
         Fld_TtNumDo       = TheXtransFilter.TtNumDo;
         Fld_TT            = TheXtransFilter.TT;
         Fld_DatumOd       = TheXtransFilter.DatumOd;
         Fld_DatumDo       = TheXtransFilter.DatumDo;
         Fld_KD_naziv      = TheXtransFilter.KD_naziv  ;
         Fld_KD_sifra      = TheXtransFilter.KD_sifra  ;
         Fld_KD_ticker     = TheXtransFilter.KD_ticker ;
         Fld_MT_naziv      = TheXtransFilter.MT_naziv  ;
         Fld_MT_sifra      = TheXtransFilter.MT_sifra  ;
         Fld_MT_ticker     = TheXtransFilter.MT_ticker ;
         Fld_PersonCd      = TheXtransFilter.PersonCD      ;
         Fld_PersonPrezime = TheXtransFilter.PresonPrezime ;
         Fld_Napomena      = TheXtransFilter.Napomena      ;
         Fld_ProjektCD     = TheXtransFilter.ProjektCd     ;
         Fld_StrC_32       = TheXtransFilter.StrC_32;
         Fld_StrH_32       = TheXtransFilter.StrH_32;  
         Fld_StrA_40       = TheXtransFilter.StrA_40; 
         Fld_StrG_40       = TheXtransFilter.StrG_40;
         Fld_ZaMMYYYY      = TheXtransFilter.ZaMMYYYY;
         Fld_IsPrekoPFS    = TheXtransFilter.IsPrekoPFS;
         Fld_IsGrupaPoStr  = TheXtransFilter.IsGrupaPoStr;
         Fld_IsOnlyUkPers  = TheXtransFilter.IsOnlyUkPers;
         Fld_Sort          = TheXtransFilter.SorterType_Dokument;

         Fld_NeedsHorizontalLine = TheXtransFilter.NeedsHorizontalLine;

      }
      
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheXtransFilter.DatumOd       = Fld_DatumOd;
      TheXtransFilter.DatumDo       = Fld_DatumDo;
      TheXtransFilter.TT            = Fld_TT;
      TheXtransFilter.TtNumOd       = Fld_TtNumOd ;
      TheXtransFilter.TtNumDo       = Fld_TtNumDo ;
      TheXtransFilter.KD_naziv      = Fld_KD_naziv;
      TheXtransFilter.KD_sifra      = Fld_KD_sifra;
      TheXtransFilter.KD_ticker     = Fld_KD_ticker;
      TheXtransFilter.MT_naziv      = Fld_MT_naziv;
      TheXtransFilter.MT_sifra      = Fld_MT_sifra;
      TheXtransFilter.MT_ticker     = Fld_MT_ticker;
      TheXtransFilter.PersonCD      = Fld_PersonCd;
      TheXtransFilter.PresonPrezime = Fld_PersonPrezime;
      TheXtransFilter.Napomena      = Fld_Napomena;
      TheXtransFilter.StrC_32       = Fld_StrC_32 ;
      TheXtransFilter.StrH_32       = Fld_StrH_32 ;
      TheXtransFilter.StrA_40       = Fld_StrA_40 ;
      TheXtransFilter.StrG_40       = Fld_StrG_40 ;
      TheXtransFilter.ProjektCd     = Fld_ProjektCD ;
      TheXtransFilter.ZaMMYYYY      = Fld_ZaMMYYYY;

      TheXtransFilter.GrupiranjePutNal     = Fld_GrupiranjePutNal  ;
      TheXtransFilter.GrupiranjeZahtjeva   = Fld_GrupiranjeZahtjeva;
      TheXtransFilter.GrupiranjeEvidencija = Fld_GrupiranjeEvidencija;
      TheXtransFilter.GrupiranjeUgovora    = Fld_GrupiranjeUgovora ;
      TheXtransFilter.GrupiranjeSMD        = Fld_GrupiranjeSMD;
      TheXtransFilter.GrupiranjeIRV        = Fld_GrupiranjeIRV;
      TheXtransFilter.IsMVR_RadMj          = Fld_IsMVR_RadMj;
      TheXtransFilter.IsPrekoPFS           = Fld_IsPrekoPFS;
      TheXtransFilter.IsGrupaPoStr         = Fld_IsGrupaPoStr ;
      TheXtransFilter.IsOnlyUkPers         = Fld_IsOnlyUkPers;
      TheXtransFilter.SorterType_Dokument  = Fld_Sort ;
      
      TheXtransFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;

      TheXtransFilter.IsAktivan           = Fld_IsAktivan;
   }

   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Mix_Filter   theRptFilter     = (VvRpt_Mix_Filter)_vvRptFilter;
      VvMixerReport      theVvMixerReport = (VvMixerReport)_vvReport;

      DateTime dateOD, dateDO;
      string comparer;
      string text, preffix;
      bool isDummy, isCheck;
      uint numOD, numDO, num;
      DataRow drSchema;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      bool shouldIgnoreClassicDateOdDo = false;

      if(theVvMixerReport is RptX_NocenjaInfo) shouldIgnoreClassicDateOdDo = true;

      // Fld_DatumOdDo                                                                                                                                                   

      drSchema = ZXC.MixerSchemaRows[ZXC.MixCI.dokDate];
      dateOD = theRptFilter.DatumOd;
      dateDO = theRptFilter.DatumDo;

      if(dateOD == dateDO) comparer = "  = ";
      else                 comparer = " >= ";

      if(shouldIgnoreClassicDateOdDo == false)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", comparer, ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:",   " <= ", ""));
      }

      // Fld_MtrosCD                                                                                                                                     

      if(theRptFilter.MT_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.mtrosCD], false, "MtrosCD", theRptFilter.MT_sifra, theRptFilter.MT_naziv, "Za mjesto troška:", " = ", ""));
      }
      
      // Fld_Napomena                                                                                                                                    

      drSchema = ZXC.MixerSchemaRows[ZXC.MixCI.napomena];
      text     = theRptFilter.Napomena;
      isCheck  = Fld_BiloGdjeUnapomeni;
      preffix  = isCheck ? "%" : "";

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Napomena", preffix + text + "%", text, "Za napomenu:", " LIKE ", ""));
      }


      ZXC.MIX_FilterStyle filterStyle = theVvMixerReport.FilterStyle; // !!! ovo 'filterStyle' se, zapravo, NIGDJE  NE KORISTI! 

      // Fld_TTnumOdDo                                                                                                                                     

//#if Fld_TTnumOdDo_PROVJERILI

      drSchema = ZXC.MixerSchemaRows[ZXC.MixCI.ttNum];
      numOD    = theRptFilter.TtNumOd;
      numDO    = theRptFilter.TtNumDo;

      if(numOD.NotZero())
      {
         if(numOD == numDO) comparer = " = ";
         else               comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TtNumOD", numOD, numOD.ToString(""), "Od TT broj:", comparer, ""/*, Rtrans.recordName*/));
      }
      else if(numDO.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumOD", numOD, "-", "Od TT broj:", " >= ", ""/*, Rtrans.recordName*/));
      }

      if(numDO.NotZero())
      {
         if(numDO == numOD) isDummy = true;
         else               isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TtNumDO", numDO, numDO.ToString(""), "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }
      else if(numOD.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TtNumDO", numDO, "-", "Do TT broj:", " <= ", ""/*, Rtrans.recordName*/));
      }
//#endif

      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_P) { AddFilterMemberzFor_PutNal_Report (theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_Z) { AddFilterMemberzFor_Zahtjev_Report(theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_S) { AddFilterMemberzFor_SMD_Report    (theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_E) { AddFilterMemberzFor_EVD_Report    (theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_R) { AddFilterMemberzFor_RVR_Report    (theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_G) { AddFilterMemberzFor_GST_Report    (theRptFilter, theVvMixerReport); return; }
      if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.XIZ_B) { AddFilterMemberzFor_ZAS_Report    (theRptFilter, theVvMixerReport); return; }

   }


   private void AddFilterMemberzFor_SMD_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvMixerReport)
   {
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('SMD')", " IN "));

      theRptFilter.TT = Mixer.TT_SMD;
      //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt], false, "TT", theRptFilter.TT, theRptFilter.TT, "Za tip:", " = ", "")); 

      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.kupdobCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za partnera:", " = ", ""));
      }


   }

   private void AddFilterMemberzFor_PutNal_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvRiskReport)
   {
      string text, preffix;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('PNL', 'PNI', 'PNT', 'PNR')", " IN "));

      if(theRptFilter.TT.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt], false, "TT", theRptFilter.TT, theRptFilter.TT, "Za tip:", " = ", "")); }

      if(theRptFilter.PersonCD.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.personCD], false, "PersonCD", theRptFilter.PersonCD, theRptFilter.PresonPrezime , "Za djelatnika:", " = ", ""));
      }
      
      // Fld_StrC_32 - odredište                                                                                                                                    
      text     = theRptFilter.StrC_32;
      preffix = Fld_BiloGdjeUstrC_32 ? "%" : "";

      if(theRptFilter.StrC_32.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strC_32], false, "StrC_32", preffix + text + "%", text, "Za odredište:", " LIKE ", ""));
      }
      
      // Fld_StrH_32 - vozilo                                                                                                                                    
      text     = theRptFilter.StrH_32;
      preffix = Fld_BiloGdjestrH_32 ? "%" : "";

      if(theRptFilter.StrH_32.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strH_32], false, "StrH_32", preffix + text + "%", text, "Za vozilo:", " LIKE ", ""));
      }

   }

   private void AddFilterMemberzFor_Zahtjev_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvRiskReport)
   {

      //theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('ZND', 'ZNU', 'ZFA', 'ZIP', 'ZPJ', 'ZRN', 'ZSI', 'ZMO', 'ZSR', 'ZKO', 'ZPR', 'ZOR', 'ZUS')", " IN "));
      string inSetClause = VvSQL.GetInSetClause(ZXC.luiListaMixTypeZahtjev.Select(lui => lui.Cd));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", inSetClause, " IN "));

      if(theRptFilter.TT.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt], false, "TT", theRptFilter.TT, theRptFilter.TT, "Za tip:", " = ", "")); }

      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.kupdobCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za partnera:", " = ", ""));
      }

      if(theRptFilter.StrA_40.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strA_40], false, "StrA_40", theRptFilter.StrA_40, theRptFilter.StrA_40, "Za odg. izvršitelja:", " = ", "")); }
      if(theRptFilter.StrG_40.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strG_40], false, "StrG_40", theRptFilter.StrG_40, theRptFilter.StrG_40, "Za vod. projekta:", " = ", "")); }

       
       
   }

   private void AddFilterMemberzFor_EVD_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvRiskReport)
   {
      //theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('EPO', 'EUG', 'EZP', 'EPP', 'ETP', 'EMC', 'EPG', 'EAT', 'ESV', 'EUV', 'ERD', 'EOD', 'EKO', 'EPU', 'EOS', 'EN1', 'EN2', 'EN3', 'EN4', 'EN5', 'EN6')", " IN "));
      string inSetClause = VvSQL.GetInSetClause(ZXC.luiListaMixTypeEvidencija.Select(lui => lui.Cd));
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", inSetClause, " IN "));
      
      if(theRptFilter.TT.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt], false, "TT", theRptFilter.TT, theRptFilter.TT, "Za tip:", " = ", "")); }

      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.kupdobCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za partnera:", " = ", ""));
      }

      // Fld_strH_32 vrsta ugovora za RUG                                                                                                                                     

      if(theRptFilter.StrH_32.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strH_32], false, "strH_32", theRptFilter.StrH_32, theRptFilter.StrH_32, "Za vrstu ugovora:", " = ", ""));
      }

      // IsAktivan RUG                                                                                                                           

      string text;
      DataRow drSchema;
      ZXC.JeliJeTakav jeLiTakav;

      drSchema  = ZXC.MixerSchemaRows[ZXC.MixCI.isXxx];
      jeLiTakav = theRptFilter.IsAktivan;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "aktivne";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JeAktivan", true, text, "Samo za", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         text = "neaktivne";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NijeAktivan", false, text, "Samo za", " = ", ""));
      }


   }

   private void AddFilterMemberzFor_RVR_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvMixerReport)
   {
      VvReportUC vvReportUC = (VvReportUC)TheVvUC;

      if(vvReportUC.TheVvReport is RptX_EvidencijaRadnogVremena)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('RVR')", " IN "));
         theRptFilter.TT = Mixer.TT_RVR;
       
         // Fld_PersonCD                                                                                                                                     

         if(theRptFilter.PersonCD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_personCD], false, "personCD", theRptFilter.PersonCD, theRptFilter.PersonCD.ToString(), "", " = ",  ""));
         }

      }
      else if(vvReportUC.TheVvReport is RptX_EvidencijaRadnogVremena_Mjesecna)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('MVR')", " IN "));
         theRptFilter.TT = Mixer.TT_MVR;

         if(theRptFilter.ZaMMYYYY.NotEmpty())
         {
            string text = theRptFilter.ZaMMYYYY;

            theRptFilter.FilterMembers.RemoveAll(fm => fm.name.StartsWith("Date"));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_konto] /* u t_konto je MMYYYY */, false, "ZaMj", text, text, "Za mjesec:", " = ", ""));
         }

         // Fld_PersonCD                                                                                                                                     

         if(theRptFilter.PersonCD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_personCD], false, "personCD", theRptFilter.PersonCD, theRptFilter.PersonCD.ToString(), "", " = ", ""));
         }

      }
      else
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('IRV')", " IN "));
         theRptFilter.TT = Mixer.TT_IRV;

         // Fld_PersonCD                                                                                                                                     

         if(theRptFilter.PersonCD.NotZero())
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.personCD], false, "personCD", theRptFilter.PersonCD, theRptFilter.PersonCD.ToString(), "", " = ", ""));
         }

      }


      // Fld_MtrosCD                                                                                                                                     

      if(theRptFilter.MT_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.mtrosCD], false, "mtrosCD", theRptFilter.MT_sifra, theRptFilter.MT_naziv, "Za mj. troška:", " = ", ""));
      }

      // Fld_ProjektCD                                                                                                                                     

      if(theRptFilter.ProjektCd.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_kpdbUlBrA_32], false, "projektCD", theRptFilter.ProjektCd, theRptFilter.ProjektCd, "Za projekt:", " = ", ""));
      }

   }

   private void AddFilterMemberzFor_GST_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvMixerReport)
   {
      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('GST')", " IN "));

      theRptFilter.TT = Mixer.TT_GST;

      if(theVvMixerReport is RptX_PriBor)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.konto], "konto",    "", " != "));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.isXxx], "isXxx",     0, " = " ));
      }
      else if(theVvMixerReport is RptX_NocenjaInfo)
      {
         DataRow drSchDatumPrijave = ZXC.MixerSchemaRows[ZXC.MixCI.dokDate];
         DateTime dateOD, dateDO;

         dateOD = theRptFilter.DatumOd;
         dateDO = theRptFilter.DatumDo;
   
       //theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDatumPrijave, false, "DateOD", dateOD, dateOD.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDatumPrijave, false, "DateDO", dateDO, dateDO.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));
      }
      else if(theVvMixerReport is RptX_StandardMixerReport) // tu racunamo da je ili KnjigaGostiju Domaca/strana! Ako se pojavi jos jedan RptX_StandardMixerReport onda treba nekak diferencirati! 
      {
         DataRow drSchDrzavaCD = ZXC.MixerSchemaRows[ZXC.MixCI.konto]; 

         if(theRptFilter.IsKnjigaDomaca) // KnjigaGostiju DOMACA 
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDrzavaCD, ZXC.FM_OR_Enum.OPEN_OR , false, "kontoOR1",    "",    "", "", " = ", ""));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDrzavaCD, ZXC.FM_OR_Enum.CLOSE_OR, false, "kontoOR2", "HRV", "HRV", "", " = ", ""));
         }
         else // KnjigaGostiju STRANA 
         {
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDrzavaCD, "kontoAND1",    "", " != "));
            theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchDrzavaCD, "kontoAND2", "HRV", " != "));
         }
      }

   }

   private void AddFilterMemberzFor_ZAS_Report(VvRpt_Mix_Filter theRptFilter, VvMixerReport theVvRiskReport)
   {
      string text, preffix;

      theRptFilter.FilterMembers.Add(new VvSqlFilterMember("tt", "('ZPG', 'ZLJ', 'RUG')", " IN "));

      if(theRptFilter.TT.NotEmpty()) { theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt], false, "TT", theRptFilter.TT, theRptFilter.TT, "Za tip:", " = ", "")); }

      if(theRptFilter.PersonCD.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.personCD], false, "PersonCD", theRptFilter.PersonCD, theRptFilter.PresonPrezime , "Za djelatnika:", " = ", ""));
      }
      
      // Fld_KupdobCD                                                                                                                                     

      if(theRptFilter.KD_sifra.NotZero())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.kupdobCD], false, "kupdobCD", theRptFilter.KD_sifra, theRptFilter.KD_naziv, "Za partnera:", " = ", ""));
      }
   }





   #endregion AddFilterMemberz()

}
