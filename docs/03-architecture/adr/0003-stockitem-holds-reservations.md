# ADR-0003: `StockItem` holds its reservations

**Status:** Accepted

**Context.** Reserving must check on-hand, reserved and the reservation list, and change them atomically (INV-2).

**Decision.** `StockItem` (one SKU at one location) is the aggregate root; `Reservation` is an entity inside it. `Reserved` is derived from active reservations, never stored independently in the model.

**Alternatives.** `Reservation` as its own aggregate: needs cross-aggregate consistency to protect "available ≥ 0". One aggregate per SKU across all locations: larger, more contention, and choosing a location is policy, not an invariant.

**Consequences.** Releasing all reservations for an order touches several stock items, so it runs as one transaction per stock item (NFR-3), driven by the `OrderCancelled` handler; the reservations table is indexed on `OrderId`. Hot SKUs contend on one row; optimistic concurrency with retry handles it (NFR-10).

**Defense in one breath.** Reserve must check and change on-hand, reserved and the reservation list atomically. A separate aggregate would need cross-aggregate consistency.
