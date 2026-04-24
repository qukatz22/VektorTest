using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static VvForm;

public static class VvIco
{
   #region Icon

   private static Stream GetManifestResourceStream(string iconFilePath)
   {
      return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(iconFilePath);
   }
   
   public static Icon DBkonekt              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.DBkonekt.ico"                ));
   public static Icon New                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.new.ico"                     ));
   public static Icon Opn                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.opn.ico"                     ));
   public static Icon Del                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.del.ico"                     ));
   public static Icon Cpy                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.cpy.ico"                     ));
   public static Icon Sav                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.sav.ico"                     ));
   public static Icon Sas                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.sas.ico"                     ));
   public static Icon Esc                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico"                     ));
   public static Icon Refresh               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.refresh.ico"), 32, 32);
   public static Icon Fnd                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.fnd.ico"                     ));
   public static Icon Frs                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.frs.ico"                     ));
   public static Icon Prv                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.prv.ico"                     ));
   public static Icon Nxt                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.nxt.ico"                     ));
   public static Icon Lst                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.lst.ico"                     ));
   public static Icon QPrint                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.qPrint.ico"), 32, 32);
   public static Icon PrintPrw              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.printPrw.ico"                ));
   public static Icon Arhiva                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.Arhiva.ico"                  ));                
   public static Icon ARH_OldVer            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.ARH_OldVer.ico"              ));
   public static Icon ARH_NewVer            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.ARH_NewVer.ico"              ));
   public static Icon ChckUpdate            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.chckUpdate.ico"              ));
   public static Icon FullScreen            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.fullScreen.ico"              ));
   public static Icon Resize                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.resize.ico"), 32, 32);
   public static Icon ThisCache             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.thisCashe.ico"               ));
   public static Icon MenueR_red            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.menueR_red.ico"              ));
   public static Icon MenueL_red            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.menueL_red.ico"              ));
   public static Icon MenueNO               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.menueNO.ico"                 ));
   public static Icon Filter                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.filter.ico"                  ));
   public static Icon TabUP                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.tabUP.ico"   ), 32, 32);
   public static Icon CloseAll              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.CloseAll.ico"), 32, 32);
   public static Icon Dirty                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.dirty.ico"                   ));

   public static Icon ToXml                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.xml.ico"                     ));
   public static Icon Go                    = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.go.ico"                      ));
   public static Icon Zoom                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.zoom.ico"                    ));
   public static Icon OffOpen               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.OffOpen.ico"                 ));     
   public static Icon Cancel                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.cancel.ico"                  ));      
   public static Icon Prnt                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.prnt.ico"                    ));        
   public static Icon Pdf                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.pdf.ico"                     ));         
   public static Icon SendMail              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.sendMail.ico"                 ));     
   public static Icon PdfManager            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.pdfManager.ico"              ));  
   public static Icon Export                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.export.ico"                  ));      
   public static Icon FirstRed              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.firstRed.ico"                ));    
   public static Icon PrevRed               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.prevRed.ico"                 ));     
   public static Icon NextRed               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.nextRed.ico"                 ));     
   public static Icon LastRed               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.lastRed.ico"                 ));     
   public static Icon Zoom100               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.zoom100.ico"                 ));     
   public static Icon ZoomW                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.zoomW.ico"                   ));       
   public static Icon ZoomP                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.zoomP.ico"                   ));       
   public static Icon GrTree                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.grTree.ico"                  ));      
   public static Icon FindPage              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.findPage.ico"                ));    
   public static Icon FindText              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.findText.ico"                ));    
   public static Icon CloseCurView          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.closeCurView.ico"            ));

                                                                                                                                           
   public static Icon Sifrar                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_green.ico"    ));
   public static Icon RecLIST               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_redDark.ico"  ));
   public static Icon ReportMenu            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_yellow.ico"   ));
   public static Icon Undef                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_blue.ico"         ));
   public static Icon Document              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue.ico"     ));
   public static Icon BlueDown              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue_Down.ico"));
 //public static Icon New                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.new.ico"                      ));
   public static Icon EmptyIcon             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.empty.ico"                    ));
   public static Icon Logo                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.Logo.ico"                     ));
   public static Icon Logo2                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.Logo2.ico"                    ));
   public static Icon Sgn                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.sgn.ico"                      ));
   public static Icon Download              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.download.ico"                 ));
   public static Icon SkySendAdd            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.skySendAdd.ico"               ));        
   public static Icon ChkSND                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.chkSND.ico"                   ));            
   public static Icon LastBlueAdd           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.lastBlueAdd.ico"              ));       
   public static Icon ChkRCV                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.chkRCV.ico"                   ));            
   public static Icon Rename                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.rename.ico"                   ));
 //public static Icon LastRed               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.lastRed.ico"                  )); 
 //public static Icon FirstRed              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.firstRed.ico"                 ));
   public static Icon CheckPrNabCij         = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_yellow.ico"       ));
   public static Icon CheckMSU              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.Admin tools.ico"              ));
   public static Icon FndGreen              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.fndGreen.ico"                 ));
   public static Icon AddRow                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addRow.ico"                   ));
   public static Icon DelRow                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.delRow.ico"                   ));
   public static Icon DelAllRow             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.delAllRow.ico"                ));
   public static Icon SpojiStavke           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addGrid.ico"                  ));
   public static Icon CopyNewTT             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.cpyTo.ico"                    ));
   public static Icon SwitchKnDev           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.dev.ico"                      ));
   public static Icon StartLink             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.link.ico"                     ));
   public static Icon Storno                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.storno.ico"                   ));
   public static Icon Omjer7030             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.7030.ico"                     ));
   public static Icon RewriteAll            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_redGreenico.ico"  ));
   public static Icon Knjizi                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ucitajRacune.ico"             ));
   public static Icon Rasknjizi             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.rasknjizi.ico"                ));
   public static Icon FromUnix              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.fromUnix.ico"                 ));
   public static Icon Settingss             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.settingss.ico"                ));
   public static Icon AvansStorno           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.opomena2.ico"                 ));
   public static Icon PdvOmjer              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.pdvOmjer.ico"                 ));
   public static Icon SpojiPRI              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ufd.ico"                      ));
   public static Icon PrnabCij              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.prNbc.ico"                    ));
   public static Icon Sinteza               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.grupiraj.ico"                 ));
   public static Icon NiceKn                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.money1.ico"                   ));
   public static Icon RbtAllTrans           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.rbt.ico"                      ));
   public static Icon PostaviMPC            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.MPC.ico"                      ));
   public static Icon SpojiUFD              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ufd.ico"                      ));
   public static Icon FiskBlg               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.fiskBlg.ico"                  ));                
   public static Icon FiskOpp               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_blueRedDark.ico"  ));
   public static Icon CisStatus             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ClsStatus.ico"                ));              
   public static Icon SqlSomeCheckQuery     = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_yellow.ico"       ));     
   public static Icon Ruc                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.smileRuc.ico"                 ));
   public static Icon ArtiklInfo            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.money1.ico"                   ));
   public static Icon BarCodeCheck          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.barkod.ico"                   ));
   public static Icon PostaviKol            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addRow.ico"                   ));
   public static Icon IrmRbt                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.rbt.ico"                      ));
   public static Icon RetMoneyCalc          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.calc2.ico"                    ));
   public static Icon FiskParagon           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.kalendar.ico"                 ));
   public static Icon Ppmv                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.noSmilePpmv.ico"              ));
   public static Icon ObrRnp                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.iatp.ico"                     ));
   public static Icon AddPPR                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.cpyToPPR.ico"                 ));
   public static Icon AddPIP                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.cpyToPIP.ico"                 ));
   public static Icon AddArtikl             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addArtikl.ico"                ));
   public static Icon PocStanje             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.pocetnoStanje.ico"            ));
   public static Icon PlusMinus             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.PlusMinus.ico"                ));
   public static Icon Otpad                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.otpad.ico"                    ));
   public static Icon ZpcTH_5               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.money5.ico"                   ));
   public static Icon ZpcTH_2               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.money1.ico"                   ));
   public static Icon IzvOk                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.IzvOk.ico"                    ));
   public static Icon ZPC_akcija            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_square_red.ico"        ));
   public static Icon PreTank               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.PreTank.ico"                  ));
   public static Icon MarkAsSENDed          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_red.ico"          ));
   public static Icon MarkAsRECEIVEd        = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_green.ico"        ));
   public static Icon LogLANADDaction       = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_blue.ico"         ));
   public static Icon UndoReceive_RollItBack= new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_blueRedDark.ico"  ));
   public static Icon UnMarkAsSENDed        = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_redDark.ico"      ));
   public static Icon UnMarkAsRECEIVEd      = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_ball_greenDark.ico"    ));
   public static Icon Sky_forceSend         = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ForceSND.ico"                 ));
   public static Icon Sky_send              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.skySend.ico"                  ));         
   public static Icon Sky_receive           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.skyReceive.ico"               ));
   public static Icon IzvLoad               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.IzvLoad.ico"                  ));
   public static Icon Calc                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.calc.ico"                     ));
   public static Icon Import_nlg            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ucitajOffRn.ico"              ));
   public static Icon Proizvodnja           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.proizvodnja.ico"              ));
   public static Icon ImportLookUp          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.importLookUp.ico"             ));
   public static Icon Excel                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.excel.ico"                    ));
   public static Icon RemovePeople          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.removePeople.ico"             ));
   public static Icon AddPeople             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addPeople.ico"                ));     
   public static Icon CalcBruto             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.calcBruto.ico"                ));     
   public static Icon Temeljnica            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.temeljnica.ico"               ));    
   public static Icon ViewChBoxGrid         = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.viewChBoxGrid.ico"            )); 
   public static Icon AllGrid               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.allGrid.ico"                  ));       
   public static Icon BlueGrid              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.blueGrid.ico"                 ));      
   public static Icon RedGrid               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.redGrid.ico"                  ));       
   public static Icon ResetPlacaGrid        = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.resetPlacaGrid.ico"           ));
   public static Icon GreenGridRvr          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.greenGridRvr.ico"             ));
   public static Icon Lock                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.lock.ico"                     ));
   public static Icon FakturPay             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.fakturPay.ico"                ));
   public static Icon Deviza2               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.deviza2.ico"                  ));
   public static Icon CbuCba                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.CbuCba.ico"                   )); 
   public static Icon ImportExcel           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.importExcel.ico"              ));
   public static Icon Kalendar              = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.kalendar.ico"                 )); 
   public static Icon Co2                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.co2.ico"                      ));   
   public static Icon What                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.What.ico"                     ));     
   public static Icon Auto                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.auto.ico"                     ));
   public static Icon Sorry2                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.sorry2.ico"                   ));
   public static Icon CpyToOrange           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.cpyToBLG.ico"                 ));
   public static Icon RtbNew                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.new.ico"                        ));
   public static Icon RtbOpen               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.rtbx_open.ico"                  ));
   public static Icon RtbDel                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.del.ico"                        ));
   public static Icon OffOpen32             = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.OffOpen.ico"             ), 32, 32);
   public static Icon CloseCurView32        = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Report.closeCurView.ico"        ), 32, 32);
   public static Icon Esc32                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.esc.ico"                 ), 32, 32);
   public static Icon Sav32                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.sav.ico"                 ), 32, 32);
   public static Icon PrintPrw32            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.printPrw.ico"            ), 32, 32);
   public static Icon Sas32                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.sas.ico"                 ), 32, 32);
   public static Icon Attached32            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.attached.ico"            ), 32, 32);
   public static Icon Next32                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.next.ico"                 ), 32, 32);
   public static Icon IzvOk32               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.IzvOk.ico"                ), 32, 32);
   public static Icon Skip32                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.skip.ico"                 ), 32, 32);
   public static Icon UcitajRn32            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.ucitajRacune.ico"         ), 32, 32);
   public static Icon Settingss32           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.settingss.ico"            ), 32, 32);
   public static Icon SendMail32            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.sendMail.ico"             ), 32, 32);
   public static Icon AddPeople32           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.addPeople.ico"            ), 32, 32);
   public static Icon Explorer32            = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.explorer.ico"             ), 32, 32);
   public static Icon TriangleGreenL24      = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_greenLeft.ico"), 24, 24);
   public static Icon TriangleBlue16        = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.bullet_triangle_blue2.ico"), 16, 16);
   public static Icon ExLinkLeft16          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.exLinkLeft.ico"           ), 16, 16);
   public static Icon LinkRight16           = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.linkRight.ico"            ), 16, 16);
   public static Icon Sorry16               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.sorry.ico"                ), 16, 16);
   public static Icon SmileGreen16          = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.smileGreen.ico"           ), 16, 16);
   public static Icon Go2_16                = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.go2.ico"                 ), 16, 16);
   public static Icon Opn2_16               = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Record.opn2.ico"                ), 16, 16);
   public static Icon Del16                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.del.ico"                    ), 16, 16);
   public static Icon New16                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_RTB.new.ico"                    ), 16, 16);
   public static Icon Help                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.help.ico"));

   public static Icon Kn                    = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.Kn.ico"));
   public static Icon Eur                   = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.Eur.ico"));
   public static Icon EurZ                  = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.EurZ.ico"));
   public static Icon KnEur                 = new Icon(GetManifestResourceStream("Vektor.Icons.ToolStrip_Modul.KnEur.ico"));


   #endregion Icon

   #region VvSubMenu

   public static VvSubMenu VvSubMenu_DodajRed      = new VvSubMenu("Dodaj red"         , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "addRow"      ,  true, VvIco.AddRow     , Keys.None, new EventHandler(ZXC.TheVvForm.DocumentUC_InsertRow)         , "Dodaj red"                   );
    public static VvSubMenu VvSubMenu_BrisiRed      = new VvSubMenu("Briši red"         , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "delRow"      ,  true, VvIco.DelRow     , Keys.None, new EventHandler(ZXC.TheVvForm.DocumentUC_DeleteRow)         , "Briši red"                   );
    public static VvSubMenu VvSubMenu_BrisiRedove   = new VvSubMenu("Briši redove"      , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "delAllRow"   ,  true, VvIco.DelAllRow  , Keys.None, new EventHandler(ZXC.TheVvForm.DocumentUC_DeleteManyRows)    , "Briši sve/označene redove"   );
    public static VvSubMenu VvSubMenu_Spoji         = new VvSubMenu("Spoji"             , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "spojiStavke" ,  true, VvIco.SpojiStavke, Keys.None, new EventHandler(ZXC.TheVvForm.TakeGridRowsFromOtherDocument), "Dodaj retke drugog dokumenta");
                                                                                                                                                                   
    public static VvSubMenu VvSubMenu_LGO           = new VvSubMenu("LGO"               , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "lskpl_empty" , false, VvIco.Go         , Keys.None, new EventHandler(ZXC.TheVvForm.LIST_GO)                      , "Description: "               );
    public static VvSubMenu VvSubMenu_FullScreen    = new VvSubMenu("FullScreen"        , ZXC.vvMenuStyleEnum.MenuItemAndButtonReport, "FullScreen"  , false, VvIco.FullScreen , Keys.F11 , new EventHandler(ZXC.TheVvForm.FullScreen_Click)             , "Description: "               );
                                                                                                      
                                                                                                      
    public static VvSubMenu VvSubMenu_KreirajDB     = new VvSubMenu("Kreiraj DB"        , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "ceateDB"     ,  true, VvIco.New        , Keys.None, new EventHandler(ZXC.TheVvForm.PRJ_CreateDB)                 , "Description: "               );
    public static VvSubMenu VvSubMenu_AktivirajDB   = new VvSubMenu("Aktiviraj DB"      , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "activDB"     ,  true, VvIco.New        , Keys.None, new EventHandler(ZXC.TheVvForm.PRJ_ActivateDB)               , "Description: "               );
    public static VvSubMenu VvSubMenu_InitNY        = new VvSubMenu("Init NY"           , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "initNY"      , false, VvIco.FromUnix   , Keys.None, new EventHandler(ZXC.TheVvForm.Init_NY_Click)                , "Description: "               );
    public static VvSubMenu VvSubMenu_AmortNY       = new VvSubMenu("Amort NY"          , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "amortNY"     , false, VvIco.FromUnix   , Keys.None, new EventHandler(ZXC.TheVvForm.Amort_NY)                     , "Description: "               );
    public static VvSubMenu VvSubMenu_RNMNY         = new VvSubMenu("RNM NY"            , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "initRNM"     , false, VvIco.FromUnix   , Keys.None, new EventHandler(ZXC.TheVvForm.RNM_NY)                       , "Prijenops nedovrsenih RNM"   );
    public static VvSubMenu VvSubMenu_OffixImpPrj   = new VvSubMenu("OffixImport"       , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "import_prjkt", false, VvIco.FromUnix   , Keys.None, new EventHandler(ZXC.TheVvForm.ImportFromOffix)              , "Description: "               );
    public static VvSubMenu VvSubMenu_Logo          = new VvSubMenu("Logo"              , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "logo_BLOB"   , false, VvIco.Logo       , Keys.None, new EventHandler(ZXC.TheVvForm.Set_Logo_BLOB)                , "Description: "               );
    public static VvSubMenu VvSubMenu_Logo2         = new VvSubMenu("Logo2"             , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "logo2_BLOB"  , false, VvIco.Logo2      , Keys.None, new EventHandler(ZXC.TheVvForm.Set_Logo2_BLOB)               , "Description: "               );
    public static VvSubMenu VvSubMenu_Fisk          = new VvSubMenu("Fisk"              , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "fiskCert"    , false, VvIco.FiskBlg    , Keys.None, new EventHandler(ZXC.TheVvForm.Set_FiskCert_BLOB)            , "Description: "               );
    public static VvSubMenu VvSubMenu_eSgn          = new VvSubMenu("eSgn"              , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "eSgnCert"    , false, VvIco.Sgn        , Keys.None, new EventHandler(ZXC.TheVvForm.Set_ESgnCert_BLOB)            , "Description: "               );

    public static VvSubMenu VvSubMenu_AddUsrToMySQL = new VvSubMenu("Add Users to MySQL", ZXC.vvMenuStyleEnum.MenuItem_Only          , "btn_usrToSql",  true, VvIco.EmptyIcon  , Keys.None , new EventHandler(ZXC.TheVvForm.AddUsersToMySQL)             , "Description: "               );
    public static VvSubMenu VvSubMenu_EmpyBtn       = new VvSubMenu(" "                 , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "btn_empty"   ,  true, VvIco.EmptyIcon  , Keys.None , null/*new EventHandler()*/                                  , "Description: "               );
    public static VvSubMenu VvSubMenu_Download      = new VvSubMenu("Download"          , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "downloadTL"  , false, VvIco.Download   , Keys.None , new EventHandler(ZXC.TheVvForm.Download_TecajList)          , "Description: "               );

    public static VvSubMenu VvSubMenu_Send          = new VvSubMenu("Send"              , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "sky_send"    , false, VvIco.Sky_send   , Keys.None, new EventHandler(ZXC.TheVvForm.SEND_ToSKY)                   , "Send"                        );
    public static VvSubMenu VvSubMenu_Receive       = new VvSubMenu("Receive"           , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "sky_receive" , false, VvIco.Sky_receive, Keys.None, new EventHandler(ZXC.TheVvForm.RECEIVE_FromSKY)              , "Receive"                     );
    public static VvSubMenu VvSubMenu_ChkSND        = new VvSubMenu("ChkSND"            , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "sky_ChkSND"  , false, VvIco.LastRed    , Keys.None, new EventHandler(ZXC.TheVvForm.Check_SEND_ToSKY)             , "Provjera Send"               );
    public static VvSubMenu VvSubMenu_ChkRCV        = new VvSubMenu("ChkRCV"            , ZXC.vvMenuStyleEnum.MenuItemAndButton      , "sky_ChkRCV"  , false, VvIco.FirstRed   , Keys.None, new EventHandler(ZXC.TheVvForm.Check_RECEIVE_FromSKY)        , "Provjera Receive"            );


    public static VvSubMenu VvSubMenu_PreimenujCD         = new VvSubMenu("PreimenujCD"              , ZXC.vvMenuStyleEnum.MenuItemAndButton, "rename_artiklCd"   , false, VvIco.Rename          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_RenameArtiklCD)                                , "Preimenuj šifru artikla"            );
    public static VvSubMenu VvSubMenu_PreimenujNaziv      = new VvSubMenu("PreimenujNaziv"           , ZXC.vvMenuStyleEnum.MenuItemAndButton, "rename_artiklName" , false, VvIco.Rename          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_RenameArtiklName)                              , "Preimenuj naziv artikla"            );
    public static VvSubMenu VvSubMenu_NewCache            = new VvSubMenu("NewCache"                 , ZXC.vvMenuStyleEnum.MenuItemAndButton, "newCache"          , false, VvIco.Resize          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_NewCache)                                      , "Regeneriraj cashe svih artikala"    );
    public static VvSubMenu VvSubMenu_VvSubMenu_HAL_ExCJ  = new VvSubMenu(ZXC.IsSvDUHdomena ? "HALMED" : "ExtCij", ZXC.vvMenuStyleEnum.MenuItemAndButton, "extCij", false, VvIco.AvansStorno     , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_ShowExternCijena)                              , "Description: "                      );
    
    public static VvSubMenu VvSubMenu_MarkAsSENDed        = new VvSubMenu("MarkAsSENDed"             , ZXC.vvMenuStyleEnum.MenuItemAndButton, "MarkAsSENDed"      , false, VvIco.MarkAsSENDed    , Keys.None, new EventHandler(ZXC.TheVvForm.MarkADDactionAs_SENDed_ToSKY)                       , "MarkAsSENDed"                       );
    public static VvSubMenu VvSubMenu_MarkAsRECEIVEd      = new VvSubMenu("MarkAsRECEIVEd"           , ZXC.vvMenuStyleEnum.MenuItemAndButton, "MarkAsRECEIVEd"    , false, VvIco.MarkAsRECEIVEd  , Keys.None, new EventHandler(ZXC.TheVvForm.MarkADDactionAs_RECEIVEd_FromSKY)                   , "MarkAsRECEIVEd"                     );
    public static VvSubMenu VvSubMenu_LogLAN_ADD_action   = new VvSubMenu("LogLAN ADD action"        , ZXC.vvMenuStyleEnum.MenuItemAndButton, "LogLANADDaction"   , false, VvIco.LogLANADDaction , Keys.None, new EventHandler(ZXC.TheVvForm.ForceADDaction4_LogLAN)                             , "LogLAN ADD action"                  );
    public static VvSubMenu VvSubMenu_Un_MarkAsSENDed     = new VvSubMenu("Un_MarkAsSENDed"          , ZXC.vvMenuStyleEnum.MenuItemAndButton, "UnMarkAsSENDed"    , false, VvIco.UnMarkAsSENDed  , Keys.None, new EventHandler(ZXC.TheVvForm.Un_MarkADDactionAs_SENDed_ToSKY)                    , "UnMarkAsSENDed"                     );
    public static VvSubMenu VvSubMenu_Un_MarkAsDISPATCHed = new VvSubMenu("Un_MarkAsDISPATCHed"      , ZXC.vvMenuStyleEnum.MenuItemAndButton, "UnMarkAsRECEIVEd"  , false, VvIco.UnMarkAsRECEIVEd, Keys.None, new EventHandler(ZXC.TheVvForm.Un_MarkADDactionAs_DISPACHED_ToShop_OrCentr_FromSKY), "UnMarkAsRECEIVEd"                   );
    public static VvSubMenu VvSubMenu_CheckPrNabCij       = new VvSubMenu("CheckPrNabCij"            , ZXC.vvMenuStyleEnum.MenuItemAndButton, "CheckPrNabCij"     , false, VvIco.CheckPrNabCij   , Keys.None, new EventHandler(ZXC.TheVvForm.Manual_CheckCache_CheckPrNabCij_CheckZPC)           , "CheckPrNabCij"                      );
    public static VvSubMenu VvSubMenu_CheckMSU            = new VvSubMenu("CheckMSU"                 , ZXC.vvMenuStyleEnum.MenuItemAndButton, "CheckMSU"          , false, VvIco.CheckMSU        , Keys.None, new EventHandler(ZXC.TheVvForm.Manual_CheckCache_badMSU)                           , ""                                   );
    public static VvSubMenu VvSubMenu_LogLAN_RWT_action   = new VvSubMenu("LogLAN RWT action"        , ZXC.vvMenuStyleEnum.MenuItemAndButton, "sky_forceSend"     , false, VvIco.Sky_forceSend   , Keys.None, new EventHandler(ZXC.TheVvForm.ForceRWTaction4_LogLAN)                             , "LogLAN RWT action"                  );
  //public static VvSubMenu VvSubMenu_Rtrans              = new VvSubMenu("Rtrans"                   , ZXC.vvMenuStyleEnum.MenuItemAndButton, "findSerLot"        , false, VvIco.FndGreen        , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_FindSerLot)                                    , "Rtrans"                             );
    public static VvSubMenu VvSubMenu_NewThisCache        = new VvSubMenu("NewThisCache"             , ZXC.vvMenuStyleEnum.MenuItemAndButton, "newThisCache"      , false, VvIco.ThisCache       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_NewThisCache)                                  , "Regeneriraj cashe samo ovog artikla");
    public static VvSubMenu VvSubMenu_DatabaseService     = new VvSubMenu("Database Service"         , ZXC.vvMenuStyleEnum.MenuItemAndButton, "databaseService"   , false, VvIco.Proizvodnja     , Keys.None, new EventHandler(ZXC.TheVvForm.DatabaseService)                                    , "Database Service"                   );

    public static VvSubMenu VvSubMenu_UndoReceive_RollItBack = new VvSubMenu("UndoReceive_RollItBack", ZXC.vvMenuStyleEnum.MenuItemAndButton, "UndoReceive_RollItBack", false, VvIco.UndoReceive_RollItBack, Keys.None, new EventHandler(ZXC.TheVvForm.UndoReceive_RollItBack), "UndoReceive_RollItBack");


    public static VvSubMenu VvSubMenu_KopirajU            = new VvSubMenu("Kopiraj u"                , ZXC.vvMenuStyleEnum.MenuItemAndButton, "copyNewTT"         , false, VvIco.CopyNewTT       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_CopyToSomeOtherTT)                                          , "Kopiraj u novi dokument drugog tipa"                                        );
    public static VvSubMenu VvSubMenu_KN_Dev              = new VvSubMenu(ZXC.IsEURoERA_projectYear ?"EUR/ Dev": "Kn / Dev", ZXC.vvMenuStyleEnum.MenuItemAndButton, "switchKnDev"       , true , VvIco.SwitchKnDev     , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_ToggleKnDeviza)                       , "Prikaz dokumenta u EUR / devizi"                                         );
    public static VvSubMenu VvSubMenu_StartLink           = new VvSubMenu("StartLink"                , ZXC.vvMenuStyleEnum.MenuItemAndButton, "startLink"         , false, VvIco.StartLink       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_LinkToOtherDocum)                                           , "Poveži sa drugim internim dokumentom"                                       );
    public static VvSubMenu VvSubMenu_Storno              = new VvSubMenu("Storno"                   , ZXC.vvMenuStyleEnum.MenuItemAndButton, "storno"            , false, VvIco.Storno          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Storno)                                                     , "Storno prikazanog dokumenta"                                                );
    public static VvSubMenu VvSubMenu_PodijeliRed         = new VvSubMenu("Podijeli Red"             , ZXC.vvMenuStyleEnum.MenuItemAndButton, "7030"              , true , VvIco.Omjer7030       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_7030)                                                       , "Podijeli red na dva: 30% i 70%"                                             );
    public static VvSubMenu VvSubMenu_RewriteAllDoc       = new VvSubMenu("RewriteAllDoc"            , ZXC.vvMenuStyleEnum.MenuItemAndButton, "rewriteAll"        , false, VvIco.RewriteAll      , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_RewriteAllDocuments)                                        , "Rewrite All Documents"                                                      );
    public static VvSubMenu VvSubMenu_Proknjizi           = new VvSubMenu("Proknjiži"                , ZXC.vvMenuStyleEnum.MenuItemAndButton, "knjizi"            , false, VvIco.Knjizi          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_LoadSingleFak2Nal)                                          , "Proknjiži ovaj dokument"                                                    );
    public static VvSubMenu VvSubMenu_PobrisiKnjizenja    = new VvSubMenu("PobrišiKnjiženja"         , ZXC.vvMenuStyleEnum.MenuItemAndButton, "rasknjizi"         , false, VvIco.Rasknjizi       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_UndoFak2Nal)                                                , "Pobriši sva knjiženja vezana uz ovaj dokument"                              );
  //public static VvSubMenu /*VvSubMenu_OffixImport*/     = new        VvSubMenu("OffixImport"              , ZXC.vvMenuStyleEnum.MenuItemAndButton, "import_rn"         , false, VvIco.FromUnix        , Keys.None, new EventHandler(              ImportFromOffix)                                                 , "ImportFromOffix"),                                                          
    public static VvSubMenu VvSubMenu_AvansST             = new VvSubMenu("AvansST"                  , ZXC.vvMenuStyleEnum.MenuItemAndButton, "avansStorno"       , false, VvIco.AvansStorno     , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_AvansStorno)                                                , "Napravi novi dokument sa stornom ovog dokumenta za predujam"                );
    public static VvSubMenu VvSubMenu_OmjerPdv            = new VvSubMenu("OmjerPdv"                 , ZXC.vvMenuStyleEnum.MenuItemAndButton, "pdvOmjer"          , true , VvIco.PdvOmjer        , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_PdvOmjer)                                                   , "Podijeli red u omjeru priznatog PDV-a"                                      );
    public static VvSubMenu VvSubMenu_PRI                 = new VvSubMenu("PRI"                      , ZXC.vvMenuStyleEnum.MenuItemAndButton, "spojiPRI"          , true , VvIco.SpojiPRI        , Keys.None, new EventHandler(ZXC.TheVvForm.TakeRowsFromManyDocumentsPRIorIZD_byTtDokDateKupdobCD_toUFAorIFA), "Učitaj stavke primki"                                                       );
    public static VvSubMenu VvSubMenu_ISP                 = new VvSubMenu("ISP"                      , ZXC.vvMenuStyleEnum.MenuItemAndButton, "copyToISP"         , false, VvIco.CpyToOrange     , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Copy_UFA_to_ISP)                                            , "Kreiraj Bagajna Isplatnicu na osnovu ovog dokumenta"                        );
    public static VvSubMenu VvSubMenu_Pravila_Fak         = new VvSubMenu("Pravila"                  , ZXC.vvMenuStyleEnum.MenuItemAndButton, "fakPravila"        , false, VvIco.Settingss       , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_FakturRules)                                                , "Pravila"                                                                    );
    public static VvSubMenu VvSubMenu_PrNabCij            = new VvSubMenu("PrNabCij"                 , ZXC.vvMenuStyleEnum.MenuItemAndButton, "prnabCij"          , true , VvIco.PrnabCij        , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_PrNabCij)                                                   , "Prosječna nabavna cijena"                                                   );
    public static VvSubMenu VvSubMenu_Popratnice          = new VvSubMenu("Popratnice"               , ZXC.vvMenuStyleEnum.MenuItemAndButton, "popratnice"        , true , VvIco.ToXml           , Keys.None, new EventHandler(ZXC.TheVvForm.LoadPopratnica)                                                  , "Učitaj popratnice"                                                          );
    public static VvSubMenu VvSubMenu_Grupiraj            = new VvSubMenu("Grupiraj"                 , ZXC.vvMenuStyleEnum.MenuItemAndButton, "sinteza"           , true , VvIco.Sinteza         , Keys.None, new EventHandler(ZXC.TheVvForm.CalcVolumen_SynthesizeG2toG1)                                    , "Sumiraj i sintetiziraj podatke Tabilce dva (Detalji) i prikaži na Tablici 1");
    public static VvSubMenu VvSubMenu_NiceKn              = new VvSubMenu("NiceKn"                   , ZXC.vvMenuStyleEnum.MenuItemAndButton, "niceKn"            , true , VvIco.NiceKn          , Keys.Control | Keys.Subtract, new EventHandler(ZXC.TheVvForm.UFD_NiceKn)                                   , "Nice Kn"                                                                    );
    public static VvSubMenu VvSubMenu_Rabat               = new VvSubMenu("Rabat"                    , ZXC.vvMenuStyleEnum.MenuItemAndButton, "rbtAllTrans"       , true , VvIco.RbtAllTrans     , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_RBT_4Q)                                                     , "Ispuni sve stavke sa određenim postotkom rabata"                            );
    public static VvSubMenu VvSubMenu_PostaviMPC          = new VvSubMenu("PostaviMPC"               , ZXC.vvMenuStyleEnum.MenuItemAndButton, "postaviMPC"        , true , VvIco.PostaviMPC      , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_PostaviMPC)                                                 , "Postavi MPC"                                                                );

    public static VvSubMenu VvSubMenu_FISK                = new VvSubMenu("FISK"                     , ZXC.vvMenuStyleEnum.MenuItemAndButton, "fisk"             , false, VvIco.FiskBlg          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Fiskalize_RACUN)                                            , "Fiskalizacija"                                                              );
    public static VvSubMenu VvSubMenu_FISKopp             = new VvSubMenu("FISKopp"                  , ZXC.vvMenuStyleEnum.MenuItemAndButton, "fiskOpp"          , false, VvIco.FiskOpp          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Fiskalize_PoslovniProstor)                                  , "Fiskaliziraj poslovni prostor"                                              );
    public static VvSubMenu VvSubMenu_CisStatus           = new VvSubMenu("Cis Status"               , ZXC.vvMenuStyleEnum.MenuItemAndButton, "cisStatus"        , false, VvIco.CisStatus        , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Fiskalize_GetCISserviceStatus)                              , "Cis Status"                                                                 );
    public static VvSubMenu VvSubMenu_FiskSveNeFisk       = new VvSubMenu("FiskSveNeFisk"            , ZXC.vvMenuStyleEnum.MenuItemAndButton, "fiskAll"          , false, VvIco.FiskBlg          , Keys.None, new EventHandler(ZXC.TheVvForm.RISK_Fiskalize_AllPreviously_NOTfiskalized)                      , "Fiskaliziraj sve nefiskalizirane"                                           );


    #region ImportOffix provjeriti treba
    public static VvSubMenu VvSubMenu_OffixImport_Skl = new VvSubMenu("OffixImport", ZXC.vvMenuStyleEnum.MenuItemAndButton, "import_skl", false, VvIco.FromUnix, Keys.None, new EventHandler(ZXC.TheVvForm.ImportFromOffix), "Description: ");
  //public static VvSubMenu VvSubMenu_OffixImport     = new VvSubMenu("OffixImport"       , ZXC.vvMenuStyleEnum.MenuItemAndButton, "import_rn"  , false, VvIco.FromUnix   , Keys.None, new EventHandler(ImportFromOffix)              , "ImportFromOffix"),

    #endregion ImportOffix provjeriti treba

    #endregion VvSubMenu

}
