# P4-05 Reservation expiry job

**Phase:** 4 · **Depends on:** P4-02, P2-03 · **Requirements:** INV-3.2 (end to end), INV-2.5, ADR-0017

## Goal
Unconfirmed reservations past their expiry are released automatically and `ReservationExpired` is published.

## Read first
- `docs/03-architecture/adr/0017-reservation-expiry.md`

## Scope
Inventory.Application (`ExpireReservations` command handler), Inventory.Infrastructure (`ExpireReservationsJob`, repository query), IntegrationTests

## Tasks (test-first)
1. Repository query: IDs of stock items with any Active reservation where `ExpiresAt <= now` (index on `ExpiresAt`).
2. `ExpireReservations` command: for each ID, load, `ExpireReservations(now)`, save (one transaction each; concurrency conflict → skip, next run retries).
3. `ExpireReservationsJob` BackgroundService on `Inventory:ExpiryCheckIntervalSeconds`; disabled with messaging in tests.
4. Integration test with `FakeClock`: reserve, advance 16 min, run the command → reservation Expired, available restored, `ReservationExpiredIntegrationEvent` in outbox. Confirmed reservation not expired.

## Acceptance
Tests pass.

## Out of scope
Saga reaction to expiry (P5).

## Done when
Standard.
