using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;
using System.Collections.Generic;
using System.Linq;

#if MICROSOFT
using XSqlConnection = System.Data.SqlClient.SqlConnection;
#else
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
#endif

// 01.09.2021: VvKristal-NEW je Windows 10 sa MS Office 2016 pa ovo ne moze ...
//using Excel = Microsoft.Office.Interop.Excel; // VOILA 

using System.IO;
using System.Xml;
using Raverus.FiskalizacijaDEV;
using Raverus.FiskalizacijaDEV.Schema;
using System.Xml.Serialization;

using System.Net;
using Org.BouncyCastle.Math.EC;
using static ArtiklDao;

public /*sealed*/ partial class VvForm : DevExpress.XtraEditors.XtraForm
{
   #region Prjkt

   /*protected*/ public void PRJ_ActivateDB(object sender, EventArgs e)
   {
      ((PrjktUC)TheVvUC).ActivateDB_OnClick(sender, e);
   }

    /*protected*/
    public void PRJ_CreateDB(object sender, EventArgs e)
   {
      ((PrjktUC)TheVvUC).CreateDB_OnClick(ZXC.projectYear, e);
   }

   /*private*/ public void Download_TecajList(object sender, EventArgs e)
   {
      DownLoadOnDateDLG dlg = new DownLoadOnDateDLG();

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      dlg.Dispose();
   }

   // OVO SE RADI U STAROJ GODINI!
   /*protected*/ public void Init_NY_Click(object sender, EventArgs e)
   {
      Init_NY_JOB("init", e);
   }

   /*protected*/ public void Init_NY_JOB(object sender, EventArgs e)
   {

      #region 1. CREATE SCHEMA IN NEW YEAR

      bool isForAmort = sender.ToString() == "amort";

      Prjkt prjkt_rec = TheVvDataRecord as Prjkt;

      int  nora=-1;
      bool pgTableExists, OK;

      uint thisYear = ZXC.ValOrZero_UInt(ZXC.projectYear);
      uint nextYear = thisYear + 1;
      string thisYearStr = thisYear.ToString();
      string nextYearStr = nextYear.ToString();

      bool nyLogLanCreated = false;

      if(isForAmort == false) OK = ((PrjktUC)TheVvUC).CreateDB(nextYearStr, e);
      else                    OK = true;

      // 13.12.2022: vratili da ipak NE ode dalje ako ny db postoji tako da se Init_NY_JOB NE MOŽE DVAPUT ... mora se prvo pobrisati 'NY DB' 
      //if(!OK) /*return*/; // 06.12.2011: neka ode dalje i ako dbPostoji 
      if(!OK)
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Init_NY se ne može izvoditi dvaput. Kontaktirajte support ukoliko treba ponoviti Init_NY proces.");
         return;
      }

      #endregion 1. CREATE SCHEMA IN NEW YEAR

      #region 2. FOREACH USED TABLE IN THIS YEAR CREATE TABLE IN NEW YEAR

      string thisYearDbName = ZXC.VvDB_NameConstructor(thisYearStr, prjkt_rec.Ticker, prjkt_rec.KupdobCD);
      string nextYearDbName = ZXC.VvDB_NameConstructor(nextYearStr, prjkt_rec.Ticker, prjkt_rec.KupdobCD);

      XSqlConnection tmpNextYearDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, nextYearDbName);

      #region init vvlogLAN
      // 29.12.2023: 
      // ovo je sad tu, a ne ono dole niže
      OK = VvSQL.CREATE_TABLE(tmpNextYearDbConnection, nextYearDbName, ZXC.vvDB_LANlogTableName, false);

      #endregion init vvlogLAN

      foreach(string vvDataRecordName in ZXC.VvPUG_RecordNamesList)
      {
         if(isForAmort == true) // Ako je za amort preskoci kplan, person, artikl, ...
         {
            if(!vvDataRecordName.StartsWith(Osred .recordName) && 
               !vvDataRecordName.StartsWith(Amort .recordName) && 
               !vvDataRecordName.StartsWith(Atrans.recordName)) continue;
         }
         else // Ako je za init sifrare u ny, preskoci amort, atrans, osred 
         {
            if(vvDataRecordName.StartsWith(Osred .recordName) || 
               vvDataRecordName.StartsWith(Amort .recordName) || 
               vvDataRecordName.StartsWith(Atrans.recordName)) continue;
         }

         pgTableExists = VvSQL.CHECK_TABLE_EXISTS(TheDbConnection, thisYearDbName, vvDataRecordName);

         if(pgTableExists)
         {
            OK = VvSQL.CREATE_TABLE(tmpNextYearDbConnection, tmpNextYearDbConnection.Database, vvDataRecordName, /*vvDataRecordName == ArtStat.recordName ? true :*/ false);

            if(!OK && VvDaoBase.CountAllRecords(tmpNextYearDbConnection, vvDataRecordName) > 0 ) continue;

            #region 3. COPY ALL SIFRARS ROWS THIS YEAR --> NEXT YEAR

            if(vvDataRecordName.StartsWith(Artikl.recordName) ||  // da idu i arhive 
               vvDataRecordName.StartsWith(Kplan .recordName) ||
               vvDataRecordName.StartsWith(Person.recordName) ||
               vvDataRecordName.StartsWith(Osred .recordName) ||
               vvDataRecordName.StartsWith(Amort .recordName) || // !!! ovo nisu sifrari 
               vvDataRecordName.StartsWith(Atrans.recordName) || // !!! ovo nisu sifrari 
               vvDataRecordName.StartsWith(Kupdob.recordName))
            {

               VvDaoBase.COPY_TABLE(TheDbConnection, nextYearDbName, vvDataRecordName, thisYearDbName, vvDataRecordName, out nora);

               if(ZXC.IsTEXTHOcentrala && ZXC.projectYearAsInt == 2022 && vvDataRecordName == Artikl.recordName) // this 2022 year only, create new TH euro artikl sifrar in 2023 
               {
                  XSqlConnection tmpThisYearDbConnection = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, thisYearDbName);

                              TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);
                  /* ALFA  */ TH_New_EURO_ArtiklSifrar_Genesis(tmpThisYearDbConnection                                                                       ); // u 2022          
                              TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);
                  
                  /* BETA  */ TH_RWTREC_OldNiceKunaArtikls	   (tmpThisYearDbConnection                                                                      ); // u 2022          
                              
                  /* GAMA  */ TH_DELREC_NewNiceEuroArtikls     (tmpThisYearDbConnection                                                                      ); // u 2022          


                              VvSQL.CREATE_TABLE(tmpNextYearDbConnection, tmpNextYearDbConnection.Database, ZXC.vvDB_LANlogTableName, false);


                              ZXC.CopyOut_InProgress =  true;
                  /* DELTA */ TH_DELREC_OldNiceKunaArtikls_2023(tmpNextYearDbConnection                                                                      ); // u 2023 ! nyConn 

                  /* KAPA  */ //TH_New_EURO_ArtiklSifrar_Genesis(tmpNextYearDbConnection                                                                       ); // u 2023 ! nyConn 
                              ZXC.CopyOut_InProgress = false;

                  tmpThisYearDbConnection.Close();

               } // if(ZXC.IsTEXTHOcentrala && ZXC.projectYearAsInt == 2022 && vvDataRecordName == Artikl.recordName) // this 2022 year only, create new TH euro artikl sifrar in 2023 
            }

            #endregion 3. COPY ALL SIFRARS ROWS THIS YEAR --> NEXT YEAR

            #region 4. IF Exists COPY Placa for MMYYYY Decembar

          //if(vvDataRecordName == Ptrano.recordName                                )
            if(vvDataRecordName == Ptrano.recordName && ZXC.projectYearAsInt != 2022) // u godini prelaska na euro, decembar placa ide samo sa copy out, kasnije opet na stari nacin: ili - ili  
            {
               OK = ZXC.PlacaDao.COPY_DECEMBAR_PLACA_TABLE(TheDbConnection, nextYearDbName, thisYearDbName, out nora);

               if(OK && nora.NotZero()) ZXC.aim_emsg(MessageBoxIcon.Information, "Preporuča se da na prebačenoj plaći ručno ispravite broj dokumenta kao i rbr po tipu transakcije na '0' (nulu).");
            }

            #endregion 4. IF Exists COPY Placa for MMYYYY Decembar

            #region 5. IF Exists Placa Fill ZXC.luiListaFondSati_NOR & ZXC.luiListaFondSati_TRG for NewYear Hours

            if(vvDataRecordName == Placa.recordName)
            {
               // dodaj nove Euro rulse ... START                                                                                                                                         

               ZXC.luiListaPRules.LazyLoad();

               bool nekiVecpostoji = ZXC.luiListaPRules.Any(pr => pr.DateT == ZXC.EURoERAstart);

               if(ZXC.projectYearAsInt == 2022 && nekiVecpostoji == false) // dodaj nove Euro rulse 
               {
                  VvLookUpItem newEuroPlacaRuleLUI;
                  VvLookUpItem oldKunePlacaRuleLUI;
                  string prLuiCd;

                  DateTime dateTime4lui;

                  prLuiCd = "OsnOdb";
                  dateTime4lui = ZXC.EURoERAstart;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "ProsPlaca";
                  dateTime4lui = ZXC.Date01012022;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "MinMioOsn";
                  dateTime4lui = ZXC.Date01012022;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "MaxMioOsn";
                  dateTime4lui = ZXC.Date01012022;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "OsnDopClUp";
                  dateTime4lui = ZXC.Date01012022;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "MaxPorOsn1";
                  dateTime4lui = ZXC.EURoERAstart;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, ZXC.EURiIzKuna_HRD_(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "MaxPorOsn2";
                  dateTime4lui = ZXC.EURoERAstart;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, /*ZXC.EURiIzKuna_HRD_*/(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }

                  prLuiCd = "MaxPorOsn3";
                  dateTime4lui = ZXC.EURoERAstart;
                  oldKunePlacaRuleLUI = ZXC.luiListaPRules.OrderBy(prlui => prlui.DateT).LastOrDefault(prlui => prlui.Cd == prLuiCd);
                  if(oldKunePlacaRuleLUI != null)
                  {
                     newEuroPlacaRuleLUI = new VvLookUpItem(oldKunePlacaRuleLUI.Cd + "_EUR", oldKunePlacaRuleLUI.Name, /*ZXC.EURiIzKuna_HRD_*/(oldKunePlacaRuleLUI.Number), oldKunePlacaRuleLUI.Flag, oldKunePlacaRuleLUI.Integer, dateTime4lui, 0, "");
                     ZXC.luiListaPRules.Add(newEuroPlacaRuleLUI);
                  }
                  
                  VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaPRules);

               } // if(ZXC.projectYearAsInt == 2022) // dodaj nove Euro rulse 

               // dodaj nove Euro rulse ...  END                                                                                                                                          

               ZXC.luiListaFondSati_NOR.LazyLoad(); 
               ZXC.luiListaFondSati_TRG.LazyLoad(); 

               string nyMMYYYY;
               VvLookUpItem lui;
               bool touchedNOR = false;
               bool touchedTRG = false;
               string radniSati;
               decimal YYYYMM;

               for(int month = 1; month <= 12; ++month)
               {
                  nyMMYYYY = month.ToString("00") + nextYearStr;

                  // NORMALNI ___ 
                  lui = ZXC.luiListaFondSati_NOR.SingleOrDefault(item => item.Cd == nyMMYYYY);

                  if(lui == null)
                  {
                     touchedNOR = true;
                     radniSati  = GetRadniSati(false, nyMMYYYY, (int)nextYear, month);
                     YYYYMM     = ZXC.ValOrZero_Decimal(nextYearStr + month.ToString("00"), 2);

                     ZXC.luiListaFondSati_NOR.Add(new VvLookUpItem(nyMMYYYY, radniSati, YYYYMM, false, 0, DateTime.Now, 0, ""));
                  }

                  // TRGOVCI ___ 
                  lui = ZXC.luiListaFondSati_TRG.SingleOrDefault(item => item.Cd == nyMMYYYY);

                  if(lui == null)
                  {
                     touchedTRG = true;
                     radniSati  = GetRadniSati(true, nyMMYYYY, (int)nextYear, month);
                     YYYYMM     = ZXC.ValOrZero_Decimal(nextYearStr + month.ToString("00"), 2);

                     ZXC.luiListaFondSati_TRG.Add(new VvLookUpItem(nyMMYYYY, radniSati, YYYYMM, false, 0, DateTime.Now, 0, ""));
                  }
               }

               if(touchedNOR) VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaFondSati_NOR);
               if(touchedTRG) VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaFondSati_TRG);

            } // if(vvDataRecordName == Placa.recordName) 

            #endregion 5. IF Exists Placa Fill ZXC.luiListaFondSati_NOR & ZXC.luiListaFondSati_TRG for NewYear Hours

            // novo za prelazak 2023 ---> 2024 
            #region 6. IF Exists COPY Mixer for TT_KDC

            if(vvDataRecordName == Xtrano.recordName)
            {
               OK = ZXC.PlacaDao.COPY_KDC_MIXER_TABLE(TheDbConnection, nextYearDbName, thisYearDbName, out nora);

               if(OK && nora.NotZero()) ZXC.aim_emsg(MessageBoxIcon.Information, "Prenio {0} KDC kompleta.", nora);
            }

            #endregion 6. IF Exists COPY Mixer for TT_KDC

            // novo za prelazak 2023 ---> 2024 
            #region 7. IF Exists COPY Mixer for TT_RASTERxy

            if(vvDataRecordName == Xtrano.recordName)
            {
               OK = ZXC.PlacaDao.COPY_RASTER_MIXER_TABLE(TheDbConnection, nextYearDbName, thisYearDbName, out nora);

               if(OK && nora.NotZero()) ZXC.aim_emsg(MessageBoxIcon.Information, "Prenio {0} Raster dokumenata.", nora);
            }

            #endregion 7. IF Exists COPY Mixer for TT_RASTERxy

            // novo za prelazak 2023 ---> 2024 
            #region 8. On TEXTHO Centrala CopyOUT 0ZPC
            
            if(vvDataRecordName == Rtrano.recordName && ZXC.IsTEXTHOcentrala)
            {
               ZXC.CopyOut_InProgress = true;

               OK = ZXC.FakturDao.COPY_0ZPC_FAKTUR_TABLE(thisYearDbName, tmpNextYearDbConnection, out nora);

               ZXC.CopyOut_InProgress = false;

               if(OK && nora.NotZero()) ZXC.aim_emsg(MessageBoxIcon.Information, "Prenio {0} Nultih ZPC-ova.", nora);
            }

            #endregion 8. On TEXTHO Centrala CopyOUT 0ZPC

         } // if(pgTableExists)

      } // foreach(string vvDataRecordName in ZXC.VvPUG_RecordNamesList) 

      #endregion 2. FOREACH USED TABLE IN THIS YEAR CREATE TABLE IN NEW YEAR

      // 14.12.2015: init vvlogLAN 
      // 29.12.2023: preselio ranije
    //#region init vvlogLAN
    //
    //OK = VvSQL.CREATE_TABLE(tmpNextYearDbConnection, nextYearDbName, ZXC.vvDB_LANlogTableName, false);
    //
    //#endregion init vvlogLAN

      // 17.02.2016: init YearDependent LookupLists 
      string thePG_LuiTable = ZXC.luiListaPozicijePlana.TableName;
      string theNY_LuiTable = ZXC.vvDB_luiPrefix + ZXC.luiListaPozicijePlana_Name + "_" + nextYearStr;

      // 30.03.2017: 
      bool nyPlanTableExists = VvSQL.CHECK_TABLE_EXISTS(ZXC.PrjConnection, ZXC.PrjConnection.Database, theNY_LuiTable);
      if(nyPlanTableExists)
      {
       //DialogResult result = MessageBox.Show("Da li zaista zelite klonirati plan prosle u sljedeću godinu?!",
       //   "PLAN NOVE GODINE POSTOJI!?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
       //
       //if(result == DialogResult.Yes)
       //{
       //   OK = VvDaoBase.CLONE_TABLE(ZXC.PrjConnection, thePG_LuiTable, theNY_LuiTable, out nora);
       //}
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Pozicije plana nece biti klonirane jer u novoj godini vec postoje.");
      }
      else
      {
         OK = VvDaoBase.CLONE_TABLE(ZXC.PrjConnection, thePG_LuiTable, theNY_LuiTable, out nora);
      }

      #region HRD 2022 ---> 2023 ONLY!

      if(ZXC.projectYearAsInt == 2022)
      {
         int debugCount = 0;
         string origDatabase = tmpNextYearDbConnection.Database;

         if(isForAmort == false) // konvertiraj sve artikle, sve kupdobe ali NE sve nego samo ovaj jedan Prjkt 
         {
            if(ZXC.IsTEXTHOany == false)
            {
               VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Artikl>(tmpNextYearDbConnection, VvDataRecord.Convert_Kuna_To_Euro_ForAllMoneyPropertiez, null, "", tmpNextYearDbConnection.Database, out debugCount);
               VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Kupdob>(tmpNextYearDbConnection, VvDataRecord.Convert_Kuna_To_Euro_ForAllMoneyPropertiez, null, "", tmpNextYearDbConnection.Database, out debugCount);
            }

            #region this prjkt_rec ONLY!       
            
            tmpNextYearDbConnection.ChangeDatabase(ZXC.VvDB_prjktDB_Name);

            VvDataRecord arhivedPrjkt_rec = prjkt_rec.CreateArhivedDataRecord(TheDbConnection, "Init_NY");

            BeginEdit(prjkt_rec);

            prjkt_rec.Convert_Kuna_To_Euro_ForAllMoneyPropertiez_JOB/*<Prjkt>*/(tmpNextYearDbConnection);

            OK = TheVvDao.RWTREC(tmpNextYearDbConnection, prjkt_rec);

            if(!OK) { CancelEdit(prjkt_rec); return; }

            EndEdit(prjkt_rec);

            arhivedPrjkt_rec.VvDao.ADDREC(tmpNextYearDbConnection, arhivedPrjkt_rec, true, true, false, false);

            PutFieldsActions(TheDbConnection, prjkt_rec, TheVvRecordUC);

            tmpNextYearDbConnection.ChangeDatabase(origDatabase);

            #endregion this prjkt_rec ONLY!                                                                                                                                          

            ZXC.aim_emsg(MessageBoxIcon.Information, "Konvertirao novčane podatke iz kuna u euro za datoteke - šifarnike: artikli, partneri i ovaj projekt.");
         }

         if(isForAmort == true) // konvertiraj sve atransove 
         {
            VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Atrans>(tmpNextYearDbConnection, VvDataRecord.Convert_Kuna_To_Euro_ForAllMoneyPropertiez, null, "", tmpNextYearDbConnection.Database, out debugCount);

            ZXC.aim_emsg(MessageBoxIcon.Information, "Konvertirao iz kuna u euro: dug i pot amort dokumenata.");
         }

      } // if(ZXC.projectYearAsInt == 2022) 

      #endregion HRD 2022 ---> 2023 ONLY!

      tmpNextYearDbConnection.Close();

      if(isForAmort == true) ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Prenesene datoteke Dugotrajne imovine.");
      else                   ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Prenesene datoteke artikala, kontni plan, djelatnici i partneri.");
   }

   private string GetRadniSati(bool isTRG, string nyMMYYYY, int year, int month)
   {
      decimal workHoursNum;
      string  workHoursStr;
      uint lastDay = (uint)DateTime.DaysInMonth(year, month);

      workHoursNum = ZXC.GetWorkHoursCount(isTRG, false, nyMMYYYY, 1, lastDay, Placa.SluzbeniDnevniFondSati);

      workHoursStr = workHoursNum.ToString0Vv();
      return workHoursStr;
   }

   // OVO SE RADI U STAROJ GODINI!
   /*protected*/ public void Amort_NY(object sender, EventArgs e)
   {
      Init_NY_JOB("amort", e);
   }

   /*private*/public void AddUsersToMySQL(object sender, EventArgs e)
   { 
   }

   /*protected*/ public void Set_Logo_BLOB    (object sender, EventArgs e) { SetSomeBLOB_JOB("theLogo"     , false); }
   /*protected*/ public void Set_Logo2_BLOB   (object sender, EventArgs e) { SetSomeBLOB_JOB("theLogo2"    , false); }
   /*protected*/ public void Set_FiskCert_BLOB(object sender, EventArgs e) { SetSomeBLOB_JOB("certFile"    , true ); }
   /*protected*/ public void Set_ESgnCert_BLOB(object sender, EventArgs e) { SetSomeBLOB_JOB("eSgnCertFile", true ); }

   private void SetSomeBLOB_JOB(string blobColName, bool isCert)
   {
      #region FileDialog

      OpenFileDialog openFileDialog = new OpenFileDialog();

      openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName;

      //openFileDialog.Filter = "Pictures (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
      //openFileDialog.Filter = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";

      if(isCert) openFileDialog.Filter = "Certificate files (*.pfx, *.p12)|*.pfx;*.p12|All files (*.*)|*.*";
      else       openFileDialog.Filter =     "Picture files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";

      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;

      openFileDialog.ShowReadOnly     = true;


      if(openFileDialog.ShowDialog() != DialogResult.OK)
      {
         openFileDialog.Dispose(); // !!! 
         return;
      }

      string fullPathName = openFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathName);

      string fileName = dInfo.Name;
      string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));
      ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName = directoryName;

      #region Read byte array from FileStream

      byte[] rawData = null;

      if(openFileDialog.ReadOnlyChecked == false) // trik za brisanje blob-a. dAKLE AKO CHECKIRAS READoNLY BLOB CE BITI POBRISAN 
      {
         FileStream fs = new FileStream(fullPathName, FileMode.Open, FileAccess.Read);
         int FileSize = (int)fs.Length;

         rawData = new byte[FileSize];
         fs.Read(rawData, 0, FileSize);
         fs.Close();
      }

      #endregion Read byte array from FileStream

      openFileDialog.Dispose(); // !!! 

      #endregion FileDialog

      Prjkt prjkt_rec = (TheVvDataRecord as Prjkt);

      VvDaoBase.Rwtrec_BLOBsingleColumn(TheDbConnection, prjkt_rec, blobColName/*"theLogo2"*/, rawData);

      ReRead_OnClick(null, EventArgs.Empty);

      ZXC.CURR_prjkt_rec = ((Prjkt)TheVvDataRecord).MakeDeepCopy();
   }

   /*protected*/ public void RNM_NY(object sender, EventArgs e)
   {
      #region Init

      DialogResult result = MessageBox.Show("Da li zaista zelite prenjeti nezatvorene RNM radne naloge u sljedeću godinu?!",
         "Potvrdite RNM PRIJENOS?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      Prjkt prjkt_rec = TheVvDataRecord as Prjkt;

      uint thisYear      = ZXC.ValOrZero_UInt(ZXC.projectYear);
      uint nextYear      = thisYear + 1;
      string thisYearStr = thisYear.ToString();
      string nextYearStr = nextYear.ToString();

      string thisYearDbName = ZXC.VvDB_NameConstructor(thisYearStr, prjkt_rec.Ticker, prjkt_rec.KupdobCD);
      string nextYearDbName = ZXC.VvDB_NameConstructor(nextYearStr, prjkt_rec.Ticker, prjkt_rec.KupdobCD);

      XSqlConnection thisYearDbConn = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, thisYearDbName);
      XSqlConnection nextYearDbConn = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, nextYearDbName);

      int debugCount = 0;

      Cursor.Current = Cursors.WaitCursor;

      #endregion Init

      #region Get rnm_NotR_FakturList

      List<Faktur> rnm_NotR_FakturList = new List<Faktur>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], "theTT"    , Faktur.TT_RNM, " = " ));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.statusCD], "theStatus", "R"          , " != "));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(thisYearDbConn, rnm_NotR_FakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      rnm_NotR_FakturList.ForEach(fak => fak.VvDao.LoadTranses(thisYearDbConn, fak, false));

      #endregion Get rnm_NotR_FakturList

      #region GetRtransList_ForRNM And ExecuteMassCopy OUT OfRecords

      List<Rtrans> rtransList_ForProjektCD;
      decimal ukFinIzlaz   ;
      decimal ukKol_OP_Ulaz;
      bool   OK            ;

      ZXC.CopyOut_InProgress = true;

      foreach(Faktur rnmFaktur_rec in rnm_NotR_FakturList)
      {
         #region Set Decimal01 & 02

         rtransList_ForProjektCD = RtransDao.GetRtransList_ForProjektCD(thisYearDbConn, rnmFaktur_rec./*ProjektCD*/TT_And_TtNum, Rtrans.artiklOrderBy_ASC);

         ukFinIzlaz    = rtransList_ForProjektCD.Where(rtr => rtr.T_TT == Faktur.TT_PPR).Sum(rtr => rtr.R_KC   ); // PIZ: faktur_rec.TrnSum_KCR       ; 
         ukKol_OP_Ulaz = rtransList_ForProjektCD.Where(rtr => rtr.T_TT == Faktur.TT_PIP).Sum(rtr => rtr.R_kolOP); // PIZ: faktur_rec.TrnSum_K_PULX_ALL; 

         rnmFaktur_rec.Decimal01 += ukFinIzlaz   ; // += jer tu vec moze biti nesto iz neke pgpg godine 
         rnmFaktur_rec.Decimal02 += ukKol_OP_Ulaz; // += jer tu vec moze biti nesto iz neke pgpg godine 

         #endregion Set Decimal01 & 02

         #region ExecuteMassCopy OUT OfRecords

         OK = VvRecLstUC.EnsureNonDuplicateKeys(nextYearDbConn, rnmFaktur_rec, /*weWantOrigDokNum*/ false, /*weWantOrigTtNum*/ true);

         rnmFaktur_rec.Transes.ForEach(trn => trn.T_dokNum = rnmFaktur_rec.DokNum);

         OK = ZXC.FakturDao.ADDREC(nextYearDbConn, rnmFaktur_rec, true, false, false, false); // VOILA! 

         if(OK) debugCount++;

         #endregion ExecuteMassCopy OUT OfRecords

      } // foreach(Faktur rnmFaktur_rec in rnm_NotR_FakturList) 

      #endregion GetRtransList_ForRNM And ExecuteMassCopy OUT OfRecords

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.CopyOut_InProgress = false;

      thisYearDbConn.Close();
      nextYearDbConn.Close();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Prenio {0} RNM dokumenta.", debugCount);

      #endregion Finish
   }

   public void After_Init_NY(object sender, EventArgs e)
   {
      // OVO SE RADI U NOVOJ GODINI! 

      #region Init

      DialogResult result = MessageBox.Show("Da li zaista zelite prenjeti promjene šifrara nakon Init_NY operacije iz prošle u ovu godinu?!",
         "Potvrdite After_Init_NY PRIJENOS?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      string tableName = TheVvDataRecord.VirtualRecordName;

      uint thisYear = ZXC.ValOrZero_UInt(ZXC.projectYear);
      uint prevYear = thisYear - 1;
      string thisYearStr = thisYear.ToString();
      string prevYearStr = prevYear.ToString();

      string thisYearDbName = ZXC.VvDB_NameConstructor(thisYearStr, ZXC.CURR_prjkt_rec.Ticker, ZXC.CURR_prjkt_rec.KupdobCD);
      string prevYearDbName = ZXC.VvDB_NameConstructor(prevYearStr, ZXC.CURR_prjkt_rec.Ticker, ZXC.CURR_prjkt_rec.KupdobCD);

      XSqlConnection thisYearDbConn = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, thisYearDbName);
      XSqlConnection prevYearDbConn = VvSQL.CREATE_AND_OPEN_XSqlConnection(ZXC.vvDB_Server, ZXC.vvDB_User, ZXC.vvDB_Password, prevYearDbName);

      int debugCount = 0;

      Cursor.Current = Cursors.WaitCursor;

      #endregion Init

      #region Voila 

      DateTime nyTableCreationTime = VvDaoBase.Get_TABLE_Create_Time(thisYearDbConn, tableName);

      DateTime afterThisTimeStamp = nyTableCreationTime.AddMinutes(1); // jer imas BUG da RWT kune u euro za 2023 ode u vvLogLan od 2022 umjesto od 2023 pa s ovime preskaces te RWT-ove 

      List<VvSQL.VvLanLogEntry> vvLanLogList = VvDaoBase.Get_AfterInitNY_LANchanges_LogList(prevYearDbConn, /*nyTableCreationTime*/afterThisTimeStamp, tableName);

      List<ZXC.DBactionForSrvRecID> recIDactions = null;

      recIDactions = VvSkyLab.Setup_AfterInitNY_Synchronization(vvLanLogList);

      if(vvLanLogList.Count.NotZero() && recIDactions != null)
      {
         debugCount = VvSkyLab.Execute_AfterInitNY_Synchronization(prevYearDbConn, thisYearDbConn, recIDactions, tableName);
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema ništa za After InitNY!?");
      }

      #endregion Voila 

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.CopyOut_InProgress = false;

      thisYearDbConn.Close();
      prevYearDbConn.Close();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Prenio {0} zapisa.", debugCount);

      #endregion Finish

   }

   #endregion Prjkt

   #region Kplan

   private void KPL_RenameKonto(object sender, EventArgs e)
   {
      VvRenameKontoDlg dlg = new VvRenameKontoDlg();
      Kplan kplan_rec = TheVvDataRecord as Kplan;
      string oldKonto = kplan_rec.Konto;


      dlg.Fld_NewKonto = oldKonto;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newKonto = dlg.Fld_NewKonto;

      dlg.Dispose();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted konto only                                                                                                                                            
      DataRow drSchema = ZXC.FtransDao.TheSchemaTable.Rows[ZXC.FtransDao.CI.t_konto];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKonto", oldKonto, " = "));

      int nora = ZXC.KplanDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newKonto);

      if(nora >= 0)
      {
         BeginEdit(kplan_rec);
         kplan_rec.Konto = newKonto;
         bool OK = TheVvDao.RWTREC(TheDbConnection, kplan_rec);
         EndEdit(kplan_rec);
         PutFieldsActions(TheDbConnection, kplan_rec, TheVvRecordUC);

         if(OK) ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, kao i samu karticu.", nora, (string)(drSchema["BaseTableName"]));
         else ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, ALI NE i samu karticu!", nora, (string)(drSchema["BaseTableName"]));
      }
   }

   private void ViewLookUp_KPI(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_KPI, ats_SubModulSet[xy.X][xy.Y], "Knjiga primitka i izdatka", ZXC.Q3un);
   }
   private void ViewLookUp_KPI24(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_KPI24, ats_SubModulSet[xy.X][xy.Y], "Knjiga primitka i izdatka 2024", ZXC.Q3un);
   }
   private void ViewLookUp_PPI(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_PPI, ats_SubModulSet[xy.X][xy.Y], "Pregled primitka i izdatka", ZXC.Q3un);
   }
   private void ViewLookUp_PPI2(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_PPI2, ats_SubModulSet[xy.X][xy.Y], "Pregled poslovnih primitka i izdatka", ZXC.Q3un);
   }   

   private void ViewLookUp_BIL(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_BilancaMP, ats_SubModulSet[xy.X][xy.Y], "Bilanca za male poduzetnike", ZXC.Q3un);
   }   
   private void ViewLookUp_RDGI(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_RDiGMP, ats_SubModulSet[xy.X][xy.Y], "Račun dobitka i gubitka za MP", ZXC.Q3un);
   }  
   private void ViewLookUp_PDO(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_ObrPD, ats_SubModulSet[xy.X][xy.Y], "Obrazac PD", ZXC.Q3un);
   }
   private void ViewLookUp_TSIpod(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      Create_LUI_Dlg(ZXC.luiListaNTR_TSIPod, ats_SubModulSet[xy.X][xy.Y], "TSI-POD", ZXC.Q3un);
   }

   private void ViewLookUp_Klase(object sender, EventArgs e)
   {
      VvLookUpLista vvLookUpLista = ZXC.luiListaKplanKlase;
      
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KPL);

      vvLookUpLista.LazyLoad();

      LookUpItem_ListView_Dialog dlg;

      dlg      = new LookUpItem_ListView_Dialog(vvLookUpLista, ats_SubModulSet[xy.X][xy.Y]/*  tsPanel_SubModul */);
      dlg.Text = "Klase";

      dlg.listView.Columns[0].Width = ZXC.Q3un;
      dlg.listView.Columns[1].Width = ZXC.Q10un*2;

      dlg.ShowDialog();
      dlg.Dispose();
   
   }
   
   #endregion Kplan

   #region Nalog

   private void NAL_Close47(object sender, EventArgs e)
   {
      NalogClose47DLG dlg = new NalogClose47DLG();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         Cursor.Current = Cursors.WaitCursor;

       //NalogDao.Close47(TheDbConnection, dlg.Fld_OcePrenosNaRashode);
       // 07.03.2016.  
              if(ZXC.CURR_prjkt_rec.IsNeprofit                         ) NalogDao.Close34 (TheDbConnection, dlg.Fld_OcePrenosNaRashode);
         else if(ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND) NalogDao.Close346(TheDbConnection, dlg.Fld_OcePrenosNaRashode);
         else                                                            NalogDao.Close47 (TheDbConnection, dlg.Fld_OcePrenosNaRashode);

         PutFieldsActions(TheDbConnection, ZXC.NalogRec.MakeDeepCopy(), TheVvRecordUC);

         Cursor.Current = Cursors.Default;
      }

      dlg.Dispose();
   }

   private void NAL_PrenosPS(object sender, EventArgs e)
   {
      ZXC.VvDataBaseInfo dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      PrenosPsDLG dlg = new PrenosPsDLG(dbi.ProjectPreviousYear, dbi.ProjectYear, true);

      dlg.Fld_DBName = ZXC.VvDB_NameConstructor(dbi.ProjectPreviousYear, dbi.ProjectName, dbi.ProjectKcdAsUInt);

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         Cursor.Current = Cursors.WaitCursor;

         List<Kplan> kplanList;

         TheVvUC.SetSifrarAndAutocomplete<Kplan>(null, VvSQL.SorterType.None);

         kplanList = /*KplanList.Convert_VvList_To_KplanList*/(VvUserControl.KplanSifrar);

         bool OK = NalogDao.PS_SConti(TheDbConnection, dlg.Fld_DBName, kplanList, dlg.Fld_Preskoci_WYRN);

         if(OK && ZXC.NalogRec != null)
         {
            TheVvTabPage.FileIsEmpty = false;
            PutFieldsActions(TheDbConnection, ZXC.NalogRec.MakeDeepCopy(), TheVvRecordUC);
         }

         Cursor.Current = Cursors.Default;
      }

      dlg.Dispose();
   }

   private void NAL_UcitajIzv(object sender, EventArgs e)
   {
      LoadIzvodDLG dlg = new LoadIzvodDLG(false);

      dlg.ShowDialog();
   }

   private void NAL_UcitajIzv_SEPA(object sender, EventArgs e)
   {
      LoadIzvodDLG dlg = new LoadIzvodDLG(true);
    
      dlg.ShowDialog();
    
      string sepaXMLfileName = ZXC.LastExportFileName;

    //XmlSerializer ser = new XmlSerializer(typeof(PAIN_002_001_03.Document)); // PAZI!!! Ne brkaj namespace PAIN_001_001_03(ZNP) vs PAIN_002_001_03(poruke greske pain) !!! 
      XmlSerializer ser = new XmlSerializer(typeof(PAIN_001_001_03.Document)); // PAZI!!! Ne brkaj namespace PAIN_001_001_03(ZNP) vs PAIN_002_001_03(poruke greske pain) !!! 
      FileStream    fs  = new FileStream   (sepaXMLfileName, FileMode.Open);

    //PAIN_002_001_03.Document theDocument = ser.Deserialize(fs) as PAIN_002_001_03.Document;
      PAIN_001_001_03.Document theDocument = ser.Deserialize(fs) as PAIN_001_001_03.Document;
      fs.Close();

      VvReport dummyReport = new RptP_SEPA(new Vektor.Reports.PIZ.CR_SepaPain00100103(), new ZXC.VvRptExternTblChooser_Placa(true, true, true, true, true), "QQQreportName", null/*((PlacaReportUC)vvReportUC).TheRptFilter*/);
      dummyReport.ExecuteExportValidation(sepaXMLfileName);
   }

   //private void NAL_ColChooser(object sender, EventArgs e)
   //{
   //   NalogDUC nalogDuc = TheVvUC as NalogDUC;

   //   //if(nalogDuc.TheColChooserGrid.Visible)
   //   //{
   //   //   nalogDuc.TheColChooserGrid.Visible = false;
   //   //}
   //   //else
   //   //{
   //   //   nalogDuc.TheColChooserGrid.Visible = true;
   //   //   nalogDuc.TheColChooserGrid.BringToFront();
   //   //}

   //   //nalogDuc.CalcLocationSizeAnchor_TheDGV_ChoosGrid(nalogDuc.TheG, ZXC.QunMrgn, nalogDuc.zaglavljeHamper.Bottom + ZXC.QunMrgn, nalogDuc.zaglavljeHamper, nalogDuc.TheColChooserGrid.Visible);

   //}
 
   internal void NAL_ToggleKnDeviza(object sender, EventArgs e)
   {
      NalogDUC theDUC = TheVvDocumentRecordUC as NalogDUC;

      if(theDUC == null ||
         theDUC.Fld_DevNameAsEnum == ZXC.ValutaNameEnum.EMPTY ||
         theDUC.Fld_DevNameAsEnum == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum) return;

      theDUC.IsShowingConvertedMoney = !theDUC.IsShowingConvertedMoney; // toggle 

      theDUC.DevTecaj = ZXC.DevTecDao.GetHnbTecaj(theDUC.Fld_DevNameAsEnum, theDUC.Fld_DokDate);

      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) // mi smo upravo u Dodaj ili Ispravi i zelimo preracunati deviznu cijenu u kune, a moze Boga mi i obrnuto 
      {
         if(theDUC.DevTecaj == 1.00M) // nije uspio naci tecaj 
         {
            theDUC.IsShowingConvertedMoney = !theDUC.IsShowingConvertedMoney; // toggle 
            return;
         }

         theDUC.TheG.EndEdit(); // ! ostavi ovo 

         decimal saldo = ConvertNalogDugPotToKuneOrDeviza(theDUC, theDUC.IsShowingConvertedMoney);

         // 10.04.2020: 
         #region isDevizniNalogNERAVNOTEZA

         bool isDevizniNalog_MalaNeravnoteza = saldo.NotZero() && Math.Abs(saldo) < 0.66M; // ako je saldo veci od 0.66kn onda nekaj drugo ne stima i nemoj ovo raditi 

         if(theDUC.IsShowingConvertedMoney == false && isDevizniNalog_MalaNeravnoteza) // kod povratka u kune 
         {
            theDUC.GetDgvFields(false);

            decimal usedAddition = theDUC.nalog_rec.SetFtransesInBalance(saldo); // VOILA 

//#if DEBUG
//            ZXC.aim_emsg(MessageBoxIcon.Information, "usedAddition = {0}", usedAddition.ToStringVv());
//#endif
            theDUC.PutDgvFields();
         }

         #endregion isDevizniNalogNERAVNOTEZA

         theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
      }

      if(theDUC.IsShowingConvertedMoney == true) // idemo iz kuna u neku drugu valutu 
      {
         theDUC.ValutaNameInUse = theDUC.Fld_DevNameAsEnum; 
      }
      else // vracamo se iz neke valute u kune
      {
         theDUC.ValutaNameInUse = ZXC.ValutaNameEnum.EMPTY;
      }

      // 31.3.2011:
      //if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) theDUC.GetDgvFields1(false); // da digne MarkTransesToDelete 
      // ma ipak ne 

      // 10.04.2020: dodan GetDgvFields(false); Faktur to ne treba jer se tam pozove pri 'PutTransSumToDocumentSumFields()' 
      //NE RADI! rADI MALI NALOG A VELIKI NE (KOPIRAJ PA TOGGLE VALUTA)
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None)
      {
         theDUC.GetDgvFields(false);
         theDUC.PutDgvFields(     );
      }
      // 10.04.2020: observacija; Faktur nema niti ovaj PutFields jer ga pak digne 'Fld_ShowInValutaLookUp' 
      theDUC.PutFields(theDUC.nalog_rec, false); 

      ToggleDevizaVisualApperiance(theDUC.IsShowingConvertedMoney, theDUC.Fld_DevName, theDUC.Fld_DevTecaj);

   }

   // 10.04.2020: 
 //private void    ConvertNalogDugPotToKuneOrDeviza(NalogDUC theDUC, bool _isShowingConvertedMoney)
   private decimal ConvertNalogDugPotToKuneOrDeviza(NalogDUC theDUC, bool _isShowingConvertedMoney)
   {
      decimal t_dug, t_pot;
      decimal new_dug, new_pot;

      decimal ukDug = 0M, ukPot = 0M;

      // 10.04.2020: vidi malo, bi li ovdje trebalo i Ron2() kao sto si i na Faktur-ovom 'ConvertFakturAllCijenaToKuneOrDeviza'?! 
      // ... ipak NE! kada sam upalio Ron2 vise ne ne stima. valjda zato sto je na Faktur-u vide decimala ...? 
      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         t_dug = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_dug, rIdx, false);
         t_pot = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_pot, rIdx, false);

         // 10.04.2020: kao i kod ConvertFakturAllCijenaToKuneOrDeviza 
         // ali ipak NE 
         if(_isShowingConvertedMoney == false)
         {
            // ipak NE: t_dug = t_dug.Ron2();
            // ipak NE: t_pot = t_pot.Ron2();
         }

         if(_isShowingConvertedMoney) // daj kune u devizu 
         {
            // 10.04.2020: kao i kod ConvertFakturAllCijenaToKuneOrDeviza 
            // ali ipak NE 
            new_dug = ZXC.DivSafe(t_dug, theDUC.DevTecaj)       ; theDUC.TheG.PutCell(theDUC.DgvCI.iT_dug, rIdx, new_dug); ukDug += new_dug;
            new_pot = ZXC.DivSafe(t_pot, theDUC.DevTecaj)       ; theDUC.TheG.PutCell(theDUC.DgvCI.iT_pot, rIdx, new_pot); ukPot += new_pot;
// ipak NE: new_dug = ZXC.DivSafe(t_dug, theDUC.DevTecaj).Ron2(); theDUC.TheG.PutCell(theDUC.DgvCI.iT_dug, rIdx, new_dug); ukDug += new_dug;
// ipak NE: new_pot = ZXC.DivSafe(t_pot, theDUC.DevTecaj).Ron2(); theDUC.TheG.PutCell(theDUC.DgvCI.iT_pot, rIdx, new_pot); ukPot += new_pot;
         }
         else // daj devizu u kune
         {
            new_dug = (t_dug * theDUC.DevTecaj); theDUC.TheG.PutCell(theDUC.DgvCI.iT_dug, rIdx, new_dug); ukDug += new_dug;
            new_pot = (t_pot * theDUC.DevTecaj); theDUC.TheG.PutCell(theDUC.DgvCI.iT_pot, rIdx, new_pot); ukPot += new_pot;
         }
      }

      return /*saldo = */ (ukDug - ukPot).Ron2();
   }

   private void RefreshFakRecID(object sender, EventArgs e)
   {
      NalogDUC theDUC = TheVvUC as NalogDUC;
      string t_tipBr;
      string tt;
      uint ttNum;
      bool found, parseOK;
      Faktur faktur_rec = new Faktur();

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         t_tipBr = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_tipBr, rIdx, false);

         parseOK = Ftrans.ParseTipBr(t_tipBr, out tt, out ttNum);

         if(parseOK)
         {
            found = FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);
         }
         else
         {
            found = false;
         }

         if(found && t_tipBr.NotEmpty())
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, faktur_rec.RecID);
         }
         else
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, 0);
         }
      }

      TheVvTabPage.PaliGasiDirtyFlag(true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAV", true); 

   }

   internal ObrProDLG obrProDLG;
   private void NAL_Proizvodnja(object sender, EventArgs e)
   {
      /*ObrProDLG*/ obrProDLG = new ObrProDLG(TheDbConnection);

      if(obrProDLG.ShowDialog() != DialogResult.OK) { obrProDLG.Dispose(); return; }

      obrProDLG.TheUC.GetKtoShemaDscFields();

      obrProDLG.Dispose();
   }

   internal AnalizaProizDLG analizaProizvodnjedlg;
   internal void NAL_AnalizaProizvodnje(object sender, EventArgs e)
   {
      /*AnalizaProizDLG*/ analizaProizvodnjedlg = new AnalizaProizDLG(TheDbConnection, sender);

      if(analizaProizvodnjedlg.ShowDialog() != DialogResult.OK) 
      {
         //List<OTPdata> OTPdataList = dlg.TheOtpList;
         analizaProizvodnjedlg.Dispose(); 
         return /*OTPdataList*/; 
      }

      analizaProizvodnjedlg.TheUC.GetKtoShemaDscFields();

      analizaProizvodnjedlg.Dispose();

      return /*null*/;
   }

   #region LoadFaktur2Nalog

   private void LoadFaktur2Nalog(object sender, EventArgs e)
   {
      LoadFaktur2NalogDlg dlg = new LoadFaktur2NalogDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      // ============================================================= 

      dlg.GetTheRulesFields();

      if(VvDaoBase.IsDocumentFromLockedPeriod(dlg.TheRules.DateOd.Date, false)) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.VvUtilDataPackage[] nacPlacArray;

      List<Faktur> fakturList = FakturDao.GetNeprebaceniFakturAndRtrans2NalogLists(TheDbConnection, dlg.TheRules, true, out nacPlacArray);

      FakturDao.ExecuteFaktur2Nalog(TheDbConnection, fakturList, dlg.TheRules, nacPlacArray);

      // ============================================================= 

      ShowNews();

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nBroj obrađenih RISK dokumenata/grupa: [{0}]\n\nBroj novih naloga: [{1}].", fakturList.Count, dlg.TheRules.NalogCount);
   
   }

   #endregion LoadFaktur2Nalog

   private void RefreshIzvod_wyrnTipBR_ForOldKindNalogIZ(object sender, EventArgs e) 
   { 
      NalogDUC theDUC = TheVvUC as NalogDUC;
      string t_tipBr;
      string t_konto;
      bool found; uint count = 0;
      Faktur faktur_rec = new Faktur();
      string tt;
      uint   ttNum;
      bool parseOK;

      if(theDUC.Fld_TipTran != Nalog.IZ_TT)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ova operacija može se izvoditi samo na nalogu izvoda - tipa 'IZ'.");
         return;
      }

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         t_tipBr = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_tipBr, rIdx, false);

         if(t_tipBr.IsEmpty()) continue; // preskoci prazan tipBr

         if(t_tipBr.StartsWith(Faktur.TT_WRN) || t_tipBr.StartsWith(Faktur.TT_YRN)) continue; // preskoci vec dobronastale wyrnTT-ove 

         parseOK = Ftrans.ParseTipBr(t_tipBr, out tt, out ttNum);

         if(parseOK && ZXC.TtInfo(tt).IsPdvTT) continue; // preskoci vec dobronastale classic thisYear TT-ove 

         t_konto = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_konto, rIdx, false);

         if(ZXC.CURR_prjkt_rec.NoNeedFor_WRN_UFRA && Kplan.GetIsKontoDobav(t_konto)) continue; // preskoci stavku izvoda koja se odnosi na dobavljaca (a niti smo POD_PO_NAPL niti OBRT_R2) 

         found = FakturDao.SetMeFaktur_ByVezniDok2(TheDbConnection, faktur_rec, t_tipBr);

         if(found && t_tipBr.NotEmpty())
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, faktur_rec.RecID       );
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_tipBr   , rIdx, faktur_rec.TT_And_TtNum);
            count++;
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji WRN/YRN faktur dokument sa 'VezniDok2' = [{0}]", t_tipBr);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, 0);
         }
      }

      if(count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema ništa za procesirati.");
      }
      else
      {
         TheVvTabPage.PaliGasiDirtyFlag(true);
         SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);
         ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nProcesirao [{0}] stavaka.", count);
      }
   }

   // Ovo se izvodi samo kod 'Gumiflex' firmi koje tek dolaze u Vektor gdje su svi PgPg tj. rucno uneseni racuni kojih nema ih va Vektoru 
   private void AutoADDWYRNfaktur_And_RefreshPS_TipBr_ForManualNalogPS(object sender, EventArgs e)
   {
      #region Init And Checks

      NalogDUC theDUC = TheVvUC as NalogDUC;
      string currNewTBR = "", currOldTBR = "", tmpTBR, tmpKTO;
      uint ftrProcessedCount = 0;
      uint newWYRNcount = 0;
      Faktur faktur_rec = new Faktur();
      Ftrans ftrans_rec;
      uint fakRecID, currNewFakRecID=0;
      uint fakYear , currNewFakYear =0;

      string dlgTipBr;
      DateTime dlgDokDate, dlgDospDate;
      decimal dlgPdvSt, dlgOsnovica, dlgPdv, dlgProlaz;
      ZXC.PdvKolTipEnum dlgPdvKolTip;

      if(theDUC.Fld_TipTran != Nalog.PS_TT)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ova operacija može se izvoditi samo na nalogu početnog stanja - tipa 'PS'.");
         return;
      }

    //if(theDUC.nalog_rec.Transes.Where(ftr => NalogDao.IsSaldaKontiKTO(ftr.T_konto, VvUserControl.KplanSifrar)).Any(ftr => ftr.T_valuta.IsEmpty()) ||
    //   theDUC.nalog_rec.Transes.Where(ftr => NalogDao.IsSaldaKontiKTO(ftr.T_konto, VvUserControl.KplanSifrar)).Any(ftr => ftr.T_tipBr .IsEmpty()))
    //{
    //   ZXC.aim_emsg(MessageBoxIcon.Error, "Neke Salda Konti stavke imaju prazan broj računa i/ili valutu.\n\nDopunite sve nedostajeće podatke pa ponovite ovu akciju.");
    //   return;
    //}
      bool noGO = false;
      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         ftrans_rec = (Ftrans)theDUC.GetDgvLineFields(rIdx, false, null);
       //if(NalogDao.IsSaldaKontiKTO(ftrans_rec.T_konto) && ftrans_rec.T_tipBr .IsEmpty()) { noGO = true; ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{0}] ima prazan broj računa.\n\nDopunite nedostajeći broj računa pa ponovite ovu akciju.\n\n[{1}]\n\n[{2}]", rIdx + 1, ftrans_rec.T_ticker, ftrans_rec.T_opis); }
         if(Kplan   .GetIsKontoKupac(ftrans_rec.T_konto) && ftrans_rec.T_tipBr .IsEmpty()) { noGO = true; ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{0}] ima prazan broj računa.\n\nDopunite nedostajeći broj računa pa ponovite ovu akciju.\n\n[{1}]\n\n[{2}]", rIdx + 1, ftrans_rec.T_ticker, ftrans_rec.T_opis); }
         if(NalogDao.IsSaldaKontiKTO(ftrans_rec.T_konto) && ftrans_rec.T_valuta.IsEmpty()) { noGO = true; ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{0}] ima praznu valutu.\n\nDopunite nedostajeću valutu pa ponovite ovu akciju.\n\n[{1}]\n\n[{2}]"          , rIdx + 1, ftrans_rec.T_ticker, ftrans_rec.T_opis); }
      }

      var saldaKontiFtransList = theDUC.nalog_rec.Transes.Where(ftr => NalogDao.IsSaldaKontiKTO(ftr.T_konto)).GroupBy(ftr => ftr.T_konto + ftr.T_kupdob_cd + ftr.R_forcedTipBr);

      List<string> usamljeneUplateTipBrList = saldaKontiFtransList.Where(ftrGR => ftrGR.Count() == 1 && ftrGR.First().T_otsKind == ZXC.OtsKindEnum.ZATVARANJE).Select(fGR => fGR.First().T_tipBr).ToList();

      if(noGO) return;

      AddDateToWYRNdlg dlg;

      #endregion Init And Checks

      #region TODOs

      // TODO: 
      //-1. (prije ove submodulakcije) izjednaciti sistem nastajanja VezniDok2; A - prenosimo PS po novome vs B - ovom sma ISPRAVLJAMO prethodno kreiran nalogPS (na stari nacin)
      // 0. PRIJE ulaska u petlju provjeriti ima li ikoji sa praznom t_valutom ili praznim t_tipBr-om a treba ih imati (NalogDao.IsSaldaKontiKTO) 
      // 1. ADDREC u WYRN radi SAMO za prvi ftrans iz tipBr grupe
      // 2. preskaci ftrans ako mu konto NIJE ni kup ni dob (Frigoterm) probaj preko PsRulea sa Kplana (izbaci prisiljene) 
      // 3. oformiti dijalog koji pita za DokDate 
      // 4. razlikvati u ovoj petlji Pg vs PgPg stavke ... Ovo se tako i onako izvodi samo kod 'Gumiflex' operacija gdje su svi PgPg tj. nema ih va Vektoru 
      // 5. ponasanje URA/UFA u ovoj petlji? (treba li DokDate, DospDate) 
      // - ako CURR_prjkt nije niti poduuzece po naplati i ako nije ObrtPdvR2 tada ge se preskace! 
      // - ako je ili ili onda ga se prebacuje ali je vazan samo DokDate a valuta je nebitna (jer sluzi samo PDV izvjestajima) 
      // 6. I kod ovog procesa i kod OTS izvjestaja izbaciti iz KSD kupciKontaSet konto gotovine kojeg saznas iz KTOsheme 
         // 7. Remapiranje malop cij kolona 
      // 8. Sta je sa tockom 5. kod Pg racuna u NalogDao? 
      #endregion TODOs

      #region The LOOP 

      List<Ftrans> olfaFtrGRlist;

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         #region Should SKIP

         tmpTBR = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_tipBr, rIdx, false);
         if(tmpTBR.IsEmpty()) continue; // preskoci prazan tipBr
         if(tmpTBR.StartsWith(Faktur.TT_WRN) || tmpTBR.StartsWith(Faktur.TT_YRN)) continue; // preskoci newKind PS tipBr-a 

         tmpKTO = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_konto, rIdx, false);
         Kplan kplan_rec = theDUC.Get_Kplan_FromVvUcSifrar(tmpKTO);
         if(kplan_rec.PsRule == Kplan.PsRuleEnum.FORCE_SaldaKontiKTO) continue; // preskaci ftrans ako mu konto NIJE ni kup ni dob (Frigoterm) (izbaci PsRule prisiljene) 

         if(ZXC.CURR_prjkt_rec.NoNeedFor_WRN_UFRA && kplan_rec.IsKontoDobav) continue; //ako CURR_prjkt nije niti poduzece po naplati i ako nije ObrtPdvR2 tada ge se preskace

         if(usamljeneUplateTipBrList.Contains(tmpTBR)) continue; // preskaci 'usamljene' uplate - za njih ne radi wyrn 

         #endregion Should SKIP

         ftrans_rec = (Ftrans)theDUC.GetDgvLineFields(rIdx, false, null);

         if(currOldTBR != ftrans_rec.T_tipBr && ftrans_rec.T_otsKind == ZXC.OtsKindEnum.OTVARANJE) // za rn koji ima i otvaranje i djelomicnu uplatu ADDwyrnREC ide samo jednom 
         {
            #region Get PgPg WYRN Data dialog

          //dlg = new AddDateToWYRNdlg(theDUC, ftrans_rec);
            olfaFtrGRlist = new List<Ftrans>(1); olfaFtrGRlist.Add(ftrans_rec);
            dlg = new AddDateToWYRNdlg(theDUC, olfaFtrGRlist);

            var dlgResult = dlg.ShowDialog();

            if(dlgResult == System.Windows.Forms.DialogResult.Abort) break;

            dlgPdvSt = dlg.TheUC.Fld_PdvSt;
            dlgOsnovica  = dlg.TheUC.Fld_Osnovica ;
            dlgPdv       = dlg.TheUC.Fld_Pdv      ;
            dlgProlaz    = dlg.TheUC.Fld_Prolaz   ;
            dlgDokDate   = dlg.TheUC.Fld_DokDate  ;
            dlgDospDate  = dlg.TheUC.Fld_DospDate ;
            dlgTipBr     = dlg.TheUC.Fld_TipBr    ;
            dlgPdvKolTip = dlg.TheUC.Fld_PdvKolTip;


            dlg.Dispose();

            #endregion Get PgPg WYRN Data dialog

            currNewTBR = NalogDao.AutoADD_PgPg_WYRN_faktur(TheDbConnection, ftrans_rec, out fakRecID, out fakYear, // ovdje ces upasti u probleme ako otvaranje NIJE prvi ftrans (npr avans pa tek onda otvaranje) 
                                                                dlgTipBr    ,
                                                                dlgDokDate  , 
                                                                dlgDospDate ,
                                                                dlgPdvSt    , 
                                                                dlgOsnovica , 
                                                                dlgPdv      , 
                                                                dlgProlaz   ,
                                                                dlgPdvKolTip);
            currOldTBR      = ftrans_rec.T_tipBr;
            currNewFakRecID = fakRecID;
            currNewFakYear  = fakYear;
            newWYRNcount++;
         }

         if(currNewTBR.NotEmpty()) // refresh t_tipBr, t_fakRecID 
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, currNewFakRecID);
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakYear , rIdx, currNewFakYear );
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_tipBr   , rIdx, currNewTBR     );
            ftrProcessedCount++;
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "ADDREC faktur dokument sa 'VezniDok2' = [{0}] NIJE USPJELO!", currNewTBR);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_fakRecID, rIdx, 0);
         }
      }

      #endregion The LOOP

      #region Finish 

      if(ftrProcessedCount.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema ništa za procesirati.");
      }
      else
      {
         TheVvTabPage.PaliGasiDirtyFlag(true);
         SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);
         ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nProcesirao [{0}] stavaka.\n\nDodao [{1}] WYRN dokumenata", ftrProcessedCount, newWYRNcount);
      }

      #endregion Finish

   }

   #endregion Nalog

   #region RecLIST

   /*protected*/public void LIST_GO(object sender, EventArgs e)
   {
      OpenNew_Record_TabPage(GetSubModulXY(TheVvRecLstUC.MasterSubModulEnum), (uint?)(TheVvRecLstUC.SelectedRecID));
   }

   #endregion RecLIST

   #region KupDob

   protected void KID_RenameTicker(object sender, EventArgs e)
   {
      VvRenameTickerDlg dlg = new VvRenameTickerDlg();
      Kupdob kupdob_rec = TheVvDataRecord as Kupdob;
      int nora, colIdx, kcdColIdx;
      string message = "";
      VvDaoBase vvDaoBase;
      object oldValue;
      string newValue;
      bool isPrenosNOTrename = false;

      string newTicker;

      uint theKCD;

      if(sender is Kupdob)
      {
         newTicker = (sender as Kupdob).Ticker;
         isPrenosNOTrename = true;
      }
      else
      {
         dlg.Fld_NewTicker = kupdob_rec.Ticker;

         if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

         newTicker = dlg.Fld_NewTicker;

         dlg.Dispose();
      }

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

      oldValue  = kupdob_rec.Ticker;
      newValue  = newTicker;

      // 12.05.2014: posto ticker NE MORA biti unikatan, da bi preimenovali samo ciljani ticker, traziti cemo po KCD-u 
      theKCD = kupdob_rec.KupdobCD;

      if(isPrenosNOTrename == false)
      {
         BeginEdit(kupdob_rec);
         kupdob_rec.Ticker = newTicker;
         bool OK = TheVvDao.RWTREC(TheDbConnection, kupdob_rec);

         if(!OK) { CancelEdit(kupdob_rec); return; }

         EndEdit(kupdob_rec);
         PutFieldsActions(TheDbConnection, kupdob_rec, TheVvRecordUC);
      }

      //============================================================================================================= 
      vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_ticker   ; kcdColIdx = ZXC.FtrCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_mtros_tk ; kcdColIdx = ZXC.FtrCI .t_mtros_cd ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .kupdobTK   ; kcdColIdx = ZXC.FexCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .posJedTK   ; kcdColIdx = ZXC.FexCI .posJedCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .mtrosTK    ; kcdColIdx = ZXC.FexCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .primPlatTK ; kcdColIdx = ZXC.FexCI .primPlatCD ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.KupdobDao; colIdx = ZXC.KpdbCI.centrTick  ; kcdColIdx = ZXC.KpdbCI.centrID    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .kupdobTK   ; kcdColIdx = ZXC.MixCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .mtrosTK    ; kcdColIdx = ZXC.MixCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .kupdob_tk  ; kcdColIdx = ZXC.OsrCI .kupdob_cd  ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .mtros_tk   ; kcdColIdx = ZXC.OsrCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .banka_tk   ; kcdColIdx = ZXC.PerCI .banka_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .mtros_tk   ; kcdColIdx = ZXC.PerCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PlacaDao ; colIdx = ZXC.PlaCI .mtros_tk   ; kcdColIdx = ZXC.PlaCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PtranoDao; colIdx = ZXC.PtoCI .t_kupdob_tk; kcdColIdx = ZXC.PtoCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      //============================================================================================================= 

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      
      ZXC.aim_emsg("Preimenovanje gotovo.\n\n" + message);
   }

   protected void KID_RenameKupdobCD(object sender, EventArgs e)
   {
      VvRenameKupdobCdDlg dlg = new VvRenameKupdobCdDlg();
      Kupdob kupdob_rec = TheVvDataRecord as Kupdob;
      int nora, colIdx;
      string message = "";
      VvDaoBase vvDaoBase;
      object oldValue;
      string newValue;
      bool isPrenosNOTrename = false;

      uint newKupdobCd;

      if(sender is Kupdob)
      {
         newKupdobCd = (sender as Kupdob).KupdobCD;
         isPrenosNOTrename = true;
      }
      else
      {
         dlg.Fld_NewKupdobCd = kupdob_rec.KupdobCD;

         if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

         newKupdobCd = dlg.Fld_NewKupdobCd;

         dlg.Dispose();
      }

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

      oldValue  = kupdob_rec.KupdobCD;
      newValue  = newKupdobCd.ToString();

      if(isPrenosNOTrename == false)
      {
         BeginEdit(kupdob_rec);
         kupdob_rec.KupdobCD = newKupdobCd;
         bool OK = TheVvDao.RWTREC(TheDbConnection, kupdob_rec);

         if(!OK) { CancelEdit(kupdob_rec); return; }

         EndEdit(kupdob_rec);
         PutFieldsActions(TheDbConnection, kupdob_rec, TheVvRecordUC);
      }

      //============================================================================================================= 
      vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_mtros_cd ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.ArtiklDao; colIdx = ZXC.ArtCI .dobavCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.ArtiklDao; colIdx = ZXC.ArtCI .proizCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .posJedCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .primPlatCD ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.KupdobDao; colIdx = ZXC.KpdbCI.centrID    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .kupdob_cd  ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .banka_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PlacaDao ; colIdx = ZXC.PlaCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.PtranoDao; colIdx = ZXC.PtoCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.RtranoDao; colIdx = ZXC.RtoCI .t_kupdobCD ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
    //vvDaoBase = ZXC.RtranoDao; colIdx = ZXC.RtoCI .t_kupdobCD ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newKupdobCd); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      //============================================================================================================= 

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      
      ZXC.aim_emsg("Preimenovanje gotovo.\n\n" + message);
   }

   protected void KID_RenameName(object sender, EventArgs e)
   {
      VvRenameNameDlg dlg = new VvRenameNameDlg();
      Kupdob kupdob_rec = TheVvDataRecord as Kupdob;
      int nora, colIdx;
      string message = "";
      VvDaoBase vvDaoBase;
      object oldValue;
      string newValue;
      bool isPrenosNOTrename = false;

      string newName;

      if(sender is Kupdob)
      {
         newName = (sender as Kupdob).Naziv;
         isPrenosNOTrename = true;
      }
      else
      {
         dlg.Fld_NewName = kupdob_rec.Naziv;

         if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

         newName = dlg.Fld_NewName;

         dlg.Dispose();
      }

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

      oldValue  = kupdob_rec.Naziv;
      newValue  = newName;

      if(isPrenosNOTrename == false)
      {
         BeginEdit(kupdob_rec);
         kupdob_rec.Naziv = newName;
         bool OK = TheVvDao.RWTREC(TheDbConnection, kupdob_rec);

         if(!OK) { CancelEdit(kupdob_rec); return; }

         EndEdit(kupdob_rec);
         PutFieldsActions(TheDbConnection, kupdob_rec, TheVvRecordUC);
      }

      //============================================================================================================= 
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI.kupdobName    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI.posJedName    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI.mtrosName     ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI.primPlatName  ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI.kupdobName    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.XtransDao; colIdx = ZXC.XtrCI.t_kpdbNameA_50; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.XtransDao; colIdx = ZXC.XtrCI.t_kpdbNameB_50; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newName); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      //============================================================================================================= 

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      
      ZXC.aim_emsg("Preimenovanje gotovo.\n\n" + message);
   }


   private int RenameThisSucker(XSqlConnection conn, VvDaoBase vvDao, int colIdx, object oldValue, object newValue)
   {
      return RenameThisSucker(conn, vvDao, colIdx, oldValue, newValue, 0, 0);
   }

   private int RenameThisSucker(XSqlConnection conn, VvDaoBase vvDao, int colIdx, object oldValue, object newValue, int kcdColIdx, uint theKCD)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      DataRow drSchemaOfColToBeChanged = vvDao.TheSchemaTable.Rows[colIdx   ];
      DataRow drSchemaOftheKCD         = vvDao.TheSchemaTable.Rows[kcdColIdx];

      filterMembers.Add(new VvSqlFilterMember(drSchemaOfColToBeChanged, "nameOfColToBeChanged", oldValue, " = "));
      
      if(kcdColIdx.NotZero() && theKCD.NotZero())
      {
         filterMembers.Add(new VvSqlFilterMember(drSchemaOftheKCD, "theKCD", theKCD, " = "));
      }

      int nora = vvDao.RenameForeignKey(conn, filterMembers, drSchemaOfColToBeChanged, newValue.ToString());

      return nora;
   }

   protected void ViewLookUp_PbZupan(object sender, EventArgs e)
   {
    //// 14.09.2016: one time TH Radnici 'set new Ticker' operation 
    //if(Getvv_PRODUCT_name() == ZXC.vv_PRODUCT_Name)
    //{
    //   DelmeLater_OneTimeOnly_SetTHradniciTicker();
    //   return;
    //}

      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.KID);

      Create_LUI_Dlg(ZXC.luiListaPostaZupan, ats_SubModulSet[xy.X][xy.Y], "Lista mjesta i županija", ZXC.Q3un);
   }

#if DelmeLater_OneTimeOnly_SetTHradniciTicker
   private void DelmeLater_OneTimeOnly_SetTHradniciTicker()
   {
   #region Get R Tip Kupdobs

      string selectWhat      = "kk.*" ;
      string orderBy         = "RecID";
      string wantedKupdobTip = "R"    ;

      List<Kupdob> kupdobRlist = new List<Kupdob>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      filterMembers.Add(new VvSqlFilterMember(ZXC.KupdobSchemaRows[ZXC.KpdbCI.tip], "elKpdbTip", wantedKupdobTip, " = "));

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, kupdobRlist, filterMembers, "kk", orderBy, false, selectWhat, "");

      #endregion Get R Tip Kupdobs

      if(kupdobRlist.Count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema Partnera Tip-a\n\n[{0}]", wantedKupdobTip);
         return;
      }

      int first_TH_Ticker_CD = 101;
      VvDaoBase vvDaoBase         ;
      object oldValue             ;
      string newValue             ;
      string newTicker            ;
      uint theKCD                 ;
      int nora, colIdx, kcdColIdx ;
      string message = ""         ;
      int maxTkLen = ZXC.KupdobDao.GetSchemaColumnSize(ZXC.KpdbCI.ticker);

      Cursor.Current = Cursors.WaitCursor;

      foreach(Kupdob kupdob_rec in kupdobRlist)
      {
         ZXC.SetStatusText("Izvodim " + (first_TH_Ticker_CD - 100).ToString() + ". " + kupdob_rec.Naziv + " od " + kupdobRlist.Count.ToString());

         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

         newTicker = "TH" + first_TH_Ticker_CD.ToString("0000");
         
         message = "";

         first_TH_Ticker_CD++;

         if(newTicker.Length > maxTkLen) { ZXC.aim_emsg(MessageBoxIcon.Error, "NewTicker tool long!\n\n{0} ---> {1}", kupdob_rec.Ticker, newTicker); continue; }

         oldValue  = kupdob_rec.Ticker;
         newValue  = newTicker;

         // 12.05.2014: posto ticker NE MORA biti unikatan, da bi preimenovali samo ciljani ticker, traziti cemo po KCD-u 
         theKCD = kupdob_rec.KupdobCD;

         //if(isPrenosNOTrename == false)
         {
            BeginEdit(kupdob_rec);
            kupdob_rec.Napom2 = oldValue.ToString();
            kupdob_rec.Ticker = newTicker;
            bool OK = TheVvDao.RWTREC(TheDbConnection, kupdob_rec);

            if(!OK) { CancelEdit(kupdob_rec); continue; }

            EndEdit(kupdob_rec);
            //PutFieldsActions(TheDbConnection, kupdob_rec, TheVvRecordUC);
         }

         //============================================================================================================= 
         vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_ticker   ; kcdColIdx = ZXC.FtrCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_mtros_tk ; kcdColIdx = ZXC.FtrCI .t_mtros_cd ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
       //vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .kupdobTK   ; kcdColIdx = ZXC.FexCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
       //vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .posJedTK   ; kcdColIdx = ZXC.FexCI .posJedCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
       //vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .mtrosTK    ; kcdColIdx = ZXC.FexCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
       //vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .primPlatTK ; kcdColIdx = ZXC.FexCI .primPlatCD ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.KupdobDao; colIdx = ZXC.KpdbCI.centrTick  ; kcdColIdx = ZXC.KpdbCI.centrID    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .kupdobTK   ; kcdColIdx = ZXC.MixCI .kupdobCD   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI .mtrosTK    ; kcdColIdx = ZXC.MixCI .mtrosCD    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .kupdob_tk  ; kcdColIdx = ZXC.OsrCI .kupdob_cd  ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.OsredDao ; colIdx = ZXC.OsrCI .mtros_tk   ; kcdColIdx = ZXC.OsrCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .banka_tk   ; kcdColIdx = ZXC.PerCI .banka_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.PersonDao; colIdx = ZXC.PerCI .mtros_tk   ; kcdColIdx = ZXC.PerCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.PlacaDao ; colIdx = ZXC.PlaCI .mtros_tk   ; kcdColIdx = ZXC.PlaCI .mtros_cd   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         vvDaoBase = ZXC.PtranoDao; colIdx = ZXC.PtoCI .t_kupdob_tk; kcdColIdx = ZXC.PtoCI .t_kupdob_cd; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldValue, newTicker, kcdColIdx, theKCD); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
         //============================================================================================================= 

         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
         // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

         ZXC.aim_log("\n\nPreimenovao. {0} [{1}]\n\n{2}", oldValue, kupdob_rec, message);

      }

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo");
   }
#endif

   protected void KID_PrenesiKnjNaDrSifru/*ORIG*/(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

      Kupdob kupdob_rec_OLD = (TheVvUC as KupdobUC).kupdob_rec;

      VvPrenesiKupdobCdDlg dlg = new VvPrenesiKupdobCdDlg(kupdob_rec_OLD);
     
      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      Kupdob kupdob_rec_NEW = TheVvUC.Get_Kupdob_FromVvUcSifrar(dlg.Fld_NewKupdobCd);

      bool isDeleteOldKupdob = dlg.Fld_isDeleteOldKupdob;
      
      dlg.Dispose();

      if(kupdob_rec_NEW == null) { ZXC.aim_emsg("NEW kupdob is null!"); return; }

      KID_RenameTicker  (kupdob_rec_NEW, EventArgs.Empty);
      KID_RenameKupdobCD(kupdob_rec_NEW, EventArgs.Empty);
      KID_RenameName    (kupdob_rec_NEW, EventArgs.Empty);

      if(isDeleteOldKupdob)
      {
         DeleteRecord_OnClick(null, EventArgs.Empty);
      }

   }

#if DEBUG

   private void /*KID_PrenesiKnjNaDrSifru*/SvDuh_SetOdjeli(object sender, EventArgs e)
   {
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite 'SvDuh_SetOdjeli'?!",
         "Potvrdite SvDuh_SetOdjeli?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Kupdob>(TheDbConnection, SvDuh_SetOdjeli_JOB, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Sredio {0} Odjela.", debugCount);
   }

   static bool SvDuh_SetOdjeli_JOB(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Kupdob kupdob_rec = vvDataRecord as Kupdob;

      if(kupdob_rec.Tip != "MT") return false;


      string kdName  = kupdob_rec.Naziv                    ; // I20 KLINIKA ZA UNUT.BOLESTI 
      string sinGR   = kupdob_rec.Naziv.SubstringSafe(0, 1); // I                           
      string newTK   = kdName.SubstringSafe(0, 3)          ; // I20                         
      string sinName = kupdob_rec.Naziv.SubstringSafe(4)   ; // KLINIKA ZA UNUT.BOLESTI     
      string anaName = kupdob_rec.Ulica1                   ; // ODJEL ZA HEM.I KOAG. +      


      if(newTK.NotEmpty())
      {
         kupdob_rec.Naziv   = newTK + " " + anaName; // I20 ODJEL ZA HEM.I KOAG. +  
         kupdob_rec.Ticker  = newTK                ; // I20                         
         kupdob_rec.Ulica1  = sinGR                ; // I                           
         kupdob_rec.Ulica2  = sinGR + " " + sinName; // I KLINIKA ZA UNUT.BOLESTI   
         kupdob_rec.Email   = sinName              ; // KLINIKA ZA UNUT.BOLESTI     
         kupdob_rec.Url     = anaName              ; // ODJEL ZA HEM.I KOAG. +      

         kupdob_rec.IsMtr  = true                  ;
      }

      return kupdob_rec.EditedHasChanges();
   }

#endif

   // KDC-ov ttNum je zapravo kupdobCD ... jedan kupdob ima samo jednog KDCa 
   private Mixer Get_KDC_ForThisKUPDOB(uint kupdobCD)
   {
      Mixer KDCmixer_rec = new Mixer();

      MixerDao.SetMeMixer(TheDbConnection, KDCmixer_rec, Mixer.TT_KDC, kupdobCD, true);

      return KDCmixer_rec;
   }

   private void  KID_CreateNew_OrEditExisting_KCD(object sender, EventArgs e)
   {
      Kupdob kupdob_rec = TheVvDataRecord as Kupdob;

      #region Show existing or open new tabpage

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.X_KDC);

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      if(existingTabPage != null) existingTabPage.Selected = true;
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      #endregion Show existing or open new tabpage

      KDCDUC theDUC     = TheVvDocumentRecordUC as KDCDUC;

      Mixer KDCmixer_rec = Get_KDC_ForThisKUPDOB(kupdob_rec.KupdobCD);

      if(KDCmixer_rec == null || KDCmixer_rec.TtNum.IsZero()) // 'Novi' - ADDREC Mixer 
      {
         NewRecord_OnClick(null, EventArgs.Empty);

         theDUC.Fld_KupDobCd   = kupdob_rec.KupdobCD;
         theDUC.Fld_KupDobName = kupdob_rec.Naziv   ;
         theDUC.Fld_KupDobTk   = kupdob_rec.Ticker  ;

         theDUC.Fld_TT       = Mixer.TT_KDC;
         theDUC.Fld_TtNum    = kupdob_rec.KupdobCD; // !!! 
         theDUC.Fld_DokDate  = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      }
      else // 'Ispravi' - RWTREC Faktur 
      {
         TheVvDataRecord = KDCmixer_rec;

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

         EditRecord_OnClick(null, EventArgs.Empty);
      }

   }

   #endregion KupDob

   #region Osred

   private void OSR_RenameOsredCD(object sender, EventArgs e)
   {
      VvRenameOsredDlg dlg = new VvRenameOsredDlg();
      Osred osred_rec = TheVvDataRecord as Osred;
      string oldOsredCD = osred_rec.OsredCD;


      dlg.Fld_NewOsredCD = oldOsredCD;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newOsredCD = dlg.Fld_NewOsredCD;

      dlg.Dispose();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted osred only                                                                                                                                            
      DataRow drSchema = ZXC.AtransDao.TheSchemaTable.Rows[ZXC.AtransDao.CI.t_osredCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elOsredCD", oldOsredCD, " = "));

      int nora = ZXC.OsredDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newOsredCD);

      if(nora >= 0)
      {
         BeginEdit(osred_rec);
         osred_rec.OsredCD = newOsredCD;
         bool OK = TheVvDao.RWTREC(TheDbConnection, osred_rec);
         EndEdit(osred_rec);
         PutFieldsActions(TheDbConnection, osred_rec, TheVvRecordUC);

         if(OK) ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, kao i samu karticu.", nora, (string)(drSchema["BaseTableName"]));
         else ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, ALI NE i samu karticu!", nora, (string)(drSchema["BaseTableName"]));
      }
   }

   private void OSR_CreateNalog(object sender, EventArgs e)
   {
      //DialogResult result = MessageBox.Show("Da li zaista zelite kreirati temeljnicu (nalog za knjizenje) na osnovu ovog dokumenta?",
      // "Potvrdite PRENOS U FINANCIJSKO KNJIGOVODSTVO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      VvCreateNalogDUCDlg dlg = new VvCreateNalogDUCDlg();

      if(dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      //if(result != DialogResult.Yes) return;

      // ___ Here we GO! _______________________________________________________________________________ 

      ushort line = 0;
      int maxOpisLen = ZXC.FtransDao.GetSchemaColumnSize(ZXC.FtransDao.CI.t_opis);
      decimal ukDug = 0.00M, ukPot = 0.00M;
      //decimal amortUk1 = 0.00M, amortUk2 = 0.00M, amortUk3 = 0.00M;
      decimal /*amort1, amort2, amort3,*/ amortAll, theoreticAmort1;
      string ivKonto, t_opis;
      bool osrOK, isRashod;
      Osred osred_rec = new Osred();
      Amort amort_rec = (Amort)TheVvDataRecord;
      decimal dug, pot;

      if(VvDaoBase.IsDocumentFromLockedPeriod(amort_rec.DokDate.Date, false)) return;

      isRashod = amort_rec.TT == Amort.RASHOD_TT;

      foreach(Atrans atrans_rec in amort_rec.Transes)
      {
         //osrOK = osred_rec.VvDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, osred_rec, atrans_rec.T_osredCD, ZXC.OsredSchemaRows[ZXC.OsrCI.osredCD], false);
         osred_rec = VvUserControl.OsredSifrar.SingleOrDefault(osr => osr.OsredCD == atrans_rec.T_osredCD);
         osrOK = osred_rec != null;
         if(!osrOK) osred_rec = new Osred();

         if(osrOK && osred_rec.KontoIv.NotEmpty()) ivKonto = osred_rec.KontoIv;
         else                                      ivKonto = "KtoIV?";

         t_opis = (osred_rec.Naziv + " / " + atrans_rec.T_opis);

         if(t_opis.Length > maxOpisLen) t_opis = t_opis.Substring(0, maxOpisLen);

         if(isRashod)
         {
            dug = 0;
            pot = atrans_rec.T_pot;
         }
         else
         {
            dug = atrans_rec.T_dug;
            pot = atrans_rec.T_pot;
         }

         NalogDao.AutoSetNalog(TheDbConnection, ref line,

            // /* DateTime n_dokDate   */ amort_rec.DokDate,
            // /* string   n_tt        */ "TM",
            // /* string   n_napomena  */ amort_rec.Napomena,
            // /* string   t_konto     */ ivKonto,
            // /* string   t_opis      */ t_opis,
            // /* decimal  t_dug       */ atrans_rec.T_dug,
            // /* decimal  t_pot       */ atrans_rec.T_pot);


            /* DateTime n_dokDate   */ amort_rec.DokDate,
            /* string   n_tt        */ "TM",
            /* string   n_napomena  */ amort_rec.Napomena,
            /* string   t_konto     */ ivKonto,
            /* string   t_kupdob_cd */ 0,
            /* string   t_ticker    */ "",
            /* string   t_mtros_cd  */ osred_rec.MtrosCd,
            /* string   t_mtros_tk  */ osred_rec.MtrosTk,
            /* string   t_tipBr     */ "",
            /* string   t_opis      */ t_opis,
            /* string   t_valuta    */ DateTime.MinValue,
            /* string   t_pdv       */ "",
            /* string   t_037       */ "",
            /* string   t_projektCD */ "",
            /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
            /* uint     t_fakRecID  */ 0,
            /* uint     t_fakYear   */ 0,
            /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
            /* string   t_fond      */ "",
            /* string   t_pozicija  */ "",
            /* decimal  t_dug       */ dug,
            /* decimal  t_pot       */ pot);

         if(isRashod)
         {

            NalogDao.AutoSetNalog(TheDbConnection, ref line,

               // /* DateTime n_dokDate   */ amort_rec.DokDate,
               // /* string   n_tt        */ "TM",
               // /* string   n_napomena  */ amort_rec.Napomena,
               // /* string   t_konto     */ ivKonto,
               // /* string   t_opis      */ t_opis,
               // /* decimal  t_dug       */ atrans_rec.T_dug,
               // /* decimal  t_pot       */ atrans_rec.T_pot);


               /* DateTime n_dokDate   */ amort_rec.DokDate,
               /* string   n_tt        */ "TM",
               /* string   n_napomena  */ amort_rec.Napomena,
               /* string   t_konto     */ osred_rec.Konto,
               /* string   t_kupdob_cd */ 0,
               /* string   t_ticker    */ "",
               /* string   t_mtros_cd  */ osred_rec.MtrosCd,
               /* string   t_mtros_tk  */ osred_rec.MtrosTk,
               /* string   t_tipBr     */ "",
               /* string   t_opis      */ t_opis,
               /* string   t_valuta    */ DateTime.MinValue,
               /* string   t_pdv       */ "",
               /* string   t_037       */ "",
               /* string   t_projektCD */ "",
               /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
               /* uint     t_fakRecID  */ 0,
               /* uint     t_fakYear   */ 0,
               /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
               /* string   t_fond      */ "",
               /* string   t_pozicija  */ "",
               /* decimal  t_dug       */ atrans_rec.T_dug,
               /* decimal  t_pot       */ 0.00M);
         }

         //------------------------------------------------------------------------- 

         ukDug += atrans_rec.T_dug;
         ukPot += atrans_rec.T_pot;

         /*@@@@@@@@@@@@@... ono about 1, 2, P, ... START @@@@@@@@@@@@@@@@@@@@@@@@*/

         if(isRashod) amortAll = Math.Abs(atrans_rec.T_dug - atrans_rec.T_pot);
         else         amortAll = atrans_rec.T_pot;

         theoreticAmort1 = atrans_rec.T_normalAm;

         // 12.03.2013:
         if(atrans_rec.T_koefAm.IsEmpty()) atrans_rec.T_koefAm = "1";

         switch(atrans_rec.T_koefAm[0])
         {
            case '2':
               if(theoreticAmort1 < amortAll) atrans_rec.Amort1 = theoreticAmort1;
               else                           atrans_rec.Amort1 = amortAll;

               atrans_rec.Amort2 = amortAll - atrans_rec.Amort1;
               atrans_rec.Amort3 = 0.00M;
               break;

            case 'P':
               if(theoreticAmort1 < amortAll) atrans_rec.Amort1 = theoreticAmort1;
               else                           atrans_rec.Amort1 = amortAll;

               if(theoreticAmort1 < (amortAll - atrans_rec.Amort1)) atrans_rec.Amort2 = theoreticAmort1;
               else                                                 atrans_rec.Amort2 = amortAll - atrans_rec.Amort1;

               atrans_rec.Amort3 = amortAll - (atrans_rec.Amort1 + atrans_rec.Amort2);
               break;

            default:

               atrans_rec.Amort1 = amortAll;
               atrans_rec.Amort2 = 0.00M;
               atrans_rec.Amort3 = 0.00M;

               break;
         }

         //amortUk1 += atrans_rec.Amort1;
         //amortUk2 += atrans_rec.Amort2;
         //amortUk3 += atrans_rec.Amort3;

         /*@@@@@@@@@@@@@... ono about 1, 2, P, ... END @@@@@@@@@@@@@@@@@@@@@@@@*/

      } // foreach(Atrans atrans_rec in amort_rec.Transes)

      // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
      /* 4310 - jednostr. 4311 - dvostruka, 4330 - potpuna */

      //if(ukPot - (amortUk1 + amortUk2 + amortUk3) > 0.02M) ZXC.aim_emsg("Q_Error: ukAmort {0:N} != someSum_rec {1:N}", ukPot, amortUk1 + amortUk2 + amortUk3);

      var atransesWosred         = amort_rec.Transes.Join(VvUserControl.OsredSifrar, atr => atr.T_osredCD, osr => osr.OsredCD, (A, O) => new 
      { 
         atrans  = A, 
         mtrosCD = O.MtrosCd,
         mtrosTK = O.MtrosTk
      });
      var atransesGroupedByMtr = atransesWosred.GroupBy(atrGr => atrGr.mtrosCD).Select(atrGr => new
      {
         mtrosCD  = atrGr.Key,
         mtrosTK  = atrGr.First().mtrosTK,
         amortUk1 = atrGr.Sum(atr => atr.atrans.Amort1),
         amortUk2 = atrGr.Sum(atr => atr.atrans.Amort2),
         amortUk3 = atrGr.Sum(atr => atr.atrans.Amort3)
      });

      //var atransesGroupedByMtros = VvUserControl.OsredSifrar.GroupJoin(amort_rec.Transes, osr => osr.OsredCD, atr => atr.T_osredCD, (O, aGr) => new 
      //{ 
      //   amortUk1 = aGr.Sum(atr => atr.Amort1),
      //   amortUk2 = aGr.Sum(atr => atr.Amort2),
      //   amortUk3 = aGr.Sum(atr => atr.Amort3) 
      //});

      foreach(var mtrGr in atransesGroupedByMtr)
      {
         if(mtrGr.amortUk1.NotZero())
            NalogDao.AutoSetNalog(TheDbConnection, ref line,
               /* DateTime n_dokDate   */ amort_rec.DokDate,
               /* string   n_tt        */ "TM",
               /* string   n_napomena  */ amort_rec.Napomena,
               /* string   t_konto     */ "4310",
               /* string   t_opis      */ amort_rec.Napomena,
               /* string   t_mtros_cd  */ mtrGr.mtrosCD,
               /* string   t_mtros_tk  */ mtrGr.mtrosTK,
               /* decimal  t_dug       */ mtrGr.amortUk1,
               /* decimal  t_pot       */ 0.00M);
         ukDug += mtrGr.amortUk1;
         //------------------------------------------------------------------------- 
         if(mtrGr.amortUk2.NotZero())
            NalogDao.AutoSetNalog(TheDbConnection, ref line,
               /* DateTime n_dokDate   */ amort_rec.DokDate,
               /* string   n_tt        */ "TM",
               /* string   n_napomena  */ amort_rec.Napomena,
               /* string   t_konto     */ "4311",
               /* string   t_opis      */ amort_rec.Napomena,
               /* string   t_mtros_cd  */ mtrGr.mtrosCD,
               /* string   t_mtros_tk  */ mtrGr.mtrosTK,
               /* decimal  t_dug       */ mtrGr.amortUk2,
               /* decimal  t_pot       */ 0.00M);
         ukDug += mtrGr.amortUk2;
         //------------------------------------------------------------------------- 
         if(mtrGr.amortUk3.NotZero())
            NalogDao.AutoSetNalog(TheDbConnection, ref line,
               /* DateTime n_dokDate   */ amort_rec.DokDate,
               /* string   n_tt        */ "TM",
               /* string   n_napomena  */ amort_rec.Napomena,
               /* string   t_konto     */ "4330",
               /* string   t_opis      */ amort_rec.Napomena,
               /* string   t_mtros_cd  */ mtrGr.mtrosCD,
               /* string   t_mtros_tk  */ mtrGr.mtrosTK,
               /* decimal  t_dug       */ mtrGr.amortUk3,
               /* decimal  t_pot       */ 0.00M);
         ukDug += mtrGr.amortUk3;
         //------------------------------------------------------------------------- 
      }


      // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

      if(dlg.Fld_OpenNalogDUC == true)
      {
         OpenNew_Record_TabPage(GetSubModulXY(ZXC.VvSubModulEnum.NAL_F), ZXC.NalogRec.RecID);
      }
      else
      {
         ZXC.aim_emsg("Gotovo. Dodao nalog sa {0} stavaka. UkDug: {1:N}   UkPot: {2:N}.", line, ukDug, ukPot);
      }

      dlg.Dispose();

      return;
   }

   private void OSR_Amortizacija(object sender, EventArgs e)
   {
      AmortizacijaDLG dlg = new AmortizacijaDLG();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         Cursor.Current = Cursors.WaitCursor;

         bool OK = AmortDao.Amortize(TheDbConnection, dlg.Fld_Datum, dlg.Fld_Opis, dlg.Fld_Razdoblje, dlg.Fld_isNivelirajEURoNeravnotezu);

         if(OK) PutFieldsActions(TheDbConnection, ZXC.AmortRec/*.MakeDeepCopy()*/, TheVvRecordUC);
         else ZXC.aim_emsg("Nema se sto amortizirati! (za zadane uvjete)");

         Cursor.Current = Cursors.Default;
      }

      dlg.Dispose();
   }

   private void OSR_Inventura(object sender, EventArgs e)
   {
      InventuraDLG dlg = new InventuraDLG();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         bool OK = AmortDao.Inventurize(TheDbConnection, dlg.Fld_Datum, dlg.Fld_Opis);

         if(OK) PutFieldsActions(TheDbConnection, ZXC.AmortRec/*.MakeDeepCopy()*/, TheVvRecordUC);
         else ZXC.aim_emsg("Nema se za sto raditi inventura!");
      }

      dlg.Dispose();

   }

   #endregion Osred

   #region Placa

   private void YesNoView_ChBoxDataGrid(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC placaDuc = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      if(placaBaseDuc.TheColChooserGrid.Visible)
      {
         placaBaseDuc.TheColChooserGrid.Visible = false;
         placaBaseDuc.TheG.Location = new Point(placaBaseDuc.TheG.Location.X, placaBaseDuc.TheColChooserGrid.Top);
         placaBaseDuc.TheG.Height = placaBaseDuc.TheG.Parent.Height - 2 * ZXC.QUN - placaBaseDuc.TheSumGrid.Height;
      }
      else
      {
         placaBaseDuc.TheColChooserGrid.Visible = true;
         placaBaseDuc.TheG.Location = new Point(placaBaseDuc.TheG.Location.X, placaBaseDuc.TheColChooserGrid.Bottom);
         placaBaseDuc.TheG.Height = placaBaseDuc.TheG.Parent.Height - 2 * ZXC.QUN - placaBaseDuc.TheSumGrid.Height - placaBaseDuc.TheColChooserGrid.Height;
      }

      placaBaseDuc.TheSumGrid.Location = new Point(placaBaseDuc.TheG.Location.X, placaBaseDuc.TheG.Bottom + ZXC.Qun12);
      
   }
   private void ViewAllColumns(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC placaDuc = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      placaBaseDuc.SelectVisibleColumns(ZXC.VvColSetVisible.AllVisible);

   }

   private void ViewRedColumns(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC placaDuc = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      placaBaseDuc.SelectVisibleColumns(ZXC.VvColSetVisible.RedVisible);
   }

   private void ViewBlueColumns(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC placaDuc = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      placaBaseDuc.SelectVisibleColumns(ZXC.VvColSetVisible.BlueVisible);
   }

   private void ResetPlacaGrid(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC placaDuc = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      placaBaseDuc.TheColChooserStates = new List<VvPref.VVColChooserStates>(placaBaseDuc.PlacaColChDefaultsList);

      placaBaseDuc.TheColChooserGrid.CellValueChanged -= new DataGridViewCellEventHandler(placaBaseDuc.TheColChooserGrid_CellValueChanged);
      placaBaseDuc.PutFields_TheColChooserGrid();
      placaBaseDuc.TheColChooserGrid.CellValueChanged += new DataGridViewCellEventHandler(placaBaseDuc.TheColChooserGrid_CellValueChanged);

      if(placaBaseDuc.TheG.ColumnHeadersDefaultCellStyle.BackColor == ZXC.vvColors.dataGridColumnHeaders_BackColor) placaBaseDuc.DisableView_ChBoxDataGrid(true);
      else                                                                                                          placaBaseDuc.DisableView_ChBoxDataGrid(false);

      placaBaseDuc.SelectVisibleColumns(ZXC.VvColSetVisible.BlueVisible);

   }

   #region Placa_AddPersons

   private void Placa_AddPersons(object sender, EventArgs e)
   {
      #region Local variables and some checks

      bool personImaPrevPtrans;
      Ptrans prevPtrans_rec;
      ZXC.DbNavigationRestrictor dbNavigationRestrictor;
      int rowIdx1, rowIdx2, rowIdx3, idxCorrector;
      List<Ptrane> prevPtrans_PtranE_List;
      List<Ptrano> prevPtrans_PtranO_List;

      //03.12.2013.
      //PlacaDUC theDUC = TheVvUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      VvDataGridView theGrid1 = placaBaseDuc.TheG;
      VvDataGridView theGrid2 = placaBaseDuc.TheG2;
      VvDataGridView theGrid3 = placaBaseDuc.TheG3;

      Placa_AddPersonsDlg dlg = new Placa_AddPersonsDlg();

      if(placaBaseDuc.Fld_TT.IsEmpty())
      {
         ZXC.RaiseErrorProvider(placaBaseDuc.tbx_tt, "Molim, zadajte najprije tip plaće.");
         return;
      }

      VvLookUpItem opcLUI, opcRadaLUI;

      #endregion Local variables

      #region Initialize Dialog values And ShowDialog()

      dlg.vvRecListUC.Fld_FilterMtrosCd    = placaBaseDuc.Fld_MtrosCd;
      dlg.vvRecListUC.Fld_FilterMtrosTk    = placaBaseDuc.Fld_MtrosTk;
      dlg.vvRecListUC.Fld_FilterMtrosNaziv = placaBaseDuc.Fld_MtrosNaziv;
      dlg.vvRecListUC.Fld_OcuPtrans =
      dlg.vvRecListUC.Fld_OcuPtrane =
      dlg.vvRecListUC.Fld_OcuPtrano = true;

      switch(placaBaseDuc.Fld_TT)
      {
         case Placa.TT_REDOVANRAD  : dlg.vvRecListUC.Fld_FilterIsPlaca = true; break;
         case Placa.TT_AUTORHONOR  : dlg.vvRecListUC.Fld_FilterIsAutH  = true; break;
         case Placa.TT_IDD_KOLONA_4: dlg.vvRecListUC.Fld_FilterIsIDDk4 = true; break;
         case Placa.TT_NADZORODBOR : dlg.vvRecListUC.Fld_FilterIsNadzO = true; break;
         case Placa.TT_PODUZETPLACA: dlg.vvRecListUC.Fld_FilterIsPoduz = true; break;
         case Placa.TT_UGOVORODJELU: dlg.vvRecListUC.Fld_FilterIsUgDj  = true; break;
         case Placa.TT_NEOPOREZPRIM: dlg.vvRecListUC.Fld_FilterIsPlaca = true; break;

         default: ZXC.RaiseErrorProvider(placaBaseDuc.tbx_tt, "Nepoznati tip plaće [" + placaBaseDuc.Fld_TT + "]!"); return;
      }

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      placaBaseDuc.Fld_MtrosCd    = dlg.vvRecListUC.Fld_FilterMtrosCd;
      placaBaseDuc.Fld_MtrosTk    = dlg.vvRecListUC.Fld_FilterMtrosTk;
      placaBaseDuc.Fld_MtrosNaziv = dlg.vvRecListUC.Fld_FilterMtrosNaziv;

      dlg.vvRecListUC.AddFilterMemberz();

      dlg.Dispose();

      #endregion Initialize Dialog values And ShowDialog()

      #region Get Person List

      List<Person> personList = new List<Person>();

      #region puse
      //TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Person);

      //switch(theDUC.Fld_TT)
      //{
      //   case Placa.TT_REDOVANRAD: personList = TheVvUC.PersonSifrar.Where(prsn => prsn.IsPlaca == true).ToList(); break;
      //   case Placa.TT_AUTORHONOR: personList = TheVvUC.PersonSifrar.Where(prsn => prsn.IsAutH == true).ToList(); break;
      //   case Placa.TT_NADZORODBOR: personList = TheVvUC.PersonSifrar.Where(prsn => prsn.IsNadzO == true).ToList(); break;
      //   case Placa.TT_PODUZETPLACA: personList = TheVvUC.PersonSifrar.Where(prsn => prsn.IsPoduz == true).ToList(); break;
      //   case Placa.TT_UGOVORODJELU: personList = TheVvUC.PersonSifrar.Where(prsn => prsn.IsUgDj == true).ToList(); break;

      //   default: ZXC.RaiseErrorProvider(theDUC.tbx_tt, "Nepoznati tip plaće [" + theDUC.Fld_TT + "]!"); return;
      //}
      #endregion puse

      VvDaoBase.LoadGenericVvDataRecordList<Person>(TheDbConnection, personList, dlg.vvRecListUC.TheFilterMembers, "prezime, ime");

      #endregion Get Person List

      #region GetPrevPtransForPerson, GetPrevPtrans_PtranEO_List, PutDgvLineFields123, ...

      Cursor.Current = Cursors.WaitCursor;

      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor("t_tt", new string[] { placaBaseDuc.Fld_TT });

      //theGrid1.RowsAdded -= new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);
      //theGrid2.RowsAdded -= new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);
      //theGrid3.RowsAdded -= new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);

      idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theGrid1);

      foreach(Person person_rec in personList)
      {
         prevPtrans_rec = PersonDao.GetPrevPtransForPerson(TheDbConnection, person_rec.PersonCD, placaBaseDuc.Fld_DokDate, placaBaseDuc.Fld_DokNum, dbNavigationRestrictor);

         personImaPrevPtrans = prevPtrans_rec.VirtualRecID != 0;

         theGrid1.Rows.Add();

         rowIdx1 = theGrid1.RowCount - idxCorrector;

         if(personImaPrevPtrans)
         {
            // PtranEs _____________________________________________________________________________________________________ 

            if(dlg.vvRecListUC.Fld_OcuPtrane)
            {
               prevPtrans_PtranE_List = GetPrevPtrans_PtranE_List(prevPtrans_rec.T_parentID, person_rec.PersonCD);

               foreach(Ptrane ptrane_rec in prevPtrans_PtranE_List)
               {
                  theGrid2.Rows.Add();

                  rowIdx2 = theGrid2.RowCount - idxCorrector;

                  placaBaseDuc.PutDgvLineFields2(ptrane_rec, rowIdx2, true);
                  placaBaseDuc.GetDgvLineFields2(rowIdx2, false, null); // da napuni Document's business.Transes 
               }
            }

            // PtranOs _____________________________________________________________________________________________________ 

            if(dlg.vvRecListUC.Fld_OcuPtrano)
            {
               prevPtrans_PtranO_List = GetPrevPtrans_PtranO_List(prevPtrans_rec.T_parentID, person_rec.PersonCD);

               foreach(Ptrano ptrano_rec in prevPtrans_PtranO_List)
               {
                  theGrid3.Rows.Add();

                  rowIdx3 = theGrid3.RowCount - idxCorrector;

                  placaBaseDuc.PutDgvLineFields3(ptrano_rec, rowIdx3, true);
                  placaBaseDuc.GetDgvLineFields3(rowIdx3, false, null); // da napuni Document's business.Transes 
               }
            }

            // Ptranses _____________________________________________________________________________________________________ 

            if(dlg.vvRecListUC.Fld_OcuPtrans || dlg.vvRecListUC.Fld_OcuPtrane || dlg.vvRecListUC.Fld_OcuPtrano)
            {

               placaBaseDuc.PutDgvLineFields1(prevPtrans_rec, rowIdx1, true);
               placaBaseDuc.GetDgvLineFields1(rowIdx1, false, null); // da napuni Document's business.Transes 

               prevPtrans_rec.CalcTransResults(placaBaseDuc.placa_rec);

               placaBaseDuc.PutDgvLineResultsFields1(rowIdx1, prevPtrans_rec, false);
            }

         } // if(personImaPrevPtrans) 

         // Overrajdamo eventualnu promjenu Imena ili Prezimena od prosle place, ili je ovo prva placa: 
         theGrid1.PutCell(placaBaseDuc.DgvCI.iT_personCD, rowIdx1, person_rec.PersonCD);
         theGrid1.PutCell(placaBaseDuc.DgvCI.iT_prezime, rowIdx1, person_rec.Prezime);
         theGrid1.PutCell(placaBaseDuc.DgvCI.iT_ime, rowIdx1, person_rec.Ime);
         theGrid1.PutCell(placaBaseDuc.DgvCI.iT_prezimeIme, rowIdx1, person_rec.PrezimeIme);

         if(true) // TODO: odluka sa dialoga 
         {
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_brutoOsn, rowIdx1, person_rec.X_brutoOsn);
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_koef    , rowIdx1, person_rec.X_koef    );
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_prijevoz, rowIdx1, person_rec.X_prijevoz);
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_opcCD   , rowIdx1, person_rec.X_opcCD   );
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_opcRadCD, rowIdx1, person_rec.X_opcRadCD);
            theGrid1.PutCell(placaBaseDuc.DgvCI.iT_isMioII , rowIdx1, VvCheckBox.GetString4Bool(person_rec.X_isMioII));

            opcLUI     = ZXC.luiListaOpcina.GetLuiForThisCd(person_rec.X_opcCD   );
            opcRadaLUI = ZXC.luiListaOpcina.GetLuiForThisCd(person_rec.X_opcRadCD);
            if(opcLUI != null)
            {
               theGrid1.PutCell(placaBaseDuc.DgvCI.iT_opcName , rowIdx1, opcLUI.Name  );
               theGrid1.PutCell(placaBaseDuc.DgvCI.iT_stPrirez, rowIdx1, opcLUI.Number);
            }
            if(opcRadaLUI != null)
            {
               theGrid1.PutCell(placaBaseDuc.DgvCI.iT_opcRadName , rowIdx1, opcRadaLUI.Name);
            }
         }

      } // foreach(Person person_rec in personList) 

      //theGrid1.RowsAdded += new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);
      //theGrid2.RowsAdded += new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);
      //theGrid3.RowsAdded += new DataGridViewRowsAddedEventHandler(theDUC.grid_RowsAdded);

      Cursor.Current = Cursors.Default;

      #endregion GetPrevPtransForPerson, GetPrevPtrans_PtranEO_List, PutDgvLineFields123, ...

   }

   private List<Ptrane> GetPrevPtrans_PtranE_List(uint t_parentID, uint t_personCD)
   {
      List<Ptrane> ptraneList = new List<Ptrane>();

      List<VvSqlFilterMember> filterMembers = Set_FilterMembers_GetPrevPtrans_PtranEO(t_parentID, t_personCD, ZXC.PtraneDao.TheSchemaTable.Rows, ZXC.PtraneDao.CI.t_parentID, ZXC.PtraneDao.CI.t_personCD);

      VvDaoBase.LoadGenericVvDataRecordList<Ptrane>(TheDbConnection, ptraneList, filterMembers, "t_serial");

      return ptraneList;
   }

   private List<Ptrano> GetPrevPtrans_PtranO_List(uint t_parentID, uint t_personCD)
   {
      List<Ptrano> ptranoList = new List<Ptrano>();

      List<VvSqlFilterMember> filterMembers = Set_FilterMembers_GetPrevPtrans_PtranEO(t_parentID, t_personCD, ZXC.PtranoDao.TheSchemaTable.Rows, ZXC.PtranoDao.CI.t_parentID, ZXC.PtranoDao.CI.t_personCD);

      VvDaoBase.LoadGenericVvDataRecordList<Ptrano>(TheDbConnection, ptranoList, filterMembers, "t_serial");

      return ptranoList;
   }

   private List<VvSqlFilterMember> Set_FilterMembers_GetPrevPtrans_PtranEO(uint parentID, uint personCD, DataRowCollection schemaTableRows, int colIdx_parentID, int colIdx_personCD)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      // For parentRecID                                                                                                                                                  
      drSchema = schemaTableRows[colIdx_parentID];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokNum", parentID, " = "));

      // For wanted personCD only                                                                                                                                            
      drSchema = schemaTableRows[colIdx_personCD];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPerson", personCD, " = "));

      return filterMembers;
   }

   #endregion Placa_AddPersons

   private void ViewLookUpPRules/*_ORIG*/(object sender, EventArgs e)
   {
      VvLookUpLista vvLookUpLista = ZXC.luiListaPRules;

      ZXC.VvSubModulEnum subModulEnum = TheVvUC.TheSubModul.subModulEnum;
      Point xy = ZXC.TheVvForm.GetSubModulXY(subModulEnum);

      vvLookUpLista.LazyLoad();

      LookUpItem_ListView_Dialog dlg;

      dlg = new LookUpItem_ListView_Dialog(vvLookUpLista, ats_SubModulSet[xy.X][xy.Y]/*  tsPanel_SubModul */);
      dlg.Text = "Pravila za place ";

      dlg.listView.Columns[0].Width = ZXC.Q6un;
      dlg.listView.Columns[1].Width = ZXC.Q10un;
      dlg.listView.Columns[2].Width = ZXC.Q5un;
      dlg.listView.Columns[3].Width = ZXC.Q3un;
      dlg.listView.Columns[4].Width = ZXC.QUN ;
      dlg.listView.Columns[5].Width = ZXC.Q5un;

      dlg.ShowDialog();
      dlg.Dispose();

   }

   private void ViewLookUpPRules_TEST(object sender, EventArgs e)
   {
      VvLookUpLista vvLookUpLista = ZXC.luiListaPorPla;

      ZXC.VvSubModulEnum subModulEnum = TheVvUC.TheSubModul.subModulEnum;
      Point xy = ZXC.TheVvForm.GetSubModulXY(subModulEnum);

      vvLookUpLista.LazyLoad();

      LookUpItem_ListView_Dialog dlg;

      dlg = new LookUpItem_ListView_Dialog(vvLookUpLista, ats_SubModulSet[xy.X][xy.Y]/*  tsPanel_SubModul */);
      dlg.Text = "Porezi Plaće";

      dlg.listView.Columns[0].Width = ZXC.Q4un;
      dlg.listView.Columns[1].Width = ZXC.Q4un;
      dlg.listView.Columns[2].Width = ZXC.Q4un;
      dlg.listView.Columns[3].Width = ZXC.Q4un;
      dlg.listView.Columns[4].Width = ZXC.Q4un;
      dlg.listView.Columns[5].Width = ZXC.Q4un;
      dlg.listView.Columns[6].Width = ZXC.Q4un;
      dlg.listView.Columns[7].Width = ZXC.Q4un;

      dlg.ShowDialog();
      dlg.Dispose();

   }

   private void PRS_RenamePersonCD(object sender, EventArgs e)
   {
      VvRenamePersonDlg dlg = new VvRenamePersonDlg();
      Person person_rec = TheVvDataRecord as Person;
      //string oldPersonCD    = person_rec.personCD;      
      uint oldPersonCD = person_rec.PersonCD;

      dlg.Fld_NewPersonCD = oldPersonCD;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      uint newPersonCD = dlg.Fld_NewPersonCD;

      dlg.Dispose();

      // 1: Ptrans 
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted person only                                                                                                                                            
      DataRow drSchema = ZXC.PtransDao.TheSchemaTable.Rows[ZXC.PtransDao.CI.t_personCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elPersonCD", oldPersonCD, " = "));

      int nora = ZXC.PersonDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newPersonCD.ToString("0000"));

      // 2: Ptrane 
      List<VvSqlFilterMember> filterMembers2 = new List<VvSqlFilterMember>(1);
      // For wanted person only                                                                                                                                            
      DataRow drSchema2 = ZXC.PtraneDao.TheSchemaTable.Rows[ZXC.PtraneDao.CI.t_personCD];
      filterMembers2.Add(new VvSqlFilterMember(drSchema2, "elPersonCD", oldPersonCD, " = "));

      int nora2 = ZXC.PersonDao.RenameForeignKey(TheDbConnection, filterMembers2, drSchema2, newPersonCD.ToString("0000"));

      // 3: Ptrano 
      List<VvSqlFilterMember> filterMembers3 = new List<VvSqlFilterMember>(1);
      // For wanted person only                                                                                                                                            
      DataRow drSchema3 = ZXC.PtranoDao.TheSchemaTable.Rows[ZXC.PtranoDao.CI.t_personCD];
      filterMembers3.Add(new VvSqlFilterMember(drSchema3, "elPersonCD", oldPersonCD, " = "));

      int nora3 = ZXC.PersonDao.RenameForeignKey(TheDbConnection, filterMembers3, drSchema3, newPersonCD.ToString("0000"));

      if(nora >= 0 && nora2 >= 0 && nora3 >= 0)
      {
         BeginEdit(person_rec);
         person_rec.PersonCD = newPersonCD;
         bool OK = TheVvDao.RWTREC(TheDbConnection, person_rec);
         EndEdit(person_rec);
         PutFieldsActions(TheDbConnection, person_rec, TheVvRecordUC);

         if(OK) ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, kao i samu karticu.", nora, (string)(drSchema["BaseTableName"]));
         else ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, ALI NE i samu karticu!", nora, (string)(drSchema["BaseTableName"]));
      }
   }

   private void Placa_CalcBrutoFromNeto(object sender, EventArgs e)
   {
      //03.12.2013.
      //PlacaDUC theDUC = TheVvDocumentRecordUC as PlacaDUC;
      PlacaBaseDUC placaBaseDuc = TheVvDocumentRecordUC as PlacaBaseDUC;

      VvDataGridView theG = placaBaseDuc.TheG;
      int currRowIdx = theG.CurrentRow.Index;
      PlacaBaseDUC.Ptrans_colIdx dgvCI = placaBaseDuc.DgvCI;

      if(currRowIdx < 0 || currRowIdx == theG.NewRowIndex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odaberite stavku.");
         return;
      }

      int perCDcolIdx = dgvCI.iT_personCD;
      int prezimeImecolIdx = dgvCI.iT_prezimeIme;
      int nettoColIdx = dgvCI.iT_netto;
      int naRukeColIdx = dgvCI.iT_naRuke;

      uint personCD = theG.GetUint32Cell(perCDcolIdx, currRowIdx, false);
      string prezimeIme = theG.GetStringCell(prezimeImecolIdx, currRowIdx, false);

      Placa_CalcBrutoFromNetoDlg dlg = new Placa_CalcBrutoFromNetoDlg(personCD, prezimeIme);

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      bool isAfterKrizPor = dlg.Fld_IsAfterKrizPor;
      decimal wantedNetto = dlg.Fld_Neto;

      #region HereWeGo

      Ptrans ptrans_rec = (Ptrans)placaBaseDuc.GetDgvLineFields1(currRowIdx, false, null);

      Decimal calcBruto = ptrans_rec.CalcBrutoDaNetto(wantedNetto, isAfterKrizPor, placaBaseDuc.placa_rec);

      theG.PutCell(dgvCI.iT_brutoOsn, currRowIdx, calcBruto);

      placaBaseDuc.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);

      decimal newNetto = isAfterKrizPor ? theG.GetDecimalCell(naRukeColIdx, currRowIdx, false) :
                                          theG.GetDecimalCell(nettoColIdx, currRowIdx, false);

      //if(newNetto != wantedNetto) ZXC.aim_emsg(MessageBoxIcon.Warning, "Sorry, nije uspjelo. " + newNetto + " != " + wantedNetto);
      if(Math.Abs(newNetto - wantedNetto) > 0.005M) ZXC.aim_emsg(MessageBoxIcon.Warning, "Sorry, nije uspjelo. " + newNetto + " != " + wantedNetto);

      #endregion HereWeGo

      dlg.Dispose();
   }

   public void PLA_CreateNalog_MakeNewLuiXmlForSKY(object sender, EventArgs e) //18.12.2025. !!!!! ??? tu dodati nove lliste
   {
      string theDirectoryWithBackslash = ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\";

      //VvLookUpLista gotoXmlList = ZXC.luiListaKPD2025;
      //gotoXmlList.SaveToXmlFile(theDirectoryWithBackslash);

      //ZXC.luiListaPorPla = new VvLookUpLista(ZXC.luiListaPorPla_Name, new string[] { "str32", "str128", "decimal", "tinyint", "int", "date", "uint", "2str128" }, ZXC.Kolona.druga, true);
      //ZXC.luiListaPorPla.LoadFromXmlFile(ZXC.TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false) + @"\", ZXC.luiListaPorPla.FileName);

      //18.12.2025. KodTipaEracuna
      //VvLookUpLista gotoXmlList = ZXC.luiListaKodTipaEracuna;
      //gotoXmlList.SaveToXmlFile(theDirectoryWithBackslash);

      VvLookUpLista gotoXmlList = ZXC.luiListaeRacPoslProc;
      gotoXmlList.SaveToXmlFile(theDirectoryWithBackslash);

   }
   public void PLA_CreateNalog/*_ORIG*/(object sender, EventArgs e)
   {
      VvCreateNalogDUCDlg dlg = new VvCreateNalogDUCDlg();

      if(dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      // ___ Here we GO! _______________________________________________________________________________ 

      ushort line = 0;
      int maxOpisLen = ZXC.FtransDao.GetSchemaColumnSize(ZXC.FtransDao.CI.t_opis);
      decimal ukDug = 0.00M, ukPot = 0.00M;
      Placa placa_rec = (Placa)TheVvDataRecord;

      if(VvDaoBase.IsDocumentFromLockedPeriod(placa_rec.DokDate.Date, false)) return;

      Cursor.Current = Cursors.WaitCursor;

      SetNalogLineForPlaca(ZXC.VirmanEnum.NET   , 0.00M, placa_rec.S_rNettoAftKrp, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.PRE   , 0.00M, placa_rec.S_tPrijevoz   , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.KRP   , 0.00M, placa_rec.S_rKrizPorUk  , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.POR   , 0.00M, placa_rec.S_rPorezAll   , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.PRI   , 0.00M, placa_rec.S_rPrirez     , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO1  , 0.00M, placa_rec.S_rMio1stup   , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO2  , 0.00M, placa_rec.S_rMio2stup   , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZDR   , 0.00M, placa_rec.S_rZdrNa      , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZOR   , 0.00M, placa_rec.S_rZorNa      , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZAP   , 0.00M, placa_rec.S_rZapNa      , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZPP   , 0.00M, placa_rec.S_rZapII      , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZPI   , 0.00M, placa_rec.S_rZpiUk      , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO1NA, 0.00M, placa_rec.S_rMio1stupNa , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO2NA, 0.00M, placa_rec.S_rMio2stupNa , ref line, placa_rec, ref ukDug, ref ukPot);
    //05.03.2020. dodano
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP63  , 0.00M, placa_rec.S_tDopZdr     , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP65  , 0.00M, placa_rec.S_tDobMIO     , ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP71  , 0.00M, placa_rec.S_tDopZdr2020 , ref line, placa_rec, ref ukDug, ref ukPot);

      SetNalogLineForPlaca(ZXC.VirmanEnum.NET   , placa_rec.S_rNettoAftKrp, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.PRE   , placa_rec.S_tPrijevoz   , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.KRP   , placa_rec.S_rKrizPorUk  , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.POR   , placa_rec.S_rPorezAll   , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.PRI   , placa_rec.S_rPrirez     , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO1  , placa_rec.S_rMio1stup   , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO2  , placa_rec.S_rMio2stup   , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZDR   , placa_rec.S_rZdrNa      , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZOR   , placa_rec.S_rZorNa      , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZAP   , placa_rec.S_rZapNa      , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZPP   , placa_rec.S_rZapII      , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.ZPI   , placa_rec.S_rZpiUk      , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO1NA, placa_rec.S_rMio1stupNa , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.MIO2NA, placa_rec.S_rMio2stupNa , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
    //05.03.2020. dodano
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP63  , placa_rec.S_tDopZdr     , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP65  , placa_rec.S_tDobMIO     , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);
      SetNalogLineForPlaca(ZXC.VirmanEnum.NP71  , placa_rec.S_tDopZdr2020 , 0.00M, ref line, placa_rec, ref ukDug, ref ukPot);

      // pitajQ
      //29.12.2021. NP - ovdje bi trebalo sumirati po ptrans_rec.T_neoPrimCD - a sto kad naletim na neki novi kojeg ovdje nema???
      if(placa_rec.TT == Placa.TT_NEOPOREZPRIM)
      {
         var sumeNetoAdd_poNeoPrimCD = placa_rec.TransesNonDeleted.Where(ptr => ptr.T_NetoAdd.NotZero())
        .GroupBy(ptr => ptr.T_neoPrimCD)
        .Select(grp => new
            {
             //npOznaka = grp.First().T_neoPrimCD,
               npOznaka = grp.Key,
               npSUM    = grp.Sum(g => g.T_NetoAdd)
            }
        );

         foreach(var sumaZaNeoPrimCD in sumeNetoAdd_poNeoPrimCD)
         {
            switch(sumaZaNeoPrimCD.npOznaka)
            {
               /*nagrada */case "63": SetNalogLineForPlaca(ZXC.VirmanEnum.NP63, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP63, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*prehrana*/case "65": SetNalogLineForPlaca(ZXC.VirmanEnum.NP65, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP65, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*dopZdr  */case "71": SetNalogLineForPlaca(ZXC.VirmanEnum.NP71, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP71, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*dnevnice*/case "17": SetNalogLineForPlaca(ZXC.VirmanEnum.NP17, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP17, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*loko voz*/case "18": SetNalogLineForPlaca(ZXC.VirmanEnum.NP18, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP18, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*prijevoz*/case "19": SetNalogLineForPlaca(ZXC.VirmanEnum.PRE , 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.PRE , sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*regr_boz*/case "21": SetNalogLineForPlaca(ZXC.VirmanEnum.NP21, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP21, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*darDjete*/case "22": SetNalogLineForPlaca(ZXC.VirmanEnum.NP22, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP22, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*otpremni*/case "26": SetNalogLineForPlaca(ZXC.VirmanEnum.NP26, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP26, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               /*jubilarn*/case "60": SetNalogLineForPlaca(ZXC.VirmanEnum.NP60, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP60, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
               default  :             SetNalogLineForPlaca(ZXC.VirmanEnum.NP71, 0.00M, sumaZaNeoPrimCD.npSUM, ref line, placa_rec, ref ukDug, ref ukPot); SetNalogLineForPlaca(ZXC.VirmanEnum.NP71, sumaZaNeoPrimCD.npSUM, 0.00M, ref line, placa_rec, ref ukDug, ref ukPot); break;
            }
         }

      } //if(placa_rec.TT == Placa.TT_NEOPOREZPRIM)

      // ___ Here we END! _______________________________________________________________________________ 

      if(dlg.Fld_OpenNalogDUC == true)
      {
         OpenNew_Record_TabPage(GetSubModulXY(ZXC.VvSubModulEnum.NAL_F), ZXC.NalogRec.RecID);
      }
      else
      {
         ZXC.aim_emsg("Gotovo. Dodao nalog sa {0} stavaka. UkDug: {1:N}   UkPot: {2:N}.", line, ukDug, ukPot);
      }

      dlg.Dispose();

      Cursor.Current = Cursors.Default;

      return;
   }

   private void SetNalogLineForPlaca(ZXC.VirmanEnum virmanEnum, decimal dug, decimal pot, ref ushort line, Placa placa_rec, ref decimal ukDug, ref decimal ukPot)
   {
    //string konto = Kplan.GetPlacaKonta    (virmanEnum, ZXC.ROOT_Ticker, dug.NotZero() ? ZXC.SaldoOrDugOrPot.DUG : ZXC.SaldoOrDugOrPot.POT);
      string konto = Kplan.GetPlacaKonta_New(virmanEnum, dug.NotZero() ? ZXC.SaldoOrDugOrPot.DUG : ZXC.SaldoOrDugOrPot.POT, placa_rec.TT);
  
      string opis, pozicija;
      KtoShemaPlacaDsc kspl = new KtoShemaPlacaDsc(ZXC.dscLuiLst_KtoShemaPlaca); 

            if(placa_rec.TT == Placa.TT_NEOPOREZPRIM) { opis = "Neoporezivi primitci za: "               + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? ""                  : "" ; }//Placa.TT_NEOPOREZPRIM
       else if(placa_rec.TT == Placa.TT_AUTORHONOR  ) { opis = "Autorski honorar za: "                   + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_AUTORHONOR  
       else if(placa_rec.TT == Placa.TT_AUTORHONUMJ ) { opis = "Autorski honorar umj. za: "              + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_AUTORHONUMJ 
       else if(placa_rec.TT == Placa.TT_AHSAMOSTUMJ ) { opis = "Autorski honorar samostalnog umj. za: "  + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_AHSAMOSTUMJ 
       else if(placa_rec.TT == Placa.TT_DDBEZDOPRINO) { opis = "Drugi dohodak bez obveze doprinosa za: " + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_DDBEZDOPRINO // 12.2018.
       else if(placa_rec.TT == Placa.TT_AUVECASTOPA ) { opis = "Autorski honorar umj. za: "              + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_AUVECASTOPA  // 12.2018.
       else if(placa_rec.TT == Placa.TT_NR1_PX1NEDOP) { opis = "Honorar nerezidenata za: "               + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_NRZ_PX1NEDOP // 12.2018.
       else if(placa_rec.TT == Placa.TT_NR2_P01NEDOP) { opis = "Honorar nerezidenata za: "               + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_NRZ_P01NEDOP // 12.2018.
       else if(placa_rec.TT == Placa.TT_NR3_PX1DADOP) { opis = "Honorar nerezidenata za: "               + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaAH : "" ; }//Placa.TT_NRZ_PX1DADOP // 12.2018.
       else if(placa_rec.TT == Placa.TT_NADZORODBOR ) { opis = "Nadzorni odbor za: "                     + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaNO : "" ; }//Placa.TT_NADZORODBOR 
       else if(placa_rec.TT == Placa.TT_TURSITVIJECE) { opis = "Turističko vijeće za: "                  + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaTV : "" ; }//Placa.TT_TURSITVIJECE
       else if(placa_rec.TT == Placa.TT_PLACAUNARAVI) { opis = "Plaća u naravi za: "                     + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaRR : "" ; }//Placa.TT_PLACAUNARAVI
       else if(placa_rec.TT == Placa.TT_POREZNADOBIT) { opis = "Porez na ispl. dobit za: "               + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? ""                  : "" ; }//Placa.TT_POREZNADOBIT
       else if(placa_rec.TT == Placa.TT_SEZZAPPOLJOP) { opis = "Sezonsko zapošljavanje za: "             + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? ""                  : "" ; }//Placa.TT_SEZZAPPOLJOP
       else if(placa_rec.TT == Placa.TT_STRUCNOOSPOS) { opis = "Plaća SO za : "                          + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaRR : "" ; }//Placa.TT_STRUCNOOSPO
       else if(placa_rec.TT == Placa.TT_UGOVORODJELU) { opis = "Ugovor o djelu za: "                     + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaUD : "" ; }//Placa.TT_UGOVORODJELU
       else                                           { opis = "Plaća za: "                              + placa_rec.MMYYYY + ". DokNum: " + placa_rec.DokNum + ", JOPPD: " + placa_rec.RSm_ID; pozicija = (ZXC.CURR_prjkt_rec.IsNeprofit == true && konto.StartsWith("4")) ? kspl.Dsc_PozicijaRR : "" ; }

      if((dug + pot).NotZero()) AddNalogLineForPlaca(ref line, konto, opis, dug, pot, placa_rec, ref ukDug, ref ukPot, pozicija);
   }

   private void AddNalogLineForPlaca(ref ushort line, string konto, string opis, decimal dug, decimal pot, Placa placa_rec, ref decimal ukDug, ref decimal ukPot, string pozicija)
   {
      NalogDao.AutoSetNalog(TheDbConnection, ref line,

         /* DateTime n_dokDate   */ placa_rec.DokDate,
         /* string   n_tt        */ "TM",
         /* string   n_napomena  */ opis,
         /* string   t_konto     */ konto,
         /* string   t_kupdob_cd */ 0,
         /* string   t_ticker    */ "",
         /* string   t_mtros_cd  */ placa_rec.MtrosCd,
         /* string   t_mtros_tk  */ placa_rec.MtrosTk,
         /* string   t_tipBr     */ "",
         /* string   t_opis      */ opis,
         /* string   t_valuta    */ DateTime.MinValue,
         /* string   t_pdv       */ "",
         /* string   t_037       */ "",
         /* string   t_projektCD */ "",
         /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* uint     t_fakRecID  */ 0,
         /* uint     t_fakYear   */ 0,
         /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* string   t_fond      */ "",
         /* string   t_pozicija  */ pozicija,
         /* decimal  t_dug       */ dug,
         /* decimal  t_pot       */ pot);
      //------------------------------------------------------------------------- 

      ukDug += dug;
      ukPot += pot;
   }

   private void PRS_SkipAllPersons(object sender, EventArgs e)
   {
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite 'izuzeti' sve djelatnike?!",
         "Potvrdite IZUZIMANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Person>(TheDbConnection, SetSkipOnAllPersons, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Izuzeo {0} djelatnika.", debugCount);
   }

   static bool SetSkipOnAllPersons(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Person person_rec = vvDataRecord as Person;

      person_rec.IsIzuzet = true;

      return person_rec.EditedHasChanges();
   }

   private void Placa_Placa2NalogRules(object sender, EventArgs e)
   {

      //VvExceptionDlg dlg = new VvExceptionDlg("eMessage", "sTrace");
      //dlg.ShowDialog();
      Placa2NalogRulesDlg dlg = new Placa2NalogRulesDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetKtoShemaPlacaDscFields();

      dlg.Dispose();
   }

   #region PutNalAndLoko To Placa

   private void TakePutNalAndLoko(object sender, EventArgs e)
   {
      #region Get Mixer List 

      PlacaBaseDUC theDUC    = TheVvUC as PlacaBaseDUC;
      Placa        placa_rec = theDUC.placa_rec;

      TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Person);
      Ptrans ptrans_rec = new Ptrans();
      Person person_rec;

      #region FilterMemberz - Mixer

      DateTime dateOd = new DateTime(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month, 1);
      DateTime dateDo = new DateTime(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month, DateTime.DaysInMonth(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month));

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember("tt", "theTT", "('PNI', 'PNL', 'PNT')", "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI./*dateA*/dokDate], false, "DateOD", dateOd, dateOd.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI./*dateA*/dokDate], false, "DateDO", dateDo, dateDo.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      #endregion FilterMemberz - Mixer

      List<Mixer> mixerList = new List<Mixer>();
      VvDaoBase.LoadGenericVvDataRecordList<Mixer>(TheDbConnection, mixerList, filterMembers, "");

      if(mixerList.Count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Nema podataka za ovo razdoblje niti na PutNalogu, niti na LokoVožnji.");
         return;
      }

      foreach(Mixer mixer_rec in mixerList)
      {
         mixer_rec.VvDao.LoadTranses(TheDbConnection, mixer_rec, false);
         mixer_rec.ConvertPutNalInKune();
      }

      #endregion Get Mixer List

      var ptransDataList = mixerList
         .GroupBy(mix => mix.PersonCD /*+ mix.Nesto*/)
         .Select(grp => new
            {
               personCD = grp.First().PersonCD,
             //netoAdd1 = grp.Sum(g => g.Sum_KolMoneyA     ), // prevozni troskovi 
               netoAdd1 = grp.Sum(g => g.KonvertMoneyPrevoz), // prevozni troskovi 
             //netoAdd2 = grp.Sum(g => g.R_PutNalAllTr    - g.Sum_KolMoneyA    ), // ostalo            
               netoAdd2 = grp.Sum(g => g.KonvertXtranoSum + g.KonvertMoneyDnevn), // ostalo            
            })
         .OrderBy(data => data.personCD);

      #region Put Ptrans Fieldz

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      // Znaci da pobrise onaj prvi automatski redak nastao pri NewRecordClick
      if(theDUC.TheG.RowCount == 2) theDUC.TheG.Rows.Clear();


      //*******
      Ptrans prevPtrans_rec;
      ZXC.DbNavigationRestrictor dbNavigationRestrictor;
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;
      dbNavigationRestrictor = new ZXC.DbNavigationRestrictor("t_tt", new string[] { placaBaseDuc.Fld_TT, Placa.TT_REDOVANRAD, Placa.TT_PODUZETPLACA });
      //*******

      foreach(var ptransData in ptransDataList)
      {
         person_rec = TheVvUC.Get_Person_FromVvUcSifrar(ptransData.personCD); if(person_rec == null) person_rec = new Person();

         //*******
         prevPtrans_rec = PersonDao.GetPrevPtransForPerson(TheDbConnection, person_rec.PersonCD, placaBaseDuc.Fld_DokDate, placaBaseDuc.Fld_DokNum, dbNavigationRestrictor);
         bool personImaPrevPtrans = prevPtrans_rec.VirtualRecID != 0;
         //*******


         // ptrans prevozni troskovi 
         ptrans_rec.Memset0(0);
         ptrans_rec.T_personCD  = ptransData.personCD;
         ptrans_rec.T_ime       = person_rec.Ime     ;
         ptrans_rec.T_prezime   = person_rec.Prezime ;
         ptrans_rec.T_NetoAdd   = ptransData.netoAdd1;
         ptrans_rec.T_neoPrimCD = (person_rec.IsPlaca == true || person_rec.IsPoduz == true) ? "18" : "";
         ptrans_rec.T_opcCD     = (personImaPrevPtrans == true) ? prevPtrans_rec.T_opcCD    : "";
         ptrans_rec.T_opcRadCD  = (personImaPrevPtrans == true) ? prevPtrans_rec.T_opcRadCD : "";
         if(ptrans_rec.T_NetoAdd.NotZero())
         {
            theDUC.TheG.Rows.Add();
            rowIdx = theDUC.TheG.RowCount - idxCorrector;
            theDUC.PutDgvLineFields1(ptrans_rec, rowIdx, true);
         }

         // ptrans ostali troskovi 
         ptrans_rec.Memset0(0);
         ptrans_rec.T_personCD  = ptransData.personCD;
         ptrans_rec.T_ime       = person_rec.Ime     ;
         ptrans_rec.T_prezime   = person_rec.Prezime ;
         ptrans_rec.T_NetoAdd   = ptransData.netoAdd2;
         ptrans_rec.T_neoPrimCD = (person_rec.IsPlaca == true || person_rec.IsPoduz == true) ? "17" : "";
         ptrans_rec.T_opcCD     = (personImaPrevPtrans == true) ? prevPtrans_rec.T_opcCD    : "";
         ptrans_rec.T_opcRadCD  = (personImaPrevPtrans == true) ? prevPtrans_rec.T_opcRadCD : "";
         if(ptrans_rec.T_NetoAdd.NotZero())
         {
            theDUC.TheG.Rows.Add();
            rowIdx = theDUC.TheG.RowCount - idxCorrector;
            theDUC.PutDgvLineFields1(ptrans_rec, rowIdx, true);
         }
      }

      SetDirtyFlag("TakePutNalAndLoko");

      #endregion Put Ptrans Fieldz

   }

   private List<VvSqlFilterMember> GetPutNaLokoFilterMembersXtrans(DateTime dateTime)
   {
      DateTime dateOd = new DateTime(dateTime.Year, dateTime.Month,  1);
      DateTime dateDo = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

    //filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt     ], false, "theTT",  Mixer.TT_RVR, "",                             ""          , " = " , ""));
      filterMembers.Add(new VvSqlFilterMember("t_tt", "theTT", "('PNI', 'PNL', 'PNT')", "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_dokDate], false, "DateOD", dateOd, dateOd.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_dokDate], false, "DateDO", dateDo      , dateDo.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));

      return filterMembers;
   }

   #endregion PutNalAndLoko To Placa

   private void CalcOvrhaRest(object sender, EventArgs e)
   {
      PlacaBaseDUC placaBaseDuc = TheVvDocumentRecordUC as PlacaBaseDUC;

      VvDataGridView theG  = placaBaseDuc.TheG;
      VvDataGridView theG3 = placaBaseDuc.TheG3;

      if(theG3.Visible == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odaberite tablicu Obustave");
         return;
      }

      
      int currRowIdx3                   = theG3.CurrentRow.Index;
      PlacaBaseDUC.Ptrano_colIdx dgvCI3 = placaBaseDuc.DgvCI3;

      if(currRowIdx3 < 0 || currRowIdx3 == theG3.NewRowIndex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odaberite stavku.");
         return;
      }

      int perCDcolIdx   = dgvCI3.iT_personCD;
      int prezimecolIdx = dgvCI3.iT_prezime ;
      int imecolIdx     = dgvCI3.iT_ime     ;

      uint   personCD   = theG3.GetUint32Cell(perCDcolIdx  , currRowIdx3, false);
      string prezimeIme = theG3.GetStringCell(prezimecolIdx, currRowIdx3, false) + " " +theG3.GetStringCell(imecolIdx    , currRowIdx3, false);

      // 04.02.2016. TODO !!!
      //Ako je neto plaća radnika jednaka ili veća od 5.693 kn, od ovrhe je izuzet iznos od 3.795,33 kn (2/3 od 5.693). radniku se mora isplatiti 3.795,33 kn, dok je preostali raspoloživ za ovrhu.
      //Ako je neto plaća radnika manja od 5.693 kn, od ovrhe je izuzet iznos od  2/3 neto plaće radnika-ovršenika. radniku se mora isplatiti iznos od 2/3 njegove plaće, dok je 1/3 njegove plaće raspoloživo za ovrhu.
      //Napomena: ova ograničenja nisu primjenjiva na ovrhu zbog zakonskog uzdržavanja, naknade štete nastale zbog narušenja zdravlja ili smanjenja, odnosno gubitka radne sposobnosti i naknade štete za izgubljeno uzdržavanje zbog smrti davatelja uzdržavanja, već se radi naplate tih tražbina primjenjuju drugačija ograničenja, nepovoljnija za radnika.

      #region HereWeGo

      placaBaseDuc.GetFields(false); // Bussiness ok 

      Ptrans ptrans_rec = placaBaseDuc.placa_rec.Transes.SingleOrDefault(ptrans => ptrans.T_personCD == personCD);

      if(ptrans_rec == null) { ZXC.aim_emsg("Nema ptrans retka za personCD [{0}]", personCD); return; }

      int ptransRowIdx = placaBaseDuc.placa_rec.Transes.IndexOf(ptrans_rec);

      ptrans_rec.CalcTransResults(placaBaseDuc.placa_rec);

      decimal maxZasticeniNetto = placaBaseDuc.placa_rec.Rule_ProsPlaca * 2M / 3M; // 2/3 prosPlace iz pRulesa 

      decimal netto    = ptrans_rec.R_Netto;
      decimal obustave = ptrans_rec.R_Obustave;
      decimal maxOvrha;
      if(netto <= placaBaseDuc.placa_rec.Rule_ProsPlaca) maxOvrha = ((netto / 3.00M            ) - obustave).Ron2();
      else                                               maxOvrha = ((netto - maxZasticeniNetto) - obustave).Ron2();

      #endregion HereWeGo

      Placa_CalcOvrvRestdlg dlg = new Placa_CalcOvrvRestdlg(personCD, prezimeIme, netto, obustave, maxOvrha);

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      theG3.PutCell(dgvCI3.iT_iznosOb, currRowIdx3, dlg.Fld_IznosOvrhe);

      placaBaseDuc.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(ptransRowIdx);

      dlg.Dispose();
   }

   private void CalcBruto1ViaKoef(object sender, EventArgs e)
   {
      DialogResult result = MessageBox.Show("Da li zaista zelite nanovo izračunati 'Bruto1' preko koeficijenta?!",
         "Potvrdite Bruto1 preko koeficijenta?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      // ================================================================================== 

      int rIdx;
      PlacaBaseDUC placaBaseDuc = TheVvDocumentRecordUC as PlacaBaseDUC;
      VvDataGridView theG = placaBaseDuc.TheG;
      Ptrans ptrans_rec;
      decimal bruto1;

      placaBaseDuc.GetFields(false); // Bussiness ok 

      for(rIdx = 0; rIdx < theG.RowCount - 1; ++rIdx)
      {
         ptrans_rec = (Ptrans)placaBaseDuc.GetDgvLineFields1(rIdx, false, null);

         //28.01.2015. dodani novi 
         // T_koefBruto1 se povecava za jos dva postotka i onda se obracunava na taj dio 
         decimal koefSt2 = ZXC.VvGet_25_of_100(ptrans_rec.T_koefBruto1, ptrans_rec.T_brutoDodSt2).Ron2();
         decimal koefSt3 = ZXC.VvGet_25_of_100(ptrans_rec.T_koefBruto1, ptrans_rec.T_brutoDodSt3).Ron2();
         decimal koefZaObrBruta = ptrans_rec.T_koefBruto1 + koefSt2 + koefSt3;
         
       //bruto1 = ptrans_rec.T_koefBruto1 * placaBaseDuc.placa_rec.Rule_VrKoefBr1;
         bruto1 = koefZaObrBruta * placaBaseDuc.placa_rec.Rule_VrKoefBr1;

         // 05.05.2015. ako je radnik na pola radnog vremena ili na manje od 8 satnog radnog vremena pa da razmjerno izracuna bruto uz pomoc koeficijenta
         if(ptrans_rec.T_dnFondSati.NotZero() && ptrans_rec.T_dnFondSati != Placa.SluzbeniDnevniFondSati) bruto1 *= ZXC.DivSafe((decimal)ptrans_rec.T_dnFondSati, (decimal)Placa.SluzbeniDnevniFondSati);
         if(ptrans_rec.T_IsPoluSat                                                                      ) bruto1 *= 0.50M;

         if(ptrans_rec.T_koefBruto1.IsZero() || placaBaseDuc.placa_rec.Rule_VrKoefBr1.IsZero()) continue;

         theG.PutCell(placaBaseDuc.DgvCI.iT_brutoOsn, rIdx, bruto1);

         placaBaseDuc.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
      }
   }

   #region Ptrane From Xtrans 'RVR' or 'MVR'

   private void TakeRVRorMVR(object sender, EventArgs e)
   {
      #region Init Stuff 

      // 26.09.2014: 
      bool isMVR = false;
      if(sender is string && (sender as string) == Mixer.TT_MVR) isMVR = true;

      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      placaBaseDuc.GetFields(false); // Bussiness ok 

      Placa placa_rec = placaBaseDuc.placa_rec;

      List<Xtrans> xtransList = new List<Xtrans>();
      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, xtransList, Get_RvrOrMvr_FilterMembers(placa_rec.MMYYYY_asDateTime, placa_rec.MMYYYY, isMVR ? Mixer.TT_MVR : Mixer.TT_RVR), ""/*"t_personCD, t_ttNum, t_serial "*/);

      // 26.09.2014: 
      List<ZXC.NameAndDecimal_CommonStruct> sviMVRsatiList = new List<ZXC.NameAndDecimal_CommonStruct>();
      if(isMVR)
      {
         xtransList.ForEach(xtr => xtr.T_moneyD = xtr.GetMonthlyPFS(xtransList));
         xtransList = ConvertMvrXtransListAsRvrXtransList(xtransList, out sviMVRsatiList);
      }

      if(xtransList.Count.IsZero()) return;

      // So, here we go... 

      TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Person);
      List<Ptrane> ptraneList = new List<Ptrane>();
      Ptrane ptrane_rec;
      Ptrans ptrans_rec;
      Person person_rec;
      bool isPoluSat;

      int  daysInMM = DateTime.DaysInMonth(placa_rec.MMYYYY_asDateTime.Year, placa_rec.MMYYYY_asDateTime.Month);
      uint ddOD    = 1;
      uint ddDO    = (uint)daysInMM;

      uint currPersonCD = 0;

      #endregion Init Stuff

      for(int i = 0; i < xtransList.Count; i++)
      {
            person_rec = TheVvUC.Get_Person_FromVvUcSifrar(xtransList[i].T_personCD);

            if(person_rec != null && placa_rec.TT == Placa.TT_PODUZETPLACA && person_rec.IsPoduz == false) { xtransList.Remove(xtransList[i]); --i; }
            if(person_rec != null && placa_rec.TT == Placa.TT_REDOVANRAD && person_rec.IsPlaca   == false) { xtransList.Remove(xtransList[i]); --i; }
      }
         
      var xtransGroup = xtransList.Where(xtr => xtr.T_personCD.NotZero())
         .GroupBy(xtr => xtr.T_personCD + (isMVR ? xtr.MvrCd_as_PlacaMvrVrCd : xtr.RvrCd_as_PlacaEvrVrCd))
         .Select(grp => new
            {
               personCD = grp.First().T_personCD,
               ime      = TheVvUC.Get_Person_FromVvUcSifrar(grp.First().T_personCD).Ime,
               prezime  = TheVvUC.Get_Person_FromVvUcSifrar(grp.First().T_personCD).Prezime,
               rvrCD    = isMVR ? grp.First().MvrCd_as_PlacaMvrVrCd : grp.First().RvrCd_as_PlacaEvrVrCd,
               pfs      = grp.First().T_moneyD,
               satiSUM  = grp.Sum(g => g.T_kol)
            }
         )
         .OrderBy(gr => gr.personCD + gr.rvrCD);

      if(xtransGroup.Count().IsZero()) return;

      foreach(var xtrGrp in xtransGroup)
      {
         if(/*isMVR == false &&*/ currPersonCD != xtrGrp.personCD) // za RedRad i Blagdane se ovo treba dogoditi samo za prvu pojavu PersonCD-a ... 
         {

            #region Redovan Rad

            ptrane_rec = new Ptrane();

            ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == xtrGrp.personCD);

            if(ptrans_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu place (osnovno-ptrans) nema djelatnika sa sifrom {0},\n\na ima ga u MVR!", xtrGrp.personCD);
               continue;
            }

            isPoluSat  = ptrans_rec == null ? false : ptrans_rec.T_IsPoluSat;

            ptrane_rec.T_personCD    = xtrGrp.personCD;
            ptrane_rec.T_prezime     = xtrGrp.prezime;
            ptrane_rec.T_ime         = xtrGrp.ime;
            ptrane_rec.T_vrstaR_cd   = placa_rec.TT == Placa.TT_PODUZETPLACA ? "16" : "01";
            ptrane_rec.T_vrstaR_name = ZXC.luiListaVrstaRadaEVR.GetNameForThisCd(ptrane_rec.T_vrstaR_cd);
            ptrane_rec.T_rsOD        = ddOD;
            ptrane_rec.T_rsDO        = ddDO; // zadnji dan u mjesecu
            ptrane_rec.T_sati        = ZXC.GetWorkHoursCount(placa_rec.IsTrgFondSati, isPoluSat, placa_rec.MMYYYY, ddOD, ddDO, ptrans_rec.T_dnFondSati); 
            ptrane_rec.T_cijPerc     = ZXC.luiListaVrstaRadaEVR.GetNumberForThisCd (ptrane_rec.T_vrstaR_cd);
            ptrane_rec.T_rsOO        = ZXC.luiListaVrstaRadaEVR.GetIntegerForThisCd(ptrane_rec.T_vrstaR_cd).ToString("00");

            ptraneList.Add(ptrane_rec); // redovan rad 
            
            #endregion Redovan Rad
   
            #region Eventual Blagdans

            decimal sumaBlagdanSati = ZXC.GetSumaBlagdanskihRadnihSatiZaMjesec(placa_rec.MMYYYY, placa_rec.IsTrgFondSati, isPoluSat, ptrans_rec.T_dnFondSati);

            if(sumaBlagdanSati.NotZero()) 
            {
               ptrane_rec = new Ptrane();

               ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == xtrGrp.personCD);
               isPoluSat  = ptrans_rec == null ? false : ptrans_rec.T_IsPoluSat;

               //do 01.06.2015.
               //string blagdanVrCd = 
               //   isMVR ? 
               //   ZXC.luiListaMixRadVrijemeMVR.GetIntegerForThisNumber(11M ).ToString("00") : // MVR 
               //   ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd    ("6" ).ToString("00") ; // RVR 

               string blagdanVrCd = 
                  isMVR ? 
                                                           ZXC.luiListaMixRadVrijemeMVR.GetIntegerForThisNumber(11M ).ToString("00") : // MVR 
                  (placa_rec.DokDate < ZXC.Date01042015) ? ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd    ("6" ).ToString("00") : // RVR 
                                                           ZXC.luiListaMixRadVrijemRVR2.GetIntegerForThisCd    ("14").ToString("00") ; // RVR 15


               ptrane_rec.T_personCD    = xtrGrp.personCD;
               ptrane_rec.T_prezime     = xtrGrp.prezime;
               ptrane_rec.T_ime         = xtrGrp.ime;
               ptrane_rec.T_vrstaR_cd   = blagdanVrCd;
               ptrane_rec.T_vrstaR_name = ZXC.luiListaVrstaRadaEVR.GetNameForThisCd(ptrane_rec.T_vrstaR_cd);
             //ptrane_rec.T_rsOD        = ddOD;
             //ptrane_rec.T_rsDO        = ddDO;
               ptrane_rec.T_sati        = sumaBlagdanSati; 
               ptrane_rec.T_cijPerc     = ZXC.luiListaVrstaRadaEVR.GetNumberForThisCd (blagdanVrCd);
               ptrane_rec.T_rsOO        = ZXC.luiListaVrstaRadaEVR.GetIntegerForThisCd(blagdanVrCd).ToString("00");

               ptraneList.Add(ptrane_rec); // blagdani 

            }

            #endregion Eventual Blagdans

            #region Eventual Prekovremeno 4 MVR

            decimal sviMVRsati = 0M;
            if(isMVR)
            {
               sviMVRsati = sviMVRsatiList.Where(lista => lista.TheName == ptrans_rec.T_personCD.ToString()).Sum(lista => lista.TheDecimal);
            }

            decimal prekovrSati = xtrGrp.pfs;
          //decimal prekovrSati = sviMVRsati - ptrane_rec.T_sati; // ptrane_rec.T_sati = ZXC.GetWorkHoursCount(placa_rec.IsTrgFondSati, isPoluSat, placa_rec.MMYYYY, ddOD, ddDO, ptrans_rec.T_dnFondSati); 
            if(isMVR && prekovrSati.IsPositive()) 
          //if(isMVR && sviMVRsati > ptrane_rec.T_sati) // ptrane_rec.T_sati = ZXC.GetWorkHoursCount(placa_rec.IsTrgFondSati, isPoluSat, placa_rec.MMYYYY, ddOD, ddDO, ptrans_rec.T_dnFondSati); 
            {
               decimal priznatiPrekovrSati = Placa.GetPriznatiPrekovrSati(prekovrSati);
               decimal visakPrekovrSati    = Placa.GetVisakPrekovrSati   (prekovrSati);
               
               ptrane_rec = new Ptrane();

               ptrans_rec = placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == xtrGrp.personCD);
               isPoluSat  = ptrans_rec == null ? false : ptrans_rec.T_IsPoluSat;

               //string prekovremenoVrCd = 
               //   isMVR ?
               //   ZXC.luiListaMixRadVrijemeMVR.GetIntegerForThisNumber(3M  ).ToString("00") : // MVR 
               //   ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd    ("4b").ToString("00") ; // EVR 

                              string prekovremenoVrCd = 
                  isMVR ?
                  ZXC.luiListaMixRadVrijemeMVR.GetIntegerForThisNumber(3M  ).ToString("00") : // MVR 
                  ZXC.luiListaMixRadVrijemeRVR.GetIntegerForThisCd    ("4b").ToString("00") ; // EVR 

               ptrane_rec.T_personCD    = xtrGrp.personCD;
               ptrane_rec.T_prezime     = xtrGrp.prezime;
               ptrane_rec.T_ime         = xtrGrp.ime;
               ptrane_rec.T_vrstaR_cd   = prekovremenoVrCd;
               ptrane_rec.T_vrstaR_name = ZXC.luiListaVrstaRadaEVR.GetNameForThisCd(ptrane_rec.T_vrstaR_cd);
             //ptrane_rec.T_rsOD        = ddOD;
             //ptrane_rec.T_rsDO        = ddDO;
               ptrane_rec.T_sati        = priznatiPrekovrSati; 
               ptrane_rec.T_cijPerc     = ZXC.luiListaVrstaRadaEVR.GetNumberForThisCd (prekovremenoVrCd);
               ptrane_rec.T_rsOO        = ZXC.luiListaVrstaRadaEVR.GetIntegerForThisCd(prekovremenoVrCd).ToString("00");

               ptraneList.Add(ptrane_rec); // prekovremeni 

               #region if(visakPrekovrSati.NotZero()) 
               if(visakPrekovrSati.NotZero())
               {
                  int ptransDgvRowIdx = placa_rec.Transes.IndexOf(ptrans_rec);

                  ptrans_rec.CalcTransResults(placa_rec);

                  decimal nepunoRVkoef            = ptrans_rec.T_dnFondSati.IsZero() ? 1.00M : (decimal)((decimal)ptrans_rec.T_dnFondSati / (decimal)Placa.SluzbeniDnevniFondSati);
                  decimal MFS                     = placa_rec.FondSati * nepunoRVkoef;
                  decimal cijenaNormalnogSata     = ZXC.DivSafe(ptrans_rec.T_brutoOsn, MFS);
                  decimal cijenaPrekovremenogSata = (ptrane_rec.T_cijPerc) / 100.00M * cijenaNormalnogSata; 
                  decimal dodatakNaBruto          = (cijenaPrekovremenogSata * visakPrekovrSati).Ron2();

                  placaBaseDuc.TheG.PutCell(placaBaseDuc.DgvCI.iT_dodBruto, ptransDgvRowIdx, dodatakNaBruto);

                  placaBaseDuc.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(ptransDgvRowIdx);

                  ZXC.aim_emsg(MessageBoxIcon.Information, "Djelatnik [{0}]\n\nima prekovremene [{1}] što je za [{2}] veće od max prekovr. [{3}]\n\nRedovna cijena sata {7}, {8}% za prekeovrmeno = {9} cijena prekovremenog sata\n\nZadao sam mu BRUTO DODATAK\n\n{4} sati x {5} cijena sata = \n\n{6} kn ",
                     ptrans_rec.T_prezimeIme, prekovrSati.ToStringVv(), visakPrekovrSati.ToStringVv(), Placa.MAXdozvoljeniPrekovrSati.ToStringVv(),
                     visakPrekovrSati.ToStringVv(), cijenaPrekovremenogSata.ToStringVv(), dodatakNaBruto.ToStringVv(),
                     cijenaNormalnogSata.ToStringVv(), ptrane_rec.T_cijPerc.ToStringVv(), cijenaPrekovremenogSata.ToStringVv());

               } // if(visakPrekovrSati.NotZero()) 

               #endregion if(visakPrekovrSati.NotZero())

            }

            #endregion Eventual Prekovremeno 4 MVR

         }

         currPersonCD = xtrGrp.personCD;

         #region From Xtrans

         //03.10.2014.
         // da ne dodaje za prekovremeno ponovo dodan nesto u tom sltilu treba jos provjerti kaj je sa rvr
         //if(xtrGrp.pfs.IsZero()) 10.10.2014. NEVALJA ako ima neku drugu vrstu rada onda ne dodaje kad ima i prekovremene
         if(xtrGrp.rvrCD != "00" && xtrGrp.rvrCD != "01") 
         {
            ptrane_rec = new Ptrane();

            ptrane_rec.T_personCD    = xtrGrp.personCD;
            ptrane_rec.T_prezime     = xtrGrp.prezime;
            ptrane_rec.T_ime         = xtrGrp.ime;
            ptrane_rec.T_vrstaR_cd   = xtrGrp.rvrCD;
            ptrane_rec.T_vrstaR_name = ZXC.luiListaVrstaRadaEVR.GetNameForThisCd(xtrGrp.rvrCD);
            ptrane_rec.T_sati        = xtrGrp.satiSUM;
            ptrane_rec.T_cijPerc     = ZXC.luiListaVrstaRadaEVR.GetNumberForThisCd(xtrGrp.rvrCD);
            ptrane_rec.T_rsOO        = ZXC.luiListaVrstaRadaEVR.GetIntegerForThisCd(xtrGrp.rvrCD).ToString("00");

            ptraneList.Add(ptrane_rec); // iz Mixera  
         }

         #endregion From Xtrans

      } // foreach(var xtrGrp in xtransGroup) 

      DumpPtranesOnTheGrid2(placaBaseDuc, ptraneList);

      SetDirtyFlag("TakeRVR");

   }

   private List<Xtrans> ConvertMvrXtransListAsRvrXtransList(List<Xtrans> MVRxtransList, out List<ZXC.NameAndDecimal_CommonStruct> sviMVRsati)
   {
      List<Xtrans> redesignedRVRxtransList = new List<Xtrans>(); 

      Xtrans newRVRxtrans_rec;

      sviMVRsati = 
         MVRxtransList
         .GroupBy(xtr => xtr.T_personCD)
         .Select(grp => new ZXC.NameAndDecimal_CommonStruct(grp.Key.ToString(), grp.Sum(q => q.MVRsVRlistKolSum)))
         .ToList();
 
      foreach(Xtrans MVRxtrans_rec in MVRxtransList) // MVR 
      {
         var xtrVRgroups = MVRxtrans_rec.MVRsVRlist
            .GroupBy(mvr => mvr.TheName)
            .Select(gr => new { vrCD = gr.Key, vrKOL = gr.Sum(MVRsVR => MVRsVR.TheDecimal) } );

         foreach(var xtrVRgroup in xtrVRgroups) // za svaku vrstu rada 
         {
            newRVRxtrans_rec = new Xtrans(); // RVR xtrans 

            newRVRxtrans_rec.T_personCD = MVRxtrans_rec.T_personCD;

            newRVRxtrans_rec.T_moneyD   = MVRxtrans_rec.T_moneyD; // PFS - prekovrermeni 

            newRVRxtrans_rec.T_strA_2   = xtrVRgroup.vrCD ;
            newRVRxtrans_rec.T_kol      = xtrVRgroup.vrKOL;

            // dodaj samo ako ima prekovrermenih, ili neku drugu 'neredovnu' vrstu rada 
            if(newRVRxtrans_rec.T_moneyD.NotZero() || newRVRxtrans_rec.T_strA_2.NotEmpty()) 
            {
               redesignedRVRxtransList.Add(newRVRxtrans_rec);
            }
         }
      }

      return redesignedRVRxtransList;
   }

   private void DumpPtranesOnTheGrid2(PlacaBaseDUC theDUC, List<Ptrane> ptraneList)
   {
      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      // Znaci da pobrise onaj prvi automatski redak nastao pri NewRecordClick
      if(theDUC.TheG.RowCount == 2) theDUC.TheG.Rows.Clear();

      foreach(Ptrane ptrane_rec in ptraneList)
      {
         theDUC.TheG2.Rows.Add();

         rowIdx = theDUC.TheG2.RowCount - idxCorrector;

         theDUC.PutDgvLineFields2(ptrane_rec, rowIdx, true);
      }
   }

   private List<VvSqlFilterMember> Get_RvrOrMvr_FilterMembers(DateTime dateTime, string mmyyyy, string wantedTT)
   {
      bool isMVR = false;
      if(wantedTT == Mixer.TT_MVR) isMVR = true;

      DateTime dateOd = new DateTime(dateTime.Year, dateTime.Month,  1);
      DateTime dateDo = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_tt     ], false, "theTT",  wantedTT    , "",                             ""          , " = " , ""));

      if(isMVR)
      {
         // u xtrans_rec.t_konto se nalazi t_MMYYYY, ali samo za MVR_DUC 
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_konto], false, "theMMYYYY", mmyyyy, "", "", " = ", ""));
      }
      else // mogli smo RVR po mmyyyy, ali smo tke kasnije ubacili mmyyyy na xtrans pa da ne zeznemo nesto staro od RVRa 
      {
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_dokDate], false, "DateOD", dateOd, dateOd.ToString("dd.MM.yyyy."), "Od datuma:", " >= ", ""));
         filterMembers.Add(new VvSqlFilterMember(ZXC.XtransSchemaRows[ZXC.XtrCI.t_dokDate], false, "DateDO", dateDo, dateDo.ToString("dd.MM.yyyy."), "Do datuma:", " <= ", ""));
      }

      return filterMembers;
   }

   private void TakeMVR(object sender, EventArgs e)
   {
      PlacaBaseDUC placaBaseDuc = TheVvUC as PlacaBaseDUC;

      if(placaBaseDuc.ThePolyGridTabControl.SelectedTab.Title != placaBaseDuc.TabPageTitle1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ovu akciju mozete inicirati samo sa TABa '{0}'.", placaBaseDuc.TabPageTitle1);
         return;
      }

      TakeRVRorMVR(Mixer.TT_MVR, EventArgs.Empty);
   }

   #endregion Ptrane From Xtrans 'RVR'

   internal/*private*/ void RecalcObustViaPostoIzNeto(object sender, EventArgs e)
   {
      //DialogResult result = MessageBox.Show("Da li zaista zelite nanovo izračunati 'Bruto1' preko koeficijenta?!",
      //   "Potvrdite Bruto1 preko koeficijenta?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      //
      //if(result != DialogResult.Yes) return;

      // ================================================================================== 

      int rIdx;
      PlacaBaseDUC placaBaseDuc = TheVvDocumentRecordUC as PlacaBaseDUC;
      VvDataGridView theG3 = placaBaseDuc.TheG3;
      //Ptrano ptrano_rec;
      //decimal bruto1;

      placaBaseDuc.GetFields(false); // Bussiness ok 

      decimal izNetoaSt;
      uint personCD;
      Ptrans ptrans_rec;
      decimal iznosOb;
      for(rIdx = 0; rIdx < theG3.RowCount - 1; ++rIdx)
      {
         izNetoaSt = theG3.GetDecimalCell(placaBaseDuc.DgvCI3.iT_izNetoaSt, rIdx, false);

         if(izNetoaSt.IsZero()) continue;

         personCD = theG3.GetUint32Cell(placaBaseDuc.DgvCI3.iT_personCD, rIdx, false);

         ptrans_rec = placaBaseDuc.placa_rec.Transes.SingleOrDefault(ptr => ptr.T_personCD == personCD);

         if(ptrans_rec == null) continue;

         // 09.11.2016: 
       //iznosOb = ZXC.VvGet_25_on_100(ptrans_rec.R_Netto   , izNetoaSt);
         iznosOb = ZXC.VvGet_25_of_100(ptrans_rec.R_ObustOsn, izNetoaSt);

         theG3.PutCell(placaBaseDuc.DgvCI3.iT_iznosOb, rIdx, iznosOb);

         
         
       //ptrano_rec = (Ptrano)placaBaseDuc.GetDgvLineFields3(rIdx, false, null);
       //placaBaseDuc.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
      }
   }

   private void Lock4ever_Placa(object sender, EventArgs e)
   {
      if((TheVvDataRecord as Placa).IsLocked == true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zapis je već odprije zaključan!");
         return;
      }

      DialogResult result = MessageBox.Show("Da li zaista želite ZAKLJUČATI ovaj dokument?", "Potvrdite ZAKLJUČAVANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if(result != DialogResult.Yes) return;

      BeginEdit(TheVvDataRecord);

      (TheVvDataRecord as Placa).IsLocked = true;

      (TheVvDataRecord as Placa).VvDao.RWTREC(TheDbConnection, TheVvDataRecord);

      EndEdit(TheVvDataRecord);

      WhenRecordInDBHasChangedAction(); // RRDREC 
   }

   // 03.03.2020: ciau nona Francesca 
   private void PRS_IBANFromKupDob(object sender, EventArgs e)
   {
      PersonUC thePersonUC = TheVvUC as PersonUC;

      bool isTekuci = thePersonUC.Fld_VrstaIsplate == Person.VrstaIsplateEnum.TEKUCI;

      Kupdob newKupdob_rec = new Kupdob();
      Person modPerson_rec = thePersonUC.person_rec.MakeDeepCopy();

      Person.CopyData_FromPerson_ToKupdob(newKupdob_rec, modPerson_rec, isTekuci);

      ZXC.TheGlobalVvDataRecord = newKupdob_rec; // !!! 

      KupdobListUC kupdobListUC = new KupdobListUC(this, newKupdob_rec, GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.KID));

      newKupdob_rec = (Kupdob)ZXC.TheVvForm.AddAndGetNewVvSifrarRecordInteractive(kupdobListUC);

      if(newKupdob_rec != null)
      {
         Person.CopyData_FromKupdob_ToPerson(modPerson_rec, newKupdob_rec, isTekuci);

         ZXC.WriteMode currWriteMode = TheVvTabPage.WriteMode;

         TheVvTabPage.WriteMode = ZXC.WriteMode.None; // da se ne dize SetDirtyFlag koji pak zjebe person_rec bussiness 

         thePersonUC.PutFields(modPerson_rec);

         TheVvTabPage.WriteMode = currWriteMode;

         SetVvMenuEnabledOrDisabled_Explicitly("ESC", true);
         SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);

      }
   }


   #endregion Placa

   #region Risk

   #region LinkToOtherDocum (RISK & MIXER)

   private void MIXER_CopyToSomeOtherRiskTT(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT("MIXER", e);
   }

   /*private*/ public void RISK_CopyToSomeOtherTT(object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      VvSubModul vvSubModul;

      // 12.01.2017: 
      if(TheVvDataRecord is Faktur && (TheVvDataRecord as Faktur).Is_STORNO)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne dozvoljava se kopiranje iz STORNO dokumenta.");
         return;
      }

      // 13.04.2016: 
      bool isAppropriateCopying = true;
      int polazniNumOf_T_cij_decimalPlaces = 0;
      int dolazniNumOf_T_cij_decimalPlaces = 0;

      bool isMixer2Risk = sender is string && ((string)sender) == "MIXER";

      // 01.03.2015: 
      if(sender is ZXC.VvSubModulEnum)
      {
         vvSubModul = GetVvSubModulFrom_SubModulEnum((ZXC.VvSubModulEnum)sender);
      }
      else // classic 
      {
       //VvCopyInNewTTDlg dlg = new VvCopyInNewTTDlg(isMixer2Risk ? TheVvUC as MixerDUC : TheVvUC as FakturDUC);
         VvCopyInNewTTDlg dlg = new VvCopyInNewTTDlg(TheVvUC as VvDocumentRecordUC);

         if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

         vvSubModul = dlg.TheVvSubModul;

         dlg.Dispose();
      }

      if(vvSubModul.subModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

      // --- Here we go! --- 

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      // 06.05.2016: 
      bool isRisk2Mixer = vvSubModul.modulEnum == ZXC.VvModulEnum.MODUL_OTHER;

      // ZXC.FakturRec = fakturToBeCopied_rec
      if(isMixer2Risk) // copying from MixerDUC to some RiskTT
      {
         ZXC.MixerRec  = (Mixer)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());
         ZXC.FakturRec = GetFakturRec_from_MixerRec((Mixer)(TheVvDataRecord.CreateNewRecordAndCloneItComplete()), vvSubModul/*.subModul_shortName*/);

         ZXC.MIXER_CopyToOtherDUC_inProgress = true;
         ZXC.RISK_CopyToOtherDUC_inProgress  = true;
      }

      else if(isRisk2Mixer) // 06.05.2016: copying from FakturDUC to some MixerDUC 
      {
         ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());
         ZXC.MixerRec  = GetMixerRec_from_FakturRec((Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete()), vvSubModul/*.subModul_shortName*/);

         ZXC.MIXER_CopyToOtherDUC_inProgress = true; // Da li DA ili NE? 
         ZXC.RISK_CopyToOtherDUC_inProgress  = true; // Da li DA ili NE? 
         ZXC.RISK_CopyToMixerDUC_inProgress  = true; 
      }
      else // classic
      {
         ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

         ZXC.RISK_CopyToOtherDUC_inProgress = true;

       //polazniNumOf_T_cij_decimalPlaces = (TheVvUC as FakturDUC).NumOf_T_cij_decimalPlaces;
         polazniNumOf_T_cij_decimalPlaces = Faktur.GetMaxCountOfSignificantDecimalPlaces(ZXC.FakturRec);
      }

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      if(isMixer2Risk == false && isRisk2Mixer == false)
      {
         dolazniNumOf_T_cij_decimalPlaces = (TheVvUC as FakturDUC).NumOf_T_cij_decimalPlaces;

         // ako cijena na dolaznom nije vidljiva tada uvijek moze 
         // a ako je vidljiva, trebabiti dovoljno velika da primi davaoca 
         isAppropriateCopying = polazniNumOf_T_cij_decimalPlaces <= dolazniNumOf_T_cij_decimalPlaces ||
                                dolazniNumOf_T_cij_decimalPlaces.IsNegative(); // ako cijena uopce nije vidljiva               
                                                                               // FakturDUC.NumOf_T_cij_decimalPlaces vraca -1 
      }

      // 13.04.2016: 
    //NewRecord_OnClick(isMixer2Risk ? ZXC.FakturRec : TheVvDataRecord, new NewRecordEventArgs(ZXC.FakturRec, (FakturDUC)TheVvUC, true));
      if(/*isAppropriateCopying*/true) // !!! privremeno suspendirano jer u fajlu je 8 dacimalnih 
      {
         if(e is NewRecordEventArgs && (e as NewRecordEventArgs).RecordUC is RNMDUC)
         {
            NewRecord_OnClick(TheVvDataRecord, (e as NewRecordEventArgs));
         }
         else // old, default
         {
            if(isRisk2Mixer) NewRecord_OnClick(               ZXC.MixerRec                   , new NewRecordEventArgs(ZXC.MixerRec , (MixerDUC )TheVvUC, true));
            else             NewRecord_OnClick(isMixer2Risk ? ZXC.FakturRec : TheVvDataRecord, new NewRecordEventArgs(ZXC.FakturRec, (FakturDUC)TheVvUC, true));
         }
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljeno kopiranje zbog broja decimala cijene!\n\npolazno {0} decimala\n\ndolazno {1} decimala", polazniNumOf_T_cij_decimalPlaces, dolazniNumOf_T_cij_decimalPlaces);
         TheTabControl_ClosePressed(TheTabControl, EventArgs.Empty);
      }

      // 31.03.2018: 
      SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");
   }

   private Faktur GetFakturRec_from_MixerRec(Mixer mixer_rec, VvSubModul subModul)
   {
      Faktur faktur_rec = SetFakturDataFromMixerData(mixer_rec, subModul);

      foreach(Xtrans xtrans_rec in mixer_rec.Transes)
      {
         faktur_rec.Transes.Add(SetRtransDataFromXtransData(mixer_rec, xtrans_rec));
      }

      return faktur_rec;
   }

   private Mixer GetMixerRec_from_FakturRec(Faktur faktur_rec, VvSubModul subModul)
   {
      Mixer mixer_rec = SetMixerDataFromFakturData(faktur_rec, subModul);

      foreach(Rtrans rtrans_rec in faktur_rec.Transes)
      {
         mixer_rec.Transes.Add(SetXtransDataFromRtransData(faktur_rec, rtrans_rec, subModul));
      }

      return mixer_rec;
   }

   private Faktur SetFakturDataFromMixerData(Mixer mixer_rec, VvSubModul subModul)
   {
      Faktur faktur_rec = new Faktur();

      #region Faktur from Mixer

      faktur_rec.TT       = subModul.subModul_shortName;

      faktur_rec.V1_tt    = mixer_rec.TT;
      faktur_rec.V1_ttNum = mixer_rec.TtNum;

      faktur_rec.DokDate  = mixer_rec.DokDate;

      faktur_rec.KupdobCD   = mixer_rec.KupdobCD;
      faktur_rec.KupdobName = mixer_rec.KupdobName;
      faktur_rec.KupdobTK   = mixer_rec.KupdobTK;

      faktur_rec.ProjektCD  = mixer_rec.ProjektCD;
      faktur_rec.OsobaX     = mixer_rec.StrG_40;

      #endregion Faktur from Mixer

      return faktur_rec;
   }

   private Mixer SetMixerDataFromFakturData(Faktur faktur_rec, VvSubModul subModul)
   {
      Mixer mixer_rec = new Mixer();

      #region Mixer from Faktur

      mixer_rec.TT         = subModul.subModul_shortName    ;
      mixer_rec.V1_tt      = faktur_rec.TT        ;
      mixer_rec.V1_ttNum   = faktur_rec.TtNum     ;
      mixer_rec.DokDate    = faktur_rec.DokDate   ;
      mixer_rec.KupdobCD   = faktur_rec.KupdobCD  ;
      mixer_rec.KupdobName = faktur_rec.KupdobName;
      mixer_rec.KupdobTK   = faktur_rec.KupdobTK  ;
      mixer_rec.ProjektCD  = faktur_rec.ProjektCD ;
      mixer_rec.StrG_40    = faktur_rec.OsobaX    ;

      #endregion Mixer from Faktur

      return mixer_rec;
   }

   private Rtrans SetRtransDataFromXtransData(Mixer mixer_rec, Xtrans xtrans_rec)
   {
      Rtrans rtrans_rec = new Rtrans();

      #region Rtrans from Xtrans

      rtrans_rec.T_artiklCD   = xtrans_rec.T_artiklCD;
      rtrans_rec.T_artiklName = xtrans_rec.T_artiklName;

      rtrans_rec.T_jedMj      = xtrans_rec.T_kpdbMjestoA_32;
      rtrans_rec.T_kol        = xtrans_rec.T_kol;
      rtrans_rec.T_cij        = xtrans_rec.T_moneyA;

         
      #endregion Rtrans from Xtrans

      return rtrans_rec;
   }

   private Xtrans SetXtransDataFromRtransData(Faktur faktur_rec, Rtrans rtrans_rec, VvSubModul subModul)
   {
      Xtrans xtrans_rec = new Xtrans();

      #region Xtrans from Rtrans

      xtrans_rec.T_artiklCD   = rtrans_rec.T_artiklCD;
      xtrans_rec.T_artiklName = rtrans_rec.T_artiklName;

      xtrans_rec.T_kpdbMjestoA_32 = rtrans_rec.T_jedMj;
      xtrans_rec.T_kol            = rtrans_rec.T_kol;
      xtrans_rec.T_moneyA         = rtrans_rec.T_cij;

      // NZI ('X_ZAH_RNM') override some stuff 
      if(subModul.subModulEnum == ZXC.VvSubModulEnum.X_ZAH_RNM)
      {
         xtrans_rec.T_moneyA = 0M;
      }

      #endregion Xtrans from Rtrans

      return xtrans_rec;
   }

   /*private*/ public void RISK_LinkToOtherDocum(object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      #region Init stuff

      if(AskLinkToOtherDocumConfirmation(sender) == false) return;

      bool OK = true;
      ToolStripButton btn = sender as ToolStripButton;
   
      if(btn.Text == "StartLink") btn.Text = "EndLink"  ; // toggle 
      else                        btn.Text = "StartLink"; // toggle 

      #endregion Init stuff

      #region Start Link

      if(ZXC.RISK_CopyToOtherDUC_inProgress  == false &&
         ZXC.MIXER_CopyToOtherDUC_inProgress == false) // Start Link 
      {
         ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());
         ZXC.RISK_CopyToOtherDUC_inProgress = true;
      }

      #endregion Start Link

      #region Ending link started from same-Faktur or kontra-Mixer DUC

      else 
      {
         #region Rwtrec EndPoint

         if(ZXC.IsSkyEnvironment == false) OK = Rwtrec_EndPoint_FAKTUR(); // Na FakturDUC-u zavrsavamo link zapocet na Faktur ili Mixer DUC-u 

         #endregion Rwtrec EndPoint
         
         #region Rwtrec StartPoint

         if(ZXC.IsSkyEnvironment == false) 
         {
            if(ZXC.MIXER_CopyToOtherDUC_inProgress == true) { if(OK) RwtrecLinkFeedback_MIXER (); } // Feedback na MixerDUC  koji je zapoceo ovaj link 
            else                                            { if(OK) RwtrecLinkFeedback_FAKTUR(); } // Feedback na FakturDUC koji je zapoceo ovaj link 
         }

         #endregion Rwtrec StartPoint

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

         ZXC.aim_emsg(MessageBoxIcon.Information, "Link uspostavljen.");

         EndLinkToOtherDocumActions();
      }

      #endregion Ending link started from MixerDUC (kontra DUC type)
   }

   private bool AskLinkToOtherDocumConfirmation(object sender)
   {
      ToolStripButton btn = sender as ToolStripButton;

      if(btn.Text != "StartLink") return true;

      VvStartLinkDlg dlg = new VvStartLinkDlg();

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return false;
      }

      dlg.Dispose();
      return true;
   }

   private void MIXER_LinkToOtherDocum(object sender, EventArgs e)
   {
      #region Init stuff

      if(AskLinkToOtherDocumConfirmation(sender) == false) return;

      bool OK = true;
      ToolStripButton btn = sender as ToolStripButton;
      
      if(btn.Text == "StartLink") btn.Text = "EndLink"  ; // toggle 
      else                        btn.Text = "StartLink"; // toggle 

      #endregion Init stuff

      #region Start Link

      if(ZXC.RISK_CopyToOtherDUC_inProgress  == false &&
         ZXC.MIXER_CopyToOtherDUC_inProgress == false) // Start Link 
      {

         ZXC.MixerRec = (Mixer)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());
         ZXC.MIXER_CopyToOtherDUC_inProgress = true;
      }

      #endregion Start Link

      #region Ending link started from same-Mixer or kontra-Faktur DUC

      else 
      {
         #region Rwtrec EndPoint

         OK = Rwtrec_EndPoint_MIXER(); // Na MixerDUC-u zavrsavamo link zapocet na Faktur ili Mixer DUC-u 

         #endregion Rwtrec EndPoint
         
         #region Rwtrec StartPoint

         if(ZXC.IsSkyEnvironment == false) 
         {
            if(ZXC.RISK_CopyToOtherDUC_inProgress == true) { if(OK) RwtrecLinkFeedback_FAKTUR(); } // Feedback na FakturDUC koji je zapoceo ovaj link 
            else                                           { if(OK) RwtrecLinkFeedback_MIXER (); } // Feedback na MixerDUC  koji je zapoceo ovaj link 
         }

         #endregion Rwtrec StartPoint

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

         ZXC.aim_emsg(MessageBoxIcon.Information, "Link uspostavljen.");

         EndLinkToOtherDocumActions();
      }

      #endregion Ending link started from MixerDUC (kontra DUC type)
   }

   private void EndLinkToOtherDocumActions()
   {
      ZXC.FakturRec = null; ZXC.RISK_CopyToOtherDUC_inProgress  = false;
      ZXC.MixerRec  = null; ZXC.MIXER_CopyToOtherDUC_inProgress = false;

      ZXC.RISK_CopyToMixerDUC_inProgress = false;

      if(ats_SubModulSet[TheVvTabPage.SubModul_xy.X][TheVvTabPage.SubModul_xy.Y].Items["StartLink"] != null)
      {
         ats_SubModulSet[TheVvTabPage.SubModul_xy.X][TheVvTabPage.SubModul_xy.Y].Items["StartLink"].Text = "StartLink";
      }
   }

   private bool Rwtrec_EndPoint_FAKTUR()
   {
      Faktur faktur_rec = (Faktur)(TheVvDataRecord);

      bool OK = true;

      // 13.09.2021:
      if(faktur_rec.TtInfo.IsV1andV2specialUseTT ||
      ZXC.FakturRec.TtInfo.IsV1andV2specialUseTT) return OK;

      BeginEdit(faktur_rec);

      string tt;
      uint   ttNum;

      if(ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         tt    = ZXC.MixerRec.TT;
         ttNum = ZXC.MixerRec.TtNum;
      }
      else
      {
         tt    = ZXC.FakturRec.TT;
         ttNum = ZXC.FakturRec.TtNum;
      }

      switch(faktur_rec.FirstEmptyVezaLine)
      {
         case 1: faktur_rec.V1_tt    = tt;
                 faktur_rec.V1_ttNum = ttNum;
                 break;
         case 2: faktur_rec.V2_tt    = tt;
                 faktur_rec.V2_ttNum = ttNum;
                 break;
         case 3: faktur_rec.V3_tt    = tt;
                 faktur_rec.V3_ttNum = ttNum;
                 break;
         case 4: faktur_rec.V4_tt    = tt;
                 faktur_rec.V4_ttNum = ttNum;
                 break;

         // 29.10.2021: ubili warning default: ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'primaocu' nema više mjesta za upis nove veze."); OK = false;  break;
      }

      if(OK) OK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false);

    //if(OK && ZXC.IsSkyEnvironment) VvDaoBase.SendWriteOperationToSKY(TheDbConnection, faktur_rec, VvSQL.DB_RW_ActionType.RWT, /*true*/ false);

      EndEdit(faktur_rec);

      return OK;
   }

   private bool Rwtrec_EndPoint_MIXER()
   {
      Mixer mixer_rec = (Mixer)(TheVvDataRecord);

      bool OK = true;

      BeginEdit(mixer_rec);

      string tt;
      uint   ttNum;

      if(ZXC.MIXER_CopyToOtherDUC_inProgress == false)
      {
         tt    = ZXC.FakturRec.TT;
         ttNum = ZXC.FakturRec.TtNum;
      }
      else
      {
         tt    = ZXC.MixerRec.TT;
         ttNum = ZXC.MixerRec.TtNum;
      }

      switch(mixer_rec.FirstEmptyVezaLine)
      {
         case 1: mixer_rec.V1_tt    = tt;
                 mixer_rec.V1_ttNum = ttNum;
                 break;
         case 2: mixer_rec.V2_tt    = tt;
                 mixer_rec.V2_ttNum = ttNum;
                 break;

            // 29.10.2021: ubili warning default: ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Mixer 'primaocu' nema više mjesta za upis nove veze."); OK = false; break;
      }

      if(OK) OK = mixer_rec.VvDao.RWTREC(TheDbConnection, mixer_rec, false, true, false);

      EndEdit(mixer_rec);

      return OK;
   }

   private bool RwtrecLinkFeedback_FAKTUR()
   {
      bool OK = true;
      
      // 13.09.2021:
      if(ZXC.FakturRec.TtInfo.IsV1andV2specialUseTT) return OK;

      BeginEdit(ZXC.FakturRec);

      string tt;
      uint   ttNum;

      if(TheVvDataRecord is Mixer)
      {
         tt    = ((Mixer)TheVvDataRecord).TT   ;
         ttNum = ((Mixer)TheVvDataRecord).TtNum;
      }
      else
      {
         tt    = ((Faktur)TheVvDataRecord).TT   ;
         ttNum = ((Faktur)TheVvDataRecord).TtNum;
      }

      switch(ZXC.FakturRec.FirstEmptyVezaLine)
      {
         case 1: ZXC.FakturRec.V1_tt    = tt;
                 ZXC.FakturRec.V1_ttNum = ttNum;
                 break;
         case 2: ZXC.FakturRec.V2_tt    = tt;
                 ZXC.FakturRec.V2_ttNum = ttNum;
                 break;
         case 3: ZXC.FakturRec.V3_tt    = tt;
                 ZXC.FakturRec.V3_ttNum = ttNum;
                 break;
         case 4: ZXC.FakturRec.V4_tt    = tt;
                 ZXC.FakturRec.V4_ttNum = ttNum;
                 break;

         // 29.10.2021: ubili warning 
       //default:   ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'davaocu' nema više mjesta za upis nove veze.");   OK = false; break;
         default: /*ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'davaocu' nema više mjesta za upis nove veze.");*/ OK = false; break;
      }

      if(OK) OK = ZXC.FakturRec.VvDao.RWTREC(TheDbConnection, ZXC.FakturRec, false, true, false);

      //if(OK && ZXC.IsSkyEnvironment) VvDaoBase.SendWriteOperationToSKY(TheDbConnection, ZXC.FakturRec, VvSQL.DB_RW_ActionType.RWT, /*true*/ false);

      EndEdit(ZXC.FakturRec);

      return OK;
   }

   private bool RwtrecLinkFeedback_MIXER()
   {
      bool OK = true;

      if(ZXC.RISK_CopyToMixerDUC_inProgress) BeginEdit(ZXC.FakturRec);
      else                                   BeginEdit(ZXC.MixerRec );

      string tt;
      uint   ttNum;

      if(TheVvDataRecord is Faktur)
      {
         tt    = ((Faktur)TheVvDataRecord).TT   ;
         ttNum = ((Faktur)TheVvDataRecord).TtNum;
      }
      else
      {
         tt    = ((Mixer)TheVvDataRecord).TT   ;
         ttNum = ((Mixer)TheVvDataRecord).TtNum;
      }

      if(ZXC.RISK_CopyToMixerDUC_inProgress)
      {
         switch(ZXC.FakturRec.FirstEmptyVezaLine)
         {
            case 1:  ZXC.FakturRec.V1_tt    = tt;
                     ZXC.FakturRec.V1_ttNum = ttNum;
                     break;
            case 2:  ZXC.FakturRec.V2_tt    = tt;
                     ZXC.FakturRec.V2_ttNum = ttNum;
                     break;

               // 29.10.2021: ubili warning default: ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'davaocu' nema više mjesta za upis nove veze."); OK = false; break;
         }
      }
      else // classic
      {
         switch(ZXC.MixerRec.FirstEmptyVezaLine)
         {
            case 1:  ZXC.MixerRec.V1_tt    = tt;
                     ZXC.MixerRec.V1_ttNum = ttNum;
                     break;
            case 2:  ZXC.MixerRec.V2_tt    = tt;
                     ZXC.MixerRec.V2_ttNum = ttNum;
                     break;

               // 29.10.2021: ubili warning default: ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Mixer 'davaocu' nema više mjesta za upis nove veze."); OK = false; break;
         }
      }

      if(OK)
      {
         if(ZXC.RISK_CopyToMixerDUC_inProgress) OK = ZXC.FakturRec.VvDao.RWTREC(TheDbConnection, ZXC.FakturRec, false, true, false);
         else                                   OK = ZXC.MixerRec .VvDao.RWTREC(TheDbConnection, ZXC.MixerRec , false, true, false);
      }

      if(ZXC.RISK_CopyToMixerDUC_inProgress) EndEdit(ZXC.FakturRec);
      else                                   EndEdit(ZXC.MixerRec );

      return OK;
   }

   #endregion LinkToOtherDocum (RISK & MIXER)

   /*private*/ public void RISK_ShowExternCijena(object sender, EventArgs e)
   {
      if(ZXC.IsSvDUH)
      {
         RISK_SVD_PairHALMED_Artikl(sender, e);
         return;
      }

      if(ZXC.IsTEXTHOshop) return;

      Artikl artikl_rec = TheVvDataRecord as Artikl;

      #region GetExterniCjenik

      decimal theMSRP = 0.00M;

      Xtrans xtrans_rec = new Xtrans();

    //bool success = XtransDao.GetExterniCjenik(TheDbConnection, xtrans_rec, artikl_rec.ArtiklCD,               false);
      theMSRP      = XtransDao.GetExterniCjenik(TheDbConnection, xtrans_rec, artikl_rec.ArtiklCD, DateTime.Now, false);

      //if(success) theMSRP = xtrans_rec.T_moneyA;
      //else        theMSRP = 0.00M;

      #endregion GetExterniCjenik

      decimal devTecaj = ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrans_rec.T_konto), DateTime.Now);

      NiceKnDlg dlg = new NiceKnDlg(0, artikl_rec.ArtiklCD, artikl_rec.ArtiklName, xtrans_rec.T_konto, devTecaj, DateTime.Now, theMSRP);

      dlg.ShowDialog();

   }

   /*private*/ public void RISK_RenameArtiklCD(object sender, EventArgs e)
   {
      VvRenameArtiklDlg dlg = new VvRenameArtiklDlg(false);
      Artikl     artikl_rec = TheVvDataRecord as Artikl;
         
      string oldArtiklCD = artikl_rec.ArtiklCD;

      dlg.Fld_NewArtikl_CD_or_Name = oldArtiklCD;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newArtiklCD = dlg.Fld_NewArtikl_CD_or_Name;

      dlg.Dispose();

      bool duplicateDetected = false;

      // 14.03.2024: 
      if(TheVvUC.Get_Artikl_FromVvUcSifrar(newArtiklCD) != null)
      {
         DialogResult result = MessageBox.Show("UPOZORENJE! Nova šifra  [" + newArtiklCD + "] već postoji. Preimenovati će se dokumenti ali NE i sama kartica artikla.",
            "Da li želite nastaviti?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;

         duplicateDetected = true;

         //ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno! Nova šifra [{0}] već postoji.", newArtiklCD);
         //return;
      }

      VvDataRecord arhivedRenamedArtikl_rec = artikl_rec.CreateArhivedDataRecord(TheDbConnection, "AUTO_REN");

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted artikl only                                                                                                                                            
      DataRow drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_artiklCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtikl", oldArtiklCD, " = "));

      int nora = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newArtiklCD);

      // 06.11.2023: dodano i za Rtrano 

      List<VvSqlFilterMember> filterMembers2 = new List<VvSqlFilterMember>(1);
      // For wanted artikl only                                                                                                                                            
      DataRow drSchema2 = ZXC.RtranoDao.TheSchemaTable.Rows[ZXC.RtranoDao.CI.t_artiklCD];
      filterMembers2.Add(new VvSqlFilterMember(drSchema2, "elArtikl", oldArtiklCD, " = "));

      int nora2 = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers2, drSchema2, newArtiklCD);

      if(nora >= 0 && nora2 >= 0)
      {
         bool OK = false;

         if(duplicateDetected == false)
         { 
            BeginEdit(artikl_rec);
            artikl_rec.ArtiklCD = newArtiklCD;
            OK = TheVvDao.RWTREC(TheDbConnection, artikl_rec);
            EndEdit(artikl_rec);

            if(OK) arhivedRenamedArtikl_rec.VvDao.ADDREC(TheDbConnection, arhivedRenamedArtikl_rec, true, true, false, false);
         }

         PutFieldsActions(TheDbConnection, artikl_rec, TheVvRecordUC);

         if(OK) ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, kao i samu karticu."   , nora, (string)(drSchema["BaseTableName"]));
         else   ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, ALI NE i samu karticu!", nora, (string)(drSchema["BaseTableName"]));

         // 14.11.2012: now, we shoulde rename ARTSTAT also! 
         if(OK)
         {
            filterMembers = new List<VvSqlFilterMember>(1);
            // For wanted artikl only                                                                                                                                            
            drSchema = ZXC.ArtStatDao.TheSchemaTable.Rows[ZXC.ArtStatDao.CI.t_artiklCD];
            filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtikl", oldArtiklCD, " = "));

            nora = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newArtiklCD);
         }
      }

      if(ZXC.Should_SynchronizeArtiklSifrar)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Ovo preimenovanje se NEĆE automatski izvršiti i u šifrarniku 'partner' firme pa ga tamo morate ponoviti.");
      }
   }

   /*private*/ public void RISK_RenameArtiklName(object sender, EventArgs e)
   {
      VvRenameArtiklDlg dlg = new VvRenameArtiklDlg(true);
      Artikl     artikl_rec = TheVvDataRecord as Artikl;
         
      string oldArtiklName = artikl_rec.ArtiklName;

      dlg.Fld_NewArtikl_CD_or_Name = oldArtiklName;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newArtiklName = dlg.Fld_NewArtikl_CD_or_Name;

      dlg.Dispose();

      bool duplicateDetected = false;

      // 14.03.2024: 
      if(TheVvUC.Get_Artikl_FromVvUcSifrarByName(newArtiklName) != null)
      {
         DialogResult result = MessageBox.Show("UPOZORENJE! Novi naziv [" + newArtiklName + "] već postoji. Preimenovati će se dokumenti ali NE i sama kartica artikla.", 
            "Da li želite nastaviti?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;

         duplicateDetected = true;

         //ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno! Novi naziv [{0}] već postoji.", newArtiklName);
         //return;
      }

      VvDataRecord arhivedRenamedArtikl_rec = artikl_rec.CreateArhivedDataRecord(TheDbConnection, "AUTO_REN");

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);
      // For wanted artikl only                                                                                                                                            
      DataRow drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtransDao.CI.t_artiklName];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtikl", oldArtiklName, " = "));

      int nora = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newArtiklName);

      // 06.11.2023: dodano i za Rtrano 

      List<VvSqlFilterMember> filterMembers2 = new List<VvSqlFilterMember>(1);
      // For wanted artikl only                                                                                                                                            
      DataRow drSchema2 = ZXC.RtranoDao.TheSchemaTable.Rows[ZXC.RtranoDao.CI.t_artiklName];
      filterMembers2.Add(new VvSqlFilterMember(drSchema2, "elArtikl", oldArtiklName, " = "));

      int nora2 = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers2, drSchema2, newArtiklName);

      if(nora >= 0 && nora2 >= 0)
      {
         bool OK = false;

         if(duplicateDetected == false)
         {
            BeginEdit(artikl_rec);
            artikl_rec.ArtiklName = newArtiklName;
            OK = TheVvDao.RWTREC(TheDbConnection, artikl_rec);
            EndEdit(artikl_rec);

            if(OK) arhivedRenamedArtikl_rec.VvDao.ADDREC(TheDbConnection, arhivedRenamedArtikl_rec, true, true, false, false);
         }

         PutFieldsActions(TheDbConnection, artikl_rec, TheVvRecordUC);

         if(OK) ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, kao i samu karticu."   , nora, (string)(drSchema["BaseTableName"]));
         else   ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka <{1}> datoteke, ALI NE i samu karticu!", nora, (string)(drSchema["BaseTableName"]));

         // No need for renameNaziv (ArtiklName nema u ArtStat-u)
       //// 14.11.2012: now, we shoulde rename ARTSTAT also! 
       //if(OK)
       //{
       //   filterMembers = new List<VvSqlFilterMember>(1);
       //   // For wanted artikl only                                                                                                                                            
       //   drSchema = ZXC.ArtStatDao.TheSchemaTable.Rows[ZXC.ArtStatDao.CI.t_artiklCD];
       //   filterMembers.Add(new VvSqlFilterMember(drSchema, "elArtikl", oldArtiklName, " = "));
       //
       //   nora = ZXC.ArtiklDao.RenameForeignKey(TheDbConnection, filterMembers, drSchema, newArtiklName);
       //}
      }

      if(ZXC.Should_SynchronizeArtiklSifrar)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Ovo preimenovanje se NEĆE automatski izvršiti i u šifrarniku 'partner' firme pa ga tamo morate ponoviti.");
      }

   }

   internal void RISK_ToggleKnDeviza(object sender, EventArgs e)
   {
      ZXC.RISK_ToggleKnDeviza_InProgress = true;

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      if(theDUC == null ||
         theDUC.Fld_DevNameAsEnum == ZXC.ValutaNameEnum.EMPTY ||
         theDUC.Fld_DevNameAsEnum == /*ZXC.ValutaNameEnum.HRK*/ZXC.EURorHRK_NameEnum) return;

      theDUC.IsShowingConvertedMoney = !theDUC.IsShowingConvertedMoney; // toggle 

      theDUC.DevTecaj = ZXC.DevTecDao.GetHnbTecaj(theDUC.Fld_DevNameAsEnum, theDUC.Fld_DokDate); //orig - vrati kasnije!!!

      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) // mi smo upravo u Dodaj ili Ispravi i zelimo preracunati deviznu cijenu u kune, a moze Boga mi i obrnuto 
      {
         if(theDUC.DevTecaj == 1.00M) // nije uspio naci tecaj 
         {
            theDUC.IsShowingConvertedMoney = !theDUC.IsShowingConvertedMoney; // toggle 
            return;
         }

         theDUC.TheG.EndEdit(); // ! ostavi ovo 

         ConvertFakturAllCijenaToKuneOrDeviza(theDUC, theDUC.IsShowingConvertedMoney);
            
         theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
      }

      if(theDUC.IsShowingConvertedMoney == true) // idemo iz kuna u neku drugu valutu 
      {
         theDUC.Fld_ShowInValutaLookUp = theDUC.Fld_DevNameAsEnum; // Fld_ShowInValutaLookUp ti istovremeno mijenja i ValutaNameInUse! 
      }
      else // vracamo se iz neke valute u kune
      {
         theDUC.Fld_ShowInValutaLookUp = ZXC.ValutaNameEnum.EMPTY; // Fld_ShowInValutaLookUp ti istovremeno mijenja i ValutaNameInUse! 
      }

      // 31.3.2011:
      //if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) theDUC.GetDgvFields1(false); // da digne MarkTransesToDelete 
      // ma ipak ne 

      //theDUC.PutFields(theDUC.mixer_rec, false); Fld_ShowInValutaLookUp digne PutFields() 

      ToggleDevizaVisualApperiance(theDUC.IsShowingConvertedMoney, theDUC.Fld_DevName, theDUC.Fld_DevTecaj);

      ZXC.RISK_ToggleKnDeviza_InProgress = false;

   }

   private void ConvertFakturAllCijenaToKuneOrDeviza(FakturExtDUC theDUC, bool _isShowingConvertedMoney)
   {
      int cIdx;
      decimal theDecimal;

      // 14.09.2011: 
      #region ZTR additions

      if(theDUC.CtrlOK(theDUC.tbx_S_ukZtr))
      {
         if(_isShowingConvertedMoney) // daj kune u devizu 
         {
            theDUC.Fld_S_ukZtr =  ZXC.DivSafe(theDUC.Fld_S_ukZtr, theDUC.DevTecaj);
         }
         else // daj devizu u kune
         {
            theDUC.Fld_S_ukZtr = (theDUC.Fld_S_ukZtr * theDUC.DevTecaj);
         }
      }

      if(theDUC.CtrlOK(theDUC.tbx_S_ukKCRP_NP1))
      {
         if(_isShowingConvertedMoney) // daj kune u devizu 
         {
            theDUC.Fld_S_ukKCRP_NP1 = ZXC.DivSafe(theDUC.Fld_S_ukKCRP_NP1, theDUC.DevTecaj);
         }
         else // daj devizu u kune
         {
            theDUC.Fld_S_ukKCRP_NP1 = (theDUC.Fld_S_ukKCRP_NP1 * theDUC.DevTecaj);
         }
      }

      #endregion ZTR additions

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         cIdx = theDUC.DgvCI.iT_cij;

         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);

            // 12.10.2016: BIG NEWS: 
            // 22.04.2024: 
          //if(_isShowingConvertedMoney == false                        ) theDecimal = theDecimal.Ron2();
            if(_isShowingConvertedMoney == false && !ZXC.IsTETRAGRAM_ANY) theDecimal = theDecimal.Ron2();

            // 12.10.2016:  BIG NEWS: 
            // 22.04.2024: 
          //if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
          //if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj).Ron2());
            if(_isShowingConvertedMoney) /* daj kune u devizu */
            { 
               if(ZXC.IsTETRAGRAM_ANY) theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal, theDUC.DevTecaj)       );
               else                    theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal, theDUC.DevTecaj).Ron2());
            }
            else                         /* daj devizu u kune */
            { 
               theDUC.TheG.PutCell(cIdx, rIdx, (theDecimal * theDUC.DevTecaj));
            }
         }

         cIdx = theDUC.DgvCI.iT_cij_kcr;
         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);
            if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
            else                         /* daj devizu u kune */ theDUC.TheG.PutCell(cIdx, rIdx,            (theDecimal * theDUC.DevTecaj));
         }
         cIdx = theDUC.DgvCI.iT_cij_kcrm;
         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);
            if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
            else                         /* daj devizu u kune */ theDUC.TheG.PutCell(cIdx, rIdx,            (theDecimal * theDUC.DevTecaj));
         }
         cIdx = theDUC.DgvCI.iT_cij_kcrp;
         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);
            if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
            else                         /* daj devizu u kune */ theDUC.TheG.PutCell(cIdx, rIdx,            (theDecimal * theDUC.DevTecaj));
         }
         cIdx = theDUC.DgvCI.iT_cij_MSK;
         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);
            if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
            else                         /* daj devizu u kune */ theDUC.TheG.PutCell(cIdx, rIdx,            (theDecimal * theDUC.DevTecaj));
         }

         cIdx = theDUC.DgvCI.iT_IRA_MPC;
         if(theDUC.TheG.CI_OK(cIdx))
         {
            theDecimal = theDUC.TheG.GetDecimalCell(cIdx, rIdx, false);
            if(_isShowingConvertedMoney) /* daj kune u devizu */ theDUC.TheG.PutCell(cIdx, rIdx, ZXC.DivSafe(theDecimal,  theDUC.DevTecaj));
            else                         /* daj devizu u kune */ theDUC.TheG.PutCell(cIdx, rIdx,            (theDecimal * theDUC.DevTecaj));
         }

      }
   }

   // PAZI! kad je EURO ERA u pitanju, _devTecaj ovdje dolazi RECIPROČAN 
   public void ToggleDevizaVisualApperiance(bool _isShowingConvertedMoney, string _devName, string _devTecajstr)
   {
      VvHamper.ApplyVVColorAndStyleTabCntrolChange(TheVvTabPage);

      TheVvTabPage.labDeviza.Visible = _isShowingConvertedMoney;

      string labelaText = "Iznosi u " + _devName + " (tečaj: ";

      decimal devTecaj      = 0M;
      decimal devTecajRecip = 0M;
      string  devTecajRecipStr = "";

      if(_devTecajstr.NotEmpty()) 
      {
         Decimal.TryParse(_devTecajstr, out devTecaj);
         devTecajRecip = 1.00M / devTecaj;
         devTecajRecipStr = devTecajRecip.ToStringVvKolDecimalPlaces(6);
      }

    //if(_isShowingConvertedMoney) TheVvTabPage.labDeviza.Text = "Iznosi u " + _devName + " " + _devTecajstr;
      if(_isShowingConvertedMoney)
      {
         if(ZXC.IsEURoERA_projectYear) labelaText += "1 EUR = " + devTecajRecipStr + " " + _devName + ")";
         else                          labelaText += _devTecajstr + ")";

         TheVvTabPage.labDeviza.Text = labelaText;
      }

      if(TheVvUC is FakturDUC && _isShowingConvertedMoney) // sad jos jemput provjeri; ne bi li mozda terebalo pokazati kaki error message 
      {
         (TheVvUC as FakturDUC).SetWarningColorsAndLabel();
      }
   }

   private void RISK_PromjenaNacPlac(object sender, EventArgs e)
   {
      #region init

      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName  &&
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Privilegirana akcija. Odbijeno.");
         return;
      }

      string NEW_nacPlacCd  ;
      bool   NEW_nacPlacFlag;

      Cursor.Current = Cursors.WaitCursor;

      FakturExtDUC theDUC      = TheVvDocumentRecordUC as FakturExtDUC;
      Faktur       faktur_rec1 = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

      // 12.07.2024: 
      if(ZXC.RRD.Dsc_IsM2PAY && faktur_rec1.IsNpCash == false) // nemre mjenjati ak je bila kartica 
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ne može se mjenjati način plaćanja kod već autoriziranog kartičnog plaćanja.");
         return;
      }

      #endregion init

      #region dlg

      // 12.07.2024: 
      if(ZXC.RRD.Dsc_IsM2PAY)
      {
         NEW_nacPlacCd   = "VISA";
         NEW_nacPlacFlag = false ;
      }
      else // classic 
      { 
         VvPromjenaNacPlacDlg dlg = new VvPromjenaNacPlacDlg();

         if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

         NEW_nacPlacCd   = dlg.Fld_NacPlac ;
         NEW_nacPlacFlag = dlg.Fld_IsNpCash;

         dlg.Dispose();
      }

      #endregion dlg

      if(NEW_nacPlacCd == theDUC.Fld_NacPlac)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadali ste isti način plaćanja, operacija je nepotrebna.");
         return;
      }

      ZXC.RISK_PromjenaNacPlac_inProgress = true;

      RISK_Storno(/*sender*/"RISK_PromjenaNacPlac", e);

      bool OK = SaveVvDataRecord(sender, e); // ovaj sejva 2. 

      if(!OK) { ZXC.RISK_PromjenaNacPlac_inProgress = false; return; }

      Faktur faktur_rec2 = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

      FakturDUC.GoTo_RISK_Dokument(faktur_rec1.TT, faktur_rec1.TtNum);

      DupCopyMenu_Button_OnClick(sender, e);
      
      theDUC.Fld_NacPlac  = NEW_nacPlacCd  ;
      theDUC.Fld_IsNpCash = NEW_nacPlacFlag;
      
      OK = SaveVvDataRecord(sender, e); // ovaj sejva 3. 

      // 12.07.2024: 
      if(!OK || ZXC.FakturRec is null) { ZXC.RISK_PromjenaNacPlac_inProgress = false; return; }

      Faktur faktur_rec3 = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

      ZXC.RISK_PromjenaNacPlac_inProgress = false;

      if(OK && ZXC.RRD.Dsc_IsIrmQuickPrint)
      {
         if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
         {
            ZXC.SetStatusText("IRM Quick Print in progress");
            QuickPrintMenu_Button_OnClick("SaveRecord_OnClick", EventArgs.Empty);
            ZXC.ClearStatusText();
         }
      }

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "PROMIJENJEN NAČIN PLAĆANJA:\n\r\n\rRačun {0} sa načinom plaćanja \"{1}\" je storniran\n\r\n\rračunom {2}\n\r\n\rRačun {3} mijenja način plaćanja rn-a {0} na \"{4}\"",
         faktur_rec1.TT_And_TtNum, faktur_rec1.NacPlac, 
         faktur_rec2.TT_And_TtNum,
         faktur_rec3.TT_And_TtNum, faktur_rec3.NacPlac);

      if(OK && ZXC.RRD.Dsc_IsIrmQuickPrint)
      {
         if(TheVvUC is IRMDUC || TheVvUC is IRMDUC_2)
         {
            NewRecord_OnClick("SaveRecord_OnClick", EventArgs.Empty);
         }
      }

   }

   private void RISK_Storno_inNY(object sender, EventArgs e)
   {
      ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Ovo je eksperimentalno rješenje.\n\r\n\rPotrebno je dobro provjeriti sadržaj novonastalog računa u novoj godini!!!");
   
      XSqlConnection nextYearDbConn = ZXC.TheSecondDbConn_SameDB_OtherYear(DateTime.Now.Year);

      bool OK;

      ZXC.CopyOut_InProgress = true;

      Faktur origFaktur_rec = (Faktur)(TheVvDataRecord);

      Faktur nyFaktur_rec = (Faktur)origFaktur_rec.CreateNewRecordAndCloneItComplete();

      OK = VvRecLstUC.EnsureNonDuplicateKeys(nextYearDbConn, nyFaktur_rec, false, false);

      #region Alter fields for storno

      foreach(Rtrans rtrans in nyFaktur_rec.Transes)
      {
         rtrans.T_kol *= -1.00M;

         rtrans.CalcTransResults(null);
      }

      nyFaktur_rec.TakeTransesSumToDokumentSum(true);

      nyFaktur_rec.VezniDok = origFaktur_rec.TtNumFiskal;

      // !!! !!! !!! 
      if(origFaktur_rec.IsF2)
      {
         nyFaktur_rec.F2_PrvFakYYiRecID = origFaktur_rec.GetFaktur_YYandRecID; // referenca na prethodni dokument 
      }

      string stornoStr = "STORNO iz " + origFaktur_rec.DokDate.Year + " god. "+ origFaktur_rec.TT + "-" + origFaktur_rec.TtNum + "/";

      string oldNapomena = origFaktur_rec.Napomena;

      nyFaktur_rec.Napomena  = ZXC.LenLimitedStr(stornoStr + oldNapomena, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena));

      if(nyFaktur_rec.S_ukKCRP_NP1.NotZero()) nyFaktur_rec.S_ukKCRP_NP1 *= -1.00M;

      if(nyFaktur_rec.IsF2)
      {
         nyFaktur_rec.StatusCD = "384";
      }

      nyFaktur_rec.DokDate  =
      nyFaktur_rec.SkladDate =
      nyFaktur_rec.PdvDate  = 
      nyFaktur_rec.DospDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
      nyFaktur_rec.RokPlac  = 0;

      nyFaktur_rec.FiskJIR =
      nyFaktur_rec.FiskZKI =
      nyFaktur_rec.V1_tt   =
      nyFaktur_rec.V2_tt   =
      nyFaktur_rec.V3_tt   =
      nyFaktur_rec.V4_tt   = "";

      nyFaktur_rec.V1_ttNum =
      nyFaktur_rec.V2_ttNum =
      nyFaktur_rec.V3_ttNum =
      nyFaktur_rec.V4_ttNum = 0;

      #endregion Alter fields for storno

      OK = ZXC.FakturDao.ADDREC(nextYearDbConn, nyFaktur_rec, true, false, false, false);

      ZXC.CopyOut_InProgress = false;

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, $"Dodan storno ovog računa u NOVOJ {DateTime.Now.Year} godini.\n\r\n\r{nyFaktur_rec}");
      else   ZXC.aim_emsg(MessageBoxIcon.Stop,        $"Storno ovog računa u NOVOJ {DateTime.Now.Year} godini.\n\r\n\rNIJE USPIO!");
   }
   public void RISK_Storno(object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      bool isPGfaktur = (TheVvDataRecord as Faktur).DokDate.Year != DateTime.Now.Year;
      
      if(isPGfaktur)
      {
         DialogResult result = MessageBox.Show($"Da li želite proizvesti storno u NOVOJ {DateTime.Now.Year} godini?",
            "Potvrdite STORNO u NG", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      
         if(result == DialogResult.Yes)
         {
            RISK_Storno_inNY(sender, e);
            return;
         }
      }

      // 21.09.2022: 
      if(ZXC.IsTEXTHOshop && ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName && ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Privilegirana akcija. Odbijeno.");
         return;
      }

      if((TheVvDocumentRecordUC as FakturExtDUC).faktur_rec.Is_F2_Avans == true)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "'Običan Storno' se ne smije raditi na avansu, za avans se mora pozvati 'Avans Storno'.");

         return;
      }

      bool isRISK_PromjenaNacPlac = (sender is string) && (sender as string) == "RISK_PromjenaNacPlac";

      DupCopyMenu_Button_OnClick(sender, e);

      // 12.07.2024: 
      if(ZXC.FakturRec is null) return;

    //FakturDUC    theDUC = TheVvDocumentRecordUC as FakturDUC   ;
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      decimal old_kol;

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         old_kol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, rIdx, false);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, rIdx, (-old_kol));

         theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
      }

      theDUC.Fld_VezniDok = /*theDUC.faktur_rec*/ZXC.FakturRec.TtNumFiskal;

      // !!! !!! !!! 
      if(ZXC.FakturRec.IsF2)
      {
       //theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.               RecID; // referenca na prethodni dokument 
         theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.GetFaktur_YYandRecID; // referenca na prethodni dokument 
      }

      string stornoStr   = "STORNO " + (/*ZXC.RISK_PromjenaNacPlac_inProgress*/isRISK_PromjenaNacPlac ? "ZaNoviNP " : "") + theDUC.faktur_rec.BackupData._tt + "-" + theDUC.faktur_rec.BackupData._ttNum + "/";
      string oldNapomena = theDUC.faktur_rec.Napomena;

      if(oldNapomena.Length + stornoStr.Length <= theDUC.tbx_Napomena.MaxLength) theDUC.Fld_Napomena  = stornoStr + oldNapomena;
      else                                                                       theDUC.Fld_Napomena  = stornoStr + oldNapomena.Substring(0, oldNapomena.Length - stornoStr.Length);

      // 26.01.2024: 
      if(theDUC.Fld_S_ukKCRP_NP1.NotZero()) theDUC.Fld_S_ukKCRP_NP1 = theDUC.Fld_S_ukKCRP_NP1 * -1.00M;

      if(theDUC.faktur_rec.IsF2)
      {
         // TODO. ne znamo kako tocno ova 2 polja trebaju pri stornu iygledati 
         // ostqvljamo ih kakvi su bili na orig rn. koji se stornira           

       //theDUC.Fld_eRproc =  ZXC.VvUBL_PolsProcEnum.P04; 
         VvLookUpItem lui; // = ZXC.luiListaeRacPoslProc.GetLuiForThisCd(/*theDUC.Fld_eRproc.ToString()*/ "4"); 
       //if(lui != null) theDUC.Fld_eRprocOpis = lui.Name;
         
         lui = ZXC.luiListaKodTipaEracuna.GetLuiForThisCd("384"); 
         theDUC.Fld_StatusCD   = lui != null ? lui.Cd   : "";
         theDUC.Fld_StatusOpis = lui != null ? lui.Name : "";
      }

   }

   private void RISK_PrikaziRUC(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

      FakturExtDUC fakExDuc = TheVvUC as FakturExtDUC;

      if(fakExDuc.TheG.Columns["R_RUC"].Visible) SetVisibilityRUCcolumns(false, fakExDuc);
      else                                       SetVisibilityRUCcolumns(true , fakExDuc);
   }
 
   private void SetVisibilityRUCcolumns(bool isVisible, FakturExtDUC fakDuc)
   {
      fakDuc.      TheG.Columns["R_cij_kcr"].Visible = 
      fakDuc.      TheG.Columns["R_RUC"]    .Visible = 
      fakDuc.      TheG.Columns["R_RUV"]    .Visible = 
      fakDuc.      TheG.Columns["R_NV" ]    .Visible = 
      fakDuc.      TheG.Columns["R_NC" ]    .Visible =
      fakDuc.TheSumGrid.Columns["R_cij_kcr"].Visible =
      fakDuc.TheSumGrid.Columns["R_RUC"]    .Visible =
      fakDuc.TheSumGrid.Columns["R_RUV"]    .Visible =
      fakDuc.TheSumGrid.Columns["R_NV" ]    .Visible =
      fakDuc.TheSumGrid.Columns["R_NC" ]    .Visible = isVisible;

      if(fakDuc is IRA_MPC_DUC || fakDuc is PON_MPC_DUC || fakDuc is OPN_MPC_DUC)
      {
         fakDuc.TheG.Columns["R_utilString"].Visible = isVisible;
      }
   }

   private void RISK_PrikaziPPMV(object sender, EventArgs e)
   {
      FakturExtDUC fakExDuc = TheVvUC as FakturExtDUC;

      if(fakExDuc.TheG.Columns["T_ppmvOsn"].Visible) SetVisibiltiPpmvColumns(false, fakExDuc);
      else                                           SetVisibiltiPpmvColumns(true , fakExDuc);
   }

   private void SetVisibiltiPpmvColumns(bool isVisible, FakturExtDUC fakDuc)
   {
      fakDuc.      TheG.Columns["T_ppmvOsn"  ].Visible = 
      fakDuc.      TheG.Columns["T_ppmvSt1i2"].Visible = 
      fakDuc.      TheG.Columns["R_ppmvIzn"  ].Visible = 
      fakDuc.TheSumGrid.Columns["T_ppmvOsn"  ].Visible =
      fakDuc.TheSumGrid.Columns["T_ppmvSt1i2"].Visible =
      fakDuc.TheSumGrid.Columns["R_ppmvIzn"  ].Visible = isVisible;
  }

   /*private*/ public void RISK_7030(object sender, EventArgs e)
   {
      //VvMessageBoxDLG dlg = new VvMessageBoxDLG();
      //dlg.ShowDialog();

      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;
      int currRowIdx = theDUC.TheG.CurrentRow.Index;

      Artikl artikl_rec = null;
      string currArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, currRowIdx, false);
      if(currArtiklCD.NotEmpty()) artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == currArtiklCD);

      theDUC.TheG.Rows.Insert(currRowIdx, theDUC.TheG.CloneWithValues(theDUC.TheG.CurrentRow));

      if(currArtiklCD.NotEmpty() && artikl_rec != null && artikl_rec.LinkArtCD.NotEmpty())
      {
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD, currRowIdx + 1, artikl_rec.LinkArtCD);
         theDUC.TheG.CurrentCell = theDUC.TheG[theDUC.DgvCI.iT_artiklCD, currRowIdx + 1];
         theDUC.TheG.BeginEdit(false);
         theDUC.originalText = "";
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
      }

      decimal cij100 = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_cij, currRowIdx, false);
      decimal cij70 = (cij100 * 0.70M).Ron2();
      decimal cij30 = (cij100 - cij70).Ron2();

      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij, currRowIdx, cij70);
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij, currRowIdx + 1, cij30);

      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);
      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx + 1);
   }

   /*private*/ public void RISK_PdvOmjer(object sender, EventArgs e)
   {
      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;
      int currRowIdx   = theDUC.TheG.CurrentRow.Index;

      Artikl artikl_rec = null;
      string currArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, currRowIdx, false);
      if(currArtiklCD.NotEmpty()) artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == currArtiklCD);

      theDUC.TheG.Rows.Insert(currRowIdx, theDUC.TheG.CloneWithValues(theDUC.TheG.CurrentRow));

      if(currArtiklCD.NotEmpty() && artikl_rec != null && artikl_rec.LinkArtCD.NotEmpty())
      {
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD, currRowIdx + 1, artikl_rec.LinkArtCD);
         theDUC.TheG.CurrentCell = theDUC.TheG[theDUC.DgvCI.iT_artiklCD, currRowIdx + 1];
         theDUC.TheG.BeginEdit(false);
         theDUC.originalText = "";
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
      }

      decimal osnUkup   = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_cij  , currRowIdx, false);
      decimal pdvSt     = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_pdvSt, currRowIdx, false);
      decimal osnMoze   = (osnUkup * ZXC.RRD.Dsc_OmjerPdv / 100.00M).Ron2();
      decimal osnNemoze = (osnUkup - osnMoze).Ron2();

      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij      , currRowIdx    , osnMoze);
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij      , currRowIdx + 1, osnNemoze);
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvKolTip, currRowIdx    , "M"  );
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvKolTip, currRowIdx + 1, "N"  );

      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx    );
      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx + 1);
   }

   private void RISK_ColChooser(object sender, EventArgs e)
   {
      //FakturDUC fakturDuc = TheVvUC as FakturDUC;

      //if(fakturDuc.TheColChooserGrid.Visible)
      //{
      //   fakturDuc.TheColChooserGrid .Visible = false;
      //}
      //else
      //{
      //   fakturDuc.TheColChooserGrid .Visible = true;
      //}

      //if(fakturDuc is FakturExtDUC) fakturDuc.CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_WidthChooserGrid(fakturDuc.TheG, ZXC.QunMrgn, ZXC.Qun8, fakturDuc.hamp_twin   , fakturDuc.hamp_IznosUvaluti, fakturDuc.TheColChooserGrid.Visible , 0);
      //else                          fakturDuc.CalcLocationSizeAnchor_TheDGVAndTheSumGrid_WidthTbxBottomOfSumGrid_WidthChooserGrid(fakturDuc.TheG, ZXC.QunMrgn, ZXC.Qun8, fakturDuc.hamp_SukKC_K, fakturDuc.hamp_IznosUvaluti, fakturDuc.TheColChooserGrid .Visible, 0);
   }
 
   private void RISK_Fak2NalogRules(object sender, EventArgs e)
   {
//      ////////////////////////////////////////////
//      NalogDUC theDUC = TheVvDocumentRecordUC as NalogDUC;
//      decimal usedAddition = theDUC.nalog_rec.SetFtransesInBalance(theDUC.nalog_rec.Saldo); // VOILA 
//
//#if DEBUG
//      ZXC.aim_emsg(MessageBoxIcon.Information, "usedAddition = {0}", usedAddition.ToStringVv());
//#endif
//      theDUC.PutDgvFields();
//      return;
//      ////////////////////////////////////////////

      Fak2NalogRulesDlg dlg = new Fak2NalogRulesDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetKtoShemaDscFields();

      dlg.Dispose();

   }

   public void RISK_DeleteDeadArtikls(object sender, EventArgs e)
   {

      DialogResult result = MessageBox.Show("Da li zaista zelite obrisati 'mrtve' kartice\'",
         "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if (result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.DeleteDeadArtikls(TheDbConnection);

      Cursor.Current = Cursors.Default;

   }

   public void RISK_NewCache/*ORIG*/(object sender, EventArgs e)
   {
      // 17.09.2017: 
      if(ZXC.IsTEXTHOshop)
      {
         ZXC.SENDtoSKY_InProgress      = 
         ZXC.RECEIVEfromSKY_InProgress = false;
      }

      string recordName = ArtStat.recordName;

      if((sender is string == false) || sender.ToString() != "SKY")
      {
         DialogResult result = MessageBox.Show("Da li zaista zelite REKREIRATI \'" + recordName + "\'", "Potvrdite REKREIRANJE Cache-a?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;
      }

      Cursor.Current = Cursors.WaitCursor;

      ZXC.RISK_NewCache_InProgress = true;

      // 11.01.2013: da ne skace DbConnection Exception kada usred regeneriranja CACHE-a trebas jos 
      RiskRulesDsc dummy = ZXC.RRD; // Da digne ucitavanje rules-a 

      bool OK;
      
      string dbName = ZXC.TheVvDatabaseInfoIn_ComboBox4Projects.DataBaseName;

      OK = VvSQL.DROP_TABLE(TheDbConnection, dbName, recordName);

      if(!OK) return;

      VvSQL.CheckTableVersion_AndCatchUpIfNeeded(TheDbConnection, dbName, recordName);

    //ArtiklDao.CheckCache(TheDbConnection);
      ArtiklDao.CheckCache(TheDbConnection, true);

      ZXC.RISK_NewCache_InProgress = false;

      Cursor.Current = Cursors.Default;
   }

   /*private*/ public void RISK_NewThisCache/*ORIG*/(object sender, EventArgs e)
   {
      ArtiklUC theUC      = TheVvUC         as ArtiklUC;
      Artikl   artikl_rec = TheVvDataRecord as Artikl  ;
      Rtrans   rtrans_rec = new Rtrans()               ;

      rtrans_rec.T_artiklCD = artikl_rec.ArtiklCD;
      rtrans_rec.T_skladCD  = theUC.Fld_ZaSkladCD;

      if(rtrans_rec.T_skladCD.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajte Skladište!");
         return;
      }
      DialogResult result = MessageBox.Show("Da li zaista zelite rekreirati Artstat [" + rtrans_rec.T_artiklCD + "] / [" + rtrans_rec.T_skladCD + "] ?",
         "Potvrdite NEW CACHE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      decimal old_UkUlazKolAll    = artikl_rec.AS_UkUlazKolAll   ;
      decimal old_UkIzlazKol      = artikl_rec.AS_UkIzlazKol     ;
      decimal old_StanjeKol       = artikl_rec.AS_StanjeKol      ;
      decimal old_KnjigCij        = artikl_rec.AS_KnjigCij       ;
      decimal old_UkUlazFinKNJAll = artikl_rec.AS_UkUlazFinKNJAll;
      decimal old_UkIzlazFinKNJ   = artikl_rec.AS_UkIzlazFinKNJ  ;
      decimal old_StanjeFinKNJ    = artikl_rec.AS_StanjeFinKNJ   ;

      ZXC.RtransDao.Delete_Then_Renew_Cache_FromThisRtrans(TheDbConnection, rtrans_rec, VvSQL.DB_RW_ActionType.DEL);
      ZXC.RtransDao.FailedIzlazTwinsListManager(TheDbConnection, VvSQL.DB_RW_ActionType.DEL);

      ReRead_OnClick(null, EventArgs.Empty); // refresh record 

      decimal new_UkUlazKolAll    = artikl_rec.AS_UkUlazKolAll   ;
      decimal new_UkIzlazKol      = artikl_rec.AS_UkIzlazKol     ;
      decimal new_StanjeKol       = artikl_rec.AS_StanjeKol      ;
      decimal new_KnjigCij        = artikl_rec.AS_KnjigCij       ;
      decimal new_UkUlazFinKNJAll = artikl_rec.AS_UkUlazFinKNJAll;
      decimal new_UkIzlazFinKNJ   = artikl_rec.AS_UkIzlazFinKNJ  ;
      decimal new_StanjeFinKNJ    = artikl_rec.AS_StanjeFinKNJ   ;

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Regeneriranje je gotovo.");

      if(old_UkUlazKolAll    != new_UkUlazKolAll   ) ZXC.aim_emsg(MessageBoxIcon.Warning, "uKol {0} / {1}"  , old_UkUlazKolAll   .ToStringVv()                 , new_UkUlazKolAll   .ToStringVv                 ());
      if(old_UkIzlazKol      != new_UkIzlazKol     ) ZXC.aim_emsg(MessageBoxIcon.Warning, "iKol {0} / {1}"  , old_UkIzlazKol     .ToStringVv()                 , new_UkIzlazKol     .ToStringVv                 ());
      if(old_StanjeKol       != new_StanjeKol      ) ZXC.aim_emsg(MessageBoxIcon.Warning, "kolSt {0} / {1}" , old_StanjeKol      .ToStringVv()                 , new_StanjeKol      .ToStringVv                 ());
      if(old_KnjigCij        != new_KnjigCij       ) ZXC.aim_emsg(MessageBoxIcon.Warning, "knjCij {0} / {1}", old_KnjigCij       .ToStringVvKolDecimalPlaces(4), new_KnjigCij       .ToStringVvKolDecimalPlaces(4));
      if(old_UkUlazFinKNJAll != new_UkUlazFinKNJAll) ZXC.aim_emsg(MessageBoxIcon.Warning, "uFin {0} / {1}"  , old_UkUlazFinKNJAll.ToStringVv()                 , new_UkUlazFinKNJAll.ToStringVv                 ());
      if(old_UkIzlazFinKNJ   != new_UkIzlazFinKNJ  ) ZXC.aim_emsg(MessageBoxIcon.Warning, "iFin {0} / {1}"  , old_UkIzlazFinKNJ  .ToStringVv()                 , new_UkIzlazFinKNJ  .ToStringVv                 ());
      if(old_StanjeFinKNJ    != new_StanjeFinKNJ   ) ZXC.aim_emsg(MessageBoxIcon.Warning, "finSt {0} / {1}" , old_StanjeFinKNJ   .ToStringVv()                 , new_StanjeFinKNJ   .ToStringVv                 ());

   }

   private void RISK_NewThisCachePPMV(object sender, EventArgs e) // PPMV na auto 
   {
      ArtiklUC theUC      = TheVvUC         as ArtiklUC;
      Artikl   artikl_rec = TheVvDataRecord as Artikl  ;

      decimal listPrice = artikl_rec.ImportCij;
      ushort  emisCO2   = artikl_rec.EmisCO2  ;
      ZXC.EuroNormaEnum euroNorma = artikl_rec.EuroNorma;

      DateTime firstRegistr = artikl_rec.AtestDate;
      DateTime today        = DateTime.Now;
      int      monthAge     = ZXC.MonthDifference(today, firstRegistr);
      decimal  tecaj        = ZXC.DevTecDao.GetHnbTecaj(ZXC.ValutaNameEnum.EUR, today);

      decimal korigStopaFor_MonthAge      = Artikl.GetListPriceKorigStopaFor_MonthAge(monthAge);
      decimal estimatedPriceByMonthAgeEUR = listPrice * korigStopaFor_MonthAge;
      decimal estimatedPriceByMonthAgeKN  = estimatedPriceByMonthAgeEUR * tecaj;

      decimal realPriceEUR     = artikl_rec.TheAsEx.PreDefVpc1;
      decimal co2TAXstopa      = Artikl.GetPpmvStopaFor_CO2_Dizel(emisCO2, euroNorma);
      decimal priceTAXstopa    = Artikl.GetPpmvStopaFor_PorOsn(estimatedPriceByMonthAgeKN);
      decimal co2TAXvalueEUR   = ZXC.VvGet_25_of_100(estimatedPriceByMonthAgeEUR, co2TAXstopa  );
      decimal priceTAXvalueEUR = ZXC.VvGet_25_of_100(estimatedPriceByMonthAgeEUR, priceTAXstopa);
      decimal totalTAXvalueEUR = ZXC.DivSafe(Artikl.Get_Ppmv_Iznos_1i2(estimatedPriceByMonthAgeKN, emisCO2, 0M, euroNorma, true), tecaj);

      decimal upperEURlimit4priceStopa9 = (400000M / tecaj);
      string message = "";

      message += "First registration\t: " + firstRegistr.ToString(ZXC.VvDateFormat) + " (" + monthAge.ToString() + " mj.) " + korigStopaFor_MonthAge.ToStringVv() + " od " + listPrice.ToString0Vv() + " = " + estimatedPriceByMonthAgeEUR.ToString0Vv() + "\n";
      
      message += "limit  9% ---> 400.000kn = " + upperEURlimit4priceStopa9.ToString0Vv() + "eur\t" + 
         (estimatedPriceByMonthAgeEUR < upperEURlimit4priceStopa9 ? 
         "it's ok. ("    + estimatedPriceByMonthAgeEUR.ToString0Vv() + " < " + upperEURlimit4priceStopa9.ToString0Vv() + ")": 
         "IS NOT OK! ("  + estimatedPriceByMonthAgeEUR.ToString0Vv() + " > " + upperEURlimit4priceStopa9.ToString0Vv() + ")") + "\n";

      message += "limit 60% ---> 36 mjeseci vs " + monthAge.ToString() + "mj\t" +
         (monthAge >= 36 ?
         "it's ok.   " :
         "IS NOT OK! " ) + "\n";

      message += "CO2 emission\t: "       + emisCO2                                 + " (" + co2TAXstopa.ToString0Vv() + "%)\n";

      message += "\nTAX\t\tosnova\t\tstopa\t\tiznos" + "\n\n";
      message += "CO2\t\t"       + estimatedPriceByMonthAgeEUR.ToString0Vv() + "\t\t" + co2TAXstopa                  .ToString0Vv() + "%\t\t" + co2TAXvalueEUR  .ToString0Vv() + "\n";
      message += "PorOsnEUR\t"   + estimatedPriceByMonthAgeEUR.ToString0Vv() + "\t\t" + priceTAXstopa                .ToString0Vv() + "%\t\t" + priceTAXvalueEUR.ToString0Vv() + "\n";
      message += "TOTAL\t\t"     + estimatedPriceByMonthAgeEUR.ToString0Vv() + "\t\t" + (co2TAXstopa + priceTAXstopa).ToString0Vv() + "%\t\t" + totalTAXvalueEUR.ToString0Vv() + "\n";

      message += "\n\n";

      decimal realPriceEURinHR = (realPriceEUR / 1.19M * 1.25M);

      message += "OglasPrice " + realPriceEUR.ToString0Vv() + "\t-mwst+pdv " + realPriceEURinHR.ToString0Vv() + " +PPMV " + totalTAXvalueEUR.ToString0Vv() + " = " + (realPriceEURinHR + totalTAXvalueEUR).ToString0Vv() + "\n";
      message += "OglasPrice " + realPriceEUR.ToString0Vv() + "\t\t               "                           + " +PPMV " + totalTAXvalueEUR.ToString0Vv() + " = " + (realPriceEUR     + totalTAXvalueEUR).ToString0Vv() + "\n";

      ZXC.aim_emsg(MessageBoxIcon.Information, message);
   }

   private void RISK_NewThisCacheTestSendMail(object sender, EventArgs e)
   {
      VvMailClient mailClient = new VvMailClient();

      mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword; // !!! ovo moze biti i prazno! Tada ida anonymously?! 

      mailClient.MailHost             = ZXC.ViperMailHost             ;
      mailClient.EmailFromAddress     = ZXC.SkyLabEmailAddress        ;
      mailClient.EmailFromDisplayName = ZXC.SkyLabEmailFromDisplayName;
    //mailClient.EmailTo              = ZXC.VektorEmailAddress        ;
      mailClient.MessageSubject       = "Testis SUBJECT";
      mailClient.MessageBody          = "TrlaBabaLanDaJojProđeDan\n\nQ'";

      string attachmentFullFileNamePath = ZXC.Vv_GZip_ThisFile(@"E:\DLtest.txt");

    //mailClient.MailAttachmentFileNameList = new System.Net.Mail.Attachment(attachmentFullFileNamePath);
      mailClient.MailAttachmentFileNameList = new string[] { attachmentFullFileNamePath };

      mailClient.SendMail_Normal(true, ZXC.VektorEmailAddress); // ili / ili 
    //mailClient.SendMail_Async (true, ZXC.VektorEmailAddress); // ili / ili 

      // delete temporary GZip file:
      try { System.IO.File.Delete(attachmentFullFileNamePath); } catch(Exception ex) { ZXC.aim_emsg("Delete File Error:\n\n{0}", ex.Message); }
   }

   /*private*/ public void RISK_FakturRules(object sender, EventArgs e)
   {
      RiskRulesDlg dlg = new RiskRulesDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      dlg.TheUC.GetDscFields();

      dlg.Dispose();

   }

   /*private*/ public void RISK_RewriteAllDocuments(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

      uint lastDokNum;
      bool skip = false;
      FakturDUC theDUC = TheVvUC as FakturDUC;

      ZXC.RewriteAllDocuments_InProgress = true;

      LastRecord_OnClick(sender, e);
      lastDokNum = (TheVvDataRecord as Faktur).DokNum;

      FirstRecord_OnClick(sender, e);

      do
      {
         skip = false;
         //// 30.04.2013: PPUK uKol 2 decijale na 3 decimale 
         //if(theDUC is PIZpDUC)
         //{
         //   if(theDUC.faktur_rec.Transes2.Any(rto => rto.T_serno.NotEmpty()) == false) skip = true;
         //}

         if(skip == false)
         {
            ExecuteRewrite(theDUC, sender, e);
         }

         NextRecord_OnClick(sender, e);

      } while((TheVvDataRecord as Faktur).DokNum != lastDokNum);

      // Za zadnjega
      ExecuteRewrite(theDUC, sender, e);

      RISK_NewCache(sender, e);

      ZXC.RewriteAllDocuments_InProgress = false;
   }

   private void ExecuteRewrite(FakturDUC theDUC, object sender, EventArgs e)
   {
      EditRecord_OnClick(sender, e);

      // 28.04.2016: TABOUT NEEDED?! 
      SendKeys.Send("{TAB}");

      // 30.04.2013: PPUK uKol 2 decijale na 3 decimale 
      if(theDUC is PIZpDUC)
      {
         // 08.07.2015:
       //ForeachRtrano_WithTserno_RewriteTkolFromPopratnica(theDUC as PIZpDUC);
       //CalcVolumen_SynthesizeG2toG1(sender, e);

         //08.07.2015 ovo ostavi jer nakon svakog javljanja revalorizacije mora se pozvati RISK_RewriteAllDocuments
         PutAndCalcOtpad(/*sender*/"RISK_RewriteAllDocuments", e);
      }

      theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

      SaveRecord_OnClick(sender, e);
   }

   private void ForeachRtrano_WithTserno_RewriteTkolFromPopratnica(PIZpDUC thePIZpDUC)
   {
      Rtrano rtrano_rec;

      for(int rIdx = 0; rIdx < thePIZpDUC.TheG2.RowCount - 1; ++rIdx)
      {
         rtrano_rec = (Rtrano)thePIZpDUC.GetDgvLineFields2(rIdx, false, null);

         if(rtrano_rec.T_serno.IsEmpty()) continue;

         // ________ Here we go! ________ 

         #region Get Incoming Rtrano, PutDgvFields2

         List<Rtrano> rtranoList = RtranoDao.GetRtranoList_For_SERNO(TheDbConnection, rtrano_rec.T_serno);

         bool rtranoNOTfound   = rtranoList.Count.IsZero();
         bool rtranoMULTIfound = rtranoList.Count > 1;

         if(rtranoNOTfound)
         {
            ZXC.aim_emsg("NEMA {0}", rtrano_rec.T_serno);
            continue;
         }

         //if(rtranoMULTIfound)
         //{
         //   string rtranos = "";
         //   foreach(Rtrano rtrano in rtranoList)
         //   {
         //      rtranos += rtrano + "\n\n";
         //   }

         //   ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE:\n\nZa serno:\n\n{0}\n\nsam pronašao više ({1}) knjiženja!\n\n{2}\n", vvtb_editingControl.Text, rtranoList.Count, rtranos);
         //}

         Rtrano firstFoundRtrano_rec = rtranoList.First();

         thePIZpDUC.TheG2.PutCell(thePIZpDUC.DgvCI2.iT_kol, rIdx, firstFoundRtrano_rec.T_kol);

         #endregion Get Incoming Rtrano, PutDgvFields2
      }

   }

   private void RISK_PrenosPS_JOB(/*bool isPrNBConly*/)
   {
      ZXC.VvDataBaseInfo dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      PrenosPsDLG dlg = new PrenosPsDLG(dbi.ProjectPreviousYear, dbi.ProjectYear, false);

      dlg.Fld_DBName = ZXC.VvDB_NameConstructor(dbi.ProjectPreviousYear, dbi.ProjectName, dbi.ProjectKcdAsUInt);

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         Cursor.Current = Cursors.WaitCursor;

         bool OK = FakturDao.PS_RISK(TheDbConnection, dlg.Fld_DBName, dbi.ProjectPreviousYear/*, isPrNBConly*/, dlg.Fld_skladCD);

         ShowNews();

         Cursor.Current = Cursors.Default;
      }

      dlg.Dispose();
   }

   private void RISK_PrenosPS           (object sender, EventArgs e) { RISK_PrenosPS_JOB(     ); }
 //private void RISK_PrenosPS_prNBC_Only(object sender, EventArgs e) { RISK_PrenosPS_JOB(true ); }

   private void RISK_AutoAddInventuraDiff(object sender, EventArgs e)
   {
      DialogResult result = MessageBox.Show("Da li zaista zelite kreirati primke i izdatnice viška/manjka!",
         "Potvrdite automatsko dodavanje INVENTURNIH viškova/manjkova?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.RISK_AutoAddInventuraDiff_inProgress = true;

    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      FakturDUC    theDUC = TheVvDocumentRecordUC as FakturDUC   ;

      uint PRIcount, IZDcount, KLKcount, IZMcount;

      bool OK = FakturDao.AutoAddInventuraDiff_RISK(TheDbConnection, theDUC.Fld_DokDate, out PRIcount, out IZDcount, out KLKcount, out IZMcount);

      //ShowNews();

      //theDUC.OnVvTBEnter_SetAutocmplt_Kupdob_sorterName(theDUC.tbx_KupdobName, EventArgs.Empty);

      ZXC.RISK_AutoAddInventuraDiff_inProgress = false;

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nPRI Vel primka viška: [{0}]\n\nIZD Vel izdat manjka: [{1}]\n\nKLK Mal primka viška: [{2}]\n\nIZM Mal izdat manjka: [{3}]\n\n",
         PRIcount, IZDcount, KLKcount, IZMcount);
   }

   private void ShowNews()
   {
      bool IsNotEmptyTable =

         TheVvDataRecord.VvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, VvSQL.DBNavigActionType.LST, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(IsNotEmptyTable) { TheVvTabPage.FileIsEmpty = false; PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC); }
      else                { TheVvTabPage.FileIsEmpty = true;                                                                     }

      SetVvMenuEnabledOrDisabled_RegardingWriteMode(ZXC.WriteMode.None);
   }

   private void RISK_PreTank(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

      #region theRtransListIRM & init stuff

      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;

      if(theDUC.Fld_TT      .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajate, molim, TT (tip transakcije).");         return; }
      if(theDUC.Fld_SkladCD2.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajate, molim, SkladCD2 (dolazno skladiste)."); return; }
      if(theDUC.Fld_DokDate == DateTime.MinValue) { ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajate, molim, datum.");              return; }

      string preTankForTT;
      switch(theDUC.Fld_TT)
      {
         case Faktur.TT_VMI: preTankForTT = Faktur.TT_IRM; break;

         default: throw new Exception("RISK_PreTank(): za TT " + theDUC.Fld_TT + "UNDONE!");
      }

      List<Rtrans>            theRtransListIRM = new List<Rtrans>();
      List<VvSqlFilterMember> filterMembers    = new List<VvSqlFilterMember>(4);
      DataRowCollection rtrSch = ZXC.RtransSchemaRows;  
      RtransDao.RtransCI rtrCI = ZXC.RtrCI;

      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_isIrmUslug], false, "theIsUsluga", false              , "", "", " = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_tt        ], false, "theTT"      , preTankForTT       , "", "", " = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladCD   ], false, "theSklCD"   , theDUC.Fld_SkladCD2, "", "", " = ", "", "R"));
      filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladDate ], false, "theDate"    , theDUC.Fld_DokDate , "", "", " = ", "", "R"));

    //VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, theRtransListIRM,     filterMembers, "", Rtrans.artiklOrderBy_ASC);
      RtransDao.GetRtransWithArtstatList   (TheDbConnection, theRtransListIRM, "", filterMembers,     Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

      // 31.01.2012: Ovo stvaralo BUG-ove! [Vidi (ponovi) VMI za Zagria.2012.IRM-99 i IRM-112 na 09.01.2012. pa ces vidjeti neravnotezu u KPM-u za 09.01.2012] 
      //for(int i = 0; i < theRtransListIRM.Count; i++) // Remove one koji nisu u minusu. Infact, this is supress duplicate 'PreTank' operation 
      //{
      //   if(theRtransListIRM[i].A_StanjeKol.IsZeroOrPositive()) { theRtransListIRM.Remove(theRtransListIRM[i]); --i; }
      //}

      TtInfo ttInfo = ZXC.TtInfo(theDUC.Fld_TT);

      #endregion init stuff

      if(theRtransListIRM.Count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nema se što preTankati na dan {0} za skladište {1}.\n\nIli je već preTankano ili nema prometa.", theDUC.Fld_DokDate.ToString(ZXC.VvDateFormat), theDUC.Fld_SkladCD2);
         return;
      }

    //var artAndCijGRP = theRtransListIRM.GroupBy(rtr => rtr.T_artiklCD + rtr.T_cij).Select(grp => new // <ALFA> 
      var artAndCijGRP = theRtransListIRM.GroupBy(rtr => rtr.T_artiklCD            ).Select(grp => new // <BETA> 
      {
         t_artiklCD = grp.First().T_artiklCD,
         t_cij      = grp.First().T_cij, // <ALFA> 
       //t_cij      = grp.Last ().T_cij, // <BETA> 

         //t_kolSUM   = grp.Sum(g => g.T_kol)
         t_kolSUM = -grp.Last().A_StanjeKol, // samo za koliko je u minusu 

         // kontrola 
         numOfDistinctCijena = grp.Distinct().Count()

      });

      Rtrans  rtrans ;
      ArtStat artstat;

      foreach(var grp in artAndCijGRP)
      {
       //rtrans  = theRtransListIRM.First(rtr => rtr.T_artiklCD == grp.t_artiklCD && rtr.T_cij == grp.t_cij); // <ALFA> 
         rtrans  = theRtransListIRM.Last (rtr => rtr.T_artiklCD == grp.t_artiklCD                          ); // <BETA> 

         artstat = ArtiklDao.GetArtiklStatus(TheDbConnection, grp.t_artiklCD, theDUC.Fld_SkladCD, theDUC.Fld_DokDate);

         decimal prNabCij     = artstat != null ? artstat.PrNabCij     : 0.00M;
       //decimal lastMalopCij = artstat != null ? artstat.LastMalopCij : 0.00M; 

         rtrans.T_kol                = grp.t_kolSUM;
       //rtrans.T_wanted             = lastMalopCij; IPAK NE! 
         rtrans.T_wanted             = grp.t_cij      ; // MPC (R_cij_MSK) 
         rtrans.T_cij                = prNabCij       ; // NBC (T_cij    ) idem ipak bez Ron2()?!      
       //rtrans.T_cij                = prNabCij.Ron2(); // NBC (T_cij    ) Ron2() dodan tek 25.01.2012 
         rtrans.T_mCalcKind          = ZXC.MalopCalcKind.By_MPC;
         rtrans.T_rbt1St             = 0.00M; // IRM-ov rabat treba ignorirati, jer malop razduzujemo po MPC prije rabata 
         rtrans.SaveTransesWriteMode = ZXC.WriteMode.Add;
         rtrans.T_recID              = 0;
         rtrans.T_TT                 = ttInfo.TwinTT; // da digne VMU calc 

         rtrans.CalcTransResults(null);

         theDUC.faktur_rec.Transes.Add(rtrans);
      }

      theDUC.PutDgvFields1();

      theDUC.PutTransSumToDocumentSumFields();

      SetDirtyFlag(this);
   }

   #region Change All Values for some Column 

   /*private*/ public void RISK_PrNabCij               (object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.PrNabCij        ); }
   /*private*/ public void RISK_PrNabCij_wMPC          (object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.PrNabCij_wMPC   ); }
   /*private*/ public void RISK_PostaviMPC             (object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.MalopCij        ); }
   /*private*/ public void RISK_PostaviPdvSt           (object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.PdvSt           ); }
   private void RISK_PostaviKol             (object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.Kol             ); }
   // novo u 2021: 
   private void RISK_PostaviPrNabCij_And_Kol(object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.PrNabCij_And_Kol); }

   // 07.07.2022: 
   private void RISK_SVD_INVkol_korekcija(object sender, EventArgs e) { RISK_SetColumnValues(RISK_ColumnKindEnum.SVD_INVkol_korekcija); }

   /*private*/
   internal enum RISK_ColumnKindEnum { PrNabCij, MalopCij, Kol, PrNabCij_And_Kol, SVD_INVkol_korekcija, PdvSt, PrNabCij_wMPC }
   internal enum RISK_CB_Column_Actions { Select_All, Deselect_All, Invert_Selection }

   /*private*/internal void RISK_SetColumnValues(RISK_ColumnKindEnum columnKind)
   {
      // 04.01.2021:
    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      FakturDUC    theDUC = TheVvDocumentRecordUC as FakturDUC   ;

      decimal newDecimalValue;
      string  artiklCD;
      ArtStat artstat_rec;
      int colIdx;

      Cursor.Current = Cursors.WaitCursor;

    //DateTime SVD_INVkol_korekcija_Date_PocetakRazdoblja = new DateTime(2022, 06, 29);
      DateTime SVD_INVkol_korekcija_Date_PocetakRazdoblja = new DateTime(2022, 06, 30);
      DateTime SVD_INVkol_korekcija_Date_KrajRazdoblja    = new DateTime(2022, 07, 01);

      for(int rowIdx = 0; rowIdx < theDUC.TheG.RowCount - 1; ++rowIdx)
      {
       //if(theDUC.TheG.CI_OK(theDUC.DgvCI.iT_artiklCD)) artiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, rIdx, false);
       //else                                            artiklCD = "";
       //artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artiklCD, theDUC.Fld_SkladCD);
       //artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)theDUC.GetDgvLineFields1(rowIdx, false, null));
         Rtrans rtrans_rec = (Rtrans)theDUC.GetDgvLineFields1(rowIdx, false, null);
       
       // 27.04.2018
       //if(columnKind == RISK_ColumnKindEnum.MalopCij && theDUC is MedjuSkladVMIuDUC                                 ) rtrans_rec.T_skladCD = theDUC.Fld_SkladCD2;
         if(columnKind == RISK_ColumnKindEnum.MalopCij && (theDUC is MedjuSkladVMIuDUC || theDUC is MedjuSkladVMI2DUC)) rtrans_rec.T_skladCD = theDUC.Fld_SkladCD2;

         artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec);

         if(artstat_rec != null)
         {
            switch(columnKind)
            {
               case RISK_ColumnKindEnum.PrNabCij     : colIdx = theDUC.DgvCI.iT_cij    ; newDecimalValue = artstat_rec.PrNabCij                               ; break;
               case RISK_ColumnKindEnum.PrNabCij_wMPC: colIdx = theDUC.DgvCI.iT_IRA_MPC; newDecimalValue = 
                                                                                         ZXC.VvGet_125_on_100(artstat_rec.PrNabCij, rtrans_rec.T_pdvSt)/*.Ron2()*//*.Ron(4)*/; break;

             //case RISK_ColumnKindEnum.MalopCij: colIdx = theDUC.DgvCI.iT_cij; newDecimalValue = artstat_rec.LastUlazMPC; break;// 30.04.2015
               case RISK_ColumnKindEnum.MalopCij:
                //if(theDUC is TransformDUC      || theDUC is MedjuSkladVMIuDUC || theDUC is MedjuSkladMVIDUC || theDUC is MedjuSkladMVI2DUC) colIdx = theDUC.DgvCI.iT_cij_MSK;
                  if(theDUC is TransformDUC      || theDUC is MedjuSkladVMIuDUC || theDUC is MedjuSkladMVIDUC || 
                     theDUC is MedjuSkladMVI2DUC || theDUC is MedjuSkladVMI2DUC || theDUC is KalkulacijaMpDUC)                                colIdx = theDUC.DgvCI.iT_cij_MSK;
                  else                                                                                                                        colIdx = theDUC.DgvCI.iT_cij    ; 
                  newDecimalValue = artstat_rec.LastUlazMPC; break;
               case RISK_ColumnKindEnum.Kol     : colIdx = theDUC.DgvCI.iT_kol; newDecimalValue = artstat_rec.StanjeKol  ; break;

               case RISK_ColumnKindEnum.PdvSt   : colIdx = theDUC.DgvCI.iT_pdvSt; newDecimalValue = Faktur.CommonPdvStForThisDate(theDUC.faktur_rec.DokDate); break;

               case RISK_ColumnKindEnum.PrNabCij_And_Kol:

                  colIdx = theDUC.DgvCI.iT_cij; newDecimalValue = artstat_rec.PrNabCij;

                  theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, rowIdx, artstat_rec.StanjeKol);

                  break;

//###################################################################################################################### 
             //case RISK_ColumnKindEnum.SVD_INVkol_korekcija:
             //
             //   colIdx = theDUC.DgvCI.iT_kol;
             //
             //   decimal zatecenaKolicina = theDUC.TheG.GetDecimalCell(colIdx, rowIdx, false);
             //
             //   newDecimalValue = zatecenaKolicina - Get_KOL_SaldoRazdoblja(TheDbConnection, rtrans_rec.T_artiklCD, rtrans_rec.T_skladCD, SVD_INVkol_korekcija_Date_PocetakRazdoblja, SVD_INVkol_korekcija_Date_KrajRazdoblja);
             //   break;
//###################################################################################################################### 

               default: colIdx = 0; newDecimalValue = 0M; break;
            }
         }
         else
         {
            newDecimalValue = 0M;
            colIdx          = 0;
         }

//###################################################################################################################### 
         if(columnKind == RISK_ColumnKindEnum.SVD_INVkol_korekcija)
         {
            colIdx = theDUC.DgvCI.iT_kol;

            decimal zatecenaKolicina = theDUC.TheG.GetDecimalCell(colIdx, rowIdx, false);

            newDecimalValue = zatecenaKolicina - Get_KOL_SaldoRazdoblja(TheDbConnection, rtrans_rec.T_artiklCD, rtrans_rec.T_skladCD, SVD_INVkol_korekcija_Date_PocetakRazdoblja, SVD_INVkol_korekcija_Date_KrajRazdoblja);
            //break;
         }
//###################################################################################################################### 

         theDUC.TheG.PutCell(colIdx, rowIdx, newDecimalValue);
      }

      // dellme latter: 
      if(columnKind == RISK_ColumnKindEnum.SVD_INVkol_korekcija)
      {
         //theDUC.Fld_DokDate = new DateTime(2022, 06, 30);
      }

      // 07.07.2022: !!! 
      // ugasio i zakljucio da je brze bez ovoga pa rucno izazvati DirtyFlag da bi sredio sume 
      // jer ovo traje broj redaka dokumenta na kvadrat (svaki red idwe za sve ostale redove !? 
      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

      // 14.06.2023: ipak je komfornije da ovo radi ali cemo staviti samo na 'kratkim' dokumentima do 32 reda 
      if(theDUC.TheG.RowCount <= 32)
      {
         theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
      }

      Cursor.Current = Cursors.Default;
   }

   private decimal Get_KOL_SaldoRazdoblja(XSqlConnection conn, string artiklCD, string skladCD, DateTime Date_PocetakRazdoblja, DateTime Date_KrajRazdoblja)
   {
      Artikl  endArtikl_rec   = TheVvUC.Get_Artikl_FromVvUcSifrar(artiklCD);
      Artikl  startArtikl_rec = endArtikl_rec.MakeDeepCopy();

      endArtikl_rec  .TheAsEx = ArtiklDao.GetArtiklStatus(conn, artiklCD, skladCD, Date_KrajRazdoblja);
      startArtikl_rec.TheAsEx = ArtiklDao.GetArtiklStatus(conn, artiklCD, skladCD, Date_PocetakRazdoblja);

      Artikl artiklRazdoblje      = endArtikl_rec.MakeDeepCopy();
      artiklRazdoblje.TheAsEx     = new ArtStat();
    //artiklRazdoblje.TheAsEx     = endArtikl_rec.TheAsEx.MakeDeepCopy();
      artiklRazdoblje.AS_ArtiklTS = endArtikl_rec.AS_ArtiklTS; // ova dva su potrebna jer se po njima GRUPIRA, a ako je podatak po kojem se grupira PTAZAN onda row ne izlazi uprkos tome sto postoji u listi! 
      artiklRazdoblje.AS_SkladCD  = endArtikl_rec.AS_SkladCD;

      /* U Razdoblju - ULAZ       */ artiklRazdoblje.AS_UkUlazKol         = endArtikl_rec  .AS_UkUlazKol  - startArtikl_rec.AS_UkUlazKol ;
      /* U Razdoblju - IZLAZ      */ artiklRazdoblje.AS_UkIzlazKol        = endArtikl_rec  .AS_UkIzlazKol - startArtikl_rec.AS_UkIzlazKol;

      return artiklRazdoblje.AS_StanjeKol;
   }

   internal void SintSameArtiklRows(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      string  outerArtiklCD, innerArtiklCD;
      decimal outerKol     , innerKol     ;

      Cursor.Current = Cursors.WaitCursor;

      for(int outerRowIdx = 0; outerRowIdx < theDUC.TheG.RowCount - 1; ++outerRowIdx)
      {
         outerArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, outerRowIdx, false);

         for(int innerRowIdx = outerRowIdx+1; innerRowIdx < theDUC.TheG.RowCount - 1; ++innerRowIdx)
         {
            innerArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, innerRowIdx, false);

            // !!! 
            // PAZI !!! Observacija od 21.05.2020: Ovdje ne provjeravas jednakost ostalih parametara rtrans calc-a a 
            // koji moraju biti isti da bi sazmi imalo smisla (mora biti ista i cijena, pdvSt, rbt, ...) 
            // !!! 
            if(outerArtiklCD == innerArtiklCD)
            {
               outerKol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, outerRowIdx, false);
               innerKol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, innerRowIdx, false);

               theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, outerRowIdx, outerKol + innerKol);

               theDUC.TheG.Rows.RemoveAt(innerRowIdx--);

               theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(outerRowIdx);
            }
         }
      }

      Cursor.Current = Cursors.Default;

      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   private void SintSameArtiklRows_TH_PSM(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      string  outerArtiklCD, innerArtiklCD;
      decimal outerKol     , innerKol     , newKOL;
      decimal outerNBV     , innerNBV     , newNBC;
      decimal outerMPV     , innerMPV     , newMPC;

      Cursor.Current = Cursors.WaitCursor;

      for(int outerRowIdx = 0; outerRowIdx < theDUC.TheG.RowCount - 1; ++outerRowIdx)
      {
         outerArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, outerRowIdx, false);

         for(int innerRowIdx = outerRowIdx+1; innerRowIdx < theDUC.TheG.RowCount - 1; ++innerRowIdx)
         {
            innerArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, innerRowIdx, false);

            if(outerArtiklCD == innerArtiklCD)
            {
               // kol 
               outerKol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, outerRowIdx, false);
               innerKol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, innerRowIdx, false);

               newKOL = outerKol + innerKol;

               theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, outerRowIdx, newKOL);

               // nbv 
               outerNBV = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kcr, outerRowIdx, false);
               innerNBV = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kcr, innerRowIdx, false);

               newNBC = ZXC.DivSafe(outerNBV + innerNBV, newKOL);

               theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij, outerRowIdx, newNBC);

               // mpv 
               outerMPV = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_MSK, outerRowIdx, false);
               innerMPV = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_MSK, innerRowIdx, false);

               newMPC = ZXC.DivSafe(outerMPV + innerMPV, newKOL);

               theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij_MSK, outerRowIdx, newMPC);

               // do it 

               theDUC.TheG.Rows.RemoveAt(innerRowIdx--);

               theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(outerRowIdx);
            }
         }
      }

      Cursor.Current = Cursors.Default;

      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   private void RISK_RBT(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_rbt1St, rIdx, theDUC.Fld_NacPlacRbt);
      }

      theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   #endregion Change All Values for some Column

   /*private*/ public void RISK_UndoFak2Nal(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

    //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA
    //if(!ZXC.KSD.Dsc_IsOnlyIOSknjizenje && ((FakturDUC)(TheVvDocumentRecordUC)).Fld_TT == Faktur.TT_IRM) return; // oni koji nemaju samoIosKnjizenje ne mogu knjiziti pojedinacne IRM-ove
      if(!ZXC.KSD.Dsc_ForceIRMkaoIRA     && ((FakturDUC)(TheVvDocumentRecordUC)).Fld_TT == Faktur.TT_IRM) return; // oni koji nemaju forceIRMkaoIRa ne mogu knjiziti pojedinacne IRM-ove

      Faktur faktur_rec = (Faktur)(TheVvDataRecord);
      string    tipBr      = faktur_rec.TipBr;
      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;


      if(VvDaoBase.IsDocumentFromLockedPeriod(faktur_rec.DokDate.Date, false)) return;

      List<Ftrans> thisFakturFtrList = new List<Ftrans>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tipBr], "elTipBr"  , tipBr,                                      " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FtransSchemaRows[ZXC.FtrCI.t_tt   ], "elNalogTT", theDUC.GetNalogTT_ForRiskTT(faktur_rec.TT), " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Ftrans>(TheDbConnection, thisFakturFtrList, filterMembers, "ftr", "");

      if(thisFakturFtrList.Count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Za ovaj dokument nema knjiženja u financijskom knjigovodstvu!");
         return;
      }

      int distinctNalogCount = thisFakturFtrList.Select(ftr => ftr.T_dokNum).Distinct().Count();

      if(distinctNalogCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Za ovaj dokument ima više od jednog knjiženja u financijskom knjigovodstvu!");
         return;
      }

      string message = "Da li zaista želite pobrisati ove stavke naloga za knjiženje!?:\n\n";
      foreach(Ftrans ftrans in thisFakturFtrList)
      {
         message += String.Format("Nalog: {0} ({1}) konto: {2} dug: {3} pot: {4}\n", ftrans.T_dokNum, ftrans.T_TT, ftrans.T_konto, ftrans.T_dug, ftrans.T_pot);
      }
      message += "\n\nUPOZORENJE! Ova će akcija možda izazvati neravnotežu spomenutih naloga.\n\nVaša je odgovornost da nalog, po potrebi, ručno korigirate.";

      DialogResult result = MessageBox.Show(message, "Potvrdite BRISANJE stavaka NZK?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      // here we go! 

      int debugCount = 0;

      VvDataRecord arhivedNalog_rec;
      Nalog        origNalog_rec = new Nalog();

      origNalog_rec.VvDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, origNalog_rec, thisFakturFtrList.First().T_dokNum, ZXC.NalogSchemaRows[ZXC.NalCI.dokNum], false);
      origNalog_rec.VvDao.LoadTranses(TheDbConnection, origNalog_rec, false);

      arhivedNalog_rec = origNalog_rec.CreateArhivedDataRecord(TheDbConnection, "UNDO_F2N");

      foreach(Ftrans ftrans_rec in thisFakturFtrList)
      {
         if(ftrans_rec.VvDao.DELREC(TheDbConnection, ftrans_rec, false)) debugCount++;
      }

      if(debugCount.NotZero())
      {
         BeginEdit(origNalog_rec);
         string newNapomena = "UNDO_F2N! " + origNalog_rec.Napomena;
         origNalog_rec.Napomena = ZXC.LenLimitedStr(newNapomena, ZXC.NalogDao.GetSchemaColumnSize(ZXC.NalCI.napomena));
         bool OK = origNalog_rec.VvDao.RWTREC(TheDbConnection, origNalog_rec);
         EndEdit(origNalog_rec);

         arhivedNalog_rec.VvDao.ADDREC(TheDbConnection, arhivedNalog_rec, true, true, false, false);
      }

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Pobrisao {0} stavaka NZK.", debugCount);
   }

   /*private*/ public void RISK_LoadSingleFak2Nal(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

    //31.03.2022. razdvajamo IsOnlyIOSknjizenje i ForceIRMkaoIRA
    //if(!ZXC.KSD.Dsc_IsOnlyIOSknjizenje && ((FakturDUC)(TheVvDocumentRecordUC)).Fld_TT == Faktur.TT_IRM) return; // oni koji nemaju samoIosKnjizenje ne mogu knjiziti pojedinacne IRM-ove
      if(!ZXC.KSD.Dsc_ForceIRMkaoIRA     && ((FakturDUC)(TheVvDocumentRecordUC)).Fld_TT == Faktur.TT_IRM) return; // oni koji nemaju ForceIRMkaoIRA ne mogu knjiziti pojedinacne IRM-ove

      if(((FakturDUC)(TheVvDocumentRecordUC)).IsOkToInitiateThisAction(ZXC.WriteMode.Edit) == false) return;

      DialogResult result = MessageBox.Show("Da li zaista želite proknjižiti ovaj dokument u financijsko knjigovodstvo?", "Potvrdite KNJIŽENJE na NZK?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      Faktur faktur_rec = (Faktur)(TheVvDataRecord);

      if(VvDaoBase.IsDocumentFromLockedPeriod(faktur_rec.DokDate.Date, false)) return;

      // za eventualno faleci TheAsEx (FakturDao.LoadTranses(): IsIRArucableTT & IsShadowTT uzmu TheAsEx a ostali ne)
      //VvDaoBase.LoadGenericVvDataRecordList<Rtrans>(TheDbConnection, faktur_rec.Transes, FakturDao.GetFilterMembers_LoadTranses(faktur_rec.RecID), Rtrans.recordName, "L.t_serial ", true);
      faktur_rec.Transes.Clear();
      RtransDao.GetRtransWithArtstatList(TheDbConnection, faktur_rec.Transes, "", GetFilterMembers_LoadTranses(faktur_rec.RecID), "");

      #region fakturList & Faktur2NalogRulesAndData

      Faktur2NalogRulesAndData theRules = new Faktur2NalogRulesAndData();

    //theRules.KtoShemaDsc = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      theRules.KtoShemaDsc = ZXC.KSD;

      theRules.DateDo = faktur_rec.DokDate.Date;
      theRules.DateOd = faktur_rec.DokDate.Date;
      theRules.Fak2nalSet = ZXC.Faktur2NalogSetEnum.OneExactTT;
      theRules.IsAutomatic = false;

      theRules.ThisTT_Only = faktur_rec.TT;
      theRules.PeriodDefinedVia_DokDate = true;

      List<Faktur> fakturList = new List<Faktur>(1);

      //fakturList.Add((Faktur)faktur_rec.CreateNewRecordAndCloneItComplete());
      fakturList.Add(faktur_rec);

      #endregion Faktur2NalogRulesAndData

      FakturDao.ExecuteFaktur2Nalog(TheDbConnection, fakturList, theRules, null);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nBroj obrađenih RISK dokumenata/grupa: [{0}]\n\nBroj novih naloga: [{1}].", 1/*fakturList.Count*/, 1/*dlg.TheRules.NalogCount*/);
   }

   private List<VvSqlFilterMember> GetFilterMembers_LoadTranses(uint fakturRecID)
   {
      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      drSchema = ZXC.RtransDao.TheSchemaTable.Rows[ZXC.RtrCI.t_parentID];

      filterMembers.Add(new VvSqlFilterMember(drSchema, "elFakturRecID", fakturRecID, " = "));

      return filterMembers;
   }

   private void TakeRowsFromManyDocuments_byTtDokDateKupdobCD(object sender, EventArgs e) // Many UFA-DEV na KLK-DEV 
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      #region variablez

      List<Faktur> fakturList = new List<Faktur>();
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      #endregion variablez

      if(theDUC.Fld_KupdobCd.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte najprije Partnera."); return; }

      #region Faktur FilterMemberz

      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakCI.tt];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elTT",       Faktur.TT_UFA,       " = ")); // TODO: da se tt moze birati preko nekog dialoga or samthing 
      drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakCI.dokDate];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokDate",  theDUC.Fld_DokDate,  " = "));
      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FexCI.kupdobCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdobCD", theDUC.Fld_KupdobCd, " = "));

      int rowIdx, idxCorrector;

      #endregion Faktur FilterMemberz

      string selectWhat = "L.*, R.*";
      string orderBy = "dokDate , ttSort, ttNum";

      string napom4link     = "";
      string origBrDok4link = "";

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, fakturList, filterMembers, "", orderBy, true, selectWhat, "");

      if(theDUC.IsShowingConvertedMoney) RISK_ToggleKnDeviza(sender, e);

      foreach(Faktur faktur_rec in fakturList)
      {
         faktur_rec.VvDao.LoadTranses(TheDbConnection, faktur_rec, false);

         #region Link on UFA-DEV

         // RWTREC UFA-DEV - start 
         ZXC.FakturRec = (Faktur)(faktur_rec.CreateNewRecordAndCloneItComplete());
         ZXC.RISK_CopyToOtherDUC_inProgress = true;

         if(ZXC.IsSkyEnvironment == false) RwtrecLinkFeedback_FAKTUR();
         // RWTREC UFA-DEV - end 

         // KLK-DEV - start 

         switch(theDUC.faktur_rec.FirstEmptyVezaLine)
         {
            case 1: theDUC.Fld_V1_tt    = faktur_rec.TT;
                    theDUC.Fld_V1_ttNum = faktur_rec.TtNum;
                    break;
            case 2: theDUC.Fld_V2_tt    = faktur_rec.TT;
                    theDUC.Fld_V2_ttNum = faktur_rec.TtNum;
                    break;
            case 3: theDUC.Fld_V3_tt    = faktur_rec.TT;
                    theDUC.Fld_V3_ttNum = faktur_rec.TtNum;
                    break;
            case 4: theDUC.Fld_V4_tt    = faktur_rec.TT;
                    theDUC.Fld_V4_ttNum = faktur_rec.TtNum;
                    break;

               // 29.10.2021: ubili warning default: ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'primaocu' nema više mjesta za upis nove veze."); break;
         }

         // KLK-DEV - end 

         #endregion Link on UFA-DEV

         #region PutDgvFields

         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

         theDUC.Fld_MalopCalcKind = ZXC.MalopCalcKind.By_MPC;

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            #region Fill missing rtrans values

            rtrans_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec.T_artiklCD, theDUC.Fld_SkladCD, theDUC.Fld_SkladDate);

            rtrans_rec.T_TT        = theDUC.Fld_TT;
            rtrans_rec.T_pdvSt     = Faktur.CommonPdvStForThisDate(faktur_rec.DokDate);
            rtrans_rec.T_mCalcKind = ZXC.MalopCalcKind.By_MPC;

            if(rtrans_rec.TheAsEx != null)
            {
               rtrans_rec.T_wanted = rtrans_rec.A_MalopCij;
            }

            #endregion Fill missing rtrans values

            theDUC.TheG.Rows.Add();

            rowIdx = theDUC.TheG.RowCount - idxCorrector;

            theDUC.PutDgvLineFields        (rtrans_rec, rowIdx, true);
            rtrans_rec.CalcTransResults    (null/*faktur_rec*/);
            theDUC.PutDgvLineResultsFields1(rowIdx, rtrans_rec, false);
            theDUC.GetDgvLineFields        (rowIdx, false, null); // da napuni Document's business.Transes 
         }

         #endregion PutDgvFields

         napom4link     += faktur_rec.TT_And_TtNum + " / ";
         origBrDok4link += faktur_rec.VezniDok     + " / ";
      }

      theDUC.Fld_Napomena = napom4link    .TrimEnd(' ').TrimEnd('/').TrimEnd(' ');
      theDUC.Fld_VezniDok = origBrDok4link.TrimEnd(' ').TrimEnd('/').TrimEnd(' ');

      VvDocumentRecordUC.RenumerateLineNumbers(theDUC.TheG, 0);
      theDUC            .UpdateLineCount(theDUC.TheG);

      theDUC.PutDgvTransSumFields();
      theDUC.PutTransSumToDocumentSumFields();

      RISK_ToggleKnDeviza(sender, e);
   }

   /*private*/ public void TakeRowsFromManyDocumentsPRIorIZD_byTtDokDateKupdobCD_toUFAorIFA(object sender, EventArgs e)
   {
      bool isIzlaz;

      if((sender as ToolStripButton).Text == Faktur.TT_PRI) isIzlaz = false;
      else                                                  isIzlaz = true ;

      TakeRowsFromManyDocumentsPRIorIZD_byTtDokDateKupdobCD_toUFAorIFA_JOB(isIzlaz, sender, e);
   }

   private void TakeRowsFromManyDocumentsPRIorIZD_byTtDokDateKupdobCD_toUFAorIFA_JOB(bool isIzlaz, object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      #region variablez

      List<Faktur> fakturList = new List<Faktur>();
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      bool isIZD2IRA = theDUC is IRADUC && isIzlaz;
      bool isIZM2IRM = theDUC is IRMDUC && isIzlaz;

      bool isDevizna = false;

      decimal kunskaCijenaNaDanOvogDokumenta = 0M;

      string priORizd_name = isIzlaz ? "izdatnice" : "primke";

    //string priORizd_TT   = isIzlaz ?                              Faktur.TT_IZD  :  Faktur.TT_PRI ;
      string priORizd_TT   = isIzlaz ? (isIZM2IRM ? Faktur.TT_IZM : Faktur.TT_IZD) : (Faktur.TT_PRI);

      #endregion variablez

      if(theDUC.Fld_KupdobCd.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, zadajte najprije Partnera."); return; }

      if(isIZD2IRA && theDUC.Fld_SkladCD != "VPSK")
      {
         DialogResult result = MessageBox.Show("Pokušavate dodati račun sa skladišta koje nije 'VPSK'. \n\nDa li želite nastaviti?", " Skladište nije VPSK?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
         if(result != DialogResult.Yes) return;
      }

      if(isIZM2IRM && theDUC.Fld_SkladCD != "MPSK")
      {
         DialogResult result = MessageBox.Show("Pokušavate dodati račun sa skladišta koje nije 'MPSK'. \n\nDa li želite nastaviti?", " Skladište nije MPSK?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
         if(result != DialogResult.Yes) return;
      }

      VvTakeRowsFromManyDocumentsDlg dlg = new VvTakeRowsFromManyDocumentsDlg("Učitaj stavke " + priORizd_name, theDUC.Fld_KupdobCd, theDUC.Fld_KupdobTk, theDUC.Fld_KupdobName, new DateTime(theDUC.Fld_DokDate.Year, theDUC.Fld_DokDate.Month, 1), theDUC.Fld_DokDate);

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      // ------------------------------------------------- 

      #region Faktur FilterMemberz

      DataRow drSchema;
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

      drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakCI.tt];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elTT",       priORizd_TT,         " = " ));
      drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakCI.dokDate];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokDateOD", dlg.Fld_DatumOd,    " >= "));
      drSchema = ZXC.FakturDao.TheSchemaTable.Rows[ZXC.FakCI.dokDate];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elDokDateDO", dlg.Fld_DatumDo,    " <= "));
      drSchema = ZXC.FaktExDao.TheSchemaTable.Rows[ZXC.FexCI.kupdobCD];
      filterMembers.Add(new VvSqlFilterMember(drSchema, "elKupdobCD", theDUC.Fld_KupdobCd, " = " ));

      int rowIdx, idxCorrector;

      #endregion Faktur FilterMemberz

      string selectWhat = "L.*, R.*";
      string orderBy = "dokDate , ttSort, ttNum";

    //string napom4link     = ""; if(isIZD2IRA) napom4link = "IZD:";
      string napom4link     = ""; if(isIZD2IRA) napom4link = "IZD:"; if(isIZM2IRM) napom4link = "IZM:";
      string origBrDok4link = "";

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, fakturList, filterMembers, "", orderBy, true, selectWhat, "");

      if(fakturList.Count.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Warning, "Za zadano razdoblje, nema " + priORizd_name + " partnera."); return; }

      if(theDUC.IsShowingConvertedMoney) { isDevizna = true; RISK_ToggleKnDeviza(sender, e); }

      foreach(Faktur primkaOrizdatnicaFaktur_rec in fakturList)
      {
         primkaOrizdatnicaFaktur_rec.VvDao.LoadTranses(TheDbConnection, primkaOrizdatnicaFaktur_rec, false);

         #region Link on UFA-DEV

         // RWTREC UFA-DEV - start 
         ZXC.FakturRec = (Faktur)(primkaOrizdatnicaFaktur_rec.CreateNewRecordAndCloneItComplete());
         ZXC.RISK_CopyToOtherDUC_inProgress = true;

         if(ZXC.IsSkyEnvironment == false) RwtrecLinkFeedback_FAKTUR();
         // RWTREC UFA-DEV - end 

         // KLK-DEV - start 

         switch(theDUC.faktur_rec.FirstEmptyVezaLine)
         {
            case 1: theDUC.Fld_V1_tt    = primkaOrizdatnicaFaktur_rec.TT;
                    theDUC.Fld_V1_ttNum = primkaOrizdatnicaFaktur_rec.TtNum;
                    break;
            case 2: theDUC.Fld_V2_tt    = primkaOrizdatnicaFaktur_rec.TT;
                    theDUC.Fld_V2_ttNum = primkaOrizdatnicaFaktur_rec.TtNum;
                    break;
            case 3: theDUC.Fld_V3_tt    = primkaOrizdatnicaFaktur_rec.TT;
                    theDUC.Fld_V3_ttNum = primkaOrizdatnicaFaktur_rec.TtNum;
                    break;
            case 4: theDUC.Fld_V4_tt    = primkaOrizdatnicaFaktur_rec.TT;
                    theDUC.Fld_V4_ttNum = primkaOrizdatnicaFaktur_rec.TtNum;
                    break;

               // 29.10.2021: ubili warning default: if(isIZD2IRA == false) ZXC.aim_emsg(MessageBoxIcon.Warning, "Na Faktur 'primaocu' nema više mjesta za upis nove veze."); break;
         }

         // KLK-DEV - end 

         #endregion Link on UFA-DEV

         #region PutDgvFields

         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

         theDUC.Fld_MalopCalcKind = ZXC.MalopCalcKind.By_MPC;

         foreach(Rtrans rtrans_rec in primkaOrizdatnicaFaktur_rec.Transes)
         {
            #region Fill missing rtrans values

          //rtrans_rec.TheAsEx = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec.T_artiklCD, theDUC.Fld_SkladCD, theDUC.Fld_SkladDate);
          //
          //rtrans_rec.T_TT        = theDUC.Fld_TT;
          //rtrans_rec.T_pdvSt     = Faktur.CommonPdvStForThisDate(faktur_rec.DokDate);
          //rtrans_rec.T_mCalcKind = ZXC.MalopCalcKind.By_MPC;
          //
          //if(rtrans_rec.TheAsEx != null)
          //{
          //   rtrans_rec.T_wanted = rtrans_rec.A_MalopCij;
          //}

            rtrans_rec.T_pdvSt = Faktur.CommonPdvStForThisDate(primkaOrizdatnicaFaktur_rec.DokDate);

            #endregion Fill missing rtrans values

            theDUC.TheG.Rows.Add();

            rowIdx = theDUC.TheG.RowCount - idxCorrector;

            if(isDevizna)
            {
             //kunskaCijenaNaDanOvogDokumenta = ZXC.RefreshedKunskaCijena(rtrans_rec.T_cij, primkaFaktur_rec.DevTecaj, theDUC.faktur_rec.DevTecaj);
             //rtrans_rec.T_cij = kunskaCijenaNaDanOvogDokumenta;

               rtrans_rec.RefreshKunskaCijena(primkaOrizdatnicaFaktur_rec.DevTecaj, theDUC.faktur_rec.DevTecaj);
            }

            theDUC.PutDgvLineFields        (rtrans_rec, rowIdx, true);
            rtrans_rec.CalcTransResults    (null/*faktur_rec*/);
            theDUC.PutDgvLineResultsFields1(rowIdx, rtrans_rec, false);
            theDUC.GetDgvLineFields        (rowIdx, false, null); // da napuni Document's business.Transes 
         }

         #endregion PutDgvFields

         if(isIZD2IRA) napom4link += primkaOrizdatnicaFaktur_rec.TtNum + "/";
         else          napom4link += primkaOrizdatnicaFaktur_rec.TT_And_TtNum + " / ";
         
         //origBrDok4link += primkaFaktur_rec.VezniDok     + " / ";

      } // foreach(Faktur primkaFaktur_rec in fakturList)

      #region Napomena or Opis 

      int maxNapomena = ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena);
      int maxOpis     = ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.opis    );

      napom4link = napom4link.TrimEnd(' ').TrimEnd('/').TrimEnd(' ');

      if(napom4link.Length <= maxNapomena) theDUC.Fld_Napomena = ZXC.LenLimitedStr(napom4link, maxNapomena);
      else                                 theDUC.Fld_Opis     = ZXC.LenLimitedStr(napom4link, maxOpis    );

      #endregion Napomena or Opis 

      VvDocumentRecordUC.RenumerateLineNumbers(theDUC.TheG, 0);
      theDUC            .UpdateLineCount(theDUC.TheG);

      theDUC.PutDgvTransSumFields();
      theDUC.PutTransSumToDocumentSumFields();

      RISK_ToggleKnDeviza(sender, e);

      // ------------------------------------------------- 

      dlg.Dispose();

      #region After IRA from IZD: COPY IZD 2 NRD & DELETE IZD Rtranses

    //if(isIZD2IRA             ) 
      if(isIZD2IRA || isIZM2IRM)
      {
         bool   OK              ;
         int    debugCount = 0  ;
         Faktur newNRDfaktur_rec;

         foreach(Faktur izdatnicaFaktur_rec in fakturList)
         {
            #region neuspjeh 
            //ZXC.FakturRec = (Faktur)(izdatnicaFaktur_rec.CreateNewRecordAndCloneItComplete());
            //ZXC.RISK_CopyToOtherDUC_inProgress = true;
            //BeginEdit(ZXC.FakturRec);
            //#region Set changed values
            //ZXC.FakturRec.TT = Faktur.TT_NRD;
            //#endregion Set changed values
            //OK = ZXC.FakturRec.VvDao.RWTREC(TheDbConnection, ZXC.FakturRec, false, false, false);
            //EndEdit(ZXC.FakturRec);
            #endregion neuspjeh 

            #region ADD NEW NRD Faktur

            newNRDfaktur_rec = (Faktur)(izdatnicaFaktur_rec.CreateNewRecordAndCloneItComplete());

            newNRDfaktur_rec.TT       = Faktur.TT_NRD;
            newNRDfaktur_rec.TtSort   = ZXC.TtInfo(newNRDfaktur_rec.TT).TtSort;
            newNRDfaktur_rec.V1_tt    = izdatnicaFaktur_rec.TT   ;
            newNRDfaktur_rec.V1_ttNum = izdatnicaFaktur_rec.TtNum;

            foreach(Rtrans NRDrtrans_rec in newNRDfaktur_rec.Transes)
            {
               NRDrtrans_rec.T_TT     = newNRDfaktur_rec.TT    ;
               NRDrtrans_rec.T_ttSort = newNRDfaktur_rec.TtSort;

               if(isIZM2IRM)
               {
                  NRDrtrans_rec.T_pdvSt = 0.00M; // jer NRD nema pdv-a, u t_cij ce imati informativno MPC sa IZM-a 

                  NRDrtrans_rec.CalcTransResults(null);
               }
            }

            newNRDfaktur_rec.TakeTransesSumToDokumentSum(true);

            OK = newNRDfaktur_rec.VvDao.ADDREC(TheDbConnection, newNRDfaktur_rec); // ADD NEW NRD Faktur 

            if(OK) ++debugCount;

            #endregion ADD NEW NRD Faktur

            #region DELETE IZD Transes

            foreach(Rtrans IZDrtrans_rec in izdatnicaFaktur_rec.Transes)
            {
               IZDrtrans_rec.T_pdvSt = 0.00M; // vracamo kako je bilo dok je bio orig IZC (za potrebe IRA-e smo gore prije PutDgvLineField-a pdvSt stavili na 25) 

               IZDrtrans_rec.VvDao.DELREC(TheDbConnection, IZDrtrans_rec, true);
            }

            #endregion DELETE IZD Transes

         }

         ZXC.FakturRec = null; ZXC.RISK_CopyToOtherDUC_inProgress = false;

         ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Obradio {0} IZDatnica, kreirao NRDove a izdatnicama pobrisao stavke.", debugCount);

         SintSameArtiklRows(null, EventArgs.Empty);

         if(isIZM2IRM == false)
         {
            SaveRecord_OnClick(null, EventArgs.Empty);
         }

      } // if(isIZD2IRA OR IZM2IRM) 

      #endregion After IRA from IZD: COPY IZD 2 NRD & DELETE IZD Rtranses

   }

   /*private*/ public void UFD_NiceKn(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      if(theDUC is KalkulacijaMpDUC_Dev == false &&
         theDUC is URMDUC_Dev           == false &&
         theDUC is PonMalDUC            == false) return;

      if(theDUC.TheG.CurrentRow == null) return;

      int rowIdx = theDUC.TheG.CurrentRow.Index;

      if(rowIdx.IsNegative()) return;

      Rtrans rtrans_rec;
      int colIdx;

reopenDialogLABEL: rtrans_rec = (Rtrans)theDUC.GetDgvLineFields1(rowIdx, false, null);

      #region GetExterniCjenik

      decimal theMSRP = 0.00M;

      Xtrans xtrans_rec = new Xtrans();

    //bool success = XtransDao.GetExterniCjenik(TheDbConnection, xtrans_rec, rtrans_rec.T_artiklCD,                     false);
      theMSRP      = XtransDao.GetExterniCjenik(TheDbConnection, xtrans_rec, rtrans_rec.T_artiklCD, theDUC.Fld_DokDate, false);

      //if(success) theMSRP = xtrans_rec.T_moneyA;
      //else        theMSRP = 0.00M;

      #endregion GetExterniCjenik
      
      NiceKnDlg dlg;

      if(theDUC is KalkulacijaMpDUC_Dev || theDUC is URMDUC_Dev)
      {
         dlg = new NiceKnDlg(rowIdx + 1, rtrans_rec.T_artiklCD, rtrans_rec.T_artiklName, theDUC.Fld_DevName, theDUC.DevTecaj, theDUC.Fld_DokDate, theMSRP);
         colIdx = theDUC.DgvCI.iT_cij_MSK;
      }
      else // theDUC is PonMalDUC 
      {
         decimal devTecaj = ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrans_rec.T_konto), DateTime.Now);

         dlg = new NiceKnDlg(rowIdx + 1, rtrans_rec.T_artiklCD, rtrans_rec.T_artiklName, xtrans_rec.T_konto, devTecaj, DateTime.Now, theMSRP);
         colIdx = theDUC.DgvCI.iT_cij/*_MSK*/;
      }

      DialogResult dlgResult =  dlg.ShowDialog();
      if(dlgResult == System.Windows.Forms.DialogResult.Cancel) return;

      if(dlgResult == System.Windows.Forms.DialogResult.Retry)
      {
         if(theDUC.IsShowingConvertedMoney) RISK_ToggleKnDeviza(sender, e);

         theDUC.TheG.PutCell(colIdx, rowIdx, dlg.Fld_ZadanoMPC);

         theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rowIdx);

         if(++rowIdx < theDUC.TheG.RowCount - 1) goto reopenDialogLABEL;
      }

      if(dlgResult == System.Windows.Forms.DialogResult.OK)
      {
         if(theDUC.IsShowingConvertedMoney) RISK_ToggleKnDeviza(sender, e);

         theDUC.TheG.PutCell(colIdx, rowIdx, dlg.Fld_ZadanoMPC);

         theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rowIdx);
      }
   }

   #region PPUK PROIZVODNJA

   /*private*/public void LoadPopratnica(object sender, EventArgs e)
   {
      #region variablez

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      int rowIdx, idxCorrector;

      Artikl artikl_rec;
      //Rtrans rtrans_rec;
      Rtrano rtrano_rec;

      List<Rtrans> rtransList;

      Kupdob sumarijaKupdob_rec;

      // 2025: 
      bool isSIROVINE4PIX = sender.ToString() == "SIROVINE4PIX";

      string theSerNo;

      Rtrano daPoprat_rtrano_rec = null;

      bool isLastRtrano_ForSerno_found;

      #endregion variablez

      #region FileDialog

      OpenFileDialog openFileDialog = new OpenFileDialog();

      openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName;

      openFileDialog.Filter = "Xml Popratnice (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
      //openFileDialog.Filter = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.RestoreDirectory = true;

      #endregion FileDialog

      if(openFileDialog.ShowDialog() == DialogResult.OK)
      {
         Cursor.Current = Cursors.WaitCursor;
         //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

         #region About openFileDialog.FileName, ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName

         string fullPathName = openFileDialog.FileName;
         System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathName);

         string fileName = dInfo.Name;
         string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));
         ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName = directoryName;

         openFileDialog.Dispose(); // !!! 

         #endregion About openFileDialog.FileName, ZXC.TheVvForm.VvPref.loadPopratPrefs.DirectoryName
         
         #region Load Xml File To bussiness data (by .NET)

         XmlSerializer ser = new XmlSerializer(typeof(Popratnice));
         FileStream fs = new FileStream(fullPathName, FileMode.Open);
         Popratnice xmlPoprat;
         try
         {
            xmlPoprat = ser.Deserialize(fs) as Popratnice;
         }
         catch(System.Exception ex)
         {
            MessageBox.Show(ex.ToString());
            fs.Close();
            return;
         }
         fs.Close();

         int distinct_PopratBrDok_Count = xmlPoprat.Items.Select(zag => zag.BrojDok).Distinct().Count();

         // 15.04.2013: zakljucismo da ipak na jedan dan u popratnicama moze biti vise od 1 dokumenta 
         //if(distinct_PopratBrDok_Count != 1)
         //{
         //   ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA! Xml datoteka sadrži {0} brojeva dokumenta!\n\n(A smije biti samo 1 broj dokumenta.)", distinct_PopratBrDok_Count);

         //   Cursor.Current = Cursors.Default;
         //   return;
         //}

         List<PopratniceZaglavljaStavke> stavkeList = new List<PopratniceZaglavljaStavke>();
         foreach(PopratniceZaglavlja zaglavlje in xmlPoprat.Items)
         {
            foreach(PopratniceZaglavljaStavke stavka in zaglavlje.Stavke)
            {
               stavkeList.Add(stavka);
            }
         }

         List<PopratniceZaglavljaRekapitulacija> rekapList = new List<PopratniceZaglavljaRekapitulacija>();
         foreach(PopratniceZaglavlja zaglavlje in xmlPoprat.Items)
         {
            // 28.02.2025: opkoljeno if-om
            if(zaglavlje.Rekapitulacija != null)
            {
               foreach(PopratniceZaglavljaRekapitulacija rekapitulacija in zaglavlje.Rekapitulacija)
               {
                  rekapList.Add(rekapitulacija);
               }
            }
         }

         #endregion Load Xml File To bussiness data (by .NET)

         #region Load Xml File To bussiness data (by Qukatz)

         /* byQ */
         //HrSumePopratnica thePopratnicaByQ = new HrSumePopratnica(fullPathName);

         /* byQ */
         //List<string> vvArtiklListDistinct = thePopratnicaByQ.StavkeList.OrderBy(st => st.VvArtiklName).Select(st => st.VvArtiklName).Distinct().ToList();

         #endregion Load Xml File To bussiness data (by Qukatz)

         #region Put Faktur Fields

         if(!isSIROVINE4PIX) // 2025: dodan ovaj if 
         {
            // Try via sumarijaID pred upisan u MatBr Kupdoba 
            sumarijaKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Matbr == xmlPoprat.Items[0].sumarijaID.ToString());

            if(sumarijaKupdob_rec == null)
            {
               // Try via NazivSumarije 
               sumarijaKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Naziv.ToUpper() == xmlPoprat.Items[0].NazivSumarije.ToUpper());
            }

            if(sumarijaKupdob_rec != null)
            {
               theDUC.Fld_KupdobCd = sumarijaKupdob_rec.KupdobCD;
               theDUC.Fld_KupdobTk = sumarijaKupdob_rec.Ticker;
               theDUC.Fld_KupdobName = sumarijaKupdob_rec.Naziv;
               theDUC.sifrarSorterType = VvSQL.SorterType.Code;
               ZXC.LoadPoprat_InProgress = true;
               theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobCd, EventArgs.Empty);
               theDUC.sifrarSorterType = VvSQL.SorterType.Name;
               //SendKeys.Send("{TAB}");
               //ZXC.OffixImport_InProgress = false;
            }
            else
            {
               sumarijaKupdob_rec = new Kupdob();
               sumarijaKupdob_rec.Ticker = "?!XY"; // Za pločicu 

               ZXC.aim_emsg(MessageBoxIcon.Error, "U šifrarniku partnera ne mogu naći šumarijuID {0}!\n\nZadajte Partnera ručno.", xmlPoprat.Items[0].sumarijaID);
            }

            theDUC.Fld_externLink1 = fullPathName;

            theDUC.Fld_DokDate = theDUC.Fld_PdvDate = theDUC.Fld_SkladDate = theDUC.Fld_DospDate = xmlPoprat.Items[0].datumpd;

         } // if(!isSIROVINE4PIX) 

         else // this is isSIROVINE4PIX 
         {
            if(stavkeList.Count.NotZero())
            {
               theDUC.Fld_DokDate = theDUC.Fld_PdvDate = theDUC.Fld_SkladDate = theDUC.Fld_DospDate = xmlPoprat.Items[0].datumpd;
            }
         }

         string napomena = "BrDok: ";
         foreach(PopratniceZaglavlja zaglavlje in xmlPoprat.Items)
         {
            napomena += zaglavlje.BrojDok + ", ";
         }
         napomena = napomena.TrimEnd(' ').TrimEnd(',');
         napomena += ". Stranica: ";
         foreach(PopratniceZaglavlja zaglavlje in xmlPoprat.Items)
         {
            napomena += zaglavlje.stranica + ", ";
         }
         napomena = napomena.TrimEnd(' ').TrimEnd(',');

         theDUC.Fld_Napomena = isSIROVINE4PIX ? fileName : ZXC.LenLimitedStr(napomena, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena));

         #endregion Put Faktur Fields

         #region PutDgvFields 1 - Rtrans Grid

         rtransList = rekapList
            .GroupBy(rekap => rekap.VvArtiklName)
            .Select(grp => new Rtrans()
            {
               T_artiklName = grp.Key,
               T_jedMj      = "m3",
               T_pdvSt      = Faktur.CommonPdvStForThisDate(xmlPoprat.Items[0].datumpd),
               T_kol        = grp.Sum(aGR => aGR.masa)
            })
            .OrderBy(suma => suma.T_artiklName, new VvCompareStringsOrdinal())
            .ToList();

         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

         foreach(Rtrans rtrans_rec in rtransList)
         {
            artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklName == rtrans_rec.T_artiklName);
            if(artikl_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Iz REKAPITULACIJE, Ne mogu naći\n\n[{0}]\n\nu datoteci artikala!", rtrans_rec.T_artiklName);
               artikl_rec = new Artikl() { ArtiklCD = "---- ??? ----", ArtiklName = rtrans_rec.T_artiklName };
            }
            rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD;

            #region Put, Calc, ... DgvFields

            theDUC.TheG.Rows.Add();

            rowIdx = theDUC.TheG.RowCount - idxCorrector;

            theDUC.PutDgvLineFields1    (rtrans_rec, rowIdx, true);
            rtrans_rec.CalcTransResults (null/*faktur_rec*/);
            theDUC.PutDgvLineResultsFields1(rowIdx, rtrans_rec, false);
            theDUC.GetDgvLineFields1    (rowIdx, false, null); // da napuni Document's business.Transes 

            #endregion Put, Calc, ... DgvFields
         }

         VvDocumentRecordUC.RenumerateLineNumbers(theDUC.TheG, 0);
         theDUC.UpdateLineCount(theDUC.TheG);

         theDUC.PutDgvTransSumFields();
         theDUC.PutTransSumToDocumentSumFields();

         #endregion PutDgvFields 1 - Rtrans Grid

         #region PutDgvFields 2 - Rtrano Grid

         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG2);

         decimal the_dimX;
         decimal the_dimZ;
         decimal the_kol ;

         foreach(PopratniceZaglavljaStavke stavka in stavkeList)
         {
            if(isSIROVINE4PIX == false) // OLD, classic 
            {
               artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklName == stavka.VvArtiklName);

               theSerNo = "";
               isLastRtrano_ForSerno_found = false;
            }

            // 2025:                     
            else // THIS is SIROVINE4PIX 
            {
               if(stavka.BrojPlocice.NotEmpty())
               { 
                  theSerNo = stavka.NazivJM + stavka.BrojPlocice;

                  daPoprat_rtrano_rec = new Rtrano();

                  isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(TheDbConnection, daPoprat_rtrano_rec, theSerNo, true);

                  #region On not found, try prev year

                  if(!isLastRtrano_ForSerno_found) // try prev year 
                  {
                      isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(ZXC.TheSecondDbConn_SameDB_prevYear, daPoprat_rtrano_rec, theSerNo, true);
                  }

                  #endregion On not found, try prev year
               }
               else // stavka.BrojPlocice je prazan
               {
                  theSerNo = stavka.NazivJM + "------";
                  isLastRtrano_ForSerno_found = false;
               }

               if(isLastRtrano_ForSerno_found == false) artikl_rec = null;
               else                                     artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == daPoprat_rtrano_rec.T_artiklCD);
            }

            if(artikl_rec == null)
            {
               if(isSIROVINE4PIX)
               {
                  // dilema; javljati ovo ili ne? 
                //ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći pločicu\n\n[{0}]\n\nu prethodno učitanim popratnicama!", theSerNo);
               }
               else // old, classic 
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Iz STAVAKA, Ne mogu naći\n\n[{0}]\n\nu datoteci artikala!", stavka.VvArtiklName);
               }

               artikl_rec = new Artikl() { ArtiklCD = "---- ??? ----", ArtiklName = stavka.VvArtiklName };
            }

            if(!isSIROVINE4PIX || isLastRtrano_ForSerno_found == false) // ako je ucitaj popratnice ILI ako je plocica nenadjena koristi iz SIROVINE4PIX.xml-a 
            { 
               the_dimX = stavka.Duljina  ;
               the_dimZ = stavka.PromjerBK;
               the_kol  = stavka.masam3   ;
            }
            else // plocica je nadjena koristi iz prethodno ucit popratnica.xml-a 
            { 
               the_dimX = daPoprat_rtrano_rec.T_dimX;
               the_dimZ = daPoprat_rtrano_rec.T_dimZ;
               the_kol  = daPoprat_rtrano_rec.T_kol ;
            }

            rtrano_rec = new Rtrano()
            {
               T_artiklCD   = artikl_rec.ArtiklCD,
             //T_artiklName = stavka.VvArtiklName,
               T_artiklName = isSIROVINE4PIX && isLastRtrano_ForSerno_found ? artikl_rec.ArtiklName : // 2025         
                                                                              stavka.VvArtiklName,    // old, classic 
               T_dimX       = the_dimX,
               T_dimZ       = the_dimZ, 
               T_kol        = the_kol ,

               // 2025: 
             //T_serno      = theDUC.Fld_KupdobTk.Substring(2, 2) + stavka.sumploc + stavka.BrojPlocice, 
               T_serno      = isSIROVINE4PIX /*&& isLastRtrano_ForSerno_found*/ ? theSerNo : 
                                                          /* Old, classic: */ theDUC.Fld_KupdobTk.SubstringSafe(2, 2) + stavka.sumploc + stavka.BrojPlocice, 
            };

            if(isSIROVINE4PIX && daPoprat_rtrano_rec != null) // provjera daPoprat_rtrano_rec vs ovaj new rtrano_rec 
            {
               if(stavka.Duljina   != rtrano_rec.T_dimX) ZXC.aim_emsg(MessageBoxIcon.Warning, "Za pločicu {0} razlikuje se Duljina   od prethodnog podatka sa popratnice.\n\rpoprat: {1}\n\rSirXml: {2}", daPoprat_rtrano_rec.T_serno, daPoprat_rtrano_rec.T_dimX, stavka.Duljina  );
               if(stavka.PromjerBK != rtrano_rec.T_dimZ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Za pločicu {0} razlikuje se PromjerBK od prethodnog podatka sa popratnice.\n\rpoprat: {1}\n\rSirXml: {2}", daPoprat_rtrano_rec.T_serno, daPoprat_rtrano_rec.T_dimZ, stavka.PromjerBK);
               if(stavka.masam3    != rtrano_rec.T_kol ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Za pločicu {0} razlikuje se masam3    od prethodnog podatka sa popratnice.\n\rpoprat: {1}\n\rSirXml: {2}", daPoprat_rtrano_rec.T_serno, daPoprat_rtrano_rec.T_kol , stavka.masam3   );
            }

            #region Put, Calc, ... DgvFields2

            theDUC.TheG2.Rows.Add();

            rowIdx = theDUC.TheG2.RowCount - idxCorrector;

            theDUC.PutDgvLineFields2(rtrano_rec, rowIdx, true);
            rtrano_rec.CalcTransResults(null/*faktur_rec*/);
            theDUC.PutDgvLineResultsFields2(rowIdx, rtrano_rec, false);
            theDUC.GetDgvLineFields2(rowIdx, false, null); // da napuni Document's business.Transes 

            #endregion Put, Calc, ... DgvFields2

         } // foreach(PopratniceZaglavljaStavke stavka in stavkeList) 

         #endregion PutDgvFields 2 - Rtrano Grid

         if(ZXC.theSecondDbConnection != null) ZXC.theSecondDbConnection.Close(); // nemoj tu pozivaty propertyy nego koristi varijablu (malo slovo)

         Cursor.Current = Cursors.Default;

         //theDUC.tbx_Napomena.Focus();
      }

   }

   #region CalcVolumen_SynthesizeG2toG1

   /*private*/ public void CalcVolumen_SynthesizeG2toG1(object sender, EventArgs e)
   {

      #region variablez

      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;
      int rowIdx, idxCorrector;

      Rtrans rtrans_rec;
    //Artikl artikl_rec;

      bool isPIX   = theDUC is PIZpDUC;
      bool isIRA_2 = theDUC is IRPDUC ;
      bool isBOR   = theDUC is BORDUC ;

      VvLookUpItem lui;

      #endregion variablez

      #region GetDgvLineField2_CalcVolumen_PutDgvLineField2_T_kol

      for(int rIdx = 0; isBOR == false && rIdx < theDUC.TheG2.RowCount - 1; ++rIdx)
      {
         GetDgvLineField2_CalcVolumen_PutDgvLineField2_T_kol(theDUC, rIdx);
      }

      #endregion GetDgvLineField2_CalcVolumen_PutDgvLineField2_T_kol

      #region SynthesizeG2toG1

      theDUC.GetFields(false); // Bussiness ok 

      #region Try restore missing rtrans_rec.R_grName data

      for(int i = 0; i < theDUC.faktur_rec.Transes.Count; ++i)
      {
         if(theDUC.faktur_rec.Transes[i].T_artiklCD.NotEmpty() &&
            theDUC.faktur_rec.Transes[i].R_grName  .IsEmpty ())

            try
            {
               lui = ZXC.luiListaRtranoGr.SingleOrDefault(l => l.Name == theDUC.faktur_rec.Transes[i+1].T_artiklName);
               if(lui != null) theDUC.faktur_rec.Transes[i].R_grName = lui.Cd;
            }
            catch(ArgumentOutOfRangeException) 
            { 
            }
      }
      
      #endregion Try restore missing rtrans_rec.R_grName data

    //List<Rtrans> savedRtranses = theDUC.faktur_rec.CloneTransesNonDeleted()        .ConvertAll(trans => trans as Rtrans); 
      List<Rtrans> savedRtranses = theDUC.faktur_rec.CloneTransesNonDeletedWithPULX().ConvertAll(trans => trans as Rtrans).Where(rtr => rtr.T_artiklCD.NotEmpty()).ToList();

      //theGrid.Rows.Clear();
      while(theDUC.TheG.RowCount > 1)
      {
         theDUC.TheG.Rows.RemoveAt(0);
      }

      var groupedRtranoList = theDUC.faktur_rec.TrnNonDel2
       //.GroupBy(rto => rto.T_artiklCD)
         .GroupBy(rto => rto.R_artCDAndGrCD)
         .Select(grp => new 
         { 
            artiklCD   = /*grp.Key*/grp.First().T_artiklCD, 
            artiklName = grp.First().T_artiklName, 
            artKolSum  = grp.Sum(r => r.T_kol), 
            grCD       = grp.First().T_grCD ,
         })
         .OrderBy(grp => grp.artiklName, new VvCompareStringsOrdinal());

      idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      foreach(var rtranoGroup in groupedRtranoList)
      {
         rtrans_rec = savedRtranses.FirstOrDefault(rtr => rtr.T_artiklCD == rtranoGroup.artiklCD && rtr.R_grName == rtranoGroup.grCD);

         if(rtrans_rec == null)
         {
            rtrans_rec = new Rtrans();

            rtrans_rec.T_artiklCD   = rtranoGroup.artiklCD  ;
            rtrans_rec.T_artiklName = rtranoGroup.artiklName;
            rtrans_rec.R_grName     = rtranoGroup.grCD      ; // trik 
            rtrans_rec.T_jedMj      = isBOR ? "kom" : "m3";
          //rtrans_rec.T_pdvSt      = Faktur.CommonPdvStForThisDate(theDUC.faktur_rec.PdvDate, true);

            #region Eventual ArtiklSifrar data (fuse?)

            //artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtranoGroup.artiklCD);
            //if(artikl_rec != null)
            //{
            //   rtrans_rec.T_jedMj = artikl_rec.JedMj;

            //   if(artikl_rec.Grupa1CD != "TRUP")
            //   {
            //      rtrans_rec.T_TT = theDUC.faktur_rec.TT/*Info.SplitTT*/;
            //   }
            //}

            #endregion Eventual ArtiklSifrar data

            #region Get T_Cij

            if(isPIX) // ne treba za IRA-2 
            {
               ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec.T_artiklCD, theDUC.faktur_rec.SkladCD, theDUC.faktur_rec.SkladDate);
               if(artStat_rec != null) rtrans_rec.T_cij = artStat_rec.PrNabCij.Ron(4);
            }
            else if(isBOR) // ne treba za IRA-2 
            {
               ArtStat artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, rtrans_rec.T_artiklCD, /*theDUC.faktur_rec.SkladCD*/  /* TODO: */"MPSK", theDUC.faktur_rec.SkladDate);
               if(artStat_rec != null) rtrans_rec.T_cij = artStat_rec./*MalopCij*/LastUlazMPC;
            }

            #endregion Get T_Cij

         }
         else
         {
            savedRtranses.Remove(rtrans_rec);
         }

         rtrans_rec.T_kol = rtranoGroup.artKolSum; // voila 

         #region Put, Calc, ... DgvFields

         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.PutDgvLineFields1(rtrans_rec, rowIdx, true);
         rtrans_rec.CalcTransResults(null/*faktur_rec*/);
         theDUC.PutDgvLineResultsFields1(rowIdx, rtrans_rec, false);
         theDUC.GetDgvLineFields1(rowIdx, false, null); // da napuni Document's business.Transes 

         #endregion Put, Calc, ... DgvFields

         #region Dodaj Dupli red po Rtrano.T_grCD (cjenovni razred + dodatni opis stavke)

         if(rtranoGroup.grCD.NotEmpty())
         {
            VvLookUpItem theLui = ZXC.luiListaRtranoGr.SingleOrDefault(l => l.Cd == rtranoGroup.grCD);

            if(theLui != null)
            {
               rtrans_rec = new Rtrans();

               rtrans_rec.T_artiklName = theLui.Name;

               theDUC.TheG.Rows.Add();

               rowIdx = theDUC.TheG.RowCount - idxCorrector;

               theDUC.PutDgvLineFields1(rtrans_rec, rowIdx, true);
               theDUC.GetDgvLineFields1(rowIdx, false, null); // da napuni Document's business.Transes 
            }
         }

         #endregion Dodaj Dupli red po Rtrano.T_grCD (cjenovni razred + dodatni opis stavke)

      } // foreach(var rtranoGroup in groupedRtranoList)

      #region Dodaj eventualno predPostojece rtrans-e koji NISU dosli via rtrano grid (TheG2)

      if(isBOR == false) foreach(Rtrans savedRtrans_rec in savedRtranses) // ne za BOR! 
      {
         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.PutDgvLineFields1(savedRtrans_rec, rowIdx, true);
         savedRtrans_rec.CalcTransResults(null/*faktur_rec*/);
         theDUC.PutDgvLineResultsFields1(rowIdx, savedRtrans_rec, false);
         theDUC.GetDgvLineFields1(rowIdx, false, null); // da napuni Document's business.Transes 
      }

      #endregion Dodaj eventualno predPostojece rtrans-e koji NISU dosli via rtrano grid (TheG2)

      VvDocumentRecordUC.RenumerateLineNumbers(theDUC.TheG, 0);
      theDUC.UpdateLineCount(theDUC.TheG);

      theDUC.PutDgvTransSumFields();
      theDUC.PutTransSumToDocumentSumFields();

      if(isPIX) // ne treba za IRA-2 
      {
         PutAndCalcOtpad(sender, e);
         theDUC.RefreshPIZX_RowColours();
      }

      #endregion SynthesizeG2toG1

   }

   private void GetDgvLineField2_CalcVolumen_PutDgvLineField2_T_kol(FakturPDUC theDUC, int rIdx)
   {
      Rtrano rtrano_rec;

      ZXC.Rtrano_CalcVolumenEnum calcKind;

      rtrano_rec = (Rtrano)theDUC.GetDgvLineFields2(rIdx, false, null);

           if(theDUC is URPDUC)  calcKind = ZXC.Rtrano_CalcVolumenEnum.TRUPAC;
      else if(theDUC is PRIpDUC) calcKind = ZXC.Rtrano_CalcVolumenEnum.TRUPAC;
      else if(theDUC is IRPDUC)  calcKind = ZXC.Rtrano_CalcVolumenEnum.LETVA ;

      else if(theDUC is PIZpDUC) { calcKind = GetCalcKindViaDim(rtrano_rec); if(calcKind == ZXC.Rtrano_CalcVolumenEnum.FUSE) return; }

      else { ZXC.aim_emsg(MessageBoxIcon.Error, "Nedefinirani CalcVolumenKind za DUC {0}", theDUC); return; }

      // -------------------------------------------------------------------------------- 

      rtrano_rec.CalcVolumen(calcKind, rtrano_rec.T_isKomDummy);

      theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_kol, rIdx, rtrano_rec.T_kol);
   }

   private ZXC.Rtrano_CalcVolumenEnum GetCalcKindViaDim(Rtrano rtrano_rec)
   {
      if(rtrano_rec.T_dimY.IsZero () &&
         rtrano_rec.T_dimX.NotZero() &&
         rtrano_rec.T_dimZ.NotZero())
      {
         return ZXC.Rtrano_CalcVolumenEnum.TRUPAC;
      }
      else if(rtrano_rec.T_dimY.NotZero() &&
              rtrano_rec.T_dimX.NotZero() &&
              rtrano_rec.T_dimZ.NotZero())
      {
         return ZXC.Rtrano_CalcVolumenEnum.LETVA;
      }
      else
      {
         return ZXC.Rtrano_CalcVolumenEnum.FUSE;
      }
   }

   #endregion CalcVolumen_SynthesizeG2toG1

   private void PutAndCalcOtpad(object sender, EventArgs e)
   {

      PutAndCalcOtpadPiljDlg dlg = new PutAndCalcOtpadPiljDlg();

      dlg.TheUC.Fld_CijOtpada = ZXC.TheVvForm.VvPref.calcOtpadPrefs.OtpadCij;
      dlg.TheUC.Fld_CijPiljev = ZXC.TheVvForm.VvPref.calcOtpadPrefs.PiljvCij;
      dlg.TheUC.Fld_UdioPuO   = ZXC.TheVvForm.VvPref.calcOtpadPrefs.UdioPuO ;

      if(sender.ToString() == "RISK_RewriteAllDocuments")
      {
      }
      else // classic
      {
         if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }
      }

      ZXC.TheVvForm.VvPref.calcOtpadPrefs.OtpadCij = dlg.TheUC.Fld_CijOtpada;
      ZXC.TheVvForm.VvPref.calcOtpadPrefs.PiljvCij = dlg.TheUC.Fld_CijPiljev;
      ZXC.TheVvForm.VvPref.calcOtpadPrefs.UdioPuO  = dlg.TheUC.Fld_UdioPuO  ;

      #region variablez

      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;
      int rowIdxOtpad  = -1, idxCorrector;
      int rowIdxPljvna = -1;

      // === OTPAD start ======================================================== 

      Artikl artiklOTPAD_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == Artikl.OtpadArtiklCD);
      if(artiklOTPAD_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "U Šifrarniku Artikala ne postoji artikl '{0}'", Artikl.OtpadArtiklCD);
         return;
      }

      Rtrans rtransOTPAD_rec = new Rtrans()
         {
            T_artiklCD   = artiklOTPAD_rec.ArtiklCD  ,
            T_artiklName = artiklOTPAD_rec.ArtiklName,
            T_jedMj      = artiklOTPAD_rec.JedMj     ,
            T_TT         = Faktur.TT_PUX        
         };

      // === OTPAD end   PILJEVINA start ======================================== 

      Artikl artiklPLJVNA_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == Artikl.PljvnArtiklCD);
      if(artiklPLJVNA_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "U Šifrarniku Artikala ne postoji artikl '{0}'", Artikl.PljvnArtiklCD);
         return;
      }

      Rtrans rtransPljvna_rec = new Rtrans()
      {
         T_artiklCD = artiklPLJVNA_rec.ArtiklCD,
         T_artiklName = artiklPLJVNA_rec.ArtiklName,
         T_jedMj = artiklPLJVNA_rec.JedMj,
         T_TT = Faktur.TT_PUX
      };

      // === PILJEVINA end ====================================================== 

      theDUC.GetFields(false); // Bussiness ok 

      #endregion variablez

      #region Calc & Put Otpad / Piljevina Kol / Cij

      rtransOTPAD_rec .T_kol  = theDUC.faktur_rec.TrnSum_K_PULX_THEORETIC_OTPAD.Ron(3)       ;
      rtransPljvna_rec.T_kol  = ZXC.VvGet_25_of_100(rtransOTPAD_rec.T_kol, dlg.TheUC.Fld_UdioPuO);
      rtransOTPAD_rec .T_kol -= rtransPljvna_rec.T_kol;

      rtransOTPAD_rec .T_noCijMal = dlg.TheUC.Fld_CijOtpada;
      rtransPljvna_rec.T_noCijMal = dlg.TheUC.Fld_CijPiljev;

      decimal izlazTrupacaFIN             = theDUC.faktur_rec.TrnSum_KC_PIZX;
      // 08.07.2015.
    //decimal zaPodjelitiPoProizvodimaFIN = izlazTrupacaFIN - ( rtransOTPAD_rec.R_KC                                +  rtransPljvna_rec.R_KC                                );
      decimal zaPodjelitiPoProizvodimaFIN = izlazTrupacaFIN - ((rtransOTPAD_rec.T_kol * rtransOTPAD_rec.T_noCijMal) + (rtransPljvna_rec.T_kol * rtransPljvna_rec.T_noCijMal));
      decimal ulazProizvodKOL             = theDUC.faktur_rec.TrnSum_K_PULX_P;
      decimal korCijenaProzivoda          = ZXC.DivSafe(zaPodjelitiPoProizvodimaFIN, ulazProizvodKOL);

      #endregion Calc & Put Otpad / Piljevina Kol / Cij

      #region Put, Calc, ... DgvFields

      // ========================================================================= 
      // find mozebitno vec postojeci 'otpad' redak 
      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         if(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, rIdx, false) == Artikl.OtpadArtiklCD)
         {
            rowIdxOtpad = rIdx;
            break;
         }
      }

      if(rowIdxOtpad.IsNegative()) // NE nadjosmo postojeci 'otpad' redak 
      {
         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);
         theDUC.TheG.Rows.Add();
         rowIdxOtpad = theDUC.TheG.RowCount - idxCorrector;
      }

      theDUC.PutDgvLineFields1(rtransOTPAD_rec, rowIdxOtpad, true);
      rtransOTPAD_rec.CalcTransResults(null/*faktur_rec*/);
      theDUC.PutDgvLineResultsFields1(rowIdxOtpad, rtransOTPAD_rec, false);
      theDUC.GetDgvLineFields1(rowIdxOtpad, false, null); // da napuni Document's business.Transes 

      // ========================================================================= 
      // find mozebitno vec postojeci 'PILJEVINA' redak 
      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         if(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, rIdx, false) == Artikl.PljvnArtiklCD)
         {
            rowIdxPljvna = rIdx;
            break;
         }
      }

      if(rowIdxPljvna.IsNegative()) // NE nadjosmo postojeci 'PLJVNA' redak 
      {
         idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);
         theDUC.TheG.Rows.Add();
         rowIdxPljvna = theDUC.TheG.RowCount - idxCorrector;
      }

      theDUC.PutDgvLineFields1(rtransPljvna_rec, rowIdxPljvna, true);
      rtransPljvna_rec.CalcTransResults(null/*faktur_rec*/);
      theDUC.PutDgvLineResultsFields1(rowIdxPljvna, rtransPljvna_rec, false);
      theDUC.GetDgvLineFields1(rowIdxPljvna, false, null); // da napuni Document's business.Transes 
      // ========================================================================= 

      // Put PROIZVODI KorigiranaCijena
      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         if(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, rIdx, false) == Artikl.OtpadArtiklCD) continue;
         if(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, rIdx, false) == Artikl.PljvnArtiklCD) continue;

         if(VvCheckBox.GetBool4String(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_isProductLine, rIdx, false)) == false) continue;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_noCijMal, rIdx, korCijenaProzivoda);
      }

      theDUC.PutDgvTransSumFields1();
      theDUC.PutTransSumToDocumentSumFields();

      theDUC.TheVvTabPage.TheVvForm.SetDirtyFlag(this);

      #endregion Put, Calc, ... DgvFields

      dlg.Dispose(); 
   }


   /*private*/
   public void RISK_LoadPPukSirovine(object sender, EventArgs e)
   {
      LoadPopratnica(/*sender*/"SIROVINE4PIX", e);
   }

   #endregion PPUK PROIZVODNJA

   private void RISK_AvansStorno_inNY(object sender, EventArgs e)
   {
      ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Ovo je eksperimentalno rješenje.\n\r\n\rPotrebno je dobro provjeriti sadržaj novonastalog računa u novoj godini!!!");
   
      XSqlConnection nextYearDbConn = ZXC.TheSecondDbConn_SameDB_OtherYear(DateTime.Now.Year);

      bool OK;

      ZXC.CopyOut_InProgress = true;

      Faktur origFaktur_rec = (Faktur)(TheVvDataRecord);

      Faktur nyFaktur_rec = (Faktur)origFaktur_rec.CreateNewRecordAndCloneItComplete();

      OK = VvRecLstUC.EnsureNonDuplicateKeys(nextYearDbConn, nyFaktur_rec, false, false);

      #region Alter fields for storno

      foreach(Rtrans rtrans in nyFaktur_rec.Transes)
      {
         rtrans.T_kol *= -1.00M;

         rtrans.CalcTransResults(null);
      }

      nyFaktur_rec.TakeTransesSumToDokumentSum(true);

      nyFaktur_rec.VezniDok = origFaktur_rec.TtNumFiskal;

      // !!! !!! !!! 
      if(origFaktur_rec.IsF2)
      {
         nyFaktur_rec.F2_PrvFakYYiRecID = origFaktur_rec.GetFaktur_YYandRecID; // referenca na prethodni dokument 
      }

      string stornoStr = "Nakon AVANSA " + origFaktur_rec.DokDate.Year + " god. "+ origFaktur_rec.TT + "-" + origFaktur_rec.TtNum + "/";

      string oldNapomena = origFaktur_rec.Napomena;

      nyFaktur_rec.Napomena  = ZXC.LenLimitedStr(stornoStr + oldNapomena, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena));

      if(nyFaktur_rec.S_ukKCRP_NP1.NotZero()) nyFaktur_rec.S_ukKCRP_NP1 *= -1.00M;

      if(nyFaktur_rec.IsF2)
      {
         nyFaktur_rec.PdvKolTip =  ZXC.VvUBL_PolsProcEnum.P04;
         nyFaktur_rec.StatusCD  = "386";

         nyFaktur_rec.F2_PrvFakYYiRecID = origFaktur_rec.GetFaktur_YYandRecID; // referenca na prethodni dokument 
      }

      nyFaktur_rec.DokDate  = 
      nyFaktur_rec.SkladDate =
      nyFaktur_rec.PdvDate  = 
      nyFaktur_rec.DospDate = VvSQL.GetServer_DateTime_Now(TheDbConnection);
      nyFaktur_rec.RokPlac  = 0;

      nyFaktur_rec.FiskJIR =
      nyFaktur_rec.FiskZKI =
      nyFaktur_rec.V1_tt   =
      nyFaktur_rec.V2_tt   =
      nyFaktur_rec.V3_tt   =
      nyFaktur_rec.V4_tt   = "";

      nyFaktur_rec.V1_ttNum =
      nyFaktur_rec.V2_ttNum =
      nyFaktur_rec.V3_ttNum =
      nyFaktur_rec.V4_ttNum = 0;

      #endregion Alter fields for storno

      OK = ZXC.FakturDao.ADDREC(nextYearDbConn, nyFaktur_rec, true, false, false, false);

      ZXC.CopyOut_InProgress = false;

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, $"Dodan storno ovog AVANSA u NOVOJ {DateTime.Now.Year} godini.\n\r\n\r{nyFaktur_rec}");
      else   ZXC.aim_emsg(MessageBoxIcon.Stop,        $"Storno ovog AVANSA u NOVOJ {DateTime.Now.Year} godini.\n\r\n\rNIJE USPIO!");
   }
   /*private*/ public void RISK_AvansStorno(object sender, EventArgs e)
   {
      // 15.01.2026: 
      if(ZXC.RISK_CopyToOtherDUC_inProgress || ZXC.RISK_CopyToMixerDUC_inProgress || ZXC.MIXER_CopyToOtherDUC_inProgress)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Morate prvo usnimiti neku ranije započetu akciju kopiranja.");
         return;
      }

      if(ZXC.IsTEXTHOshop) return;

      bool isPGfaktur = (TheVvDataRecord as Faktur).DokDate.Year != DateTime.Now.Year;

      // kopiraj ovaj dokument u isti takav TT
      // kolicine postavi na -
      // u t_pdvKolTip postavi "A"
      // u Fld_VezniDok / VezniDok stavi broj racuna kojeg kopiras u izgledu 5-1-1
      //

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      decimal old_kol;

      bool isFromPGyear = theDUC is WYRNDUC;

      if(!theDUC.faktur_rec.Is_F2_Avans && !theDUC.faktur_rec.IsF1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Avans Storno radi se samo sa avansa, a ovo nije avans.");

         return;
      }

      if(isPGfaktur)
      {
         DialogResult result = MessageBox.Show($"Da li želite proizvesti storno u NOVOJ {DateTime.Now.Year} godini?",
            "Potvrdite STORNO u NG", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

         if(result == DialogResult.Yes)
         {
            RISK_AvansStorno_inNY(sender, e);
            return;
         }
      }

      if(isFromPGyear)
      {
         Point xy = Point.Empty;

         xy = ZXC.TheVvForm.GetSubModulXY(ZXC.TheVvForm.GetVvSubModulEnumFrom_SubModulShortName(ZXC.RRD.Dsc_F2_TT));

         // --- Here we go! --- 

         ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

         VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

         ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

         ZXC.RISK_CopyToOtherDUC_inProgress = true;

         if(existingTabPage != null) existingTabPage.Selected = true; 
         else                        OpenNew_Record_TabPage(xy, null);

         NewRecord_OnClick(TheVvDataRecord, new NewRecordEventArgs(ZXC.FakturRec, (FakturDUC)TheVvUC, true));

         theDUC = (FakturExtDUC)TheVvUC;

         // 31.03.2018: 
         SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

      } // if(isFromPGyear)

      else // classic
      {
         DupCopyMenu_Button_OnClick(sender, e);
      }

      if(ZXC.FakturRec == null) return;

      for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
      {
         old_kol = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol, rIdx, false);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, rIdx, (-old_kol));

         if(ZXC.IsF2_2026_rules == false) // staro ponasanje. u 2026 ne smije vise ic 'A' u PdvKolTip 
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvKolTip, rIdx, theDUC.GetOneLetter4PdvKolTip(ZXC.PdvKolTipEnum.AVANS_STORNO));
         }

         theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(rIdx);
      }

      theDUC.Fld_VezniDok = /*theDUC.faktur_rec*/ZXC.FakturRec.TtNumFiskal;

    //string stornoStr   = "Nakon AVANSA " + theDUC.faktur_rec.BackupData._tt + "-" + theDUC.faktur_rec.BackupData._ttNum + "/";
      string stornoStr   = "Nakon AVANSA " + ZXC.FakturRec.TipBr;
      string oldNapomena = theDUC.faktur_rec.Napomena;

      if(oldNapomena.Length + stornoStr.Length <= theDUC.tbx_Napomena.MaxLength) theDUC.Fld_Napomena  = stornoStr + oldNapomena;
      else                                                                       theDUC.Fld_Napomena  = stornoStr + oldNapomena.Substring(0, oldNapomena.Length - stornoStr.Length);

      if(ZXC.FakturRec.IsF2)
      {
       //theDUC.Fld_eRproc = (ZXC.VvUBL_PolsProcEnum)Enum.Parse(typeof(ZXC.VvUBL_PolsProcEnum), wanted_eRproc);
         theDUC.Fld_eRproc =  ZXC.VvUBL_PolsProcEnum.P04; 
         VvLookUpItem lui = ZXC.luiListaeRacPoslProc.GetLuiForThisCd(/*theDUC.Fld_eRproc.ToString()*/ "4"); 
         if(lui != null) theDUC.Fld_eRprocOpis = lui.Name;

         lui = ZXC.luiListaKodTipaEracuna.GetLuiForThisCd("386"); 
         theDUC.Fld_StatusCD   = lui != null ? lui.Cd   : "";
         theDUC.Fld_StatusOpis = lui != null ? lui.Name : "";

         // !!! !!! !!! 
       //theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.               RecID; // referenca na prethodni dokument 
         theDUC.Fld_F2_PrvFakRecID = ZXC.FakturRec.GetFaktur_YYandRecID; // referenca na prethodni dokument 
      }
   }
   /*private*/ public void RISK_FinalRn(object sender, EventArgs e)
   {
      //ZXC.aim_emsg(MessageBoxIcon.Stop, "Za ovu funkcionalnost, molim Vas, kontaktirajte support."); return;

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      
      bool isAvansStornoF1 = theDUC.faktur_rec.Napomena.ToUpper().Contains("NAKON AVANSA") &&
                             theDUC.faktur_rec.S_ukKCRP.IsNegative();

      bool isAvansStornoF2 = theDUC.faktur_rec.PdvKolTip == ZXC.VvUBL_PolsProcEnum.P04 &&
                             theDUC.faktur_rec.StatusCD == "386" &&
                             theDUC.faktur_rec.S_ukKCRP.IsNegative();

      if(!isAvansStornoF1 && !isAvansStornoF2) 
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Konačni račun izrađuje se samo iz STORNA avansa / predujma.");
         return;
      }

      ZXC.RISK_FinalRn_inProgress = true;

      DupCopyMenu_Button_OnClick(sender, e);

      ZXC.RISK_FinalRn_inProgress = false;
   }

   /*private*/ public void RISK_RBT_4Q/*_ORIG*/(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      VvNewRabatDlg dlg = new VvNewRabatDlg();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_rbt1St, rIdx, dlg.Fld_NewRabat);
         }
      }

      dlg.Dispose();

      theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   /*private*/ public void RISK_Baksa(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      if(theDUC.TheG.RowCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Dozvoljeno samo na 'praznom' računu (bez redaka)");
         return;
      }

      VvBaksaDlg dlg = new VvBaksaDlg();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         string baksaArtCD = "007";
         int rIdx = 0;

         Artikl artikl_rec = theDUC.Get_Artikl_FromVvUcSifrar(baksaArtCD);

         if(artikl_rec != null)
         {
            Rtrans rtrans_rec = new Rtrans()
            {
                  T_TT          = Faktur.TT_IRM, // za calc 
                  T_skladDate   = theDUC.Fld_DokDate,

                  T_artiklCD    = artikl_rec.ArtiklCD,
                  T_artiklName  = artikl_rec.ArtiklName,
                  T_cij         = dlg.Fld_Baksa,
                //T_wanted      = dlg.Fld_Baksa,
                  T_kol         = 1.00M,
                  T_pdvSt       = Faktur.CommonPdvStForThisDate(theDUC.Fld_DokDate),
                  T_isIrmUsluga = true
            };

            theDUC.TheG.Rows.Add();

            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD   , rIdx, artikl_rec.ArtiklCD);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName , rIdx, artikl_rec.ArtiklName);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij        , rIdx, dlg.Fld_Baksa);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij_kcrp   , rIdx, dlg.Fld_Baksa);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol        , rIdx, 1.00M);
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvSt      , rIdx, Faktur.CommonPdvStForThisDate(theDUC.Fld_DokDate));
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_isIrmUsluga, rIdx, "X");

            theDUC.PutDgvLineFields(rtrans_rec, rIdx, true);

            rtrans_rec.CalcTransResults(null/*faktur_rec*/);
            theDUC.PutDgvLineResultsFields1(rIdx, rtrans_rec, false);
            theDUC.GetDgvLineFields(rIdx, false, null); // da napuni Document's business.Transes 

         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Nema 'bakša' artikla {0}", baksaArtCD);
         }
      }

      dlg.Dispose();

      theDUC.PutDgvTransSumFields();
      theDUC.PutTransSumToDocumentSumFields();
    //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   private void KKM_AddNewArtikl(object sender, EventArgs e)
   {
      AddArtiklUMJDlg dlg = new AddArtiklUMJDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      if(theDUC.Fld_KupdobName.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Prije ove akcije zadajte partnera."); return; }

      Artikl artikl_rec = new Artikl();

      string artCDpreffix = "LK"; // TODO: !!!!! da ovo dojde sa nekog rules-a 

      uint nextNum = ZXC.ArtiklDao.GetNextSifraWroot_String(TheDbConnection, Artikl.recordName, artikl_rec.SifraColName, artCDpreffix);

      artikl_rec.ArtiklCD   = artCDpreffix + nextNum; 
      artikl_rec.ArtiklName = Artikl.CreateUmjetninaNaziv(dlg.TheUC.Fld_ArtiklName, artikl_rec.ArtiklCD, theDUC.Fld_KupdobName);
      artikl_rec.Velicina   = dlg.TheUC.Fld_Format; 

      artikl_rec.PdvKat   = Faktur.CommonPdvStForThisDate(theDUC.Fld_DokDate).ToString("00");
      artikl_rec.SkladCD  = theDUC.Fld_SkladCD;
      artikl_rec.TS       = "ROB";
      artikl_rec.Grupa1CD = ZXC.UmjetninaGrCD;
      artikl_rec.JedMj    = "KOM";
      artikl_rec.Grupa2CD = ZXC.LenLimitedStr(dlg.TheUC.Fld_GrupaB, ZXC.ArtiklDao.GetSchemaColumnSize(ZXC.ArtCI.grupa2CD));
      artikl_rec.Velicina = ZXC.LenLimitedStr(dlg.TheUC.Fld_Format, ZXC.ArtiklDao.GetSchemaColumnSize(ZXC.ArtCI.velicina));

      //20.11.2013. kaj ja ne vidim da nigdje nama proizvodjaca ili je to stvarno tak
      artikl_rec.ProizCD = theDUC.Fld_KupdobCd;

      dlg.Dispose();

      bool OK = artikl_rec.VvDao.ADDREC(TheDbConnection, artikl_rec);

      //theDUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);

      if(OK)
      {
         int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);
        
         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD, rowIdx, artikl_rec.ArtiklCD);
         theDUC.TheG.CurrentCell = theDUC.TheG[theDUC.DgvCI.iT_artiklCD, rowIdx];
         theDUC.TheG.BeginEdit(false);
         theDUC.originalText = "";
         theDUC.AnyArtiklTextBox_OnGrid_Leave(theDUC.TheG.EditingControl, e);
         theDUC.TheG.EndEdit();
         SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");
      }
   }

   private decimal GetOP_CIJ(decimal normalCIJ, decimal origPak)
   {
      return ZXC.DivSafe(normalCIJ, origPak);
   }

   private void ShowArtiklInfo(object sender, EventArgs e)
   {
      #region Init Stuff

      if(TheVvUC is FakturDUC == false) return;

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      // 06.11.2023: 
      if(theDUC.faktur_rec.TT != Faktur.TT_IRA &&
         theDUC.faktur_rec.TT != Faktur.TT_IZD &&
         theDUC.faktur_rec.TT != Faktur.TT_PON &&
         theDUC.faktur_rec.TT != Faktur.TT_OPN) return;

      if(theDUC.TheG.CurrentRow == null)        return;

      int currRowIdx      = theDUC.TheG.CurrentRow.Index;
      Artikl artikl_rec = null;
      ArtStat artStat_rec = null;
      string currArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, currRowIdx, false);
      if(currArtiklCD.NotEmpty()) artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == currArtiklCD);

      bool doWeWant_OP = false;

      if(currArtiklCD.IsEmpty() || artikl_rec == null) return;

      #endregion Init Stuff

      #region Get ArtiklInfo Data

    //artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, (Rtrans)theDUC.GetDgvLineFields1(currRowIdx, false, null));
      artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl_rec.ArtiklCD, theDUC.Fld_SkladCD);

      decimal    ulazProsCij,    ulazMinCij,    ulazMaxCij,    ulazLastCij,    ulazProsCijOP,    ulazMinCijOP,    ulazMaxCijOP,    ulazLastCijOP,
                izlazProsCij,   izlazMinCij,   izlazMaxCij,   izlazLastCij,   izlazProsCijOP,   izlazMinCijOP,   izlazMaxCijOP,   izlazLastCijOP,
              thisKupProsCij, thisKupMinCij, thisKupMaxCij, thisKupLastCij, thisKupProsCijOP, thisKupMinCijOP, thisKupMaxCijOP, thisKupLastCijOP;
      DateTime lastUlaz, lastIzlaz, thisKupLastIzlaz;

      if(artStat_rec != null)
      {
         #region Artikl Data

         doWeWant_OP = artStat_rec.OrgPak != 1.00M;

       //decimal ulazProsCij    = artStat_rec.UlazCijProsKNJ   ; decimal izlazProsCij   = artStat_rec.IzlazCijProsKNJ  ;
         ulazProsCij   = artStat_rec.PrNabCij     ; izlazProsCij   = artStat_rec.IzlProdCijPros  ;
         ulazMinCij    = artStat_rec.UlazCijMin   ; izlazMinCij    = artStat_rec.IzlazCijMin     ;
         ulazMaxCij    = artStat_rec.UlazCijMax   ; izlazMaxCij    = artStat_rec.IzlazCijMax     ;
         ulazLastCij   = artStat_rec.UlazCijLast  ; izlazLastCij   = artStat_rec.IzlazCijLast    ;

         if(doWeWant_OP)
         {
            ulazProsCijOP = artStat_rec.PrNabCijOP   ; izlazProsCijOP = artStat_rec.IzlProdCijProsOP;
            ulazMinCijOP  = artStat_rec.UlazCijMinOP ; izlazMinCijOP  = artStat_rec.IzlazCijMinOP   ;
            ulazMaxCijOP  = artStat_rec.UlazCijMaxOP ; izlazMaxCijOP  = artStat_rec.IzlazCijMaxOP   ;
            ulazLastCijOP = artStat_rec.UlazCijLastOP; izlazLastCijOP = artStat_rec.IzlazCijLastOP  ;
         }
         else
         {
            ulazProsCijOP = 0.00M; izlazProsCijOP = 0.00M;
            ulazMinCijOP  = 0.00M; izlazMinCijOP  = 0.00M;
            ulazMaxCijOP  = 0.00M; izlazMaxCijOP  = 0.00M;
            ulazLastCijOP = 0.00M; izlazLastCijOP = 0.00M;
         }

         lastUlaz      = artStat_rec.DateZadUlaz  ; lastIzlaz      = artStat_rec.DateZadIzlaz    ;

         #endregion Artikl Data

         #region ThisKupac Data

         #region Get thisKupacRtransList
         List<Rtrans> thisKupacRtransList = new List<Rtrans>();

         List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4); DataRowCollection rtrSch = ZXC.RtransSchemaRows; RtransDao.RtransCI rtrCI = ZXC.RtrCI;
   
         filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_artiklCD  ], false, "theArtiklCD"  , artikl_rec.ArtiklCD    , "", "", " = " , "", "R"));
         filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_kupdobCD  ], false, "theKCD"       , theDUC.Fld_KupdobCd    , "", "", " = " , "", "R"));
       //filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_tt        ], false, "theTT"        , TtInfo.Prihod_IN_Clause, "", "", " IN ", "", "R"));
         filterMembers.Add(new VvSqlFilterMember("R.t_tt",                          "theTT"        , TtInfo.Prihod_IN_Clause, "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
         filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladCD   ], false, "theSklCD"     , theDUC.Fld_SkladCD     , "", "", " = " , "", "R"));
       //filterMembers.Add(new VvSqlFilterMember(rtrSch[rtrCI.t_skladDate ], false, "theDate"      , theDUC.Fld_DokDate     , "", "", " <= ", "", "R"));
   
         RtransDao.GetRtransWithArtstatList(TheDbConnection, thisKupacRtransList, "", filterMembers, Rtrans.artiklOrderBy_ASC.Replace("t_", "R.t_"));

         #endregion Get thisKupacRtransList

         if(thisKupacRtransList != null && thisKupacRtransList.Count.NotZero())
         {
            thisKupProsCij   = ZXC.DivSafe(thisKupacRtransList.Sum(rtr => rtr.R_KCR), thisKupacRtransList.Sum(rtr => rtr.R_kol));
            thisKupMinCij    = thisKupacRtransList.Min (rtr => rtr.R_CIJ_KCR) ;
            thisKupMaxCij    = thisKupacRtransList.Max (rtr => rtr.R_CIJ_KCR) ;
            thisKupLastCij   = thisKupacRtransList.         Last().R_CIJ_KCR  ;
            thisKupLastIzlaz = thisKupacRtransList.         Last().T_skladDate;

            if(doWeWant_OP)
            {
               thisKupProsCijOP = GetOP_CIJ(thisKupProsCij, artStat_rec.OrgPak);
               thisKupMinCijOP  = GetOP_CIJ(thisKupMinCij , artStat_rec.OrgPak);
               thisKupMaxCijOP  = GetOP_CIJ(thisKupMaxCij , artStat_rec.OrgPak);
               thisKupLastCijOP = GetOP_CIJ(thisKupLastCij, artStat_rec.OrgPak);
            }
            else
            {
               thisKupProsCijOP = 0.00M;
               thisKupMinCijOP  = 0.00M;
               thisKupMaxCijOP  = 0.00M;
               thisKupLastCijOP = 0.00M;
            }
         }
         else
         {
            thisKupProsCij   = 
            thisKupMinCij    = 
            thisKupMaxCij    = 
            thisKupLastCij   = 
            thisKupProsCijOP = 
            thisKupMinCijOP  = 
            thisKupMaxCijOP  = 
            thisKupLastCijOP = 0.00M;
            thisKupLastIzlaz = DateTime.MinValue;

         }

         #endregion ThisKupac Data

      }
      else return;

      #endregion Get ArtiklInfo Data

      #region Dialog

      decimal newCij = 0.00M;

      ShowArtiklInfoDlg dlg = new ShowArtiklInfoDlg(doWeWant_OP, artStat_rec.OrgPak, artStat_rec.ArtiklJM, artStat_rec.OrgPakJM);

      dlg.TheUC.Fld_Artikl = artikl_rec.ArtiklCD + " [" + artikl_rec.ArtiklName + "]";
      dlg.TheUC.Fld_Kupac  = theDUC.Fld_KupdobCd + " [" + theDUC.Fld_KupdobName + "]";
      dlg.TheUC.Fld_Sklad  = theDUC.Fld_SkladCD  + " [" + theDUC.Fld_SkladOpis  + "]";

      dlg.TheUC.Fld_UlazProsCij          = ulazProsCij     ; 
      dlg.TheUC.Fld_UlazProsCijOP        = ulazProsCijOP   ; 
      dlg.TheUC.Fld_UlazMinCij           = ulazMinCij      ; 
      dlg.TheUC.Fld_UlazMinCijOP         = ulazMinCijOP    ; 
      dlg.TheUC.Fld_UlazMaxCij           = ulazMaxCij      ; 
      dlg.TheUC.Fld_UlazMaxCijOP         = ulazMaxCijOP    ; 
      dlg.TheUC.Fld_UlazLastCij          = ulazLastCij     ; 
      dlg.TheUC.Fld_UlazLastCijOP        = ulazLastCijOP   ; 
      dlg.TheUC.Fld_IzlazProsCij         = izlazProsCij    ; 
      dlg.TheUC.Fld_IzlazProsCijOP       = izlazProsCijOP  ; 
      dlg.TheUC.Fld_IzlazMinCij          = izlazMinCij     ; 
      dlg.TheUC.Fld_IzlazMinCijOP        = izlazMinCijOP   ; 
      dlg.TheUC.Fld_IzlazMaxCij          = izlazMaxCij     ; 
      dlg.TheUC.Fld_IzlazMaxCijOP        = izlazMaxCijOP   ; 
      dlg.TheUC.Fld_IzlazLastCij         = izlazLastCij    ; 
      dlg.TheUC.Fld_IzlazLastCijOP       = izlazLastCijOP  ; 
      dlg.TheUC.Fld_ThisKupProsCij       = thisKupProsCij  ; 
      dlg.TheUC.Fld_ThisKupProsCijOP     = thisKupProsCijOP; 
      dlg.TheUC.Fld_ThisKupMinCij        = thisKupMinCij   ; 
      dlg.TheUC.Fld_ThisKupMinCijOP      = thisKupMinCijOP ; 
      dlg.TheUC.Fld_ThisKupMaxCij        = thisKupMaxCij   ; 
      dlg.TheUC.Fld_ThisKupMaxCijOP      = thisKupMaxCijOP ; 
      dlg.TheUC.Fld_ThisKupLastCij       = thisKupLastCij  ; 
      dlg.TheUC.Fld_ThisKupLastCijOP     = thisKupLastCijOP;
      dlg.TheUC.Fld_DatelastIzlaz        = lastIzlaz       ;
      dlg.TheUC.Fld_DatelastUlaz         = lastUlaz        ;
      dlg.TheUC.Fld_DatethisKupLastIzlaz = thisKupLastIzlaz;

      dlg.TheUC.Fld_NewCijena   = ulazProsCij  ; 
      dlg.TheUC.Fld_NewCijenaOP = ulazProsCijOP;

      dlg.TheUC.Fld_IsNewCijOP = doWeWant_OP; 

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      newCij = dlg.TheUC.Fld_NewCijena;

      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij, currRowIdx, newCij);
      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx);

      #endregion Dialog
   }

   private void ProizvodViaNormativ(object sender, EventArgs e)
   {
   }

   private void FakturirajBoravak(object sender, EventArgs e)
   {
      Cursor.Current = Cursors.WaitCursor;

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      List<Rtrans> toFaktur_rtransList    = new List<Rtrans>();
      List<Rtrans> projekt_BOR_RtransList = theDUC.faktur_rec.CloneTranses().ConvertAll(trans => trans as Rtrans);
      List<Rtrans> projekt_IZM_RtransList = RtransDao.GetRtransList_ForProjektCD(TheDbConnection, theDUC.faktur_rec.TT_And_TtNum, Rtrans.artiklOrderBy_ASC);
      List<Faktur> projekt_IZM_FakturList = RtransDao.GetFakturList_ForProjektCD(TheDbConnection, theDUC.faktur_rec.TT_And_TtNum); // fuse? 

      if(projekt_IZM_FakturList.Any(f => f.TT == Faktur.TT_IRM))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Za ovaj boravak već postoji račun {0}", projekt_IZM_FakturList.First(f => f.TT == Faktur.TT_IRM).TT_And_TtNum);
         Cursor.Current = Cursors.Default;
         return;
      }

      TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      projekt_BOR_RtransList.ForEach(r => r.T_pdvSt = 13M);

      ushort serial = (ushort)(projekt_BOR_RtransList.Max(r => r.T_serial));

      var IZMrichRtranses = projekt_IZM_RtransList
         .Join(VvUserControl.ArtiklSifrar, rtr => rtr.T_artiklCD, art => art.ArtiklCD, (rtr, art) => new Rtrans()
         {
            currentData = rtr.currentData,
            _rtrResults = rtr._rtrResults,
            TheAsEx     = rtr.TheAsEx    ,
            R_grName    = art.Grupa1CD   ,
            R_utilBool  = art.IsPNP      , //porez na potrosnju 
         });

      // JPN - jelo, pice, napitak 
      var IZMrichRtranses_NeJPN = IZMrichRtranses.Where(r => r.R_isJPN == false)                   ; // Ostalo              

      var IZMrichRtranses_JPN25    = IZMrichRtranses.Where(r => r.R_utilBool == false && r.R_isJPN == true && r.T_pdvSt == 25M); // Jela i pića pdv 25% - bez poreza na potrosnju  
      var IZMrichRtranses_JPN13    = IZMrichRtranses.Where(r => r.R_utilBool == false && r.R_isJPN == true && r.T_pdvSt == 13M); // Jela i pića pdv 13% - bez poreza na potrosnju  
      var IZMrichRtranses_JPN05    = IZMrichRtranses.Where(r => r.R_utilBool == false && r.R_isJPN == true && r.T_pdvSt ==  5M); // Jela i pića pdv  5% - bez poreza na potrosnju  
      var IZMrichRtranses_JPN25pnp = IZMrichRtranses.Where(r => r.R_utilBool == true  && r.R_isJPN == true && r.T_pdvSt == 25M); // Jela i pića pdv 25% - sa  porezom na potrosnju 
      var IZMrichRtranses_JPN13pnp = IZMrichRtranses.Where(r => r.R_utilBool == true  && r.R_isJPN == true && r.T_pdvSt == 13M); // Jela i pića pdv 13% - sa  porezom na potrosnju 
      var IZMrichRtranses_JPN05pnp = IZMrichRtranses.Where(r => r.R_utilBool == true  && r.R_isJPN == true && r.T_pdvSt ==  5M); // Jela i pića pdv  5% - sa  porezom na potrosnju 

      var groupedIZMrichRtranses_NeJPN = IZMrichRtranses_NeJPN
         .GroupBy(rtr => rtr.T_artiklCD)
         .Select(grp => new Rtrans() 
            { 
               T_artiklCD   = /*grp.Key*/"", // !!!: IZM je vec skinuo lager, ovdje stavka samo opisna 
               T_artiklName = grp.First().T_artiklName + " [" + grp.Key + "]",
               T_kol        = grp.Sum(r => r.T_kol),
               T_cij        = ZXC.DivSafe(grp.Sum(r => r.R_KCRP), grp.Sum(r => r.T_kol)),
               T_jedMj      = grp.First().T_jedMj  ,
               T_pdvSt      = grp.First().T_pdvSt  ,
               T_serial     = ++serial             ,
            })
         .OrderBy(grp => grp.T_artiklName, new VvCompareStringsOrdinal());

      Rtrans rtrans_JPN25    = null;
      Rtrans rtrans_JPN13    = null;
      Rtrans rtrans_JPN05    = null;
      Rtrans rtrans_JPN25pnp = null;
      Rtrans rtrans_JPN13pnp = null;
      Rtrans rtrans_JPN05pnp = null;
      decimal pnpSt = ZXC.RRD.Dsc_PnpSt;

      if(IZMrichRtranses_JPN25.Count().NotZero()) rtrans_JPN25 = new Rtrans() 
      { T_artiklName = "Konzumacija jela i pića (PDV 25%)"      , T_kol = 1M, T_cij = IZMrichRtranses_JPN25   .Sum(r => r.R_KCRP), T_pdvSt = 25M,                  T_serial = ++serial };
      if(IZMrichRtranses_JPN13.Count().NotZero()) rtrans_JPN13 = new Rtrans()                                                                                    
      { T_artiklName = "Konzumacija jela i pića (PDV 13%)"      , T_kol = 1M, T_cij = IZMrichRtranses_JPN13   .Sum(r => r.R_KCRP), T_pdvSt = 13M,                  T_serial = ++serial };
      if(IZMrichRtranses_JPN05.Count().NotZero()) rtrans_JPN05 = new Rtrans()                                                                                    
      { T_artiklName = "Konzumacija jela i pića (PDV  5%)"      , T_kol = 1M, T_cij = IZMrichRtranses_JPN05   .Sum(r => r.R_KCRP), T_pdvSt =  5M,                  T_serial = ++serial };
      if(IZMrichRtranses_JPN25pnp.Count().NotZero()) rtrans_JPN25pnp = new Rtrans()                                                                              
      { T_artiklName = "Konzumacija jela i pića (PDV 25% i PNP)", T_kol = 1M, T_cij = IZMrichRtranses_JPN25pnp.Sum(r => r.R_KCRP), T_pdvSt = 25M, T_pnpSt = pnpSt, T_serial = ++serial };
      if(IZMrichRtranses_JPN13pnp.Count().NotZero()) rtrans_JPN13pnp = new Rtrans()                                                                              
      { T_artiklName = "Konzumacija jela i pića (PDV 13% i PNP)", T_kol = 1M, T_cij = IZMrichRtranses_JPN13pnp.Sum(r => r.R_KCRP), T_pdvSt = 13M, T_pnpSt = pnpSt, T_serial = ++serial };
      if(IZMrichRtranses_JPN05pnp.Count().NotZero()) rtrans_JPN05pnp = new Rtrans()                                                                              
      { T_artiklName = "Konzumacija jela i pića (PDV  5% i PNP)", T_kol = 1M, T_cij = IZMrichRtranses_JPN05pnp.Sum(r => r.R_KCRP), T_pdvSt =  5M, T_pnpSt = pnpSt, T_serial = ++serial };

/* A */ toFaktur_rtransList.AddRange(projekt_BOR_RtransList)                                      ;
/* B */ toFaktur_rtransList.AddRange(groupedIZMrichRtranses_NeJPN)                                ;
/* C */ 
      if(rtrans_JPN25    != null) toFaktur_rtransList.Add(rtrans_JPN25   );
      if(rtrans_JPN25pnp != null) toFaktur_rtransList.Add(rtrans_JPN25pnp);
      if(rtrans_JPN13    != null) toFaktur_rtransList.Add(rtrans_JPN13   );
      if(rtrans_JPN13pnp != null) toFaktur_rtransList.Add(rtrans_JPN13pnp);
      if(rtrans_JPN05    != null) toFaktur_rtransList.Add(rtrans_JPN05   );
      if(rtrans_JPN05pnp != null) toFaktur_rtransList.Add(rtrans_JPN05pnp);

      #region AddNewIRM Interactively (was AutoSetFaktur)

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.R_IRM_2);

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

      List<Rtrans> origBORtranses = ZXC.FakturRec.Transes;
      string origBORskladCD       = ZXC.FakturRec.SkladCD;
      string origBORprojektCD     = ZXC.FakturRec.ProjektCD;

      ZXC.FakturRec.Transes       = toFaktur_rtransList;
      ZXC.FakturRec.SkladCD       = "MPSK";
      ZXC.FakturRec.ProjektCD     = ZXC.FakturRec.TipBr;

      ZXC.RISK_CopyToOtherDUC_inProgress = true;

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      NewRecord_OnClick(ZXC.FakturRec, EventArgs.Empty);

      ZXC.FakturRec.Transes   = origBORtranses  ;
      ZXC.FakturRec.SkladCD   = origBORskladCD  ;
      ZXC.FakturRec.ProjektCD = origBORprojektCD;

#if NJETT
      ushort  line = 0;
      decimal s_ukK = 0M, s_ukKC = 0M, s_ukKCR = 0M, s_ukKCRP = 0M, s_ukRbt1 = 0M, s_ukOsn25m = 0M, s_ukPdv = 0M, s_ukPdv25m = 0M;
      int     s_trnCount = 0;
      string napomena = "";

      foreach(Rtrans rtrans in toFaktur_rtransList)
      {
         //line = 0;

         s_ukK       = toFaktur_rtransList.Sum(raw2 =>  rtrans.T_kol  );
         s_ukKC      = toFaktur_rtransList.Sum(raw2 => (rtrans.R_KC  ));
         s_ukKCR     = toFaktur_rtransList.Sum(raw2 => (rtrans.R_KCR ));
         s_ukKCRP    = toFaktur_rtransList.Sum(raw2 => (rtrans.R_KCRP));
                                                        
         s_ukRbt1    = toFaktur_rtransList.Sum(raw2 => (rtrans.R_rbt1));
         s_ukOsn25m  = s_ukKCR;                                
         s_ukPdv     = toFaktur_rtransList.Sum(raw2 => (rtrans.R_pdv ));
         s_ukPdv25m  = s_ukPdv;

         s_trnCount  = toFaktur_rtransList.Count;

         FakturDao.AutoSetFaktur(TheDbConnection, ref line,

            /* string   f_tt          ,*/ Faktur.TT_IRM                 ,
            /* string   f_ttNum       ,*/ 0                             ,
            /* DateTime f_dokDate     ,*/ theDUC.faktur_rec.DokDate     ,
            /* string   f_skladCD     ,*/ "MPSK"                        ,
            /* string   f_vezniDok    ,*/ ""                            , // Limited_Faktur(rawData.t_dokumCD , ZXC.FakCI.vezniDok  ),
            /* string   f_projektCD   ,*/ theDUC.faktur_rec.ProjektCD   ,
            /* string   f_napomena    ,*/
          VvRtransImporter.Limited_Faktur(napomena , ZXC.FakCI.napomena),
            /* decimal  s_ukZavisni   ,*/ 0M                            ,
            /* decimal  s_ukKCRP      ,*/ s_ukKCRP                      ,
            /* decimal  s_ukKCRM      ,*/ 0M                            ,
            /* decimal  s_ukKCR       ,*/ s_ukKCR                       ,
            /* decimal  s_ukKC        ,*/ s_ukKC                        ,
            /* decimal  s_ukK         ,*/ s_ukK                         ,
            /* decimal  s_ukRbt1      ,*/ s_ukRbt1                      ,
            /* decimal  s_ukOsn23m    ,*/ s_ukOsn25m                    ,
            /* decimal  s_ukPdv       ,*/ s_ukPdv                       ,
            /* decimal  s_ukPdv25m    ,*/ s_ukPdv25m                    ,
            /* decimal  s_ukMrz       ,*/ 0M                            ,
            /* decimal  s_ukMSKpdv_25 ,*/ 0M                            ,
            /* decimal  s_ukMSK_25    ,*/ 0M                            ,
            /* uint     s_trnCount    ,*/ (uint)s_trnCount              ,
            /* string   f_kupdobCD    ,*/ theDUC.faktur_rec.KupdobCD    ,
            /* string   f_kupdobName  ,*/ theDUC.faktur_rec.KupdobName  ,
            /* string   f_kupdobTicker,*/ theDUC.faktur_rec.KupdobTK    ,
    /*         string   f_kdOib       ,*/ theDUC.faktur_rec.KdOib       ,
    /*         string   f_kdUlica     ,*/ theDUC.faktur_rec.KdUlica     ,
    /*         string   f_kdMjesto    ,*/ theDUC.faktur_rec.KdMjesto    ,
    /*         string   f_kdZip       ,*/ theDUC.faktur_rec.KdZip       ,
    /*         string   f_ZiroRn      ,*/ theDUC.faktur_rec.ZiroRn      ,
    /*         string   f_Konto       ,*/ theDUC.faktur_rec.Konto       ,
    /*ZXC.PdvKnjigaEnum f_PdvKnjiga   ,*/ ZXC.PdvKnjigaEnum.REDOVNA     ,
            /* string   f_rokPlac     ,*/ theDUC.faktur_rec.RokPlac     ,
            /* string   f_dospDate    ,*/ theDUC.faktur_rec.DospDate    ,

            /* string   t_artiklCD    ,*/ rtrans.T_artiklCD             ,
            /* string   t_artiklName  ,*/ rtrans.T_artiklName           ,
            /* decimal  t_kol         ,*/ rtrans.T_kol                  ,
            /* decimal  t_cij         ,*/ rtrans.T_cij                  ,
            /* string   t_konto       ,*/ ""                            ,
            /* decimal  t_pdvSt       ,*/ rtrans.T_pdvSt                ,
            /* decimal  t_rbt1St      ,*/ rtrans.T_rbt1St               ,
            /* decimal  t_rbt2St      ,*/ rtrans.T_rbt2St               ,
            /* decimal  t_wanted      ,*/ rtrans.T_wanted               ,
  /* ZXC.MalopCalcKind  t_mCalcKind   ,*/ rtrans.T_mCalcKind            ,
  /* ZXC.PdvKolTipEnum  t_pdvKolTip   ,*/ rtrans.T_pdvColTip            ,
  /*            string  t_jedMJ       ,*/ rtrans.T_jedMj                ,
            /* decimal  t_ztr         ,*/ 0M                            ,
            /* decimal  t_doCijMal    ,*/ 0M                            ,
            /* decimal  t_noCijMal    ,*/ 0M                            ,
            /* decimal  t_pnpSt       ,*/ rtrans.T_pnpSt                );

      }
#endif
      #endregion AddNewIRM Interactively (was AutoSetFaktur)

      Cursor.Current = Cursors.Default;

   }

   private void RISK_TH_manyZPC_5week    (object sender, EventArgs e) { /*if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return;*/ RISK_TH_manyZPC(/*true */ZXC.TH_ShopWeekKind._5W    ); }
   private void RISK_TH_manyZPC_2week_PON(object sender, EventArgs e) { /*if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return;*/ RISK_TH_manyZPC(/*false*/ZXC.TH_ShopWeekKind._2W_PON); }
   private void RISK_TH_manyZPC_2week_SRI(object sender, EventArgs e) { /*if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return;*/ RISK_TH_manyZPC(/*false*/ZXC.TH_ShopWeekKind._2W_SRI); }
   private void RISK_TH_manyZPC_3week_SRI(object sender, EventArgs e) { /*if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return;*/ RISK_TH_manyZPC(/*false*/ZXC.TH_ShopWeekKind._3W_SRI); }

   private void RISK_TH_manyZPC(ZXC.TH_ShopWeekKind shopWeekKind)
   {
    //if(ZXC.VvDeploymentSite != ZXC.VektorSiteEnum.TEXTHO) return;
      if(ZXC.IsTEXTHOshop) return;

      // 07.11.2017:
      if(ZXC.IsTHcentralaDuringSkylabCacheTime) { ZXC.IssueTHcentralaDuringSkylabCacheTimeMessage(); return; }

      Cursor.Current = Cursors.WaitCursor;

      DateTime serverDateNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      // 25.09.2018: nestaje 'IsTH_2WeekShop' te se razlucije na IsTH_2Week_PON_Shop i IsTH_2Week_SRI_Shop 
    //var malop2weekSkladCDlist      = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2WeekShop     (lui.Cd)).Select(l => l.Cd);
      var malop2week_PON_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_PON_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop2week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop5weekSkladCDlist      = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_5WeekShop     (lui.Cd)).Select(l => l.Cd).ToList();
      var malop3week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_3Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();

      List<string> skladCDlist = new List<string>();
      switch(shopWeekKind)
      {
         // 31.12.2022: 
       //case ZXC.TH_ShopWeekKind._2W_PON: skladCDlist = malop2week_PON_SkladCDlist                                                        ; break;
       //case ZXC.TH_ShopWeekKind._2W_SRI: skladCDlist = malop2week_SRI_SkladCDlist                                                        ; break;
       //case ZXC.TH_ShopWeekKind._5W    : skladCDlist = malop5weekSkladCDlist                                                             ; break;
       //case ZXC.TH_ShopWeekKind._3W_SRI: skladCDlist = malop3week_SRI_SkladCDlist                                                        ; break;
         case ZXC.TH_ShopWeekKind._2W_PON: skladCDlist = malop2week_PON_SkladCDlist.Where(skl => ZXC.IsTH_DEAD_Shop(skl) == false).ToList(); break;
         case ZXC.TH_ShopWeekKind._2W_SRI: skladCDlist = malop2week_SRI_SkladCDlist.Where(skl => ZXC.IsTH_DEAD_Shop(skl) == false).ToList(); break;
         case ZXC.TH_ShopWeekKind._5W    : skladCDlist = malop5weekSkladCDlist     .Where(skl => ZXC.IsTH_DEAD_Shop(skl) == false).ToList(); break;
         case ZXC.TH_ShopWeekKind._3W_SRI: skladCDlist = malop3week_SRI_SkladCDlist.Where(skl => ZXC.IsTH_DEAD_Shop(skl) == false).ToList(); break;
      }

      //DateTime nextZPCDate = ZXC.TH_GetNextZPCDate(serverDateNow);
      DateTime nextZPCDate =                      (serverDateNow.Date);
      DateTime wantedZPCDate, dokDateZPC;
      string statusText;

      Create_manyZPCDlg dlg = new Create_manyZPCDlg(nextZPCDate, /*is5week*/shopWeekKind);

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      nextZPCDate   = dlg.TheUC.Fld_nextZPCDate  .Date;
      wantedZPCDate = dlg.TheUC.Fld_wantedZPCDate.Date;

      if(wantedZPCDate.IsEmpty()) dokDateZPC = nextZPCDate  ;
      else                        dokDateZPC = wantedZPCDate;

      string onlyForSkladCD = dlg.TheUC.Fld_skladCD;

      dlg.Dispose();

      if(VvDaoBase.IsDocumentFromLockedPeriod(dokDateZPC.Date, false)) return;

      #region test
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 27), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 27), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 28), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 28), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 29), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 29), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 30), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 30), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 31), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 10, 31), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 01), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 01), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 02), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 02), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 03), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 03), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 04), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 04), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 05), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 05), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 06), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 06), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 07), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 07), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 08), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 08), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 09), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 09), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 10), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 10), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 11), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 11), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 12), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 12), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 13), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 13), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 14), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 14), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 15), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 15), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 16), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 16), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 17), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 17), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 18), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 18), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 19), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 19), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 20), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 20), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 21), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 21), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 22), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 22), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 23), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 23), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 24), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 24), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 25), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 25), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 26), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 26), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 27), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 27), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 28), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 28), "36M2");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 29), "34M5");
      //nextZPCDate = ZXC.TH_GetNextZPCDate(new DateTime(2014, 11, 29), "36M2");
      #endregion test

      int newZpcRtransListCount;
      int fakturCount = 0;
      int transCount  = 0;

      //Faktur faktur_rec;

      ZXC.SetStatusText("RISK_TH_manyZPC START");
      ZXC.aim_log      ("RISK_TH_manyZPC START");

    //foreach(string malopSkladCD in (is5week ? malop5weekSkladCDlist : malop2weekSkladCDlist))
      foreach(string malopSkladCD in skladCDlist)
      {
         // 31.12.2022: 
       //if(ZXC.IsTH_DEAD_Shop(malopSkladCD)) continue;

         // 10.07.2015: 
         if(onlyForSkladCD.NotEmpty() && onlyForSkladCD != malopSkladCD) continue;

         // THPR news: 
       //newZpcRtransListCount = TH_CreateZPC(nextZPCDate, malopSkladCD, is5week, dokDateZPC, false       );
         newZpcRtransListCount = TH_CreateZPC(nextZPCDate, malopSkladCD,          dokDateZPC, false, false);

         if(newZpcRtransListCount.NotZero())
         {
            fakturCount++;
            transCount += newZpcRtransListCount;
         }

         // ipak NE 
       //if(ZXC.FakturRec != null) // ZXC.FakturRec nastane u AutoAddFaktur 
       //{
       //   ZXC.FakturRec.VvDao.LoadTranses (TheDbConnection, ZXC.FakturRec, false);
       //   ZXC.FakturRec.VvDao.LoadExtender(TheDbConnection, ZXC.FakturRec, false);
       //
       //   VvDaoBase.SendWriteOperationToSKY(TheDbConnection, ZXC.FakturRec, VvSQL.DB_RW_ActionType.ADD, false);
       //}

       //statusText = String.Format("Done {0} / {1} [{2}]", fakturCount, (is5week ? malop5weekSkladCDlist : malop2weekSkladCDlist).Count(), malopSkladCD);
         statusText = String.Format("Done {0} / {1} [{2}]", fakturCount,                                               skladCDlist.Count(), malopSkladCD);
         ZXC.SetStatusText(statusText); if(Cursor.Current != Cursors.WaitCursor) Cursor.Current = Cursors.WaitCursor;
         ZXC.aim_log(statusText);
      }

      Cursor.Current = Cursors.Default;

      ZXC.ClearStatusText();

      TheVvRecordUC.ThePrefferedRecordSorter = Faktur.sorterDokDate;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao\n\n{0} ZPCa.\n\nUkupno {1} novih stavaka", fakturCount, transCount);
   }

#if TH_CreateZPC_OLD

   internal int TH_CreateZPC(DateTime nextZPCDate, string malopSkladCD, bool is5week, DateTime dokDateZPC, bool isChkOnly)
   {
      Cursor.Current = Cursors.WaitCursor;

    //bool is5week = ZXC.IsTH_5Week(malopSkladCD);
      bool is2week = !is5week;

      ZXC.TH_CycleMoment thCycleMoment = ZXC.Get_TH_CycleMoment(nextZPCDate, is5week, malopSkladCD);

   #region Get NULTI ZPC for this sklad - PUSE

      uint nultiZpcTtNum        = 0;
      Faktur nultiZpcFaktur_rec = null;

      if(thCycleMoment == ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_1___regular     ||
         thCycleMoment == ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_1___regular     ||
         thCycleMoment == ZXC.TH_CycleMoment.W5_Tjedan_1___10_posto_za_letak ||
         thCycleMoment == ZXC.TH_CycleMoment.W6_Tjedan_1___10_posto_za_letak   ) // time to RESET all prices 
      {
         nultiZpcTtNum = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(malopSkladCD) * 10000;
         
         nultiZpcFaktur_rec = new Faktur();
         
         bool OK = FakturDao.SetMeFaktur(TheDbConnection, nultiZpcFaktur_rec, Faktur.TT_ZPC, nultiZpcTtNum, false);
         
         if(!OK) { ZXC.aim_emsg("Nema NULTOG ZPC-a za skald [{0}] ttNum [{1}]", malopSkladCD, nultiZpcTtNum); return 0; } // !!! 
         
         nultiZpcFaktur_rec.VvDao.LoadTranses(TheDbConnection, nultiZpcFaktur_rec, false);
      }

      #endregion Get NULTI ZPC fro this sklad

   #region Init Stuff

      DataRowCollection   ArsSch = ZXC.ArtStatSchemaRows; 
      ArtStatDao.ArtStatCI AstCI = ZXC.AstCI            ;
      List<Artikl> theArtiklWithArtstatList = new List<Artikl>();
      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

      // 06.08.2015: !!! 
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.t_skladDate] , false, "DateDO", nextZPCDate        , "", "", " <  ", ""));
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.artGrCd3   ] , false, "Gr3CD" , Artikl.ProdRobaGrCD, "", "", " =  ", ""));
    //ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, malopSkladCD, nextZPCDate, rptFilter, "", "artiklCD "    );
      // 06.08.2015: !!!!!! jos kasnije u danu, a NAKON deployanja izbacio fm DateDO jer se duplira (u inner joinu vec ide jemput)
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.t_skladDate] , false, "DateDO", dokDateZPC         , "", "", " <  ", ""));
      rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.artGrCd3   ] , false, "Gr3CD" , Artikl.ProdRobaGrCD, "", "", "  = ", ""));
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, malopSkladCD, dokDateZPC, rptFilter , "", "artiklCD "    );

      // 07.07.2015: 
      if(nextZPCDate >= ZXC.TexthoNewArtiklsDate) theArtiklWithArtstatList.RemoveAll(art => art.ArtiklCD.Length <  4); // izbaci stare artikle 
      else                                        theArtiklWithArtstatList.RemoveAll(art => art.ArtiklCD.Length >= 4); // izbaci nove  artikle 

      #endregion Init Stuff

   #region Set RtransList

      Rtrans newZpcRtrans_rec, oldNultiZpcRtrans_rec;
      List<Rtrans> newZpcRtransList = new List<Rtrans>(theArtiklWithArtstatList.Count);
    //Rtrans[] newZpcRtransList = new Rtrans[theArtiklWithArtstatList.Count];

      string artGR;
      decimal oldMalopCij, newMalopCij;

      int i = 0;
      foreach(Artikl artWast_rec in theArtiklWithArtstatList)
      {
         oldMalopCij = artWast_rec.AS_KnjigCij;
         newMalopCij = oldMalopCij            ;
         artGR       = artWast_rec.AS_ArtGrCd2;

   #region Set newMalopCij ... VOILA!!! ... OLD ORIG

       //switch(thCycleMoment)
       //{
       //   case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_123__do_30kn               :
       //   case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_1_2___do_30kn              : // postavi cijenu kao na nultom ZPC-u, tj. RESET cijene 
       //   case ZXC.TH_CycleMoment.W5_Tjedan_1___10_posto_za_letak            : oldNultiZpcRtrans_rec = nultiZpcFaktur_rec.Transes.FirstOrDefault(rtr => rtr.T_artiklCD == artWast_rec.ArtiklCD);
       //                                                                        if(oldNultiZpcRtrans_rec != null) newMalopCij = oldNultiZpcRtrans_rec.T_noCijMal;
       //                                                                        else                              newMalopCij = 0M;
       //                                                                        break; 
       //   case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_1_2___do_30kn_excl_50_posto: if(artGR == "ODE")         newMalopCij = oldMalopCij / 2M; // Exclusive artikli na 50% 
       //                                                                        else if(oldMalopCij > 30M) newMalopCij = 30M             ; // Šišaj cijene na max 30   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_45___do_25kn_excl_50_posto :
       //   case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_3_4___do_25kn_excl_50_posto:
       //   case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_3_4___do_25kn_excl_50_posto: if(artGR != "ODE" && oldMalopCij > 25M) newMalopCij = 25M; // Šišaj cijene na max 25   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_5_6___do_20kn_excl_50_posto:
       //   case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_5_6___do_20kn_excl_50_posto: if(artGR != "ODE" && oldMalopCij > 20M) newMalopCij = 20M; // Šišaj cijene na max 20   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_4____do_15kn                       :
       //   case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_567__do_15kn_happy_hour_33_33_posto:
       //   case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_1_2___do_15kn              :
       //   case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_1_2___do_15kn              : if(oldMalopCij > 15M) newMalopCij = 15M; // Šišaj SVE cijene na max 15   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_1____do_10kn                    :
       //   case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_23___do_10kn_happy_hour_50_posto:
       //   case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_3_4___do_10kn              :
       //   case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_3_4___do_10kn              : if(oldMalopCij > 10M) newMalopCij = 10M; // Šišaj SVE cijene na max 10   
       //                                                                        break;
       //
       // //case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_4567_do_5kn                :
       // //case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_5_6___do__5kn              :
       // //case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_5_6___do__5kn              :
       //   case ZXC.TH_CycleMoment.NEDEFINIRAN_CycleMoment                    : if(oldMalopCij >  7M) newMalopCij =  7M; // Šišaj SVE cijene na max  7   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_4567_do_5kn                :
       //   case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_5_6___do__5kn              :
       //   case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_5_6___do__5kn              : if(oldMalopCij >  5M) newMalopCij =  5M; // Šišaj SVE cijene na max  5   
       //                                                                        break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_67___do_25kn_happy_hour_20_posto: if(oldMalopCij > 25M) newMalopCij = 25M; // Šišaj cijene na max 25   
       //                                                                             break;
       //
       //   case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_1____do_20kn                    : 
       //   case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_23___do_20kn_happy_hour_25_posto: if(oldMalopCij > 20M) newMalopCij = 20M; // Šišaj cijene na max 20   
       //                                                                             break;
       //}                                                                            

         #endregion Set newMalopCij

   #region Set newMalopCij ... VOILA!!! ...

         switch(thCycleMoment)
         {
            case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_1___regular               : // postavi cijenu kao na nultom ZPC-u, tj. RESET cijene 
            case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_1___regular               :
            case ZXC.TH_CycleMoment.W5_Tjedan_1___10_posto_za_letak           :
            case ZXC.TH_CycleMoment.W6_Tjedan_1___10_posto_za_letak           : oldNultiZpcRtrans_rec = nultiZpcFaktur_rec.Transes.FirstOrDefault(rtr => rtr.T_artiklCD == artWast_rec.ArtiklCD);
                                                                                if(oldNultiZpcRtrans_rec != null) newMalopCij = oldNultiZpcRtrans_rec.T_noCijMal;
                                                                                else                              newMalopCij = 0M;
                                                                                break;

            // 13.11.2017: 
          //case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_1_2__do_30kn_excl_50_posto: if(artGR == "ODE")         newMalopCij =                           oldMalopCij / 2M; // Exclusive artikli na 50% 
            case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_1_2_do_30kn_excl_50_posto :
            case ZXC.TH_CycleMoment.W6_Tjedan_5_Dan_1_2_do_30kn_excl_50_posto : if(artGR == "ODE")         newMalopCij = isChkOnly ? oldMalopCij : oldMalopCij / 2M; // Exclusive artikli na 50% 
                                                                                else if(oldMalopCij > 30M) newMalopCij = 30M                                       ; // Šišaj cijene na max 30   
                                                                                break;

            case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_3_4_do_25kn_excl_50_posto :
            case ZXC.TH_CycleMoment.W6_Tjedan_5_Dan_3_4_do_25kn_excl_50_posto : if(artGR != "ODE" && oldMalopCij > 25M) newMalopCij = 25M; // Šišaj cijene na max 25 + Exc50 po W% Tj4 dan1  
                                                                                break;

            case ZXC.TH_CycleMoment.W5_Tjedan_4_Dan_5_6_do_20kn_excl_50_posto :
            case ZXC.TH_CycleMoment.W6_Tjedan_5_Dan_567_do_20kn_excl_50_posto : if(artGR != "ODE" && oldMalopCij > 20M) newMalopCij = 20M; // Šišaj cijene na max 20 + Exc50 po W% Tj4 dan1
                                                                                break;

            case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_1_2_do_15kn_excl_50_posto : if(artGR != "ODE" && oldMalopCij > 15M) newMalopCij = 15M; // Šišaj cijene na max 15 + Exc50 po W% Tj4 dan1
                                                                                break;
                                                                                 
            case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_2___do_30kn               :
            case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_2__do_30kn                :  if(oldMalopCij > 30M) newMalopCij = 30M                  ; // Šišaj SVE cijene na max 30 
                                                                                 break;
                                                                              
            case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_3_4_do_25kn               :
            case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_34_do_25kn                :  if(oldMalopCij > 25M) newMalopCij = 25M                  ; // Šišaj SVE cijene na max 25 
                                                                                 break;

            case ZXC.TH_CycleMoment.W2_Tjedan_1_Dan_5_6_do_20kn:
            case ZXC.TH_CycleMoment.W3_Tjedan_1_Dan_56_do_20kn :
            case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan__3_do_20kn                :  if(oldMalopCij > 20M) newMalopCij = 20M                  ; // Šišaj SVE cijene na max 20   
                                                                                 break;
                                                                              
            case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_1_2_do_15kn               :
            case ZXC.TH_CycleMoment.W3_Tjedan_2_Dan_45_do_15kn                :
            case ZXC.TH_CycleMoment.W6_Tjedan_6_Dan_1i2_do_15kn               : if(oldMalopCij > 15M) newMalopCij = 15M                   ; // Šišaj SVE cijene na max 15   
                                                                                break;

            case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_3___do_10kn               :
            case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_2__do_10kn                : 
            case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_3___do_10kn               :
            case ZXC.TH_CycleMoment.W6_Tjedan_6_Dan_3___do_10kn               : if(oldMalopCij > 10M) newMalopCij = 10M                   ; // Šišaj SVE cijene na max 10   
                                                                                break;                                                    
                                                                                                                                          
            case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_4___do__7kn               :                                                           
            case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_3__do_7kn                 :                                                           
            case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_4___do__7kn               :                                                           
            case ZXC.TH_CycleMoment.W6_Tjedan_6_Dan_4___do__7kn               : if(oldMalopCij >  7M) newMalopCij =  7M                   ; // Šišaj SVE cijene na max  7   
                                                                                break;                                                    
                                                                                                                                           
            case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_5___do__5kn               :                                                           
            case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_4__do_5kn                 :                                                           
            case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_5___do__5kn               :                                                           
            case ZXC.TH_CycleMoment.W6_Tjedan_6_Dan_5___do__5kn               : if(oldMalopCij >  5M) newMalopCij =  5M                   ; // Šišaj SVE cijene na max  5   
                                                                                break;                                                    
                                                                                                                                          
            case ZXC.TH_CycleMoment.W2_Tjedan_2_Dan_6___do__3kn               :                                                                          
            case ZXC.TH_CycleMoment.W3_Tjedan_3_Dan_5__do_3kn                 :                                                           
            case ZXC.TH_CycleMoment.W5_Tjedan_5_Dan_6___do__3kn               :                                                           
            case ZXC.TH_CycleMoment.W6_Tjedan_6_Dan_6___do__3kn               : if(oldMalopCij >  3M) newMalopCij =  3M                   ; // Šišaj SVE cijene na max  3   
                                                                                break;                                                     

         }                                                                            

         #endregion Set newMalopCij

         // !!! !!! !!! 
         if(ZXC.AlmostEqual(oldMalopCij, newMalopCij, 0.02M)) continue; // well, NEMOJ ZPCovati one s nepromijenjenom cijenom 
         // !!! !!! !!! 

   #region Set newZpcRtrans_rec data

         newZpcRtrans_rec = new Rtrans();

         newZpcRtrans_rec.T_TT         = Faktur.TT_ZPC                             ;
       //newZpcRtrans_rec.T_skladDate  = nextZPCDate                               ;
         newZpcRtrans_rec.T_skladDate  = dokDateZPC                                ;
         newZpcRtrans_rec.T_artiklCD   = artWast_rec.ArtiklCD                      ;
         newZpcRtrans_rec.T_skladCD    = malopSkladCD                              ;
         newZpcRtrans_rec.T_artiklName = artWast_rec.ArtiklName                    ;
         newZpcRtrans_rec.T_jedMj      = artWast_rec.JedMj                         ;
         newZpcRtrans_rec.T_kol        = artWast_rec.AS_StanjeKol                  ;   
         newZpcRtrans_rec.T_cij        = artWast_rec.AS_PrNabCij                   ;   
         newZpcRtrans_rec.T_pdvSt      = Faktur.CommonPdvStForThisDate(dokDateZPC) ;   
         newZpcRtrans_rec.T_wanted     = newMalopCij                               ;   
         newZpcRtrans_rec.T_doCijMal   = oldMalopCij                               ;   
         newZpcRtrans_rec.T_noCijMal   = newMalopCij                               ;   
         newZpcRtrans_rec.T_mCalcKind  = ZXC.MalopCalcKind.By_MPC                  ;

         #endregion Set newZpcRtrans_rec data

         newZpcRtrans_rec.CalcTransResults(null);

         newZpcRtransList.Add(newZpcRtrans_rec);
       //newZpcRtransList[i++] = newZpcRtrans_rec;

      }

      #endregion Set RtransList

   #region Set faktur_rec

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      faktur_rec.TT = Faktur.TT_ZPC;
      faktur_rec.Napomena = thCycleMoment.ToString();

      faktur_rec.SkladDate = // !!! 
      faktur_rec.DokDate   = /*nextZPCDate*/ dokDateZPC;
      faktur_rec.SkladCD   = malopSkladCD;

      faktur_rec.S_ukK        =       newZpcRtransList.Sum(rtr => rtr.T_kol   );
      faktur_rec.S_ukK2       =       newZpcRtransList.Sum(rtr => rtr.T_kol2  );
      faktur_rec.S_ukKC       =       newZpcRtransList.Sum(rtr => rtr.R_KC    );
      faktur_rec.S_ukRbt1     =       newZpcRtransList.Sum(rtr => rtr.R_rbt1  );
      faktur_rec.S_ukKCR      =       newZpcRtransList.Sum(rtr => rtr.R_KCR   );
      faktur_rec.S_ukKCRM     =       newZpcRtransList.Sum(rtr => rtr.R_KCRM  );
      faktur_rec.S_ukKCRP     =       newZpcRtransList.Sum(rtr => rtr.R_KCRP  );
    //faktur_rec.S_ukMskPdv   =       newZpcRtransList.Sum(rtr => rtr.R_mskPdv);
    //faktur_rec.S_ukMSK      =       newZpcRtransList.Sum(rtr => rtr.R_MSK   );
      faktur_rec.S_ukTrnCount = (uint)newZpcRtransList.Count();

      faktur_rec.S_ukPdv25m   = /* faktur_rec.TrnSum_Pdv25m  ; */ newZpcRtransList.Sum(rtrn => rtrn.R_pdv).Ron2();
      faktur_rec.S_ukOsn25m   = /* faktur_rec.TrnSum_Osn25m  ; */ newZpcRtransList.Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR));
      faktur_rec.S_ukMrz      = /* faktur_rec.TrnSum_Mrz     ; */ newZpcRtransList.Sum(rtrn => rtrn.R_mrz);
      faktur_rec.S_ukPdv      = /* faktur_rec.TrnSum_Pdv     ; */ newZpcRtransList.Sum(rtrn => rtrn.R_pdv).Ron2();
      faktur_rec.S_ukMskPdv25 = /* faktur_rec.TrnSum_MskPdv25; */ newZpcRtransList.Sum(rtrn => rtrn.R_mskPdv25); 
      faktur_rec.S_ukMSK_25   = /* faktur_rec.TrnSum_MSK_25  ; */ newZpcRtransList.Sum(rtrn => rtrn.R_MSK_25  );
    //faktur_rec.K_NivVrj25   = /* faktur_rec.TrnSum_        ; */ newZpcRtransList.Sum(rtr => rtr.R_NivVrj25  );

      #endregion Set faktur_rec

   #region foreach Rtrans AutoSetFaktur

      foreach(Rtrans rtrans in newZpcRtransList)
      {
         // 08.11.2017: dodan if 
         if(isChkOnly)
         {
          //ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA! [{3}]\tima krivu MPC ili nedostaje na ovom dokumentu.\n\nMPC\t{0}\ntreba biti\t{1}\n\nSklad:\t{2}\nDatum:\t{4}",
          //   rtrans.T_doCijMal.ToStringVv(), rtrans.T_noCijMal.ToStringVv(), rtrans.T_skladCD, rtrans.T_artiklCD, rtrans.T_skladDate.ToString(ZXC.VvDateFormat));
            ZXC.aim_log("GREŠKA![{3}]\tima krivu MPC ili nedostaje na ZPC. MPC\t{0} treba biti\t{1} Sklad:\t{2} Datum:\t{4}",
               rtrans.T_doCijMal.ToStringVv(), rtrans.T_noCijMal.ToStringVv(), rtrans.T_skladCD, rtrans.T_artiklCD, rtrans.T_skladDate.ToString(ZXC.VvDateFormat));
         }
         else
         {
            FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans); // old, orig 
         }
      }

      #endregion foreach Rtrans AutoSetFaktur

      return newZpcRtransList.Count;

#if nonono

   #region Set faktur_rec

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      faktur_rec.TT      = Faktur.TT_ZPC;
      faktur_rec.TtNum   = faktur_rec.VvDao.GetNextTtNum(TheDbConnection, Faktur.TT_ZPC, malopSkladCD);
      faktur_rec.DokDate = nextZPCDate;
      faktur_rec.SkladCD = malopSkladCD;

      faktur_rec.S_ukK        =       newZpcRtransList.Sum(rtr => rtr.T_kol   );
      faktur_rec.S_ukKC       =       newZpcRtransList.Sum(rtr => rtr.R_KC    );
      faktur_rec.S_ukRbt1     =       newZpcRtransList.Sum(rtr => rtr.R_rbt1  );
      faktur_rec.S_ukKCR      =       newZpcRtransList.Sum(rtr => rtr.R_KCR   );
      faktur_rec.S_ukKCRM     =       newZpcRtransList.Sum(rtr => rtr.R_KCRM  );
      faktur_rec.S_ukKCRP     =       newZpcRtransList.Sum(rtr => rtr.R_KCRP  );
    //faktur_rec.S_ukMskPdv   =       rtransList.Sum(rtr => rtr.R_mskPdv);
    //faktur_rec.S_ukMSK      =       rtransList.Sum(rtr => rtr.R_MSK   );
      faktur_rec.S_ukTrnCount = (uint)newZpcRtransList.Count();


      #endregion Set faktur_rec

   #region foreach Rtrans AutoSetFaktur

      foreach(Rtrans rtrans in newZpcRtransList)
      {
         FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans);
      }

      #endregion foreach Rtrans AutoSetFaktur
#endif
   }
#endif

   internal int TH_CreateZPC/*_NEW*/(DateTime nextZPCDate, string malopSkladCD, DateTime dokDateZPC, bool isChkOnly, bool isNew_EURO_NultiZPC_Genesis)
   {
      Cursor.Current = Cursors.WaitCursor;

      TH_PriceRuleForCycleMoment theTHPR = null;

      #region Get theArtiklWithArtstatList

      DataRowCollection   ArsSch = ZXC.ArtStatSchemaRows; 
      ArtStatDao.ArtStatCI AstCI = ZXC.AstCI            ;
      List<Artikl> theArtiklWithArtstatList = new List<Artikl>();
      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

      // 06.08.2015: !!! 
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.t_skladDate] , false, "DateDO", nextZPCDate        , "", "", " <  ", ""));
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.artGrCd3   ] , false, "Gr3CD" , Artikl.ProdRobaGrCD, "", "", " =  ", ""));
    //ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, malopSkladCD, nextZPCDate, rptFilter, "", "artiklCD "    );
      // 06.08.2015: !!!!!! jos kasnije u danu, a NAKON deployanja izbacio fm DateDO jer se duplira (u inner joinu vec ide jemput)
    //rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.t_skladDate] , false, "DateDO", dokDateZPC         , "", "", " <  ", ""));

      if(isNew_EURO_NultiZPC_Genesis)
      {
         theArtiklWithArtstatList = VvUserControl.ArtiklSifrar.Where(art => art.MadeIn == VvForm.artMadeIn_EUR).OrderBy(art => art.ArtiklCD).ToList();

         if(ZXC.IsTH_3Week_SRI_Shop(malopSkladCD)) // izbaci visak 
         {
            theArtiklWithArtstatList.RemoveAll(art => art.Grupa1CD == "Akat");
         }

      }
      else // classic 
      { 
         rptFilter.FilterMembers.Add(new VvSqlFilterMember(ArsSch[AstCI.artGrCd3   ] , false, "Gr3CD" , Artikl.ProdRobaGrCD, "", "", "  = ", ""));

         ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, malopSkladCD, dokDateZPC, rptFilter, "", "artiklCD ");
      }

      // 07.07.2015: 
      if(nextZPCDate >= ZXC.TexthoNewArtiklsDate) theArtiklWithArtstatList.RemoveAll(art => art.ArtiklCD.Length <  4); // izbaci stare artikle 
      else                                        theArtiklWithArtstatList.RemoveAll(art => art.ArtiklCD.Length >= 4); // izbaci nove  artikle 

      #endregion Get theArtiklWithArtstatList

      #region Set RtransList

      Rtrans newZpcRtrans_rec;
      List<Rtrans> newZpcRtransList = new List<Rtrans>(theArtiklWithArtstatList.Count);

      decimal oldMalopCij, newMalopCij;

      if(isNew_EURO_NultiZPC_Genesis)
      {
      }
      else // classic 
      {
         theTHPR = TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(malopSkladCD, nextZPCDate);
      }

      #region TimeForNultiZPCprice

      Faktur nultiZpcFaktur_rec = null;

      // 15.03.2021: uvijek idi po nultu ZPC 
    //// if needed, Get nultiZpcFaktur_rec for current malopSkladCD 
    //if(theTHPR.IsTimeForNultiZPCprice)
      if(!isNew_EURO_NultiZPC_Genesis)
      {
         nultiZpcFaktur_rec = new Faktur();

         uint nultiZpcTtNum = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(malopSkladCD) * 10000;

         bool OK = FakturDao.SetMeFaktur(TheDbConnection, nultiZpcFaktur_rec, Faktur.TT_ZPC, nultiZpcTtNum, false);

         if(!OK) { ZXC.aim_emsg("Nema NULTOG ZPC-a za skald [{0}] ttNum [{1}]", malopSkladCD, nultiZpcTtNum); return newZpcRtransList.Count; } // !!! 

         nultiZpcFaktur_rec.VvDao.LoadTranses(TheDbConnection, nultiZpcFaktur_rec, false);
      }

      #endregion TimeForNultiZPCprice

      foreach(Artikl artWast_rec in theArtiklWithArtstatList)
      {
         if(isNew_EURO_NultiZPC_Genesis)
         {
            oldMalopCij = 0M;

            newMalopCij = artWast_rec.ImportCij;
         }
         else
         {
            oldMalopCij = artWast_rec.AS_KnjigCij;

            // 04.6.2018: novoprepoznati NERIJESENI problem u 'Get_ZPC_NewMalopCij': neki put ZPC treba DIGNUTI a ne spustiti cijene; 
            newMalopCij = TH_PriceRuleForCycleMoment.Get_ZPC_NewMalopCij(TheDbConnection, theTHPR, artWast_rec, nultiZpcFaktur_rec, isChkOnly);
         }

         // 31.12.2022: 
       //if(ZXC.AlmostEqual(oldMalopCij, newMalopCij, 0.02M )) continue; // well, NEMOJ ZPCovati one s nepromijenjenom cijenom 
         if(ZXC.AlmostEqual(oldMalopCij, newMalopCij, 0.005M)) continue; // well, NEMOJ ZPCovati one s nepromijenjenom cijenom 

         #region Set newZpcRtrans_rec data

         newZpcRtrans_rec = new Rtrans();

         newZpcRtrans_rec.T_TT         = Faktur.TT_ZPC                             ;
       //newZpcRtrans_rec.T_skladDate  = nextZPCDate                               ;
         newZpcRtrans_rec.T_skladDate  = dokDateZPC                                ;
         newZpcRtrans_rec.T_artiklCD   = artWast_rec.ArtiklCD                      ;
         newZpcRtrans_rec.T_skladCD    = malopSkladCD                              ;
         newZpcRtrans_rec.T_artiklName = artWast_rec.ArtiklName                    ;
         newZpcRtrans_rec.T_jedMj      = artWast_rec.JedMj                         ;
         newZpcRtrans_rec.T_kol        = artWast_rec.AS_StanjeKol                  ;   
         newZpcRtrans_rec.T_cij        = artWast_rec.AS_PrNabCij                   ;   
         newZpcRtrans_rec.T_pdvSt      = Faktur.CommonPdvStForThisDate(dokDateZPC) ;   
         newZpcRtrans_rec.T_wanted     = newMalopCij                               ;   
         newZpcRtrans_rec.T_doCijMal   = oldMalopCij                               ;   
         newZpcRtrans_rec.T_noCijMal   = newMalopCij                               ;   
         newZpcRtrans_rec.T_mCalcKind  = ZXC.MalopCalcKind.By_MPC                  ;

         #endregion Set newZpcRtrans_rec data

         newZpcRtrans_rec.CalcTransResults(null);

         newZpcRtransList.Add(newZpcRtrans_rec);
       //newZpcRtransList[i++] = newZpcRtrans_rec;

      }

      #endregion Set RtransList

      #region Set faktur_rec

      Faktur faktur_rec = new Faktur();
      ushort line = 0;

      if(isNew_EURO_NultiZPC_Genesis)
      {
         faktur_rec.TtNum = (uint)ZXC.luiListaSkladista.GetIntegerForThisCd(malopSkladCD) * 10000;
      }

      faktur_rec.TT = Faktur.TT_ZPC;
    //faktur_rec.Napomena = thCycleMoment.ToString();
      faktur_rec.Napomena = isNew_EURO_NultiZPC_Genesis ? "Nulti ZPC" : theTHPR.Opis;

      faktur_rec.SkladDate = // !!! 
      faktur_rec.DokDate   = /*nextZPCDate*/ dokDateZPC;
      faktur_rec.SkladCD   = malopSkladCD;

      faktur_rec.S_ukK        =       newZpcRtransList.Sum(rtr => rtr.T_kol   );
      faktur_rec.S_ukK2       =       newZpcRtransList.Sum(rtr => rtr.T_kol2  );
      faktur_rec.S_ukKC       =       newZpcRtransList.Sum(rtr => rtr.R_KC    );
      faktur_rec.S_ukRbt1     =       newZpcRtransList.Sum(rtr => rtr.R_rbt1  );
      faktur_rec.S_ukKCR      =       newZpcRtransList.Sum(rtr => rtr.R_KCR   );
      faktur_rec.S_ukKCRM     =       newZpcRtransList.Sum(rtr => rtr.R_KCRM  );
      faktur_rec.S_ukKCRP     =       newZpcRtransList.Sum(rtr => rtr.R_KCRP  );
    //faktur_rec.S_ukMskPdv   =       newZpcRtransList.Sum(rtr => rtr.R_mskPdv);
    //faktur_rec.S_ukMSK      =       newZpcRtransList.Sum(rtr => rtr.R_MSK   );
      faktur_rec.S_ukTrnCount = (uint)newZpcRtransList.Count();

      faktur_rec.S_ukPdv25m   = /* faktur_rec.TrnSum_Pdv25m  ; */ newZpcRtransList.Sum(rtrn => rtrn.R_pdv).Ron2();
      faktur_rec.S_ukOsn25m   = /* faktur_rec.TrnSum_Osn25m  ; */ newZpcRtransList.Sum(rtrn => (rtrn.TtInfo.IsMalopTT ? rtrn.R_PdvOsn : rtrn.R_KCR));
      faktur_rec.S_ukMrz      = /* faktur_rec.TrnSum_Mrz     ; */ newZpcRtransList.Sum(rtrn => rtrn.R_mrz);
      faktur_rec.S_ukPdv      = /* faktur_rec.TrnSum_Pdv     ; */ newZpcRtransList.Sum(rtrn => rtrn.R_pdv).Ron2();
      faktur_rec.S_ukMskPdv25 = /* faktur_rec.TrnSum_MskPdv25; */ newZpcRtransList.Sum(rtrn => rtrn.R_mskPdv25); 
      faktur_rec.S_ukMSK_25   = /* faktur_rec.TrnSum_MSK_25  ; */ newZpcRtransList.Sum(rtrn => rtrn.R_MSK_25  );
    //faktur_rec.K_NivVrj25   = /* faktur_rec.TrnSum_        ; */ newZpcRtransList.Sum(rtr => rtr.R_NivVrj25  );

      #endregion Set faktur_rec

      #region foreach Rtrans AutoSetFaktur

      foreach(Rtrans rtrans in newZpcRtransList)
      {
         // 08.11.2017: dodan if 
         if(isChkOnly)
         {
          //ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA! [{3}]\tima krivu MPC ili nedostaje na ovom dokumentu.\n\nMPC\t{0}\ntreba biti\t{1}\n\nSklad:\t{2}\nDatum:\t{4}",
          //   rtrans.T_doCijMal.ToStringVv(), rtrans.T_noCijMal.ToStringVv(), rtrans.T_skladCD, rtrans.T_artiklCD, rtrans.T_skladDate.ToString(ZXC.VvDateFormat));
            ZXC.aim_log("GREŠKA![{3}]\tima krivu MPC ili nedostaje na ZPC. MPC\t{0} treba biti\t{1} Sklad:\t{2} Datum:\t{4}",
               rtrans.T_doCijMal.ToStringVv(), rtrans.T_noCijMal.ToStringVv(), rtrans.T_skladCD, rtrans.T_artiklCD, rtrans.T_skladDate.ToString(ZXC.VvDateFormat));
         }
         else
         {
            FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans); // old, orig 
         }
      }

      #endregion foreach Rtrans AutoSetFaktur

      return newZpcRtransList.Count;
   }

   private void RISK_AddTRIartikle(object sender, EventArgs e)
   {
      #region Init Stuff

      FakturDUC theDUC = TheVvDocumentRecordUC as TransformDUC;
      
      if(theDUC.TheG.CurrentRow == null) return;

      int currRowIdx  = theDUC.TheG.CurrentRow.Index;
      int firstRowIdx = 0                           ;

      Rtrans firstLineRtrans_rec = (Rtrans)theDUC.GetDgvLineFields1(/*currRowIdx*/firstRowIdx, false, null);

      if(firstLineRtrans_rec.T_artiklCD.IsEmpty()) return;

      Artikl firstLineArtikl_rec = theDUC.Get_Artikl_FromVvUcSifrar(firstLineRtrans_rec.T_artiklCD);

      if(firstLineArtikl_rec == null) return;

      if(firstLineArtikl_rec.Grupa3CD != Artikl.NabRobaGrCD /*"NBKG"*/)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Artikl u prvom retku nije vreća!\n\r{0}", firstLineArtikl_rec);
         return;
      }

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      List<string> bagArtiklPreffixList = VvSkyLab.Get_TH_BagArtiklPreffixList(firstLineArtikl_rec.ArtiklCD);

      List<Artikl> forThisBag_artiklList = VvUserControl.ArtiklSifrar.Where(art => bagArtiklPreffixList.Contains(art.Placement)).ToList();

      ArtStat artStatTRM_rec;

      bool noGoodMPC;

      Rtrans zeleniTRMrtrans_rec;

      decimal theMPC;

      #endregion Init Stuff

      #region Put, Calc, ... DgvFields

      foreach(Artikl artikl in forThisBag_artiklList)
      {
         theDUC.TheG.Rows.Add();
         
         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         #region artstat

         artStatTRM_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, artikl.ArtiklCD, theDUC.Fld_SkladCD2, theDUC.Fld_DokDate);

         noGoodMPC = false;
         theMPC = 0M;

         if(artStatTRM_rec != null)
         {
            //theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij_MSK, rowIdx, artStatTRM_rec.LastUlazMPC);

            theMPC = artStatTRM_rec.LastUlazMPC;

            if(theMPC.IsZero()) noGoodMPC = true;
         }
         else noGoodMPC = true;

         if(noGoodMPC) ZXC.aim_emsg(MessageBoxIcon.Error, "Za artikl [{0}] nema MPC!", artikl.ArtiklCD);

         #endregion artstat

         zeleniTRMrtrans_rec = new Rtrans()
         {
            T_TT         = Faktur.TT_TRM    ,
            T_artiklCD   = artikl.ArtiklCD  ,
            T_artiklName = artikl.ArtiklName,
            T_kol        = 1M               ,
            T_jedMj      = "kom"            ,
            T_mCalcKind  = ZXC.MalopCalcKind.By_MPC,
            T_wanted     = theMPC           ,

            // 16.01.2024: sa zaostatkom, bijo BUG! 
            T_pdvSt      = Faktur.CommonPdvStForThisDate(theDUC.faktur_rec.DokDate),
         };

         zeleniTRMrtrans_rec.CalcTransResults(null);

         theDUC.PutDgvLineFields1(zeleniTRMrtrans_rec, rowIdx, true);
         theDUC.PutDgvLineResultsFields1(rowIdx, zeleniTRMrtrans_rec, false);

         theDUC.GetDgvLineFields1(rowIdx, false, null); // da napuni Document's business.Transes 
      }
      
    //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

      #endregion Put, Calc, ... DgvFields
   }

   private void RISK_RetMoneyCalc_KN_input(object sender, EventArgs e) 
   {
      RISK_RetMoneyCalc_2023(ZXC.GotMoneyKind.Kune, e);
   }
   private void RISK_RetMoneyCalc_EUR_input(object sender, EventArgs e) 
   {
      RISK_RetMoneyCalc_2023(ZXC.GotMoneyKind.Euri, e);
   }

   private void RISK_RetMoneyCalc_Kn_AND_EUR_input(object sender, EventArgs e)
   {
      RISK_RetMoneyCalc_2023(ZXC.GotMoneyKind.Kune_AND_Euri, e);
   }

   private void RISK_RetMoneyCalc_2023(object sender, EventArgs e)
   {
      ZXC.GotMoneyKind gotMoneyKind = (ZXC.GotMoneyKind)sender;

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      bool isKNinput      = gotMoneyKind == ZXC.GotMoneyKind.Kune;
      bool isEURinput     = gotMoneyKind == ZXC.GotMoneyKind.Euri;
      string inputU       = isKNinput ? " u Kn: " : " u EUR: ";

      VvRetMoneyCalcDlg dlg = new VvRetMoneyCalcDlg(gotMoneyKind, "Unesi iznos dobiven od kupca", "Dobiven iznos" + inputU);

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         decimal theGotMoney = 0M;
         
         switch(gotMoneyKind)
         {
            case ZXC.GotMoneyKind.Kune         : theGotMoney = dlg.Fld_GotMoney_1; break;
            case ZXC.GotMoneyKind.Euri         : theGotMoney = dlg.Fld_GotMoney_2; break;
            case ZXC.GotMoneyKind.Kune_AND_Euri: theGotMoney = dlg.Fld_GotMoney_1 + 
                                                           dlg.Fld_GotMoney_2; break;
         }

         if(gotMoneyKind == ZXC.GotMoneyKind.Kune_AND_Euri) // Miješani GotMoney - kupac dao kune i eure 
         {
            isEURinput = true ; theGotMoney = ZXC.EURiIzKuna_HRD_(dlg.Fld_GotMoney_1) + dlg.Fld_GotMoney_2;
          //isEURinput = false; theGotMoney = ZXC.KuneIzEURa_HRD_(dlg.Fld_GotMoney_2) + dlg.Fld_GotMoney_1;
         }
         
         (decimal retMoney_KN, decimal retMoney_EUR) = ZXC.RetCalc_KN_OR_EUR(isEURinput, theDUC.Fld_S_ukKCRP, /*dlg.Fld_GotMoney_1*/theGotMoney);

         #region Check for negative

         if(retMoney_KN .IsNegative() || 
            retMoney_EUR.IsNegative())
         {
           //ZXC.aim_emsg(MessageBoxIcon.Error,
           //   "\n\n{2}\n\n\t\t{0} kn\n\n\nIznos koji treba uzvratiti je negativan.\n\nUnijeli ste premali dobiven iznos plaćanja\t:\t{1} {4}.\n\nZa iznos računa\t\t\t:\t{3} EUR.\n\nPonovno unesite dobiven iznos novca!",
           //   retMoney.ToStringVv(), dlg.Fld_Money.ToStringVv(), "GREŠKA!!!", theDUC.Fld_S_ukKCRP.ToStringVv(), inputU);
            ZXC.aim_emsg(MessageBoxIcon.Error,
               "\n\nGREŠKA!!!\n\n\nIznos koji treba uzvratiti je negativan.\n\nUnešen je premali dobiveni iznos\t:\t{0} {1}.\n\nZa iznos računa\t\t\t:\t{2} EUR.\n\nPonovno unesite dobiven iznos novca!",
               theGotMoney.ToStringVv(), inputU, theDUC.Fld_S_ukKCRP.ToStringVv());
         
            RISK_RetMoneyCalc_2023(sender, e);
         }

         #endregion Check for negative

         else
         { 
            VvReturnMoney_KNorEUR_Dlg dlgKnEur = new VvReturnMoney_KNorEUR_Dlg(isEURinput, theDUC.Fld_S_ukKCRP, theGotMoney);

            dlgKnEur.lbl_decFakMoney_Kn .Text = ZXC.KuneIzEURa_HRD_(theDUC.Fld_S_ukKCRP).ToStringVv() + " Kn";
            dlgKnEur.lbl_decFakMoney_EUR.Text =                     theDUC.Fld_S_ukKCRP .ToStringVv() + " EUR";

            dlgKnEur.lbl_decGotMoney_Kn .Text = isEURinput ? ZXC.KuneIzEURa_HRD_(theGotMoney).ToStringVv() + " Kn"  :                     theGotMoney .ToStringVv() + " Kn" ;
            dlgKnEur.lbl_decGotMoney_EUR.Text = isEURinput ?                     theGotMoney .ToStringVv() + " EUR" : ZXC.EURiIzKuna_HRD_(theGotMoney).ToStringVv() + " EUR";

            dlgKnEur.lbl_decRetMoney_KN .Text = retMoney_KN .ToStringVv() + " Kn" ;
            dlgKnEur.lbl_decRetMoney_EUR.Text = retMoney_EUR.ToStringVv() + " EUR";

            dlgKnEur.Fld_RetMoneyKN  = isEURinput ?         0.0M : retMoney_KN;
            dlgKnEur.Fld_RetMoneyEUR = isEURinput ? retMoney_EUR :        0.0M;

            dlgKnEur.ShowDialog();
            dlgKnEur.Dispose();
         }
      }

      dlg.Dispose();
      
   }

   private void RISK_RetMoneyCalc(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      VvRetMoneyCalcDlg dlg = new VvRetMoneyCalcDlg(ZXC.GotMoneyKind.Kune, "Unesi iznos dobiven od kupca", "Dobiven iznos:");

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         decimal retMoney = dlg.Fld_GotMoney_1 - theDUC.Fld_S_ukKCRP;

         string errMsg = retMoney.IsNegative() ? "GREŠKA!!!" : "";

         if(retMoney.IsNegative())
         {

            ZXC.aim_emsg(MessageBoxIcon.Error,
               "\n\n{2}\n\n\t\t{0} kn\n\n\nIznos koji treba uzvratiti je negativan.\n\nUnijeli ste premali iznos plaćanja\t:\t{1} kn.\n\nZa iznos računa\t\t\t:\t{3} kn.\n\nPonovno unesite primljen iznos novca!",
               retMoney.ToStringVv(), dlg.Fld_GotMoney_1.ToStringVv(), errMsg, theDUC.Fld_S_ukKCRP.ToStringVv());

            RISK_RetMoneyCalc(null, EventArgs.Empty);

         }
         else 
         {

          VvReturnMoneyDlg dlgR = new VvReturnMoneyDlg();
 
          dlgR.lbl_decS_ukKCRP.Text = theDUC.Fld_S_ukKCRP.ToStringVv();
          dlgR.lbl_decMoney   .Text = dlg.Fld_GotMoney_1.ToStringVv();
          dlgR.lbl_decRetMoney.Text = retMoney.ToStringVv();
          dlgR.ShowDialog();
 
          dlgR.Dispose();
         }
      }

      dlg.Dispose();
      
   }

   private void RISK_FiskParagon(object sender, EventArgs e)
   {
      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName  &&
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Privilegirana akcija. Odbijeno.");
         return;
      }

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      ZXC.RISK_FiskParagon_InProgress = true;

      Faktur fakturLastOK_IRM_rec = new Faktur();

      bool OK = TheVvDao.FrsPrvNxtLst_REC(TheDbConnection, fakturLastOK_IRM_rec, VvSQL.DBNavigActionType.LST, Faktur.sorterTtNum, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);

      if(!OK) { ZXC.aim_emsg("Ne mogu."); return; }

      VvFiskParagonDlg dlg = new VvFiskParagonDlg("FiskParagon");

      dlg.Fld_LastOK_IRM_Date = 
      dlg.Fld_StartDate       = fakturLastOK_IRM_rec.DokDate;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         ZXC.RISK_FiskParagon_InProgress = false;
         return;
      }

    //dlg.Fld_StartDate = DateTime.Today - ZXC.OneDaySpan;

      theDUC.Prev_ParagonIRM_Date = dlg.Fld_StartDate;

      if(theDUC.Prev_ParagonIRM_Date.Date < fakturLastOK_IRM_rec.DokDate.Date)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Datum mora biti jednak ili veći od zadnjeg IRMa.");
         dlg.Dispose();
         ZXC.RISK_FiskParagon_InProgress = false;
         return;
      }

      // Here we go: 
      if(theDUC.Prev_ParagonIRM_Date.Date == fakturLastOK_IRM_rec.DokDate.Date) // nastavljamo na isti datum 
      {
         theDUC.Prev_ParagonIRM_Date = fakturLastOK_IRM_rec.DokDate + ZXC.OneMinuteSpan;
      }
      else // nastavljamo sa nekim sljedecim danom od LastOK_IRM-a 
      {
         DateTime first_ParagonIRM_Date = theDUC.Prev_ParagonIRM_Date;

         theDUC.Prev_ParagonIRM_Date =
            new DateTime(
               first_ParagonIRM_Date.Year,
               first_ParagonIRM_Date.Month,
               first_ParagonIRM_Date.Day,
               ZXC.CURR_prjkt_rec.RvrOd.Hour,
               ZXC.CURR_prjkt_rec.RvrOd.Minute /*+ 1*/,
               0);
      }

      dlg.Dispose();
   }

   private void RISK_RNP_Analiza(object sender, EventArgs e)
   {
      PrintPreviewMenu_Button_OnClick(FakturDUC.RNP_Analiza, e);
   }

   // 08.11.2017: ovaj postaje PUSE, a iskoristaqvamo ga za provjeru 'da li je ZPC dobro nastao & receivan' 
   private void RISK_CheckZPC(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOshop) return;

      RtransDao.CheckAndRepare_ZPC_Kol_And_OldMpc(TheDbConnection);
   }

   // 08.11.2017: 'da li je ZPC dobro nastao & receivan' 
   private void RISK_CheckZPC_NEW(object sender, EventArgs e)
   {
      if(ZXC.IsTEXTHOany == false) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("RISK_CheckZPC START");
      ZXC.aim_log      ("RISK_CheckZPC START");

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      // THPR news: 
    //int wouldBe_newZpcRtransListCount = TH_CreateZPC(theDUC.Fld_DokDate, theDUC.Fld_SkladCD, /*is5week*/ ZXC.IsTH_5WeekShop(theDUC.Fld_SkladCD), theDUC.Fld_DokDate, /* isChkOnly*/ true       );
      int wouldBe_newZpcRtransListCount = TH_CreateZPC(theDUC.Fld_DokDate, theDUC.Fld_SkladCD,                                                     theDUC.Fld_DokDate, /* isChkOnly*/ true, false);

      Cursor.Current = Cursors.Default;
      ZXC.ClearStatusText();

      if(wouldBe_newZpcRtransListCount.NotZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA!\n\n{0} stavak. nedostaje ili ima krivu cijenu s obzirom trenutak u ciklusu!", wouldBe_newZpcRtransListCount);
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Information, "Sve MPC su ok.");
      }
   }

   // fuse 
   //private void QweQwe_UnusefulZPC(object sender, EventArgs e)
   //{
   //   List<Rtrans> unusefulZPC_rtransList = RtransDao.Get_UnusefulZPC_RtransList(TheDbConnection, DateTime.Now);
   //}

   /*private*/ public void Manual_CheckCache_CheckPrNabCij_CheckZPC(object sender, EventArgs e)
   { 
      bool allOKa = ArtiklDao.CheckCache                       (TheDbConnection);
    //bool allOKb = RtransDao.CheckAndRepare_ZPC_Kol_And_OldMpc(TheDbConnection);
      bool allOKc = RtransDao.CheckPrNabDokCij                 (TheDbConnection);

      if(allOKa && /*allOKb &&*/ allOKc) ZXC.aim_emsg(MessageBoxIcon.Information, "Sve je OK.");
   }

   /*private*/ public void Manual_CheckCache_badMSU(object sender, EventArgs e)
   {
      bool allOK = ArtiklDao.Check_MSU_Cache(TheDbConnection, false);

      if(allOK) ZXC.aim_emsg(MessageBoxIcon.Information, "Sve je OK.");
   }

#if someOneTimeOnlyTemboStuff
   /// <summary>
   /// TEMP!!! DellMeLatter
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void artiklPRS_SkipAllPersons(object sender, EventArgs e)
   {
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite Preimenovati Artikle: Brand + OldNaziv?!",
         "Potvrdite IZUZIMANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Artikl>(TheDbConnection, AddArtiklBrandPreffix2Naziv, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Preimenovao {0} artikala.", debugCount);
   }

   /// <summary>
   /// TEMP!!! DellMeLatter
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void PRS_SkipAllPersons(object sender, EventArgs e)
   {
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite Preimenovati Artikle: Brand + OldNaziv?!",
         "Potvrdite IZUZIMANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Rtrans>(TheDbConnection, AddArtiklBrandPreffix2Naziv4Rtrans, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Preimenovao {0} artikala.", debugCount);
   }

   /// <summary>
   /// TEMP!!! DellMeLatter
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   static bool AddArtiklBrandPreffix2Naziv(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Artikl artikl_rec = vvDataRecord as Artikl;

      if(artikl_rec.Grupa1CD.StartsWith("TOTAL")) return false;

      if(artikl_rec.ArtiklName.StartsWith(artikl_rec.Grupa1CD) /*&& !artikl_rec.ArtiklName.StartsWith("CASTRO ")*/) return false;

      if(artikl_rec.Grupa1CD == "CASTRO")
      {
         artikl_rec.ArtiklName = "CASTROL " + artikl_rec.ArtiklName;
      }
      else
      {
         artikl_rec.ArtiklName = artikl_rec.Grupa1CD + " " + artikl_rec.ArtiklName;
      }

      return artikl_rec.EditedHasChanges();
   }

   /// <summary>
   /// TEMP!!! DellMeLatter
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   static bool AddArtiklBrandPreffix2Naziv4Rtrans(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Rtrans rtrans_rec = vvDataRecord as Rtrans;

      ZXC.TheVvForm.TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      Artikl artiklSifrar_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == rtrans_rec.T_artiklCD);

      if(artiklSifrar_rec == null) return false;

      if(artiklSifrar_rec.Grupa1CD.StartsWith("TOTAL")) return false;

      if(rtrans_rec.T_artiklName.StartsWith(artiklSifrar_rec.Grupa1CD) /*&& !artikl_rec.ArtiklName.StartsWith("CASTRO ")*/) return false;

      if(artiklSifrar_rec.Grupa1CD == "CASTRO")
      {
         rtrans_rec.T_artiklName = "CASTROL " + rtrans_rec.T_artiklName;
      }
      else
      {
         rtrans_rec.T_artiklName = artiklSifrar_rec.Grupa1CD + " " + rtrans_rec.T_artiklName;
      }

      return rtrans_rec.EditedHasChanges();
   }
#endif
   
   private void RISK_AddSirovinaRowsViaNormativ(object sender, EventArgs e)
   { 
      #region Init Stuff

      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;
      int currRowIdx   = theDUC.TheG.CurrentRow.Index;

      // must be on 'product' (green) line. If not - go out 
      bool isProductLineChecked = VvCheckBox.GetBool4String(theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_isProductLine, currRowIdx, false)); if(isProductLineChecked == false) return;

      Rtrans produktRtrans_rec = (Rtrans)theDUC.GetDgvLineFields1(currRowIdx, false, null);

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      List<Faktur> fakturList = new List<Faktur>();

      #endregion Init Stuff

      #region Get NOR FakturList

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(1);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FaktExSchemaRows[ZXC.FexCI.prjArtCD], "elPrjArtCD", produktRtrans_rec.T_artiklCD, " = "));

      string selectWhat = "L.*, R.*";
      string orderBy = "dokDate , ttSort, ttNum";

      VvDaoBase.LoadGenericVvDataRecordList(TheDbConnection, fakturList, filterMembers, "", orderBy, true, selectWhat, "");

      if(fakturList.Count.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Za proizvod\n\n[{0}]\n\nNE postoji normativ (NOR)!", produktRtrans_rec.T_artiklCD);
         return;
      }
      if(fakturList.Count > 1 )
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Za proizvod\n\n[{0}]\n\npostoje {1} normativa (NOR).\n\nKoristim prvoga {2}-{3}",
            produktRtrans_rec.T_artiklCD, fakturList.Count, fakturList[0].TT, fakturList[0].TtNum);
      }

      //fakturList.ForEach(fak => fak.VvDao.LoadTranses(TheDbConnection, fak, false));

      Faktur faktur_recNOR = fakturList[0];

      faktur_recNOR.VvDao.LoadTranses(TheDbConnection, faktur_recNOR, false);

      #endregion Get NOR FakturList

      #region Put, Calc, ... DgvFields

      ArtStat artStat_rec;

      foreach(Rtrans sirovinaRtrans_rec in faktur_recNOR.Transes)
      {
         theDUC.TheG.Rows.Add();
         
         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         sirovinaRtrans_rec.T_kol *= produktRtrans_rec.T_kol;

         artStat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, sirovinaRtrans_rec.T_artiklCD, theDUC.Fld_SkladCD, theDUC.Fld_DokDate);
         if(artStat_rec != null) sirovinaRtrans_rec.T_cij = artStat_rec.PrNabCij.Ron(4);

         theDUC.PutDgvLineFields1(sirovinaRtrans_rec, rowIdx, true);
       //sirovinaRtrans_rec.CalcTransResults(null/*faktur_rec*/);
       //theDUC.PutDgvLineResultsFields1(rowIdx, sirovinaRtrans_rec, false);
       //theDUC.GetDgvLineFields1(rowIdx, false, null); // da napuni Document's business.Transes 
         
      }

      theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

      #endregion Put, Calc, ... DgvFields
   }

   private void RISK_Adjust_SukKCR(object sender, EventArgs e)
   {
    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      FakturDUC    theDUC = TheVvDocumentRecordUC as    FakturDUC;

      VvRetMoneyCalcDlg dlg = new VvRetMoneyCalcDlg(ZXC.GotMoneyKind.Kune, "Unesi ciljani iznos", "Ciljani iznos:");

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         decimal newRedCIJ, deltaKCR, oldKCR, oldRedCIJ, deltaKoef;
         decimal targetKCR = dlg.Fld_GotMoney_1; // Fld_Money je Target KCR 

         Faktur faktur_rec = theDUC.faktur_rec;

         oldKCR    = faktur_rec.S_ukKCR;
         deltaKCR  = targetKCR - oldKCR;

         deltaKoef = ZXC.DivSafe(deltaKCR, oldKCR);

         for(int rIdx = 0; rIdx < theDUC.TheG.RowCount - 1; ++rIdx)
         {
            oldRedCIJ   = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_cij, rIdx, false);

            newRedCIJ   = oldRedCIJ * (1M + deltaKoef);

            theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij, rIdx, newRedCIJ);
         }
      }

      dlg.Dispose();

      theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

   private void RISK_PutAllLager_WithoutKol(object sender, EventArgs e)
   {
      RISK_PutAllLager_JOB(false);
   }

   private void RISK_PutAllLager_WithKol(object sender, EventArgs e)
   {
      RISK_PutAllLager_JOB(true);
   }

   private void RISK_PutAllLager_JOB(bool isKolWanted)
   {
    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;

    //TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      List<Artikl> theArtiklWithArtstatList = new List<Artikl>();

      ArtiklDao.Get_HasKolStOnly_ArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, theDUC.Fld_SkladCD, theDUC.Fld_DokDate, "", "artiklName ");

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      #region Pagging

      // 08.12.2018: (Hi, John) Paging ... START ... 
      // "Red:1-100" 

      int startRowIdx;
      int rangeCount ;

      if(theDUC.GetPagging_IndexAndCount_FromNapomena(theDUC.Fld_Napomena, out startRowIdx, out rangeCount))
      {
         int listCount    = theArtiklWithArtstatList.Count;
         int lastValidIdx = listCount - 1;
         int endRowIdx    = startRowIdx + rangeCount -1;

         if(startRowIdx > lastValidIdx || 
            endRowIdx   > lastValidIdx  )
         {

            ZXC.aim_emsg(MessageBoxIcon.Error, "Neispravno zadani Od - Do!");
            return;
         }

         theDUC.Fld_Napomena += " od " + theArtiklWithArtstatList.Count + " redaka.";

         theArtiklWithArtstatList = theArtiklWithArtstatList.GetRange(startRowIdx, rangeCount).ToList();
      }

    //if(theArtiklWithArtstatList.Count > 100                            ) 
      if(theArtiklWithArtstatList.Count > 100 && ZXC.IsTEXTHOany == false)
      {
         DialogResult result = MessageBox.Show("Da li zaista želite postaviti " + theArtiklWithArtstatList.Count + " redaka ovome dokumentu?!",
            "Potvrdite moguće dugotrajnu operaciju?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;
      }

      // 08.12.2018: (Hi, John) Paging ...  END  ... 

      #endregion Pagging

      foreach(Artikl artikl_rec in theArtiklWithArtstatList)
      {
         // 16.01.2015: preskaci ako ga vec ima ... pattern: Dubravko prvo stavi sto ocima vidi zatim nadopuni sa onim sto kaze LagerLista 
         if(theDUC.faktur_rec.Transes.Count(rtr => rtr.T_artiklCD == artikl_rec.ArtiklCD).NotZero()) continue;

         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD  , rowIdx, artikl_rec.ArtiklCD     );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, rowIdx, artikl_rec.ArtiklName   );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_jedMj     , rowIdx, artikl_rec.JedMj        );

         if(isKolWanted)
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol       , rowIdx, artikl_rec.AS_StanjeKol );
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol2      , rowIdx, artikl_rec.AS_StanjeKol2);
         }

         // Velep / Malop cij handling 
       //theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij       , rowIdx, artikl_rec.AS_PrNabCij  );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij       , rowIdx, artikl_rec.AS_KnjigCij  );

      } // foreach(Artikl artikl_rec in theArtiklWithArtstatList) 

      // 23.12.2016: maknuli radi ubrzanja ... 30.12.2016 ipak vratili jer treba 
      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
        theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();

   }

   private void RISK_AddPPRonRNM(object sender, EventArgs e) 
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_PPR, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (RNMDUC)TheVvUC, true));
   }

   private void RISK_AddPIPonRNM(object sender, EventArgs e) 
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_PIP, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (RNMDUC)TheVvUC, true));
   }

   private void RISK_ZPC_THlowPriceUglyEuros2NiceEuros(object sender, EventArgs e)
   { 
    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;

    //TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      List<Artikl> theArtiklWithArtstatList = new List<Artikl>();

      ArtiklDao.Get_HasKolStOnly_ArtiklWithArtstatList(TheDbConnection, theArtiklWithArtstatList, theDUC.Fld_SkladCD, theDUC.Fld_DokDate, "", "artiklCd ");

      bool is5week = ZXC.IsTH_5WeekShop(theDUC.Fld_SkladCD);

    //theArtiklWithArtstatList.RemoveAll(art => art.ImportCij > (is5week ? 4.00M : 2.00M));
      theArtiklWithArtstatList.RemoveAll(art => (art.AS_MalopCij > 1.98M && 
                                                 art.AS_MalopCij < 2.00M) == false);

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      foreach(Artikl artikl_rec in theArtiklWithArtstatList)
      {
         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklCD  , rowIdx, artikl_rec.ArtiklCD     );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, rowIdx, artikl_rec.ArtiklName   );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_jedMj     , rowIdx, artikl_rec.JedMj        );

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvSt     , rowIdx, 25.00M);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol, rowIdx, artikl_rec.AS_StanjeKol);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij       , rowIdx, artikl_rec.AS_PrNabCij  );
       //theDUC.TheG.PutCell(theDUC.DgvCI.iT_doCijMal  , rowIdx, artikl_rec.AS_MalopCij  );
       //theDUC.TheG.PutCell(theDUC.DgvCI.iT_noCijMal  , rowIdx, artikl_rec.ImportCij    );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_doCijMal  , rowIdx, 1.99M                   );
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_noCijMal  , rowIdx, 2.00M                   );

      } // foreach(Artikl artikl_rec in theArtiklWithArtstatList) 

      // 23.12.2016: maknuli radi ubrzanja ... 30.12.2016 ipak vratili jer treba 
      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
      //theDUC.GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS();
   }

#if DEBUG

   private void /*Manual_CheckCache_badMSU*/SvDuh_Extract_ORG_FromNaziv(object sender, EventArgs e)
   {
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite 'SvDuh_Extract_ORG_FromNaziv'?!",
         "Potvrdite SvDuh_Extract_ORG_FromNaziv?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Artikl>(TheDbConnection, SvDuh_Extract_ORG_FromNaziv_JOB, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Sredio {0} ORG-a.", debugCount);
   }

   static bool SvDuh_Extract_ORG_FromNaziv_JOB(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Artikl artikl_rec = vvDataRecord as Artikl;

      if(artikl_rec.OrgPak.NotEmpty() && artikl_rec.OrgPak != "1") return false;

      string newORG = "";

      string artName = artikl_rec.ArtiklName.Replace(" ", "").ToUpper();

      if(!artName.Contains("X")) return false;

      if(!artName.ToUpper().Contains("MG")) return false;

      int xIdx = artName.IndexOf('X');

      if(xIdx < 5) return false;

      if(xIdx == artName.Length - 1) return false;

      char ch1BefX = artName[xIdx - 1];
      char ch2BefX = artName[xIdx - 2];
      char ch3BefX = artName[xIdx - 3];
      char ch1AftX = artName[xIdx + 1];

      if(Char.IsDigit(ch1BefX) == false || Char.IsDigit(ch1AftX) == false) return false;

      if(Char.IsDigit(ch3BefX) && Char.IsDigit(ch2BefX) && Char.IsDigit(ch1BefX)) newORG = artName.SubstringSafe(xIdx - 3, 3);
      else if(Char.IsDigit(ch2BefX) && Char.IsDigit(ch1BefX)) newORG = artName.SubstringSafe(xIdx - 2, 2);
      else if(Char.IsDigit(ch1BefX)) newORG = artName.SubstringSafe(xIdx - 1, 1);

      if(newORG.NotEmpty())
      {
         artikl_rec.OrgPak = newORG;
      }

      return artikl_rec.EditedHasChanges();
   }

   private void RISK_SVD_PairHALMED_Artikl_WebServis(object sender, EventArgs e)
   {
      string uri = @"https://ws.halmed.hr/ws/test/FarmakoWS.php"; // testno okruzenje 
      string username = "viper";
      string password = "viperzg06";

      string encodedBasicAuthorization = Vv_WS.GetEncodedBasicAuthorization(username, password);

      XmlDocument xmlDocument_request = new XmlDocument();

      WebRequest webRequest = Vv_WS.CreateWebRequest(uri, encodedBasicAuthorization, xmlDocument_request, "PreuzimanjeSifrarnika");

      XmlDocument xmlDocument_response = Vv_WS.GetXmlDocumentFromWebResponse(webRequest);

   }

   private void RISK_SVD_PairHALMED_Artikl(object sender, EventArgs e)
   {
//=========== testis only start ===================================== 

      Halmed_SVD.OblikLijeka_tablica hzzoArtikliTablica = new Halmed_SVD.OblikLijeka_tablica();

      Halmed_SVD.OblikLijeka_tablica.LoadFromFile(@"E:\0_DOWNLOAD\Export Import Rules\HALMED_SVD\Šifrarnik lijekova s oblicima - 2020-ws CLEAN byQ.xml", out hzzoArtikliTablica);
    //Halmed_SVD.OblikLijeka_tablica.LoadFromFile(@"E:\0_UPLOAD\SvetiDuh\2018 VEKTOR\User Data\HALMED in2023 for2022\Šifrarnik lijekova s oblicima - 2022.XML", out hzzoArtikliTablica);

      ZXC.hArtiklList_FromXML_wATK11 = hzzoArtikliTablica.Items.ToList();

//============ end=================================================== 
      int debugCount;

      DialogResult result = MessageBox.Show("Da li zaista zelite 'SvDuh_Set_HALMEDartikl_s_lio_to_Atest'?!",
         "Potvrdite SvDuh_Set_HALMEDartikl_s_lio_to_Atest?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.hArtiklList = VvDaoBase.Get_HALMEDartikl_List(TheDbConnection, "");

      ZXC.ErrorsList = new List<string>();

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Artikl>(TheDbConnection, SvDuh_Set_HALMEDartikl_s_lio_to_Atest_JOB, null, "", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount);

      Cursor.Current = Cursors.Default;

      if(ZXC.ErrorsList.Count.IsPositive()) ZXC.aim_emsg_List(string.Format("{0} Neunikatnih ATK11 šifri.", ZXC.ErrorsList.Count), ZXC.ErrorsList);

      ZXC.aim_emsg("Gotovo. Sredio {0} s_lio-a.", debugCount);
   }

   static bool SvDuh_Set_HALMEDartikl_s_lio_to_Atest_JOB(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
      Artikl artikl_rec = vvDataRecord as Artikl;

      if(artikl_rec.IsSvdArtGR_Ljek_ == false) return false;

      if(artikl_rec.ArtiklCD2.IsEmpty() /* ---> ATK */ || artikl_rec.AtestBr.NotEmpty()) return false;

      Halmed_SVD.OblikLijeka_tablicaOblikLijeka the_hArtikl_FromXML_wATK11 = null;
      Halmed_SVD.HALMEDartikl                   the_hArtikl                = new Halmed_SVD.HALMEDartikl();

      List<Halmed_SVD.OblikLijeka_tablicaOblikLijeka> thisATK11_hArtikl_FromXML_wATK11_List = ZXC.hArtiklList_FromXML_wATK11.Where(ha => ha.HZZO_SIFRA == artikl_rec.ArtiklCD2).ToList();
      List<Halmed_SVD.HALMEDartikl                  > thisATK11_hArtikl_List                = ZXC.hArtiklList               .Where(ha => ha.hzzo_sifra == artikl_rec.ArtiklCD2).ToList();

#if staraLogika
      switch(thisATK11_hArtikl_FromXML_wATK11_List.Count)
      {
         case 0 : // nije nasao po ATK11 
                  the_hArtikl = thisATK11_hArtikl_List.FirstOrDefault();
                  // second chance: 
                  if(the_hArtikl.s_lio.NotEmpty())
                  {
                     artikl_rec.AtestBr = the_hArtikl.s_lio;
                     return artikl_rec.EditedHasChanges();
                  }
                  return false; 

         case 1: // OK. Nasao je jednog jedinog 
                 Halmed_SVD.OblikLijeka_tablicaOblikLijeka  foundArtikl = thisATK11_hArtikl_FromXML_wATK11_List.First();
                 the_hArtikl_FromXML_wATK11 = foundArtikl;
                 if(ZXC.SubstringSafe(foundArtikl.HZZO_SIFRA, 0, 7) != foundArtikl.S_ATK)
                 {
                    ZXC.ErrorsList.Add(String.Format("ZaATK11 [{0}]\n\n{1}\n\nHZZO sifra i s_atk se ne podudaraju u HALMEDovom sifrarniku!", artikl_rec.ArtiklCD2, foundArtikl.S_ATK));
                 }
                 break; 

         default: // notOK. nasao vise od 1 
                  ZXC.ErrorsList.Add(String.Format("ZaATK11 [{0}]\n\n{1}\n\nPostoje {2} artikla u HALMEDovom sifrarniku!", artikl_rec.ArtiklCD2, artikl_rec.ArtiklName, thisATK11_hArtikl_FromXML_wATK11_List.Count));
                  return false;
      }
#endif
      switch(thisATK11_hArtikl_List.Count)
      {
         case 0 : // nije nasao po ATK11 
                  return false; 

         case 1: // OK. Nasao je jednog jedinog 

                  the_hArtikl = thisATK11_hArtikl_List.First();

                  break; 

         default: // notOK. nasao vise od 1 
                //ZXC.ErrorsList.Add(String.Format("ZaATK11 [{0}]\n\n{1}\n\nPostoje {2} artikla u HALMEDovom sifrarniku!", artikl_rec.ArtiklCD2, artikl_rec.ArtiklName, thisATK11_hArtikl_FromXML_wATK11_List.Count));
                  ZXC.ErrorsList.Add(String.Format("ZaATK11 [{0}]\n\n{1}\n\nPostoje {2} artikla u HALMEDovom sifrarniku!", artikl_rec.ArtiklCD2, artikl_rec.ArtiklName, thisATK11_hArtikl_List.Count));
                  return false;
      }

    //string the_s_lio = the_hArtikl_FromXML_wATK11 is null ? "" : the_hArtikl_FromXML_wATK11.S_LIO;
      string the_s_lio = the_hArtikl.s_lio;

      if(the_s_lio.NotEmpty())
      {
         artikl_rec.AtestBr = the_s_lio;
      }

      return artikl_rec.EditedHasChanges();
    //return false;
   }

   #region PUSE DELME LATER

   //static bool SvDuh_Set_HALMEDartikl_s_lio_to_Atest_JOB_OLD_AND_PUSE(XSqlConnection conn, VvDataRecord vvDataRecord)
   //{
   //   Artikl artikl_rec = vvDataRecord as Artikl;
   //
   //   if(artikl_rec.IsSvdArtGR_Ljek_ == false) return false;
   //
   //   if(artikl_rec.ArtiklCD2.IsEmpty() /* ---> ATK */ || artikl_rec.AtestBr.NotEmpty()) return false;
   //
   //   string ATK_root7 = artikl_rec.ArtiklCD2.SubstringSafe(0, 7);
   //
   //   List<Halmed_SVD.HALMEDartikl> ATK7_hArtiklList = ZXC.hArtiklList.Where(ha => ha.s_atk == ATK_root7).ToList();
   //
   //   if(ATK7_hArtiklList.IsEmpty()) return false;
   //
   //   string the_s_lio = Get_s_lio_Abrakadabra_byQ(ATK7_hArtiklList, artikl_rec);
   //
   //   if(the_s_lio.NotEmpty())
   //   {
   //      artikl_rec.AtestBr = the_s_lio;
   //   }
   //
   //   return artikl_rec.EditedHasChanges();
   // //return false;
   //}
   //
   //private static string Get_s_lio_Abrakadabra_byQ(List<Halmed_SVD.HALMEDartikl> ATK7_hArtiklList, Artikl artikl_rec)
   //{
   //   Halmed_SVD.HALMEDartikl hArtikl;
   //
   //   string hBrPak    = Get_hBrPak   (artikl_rec);
   //   string hMjOzn    = Get_hMjOzn   (artikl_rec);
   //   string hDoza     = Get_hDoza    (artikl_rec); 
   //
   //   if(hMjOzn == "L") // Svduh pise litre a hlmed i hzzo mililitrer
   //   {
   //      hMjOzn = "ML";
   //
   //      hDoza  = (ZXC.ValOrZero_Decimal(hDoza, 2) * 1000M).ToString0Vv_NoGroup();
   //   }
   //
   // //hArtikl = ATK7_hArtiklList.FirstOrDefault(ha => artikl_rec.ArtiklName.Contains(ha.opis_doze.ToLower()));
   // //hArtikl = ATK7_hArtiklList.FirstOrDefault(ha =>
   // //
   // //   ha.br_pak           == hBrPak &&
   // //   ha.mj_ozn.ToLower() == hMjOzn.ToLower() &&
   // //   ha.doza             == hDoza            );
   //
   //   var candidateList1 = ATK7_hArtiklList.Where(ha =>  ha.br_pak == hBrPak &&
   //                                                      ha.mj_ozn.ToLower() == hMjOzn.ToLower() &&
   //                                                      ha.doza == hDoza).ToList();
   //
   //   if(candidateList1.IsEmpty()) return "";
   //
   //   if(candidateList1.Count() == 1) return candidateList1.First().s_lio;
   //
   // //hArtikl = candidateList1.FirstOrDefault(ha => artikl_rec.ArtiklName.ToLower().Contains(ha.naziv.ToLower()));
   //   hArtikl = candidateList1.FirstOrDefault(ha => Get_FirstWord(artikl_rec.ArtiklName).ToLower() == Get_FirstWord(ha.naziv).ToLower());
   //
   //   if(hArtikl.s_lio.NotEmpty()) return hArtikl.s_lio;
   //
   //   return candidateList1.First().s_lio;
   //}
   //
   //private static string Get_hDoza(Artikl artikl_rec)
   //{
   //   int lastSpace = artikl_rec.ArtiklName.LastIndexOf(" ");
   //
   //   string shorterName = artikl_rec.ArtiklName.SubstringSafe(0, lastSpace);
   //
   //   lastSpace = shorterName.LastIndexOf(" ");
   //
   //   return shorterName.SubstringSafe(lastSpace + 1);
   //}
   //
   //private static string Get_hMjOzn(Artikl artikl_rec)
   //{
   //   if(artikl_rec.ArtiklName.EndsWith(" MG") == false &&
   //      artikl_rec.ArtiklName.EndsWith(" G")  == false &&
   //      artikl_rec.ArtiklName.EndsWith(" KG") == false &&
   //      artikl_rec.ArtiklName.EndsWith(" ML") == false &&
   //      artikl_rec.ArtiklName.EndsWith(" L")  == false &&
   //      artikl_rec.ArtiklName.EndsWith(" MM") == false &&
   //      artikl_rec.ArtiklName.EndsWith(" CM") == false &&
   //      artikl_rec.ArtiklName.EndsWith(" M")  == false  ) return "";
   //
   //   int lastSpace = artikl_rec.ArtiklName.LastIndexOf(" ");
   //
   //   return artikl_rec.ArtiklName.SubstringSafe(lastSpace + 1);
   //}
   //
   //private static string Get_hBrPak(Artikl artikl_rec)
   //{
   //   return artikl_rec.OrgPak;
   //}
   //
   //private static string Get_FirstWord(string text)
   //{
   //   int firstSpace = text.IndexOf(" ");
   //
   //   if(firstSpace.IsPositive()) return text.SubstringSafe(0, firstSpace);
   //
   //   return text;
   //}

   #endregion PUSE DELME LATER

#else
   private void RISK_SVD_PairHALMED_Artikl(object sender, EventArgs e)
   {
      throw new Exception("RISK_SVD_PairHALMED_Artikl radi samo u DEBUG modu.");
   }
#endif

   #region SVD

   private void RISK_CopySVD_NRD_to_URA(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_URA_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (NRD_SVD_DUC)TheVvUC, true));
   }

   private void RISK_CopySVD_URA_to_IZD(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IZD_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (URA_SVD_DUC)TheVvUC, true));
   }

   private void RISK_CopySVD_ZAH_to_IZD(object sender, EventArgs e)
   {
      #region Check idemo li dalje 

      if(ZXC.IsSvDUH_ZAHonly)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena operacija.");
         return;
      }

      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec                 ;

    //bool isVeza1Empty = faktur_rec.V1_tt.IsEmpty() && faktur_rec.V1_ttNum.IsZero(); // npr kad pozove SubMakciju 'IZD' a ne sejva nego ESC                        
                                                                                      // tada ZAH dobije status 'D' a bez zaista IZD pa treba dozvoliti ponavljanje 
    //bool isVeza1_NOT_Empty = !isVeza1Empty;

    //if(faktur_rec.StatusCD == "D")
    //if(faktur_rec.StatusCD == "D" && isVeza1_NOT_Empty)
      if(faktur_rec.StatusCD == "D")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već izdana");
         return;
      }
      if(faktur_rec.StatusCD == "O")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je još u izradi");
         return;
      }
      if(faktur_rec.StatusCD == "S")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je stornirana");
         return;
      }
      if(faktur_rec.StatusCD == "P")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova zahtjevnica je predložak");
         return;
      }
      if(faktur_rec.StatusCD == "C")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica još nije odobrena");
         return;
      }

      if(faktur_rec.StatusCD != "N")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica nije spremna za izdavanje?"); // ovo je nama kontrola 
         return;
      }

      #endregion Check idemo li dalje 

      // here we go ... 

      Faktur ZAHfaktur_rec = (Faktur)(faktur_rec.CreateNewRecordAndCloneItComplete());

      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IZD_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (ZAH_SVD_DUC)TheVvUC, true));

   }

   private bool Get_has_everyZAHrtrans_IZDrtrans(Faktur ZAHfaktur_rec)
   {
      bool everyZAHrtrans_has_IZDrtrans = true;

      List<Rtrans> IZDfromZAHlinkedRtransList = RtransDao.GetRtransList_TT_V1tt_V1ttNum_KupdobCD/*_DokDateOD*/(TheDbConnection, Faktur.TT_IZD, ZAHfaktur_rec.TT, ZAHfaktur_rec.TtNum, ZAHfaktur_rec.KupdobCD/*, faktur_rec.DokDate*/);

      if(IZDfromZAHlinkedRtransList.IsEmpty()) return false;

      bool IZDrtransFound;

      foreach(Rtrans ZAHrtrans in ZAHfaktur_rec.Transes)
      {
         IZDrtransFound = IZDfromZAHlinkedRtransList.Any(IZDrtr => IZDrtr.T_artiklCD == ZAHrtrans.T_artiklCD);

         if(IZDrtransFound == false)
         {
            everyZAHrtrans_has_IZDrtrans = false;
            break;
         }
      }

      return everyZAHrtrans_has_IZDrtrans;
   }

   private void RISK_CopySVD_IZD_to_ZAH(object sender, EventArgs e)
   {

      #region Rwtrec Feedback 

      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec                 ;

      BeginEdit(theDUC.faktur_rec);

      bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

      EndEdit(faktur_rec);

      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

      #endregion Rwtrec Feedback 

      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_ZAH_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (IZD_SVD_DUC)TheVvUC, true));

   }

   private void RISK_CopySVD_URA_to_NRD(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_NRD_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (URA_SVD_DUC)TheVvUC, true));
   }

   private void RISK_CopySVD_UGO_to_NRD(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_NRD_SVD, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (UGODUC)TheVvUC, true));
   }

   /*private*/ public void RISK_Copy_UFA_to_ISP(object sender, EventArgs e)
   {
      VvRecordUC vvrecorUC;
           if(TheVvUC is URADUC )  vvrecorUC = (URADUC)TheVvUC;
      else if(TheVvUC is POT_DUC)  vvrecorUC = (POT_DUC)TheVvUC;
      else /*(TheVvUC is UFADUC)*/ vvrecorUC = (UFADUC)TheVvUC;
      
    //RISK_CopyToSomeOtherTT(                                                 ZXC.VvSubModulEnum.R_ISP, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (UFADUC)TheVvUC, true));
      RISK_CopyToSomeOtherTT(ZXC.IsTETRAGRAM_ANY ? ZXC.VvSubModulEnum.R_ABI : ZXC.VvSubModulEnum.R_ISP, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, vvrecorUC      , true));
   }

   /*private*/ public void RISK_Copy_URA_to_ISP(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_ISP, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (URADUC)TheVvUC, true));
   }

   /*private*/public void RISK_Copy_IFA_to_UPL(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_UPL, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (IFADUC)TheVvUC, true));
   }

   private void RISK_Copy_IRA_to_UPL(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_UPL, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (IRADUC)TheVvUC     , true));
   }

   private void RISK_Copy_PON_to_IRA(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IRA, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (PonudaDUC)TheVvUC, true));
   }
   private void RISK_Copy_PON_MPC_to_IRA_MPC(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IRA_MPC, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (PON_MPC_DUC)TheVvUC, true));
   }
   private void RISK_Copy_PON_MPC_to_ZAR(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_ZAR, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (PON_MPC_DUC)TheVvUC, true));
   }

   private void RISK_Copy_PON_MPC_to_OPN_MPC(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_OPN_MPC, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (PON_MPC_DUC)TheVvUC, true));
   }
   private void RISK_Copy_OPN_MPC_to_IRA_MPC(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IRA_MPC, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (OPN_MPC_DUC)TheVvUC, true));
   }

   private void RISK_Copy_IZD_MPC_to_IRA_MPC(object sender, EventArgs e)
   {
      RISK_CopyToSomeOtherTT(ZXC.VvSubModulEnum.R_IRA_MPC, /*EventArgs.Empty*/new NewRecordEventArgs(ZXC./*Mixer*/FakturRec, (IZD_MPC_DUC)TheVvUC, true));
   }
   private void RISK_Copy_T1_IRA_to_T2_URA(object sender, EventArgs e)
   {
      #region Vidi malo idemo li dalje

      string message = "Da li zaista zelite kreirati T2 URA-u?!";

      DialogResult result = MessageBox.Show(message, "POTVRDITE KREIRANJE T2 FAKTURE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes)
      {
         return;
      }

      #endregion Vidi malo idemo li dalje

      #region init

      // tu fali provjera i eventualni return ako ovo stisnu gdje nema smisla 

    //Faktur origin_faktur_T1_IRA_rec = TheVvDataRecord as Faktur;
      Faktur origin_faktur_T1_IRA_rec = (Faktur)TheVvDataRecord.CreateNewRecordAndCloneItComplete();
    //Faktur kontra_faktur_T2_URA_rec = new Faktur();

      #endregion init

      uint T2_URA_TtNum = AutoSet_T2URA_Faktur(TheDbConnection, origin_faktur_T1_IRA_rec/*, kontra_faktur_T2_URA_rec*/);

      #region Rwtrec Feedback CashFakturu

      Faktur origCashFaktur_rec = TheVvDataRecord as Faktur;

      BeginEdit(origCashFaktur_rec);

      string v1_tt = "T2U";
    //uint v1_ttNum = kontra_faktur_T2_URA_rec.TtNum;
      uint v1_ttNum = T2_URA_TtNum;

      bool imaMjesta = true;

      switch(origCashFaktur_rec.FirstEmptyVezaLine)
      {
         case 1:
            origCashFaktur_rec.V1_tt    = v1_tt;
            origCashFaktur_rec.V1_ttNum = v1_ttNum;
            break;
         case 2:
            origCashFaktur_rec.V2_tt    = v1_tt;
            origCashFaktur_rec.V2_ttNum = v1_ttNum;
            break;
         case 3:
            origCashFaktur_rec.V3_tt    = v1_tt;
            origCashFaktur_rec.V3_ttNum = v1_ttNum;
            break;
         case 4:
            origCashFaktur_rec.V4_tt    = v1_tt;
            origCashFaktur_rec.V4_ttNum = v1_ttNum;
            break;

         default: imaMjesta = false; break;
      }

      if(imaMjesta)
      {
         ZXC.FakturRec.VvDao.RWTREC(TheDbConnection, origCashFaktur_rec, false, true, false);
      }

      EndEdit(origCashFaktur_rec);

      PutFieldsActions(TheDbConnection, origCashFaktur_rec, TheVvRecordUC);

      #endregion Rwtrec Feedback CashFakturu

    //ZXC.aim_emsg(MessageBoxIcon.Information, "Dodana je T2 URA br. [{0}]", kontra_faktur_T2_URA_rec.TtNum);
      ZXC.aim_emsg(MessageBoxIcon.Information, "Dodana je T2 URA br. [{0}]", T2_URA_TtNum);
   }

   private uint AutoSet_T2URA_Faktur(XSqlConnection conn, Faktur origin_faktur_T1_IRA_rec)
   {
      #region Set T2 URA from T1 IRA

      Faktur kontra_faktur_T2_URA_rec;

      Rtrans rtrans_T1_IRA_rec;

      string T1_IRA_TtNumFiskal = origin_faktur_T1_IRA_rec.TtNumFiskal;

      for(int i = 0; i < origin_faktur_T1_IRA_rec.Transes.Count; ++i)
      {
         rtrans_T1_IRA_rec = origin_faktur_T1_IRA_rec.Transes[i];

         rtrans_T1_IRA_rec.T_TT        = Faktur.TT_URA;
         rtrans_T1_IRA_rec.T_wanted    = 0M;
         rtrans_T1_IRA_rec.T_pdvColTip = ZXC.PdvKolTipEnum.NIJE;

         rtrans_T1_IRA_rec.CalcTransResults(null);
      }

      origin_faktur_T1_IRA_rec.TT = Faktur.TT_URA;
      origin_faktur_T1_IRA_rec.TakeTransesSumToDokumentSum(true); // !!! 

      kontra_faktur_T2_URA_rec = (Faktur)(origin_faktur_T1_IRA_rec.CreateNewRecordAndCloneItComplete());

      kontra_faktur_T2_URA_rec.InvokeTransClear();

      kontra_faktur_T2_URA_rec.TT       = Faktur.TT_URA;
      kontra_faktur_T2_URA_rec.TtNum    = 0;

      kontra_faktur_T2_URA_rec.SkladCD  = "Z-GL-2";

      kontra_faktur_T2_URA_rec.KupdobCD     = 003916;
      kontra_faktur_T2_URA_rec.KupdobTK     = "TETR16";
      kontra_faktur_T2_URA_rec.KupdobName   = "Tetragram projekt d.o.o.";
      kontra_faktur_T2_URA_rec.KdUlica      = "Svetice 19";
      kontra_faktur_T2_URA_rec.KdMjesto     = "Zagreb";
      kontra_faktur_T2_URA_rec.KdZip        = "10000";
      kontra_faktur_T2_URA_rec.KdOib        = "91926034939";
      kontra_faktur_T2_URA_rec.PosJedCD     = 003916;
      kontra_faktur_T2_URA_rec.PosJedTK     = "TETR16";
      kontra_faktur_T2_URA_rec.PosJedName   = "Tetragram projekt d.o.o.";
      kontra_faktur_T2_URA_rec.PosJedUlica  = "Svetice 19";
      kontra_faktur_T2_URA_rec.PosJedMjesto = "Zagreb";
      kontra_faktur_T2_URA_rec.PosJedZip    = "10000";

    //kontra_faktur_T2_URA_rec.VezniDok = origin_faktur_T1_IRA_rec.TtNumFiskal;
      kontra_faktur_T2_URA_rec.VezniDok = T1_IRA_TtNumFiskal;
      
      kontra_faktur_T2_URA_rec.NacPlac  =
      kontra_faktur_T2_URA_rec.NacPlac2 = "";

      kontra_faktur_T2_URA_rec.V1_tt    = "T1I";
      kontra_faktur_T2_URA_rec.V1_ttNum = origin_faktur_T1_IRA_rec.TtNum;
    //kontra_faktur_T2_URA_rec.V2_tt    = "RID";
    //kontra_faktur_T2_URA_rec.V2_ttNum = origin_faktur_T1_IRA_rec.RecID;

      kontra_faktur_T2_URA_rec.FiskJIR   =
      kontra_faktur_T2_URA_rec.FiskZKI   = 
      kontra_faktur_T2_URA_rec.FiskPrgBr = "";

      #endregion Set T2 URA from T1 IRA

      #region Change database name to TheDbConnection

      string kontra_dbName =
         ZXC.vvDB_VvDomena == "vvT1" ? conn.Database.Replace("vvT1", "vvT2").Replace("TGPROJ", "TGPLEM") :
         ZXC.vvDB_VvDomena == "vvT2" ? conn.Database.Replace("vvT2", "vvT1").Replace("TGPLEM", "TGPROJ") :
         throw new Exception("vvDomena nije niti T1 niti T2");

      string orig_dbName = conn.Database;

      conn.ChangeDatabase(kontra_dbName);

      #endregion Change database name to TheDbConnection

      #region AutoSetFaktur

      ushort line = 0;

      Rtrans rtrans_T2_URA_rec;

      for(int i = 0; i < origin_faktur_T1_IRA_rec.Transes.Count; ++i)
      {
         rtrans_T2_URA_rec = origin_faktur_T1_IRA_rec.Transes[i];

         FakturDao.AutoSetFaktur(conn, ref line, kontra_faktur_T2_URA_rec, rtrans_T2_URA_rec);
      }

      #endregion AutoSetFaktur

      #region Restore database name to TheDbConnection

      conn.ChangeDatabase(orig_dbName);

      #endregion Restore database name to TheDbConnection

      return kontra_faktur_T2_URA_rec.TtNum;
   }

   private void RISK_StornoSVD_ZAH(object sender, EventArgs e)
   {
      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec                 ;

      if(faktur_rec.StatusCD == "D")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već izdana i ne može se stornirati");
         return;
      }
      if(faktur_rec.StatusCD == "P")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je predložak i ne može se stornirati");
         return;
      }

      BeginEdit(theDUC.faktur_rec);
      
      faktur_rec.StatusCD = "S";
      
      bool rwtOK = faktur_rec.VvDao.RWTREC( TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 
      
      EndEdit(faktur_rec);
      
      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 
    
   }

   private void RISK_ListaNEIzdanih_SVD_ZAH(object sender, EventArgs e)
   {
      OpenNew_RecLst_TabPage(GetSubModulXY(ZXC.VvSubModulEnum.LsFAK), null);

      FakturListUC fakListUC = ZXC.TheVvForm.TheVvRecLstUC as FakturListUC;
      fakListUC.Fld_FromTT       = Faktur.TT_ZAH;
      fakListUC.Fld_FilterTT     = Faktur.TT_ZAH;
      fakListUC.Fld_FilterStatus = "N"; // kako ćemo ovdje staviti i I djelomične!!!

      VvHamper.Open_Close_Fields_ForWriting(fakListUC.tbx_filterTT, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      VvHamper.Open_Close_Fields_ForWriting(fakListUC.tbx_TT      , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);

      if(ZXC.IsSvDUH_ZAHonly)
      {
         Kupdob kupdob_rec = fakListUC.Get_Kupdob_FromVvUcSifrar_byTicker(ZXC.CURR_userName);

         fakListUC.tbx_filtPartnerTick.Text = ZXC.CURR_userName;
         fakListUC.tbx_filtPartnerCD.Text = kupdob_rec.KupdobCD.ToString("000000");
         fakListUC.tbx_filtPartnerName.Text = kupdob_rec.Naziv;

         VvHamper.Open_Close_Fields_ForWriting(fakListUC.tbx_filtPartnerTick, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(fakListUC.tbx_filtPartnerCD  , ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
         VvHamper.Open_Close_Fields_ForWriting(fakListUC.tbx_filtPartnerName, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvFindDialog);
      }

      fakListUC.button_GO.PerformClick();
   }

   // Odobri i posalji u apoteku 
   // smije raditi samo na C statusu 
   private void RISK_SendToApoteka_SVD_ZAH(object sender, EventArgs e)
   {
      if(!ZXC.IsSvDUH_ZAHonly) return;

      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec;

      if(faktur_rec.StatusCD == "D")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je zatvorena - u cijelosti izdana");
         return;
      }
      if(faktur_rec.StatusCD == "N")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već poslana u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "S")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je stornirana i ne može se poslati u apoteku. Zahtjevnicu možete kopirati pa novu poslati u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "P")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je predložak i ne može se poslati u apoteku. Zahtjevnicu možete kopirati pa kao novu poslati u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "O")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica još nije završena");
         return;
      }
      if(faktur_rec.StatusCD != "C")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica nije spremna za slanje?"); // ovo je nama kontrola 
         return;
      }

      ZXC.SVD_PotrosnjaInfo SVD_PotrosnjaInfo = RtransDao.Get_SVD_PotrosnjaInfo(TheDbConnection, Faktur.TT_IZD, TheVvUC.Get_Kupdob_FromVvUcSifrar((TheVvDocumentRecordUC as FakturDUC).faktur_rec.KupdobCD), DateTime.MinValue, DateTime.Now, VvUserControl.KupdobSifrar, false);

      SVD_PotrosnjaInfoDlg potrDlg = new SVD_PotrosnjaInfoDlg(SVD_PotrosnjaInfo);

      if(potrDlg.ShowDialog() != DialogResult.OK)
      {
         potrDlg.Dispose();
         return;
      }
      potrDlg.Dispose();

      SVD_Odobrenje_Dlg odbDlg = new SVD_Odobrenje_Dlg(faktur_rec.KupdobTK.SubstringSafe(0, 1));

      if(odbDlg.ShowDialog() != DialogResult.OK)
      {
         odbDlg.Dispose();
         return;
      }

      if(Check_SVDZAH_OdobrenjeCredentials(faktur_rec.KupdobTK, odbDlg.Fld_UserName, odbDlg.Fld_Password) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Pogrešan password i/ili username ovlaštene osobe za odobravanje zahtjevnice.");
         return;
      }

      // here we go ... username & password are OK. 

      odbDlg.ComboBoxUser_AddEventuallyNewDataSourceMember();

      string odobrioFullName;
      User user_rec = TheVvUC.Get_User_FromVvUcSifrar(odbDlg.Fld_UserName);
      odbDlg.Dispose();

    //11.10.2022.
    //if(user_rec != null) odobrioFullName = user_rec.Email + " " + user_rec.Ime     + " " + user_rec.Prezime;
      if(user_rec != null) odobrioFullName = user_rec.Ime   + " " + user_rec.Prezime + ", " + user_rec.Email;
      else                 odobrioFullName = "";

      BeginEdit(theDUC.faktur_rec);

    //faktur_rec.OdgvPersName = ZXC.LenLimitedStr(odobrioFullName, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.odgvPersName));
      faktur_rec.Napomena2    = ZXC.LenLimitedStr(odobrioFullName, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.napomena2));

      faktur_rec.StatusCD = "N";

      bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

      EndEdit(faktur_rec);

      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

   }

   internal bool Check_SVDZAH_OdobrenjeCredentials(string kupdobTK, string _userName, string _password)
   {
      // mora se pofudarati prvo slovo tickera i usernamea (znaci issta-odgovarajuca klinika/zavod) 
      if(kupdobTK.SubstringSafe(0, 1) != _userName.SubstringSafe(0, 1)) return false;

      // da su prva tri sliova isat 
      if(_userName.SubstringSafe(0, 1) != _userName.SubstringSafe(1, 1) ||
         _userName.SubstringSafe(0, 1) != _userName.SubstringSafe(2, 1)) return false;

      User user_rec = VvUserControl.UserSifrar.SingleOrDefault(user => user.UserName == _userName);

      if(user_rec == null) return false;

      return user_rec.PasswdDecrypted.Trim('\0') == _password;
   }

   // Oznaci kao zavrsenu i posalji na cekanje odobrenja 
   // smije raditi samo na O statusu 
   private void RISK_ZavrsenaPaSaljiNaOdobrenje_SVD_ZAH(object sender, EventArgs e)
   {
      if(!ZXC.IsSvDUH_ZAHonly) return;

      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;
      Faktur faktur_rec = theDUC.faktur_rec;

      if(faktur_rec.StatusCD == "D")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je zatvorena - u cijelosti izdana");
         return;
      }
      if(faktur_rec.StatusCD == "N")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već poslana u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "S")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je stornirana i ne može se poslati u apoteku. Zajthjevnicu možete kopirati pa novu poslati u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "P")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je predložak i ne može se poslati u apoteku. Zajthjevnicu možete kopirati pa kao novu poslati u apoteku");
         return;
      }
      if(faktur_rec.StatusCD == "C")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već na čekanju za odobrenje");
         return;
      }
      if(faktur_rec.StatusCD != "O")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica nije završena?"); // ovo je nama kontrola 
         return;
      }

      BeginEdit(theDUC.faktur_rec);

      faktur_rec.StatusCD = "C";

      bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

      EndEdit(faktur_rec);

      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

   }

   private Faktur Get_SVD_ZAHpredlozak(bool _shouldBeSilent)
   {
      if(!ZXC.IsSvDUH_ZAHonly) return null;

      uint ZAHpredNultiTtNum = ZXC.ValOrZero_UInt(ZXC.CURR_user_rec.Oib) * 10000;

      Faktur faktur_rec_ZAHnulti = new Faktur();

      FakturDao.SetMeFaktur(TheDbConnection, faktur_rec_ZAHnulti, Faktur.TT_ZAH, ZAHpredNultiTtNum, _shouldBeSilent);

      return faktur_rec_ZAHnulti;
   }

   private void RISK_UrediPredlozak_ZAH(object sender, EventArgs e)
   {
      Faktur faktur_rec_ZAHnulti = Get_SVD_ZAHpredlozak(true);

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC                     ;

      if(faktur_rec_ZAHnulti == null || faktur_rec_ZAHnulti.TtNum.IsZero()) // 'Novi' - ADDREC Faktur 
      {
         NewRecord_OnClick(null, EventArgs.Empty);

         uint ZAHpredNultiTtNum = ZXC.ValOrZero_UInt(ZXC.CURR_user_rec.Oib) * 10000;

         theDUC.PutAllKupdobFields(ZXC.SvDUH_ZAHonlyKupdob_rec);

         theDUC.Fld_TT       = Faktur.TT_ZAH;
         theDUC.Fld_TtNum    = ZAHpredNultiTtNum;
         theDUC.Fld_StatusCD = "P";
         theDUC.Fld_DokDate  = ZXC.projectYearFirstDay;
      }
      else // 'Ispravi' - RWTREC Faktur 
      {
         TheVvDataRecord = faktur_rec_ZAHnulti;

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

         EditRecord_OnClick(null, EventArgs.Empty);
      }
   }

   private void RISK_NovaIzPredloska_ZAH(object sender, EventArgs e)
   {
      if(!ZXC.IsSvDUH_ZAHonly) return;

      Faktur faktur_rec_ZAHnulti = Get_SVD_ZAHpredlozak(true);

      if(faktur_rec_ZAHnulti == null || faktur_rec_ZAHnulti.TtNum.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema predloška. Trebate ga tek kreirati.");

         return;
      }

      TheVvDataRecord = faktur_rec_ZAHnulti;

      //if((TheVvDataRecord as Faktur).SkladCD.IsEmpty()) (TheVvDataRecord as Faktur).SkladCD = "10";

      PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

      DupCopyMenu_Button_OnClick(null, EventArgs.Empty);
   }

   // Oznaci kao zatvorenu           
   // smije raditi samo na N statusu 
   private void RISK_RucnoZatvori_SVD_ZAH(object sender, EventArgs e)
   {
      if(ZXC.IsSvDUH_ZAHonly)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Nedozvoljena operacija.");
         return;
      }

      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec                 ;

      if(faktur_rec.StatusCD == "D")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je već zatvorena");
         return;
      }
      if(faktur_rec.StatusCD == "O")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je još u izradi");
         return;
      }
      if(faktur_rec.StatusCD == "S")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica je stornirana");
         return;
      }
      if(faktur_rec.StatusCD == "P")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova zahtjevnica je predložak");
         return;
      }
      if(faktur_rec.StatusCD == "C")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica još nije odobrena");
         return;
      }

      if(faktur_rec.StatusCD != "N")
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Zahtjevnica nije spremna za zatvaranje?"); // ovo je nama kontrola 
         return;
      }

      BeginEdit(theDUC.faktur_rec);

      faktur_rec.StatusCD = "D";

      bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

      EndEdit(faktur_rec);

      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 
   }

   private void RISK_FindArtiklFromExternName(object sender, EventArgs e)
   { }

   private void RISK_SVD_INV(object sender, EventArgs e)
   {
      #region init

      if(!ZXC.IsSvDUH) return;

      SVD_INV_Dlg dlg = new SVD_INV_Dlg();

      dlg.Fld_InventuraDate = DateTime.Today;
      dlg.Fld_MaxPageSize   = 500;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      DateTime invDate = dlg.Fld_InventuraDate;

      int maxPageSize = dlg.Fld_MaxPageSize; 

      dlg.Dispose();

      int currStartIdx;
      int currLeftover;
      int currPageSize;

      string currSklCD;
      string currGrpCD;

      List<Artikl> currentList;
      List<Artikl> currentRange;

      int    debugCount = 0;

      List<Artikl> art_10____list = new List<Artikl>();
      List<Artikl> art_20____list = new List<Artikl>();
      List<Artikl> art_22____list = new List<Artikl>();
      List<Artikl> art_24____list = new List<Artikl>();
      List<Artikl> art_30____list = new List<Artikl>();
      List<Artikl> art_40____list = new List<Artikl>();
      List<Artikl> art_50____list = new List<Artikl>();
      List<Artikl> art_80____list = new List<Artikl>();

      List<Artikl> art_10_00_list;
      List<Artikl> art_10_10_list;
      List<Artikl> art_10_20_list;
      List<Artikl> art_10_30_list;
      List<Artikl> art_10_40_list;
      List<Artikl> art_10_50_list;
      List<Artikl> art_10_60_list;
      List<Artikl> art_10_70_list;
      List<Artikl> art_10_80_list;
      List<Artikl> art_10_90_list;
      List<Artikl> art_10_N0_list;
      List<Artikl> art_10_A0_list;           
      List<Artikl> art_10_LP_list;

      #endregion init

      #region Get theArtiklWithArtstatList

      DataRowCollection   ArsSch = ZXC.ArtStatSchemaRows; 
      ArtStatDao.ArtStatCI AstCI = ZXC.AstCI            ;
      VvRpt_RiSk_Filter rptFilter = new VvRpt_RiSk_Filter();

      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_10____list, "10", invDate, rptFilter , "", "artiklName "); art_10____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_20____list, "20", invDate, rptFilter , "", "artiklName "); art_20____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_22____list, "22", invDate, rptFilter , "", "artiklName "); art_22____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_24____list, "24", invDate, rptFilter , "", "artiklName "); art_24____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_30____list, "30", invDate, rptFilter , "", "artiklName "); art_30____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_40____list, "40", invDate, rptFilter , "", "artiklName "); art_40____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_50____list, "50", invDate, rptFilter , "", "artiklName "); art_50____list.RemoveAll(art => art.AS_HasUselessPST);
      ArtiklDao.GetArtiklWithArtstatList(TheDbConnection, art_80____list, "80", invDate, rptFilter , "", "artiklName "); art_80____list.RemoveAll(art => art.AS_HasUselessPST);

      art_10_00_list = art_10____list.Where(art => art.Grupa1CD == "00").ToList();
      art_10_10_list = art_10____list.Where(art => art.Grupa1CD == "10").ToList();
      art_10_20_list = art_10____list.Where(art => art.Grupa1CD == "20").ToList();
      art_10_30_list = art_10____list.Where(art => art.Grupa1CD == "30").ToList();
      art_10_40_list = art_10____list.Where(art => art.Grupa1CD == "40").ToList();
      art_10_50_list = art_10____list.Where(art => art.Grupa1CD == "50").ToList();
      art_10_60_list = art_10____list.Where(art => art.Grupa1CD == "60").ToList();
      art_10_70_list = art_10____list.Where(art => art.Grupa1CD == "70").ToList();
      art_10_80_list = art_10____list.Where(art => art.Grupa1CD == "80").ToList();
      art_10_90_list = art_10____list.Where(art => art.Grupa1CD == "90").ToList();
      art_10_N0_list = art_10____list.Where(art => art.Grupa1CD == "N0").ToList();
      art_10_A0_list = art_10____list.Where(art => art.Grupa1CD == "A0").ToList();
      art_10_LP_list = art_10____list.Where(art => art.Grupa1CD == "LP").ToList();

      #endregion Get theArtiklWithArtstatList

      #region AutoAdd_SVD_INV_DokumentS

      // 10 00 ################################################################################################################################################## 

      currentList  = art_10_00_list; // variraj 
      currSklCD    = "10"          ; // variraj 
      currGrpCD    =       "00"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 10 ################################################################################################################################################## 
      currentList    = art_10_10_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "10"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 20 ################################################################################################################################################## 
      currentList    = art_10_20_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "20"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 30 ################################################################################################################################################## 
      currentList    = art_10_30_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "30"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 40 ################################################################################################################################################## 
      currentList    = art_10_40_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "40"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      
      // 10 50 ################################################################################################################################################## 
      currentList    = art_10_50_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "50"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      
      // 10 60 ################################################################################################################################################## 
      currentList    = art_10_60_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "60"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      
      // 10 70 ################################################################################################################################################## 
      currentList    = art_10_70_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "70"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      // 10 80 ################################################################################################################################################## 
      currentList    = art_10_80_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "80"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      
      // 10 90 ################################################################################################################################################## 
      currentList    = art_10_90_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "90"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
      
      // 10 N0 ################################################################################################################################################## 
      currentList    = art_10_N0_list; // variraj 
      currSklCD      = "10"          ; // variraj 
      currGrpCD      =       "N0"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 A0 ################################################################################################################################################## 

      currentList  = art_10_A0_list; // variraj 
      currSklCD    = "10"          ; // variraj 
      currGrpCD    =       "A0"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 10 LP ################################################################################################################################################## 

      currentList  = art_10_LP_list; // variraj 
      currSklCD    = "10"          ; // variraj 
      currGrpCD    =       "LP"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 20    ################################################################################################################################################## 

      currentList  = art_20____list; // variraj 
      currSklCD    = "20"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 22    ################################################################################################################################################## 

      currentList  = art_22____list; // variraj 
      currSklCD    = "22"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 24    ################################################################################################################################################## 

      currentList  = art_24____list; // variraj 
      currSklCD    = "24"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }

      // 30    ################################################################################################################################################## 

      currentList  = art_30____list; // variraj 
      currSklCD    = "30"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
     
      // 40    ################################################################################################################################################## 

      currentList  = art_40____list; // variraj 
      currSklCD    = "40"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
     
      // 50    ################################################################################################################################################## 

      currentList  = art_50____list; // variraj 
      currSklCD    = "50"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }
   
      // 80    ################################################################################################################################################## 

      currentList  = art_80____list; // variraj 
      currSklCD    = "80"          ; // variraj 
      currGrpCD    =       "--"    ; // variraj 
      currLeftover = currentList.Count;
      currStartIdx = 0;
      currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;

      while(currLeftover.NotZero())
      {
         currentRange = currentList.GetRange(currStartIdx, currPageSize);
         AutoAdd_SVD_INV_Dokument(TheDbConnection, invDate, currSklCD, currGrpCD, currentRange, currStartIdx, currPageSize); debugCount++;
         currLeftover -= currPageSize;
         currStartIdx += currPageSize;
         currPageSize = maxPageSize > currLeftover ? currLeftover : maxPageSize;
      }


      #endregion AutoAdd_SVD_INV_DokumentS

      #region Finish

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg("Gotovo. Kreirao {0} INV dokumenta.", debugCount);

      #endregion Finish

   }

   private void AutoAdd_SVD_INV_Dokument(XSqlConnection _theDbConnection, DateTime invDate, string skladCD, string groupCD, List<Artikl> artiklList, int currStartIdx, int currPageSize)
   {
      string napomena = skladCD + "-" + groupCD + " " + ZXC.LenLimitedStr(artiklList.First().ArtiklName, 12) + "-" + ZXC.LenLimitedStr(artiklList.Last().ArtiklName, 12);

      napomena = ZXC.LenLimitedStr(napomena, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena));

      #region Set faktur_rec data

      Faktur faktur_rec = new Faktur();
      
      faktur_rec.TT         = Faktur.TT_INV                   ;
      faktur_rec.TtSort     = ZXC.TtInfo(faktur_rec.TT).TtSort;
      faktur_rec.SkladDate  =
      faktur_rec.DokDate    = invDate                         ;
      faktur_rec.SkladCD    = skladCD                         ;
      faktur_rec.DokNum     = faktur_rec.VvDao.GetNextDokNum(_theDbConnection, Faktur.recordName);
      faktur_rec.TtNum      = faktur_rec.VvDao.GetNextTtNum (_theDbConnection, faktur_rec.TT, faktur_rec.SkladCD);
      faktur_rec.Napomena   = napomena                        ;

      #endregion Set faktur_rec Data

      #region Set rtrans_rec Data - (foreach artikl)

      Rtrans rtrans_rec;

      ushort line = 0;

      foreach(Artikl artikl_rec in artiklList)
      {
         rtrans_rec = new Rtrans();

         rtrans_rec.T_serial     = ++line;

         rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD    ;
         rtrans_rec.T_artiklName = artikl_rec.ArtiklName  ;
         rtrans_rec.T_jedMj      = artikl_rec.JedMj       ;
         rtrans_rec.T_kol        = artikl_rec.AS_StanjeKol;
         rtrans_rec.T_cij        = artikl_rec.AS_PrNabCij ;
         rtrans_rec.T_TT         = faktur_rec.TT          ; // !!! 
         rtrans_rec.T_ttNum      = faktur_rec.TtNum       ;
         rtrans_rec.T_ttSort     = faktur_rec.TtSort      ;
         rtrans_rec.T_skladCD    = faktur_rec.SkladCD     ;
       //rtrans_rec.T_parentID   = faktur_rec.RecID; ... ovoga automatski postavi VvDaoBase.AddrecDocument_AddrecTranses() 
         rtrans_rec.T_dokNum     = faktur_rec.DokNum      ;
         rtrans_rec.T_skladDate  = faktur_rec.DokDate     ;
         rtrans_rec.T_kupdobCD   = faktur_rec.KupdobCD    ;
         rtrans_rec.CalcTransResults(null) ;
         faktur_rec.Transes.Add(rtrans_rec);
      }

      #endregion Set rtrans_rec Data - ZELENI redci

      #region TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

      faktur_rec.TakeTransesSumToDokumentSum(true);

      // NOTA BENE: ovdje ide pravi ADDREC (fak + Transes) a ne AutoSetFaktur koji ne zna sume prije nego sto doda sve transove 
      bool OK = faktur_rec.VvDao.ADDREC(_theDbConnection, faktur_rec);

      #endregion TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

   }

   private void RISK_INV_PORAVNANJE(object sender, EventArgs e)
   {
      #region init

      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    DUCfaktur_rec = theDUC.faktur_rec              ;

      VvDataRecord arhivedINVfaktur_rec;

      INV_Poravnanje_Dlg dlg = new INV_Poravnanje_Dlg();

      dlg.Fld_InventuraDate_real   = DUCfaktur_rec.DokDate;
      dlg.Fld_InventuraDate_wanted = DateTime.Today       ;

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      DateTime invDate_real   = dlg.Fld_InventuraDate_real  ;
      DateTime invDate_wanted = dlg.Fld_InventuraDate_wanted;

      dlg.Dispose();

      decimal prmRazdobKOL     = 0.00M;
      decimal zatecenaInvKOL   = 0.00M;
      decimal korigiranaInvKOL = 0.00M;

      bool fakturTouched, rwtOK;

      int touchedFAKcount = 0;
      int touchedRTRcount = 0;

      #endregion init

      #region Get INVfakturList

      List<Faktur>            INVfakturList    = new List<Faktur>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt     ], "theTT"     , Faktur.TT_INV, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "theDokDate", invDate_real , " = " ));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(TheDbConnection, INVfakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      INVfakturList.ForEach(fak => fak.VvDao.LoadTranses(TheDbConnection, fak, false));

      #endregion Get INVfakturList

      foreach(Faktur faktur_rec in INVfakturList)
      {
         fakturTouched = false;
         rwtOK = true;

         BeginEdit(faktur_rec);

         arhivedINVfaktur_rec = faktur_rec.CreateArhivedDataRecord(TheDbConnection, "INVporav");

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            prmRazdobKOL = 0.00M;

            zatecenaInvKOL   = rtrans_rec.T_kol;
            prmRazdobKOL     = Get_KOL_SaldoRazdoblja(TheDbConnection, rtrans_rec.T_artiklCD, rtrans_rec.T_skladCD, invDate_real, invDate_wanted);
            korigiranaInvKOL = zatecenaInvKOL + prmRazdobKOL;

            rtrans_rec.SaveTransesWriteMode = ZXC.WriteMode.Edit; // !!! 

            rtrans_rec.T_skladDate = invDate_wanted;

            if(prmRazdobKOL.NotZero())
            {
               fakturTouched = true;
               
               touchedRTRcount++;

               rtrans_rec.T_kol = korigiranaInvKOL;

               rtrans_rec.CalcTransResults(faktur_rec);

            }

         } // foreach(Rtrans INVrtrans_rec in INVfaktur_rec.Transes)

         if(/*fakturTouched*/true) // olvejz, zbog DokDate-a 
         {
            touchedFAKcount++;

            faktur_rec.DokDate = invDate_wanted;

            faktur_rec.TakeTransesSumToDokumentSum(true);

            rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, false, false, false, false);

            arhivedINVfaktur_rec.VvDao.ADDREC(TheDbConnection, arhivedINVfaktur_rec, true, true, false, false);
         }

         EndEdit(faktur_rec);

      } // foreach(Faktur INVfaktur_rec in INVfakturList)   

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg("Gotovo. Korigirao {0} INV fakturs, {1} INV rtranses.", touchedFAKcount, touchedRTRcount);

   } // private void RISK_INV_PORAVNANJE(object sender, EventArgs e) 

   #endregion SVD

   // Selmet to ask:                                                          
   // 1. u fajlu iscitanja bi nam trebao i ArtiklCD (osim barcodea i kolicine) 
   // 
   // 2. BUDUCI DA klijent tvrdi da IMA DO 4 BARKODA, MORAMO RASTAVITI ARTIKL NA VISE RADOVA ZA ISTI ARTIKL
   // 
   // 3. duplicate handling (barkod ukazuje na vise od 1 artikla) - SELMET rjesava?  
   // 
   // 4. barkod nije pronadjen. A - tom artiklu je BK prazan, B - ocitan je krivi BK na ambalazi
   //    -||- A - moze li se upariti BK na terminalu? B - 
   // 
   // 5. on init: TextBox za numeric (ttSort + ttNum) max 9 znakova; dvojaki unos - skeniranje ili tastatura 
   // 
   // 6. izlazni fajl: „VvBCS_numeric.txt“
   // 
   // 6. moze li scaner iscitavati sa monitora (ekrana)?                        
   // 
   // 7. web servis !!!
   // 
   // ArtiklCD;3425901018201;18
   // ArtiklCD;3425901019314;6
   // ArtiklCD;3425901019260;18
   // ArtiklCD;3859888346420;44
   // ArtiklCD;qwe;55

   private void RISK_BarCode_Check(object sender, EventArgs e)
   {
      string  barcode  = ""    ;
      string  artiklCD = ""    ;
      uint    ttNum    = 0     ;
      decimal kol      = 0.00M ;
      int     rIdx             ;
      uint    imgRecID =      1; // imaginary RecID 
      string  rowInfo, msgStr  ;

      FakturDUC theDUC = TheVvDocumentRecordUC as FakturDUC;

      // !!! 
      FakturDao.Set_BarcodeFile_VvNames();

      // !!! 
      List<ZXC.VvUtilDataPackage> barcodeWkolList = FakturDao.Get_BarcodeFile_Content_ForDokument(theDUC.faktur_rec.TT, theDUC.faktur_rec.TtNum);

      if(barcodeWkolList.IsEmpty()) return;

      Artikl artikl_rec;
      Rtrans rtrans_rec;
      Rtrans rtrans_rec2;

      List<Rtrans> rtransList = theDUC.faktur_rec.Transes.ToList();

      foreach(Rtrans rtr in rtransList) // add barKod 1 & 2 to all rtranses (as 'R_utilString') 
      {
         artikl_rec = theDUC.Get_Artikl_FromVvUcSifrar(rtr.T_artiklCD);

         if(artikl_rec != null) rtr.R_utilString = artikl_rec.BarCode1 + artikl_rec.BarCode2;
         else                   rtr.R_utilString = ""                                       ;
      }

      ZXC.ErrorsList = new List<string>()                                  ; // ili / ili 
      List<VvReportSourceUtil> messageList = new List<VvReportSourceUtil>(); // ili / ili 

    //VvMessageBoxDLG  barcodesStatusInfoForm = new VvMessageBoxDLG (false);
      VvMessageBoxForm barcodesStatusInfoForm = new VvMessageBoxForm(false, ZXC.VvmBoxKind.BarCodeInfo);
      barcodesStatusInfoForm.Text = "STATUSI OČITANJA:";

      foreach(ZXC.VvUtilDataPackage barcodeWkol in barcodeWkolList)
      {
         ttNum    = barcodeWkol.TheUint   ;
         artiklCD = barcodeWkol.TheStr1   ;
         barcode  = barcodeWkol.TheStr2   ;
         kol      = barcodeWkol.TheDecimal;

         if(barcode.IsEmpty()) continue;

       //rtrans_rec  = rtransList.SingleOrDefault(r => r.R_utilString       == barcode );
         rtrans_rec  = rtransList.SingleOrDefault(r => r.R_utilString.Contains(barcode));
         rtrans_rec2 = rtransList.SingleOrDefault(r => r.T_artiklCD         == artiklCD); // da vidimo treba li nam RWTREC Artikl za novopridruzeni barkod 
         rIdx        = rtransList.IndexOf(rtrans_rec);

         #region RWTREC Artikl za novopridruzeni barkod

         if(rtrans_rec == null && rtrans_rec2 != null) // nema ga po barkodu a ima ga po artiklCD 
         {
            bool goRWT = true, rwtOK = false;

            Artikl artikl_rec4RWT = theDUC.Get_Artikl_FromVvUcSifrar(artiklCD);

            if(artikl_rec4RWT != null)
            {
               BeginEdit(artikl_rec4RWT);

                    if(artikl_rec4RWT.BarCode1.IsEmpty()) artikl_rec4RWT.BarCode1 = barcode;
               else if(artikl_rec4RWT.BarCode2.IsEmpty()) artikl_rec4RWT.BarCode2 = barcode;
               else { goRWT = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "Artikl [{0}]\n\nNEMA VIŠE MJESTA  ZA UPISATI NOVOUPARENI BARKOD\n\n[{1}]", artiklCD, barcode); }

               if(goRWT)
               {
                  rwtOK = artikl_rec4RWT.VvDao.RWTREC(TheDbConnection, artikl_rec4RWT);
               }

               EndEdit(artikl_rec4RWT);

               if(rwtOK)
               {
                  rtrans_rec = rtrans_rec2.MakeDeepCopy();
                  rtrans_rec.R_utilString = artikl_rec4RWT.BarCode1 + artikl_rec4RWT.BarCode2;
                  rIdx       = rtransList.IndexOf(rtrans_rec2);
               }
            }

         } // if(rtrans_rec == null && rtrans_rec2 != null) // nema ga po barkodu a ima ga po artiklCD 

         #endregion RWTREC Artikl za novopridruzeni barkod

         VvReportSourceUtil message = new VvReportSourceUtil();
         message.TheCD = barcode  ;
         message.Kol   = kol      ;
         message.Count = rIdx + 1 ;

         if(rtrans_rec != null && rtrans_rec.R_utilString.NotEmpty()) // nasao je barKod u listi 
         {
            rtrans_rec.T_kol -= kol;

            if(rIdx.IsNegative()) rowInfo = "";
            else                  rowInfo = " red " + (rIdx + 1).ToString() + ". (skenirano " + kol.ToStringVv() + " kom. artikla [" + rtrans_rec.T_artiklCD + " " + rtrans_rec.T_artiklName + "]) ";

            message.ArtiklGrCD   = rtrans_rec.T_artiklCD  ;
            message.ArtiklGrName = rtrans_rec.T_artiklName;

            //message.KupdobName   = rowInfo;

            if(rtrans_rec.T_kol.IsZero())
            {
               msgStr = "KOMPLETNO provjereno";
               message.KupdobName = msgStr;
               message.DevName    = "OK";

               ZXC.ErrorsList.Add(barcode + " OK --->"  + rowInfo + msgStr);
               theDUC.PutDgvLineFields1(new Rtrans() { RtrResults = new RtransResultStruct() }, rIdx, true); // ocisti redak DUC-a 
            }
            else if(rtrans_rec.T_kol.IsPositive())
            {
               msgStr = "još provjeriti: " + rtrans_rec.T_kol.ToStringVv() + " komada";
               message.KupdobName = msgStr;
               message.DevName    = "NEPOTPUN";

               ZXC.ErrorsList.Add(barcode + " OK --->"  + rowInfo +  msgStr);
               theDUC.PutDgvLineFields1(rtrans_rec, rIdx, true);
            }
            else if(rtrans_rec.T_kol.IsNegative())
            {
               msgStr = rtrans_rec.T_kol.ToStringVv() + (rtrans_rec.R_utilBool ? " Na ovom dokumentu nema artikla sa takvim barkodom!" : " odlazimo u MINUS, previše je komada na provjeri!");
               message.KupdobName = msgStr;
               message.DevName    = rtrans_rec.R_utilBool ? "NEMA GA" : "MINUS";

               ZXC.ErrorsList.Add(barcode + " ERR --->" + rowInfo + msgStr);
               theDUC.PutDgvLineFields1(rtrans_rec, rIdx, true);
            }
         } // if(rtrans_rec != null && rtrans_rec.R_utilString.NotEmpty()) // nasao je barKod u listi 
         else // nema barKoda na DUC-u 
         {
            artikl_rec = theDUC.Get_Artikl_FromVvUcSifrarBC1(barcode);

            if(artikl_rec == null) // nema ga u artikl sifariku 
            {
               msgStr          = "Artikl ne postoji u Vektorovoj bazi artikala";
               message.DevName = "NEPOZNAT";
            }
            else // ima ga u sifarniku ali nije na dokumentu 
            {
               msgStr          = "Na dokumentu nema ovog artikla";
               message.DevName = "NEMA GA";

               message.ArtiklGrCD   = artikl_rec.ArtiklCD  ;
               message.ArtiklGrName = artikl_rec.ArtiklName;
            }

            message.KupdobName = msgStr;

            if(artikl_rec != null) rowInfo = " (skenirano " + kol.ToStringVv() + " kom. artikla [" + artikl_rec.ArtiklCD + " " + artikl_rec.ArtiklName + "] ";
            else                   rowInfo = " (skenirano " + kol.ToStringVv() + " kom. artikla [NEPOZNAT - NEPOSTOJEĆI ARTIKL] ";
            ZXC.ErrorsList.Add(barcode + " ERR --->" + rowInfo + msgStr);

            Rtrans newRtrans_rec       = new Rtrans();
            newRtrans_rec.T_artiklName = rowInfo   ;
            newRtrans_rec.T_artiklCD   = barcode    ;
            newRtrans_rec.R_utilString = barcode    ;
            newRtrans_rec.R_utilBool   = true      ; // flag da znamo da je nepostojeci na dokumentu tj. umjetno dodani rtrans rtransList-i +
            newRtrans_rec.T_kol       -= kol       ;
            newRtrans_rec.T_recID      = imgRecID++; // izmisljeni - unikatni - za Comparera 
            rtransList.Add(newRtrans_rec);

            int idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);
            theDUC.TheG.Rows.Add();
            int rowIdx = theDUC.TheG.RowCount - idxCorrector;

            theDUC.PutDgvLineFields1(newRtrans_rec, rowIdx, true);
         }

         messageList.Add(message);

      } // foreach(ZXC.NameAndDecimal_CommonStruct barcodeWkol in barcodeWkolList) 

      barcodesStatusInfoForm.Show/*Dialog*/(this);

    //barcodesStatusInfoForm.TheUC.PutDgvFields(ZXC.ErrorsList);
      barcodesStatusInfoForm.TheUC.PutDgvFields_BarCodeInfo(messageList);

      //ReRead_OnClick(null, EventArgs.Empty); // refresh record 

      ZXC.ErrorsList = null;
   }

   private void RISK_Create_IFA_onPdf417(object sender, EventArgs e)
   {
      #region Init & Get Virman data

      Vv_PDF417_Dlg dlg = new Vv_PDF417_Dlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      //========================================================================= 

      VirmanStruct virman = new VirmanStruct();

      virman.Money   = (dlg.Fld_Lines.Length >  2 ? ZXC.ValOrZero_Decimal(dlg.Fld_Lines[ 2], 2) : 0M); // string fld_IZNOS
      virman.Money  /= 100M;

      virman.Plat1   = (dlg.Fld_Lines.Length >  3 ?                       dlg.Fld_Lines[ 3]     : ""); // string fld_PLAT1    //ZXC.CURR_prjkt_rec.Naziv       ;
      virman.Plat2   = (dlg.Fld_Lines.Length >  4 ?                       dlg.Fld_Lines[ 4]     : ""); // string fld_PLAT2    //ZXC.CURR_prjkt_rec.Ulica1      ;
      virman.Plat3   = (dlg.Fld_Lines.Length >  5 ?                       dlg.Fld_Lines[ 5]     : ""); // string fld_PLAT3    //ZXC.CURR_prjkt_rec.ZipAndMjesto;
      virman.Prim1   = (dlg.Fld_Lines.Length >  6 ?                       dlg.Fld_Lines[ 6]     : ""); // string fld_PRIM1    
      virman.Prim2   = (dlg.Fld_Lines.Length >  7 ?                       dlg.Fld_Lines[ 7]     : ""); // string fld_PRIM2    
      virman.Prim3   = (dlg.Fld_Lines.Length >  8 ?                       dlg.Fld_Lines[ 8]     : ""); // string fld_PRIM3    
      virman.Ziro2   = (dlg.Fld_Lines.Length >  9 ?                       dlg.Fld_Lines[ 9]     : ""); // string fld_PRIM_IBAN
      virman.PnboMod = (dlg.Fld_Lines.Length > 10 ?                       dlg.Fld_Lines[10]     : ""); // string fld_PRIM_MOD 
      virman.Pnbo    = (dlg.Fld_Lines.Length > 11 ?                       dlg.Fld_Lines[11]     : ""); // string fld_PRIM_PNB 
      virman.SifraPl = (dlg.Fld_Lines.Length > 12 ?                       dlg.Fld_Lines[12]     : ""); // string fld_SIF_NAMJ
      virman.OpisPl  = (dlg.Fld_Lines.Length > 13 ?                       dlg.Fld_Lines[13]     : ""); // string fld_OPIS     

      //========================================================================= 

      dlg.Dispose();

      #endregion Init & Get Virman data

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None)
      {
         NewRecord_OnClick(null, EventArgs.Empty);
      }

      FakturExtDUC theDUC = TheVvUC as FakturExtDUC;

      theDUC.Fld_KupdobName = virman.Plat1;
      theDUC.Fld_VezniDok   = virman.Pnbo ;

      decimal pdvSt  = Faktur.CommonPdvStForThisDate(DateTime.Today);

      decimal theCij = ZXC.VvGet_100_from_125(virman.Money, pdvSt);

      theDUC.TheG.Rows.Add();

      theDUC.TheG.PutCell(theDUC.DgvCI.iT_artiklName, 0, virman.OpisPl);
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_kol       , 0, 1.00M        );
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_cij       , 0, theCij       );
      theDUC.TheG.PutCell(theDUC.DgvCI.iT_pdvSt     , 0, pdvSt        );

      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(0);
   }

   #region PCTOGO

   #region PTG_CreateNew_AUN_DOD_

   private void RISK_PTG_KUG_to_AUN (object sender, EventArgs e)  { PTG_CreateNew_AUN_DOD(ZXC.VvSubModulEnum.R_ANU_PTG); }
   private void RISK_PTG_UGAN_to_DIZ(object sender, EventArgs e)  { PTG_CreateNew_AUN_DOD(ZXC.VvSubModulEnum.R_DOD_PTG); }
   private void RISK_PTG_UGAN_to_PVR(object sender, EventArgs e)  { PTG_CreateNew_AUN_DOD(ZXC.VvSubModulEnum.R_PVR_PTG); }
   private void RISK_PTG_UGAN_to_ZIZ(object sender, EventArgs e)  { PTG_CreateNew_AUN_DOD(ZXC.VvSubModulEnum.R_ZIZ_PTG); }

   private void PTG_CreateNew_AUN_DOD(ZXC.VvSubModulEnum vvSubModulEnum)
   {
      #region RISK_CopyToSomeOtherTT - procisceni

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(vvSubModulEnum);

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete()); // trebamo ga za PutDefaultDUCfields 

      //ZXC.RISK_CopyToOtherDUC_inProgress = true;

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      #endregion RISK_CopyToSomeOtherTT - procisceni

      #region NewRecord_OnClick - procisceni

      NewRecord_OnClick(null, EventArgs.Empty);

      #endregion NewRecord_OnClick - procisceni

      if(vvSubModulEnum != ZXC.VvSubModulEnum.R_ANU_PTG)
      {
         ((FakturExtDUC)TheVvRecordUC).PutAllKupdobFields(TheVvUC.Get_Kupdob_FromVvUcSifrar(ZXC.FakturRec.KupdobCD));

         // 03.12.2021: byQ: ako odremarkiras ove SendKeys, dize se Exception da je Kupdob prazan "molimo zadaje partnera prije usnimavanja"
         // raylog tomu je sto su kupdob polja readonly, kakve veze ovo sa SendKeys ima veze ne znam ali pomaze!? ... ne dize se validacija 

         //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");
      }

      if(vvSubModulEnum == ZXC.VvSubModulEnum.R_ANU_PTG)
      {
         ((FakturExtDUC)TheVvRecordUC as ANU_PTG_DUC).Fld_PTG_DanFakturiranjaString = ZXC.FakturRec.OpciAvalue;
         ((FakturExtDUC)TheVvRecordUC as ANU_PTG_DUC).Fld_PTG_DanFakturiranjaOpis   = ZXC.luiListaPTG_DanZaFaktur.GetNameForThisCd(ZXC.FakturRec.OpciAvalue);
      }
      //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

      if(vvSubModulEnum == ZXC.VvSubModulEnum.R_ZIZ_PTG)
      {
         VvHamper.Open_Close_Fields_ForWriting((TheVvRecordUC as ZIZ_PTG_DUC).hamp_partner_PTG, ZXC.ZaUpis.Zatvoreno, ZXC.ParentControlKind.VvOtherUC);
      }

      // 25.03.2025: jer ko da je bussiness prazan pa ak se npr odlaskom na drugi UC 
      // digne validacija koja ima prazan bussiness                                  
      TheVvUC.GetFields(false);

      // 02.06.2025: 
      ZXC.FakturRec = null;
   }


   #endregion PTG_CreateNew_AUN_DOD

   #region PTG_CreateNew_OrEditExisting_KOP

   private void RISK_PTG_UGAN_to_KOP(object sender, EventArgs e)
   {
      ZXC.KOPfromUGAN_InProgress = true;
      ZXC.KOPfromUGAN_TabIndex   = TheTabControl.SelectedIndex;

      PTG_CreateNew_OrEditExisting_KOP(new PTG_Ugovor(((Faktur)TheVvDataRecord)));
   }
   
   private void RISK_PTG_KOPfromFUG(object sender, EventArgs e)
   {
      FUG_PTG_UC theUC = TheVvUC as FUG_PTG_UC;

      VvDataGridView theG = theUC.TheG_1;

      int rowIdx = theG.CurrentRow.Index;

      if(rowIdx.IsNegative()) return;

      string tipBr = theG.GetStringCell(theUC.DgvCI.iT_uganNum, rowIdx, false);

      // just open UGAN_DUC 
      ZXC.TheVvForm.ShowFakturDUC_For_TipBr(tipBr);

      //PTG_CreateNew_OrEditExisting_KOP(new PTG_Ugovor(((Faktur)TheVvDataRecord)));

      ZXC.KOPfromFUG_InProgress = true;

      RISK_PTG_UGAN_to_KOP(sender, e);
   }

   internal void PTG_CreateNew_OrEditExisting_KOP(PTG_Ugovor ugan_rec)
   {
      ZXC.VvSubModulEnum vvSubModulEnum = ZXC.VvSubModulEnum.X_KOP;

      uint KOP_TtNum = ugan_rec.TtNum;

      Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(ugan_rec.KupdobCD);

      ugan_rec.LoadOtplatniPlan(TheDbConnection);

      #region RISK_CopyToSomeOtherTT - procisceni

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(vvSubModulEnum);

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

    //ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete()); // trebamo ga za PutDefaultDUCfields 
    //ZXC.MixerRec  = (Mixer) (TheVvDataRecord.CreateNewRecordAndCloneItComplete()); // trebamo ga za PutDefaultDUCfields 

      //ZXC.RISK_CopyToOtherDUC_inProgress = true;

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      #endregion RISK_CopyToSomeOtherTT - procisceni

      #region NewRecord_OnClick - procisceni

      KOP_PTG_DUC theDUC = TheVvDocumentRecordUC as KOP_PTG_DUC;

      theDUC.ThePTG_Ugovor   = ugan_rec                ;
      theDUC.TheOtplatniPlan = ugan_rec.TheOtplatniPlan;

      Mixer KOPmixer_rec = Get_KOP_ForThisUGAN(KOP_TtNum);

      if(KOPmixer_rec == null || KOPmixer_rec.TtNum.IsZero()) // 'Novi' - ADDREC Mixer 
      {
         NewRecord_OnClick(null, EventArgs.Empty);

         theDUC.Fld_KupDobCd   = kupdob_rec.KupdobCD;
         theDUC.Fld_KupDobName = kupdob_rec.Naziv   ;
         theDUC.Fld_KupDobTk   = kupdob_rec.Ticker  ;

         theDUC.Fld_TT       = Mixer.TT_KOP;
         theDUC.Fld_TtNum    = KOP_TtNum;
         theDUC.Fld_DokDate  = VvSQL.GetServer_DateTime_Now(TheDbConnection);

         theDUC.Fld_V1_ttNum = ugan_rec.PTG_KUGnum ;
         theDUC.Fld_V2_ttNum = ugan_rec.PTG_UGANnum;

         Xtrans xtrans_rec;

         List<Xtrans> KOP_xtranses = new List<Xtrans>();

         foreach(PTG_Rata rata in ugan_rec.TheOtplatniPlan.UGAN_RateList)
         {
            xtrans_rec = ugan_rec.TheOtplatniPlan.Create_Xtrans_FromPTG_Rata(rata);

            KOP_xtranses.Add(xtrans_rec);
         }

         int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

         foreach(Xtrans xtrans in KOP_xtranses)
         {
            theDUC.TheG.Rows.Add();

            rowIdx = theDUC.TheG.RowCount - idxCorrector;

            theDUC.PutDgvLineFields1(xtrans, rowIdx, false);
         }

         theDUC.PutDgvTransSumFields();

         // Rucno disable-iranje dirty flaga
         TheVvTabPage.PaliGasiDirtyFlag              (false);
         SetVvMenuEnabledOrDisabled_Explicitly("SAV", false);
         SetVvMenuEnabledOrDisabled_Explicitly("SAS", false);

      }
      else // 'Ispravi' - RWTREC Faktur 
      {
         TheVvDataRecord = KOPmixer_rec;

         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);

         EditRecord_OnClick(null, EventArgs.Empty);
      }

      #endregion NewRecord_OnClick - procisceni

      SetColumnIsXxx_ReadOnly(theDUC);

      SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");
   }

   private void SetDatumOdDo_sazeteRate(object sender, EventArgs e)
   {
      KOP_PTG_DUC theDUC = TheVvUC as KOP_PTG_DUC;

      theDUC.GetFields(false);

      Mixer KOPmixer_rec = TheVvDataRecord as Mixer;

      // treba ti tu novi nezavisni 
      PTG_OtplatniPlan local_PTG_OtplatniPlan = new PTG_OtplatniPlan(TheDbConnection, theDUC.ThePTG_Ugovor);

      local_PTG_OtplatniPlan/*theDUC.TheOtplatniPlan*/.Merge_KOP(KOPmixer_rec, true); 

      for(int rowIdx = 0; rowIdx < local_PTG_OtplatniPlan.UGAN_RateList.Count; ++rowIdx)
      {
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_dateOd, rowIdx, local_PTG_OtplatniPlan.UGAN_RateList[rowIdx].DatumOd);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_dateDo, rowIdx, local_PTG_OtplatniPlan.UGAN_RateList[rowIdx].DatumDo);
      }

      TheVvTabPage.PaliGasiDirtyFlag(true);
      SetVvMenuEnabledOrDisabled_Explicitly("SAV", true);

   }

   #endregion PTG_CreateNew_OrEditExisting_KOP

   private void RISK_PTG_AddRate(object sender, EventArgs e)
   {
      //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      UGNorAUN_PTG_DUC theDUC = TheVvDocumentRecordUC as UGNorAUN_PTG_DUC;

      VvBrojRataPlusMinus_PTG_Dlg dlg = new VvBrojRataPlusMinus_PTG_Dlg();

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         theDUC.Fld_PTG_KorekcijaRata +=  dlg.Fld_PTG_brRataPlusMinus;
         SetDirtyFlag("PTG_AddRate");
      }

      dlg.Dispose();
   }

   private void RISK_PTG_DelRate(object sender, EventArgs e) {}

   private Mixer Get_KOP_ForThisUGAN(uint KOP_TtNum)
   {
      Mixer KOPmixer_rec = new Mixer();

      MixerDao.SetMeMixer(TheDbConnection, KOPmixer_rec, Mixer.TT_KOP, KOP_TtNum, true);

      return KOPmixer_rec;
   }

   private void SetColumnIsXxx_ReadOnly(KOP_PTG_DUC theDUC)
   {
      VvDataGridView dgv = theDUC.TheG;
      int rIdx;
      for(int i = 0; i < dgv.Rows.Count; i++)
      {
         rIdx = i;
         dgv.Rows[rIdx].Cells["T_isXxx"].ReadOnly = true;

         if(dgv.Rows[rIdx].Cells["T_isXxx"].Value == null || dgv.Rows[rIdx].Cells["T_isXxx"].Value.ToString() == "")
            dgv.Rows[rIdx].Cells["T_isXxx"].Style.BackColor = ZXC.vvColors.vvTBoxReadOnly_True_BackColor;
      }
   }

   internal void RISK_PTG_Izlistaj_TheG1_RateZaFak(object sender, EventArgs e)
   {
      TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.None);

      FUG_PTG_UC theUC = TheVvUC as FUG_PTG_UC;

      if(ZXC.IsRipleyOrKristal)
      {
         if(theUC.Fld_DatumDo    .IsEmpty()) theUC.Fld_DatumDo     = DateTime.Today;
         if(theUC.Fld_DatumRacuna.IsEmpty()) theUC.Fld_DatumRacuna = DateTime.Today;
      }

      PTG_Ugovor.PTG_FakturiranjeKind ptgFakKind = theUC.Fld_PTG_FakturiranjeKind;
      DateTime                        dateDo     = theUC.Fld_DatumDo;

      switch(ptgFakKind)
      {
         case PTG_Ugovor.PTG_FakturiranjeKind.Grupno     :
         case PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno:

            theUC.The_Rata_List_ZaFakturiranje = PTG_Ugovor.Get_PTG_Rata_List_ZaFakturiranje(TheDbConnection, dateDo, ptgFakKind);

            theUC.PutDgvFields1(/*The_Rata_List_ZaFakturiranje*/);
            break;

         case PTG_Ugovor.PTG_FakturiranjeKind.Otkup:

            theUC.The_Rata_List_ZaFakturiranje = PTG_Ugovor.Get_PTG_Rata_List_ZaFakturiranje(TheDbConnection, dateDo, ptgFakKind);

            theUC.PutDgvFields1(/*The_Rata_List_ZaFakturiranje*/);
            break;

         case PTG_Ugovor.PTG_FakturiranjeKind.UslugeA1grupno:

          //theUC.The_Rata_List_ZaFakturiranje = PTG_Ugovor.Get_PTG_Rata_List_ZaFakturiranje(TheDbConnection, DateTime.Now, ptgFakKind);
            break;

      } // switch(ptgFakKind) 

      //string PTG_FUG_ID = GetPTG_FUG_ID(ptgFakKind);
      theUC.Fld_PTG_FUG_ID = GetPTG_FUG_ID(ptgFakKind);
   }

   private string GetPTG_FUG_ID(PTG_Ugovor.PTG_FakturiranjeKind ptgFakKind)
   {
      string PTG_FUG_ID = "";

      switch(ptgFakKind)
      {
         case PTG_Ugovor.PTG_FakturiranjeKind.Grupno        : PTG_FUG_ID = "G-" + ZXC.ThisYearSeconds; break; 
         case PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno   : PTG_FUG_ID = "P-" + ZXC.ThisYearSeconds; break;
         case PTG_Ugovor.PTG_FakturiranjeKind.Otkup         : PTG_FUG_ID = "O-" + ZXC.ThisYearSeconds; break;
         case PTG_Ugovor.PTG_FakturiranjeKind.UslugeA1grupno: PTG_FUG_ID = "A-" + ZXC.ThisYearSeconds; break;
      } // switch(ptgFakKind) 

      return PTG_FUG_ID;
   }

   private void RISK_PTG_AutoAddFaktursFromRate(object sender, EventArgs e)
   {
      // REFRESH LIST first! 

      RISK_PTG_Izlistaj_TheG1_RateZaFak(sender, e);

      #region Init

      ushort line = 0;
      int debugCount = 0;

      uint ttNumOD = 0;
      uint ttNumDO = 0;

      Faktur faktur_rec;
      Rtrans rtrans_rec;

      FUG_PTG_UC theUC = TheVvUC as FUG_PTG_UC;

      PTG_Ugovor.PTG_FakturiranjeKind ptgFakKind = theUC.Fld_PTG_FakturiranjeKind;

    //List<PTG_Rata> theRataList = theUC.The_Rata_List_ZaFakturiranje;
      List<PTG_Rata> theRataList = theUC.The_Rata_List_ZaFakturiranje.Where(rata => !rata.KOPfakStop).ToList();

      List<Rtrans> thisFakturaRtransList;

      IEnumerable<IGrouping</*uint*/string, PTG_Rata>> fakturaRateGroups = null;

      theUC.ThePolyGridTabControl.SelectedIndex = 1;

      if(theRataList.IsEmpty()) return;

      #endregion Init

      #region Grupiraj rate po fakturi

      switch(ptgFakKind)
      {
         case PTG_Ugovor.PTG_FakturiranjeKind.Grupno        : fakturaRateGroups = theRataList.GroupBy(rata => rata.GrupFakGrID ); break; 
         case PTG_Ugovor.PTG_FakturiranjeKind.Pojedinacno   : fakturaRateGroups = theRataList.GroupBy(rata => rata.PojedFakGrID); break;
         case PTG_Ugovor.PTG_FakturiranjeKind.Otkup         : fakturaRateGroups = theRataList.GroupBy(rata => rata.PojedFakGrID); break;
         case PTG_Ugovor.PTG_FakturiranjeKind.UslugeA1grupno: break;

      } // switch(ptgFakKind) 

      #endregion Grupiraj rate po fakturi

      #region Prvo ih samo pokazi na grid-u

      theUC.PutDefaultFilterFields(0, 0, "");
      theUC.TheG_2.Rows.Clear();

      //foreach(var fakturaGR in fakturaRateGroups)
      for(int rIdx=0; rIdx < fakturaRateGroups.Count(); ++rIdx)
      {
         faktur_rec = CreateFakturFromRateGroup(fakturaRateGroups.ElementAt(rIdx), theUC.Fld_PTG_FUG_ID, theUC.Fld_DatumRacuna, ptgFakKind);

         theUC.TheG_2.Rows.Add();

         theUC.PutDgvFields_FaktureZaSlanje(faktur_rec, fakturaRateGroups.ElementAt(rIdx).First(), rIdx);

         theUC.TheG_2.Rows[rIdx].HeaderCell.Value = (rIdx + 1).ToString();
      }

      theUC.PutDefaultFilterFields(0, 0, theUC.Fld_PTG_FUG_ID);

      #endregion Prvo ih samo pokazi na grid-u

      #region Vidi malo idemo li dalje

      string message = "Da li zaista zelite kreirati " + fakturaRateGroups.Count() + " faktura?";

      DialogResult result = MessageBox.Show(message, "POTVRDITE KREIRANJE FAKTURA", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes)
      {
         theUC.TheG_2.Rows.Clear();
         theUC.PutDefaultFilterFields(0, 0, "");
         return;
      }

      #endregion Vidi malo idemo li dalje

      // Here we go: ... 

      #region Execute AutoSetFaktur 

      Cursor.Current = Cursors.WaitCursor;

      int rowIdx = 0;
      theUC.TheG_2.Rows.Clear();
      theUC.PutDefaultFilterFields(0, 0, "");

      decimal sumRata = 0M;

      foreach(var fakturaGR in fakturaRateGroups)
      {
         line = 0;

         faktur_rec = CreateFakturFromRateGroup(fakturaGR, theUC.Fld_PTG_FUG_ID, theUC.Fld_DatumRacuna, ptgFakKind);

         thisFakturaRtransList = new List<Rtrans>();

         PTG_Ugovor   UGANfaktur_rec      = null;
         List<Rtrans> UNA_SIN_rtrans_list = null;

         foreach(PTG_Rata rata in fakturaGR)
         {
            if(rata.IsOtkup)
            {
               Faktur tmpFaktur_rec = new Faktur();
               bool ok = FakturDao.SetMeFaktur(TheDbConnection, tmpFaktur_rec, rata.UGANtt, rata.UGANttNum, false);
               if(!ok) continue;

               tmpFaktur_rec.VvDao.LoadTranses(TheDbConnection, tmpFaktur_rec, false);

                            UGANfaktur_rec      = new PTG_Ugovor(tmpFaktur_rec);
               List<Rtrans> UGANrtrans_list     = UGANfaktur_rec.Transes;
               List<Rtrans> DODrtrans_list      = RtransDao.Get_DOD_RtransList(TheDbConnection, UGANfaktur_rec);
                            UNA_SIN_rtrans_list = FakturDUC.Get_UNA_SIN_rtrans_list(UGANrtrans_list, DODrtrans_list);

               foreach(Rtrans UNA_SIN_rtrans in UNA_SIN_rtrans_list)
               {
                  rtrans_rec = Create_RtransFromPTG_UNA_SIN_rtrans(UNA_SIN_rtrans, rata, UGANfaktur_rec, theUC.Fld_DatumRacuna, 0);
                  faktur_rec.Transes.Add(rtrans_rec);
               }
            }
            else // classic, NOT otkup rata 
            {
               rtrans_rec = CreateRtransFromOnePTG_Rata(rata, theUC.Fld_DatumRacuna);
               faktur_rec.Transes.Add(rtrans_rec);
            }
         }

         #region TakeTransesSumToDokumentSum

         faktur_rec.TakeTransesSumToDokumentSum(true);
         faktur_rec.Transes = null;

         #endregion TakeTransesSumToDokumentSum

         foreach(PTG_Rata rata in fakturaGR)
         {
            if(rata.IsOtkup)
            {
               foreach(Rtrans UNA_SIN_rtrans in UNA_SIN_rtrans_list)
               {
                  if(line.IsZero()) debugCount++;

                  rtrans_rec = Create_RtransFromPTG_UNA_SIN_rtrans(UNA_SIN_rtrans, rata, UGANfaktur_rec, theUC.Fld_DatumRacuna, (ushort)(line + 1));

                  FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans_rec);

                  if(debugCount == 1) ttNumOD = faktur_rec.TtNum;
               }
            }
            else // classic, NOT otkup rata 
            { 
               if(line.IsZero()) debugCount++;

               rtrans_rec = CreateRtransFromOnePTG_Rata(rata, theUC.Fld_DatumRacuna);

               FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans_rec);

               if(debugCount == 1) ttNumOD = faktur_rec.TtNum;
            }
         } // foreach(PTG_Rata rata in fakturaGR) (create new Rtrans)  

         ttNumDO = faktur_rec.TtNum;

         #region Show News

         theUC.TheG_2.Rows.Add();

         theUC.PutDgvFields_FaktureZaSlanje(faktur_rec, fakturaGR.First(), rowIdx);

         sumRata += faktur_rec.S_ukKCRP;

         theUC.TheG_2.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

         rowIdx++;

         #endregion Show News

      } // foreach(var fakturaGR in fakturaRateGroups) 

      theUC.TheG2_SumGrid.PutCell(theUC.DgvCI2.iT_iznosRn, 0, sumRata.ToStringVv());

      theUC.PutDefaultFilterFields(ttNumOD, ttNumDO, theUC.Fld_PTG_FUG_ID);

      #endregion Execute AutoSetFaktur 

      #region Finish

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg("Gotovo. Kreirao {0} Fakturs.", debugCount);

      #endregion Finish

   }

   private Faktur CreateFakturFromRateGroup(IGrouping</*uint*/string, PTG_Rata> fakturaGR, string fug_id, DateTime fugDokDate, PTG_Ugovor.PTG_FakturiranjeKind ptgFakKind)
   {
      PTG_Rata firstRata = fakturaGR.First();

      Faktur faktur_rec = new Faktur();

    //faktur_rec.TT          = Faktur.TT_IFA;
      faktur_rec.TT          = Faktur.TT_IRA;
      faktur_rec.DokDate     = 
      faktur_rec.PdvDate     = fugDokDate;

      faktur_rec.Napomena    = "";
      if(ptgFakKind == PTG_Ugovor.PTG_FakturiranjeKind.Otkup)
      {
         faktur_rec.Napomena += "Otkup po ugovoru: " + firstRata.UGANttNum + " ";
      }
      faktur_rec.Napomena += firstRata.UGANrnNapomena;

      #region Kupdob

      Kupdob kupdob_rec = firstRata.TheKupdob;

      faktur_rec.KupdobCD    = 
      faktur_rec.PosJedCD    = kupdob_rec.KupdobCD;
      faktur_rec.KupdobName  =
      faktur_rec.PosJedName  = kupdob_rec.Naziv   ;
      faktur_rec.KupdobTK    =
      faktur_rec.PosJedTK    = kupdob_rec.Ticker  ;
      faktur_rec.KdUlica     =
      faktur_rec.PosJedUlica = kupdob_rec.Ulica2  ;
      faktur_rec.KdZip       =
      faktur_rec.PosJedZip   = kupdob_rec.PostaBr ;
      faktur_rec.KdMjesto    =
      faktur_rec.PosJedMjesto= kupdob_rec.Grad    ;
      faktur_rec.KdOib       = kupdob_rec.Oib     ;
      faktur_rec.Konto       = kupdob_rec.KontoPot;

      #endregion Kupdob

      faktur_rec.RokPlac     = firstRata.UGANrokPlac; 
      faktur_rec.DospDate    = faktur_rec.DokDate + new TimeSpan(faktur_rec.RokPlac, 0, 0, 0);
      faktur_rec.PdvKnjiga   = ZXC.PdvKnjigaEnum.REDOVNA;
      faktur_rec.PdvR12      = ZXC.PdvR12Enum.R1;

      faktur_rec.VezniDok    = fug_id;

      faktur_rec.OpciAvalue  = firstRata.UGANnarudzba;

      faktur_rec.SkladCD     = ZXC.PTG_ZNJ;

      return faktur_rec;
   }

   private Rtrans CreateRtransFromOnePTG_Rata(PTG_Rata rata, DateTime fugDokDate)
   {
      Rtrans rtrans_rec = new Rtrans();

      string uganString = rata.TheKUGkupdobTK.IsEmpty() ? " ugovor: " : " aneks: ";
      string artiklName;

      //if(rata.IsOtkup) artiklName = "Ovdje trebaju doći pojedinačni artikli " + "Otkup po ugovoru: " + rata.UGANttNum;

      artiklName = "Iznos najamnine za razdoblje " + rata.DatumOd.ToString(ZXC.VvDateFormat) + " do " + rata.DatumDo.ToString(ZXC.VvDateFormat) + uganString + rata.UGANttNum /*+ ", " + rata.RataRbr + ". rata"*/;

      // kaze da je predugacko!!!
      //if(rata.RataRbr.IsNegative())
      //   artiklName = "Iznos najamnine za dodatno izdavanje po dodatku broj: " + rata.RataRbr*-1 + " ugovora o najmu br: " + rata.UGANttNum +  " za period najma od" + rata.DatumOd.ToString(ZXC.VvDateFormat) + " do " + rata.DatumDo.ToString(ZXC.VvDateFormat);

      rtrans_rec.T_artiklName = artiklName    ;
      rtrans_rec.T_cij        = rata.RataMoney;
      rtrans_rec.T_kol        = 1.00M;
      rtrans_rec.T_rbt1St     = 0.00M; // todo 
      rtrans_rec.T_pdvSt      = Faktur.CommonPdvStForThisDate(/*rata.UGANdate*/fugDokDate);

      rtrans_rec.CalcTransResults(null);

      rtrans_rec.T_serlot = rata.Serlot4Rtrans;

      return rtrans_rec;
   }

   private Rtrans Create_RtransFromPTG_UNA_SIN_rtrans(Rtrans OTKUP_SIN_rtrans, PTG_Rata rata, PTG_Ugovor UGANfaktur_rec, DateTime fugDokDate, ushort artiklRbr)
   {
      Rtrans rtrans_rec = OTKUP_SIN_rtrans.MakeDeepCopy();
    //Rtrans rtrans_rec = OTKUP_SIN_rtrans.CreateNewRecordAndCloneItComplete();

      rtrans_rec.T_cij   = ZXC.VvGet_25_of_100(rtrans_rec.T_cij, UGANfaktur_rec.PTG_OtkupPosto).Ron2();
      rtrans_rec.T_pdvSt = Faktur.CommonPdvStForThisDate(fugDokDate);

      rtrans_rec.CalcTransResults(null);

      rtrans_rec.T_serlot = rata.Serlot4Rtrans + "/" + artiklRbr;

      return rtrans_rec;
   }

   private void RISK_PTG_Izlistaj_TheG2_FaktureZaSlanje(object sender, EventArgs e)
   {
      FUG_PTG_UC theUC = TheVvUC as FUG_PTG_UC;

      List<Faktur>            fakturList = new List<Faktur>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(4);

         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt      ], "theTT"  , Faktur.TT_IRA               , " = " ));

      if(theUC.Fld_PTG_RacunOd_Filter.NotZero())
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.ttNum   ], "ttNumOd", theUC.Fld_PTG_RacunOd_Filter, " >= "));
      if(theUC.Fld_PTG_RacunDo_Filter.NotZero())
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.ttNum   ], "ttNumDo", theUC.Fld_PTG_RacunDo_Filter, " <= "));
      if(theUC.Fld_PTG_FUG_ID_Filter.NotEmpty())
         filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.vezniDok], "fugID"  , theUC.Fld_PTG_FUG_ID_Filter , " = " ));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(TheDbConnection, fakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      Kupdob kupdob_rec;

      int rowIdx = 0;
      decimal sumRata = 0M;

      theUC.TheG_2.Rows.Clear();

      foreach(Faktur faktur_rec in fakturList)
      {
         theUC.TheG_2.Rows.Add();

         kupdob_rec = theUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD);

         theUC.PutDgvFields_FaktureZaSlanje(faktur_rec, kupdob_rec, rowIdx);

         sumRata += faktur_rec.S_ukKCRP;

         theUC.TheG_2.Rows[rowIdx].HeaderCell.Value = (rowIdx + 1).ToString();

         rowIdx++;

      } // foreach(var fakturaGR in fakturaRateGroups) 
      theUC.TheG2_SumGrid.PutCell(theUC.DgvCI2.iT_iznosRn, 0, sumRata.ToStringVv());

   }

   private void RISK_PTG_Dispatch_FakturList(object sender, EventArgs e)
   {
      #region Get fakturList from FUG_DUC

      #region Init

      FUG_PTG_UC theUC = TheVvUC as FUG_PTG_UC;

      int rIdx;
      string tipBr;

      string tt;
      uint   ttNum;
      bool   parseOK, fakOK;

      Faktur faktur_rec;

      List<Faktur> fakturList = new List<Faktur>(theUC.TheG_2.RowCount);

      bool currRowIs_eMail, currRowIs_eRacun;

      #endregion Init

      for(rIdx = 0; rIdx < theUC.TheG_2.RowCount /*- 1*/; ++rIdx)
      {
         currRowIs_eMail  = theUC.TheG_2.GetStringCell(theUC.DgvCI2.iT_isNaEmail, rIdx, false) == "X";
         currRowIs_eRacun = theUC.TheG_2.GetStringCell(theUC.DgvCI2.iT_iseRacun , rIdx, false) == "X";

         switch(theUC.Fld_PTG_SlanjeFakKind)
         {
            case PTG_Ugovor.PTG_SlanjeFakKind.Print : if(currRowIs_eMail || currRowIs_eRacun) continue; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.eMail : if(currRowIs_eMail  == false          ) continue; break;
            case PTG_Ugovor.PTG_SlanjeFakKind.eRacun: if(currRowIs_eRacun == false          ) continue; break;
         }

         tipBr = theUC.TheG_2.GetStringCell(theUC.DgvCI2.iT_brRacuna, rIdx, false);

         parseOK = Ftrans.ParseTipBr(tipBr, out tt, out ttNum);

         if(parseOK == false) continue;

         faktur_rec = new Faktur();

         fakOK = FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);

         if(fakOK == false) continue;

         faktur_rec.VvDao.LoadTranses(TheDbConnection, faktur_rec, false);

         fakturList.Add(faktur_rec);

      } // for(rIdx = 0; rIdx < theUC.TheG_2.RowCount /*- 1*/; ++rIdx) 

      #endregion Get fakturList from FUG_DUC

      DialogResult result = MessageBox.Show("Da li zaista želite pokušati isporučiti ovih " + fakturList.Count + " računa?!",
         "Potvrdite isporuku", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      switch(theUC.Fld_PTG_SlanjeFakKind)
      {
         case PTG_Ugovor.PTG_SlanjeFakKind.Print : VvRiskReport.FakturListTo_QuickPrint(fakturList); break;
         case PTG_Ugovor.PTG_SlanjeFakKind.eMail : VvRiskReport.FakturListTo_eMail     (fakturList); break;
         case PTG_Ugovor.PTG_SlanjeFakKind.eRacun: VvRiskReport.FakturListTo_eRacun    (fakturList); break;
      }

      // show news: 
      RISK_PTG_Izlistaj_TheG2_FaktureZaSlanje(null, EventArgs.Empty);
   }

   private void primjer_zaSlanje_eMaila(object sender, EventArgs e)
   {
      VvMailClient mailClient = new VvMailClient();

      mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword; // !!! ovo moze biti i prazno! Tada ida anonymously?! 

      mailClient.MailHost             = ZXC.ViperMailHost             ;
      mailClient.EmailFromAddress     = ZXC.SkyLabEmailAddress        ;
      mailClient.EmailFromDisplayName = ZXC.SkyLabEmailFromDisplayName;
    //mailClient.EmailTo              = ZXC.VektorEmailAddress        ;
      mailClient.MessageSubject       = "PC_TO_GO Testis SUBJECT";
      mailClient.MessageBody          = "TrlaBabaLanDaJojProđeDan\n\nQ'";

      string attachmentFullFileNamePath = /*ZXC.Vv_GZip_ThisFile(*/@"E:\DLtest.txt"/*)*/;

    //mailClient.MailAttachmentFileNameList = new System.Net.Mail.Attachment(attachmentFullFileNamePath);
      mailClient.MailAttachmentFileNameList = new string[] { attachmentFullFileNamePath };

      mailClient.SendMail_Normal(true, ZXC.VektorEmailAddress); // ili / ili 
    //mailClient.SendMail_Async (true, ZXC.VektorEmailAddress); // ili / ili 

   }


   private void RISK_PTG_Add_UgAnDo_Serno_Rtrano(object sender, EventArgs e) // <F3> 
   {
      if(!ZXC.IsPCTOGO) return;
      if(ZXC.TheVvForm.TheVvUC is MOD_PTG_DUC || ZXC.TheVvForm.TheVvUC is PVR_PTG_DUC || ZXC.TheVvForm.TheVvUC is ZIZ_PTG_DUC) return;
      if(TheVvTabPage.WriteMode != ZXC.WriteMode.None) return;

      #region init

      FakturPDUC theDUC     = TheVvUC as FakturPDUC;

      if(theDUC == null) return;

      Faktur     faktur_rec = theDUC.faktur_rec    ;

      if(faktur_rec.Transes.IsEmpty()) return;

      ZXC.RISK_Edit_RtranoOnly_InProgress = true;

      EditRecord_OnClick("RISK_PTG_AddRtrano", EventArgs.Empty);

      if(TheVvTabPage.WriteMode == ZXC.WriteMode.None) return;

      theDUC.ThePolyGridTabControl.TabPages[theDUC.TabPageTitle2].Selected = true;
      theDUC.TheG2.Select();

      Rtrano newRtrano_rec;

      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG2);

      #endregion init

      #region We came from MOD_DUC?

      bool isFromMOD = sender is List<Rtrano>;

      List<Rtrano> MOCrtranos;
      
      if(isFromMOD) MOCrtranos = sender as List<Rtrano>;
      else          MOCrtranos = null                  ;

      Rtrano MOCrtrano;

      #endregion We came from MOD_DUC?

      #region Set Rtrano Grid

      // prvi ulazak na serno grid: 
      bool transes2_does_NOT_exists = (faktur_rec.Transes2.IsEmpty());
      // Transes2 nije prazan, ali su svi serno-i prazni ... beskorisni: 
      bool transes2_exists_but_is_useless = !(faktur_rec.Transes2.Any(rto => rto.T_serno.NotEmpty()));

      if(transes2_does_NOT_exists || transes2_exists_but_is_useless) 
      {
         if(transes2_exists_but_is_useless)
         {
            foreach(VvTransRecord ugandoRtrano_rec in faktur_rec.Transes2)
            {
               ugandoRtrano_rec.VvDao.DELREC(TheDbConnection, ugandoRtrano_rec, false);
            }

            faktur_rec.InvokeTransClear2();

            theDUC.TheG2.Rows.Clear();
         }

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            if(Artikl.DoesThisArtikl_Needs_RtranoRow_ForSerno(rtrans_rec.T_artiklCD, rtrans_rec.T_TT) == false) continue;

            for(int i = 1; i <= (int)rtrans_rec.T_kol; ++i)
            {
               newRtrano_rec = new Rtrano(rtrans_rec);

               if(rtrans_rec.TtInfo.TwinTT.NotEmpty())
               {
                  newRtrano_rec.T_TT      = rtrans_rec.TtInfo.TwinTT;
                  newRtrano_rec.T_skladCD = faktur_rec.SkladCD2     ;
               }

               // isFromMOD?: start _________________________________________________

               if(isFromMOD)
               {
                  MOCrtrano = MOCrtranos.FirstOrDefault(rto => rto.T_artiklCD == rtrans_rec.T_artiklCD);

                  if(MOCrtrano != null)
                  {
                     newRtrano_rec.T_serno = MOCrtrano.T_serno;
                     newRtrano_rec.T_grCD  = MOCrtrano.T_grCD ;

                     MOCrtranos.RemoveAll(rto => rto.T_serno == MOCrtrano.T_serno);
                  }
               }

               // isFromMOD?:  end  _________________________________________________

               theDUC.TheG2.Rows.Add();

               rowIdx = theDUC.TheG2.RowCount - idxCorrector;
                              
               theDUC.PutDgvLineFields2(newRtrano_rec, rowIdx, false);
            }
         }
      }
      else // NIJE NI prvi ulazak na serno grid NI da su svi beskorisni 
      {
         // Rtrans pass START ___________________________________________________________________________________________ 

         List<Rtrano> thisRtransRtranoList;

         foreach(Rtrans rtrans_rec in faktur_rec.Transes)
         {
            if(Artikl.DoesThisArtikl_Needs_RtranoRow_ForSerno(rtrans_rec.T_artiklCD, rtrans_rec.T_TT) == false) continue;

            thisRtransRtranoList = faktur_rec.Transes2.Where(rto => rto.T_artiklCD == rtrans_rec.T_artiklCD && rto.T_paletaNo == rtrans_rec.T_serial).ToList();

            // smanjena je rtrans kolicina 
            if(rtrans_rec.T_kol < thisRtransRtranoList.Count)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Uslijed SMANJENJA količine stavke ugovora artikla [{0}]\n\r\n\rnastao je višak od {1} red. na listi serijskih brojeva!", rtrans_rec.T_artiklCD, (thisRtransRtranoList.Count - rtrans_rec.T_kol).ToString0Vv());
            }

            // povecana je rtrans kolicina ______________________________________________ 
            if(rtrans_rec.T_kol > thisRtransRtranoList.Count) 
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Uslijed POVEĆANJA količine stavke ugovora artikla [{0}]\n\r\n\rdodati ću {1} red. na listu serijskih brojeva.", rtrans_rec.T_artiklCD, (rtrans_rec.T_kol - thisRtransRtranoList.Count).ToString0Vv());

               for(int i = 1; i <= (rtrans_rec.T_kol - thisRtransRtranoList.Count); ++i)
               {
                  newRtrano_rec = new Rtrano(rtrans_rec);

                  if(rtrans_rec.TtInfo.TwinTT.NotEmpty())
                  {
                     newRtrano_rec.T_TT      = rtrans_rec.TtInfo.TwinTT;
                     newRtrano_rec.T_skladCD = faktur_rec.SkladCD2     ;
                  }

                  theDUC.TheG2.Rows.Add();

                  rowIdx = theDUC.TheG2.RowCount - idxCorrector;

                  theDUC.PutDgvLineFields2(newRtrano_rec, rowIdx, false);
               }
            } // povecana je rtrans kolicina ____________________________________________ 
         }

         // Rtrans pass END _____________________________________________________________________________________________ 

         // Rtrano pass START ___________________________________________________________________________________________ 

         bool rtransFound;

         theDUC.GetDgvFields2(false); // da napuni bussiness 

         foreach(Rtrano rtrano_rec in faktur_rec.Transes2)
         {
            rtransFound = faktur_rec.Transes.Any(rtr => rtr.T_artiklCD == rtrano_rec.T_artiklCD && rtr.T_serial == rtrano_rec.T_paletaNo);

            if(!rtransFound)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Na listi serijskih brojeva postoji artikl [{0}]\n\r\n\rkojega nema na stavkama ugovora!", rtrano_rec.T_artiklCD);
            }
         }

         // Rtrano pass END   ___________________________________________________________________________________________ 

         if(isFromMOD) // vec prethodno djelomicno popunjen rtrano grid 
         {
            Rtrano rtrano_rec;

            bool touched = false;
            bool sernoIsAlreadyPaired;

            for(rowIdx = 0; rowIdx < faktur_rec.Transes2.Count; ++rowIdx)
            {
               rtrano_rec = (Rtrano)theDUC.GetDgvLineFields2(rowIdx, false, null);

               if(rtrano_rec.T_serno.IsEmpty())
               {
                  MOCrtrano = MOCrtranos.FirstOrDefault(rto => rto.T_artiklCD == rtrano_rec.T_artiklCD);

                  if(MOCrtrano != null) sernoIsAlreadyPaired = faktur_rec.Transes2.Any(rto => rto.T_serno == MOCrtrano.T_serno);
                  else                  sernoIsAlreadyPaired = false;

                  if(sernoIsAlreadyPaired)
                  {
                     ZXC.aim_emsg(MessageBoxIcon.Stop, "Serno {0} je već uparen?!", MOCrtrano.T_serno);

                     MOCrtranos.RemoveAll(rto => rto.T_serno == MOCrtrano.T_serno);
                  }

                  if(MOCrtrano != null && sernoIsAlreadyPaired == false)
                  {
                     theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_serno, rowIdx, MOCrtrano.T_serno);
                     theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_grCD , rowIdx, MOCrtrano.T_grCD );

                     theDUC.GetDgvLineFields2(rowIdx, false, null);

                     MOCrtranos.RemoveAll(rto => rto.T_serno == MOCrtrano.T_serno);

                     touched = true;
                  }
               }
            }

            if(touched) { ZXC.TheVvForm.SetDirtyFlag("RISK_PTG_Add_UgAnDo_Serno_Rtrano"); }
            else        { ZXC.aim_emsg(MessageBoxIcon.Warning, "Nema se što s čim upariti?!"); }

         } // if(isFromMOD) // vec prethodno djelomicno popunjen rtrano grid 

      } // else // NIJE NI prvi ulazak na serno grid NI da su svi beskorisni 

      #endregion Set Rtrano Grid

      #region Restrikcije rada na gridu

      //theDUC.TheG2.AllowUserToAddRows = false;
      theDUC.TheG2.AllowDrop = false; // za DragAndDrop row reoredering 
      
      theDUC.TheG2.CellClick -= new DataGridViewCellEventHandler(theDUC.TheG2.UpdateVvDataRecord_OnCellClick);
      theDUC.TheG2.CellClick -= new DataGridViewCellEventHandler(theDUC.TheG2.UpdateVvLookUpItem_OnCellClick);
      //theGrid1.CellMouseDoubleClick -= new DataGridViewCellMouseEventHandler(theGrid1.UpdateVvDataRecord_OnCellMouseDoubleClick);
      theDUC.TheG2.KeyDown -= new KeyEventHandler(theDUC.TheG2.UpdateVvDataRecord_OnKeyDown);
      theDUC.TheG2.KeyDown -= new KeyEventHandler(theDUC.TheG2.UpdateVvLookUpItem_OnKeyDown);
      
      ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = true;
      theDUC.TheG2.AllowUserToDeleteRows = false;
      theDUC.TheG2.AllowUserToAddRows    = false;
      ZXC.TheVvForm.VvFlag_AllowGridAddOrDeleteRowsIsChanging = false;

      #endregion Restrikcije rada na gridu

      #region finish

      ZXC.RISK_Edit_RtranoOnly_InProgress = false;

      #endregion finish

   }

   public void RISK_Show_PTG_PCKinfo(object sender, EventArgs e)
   {
      int    currRowIdx;
      string currArtiklCD, currSkladCD, artiklTS;

    //if(TheVvUC is FakturDUC == false)
      if(TheVvUC is ArtiklUC          )
      {
         Artikl artikl_rec_fromUC = (TheVvUC as ArtiklUC).artikl_rec;
         currSkladCD  = ZXC.PTG_ZNJ/*"ZNJ"*/;
         artiklTS     = ZXC.PCK_TS;

         //currArtiklCD = VvUserControl.ArtiklSifrar.OrderBy(art => art.ArtiklCD).FirstOrDefault(art => art.TS == ZXC.PCK_TS).ArtiklCD;

         if(artikl_rec_fromUC.AS_DateZadPromj.NotEmpty() && artikl_rec_fromUC.AS_ArtiklTS == ZXC.PCK_TS)
         {
            currArtiklCD = artikl_rec_fromUC.ArtiklCD;
         }
         else
         {
            currArtiklCD = PCK_ArtiklList_UC.GetFirstActivePCKartiklCD(TheDbConnection, currSkladCD, artikl_rec_fromUC.PCK_BazaCD);

            if(currArtiklCD.IsEmpty()) // bio je na 
            {
               currArtiklCD = PCK_ArtiklList_UC.GetFirstActivePCKartiklCD(TheDbConnection, currSkladCD, "");
            }
         }

         goto label_execute;
      }

    //FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
    //int currRowIdx = theDUC.TheG.CurrentRow.Index;

    //string currArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, currRowIdx, false);
    //string artiklTS     = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklTS, currRowIdx, false);

      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;
      
      if(theDUC.ThePolyGridTabControl.SelectedTab == theDUC.ThePolyGridTabControl.TabPages[theDUC.TabPageTitle1] && theDUC.TheG.CurrentRow != null)// na rtrans gridu
      { 
         currRowIdx   = theDUC.TheG.CurrentRow.Index;
         currArtiklCD = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklCD, currRowIdx, false);
         currSkladCD  = theDUC.DgvCI.iT_skladCD.IsPositive() ? theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_skladCD , currRowIdx, false) : theDUC.Fld_SkladCD;
         artiklTS     = theDUC.TheG.GetStringCell(theDUC.DgvCI.iT_artiklTS, currRowIdx, false);
      }
      else if(theDUC.ThePolyGridTabControl.SelectedTab == theDUC.ThePolyGridTabControl.TabPages[theDUC.TabPageTitle2] && theDUC.TheG2.CurrentRow != null)//na rtrano gridu
      { 
         currRowIdx   = theDUC.TheG2.CurrentRow.Index;
       //currArtiklCD = theDUC.TheG2.GetStringCell(theDUC.DgvCI2.iT_artiklCD, currRowIdx, false);
         currSkladCD  = theDUC.TheG2.GetStringCell(theDUC.DgvCI2.iT_skladCD , currRowIdx, false);
         artiklTS     = theDUC.TheG2.GetStringCell(theDUC.DgvCI2.iT_artiklTS, currRowIdx, false);

         if(theDUC is MOD_PTG_DUC && theDUC.TheG2.CurrentCell.ColumnIndex == theDUC.DgvCI2.iR_artiklCD_Old)
         {
            currArtiklCD = theDUC.TheG2.GetStringCell(theDUC.DgvCI2.iR_artiklCD_Old, currRowIdx, false);
         }
         else
         {
            currArtiklCD = theDUC.TheG2.GetStringCell(theDUC.DgvCI2.iT_artiklCD, currRowIdx, false);
         }
      }
      else
      {
         currSkladCD = ZXC.PTG_ZNJ/*"ZNJ"*/;
         artiklTS    = ZXC.PCK_TS;

       //currArtiklCD = VvUserControl.ArtiklSifrar.OrderBy(art => art.ArtiklCD).FirstOrDefault(art => art.TS == ZXC.PCK_TS).ArtiklCD;

         currArtiklCD = PCK_ArtiklList_UC.GetFirstActivePCKartiklCD(TheDbConnection, currSkladCD, "");
      }

label_execute:

      if(currArtiklCD.NotEmpty() && artiklTS == ZXC.PCK_TS)
      {
       //List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, TheVvUC.Get_Artikl_FromVvUcSifrar(currArtiklCD), currSkladCD, ZXC.PCK_Info_Kind.OvaBazaOnly, false, false);
       //List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, null                                           , currSkladCD, ZXC.PCK_Info_Kind.SveBazeOnly, false, false);
         List<PCK_Artikl> PCK_ArtikList      = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, TheVvUC.Get_Artikl_FromVvUcSifrar(currArtiklCD), currSkladCD, ZXC.PCK_Info_Kind.OvaBazaOnly, ""    , ""  );
         List<PCK_Artikl> PCK_SviArtikliList = RtranoDao.Get_PCK_ArtiklList_ByPCK_Baza_AndSklad(TheDbConnection, null                                           , currSkladCD, ZXC.PCK_Info_Kind.SveBazeOnly, ""    , ""  );

         PCK_ArtiklList_Dlg pckDaoDlg = new PCK_ArtiklList_Dlg(currArtiklCD, currSkladCD/*, PCK_ArtiklList_Caller.SubModulAction*/);
   
         pckDaoDlg.TheUC.PutDgvFields(PCK_ArtikList, PCK_SviArtikliList, currArtiklCD, currSkladCD);
         
         pckDaoDlg.ShowDialog();
         pckDaoDlg.Dispose();      
      }
   }

   private void RISK_PTG_Modificiraj(object sender, EventArgs e)
   {
      #region Init

      if(TheVvUC is FakturDUC == false) return;

      FakturExtDUC theDUC     = TheVvDocumentRecordUC as FakturExtDUC;
      
      if(theDUC.TheG.CurrentRow == null) return;

      int currRowIdx = theDUC.TheG.CurrentRow.Index;

      string  currArtiklCD   = theDUC.TheG.GetStringCell (theDUC.DgvCI.iT_artiklCD  , currRowIdx, false);
      string  currArtiklName = theDUC.TheG.GetStringCell (theDUC.DgvCI.iT_artiklName, currRowIdx, false);
      string  currSkladCD    = theDUC.TheG.GetStringCell (theDUC.DgvCI.iT_skladCD   , currRowIdx, false);
      string  artiklTS       = theDUC.TheG.GetStringCell (theDUC.DgvCI.iT_artiklTS  , currRowIdx, false);
      decimal mocKol         = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_kol       , currRowIdx, false);
      decimal ciljRAM        = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_doCijMal  , currRowIdx, false);
      decimal ciljHDD        = theDUC.TheG.GetDecimalCell(theDUC.DgvCI.iT_noCijMal  , currRowIdx, false);

      if(currArtiklCD.IsEmpty() || artiklTS != ZXC.PCK_TS) return;

      Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklCD == currArtiklCD);

      VvModificiraj_PTG_Dlg modDlg = new VvModificiraj_PTG_Dlg(currArtiklCD);

      modDlg.Fld_PTG_MODkomada = mocKol;

      DialogResult dialogResult = modDlg.ShowDialog();

      mocKol = modDlg.Fld_PTG_MODkomada;

      modDlg.Dispose();

      if(dialogResult != DialogResult.OK) return;

      #endregion Init

      // _________ here we go _________ 

      #region RISK_CopyToSomeOtherTT - procisceni

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.R_MOD_PTG);

      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      ZXC.FakturRec = (Faktur)(TheVvDataRecord.CreateNewRecordAndCloneItComplete()); // trebamo ga za PutDefaultDUCfields 

      //ZXC.RISK_CopyToOtherDUC_inProgress = true;

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      #endregion RISK_CopyToSomeOtherTT - procisceni

      NewRecord_OnClick(null, EventArgs.Empty);

      MOD_PTG_DUC theMOD_DUC = TheVvUC as MOD_PTG_DUC;

      theMOD_DUC.PutAllKupdobFields(TheVvUC.Get_Kupdob_FromVvUcSifrar(ZXC.FakturRec.KupdobCD));

    //theMOD_DUC.Fld_Napomena     = "qweqwe"             ;
      theMOD_DUC.Fld_DokDate      = ZXC.FakturRec.DokDate; // MOD ne smije imati datum kasniji od UgAn-a 
      theMOD_DUC.Fld_SkladCD      = currSkladCD          ;
      theMOD_DUC.Fld_V1_tt        = ZXC.FakturRec.TT     ;
      theMOD_DUC.Fld_V1_ttNum     = ZXC.FakturRec.TtNum  ;
      theMOD_DUC.Fld_PrjArtCD     = currArtiklCD         ;
      theMOD_DUC.Fld_PrjArtName   = currArtiklName       ;

      if(artikl_rec != null)
      {
         theMOD_DUC.Fld_PTG_RamKlasa = artikl_rec.Grupa2CD;
         theMOD_DUC.Fld_PTG_HddKlasa = artikl_rec.Grupa3CD;

         theMOD_DUC.Fld_Decimal01 = ciljRAM;
         theMOD_DUC.Fld_Decimal02 = ciljHDD;
         theMOD_DUC.Fld_someMoney = mocKol ;
      }

      // MOC rtrano stavke: 

      Rtrano rtrano_rec;

      for(int rIdx = 0; rIdx < mocKol; ++ rIdx)
      {
         rtrano_rec = new Rtrano()
         {
            T_TT         = Faktur.TT_MOC,
            T_artiklCD   = currArtiklCD,
            T_artiklName = currArtiklName,
            T_skladCD    = currSkladCD,
            T_kol        = 1,
            T_dimZ       = ciljRAM,
            T_decC       = ciljHDD,
            
         };

         theMOD_DUC.TheG2.Rows.Add();

         theMOD_DUC.PutDgvLineFields2(rtrano_rec, rIdx, true);

         theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_ramKlasa, rIdx, artikl_rec.Grupa2CD);
         theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_hddKlasa, rIdx, artikl_rec.Grupa3CD);
         theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_artiklTS, rIdx, ZXC.PCK_TS         );

         theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iR_artiklCD_Old, rIdx, "");

      }

      theMOD_DUC.TheG2.ClearSelection();

   } //private void RISK_PTG_Modificiraj(object sender, EventArgs e) 

   private void RISK_PTG_AddModOnUGAN(object sender, EventArgs e)
   {
      FakturExtDUC theDUC        = TheVvDocumentRecordUC as FakturExtDUC;
      Faktur       MODfaktur_rec = TheVvDataRecord       as Faktur      ;

      string returnMessage = "Modifikacija nije povezana sa ugovorom ili nema MOC redaka.";

    //if(MODfaktur_rec.V1_tt != Faktur.TT_UGN && MODfaktur_rec.V1_tt != Faktur.TT_AUN                                        ) { ZXC.aim_emsg(MessageBoxIcon.Stop, returnMessage); return; } // todo? dodati DOD, povrat, ...
      if(MODfaktur_rec.V1_tt != Faktur.TT_UGN && MODfaktur_rec.V1_tt != Faktur.TT_AUN && MODfaktur_rec.V1_tt != Faktur.TT_DIZ) { ZXC.aim_emsg(MessageBoxIcon.Stop, returnMessage); return; } // todo? dodati DOD, povrat, ...
      if(MODfaktur_rec.V1_ttNum.IsZero())                                                                                      { ZXC.aim_emsg(MessageBoxIcon.Stop, returnMessage); return; }
      if(MODfaktur_rec.Transes2.Any(rto => rto.T_TT == Faktur.TT_MOC) == false)                                                { ZXC.aim_emsg(MessageBoxIcon.Stop, returnMessage); return; }

      string targetTT    = MODfaktur_rec.V1_tt   ;
      uint   targetTtNum = MODfaktur_rec.V1_ttNum;

      FakturDUC.GoTo_RISK_Dokument(targetTT, targetTtNum);

      Faktur UGANDOfaktur_rec = TheVvDataRecord as Faktur;

      List<Rtrano> MOCrtranos     = MODfaktur_rec   .Transes2.Where(rto => rto.T_TT == Faktur.TT_MOC).ToList();
      List<Rtrans> UGANDOrtranses = UGANDOfaktur_rec.Transes;

      if(Check_MODrtranos_2_UGANrtranos_alreadyPaired(MOCrtranos, UGANDOfaktur_rec.Transes2))
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski brojevi su već upareni?!");
         return;
      }

      //ZXC.RISK_Edit_RtranoOnly_InProgress = true; ugasio 26.12.2024. ponavlja se u RISK_PTG_Add_UgAnDo_Serno_Rtrano

      RISK_PTG_Add_UgAnDo_Serno_Rtrano(MOCrtranos, EventArgs.Empty);

   }

   private bool Check_MODrtranos_2_UGANrtranos_alreadyPaired(List<Rtrano> MOCrtranos, List<Rtrano> UGANDOrtranos)
   {
      return false; // todo
   }

   private void RISK_PTG_SwitchPlusMinus_RAMorHDD(object sender, EventArgs e)
   { 
      MOD_PTG_DUC theMOD_DUC = TheVvUC as MOD_PTG_DUC;

      int currRowIdx = theMOD_DUC.TheG2.CurrentCellAddress.Y;
      int currColIdx = theMOD_DUC.TheG2.CurrentCellAddress.X;

      string theTT = theMOD_DUC.TheG2.GetStringCell(theMOD_DUC.DgvCI2.iT_TT, currRowIdx, false);

      if(theTT != Faktur.TT_MOI && theTT != Faktur.TT_MOU) return;

    //decimal currValue = theMOD_DUC.TheG2.GetDecimalCell(currColIdx, currRowIdx, false);
      
      decimal ramPlus  = theMOD_DUC.TheG2.GetDecimalCell(theMOD_DUC.DgvCI2.iT_RAM_plus , currRowIdx, false);
      decimal ramMinus = theMOD_DUC.TheG2.GetDecimalCell(theMOD_DUC.DgvCI2.iT_RAM_minus, currRowIdx, false);
      decimal hddPlus  = theMOD_DUC.TheG2.GetDecimalCell(theMOD_DUC.DgvCI2.iT_HDD_plus , currRowIdx, false);
      decimal hddMinus = theMOD_DUC.TheG2.GetDecimalCell(theMOD_DUC.DgvCI2.iT_HDD_minus, currRowIdx, false);

    //if(currValue.IsZero()) return;
      if((ramPlus  + 
          ramMinus + 
          hddPlus  + 
          hddMinus ).IsZero()) return;

    //if(currColIdx == theMOD_DUC.DgvCI2.iT_RAM_plus ) { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_plus , currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_minus, currRowIdx, currValue); }
    //if(currColIdx == theMOD_DUC.DgvCI2.iT_RAM_minus) { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_minus, currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_plus , currRowIdx, currValue); }
    //if(currColIdx == theMOD_DUC.DgvCI2.iT_HDD_plus ) { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_plus , currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_minus, currRowIdx, currValue); }
    //if(currColIdx == theMOD_DUC.DgvCI2.iT_HDD_minus) { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_minus, currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_plus , currRowIdx, currValue); }
      if(ramPlus .NotZero())                           { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_plus , currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_minus, currRowIdx, ramPlus ); }
      if(ramMinus.NotZero())                           { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_minus, currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_RAM_plus , currRowIdx, ramMinus); }
      if(hddPlus .NotZero())                           { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_plus , currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_minus, currRowIdx, hddPlus ); }
      if(hddMinus.NotZero())                           { theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_minus, currRowIdx, 0M); theMOD_DUC.TheG2.PutCell(theMOD_DUC.DgvCI2.iT_HDD_plus , currRowIdx, hddMinus); }

      theMOD_DUC.SetRow_TT_and_Color_and_Calc_newRam_newHdd(this, EventArgs.Empty);
      theMOD_DUC.Put_MOD_RAM_HDD_PlusMinus_ColSum_OnSumGrid();
      theMOD_DUC.Put_MOD_Semafor_Labels();

      ZXC.TheVvForm.VvFlag_PretendDgvCurrentCellIsInEditMode = true;
      SetDirtyFlag(this); 
   }

   private void ViewLookUp_PCKpricesPerGB(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.R_MOD_PTG);
      VvLookUpLista luiList = ZXC.luiListaPCKpricesPerGB;

      luiList.LazyLoad();
      LookUpItem_ListView_Dialog dlg;

      dlg      = new LookUpItem_ListView_Dialog(ZXC.luiListaPCKpricesPerGB, ats_SubModulSet[xy.X][xy.Y]);
      dlg.Text = "RAM/HDD cijenaGB";

      dlg.listView.Columns[0].Width = ZXC.Q5un;
      dlg.listView.Columns[1].Width = ZXC.Qun12;
      dlg.listView.Columns[2].Width = ZXC.Q3un;
      dlg.listView.Columns[3].Width = ZXC.Qun12;
      dlg.listView.Columns[4].Width = ZXC.Qun12;
      dlg.listView.Columns[5].Width = ZXC.Q4un + ZXC.Qun2;

      dlg.ShowDialog();
      dlg.Dispose();

   }

   private void RISK_PTG_PVR_AddAllUNJ  (object sender, EventArgs e) 
   {
      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      uint UGAN_ttNum        = theDUC.UGAN_ttNum_ofThisDUC;
      string povratNaSkladCD = theDUC.faktur_rec.SkladCD2;

      if(theDUC.TheG2.RowCount > 1)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop, "Ova akcija smije se pozivati samo ukoliko je tablica prazna, tj. nema prethodno dodanih redaka.");
         return;
      }

      List<Rtrano> UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList_stillUNJonly(TheDbConnection, UGAN_ttNum);

      foreach(Rtrano rtrano_rec in UGAN_RtranoList)
      {
         //rtrano.T_paletaNo = 0;
         rtrano_rec.T_skladCD = povratNaSkladCD;

         uint theT_RecID = theDUC.Get_UgAnDod_Rtrans_T_recID_fromRtrano(rtrano_rec);

         rtrano_rec.T_rtrRecID = theT_RecID;
      }


      int rowIdx, idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG2);

      foreach(Rtrano rtrano_rec in UGAN_RtranoList)
      {
         //rtrano_rec.T_rtrRecID = 0; !? 

         theDUC.TheG2.Rows.Add();
      
         rowIdx = theDUC.TheG2.RowCount - idxCorrector;

         rtrano_rec.T_TT = Faktur.TT_PV2;

         theDUC.PutDgvLineFields2(rtrano_rec, rowIdx, true);
      } 
   }
   private void RISK_PTG_PVR_Select_All       (object sender, EventArgs e) { RISK_Set_CBcol_G2(RISK_CB_Column_Actions.Select_All);       }
   private void RISK_PTG_PVR_Deselect_All     (object sender, EventArgs e) { RISK_Set_CBcol_G2(RISK_CB_Column_Actions.Deselect_All);     }
   private void RISK_PTG_PVR_Invert_Selection (object sender, EventArgs e) { RISK_Set_CBcol_G2(RISK_CB_Column_Actions.Invert_Selection); }

   private void RISK_Set_CBcol_G2(RISK_CB_Column_Actions action)
   {
      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;

      int colIdx = theDUC.DgvCI2.iT_selection;

      bool willBeChecked;
      bool currChkState ;

      Cursor.Current = Cursors.WaitCursor;

      for(int rowIdx = 0; rowIdx < theDUC.TheG2.RowCount - 1; ++rowIdx)
      {
       //currChkState = VvCheckBox.GetBool4String(theDUC.TheG2.GetStringCell(colIdx, rowIdx, false));
         currChkState =                          (theDUC.TheG2.GetBoolCell  (colIdx, rowIdx, false));

         switch(action)
         {
            case RISK_CB_Column_Actions.Select_All      : willBeChecked =         true ; break;
            case RISK_CB_Column_Actions.Deselect_All    : willBeChecked =         false; break;
            case RISK_CB_Column_Actions.Invert_Selection: willBeChecked = !currChkState; break;

            default: willBeChecked = false; break;
         }

       //theDUC.TheG2.PutCell(colIdx, rowIdx, VvCheckBox.GetString4Bool(willBeChecked));
         theDUC.TheG2.PutCell(colIdx, rowIdx,                          (willBeChecked));
      }

      Cursor.Current = Cursors.Default;
   }

   private void RISK_PTG_PVR_DelUnCheck (object sender, EventArgs e) 
   {
      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;

      int colIdx = theDUC.DgvCI2.iT_selection;

      bool currChkState ;

      Cursor.Current = Cursors.WaitCursor;

      for(int rowIdx = 0; rowIdx < theDUC.TheG2.RowCount - 1; ++rowIdx)
      {
         currChkState = (theDUC.TheG2.GetBoolCell  (colIdx, rowIdx, false));

         if(currChkState == false) theDUC.TheG2.Rows[rowIdx].Selected = true;
      }

      foreach(DataGridViewRow row in theDUC.TheG2.SelectedRows)
      {
         if(row.IsNewRow == false) theDUC.TheG2.Rows.Remove(row);
      }

      Cursor.Current = Cursors.Default;

   }

   private void RISK_GetLast_UgAn_ForThisSerno(object sender, EventArgs e)
   {
      VvGetLastFakturForThisSernoDlg dlg = new VvGetLastFakturForThisSernoDlg("UgAn Tražilica preko serijskog broja");

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string theWantedSerno = dlg.Fld_TheWantedSerno;

      dlg.Dispose();
            
      // here we go 

      UGNorAUN_PTG_DUC theDUC = TheVvDocumentRecordUC as UGNorAUN_PTG_DUC;

      Faktur last_UgAn_rec_forThisSerno = new Faktur();

      bool fakturFound = theDUC.GetLastFakturForThisSerno(TheDbConnection, last_UgAn_rec_forThisSerno, theWantedSerno);

      if(fakturFound == false)
      {
         //ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći ugovor / aneks na kojem se (kao zadnje stanje) nalazi ovaj serijski broj.\n\r\n\r", theWantedSerno);

         ZXC.PTG_lastUsedSerno = "";

         return;
      }

      ZXC.PTG_lastUsedSerno = theWantedSerno;

      FakturDUC.GoTo_RISK_Dokument(last_UgAn_rec_forThisSerno.TT, last_UgAn_rec_forThisSerno.TtNum);
   }

   private void RISK_GetFirst_UgAn_ForKupdobAndArtikl(object sender, EventArgs e)
   {
      VvGetFirstFakturForThisArtiklAndKupdobDlg dlg = new VvGetFirstFakturForThisArtiklAndKupdobDlg();

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string theWantedArtiklCD = dlg.Fld_ArtiklCd;
      uint   theWantedKupdobCD = dlg.Fld_KupdobCd;
      dlg.Dispose();

      List<Faktur> uganList = RtransDao.GetUgAnFakturList_forThis_KupdobAndArtikl(TheDbConnection, theWantedKupdobCD, theWantedArtiklCD);

      List<ZXC.VvUtilDataPackage> udpList = uganList.Select(fak => new ZXC.VvUtilDataPackage() { TheStr1 = fak.TT_And_TtNum, TheStr2 = fak.KupdobName }).ToList();

      string naziv = "UGAN za artikl " + theWantedArtiklCD;
     
      string uganTTiTtNum = UDP_Dlg.ChooseUDP(udpList, naziv, 2).TheStr1;

      if(uganTTiTtNum == null) return;

      if(uganTTiTtNum.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći ugovor / aneks po partneru\n\r\n\r{0}\n\r\n\rna kojem se nalazi artikl.\n\r\n\r{1}", theWantedKupdobCD, theWantedArtiklCD);
         return;
      }

      string tt;
      uint ttNum;
      bool parseOK = Ftrans.ParseTipBr(uganTTiTtNum, out tt, out ttNum);

      if(parseOK == false) return;

      FakturDUC.GoTo_RISK_Dokument(tt, ttNum);


      // here we go 

      // UGNorAUN_PTG_DUC theDUC = TheVvDocumentRecordUC as UGNorAUN_PTG_DUC;
      //
      // Faktur first_UgAn_rec_forThis_KupdobAndArtikl = new Faktur();
      //
      // bool uganFakturFound = RtransDao.Getfirst_UgAn_rec_forThis_KupdobAndArtikl(TheDbConnection, first_UgAn_rec_forThis_KupdobAndArtikl, theWantedKupdobCD, theWantedArtiklCD);
      //
      // if(uganFakturFound == false)
      // {
      //    ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu naći ugovor / aneks po partneru\n\r\n\r{0}\n\r\n\rna kojem se nalazi artikl.\n\r\n\r{1}", theWantedKupdobCD, theWantedArtiklCD);
      //
      //    return;
      // }
      //
      // FakturDUC.GoTo_RISK_Dokument(first_UgAn_rec_forThis_KupdobAndArtikl.TT, first_UgAn_rec_forThis_KupdobAndArtikl.TtNum);
   }

   private void RISK_PTG_DajRealSerno(object sender, EventArgs e)
   { 
      FakturPDUC theDUC = TheVvDocumentRecordUC as FakturPDUC;

      if(theDUC.TheG2.CurrentRow == null) return;

      int currRowIdx = theDUC.TheG2.CurrentRow.Index;

      Point currCell = theDUC.TheG2.CurrentCellAddress;

      if(currCell.X != theDUC.DgvCI2.iT_serno) return;

      Rtrano rtrano_rec = (Rtrano)theDUC.GetDgvLineFields2(currRowIdx, false, null);

      if(rtrano_rec.T_TT != Faktur.TT_PV2 && rtrano_rec.T_TT != Faktur.TT_ZU2) return;

      string oldOlfaSerno = rtrano_rec.T_serno;

      if(oldOlfaSerno.IsEmpty()) return;

      if(oldOlfaSerno.StartsWith(ZXC.PTG_PENDING_SernoPreffix) == false) return;

      VvGetLastFakturForThisSernoDlg dlg = new VvGetLastFakturForThisSernoDlg("Real serijski broj");

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string newRealSerno = dlg.Fld_TheWantedSerno;

      dlg.Dispose();

      #region Check for double serno entry

      int theSernoCount = theDUC.SernoCountOnGrid(theDUC.TheG2, newRealSerno);

      if(theSernoCount/* > 1*/.NotZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Na dokumentu ovaj serijski broj već postoji!\n\r\n\r{0}", newRealSerno);
         //theDUC.TheG2.EndEdit();
         //theDUC.TheG2.PutCell(ci2.iT_serno, currRowIdx, "");
         //e.Cancel = true;
         return;
      }

      #endregion Check for double serno entry

      #region Smije li ovaj serno doc na ovaj POVRAT?

      Rtrano last_rtrano_rec_forThisSerno = new Rtrano();

      bool isLastRtrano_ForSerno_found = RtranoDao.Get_LastRtrano_ForSerno(TheDbConnection, last_rtrano_rec_forThisSerno, newRealSerno, true);

      if(isLastRtrano_ForSerno_found) // znaci nije uparivanjeSernoa nego regularan povrat sernoa koji je otisao u najam 
      {
         uint UGAN_ttNum = theDUC.UGAN_ttNum_ofThisDUC;

         List<Rtrano> UGAN_RtranoList = RtranoDao.Get_UGAN_RtranoList_stillUNJonly(TheDbConnection, UGAN_ttNum);

         if(UGAN_RtranoList.Contains(last_rtrano_rec_forThisSerno) == false)
         {
            ZXC.aim_emsg(MessageBoxIcon.Stop, "Serijski broj NIJE u ovom najmu?!\n\r\n\r{0}", last_rtrano_rec_forThisSerno);
            //e.Cancel = true;
            //theGrid2.EndEdit();
            //theGrid2.PutCell(ci2.iT_serno, currRowIdx, "");
            return;
         }
      }

      #endregion smije li ovaj serno doc na ovaj POVRAT?

      List<Rtrano> rtranoList = RtranoDao.GetRtranoList_For_SERNO(TheDbConnection, oldOlfaSerno);

      foreach(Rtrano rtrano in rtranoList)
      {
         BeginEdit(rtrano);

         rtrano.T_serno = newRealSerno;

         rtrano.VvDao.RWTREC(TheDbConnection, rtrano, false, true, false);

         EndEdit(rtrano);
      }

      theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_serno, currRowIdx, newRealSerno);

      ZXC.aim_emsg("Gotovo. Preimenovao {0} stavaka.", rtranoList.Count());
   }

   #endregion PCTOGO

   internal void ShowFakturDUC_For_TipBr(string tipBr)
   {
      string tt;
      uint   ttNum;
      bool parseOK = Ftrans.ParseTipBr(tipBr, out tt, out ttNum);

      if(parseOK == false) return;

      ZXC.VvSubModulEnum vvSubModulEnum = FakturDUC.GetVvSubModulEnum_ForTT(tt);
      if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

      if(ZXC.IsPCTOGO)
      {
         if(ZXC.TtInfo(tt).LinkedDefaultTT.NotEmpty())  tt = ZXC.TtInfo(tt).LinkedDefaultTT; 

       //if(tt == Faktur.TT_DI2) tt = Faktur.TT_DIZ;
       //if(tt == Faktur.TT_PV2) tt = Faktur.TT_PVR;
       //if(tt == Faktur.TT_ZI2) tt = Faktur.TT_ZIZ;
      }


      Faktur faktur_rec = new Faktur();
      bool fakOK = FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);
      if(fakOK == false) return;

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(vvSubModulEnum);
    //ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;
    //VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);
    //
    //if(existingTabPage != null)
    //   existingTabPage.Selected = true; 
    //else
      OpenNew_Record_TabPage(vvSubModul.xy, faktur_rec.RecID);
   }

   internal void ShowNalogDUC_For_DokNum(string dokNum)
   {
      ZXC.VvSubModulEnum vvSubModulEnum = ZXC.VvSubModulEnum.NAL_F;
      if(vvSubModulEnum == ZXC.VvSubModulEnum.UNDEF) return;

      Nalog nalog_rec = new Nalog();
      bool nalOK = nalog_rec.VvDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, nalog_rec, dokNum, ZXC.NalogSchemaRows[ZXC.NalCI.dokNum], false, false);

      if(nalOK == false) return;

      VvSubModul vvSubModul = GetVvSubModulFrom_SubModulEnum(vvSubModulEnum);
      //ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;
      //VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);
      //
      //if(existingTabPage != null)
      //   existingTabPage.Selected = true; 
      //else
      OpenNew_Record_TabPage(vvSubModul.xy, nalog_rec.RecID);
   }

   private void RISK_TgIRA_to_RozelIFA(object sender, EventArgs e)
   {
      #region Init

      VvTetragamIRA_to_RozelIFA_Dlg dlg = new VvTetragamIRA_to_RozelIFA_Dlg();

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      dlg.Dispose();
            
      // na godinu ce valjda ovo prestati trabati pa za 2025 ovak sve fiksno 
      string host     = "79.143.181.154";
      string username = "vvT1_superuser";
      string cvbcvb   = "cvbcvb"        ;
      string ticker   = "TGPROJ"        ;

    //string dbname   = ZXC.VvDB_NameConstructor(ZXC.projectYear, ticker, 1);
    //string dbname   = "vvT1_vv2025_TGPROJ_000001";
      string dbname   = "vvT1_vv" + ZXC.projectYear + "_TGPROJ_000001";

      XSqlConnection skylab_dbConn = VvSQL.CREATE_AND_OPEN_XSqlConnection(host, username, ZXC.EncryptThis_UserUC_Password(cvbcvb, username), dbname);

      bool okConn = (skylab_dbConn != null && skylab_dbConn.State == System.Data.ConnectionState.Open);

      if(!okConn) return;

    //DateTime dateOD = new DateTime(2025, 01,       03);
    //DateTime dateDO = new DateTime(2025, 01, /*31*/03).EndOfDay(); // !!! pazi kad ces raditi get dialog fields da dateDO dode na kraj dana 
      DateTime dateOD = dlg.Fld_DatumOd;
      DateTime dateDO = dlg.Fld_DatumDo;

      int skipCount_Faktur = 0;
      int newCount_Faktur  = 0;
      int newCount_Kupdob  = 0;
      int loopCounter      = 0;

      bool skyKupdobFound;

      #endregion Init

      #region Get Skylab's IRA fakturs

      List<Faktur> fakturIRAlist = new List<Faktur>();

      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);

      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "theDateOD", dateOD       , " >= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "theDateDO", dateDO       , " <= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt     ], "tt"       , Faktur.TT_IRA,  " = "));

      Cursor.Current = Cursors.WaitCursor;

      ZXC.SetStatusText("Preuzimam IRA zaglavlja");

      bool OK = VvDaoBase.LoadGenericVvDataRecordList<Faktur>(skylab_dbConn, fakturIRAlist, filterMembers, "", "dokDate, ttSort, ttNum", true);

      ZXC.SetStatusText("Preuzimam IRA stavke");

      //if(OK)
      //{
      //   fakturIRAlist.ForEach(fak => fak.VvDao.LoadTranses(skylab_dbConn, fak, false));
      //}

      //skylab_dbConn.Close();

      var msgList = new List<string>();

      var skladGroups = fakturIRAlist.GroupBy(fak => fak.SkladCD); // tu bi trebalo po OPP-u a ne pos skladCD-u 

      foreach(var/*List<Faktur>*/ skladGroupFakturList in skladGroups)
      {
         msgList.Add(String.Format("Sklad: [{0}]      BrojOD: [{1}]      BrojDO: [{2}]", skladGroupFakturList.Key, skladGroupFakturList.Min(fak => fak.TtNum), skladGroupFakturList.Max(fak => fak.TtNum)));
      }

      ZXC.ClearStatusText();

      ZXC.aim_emsg_List(string.Format("Učitavanje {2} IRA od datuma {0} do datuma {1} .", dateOD.ToString(ZXC.VvDateFormat), dateDO.ToString(ZXC.VvDateFormat), fakturIRAlist.Count), msgList);

      DialogResult result = MessageBox.Show("Da li zaista želite učitati ove IRA-a?", "NASTAVITI?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      #endregion Get Skylab's IRAs

      #region ADD local IFA fakturs

      Faktur Rozel_IFA_faktur ;
      Faktur Rozel_OLD_faktur ;
      Kupdob skylab_IRA_kupdob;
      Kupdob newKupdob_rec    ;

      bool ADD_OK, vezDokExists;

      Cursor.Current = Cursors.WaitCursor;

      foreach(Faktur skylab_IRA_faktur in fakturIRAlist)
      {
         skylab_IRA_faktur.VvDao.LoadTranses(skylab_dbConn, skylab_IRA_faktur, false);

         skylab_IRA_kupdob  = new Kupdob();
         skyKupdobFound     = ZXC.KupdobDao.SetMe_Record_bySomeUniqueColumn(skylab_dbConn, skylab_IRA_kupdob, skylab_IRA_faktur.KupdobCD, ZXC.KupdobSchemaRows[ZXC.KpdbCI.kupdobCD], false);
         
         if(skyKupdobFound == false) skylab_IRA_kupdob = null;

         (Rozel_IFA_faktur, newKupdob_rec) = Faktur.Get_RozelIFA_from_skylabIRA_faktur(TheDbConnection, skylab_IRA_faktur, skylab_IRA_kupdob);

         #region Skip on VezniDok already exists

         Rozel_OLD_faktur = new Faktur();

         vezDokExists = ZXC.FakturDao.SetMe_Record_bySomeUniqueColumn(TheDbConnection, Rozel_OLD_faktur, Rozel_IFA_faktur.VezniDok, ZXC.FakturSchemaRows[ZXC.FakCI.vezniDok], false, true);

         // zanemari ako nađe na YRN-u, ali bu zajeb ako ga ima i u IFA-ma ... ne da mi se LoadGeneric i FM-ove, ko ih jbe 
         if(vezDokExists && Rozel_OLD_faktur.TT == Faktur.TT_YRN) vezDokExists = false;

         if(vezDokExists)
         {
            skipCount_Faktur++;
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Prema Veznom dokumentu\n\r\n\r{0}\n\r\n\rOvaj račun već postoji te ga preskačem!\n\r\n\r{1}", Rozel_IFA_faktur.VezniDok, Rozel_OLD_faktur);
            continue;
         }

         #endregion Skip on VezniDok already exists

         ADD_OK = Rozel_IFA_faktur.VvDao.ADDREC(TheDbConnection, Rozel_IFA_faktur);

         if(ADD_OK)
         {
            newCount_Faktur++;

            #region eventual ADD NEW Kupdob

            if(newKupdob_rec != null)
            {
               ADD_OK = newKupdob_rec.VvDao.ADDREC(TheDbConnection, newKupdob_rec);

               if(ADD_OK)
               {
                  newCount_Kupdob++;
                  TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Code, true);
               }
               else
               {
                  ZXC.aim_emsg(MessageBoxIcon.Error, "Kupac nije dodan u adresar!\n\r\n\r", newKupdob_rec);
               }
            }

            #endregion eventual ADD NEW Kupdob

         }
         else
         {
            result = MessageBox.Show("Da li želite nastaviti učitatavanje IRA-a?", "Pojavio se problem, NASTAVITI?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(result != DialogResult.Yes) return;
         }

         ZXC.SetStatusText(String.Format("Obradio {0}. od {1}", ++loopCounter, fakturIRAlist.Count));
      }

      #endregion ADD local IFA fakturs

      #region finish

      skylab_dbConn.Close();

      Cursor.Current = Cursors.Default;

      ZXC.ClearStatusText();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\r\n\rDodao {0} novih IFA\n\r\n\rPreskočio {1} starih po vez. dok.\n\r\n\rDodao {2} novih kupaca", newCount_Faktur, skipCount_Faktur, newCount_Kupdob);

      #endregion finish

   }

   public void QPrint_ZAR(object sender, EventArgs e) 
   {
      FakturExtDUC theDUC = TheVvUC as FakturExtDUC;

      theDUC.TheFakturDocFilterUC.IsPrn_ZAR = true; 
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(false);
      theDUC.TheFakturDocFilterUC.IsPrn_ZAR = false;
   }
   public void QPrint_UZP(object sender, EventArgs e)
   {
      FakturExtDUC theDUC = TheVvUC as FakturExtDUC;

      theDUC.TheFakturDocFilterUC.IsPrn_UZP = true;
      ((VvRecordUC)TheVvUC).ShowRecordReportPreview_Or_QuickPrintRecord(false);
      theDUC.TheFakturDocFilterUC.IsPrn_UZP = false;
   }


   #endregion Risk

   #region Common DocumentUC Actions

   public void TakeGridRowsFromOtherDocument(object sender, EventArgs e)
   {
      VvAddDataGridRowFromOtherDocDlg dlg = new VvAddDataGridRowFromOtherDocDlg();

      if(dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      if(ZXC.ThisIsVektorProject)
      {
         TheVvDocumentRecordUC.TakeGridRowsFromOtherDocument(dlg.Fld_TT, dlg.Fld_TtNum, 0);
      }
      else
      {
         TheVvDocumentRecordUC.TakeGridRowsFromOtherDocument("", 0, dlg.Fld_DocNum);
      }
   }

   /*private*/public void DocumentUC_InsertRow(object sender, EventArgs e)
   {
      VvDataGridView theGrid;

      if(TheVvDataRecord.IsPolyDocument)
      {
         theGrid = TheVvPolyDocumRecordUC.TheCurrentG;
      }
      else
      {
         theGrid = TheVvDocumentRecordUC.TheG;
      }

      int currRowIdx = theGrid.CurrentCellAddress.Y;

      if(currRowIdx < 0) return;

      theGrid.Rows.Insert(currRowIdx, 1);

      // 2.9.2010:
      //theGrid.CurrentCell = theGrid.Rows[currRowIdx].Cells[2];
      // jer npr u slucaju MedjuskladDUC Grida, prva visible column je [3], zbog specificne dodene kolone T_TwinID na pocetku 
      int firstVisibleColumnIndex;
      for(firstVisibleColumnIndex = 0; theGrid.Rows[currRowIdx].Cells[firstVisibleColumnIndex].Visible == false && firstVisibleColumnIndex < theGrid.ColumnCount; ++firstVisibleColumnIndex);
      theGrid.CurrentCell = theGrid.Rows[currRowIdx].Cells[firstVisibleColumnIndex];
   }

   /*private*/public void DocumentUC_DeleteRow(object sender, EventArgs e)
   {
      VvDataGridView theGrid;

      if(TheVvDataRecord.IsPolyDocument)
      {
         theGrid = TheVvPolyDocumRecordUC.TheCurrentG;
      }
      else
      {
         theGrid = TheVvDocumentRecordUC.TheG;
      }

      int currRowIdx = theGrid.CurrentCellAddress.Y;

      if(currRowIdx < 0 || currRowIdx == theGrid.NewRowIndex) return;

      // 29.09.2017: 
      if(ZXC.TH_Should_ESC_DRW_Log)
      {
         TheVvUC.GetFields(false);
         //bool OK = 
         XtransDao.Addrec_ESC_DRW_Log(TheDbConnection, "DR1", TheVvDataRecord as Faktur, currRowIdx);
      }

      theGrid.Rows.RemoveAt(currRowIdx);
   }

   /*private*/public void DocumentUC_DeleteManyRows(object sender, EventArgs e)
   {
      VvDataGridView theGrid;

      VvDocumentRecordUC theDocumentRecordUC = TheVvUC as VvDocumentRecordUC;

      if(TheVvDataRecord.IsPolyDocument)
      {
         theGrid = TheVvPolyDocumRecordUC.TheCurrentG;
      }
      else
      {
         theGrid = TheVvDocumentRecordUC.TheG;
      }

      // 29.09.2017: 
      if(ZXC.TH_Should_ESC_DRW_Log)
      {
         TheVvUC.GetFields(false);
         //bool OK = 
         XtransDao.Addrec_ESC_DRW_Log(TheDbConnection, "DRX", TheVvDataRecord as Faktur, /*currRowIdx*/-1);
      }

      if(theGrid.SelectedRows.Count.IsZero())
      {
         theGrid.SelectAll();
      }

      //theGrid.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(theDocumentRecordUC.grid_RowsRemoved);

      foreach(DataGridViewRow row in theGrid.SelectedRows)
      {
         if(row.IsNewRow == false) theGrid.Rows.Remove(row);
      }

      //theGrid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(theDocumentRecordUC.grid_RowsRemoved);

#if biloNekad

      if(theGrid.SelectedRows.Count.IsZero())
      {
         //theGrid.Rows.Clear();
         while(theGrid.RowCount > 1)
         {
            theGrid.Rows.RemoveAt(0);
         }
      }
      else
      {
         foreach(DataGridViewRow row in theGrid.SelectedRows)
         {
            if(row.IsNewRow == false) theGrid.Rows.Remove(row);
         }
      }

#endif

   }

   #endregion Common DocumentUC Actions


   #region ImportFromOffix

   /*private*/
   public void ImportFromOffix/*_ORIG*/(object sender, EventArgs e)
   {

      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName &&
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Privilegirana akcija. Odbijeno.");
         return;
      }

      ImportFromOffix_JOB(false);

   }

   private void UcitajOffixRacune(object sender, EventArgs e)
   {
      //ImportFromOffix_JOB(true);
      ZXC.aim_emsg("UcitajOffixRacune je Past USE!!!");
   }

   private void ImportFromOffix_JOB(bool isUcitajRacune)
   {
      #region Check Prerequisites
      
      if(TheVvDataRecord.VirtualRecordName == Nalog .recordName ||
         TheVvDataRecord.VirtualRecordName == Osred .recordName ||
         TheVvDataRecord.VirtualRecordName == Person.recordName ||
         TheVvDataRecord.VirtualRecordName == Artikl.recordName ||
         TheVvDataRecord.VirtualRecordName == Placa .recordName)
      {
         int? kupdobCount = VvDaoBase.CountAllRecords(TheDbConnection, Kupdob.recordName);

         if(kupdobCount == null || kupdobCount == 0)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "UPOZORENJE: Tablica 'Poslovnog Adresara' je prazna!\n\nPreporica se da prvo importirate Poslovni Adresar.");
         }
      }

      
      #endregion Check Prerequisites

      #region Initializations, ...

      bool ftpOK = true;
      int  nora=-1, transNora;

      VvTransRecord vvTransRecord=null;

      LoginToLinuxForm loginForm = new LoginToLinuxForm(isUcitajRacune);

      loginForm.Fld_FileName = SuggestImportFileName(TheVvDataRecord.VirtualLegacyRecordPreffix, ZXC.CURR_prjkt_rec.KupdobCD, isUcitajRacune);

      if(TheVvDataRecord.IsDocument && !isUcitajRacune)
      {
         vvTransRecord = ((VvDocumentRecord)TheVvDataRecord).VvTransRecordFactory();

         loginForm.Fld_FileNameTrans = SetTransImportFileName(vvTransRecord.VirtualLegacyRecordPreffix, loginForm.Fld_FileName, TheVvDataRecord.VirtualLegacyRecordPreffix);
         loginForm.tbx_fileNameTrans.Visible = true;
      }


      if(loginForm.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.OffixImport_InProgress = true;

      string currentLocalDirerctory = System.Environment.CurrentDirectory;

      Environment.CurrentDirectory = Get_MyDocumentsLocation_ProjectAndUser_Dependent(false);

      #endregion Initializations, ...

      #region FTP

      // ___ Here we GO! FTP GetFIle _____________________________________________________________________________

      if(loginForm.Fld_SkipFtp == false)
      {

         if(loginForm.Fld_IsNewFTP_Client == false)
         {
            #region PUSE via FTPConnection class

            FTPConnection ftp = new FTPConnection();

            try
            {
               ftp.Open(loginForm.Fld_ServerHost, loginForm.Fld_UserName, loginForm.Fld_Password, loginForm.Fld_FtpMode);

               ftp.SetCurrentDirectory(loginForm.Fld_RemoteDirectory);

               FtpGetFile_PUSE(TheVvDataRecord, loginForm.Fld_FileName, ftp, isUcitajRacune);
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška: {0}", ex.Message);

               ftpOK = false;
            }

            ftp.Close();

            #endregion PUSE via FTPConnection class
         }

         else
         {
            #region FTP SourceForge

            FTP_SourceForge ftp2 = null;

            try
            {
               ftp2 = new FTP_SourceForge(loginForm.Fld_ServerHost, loginForm.Fld_UserName, loginForm.Fld_Password, loginForm.Fld_FtpMode);

               ftp2.ChangeDir(loginForm.Fld_RemoteDirectory);

               FtpGetFile2(TheVvDataRecord, loginForm.Fld_FileName, ftp2, isUcitajRacune);
            }
            catch(Exception ex)
            {
               ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška: {0}", ex.Message);

               ftpOK = false;
            }

            if(ftp2 != null) ftp2.Disconnect();

            #endregion FTP SourceForge
         }

      } // if(loginForm.Fld_SkipFtp == false) 

      // ___ Here we END! FTP GetFIle _____________________________________________________________________________

      if(ftpOK == false)
      {
         Cursor.Current = Cursors.Default;

         ZXC.OffixImport_InProgress = false;

         return;
      }

      #endregion FTP

      #region LOAD DATA INFILE

      // ___ Here we GO! MySQL LOAD DATA INFIle _____________________________________________________________________________ 

      Cursor.Current = Cursors.WaitCursor;

      if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && (ZXC.CURR_prjkt_rec.Ticker.StartsWith("ZAGRIA") ||
                                                                    ZXC.CURR_prjkt_rec.Ticker.StartsWith("INSSO")  || // dodano u 2024 
                                                                    ZXC.CURR_prjkt_rec.Ticker.StartsWith("INSSSO") || // dodano u 2024 
                                                                    ZXC.CURR_prjkt_rec.Ticker.StartsWith("ELPERF")))
      {
         VvArtikl_ZAGRIA_Importer importer = new VvArtikl_ZAGRIA_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("DUCATI")) // U Mixer! Externi cijenik da Italy 
      {
         VvXtransImporter importer = new VvXtransImporter(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

#if(DEBUG)

      else if(TheVvDataRecord.VirtualRecordName == Person.recordName && (ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO") ||
                                                                         ZXC.CURR_prjkt_rec.Ticker.StartsWith("QQTEXT")))
      {
         VvPerson_TEXTHO_Importer importer = new VvPerson_TEXTHO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.vvDB_VvDomena == "vvFG")
      {
         VvKupdob_FRAG_Importer importer = new VvKupdob_FRAG_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.vvDB_VvDomena == "vvFG")
      {
         VvArtikl_FRAG_Importer importer = new VvArtikl_FRAG_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("KEREMP"))
      {
         VvArtikl_KEREMP_Importer importer = new VvArtikl_KEREMP_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO"))
      {
         VvArtikl_TEXTHO_Importer importer = new VvArtikl_TEXTHO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("BRADA"))
      {
         VvArtikl_BRADA_Importer importer = new VvArtikl_BRADA_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("METFLX"))
      {
         VvArtikl_METFLX_Importer importer = new VvArtikl_METFLX_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("METFLX"))
      {
         VvKupdob_METFLX_Importer importer = new VvKupdob_METFLX_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO"))
      {
         VvKupdob_TEXTHO_Importer importer = new VvKupdob_TEXTHO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Osred.recordName && ZXC.vvDB_VvDomena == "vvHZ")
      {
         VvOsred_HZTK_Importer importer = new VvOsred_HZTK_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO"))
      {
         VvArtikl_TEMBO_Importer importer = new VvArtikl_TEMBO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("VERIDI"))
      {
         VvArtikl_VERIDI_Importer importer = new VvArtikl_VERIDI_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("VELEFO"))
      {
         VvArtikl_VELEFO_Importer importer = new VvArtikl_VELEFO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("LIKUM"))
      {
       //VvArtikl_LIKUM_Importer importer = new VvArtikl_LIKUM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");
         VvArtikl_LIKUM2_Importer importer = new VvArtikl_LIKUM2_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("LIKUM"))
      {
       //VvKupdob_LIKUM_Importer importer = new VvKupdob_LIKUM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");
         VvKupdob_LIKUM2_Importer importer = new VvKupdob_LIKUM2_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("HZTK"))
      {
         VvKupdob_HZTK_Importer importer = new VvKupdob_HZTK_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO"))
      {
         VvKupdob_TEXTHO_Importer importer = new VvKupdob_TEXTHO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TURZML"))
      {
         VvKupdob_TURZML_Importer importer = new VvKupdob_TURZML_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("KEREMP"))
      {
         VvKupdob_KEREMP_Importer importer = new VvKupdob_KEREMP_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"))
      {
         VvKupdob_AGSJAJ_Importer importer = new VvKupdob_AGSJAJ_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Person.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("KEREMP"))
      {
         VvPerson_KEREMP_Importer importer = new VvPerson_KEREMP_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Person.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"))
      {
         VvPerson_AGSJAJ_Importer importer = new VvPerson_AGSJAJ_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"))
      {
         VvArtikl_AGSJAJ_Importer importer = new VvArtikl_AGSJAJ_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Osred.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"))
      {
         VvOsred_AGSJAJ_Importer importer = new VvOsred_AGSJAJ_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Osred.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TURZML"))
      {
         VvOsred_TURZML_Importer importer = new VvOsred_TURZML_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Osred.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("KEREMP"))
      {
         VvOsred_KEREMP_Importer importer = new VvOsred_KEREMP_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO"))
      {
         VvKupdob_TEMBO_Importer importer = new VvKupdob_TEMBO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("JOSAVC"))
      {
         VvArtikl_JOSAVAC_Importer importer = new VvArtikl_JOSAVAC_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("JOSAVC"))
      {
         VvKupdob_JOSAVAC_Importer importer = new VvKupdob_JOSAVAC_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("FRIGOT"))
      {
         VvArtikl_FRIGOTERM_Importer importer = new VvArtikl_FRIGOTERM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("FRIGOT"))
      {
         VvKupdob_FRIGOTERM_Importer importer = new VvKupdob_FRIGOTERM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))
      {
         VvArtikl_SENSO_Importer importer = new VvArtikl_SENSO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Nalog.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("SENSO"))
      {
         VvNalogPS_SENSO_Importer importer = new VvNalogPS_SENSO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Nalog.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEMBO"))
      {
         VvNalogPS_TEMBO_Importer importer = new VvNalogPS_TEMBO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      else if(TheVvDataRecord.VirtualRecordName == Nalog.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TEXTHO"))
      {
         VvNalogPS_TEXTHO_Importer importer = new VvNalogPS_TEXTHO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Amort.recordName && ZXC.vvDB_VvDomena == "vvHZ")
      {
         VvAtransImporter importer = new VvAtransImporter(TheDbConnection, loginForm.Fld_FileNameTrans, "\t", true);

         AtransDao.AtransCI fldRbr = new AtransDao.AtransCI();

         fldRbr.t_osredCD  = 1;
         fldRbr.t_dokDate  =20;
         fldRbr.t_tt       = 8;
         fldRbr.t_opis     = 8;
         fldRbr.t_normalAm = 0;
         fldRbr.t_kol      = 5;
         fldRbr.t_koef_am  = 0;
         fldRbr.t_amort_st =13;
         fldRbr.t_dug      = 9;
         fldRbr.t_pot      =14;

         importer.FldRbr = fldRbr;

         transNora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Amort.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("AGSJAJ"))
      {
         VvAtransImporter importer = new VvAtransImporter(TheDbConnection, loginForm.Fld_FileNameTrans, "\t", true);

         AtransDao.AtransCI fldRbr = new AtransDao.AtransCI();

         fldRbr.t_osredCD  = 1;
         fldRbr.t_dokDate  = 4;
         fldRbr.t_tt       = 3;
         fldRbr.t_opis     = 3;
         fldRbr.t_normalAm = 0;
         fldRbr.t_kol      = 6;
         fldRbr.t_koef_am  = 0;
         fldRbr.t_amort_st = 5;
         fldRbr.t_dug      = 7;
         fldRbr.t_pot      = 8;

         importer.FldRbr = fldRbr;

         transNora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Amort.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("KEREMP"))
      {
         VvAtransImporter importer = new VvAtransImporter(TheDbConnection, loginForm.Fld_FileNameTrans, "\t", true);

         AtransDao.AtransCI fldRbr = new AtransDao.AtransCI();

         fldRbr.t_osredCD  = 1;
         fldRbr.t_dokDate  = 4;
         fldRbr.t_tt       = 3;
         fldRbr.t_opis     = 3;
         fldRbr.t_normalAm = 0;
         fldRbr.t_kol      = 5;
         fldRbr.t_koef_am  = 0;
         fldRbr.t_amort_st = 6;
         fldRbr.t_dug      = 7;
         fldRbr.t_pot      = 8;

         importer.FldRbr = fldRbr;

         transNora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Amort.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("TURZML"))
      {
         VvAtransImporter importer = new VvAtransImporter(TheDbConnection, loginForm.Fld_FileNameTrans, "\t", true);

         AtransDao.AtransCI fldRbr = new AtransDao.AtransCI();

         fldRbr.t_osredCD  = 1;
         fldRbr.t_dokDate  = 2;
         fldRbr.t_tt       = 3;
         fldRbr.t_opis     = 4;
         fldRbr.t_normalAm = 5;
         fldRbr.t_kol      = 6;
         fldRbr.t_koef_am  = 7;
         fldRbr.t_amort_st = 8;
         fldRbr.t_dug      = 9;
         fldRbr.t_pot      =10;

         importer.FldRbr = fldRbr;

         transNora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Kupdob.recordName && (ZXC.CURR_prjkt_rec.Ticker.StartsWith("TGPLEM") || ZXC.CURR_prjkt_rec.Ticker.StartsWith("TGPROJ")))
      {
         VvKupdob_TGPLEM_Importer importer = new VvKupdob_TGPLEM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("PCTOGO"))
      {
         VvArtikl_PCTOGO_Importer importer = new VvArtikl_PCTOGO_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }

#endif

      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 
      // ====================================================================================== 

      else if(TheVvDataRecord.VirtualRecordName == Amort.recordName)
      {
         VvAtransImporter importer = new VvAtransImporter(TheDbConnection, loginForm.Fld_FileNameTrans, "\t", false);

         AtransDao.AtransCI fldRbr = new AtransDao.AtransCI();

         fldRbr.t_osredCD  = 1;
         fldRbr.t_dokDate  = 2;
         fldRbr.t_tt       = 3;
         fldRbr.t_opis     = 4;
         fldRbr.t_normalAm = 5;
         fldRbr.t_kol      = 6;
         fldRbr.t_koef_am  = 7;
         fldRbr.t_amort_st = 8;
         fldRbr.t_dug      = 9;
         fldRbr.t_pot      =10;

         importer.FldRbr = fldRbr;

         transNora = importer.LoadData(true);
      }

      else if(TheVvDataRecord.VirtualRecordName == Faktur.recordName)
      {
         VvRtransImporter importer = new VvRtransImporter(TheDbConnection, loginForm.Fld_FileName/*Trans*/, "\t", loginForm.Fld_IsUFA, loginForm.Fld_IsIFA, loginForm.Fld_IsForceOffix);

         if(importer.ImportKind == VvRtransImporter.ImportKindEnum.TEXTHO)
         {
            string scd;

  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "14M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "16M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "18M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "20M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "22M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "24M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "26M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "28M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "30M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "32M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "34M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "58M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "60M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
//importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "98M5"; importer.FullPathFileName = loginForm.Fld_FileName;                            nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "36M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "38M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "40M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "42M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "44M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "46M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "48M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "50M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "52M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "54M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "56M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "62M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);
  importer.RawDataList = new List<VvRtransImporter.RawTransData>(); scd = "64M2"; importer.FullPathFileName = loginForm.Fld_FileName.Replace(".txt", "_2w.txt"); nora = importer.LoadData(true, scd); ZXC.aim_emsg(MessageBoxIcon.Information, "[{2}] Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, importer.FullPathFileName, scd);

         }
         else // classic
         {
            nora = importer.LoadData(true);
         }

      }

      else if(TheVvDataRecord.VirtualRecordName == Nalog.recordName && isUcitajRacune == true) // Nalog moze izi dvojako. 1. kao inicijalni import ili 2. UcitajRacune iz Materijalnog
      {
         VvFtransImporter importer = new VvFtransImporter(TheDbConnection, loginForm.Fld_FileName, "\t");

         FtransDao.FtransCI fldRbr = new FtransDao.FtransCI();

         fldRbr.t_konto     =  1;
         fldRbr.t_kupdob_cd =  2;
         fldRbr.t_mtros_cd  =  3;
         fldRbr.t_tipBr     =  4;
         fldRbr.t_opis      =  5;
         fldRbr.t_valuta    =  6;
         fldRbr.t_tt        =  7;
         fldRbr.t_dokDate   =  8;
         fldRbr.t_pdv       =  9;
         fldRbr.t_037       = 10;
         fldRbr.t_dug       = 11;
         fldRbr.t_pot       = 12;
         
         importer.FldRbr = fldRbr;

         importer.FldRbrNalog = 13;

         transNora = importer.LoadData(false);

      }
      else if(TheVvDataRecord.VirtualRecordName == Nalog.recordName && isUcitajRacune == false && ZXC.vvDB_VvDomena == "vvFG") // FRAG 2019 
      {
         VvFtransImporter importer = new VvFtransImporter(TheDbConnection, loginForm.Fld_FileName, "\t");

         FtransDao.FtransCI fldRbr = new FtransDao.FtransCI();

         fldRbr.t_konto     =  3;
         //fldRbr.t_kupdob_cd =  2;
         //fldRbr.t_mtros_cd  =  3;
         //fldRbr.t_tipBr     =  4;
         fldRbr.t_opis      =  4;
         //fldRbr.t_valuta    =  6;
         //fldRbr.t_tt        =  7;
         fldRbr.t_dokDate   =  1;
         //fldRbr.t_pdv       =  9;
         //fldRbr.t_037       = 10;
         fldRbr.t_dug       = 5;
         fldRbr.t_pot       = 6;
         
         importer.FldRbr = fldRbr;

         importer.FldRbrNalog = 13;

         transNora = importer.LoadData(false);

      }

      else
      {
         TheVvDao.IMPORT_OFFIX(TheDbConnection, TheVvDataRecord, loginForm.Fld_FileName, loginForm.Fld_OnDuplicateDoReplace, out nora);

         if(TheVvDataRecord.IsDocument)
         {
            vvTransRecord.VvDao.IMPORT_OFFIX(TheDbConnection, vvTransRecord, loginForm.Fld_FileNameTrans, /*loginForm.Fld_OnDuplicateDoReplace*/ false, out transNora);
         }

         if(TheVvDataRecord.VirtualRecordName == Placa.recordName)
         {
            vvTransRecord = new Ptrane();
            string fileNameTrans2 = SetTransImportFileName(vvTransRecord.VirtualLegacyRecordPreffix, loginForm.Fld_FileName, TheVvDataRecord.VirtualLegacyRecordPreffix);
            vvTransRecord.VvDao.IMPORT_OFFIX(TheDbConnection, vvTransRecord, fileNameTrans2, /*loginForm.Fld_OnDuplicateDoReplace*/ false, out transNora);

            vvTransRecord = new Ptrano();
            string fileNameTrans3 = SetTransImportFileName(vvTransRecord.VirtualLegacyRecordPreffix, loginForm.Fld_FileName, TheVvDataRecord.VirtualLegacyRecordPreffix);
            vvTransRecord.VvDao.IMPORT_OFFIX(TheDbConnection, vvTransRecord, fileNameTrans3, /*loginForm.Fld_OnDuplicateDoReplace*/ false, out transNora);
         }
      }

      // ___ Here we END! MySQL LOAD DATA INFIle _____________________________________________________________________________


      loginForm.Dispose();

      Environment.CurrentDirectory = currentLocalDirerctory;

      #endregion LOAD DATA INFILE

      #region Eventual Additional Operations

      switch(TheVvDataRecord.VirtualRecordName)
      {
         case Kplan .recordName: KplanDao .ImportFromOffix_Translate437           (TheDbConnection); break;
         case Nalog .recordName: if(!isUcitajRacune)
                                 {
                                 FtransDao.ImportFromOffix_Translate437_SetTickers(TheDbConnection); 
                                 }
                                 break;
         case Kupdob.recordName: KupdobDao.ImportFromOffix_Translate437           (TheDbConnection); break;
         case Prjkt .recordName: PrjktDao .ImportFromOffix_Translate437           (TheDbConnection); break;
         case Osred .recordName: OsredDao .ImportFromOffix_Translate437_SetTickers(TheDbConnection); break;
         case Person.recordName: PersonDao.ImportFromOffix_Translate437_SetTickers(TheDbConnection); break;
         case Artikl.recordName: ArtiklDao.ImportFromOffix_Translate437           (TheDbConnection);
                         if(ZXC.IsSvDUH) ZXC.aim_emsg("NE zaboravi: SvDuh_Extract_ORG_FromNaziv()"); break;

         case Placa .recordName: PtransDao.ImportFromOffix_Translate437           (TheDbConnection);
                                 PtraneDao.ImportFromOffix_Translate437           (TheDbConnection);
                                 PtranoDao.ImportFromOffix_Translate437_SetTickers(TheDbConnection); break;
      }

      #endregion Eventual Additional Operations

      #region Show News

      bool IsNotEmptyTable = false;

      VvSQL.DBNavigActionType frs_lst = (TheVvDataRecord.IsDocumentLike ? VvSQL.DBNavigActionType.LST : VvSQL.DBNavigActionType.FRS);
      IsNotEmptyTable = TheVvDataRecord.VvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, frs_lst, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);
      if(IsNotEmptyTable)
      {
         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
         TheVvTabPage.FileIsEmpty = false;
      }
      else
      {
         TheVvTabPage.FileIsEmpty = true;
      }

      SetVvMenuEnabledOrDisabled_RegardingWriteMode(ZXC.WriteMode.None);

      #endregion Show News

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, loginForm.Fld_FileName);

      ZXC.OffixImport_InProgress = false;

      return;
   }

   private void FtpGetFile_PUSE(VvDataRecord vvDataRecord, string fileName, FTPConnection ftp, bool isUcitajRacune)
   {
      ftp.GetFile(fileName, FTPFileTransferType.ASCII);

      string transFileName;

      // u ovaj switch treba dodati sve 'Document' slucajeve 
      switch(vvDataRecord.VirtualRecordName)
      {
         case Nalog.recordName:

            if(isUcitajRacune == false)
            {
               transFileName = "vv_" + "ft" + fileName.Substring(5);

               ftp.GetFile(transFileName, FTPFileTransferType.ASCII);
            }
            break;

         case Placa.recordName:

            transFileName = "vv_" + "pt" + fileName.Substring(5);

            ftp.GetFile(transFileName, FTPFileTransferType.ASCII);

            transFileName = "vv_" + "pe" + fileName.Substring(5);

            ftp.GetFile(transFileName, FTPFileTransferType.ASCII);

            transFileName = "vv_" + "po" + fileName.Substring(5);

            ftp.GetFile(transFileName, FTPFileTransferType.ASCII);

            break;
      }

   }

   private void ExecuteDownload(FTP_SourceForge ftp, string fileName)
   {
      ftp.OpenDownload(fileName);
      int perc;
      while(ftp.DoDownload() > 0)
      {
         perc = (int)(((ftp.BytesTotal) * 100) / ftp.FileSize);
         Console.Write("\rDownloading: {0}/{1} {2}%", ftp.BytesTotal, ftp.FileSize, perc);
         Console.Out.Flush();
      }
      Console.WriteLine("");
   }

   private void FtpGetFile2(VvDataRecord vvDataRecord, string fileName, FTP_SourceForge ftp, bool isUcitajRacune)
   {
      ExecuteDownload(ftp, fileName);

      string transFileName;

      // u ovaj switch treba dodati sve 'Document' slucajeve 
      switch(vvDataRecord.VirtualRecordName)
      {
         case Nalog.recordName:

            if(isUcitajRacune == false)
            {
               transFileName = "vv_" + "ft" + fileName.Substring(5);

               ExecuteDownload(ftp, transFileName);
            }
            break;

         case Placa.recordName:

            transFileName = "vv_" + "pt" + fileName.Substring(5);

            ExecuteDownload(ftp, transFileName);

            transFileName = "vv_" + "pe" + fileName.Substring(5);

            ExecuteDownload(ftp, transFileName);

            transFileName = "vv_" + "po" + fileName.Substring(5);

            ExecuteDownload(ftp, transFileName);

            break;
      }

   }

   private string SuggestImportFileName(string legacyRecordPreffix, uint CURR_prjkt_KCD, bool isUcitajRacune)
   {
      if(isUcitajRacune) return "vv_" + "ftZXC_"            + CURR_prjkt_KCD.ToString("000000") + ".txt";
      else               return "vv_" + legacyRecordPreffix + CURR_prjkt_KCD.ToString("000000") + ".txt";
   }

   private string SetTransImportFileName(string legacyRecordPreffix, string documentFileName, string documentRecordPreffix)
   {
      return documentFileName.Replace(documentRecordPreffix, legacyRecordPreffix);
   }

   #endregion ImportFromOffix

   #region Mixer

   #region VIRMANI

   private void MIX_PayProjektDavanja(object sender, EventArgs e)
   {
      #region Local fieldz and init...

      ProjektPayDlg dlg = new ProjektPayDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      ZXC.ProjektPaySetEnum paySet = dlg.TheUC.Fld_ProjektPaySet;

      string razdoblje = dlg.TheUC.Fld_Razdoblje;

      dlg.Dispose();

      VirmanDUC theDUC = TheVvUC as VirmanDUC;
      int rowIdx, idxCorrector;

      VirmanStruct virman;

      #endregion Local fieldz and init...

      #region Load adequate PrjktList and loop through it

      List<Prjkt> prjktList = new List<Prjkt>();

      string origDbName = ZXC.TheMainDbConnection.Database;

      VvDaoBase.LoadGenericVvDataRecordList(/*conn*/ZXC.PrjConnection, prjktList, SetFilterMembers_MIX_PayProjektDavanja(dlg.TheUC.Fld_ProjektPaySet), "", "Naziv");

      ZXC.SetMainDbConnDatabaseName(origDbName); // jer LoadGenericVvDataRecordList promijeni na 'vvektor' ... 

      if(prjktList.Count.IsZero()) return;

      idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      // Znaci da pobrise onaj prvi automatski redak nastao pri NewRecordClick
      if(theDUC.TheG.RowCount == 2)  theDUC.TheG.Rows.Clear();

      foreach(Prjkt prjkt_rec in prjktList)
      {
         if(prjkt_rec.IsObrt == false && paySet == ZXC.ProjektPaySetEnum.PidKmDopr) continue;

         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.PutPlatData(rowIdx, prjkt_rec);
         theDUC.PutVirmanDates(rowIdx, theDUC.Fld_DokDate);

         virman = SetVirmanData(paySet, prjkt_rec, razdoblje);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbNameB_50  , rowIdx, virman.Prim1);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbUlBrB_32  , rowIdx, virman.Prim2);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbMjestoB_32, rowIdx, virman.Prim3);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbZiroB_32  , rowIdx, virman.Ziro2);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_moneyA        , rowIdx, virman.Money);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_opis_128      , rowIdx, virman.OpisPl);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64  , rowIdx, virman.Pnbo);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_strB_2        , rowIdx, virman.PnboMod);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_strC_2        , rowIdx, virman.SifraPl);
      }

      SetDirtyFlag("MIX_PayProjektDavanja");

      #endregion Load adequate PrjktList
   }

   private VirmanStruct SetVirmanData(ZXC.ProjektPaySetEnum paySet, Prjkt prjkt_rec, string razdoblje)
   {
      VirmanStruct virman = new VirmanStruct();

      string ziroHNBpreffix = "1001005-";
      string ziroHNBsuffix;

      string ziroKind, porezKind, pnboKind, str4ISOcheck;
      string minusRazdoblje = (razdoblje.IsEmpty() == true) ? "" : "-" + razdoblje;

      virman.SifraPl = "08";

      switch(paySet)
      {
         case ZXC.ProjektPaySetEnum.PidDobit:

            ziroKind  = ((prjkt_rec.IsObrt == false || prjkt_rec.IsDobit == true) ? "1606"  : "1200"   );
            porezKind = ((prjkt_rec.IsObrt == false || prjkt_rec.IsDobit == true) ? "DOBIT" : "DOHODAK");
            pnboKind  = ((prjkt_rec.IsObrt == false || prjkt_rec.IsDobit == true) ? "1651"  : "1430"   );

            str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);
           
            virman.Prim1   = "";//DRŽAVNI PRORAČUN"; 02.03.2012. ovo su gradski a ne drzavni
            virman.Prim2   = "Porez na " + porezKind;
            virman.Prim3   = "ZAGREB";
            virman.Ziro2   = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
            virman.OpisPl  = "Akontacija poreza na " + porezKind;
            virman.PnboMod = "68";
            virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidDobit;
            break;
        
         case ZXC.ProjektPaySetEnum.PidMO1:

            ziroHNBsuffix = "1863000160";
            pnboKind = "8214";

            virman.Prim1   = "VLASNIK OBRTA";
            virman.Prim2   = "DOPRINOS ZA MIROV. OSIGURANJE";
            virman.Prim3   = "I STUP";
            virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix;
            virman.OpisPl  = "OIB " + prjkt_rec.Oib + " Obveza za " + razdoblje + " Iznos: " + prjkt_rec.PidMO1 + " Rok uplate: " + virman.DateValuta.ToShortDateString();
            virman.PnboMod = "68";
            virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidMO1;
            break;
        
         case ZXC.ProjektPaySetEnum.PidMO2:

            ziroHNBsuffix = "1700036001";
            pnboKind = "2046";

            virman.Prim1   = "VLASNIK OBRTA";
            virman.Prim2   = "DOPRINOS ZA MIROV. OSIGURANJE";
            virman.Prim3   = "II STUP";
            virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix;
            virman.OpisPl  = "OIB " + prjkt_rec.Oib + " Obveza za " + razdoblje + " Iznos: " + prjkt_rec.PidMO2 + " Rok uplate: " + virman.DateValuta.ToShortDateString();
            virman.PnboMod = "68";
            virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidMO2;
            break;
        
         case ZXC.ProjektPaySetEnum.PidZdr:

            ziroHNBsuffix = "1863000160";
            pnboKind = "8478";

            virman.Prim1   = "VLASNIK OBRTA";
            virman.Prim2   = "DOPRINOS ZA OSNOVNO";
            virman.Prim3   = "ZDRAVSTVENO OSIGURANJE";
          //virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix; do 01.01.2015.
            virman.Ziro2   = "HR6510010051550100001";
            virman.OpisPl  = "OIB " + prjkt_rec.Oib + " Obveza za " + razdoblje + " Iznos: " + prjkt_rec.PidZdr + " Rok uplate: " + virman.DateValuta.ToShortDateString();
            virman.PnboMod = "68";
            virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidZdr;

            break;
        
         case ZXC.ProjektPaySetEnum.PidZor:

            ziroHNBsuffix = "1863000160";
            pnboKind = "8591";

            virman.Prim1   = "VLASNIK OBRTA";
            virman.Prim2   = "DOPRINOS ZA ZDRAV. OSIGURANJE";
            virman.Prim3   = "U SLUČAJU OZLJEDE NA RADU";
          //virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix; do 01.01.2015.
            virman.Ziro2   = "HR6510010051550100001";
            virman.OpisPl  = "OIB " + prjkt_rec.Oib + " Obveza za " + razdoblje + " Iznos: " + prjkt_rec.PidZor + " Rok uplate: " + virman.DateValuta.ToShortDateString();
            virman.PnboMod = "68";
            virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidZor;
            break;

        case ZXC.ProjektPaySetEnum.PidSume:

            ziroHNBsuffix = "1700055099";

            virman.Prim1   = "DRŽAVNI PRORAČUN";
            virman.Prim2   = "REPUBLIKE HRVATSKE";
            virman.Prim3   = "HRVATSKE ŠUME";
          //virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix; od 2015 novi uplatni racun
            virman.Ziro2   = "HR1210010051863000160";
            virman.OpisPl  = "Naknada za šume";
          //virman.PnboMod = "67"; od 2015 novi uplatni racun
            virman.PnboMod = "68";
          //virman.Pnbo    = prjkt_rec.Oib + "-" + ZXC.projectYear;
            virman.Pnbo    = "5126-" + prjkt_rec.Oib;
            virman.Money   = prjkt_rec.PidSume;
            break;

       case ZXC.ProjektPaySetEnum.PidTurst:

            ziroKind = "2715";

            str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);
           
            virman.Prim1   = "";
            virman.Prim2   = "TURISTIČKA";
            virman.Prim3   = "ZAJEDNICA";
            virman.Ziro2   = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
            virman.OpisPl  = "Akontacija doprinosa Turistčkoj zajednici za " + razdoblje;
            virman.PnboMod = "67";
            virman.Pnbo    = prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidTurst;
            break;

       case ZXC.ProjektPaySetEnum.PidSRent:

            ziroKind = "2870";

            str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);

            virman.Prim1   = "";
            virman.Prim2   = "SPOMENIČKA RENTA";
            virman.Prim3   = "";
            virman.Ziro2   = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
            virman.OpisPl  = "Spomenička renta za " + razdoblje;
            virman.PnboMod = "67";
            virman.Pnbo    = prjkt_rec.Oib + minusRazdoblje;
            virman.Money   = prjkt_rec.PidSRent;
            break;

       case ZXC.ProjektPaySetEnum.PidKmClan:

            if(prjkt_rec.IsObrt)
            {
               ziroKind = "5236";

               str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);

               virman.Prim1   = "KOMORSKI DOPRINOS";
               virman.Prim2   = "U PAUŠALNOM IZNOSU";
               virman.Prim3   = "";
               virman.Ziro2   = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
               virman.OpisPl  = "Komorski doprinos u paušalnom iznosu za razdoblje " + razdoblje;
               virman.PnboMod = "67";
               virman.Pnbo    = prjkt_rec.Oib + minusRazdoblje;
               virman.Money   = prjkt_rec.PidKmClan;

            }
            else
            {
               ziroHNBsuffix = "1700052620";

               virman.Prim1   = "";
               virman.Prim2   = "ČLANARINA ZA HGK";
               virman.Prim3   = "";
               virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix;
               virman.OpisPl  = "Članarina za HGK za razdoblje " + razdoblje;
               virman.PnboMod = "67";
               virman.Pnbo    = prjkt_rec.Oib + "-002" + minusRazdoblje;
             //if(prjkt_rec.PidKmClan.IsZero()) virman.Money = 55.00M;        02.03.2012.
             //else                            virman.Money = prjkt_rec.PidKmClan;
               virman.Money = prjkt_rec.PidKmClan;
            }
            break;
     
         case ZXC.ProjektPaySetEnum.PidKmDopr:

            //if(prjkt_rec.IsObrt)
            //{
            //   ziroKind = "5241";

            //   str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);

            //   virman.Prim1   = "KOMORSKI DOPRINOS";
            //   virman.Prim2   = "OD DOHOTKA, DOBITI ILI PLAĆE";
            //   virman.Prim3   = "";
            //   virman.Ziro2   = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
            //   virman.OpisPl  = "Komorski doprinos od dohotka za razdoblje " + razdoblje;
            //   virman.PnboMod = "67";
            //   virman.Pnbo    = prjkt_rec.Oib + minusRazdoblje;
            //   virman.Money   = prjkt_rec.PidKmDopr;

            //} // 12.03.2014.
            if(prjkt_rec.IsObrt)
            {
               //ziroKind = "8770";

               //str4ISOcheck = ZXC.GetStr4ISOcheck(prjkt_rec.OpcCd, ziroKind);

               //virman.Prim1 = "KOMORSKI DOPRINOS";
               //virman.Prim2 = "OD DOHOTKA, DOBITI ILI PLAĆE";
               //virman.Prim3 = "";
               //virman.Ziro2 = ziroHNBpreffix + str4ISOcheck + ZXC.GetISO7064(str4ISOcheck);
               //virman.OpisPl = "Komorski doprinos od dohotka za razdoblje " + razdoblje;
               //virman.PnboMod = "67";
               //virman.Pnbo = prjkt_rec.Oib + minusRazdoblje;
               //virman.Money = prjkt_rec.PidKmDopr;

               ziroHNBsuffix = "1863000160";
               pnboKind = "8770";

               virman.Prim1   = "VLASNIK OBRTA";
               virman.Prim2   = "DOPRINOS ZA ";
               virman.Prim3   = "ZAPOŠLJAVANJE";
               virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix;
               virman.OpisPl  = "OIB " + prjkt_rec.Oib + " Obveza za " + razdoblje + " Iznos: " + prjkt_rec.PidKmDopr + " Rok uplate: " + virman.DateValuta.ToShortDateString();
               virman.PnboMod = "68";
               virman.Pnbo    = pnboKind + "-" + prjkt_rec.Oib + minusRazdoblje;
               virman.Money   = prjkt_rec.PidKmDopr;

            }
            // od TODO!!!12.03.2014. kako da ne idu firme !!!!!
            else
            {
            //   ziroHNBsuffix = "1700052783";

            //   virman.Prim1   = "";
            //   virman.Prim2   = "DOPRINOS ZA HGK";
            //   virman.Prim3   = "";
            //   virman.Ziro2   = ziroHNBpreffix + ziroHNBsuffix;
            //   virman.OpisPl  = "Doprinos za HGK za razdoblje " + razdoblje;
            //   virman.PnboMod = "67";
            //   virman.Pnbo    = prjkt_rec.Oib+ "-001" + minusRazdoblje;
            //   virman.Money   = prjkt_rec.PidKmDopr;

            }

            break;

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "PaySet: [" + paySet + "] still undone in 'SetVirmanData'."); break;
      }

      return virman;
   }

   private List<VvSqlFilterMember> SetFilterMembers_MIX_PayProjektDavanja(ZXC.ProjektPaySetEnum paySet)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      DataRowCollection prjSch = ZXC.PrjktDao.TheSchemaTable.Rows;  
      PrjktDao.PrjktCI  prjCI  = ZXC.PrjktDao.CI;

      filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.isSkip], "isSkip", 0, " = "));

      switch(paySet)
      {
         case ZXC.ProjektPaySetEnum.PidDobit   : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidDobit], "money", decimal.Zero, " > ")); break;
            // 19.01.2012: clanarine idu sve (ili doslovna ili 55 ako je prazno) onda pak 02.03.2012. clanarine HGK idu samo ako su neprazne
         case ZXC.ProjektPaySetEnum.PidKmClan  : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidKmClan], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidKmDopr  : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidKmDopr], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidMO1     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidMO1   ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidMO2     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidMO2   ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidSRent   : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidSRent ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidSume    : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidSume  ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidTurst   : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidTurst ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidZdr     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidZdr   ], "money", decimal.Zero, " > ")); break;
         case ZXC.ProjektPaySetEnum.PidZor     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.pidZor   ], "money", decimal.Zero, " > ")); break;
         //case ZXC.ProjektPaySetEnum.ToPayA     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.toPayA  ], "money", decimal.Zero, " > ")); break;
         //case ZXC.ProjektPaySetEnum.ToPayB     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.toPayB  ], "money", decimal.Zero, " > ")); break;
         //case ZXC.ProjektPaySetEnum.ToPayC     : filterMembers.Add(new VvSqlFilterMember(prjSch[prjCI.toPayC  ], "money", decimal.Zero, " > ")); break;

         default: ZXC.aim_emsg(MessageBoxIcon.Error, "PaySet: [" + paySet + "] still undone in 'SetFilterMembers'."); break;
      }

      return filterMembers;
   }
   
   private void MIX_PayFakturOLDandORIG(object sender, EventArgs e)
   {
      #region Local fieldz and init...

      VirmanDUC theDUC = TheVvUC as VirmanDUC;

      int currRowIdx = theDUC.TheG.CurrentRow.Index;

      #endregion local fieldz and init...

      #region FindFaktur

      Faktur         faktur_rec = new Faktur();

      VvSubModul theVvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.R_UFA);

      VvFindDialog dlg = FakturDUC.CreateFind_Faktur_Dialog(theVvSubModul);

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         if(!ZXC.FakturDao.SetMe_Record_byRecID(TheDbConnection, faktur_rec, (uint)dlg.SelectedRecID, false)) return;
         if(faktur_rec.IsExtendable == true) faktur_rec.VvDao.LoadExtender(TheDbConnection, faktur_rec, false);
      }
      else
      {
         faktur_rec = null;
      }

      dlg.Dispose();

      #endregion FindFaktur

      #region PutLineFields

      //artikl_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == mixer_rec.KupdobCD);

      if(faktur_rec == null) return;

      if(theDUC.TheG.RowCount == currRowIdx + 1) theDUC.TheG.Rows.Add();

      if(faktur_rec.TT.StartsWith("U")) // kak je bilo - samo za ulazne racune
      {
         theDUC.PutPrimData(currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec));
         theDUC.PutPlatData(currRowIdx, ZXC.CURR_prjkt_rec);
         theDUC.PutVirmanDates(currRowIdx, theDUC.Fld_DokDate);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_moneyA, currRowIdx, faktur_rec.S_ukKCRP);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_opis_128, currRowIdx, "Račun br. " + faktur_rec.VezniDok + " (" + faktur_rec.TipBr + ")");

         if(faktur_rec.PnbV.NotEmpty())
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_strB_2, currRowIdx, faktur_rec.PnbM);
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.PnbV);
         }
         else
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.VezniDok);
         }
      }
      else // dodala 15.11.2011. za izlazne racune - knj.servisi Izlazni racuni
      {
         theDUC.PutPrimData     (currRowIdx, ZXC.CURR_prjkt_rec);
         theDUC.PutPlatData     (currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec));
         theDUC.PutVirmanDates  (currRowIdx, theDUC.Fld_DokDate);

         if(faktur_rec.ZiroRn.NotEmpty()) theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbZiroB_32, currRowIdx, ZXC.GetIBANfromOldZiro(faktur_rec.ZiroRn));
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbZiroB_32, currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec).Ziro1);

         theDUC.TheG.PutCell(theDUC.DgvCI.iT_moneyA, currRowIdx, faktur_rec.S_ukKCRP);
         theDUC.TheG.PutCell(theDUC.DgvCI.iT_opis_128, currRowIdx, "Račun br. " + faktur_rec.VezniDok + " (" + faktur_rec.TipBr + ")");

         if(faktur_rec.PnbV.NotEmpty())
         {
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_strB_2, currRowIdx, faktur_rec.PnbM);
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.PnbV);
         }
         else
         {
          //  theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.VezniDok);
         }

      }
      SetDirtyFlag("MIX_PayFaktur");

      #endregion PutLineFields

   }

   private void MIX_PayFaktur(object sender, EventArgs e)
   {
      #region Local fieldz and init...

      VirmanDUC theDUC = TheVvUC as VirmanDUC;

      int currRowIdx = theDUC.TheG.CurrentRow.Index;

      #endregion local fieldz and init...

      VvSubModul theVvSubModul = GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.R_UFA);

      VvFindDialog dlg = FakturDUC.CreateFind_Faktur_Dialog(theVvSubModul);

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      Faktur faktur_rec;
      uint   fakturRecID;

      foreach(DataGridViewRow row in dlg.TheRecListUC.TheGrid.SelectedRows)
      {
         fakturRecID = ZXC.ValOrZero_UInt(row.Cells["recID"].Value.ToString());
         faktur_rec = new Faktur(fakturRecID);

         if(!ZXC.FakturDao.SetMe_Record_byRecID(TheDbConnection, faktur_rec, fakturRecID, false)) continue;
         if(faktur_rec.IsExtendable == true) faktur_rec.VvDao.LoadExtender(TheDbConnection, faktur_rec, false);

         #region PutLineFields

         //artikl_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpd => kpd.KupdobCD == mixer_rec.KupdobCD);

         if(faktur_rec == null) return;

         if(theDUC.TheG.RowCount == currRowIdx + 1) theDUC.TheG.Rows.Add();

         if(faktur_rec.TT.StartsWith("U")) // kak je bilo - samo za ulazne racune
         {
            theDUC.PutPrimData(currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec));
            theDUC.PutPlatData(currRowIdx, ZXC.CURR_prjkt_rec);
            theDUC.PutVirmanDates(currRowIdx, theDUC.Fld_DokDate);

            theDUC.TheG.PutCell(theDUC.DgvCI.iT_moneyA, currRowIdx, faktur_rec.S_ukKCRP);
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_opis_128, currRowIdx, "Račun br. " + faktur_rec.VezniDok + " (" + faktur_rec.TipBr + ")");

            if(faktur_rec.PnbV.NotEmpty())
            {
               theDUC.TheG.PutCell(theDUC.DgvCI.iT_strB_2, currRowIdx, faktur_rec.PnbM);
               theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.PnbV);
            }
            else
            {
               theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.VezniDok);
            }
         }
         else // dodala 15.11.2011. za izlazne racune - knj.servisi Izlazni racuni
         {
            theDUC.PutPrimData     (currRowIdx, ZXC.CURR_prjkt_rec);
            theDUC.PutPlatData     (currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec));
            theDUC.PutVirmanDates  (currRowIdx, theDUC.Fld_DokDate);

            if(faktur_rec.ZiroRn.NotEmpty()) theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbZiroB_32, currRowIdx, ZXC.GetIBANfromOldZiro(faktur_rec.ZiroRn));
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_kpdbZiroB_32, currRowIdx, Kupdob.SetKupdobForVirmanFromFaktur(faktur_rec).Ziro1);

            theDUC.TheG.PutCell(theDUC.DgvCI.iT_moneyA, currRowIdx, faktur_rec.S_ukKCRP);
            theDUC.TheG.PutCell(theDUC.DgvCI.iT_opis_128, currRowIdx, "Račun br. " + faktur_rec.VezniDok + " (" + faktur_rec.TipBr + ")");

            if(faktur_rec.PnbV.NotEmpty())
            {
               theDUC.TheG.PutCell(theDUC.DgvCI.iT_strB_2, currRowIdx, faktur_rec.PnbM);
               theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.PnbV);
            }
            else
            {
             //  theDUC.TheG.PutCell(theDUC.DgvCI.iT_vezniDokB_64, currRowIdx, faktur_rec.VezniDok);
            }

         }

         currRowIdx++;

         #endregion PutLineFields

      } // foreach(DataGridViewRow row in dlg.TheRecListUC.TheGrid.SelectedRows) 

      SetDirtyFlag("MIX_PayFaktur");
   }

   #endregion VIRMANI

   #region RASTER

   private void MIX_CreateRasterFaktur(object sender, EventArgs e)
   {
      int debugCount/*, newFakCount*/;

    //Mixer mixer_rec = ((Mixer)TheVvDataRecord);
      Mixer mixer_rec = (Mixer)(TheVvDataRecord).CreateNewRecordAndCloneItComplete();

      string message; 
      int rowIdx;
      if(sender is int) // Create ONLY 1 single faktur. object is rowIdx
      {
         message = "Da li zaista zelite kreirati 1 fakturu?!";
         rowIdx = (int)sender;
         Xtrans xtrans_rec = (Xtrans)mixer_rec.Transes[rowIdx - 1].CreateNewRecordAndCloneItComplete();
         mixer_rec.InvokeTransClear();
         mixer_rec.Transes = new List<Xtrans>(1);
         mixer_rec.Transes.Add(xtrans_rec);
      }
      else
      {
         message = "Da li zaista zelite kreirati SVE fakture?!";
         rowIdx = -1;
      }

      //newFakCount = mixer_rec.Transes.Where(xtr => xtr.T_isXxx == false).Select(xtr => xtr.T_kupdobCD).Distinct().Count(); // dole se grupiraju linije po T_kupdobCD-u 

      //DialogResult result = MessageBox.Show("Da li zaista zelite kreirati fakture?! (" + newFakCount + ") komada",
      //   "Potvrdite FAKTURIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      DialogResult result = MessageBox.Show(message, "Potvrdite FAKTURIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

      if(result != DialogResult.Yes) return;

      Cursor.Current = Cursors.WaitCursor;

      // here we go! 

      bool shouldGroupRepeatingKupdobs = (TheVvUC is RasterDUC); // RasterDUC - grupiraj, RasterBDUC - NE grupiraj 

      debugCount = CreateFakturs(mixer_rec, shouldGroupRepeatingKupdobs);

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg("Gotovo. Kreirao {0} Fakturs.", debugCount);
   }

   private int CreateFakturs(Mixer mixer_rec, bool shouldGroup)
   {
      #region Initializations and RtransList creation

      if(mixer_rec.Transes.Count.IsZero()) return 0;

      TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);

      int    debugCount = 0;
      ushort line       = 0;
      uint   fakRbr     = 1;

      uint   currKupdobCD;
      uint   currRecID;

      Faktur faktur_rec;
      Xtrans xtrans;

      currKupdobCD = mixer_rec.Transes[0].T_kupdobCD;
      currRecID    = mixer_rec.Transes[0].T_recID;

      List<Rtrans> rtransList = new List<Rtrans>(mixer_rec.Transes.Count);

      foreach(Xtrans xtrans_rec in mixer_rec.Transes)
      {
         if(xtrans_rec.T_isXxx) continue; // should skip 

         if((shouldGroup == false && currRecID != xtrans_rec.T_recID) || currKupdobCD != xtrans_rec.T_kupdobCD)
         {
            fakRbr++;
            currKupdobCD = xtrans_rec.T_kupdobCD;
         }

         rtransList.Add(CreateRtransFromXtrans(mixer_rec, xtrans_rec, fakRbr, null)); // tu se obavi CalcTrans() 

         #region Raster P DUC: eventual Linked Artikl1 and eventual Linked Artikl2

         if(TheVvUC is RasterBDUC) // eventual Linked Artikl1 and eventual Linked Artikl2 
         {
            Artikl artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == xtrans_rec.T_artiklCD);

            if(artikl_rec != null && artikl_rec.LinkArtCD.NotEmpty())
            {
               Artikl linkedArtikl_rec1 = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == artikl_rec.LinkArtCD);

               if(linkedArtikl_rec1 != null)
               {
                  rtransList.Add(CreateRtransFromXtrans(mixer_rec, xtrans_rec, fakRbr, linkedArtikl_rec1)); // tu se obavi CalcTrans() 

                  if(linkedArtikl_rec1.LinkArtCD.NotEmpty())
                  {
                     Artikl linkedArtikl_rec2 = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == linkedArtikl_rec1.LinkArtCD);

                     if(linkedArtikl_rec2 != null)
                     {
                        rtransList.Add(CreateRtransFromXtrans(mixer_rec, xtrans_rec, fakRbr, linkedArtikl_rec2)); // tu se obavi CalcTrans() 
                     }
                  }
               }
            }
         }

         #endregion RasterPDUC: eventual Linked Artikl1 and eventual Linked Artikl2
      }

      ZXC.luiListaSkladista.LazyLoad(); // za skladCD '1' 

      #endregion Initializations and RtransList creation

      currKupdobCD = mixer_rec.Transes[0].T_kupdobCD;
      currRecID    = rtransList[0].T_recID;

      fakRbr = 1;

      if(mixer_rec.Transes[0].T_isXxx == false) // if true should skip 
      {
         faktur_rec = CreateFakturFromMixXtrans(mixer_rec, mixer_rec.Transes[0], rtransList, fakRbr);
      }
      else
      {
         faktur_rec = null;
      }

    //foreach(Xtrans rptXtrans_rec in mixer_rec.Transes)
      foreach(Rtrans rtrans in rtransList) // u rtransList-u moze biti vise stavaka nego u mixer_rec.Transes-u zbog eventualnih linkedArtikal-a 
      {
         xtrans = mixer_rec.Transes.Single(xtr => xtr.T_recID == rtrans.T_recID);

         if(xtrans.T_isXxx) continue; // should skip 

         if((shouldGroup == false && currRecID != xtrans.T_recID) || currKupdobCD != xtrans.T_kupdobCD)
         {
            line = 0;

            currKupdobCD = xtrans.T_kupdobCD;
            currRecID    = xtrans.T_recID;

            faktur_rec = CreateFakturFromMixXtrans(mixer_rec, xtrans, rtransList, ++fakRbr);
         }

         //rtrans_rec = rtransList.Single(rtrans => rtrans.T_serial == rptXtrans_rec.T_serial);

         if(line.IsZero() && faktur_rec != null) debugCount++;

         // 18.12.2026: dodan if() 
         if(faktur_rec != null) FakturDao.AutoSetFaktur(TheDbConnection, ref line, faktur_rec, rtrans);
         else { ZXC.aim_emsg(MessageBoxIcon.Warning, $"Racun nece biti kreeiran jer je B2B / B2C nedefinirano! Kupac: [{xtrans.T_kpdbNameA_50}]"); continue; }

         #region Rwtrec Feedback

         BeginEdit(xtrans);

         xtrans.T_kpdbMjestoA_32 = faktur_rec.TT_And_TtNum;
         xtrans.T_moneyD         = faktur_rec.S_ukKCRP;

         xtrans.VvDao.RWTREC(TheDbConnection, xtrans, false, true, false);

         EndEdit(xtrans);

         #endregion Rwtrec Feedback

      }

      return debugCount;
   }

   private Faktur CreateFakturFromMixXtrans(Mixer mixer_rec, Xtrans xtrans_rec, List<Rtrans> allFakturs_rtransList, uint fakRbr)
   {
      Faktur faktur_rec = new Faktur();

      var thisFaktur_rtransList = allFakturs_rtransList.Where(rtr => rtr.FakRbr == fakRbr);

      Kupdob kupdobSifrar_rec;
      if(xtrans_rec.T_kupdobCD.NotZero())
      {
         kupdobSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == xtrans_rec.T_kupdobCD); if(kupdobSifrar_rec == null) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne postoji PARTNER sa sifrom [" + xtrans_rec.T_kupdobCD + "]!");
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0} nema zadanog partnera!", xtrans_rec.T_serial);
         kupdobSifrar_rec = new Kupdob();
      }

      DateTime fakturDateAndTime = new DateTime(mixer_rec.DokDate.Year, mixer_rec.DokDate.Month, mixer_rec.DokDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

      faktur_rec.TT          = Faktur.TT_IFA            ;
      faktur_rec.DokDate     = fakturDateAndTime        ;
      faktur_rec.PdvDate     = fakturDateAndTime        ;
      faktur_rec.Napomena    = mixer_rec.Napomena       ;
      // 13.10.2017: 
    //faktur_rec.KupdobCD    = kupdobSifrar_rec.KupdobCD;
    //faktur_rec.KupdobName  = kupdobSifrar_rec.Naziv   ;
    //faktur_rec.KupdobTK    = kupdobSifrar_rec.Ticker  ;
    //faktur_rec.KdUlica     = kupdobSifrar_rec.Ulica2  ;
    //faktur_rec.KdZip       = kupdobSifrar_rec.PostaBr ;
    //faktur_rec.KdMjesto    = kupdobSifrar_rec.Grad    ;
      faktur_rec.KupdobCD    = 
      faktur_rec.PosJedCD    = kupdobSifrar_rec.KupdobCD;
      faktur_rec.KupdobName  =
      faktur_rec.PosJedName  = kupdobSifrar_rec.Naziv   ;
      faktur_rec.KupdobTK    =
      faktur_rec.PosJedTK    = kupdobSifrar_rec.Ticker  ;
      faktur_rec.KdUlica     =
      faktur_rec.PosJedUlica = kupdobSifrar_rec.Ulica2  ;
      faktur_rec.KdZip       =
      faktur_rec.PosJedZip   = kupdobSifrar_rec.PostaBr ;
      faktur_rec.KdMjesto    =
      faktur_rec.PosJedMjesto= kupdobSifrar_rec.Grad    ;
      faktur_rec.KdOib       = kupdobSifrar_rec.Oib     ;
      faktur_rec.Konto       = kupdobSifrar_rec.KontoPot;
      faktur_rec.RokPlac     = mixer_rec.IntA           ;
      faktur_rec.DospDate    = mixer_rec.DateA          ;
      //faktur_rec.VezniDok    = "Raster: " + Faktur.Set_TT_And_TtNum(mixer_rec.TT, mixer_rec.TtNum);
      faktur_rec.V1_tt       = mixer_rec.TT;
      faktur_rec.V1_ttNum    = mixer_rec.TtNum;
      faktur_rec.PdvKnjiga   = ZXC.PdvKnjigaEnum.REDOVNA;
      faktur_rec.PdvR12      = ZXC.PdvR12Enum.R1;

      // 11.06.2013: 
      //faktur_rec.ZiroRn      = ZXC.GetIBANfromOldZiro(ZXC.CURR_prjkt_rec.Ziro1);
      // 03.09.2013. ovdje je zeznuto jer na rasterA nema StrA_40 pa nije dolazio zirac na fakturu pa time i na ispis virmana
      if(mixer_rec.TT == Mixer.TT_RASTERF) faktur_rec.ZiroRn = ZXC.GetIBANfromOldZiro(ZXC.CURR_prjkt_rec.Ziro1);
      else                                 faktur_rec.ZiroRn = mixer_rec.StrA_40;

      // 02.01.2013: start 
      VvLookUpItem theLui = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Integer == 1); // probaj naci lui sa integerom '1' (integer nam je kao intera sifra skladista) 

      if(theLui != null) faktur_rec.SkladCD = theLui.Cd;
      else                     { faktur_rec.SkladCD = ""; ZXC.aim_emsg(MessageBoxIcon.Error, "Nedostaje SKLADIŠTE sa brojčanom oznakom 1"); }

      faktur_rec.NacPlac  = mixer_rec.StrD_32.IsEmpty() ? "Virman" : mixer_rec.StrD_32;
      faktur_rec.IsNpCash = mixer_rec.IsXxx;

      // 02.01.2013: end 

      if(mixer_rec.MtrosCD.NotZero())
      {
         Kupdob mtrosSifrar_rec = VvUserControl.KupdobSifrar.SingleOrDefault(vvDR => vvDR.KupdobCD == mixer_rec.MtrosCD); if(mtrosSifrar_rec == null) ZXC.aim_emsg(MessageBoxIcon.Warning, "Ne postoji MJTROŠKA sa sifrom [" + mixer_rec.MtrosCD + "]!");

         faktur_rec.MtrosCD   = mixer_rec.MtrosCD;
         faktur_rec.MtrosTK   = mixer_rec.MtrosTK;
         faktur_rec.MtrosName = mtrosSifrar_rec.Naziv;
      }

      faktur_rec.OpciBlabel  = mixer_rec .StrC_32       ; // naziv objekta 
      faktur_rec.OpciBvalue  = xtrans_rec.T_kpdbUlBrB_32; // broj objekta  

      if(mixer_rec.DevName.NotEmpty() &&
         mixer_rec.DevName != /*"HRK"*/ ZXC.EURorHRKstr)
      {
         faktur_rec.DevName = mixer_rec.DevName;
      }

      faktur_rec.S_ukK        =       thisFaktur_rtransList.Sum(rtr => rtr.T_kol  );
      faktur_rec.S_ukK2       =       thisFaktur_rtransList.Sum(rtr => rtr.T_kol2 );
      faktur_rec.S_ukKC       =       thisFaktur_rtransList.Sum(rtr => rtr.R_KC   );
      faktur_rec.S_ukRbt1     =       thisFaktur_rtransList.Sum(rtr => rtr.R_rbt1 );
      faktur_rec.S_ukKCR      =       thisFaktur_rtransList.Sum(rtr => rtr.R_KCR  );
      faktur_rec.S_ukKCRM     =       thisFaktur_rtransList.Sum(rtr => rtr.R_KCRM );
      faktur_rec.S_ukKCRP     =       thisFaktur_rtransList.Sum(rtr => rtr.R_KCRP );

      if(ZXC.IsStillOldPdv23_ForThisDate(faktur_rec.DokDate))
      {
         faktur_rec.S_ukOsn23m = thisFaktur_rtransList.Sum(rtr => rtr.R_KCR);
         faktur_rec.S_ukPdv23m = thisFaktur_rtransList.Sum(rtr => rtr.R_pdv);
      }
      else
      {
         // 11.06.2013:
         if(thisFaktur_rtransList.Any(rtr => rtr.T_pdvSt.IsZero())) // za sada ovako glupo... 
         {
            faktur_rec.S_ukOsn0 = thisFaktur_rtransList.Sum(rtr => rtr.R_KCR);
          //faktur_rec.S_ukPdv0 = thisFaktur_rtransList.Sum(rtr => rtr.R_pdv);
         }
         else
         {
            faktur_rec.S_ukOsn25m = thisFaktur_rtransList.Sum(rtr => rtr.R_KCR);
            faktur_rec.S_ukPdv25m = thisFaktur_rtransList.Sum(rtr => rtr.R_pdv);
         }
      }
      // 15.04.2020: 
    //faktur_rec.S_ukPdv = thisFaktur_rtransList.Sum(rtr => rtr.R_pdv);
      faktur_rec.S_ukPdv = thisFaktur_rtransList.Sum(rtr => rtr.R_pdv).Ron2();

      faktur_rec.S_ukTrnCount = (uint)thisFaktur_rtransList.Count();

      // 2026: 
      #region F2 validations

      if(ZXC.IsBadOib(ZXC.CURR_user_rec.Oib, false))
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, $"Neispravan OIB operatera:[{ZXC.CURR_user_rec.Oib}] za usera {ZXC.CURR_user_rec.RecID} - {ZXC.CURR_user_rec.UserName}! Račun nije moguće usnimiti kao Fiskalni Račun.");
         return null;
      }

      if(ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B)
      {
         faktur_rec.F2_R1kind = ZXC.F2_R1enum.Nepoznato;

         if(ZXC.CURR_prjkt_rec.F2_ImaSamo_F2_B2B) 
         {
            faktur_rec.F2_R1kind = ZXC.F2_R1enum.B2B;
         }
         else // TETRAGRAM, PANIGALE, FRAG, METAFLEX, PPUK, PLODINE mogu imati i B2C i B2B racune 
         {
            Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD);

            if(kupdob_rec != null)
            {
               if(kupdob_rec.R1kind != ZXC.F2_R1enum.Nepoznato) faktur_rec.F2_R1kind = kupdob_rec.R1kind;
               else
               {
                  faktur_rec.F2_R1kind = KupdobDao.GetMandatory_Kupdob_R1enum_FromDialog(TheDbConnection, kupdob_rec);
               }
            }

            if(faktur_rec.F2_R1kind == ZXC.F2_R1enum.Nepoznato)
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu saznati OBAVEZAN podatak da li je kupac B2B ili B2C!");
               //e.Cancel = true;
               return null;
            }
         }

         faktur_rec.PdvKolTip = (ZXC.VvUBL_PolsProcEnum)Enum.Parse(typeof(ZXC.VvUBL_PolsProcEnum), ZXC.RRD.Dsc_Default_eRposProc);
         faktur_rec.StatusCD  = "380";

         #region Other F2 validations

         if(faktur_rec.KupdobCD != ZXC.RRD.Dsc_MalopKCD && faktur_rec.KdAdresa.IsEmpty())
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Poštanska adresa kupca je prazna! eRačun mora imati bar 'Mjesto'");
            return null;
         }

         #endregion Other F2 validations

      }

      #endregion F2 validations

      return faktur_rec;
   }

   private Rtrans CreateRtransFromXtrans(Mixer mixer_rec, Xtrans xtrans_rec, uint fakRbr, Artikl linkedArtikl_rec)
   {
      Rtrans rtrans_rec = new Rtrans();

      ArtStat artstat_rec = ArtiklDao.GetArtiklStatus(TheDbConnection, xtrans_rec.T_artiklCD, "");
      
      Artikl mainArtikl_rec;

      if(artstat_rec != null)
      {
         rtrans_rec.T_rbt1St = artstat_rec.PreDefRbt1;
         rtrans_rec.T_jedMj  = artstat_rec.ArtiklJM  ;
      }

      rtrans_rec.FakRbr = fakRbr;

      // !!! 
    //rtrans_rec.T_serial     = rptXtrans_rec.T_serial  ; // ova dva sluze za vezu na rptXtrans_rec     
      rtrans_rec.T_recID      = xtrans_rec.T_recID   ; // ova dva sluze za vezu na rptXtrans_rec     
      rtrans_rec.T_kupdobCD   = xtrans_rec.T_kupdobCD; // poslije ih ionako pregazi AutoSetFaktur 

      if(ZXC.IsStillOldPdv23_ForThisDate(mixer_rec.DokDate))
      {
         rtrans_rec.T_pdvSt = 23.00M;
      }
      else
      {
         rtrans_rec.T_pdvSt = 25.00M;
      }

      if(linkedArtikl_rec != null)
      {
         rtrans_rec.T_artiklCD   = linkedArtikl_rec.ArtiklCD;
         rtrans_rec.T_artiklName = linkedArtikl_rec.ArtiklName;
         rtrans_rec.T_konto      = linkedArtikl_rec.Konto;
         rtrans_rec.T_kol        = 1.00M;
         rtrans_rec.T_cij        = linkedArtikl_rec.ImportCij;

         #region KPD sifra

         if(ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B)
         {
            string kpdSifra = linkedArtikl_rec.KPD;

            if(kpdSifra.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, $"KPD šifra je obavezna na eRačun dokumentima!\n\n ArtiklCD: {linkedArtikl_rec.ArtiklCD}");
            }
         }

         #endregion KPD sifra

      }
      else
      {
         mainArtikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(art => art.ArtiklCD == xtrans_rec.T_artiklCD);

         #region KPD sifra

         if(ZXC.CURR_prjkt_rec.F2_Ima_F2_B2B)
         {
            string kpdSifra = mainArtikl_rec.KPD;

            if(kpdSifra.IsEmpty())
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, $"KPD šifra je obavezna na eRačun dokumentima!\n\n ArtiklCD: {mainArtikl_rec.ArtiklCD}");
            }
         }

         #endregion KPD sifra

         rtrans_rec.T_artiklCD   = xtrans_rec.T_artiklCD;
         rtrans_rec.T_artiklName = xtrans_rec.T_artiklName;
         
         if(mainArtikl_rec != null) rtrans_rec.T_konto = mainArtikl_rec.Konto;

         if(mainArtikl_rec != null && mainArtikl_rec.PdvKat == "00") rtrans_rec.T_pdvSt = 0.00M;

         rtrans_rec.T_kol        = xtrans_rec.T_kol;

         decimal money;
         if(mixer_rec.DevName.IsEmpty() ||
            mixer_rec.DevName == /*"HRK"*/ ZXC.EURorHRKstr)
         {
            money = xtrans_rec.T_moneyA;
         }
         else
         {
            money = mixer_rec.DevTecaj * xtrans_rec.T_moneyA;
         }
         rtrans_rec.T_cij = money;
      }

      rtrans_rec.CalcTransResults(null);

      return rtrans_rec;
   }

   private string LimitedStrFtrans(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.FtransDao.GetSchemaColumnSize(cIdx));
   }
   private string LimitedStrNalog(string data, int cIdx)
   {
      return ZXC.LenLimitedStr(data, ZXC.NalogDao.GetSchemaColumnSize(cIdx));
   }

   private void MIX_CreateRaster_Only1Faktur(object sender, EventArgs e)
   {
      VvOneRowActionDlg dlg = new VvOneRowActionDlg();

      dlg.Fld_LblText = "Fakturiraj samo redak broj:";

      if(dlg.ShowDialog() == DialogResult.OK)
      {
         MixerDUC theDUC = TheVvUC as MixerDUC;
         int rowCount = theDUC.TheG.RowCount;
         int rowIdx = dlg.Fld_OneRow;
         if(rowIdx.IsPositive() && rowIdx <= rowCount)
         {
            MIX_CreateRasterFaktur(rowIdx, EventArgs.Empty);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postojeći redak.");
         }
      }

      dlg.Dispose();

   }

   private void Create_FakturList_To_PDF(object sender, EventArgs e)
   {
      ToolStripButton btn = sender as ToolStripButton;
      ushort subDsc = 0; 

      switch(btn.Name)
      {
         case "PDF_A": subDsc = 1; break;
         case "PDF_B": subDsc = 2; break;
         case "PDF_C": subDsc = 3; break;
         case "PDF_D": subDsc = 4; break;
         default:      subDsc = 0; break;
      }

      List<Faktur> theFakturList = GetDistinct_FakturList_FromRaster();

      VvRiskReport.FakturListTo_PDF(theFakturList/*.Skip(33).ToList()*/, subDsc);

   }

   private List<Faktur> GetDistinct_FakturList_FromRaster()
   {
      Mixer mixer_rec = TheVvDataRecord as Mixer;

      List<Xtrans> xtransList = mixer_rec.Transes.Where(xtr => xtr.T_kpdbMjestoA_32.NotEmpty() && xtr.T_isXxx == false).ToList();

      if(xtransList.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Nema faktura.");
         return null;
      }

      List<Faktur> theFakturList = new List<Faktur>();

      List<string> tipBrList = xtransList.Select(xtr => xtr.T_kpdbMjestoA_32).Distinct().ToList();

      string tt;
      uint ttNum;
      Faktur faktur_rec;
      bool ok;

      foreach(string tipBr in tipBrList)
      {
         Ftrans.ParseTipBr(tipBr, out tt, out ttNum);

         faktur_rec = new Faktur();

         ok = FakturDao.SetMeFaktur(TheDbConnection, faktur_rec, tt, ttNum, false);
         if(ok)
         {
            faktur_rec.VvDao.LoadTranses(TheDbConnection, faktur_rec, false);
            theFakturList.Add(faktur_rec);
         }
      }

      return theFakturList;
   }

   #endregion RASTER

   #region PUTNAL, LOKO

   public void PutNalLoko_CreateNalog(object sender, EventArgs e)
   {
      #region Init stuff

      VvCreateNalogDUCDlg dlg = new VvCreateNalogDUCDlg();

      if(dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      ushort line = 0;
      decimal ukDug = 0.00M, ukPot = 0.00M;
      Mixer mixer_rec = (Mixer)TheVvDataRecord;

    //KtoShemaDsc ktoShemaDsc = new KtoShemaDsc(ZXC.dscLuiLst_KtoShema);
      KtoShemaDsc ktoShemaDsc = ZXC.KSD;
      string konto, relacija, zadatak;
      Kupdob kupdob_rec = VvUserControl.KupdobSifrar.FirstOrDefault(kpdb => kpdb.Naziv.Contains(mixer_rec.PersonPrezim) || kpdb.Prezime == mixer_rec.PersonPrezim); if(kupdob_rec == null) kupdob_rec = new Kupdob();
      decimal /*moneyUkupno, moneyAkont, moneyDnevn, moneyPrevoz, moneyXtrano,*/ dug, pot;
      DateTime date;

      #endregion Init stuff

      #region Real stuff

      // ___ Here we GO! _______________________________________________________________________________ 

      switch(((Mixer)TheVvDataRecord).TT)
      {
         #region TT_PUTN_T

         case Mixer.TT_PUTN_T:

            date = mixer_rec.DateB; // PNL: dokDate, PNIT: dnevnicaDate

            decimal moneyUkupno = mixer_rec.R_PutNalToPay;
            decimal moneyAkont  = mixer_rec.MoneyA;
            decimal moneyDnevn  = mixer_rec.R_moneyBxC;
            decimal moneyPrevoz = mixer_rec.Sum_KolMoneyA;
            decimal moneyXtrano;

            relacija = mixer_rec.StrC_32 ; 
            zadatak  = mixer_rec.StrB_128;  

            konto = ktoShemaDsc.Dsc_PNTukupno  ; dug = 0.00M;       pot = moneyUkupno; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            konto = ktoShemaDsc.Dsc_PNTacc     ; dug = 0.00M;       pot = moneyAkont ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);

            if(mixer_rec.Konto.NotEmpty() ) konto = mixer_rec.Konto;
            else                            konto = ktoShemaDsc.Dsc_PNTdnevnice; 
                                                                    dug = moneyDnevn ; pot = 0.00M      ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            if(mixer_rec.Konto2.NotEmpty()) konto = mixer_rec.Konto2; 
            else                            konto = ktoShemaDsc.Dsc_PNTprijevTr; 
                                                                    dug = moneyPrevoz; pot = 0.00M      ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);

            zadatak = "";

            foreach(Xtrano xtrano_rec in mixer_rec.Transes2)
            {
               konto = xtrano_rec.T_konto.NotEmpty() ? xtrano_rec.T_konto : ktoShemaDsc.Dsc_PNTostaliTr;
               
               relacija = xtrano_rec.T_opis_128;

               moneyXtrano = xtrano_rec.T_moneyA;

               dug = moneyXtrano; pot = 0.00M; 
               
               SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            }

            break;

         #endregion TT_PUTN_T

         #region TT_PUTN_I

         case Mixer.TT_PUTN_I: 

            date = mixer_rec.DateB; // PNIT: dnevnicaDate! 

            // ===                                 
            /* */ mixer_rec.ConvertPutNalInKune();
            // ===                                 

            relacija = mixer_rec.StrC_32 ; 
            zadatak  = mixer_rec.StrB_128;  

            konto = ktoShemaDsc.Dsc_PNTukupno  ; dug = 0.00M;                        pot = mixer_rec.KonvertMoneyUkupno; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            konto = ktoShemaDsc.Dsc_PNTacc     ; dug = 0.00M;                        pot = mixer_rec.KonvertMoneyAkont ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);

            if(mixer_rec.Konto.NotEmpty() ) konto = mixer_rec.Konto;
            else                            konto = ktoShemaDsc.Dsc_PNTdnevnice; 
                                                 dug = mixer_rec.KonvertMoneyDnevn ; pot = 0.00M                       ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            if(mixer_rec.Konto2.NotEmpty()) konto = mixer_rec.Konto2; 
            else                            konto = ktoShemaDsc.Dsc_PNTprijevTr; 
                                                 dug = mixer_rec.KonvertMoneyPrevoz; pot = 0.00M                       ; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);

            zadatak = "";

            foreach(Xtrano xtrano_rec in mixer_rec.Transes2)
            {
               konto = xtrano_rec.T_konto.NotEmpty() ? xtrano_rec.T_konto : ktoShemaDsc.Dsc_PNTostaliTr;
               
               relacija = xtrano_rec.T_opis_128;

               moneyXtrano = xtrano_rec.T_moneyA * ZXC.DevTecDao.GetHnbTecaj(ZXC.GetValutaNameEnumFromValutaName(xtrano_rec.T_devName), date);

               dug = moneyXtrano; pot = 0.00M; 
               
               SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            }

            break;

         #endregion TT_PUTN_I

         #region TT_PUTN_L

         case Mixer.TT_PUTN_L:

            date = mixer_rec.DokDate; // PNL: dokDate, PNIT: dnevnicaDate

            relacija = mixer_rec.Transes.Min(trn => trn.T_dateOd).ToString(ZXC.VvDateDdMmFormat); 
            zadatak  = mixer_rec.Transes.Max(trn => trn.T_dateOd).ToString(ZXC.VvDateFormat    );  

            konto = ktoShemaDsc.Dsc_LokoUkup    ; dug = 0.00M;                   pot = mixer_rec.R_PutNalAllTr; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            konto = ktoShemaDsc.Dsc_LokoPrijevTr; dug = mixer_rec.Sum_KolMoneyA; pot = 0.00M;                   SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);

            zadatak = "";

            foreach(Xtrano xtrano_rec in mixer_rec.Transes2)
            {
               konto = xtrano_rec.T_konto.NotEmpty() ? xtrano_rec.T_konto : ktoShemaDsc.Dsc_LokoOstTr;
               
               relacija = xtrano_rec.T_opis_128;

               dug = xtrano_rec.T_moneyA; pot = 0.00M; SetNalogLineForPutNal(konto, date, kupdob_rec, dug, pot, ref line, mixer_rec, relacija, zadatak, ref ukDug, ref ukPot);
            }

            break;

         #endregion TT_PUTN_L

      }

      // ___ Here we END! _______________________________________________________________________________ 

      #endregion Real stuff

      #region Final stuff

      Cursor.Current = Cursors.WaitCursor;

      if(dlg.Fld_OpenNalogDUC == true)
      {
         OpenNew_Record_TabPage(GetSubModulXY(ZXC.VvSubModulEnum.NAL_F), ZXC.NalogRec.RecID);
      }
      else
      {
         ZXC.aim_emsg("Gotovo. Dodao nalog sa {0} stavaka. UkDug: {1:N}   UkPot: {2:N}.", line, ukDug, ukPot);
      }

      dlg.Dispose();

      Cursor.Current = Cursors.Default;

      #endregion Final stuff

      return;
   }

   private void SetNalogLineForPutNal(string konto, DateTime date, Kupdob kupdob_rec, decimal dug, decimal pot, ref ushort line, Mixer mixer_rec, string relacija, string zadatak, ref decimal ukDug, ref decimal ukPot)
   {
      string opis = opis = LimitedStrFtrans(string.Format("{0} {1}-{2}", mixer_rec.TipBr, relacija, zadatak), ZXC.FtrCI.t_opis);
      
      if((dug + pot).NotZero()) AddNalogLineForPutNal(ref line, date, konto, kupdob_rec, opis, dug, pot, mixer_rec, ref ukDug, ref ukPot);
   }

   private void AddNalogLineForPutNal(ref ushort line, DateTime date, string konto, Kupdob kupdob_rec, string opis, decimal dug, decimal pot, Mixer mixer_rec, ref decimal ukDug, ref decimal ukPot)
   {
      bool IsVisibelPozicija = ((ZXC.CURR_prjkt_rec.IsNeprofit && konto.StartsWith("4")) || (ZXC.CURR_prjkt_rec.PlanKind == ZXC.PlanKindEnum.PlnBy_FOND && konto.StartsWith("3")));

      NalogDao.AutoSetNalog(TheDbConnection, ref line,

         /* DateTime n_dokDate   */ date /*mixer_rec.DokDate*/,
         /* string   n_tt        */ "TM",
         /* string   n_napomena  */ opis,
         /* string   t_konto     */ konto,
         /* string   t_kupdob_cd */ kupdob_rec.KupdobCD,
         /* string   t_ticker    */ kupdob_rec.Ticker,
         /* string   t_mtros_cd  */ mixer_rec.MtrosCD,
         /* string   t_mtros_tk  */ mixer_rec.MtrosTK,
         /* string   t_tipBr     */ mixer_rec.TipBr,
         /* string   t_opis      */ opis,
         /* string   t_valuta    */ DateTime.MinValue,
         /* string   t_pdv       */ "",
         /* string   t_037       */ "",
         /* string   t_projektCD */ mixer_rec.ProjektCD,
         /* ushort   t_pdvKnjiga */ ZXC.PdvKnjigaEnum.NIJEDNA,
         /* uint     t_fakRecID  */ 0,
         /* uint     t_fakYear   */ 0,
         /* OtsKindEnum t_otsKind*/ ZXC.OtsKindEnum.NIJEDNO,
         /* string   t_fond      */ "",
         /* string   t_pozicija  */ IsVisibelPozicija ? mixer_rec.ExternLink2 : "",
         /* decimal  t_dug       */ dug,
         /* decimal  t_pot       */ pot);
      //------------------------------------------------------------------------- 

      ukDug += dug;
      ukPot += pot;
   }

   private void MIXER_Xtrano_7030(object sender, EventArgs e)
   {
      MixerDUC theDUC = TheVvDocumentRecordUC as MixerDUC;
      if(theDUC.TheG.Visible) return;
      if(theDUC.TheG2.CurrentRow == null) return;

      int currRowIdx   = theDUC.TheG2.CurrentRow.Index;

      theDUC.TheG2.Rows.Insert(currRowIdx, theDUC.TheG2.CloneWithValues(theDUC.TheG2.CurrentRow));

      decimal cij100 = theDUC.TheG2.GetDecimalCell(theDUC.DgvCI2.iT_moneyA, currRowIdx, false);
      decimal cij70  = (cij100 * 0.70M).Ron2();
      decimal cij30  = (cij100 - cij70).Ron2();

      theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_moneyA, currRowIdx    , cij70);
      theDUC.TheG2.PutCell(theDUC.DgvCI2.iT_moneyA, currRowIdx + 1, cij30);

      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx    );
      theDUC.GetLineFlds_CalcTrans_PutLineResultFlds_PutTransSumFlds(currRowIdx + 1);
   }

   private void CiribuCiriba(object sender, EventArgs e)
   {
      CiribuCiribaDlg dlg = new CiribuCiribaDlg();

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      //========================================================================= 
      Cursor.Current = Cursors.WaitCursor;

      (TheVvUC as PutNalDUC).Perform_CiribuCiriba(dlg.TheUC);
      
      ShowNews();

      Cursor.Current = Cursors.Default;

      //========================================================================= 

      dlg.Dispose();
   }
   
   #endregion PUTNAL, LOKO

   #region GFI, TSI, PD

   private void GFI_TSI_ExportToExcel(object sender, EventArgs e)
   {
      GFI_TSI_DUC theDUC    = TheVvUC as GFI_TSI_DUC;
      if(theDUC.Fld_ExternLink2.IsEmpty())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "SaveAs Excel ime datoteke je prazno.\n\nZadajte, molim, ime nove datoteke.");
         return;
      }

      if(theDUC.Fld_IntA.IsZero()) // offset 
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Zadajte, molim, offset.");
         return;
      }

      GFI_TSI_ExportToExcel_JOB();

      // Clean up the unmanaged Excel COM resources by forcing a garbage 
      // collection as soon as the calling function is off the stack (at 
      // which point these objects are no longer rooted).

      GC.Collect();
      GC.WaitForPendingFinalizers();
      // GC needs to be called twice in order to get the Finalizers called 
      // - the first time in, it simply makes a list of what is to be 
      // finalized, the second time in, it actually is finalizing. Only 
      // then will the object do its automatic ReleaseComObject.
      GC.Collect();
      GC.WaitForPendingFinalizers();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.");
   }

   private void GFI_TSI_ExportToExcel_JOB()
   {

        // 01.09.2021: VvKristal-NEW je Windows 10 sa MS Office 2016 pa ovo ne moze ...
#if stariOffice2010

      #region variablez 

      GFI_TSI_DUC theDUC    = TheVvUC as GFI_TSI_DUC;
      Mixer       mixer_rec = theDUC.mixer_rec/*.MakeDeepCopy()*/;

      Point xy;

      int aopInXls, aopX, offset = mixer_rec.IntA;

      object missing = Type.Missing;

      string Cell_XY ;
      int    AOP     ;
      string TheRule ;
      string Tip     ;
      bool   isConst;

      Excel.Application xlsApp       = null;
      Excel.Workbook    xlsWorkBook  = null;
      Excel.Worksheet   xlsWorkSheet = null;
      Excel.Range       xlsRange     = null;

      #endregion variablez

      try
      {
         #region Init Excel operations 

         string origFileName   = mixer_rec.ExternLink1;
         string saveAsFileName = mixer_rec.ExternLink2;
         string sheetName      = mixer_rec.StrC_32    ;
   
         xlsApp = new Excel.Application();
         //xlsApp.Visible = true; // OcuCevapOcuPljeskavicu - StoCu?! 
         xlsWorkBook = xlsApp.Workbooks.Open(origFileName, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
         xlsWorkSheet = (Excel.Worksheet)xlsWorkBook.Worksheets.get_Item(sheetName);

         #endregion Init Excel operations

         #region Send Values

         // Cell XY - xtrans_rec.T_kpdbUlBrA_32 
         // AOP     - xtrans_rec.T_intA         
         // TheRule - xtrans_rec.T_kpdbNameA_50 
         // Tip     - xtrans_rec.T_strA_2       
         // Offset  - mixer_rec .IntA           

         foreach(Xtrans xtrans_rec in mixer_rec.Transes)
         {
          //Cell_XY = xtrans_rec.T_kpdbUlBrA_32 ;
            Cell_XY = xtrans_rec.T_kpdbUlBrA_32 .Replace(" ", "");
            AOP     = xtrans_rec.T_intA         ;
            TheRule = xtrans_rec.T_kpdbNameA_50 ;
            Tip     = xtrans_rec.T_strA_2       ;
            isConst = Tip.StartsWith("N")       ;

            if(Cell_XY.IsEmpty() || 
               AOP.IsZero()      || 
               TheRule.IsEmpty() ||
               Tip.StartsWith("S")) continue;

            // ====================================================================================================================== 


            xy = Exy(Cell_XY);

            aopX = xy.X - offset;

            if(xy.X.IsZeroOrNegative() || xy.Y.IsZeroOrNegative()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{1}] Cell address [{2}] [{0}] besmislena!", xy, xtrans_rec.T_serial, Cell_XY); continue; }
            if(aopX.IsZeroOrNegative()                           ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Redak [{3}] Cell address [{4}] [{0}] minus offset [{1}] = [{2}]\n\nKrivi offset ili krivi cell address?!", xy, offset, aopX, xtrans_rec.T_serial, Cell_XY); continue; }

            xlsRange = xlsWorkSheet.Range[AopColName(aopX) + xy.Y];

            aopInXls = (int)xlsRange.Value;

            if(aopInXls != AOP) { ZXC.aim_emsg(MessageBoxIcon.Error, "Cell address [{0}] AOP [{1}]\n\nU Excel datoteci je za taj cell predviđen AOP [{2}]", Cell_XY, AOP.ToString("000"), aopInXls.ToString("000")); continue; }

            xlsWorkSheet.Cells[xy.Y, xy.X] = theDUC.GetTheAOPValue(xtrans_rec); // VOILA AOP value

            if(xtrans_rec.T_moneyA.NotZero()) xlsWorkSheet.Cells[xy.Y, xy.X - 1] = xtrans_rec.T_moneyA; // PRETHODNA GODINA value

            if(sheetName == ZXC.luiListaGFI_Bilanca_Name.Substring(4) ||
               sheetName == ZXC.luiListaGFI_RDG_Name    .Substring(4) ||
               sheetName == ZXC.luiListaGFI_NT_I_Name   .Substring(4) ||
               sheetName == ZXC.luiListaGFI_NT_D_Name   .Substring(4)  )
            {
               xlsWorkSheet.Cells[xy.Y, xy.X - 2] = xtrans_rec.T_kpdbMjestoA_32; // RedniBrojBiljeske value 
            }

            // ====================================================================================================================== 
         }

         #endregion Send Values

         #region Save and Close Excel operations

         xlsWorkBook.SaveAs(saveAsFileName, Excel.XlFileFormat.xlExcel8/*xlOpenXMLWorkbook*/,
              missing, missing, missing, missing,
              Excel.XlSaveAsAccessMode.xlNoChange,
              missing, missing, missing, missing, missing);

         xlsWorkBook.Close(false/*true*/, missing, missing);
         xlsApp.Quit();

         #endregion Save and Close Excel operations

      }

      #region catch & finally 

      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "GFI_TSI_ExportToExcel_JOB throws the error: " + ex.Message);
      }
      finally
      {
         // Clean up the unmanaged Excel COM resources by explicitly 
         // calling Marshal.FinalReleaseComObject on all accessor objects. 
         // See http://support.microsoft.com/kb/317109.

         // 22.06.2015: 
       //if(xlsRange     != null)       { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsRange    ); xlsRange     = null; }
       //if(xlsWorkSheet != null)       { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsWorkSheet); xlsWorkSheet = null; }
       //if(xlsWorkBook  != null)       { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsWorkBook ); xlsWorkBook  = null; }
       //if(xlsApp       != null)       { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsApp      ); xlsApp       = null; }
         if(xlsRange     != null) { try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsRange    ); xlsRange     = null; } catch { ; } }
         if(xlsWorkSheet != null) { try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsWorkSheet); xlsWorkSheet = null; } catch { ; } }
         if(xlsWorkBook  != null) { try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsWorkBook ); xlsWorkBook  = null; } catch { ; } }
         if(xlsApp       != null) { try { System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlsApp      ); xlsApp       = null; } catch { ; } }
      }

        #endregion catch & finally

#endif

    }

    private Point Exy(string a1str)
   {
      int i, x, y;

      for(i = 0; i < a1str.Length; ++i)
      {
         if(Char.IsDigit(a1str[i])) break;
      }

      x = ZXC.ExcelColNumber(a1str.Substring(0, i));
      y = ZXC.ValOrZero_Int(a1str.Substring(i));

      return new Point(x, y);
   }

   private string AopColName(int aopX)
   {
      return ZXC.ExcelColName(aopX);
   }

   #endregion GFI, TSI, PD

   #region GFI TSI LookUpListe

   private void ViewLookUp_TSI_Podaci (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaTSI_Podaci , ats_SubModulSet[xy.X][xy.Y], "TSI_Podaci" ,ZXC.Q10un + ZXC.Q5un); }
   private void ViewLookUp_GFI_Bilanca(object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaGFI_Bilanca, ats_SubModulSet[xy.X][xy.Y], "GFI_Bilanca",ZXC.Q10un + ZXC.Q5un); }
   private void ViewLookUp_GFI_RDG    (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaGFI_RDG    , ats_SubModulSet[xy.X][xy.Y], "GFI_RDG"    ,ZXC.Q10un + ZXC.Q5un); }
   private void ViewLookUp_GFI_PodDod (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaGFI_PodDop , ats_SubModulSet[xy.X][xy.Y], "GFI_PodDop" ,ZXC.Q10un + ZXC.Q5un); }
   private void ViewLookUp_GFI_NT_I   (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaGFI_NT_I   , ats_SubModulSet[xy.X][xy.Y], "GFI_NT_I"   ,ZXC.Q10un + ZXC.Q5un); }
   private void ViewLookUp_GFI_NT_D   (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.GFI_TSI); Create_LUI_Dlg(ZXC.luiListaGFI_NT_D   , ats_SubModulSet[xy.X][xy.Y], "GFI_NT_D"   ,ZXC.Q10un + ZXC.Q5un); }

   private void MIX_Popuni_GFI_TSI_DUC(object sender, EventArgs e)
   {
      GFI_TSI_DUC theDUC = TheVvUC as GFI_TSI_DUC;

      ZXC.luiListaTSI_Podaci .LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaTSI_Podaci ); 
      ZXC.luiListaGFI_Bilanca.LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaGFI_Bilanca);
      ZXC.luiListaGFI_RDG    .LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaGFI_RDG    );
      ZXC.luiListaGFI_PodDop .LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaGFI_PodDop );
      ZXC.luiListaGFI_NT_I   .LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaGFI_NT_I   );
      ZXC.luiListaGFI_NT_D   .LazyLoad(); theDUC.Load_GFI_TSI_FromLookUpList(ZXC.luiListaGFI_NT_D   );

      ShowNews();
   }

   #endregion LookUpListe

   #region STATISTIKA NPF LookUpListe

   private void ViewLookUp_PR_RAS_NPF   (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.STAT_NPF); Create_LUI_Dlg(ZXC.luiLista_PR_RAS_NPF  , ats_SubModulSet[xy.X][xy.Y], "PR_RAS_NPF"   ,ZXC.Q10un *2/* ZXC.Q5un*/); }
   private void ViewLookUp_S_PR_RAS_NPF (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.STAT_NPF); Create_LUI_Dlg(ZXC.luiLista_S_PR_RAS_NPF, ats_SubModulSet[xy.X][xy.Y], "S_PR_RAS_NPF" ,ZXC.Q10un *2/* ZXC.Q5un*/); }
   private void ViewLookUp_BIL_NPF      (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.STAT_NPF); Create_LUI_Dlg(ZXC.luiLista_BIL_NPF     , ats_SubModulSet[xy.X][xy.Y], "BIL_NPF"      ,ZXC.Q10un *2/* ZXC.Q5un*/); }
   private void ViewLookUp_G_PR_IZ_NPF  (object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.STAT_NPF); Create_LUI_Dlg(ZXC.luiLista_G_PR_IZ_NPF , ats_SubModulSet[xy.X][xy.Y], "G_PR_IZ_NPF " ,ZXC.Q10un *2/* ZXC.Q5un*/); }

   private void MIX_Popuni_STAT_DUC(object sender, EventArgs e)
   {
      Statistika_NPF_DUC theDUC = TheVvUC as Statistika_NPF_DUC;

      ZXC.luiLista_PR_RAS_NPF  .LazyLoad(); theDUC.Load_STAT_NPF_FromLookUpList(ZXC.luiLista_PR_RAS_NPF  ); 
      ZXC.luiLista_S_PR_RAS_NPF.LazyLoad(); theDUC.Load_STAT_NPF_FromLookUpList(ZXC.luiLista_S_PR_RAS_NPF);
    //ZXC.luiLista_BIL_NPF     .LazyLoad(); theDUC.Load_STAT_NPF_FromLookUpList(ZXC.luiLista_BIL_NPF     );
    //ZXC.luiLista_G_PR_IZ_NPF .LazyLoad(); theDUC.Load_STAT_NPF_FromLookUpList(ZXC.luiLista_G_PR_IZ_NPF );

      ShowNews();
   }

   
   #endregion STATISTIKA NPF LookUpListe


   #region ImportExtCjenik

   private void ImportExtCjenik(object sender, EventArgs e)
   {
      #region Initializations, ...

      int  nora=-1, transNora;
      bool isUcitajRacune = false;

      VvTransRecord vvTransRecord=null;

      LoginToLinuxForm loginForm = new LoginToLinuxForm(isUcitajRacune);

      loginForm.Fld_FileName = SuggestImportFileName(TheVvDataRecord.VirtualLegacyRecordPreffix, ZXC.CURR_prjkt_rec.KupdobCD, isUcitajRacune);

      if(TheVvDataRecord.IsDocument && !isUcitajRacune)
      {
         vvTransRecord = ((VvDocumentRecord)TheVvDataRecord).VvTransRecordFactory();

         loginForm.Fld_FileNameTrans = SetTransImportFileName(vvTransRecord.VirtualLegacyRecordPreffix, loginForm.Fld_FileName, TheVvDataRecord.VirtualLegacyRecordPreffix);
         loginForm.tbx_fileNameTrans.Visible = true;
      }


      if(loginForm.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

      Cursor.Current = Cursors.WaitCursor;

      ZXC.OffixImport_InProgress = true;

      string currentLocalDirerctory = System.Environment.CurrentDirectory;

      Environment.CurrentDirectory = Get_MyDocumentsLocation_ProjectAndUser_Dependent(false);

      #endregion Initializations, ...

      #region LOAD DATA INFILE

      // ___ Here we GO! MySQL LOAD DATA INFIle _____________________________________________________________________________ 

      Cursor.Current = Cursors.WaitCursor;

      if(ZXC.CURR_prjkt_rec.Ticker.StartsWith("DUCATI")) // U Mixer! Externi cijenik da Italy 
      {
         VvXtransImporter importer = new VvXtransImporter(TheDbConnection, loginForm.Fld_FileName, "\t");

         nora = importer.LoadData(true);
      }
      //else if(TheVvDataRecord.VirtualRecordName == Artikl.recordName && ZXC.CURR_prjkt_rec.Ticker.StartsWith("FRIGOT"))
      //{
      //   VvArtikl_FRIGOTERM_Importer importer = new VvArtikl_FRIGOTERM_Importer(TheDbConnection, loginForm.Fld_FileName, "\t");

      //   nora = importer.LoadData(true);
      //}


      // ___ Here we END! MySQL LOAD DATA INFIle _____________________________________________________________________________


      loginForm.Dispose();

      Environment.CurrentDirectory = currentLocalDirerctory;

      #endregion LOAD DATA INFILE

      #region Show News

      bool IsNotEmptyTable = false;

      VvSQL.DBNavigActionType frs_lst = (TheVvDataRecord.IsDocumentLike ? VvSQL.DBNavigActionType.LST : VvSQL.DBNavigActionType.FRS);
      IsNotEmptyTable = TheVvDataRecord.VvDao.FrsPrvNxtLst_REC(TheDbConnection, TheVvDataRecord, frs_lst, /*TheVvDataRecord.DefaultSorter*/ TheVvRecordUC.ThePrefferedRecordSorter, IsArhivaTabPage, TheVvUC.VvNavRestrictor_TT, TheVvUC.VvNavRestrictor_SKL, TheVvUC.VvNavRestrictor_SKL2);
      if(IsNotEmptyTable)
      {
         PutFieldsActions(TheDbConnection, TheVvDataRecord, TheVvRecordUC);
         TheVvTabPage.FileIsEmpty = false;
      }
      else
      {
         TheVvTabPage.FileIsEmpty = true;
      }

      SetVvMenuEnabledOrDisabled_RegardingWriteMode(ZXC.WriteMode.None);

      #endregion Show News

      Cursor.Current = Cursors.Default;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\nUcitao datoteku: [{1}]\n\nNumber of rows affected: [{0}].", nora, loginForm.Fld_FileName);

      ZXC.OffixImport_InProgress = false;

      return;
   }

   #endregion ImportExtCjenik

   #region UcitajExcel_PutRadList

   private void UcitajExcel_PutRadList(object sender, EventArgs e)
   {
      TheVvUC.SetSifrarAndAutocomplete<Person>(null, VvSQL.SorterType.Code);
      ZXC.luiListaMixerVozilo.LazyLoad();

      LoadExcelPnrDLG dlg = new LoadExcelPnrDLG(ZXC.ImportMode.ADDREC);

      dlg.ShowDialog();

      dlg.Dispose();

      ShowNews();
   }

   #endregion UcitajExcel_PutRadList

   private void VisibleVrCol(object sender, EventArgs e)
   {
      //16.11.2017:
    //RvrMjesecDUC rvrMjDuc = TheVvUC as RvrMjesecDUC;
     
      MixerDUC mixerDuc = TheVvUC as MixerDUC;
      if(mixerDuc == null || (mixerDuc is RvrMjesecDUC == false && mixerDuc is AvrDUC == false)) return;

      if(/*rvrMjDuc*/ mixerDuc.TheG.Columns["T_str01"].Visible) SetVisibilityVrRadaColumns(false, mixerDuc);
      else                                                      SetVisibilityVrRadaColumns(true , mixerDuc);
   }

   // 16.11.2017: MVR i AVR imaju istu akciju pokazivanja str kolona
 //private void SetVisibilityVrRadaColumns(bool isVisible, RvrMjesecDUC rvrMjDuc)
   private void SetVisibilityVrRadaColumns(bool isVisible, MixerDUC     mixerDuc)
   {

    //rvrMjDuc.TheG.Columns.Cast<DataGridViewColumnCollection>().Where(col => col.Name == "");

      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str01].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str02].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str03].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str04].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str05].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str06].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str07].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str08].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str09].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str10].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str11].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str12].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str13].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str14].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str15].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str16].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str17].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str18].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str19].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str20].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str21].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str22].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str23].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str24].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str25].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str26].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str27].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str28].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str29].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str30].Visible =
      /*rvrMjDuc*/ mixerDuc.TheG.Columns[/*rvrMjDuc*/ mixerDuc.DgvCI.iT_str31].Visible = isVisible;

      // 17.11.2017: dodano jer je str prikazivao i za dane kojih nema u mjesecu
      mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str29].Visible = true ? (mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str29].Visible && mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_dec29].Visible) : false;
      mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str30].Visible = true ? (mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str30].Visible && mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_dec30].Visible) : false;
      mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str31].Visible = true ? (mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_str31].Visible && mixerDuc.TheG.Columns[mixerDuc.DgvCI.iT_dec31].Visible) : false;
      
   }

   #region PLAN

   private void PLAN_PopuniDUC_ttPLN(object sender, EventArgs e) 
   {
      PlanDUC theDUC = TheVvUC as PlanDUC;

      ZXC.luiListaPozicijePlana.LazyLoad(); theDUC.Load_Plan_FromLookUpList(ZXC.luiListaPozicijePlanaPLN); 

      ShowNews();
   }

   private void PLAN_PopuniDUC_ttPBN(object sender, EventArgs e)
   {
      PlanDUC theDUC = TheVvUC as PlanDUC;

      ZXC.luiListaPozicijePlanaPBN.LazyLoad(); theDUC.Load_Plan_FromLookUpList(ZXC.luiListaPozicijePlanaPBN);

      ShowNews();
   }

   private void ViewLookUp_PozicijePlana(object sender, EventArgs e) 
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.PLAN); 
      Create_LUI_Dlg(ZXC.luiListaPozicijePlana, ats_SubModulSet[xy.X][xy.Y], "Pozicije plana", ZXC.Q10un + ZXC.Q5un); 
   }
   private void ViewLookUp_PozicijePlanaPBN(object sender, EventArgs e)
   {
      Point xy = GetSubModulXY(ZXC.VvSubModulEnum.PLAN);
      Create_LUI_Dlg(ZXC.luiListaPozicijePlanaPBN, ats_SubModulSet[xy.X][xy.Y], "Pozicije plana nabave", ZXC.Q10un * 2);
   }

   protected void Xtrans_RenamePozicija(object sender, EventArgs e)
   {
      VvRenamePozicijaDlg dlg = new VvRenamePozicijaDlg();
      Mixer Mixer_rec = TheVvDataRecord as Mixer;
      int nora, colIdx;
      string message = "";
      VvDaoBase vvDaoBase;

      string oldPozicija;
      string newPozicija;

      //dlg.Fld_NewPozicija = Mixer_rec.Pozicija;

      if(dlg.ShowDialog() != DialogResult.OK) { dlg.Dispose(); return; }

      oldPozicija = dlg.Fld_OldPozicija;
      newPozicija = dlg.Fld_NewPozicija;

      dlg.Dispose();

      // 17.02.2016: 'zz_PozicijePlana' ---> 'zz_PozicijePlana_2015' & 'zz_PozicijePlana_2016' 
      if(oldPozicija.IsEmpty() || 
         newPozicija.IsEmpty())
      {
         DialogResult result = MessageBox.Show("Da li zaista zelite umnožiti\n\n'zz_PozicijePlana' --->\n\n'zz_PozicijePlana_2015' & 'zz_PozicijePlana_2016'?",
                                       "Potvrdite KLONIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
         if(result != DialogResult.Yes) return;

         bool OK;

         string origLuiTable  = ZXC.vvDB_luiPrefix + ZXC.luiListaPozicijePlana_Name          ;
         string the15LuiTable = ZXC.vvDB_luiPrefix + ZXC.luiListaPozicijePlana_Name + "_2015";
         string the16LuiTable = ZXC.vvDB_luiPrefix + ZXC.luiListaPozicijePlana_Name + "_2016";

         OK = VvDaoBase.CLONE_TABLE(ZXC.PrjConnection, origLuiTable, the15LuiTable, out nora);

         if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "Klonirao\n\n{0}\n\nu\n\n{1}\n\n{2} rows affected.", origLuiTable, the15LuiTable, nora);
         else
         {
            OK = VvDaoBase.COPY_TABLE(ZXC.PrjConnection, ZXC.PrjConnection.Database, origLuiTable, ZXC.PrjConnection.Database, the15LuiTable, out nora);
            if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "KOPIRAO\n\n{0}\n\nu\n\n{1}\n\n{2} rows affected.", origLuiTable,  the15LuiTable, nora);
         }

         OK = VvDaoBase.CLONE_TABLE(ZXC.PrjConnection, origLuiTable, the16LuiTable, out nora);

         if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "Klonirao\n\n{0}\n\nu\n\n{1}\n\n{2} rows affected.", origLuiTable, the16LuiTable, nora);
         else
         {
            OK = VvDaoBase.COPY_TABLE(ZXC.PrjConnection, ZXC.PrjConnection.Database, origLuiTable, ZXC.PrjConnection.Database, the16LuiTable, out nora);
            if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "KOPIRAO\n\n{0}\n\nu\n\n{1}\n\n{2} rows affected.", origLuiTable,  the16LuiTable, nora);
         }

         ZXC.luiListaPozicijePlana.LazyLoadForced();

         return; // !!! 
      }

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

      //============================================================================================================= 

      vvDaoBase = ZXC.FtransDao; colIdx = ZXC.FtrCI .t_pozicija   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldPozicija, newPozicija); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .vezniDok2    ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldPozicija, newPozicija); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.XtransDao; colIdx = ZXC.XtrCI.t_kpdbZiroA_32; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldPozicija, newPozicija); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);
      vvDaoBase = ZXC.MixerDao ; colIdx = ZXC.MixCI.externLink2   ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldPozicija, newPozicija); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);

      ZXC.luiListaPozicijePlana.Where(lui => lui.Cd == oldPozicija).ToList().ForEach(l => l.Cd = newPozicija); 
      VvDaoBase.SaveLookUpListToSqlTable(ZXC.luiListaPozicijePlana);
      VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ false);
      VvLookUpLista.LoadResultLuiList_PozicijePlana_PLN_or_RLZ(/* isPLN */ true );

      (TheVvUC as PlanDUC).vvtbT_kpdbZiroA_32.JAM_Set_LookUpTable(ZXC.luiListaPozicijePlanaPLN, (int)ZXC.Kolona.prva);

      //vvDaoBase = ZXC.FaktExDao; colIdx = ZXC.FexCI .posJedTK     ; nora = RenameThisSucker(TheDbConnection, vvDaoBase, colIdx, oldPozicija, newPozicija); message += String.Format("Preimenovao {0} stavaka za <{1}.{2}>.\n", nora, vvDaoBase.TableName, vvDaoBase.TheSchemaTable.Rows[colIdx]["ColumnName"]);

      //============================================================================================================= 

      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 
      // *#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*#*# 

      ShowNews();

      ZXC.aim_emsg("Preimenovanje gotovo.\n\n" + message);
   }


   #endregion PLAN

   private void ViewLookUp_KlasifikOzn(object sender, EventArgs e) { Point xy = GetSubModulXY(ZXC.VvSubModulEnum.X_URZ); Create_LUI_Dlg(ZXC.luiListaKlasifikALL, ats_SubModulSet[xy.X][xy.Y], "KLASIFIKACIJSKE OZNAKE", ZXC.Q10un * 2/* ZXC.Q5un*/); }

   private void MIXER_Copy_URZtoRUG(object sender, EventArgs e)
   {
      ZXC.VvDataBaseInfo tabPageOD_dbi = TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage;

      VvSubModul vvSubModul = ZXC.TheVvForm.GetVvSubModulFrom_SubModulEnum(ZXC.VvSubModulEnum.X_RUG);

      VvTabPage existingTabPage = TheTabControl.TabPages.Cast<VvTabPage>().FirstOrDefault(tab => tab.WriteMode == ZXC.WriteMode.None && tab.SubModul_xy == vvSubModul.xy && tab.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName == tabPageOD_dbi.DataBaseName);

      ZXC.MixerRec = (Mixer)(TheVvDataRecord.CreateNewRecordAndCloneItComplete());

      ZXC.MIXER_CopyToOtherDUC_inProgress = true;

      if(existingTabPage != null) existingTabPage.Selected = true; 
      else                        OpenNew_Record_TabPage(vvSubModul.xy, null);

      NewRecord_OnClick(ZXC.MixerRec, new NewRecordEventArgs(TheVvDataRecord, (MixerDUC)TheVvUC, true));
      
      ZXC.MIXER_CopyToOtherDUC_inProgress = false;

   }

   #region RGC

   private void RGC_PopuniDUC(object sender, EventArgs e)
   {
      RegistarCijeviDUC theDUC = TheVvUC as RegistarCijeviDUC;

      ZXC.luiListaSerlot.LazyLoad(); theDUC.Load_RGC_FromLookUpList(ZXC.luiListaSerlot);

      ShowNews();
   }

   private void ViewLookUp_RegistarCijevi(object sender, EventArgs e)
   {
      VvLookUpLista vvLookUpLista = ZXC.luiListaSerlot;

      ZXC.VvSubModulEnum subModulEnum = TheVvUC.TheSubModul.subModulEnum;
      Point xy = ZXC.TheVvForm.GetSubModulXY(subModulEnum);

      vvLookUpLista.LazyLoad();

      LookUpItem_ListView_Dialog dlg;

      dlg = new LookUpItem_ListView_Dialog(vvLookUpLista, ats_SubModulSet[xy.X][xy.Y]/*  tsPanel_SubModul */);
      dlg.Text = "Dodatni podaci za Registar cijevi";

      dlg.listView.Columns[0].Width = ZXC.Q3un;
      dlg.listView.Columns[1].Width = ZXC.Q10un + ZXC.Qun5;
      //dlg.listView.Columns[2].Width = ZXC.Q5un;
      //dlg.listView.Columns[3].Width = ZXC.Q2un;
      //dlg.listView.Columns[4].Width = ZXC.Q2un;
      //dlg.listView.Columns[5].Width = ZXC.Q5un;

      dlg.ShowDialog();
      dlg.Dispose();
   }

   #endregion RGC

   #region MIX_Create_RNM

   private void MIX_Create_RNM(object sender, EventArgs e)
   { 
      int debugCount;

      Mixer mixer_rec = (Mixer)(TheVvDataRecord).CreateNewRecordAndCloneItComplete();

      string message; 

      message = "Da li zaista zelite kreirati RNM dokumente?!";

      //newFakCount = mixer_rec.Transes.Where(xtr => xtr.T_isXxx == false).Select(xtr => xtr.T_kupdobCD).Distinct().Count(); // dole se grupiraju linije po T_kupdobCD-u 

      //DialogResult result = MessageBox.Show("Da li zaista zelite kreirati fakture?! (" + newFakCount + ") komada",
      //   "Potvrdite FAKTURIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      DialogResult result = MessageBox.Show(message, "Potvrdite izradu Radnih Naloga?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      if(mixer_rec.Transes.Any(xtr => xtr.T_kpdbZiroB_32.NotEmpty()))
      {
         message = "Ovaj NZI dokument već ima vezu na neki RNM dokument!\n\nDa li zaista želite izraditi još RNM-a?";
         result = MessageBox.Show(message, "Potvrdite izradu Radnih Naloga?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
         if(result != DialogResult.Yes) return;
      }

      Cursor.Current = Cursors.WaitCursor;

      // here we go! 

      debugCount = Create_RNM_Fakturs(mixer_rec);

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao {0} RNM dokumenta.", debugCount);
   }

   private int Create_RNM_Fakturs(Mixer mixer_rec)
   {
      #region Init, variablez, sifrars, ...

      if(mixer_rec.Transes.Count.IsZero()) return 0;

      TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      ZXC.luiListaSkladista.LazyLoad(); // za skladCD '1' 

      int    debugCount = 0;
      bool   OK;
      ushort line;

      Faktur faktur_rec;
      Rtrans whiteRtrans_rec, greenRtrans_rec;

      Xtrans firstInGR_xtrans_rec;

      Artikl whiteArtikl_rec, greenArtikl_rec;

      // KOLONE
      // 
      // T_artiklCD       "Šifra Proizvoda"
      // T_artiklName     "Naziv Proizvoda"
      // T_kol            "KolProiz"
      // T_vezniDokA_64   "Šifra Materijala"
      // T_opis_128       "Naziv Materijala"
      // T_kpdbZiroA_32   "RGC"
      // T_moneyA         "KolMat"
      // T_kpdbZiroB_32   "RNM"
      // 
      // 
      // ZAGLAVLJE
      // 
      // mixer_rec.KupdobCD    = Fld_KupDobCd        faktEx.KupdobCD
      // mixer_rec.KupdobTK    = Fld_KupDobTk        faktEx.KupdobName
      // mixer_rec.KupdobName  = Fld_KupDobName      faktEx.KupdobTK
      // mixer_rec.StrE_256    = Fld_Primjedba       fakturLocal_rec.Opis
      // mixer_rec.StrD_32     = Fld_VrstaRNM        vezniDok
      // 
      // mixer_rec.Konto       = Fld_IzlaznoSkl 
      // mixer_rec.Konto2      = Fld_UlaznoSkl  
      // mixer_rec.StrF_64     = Fld_Standard   
      // mixer_rec.DateA       = Fld_DateTrazRok;
      // mixer_rec.DateB       = Fld_DateMogRok ;

      #endregion Init, variablez, sifrars, ...

      var rnmGRps = mixer_rec.Transes.GroupBy(xtr => xtr.T_vezniDokA_64 + xtr.T_kpdbZiroA_32);

      foreach(var rnmGR in rnmGRps)
      {
         #region Check data and skip if bad

         firstInGR_xtrans_rec = rnmGR.FirstOrDefault();
         whiteArtikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(firstInGR_xtrans_rec.T_vezniDokA_64);

         if(firstInGR_xtrans_rec == null || whiteArtikl_rec == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0}\n\nNema materijala ('bijeli' artikl) u sifrarniku artikala!\n\nNa RNM idu samo 'zeleni' redci.", firstInGR_xtrans_rec.T_serial);
            //continue;
         }

         #endregion Check data and skip if bad

         #region Set faktur_rec data

         line = 0;

         faktur_rec = new Faktur();
         
         faktur_rec.TT         = Faktur.TT_RNM                   ;
         faktur_rec.TtSort     = ZXC.TtInfo(faktur_rec.TT).TtSort;
         faktur_rec.SkladDate  =
         faktur_rec.DokDate    = mixer_rec.DokDate               ;
         faktur_rec.SkladCD    = mixer_rec.Konto                 ;
         faktur_rec.SkladCD2   = mixer_rec.Konto2                ;
         faktur_rec.DokNum     = faktur_rec.VvDao.GetNextDokNum(TheDbConnection, Faktur.recordName);
         faktur_rec.TtNum      = faktur_rec.VvDao.GetNextTtNum(TheDbConnection, faktur_rec.VirtualTT, faktur_rec.SkladCD, false, mixer_rec.StrD_32);
         faktur_rec.ProjektCD  = faktur_rec.TT_And_TtNum         ;
         faktur_rec.Napomena   = mixer_rec.Napomena              ;
         faktur_rec.Opis       = mixer_rec.StrE_256              ;
         faktur_rec.VezniDok   = mixer_rec.StrD_32               ;
         faktur_rec.VezniDok2  = mixer_rec.StrF_64               ;
         faktur_rec.DospDate   = mixer_rec.DateA                 ;
         faktur_rec.V1_tt      = mixer_rec.TT                    ;
         faktur_rec.V1_ttNum   = mixer_rec.TtNum                 ;
         faktur_rec.V2_tt      = mixer_rec.V1_tt                 ;
         faktur_rec.V2_ttNum   = mixer_rec.V1_ttNum              ;
         faktur_rec.V3_tt      = mixer_rec.V2_tt                 ;
         faktur_rec.V3_ttNum   = mixer_rec.V2_ttNum              ;
         faktur_rec.OpciAvalue = mixer_rec.TT_And_TtNum          ;

         #region Kupdob Data

         Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(mixer_rec.KupdobCD);

         if(kupdob_rec != null)
         {
            faktur_rec.KupdobCD     = kupdob_rec.KupdobCD;
            faktur_rec.KupdobTK     = kupdob_rec.Ticker  ;
            faktur_rec.KupdobName   = kupdob_rec.Naziv   ;
            faktur_rec.KdOib        = kupdob_rec.Oib;
            faktur_rec.VatCntryCode = kupdob_rec.VatCntryCode;
            faktur_rec.KdUlica      = kupdob_rec.Ulica2;
            faktur_rec.KdZip        = kupdob_rec.PostaBr;
            faktur_rec.KdMjesto     = kupdob_rec.Grad;
            faktur_rec.DevName      = kupdob_rec.DevName;

          //faktur_rec.PdvKnjiga    = ZXC.PdvKnjigaEnum.REDOVNA;
          //
          //if(faktur_rec.TT == Faktur.TT_WRN)
          //   faktur_rec.ZiroRn = ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1);
          //
          //if(kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 && faktur_rec.TT == Faktur.TT_WRN)
          //{
          //   faktur_rec.PdvR12 = ZXC.PdvR12Enum.R2;
          //}
          //else
          //{
          //   faktur_rec.PdvR12 = ZXC.PdvR12Enum.R1;
          //}

            #region PoslJed

            faktur_rec.PosJedCD     = kupdob_rec.KupdobCD;
            faktur_rec.PosJedTK     = kupdob_rec.Ticker  ;
            faktur_rec.PosJedName   = kupdob_rec.Naziv   ;
            faktur_rec.PosJedUlica  = kupdob_rec.Ulica2  ;
            faktur_rec.PosJedZip    = kupdob_rec.PostaBr ;
            faktur_rec.PosJedMjesto = kupdob_rec.Grad    ;

            #endregion PoslJed

            #region Putnik 06.11.2013.

            if(kupdob_rec.PutnikID.NotZero()) faktur_rec.PersonCD   = kupdob_rec.PutnikID;
            if(kupdob_rec.PutName.NotEmpty()) faktur_rec.PersonName = kupdob_rec.PutName ;

            #endregion Putnik

            #region EU VAT Code Action

          //if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsUlazniPdvTT)
          //{
          //   faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .NIJEDNA;
          //   faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
          //   faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
          //}
          //if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsIzlazniPdvTT)
          //{
          //   faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .REDOVNA;
          //   faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
          //   faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
          //}

            #endregion EU VAT Code Action

         }
         else ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Nema partnera sa šifrom [{0}]\n\n[{0}] [{1}]", mixer_rec.KupdobCD, mixer_rec.KupdobTK);

         #endregion Kupdob Data

         #endregion Set faktur_rec Data

         #region Set rtrans_rec Data - BIJELI redak (Group Leader)

         if(firstInGR_xtrans_rec != null && whiteArtikl_rec != null)
         {
            whiteRtrans_rec = new Rtrans();
          
            whiteRtrans_rec.T_serlot     = firstInGR_xtrans_rec.T_kpdbZiroA_32;
            whiteRtrans_rec.T_artiklCD   = whiteArtikl_rec.ArtiklCD      ;
            whiteRtrans_rec.T_artiklName = whiteArtikl_rec.ArtiklName    ;
            whiteRtrans_rec.T_jedMj      = whiteArtikl_rec.JedMj         ;
            whiteRtrans_rec.T_ppmvOsn    = whiteArtikl_rec.R_orgPak  ;
            whiteRtrans_rec.T_kol        = rnmGR.Sum(xtr => xtr.T_moneyA);
          //whiteRtrans_rec.T_kol2       = 0M          ;
          //whiteRtrans_rec.T_cij        = 0M          ;
          //whiteRtrans_rec.T_pdvColTip  = dlgPdvKolTip;
          //whiteRtrans_rec.T_pdvSt      = dlgPdvSt;    
            whiteRtrans_rec.T_serial     = ++line;
            whiteRtrans_rec.T_TT         = faktur_rec.TT     ;
            whiteRtrans_rec.T_ttNum      = faktur_rec.TtNum  ;
            whiteRtrans_rec.T_ttSort     = faktur_rec.TtSort ;
            whiteRtrans_rec.T_skladCD    = faktur_rec.SkladCD;
          //whiteRtrans_rec.T_parentID   = faktur_rec.RecID; ... ovoga automatski postavi VvDaoBase.AddrecDocument_AddrecTranses() 
            whiteRtrans_rec.T_dokNum     = faktur_rec.DokNum;
            whiteRtrans_rec.T_skladDate  = faktur_rec.DokDate;
            whiteRtrans_rec.T_kupdobCD   = faktur_rec.KupdobCD;
            whiteRtrans_rec.CalcTransResults(null);
            faktur_rec.Transes.Add(whiteRtrans_rec);

         } // if(firstInGR_xtrans_rec != null && whiteArtikl_rec != null)

         #endregion Set rtrans_rec Data - BIJELI redak (Group Leader - cijev)

         #region Set rtrans_rec Data - ZELENI redci (foreach proizvod)

         foreach(Xtrans xtrans_rec in rnmGR)
         {
            greenRtrans_rec = new Rtrans();

            #region Check data and skip if bad

            greenArtikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(xtrans_rec.T_artiklCD);

            if(greenArtikl_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0}\n\nNema proizvoda ('zeleni' artikl) u sifrarniku artikala!\n\nRNM će imati samo cijev.", xtrans_rec.T_serial);
               continue;
            }

            #endregion Check data and skip if bad

            greenRtrans_rec.T_artiklCD   = greenArtikl_rec.ArtiklCD  ;
            greenRtrans_rec.T_artiklName = greenArtikl_rec.ArtiklName;
            greenRtrans_rec.T_jedMj      = greenArtikl_rec.JedMj     ;
            greenRtrans_rec.T_ppmvOsn    = greenArtikl_rec.R_orgPak  ;
            greenRtrans_rec.T_kol        = xtrans_rec.T_kol          ;
          //greenRtrans_rec.T_kol2       = 0M          ;
          //greenRtrans_rec.T_cij        = 0M          ;
          //greenRtrans_rec.T_pdvColTip  = dlgPdvKolTip;
          //greenRtrans_rec.T_pdvSt      = dlgPdvSt    ;
            greenRtrans_rec.T_serial     = ++line;
            greenRtrans_rec.T_TT         = faktur_rec.TtInfo.SplitTT; // !!! 
            greenRtrans_rec.T_ttNum      = faktur_rec.TtNum         ;
            greenRtrans_rec.T_ttSort     = faktur_rec.TtSort        ;
            greenRtrans_rec.T_skladCD    = faktur_rec.SkladCD       ;
          //greenRtrans_rec.T_parentID   = faktur_rec.RecID; ... ovoga automatski postavi VvDaoBase.AddrecDocument_AddrecTranses() 
            greenRtrans_rec.T_dokNum     = faktur_rec.DokNum        ;
            greenRtrans_rec.T_skladDate  = faktur_rec.DokDate       ;
            greenRtrans_rec.T_kupdobCD   = faktur_rec.KupdobCD      ;
            greenRtrans_rec.CalcTransResults(null) ;
            faktur_rec.Transes.Add(greenRtrans_rec);
         }

         #endregion Set rtrans_rec Data - ZELENI redci

         #region TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

         faktur_rec.TakeTransesSumToDokumentSum(true);

         // NOTA BENE: ovdje ide pravi ADDREC (fak + Transes) a ne AutoSetFaktur koji ne zna sume prije nego sto doda sve transove 
         OK = faktur_rec.VvDao.ADDREC(TheDbConnection, faktur_rec);

         if(OK) ++debugCount;

         #endregion TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

         #region Rwtrec Feedback To Xtrans

         foreach(Xtrans xtrans_rec in rnmGR)
         {
            BeginEdit(xtrans_rec);

            xtrans_rec.T_kpdbZiroB_32 = faktur_rec.TT_And_TtNum;

            xtrans_rec.VvDao.RWTREC(TheDbConnection, xtrans_rec, false, true, false);

            EndEdit(xtrans_rec);
         }

         #endregion Rwtrec Feedback
      }

      return debugCount;
   }

   #endregion MIX_Create_RNM

   #region MIX_Create_RISK_SVD_NRD

   private void MIX_Create_RISK_SVD_NRD(object sender, EventArgs e)
   { 
      int debugCount;

      Mixer mixer_rec = (Mixer)(TheVvDataRecord).CreateNewRecordAndCloneItComplete();

      string message; 

      message = "Da li zaista zelite kreirati NRD dokumente?!";

      //newFakCount = mixer_rec.Transes.Where(xtr => xtr.T_isXxx == false).Select(xtr => xtr.T_kupdobCD).Distinct().Count(); // dole se grupiraju linije po T_kupdobCD-u 

      //DialogResult result = MessageBox.Show("Da li zaista zelite kreirati fakture?! (" + newFakCount + ") komada",
      //   "Potvrdite FAKTURIRANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      DialogResult result = MessageBox.Show(message, "Potvrdite izradu Narudžbi?!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if(result != DialogResult.Yes) return;

      if(mixer_rec.Transes.Any(xtr => xtr.T_kpdbZiroB_32.NotEmpty()))
      {
         message = "Ova prednarudžba već ima vezu na neki NRD dokument!\n\nDa li zaista želite izraditi još NRD-a?";
         result = MessageBox.Show(message, "Potvrdite izradu Narudžbi?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
         if(result != DialogResult.Yes) return;
      }

      Cursor.Current = Cursors.WaitCursor;

      // here we go! 

      debugCount = Create_NRD_Fakturs(mixer_rec);

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao {0} NRD dokumenta.", debugCount);
   }


   // 10.06.2019.: op.a.                                                                       
   // tute mozda napraviti da kada ne najde Ugovor da proba najti zadnju URU pa da bar napravi 
   // NRD na 'dobrog' dobavljaca (a sa praznim UGO TtNum-om)                                   
   private int Create_NRD_Fakturs(Mixer mixer_rec)
   {
      if((TheVvUC as PredNrdDUC).PNA_Missing_UGO_TransList.NotEmpty())
      {
         foreach(Rtrans rtr in (TheVvUC as PredNrdDUC).PNA_Missing_UGO_TransList)
         {
            ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0}\n\nartikl {1} {2}\n\nNEMA UGOVORA!", rtr.T_serial, rtr.T_artiklCD, rtr.T_artiklName);
         }

         DialogResult result = MessageBox.Show("Neki artikli nemaju Ugovor pa se za njih neće napraviti narudžba!\n\nDa li želite kreirati djelomične narudžbe za preostale artikle?!",
            "Potvrdite izradu djelomičnih narudžbi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return 0;

      }

      #region Init, variablez, sifrars, ...

      if(mixer_rec.Transes.Count.IsZero()) return 0;

      TheVvUC.SetSifrarAndAutocomplete<Kupdob>(null, VvSQL.SorterType.Name);
      TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.Name);
      ZXC.luiListaSkladista.LazyLoad(); // za skladCD '1' 

      int    debugCount = 0;
      bool   OK;
      ushort line;

      Faktur NRDfaktur_rec;
      Rtrans narudzbaRtrans_rec;

      Xtrans firstIn_UGO_GR_xtrans_rec;

      Artikl narudzbaArtikl_rec;

      // KOLONE
      // 
      // T_artiklCD       "Šifra Proizvoda"
      // T_artiklName     "Naziv Proizvoda"
      // T_kol            "KolProiz"
      // T_vezniDokA_64   "Šifra Materijala"
      // T_opis_128       "Naziv Materijala"
      // T_kpdbZiroA_32   "RGC"
      // T_moneyA         "KolMat"
      // T_kpdbZiroB_32   "RNM"
      // 
      // 
      // ZAGLAVLJE
      // 
      // mixer_rec.KupdobCD    = Fld_KupDobCd        faktEx.KupdobCD
      // mixer_rec.KupdobTK    = Fld_KupDobTk        faktEx.KupdobName
      // mixer_rec.KupdobName  = Fld_KupDobName      faktEx.KupdobTK
      // mixer_rec.StrE_256    = Fld_Primjedba       fakturLocal_rec.Opis
      // mixer_rec.StrD_32     = Fld_VrstaRNM        vezniDok
      // 
      // mixer_rec.Konto       = Fld_IzlaznoSkl 
      // mixer_rec.Konto2      = Fld_UlaznoSkl  
      // mixer_rec.StrF_64     = Fld_Standard   
      // mixer_rec.DateA       = Fld_DateTrazRok;
      // mixer_rec.DateB       = Fld_DateMogRok ;

      VvLookUpItem pdvLui;
      decimal pdvSt;

      #endregion Init, variablez, sifrars, ...

      MixerDao.Set_UGO_TtNum_4XtransGroupping(TheDbConnection, mixer_rec.Transes);

    //var ugoGRps = mixer_rec.Transes.GroupBy(xtr => xtr.T_personCD   ); // U T_personCD    je TtNum UGOVORA (UGO)
      var ugoGRps = mixer_rec.Transes.GroupBy(xtr => xtr.R_externTtNum); // U R_externTtNum je TtNum UGOVORA (UGO)

      foreach(var ugoGR in ugoGRps)
      {
         firstIn_UGO_GR_xtrans_rec = ugoGR.FirstOrDefault();
         narudzbaArtikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(firstIn_UGO_GR_xtrans_rec.T_artiklCD);

         #region Set faktur_rec data

         line = 0;

         NRDfaktur_rec = new Faktur();
         
         #region Kupdob Data

       //Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(/*mixer_rec.KupdobCD*/firstIn_UGO_GR_xtrans_rec.T_kupdobCD );
         Kupdob kupdob_rec = TheVvUC.Get_Kupdob_FromVvUcSifrar(/*mixer_rec.KupdobCD*/firstIn_UGO_GR_xtrans_rec.R_externKCD);

         if(kupdob_rec != null)
         {
            NRDfaktur_rec.KupdobCD     = kupdob_rec.KupdobCD;
            NRDfaktur_rec.KupdobTK     = kupdob_rec.Ticker  ;
            NRDfaktur_rec.KupdobName   = kupdob_rec.Naziv   ;
            NRDfaktur_rec.KdOib        = kupdob_rec.Oib;
            NRDfaktur_rec.VatCntryCode = kupdob_rec.VatCntryCode;
            NRDfaktur_rec.KdUlica      = kupdob_rec.Ulica2;
            NRDfaktur_rec.KdZip        = kupdob_rec.PostaBr;
            NRDfaktur_rec.KdMjesto     = kupdob_rec.Grad;
            NRDfaktur_rec.DevName      = kupdob_rec.DevName;

          //faktur_rec.PdvKnjiga    = ZXC.PdvKnjigaEnum.REDOVNA;
          //
          //if(faktur_rec.TT == Faktur.TT_WRN)
          //   faktur_rec.ZiroRn = ZXC.GetIBANfromOldZiro(kupdob_rec.Ziro1);
          //
          //if(kupdob_rec.PdvRTip == ZXC.PdvRTipEnum.OBRT_R2 && faktur_rec.TT == Faktur.TT_WRN)
          //{
          //   faktur_rec.PdvR12 = ZXC.PdvR12Enum.R2;
          //}
          //else
          //{
          //   faktur_rec.PdvR12 = ZXC.PdvR12Enum.R1;
          //}

            #region PoslJed

            NRDfaktur_rec.PosJedCD     = kupdob_rec.KupdobCD;
            NRDfaktur_rec.PosJedTK     = kupdob_rec.Ticker  ;
            NRDfaktur_rec.PosJedName   = kupdob_rec.Naziv   ;
            NRDfaktur_rec.PosJedUlica  = kupdob_rec.Ulica2  ;
            NRDfaktur_rec.PosJedZip    = kupdob_rec.PostaBr ;
            NRDfaktur_rec.PosJedMjesto = kupdob_rec.Grad    ;

            #endregion PoslJed

            #region Putnik 06.11.2013.

            if(kupdob_rec.PutnikID.NotZero()) NRDfaktur_rec.PersonCD   = kupdob_rec.PutnikID;
            if(kupdob_rec.PutName.NotEmpty()) NRDfaktur_rec.PersonName = kupdob_rec.PutName ;

            #endregion Putnik

            #region EU VAT Code Action

          //if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsUlazniPdvTT)
          //{
          //   faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .NIJEDNA;
          //   faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
          //   faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
          //}
          //if(ZXC.EU_VatCodes_woHR.Contains(kupdob_rec.VatCntryCode) && faktur_rec.TtInfo.IsIzlazniPdvTT)
          //{
          //   faktur_rec.PdvKnjiga  = ZXC.PdvKnjigaEnum .REDOVNA;
          //   faktur_rec.PdvGEOkind = ZXC.PdvGEOkindEnum.EU;
          //   faktur_rec.PdvR12     = ZXC.PdvR12Enum    .R1;
          //}

            #endregion EU VAT Code Action

         }

         else
         { 
            if(ugoGR.First().T_kupdobCD.IsZero()) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Nema Ugovora!\n\nartikl [{0}]", firstIn_UGO_GR_xtrans_rec.T_artiklName);
            else ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Nema partnera sa šifrom [{0}]\n\nna koga glasi Ugovor.", ugoGR.First().T_kupdobCD);

            continue;
         }

         #endregion Kupdob Data

         NRDfaktur_rec.TT         = Faktur.TT_NRD                   ;
         NRDfaktur_rec.TtSort     = ZXC.TtInfo(NRDfaktur_rec.TT).TtSort;
         NRDfaktur_rec.SkladDate  =
         NRDfaktur_rec.DokDate    = mixer_rec.DokDate               ;
         NRDfaktur_rec.SkladCD    = mixer_rec.Konto                 ;
       //faktur_rec.SkladCD2      = mixer_rec.Konto2                ;
         NRDfaktur_rec.DokNum     = NRDfaktur_rec.VvDao.GetNextDokNum(TheDbConnection, Faktur.recordName);
         NRDfaktur_rec.TtNum      = NRDfaktur_rec.VvDao.GetNextTtNum(TheDbConnection, NRDfaktur_rec.VirtualTT, NRDfaktur_rec.SkladCD);
         NRDfaktur_rec.ProjektCD  = Faktur.TT_UGO + "-" + ugoGR.Key ;
         NRDfaktur_rec.Napomena   = mixer_rec.Napomena              ;
       //faktur_rec.VezniDok      = mixer_rec.StrD_32               ;
       //faktur_rec.Opis          = mixer_rec.StrE_256              ;
       //faktur_rec.VezniDok2     = mixer_rec.StrF_64               ;
       //faktur_rec.DospDate      = mixer_rec.DateA                 ;
         NRDfaktur_rec.V1_tt      = mixer_rec.TT                    ;
         NRDfaktur_rec.V1_ttNum   = mixer_rec.TtNum                 ;
         NRDfaktur_rec.V2_tt      = mixer_rec.V1_tt                 ;
         NRDfaktur_rec.V2_ttNum   = mixer_rec.V1_ttNum              ;
         NRDfaktur_rec.V3_tt      = mixer_rec.V2_tt                 ;
         NRDfaktur_rec.V3_ttNum   = mixer_rec.V2_ttNum              ;
       //faktur_rec.OpciAvalue    = mixer_rec.TT_And_TtNum          ;

         #endregion Set faktur_rec Data

         #region Set rtrans_rec Data - ZELENI redci (foreach proizvod)

         foreach(Xtrans xtrans_rec in ugoGR)
         {
            narudzbaRtrans_rec = new Rtrans();

            #region Check data and skip if bad

            narudzbaArtikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(xtrans_rec.T_artiklCD);

            if(narudzbaArtikl_rec == null)
            {
               ZXC.aim_emsg(MessageBoxIcon.Warning, "Redak {0}\n\nNema artikla u sifrarniku artikala!", xtrans_rec.T_serial);
               continue;
            }

            #endregion Check data and skip if bad

            #region PdvSt

            if(narudzbaArtikl_rec.PdvKat.NotEmpty())
            {
               pdvLui = ZXC.luiListaPdvKat.GetLuiForThisCd(narudzbaArtikl_rec.PdvKat);
               pdvSt = pdvLui.Number;
            }
            else
            {
               pdvSt = Faktur.CommonPdvStForThisDate(NRDfaktur_rec.DokDate);
            }

            #endregion PdvSt

            narudzbaRtrans_rec.T_artiklCD   = narudzbaArtikl_rec.ArtiklCD  ;
            narudzbaRtrans_rec.T_artiklName = narudzbaArtikl_rec.ArtiklName;
            narudzbaRtrans_rec.T_jedMj      = narudzbaArtikl_rec.JedMj     ;
            narudzbaRtrans_rec.T_doCijMal   = xtrans_rec.T_moneyB          ;
            narudzbaRtrans_rec.T_kol        = xtrans_rec.T_kol             ;
          //narudzbaRtrans_rec.T_kol2       = 0M                           ;
          //narudzbaRtrans_rec.T_cij        = xtrans_rec.T_moneyA          ;
            narudzbaRtrans_rec.T_cij        = xtrans_rec.R_externCij       ;
          //narudzbaRtrans_rec.T_pdvColTip  = dlgPdvKolTip                 ;
          //narudzbaRtrans_rec.T_pdvSt      = xtrans_rec.T_dec02           ;
            narudzbaRtrans_rec.T_pdvSt      = pdvSt                        ;
            narudzbaRtrans_rec.T_serial     = ++line                       ;                      
            narudzbaRtrans_rec.T_TT         = NRDfaktur_rec.TT             ;
            narudzbaRtrans_rec.T_ttNum      = NRDfaktur_rec.TtNum          ;
            narudzbaRtrans_rec.T_ttSort     = NRDfaktur_rec.TtSort         ;
            narudzbaRtrans_rec.T_skladCD    = NRDfaktur_rec.SkladCD        ;
          //narudzbaRtrans_rec.T_parentID   = NRDfaktur_rec.RecID; ... ovoga automatski postavi VvDaoBase.AddrecDocument_AddrecTranses() 
            narudzbaRtrans_rec.T_dokNum     = NRDfaktur_rec.DokNum         ;
            narudzbaRtrans_rec.T_skladDate  = NRDfaktur_rec.DokDate        ;
            narudzbaRtrans_rec.T_kupdobCD   = NRDfaktur_rec.KupdobCD       ;

            narudzbaRtrans_rec.CalcTransResults(null) ;

            NRDfaktur_rec.Transes.Add(narudzbaRtrans_rec);
         }

         #endregion Set rtrans_rec Data - ZELENI redci


         #region TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

         NRDfaktur_rec.TakeTransesSumToDokumentSum(true);

         // NOTA BENE: ovdje ide pravi ADDREC (fak + Transes) a ne AutoSetFaktur koji ne zna sume prije nego sto doda sve transove 
         OK = NRDfaktur_rec.VvDao.ADDREC(TheDbConnection, NRDfaktur_rec);

         if(OK) ++debugCount;

         #endregion TakeTransesSumToDokumentSum And finally ADDREC(faktur_rec)

         #region Rwtrec Feedback To Xtrans

         foreach(Xtrans xtrans_rec in ugoGR)
         {
            BeginEdit(xtrans_rec);

            xtrans_rec.T_kpdbZiroB_32 = NRDfaktur_rec.TT_And_TtNum;

            xtrans_rec.VvDao.RWTREC(TheDbConnection, xtrans_rec, false, true, false);

            EndEdit(xtrans_rec);
         }

         #endregion Rwtrec Feedback
      }

      return debugCount;
   }


   #endregion MIX_Create_RISK_SVD_NRD


   #region MIXER_CreateAVR_fromRNZ

   private void MIXER_CreateAVR_fromRNZ(object sender, EventArgs e)
   {
      #region Init

      Cursor.Current = Cursors.WaitCursor;

      int mixerCount = 0;

      DateTime serverDateNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      VvAVRfromRNZDlg dlg = new VvAVRfromRNZDlg("AVR iz RNZ");

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string zaMMYYYY = dlg.Fld_ZaMjesec;

      dlg.Dispose();

      if(zaMMYYYY.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nije zadan mjesec."); return; }

      Mixer  mixer_rec        ;
      Xtrans xtrans_rec = null;
      Kupdob kupdob_rec;

      ushort line = 0;

      AvrDUC theDUC = TheVvUC as AvrDUC;

      #endregion Init

      #region Get rnzFakturList

      DateTime dateOD = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, false)           ;
      DateTime dateDO = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, true ).EndOfDay();

      List<Faktur> rnzFakturList = new List<Faktur>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.tt     ], "theTT" , Faktur.TT_RNZ, " = " ));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateOD", dateOD       , " >= "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.FakturSchemaRows[ZXC.FakCI.dokDate], "dateDO", dateDO       , " <= "));

      VvDaoBase.LoadGenericVvDataRecordList<Faktur>(TheDbConnection, rnzFakturList, filterMembers, "", "dokDate, ttSort, ttNum", true);

      //rnzFakturList.ForEach(fak => fak.VvDao.LoadTranses(TheDbConnection, fak, false));

      #endregion Get rnzFakturList

      #region Bussiness Logic

      if(rnzFakturList.Count.NotZero())
      {
         var rnzFakturListGrouppedByPJ = rnzFakturList.GroupBy(fak => fak.PosJedCD);

         foreach(var rnzFak_PJ_GR in rnzFakturListGrouppedByPJ)
         {
            #region Set AVR Mixer Data

            line = 0;

            mixer_rec = new Mixer();

            mixer_rec.TT          = Mixer.TT_AVR;
            mixer_rec.DokDate     = VvSQL.GetServer_DateTime_Now(TheDbConnection);
          //mixer_rec.Napomena    = Limited_Mixer(FullPathFileName, ZXC.MixCI.napomena);

            mixer_rec.V2_ttNum   = rnzFak_PJ_GR.Key                 ;
            mixer_rec.StrF_64    = rnzFak_PJ_GR.First().PosJedName  ;
            mixer_rec.StrC_32    = rnzFak_PJ_GR.First().PosJedTK    ;
            mixer_rec.StrB_128   = rnzFak_PJ_GR.First().PosJedAdresa;

            mixer_rec.KupdobCD   = rnzFak_PJ_GR.First().KupdobCD  ;
            mixer_rec.KupdobName = rnzFak_PJ_GR.First().KupdobName;
            mixer_rec.KupdobTK   = rnzFak_PJ_GR.First().KupdobTK  ;

            mixer_rec.StrD_32    = zaMMYYYY;

            #endregion AVR Mixer

            var rnzFak_PJ_GR_GrouppedByPersonCD = rnzFak_PJ_GR.GroupBy(fak => fak.PersonCD);

            kupdob_rec = theDUC.Get_Kupdob_FromVvUcSifrar(rnzFak_PJ_GR.Key);

            foreach(var rnzFak_PJ_And_PesonCD_GR in rnzFak_PJ_GR_GrouppedByPersonCD)
            {
               #region Set AVR Xtranses Data

               xtrans_rec = new Xtrans();

               xtrans_rec.T_personCD                   = rnzFak_PJ_And_PesonCD_GR.Key               ;
               xtrans_rec.T_kpdbNameA_50 /* prezime */ = rnzFak_PJ_And_PesonCD_GR.First().OpciBvalue;
               xtrans_rec.T_kpdbNameB_50 /* ime     */ = rnzFak_PJ_And_PesonCD_GR.First().OpciBlabel;

               SetAVR_Xtrans_ColValues_From_RnzFak_PJ_And_PesonCD_GR(xtrans_rec, rnzFak_PJ_And_PesonCD_GR, zaMMYYYY, kupdob_rec);

               #endregion AVR Xtranses

               MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);

            } // foreach(var fak_PJ_And_PesonCD_GR in fak_PJ_GR_GrouppedByPersonCD) 

            mixerCount++;
         }
      }

      #endregion Bussiness Logic

      #region Finalize

      Cursor.Current = Cursors.Default;

      ZXC.ClearStatusText();

      TheVvRecordUC.ThePrefferedRecordSorter = Mixer.sorterDokDate;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao\n\n{0} AVRa.", mixerCount);

      #endregion Finalize
   }

   private void SetAVR_Xtrans_ColValues_From_RnzFak_PJ_And_PesonCD_GR(Xtrans xtrans_rec, IGrouping<uint, Faktur> rnzFak_PJ_And_PesonCD_GR, string zaMMYYYY, Kupdob kupdob_rec)
   {
      System.Reflection.PropertyInfo pInfo_uDanu_OD;
      System.Reflection.PropertyInfo pInfo_uDanu_DO;

      Type xtType = typeof(Xtrans);

      DateTime theDate       ;
      string   dd            ;
      DayOfWeek ddIsDOW      ;
      TimeSpan kupdobHHMM_OD ;
      TimeSpan kupdobHHMM_DO ;
      TimeSpan kupdobHHMMFond;
      TimeSpan maxHHMMFond = new TimeSpan(12, 00, 00); // 12 satno radno vrijeme ... tu jos treba razmisliti. 
      bool theDateExistsOnRnz;
      bool DOW_isOK          ;
      bool isBlagdan         ;

      for(int day = 1; day <= 31; ++day)
      {
         dd = day.ToString("00");
         theDate = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, false, dd);
         ddIsDOW = theDate.DayOfWeek;

       //xtrans_rec.T_dec01   = blabla;
       //xtrans_rec.T_dec01_2 = blabla;

         pInfo_uDanu_OD = xtType.GetProperty("T_dec" + dd       );
         pInfo_uDanu_DO = xtType.GetProperty("T_dec" + dd + "_2");

         kupdobHHMM_OD = kupdob_rec.HHMM_OD((int)ddIsDOW);
         kupdobHHMM_DO = kupdob_rec.HHMM_DO((int)ddIsDOW);

         kupdobHHMMFond = kupdobHHMM_DO - kupdobHHMM_OD;

         theDateExistsOnRnz = rnzFak_PJ_And_PesonCD_GR.Any(rnz => theDate >= rnz.DokDate.Date && theDate <= rnz.DokDate2.Date);

         DOW_isOK = kupdobHHMM_OD != TimeSpan.Zero && kupdobHHMM_DO != TimeSpan.Zero;

         isBlagdan = ZXC.IsThisDanPraznik(theDate, theDate.Year);

         if(theDateExistsOnRnz && kupdobHHMMFond <= maxHHMMFond && DOW_isOK && isBlagdan == false)
         {
            pInfo_uDanu_OD.SetValue(xtrans_rec, ZXC.GetDecimalFromTimeSpan(kupdobHHMM_OD), null);
            pInfo_uDanu_DO.SetValue(xtrans_rec, ZXC.GetDecimalFromTimeSpan(kupdobHHMM_DO), null);
         }
      }
   }

   #endregion MIXER_CreateAVR_fromRNZ

   #region MIXER_CreateRVR_fromAVR

   private void MIXER_CreateRVR_fromAVR(object sender, EventArgs e)
   {
      // mixer (+xtrans) iz mixera (+xtrans)
      // 1. get AVRmixerList (AVR, zaMMYYYY) 2 filterMemberz-a ... 
      //    get xtransList foreach mixer_rec 
      //    create uniju svih xtransLista = MMYYYYxtransList
      //    create UtilDataPackegeList (foreach xtrans ... foreach DD of xtrans => t_personCD, t_dokDate, t_timeOD, t_timeDO, t_vrstaRada 
      // 2. var UtilDataPackegeListGroupedBy_dokDate
      // 3. foreach(var udpGR in UtilDataPackegeListGroupedBy_dokDate) { create new Xtrans_rec }

      #region Init

      Cursor.Current = Cursors.WaitCursor;

      int mixerCount = 0;

      DateTime serverDateNow = VvSQL.GetServer_DateTime_Now(TheDbConnection);

      VvAVRfromRNZDlg dlg = new VvAVRfromRNZDlg("RVR iz AVR");

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      string zaMMYYYY = dlg.Fld_ZaMjesec;

      dlg.Dispose();

      if(zaMMYYYY.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Nije zadan mjesec."); return; }

      Mixer  mixer_rec        ;
    //Xtrans xtrans_rec = null;
      Kupdob kupdob_rec;

      ushort line = 0;

      RvrDUC theDUC = TheVvUC as RvrDUC;

      #endregion Init

      #region Get avrMixerList w Transes

      DateTime rvrXtr_dateOD;
      DateTime rvrXtr_dateDO;

      List<Mixer> avrMixerList = new List<Mixer>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.tt     ], "theTT"   , Mixer.TT_AVR, " = "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.MixerSchemaRows[ZXC.MixCI.strD_32], "zaMMYYYY", zaMMYYYY    , " = "));

      VvDaoBase.LoadGenericVvDataRecordList<Mixer>(TheDbConnection, avrMixerList, filterMembers, "", "dokDate, ttNum", false);

      avrMixerList.ForEach(mix => mix.VvDao.LoadTranses(TheDbConnection, mix, false));

      #endregion Get avrMixerList

      #region Create RVR xtranses from AVR xtranses

      List<Xtrans> allAVRxtransList = new List<Xtrans>();
      List<Xtrans> allRVRxtransList = new List<Xtrans>();
      avrMixerList.ForEach(mix => allAVRxtransList.AddRange(mix.Transes));

      //Xtrans rvrXtrans_rec;

      foreach(Xtrans avrXtrans_rec in allAVRxtransList)
      {
         if((avrXtrans_rec.T_dec01.NotZero() && avrXtrans_rec.T_dec01_2.NotZero()) || avrXtrans_rec.T_str01.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "01", avrXtrans_rec.T_dec01, avrXtrans_rec.T_dec01_2, avrXtrans_rec.T_str01);
       //else if(avrXtrans_rec.T_str01.ToUpper().StartsWith("GO")) ;
         if((avrXtrans_rec.T_dec02.NotZero() && avrXtrans_rec.T_dec02_2.NotZero()) || avrXtrans_rec.T_str02.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "02", avrXtrans_rec.T_dec02, avrXtrans_rec.T_dec02_2, avrXtrans_rec.T_str02);
         if((avrXtrans_rec.T_dec03.NotZero() && avrXtrans_rec.T_dec03_2.NotZero()) || avrXtrans_rec.T_str03.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "03", avrXtrans_rec.T_dec03, avrXtrans_rec.T_dec03_2, avrXtrans_rec.T_str03);
         if((avrXtrans_rec.T_dec04.NotZero() && avrXtrans_rec.T_dec04_2.NotZero()) || avrXtrans_rec.T_str04.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "04", avrXtrans_rec.T_dec04, avrXtrans_rec.T_dec04_2, avrXtrans_rec.T_str04);
         if((avrXtrans_rec.T_dec05.NotZero() && avrXtrans_rec.T_dec05_2.NotZero()) || avrXtrans_rec.T_str05.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "05", avrXtrans_rec.T_dec05, avrXtrans_rec.T_dec05_2, avrXtrans_rec.T_str05);
         if((avrXtrans_rec.T_dec06.NotZero() && avrXtrans_rec.T_dec06_2.NotZero()) || avrXtrans_rec.T_str06.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "06", avrXtrans_rec.T_dec06, avrXtrans_rec.T_dec06_2, avrXtrans_rec.T_str06);
         if((avrXtrans_rec.T_dec07.NotZero() && avrXtrans_rec.T_dec07_2.NotZero()) || avrXtrans_rec.T_str07.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "07", avrXtrans_rec.T_dec07, avrXtrans_rec.T_dec07_2, avrXtrans_rec.T_str07);
         if((avrXtrans_rec.T_dec08.NotZero() && avrXtrans_rec.T_dec08_2.NotZero()) || avrXtrans_rec.T_str08.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "08", avrXtrans_rec.T_dec08, avrXtrans_rec.T_dec08_2, avrXtrans_rec.T_str08);
         if((avrXtrans_rec.T_dec09.NotZero() && avrXtrans_rec.T_dec09_2.NotZero()) || avrXtrans_rec.T_str09.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "09", avrXtrans_rec.T_dec09, avrXtrans_rec.T_dec09_2, avrXtrans_rec.T_str09);
         if((avrXtrans_rec.T_dec10.NotZero() && avrXtrans_rec.T_dec10_2.NotZero()) || avrXtrans_rec.T_str10.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "10", avrXtrans_rec.T_dec10, avrXtrans_rec.T_dec10_2, avrXtrans_rec.T_str10);
         if((avrXtrans_rec.T_dec11.NotZero() && avrXtrans_rec.T_dec11_2.NotZero()) || avrXtrans_rec.T_str11.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "11", avrXtrans_rec.T_dec11, avrXtrans_rec.T_dec11_2, avrXtrans_rec.T_str11);
         if((avrXtrans_rec.T_dec12.NotZero() && avrXtrans_rec.T_dec12_2.NotZero()) || avrXtrans_rec.T_str12.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "12", avrXtrans_rec.T_dec12, avrXtrans_rec.T_dec12_2, avrXtrans_rec.T_str12);
         if((avrXtrans_rec.T_dec13.NotZero() && avrXtrans_rec.T_dec13_2.NotZero()) || avrXtrans_rec.T_str13.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "13", avrXtrans_rec.T_dec13, avrXtrans_rec.T_dec13_2, avrXtrans_rec.T_str13);
         if((avrXtrans_rec.T_dec14.NotZero() && avrXtrans_rec.T_dec14_2.NotZero()) || avrXtrans_rec.T_str14.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "14", avrXtrans_rec.T_dec14, avrXtrans_rec.T_dec14_2, avrXtrans_rec.T_str14);
         if((avrXtrans_rec.T_dec15.NotZero() && avrXtrans_rec.T_dec15_2.NotZero()) || avrXtrans_rec.T_str15.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "15", avrXtrans_rec.T_dec15, avrXtrans_rec.T_dec15_2, avrXtrans_rec.T_str15);
         if((avrXtrans_rec.T_dec16.NotZero() && avrXtrans_rec.T_dec16_2.NotZero()) || avrXtrans_rec.T_str16.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "16", avrXtrans_rec.T_dec16, avrXtrans_rec.T_dec16_2, avrXtrans_rec.T_str16);
         if((avrXtrans_rec.T_dec17.NotZero() && avrXtrans_rec.T_dec17_2.NotZero()) || avrXtrans_rec.T_str17.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "17", avrXtrans_rec.T_dec17, avrXtrans_rec.T_dec17_2, avrXtrans_rec.T_str17);
         if((avrXtrans_rec.T_dec18.NotZero() && avrXtrans_rec.T_dec18_2.NotZero()) || avrXtrans_rec.T_str18.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "18", avrXtrans_rec.T_dec18, avrXtrans_rec.T_dec18_2, avrXtrans_rec.T_str18);
         if((avrXtrans_rec.T_dec19.NotZero() && avrXtrans_rec.T_dec19_2.NotZero()) || avrXtrans_rec.T_str19.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "19", avrXtrans_rec.T_dec19, avrXtrans_rec.T_dec19_2, avrXtrans_rec.T_str19);
         if((avrXtrans_rec.T_dec20.NotZero() && avrXtrans_rec.T_dec20_2.NotZero()) || avrXtrans_rec.T_str20.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "20", avrXtrans_rec.T_dec20, avrXtrans_rec.T_dec20_2, avrXtrans_rec.T_str20);
         if((avrXtrans_rec.T_dec21.NotZero() && avrXtrans_rec.T_dec21_2.NotZero()) || avrXtrans_rec.T_str21.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "21", avrXtrans_rec.T_dec21, avrXtrans_rec.T_dec21_2, avrXtrans_rec.T_str21);
         if((avrXtrans_rec.T_dec22.NotZero() && avrXtrans_rec.T_dec22_2.NotZero()) || avrXtrans_rec.T_str22.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "22", avrXtrans_rec.T_dec22, avrXtrans_rec.T_dec22_2, avrXtrans_rec.T_str22);
         if((avrXtrans_rec.T_dec23.NotZero() && avrXtrans_rec.T_dec23_2.NotZero()) || avrXtrans_rec.T_str23.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "23", avrXtrans_rec.T_dec23, avrXtrans_rec.T_dec23_2, avrXtrans_rec.T_str23);
         if((avrXtrans_rec.T_dec24.NotZero() && avrXtrans_rec.T_dec24_2.NotZero()) || avrXtrans_rec.T_str24.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "24", avrXtrans_rec.T_dec24, avrXtrans_rec.T_dec24_2, avrXtrans_rec.T_str24);
         if((avrXtrans_rec.T_dec25.NotZero() && avrXtrans_rec.T_dec25_2.NotZero()) || avrXtrans_rec.T_str25.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "25", avrXtrans_rec.T_dec25, avrXtrans_rec.T_dec25_2, avrXtrans_rec.T_str25);
         if((avrXtrans_rec.T_dec26.NotZero() && avrXtrans_rec.T_dec26_2.NotZero()) || avrXtrans_rec.T_str26.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "26", avrXtrans_rec.T_dec26, avrXtrans_rec.T_dec26_2, avrXtrans_rec.T_str26);
         if((avrXtrans_rec.T_dec27.NotZero() && avrXtrans_rec.T_dec27_2.NotZero()) || avrXtrans_rec.T_str27.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "27", avrXtrans_rec.T_dec27, avrXtrans_rec.T_dec27_2, avrXtrans_rec.T_str27);
         if((avrXtrans_rec.T_dec28.NotZero() && avrXtrans_rec.T_dec28_2.NotZero()) || avrXtrans_rec.T_str28.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "28", avrXtrans_rec.T_dec28, avrXtrans_rec.T_dec28_2, avrXtrans_rec.T_str28);
         if((avrXtrans_rec.T_dec29.NotZero() && avrXtrans_rec.T_dec29_2.NotZero()) || avrXtrans_rec.T_str29.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "29", avrXtrans_rec.T_dec29, avrXtrans_rec.T_dec29_2, avrXtrans_rec.T_str29);
         if((avrXtrans_rec.T_dec30.NotZero() && avrXtrans_rec.T_dec30_2.NotZero()) || avrXtrans_rec.T_str30.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "30", avrXtrans_rec.T_dec30, avrXtrans_rec.T_dec30_2, avrXtrans_rec.T_str30);
         if((avrXtrans_rec.T_dec31.NotZero() && avrXtrans_rec.T_dec31_2.NotZero()) || avrXtrans_rec.T_str31.NotEmpty()) CreateRVRxtrans_fromAVRxtrans(allRVRxtransList, zaMMYYYY, avrXtrans_rec, "31", avrXtrans_rec.T_dec31, avrXtrans_rec.T_dec31_2, avrXtrans_rec.T_str31);
      }

      allRVRxtransList = allRVRxtransList.OrderBy(avr => avr.T_dokDate).ToList();

      #endregion Create RVR xtranses from AVR xtranses

      #region Bussiness Logic

      if(allRVRxtransList.Count.NotZero())
      {
         var rvrXtransListGrouppedByDate = allRVRxtransList.GroupBy(x => x.T_dokDate);

         string avrNapomena;

         foreach(var rvrXtr_dokDate_GR in rvrXtransListGrouppedByDate)
         {
            #region Set AVR Mixer Data
         
            line = 0;
         
            mixer_rec         = new Mixer()          ;
            mixer_rec.TT      = Mixer.TT_RVR         ;
            mixer_rec.DokDate = rvrXtr_dokDate_GR.Key;

            avrNapomena = "AVR: ";
            var avrTtNums = rvrXtr_dokDate_GR.Select(avr => avr.T_ttNum).Distinct().ToList();
            avrTtNums.ForEach(avr => avrNapomena += avr + ", "); avrNapomena = avrNapomena.TrimEnd(' ').TrimEnd(',') + ".";
            mixer_rec.Napomena    = ZXC.LenLimitedStr(avrNapomena, ZXC.MixerDao.GetSchemaColumnSize(ZXC.MixCI.napomena));
         
            mixer_rec.StrD_32    = zaMMYYYY; // ostavim ovo? mada se ne vidi na DUC-u 
         
            #endregion AVR Mixer

            foreach(Xtrans xtrans_rec in rvrXtr_dokDate_GR)
            {
               #region Set AVR Xtranses Data
            
               //xtrans_rec = new Xtrans();
               //
               //xtrans_rec.T_personCD                   = rnzFak_PJ_And_PesonCD_GR.Key               ;
               //xtrans_rec.T_kpdbNameA_50 /* prezime */ = rnzFak_PJ_And_PesonCD_GR.First().OpciBlabel;
               //xtrans_rec.T_kpdbNameB_50 /* ime     */ = rnzFak_PJ_And_PesonCD_GR.First().OpciBvalue;
               //
               //SetAVR_Xtrans_ColValues_From_RnzFak_PJ_And_PesonCD_GR(xtrans_rec, rnzFak_PJ_And_PesonCD_GR, zaMMYYYY, kupdob_rec);
            
               #endregion AVR Xtranses
            
               MixerDao.AutoSetMixer(TheDbConnection, ref line, mixer_rec, xtrans_rec);
            
            } // foreach(var fak_PJ_And_PesonCD_GR in fak_PJ_GR_GrouppedByPersonCD) 
         
            mixerCount++;
         } // foreach(var rvrXtr_dokDate_GR in rvrXtransListGrouppedByDate) 

      } // if(avrMixerList.Count.NotZero())

      #endregion Bussiness Logic

      #region Finalize

      Cursor.Current = Cursors.Default;

      ZXC.ClearStatusText();

      TheVvRecordUC.ThePrefferedRecordSorter = Mixer.sorterDokDate;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao\n\n{0} RVRa.", mixerCount);

      #endregion Finalize

   }

   private enum Rvr_hh_Kind { thisDayNormal, thisDayNocni, nextDayNormal, nextDayNocni }; 
   private int  hhNocStart = 22; 
   private int  hhNocEnd   = 06;
   private bool IsHHnocniToday  (int hh)                         { return hh >= hhNocStart; }
   private bool IsHHnocniNextDay(int hh)                         { return hh <  hhNocEnd  ; }
   private bool IsHHNextDay     (int firstHH, int hh)            { return hh < firstHH    ; }
   private bool IsTimeDoNextDay (decimal timeOD, decimal timeDO) { return timeDO < timeOD ; }

   private void CreateRVRxtrans_fromAVRxtrans(List<Xtrans> allRVRxtransList, string zaMMYYYY, Xtrans avrXtrans_rec, string dd, decimal timeOD, decimal timeDO, string avrVR)
   {
      DateTime dokDateThisDay = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, false, dd);
      DateTime dokDateNextDay = dokDateThisDay + ZXC.OneDaySpan;

      DateTime dateOD = ZXC.GetDateTimeFromDecimal_fullDateTime(dokDateThisDay, timeOD);
      DateTime dateDO = ZXC.GetDateTimeFromDecimal_fullDateTime(IsTimeDoNextDay(timeOD, timeDO) ? dokDateNextDay : dokDateThisDay, timeDO); 

      Dictionary<int, Rvr_hh_Kind> hhKind = new Dictionary<int, Rvr_hh_Kind>();

      if(timeOD.IsZero() && timeDO.IsZero()) // vremena su prazna a VR je zadano; (GO, S, ...) 
      {
         hhKind[0] = Rvr_hh_Kind.thisDayNormal;
      }

      //for(int hh = dateOD.Hour; hh < dateDO.Hour; ++hh)
      for(DateTime currDateTime = dateOD; currDateTime < dateDO; currDateTime += ZXC.OneHourSpan)
      {
         if(currDateTime.Day != dateOD.Day) // nextDay 
         {
            if(IsHHnocniNextDay(currDateTime.Hour)) hhKind[currDateTime.Hour] = Rvr_hh_Kind.nextDayNocni ;
            else                                    hhKind[currDateTime.Hour] = Rvr_hh_Kind.nextDayNormal;
         }
         else // thisDay 
         {
            if(IsHHnocniToday(currDateTime.Hour)) hhKind[currDateTime.Hour] = Rvr_hh_Kind.thisDayNocni ;
            else                                  hhKind[currDateTime.Hour] = Rvr_hh_Kind.thisDayNormal;
         }
      }

      Rvr_hh_Kind currHhKind = hhKind[dateOD.Hour];
      int         currHHod   = dateOD.Hour        ;
      int         currMMod   = dateOD.Minute      ;
      bool        isNextDay                       ;
      Xtrans      rvrXtrans_rec                   ;

    //for(int hh = dateOD.Hour; hh < dateDO.Hour; ++hh)
      for(DateTime currDateTime = dateOD; currDateTime < dateDO; currDateTime += ZXC.OneHourSpan)
      {
         if(currHhKind != hhKind[currDateTime.Hour]) // new hhKind action 
         {
            isNextDay = currHhKind == Rvr_hh_Kind.nextDayNormal || currHhKind == Rvr_hh_Kind.nextDayNocni;
            rvrXtrans_rec = CreateRVRxtransForRVRkind(isNextDay ? dokDateNextDay : dokDateThisDay, avrXtrans_rec, currHHod, currMMod, currDateTime.Hour, currDateTime.Minute, currHhKind, avrVR);
            if(rvrXtrans_rec != null) allRVRxtransList.Add(rvrXtrans_rec);
            currHhKind = hhKind[currDateTime.Hour];
            currHHod   = currDateTime.Hour  ;
            currMMod   = currDateTime.Minute;
         }
      } // for(int hh = dateOD.Hour; hh < dateDO.Hour; ++hh)

      // Za zadnjega 
      isNextDay = currHhKind == Rvr_hh_Kind.nextDayNormal || currHhKind == Rvr_hh_Kind.nextDayNocni;
      rvrXtrans_rec = CreateRVRxtransForRVRkind(isNextDay ? dokDateNextDay : dokDateThisDay, avrXtrans_rec, currHHod, currMMod, dateDO.Hour, dateDO.Minute, currHhKind, avrVR);
      if(rvrXtrans_rec != null) allRVRxtransList.Add(rvrXtrans_rec);
   }

 //private string GetNextDDforMonth(string dd, string zaMMYYYY)
 //{
 //   DateTime prevDate = Placa.GetDateTimeFromMMYYYY(zaMMYYYY, false, dd);
 //
 //   if(prevDate.Day == DateTime.DaysInMonth(prevDate.Year, prevDate.Month)) return "01";
 //   else                                                                    return (ZXC.ValOrZero_Int(dd) + 1).ToString();
 //}

   private static Xtrans CreateRVRxtransForRVRkind(DateTime dokDate, Xtrans avrXtrans_rec, int hhOD, int mmOD, int hhDO, int mmDO, Rvr_hh_Kind currHhKind, string avrVR)
   {
      if(avrVR.NotEmpty() && !avrVR.ToUpper().StartsWith("GO")) return null; // u RVR idemo samo ako je avrVR prazan (redovan rad) ili ako je 'GO' (godisnji odmor) 

      Xtrans rvrXtrans_rec = new Xtrans();

      rvrXtrans_rec.T_dokDate      = dokDate                               ;
      rvrXtrans_rec.T_dateOd       = new DateTime(dokDate.Year, dokDate.Month, dokDate.Day, hhOD, mmOD, 00);
      rvrXtrans_rec.T_dateDo       = new DateTime(dokDate.Year, dokDate.Month, dokDate.Day, hhDO, mmDO, 00);
      rvrXtrans_rec.T_kol          = RvrDUC.RVRelapsedHours(rvrXtrans_rec.T_dateOd, rvrXtrans_rec.T_dateDo);
      rvrXtrans_rec.T_TT           = avrXtrans_rec.T_TT                                                    ;
      rvrXtrans_rec.T_ttNum        = avrXtrans_rec.T_ttNum                                                 ;
      rvrXtrans_rec.T_personCD     = avrXtrans_rec.T_personCD                                              ;
      rvrXtrans_rec.T_kpdbNameA_50 = avrXtrans_rec.T_kpdbNameA_50                                          ;
      rvrXtrans_rec.T_kpdbNameB_50 = avrXtrans_rec.T_kpdbNameB_50                                          ;

      rvrXtrans_rec.T_strA_2       = GetRVR_VR(dokDate, currHhKind, avrVR)                                 ;

      return rvrXtrans_rec;
   }

   private static string GetRVR_VR(DateTime dokDate, Rvr_hh_Kind currHhKind, string avrVR)
   {
      bool isBlagdan = ZXC.IsThisDanPraznik(dokDate, dokDate.Year);
      bool isNedjelj = dokDate.DayOfWeek == DayOfWeek.Sunday      ;
      bool isNocni   = (currHhKind == Rvr_hh_Kind.nextDayNocni || currHhKind == Rvr_hh_Kind.thisDayNocni);
      bool isGodisnj = avrVR.ToUpper().StartsWith("GO");

      if(avrVR.IsEmpty() && !isBlagdan && !isNedjelj && !isNocni && !isGodisnj) return "";

      if(isBlagdan && isGodisnj) return "14";
      if(isBlagdan && isNocni  ) return "26";
      if(isNedjelj && isNocni  ) return "27";
      if(isNedjelj && isGodisnj) return ""  ;
      if(isNocni               ) return "25";
      if(isBlagdan             ) return "28";
      if(isNedjelj             ) return "30";
      if(isGodisnj             ) return "13";

      return "";
   }

   #endregion MIXER_CreateRVR_fromAVR

   #region BMW

   private void BMW_Variate_RealMPC(object sender, EventArgs e)
   { 
      BmwDUC theDUC = TheVvUC as BmwDUC                            ; 
      BMW    theBMW = theDUC.Create_BMW_FromMixer(theDUC.mixer_rec);
      theDUC.TheG.Rows.Clear()                                     ;

      List<decimal> oglasMPCList = new List<decimal> 
      { 
         38000, 
         39000, 
         40000, 
         41000, 
         42000, 
         43000, 
         44000, 
         45000, 
         46000, 
         47000, 
         48000, 
      };

      decimal origOglasMPC = theBMW.RealMPC_EUR;
      int highlightedRowIdx = 0, currRowIdx;

      foreach(decimal oglasMPC in oglasMPCList)
      {
         theBMW.RealMPC_EUR = oglasMPC;
         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(oglasMPC, origOglasMPC, 500), ref highlightedRowIdx);
      }

      theDUC.ColorOfColumns("RealMPC_EUR", highlightedRowIdx);
   }

   private void BMW_Variate_Model(object sender, EventArgs e)
   {
      BmwDUC theDUC = TheVvUC as BmwDUC                            ; 
      BMW    theBMW = theDUC.Create_BMW_FromMixer(theDUC.mixer_rec);
      theDUC.TheG.Rows.Clear()                                     ;

      decimal origHR_historyOsnovnaCij_Kn = theBMW.HR_historyOsnovnaCij_Kn;
      int highlightedRowIdx = 0, currRowIdx;

      theBMW.HR_historyOsnovnaCij_Kn = 392540; // 330xd 12. 2015. 142 co2
      theBMW.co2Emisija              =    142;
      currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(theBMW.HR_historyOsnovnaCij_Kn, origHR_historyOsnovnaCij_Kn, 20000), ref highlightedRowIdx); ;

      theBMW.HR_historyOsnovnaCij_Kn = 434138; // 335xd 12. 2015. 148 co2
      theBMW.co2Emisija              =    148;
      currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(theBMW.HR_historyOsnovnaCij_Kn, origHR_historyOsnovnaCij_Kn, 20000), ref highlightedRowIdx);

      theBMW.HR_historyOsnovnaCij_Kn = 466650; // 530xd 12. 2015. 149 co2
      theBMW.co2Emisija              =    149;
      currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(theBMW.HR_historyOsnovnaCij_Kn, origHR_historyOsnovnaCij_Kn, 20000), ref highlightedRowIdx);

      theBMW.HR_historyOsnovnaCij_Kn = 530240; // 535xd 12. 2015. 154 co2
      theBMW.co2Emisija              =    154;
      currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(theBMW.HR_historyOsnovnaCij_Kn, origHR_historyOsnovnaCij_Kn, 20000), ref highlightedRowIdx);

      theDUC.ColorOfColumns("HR_historyOsnovnaCij_Kn", highlightedRowIdx);

   }

   private void BMW_Variate_Carinik(object sender, EventArgs e)
   { 
      BmwDUC theDUC = TheVvUC as BmwDUC                            ; 
      BMW    theBMW = theDUC.Create_BMW_FromMixer(theDUC.mixer_rec);
      theDUC.TheG.Rows.Clear()                                     ;

      List<decimal> opremaPriceList = new List<decimal> { 0, 50000, 75000, 100000, 125000, 150000, 175000, 200000, 225000, 250000, 275000, 300000 };

      decimal origOpremaPriceList = theBMW.HR_historyOprermaCij_Kn;
      int highlightedRowIdx = 0, currRowIdx;

      foreach(decimal opremaPrice in opremaPriceList)
      {
         theBMW.HR_historyOprermaCij_Kn = opremaPrice;
         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, ZXC.AlmostEqual(opremaPrice, origOpremaPriceList, 12500), ref highlightedRowIdx);
      }

      theDUC.ColorOfColumns("HR_historyOprermaCij_Kn", highlightedRowIdx);

   }

   private void BMW_Variate_CO2(object sender, EventArgs e) 
   { 
      BmwDUC theDUC = TheVvUC as BmwDUC                            ; 
      BMW    theBMW = theDUC.Create_BMW_FromMixer(theDUC.mixer_rec);
      theDUC.TheG.Rows.Clear()                                     ;

      List<int> co2List = new List<int> { 140, 149, 154, 156, 159, 164 };

      int origCO2 = theBMW.co2Emisija;
      int highlightedRowIdx = 0, currRowIdx;

      foreach(int co2 in co2List)
      {
         theBMW.co2Emisija = co2;
         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, (co2 == origCO2), ref highlightedRowIdx);
      }

      theDUC.ColorOfColumns("co2Emisija", highlightedRowIdx);

   }

   private void BMW_Variate_MMYY(object sender, EventArgs e)
   {
      BmwDUC theDUC = TheVvUC as BmwDUC; 
      BMW    theBMW = theDUC.Create_BMW_FromMixer(theDUC.mixer_rec);
      theDUC.TheG.Rows.Clear();

      int yy;
      int origMM = theBMW.mm1Registracije;
      int origYY = theBMW.yy1Registracije;
      int highlightedRowIdx = 0, currRowIdx;

      yy = 2014;
      for(int mm = 1; mm <= 12; ++mm)
      {
         theBMW.mm1Registracije = mm;
         theBMW.yy1Registracije = yy;

         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, (mm == origMM && yy == origYY), ref highlightedRowIdx);
      }
      yy = 2015;
      for(int mm = 1; mm <= 12; ++mm)
      {
         theBMW.mm1Registracije = mm;
         theBMW.yy1Registracije = yy;

         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, (mm == origMM && yy == origYY), ref highlightedRowIdx);
      }
      yy = 2016;
      for(int mm = 1; mm <= 12; ++mm)
      {
         theBMW.mm1Registracije = mm;
         theBMW.yy1Registracije = yy;

         currRowIdx = theDUC.PutDgvLineFields_BMW(theBMW, (mm == origMM && yy == origYY), ref highlightedRowIdx);
      }

      theDUC.ColorOfColumns("monthAge", highlightedRowIdx);

   }

   #endregion BMW

   #endregion Mixer

   #region FISKALIZACIJA

   /*private*/public void RISK_Fiskalize_RACUN(object sender, EventArgs e)
   {
      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "U Projekt-u nije upaljena fiskalizacija!"); return; }

      Faktur faktur_rec = (Faktur)(TheVvDataRecord);

      if(faktur_rec.IsF1 == false) { ZXC.aim_emsg(MessageBoxIcon.Stop, "Račun nije F1! Ne može se slati na F1 fiskalizaciju."); return; }

      try
      {
         //RISK_Fiskalize_GetCISserviceStatus(sender, e); 
         //RISK_Fiskalize_SendECHO           (sender, e); 
         //RISK_Fiskalize_PoslovniProstor    (sender, e); 

         //============================================================================================================================================ 
         //============== VOILA! ====================================================================================================================== 
         //============================================================================================================================================ 

         bool isManualyInitiated = true;
         if(sender is bool) isManualyInitiated = (bool)sender; // znaci da smo tu dosli iz redovnog sejvanja. isManualyInitiated == FALSE! 

         BeginEdit(faktur_rec);

         bool OK = RISK_Fiskalize_RACUN_JOB(faktur_rec, isManualyInitiated);

         // 07.03.2018: 
#if DEBUG
         if(ZXC.IsTH_vvTQ_VvKristal) // da ne refiskalizira stalno kod isprobavanja 
         {
            faktur_rec.FiskJIR = faktur_rec.AddTS.ToString(ZXC.VvDateAndTimeFormat);
            OK = true;
         }
#endif

         // 10.04.2019: kada superuser na TH poslovnici popunjava 'rupu' u brojevima jer je                           
         // ZXC.VvXmlDRfilesAlertRaised == true                                                                       
         // a mi iz Vipera smo nedostupni ... tako da poslovnica moze nastaviti a mi bumo naknadno dojeb.             
         // treba dati umjetni - privremeni FiskJIR da niti naknadni ulazak obicnog user-a ne izazove refiskalizaciju 

         if(ZXC.IsTEXTHOshop && ZXC.VvXmlDR_LastDocumentMissing_AlertRaised && ZXC.CurrUserHasSuperPrivileges)
         {
            faktur_rec.FiskJIR = "PRIVREMENO! " + faktur_rec.AddTS.ToString(ZXC.VvDateAndTimeFormat);
            OK = true;
         }

         if(OK) OK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, isManualyInitiated == false/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

         EndEdit(faktur_rec);

         if(OK) WhenRecordInDBHasChangedAction(); // RRDREC 

         // 12.01.2017: 
         if(OK && ZXC.IsSkyEnvironment && isManualyInitiated == true) VvDaoBase.SendWriteOperationToSKY(ZXC.TheSecondDbConn_SameDB, TheVvDataRecord, VvSQL.DB_RW_ActionType.RWT, /*true*/ false);

         //============================================================================================================================================ 
         //============== VOILA! ====================================================================================================================== 
         //============================================================================================================================================ 

      }
      catch(System.Net.WebException webEx)
      {
         if(webEx.Message == "The operation has timed out")
         {
            ZXC.aim_emsg(MessageBoxIcon.Error,
               "NEUSPJEŠAN POKUŠAJ FISKALIZACIJE RAČUNA!\n\r\n\rPrekoračeno je zadano vrijeme čekanja na odgovor (timeout) od {0} sekundi.",
                  ZXC.RRD.Dsc_FISK_TimeOutSeconds);
         }
         else
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, webEx.Message);
         }
      }
      catch(Exception ex)
      {
         //ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);
      }
      // 09.05.2016: EndEdit se inace ne bi dizao pri pojavi Exceptiona (pokusaj ispravljanja db_rec is null BUG-a) 
      finally
      {
         EndEdit(faktur_rec);
      }
   }

   private bool RISK_Fiskalize_RACUN_JOB(Faktur faktur_rec, bool isManualyInitiated)  // VOILA! 
   {
      if(faktur_rec.IsF1 == false) return false;

      #region Should we fiskalize end how

      // 27.04.2017: !!! BIG NEWS !!! 
    //string FiskOibOper = ZXC.CURR_user_rec.Oib;
      string faktur_AddUID = faktur_rec.AddUID;

      if(faktur_AddUID == ZXC.vvDB_programSuperUserName) faktur_AddUID = ZXC.CURR_user_rec.UserName; // ako ima potrebe da IRM doda superuser (npr. rupa u sljednosti) tada ne salji superuser-ov oib nego CURR_user oib 

      string FiskOibOper   = GetFisk_Oib_Oper(faktur_AddUID); // adduid oib instead of CURR_user oib

      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "U Projekt-u nije upaljena fiskalizacija!");                return false; }
      if(faktur_rec.IsAlreadyFiskalized            ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Račun je već FISKALIZIRAN!");                              return false; }
      if(FiskOibOper.IsEmpty()                     ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Za User-a " + faktur_AddUID + " nije definiran OIB!");     return false; }
      if(ZXC.IsBadOib(FiskOibOper, false)          ) { ZXC.aim_emsg(MessageBoxIcon.Error, "User "      + faktur_AddUID + " ima netočan OIB!"   );     return false; }

      #region Is racun mine anyway? 

      // 29.11.2016: 
      if(ZXC.IsSkyEnvironment && faktur_rec.LanSrvID != ZXC.vvDB_ServerID) 
      {
         string errMsg = string.Format("Pokusavate fiskalizirati racun\n\n{0}\n\nkoji nije sa ovog servera!", faktur_rec.ToString());

         VvMailClient mailClient = new VvMailClient();
         mailClient.SendMail_SUPPORT(false, "Fiskal PROBLEM", errMsg, "Fiskal!");

         ZXC.aim_emsg(errMsg); 
         return false;
      }
      
      #endregion Is racun mine anyway?

      if(isManualyInitiated)
      {
         // PAZI !!! tu si 01.02.2024: pretumbao DialogResult RetryCancel, YesNo, OkCancel result-ove, pa si mozda nes zjebal 

         if(faktur_rec.FiskPrgBr.IsEmpty()) // iz nekog razloga automatska fiskalizacija nije obavljena. 
                                            // ParagonBroj je PRAZAN pa pretpostavljamo da zeli retry dok jos nije odstampao racun.
         {
            DialogResult result = ZXC.CURR_prjkt_rec.IsNoAutoFiskal ?

               MessageBox.Show(String.Format("Potvrdite ručnu FISKALIZACIJU?!"),
              "Potvrdite FISKALIZACIJU", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
               :
               MessageBox.Show(String.Format("Potvrdite ponovni pokušaj FISKALIZACIJE?!\n\nParagon broj je PRAZAN!\n\nDakle, ovaj račun još uvijek NIJE odštampan te kupcu NIJE uručen PARAGON RAČUN?!"), 
              "Potvrdite ponovni pokušaj FISKALIZACIJE?!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if(result != DialogResult.OK) return false;
         }
         else // ParagonBroj je ZADAN pa pretpostavljamo da je paragon izdan pa je ovo 'racun.NakDost = true' situacija.
         {
            DialogResult result =
               MessageBox.Show(
                  String.Format("Potvrdite NAKNADNO slanje računa na FISKALIZACIJU?!\n\nParagon broj je {0}.\n\nDakle, ovaj račun je izdan kupcu u PARAGON obliku?!", 
                  faktur_rec.FiskPrgBr), 
               "Potvrdite NAKNADNU FISKALIZACIJU računa?!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if(result != DialogResult.OK) return false;
         }
      }

      #endregion Should we fiskalize end how

      #region OIB, IsPDV, DokDate, OznakaSlijednosti

      RacunType racun = new RacunType() 
      { 
         Oib        = ZXC.CURR_prjkt_rec.Oib, 
         USustPdv   = ZXC.CURR_prjkt_rec.IS_IN_PDV,
         DatVrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatumVrijeme(faktur_rec.DokDate), 
         OznSlijed  = OznakaSlijednostiType.P // P - na nivou poslovnog prostora, N - na nivou naplatnog uređaja 
      };

      #endregion OIB, IsPDV, DokDate, OznakaSlijednosti

      #region BrojRacunaType = BrOznRac + OznPosPr + OznNapUr

      BrojRacunaType br = new BrojRacunaType()
      {
         // Well, OPP - oznaka poslovnog prostora je, dakle, 'SkladCd'! Mora se prijaviti SVAKO SKLADISTE (pa valjda, taman sa istom adresom) 

         BrOznRac = faktur_rec.TtNumRbr      .ToString(), 
         OznPosPr = faktur_rec.TtNumSkPp     .ToString(), // ( opp – oznaka poslovnog prostora ) 
         OznNapUr = Faktur    .TtNumFiskalONU             // ( onu – oznaka naplatnog uređaja )  
      };

      racun.BrRac = br;

      #endregion BrojRacunaType = BrOznRac + OznPosPr + OznNapUr

      #region IznosUkupno, NacinPlacanja, OibOperatera, ParagonBlokBroj, isNaknadnoDostavljen

      racun.IznosUkupno = /*JehbhemTiMaterXMLsku*/(faktur_rec.S_ukKCRP.ToStringVv_NoGroup_ForceDot_Fisk()); 

      racun.NacinPlac    = faktur_rec.FiskNacPlacEnum;
      racun.OibOper      = FiskOibOper/*ZXC.CURR_user_rec.Oib*/;
      racun.ParagonBrRac = faktur_rec.FiskPrgBr;
      racun.NakDost      = racun.ParagonBrRac.NotEmpty();

      if(faktur_rec.F2_R1kind == ZXC.F2_R1enum.B2B && racun.NacinPlac != NacinPlacanjaType.T) // Denisove gace u TH (Firma B2B + np cash / kartica) 
      {
         // 2026: !!!??? 
         racun.OibPrimateljaRacuna = faktur_rec.KdOib;
      }

      #endregion IznosUkupno, NacinPlacanja, OibOperatera, isNaknadnoDostavljen

      #region Zastitni Kod Racuna / Izdavatelja

      racun.ZastKod = 
         Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(
            //"FISKAL 1", 
            Raverus.FiskalizacijaDEV.PopratneFunkcije.Potpisivanje.DohvatiCertifikat_FromPrjkt(), // VOILA by Q 
            racun.Oib, 
            racun.DatVrijeme.Replace('T', ' '), 
            racun.BrRac.BrOznRac, 
            racun.BrRac.OznPosPr, 
            racun.BrRac.OznNapUr, 
            racun.IznosUkupno
         );

      #endregion Zastitni Kod Racuna / Izdavatelja

      #region Add PDV (and eventually some other tax - fuse)

      AddFiskalPdvEntry(25, faktur_rec.S_ukOsn25m, faktur_rec.S_ukPdv25m, racun.Pdv);
      AddFiskalPdvEntry(23, faktur_rec.S_ukOsn23m, faktur_rec.S_ukPdv23m, racun.Pdv);
      AddFiskalPdvEntry(22, faktur_rec.S_ukOsn22m, faktur_rec.S_ukPdv22m, racun.Pdv);
      AddFiskalPdvEntry(10, faktur_rec.S_ukOsn10m, faktur_rec.S_ukPdv10m, racun.Pdv);

      // 25.04.2024: TEK!!! HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, HA, ... 
      AddFiskalPdvEntry( 5, faktur_rec.S_ukOsn05m, faktur_rec.S_ukPdv05m, racun.Pdv);

      // 07.06.2016: tek! ha, ha, ha... PNP entry 
      AddFiskalPdvEntry(ZXC.RRD.Dsc_PnpSt, faktur_rec.S_ukOsnPNP, faktur_rec.S_ukIznPNP, racun.Pnp);

      // 23.09.2016: tek! ha, ha, ha, ha, ha, ha... PPMV entry 
      AddFiskal_PPMV_Entry(faktur_rec.FirstTrn_PpmvSt1i2, faktur_rec.FirstTrn_PpmvOsn, faktur_rec.FirstTrn_PpmvIzn, racun.OstaliPor);

      #region PDV osnovica za UMJETNINE, rabljena vozila, ... (Iznos Marže)

      if(faktur_rec.IsUMJETNINA == true && faktur_rec.R_ukOsn25_UMJETNINA.NotZero())
      {
         //racun.IznosOslobPdvSpecified = true;

         racun.IznosMarza = faktur_rec.R_ukOsn25_UMJETNINA.ToStringVv_NoGroup_ForceDot_Fisk();
      }

      #endregion PDV osnovica za UMJETNINE, rabljena vozila, ... (Iznos Marže)

      #region Oslobodjeno od PDV-a

      decimal oslobodjenoOdPDVa = faktur_rec.S_ukOsn08 +
                                  faktur_rec.S_ukOsn09 +
                                  faktur_rec.S_ukOsn10 +
                                  faktur_rec.S_ukOsn11 +
                                  faktur_rec.S_ukOsn12 + //12.06.2024 tek dodano 
                                  faktur_rec.S_ukOsn13 + //12.06.2024 tek dodano 
                                  faktur_rec.S_ukOsn14 + //12.06.2024 tek dodano 
                                  faktur_rec.S_ukOsn15 + //12.06.2024 tek dodano 
                                  faktur_rec.S_ukOsn16 + //12.06.2024 tek dodano 
                                  faktur_rec.S_ukOsn0  ; //13.06.2024 tek dodano ... da li je ovo dobro ... ili treba ici kao 'AddFiskalPdvEntry' sa stopom nula!? 


      if(oslobodjenoOdPDVa.NotZero())
      {
         //racun.IznosOslobPdvSpecified = true;

         racun.IznosOslobPdv = oslobodjenoOdPDVa.ToStringVv_NoGroup_ForceDot_Fisk();
      }

      #endregion Oslobodjeno od PDV-a

      #region NE podlijeze PDV-u

      decimal nePodlijezePDVu = faktur_rec.S_ukOsn07;

      if(nePodlijezePDVu.NotZero())
      {
         //racun.IznosNePodlOporSpecified = true;

         racun.IznosNePodlOpor = nePodlijezePDVu.ToStringVv_NoGroup_ForceDot_Fisk();
      }

      #endregion NE podlijeze PDV-u

      #endregion Add PDV (and eventually some other tax - fuse)

      #region NEW from 2024 ... CHECK is some TAX missing

#if NOTYETTa
      // 25.04.2024: 
      decimal sumaSvihPoreza = racun.Pdv.Sum(rPdv => ZXC.ValOrZero_Decimal(rPdv.Iznos.Replace('.', ','), 2));
      decimal fakturPorezi   = faktur_rec.S_ukKCRP - faktur_rec.S_ukKCR;

      a i ovo je todo:
      decimal sumaSvihOsnovica = qweqwe;
      decimal fakturOsnovice   = qweqwe;
      ... pa i to provjeravati

      // TODO: s tamarom provjeriti slucajeve: malop, pnp, ppmv, umjetnina i sve sto nije klasicni KCR + pdv = KCRP 
      // tj. koju R_ varijablu treba kontrolirati sa sumaSvihPoreza, kako doci do 'fakturPorezi'                    
      if(ZXC.AlmostEqual(sumaSvihPoreza, fakturPorezi, 0.05M) == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, "KONTAKTIRAJTE SUPPORT!\n\r\n\rSuma poreza za fiskalizaciju ne odgovara\n\r\n\rfiskaliziram {0}\n\rfaktur_rec {1}",
            sumaSvihPoreza, fakturPorezi);
      }
#endif 

      #endregion NEW from 2024 ... CHECK is some TAX missing

      #region CentralniInformacijskiSustav.PosaljiRacun And Get Returned Info

      string theJIR = "", messageID = "";

      CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

      int timeOutMilliseconds = (int)(TimeSpan.FromSeconds(ZXC.RRD.Dsc_FISK_TimeOutSeconds).TotalMilliseconds);
      cis.TimeOut = timeOutMilliseconds; // postavljamo timeout via RiskRules 

      cis.SoapMessageSending += cis_SoapMessageSending;
      cis.SoapMessageSent    += cis_SoapMessageSent;

    //XmlDocument racunOdgovor_xmlDoc = cis.PosaljiRacun(racun, "FISKAL 1");                                   
    //string VvTempCertifikatFile   = @"E:\FiskalDemoCertificateByQukatz.pfx";
    //string VvTempCertifikatPasswd = @"326598";
      XmlDocument racunOdgovor_xmlDoc = null;
      try
      {
       //racunOdgovor_xmlDoc = cis.PosaljiRacun(racun, VvTempCertifikatFile, VvTempCertifikatPasswd); 
         // 19.12.2016: 
       //racunOdgovor_xmlDoc = cis.PosaljiRacun(                         racun);

         // 02.10.2017: Dilema! Da li uopce provjeravati ZXC.Vv_ThisClientHasFiskalConnection       
         // ili mozda bolje                              ZXC.Vv_ThisClientHas_NO_InternetConnection 
         // ZXC.Vv_ nece javiti nikakvu poruku, dok ce neuspio 'PosaljiRacun' pokusaj javiti gresku.
         // Dakle, sto zelimo? Da se useru naznaci ili ne da nije uspjelo. Trajanje provjere i pokusaj slanja traje podjednako! 
         if(/*ZXC.Vv_ThisClientHasFiskalConnection*/true)
         {
            racunOdgovor_xmlDoc = cis.PosaljiRacun(faktur_rec.TT_And_TtNum, racun);
         }
      }
      catch(System.Net.WebException ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška kod slanja SOAP poruke. Status greške: {0}. Poruka greške: {1}", ex.Status, ex.Message);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška kod slanja SOAP poruke. Poruka greške: {0}", ex.Message);
      }

      string returnedInnerXml;

      if(racunOdgovor_xmlDoc != null) returnedInnerXml = racunOdgovor_xmlDoc.InnerXml;
      else                            returnedInnerXml = null;

      if(returnedInnerXml == null)
      {
         // 
      }
      else // Deserialize Xml (this is by Q) 
      {
   //      // ovako mi ne ide! 
   //      //XmlSerializer xmlSer = new XmlSerializer(typeof(RacunOdgovor));
   //      //RacunOdgovor rnResponse = ((RacunOdgovor)xmlSer.Deserialize(qweqwe);

         string theJIRbyQ = ZXC.VvXmlElementValue(returnedInnerXml, "Jir", "tns");
                theJIR    = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(racunOdgovor_xmlDoc);

         if(theJIR != theJIRbyQ) ZXC.aim_emsg(MessageBoxIcon.Error, "Razliciti JIR\n\r\n\r{0}\n\r{1}", theJIR, theJIRbyQ);

         string messageIDbyQ = ZXC.VvXmlElementValue(returnedInnerXml, "IdPoruke", "tns");
                messageID    = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiUuid(racunOdgovor_xmlDoc, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor);

         if(messageID != messageIDbyQ) ZXC.aim_emsg(MessageBoxIcon.Error, "Razliciti messageID\n\r\n\r{0}\n\r{1}", messageID, messageIDbyQ);

      }

      #endregion CentralniInformacijskiSustav.PosaljiRacun And Get Returned Info

      #region Set FISK FEEDBACK data (JIR, ZKI, MessageID, OibOp)

      faktur_rec.FiskJIR   = theJIR;
      faktur_rec.FiskMsgID = messageID;
      faktur_rec.FiskZKI   = racun.ZastKod;
      faktur_rec.FiskOibOp = racun.OibOper;

      #endregion Set FISK FEEDBACK data (JIR, ZKI, MessageID, OibOp)

      return true;
   }

   //private string GetFisk_Oib_Oper(string faktur_AddUID)
   //{
   //   if(VvUserControl.UserSifrar == null) TheVvUC.SetSifrarAndAutocomplete<User>(null, VvSQL.SorterType.Code);
   //
   //   User user_rec = VvUserControl.UserSifrar.SingleOrDefault(user => user.UserName == faktur_AddUID);
   //
   //   if(user_rec != null) return user_rec.Oib;
   //   else                 return ""          ;
   //}

   internal string GetFisk_Oib_Oper(string faktur_AddUID)
   {
      User user_rec = TheVvUC.Get_User_FromVvUcSifrar(faktur_AddUID);

      if(user_rec != null) return user_rec.Oib;
      else                 return ""          ;
   }

   internal string GetFisk_RecID_Oper(string faktur_AddUID)
   {
      User user_rec = TheVvUC.Get_User_FromVvUcSifrar(faktur_AddUID);

      if(user_rec != null) return user_rec.RecID.ToString("000");
      else                 return ""                            ;
   }

   internal string GetFisk_ImePrezime_Oper(string faktur_AddUID)
   {
      User user_rec = TheVvUC.Get_User_FromVvUcSifrar(faktur_AddUID);

      if(user_rec != null) return user_rec.ImePrezime;
      else                 return ""                 ;
   }

   private decimal JehbhemTiMaterXMLsku(decimal badNumber)
   {
      decimal goodNumber;

      // Groovie: 125M NE prolazi, 125.00M prolazi!, 1250M prolazy, 12500M prolazy?! VOTAFAK!!! 
      // Dakle, treba ga prvo u string sa 2 decimale pa nazad u decimal. JeoImPsMtr!!! 

      goodNumber = ZXC.ValOrZero_Decimal(badNumber.ToStringVv_NoGroup_ForceDot(), 2);

      return goodNumber;
   }

   private void AddFiskalPdvEntry(decimal stopa, decimal osnovica, decimal iznos, List<PorezType> listaPoreza)
   {
      if(iznos.IsZero()) return;

      listaPoreza.Add(new PorezType() 
      { 
         Stopa    = /*JehbhemTiMaterXMLsku*/(stopa   ).ToStringVv_NoGroup_ForceDot_Fisk(),
         Osnovica = /*JehbhemTiMaterXMLsku*/(osnovica).ToStringVv_NoGroup_ForceDot_Fisk(), 
         Iznos    = /*JehbhemTiMaterXMLsku*/(iznos   ).ToStringVv_NoGroup_ForceDot_Fisk() 
      });
   }

   private void AddFiskal_PPMV_Entry(decimal stopa, decimal osnovica, decimal iznos, List<PorezOstaloType> listaPoreza)
   {
      if(iznos.IsZero()) return;

      listaPoreza.Add(new PorezOstaloType() 
      { 
         Stopa    = /*JehbhemTiMaterXMLsku*/(stopa   ).ToStringVv_NoGroup_ForceDot_Fisk(),
         Osnovica = /*JehbhemTiMaterXMLsku*/(osnovica).ToStringVv_NoGroup_ForceDot_Fisk(), 
         Iznos    = /*JehbhemTiMaterXMLsku*/(iznos   ).ToStringVv_NoGroup_ForceDot_Fisk() 
      });
   }

   private string RISK_Fiskalize_SendECHO(object sender, EventArgs e) // PINGaj  CentralniInformacijskiSustav 
   {
      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "U Projekt-u nije upaljena fiskalizacija!"); return ""; }

      CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();
      
      System.Xml.XmlDocument doc = cis.PosaljiEcho("");
      
      if(doc != null) return doc.InnerXml;
      else            return null;

   }

   // Well, OPP - oznaka poslovnog prostora je, dakle, 'SkladCd'! Mora se prijaviti SVAKO SKLADISTE (pa valjda, taman sa istom adresom) 
   /*private*/public void RISK_Fiskalize_PoslovniProstor(object sender, EventArgs e)
   {
      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "U Projekt-u nije upaljena fiskalizacija!"); return; }

      FiskalizePoslProstorDlg dlg = new FiskalizePoslProstorDlg();

      #region Initialize OPP Values

      VvLookUpItem theLui = ZXC.luiListaSkladista.SingleOrDefault(lui => lui.Cd == ZXC.TheVvForm.VvPref.findArtikl.LastUsedSkladCD); 

      if(theLui != null)
      {
         dlg.TheUC.Fld_SkladBr   = theLui.Integer;
         dlg.TheUC.Fld_SkladCd   = theLui.Cd     ;
         dlg.TheUC.Fld_SkladOpis = theLui.Name   ;
      }

      dlg.TheUC.Fld_DateStart = VvSQL.GetServer_DateTime_Now(TheDbConnection); // TODO: !!!!! ??? 
      dlg.TheUC.Fld_Grad      = ZXC.CURR_prjkt_rec.Grad           ;
      dlg.TheUC.Fld_Oib       = ZXC.CURR_prjkt_rec.Oib            ;
      dlg.TheUC.Fld_OibSw     = ZXC.copyrightOIB                  ;
      dlg.TheUC.Fld_Opcina    = ZXC.CURR_prjkt_rec.Opcina.NotEmpty() ? ZXC.CURR_prjkt_rec.Opcina : ZXC.CURR_prjkt_rec.Grad;
      dlg.TheUC.Fld_PostaBr   = ZXC.CURR_prjkt_rec.PostaBr        ;
      dlg.TheUC.Fld_RadVrij   = "od " + ZXC.CURR_prjkt_rec.RvrOd.ToShortTimeString() + " do " + ZXC.CURR_prjkt_rec.RvrDo.ToShortTimeString();

      dlg.TheUC.Fld_Ulica     = ZXC.CURR_prjkt_rec.UlicaBezBroja_2;
      dlg.TheUC.Fld_DodKbr    = ""                                ; 
      dlg.TheUC.Fld_Kbr       = ZXC.CURR_prjkt_rec.UlicniBroj_2   ; 

      // 21.01.2015: lec čraj find in Kupdob 
      Kupdob kupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(kpdb => kpdb.KupdobCD == dlg.TheUC.Fld_SkladBr);
      if(kupdob_rec != null && kupdob_rec.Tip.Contains("S")) // nasao je u adresaru partnera ovo skladiste? 
      {
         if(kupdob_rec.Grad   .NotEmpty()) dlg.TheUC.Fld_Grad      = kupdob_rec.Grad           ;
         if(kupdob_rec.Opcina .NotEmpty()) dlg.TheUC.Fld_Opcina    = kupdob_rec.Opcina         ; else dlg.TheUC.Fld_Opcina = kupdob_rec.Grad;
         if(kupdob_rec.PostaBr.NotEmpty()) dlg.TheUC.Fld_PostaBr   = kupdob_rec.PostaBr        ;

         // TODO: jednog dana staviti radVrijema na partnera pa da svako skladistwe moze imati zasebno rvr
       //dlg.TheUC.Fld_RadVrij   = "od " + ZXC.CURR_prjkt_rec.RvrOd.ToShortTimeString() + " do " + ZXC.CURR_prjkt_rec.RvrDo.ToShortTimeString();

         if(kupdob_rec.UlicaBezBroja_2.NotEmpty()) dlg.TheUC.Fld_Ulica = kupdob_rec.UlicaBezBroja_2;
         if(kupdob_rec.UlicniBroj_2   .NotEmpty()) dlg.TheUC.Fld_Kbr   = kupdob_rec.UlicniBroj_2   ; 
      }

      #endregion Initialize OPP Values

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      PoslovniProstorType poslovniProstor = new PoslovniProstorType() 
      { 
         Oib = dlg.TheUC.Fld_Oib, 
         
       //fisk_opp = "POSLOVNICA" + (faktur_rec.SkladCD.IsEmpty() ? "0" : faktur_rec.SkladCD); // ( opp – oznaka poslovnog prostora ) 
         OznPoslProstora = dlg.TheUC.Fld_SkladBr.ToString(),

         // 07.01.2013: 
         SpecNamj = dlg.TheUC.Fld_OibSw // VIPER-ov OIB 
      };

      AdresaType adresa = new AdresaType() 
      { 
         Ulica            = dlg.TheUC.Fld_Ulica  , // Adresa Fakturiranja 
         KucniBroj        = dlg.TheUC.Fld_Kbr    ,
         KucniBrojDodatak = dlg.TheUC.Fld_DodKbr , 
         BrojPoste        = dlg.TheUC.Fld_PostaBr, 
         Naselje          = dlg.TheUC.Fld_Grad   , 
         Opcina           = dlg.TheUC.Fld_Opcina
      };

      AdresniPodatakType adresniPodatak = new AdresniPodatakType() 
      { 
         Item = adresa 
      };

      poslovniProstor.AdresniPodatak       = adresniPodatak;
      poslovniProstor.RadnoVrijeme         = dlg.TheUC.Fld_RadVrij;
      poslovniProstor.DatumPocetkaPrimjene = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatum(dlg.TheUC.Fld_DateStart);

      CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

      cis.SoapMessageSending += cis_SoapMessageSending;
      cis.SoapMessageSent    += cis_SoapMessageSent;

      XmlDocument xmlDoc = null;
      try
      {
       //string VvTempCertifikatFile   = @"E:\FiskalDemoCertificateByQukatz.pfx";
       //string VvTempCertifikatPasswd = @"326598";
       //xmlDoc = cis.PosaljiPoslovniProstor(                                   poslovniProstor, "FISKAL 1");                                   
       //xmlDoc = cis.PosaljiPoslovniProstor(                                   poslovniProstor, VvTempCertifikatFile, VvTempCertifikatPasswd); 
         // 19.12.2016:                                                         
       //xmlDoc = cis.PosaljiPoslovniProstor(                                   poslovniProstor                                              ); 
         xmlDoc = cis.PosaljiPoslovniProstor((theLui != null ? theLui.Cd : ""), poslovniProstor); 
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);
      }


      string returnedInnerXml;

      if(xmlDoc != null) returnedInnerXml = xmlDoc.InnerXml;
      else               returnedInnerXml = null;

      if(returnedInnerXml == null)
      {
         // javi koja je greska
      }
      else // Deserialize Xml (this is by Q) 
      {
         string messageIDbyQ = ZXC.VvXmlElementValue(returnedInnerXml, "IdPoruke", "tns");
         string messageID    = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiUuid(xmlDoc, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.PoslovniProstorOdgovor);

         if(messageID != messageIDbyQ) ZXC.aim_emsg(MessageBoxIcon.Error, "Razliciti messageID\n\r\n\r{0}\n\r{1}", messageID, messageIDbyQ);
      }

      dlg.Dispose();

      if(returnedInnerXml != null)
      {
         MessageBox.Show(returnedInnerXml, "ODGOVOR pri fiskalizaciji poslovnog prostora", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
   }

   /*private*/public void RISK_Fiskalize_GetCISserviceStatus(object sender, EventArgs e)
   {
      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) { ZXC.aim_emsg(MessageBoxIcon.Error, "U Projekt-u nije upaljena fiskalizacija!"); return; }

      Cursor.Current = Cursors.WaitCursor;
      ZXC.stopWatch.Start();
      Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum status = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiStatusCisServisa();
      ZXC.stopWatch.Stop();
      //label26.Text = String.Format("Vrijeme: {0} s", stopWatch.Elapsed.TotalSeconds);
      //Application.DoEvents();

      ZXC.aim_emsg(MessageBoxIcon.Information, "CIS ServiceStatus:\n\n{0}\n\nElapsed seconds: {1}", status.ToString(), ZXC.stopWatch.Elapsed.TotalSeconds.ToString0Vv());

      ZXC.stopWatch.Reset();
      Cursor.Current = Cursors.Default;

      //switch(status)
      //{
      //   case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.green:
      //      pictureBox2.Image = Properties.Resources.Symbol_Check;
      //      break;
      //   case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.yellow:
      //      MessageBox.Show(status.ToString());
      //      break;
      //   case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.red:
      //      pictureBox2.Image = Properties.Resources.Symbol_Error;
      //      break;
      //   case Raverus.FiskalizacijaDEV.PopratneFunkcije.ServiceStatusEnum.unknown:
      //      MessageBox.Show(status.ToString());
      //      break;
      //   default:
      //      break;
      //}

   }

   #region Ako bi htjeli Stopati Elapsed Time Responsa

   void cis_SoapMessageSent(object sender, EventArgs e)
   {
      ZXC.stopWatch.Stop();
      //pictureBox1.Visible = false;
      //button1.Enabled = true;
      //button2.Enabled = true;
      //button3.Enabled = true;
      string label26_Text = String.Format("Vrijeme: {0} s", ZXC.stopWatch.Elapsed.TotalSeconds);
      // 26.05.2017: UGASIO DoEvents
    //Application.DoEvents();
      //ZXC.stopWatch.Reset();
      Cursor.Current = Cursors.Default;
   }

   void cis_SoapMessageSending(object sender, CentralniInformacijskiSustavEventArgs e)
   {
      Cursor.Current = Cursors.WaitCursor;
      ZXC.stopWatch.Reset();
      ZXC.stopWatch.Start();
      //pictureBox1.Visible = true;

      // 26.05.2017: UGASIO DoEvents
    //Application.DoEvents();
   }

   #endregion Ako bi htjeli Stopati Elapsed Time Responsa

   // PUSE 
   //private static string GetFiskZastKodIzdavat(RacunType racun, string theSecurityPreffix) // theSecurityPreffix ce biti naknadno definiran (privateKey, someSignature, ...?)
   //{
   //   return
   //      (
   //         theSecurityPreffix + 
   //         racun.Oib + 
   //         racun.DatVrijeme + 
   //         racun.BrRac.BrOznRac + 
   //         racun.BrRac.OznPosPr + 
   //         racun.BrRac.OznNapUr + 
   //         racun.IznosUkupno/*.ToStringVv()*/

   //      ).VvCalculateMD5();
   //}

   //private void RISK_FiskalPUSE(object sender, EventArgs e)
   //{
   //   VvFiskalizacija TheFISK = null;
   //   Faktur          faktur_rec = (Faktur)(TheVvDataRecord);

   //   string certFullPathFileName, certPassword, xmlFullPathFileName;
   //   bool isNaknadno;

   //   // TODO: start ________________________________________________________________________________
   //   certFullPathFileName = @"E:\FiskalDemoCertificateByQukatz.pfx";
   //   certPassword         = "326598";
   //   xmlFullPathFileName  = @"E:\Fisk_" + faktur_rec.TT_And_TtNum + ".xml";
   //   isNaknadno           = false;
   //   // TODO: end  ________________________________________________________________________________

   //   bool isGoodToGO = true;

   //   try
   //   {
   //      TheFISK = new VvFiskalizacija(TheDbConnection, faktur_rec, certFullPathFileName, certPassword, xmlFullPathFileName, isNaknadno);
   //   }
   //   catch(CryptographicException  ex) { isGoodToGO = false; ZXC.aim_emsg(MessageBoxIcon.Error, "Problem sa CERTIFIKATom:\n\n" + ex.Message  + "\n\nCertifikat:\n\n[" + certFullPathFileName + "]");   }
   //   catch(FileNotFoundException   ex) { isGoodToGO = false; ZXC.aim_emsg(MessageBoxIcon.Error, "Nema datoteke:\n\n[" + ex.FileName + "]!");   }
   //   catch(Exception               ex) { isGoodToGO = false; ZXC.aim_emsg(MessageBoxIcon.Error, "GREŠKA pri FISKALIZACIJI:\n\n" + ex.Message); }

   //   if(isGoodToGO) isGoodToGO = TheFISK.CreateXMLfile();
   //   else           return;

   //   if(isGoodToGO) isGoodToGO = TheFISK.SignXMLfile();
   //   else           return;

   //}

   // VOILA! 


   // TODO: !!!!!!!!! 

   /*private*/public void RISK_Fiskalize_AllPreviously_NOTfiskalized(object sender, EventArgs e)
   {
      RISK_Fiskalize_AllPreviously_NOTfiskalized_JOB(sender, e, false);
   }

   public void RISK_Fiskalize_AllPreviously_NOTfiskalized_JOB(object sender, EventArgs e, bool isAutoCheck)
   {
      int debugCount;

      // 25.01.2018: 
    //if(isAutoCheck == false)
      if(true                ) // pitaj uvijek 
      {
         DialogResult result = ZXC.ThisIsSkyLabProject == false ? MessageBox.Show("Da li zaista zelite FISKALIZIRATI sve do sada neuspješno fiskalizirane račune?!",
            "Potvrdite naknadnu FISKALIZACIJU?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) : DialogResult.Yes;

         if(result != DialogResult.Yes) return;
      }

      Cursor.Current = Cursors.WaitCursor;

      // ================================================================================================================================ 
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

      DataRowCollection    FakExSch = ZXC.FaktExDao.TheSchemaTable.Rows;  
      FaktExDao.FaktExCI   FakExCI  = ZXC.FaktExDao.CI;                   

      filterMembers.Add(new VvSqlFilterMember("tt",      "theTT", TtInfo.Prihod_IN_Clause, "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.fiskJIR], "theJIR", "",  " = "));

      VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Faktur>(TheDbConnection, RetardFiskalization, filterMembers, "dokDate , ttSort, ttNum ", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount, true);
      // ================================================================================================================================ 

      Cursor.Current = Cursors.Default;

      ShowNews();

      ZXC.aim_emsg("Gotovo. Fiskalizirao {0} računa.", debugCount);
   }

   // NOTA BENE! Do ovdje se dolazi brojeci nefiskalizirne IRM-ove                       
   // A ovaj ce dole fiskalizirati i sve ostale, eventualno, nefiskalizirane racune.     
   // Recimo IRA-e, ako prema pravilu u Prjkt_rec-u trebaju, a nisu, biti fiskalizirane. 
   private bool RetardFiskalization(XSqlConnection conn, VvDataRecord vvDataRecord)
   {
#if DEBUG
      ZXC.aim_emsg("DEBUG MODE: RetardFiskalization - early exit.");
      return false;
#endif
      Faktur faktur_rec = vvDataRecord as Faktur;

      // 26.01.2026: 
    //if(                   Faktur.IsFiskalDutyTT_ONLINE(faktur_rec.TT, faktur_rec.NacPlac, faktur_rec.NacPlac2)/*&& ZXC.Vv_ThisClientHasFiskalConnection*/                                              ) // ipak bez Vv_ThisClientHasFiskalConnection
      if(faktur_rec.IsF1 && Faktur.IsFiskalDutyTT_ONLINE(faktur_rec.TT, faktur_rec.NacPlac, faktur_rec.NacPlac2)/*&& ZXC.Vv_ThisClientHasFiskalConnection*/ && ZXC.CURR_prjkt_rec.IsNoAutoFiskal == false) // ipak bez Vv_ThisClientHasFiskalConnection                                  
      {
         RISK_Fiskalize_RACUN_JOB(faktur_rec, false);
      }

      return faktur_rec.EditedHasChanges() || faktur_rec.TheEx.EditedHasChanges();
   }

   internal bool TH_CheckAndForceFiskalization()
   {

      bool OK = true;

      if(ZXC.RISK_DisableAutoFiskTemporarily       ) return OK;

      if(ZXC.CURR_prjkt_rec.IsFiskalOnline == false) return OK;

      if(ZXC.CURR_prjkt_rec.IsNoAutoFiskal == true ) return OK;

      if(TheDbConnection.Database == ZXC.VvDB_prjktDB_Name) return OK;

      // 27.08.2024: 
    //uint NOTfisk_IRM_Count = FakturDao.CountNOTfiskalized_IRMs   (TheDbConnection);
       int NOTfisk_IRM_Count =           CountNOTfiskalized_Fakturs(TheDbConnection);

      if(NOTfisk_IRM_Count.NotZero())
      {
         Cursor.Current = Cursors.WaitCursor;
         
         int debugCount;

         // ================================================================================================================================ 
         List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);

         DataRowCollection FakExSch = ZXC.FaktExDao.TheSchemaTable.Rows;
         FaktExDao.FaktExCI FakExCI = ZXC.FaktExDao.CI;

         filterMembers.Add(new VvSqlFilterMember("tt", "theTT", TtInfo.Prihod_IN_Clause, "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
         filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.fiskJIR], "theJIR", "", " = "));

         VvDaoBase.GenericLoopAnd_RWTREC_AllRecord<Faktur>(TheDbConnection, RetardFiskalization, filterMembers, "dokDate , ttSort, ttNum ", TheVvTabPage.TheVvDatabaseInfoOn_SelectedVvTabPage.DataBaseName, out debugCount, true);
         // ================================================================================================================================ 

         Cursor.Current = Cursors.Default;

         //ShowNews();

         ZXC.aim_emsg("FISKALIZIRANO je {0} od {1} prethodno NEfiskaliziranih računa.", debugCount, NOTfisk_IRM_Count);

         OK = NOTfisk_IRM_Count == debugCount;
      }

      return OK;
   }

   private int CountNOTfiskalized_Fakturs(XSqlConnection conn)
   {
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(2);
      
      DataRowCollection FakExSch = ZXC.FaktExDao.TheSchemaTable.Rows;
      FaktExDao.FaktExCI FakExCI = ZXC.FaktExDao.CI;
      
      filterMembers.Add(new VvSqlFilterMember("tt", "theTT", TtInfo.Prihod_IN_Clause, "", "", " IN ")); // MORA BITI NONPARAMETERIZED VALUE za IN_clause!!!
      filterMembers.Add(new VvSqlFilterMember(FakExSch[FakExCI.fiskJIR], "theJIR", "", " = "));

      List<Faktur> theFakturList = new List<Faktur>();

      VvDaoBase.LoadGenericVvDataRecordList(conn, theFakturList, filterMembers, "", /*FakturOrderBy*/" dokDate, ttSort, ttNum ", true);

      theFakturList.RemoveAll(faktur => faktur.IsFiskalDutyFaktur_ONLINE == false || faktur.FiskJIR.NotEmpty());

      return theFakturList.Count();
   }

   private void Swap_DisableEnable_AutoFisk(object sender, EventArgs e)
   {
      ZXC.RISK_DisableAutoFiskTemporarily = !ZXC.RISK_DisableAutoFiskTemporarily;

      ZXC.aim_emsg(MessageBoxIcon.Information, "RISK_DisableAutoFiskTemporarily iz now " + ZXC.RISK_DisableAutoFiskTemporarily.ToString());
   }

   #endregion FISKALIZACIJA

   #region All About SKY

   private void Swap_EnableDisable_Cache(object sender, EventArgs e)
   {
      ZXC.RISK_DisableCacheTemporarily = !ZXC.RISK_DisableCacheTemporarily;

      if(ZXC.RISK_DisableCacheTemporarily == true ) ZXC.aim_emsg(MessageBoxIcon.Warning, "Cache is\n\nDISABLED");
      if(ZXC.RISK_DisableCacheTemporarily == false) ZXC.aim_emsg(MessageBoxIcon.Warning, "Cache is\n\nENABLED" );
   }

   private void UnLock(object sender, EventArgs e)
   {
      ZXC.SENDtoSKY_InProgress      = 
      ZXC.RECEIVEfromSKY_InProgress = false;

      VvSQL.DROP_TABLE(TheDbConnection, TheDbConnection.Database, ZXC.vvDB_lockerTableName);

      ZXC.VvXmlDR_LastDocumentMissing_AlertRaised = false;

   }

   public void SEND_ToSKY(object sender, EventArgs e)
   {
      // 23.09.2017: sa '_JOB' varijantom SkyLab-u saljemo CurrItemIsLastItem value 
      SEND_ToSKY_JOB(sender, e);
   }
   public List<VvSkyLab.VvSyncInfo> SEND_ToSKY_JOB(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.SEND, false, true, false);

      return syncInfoList;
   }
   private void SEND_ToSKY_AddOnly(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.SEND, false, true, true);
   }

   public void RECEIVE_FromSKY(object sender, EventArgs e)
   {
      // 23.09.2017: sa '_JOB' varijantom SkyLab-u saljemo CurrItemIsLastItem value 
      RECEIVE_FromSKY_JOB(sender, e);
   }
   public List<VvSkyLab.VvSyncInfo> RECEIVE_FromSKY_JOB(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.RECEIVE, false, true, false);

      EventualRegenerateWholeCache(syncInfoList); // Samo za ZXC.IsTEXTHOshop, centrala ima drugaciju logiku 

      return syncInfoList;
   }

   /*private*/ public void Check_SEND_ToSKY(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.SEND, true, true, false);
   }
   private void Check_SEND_ToSKY_AddOnly(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.SEND, true, true, true);
   }

   /*private*/ public void Check_RECEIVE_FromSKY(object sender, EventArgs e)
   {
      List<VvSkyLab.VvSyncInfo> syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.RECEIVE, true, true, false);
   }

   internal List<VvSkyLab.VvSyncInfo> SynchronyzeWithSKY(ZXC.SkyOperation skyOperation, bool isCheckOnly, bool isInitiatedExplicit, bool isADD_ONLY)
   {
      // 02.10.2017: 
    //if(ZXC.NO_Internet                         ) return null; 
      if(ZXC.Vv_ThisClientHas_NO_SkyLabConnection) return null; 

      if(ZXC.IsTEXTHOsky) { ZXC.aim_emsg(MessageBoxIcon.Error, "Akcija odbijena!\n\nNe mogu sinhronizirati samog sebe."); return null; } 

      if(ZXC.SENDorRECEIVE_SKY_InProgress) 
      { 
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Sinhronizacijska akcija odbijena. Pokušajte malo kasnije ponovno.\n\nNeka druga,\n\nvjerovatno background,\n\nsync operacija u tijeku.");

         // 27.08.2018: dodan log
         ZXC.aim_log("SynchronyzeWithSKY failed: ZXC.SENDtoSKY_InProgress: {0}, ZXC.RECEIVEfromSKY_InProgress: {1}.", ZXC.SENDtoSKY_InProgress.ToString(), ZXC.RECEIVEfromSKY_InProgress.ToString());

         // 25.05.2015: !!! BIG BIG BIG NEWS !!!                                                
         // Mozda je ovo finalni udarac omogucavanju odlaska u minus TH Shopova                 
         // jerbo pri neuspjelom prethodnom RECEIVE-anju (timeout), pri sljed. ulasku u program 
         // ovaj ne da RECEIVE-ati zbog lockera, ali bogami niti Rebuild CACHE-a                
         // a za cacheove koje je prethodno djelomicni RECEIVE izbrisao                         
         // tako da ovdje onde prisiljavamo RISK_NewCache                                       
         if(ZXC.IsTEXTHOshop) // Dakle samo za SHOP, na centrali idemo dalje!?  
         {
            RISK_NewCache("SKY", EventArgs.Empty);

            //... ovo gore RISK_NewCache bi kasnije mozda trebalo optimizirati      
            // tako da ga opkolis sa ... missing_Count = CountMissingArtstat(conn); 
            // if(missing_Count.NotZero()) ... tek tada daje cio novi cache         

         }

         return null; 

      } // neka druga, vjerovatno background, sync operacija u tijeku 

      string origLanDatabase =     TheDbConnection   .Database;
      string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      //02.10.2017: 
    //// 19.05.2015: 
    //if(ZXC.NO_SKY_Connection) return null;

      List<VvSkyLab.VvSyncInfo> syncInfoList = VvSkyLab.SynchronyzeWithSKY(TheDbConnection, ZXC.TheSkyDbConnection, skyOperation, isCheckOnly, isInitiatedExplicit, isADD_ONLY);

          TheDbConnection   .ChangeDatabase(origLanDatabase);
      ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      return syncInfoList;
   }

   //void VvForm_FormClosing_ExecuteSynchronisation_SEND(object sender, FormClosingEventArgs e)
   //{
   //   if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return;
   //
   // //if(true /* fuse: ocuNecu ovo sa login forme ili rulsa*/) SynchronyzeWithSKY(ZXC.SkyOperation.SEND, false, false);
   //   if(true /* fuse: ocuNecu ovo sa login forme ili rulsa*/)
   //   {
   //      try
   //      {
   //         SynchronyzeWithSKY(ZXC.SkyOperation.SEND, false, false);
   //      }
   //      catch { } // There is already an open DataReader associated with this Connection which must be closed first. 
   //   }
   //}

   internal void VvForm_FormLoad_ExecuteSynchronisation_SEND_then_RECEIVE(object sender, EventArgs e)
   {
      if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return;

      // 27.09.2016: !!! nemoj implicitno sinhronizirati ako nije tekuca godina 
      if(ZXC.projectYearFirstDay.Year != DateTime.Now.Year) return;

      // 17.09.2018: ubrzanje 4 qukatz servisni ulazak 
      if(ZXC.ThisIsQUKATZ) return;

      // 17.09.2018: ubrzanje 4 superuser@TEXTHOshop servisni ulazak 
      if(ZXC.CURR_userName == ZXC.vvDB_programSuperUserName && ZXC.IsTEXTHOshop) return; 

      // 30.11.2015: 
      // 01.12.2015: 
      //if(ZXC.IsTEXTHOcentrala && ZXC.vvDB_IsLocalhost == false) return; // ne sinhroniziraj implicitno na centrali ako nisi na serveru ('localhost'-u) 
      if(ZXC.IsTEXTHOcentrala) return; // ne sinhroniziraj implicitno na centrali NIKADA                              

      List<VvSkyLab.VvSyncInfo> syncInfoList;

      ZXC.TheVvForm.Refresh(); Cursor.Current = Cursors.WaitCursor;

      /*List<VvSkyLab.VvSyncInfo> syncInfoList =*/
      if(true /* fuse: ocuNecu ovo sa login forme ili rulsa */)                SynchronyzeWithSKY(ZXC.SkyOperation.SEND   , false, false, false);
      /*List<VvSkyLab.VvSyncInfo> syncInfoList =*/
      if(true /* fuse: ocuNecu ovo sa login forme ili rulsa */) syncInfoList = SynchronyzeWithSKY(ZXC.SkyOperation.RECEIVE, false, false, false);

      // 29.05.2015: odlucio do dalnjega suspendati ovo ziherastvo! 
      //if(syncInfoList != null && syncInfoList.Count.NotZero() && ZXC.IsTEXTHOshop) RISK_NewCache(/*object sender*/"SKY", EventArgs.Empty);
      // 22.06.2015: kaj ziher je ziher 
      //if(DateTime.Today.DayOfWeek == DayOfWeek.Saturday) RISK_NewCache(/*object sender*/"SKY", EventArgs.Empty);
      // 24.06.2015: zbog Delete_Then_Renew_Cache_FromThisRtrans_JOB: if(ZXC.IsTEXTHOshop && ZXC.RECEIVEfromSKY_InProgress) return true; 
      //if(syncInfoList != null && syncInfoList.Count.NotZero() && ZXC.IsTEXTHOshop) RISK_NewCache(/*object sender*/"SKY", EventArgs.Empty);
      EventualRegenerateWholeCache(syncInfoList); // ovo je samo preimenovano - refaktorirano jer ti isto treba i gore u 'RECEIVE_FromSKY()' 

      // 28.08.2016: 
      Eventual_CheckSync_SendMailReport(); 

      Cursor.Current = Cursors.Default;
   }

   private void Eventual_CheckSync_SendMailReport()
   {
      // SAMO TEXTHOshop u ponedeljak do 9:59 ujutro 
      if(ZXC.IsTEXTHOshop == false                                 ) return;
    //if(ZXC.programStartedOnDateTime.DayOfWeek != DayOfWeek.Monday) return; sa ovim u komentaru, ide svaki dan! 

      // 01.09.2025: gasimo ovo ogranicenje tako da ide savki put kod ulaska u program, a ne samo ako je prije 9 ujutro
    //if(ZXC.programStartedOnDateTime.Hour      > 9                ) return; 

      // ... here we go ... 

      // op.a. 23.10.2016. si se isao cuditi zasto se i na SHOPovima pri ulasku ne poziva 'SqlSomeCheckQuery_JOB' 
      // pa zakljucio da je to valjda zbog toga sto ce eventualni error prije ili kasnije isplivati na centrali   
      // ovaj zakljucak je NEPROVJEREN te bi ga trebalo u buduce pomnije izanalizirati                            

      bool sqlCheckOK  = true; //  SqlSomeCheckQuery_JOB ("SkyLab", EventArgs.Empty   );    skip do dalnjega 
      bool syncCheckOK =           Check_SYNC_WithSKY_JOB("SkyLab", EventArgs.Empty, 1); // "SkyLab" parametar sluzi da CheckSYNC ide silently a ne na grid UC-a 

#region Check ZPC existance for today

      // 10.11.2017: ima li u fajlu valjani ZPC za danasnji dan 

      string sklCdRoot = ZXC.vvDB_ServerID.ToString("00") + "M";

      VvLookUpItem lui = ZXC.luiListaSkladista./*Single*/FirstOrDefault(l => l.Cd.StartsWith(sklCdRoot));
      string skladCD = lui.Cd;

      // THPR news: 
    //DateTime activeZPCdate = ZXC                       .GetActiveZPCdate_ForThisDay(DateTime.Today, skladCD, ZXC.IsTH_5WeekShop(skladCD));
      DateTime activeZPCdate = TH_PriceRuleForCycleMoment.GetActiveZPCdate_ForThisDay(TH_PriceRuleForCycleMoment.GetTHPR_ForThisDay(skladCD, DateTime.Today), DateTime.Today);

      bool hasZPCforToday = FakturDao.FakturExistsFor_Sklad_And_TT_And_Date(TheDbConnection, skladCD, Faktur.TT_ZPC, activeZPCdate);

      // 03.01.2024: 
      if(hasZPCforToday == false && activeZPCdate < ZXC.projectYearFirstDay)
      {
         hasZPCforToday = true;
      }

      // 10.11.2017: javi i poslovnici da zna 
      if(hasZPCforToday == false)
      {
         ZXC.aim_emsg(MessageBoxIcon.Stop   , "Za današnji dan NEMA (nije stigao sinkronizacijom) ZPC-a!\n\nOBAVEZNO kontaktirajte support."   );
         ZXC.aim_emsg(MessageBoxIcon.Warning, "Budući da za danas NEMA ZPC-a, IRM i TRI imati će krive MPC!\n\nOBAVEZNO kontaktirajte support.");
      }

#endregion Check ZPC existance for today

#region For every RECEIVEd ZPC call CheckZPC() ... u ZXC.Received_ZPC_List nam je lista EVENTUALNO receivanih ZPC-ova

      if(ZXC.Received_ZPC_List != null)
      {
         foreach(ZXC.CdAndName_CommonStruct receivedZPC in ZXC.Received_ZPC_List)
         {
            int wouldBe_newZpcRtransListCount = 0;
            bool zpcOK = true;
      
            // THPR news: 
          //wouldBe_newZpcRtransListCount = ZXC.TheVvForm.TH_CreateZPC(receivedZPC.TheDate, receivedZPC.TheCd, /*is5week*/ ZXC.IsTH_5WeekShop(receivedZPC.TheCd), receivedZPC.TheDate, /* isChkOnly*/ true       );
            wouldBe_newZpcRtransListCount = ZXC.TheVvForm.TH_CreateZPC(receivedZPC.TheDate, receivedZPC.TheCd,                                                    receivedZPC.TheDate, /* isChkOnly*/ true, false);
            
            zpcOK = wouldBe_newZpcRtransListCount.IsZero();
            
            if(zpcOK == false)
            {
               ZXC.aim_emsg(MessageBoxIcon.Stop, "GREŠKA!\n\n{0} stavak. nedostaje ili ima krivu cijenu s obzirom trenutak u ciklusu!\n\nZPC za skl: {1} datum: {2} broj: {3}",
                  wouldBe_newZpcRtransListCount, receivedZPC.TheCd, receivedZPC.TheDate.ToString(ZXC.VvDateFormat), receivedZPC.TheUint);
      
               ZXC.aim_emsg(MessageBoxIcon.Warning, "OBAVEZNO kontaktirajte support.");
      
               string errMsg = "CheckZPC: Missing rtrans or wrong MPC rtrans. ErrCount: " + wouldBe_newZpcRtransListCount;
            
               VvMailClient mailClient = new VvMailClient();
               mailClient.SendMail_SUPPORT(false, "ZPC!", errMsg, ZXC.VvDeploymentSite + "_" + ZXC.vvDB_ServerID.ToString());
            }
         }

         ZXC.Received_ZPC_List.Clear();
      } // if(ZXC.Received_ZPC_List != null) 

#endregion For every RECEIVEd ZPC call CheckZPC()

      bool hasErrors = (sqlCheckOK == false || syncCheckOK == false || hasZPCforToday == false);

    //if(sqlCheckOK == false || syncCheckOK == false || hasZPCforTHisDay == false)
      if(hasErrors                                                               )
      {
   
         string logFileName = ZXC.aim_log_file_name();
    
         VvMailClient mailClient = new VvMailClient();
    
         mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword       ; // !!! ovo moze biti i prazno! Tada ida anonymously?! 
    
         mailClient.MailHost             = ZXC.ViperMailHost             ;
         mailClient.EmailFromAddress     = ZXC.SkyLabEmailAddress        ;
         mailClient.EmailFromDisplayName = "TH " + ZXC.vvDB_ServerID + " " + ZXC.SkyLabEmailFromDisplayName;
       //mailClient.EmailTo              = ZXC.VektorEmailAddress        ;
       //mailClient.MessageSubject       = (hasErrors ?                                    "ERR " : "OK ")            + 
         mailClient.MessageSubject       = (hasErrors ? hasZPCforToday == false ? "ZPC " : "ERR " : "OK ")            + 
                                           "sqlCheckOK = "        + sqlCheckOK     + 
                                           " / syncCheckOK = "    + syncCheckOK    +
                                           " / hasZPCforToday = " + hasZPCforToday +
                                           " TEXTOshop " + ZXC.vvDB_ServerID       ;

         mailClient.MessageBody          = "";

         mailClient.MessageBody         += VvSkyLab.ExtractErrorAndSyncDiffLinesFromLogFile(logFileName);
    
         string attachmentFullFileNamePath = ZXC.Vv_GZip_ThisFile(logFileName);

         if(hasErrors)
         {
          //mailClient.MailAttachmentFileNameList = new System.Net.Mail.Attachment(attachmentFullFileNamePath);
            mailClient.MailAttachmentFileNameList = new string[] { attachmentFullFileNamePath };
         }

         mailClient.SendMail_Normal(/*true*/ false, ZXC.VektorEmailAddress); // silent send 

         if(hasErrors)
         {
            // delete temporary GZip file:
            try { System.IO.File.Delete(attachmentFullFileNamePath); }
            catch(Exception ex) { ZXC.aim_emsg("Delete File Error:\n\n{0}", ex.Message); }
         }

      } // if(hasErrors) 


   }

   // Ako odlucis UVIJEK na centrali rebuildati cache nakon RECEIVE-a, 
   // moras prvo i supressati Delete_Then_Renew_Cache_FromThisRtrans_JOB prilikom risiva IRM-ova
   // te onda regenerirati cio ovdje. 
   //
   // u RtransDao imas Should_Delete_Then_Renew_Cache():
   //       // 24.06.2015: 
   //       if(ZXC.IsTEXTHOshop && ZXC.RECEIVEfromSKY_InProgress) goOn = false;  // !!! 24.06.2015: za ono SENDanje 1212 ZPCa pa da kad poslovnice RECEIVEaju bude brze ... u oparu sa 'VvForm_FormLoad_ExecuteSynchronisation_SEND_then_RECEIVE' RISK_NewCache! 
   // pa bi ga trebalo obogatiti i za 'ZXC.IsTEXTHOcent' 
   private void EventualRegenerateWholeCache(List<VvSkyLab.VvSyncInfo> syncInfoList)
   {
      // 23.11.2015: thoughts: ako ubuduce i stavis filter da se, recimo, ovo desava samo subotom, 
      // onda treba ostale dane NE samo supresati RISK_NewCache nego i pozvati obican CheckCache da rebuilda za eventualno 
      // RECEIVE-ane a ne cacheane stavke zbog Should_Delete_Then_Renew_Cache() kočnice if(ZXC.IsTEXTHOshop && ZXC.RECEIVEfromSKY_InProgress) goOn = false; 
      if(syncInfoList != null && syncInfoList.Count.NotZero() && ZXC.IsTEXTHOshop)
      {
         // 18.12.2016: dosta sranja! Ziheraski svaki dan ujutro! 
       //if(DateTime.Today.DayOfWeek == DayOfWeek.Saturday) RISK_NewCache("SKY", EventArgs.Empty);
         // 25.03.2017: dosta sranja!!! JOS ZiheraskiJE KOD SVAKOG ULASKA! 
       //if(ZXC.programStartedOnDateTime.Hour < 9) RISK_NewCache("SKY", EventArgs.Empty);
         if(true                                 ) RISK_NewCache("SKY", EventArgs.Empty);
         else                                      ArtiklDao.CheckCache(TheDbConnection, true);
      }
      // 30.12.2022: ako nije nista receive-ao a danas je dan inventure ... rebuildaj! 
      else if(ZXC.IsTEXTHOshop && ZXC.programStartedOnDateTime.Date == ZXC.TexthoInventuraDate.Date)
      {
         this.Show();
         RISK_NewCache("SKY", EventArgs.Empty);
      }
   }

#region MarkAs SENDed_OR_RECEIVEd To or From SKY

   /*private*/ public void MarkADDactionAs_SENDed_ToSKY(object sender, EventArgs e)
   {
      ForceSKYlogEntry(ZXC.SkyOperation.SEND);
   }
   /*private*/ public void Un_MarkADDactionAs_SENDed_ToSKY(object sender, EventArgs e)
   {
      DeleteADD_SKYlogEntry(ZXC.SkyOperation.SEND);
   }

   /*private*/ public void MarkADDactionAs_RECEIVEd_FromSKY(object sender, EventArgs e)
   {
      ForceSKYlogEntry(ZXC.SkyOperation.RECEIVE);
   }

   // 15.10.2020: tek sada (prije bio prazni 'UnMarkAsRECEIVEd')
   /*private*/ public void Un_MarkADDactionAs_DISPACHED_ToShop_OrCentr_FromSKY(object sender, EventArgs e)
   {
      if(!ZXC.IsTEXTHOsky)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Un_MarkADDactionAs_DISPACHED_ToShop_OrCentr_FromSKY moze samo na SKY-u!");
#if !DEBUG
         return;
#endif
      }

      VvUn_MarkAsDISPATCHedDlg dlg = new VvUn_MarkAsDISPATCHedDlg();

      if(dlg.ShowDialog() != DialogResult.OK)
      {
         dlg.Dispose();
         return;
      }

      int serverID = dlg.Fld_ServerID;

      dlg.Dispose();

      DeleteADD_SKYlogEntry_AsDISPACHED_ToShop_OrCentr(serverID);
   }


   private void ForceSKYlogEntry(ZXC.SkyOperation skyOperation)
   {
      if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return;

      if(ZXC.IsTEXTHOcentrala)
      {
         if(skyOperation == ZXC.SkyOperation.SEND   ) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi SEND operaciju!"); return; }
         if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
      }
      if(ZXC.IsTEXTHOshop)
      {
         if(skyOperation == ZXC.SkyOperation.SEND   ) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi SEND operaciju!"); return; }
         if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
      }

      string origLanDatabase = TheDbConnection.Database;
      string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      bool OK = VvSkyLab.ForceSKYlogEntry_ADD(TheDbConnection, ZXC.TheSkyDbConnection, skyOperation, TheVvDataRecord);

      TheDbConnection.ChangeDatabase(origLanDatabase);
      ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "GOTOVO\n\nOn {3} database\n\nIn '{2}'\n\nSkyLogEntry for ADD action has been successfuly inserted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, ZXC.vvDB_SKYlogTableName, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD");
   }

   private void DeleteADD_SKYlogEntry_AsDISPACHED_ToShop_OrCentr(int serverID)
   {
      string origLanDatabase = TheDbConnection.Database;
      //string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      string SKY_log_tableName = ZXC.vvDB_SKYlogTableNameBase + serverID.ToString("00");

      bool OK = VvSkyLab.DeleteSKYlogEntry_ADD_AsDISPACHED_ToShop_OrCentr(TheDbConnection, SKY_log_tableName, TheVvDataRecord);

      TheDbConnection.ChangeDatabase(origLanDatabase);
    //ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, 
         "GOTOVO\n\nIn '{2}'\n\nSkyLogEntry for ADD action has been successfuly deleted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, SKY_log_tableName/*ZXC.vvDB_SKYlogTableName*//*, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD"*/);
      else ZXC.aim_emsg(MessageBoxIcon.Error,
         "ERROR\n\nIn '{2}'\n\nSkyLogEntry for ADD action has been UNsuccessfuly deleted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, SKY_log_tableName/*ZXC.vvDB_SKYlogTableName*//*, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD"*/);
   }

   private void DeleteADD_SKYlogEntry(ZXC.SkyOperation skyOperation)
   {
      if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return;

      if(ZXC.IsTEXTHOcentrala)
      {
         if(skyOperation == ZXC.SkyOperation.SEND) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi SEND operaciju!"); return; }
         if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.SEND) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
      }
      if(ZXC.IsTEXTHOshop)
      {
         if(skyOperation == ZXC.SkyOperation.SEND) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi SEND operaciju!"); return; }
         if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.SEND) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
      }

      string origLanDatabase = TheDbConnection.Database;
      string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      bool OK = VvSkyLab.DeleteSKYlogEntry_ADD(TheDbConnection, ZXC.TheSkyDbConnection, skyOperation, TheVvDataRecord, false);

      TheDbConnection.ChangeDatabase(origLanDatabase);
      ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "GOTOVO\n\nOn {3} database\n\nIn '{2}'\n\nSkyLogEntry for ADD action has been successfuly deleted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, ZXC.vvDB_SKYlogTableName, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD");
   }

   /*private*/ public void ForceADDaction4_LogLAN(object sender, EventArgs e)
   {
      // !!! if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return; !!! 

      //if(ZXC.IsTEXTHOcentrala)
      //{
      //   if(skyOperation == ZXC.SkyOperation.SEND)    if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi SEND operaciju!"   ); return; }
      //   if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
      //}
      //if(ZXC.IsTEXTHOshop)
      //{
      //   if(skyOperation == ZXC.SkyOperation.SEND)    if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi SEND operaciju!"   ); return; }
      //   if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
      //}
      //
      string origLanDatabase = TheDbConnection.Database;
      //string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      bool OK = VvSkyLab.ForceLANlogEntry_ADD(TheDbConnection, /*ZXC.TheSkyDbConnection, skyOperation,*/ TheVvDataRecord);

      TheDbConnection.ChangeDatabase(origLanDatabase);
      //ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "GOTOVO\n\nIn '{2}'\n\nLanLogEntry for ADD action has been successfuly inserted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, ZXC.vvDB_LANlogTableName/*, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD"*/);
   }

   /*private*/ public void ForceRWTaction4_LogLAN(object sender, EventArgs e)
   {
      // !!! if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return; !!! 

      //if(ZXC.IsTEXTHOcentrala)
      //{
      //   if(skyOperation == ZXC.SkyOperation.SEND)    if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi SEND operaciju!"   ); return; }
      //   if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.CentOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
      //}
      //if(ZXC.IsTEXTHOshop)
      //{
      //   if(skyOperation == ZXC.SkyOperation.SEND)    if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi SEND operaciju!"   ); return; }
      //   if(skyOperation == ZXC.SkyOperation.RECEIVE) if(TheVvDataRecord.SkyRule.ShopOPS == ZXC.SkyOperation.SEND   ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
      //}
      //
      string origLanDatabase = TheDbConnection.Database;
      //string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      bool OK = VvSkyLab.ForceLANlogEntry_RWT(TheDbConnection, /*ZXC.TheSkyDbConnection, skyOperation,*/ TheVvDataRecord);

      TheDbConnection.ChangeDatabase(origLanDatabase);
      //ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "GOTOVO\n\nIn '{2}'\n\nLanLogEntry for RWT action has been successfuly inserted.\n\nLanRecID: {0}\n\nLanSrvID: {1}",
         TheVvDataRecord.VirtualLanRecID, TheVvDataRecord.VirtualLanSrvID, ZXC.vvDB_LANlogTableName/*, skyOperation == ZXC.SkyOperation.SEND ? "LOCAL" : "SKY in CLOUD"*/);
   }

   private void Force_MANY_RWTaction4_LogLAN(object sender, EventArgs e)
   {
#region Init

      if(ZXC.IsSkyEnvironment == false || ZXC.vvDB_ServerID == ZXC.vvDB_ServerID_SkyCloud) return;
      
      if(ZXC.SENDorRECEIVE_SKY_InProgress) { ZXC.aim_emsg(MessageBoxIcon.Warning, "Akcija odbijena. Pokušajte malo kasnije ponovno.\n\nNeka druga,\n\nvjerovatno background,\n\nsync operacija u tijeku."); return; } // neka druga, vjerovatno background, sync operacija u tijeku 

      string origLanDatabase = TheDbConnection.Database;
      string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      SIN_UC theUC = TheVvUC as SIN_UC;

      if(theUC.TheG.SelectedRows.Count.IsZero()) { ZXC.aim_emsg(MessageBoxIcon.Stop, "Nije selektiran nijedan redak."); return; }

      bool   OK         ;
      string tt         ;
      uint   ttNum      ;
      uint   count   = 0;
      uint   okCount = 0;
      Faktur faktur_rec ;

#endregion Init

      foreach(DataGridViewRow row in theUC.TheG.SelectedRows)
      {
         count++;

         tt    = theUC.TheG.GetStringCell(theUC.DgvCI.iT_tt   , row.Index, false);
         ttNum = theUC.TheG.GetUint32Cell(theUC.DgvCI.iT_ttNum, row.Index, false);
       //ZXC.aim_emsg("row {0} tt {1} ttNum {2} ", row.Index, tt, ttNum);

         faktur_rec = new Faktur();
         OK = faktur_rec.VvDao.SetMe_VvDocumentRecord_byTtAndTtNum(TheDbConnection, faktur_rec, tt, ttNum, false, false);
         if(!OK) continue;

         if(ZXC.IsTEXTHOcentrala && faktur_rec.SkyRule.CentOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi SEND operaciju!\n\n{0}",   faktur_rec); continue; }
         if(ZXC.IsTEXTHOshop     && faktur_rec.SkyRule.ShopOPS == ZXC.SkyOperation.RECEIVE) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi SEND operaciju!\n\n{0}", faktur_rec); continue; }

         OK = VvSkyLab.ForceLANlogEntry_RWT(TheDbConnection, faktur_rec);

         if(OK) okCount++;
      }

#region Final

      TheDbConnection.ChangeDatabase(origLanDatabase);
      ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

#endregion Final

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo.\n\n\n\nCount\t[{0}]\n\nokCount\t[{1}]\n\nerrCount\t[{2}]\n\n\n\nNext SEND operation will RE SEND [{3}] records.", count, okCount, count - okCount, okCount);
   }


   /*private*/ public void UndoReceive_RollItBack(object sender, EventArgs e) // Kada record stigne nepotpun (fale ili rtrans ili faktExt ili vvlogLAN)
   {
      // if(ZXC.IsSkyEnvironment == false) return; mozda zatreba i negdje drugdje 

      if(TheVvDataRecord.VirtualRecordName != Faktur.recordName)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "UndoReceive_RollItBack radi SAMO za Faktur record!"); return;
      }

      if(ZXC.IsTEXTHOcentrala)
      {
         //if(TheVvDataRecord.SkyRule.CentOPS != ZXC.SkyOperation.RECEIVE                                                                      ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
         if(TheVvDataRecord.SkyRule.CentOPS != ZXC.SkyOperation.RECEIVE && TheVvDataRecord.SkyRule.CentOPS != ZXC.SkyOperation.SendAndReceive) { ZXC.aim_emsg(MessageBoxIcon.Error, "Centrala ne može izvoditi RECEIVE operaciju!"); return; }
      }
      if(ZXC.IsTEXTHOshop)
      {
         //if(TheVvDataRecord.SkyRule.ShopOPS != ZXC.SkyOperation.RECEIVE                                                                      ) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
         if(TheVvDataRecord.SkyRule.ShopOPS != ZXC.SkyOperation.RECEIVE && TheVvDataRecord.SkyRule.ShopOPS != ZXC.SkyOperation.SendAndReceive) { ZXC.aim_emsg(MessageBoxIcon.Error, "Poslovnica ne može izvoditi RECEIVE operaciju!"); return; }
      }

      string origLanDatabase = TheDbConnection.Database;
      //string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      int nora;

      bool OK = VvSkyLab.UndoReceive_RollItBack(TheDbConnection, /*ZXC.TheSkyDbConnection,*/ TheVvDataRecord, out nora);

      TheDbConnection.ChangeDatabase(origLanDatabase);
      //ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

      if(OK) ZXC.aim_emsg(MessageBoxIcon.Information, "GOTOVO\n\nZa RecID {0} \n\n{1} rows affected. (deleted)", TheVvDataRecord.VirtualRecID, nora);

      ShowNews();
   }

#endregion MarkAs SENDed_OR_RECEIVEd To or From SKY

   public void SqlSomeCheckQuery(object sender, EventArgs e) // OVO se moze/smije/treba koristiti i na NON SkyEnvironment database-ima! 
   {
      SqlSomeCheckQuery_JOB(sender, e);
   }

   public bool SqlSomeCheckQuery_JOB(object sender, EventArgs e) // OVO se moze/smije/treba koristiti i na NON SkyEnvironment database-ima! 
   {
      bool hasProblem;
      bool GeneralOK = true;
      bool sqlOK;
      DialogResult result;
      ZXC.MySqlCheck_Kind MySqlCheck_Kind;

      bool isProgramLoad = sender.ToString() == "PROGRAM LOAD";
      bool isSkyLab      = sender.ToString() == "SkyLab"      ;

      if(isSkyLab == false) Cursor.Current = Cursors.WaitCursor;

      string origDatabase = ZXC.TheMainDbConnection.Database;
      ZXC.SetMainDbConnDatabaseName(VvSQL.GetDbNameForThisTableName(Faktur.recordName));

      List<ZXC.VvUtilDataPackage> theTtAndTtNumList;

      if(isSkyLab == false) ZXC.SetStatusText("A_FaktEx_without_Faktur");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.A_FaktEx_without_Faktur; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) // DELETE? 
      {
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind);
         if(isSkyLab == false)
         {
            result = MessageBox.Show("Da li zelite POBRISATI stavke [" + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result == DialogResult.Yes) sqlOK = VvDaoBase.SqlDeleteOrphanRows(TheDbConnection, MySqlCheck_Kind);
         }
      }

      if(isSkyLab == false) ZXC.SetStatusText("B_Faktur_without_KupdobCD_OR_FaktEx");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.B_Faktur_without_KupdobCD_OR_FaktEx; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem)
      {
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
      }

      if(isSkyLab == false) ZXC.SetStatusText("C_Faktur_withMore_FaktEx");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.C_Faktur_withMore_FaktEx; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) 
      { 
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
       //OK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
         if(isSkyLab == false)
         {
            result = MessageBox.Show("Da li zelite POBRISATI stavke [" + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result == DialogResult.Yes) sqlOK = VvDaoBase.SqlDeleteTooManyFaktExRows(TheDbConnection, MySqlCheck_Kind, theTtAndTtNumList);
         }
      }

      if(isSkyLab == false) ZXC.SetStatusText("D_Rtrans_without_Faktur");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.D_Rtrans_without_Faktur; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) // DELETE? 
      {
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind);
         if(isSkyLab == false)
         {
            result = MessageBox.Show("Da li zelite POBRISATI stavke [" + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result == DialogResult.Yes) sqlOK = VvDaoBase.SqlDeleteOrphanRows(TheDbConnection, MySqlCheck_Kind);
         }
      }

      if(isSkyLab == false) ZXC.SetStatusText("E_Rtrans_duplicates");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.E_Rtrans_duplicates; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) 
      { 
       //GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind);
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
         if(isSkyLab == false)
         {
            // 08.08.2025: 
            //result = MessageBox.Show("Da li zelite POBRISATI stavke [" + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(ZXC.IsTEXTHOshop)
            {
               result = DialogResult.Yes;
            }
            else // classic / default 
            {
               result = MessageBox.Show("Da li zelite POBRISATI stavke [" + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }

            if(result == DialogResult.Yes)
            {
               var distinctTtAndTtNum = theTtAndTtNumList.Distinct();
               Faktur inProblemfaktur_rec; bool E_OK;
               int dupErrCount;
               foreach(ZXC.VvUtilDataPackage udp in distinctTtAndTtNum)
               {
                  do
                  {
                     inProblemfaktur_rec = new Faktur();
                     E_OK = FakturDao.SetMeFaktur(TheDbConnection, inProblemfaktur_rec, udp.TheStr1, udp.TheUint, false);
                     inProblemfaktur_rec.VvDao.LoadTranses(TheDbConnection, inProblemfaktur_rec, false);
                     dupErrCount = inProblemfaktur_rec.TransDuplicatesCount();
                     if(E_OK && dupErrCount.NotZero())
                     {
                        sqlOK = VvDaoBase.SqlDeleteDuplicateTranses(TheDbConnection, MySqlCheck_Kind, inProblemfaktur_rec.RecID).NotZero();
                     }
                  } while(dupErrCount.NotZero());
               } // foreach(ZXC.VvUtilDataPackage udp in distinctTtAndTtNum) 
            } // if(result == DialogResult.Yes) 
         } // if(isSkyLab == false) 
      }

      if(isSkyLab == false) ZXC.SetStatusText("F_Faktur_without_vvLogLAN");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.F_Faktur_without_vvLogLAN; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem)
      {
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
      }

      if(isSkyLab == false) ZXC.SetStatusText("G_Rtrans_without_TwinRtr");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.G_Rtrans_without_TwinRtr; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) { GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind); }

      if(isSkyLab == false) ZXC.SetStatusText("H_vvLogERR_xy_exists");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.H_vvLogERR_xy_exists; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem) // let's abandon this in case of 'Connection must be valid and open' case only 
      {
         if(ZXC.IsTEXTHOsky) // jer tu ne znas koji konkretni vvlogERR_XY je u pitanju 
         {
            string errMessage = "";
            if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
            GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
         }
         else
         {
            List<VvSQL.VvSkyLogEntry> today_errLogList = VvDaoBase.Get_TODAY_ErrLogEntryList(TheDbConnection);
            
            // 29.10.2018: 
            if(today_errLogList.NotEmpty()) GeneralOK = false; 

            // 29.10.2018: mijenjamo logiku 'autoatskog' brisanja sadrzaja ERRLogFajla: ne brisemo vise nista, 
            // nego provjeravami i javljamo samo DNEVNO svjeze errore.                                         
            //today_errLogList.RemoveAll(err => err.syncMessage.ToLower().Contains("Connection must be valid and open"       .ToLower()));
            //today_errLogList.RemoveAll(err => err.syncMessage.ToLower().Contains("ThisClient Doesn't have SkyLabConnection".ToLower()));
            //today_errLogList.RemoveAll(err => err.syncMessage.ToLower().Contains("SEND...errTS:"                           .ToLower()));
            //today_errLogList.RemoveAll(err => err.syncMessage.ToLower().Contains("Akcija RWTZa SENDERa"                    .ToLower()));
            //today_errLogList.RemoveAll(err => err.Is_Shop_SEND_ADDaction_IRM); // pretposavljas da ce neuspjeh sendanja IRMa prije ili kasnije biti resendan ili otkriven? 

            //bool today_errLogListIsEmpty = today_errLogList.IsEmpty();

            if(false/*today_errLogListIsEmpty*/) // od 29.10.2018. vise ne brisemo ERRlogTable 
            {
               sqlOK = VvSQL.DROP_TABLE(TheDbConnection, TheDbConnection.Database, ZXC.vvDB_ERRlogTableName);
            }
            else
            {
               // 17.09.2018: zanemari, ne javljaj, isplivati ce, valjda, na mail 
             //GeneralOK = false;
             //ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.", MySqlCheck_Kind);

               if(isProgramLoad == false)
               {
                  List<VvSkyLab.VvSyncInfo> syncInfoList = new List<VvSkyLab.VvSyncInfo>();

                  ZXC.ErrorsList = new List<string>();

                  VvSkyLab.VvSyncInfo syncInfo;

                  string errMsg2publish;

                  foreach(VvSQL.VvSkyLogEntry errorLogEntry in today_errLogList)
                  {
                     syncInfo         = new VvSkyLab.VvSyncInfo();
                     syncInfo.SkyRule = new SkyRule();

                     syncInfo.SqlErrMessage     = "errTS: " + errorLogEntry.skyLogTS.ToString(ZXC.VvDateAndTimeFormat);
                     syncInfo.SqlErrNo          = (int)errorLogEntry.syncErrNo;
                     syncInfo.WantedOperation   = errorLogEntry.operation;
                     syncInfo.SkyRule.Record    = errorLogEntry.record;
                     syncInfo.SkyRule.DocumTT   = errorLogEntry.tt;
                     syncInfo.SkyRule.LanSrvID  = errorLogEntry.origSrvID;
                     syncInfo.SkyRule.LanRecID  = errorLogEntry.origRecID;
                     syncInfo.SkyRule.OrigRecID = errorLogEntry.currRecID;

                     syncInfo.VvDataRecordInfo = errorLogEntry.syncMessage;

                     syncInfo.ResultingSrvRecIDaction = new ZXC.DBactionForSrvRecID(errorLogEntry.currRecID, errorLogEntry.origSrvID, errorLogEntry.origRecID, errorLogEntry.action);

                     syncInfoList.Add(syncInfo);

                     errMsg2publish = string.Format("{0} {1} {2} {3} {4} ", syncInfo.SqlErrMessage, syncInfo.ResultingSrvRecIDaction.action, syncInfo.SkyRule.Record, syncInfo.SkyRule.DocumTT, syncInfo.VvDataRecordInfo);

                     ZXC.ErrorsList.Add(errMsg2publish);

                   //if(isSkyLab == true) ZXC.aim_log("{0}...{1}...{2}", syncInfo.WantedOperation, syncInfo.SqlErrMessage, syncInfo.VvDataRecordInfo);
                     if(isSkyLab == true) ZXC.aim_log(errMsg2publish);
                  }

                //if(isSkyLab == false                      ) VvSkyLab.ReportSynchronyzeWithSKYResults(syncInfoList, ZXC.SkyOperation.NONE, true, DateTime.MinValue, DateTime.MinValue, true, true);
                  if(isSkyLab == false && GeneralOK == false) VvSkyLab.ReportSynchronyzeWithSKYResults(syncInfoList, ZXC.SkyOperation.NONE, true, DateTime.MinValue, DateTime.MinValue, true, true);
                  //else TODO; smisli kako ces VvJanitoru (SAMO JANITORU? a VvSkyLab-u?) prenijeti syncInfoList da bi onda sebi na mail poslao
                  // saszeto skraceno:
                  // D:\QCS_2015\Vektor\VvUC\PrjUC\SkyRuleUC.cs: private void PutDgvLineFields(int rowIdx, VvSkyLab.VvSyncInfo syncInfo)

#region DELETE vvLogERR

                  // 29.10.2018: mijenjamo logiku 'autoatskog' brisanja sadrzaja ERRLogFajla: ne brisemo vise nista, 
                  //if(isSkyLab == false) result = MessageBox.Show("Da li zelite POBRISATI vvLogERR - " + MySqlCheck_Kind + "?!", "Potvrdite BRISANJE?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                  //else                  result = System.Windows.Forms.DialogResult.No;
                                          result = System.Windows.Forms.DialogResult.No;

                  if(result == DialogResult.Yes)
                  {
                     sqlOK = VvSQL.DROP_TABLE(TheDbConnection, TheDbConnection.Database, ZXC.vvDB_ERRlogTableName);

                     string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

                     ZXC.TheSkyDbConnection.ChangeDatabase(TheDbConnection.Database);
                     sqlOK = VvSQL.DROP_TABLE(ZXC.TheSkyDbConnection, TheDbConnection.Database, ZXC.vvDB_CpyERRlogTableName);
                     ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

                  }

#endregion DELETE vvLogERR

               }  // if(sender.ToString() != "PROGRAM LOAD") 

               else if(ZXC.IsTEXTHOshop) // this IS Program LOAD and SHOP and LogERR exists 
               {                         // Let's duplicate SHOP's vvlogERR_XY to Cloud in SKY 
                  string origSkyDatabase = ZXC.TheSkyDbConnection.Database;
                  ZXC.TheSkyDbConnection.ChangeDatabase(TheDbConnection.Database);

                  sqlOK = VvSQL.DROP_TABLE(ZXC.TheSkyDbConnection, TheDbConnection.Database, ZXC.vvDB_CpyERRlogTableName);

                  foreach(VvSQL.VvSkyLogEntry errLogEntry in today_errLogList)
                  {
                     VvDaoBase.Insert_SKYorERR_LogEntry_EXECUTION(ZXC.TheSkyDbConnection, true, errLogEntry, true);
                  }

                  ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);
               }
            }
         }
      } // "H_vvLogERR_xy_exists" 

      if(isSkyLab == false) ZXC.SetStatusText("I_Rtrans_with_wrong_TwinID");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.I_Rtrans_with_wrong_TwinID; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem)
      {
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
      }

      if(isSkyLab == false) ZXC.SetStatusText("J_FakturSUM_vs_RtransSUM");
      MySqlCheck_Kind = ZXC.MySqlCheck_Kind.J_FakturSUM_vs_RtransSUM; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
      if(hasProblem)
      {
         string errMessage = "";
         if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
         GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
      }

      if(ZXC.IsTEXTHOshop == false)
      {
         if(isSkyLab == false) ZXC.SetStatusText("K_TwinRtransVsArtstatCij");
         MySqlCheck_Kind = ZXC.MySqlCheck_Kind.K_TwinRtransVsArtstatCij; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
         if(hasProblem)
         {
            string errMessage = "";
            if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
            GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);

#region M_MsiMsu_Roundtrip

            if(isSkyLab == false) ZXC.SetStatusText("M_MsiMsu_Roundtrip");
            MySqlCheck_Kind = ZXC.MySqlCheck_Kind.M_MsiMsu_Roundtrip; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
            if(hasProblem)
            {
               /*string*/ errMessage = "";
               if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
               GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
            }

#endregion M_MsiMsu_Roundtrip

         }
      }
      if(ZXC.CURR_prjkt_rec.IsFiskalOnline || ZXC.IsTEXTHOcentrala)
      {
         if(isSkyLab == false) ZXC.SetStatusText("L_NotFiskalizedIRMs");
         MySqlCheck_Kind = ZXC.MySqlCheck_Kind.L_NotFiskalizedIRMs; hasProblem = VvDaoBase.SqlSomeCheckQuery(TheDbConnection, MySqlCheck_Kind, out theTtAndTtNumList);
         if(hasProblem)
         {

            // 18.05.2017: START 

            Faktur fiskFaktur_rec;

            // 12.09.2017: tek! (prije bi na kraju uvijek bilo OK, ako je 'L_NotFiskalizedIRMs' OK!) 
            bool setMeOK;

            for(int idx = 0; idx < theTtAndTtNumList.Count; ++idx)
            {
               fiskFaktur_rec = new Faktur();
               setMeOK = FakturDao.SetMeFaktur(TheDbConnection, fiskFaktur_rec, theTtAndTtNumList[idx].TheStr1, theTtAndTtNumList[idx].TheUint, false);
               // 01.02.2018: 
             //if(setMeOK && fiskFaktur_rec.IsFiskalDutyFaktur_ONLINE == false                                 ) // ova faktura niti nije trebala biti fiskalizirana 
               if(setMeOK && fiskFaktur_rec.IsFiskalDutyFaktur_ONLINE == false && ZXC.IsTEXTHOcentrala == false) // ova faktura niti nije trebala biti fiskalizirana 
               {                                                                  // da provjerava i na THcentrali, jer je njoj IsFiskalDutyFaktur_ONLINE == false   
                //theTtAndTtNumList.RemoveAt(udp);
                  theTtAndTtNumList.Remove(theTtAndTtNumList[idx--]); // !!! 
               }
            } // for(int idx = 0; idx < theTtAndTtNumList.Count; ++idx) 

            hasProblem = theTtAndTtNumList.Count.NotZero();

            // 18.05.2017:  END  

            if(hasProblem) // if dodan tek 18.05.2017: 
            {
               string errMessage = "";
               if(isProgramLoad == false) errMessage = GetErrMessageList(theTtAndTtNumList);
               GeneralOK = false; ZXC.aim_emsg(MessageBoxIcon.Warning, "{0}\n\nKontaktirajte SUPPORT.\n\n{1}", MySqlCheck_Kind, errMessage);
            } // if(hasProblem) // if dodan tek 18.05.2017: 

         } // if(hasProblem) old 

      } // if(ZXC.CURR_prjkt_rec.IsFiskalOnline || ZXC.IsTEXTHOcentrala) 

      // ================================================================================================================================================ 
      // ================================================================================================================================================ 
      // ================================================================================================================================================ 

      ZXC.SetMainDbConnDatabaseName(origDatabase);

      if(isSkyLab == false) Cursor.Current = Cursors.Default;
      if(isSkyLab == false) ZXC.ClearStatusText();

      if(GeneralOK)
      {
         if(isProgramLoad == false) ZXC.aim_emsg(MessageBoxIcon.Information, "Sve je OK.");
      }
      else
      {
         // 29.10.2018: 
                                  //ZXC.aim_emsg(MessageBoxIcon.Warning, "SQL provjera gotova. Otkrivene su greške.\n\nKontaktirajte SUPPORT.");
         if(isProgramLoad == false) ZXC.aim_emsg(MessageBoxIcon.Warning, "SQL provjera gotova. Otkrivene su greške.\n\nKontaktirajte SUPPORT.");
      }

      return GeneralOK;
   }

   internal static string GetErrMessageList(List<ZXC.VvUtilDataPackage> theTtAndTtNumList)
   {
      int count = 0, maxCount = 10;
      string errMessage = "";

      foreach(ZXC.VvUtilDataPackage udp in theTtAndTtNumList)
      {
         if(++count <= maxCount)
         {
            errMessage += "stavka: " + udp.TheStr1 + "-" + udp.TheUint + " ---------------------------------------\n";
         }
         else
         {
            errMessage += "\n... i još [" + (theTtAndTtNumList.Count - count + 1).ToString() + "] stavaka ...";
            break;
         }
      }

      return errMessage;
   }

   private void Check_SYNC_WithSKY_1(object sender, EventArgs e) { Check_SYNC_WithSKY_JOB(sender, e, 1); }
   private void Check_SYNC_WithSKY_2(object sender, EventArgs e) { Check_SYNC_WithSKY_JOB(sender, e, 2); }
   private void Check_SYNC_WithSKY_3(object sender, EventArgs e) { Check_SYNC_WithSKY_JOB(sender, e, 3); }

   public bool Check_SYNC_WithSKY_JOB(object sender, EventArgs e, uint grouppingLevel)
   {
      #region Init

      // ako je 'isSkyLab == true' znaci to je SAMO TEXTHOshop implicitno kod ulaska u prg do 9:59 ujutro 
      bool isSkyLab = sender.ToString() == "SkyLab";

      if(ZXC.SENDorRECEIVE_SKY_InProgress) { ZXC.aim_emsg(MessageBoxIcon.Warning, "Akcija odbijena. Pokušajte malo kasnije ponovno.\n\nNeka druga,\n\nvjerovatno background,\n\nsync operacija u tijeku."); return true; } // neka druga, vjerovatno background, sync operacija u tijeku 

      string origLanDatabase = TheDbConnection.Database;
      string origSkyDatabase = ZXC.TheSkyDbConnection.Database;

      //List<VvReportSourceUtil> LANlist = new List<VvReportSourceUtil>();
      //List<VvReportSourceUtil> SKYlist = new List<VvReportSourceUtil>();

      int rowIdx = -1;
      string skladCD = "";
      string tt = "";

      DateTime dokDate = DateTime.MinValue;

      SIN_UC theUC = isSkyLab ? null : TheVvUC as SIN_UC;

      #endregion Init

      if(grouppingLevel > 1)
      {
         rowIdx = theUC.TheG.CurrentRow != null ? theUC.TheG.CurrentRow.Index : -1;

         if(rowIdx.IsNegative()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Molim, selektirajte redak"); return false; }

         skladCD = theUC.TheG.GetStringCell(theUC.DgvCI.iT_skladCD, rowIdx, false);
         tt      = theUC.TheG.GetStringCell(theUC.DgvCI.iT_tt     , rowIdx, false);

         if(grouppingLevel > 2) dokDate = theUC.TheG.GetDateCell(theUC.DgvCI.iT_skladDate, rowIdx, false);
      }

      /*=*/
      /*=*/
      List<VvSkyLab.CheckSyncPair> checkSyncPairList = VvSkyLab.Get_SYNC_WithSKY_Lists(TheDbConnection, ZXC.TheSkyDbConnection, grouppingLevel, skladCD, tt, dokDate);
      /*=*/

      // 24.10.2016: za SHOP-ove javi samo sync problem sa IRM-ovima, jer bez ovoga                               
      // javlja i npr. MSI, TRI, VMI koje je neka druga poslovnica SEND-ala nakon sto je ova pocela se RECEIVE-om 
      if(isSkyLab) // ...  znaci to je SAMO TEXTHOshop implicitno kod ulaska u prg do 9:59 ujutro                 
      {
         checkSyncPairList.RemoveAll(csp => csp.tt != Faktur.TT_IRM);
      }

      if(checkSyncPairList == null || checkSyncPairList.Count.IsZero()) 
      {
         if(isSkyLab == false)
         {
            theUC.TheG.Rows.Clear();
            ZXC.aim_emsg(MessageBoxIcon.Information, "Sve je sinhronizirano.");
         }
         else
         {
            ZXC.aim_log("Sve je sinhronizirano.");
         }
         
         return true; 
      }

      if(isSkyLab == false) // NOT SkyLab 
      {
         theUC.PutDgvFields1(checkSyncPairList);
      }

      else // this is SkyLab ...  znaci to je SAMO TEXTHOshop implicitno kod ulaska u prg do 9:59 ujutro 
      {
         ZXC.aim_log("Sync Diff Start:\n");
         foreach(VvSkyLab.CheckSyncPair checkSyncPair in checkSyncPairList)
         {
            ZXC.aim_log("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
               checkSyncPair.skladCD                        ,
               checkSyncPair.tt                             ,
               checkSyncPair.lanCount    .ToString  ("0;;#"),
               checkSyncPair.skyCount    .ToString  ("0;;#"),
               checkSyncPair.lanSumKol   .ToStringVv(      ),
               checkSyncPair.skySumKol   .ToStringVv(      ),
               checkSyncPair.lanSumKolCij.ToStringVv(      ),
               checkSyncPair.skySumKolCij.ToStringVv(      ));

            // 08.08.2025: auto add LANlogEntry_RWT 
            #region auto add LANlogEntry_RWT

            if(ZXC.IsTEXTHOshop) // if korigiran 20.12.2025. 
            {
               bool   setMeFaktur_OK;
               bool   forceLANlogEntry_RWT_OK;

               string unsyncMessage = "";

               Faktur unsyncIRMfakur_vvDataRecord;

               List<VvSkyLab.CheckSyncPair> checkSyncPairList_2 = VvSkyLab.Get_SYNC_WithSKY_Lists(TheDbConnection, ZXC.TheSkyDbConnection, 2 /*grouppingLevel*/, checkSyncPair.skladCD, checkSyncPair.tt, checkSyncPair.skladDate);

               foreach(VvSkyLab.CheckSyncPair checkSyncPair_2 in checkSyncPairList_2)
               {
                  unsyncIRMfakur_vvDataRecord = new Faktur();

                  setMeFaktur_OK = FakturDao.SetMeFaktur(TheDbConnection, unsyncIRMfakur_vvDataRecord, checkSyncPair_2.tt, (uint)checkSyncPair_2.ttNum, true);

                  if(setMeFaktur_OK)
                  {
                     forceLANlogEntry_RWT_OK = VvSkyLab.ForceLANlogEntry_RWT(TheDbConnection, /*ZXC.TheSkyDbConnection, skyOperation,*/ unsyncIRMfakur_vvDataRecord);

                     unsyncMessage += checkSyncPair_2.ToString() + "\n";
                  }
               }

               #region posalji mi mail da znam da radi

               VvMailClient mailClient = new VvMailClient();

               mailClient.EmailFromPasswd      = ZXC.SkyLabEmailPassword; // !!! ovo moze biti i prazno! Tada ida anonymously?! 
               mailClient.MailHost             = ZXC.ViperMailHost;
               mailClient.EmailFromAddress     = ZXC.SkyLabEmailAddress;
               mailClient.EmailFromDisplayName = "TH " + ZXC.vvDB_ServerID + " " + ZXC.SkyLabEmailFromDisplayName;
               mailClient.MessageSubject       = "TH " + checkSyncPair.skladCD + " AUTO ForceLANlogEntry_RWT";

               mailClient.MessageBody = unsyncMessage;

               mailClient.SendMail_Normal(false, ZXC.VektorEmailAddress); // silent send 

               #endregion posalji mi mail da znam da radi

            }

            #endregion auto add LANlogEntry_RWT

            #region SEND To Sky

            // 08.08.2025: auto add LANlogEntry_RWT 
            SEND_ToSKY_JOB(sender, e);

            #endregion SEND To Sky

         } // foreach(VvSkyLab.CheckSyncPair checkSyncPair in checkSyncPairList)

         ZXC.aim_log("Sync Diff End.\n");
      }

#region Final

      TheDbConnection.ChangeDatabase(origLanDatabase);
      ZXC.TheSkyDbConnection.ChangeDatabase(origSkyDatabase);

#endregion Final

      return false; // isOK 
   }

   private void CheckDownloadSpeed(object sender, EventArgs e) 
   {
      VvAboutBox.CheckSpeed(sender, e);
   }

   #endregion All About SKY

   #region SVD or TH DatabaseService (TH newEUR, SVD donacije set PrNabCij)

   /*private*/ public void DatabaseService(object sender, EventArgs e)
   {
      if(ZXC.CURR_userName != ZXC.vvDB_systemSuperUserName &&
         ZXC.CURR_userName != ZXC.vvDB_programSuperUserName && !ZXC.CURR_user_rec.IsSuper)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Privilegirana akcija. Odbijeno.");
         return;
      }

      if(ZXC.IsSvDUH)
      {
         DialogResult result = MessageBox.Show("Da li zaista zelite SvDUH_CheckPrNabCij_IZDskl20only?!", "Potvrdite SvDUH_CheckPrNabCij_IZDskl20only?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;

       //RtransDao.SvDUH_CheckPrNabCij_IZDsklDonacijeOnly(TheDbConnection, "20");
         RtransDao.SvDUH_CheckPrNabCij_IZDsklDonacijeOnly(TheDbConnection, "22");
       //RtransDao.SvDUH_CheckPrNabCij_IZDsklDonacijeOnly(TheDbConnection, "24");

       //RtransDao.SvDUH_CheckPrNabCij_IZDsklDonacijeOnly(TheDbConnection, "26");
      }

      else if(ZXC.IsTEXTHOcentrala || ZXC.IsRipleyOrKristal)
      {

         DialogResult result = MessageBox.Show("Da li zaista zelite VvGenesisOvoOno?!", "Potvrdite VvGenesisOvoOno?!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

         if(result != DialogResult.Yes) return;

         // here we go: 
         Cursor.Current = Cursors.WaitCursor;

#if bilo_pri_prelasku_na_euro

         TH_New_EURO_ArtiklSifrar_Genesis(TheDbConnection); // ovaj ide u 2023 ... po novome jer Init_NY napravi prije u 2022 

         TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);

         ZXC.RISK_DisableCacheTemporarily = true;

         TH_New_EURO_NultiZPC_Genesis(TheDbConnection); // i ovaj ide u 2023 
#endif
         TH_New_Artikls_And_NultiZPCrtranses_Genesis_V2(TheDbConnection);

         Cursor.Current = Cursors.Default;
      }
   }

#if bilo_pri_prelasku_na_euro
   private void TH_New_EURO_NultiZPC_Genesis(XSqlConnection conn)
   {
      var malop2week_PON_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_PON_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop2week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop5weekSkladCDlist      = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_5WeekShop     (lui.Cd)).Select(l => l.Cd).ToList();
      var malop3week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_3Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      
      List<string> skladCDlist = new List<string>();
      int newZpcRtransListCount;
      int fakturCount = 0;
      int transCount  = 0;
      string statusText;
      DateTime nultiZPCDate = ZXC.Date01012023;

      skladCDlist = new List<string>();
      malop5weekSkladCDlist     .RemoveAll(skl => ZXC.IsTH_DEAD_Shop(skl));
      malop3week_SRI_SkladCDlist.RemoveAll(skl => ZXC.IsTH_DEAD_Shop(skl));

      skladCDlist.AddRange(malop5weekSkladCDlist     );
      skladCDlist.AddRange(malop3week_SRI_SkladCDlist);

      foreach(string malopSkladCD in skladCDlist)
      {
         if(ZXC.IsTH_DEAD_Shop(malopSkladCD)) continue;

         newZpcRtransListCount = TH_CreateZPC(nultiZPCDate, malopSkladCD, nultiZPCDate, false, true);

         if(newZpcRtransListCount.NotZero())
         {
            fakturCount++;
            transCount += newZpcRtransListCount;
         }

         statusText = String.Format("Done {0} / {1} [{2}]", fakturCount, skladCDlist.Count(), malopSkladCD);

         ZXC.SetStatusText(statusText); if(Cursor.Current != Cursors.WaitCursor) Cursor.Current = Cursors.WaitCursor;
         ZXC.aim_log(statusText);

      } // foreach(string malopSkladCD in skladCDlist) 

      ZXC.ClearStatusText();

      TheVvRecordUC.ThePrefferedRecordSorter = Faktur.sorterDokDate;

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Kreirao\n\n{0} ZPCa.\n\nUkupno {1} novih stavaka", fakturCount, transCount);
   }
#endif

   internal static string artMadeIn_EUR  = "NEW_EUR" ;
   internal static string artMadeIn_Kuna = "OLD_Kuna";
   private void TH_DELREC_OldNiceKunaArtikls_2023(XSqlConnection conn)
   {
      bool OK;
      int count = 0;

      #region Get 2023 ArtiklList

      List<Artikl> oldNiceKunaIn2023_ArtiklList = new List<Artikl>();
      List<VvSqlFilterMember> filterMembers = new List<VvSqlFilterMember>(3);
      filterMembers.Add(new VvSqlFilterMember(ZXC.ArtiklSchemaRows[ZXC.ArtCI.madeIn  ], "madeIn", artMadeIn_EUR, " != "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.ArtiklSchemaRows[ZXC.ArtCI.grupa3CD], "gr3cd" , "NBKG"       , " != "));
      filterMembers.Add(new VvSqlFilterMember(ZXC.ArtiklSchemaRows[ZXC.ArtCI.ts      ], "ts"    , "USL"        , " != "));

      VvDaoBase.LoadGenericVvDataRecordList<Artikl>(conn, oldNiceKunaIn2023_ArtiklList, filterMembers, "", "artiklCD", false);

      #endregion Get 2023 ArtiklList

    //var OLD_HRD_MP_artiklList = VvUserControl.ArtiklSifrar > daj2023
    //   .Where(art => art.MadeIn   != artMadeIn_EUR      && 
    //                 art.Grupa3CD != "NBKG"             && 
    //                 art.TS       != "USL"               );

      foreach(Artikl artikl_rec in /*OLD_HRD_MP_artiklList*/oldNiceKunaIn2023_ArtiklList)
      {
         OK = TheVvDao.DELREC(conn, artikl_rec, false);

         if(OK) ++count;
      }

      ZXC.aim_emsg(MessageBoxIcon.Information, "POBRISAO {0} starih Kuna artikala u 2023.", count);
   }

   private void TH_DELREC_NewNiceEuroArtikls(XSqlConnection conn)
   {
      bool OK;
      int count = 0;

      var NEW_EUR_MP_artiklList = VvUserControl.ArtiklSifrar
         .Where(art => art.MadeIn   == artMadeIn_EUR      && 
                       art.Grupa3CD != "NBKG"             && 
                       art.TS       != "USL"               );

      foreach(Artikl artikl_rec in NEW_EUR_MP_artiklList)
      {
         OK = TheVvDao.DELREC(conn, artikl_rec, false);

         if(OK) ++count;
      }

      ZXC.aim_emsg(MessageBoxIcon.Information, "POBRISAO {0} novih EURO artikala u 2022.", count);
   }

   private void TH_RWTREC_OldNiceKunaArtikls(XSqlConnection conn)
   {
      bool OK;
      decimal kuneMoney;
      int count = 0;

      Artikl newEuroArtikl_rec;

      var OLD_HRD_MP_artiklList = VvUserControl.ArtiklSifrar
         .Where(art => art.MadeIn != artMadeIn_EUR && (art.Grupa3CD == "PRKOM" || (art.Grupa3CD == "NBiPR" && art.Grupa1CD != "Akat" && art.ArtiklCD.Length == 6)));

      ZXC.ErrorsList = new List<string>();

      Cursor.Current = Cursors.WaitCursor;

      foreach(Artikl artikl_rec in OLD_HRD_MP_artiklList)
      {
         BeginEdit(artikl_rec);

         if(artikl_rec.Grupa3CD == "NBiPR" && artikl_rec.Grupa1CD != "Akat" && artikl_rec.ArtiklCD.Length == 6) // vrecice npr 'VR0200' 
         {
            kuneMoney = ZXC.ValOrZero_Decimal(artikl_rec.ArtiklCD.SubstringSafe(2, 4), 0) / 100.00M;
         }
         else // classic MP artikli npr 'AC0001'
         {
            kuneMoney = ZXC.ValOrZero_Decimal(artikl_rec.ArtiklCD.SubstringSafe(2, 4), 0);
         }

         artikl_rec.MadeIn     = artMadeIn_Kuna                         ; // za lakse filtriranje u SQL recenicama 
         artikl_rec.Placement  = artikl_rec.ArtiklCD.SubstringSafe(0, 2); // rootName                              
         artikl_rec.Starost    =                     kuneMoney          ; // real kune                             

         newEuroArtikl_rec = VvSkyLab.Get_newNiceEuroArtikl(artikl_rec.ArtiklCD, VvUserControl.ArtiklSifrar);

         if(newEuroArtikl_rec != null)
         { 
          //artikl_rec.ImportCij   = ZXC.EURiIzKuna_HRD_(kuneMoney); // ružni euri 
            artikl_rec.ImportCij   = newEuroArtikl_rec.ImportCij   ; // nice euri  
            artikl_rec.ArtiklCD2   = newEuroArtikl_rec.ArtiklCD    ; // oldKunski artikl sada zna na kojega ce newEuro artikla zavrsiti 
            artikl_rec.ArtiklName2 = newEuroArtikl_rec.ArtiklName  ; // oldKunski artikl sada zna na kojega ce newEuro artikla zavrsiti 

            OK = /*TheVvDao*/artikl_rec.VvDao.RWTREC(conn, artikl_rec);
         }
         else
         {
            ZXC.ErrorsList.Add(String.Format("{0} NEMA newEuroArtikl_rec", artikl_rec));
            OK = false;
         }

         EndEdit(artikl_rec);

         if(OK) ++count;

      }
      
      Cursor.Current = Cursors.Default;

      if(ZXC.ErrorsList.NotEmpty())
      {
         ZXC.aim_emsg_List(string.Format("{0} artikala bez 'EURo' pandana.", ZXC.ErrorsList.Count), ZXC.ErrorsList);
      }

      ZXC.ErrorsList = null;

      ZXC.aim_emsg(MessageBoxIcon.Information, "Preimenovao {0} starih artikala.", count);
   }

   private void TH_New_EURO_ArtiklSifrar_Genesis(XSqlConnection conn)
   {
      int count = 0;

      decimal increment0_25 = 0.25M;
      decimal increment0_50 = 0.50M;
      decimal increment1_00 = 1.00M;
      decimal increment5_00 = 5.00M;

      count += TH_Addrec_EUR_Artikl_Range(conn, "AC",  3.00M,  10.00M, increment0_50, "Akat", "CIP", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AC", 11.00M,  20.00M, increment1_00, "Akat", "CIP", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AC", 25.00M,  60.00M, increment5_00, "Akat", "CIP", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AD",  0.50M,   5.00M, increment0_25, "Akat", "ODD", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AD",  5.50M,  10.00M, increment0_50, "Akat", "ODD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AE", 10.00M,  15.00M, increment0_50, "Akat", "ODE", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AE", 16.00M,  20.00M, increment1_00, "Akat", "ODE", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AE", 25.00M, 120.00M, increment5_00, "Akat", "ODE", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AM",  0.50M,   5.00M, increment0_25, "Akat", "MOD", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AM",  5.50M,  10.00M, increment0_50, "Akat", "MOD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AO",  0.50M,   5.00M, increment0_25, "Akat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AO",  5.50M,  20.00M, increment0_50, "Akat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AO", 21.00M,  50.00M, increment1_00, "Akat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AO", 55.00M, 135.00M, increment5_00, "Akat", "ODN", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AR",  3.00M,  10.00M, increment0_50, "Akat", "REM", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AR", 11.00M,  20.00M, increment1_00, "Akat", "REM", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "AT",  3.00M,  10.00M, increment0_50, "Akat", "TOR", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AT", 11.00M,  20.00M, increment1_00, "Akat", "TOR", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "AT", 25.00M,  60.00M, increment5_00, "Akat", "TOR", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BC",  1.50M,  10.00M, increment0_50, "Bkat", "CIP", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BC", 11.00M,  20.00M, increment1_00, "Bkat", "CIP", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BD",  0.50M,   5.00M, increment0_25, "Bkat", "ODD", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BD",  5.50M,  10.00M, increment0_50, "Bkat", "ODD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BM",  0.50M,   5.00M, increment0_25, "Bkat", "MOD", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BM",  5.50M,  10.00M, increment0_50, "Bkat", "MOD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BO",  0.50M,   5.00M, increment0_25, "Bkat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BO",  5.50M,  20.00M, increment0_50, "Bkat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BO", 21.00M,  50.00M, increment1_00, "Bkat", "ODN", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BR",  1.50M,  10.00M, increment0_50, "Bkat", "REM", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "BT",  1.50M,  10.00M, increment0_50, "Bkat", "TOR", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "BT", 11.00M,  20.00M, increment1_00, "Bkat", "TOR", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "CD",  0.50M,   5.00M, increment0_25, "Ckat", "ODD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "CM",  0.50M,   5.00M, increment0_25, "Ckat", "MOD", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "CO",  0.50M,   5.00M, increment0_25, "Ckat", "ODN", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "CO",  5.50M,  20.00M, increment0_50, "Ckat", "ODN", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "KT",  0.50M,  10.00M, increment0_50, "Bkat", "KUT", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "KT", 11.00M,  20.00M, increment1_00, "Bkat", "KUT", "PRKOM");
      count += TH_Addrec_EUR_Artikl_Range(conn, "KT", 25.00M,  50.00M, increment5_00, "Bkat", "KUT", "PRKOM");
                                          
      count += TH_Addrec_EUR_Artikl_Range(conn, "VR",  0.30M,   0.30M, increment1_00,     "", "VRE", "NBiPR"); // VRECICA! 

      ZXC.aim_emsg(MessageBoxIcon.Information, "Dodao {0} novih artikala.", count);

   }

   private int TH_New_Rtranses_NultiZPC_Genesis_V2(XSqlConnection conn, List<uint> nulti_ZPC_TtNumList, List<Artikl> artiklListALL)
   {
      var malop2week_PON_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_PON_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop2week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_2Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      var malop5weekSkladCDlist      = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_5WeekShop     (lui.Cd)).Select(l => l.Cd).ToList();
      var malop3week_SRI_SkladCDlist = ZXC.luiListaSkladista.Where(lui => lui.Flag == true && ZXC.IsTH_3Week_SRI_Shop(lui.Cd)).Select(l => l.Cd).ToList();
      
      List<string> skladCDlist = new List<string>();
      int newZpcRtransListCount = 0;
    //int fakturCount = 0;
    //int transCount  = 0;
      string statusText;
      DateTime nultiZPCDate = ZXC.Date01012023;

      skladCDlist = new List<string>();
      malop5weekSkladCDlist     .RemoveAll(skl => ZXC.IsTH_DEAD_Shop(skl));
      malop3week_SRI_SkladCDlist.RemoveAll(skl => ZXC.IsTH_DEAD_Shop(skl));

      skladCDlist.AddRange(malop5weekSkladCDlist     );
      skladCDlist.AddRange(malop3week_SRI_SkladCDlist);

      List<Artikl> artiklList_3 = artiklListALL.Where(art => art.Grupa1CD != "Akat").ToList();
      List<Artikl> theArtiklList;

      Rtrans lastRtrans_rec, newRtrans_rec;
      bool   rtrOK, fakOK;
      uint   nultiZPCttNum;
      ushort newSerial;
      bool   is3w;
      Faktur nultiZPCfaktur_rec;

      foreach(string malopSkladCD in skladCDlist)
      {
         if(ZXC.IsTH_DEAD_Shop(malopSkladCD)) continue;

         #region iz TH_CreateZPC

         lastRtrans_rec = new Rtrans();

         nultiZPCttNum = nulti_ZPC_TtNumList.Single(ttnum => (ttnum / 10000).ToString() == malopSkladCD.SubstringSafe(0, 2));

         rtrOK = FakturDao.SetMeLastRtransForTtAndTtNum(conn, lastRtrans_rec, Faktur.TT_ZPC, nultiZPCttNum, false);

         is3w = ZXC.IsTH_3Week_SRI_Shop(malopSkladCD);

         theArtiklList = is3w ? artiklList_3 : artiklListALL;

         if(rtrOK)
         {
            newSerial = lastRtrans_rec.T_serial;

            foreach(Artikl artikl_rec in theArtiklList)
            {
               newRtrans_rec = lastRtrans_rec.MakeDeepCopy();

               newRtrans_rec.T_serial     = ++newSerial          ;
               newRtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD  ;
               newRtrans_rec.T_artiklName = artikl_rec.ArtiklName;

               newRtrans_rec.T_noCijMal   = artikl_rec.ImportCij ;

               rtrOK = newRtrans_rec.VvDao.ADDREC(conn, newRtrans_rec, false, false, false, false);

               if(rtrOK) newZpcRtransListCount++;
            }

            // ForceRWTaction4_LogLAN 

            nultiZPCfaktur_rec = new Faktur();

            fakOK = FakturDao.SetMeFaktur(conn, nultiZPCfaktur_rec, Faktur.TT_ZPC, nultiZPCttNum, false);

            if(fakOK)
            {
               VvSkyLab.ForceLANlogEntry_RWT(TheDbConnection, nultiZPCfaktur_rec);
            }
         }

         #endregion iz TH_CreateZPC

         statusText = String.Format("Done [{0}]", malopSkladCD);

         ZXC.SetStatusText(statusText); if(Cursor.Current != Cursors.WaitCursor) Cursor.Current = Cursors.WaitCursor;
         ZXC.aim_log(statusText);

      } // foreach(string malopSkladCD in skladCDlist) 

      ZXC.ClearStatusText();

      TheVvRecordUC.ThePrefferedRecordSorter = Faktur.sorterDokDate;

      //ShowNews();

      //ZXC.aim_emsg(MessageBoxIcon.Information, "Gotovo. Ukupno {0} novih stavaka", newZpcRtransListCount);

      return newZpcRtransListCount;
   }
   private void TH_New_Artikls_And_NultiZPCrtranses_Genesis_V2(XSqlConnection conn)
   {
    //int artCount = 0;
    //int zpcCount = 0;
    //int artCountSum = 0;
      int zpcCountSum = 0;

      decimal increment0_25 = 0.25M;
      decimal increment0_50 = 0.50M;
      decimal increment1_00 = 1.00M;
      decimal increment5_00 = 5.00M;
      decimal increment0_20 = 0.20M;

      List<uint  > nulti_ZPC_TtNumList = VvDaoBase.GetNultiZpcTtNums(TheDbConnection);
      List<Artikl> artiklListALL       = new List<Artikl>();

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 5.25M, 9.75M, increment0_50, "Akat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 5.25M, 9.75M, increment0_50, "Akat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 5.25M, 9.75M, increment0_50, "Akat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 5.25M, 9.75M, increment0_50, "Bkat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 5.25M, 9.75M, increment0_50, "Bkat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 5.25M, 9.75M, increment0_50, "Bkat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "KT", 0.25M, 9.75M, increment0_50, "Bkat", "KUT", "PRKOM"));

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 0.20M, 0.80M, increment0_20, "Akat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 1.20M, 1.80M, increment0_20, "Akat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 2.20M, 2.80M, increment0_20, "Akat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 3.20M, 3.80M, increment0_20, "Akat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AD", 4.20M, 4.80M, increment0_20, "Akat", "ODD", "PRKOM"));

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 0.20M, 0.80M, increment0_20, "Akat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 1.20M, 1.80M, increment0_20, "Akat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 2.20M, 2.80M, increment0_20, "Akat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 3.20M, 3.80M, increment0_20, "Akat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AM", 4.20M, 4.80M, increment0_20, "Akat", "MOD", "PRKOM"));

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 0.20M, 0.80M, increment0_20, "Akat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 1.20M, 1.80M, increment0_20, "Akat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 2.20M, 2.80M, increment0_20, "Akat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 3.20M, 3.80M, increment0_20, "Akat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "AO", 4.20M, 4.80M, increment0_20, "Akat", "ODN", "PRKOM"));

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 0.20M, 0.80M, increment0_20, "Bkat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 1.20M, 1.80M, increment0_20, "Bkat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 2.20M, 2.80M, increment0_20, "Bkat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 3.20M, 3.80M, increment0_20, "Bkat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BD", 4.20M, 4.80M, increment0_20, "Bkat", "ODD", "PRKOM"));
      
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 0.20M, 0.80M, increment0_20, "Bkat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 1.20M, 1.80M, increment0_20, "Bkat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 2.20M, 2.80M, increment0_20, "Bkat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 3.20M, 3.80M, increment0_20, "Bkat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BM", 4.20M, 4.80M, increment0_20, "Bkat", "MOD", "PRKOM"));
      
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 0.20M, 0.80M, increment0_20, "Bkat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 1.20M, 1.80M, increment0_20, "Bkat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 2.20M, 2.80M, increment0_20, "Bkat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 3.20M, 3.80M, increment0_20, "Bkat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "BO", 4.20M, 4.80M, increment0_20, "Bkat", "ODN", "PRKOM"));

      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CD", 0.20M, 0.80M, increment0_20, "Ckat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CD", 1.20M, 1.80M, increment0_20, "Ckat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CD", 2.20M, 2.80M, increment0_20, "Ckat", "ODD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CD", 3.20M, 3.80M, increment0_20, "Ckat", "ODD", "PRKOM"));
      
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CM", 0.20M, 0.80M, increment0_20, "Ckat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CM", 1.20M, 1.80M, increment0_20, "Ckat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CM", 2.20M, 2.80M, increment0_20, "Ckat", "MOD", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CM", 3.20M, 3.80M, increment0_20, "Ckat", "MOD", "PRKOM"));
      
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CO", 0.20M, 0.80M, increment0_20, "Ckat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CO", 1.20M, 1.80M, increment0_20, "Ckat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CO", 2.20M, 2.80M, increment0_20, "Ckat", "ODN", "PRKOM"));
      artiklListALL.AddRange(TH_Addrec_Artikl_Range_V2(conn, nulti_ZPC_TtNumList, "CO", 3.20M, 3.80M, increment0_20, "Ckat", "ODN", "PRKOM"));

      TheVvUC.SetSifrarAndAutocomplete<Artikl>(null, VvSQL.SorterType.None);

      zpcCountSum = TH_New_Rtranses_NultiZPC_Genesis_V2(TheDbConnection, nulti_ZPC_TtNumList, artiklListALL);

      ShowNews();

      ZXC.aim_emsg(MessageBoxIcon.Information, "Dodao {0} novih artikala i {1} nulti zpc stavaka", artiklListALL.Count, zpcCountSum);
   }

   private int TH_Addrec_EUR_Artikl_Range(XSqlConnection conn, string rootName, decimal euroMoneyOD, decimal euroMoneyDO, decimal euroIncrement, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod)
   {
      int count = 0;
      bool OK;
      for(decimal euroMoney = euroMoneyOD; euroMoney <= euroMoneyDO; euroMoney += euroIncrement)
      {
         OK = VvSkyLab.TH_Addrec_EUR_Artikl(conn, rootName, euroMoney, gr1cd_kategorija, gr2cd_kind, gr3cd_nabILIprod);

         if(OK) ++count;
      }

      return count;
   }

   private List<Artikl> TH_Addrec_Artikl_Range_V2(XSqlConnection conn, List<uint> nulti_ZPC_TtNumList, string rootName, decimal euroMoneyOD, decimal euroMoneyDO, decimal euroIncrement, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod)
   {
      bool OK;
      Artikl artikl_rec;
      List<Artikl> artiklList = new List<Artikl>();

      for(decimal euroMoney = euroMoneyOD; euroMoney <= euroMoneyDO; euroMoney += euroIncrement)
      {
         (OK, artikl_rec) = VvSkyLab.TH_Addrec_EUR_Artikl_V2(conn, rootName, euroMoney, gr1cd_kategorija, gr2cd_kind, gr3cd_nabILIprod);

         if(OK)
         {
            artiklList.Add(artikl_rec);
         }

      }

      return artiklList;
   }

   // ovo se ne koristi nigdje 
   private Artikl Get_TH_EUR_Artikl_From_OLD_HRD_ArtiklCD(string old_hrd_ArtiklCD)
   {
      Artikl old_hrd_artikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(old_hrd_ArtiklCD);

      if(old_hrd_artikl_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji artikl sa old_hrd_ArtiklCD [{0}]", old_hrd_ArtiklCD);
         return null;
      }

      decimal oldHRDcij  = old_hrd_artikl_rec.Starost  ;
      decimal fixEURcij  = old_hrd_artikl_rec.ImportCij;
      decimal niceEURcij = ZXC.NiceEURo_0_25(fixEURcij); // da li 0_05, 0_25 ili 0_50 !!!??? 

      Artikl new_eur_artikl_rec = TheVvUC.Get_Artikl_FromVvUcSifrar(niceEURcij);

      if(new_eur_artikl_rec == null)
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ne postoji artikl sa niceEURcij [{0}]", niceEURcij);
         return null;
      }

      return new_eur_artikl_rec;
   }

#if nepotrebno_ali_primjer_za_aggragate
   private int TH_Addrec_EUR_Artikl_Range(string rootName, decimal euroMoneyOD, decimal euroMoneyDO, decimal euroIncrement, string gr1cd_kategorija, string gr2cd_kind, string gr3cd_nabILIprod)
   {
      int count = 0;
      bool OK;
      Artikl oldArtikl;
      decimal thisKuneMoney;

    //var oldArtiklList = VvUserControl.ArtiklSifrar.Where(artOLD => artOLD.MadeIn == artMadeIn_Kuna && artOLD.Placement == rootName);

      for(decimal euroMoney = euroMoneyOD; euroMoney <= euroMoneyDO; euroMoney += euroIncrement)
      {
         thisKuneMoney = ZXC.KuneIzEURa_HRD_(euroMoney);

         var oldArtiklList = VvUserControl.ArtiklSifrar.Where(artOLD => artOLD.MadeIn == artMadeIn_Kuna && artOLD.Placement == rootName && artOLD.Starost <= thisKuneMoney);

         oldArtikl = 

            oldArtiklList.Aggregate((currMinDiffArtikl, nextArtikl) => 
            
          //Math.Abs(currMinDiffArtikl.Starost - thisKuneMoney            ) < Math.Abs(nextArtikl.Starost - thisKuneMoney     ) ? currMinDiffArtikl : nextArtikl);
                    (thisKuneMoney             - currMinDiffArtikl.Starost) <         (thisKuneMoney      - nextArtikl.Starost) ? currMinDiffArtikl : nextArtikl);

         OK = VvSkyLab.TH_Addrec_EUR_Artikl(TheDbConnection, rootName, euroMoney, gr1cd_kategorija, gr2cd_kind, gr3cd_nabILIprod, oldArtikl.ArtiklCD);

         if(OK) ++count;
      }

      return count;
   }
#endif

   #endregion SVD or TH DatabaseService (TH newEUR, SVD donacije set PrNabCij)

   #region eRacun

#if dokSmoMisliliInaFINA
   private void RISK_Outgoing_eRacun_FINA(object sender, EventArgs e)
   {
      #region Init

      FakturDUC theDUC       = TheVvDocumentRecordUC as FakturDUC                     ;
      Faktur    faktur_rec   = theDUC.faktur_rec                                      ;
      Kupdob    kupdob_rec   = theDUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.KupdobCD)  ;
      Kupdob    primPlat_rec = theDUC.Get_Kupdob_FromVvUcSifrar(faktur_rec.PrimPlatCD);

      PrnFakDsc thePFD = ((VvRpt_RiSk_Filter)((VvRiskReport)theDUC.TheVvReport).VirtualRptFilter).PFD;

      //System.Text.Encoding encoding_noBOM = new System.Text.UTF8Encoding(false);
      //System.Text.Encoding encoding___BOM = new System.Text.UTF8Encoding(true );

      EN16931.UBL.InvoiceType the_eRacun = null;

      System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = null;

      byte[] xmlSignedByteArray = null;

      System.Text.Encoding encoding4UBL = ZXC.VvUTF8Encoding_noBOM;

      bool everythingOK = true;

    //string certPassword = "1q1q1Q"; // TODO: !!! 
      string certPassword = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted; // !!! 

      #endregion Init

      #region Check if is already successfuly sent

      if(faktur_rec.FiskPrgBr.ToUpper().Contains("OK: "))
      {
         Cursor.Current = Cursors.Default;
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno.\n\nOvaj račun je već uspješno poslan.");
         return;
      }

      #endregion Check if is already successfuly sent

      #region FileDialog

      SaveFileDialog saveFileDialog = new SaveFileDialog();

      saveFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eRacun_Izlaz_Prefs.DirectoryName;

      saveFileDialog.Title = "Kreiranje datoteke 'e-Račun za Državu'";
      saveFileDialog.Filter = "Xml e-Račun (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
      //saveFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      saveFileDialog.FilterIndex = 1;
      saveFileDialog.RestoreDirectory = true;
      saveFileDialog.DefaultExt = "xml";

      string suggestedFileName = faktur_rec.TT_And_TtNum + "-" + faktur_rec.DokDate_DDMMYY + "-" + faktur_rec.KupdobTK/*KupdobName*/;

      saveFileDialog.FileName = suggestedFileName.IsEmpty() ? "" : suggestedFileName + "." + saveFileDialog.DefaultExt;

      Cursor.Current = Cursors.Default;

      if(saveFileDialog.ShowDialog() != DialogResult.OK)
      {
         saveFileDialog.Dispose(); // !!! 
         Cursor.Current = Cursors.Default;
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      string fullPath_XML_FileName = saveFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPath_XML_FileName);

      string fileNameOnly = dInfo.Name;
      string directoryName = fullPath_XML_FileName.Substring(0, fullPath_XML_FileName.Length - (fileNameOnly.Length + 1));
      ZXC.TheVvForm.VvPref.eRacun_Izlaz_Prefs.DirectoryName = directoryName;

      saveFileDialog.Dispose(); // !!! 

      #endregion FileDialog

      try
      { 
         #region Create eRacun Object From Faktur

         Cursor.Current = Cursors.WaitCursor;

         the_eRacun = EN16931.UBL.InvoiceType.Create_eRacun_fromFaktur(faktur_rec, thePFD, kupdob_rec, primPlat_rec, VvUserControl.ArtiklSifrar, null, "");

         #endregion Create eRacun Object From Faktur

         #region Serialize

         string xmlString = the_eRacun.Serialize(encoding4UBL);

         #endregion Serialize

         #region Remove Empty Nodes

         XmlDocument xmlDocument = ZXC.RemoveEmptyNodes(xmlString);
         xmlString = xmlDocument.OuterXml;

         #endregion Remove Empty Nodes

         #region Get Certificate

         certificate = the_eRacun.Get_eRacun_Application_Certificate("VEKTOR"); // "VEKTOR" je 'Issued To' u mmc WindowsCertificateStore-u, kasnije mozes kao i Fiskalnoga iz MySQL-a 

         if(certificate == null)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom dohvata certifikata!");
            Cursor.Current = Cursors.Default;
            return;
         }

         #endregion Get Certificate 

         #region UBLextension - UBLsigner (By FINA - Jura)

         hr.fina.eracun.signature.UBLSigner ublSigner = null; // rem2025: new hr.fina.eracun.signature.UBLSigner(certificate, certPassword, "Invoice");
         //u memoryStream je xml prije potpisa 
         xmlSignedByteArray = ublSigner.signUBLDocument(encoding4UBL.GetBytes(xmlString));

         xmlString = encoding4UBL.GetString(xmlSignedByteArray);

         #endregion UBLextension - UBLsigner

         #region Validate eRacun XML File 

         MemoryStream memoryStream = new MemoryStream();

         xmlDocument.LoadXml(xmlString);

         xmlDocument.Save(memoryStream);

         bool validateOK = false;
         try { validateOK = EN16931.UBL.InvoiceType.ValidateThis_XML_eRacun(memoryStream); } catch(Exception ex) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message); }

         if((memoryStream != null))
         {
            memoryStream.Dispose();
         }

         #endregion Validate eRacun XML File 

         #region Format / Beautify XML (DEBUG ONLY)
#if DEBUG

         xmlString = ZXC.BeautifyXml(xmlString, encoding4UBL);

#endif
         #endregion Format / Beautify XML (DEBUG ONLY)

         #region Save eRacun XML File 

         bool saveOK = the_eRacun.VvSaveToFile(xmlString, fullPath_XML_FileName, encoding4UBL);

         #endregion Save eRacun XML File 

         #region Show XML (DEBUG ONLY)
         #if DEBUG

         try
         {
            System.Diagnostics.Process.Start(fullPath_XML_FileName);
         }
         catch(Exception ex)
         {
            ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
         }

         #endif
         #endregion Show XML (DEBUG ONLY)

      }
      catch(Exception ex) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška: {0}", ex.Message); everythingOK = false; }

      if(everythingOK == false)
      {
         Cursor.Current = Cursors.Default;
         return;
      }

      string sendingResult = "";
      try
      {
         sendingResult = the_eRacun.VvSendB2GOutgoingInvoiceViaPKIWebService(xmlSignedByteArray, certificate, certPassword);
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja WebService-a: {0}", ex.Message);
         everythingOK = false;
         sendingResult = ex.Message;
      }

      #region Rwtrec Feedback 

      BeginEdit(faktur_rec);

      faktur_rec.FiskPrgBr = ZXC.LenLimitedStr(sendingResult, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.fiskPrgBr));

      bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

      EndEdit(faktur_rec);

      if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

      #endregion Rwtrec Feedback 

      if(everythingOK) ZXC.aim_emsg(MessageBoxIcon.Information, sendingResult);

      Cursor.Current = Cursors.Default;
      return;

   } // private void RISK_Outgoing_eRacun(object sender, EventArgs e)
#endif
   /*private*/
   public void RISK_Outgoing_eRacun_STATUS(object sender, EventArgs e)
   {
      FakturDUC theDUC     = TheVvDocumentRecordUC as FakturDUC;
      Faktur    faktur_rec = theDUC.faktur_rec                 ;

      bool everythingOK = true;

      WebApiResult<List<VvMER_ResponseData>> webApiResultWithList = null;
      WebApiResult<VvMER_ResponseData>       webApiResult = null;
      VvMER_ResponseData                     responseData = null;

      string sendingResult = "";
      string newStatusMessage = "";

      if(theDUC.faktur_rec.MER_ElectronicID.IsZero())
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Ovaj račun nema 'eRačun status' jer još nije niti poslan.");
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      try
      {
         switch(ZXC.F2_TheProvider)
         {
            case ZXC.F2_Provider_enum.PND: // ######################################################################################################################################## 

               //webApiResult = Vv_eRacun_HTTP.VvPND_WebService_QueryOutbox_TRNandDPS_Single((uint)theDUC.faktur_rec.MER_ElectronicID);

               break;

            case ZXC.F2_Provider_enum.MER: // ######################################################################################################################################## 
            default                      : // ######################################################################################################################################## 

               webApiResultWithList = Vv_eRacun_HTTP.VvMER_WebService_QueryOutbox_TRN_Single((uint)theDUC.faktur_rec.MER_ElectronicID);

             //webApiResult = webApiResultList.ResponseData.FirstOrDefault();
             //responseData = webApiResult?.ResponseData;
             //responseData = webApiResultWithList.ResponseData.FirstOrDefault();
             //responseData = webApiResultWithList.ResponseData?.FirstOrDefault();

               webApiResult = WebApiResult<VvMER_ResponseData>.GetWebApiResult_From_WebApiResultWithList(webApiResultWithList);
               responseData = webApiResult.ResponseData;

               if(responseData == null || webApiResultWithList.ResponseData.IsEmpty())
               {
                  Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult);
                  everythingOK = false; 
               }
               else // uspjeh 
               {
                  sendingResult = "[" + responseData.ElectronicId + "] " + responseData.StatusName + ": " + VvSQL.GetServer_DateTime_Now(TheDbConnection).ToString(ZXC.VvDateAndTimeFormat_NoSec);
                  //ZXC.aim_log(faktur_rec.TT_And_TtNum + " vvMER_eRačun CHK STATUS: [" + sendingResult + "]");

                  newStatusMessage = "Current status is: " + responseData.StatusName;
               }

               break; // default je MER

         } // switch(ZXC.F2_TheProvider) 

      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         everythingOK = false;
         sendingResult = ex.Message;
      }

      #region Rwtrec Feedback 

      if(everythingOK)
      { 
         BeginEdit(faktur_rec);

         if(!ZXC.IsF2_2026_rules) faktur_rec.FiskPrgBr = ZXC.LenLimitedStr(((int)responseData.StatusId).ToString() + sendingResult, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.fiskPrgBr));
         faktur_rec.F2_StatusCD = (int)responseData.StatusId;
         
         bool rwtOK = faktur_rec.VvDao.RWTREC(TheDbConnection, faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 
         
         EndEdit(faktur_rec);
         
         if(rwtOK) WhenRecordInDBHasChangedAction(); // RRDREC 

         if(everythingOK) ZXC.aim_emsg(MessageBoxIcon.Information, newStatusMessage);
      }

      #endregion Rwtrec Feedback 

      Cursor.Current = Cursors.Default;

      return;
   }
   private void RISK_Outgoing_eRacun(object sender, EventArgs e)
   {
      //bool isQuickSend = sender.ToString() == "QuickSend";

      bool _isOneOnlyFromFakturDUC = true;

      #region Set oeRp parameters

      Outgoing_eRacun_parameters oeRp = new Outgoing_eRacun_parameters(_isOneOnlyFromFakturDUC);

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;

      /* oeRp_1. */ oeRp.faktur_rec     = theDUC.faktur_rec                                                           ;
      /* oeRp_2. */ oeRp.kupdob_rec     = theDUC.Get_Kupdob_FromVvUcSifrar(oeRp.faktur_rec.KupdobCD)                  ;
      /* oeRp_3. */ oeRp.primPlat_rec   = theDUC.Get_Kupdob_FromVvUcSifrar(oeRp.faktur_rec.PrimPlatCD)                ;

    //if(isQuickSend) oeRp.thePFD = new PrnFakDsc(FakturDUC.GetDscLuiListForThisTT(oeRp.faktur_rec.TT, /*subDsc*/ 0 /*?!*/)); // TODO: !!! ili ne? 
    /*else*/          oeRp.thePFD = ((VvRpt_RiSk_Filter)((VvRiskReport)theDUC.TheVvReport).VirtualRptFilter).PFD;

      #region FileDialog (oeRp.fullPath_XML_FileName, oeRp.pdfFileNameOnly)

      string fullPath_PDF_FileName;

      SaveFileDialog saveFileDialog = new SaveFileDialog();

      saveFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eRacun_Izlaz_Prefs.DirectoryName;

      saveFileDialog.Title            = "Kreiranje datoteke 'e-Računa'";
      saveFileDialog.Filter           = "Xml e-Račun (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
 //   saveFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      saveFileDialog.FilterIndex      = 1;
      saveFileDialog.RestoreDirectory = true;
      saveFileDialog.DefaultExt       = "xml";

 //   string suggestedFileName = oeRp.faktur_rec.TT_And_TtNum + "-" + oeRp.faktur_rec.DokDate_DDMMYY + "-" + oeRp.faktur_rec.KupdobTK/*KupdobName*/;
      string suggestedFileName = oeRp.suggestedXmlFileName;

      saveFileDialog.FileName = suggestedFileName.IsEmpty() ? "" : suggestedFileName + "." + saveFileDialog.DefaultExt;

      Cursor.Current = Cursors.Default;

      if(!ZXC.IsF2_2026_rules && saveFileDialog.ShowDialog() != DialogResult.OK)
      {
         saveFileDialog.Dispose(); // !!! 
         Cursor.Current = Cursors.Default;
         return;
      }

      Cursor.Current = Cursors.WaitCursor;

      // prejebavamo 
      if(ZXC.IsF2_2026_rules) saveFileDialog.FileName = Path.Combine(VvForm.GetLocalDirectoryForVvFile(/*"_ eRacun IZLAZNI"*/ZXC.eRacuniDIR), oeRp.suggestedXmlFileName + "." + saveFileDialog.DefaultExt);

      /* oeRp_7. */ 
        oeRp.fullPath_XML_FileName = saveFileDialog.FileName;
             fullPath_PDF_FileName = oeRp.fullPath_XML_FileName.Replace(saveFileDialog.DefaultExt, "pdf");

      System.IO.DirectoryInfo dInfo           = new System.IO.DirectoryInfo(oeRp.fullPath_XML_FileName)  ;
      string                  xmlFileNameOnly = dInfo.Name                                               ;
           /* oeRp_6. */ oeRp.pdfFileNameOnly = xmlFileNameOnly.Replace(saveFileDialog.DefaultExt, "pdf");
      string                  directoryName   = oeRp.fullPath_XML_FileName.Substring(0, oeRp.fullPath_XML_FileName.Length - (xmlFileNameOnly.Length + 1));

      ZXC.TheVvForm.VvPref.eRacun_Izlaz_Prefs.DirectoryName = directoryName;

      // 2026: 
      oeRp.qweFileNameBaseOnly = dInfo.Name    .Replace("." + saveFileDialog.DefaultExt, "");
      oeRp.qweTheDirectoryName = dInfo.FullName.Replace(dInfo.Name, ""); 

      saveFileDialog.Dispose(); // !!! 

      #endregion FileDialog (oeRp.fullPath_XML_FileName, oeRp.pdfFileNameOnly)

      // PDF byte array:
      /* oeRp_5. */

      CrystalDecisions.CrystalReports.Engine.ReportDocument theReportDocument;

    //if(isQuickSend)
    //{
    //   RptR_IRA theRptR_IRA = VvRiskReport.GetRptR_IRA(oeRp.faktur_rec, oeRp.thePFD, oeRp.faktur_rec.TT);
    //
    //   theReportDocument = theRptR_IRA.VirtualReportDocument;
    //}
    //else
    //{
         theReportDocument = theDUC.TheVvReport.VirtualReportDocument; // classic 
    //}

      oeRp.PDF_as_base64_byteArray  = Get_PDF_4eRacun_bytes(fullPath_PDF_FileName, theReportDocument);

      #endregion Set oeRp parameters

      RISK_Outgoing_eRacun_JOB(oeRp, true);
   }
   internal /*void*/bool RISK_Outgoing_eRacun_JOB(Outgoing_eRacun_parameters oeRp, bool isOneSingleFaktur, bool isIRMcalcB = false)
   {
      // 25.12.2025: ipak ugasio, jer onda ne radi stari nacin iz predprinta 
    //if(Vv_eRacun_HTTP.Is_FIR_SEND_ON() == false) return false;

      #region Init

      Vv_eRacun_HTTP.InitProjectData();

      EN16931.UBL.InvoiceType the_eRacun                                         = null;
    //System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = null;
    //byte[] xmlSignedByteArray                                                  = null;
      bool everythingOK                                                          = true;
      System.Text.Encoding encoding4UBL                                          = ZXC.VvUTF8Encoding_noBOM;
    //string certPassword                                                        = Vv_eRacun_HTTP.VvMER_Password; // !!! 
      string xmlString                                                           = "";

      #endregion Init

      #region Check if is already successfuly sent

      //if(!ZXC.IsF2_2026_rules && oeRp.faktur_rec.MER_ElectronicID.NotZero()) // brisi ovo u 2026 
      //{
      //   Cursor.Current = Cursors.Default;
      //   ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno.\n\nOvaj račun je već poslan.");
      //   return false;
      //}

      bool shouldProceed = oeRp.faktur_rec.F2_IsOKToSend || oeRp.wantsKOPIJA;

      if(ZXC.IsF2_2026_rules && !shouldProceed)
      {
         Cursor.Current = Cursors.Default;
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno.\n\nOvaj račun je već poslan.");
         return false;
      }
      bool isDiscrepancyBecauseOfMPC = oeRp.faktur_rec.Has_TrnSum_vs_S_Sum_Discrepancy.Contains("MPC sa dokumenta");

      if(ZXC.IsF2_2026_rules && oeRp.faktur_rec.Has_TrnSum_vs_S_Sum_Discrepancy.NotEmpty() /*CRVENO! */ && !isDiscrepancyBecauseOfMPC)
      {
         Cursor.Current = Cursors.Default;
         ZXC.aim_emsg(MessageBoxIcon.Error, "Odbijeno.\n\nNe mogu poslati 'crveni' dokument. Kontaktirajte support.");
         return false;
      }

      //if(oeRp.kupdob_rec.Email.IsEmpty() && !ZXC.IsF2_2026_rules)
      //{
      //   //Cursor.Current = Cursors.Default;
      //   ZXC.aim_emsg(MessageBoxIcon.Error  , "Partner nema zadanu email adresu u adresaru.\n\nNemoguća je dostava ovoga računa!");
      //   return false;
      //}

      #endregion Check if is already successfuly sent

      #region Create eRacun xmlString and save it to file

      try
      { 
         #region Create eRacun Object From Faktur

         Cursor.Current = Cursors.WaitCursor;

         //the_eRacun = EN16931.UBL.InvoiceType.Create_eRacun_fromFaktur
         //   (
         //      oeRp.faktur_rec             ,
         //      oeRp.thePFD                 ,
         //      oeRp.kupdob_rec             ,
         //      oeRp.primPlat_rec           , 
         //      VvUserControl.ArtiklSifrar  ,
         //      oeRp.PDF_as_base64_byteArray,
         //      oeRp.pdfFileNameOnly
         //   );

         the_eRacun = EN16931.UBL.InvoiceType.Create_eRacun_fromFaktur(TheDbConnection, oeRp, VvUserControl.ArtiklSifrar, isIRMcalcB);

         #endregion Create eRacun Object From Faktur

         #region Serialize

         xmlString = the_eRacun.Serialize(encoding4UBL);

         #endregion Serialize

         #region Remove Empty Nodes

         XmlDocument xmlDocument = ZXC.RemoveEmptyNodes(xmlString);
         xmlString = xmlDocument.OuterXml;

         //xmlString = ZXC.RemoveEmptyNodes_v2(xmlString);

         #endregion Remove Empty Nodes

         #region Get Certificate

         //certificate = the_eRacun.Get_eRacun_Application_Certificate("VEKTOR"); // "VEKTOR" je 'Issued To' u mmc WindowsCertificateStore-u, kasnije mozes kao i Fiskalnoga iz MySQL-a 
         //
         //if(certificate == null)
         //{
         //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom dohvata certifikata!");
         //   Cursor.Current = Cursors.Default;
         //   return;
         //}

         #endregion Get Certificate 

         #region UBLextension - UBLsigner (By FINA - Jura)

         // hr.fina.eracun.signature.UBLSigner ublSigner = new hr.fina.eracun.signature.UBLSigner(certificate, certPassword, "Invoice");
         // //u memoryStream je xml prije potpisa 
         // xmlSignedByteArray = ublSigner.signUBLDocument(encoding4UBL.GetBytes(xmlString));
         // 
         // xmlString = encoding4UBL.GetString(xmlSignedByteArray);

         #endregion UBLextension - UBLsigner

         #region Validate eRacun XML File 

         MemoryStream memoryStream = new MemoryStream();

         xmlDocument.LoadXml(xmlString);

         xmlDocument.Save(memoryStream);

         bool validateOK = false;

         try { validateOK = EN16931.UBL.InvoiceType.ValidateThis_XML_eRacun(memoryStream, ZXC.IsF2_2026_rules, oeRp.qweXMLfileNameOnly); } catch(Exception ex) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message); }

         if((memoryStream != null))
         {
            memoryStream.Dispose();
         }

         // 24.12.2025: tek?! 
         if(validateOK == false)
         {
            everythingOK = false;
         }

         #endregion Validate eRacun XML File 

         #region Format / Beautify XML (DEBUG ONLY)
#if DEBUG

         xmlString = ZXC.BeautifyXml(xmlString, encoding4UBL);

#endif
         #endregion Format / Beautify XML (DEBUG ONLY)

         #region Save eRacun XML File 

         // 17.10.2025: prestajemo snimati na disk za F2 
         // ipak vretio 
       //if(ZXC.IsF2_2026_rules == false)
         {
            bool saveOK = EN16931.UBL.InvoiceType.VvSaveToFile(xmlString, oeRp.qweXMLfullPathAndName/*oeRp.fullPath_XML_FileName*/, encoding4UBL);
         }

         #endregion Save eRacun XML File 

         #region Show XML (DEBUG ONLY)
#if DEBUG

         // upali opet ako ce trebati proucavati greske u InvoiceType XML-u 

         //return false; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 

         //try
         //{
         //   /*the_eRacun*/EN16931.UBL.InvoiceType.VvSaveToFile(xmlString, oeRp.qweXMLfullPathAndName, encoding4UBL);
         //   System.Diagnostics.Process.Start(oeRp.qweXMLfullPathAndName);
         //}
         //catch(Exception ex)
         //{
         //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, ex.Message);
         //}

#endif
         #endregion Show XML (DEBUG ONLY)

      }
      catch(Exception ex) { ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška: {0}", ex.Message); everythingOK = false; }

      if(everythingOK == false)
      {
         Cursor.Current = Cursors.Default;
         return false;
      }

      #endregion Create eRacun xmlString and save it to file

      #region SEND_eRacun_MER_WebService 

      WebApiResult<VvMER_ResponseData> webApiResult = null;

      string sendingResult = "";

      //ZXC.AMSstatus AMSstatus = oeRp.faktur_rec.F2_AMSstatus;
      //
      //if(oeRp.faktur_rec.IsF2)
      //{
      //   AMSstatus = KupdobDao.RefreshKupdob_AMSstatus(TheDbConnection, oeRp.kupdob_rec);
      //}
      //
      //if(AMSstatus == ZXC.AMSstatus.NEPOZNAT) { ZXC.aim_emsg(MessageBoxIcon.Stop, "AMS status je nepoznat! Ne mogu procesirati ovaj račun."); Cursor.Current = Cursors.Default; return false; }

      everythingOK = false;

      try
      {
         switch(ZXC.F2_TheProvider)
         {
            case ZXC.F2_Provider_enum.PND: // ######################################################################################################################################## 

             //if(oeRp.faktur_rec.IsF2send)
             //{
                  webApiResult = Vv_eRacun_HTTP.VvPND_WebService_SEND(xmlString); // ###################### VOILA ################### 

                  if(webApiResult.StatusCode != (int)System.Net.HttpStatusCode.OK || webApiResult.ResponseData.Id.IsZero()) // Error 
                  {
                     everythingOK = false;
                     sendingResult = string.Format("Dogodila se greška\n\rStatusCode:\n\r{0}\n\rStatusDescription:\n\r{1}\n\rResponseJson:\n\r{2}\n\rErrorBody:\n\r{3}\n\rExceptionMessage:\n\r{4}",
                        webApiResult.StatusCode, webApiResult.StatusDescription, ZXC.PrettyPrintJson(webApiResult.ResponseString.NullSafe()), webApiResult.ErrorBody.NullSafe(), webApiResult.ExceptionMessage.NullSafe());

                     Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult);
                  }
                  else // It's OK 
                  {
                     everythingOK = true;
                     sendingResult = "[" + webApiResult.ResponseData.Id + "] " +
                                           webApiResult.ResponseData.Message + ": " +
                                           webApiResult.ResponseData.InsertedOn;
                  }
             //}
             //else if(oeRp.faktur_rec.IsF2eIzvj)
             //{
             //   webApiResult = Vv_eRacun_HTTP.VvPND_WebService_eIzvj(xmlString, "IR"); 
             //
             //   if(webApiResult.StatusCode != (int)System.Net.HttpStatusCode.OK || webApiResult.ResponseData.IsSuccess == false) // Error 
             //   {
             //      everythingOK = false;
             //      sendingResult = string.Format("Dogodila se greška\n\rStatusCode:\n\r{0}\n\rStatusDescription:\n\r{1}\n\rResponseJson:\n\r{2}\n\rErrorBody:\n\r{3}\n\rExceptionMessage:\n\r{4}",
             //         webApiResult.StatusCode, webApiResult.StatusDescription, ZXC.PrettyPrintJson(webApiResult.ResponseString.NullSafe()), webApiResult.ErrorBody.NullSafe(), webApiResult.ExceptionMessage.NullSafe());
             //
             //      Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, ZXC.F2_WebApi.eIzvj);
             //   }
             //   else // It's OK 
             //   {
             //      everythingOK = true;
             //      sendingResult = "message: " + webApiResult.ResponseData.Message;
             //   }
             //}

               break;

            case ZXC.F2_Provider_enum.MER: // ######################################################################################################################################## 
            default                      : // ######################################################################################################################################## 

             //if(oeRp.faktur_rec.IsF2send || !ZXC.IsF2_2026_rules)
             //{ 
                  webApiResult = Vv_eRacun_HTTP.VvMER_WebService_SEND(xmlString); // ###################### VOILA ################### 

                  if(webApiResult.StatusCode != (int)System.Net.HttpStatusCode.OK || webApiResult.ResponseData.ElectronicId.IsZero()) // Error 
                  {
                     everythingOK = false;
                     sendingResult = string.Format("Dogodila se greška\n\rStatusCode:\n\r{0}\n\rStatusDescription:\n\r{1}\n\rResponseJson:\n\r{2}\n\rErrorBody:\n\r{3}\n\rExceptionMessage:\n\r{4}",
                        webApiResult.StatusCode, webApiResult.StatusDescription, ZXC.PrettyPrintJson(webApiResult.ResponseString.NullSafe()), webApiResult.ErrorBody.NullSafe(), webApiResult.ExceptionMessage.NullSafe());

                   //Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult                                  );
                     Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, null, oeRp.qweXMLfullPathAndName);
               }
                  else // It's OK 
                  {
                     everythingOK = true;
                     sendingResult = "[" + webApiResult.ResponseData.ElectronicId + "] " +
                                           webApiResult.ResponseData.StatusName + ": " +
                                           webApiResult.ResponseData.Sent;
                  }
             //}
             //else if(oeRp.faktur_rec.IsF2eIzvj)
             //{
             //   webApiResult = Vv_eRacun_HTTP.VvMER_WebService_eIzvj(xmlString, oeRp.faktur_rec.DokDate, /* isCopy */ false, "IR"); 
             //
             //   if(webApiResult.StatusCode != (int)System.Net.HttpStatusCode.OK || webApiResult.ResponseData.IsSuccess == false) // Error 
             //   {
             //      everythingOK = false;
             //      sendingResult = string.Format("Dogodila se greška\n\rStatusCode:\n\r{0}\n\rStatusDescription:\n\r{1}\n\rResponseJson:\n\r{2}\n\rErrorBody:\n\r{3}\n\rExceptionMessage:\n\r{4}",
             //         webApiResult.StatusCode, webApiResult.StatusDescription, ZXC.PrettyPrintJson(webApiResult.ResponseString.NullSafe()), webApiResult.ErrorBody.NullSafe(), webApiResult.ExceptionMessage.NullSafe());
             //
             //      Vv_eRacun_HTTP.Show_WebApiResult_ErrorMessageBox(webApiResult, ZXC.F2_WebApi.eIzvj);
             //   }
             //   else // It's OK 
             //   {
             //      everythingOK = true;
             //      sendingResult = "isSuccess: " + webApiResult.ResponseData.IsSuccess;
             //   }
             //}

               break; // default je MER
         }

         //ZXC.aim_log(oeRp.faktur_rec.TT_And_TtNum + " vvMER_eRačun: [" + sendingResult + "]");

      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Greška prilikom slanja na WebServis: {0}", ex.Message);
         everythingOK = false;
         sendingResult = ex.Message;
      }

      #endregion SEND_eRacun_MER_WebService 

      #region Rwtrec Feedback 

      if(everythingOK) // 02.11.2025: Rwtrec Feedback opkoljen if-om 
      { 
         BeginEdit(oeRp.faktur_rec);

                                          // ugašeno 2026. godini. oslobodili FiskPrgBr za nesto drugo 
                                        //oeRp.faktur_rec.FiskPrgBr       = ZXC.LenLimitedStr(sendingResult, ZXC.FaktExDao.GetSchemaColumnSize(ZXC.FexCI.fiskPrgBr));
                                          
                                          oeRp.faktur_rec.F2_SentTS       = VvSQL.GetServer_DateTime_Now(TheDbConnection)       ;
                                        //oeRp.faktur_rec.F2_AMSstatus    = AMSstatus                                           ;
         /*if(oeRp.faktur_rec.IsF2send)*/ oeRp.faktur_rec.F2_ElectronicID = (uint)webApiResult.ResponseData.ElectronicId        ;
         /*if(oeRp.faktur_rec.IsF2send)*/ oeRp.faktur_rec.F2_StatusCD     = (int) webApiResult.ResponseData.StatusId            ;

         bool rwtOK = oeRp.faktur_rec.VvDao.RWTREC(TheDbConnection, oeRp.faktur_rec, false, true, false, false, true/* !!! */ ); // zbog implicitne synchronizacije ovaj rwtrec ne zelimo u Insert_LAN_LogEntry 

         EndEdit(oeRp.faktur_rec);

         if(!ZXC.IsF2_2026_rules && rwtOK && oeRp.IsOneOnlyFromFakturDUC) (TheVvDocumentRecordUC as FakturExtDUC).Fld_fiskPrgBr = oeRp.faktur_rec.FiskPrgBr;                   // TODO in 2026 
         if( ZXC.IsF2_2026_rules && rwtOK && oeRp.IsOneOnlyFromFakturDUC) (TheVvDocumentRecordUC as FakturExtDUC).Fld_fiskPrgBr = oeRp.faktur_rec.F2_ElectronicID. ToString(); // TODO in 2026 
         
         if(isOneSingleFaktur) ZXC.aim_emsg(MessageBoxIcon.Information, sendingResult);
      }

      #endregion Rwtrec Feedback 

      Cursor.Current = Cursors.Default;

      if(everythingOK && isOneSingleFaktur)
      {
         ReRead_OnClick(null, EventArgs.Empty);
      }

      return true;

   } // private void RISK_Outgoing_eRacun(object sender, EventArgs e)
   private void RISK_Incoming_eRacun(object sender, EventArgs e)
   {
#if nijeOvoNikadSazivilo

      #region variablez

      FakturExtDUC theDUC = TheVvDocumentRecordUC as FakturExtDUC;
      int rowIdx, idxCorrector;

      Artikl artikl_rec;

      List<Rtrans> rtransList;

      Kupdob dobavKupdob_rec;

      #endregion variablez

      #region FileDialog

      OpenFileDialog openFileDialog   = new OpenFileDialog();
      openFileDialog.InitialDirectory = ZXC.TheVvForm.VvPref.eRacun_Ulaz_Prefs.DirectoryName;
      openFileDialog.Filter           = "Xml e-Račun (*.xml)|*.xml|Sve Datoteke (*.*)|*.*";
    //openFileDialog.Filter           = "(*" + thisDocumentID + "*)|" + "*" + thisDocumentID + "*" + "|All files (*.*)|*.*";
      openFileDialog.FilterIndex      = 1;
      openFileDialog.RestoreDirectory = true;

      if(openFileDialog.ShowDialog() != DialogResult.OK)
      {
         openFileDialog.Dispose(); // !!! 
         return;
      }

      string fullPathName            = openFileDialog.FileName;
      System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(fullPathName);

      string fileName = dInfo.Name;
      string directoryName = fullPathName.Substring(0, fullPathName.Length - (fileName.Length + 1));
      ZXC.TheVvForm.VvPref.eRacun_Ulaz_Prefs.DirectoryName = directoryName;

      openFileDialog.Dispose(); // !!! 

      Cursor.Current = Cursors.WaitCursor;
      //SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}"); SendKeys.Send("{TAB}");

      #endregion FileDialog

      #region Qtmp Validate

      bool isValidateOK = EN16931.UBL.InvoiceType.ValidateThis_XML_eRacun(fullPathName);

      #endregion Qtmp Validate

      #region Deserialize - Load Xml File To bussiness data (by .NET)

      EN16931.UBL.InvoiceType invoice = null;

      XmlSerializer ser = new XmlSerializer(typeof(EN16931.UBL.InvoiceType));
      FileStream fs = new FileStream(fullPathName, FileMode.Open);
      try
      {
         invoice = ser.Deserialize(fs) as EN16931.UBL.InvoiceType;
      }
      catch (Exception ex)
      {
         ZXC.aim_emsg_VvException(ex);
      }
      fs.Close();
      
      
      List<EN16931.UBL.InvoiceLineType> stavkeList = new List<EN16931.UBL.InvoiceLineType>();
      foreach(EN16931.UBL.InvoiceLineType stavka in invoice.InvoiceLine)
      {
         stavkeList.Add(stavka);
      }

      #endregion Deserialize - Load Xml File To bussiness data (by .NET)

      #region Create Faktur Object From eRacun

      Faktur faktur_rec = invoice.Create_Faktur_From_eRacun(VvUserControl.KupdobSifrar, VvUserControl.ArtiklSifrar);

      #endregion Create Faktur Object From eRacun

#if docnye
      #region Put Faktur Fields

      // Try via sumarijaID pred upisan u MatBr Kupdoba 
      dobavKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Matbr == invoice.Items[0].sumarijaID.ToString());

      if(dobavKupdob_rec == null)
      {
         // Try via NazivSumarije 
         dobavKupdob_rec = VvUserControl.KupdobSifrar.SingleOrDefault(k => k.Naziv.ToUpper() == invoice.Items[0].NazivSumarije.ToUpper());
      }

      if(dobavKupdob_rec != null)
      {
         theDUC.Fld_KupdobCd = dobavKupdob_rec.KupdobCD;
         theDUC.Fld_KupdobTk = dobavKupdob_rec.Ticker;
         theDUC.Fld_KupdobName = dobavKupdob_rec.Naziv;
         theDUC.sifrarSorterType = VvSQL.SorterType.Code;
         ZXC.LoadPoprat_InProgress = true;
         theDUC.AnyKupdobTextBoxLeave(theDUC.tbx_KupdobCd, EventArgs.Empty);
         theDUC.sifrarSorterType = VvSQL.SorterType.Name;
         //SendKeys.Send("{TAB}");
         //ZXC.OffixImport_InProgress = false;
      }
      else
      {
         dobavKupdob_rec = new Kupdob();
         dobavKupdob_rec.Ticker = "?!XY"; // Za pločicu 

         ZXC.aim_emsg(MessageBoxIcon.Error, "U šifrarniku partnera ne mogu naći šumarijuID {0}!\n\nZadajte Partnera ručno.", invoice.Items[0].sumarijaID);
      }

      theDUC.Fld_externLink1 = fullPathName;

      theDUC.Fld_DokDate = theDUC.Fld_PdvDate = theDUC.Fld_SkladDate = theDUC.Fld_DospDate = invoice.Items[0].datumpd;

      string napomena = "BrDok: ";
      foreach(PopratniceZaglavlja zaglavlje in invoice.Items)
      {
         napomena += zaglavlje.BrojDok + ", ";
      }
      napomena = napomena.TrimEnd(' ').TrimEnd(',');
      napomena += ". Stranica: ";
      foreach(PopratniceZaglavlja zaglavlje in invoice.Items)
      {
         napomena += zaglavlje.stranica + ", ";
      }
      napomena = napomena.TrimEnd(' ').TrimEnd(',');

      theDUC.Fld_Napomena = ZXC.LenLimitedStr(napomena, ZXC.FakturDao.GetSchemaColumnSize(ZXC.FakCI.napomena));

      #endregion Put Faktur Fields

      #region PutDgvFields 1 - Rtrans Grid

      rtransList = rekapList
         .GroupBy(rekap => rekap.VvArtiklName)
         .Select(grp => new Rtrans()
         {
            T_artiklName = grp.Key,
            T_jedMj      = "m3",
            T_pdvSt      = Faktur.CommonPdvStForThisDate(invoice.Items[0].datumpd),
            T_kol        = grp.Sum(aGR => aGR.masa)
         })
         .OrderBy(suma => suma.T_artiklName, new VvCompareStringsOrdinal())
         .ToList();

      idxCorrector = VvUserControl.GetDGVsIdxCorrrector(theDUC.TheG);

      foreach(Rtrans rtrans_rec in rtransList)
      {
         artikl_rec = VvUserControl.ArtiklSifrar.SingleOrDefault(a => a.ArtiklName == rtrans_rec.T_artiklName);
         if(artikl_rec == null)
         {
            ZXC.aim_emsg(MessageBoxIcon.Error, "Iz REKAPITULACIJE, Ne mogu naći\n\n[{0}]\n\nu datoteci artikala!", rtrans_rec.T_artiklName);
            artikl_rec = new Artikl() { ArtiklCD = "---- ??? ----", ArtiklName = rtrans_rec.T_artiklName };
         }
         rtrans_rec.T_artiklCD   = artikl_rec.ArtiklCD;

      #region Put, Calc, ... DgvFields

         theDUC.TheG.Rows.Add();

         rowIdx = theDUC.TheG.RowCount - idxCorrector;

         theDUC.PutDgvLineFields1    (rtrans_rec, rowIdx, true);
         rtrans_rec.CalcTransResults (null/*faktur_rec*/);
         theDUC.PutDgvLineResultsFields1(rowIdx, rtrans_rec, false);
         theDUC.GetDgvLineFields1    (rowIdx, false, null); // da napuni Document's business.Transes 

      #endregion Put, Calc, ... DgvFields
      }

      VvDocumentRecordUC.RenumerateLineNumbers(theDUC.TheG, 0);
      theDUC.UpdateLineCount(theDUC.TheG);

      theDUC.PutDgvTransSumFields();
      theDUC.PutTransSumToDocumentSumFields();

      #endregion PutDgvFields 1 - Rtrans Grid
#endif

      Cursor.Current = Cursors.Default;

      //theDUC.tbx_Napomena.Focus();

#endif
   } // private void RISK_Incoming_eRacun(object sender, EventArgs e) 

   #endregion eRacun

}
