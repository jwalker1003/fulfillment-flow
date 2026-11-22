# P2-03 Release, expire and confirm reservations

**Phase:** 2 · **Depends on:** P2-02 · **Requirements:** INV-3.1, INV-3.2, INV-3.3, INV-3.4

## Goal
Reservations can be released, expired by time, and confirmed so they stop expiring.

## Read first
- `docs/02-domain/contexts/inventory.md`
- `docs/03-architecture/adr/0017-reservation-expiry.md`

## Scope
Inventory.Domain, Inventory.Domain.Tests

## Tasks (test-first)
1. `ReleaseReservation(ReservationId, DateTimeOffset now)` → `Result`: unknown → `inventory.reservation-not-found`; not Active → success, no change, no event (INV-3.3); Active → Released + `ReservationReleased`.
2. `ExpireReservations(DateTimeOffset now)` → `Result`: every Active reservation with `ExpiresAt != null && ExpiresAt <= now` → Expired + `ReservationExpired` each.
3. `ConfirmReservation(ReservationId, DateTimeOffset now)` → `Result`: Active → `ExpiresAt = null` + `ReservationConfirmed` (no-op if already confirmed); not Active → `inventory.reservation-not-active`.
4. `HasExpiredReservations(DateTimeOffset now)` query helper (used by the expiry job's in-memory filter).

## Acceptance
INV-3.1–3.4 tests pass; tests include boundary `ExpiresAt == now` (expires) and confirmed reservations ignored by expiry.

## Out of scope
The background job (P4-05); releasing all reservations for an order (P5-05).

## Done when
Standard.
