# DevExpress Migracija — RESUME / HANDOFF

> **Svrha:** Brza orijentacija pri nastavku rada nakon pauze. Autoritativni plan je
> `DevExpress_Migration_V4.md`. Ovaj dokument je samo **bookmark** — gdje smo stali,
> koji je sljedeći commit, što treba prvo pročitati.

---

## 📍 Gdje smo stali (stanje na dan pauze)

**Trenutni branch:** `DevEx-JamesBond` (remote `origin: qukatz22/VektorTest`)

**Zadnji završeni commit:** **Faza 2i / C43 skin persistence cleanup** — commit
`9f4d9d5` (`C43 persist selected DX skin`). Load path kopira
`VvColorsAndStylesEnv.DxSkinName` prije fallback mappinga, pa se spremljeni DX skin
poštuje nakon restarta. Default `InitializeVvColorsAndStyles()` postavlja
`DxSkinName` preko istog fallback helpera. `vvform_VisualStyle` ostaje samo legacy
XML fallback za stare environment datoteke.

**Trenutni necommitani checkpoint:** **Faza 2i / C44 skin phase closure** —
`UtilsEtc\VvColors.cs` više ne postavlja `this.Style = ZXC.vvColors.vvform_VisualStyle`.
V4 §2i checklist u `DevExpress_Migration_V4.md` označen je dovršenim: mapper,
dialog DX skin izbor i environment fallback/load path su implementirani.

**Sljedeći korak:** validirati C44 build/smoke, zatim krenuti u 2j (`VvHamper`
decouple). Detach ostaje za Fazu 3.

**2h autoritativni anchor (V4 §2h):** preferirani target je `TreeList` zbog DX
konzistencije; konfigurirati 1 `TreeListColumn`; populate preko `AppendNode`;
event mapping `AfterSelect` → `FocusedNodeChanged`; ikone preko `SelectImageIndex`.

**2h code surface koji prvo treba dirati/pročitati:**
- `zVvForm\Initializations_Settings.cs` — TreePanel fields: `TreeView_Modul`,
  `aTreeNode0_Modul`, `aTreeNode1_SubModul`, `aTreeNode2_ReportModul`.
- `zVvForm\Moduls_CommandPanel.cs` — init/populate path: `TreeView_Modul = new
  TreeControl()`, `TreeView_Modul_AfterSelect`, `InitializeTreeView_ModulNode0`,
  node arrays, `Nodes.Add`, `ImageIndex`, `SelectedImageIndex`, `ExpandedChanged`.
- `zVvForm\OnClick_EventHandlers.cs` — event sender casts to `TreeControl` and
  `SelectedNode.Tag` in modul/report click paths.
- `zVvForm\TabControl_TabPages.cs` — calls to `aTreeNode0_Modul[xy.X].Expand()` and
  `aTreeNode1_SubModul[xy.X][xy.Y].Expand()`.

**Predloženi nastavak 2h slicing:** C37 izolira preostale Crownwood `TreeControl`
usage-e za `TreeView_Modul`; C38 završava V4 §2h checklist i tracker update nakon
runtime smoke testa lijevog modul tree-a.

**Status Faze 1 (Decoupling):** ✅ **POTPUNO ZAVRŠENA** (sve pod-faze 1a→1f kroz
commite C1–C16).

**Status Faze 2 (SWAP):** ⏳ pod-faze 2a (C17, C18), 2b (C19), 2c-refactor
(C20a), 2c-kontejner (C20b), početni 2d rebase (C21), 2e (C22), 2f (C23) i
2g završeni; sljedeća je 2h.

---

## ✅ Što je gotovo (Faza 1, C1–C16)

| Pod-faza | Commiti | Ishod |
|---|---|---|
| 1a — ZXC infrastruktura | C1–C7 | `ActiveDocumentHost` registar, DB fixovi, path/status-text delegate sink, `ResetAll_GlobalStatusVariables()` prošireno |
| 1b — `IVvDocumentHost` + `VvToolbarFactory` | C8–C11 | Interface definiran, VvForm implementira kroz explicit impl; `ApplyWriteMode` kontrakt (Option B — tijelo ostaje na VvFormu do Faze 2g) |
| 1c — VvUserControl decoupling | C12 | Settable `TheVvTabPage` + `DocumentHost` property s fallbackom |
| 1d — Business layer decoupling | C13, C14 | `Rtrans` rutiran kroz `Active(Document)RecordUCProvider`; status text push/pop sink (`StatusTextPusher`/`Popper`) |
| 1e — Per-host flag bucket | C15 | `Framework\VvPerHostState.cs` + `IVvDocumentHost.PerHost` (infrastruktura; flipanje call-siteova u Fazi 3) |
| 1f — Audit | C16 | Workspace census: **1 755 `ZXC.TheVvForm` call-siteova / 120 fajlova**; V4 §3.1f popis pokriva samo 10 %; R7 potvrđen |
| 2a — DX reference setup | C17 | 6 DX DLL referenci dodano u `Vektor.csproj` (Opcija B: direct DLL, ne NuGet); pokriva XtraEditors, XtraBars, XtraTreeList + dependencies; clean-then-build verificiran EXIT 0; uvedeno **pravilo discipline #8** (clean-then-build invarianta) |
| 2a — DX skin init | C18 | `Office 2019 Colorful` skin postavljen u `Main()`; `SkinManager.EnableFormSkins()` + `UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful")`; 2 nova `using`-a (`DevExpress.LookAndFeel`, `DevExpress.Skins`); zero behavioral impact (nema DX kontrola u UI hijerarhiji); skin engine učitan i čeka Fazu 2b |
| 2b — VvForm base swap | C19 | `VvForm : DotNetMagicForm` → `VvForm : DevExpress.XtraEditors.XtraForm` u **9 partial deklaracija** (`zVvForm\VvForm.cs`, `Initializations_Settings.cs`, `Menus_ToolStrips.cs`, `Moduls_CommandPanel.cs`, `OnClick_EventHandlers.cs`, `SubModulActions.cs`, `TabControl_TabPages.cs`, `VvForm_Q.cs`, `UtilsEtc\F2eRacuni.cs`); FQN korišten (bez novog `using DevExpress.XtraEditors;`); `VvForm_IVvDocumentHost.cs` netaknut (samo interface, bez base). Decommissioning: 5 `Form.Style` referenci komentirano (3× `this.Style = …`, 1 compound `ZXC.vvColors.vvform_VisualStyle = this.Style = X;` razdvojen, 1× `ZXC.TheVvForm.Style` u `VvColorsStylsDlg.cs`). `ZXC.vvColors.vvform_VisualStyle` field i `tabControlColors.Style` zadržani (Crownwood child controls i dalje žive). **Tool discipline:** isključivo `multi_replace_string_in_file` (14/14 OK u jednom batch-u); zero PowerShell `WriteAllText`. Encoding očuvan: 4× UTF-16 LE, 5× UTF-8 noBOM, 1× UTF-8 BOM (verificirano BOM signature inspection). Clean-then-build EXIT 0, smoketest ✅: app startira, Office 2019 Colorful vidljiv na glavnoj formi, tabovi i login funkcionalni. Lekcija: prošli failed C19 attempt pokvario je 5 UTF-16 fajlova `WriteAllText` overwriteom; retry plan dokumentiran u tracker razgovoru. |
| 2c (refactor) — VvTabPage event extract | C20a | `VvTabPage_VisibleChanged` body u `Framework\VvTabPage.cs` extracted u dvije nove `public` metode: `OnActivated()` (cijeli `else`-blok aktivacije — first appearance + repeat-activation grane) i `OnDeactivated()` (`if (Visible == false)` deaktivacijski blok). Stari `VvTabPage_VisibleChanged(sender, e)` postaje 4-redni dispatcher koji čita `this.Visible` i poziva odgovarajuću metodu. Sve postojeće hookove (`this.VisibleChanged += ...`), programski poziv `VvTabPage_VisibleChanged(null, null)` i helper metode (`PutTSB_EnabledStateSnapshot`, `GetTSB_EnabledStateSnapshot`, `ChangeVisibilitiOfToolStripAndMenuItem_SubModulSet`, `VvTabPage_Validating`) **netaknute**. Imena `OnActivated`/`OnDeactivated` sigurna (Crownwood `TabPage` ne emita virtuale tih imena; workspace-wide regex scan zero matches). Diff: 33 insertions / 24 deletions, text mode. Encoding UTF-8 noBOM očuvan. Clean-then-build EXIT 0, zero CS errors, smoketest ✅ (identično ponašanje kao C19). **Strateški cilj per V4 §3.2c korak 3:** sad postoje gotove metode kao binding-target za DX event-e: `tabbedView.DocumentActivated += (s,e) => ((VvTabPage)e.Document.Control).OnActivated();`. Bez ovog refactor-a, C20b kontejner swap morao bi istovremeno raditi i extract — razbijanje na C20a + C20b je primjena **pravila atomic commiti (#1)**. |
| 2c/2d — main tab host + VvTabPage rebase | C20b-C21 (`447ce29`) | Glavni `TheTabControl` prešao na `DocumentManager` + `TabbedView`; `VvTabPage` rebase-an na `UserControl` jer se content nije prikazivao dok je ostao Crownwood `TabPage`. Dodani `Title`/`Image`/`Selected` shimovi za `Document.Caption`, `Document.ImageOptions.Image` i `TabbedView.Controller.Activate`. Dirty prompt uklonjen iz običnog tab switcha; ostaje na close/exit. Arhiva blokira close/switch i otvaranje novog taba. `F2_Izlaz_UC`/`F2_Ulaz_UC` refresh castovi prilagođeni. Clean-then-build EXIT 0; F5 smoke ✅ content visible. |

**Princip disciplinea kroz cijelu Fazu 1:** atomic commiti, fallback-safe delegation
(svaki call-site zadržava staru putanju ako delegat nije postavljen), zero behavioral
change, build green nakon svakog commita.

---

## ▶️ Sljedeći korak: P2c-kontejner / C20b — `TheTabControl`: Crownwood `TabControl` → DX `DocumentManager` + `TabbedView`

Per V4 §3.2c (linije 531-539) — zamjena glavnog tab kontejnera koji hosta sve
module/dokumente. C20a (refactor) je već zatvoren, pa `OnActivated`/`OnDeactivated`
metode sad postoje kao binding-target za DX event-e.

**V4 autoritativni anchor (§2.2 #1, linija 400-404, citirano doslovno):**

> *"Glavni tab: `DocumentManager` + `TabbedView` od Faze 2 odmah. Iako V3 preporuka
> bijaše `XtraTabControl`, odlučujemo od početka koristiti `TabbedView` jer: API
> je kompatibilan za običan tabbed rad (swap faza); Kad dođe Faza 3 (detach), ne
> treba druga migracija; Izbjegava se dvostruko diranje istog koda (ključni
> argument za V4)."*

**Posljedica za C20b:** ruta je **`DocumentManager + TabbedView`**, ne `XtraTabControl`.
Paradigm-shift `TabPages.Add(tabPage)` → `view.AddDocument(new Document { Control = ... })`
je **prihvaćen trošak** koji V4 svjesno preuzima da izbjegne dvostruki rad u Fazi 3.

Glavne točke koje C20b dotice:

- `VvTabPage_Validating` arhiva-blokada → `TabbedView.DocumentClosing` s
  `e.Cancel = isArchiveMode` (per V4 §3.2c korak 4).
- Crystal Reports BackgroundWorker lifecycle vezan za tab activation — mora
  ostati funkcionalan kroz nove event-e.
- `thisIsFirstAppereance` flag mora preživjeti u novom modelu (postavlja se
  `false` nakon prve aktivacije; novi model: prva `DocumentActivated` poziva
  `OnActivated()` koja postavlja `false`).

### Fiksirane odluke (V4 §6 + §3.2c)

- **Glavni kontejner:** `DocumentManager` + `TabbedView` (per V4 §3.2c). Ovo daje
  bazu za Fazu 3 (FLOATING-DETACH).
- **Floating eksplicitno OFF u Fazi 2:** `TabbedView.DocumentGroups` jedan group,
  `tabbedView.DocumentProperties.AllowFloat = false` (ili ekvivalent) — built-in
  floating uključujemo tek u Fazi 3.
- **`VvTabPage` ostaje Crownwood `TabPage` u C20** — sub-faza 2d adresira njegov
  swap. C20 fokus je samo na **kontejneru** i njegovim event handlerima. Tijekom
  C20 trebamo wrapper koji hosta Crownwood `TabPage` instance unutar DX `Document`
  (najjednostavnije: `Document.Control = vvTabPage as Control`, jer Crownwood
  TabPage nasljeđuje od `Control`).
- **Smoketest acceptance criteria:** sva otvaranja modula iz lijevog modul-panel
  rade; tab switching radi; arhiva-blokada radi (otvori Faktur, switchaj na
  arhivirani modul → ne smije zatvoriti); status bar se ažurira pri tab switch.

### C20b checklist (high-level — detalji u V4 §3.2c)

- [ ] Audit `zVvForm\TabControl_TabPages.cs` i `zVvForm\VvForm.cs` Fieldz
  region — sve reference na `Crownwood.DotNetMagic.Controls.TabControl` u
  glavnoj formi (polje `workTabControl`, getter `TheTabControl`, ...).
- [ ] Promijeniti `workTabControl` polje na `DevExpress.XtraBars.Docking2010.DocumentManager`
  + `DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView` (per V4 §2.1 +
  §2.2 #1).
- [ ] Mappirati `TheTabControl.TabPages.Add(vvTabPage)` →
  `tabbedView.AddDocument(new Document { Control = vvTabPage, Caption = vvTabPage.Title })`
  na svim 308 call-siteovima (per C16 audit; FakturDUC family ima najveći
  hit-count).
- [ ] Wire-up `TabbedView.DocumentActivated` → `((VvTabPage)e.Document.Control).OnActivated()`
  + `DocumentDeactivated` → `OnDeactivated()` (per V4 §3.2c korak 3 + C20a
  preparation).
- [ ] Migrirati `VvTabPage_Validating` logiku u `TabbedView.DocumentClosing`
  s `e.Cancel = isArchiveMode` (per V4 §3.2c korak 4).
- [ ] **Eksplicitno ugasiti built-in floating:** `tabbedView.DocumentProperties.AllowFloat = false`
  (per V4 §3.2c korak 7) — floating se uključuje tek u Fazi 3.
- [ ] **Crownwood `TabPage` kao `Document.Control`:** Crownwood `TabPage`
  nasljedjuje od `Control` pa stane u `Document.Control` slot bez ikakvog
  wrapper-a. Provjeriti da DX ne zalupi na non-DX child-u (ne bi smio).
  VvTabPage rebase je **zasebna pod-faza 2d (C21)** — NIJE u scope-u C20b.
- [ ] **Clean-then-build (per pravilo #8)** — verificirati EXIT 0 prije commita.
- [ ] Smoketest po acceptance kriterijima:
  - sva otvaranja modula iz lijevog modul-panela rade;
  - tab switching radi;
  - arhiva-blokada radi (otvori Faktur, switch-aj na arhivirani modul → ne
    smije zatvoriti);
  - status bar se ažurira pri tab switch (sink iz C5+C14 i dalje radi);
  - `PaliGasiDirtyFlag` radi pri editu polja (dirty ikona se prikazuje na tab);
  - Crystal Reports BackgroundWorker lifecycle preserve-an.
- [ ] Append C20b row u V4 §6 tracker.

### Potencijalni rizici za C20

- **`VvTabPage_VisibleChanged` event ne postoji u DX modelu** — DX koristi
  `DocumentActivated`/`DocumentDeactivated` događaje na `TabbedView`. Mapping nije
  1-na-1 (DX ne emita za sve scenario-e za koje Crownwood emita `VisibleChanged`).
  Treba pažljivo testirati: program-driven activate vs user-click activate vs
  reload-existing tab.
- **Crownwood `TabPage` kao `Document.Control`** — provjeriti da DX wrapper ne
  zalupi na non-DX child controlu (ne bi smio jer `Control` je univerzalan).
- **Crystal Reports BackgroundWorker** — neki taboji startaju async report load
  na activation; novi event timing može poremetiti redoslijed.
- **Status bar i TS toolbar updates** — `tsBtn_*` enable/disable po activeUC
  trenutno ide kroz Crownwood `SelectionChanged`; treba mapirati na DX
  `DocumentActivated`.
- **Designer surface** — VvForm partial s `InitializeComponent` (ako postoji za
  workTabControl) treba ručno editirati kao text, ne kroz Designer (Designer
  može pokušati regenerirati `Crownwood.TabControl` polje).

**Recovery strategy:** ako C20 ispadne pre-veliki, split na C20a (extract
`OnActivated`/`OnDeactivated` u `VvTabPage` — pure refactor, build green s
identičnim runtime behavior-om) + C20b (kontejner swap koji koristi nove metode).
C20a je nizak rizik i lokaliziran; C20b je veći ali s jasnim scope-om.

---

## 📂 Ključni fajlovi za kontekst pri nastavku

Prioritet čitanja (sljedeći put):

1. **`MarkDowns\DevExpress_Migration_V4.md`** — §6 tracker (C1→C16) + §3.2a checklist.
2. **`MarkDowns\DevExpress_Migration_V4_RESUME.md`** — ovaj fajl.
3. **`Framework\IVvDocumentHost.cs`** — trenutna verzija interface-a (važno za
   Fazu 2g kad se tipovi retypiraju iz `ToolStrip` u `Bar`/`BarManager`).
4. **`Framework\VvPerHostState.cs`** — per-host flag bucket (C15 infrastruktura).
5. **`Framework\ZXC.cs`** — Status Text Sink regija (C5+C14), path provideri (C4),
   `ActiveDocumentHost` registar (C1), `ResetAll_GlobalStatusVariables()` (C7).
6. **`zVvForm\VvForm_IVvDocumentHost.cs`** — partial s explicit interface impl (C8).
7. **`zVvForm\Initializations_Settings.cs`** — wire-up lambdi u `InitializeVvForm()`
   (C4, C5, C14).

---

## 🧭 Dugoročni roadmap (podsjetnik)

```
Faza 1 (Decoupling)   ✅ ZAVRŠENO  (C1–C16)
Faza 2 (SWAP)         ⏳ U TIJEKU
  ├── 2a NuGet setup           ✅ C17 — direct DLL references, Opcija B (NE NuGet)
  │                            ✅ C18 — skin init u VvForm.Main() (Office 2019 Colorful)
  ├── 2b VvForm → XtraForm     ✅ C19
  ├── 2c (refactor) VisibleChanged extract  ✅ C20a
  ├── 2c (kontejner) TheTabControl → DocumentManager+TabbedView   ✅ C20b
  ├── 2d VvTabPage rebase (Crownwood.TabPage → UserControl)        ✅ C21
  ├── 2e VvInnerTabPage → XtraTabPage + DUC-internal Crownwood.TabControl → XtraTabControl  ⏳ SLJEDEĆE
  ├── 2f UC-ovi s unutarnjim TabControl-ima (priority rewritten per C16 audit —
  │      FakturDUC family first, not V4 §3.1f order)
  ├── 2g MenuStrip+ToolStrip → BarManager (tu ApplyWriteMode tijelo migrira iz
  │      VvForma u VvToolbarFactory — Option B terminira)
  ├── 2h TreeControl → TreeList
  ├── 2i VvColors → SkinStyle
  ├── 2j VvHamper decouple
  └── 2k Cleanup (Crownwood DLL uklonjen)
Faza 3 (DETACH)       🕓 NAKON 1–2 MJESECA PRODUKCIJE Faze 2
                          — aktivacija `DocumentProperties.AllowFloat = true` +
                            VvFloatingForm + DB concurrency + per-host flagovi
Faza 4 (Cleanup/docs) 🕓
```

---

## 🔑 Pravila discipline koja moramo zadržati u Fazi 2

Iskustva iz Faze 1 koja su se pokazala ispravnima:

1. **Atomic commiti.** Jedan koncept po commitu. Tracker row u V4 §6 nakon svakog.
2. **Build green invarianta.** Svaki commit kompajlira; nikad „završit ću sljedeći put".
3. **Fallback-safe delegation.** Kad uvodimo indirekciju, stara putanja mora i dalje
   raditi ako delegat/provider nije postavljen.
4. **Option B disciplina.** Ne mijenjaj kontrakt (interface/factory) bez stvarne
   potrebe; tijelo može ostati na „starom" mjestu dok ga prirodni slijed ne prisili
   na preselenje (case-in-point: `ApplyWriteMode` ostaje na VvFormu do 2g).
5. **Zero behavioral change** kao default; ako commit mijenja ponašanje, mora biti
   eksplicitno naglašeno u tracker row-u.
6. **Croatian komentari, C# 7.3, .NET Framework 4.8, Newtonsoft.Json, `HttpWebRequest`**
   — codebase konvencije se ne dodiruju.
7. **Jedan kontinuirani response block** po turn-u (user preference iz
   copilot-instructions).
8. **Clean-then-build pri verifikaciji „build green".** MSBuild incremental cache
   (`obj\` folder + `.suo`/`.vs` cache) zna tvrditi `CS0246` greške na tipovima
   koje csproj zapravo uredno resolva (otkriveno između C16 i C17 pri istrazi
   phantom `Hapi`/`HandpointSDK`/`PusherClient` greške). **Disciplina:** prije
   svakog tvrdnje „build green" pokrenuti **clean-then-build** sekvencu, ne samo
   incremental build.
   - **Iz CLI-a (autoritativno):**
     ```powershell
     $msb = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
     & $msb 'Vektor.csproj' /t:Clean /v:minimal /nologo
     & $msb 'Vektor.csproj' /v:minimal /nologo /p:Configuration=Debug
     ```
     Exit code `0` = green; bilo što drugo = red.
   - **Iz VS-a (sekundarno, brže za development):** `Build → Clean Solution`,
     pa `Build → Build Solution` (NE samo F6 koji je incremental).
   - Copilot agent `run_build` tool je **incremental-only** i ne čisti cache —
     njegov rezultat treba uzimati s rezervom; pri sumnji preferirati CLI sekvencu
     iznad.
   - Phantom `PusherClient.dll` missing-dep warning **NIJE blocker** (transitive
     dep koji nijedan source kod ne koristi) — može se ignorirati u tracker
     row-ovima ako MSBuild exit kod je 0.
9. **Copilot ne pokreće mutate-history git komande bez izričite usmene autorizacije
   korisnika u istom turn-u.** Zabranjeno bez eksplicitne dozvole: `git commit`,
   `git push`, `git reset --hard`, `git revert`, `git rebase`, `git cherry-pick`,
   `git stash drop`, `git tag`, brisanje branchova, force-push. **Dozvoljeno slobodno**
   (read-only): `git status`, `git log`, `git diff`, `git show`, `git for-each-ref`,
   `git branch -r --contains`. **Dozvoljeno samo kao recovery** (uz jasno navedenu
   svrhu): `git checkout -- <file>` i `git update-ref refs/backup/...` — isključivo
   za oporavak vlastite greške u worktree-ju. **Disciplina:** sve tracker update-ove,
   code edit-ove i bilo koje izmjene fajlova Copilot ostavlja u **working tree-ju**;
   korisnik pregleda diff (kroz VS Git UI ili `git diff`) i commit-a ručno odlučujući
   sam tajming, granuliranost i poruku. Pravilo uvedeno post-Strategija-X incidenta
   (commiti `06ff075` + `b34e4ac`, oba revertana hard reset-om na `1db559a` jer su
   bili V4-deviation), gdje je Copilot pogrešno samostalno commitao tracker izmjenu.
10. **`MarkDowns/DevExpress_Migration_V4.md` je AUTORITATIVAN plan migracije.**
    Bilo kakvo strateško odstupanje (smjer, redoslijed, tip-targeti, scope faze)
    zahtijeva eksplicitnu autorizaciju korisnika i dokumentirani amandman u V4.md.
    Tracker entry-ji u V4 §6 ne smiju samostalno odstupati od V4 §0–§5; ako se to
    dogodi, to je bug, ne odluka.
    - **Obavezni pre-flight format za svaki commit koji predlaže strateški smjer**
      (ne taktiku):
      ```
      Prije commita Cxx koji ima smjer "[X]":
      - V4 §[paragraf] kaže: "[doslovni citat]"
      - Moja akcija usklađena s citatom: [da/ne]
      - Ako ne: STOP, pitaj korisnika.
      ```
    - **Tracker entry-ji odstupanja** moraju biti označeni `V4-deviation — REQUIRES V4 amendment`,
      a V4 §0–§5 mora biti revidiran prije code rada.
    - **V4 sanity check pri restart sesije:** prvi prompt nakon pauze treba biti
      `"Pročitaj MarkDowns/DevExpress_Migration_V4.md §2.2, §3.2[trenutna pod-faza], §6
      zaključak. Reci u 5 rečenica što plan traži. NE čitaj tracker prije."` Time se
      izbjegava da Copilot koristi (potencijalno krive) tracker zapise kao primarni
      autoritet umjesto V4 §0–§5 plana. Pravilo uvedeno post-Strategija-X incidenta
      gdje je Copilot dvaput odlutao od V4 §2.2 #1 (DocumentManager od početka) prema
      vlastitoj XtraTabControl interpretaciji koja nikad nije bila u V4.

---

## ✂️ Gdje pogledati ako zaboraviš specifičan detalj

| Pitanje | V4 sekcija |
|---|---|
| Što radi C?? commit? | §6 tracker |
| Koji flagovi su per-host vs global? | §1.14 |
| DB connection concurrency opcije? | §1.15, §3.3d |
| Koje Crownwood tipove zamjenjujemo čime? | §2.1 |
| Koje DUC-ove treba migrirati u 2f i kojim redoslijedom? | §3.2f + **C16 audit (B) tablica** — FakturDUC family ima prioritet nad V4 §3.1f popisom |
| Koje rizike pratimo? | §4 (R1–R15) |

---

**Kraj handoff dokumenta.** Sljedeći prompt nakon pauze može biti jednostavno
„P2b / C19" i imaš sve što trebaš.
