# Faza 1 — Smoke Test Protocol

> **Svrha:** Nakon **svakog** pod-koraka Faze 1 (1a → 1f) proći ovaj protokol prije
> oznake pod-koraka kao „done" i prije nego se krene na sljedeći.
>
> **Preduvjet:** Build prošao bez errora i warninga u `DevEx-JamesBond` grani, u Debug
> i Release konfiguraciji.
>
> **Cilj:** Faza 1 **ne smije** mijenjati ponašanje aplikacije. Svaki smoke-test scenarij
> koji je radio prije promjene mora raditi identično nakon promjene. Bilo kakva razlika
> = **regresija** = STOP + reci mi što se dogodilo.
>
> **Vrijeme izvođenja cijele checkliste:** ~15–25 min (jednom uvježbano). Prva 2–3 puta
> računaj 40 min dok se ne naviknu koraci.

---

## 0. Pre-flight (svaki put prije testiranja)

- [ ] Branch: `DevEx-JamesBond` (potvrdi s `git branch --show-current`)
- [ ] `git status` — working tree clean ili samo očekivane izmjene pod-koraka
- [ ] Build Debug → 0 errors, 0 new warnings (usporedi s baseline brojkama)
- [ ] Build Release → 0 errors, 0 new warnings
- [ ] Obrisati `bin\` i `obj\` ako se sumnja na stale build (`Clean Solution`)
- [ ] Imati backup `vvektor` i testne PUG baze **iz istog trenutka kao `pre-phase1-baseline`** tag (SQL Server backup ili MySQL dump) — za deterministički reset između pokretanja
- [ ] Aplikaciju pokretati **uvijek iz Visual Studija (F5, Debug)** tijekom Faze 1 — catch-all na `AppDomain.UnhandledException` je lakše hvatati s debuggerom

---

## 1. Login & inicijalizacija (core path ZXC.PrjConnection + VvDB_NameConstructor)

**Why:** Pod-koraci 1a mijenjaju `PrjConnection` i `VvDB_NameConstructor` — ako login ili otvaranje modula ne rade, **sve je razbijeno**. Ovo je najbrži sanity check.

- [ ] Aplikacija se pokreće bez unhandled exceptiona (Output window clean)
- [ ] Login dialog se prikazuje
- [ ] Unijeti validnog test usera + odabrati testni Projekt + testnu godinu (npr. `VIPER1 / 2026`)
- [ ] Login uspješan → glavna `VvForm` se otvara
- [ ] Toolbar + meniji prikazani i enabled/disabled stanje odgovara očekivanju za odabranu ulogu
- [ ] Status bar prikazuje ime usera/projekta (ili što već pokazuje baseline)
- [ ] Title bar forme je ispravan

**Ako ovo ne prolazi:** stop, reci mi output + stack trace.

---

## 2. PrjConnection sanity (vvektor baza — `PrjktUC`, `PrvlgUC`, `UserUC`)

**Why:** `ZXC.PrjConnection` → `vvektor` database. Pod-korak 1a **mora očuvati** putanju
do imena baze. Ovo je jedini siguran test da `ZXC.PrjktDB_Name` property daje isti
rezultat kao prethodni `TheVvForm.GetvvDB_prjktDB_name()` poziv.

### 2.1 `PrjktUC` — ADDREC / RWTREC / DELREC

- [ ] Otvoriti `PrjktUC` (modul **Projekt** → SubModul **Projekti**, ili gdje već živi u navigaciji)
- [ ] **ADDREC:** Novi rekord
  - [ ] „Novi" (NEW) — forma se prebacuje u Edit mode
  - [ ] Unijeti minimalne validne vrijednosti (šifra, naziv — copy-paste iz postojećeg + suffix `_SMK`)
  - [ ] Save (SAV) — bez greške, rekord vidljiv u listi
- [ ] **RWTREC:** Edit postojećeg
  - [ ] Otvoriti taj isti `_SMK` rekord (OPN)
  - [ ] Promijeniti naziv (npr. dodaj `_X`)
  - [ ] Save — bez greške, izmjena perzistirana
- [ ] **DELREC:** Brisanje
  - [ ] Označiti `_SMK` rekord
  - [ ] Delete (DEL), potvrditi
  - [ ] Rekord nestao iz liste, bez greške

### 2.2 `PrvlgUC` (sanity read)

- [ ] Otvoriti `PrvlgUC` — lista privilegija se učitava bez greške
- [ ] Otvoriti jedan rekord, zatvoriti (bez izmjene) — ESC vraća u browse

### 2.3 `UserUC` (sanity read)

- [ ] Otvoriti `UserUC` — lista korisnika se učitava
- [ ] Otvoriti test usera, zatvoriti bez izmjene

**Što ovo dokazuje:** `PrjConnection` getter čita ispravno `PrjktDB_Name` (novo property)
i da DAO sloj (`ADDREC`/`RWTREC`/`DELREC`) i dalje vidi `vvektor` bazu.

---

## 3. TheMainDbConnection sanity (PUG baza — `ArtiklUC`)

**Why:** `ZXC.TheMainDbConnection` ide na PUG (npr. `vv2026_VIPER1_000001`). Ime se
konstruira preko `ZXC.VvDB_NameConstructor` koji čita `ZXC.VvDB_Prefix` (novo standalone
property u 1a). Ako je prefix pogrešan, ili nije postavljen pri loginu, **otvaranje
bilo kojeg PUG modula će puknuti** ili će uzeti krivu bazu.

### 3.1 `ArtiklUC` — edit & save

- [ ] Otvoriti `ArtiklUC` (modul **Šifrarnik** / **Artikli**)
- [ ] Lista artikala se učitava (ne prazna, osim ako je testna baza čista)
- [ ] Odabrati jedan postojeći artikl → OPN
- [ ] Promijeniti jedno benign polje (npr. `Opis` — dodaj suffix ` _SMK`)
- [ ] Save (SAV) — bez greške
- [ ] Zatvoriti tab, reotvoriti `ArtiklUC`, verificirati da je izmjena perzistirana
- [ ] Revert izmjene (obrisati ` _SMK` iz opisa) i save → cleanup stanja

### 3.2 Dodatna PUG verifikacija (jedan od sljedećih — minimum jedan)

- [ ] `KupdobUC` (Kupci/dobavljači) — otvori jedan rekord, zatvori bez izmjene
- [ ] `KplanUC` (Kontni plan) — otvori listu, otvori jedan rekord, zatvori

**Što ovo dokazuje:** `VvDB_Prefix` je ispravno postavljen pri loginu; `VvDB_NameConstructor`
daje točno ime PUG baze; `TheMainDbConnection` radi.

---

## 4. Switch-godina (prev-year / other-year)

**Why:** `ZXC.TheSecondDbConn_SameDB_prevYear` i `SameDB_OtherYear(int)` koriste
`PrjConnection` + custom `VvDB_NameConstructor(year)` logiku. Ako je bilo koja od njih
pobjegla od `PrjktDB_Name`/`VvDB_Prefix` stazi, **prethodna godina se ne otvara**.

- [ ] U modulu koji otvara izvještaj iz prethodne godine (npr. Bilanca — ili što već klijent tipično koristi), pokrenuti operaciju koja čita prev-year
- [ ] Rezultat jednak baseline rezultatu (usporediti key brojke — iznos, broj redaka)

**Ako nemaš scenariji prev-year uz sebe:** preskoči ovaj korak s napomenom u log-u; test ga kasnije u Fazi 1d audit-u.

---

## 5. Rtrans calc path (pod-korak 1d)

**Why:** 1d refactor dira `Rtrans.Get_S_KC_fromScreen()` i `CheckZtrColExists()` koji
rutiraju kroz `DocumentHost`. Sva tri calc grana (MALOP_IZLAZ, MALOP_ULAZ, VELEP) moraju
se ponašati identično.

- [ ] **MALOP_IZLAZ:** Otvoriti postojeći MALOP izlazni Faktur, ući u uređivanje, otvoriti jednu stavku, pokrenuti recalc (obično automatski na promjeni kol/cijene), verificirati da se iznos podudara s baseline vrijednošću
- [ ] **MALOP_ULAZ:** Isto, ali na ulaznom MALOP dokumentu
- [ ] **VELEP:** Isto, na VELEP izlaznom

**Minimum:** po jedan Faktur na svakoj od 3 grane. **Idealno:** po jedan na svakoj od 5 podputanja (IZLAZ, ULAZ, ULAZ_ZPC, ULAZ_ByCIJENA, VELEP_ByMPC) — vidi §1.8 V4.

---

## 6. FIR outbound (eRačun) — full roundtrip

**Why:** Najzapleteniji flow u aplikaciji. Ako ovo prolazi, 90% je sigurno da decoupling
nije pokvario core business.

- [ ] Otvoriti postojeći FIR Faktur u testnom projektu
- [ ] Send → provider (MER ili PND, što je testno podešeno) — odgovor stigne, status ok
- [ ] QueryOutbox TRN — vraća status (FISK/MAP/eIZVJ, whatever je očekivano za test)
- [ ] Status bar prikazuje napredak (validira §1.9 decoupling — status text routing)

**Minimum u Fazi 1:** samo Send + QueryOutbox. Full archive roundtrip tek kad stignemo u Fazu 2.

---

## 7. FUR inbound (eRačun)

- [ ] QueryInbox → lista pristiglih dokumenata
- [ ] Receive XML za jedan (testni) dokument
- [ ] `Xtrano` rekord kreiran, vidljiv u arhivi

---

## 8. Tab switching + WriteMode + status bar (pod-koraci 1b, 1d)

**Why:** `VvToolbarFactory.ApplyWriteMode` (1b) preuzima logiku iz
`SetVvMenuEnabledOrDisabled_RegardingWriteMode_JOB`. Ako factory promaši jedan od 7
specijalnih case-ova (§1.6), tab switching ili WriteMode toggle prikazat će krivo
enable/disable stanje gumba.

- [ ] Otvoriti 3 različita taba različitog tipa: jedan record DUC (npr. `FakturDUC`), jedan report (npr. Bilanca filter), jedan sifrar (`ArtiklUC`)
- [ ] Switch-ati između njih — meni + toolbar gumbi se mijenjaju ispravno
- [ ] U `FakturDUC`: Browse → Edit (OPN postojeći) → Save → Browse — sav flow, svi gumbi enabled/disabled po očekivanju
- [ ] Test za deployment varijante **koje ti vrijede**:
  - [ ] `IsTEXTHOshop`: OPN, DEL moraju biti disabled (osim na `InventuraDUC`/`InventuraMPDUC`)
  - [ ] `IsPCTOGO`: NEW, DUP disabled na relevantnim DUC-ima
  - [ ] `IsSvDUH_ZAHonly`: NEW, OPN, DEL, DUP disabled na `IZD_SVD_DUC`
  - [ ] Ako deployment nije jedan od ovih — samo standard WriteMode flow
- [ ] Status bar grid CellEnter/CellLeave u jednoj grid kontroli (Mixer, Faktur, Placa — što god je bliže ruci): hover preko stavki mijenja status text, leave ga vraća (validacija §1.9 + 1d).

---

## 9. Arhiva mode (VvTabPage_Validating)

- [ ] Otvoriti dokument, prebaciti ga u arhivu (ARH gumb)
- [ ] Pokušati switch na drugi tab → **blokirano** (dialog „izađi iz arhive prvo" ili equivalent)
- [ ] Izaći iz arhive → switch ponovo radi

---

## 10. Report run (Crystal Reports)

- [ ] Pokrenuti jedan standard report (npr. IOS, Kartica kupca, nešto brzo)
- [ ] Report se renderira, preview prikazan, Print/PDF/Export gumbi rade
- [ ] Zatvaranje report taba — bez hanged BackgroundWorkera (provjeri Task Manager — proces broji thread-ove normalno)

---

## 11. Shutdown

- [ ] File → Exit (ili X na formi)
- [ ] `VvEnvironmentDescriptor.xml` se snima bez greške (provjeri timestamp + da nije 0-byte file)
- [ ] Proces se potpuno gasi (ne ostaje u Task Manageru)
- [ ] Ponovno pokretanje aplikacije — učita saved layout, sav user state perzistiran

---

## 12. Negativni / edge scenariji (brzo)

- [ ] Pokušati save rekord s nevalidnim poljem — validacija radi, Croatian error message prikazan
- [ ] ESC u Edit modu — prompt za unsaved changes, odabrati „Ne spremi" → rollback
- [ ] Otvoriti tab, odmah ga zatvoriti (X na tabu) — bez exceptiona

---

## 13. Memory / resource sanity (opcionalno, ali preporučeno nakon 1e)

- [ ] Otvoriti i zatvoriti isti DUC 10× zaredom — memorija u Task Manageru ne raste linearno (GC bi trebao držati steady state ± oscilacija)
- [ ] `ZXC.UnregisterDocumentHost` poziv se okida (breakpoint ili log) kod zatvaranja taba — validacija 1e reset-patha

---

## Log sheet (popuni pri svakom prolasku)

Preporuka: jedan retak u `MarkDowns\Phase1_SmokeTests_Log.md` za svaki prolazak. Format:

    | Datum | Commit SHA | Pod-korak | Prolaz | Napomene |
    |---|---|---|---|---|
    | 2026-04-24 14:30 | abc123f | baseline | OK | referent |
    | 2026-04-25 10:15 | def456a | 1a | OK | - |
    | 2026-04-25 16:40 | 789bcde | 1b | FAIL | §8 WriteMode: IsPCTOGO NEW ostao enabled na `KOP_PTG_DUC` |
    | 2026-04-26 09:05 | 012aabc | 1b-fix | OK | popravljen ApplyWriteMode case-KOP_PTG |

**Ovaj log je TVOJ alat, ne moj** — ali iznimno koristan kad tjedan dana nakon nečega
trebaš rekonstruirati što je točno puklo gdje. Ako stigne regresija, prvi je hit git
bisect od zadnjeg OK retka.

---

## Pravilo STOP

Ako **bilo koji** korak u §1–§9 pukne, **ne ide se dalje na sljedeći pod-korak Faze 1**
dok se regresija ne identificira i popravi. §10–§13 su „nice to have" u Fazi 1 — ako
padne nešto tamo, može se eskalirati kao TODO za Fazu 2, ali ne smije se ignorirati.

---

## Što dodati kasnije (post-Faza 1)

Ovo je **smoke** test — minimum viable protection. U pripremi za Fazu 2 treba napraviti
proširenje (regression harness) s:

- Skripta za automatsko kreiranje test Faktura iz XML-a (deterministička data)
- Snapshot comparison (screenshot diff za VvForm layout)
- Automatski run iz CI-a prije merge u main/produkciju

To je tema zasebnog request-a kad Faza 1 završi.