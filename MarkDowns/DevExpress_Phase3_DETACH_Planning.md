# DevExpress Migration — Phase 3 DETACH Planning

## 1. Status i preduvjeti

Ovaj dokument planira Fazu 3: FLOATING-DETACH. Ne uvodi runtime promjene sam po sebi.

V4 §0 obvezuje dvije stvari:

- Faze 1–2 su SWAP faza: Crownwood -> DevExpress uz isto ponašanje, bez novih featurea.
- Faza 3 je DETACH faza: nova funkcionalnost, počinje tek nakon stabilne i ekstenzivno testirane SWAP faze.

Preduvjet za početak implementacije:

1. Faza 2 je runtime potvrđena.
2. Full regression checklist iz V4 §2k je odrađen.
3. Screenshot usporedbe su prihvatljive.
4. Tim prihvaća da su preostali bugovi u Fazi 2 swap-regresije, a ne dio detach implementacije.

## 2. Cilj Faze 3

Korisnik povuče tab iz glavne forme. Umjesto default DevExpress lightweight floating windowa, aplikacija kreira pravu top-level formu:

- `VvFloatingForm : XtraForm, IVvDocumentHost`
- vlastita taskbar ikona,
- vlastiti `BarManager`, menu i toolbarovi,
- vlastiti status bar,
- vlastiti per-host state,
- isti `VvTabPage`/`VvUserControl` nastavlja živjeti, bez reloadanja podataka,
- zatvaranje detached forme vraća sadržaj natrag u glavnu formu.

## 3. Ne-ciljevi prve iteracije

Ne raditi u prvoj iteraciji:

- trajnu perzistenciju pozicija detached prozora,
- novi MDI model,
- reimplementaciju cijelog tab sustava,
- connection-per-call refactor,
- per-host DB connection pool ako lock-based rješenje prođe,
- promjene business workflowa,
- promjene report sadržaja,
- nove module ili featuree nevezane uz detach.

## 4. Postojeća infrastruktura na koju se oslanjamo

| Komponenta | Status | Uloga u Fazi 3 |
|---|---|---|
| `DocumentManager` + `TabbedView` | Aktivno u Fazi 2 | Izvor detach gesturea i source documenta. |
| `VvTabPage` | UserControl hosted kao Document.Control | Prenosi lifecycle, WriteMode, UC i compatibility shimove. |
| `IVvDocumentHost` | Postoji | Kontrakt koji moraju implementirati `VvForm` i `VvFloatingForm`. |
| `ZXC.RegisterDocumentHost` / `UnregisterDocumentHost` / `SetActiveDocumentHost` | Postoji | Registry više UI hostova. |
| `ZXC.ActivePerHostState` | Postoji dormant | Ulaz za per-host flag migraciju. |
| `VvPerHostState` | Postoji | Per-host bucket za flagove u Fazi 3. |
| `VvToolbarFactory` | Postoji | Kreiranje i upravljanje menu/toolbar stateom. |
| `DxBarManager` / `DxBarItemsByName` | Postoji na hostu | Neovisni toolbar/menu item state po formi. |
| `VvForm.ApplyDxSkin` | Postoji | Skin treba primijeniti i na floating formu. |

## 5. Arhitektura ciljnog rješenja

### 5.1 Tipovi

#### `VvFloatingForm`

Klasa:

- base: `DevExpress.XtraEditors.XtraForm`
- interfaces: `IVvDocumentHost`
- P3-2 skeleton postoji: `zVvForm\VvFloatingForm.cs`
- trenutno vlasništvo:
  - vlastiti `BarManager`,
  - vlastiti status strip/status label,
  - vlastiti `VvPerHostState`,
  - registracija u `ZXC.RegisterDocumentHost`,
  - aktivacija kroz `ZXC.SetActiveDocumentHost`,
  - unregister na `FormClosed`.
- planirano vlasništvo u sljedećim sliceovima:
  - source `VvTabPage`,
  - source `Document`,
  - hosted `VvUserControl`,
  - vlastiti `Bar_Record`, `Bar_Report`, `DxMenuBar`,
  - detach/reattach context.

Napomena P3-2: skeleton još ne reparenta content i ne zove `VvToolbarFactory.ApplyWriteMode`, jer factory trenutno podržava samo `VvForm` path za stvarnu WriteMode logiku.

Napomena P3-3: `BeginFloating` sada otvara top-level `VvFloatingForm` preview s naslovom source taba i i dalje cancelira default DX lightweight floating. Content se još ne reparenta; ovo je dokaz da gesture može kreirati pravi host/form lifecycle bez diranja aktivnog taba.

Napomena P3-4: dodan je `VvDetachedDocumentContext` i minimalni reparent flow. `VvFloatingForm` preuzima postojeći `VvUserControl` iz `VvTabPage.panelZaUC`, a na zatvaranje ga vraća u isti source tab container. DX `Document` se u ovom sliceu još ne uklanja iz glavnog `TabbedView` modela, dirty-close semantika još ostaje za sljedeći slice.

Napomena P3-5: dodan je `VvTabPage.IsDetached` lifecycle guard. Duplicate detach istog taba se ignorira, context konstruktor dodatno blokira već-detached tab, a reattach resetira marker, aktivira source tab i vraća `ZXC.ActiveDocumentHost` na source `VvForm`.

Napomena P3-6: detached `FormClosing` sada privremeno vraća content u source tab i koristi postojeći `HasTheTabPageAnyUnsavedData` dirty prompt te arhiva-blokadu. Ako korisnik odustane, close se cancelira, content se vraća u floating formu i host ostaje detached.

Napomena P3-7: `ZXC.SetStatusText` i `ZXC.ClearStatusText` sada prvo rutiraju kroz `ZXC.ActiveDocumentHost as IVvDocumentHost`. Detached forma zato dobiva vlastite status poruke dok je aktivni host; ako host nije dostupan, ostaje stari delegate/fallback put na main `VvForm`.

Napomena P3-8: detached content sada rekurzivno veže `Enter`, `GotFocus` i `MouseDown` na active-host routing, pa interakcija unutar floating forme vraća `ZXC.ActiveDocumentHost` na `VvFloatingForm`. `VvForm.OnActivated` vraća active host na main formu kad korisnik aktivira glavni prozor.

Napomena P3-9: `IVvDocumentHost` DX bar placeholderi su settable, a `VvToolbarFactory` sada može kreirati skeleton `DxMenuBar`, `DxBar_Record` i `DxBar_Report` za bilo koji host. `VvFloatingForm` inicijalizira vlastiti `BarManager` i vlastite skeleton barove; business toolbar/menu item population ostaje za sljedeći slice.

Napomena P3-10: detached skeleton barovi sada imaju samo sigurne iteme: `Zatvori detached tab` koji zove `Close()` i disabled title indicator. Business akcije (SAV/ESC/PRN/etc.) još se namjerno ne kopiraju dok se ne razriješe legacy handleri koji ovise o main `VvForm` stateu.

Napomena P3-11: dodan je centralni `VvDetachedDocumentContext.CanDetach` eligibility guard. Detach se blokira za null tab, već-detached tab, arhiva tab, tab bez `VvUserControl` i UC koji se dispose-a; `BeginFloating` logira razlog i ne pokušava reparent.

Napomena P3-12: baseline P3-1..P3-11 je dokumentiran u `DevExpress_Phase3_Readiness_Gate.md` s manual smoke checklistom i poznatim granicama prije business-toolbar, DB-lock i per-host flag sliceova.

Napomena P3-13: main `VvForm` close sada prije standardnog open-tab dirty loopa zatvara sve registrirane `VvFloatingForm` hostove. Svaki floating close koristi postojeći reattach/dirty-cancel flow; ako neki detached close bude canceliran, cancelira se i main close.

Napomena P3-14: uvedeni su `ZXC.UseSecondDbConnection(...)` i `ZXC.UseThirdDbConnection(...)` kao centralni lock helperi za V4 §3d lock-based serializaciju. Inventar accessora: `TheSecondDbConn_SameDB`, `TheSecondDbConn_SameDB_prevYear`, `TheSecondDbConn_SameDB_OtherYear(int)`, `TheSecondDbConn_OtherDB(string)`, `TheThirdDbConn_SameDB`, `TheThirdDbConn_OtherDB(string)`. Sljedeći reževi trebaju migrirati call-siteove na helper tako da `ChangeDatabase()+query` budu unutar istog locka.

Napomena P3-15: prvi konkretni call-siteovi migrirani su na `UseSecondDbConnection`: HNB/DevTec import + reload (`DevTecDao`), Rtrans previous-year loadovi (`RtransDao`) i SKY write (`FakturDao`). Ciljano je da accessor `ChangeDatabase()` i neposredni query/write ostanu u istom critical sectionu.

Napomena P3-16: svi pronađeni direct `TheThirdDbConn_SameDB` call-siteovi migrirani su na `UseThirdDbConnection` (`ArtStat`, `ArtiklDao`). Dodatno su high-risk `SendWriteOperationToSKY` second-DB write pathovi u `VvForm_Q` i `SubModulActions` migrirani na `UseSecondDbConnection`.

Napomena P3-17: migriran je batch previous-year read pathova na `UseSecondDbConnection`: `Reports_RIZ` selected report reads, eRačun previous-faktur lookup (`UBL_eRacun_Q`, `F2eRacuni`) i `PTG_OtplatniPlan` previous-year rtrans lookup. Preostali direct usage-i trebaju novi scan nakon builda, ali glavni report/e-invoice batch je lock-serializiran.

Napomena P3-18: završni second-DB scan nakon P3-17 zatvorio je dodatne aktivne call-siteove na `UseSecondDbConnection`: preostale `Reports_RIZ` previous-year read pathove (`GetRtransList`, `GetFakturList`, `GetRtransWithArtstatList`, BBB prior-period list), `ArtiklDao.GetPrNabCijFrom_One_LinkedIzlazRtrans`, `RtransDao` oldvej previous-year block, SKY RWT fallback u `VvDaoBase`, `VvDaoBase` next-year sifrar sync/check pathove i `SubModulActions` next-year/prev-year storno/popratnica pathove. Nakon build-green scan-a preostali `TheSecondDbConn_*` hitovi izvan `ZXC` su samo komentari ili lambda factory reference unutar `UseSecondDbConnection` wrappera; manual concurrency smoke test ostaje otvoren.

Napomena P3-19: prvi minimalni per-host flag slice preselio je cache-suppression par `RISK_CheckPrNabDokCij_inProgress` / `RISK_CheckZPCkol_inProgress` iza postojećih `ZXC` property imena, ali vrijednosti se sada čitaju/pišu iz `ZXC.ActivePerHostState` kad postoji aktivni `IVvDocumentHost`. `ShouldSupressRenewCache` sada zbraja active-host state + legacy fallback + global `RISK_DisableCacheTemporarily`, čime se ispunjava prvi dio V4 §3e bez diranja kompleksnih save/copy workflow flagova.

Napomena P3-20: dodani su mali private helperi u `ZXC` za per-host bool routing (`GetActivePerHostFlag`, `GetActiveOrFallbackPerHostFlag`, `SetActivePerHostFlag`) i P3-19 propertyji su refaktorirani na njih. Ovo je infrastrukturni, build-green refactor bez promjene javnih imena i bez dodatne business semantike; cilj je da sljedeći save/copy flagovi imaju manji i konzistentniji diff.

Napomena P3-21: record-level save flag `RISK_SaveVvDataRecord_inProgress` sada je migriran istim helper patternom na active-host `VvPerHostState` uz legacy fallback backing field. Time se prvi record-level flag ponaša per-host bez izmjene postojećih FakturDUC/VvForm_Q call-siteova; manual test “tab A save u toku, tab B drugi save” ostaje otvoren dok se ne migriraju i preostali save/copy companion flagovi.

Napomena P3-22: record-level reentrancy guard `GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress` sada je migriran na active-host `VvPerHostState` uz legacy fallback. Call-siteovi u `VvUserControlRecord_Sub` ostaju na istom `ZXC` imenu, ali guard više nije globalan kada postoji aktivni document host.

Napomena P3-23: preostali record-level companion flagovi `RISK_FinalRn_inProgress`, `RISK_Edit_RtranoOnly_InProgress` i `DupCopyMenu_inProgress` sada su migrirani istim `ZXC` helper patternom na active-host `VvPerHostState` uz legacy fallback. Scan nakon patcha potvrđuje da live call-siteovi u `SubModulActions`, `VvForm_Q`, `FakturDUC` i `ArtiklUC` ostaju na istim `ZXC` property imenima, a host-specific routing se događa centralno u `ZXC`.

Napomena P3-24: cross-DUC copy flagovi `RISK_CopyToOtherDUC_inProgress`, `RISK_CopyToMixerDUC_inProgress` i `MIXER_CopyToOtherDUC_inProgress` sada su migrirani istim `ZXC` helper patternom na active-host `VvPerHostState` uz legacy fallback. Time copy/link workflow guardovi više nisu globalni kada postoji aktivni document host, dok postojeći call-siteovi i dalje ostaju na `ZXC` property imenima.

Napomena P3-25: UI-state flagovi `RESET_InitialLayout_InProgress`, `MenuReset_SvDUH_ZAHonly_InProgress`, `PutRiskFilterFieldsInProgress`, `DumpChosenOtsList_OnNalogDUC_InProgress` i `LoadIzvodDLG_isON` sada su migrirani istim `ZXC` helper patternom na active-host `VvPerHostState` uz legacy fallback. Scan nakon patcha potvrđuje da live call-siteovi ostaju na istim `ZXC` property imenima, a host-specific routing se događa centralno u `ZXC`.

Napomena P3-26: preostali RISK-field companion flagovi `RISK_ToggleKnDeviza_InProgress`, `RISK_InitZPCvalues_InProgress`, `RISK_PULXPIZX_Calc_InProgress`, `RISK_PromjenaNacPlac_inProgress` i `RISK_AutoAddInventuraDiff_inProgress` sada su migrirani istim `ZXC` helper patternom na active-host `VvPerHostState` uz legacy fallback. Time je kompletiran trenutni `VvPerHostState` flag routing set; stara `ZXC` imena ostaju kompatibilna property fasada za postojeće call-siteove.

Napomena P3-27: dodan je process-level M2PAY transaction guard (`ZXC.TryBeginM2PayTransaction` / `EndM2PayTransaction`) jer je M2PAY fizički hardware singleton. `Sale`, `Refund` i `SaleReversal` entry pointovi u `VvForm_Q` sada provjeravaju API/device preduvjete i zatim uzimaju globalni guard dok traje HAPI transaction start + modalni status dialog; drugi host dobiva hrvatsku UX poruku da je transakcija već u tijeku u drugom prozoru.

Napomena P3-28: detached forma sada dobiva V4-style title bar tekst `Vektor — {ModulName} / {SubModulName} — {WriteMode}` iz source `VvTabPage`/`VvSubModul` podataka i preuzima ikonu sa source/main forme, tako da se u taskbaru razlikuje kao pravi Vektor top-level prozor. Business toolbar populacija ostaje odvojena otvorena stavka.

Napomena P3-29: dodan je detached `Bar_SubModul` skeleton kroz `IVvDocumentHost.DxBar_SubModul` i `VvToolbarFactory.CreateBar_SubModul`. `VvFloatingForm` sada ima vlastiti DX SubModul bar s disabled placeholder itemom; business item population namjerno ostaje otvorena odvojena stavka.

Napomena P3-30: `VvToolbarFactory.ApplyWriteMode` više ne baca `NotImplementedException` za detached `IVvDocumentHost`; dodan je minimalni DX skeleton path koji enable-a Record/SubModul barove i skriva Report bar u write-mode scenarijima. `VvFloatingForm` sada primjenjuje `SourceTabPage.WriteMode` nakon kreiranja detached barova; puna business enable/disable logika ostaje otvorena za business item population slice.

Napomena P3-31: status hint push/pop putanja (`ZXC.StatusTextPusher` / `StatusTextPopper`) sada ruti kroz `ZXC.PushStatusText` / `PopStatusText`, koji koriste `ActiveDocumentHost` i per-host `VvPerHostState.StatusTextBackup`. Time grid `CellEnter/CellLeave` i `VvTextBox` hintovi više ne zatvaraju glavnu `VvForm` status traku kada je aktivan detached host; ručni smoke test s dva prozora ostaje za korisničku validaciju.

Napomena P3-32: `VvDetachedDocumentContext` sada eksplicitno snima `WriteModeAtDetach` i `IsArhivaAtDetach`. Detached forma koristi taj snapshot za inicijalni `ApplyWriteMode` i title tekst, a `CanDetach` više ne blokira tabove u Arhivi jer V4 §1.16/§3h zahtijeva Opciju B: arhiva putuje s tabom. Zatvaranje detached forme i dalje provjerava archive-exit guard prije reattacha.

Napomena P3-33: `VvFloatingForm` sada zatvara detached sadržaj preko validiranog `TryReattachContentCore` toka. Ako source forma/tab/panel ili hosted UC više nisu dostupni, recovery putanja skida active-host evente, spušta `IsDetached` gdje je moguće i kontrolirano dispose-a hosted UC uz hrvatsku warning poruku umjesto da close path ostavi orphan control ili sruši proces.

Minimalne odgovornosti:

1. Preuzeti tab content iz main `TabbedView` documenta.
2. Izgraditi vlastite menu/toolbare preko `VvToolbarFactory`.
3. Registrirati se kao document host.
4. Rutirati status text u vlastiti status bar.
5. Na zatvaranje reattachati content u originalni `VvTabPage`.
6. Unregister host.

#### `VvDetachedDocumentContext`

Preporuka: mali internal/helper model za držanje detach statea.

Polja:

- `VvForm SourceForm`
- `VvTabPage SourceTabPage`
- `Document SourceDocument`
- `VvUserControl HostedUserControl`
- `ZXC.WriteMode WriteModeAtDetach`
- toolbar snapshot state ako već postoji kao strukturirani object

Ako postojeći toolbar snapshot nije enkapsuliran, u prvom sliceu samo dokumentirati/izdvojiti minimalni helper.

### 5.2 Ownership pravilo

Jedan `VvUserControl` smije biti child samo jednog container-a u jednom trenutku:

- attached: `sourceTabPage.Controls` sadrži UC,
- detached: `floatingForm.Controls` sadrži UC,
- reattach: UC se vraća u `sourceTabPage.Controls`.

Nema kloniranja UC-a i nema reloadanja podataka u osnovnoj detach operaciji.

## 6. Detach flow

Planirani flow:

1. User povuče tab van `TabbedView` granice.
2. `TabbedView.DocumentFloating` event se aktivira.
3. Handler postavlja `e.Cancel = true`.
4. Aplikacija kreira `VvFloatingForm` s contextom source documenta.
5. `VvFloatingForm`:
   - sprema toolbar/status snapshot source taba,
   - uklanja UC iz `VvTabPage.Controls`,
   - dodaje UC u `VvFloatingForm.Controls`,
   - postavlja `UC.DocumentHost = this` ako takav property/call-site već postoji,
   - čuva `UC.TheVvTabPage = sourceTabPage` ili ekvivalentni existing bridge,
   - gradi menu/toolbare,
   - primjenjuje `ApplyWriteMode(this, sourceTabPage.WriteMode)`,
   - registrira host u `ZXC.RegisterDocumentHost(this)`,
   - postavlja active host na sebe,
   - prikazuje formu.

Napomena: ne brisati source `VvTabPage` model ako treba služiti kao attach anchor. Ako DevExpress document mora biti privremeno uklonjen iz main `TabbedView`, to napraviti centralno i reverzibilno.

## 7. Reattach flow

Planirani flow na `VvFloatingForm.FormClosing`:

1. Ako UC ima dirty state, proći isti dirty prompt kao close tab.
2. Ako korisnik odustane, cancel closing.
3. Inače:
   - ukloniti UC iz `VvFloatingForm.Controls`,
   - vratiti UC u `sourceTabPage.Controls`,
   - vratiti document u main `TabbedView` ako je bio uklonjen,
   - vratiti `UC.DocumentHost = ZXC.TheVvForm` ili existing equivalent,
   - vratiti toolbar snapshot kao aktivaciju taba,
   - `ZXC.UnregisterDocumentHost(this)`,
   - aktivirati source tab/document u glavnoj formi,
   - dispose floating form.

## 8. DB concurrency odluka

V4 §1.15 navodi tri opcije. Za prvu iteraciju koristiti preporuku V4 §3d:

**Lock-based serialization.**

Razlog:

- najmanje invazivno,
- najbrže za stabilnu prvu detach iteraciju,
- pool/per-call se mogu uvesti kasnije ako mjerenje pokaže problem.

Plan:

1. Inventarizirati sve accessore za `TheSecondDbConn_*` i `TheThirdDbConn_*`.
2. Za svaki path gdje se radi `ChangeDatabase()` pa query, omotati kritičnu sekciju u lock.
3. Ne zaključavati preširoko UI kod.
4. Testirati dva detached/main prozora s različitim year/db kontekstima.

## 9. Per-host flagovi

Koristiti V4 §1.14 klasifikaciju.

### 9.1 Global ostaje global

Primjeri:

- session-once cache checks,
- long-running process mutex operacije,
- import/export file I/O,
- Sky sync,
- M2PAY physical singleton state.

### 9.2 Per-document-host seli u `VvPerHostState`

Prioritetno:

- record-level save/edit flags,
- cross-DUC copy flags,
- UI state flags,
- RISK field operation flags.

Prvi implementation slice ne mora preseliti svih ~15 flagova ako detach nije još omogućen za te workflowe, ali mora imati listu i testove za svaku migraciju.

## 10. M2PAY guard

M2PAY je fizički singleton. Detached form ne smije omogućiti dvije paralelne hardverske transakcije.

Plan:

- process-level guard/mutex oko aktivne M2PAY transakcije,
- user-facing poruka ako drugi host pokuša plaćanje,
- poruka treba navesti koji prozor/host drži transakciju ako to možemo pouzdano znati.

## 11. Status bar routing

Cilj: status text ide u aktivni host, ne uvijek u glavnu formu.

Plan:

1. Revidirati `ZXC.SetStatusText` / `ClearStatusText` sink.
2. Ako postoji `ActiveDocumentHost`, rutirati u `host.SetStatusText(...)`.
3. Ako nema hosta, fallback na postojeći sink.
4. `VvFloatingForm` implementira vlastiti status label.

Test:

- grid CellEnter u glavnoj formi piše u main status bar,
- grid CellEnter u detached formi piše u detached status bar,
- switch focus mijenja active host.

## 12. Shortcut i BarManager fokus

Svaki host ima vlastiti `BarManager`.

Rizici:

- isti shortcut registriran u main i detached formi,
- event handler pogodi krivi UC jer koristi `ZXC.TheVvForm`,
- menu item enabled state ostane iz drugog hosta.

Plan:

- `VvFloatingForm` gradi vlastite bar iteme,
- handleri moraju raditi preko host/contexta gdje je moguće,
- za legacy handler koji još koristi `ZXC.TheVvForm` otvoriti poseban bug/slice.

## 13. Edge cases

| Edge case | Očekivanje |
|---|---|
| Arhiva mode | Detach dozvoljen; arhiva putuje s tabom. |
| Dirty tab detach | Detach ne smije izgubiti dirty state. |
| Dirty detached close | Koristiti isti dirty prompt kao tab close. |
| Main form close s detached formama | Prompt: zatvoriti/reattachati detached prozore prije izlaza. |
| Crash u detached formi | Preferirati graceful reattach ako je moguće, inače controlled dispose. |
| Crystal Reports BackgroundWorker | Mora pripadati hostu/prozoru koji ga je pokrenuo. |
| Sifrar cache refresh | Global ili host-aware prema §1.14 klasifikaciji. |
| Rtrans screen read | Mora gađati vlastiti FakturDUC, ne `ZXC.TheVvForm` ako je detached. |
| Memory leak | 100x detach/reattach ciklus bez rasta živih formi/UC-a. |

## 14. Predloženi implementation slices

### P3-0 — Readiness gate

- potvrditi Faza 2 regression i screenshot stavke,
- commitati tracker update da SWAP faza ima human green,
- bez code promjene za detach.

### P3-1 — Event interception spike

- [x] Hook `TabbedView.BeginFloating` (`DocumentCancelEventHandler`).
- [x] `DocumentProperties.AllowFloat = true` za gesture.
- [x] `e.Cancel = true` za sprječavanje default DX lightweight floatinga.
- [x] Logirati source `Document`/`VvTabPage` context preko `Debug.WriteLine`.
- [x] Ne kreirati floating form još.
- [x] VS-build green.

**Rezultat P3-1:** DevExpress v25.2 `TabbedView` nema event imena `DocumentFloating`; metadata scan pokazuje `BeginFloating`, `Floating`, `EndFloating`. Za interception koristimo `BeginFloating` jer event args imaju `Document` i writable `Cancel`.

### P3-2 — `VvFloatingForm` skeleton

- nova klasa `VvFloatingForm : XtraForm, IVvDocumentHost`,
- minimalni constructor prima context,
- vlastiti status bar,
- registry/unregistry host,
- bez reparenta UC-a.

### P3-3 — Toolbar/menu skeleton

- `VvFloatingForm` dobiva vlastiti `BarManager`,
- koristi `VvToolbarFactory.CreateMenuBar(..., isDetached: true)`,
- kreira `Bar_Record`, `Bar_Report`, osnovni menu,
- `ApplyWriteMode(this, writeMode)` radi ili eksplicitno dokumentira unsupported paths.

### P3-4 — Reparent UC detach

- stvarni reparent UC-a iz `VvTabPage` u `VvFloatingForm`,
- preserve `VvTabPage` model,
- preserve dirty/write state,
- no data reload.

### P3-5 — Reattach on close

- `FormClosing` vraća UC u source tab,
- vraća document/activaciju,
- dirty prompt/cancel radi.

### P3-6 — Status routing

- active host focus tracking,
- status text ide u pravi host.

### P3-7 — Per-host flags batch 1

- preseliti najkritičnije record-level flags,
- testirati save/edit u main + detached.

### P3-8 — DB lock serialization

- implementirati lock oko `TheSecondDbConn_*` / `TheThirdDbConn_*` ChangeDatabase+query critical sections,
- test concurrent year/db access.

### P3-9 — Edge-case hardening

- main form close with detached forms,
- Crystal Reports detached context,
- shortcuts/focus,
- M2PAY guard.

### P3-10 — UX polish

- title format: `Vektor — {ModulName}/{SubModulName} — {WriteMode}`,
- taskbar icon,
- optional reattach gesture ako izvedivo bez većeg rizika.

### P3-11 — Stress and leak validation

- 100x detach/reattach,
- memory/process handle observation,
- DB concurrency stress,
- report worker stress.

## 15. Validation matrix

| Područje | Test |
|---|---|
| Detach gesture | Drag tab van forme kreira top-level formu. |
| No default float | DevExpress lightweight float se ne pojavljuje. |
| Reattach | Close detached vraća tab u main form. |
| Data state | Record selection, dirty state i filters ostaju isti. |
| WriteMode | Main i detached mogu imati različit WriteMode bez curenja statea. |
| Toolbar state | Save/Delete/Print enabled state ispravan po hostu. |
| Status bar | Status text ide u prozor koji ima focus. |
| Tree navigation | Main tree i detached tab state ne konfliktiraju. |
| Dirty prompt | Close detached s dirty data može se cancelati. |
| Arhiva | Arhiva detached dozvoljena, ali pravila arhive ostaju. |
| DB concurrency | Dva prozora s različitim year/db pozivima bez racea. |
| M2PAY | Samo jedna transakcija u procesu. |
| Crystal Reports | Report preview/export radi u detached hostu. |
| Main close | Aplikacija zna zatvoriti/reattachati sve detached forme. |
| Leak | 100x detach/reattach bez očitog memory/control leak-a. |

## 16. Stop conditions

Zaustaviti implementaciju i otvoriti odluku ako:

- detach zahtijeva promjenu poslovnog workflowa,
- `VvUserControl` ne može sigurno promijeniti parent bez reloadanja,
- postojeći event handleri masovno gađaju `ZXC.TheVvForm` i lome detached host,
- lock-based DB pristup uzrokuje neprihvatljiv freeze,
- Crystal Reports lifecycle ne može raditi iz detached forme bez većeg refactora.

## 17. Preporučeni prvi realni korak

Ne početi s kompletnim `VvFloatingForm` odmah.

Prvi realni code slice treba biti P3-1: mali spike koji intercepta `DocumentFloating`, cancelira default DevExpress floating i dokazuje da imamo točan source `Document`/`VvTabPage` context. To je najjeftiniji način da validiramo DevExpress event model prije reparentanja UC-a.

Ako P3-1 prođe, tek tada graditi `VvFloatingForm` skeleton.
