# DevExpress Migracija — RESUME / HANDOFF

> **Svrha:** Brza orijentacija pri nastavku rada nakon pauze. Autoritativni plan je
> `DevExpress_Migration_V4.md`. Ovaj dokument je samo **bookmark** — gdje smo stali,
> koji je sljedeći commit, što treba prvo pročitati.

---

## 📍 Gdje smo stali (stanje na dan pauze)

**Trenutni branch:** `DevEx-JamesBond` (remote `origin: qukatz22/VektorTest`)

**Zadnji završeni korak:** **Faza 2a / C18** — DevExpress default skin init
(`Office 2019 Colorful`) u `static void Main()` u `zVvForm\VvForm.cs`. Prvi
runtime semantic change u Fazi 2; clean-then-build verificiran EXIT 0.

**Sljedeći korak:** **Faza 2b / C19** — `VvForm : DotNetMagicForm` →
`VvForm : XtraForm` (zamjena base klase glavne forme). Najveći diff u Fazi 2
do sada — dotiče Crownwood-specific API-je u VvFormu (npr. `TabControl`,
`TabPage`, `Tab`, custom skin handlers). Detalje vidi V4 §3.2b.

**Status Faze 1 (Decoupling):** ✅ **POTPUNO ZAVRŠENA** (sve pod-faze 1a→1f kroz
commite C1–C16).

**Status Faze 2 (SWAP):** ⏳ pod-faza 2a završena (C17, C18); 2b sljedeća.

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

**Princip disciplinea kroz cijelu Fazu 1:** atomic commiti, fallback-safe delegation
(svaki call-site zadržava staru putanju ako delegat nije postavljen), zero behavioral
change, build green nakon svakog commita.

---

## ▶️ Sljedeći korak: P2b / C19 — `VvForm : DotNetMagicForm` → `VvForm : XtraForm`

Per V4 §3.2b — zamjena base klase glavne forme. Ovo je **najveći atomic commit
u Fazi 2 do sada** jer dotiče Crownwood-specific API-je u VvForm partial classes
(custom title bar, skin handlers, tab control hosting, ...). C18 je pripremio
DX skin engine; C19 prvi put uvodi DX UI element u UI hijerarhiju.

### Fiksirane odluke (V4 §6 + §3.2b)

- **Base klasa:** `DevExpress.XtraEditors.XtraForm` (najjednostavniji DX form base
  s skin podrškom, bez Ribbon/DocumentManager komplikacija — tih će biti u 2c).
- **Skin podrška:** XtraForm automatski poštuje `UserLookAndFeel.Default` koji je
  u C18 postavljen na `Office 2019 Colorful` — vizualna razlika od C18 vidljiva ovdje.
- **Crownwood `DotNetMagicForm`-specific API-ji** koji se moraju adresirati: vidi
  V4 §2.1 i §3.2b za popis (custom title, BackgroundImage, MagicMode, ...).
- **`IVvDocumentHost` ostaje** — explicit interface impl (C8) je rezistentan na
  promjenu base klase.

### C19 checklist (high-level)

- [ ] Audit `zVvForm\VvForm*.cs` partial files — gdje god je `DotNetMagicForm`
  type referenciran (base, polja, casts, event handlers).
- [ ] Audit `using Crownwood.DotNetMagic.Forms;` direktive — ostaju za sada
  (drugi tipovi iz tog namespacea su još u upotrebi do 2c-2g).
- [ ] Promijeniti `public partial class VvForm : DotNetMagicForm, IVvDocumentHost`
  → `public partial class VvForm : XtraForm, IVvDocumentHost`.
- [ ] Dodati `using DevExpress.XtraEditors;` (alfabetski kraj postojećeg
  `DevExpress.LookAndFeel`/`DevExpress.Skins` bloka iz C18).
- [ ] **Pažljivi audit override-ova** u VvForm partials: `OnPaint`, `OnPaintBackground`,
  `WndProc`, `CreateParams`, `OnLoad`, `OnFormClosing`, `OnSizeChanged` — Crownwood
  vs XtraForm imaju različite virtuali; svaki override mora biti testiran.
- [ ] **Crownwood-specific properties** koji ne postoje na XtraForm: ako ih VvForm
  konfigurira u InitializeComponent ili runtime kodu, treba ili maknuti ili
  pronaći XtraForm ekvivalent.
- [ ] **Clean-then-build (per pravilo #8)** — verificirati EXIT 0 prije commita.
- [ ] Smoketest: aplikacija se pokrene, glavna forma se vidi s **Office 2019
  Colorful skinom** (prva vidljiva razlika), tabovi se otvaraju, VvForm
  funkcionalnost (login, modul switching, status bar) radi.
- [ ] Append C19 row u V4 §6 tracker.

### Potencijalni rizici za C19

- **Custom title bar / non-client area.** Crownwood ima svoj custom paint za title
  bar; XtraForm također custom paint ali drugačijim mehanizmom. Ako VvForm
  override-uje paint logic, mora se prilagoditi.
- **MdiContainer behaviour.** Vektor je MDI app (više modula u tabovima); XtraForm
  podržava MDI ali kroz `IsMdiContainer` property — mora se zadržati.
- **TabControl host.** Glavna forma hosta `TabControl` (vjerojatno Crownwoodov
  `Crownwood.DotNetMagic.Controls.TabControl`); to **ostaje Crownwood do Faze 2c**.
  Mora se osigurati da XtraForm uredno hosta Crownwood control kao child — što bi
  trebalo raditi (svaka Form hosta bilo koji Control).
- **DialogResult / FormBorderStyle** — XtraForm i DotNetMagicForm imaju iste base
  Form properties, no value defaults mogu se razlikovati.
- **Designer surface.** Ako VvForm ima `.Designer.cs` (vjerojatno) — designer može
  poludjeti pri prvom otvaranju nakon base klase swap. Predlaže se: **NE otvarati
  VvForm u designeru** dok C19 ne bude commitan i build green; otvaranje samo iz
  command line / Solution Explorer kao text edit.

**Recovery strategy:** ako C19 build-fail bude pre-velik, opcija je **revert svega
osim skin init iz C18** i reorganizirati 2b u manje commite (2b1 = pripremne
izmjene partial-classa, 2b2 = base swap). To će se procijeniti kad se vidi opseg
diff-a tijekom C19 implementacije.

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
