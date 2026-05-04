# Vektor Project - Copilot Instructions

## Project Overview
Croatian accounting/ERP desktop application (.NET Framework 4.8, C# 7.3, WinForms). Implements the Croatian eRačun (e-invoice) ecosystem: MER (moj-eracun.hr) and PND (eposlovanje.hr) providers.

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

## DevExpress migration — authoritative plan (branch `DevEx-JamesBond`)

When working on the DevExpress migration, the file
`MarkDowns/DevExpress_Migration_V4.md` is the **AUTHORITATIVE plan**. The user
explicitly accepted V4 after deliberation between V2/V3/V4 alternatives — V4
is the ratified contract, not a draft.

**Strict rules for the AI assistant:**

1. **Before any strategic decision** (direction, ordering, type targets, scope
   of a phase), the assistant must:
   - Read the relevant V4 paragraph (§0–§5) explicitly.
   - Quote the paragraph in the response that proposes the action.
   - Confirm the proposed action matches the quoted paragraph.
   - If it does **not** match V4, STOP and ask the user for explicit
     authorization before proceeding. The deviation must also be recorded
     as a V4 amendment (new entry in §6 marked
     `V4-deviation — REQUIRES V4 amendment`).
2. **Tracker entries in V4 §6 do not override V4 §0–§5.** A tracker row
   describing an action that contradicts the strategic plan is a bug, not a
   decision. If found, alert the user and revert/rewrite the row before
   continuing.
3. **Tactical-execution detail** (which exact file to edit, which exact API
   call to use for an already-V4-approved swap, error handling within an
   approved phase) does **not** require pre-quote — only strategic decisions do.

## Git discipline

The AI assistant must **not** invoke mutate-history git commands without
explicit per-occasion authorization from the user in the same turn.

- **Forbidden without permission:** `git commit`, `git push`, `git reset --hard`,
  `git revert`, `git rebase`, `git cherry-pick`, `git stash drop`, `git tag`,
  branch deletion, force-push.
- **Permitted freely (read-only):** `git status`, `git log`, `git diff`, `git show`, `git for-each-ref`, `git branch -r --contains`.
- **Permitted as recovery only:** `git checkout -- <file>` and
  `git update-ref refs/backup/...` — solely to recover from an error the
  assistant itself made in the working tree, with the recovery purpose
  clearly stated.
- **Workflow:** the assistant prepares all edits in the working tree; the user
  reviews the diff (VS Git UI or `git diff`) and decides commit timing,
  granularity, and message.

## Build verification

When the assistant claims "build green", it should first attempt the **clean-then-build**
sequence (per V4_RESUME.md discipline rule #8), not incremental build:

```powershell
$msb = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
& $msb 'Vektor.csproj' /t:Clean /v:minimal /nologo
& $msb 'Vektor.csproj' /v:minimal /nologo /p:Configuration=Debug
```

Exit code `0` = green; anything else = red. If the CLI sequence is repeatedly
cancelled by the environment, the assistant may treat
Visual Studio `run_build` success or explicit user build validation as sufficient
for autonomous Phase 2 commits. In that fallback case, the tracker/commit note
must say `VS-build green` or `user-build green` instead of `clean-build green`.

## Response Formatting
- Format all responses as one continuous block (jedan window) to allow for easy application and copying of the entire response at once, avoiding fragmented multi-block responses.

## User Expectations
- Continue executing multi-step coding/migration tasks to completion without unnecessary pauses or requiring repeated 'nastavi' prompts, while still reporting concise progress. During V4 DevExpress Phase 2, only stop for real blockers such as failed validation, V4 strategic deviation, unsafe git history operations beyond allowed commits, or required human runtime smoke tests.
- Autonomous execution of every remaining DevExpress migration slice through the end of Phase 2 without waiting for repeated "nastavi" prompts, stopping only for real blockers, required authorization, or human runtime smoke tests.
- The assistant is granted permission to commit independently through the end of Phase 2 of the V4 DevExpress migration, while still following project build/validation discipline. This includes executing autonomous build-green atomic commits to streamline the migration process.
- Execute Phase 3 work autonomously in as few turns as possible, with minimal waiting for 'nastavi', and generally agree with implementation suggestions unless a true blocker appears.

## DevExpress Detach Behavior
- Closing a detached form should only close the document; returning a detached document to the main interface must be done through mouse interaction, not automatically on close.
- Detached forms should not display unnecessary top toolbars or menus.