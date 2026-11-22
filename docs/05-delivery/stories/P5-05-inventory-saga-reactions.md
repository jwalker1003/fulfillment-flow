# P5-05 Inventory reactions: release, confirm, commit

**Phase:** 5 · **Depends on:** P4-03 · **Requirements:** INV-3.5, INV-3.4 (end to end), INV-4.4

## Goal
Inventory compensates on cancellation, confirms paid reservations, and commits on dispatch.

## Read first
- `docs/02-domain/contexts/inventory.md` — Application layer
- `docs/02-domain/event-catalog.md` — Subscription matrix

## Scope
Inventory.Application, Inventory.Infrastructure (queries), Application.Tests, IntegrationTests

## Tasks (test-first)
1. `ReleaseOrderReservations : IIntegrationEventHandler<OrderCancelledIntegrationEvent>`: for each stock item with an active reservation for the order → release (one transaction each).
2. `ConfirmOrderReservations : …<FulfillmentRequestedIntegrationEvent>`: confirm each listed reservation (find stock item by reservation ID). Already not active → log, continue.
3. `CommitShipmentReservations : …<ShipmentDispatchedIntegrationEvent>`: commit each listed reservation.
4. Each handler is safe to run twice (domain no-ops) and writes its inbox row last.

## Acceptance
INV-3.4, 3.5, 4.4 tests pass, including duplicate delivery.

## Out of scope
Saga logic.

## Done when
Standard.
