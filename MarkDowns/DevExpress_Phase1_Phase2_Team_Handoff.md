# DevExpress Migration — Phase 1/2 Team Handoff

## 1. Svrha dokumenta

Ovaj dokument je praktični handoff za programere koji nastavljaju rad na Vektoru nakon DevExpress migracije Faze 1 i Faze 2.

Ne zamjenjuje `MarkDowns/DevExpress_Migration_V4.md`. V4 ostaje autoritativni strateški plan. Ovdje je sažetak za svakodnevni rad: što se promijenilo, koji su novi API-ji, što se više ne smije koristiti, gdje očekivati regresije i što testirati prije releasea.

## 2. Executive summary

- Aktivni UI kod je prebačen s Crownwood DotNetMagic na DevExpress/WinForms stack.
- Aktivni source/config/project/deploy scan nakon Faze 2 §2k ne sadrži live `DotNetMagic`/`Crownwood.DotNetMagic` reference.
- `DotNetMagic` reference uklonjene su iz aktivnih project fileova (`Vektor.csproj`, `SkyLab.csproj`).
- Glavna forma je DevExpress `XtraForm`.
- Glavni tab host je DevExpress `DocumentManager` + `TabbedView`.
- Unutarnji tabovi koriste DevExpress-compatible wrapper (`VvInnerTabControl`, `VvInnerTabPage`).
- Modul tree koristi DevExpress `TreeList` (`DxTreeView_Modul`).
- Menu/toolstrip layer ima DevExpress `BarManager` / `BarItem` infrastrukturu uz compatibility layer.
- Skin/color sustav koristi DevExpress skin name (`DxSkinName`) i RGB konstante umjesto Crownwood style enum-a.
- Floating detach NIJE dio Faze 2. To je Faza 3 i smije početi tek nakon runtime stabilizacije SWAP faze.

## 3. Granica Faze 1, Faze 2 i Faze 3

V4 §0 fiksira dvije makro-faze:

| Faza | Što radi | Što ne radi |
|---|---|---|
| Faza 1 | Decoupling infrastrukture i priprema host apstrakcija. | Ne mijenja bitno ponašanje UI-a. Ne uvodi detach. |
| Faza 2 | SWAP: Crownwood -> DevExpress/WinForms uz isto ponašanje. | Ne uvodi nove featuree. Ne uvodi floating detach. |
| Faza 3 | Floating detach: tab može postati samostalna top-level forma. | Ne smije se miješati s bugfixevima SWAP faze. |

Praktično pravilo: ako promjena dodaje novo ponašanje za floating/detach, nije Faza 2 bugfix nego Faza 3 rad.

## 4. Glavne promjene po slojevima

### 4.1 Startup i glavna forma

- `VvForm` je sada `DevExpress.XtraEditors.XtraForm`.
- DevExpress skin engine se pali prije kreiranja forme:
  - `SkinManager.EnableFormSkins();`
  - `UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");`
- Default skin se kasnije može pregaziti iz `VvEnvironmentDescriptor` preko `DxSkinName`.

Bitno: više nema `Form.Style` / Crownwood `VisualStyle` propertyja.

### 4.2 Glavni tab host

Stari model:

| Staro | Novo |
|---|---|
| Crownwood `TabControl` | DevExpress `DocumentManager` + `TabbedView` |
| Crownwood `TabPage` host | `Document.Control = VvTabPage` |
| `SelectedTab` / `SelectedIndex` | `ActiveDocument`, `SelectedTabPage`/shimovi |
| `ClosePressed` + `Validating` | `DocumentClosing` |
| `VisibleChanged` tab lifecycle | `DocumentActivated` / `DocumentDeactivated` |

Ključna mjesta:

- `zVvForm/TabControl_TabPages.cs`
  - `InitializeWorkTabControl()` kreira `DocumentManager` i `TabbedView`.
  - `DocumentActivated` delegira u `VvTabPage.OnActivated()`.
  - `DocumentDeactivated` delegira u `VvTabPage.OnDeactivated()`.
  - `DocumentClosing` čuva dirty prompt i arhiva blokade.
  - `TabMouseActivating` sprječava mouse switch iz arhiva taba.
- `Framework/VvTabPage.cs`
  - `VvTabPage` se dodaje u `TabbedView` preko `AddDocument(this)`.
  - `Selected` shim aktivira DevExpress document preko `TabbedView.Controller.Activate(...)`.
  - `Title`/`Image`/selected compatibility ostaje zbog postojećih call-siteova.

### 4.3 Unutarnji tabovi

Unutarnji tabovi u DUC-ovima prebačeni su na DevExpress-compatible wrapper:

| Staro | Novo |
|---|---|
| Crownwood inner `TabControl` | `VvInnerTabControl` / `XtraTabControl` wrapper |
| Crownwood inner `TabPage` | `VvInnerTabPage` / `XtraTabPage` wrapper |
| `SelectedTab` | `SelectedTabPage` |
| `SelectedIndexChanged` | `SelectedPageChanged` gdje je primjenjivo |

Kod koji radi s unutarnjim tabovima treba koristiti postojeće wrapper propertyje umjesto direktnog DevExpress ili starog Crownwood API-ja.

### 4.4 Menu i toolbar layer

Faza 1 uvela je host apstrakciju, Faza 2 dodala DevExpress bar infrastrukturu.

Ključni tipovi:

- `IVvDocumentHost`
  - host abstraction za glavnu formu i buduću detached formu.
  - izlaže WinForms compatibility members i DevExpress members (`DxBarManager`, `DxBar_Record`, `DxBar_Report`, `DxMenuBar`, `DxBarItemsByName`).
- `VvToolbarFactory`
  - službeni ulaz za nove menu/toolbar call-siteove.
  - gradi `Bar`, `BarButtonItem`, `BarSubItem`, `BarEditItem` iz postojećih `VvMenu`/`VvSubMenu` struktura.
  - `ApplyWriteMode(host, writeMode)` je službeni entry point za enable/disable logiku.

Pravila:

- Novi kod ne smije direktno ovisiti o `ZXC.TheVvForm` ako je potreban document host; koristi `IVvDocumentHost` gdje postoji put.
- Novi menu/toolbar kod ne smije zaobilaziti `VvToolbarFactory`.
- `DxBarItemsByName` je lookup po legacy item name-u za bridging i enable/disable logiku.

### 4.5 Modul tree

Stari Crownwood `TreeControl` je uklonjen iz aktivnog patha.

| Staro | Novo |
|---|---|
| Crownwood `TreeControl` | DevExpress `TreeList` (`DxTreeView_Modul`) |
| Crownwood `Node` | `DevExpress.XtraTreeList.Nodes.TreeListNode` |
| `AfterSelect` | `FocusedNodeChanged` |
| node image | `ImageIndex` / `SelectImageIndex` |
| expand event | `AfterExpand` |

Ključna mjesta:

- `zVvForm/Moduls_CommandPanel.cs`
  - `InitializeDxTreeViewModul()` kreira i konfigurira `TreeList`.
  - `AppendDxTreeNode(...)` centralno kreira node i puni `Tag`, `ImageIndex`, `SelectImageIndex`.
  - `DxTreeView_Modul_FocusedNodeChanged(...)` poziva `HandleTreeView_ModulNodeTag(...)`.
  - `DxTreeView_Modul_AfterExpand(...)` poziva `HandleTreeView_ModulReportGroupExpanded(...)`.

Praktično: ako se tree selection ponaša čudno, prvo gledati `Tag` vrijednosti i `FocusedNodeChanged` event flow.

### 4.6 Skin/color sustav

Crownwood style enum-i su uklonjeni iz aktivnog modela.

| Staro | Novo |
|---|---|
| `VisualStyle` | `DxSkinName` string |
| `OfficeStyle` | uklonjeno iz aktivnog UI-ja |
| `MediaPlayerStyle` | uklonjeno iz aktivnog UI-ja |
| `TreeControlStyles` | uklonjeno iz aktivnog UI-ja |
| `Office2007ColorTable` / `MediaPlayerColorTable` | RGB konstante |

Ključna mjesta:

- `UtilsEtc/VvColorsAndStyles.cs`
  - više nema Crownwood enum fieldova.
  - `DxSkinName` je aktivni skin preference.
- `UtilsEtc/VvColors.cs`
  - color picker koristi WinForms/DevExpress kontrole i lokalne RGB tablice.
- `UtilsEtc/VvColorsStylsDlg.cs`
  - prikazuje DevExpress skin izbor i color edit UI.
  - ne prikazuje Crownwood style enum-e.
- `zVvForm/Initializations_Settings.cs`
  - `ApplyDxSkin(...)`
  - `GetDxSkinNameFromEnvironment(...)`

## 5. Novi i važni compatibility API-ji

| API | Gdje | Namjena |
|---|---|---|
| `IVvDocumentHost` | `Framework/IVvDocumentHost.cs` | Host abstraction za VvForm i budući detached host. |
| `VvToolbarFactory` | `Framework/VvToolbarFactory.cs` | Factory/entry point za menu i toolbar operacije. |
| `DxBarManager` | `IVvDocumentHost` / `VvForm` | DevExpress manager za menu/toolbare. |
| `DxBarItemsByName` | `IVvDocumentHost` / `VvForm` | Lookup DevExpress bar itema po legacy imenu. |
| `VvTabPage.Selected` shim | `Framework/VvTabPage.cs` | Zadržava stari `Selected = true` contract, aktivira DX document. |
| `VvTabPage.Title` | `Framework/VvTabPage.cs` | Caption bridge prema `Document.Caption`. |
| `VvInnerTabControl.SelectedTabPage` | `Framework/VvTabPage.cs` | Compatibility za inner tabs. |
| `DxTreeView_Modul` | `zVvForm/Initializations_Settings.cs` / `Moduls_CommandPanel.cs` | DevExpress TreeList modul navigation. |
| `VvForm.ApplyDxSkin(string)` | `zVvForm/Initializations_Settings.cs` | Centralna primjena DX skina. |
| `VvForm.GetDxSkinNameFromEnvironment(...)` | `zVvForm/Initializations_Settings.cs` | Preference fallback za skin. |

## 6. Zabranjeni legacy API-ji

Novi kod ne smije uvoditi:

- `Crownwood.DotNetMagic.*`
- `DotNetMagicForm`
- `Crownwood.DotNetMagic.Controls.TabControl`
- `Crownwood.DotNetMagic.Controls.TabPage`
- `TreeControl`
- `Node` iz Crownwooda
- `ButtonWithStyle`
- `TitleBar`
- `VisualStyle`
- `OfficeStyle`
- `MediaPlayerStyle`
- `TreeControlStyles`
- `Office2007ColorTable`
- `MediaPlayerColorTable`
- reference na `DotNetMagic.DLL`

Ako treba wrapper zbog starog call-sitea, preferirati postojeći compatibility layer. Ne vraćati Crownwood reference.

## 7. Mapping tablica

| Legacy | Novi target | Napomena |
|---|---|---|
| `DotNetMagicForm` | `DevExpress.XtraEditors.XtraForm` ili `System.Windows.Forms.Form` | Glavna forma je XtraForm; izolirani dialog može biti WinForms Form. |
| Main `TabControl` | `DocumentManager` + `TabbedView` | Floating je disabled u Fazi 2. |
| Main `TabPage` | `VvTabPage` hosted kao `Document.Control` | `Selected`/`Title` shimovi ostaju. |
| Inner `TabControl` | `VvInnerTabControl` / `XtraTabControl` | Koristiti wrapper API-je. |
| Inner `TabPage` | `VvInnerTabPage` / `XtraTabPage` | Koristiti `SelectedTabPage`. |
| `TreeControl` | `TreeList` | `FocusedNodeChanged` umjesto `AfterSelect`. |
| `Node` | `TreeListNode` | `Tag` i image index se pune u `AppendDxTreeNode`. |
| WinForms ToolStrip item target | DevExpress `BarItem` target | Bridging kroz `VvToolbarFactory`. |
| Crownwood style enums | `DxSkinName` | String skin name, default `Office 2019 Colorful`. |
| Crownwood color tables | RGB constants | Nema runtime dependency na DotNetMagic. |

## 8. Kako dodati novi ekran/modul nakon migracije

1. Ne referencirati Crownwood.
2. Ako treba document/tab host, tražiti postojeći `VvTabPage` / `VvInnerTabControl` pattern.
3. Za main tab aktivaciju koristiti postojeće VvForm/VvTabPage helper-e, ne direktno manipulirati `TabbedView` osim u centralnom host layeru.
4. Za menu/toolbar elemente koristiti postojeće `VvMenu`/`VvSubMenu` strukture i `VvToolbarFactory` gdje je primjenjivo.
5. Za enable/disable logiku koristiti `VvToolbarFactory.ApplyWriteMode(...)` za nove call-siteove.
6. Za skin koristiti `VvForm.ApplyDxSkin(...)` i `DxSkinName`.
7. Za modul tree dodavanje koristiti postojeće node-building metode (`AppendDxTreeNode`, postojeće arraye `aDxTreeNode*`).

## 9. Kako prepoznati bug migracije

### 9.1 Tab lifecycle bug

Simptomi:

- toolbarovi se ne mijenjaju pri switchu taba,
- status bar ostaje od prethodnog taba,
- filter/report panel se ne refresha,
- aktivni tab vizualno nije isti kao logički aktivni tab.

Gledati:

- `TheTabControl_DocumentActivated`
- `TheTabControl_DocumentDeactivated`
- `VvTabPage.OnActivated()`
- `VvTabPage.OnDeactivated()`
- `VvTabPage.Selected` shim

Tipičan uzrok: kod još očekuje `VisibleChanged`, ali u `TabbedView` modelu kontrole ostaju `Visible=true`; mijenja se `ActiveDocument`.

### 9.2 Toolbar/menu state bug

Simptomi:

- Save/Delete/Print gumbi ostaju enabled/disabled krivo,
- WriteMode se ne reflektira nakon tab switcha,
- shortcut radi ali toolbar item ne radi ili obratno.

Gledati:

- `VvToolbarFactory.ApplyWriteMode(...)`
- `SetVvMenuEnabledOrDisabled_RegardingWriteMode(...)`
- `DxBarItemsByName`
- bridge iz `VvSubMenu.evHandler` prema `BarButtonItem.ItemClick`

Tipičan uzrok: novi kod direktno dira stari ToolStrip/MenuItem path umjesto factory/host lookupa.

### 9.3 Dirty prompt / close bug

Simptomi:

- zatvaranje taba ne pita za nespremljene podatke,
- tab se zatvori iako korisnik odustane,
- arhiva tab se može zatvoriti ili napustiti kad ne bi smio.

Gledati:

- `TheTabControl_DocumentClosing`
- `TheTabControl_TabMouseActivating`
- `HasTheTabPageAnyUnsavedData(...)`
- `IsArhivaTabPage`

Tipičan uzrok: close/switch path nije prošao kroz DevExpress `DocumentClosing`/`TabMouseActivating` event.

### 9.4 Skin/color bug

Simptomi:

- skin se ne primjenjuje nakon promjene u dialogu,
- boje modula/report filtera nisu usklađene,
- stari XML environment nema očekivani skin.

Gledati:

- `VvForm.ApplyDxSkin(...)`
- `GetDxSkinNameFromEnvironment(...)`
- `VvColorsStylsDlg.RadioBtn_FormStyle(...)`
- `VvHamper.SetUpVisualStyle()` / `StartPostavKaoMediaPlayer()`
- `VvColorsAndStyles.DxSkinName`

Tipičan uzrok: kod pokušava vratiti stari `VisualStyle` mental model. Aktivni model je `DxSkinName` + RGB constants.

### 9.5 Tree selection bug

Simptomi:

- klik na modul/submodul ne otvara očekivani ekran,
- report grupa se ne expandira pravilno,
- focus ostaje na nodeu i drugi klikovi imaju čudan efekt.

Gledati:

- `InitializeDxTreeViewModul()`
- `AppendDxTreeNode(...)`
- `DxTreeView_Modul_FocusedNodeChanged(...)`
- `DxTreeView_Modul_AfterExpand(...)`
- `HandleTreeView_ModulNodeTag(...)`
- `HandleTreeView_ModulReportGroupExpanded(...)`

Tipičan uzrok: `Tag` na `TreeListNode` nije tip/format koji handler očekuje ili nije resetiran `FocusedNode`.

## 10. Regression checklist prije release kandidata

Minimalno ručno provjeriti:

- aplikacija starta bez DotNetMagic DLL-a,
- login,
- otvaranje nekoliko modula,
- tab switch mišem i programatski,
- close tab s clean stateom,
- close tab s dirty stateom,
- arhiva tab: switch/close blokade,
- menu shortcuts,
- toolbar enable/disable kroz WriteMode,
- status bar tekst,
- modul tree root/submodul/report node klikovi,
- tree expand/collapse,
- color/skin dialog,
- spremanje i ponovno učitavanje environment preferenci,
- report preview/print/export,
- FIR outbound flow,
- FUR inbound flow,
- plaće,
- amortizacija,
- TEXTHOshop varijanta,
- PCTOGO varijanta,
- SvDUH varijanta.

## 11. Screenshot checklist

Usporediti prije/poslije za reprezentativne scenarije:

- main form nakon login-a,
- modul panel lijevo/desno,
- otvoren standardni DUC s record toolbarom,
- otvoren report tab/filter,
- dialog za boje/skin,
- warning/dirty prompt,
- arhiva blokada,
- report preview.

## 12. Commit reference

Zadnji cleanup blok nakon baselinea `e5b9e2f`:

| Commit | Sažetak |
|---|---|
| `e3b6287` | Uklonjen obsolete module `TreeControl` field. |
| `9b2b7e6` | `VvColors` decouplan od Crownwood color table API-ja. |
| `b93d9f0` | Uklonjeni obsolete Crownwood style selector-i. |
| `e6b8cb7` | Uklonjena legacy Crownwood style model polja. |
| `d68af3e` | Uklonjene `DotNetMagic` project reference. |
| `628c8c5` | Uklonjeni stale legacy helper nazivi. |
| `8284639` | Zatvorene autonomne Crownwood cleanup tracker stavke. |

Za punu povijest odluka koristiti `MarkDowns/DevExpress_Migration_V4.md` i git log na branchu `DevEx-JamesBond`.

## 13. Što je ostalo nakon Faze 2

Autonomni/coding dio Faze 2 do §2k je završen. Preostalo nije novi coding slice nego ljudska validacija:

- full regression test po modulima,
- screenshot usporedbe,
- odluka da je SWAP faza dovoljno stabilna za release candidate.

Tek nakon toga otvarati Fazu 3 DETACH rad.
