# ADR-0017: Reservation lifetime, expiry worker, confirmation

**Status:** Proposed. Resolves v1.0 gap 6.

**Decision.**
- Lifetime: `Inventory:ReservationLifetimeMinutes` (default 15). The application computes `expiresAt = clock.UtcNow + lifetime` and passes it to `Reserve`.
- `ExpireReservationsJob` (BackgroundService, every 30 s) finds stock items with active, unconfirmed reservations whose `ExpiresAt <= now` and calls `ExpireReservations(now)` on each, one transaction per stock item.
- On `FulfillmentRequested`, Inventory confirms each listed reservation: `ExpiresAt` becomes null, so paid orders in the warehouse never expire (INV-3.4).
- If a reservation expires anyway (race between payment and confirmation), the saga cancels and refunds (SAGA-5.1).

**Alternatives.** No expiry (abandoned flows lock stock forever); lazy expiry on the next reserve (unpredictable timing, hard to demo).

**Consequences.** Expiry also cleans up reservations leaked by a late `StockReserved` after cancellation (open question 3).

**Defense in one breath.** A reservation is a promise with a deadline; once the customer has paid and the warehouse is working on it, the deadline no longer applies.
