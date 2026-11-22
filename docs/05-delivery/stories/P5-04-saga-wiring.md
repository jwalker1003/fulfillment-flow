# P5-04 Saga handler, internal commands, persistence

**Phase:** 5 · **Depends on:** P5-03, P4-03 · **Requirements:** SAGA-1 … SAGA-7 (wiring), NFR-3, ADR-0007

## Goal
The saga runs inside Ordering: it receives integration events, persists its state, and emits integration events and internal commands through Ordering's outbox.

## Read first
- `docs/02-domain/saga.md`
- `docs/03-architecture/adr/0007-saga-process-manager.md`
- `docs/04-contracts/integration-events.md` — Ordering section (internal commands)

## Scope
Ordering.Application (`FulfillmentProcesses/`), Ordering.Infrastructure, Application.Tests

## Tasks (test-first)
0. Create the Billing integration events in `FulfillmentFlow.Billing.Contracts` now (records only, from the contracts doc) so the saga can subscribe; Billing's behaviour comes in P6-03.
1. `IFulfillmentProcessRepository` (`Get(orderId)`, `Save` → moves `PendingMessages` to outbox: integration events as `Kind=Event`, internal commands as `Kind=Command`).
2. `OrderFulfillmentProcessHandler` implementing `IIntegrationEventHandler<T>` for: `OrderPlaced`, `StockReserved`, `ReservationFailed`, `ReservationExpired`, `PaymentCaptured` (Billing.Contracts), `PaymentFailed`, `ShipmentDispatched` (Fulfillment.Contracts), `OrderCancelled`. Translates contracts → domain inputs. Missing process (non-`OrderPlaced`) → ignore.
3. Internal command handlers: `MarkOrderPaid`, `MarkOrderShipped`, `CancelOrder` (internal; reuses the public cancel command handler). A command rejected by `Order` (e.g. cancel after shipped) is logged and recorded, not retried.
4. Outbox processor routes `Kind=Command` to `ICommandDispatcher`.
5. Persistence: `ordering_FulfillmentProcesses` (step, flags, end reason as columns; reservations, processed IDs, history as JSON columns), `Version` token. Migration.
6. Application test with in-memory bus: OrderPlaced → StockReserved ×N → `PaymentRequested` published.

## Acceptance
Saga transaction writes only the process row, outbox rows and inbox row (assert via EF change tracker in a test). Application tests pass.

## Out of scope
Full end-to-end (P5-06).

## Done when
Standard.
