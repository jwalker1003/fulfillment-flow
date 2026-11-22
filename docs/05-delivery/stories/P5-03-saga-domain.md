# P5-03 `OrderFulfillmentProcess` state machine

**Phase:** 5 · **Depends on:** P3-03 · **Requirements:** SAGA-1.1, SAGA-1.2, SAGA-1.3, SAGA-2.1, SAGA-3.1, SAGA-4.1, SAGA-4.2, SAGA-5.1, SAGA-6.1, SAGA-6.2, SAGA-6.3, SAGA-7.1 (state part) — all at unit level

## Goal
The saga's decisions are a pure, fully unit-tested state machine.

## Read first
- `docs/02-domain/saga.md` (**the** spec for this story)
- `docs/03-architecture/adr/0007-saga-process-manager.md`

## Scope
`FulfillmentFlow.Ordering.Domain/FulfillmentProcesses/`, Ordering.Domain.Tests

## Design constraint
Ordering.Domain must not reference Contracts (NFR-1). So the process consumes **domain-side input records** defined in Ordering.Domain (e.g. `StockReservedInput(MessageId, OrderLineId, ReservationId, Sku, LocationId, Quantity)`), and emits **outgoing message records** (`RequestPayment`, `RequestFulfillment(picks)`, `RequestRefund`, `StepChanged`, `SendMarkOrderPaid`, `SendMarkOrderShipped`, `SendCancelOrder(reason)`) in `PendingMessages`. The application handler (P5-04) translates contracts ↔ these records.

## Tasks (test-first)
1. `FulfillmentStep` enum; `OrderFulfillmentProcess.Start(OrderPlacedInput, now)`.
2. One `Handle(...)` method per input type implementing the state machine, the late/duplicate table, and the single refund rule from `saga.md`.
3. `History` entries (at, message type, step before/after, outcome Applied | Duplicate | Ignored, note).
4. `StepChanged` emitted on every step change.
5. Tests: one per transition in the state diagram, one per row of the late/duplicate table, and these scenarios: happy path; insufficient stock (no `RequestPayment` ever); payment failed; customer cancel in each non-terminal step (refund only when paid); expiry before and after payment; duplicate of every input; `PaymentCaptured` after `Cancelled`.

## Acceptance
All listed SAGA IDs have unit tests passing; Ordering.Domain still references only SharedKernel.

## Out of scope
Persistence, handler wiring (P5-04).

## Done when
Standard.
