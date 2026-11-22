# P4-01 SQLite persistence for Inventory and Ordering

**Phase:** 4 · **Depends on:** P3-05 · **Requirements:** NFR-1 (domain stays pure), NFR-8, NFR-10, ADR-0020

## Goal
Inventory and Ordering aggregates load and save through EF Core repositories against SQLite, with optimistic concurrency, without changing domain code.

## Read first
- `docs/03-architecture/architecture.md` — Persistence
- `docs/03-architecture/adr/0020-sqlite-module-tables.md`
- `docs/02-domain/contexts/inventory.md` and `ordering.md` — Persistence sections

## Scope
`FulfillmentFlow.BuildingBlocks.Infrastructure`, `FulfillmentFlow.Inventory.Infrastructure`, `FulfillmentFlow.Ordering.Infrastructure`, `tests/FulfillmentFlow.IntegrationTests/`

## Tasks (test-first)
1. BuildingBlocks.Infrastructure: `ModuleDbContext` base (table prefix convention, `decimal`/`DateTimeOffset` text converters, `Version` concurrency token convention for `AggregateRoot<>`), SQLite connection setup (WAL, busy_timeout), `SystemClock`.
2. `InventoryDbContext` with mappings for `Location`, `StockItem` (+ `Reservation` owned collection or child table, using backing fields), `StockCount`. Repositories implementing Inventory.Application interfaces. Unique `(Sku, LocationId)`, unique `(StockItemId, OrderLineId)`, index `OrderId`.
3. `OrderingDbContext` with `Order` (+ lines), `CatalogProduct`. Repositories.
4. EF migrations per module (`dotnet ef migrations add Initial --context ...`), applied at startup in Development/tests.
5. `AddInventoryModule` / `AddOrderingModule` DI extension methods.
6. Integration test fixture: SQLite in-memory shared connection, migrations applied, `FakeClock`.
7. Round-trip tests: every aggregate saved and reloaded equals the original state (including reservations, statuses, nullable expiry, shipping address).
8. Concurrency test: two contexts load the same `StockItem`, both reserve, second save fails with a concurrency exception mapped to `api.concurrency-conflict` result by the repository.

## Acceptance
Round-trip and concurrency tests pass. No domain project changed except adding private parameterless constructors if EF needs them (allowed; must be `private`).

## Out of scope
Outbox (P4-02) — in this story `Save` still publishes directly to the in-memory bus.

## Done when
Standard.
