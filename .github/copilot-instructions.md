# Vektor Project - Copilot Instructions

## Project Overview
Croatian accounting/ERP desktop application (.NET Framework 4.8, C# 7.3, WinForms).
Implements the Croatian eRačun (e-invoice) ecosystem: MER (moj-eracun.hr) and PND (eposlovanje.hr) providers.

## Architecture
- `Vv_eRacun_HTTP` – static class, all HTTP API calls (MER/PND web services)
- `VvMER_RequestData` / `VvMER_ResponseData` – JSON request/response DTOs
- `WebApiResult<T>` – generic API result wrapper
- `Faktur` – outgoing invoice record (FIR = Fakturni Izlazni Račun)
- `Xtrano` – incoming invoice archive record (FUR = Fakturni Ulazni Račun)
- `Ftrans` – payment transaction record
- `Kupdob` – customer/supplier master record
- `ZXC` – global static context (project year, DB connection, config)
- `VvDaoBase` – generic DAO base class (ADDREC/RWTREC/DELREC pattern)

## Database
- Conditional compile: `#if MICROSOFT` → SqlClient, else MySql.Data.MySqlClient
- DAO pattern: `ADDREC`, `RWTREC`, `DELREC`, `SetMe_Record_byRecID`

## Key Conventions
- Croatian UI strings (error messages in Croatian)
- `NotEmpty()`, `IsZero()`, `NotZero()` are extension methods
- `ZXC.aim_emsg(...)` for user-facing error messages
- `EN16931.UBL.InvoiceType` / `CreditNoteType` for UBL XML (de)serialization
- Newtonsoft.Json for all JSON serialization (`[JsonProperty]` attributes)
- C# 7.3 — no switch expressions, no records, no nullable reference types
- .NET Framework 4.8 — no `HttpClient`, uses `HttpWebRequest`/`HttpWebResponse`

## eRačun Workflow
- FIR (outbound): Send → QueryOutbox TRN → FISK/REJECT/MAP/eIZVJ status → Archive
- FUR (inbound): QueryInbox → Receive XML → Create Xtrano → Import as Faktur
- NIR: Local payment report XML generation (EvidentirajNaplatuZahtjev)
- HDD variants: same flow but reading XML files from local disk instead of web service

## Adding a New API Endpoint (pattern)
1. Add URL constant in `#region MER/PND Web Service URLs`
2. Add a `ZXC.F2_WebApi` enum value
3. Add constructor overload to `VvMER_RequestData` if needed
4. Call `Vv_POSTmethod_ExecuteJson<T>` or `Vv_GETmethod_ExecuteJson<T, TRequest>`
5. Add a `case` to `Show_WebApiResult_ErrorMessageBox` switch

## Naming Conventions
- Methods: `WS_*` = live web service, `HDD_*` = local disk variant
- Load lists: `Load_*`, sync/import: `WS_Import_*` / `HDD_Import_*`
- Status refresh: `WS_Refresh_*`, mark-as-paid: `WS_Discover_Candidates_And_Eventually_MAPaj_*`
- Comments marking major methods: `/* AAA */`, `/* BBB */`, `/* XXX */`, `/* YYY */` etc.

## Key Enum Values
- `ZXC.F2_Provider_enum.MER` / `.PND` — which provider is active
- `ZXC.F2_RolaKind` — project role (FUR_Only, ALL, etc.)
- `ZXC.F2_WebApi` — identifies which API call a `WebApiResult` came from
- `Mixer.TT_AIR` / `TT_AUR` / `TT_MAP` — transaction type codes for Xtrano records

## Do Not
- Do not use `HttpClient` — always use `HttpWebRequest`/`HttpWebResponse`
- Do not use C# 8+ features (switch expressions, records, nullable reference types, `??=`)
- Do not use `System.Text.Json` — always use `Newtonsoft.Json`
- Do not hardcode English UI strings — use Croatian