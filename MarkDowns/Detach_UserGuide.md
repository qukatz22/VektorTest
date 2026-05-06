# Vektor — Korisnički vodič za odvajanje (detach) tabova

Vektor podržava rad u više prozora istovremeno. Bilo koji otvoreni tab (modul,
unos, lista, izvještaj) može se "odvojiti" iz glavnog prozora u zasebni prozor
koji se ponaša kao samostalna instanca s vlastitom taskbar ikonom.

## Kako odvojiti tab (detach)

1. Otvorite željeni modul/tab u glavnom prozoru.
2. Mišem **povucite naslov taba (tab header) van granica glavnog prozora**.
3. Otpustite miš — tab se pretvara u samostalan top-level prozor.

Detached prozor:

- ima vlastitu taskbar ikonu i zaseban naslov u formatu
  `Vektor — {Modul}/{SubModul} — {WriteMode}`,
- nasljeđuje WriteMode (Browse/Edit/...) iz trenutka odvajanja,
- ne reload-a podatke — selekcija recorda, dirty stanje i filteri ostaju isti,
- ne prikazuje vlastiti meni/toolbar na vrhu (chrome je minimalan).

## Kako vratiti tab natrag (reattach)

1. Mišem **uhvatite title bar detached prozora**.
2. Povucite ga **preko glavnog Vektor prozora** i otpustite.
3. Tab se vraća kao standardan tab unutar glavnog prozora s istim sadržajem.

## Kako zatvoriti detached prozor bez vraćanja

- Zatvaranje detached prozora (X gumb) **zatvara samo taj document**, ne vraća
  ga u glavnu formu i ne zatvara aplikaciju.
- Ako je u detached prozoru otvoreno nesnimljeno (dirty) stanje, dobit ćete
  standardni dirty prompt prije zatvaranja.

## Što istovremeno radi u više prozora

- Različit WriteMode po prozoru (npr. glavni: Edit, detached: Browse).
- Različita arhiva po prozoru.
- Različite RISK / record-level operacije po prozoru — operacija u jednom
  prozoru ne blokira drugi prozor.
- Crystal Reports izvještaji se izvršavaju neovisno po prozoru.
- DB konekcije za sekundarne i prev-year putanje serijaliziraju se automatski
  (lock-based), pa nema race-eva pri istovremenom čitanju iz različitih godina.

## Što i dalje vrijedi globalno

- M2PAY hardware uređaj — samo jedna transakcija u procesu istovremeno;
  pokretanje plaćanja u drugom prozoru dok je jedno u tijeku prikazuje upozorenje.
- Sky sync (single server endpoint).
- Cache management (refresh sifrara) — ne pokretati paralelno; ako je potrebno,
  pokrenuti iz jednog prozora i pričekati završetak.

## Zatvaranje aplikacije s otvorenim detached prozorima

Pri zatvaranju glavnog prozora (X gumb / Datoteka → Izlaz), Vektor prvo
zatvara sve detached prozore. Ako neki detached prozor sadrži dirty
stanje, prikazat će se prompt; cancel u tom promptu cancelira i zatvaranje
glavnog prozora.

## Skin / izgled

Default skin je `Office 2019 Colorful`. Korisnik može izabrati drugi skin kroz
`Pogled → Stilovi i boje` dijalog. Postavka se sprema u
`VvEnvironmentDescriptor.xml` po korisniku i primjenjuje se i na detached
prozore.
