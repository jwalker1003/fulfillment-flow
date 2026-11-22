# P3-03 Order lifecycle and cancel

**Phase:** 3 · **Depends on:** P3-02 · **Requirements:** ORD-3.1, ORD-3.2, ORD-3.3, ORD-3.4, ORD-4.1, ORD-4.2, ORD-4.3

## Goal
The order's status is a closed state machine inside the aggregate.

## Read first
- `docs/02-domain/contexts/ordering.md`

## Scope
Ordering.Domain, Ordering.Domain.Tests

## Tasks (test-first)
1. Private static transition table: Draft→Placed, Placed→Paid, Paid→Shipped, Draft/Placed/Paid→Cancelled. One private `TransitionTo(OrderStatus)` used by every method.
2. `MarkPaid()` → `OrderPaid`; `MarkShipped()` → `OrderShipped`; `Cancel(string reason)` → `ordering.cancellation-reason-required` if blank, else `OrderCancelled(reason, previousStatus)`.
3. Invalid transitions → `ordering.invalid-status-transition`, status unchanged.
4. Theory test over **every** (from, to) pair: valid pairs succeed, all others fail (ORD-3.1/3.2). Terminal states reject every method, including draft edits (ORD-3.3).

## Acceptance
Listed IDs pass. `Status` has no setter; no method other than `TransitionTo` assigns it.

## Out of scope
Saga.

## Done when
Standard.
