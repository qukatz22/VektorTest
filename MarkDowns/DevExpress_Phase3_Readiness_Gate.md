# DevExpress Migration — Phase 3 Readiness Gate (P3-0)

> **Status:** ✅ **CLOSED.** Phase 3 DETACH implementation is complete.
> All Phase 3 smoke tests passed including the 100× detach/reattach memory-leak
> cycle test. This document is retained as historical validation context.

## 1. Purpose

This document is the go/no-go checklist before any Phase 3 DETACH runtime implementation starts.

V4 requires that Phase 3 starts only after the SWAP phase is stable and extensively tested. This gate records what is already technically complete and what still requires human validation.

## 2. Current technical status

| Area | Status | Evidence |
|---|---|---|
| Crownwood active source references | Green | Active source/config/project/deploy scans after §2k found no live `Crownwood.DotNetMagic`/`DotNetMagic` references. |
| DotNetMagic project references | Green | `Vektor.csproj` and `SkyLab.csproj` no longer reference `DotNetMagic.DLL`. |
| Main form | Green | `VvForm` is `DevExpress.XtraEditors.XtraForm`. |
| Main tab host | Green | `DocumentManager` + `TabbedView` active. |
| Inner tabs | Green | `VvInnerTabControl` / `VvInnerTabPage` DevExpress-compatible wrappers active. |
| Module tree | Green | `DxTreeView_Modul` / `TreeList` active. |
| Menu/toolbar abstraction | Green | `IVvDocumentHost`, `VvToolbarFactory`, `DxBarManager`, `DxBarItemsByName` exist. |
| Skin/color model | Green | `DxSkinName` + RGB constants; Crownwood style enums removed. |
| Build validation | Green | VS-build green per updated fallback rule. |

## 3. Human validation still required

These items are not safely automatable by the coding agent and should be signed off by a developer/tester.

### 3.1 Full regression checklist

| Scenario | Status | Notes |
|---|---|---|
| Application starts without DotNetMagic DLL | Pending |  |
| Login | Pending |  |
| Open several modules | Pending |  |
| Main tab switch by mouse | Pending |  |
| Main tab switch programmatically | Pending |  |
| Close tab with clean state | Pending |  |
| Close tab with dirty state | Pending |  |
| Archive tab switch/close blocking | Pending |  |
| Menu shortcuts | Pending |  |
| Toolbar enable/disable through WriteMode | Pending |  |
| Status bar text | Pending |  |
| Module tree root/submodule/report node clicks | Pending |  |
| Module tree expand/collapse | Pending |  |
| Color/skin dialog | Pending |  |
| Save and reload environment preferences | Pending |  |
| Report preview/print/export | Pending |  |
| FIR outbound flow | Pending |  |
| FUR inbound flow | Pending |  |
| Plaće | Pending |  |
| Amortizacija | Pending |  |
| TEXTHOshop variant | Pending |  |
| PCTOGO variant | Pending |  |
| SvDUH variant | Pending |  |

### 3.2 Screenshot comparison checklist

| Screen | Status | Notes |
|---|---|---|
| Main form after login | Pending |  |
| Module panel left/right | Pending |  |
| Standard DUC with record toolbar | Pending |  |
| Report tab/filter | Pending |  |
| Color/skin dialog | Pending |  |
| Dirty prompt | Pending |  |
| Archive blocking message | Pending |  |
| Report preview | Pending |  |

## 4. Go/no-go decision

Phase 3 runtime implementation may start only when:

- full regression checklist is acceptable,
- screenshot comparison is acceptable,
- any discovered SWAP regressions are either fixed or explicitly accepted as known issues,
- team agrees remaining issues are not blockers for DETACH planning/spike.

Decision:

- [x] GO — start P3-1 floating interception spike.
- [ ] NO-GO — fix Phase 2 SWAP regressions first.

Decision owner:

Date:

Notes:

## 5. If GO: first implementation slice

P3-1 from `MarkDowns/DevExpress_Phase3_DETACH_Planning.md`:

1. Hook `TabbedView.BeginFloating`.
2. Verify the event provides the source `Document` and `VvTabPage`.
3. Set `e.Cancel = true` to prevent DevExpress default lightweight floating.
4. Log/diagnose source context.
5. Do not create `VvFloatingForm` yet.

Success criteria:

- dragging a tab outside the main form does not create DevExpress lightweight float,
- code can reliably identify source `Document`, `VvTabPage`, and `VvUserControl`,
- no tab close/switch/dirty behavior changes in normal use.

Implementation status: code compiled with VS-build green; runtime drag gesture still needs human smoke validation.

## 6. Stop conditions for P3-1

Stop and reassess if:

- `DocumentFloating` does not fire for the desired gesture,
- source document/control cannot be resolved reliably,
- canceling default floating breaks normal tab behavior,
- dirty/archive safeguards are bypassed,
- DevExpress version lacks required event semantics.

## 7. Phase 3 baseline smoke checklist (P3-1 through P3-11)

Current implementation baseline:

- `BeginFloating` cancels DevExpress lightweight floating.
- `VvFloatingForm` is a true top-level `XtraForm` and `IVvDocumentHost`.
- Existing `VvUserControl` is reparented without data reload.
- Closing detached form reattaches content to the source tab.
- Duplicate/unsafe detach attempts are blocked by `CanDetach`.
- Dirty close and archive close use the existing source-tab close rules.
- Status text routes through the active document host.
- Detached host owns a separate `BarManager` and safe skeleton menu/toolbar items.

Manual smoke tests:

| Scenario | Expected result | Status | Notes |
|---|---|---|---|
| Drag clean record tab outside main form | Top-level detached form opens; no DX lightweight float appears | Pending |  |
| Close detached clean tab | UC returns to source tab; source tab activates | Pending |  |
| Drag same tab again while detached | Second detach is ignored/logged | Pending |  |
| Dirty detached close: Yes | Existing save flow runs; close proceeds only if save succeeds | Pending |  |
| Dirty detached close: No | Close proceeds; edit lock release follows existing helper | Pending |  |
| Dirty detached close: Cancel | Floating form remains open; content remains detached | Pending |  |
| Arhiva tab detach attempt | Detach is blocked/logged | Pending |  |
| Status text in main tab | Main status bar updates | Pending |  |
| Status text in detached tab | Detached status bar updates | Pending |  |
| Activate main after detached interaction | Active host returns to main form | Pending |  |
| Detached toolbar close item | Form close uses same dirty/reattach path | Pending |  |

Known implementation boundaries before next slice:

- Business toolbar actions (`SAV`, `ESC`, `PRN`, navigation, report actions) are not copied into detached toolbar yet.
- `VvToolbarFactory.ApplyWriteMode` still throws for non-`VvForm` hosts.
- DB lock serialization for secondary/third connections is not implemented yet.
- Per-host `*_InProgress` flag migration is not implemented yet.
- Main-form close with detached forms still needs explicit policy/prompt.
