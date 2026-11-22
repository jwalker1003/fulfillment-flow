# P3-05 Inventory reserves on `OrderPlaced` (in memory)

**Phase:** 3 · **Depends on:** P2-06, P3-02, P3-04 · **Requirements:** INV-2.6, INV-2.5 (application part), INV-1.3 (application part), NFR-2, NFR-3

## Goal
Placing an order reserves stock through integration events only. Phase 3 gate.

## Read first
- `docs/02-domain/contexts/inventory.md` — Application layer
- `docs/03-architecture/adr/0015-reservation-and-payment-sequence.md`
- `docs/04-contracts/integration-events.md` — Inventory section

## Scope
`FulfillmentFlow.Inventory.Contracts`, `FulfillmentFlow.Inventory.Application`, Application.Tests

## Tasks (test-first)
1. Inventory.Contracts: all Inventory integration events.
2. Inventory.Application: `IStockItemRepository` (`Get(sku, location)`, `GetCandidates(sku)`, `FindByOrderLine(orderLineId)`, `FindWithActiveReservationsForOrder(orderId)`, `Save`), `ILocationRepository`, `InventoryOptions` (reservation lifetime), `InventoryIntegrationEventMapper`.
3. Commands: `RegisterLocation`, `ReceiveStock` (creates stock item if missing; unknown location → `inventory.unknown-location`), `AdjustStock`, `SetReorderThreshold`.
4. `ReserveStockForOrder : IIntegrationEventHandler<OrderPlacedIntegrationEvent>`: for each line → if `FindByOrderLine` exists, skip; else load candidates with location coordinates, call `IAllocationStrategy`, `Reserve` on chosen item with `expiresAt = clock.UtcNow + lifetime`, save. On failure → publish `ReservationFailedIntegrationEvent(reasonCode)` and stop.
5. End-to-end in-memory test: register 2 locations, receive stock, create/place an order via Ordering commands, drain the bus → `StockReserved` per line at the nearest location; second order exceeding stock → `ReservationFailed` and earlier lines remain reserved (released later by P5-05).
6. Test that delivering the same `OrderPlaced` twice creates no extra reservations.

## Acceptance
INV-2.6 tests pass. Ordering.Application and Inventory.Application don't reference each other (architecture test).

## Out of scope
SQLite, outbox, saga.

## Done when
Standard. **Phase 3 gate** reviewed by Brad.
