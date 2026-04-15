# DevExpress WinForms Migration — Architecture & Strategy

## 1. Current State (Crownwood DotNetMagic)

### 1.1 Form Hierarchy

    VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm
    ├── menuStrip (MenuStrip) — 7 top-level menus (Datoteka, Pogled, Format, Forme, SubModul, Izvještaji, Pomoć)
    ├── tsPanel_Modul → ts_Modul (ToolStrip) — module navigation buttons
    ├── tsPanel_Record → ts_Record (ToolStrip) — CRUD + nav (NEW, OPN, DEL, SAV, ESC, FRS, PRV, NXT, LST, FND, PRN, PRW, ARH…)
    ├── tsPanel_SubModul → ats_SubModulSet[modulIdx][subModulIdx] (ToolStrip[][]) — per-submodule toolstrips
    ├── ts_Report (ToolStrip) — report operations (GO, Print, PDF, Export, Zoom, page nav)
    ├── modulPanel (Panel, DockStyle.Left|Right) — module tree navigation
    │   └── TreeView_Modul (Crownwood.DotNetMagic.Controls.TreeControl)
    ├── spliterModulPanelTabControl (Splitter)
    └── TheTabControl (Crownwood.DotNetMagic.Controls.TabControl)
        ├── VvTabPage[0] → VvUserControl (e.g., FakturDUC)
        ├── VvTabPage[1] → VvUserControl (e.g., NalogDUC)
        └── VvTabPage[N] → VvUserControl (e.g., PlacaDUC)

### 1.2 Key Classes

| Class | File | Role |
|---|---|---|
| `VvForm` | `zVvForm\*.cs` (partial, ~7 files) | Main MDI-like form. Owns ALL menus, toolstrips, tab control. Inherits `DotNetMagicForm`. |
| `VvTabPage` | `Framework\VvTabPage.cs` | Extends `Crownwood.DotNetMagic.Controls.TabPage`. Manages UC lifecycle, handles `VisibleChanged` to switch toolstrips. Owns per-tab `WriteMode`. |
| `VvUserControl` | `Framework\VvUserControl*.cs` | Base for all content UCs (FakturDUC, NalogDUC, PlacaDUC, etc.). Navigates to `VvTabPage` via `this.Parent.Parent` cast. Holds static sifrar caches. |
| `ZXC` | `Framework\ZXC.cs` | Global static state (~9800 lines). `TheVvForm` (singleton), `CurrentForm`, project flags, 5 DB connections, 150+ lookup lists, ~50 `*_InProgress` flags, TtInfo dictionary, deployment-site flags. |
| `VvEnvironmentDescriptor` | `UtilsEtc\VvEnvironmentDescriptor.cs` | XML-serialized form state (toolbar layout as `List<VvToolStripItem_State>`, position, colors, font). |

### 1.3 Tab-Toolbar Coupling (The Core Problem)

Current flow when a tab becomes active:

    VvTabPage_VisibleChanged(visible = true)
      │
      ├── GetTSB_EnabledStateSnapshot() — save outgoing tab's toolbar button states
      ├── ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet() — hide ALL ats_SubModulSet[][] except current
      ├── SetVvSubModulSetMenuEnabledOrDisabled_RegardingWriteMode() — enable/disable per WriteMode
      ├── Show/hide ts_Report vs ts_Record based on TabPageKind
      └── PutTSB_EnabledStateSnapshot() — restore incoming tab's saved toolbar button states

Additionally, `VvTabPage_Validating` **blocks tab switching** during archive mode — this will interact with detach logic.

**Critical coupling points:**

- `VvTabPage.theVvForm` — constructor takes `VvForm _vvForm` parameter, stores as private field `theVvForm`
- `ats_SubModulSet[i][j]` — 2D ToolStrip array lives on VvForm, tabs just select which one is visible
- `SetVvMenuEnabledOrDisabled_*()` — directly manipulates `ts_Record.Items["NEW"]`, etc. by string name
- `ZXC.TheVvForm` — used throughout business logic (e.g., `Rtrans.Get_S_KC_fromScreen()` casts `ZXC.TheVvForm.TheVvDocumentRecordUC`)
- `VvUserControl.TheVvTabPage` — navigates via `this.Parent.Parent` cast to reach containing `VvTabPage` — **breaks if UC is reparented**

### 1.4 Toolbar Infrastructure Detail

All menus and toolbars are **programmatically created** (no Designer). Defined via:

    // Menu definition data structures (in Menus_ToolStrips.cs)
    VvMenu[] aMainMenu = new VvMenu[] {
        new VvMenu("Datoteka",   true,  "", new VvSubMenu[] { ... 32 items ... }),
        new VvMenu("Pogled",     true,  "", new VvSubMenu[] { ... 20 items ... }),
        new VvMenu("Format",     true,  "", new VvSubMenu[] { ... }),
        new VvMenu("Forme",      true,  "", new VvSubMenu[] { }),
        new VvMenu("",           false, "SubModul", new VvSubMenu[] { }),
        new VvMenu("Izvještaji", false, "Report",   new VvSubMenu[] { ... 23 items ... }),
        new VvMenu("Pomoć",      true,  "", new VvSubMenu[] { ... }),
    };

Each `VvSubMenu` specifies: text, icon, shortcut key, event handler, and `vvMenuStyleEnum` (determines if it becomes a menu item, toolbar button, or both).

### 1.5 WriteMode Enable/Disable

Central methods on VvForm:

- `SetVvMenuEnabledOrDisabled_RegardingWriteMode(WriteMode)` — master toggle
- `SetVvMenuEnabledOrDisabled_Explicitly(string name, bool enabled)` — per-button
- `SetVvMenuEnabledOrDisabled_NoTabPageIsOpened()` — initial disabled state
- `SetVvMenuEnabledOrDisabled_FilterTabPageIsOpened()` — report filter state
- `SetVvMenuEnabledOrDisabled_ArhivaTabPageIsOpen()` — archive mode

Special cases that `VvToolbarFactory.ApplyWriteMode` must preserve:

| Condition | Buttons Affected |
|---|---|
| `IsTEXTHOshop` | Disables OPN, DEL |
| `IsPCTOGO` on certain PTG DUCs | Disables NEW, DUP |
| `IsSvDUH_ZAHonly` | Restricts IZD/ZAH buttons |
| `theVvUC is KDCDUC` | Special button set |
| Various UC type checks | Many per-DUC overrides |

### 1.6 Business Layer UI Coupling (Rtrans)

The `Rtrans` class in `BusinessLayer\Rtrans.cs` directly reaches into the UI layer:

    // Rtrans.cs — business logic directly touches UI singletons
    private decimal Get_S_KC_fromScreen()
    {
        return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_KC_fromScreen();
    }

    internal static bool CheckZtrColExists()
    {
        FakturDUC theDUC = ZXC.TheVvForm.TheVvRecordUC as FakturDUC;
        ...
    }

This pattern appears in calculation methods like `INIT_Memset0Rtrans_GetZtr()` which reads live values from the screen grid to compute dependent transport costs (`T_ztr`). This coupling **must be addressed** before tab detach can work, since a detached tab's DUC is no longer `ZXC.TheVvForm.TheVvRecordUC`.

### 1.7 Rtrans CalcTransResults Architecture

`Rtrans.CalcTransResults()` branches into three calculation paths:

    CalcTransResults(Faktur)
    ├── IsForceMalUlazCalc → CalcTrans_MALOP_Results_ULAZ()
    ├── IsMalopTT          → CalcTrans_MALOP_Results()
    │   ├── IsNivelacijaZPC → CalcTrans_MALOP_Results_ULAZ_ZPC()
    │   ├── IsFinKol_U      → CalcTrans_MALOP_Results_ULAZ_ByCIJENA()
    │   └── else            → CalcTrans_MALOP_Results_IZLAZ()
    └── else               → CalcTrans_VELEP_Results()
        └── Is_VelepByMPC  → CalcTrans_VELEP_Results_ByMPC()

All paths call `INIT_Memset0Rtrans_GetZtr(faktur_rec)` which is where the `Get_S_KC_fromScreen()` call lives. This is the **critical decoupling point**.

### 1.8 VvUserControl Navigation Chain

`VvUserControl` navigates to its owning `VvTabPage` via a fragile cast chain:

    // VvUserControl.cs
    public VvTabPage TheVvTabPage
    {
        get { return (VvTabPage)this.Parent.Parent; }
    }

And `TheDbConnection` falls back through this chain:

    public XSqlConnection TheDbConnection
    {
        get
        {
            return TheVvTabPage?.TheDbConnection
                ?? ZXC.TheVvForm.TheVvTabPage.TheDbConnection;
        }
    }

**Reparenting the UC to a VvFloatingForm will break `this.Parent.Parent`** because the WinForms control hierarchy changes. This is the single most dangerous coupling point for Phase 4.

The fallback `ZXC.TheVvForm.TheVvTabPage.TheDbConnection` is specifically for `VvFindDialog` scenarios where the UC may not have a tab parent. With `IVvDocumentHost`, this fallback should route through `DocumentHost` instead of hardcoded `ZXC.TheVvForm`.

### 1.9 Static Sifrar Caches on VvUserControl

`VvUserControl` holds class-level static caches used by all UC instances:

    public static List<Kupdob> KupdobSifrar;
    public static List<Kplan>  KplanSifrar;
    public static List<Artikl> ArtiklSifrar;
    // ... many more

These are shared across ALL UC instances — including detached ones. This is acceptable for read-only lookup data, but cache refresh operations must be coordinated because `SetSifrarAndAutocomplete<T>()` calls `ZXC.SetMainDbConnDatabaseName()` before loading sifrar data, which **mutates the shared global DB connection**. Two detached tabs triggering sifrar refresh simultaneously will race on `ChangeDatabase()`.

### 1.10 ZXC Global State Scope

`ZXC` is far larger than just `TheVvForm`. Key categories of global state:

| Category | Examples | Detach Impact |
|---|---|---|
| DB Connections | `TheMainDbConnection`, `TheSecondDbConnection`, `TheThirdDbConnection`, `TheSkyDbConnection`, `TheMbfDbConnection` | **Critical** — all shared, with `ChangeDatabase()` calls |
| `*_InProgress` flags | ~50 flags: `RISK_Cache_Checked`, `SENDtoSKY_InProgress`, `RISK_SaveVvDataRecord_inProgress`, etc. | **High** — concurrent operations from two windows could conflict |
| Deployment site checks | `IsTEXTHOshop`, `IsPCTOGO`, `IsSvDUH`, `IsTETRAGRAM_ANY` | Safe (read-only per session) |
| Lookup lists | 150+ `VvLookUpLista` instances | Safe (shared read-only) |
| Global data records | `NalogRec`, `FakturRec`, `MixerRec`, `AmortRec`, `DevTecRec` | **High** — used as temp workspace by business logic |

---

## 2. Target State (DevExpress WinForms)

### 2.1 Component Mapping

| Current                         | DevExpress Replacement                       | Notes                                        |
|---------------------------------|----------------------------------------------|----------------------------------------------|
| `DotNetMagicForm`               | `XtraForm`                                   | Simple base form                             |
| `Crownwood.TabControl`          | `DocumentManager` + `TabbedView`             | Built-in float/dock, Chrome-like detach      |
| `MenuStrip`                     | `BarManager` (`Bar` as MainMenu)             | Per-form instances possible                  |
| `ToolStrip` / `ToolStripPanel`  | `BarManager` + `Bar` (toolbar mode)          | Same BarManager manages menus + toolbars     |
| `ToolStripButton`               | `BarButtonItem`                              | Richer styling, glyph support                |
| `ToolStripComboBox`             | `BarEditItem` + `RepositoryItemComboBox`     | Integrated in bar system                     |
| `Crownwood.TreeControl`         | `NavBarControl` or `TreeList`                | Module navigation panel                      |
| `Crownwood.VisualStyle`         | DevExpress Skins (`SkinManager`)             | Unified skinning across all controls         |

### 2.2 Chrome-Like Detach — Key Requirement

> "mogućnost detach-a pojedinog tab-a koji bi trebao kreirati novu formu na taskbar-u
> ali bi trebao imati i **svoj novi set menija i toolstripova** nezavisnih o meniju
> i toolstripovima ishodišne forme"

This means:

1. Dragging a tab out creates a **real top-level Form** (taskbar entry)
2. The new form has its **own independent** menu bar, ts_Record, ts_SubModul, and ts_Report
3. The new form's toolbar state is independent (WriteMode, enabled/disabled)
4. Closing the detached form returns the tab to the main form
5. Multiple detached windows can coexist

---

## 3. Architecture Strategy

### 3.1 The IVvDocumentHost Abstraction

    ┌──────────────────────────┐
    │    IVvDocumentHost       │  ← NEW interface
    │                          │
    │  + TheBarManager         │
    │  + Btn_NEW, Btn_OPN…     │
    │  + Bar_Record            │
    │  + Bar_SubModul          │
    │  + Bar_Report            │
    │  + SetWriteMode(wm)     │
    │  + SetStatusText(text)   │
    │  + ClearStatusText()     │
    │  + AsForm                │
    └─────────┬────────┬───────┘
              │        │
    ┌─────────┘        └─────────┐
    ▼                            ▼
    ┌─────────────┐     ┌────────────────┐
    │   VvForm    │     │ VvFloatingForm │  ← NEW class
    │ (main form) │     │ (detached tab) │
    │ implements  │     │ implements     │
    │ IVvDocHost  │     │ IVvDocHost     │
    └─────────────┘     └────────────────┘

### 3.2 VvToolbarFactory

Extract toolbar creation from VvForm into a static factory:

    static class VvToolbarFactory
    {
        Bar CreateBar_Record(BarManager mgr, IVvDocumentHost host);
        Bar CreateBar_Report(BarManager mgr, IVvDocumentHost host);
        Bar CreateBar_SubModul(BarManager mgr, VvSubModulEnum subModul);
        Bar CreateMenuBar(BarManager mgr, bool isDetached);
        void ApplyWriteMode(IVvDocumentHost host, WriteMode wm);
    }

`ApplyWriteMode` must replicate all special-case UC-type checks currently in `SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB` (TEXTHOshop, PCTOGO, SvDUH_ZAHonly, KDCDUC, etc.). The factory must also accept product-type parameters to replicate per-deployment button filtering (Surger shows only PRIJEM/OPRANA modules, Remonster shows only DEBIT module).

### 3.3 VvUserControl Decoupling

Current (tight coupling):

    // VvUserControl.cs — fragile Parent.Parent navigation
    public VvTabPage TheVvTabPage
    {
        get { return (VvTabPage)this.Parent.Parent; }
    }

    // In Rtrans.cs (business layer!):
    private decimal Get_S_KC_fromScreen()
    {
        return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_KC_fromScreen();
    }

Target (decoupled):

    // VvUserControl gains:
    public IVvDocumentHost DocumentHost { get; set; }

    // TheVvTabPage uses direct reference instead of Parent.Parent:
    private VvTabPage _theVvTabPage;
    public VvTabPage TheVvTabPage
    {
        get { return _theVvTabPage ?? (VvTabPage)this.Parent?.Parent; }
        set { _theVvTabPage = value; }
    }

    // TheDbConnection fallback routes through DocumentHost:
    public XSqlConnection TheDbConnection
    {
        get
        {
            return TheVvTabPage?.TheDbConnection
                ?? DocumentHost?.TheDbConnection;
        }
    }

    // Business layer routes through DocumentHost instead of ZXC.TheVvForm

The `DocumentHost` property must be injected at the `VvUserControl` level (the root of the UC hierarchy) so all subclasses — `VvRecordUC`, `VvDocumLikeRecordUC`, `VvDocumentRecordUC`, and all concrete DUCs — inherit it.

### 3.4 ZXC Singleton Adaptation

    // ZXC (current)                 ZXC (target)
    // ─────────────                 ────────────
    // TheVvForm (singleton)    →    TheVvForm (still singleton, main form)
    // CurrentForm              →    ActiveDocumentHost : IVvDocumentHost
    //                               RegisterDocumentHost(host)
    //                               UnregisterDocumentHost(host)
    //                               AllDocumentHosts : List<IVvDocumentHost>

Additionally, two critical infrastructure dependencies on `TheVvForm` must be broken:

    // Current — every DAO operation flows through TheVvForm:
    PrjConnection → TheVvForm.GetvvDB_prjktDB_name()
    VvDB_NameConstructor() → TheVvForm.GetvvDB_prefix()

    // Target — standalone properties set once at login:
    ZXC.PrjktDB_Name (string, set at login)
    ZXC.VvDB_Prefix (string, set at login)

This is the **single most impactful decoupling** in the entire migration — more critical than any UI coupling, since every database operation flows through `PrjConnection`.

### 3.5 Detach Flow

    User drags tab out of TabbedView
            │
            ▼
    TabbedView.DocumentFloating event fires
            │
            ▼
    e.Cancel = true  (prevent default DevExpress lightweight float)
            │
            ▼
    Create new VvFloatingForm(sourceTabPage)
      ├── Create own BarManager
      ├── VvToolbarFactory.CreateMenuBar(…, isDetached: true)
      ├── VvToolbarFactory.CreateBar_Record(…)
      ├── VvToolbarFactory.CreateBar_SubModul(…, tabPage.SubModulEnum)
      ├── Reparent VvUserControl from tab → this.Controls
      ├── Set UC.DocumentHost = this
      ├── Set UC.TheVvTabPage = sourceTabPage (preserve reference!)
      ├── ZXC.RegisterDocumentHost(this)
      └── this.Show()
            │
            ▼
    User closes VvFloatingForm
      ├── Remove UC from Controls
      ├── Reattach UC to original VvTabPage
      ├── Set UC.DocumentHost = ZXC.TheVvForm
      ├── UC.TheVvTabPage = null (revert to Parent.Parent)
      ├── ZXC.UnregisterDocumentHost(this)
      └── Dispose

---

## 4. Source Code Analysis

### 4.1 VvMenu/VvSubMenu Struct Definition Maps Directly to VvToolbarFactory

All 7 top-level menus (~83 sub-items) are defined as `VvMenu[]` / `VvSubMenu[]` struct arrays. Each `VvSubMenu` carries: text, icon, shortcut key, event handler, and `vvMenuStyleEnum` (determines menu item, toolbar button, or both).

This data-driven pattern maps naturally to `VvToolbarFactory` — the same struct arrays can drive DevExpress `BarItem` creation. Phase 3 is **more tractable** than expected because the definition data is already decoupled from the creation logic.

### 4.2 SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB — The Single Extraction Point

This single method contains **all** UC-type-specific enable/disable rules:

| Condition | Buttons Affected | Specific DUC Types |
|---|---|---|
| `IsTEXTHOshop` | Disables OPN, DEL | Except `InventuraDUC`, `InventuraMPDUC` |
| `IsPCTOGO` | Disables NEW, DUP | `ANU_PTG_DUC`, `DIZ_PTG_DUC`, `PVR_PTG_DUC`, `A1_ANU_PTG_DUC`, `PRN_DIZ_PTG_DUC` |
| `IsPCTOGO` | Disables DUP only | `ZIZ_PTG_DUC` |
| `IsPCTOGO` | Disables NEW, OPN, DUP | `KOP_PTG_DUC` |
| `KDCDUC` | Disables NEW, DUP | Always |
| `IsSvDUH_ZAHonly` | Disables NEW, OPN, DEL, DUP | `IZD_SVD_DUC` |
| `IsSvDUH_ZAHonly` | Disables DEL | `ZAH_SVD_DUC` (non-super users) |

This becomes `VvToolbarFactory.ApplyWriteMode(IVvDocumentHost, WriteMode)` in Phase 1. The extraction is straightforward because all rules are concentrated in one method.

### 4.3 Product-Type Checks in Toolbar Initialization

`InitalizeToolStrip_Modul()` filters SubModul buttons by deployment site:

| Product | Visible Buttons |
|---|---|
| `Surger` | Only PRIJEM, OPRANA modules |
| `Remonster` | Only DEBIT module |
| All others | Full set |

Saves initial states in `VvTsiModulDefaultStates[]`. `VvToolbarFactory` must accept product-type parameters to replicate these filters.

### 4.4 Toolbar State Persistence Uses List-Based Merge Logic

`VvForm_Load_JOB` restores toolbar button states from `VvEnvironmentDescriptor` using a **merge** strategy:

1. Iterate current toolbar items; for each item NOT in the saved list, append it with `(name, visible=false, enabled=false)` defaults
2. Then iterate saved items and apply position/visible/available state
3. This allows new buttons added in code updates to appear in existing users' saved layouts

On save (`VvForm_FormClosing`), the pattern is `List.Clear()` → `Add()` loop for each toolbar item.

`VvToolbarFactory` must replicate this merge behavior. DevExpress `BarManager.SaveLayoutToXml` / `RestoreLayoutFromXml` may not support this out of the box — a custom merge step will be needed.

### 4.5 VvTabPage Constructor Session Cache Checks

The VvTabPage constructor checks and sets these global flags on first tab creation:

- `ZXC.RISK_Cache_Checked`
- `ZXC.RISK_PrNabCij_Checked`
- `ZXC.RISK_NOTfisk_Checked`
- `ZXC.RISK_BadMSU_Checked`
- `ZXC.DUC_UnlinkedTranses_Checked`

These are **session-level** checks (run once, then flag prevents re-run). When creating a `VvFloatingForm` in Phase 4, these flags will already be `true`, so the detached tab will correctly skip them. **No action required** — but this behavior must be preserved (do not reset flags on detach).

### 4.6 VvTabPage `thisIsFirstAppereance` Flag

`VvTabPage_VisibleChanged` has two distinct code paths:

- **First appearance** (`thisIsFirstAppereance = true`): Performs initial data load, sets up grid, populates sifrar caches
- **Subsequent appearances**: Restores saved toolbar button state snapshot (`PutTSB_EnabledStateSnapshot`)

For detached tabs, when the UC is reparented to `VvFloatingForm`, the tab page's `VisibleChanged` no longer fires. The detach/reattach flow must explicitly handle:

- **Detach:** Save toolbar snapshot (as if deactivating)
- **Reattach:** Restore toolbar snapshot (as if reactivating), but `thisIsFirstAppereance` must remain `false`

### 4.7 VvTabPage_Validating Blocks Tab Switching During Archive Mode

    VvTabPage_Validating:
      if (isArchiveMode) { show error; e.Cancel = true; }

For detach, two options exist:

- **Option A:** Prevent detaching archive-mode tabs (show error)
- **Option B:** Allow detach — archive mode travels with the tab (simpler, since the archive state is per-`VvTabPage.WriteMode`)

Recommend **Option B**.

### 4.8 VvForm_Load_JOB Opens Initial SubModul via SubModulWanted_Click

The load sequence calls `SubModulWanted_Click(VvPref.login.TheINITIAL_VvSubModulEnum, ...)` to create the first `VvTabPage`. Special overrides:

- `IsSvDUH_ZAHonly` → forces `R_ZAH_SVD` as initial SubModul
- `IsSkyEnvironment` → forces tab open regardless of preference (needed for sync)
- `VvXmlDR_LastDocumentMissing_AlertRaised` → skips opening

This means `SubModulWanted_Click` is the **tab creation entry point**. It must be examined in Phase 1 to understand the full tab creation lifecycle and identify any additional coupling.

### 4.9 UC Class Hierarchy (Complete)

    VvUserControl (abstract)
      └── VvRecordUC (abstract)
            └── VvDocumLikeRecordUC (abstract)
                  │  VirtualDocumentRecord
                  │  Put_NewDocum_NumAndDateFields()
                  │  Put_NewTT_Num()
                  └── VvDocumentRecordUC (abstract)
                        ├── TheG : VvDataGridView
                        ├── TheSumGrid : DataGridView
                        ├── VirtualTranses (from VirtualDocumentRecord)
                        ├── TheVvDaoTrans : VvDaoBase
                        ├── TheTtLookUpList
                        ├── GetDgvLineFields() — abstract
                        ├── PutDgvLineFields() — abstract
                        ├── PutDgvLineResultsFields() — virtual
                        └── PutDgvTransSumFields() — abstract

Interface `IVvHasSumInDataLayerDocumentRecordUC` adds `PutTransSumToDocumentSumFields()` for UCs that sync sums back to the document record.

### 4.10 VvDocumentRecordUC Grid Construction Uses VvHamper and ZXC.QUN

Grid construction in `VvDocumentRecordUC` is deeply tied to the layout system:

    _theGrid.ColumnHeadersHeight = ZXC.QUN;
    _theGrid.RowTemplate.Height  = ZXC.QUN;
    _theGrid.RowHeadersWidth     = ZXC.Q3un + ZXC.Qun4;

Also calls `VvHamper.ApplyVVColorAndStyleTabCntrolChange()` for styling.

VvHamper removal (Phase 5) affects not just form layout but also grid row/column sizing in every `VvDocumentRecordUC` subclass. The QUN-based sizing should be preserved even if VvHamper is decoupled from Crownwood.

### 4.11 VvEnvironmentDescriptor XML Location Is Deployment-Dependent

The env descriptor path uses `Get_MyDocumentsLocation_ProjectAndUser_Dependent()` with:

- Separate THSHOP (TEXTHOshop) variant path (`VvEnvironDesc_THSHOP.xml`)
- Backup file support (`VvEnvironmentDescriptor_Bkp.xml`)
- `SvDUH_ZAHonly` web download fallback for first-time users (from `viper.hr/vektorSVD/`)
- `IsTEXTHOshop` skip-save guard (prevents overwriting shared layout)

For detached forms: state is transient (not persisted). If we ever want to persist detached window positions, the env descriptor format would need a list of `VvFloatingFormState` entries alongside the existing toolbar state lists.

### 4.12 ZXC.TheVvForm Coupling — Complete Inventory

Direct `ZXC.TheVvForm` references exist in at least these files:

**External files (UC and business layer):**

| File | Approx. Lines | Usage Pattern |
|---|---|---|
| `BusinessLayer\Rtrans.cs` | multiple | `Get_S_KC_fromScreen()`, `CheckZtrColExists()` — casts `TheVvDocumentRecordUC` |
| `VvUC\RiskUC\PTG_DUC.cs` | ~1998-2137 | Toolbar manipulation, SubModulSet access |
| `VvUC\FinUC\Fin_Dlg_UC.cs` | ~504-684 | SubModulSet visibility, toolbar state |
| `VvUC\FinUC\Fin_Dlg_UC_Q.cs` | ~1707-1724 | Form property access |
| `VvUC\PlaUC\PlacaDUC_Q.cs` | ~154-165 | TheVvTabPage access |
| `VvUC\MixerUC\SmdDUC.cs` | ~128-145 | Form access |
| `VvUC\MixerUC\ZahtjeviDUC.cs` | ~41-51 | SubModulSet access |
| `VvUC\MixerUC\ZahtjevRNMDUC.cs` | ~250-259 | Form access |
| `Framework\VvFindDialog.cs` | ~100-214 | TheVvTabPage, TheDbConnection fallback |

**Internal to ZXC.cs itself:**

| ZXC.cs Line (approx.) | Code Path | Severity |
|---|---|---|
| 424 | `PrjConnection` getter → `TheVvForm.GetvvDB_prjktDB_name()` | **CRITICAL** — every DAO operation |
| 990 | `VvDB_NameConstructor()` → `TheVvForm.GetvvDB_prefix()` | **CRITICAL** — all DB name construction |
| 601 | `TheSkyDbConnection` getter → `TheVvForm.GetvvDB_prefix()` | High — Sky sync |
| 5432 | `aim_log_file_name()` → `TheVvForm.Get_MyDocumentsLocation_ProjectAndUser_Dependent(false)` | Medium — logging |
| 7173–7189 | `SetStatusText()` / `ClearStatusText()` → `TheVvForm.TStripStatusLabel` | Medium — called from business layer |
| 7361 | `VvSerializedDR_DirectoryName` → `VvForm.GetLocalDirectoryForVvFile()` | Medium — XML DR storage |
| 7449 | `Delete_OldNamingConvention_AutoCreated_VvXmlDR_Directory()` → `VvForm.GetLocalDirectoryForVvFile()` | Low — one-time cleanup |
| 9457–9458 | `TH_Should_ESC_DRW_Log` → `TheVvForm.TheVvUC is IRMDUC` + `TheVvForm.TheVvTabPage.WriteMode` | Medium — TEXTHO logging |

**`PrjConnection` (line 424) is the single most impactful coupling point in the entire codebase.** Phase 1 must extract the project DB name into a standalone `ZXC.PrjktDB_Name` property set once at login.

### 4.13 Status Bar Coupling Is Business-Layer-Wide

`ZXC.SetStatusText()` and `ZXC.ClearStatusText()` (lines 7171–7189) directly access `ZXC.TheVvForm.TStripStatusLabel.Text` with `.Invalidate()` / `.Update()` / `.Refresh()` calls. Additionally, `VvUserControlRecord_Sub.cs` (lines 703–733) has `grid_CellEnter_SetStatusText` / `grid_CellLeave_RestoreStatusText` that access `ZXC.TheVvForm.TStripStatusLabel` and `ZXC.TheVvForm.statusTextBackup`.

These methods are called from business-layer code in multiple files (Mixer, Placa, Ptrane, Person, Htrans, etc.). With detached tabs, status text should route through the calling tab's `IVvDocumentHost` so each window shows its own status.

### 4.14 VvForm.VvSubModul Nested Type Creates Type-Level Dependency

`ZXC.IssueAccessDeniedMessage()` (line 5482) takes a `VvForm.VvSubModul` parameter. This is a **type dependency** on `VvForm`, not just a reference dependency — any caller must import the `VvForm` type to construct the parameter.

Extract `VvSubModul` to a standalone class or move it into `ZXC` as `ZXC.VvSubModul` so business-layer code does not depend on the form type.

### 4.15 DB Connection Variants and Concurrency Risk

Beyond `TheMainDbConnection`, the Second and Third connections each expose **multiple accessor variants** that call `ChangeDatabase()`:

    // Second connection variants (all share theSecondDbConnection):
    TheSecondDbConn_SameDB           → ChangeDatabase(PrjConnection)
    TheSecondDbConn_SameDB_prevYear  → ChangeDatabase(prev year DB)
    TheSecondDbConn_SameDB_OtherYear(int) → ChangeDatabase(specified year DB)
    TheSecondDbConn_OtherDB(string)  → ChangeDatabase(arbitrary DB name)

    // Third connection variants (all share theThirdDbConnection):
    TheThirdDbConn_SameDB            → ChangeDatabase(PrjConnection)
    TheThirdDbConn_OtherDB(string)   → ChangeDatabase(arbitrary DB name)

With detached tabs, concurrent calls to different variants on the same shared connection will race. For example, Tab A calls `TheSecondDbConn_SameDB_prevYear` while Tab B calls `TheSecondDbConn_SameDB` — both mutate the same `theSecondDbConnection` instance.

**Mitigation options:**

1. **Lock-based serialization** (simplest): Wrap each `ChangeDatabase()` + query in a `lock(theSecondDbConnection)` block
2. **Per-DocumentHost connection pool** (safest): Each host gets its own Second/Third connection instances
3. **Connection-per-call pattern** (most scalable): Open/close short-lived connections instead of shared long-lived ones

### 4.16 `*_InProgress` Flag Complete Inventory (~50 Flags)

| Category | Flags | Detach Scope |
|---|---|---|
| **Session-once checks** | `RISK_Cache_Checked`, `RISK_PrNabCij_Checked`, `RISK_BadMSU_Checked`, `RISK_NOTfisk_Checked`, `RISK_TtNum_Slijednost_Checked`, `DUC_UnlinkedTranses_Checked`, `Fak2NalAutomationChecked` | Global (safe) |
| **Long-running ops** | `RenewCache_InProgress`, `RewriteAllDocuments_InProgress`, `RepairMissingFakturEx_InProgress`, `FakturList_To_PDF_InProgress`, `RISK_VvPDFreporter_InProgress` | Global (mutex — must block all windows) |
| **Record-level ops** | `RISK_SaveVvDataRecord_inProgress`, `GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress`, `RISK_FinalRn_inProgress`, `RISK_Edit_RtranoOnly_InProgress`, `DupCopyMenu_inProgress` | **Per-DocumentHost** — two tabs saving simultaneously could conflict |
| **Import/export** | `Restore_FromVvXmlDR_InProgress`, `OffixImport_InProgress`, `LoadPoprat_InProgress`, `CopyOut_InProgress`, `KOPfromFUG_InProgress`, `KOPfromUGAN_InProgress` | Global (file I/O — naturally serialized) |
| **Cache mgmt** | `RISK_DisableCacheTemporarily`, `RISK_DisableAutoFiskTemporarily`, `ShouldForceSifrarRefreshing`, `RISK_NewCache_InProgress` | Global (cache is shared) |
| **Cross-DUC copy** | `RISK_CopyToOtherDUC_inProgress`, `RISK_CopyToMixerDUC_inProgress`, `MIXER_CopyToOtherDUC_inProgress` | **Per-DocumentHost** — could conflict if two windows copy simultaneously |
| **UI state** | `RESET_InitialLayout_InProgress`, `MenuReset_SvDUH_ZAHonly_InProgress`, `PutRiskFilterFieldsInProgress`, `DumpChosenOtsList_OnNalogDUC_InProgress`, `LoadIzvodDLG_isON` | **Per-DocumentHost** — UI flags should be scoped to owning window |
| **RISK field ops** | `RISK_ToggleKnDeviza_InProgress`, `RISK_InitZPCvalues_InProgress`, `RISK_PULXPIZX_Calc_InProgress`, `RISK_CheckPrNabDokCij_inProgress`, `RISK_CheckZPCkol_inProgress`, `RISK_PromjenaNacPlac_inProgress`, `RISK_AutoAddInventuraDiff_inProgress` | **Per-DocumentHost** — operate on active DUC data |
| **Sky sync** | `SENDtoSKY_InProgress`, `RECEIVEfromSKY_InProgress`, `SENDorRECEIVE_SKY_InProgress` | Global (single server endpoint) |
| **Hardware** | `M2PAY_API_Initialized`, `M2PAY_Device_Connected`, `M2PAY_AuthorizationStatus` | Global (physical device singleton) |
| **Misc** | `InitializeApplication_InProgress`, `LoadImportFile_HasErrors`, `LoadCrystalReports_HasErrors`, `VvXmlDR_LastDocumentMissing_AlertRaised`, `ForceXtablePreffix`, `GOST_SOBA_BOR_SetOtherData_InProgress`, `RISK_FiskParagon_InProgress` | Global |

**Summary:** ~20 flags are safe as global, ~15 should become per-`IVvDocumentHost`, ~15 need case-by-case analysis.

### 4.17 ResetAll_GlobalStatusVariables() Is Critically Incomplete

`ResetAll_GlobalStatusVariables()` (lines 179–183) only resets **2 of ~50** `*_InProgress` flags (`Fak2NalAutomationChecked` and `LastUsedProjektTT_IsDiscovered`). It contains a TODO comment acknowledging the gap.

In the multi-window detach scenario, if a tab is detached/reattached and an `*_InProgress` flag was left `true` due to an exception, there is no reset path. Phase 1 should classify each flag, expand the reset method (or create per-host reset) for flags that could become stale.

### 4.18 ShouldSupressRenewCache Is a Computed Compound Flag

    internal static bool ShouldSupressRenewCache
    {
        get { return RenewCache_InProgress
                  || RISK_SaveVvDataRecord_inProgress
                  || GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress; }
    }

This combines one global flag (`RenewCache_InProgress`) with two per-DocumentHost flags. If the per-host flags are scoped to `IVvDocumentHost`, this property's logic must check the **active** host's flags, not global statics. This is a subtle break point.

### 4.19 SENDtoSKY / RECEIVEfromSKY Have Dormant VvForm Coupling

Lines 9590–9632: These `*_InProgress` properties are backed by private fields with **commented-out logging code** in the setters that references `ZXC.TheVvForm.TheVvUC` and `ZXC.TheVvForm.TheVvDataRecord`.

If this diagnostic logging is ever re-enabled, it will create a VvForm coupling in the Sky sync path. The compound property `SENDorRECEIVE_SKY_InProgress` is a read-only gate (`get` returns either `true`). When re-enabling logging, route through `ZXC.ActiveDocumentHost`.

### 4.20 M2PAY Payment Terminal State Is Hardware Singleton

`M2PAY_API_Initialized`, `M2PAY_Device_Connected`, `M2PAY_AuthorizationStatus` represent physical payment terminal state and **must remain global singleton** regardless of detached tabs. Only one window should be able to initiate a payment transaction at a time.

Phase 4 must add a guard in `IVvDocumentHost` that checks whether another host is currently processing a payment before allowing a second one.

### 4.21 VektorSiteEnum-Based Deployment Checks Are Safe

`IsTEXTHOshop`, `IsPCTOGO`, `IsSvDUH`, `IsTETRAGRAM_ANY`, `IsSkyEnvironment`, etc. are all computed from `VvDeploymentSite` (a `VektorSiteEnum` value). These are **immutable per session** — set at login, never changed. Safe to remain global statics with no detach impact.

---

## 5. Known Risks and Gotchas

### 5.1 Business Layer Coupling to UI

`Rtrans.cs` directly references `ZXC.TheVvForm.TheVvDocumentRecordUC` in:

- `Get_S_KC_fromScreen()` — reads sum of KC from FakturDUC grid
- `Get_S_OrgPakKol_fromScreen()` — reads sum of OrgPak quantities
- `CheckZtrColExists()` — validates DUC grid column existence

**Mitigation:** Route these through `ZXC.ActiveDocumentHost` or the UC's own `DocumentHost` property.

### 5.2 Shared DB Connections (NOT Per-Tab)

The application uses **5 global static DB connections**, all shared across the entire process:

| Connection | Purpose | Concurrency Risk |
|---|---|---|
| `TheMainDbConnection` | Primary operations. Routes through `ZXC.PrjConnection` which calls `ChangeDatabase()`. | **Critical** — every DAO operation |
| `TheSecondDbConnection` | Secondary queries. Exposes 4 accessor variants, each calling `ChangeDatabase()` to different DBs. | **Critical** — concurrent variant calls race |
| `TheThirdDbConnection` | Tertiary queries. Exposes 2 accessor variants with `ChangeDatabase()`. | **High** |
| `TheSkyDbConnection` | Sky server sync. Auto-reconnect with `Ping()`. | Medium |
| `TheMbfDbConnection` | MBF server. Obfuscated credentials, `Ping()` auto-reconnect. | Medium |

If two detached tabs call `ChangeDatabase()` concurrently on any shared connection, they will interfere. Additionally, `SetSifrarAndAutocomplete<T>()` calls `ChangeDatabase()` during sifrar loading, adding another concurrency surface.

**Mitigation options:**

1. Lock-based serialization — wrap `ChangeDatabase()` + query in `lock()` blocks
2. Per-DocumentHost connection pool — each host gets its own connection instances
3. Connection-per-call pattern — open/close short-lived connections

### 5.3 VvUserControl.TheVvTabPage Parent.Parent Navigation

`VvUserControl.TheVvTabPage` navigates via `(VvTabPage)this.Parent.Parent`. When a UC is reparented to `VvFloatingForm`, the Parent chain changes and this cast **will throw an InvalidCastException**.

**Mitigation (Phase 1, prerequisite):** Add a settable `TheVvTabPage` property that defaults to `Parent.Parent` but can be explicitly set during reparent.

### 5.4 Crystal Reports Integration

`ts_Report` toolbar is tightly integrated with Crystal Reports viewer. Report generation uses a `BackgroundWorker` that lives on `VvTabPage`. Detached report tabs need:

- Their own `BackgroundWorker` instance
- Their own Crystal Reports viewer + toolbar
- Independent `ReportMode` state

### 5.5 WriteMode — Per-Tab vs Global

`WriteMode` is already per-tab on `VvTabPage.WriteMode`, which is correct for detach. However, some code paths may reach `WriteMode` through `ZXC.TheVvForm.TheVvTabPage.WriteMode` — which always resolves to the **main form's active tab**, not the caller's tab. These paths must be audited in Phase 1.

### 5.6 VvEnvironmentDescriptor

Currently persists one form's state using `List<VvToolStripItem_State>` with merge logic on load. Must adapt to:

- Save main form state (as before)
- Optionally remember which tabs were detached and their window positions
- Each detached form's toolbar layout is transient (not persisted)

### 5.7 Keyboard Shortcuts

Current shortcuts (Ctrl+N, Ctrl+S, F2, F5, Ctrl+F, etc.) are defined per `VvSubMenu.shortKeys`. In multi-window scenario, shortcuts must only fire on the **focused** window's `BarManager`.

### 5.8 Rtrans CalcTransResults Dependency on Active DUC

`INIT_Memset0Rtrans_GetZtr()` calls `Get_S_KC_fromScreen()` which assumes the active screen DUC is the correct `FakturDUC`. With detached tabs, two `FakturDUC` instances could exist simultaneously. The calculation must reference **its own** DUC, not the globally active one.

### 5.9 ZXC `*_InProgress` Flag Concurrency

ZXC contains ~50 static boolean `*_InProgress` flags acting as ad-hoc mutex guards. With two independent windows, a user could trigger conflicting operations:

- Tab A starts cache renewal → `RenewCache_InProgress = true`
- Tab B attempts a save → blocked by the flag, but the flag was set by Tab A

~15 flags should become per-`IVvDocumentHost`, ~20 are safe as global, ~15 need case-by-case analysis. See §4.16 for the complete inventory and classification.

### 5.10 Global Data Records as Temp Workspace

`ZXC.NalogRec`, `ZXC.FakturRec`, `ZXC.MixerRec`, `ZXC.AmortRec`, and `ZXC.DevTecRec` are used as temporary workspace by business logic. Two concurrent operations could overwrite each other's temp data.

**Mitigation:** Move these to per-operation local variables where possible, or scope them per `IVvDocumentHost`.

### 5.11 VvHamper Layout System Complexity

`VvHamper` is not just a Crownwood style helper — it's used pervasively for programmatic control layout using `ZXC.Redak` / `ZXC.Kolona` enums and the QUN (quantum unit) measurement system (`ZXC.QUN`, `ZXC.Qun2`, `ZXC.Q3un`, etc.). Grid row/column sizing in every `VvDocumentRecordUC` also depends on QUN values. Removing VvHamper in Phase 5 requires either:

- Migrating to DevExpress `LayoutControl` (significant rework)
- Keeping VvHamper as a standalone layout utility (decoupled from Crownwood)

---

## 6. Migration Phases

### Phase 0: Preparation

- [ ] Analyze and document current form, tab, and toolbar behaviors
- [ ] Identify all `SetVvMenuEnabledOrDisabled_*()` usages and implications
- [ ] Map current `DotNetMagic` components to DevExpress equivalents
- [ ] Catalog all `ZXC.*_InProgress` flags and assess concurrency impact
- [ ] Catalog all `ZXC.TheVvForm.*` references in business layer code
- [ ] Train team on DevExpress concepts: `XtraForm`, `DocumentManager`, `BarManager`, Skins
- [ ] Set up basic DevExpress form with menu and toolbar

### Phase 1: Introduce Abstractions (No Visual Change)

**Interface & Factory:**

- [ ] Create `IVvDocumentHost` interface (including `SetStatusText()` / `ClearStatusText()`)
- [ ] Create `VvToolbarFactory` — extract from `InitializeMainMenu()`, `InitalizeToolStrip_Record()`, `InitializeToolStrip_Report()`
- [ ] Extract `VvToolbarFactory.ApplyWriteMode()` from `SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB()`
- [ ] Preserve product-type button filtering in factory (Surger, Remonster, etc.)
- [ ] Create `VvCommands` static class — extract menu/toolbar event handlers
- [ ] VvForm implements `IVvDocumentHost`

**ZXC Infrastructure Decoupling:**

- [ ] Extract `PrjConnection` → `ZXC.PrjktDB_Name` standalone property, set once at login — eliminate `TheVvForm.GetvvDB_prjktDB_name()` dependency (**CRITICAL**)
- [ ] Extract `VvDB_NameConstructor()` prefix → `ZXC.VvDB_Prefix` standalone property, set once at login — eliminate `TheVvForm.GetvvDB_prefix()` dependency (**CRITICAL**)
- [ ] Add `ZXC.ActiveDocumentHost` tracking + `RegisterDocumentHost` / `UnregisterDocumentHost`
- [ ] Extract `VvForm.VvSubModul` nested type to standalone class or into `ZXC`

**VvUserControl Decoupling:**

- [ ] Add `VvUserControl.DocumentHost` property at root UC level (defaults to `ZXC.TheVvForm`)
- [ ] Add settable `VvUserControl.TheVvTabPage` property to replace `this.Parent.Parent` navigation
- [ ] Fix `VvUserControl.TheDbConnection` fallback to route through `DocumentHost`

**Toolbar & Status Bar Refactoring:**

- [ ] Refactor all `SetVvMenuEnabledOrDisabled_*()` methods to use `IVvDocumentHost`
- [ ] Refactor `ZXC.SetStatusText()` and `VvUserControlRecord_Sub.grid_CellEnter_SetStatusText` to route through `DocumentHost`

**Business Layer Decoupling:**

- [ ] Decouple `Rtrans.Get_S_KC_fromScreen()` and `CheckZtrColExists()` from `ZXC.TheVvForm`

**Audit & Classification:**

- [ ] Audit `ZXC.TheVvForm` references in all files (PTG_DUC, Fin_Dlg_UC, Fin_Dlg_UC_Q, PlacaDUC_Q, SmdDUC, ZahtjeviDUC, ZahtjevRNMDUC, VvFindDialog, and internal ZXC.cs paths)
- [ ] Classify all ~50 `*_InProgress` flags — global vs per-DocumentHost (see §4.16)
- [ ] Audit `ShouldSupressRenewCache` compound flag for per-host correctness
- [ ] Audit `SetSifrarAndAutocomplete<T>()` `ChangeDatabase()` calls for concurrency
- [ ] Address Second/Third DB connection concurrency with lock or per-host pooling
- [ ] Expand `ResetAll_GlobalStatusVariables()` to cover per-host flags on detach/reattach
- [ ] Guard M2PAY payment operations to single-window-at-a-time
- [ ] Ensure Sky `*_InProgress` property setters do not re-enable dormant `TheVvForm` logging

**Documentation:**

- [ ] Preserve toolbar state merge logic compatibility for `VvEnvironmentDescriptor`
- [ ] Document `thisIsFirstAppereance` behavior for detach/reattach
- [ ] Examine `SubModulWanted_Click` as tab creation entry point

**Risk: Low-Medium** — pure refactoring, no visual changes, but scope is substantial due to ZXC infrastructure decoupling and flag classification.

### Phase 2: Replace Crownwood TabControl

- [ ] Add DevExpress NuGet packages
- [ ] Replace `Crownwood.TabControl` with `DocumentManager` + `TabbedView`
- [ ] Adapt `VvTabPage` to wrap `BaseDocument` (or replace entirely)
- [ ] Migrate `VvTabPage_VisibleChanged` → `TabbedView.DocumentActivated`
- [ ] Migrate `VvTabPage_Validating` logic (archive mode block)
- [ ] Migrate `BackgroundWorker` report generation (lives on `VvTabPage`)
- [ ] Test each module's tab lifecycle

**Risk: Medium** — visible change, needs per-module testing.

### Phase 3: Replace Menus and Toolbars

- [ ] Replace `MenuStrip` with `BarManager` + `Bar` (MainMenu)
- [ ] Replace `ts_Record`, `ts_Modul`, `ts_Report` with `BarManager` `Bar` objects
- [ ] Replace `ats_SubModulSet[][]` with per-submodule `Bar` created via `VvToolbarFactory`
- [ ] Migrate `VvEnvironmentDescriptor` to save/load `BarManager` layout (custom merge step needed — see §4.4)

**Risk: Medium** — button-by-button mapping, shortcuts must be preserved. Mitigated by the data-driven `VvMenu[]`/`VvSubMenu[]` struct definition pattern (see §4.1) which maps naturally to DevExpress `BarItem` creation.

### Phase 4: Implement Tab Detach

- [ ] Create `VvFloatingForm : XtraForm, IVvDocumentHost`
- [ ] Handle `TabbedView.DocumentFloating` → create VvFloatingForm
- [ ] Implement UC reparenting with explicit `TheVvTabPage` assignment (avoids Parent.Parent breakage)
- [ ] Handle `VvFloatingForm.FormClosing` → return UC to main form
- [ ] Test WriteMode independence between windows
- [ ] Test DB connection sharing between windows (concurrent `ChangeDatabase()` scenarios)
- [ ] Test `*_InProgress` flag isolation between concurrent operations
- [ ] Guard M2PAY payment terminal to single-window operation

**Risk: Medium** — new feature, but well-isolated by Phase 1 abstractions.

### Phase 5: Remove Crownwood Entirely

- [ ] Replace `DotNetMagicForm` base with `XtraForm`
- [ ] Replace `Crownwood.TreeControl` with `NavBarControl` or `TreeList`
- [ ] Replace `Crownwood.VisualStyle` with DevExpress Skins
- [ ] Remove `VvHamper` Crownwood style helpers — **Note:** VvHamper is used pervasively for field layout (row/column positioning via `ZXC.Redak`/`ZXC.Kolona` enums and QUN units) and grid sizing. Replacement requires either migrating to DevExpress `LayoutControl` or decoupling VvHamper into a standalone layout utility.
- [ ] Remove Crownwood DLL references

**Risk: Medium** — VvHamper removal is more complex than simple style removal due to QUN-based grid sizing in every `VvDocumentRecordUC` subclass.

---

## 7. Effort Estimates

| Phase     | Scope              | Est. Days  | Key Drivers                                                         |
|-----------|--------------------|:----------:|---------------------------------------------------------------------|
| 0         | Preparation        |        2-3 | Discovery, team training, DevExpress setup                          |
| 1         | Abstractions       |    12-17   | PrjConnection/VvDB decoupling, ~50 flag classification, 9+ file audit |
| 2         | Tab Control        |      3-5   | Per-module lifecycle testing                                        |
| 3         | Menus and Toolbars |      4-7   | Button mapping (mitigated by struct-driven definition)              |
| 4         | Tab Detach         |      6-9   | DB concurrency testing, M2PAY guard, flag isolation                 |
| 5         | Crownwood Removal  |      4-6   | VvHamper/QUN grid sizing in every DUC subclass                      |
| **Total** |                    | **31-47**  |                                                                     |

Phase 1 is the **most important** — it decouples everything and makes Phases 2–5 safe incremental steps.