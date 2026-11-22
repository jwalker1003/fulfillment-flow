# P2-04 Commit a reservation

**Phase:** 2 · **Depends on:** P2-02 · **Requirements:** INV-4.1, INV-4.2, INV-4.3

## Goal
Shipping consumes a reservation: on-hand and reserved both drop.

## Read first
- `docs/02-domain/contexts/inventory.md`

## Scope
Inventory.Domain, Inventory.Domain.Tests

## Tasks (test-first)
1. `CommitReservation(ReservationId, DateTimeOffset now)` → `Result`:
   - Active → Committed; OnHand −= quantity; reserved drops automatically; `ReservationCommitted`.
   - Committed → success, no change, no event (INV-4.3).
   - Released/Expired → `inventory.reservation-not-active`.
   - Unknown → `inventory.reservation-not-found`.
2. Works for both confirmed and unconfirmed active reservations.

## Acceptance
INV-4.1–4.3 tests pass; invariant on-hand ≥ reserved holds after commit.

## Out of scope
Handling `ShipmentDispatched` (P5-05).

## Done when
Standard.
