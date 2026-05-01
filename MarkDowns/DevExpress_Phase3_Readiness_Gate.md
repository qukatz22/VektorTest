# DevExpress Migration — Phase 3 Readiness Gate (P3-0)

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
