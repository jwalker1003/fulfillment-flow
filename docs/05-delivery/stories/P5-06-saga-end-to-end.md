# P5-06 Saga end-to-end tests

**Phase:** 5 · **Depends on:** P5-02, P5-04, P5-05 · **Requirements:** SAGA-1.1 … SAGA-7.1 (end to end) — Phase 5 gate

## Goal
Every saga path works across real modules on SQLite with drained outboxes.

## Read first
- `docs/02-domain/saga.md`
- `docs/06-quality/test-strategy.md` — Integration tests

## Scope
IntegrationTests (`Saga/`). Production code only for bugs found.

## Tasks
Billing is not built yet: a test helper `FakeBilling` publishes `PaymentCaptured`/`PaymentFailed` integration events into Billing-like outbox rows when it sees `PaymentRequested` (registered only in tests, removed in P6-03).

Scenarios (each asserts order status, saga step + end reason, stock levels, pick list state, and published messages):
1. **SAGA-1** happy path: place → reserved → paid → pick list → complete picks → dispatch → order Shipped, reservations Committed, on-hand reduced.
2. **SAGA-2** second line out of stock: order Cancelled "insufficient stock", first line's reservation Released, no `PaymentRequested` in any outbox.
3. **SAGA-3** payment fails: Cancelled "payment failed", all reservations Released.
4. **SAGA-4** customer cancels when Placed, and when Paid (pick list Cancelled, `RefundRequested` published).
5. **SAGA-5** reservation expires before payment (advance `FakeClock`, run expiry): Cancelled "reservation expired"; variant after payment → `RefundRequested`.
6. **SAGA-6** re-deliver every message of the happy path a second time (re-insert outbox rows with the same `MessageId`): identical final state; `PaymentCaptured` after cancellation → `RefundRequested`.
7. **SAGA-7** process history contains every step with timestamps and the end reason.

## Acceptance
All scenarios pass reliably.

## Done when
Standard. **Phase 5 gate** reviewed by Brad.
