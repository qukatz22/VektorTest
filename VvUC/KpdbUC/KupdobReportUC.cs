using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CrystalDecisions.Windows.Forms;

public class KupdobReportUC : VvReportUC
{
   #region Fieldz

   public KupdobFilterUC TheKupdobFilterUC { get; set; }

   #endregion Fieldz

   #region Constructor

   public KupdobReportUC(Control parent, VvRpt_Kupdob_Filter _rptFilter, VvForm.VvSubModul vvSubModul)
   {
     this.TheSubModul  = vvSubModul;
     this.TheRptFilter = _rptFilter;
     
     this.SuspendLayout();
      
     TheKupdobFilterUC        = new KupdobFilterUC(this);
     TheKupdobFilterUC.Parent = TheFilterPanel;

     InitializeVvUserControl(parent);
     
     //  sirina FilterPanela
     TheFilterPanel_Width = TheKupdobFilterUC.Width;

     //CalcTheFilterPanelWidth(); ovo je premjesteno u KupdobFilterUC : VvFilterUC zbog +/- hampera 
                                 //jer se odabirom +/- mjenja visina filtera i onda dobiva vertikalni scroll

     this.ResumeLayout();
   }

   #endregion Constructor

   #region PutFields(), GetFields()

   public override void GetFields(bool fuse)
   {
      GetFilterFields();
   }
 
   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheRptFilter = (VvRpt_Kupdob_Filter)_filter_data;
      
      if(TheRptFilter != null)
      {
         TheKupdobFilterUC.PutFilterFields(TheRptFilter);
      }
      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheKupdobFilterUC.GetFilterFields();
   }

   #endregion PutFields(), GetFields()

   #region Report/Filter

   public VvKupdobReport TheVvKupDobReport
   {
      get { return TheVvReport as VvKupdobReport; } 
   }

   public VvRpt_Kupdob_Filter TheRptFilter
   {
      get;
      set;
   }

   #endregion Report/Filter

   #region Override

   public override VvRptFilter VirtualRptFilter
   {
      get { return this.TheRptFilter; }
   }

   public override void AddFilterMemberz()
   {
      TheKupdobFilterUC.AddFilterMemberz(TheRptFilter, TheVvKupDobReport);
   }

   public override void ResetRptFilterRbCbControls()
   {
      TheKupdobFilterUC.cb_Line.Checked = false;
      TheKupdobFilterUC.rbt_SveCPj.Checked = TheKupdobFilterUC.rbt_SveOF.Checked = TheKupdobFilterUC.rbt_SveSD.Checked = true;
   }

   #endregion Override

}

public class KupdobFilterUC : VvFilterUC
{
   #region Fieldz

   private VvHamper hamp_OdDo, hamp_Grupa, hamp_Samo, hamp_sort,
                     hamp_ObrtFirma, hamp_StranacDomaci, hamp_CentralaPoslJed, hamp_Open, hamp_mtros, hamp_KDB;
   public VvHamper   hamp_izuzet;
   private VvTextBox tbx_recIDOd, tbx_RecIDdo,tbx_tickerOd, tbx_KD_tickerDo,tbx_nazivOd, tbx_nazivDo, 
                     tbxLookUp_Grupa, tbx_DjelatOpis, tbxLookUp_Djelat, tbx_OpcinaOpis, tbxLookUp_Opcina,
                     tbx_ZupanOpis, tbxLookUp_Zupan,
                     tbx_grad, tbx_prezime, tbx_mbr, tbx_oib, tbx_ziro,
                     tbx_Cent_ID, tbx_Cent_Ticker, tbx_Cent_Naziv,
                     tbx_Putnik_ID, tbx_Putnik_Naziv;

   public RadioButton rbt_SveOF, rbt_Obrt, rbt_Firme, rbt_Stranac, rbt_Domaci, rbt_SveSD, rbt_Centrala, rbt_PoslJed, rbt_SveCPj,
                      rbt_Svi, rbt_MTros, rbt_NoMtros, rbt_SviPrj, rbt_Izuzet, rbt_NoIzuzet;
   private RadioButton rbt_sortNaziv, rbt_sortSifra, rbt_sortTicker, rbt_sortGrad, rbt_sortPrezime, rbt_sortOIB;
   private Button      btn_PlusMinus;
   private Label       lblCrta;
   private CheckBox    cbxOnlyBanka, cbxOnlyKup, cbxOnlyDob, cbxOnlyOsobe, cbxOnlyHasFak;

   #endregion Fieldz

   #region Constructor

   /// <summary>
   /// nastaje preko constructora FinFilterUC-a
   /// </summary>

   public KupdobFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;

      InitializeHamper_OdDo(out hamp_OdDo);

      MaxHamperWidth = hamp_OdDo.Width;
      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);
      
      nextY = hamp_OdDo.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Sort(out hamp_sort);

      nextY = hamp_sort.Bottom + razmakIzmjedjuHampera;

      CreateHamperOpen();

      nextY = hamp_Open.Bottom;
      InitializeHamper_Grupa(out hamp_Grupa);

      nextY = hamp_Grupa.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Samo(out hamp_Samo);

      nextY = hamp_Samo.Bottom + razmakIzmjedjuHampera;

      InitializeHamper_ObrtFirma(out hamp_ObrtFirma);

      nextX = hamp_ObrtFirma.Right + razmakIzmjedjuHampera;
      InitializeHamper_StranacDomaci(out hamp_StranacDomaci);

      nextX = hamp_StranacDomaci.Right + razmakIzmjedjuHampera;
      InitializeHamper_CentralaPJ(out hamp_CentralaPoslJed);

      nextX = ZXC.Qun2 - ZXC.Qun4;
      nextY = hamp_CentralaPoslJed.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_MTros(out hamp_mtros);

      nextX = hamp_mtros.Right + razmakIzmjedjuHampera;
      InitializeHamper_Izuzeti(out hamp_izuzet);

      nextX = ZXC.Qun2 - ZXC.Qun4;
      nextY = hamp_izuzet.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_KupDobBanka(out hamp_KDB);

      nextX = ZXC.Qun2 - ZXC.Qun4;
      nextY     = LocationOfHamper_HorLine(nextX, hamp_Open.Bottom + razmakIzmjedjuHampera, MaxHamperWidth) + razmakIzmjedjuHampera;
      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);// jer se ponasa ko reportFilter

      this.ResumeLayout();
   }


   #endregion Constructor
   
   #region Hamper_s

   private void InitializeHamper_OdDo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbOd, lbDo, lbSifra, lbTicker, lbNaziv;

      lbOd = hamper.CreateVvLabel(1, 0, "OD:", ContentAlignment.MiddleCenter);
      lbDo = hamper.CreateVvLabel(2, 0, "DO:", ContentAlignment.MiddleCenter);

      lbSifra   = hamper.CreateVvLabel(0, 1, "Šifra:"  , ContentAlignment.MiddleRight);
      lbTicker  = hamper.CreateVvLabel(0, 2, "Ticker:" , ContentAlignment.MiddleRight);
      lbNaziv   = hamper.CreateVvLabel(0, 3, "Naziv:"  , ContentAlignment.MiddleRight);

      tbx_recIDOd  = hamper.CreateVvTextBox(1, 1, "tbx_KD_sifraod", "Od šifre partnera", 6);
      tbx_RecIDdo  = hamper.CreateVvTextBox(2, 1, "tbx_KD_sifrado", "Do šifre partnera", 6);

      tbx_tickerOd    = hamper.CreateVvTextBox(1, 2, "tbx_KD_tickerod", "Od tickera partnera", 6);
      tbx_KD_tickerDo = hamper.CreateVvTextBox(2, 2, "tbx_KD_tickerdo", "Do tickera partnera", 6);
      
      tbx_nazivOd  = hamper.CreateVvTextBox(1, 3, "tbx_KupDobod", "Od naziva partnera", 30);
      tbx_nazivDo  = hamper.CreateVvTextBox(2, 3, "tbx_KupDobdo", "Do naziva partnera", 30);

      tbx_recIDOd .JAM_MustTabOutBeforeSubmit = tbx_RecIDdo    .JAM_MustTabOutBeforeSubmit = true;
      tbx_tickerOd.JAM_MustTabOutBeforeSubmit = tbx_KD_tickerDo.JAM_MustTabOutBeforeSubmit = true;
      tbx_nazivOd .JAM_MustTabOutBeforeSubmit = tbx_nazivDo    .JAM_MustTabOutBeforeSubmit = true;


      if(TheVvUC.TheSubModul.modulEnum == ZXC.VvModulEnum.MODUL_PRJKT)
      {
         tbx_recIDOd    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterKCD   .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra) , null);
         tbx_RecIDdo    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterKCD   .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra) , null);
         tbx_tickerOd   .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker), null);
         tbx_KD_tickerDo.JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker), null);
         tbx_nazivOd    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterNaziv .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv) , null);
         tbx_nazivDo    .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterNaziv .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv) , null);
      }
      else
      {
         tbx_recIDOd    .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , null);
         tbx_RecIDdo    .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD   .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , null);
         tbx_tickerOd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), null);
         tbx_KD_tickerDo.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), null);
         tbx_nazivOd    .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , null);
         tbx_nazivDo    .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv .SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , null);
      }
      tbx_tickerOd.JAM_CharacterCasing = tbx_KD_tickerDo.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_recIDOd.JAM_MarkAsNumericTextBox(0, false);
      tbx_RecIDdo.JAM_MarkAsNumericTextBox(0, false);
      tbx_recIDOd.TextAlign = tbx_RecIDdo.TextAlign = HorizontalAlignment.Left;
      tbx_recIDOd.JAM_FillCharacter = '0';
      tbx_RecIDdo.JAM_FillCharacter = '0';

      VvHamper.HamperStyling(hamper);
  }

   private void InitializeHamper_Sort(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 3, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl = hamper.CreateVvLabel  (0, 0, "Slijed ispisa:", ContentAlignment.MiddleLeft);

      rbt_sortSifra   = hamper.CreateVvRadioButton(0, 1, null, "Po šifri"    , TextImageRelation.ImageBeforeText);
      rbt_sortTicker  = hamper.CreateVvRadioButton(1, 1, null, "Po tickeru"  , TextImageRelation.ImageBeforeText);
      rbt_sortNaziv   = hamper.CreateVvRadioButton(2, 1, null, "Po nazivu"   , TextImageRelation.ImageBeforeText);
      rbt_sortGrad    = hamper.CreateVvRadioButton(0, 2, null, "Po gradu"    , TextImageRelation.ImageBeforeText);
      rbt_sortPrezime = hamper.CreateVvRadioButton(1, 2, null, "Po prezimenu", TextImageRelation.ImageBeforeText);
      rbt_sortOIB     = hamper.CreateVvRadioButton(2, 2, null, "Po OIB-u"    , TextImageRelation.ImageBeforeText);

      rbt_sortSifra.Checked = true;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   private void CreateHamperOpen()
   {
      hamp_Open          = new VvHamper(2, 1, "", this, false);
      hamp_Open.Location = new Point(nextX, nextY);

      hamp_Open.VvColWdt      = new int[] { ZXC.QUN, ZXC.Q3un - ZXC.Qun2 };
      hamp_Open.VvSpcBefCol   = new int[] { 0, ZXC.Qun4 };
      hamp_Open.VvRightMargin = hamp_Open.VvLeftMargin;

      hamp_Open.VvRowHgt       = new int[] { ZXC.QUN };
      hamp_Open.VvSpcBefRow    = new int[] { 0 };
      hamp_Open.VvBottomMargin = hamp_Open.VvTopMargin;

      btn_PlusMinus     = hamp_Open.CreateVvButton(0, 0, new EventHandler(btn_PlusMinus_Click), "+");
      btn_PlusMinus.Tag = "PlusMinus";
      Label lblNaslov = hamp_Open.CreateVvLabel(1, 0, "Dodatno", ContentAlignment.MiddleLeft);

      lblCrta           = new Label();
      lblCrta.Parent    = this;
      lblCrta.Location  = new Point(hamp_Open.Right, hamp_Open.Top + ZXC.Qun4 + ZXC.Qun5);
      lblCrta.Size      = new Size(ZXC.Q4un, 1);
      lblCrta.BackColor = Color.LightGray;
   }

   private void btn_PlusMinus_Click(object sender, System.EventArgs e)
   {
      Button btn = sender as Button;

      if (btn.Text == "+")
      {
         btn.Text = "-";
         hamp_Grupa.          Visible = 
         hamp_ObrtFirma.      Visible = 
         hamp_Samo.           Visible =
         hamp_StranacDomaci.  Visible =
         hamp_CentralaPoslJed.Visible = 
         hamp_mtros.          Visible = 
         hamp_KDB.            Visible = true;
         if(TheVvUC.TheSubModul.subModulEnum == ZXC.VvSubModulEnum.PRIZ)
         {
            hamp_izuzet.Visible   = true;
            cbxOnlyHasFak.Visible = true;
         }
         lblCrta.             Visible = false;

         nextY = LocationOfHamper_HorLine(nextX, hamp_KDB.Bottom + razmakIzmjedjuHampera, MaxHamperWidth) + razmakIzmjedjuHampera;
         this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);
         ((KupdobReportUC)TheVvUC).CalcTheFilterPanelWidth();
      }

      else
      {
         btn.Text = "+";
         hamp_Grupa.          Visible = 
         hamp_ObrtFirma.      Visible = 
         hamp_Samo.           Visible =
         hamp_StranacDomaci.  Visible =
         hamp_CentralaPoslJed.Visible = 
         hamp_mtros.          Visible = 
         hamp_KDB.            Visible = false;
         hamp_izuzet.         Visible = false;
         lblCrta.             Visible = true;

         nextY = LocationOfHamper_HorLine(nextX, hamp_Open.Bottom + razmakIzmjedjuHampera, MaxHamperWidth) + razmakIzmjedjuHampera;
         this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);
         ((KupdobReportUC)TheVvUC).CalcTheFilterPanelWidth();
      }
   }

   private void InitializeHamper_Grupa(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q3un + ZXC.Qun2, ZXC.Q7un - ZXC.Qun2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN, ZXC.QUN, ZXC.QUN, ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbGrupa, lblDjelat, lblOpcina, lblZupan; 
      lbGrupa   = hamper.CreateVvLabel(0, 0, "Grupa:"     , ContentAlignment.MiddleRight);
      lblDjelat = hamper.CreateVvLabel(0, 1, "Djelatnost:", ContentAlignment.MiddleRight);
      lblOpcina = hamper.CreateVvLabel(0, 2, "Općina:"    , ContentAlignment.MiddleRight);
      lblZupan  = hamper.CreateVvLabel(0, 3, "Županija:"  , ContentAlignment.MiddleRight);
      
      tbxLookUp_Grupa = hamper.CreateVvTextBoxLookUp(1, 0, "tbxLookUp_Grupa", "Samo za grupu partnera");
      tbxLookUp_Grupa.JAM_Set_LookUpTable(ZXC.luiListaGrupaPartnera, (int)ZXC.Kolona.prva);
      tbxLookUp_Grupa.JAM_lookUp_NOTobligatory  = true;
      tbxLookUp_Grupa.JAM_lookUp_MultiSelection = true;
      tbxLookUp_Grupa.JAM_CharacterCasing = CharacterCasing.Upper;
        
      tbx_DjelatOpis   = hamper.CreateVvTextBox      (2, 1, "tbx_DjelatOpis", "");
      tbxLookUp_Djelat = hamper.CreateVvTextBoxLookUp(1, 1, "tbxLookUp_Djelat", "Samo za djelatnost", 6);
      tbxLookUp_Djelat.JAM_Set_LookUpTable(ZXC.luiListaDjelat, (int)ZXC.Kolona.prva);
      tbxLookUp_Djelat.JAM_lui_NameTaker_JAM_Name = tbx_DjelatOpis.JAM_Name; 

      tbx_OpcinaOpis   = hamper.CreateVvTextBox      (2, 2, "tbx_OpcinaOpis", "");
      tbxLookUp_Opcina = hamper.CreateVvTextBoxLookUp(1, 2, "tbxLookUp_Opcina", "Samo za opcinu", 4);
      tbxLookUp_Opcina.JAM_Set_LookUpTable(ZXC.luiListaOpcina, (int)ZXC.Kolona.prva);
      tbxLookUp_Opcina.JAM_lui_NameTaker_JAM_Name = tbx_OpcinaOpis.JAM_Name;

      tbx_ZupanOpis   = hamper.CreateVvTextBox      (2, 3, "tbx_ZupanOpis", "");
      tbxLookUp_Zupan = hamper.CreateVvTextBoxLookUp(1, 3, "tbxLookUp_Zupan", "Samo za zupaniju", 3);
      tbxLookUp_Zupan.JAM_Set_LookUpTable(ZXC.luiListaZupanija, (int)ZXC.Kolona.prva);
      tbxLookUp_Zupan.JAM_lui_NameTaker_JAM_Name = tbx_ZupanOpis.JAM_Name; 

      tbxLookUp_Grupa.JAM_CharacterCasing  = tbxLookUp_Djelat.JAM_CharacterCasing =
      tbxLookUp_Opcina.JAM_CharacterCasing = tbxLookUp_Zupan.JAM_CharacterCasing = CharacterCasing.Upper;

      tbxLookUp_Grupa.JAM_WriteOnly  = tbxLookUp_Djelat.JAM_WriteOnly =
      tbxLookUp_Opcina.JAM_WriteOnly = tbxLookUp_Zupan.JAM_WriteOnly  = true;

      tbx_DjelatOpis.JAM_ReadOnly =
      tbx_OpcinaOpis.JAM_ReadOnly = tbx_ZupanOpis.JAM_ReadOnly  = true;

      tbx_DjelatOpis.Tag =
      tbx_OpcinaOpis.Tag = tbx_ZupanOpis.Tag  = ZXC.vvColors.userControl_BackColor;
      
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;
   }

   private void InitializeHamper_Samo(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 8, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un, ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN,  ZXC.QUN ,  ZXC.QUN,  ZXC.QUN};
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbGrad, lbPrezime, lbMbr, lblZiro, lblCetr, lblPutnik, lbOib;
      
      lbGrad      = hamper.CreateVvLabel(0, 0, "Grad:"      , ContentAlignment.MiddleRight);
      lbPrezime   = hamper.CreateVvLabel(0, 1, "Prezime:"   , ContentAlignment.MiddleRight);
      lbOib       = hamper.CreateVvLabel(0, 2, "OIB:"       , ContentAlignment.MiddleRight);
      lbMbr       = hamper.CreateVvLabel(0, 3, "Mat.br:"    , ContentAlignment.MiddleRight);
      lblZiro     = hamper.CreateVvLabel(0, 4, "Žiro-račun:", ContentAlignment.MiddleRight);
      lblPutnik   = hamper.CreateVvLabel(0, 5, "Putnik:"    , ContentAlignment.MiddleRight);
      lblCetr     = hamper.CreateVvLabel(0, 6, "Centrala:"  , ContentAlignment.MiddleRight);
      
      tbx_grad    = hamper.CreateVvTextBox(1, 0, "tbx_grad"   , "Samo za grad");
      tbx_prezime = hamper.CreateVvTextBox(1, 1, "tbx_prezime", "Samo za prezime");
      tbx_oib     = hamper.CreateVvTextBox(1, 2, "tbx_oib"    , "Samo za OIB");
      tbx_mbr     = hamper.CreateVvTextBox(1, 3, "tbx_mbr"    , "Samo za maticni broj");
      tbx_ziro    = hamper.CreateVvTextBox(1, 4, "tbx_ziro"   , "Samo za ziro-racun");

      tbx_Putnik_ID    = hamper.CreateVvTextBox(1, 5, "tbx_PutnikCd", "Samo za putnika -sifra", 6);
      tbx_Putnik_Naziv = hamper.CreateVvTextBox(2, 5, "tbx_Putnik"  , "Samo za putnika - naziv");
      tbx_Putnik_ID.JAM_FillCharacter = '0';
      tbx_Putnik_ID.JAM_MarkAsNumericTextBox(0, false);

      tbx_Cent_ID     = hamper.CreateVvTextBox(1, 6, "tbx_flitKupdobID"   , "Samo za centralu - sifra", 6);
      tbx_Cent_Ticker = hamper.CreateVvTextBox(2, 6, "tbx_flitCent_Ticker", "Samo za centralu - ticker");
      tbx_Cent_Naziv  = hamper.CreateVvTextBox(1, 7, "tbx_flitKupdobNaziv", "Samo za centralu - naziv", 30, 1, 0);

      tbx_Cent_ID.JAM_CharEdits     = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_Cent_Ticker.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_Cent_ID.JAM_MarkAsNumericTextBox(0, false);

      //==============================================================================================================================

      tbx_Cent_ID.JAM_SetAutoCompleteData    (Kupdob.recordName, Kupdob.sorterKCD.SortType    , ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra) , new EventHandler(AnyKupdobTextBoxLeave));
      tbx_Cent_Ticker.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_Cent_Naziv.JAM_SetAutoCompleteData (Kupdob.recordName, Kupdob.sorterNaziv.SortType , ZXC.AutoCompleteRestrictor.KID_Centrala_Only, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName)  , new EventHandler(AnyKupdobTextBoxLeave));

      // 3.5.2011. nemamo pojma zasto smo ovo tako zadali / neradi ctrlF -- pa ya sada neka bude ovako 
      //tbx_Cent_ID.JAM_DisableUpdateVvDataRecordActions =
      //tbx_Cent_Ticker.JAM_DisableUpdateVvDataRecordActions =
      //tbx_Cent_Naziv.JAM_DisableUpdateVvDataRecordActions = true;
      //==============================================================================================================================

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

      hamper.Visible = false;
   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

 //     if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != TheVvUC.originalText)
      {
         TheVvUC.originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(TheVvUC.FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_CentralaNaziv  = kupdob_rec.Naziv;
            Fld_CentralaID     = kupdob_rec.KupdobCD/*RecID*/;
            Fld_CentralaTicker = kupdob_rec.Ticker;
         }
         else
         {
            Fld_CentralaNaziv = Fld_CentralaTicker = Fld_CentralaIDAsTxt = "";
         }
      }
   }

   private void InitializeHamper_ObrtFirma(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Fir/Obr", ContentAlignment.MiddleLeft);

      rbt_SveOF = hamper.CreateVvRadioButton(0, 1, null, "Svi"  , TextImageRelation.ImageBeforeText);
      rbt_Firme = hamper.CreateVvRadioButton(0, 2, null, "Firme", TextImageRelation.ImageBeforeText);
      rbt_Obrt  = hamper.CreateVvRadioButton(0, 3, null, "Obrti", TextImageRelation.ImageBeforeText);

      rbt_SveOF.Checked = true;
      rbt_SveOF.Tag     = true;

      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;

   }

   private void InitializeHamper_StranacDomaci(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Dom/Str", ContentAlignment.MiddleLeft);

      rbt_SveSD   = hamper.CreateVvRadioButton(0, 1, null, "Svi"   , TextImageRelation.ImageBeforeText);
      rbt_Domaci  = hamper.CreateVvRadioButton(0, 2, null, "Domaći", TextImageRelation.ImageBeforeText);
      rbt_Stranac = hamper.CreateVvRadioButton(0, 3, null, "Strani", TextImageRelation.ImageBeforeText);

      rbt_SveSD.Checked = true;
      rbt_SveSD.Tag     = true;

      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;

   }
 
   private void InitializeHamper_CentralaPJ(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q3un + ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Cent/PJed", ContentAlignment.MiddleLeft);

      rbt_SveCPj   = hamper.CreateVvRadioButton(0, 1, null, "Svi"     , TextImageRelation.ImageBeforeText);
      rbt_Centrala = hamper.CreateVvRadioButton(0, 2, null, "Centrale", TextImageRelation.ImageBeforeText);
      rbt_PoslJed  = hamper.CreateVvRadioButton(0, 3, null, "Posl.Jed", TextImageRelation.ImageBeforeText);

      rbt_SveCPj.Checked = true;
      rbt_SveCPj.Tag     = true;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);

      hamper.Visible = false;

   }

   private void InitializeHamper_MTros(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "MjestoTroška", ContentAlignment.MiddleLeft);

      rbt_Svi     = hamper.CreateVvRadioButton(0, 1, null, "Svi"     , TextImageRelation.ImageBeforeText);
      rbt_MTros   = hamper.CreateVvRadioButton(0, 2, null, "MjTroška", TextImageRelation.ImageBeforeText);
      rbt_NoMtros = hamper.CreateVvRadioButton(0, 3, null, "Ostali"  , TextImageRelation.ImageBeforeText);

      rbt_Svi.Checked = true;
      rbt_Svi.Tag     = true;

      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;
   }

   private void InitializeHamper_KupDobBanka(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 6, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Ispiši samo:", ContentAlignment.MiddleLeft);

      cbxOnlyDob    = hamper.CreateVvCheckBox_OLD(0, 1, null, "Dobavljače", RightToLeft.No);
      cbxOnlyKup    = hamper.CreateVvCheckBox_OLD(0, 2, null, "Kupce"     , RightToLeft.No);
      cbxOnlyBanka  = hamper.CreateVvCheckBox_OLD(0, 3, null, "Banke"     , RightToLeft.No);
      cbxOnlyOsobe  = hamper.CreateVvCheckBox_OLD(0, 4, null, "Osobe"     , RightToLeft.No);
      cbxOnlyHasFak = hamper.CreateVvCheckBox_OLD(0, 5, null, "ImajuCIjFk", RightToLeft.No);
      cbxOnlyHasFak.Visible = false;
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;

   }

   private void InitializeHamper_Izuzeti(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 4, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun2 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun12;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl1 = hamper.CreateVvLabel(0, 0, "Projekti", ContentAlignment.MiddleLeft);

      rbt_SviPrj   = hamper.CreateVvRadioButton(0, 1, null, "Svi"    , TextImageRelation.ImageBeforeText);
      rbt_Izuzet   = hamper.CreateVvRadioButton(0, 2, null, "Izuzeti", TextImageRelation.ImageBeforeText);
      rbt_NoIzuzet = hamper.CreateVvRadioButton(0, 3, null, "Ostali ", TextImageRelation.ImageBeforeText);

      rbt_SviPrj.Checked = true;
      rbt_SviPrj.Tag     = true;

      VvHamper.HamperStyling(hamper);
      hamper.Visible = false;
   }

   #endregion Hamper_s

   #region Fld_

   public string Fld_TickerOd
   {
      get { return tbx_tickerOd.Text; }
      set {        tbx_tickerOd.Text = value; }
   }
  
   public uint Fld_RecIDod
   {
      get { return ZXC.ValOrZero_UInt(tbx_recIDOd.Text); }
      set {                           tbx_recIDOd.Text = value.ToString("000000"); }

   }
  
   public string Fld_RecIDAsTxtOd
   {
      get { return tbx_recIDOd.Text; }
      set {        tbx_recIDOd.Text = value; }
   }

   public string Fld_NazivOd
   {
      get { return tbx_nazivOd.Text; }
      set {        tbx_nazivOd.Text = value; }
   }

   public string Fld_TickerDo
   {
      get { return tbx_KD_tickerDo.Text; }
      set {        tbx_KD_tickerDo.Text = value; }
   }
  
   public uint Fld_RecIDdo
   {
      get { return ZXC.ValOrZero_UInt(tbx_RecIDdo.Text); }
      set {                           tbx_RecIDdo.Text = value.ToString("000000"); }

   }
   
   public string Fld_RecIDAsTxtDo
   {
      get { return tbx_RecIDdo.Text; }
      set {        tbx_RecIDdo.Text = value; }
   }

   public string Fld_NazivDo
   {
      get { return tbx_nazivDo.Text; }
      set {        tbx_nazivDo.Text = value; }
   }

   public string Fld_Grupa
   {
      get { return tbxLookUp_Grupa.Text; }
      set {        tbxLookUp_Grupa.Text = value; }
   }

   public string Fld_Djelatnost
   {
      get { return tbxLookUp_Djelat.Text; }
      set {        tbxLookUp_Djelat.Text = value; }
   }

   public string Fld_Opcina
   {
      get { return tbxLookUp_Opcina.Text; }
      set {        tbxLookUp_Opcina.Text = value; }
   }

   public string Fld_Zupanija
   {
      get { return tbxLookUp_Zupan.Text; }
      set {        tbxLookUp_Zupan.Text = value; }
   }

   public string Fld_Grad
   {
      get { return tbx_grad.Text; }
      set {        tbx_grad.Text = value; }
   }
   
   public string Fld_Prezime
   {
      get { return tbx_prezime.Text; }
      set {        tbx_prezime.Text = value; }
   }
  
   public string Fld_Oib
   {
      get { return tbx_oib.Text; }
      set {        tbx_oib.Text = value; }
   }
 
   public string Fld_MatBr
   {
      get { return tbx_mbr.Text; }
      set {        tbx_mbr.Text = value; }
   }

   public string Fld_ZiroRacun
   {
      get { return tbx_ziro.Text; }
      set {        tbx_ziro.Text = value; }
   }

   public VvSQL.SorterType Fld_SifrarSort
   {
      get
      {
                   if(rbt_sortSifra.Checked)   return VvSQL.SorterType.Code/*RecID*/;
              else if(rbt_sortTicker.Checked)  return VvSQL.SorterType.Ticker;
              else if(rbt_sortNaziv.Checked)   return VvSQL.SorterType.Name;
              else if(rbt_sortGrad.Checked)    return VvSQL.SorterType.City;
              else if(rbt_sortPrezime.Checked) return VvSQL.SorterType.Person;
              else if(rbt_sortOIB.Checked)     return VvSQL.SorterType.OIB;
         else return VvSQL.SorterType.None;
      }
      set
      {
         switch(value)
         {
            case VvSQL.SorterType.Code/*RecID*/ : rbt_sortSifra.Checked = true; break;
            case VvSQL.SorterType.Ticker: rbt_sortTicker .Checked = true; break;
            case VvSQL.SorterType.Name  : rbt_sortNaziv  .Checked = true; break;
            case VvSQL.SorterType.City  : rbt_sortGrad   .Checked = true; break;
            case VvSQL.SorterType.Person: rbt_sortPrezime.Checked = true; break;
            case VvSQL.SorterType.OIB   : rbt_sortOIB    .Checked = true; break;
         }
      }
   }

   public string Fld_CentralaNaziv
   {
      get { return tbx_Cent_Naziv.Text; }
      set {        tbx_Cent_Naziv.Text = value; }
   }

   public string Fld_CentralaTicker
   {
      get { return tbx_Cent_Ticker.Text; }
      set {        tbx_Cent_Ticker.Text = value; }
   }

   public uint Fld_CentralaID
   {
      get { return ZXC.ValOrZero_UInt(tbx_Cent_ID.Text); }
      set {                           tbx_Cent_ID.Text = value.ToString("000000"); }

   }

   public string Fld_CentralaIDAsTxt
   {
      get { return tbx_Cent_ID.Text; }
      set {        tbx_Cent_ID.Text = value; }
   }

   public string Fld_PutnikNaziv
   {
      get { return tbx_Putnik_Naziv.Text; }
      set {        tbx_Putnik_Naziv.Text = value; }
   }

   public uint Fld_PutnikCd
   {
      get { return ZXC.ValOrZero_UInt(tbx_Putnik_ID.Text); }
      set {                           tbx_Putnik_ID.Text = value.ToString("000000"); }
   }
   
   public string Fld_FilterPutnikCdAsTxt
   {
      get { return tbx_Putnik_ID.Text; }
      set {        tbx_Putnik_ID.Text = value; }
   }

   public bool Fld_NeedsHorizontalLine
   {
      get { return cb_Line.Checked; }
      set {        cb_Line.Checked = value; }
   }

   public ZXC.JeliJeTakav Fld_IsObrt
   {
      get
      {
              if(rbt_Obrt .Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_Firme.Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_SveOF.Checked)  return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsObrt: who df is checked?");
      }
   }

   public ZXC.JeliJeTakav Fld_IsCentrala
   {
      get
      {
              if(rbt_Centrala.Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_PoslJed .Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_SveCPj  .Checked)  return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsCentrala: who df is checked?");
      }
   }

   public ZXC.JeliJeTakav Fld_IsStranac
   {
      get
      {
              if(rbt_Stranac.Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_Domaci .Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_SveSD  .Checked)  return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsStranac: who df is checked?");
      }
   }

   public ZXC.JeliJeTakav Fld_IsMtros
   {
      get
      {
              if(rbt_MTros  .Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_NoMtros.Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_Svi    .Checked)  return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsMtros: who df is checked?");
      }
   }

   public bool Fld_IsDobav
   {
      get { return cbxOnlyDob.Checked; }
      set {        cbxOnlyDob.Checked = value; }
   }
  
   public bool Fld_IsKupac
   {
      get { return cbxOnlyKup.Checked; }
      set {        cbxOnlyKup.Checked = value; }
   }
  
   public bool Fld_IsBanka
   {
      get { return cbxOnlyBanka.Checked; }
      set {        cbxOnlyBanka.Checked = value; }
   }

   public bool Fld_IsOsoba
   {
      get { return cbxOnlyOsobe.Checked; }
      set {        cbxOnlyOsobe.Checked = value; }
   }
   
   public bool Fld_HasZaFakturiranje
   {
      get { return cbxOnlyHasFak.Checked; }
      set {        cbxOnlyHasFak.Checked = value; }
   }

   public ZXC.JeliJeTakav Fld_IsIzuzet
   {
      get
      {
              if(rbt_Izuzet  .Checked)  return ZXC.JeliJeTakav.JE_TAKAV;
         else if(rbt_NoIzuzet.Checked ) return ZXC.JeliJeTakav.NIJE_TAKAV;
         else if(rbt_SviPrj  .Checked)  return ZXC.JeliJeTakav.NEBITNO;

              else throw new Exception("Fld_IsIzuzet: who df is checked?");
      }
   }

   #endregion Fld_

   #region PutFilterFields(), GetFilterFields()

   private VvRpt_Kupdob_Filter TheKupdobFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VvRpt_Kupdob_Filter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheKupdobFilter = (VvRpt_Kupdob_Filter)_filter_data;

      if(TheKupdobFilter != null)
      {
         Fld_SifrarSort = TheKupdobFilter.SorterType_Sifrar;
         
         Fld_RecIDod    = TheKupdobFilter.KupdobCdOd    ;
         Fld_RecIDdo    = TheKupdobFilter.KupdobCdDo    ;
         Fld_TickerOd   = TheKupdobFilter.TickerOd   ;
         Fld_TickerDo   = TheKupdobFilter.TickerDo   ;
         Fld_NazivOd    = TheKupdobFilter.NazivOd    ;
         Fld_NazivDo    = TheKupdobFilter.NazivDo    ;
         Fld_Grad       = TheKupdobFilter.Grad       ;
         Fld_Prezime    = TheKupdobFilter.Prezime    ; 
         Fld_MatBr      = TheKupdobFilter.MatBr      ;
         Fld_Oib        = TheKupdobFilter.Oib        ;
         Fld_ZiroRacun  = TheKupdobFilter.ZiroRn     ;
         Fld_Grupa      = TheKupdobFilter.Grupa      ;
         Fld_Djelatnost = TheKupdobFilter.Djelat     ;
         Fld_Opcina     = TheKupdobFilter.Opcina     ;
         Fld_Zupanija   = TheKupdobFilter.Zupanija   ;

         Fld_CentralaID     = TheKupdobFilter.CentrID    ;
         Fld_CentralaNaziv  = TheKupdobFilter.CentrNaziv ;
         Fld_CentralaTicker = TheKupdobFilter.CentrTicker;
         Fld_PutnikCd       = TheKupdobFilter.PutnikCd   ;
         Fld_PutnikNaziv    = TheKupdobFilter.PutnikNaziv;

         Fld_NeedsHorizontalLine = TheKupdobFilter.NeedsHorizontalLine;

      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      TheKupdobFilter.SorterType_Sifrar = Fld_SifrarSort;

      TheKupdobFilter.KupdobCdOd  =  Fld_RecIDod    ;
      TheKupdobFilter.KupdobCdDo  =  Fld_RecIDdo    ;
      TheKupdobFilter.TickerOd    =  Fld_TickerOd   ;
      TheKupdobFilter.TickerDo    =  Fld_TickerDo   ;
      TheKupdobFilter.NazivOd     =  Fld_NazivOd    ;
      TheKupdobFilter.NazivDo     =  Fld_NazivDo    ;
      TheKupdobFilter.Grad        =  Fld_Grad       ;
      TheKupdobFilter.Prezime     =  Fld_Prezime    ; 
      TheKupdobFilter.Oib         = Fld_Oib        ;
      TheKupdobFilter.MatBr       =  Fld_MatBr      ;
      TheKupdobFilter.ZiroRn      =  Fld_ZiroRacun  ;
      TheKupdobFilter.Grupa       =  Fld_Grupa      ;
      TheKupdobFilter.Djelat      =  Fld_Djelatnost ;
      TheKupdobFilter.Opcina      =  Fld_Opcina     ;
      TheKupdobFilter.Zupanija    =  Fld_Zupanija   ;

      TheKupdobFilter.CentrID      = Fld_CentralaID    ;
      TheKupdobFilter.CentrNaziv   = Fld_CentralaNaziv ;
      TheKupdobFilter.CentrTicker  = Fld_CentralaTicker;
      TheKupdobFilter.PutnikCd     = Fld_PutnikCd      ;
      TheKupdobFilter.PutnikNaziv  = Fld_PutnikNaziv   ;

      TheKupdobFilter.IsObrt       = Fld_IsObrt;
      TheKupdobFilter.IsStranac    = Fld_IsStranac;
      TheKupdobFilter.IsCentrala   = Fld_IsCentrala;
      TheKupdobFilter.IsMTros      = Fld_IsMtros;
      TheKupdobFilter.IsIzuzet     = Fld_IsIzuzet;
      TheKupdobFilter.IsKup        = Fld_IsKupac;
      TheKupdobFilter.IsDob        = Fld_IsDobav;
      TheKupdobFilter.IsBanka      = Fld_IsBanka;
      TheKupdobFilter.IsOsoba      = Fld_IsOsoba;
      TheKupdobFilter.HasZaFaktur  = Fld_HasZaFakturiranje;

      TheKupdobFilter.NeedsHorizontalLine = Fld_NeedsHorizontalLine;

   }

   #endregion PutFilterFields(), GetFilterFields()

   #region AddFilterMemberz()

   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_Kupdob_Filter theRptFilter      = (VvRpt_Kupdob_Filter)_vvRptFilter;
      VvKupdobReport      theVvKupDobReport = (VvKupdobReport)_vvReport;

      DataRow drSchema;
      uint    num, numOD, numDO;
      string  comparer;
      string  text, textOD, textDO, text2;
      bool    isDummy, isCheck;

      ZXC.JeliJeTakav jeLiTakav;

      theRptFilter.FilterMembers.Clear();
      theRptFilter.ClearAllFilters_FromClauseGotTableName();

      isDummy = false;

      // Fld_RecIDodDo                                                                                                                                      

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD/*prjktKupdobCD*/];
     
      numOD = theRptFilter.KupdobCdOd;
      numDO = theRptFilter.KupdobCdDo;

      if(numOD != 0)
      {
         if(numOD == numDO) comparer = " = ";
         else               comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "RecIDod", numOD, numOD.ToString("000000"), "Od šifre:", comparer, ""));
      }
      else if(numDO != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "RecIDod", numOD, "-", "Od šifre:", " >= ", ""));
      }

      if(numDO != 0)
      {
         if(numDO == numOD) isDummy = true;
         else               isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "RecIDdo", numDO, numDO.ToString("000000"), "Do šifre:", " <= ", ""));
      }
      else if(numOD != 0)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "RecIDdo", numDO, "-", "Do šifre:", " <= ", ""));
      }

      // Fld_TickerOdDo                                                                                                                                     

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.ticker];

      textOD = theRptFilter.TickerOd;
      textDO = theRptFilter.TickerDo;
   
      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "TickerOD", textOD, textOD, "Od tickera:", comparer, ""));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TickerOD", textOD, "-", "Od tickera:", " >= ", ""));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else                 isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "TickerDO", textDO, textDO, "Do tickera:", " <= ", ""));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "TickerDO", textDO, "-", "Do tickera:", " <= ", ""));
      }

      // Fld_NazivOdDo                                                                                                                                     

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.naziv];

      textOD = theRptFilter.NazivOd;
      textDO = theRptFilter.NazivDo;

      if(textOD.NotEmpty())
      {
         if(textOD == textDO) comparer = " = ";
         else                 comparer = " >= ";

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NazivOD", textOD, textOD, "Od naziva:", comparer, ""));
      }
      else if(textDO.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "NazivOD", textOD, "-", "Od naziva:", " >= ", ""));
      }

      if(textDO.NotEmpty())
      {
         if(textDO == textOD) isDummy = true;
         else                 isDummy = false;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, isDummy, "NazivDO", textDO, textDO, "Do naziva:", " <= ", ""));
      }
      else if(textOD.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, true, "NazivDO", textDO, "-", "Do naziva:", " <= ", ""));
      }

      // Fld_Grupa                                                                                                                                      
     
      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.tip];
      text     = theRptFilter.Grupa ;

      if(text.NotEmpty())
      {
         text = ZXC.GetOrovaniCharPattern(theRptFilter.Grupa);
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Grupa", text, text, "Za grupu:", " REGEXP ", ""));
      }

      // Fld_Djelatnost                                                                                                                                     
      
      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.sifDcd];
      text     = theRptFilter.Djelat;
      text2 = text + " " + tbx_DjelatOpis.Text;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Djelatnost", text, text2, "Za djelatnost:", " = ", ""));
      }

      // Fld_Zupanija                                                                                                                                     

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.zupCd];
      text     = theRptFilter.Zupanija;
      text2    = text + " " + tbx_ZupanOpis.Text;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Zupanija", text, text2, "Za županiju:", " = ", ""));
      }

      // Fld_Opcina                                                                                                                    

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.opcCd];
      text     = theRptFilter.Opcina;
      text2    = text + " " + tbx_OpcinaOpis.Text;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Opcina", text, text2, "Za općinu:", " = ", ""));
      }
      
      // IsObrt                                                                                                                          

      drSchema  = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isObrt];
      jeLiTakav = theRptFilter.IsObrt;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "Obrt";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JeObrt", true, text, "Za tip:", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         text = "Firma";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Firma", false, text, "Za tip:", " = ", ""));
      }

      // IsStranac                                                                                                                         

      drSchema  = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isFrgn];
      jeLiTakav = theRptFilter.IsStranac;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "Starnac";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JeStranac", true, text, "Za tip:", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         text = "Domaci";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Domaci", false, text, "Za tip:", " = ", ""));
      }

      // IsCentrala                                                                                                                         

      drSchema  = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isCentr];
      jeLiTakav = theRptFilter.IsCentrala;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "Centrala";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Centrala", true, text, "Za tip:", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.centrID];
         text = "Poslovna jedinica";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "PoslJed", 0, text, "Za tip:", " != ", ""));
      }

      // Fld_Grad                                                                                                             
     
      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.grad];
      text     = theRptFilter.Grad;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Grad", text + "%", text, "Za grad:", " LIKE ", ""));
      }

      // Fld_Prezime                                                                                                             

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.prezime];
      text     = theRptFilter.Prezime;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Prezime", text + "%", text, "Za prezime:", " LIKE ", ""));
      }
      
      // Fld_Oib                                                                                                             

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.oib];
      text     = theRptFilter.Oib;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Oib", text + "%", text, "Za OIB:", " LIKE ", ""));
      }


      // Fld_MatBr                                                                                                             

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.matbr];
      text     = theRptFilter.MatBr;

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "MatBr", text + "%", text, "Za mat.br:", " LIKE ", ""));
      }

      // Fld_Ziro                                                                                                             

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.ziro1];
      text     = theRptFilter.ZiroRn;
      string locator = "LOCATE('" + text + "', CONCAT(ziro1, ziro2, ziro3, ziro4))";

      if(text.NotEmpty())
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(locator, "Zirac", 0, text, "Za žiro-račun:", " > "));
      }

      // Fld_PutnikCd                                                                                                                                          

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.putnikID];
      num      = theRptFilter.PutnikCd;

      text = tbx_Putnik_ID.Text + " " + tbx_Putnik_Naziv.Text;
 
      if(num.NotZero())
      {
        theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "PutnikId", num, text, "Za putnika: ", " = ", ""));
      }

      // Fld_PutnikNaziv                                                                                                                                          
      
      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.putName];
      text     = theRptFilter.PutnikNaziv;

      text2 = tbx_Putnik_ID.Text + " " + tbx_Putnik_Naziv.Text;

      if(text.NotEmpty())
      {
        theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "PutnikName", text, text2, "Za putnika: ", " LIKE ", ""));
      }

      // Fld_CentralaID                                                                                                                                     

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.centrID];
      num      = theRptFilter.CentrID;

      if(num.NotZero())
      {
         text = tbx_Cent_ID.Text + " " + tbx_Cent_Ticker.Text + " " + tbx_Cent_Naziv.Text;

         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "CentralaId", num, text, "Za centralu: ", " = ", ""));
      }

      
      // IsMTros                                                                                                                          

      drSchema  = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isMtr];
      jeLiTakav = theRptFilter.IsMTros;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "Mjesto troška";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JeMtr", true, text, "Za ", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         text = "nije mjesto troška";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Firma", false, text, "Za:", " = ", ""));
      }


      // Fld_IsKupac                                                                                                                                          

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isKupac];
      isCheck  = Fld_IsKupac;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, "IsKupac", isCheck, " = "));
      }

      // Fld_IsDobav                                                                                                                                          

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isDobav];
      isCheck  = Fld_IsDobav;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, "IsDobav", isCheck, " = "));
      }


      // Fld_IsBanka                                                                                                                                          

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isBanka];
      isCheck = Fld_IsBanka;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, "IsBanka", isCheck, " = "));
      }
      
      // Fld_IsOsoba                                                                                                                                          

      drSchema = ZXC.KupdobSchemaRows[ZXC.KpdbCI.isZzz];
      isCheck = Fld_IsOsoba;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, "IsOsoba", isCheck, " = "));
      }

      // Fld_HasZaFakturiranje                                                                                                                                          

      isCheck = Fld_HasZaFakturiranje;

      if(isCheck)
      {
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember("HasZaFak", isCheck));
      }

      // IsIzuzet                                                                                                                          

      drSchema  = ZXC.PrjktSchemaRows[ZXC.PrjCI.isSkip];
      jeLiTakav = theRptFilter.IsIzuzet;

      if(jeLiTakav == ZXC.JeliJeTakav.JE_TAKAV)
      {
         text = "izuzeti projekti";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "JeSkip", true, text, "Samo ", " = ", ""));
      }
      else if(jeLiTakav == ZXC.JeliJeTakav.NIJE_TAKAV)
      {
         text = "aktivni projekti";
         theRptFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "NijeSkip", false, text, "Samo ", " = ", ""));
      }

   }

   #endregion AddFilterMemberz()

}
