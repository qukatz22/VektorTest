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
| `VvTabPage` | `Framework\VvTabPage.cs` | Extends `Crownwood.DotNetMagic.Controls.TabPage`. Manages UC lifecycle, handles `VisibleChanged` to switch toolstrips. |
| `VvUserControl` | `Framework\VvUserControl*.cs` | Base for all content UCs (FakturDUC, NalogDUC, PlacaDUC, etc.). The actual working content. |
| `ZXC` | `Framework\ZXC.cs` | Global static state. `TheVvForm` (singleton), `CurrentForm`, `WriteMode`, project flags, DB info. |
| `VvEnvironmentDescriptor` | `UtilsEtc\VvEnvironmentDescriptor.cs` | XML-serialized form state (toolbar layout, position, colors, font). |

### 1.3 Tab-Toolbar Coupling (The Core Problem)

Current flow when a tab becomes active:

    VvTabPage_VisibleChanged(visible = true)
      │
      ├── GetTSB_EnabledStateSnapshot() — save outgoing tab's toolbar button states
      ├── ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet() — hide ALL ats_SubModulSet[][] except current
      ├── SetVvSubModulSetMenuEnabledOrDisabled_RegardingWriteMode() — enable/disable per WriteMode
      ├── Show/hide ts_Report vs ts_Record based on TabPageKind
      └── PutTSB_EnabledStateSnapshot() — restore incoming tab's saved toolbar button states

**Critical coupling points:**

- `VvTabPage.TheVvForm` — every tab references the singleton VvForm
- `ats_SubModulSet[i][j]` — 2D ToolStrip array lives on VvForm, tabs just select which one is visible
- `SetVvMenuEnabledOrDisabled_*()` — directly manipulates `ts_Record.Items["NEW"]`, etc. by string name
- `ZXC.TheVvForm` — used throughout business logic (e.g., `Rtrans.Get_S_KC_fromScreen()` casts `ZXC.TheVvForm.TheVvDocumentRecordUC`)

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

Special cases: TEXTHOshop disables OPN/DEL, PCTOGO disables NEW/DUP on certain PTG DUCs, SvDUH_ZAHonly restrictions on IZD/ZAH.

### 1.6 Business Layer UI Coupling (Rtrans example)

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

### 3.1 The IVvDocumentHost Abstraction (Phase 1 — Critical Enabler)

    ┌──────────────────────────┐
    │    IVvDocumentHost       │  ← NEW interface
    │                          │
    │  + TheBarManager         │
    │  + Btn_NEW, Btn_OPN…     │
    │  + Bar_Record            │
    │  + Bar_SubModul          │
    │  + Bar_Report            │
    │  + SetWriteMode(wm)     │
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

### 3.2 VvToolbarFactory (Phase 1)

Extract toolbar creation from VvForm into a static factory:

    static class VvToolbarFactory
    {
        Bar CreateBar_Record(BarManager mgr, IVvDocumentHost host);
        Bar CreateBar_Report(BarManager mgr, IVvDocumentHost host);
        Bar CreateBar_SubModul(BarManager mgr, VvSubModulEnum subModul);
        Bar CreateMenuBar(BarManager mgr, bool isDetached);
        void ApplyWriteMode(IVvDocumentHost host, WriteMode wm);
    }

### 3.3 VvUserControl Decoupling

Current (tight coupling):

    // In Rtrans.cs (business layer!):
    private decimal Get_S_KC_fromScreen()
    {
        return ((FakturDUC)ZXC.TheVvForm.TheVvDocumentRecordUC).Get_S_KC_fromScreen();
    }

Target (decoupled):

    // VvUserControl gains:
    public IVvDocumentHost DocumentHost { get; set; }

    // Business layer routes through DocumentHost instead of ZXC.TheVvForm

### 3.4 ZXC Singleton Adaptation

    // ZXC (current)                 ZXC (target)
    // ─────────────                 ────────────
    // TheVvForm (singleton)    →    TheVvForm (still singleton, main form)
    // CurrentForm              →    ActiveDocumentHost : IVvDocumentHost
    //                               RegisterDocumentHost(host)
    //                               UnregisterDocumentHost(host)
    //                               AllDocumentHosts : List of IVvDocumentHost

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
      ├── ZXC.RegisterDocumentHost(this)
      └── this.Show()
            │
            ▼
    User closes VvFloatingForm
      ├── Remove UC from Controls
      ├── Reattach UC to original VvTabPage
      ├── ZXC.UnregisterDocumentHost(this)
      └── Dispose

### 3.6 Phase 0 — Preparation

- [ ] Analyze and document current form, tab, and toolbar behaviors
- [ ] Identify all `SetVvMenuEnabledOrDisabled_*()` usages and implications
- [ ] Map current `DotNetMagic` components to DevExpress equivalents
- [ ] Train team on DevExpress concepts: `XtraForm`, `DocumentManager`, `BarManager`, Skins
- [ ] Set up basic DevExpress form with menu and toolbar

---

## 4. Migration Phases

### Phase 1: Introduce Abstractions (No Visual Change)

- [ ] Create `IVvDocumentHost` interface
- [ ] Create `VvToolbarFactory` — extract from `InitalizeToolStrip_Record()`, `InitializeToolStrip_Report()`
- [ ] Create `VvCommands` static class — extract event handlers (NewRecord_OnClick, etc.)
- [ ] VvForm implements `IVvDocumentHost`
- [ ] Add `VvUserControl.DocumentHost` property (defaults to `ZXC.TheVvForm`)
- [ ] Add `ZXC.ActiveDocumentHost` tracking
- [ ] Refactor `SetVvMenuEnabledOrDisabled_*()` to go through `IVvDocumentHost`
- [ ] Decouple `Rtrans.Get_S_KC_fromScreen()` and `CheckZtrColExists()` from `ZXC.TheVvForm`
- [ ] **Risk: Low** — pure refactoring, no visual changes

### Phase 2: Replace Crownwood TabControl

- [ ] Add DevExpress NuGet packages
- [ ] Replace `Crownwood.TabControl` with `DocumentManager` + `TabbedView`
- [ ] Adapt `VvTabPage` to wrap `BaseDocument` (or replace entirely)
- [ ] Migrate `VvTabPage_VisibleChanged` → `TabbedView.DocumentActivated`
- [ ] Test each module's tab lifecycle
- [ ] **Risk: Medium** — visible change, needs per-module testing

### Phase 3: Replace Menus and Toolbars

- [ ] Replace `MenuStrip` with `BarManager` + `Bar` (MainMenu)
- [ ] Replace `ts_Record`, `ts_Modul`, `ts_Report` with `BarManager` `Bar` objects
- [ ] Replace `ats_SubModulSet[][]` with per-submodule `Bar` created via `VvToolbarFactory`
- [ ] Migrate `VvEnvironmentDescriptor` to save/load `BarManager` layout
- [ ] **Risk: Medium** — button-by-button mapping, shortcuts must be preserved

### Phase 4: Implement Tab Detach

- [ ] Create `VvFloatingForm : XtraForm, IVvDocumentHost`
- [ ] Handle `TabbedView.DocumentFloating` → create VvFloatingForm
- [ ] Implement UC reparenting (detach and reattach)
- [ ] Handle `VvFloatingForm.FormClosing` → return UC to main form
- [ ] Test WriteMode independence between windows
- [ ] Test DB connection sharing between windows
- [ ] **Risk: Medium** — new feature, but well-isolated by Phase 1 abstractions

### Phase 5: Remove Crownwood Entirely

- [ ] Replace `DotNetMagicForm` base with `XtraForm`
- [ ] Replace `Crownwood.TreeControl` with `NavBarControl` or `TreeList`
- [ ] Replace `Crownwood.VisualStyle` with DevExpress Skins
- [ ] Remove `VvHamper` Crownwood style helpers
- [ ] Remove Crownwood DLL references
- [ ] **Risk: Low** — cleanup

---

## 5. Known Risks and Gotchas

### 5.1 Business Layer Coupling to UI

`Rtrans.cs` directly references `ZXC.TheVvForm.TheVvDocumentRecordUC` in:

- `Get_S_KC_fromScreen()` — reads sum of KC from FakturDUC grid
- `Get_S_OrgPakKol_fromScreen()` — reads sum of OrgPak quantities
- `CheckZtrColExists()` — validates DUC grid column existence

**Mitigation:** Route these through `ZXC.ActiveDocumentHost` or the UC's own `DocumentHost` property.

### 5.2 Single DB Connection per Tab

Each `VvTabPage` has `TheDbConnection`. Detached windows share the same MySQL connection pool but each tab's operations must remain isolated.

### 5.3 Crystal Reports Integration

`ts_Report` toolbar is tightly integrated with Crystal Reports viewer. Detached report tabs need their own viewer instance + toolbar.

### 5.4 WriteMode Conflicts

If main form has a tab in Edit mode and a detached tab also enters Edit mode, both have independent WriteMode states. This is the **desired behavior** but must be verified against `ZXC.WriteMode` global usage.

### 5.5 VvEnvironmentDescriptor

Currently persists one form's state. Must adapt to:

- Save main form state (as before)
- Optionally remember which tabs were detached and their window positions
- Each detached form's toolbar layout is transient (not persisted)

### 5.6 Keyboard Shortcuts

Current shortcuts (Ctrl+N, Ctrl+S, F2, F5, Ctrl+F, etc.) are defined per `VvSubMenu.shortKeys`. In multi-window scenario, shortcuts must only fire on the **focused** window's `BarManager`.

### 5.7 Rtrans CalcTransResults Dependency on Active DUC

`INIT_Memset0Rtrans_GetZtr()` calls `Get_S_KC_fromScreen()` which assumes the active screen DUC is the correct `FakturDUC`. With detached tabs, two `FakturDUC` instances could exist simultaneously. The calculation must reference **its own** DUC, not the globally active one.

---

## 6. Estimated Effort

| Phase     | Scope              | Est. Days  | Dependencies                |
|-----------|--------------------|-----------:|-----------------------------|
| 1         | Abstractions       |        5-8 | None                        |
| 2         | Tab Control        |        3-5 | Phase 1, DevExpress license |
| 3         | Menus and Toolbars |        5-8 | Phase 1, DevExpress license |
| 4         | Tab Detach         |        3-5 | Phases 1+2+3                |
| 5         | Crownwood Removal  |        2-3 | Phase 2+3                   |
| **Total** |                    | **18-29**  |                             |

Phase 1 is the **most important** — it decouples everything and makes phases 2-5 safe incremental steps.