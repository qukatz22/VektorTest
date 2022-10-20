using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public class ExtCjeniciDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_ValName;
   private VvHamper hamp_ValName;

   #endregion Fieldz

   #region Constructor

   public ExtCjeniciDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_EXT_CIJ
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_ValutaName(out hamp_ValName);

      nextY = hamp_ValName.Bottom;
   }
   private void InitializeHamper_ValutaName(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, hamp_napomena.Left, hamp_napomena.Bottom, razmakHamp);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q10un + ZXC.Q7un, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4,            ZXC.Qun4,           ZXC.Qun2, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Link1:", ContentAlignment.MiddleRight);
                        tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel...", GetDB_ColumnSize(DB_ci.externLink1));

      btn_goExLink1 = hamper.CreateVvButton(2, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag = 1;
      btn_goExLink1.TabStop = false;

      btn_goExLink1.Visible = false;
      
      btn_openExLink1 = hamper.CreateVvButton(3, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag = 1;
      btn_openExLink1.TabStop = false;

                          hamper.CreateVvLabel  (4, 0, "IzvVal:", ContentAlignment.MiddleRight);
      tbx_ValName = hamper.CreateVvTextBoxLookUp(5, 0, "tbx_ValName", "Naziv devizne valute", GetDB_ColumnSize(DB_ci.devName));
      tbx_ValName.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_ValName.JAM_Set_LookUpTable(ZXC.luiListaDeviza, (int)ZXC.Kolona.prva);
      // Nota bene; buduci je tbx_DevName VvLookUp TextBox onde mu je tbx_tt.JAM_FieldExitMethod 'potrosena' lookUp methodom. 
      //  tbx_ValName.JAM_FieldExitMethod_2 = new EventHandler(OnExit_DevName_SetValutaNameInUse);


   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn    (ZXC.Q5un, "ArtiklCD", "Opis/Naziv proizvoda, usluge, robe ...");
      T_artiklName_CreateColumn  (0, "NazivArt", "Opis/Naziv proizvoda, usluge, robe ...");
      T_strA_2_CreateColumn      (ZXC.Q2un, "GR1"                     , ""                                      , null);
      T_strC_2_CreateColumn      (ZXC.Q2un, "GR2"                     , ""                                            );
      T_strB_2_CreateColumn      (ZXC.Q2un, "GR3"                     , ""                                            );
      T_vezniDokA_64_CreateColumn(ZXC.Q6un, "Šifra zamjenskog artikla", ""                                      , null);
      T_moneyA_CreateColumn      (ZXC.Q5un, "Cijena"                  , ""                                      ,    2);
      T_konto_CreateColumn       (ZXC.Q5un, "DevName"                 , ""                                            );
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

//   public ExtCjeniciFilterUC TheExtCjeniciFilterUC { get; set; }
//   public ExtCjeniciDocFilter TheExtCjeniciDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      //this.TheExtCjeniciDocFilter = new ExtCjeniciDocFilter(this);

      //TheExtCjeniciFilterUC = new ExtCjeniciFilterUC(this);
      //TheExtCjeniciFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      //ThePanelForFilterUC_PrintTemplateUC.Width = TheExtCjeniciFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //ExtCjeniciDocFilter mixerFilter = (ExtCjeniciDocFilter)vvRptFilter;

      //return new RptX_ExtCjenici(new Vektor.Reports.XIZ.CR_ExtCjeniciDuc(), reportName, mixerFilter);
      //// return new RptX_ExtCjenici(new Vektor.Reports.XIZ.CR_VirFromExtCjenici(), reportName, mixerFilter); 
      return null;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         //return this.TheExtCjeniciDocFilter;
         return null;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         //return this.TheExtCjeniciFilterUC;
         return null;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
    //  SetUpColor(clr_ExtCjenici, Color.Empty, clr_ExtCjenici);
   }

   #endregion Colors

   #region Fld_

   public string Fld_DevName { get { return tbx_ValName.Text; } set { tbx_ValName.Text = value; } }

   public ZXC.ValutaNameEnum Fld_DevNameAsEnum
   {
      get
      {
         if(tbx_ValName.Text.IsEmpty()) return ZXC.ValutaNameEnum.EMPTY;
         else                           return (ZXC.ValutaNameEnum)Enum.Parse(typeof(ZXC.ValutaNameEnum), tbx_ValName.Text, true);
      }
   }



   #endregion Fld_

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {

   }
   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_ValName    )) Fld_DevName     = mixer_rec.DevName;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;

      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);
   }
   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_ValName    )) mixer_rec.DevName     = Fld_DevName    ;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
 }

   #endregion override void PutDefaultBabyDUCfields()

}


public class NazivArtiklaZaKupcaDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDob_cd, tbx_kupDob_tk, tbx_kupDob_Naziv;
   private VvHamper  hamp_kupDob;

   #endregion Fieldz

   #region Constructor

   public NazivArtiklaZaKupcaDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_NAK
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {
      InitializeHamper_KupDob(out hamp_kupDob);
      nextY = hamp_kupDob.Bottom;

      hamp_tt      .Location = new Point(hamp_kupDob.Right                , hamp_kupDob.Top);
      hamp_dokDate .Location = new Point(hamp_kupDob.Right                , hamp_tt.Bottom );
      hamp_dokNum  .Location = new Point(hamp_tt.Right - hamp_dokNum.Width, hamp_tt.Bottom);
      hamp_napomena.Location = new Point(nextX                            , hamp_kupDob.Bottom);

      nextY = hamp_napomena.Bottom;
   }
   private void InitializeHamper_KupDob(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, hamp_napomena.Left, hamp_tt.Top, razmakHamp);
      //                                     0        1           2        3        4  
      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q3un - ZXC.Qun4, ZXC.Q3un - ZXC.Qun4, ZXC.Q10un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                         hamper.CreateVvLabel  (0, 0, "Partner:"        , ContentAlignment.MiddleRight);
      tbx_kupDob_cd    = hamper.CreateVvTextBox(1, 0, "tbx_kupDob_cd"   , "Šifra partnera", GetDB_ColumnSize(DB_ci.kupdobCD));
      tbx_kupDob_tk    = hamper.CreateVvTextBox(2, 0, "tbx_kupDob_tk"   , "Tiker partnera", GetDB_ColumnSize(DB_ci.kupdobTK));
      tbx_kupDob_Naziv = hamper.CreateVvTextBox(3, 0, "tbx_kupDob_Naziv", "Naziv partnera", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDob_cd.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDob_tk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDob_cd   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra ), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_kupDob_tk   .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupDobTextBoxLeave));
      tbx_kupDob_Naziv.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName  ), new EventHandler(AnyKupDobTextBoxLeave));

      this.ControlForInitialFocus = tbx_kupDob_Naziv;
   }

   public void AnyKupDobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         this.originalText = tb.Text;

         kupdob_rec = KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobCD   = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTK   = kupdob_rec.Ticker;
            Fld_KupDobName = kupdob_rec.Naziv;
         }
         else
         {
            Fld_KupDobCDAsTxt = Fld_KupDobTK = Fld_KupDobName = "";
         }
      }
   }


   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_artiklCD_CreateColumn    (ZXC.Q5un   , "ArtiklCD"                  , "Opis/Naziv proizvoda, usluge, robe ...");
      T_artiklName_CreateColumn  (ZXC.Q10un*2, "Naziv artikla iz sifarnika", "Opis/Naziv proizvoda, usluge, robe ...");
      T_vezniDokA_64_CreateColumn(ZXC.Q10un*2, "Naziv artikla za Partnera" , ""                                      , null);
      T_opis_128_CreateColumn    (          0, "Napomena"                  , ""                                      , 128, null);
   }

   #endregion InitializeTheGrid_Columns()

   #region PrintDocumentRecord

//   public ExtCjeniciFilterUC TheExtCjeniciFilterUC { get; set; }
//   public ExtCjeniciDocFilter TheExtCjeniciDocFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      //this.TheExtCjeniciDocFilter = new ExtCjeniciDocFilter(this);

      //TheExtCjeniciFilterUC = new ExtCjeniciFilterUC(this);
      //TheExtCjeniciFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      //ThePanelForFilterUC_PrintTemplateUC.Width = TheExtCjeniciFilterUC.Width;
   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //ExtCjeniciDocFilter mixerFilter = (ExtCjeniciDocFilter)vvRptFilter;

      //return new RptX_ExtCjenici(new Vektor.Reports.XIZ.CR_ExtCjeniciDuc(), reportName, mixerFilter);
      //// return new RptX_ExtCjenici(new Vektor.Reports.XIZ.CR_VirFromExtCjenici(), reportName, mixerFilter); 
      return null;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         //return this.TheExtCjeniciDocFilter;
         return null;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         //return this.TheExtCjeniciFilterUC;
         return null;
      }
   }

   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
    //  SetUpColor(clr_ExtCjenici, Color.Empty, clr_ExtCjenici);
   }

   #endregion Colors

   #region Fld_

   public uint   Fld_KupDobCD      { get { return tbx_kupDob_cd.GetSomeRecIDField(); } set { tbx_kupDob_cd.PutSomeRecIDField(value); } }
   public string Fld_KupDobCDAsTxt { get { return tbx_kupDob_cd.Text;                } set { tbx_kupDob_cd.Text            = value ; } }
   public string Fld_KupDobTK      { get { return tbx_kupDob_tk.Text;                } set { tbx_kupDob_tk.Text            = value ; } }
   public string Fld_KupDobName    { get { return tbx_kupDob_Naziv.Text;             } set { tbx_kupDob_Naziv.Text         = value ; } }
   
   #endregion Fld_

   #region override void PutDefaultBabyDUCfields()

   protected override void PutDefaultBabyDUCfields()
   {

   }
   /*protected*/public override void PutSpecificsFld()
   {

      Fld_TtOpis = ZXC.luiListaMixerType.GetNameForThisCd(mixer_rec.TT);

     if(CtrlOK(tbx_kupDob_cd   )) Fld_KupDobCD   = mixer_rec.KupdobCD  ;
     if(CtrlOK(tbx_kupDob_tk   )) Fld_KupDobTK   = mixer_rec.KupdobTK  ;
     if(CtrlOK(tbx_kupDob_Naziv)) Fld_KupDobName = mixer_rec.KupdobName;

   }
   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDob_cd   )) mixer_rec.KupdobCD   = Fld_KupDobCD  ;
      if(CtrlOK(tbx_kupDob_tk   )) mixer_rec.KupdobTK   = Fld_KupDobTK  ;
      if(CtrlOK(tbx_kupDob_Naziv)) mixer_rec.KupdobName = Fld_KupDobName;
   }

   #endregion override void PutDefaultBabyDUCfields()

}

