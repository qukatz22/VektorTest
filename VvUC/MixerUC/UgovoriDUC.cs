using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

public partial class UgovoriDUC : MixerDUC
{
   #region Fieldz

   private VvTextBox tbx_kupDobCd, tbx_kupDobTk, tbx_kupDobName,
                     tbx_strA_40 , /* Klasa    */
                     tbx_strC_32 , /* CuvanjeUg */
                     tbx_strD_32 , /* UrBr      */
                     tbx_strE_256, /* Predmet  */
                     tbx_strF_64 , /* OrigBrUg  */
                     tbx_strG_40 , /* TrajanjeUg*/
                     tbx_moneyA  , /* IznosUg   NETO*/
                     tbx_moneyB  , /* IznosUg   BRUTO*/
                     tbx_strH_32   /* VrstaUg   */
                     ;
   public VvHamper hamp_Ugovori, hamp_partner, hamp_veze, hamp_isAktivan;
   
   private CheckBox cbx_isAktivan;
 
   #endregion Fieldz

   #region Constructor

   public UgovoriDUC(Control parent, Mixer _mixer, VvForm.VvSubModul vvSubModul)
      : base(parent, _mixer, vvSubModul)
   {

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor
         (Mixer.tt_colName, new string[] 
         { 
            Mixer.TT_RUG
         });

   }

   #endregion Constructor

   #region CreateSpecificHampers()

   protected override void CreateSpecificHampers()
   {

      hamp_tt.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);
      hamp_dokDate.Location = new Point(ZXC.QunMrgn, hamp_tt.Bottom + ZXC.Qun4);

      InitializeHamper_IsAktivan(out hamp_isAktivan);

      nextX = hamp_dokDate.Right;
      nextY = hamp_dokDate.Top;

      InitializeHamper_Partner(out hamp_partner);

      hamp_dokNum.Location = new Point(hamp_partner.Right - hamp_dokNum.Width, ZXC.QunMrgn);

      nextX = ZXC.QunMrgn;
      nextY = hamp_partner.Bottom;

      InitializeHamper_Ugovori(out hamp_Ugovori);

      nextY = hamp_Ugovori.Bottom;
      InitializeHamper_Veze(out hamp_veze);


      hamp_napomena.Location = new Point(nextX, hamp_veze.Bottom);
      nextY = hamp_napomena.Bottom;

   }

   public void InitializeHamper_Ugovori(out VvHamper hamper)
   {
      hamper = new VvHamper(8, 5, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0         1        2          3         4         5         6          7     
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q4un, ZXC.Q4un, ZXC.Q4un, ZXC.Q3un, ZXC.Q5un , ZXC.Q4un, ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 , ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      string textDat = (ZXC.IsZASTITARI) ? "Datum raskida:" : "Datum ugovora:";
      
                      hamper.CreateVvLabel  (0, 0,                textDat       , ContentAlignment.MiddleRight);
      tbx_dateA     = hamper.CreateVvTextBox(1, 0, "tbx_DatNast", textDat, GetDB_ColumnSize(DB_ci.dateA));
      tbx_dateA.JAM_IsForDateTimePicker = true;
      dtp_dateA                    = hamper.CreateVvDateTimePicker(1, 0, "", tbx_dateA);
      dtp_dateA.Name = "dtp_dateA";

                    hamper.CreateVvLabel  (2, 0, "Klasa:", ContentAlignment.MiddleRight);
      tbx_strA_40 = hamper.CreateVvTextBox(3, 0, "tbx_klasOznRO", "tbx_klasOznRO", GetDB_ColumnSize(DB_ci.strA_40), 1, 0);
    //tbx_strA_40.JAM_ReadOnly = true;
         
                      hamper.CreateVvLabel  (5, 0,               "UrBroj:", ContentAlignment.MiddleRight);
      tbx_strD_32   = hamper.CreateVvTextBox(6, 0, "tbx_UrBrAl", "Urudžbeni broj",  GetDB_ColumnSize(DB_ci.strD_32), 1, 0);
    //tbx_strD_32.JAM_ReadOnly = true;

 
                     hamper.CreateVvLabel  (0, 1,                 "Predmet:", ContentAlignment.MiddleRight);
      tbx_strE_256 = hamper.CreateVvTextBox(1, 1, "tbx_strE_256", "Predmet" , GetDB_ColumnSize(DB_ci.strE_256), 6, 1);
      tbx_strE_256.Multiline = true;
      tbx_strE_256.ScrollBars = ScrollBars.Vertical;

                     hamper.CreateVvLabel  (0, 3,               "Iznos Neto:", ContentAlignment.MiddleRight);
      tbx_moneyA   = hamper.CreateVvTextBox(1, 3, "tbx_moneyA", "Iznos ", GetDB_ColumnSize(DB_ci.moneyA));
      tbx_moneyA.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
 
                     hamper.CreateVvLabel  (2, 3,               "Iznos Bruto:", ContentAlignment.MiddleRight);
      tbx_moneyB   = hamper.CreateVvTextBox(3, 3, "tbx_moneyB", "Iznos ", GetDB_ColumnSize(DB_ci.moneyB));
      tbx_moneyB.JAM_MarkAsNumericTextBox(2, true, decimal.MaxValue, decimal.MinValue, true);
           
                     hamper.CreateVvLabel  (4, 3,                "Trajanje ug.:", ContentAlignment.MiddleRight);
      tbx_strG_40  = hamper.CreateVvTextBox(5, 3, "tbx_UstrJed", "Trajanje ugovora ", GetDB_ColumnSize(DB_ci.strG_40));

      string textC32 = (ZXC.IsZASTITARI) ? "Broj osoba:" : "Rok čuvanja:";

                     hamper.CreateVvLabel  (6, 3,                textC32, ContentAlignment.MiddleRight);
      tbx_strC_32  = hamper.CreateVvTextBox(7, 3, "tbx_strC_32", textC32, GetDB_ColumnSize(DB_ci.strC_32));

                     hamper.CreateVvLabel  (0, 4,                "BrojUg:", ContentAlignment.MiddleRight);
      tbx_strF_64  = hamper.CreateVvTextBox(1, 4, "tbx_strF_64", "Originalni broj ugovora" , GetDB_ColumnSize(DB_ci.strF_64),2,0);

                     hamper.CreateVvLabel        (5, 4,                 ZXC.IsZASTITARI ? "Vrsta štićenja:" : "Vrsta:", ContentAlignment.MiddleRight);
      tbx_strH_32  = hamper.CreateVvTextBoxLookUp("tbx_strH_32", 6, 4,  ZXC.IsZASTITARI ? "Vrsta štićenja" : "Vrsta ugovora" , GetDB_ColumnSize(DB_ci.strH_32), 1, 0);
      tbx_strH_32.JAM_Set_LookUpTable(ZXC.luiListaVrstaUgovora, (int)ZXC.Kolona.prva);

   }
  
   private void InitializeHamper_Partner(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q10un + ZXC.Q3un + ZXC.Qun4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4            };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

                       hamper.CreateVvLabel  (0, 0, "Ugovorna strana:", ContentAlignment.MiddleRight);
      tbx_kupDobCd   = hamper.CreateVvTextBox(1, 0, "tbx_kupDobCd"  , "Sifra Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobCD)  );
      tbx_kupDobTk   = hamper.CreateVvTextBox(2, 0, "tbx_kupDobTk"  , "Tiker Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobTK)  );
      tbx_kupDobName = hamper.CreateVvTextBox(3, 0, "tbx_kupDobName", "Naziv Partnera: Preporučeni dobavljač, Investitor, Kooperant", GetDB_ColumnSize(DB_ci.kupdobName));

      tbx_kupDobCd.JAM_CharEdits       = ZXC.JAM_CharEdits.DigitsOnly;
      tbx_kupDobTk.JAM_CharacterCasing = CharacterCasing.Upper;

      tbx_kupDobCd  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterKCD.SortType   , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobTk  .JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterTicker.SortType, new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
      tbx_kupDobName.JAM_SetAutoCompleteData(Kupdob.recordName, Kupdob.sorterNaziv.SortType , new EventHandler(OnVvTBEnter_SetAutocmplt_Kupdob_sorterName), new EventHandler(AnyKupdobTextBoxLeave));

      this.ControlForInitialFocus = tbx_kupDobName;

   }

   private void InitializeHamper_Veze(out VvHamper hamper)
   {
      hamper = new VvHamper(11, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, razmakHamp);
      //                                     0               1                             2                  3            4                   5                   6              7        8            9           10       
      hamper.VvColWdt      = new int[] { ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.Q2un, ZXC.Q7un+ ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.QUN - ZXC.Qun4, ZXC.Q2un + ZXC.Qun8, ZXC.Q3un- ZXC.Qun4 , ZXC.Q6un, ZXC.Q3un - ZXC.Qun4, ZXC.QUN - ZXC.Qun4 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,            ZXC.Qun4, ZXC.Qun4,           ZXC.Qun4,                  0,                  0,            ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4,                  0 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
                        hamper.CreateVvLabel  (0, 0, "Link1:", ContentAlignment.MiddleRight);
      tbx_externLink1 = hamper.CreateVvTextBox(1, 0, "tbx_externLink1", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink1), 2, 0);
      btn_goExLink1   = hamper.CreateVvButton (4, 0, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink1.Name = "btn_goExLink1";
      btn_goExLink1.FlatStyle = FlatStyle.Flat;
      btn_goExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink1.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink1.Tag = 1;
      btn_goExLink1.TabStop = false;
      btn_goExLink1.Visible = false;

      btn_openExLink1 = hamper.CreateVvButton(5, 0, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink1.Name = "btn_openExLink1";
      btn_openExLink1.FlatStyle = FlatStyle.Flat;
      btn_openExLink1.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink1.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink1.Tag = 1;
      btn_openExLink1.TabStop = false;

                        hamper.CreateVvLabel  (0, 1, "Link2:", ContentAlignment.MiddleRight);
      tbx_externLink2 = hamper.CreateVvTextBox(1, 1, "tbx_externLink2", "Link na externi dokument, npr. Word, Excel... ", GetDB_ColumnSize(DB_ci.externLink2), 2, 0);
      btn_goExLink2   = hamper.CreateVvButton (4, 1, new EventHandler(Link_ExternDokument_Click), "");
      btn_goExLink2.Name = "btn_goExLink2";
      btn_goExLink2.FlatStyle = FlatStyle.Flat;
      btn_goExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_goExLink2.Image = VvIco.ExLinkLeft16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico")), 16, 16)*/.ToBitmap();
      btn_goExLink2.Tag = 2;
      btn_goExLink2.TabStop = false;
      btn_goExLink2.Visible = false;

      btn_openExLink2 = hamper.CreateVvButton(5, 1, new EventHandler(Show_ExternDokument_Click), "");
      btn_openExLink2.Name = "btn_openExLink2";
      btn_openExLink2.FlatStyle = FlatStyle.Flat;
      btn_openExLink2.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_openExLink2.Image = VvIco.LinkRight16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico")), 16, 16)*/.ToBitmap();
      btn_openExLink2.Tag = 2;
      btn_openExLink2.TabStop = false;

                      hamper.CreateVvLabel        ( 6, 0, "Veza1:", ContentAlignment.MiddleRight);
      tbx_v1_tt     = hamper.CreateVvTextBoxLookUp( 7, 0, "tbx_v1_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v1_tt));
      tbx_v1_ttOpis = hamper.CreateVvTextBox      ( 8, 0, "tbx_v1_ttOpis", "Veza na interni dokument", 32);
      tbx_v1_ttNum  = hamper.CreateVvTextBox      ( 9, 0, "tbx_v1_ttNum", "Veza na interni dokument", 6/*GetDB_ColumnSize(DB_ci.iz_ttNum)*/);
      btn_v1TT      = hamper.CreateVvButton       (10, 0, new EventHandler(GoTo_MIXER_Dokument_Click), "");

      btn_v1TT.Name = "v1_TT";
      btn_v1TT.FlatStyle = FlatStyle.Flat;
      btn_v1TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;
      btn_v1TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v1TT.Tag = 1;
      btn_v1TT.TabStop = false;

      tbx_v1_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v1_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v1_tt.JAM_lui_NameTaker_JAM_Name = tbx_v1_ttOpis.JAM_Name;
      tbx_v1_ttOpis.JAM_ReadOnly = true;
      tbx_v1_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;

                      hamper.CreateVvLabel        ( 6, 1, "Veza2:", ContentAlignment.MiddleRight);
      tbx_v2_tt     = hamper.CreateVvTextBoxLookUp( 7, 1, "tbx_v2_tt", "Veza na interni dokument", GetDB_ColumnSize(DB_ci.v2_tt));
      tbx_v2_ttOpis = hamper.CreateVvTextBox      ( 8, 1, "tbx_v2_ttOpis", "Veza na interni dokument");
      tbx_v2_ttNum  = hamper.CreateVvTextBox      ( 9, 1, "tbx_v2_ttNum", "Veza na interni dokument" + " broj", 6/*GetDB_ColumnSize(DB_ci.na_ttNum)*/);
      btn_v2TT      = hamper.CreateVvButton       (10, 1, new EventHandler(GoTo_MIXER_Dokument_Click), "");
      btn_v2TT.Name = "v2_TT";

      btn_v2TT.FlatStyle = FlatStyle.Flat;
      btn_v2TT.FlatAppearance.BorderColor = ZXC.vvColors.userControl_BackColor;

      btn_v2TT.Image = VvIco.TriangleBlue16/*new Icon((ZXC.TheVvForm.GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico")), 16, 16)*/.ToBitmap();
      btn_v2TT.Tag = 2;
      btn_v2TT.TabStop = false;

      tbx_v2_tt.JAM_CharacterCasing = CharacterCasing.Upper;
      tbx_v2_tt.JAM_Set_NOTobligatory_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);
      tbx_v2_tt.JAM_lui_NameTaker_JAM_Name = tbx_v2_ttOpis.JAM_Name;
      tbx_v2_ttOpis.JAM_ReadOnly = true;

      tbx_v2_ttNum.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
   }

   private void InitializeHamper_IsAktivan(out VvHamper hamper)
   {
      hamper = new VvHamper(1, 1, "", TheTabControl.TabPages[0], false, hamp_tt.Right, hamp_tt.Top, razmakHamp);
      //                                     0                 1                
      hamper.VvColWdt      = new int[] { ZXC.Q6un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_isAktivan = hamper.CreateVvCheckBox_OLD(0, 0, new EventHandler(cbx_isAktivan_CheckedChanged), "Neaktivan", RightToLeft.No);
      cbx_isAktivan.CheckedChanged += new EventHandler(cbx_isAktivan_CheckedChanged);
   }

   private void cbx_isAktivan_CheckedChanged(object sender, EventArgs e)
   {
      CheckBox cbx = sender as CheckBox;

      if(cbx.Checked)  cbx_isAktivan.Text = "Aktivan"  ;
      else             cbx_isAktivan.Text = "Neaktivan";
      
   }


   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(isPopulatingSifrar) return;

      //if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Kupdob kupdob_rec;

      if(tb.Text != this.originalText)
      {
         originalText = tb.Text;
         kupdob_rec = VvUserControl.KupdobSifrar.Find(FoundInSifrar<Kupdob>);

         if(kupdob_rec != null && tb.Text != "")
         {
            Fld_KupDobName = kupdob_rec.Naziv;
            Fld_KupDobCd = kupdob_rec.KupdobCD/*RecID*/;
            Fld_KupDobTk = kupdob_rec.Ticker;
         }
         else
         {
            Fld_KupDobName = Fld_KupDobTk = Fld_KupDobCdAsTxt = "";
         }
      }
   }

   #endregion CreateSpecificHampers()

   #region InitializeTheGrid_Columns()

   protected override void InitializeDUC_Specific_Columns()
   {
      T_opis_128_CreateColumn(0, "Komentar", "Komentar", 128, null);
   }

   #endregion InitializeTheGrid_Columns()

   #region Fld_

   public uint    Fld_KupDobCd      { get { return tbx_kupDobCd.GetSomeRecIDField(); } set { tbx_kupDobCd.PutSomeRecIDField(value); } }
   public string  Fld_KupDobCdAsTxt { get { return tbx_kupDobCd.Text;                } set { tbx_kupDobCd  .Text = value;           } }
   public string  Fld_KupDobName    { get { return tbx_kupDobName.Text;              } set { tbx_kupDobName.Text = value;           } }
   public string  Fld_KupDobTk      { get { return tbx_kupDobTk.Text;                } set { tbx_kupDobTk  .Text = value;           } }
                  
   public string  Fld_StrA_40       { get { return tbx_strA_40 .Text;                } set { tbx_strA_40  .Text = value;            } }
   public string  Fld_StrC_32       { get { return tbx_strC_32 .Text;                } set { tbx_strC_32  .Text = value;            } }
   public string  Fld_StrD_32       { get { return tbx_strD_32 .Text;                } set { tbx_strD_32  .Text = value;            } }
   public string  Fld_StrE_256      { get { return tbx_strE_256.Text;                } set { tbx_strE_256 .Text = value;            } }
   public string  Fld_StrF_64       { get { return tbx_strF_64 .Text;                } set { tbx_strF_64  .Text = value;            } }
   public string  Fld_StrG_40       { get { return tbx_strG_40 .Text;                } set { tbx_strG_40  .Text = value;            } }
   public string  Fld_StrH_32       { get { return tbx_strH_32 .Text;                } set { tbx_strH_32  .Text = value;            } }
   public decimal Fld_MoneyA        { get { return tbx_moneyA  .GetDecimalField();   } set { tbx_moneyA   .PutDecimalField(value);  } } // Iznos ugovora Neto 
   public decimal Fld_MoneyB        { get { return tbx_moneyB  .GetDecimalField();   } set { tbx_moneyB   .PutDecimalField(value);  } } // Iznos ugovora Bruto

   public bool Fld_IsAktivan        { get { return cbx_isAktivan.Checked           ; } set { cbx_isAktivan.Checked = value       ;  } }

   #endregion _Fld

   #region override PutSpecificsFld() GetSpecificsFld()

   /*protected*/public override void PutSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd  )) Fld_KupDobCd   = mixer_rec.KupdobCD;
      if(CtrlOK(tbx_kupDobName)) Fld_KupDobName = mixer_rec.KupdobName;
      if(CtrlOK(tbx_kupDobTk  )) Fld_KupDobTk   = mixer_rec.KupdobTK;
      if(CtrlOK(tbx_ProjektCD )) Fld_ProjektCD  = mixer_rec.ProjektCD;
      Fld_TtOpis = "REGISTAR UGOVORA";

      if(CtrlOK(tbx_v1_tt      )) Fld_V1_tt       = mixer_rec.V1_tt;
      if(CtrlOK(tbx_v1_ttOpis  )) Fld_V1_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V1_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v1_ttNum   )) Fld_V1_ttNum    = mixer_rec.V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) Fld_V2_tt       = mixer_rec.V2_tt;
      if(CtrlOK(tbx_v2_ttOpis  )) Fld_V2_ttOpis   = ZXC.GetNameForThisCdFromManyLuiLists(mixer_rec.V2_tt, ZXC.luiListaFakturType, ZXC.luiListaMixerType);
      if(CtrlOK(tbx_v2_ttNum   )) Fld_V2_ttNum    = mixer_rec.V2_ttNum;
      if(CtrlOK(tbx_externLink1)) Fld_ExternLink1 = mixer_rec.ExternLink1;
      if(CtrlOK(tbx_externLink2)) Fld_ExternLink2 = mixer_rec.ExternLink2;

      if(CtrlOK(tbx_strA_40 )) Fld_StrA_40   = mixer_rec.StrA_40 ;
      if(CtrlOK(tbx_strC_32 )) Fld_StrC_32   = mixer_rec.StrC_32 ;
      if(CtrlOK(tbx_strD_32 )) Fld_StrD_32   = mixer_rec.StrD_32 ;
      if(CtrlOK(tbx_strE_256)) Fld_StrE_256  = mixer_rec.StrE_256;
      if(CtrlOK(tbx_strF_64 )) Fld_StrF_64   = mixer_rec.StrF_64 ;
      if(CtrlOK(tbx_strG_40 )) Fld_StrG_40   = mixer_rec.StrG_40 ;
      if(CtrlOK(tbx_strH_32 )) Fld_StrH_32   = mixer_rec.StrH_32 ;
      if(CtrlOK(tbx_moneyA  )) Fld_MoneyA    = mixer_rec.MoneyA  ;
      if(CtrlOK(tbx_moneyB  )) Fld_MoneyB    = mixer_rec.MoneyB  ;

                               Fld_IsAktivan = mixer_rec.IsXxx;
   }

   protected override void GetSpecificsFld()
   {
      if(CtrlOK(tbx_kupDobCd   )) mixer_rec.KupdobCD    = Fld_KupDobCd;
      if(CtrlOK(tbx_kupDobName )) mixer_rec.KupdobName  = Fld_KupDobName;
      if(CtrlOK(tbx_kupDobTk   )) mixer_rec.KupdobTK    = Fld_KupDobTk;
      if(CtrlOK(tbx_ProjektCD  )) mixer_rec.ProjektCD   = Fld_ProjektCD;
      if(CtrlOK(tbx_v1_tt      )) mixer_rec.V1_tt       = Fld_V1_tt;
      if(CtrlOK(tbx_v1_ttNum   )) mixer_rec.V1_ttNum    = Fld_V1_ttNum;
      if(CtrlOK(tbx_v2_tt      )) mixer_rec.V2_tt       = Fld_V2_tt;
      if(CtrlOK(tbx_v2_ttNum   )) mixer_rec.V2_ttNum    = Fld_V2_ttNum;
      if(CtrlOK(tbx_externLink1)) mixer_rec.ExternLink1 = Fld_ExternLink1;
      if(CtrlOK(tbx_externLink2)) mixer_rec.ExternLink2 = Fld_ExternLink2;

      if(CtrlOK(tbx_strA_40    )) mixer_rec.StrA_40     = Fld_StrA_40   ;
      if(CtrlOK(tbx_strC_32    )) mixer_rec.StrC_32     = Fld_StrC_32   ;
      if(CtrlOK(tbx_strD_32    )) mixer_rec.StrD_32     = Fld_StrD_32   ;
      if(CtrlOK(tbx_strE_256   )) mixer_rec.StrE_256    = Fld_StrE_256  ;
      if(CtrlOK(tbx_strF_64    )) mixer_rec.StrF_64     = Fld_StrF_64   ;
      if(CtrlOK(tbx_strG_40    )) mixer_rec.StrG_40     = Fld_StrG_40   ;
      if(CtrlOK(tbx_strH_32    )) mixer_rec.StrH_32     = Fld_StrH_32   ;
      if(CtrlOK(tbx_moneyA     )) mixer_rec.MoneyA      = Fld_MoneyA    ;
      if(CtrlOK(tbx_moneyB     )) mixer_rec.MoneyB      = Fld_MoneyB    ;

                                  mixer_rec.IsXxx = Fld_IsAktivan ;

   }

   public void PutUgovoriFieldsFromUrudzbeniData(Mixer mixerUrudzbeni_rec)
   {
      Fld_KupDobCd   = mixerUrudzbeni_rec.KupdobCD  ;
      Fld_KupDobTk   = mixerUrudzbeni_rec.KupdobTK  ;
      Fld_KupDobName = mixerUrudzbeni_rec.KupdobName;
      Fld_StrA_40    = mixerUrudzbeni_rec.StrA_40   ; // klasa 
      Fld_StrD_32    = mixerUrudzbeni_rec.StrD_32   ; // urudzbeni broj 
   }

   #endregion override PutSpecificsFld() GetSpecificsFld()

   #region PrintDocumentRecord

   public UgovoriFilterUC TheUgovoriFilterUC { get; set; }
   public UgovoriFilter TheUgovoriFilter { get; set; }

   protected override void CreateMixerDokumentPrintUC()
   {
      this.TheUgovoriFilter = new UgovoriFilter(this);

      TheUgovoriFilterUC = new UgovoriFilterUC(this);
      TheUgovoriFilterUC.Parent = ThePanelForFilterUC_PrintTemplateUC;
      ThePanelForFilterUC_PrintTemplateUC.Width = TheUgovoriFilterUC.Width;

   }

   public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
   {
      //UgovoriFilter mixerFilter = (UgovoriFilter)vvRptFilter;
      EvidencijaFilter mixerFilter = (EvidencijaFilter)vvRptFilter;

      //switch(mixerFilter.PrintUgovori)
      switch(mixerFilter.PrintEvidencija)
      {
         //case UgovoriFilter.PrintUgovoriEnum.Ugovori: specificMixerReport = new RptX_Ugovori(new Vektor.Reports.XIZ.CR_UgovoriDUC(), "Ugovori", mixerFilter); break;
         case EvidencijaFilter.PrintEvidencijaEnum.Evidencija: specificMixerReport = new RptX_Evidencija(new Vektor.Reports.XIZ.CR_EvidencijaDUC(), "Ugovori", mixerFilter); break;

         default: ZXC.aim_emsg("{0}\nPrintSomeUgovoriDocumentrd <{1}> undone!", ZXC.GetMethodNameDaStack(), mixerFilter.PrintEvidencija); return null;
      }

      return specificMixerReport;
   }

   public override VvRptFilter VirtualRptFilter
   {
      get
      {
         return this.TheUgovoriFilter;
      }
   }

   public override VvFilterUC VirtualFilterUC
   {
      get
      {
         return this.TheUgovoriFilterUC;
      }
   }

   public override void SetFilterRecordDependentDefaults()
   {
   }


   #endregion PrintDocumentRecord

   #region Colors

   protected override void AddColorsToBaby()
   {
      SetUpColor(clr_RUG, Color.Empty, clr_RUG);
   }

   #endregion Colors

   #region overrideTabPageTitle

   public override string TabPageTitle1
   {
      get { return ""; }
   }

   #endregion overrideTabPageTitle

}

public class UgovoriFilterUC : VvFilterUC
{
   #region Fieldz

   #endregion Fieldz

   #region  Constructor

   public UgovoriFilterUC(VvUserControl vvUC)
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

   private UgovoriFilter TheUgovoriFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as UgovoriFilter; }
      set { this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheUgovoriFilter = (UgovoriFilter)_filter_data;

      if(TheUgovoriFilter != null)
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

public class UgovoriFilter : VvRpt_Mix_Filter
{

   public enum PrintUgovoriEnum
   {
      Ugovori
   }

   public PrintUgovoriEnum PrintUgovori { get; set; }

   public UgovoriDUC theDUC;

   public UgovoriFilter(UgovoriDUC _theDUC)
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
      PrintUgovori = PrintUgovoriEnum.Ugovori;
   }

   #endregion SetDefaultFilterValues()

}


