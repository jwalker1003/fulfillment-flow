# P7-01 API host, error catalog, problem details

**Phase:** 7 · **Depends on:** P6-03 · **Requirements:** API-3, API-7 (admin dead letters), NFR-11 (exposure)

## Goal
The API host composes all modules, runs workers, and turns every `DomainError` into correct problem details.

## Read first
- `docs/04-contracts/error-catalog.md`
- `docs/04-contracts/openapi.yaml` — `ProblemDetails`, `/admin/dead-letters`
- `docs/03-architecture/architecture.md` — Errors, Background workers, Configuration keys

## Scope
`FulfillmentFlow.Api` (`Program.cs`, `Errors/`, `Endpoints/Admin*`), IntegrationTests (`Api/`)

## Tasks (test-first)
1. `Program.cs`: register all modules, outbox processors, expiry job, `AddProblemDetails`, OpenAPI, JSON camelCase + string enums, migrations on startup (Development/tests).
2. `ErrorCatalog` (code → status, title, requirement) exactly matching the doc; `ResultExtensions.ToHttpResult()` producing problem details with `code`, `requirement`, `traceId`, `type = urn:fulfillment-flow:error:{code}`.
3. Request validation (missing/invalid JSON fields) → 422 `api.validation-failed` with `errors`.
4. Global exception handler → 500 `api.unexpected-error` (logged).
5. `GET /admin/dead-letters`.
6. `WebApplicationFactory` fixture: in-memory SQLite, `Messaging:Enabled=false`, `FakeClock`, drainer exposed.
7. Test: every error code constant in the code base (reflection over `public const string` fields in `*Errors` classes) exists in `ErrorCatalog`, and vice versa.

## Acceptance
API-3 tests pass; catalog completeness test passes.

## Out of scope
Module endpoints (P7-02 … P7-04).

## Done when
Standard.
