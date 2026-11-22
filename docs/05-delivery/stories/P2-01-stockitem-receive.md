# P2-01 `StockItem` and receive

**Phase:** 2 · **Depends on:** P1-02 · **Requirements:** INV-1.1, INV-1.2, INV-1.3

## Goal
The `StockItem` aggregate exists with on-hand, reserved (empty), available, and can receive stock. `Location` reference data type exists.

## Read first
- `docs/02-domain/contexts/inventory.md` — Model, IDs, Rules in code
- `docs/02-domain/glossary.md`

## Scope
`src/Modules/Inventory/FulfillmentFlow.Inventory.Domain/` (folders `StockItems/`, `Locations/`, `Events/`), `tests/FulfillmentFlow.Inventory.Domain.Tests/`

## Tasks (test-first)
1. IDs: `StockItemId`, `ReservationId`, `LocationId` (code string), `OrderId`, `OrderLineId` (Inventory's own).
2. `Location` aggregate: `Register(LocationId, name, GeoPoint)`.
3. `StockItem.Create(Sku, LocationId)` with on-hand 0; `Receive(Quantity, DateTimeOffset now)` → `Result`.
4. Domain events: `StockReceived(StockItemId, Sku, LocationId, Quantity received, Quantity onHand, Quantity reserved, OccurredAt)`.
5. INV-1.3 at domain level: creating then receiving yields on-hand = received and one `StockReceived`. (The application's "create if missing" comes in P3-05/P4-01.)

## Acceptance
- INV-1.1, 1.2, 1.3 tests pass. Failing receive leaves state unchanged and raises no event.
- `Available` and `Reserved` are computed properties; no setters.

## Out of scope
Persistence, application services, INV-1.4 (application-level, P7-03).

## Done when
Standard.
