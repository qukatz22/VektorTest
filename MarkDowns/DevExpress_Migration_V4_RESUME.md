# DevExpress Migracija — RESUME / HANDOFF

> **Svrha:** Brza orijentacija pri nastavku rada nakon pauze. Autoritativni plan je
> `DevExpress_Migration_V4.md`. Ovaj dokument je samo **bookmark** — gdje smo stali,
> koji je sljedeći commit, što treba prvo pročitati.

---

## 📍 Gdje smo stali (stanje na dan pauze)

**Trenutni branch:** `DevEx-JamesBond` (remote `origin: qukatz22/VektorTest`)

**Zadnji završeni korak:** **Faza 1f / C16** — Audit report (discovery-only,
zero code change).

**Sljedeći korak:** **Faza 2a / C17** — DevExpress infrastruktura setup (prvi
stvarni dodir DX-a u solution).

**Status Faze 1 (Decoupling):** ✅ **POTPUNO ZAVRŠENA** (sve pod-faze 1a→1f kroz
commite C1–C16).

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

**Princip disciplinea kroz cijelu Fazu 1:** atomic commiti, fallback-safe delegation
(svaki call-site zadržava staru putanju ako delegat nije postavljen), zero behavioral
change, build green nakon svakog commita.

---

## ▶️ Sljedeći korak: P2a / C17 — DevExpress NuGet setup

Per V4 §3.2a — **infrastrukturna priprema** (nijedan Crownwood dodir još se ne
mijenja).

### Fiksirane odluke (V4 §6)

- **DX verzija:** `DevExpress WinForms Controls v25.2.6` (Licensed) — NE „najnoviji
  23.x LTS" kako je V4 originalno sugerirao.
- **Default skin:** `Office 2019 Colorful` — `UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful)`
  u `Program.cs Main()`, samo ako user nema saved preference.
- **TreeView_Modul → TreeList** (odluka za Fazu 2h, ne C17).

### C17 checklist

- [ ] Dodati NuGet pakete verzije `25.2.6` u projekt(e) koji će hostati DX UI:
  - `DevExpress.Win.Design`
  - `DevExpress.Win.Navigation`
  - `DevExpress.Win.TreeList`
  - `DevExpress.Data`
  - `DevExpress.Utils`
  - `DevExpress.Images`
  - `DevExpress.XtraEditors`
  - `DevExpress.XtraBars`
  - `DevExpress.XtraTab`
  - `DevExpress.XtraBars.Docking2010`
- [ ] Dodati `licenses.licx` sa svim korištenim DX komponentama (ili potvrditi
  da postoji i proširiti ako treba).
- [ ] `Program.cs Main()` — dodati skin init **prije** `Application.Run(…)`, ali
  iza eventualnog `VvEnvironmentDescriptor` loada (saved preference pobjeđuje default).
- [ ] **NIKAKVA druga izmjena.** Crownwood reference ostaju netaknute.
- [ ] Build solucije — validacija da DX reference koegzistiraju s Crownwoodom bez
  konflikta (namespace, version, licensing).
- [ ] Smoketest: aplikacija se pokrene kao prije; skin inicijalizacija se odvija
  ali vizualno ništa ne smije biti promijenjeno (Crownwood forme/controli ignoriraju
  DX skin).
- [ ] Append C17 row u `DevExpress_Migration_V4.md` §6 tracker (nakon C16).

### Potencijalni rizici za C17

- **Licenca:** DX 25.2.6 zahtijeva aktivnu licencu na build mašini. Ako buildmachine
  nema DX instaliran, NuGet paketi će se instalirati ali `licenses.licx` može javljati
  greške. Validirati prije push-a.
- **Namespace clash:** `DevExpress.XtraEditors.XtraForm` vs `Crownwood.DotNetMagic.Forms.DotNetMagicForm`
  — samo ako se istovremeno using-aju u istom fajlu; u C17 ih ne diramo zajedno.
- **Version pinning:** fiksirati točno `25.2.6` (ne `[25.2.*,)`) da se izbjegne
  slučajni bump tijekom restore-a.
- **Restore time:** prvi `nuget restore` s DX paketima može biti znatno sporiji;
  očekivano.

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
Faza 2 (SWAP)         ⏳ POČINJE   (C17 = 2a infrastruktura)
  ├── 2a NuGet setup  ← SLJEDEĆE
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
„P2a / C17" i imaš sve što trebaš.
