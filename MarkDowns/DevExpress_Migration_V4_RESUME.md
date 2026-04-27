# DevExpress Migracija — RESUME / HANDOFF

> **Svrha:** Brza orijentacija pri nastavku rada nakon pauze. Autoritativni plan je
> `DevExpress_Migration_V4.md`. Ovaj dokument je samo **bookmark** — gdje smo stali,
> koji je sljedeći commit, što treba prvo pročitati.

---

## 📍 Gdje smo stali (stanje na dan pauze)

**Trenutni branch:** `DevEx-JamesBond` (remote `origin: qukatz22/VektorTest`)

**Zadnji završeni korak:** **Faza 2a / C17** — DevExpress reference setup
(Opcija B: direct DLL references, 6 DX DLL-ova ubačeno u `Vektor.csproj`;
clean-then-build verificiran EXIT 0).

**Sljedeći korak:** **Faza 2a / C18** — Default skin init (`Office 2019 Colorful`)
u `static void Main()` u `zVvForm\VvForm.cs` (NE `Program.cs` — ne postoji;
`<StartupObject>VvForm</StartupObject>`). Ovo je **prvi runtime semantic change**
u Fazi 2 i zato u zasebnom commitu.

**Status Faze 1 (Decoupling):** ✅ **POTPUNO ZAVRŠENA** (sve pod-faze 1a→1f kroz
commite C1–C16).

**Status Faze 2 (SWAP):** ⏳ započeta s C17.

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

**Princip disciplinea kroz cijelu Fazu 1:** atomic commiti, fallback-safe delegation
(svaki call-site zadržava staru putanju ako delegat nije postavljen), zero behavioral
change, build green nakon svakog commita.

---

## ▶️ Sljedeći korak: P2a / C18 — Default skin init

Per V4 §3.2a (preostali dio nakon C17 references) — **prvi runtime semantic change** u
Fazi 2. C17 je uveo DX reference; C18 ih prvi put **koristi** u runtime kodu.

### Fiksirane odluke (V4 §6)

- **DX verzija:** `DevExpress WinForms Controls v25.2.6` (Licensed) — već referencirana u csproj-u (C17).
- **Default skin:** `Office 2019 Colorful` — `UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful)`.
- **Lokacija init poziva:** `zVvForm\VvForm.cs` `static void Main()` (NE `Program.cs` — ne postoji).
- **TreeView_Modul → TreeList** (Faza 2h, ne C18).

### C18 checklist

- [ ] Otvoriti `zVvForm\VvForm.cs` i locirati `static void Main(string[] args)`.
- [ ] Dodati skin init **iza** `Application.SetCompatibleTextRenderingDefault(false)`,
  **prije** `Application.Run(new VvForm())`. Razlog: skin treba biti postavljen prije
  prve DX kontrole (kojih u Fazi 2 odmah još nema, ali disciplina za buduće faze).
- [ ] Conditional: ako `VvEnvironmentDescriptor` ima saved skin preference (Faza 2i
  proširenje), koristiti tu; inače default `Office2019Colorful`. **U C18 zasad samo
  default** — saved-preference detekcija dolazi u C2X kad se VvColors konvertira.
- [ ] `using DevExpress.LookAndFeel;` + `using DevExpress.Skins;` na vrh fajla.
- [ ] **NIKAKVA druga izmjena** — ne diramo Crownwood, ne diramo VvForm konstruktor,
  ne diramo InitializeVvForm.
- [ ] **Clean-then-build (per pravilo #8)** — verificirati EXIT 0 prije commita.
- [ ] Smoketest: aplikacija se pokrene; budući da nema ijedne DX kontrole instancirane,
  vizualno **ništa** ne smije biti drugačije. Skin engine se učitava ali nema na čemu
  raditi (Crownwood forme/controli ignoriraju DX skin).
- [ ] Append C18 row u V4 §6 tracker.

### Potencijalni rizici za C18

- **DX runtime initialization side-effects.** `SetSkinStyle` ne smije zatražiti licencu
  niti inicijalizirati XPF/Workspace ako se samo poziva čisti method. Validirati da
  app start time nije značajno usporen.
- **Static field initialization order.** `VvForm` ima statičke field initialise-ove
  (singleton, ZXC.TheVvForm setting). DX skin treba biti postavljen **prije** ijedne
  Form instance kreacije. Trenutni `Main()` već dobro slijedi taj pattern (`Application.Run`
  je posljednja linija) — naša izmjena ide samo iznad njega.
- **`VvApplication_ThreadException` nije dirana** — DX licensing greške bi se trebale
  pojaviti kao DialogResult, ne kao thread exception. Ako greška ne dođe u smoketestu, OK.

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
Faza 2 (SWAP)         ⏳ U TIJEKU  (C17 done — references; C18 next — skin init)
  ├── 2a NuGet setup  ✅ C17 — direct DLL references, Opcija B (NE NuGet)
  │                   ⏳ C18 — skin init u VvForm.Main()
  ├── 2b VvForm → XtraForm
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
„P2a / C18" i imaš sve što trebaš.
