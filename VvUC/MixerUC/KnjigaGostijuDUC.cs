using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class KnjigaGostijuDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_ime, tbx_prezime,
                     tbx_mjestoRod, tbx_lookUpDrzavlj, tbx_lookUpDrzavaRod, tbx_opisUpDrzavlj, tbx_opisDrzavaRod,
                     tbx_vrstaPutIsprave, tbx_brojPutIspr, tbx_mjestoUlaska,
                     tbx_vrstaGosta, tbx_statusGosta, tbx_tipObjekta,
                     tbx_brojSobe, tbx_adresaSt,
                     tbx_datumRod, tbx_datumUlaska, tbx_datumOdjave,
                     tbx_opisVrstaPutIsp ,
                     tbx_opisMjestoUlaska   ,
                     tbx_opisVrstaGosta     ,
                     tbx_opisStatusGosta    ,
                     tbx_opisTipObjekta, tbx_objektSifra, tbx_moneyBP    
                     ; 

   public VvHamper  hamp_KnjigaGostiju, hamp_datOdjave, hamp_lookUp;

   private VvDateTimePicker dtp_datumUlaska, dtp_datumOdjave, dtp_datumRod;
   private CheckBox cbx_isEU;

   #endregion Fieldz

   #region Constructor

   public KnjigaGostijuDUC(Control parent, Mixer _mixer, VvSubModul vvSubModul) : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
        (Mixer.tt_colName, new string[] 
         { 
               Mixer.TT_GST
         });
   }

   #endregion Constructor

   #region CreateSpecificHampers()
      
   protected override void CreateSpecificHampers()
   {
      
      InitializeHamper_DatOdjave(out hamp_datOdjave);

      hamp_datOdjave.Location = new Point(hamp_dokDate.Right  , hamp_dokDate.Top);
      hamp_tt       .Location = new Point(hamp_datOdjave.Right, hamp_dokDate.Top);

      InitializeHamper_KnjigaGostiju(out hamp_KnjigaGostiju);
      nextY = hamp_KnjigaGostiju.Bottom + ZXC.Qun4;

      InitializeHamper_LookUp(out hamp_lookUp);
      nextY = hamp_lookUp.Bottom + ZXC.Qun4;
      hamp_napomena.Location = new Point(nextX, nextY);
      nextY = hamp_napomena.Bottom;
   }

   private void InitializeHamper_LookUp(out VvHamper hamper)
   {
      hamper = new VvHamper(10, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                  1         2               3                   4              5             6       7         8       9    
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un - ZXC.Qun2, ZXC.Q9un, ZXC.Q2un, ZXC.Q6un + ZXC.Qun4, ZXC.Q3un - ZXC.Qun2, ZXC.QUN , ZXC.Q4un + ZXC.Qun2, ZXC.QUN , ZXC.Q2un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4,             ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;
      hamper.CreateVvLabel(0, 0, "Državljanstvo:"                     , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(4, 0, "Država rođenja:"                    , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "Vrsta putne isprave:"               , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(3, 1, "Br. put. isprave / br. OI - izdana:", 1, 0, ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 2, "Mjesto ulaska u RH:"                , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(3, 2, "Datum ulaska u RH:"                 ,  1, 0, ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 3, "Vrsta gosta:"                       , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(3, 3, "Status gosta:"                      ,  1, 0,ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 4, "Tip objekta:", ContentAlignment.MiddleRight);

      tbx_lookUpDrzavlj    = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_drzavljanstvo"      , "Državljanstvo ");
      tbx_opisUpDrzavlj    = hamper.CreateVvTextBox      (2, 0, "tbx_opisUpDrzavlj"      , "Državljanstvo");
      tbx_lookUpDrzavaRod  = hamper.CreateVvTextBoxLookUp(5, 0, "tbx_drzavaRod"          , "Država rođenja");
      tbx_opisDrzavaRod    = hamper.CreateVvTextBox      (6, 0, "tbx_opisDrzavaRod"      , "Država rođenja", 128, 3, 0);
      tbx_vrstaPutIsprave  = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_vrstaPutIsprave"    , "Vrsta putne isprave");
      tbx_opisVrstaPutIsp  = hamper.CreateVvTextBox      (2, 1, "tbx_opisVrstaPutIsprave", "Vrsta putne isprave");
      tbx_brojPutIspr      = hamper.CreateVvTextBox      (5, 1, "tbx_brojPutIspr"        , "Br. put. isprave / br. OI - izdana", 128, 4, 0);
      tbx_mjestoUlaska     = hamper.CreateVvTextBoxLookUp(1, 2, "tbx_mjestoUlaska"       , "Mjesto ulaska u RH");
      tbx_opisMjestoUlaska = hamper.CreateVvTextBox      (2, 2, "tbx_opisMjestoUlaska"   , "Mjesto ulaska u RH");
      tbx_datumUlaska      = hamper.CreateVvTextBox      (5, 2, "tbx_datumUlsaka"        , "Datum ulaska u RH",GetDB_ColumnSize(DB_ci.dateTimeA), 1, 0);
      tbx_datumUlaska.JAM_IsForDateTimePicker = true;
      dtp_datumUlaska = hamper.CreateVvDateTimePicker(5, 2, "", 1, 0, tbx_datumUlaska);
      dtp_datumUlaska.Name = "dtp_datumUlsaka";
      tbx_vrstaGosta       = hamper.CreateVvTextBoxLookUp(1, 3, "tbx_vrstaGosta"     , "Vrsta gosta");
      tbx_opisVrstaGosta   = hamper.CreateVvTextBox      (2, 3, "tbx_opisVrstaGosta" , "Vrsta gosta");
      tbx_statusGosta      = hamper.CreateVvTextBoxLookUp(5, 3, "tbx_statusGosta"    , "Status gosta");
      
      
                    hamper.CreateVvLabel  (8, 3, "B.P.:", ContentAlignment.MiddleRight);
      tbx_moneyBP = hamper.CreateVvTextBox(9, 3, "tbx_moneyBP", "Iznos boravišne pristojbe", GetDB_ColumnSize(DB_ci.moneyA));
      tbx_moneyBP.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);


      tbx_opisStatusGosta = hamper.CreateVvTextBox      (6, 3, "tbx_opisStatusGosta", "Status gosta", 128, 1, 0);
      tbx_tipObjekta      = hamper.CreateVvTextBoxLookUp(1, 4, "tbx_tipObjekta"     , "Tip objekta"            );
      tbx_opisTipObjekta  = hamper.CreateVvTextBox      (2, 4, "tbx_opisTipObjekta" , "Tip objekta"            );
      tbx_objektSifra     = hamper.CreateVvTextBox      (3, 4, "tbx_ObjektSifra"    , "Tip objekta", 10, 1, 0  );

      cbx_isEU = hamper.CreateVvCheckBox_OLD(3, 0, null, "EU", RightToLeft.No);
      cbx_isEU.Name = "IsEU";
      cbx_isEU.Enabled = false;
      tbx_lookUpDrzavlj.JAM_lui_FlagTaker_JAM_Name = cbx_isEU.Name;


                                hamper.CreateVvLabel  (6, 4, "Broj sobe:", 1, 0, ContentAlignment.MiddleRight);
      tbx_brojSobe            = hamper.CreateVvTextBox(8, 4, "tbx_brojSobe", "Broj Sobe", 10, 1, 0);
      
      tbx_lookUpDrzavlj  .JAM_Set_LookUpTable(ZXC.luiListaDrzave, (int)ZXC.Kolona.prva);
      tbx_lookUpDrzavlj  .JAM_lui_NameTaker_JAM_Name = tbx_opisUpDrzavlj.JAM_Name;
      
      tbx_lookUpDrzavaRod.JAM_Set_LookUpTable(ZXC.luiListaDrzave, (int)ZXC.Kolona.prva);
      tbx_lookUpDrzavaRod.JAM_lui_NameTaker_JAM_Name = tbx_opisDrzavaRod.JAM_Name; 
      
      tbx_vrstaPutIsprave.JAM_Set_LookUpTable(ZXC.luiListaVrstaPutIsprave, (int)ZXC.Kolona.prva);
      tbx_vrstaPutIsprave.JAM_lui_NameTaker_JAM_Name = tbx_opisVrstaPutIsp.JAM_Name; 
      
      tbx_mjestoUlaska   .JAM_Set_LookUpTable(ZXC.luiListaGranPrijelaz, (int)ZXC.Kolona.prva);
      tbx_mjestoUlaska   .JAM_lui_NameTaker_JAM_Name = tbx_opisMjestoUlaska.JAM_Name; 
      
      tbx_vrstaGosta     .JAM_Set_LookUpTable(ZXC.luiListaVrstaGosta, (int)ZXC.Kolona.prva);
      tbx_vrstaGosta     .JAM_lui_NameTaker_JAM_Name = tbx_opisVrstaGosta.JAM_Name; 
      
      tbx_statusGosta    .JAM_Set_LookUpTable(ZXC.luiListaStatusGosta, (int)ZXC.Kolona.prva);
      tbx_statusGosta    .JAM_lui_NameTaker_JAM_Name   = tbx_opisStatusGosta.JAM_Name; 
      tbx_statusGosta    .JAM_lui_NumberTaker_JAM_Name = tbx_moneyBP.JAM_Name; 
      
      tbx_tipObjekta     .JAM_Set_LookUpTable(ZXC.luiListaTipObjekta, (int)ZXC.Kolona.prva);
      tbx_tipObjekta     .JAM_lui_NameTaker_JAM_Name    = tbx_opisTipObjekta.JAM_Name; 
      tbx_tipObjekta     .JAM_lui_IntegerTaker_JAM_Name = tbx_objektSifra.JAM_Name; 

      tbx_opisVrstaPutIsp  .JAM_ReadOnly = 
      tbx_opisMjestoUlaska .JAM_ReadOnly = 
      tbx_opisVrstaGosta   .JAM_ReadOnly = 
      tbx_opisStatusGosta  .JAM_ReadOnly = 
      tbx_opisTipObjekta   .JAM_ReadOnly = 
      tbx_objektSifra      .JAM_ReadOnly = 
      tbx_opisUpDrzavlj    .JAM_ReadOnly = 
      tbx_opisDrzavaRod    .JAM_ReadOnly = true;

      tbx_lookUpDrzavlj  .JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_lookUpDrzavaRod.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_vrstaGosta     .JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_statusGosta    .JAM_CharacterCasing = CharacterCasing.Upper;

   }

   private void InitializeHamper_DatOdjave(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);

      hamper.VvColWdt      = new int[] { ZXC.Q4un, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel  (0, 0, "Datum odjave:", ContentAlignment.MiddleRight);
      tbx_datumOdjave = hamper.CreateVvTextBox(1, 0, "tbx_datumOdjave"     , "datumOdjave     ");
      tbx_datumOdjave.JAM_IsForDateTimePicker = true;
      dtp_datumOdjave = hamper.CreateVvDateTimePicker(1, 0, "", tbx_datumOdjave);
      dtp_datumOdjave.Name = "dtp_datumOdjave";

      //tbx_datumOdjave.JAM_ForeColor = Color.DarkBlue;
      //tbx_datumOdjave.JAM_Highlighted = true;
   }
  
   public void InitializeHamper_KnjigaGostiju(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0        1          2         3         4        5         6          7   
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q3un, ZXC.Q6un, ZXC.Q4un, ZXC.Q5un, ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun2, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                        hamper.CreateVvLabel(0, 0, "Ime:"           , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel(2, 0, "Prezime:"       , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel(4, 0, "Mjesto rođenja:", ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel(6, 0, "Datum rođenja:" , ContentAlignment.MiddleRight);
                        hamper.CreateVvLabel(0, 1, "Adresa:"        , ContentAlignment.MiddleRight);

      tbx_ime         = hamper.CreateVvTextBox(1, 0, "tbx_ime"      , "Ime ");
      tbx_prezime     = hamper.CreateVvTextBox(3, 0, "tbx_prezime"  , "Prezime ");
      tbx_mjestoRod   = hamper.CreateVvTextBox(5, 0, "tbx_mjestoRod", "Mjesto rođenja");
      tbx_datumRod    = hamper.CreateVvTextBox(7, 0, "tbx_datumRod" , "Datum rođenja");
      tbx_datumRod.JAM_IsForDateTimePicker = true;
      dtp_datumRod    = hamper.CreateVvDateTimePicker(7, 0, "", tbx_datumRod);
      dtp_datumRod.Name = "dtp_datumRod";
      tbx_adresaSt    = hamper.CreateVvTextBox(1, 1, "tbx_adresaStanovanja", "adresaStanovanja",200, 6, 0);

      tbx_ime    .JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_prezime.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_prezime.JAM_Highlighted     = true;

      this.ControlForInitialFocus = tbx_ime;
   }

 
   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn(0, "Bilješke", "Bilješke", 128, null);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public string Fld_ime                 { get { return tbx_ime                .Text; } set { tbx_ime                .Text = value; } }
   public string Fld_prezime             { get { return tbx_prezime            .Text; } set { tbx_prezime            .Text = value; } }
   public string Fld_mjestoRod           { get { return tbx_mjestoRod          .Text; } set { tbx_mjestoRod          .Text = value; } }
   public string Fld_lookUpDrzavlj       { get { return tbx_lookUpDrzavlj      .Text; } set { tbx_lookUpDrzavlj      .Text = value; } }
   public string Fld_opisUpDrzavlj       { get { return tbx_opisUpDrzavlj      .Text; } set { tbx_opisUpDrzavlj      .Text = value; } }
   public string Fld_lookUpDrzavaRod     { get { return tbx_lookUpDrzavaRod    .Text; } set { tbx_lookUpDrzavaRod    .Text = value; } }
   public string Fld_opisDrzavaRod       { get { return tbx_opisDrzavaRod      .Text; } set { tbx_opisDrzavaRod      .Text = value; } }
   public string Fld_vrstaPutIsprave     { get { return tbx_vrstaPutIsprave    .Text; } set { tbx_vrstaPutIsprave    .Text = value; } }
   public string Fld_mjestoUlaska        { get { return tbx_mjestoUlaska       .Text; } set { tbx_mjestoUlaska       .Text = value; } }
   public string Fld_vrstaGosta          { get { return tbx_vrstaGosta         .Text; } set { tbx_vrstaGosta         .Text = value; } }
   public string Fld_statusGosta         { get { return tbx_statusGosta        .Text; } set { tbx_statusGosta        .Text = value; } }
   public string Fld_tipObjekta          { get { return tbx_tipObjekta         .Text; } set { tbx_tipObjekta         .Text = value; } }
   public string Fld_opisVrstaPutIsprave { get { return tbx_opisVrstaPutIsp    .Text; } set { tbx_opisVrstaPutIsp    .Text = value; } }
   public string Fld_opisMjestoUlaska    { get { return tbx_opisMjestoUlaska   .Text; } set { tbx_opisMjestoUlaska   .Text = value; } }
   public string Fld_opisVrstaGosta      { get { return tbx_opisVrstaGosta     .Text; } set { tbx_opisVrstaGosta     .Text = value; } }
   public string Fld_opisStatusGosta     { get { return tbx_opisStatusGosta    .Text; } set { tbx_opisStatusGosta    .Text = value; } }
   public string Fld_opisTipObjekta      { get { return tbx_opisTipObjekta     .Text; } set { tbx_opisTipObjekta     .Text = value; } }
   public string Fld_brojPutIspr         { get { return tbx_brojPutIspr        .Text; } set { tbx_brojPutIspr        .Text = value; } }
   public string Fld_brojSobe            { get { return tbx_brojSobe           .Text; } set { tbx_brojSobe           .Text = value; } }
   public string Fld_adresadrStanov      { get { return tbx_adresaSt           .Text; } set { tbx_adresaSt           .Text = value; } }
   public string Fld_OpisUpDrzavlj       { get { return tbx_opisUpDrzavlj      .Text; } set { tbx_opisUpDrzavlj      .Text = value; } }
   public string Fld_OpisDrzavaRod       { get { return tbx_opisDrzavaRod      .Text; } set { tbx_opisDrzavaRod      .Text = value; } }

   public DateTime Fld_DateUlsaka        { get { return dtp_datumUlaska .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_datumUlaska .Value = value; }}}
   public DateTime Fld_DateOdjave        { get { return dtp_datumOdjave .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_datumOdjave .Value = value; }}}
   public DateTime Fld_DateRod           { get { return dtp_datumRod    .Value; } set { if(value >= DateTimePicker.MinimumDateTime && value <= DateTimePicker.MaximumDateTime) { dtp_datumRod    .Value = value; }}}
                                         
   public int Fld_ObjektSifra            { get { return tbx_objektSifra.GetIntField(); } set { tbx_objektSifra.PutIntField(value); } }

   public bool Fld_IsEU                  { get { return cbx_isEU.Checked; } set { cbx_isEU.Checked = value; } }

   public decimal Fld_IznosBP            { get { return tbx_moneyBP.GetDecimalField(); } set { tbx_moneyBP.PutDecimalField(value); } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()
   
   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_ime                 )) Fld_ime              = mixer_rec.StrA_40  ;        
      if(CtrlOK(tbx_prezime             )) Fld_prezime          = mixer_rec.StrF_64  ;
      if(CtrlOK(tbx_mjestoRod           )) Fld_mjestoRod        = mixer_rec.StrH_32 ;
      if(CtrlOK(tbx_lookUpDrzavlj       )) Fld_lookUpDrzavlj    = mixer_rec.Konto    ;
      if(CtrlOK(tbx_lookUpDrzavaRod     )) Fld_lookUpDrzavaRod  = mixer_rec.Konto2   ;
      if(CtrlOK(tbx_vrstaPutIsprave     )) Fld_vrstaPutIsprave  = mixer_rec.V1_tt    ;
      if(CtrlOK(tbx_mjestoUlaska        )) Fld_mjestoUlaska     = mixer_rec.V2_tt    ;
      if(CtrlOK(tbx_vrstaGosta          )) Fld_vrstaGosta       = mixer_rec.StrC_32  ;
      if(CtrlOK(tbx_statusGosta         )) Fld_statusGosta      = mixer_rec.StrD_32  ;
      if(CtrlOK(tbx_tipObjekta          )) Fld_tipObjekta       = mixer_rec.KupdobTK ;
      if(CtrlOK(tbx_brojPutIspr         )) Fld_brojPutIspr      = mixer_rec.StrB_128 ;
      if(CtrlOK(tbx_brojSobe            )) Fld_brojSobe         = mixer_rec.MtrosTK;
      if(CtrlOK(tbx_adresaSt            )) Fld_adresadrStanov   = mixer_rec.StrE_256 ;

      if(CtrlOK(tbx_datumUlaska         )) Fld_DateUlsaka       = mixer_rec.DateTimeA  ;
      if(CtrlOK(tbx_datumOdjave         )) Fld_DateOdjave       = mixer_rec.DateTimeB  ;
      if(CtrlOK(tbx_datumRod            )) Fld_DateRod          = mixer_rec.DateB  ;
     
      if(CtrlOK(tbx_objektSifra         )) Fld_ObjektSifra      = mixer_rec.IntA  ;
      if(CtrlOK(cbx_isEU                )) Fld_IsEU             = mixer_rec.IsXxx;

      if(CtrlOK(tbx_moneyBP             )) Fld_IznosBP          = mixer_rec.MoneyA;

      Fld_opisUpDrzavlj       = ZXC.luiListaDrzave         .GetNameForThisCd(mixer_rec.Konto);
      Fld_opisDrzavaRod       = ZXC.luiListaDrzave         .GetNameForThisCd(mixer_rec.Konto2);
      Fld_opisVrstaPutIsprave = ZXC.luiListaVrstaPutIsprave.GetNameForThisCd(mixer_rec.V1_tt);
      Fld_opisMjestoUlaska    = ZXC.luiListaGranPrijelaz   .GetNameForThisCd(mixer_rec.V2_tt);
      Fld_opisVrstaGosta      = ZXC.luiListaVrstaGosta     .GetNameForThisCd(mixer_rec.StrC_32);
      Fld_opisStatusGosta     = ZXC.luiListaStatusGosta    .GetNameForThisCd(mixer_rec.StrD_32);
      Fld_opisTipObjekta      = ZXC.luiListaTipObjekta     .GetNameForThisCd(mixer_rec.KupdobTK);


   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_ime                 ))  mixer_rec.StrA_40   = Fld_ime              ;        
      if(CtrlOK(tbx_prezime             ))  mixer_rec.StrF_64   = Fld_prezime          ;
      if(CtrlOK(tbx_mjestoRod           ))  mixer_rec.StrH_32   = Fld_mjestoRod        ;
      if(CtrlOK(tbx_lookUpDrzavlj       ))  mixer_rec.Konto     = Fld_lookUpDrzavlj    ;
      if(CtrlOK(tbx_lookUpDrzavaRod     ))  mixer_rec.Konto2    = Fld_lookUpDrzavaRod  ;
      if(CtrlOK(tbx_vrstaPutIsprave     ))  mixer_rec.V1_tt     = Fld_vrstaPutIsprave  ;
      if(CtrlOK(tbx_mjestoUlaska        ))  mixer_rec.V2_tt     = Fld_mjestoUlaska     ;
      if(CtrlOK(tbx_vrstaGosta          ))  mixer_rec.StrC_32   = Fld_vrstaGosta       ;
      if(CtrlOK(tbx_statusGosta         ))  mixer_rec.StrD_32   = Fld_statusGosta      ;
      if(CtrlOK(tbx_tipObjekta          ))  mixer_rec.KupdobTK  = Fld_tipObjekta       ;
      if(CtrlOK(tbx_brojPutIspr         ))  mixer_rec.StrB_128  = Fld_brojPutIspr      ;
      if(CtrlOK(tbx_brojSobe            ))  mixer_rec.MtrosTK   = Fld_brojSobe         ;
      if(CtrlOK(tbx_adresaSt            ))  mixer_rec.StrE_256  = Fld_adresadrStanov   ;

      if(CtrlOK(tbx_objektSifra         ))  mixer_rec.IntA      = Fld_ObjektSifra      ;
      if(CtrlOK(cbx_isEU                ))  mixer_rec.IsXxx     = Fld_IsEU             ;
                                                    
      if(CtrlOK(tbx_datumUlaska         ))  mixer_rec.DateTimeA = Fld_DateUlsaka       ;
      if(CtrlOK(tbx_datumOdjave         ))  mixer_rec.DateTimeB = Fld_DateOdjave       ;
      if(CtrlOK(tbx_datumRod            ))  mixer_rec.DateB     = Fld_DateRod          ;
       
      if(CtrlOK(tbx_moneyBP             ))  mixer_rec.MoneyA    = Fld_IznosBP          ;
                                                
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord


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
      get { return " Bilješke "; }
   }
   //public override string TabPageTitle2
   //{
   //   get { return "Obračun ostalih troškova"; }
   //}
   
   #endregion overrideTabPageTitle

}
