using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;

public class SkyRuleUC : VvRecordUC
{
   #region Fieldz

   private SkyRule   skyRule_rec;
   private VvTextBox tbx_record, tbx_documTT, tbx_opis;
   
   private RadioButton 
   /*ZXC.LanSrvKind     _birthLoc*/     rbt_birthLoc_NONE  , rbt_birthLoc_CENT        , rbt_birthLoc_SHOP    , rbt_birthLoc_SKY          ,
   /*ZXC.SkySklKind     _skl1kind*/     rbt_skl1kind_NONE  , rbt_skl1kind_CentGLSK    , rbt_skl1kind_CentPVSK, rbt_skl1kind_ShopVPSK     , rbt_skl1kind_ShopMPSK,
   /*ZXC.SkySklKind     _skl2kind*/     rbt_skl2kind_NONE  , rbt_skl2kind_CentGLSK    , rbt_skl2kind_CentPVSK, rbt_skl2kind_ShopVPSK     , rbt_skl2kind_ShopMPSK,
   /*ZXC.SkyOperation   _centOPS*/      rbt_centOPS_NONE   , rbt_centOPS_RECEIVE      , rbt_centOPS_SEND     , rbt_centOPS_SendAndReceive,
   /*ZXC.SkyOperation   _shopOPS*/      rbt_shopOPS_NONE   , rbt_shopOPS_RECEIVE      , rbt_shopOPS_SEND     , rbt_shopOPS_SendAndReceive,
   /*ZXC.SkyReceiveKind _shopRCVkind*/  rbt_shopRCVkind_NONE         , 
                                        rbt_shopRCVkind_EVERYTHING   , 
                                        rbt_shopRCVkind_OnlyLOCALskl1, 
                                        rbt_shopRCVkind_OnlyOTHERskl1, 
                                        rbt_shopRCVkind_OnlyLOCALskl2, 
                                        rbt_shopRCVkind_OnlyOTHERskl2;

   private CheckBox cbx_centCanADD, cbx_centCanRWT, cbx_centCanDEL,
                    cbx_shopCanADD, cbx_shopCanRWT, cbx_shopCanDEL,
                    cbx_NotBkgrndSND, cbx_NotSNDonExLd, cbx_NotRCVonLoad;

   private VvHamper hamp_record, hamp_birthLoc, hamp_skl1kind, hamp_skl2kind, hamp_centOPS, hamp_shopOPS, hamp_shopRCVkind, hamp_centShopCan, hamp_opis, hamp_disable;

   private SkyRuleDao.SkyRuleCI DB_ci
   {
      get { return ZXC.SkyCI; }
   }

   int nextX, nextY;

   #endregion Fieldz

   #region Constructor

   public SkyRuleUC(Control parent, SkyRule _skyRule, VvForm.VvSubModul vvSubModul)
   {
      TheTabControl.TabPages.Add(CreateVvInnerTabPages("Matični", "", ZXC.VvInnerTabPageKindEnum.ReadWrite_TabPage));

      nextX = ZXC.Qun8;
      nextY = ZXC.QunMrgn; 
      InitializeHamperSky_record      (out hamp_record     ); nextY = hamp_record     .Bottom;
      InitializeHamperSky_birthLoc    (out hamp_birthLoc   ); nextY = hamp_birthLoc   .Bottom;
      InitializeHamperSky_skl1kind    (out hamp_skl1kind   ); nextY = hamp_skl1kind   .Bottom;
      InitializeHamperSky_skl2kind    (out hamp_skl2kind   ); nextY = hamp_skl2kind   .Bottom;
      InitializeHamperSky_centOPS     (out hamp_centOPS    ); nextY = hamp_centOPS    .Bottom;
      InitializeHamperSky_shopOPS     (out hamp_shopOPS    ); nextY = hamp_shopOPS    .Bottom;
      InitializeHamperSky_shopRCVkind (out hamp_shopRCVkind); nextY = hamp_shopRCVkind.Bottom;
      InitializeHamperOnly_centShopCan(out hamp_centShopCan); nextY = hamp_centShopCan.Bottom;
      InitializeHamperOnly_opis       (out hamp_opis       ); nextY = hamp_opis       .Bottom;
      InitializeHamperSky_disable     (out hamp_disable    ); 

      InitializeVvUserControl(parent);
      skyRule_rec = _skyRule;

      ThePrefferedRecordSorter = VirtualDataRecord.DefaultSorter;

      this.TheSubModul = vvSubModul;
   }


   #endregion Constructor

   #region hamper - i

   private void InitializeHamperSky_record(out VvHamper hamper)
   {
      hamper          = new VvHamper(2, 2, "", TheTabControl.TabPages[0], false);
      hamper.Location = new Point(nextX, nextY);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0,  0, "Record:" , ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0,  1, "DocumTT:", ContentAlignment.MiddleRight);

      tbx_record  = hamper.CreateVvTextBox      (1, 0, "tbx_record" , "Record ", GetDB_ColumnSize(DB_ci.record ));
      tbx_documTT = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_documTT", "DocumTT", GetDB_ColumnSize(DB_ci.documTT));
      tbx_documTT.JAM_Set_LookUpTable(ZXC.luiListaFakturType, (int)ZXC.Kolona.prva);

      this.ControlForInitialFocus = tbx_record;
      

      // proba za drugoaciji sistem validacija. Ne po TextBox-u nego na kraju validirati cio UC. 

      //this.Validating += new System.ComponentModel.CancelEventHandler(SkyRuleUC_Validating);
   }

   private void InitializeHamperSky_birthLoc(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "BirthLoc:", ContentAlignment.MiddleRight);

      rbt_birthLoc_NONE = hamper.CreateVvRadioButton(1, 0, null, "NONE", TextImageRelation.ImageBeforeText);
      rbt_birthLoc_CENT = hamper.CreateVvRadioButton(2, 0, null, "CENT", TextImageRelation.ImageBeforeText);
      rbt_birthLoc_SHOP = hamper.CreateVvRadioButton(3, 0, null, "SHOP", TextImageRelation.ImageBeforeText);
      rbt_birthLoc_SKY  = hamper.CreateVvRadioButton(4, 0, null, "SKY" , TextImageRelation.ImageBeforeText);
      rbt_birthLoc_NONE.Checked = true;
      rbt_birthLoc_NONE.Tag = true;

   }

   private void InitializeHamperSky_skl1kind(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Skl1kind:", ContentAlignment.MiddleRight);

      rbt_skl1kind_NONE     = hamper.CreateVvRadioButton(1, 0, null, "NONE"    , TextImageRelation.ImageBeforeText);
      rbt_skl1kind_CentGLSK = hamper.CreateVvRadioButton(2, 0, null, "CentGLSK", TextImageRelation.ImageBeforeText);
      rbt_skl1kind_CentPVSK = hamper.CreateVvRadioButton(3, 0, null, "CentPVSK", TextImageRelation.ImageBeforeText);
      rbt_skl1kind_ShopVPSK = hamper.CreateVvRadioButton(4, 0, null, "ShopVPSK", TextImageRelation.ImageBeforeText);
      rbt_skl1kind_ShopMPSK = hamper.CreateVvRadioButton(5, 0, null, "ShopMPSK", TextImageRelation.ImageBeforeText);
      rbt_skl1kind_NONE.Checked = true;
      rbt_skl1kind_NONE.Tag = true;

   }

   private void InitializeHamperSky_skl2kind(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "Skl2kind:", ContentAlignment.MiddleRight);

      rbt_skl2kind_NONE     = hamper.CreateVvRadioButton(1, 0, null, "NONE"    , TextImageRelation.ImageBeforeText);
      rbt_skl2kind_CentGLSK = hamper.CreateVvRadioButton(2, 0, null, "CentGLSK", TextImageRelation.ImageBeforeText);
      rbt_skl2kind_CentPVSK = hamper.CreateVvRadioButton(3, 0, null, "CentPVSK", TextImageRelation.ImageBeforeText);
      rbt_skl2kind_ShopVPSK = hamper.CreateVvRadioButton(4, 0, null, "ShopVPSK", TextImageRelation.ImageBeforeText);
      rbt_skl2kind_ShopMPSK = hamper.CreateVvRadioButton(5, 0, null, "ShopMPSK", TextImageRelation.ImageBeforeText);
      rbt_skl2kind_NONE.Checked = true;
      rbt_skl2kind_NONE.Tag = true;

   }

   private void InitializeHamperSky_centOPS(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q6un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "CentOPS:", ContentAlignment.MiddleRight);

      rbt_centOPS_NONE           = hamper.CreateVvRadioButton(1, 0, null, "NONE"          , TextImageRelation.ImageBeforeText);
      rbt_centOPS_RECEIVE        = hamper.CreateVvRadioButton(2, 0, null, "RECEIVE"       , TextImageRelation.ImageBeforeText);
      rbt_centOPS_SEND           = hamper.CreateVvRadioButton(3, 0, null, "SEND"          , TextImageRelation.ImageBeforeText);
      rbt_centOPS_SendAndReceive = hamper.CreateVvRadioButton(4, 0, null, "SendAndReceive", TextImageRelation.ImageBeforeText);
      rbt_centOPS_NONE.Checked = true;
      rbt_centOPS_NONE.Tag = true;

   }

   private void InitializeHamperSky_shopOPS(out VvHamper hamper)
   {
      hamper = new VvHamper(6, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q6un, ZXC.Q5un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "ShopOPS:", ContentAlignment.MiddleRight);

      rbt_shopOPS_NONE           = hamper.CreateVvRadioButton(1, 0, null, "NONE"          , TextImageRelation.ImageBeforeText);
      rbt_shopOPS_RECEIVE        = hamper.CreateVvRadioButton(2, 0, null, "RECEIVE"       , TextImageRelation.ImageBeforeText);
      rbt_shopOPS_SEND           = hamper.CreateVvRadioButton(3, 0, null, "SEND"          , TextImageRelation.ImageBeforeText);
      rbt_shopOPS_SendAndReceive = hamper.CreateVvRadioButton(4, 0, null, "SendAndReceive", TextImageRelation.ImageBeforeText);
      rbt_shopOPS_NONE.Checked = true;
      rbt_shopOPS_NONE.Tag = true;

   }

   private void InitializeHamperSky_shopRCVkind(out VvHamper hamper)
   {
      hamper = new VvHamper(7, 1, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "ShopRCVkind:", ContentAlignment.MiddleRight);

      rbt_shopRCVkind_NONE          = hamper.CreateVvRadioButton(1, 0, null, "NONE"         , TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_EVERYTHING    = hamper.CreateVvRadioButton(2, 0, null, "EVERYTHING"   , TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_OnlyLOCALskl1 = hamper.CreateVvRadioButton(3, 0, null, "OnlyLOCALskl1", TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_OnlyOTHERskl1 = hamper.CreateVvRadioButton(4, 0, null, "OnlyOTHERskl1", TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_OnlyLOCALskl2 = hamper.CreateVvRadioButton(5, 0, null, "OnlyLOCALskl2", TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_OnlyOTHERskl2 = hamper.CreateVvRadioButton(6, 0, null, "OnlyOTHERskl2", TextImageRelation.ImageBeforeText);
      rbt_shopRCVkind_NONE.Checked = true;
      rbt_shopRCVkind_NONE.Tag = true;

   }

   private void InitializeHamperOnly_centShopCan(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", TheTabControl.TabPages[0], false, nextX, nextY, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q3un, ZXC.Q3un, ZXC.Q3un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4, ZXC.Qun4, ZXC.Qun4 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0, 0, "CentCan:", ContentAlignment.MiddleRight);
      hamper.CreateVvLabel(0, 1, "ShopCan:", ContentAlignment.MiddleRight);

      cbx_centCanADD = hamper.CreateVvCheckBox_OLD(1, 0, null, "ADD", RightToLeft.No);
      cbx_centCanRWT = hamper.CreateVvCheckBox_OLD(2, 0, null, "RWT", RightToLeft.No);
      cbx_centCanDEL = hamper.CreateVvCheckBox_OLD(3, 0, null, "DEL", RightToLeft.No);
      cbx_shopCanADD = hamper.CreateVvCheckBox_OLD(1, 1, null, "ADD", RightToLeft.No);
      cbx_shopCanRWT = hamper.CreateVvCheckBox_OLD(2, 1, null, "RWT", RightToLeft.No);
      cbx_shopCanDEL = hamper.CreateVvCheckBox_OLD(3, 1, null, "DEL", RightToLeft.No);



   }
   
   private void InitializeHamperOnly_opis(out VvHamper hamper)
   {
      hamper          = new VvHamper(2, 1, "", TheTabControl.TabPages[0], false);
      hamper.Location = new Point(nextX, nextY);

      hamper.VvColWdt      = new int[] { ZXC.Q5un, ZXC.Q10un * 4};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4};
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      hamper.CreateVvLabel(0,  0, "Opis:" , ContentAlignment.MiddleRight);

      tbx_opis = hamper.CreateVvTextBox(1, 0, "tbx_opis", "Opis", GetDB_ColumnSize(DB_ci.opis ));
   }

   private void InitializeHamperSky_disable(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 2, "", TheTabControl.TabPages[0], false, hamp_record.Right + ZXC.Q2un, ZXC.QunMrgn, ZXC.Qun10);

      hamper.VvColWdt      = new int[] { ZXC.Q10un + ZXC.Q4un, ZXC.Q10un + ZXC.Q4un};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4, ZXC.Qun4  };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      cbx_NotBkgrndSND = hamper.CreateVvCheckBox_OLD(0, 0, null, "Disable Automatic Background SEND"          , RightToLeft.No);
      cbx_NotSNDonExLd = hamper.CreateVvCheckBox_OLD(0, 1, null, "Disable Automatic SEND On Exit/Load Program", RightToLeft.No);
      cbx_NotRCVonLoad = hamper.CreateVvCheckBox_OLD(1, 1, null, "Disable Automatic RECEIVE On Load Program"  , RightToLeft.No);
   }

   #endregion hamper -i

   #region Fld_

   public string   Fld_Record  { get { return tbx_record .Text; } set { tbx_record .Text = value; } }
   public string   Fld_DocumTT { get { return tbx_documTT.Text; } set { tbx_documTT.Text = value; } }
   public string   Fld_Opis    { get { return tbx_opis   .Text; } set { tbx_opis   .Text = value; } }

   public ZXC.LanSrvKind Fld_BirthLoc
   {
      get
      {
              if(rbt_birthLoc_NONE.Checked) return ZXC.LanSrvKind.NONE;
         else if(rbt_birthLoc_CENT.Checked) return ZXC.LanSrvKind.CENT;
         else if(rbt_birthLoc_SHOP.Checked) return ZXC.LanSrvKind.SHOP;
         else if(rbt_birthLoc_SKY .Checked) return ZXC.LanSrvKind.SKY ;

              else throw new Exception("Fld_BirthLoc: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.LanSrvKind.NONE: rbt_birthLoc_NONE.Checked = true; break;
            case ZXC.LanSrvKind.CENT: rbt_birthLoc_CENT.Checked = true; break;
            case ZXC.LanSrvKind.SHOP: rbt_birthLoc_SHOP.Checked = true; break;
            case ZXC.LanSrvKind.SKY : rbt_birthLoc_SKY .Checked = true; break;
         }
      }
   }

   public ZXC.SkySklKind Fld_Skl1kind
   {
      get
      {
              if(rbt_skl1kind_NONE    .Checked) return ZXC.SkySklKind.NONE    ;
         else if(rbt_skl1kind_CentGLSK.Checked) return ZXC.SkySklKind.CentGLSK;
         else if(rbt_skl1kind_CentPVSK.Checked) return ZXC.SkySklKind.CentPVSK;
         else if(rbt_skl1kind_ShopVPSK.Checked) return ZXC.SkySklKind.ShopVPSK;
         else if(rbt_skl1kind_ShopMPSK.Checked) return ZXC.SkySklKind.ShopMPSK;

              else throw new Exception("Fld_Skl1kind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.SkySklKind.NONE    : rbt_skl1kind_NONE    .Checked = true; break;
            case ZXC.SkySklKind.CentGLSK: rbt_skl1kind_CentGLSK.Checked = true; break;
            case ZXC.SkySklKind.CentPVSK: rbt_skl1kind_CentPVSK.Checked = true; break;
            case ZXC.SkySklKind.ShopVPSK: rbt_skl1kind_ShopVPSK.Checked = true; break;
            case ZXC.SkySklKind.ShopMPSK: rbt_skl1kind_ShopMPSK.Checked = true; break;
         }
      }
   }

   public ZXC.SkySklKind Fld_Skl2kind
   {
      get
      {
              if(rbt_skl2kind_NONE    .Checked) return ZXC.SkySklKind.NONE    ;
         else if(rbt_skl2kind_CentGLSK.Checked) return ZXC.SkySklKind.CentGLSK;
         else if(rbt_skl2kind_CentPVSK.Checked) return ZXC.SkySklKind.CentPVSK;
         else if(rbt_skl2kind_ShopVPSK.Checked) return ZXC.SkySklKind.ShopVPSK;
         else if(rbt_skl2kind_ShopMPSK.Checked) return ZXC.SkySklKind.ShopMPSK;

              else throw new Exception("Fld_skl2kind: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.SkySklKind.NONE    : rbt_skl2kind_NONE    .Checked = true; break;
            case ZXC.SkySklKind.CentGLSK: rbt_skl2kind_CentGLSK.Checked = true; break;
            case ZXC.SkySklKind.CentPVSK: rbt_skl2kind_CentPVSK.Checked = true; break;
            case ZXC.SkySklKind.ShopVPSK: rbt_skl2kind_ShopVPSK.Checked = true; break;
            case ZXC.SkySklKind.ShopMPSK: rbt_skl2kind_ShopMPSK.Checked = true; break;
         }
      }
   }

   public ZXC.SkyOperation Fld_CentOPS
   {
      get
      {
              if(rbt_centOPS_NONE          .Checked) return ZXC.SkyOperation.NONE          ;
         else if(rbt_centOPS_SEND          .Checked) return ZXC.SkyOperation.SEND          ;
         else if(rbt_centOPS_RECEIVE       .Checked) return ZXC.SkyOperation.RECEIVE       ;
         else if(rbt_centOPS_SendAndReceive.Checked) return ZXC.SkyOperation.SendAndReceive;

              else throw new Exception("Fld_CentOPS: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.SkyOperation.NONE          : rbt_centOPS_NONE          .Checked = true; break;
            case ZXC.SkyOperation.SEND          : rbt_centOPS_SEND          .Checked = true; break;
            case ZXC.SkyOperation.RECEIVE       : rbt_centOPS_RECEIVE       .Checked = true; break;
            case ZXC.SkyOperation.SendAndReceive: rbt_centOPS_SendAndReceive.Checked = true; break;
         }
      }
   }

   public ZXC.SkyOperation Fld_ShopOPS
   {
      get
      {
              if(rbt_shopOPS_NONE          .Checked) return ZXC.SkyOperation.NONE          ;
         else if(rbt_shopOPS_SEND          .Checked) return ZXC.SkyOperation.SEND          ;
         else if(rbt_shopOPS_RECEIVE       .Checked) return ZXC.SkyOperation.RECEIVE       ;
         else if(rbt_shopOPS_SendAndReceive.Checked) return ZXC.SkyOperation.SendAndReceive;

              else throw new Exception("Fld_shopOPS: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.SkyOperation.NONE          : rbt_shopOPS_NONE          .Checked = true; break;
            case ZXC.SkyOperation.SEND          : rbt_shopOPS_SEND          .Checked = true; break;
            case ZXC.SkyOperation.RECEIVE       : rbt_shopOPS_RECEIVE       .Checked = true; break;
            case ZXC.SkyOperation.SendAndReceive: rbt_shopOPS_SendAndReceive.Checked = true; break;
         }
      }
   }

   public ZXC.SkyReceiveKind Fld_ShopRCVkind
   {
      get
      {
              if(rbt_shopRCVkind_NONE         .Checked) return ZXC.SkyReceiveKind.NONE         ;
         else if(rbt_shopRCVkind_EVERYTHING   .Checked) return ZXC.SkyReceiveKind.EVERYTHING   ;
         else if(rbt_shopRCVkind_OnlyLOCALskl1.Checked) return ZXC.SkyReceiveKind.OnlyLOCALskl1;
         else if(rbt_shopRCVkind_OnlyLOCALskl2.Checked) return ZXC.SkyReceiveKind.OnlyLOCALskl2;
         else if(rbt_shopRCVkind_OnlyOTHERskl1.Checked) return ZXC.SkyReceiveKind.OnlyOTHERskl1;
         else if(rbt_shopRCVkind_OnlyOTHERskl2.Checked) return ZXC.SkyReceiveKind.OnlyOTHERskl2;

              else throw new Exception("Fld_shopOPS: who df is checked?");
      }
      set
      {
         switch(value)
         {
            case ZXC.SkyReceiveKind.NONE         : rbt_shopRCVkind_NONE         .Checked = true; break;
            case ZXC.SkyReceiveKind.EVERYTHING   : rbt_shopRCVkind_EVERYTHING   .Checked = true; break;
            case ZXC.SkyReceiveKind.OnlyLOCALskl1: rbt_shopRCVkind_OnlyLOCALskl1.Checked = true; break;
            case ZXC.SkyReceiveKind.OnlyLOCALskl2: rbt_shopRCVkind_OnlyLOCALskl2.Checked = true; break;
            case ZXC.SkyReceiveKind.OnlyOTHERskl1: rbt_shopRCVkind_OnlyOTHERskl1.Checked = true; break;
            case ZXC.SkyReceiveKind.OnlyOTHERskl2: rbt_shopRCVkind_OnlyOTHERskl2.Checked = true; break;
         }
      }
   }

   public bool Fld_CentCanADD { get { return cbx_centCanADD.Checked; } set { cbx_centCanADD.Checked = value; } }
   public bool Fld_CentCanRWT { get { return cbx_centCanRWT.Checked; } set { cbx_centCanRWT.Checked = value; } }
   public bool Fld_CentCanDEL { get { return cbx_centCanDEL.Checked; } set { cbx_centCanDEL.Checked = value; } }
   public bool Fld_ShopCanADD { get { return cbx_shopCanADD.Checked; } set { cbx_shopCanADD.Checked = value; } }
   public bool Fld_ShopCanRWT { get { return cbx_shopCanRWT.Checked; } set { cbx_shopCanRWT.Checked = value; } }
   public bool Fld_ShopCanDEL { get { return cbx_shopCanDEL.Checked; } set { cbx_shopCanDEL.Checked = value; } }

   public bool Fld_NotBkgrndSND { get { return cbx_NotBkgrndSND.Checked; } set { cbx_NotBkgrndSND.Checked = value; } }
   public bool Fld_NotSNDonExLd { get { return cbx_NotSNDonExLd.Checked; } set { cbx_NotSNDonExLd.Checked = value; } }
   public bool Fld_NotRCVonLoad { get { return cbx_NotRCVonLoad.Checked; } set { cbx_NotRCVonLoad.Checked = value; } }



   #endregion Fld_

   #region PutFields / GetFields

   public override void PutFields(VvDataRecord skyRule)
   {
      skyRule_rec = (SkyRule)skyRule;

      if(skyRule_rec != null)
      {
         PutMetaFileds(skyRule_rec.AddUID, skyRule_rec.AddTS, skyRule_rec.ModUID, skyRule_rec.ModTS, skyRule_rec.RecID, skyRule_rec.LanSrvID, skyRule_rec.LanRecID);
//         PutIdentityFields(skyRule_rec.UserName, skyRule_rec.PrjktTick, ZXC.luiListaSkyRuleType.GetNameForThisCd(skyRule_rec.SkyRuleType), ZXC.luiListaSkyRuleScope.GetNameForThisCd(skyRule_rec.SkyRuleScope));
         VvHamper.SetChkBoxRadBttnAutoCheck(this, true);

         Fld_Record       = skyRule_rec.Record      ;
         Fld_DocumTT      = skyRule_rec.DocumTT     ;
         Fld_BirthLoc     = skyRule_rec.BirthLoc    ;
         Fld_Skl1kind     = skyRule_rec.Skl1kind    ;
         Fld_Skl2kind     = skyRule_rec.Skl2kind    ;
         Fld_CentOPS      = skyRule_rec.CentOPS     ;
         Fld_ShopOPS      = skyRule_rec.ShopOPS     ;
         Fld_ShopRCVkind  = skyRule_rec.ShopRCVkind ;
         Fld_CentCanADD   = skyRule_rec.CentCanADD  ;
         Fld_CentCanRWT   = skyRule_rec.CentCanRWT  ;
         Fld_CentCanDEL   = skyRule_rec.CentCanDEL  ;
         Fld_ShopCanADD   = skyRule_rec.ShopCanADD  ;
         Fld_ShopCanRWT   = skyRule_rec.ShopCanRWT  ;
         Fld_ShopCanDEL   = skyRule_rec.ShopCanDEL  ;
         Fld_Opis         = skyRule_rec.Opis        ;
         Fld_NotBkgrndSND = skyRule_rec.NotBkgrndSND;
         Fld_NotSNDonExLd = skyRule_rec.NotSNDonExLd;
         Fld_NotRCVonLoad = skyRule_rec.NotRCVonLoad;


         VvHamper.SetChkBoxRadBttnAutoCheck(this, false);

      }
   }

   public override void GetFields(bool fuse)
   {
         skyRule_rec.Record       = Fld_Record     ;
         skyRule_rec.DocumTT      = Fld_DocumTT    ;
         skyRule_rec.BirthLoc     = Fld_BirthLoc   ;
         skyRule_rec.Skl1kind     = Fld_Skl1kind   ;
         skyRule_rec.Skl2kind     = Fld_Skl2kind   ;
         skyRule_rec.CentOPS      = Fld_CentOPS    ;
         skyRule_rec.ShopOPS      = Fld_ShopOPS    ;
         skyRule_rec.ShopRCVkind  = Fld_ShopRCVkind;
         skyRule_rec.CentCanADD   = Fld_CentCanADD ;
         skyRule_rec.CentCanRWT   = Fld_CentCanRWT ;
         skyRule_rec.CentCanDEL   = Fld_CentCanDEL ;
         skyRule_rec.ShopCanADD   = Fld_ShopCanADD ;
         skyRule_rec.ShopCanRWT   = Fld_ShopCanRWT ;
         skyRule_rec.ShopCanDEL   = Fld_ShopCanDEL ;
         skyRule_rec.Opis         = Fld_Opis       ;
         skyRule_rec.NotBkgrndSND = Fld_NotBkgrndSND;
         skyRule_rec.NotSNDonExLd = Fld_NotSNDonExLd;
         skyRule_rec.NotRCVonLoad = Fld_NotRCVonLoad;

   }

   #endregion PutFields / GetFields

   #region Overriders

   public override VvDataRecord VirtualDataRecord
   {
      get { return this.skyRule_rec; }
      set {        this.skyRule_rec = (SkyRule)value; }
   }

   public override VvDaoBase TheVvDao
   {
      get { return ZXC.SkyRuleDao; }
   }

   public override VvFindDialog CreateVvFindDialog()
   {
      return CreateFind_SkyRule_Dialog();
   }

   public static VvFindDialog CreateFind_SkyRule_Dialog()
   {
      VvForm.VvSubModul vvSubModul   = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.LsSKY);
      VvDataRecord      vvDataRecord = ZXC.TheVvForm.CreateTheVvDataRecord_SwitchSubModulEnum(vvSubModul);

      VvFindDialog vvFindDialog = new VvFindDialog();
      VvRecLstUC vvRecListUC    = new SkyRuleListUC(vvFindDialog, (SkyRule)vvDataRecord, vvSubModul);
      vvFindDialog.TheRecListUC = vvRecListUC;

      return vvFindDialog;
   }


   public override Size ThisUcSize
   {
      get
      {
         return new Size(hamp_record.Right + ZXC.QunMrgn, hamp_record.Bottom + ZXC.QunMrgn);
      }
   }

   /* report
      public RptF_KKonta TheRptF_KKonta { get; set; }

      public override VvReport VirtualReport
      {
         get
         {
            return this.TheRptF_KKonta;
         }
      }

      public override string VirtualReportName
      {
         get
         {
            return "KARTICA KONTA";
         }
      }

      public override VvReport CreateVvRecordReport(string reportName, VvRptFilter vvRptFilter)
      {
         return new RptF_KKonta(reportName, (VvRpt_Fin_Filter)vvRptFilter);
      }
      */
   #endregion override-s
}

public class SkyRuleFilterUC : VvFilterUC
{
   #region Fieldz

   public VvTextBox tbxF_userName,
                    tbxF_prjktID, tbxF_prjktTick, tbxF_prjktnaziv,
                    tbxF_skyRuleScope_Name, tbxF_skyRuleType_Name, tbxF_documType_Name,
                    tbxF_skyRuleScope_Cd, tbxF_skyRuleType_Cd, tbxF_documType_Cd;

   private VvHamper hamper_UserName, hamper_Prjkt, hamper_SkyRule;

   #endregion Fieldz

   #region Constructor

   public SkyRuleFilterUC(VvUserControl vvUC)
   {
      this.SuspendLayout();

      TheVvUC = vvUC;

      InitializeHamper_UserName(out hamper_UserName);

      nextY = hamper_UserName.Bottom + razmakIzmjedjuHampera;
      InitializeHamper_Prjkt(out hamper_Prjkt);

      nextY = hamper_Prjkt.Bottom + razmakIzmjedjuHampera;
      MaxHamperWidth = hamper_Prjkt.Width;
      CreateHamper_4ButtonsResetGo_Width(MaxHamperWidth);
      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper_UserName, MaxHamperWidth, razmakIzmjedjuHampera);

      InitializeHamper_Privilegije(out hamper_SkyRule);
      nextY = hamper_SkyRule.Bottom + razmakIzmjedjuHampera;

      nextY = LocationOfHamper_HorLine(nextX, nextY, MaxHamperWidth) +  razmakIzmjedjuHampera;

      this.Size = new Size(MaxHamperWidth + 2 * razmakIzmjedjuHampera, nextY + ZXC.QunMrgn);

      VvHamper.Open_Close_Fields_ForWriting(this, ZXC.ZaUpis.Otvoreno, ZXC.ParentControlKind.VvReportUC);// jer se ponasa ko reportFilter

      this.ResumeLayout();
   }

   #endregion Constructor

   #region Hamper_UserName

   private void InitializeHamper_UserName(out VvHamper hamper)
   {
      hamper = new VvHamper(2, 1, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q5un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8, ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      hamper.VvRowHgt       = new int[] { ZXC.QUN };
      hamper.VvSpcBefRow    = new int[] { ZXC.Qun8 };
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbUserName;

      lbUserName    = hamper.CreateVvLabel  (0, 0, "User Name:", ContentAlignment.MiddleCenter);
      tbxF_userName = hamper.CreateVvTextBox(1, 0, "tbxF_userName", "User name");

      if(TheVvUC is UserUC)
      {
         tbxF_userName.JAM_ReadOnly = true;
      }
      else
      {
         tbxF_userName.JAM_SetAutoCompleteData(User.recordName, User.sorterUserName.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_User_sorterUserName), null);
         tbxF_userName.JAM_WriteOnly = true;
      }

      VvHamper.HamperStyling(hamper);
   }

   #endregion Hamper_UserName

   #region Hamper_Prjkt

   private void InitializeHamper_Prjkt(out VvHamper hamper)
   {
      hamper = new VvHamper(4, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q4un, ZXC.Q2un - ZXC.Qun8, ZXC.Q4un };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8           , ZXC.Qun8, ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i] = ZXC.QUN;
      }
      hamper.VvSpcBefRow = new int[] { ZXC.Qun4, ZXC.Qun8 };

      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbPrjkt;

      lbPrjkt = hamper.CreateVvLabel(0, 0, "Projekt:", ContentAlignment.MiddleRight);

      tbxF_prjktID    = hamper.CreateVvTextBox(1, 0, "tbxF_prjktID"   , "Sifra projekta" , 6);
      tbxF_prjktTick  = hamper.CreateVvTextBox(3, 0, "tbxF_prjktTick" , "Ticker projekta", 6);
      tbxF_prjktnaziv = hamper.CreateVvTextBox(1, 1, "tbxF_prjktnaziv", "Naziv projekta" , 30, 2, 0);

      if(TheVvUC is PrjktUC)
      {
         tbxF_prjktID.JAM_ReadOnly = tbxF_prjktTick.JAM_ReadOnly = tbxF_prjktnaziv.JAM_ReadOnly = true;
      }
      else
      {
         tbxF_prjktID   .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterKCD.SortType    , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterSifra), new EventHandler(AnyKupdobTextBoxLeave));
         tbxF_prjktTick .JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterTicker.SortType, new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterTicker), new EventHandler(AnyKupdobTextBoxLeave));
         tbxF_prjktnaziv.JAM_SetAutoCompleteData(Prjkt.recordName, Prjkt.sorterNaziv.SortType , new EventHandler(TheVvUC.OnVvTBEnter_SetAutocmplt_Prjkt_sorterNaziv), new EventHandler(AnyKupdobTextBoxLeave));

         tbxF_prjktID.JAM_CharEdits = ZXC.JAM_CharEdits.DigitsOnly;
         tbxF_prjktID.JAM_MustTabOutBeforeSubmit    = true;
       
         tbxF_prjktTick.JAM_MustTabOutBeforeSubmit  = true;
         tbxF_prjktTick.JAM_CharacterCasing = CharacterCasing.Upper;
         
         tbxF_prjktnaziv.JAM_MustTabOutBeforeSubmit = true;

         tbxF_prjktID.JAM_WriteOnly = tbxF_prjktTick.JAM_WriteOnly = tbxF_prjktnaziv.JAM_WriteOnly = true;

      }

      tbxF_prjktID.JAM_MarkAsNumericTextBox(0, false);
      tbxF_prjktID.TextAlign = HorizontalAlignment.Left;
      tbxF_prjktID.JAM_FillCharacter = '0';
     
      VvHamper.HamperStyling(hamper);

   }

   void AnyKupdobTextBoxLeave(object sender, EventArgs e)
   {
      if(TheVvUC.isPopulatingSifrar) return;

      if(ZXC.TheVvForm.TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      TextBox tb = sender as TextBox;
      Prjkt prjkt_rec;

      if(tb.Text != this.TheVvUC.originalText)
      {
         this.TheVvUC.originalText = tb.Text;
         prjkt_rec = VvUserControl.PrjktSifrar.Find(TheVvUC.FoundInSifrar<Prjkt>);

         if(prjkt_rec != null && tb.Text != "")
         {
            Fld_PrjktNaziv = prjkt_rec.Naziv;
            Fld_PrjktID    = prjkt_rec.KupdobCD/*RecID*/;
            Fld_PrjktTick  = prjkt_rec.Ticker;
         }
         else
         {
            Fld_PrjktNaziv = Fld_PrjktTick = Fld_PrjktIDAsTxt = "";
         }
      }
   }

   #endregion Hamper_Prjkt

   #region Hamper_Privilegije

   private void InitializeHamper_Privilegije(out VvHamper hamper)
   {
      hamper = new VvHamper(3, 2, "", this, false, nextX, nextY, razmakHampGroup);

      hamper.VvColWdt      = new int[] { ZXC.Q4un - ZXC.Qun4, ZXC.Q2un + ZXC.Qun2, ZXC.Q7un + ZXC.Qun2 };
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun8           , ZXC.Qun8           , ZXC.Qun8 };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.QUN;
         hamper.VvSpcBefRow[i] = ZXC.Qun8;
      }
      hamper.VvBottomMargin = hamper.VvTopMargin;

      Label lbl_skyRuleScope, lbl_skyRuleType;

      lbl_skyRuleScope = hamper.CreateVvLabel(0, 0, "Privilegija"      , ContentAlignment.MiddleCenter);
      lbl_skyRuleType  = hamper.CreateVvLabel(0, 1, "Doseg privilegije", ContentAlignment.MiddleCenter);


      tbxF_skyRuleType_Cd    = hamper.CreateVvTextBoxLookUp(1, 0, "tbx_skyRuleType_Cd", "Privilegija");
      tbxF_skyRuleType_Name  = hamper.CreateVvTextBox      (2, 0, "tbx_skyRuleType_Name" , "");
      tbxF_skyRuleScope_Cd   = hamper.CreateVvTextBoxLookUp(1, 1, "tbx_skyRuleScope_Cd", "Doseg privilegije");
      tbxF_skyRuleScope_Name = hamper.CreateVvTextBox      (2, 1, "tbx_skyRuleScope_Name", "");


      //tbxF_skyRuleScope_Cd.JAM_Set_LookUpTable(ZXC.luiListaSkyRuleScope, (int)ZXC.Kolona.prva);
      //tbxF_skyRuleScope_Cd.JAM_lui_NameTaker_JAM_Name = tbxF_skyRuleScope_Name.JAM_Name;
     
      //tbxF_skyRuleType_Cd.JAM_Set_LookUpTable(ZXC.luiListaSkyRuleType, (int)ZXC.Kolona.prva);
      //tbxF_skyRuleType_Cd.JAM_lui_NameTaker_JAM_Name = tbxF_skyRuleType_Name.JAM_Name;

      tbxF_skyRuleScope_Cd.JAM_WriteOnly = tbxF_skyRuleType_Cd.JAM_WriteOnly = true;

      tbxF_skyRuleType_Name. JAM_ReadOnly =
      tbxF_skyRuleScope_Name.JAM_ReadOnly = true;
      tbxF_skyRuleType_Name. Tag =
      tbxF_skyRuleScope_Name.Tag = ZXC.vvColors.userControl_BackColor;

      VvHamper.DesnoPoravnanjeHamperWidth_RadiLjepsegIzgleda(hamper, MaxHamperWidth, razmakIzmjedjuHampera);
      VvHamper.HamperStyling(hamper);
   }

   #endregion Hamper_Privilegije

   #region Fld_

   public string Fld_UserName
   {
      get { return tbxF_userName.Text; }
      set {        tbxF_userName.Text = value; }
   }
   public string Fld_PrjktTick
   {
      get { return tbxF_prjktTick.Text; }
      set {        tbxF_prjktTick.Text = value; }
   }
   public uint Fld_PrjktID
   {
      get { return ZXC.ValOrZero_UInt(tbxF_prjktID.Text); }
      set {                           tbxF_prjktID.Text = value.ToString("000000"); }

   }
   public string Fld_PrjktIDAsTxt
   {
      get { return tbxF_prjktID.Text; }
      set {        tbxF_prjktID.Text = value; }
   }
   public string Fld_PrjktNaziv
   {
      get { return tbxF_prjktnaziv.Text; }
      set {        tbxF_prjktnaziv.Text = value; }
   }
   public string Fld_SkyRuleScope
   {
      get { return tbxF_skyRuleScope_Name.Text; }
      set {        tbxF_skyRuleScope_Name.Text = value; }
   }

   public string Fld_SkyRuleType
   {
      get { return tbxF_skyRuleType_Name.Text; }
      set {        tbxF_skyRuleType_Name.Text = value; }
   }

   public string Fld_SkyRuleScopeCd
   {
      get { return tbxF_skyRuleScope_Cd.Text; }
      set {        tbxF_skyRuleScope_Cd.Text = value; }
   }

   public string Fld_SkyRuleTypeCd
   {
      get { return tbxF_skyRuleType_Cd.Text; }
      set {        tbxF_skyRuleType_Cd.Text = value; }
   }

   #endregion Fld_

   #region PutFilterFields() GetFilterFields()

   private VvRpt_SkyRule_Filter TheSkyRuleFilter
   {
      get { return this.TheVvUC.VirtualRptFilter as VvRpt_SkyRule_Filter; }
      set {        this.TheVvUC.VirtualRptFilter = value; }
   }

   public override void PutFilterFields(VvRptFilter _filter_data)
   {
      TheSkyRuleFilter = (VvRpt_SkyRule_Filter)_filter_data;

      if(TheSkyRuleFilter != null)
      {
         //Fld_UserName     = TheSkyRuleFilter.UserName;
         //Fld_PrjktID      = TheSkyRuleFilter.PrjktID;
         //Fld_PrjktTick    = TheSkyRuleFilter.PrjktTick;
         //Fld_PrjktNaziv   = TheSkyRuleFilter.PrjktNaziv;
         //Fld_SkyRuleScopeCd = TheSkyRuleFilter.SkyRuleScope;
         //Fld_SkyRuleTypeCd  = TheSkyRuleFilter.SkyRuleType;
      }

      // Za JAM_... : 
      this.ValidateChildren();
   }

   public override void GetFilterFields()
   {
      //TheSkyRuleFilter.UserName   = Fld_UserName;
      //TheSkyRuleFilter.PrjktID    = Fld_PrjktID;
      //TheSkyRuleFilter.PrjktTick  = Fld_PrjktTick;
      //TheSkyRuleFilter.PrjktNaziv = Fld_PrjktNaziv;
      //TheSkyRuleFilter.SkyRuleScope = Fld_SkyRuleScopeCd;
      //TheSkyRuleFilter.SkyRuleType  = Fld_SkyRuleTypeCd;
   }

   #endregion PutFilterFields() GetFilterFields()

   #region AddFilterMemberz()

   // iznimno ovdje a ne na npr. SkyRuleFilterUC jer takav ne postoji (Privilegije zasada nemaju Report-ove) 
   // al ce ipak imati 
   // public void AddFilterMemberz(VvRpt_SkyRule_Filter _theFilter/*, VvFinReport theVvFinReport*/)
   public override void AddFilterMemberz(VvRptFilter _vvRptFilter, VvReport _vvReport)
   {
      VvRpt_SkyRule_Filter _theFilter = (VvRpt_SkyRule_Filter)_vvRptFilter;

      string filterText;
      uint   uNum;
      DataRow drSchema;

      _theFilter.FilterMembers.Clear();
      _theFilter.ClearAllFilters_FromClauseGotTableName();

      //// Fld_UserName                                                                                                                                         

      //drSchema   = ZXC.SkyRuleSchemaRows[ZXC.SkyCI.userName];
      //filterText = Fld_UserName;

      //if(filterText.NotEmpty())
      //{
      //   _theFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "UserName", filterText, filterText, "", " = ", ""));
      //}

      //// Fld_Prjkt_sifra                                                                                                                                  

      //drSchema = ZXC.SkyRuleSchemaRows[ZXC.SkyCI.prjktID];
      //uNum     = Fld_PrjktID;

      //if(uNum != 0)
      //{
      //   //string kcd_string = tbx_KD_sifra.Text + " " + tbx_KD_ticker.Text + " " + tbx_KD_naziv.Text;

      //   _theFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "Projekt", uNum, ""/*kcd_string*/, "", " = ", ""));
      //}

      //// Fld_SkyRuleTypeCd                                                                                                                                         

      //drSchema   = ZXC.SkyRuleSchemaRows[ZXC.SkyCI.skyRuleType];
      //filterText = Fld_SkyRuleTypeCd;

      //if(filterText.NotEmpty())
      //{
      //   _theFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkyRuleType", filterText, filterText, "", " = ", ""));
      //}

      //// Fld_SkyRuleScopeCd                                                                                                                                         

      //drSchema   = ZXC.SkyRuleSchemaRows[ZXC.SkyCI.skyRuleScope];
      //filterText = Fld_SkyRuleScopeCd;

      //if(filterText.NotEmpty())
      //{
      //   _theFilter.FilterMembers.Add(new VvSqlFilterMember(drSchema, false, "SkyRuleScope", filterText, filterText, "", " = ", ""));
      //}

      //                                                                                                                                                  
   }

   #endregion AddFilterMemberz()

}


public class SIN_UC : VvUserControl
{
   #region Fieldz

   private Crownwood.DotNetMagic.Controls.TabControl ThePolyGridTabControl { get; set; }
   private Crownwood.DotNetMagic.Controls.TabPage    tabPage1, tabPage2;

   public Sin_VvSyncInfo_UC TheUC { get; private set; }

   public VvDataGridView TheG { get; set; }

   private VvTextBox vvtb_skladCD      , 
                     vvtb_tt           , 
                     vvtb_skladDate    , 
                     vvtb_artiklCD     , 
                     vvtb_ttNum        , 
                     vvtb_tSerial      ,
                     vvtb_lanCount     , 
                     vvtb_lanSumKol    , 
                     vvtb_lanSumKolCij ,
                     vvtb_skyCount     , 
                     vvtb_skySumKol    , 
                     vvtb_skySumKolCij ;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;
   private Color clr_oth_bc, clr_lan_bc, clr_sky_bc,
                 clr_oth_fc, clr_lan_fc, clr_sky_fc,
                 clr_empty;


   #endregion Fieldz

   #region Constructor

   public SIN_UC(Control _parent, VvForm.VvSubModul vvSubModul)
   {
      this.SuspendLayout();
      
      this.Parent = _parent;
      this.Dock = DockStyle.Fill;

      SetColors();

      CreateThePolyGridTabControl();
      CreateTheGrid();

      TheUC = new Sin_VvSyncInfo_UC(tabPage1);
      

      this.ResumeLayout();

      SetSyncColumnIndexes();
   }

   #endregion Constructor

   #region Colors

   private void SetColors()
   {
      clr_oth_bc = Color.LightCyan;
      clr_lan_bc = Color.CornflowerBlue;
      clr_sky_bc = Color.SteelBlue;
      clr_oth_fc = Color.DarkBlue;
      clr_lan_fc = 
      clr_sky_fc = Color.LemonChiffon;
      clr_empty  = Color.Violet;
   }

   #endregion Colors

   #region CreateThePolyGridTabControl

   private void CreateThePolyGridTabControl()
   {
      ThePolyGridTabControl                  = new Crownwood.DotNetMagic.Controls.TabControl();
      ThePolyGridTabControl.Appearance       = Crownwood.DotNetMagic.Controls.VisualAppearance.MultiDocument;
      ThePolyGridTabControl.ShowArrows       = false;
      ThePolyGridTabControl.ShowClose        = false;
      ThePolyGridTabControl.HotTrack         = true;
      ThePolyGridTabControl.Style            = ZXC.vvColors.vvform_VisualStyle;
      ThePolyGridTabControl.OfficeStyle      = ZXC.vvColors.tabControl_OfficeStyle;
      ThePolyGridTabControl.MediaPlayerStyle = ZXC.vvColors.tabControl_MediaPlayerStyle;
      ThePolyGridTabControl.Parent           = this;
      ThePolyGridTabControl.Location         = new Point(ZXC.Qun8, ZXC.Qun8);
      ThePolyGridTabControl.Dock             = DockStyle.Fill;


      tabPage1 = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle1);
      tabPage2 = new Crownwood.DotNetMagic.Controls.TabPage(TabPageTitle2);
      ThePolyGridTabControl.TabPages.Add(tabPage1);
      ThePolyGridTabControl.TabPages.Add(tabPage2);

      ThePolyGridTabControl.TabPages[TabPageTitle1].BackColor = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].BackColor = Color.NavajoWhite;

      ThePolyGridTabControl.TabPages[TabPageTitle1].Tag = ZXC.vvColors.tabPage4TheG_BackColor;
      ThePolyGridTabControl.TabPages[TabPageTitle2].Tag = Color.NavajoWhite;

   }

   #endregion CreateThePolyGridTabControl

   #region TabPageTitle

   public string TabPageTitle1 { get { return "°  Faktur  °"; } }
   public string TabPageTitle2 { get { return "°°"; } }

   #endregion TabPageTitle

   #region TheGrid and columns

   private void CreateTheGrid()
   {
      TheG          = new VvDataGridView();
      TheG.Parent   = tabPage1;
      TheG.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      TheG.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      TheG.AutoGenerateColumns                  = false;

      TheG.RowHeadersBorderStyle = TheG.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      TheG.ColumnHeadersHeight   = ZXC.QUN + ZXC.Qun4;
      TheG.RowTemplate.Height    = ZXC.QUN;
      TheG.RowHeadersWidth       = ZXC.Q2un + ZXC.Qun4;
      TheG.Height                = TheG.ColumnHeadersHeight + TheG.RowTemplate.Height;

      TheG.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      TheG.Validating     += new System.ComponentModel.CancelEventHandler(VvDocumentRecordUC.grid_Validating);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheG);
      VvHamper.Open_Close_Fields_ForWriting(TheG, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      TheG.AllowUserToAddRows       =
      TheG.AllowUserToDeleteRows    =
      TheG.AllowUserToOrderColumns  =
      TheG.AllowUserToResizeColumns =
      TheG.AllowUserToResizeRows    = false;

      TheG.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      TheG.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      TheG.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      TheG.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;

      TheG.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.BaseFont;
      TheG.RowsDefaultCellStyle         .Font = ZXC.vvFont.BaseFont;
      TheG.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.BaseFont;

      CreateColumn(TheG);

   
      TheG.Parent.Size   = new Size(this.Width, this.Height);
      TheG.Parent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      TheG.Size = new Size(this.Width - 2*ZXC.QunMrgn, this.Height - ZXC.Q2un);
      TheG.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

   }

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_skladCD       = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_skladCD"      , null, -12, "SkladCD"       ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skladCD      , null, "R_skladCD"      , "SkladCD"      , ZXC.Q3un           ); vvtb_skladCD      .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc; 
      vvtb_tt            = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_tt"           , null, -12, "TT"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tt           , null, "R_tt"           , "TT"           , ZXC.Q2un           ); vvtb_tt           .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_skladDate     = theGrid.CreateVvTextBoxFor_DateTime_ColumnTemplate(       "vvtb_skladDate"    , null, -12, "SkladDate"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skladDate    , null, "R_skladDate"    , "Dok Date"     , ZXC.Q4un - ZXC.Qun2); vvtb_skladDate    .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_artiklCD      = theGrid.CreateVvTextBoxFor_String_ColumnTemplate  (       "vvtb_artiklCD"     , null, -12, "ArtiklCD"      ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_artiklCD     , null, "R_artiklCD"     , "Artikl CD"    , ZXC.Q3un           ); vvtb_artiklCD     .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q3un;
      vvtb_ttNum         = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_ttNum"        , null, -12, "TtNum"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_ttNum        , null, "R_ttNum"        , "TT Num"       , ZXC.Q3un           ); vvtb_ttNum        .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_tSerial       = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_tSerial"      , null, -12, "TSerial"       ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_tSerial      , null, "R_tSerial"      , "Serial"       , ZXC.Q2un + ZXC.Qun4); vvtb_tSerial      .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_oth_bc;  colVvText.DefaultCellStyle.ForeColor = clr_oth_fc;
      vvtb_lanCount      = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_lanCount"     , null, -12, "LAN Count"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_lanCount     , null, "R_lanCount"     , "LANrtrCount"  , ZXC.Q4un           ); vvtb_lanCount     .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_lan_bc;  colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_lanSumKol     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_lanSumKol"    , null, -12, "LAN SumKol"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_lanSumKol    , null, "R_lanSumKol"    , "LANsumKol"    , ZXC.Q4un           ); vvtb_lanSumKol    .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_lan_bc;  colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_lanSumKolCij  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_lanSumKolCij" , null, -12, "LAN SumKolCij" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_lanSumKolCij , null, "R_lanSumKolCij" , "LANsumKolCij" , ZXC.Q5un - ZXC.Qun2); vvtb_lanSumKolCij .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_lan_bc;  colVvText.DefaultCellStyle.ForeColor = clr_lan_fc;
      vvtb_skyCount      = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate (false, "vvtb_skyCount"     , null, -12, "SKY Count"     ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skyCount     , null, "R_skyCount"     , "SKYrtrCount"  , ZXC.Q4un           ); vvtb_skyCount     .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_sky_bc;  colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_skySumKol     = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_skySumKol"    , null, -12, "SKY SumKol"    ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skySumKol    , null, "R_skySumKol"    , "SKYsumKol"    , ZXC.Q4un           ); vvtb_skySumKol    .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_sky_bc;  colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;
      vvtb_skySumKolCij  = theGrid.CreateVvTextBoxFor_Decimal_ColumnTemplate (    2, "vvtb_skySumKolCij" , null, -12, "SKY SumKolCij" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skySumKolCij , null, "R_skySumKolCij" , "SKYsumKolCij" , ZXC.Q5un - ZXC.Qun2); vvtb_skySumKolCij .JAM_ReadOnly = true;  colVvText.DefaultCellStyle.BackColor = clr_sky_bc;  colVvText.DefaultCellStyle.ForeColor = clr_sky_fc;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);
      colScrol.DefaultCellStyle.BackColor = TheG.ColumnHeadersDefaultCellStyle.BackColor;

   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private Sync_colIdx ci;
   public Sync_colIdx DgvCI { get { return ci; } }
   public struct Sync_colIdx
   {
      internal int iT_skladCD     ; 
      internal int iT_tt          ; 
      internal int iT_skladDate   ; 
      internal int iT_artiklCD    ; 
      internal int iT_ttNum       ; 
      internal int iT_tSerial     ; 
      internal int iT_lanCount    ; 
      internal int iT_lanSumKol   ; 
      internal int iT_lanSumKolCij; 
      internal int iT_skyCount    ; 
      internal int iT_skySumKol   ; 
      internal int iT_skySumKolCij; 

   }

   private void SetSyncColumnIndexes()
   {
      ci = new Sync_colIdx();

      ci.iT_skladCD       = TheG.IdxForColumn("R_skladCD"     );
      ci.iT_tt            = TheG.IdxForColumn("R_tt"          );
      ci.iT_skladDate     = TheG.IdxForColumn("R_skladDate"   );
      ci.iT_artiklCD      = TheG.IdxForColumn("R_artiklCD"    );
      ci.iT_ttNum         = TheG.IdxForColumn("R_ttNum"       );
      ci.iT_tSerial       = TheG.IdxForColumn("R_tSerial"     );
      ci.iT_lanCount      = TheG.IdxForColumn("R_lanCount"    );
      ci.iT_lanSumKol     = TheG.IdxForColumn("R_lanSumKol"   );
      ci.iT_lanSumKolCij  = TheG.IdxForColumn("R_lanSumKolCij");
      ci.iT_skyCount      = TheG.IdxForColumn("R_skyCount"    );
      ci.iT_skySumKol     = TheG.IdxForColumn("R_skySumKol"   );
      ci.iT_skySumKolCij  = TheG.IdxForColumn("R_skySumKolCij");
   }

   #endregion SetColumnIndexes()

   #region PutDgvFields1

   public void PutDgvFields1(List<VvSkyLab.CheckSyncPair> checkSyncPairList)
   {
      int rowIdx;

      TheG.Rows.Clear();

      if(checkSyncPairList != null)
      {
         for(rowIdx = 0; rowIdx < checkSyncPairList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheG.Rows.Add();

            PutDgvLineFields1(rowIdx, checkSyncPairList[rowIdx]);

            TheG.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

            if(TheG.GetDecimalCell(ci.iT_lanCount, rowIdx, false).IsZero()) 
            {
               TheG.Rows[rowIdx].Cells[ci.iT_lanCount    ].Style.BackColor =
               TheG.Rows[rowIdx].Cells[ci.iT_lanSumKol   ].Style.BackColor = 
               TheG.Rows[rowIdx].Cells[ci.iT_lanSumKolCij].Style.BackColor = clr_empty;
            }
            if(TheG.GetDecimalCell(ci.iT_skyCount, rowIdx, false).IsZero()) 
            {
               TheG.Rows[rowIdx].Cells[ci.iT_skyCount    ].Style.BackColor =
               TheG.Rows[rowIdx].Cells[ci.iT_skySumKol   ].Style.BackColor = 
               TheG.Rows[rowIdx].Cells[ci.iT_skySumKolCij].Style.BackColor = clr_empty;
            }

            TheG.Rows[rowIdx].Cells[ci.iT_lanCount].Style.Alignment = 
            TheG.Rows[rowIdx].Cells[ci.iT_skyCount].Style.Alignment = 
            TheG.Rows[rowIdx].Cells[ci.iT_ttNum   ].Style.Alignment = 
            TheG.Rows[rowIdx].Cells[ci.iT_tSerial ].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
         
            TheG.Columns[ci.iT_lanCount].HeaderCell.Style.Alignment = 
            TheG.Columns[ci.iT_skyCount].HeaderCell.Style.Alignment = 
            TheG.Columns[ci.iT_ttNum   ].HeaderCell.Style.Alignment = 
            TheG.Columns[ci.iT_tSerial ].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

         }

      }

   }

   private void PutDgvLineFields1(int rowIdx, VvSkyLab.CheckSyncPair checkSyncPair)
   {
      TheG.PutCell(ci.iT_skladCD     , rowIdx, checkSyncPair.skladCD     );
      TheG.PutCell(ci.iT_tt          , rowIdx, checkSyncPair.tt          );
      TheG.PutCell(ci.iT_skladDate   , rowIdx, checkSyncPair.skladDate   );
      TheG.PutCell(ci.iT_artiklCD    , rowIdx, checkSyncPair.artiklCD    );
      TheG.PutCell(ci.iT_ttNum       , rowIdx, checkSyncPair.ttNum       .ToString("0;;#"));
      TheG.PutCell(ci.iT_tSerial     , rowIdx, checkSyncPair.tSerial     .ToString("0;;#"));
      TheG.PutCell(ci.iT_lanCount    , rowIdx, checkSyncPair.lanCount    .ToString("0;;#"));
      TheG.PutCell(ci.iT_lanSumKol   , rowIdx, checkSyncPair.lanSumKol   );
      TheG.PutCell(ci.iT_lanSumKolCij, rowIdx, checkSyncPair.lanSumKolCij);
      TheG.PutCell(ci.iT_skyCount    , rowIdx, checkSyncPair.skyCount    .ToString("0;;#"));
      TheG.PutCell(ci.iT_skySumKol   , rowIdx, checkSyncPair.skySumKol   );
      TheG.PutCell(ci.iT_skySumKolCij, rowIdx, checkSyncPair.skySumKolCij);
   }

   public override void GetFields(bool fuse)
   {
      // notin to do;
   }

   #endregion PutDgvFields1

}

public partial class Sin_VvSyncInfoDLG :  VvDialog
{
   public Sin_VvSyncInfo_UC TheUC { get; set; }
   private Button okButton, cancelButton;
   private int dlgWidth, dlgHeight;
   private List<VvSkyLab.VvSyncInfo> TheSyncInfoList;

   private bool IsForErrorLOG;
   //private ZXC.SkyOperation TheWantedOperation;
   //private bool             TheIsInitiatedExplicit;
   //private DateTime         TheThisSyncTS; 
   //private DateTime         TheEndOfSyncTS;

   public Sin_VvSyncInfoDLG(bool isErrorLogInfo)
   {
      ZXC.CurrentForm = this;
      IsForErrorLOG = isErrorLogInfo;

      TheUC = new Sin_VvSyncInfo_UC(this, TheSyncInfoList, IsForErrorLOG);

      SuspendLayout();

      this.Font        = ZXC.vvFont.BaseFont;
      this.Style       = ZXC.vvColors.vvform_VisualStyle;
      this.BackColor   = ZXC.vvColors.userControl_BackColor;

      this.StartPosition = FormStartPosition.Manual;
      this.Text = "SyncInfo";

      TheUC.Parent   = this;
      TheUC.Location = new Point(ZXC.Qun8, ZXC.Qun8);
      
      dlgWidth  = TheUC.Width;
      dlgHeight = TheUC.Height + ZXC.QunMrgn * 2 + ZXC.QunBtnH;

      this.MaximizeBox = true;

      this.ClientSize = new Size(dlgWidth, dlgHeight);
      //AddOkButton(out okButton, dlgWidth, dlgHeight);
      //okButton.Location = Calc_LeftMost_ButtonLocation(dlgWidth, dlgHeight, 2);
      AddOkCancelButtons(out okButton, out cancelButton, dlgWidth, dlgHeight);
      okButton.Anchor = cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      this.AcceptButton = okButton;

      okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

      TheUC.Anchor         =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      TheUC.TheGrid.Anchor =  AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      
      this.cancelButton.Click += new EventHandler(cancelButton_Click); // Da supresa validaciju

      ResumeLayout();


   }
   void cancelButton_Click(object sender, EventArgs e)
   {
      this.Close();
   }

   public Sin_VvSyncInfoDLG(List<VvSkyLab.VvSyncInfo> syncInfoList, ZXC.SkyOperation wantedOperation, bool isInitiatedExplicit, DateTime thisSyncTS, DateTime endOfSyncTS, bool isCheckOnly, bool isErrorLogInfo)
      : this(isErrorLogInfo)
   {
      // TODO: Complete member initialization
      this.TheSyncInfoList       = syncInfoList;
      //this.TheWantedOperation    = wantedOperation;
      //this.TheIsInitiatedExplicit= isInitiatedExplicit;
      //this.TheThisSyncTS         = thisSyncTS;
      //this.TheEndOfSyncTS        = endOfSyncTS;

      bool anyBAD = syncInfoList.Any(s => s.IsOK == false);


      this.Text = (IsForErrorLOG ? "TODAY ERROR LOG content. " : "") + (anyBAD ? "!!! " : "") + ZXC.vv_PRODUCT_Name + " " + (isCheckOnly ? " CheckOnly " : "") +
         
         wantedOperation + (isInitiatedExplicit ? " Explicit " : " Implicit ") + " start: " + thisSyncTS.ToString(ZXC.VvDateAndTimeFormat) + " end: " + endOfSyncTS.ToString(ZXC.VvDateAndTimeFormat)
         + " duration: " + (endOfSyncTS - thisSyncTS) + " total: " + syncInfoList.Count + " ok: " + syncInfoList.Count(s => s.IsOK) + " BAD: " + syncInfoList.Count(s => s.IsOK == false);

      this.FormClosing += new FormClosingEventHandler(RestoreZxcCurrentForm_FormClosing);

   }
   void RestoreZxcCurrentForm_FormClosing(object sender, FormClosingEventArgs e)
   {
      ZXC.CurrentForm = ZXC.TheVvForm;
   }

}

public class Sin_VvSyncInfo_UC : UserControl
{
   #region Fieldz

   public VvDataGridView TheGrid { get; set; }

   private VvTextBox
      /*ZXC.SkyOperation       */ vvtb_WantedOperation,
      // /*SkyRule                */ vvtb_SkyRule                ,
      /*string                 */ vvtb_record,
      /*string                 */ vvtb_documTT,
      /*ZXC.LanSrvKind         */ vvtb_birthLoc,
      /*ZXC.SkySklKind         */ vvtb_skl1kind,
      /*ZXC.SkySklKind         */ vvtb_skl2kind,
      /*string                 */ vvtb_VvDataRecordInfo,
      /*bool                   */ vvtb_IsOK,
      /*int                    */ vvtb_SqlErrNo,
      /*string                 */ vvtb_SqlErrMessage,
      // /*ZXC.DBactionForSrvRecID*/ vvtb_ResultingSrvRecIDaction,
      /*uint                   */ vvtb_recID,
      /*uint                   */ vvtb_lanSrvID,
      /*uint                   */ vvtb_lanRecID,
      /*VvSQL.DB_RW_ActionType */ vvtb_action;

   private VvTextBoxColumn colVvText;
   private DataGridViewTextBoxColumn colScrol;

   private bool IsForErrorLOG;

   #endregion Fieldz

   #region Constructor

   public Sin_VvSyncInfo_UC(Control _parent, List<VvSkyLab.VvSyncInfo> TheSyncInfoList, bool isForErrorLOG)
   {
      this.SuspendLayout();

      this.Parent = _parent;

      IsForErrorLOG = isForErrorLOG;

      CreateTheGrid();
      CalcLocationAndSize();

      this.ResumeLayout();

      SetOTPColumnIndexes();

   }

   public Sin_VvSyncInfo_UC(Control _parent)
   {
   }

   #endregion Constructor

   #region CalcLocationAndSize

   private void CalcLocationAndSize()
   {
      this.Size = new Size(TheGrid.Width + 2 * ZXC.QunMrgn, SystemInformation.WorkingArea.Height - ZXC.Q5un);
      TheGrid.Height = this.Size.Height - ZXC.Q2un;
   }

   #endregion CalcLocationAndSize

   #region TheGrid

   private void CreateTheGrid()
   {
      TheGrid          = new VvDataGridView();
      TheGrid.Parent   = this;
      TheGrid.Location = new Point(ZXC.QunMrgn, ZXC.QunMrgn);

      TheGrid.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      TheGrid.AutoGenerateColumns                  = false;

      TheGrid.RowHeadersBorderStyle = TheGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      TheGrid.ColumnHeadersHeight   = ZXC.QUN - ZXC.Qun8;
      TheGrid.RowTemplate.Height    = ZXC.QUN - ZXC.Qun8;
      TheGrid.RowHeadersWidth       = ZXC.Q4un;
      TheGrid.Height                = TheGrid.ColumnHeadersHeight + TheGrid.RowTemplate.Height;

      TheGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheGrid);

      TheGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(VvDocumentRecordUC.grid_CellFormatting_FormatVvDateTime);
      TheGrid.Validating += new System.ComponentModel.CancelEventHandler(VvDocumentRecordUC.grid_Validating);

      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheGrid);
      VvHamper.Open_Close_Fields_ForWriting(TheGrid, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);

      TheGrid.AllowUserToAddRows       =
      TheGrid.AllowUserToDeleteRows    =
      TheGrid.AllowUserToOrderColumns  =
      TheGrid.AllowUserToResizeColumns =
      TheGrid.AllowUserToResizeRows    = false;

      TheGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.PowderBlue;
      TheGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkSlateGray;
      TheGrid.RowHeadersDefaultCellStyle.BackColor    = Color.PowderBlue; //Color.FloralWhite;
      TheGrid.RowHeadersDefaultCellStyle.ForeColor    = Color.DarkSlateGray;

      TheGrid.ColumnHeadersDefaultCellStyle.Font = ZXC.vvFont.SmallFont;
      TheGrid.RowsDefaultCellStyle         .Font = ZXC.vvFont.SmallFont;
      TheGrid.RowHeadersDefaultCellStyle   .Font = ZXC.vvFont.SmallFont;

      CreateColumn(TheGrid);
      int sumoOfColumns = 0;
      foreach(DataGridViewColumn dc in TheGrid.Columns)
      {
         sumoOfColumns += dc.Width;
      }

      TheGrid.Width = sumoOfColumns + TheGrid.RowHeadersWidth + ZXC.QUN + ZXC.Qun4;
      TheGrid.Height = this.Size.Height - ZXC.QUN;
   }

   #endregion TheGrid

   #region TheGridColumn

   private void CreateColumn(VvDataGridView theGrid)
   {
      vvtb_WantedOperation  = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_WantedOperation" , null, -12, "WantedOperation" ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_WantedOperation , null, "R_WantedOperation" , "Operation" , ZXC.Q3un); vvtb_WantedOperation  .JAM_ReadOnly = true; 
      vvtb_record           = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_record"          , null, -12, "Record"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_record          , null, "R_record"          , "Record"    , ZXC.Q3un); vvtb_record           .JAM_ReadOnly = true;
      vvtb_documTT          = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_documTT"         , null, -12, "DocumTT"         ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_documTT         , null, "R_documTT"         , "TT "       , ZXC.Q2un); vvtb_documTT          .JAM_ReadOnly = true;
      vvtb_birthLoc         = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_birthLoc"        , null, -12, "BirthLoc"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_birthLoc        , null, "R_birthLoc"        , "BirthLoc"  , ZXC.Q3un); vvtb_birthLoc         .JAM_ReadOnly = true;
      vvtb_skl1kind         = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_skl1kind"        , null, -12, "Skl1kind"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skl1kind        , null, "R_skl1kind"        , "Skl1kind"  , ZXC.Q3un); vvtb_skl1kind         .JAM_ReadOnly = true;
      vvtb_skl2kind         = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_skl2kind"        , null, -12, "Skl2kind"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_skl2kind        , null, "R_skl2kind"        , "Skl2kind"  , ZXC.Q3un); vvtb_skl2kind         .JAM_ReadOnly = true;
      vvtb_VvDataRecordInfo = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_VvDataRecordInfo", null, -12, "VvDataRecordInfo"); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_VvDataRecordInfo, null, "R_VvDataRecordInfo", "RecordInfo", ZXC.Q10un + ZXC.Qun5); vvtb_VvDataRecordInfo.JAM_ReadOnly = true; colVvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; colVvText.MinimumWidth = ZXC.Q10un+ ZXC.Qun5;
      vvtb_IsOK             = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_IsOK"            , null, -12, "IsOK"            ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_IsOK            , null, "R_IsOK"            , "IsOK"      , ZXC.Q2un); vvtb_IsOK             .JAM_ReadOnly = true;
      vvtb_SqlErrNo         = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb_SqlErrNo" , null, -12, "SqlErrNo"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_SqlErrNo        , null, "R_SqlErrNo"        , "ErrNo"     , ZXC.Q2un); vvtb_SqlErrNo         .JAM_ReadOnly = true;
      vvtb_SqlErrMessage    = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_SqlErrMessage"   , null, -12, "SqlErrMessage"   ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_SqlErrMessage   , null, "R_SqlErrMessage"   , "ErrMessage", ZXC.Q10un + ZXC.Qun5); vvtb_SqlErrMessage   .JAM_ReadOnly = true;
      vvtb_recID            = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb_recID"    , null, -12, "RecID"           ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_recID           , null, "R_recID"           , "RecID"     , ZXC.Q2un); vvtb_recID            .JAM_ReadOnly = true;
      vvtb_lanSrvID         = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb_lanSrvID" , null, -12, "LanSrvID"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_lanSrvID        , null, "R_lanSrvID"        , "LanSrvID"  , ZXC.Q3un); vvtb_lanSrvID         .JAM_ReadOnly = true;
      vvtb_lanRecID         = theGrid.CreateVvTextBoxFor_Integer_ColumnTemplate(false, "vvtb_lanRecID" , null, -12, "LanRecID"        ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_lanRecID        , null, "R_lanRecID"        , "LanRecID"  , ZXC.Q3un); vvtb_lanRecID         .JAM_ReadOnly = true;
      vvtb_action           = theGrid.CreateVvTextBoxFor_String_ColumnTemplate ("vvtb_action"          , null, -12, "Action"          ); colVvText = theGrid.CreateVvTextBoxColumn(vvtb_action          , null, "R_action"          , "Action  "  , ZXC.Q3un); vvtb_action           .JAM_ReadOnly = true;

      colScrol = theGrid.CreateScrollColumn("scrol", ZXC.QUN);

   }

   #endregion TheGridColumn

   #region SetColumnIndexes()

   private SinVvSyncInfo_colIdx ci;
   public SinVvSyncInfo_colIdx DgvCI { get { return ci; } }
   public struct SinVvSyncInfo_colIdx
   {
      internal int iT_WantedOperation;
      internal int iT_record;
      internal int iT_documTT;
      internal int iT_birthLoc;
      internal int iT_skl1kind;
      internal int iT_skl2kind;
      internal int iT_VvDataRecordInfo;
      internal int iT_IsOK;
      internal int iT_SqlErrNo;
      internal int iT_SqlErrMessage;
      internal int iT_recID;
      internal int iT_lanSrvID;
      internal int iT_lanRecID;
      internal int iT_action;

   }

   private void SetOTPColumnIndexes()
   {
      ci = new SinVvSyncInfo_colIdx();

      ci.iT_WantedOperation  = TheGrid.IdxForColumn("R_WantedOperation");
      ci.iT_record           = TheGrid.IdxForColumn("R_record");
      ci.iT_documTT          = TheGrid.IdxForColumn("R_documTT");
      ci.iT_birthLoc         = TheGrid.IdxForColumn("R_birthLoc");
      ci.iT_skl1kind         = TheGrid.IdxForColumn("R_skl1kind");
      ci.iT_skl2kind         = TheGrid.IdxForColumn("R_skl2kind");
      ci.iT_VvDataRecordInfo = TheGrid.IdxForColumn("R_VvDataRecordInfo");
      ci.iT_IsOK             = TheGrid.IdxForColumn("R_IsOK");
      ci.iT_SqlErrNo         = TheGrid.IdxForColumn("R_SqlErrNo");
      ci.iT_SqlErrMessage    = TheGrid.IdxForColumn("R_SqlErrMessage");
      ci.iT_recID            = TheGrid.IdxForColumn("R_recID");
      ci.iT_lanSrvID         = TheGrid.IdxForColumn("R_lanSrvID");
      ci.iT_lanRecID         = TheGrid.IdxForColumn("R_lanRecID");
      ci.iT_action           = TheGrid.IdxForColumn("R_action");
   }

   #endregion SetColumnIndexes()

   public void PutDgvFields(List<VvSkyLab.VvSyncInfo> syncInfoList)
   {
      int rowIdx;

      TheGrid.Rows.Clear();

      if(syncInfoList != null)
      {
         for(rowIdx = 0; rowIdx < syncInfoList.Count; ++rowIdx)  // 'exists safe': PutCell vodi brigu da li col uopce postoji 
         {
            TheGrid.Rows.Add();

            PutDgvLineFields(rowIdx, syncInfoList[rowIdx]);

            TheGrid.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

            if(TheGrid.GetBoolCell(ci.iT_IsOK, rowIdx, false) == false && IsForErrorLOG == false) // NONE znaci da smo dosli za ispis ERR log-a 
            {
               foreach(DataGridViewTextBoxCell tbxCell in TheGrid.Rows[rowIdx].Cells)
               {
                  tbxCell.Style.ForeColor = Color.Red;
                //tbxCell.Style.Font      = ZXC.vvFont.BaseBoldFont;

                 // Color.FromArgb(207, 183, 194);
               }
            }

         }

      }

   }

   private void PutDgvLineFields(int rowIdx, VvSkyLab.VvSyncInfo syncInfo)
   {
      TheGrid.PutCell(ci.iT_WantedOperation , rowIdx, syncInfo.WantedOperation                 );
      TheGrid.PutCell(ci.iT_record          , rowIdx, syncInfo.SkyRule.Record                  );
      TheGrid.PutCell(ci.iT_documTT         , rowIdx, syncInfo.SkyRule.DocumTT                 );
      TheGrid.PutCell(ci.iT_birthLoc        , rowIdx, syncInfo.SkyRule.BirthLoc                );
      TheGrid.PutCell(ci.iT_skl1kind        , rowIdx, syncInfo.SkyRule.Skl1kind                );
      TheGrid.PutCell(ci.iT_skl2kind        , rowIdx, syncInfo.SkyRule.Skl2kind                );
      TheGrid.PutCell(ci.iT_VvDataRecordInfo, rowIdx, syncInfo.VvDataRecordInfo                );
      TheGrid.PutCell(ci.iT_IsOK            , rowIdx, syncInfo.IsOK                            );
      TheGrid.PutCell(ci.iT_SqlErrNo        , rowIdx, syncInfo.SqlErrNo                        );
      TheGrid.PutCell(ci.iT_SqlErrMessage   , rowIdx, syncInfo.SqlErrMessage                   );
      TheGrid.PutCell(ci.iT_recID           , rowIdx, syncInfo.ResultingSrvRecIDaction.recID   );
      TheGrid.PutCell(ci.iT_lanSrvID        , rowIdx, syncInfo.ResultingSrvRecIDaction.lanSrvID);
      TheGrid.PutCell(ci.iT_lanRecID        , rowIdx, syncInfo.ResultingSrvRecIDaction.lanRecID);
      TheGrid.PutCell(ci.iT_action          , rowIdx, syncInfo.ResultingSrvRecIDaction.action  );
   }
}