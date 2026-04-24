# DevExpress Migration — V4 (Authoritative)

> Ovaj dokument zamjenjuje `DevExpress_Migration_V2.md` i `DevExpress_Migration_V3.md` kao
> **jedinstveni referentni plan** migracije Vektor WinForms UI-a s Crownwood DotNetMagic
> na DevExpress WinForms. V2 i V3 ostaju u repozitoriju kao povijesne reference; od sada
> se radi **isključivo prema V4**.
>
> **Scope ovog dokumenta:** kompletna analiza zatečenog stanja + potpun plan migracije,
> od decouplinga `ZXC` infrastrukture do implementacije FLOATING-DETACH funkcionalnosti.
>
> **Target framework:** .NET Framework 4.8, C# 7.3, WinForms.
>
> **Target DevExpress:** bilo koja aktuelna DX verzija 22.x ili novija (sve podržavaju
> .NET Framework 4.5.2+). Preciznu verziju fiksirati u Koraku 0 (v. §6.1).

---

## 0. Strateška odluka (fiksirano)

Migracija se izvodi u **dvije makro-faze koje NIKAD ne idu zajedno**:

1. **SWAP-FAZA (Faze 1–2 iz ovog dokumenta):** Crownwood → DevExpress uz
   **bit-po-bit identično ponašanje**. Nema novih feature-a. Nakon ove faze aplikacija
   se mora ponašati kao prije — svaka razlika je bug swapa.
2. **DETACH-FAZA (Faza 3 iz ovog dokumenta):** dodavanje FLOATING-DETACH funkcionalnosti
   (tab se povlačenjem van pretvara u samostalnu top-level formu sa svojim menijem
   i toolstripovima, neovisnim od ishodišne). Započinje **tek nakon što je
   SWAP-FAZA produkcijski stabilna i ekstenzivno testirana.**

**Argumentacija (konačna, bez dileme):**

- Izolacija uzroka regresija: bug u swapu vs bug u detachu ne smiju se miješati.
- Git-bisect friendly: SWAP-FAZA se može zasebno mergati, cut-rezati release kandidat,
  pa po potrebi rollback-ati bez gubitka detach rada.
- DevExpress API krivulja učenja: do kraja SWAP-FAZA već vladamo `XtraTabControl`,
  `BarManager`, skin-sustavom — što čini detach dizajn kvalitetnijim.
- Testna matrica je linearna umjesto kartezijeva produkta.
- Nema vremenskog pritiska (klijent je to eksplicitno potvrdio).

**FLOATING-DETACH se SIGURNO radi** — nije pod upitnikom. Samo je odgođen iz
inženjerskih (ne poslovnih) razloga.

---

## 1. Zatečeno stanje — Crownwood arhitektura

### 1.1 Hijerarhija forme

    VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm           (singleton via ZXC.TheVvForm)
    ├── menuStrip (System.Windows.Forms.MenuStrip)
    │     └── 7 top-level menija: Datoteka, Pogled, Format, Forme, SubModul, Izvještaji, Pomoć
    ├── tsPanel_Modul      → ts_Modul (ToolStrip)          — modul navigacijski gumbi
    ├── tsPanel_Record     → ts_Record (ToolStrip)         — CRUD + nav (NEW, OPN, DEL, SAV,
    │                                                         ESC, FRS, PRV, NXT, LST, FND,
    │                                                         PRN, PRW, ARH, …)
    ├── tsPanel_SubModul   → ats_SubModulSet[i][j] (ToolStrip[][])  — per-SubModul toolbari
    ├── ts_Report          (ToolStrip)                     — report ops (GO, Print, PDF,
    │                                                         Export, Zoom, page nav)
    ├── modulPanel (Panel, DockStyle.Left|Right)
    │     └── TreeView_Modul (Crownwood.DotNetMagic.Controls.TreeControl)
    ├── spliterModulPanelTabControl (Splitter)
    └── TheTabControl (Crownwood.DotNetMagic.Controls.TabControl)
          ├── VvTabPage[0] → VvUserControl (npr. FakturDUC)
          ├── VvTabPage[1] → VvUserControl (npr. NalogDUC)
          └── VvTabPage[N] → VvUserControl (npr. PlacaDUC)

### 1.2 Ključne klase

| Klasa | Datoteka | Uloga |
|---|---|---|
| `VvForm` | `zVvForm\*.cs` (partial, ~7 fajlova) | Glavna MDI-like forma. Posjeduje SVE menije, toolstripove, tab control. Nasljeđuje `DotNetMagicForm`. |
| `VvTabPage` | `Framework\VvTabPage.cs` | Proširuje `Crownwood.DotNetMagic.Controls.TabPage`. Lifecycle UC-a, `VisibleChanged` sklopka za toolbarove, per-tab `WriteMode`. |
| `VvInnerTabPage` | `Framework\VvTabPage.cs` (dno) | Proširuje isti Crownwood `TabPage`. "Unutarnji" tabovi u DUC-ovima (report viewer, grid, zoom…). |
| `VvUserControl` | `Framework\VvUserControl*.cs` | Bazni UC (FakturDUC, NalogDUC, PlacaDUC…). Navigacija u `VvTabPage` preko fragilnog `this.Parent.Parent` cast-a. Drži statičke sifrar cacheve. |
| `ZXC` | `Framework\ZXC.cs` (~9800 LOC) | Globalno statičko stanje: `TheVvForm` singleton, 5 DB konekcija, ~150 lookup listi, ~50 `*_InProgress` mutex flagova, TtInfo, deployment flagovi. |
| `VvEnvironmentDescriptor` | `UtilsEtc\VvEnvironmentDescriptor.cs` | XML-serijalizirani state forme (toolbar layout kao `List<VvToolStripItem_State>`, pozicija, boje, font). |
| `VvHamper` | `UtilsEtc\VvHamper.cs` | Pervazivan layout sustav (`ZXC.QUN`, `ZXC.Redak`, `ZXC.Kolona`). NIJE samo Crownwood style helper — koristi se za programatski layout kontrola u svakom DUC-u. |
| `VvColors` / `VvColorsStylsDlg` | `UtilsEtc\*.cs` | Skin odabir preko `Crownwood.Common.VisualStyle` enum-a. |

### 1.3 Inventar Crownwood tipova

#### 1.3.1 Direktno naslijeđeni Crownwood tipovi

| Vektor klasa | Bazni Crownwood tip | Broj instanci | Komentar |
|---|---|---|---|
| `VvForm` | `Crownwood.DotNetMagic.Forms.DotNetMagicForm` | 1 (singleton) | Skin, caption style |
| `VvTabPage` | `Crownwood.DotNetMagic.Controls.TabPage` | N (po otvorenom modulu) | Glavni tab layout |
| `VvInnerTabPage` | `Crownwood.DotNetMagic.Controls.TabPage` | N (unutar DUC-ova) | Sekundarni tabovi |

#### 1.3.2 Direktno instancirani Crownwood tipovi

| Crownwood tip | Mjesta korištenja | Uloga |
|---|---|---|
| `Crownwood.DotNetMagic.Controls.TabControl` | `VvForm.TheTabControl` (glavni), `VvRecordUC.TheTabControl` (per-DUC), `FUG_PTG_UC.ThePolyGridTabControl`, `FakturExtDUC.TheTabControl` | Glavni + unutarnji tab kontejneri |
| `Crownwood.DotNetMagic.Controls.TreeControl` | `VvForm.TreeView_Modul` | Modul navigacijsko stablo |
| `Crownwood.DotNetMagic.Common.VisualStyle` | `VvColors.cs`, `VvColorsStylsDlg.cs`, `VvEnvironmentDescriptor` (perzistencija) | Enum za odabir skina |

#### 1.3.3 Datoteke koje direktno referenciraju `Crownwood.*`

Osim `zVvForm\*.cs` i `Framework\VvTabPage.cs`, Crownwood API se pojavljuje u:

- `UtilsEtc\VvColors.cs`, `UtilsEtc\VvColorsStylsDlg.cs` — skin sustav
- `UtilsEtc\VvAboutBox.cs` — TabControl u About dialogu
- `Framework\VvUserControlRecord_Sub.cs` — `TheTabControl` property + CellEnter/CellLeave status bar
- `VvUC\PrjUC\SkyRuleUC.cs` — TabControl u UC-u
- `VvUC\PlaUC\PersonUC.cs` — TabControl u UC-u
- `VvUC\PlaUC\PlacaDUC_Q.cs` — TheVvTabPage access
- `VvUC\MixerUC\UgovoriDUC.cs`, `ZahtjeviDUC.cs`, `ZahtjevRNMDUC.cs`, `SmdDUC.cs`, `ZLJ_DUC.cs`, `UrudzbeniDUC.cs` — razni TabControl / TheVvForm dodiri
- `VvUC\FinUC\Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs` — SubModulSet + TabControl
- `VvUC\RiskUC\PTG_DUC.cs`, `ArtiklUC.cs` — toolbar manipulation preko TheVvForm
- `VvUC\FakturExtDUC.cs`, `VvUC\RiskUC\FUG_PTG_UC.cs` — Inner TabControl

**Procjena:** ~18–20 datoteka direktno dotiču Crownwood API. Nakon decouplinga (Faza 1)
većina ovih dodira prelazi u indirektne preko `IVvDocumentHost` / `VvToolbarFactory`.

### 1.4 Tab ↔ Toolbar spregnutost (srce problema)

Trenutni flow kada tab postane aktivan:

    VvTabPage_VisibleChanged(visible = true)
      │
      ├── GetTSB_EnabledStateSnapshot()                        — snimi stanje izlaznog taba
      ├── ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet() — sakrij sve ats_SubModulSet[][] osim trenutnog
      ├── SetVvSubModulSetMenuEnabledOrDisabled_RegardingWriteMode() — enable/disable po WriteMode
      ├── Show/hide ts_Report vs ts_Record po TabPageKind
      └── PutTSB_EnabledStateSnapshot()                        — vrati stanje ulaznog taba

Plus: `VvTabPage_Validating` **blokira tab switching** tijekom arhive.

**Kritične točke sprege (za kasniji detach):**

- `VvTabPage.theVvForm` — konstruktor prima `VvForm _vvForm` i sprema kao private field
- `ats_SubModulSet[i][j]` — 2D ToolStrip array živi na `VvForm`; tabovi samo odlučuju koji je vidljiv
- `SetVvMenuEnabledOrDisabled_*()` — direktno manipulira `ts_Record.Items["NEW"]` po string imenu
- `ZXC.TheVvForm` — reference posvuda u business logici (npr. `Rtrans.Get_S_KC_fromScreen()`)
- `VvUserControl.TheVvTabPage` — navigira preko `(VvTabPage)this.Parent.Parent` → **lomi se pri reparent-u**
- `VvUserControl.TheDbConnection` — fallback kroz `ZXC.TheVvForm.TheVvTabPage.TheDbConnection`

### 1.5 Toolbar infrastruktura

Svi meniji i toolbarovi su **programatski kreirani** (bez Designera), definirani data-driven
strukturama u `Menus_ToolStrips.cs`:

    VvMenu[] aMainMenu = new VvMenu[] {
        new VvMenu("Datoteka",   true,  "", new VvSubMenu[] { /* 32 stavke */ }),
        new VvMenu("Pogled",     true,  "", new VvSubMenu[] { /* 20 stavki */ }),
        new VvMenu("Format",     true,  "", new VvSubMenu[] { … }),
        new VvMenu("Forme",      true,  "", new VvSubMenu[] { }),
        new VvMenu("",           false, "SubModul", new VvSubMenu[] { }),
        new VvMenu("Izvještaji", false, "Report",   new VvSubMenu[] { /* 23 stavke */ }),
        new VvMenu("Pomoć",      true,  "", new VvSubMenu[] { … }),
    };

Svaki `VvSubMenu` nosi: tekst, ikonu, shortcut, event handler, `vvMenuStyleEnum` (meni/toolbar/oboje).

**Ovo je prednost za migraciju:** data-driven definicija mapira se prirodno u DevExpress
`BarItem` kreaciju — Faza 2/3 je **tehnički jednostavnija od očekivane**.

### 1.6 WriteMode enable/disable

Centralne metode na `VvForm`:

- `SetVvMenuEnabledOrDisabled_RegardingWriteMode(WriteMode)` — master toggle
- `SetVvMenuEnabledOrDisabled_Explicitly(string name, bool enabled)` — per-gumb po string imenu
- `SetVvMenuEnabledOrDisabled_NoTabPageIsOpened()` — početno disabled stanje
- `SetVvMenuEnabledOrDisabled_FilterTabPageIsOpened()` — report filter
- `SetVvMenuEnabledOrDisabled_ArhivaTabPageIsOpen()` — arhiva

Specijalni slučajevi koje `VvToolbarFactory.ApplyWriteMode` mora očuvati:

| Uvjet | Utječe na gumbe | DUC tipovi |
|---|---|---|
| `IsTEXTHOshop` | Disables OPN, DEL | Osim `InventuraDUC`, `InventuraMPDUC` |
| `IsPCTOGO` | Disables NEW, DUP | `ANU_PTG_DUC`, `DIZ_PTG_DUC`, `PVR_PTG_DUC`, `A1_ANU_PTG_DUC`, `PRN_DIZ_PTG_DUC` |
| `IsPCTOGO` | Disables DUP samo | `ZIZ_PTG_DUC` |
| `IsPCTOGO` | Disables NEW, OPN, DUP | `KOP_PTG_DUC` |
| `theVvUC is KDCDUC` | Disables NEW, DUP | Uvijek |
| `IsSvDUH_ZAHonly` | Disables NEW, OPN, DEL, DUP | `IZD_SVD_DUC` |
| `IsSvDUH_ZAHonly` | Disables DEL | `ZAH_SVD_DUC` (non-super users) |

Sva ova pravila koncentrirana su u metodi `SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB`
— to je **single extraction point** za `VvToolbarFactory.ApplyWriteMode`.

### 1.7 Product-type filtriranje

`InitalizeToolStrip_Modul()` filtrira SubModul gumbe po deployment siteu:

| Site | Vidljivi moduli |
|---|---|
| `Surger` | samo PRIJEM, OPRANA |
| `Remonster` | samo DEBIT |
| Ostali | puni set |

Početna stanja snimaju se u `VvTsiModulDefaultStates[]`. Factory mora primiti site parametar.

### 1.8 Business layer ↔ UI sprega

`Rtrans.cs` direktno dotiče UI singleton:

    // BusinessLayer\Rtrans.cs
    private decimal Get_S_KC_fromScreen()
    {
        return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_KC_fromScreen();
    }
    internal static bool CheckZtrColExists()
    {
        FakturDUC theDUC = ZXC.TheVvForm.TheVvRecordUC as FakturDUC;
        …
    }

Poziva se iz `Rtrans.CalcTransResults()` → `INIT_Memset0Rtrans_GetZtr()`. Ovo je **kritična
točka decoupling-a** — detached tab više nije `ZXC.TheVvForm.TheVvRecordUC`.

`Rtrans.CalcTransResults` granaju u tri calc putanje:

    CalcTransResults(Faktur)
    ├── IsForceMalUlazCalc → CalcTrans_MALOP_Results_ULAZ()
    ├── IsMalopTT          → CalcTrans_MALOP_Results()
    │   ├── IsNivelacijaZPC → CalcTrans_MALOP_Results_ULAZ_ZPC()
    │   ├── IsFinKol_U      → CalcTrans_MALOP_Results_ULAZ_ByCIJENA()
    │   └── else            → CalcTrans_MALOP_Results_IZLAZ()
    └── else               → CalcTrans_VELEP_Results()
        └── Is_VelepByMPC  → CalcTrans_VELEP_Results_ByMPC()

Sve putanje pozivaju `INIT_Memset0Rtrans_GetZtr(faktur_rec)`. To je jedino mjesto za
decoupling u `Rtrans`-u.

### 1.9 Status bar sprega (business-layer-wide)

`ZXC.SetStatusText()` i `ZXC.ClearStatusText()` (`ZXC.cs` linije ~7171–7189) direktno
pristupaju `ZXC.TheVvForm.TStripStatusLabel.Text` s `.Invalidate()`/`.Update()`/`.Refresh()`.

`VvUserControlRecord_Sub.grid_CellEnter_SetStatusText` / `grid_CellLeave_RestoreStatusText`
(linije ~703–733) imaju dodatnu spregu preko `ZXC.TheVvForm.statusTextBackup`.

Zove se iz Mixer, Placa, Ptrane, Person, Htrans. Za detach mora ići preko
`DocumentHost.SetStatusText()`.

### 1.10 `VvUserControl` navigation chain

    // Trenutno — fragilno
    public VvTabPage TheVvTabPage
    {
        get { return (VvTabPage)this.Parent.Parent; }
    }

    public XSqlConnection TheDbConnection
    {
        get
        {
            return TheVvTabPage?.TheDbConnection
                ?? ZXC.TheVvForm.TheVvTabPage.TheDbConnection;
        }
    }

Reparent UC-a u `VvFloatingForm` **lomi `this.Parent.Parent`**. Ovo je najopasnija
sprega za detach.

### 1.11 Statički sifrar cache-evi

`VvUserControl` drži class-level statičke cache-eve (`KupdobSifrar`, `KplanSifrar`,
`ArtiklSifrar`, itd.) koji se dijele između svih UC instanci. `SetSifrarAndAutocomplete<T>()`
poziva `ZXC.SetMainDbConnDatabaseName()` koja mutira **shared globalnu DB konekciju** —
concurrent refresh iz dvije detached tab-e = race.

### 1.12 ZXC globalno stanje (kompletan scope)

| Kategorija | Primjeri | Detach utjecaj |
|---|---|---|
| DB konekcije | `TheMainDbConnection`, `TheSecondDbConnection`, `TheThirdDbConnection`, `TheSkyDbConnection`, `TheMbfDbConnection` | **Kritično** — sve dijeljene, s `ChangeDatabase()` |
| `*_InProgress` flagovi | ~50 (popis u §1.14) | Varira — v. klasifikaciju |
| Deployment provjere | `IsTEXTHOshop`, `IsPCTOGO`, `IsSvDUH`, `IsTETRAGRAM_ANY` | Sigurno (read-only per session) |
| Lookup liste | ~150 `VvLookUpLista` | Sigurno (read-only shared) |
| Temp workspace recordi | `NalogRec`, `FakturRec`, `MixerRec`, `AmortRec`, `DevTecRec` | **Visoko** — temp workspace business logike |

### 1.13 Infrastrukturne `ZXC.TheVvForm` ovisnosti (najkritičnije)

Interno u `ZXC.cs` ključne točke:

| Linija (cca) | Kod | Severity |
|---|---|---|
| 424 | `PrjConnection` getter → `TheVvForm.GetvvDB_prjktDB_name()` | **KRITIČNO** — svaka DAO operacija |
| 990 | `VvDB_NameConstructor()` → `TheVvForm.GetvvDB_prefix()` | **KRITIČNO** — svako DB ime |
| 601 | `TheSkyDbConnection` → `TheVvForm.GetvvDB_prefix()` | Visoko — Sky sync |
| 5432 | `aim_log_file_name()` → `TheVvForm.Get_MyDocumentsLocation_…()` | Srednje |
| 7173–7189 | `SetStatusText()` / `ClearStatusText()` → `TheVvForm.TStripStatusLabel` | Srednje |
| 7361 | `VvSerializedDR_DirectoryName` → `VvForm.GetLocalDirectoryForVvFile()` | Srednje |
| 9457–9458 | `TH_Should_ESC_DRW_Log` → `TheVvForm.TheVvUC`, `TheVvForm.TheVvTabPage.WriteMode` | Srednje |

**`PrjConnection` je najutjecajnija sprega u cijelom codebaseu** — svaka DB operacija
prolazi kroz nju. Mora biti rano extracted (Faza 1a).

**Status implementacije (Faza 1a, commiti C1–C5):** sljedeće točke iz tablice su već
riješene **delegate-based indirekcijom** (ne cached standalone properties, kako je
prvotni plan V4 pretpostavljao):

| Točka | Commit | Mehanizam |
|---|---|---|
| `aim_log_file_name()` | C4 | `ZXC.ProjectAndUserDocumentsLocationProvider` (Func<bool,string>) |
| `VvSerializedDR_DirectoryName` | C4 | `ZXC.LocalDirectoryForVvFileProvider` (Func<string,string>) |
| `SetStatusText` / `ClearStatusText` | C5 | `ZXC.StatusTextSetter` (Action<string>) + `ZXC.StatusTextClearer` (Action) |
| `ActiveDocumentHost` registar | C1 | `ZXC.RegisterDocumentHost` / `UnregisterDocumentHost` / `SetActiveDocumentHost` (tip `object` do Faze 1b) |
| `VvForm.VvSubModul` nested extract | C3 | `Framework\VvSubModul.cs` — global namespace, kao i `VvSubMenu` + `VvReportSubModul` |

**Važna spoznaja (C4):** `ZXC.PUG_ID` je computed getter iz `CURR_prjkt_rec` — **mijenja
se tijekom sesije** na project switchu. Zato `VvSerializedDR_DirectoryName` **ne smije**
biti cached property (kao što je plan §3.1a prvotno sugerirao za log/VvSerializedDR —
„set pri loginu"); mora ostati computed ili delegate-based. Ovo pravilo vrijedi za svaku
putanju koja koristi `PUG_ID` ili ostale per-project promjenljivi state.

### 1.14 `*_InProgress` flag inventar (klasifikacija)

| Kategorija | Flagovi | Scope nakon detacha |
|---|---|---|
| **Session-once** | `RISK_Cache_Checked`, `RISK_PrNabCij_Checked`, `RISK_BadMSU_Checked`, `RISK_NOTfisk_Checked`, `RISK_TtNum_Slijednost_Checked`, `DUC_UnlinkedTranses_Checked`, `Fak2NalAutomationChecked` | Global (safe) |
| **Long-running ops (process mutex)** | `RenewCache_InProgress`, `RewriteAllDocuments_InProgress`, `RepairMissingFakturEx_InProgress`, `FakturList_To_PDF_InProgress`, `RISK_VvPDFreporter_InProgress` | Global (mora blokirati sve prozore) |
| **Record-level ops** | `RISK_SaveVvDataRecord_inProgress`, `GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress`, `RISK_FinalRn_inProgress`, `RISK_Edit_RtranoOnly_InProgress`, `DupCopyMenu_inProgress` | **Per-DocumentHost** |
| **Import/export** | `Restore_FromVvXmlDR_InProgress`, `OffixImport_InProgress`, `LoadPoprat_InProgress`, `CopyOut_InProgress`, `KOPfromFUG_InProgress`, `KOPfromUGAN_InProgress` | Global (file I/O naturally serialized) |
| **Cache mgmt** | `RISK_DisableCacheTemporarily`, `RISK_DisableAutoFiskTemporarily`, `ShouldForceSifrarRefreshing`, `RISK_NewCache_InProgress` | Global |
| **Cross-DUC copy** | `RISK_CopyToOtherDUC_inProgress`, `RISK_CopyToMixerDUC_inProgress`, `MIXER_CopyToOtherDUC_inProgress` | **Per-DocumentHost** |
| **UI state** | `RESET_InitialLayout_InProgress`, `MenuReset_SvDUH_ZAHonly_InProgress`, `PutRiskFilterFieldsInProgress`, `DumpChosenOtsList_OnNalogDUC_InProgress`, `LoadIzvodDLG_isON` | **Per-DocumentHost** |
| **RISK field ops** | `RISK_ToggleKnDeviza_InProgress`, `RISK_InitZPCvalues_InProgress`, `RISK_PULXPIZX_Calc_InProgress`, `RISK_CheckPrNabDokCij_inProgress`, `RISK_CheckZPCkol_inProgress`, `RISK_PromjenaNacPlac_inProgress`, `RISK_AutoAddInventuraDiff_inProgress` | **Per-DocumentHost** |
| **Sky sync** | `SENDtoSKY_InProgress`, `RECEIVEfromSKY_InProgress`, `SENDorRECEIVE_SKY_InProgress` | Global (single server endpoint) |
| **Hardware (M2PAY)** | `M2PAY_API_Initialized`, `M2PAY_Device_Connected`, `M2PAY_AuthorizationStatus` | Global (fizički singleton) |
| **Misc** | `InitializeApplication_InProgress`, `LoadImportFile_HasErrors`, `LoadCrystalReports_HasErrors`, `VvXmlDR_LastDocumentMissing_AlertRaised`, `ForceXtablePreffix`, `GOST_SOBA_BOR_SetOtherData_InProgress`, `RISK_FiskParagon_InProgress` | Global |

**Sažetak:** ~20 flagova ostaje global, ~15 postaje per-`IVvDocumentHost`, ~15 slučaj-po-slučaj.

`ResetAll_GlobalStatusVariables()` trenutno resetira samo 2 od ~50 flagova (s TODO-om). Mora
se proširiti u Fazi 1.

`ShouldSupressRenewCache` compound property miješa global i per-host flagove — njegovu logiku
treba revidirati nakon klasifikacije.

### 1.15 DB konekcije — varijante i concurrency

Beyond `TheMainDbConnection`:

    // Second:
    TheSecondDbConn_SameDB            → ChangeDatabase(PrjConnection)
    TheSecondDbConn_SameDB_prevYear   → ChangeDatabase(prev year DB)
    TheSecondDbConn_SameDB_OtherYear(int) → ChangeDatabase(specified year)
    TheSecondDbConn_OtherDB(string)   → ChangeDatabase(arbitrary)

    // Third:
    TheThirdDbConn_SameDB             → ChangeDatabase(PrjConnection)
    TheThirdDbConn_OtherDB(string)    → ChangeDatabase(arbitrary)

Concurrent pozivi iz dva prozora (Tab A: `SameDB_prevYear`, Tab B: `SameDB`) race-aju na
istoj instanci. **Ključno za Fazu 3 (detach).**

**Mitigacija (tri opcije, odluka u Fazi 3):**

1. **Lock-based serialization** (najjednostavnije): `lock(theSecondDbConnection)` oko
   `ChangeDatabase()` + query
2. **Per-DocumentHost connection pool** (najsigurnije): svaki host dobiva svoje instance
3. **Connection-per-call** (najskalabilnije ali invazivno)

### 1.16 `VvTabPage` lifecycle nijanse

**Session-cache checks u konstruktoru** (set-once flagovi iz §1.14) — sigurno za detach
(flagovi su već `true` kad se stvori `VvFloatingForm`).

**`thisIsFirstAppereance` flag:**

- Prvo pojavljivanje: inicijalni data load, grid setup, sifrar populate
- Sljedeća: restore toolbar button state snapshot

Za detach: pri reparent-u UC-a, tab-ov `VisibleChanged` više ne reagira. Detach/reattach
flow mora eksplicitno:

- **Detach:** spremi toolbar snapshot (kao deaktivacija)
- **Reattach:** vrati toolbar snapshot; `thisIsFirstAppereance` mora ostati `false`

**`VvTabPage_Validating` blokira switch u arhivi.** Za detach: **Opcija B — dopustiti
detach, arhiva putuje s tabom** (preporuka).

---

## 2. Ciljno stanje — DevExpress mapiranje

### 2.1 Komponente

| Trenutno | DevExpress zamjena | Namespace | Komentar |
|---|---|---|---|
| `Crownwood.DotNetMagic.Forms.DotNetMagicForm` | `XtraForm` | `DevExpress.XtraEditors` | Jednostavan bazni tip. `RibbonForm` odbačen (ne koristimo Ribbon). |
| `Crownwood.TabControl` (glavni) | `DocumentManager` + `TabbedView` | `DevExpress.XtraBars.Docking2010` | **Obavezno za detach!** `TabbedView` ima built-in floating support. |
| `Crownwood.TabControl` (unutarnji u UC-ovima) | `XtraTabControl` | `DevExpress.XtraTab` | Nema detacha — lightweight kontrola |
| `Crownwood.TabPage` (za glavni) | `Document` (u `TabbedView`) | `DevExpress.XtraBars.Docking2010.Views.Tabbed` | Nije Control — wrapping oko UserControl-a |
| `Crownwood.TabPage` (za unutarnji) | `XtraTabPage` | `DevExpress.XtraTab` | 1:1 |
| `Crownwood.TreeControl` | `TreeList` (preferirano) ili native `TreeView` | `DevExpress.XtraTreeList` | TreeList = data-bound kolonski model. Alternativa: native `TreeView` za nižu kompleksnost. |
| `MenuStrip` | `BarManager` (Bar u MainMenu modu) | `DevExpress.XtraBars` | Per-form instance moguća (ključno za detach) |
| `ToolStrip` / `ToolStripPanel` | `BarManager` + `Bar` (toolbar mode) | `DevExpress.XtraBars` | Isti BarManager drži menije + toolbare |
| `ToolStripButton` | `BarButtonItem` | `DevExpress.XtraBars` | |
| `ToolStripComboBox` | `BarEditItem` + `RepositoryItemComboBox` | `DevExpress.XtraBars`, `DevExpress.XtraEditors.Repository` | |
| `Crownwood.Common.VisualStyle` enum | `UserLookAndFeel.Default.SkinName` + `SkinManager` | `DevExpress.LookAndFeel`, `DevExpress.Skins` | |

### 2.2 Ključne arhitekturne odluke

1. **Glavni tab: `DocumentManager` + `TabbedView` od Faze 2 odmah.** Iako V3 preporuka
   bijaše `XtraTabControl`, odlučujemo od početka koristiti `TabbedView` jer:
   - API je kompatibilan za običan tabbed rad (swap faza)
   - Kad dođe Faza 3 (detach), ne treba druga migracija
   - Izbjegava se dvostruko diranje istog koda (ključni argument za V4)

2. **Unutarnji UC tabovi: `XtraTabControl`.** Nema potrebe za floating, lightweight je.

3. **Per-Form `BarManager`.** Svaka forma (glavna i svaka `VvFloatingForm`) ima vlastiti
   `BarManager`. Osigurava neovisan menu/toolbar state.

4. **Skin umjesto per-form colorisanja.** `VvColors` sustav se kompletno preispisuje na
   `SkinStyle` bazi.

5. **`DocumentManager` floating: RAW custom.** Default DevExpress floating je lightweight
   child window. Za pravi top-level Form s taskbar ikonom i **neovisnim menijima/toolstripom**, koristi
   se `DocumentFloating` event + manual `VvFloatingForm` kreacija (v. Faza 3).

### 2.3 Ciljana arhitektura (dijagram)

    ┌──────────────────────────┐
    │    IVvDocumentHost       │
    │                          │
    │  + TheBarManager         │
    │  + Btn_NEW … Btn_ARH     │
    │  + Bar_Record            │
    │  + Bar_SubModul          │
    │  + Bar_Report            │
    │  + TStripStatusLabel     │
    │  + TheDbConnection       │
    │  + SetWriteMode(wm)      │
    │  + SetStatusText(text)   │
    │  + ClearStatusText()     │
    │  + AsForm                │
    └────────┬─────────┬───────┘
             │         │
    ┌────────┘         └────────┐
    ▼                           ▼
    ┌─────────────┐   ┌────────────────────┐
    │   VvForm    │   │  VvFloatingForm    │   ← uvodi se tek u Fazi 3
    │  (singleton)│   │  (per detached tab)│
    └─────────────┘   └────────────────────┘

---

## 3. Plan migracije — 4 faze

### Faza 1: **Decoupling** (pripremne apstrakcije)

**Cilj:** Izvući sve `TheVvForm`-specifične reference iz ZXC-a i business layera u
apstrakcije koje su **same po sebi nisu detach** — ali bez njih ni swap ne može
biti čist (jer bez `IVvDocumentHost` i `VvToolbarFactory` morat ćemo pri swapu
dirati istu bazu dva puta).

**Rizik: Nizak.** Pure refactor bez vizualnih promjena. Sav kod i dalje radi s Crownwoodom.

#### 1a — ZXC infrastruktura (najvažnije, rano)

- [ ] **`ZXC.PrjktDB_Name`** kao standalone property; `PrjConnection` getter više ne zove `TheVvForm.GetvvDB_prjktDB_name()`. **Postavi pri loginu.** *(kritično — svaka DAO operacija)*
- [ ] **`ZXC.VvDB_Prefix`** standalone; `VvDB_NameConstructor()` ga čita. **Postavi pri loginu.** *(kritično — sva DB imena)*
- [x] `ZXC.ActiveDocumentHost` (`object` do 1b, kasnije `IVvDocumentHost`) + `RegisterDocumentHost` / `UnregisterDocumentHost` (lista svih hostova) — **C1 ✅**
- [x] Extract `VvForm.VvSubModul` nested type u standalone `Framework\VvSubModul.cs` (zajedno s `VvSubMenu` + `VvReportSubModul`) — eliminira type-level ovisnost business layera na `VvForm` — **C3 ✅**
- [x] `ZXC.SetStatusText()` / `ClearStatusText()` → kroz **delegate sink** (`StatusTextSetter` / `StatusTextClearer`) koji VvForm postavi u `InitializeVvForm()`. U Fazi 3 tijelo delegata rutira kroz `ActiveDocumentHost` bez diranja call-siteova — **C5 ✅**
- [x] `aim_log_file_name()`, `VvSerializedDR_DirectoryName` → kroz **delegate providere** (`ProjectAndUserDocumentsLocationProvider`, `LocalDirectoryForVvFileProvider`) umjesto cached propertyja. Razlog: ovisnost o mutabilnom `PUG_ID` / `vvDB_User` koji se mijenja pri project switchu (v. §1.13) — **C4 ✅**
- [ ] **`ZXC.PrjktDB_Name`** kao standalone property; `PrjConnection` getter više ne zove `TheVvForm.GetvvDB_prjktDB_name()`. **Postavi pri loginu.** *(kritično — svaka DAO operacija)* — **C6 planirano**
- [ ] **`ZXC.VvDB_Prefix`** standalone; `VvDB_NameConstructor()` ga čita. **Postavi pri loginu.** *(kritično — sva DB imena)* — **C6 planirano**

#### 1b — `IVvDocumentHost` + `VvToolbarFactory`

- [ ] Definirati `IVvDocumentHost` interface (menu, bar_Record, bar_SubModul, bar_Report, status, WriteMode, TheDbConnection, AsForm)
- [ ] Extract `VvToolbarFactory` static klasa:
  - `CreateBar_Record(BarManager, IVvDocumentHost)` *(zasad prazna stub metoda — tek u Fazi 2 gradi BarManager; u Fazi 1 factory samo drži signature)*
  - `CreateBar_Report(…)`, `CreateBar_SubModul(…)`, `CreateMenuBar(…)`
  - `ApplyWriteMode(IVvDocumentHost, WriteMode)` — **TU se preseljava sva logika iz `SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB`** (7 specijalnih case-ova iz §1.6)
  - `ApplyProductTypeFilter(…)` — Surger/Remonster filtriranje iz `InitalizeToolStrip_Modul()`
- [ ] `VvCommands` static klasa — extract event handlere iz `VvForm` partial fileova
- [ ] `VvForm` implementira `IVvDocumentHost`
- [ ] Svi pozivi `SetVvMenuEnabledOrDisabled_*()` preusmjeriti u `VvToolbarFactory.ApplyWriteMode(DocumentHost, wm)` (u Fazi 1 unutar factoryja i dalje pristupamo `ToolStrip.Items[…]` — factory apstrahira target, ne tehnologiju)

#### 1c — `VvUserControl` decoupling

- [ ] Na `VvUserControl` dodati `IVvDocumentHost DocumentHost { get; set; }` property (default: `ZXC.TheVvForm`)
- [ ] Settable `TheVvTabPage` property (fallback na `Parent.Parent`):- [ ] `TheDbConnection` fallback preko `DocumentHost.TheDbConnection`

#### 1d — Business layer decoupling

- [ ] `Rtrans.Get_S_KC_fromScreen()`, `Get_S_OrgPakKol_fromScreen()`, `CheckZtrColExists()` → rutirati kroz argument `IVvDocumentHost host` (ili preko `ZXC.ActiveDocumentHost`)
- [ ] `VvUserControlRecord_Sub.grid_CellEnter_SetStatusText` / `grid_CellLeave_RestoreStatusText` → kroz `DocumentHost`

#### 1e — Flag klasifikacija i reset

- [ ] Po tablici §1.14, označiti svaki flag kao `global` / `per-host` / `TBD`
- [ ] Per-host flagove premjestiti u `IVvDocumentHost` implementacije (na `VvForm` zasad sve; kasnije i na `VvFloatingForm`)
- [ ] Revidirati `ShouldSupressRenewCache` compound logiku
- [ ] `ResetAll_GlobalStatusVariables()` proširiti da pokriva sve global flagove + osigurati per-host reset path (poziv iz `UnregisterDocumentHost`)

#### 1f — Audit (mapa dodira)

- [ ] Audit `ZXC.TheVvForm.*` u: `Rtrans.cs`, `PTG_DUC.cs`, `Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs`, `PlacaDUC_Q.cs`, `SmdDUC.cs`, `ZahtjeviDUC.cs`, `ZahtjevRNMDUC.cs`, `ZLJ_DUC.cs`, `UrudzbeniDUC.cs`, `ArtiklUC.cs`, `VvFindDialog.cs` + interne ZXC putanje (§1.13)
- [ ] Audit `SetSifrarAndAutocomplete<T>` `ChangeDatabase()` concurrency surface
- [ ] Audit `VvForm.TheVvTabPage.WriteMode` pristupa — gdje god može biti `DocumentHost.ActiveTab.WriteMode`

**DeliverableFaze 1:** aplikacija radi **identično** kao prije. Crownwood je još uvijek na
mjestu. Ali svi `TheVvForm` dodiri su apstrahirani.

**Procjena: 12–17 radnih dana.**

### Faza 2: **SWAP** (Crownwood → DevExpress, bez novih feature-a)

**Preduvjet:** Faza 1 potpuno završena i testirana.

**Cilj:** Zamijeniti sve Crownwood reference DevExpress ekvivalentima. Nikakav novi
feature — detach je uFazi 3. Nakon Faze 2 aplikacija se mora ponašati **bit-po-bit
identično** kao prije migracije.

**Rizik: Srednji.** Veliki broj fajlova dotaknut, ali apstrakcije iz Faze 1 minimiziraju dubinu.

#### 2a — Infrastrukturna priprema

- [ ] Odabrati DX verziju (preporuka: najnoviji 23.x LTS s .NET Framework 4.8 podrškom)
- [ ] Dodati NuGet pakete: `DevExpress.Win.Design`, `DevExpress.Win.Navigation`, `DevExpress.Win.TreeList`, `DevExpress.Data`, `DevExpress.Utils`, `DevExpress.Images`, `DevExpress.XtraEditors`, `DevExpress.XtraBars`, `DevExpress.XtraTab`, `DevExpress.XtraBars.Docking2010`
- [ ] Dodati `licenses.licx` sa svim korištenim komponentama
- [ ] `Program.cs Main()`: `UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful)` (ili odabrani default)
- [ ] Build solucije bez ikakve druge izmjene — validacija da DX reference ne lome kompilaciju

#### 2b — `VvForm` bazna klasa

- [ ] `Crownwood.DotNetMagic.Forms.DotNetMagicForm` →`DevExpress.XtraEditors.XtraForm`
- [ ] Ukloniti property-je koji više ne postoje (`Style`, `InertForm`, …)
- [ ] Smoketest: forma se otvara, stari MenuStrip/ToolStrip rade normalno

#### 2c — Glavni `TheTabControl`: `Crownwood.TabControl` → `DocumentManager` + `TabbedView`

- [ ] Zamijeniti kreaciju u `VvForm` (tip + event wire-up)
- [ ] **Extract logiku iz `VvTabPage_VisibleChanged`** u dvije metode na `VvTabPage`:
`OnActivated()` i `OnDeactivated()`. *(ovo je najosjetljivija operacija cijele migracije — ~100 redaka specijalne logike)*
- [ ] Handler na `TabbedView.DocumentActivated` → `((VvTabPage)e.Document.Control).OnActivated()` + `DocumentDeactivated` → `OnDeactivated()`
- [ ] Migrirati `VvTabPage_Validating` logiku u `TabbedView.DocumentClosing` ili `QueryControl` equivalent s `e.Cancel = isArchiveMode`
- [ ] Testirati: otvaranje taba, switching, arhiva blokada, dirty flag, Crystal Reports BackgroundWorker lifecycle
- [ ] **Eksplicitno ugasiti built-in floating u Fazi 2:** `TabbedView.AllowFloating= false` (ili ekvivalent) — tek u Fazi 3 ga uključujemo

#### 2d — `VvTabPage` bazna klasa

- [ ]`Crownwood.DotNetMagic.Controls.TabPage` → rješenje mora biti kompatibilno s `TabbedView` modelom. Dvije opcije:
- **Opcija 1 (preferirano):** `VvTabPage` postaje `UserControl` (nasljeđuje `Control`), a `TabbedView.Document` ga drži kao `.Control`. Više nije `TabPage` uopće.
- **Opcija 2:** zadržati `XtraTabPage` naslijeđe za unutarnje, a `TabbedView.Document` kao wrapper nad `VvTabPage` kao `Control`.
- [ ] `Title` → per model: `Document.Caption = …`
- [ ] `Image` → `Document.ImageOptions.Image = VvIco.Dirty.ToBitmap()` (ili analog za `TabbedView`)
- [ ] `Selected =true` → `TabbedView.ActivateDocument(document)`
- [ ] `thisIsFirstAppereance` flag preživjeti u novom modelu (postavi `false` nakon prve aktivacije)

#### 2e — `VvInnerTabPage` bazna klasa

- [ ] `Crownwood.DotNetMagic.Controls.TabPage` → `XtraTabPage`
- [ ] `Title` → `Text`, `Image` → `ImageOptions.Image`
- [ ] Svaki UC koji drži `Crownwood.TabControl` kao polje → `XtraTabControl`

#### 2f — UC-ovi s unutarnjim TabControl-ima (po prioritetu)

Poredak od najmanjeg do najvećeg rizika:

1. [ ] `UtilsEtc\VvAboutBox.cs` — izoliran
2. [ ] `VvUC\PlaUC\PersonUC.cs`
3. [ ] `VvUC\MixerUC\UgovoriDUC.cs`, `ZahtjeviDUC.cs`
4. [ ] `VvUC\PrjUC\SkyRuleUC.cs`
5. [ ] `VvUC\FinUC\Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs`
6. [ ] `VvUC\RiskUC\PTG_DUC.cs`
7. [ ] `Framework\VvUserControlRecord_Sub.cs`
8. [ ] `VvUC\FakturExtDUC.cs`
9. [ ] `VvUC\RiskUC\FUG_PTG_UC.cs` (PolyGrid)

Za svaki: (a) tippolja, (b) instantacija, (c) `SelectedTab` → `SelectedTabPage`,
(d) event rename `SelectedIndexChanged` → `SelectedPageChanged` s `TabPageChangedEventArgs`.

#### 2g — `MenuStrip` + `ToolStrip` → `BarManager`

- [ ] `VvForm` dobiva `BarManager` (jedan per forma)
- [ ] `VvToolbarFactory` metode (iz Faze 1b bile su stubovi) sada grade `Bar` objekte s `BarButtonItem`,`BarSubItem`, `BarEditItem`
- [ ] Data-driven kreacija iz `VvMenu[]` struktura (§1.5) mapira se izravno u DX `BarItem`-e — struct array ostaje nepromijenjen
- [ ] Shortcut keys (`vvSubMenu.shortKeys`) → `BarItem.ItemShortcut`
- [ ] `VvEnvironmentDescriptor` perzistencija: DX ima `BarManager.SaveLayoutToXml` / `RestoreLayoutFromXml`, ALI naš postojeći merge pattern (novi gumbi u kodu →dodani s `visible=false`) DX ne podržava direktno. **Implementirati custom merge step** koji radi istu stvar nad DX layout XML-om.
- [ ] Migracija postojećih user `VvEnvironmentDescriptor.xml` fileova — lookup tablica: stari `VisualStyle` string → novi skin name
- [ ] Enable/disable po WriteMode sada ide kroz `BarButtonItem.Enabled` (factory metoda `ApplyWriteMode` iz Faze 1b pretače target sa `ToolStripItem` na `BarButtonItem`)

#### 2h — `TreeView_Modul`: `Crownwood.TreeControl` → `TreeList` (ili native `TreeView`)

- [ ] Odluka: `TreeList` (bogatije API, DX izgled) vs native `TreeView` (najmanji rizik, gotovo identičan Crownwood API). **Preporuka V4: `TreeList`** — svi ostali kontroli su DX, konzistencija je bitnija od jednostavnosti.
- [ ] Konfigurirati 1 `TreeListColumn`, populate preko `AppendNode`
- [ ] Event mapping: `AfterSelect` → `FocusedNodeChanged`, ikone preko `SelectImageIndex`

#### 2i — Skin sustav (`VvColors` + `VvColorsStylsDlg`)

- [ ] Izbaciti `Crownwood.Common.VisualStyle` iz `VvColors.cs`
- [ ] Napraviti **lookup tablicu `OldStyleName → NewSkinName`** za migraciju user preferenci:

| Crownwood VisualStyle | DevExpress Skin |
|---|---|
| `IDE2005` | `Visual Studio 2013 Light` |
| `Office2003` | `Office 2007 Silver` |
| `Office2007Blue` | `Office 2019 Colorful` |
| `Office2007Black` | `The Bezier` |
| `Office2007Silver` | `Office 2019 Black` |

- [ ] `VvColorsStylsDlg.cs` — ne prikazuje više Crownwood enum, nego listu `SkinManager.Default.Skins`
- [ ] `VvEnvironmentDescriptor` load path detektira stari format i primjenjuje lookup

#### 2j — `VvHamper` — zadržati, decouplati od Crownwooda

- [ ] `VvHamper.ApplyVVColorAndStyleTabCntrolChange(this)` signature mijenja se s Crownwood `TabPage` na novi tip
- [ ] QUN-based grid sizing u `VvDocumentRecordUC` (ColumnHeadersHeight, RowTemplate.Height, RowHeadersWidth) **ostaje nepromijenjeno** — VvHamper nije Crownwood-specific u toj ulozi
- [ ] Ostali VvHamper layout putevi (`ZXC.Redak`, `ZXC.Kolona`) — nema Crownwood ovisnosti, nema promjene

#### 2k — Cleanup

- [ ] Ukloniti sve `using Crownwood.DotNetMagic.*` direktive
- [ ] Ukloniti Crownwood DLL iz `packages.config` / `PackageReference` i `References`
- [ ] UklonitiCrownwood DLL iz deploy skripta / installer-a
- [ ] Fullregression test po svakom modulu: FIR outbound, FUR inbound, plaće, amortizacija, izvještaji, TEXTHOshop varijanta, PCTOGO varijanta, SvDUH varijanta
- [ ] Usporediti screenshot-e prije/poslije za referentne scenarije

**Deliverable Faze 2:** produkcijski-spremna aplikacija bez ijedneCrownwood reference,
ponašanje identično. Ovo je **kandidat za release** — može se mjesecima koristiti
u produkciji prije Faze 3.

**Procjena: 10–15 radnih dana** (ubrzano zbog Faze 1 koja je većobavila 60% decouplinga).

### Faza 3: **FLOATING-DETACH** (novafunkcionalnost)

**Preduvjet:** Faza 2 u produkciji, stabilna, ekstenzivno testirana.

**Cilj:** Povlačenje taba van glavne forme kreira pravutop-level formu s taskbar
ikonom i **vlastitim neovisnim menijem/toolstripom**. Više detached formi koegzistira.
Zatvaranje detached forme vraća tab u glavnu formu.

**Rizik: Srednji.** Dobro izoliran od Faze 2 zahvaljujući apstrakcijama iz Faze 1.

#### 3a — `VvFloatingForm`

- [ ] Klasa `VvFloatingForm : XtraForm, IVvDocumentHost`
- [ ]Vlastiti `BarManager`, vlastiti `Bar_Record`, `Bar_SubModul`, `Bar_Report` preko `VvToolbarFactory`
- [ ] `VvToolbarFactory.CreateMenuBar(…, isDetached: true)` parametar — detached ima reduciraniji meni (npr. nema „Nova SubModul tab" opcije, ili ih ima s efektom u originalnoj formi)
- [ ] Status bar na formi s vlastitim `TStripStatusLabel`

#### 3b — Detach flow

  User povlači tab van TabbedView-a
        │
        ▼
  TabbedView.DocumentFloating event
        │
        ▼
  e.Cancel = true (sprječavamo default lightweight floating)
        │
        ▼
  new VvFloatingForm(sourceTabPage):
    ├── Create BarManager + Bar_Record/SubModul/Report preko VvToolbarFactory
    ├── Reparent VvUserControl iz VvTabPage → this.Controls
    ├── UC.DocumentHost = this
    ├── UC.TheVvTabPage = sourceTabPage  (PRESERVE — ne resetiraj!)
    ├── Savetoolbar snapshot (snimi stanje kao deaktivaciju taba)
    ├── ZXC.RegisterDocumentHost(this)
    ├── ApplyWriteMode(this, sourceTabPage.WriteMode)
    └── this.Show()

#### 3c — Reattach flow

  User zatvara VvFloatingForm
        │
        ▼
  FormClosing event:
    ├── Ukloni UC iz this.Controls
    ├── Re-attach UC natrag u sourceTabPage.Controls
    ├── UC.DocumentHost = ZXC.TheVvForm
    ├── UC.TheVvTabPage = null (revert na Parent.Parent putanju)
    ├── Restore toolbar snapshot (restore kao aktivaciju)
    ├── ZXC.UnregisterDocumentHost(this)
    └── Dispose

#### 3d — DBkonekcije concurrency

- [ ] Odluka: Lock-based vs per-host pool (vidi §1.15)
- [ ] **Preporuka V4: Lock-based** u prvoj iteraciji (manja invazivnost). Poolje eskalacija.
- [ ] Za svaku accessor metodu `TheSecondDbConn_*` / `TheThirdDbConn_*`: wrap `ChangeDatabase()` + query u `lock(theConnection)` blok
- [ ] Testirati: dva prozora + simultani dokumenti s pozivima `TheSecondDbConn_SameDB_prevYear` vs `TheSecondDbConn_SameDB`

#### 3e — Per-host `*_InProgress` flagovi

- [ ] Preseliti ~15 flagova iz §1.14 s `[per-DocumentHost]` oznakom iz ZXC statics na `IVvDocumentHost` instance state
- [ ] Revidirati `ShouldSupressRenewCache` da zbraja global + active host flagove
- [ ] Test: tab A save u toku, tab B drugi save → očekivano behavior (blokiran ili ne, prema record-level flagu)

#### 3f — M2PAY hardware guard

- [ ] Prije pokretanja M2PAY transakcije, provjeriti `M2PAY_API_Initialized` + dodatni process-level mutex — samo jedan host smije imati aktivnu transakciju
- [ ] Prikazati UXporuku ako drugi host pokušava: „Plaćanje je u tijeku u prozoru {X}, zatvorite ili dovršite ga prvo."

#### 3g — Status bar routing

- [ ] Svaka `VvFloatingForm` ima vlastiti status label — `DocumentHost.SetStatusText()` piše nasvoj, ne na glavne forme
- [ ] Testirati: grid CellEnter/CellLeave statustext prikazuju se samo u prozoru u kojem je grid aktivan

#### 3h — Edge case-ovi

- [ ] Crystal Reports BackgroundWorker udetached formi — mora živjeti na `VvFloatingForm` context-u, ne na glavnom
- [ ] Shortcut keys — samo fokusirani `BarManager` dobiva input
- [ ] Arhiva mode u detached: dopušteno (Opcija B iz §1.16) — WriteMode putuje s tabom
- [ ] Zatvaranje glavne forme s otvorenim detached tabovima: prompt user-u, sve zatvoriti ili reattach-ati
- [ ] Crashu detached formi: oporavak — reattach UC na glavnu ili graceful dispose
- [ ] `VvEnvironmentDescriptor` — zasad NE perzistirati pozicije detached prozora. Ako se pokaže potreba, dodati `List<VvFloatingFormState>` u descriptor.

#### 3i — UX polish

- [ ] Title bar detached forme: „Vektor — {ModulName}/ {SubModulName} — {WriteMode}"
- [ ] Taskbar ikona ista kao glavna
- [ ] Detach gesture: drag tab van granice `TabbedView`-a (standard DX gesture)
- [ ] Reattach gesture: drag title bar detached forme natrag u glavnu → `Document` se vraća

#### 3j — Testiranje

- [ ] WriteMode neovisnost (glavni prozor:Edit; detached: Browse)
- [ ] DB konekcije pod concurrent load
- [ ] Flag izolacija (record-level ops u 2 prozora)
- [ ] M2PAY guard
- [ ] Sifrar cache refresh concurrency
- [ ] Crystal Reports u detached
- [ ] Rtrans `Get_S_KC_fromScreen` ispravno gađa **vlastiti** FakturDUC (ne ZXC.TheVvForm-ov)
- [ ] Reattach nakon dugog rada
- [ ] Memory leak provjera — detach/reattach ciklus 100×

**Procjena: 6–9 radnih dana** (zahvaljujući Fazi 1 koja je već napravila decoupling).

### Faza 4: **Finalni cleanup i dokumentacija**

- [ ] Ukloniti dead code (stari `SetVvMenuEnabledOrDisabled_*` metode ako nisu u factoryju završile)
- [ ] Ukloniti transient migrationcode (lookup tablice za VisualStyle → SkinName nakon što su svi useri migrirani)
- [ ] Update `copilot-instructions.md` (nema više Crownwooda u „Do Not" sekciji; dodaj DX konvencije)
- [ ] User-facing dokumentacija za detach (Croatian)

**Procjena: 2–3 dana.**

---

## 4. Rizici i mitigacija — objedinjena tablica

| # | Rizik | Faza | Mitigacija |
|---|---|---|---|
| R1 | `PrjConnection` je na kritičnom putusvake DB operacije | 1a | Extract `ZXC.PrjktDB_Name` rano, prije bilo kakve druge promjene. **Napomena (C2):** `TheThirdDbConn_SameDB` je ranije provjeravao `.Database != PrjConnection` i bacao `MySqlException` pri uklanjanju `GetvvDB_` indirekcije; fix — usporedba ide s backing fieldom `theMainDbConnection.Database`, ne s izvedenim `PrjConnection`-om. Ako netko u Fazi 1b/2 ponovno dira DB connection accessore, ovo mora ostati. |
| R2 | `VvTabPage_VisibleChanged` ima ~100 redaka specijalne logike | 2c | Extract u`OnActivated`/`OnDeactivated` s identičnim grananjem; unit testove ponašanja napisati prije |
| R3 | `Parent.Parent` navigacija puca pri reparent-u | 1c | Settable `TheVvTabPage` property — fallback tek ako property nije postavljen |
| R4 | `Rtrans.Get_S_KC_fromScreen()` business ↔ UI sprega | 1d | Kroz `DocumentHost`/ argument injection |
| R5 | Shared DB connections race pri detachu | 3d | Lock-based serialization (prva iteracija) |
| R6 | `*_InProgress` ad-hoc mutex flagovi | 1e, 3e | Klasifikacija + per-host scope za ~15 flagova |
| R7 | Sifrar `ChangeDatabase()` race | 3d | Isti lock mehanizam pokriva i sifrar refresh |
| R8 | M2PAY hardware singleton | 3f | Process-wide mutex + UX poruka |
| R9 | `VvEnvironmentDescriptor` merge logika | 2g | Custom merge korak iznad DX `SaveLayoutToXml` |
| R10 | `VisualStyle` user preference migracija | 2i | Lookup tablica pri prvom loadu starog formata |
| R11 | `VvHamper` QUN grid sizing u svakom DUC-u | 2j | VvHamper ostaje; samo jedna metoda (`ApplyVVColorAndStyleTabCntrolChange`) se prilagođava |
| R12 | Crystal Reports BackgroundWorker u detached | 3h | Worker instance per-host, nijestatički |
| R13 | Crownwood `.resx` serijalizirani resursi (ako postoje) | 2b | Grep `.resx` fajlova pri swapu `DotNetMagicForm` base klase |
| R14 | Dormant `SENDtoSKY` loggingreferencira `TheVvForm` (komentirano) | 1a | Ako se ikadre-enable-a, ići kroz `ActiveDocumentHost` |
| R15 | Global data recordi (`NalogRec`, `FakturRec`…) kao temp workspace | 3e (iliraniji) | Preseliti u per-operation lokale gdje moguće; inače per-host |

---

## 5. Matrica napora (sumarno)

| Faza | Opis | Dana | Ključni driver |
|---|---|:---:|---|
| 0 | Priprema (DX odabir, training, setup) | 2–3 | DX verzija, licenca, skin test |
| 1 | Decoupling (ZXC, IVvDocumentHost, factory, VvUC, Rtrans, flagovi) | 12–17 | `PrjConnection` + `VvDB_Prefix` + ~50 flagova + ~12 fajlova audit |
| 2 | SWAP Crownwood → DevExpress | 10–15 | `VisibleChanged` extrakcija, BarManager migracija,~18 fajlova |
| 3 | FLOATING-DETACH | 6–9 | `VvFloatingForm`, DB concurrency, per-host flagovi, M2PAY guard |
| 4 | Final cleanup +docs | 2–3 | — |
| **Σ** | | **32–47** | |

**Preporučeni timing:**

- Faze 0–2idu **sekvencijalno, bez pauza** (od odluke do produkcijskog releasea swap-faze: ~6–8 tjedana s 4h/dan).
- Između Faze 2 i Faze 3: **obavezno najmanje 1–2 mjeseca produkcijskog korištenja swap-verzije** da se detektiraju suptilni bugovi prije uvođenja nove funkcionalnosti.
- Faza 3 zatim u mirnom tempu (~2 tjedna) + ekstenzivno testiranje prije produkcije.

---

## 6. Otvorena pitanja (za odluke prije početka)

### Odluke donesene za Faze 0–2 (SWAP)

1. **DX verzija i licenca.** ✅ **ODLUČENO:** `DevExpress WinForms Controls v25.2.6` (Licensed).
   - Koristi se postojeća licenca; sve potrebne komponente (Bars, TreeList, XtraTab, XtraEditors, Docking2010) pokrivene su u ovoj verziji.
   - Fiksirati točno ovu verziju u Koraku 2a NuGet paketa (ne „najnoviji 23.x LTS" kako je prvotno predložio V4).
   - Napomena: minimum target `.NET Framework 4.5.2+` zadovoljen (projekt je na 4.8).

2. **DX skin default.** ✅ **ODLUČENO:** `Office 2019 Colorful` (prihvaćen V4 prijedlog).
   - Postavlja se u `Program.cs Main()`: `UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful)`.
   - Primjenjuje se samo ako korisnik nema saved preference u `VvEnvironmentDescriptor`.

3. **TreeList vs native TreeView za `TreeView_Modul`.** ✅ **ODLUČENO:** `TreeList` (prihvaćen V4 prijedlog).
   - Konzistentnost s ostalim DX kontrolama nadjačava niži rizik native `TreeView`-a.
   - Implementacija: 1 `TreeListColumn`, populate preko `AppendNode`, event `FocusedNodeChanged`, ikone preko `SelectImageIndex` (v. §2.1 / Korak 2h).

### Progres Faze 1a (commit-level tracker)

| Commit | Opseg | Status |
|---|---|---|
| C1 | `ZXC.ActiveDocumentHost` skelet (object-typed; Register/Unregister/SetActive, thread-safe) | ✅ |
| C2 | `TheThirdDbConn_SameDB` MySqlException fix + uklanjanje `GetvvDB_` indirekcije iz ZXC-a | ✅ |
| C3 | `VvSubModul` / `VvSubMenu` / `VvReportSubModul` extrakcija iz nested `VvForm` tipa u `Framework\VvSubModul.cs` (global namespace) | ✅ |
| C4 | `ZXC` path provideri — `ProjectAndUserDocumentsLocationProvider`, `LocalDirectoryForVvFileProvider` (delegate-based zbog `PUG_ID` mutabilnosti) | ✅ |
| C5 | `ZXC` status text sink — `StatusTextSetter`, `StatusTextClearer` (delegate-based; u Fazi 3 rutira kroz `ActiveDocumentHost` bez diranja call-siteova) | ✅ |
| C6 | `ZXC.VvDB_prjktDB_Name` dead backing field + dead setter uklonjeni; `PrjConnection` i `VvDB_NameConstructor()` potvrđeno čitaju direktno iz ZXC-a (ne kroz `TheVvForm.Getvv*()`). Build green. Napomena: `VvForm.GetvvDB_prjktDB_name()` / `GetvvDB_prefix()` / `Getvv_PRODUCT_name()` **zadržani** — koriste se interno u `VvForm` partial fileovima; plan je ukloniti ih u Fazi 1f audit-u. | ✅ |
| C7 | `ResetAll_GlobalStatusVariables()` proširenje po klasifikaciji §1.14 — resetirani svi Global-scope flagovi (Session-once, Long-running, Import/export, Cache mgmt, Sky sync, Misc) + M2PAY authorization status; `SENDorRECEIVE_SKY_InProgress` preskočen jer je computed property (OR dvaju flagova); M2PAY_API_Initialized / M2PAY_Device_Connected namjerno ne resetirani (reflektiraju hardverski state). Dodan sibling `Reset_PerHost_StatusVariables_ForAllHosts()` kao placeholder do Faze 1b — trenutno defanzivno resetira per-host flagove (Record-level, Cross-DUC copy, UI state, RISK field ops) na ZXC static razini; u Fazi 1b iteracija DocumentHosts pozvat ce host.ResetPerHostStatusVariables(). Zove se iz `VvForm_Q` project-switch putanje (`tsCbxVvDataBase_SelectedIndexChanged_JOB`). Build green. | ✅ |
| C8 | **Faza 1b kick-off.** Kreiran `Framework\IVvDocumentHost.cs` — apstrakcija host-a dokumenta (Bar_Record, Bar_Report, Bars_SubModul, TheMenuStrip, TStripStatusLabel, TheDbConnection, SetStatusText, ClearStatusText, AsForm). Tipovi u Fazi 1 namjerno `System.Windows.Forms.ToolStrip / MenuStrip / ToolStripStatusLabel` — u Fazi 2g retypira se u `DevExpress.XtraBars.Bar` / `BarManager`. `TheTabControl` namjerno NIJE izložen kroz interface (Crownwood→TabbedView swap u Fazi 2c ne smije curiti). Kreiran partial `zVvForm\VvForm_IVvDocumentHost.cs` — `VvForm` implementira interface uglavnom kroz **explicit interface implementation** koja delegira na postojeće public membere (`menuStrip`, `ts_Record`, `ts_Report`, `ats_SubModulSet`); postojeći call-siteovi ostaju nepromijenjeni. `SetStatusText`/`ClearStatusText` na VvForm implementiraju se s istim Invalidate/Update/Refresh pattern-om kao C5 delegate (ZXC delegat može kasnije rutirati kroz `ActiveDocumentHost` umjesto direktno u Fazi 3). `ZXC.ActiveDocumentHost` ostaje tipiziran kao `object` do Faze 1b završetka — retype u `IVvDocumentHost` tek kada svi potrošači budu migrirani. Build green. | ✅ |
Postojeći `VvForm.SetVvMenuEnabledOrDisabled_*` call-siteovi unutar VvForm partial fileova **nisu dirani** — i dalje zovu direktno. Build green. | ✅ |
| C10 | **Option B — `ApplyWriteMode` kontrakt formaliziran, tijelo ostaje na VvFormu do Faze 2g.** Strateška odluka u Fazi 1b: tijelo `SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB` (svih 7 specijalnih case-ova iz §1.6: IsTEXTHOshop × Inventura exception, IsPCTOGO × 4 varijante, KDCDUC, IsSvDUH_ZAHonly × 2) **NE seli se fizički** u `VvToolbarFactory`. Razlog — duboke ovisnosti na VvForm-private stateu: `aMainMenu[][]`, `aSubTopMenuItem[][]`, `TheVvUC` type-checks (~10 DUC tipova), `TheVvTabPage.ArhivaTableIsNotEmpty`. Premjestaj bi zahtijevao interface-pollution (IVvDocumentHost bi izlagao te članove kao Crownwood-era `ToolStripButton` kolekciju), što je u suprotnosti s atomic-commit principom Faze 1b + curilo bi Crownwood tipove kroz neutralni interface. Fizički premjestaj odgođen za Fazu 2g — tada target flipa na DX `BarButtonItem`, pa se retypiranje i premjestaj rade istovremeno u jednom koraku. **U C10 promijenjeno:** (1) doc-anchor blok iznad `_JOB` u `zVvForm\Menus_ToolStrips.cs` koji enumerira svih 7 case-ova + AOLD/ANEW/ARH/SAS specials kao referencu za buduće čitatelje; (2) `VvToolbarFactory.ApplyWriteMode` XML doc i file header apdejtirani — sada službeno proglašavaju delegaciju **definitivnim kontraktom Faze 1b** (ne više "privremenim" kako je stajalo u C9); (3) `ApplyProductTypeFilter` komentar također pomaknut na Fazu 2g (C10/C11 napomena uklonjena). **Ulazni kontrakt** za buduće call-siteove (business layer u Fazi 1d, `VvFloatingForm` u Fazi 3): ulaze isključivo kroz `VvToolbarFactory.ApplyWriteMode(host, wm)` — nikada direktno u VvForm metodu. Nulta promjena ponašanja; build green. | ✅ |
Ostale varijante (`SetVvMenuEnabledOrDisabled_NoTabPageIsOpened`, `_FilterTabPageIsOpened`, `_Explicitly`, `_ArhivaTabPageIsOpen`) **nisu proširivane na factory** jer po Option B principu kontrakt se ne proširuje bez stvarne potrebe — njihov vanjski surface je minimalan (3 call-sitea u `VvTabPage.cs` + `VvUserControl_Sub.cs`) i migracija im ide zajedno s `_RegardingWriteMode` tijelom u Fazi 2g kad target flipa na DX `BarButtonItem`. Nulta promjena ponašanja; build green. | ✅ |
| C13 | **Faza 1d — `Rtrans` business-layer decoupling (R4 mitigacija).** Identicna strategija kao C4/C5: **delegate-provider** indirekcija umjesto sirenja `IVvDocumentHost` interfacea UC-specifichnim tipovima. (1) `Framework\ZXC.cs` — nova regija `Active Record UC Providers (Phase 1d / C13)` s dva delegata: `Func<VvRecordUC> ActiveRecordUCProvider` i `Func<VvDocumentRecordUC> ActiveDocumentRecordUCProvider`. (2) `zVvForm\Initializations_Settings.cs` — wire-up u `InitializeVvForm()` odmah nakon `StatusTextClearer` bloka: lambde vracaju `this.TheVvRecordUC` / `this.TheVvDocumentRecordUC`. (3) `BusinessLayer\Rtrans.cs` — 4 metode refaktorirane na null-check fallback pattern (`ZXC.Active*Provider != null ? ZXC.Active*Provider() : ZXC.TheVvForm.TheVv*UC`): `GetFakturRecForZavisniPurposes` (~2701), `CheckZtrColExists` static (~2712), `Get_S_KC_fromScreen` (~2810), `Get_S_OrgPakKol_fromScreen` (~2815). Posljednje dvije se zovu iz `CalcTrans_*` grana (v. §1.8 — `INIT_Memset0Rtrans_GetZtr` putanja), ostale iz kontekst-validacije prije calc-a. **Zashto delegati a ne clanovi `IVvDocumentHost`:** izbjegava leak `VvRecordUC` / `VvDocumentRecordUC` tipova kroz neutralni interface (isti princip kao TheTabControl namjerno neizlozen — v. C8 doc). U Fazi 3 body delegata flipne da rutira kroz `ZXC.ActiveDocumentHost` — Rtrans call-siteovi ostaju nepromijenjeni, pa detached FakturDUC pri calc-u automatski gadja svoj DUC (ne glavne forme, sto bi inache bio bug). **Fallback-safe:** ako provider nije postavljen (dijagnostika prije `InitializeVvForm()`), pada na stari put. Nulta promjena ponasanja; adresira R4 (najvisi severity business-UI sprega iz §1.8). Build green. | ✅ |\n| C12 | **Faza 1c — `VvUserControl` decoupling (R3 mitigacija).**

### Otvorena pitanja za Fazu 3 (DETACH) — odgovor kasnije

4. **DB concurrency strategy za Fazu 3.** ⏳ **PENDING** — Lock-based (preporuka V4) vs per-host pool. Odluka se donosi prije početka Faze 3, nakon što Faza 2 bude u produkciji i performanse shared konekcija budu izmjerene.

5. **Detach UX.** ⏳ **PENDING** — drag-za-reattach (DX standard) vs samo close-za-reattach.

6. **Perzistencija pozicija detached prozora.** ⏳ **PENDING** — V4 preporučuje **NE** u prvoj iteraciji.

---

## 7. Što je svjesno izostavljeno iz V4 (i zašto)

Da bi V4 bio fokusiran, sljedeće **nije** uključeno ovdje. Ako želiš, svako se može tražiti
kao zaseban request:

- **Granularni per-DUC file-by-file checklist.** ~40 DUC klasa u `VvUC\*UC\` — svaki ima
specifične dodire (polja, custom handleri, specijalni grid setup). V4 pokriva high-level
grupe. Per-DUC drilldown traži fresh introspekciju svakog fajla. **Predlažem tražiti
kao „V4-DUC-Checklist" zasebno kad dođeš do Faze 2f.**
- **Konkretan C# kod `IVvDocumentHost` interface.** V4 opisuje membere. Kod bih generirao
kad započnemo Fazu 1b.
- **`VvToolbarFactory` implementacija.** Isto — kod za Fazu 1b/2g.
- **`VvFloatingForm` kod.** ZaFazu 3a.
- **Unit/integration test strategija.** Vrijedi zaseban dokument (regression harness za
swap fazu je kritičan).

---

## 8. TL;DR

- **Odluka:** SWAP prvo, DETACH kasnije. Detach je siguran ali ne-hitan.
- **4faze:** (1) Decoupling → (2) Swap → pauza za produkcijsko testiranje → (3) Detach → (4) Cleanup.
- **Faza 1 (decoupling) je najvažnija** — bez nje bismo dvaput dirali isti kod.Fokus: `PrjConnection`, `VvDB_Prefix`, `IVvDocumentHost`, `VvToolbarFactory`, flag klasifikacija, `Rtrans`decoupling, `VvUserControl.TheVvTabPage` settable.
- **Faza 2 (swap) koristi `DocumentManager` + `TabbedView` od početka** (ne `XtraTabControl`) da se izbjegne dvostruki rad kad dođe detach. Floating je privremeno isključen.
- **Faza 3 (detach) je disciplinirana:** `VvFloatingForm : IVvDocumentHost`, reparent UC-a, lock-based DB concurrency, per-host flagovi, M2PAY guard.
- **Ukupno 32–47 radnih dana** (od kojih je ~12–17 Faza 1 — najgušća).
- **15 rizika identificirano i mitigirano.** Najopasnija točka: `VvTabPage_VisibleChanged` extrakcija u Fazi 2c.
- **FLOATING-DETACH se SIGURNO radi** — samo ne u istoj milestone sa swapom.

---

## 9. Povijesni kontekst

- `DevExpress_Migration_V2.md` (2025) — prvi plan koji je miješao swap i detach u jednu fazu s 5 potfaza. Dobra analiza ZXC sprege.
- `DevExpress_Migration_V3.md` (2025) — reakcija na V2; predlagala čisti swap bez apstrakcija. Izvor tablica mapiranja kontrola.
- `DevExpress_Migration_V4.md` (ovaj, 2026) — autoritativan. Kombinira V2 dubinu analize i V3 fokus na "prvo swap", ali s **Fazom 1 decouplinga** kao obaveznim preduvjetom (izbjegava dupliranje rada iz V3 scenarija).
