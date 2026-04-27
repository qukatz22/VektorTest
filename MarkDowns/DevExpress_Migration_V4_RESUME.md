# DevExpress Migracija — RESUME / HANDOFF

> **Svrha:** Brza orijentacija pri nastavku rada nakon pauze. Autoritativni plan je
> `DevExpress_Migration_V4.md`. Ovaj dokument je samo **bookmark** — gdje smo stali,
> koji je sljedeći commit, što treba prvo pročitati.

---

## 📍 Gdje smo stali (stanje na dan pauze)

**Trenutni branch:** `DevEx-JamesBond` (remote `origin: qukatz22/VektorTest`)

**Zadnji završeni korak:** **Faza 2b / C19** — `VvForm : DotNetMagicForm` →
`VvForm : DevExpress.XtraEditors.XtraForm`. Base klasa zamijenjena u **9 partial
klasa** (`zVvForm\*.cs` × 8 + `UtilsEtc\F2eRacuni.cs`); 5 referenci na
Crownwood-specific `Form.Style` property (koje XtraForm ne nudi) komentirano.
`ZXC.vvColors.vvform_VisualStyle` field zadržan jer ga TabControl/TreeControl
(i dalje Crownwood) i `VvColorsStylsDlg` koriste; Faza 2i preuzima migraciju
VvColors → DX SkinStyle. Encoding 4 UTF-16 LE fajla preserve-an (svi `multi_replace_string_in_file` poziv); zero CS errors,
clean-then-build EXIT 0; smoketest ✅ (aplikacija startira s Office 2019
Colorful skinom, tabovi rade, login/modul switching OK).

**Sljedeći korak:** **Faza 2c / C20** — glavni `TheTabControl`:
`Crownwood.DotNetMagic.Controls.TabControl` → DX `DocumentManager` + `TabbedView`
(ili kao manje invazivna alternativa za prvi korak: `XtraTabControl`).
**NAJOSJETLJIVIJA OPERACIJA cijele migracije** per V4 §3.2c — ~100 redaka
specijalne logike u `VvTabPage_VisibleChanged` koje treba extract-ati u
`OnActivated()`/`OnDeactivated()` metode. Detalje vidi V4 §3.2c.

**Status Faze 1 (Decoupling):** ✅ **POTPUNO ZAVRŠENA** (sve pod-faze 1a→1f kroz
commite C1–C16).

**Status Faze 2 (SWAP):** ⏳ pod-faze 2a (C17, C18) i 2b (C19) završene; 2c sljedeća.

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

**Princip disciplinea kroz cijelu Fazu 1:** atomic commiti, fallback-safe delegation
(svaki call-site zadržava staru putanju ako delegat nije postavljen), zero behavioral
change, build green nakon svakog commita.

---

## ▶️ Sljedeći korak: P2c / C20 — `TheTabControl`: Crownwood `TabControl` → DX `DocumentManager` + `TabbedView`

Per V4 §3.2c — zamjena glavnog tab kontejnera koji hosta sve module/dokumente.
**Najosjetljivija operacija cijele migracije** jer dotiče:

- ~100 redaka specijalne logike u `VvTabPage_VisibleChanged` (treba extract
  u `OnActivated()` / `OnDeactivated()` metode na `VvTabPage`).
- `VvTabPage_Validating` logika koja blokira zatvaranje arhiva taba
  (mora se mapirati na `TabbedView.DocumentClosing` s `e.Cancel = isArchiveMode`).
- Crystal Reports BackgroundWorker lifecycle vezan za tab activation.
- `thisIsFirstAppereance` flag mora preživjeti u novom modelu.

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

### C20 checklist (high-level — detalji u V4 §3.2c)

- [ ] Audit `zVvForm\TabControl_TabPages.cs` i `zVvForm\VvForm.cs` Fieldz
  region — sve reference na `Crownwood.DotNetMagic.Controls.TabControl` u
  glavnoj formi (polje `workTabControl`, getter `TheTabControl`, ...).
- [ ] Audit `VvTabPage_VisibleChanged` body (vjerojatno u `Framework\VvTabPage.cs`
  ili sličnom) — ovo je centar gravitacije C20.
- [ ] Decision split: implementirati `OnActivated()` i `OnDeactivated()` na
  `VvTabPage` kao **prvi atomic** (C20a), pa swap kontejnera (C20b)? Ili kao
  jedan veliki commit (C20)? Procijeniti kad se vidi opseg `VisibleChanged`.
- [ ] Promijeniti `workTabControl` polje s `Crownwood.DotNetMagic.Controls.TabControl`
  na `DevExpress.XtraBars.Docking2010.DocumentManager` + `TabbedView`.
- [ ] Mappirati `SelectedTab`/`SelectedIndex` API kroz V4 §3.2c tablicu.
- [ ] **Clean-then-build (per pravilo #8)** — verificirati EXIT 0 prije commita.
- [ ] Smoketest po acceptance kriterijima gore.
- [ ] Append C20 row u V4 §6 tracker.

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
Faza 2 (SWAP)         ⏳ U TIJEKU  (2a done; 2b next)
  ├── 2a NuGet setup  ✅ C17 — direct DLL references, Opcija B (NE NuGet)
  │                   ✅ C18 — skin init u VvForm.Main() (Office 2019 Colorful)
  ├── 2b VvForm → XtraForm    ⏳ SLJEDEĆE — C19
  ├── 2c TheTabControl → DocumentManager+TabbedView (najosjetljivije!)
  ├── 2d VvTabPage rebase
  ├── 2e VvInnerTabPage → XtraTabPage
  ├── 2f UC-ovi s unutarnjim TabControl-ima (priority rewritten per C16 audit —
  │      FakturDUC family first, not V4 §3.1f order)
  ├── 2g MenuStrip+ToolStrip → BarManager (tu ApplyWriteMode tijelo migrira iz
  │      VvForma u VvToolbarFactory — Option B terminira)
  ├── 2h TreeControl → TreeList
  ├── 2i VvColors → SkinStyle
  ├── 2j VvHamper decouple
  └── 2k Cleanup (Crownwood uklonjen)
Faza 3 (DETACH)       🕓 NAKON 1–2 MJESECA PRODUKCIJE Faze 2
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
